using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Xml;
using NUnit.Framework;




public class CardDisplayScript : MonoBehaviour
{
    public CCard _card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public Image cardImage;


    private void Start()
    {
        
    }

    
    public void Awake()
    {
        _card = GetComponent<CCard>();
        SetCardUI();
    }

    private void OnValidate()
    {
        Awake();
    }



    public void SetCardUI()
    { 
       if (_card != null && _card.CardData !=null)
        {
            SetCardText();
            SetCardImage();
        }
    }

    //calls carddata via CCard from CrewCardScriptable
    private void SetCardText()
    {
        nameText.text = _card.CardData.crewCardName;
        powerText.text = _card.CardData.power.ToString();
    }

    private void SetCardImage()
    {
        cardImage.sprite = _card.CardData.image;
    }




}
