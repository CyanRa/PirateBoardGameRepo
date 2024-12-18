using System;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
public class FleetManager : CommunicationBridge
{
    public Alteruna.Avatar avatar;
    public LayerMask clickable;
    private List<string> listOfPlayers;
    public List<GameObject> myShips;
    public List<GameObject> selectedShips;
    public ShipSpawnerBehaviour MainSpawner;
    public Button endTurnButton;
    public GameObject shipPrefab;
    public GameObject MenuController;
    public GameObject MultiplayerSystem;
    private bool isMyTurn = false;    
    private bool isHost;
    public bool gameStarted = false;
    [SerializeField] TextMeshPro EndTurnText;
    
    //Sets you as host if you are first in the room, and grabs the menu and multiplayer objects
    public void Awake(){
        isHost = Multiplayer.Instance.Me.Index == 0;
        MenuController = GameObject.Find("MenuSystem");
        MultiplayerSystem = GameObject.Find("Multiplayer");
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        MainSpawner = GetComponent<ShipSpawnerBehaviour>(); 
    }
 
    void Update()
    {
    //Checks is the controlling avatar matches the 
       if(!avatar.IsMe && isMyTurn){return;}

    //Selecting ships
        if (Input.GetMouseButtonDown(0) && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){  

		    Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		    RaycastHit hit;

		    if( Physics.Raycast( ray, out hit, 2000, clickable)){
                    if(true){
                    SelectByClicking(hit.transform.gameObject);                                                
                    }                                             
            }else{
             DeselectAll();
            }  
        }       
    }
    
   
#region SHIPS
    public void DeselectAll(){                       
        selectedShips.Clear();
    }

    public void SelectByClicking(GameObject unit){   
        DeselectAll();
        selectedShips.Add(unit);
        EnableUnitMovement(unit, true);
        unit.GetComponent<Ship>().PlaySelectShipAudioClip();
    }

    //Movement enabling also reuglates highlighting and dehighlighting neighbouring terrains
    public void EnableUnitMovement(GameObject unit, bool shouldMove){   
        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == true){
            unit.GetComponent<Ship>().occupyingMapPiece.HighlightNeighbours();
        }

        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == false){
            unit.GetComponent<Ship>().occupyingMapPiece.DeHighlightNeighbours();
        }

        unit.GetComponent<Ship>().enabled = shouldMove;
        Debug.Log("Enabled movement of " + unit.transform.name);
    }


    public void DestroyShip(int shipIndex){
        myShips.RemoveAt(shipIndex);
    }

    public void AddShipToFleet(GameObject spawnedShip, bool isFlagship){
        spawnedShip.GetComponent<Ship>().fleetsAvatar = GetComponent<Alteruna.Avatar>();
        spawnedShip.GetComponent<Ship>().myFleet = GetComponent<FleetManager>();
        if(myShips.Count < 5 && !isFlagship){
            myShips.Add(spawnedShip);
            AddShipToPanelUI(spawnedShip);
        }else if(isFlagship){
            myShips.Add(spawnedShip);
            AddFlagShipToPanelUI(spawnedShip);         
        }else{
            Debug.Log("Ship limit reached for " + avatar.name);
        }
        
    }
    private void AddShipToPanelUI(GameObject newShip){
        int index = myShips.Count;
        MenuController.GetComponent<MenuBehaviour>().AddShipToUI(newShip, index);        
    }
    private void AddFlagShipToPanelUI(GameObject newShip){
        MenuController.GetComponent<MenuBehaviour>().AddFlagShipToUI(newShip);        
    }

    /*  //Checks if the passed on ship is in the fleet ship list
    private bool CheckPosessionOfShip(GameObject ship){    
        bool isMyShip = false;
        foreach(GameObject x in myShips){
            if(ship.transform.name == x.transform.name){
                isMyShip = true;
                return isMyShip;
            }
        }
        return isMyShip;
    }
    */
#endregion
#region GAME_TURNS
    public void EndTurn(){
        if(Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
              MenuController.GetComponent<MenuBehaviour>().BroadcastPassTurn();    
        }     
    }

    public void StartGame(){
        Debug.Log("StartGame method in fleet is invoked");
        List<User> myUsers = MultiplayerSystem.GetComponent<Multiplayer>().GetUsers();
        if(isHost){
            isMyTurn = true;
            MainSpawner.SpawnFlagShip();
            MenuController.GetComponent<MenuBehaviour>().BroadcastDisplayListOfPlayers(myUsers);      
        }else{
            MainSpawner.SpawnFlagShip();
        }                        
        //MenuController.GetComponent<MenuBehaviour>().BroadcastPassTurn(myUsers[0].Name); 
        gameStarted = true;                
    }
#endregion
}
