using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Alteruna;

public class MapPieceBehaviour : AttributesSync
{   
    public Ship occupyingShip;
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
    public bool areNeighboursHighlited = false;

    private System.Numerics.Vector3 verticalVector = new System.Numerics.Vector3(0f, 30f, 0f);
    void Start()
    {
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
       // myMaterial.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
    }

    private void OnMouseEnter(){
        tempMaterial = GetComponent<MeshRenderer>().material;        
        GetComponent<MeshRenderer>().material = highLightedMaterial;
       // transform.position += new UnityEngine.Vector3(0,10, 0);
       // transform.localScale = new UnityEngine.Vector3(1.1f, 1.0f, 1.1f);
    }

    private void OnMouseExit(){
        GetComponent<MeshRenderer>().material = tempMaterial;

        if(areNeighboursHighlited == false){
            GetComponent<MeshRenderer>().material = myMaterial;           
        }        
    //  transform.position += new UnityEngine.Vector3(0,-10, 0);
    //  transform.localScale = new UnityEngine.Vector3(0.9f, 1.0f, 0.9f);
    }

    public void HighlightNeighbours(){
        foreach(MapPieceBehaviour map in neighboringTerrain){
            map.areNeighboursHighlited = true;
            map.GetComponent<MeshRenderer>().material = neighbouringTerrainMaterial;
        }
    }

  
    public void DeHighlightNeighbours(){       
        foreach(MapPieceBehaviour map in neighboringTerrain){           
            map.GetComponent<MeshRenderer>().material = myMaterial;
            map.areNeighboursHighlited = false;
        }       
    }
}
