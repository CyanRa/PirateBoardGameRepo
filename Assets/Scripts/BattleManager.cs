using System;
using System.Threading;
using Alteruna;
using TMPro;
using UnityEngine;

public class BattleManager : AttributesSync
{
    public Hand myHand;
    public bool cardsPlayedLastTurn;
    public int winner;
    public int myTurnID;

    public TextMeshProUGUI turnOwnerText;
    [SynchronizableField]public string attackerName;
    [SynchronizableField]public string defenderName;
    [SynchronizableField]public string turnOwnerDisplay;
    [SynchronizableField]public int turnOwner;
    [SynchronizableField]public int attackerUID;
    [SynchronizableField]public int defenderUID;
    public Ship shipInCombat;

    [SynchronizableField] public int attackerPower;
    [SynchronizableField] public int defenderPower;

    void Start()
    {
        turnOwner = 1;
    }

    public void SetDefender(string defender){
        defenderName = defender;
    }
    public void SetAttacker(string attacker){
        attackerName = attacker;
    }

    
    public void BroadcastSetTurnOwnerDisplay(){
        BroadcastRemoteMethod("SetTurnOwnerText");
    }
    [SynchronizableMethod]
    public void SetTurnOwnerText(){
        if(turnOwner == 1){
            turnOwnerDisplay = attackerName;
        }else{
            turnOwnerDisplay = defenderName;
        }
        turnOwnerText.text = turnOwnerDisplay;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void InvokeOpponentHandDisplay(int numberOfCardsInOpponentHand){
        if(myTurnID == 1){
            InvokeRemoteMethod("InitializeUI", (ushort)0, numberOfCardsInOpponentHand);
        }else if(myTurnID == 0){
            Debug.Log("My UID is "+attackerUID + ". Defender UID is " + defenderUID);
            InvokeRemoteMethod("InitializeUI", 1, numberOfCardsInOpponentHand);
        }
        
    }
    [SynchronizableMethod]
    public void InitializeUI(int numberOfCardsInOpponentHand){
        myHand.InstantiateOpponentHandZone(numberOfCardsInOpponentHand);
    }

    public void BroadcastInitializePrefabForDefender(ushort defenderID,string defenderShip){
        InvokeRemoteMethod("InitializeHandPrefabForDefender", defenderID, defenderShip);
    }

    [SynchronizableMethod]
    public void InitializeHandPrefabForDefender( string defenderShip){
        Ship _ship = GameObject.Find(defenderShip).GetComponent<Ship>();
        _ship.GetComponentInParent<FleetManager>().GetComponent<Hand>().BattleCanvas.SetActive(true);

    }

    [SynchronizableMethod]
    public void EndBattle(){
        if(attackerPower > defenderPower){
            Debug.Log("Attacking fleet: " + attackerName + " is victorious!");
        }else{
            Debug.Log("Defending fleet: " + defenderName + " is victorious!");
        }
    }

    
}
