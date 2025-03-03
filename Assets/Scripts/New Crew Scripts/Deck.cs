using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class CommunityDeck : MonoBehaviour
{
    public static CommunityDeck Instance { get; private set; } //Singleton

    public List<CrewMember> crewDeck;
    public List<CrewMember> tutorCrewDeck;
    public List<CrewMember> discardPile;

    

    public Hand _handScript;

    public class Card{
        public string name;
        public string image;
        public int power;

        public bool isTutor; //still need to make isTutor logic
        public bool isSelected;
        public bool isCommited;
    }
    private void Awake() //Singleton declaration
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    //function to draw 1 card
    public CrewMember DrawCard()
    {
        if (crewDeck.Count <= 0)
        {
            discardPile = crewDeck;
            discardPile.Clear();
            ShuffleDeck(crewDeck);
        }
        
        CrewMember _crewMemberToReturn = crewDeck[0];
        crewDeck.RemoveAt(0);
        return _crewMemberToReturn;
    }

    public void ShuffleDeck(List<CrewMember> listToShuffle)
    {
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = listToShuffle[i];
            listToShuffle[i] = listToShuffle[j];
            listToShuffle[j] = temp;
        }
    }

    public void AddCrewMemberToDiscardPile(CrewMember crewToAddToDiscardPile)
    {
        discardPile.Add(crewToAddToDiscardPile);
    }

    //Need to populate Deck
}
