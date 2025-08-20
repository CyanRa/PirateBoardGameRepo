using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class DragonBehaviour : AttributesSync, IBoardEvent
{
    public MapPieceBehaviour occupyingMapPiece;


    private IEnumerator FlyDragonTowards(Transform anchor)
    {
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        float timeOfAnimation = transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
        animator.enabled = false;
        while (!(transform.GetChild(0).position.x == anchor.position.x && transform.GetChild(0).position.z == anchor.position.z))
        {
            transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, anchor.position, 5f * Time.deltaTime);
            transform.GetChild(0).forward = anchor.position - transform.GetChild(0).position;
            
            yield return null;
        }
        animator.enabled = true;
        animator.Play("BlackDragonIdle", 0, timeOfAnimation);

    }

    public void ProcessMyTurn()
    {
        int rand = UnityEngine.Random.Range(0, occupyingMapPiece.neighboringTerrain.Count);
        BroadcastRemoteMethod("BroadcastFlyDragonTowards", rand);

    }

    private void HandleMapPieceEntry()
    {

    }

    [SynchronizableMethod]
    private void BroadcastFlyDragonTowards(int mapIndex)
    {
        StartCoroutine(FlyDragonTowards(occupyingMapPiece.neighboringTerrain[mapIndex].transform.GetChild(0)));
        occupyingMapPiece.myInteractables.Remove(MapPieceBehaviour.MapInteractables.Dragon);
        occupyingMapPiece = occupyingMapPiece.neighboringTerrain[mapIndex];
        occupyingMapPiece.myInteractables.Add(MapPieceBehaviour.MapInteractables.Dragon);
    }

    private void AttackShip(Ship ship)
    {

    }


    public void Die()
    {
        BroadcastRemoteMethod("BroadDie");
    }
    [SynchronizableMethod]
    private void BroadDie()
    {
        occupyingMapPiece.myInteractables.Remove(MapPieceBehaviour.MapInteractables.Dragon);
        Destroy(gameObject);
    }
    

}
