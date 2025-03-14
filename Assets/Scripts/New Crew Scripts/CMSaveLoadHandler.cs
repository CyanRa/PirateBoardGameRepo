using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using System.Linq;
using Alteruna;

public class CMSaveLoadHandler : AttributesSync
{
    public TextAsset JsonToLoadFrom;
    public TextAsset JsonToLoadFrom2;
    public CMData loadedCrewMember = new CMData();
    public CMData tutorDeck = new CMData();

    public List<CrewMember> DrawPileCrewMember = new List<CrewMember>();
    public List<CrewMember> DiscardPileCrewMember = new List<CrewMember>();
    public List<CrewMember> DrawPileTutor = new List<CrewMember>();

    public void Start()
    {
        loadedCrewMember = JsonUtility.FromJson<CMData>(JsonToLoadFrom.text);
        tutorDeck = JsonUtility.FromJson<CMData>(JsonToLoadFrom2.text);
        DrawPileCrewMember = loadedCrewMember.crewMember;
        ShuffleDeck();
    }


    public CrewMember ReturnDrawCard(){
        if(DrawPileCrewMember.Count > 0){
            CrewMember _crewMember = DrawPileCrewMember[0];
            BroadcastRemoteMethod("RemoveTopCardFromCrewMemberDeck");
            return _crewMember;   
        }else{
            DrawPileCrewMember = DiscardPileCrewMember;
            ShuffleDeck();
            CrewMember _crewMember = DrawPileCrewMember[0];
            BroadcastRemoteMethod("RemoveTopCardFromCrewMemberDeck");
            return _crewMember; 
        }
            
    }

    [SynchronizableMethod]
    public void RemoveTopCardFromCrewMemberDeck(){
        DrawPileCrewMember.RemoveAt(0);
    }


    [SynchronizableMethod]
    public void ShuffleDeck(){
       List<CrewMember> shuffledList = DrawPileCrewMember.OrderBy( x => UnityEngine.Random.value ).ToList();
       DrawPileCrewMember = shuffledList;
    }

    [SynchronizableMethod]
    public void AddCrewMemberToDiscardPile(CrewMember _crewMember){

    }

    
}
