using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using NUnit.Framework;
using Dreamteck;
using JetBrains.Annotations;

public class JsonHandler : MonoBehaviour
{
    public TextAsset JsonToLoadFrom;
    public CommunityDeck communityDeckToPopulate;
    public CrewMember deWrappingCrewMember;
    List<CrewMember> listToReturn = new List<CrewMember>();
    
    [System.Serializable]
    public class LoadedCrewMember{
        public string name;
        public string image;
        public int power;
        public bool isTutor;
        public bool isCommited;
        public bool isSelected;
    }
    [System.Serializable]
    public class LoadedDeck{
        public List<LoadedCrewMember> loadedCrewMembers;
    }

    public LoadedDeck myStarterDeck = new LoadedDeck();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myStarterDeck = JsonUtility.FromJson<LoadedDeck>(JsonToLoadFrom.text);
       
        //CommunityDeck.Instance._crewDeck = myStarterDeck.loadedCrewMembers;        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
