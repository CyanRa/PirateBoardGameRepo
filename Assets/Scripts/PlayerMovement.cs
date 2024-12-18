using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using System.Data;
using System;

public class PlayerController : CommunicationBridge
{
    PathCreator myPathCreator;
    private bool isMoving = false;
    public String Fleet;


    public Camera myCamera;
    private Transform mapPieceAnchor;
    public float Speed = 10.0f;
    public float RotationSpeed = 180.0f;

    private SpriteRenderer _renderer;

    public override void Possessed(bool isPossessor, User user)
    {
        enabled = isPossessor;
        
        if (isPossessor)
        {
           // _renderer = GetComponent<SpriteRenderer>();
            // Set the color sprite representing me to be green
           // _renderer.color = Color.green;
        }
    }

    // Only runs when possessed by me.
    void Update()
    {
         if (Input.GetMouseButtonDown(1) && !isMoving){
           
	
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;
		
		if( Physics.Raycast( ray, out hit, 1000 ) )
		{
            mapPieceAnchor = hit.transform.GetChild(0).transform;
            isMoving = true;
            Debug.Log( hit.transform.name );
        }
               
            //GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, 10f );
         }

         if(isMoving){
            MoveToAnchor(mapPieceAnchor);
            Debug.Log("Moving to " + mapPieceAnchor.name);
         }
        
    }

    private void MoveToAnchor(Transform transform){
        if(GetComponent<Transform>().position.x != transform.position.x && GetComponent<Transform>().position.z != transform.position.z){
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, 1f );
            GetComponent<Transform>().forward = mapPieceAnchor.position - GetComponent<Transform>().position;
                    }else{
            Debug.Log("stopped moving");
            isMoving = false;
        }
    }
    
}
