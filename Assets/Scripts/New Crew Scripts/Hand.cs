using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Alteruna;
public class Hand : MonoBehaviour
{
    public List<CrewMember> myFleetCrew;
    public int _totalPower;

    //public CommunityDeck deckScript;
    public GameObject deckLoader;

    public GameObject crewMemberPrefab;
    public GameObject BattleCanvas;

    public Transform handZone;
    public Transform committedZone;

    Alteruna.Avatar _avatar;

    public void Start()
    {
        _avatar = this.gameObject.GetComponent<FleetManager>().avatar;
        //Initial referencing of Buttons and decks for fighting phase
        if(_avatar.IsMe){
        committedZone = GameObject.Find("Committed Zone").transform;
        handZone = GameObject.Find("HandZone").transform;
        deckLoader = GameObject.Find("CommunityDeck");
        BattleCanvas = GameObject.Find("2PLAYERFIGHTPREFAB");
        
        Button _DrawCardButton = GameObject.Find("DrawACardButton").GetComponent<Button>();
        _DrawCardButton.onClick.AddListener(DrawCard);
        Button _EndBattleButton = GameObject.Find("EndBattleButton").GetComponent<Button>();
        _DrawCardButton.onClick.AddListener(EndBattle);
        BattleCanvas.SetActive(false);

        CMSaveLoadHandler _cMSaveLoadHandler = deckLoader.GetComponent<CMSaveLoadHandler>(); 
        }
               
        //myFleetCrew = _cMSaveLoadHandler.loadedCrewMember.crewMember;
        InstantiateHand();
    }

    public void InstantiateHand()
    {
        int i = 0;
        foreach (CrewMember ownedCrewMember in myFleetCrew)
        {
            GameObject _crewMember = Instantiate(crewMemberPrefab);
            CMBehaviour _cMBehaviour = _crewMember.GetComponent<CMBehaviour>();
            _cMBehaviour.crewMember = myFleetCrew[i];
            _cMBehaviour.LoadCardDisplay();
            _crewMember.transform.SetParent(handZone);
            _cMBehaviour.committedZone = committedZone;
            i ++;
        }
    }


    public void DiscardCrewMember(CrewMember crewToDiscard)
    {
        //deckScript.AddCrewMemberToDiscardPile(crewToDiscard);
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
        if(_avatar.IsMe){
            myFleetCrew.Add(deckLoader.GetComponent<CMSaveLoadHandler>().ReturnDrawCard());
            Debug.Log("Drawing a card for: " + this.gameObject.name);
        }
        
    }
    public void DrawNCards(int amountOfCardsToDraw){
        for(int i = 0; i < amountOfCardsToDraw; i++){
            DrawCard();
        }
    }

}
