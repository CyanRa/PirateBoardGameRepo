using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Alteruna;
using System;

public class MapPieceBehaviour : AttributesSync
{   
    private static LTDescr delay;
    private static LTDescr mapPieceHighlightDelay;
    [SynchronizableField]public String occupyingShip = "";
    [SynchronizableField]public String occupyingFleet = "";
    [SerializeField]public Ship defenderShip = null;
    public List<MapPieceBehaviour> neighboringTerrain = new List<MapPieceBehaviour>();
    public bool areNeighboursHighlited = false;
    public bool allowTerrainHighlight = true;
    public bool isAttacker = false;
    private GameObject ScrollPrefab;
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
   

    [Header("MATERIALS")]
    public Material myMaterial; 
    public Material highLightedMaterial;
    private Material tempMaterial;
    public Material neighbouringTerrainMaterial;   
    public Material hostileNeighbouringTerrainMaterial; 
    public Material allyNeighbouringTerrainMaterial;
    public Material treasureMaterial;
    private MenuBehaviour MenuSystem;

    void Start()
    {
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

    private void SpawnRumorScroll(){
        ScrollPrefab = Instantiate(MenuSystem.rumorScrollPrefab);
        ScrollPrefab.transform.position = this.transform.GetChild(0).transform.position;
        ScrollPrefab.transform.position = new UnityEngine.Vector3(ScrollPrefab.transform.position.x, ScrollPrefab.transform.position.y+5, ScrollPrefab.transform.position.z);
    }

    private void OnMouseEnter(){
        delay = LeanTween.delayedCall(1f, ()=>{
            TooltipSystem.Show(myInteractables[0].ToString());
        });
       
        if(allowTerrainHighlight){
        tempMaterial = GetComponent<MeshRenderer>().material;        
        GetComponent<MeshRenderer>().material = highLightedMaterial;
        }       
    }

    private void OnMouseExit(){
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
        GetComponent<MeshRenderer>().material = tempMaterial;
        if(areNeighboursHighlited == false && allowTerrainHighlight){
            GetComponent<MeshRenderer>().material = myMaterial;           
        }
        if(HasTreasure()){
            GetComponent<MeshRenderer>().material = treasureMaterial;   
        }        
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

    public void HighlightNeighbours(Ship unit){
        foreach(MapPieceBehaviour map in neighboringTerrain){
            map.areNeighboursHighlited = true;
            if(map.occupyingShip == ""){
                map.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
            }else{
        
                if(map.occupyingFleet != unit.myFleet.name){  
                    map.GetComponent<MeshRenderer>().material = hostileNeighbouringTerrainMaterial;
                }
                if(map.occupyingFleet == unit.myFleet.name){
                    map.GetComponent<MeshRenderer>().material = allyNeighbouringTerrainMaterial;
                }
            }
            foreach(MapPieceBehaviour map2 in map.neighboringTerrain){
                map2.areNeighboursHighlited = true;
                if(map2.occupyingShip == ""){
                map2.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
            }else{
        
                if(map2.occupyingFleet != unit.myFleet.name){  
                    map2.GetComponent<MeshRenderer>().material = hostileNeighbouringTerrainMaterial;
                }
                if(map2.occupyingFleet == unit.myFleet.name){
                    map2.GetComponent<MeshRenderer>().material = allyNeighbouringTerrainMaterial;
                }
            }
            }      
        }
        allowTerrainHighlight = false;
    }
 
    public void DeHighlightNeighbours(){       
        foreach(MapPieceBehaviour map in neighboringTerrain){           
            map.GetComponent<MeshRenderer>().material = myMaterial;
            map.areNeighboursHighlited = false;
            foreach(MapPieceBehaviour map2 in map.neighboringTerrain){
                map2.GetComponent<MeshRenderer>().material = myMaterial;
                map2.areNeighboursHighlited = false;
            }
        } 
        allowTerrainHighlight = true;      
    }


    public void EnterMapPiece(Ship enteringShip)
    {
        if(occupyingFleet == ""){
            BroadcastOccupyingMapPiece(enteringShip);
        }else if(occupyingFleet == enteringShip.myFleet.name){
            return;
        }else{
            BroadCastBeginBattle(enteringShip.name, occupyingShip);  
            BroadcastOccupyingMapPiece(enteringShip);   
        } 
        GenerateInteractable(enteringShip);
    }
    public void BroadcastOccupyingMapPiece(Ship enteringShip){        
        BroadcastRemoteMethod("OccupyMapPiece", enteringShip.name);
        BroadcastRemoteMethod("SetOccupyingFleet", enteringShip.myFleet.name);
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
    }
    public void RemoveTreasure(){
        myInteractables.Remove(MapPieceBehaviour.MapInteractables.Treasure);
    }
}
