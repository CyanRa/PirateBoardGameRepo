using UnityEngine;
using System.Collections.Generic;

public abstract class PersistentBoardEvent : MonoBehaviour
{
    private GameEventManager myGameEventManager;
    protected virtual void InitializePersistentBoardEvent(){
        myGameEventManager = GameObject.Find("Map Holder").GetComponent<GameEventManager>();
        //myGameEventManager.myPersistentBoardEvents.Add(this);
    }


}

public class StormPBE : PersistentBoardEvent
{
    private MapPieceBehaviour occupyingMapPiece;
    private List<MapPieceBehaviour> StormingMapPieces;
    protected override void InitializePersistentBoardEvent(){
        base.InitializePersistentBoardEvent();  
    }
}