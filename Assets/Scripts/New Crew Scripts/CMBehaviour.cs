using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using static UnityEngine.UI.Image;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using Alteruna;

public class CMBehaviour : MonoBehaviour
{
    public bool isTutor; //still need to make isTutor logic make sepperate class
    public bool isSelected;
    public bool isCommitted;

    public TextMeshProUGUI displayedName;
    public TextMeshProUGUI displayedPower;
    public Image displayedImage;

    public Transform committedZone;
    public CrewMember crewMember;
    public Hand myHand;

    
    public void Start()
    {
        isSelected = false;
        isCommitted = false;
        isCommitted = false;
        try{
            Button button = GameObject.Find("CommitCardsButton").GetComponent<Button>();
            button.onClick.AddListener(CommitCrewToBattle);
        }
        catch(Exception e){
            Debug.Log("Card instantiated for display only");
        }
        
    }

    public void LoadCardDisplay()
    {
        displayedName.text = crewMember.crewMemberName;
        displayedPower.text = crewMember.crewMemberPower.ToString();
        displayedImage.sprite = Resources.Load<Sprite>(crewMember.crewMemberImage);
    }

    //method for select cards, still need to add actual highlighting of cards.
    public void SelectThisCrew()
    {
        if(myHand.battleManager.turnOwner == myHand.battleManager.myTurnID){
            if (!isSelected)
        {
            isSelected = true;
            Debug.Log("is Selected");
            GetComponent<Image>().enabled = true;
        }
        else
        {
            isSelected = false;
            Debug.Log("is Deselected");
            GetComponent<Image>().enabled = false;
        }
        }
        
    }

    //method for commiting this card
    public void CommitCrewToBattle()
    {
        if(myHand.battleManager.turnOwner == myHand.battleManager.myTurnID){
        if (isSelected)
        {
            this.transform.SetParent(committedZone);       
            isSelected = false;
            isCommitted = true;
            myHand.battleManager.cardsPlayedLastTurn = true;
            if(myHand.battleManager.myTurnID == 0){
                myHand.battleManager.defenderPower += GetComponent<CMBehaviour>().crewMember.crewMemberPower;
            }else{
                myHand.battleManager.attackerPower += GetComponent<CMBehaviour>().crewMember.crewMemberPower;
            }
            //handScript.CommitCard(GetComponent<CrewMember>());
        }
        }
        
    }
}
