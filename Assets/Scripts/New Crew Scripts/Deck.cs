using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class CommunityDeck : MonoBehaviour
{
    public static CommunityDeck Instance { get; private set; } //Singleton

    private List<CrewMember> _crewDeck;
    private List<CrewMember> _tutorCrewDeck;
    private List<CrewMember> _discardPile;



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
    

    //Need to get back to this after making Hand script
    /*public void DrawCard()
    {
        if (_crewDeck.Count <= 0)
        {
            _discardPile = _crewDeck;
            _discardPile.Clear();
            ShuffleDeck(_crewDeck);
        }
        _crewDeck[0].transform.parent = _handZone.transform;
        return.Add(_crewDeck[0]);
        _crewDeck.RemoveAt(0);
    }*/

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
        _discardPile.Add(crewToAddToDiscardPile);
    }
}
