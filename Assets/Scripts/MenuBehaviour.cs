using System;
using System.Collections.Generic;
using System.Linq;
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
    public Spawner mySpawner;
    public GameObject MultiplayerPanel;
    public GameObject MenuPanel;
    public GameObject FleetPanel;
    public GameObject ActionBar;
    public GameObject StopOnMousePlane;
    public GameObject MultiplayerSystem;
    public GameObject CrewDisplayPanel;
    public GameObject crewMemberPrefab;
    public GameObject InteractablePanelPrefab;
    public GameObject InteractableButtonPrefab;
    public GameObject rumorScrollPrefab;
    public Sprite HarbourButtonSprite;
    public Sprite RumorButtonSprite;
    public Sprite TreasureButtonImage;
    
    
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ){    
            OpenMenu();
        }    
    }

    public void ChooseColour(Button _button){
        FleetManager _fleet = Multiplayer.GetAvatar().GetComponent<FleetManager>();
        if (_fleet.fleetColour==""){
            BroadcastRemoteMethod("LockInFleetColour", _button.gameObject.name, _fleet.name);
            
        }       
    }

    [SynchronizableMethod]
    public void LockInFleetColour(string _colour, string _avatarName){
        
        FleetManager _fleet = GameObject.Find(_avatarName).GetComponent<FleetManager>();
        _fleet.fleetColour = _colour;
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
        BroadcastRemoteMethod("DisplayListOfPlayers", listOfUsers);
    }

    [SynchronizableMethod]
    public void DisplayListOfPlayers(List<string> listOfUsers){
                    
        foreach(string user in listOfUsers){
           UserDisplayText.text += user + "\n";
           playersList.Add(user);
           Debug.Log("Added " + user + " to playersList");   
        }  
        turnOwner = listOfUsers[0];
        TurnDisplayText.text = turnOwner + "'s Turn";
    }

    

    public void AddShipToUI(GameObject spawnedShip, int index){
        GameObject shipIconTemp = Instantiate(ShipDisplayPrefab);       
        shipIconTemp.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();
        shipIconTemp.transform.SetParent(FleetPanel.transform);
        shipIconTemp.transform.localScale = new Vector3(1,1,1);
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
        Ship _ship = spawnedShip.GetComponent<Ship>();
        _ship.goldDisplay = shipIconTemp.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
        _ship.UpdateGoldDisplay();
        tempButton.onClick.AddListener(() => spawnedShip.GetComponent<Ship>().SelectShipFromItsIcon(spawnedShip));       
    }

    public void AddFlagShipToUI(GameObject spawnedShip){
        GameObject shipIconTemp = Instantiate(FlagShipDisplayPrefab);       
        shipIconTemp.transform.SetParent(FleetPanel.transform);
        shipIconTemp.transform.localScale = new Vector3(1,1,1);
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
        Ship _ship = spawnedShip.GetComponent<Ship>();
        _ship.goldDisplay = shipIconTemp.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
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
        GameObject _interactable = Instantiate(InteractableButtonPrefab);
        _interactable.transform.SetParent(InteractablePanelPrefab.transform);
        Button _button = _interactable.GetComponent<Button>();
        switch(interactable)
        {
            case "Tavern": 
                _button.onClick.AddListener(() => TavernButtonMethod(_ship, _interactable));
                break;
            case "PirateCove":
                _button.onClick.AddListener(PirateCoveMethod);
                break;
            case "Harbor":
                _interactable.GetComponent<Image>().sprite = HarbourButtonSprite;
                _button.onClick.AddListener(() => HarborButtonMethod(_ship, _interactable, _mapPiece));
                break;
            case "Rumor":
                _interactable.GetComponent<Image>().sprite = RumorButtonSprite;
                _button.onClick.AddListener(() => RumorButtonMethod(_interactable, _mapPiece, _ship));
                break;
            case "Treasure":
                _interactable.GetComponent<Image>().sprite = TreasureButtonImage;
                _button.onClick.AddListener(() => TreasureButtonMethod(_interactable, _ship, _mapPiece));
                break;
                default:
                break;
        }
        
    }

    private void TreasureButtonMethod(GameObject _buttonPrefab, Ship _ship, Transform _mapPiece)
    {
        if(_ship.actionPoints > 0){
        _ship.actionPoints -= 1;
        MapPieceBehaviour tempMapPiece = _mapPiece.GetComponent<MapPieceBehaviour>();
        tempMapPiece.RemoveTreasure();
        _ship.shipGold += UnityEngine.Random.Range(2,5);
        _ship.UpdateGoldDisplay();
        Destroy(_buttonPrefab);
        }

    }

    private void RumorButtonMethod(GameObject _buttonPrefab, Transform _scroll, Ship _ship)
    {
        if(_ship.actionPoints > 0){
            _ship.actionPoints -= 1;
        MapPieceBehaviour shipOccupiedMapPiece = GameObject.Find(_ship.occupyingMapPieceName).GetComponent<MapPieceBehaviour>();
        shipOccupiedMapPiece.BroadcastRemoveRumor();
        SpawnRumor();
        int mapNumber = UnityEngine.Random.Range(0,52);
        MapPieceBehaviour _mapPiece = Map.transform.GetChild(mapNumber).GetComponent<MapPieceBehaviour>();
        _mapPiece.GenerateTreasure();
        Destroy(_buttonPrefab);
        }
        
    }

    private void TavernButtonMethod(Ship _ship, GameObject _buttonPrefab){
        if(_ship.shipGold >= 2 && _ship.actionPoints > 0){
            _ship.actionPoints -= 1;
            _ship.SpendGold(2);
            _ship.GetComponentInParent<FleetManager>().GetVictoryPoints(1);
            Destroy(_buttonPrefab);
        }
    }
    private void PirateCoveMethod(){

    }

    private void HarborButtonMethod(Ship _ship, GameObject _buttonPrefab, Transform _mapPiece){
        
        if(_ship.shipGold >= _ship.GetComponentInParent<FleetManager>().myShips.Count && _ship.actionPoints > 0){
             _ship.actionPoints -= 1;
            _ship.SpendGold(_ship.GetComponentInParent<FleetManager>().myShips.Count);
            _ship.GetComponentInParent<FleetManager>().MainSpawner.SpawnShip(_mapPiece);
            Destroy(_buttonPrefab);
        }
    }

    private void SpawnRumor(){
        int randMapPiece = UnityEngine.Random.Range(0,52);
        MapPieceBehaviour _mapPiece = Map.transform.GetChild(randMapPiece).GetComponent<MapPieceBehaviour>();
        if(!_mapPiece.HasRumor()){
            _mapPiece.BroadcastGenerateRumor();
            
        }else{
            SpawnRumor();
            return;
        }
        
    }
     public void ResetInteractablePanel(){
        foreach(Transform child in InteractablePanelPrefab.transform){
            Destroy(child.gameObject);
        }
    }
}
