using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SteeringWheelController steeringWheelController;
    public TextMeshProUGUI comment;
    private void Start()
    {
        steeringWheelController = transform.parent.parent.GetComponentInChildren<SteeringWheelController>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        StartCoroutine(FadeOptionImage());

        if (transform.GetSiblingIndex() == 0)
        {
            steeringWheelController.SteerLeft();
        }
        else
        {
            steeringWheelController.SteerRight();
        }

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        StopAllCoroutines();
        Color colorToFade = transform.GetChild(0).GetComponent<Image>().color;
        colorToFade.a = 1;
        Color colorToOpaq = comment.color;
        colorToOpaq.a = 0;
        transform.GetChild(0).GetComponent<Image>().color = colorToFade;
        comment.color = colorToOpaq;

    }

    private IEnumerator FadeOptionImage()
    {
        bool fading = true;
        Color colorToFade = transform.GetChild(0).GetComponent<Image>().color;
        Color colorToOpaq = comment.color;
        while (fading)
        {

            colorToFade += new Color(0, 0, 0, -0.02f);
            colorToOpaq += new Color(0,0,0,0.02f);
            transform.GetChild(0).GetComponent<Image>().color = colorToFade;
            comment.color = colorToOpaq;
            if (colorToFade.a < 0.2f)
            {
                fading = false;
            }
            yield return null;
        }
    }
}
