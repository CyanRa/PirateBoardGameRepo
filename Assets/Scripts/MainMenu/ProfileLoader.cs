using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;


public class ProfileLoader : MonoBehaviour
{
    public TextAsset JsonToLoadFrom;
    public TextMeshProUGUI inputProfileText;
    public Profiles loadedProfile;
    private Profiles profilesToSave = new Profiles();
    private Profile profile = new Profile();
    private static string Path => Application.dataPath + "/Profiles/Profiles.json";
    void Start()
    {
        loadedProfile = JsonUtility.FromJson<Profiles>(JsonToLoadFrom.text);
    }

    public void SaveProfile(){
        bool alreadyExists = false;
        profile.profileName = inputProfileText.text;
        foreach(Profile tempProfile in loadedProfile.loadedProfiles){
            if(tempProfile.profileName == profile.profileName){
                alreadyExists = true;
                Debug.Log("Profile already exists");
            }
        }
        if(alreadyExists)return;
        profilesToSave.loadedProfiles.Add(profile);
        string json = JsonUtility.ToJson(profilesToSave, true);
        File.WriteAllText(Path, json);
    }
    public void UpdateProfileList(){
        loadedProfile = JsonUtility.FromJson<Profiles>(JsonToLoadFrom.text);
    }
 

    


}
