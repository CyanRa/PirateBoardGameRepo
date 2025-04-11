using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StormBehaviour : MonoBehaviour
{
    [SerializeField]private MapPieceBehaviour occupyingMapPiece;
    [SerializeField]private List<MapPieceBehaviour> StormingMapPieces;
    private float Speed = 1.0f;
    [SerializeField]private MapPieceBehaviour mapPiece;
    [SerializeField]private MapPieceBehaviour mapPiece2;
    public void SetSpawnLocation(MapPieceBehaviour mapPieceBehaviour){
       HandleOccupationByStorm(mapPieceBehaviour);
    }

    public void MoveStorm(MapPieceBehaviour _mapPieceToMoveTo){
        foreach(MapPieceBehaviour _mapPiece in StormingMapPieces.ToList()){
            _mapPiece.isStorming = false;
            StormingMapPieces.Remove(_mapPiece);
        }
        StartCoroutine(Co_MovingStorm(_mapPieceToMoveTo));
        
        
    }
    private void HandleOccupationByStorm(MapPieceBehaviour mapPieceBehaviour){
        occupyingMapPiece = mapPieceBehaviour;
        StormingMapPieces.Add(mapPieceBehaviour);
        mapPieceBehaviour.isStorming = true;
        foreach(MapPieceBehaviour map in mapPieceBehaviour.neighboringTerrain){
            StormingMapPieces.Add(map);
            map.isStorming = true;
        }
    }
    private IEnumerator Co_MovingStorm(MapPieceBehaviour _mapPieceToMoveTo){
        Transform mapPieceAnchor = _mapPieceToMoveTo.transform.GetChild(0);
        GameObject tempObject = new GameObject("tempCloudAnchor");
        tempObject.transform.position = mapPieceAnchor.position;
        tempObject.transform.position = new Vector3(mapPieceAnchor.position.x, gameObject.transform.position.y, mapPieceAnchor.position.z);
        tempObject.transform.Rotate(-90,0,0);
        Transform tempAnchor = tempObject.transform;

        while(GetComponent<Transform>().position.x != tempAnchor.position.x && GetComponent<Transform>().position.z != tempAnchor.position.z){
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, tempAnchor.position, 0.5f*Time.deltaTime);
            //GetComponent<Transform>().forward = tempAnchor.position - GetComponent<Transform>().position;

            yield return null;                       
        }
        
        //GetComponent<Transform>().rotation = tempAnchor.rotation;
        HandleOccupationByStorm(_mapPieceToMoveTo);
        yield return null;
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)){            
          MoveStorm(mapPiece);
        }
    }
    public void Start()
    {
        HandleOccupationByStorm(mapPiece2);
    }
}
