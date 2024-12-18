using UnityEngine;

public class PathCreator : MonoBehaviour
{
    private float interpolateAmount;
    public float speed = 1;
   
     [SerializeField] public Transform pointA; 
     [SerializeField] public Transform pointB; 
     [SerializeField] public Transform pointC;
     [SerializeField] public Transform pointBC;
     [SerializeField] public Transform pointAB_BC;

     [SerializeField] public Transform pointAB; 
       void Update()
    {
        interpolateAmount = (interpolateAmount + Time.deltaTime*speed) %1f;

        pointAB.position = Vector3.Lerp(pointA.position, pointB.position, interpolateAmount);    
        pointBC.position = Vector3.Lerp(pointB.position, pointC.position, interpolateAmount);    
        pointAB_BC.position = Vector3.Lerp(pointAB.position, pointBC.position, interpolateAmount);    
    }
}
