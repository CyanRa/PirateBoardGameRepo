using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using System.Data;
using System;
using UnityEditor;
using RTS_Cam;
using UnityEngine.UIElements;


public class Ship : AttributesSync
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
        if(!fleetsAvatar.IsMe ){
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
                    //OccupyMapPiece(true);
                    occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
                    isMoving = true;
                }    
            }     
        }
    //Movement when unit is selected and is registered to a map piece        
            if (Input.GetMouseButtonDown(1) && !isMoving && occupyingMapPiece != null){        	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();
		
		        if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
		            
                    if(occupyingMapPiece.neighboringTerrain.Contains(hit.transform.GetComponent<MapPieceBehaviour>())){
                         MoveFromAMapPieceToAMapPiece(hit);
                    }                    
                }
            } 
        
    //Deselection when left clicking
            if(Input.GetMouseButtonDown(0) && !isMoving){
                
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();

                if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
                    myFleet.DeselectAll();
                    EnableUnitMovement(this.gameObject, false);
                }                
            }            
        
    //Locks in movement until final position           
            if(isMoving){
                MoveToAnchor(mapPieceAnchor);
            }
        
    }

    public void MoveFromAMapPieceToAMapPiece(RaycastHit _hit){
        OccupyMapPiece(false);
        mapPieceAnchor = _hit.transform.GetChild(0).transform;
        occupyingMapPiece = _hit.transform.GetComponent<MapPieceBehaviour>();
        occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
        occupyingMapPiece.defenderShip = GetComponent<Ship>();
        isMoving = true;
        gameObject.GetComponent<Ship>().PlayShipBellRingAudioClip();
                       
    }

    //TO BE REPLACED WITH SPLINE MOVEMENT
    public void MoveToAnchor(Transform transform){        
        if(GetComponent<Transform>().position.x != transform.position.x && GetComponent<Transform>().position.z != transform.position.z){
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, Speed*Time.deltaTime );
            GetComponent<Transform>().forward = mapPieceAnchor.position - GetComponent<Transform>().position;
            
            myCamera.targetFollow = transform;
            
        }else{
            isMoving = false;
            myFleet.DeselectAll();
            EnableUnitMovement(this.gameObject, false);
            myCamera.ResetTarget();
        }
        
    }

    //Movement enabling also reuglates highlighting and dehighlighting neighbouring terrains
    public void EnableUnitMovement(GameObject unit, bool shouldMove){   
        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == true){
            unit.GetComponent<Ship>().occupyingMapPiece.HighlightNeighbours(unit.GetComponent<Ship>());
        }

        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == false){
            unit.GetComponent<Ship>().occupyingMapPiece.DeHighlightNeighbours();
        }

        unit.GetComponent<Ship>().enabled = shouldMove;
    }
    
    //OCCUPY DE-OCCUPY MAP PIECES
    public void OccupyMapPiece(bool a){
        if(a){
            occupyingMapPiece.occupyingShip = transform.name;
        }else{
            occupyingMapPiece.occupyingShip = "";
        }
    }

    //SELECTING SHIPS FROM FLEET PANEL ICONS
    public void SelectShipFromItsIcon(GameObject shipToSelect){
        if(myFleet.Multiplayer.Me.Name == myFleet.MenuController.GetComponent<MenuBehaviour>().turnOwner){
           MoveCameraFromIconSelection(shipToSelect);
            myFleet.SelectByClicking(shipToSelect);
            shipToSelect.GetComponent<Ship>().PlaySelectShipAudioClip();  
        }                   
    }

    

    public void PlaySelectShipAudioClip(){
        myAudioSource.PlayOneShot(selectShipAudioClip);
    }
    public void PlayShipBellRingAudioClip(){
        myAudioSource.PlayOneShot(shipBellRingAudioClip);
    }

    //NEEDS SOME BUFFER 
    private void MoveCameraFromIconSelection(GameObject shipToSelect){
        myCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        myCamera.transform.position = new Vector3(shipToSelect.transform.position.x, 600, shipToSelect.transform.position.z);    
    }

    public void BroadcastChangeShipColour(int tempColourID){
        BroadcastRemoteMethod("ChangeShipColour", tempColourID);       
    }

    [SynchronizableMethod]
    public void ChangeShipColour(string tempColour){
        Renderer tempRenderer = gameObject.GetComponent<Renderer>();
        
         switch(tempColour)
            {
                case "Red": tempRenderer.material.SetColor("_BaseColor", Color.red); break;
                case "Blue": tempRenderer.material.SetColor("_BaseColor", Color.blue); break;
                case "Green": tempRenderer.material.SetColor("_BaseColor", Color.green); break;
                case "Yellow": tempRenderer.material.SetColor("_BaseColor", Color.yellow); break;
                default:print("psht"); break;
            }
    }
}
