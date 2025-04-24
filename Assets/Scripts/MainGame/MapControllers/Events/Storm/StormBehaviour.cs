using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Alteruna;
using RTS_Cam;

public class StormBehaviour : AttributesSync, IBoardEvent
{
    [SerializeField]private MapPieceBehaviour occupyingMapPiece;
    [SerializeField]private List<MapPieceBehaviour> StormingMapPieces;
    private RTS_Camera myCamera;
    bool isMoving;
    public LayerMask MovementLayer;

    private void Awake()
    {
        myCamera = GameObject.Find("RTS_Camera_var1").GetComponent<RTS_Camera>();   
    }
    public void SpawnStormCloud(MapPieceBehaviour mapPieceBehaviour){
       HandleOccupationByStorm(mapPieceBehaviour);
    }

    public void MoveStorm(MapPieceBehaviour _mapPieceToMoveTo){
        foreach(MapPieceBehaviour _mapPiece in StormingMapPieces.ToList()){
            _mapPiece.isStorming = false;
            StormingMapPieces.Remove(_mapPiece);
        }
        StartCoroutine(Co_MovingStorm(_mapPieceToMoveTo));   
    }

    
    public void ProcessMyTurn(){
        int tempRand = UnityEngine.Random.Range(1,StormingMapPieces.Count-1);
        BroadcastRemoteMethod("BroadcastProcessingTurn", tempRand);
    }
    [SynchronizableMethod]
    private void BroadcastProcessingTurn(int tempRand){
        HandleOccupationByStorm(StormingMapPieces[tempRand]);
    }

    private void HandleOccupationByStorm(MapPieceBehaviour mapPieceBehaviour){
        if(isMoving)return;
        MoveStorm(mapPieceBehaviour);
        occupyingMapPiece = mapPieceBehaviour;
        StormingMapPieces.Add(mapPieceBehaviour);
        mapPieceBehaviour.isStorming = true;

        
        foreach(MapPieceBehaviour map in mapPieceBehaviour.neighboringTerrain){
            StormingMapPieces.Add(map);
            map.isStorming = true;
            foreach(Ship ship in map.occupyingShips){
                map.HandleStorm(ship);
            }
        }
        
    }
    private IEnumerator Co_MovingStorm(MapPieceBehaviour _mapPieceToMoveTo){
        isMoving = true;
        Transform mapPieceAnchor = _mapPieceToMoveTo.transform.GetChild(0);
        GameObject tempObject = new GameObject("tempCloudAnchor");
        tempObject.transform.position = mapPieceAnchor.position;
        tempObject.transform.position = new Vector3(mapPieceAnchor.position.x, gameObject.transform.position.y, mapPieceAnchor.position.z);
        tempObject.transform.Rotate(-90,0,0);
        Transform tempAnchor = tempObject.transform;
        myCamera.targetFollow = transform;

        while(GetComponent<Transform>().position.x != tempAnchor.position.x && GetComponent<Transform>().position.z != tempAnchor.position.z){
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, tempAnchor.position, 5f*Time.deltaTime);
            GetComponent<Transform>().forward = tempAnchor.position - GetComponent<Transform>().position;
            GetComponent<Transform>().rotation = tempAnchor.rotation;
            yield return null;                       
        }  

        myCamera.targetFollow = null;    
        isMoving = false;  
        Destroy(tempObject);      
        yield return null;       
    }
    public void SelectStormForMovement(){
        StartCoroutine(WaitForMapPieceSelect());
    }

    public IEnumerator WaitForMapPieceSelect(){
        yield return StartCoroutine(MapPieceSelect());
    }
    public IEnumerator MapPieceSelect(){
        bool done = false;
        HighlightMapPiecesForSelection();
        while(done == false){
            if (Input.GetMouseButtonDown(1)){                   	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;		
		        if( Physics.Raycast( ray, out hit) ){
                    int mapPieceIndex = StormingMapPieces.IndexOf(hit.transform.GetComponent<MapPieceBehaviour>());	
                    DeHighlightMapPiecesForSelection();
                    BroadcastRemoteMethod("BroadcastProcessingTurn", mapPieceIndex); 	    
                    done = true;                                       
                }    
            }
        yield return null;
        }
              
    }

    public void HighlightMapPiecesForSelection(){
        foreach(MapPieceBehaviour _mapPiece in StormingMapPieces){
            if(_mapPiece != StormingMapPieces[0]){
                _mapPiece.movingStorm = true;     
                _mapPiece.GetComponent<MeshRenderer>().material = _mapPiece.allyNeighbouringTerrainMaterial;
            }         
        }
    }
    public void DeHighlightMapPiecesForSelection(){
        foreach(MapPieceBehaviour _mapPiece in StormingMapPieces){
                _mapPiece.movingStorm = false;
                _mapPiece.GetComponent<MeshRenderer>().material = _mapPiece.myMaterial;                               
        }
    }
}
