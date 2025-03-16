using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Alteruna;
using System;

public class Hand : MonoBehaviour
{
    public List<CrewMember> myFleetCrew;
    public int _totalPower;

    //public CommunityDeck deckScript;
    public GameObject deckLoader;

    public GameObject crewMemberPrefab;
    public GameObject enemyCardbackPrefab;
    public GameObject BattleCanvas;

    public Transform handZone;
    public Transform committedZone;
    public Transform opponentHandZone;
    public Transform opponentCommittedZone;
    public BattleManager battleManager;
    public CMSaveLoadHandler _cMSaveLoadHandler;

    public Alteruna.Avatar avatar;

    
    public void Start()
    {
        avatar = this.gameObject.GetComponent<FleetManager>().avatar;
        //Initial referencing of Buttons and decks for fighting phase
        if(!avatar.IsMe) return;
        InitializeBattleZoneGameObjects();
        _cMSaveLoadHandler = deckLoader.GetComponent<CMSaveLoadHandler>();                   
    }

    private void InitializeBattleZoneGameObjects()
    {
        committedZone = GameObject.Find("Committed Zone").transform;
        handZone = GameObject.Find("HandZone").transform;
        opponentHandZone = GameObject.Find("Opp Hand Zone").transform;
        opponentCommittedZone = GameObject.Find("Opp Committed Zone").transform;
        deckLoader = GameObject.Find("CommunityDeck");
        BattleCanvas = GameObject.Find("CardCanvas");
        battleManager = BattleCanvas.GetComponentInParent<BattleManager>();
        BattleCanvas.SetActive(false);
    }

    public void InstantiateHand()
    {
        
        int i = 0;
        foreach (CrewMember ownedCrewMember in myFleetCrew)
        {
            GameObject _crewMember = Instantiate(crewMemberPrefab);
            CMBehaviour _cMBehaviour = _crewMember.GetComponent<CMBehaviour>();
            _cMBehaviour.myHand = GetComponent<Hand>();
            _cMBehaviour.crewMember = myFleetCrew[i];
            _cMBehaviour.LoadCardDisplay();
            _crewMember.transform.SetParent(handZone);
            _cMBehaviour.committedZone = committedZone;
            i ++;
        }
    }
    public void InstantiateOpponentHandZone(int numberOfOpponentCards){
        if(opponentHandZone.childCount > 0){
            foreach(Transform child in opponentHandZone){
                Destroy(child.gameObject);
            }

        }
        for(int i = 0; i < numberOfOpponentCards; i++){
            GameObject _crewMember = Instantiate(enemyCardbackPrefab);
            _crewMember.transform.SetParent(opponentHandZone);
        }
        
    }
    public void PurgeUI(){
        foreach(Transform child in opponentHandZone){
            Destroy(child.gameObject);
        }
        foreach(Transform child in opponentCommittedZone){
            Destroy(child.gameObject);
        }
        foreach(Transform child in handZone){
            Destroy(child.gameObject);
        }
        foreach(Transform child in committedZone){
            Destroy(child.gameObject);
        }
    }
    public void InstantiateCommitedCard(){
        GameObject _crewMember = Instantiate(enemyCardbackPrefab);
        _crewMember.transform.SetParent(opponentCommittedZone);
    }


    public void DiscardCrewMember(CrewMember crewToDiscard)
    {
        _cMSaveLoadHandler.AddCrewMemberToDiscardPile(crewToDiscard);
    }
    
    public void EndBattle()
    {
        _totalPower = 0;
        foreach (Transform _card in committedZone)
        {
            CMBehaviour _crewMember = _card.gameObject.GetComponent<CMBehaviour>();
            _totalPower += _crewMember.crewMember.crewMemberPower;
            Destroy(_card.gameObject);
        }
        Debug.Log("Total Power: "+_totalPower);
        foreach (Transform _card in handZone)
        {
            Destroy(_card.gameObject);
        }
    }

    public void DrawCard()
    {
        if(avatar.IsMe){
            myFleetCrew.Add(_cMSaveLoadHandler.ReturnDrawCard());
            Debug.Log("Drawing a card for: " + this.gameObject.name);
        }
        
    }
    public void DrawNCards(int amountOfCardsToDraw){
        for(int i = 0; i < amountOfCardsToDraw; i++){
            DrawCard();
        }
    }

    public void EndCardTurn(){
        if(battleManager.cardsPlayedLastTurn == false){
            battleManager.BroadcastEndBattle();
        }
        if(battleManager.turnOwner == battleManager.myTurnID){
            if(battleManager.turnOwner == 1){
            battleManager.turnOwner = 0;
            }else{
            battleManager.turnOwner = 1;
            }
            battleManager.BroadcastSetTurnOwnerDisplay();
        }
        battleManager.cardsPlayedLastTurn = false;
      
    }

}
