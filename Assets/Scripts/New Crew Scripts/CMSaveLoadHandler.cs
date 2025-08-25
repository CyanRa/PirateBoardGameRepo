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
    public List<CrewMember> CrewMembersInGame = new List<CrewMember>();

    public void Start()
    {
        loadedCrewMember = JsonUtility.FromJson<CMData>(JsonToLoadFrom.text);
        tutorDeck = JsonUtility.FromJson<CMData>(JsonToLoadFrom2.text);
        DrawPileCrewMember = loadedCrewMember.crewMember;
        CrewMembersInGame = tutorDeck.crewMember;
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
    public CrewMember ReturnDrawOnePowerCard()
    {
        if (DrawPileCrewMember.Any(f => f.crewMemberName == "Swinging Swashbuckler"))
        {
            int index = DrawPileCrewMember.FindIndex(a => a.crewMemberName == "Swinging Swashbuckler");
            CrewMember _crewMember = DrawPileCrewMember[index];
            BroadcastRemoteMethod("RemoveIndexCardFromCrewMemberDeck", index);
            return _crewMember;
        }
        else
        {
            return null;
        }
    }

    [SynchronizableMethod]
    public void RemoveTopCardFromCrewMemberDeck(){
        DrawPileCrewMember.RemoveAt(0);
    }
    [SynchronizableMethod]
    public void RemoveIndexCardFromCrewMemberDeck(int index){
        DrawPileCrewMember.RemoveAt(index);
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
