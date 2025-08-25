using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alteruna;
using NUnit.Framework;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : AttributesSync
{
    
    bool menuOpen = false;
    bool StopOnMouseOn = false;
#region GAMEOBJECTS
    public GameObject ShipDisplayPrefab;
    public GameObject Map;
    public GameObject FlagShipDisplayPrefab;
    public GameObject PreGameMenu;
    public GameObject InfoTab;
    public Spawner mySpawner;
    public GameObject InteractablesChoicePanel;
    public GameObject MultiplayerPanel;
    public GameObject MenuPanel;
    public GameObject FleetPanel;
    public GameObject ActionBar;
    public GameObject StopOnMousePlane;
    public GameObject MultiplayerSystem;
    public GameObject CrewDisplayPanel;
    public GameObject CrewDisplayForPVE;
    public GameObject crewMemberPrefab;
    public GameObject crewMemberPrefabPvE;
    public GameObject InteractablePanelPrefab;
    public GameObject InteractableButtonPrefab;
    public GameObject rumorScrollPrefab;
    public GameObject sirensPrefab;
    public GameObject piratePrefab;
    public GameObject treasureChestPrefab;
    public Sprite HarbourButtonSprite;
    public Sprite RepairButtonSprite;
    public Sprite UpgradeButtonSprite;
    public Sprite RumorButtonSprite;
    public Sprite PirateCoveButtonSprite;
    public Sprite AltPirateCoveButtonSprite;
    public Sprite TreasureButtonImage;
    public Sprite SirensButtonImage;
    public Sprite DragonButtonImage;
    public Sprite RetalButtonImage;
    public Sprite LeaveButtonImage;
    public Sprite CantLeaveButtonImage;
    public Sprite PiratesButtonImage;
    public Sprite TavernButtonImage;
    public Sprite AltTavernButtonImage;
    public Sprite PvPButtonImage;
    public GameObject consumablePanel;
    public GameObject defendingShipOptionsPanel;
    public GameObject victoryPanel;
    public List<Transform> SpawnPoints = new List<Transform>();
    List<int> spawnPoints = new List<int>();
    private GameEventManager gameEventManager;
    
    
    
#endregion
    public Button StartGameButton;
    public string turnOwner;
    [SerializeField]public List<string> playersList;
    
    [SerializeField]public TextMeshProUGUI TurnDisplayText;
    [SerializeField]public TextMeshProUGUI UserDisplayText;


    void Start()
    {
        StartGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        StartGameButton.onClick.AddListener(BroadCastTriggerStartGame);
        gameEventManager = GameObject.Find("Map Holder").GetComponent<GameEventManager>();
 
        
    }
    public void ShowConsumablePanel(){
        if(consumablePanel.activeSelf == true){
            consumablePanel.GetComponent<ConsumableMenuBehaviour>().HideConsumableInspector();
            consumablePanel.GetComponent<ConsumableMenuBehaviour>().DeleteConsumables();
            consumablePanel.SetActive(false);
        }else{
            consumablePanel.SetActive(true);
            consumablePanel.GetComponent<ConsumableMenuBehaviour>().InstantiateConsumables(Multiplayer.GetAvatar().GetComponent<Inventory>().myConsumables);           
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ){    
            OpenMenu();
        }    
    }

    public void ChooseColour(Button _button){
        FleetManager _fleet = Multiplayer.GetAvatar().GetComponent<FleetManager>();
        
        if (_fleet.fleetColour == "")
        {
            BroadcastRemoteMethod("LockInFleetColour", _button.gameObject.name, _fleet.name);
        }       
    }

    [SynchronizableMethod]
    public void LockInFleetColour(string _colour, string _avatarName){
        
        FleetManager _fleet = GameObject.Find(_avatarName).GetComponent<FleetManager>();
        _fleet.fleetColour = _colour;
        if (_fleet.isHost)
            { 
                
            } 
        Destroy(GameObject.Find(_colour));
    }
    

    public void ToggleMultiplayerPanel(){     
        if(MultiplayerPanel.activeInHierarchy){
            MultiplayerPanel.SetActive(false);
           
        }else{
             MultiplayerPanel.SetActive(true);
        }
        
    }
    public void QuitGame(){
        Application.Quit();
    }

    public void OpenMenu(){
        MenuPanel.SetActive(!menuOpen);
        MenuPanel.SetActive(!StopOnMouseOn);
        menuOpen = !menuOpen;
        StopOnMouseOn = !StopOnMouseOn;    
    }

    public void BroadcastPassTurn(){
        BroadcastRemoteMethod("PassTurn");        
    }


    public void BroadCastPassTurn(bool b){
    }
    public void DisplayCrew(List<CrewMember> myFleetCrew){

        if(CrewDisplayPanel.activeSelf == false){
            CrewDisplayPanel.SetActive(true);
            int i = 0;
            foreach (CrewMember ownedCrewMember in myFleetCrew)
            {
            GameObject _crewMember = Instantiate(crewMemberPrefab);
            CMBehaviour _cMBehaviour = _crewMember.GetComponent<CMBehaviour>();
            _cMBehaviour.crewMember = myFleetCrew[i];
            _cMBehaviour.LoadCardDisplay();
            _cMBehaviour.enabled = false;
            _crewMember.GetComponent<Button>().enabled = false;
            _crewMember.transform.SetParent(CrewDisplayPanel.transform);
            
            i ++;
            }
        }else{
            foreach (Transform _card in CrewDisplayPanel.transform)
            {
                Destroy(_card.gameObject);
            }
            CrewDisplayPanel.SetActive(false);
        }
    }

    [SynchronizableMethod]
    public void DisplayInfoTab()
    {
        if (InfoTab.activeSelf)
        {
            InfoTab.SetActive(false);
        }
        else
        {

            InfoTab.SetActive(true);
        }
    }
    public void DisplayCrewForPVE(List<CrewMember> myFleetCrew)
    {

        if (CrewDisplayForPVE.activeSelf == false)
        {
            CrewDisplayForPVE.SetActive(true);
            int i = 0;
            foreach (CrewMember ownedCrewMember in myFleetCrew)
            {
                GameObject _crewMember = Instantiate(crewMemberPrefabPvE);
                CMBehaviour _cMBehaviour = _crewMember.GetComponent<CMBehaviour>();
                _cMBehaviour.crewMember = myFleetCrew[i];
                _cMBehaviour.LoadCardDisplay();
                _crewMember.transform.SetParent(CrewDisplayForPVE.transform.GetChild(0));
                i++;
            }
        }
        else
        {
            foreach (Transform _card in CrewDisplayForPVE.transform.GetChild(0))
            {
                Destroy(_card.gameObject);
            }
            CrewDisplayForPVE.SetActive(false);
        }
    }

    public void DisplayInteractableChoicePanel()
    {
        if (InteractablesChoicePanel.activeSelf)
        {
            InteractablesChoicePanel.SetActive(false);
        }
        else
        {
            InteractablesChoicePanel.SetActive(true);
        }
    }



    [SynchronizableMethod]
    void PassTurn(){
            int turnOwnerIndex = playersList.IndexOf(turnOwner);
            if(playersList.Count-1 != turnOwnerIndex){
                turnOwner = playersList[turnOwnerIndex+1];
            }else{
                turnOwner = playersList[0];
            }
            TurnDisplayText.text = turnOwner + "'s Turn";
            FleetManager _fleet = Multiplayer.GetAvatar().GetComponent<FleetManager>();
            _fleet.StartTurn();           
            Commit();       
    }


    public void BroadcastDisplayListOfPlayers(List<User> myUsersPar){
        List<string> listOfUsers = new List<string>();
        for(int i = 0; i < myUsersPar.Count; i++){
            listOfUsers.Add(myUsersPar[i].Name);
        }
        GenerateInitSpawnPointsAndSendToUsers(listOfUsers);      
        BroadcastRemoteMethod("DisplayListOfPlayers", listOfUsers);
    }


    private void GenerateInitSpawnPointsAndSendToUsers(List<String> players)
    {       
        spawnPoints.Add(0);
        spawnPoints.Add(1);
        spawnPoints.Add(2);
        spawnPoints.Add(3);
        spawnPoints = spawnPoints.OrderBy( x => UnityEngine.Random.value ).ToList( );       

        for(int i = 0; i<players.Count; i++){
            Multiplayer.GetAvatar((ushort)i).GetComponent<FleetManager>().InitSpawnPoint = SpawnPoints[i].name;          
        }
        Commit();
        BroadcastRemoteMethod("SpawnFlagShips", spawnPoints);
    }

    [SynchronizableMethod]
    private void SpawnFlagShips(List<int> spawnPoints){
        Multiplayer.GetAvatar().GetComponent<FleetManager>().SpawnFlagShipsAtRandomSpawns(spawnPoints);
    }
    
    

    [SynchronizableMethod]
    public void DisplayListOfPlayers(List<string> listOfUsers){
                    
        foreach(string user in listOfUsers){
           UserDisplayText.text += user + "\n";
           playersList.Add(user);  
        }  
        turnOwner = listOfUsers[0];
        TurnDisplayText.text = turnOwner + "'s Turn";
        if(playersList.Last() == Multiplayer.Me.Name){
            Multiplayer.GetAvatar().GetComponent<FleetManager>().lastPlayer = true;
        }
    }

    

    public void AddShipToUI(GameObject spawnedShip, int index){
        GameObject shipIconTemp = Instantiate(ShipDisplayPrefab);       
        shipIconTemp.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();
        shipIconTemp.transform.SetParent(FleetPanel.transform);
        shipIconTemp.transform.localScale = new Vector3(1,1,1);
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
        Ship _ship = spawnedShip.GetComponent<Ship>();
        _ship.SetMovementIcon(shipIconTemp.transform.GetChild(0).gameObject);
        _ship.SetActionIcon(shipIconTemp.transform.GetChild(1).gameObject);
        _ship.goldDisplay = shipIconTemp.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();
        _ship.UpdateGoldDisplay();
        tempButton.onClick.AddListener(() => spawnedShip.GetComponent<Ship>().SelectShipFromItsIcon(spawnedShip));       
    }
    public void RemoveShipFromUI(int index){
        Destroy(FleetPanel.transform.GetChild(index).gameObject);
    }

    public void AddFlagShipToUI(GameObject spawnedShip){
        GameObject shipIconTemp = Instantiate(FlagShipDisplayPrefab);       
        shipIconTemp.transform.SetParent(FleetPanel.transform);
        shipIconTemp.transform.localScale = new Vector3(1,1,1);
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
        Ship _ship = spawnedShip.GetComponent<Ship>();
        _ship.SetMovementIcon(shipIconTemp.transform.GetChild(0).gameObject);
        _ship.SetActionIcon(shipIconTemp.transform.GetChild(1).gameObject);
        _ship.goldDisplay = shipIconTemp.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();
        _ship.UpdateGoldDisplay();
        tempButton.onClick.AddListener(() => spawnedShip.GetComponent<Ship>().SelectShipFromItsIcon(spawnedShip));      
    }

    
    public int GetColourID(string _requestingUser){            
        int id = playersList.IndexOf(_requestingUser);              
        return id;    
    }

#region GAME_START
    public void BroadCastTriggerStartGame(){
        BroadcastRemoteMethod("TriggerStartGame");
         
    }
    [SynchronizableMethod]
    public void TriggerStartGame(){
        Alteruna.Avatar myAvatar = Multiplayer.GetAvatar();
        myAvatar.GetComponent<FleetManager>().StartGame();
        PreGameMenu.SetActive(false);       
        Destroy(StartGameButton.gameObject);        
    }
    #endregion




    public void InstantiateInteractableButton(string interactable, Ship _ship, Transform _mapPiece)
    {
        FleetManager _fleet = _ship.GetComponentInParent<FleetManager>();
        GameObject _interactable = Instantiate(InteractableButtonPrefab, InteractablePanelPrefab.transform);
        Button _button = _interactable.GetComponent<Button>();

        switch (interactable)
        {
            case "AnotherPlayer":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = PvPButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Hostile Fleet!";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Another great pirate's fleet contests our pressence! We must deal with them immediately";
                _button.onClick.AddListener(() => _mapPiece.GetComponent<MapPieceBehaviour>().WaitForShipToAttackSelect(_ship));
                break;
            case "Tavern":
                if (_ship.shipGold < 1)
                {
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = TavernButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Broke Scallywag";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Treasureless! You are forced to scuttle on the outskirts of tavern trying to overhear some rumour...\n 50%/50%";
                _button.onClick.AddListener(() => TryTavernRumorGeneration(_mapPiece, _ship, _interactable));               
                }
                else
                { 
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = AltTavernButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Toast and Boast!";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You get a chance to make a name for yourself by buying everyone a round of drinks! Gain one victory point";
                _button.onClick.AddListener(() => TavernButtonMethod(_ship,_interactable));
                }
                break;
                
            case "PirateCove":
                if (_ship.shipGold > 0)
                {
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = PirateCoveButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Pirate Cove";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Questionable artifacts are on sale for those who dare to bring them aboard their ship\n 1 gold for a random power";
                    _button.onClick.AddListener(() => PirateCoveMethod(_ship, _interactable));
                }
                else
                { 
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = AltPirateCoveButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Recruitment Opportunity";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Attempt to recruit some young talent into your crew\n 30% chance to recruit a 1-power sailor";
                    _button.onClick.AddListener(() => AltPirateCoveMethod(_ship, _interactable));
                }
                
                break;
            case "Harbor":
                if (_ship.shipGold >= _ship.myFleet.myShips.Count)
                {
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = HarbourButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "A Harbor";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You meet a shipmaster open to comission. He offers to construct a fleetship to add to your escadra for " + _ship.myFleet.myShips.Count + " gold";
                    _button.onClick.AddListener(() => HarborButtonMethod(_ship, _interactable, _mapPiece));
                }
                else
                {
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = HarbourButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "A Harbor";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You can't afford to comission a ship, Dyaaaarrrn it! :(";
                    Destroy(_button);
                    _interactable.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                }
                
                break;
            case "Repair":
                if (_ship.healthPoints < 2)
                {
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = RepairButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Repairs";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Take some time repairing any damage the ship has suffered";
                    _button.onClick.AddListener(() => RepairAtHarborButtonMethod(_ship, _interactable));
                }
                else if (!_ship.myFleet.IsFlagshipAlive())
                {
                    _interactable.transform.GetChild(0).GetComponent<Image>().sprite = UpgradeButtonSprite;
                    _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "New Flagship!";
                    _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Our glorious fleet with rise again with a brand new flagship!";
                    _button.onClick.AddListener(() => UpgradeAtHarborButtonMethod(_ship, _interactable));
                } else { 
                    InstantiateALeaveInteractable(_interactable);
                }
                
                break;
            case "Rumor":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = RumorButtonSprite;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Rumors at Sea";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "A word reaches you about hidden treasure somewhere in the contested sea. Find it and claim it";
                _button.onClick.AddListener(() => RumorButtonMethod(_interactable, _mapPiece, _ship));
                break;
            case "Treasure":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = TreasureButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Hidden Treasure";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "The rumours were true! Dublons'o'plenty lie beneath these sands\n Gain 2-4 gold";
                _button.onClick.AddListener(() => TreasureButtonMethod(_interactable, _ship, _mapPiece));
                break;
            case "Sirens":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = SirensButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Siren's Call";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You find a ship entranced by prayin sirens' song. Drive them back, and rescue the ship\n 1d6";
                _button.onClick.AddListener(() => SirensButtonMethod(_interactable, _ship, _mapPiece));
                break;
            case "Dragon":
                _interactable.GetComponent<Image>().sprite = DragonButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "A Dragon!";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "A flying ship fiery ship in the sky! Nay, a black dragon! All hands on deck!\n 3d6";
                _button.onClick.AddListener(() => DragonButtonMethod(_interactable, _ship, _mapPiece));
                break;
            case "Pirates":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = PiratesButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "A Rogue Pirate Ship";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "These fools are beyond recruitment. Give them steel and fire, and free their loot!\n 1d4";
                _button.onClick.AddListener(() => PiratesButtonMethod(_interactable, _ship, _mapPiece));
                break;
            case "Retal":
                _interactable.transform.GetChild(0).GetComponent<Image>().sprite = PiratesButtonImage;
                _interactable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Opportunit Attack!";
                _interactable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Use the momentum from combat to re-engage the enemy!\n You will be able to move after this action";
                _button.onClick.AddListener(() => RetalButtonMethod(_interactable, _ship, _mapPiece));
                break;
            case "Leave":
                _interactable.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
                InstantiateALeaveInteractable(_interactable);
                break;
            case "CantLeave":
                InstantiateACantLeaveInteractable(_interactable);
                break;
            default:
                break;
        }
        _button.onClick.AddListener(() => DisplayInteractableChoicePanel());
        _button.onClick.AddListener(() => SetMapPieceSelectable(_mapPiece));
        
    }
    private void SetMapPieceSelectable(Transform map)
    {
        gameEventManager.mapPieceSelectable = true;
    }
    private void InstantiateALeaveInteractable(GameObject leaveInteractable)
    { 
        leaveInteractable.transform.GetChild(0).GetComponent<Image>().sprite = LeaveButtonImage;
        leaveInteractable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Leave";
        leaveInteractable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "None of that nonsense thank you very much";
        Button _button = leaveInteractable.GetComponent<Button>();
        _button.onClick.AddListener(() => LeaveMethod(_button.gameObject));
    }
    private void InstantiateACantLeaveInteractable(GameObject leaveInteractable)
    { 
        leaveInteractable.transform.GetChild(0).GetComponent<Image>().sprite = CantLeaveButtonImage;
        leaveInteractable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Tied Up in Combat!";
        leaveInteractable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "The ship must engage the enemy lest it be sunk in retreat";
        Button _button = leaveInteractable.GetComponent<Button>();
        Destroy(_button);
    }
    private void LeaveMethod(GameObject _buttonPrefab)
    {
        Destroy(_buttonPrefab.gameObject);
    }
    private void PiratesButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if (_ship.actionPoints > 0)
        {
            DisplayCrewForPVE(Multiplayer.GetAvatar().GetComponent<Hand>().myFleetCrew);
            CrewDisplayForPVE.GetComponent<PvEHandler>().myFleet = _ship.myFleet;
            CrewDisplayForPVE.GetComponent<PvEHandler>().InitiateEncounter("Pirates", _ship, _mapPiece.gameObject.GetComponent<MapPieceBehaviour>());
            Destroy(_buttonPrefab.gameObject);
            _ship.SpendActionPoints(1);
        }

    }
    private void TryTavernRumorGeneration(Transform _mapPieceTransform, Ship _ship, GameObject _buttonPrefab)
    {
        
        int coinFlip = UnityEngine.Random.Range(0, 2);
        if (coinFlip == 1)
        {
            if (_ship.actionPoints > 0)
            {
                _ship.SpendActionPoints(1);
                MapPieceBehaviour shipOccupiedMapPiece = _mapPieceTransform.gameObject.GetComponent<MapPieceBehaviour>();
                int mapNumber = UnityEngine.Random.Range(0, 52);
                MapPieceBehaviour _mapPiece = Map.transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>();
                _mapPiece.GenerateTreasure();
                Destroy(_buttonPrefab);
            }
        }
    }
    private void SirensButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if (_ship.actionPoints > 0)
        {
            DisplayCrewForPVE(Multiplayer.GetAvatar().GetComponent<Hand>().myFleetCrew);
            CrewDisplayForPVE.GetComponent<PvEHandler>().myFleet = _ship.myFleet;
            CrewDisplayForPVE.GetComponent<PvEHandler>().InitiateEncounter("Sirens", _ship, _mapPiece.gameObject.GetComponent<MapPieceBehaviour>());
            Destroy(_buttonPrefab.gameObject);
            _ship.SpendActionPoints(1);
        }
    }
    private void DragonButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if (_ship.actionPoints > 0)
        {
            DisplayCrewForPVE(Multiplayer.GetAvatar().GetComponent<Hand>().myFleetCrew);
            CrewDisplayForPVE.GetComponent<PvEHandler>().myFleet = _ship.myFleet;
            CrewDisplayForPVE.GetComponent<PvEHandler>().InitiateEncounter("Dragon", _ship, _mapPiece.gameObject.GetComponent<MapPieceBehaviour>());
            Destroy(_buttonPrefab.gameObject);
            _ship.SpendActionPoints(1);
        }         
    } 
    private void TreasureButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if (_ship.actionPoints > 0)
        {
            _ship.SpendActionPoints(1);
            MapPieceBehaviour tempMapPiece = _mapPiece.GetComponent<MapPieceBehaviour>();
            tempMapPiece.RemoveTreasure();
            _ship.shipGold += UnityEngine.Random.Range(2, 5);
            _ship.UpdateGoldDisplay();
            Destroy(_buttonPrefab);
        }
    }
    private void RetalButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if(_ship.actionPoints > 0){
            _ship.SpendActionPoints(1);
            _ship.hasRetal = false;
            MapPieceBehaviour tempMapPiece = _mapPiece.GetComponent<MapPieceBehaviour>();
            tempMapPiece.WaitForShipToAttackSelect(_ship);
            Destroy(_buttonPrefab);
        }
    }
    private void RumorButtonMethod(GameObject _buttonPrefab, Transform _scroll, Ship _ship)
    {
        if (_ship.actionPoints > 0)
        {
            _ship.SpendActionPoints(1);
            MapPieceBehaviour shipOccupiedMapPiece = GameObject.Find(_ship.occupyingMapPieceName).GetComponent<MapPieceBehaviour>();
            shipOccupiedMapPiece.BroadcastRemoveRumor();
            SpawnRumor();
            int mapNumber = UnityEngine.Random.Range(0, 52);
            MapPieceBehaviour _mapPiece = Map.transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>();
            _mapPiece.GenerateTreasure();
            Destroy(_buttonPrefab);
        }
    }
    private void TavernButtonMethod(Ship _ship, GameObject _buttonPrefab){
        if(_ship.shipGold >= 2 && _ship.actionPoints > 0){
            _ship.SpendActionPoints(1);
            _ship.SpendGold(2);
            _ship.GetComponentInParent<FleetManager>().GetVictoryPoints(1);
            Destroy(_buttonPrefab);
        }
    }
    private void PirateCoveMethod(Ship _ship, GameObject _buttonPrefab){
        if(_ship.shipGold >=1 && _ship.actionPoints > 0){
            _ship.SpendActionPoints(1);
            _ship.SpendGold(1);
            _ship.myFleet.myInventory.AddConsumable();       
            Destroy(_buttonPrefab);
        }     
    }
    private void AltPirateCoveMethod(Ship _ship, GameObject _buttonPrefab){
        int coinFlip = UnityEngine.Random.Range(0, 3);
        if (coinFlip == 2) {
            _ship.GetComponentInParent<Hand>().DrawAOnePowerCard();
        }
        _ship.SpendActionPoints(1);
        Destroy(_buttonPrefab);
    
    }
    private void HarborButtonMethod(Ship _ship, GameObject _buttonPrefab, Transform _mapPiece)
    {
        if (_ship.shipGold >= _ship.GetComponentInParent<FleetManager>().myShips.Count && _ship.actionPoints > 0)
        {
            _ship.SpendActionPoints(1);
            _ship.SpendGold(_ship.GetComponentInParent<FleetManager>().myShips.Count);
            _ship.GetComponentInParent<FleetManager>().MainSpawner.SpawnShip(_mapPiece.GetChild(0));
            Destroy(_buttonPrefab);
        }
    }
    private void RepairAtHarborButtonMethod(Ship _ship, GameObject _buttonPrefab){
        if (_ship.healthPoints == 1 && _ship.actionPoints > 0)
        {
            _ship.SpendActionPoints(1);
            _ship.ChangeShipHealth(-1);
            Destroy(_buttonPrefab);
        }     
        
    }
    private void UpgradeAtHarborButtonMethod(Ship _ship, GameObject _buttonPrefab)
    {
        if (_ship.actionPoints > 0)
        {
            _ship.myFleet.UpgradeThisShipToFlagShip(_ship);
            Destroy(_buttonPrefab);
        }   
    }
    private void SpawnRumor()
    {
        int randMapPiece = UnityEngine.Random.Range(0, 52);
        MapPieceBehaviour _mapPiece = Map.transform.GetChild(randMapPiece).GetComponent<MapPieceBehaviour>();
        if (!_mapPiece.HasRumor())
        {
            _mapPiece.BroadcastGenerateRumor();

        }
        else
        {
            SpawnRumor();
            return;
        }
    }
    public void ResetInteractablePanel(){
        foreach(Transform child in InteractablePanelPrefab.transform){
            Destroy(child.gameObject);
        }
    }
    public void DisplayVictoryPanel(bool a){  
        victoryPanel.SetActive(a);
    }
}
