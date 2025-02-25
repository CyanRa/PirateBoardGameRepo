using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Defines what a crewcard is, will connect crewcard data and behaviour
/// </summary>

[RequireComponent(typeof(CardDisplayScript))] //should attach card UI to cards
[RequireComponent(typeof(CardBehaviorScript))] //should attach card movement script to all cards
public class CCard : MonoBehaviour
{
    public CrewCardScriptable CardData;


    //set CrewCard data at runtime and Update the cards UI
    public void Setup(CrewCardScriptable data)
    {
        CardData = data;
        GetComponent<CardDisplayScript>().SetCardUI();
    }
}
