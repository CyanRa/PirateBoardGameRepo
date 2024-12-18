using System;
using System.Collections.Generic;
using System.Linq;
using Alteruna;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : AttributesSync
{
    bool menuOpen = false;
    bool StopOnMouseOn = false;
#region GAMEOBJECTS
    public GameObject ShipDisplayPrefab;
    public GameObject FlagShipDisplayPrefab;
    public GameObject MultiplayerPanel;
    public GameObject MenuPanel;
    public GameObject FleetPanel;
    public GameObject ActionBar;
    public GameObject StopOnMousePlane;
    public GameObject MultiplayerSystem;
#endregion
    public Button StartGameButton;
    public string turnOwner;
    public List<string> playersList;
    
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
    
    [SynchronizableMethod]
    void PassTurn(){
            int turnOwnerIndex = playersList.IndexOf(turnOwner);
            if(playersList.Count-1 != turnOwnerIndex){
                Debug.Log("player list size is: " + playersList.Count);
                Debug.Log("turn owner idex is: " + turnOwnerIndex);
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
        }
        turnOwner = listOfUsers[0];
        TurnDisplayText.text = turnOwner + "'s Turn";
        Commit();
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

#region GAME_START
    public void BroadCastTriggerStartGame(){
        Debug.Log("Broadcasting game start");
        BroadcastRemoteMethod("TriggerStartGame");
         
    }
    [SynchronizableMethod]
    public void TriggerStartGame(){
        Alteruna.Avatar myAvatar = Multiplayer.GetAvatar();
        myAvatar.GetComponent<FleetManager>().StartGame();
        Destroy(StartGameButton.gameObject);
    }
#endregion
}
