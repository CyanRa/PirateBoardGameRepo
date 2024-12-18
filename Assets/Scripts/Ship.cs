using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using System.Data;
using System;
using UnityEditor;
using RTS_Cam;
using UnityEngine.UIElements;


public class Ship : MonoBehaviour
{
    //Branch change test
    PathCreator myPathCreator;
    public bool isMoving = false;
    public String Fleet;
    public int indexOfShip;
    public MapPieceBehaviour occupyingMapPiece;
    public FleetManager myFleet;
    public RTS_Camera myCamera;
    private Transform mapPieceAnchor;
    public float Speed = 10.0f;
    public float RotationSpeed = 180.0f;
    public LayerMask MovementLayer;
    public Alteruna.Avatar fleetsAvatar;
    public AudioSource myAudioSource;
    public AudioClip selectShipAudioClip;
    public AudioClip shipBellRingAudioClip;

    private void Awake(){
        myCamera = GameObject.Find("RTS_Camera").GetComponent<RTS_Camera>();        
    }
 
    void Update(){
    //Quit the update if using avatar is not the avatar of the fleet
        if(!fleetsAvatar.IsMe){
            return;
        }
    //Allows for a ship to move to any map piece before having one
    //Will be replaced by spawning logic                  
        if(occupyingMapPiece == null){
            if (Input.GetMouseButtonDown(1) && !isMoving ){        	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
		
		        if( Physics.Raycast( ray, out hit, 1000, MovementLayer ) ){		            
                    mapPieceAnchor = hit.transform.GetChild(0).transform;
                    occupyingMapPiece = hit.transform.GetComponent<MapPieceBehaviour>();
                    occupyingMapPiece.occupyingShip = GetComponent<Ship>();
                    isMoving = true;
                    Debug.Log( hit.transform.name );
                }    
            }     
        }else{
    //Movement when unit is selected and is registered to a map piece        
                if (Input.GetMouseButtonDown(1) && !isMoving ){        	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();
		
		        if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
		            
                    if(occupyingMapPiece.neighboringTerrain.Contains(hit.transform.GetComponent<MapPieceBehaviour>())){
                        OccupyMapPiece(false);
                        mapPieceAnchor = hit.transform.GetChild(0).transform;
                        occupyingMapPiece = hit.transform.GetComponent<MapPieceBehaviour>();
                        OccupyMapPiece(true);
                        isMoving = true;
                        gameObject.GetComponent<Ship>().PlayShipBellRingAudioClip();
                        Debug.Log( hit.transform.name );
                    }                    
                }  
        }else{
    //Deselection when left clicking
            if(Input.GetMouseButtonDown(0) && !isMoving){
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();

                if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
                    myFleet.DeselectAll();
                    myFleet.EnableUnitMovement(this.gameObject, false);
                }                
            }            
        }
    //Locks in movement until final position           
            if(isMoving){
                MoveToAnchor(mapPieceAnchor);
            }
        }
    }

    //TO BE REPLACED WITH SPLINE MOVEMENT
    public void MoveToAnchor(Transform transform){        
        if(GetComponent<Transform>().position.x != transform.position.x && GetComponent<Transform>().position.z != transform.position.z){
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, Speed*Time.deltaTime );
            GetComponent<Transform>().forward = mapPieceAnchor.position - GetComponent<Transform>().position;
            
            myCamera.targetFollow = transform;
            
        }else{
            Debug.Log("stopped moving");
            isMoving = false;
            myFleet.DeselectAll();
            myFleet.EnableUnitMovement(this.gameObject, false);
            myCamera.ResetTarget();
        }
        
    }

    //OCCUPY DE-OCCUPY MAP PIECES
    public void OccupyMapPiece(bool a){
        if(a){
            occupyingMapPiece.occupyingShip = GetComponent<Ship>();
        }else{
            occupyingMapPiece.occupyingShip = null;
        }
    }

    //SELECTING SHIPS FROM FLEET PANEL ICONS
    public void SelectShipFromItsIcon(GameObject shipToSelect){              
        myCamera.targetFollow = shipToSelect.transform;        
        myFleet.SelectByClicking(shipToSelect);
        shipToSelect.GetComponent<Ship>().PlaySelectShipAudioClip();       
    }

    public void PlaySelectShipAudioClip(){
        myAudioSource.PlayOneShot(selectShipAudioClip);
    }
    public void PlayShipBellRingAudioClip(){
        myAudioSource.PlayOneShot(shipBellRingAudioClip);
    } 
}
