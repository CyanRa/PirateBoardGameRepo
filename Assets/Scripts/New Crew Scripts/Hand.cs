using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Hand : MonoBehaviour
{
    public List<CrewMember> myFleetCrew;
    public int _totalPower;

    //public CommunityDeck deckScript;
    public GameObject deckLoader;
    public CrewMember crewMemberScript;

    public GameObject crewMemberPrefab;
    public Transform handZone;
    public Transform committedZone;

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
        //_myFleetCrew.Add(_deckScript.DrawCard());
    }

}
