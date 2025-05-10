using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Alteruna;
using System;
using JetBrains.Annotations;
using UnityEngine.Events;
using System.Data;
using RTS_Cam;
using System.Linq;

public class MapPieceBehaviour : AttributesSync
{   
    private static LTDescr delay;
    private static LTDescr mapPieceHighlightDelay;
    [SynchronizableField]public String occupyingShip = "";
    [SynchronizableField]public String occupyingFleet = "";
    [SerializeField]public Ship defenderShip = null;
    public List<Ship> occupyingShips;
    public List<MapPieceBehaviour> neighboringTerrain = new List<MapPieceBehaviour>();
    public bool areNeighboursHighlited = false;
    public bool allowTerrainHighlight = true;
    public bool isAttacker = false;
    public bool isHighlighted = false;
    private GameObject ScrollPrefab;
    private GameObject TreasurePrefab;
    public List<MapInteractables> myInteractables;

    public enum MapInteractables{
        Tavern,
        Harbor,
        PirateCove,
        Rumor,
        Treasure,
        Empty
        
    }
    public MapStatus myMapStatus;
    public enum MapStatus{
        Empty,
        Allied,
        Contested,
        Hostile
    }
    public bool isStorming = false;
    public bool movingStorm;
   

    [Header("MATERIALS")]
    public Material myMaterial; 
    public Material highLightedMaterial;
    public Material tempMaterial;
    public Material neighbouringTerrainMaterial;   
    public Material hostileNeighbouringTerrainMaterial; 
    public Material allyNeighbouringTerrainMaterial;
    public Material contestedNeighbouringTerrain;
    public Material treasureMaterial;

    private MenuBehaviour MenuSystem;
    public bool selectableMode;
    RTS_Camera myCamera;

    void Start()
    {
        myCamera = GameObject.Find("RTS_Camera_var1").GetComponent<RTS_Camera>();   
        MenuSystem = GameObject.Find("MenuSystem").GetComponent<MenuBehaviour>();
        occupyingShip = "";
        foreach(MapInteractables _interactable in myInteractables){
            if(_interactable == MapInteractables.Rumor){
                SpawnRumorScroll();
            }
        }
        if(myInteractables.Count == 0){
            myInteractables.Add(MapInteractables.Empty);
        }
        myMapStatus = MapStatus.Empty;
    }

    private void OnMouseDown()
    {
        TooltipSystem.SetAllignmentMiddle();
        TooltipSystem.Show(myInteractables[0].ToString());
        if(selectableMode){
            ReturnThisMapPiece();
        }
    }

    private MapPieceBehaviour ReturnThisMapPiece()
    {
        return GetComponent<MapPieceBehaviour>();
    }

    private void OnMouseEnter(){      
        if(allowTerrainHighlight){
        tempMaterial = GetComponent<MeshRenderer>().material;        
        GetComponent<MeshRenderer>().material = highLightedMaterial;
        }
        if(isHighlighted){
            GetComponent<Renderer>().material = highLightedMaterial;
        }       
    }

    private void OnMouseExit(){
        TooltipSystem.Hide();
        if(movingStorm){GetComponent<MeshRenderer>().material = allyNeighbouringTerrainMaterial;return;};
        if(!isHighlighted){
            ResetMaterial();
        }else{
            GetComponent<Renderer>().material = neighbouringTerrainMaterial;
        }
       
    }
    private void OnClicked(){
        
    }
    private string FormToolTipString(){
        string toolTipString = "";
        if(myMapStatus == MapStatus.Empty && myInteractables[0] == MapInteractables.Empty){
            toolTipString = "An empty area";
        }else{
            toolTipString = myMapStatus.ToString();
        }
        return toolTipString;
    }

