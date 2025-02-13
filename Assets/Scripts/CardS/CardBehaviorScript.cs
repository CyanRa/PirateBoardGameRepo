using UnityEngine;
using UnityEngine.EventSystems;

//Allows dragging of Crew Cards
public class CardBehaviorScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler

{
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
        Debug.Log("Drag");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

    }

}