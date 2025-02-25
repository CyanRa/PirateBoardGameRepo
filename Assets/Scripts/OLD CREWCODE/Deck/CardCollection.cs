using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Collection of all Crew Cards
/// </summary>
/// 
[CreateAssetMenu(menuName = "CrewCardCollection")]
public class CardCollection : ScriptableObject
{
    public List<CrewCardScriptable> CrewCards;



    // Ability to remove cards
    public void RemoveCardFromCrewCards(CrewCardScriptable card)
    {
        if (CrewCards.Contains(card))
        {
            CrewCards.Remove(card);
        }
        else
        {
            Debug.LogWarning("Card not present in CrewCards");
        }
    }

    //Ability to add cards
    public void AddCardToCrewCards(CrewCardScriptable card)
    {
        CrewCards.Add(card);
    }


}