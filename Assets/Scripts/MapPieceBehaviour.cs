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
    [SynchronizableField]public String occupyingShip = "";
    [SynchronizableField]public String occupyingFleet = "";
    [SerializeField]public Ship defenderShip = null;
    public List<MapPieceBehaviour> neighboringTerrain = new List<MapPieceBehaviour>();
    public bool areNeighboursHighlited = false;
    public bool allowTerrainHighlight = true;
    public bool isAttacker = false;
    public MapInteractables myInteractable;
    public enum MapInteractables{
        Tavern,
        Harbor,
        PirateCove,
        Rumor,
        Empty
    }
   

    [Header("MATERIALS")]
    public Material myMaterial; 
    public Material highLightedMaterial;
    private Material tempMaterial;
    public Material neighbouringTerrainMaterial;   
    public Material hostileNeighbouringTerrainMaterial; 
    public Material allyNeighbouringTerrainMaterial;

    void Start()
    {
        occupyingShip = "";
    }

    private void OnMouseEnter(){
        TooltipSystem.Show(myInteractable.ToString(), "Map piece contains");
        if(allowTerrainHighlight){
            tempMaterial = GetComponent<MeshRenderer>().material;        
            GetComponent<MeshRenderer>().material = highLightedMaterial;
        }
    }

    private void OnMouseExit(){
        TooltipSystem.Hide();
        GetComponent<MeshRenderer>().material = tempMaterial;

        if(areNeighboursHighlited == false && allowTerrainHighlight){
            GetComponent<MeshRenderer>().material = myMaterial;           
        }        
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
        }
        allowTerrainHighlight = false;
    }
 
    public void DeHighlightNeighbours(){       
        foreach(MapPieceBehaviour map in neighboringTerrain){           
            map.GetComponent<MeshRenderer>().material = myMaterial;
            map.areNeighboursHighlited = false;
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
        switch(myInteractable)
        {
            case MapInteractables.Empty: break;
            case MapInteractables.Harbor:
                _menuBehaviour.InstantiateInteractableButton("Harbor", _fleet, gameObject.transform);
                break;
            case MapInteractables.Tavern: 
                _menuBehaviour.InstantiateInteractableButton("Tavern", _fleet, null);
                break;
            case MapInteractables.PirateCove: 
                _menuBehaviour.InstantiateInteractableButton("PirateCove",_fleet, null);
                break;
            case MapInteractables.Rumor: break;
            default: break;
        }
    }
}
