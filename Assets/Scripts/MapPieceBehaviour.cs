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
    public List<String> interactables = new List<String>();
    public Material myMaterial; 
    public Material highLightedMaterial;
    private Renderer renderer;
    private Transform transform;
    private Material tempMaterial;
    public Material neighbouringTerrainMaterial;   
    public Material hostileNeighbouringTerrainMaterial; 
    public Material allyNeighbouringTerrainMaterial;
    public bool areNeighboursHighlited = false;
    public bool allowTerrainHighlight = true;

    void Start()
    {
        occupyingShip = "";
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
    }

    private void OnMouseEnter(){
        if(allowTerrainHighlight){
            tempMaterial = GetComponent<MeshRenderer>().material;        
            GetComponent<MeshRenderer>().material = highLightedMaterial;
        }
    }

    private void OnMouseExit(){
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
                Debug.Log(occupyingFleet + "      " + unit.myFleet.name);
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
            ShipsEnterCombat(enteringShip, defenderShip);           
        }
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

    public void ShipsEnterCombat(Ship attackerShip, Ship defenderShip){
        return;
    }
}
