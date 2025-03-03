using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Hand : MonoBehaviour
{
    public List<CrewMember> myFleetCrew;
    public List<CrewMember> commitedCards;
    public int _totalPower;

    //public CommunityDeck deckScript;
    public GameObject deckLoader;
    public CrewMember crewMemberScript;

    public GameObject crewMemberPrefab;
    public Transform handZone;

    public void Start()
    {
        CMSaveLoadHandler _cMSaveLoadHandler = deckLoader.GetComponent<CMSaveLoadHandler>();
        myFleetCrew = _cMSaveLoadHandler.loadedCrewMember.crewMember;
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
            i ++;
        }
    }


    public void DiscardCrewMember(CrewMember crewToDiscard)
    {
        //deckScript.AddCrewMemberToDiscardPile(crewToDiscard);
    }

    public void CommitCard(CrewMember _crewMember)
    {
        commitedCards.Add(_crewMember);
        myFleetCrew.Remove(_crewMember);
    }

    public void EndBattle()
    {
        _totalPower = 0;
        foreach (CrewMember _crewMember in commitedCards)
        {
            _totalPower += _crewMember.crewMemberPower;
            DiscardCrewMember(_crewMember);
        }
        commitedCards = null;
    }

    public void DrawCard()
    {
        //_myFleetCrew.Add(_deckScript.DrawCard());
    }



}
