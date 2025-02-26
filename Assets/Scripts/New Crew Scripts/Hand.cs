using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    public List<CrewMember> _myFleetCrew;
    public List<CrewMember> _commitedCards;
    public int _totalPower;

    public CommunityDeck _deckScript;
    public CrewMember _crewMemberScript;



    public void DiscardCrewMember(CrewMember crewToDiscard)
    {
        _deckScript.AddCrewMemberToDiscardPile(crewToDiscard);
    }

    public void CommitCard(CrewMember _crewMember)
    {
        _commitedCards.Add(_crewMember);
        _myFleetCrew.Remove(_crewMember);
    }

    public void EndBattle()
    {
        _totalPower = 0;
        foreach (CrewMember _crewMember in _commitedCards)
        {
            _totalPower += _crewMember._power;
            DiscardCrewMember(_crewMember);
        }
        _commitedCards = null;
    }

    public void DrawCard()
    {
        _myFleetCrew.Add(_deckScript.DrawCard());
    }



}
