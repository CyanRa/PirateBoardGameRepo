using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Allows dragging of Crew Cards
public class CardBehaviorScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler

{
    private bool _isBeingDragged;
    private Canvas _cCardCanvas;
    private RectTransform _rectTransform;
    private CCard _card;

    private readonly string CANVAS_TAG = "CCardCanvas";


    private void Start()
    {
        _cCardCanvas = GameObject.FindGameObjectWithTag(CANVAS_TAG).GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("BeginDrag");

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        //Debug.Log("Drag");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("EndDrag");
        Deck.Instance.DiscardCard(_card);
        Debug.Log(_card);
    }
}