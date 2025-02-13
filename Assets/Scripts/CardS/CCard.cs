using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Defines what a crewcard is, will connect crewcard data and behaviour
/// </summary>

[RequireComponent(typeof(CardDisplayScript))] // not 100% sure this is refrencing the correct script or if this even makes sense in the context of what im doing
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
