using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CMSaveLoadHandler : MonoBehaviour
{
    public TextAsset JsonToLoadFrom;
    public TextAsset JsonToLoadFrom2;
    public CMData loadedCrewMember = new CMData();
    public CMData tutorDeck = new CMData();

    public void Start()
    {
        loadedCrewMember = JsonUtility.FromJson<CMData>(JsonToLoadFrom.text);
        tutorDeck = JsonUtility.FromJson<CMData>(JsonToLoadFrom2.text);
    }

}
