using System;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using RTS_Cam;
using UnityEditor;
public class FleetManager : CommunicationBridge
{
    public Alteruna.Avatar avatar;
    public LayerMask clickable;
    public LayerMask stormLayer;
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
    [SerializeField]public string fleetColour;
    public Material shipMaterialColour;
    [SerializeField]public int myGold;
    public TextMeshProUGUI goldCount;
    [SerializeField]public int victoryPoints;
    public TextMeshProUGUI victoryPointsCount;
    public int fleetPositionIndex;
    private Animator myAnimator;
    public Inventory myInventory;
    private GameEventManager gameEventManager;
    public bool lastPlayer;
    
    [SerializeField]public bool immuneToStorm = false;
    public bool choosingStorm;
    RTS_Camera myCamera;
    
    public enum FleetControlState{
        SelectingShip,
        SelectingMapPiece,
        ChoosingAction,
        InCombat
    }
    public FleetControlState _fleetState = FleetControlState.SelectingShip;

    public void Awake(){
        myCamera = GameObject.Find("RTS_Camera_var1")?.GetComponent<RTS_Camera>();   
        gameEventManager = GameObject.Find("Map Holder")?.GetComponent<GameEventManager>();
        isHost = Multiplayer.Instance.Me.Index == 0;
        MenuController = GameObject.Find("MenuSystem");
        MultiplayerSystem = GameObject.Find("Multiplayer");
        MainSpawner = GetComponent<ShipSpawnerBehaviour>();
        fleetPositionIndex = 0;   
        myAnimator = GameObject.Find("MainGameAnimator")?.GetComponentInChildren<Animator>();
		myAnimator?.SetTrigger("Start");  
        myInventory = GetComponent<Inventory>();   
    }

    public void Start(){
        if(!avatar.IsMe) return;

        victoryPointsCount = GameObject.Find("VictoryPointsCount").GetComponentInChildren<TextMeshProUGUI>();
        Button ShowCrewButton = GameObject.Find("ShowCrewButton").GetComponent<Button>();
        ShowCrewButton.onClick.AddListener(DisplayCrew);
        UpdateVictoryPointsDisplay();
        
        
    }
    public void UpdateVictoryPointsDisplay(){
        victoryPointsCount.text = victoryPoints.ToString();
    }
    
    private void InitEndTurnButton(){
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
    }
 
