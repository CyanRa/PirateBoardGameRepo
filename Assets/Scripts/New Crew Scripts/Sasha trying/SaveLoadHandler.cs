using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadHandler : MonoBehaviour
{
    public TextAsset JsonToLoadFrom;
     public TextAsset JsonToLoadFrom2;
    public FightersData loadedFighters = new FightersData();
    public FightersData deck2 = new FightersData();

    void Start()
    {
        loadedFighters = JsonUtility.FromJson<FightersData>(JsonToLoadFrom.text);  
        deck2 = JsonUtility.FromJson<FightersData>(JsonToLoadFrom2.text);  
    }
    
}
