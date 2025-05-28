using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;

public class GameEventManager : AttributesSync
{
    public GameObject StormPrefab;
    public GameObject DragonPrefab;
    public List<IBoardEvent> myPersistentBoardEvents = new List<IBoardEvent>();
    private bool myBool;
    [SynchronizableField]public bool mapPreviewing = true;
    [SynchronizableField]public bool shipMoving = false;
    public List<int> boardEvents = new List<int>();

    
    public bool GameTurn
    {
        get { return myBool; }
        set
        {
            if (value == myBool)
                return;

            myBool = value;
            if (myBool)
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

    public void GenerateBoardEvent()
    {
        int eventNumber = UnityEngine.Random.Range(0, 5);
        
        switch (eventNumber)
        {
            
            case 0:
                int mapNumber = UnityEngine.Random.Range(0, 52);
                if (transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>().myInteractables.Contains(MapPieceBehaviour.MapInteractables.Sirens))
                {
                    GenerateBoardEvent();
                    return;
                }
                BroadcastRemoteMethod("SpawnSirens", mapNumber);
                break;
            case 1:
                mapNumber = UnityEngine.Random.Range(0, 52);
                if (myPersistentBoardEvents.OfType<StormBehaviour>().Any())
                {
                    GenerateBoardEvent();
                    return;
                }
                BroadcastRemoteMethod("BroadcastSpawnStorm", mapNumber);
                break;
            case 2:
                mapNumber = UnityEngine.Random.Range(0, 52);
                if (transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>().myInteractables.Contains(MapPieceBehaviour.MapInteractables.Sirens))
                {
                    GenerateBoardEvent();
                    return;
                }
                BroadcastRemoteMethod("SpawnSirens", mapNumber);
                break;
            case 3:
                mapNumber = UnityEngine.Random.Range(0, 52);
                if (myPersistentBoardEvents.OfType<DragonBehaviour>().Any())
                {
                    GenerateBoardEvent();
                    return;
                }
                BroadcastRemoteMethod("SpawnDragon", mapNumber);
                break;   
            case 4:
                mapNumber = UnityEngine.Random.Range(0, 52);
                if (myPersistentBoardEvents.OfType<DragonBehaviour>().Any())
                {
                    GenerateBoardEvent();
                    return;
                }
                BroadcastRemoteMethod("SpawnDragon", mapNumber);
                break;   
            default: break;

        }
    }

    [SynchronizableMethod]
    private void SpawnSirens(int mapNumber)
    { 
        transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>().GenerateSirens();
    }

    [SynchronizableMethod]
    private void SpawnDragon(int mapNumber)
    {
        GameObject newDragon = Instantiate(DragonPrefab);
        DragonBehaviour Dragon = newDragon.GetComponent<DragonBehaviour>();
        Dragon.occupyingMapPiece = transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>();
        transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>().myInteractables.Add(MapPieceBehaviour.MapInteractables.Dragon);
        newDragon.transform.position = new Vector3(transform.GetChild(mapNumber).GetChild(0).position.x, newDragon.transform.position.y, transform.GetChild(mapNumber).GetChild(0).position.z);
        myPersistentBoardEvents.Add(Dragon);
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


}