    public void HighlightNeighbours(Ship unit){
        bool isBlocked = true;
        foreach(MapPieceBehaviour map in neighboringTerrain){
            map.areNeighboursHighlited = true;
            switch(map.myMapStatus){
                case MapStatus.Empty: 
                    map.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
                break;
                case MapStatus.Allied:
                    map.GetComponent<MeshRenderer>().material = allyNeighbouringTerrainMaterial;           
                break;
                case MapStatus.Contested:
                    map.GetComponent<MeshRenderer>().material = contestedNeighbouringTerrain;
                break;
                case MapStatus.Hostile:
                    map.GetComponent<MeshRenderer>().material = hostileNeighbouringTerrainMaterial;
                break;
                default:break;                
            }

                foreach(MapPieceBehaviour map2 in map.neighboringTerrain){
                    isBlocked = true;
                    foreach(MapPieceBehaviour connectingMapPiece in map2.neighboringTerrain){                      
                        if(neighboringTerrain.Contains(connectingMapPiece) && connectingMapPiece.myMapStatus != MapPieceBehaviour.MapStatus.Hostile){
                            isBlocked = false;
                        }
                    }
                    if(!isBlocked){
                        map2.areNeighboursHighlited = true;
                        switch(map2.myMapStatus){
                        case MapStatus.Empty: 
                            map2.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
                        break;
                        case MapStatus.Allied:
                            map2.GetComponent<MeshRenderer>().material = allyNeighbouringTerrainMaterial;           
                        break;
                        case MapStatus.Contested:
                            map2.GetComponent<MeshRenderer>().material = contestedNeighbouringTerrain;
                        break;
                        case MapStatus.Hostile:
                            map2.GetComponent<MeshRenderer>().material = hostileNeighbouringTerrainMaterial;
                        break;
                        default:break;               
                        }
                    }
                
                    
                }
            }
        GetComponent<MeshRenderer>().material =myMaterial;
        areNeighboursHighlited = true;
        allowTerrainHighlight = false;
    }
 
    public void DeHighlightNeighbours(){       
        foreach(MapPieceBehaviour map in neighboringTerrain){      
            map.areNeighboursHighlited = false;     
            map.GetComponent<MeshRenderer>().material = myMaterial;
            if(map.HasTreasure()){
                map.GetComponent<MeshRenderer>().material = treasureMaterial;
            }
            map.areNeighboursHighlited = false;
            foreach(MapPieceBehaviour map2 in map.neighboringTerrain){
                map2.areNeighboursHighlited = false;
                map2.GetComponent<MeshRenderer>().material = myMaterial;
                if(map2.HasTreasure()){map2.GetComponent<MeshRenderer>().material = treasureMaterial;}
                map2.areNeighboursHighlited = false;
            }
        } 
        allowTerrainHighlight = true;      
    }

   public void ResetMaterial(){
        GetComponent<MeshRenderer>().material = tempMaterial;
        if(areNeighboursHighlited == false && allowTerrainHighlight){
            GetComponent<MeshRenderer>().material = myMaterial;           
        }else if(HasTreasure()){
            GetComponent<MeshRenderer>().material = treasureMaterial;   
        }else if(areNeighboursHighlited){
            GetComponent<MeshRenderer>().material = tempMaterial;  
        }else{
            GetComponent<MeshRenderer>().material = myMaterial;  
        }  
   }
    public void EnterMapPiece(Ship enteringShip)
    {
        HandleStorm(enteringShip);
        enteringShip.occupyingMapPiece = GetComponent<MapPieceBehaviour>();
        ResetMaterial();        
        switch(myMapStatus){
            
            case MapStatus.Empty:         
            ConquerMapPiece(enteringShip);
            break;
            case MapStatus.Allied:         
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            BroadCastAddOccupyingShip(enteringShip.name);

            break;
            case MapStatus.Contested:
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            BroadCastAddOccupyingShip(enteringShip.name);
            WaitForShipToAttackSelect(enteringShip);

            break;
            case MapStatus.Hostile:           
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            SetMyShipsSelectable();
            BroadCastAddOccupyingShip(enteringShip.name);
            WaitForShipToAttackSelect(enteringShip);
            break;
            default:break;
        }
        GenerateInteractable(enteringShip);
    }

    //
    public void EnterMapPiece(Ship enteringShip, bool passiveEntry)
    {
        enteringShip.occupyingMapPiece = GetComponent<MapPieceBehaviour>();
        ResetMaterial();        
        switch(myMapStatus){
            
            case MapStatus.Empty:         
            ConquerMapPiece(enteringShip);
            break;
            case MapStatus.Allied:         
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            BroadCastAddOccupyingShip(enteringShip.name);

            break;
            case MapStatus.Contested:
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            BroadCastAddOccupyingShip(enteringShip.name);
            break;
            case MapStatus.Hostile:           
            enteringShip.offsetPosition[1] = FriendlyShipCount(enteringShip);
            BroadCastAddOccupyingShip(enteringShip.name);
            break;
            default:break;
        }
    }

    public void SetMyShipsSelectable(){
        foreach(Ship _ship in occupyingShips){
                _ship.selectingShip = true;
            }
    }  
    public void SetMyShipsNonSelectable(){
        foreach(Ship _ship in occupyingShips){
                _ship.selectingShip = false;
            }
    }  

