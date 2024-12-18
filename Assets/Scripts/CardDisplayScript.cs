using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardDisplayScript : MonoBehaviour
{
    public CrewCard card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;


    private void Start()
    {
        nameText.text = card.name;
        powerText.text = card.power.ToString();
    }
}
