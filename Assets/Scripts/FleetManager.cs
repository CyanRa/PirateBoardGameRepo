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
        if(!avatar.IsMe) return; // Guard Cluase??
        Button ShowCrewButton = GameObject.Find("ShowCrewButton").GetComponent<Button>();
        ShowCrewButton.onClick.AddListener(DisplayCrew);
        
    }
    
    private void InitEndTurnButton(){
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
    }
 
    void Update()
    {
        //Checks is the controlling avatar matches 
       if(!avatar.IsMe) return;

        if(Input.GetKeyDown(KeyCode.F) && myShips.Count < 5 && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
           MainSpawner.SpawnShip();
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
            }else
                {
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
              foreach(GameObject ship in myShips){
                ship.GetComponent<Ship>().movementPoints = 1;
              }
        }     
    }
    public void StartTurn(){
        isMyTurn = true;
    }

    public void DisplayCrew(){
        MenuController.GetComponent<MenuBehaviour>().DisplayCrew(GetComponent<Hand>().myFleetCrew);
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
        foreach(GameObject ship in myShips){
            if(ship.name == attacker){
                Debug.Log(attacker + " is the attacking ship");
                EnterCombatAsAttacker(attacker, defender);
            }else if(ship.name == defender){
                EnterCombatAsDefender(defender);
            }else{
                Debug.Log("Players are fighting. But not you");
            }
        }
    }
    public void EnterCombatAsAttacker(string attacker, string defender){
        Hand _myHand = GetComponent<Hand>();
        _myHand.BattleCanvas.SetActive(true);
        _myHand.InstantiateHand();
        BattleManager _BattleManager = _myHand.BattleCanvas.transform.GetComponentInParent<BattleManager>();
        _BattleManager.shipInCombat = GameObject.Find(attacker).GetComponent<Ship>();
        _BattleManager.attackerUID = Multiplayer.GetUser().Index;
        _BattleManager.myHand = GetComponent<Hand>();
        _BattleManager.myTurnID = 1;
        _BattleManager.SetAttacker(avatar.name);
        _BattleManager.BroadcastSetTurnOwnerDisplay();
        Button _endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        _endCardTurnButton.onClick.AddListener(GetComponent<Hand>().EndCardTurn);        
        Ship _defenderShip = GameObject.Find(defender).GetComponent<Ship>();
        ushort defenderUID = _defenderShip.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        MapPieceBehaviour mapPiece = GameObject.Find(_defenderShip.occupyingMapPieceName).GetComponent<MapPieceBehaviour>();
        mapPiece.BroadcastBeginBattleDefender("", defender, defenderUID);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
        _BattleManager.BroadcastInitializePrefabForDefender(defenderUID, _defenderShip.name);

    }

    public void EnterCombatAsDefender(string defender){
        GetComponent<Hand>().BattleCanvas.SetActive(true);
        GetComponent<Hand>().InstantiateHand();
        BattleManager _BattleManager = GetComponent<Hand>().BattleCanvas.transform.GetComponentInParent<BattleManager>();
        _BattleManager.shipInCombat = GameObject.Find(defender).GetComponent<Ship>();
        _BattleManager.defenderUID = _BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        //defender turnid set to 0
        _BattleManager.myTurnID = 0;
        _BattleManager.SetDefender(_BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().name); 
        _BattleManager.myHand = GetComponent<Hand>();
        Button endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        endCardTurnButton.onClick.AddListener(GetComponent<Hand>().EndCardTurn);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
    }

    

}
