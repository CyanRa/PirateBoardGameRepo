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
        Debug.Log("HANDLING TURN");
        foreach(IBoardEvent persistentBoardEvent in myPersistentBoardEvents.Cast<IBoardEvent>())
        {
            Debug.Log("PROCESSING CLOUD ");
            persistentBoardEvent.ProcessMyTurn();
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)){            
          GameTurn = true;
        }
        if(Input.GetKeyDown(KeyCode.X)){            
          SpawnStorm(transform.GetChild(5).GetComponent<MapPieceBehaviour>());
        }
    }
}
