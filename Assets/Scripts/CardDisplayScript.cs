using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Xml;
using NUnit.Framework;

public class CardDisplayScript : MonoBehaviour
{
    public CrewCard card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public Image cardImage;


    private void Start()
    {
        
    }



    private void Update()
    {
        nameText.text = card.name;
        powerText.text = card.power.ToString();
        cardImage.sprite = card.image;
    }
}
