using System;
using System.Collections.Generic;
using System.Linq;
using Alteruna;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : AttributesSync
{
    bool menuOpen = false;
    bool StopOnMouseOn = false;
#region GAMEOBJECTS
    public GameObject ShipDisplayPrefab;
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
                Debug.Log("player list size is: " + playersList.Count);
                Debug.Log("turn owner index is: " + turnOwnerIndex);
                turnOwner = playersList[turnOwnerIndex+1];
            }else{
                turnOwner = playersList[0];
            }
            TurnDisplayText.text = turnOwner + "'s Turn";           
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
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
        tempButton.onClick.AddListener(() => spawnedShip.GetComponent<Ship>().SelectShipFromItsIcon(spawnedShip));       
    }

    public void AddFlagShipToUI(GameObject spawnedShip){
        GameObject shipIconTemp = Instantiate(FlagShipDisplayPrefab);
        shipIconTemp.transform.SetParent(FleetPanel.transform);
        Button tempButton = shipIconTemp.GetComponentInChildren<Button>();
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
}
