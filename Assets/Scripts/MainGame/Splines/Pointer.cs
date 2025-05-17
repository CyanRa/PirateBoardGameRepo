using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Alteruna;
using System.Collections.Generic;
using System.Linq;

public class Pointer : AttributesSync
{
    [HideInInspector]
    public Transform objectPosition;
    public Transform playerPosition;
    [SynchronizableField] public string startMapPieceName ="";
    public MapPieceBehaviour startMapPiece;
    public float bendAmount;

    private Spline spline;
    private BezierKnot playerKnot;
    private BezierKnot objectKnot;

    public void Start()
    {
        spline = GetComponent<SplineContainer>().Spline;
    }
    public void SetupSpline(){       
        playerPosition = Multiplayer.GetAvatar().transform.GetChild(0);
        spline.Add(playerKnot);
        spline.Insert(1, objectKnot);
    }
    public void SetupSpline(Transform ship){      
        spline.Clear(); 
        playerPosition = ship;
        spline.Add(playerKnot);
        spline.Insert(1, objectKnot);
    }

    public void SetupSpline(Transform map, int index){      
        spline.Insert(index, objectKnot);
    }

    public void GetStartMapPiece(){
        startMapPiece = GameObject.Find(startMapPieceName).GetComponent<MapPieceBehaviour>();
    }
    

    public void FindSelectedObject(){
        objectKnot.Position = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z);
        playerKnot.Position = objectPosition.position;

        playerKnot.TangentOut = new float3(bendAmount, 0f, 0f);
        objectKnot.TangentIn = new float3(bendAmount, 0f, 0f);

        spline.SetKnot(0, playerKnot);
        spline.SetKnot(1, objectKnot);

        spline.SetTangentMode(0, mode: TangentMode.Mirrored, BezierTangent.Out);
        spline.SetTangentMode(1, mode: TangentMode.Mirrored, BezierTangent.In);

        GetComponent<SplineInstantiate>().enabled = true;
    }

    public void FindSelectedObject(List<MapPieceBehaviour> piecesToGoal){
        spline.Clear();
        int mapIndex = 0;
        
        foreach(MapPieceBehaviour map in piecesToGoal){
            spline.Add(piecesToGoal[mapIndex].transform.GetChild(0).position, TangentMode.Mirrored);
            mapIndex++;
        }

        GetComponent<SplineInstantiate>().enabled = true;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)){
            BroadcastRemoteMethod("HidePathFromDeselection");
        }
    }

    public void BroadCastHidePath(){
        BroadcastRemoteMethod("HidePathFromDeselection");
    }

    [SynchronizableMethod]
    void HidePathFromDeselection(){
        GetComponent<SplineInstantiate>().enabled = false;
    }
}
