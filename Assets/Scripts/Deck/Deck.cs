using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Contains all cards in play (Deck, Hand and Discard) and manages their cycle.
/// </summary>
public class Deck : MonoBehaviour
{
    public static Deck Instance { get; private set; } //Singleton

    private CardCollection _communityDeck; //reference CardCollection Script
    private CCard _cardPrefab; //reference to our CardPrefab, gets copied later with different versions (srciptables)

    public List<CCard> DeckPile;
    public List<CCard> DiscardPile;

    public List<CCard> HandCards;

    private Canvas _cardCanvas;



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

    private void Start()
    {
        InstantiateDeck();
    }

    private void InstantiateDeck()
    {
        CCard card = Instantiate(_cardPrefab, _cardCanvas.transform);
    }



}
