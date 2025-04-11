using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public GameObject StormPrefab;
    public List<PersistentBoardEvent> myPersistentBoardEvents;

    public void SpawnStorm(MapPieceBehaviour mapPieceBehaviour){
        GameObject SpawnedStorm = Instantiate(StormPrefab);
        StormBehaviour stormBehaviour = SpawnedStorm.GetComponent<StormBehaviour>();
        stormBehaviour.SetSpawnLocation(mapPieceBehaviour);
    }
}
