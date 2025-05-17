using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;

public class GameEventManager : AttributesSync
{
    public GameObject StormPrefab;
    public List<IBoardEvent> myPersistentBoardEvents = new List<IBoardEvent>();
   
    private bool myBool;
    [SynchronizableField]public bool mapPreviewing = true;
    [SynchronizableField]public bool shipMoving = false;

    
    public bool GameTurn
    {
        get { return myBool ; }
        set
        {
            if( value == myBool )
            return ;

            myBool = value ;
            if( myBool )
            {
                HandleAllBoardEvents();
                GameTurn = false;
            }    
        }    
    }
    public void HighlightHarbors(){
        foreach(Transform childMap in transform){
            if(childMap.GetComponent<MapPieceBehaviour>().myInteractables[0] == MapPieceBehaviour.MapInteractables.Harbor && childMap.GetComponent<MapPieceBehaviour>().occupyingShips.Count == 0){
                childMap.GetComponent<MapPieceBehaviour>().isHighlighted = true;
                childMap.GetComponent<MapPieceBehaviour>().GetComponent<MeshRenderer>().material = childMap.GetComponent<MapPieceBehaviour>().neighbouringTerrainMaterial;
            }
            
        }
    }
    public void DehighlightMaps(){
        foreach(Transform childMap in transform){ 
            childMap.GetComponent<MapPieceBehaviour>().isHighlighted = false;
            childMap.GetComponent<MapPieceBehaviour>().GetComponent<MeshRenderer>().material = childMap.GetComponent<MapPieceBehaviour>().myMaterial;         
        }
    }

    public void SpawnStorm(MapPieceBehaviour mapPieceBehaviour){
        int mapPieceIndex = mapPieceBehaviour.transform.GetSiblingIndex();
        BroadcastRemoteMethod("BroadcastSpawnStorm", mapPieceIndex);
    }

    [SynchronizableMethod]
    public void BroadcastSpawnStorm(int mapPieceIndex){
        MapPieceBehaviour mapPieceBehaviour = transform.GetChild(mapPieceIndex).GetComponent<MapPieceBehaviour>();
        GameObject SpawnedStorm = Instantiate(StormPrefab);
        StormBehaviour stormBehaviour = SpawnedStorm.GetComponent<StormBehaviour>();
        stormBehaviour.SpawnStormCloud(mapPieceBehaviour);
        myPersistentBoardEvents.Add(stormBehaviour);
        
    }
    public void HandleAllBoardEvents(){
        foreach(IBoardEvent persistentBoardEvent in myPersistentBoardEvents.Cast<IBoardEvent>())
        {
            persistentBoardEvent.ProcessMyTurn();
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)){            
          GameTurn = true;
        }
        if(Input.GetKeyDown(KeyCode.X)){            
          SpawnStorm(transform.GetChild(18).GetComponent<MapPieceBehaviour>());
        }
    }
}
