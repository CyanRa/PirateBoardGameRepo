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
    public Material myMaterial;
    public Material highLightedMaterial;
    [Range (0f,1f)]
    public float alpha = 1f;
    public List<MapPieceBehaviour> neighboringTerrain = new List<MapPieceBehaviour>();
    public Transform heighlightedHight;
    private Renderer renderer;
    private Transform transform;
    private Material tempMaterial;
    public Material neighbouringTerrainMaterial;   
    public Material hostileNeighbouringTerrainMaterial;
    
    public Material allyNeighbouringTerrainMaterial;
    public bool areNeighboursHighlited = false;

    public bool allowTerrainHighlight = true;

    private System.Numerics.Vector3 verticalVector = new System.Numerics.Vector3(0f, 30f, 0f);
    void Start()
    {
        occupyingShip = "";
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
       // myMaterial.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
    }

    private void OnMouseEnter(){
        if(allowTerrainHighlight){
            tempMaterial = GetComponent<MeshRenderer>().material;        
            GetComponent<MeshRenderer>().material = highLightedMaterial;
        }
        
       // transform.position += new UnityEngine.Vector3(0,10, 0);
       // transform.localScale = new UnityEngine.Vector3(1.1f, 1.0f, 1.1f);
    }

    private void OnMouseExit(){
        GetComponent<MeshRenderer>().material = tempMaterial;

        if(areNeighboursHighlited == false && allowTerrainHighlight){
            GetComponent<MeshRenderer>().material = myMaterial;           
        }        
    //  transform.position += new UnityEngine.Vector3(0,-10, 0);
    //  transform.localScale = new UnityEngine.Vector3(0.9f, 1.0f, 0.9f);
    }

    //SHOULD BE RESTRUCTURED INTO A SWITCH CHECK AND POSSIBLY THINK OF ALLY SHIPS
    public void HighlightNeighbours(Ship unit){
        foreach(MapPieceBehaviour map in neighboringTerrain){
            map.areNeighboursHighlited = true;
            if(map.occupyingShip == ""){
                map.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
            }else{
                if(occupyingFleet != unit.myFleet.name){  
                    map.GetComponent<MeshRenderer>().material = hostileNeighbouringTerrainMaterial;
                }
                if(occupyingFleet == unit.myFleet.name){
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

    public void BroadcastOccupyingMapPiece(Ship enteringShip){
        
        BroadcastRemoteMethod("OccupyMapPiece", enteringShip.name);
        BroadcastRemoteMethod("SetOccupyingFleet", enteringShip.myFleet.name);
        Debug.Log(enteringShip.myFleet.name);
    }
    [SynchronizableMethod]
    public void OccupyMapPiece(String enteringShip){ 
        occupyingShip = enteringShip;
    }
    [SynchronizableMethod]
    public void SetOccupyingFleet(String enteringFleet){
        occupyingFleet = enteringFleet;
    }
}