    public void DestroyAllShips(){
        foreach(Ship ship in occupyingShips.ToList()){
            ship.ChangeShipHealth(2);
        }
    }
    public void HandleStorm(Ship enteringShip)
    {
       if(isStorming && enteringShip.isInsideStorm == false){
        if(enteringShip.myFleet.immuneToStorm != true){
            enteringShip.ChangeShipHealth(1);
        }           
        enteringShip.isInsideStorm = true;
        }else if(!isStorming && enteringShip.isInsideStorm == true){
            enteringShip.isInsideStorm = false;
        }
    }

    public int FriendlyShipCount(Ship enteringShip){
        int _friendlyShipCount = 0;
        foreach(Ship _ship in occupyingShips){
                if(_ship.myFleet == enteringShip.myFleet){
                    _friendlyShipCount +=1;
                }
            }
        return _friendlyShipCount;
    }
    public void ConquerMapPiece(Ship enteringShip){
        occupyingShip = enteringShip.name;
        SetMapPieceAllied();
        InvokeRemoteMethod("SetMapPieceHostile");
        BroadcastRemoteMethod("AddOccupyingShip", enteringShip.name);
    }
    public void BroadCastAddOccupyingShip(string enteringShip){
        BroadcastRemoteMethod("AddOccupyingShip", enteringShip);
    }
    public void BroadcastOccupyingMapPiece(Ship enteringShip){        
        BroadcastRemoteMethod("OccupyMapPiece", enteringShip.name);
        BroadcastRemoteMethod("SetOccupyingFleet", enteringShip.myFleet.name);
    }
    public void BroadCastRemoveOccupyingShip(string enteringShip){
        BroadcastRemoteMethod("RemoveOccupyingShip", enteringShip);
        
    }
    [SynchronizableMethod]
    private void RemoveOccupyingShip(string enteringShip){
        if(GameObject.Find(enteringShip)?.GetComponent<Ship>() == null)return;
        Ship _ship = GameObject.Find(enteringShip).GetComponent<Ship>();
        occupyingShips.Remove(_ship);  
        if(occupyingShips.Count == 0){
            SetMapPieceEmpty();
        }
    }
    public void HandleMapPieceStatus(){  
        BroadcastRemoteMethod("SynchMapStatus");     
    }
    [SynchronizableMethod]
    private void SynchMapStatus(){
        List<FleetManager> occupyingFleets = new List<FleetManager>();
        
        foreach(Ship _ship in occupyingShips){
            if(!occupyingFleets.Contains(_ship.GetComponentInParent<FleetManager>())){
                occupyingFleets.Add(_ship.GetComponentInParent<FleetManager>());
            }
        }
        foreach(FleetManager occfleet in occupyingFleets){
            Debug.Log("Map piece " + name + " is occupied by " + occfleet.name);
        }
        if(occupyingFleets.Count > 1 && !occupyingFleets.Contains(Multiplayer.GetAvatar().GetComponent<FleetManager>())){
            SetMapPieceHostile();
        }else if(occupyingFleets.Count > 1 && occupyingFleets.Contains(Multiplayer.GetAvatar().GetComponent<FleetManager>())){
            SetMapPieceContested();
        }else if(occupyingFleets.Count == 1){
            if(occupyingFleets[0] == Multiplayer.GetAvatar().GetComponent<FleetManager>()){
                SetMapPieceAllied();
            }else{
                SetMapPieceHostile();
            }
        }else if(occupyingFleets.Count == 0){
            SetMapPieceEmpty();
        }
    }
    [SynchronizableMethod]
    private void SetMapPieceAllied(){
        myMapStatus = MapStatus.Allied;
    }
    [SynchronizableMethod]
    private void SetMapPieceContested(){
        myMapStatus = MapStatus.Contested;
    }
    [SynchronizableMethod]
    private void SetMapPieceHostile(){
        myMapStatus = MapStatus.Hostile;
    }
    [SynchronizableMethod]
    private void SetMapPieceEmpty(){
        myMapStatus = MapStatus.Empty;
    }
    [SynchronizableMethod]
    private void AddOccupyingShip(string enteringShip){
        Ship _ship = GameObject.Find(enteringShip).GetComponent<Ship>();
        _ship.occupyingMapPiece = GetComponent<MapPieceBehaviour>();
        occupyingShips.Add(_ship);  
    }  
    [SynchronizableMethod]
    public void OccupyMapPiece(String enteringShip){ 
        occupyingShip = enteringShip; 
           
    }
    [SynchronizableMethod]
    public void SetOccupyingFleet(String enteringFleet){
        occupyingFleet = enteringFleet;
    }

