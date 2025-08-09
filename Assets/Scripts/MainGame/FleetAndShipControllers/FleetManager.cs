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
using System.Transactions;

public class FleetManager : AttributesSync
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
    public bool isMyTurn = false;    
    public bool isHost;
    public bool gameStarted = false;
    [SerializeField] TextMeshPro EndTurnText;
    [SerializeField]public string fleetColour;
    public Material shipMaterialColour;
    [SerializeField]public int myGold;
    public TextMeshProUGUI goldCount;
    [SerializeField]public int victoryPoints;
    public TextMeshProUGUI victoryPointsCount;
    [SerializeField]public int fleetPositionIndex;
    private Animator myAnimator;
    public Inventory myInventory;
    private GameEventManager gameEventManager;
    private VictoryPanelBehaviour myVictoryPanel;
    public bool victoryDecisionMade = false;
    public bool lastPlayer;
    public bool ok = false;
    
    [SerializeField]public bool immuneToStorm = false;
    public bool choosingStorm = false;
    RTS_Camera myCamera;
    [SerializeField]public string InitSpawnPoint = "";
    public Pointer myPointer;
    
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
        myVictoryPanel = GameObject.Find("VictoryPanel")?.GetComponentInChildren<VictoryPanelBehaviour>();
        myVictoryPanel.myFleet = GetComponent<FleetManager>();
        myVictoryPanel.gameObject.SetActive(false);
        myPointer = GameObject.Find("PointerObject").GetComponent<Pointer>();
        
        
    }

    public void Start(){
        if(!avatar.IsMe) return;

        victoryPointsCount = GameObject.Find("VictoryPointsCount").GetComponentInChildren<TextMeshProUGUI>();
        Button ShowCrewButton = GameObject.Find("ShowCrewButton").GetComponent<Button>();
        ShowCrewButton.onClick.AddListener(DisplayCrew);
        //Button ShowCrewButtonForPVE = GameObject.Find("ShowCrewButton").GetComponent<Button>();
        //ShowCrewButton.onClick.AddListener(() => DisplayCrewForPVE());
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
       if(choosingStorm) return;
       if(_fleetState == FleetControlState.SelectingMapPiece)return;

        if(Input.GetKeyDown(KeyCode.F) && myShips.Count < 5 && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){
           MainSpawner.SpawnShip();
        }

        if (Input.GetMouseButtonDown(0) && Multiplayer.Me.Name == MenuController.GetComponent<MenuBehaviour>().turnOwner){  
		    Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		    RaycastHit hit;

		    if( Physics.Raycast( ray, out hit, 2000, clickable)){
                if("Fleet ("+hit.transform.GetComponent<Ship>().myFleetName+")" == name){
                    if(SelectedShip != null){                    
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
        if (SelectedShip != null)
        {
            SelectedShip.GetComponent<Ship>().ChangeShipColour(fleetColour);
            SelectedShip.GetComponent<Ship>().occupyingMapPiece?.DeHighlightNeighbours();
            SelectedShip = null;
            _fleetState = FleetControlState.SelectingShip;
            
        }       
    }
    
    public void SelectByClicking(GameObject unit){
        MenuController.GetComponent<MenuBehaviour>().ResetInteractablePanel();
        if (!myShips.Contains(unit)) return;
        if(_fleetState != FleetControlState.SelectingShip)return;
        if(SelectedShip != null){
            DeselectAll();
        }
        SelectedShip = unit;
        if(unit.GetComponent<Ship>().hasRetal){
            MenuController.GetComponent<MenuBehaviour>().InstantiateInteractableButton("Retal", unit.GetComponent<Ship>(), unit.GetComponent<Ship>().occupyingMapPiece.transform);
        }
        SetNewShipForPath(unit.name);
        //BroadcastRemoteMethod("SetNewShipForPath", unit.name);
        shipMaterialColour = SelectedShip.GetComponent<Renderer>().material;
        SelectedShip.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
        unit.GetComponent<Ship>().EnableUnitMovement(unit, true);
        unit.GetComponent<Ship>().StartCoroutine(unit.GetComponent<Ship>().Co_PlaySelectShipAudioClip());
        _fleetState = FleetControlState.SelectingMapPiece;
    }

    [SynchronizableMethod]
    void SetNewShipForPath(string ship){       
        Transform _ship = GameObject.Find(ship).transform;
        Multiplayer.GetAvatar().GetComponent<FleetManager>().myPointer.SetupSpline(_ship);
        myPointer.startMapPieceName = _ship.GetComponent<Ship>().occupyingMapPieceName;
        myPointer.GetStartMapPiece();
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
            //WHAT IF LAST PLAYER DIES OR LEAVES THE GAME???
            if (lastPlayer)
            {
                gameEventManager.GameTurn = true;
                gameEventManager.GenerateBoardEvent();
            }               
        } 
        
        foreach(Transform map in gameEventManager.transform){
            map.GetComponent<MapPieceBehaviour>().HandleMapPieceStatus();
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
    public void DisplayCrewForPVE()
    { 
        MenuController.GetComponent<MenuBehaviour>().DisplayCrewForPVE(GetComponent<Hand>().myFleetCrew);
    }

    public void StartGame()
    {
        List<User> myUsers = MultiplayerSystem.GetComponent<Multiplayer>().GetUsers();

        InitEndTurnButton();
        GetComponent<Hand>().DrawNCards(5);

        if (isHost)
        {
            isMyTurn = true;
            MenuController.GetComponent<MenuBehaviour>().BroadcastDisplayListOfPlayers(myUsers);
        }


        gameStarted = true;
    }
    public void SpawnFlagShipsAtRandomSpawns(List<int> spawnPoints){
        InitSpawnPoint = MenuController.GetComponent<MenuBehaviour>().SpawnPoints[spawnPoints[Multiplayer.GetUser().Index]].name;
        Transform Spawn = GameObject.Find(InitSpawnPoint).transform.GetChild(0);
        MainSpawner.spawnPoint = Spawn;
        MainSpawner.SpawnFlagShip();
        myShips[0].GetComponent<Ship>().enabled = true;
    }

    #endregion

    public void EnterCombat(string attacker, string defender){
        foreach(GameObject ship in myShips){
            if(ship.name == attacker){
                Hand _myHand = GetComponent<Hand>();
                BattleManager _BattleManager = _myHand.BattleCanvas.transform.GetComponentInParent<BattleManager>();
                _BattleManager.attackingShip = attacker;
                EnterCombatAsAttacker(attacker, defender);
                
            }else if(ship.name == defender){
                EnterCombatAsDefender(defender);
            }else{
            }
        }
    }
    
    public IEnumerator SelectShipToAttack(Ship attacker){
        attacker.occupyingMapPiece.SetMyShipsSelectable();
        attacker.selectingShip = false;
        bool done = false;
        MenuController.GetComponent<MenuBehaviour>().DisplayInfoTab();
        DisplaySystem.SetAttDisplay(name,attacker.healthPoints.ToString(),attacker.shipGold.ToString(),attacker.damageBoost.ToString(),victoryPoints.ToString());
        while (!done)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<Ship>() != null)
                    {
                        if (hit.transform.GetComponent<Ship>().myFleet != GetComponent<FleetManager>() && hit.transform.GetComponent<Ship>().occupyingMapPiece == attacker.occupyingMapPiece)
                        {
                            attacker.occupyingMapPiece.SetMyShipsNonSelectable();
                            myCamera.targetFollow = null;
                            hit.transform.GetComponent<Ship>().WaitForDefenderShipReaction(attacker.myFleet.avatar.Owner.Index, attacker.name);
                            hit.transform.GetComponent<Ship>().ChangeShipColour(hit.transform.GetComponentInParent<FleetManager>().fleetColour);
                            done = true;
                            attacker.occupyingMapPiece.SetMyShipsNonSelectable();
                            
                        }
                    }
                }
            }
            yield return null;
        }  
    }


    public void EnterCombatAsAttacker(string attacker, string defender){
        MenuController.GetComponent<MenuBehaviour>().DisplayInfoTab();
        Hand _myHand = GetComponent<Hand>();
        _myHand.BattleCanvas.SetActive(true);
        _myHand.InstantiateHand();
        BattleManager _BattleManager = _myHand.BattleCanvas.transform.GetComponentInParent<BattleManager>();
        _BattleManager.PurgeDataOfFinishedBattle();
        _BattleManager.shipInCombat = GameObject.Find(attacker).GetComponent<Ship>();
        _BattleManager.attackingShip = attacker;
        _BattleManager.attackerUID = Multiplayer.GetUser().Index;
        _BattleManager.myHand = GetComponent<Hand>();
        _BattleManager.myTurnID = 1;
        _BattleManager.SetAttacker(avatar.name);
        _BattleManager.BroadcastSetTurnOwnerDisplay();
        Ship defendingShip = GameObject.Find(defender).GetComponent<Ship>();
        _BattleManager.defenderUID = defendingShip.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        Button _endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        _endCardTurnButton.onClick.AddListener(GetComponent<Hand>().EndCardTurn);        
        Ship _defenderShip = GameObject.Find(defender).GetComponent<Ship>();
        ushort defenderUID = _defenderShip.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        MapPieceBehaviour mapPiece = GameObject.Find(_defenderShip.occupyingMapPieceName).GetComponent<MapPieceBehaviour>();
        mapPiece.BroadcastBeginBattleDefender("", defender, defenderUID);
        _BattleManager.BroadcastInitializePrefabForDefender(defenderUID, _defenderShip.name);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
        
        if(_BattleManager.shipInCombat.isFlagship){
            //sets initial display of your power to 1 to reming the player that they have the bonus. Probably should move this to another class just like the whole method..
            _BattleManager.myPowerDisplay.GetComponentInChildren<TextMeshProUGUI>().text = 1.ToString();
        }
        _BattleManager.Commit();
    }

    
    public void EnterCombatAsDefender(string defender){
        MenuController.GetComponent<MenuBehaviour>().DisplayInfoTab();
        BattleManager _BattleManager = GetComponent<Hand>().BattleCanvas.transform.GetComponentInParent<BattleManager>();
        _BattleManager.PurgeDataOfFinishedBattle();
        _BattleManager.shipInCombat = GameObject.Find(defender).GetComponent<Ship>();
        _BattleManager.shipInCombat.hasRetal = true;
        _BattleManager.defenderUID = _BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().Possessor.Index;
        _BattleManager.myTurnID = 0;
        _BattleManager.SetDefender(_BattleManager.shipInCombat.GetComponentInParent<Alteruna.Avatar>().name); 
        _BattleManager.myHand = GetComponent<Hand>();
        Button endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        endCardTurnButton.onClick.AddListener(GetComponent<Hand>().EndCardTurn);
        _BattleManager.InvokeOpponentHandDisplay(GetComponent<Hand>().myFleetCrew.Count);
        _BattleManager.RequestInvokeOppHandDisplay();
        if(_BattleManager.shipInCombat.isFlagship){
            //sets initial display of your power to 1 to reming the player that they have the bonus. Probably should move this to another class just like the whole method..
            _BattleManager.myPowerDisplay.GetComponentInChildren<TextMeshProUGUI>().text = 1.ToString();
        }
        _BattleManager.defendingShip = defender;
        
        _BattleManager.Commit();
    }
  
    //Not in EnterCombatAsDefender to pre initialize
    public void InitDefenderBattleManager(){
        GetComponent<Hand>().BattleCanvas.SetActive(true);
        GetComponent<Hand>().InstantiateHand();
    }

    
    public void GetVictoryPoints(int _victoryPoints){
        victoryPoints += _victoryPoints;
        UpdateVictoryPointsDisplay();
    }

    public IEnumerator WaitForStormSelect(){
        yield return StartCoroutine(StormSelect());
    }

    public IEnumerator StormSelect(){   
        choosingStorm = true;      
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
                        choosingStorm = false;                
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

    public bool IsFlagshipAlive()
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
                        if(hit.transform.GetComponent<MapPieceBehaviour>().myInteractables[0] == MapPieceBehaviour.MapInteractables.Harbor && hit.transform.GetComponent<MapPieceBehaviour>().occupyingShips.Count == 0){
                            MainSpawner.SpawnShip(hit.transform.GetChild(0));
                            gameEventManager.DehighlightMaps();
                            done = true;
                        }
                    }
                }
            } 
        yield return null;
        }
    }
    public void GainFlagshipActionPoint(){
        foreach(GameObject ship in myShips){
            Ship _ship = ship.GetComponent<Ship>();

            if(_ship.isFlagship){
                _ship.actionPoints += 1;
                _ship.movementPoints +=1;
            }
        }
    }

    public void HandleCombatVictory(int damage, string ship, string shipb){
        Ship defendingShip = GameObject.Find(ship).GetComponent<Ship>();
        Ship attackingShip = GameObject.Find(shipb).GetComponent<Ship>();
        if(damage > 4 || defendingShip.healthPoints == 1){
            MenuController.GetComponent<MenuBehaviour>().DisplayVictoryPanel(true);
            defendingShip.occupyingMapPiece.BroadCastRemoveOccupyingShip(defendingShip.name);   
            StartCoroutine(WaitForVictoryDecision(defendingShip, attackingShip));            
        }else{
            switch(defendingShip.shipGold){
                case 1: attackingShip.GetGold(1);
                        defendingShip.GetGold(-1);
                        defendingShip.ChangeShipHealth(1);
                break;
                case 0: defendingShip.ChangeShipHealth(1);
                break;
                default:attackingShip.GetGold(2);
                        defendingShip.GetGold(-2); 
                        defendingShip.ChangeShipHealth(1);
                break;
            }
        }
        attackingShip.occupyingMapPiece.HandleMapPieceStatus();
    }

    private IEnumerator WaitForVictoryDecision(Ship defendingShip, Ship attackingShip){
        
        while(!victoryDecisionMade){              
            yield return null;
        }
        
            switch(myVictoryPanel.decision){
                case "Plunder":
                GetComponent<FleetManager>().GetVictoryPoints(3);
                attackingShip.GetGold(defendingShip.shipGold);
                defendingShip.ChangeShipHealth(2);
                myVictoryPanel.decision = "";
                MenuController.GetComponent<MenuBehaviour>().DisplayVictoryPanel(false);  
                victoryDecisionMade = false;            
                break;
                
                case "Capture":
                GetComponent<FleetManager>().GetVictoryPoints(1);
                MainSpawner.SpawnShip(attackingShip.occupyingMapPiece.transform.GetChild(0));
                myShips.Last().GetComponent<Ship>().GetGold(defendingShip.shipGold);
                myShips.Last().GetComponent<Ship>().ChangeShipHealth(1);
                defendingShip.ChangeShipHealth(2);
                myVictoryPanel.decision = "";
                MenuController.GetComponent<MenuBehaviour>().DisplayVictoryPanel(false);
                victoryDecisionMade = false;
                break;
                
                default: yield return null;
                break;  
            }     
    }
}
