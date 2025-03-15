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

    public void SelectThisCrew()
    {
        if(myHand.battleManager.turnOwner == myHand.battleManager.myTurnID){
            if (!isSelected)
        {
            isSelected = true;
            GetComponent<Image>().enabled = true;
        }
        else
        {
            isSelected = false;
            GetComponent<Image>().enabled = false;
        }
        }
        
    }


    public void CommitCrewToBattle()
    {
        if(!myHand.avatar.IsMe)return;

        if(!myHand.battleManager.MyTurn() || !isSelected)return;        
            GetComponent<Image>().enabled = false;
            this.transform.SetParent(committedZone);       
            isSelected = false;
            isCommitted = true;
            myHand.battleManager.cardsPlayedLastTurn = true;
            
            if(myHand.battleManager.myTurnID == 0){
                myHand.battleManager.defenderPower += GetComponent<CMBehaviour>().crewMember.crewMemberPower;
                myHand.battleManager.InvokeDisplayCommitedCard(myHand.battleManager.attackerUID);
            }else{
                myHand.battleManager.attackerPower += GetComponent<CMBehaviour>().crewMember.crewMemberPower;
                myHand.battleManager.InvokeDisplayCommitedCard(myHand.battleManager.defenderUID);
            }
            myHand.battleManager.InvokeOpponentHandDisplay(myHand.myFleetCrew.Count - myHand.committedZone.childCount);       
        
    }
}
