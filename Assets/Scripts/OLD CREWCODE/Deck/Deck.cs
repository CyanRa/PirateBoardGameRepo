using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Contains all cards in play (Deck, Hand and Discard) and manages their cycle.
/// </summary>
public class Deck : MonoBehaviour
{
    public static Deck Instance { get; private set; } //Singleton

    [SerializeField] public CardCollection _communityDeck; //reference CardCollection Script
    [SerializeField] public CCard _cardPrefab; //reference to our CardPrefab, gets copied later with different versions (srciptables)
    
    public Canvas _cardCanvas;
    public GameObject _handZone;


    private List<CCard> _deckPile = new();
    private List<CCard> _discardPile;

    public List<CCard> _handCards;
    public List<CCard> _tempCards;




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
        for (int i = 0; i < _communityDeck.CrewCards.Count; i++)
        {
            CCard card = Instantiate(_cardPrefab, _cardCanvas.transform); //Instantiates the card prefab as a child of the card Canvas
            card.Setup(_communityDeck.CrewCards[i]);
            _deckPile.Add(card); //Starts all cards in the deckpile
            card.gameObject.SetActive(false);
        }
        ShuffleDeck();
    }


    //Call once at start and whenever needed
    private void ShuffleDeck()
    {
        for (int i = _deckPile.Count - 1; i > 0; i--)
            {
            int j = Random.Range(0, i + 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }


    //draws opening hand
    public void DrawHand(int amount = 3)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (_deckPile.Count <= 0)
        {
            _discardPile = _deckPile;
            _discardPile.Clear();
            ShuffleDeck();
        }
        _deckPile[0].transform.parent = _handZone.transform;
        _handCards.Add(_deckPile[0]);
        _deckPile[0].gameObject.SetActive(true);
        _deckPile.RemoveAt(0);
    }


    //method to discard cards from hand to discard
    public void DiscardCard(CCard card)
    {
        if (_handCards.Contains(card))
        {
            _handCards.Remove(card);
            _discardPile.Add(card);
            card.gameObject.SetActive(false);
        }
    }

}