    void Update()
    {
        //Checks is the controlling avatar matches 
       if(!avatar.IsMe) return;
       if(_fleetState == FleetControlState.SelectingMapPiece)return;

        if(Input.GetKeyDown(KeyCode.F) && myShips.Count < 5 && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
           MainSpawner.SpawnShip();
        }

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
       
        if(Input.GetMouseButtonDown(1) && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){  
		    Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		    RaycastHit hit;

		    if( Physics.Raycast( ray, out hit, 2000, clickable)){
                if(SelectedShip != null)
                {
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
            SelectedShip.GetComponent<Ship>().occupyingMapPiece?.DeHighlightNeighbours();                      
            SelectedShip = null;
            _fleetState = FleetControlState.SelectingShip;
        }       
    }
    
    public void SelectByClicking(GameObject unit){
        if(_fleetState != FleetControlState.SelectingShip)return;
        if(SelectedShip != null){
            DeselectAll();
        }   
        SelectedShip = unit;
        shipMaterialColour = SelectedShip.GetComponent<Renderer>().material;
        SelectedShip.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
        unit.GetComponent<Ship>().EnableUnitMovement(unit, true);
        unit.GetComponent<Ship>().PlaySelectShipAudioClip();
        _fleetState = FleetControlState.SelectingMapPiece;
    }

    public void RemoveShip(GameObject ship){
        if(!myShips.Contains(ship))return;
        MenuController.GetComponent<MenuBehaviour>().RemoveShipFromUI(myShips.IndexOf(ship));
        myShips.Remove(ship);
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
            MenuController.GetComponent<MenuBehaviour>().ResetInteractablePanel(); 
            isMyTurn = false; 
            immuneToStorm = false;               
        } 
        if(Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().playersList[MenuController.GetComponent<MenuBehaviour>().playersList.Count -1]){
            gameEventManager.GameTurn = true;
        }    
    }
    public void StartTurn(){
        if(Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
            StartCoroutine(PlayStartTurnAnimation());
            isMyTurn = true;
            GetComponent<Hand>().DrawCard();
            fleetPositionIndex = MenuController.GetComponent<MenuBehaviour>().playersList.IndexOf(Multiplayer.GetUser().Name);  
            foreach(GameObject ship in myShips){
                ship.GetComponent<Ship>().movementPoints = 1;
                ship.GetComponent<Ship>().actionPoints = 1;
                ship.GetComponent<Ship>().UpdateShipDisplayIcon();
              }
        }       
    }

    IEnumerator PlayStartTurnAnimation(){
			myAnimator.SetTrigger("StartTurn");           
			yield return new WaitForSeconds(2);
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
        gameStarted = true;                    
    }

   
#endregion

    public void EnterCombat(string attacker, string defender){
        foreach(GameObject ship in myShips){
            if(ship.name == attacker){
                Debug.Log("ATTACKING SHIP REGISTERED", ship);
                EnterCombatAsAttacker(attacker, defender);
            }else if(ship.name == defender){
                EnterCombatAsDefender(defender);
                Debug.Log("DEFENDING SHIP REGISTERED", ship);
            }else{
            }
        }
    }
    public IEnumerator SelectShipToAttack(Ship attacker){
        
        bool done = false;
        while(!done){       
            if(Input.GetMouseButtonDown(0)){
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit; 
                if(Physics.Raycast(ray, out hit)){
                    if(hit.transform.GetComponent<Ship>() != null){
                        if(hit.transform.GetComponent<Ship>().myFleet != GetComponent<FleetManager>()){
                            //EnterCombatAsAttacker(attacker.name, hit.transform.name);
                            attacker.occupyingMapPiece.SetMyShipsNonSelectable();
                            hit.transform.GetComponent<Ship>().WaitForDefenderShipReaction(attacker.myFleet.avatar.Owner.Index, attacker.name);
                            hit.transform.GetComponent<Ship>().ChangeShipColour(hit.transform.GetComponentInParent<FleetManager>().fleetColour);
                            done = true;
                        }                                        
                    }
                }		            
            }
            yield return null;
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
        _BattleManager.BroadcastInitializePrefabForDefender(defenderUID, _defenderShip.name);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
        

    }

    
    public void EnterCombatAsDefender(string defender){
        GetComponent<Hand>().BattleCanvas.SetActive(true);
        GetComponent<Hand>().InstantiateHand();
        BattleManager _BattleManager = GetComponent<Hand>().BattleCanvas.transform.GetComponentInParent<BattleManager>();
        _BattleManager.shipInCombat = GameObject.Find(defender).GetComponent<Ship>();
        _BattleManager.defenderUID = _BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        _BattleManager.myTurnID = 0;
        _BattleManager.SetDefender(_BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().name); 
        _BattleManager.myHand = GetComponent<Hand>();
        Button endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        endCardTurnButton.onClick.AddListener(GetComponent<Hand>().EndCardTurn);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
    }

    
    public void GetVictoryPoints(int _victoryPoints){
        victoryPoints += _victoryPoints;
        UpdateVictoryPointsDisplay();
    }

    public IEnumerator WaitForStormSelect(){
        yield return StartCoroutine(StormSelect());
    }

    public IEnumerator StormSelect(){         
        bool done = false;
        while(!done){
        
            if(Input.GetMouseButtonDown(0)){
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit; 
                if(Physics.Raycast(ray, out hit)){
                    if(hit.transform.gameObject.GetComponent<StormBehaviour>() != null){
                        StormBehaviour storm = hit.transform.GetComponent<StormBehaviour>();
                        storm.SelectStormForMovement();
                        done = true;                 
                    }
                }		            
            }
            yield return null;
        }       
    }
    public IEnumerator WaitForMapPieceSelect(){
        yield return StartCoroutine(MapPieceSelect());
    }
    private IEnumerator MapPieceSelect(){
        if(!IsFlagshipAlive())yield break;
        bool done = false;

        while(!done){
            if (Input.GetMouseButtonDown(0)){                       	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
		
		        if( Physics.Raycast( ray, out hit)){ 
                    if(hit.transform.GetComponent<MapPieceBehaviour>()!=null){
                        foreach(GameObject ship in myShips){
                            if(ship.GetComponent<Ship>().isFlagship == true){
                                ship.GetComponent<Ship>().occupyingMapPiece.BroadCastRemoveOccupyingShip(ship.name);
                                ship.GetComponent<Ship>().enabled = true;
                                ship.GetComponent<Ship>().MoveToAMapPiece(hit.transform);
                                
                                ship.GetComponent<Ship>().isMoving = true;
                                done = true;                            
                            }
                        }
                    }
                }
            } 
        yield return null;
        }
    }

    private bool IsFlagshipAlive()
    {
        foreach(GameObject ship in myShips){
            if(ship.GetComponent<Ship>().isFlagship == true){
                return true;
            }
        }
        return false;
    }
    public void WaitForHarborSelect(){
        gameEventManager.HighlightHarbors();
        StartCoroutine(HarborSelect());
    }
    private IEnumerator HarborSelect(){
        bool done = false;

        while(!done){
            if (Input.GetMouseButtonDown(0)){                       	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
		
		        if( Physics.Raycast( ray, out hit)){ 
                    if(hit.transform.GetComponent<MapPieceBehaviour>()!=null){
                        if(hit.transform.GetComponent<MapPieceBehaviour>().myInteractables[0] == MapPieceBehaviour.MapInteractables.Harbor){
                            MainSpawner.SpawnShip(hit.transform.GetChild(0));
                            gameEventManager.DehighlightMaps();
                            hit.transform.GetComponent<MapPieceBehaviour>().isHighlighted = false;
                            done = true;
                        }
                    }
                }
            } 
        yield return null;
        }
    }

}
