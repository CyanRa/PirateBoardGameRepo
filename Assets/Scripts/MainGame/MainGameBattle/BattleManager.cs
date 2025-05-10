using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alteruna;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : AttributesSync
{
    public Hand myHand;
    public bool cardsPlayedLastTurn;
    public int winner;
    public int myTurnID;

    public TextMeshProUGUI turnOwnerText;
    [SynchronizableField]public string attackerName;
    [SynchronizableField]public string defenderName;
    [SynchronizableField]public string attackingShip ="";
    [SynchronizableField]public string defendingShip="";
    [SynchronizableField]public string turnOwnerDisplay;
    [SynchronizableField]public int turnOwner;
    [SynchronizableField]public int attackerUID;
    [SynchronizableField]public int defenderUID;
    public Ship shipInCombat;
    GameObject DisplayPanel;

    [SynchronizableField] public int attackerPower;
    public GameObject oppPowerDisplay;
    [SynchronizableField] public int defenderPower;
    public GameObject myPowerDisplay;
    [SynchronizableField] public List<int> attackerPlayedCards;
    [SynchronizableField] public List<int> defenderPlayedCards;


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

    public void RequestInvokeOppHandDisplay(){
        InvokeRemoteMethod("InvokeOppHandDisplay", (ushort)attackerUID);
    }
    [SynchronizableMethod]
    public void InvokeOppHandDisplay()
    {
        InvokeOpponentHandDisplay(myHand.myFleetCrew.Count);
    }
    [SynchronizableMethod]
    
    public void InvokeOpponentHandDisplay(int numberOfCardsInOpponentHand){
        if(myTurnID == 1){
            InvokeRemoteMethod("InitializeUI", (ushort)defenderUID, numberOfCardsInOpponentHand);
        }else{
            InvokeRemoteMethod("InitializeUI", (ushort)attackerUID, numberOfCardsInOpponentHand);
        }
        
    }
    [SynchronizableMethod]
    public void InitializeUI(int numberOfCardsInOpponentHand){        
        myHand.InstantiateOpponentHandZone(numberOfCardsInOpponentHand);
    }

    public void InvokeDisplayCommitedCard(int uid, int _power){
        InvokeRemoteMethod("DisplayCommitedCard", (ushort)uid, _power);
    }
    [SynchronizableMethod]
    public void DisplayCommitedCard(int _power){
        if(myTurnID ==1){
            defenderPlayedCards.Add(_power);
        }else{
            attackerPlayedCards.Add(_power);
        }
        myHand.InstantiateCommitedCard();
    }

    public void BroadcastInitializePrefabForDefender(ushort defenderID,string defenderShip){
        InvokeRemoteMethod("InitializeHandPrefabForDefender", defenderID, defenderShip);
    }

    [SynchronizableMethod]
    public void InitializeHandPrefabForDefender( string defenderShip){
        Ship _ship = GameObject.Find(defenderShip).GetComponent<Ship>();
        _ship.GetComponentInParent<FleetManager>().GetComponent<Hand>().BattleCanvas.SetActive(true);

    }
    public void BroadcastEndBattle(){
        
        InvokeRemoteMethod("EndBattle",(ushort)attackerUID);
        InvokeRemoteMethod("FinishBattleForDefender",(ushort)defenderUID);
    }

    [SynchronizableMethod]
    public void EndBattle(){
        StartCoroutine(DisplayBattleEnd(defenderPlayedCards, true));
          
    }
    [SynchronizableMethod]
    public void ContinueEndBattle(){       
        if (attackerPower > defenderPower){     
            InvokeRemoteMethod("DisplayAttackerOptions", (ushort)attackerUID, defendingShip, attackerPower - defenderPower);
        }else{
            shipInCombat.occupyingMapPiece.HandleMapPieceStatus();
        }
        //BREAKS
        
        Button _endCardTurnButton = GameObject.Find("EndCardTurnButton")?.GetComponent<Button>();
        _endCardTurnButton.onClick.RemoveAllListeners();
        myHand.PurgeUI();
        PurgeDataOfFinishedBattle();
        cardsPlayedLastTurn = true;
        myHand.BattleCanvas.SetActive(false);
        InvokeRemoteMethod("ContinueEndBattleDefender", (ushort)defenderUID);
    }
    [SynchronizableMethod]
    public void ContinueEndBattleDefender(){       
        Button _endCardTurnButton = GameObject.Find("EndCardTurnButton").GetComponent<Button>();
        _endCardTurnButton.onClick.RemoveAllListeners();
        myHand.PurgeUI();
        PurgeDataOfFinishedBattle();
        cardsPlayedLastTurn = false;
        myHand.BattleCanvas.SetActive(false);
    }
    [SynchronizableMethod]
    private void FinishBattleForDefender(){
        StartCoroutine(DisplayBattleEnd(attackerPlayedCards, false));
    }

    private IEnumerator DisplayBattleEnd(List<int> playedCardsPower, bool isAttacker){

        int pos = 0;
        int accPwr = 0;
        if(shipInCombat.isFlagship){
            if(myTurnID == 1){
                attackerPower +=1;
                accPwr +=1;

            }else{
                defenderPower +=1;
                accPwr+=1;
            }     
        }
        foreach(int cardPower in playedCardsPower.ToList()){
            myHand.GenerateAndDisplayAndProcessCard(cardPower, pos, accPwr);
            accPwr += cardPower;
           
            pos++;
            yield return new WaitForSeconds(1.5f);
        }
        
        yield return new WaitForSeconds(3f);
        InvokeRemoteMethod("ContinueEndBattle", (ushort)attackerUID);    
    }

    
    private void PurgeDataOfFinishedBattle(){
        turnOwner = 1;
        attackerPower = 0;
        defenderPower = 0;    
        myPowerDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        oppPowerDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        attackerPlayedCards.Clear();
        defenderPlayedCards.Clear();
    }

    [SynchronizableMethod]
    private void DisplayAttackerOptions(string ship, int damage){ 
        Ship _ship = GameObject.Find(ship).GetComponent<Ship>();
        Ship _attackingShip = GameObject.Find(attackingShip).GetComponent<Ship>();
        _attackingShip.GetComponentInParent<FleetManager>().HandleCombatVictory(damage, defendingShip, attackingShip);
    }

    public bool MyTurn(){
        if(myTurnID == turnOwner){
            return true;
        }else{
            return false;
        }
    }

    
}