    public void BroadCastBeginBattle(string attacker, string defender){
        int attackerID = Multiplayer.GetUser().Index;
        InvokeRemoteMethod("BeginBattle", (ushort)attackerID, attacker, defender);
    }
    public void WaitForShipToAttackSelect(Ship attacker){
        myCamera.targetFollow = occupyingShips[0].transform;
        myCamera.transform.LookAt(occupyingShips[0].transform);
        attacker.myFleet.StartCoroutine(attacker.myFleet.SelectShipToAttack(attacker));
    }
    public void BroadcastBeginBattleDefender(string attacker, string defender, ushort defenderID){
        InvokeRemoteMethod("BeginBattle", defenderID, attacker, defender);
    }
    [SynchronizableMethod]
    public void BeginBattle(string attacker, string defender){
       Multiplayer.GetAvatar().GetComponent<FleetManager>().EnterCombat(attacker, defender);
    }

    private void GenerateInteractable(Ship _enteringShip){
        FleetManager _fleet = _enteringShip.myFleet;
        MenuBehaviour _menuBehaviour = _fleet.MenuController.GetComponent<MenuBehaviour>();
        for(int i = 0; i < myInteractables.Count; i++){
            switch(myInteractables[i])
        {
            case MapInteractables.Empty: break;
            case MapInteractables.Harbor:
                _menuBehaviour.InstantiateInteractableButton("Harbor", _enteringShip, gameObject.transform);
                _menuBehaviour.InstantiateInteractableButton("Repair", _enteringShip, null);
                break;
            case MapInteractables.Tavern: 
                _menuBehaviour.InstantiateInteractableButton("Tavern", _enteringShip, null);
                break;
            case MapInteractables.PirateCove: 
                _menuBehaviour.InstantiateInteractableButton("PirateCove",_enteringShip, null);
                break;
            case MapInteractables.Rumor: 
                _menuBehaviour.InstantiateInteractableButton("Rumor", _enteringShip, ScrollPrefab.transform);
                break;
            case MapInteractables.Treasure:
                _menuBehaviour.InstantiateInteractableButton("Treasure", _enteringShip, gameObject.transform);
                break;
            default: break;
        }
        }
        
    }

    public void BroadcastGenerateRumor(){
        BroadcastRemoteMethod("GenerateRumor");
    }
    [SynchronizableMethod]
    public void GenerateRumor(){
        myInteractables.Add(MapInteractables.Rumor);
        SpawnRumorScroll();
    }
    public void BroadcastRemoveRumor(){
        BroadcastRemoteMethod("RemoveRumor");
    }
    [SynchronizableMethod]
    public void RemoveRumor(){
        myInteractables.Remove(MapInteractables.Rumor);
        Destroy(ScrollPrefab);
        if(myInteractables.Count == 0){
            myInteractables.Add(MapInteractables.Empty);
        }
    }
    public void GenerateTreasure(){
        GetComponent<MeshRenderer>().material = treasureMaterial;
        myInteractables.Add(MapInteractables.Treasure);
        SpawnTresureChest();
    }
    public void RemoveTreasure(){
        myInteractables.Remove(MapPieceBehaviour.MapInteractables.Treasure);
        Destroy(TreasurePrefab);
    }
    public void SpawnTresureChest(){
        TreasurePrefab = Instantiate(MenuSystem.treasureChestPrefab);
        TreasurePrefab.transform.position = this.transform.GetChild(0).transform.position;
        TreasurePrefab.transform.position = new UnityEngine.Vector3(TreasurePrefab.transform.position.x, TreasurePrefab.transform.position.y+4, TreasurePrefab.transform.position.z);
    }
    private bool HasTreasure()
    {
        foreach(MapInteractables _interactable in myInteractables){
            if(_interactable == MapInteractables.Treasure){
                return true;
            }
        }
        return false;
    }
    public bool HasRumor(){
        foreach(MapInteractables _interactable in myInteractables){
            if(_interactable == MapInteractables.Rumor){
                return true;
            }
        }
        return false;
    }
     private void SpawnRumorScroll(){
        ScrollPrefab = Instantiate(MenuSystem.rumorScrollPrefab);
        ScrollPrefab.transform.position = this.transform.GetChild(0).transform.position;
        ScrollPrefab.transform.position = new UnityEngine.Vector3(ScrollPrefab.transform.position.x, ScrollPrefab.transform.position.y+4, ScrollPrefab.transform.position.z);
    }
}
