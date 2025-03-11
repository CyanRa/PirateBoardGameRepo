using System;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class FleetManager : CommunicationBridge
{
    public Alteruna.Avatar avatar;
    public LayerMask clickable;
    private List<string> listOfPlayers;
    public List<GameObject> myShips;
    public GameObject SelectedShip;
    public ShipSpawnerBehaviour MainSpawner;
    public Button endTurnButton;
    public GameObject shipPrefab;
    public GameObject MenuController;
    public GameObject MultiplayerSystem;
    private bool isMyTurn = false;    
    private bool isHost;
    public bool gameStarted = false;
    [SerializeField] TextMeshPro EndTurnText;
    //public int fleetColourID;
    [SerializeField]public string fleetColour;
    public Material shipMaterialColour;
    
    [SerializeField]public string Username = "";
    //Sets you as host if you are first in the room, and grabs the menu and multiplayer objects

    public void Awake(){
        isHost = Multiplayer.Instance.Me.Index == 0;
        MenuController = GameObject.Find("MenuSystem");
        MultiplayerSystem = GameObject.Find("Multiplayer");
        
        
        
        
       
        

        MainSpawner = GetComponent<ShipSpawnerBehaviour>(); 
    }

    public void Start(){
        if(avatar.IsMe){
        Button ShowCrewButton = GameObject.Find("ShowCrewButton").GetComponent<Button>();
        ShowCrewButton.onClick.AddListener(DisplayCrew);
        }
        
    }
    
    private void InitEndTurnButton(){
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
    }
 
    void Update()
    {
    //Checks is the controlling avatar matches the 
       if(!avatar.IsMe){return;}


        if(Input.GetKeyDown(KeyCode.F) && myShips.Count < 5 && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
           GetComponent<ShipSpawnerBehaviour>().SpawnShip();
           Debug.Log("SPAWNING CALLED");
        }
    //Selecting ships
        if (Input.GetMouseButtonDown(0) && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){  
		    Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		    RaycastHit hit;

		    if( Physics.Raycast( ray, out hit, 2000, clickable)){
                if(hit.transform.GetComponent<Ship>().myFleet.name == name){
                      if(SelectedShip != null)
                {
                    
                    DeselectAll();
                }
                    SelectByClicking(hit.transform.gameObject);                                                
                                                                 
            }else{
             DeselectAll();
            }  
            }else{
                return;
            }
              
        }
        if (Input.GetMouseButtonDown(1) && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){  
		    Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		    RaycastHit hit;

		    if( Physics.Raycast( ray, out hit, 2000, clickable)){
                if(SelectedShip != null)
                {
                    SelectedShip.GetComponent<Ship>().occupyingMapPiece.DeHighlightNeighbours();
                    DeselectAll();
                }
                    SelectByClicking(hit.transform.gameObject);                                                
                                                                 
            }else{
             DeselectAll();
            }  
        }
             
    }
    
   
#region SHIPS
    public void DeselectAll(){
        if(SelectedShip != null){
            SelectedShip.GetComponent<Ship>().ChangeShipColour(fleetColour);                   
            SelectedShip = null;
        }       
    }

    public void SelectByClicking(GameObject unit){   
        SelectedShip = unit;
        shipMaterialColour = SelectedShip.GetComponent<Renderer>().material;
        SelectedShip.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
        unit.GetComponent<Ship>().EnableUnitMovement(unit, true);
        unit.GetComponent<Ship>().PlaySelectShipAudioClip();
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
        }       
    }
    private void AddShipToPanelUI(GameObject newShip){
        int index = myShips.Count;
        MenuController.GetComponent<MenuBehaviour>().AddShipToUI(newShip, index);        
    }
    private void AddFlagShipToPanelUI(GameObject newShip){
        MenuController.GetComponent<MenuBehaviour>().AddFlagShipToUI(newShip);        
    }

#endregion
#region GAME_TURNS
    public void EndTurn(){
        if(Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
              MenuController.GetComponent<MenuBehaviour>().BroadcastPassTurn(); 
              isMyTurn = false;   
              GetComponent<Hand>().DrawCard();
        }     
    }
    public void StartTurn(){
        isMyTurn = true;
    }

    public void DisplayCrew(){
        MenuController.GetComponent<MenuBehaviour>().DisplayCrew(GetComponent<Hand>().myFleetCrew);
    }

    public void GetColourID(){
        Debug.Log("User name requesting color is: " + Multiplayer.GetUser());
        //fleetColourID = MenuController.GetComponent<MenuBehaviour>().GetColourID(Multiplayer.GetUser());        
    }

    public void StartGame(){
        List<User> myUsers = MultiplayerSystem.GetComponent<Multiplayer>().GetUsers();
        MainSpawner.InitSpawnPoint();
        InitEndTurnButton();
        GetComponent<Hand>().DrawNCards(5);
        

        if(isHost){
            isMyTurn = true;
            MenuController.GetComponent<MenuBehaviour>().BroadcastDisplayListOfPlayers(myUsers);                       
            MainSpawner.SpawnFlagShip();
                
        }        

        if(!isHost){
            MainSpawner.SpawnFlagShip();
        }
                                          
        //MenuController.GetComponent<MenuBehaviour>().BroadcastPassTurn(myUsers[0].Name); 
        gameStarted = true;                
    }

   
#endregion

    public void EnterCombat(string attacker, string defender){

    }

}
