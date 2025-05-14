using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Alteruna;

public class Pointer : AttributesSync
{
    [HideInInspector]
    public Transform objectPosition;
    private Transform playerPosition;
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
    

    public void FindSelectedObject(){
        objectKnot.Position = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z);
        playerKnot.Position = objectPosition.position;

        playerKnot.TangentOut = new float3(bendAmount, 0f, 1f);
        objectKnot.TangentIn = new float3(bendAmount, 0f, -1f);

        spline.SetKnot(0, playerKnot);
        spline.SetKnot(1, objectKnot);

        spline.SetTangentMode(0, mode: TangentMode.Mirrored, BezierTangent.Out);
        spline.SetTangentMode(1, mode: TangentMode.Mirrored, BezierTangent.In);

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
