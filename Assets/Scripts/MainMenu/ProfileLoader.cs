using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Linq;
using Unity.VisualScripting;


public class ProfileLoader : MonoBehaviour
{
    public TextAsset JsonToLoadFrom;
    public TextMeshProUGUI inputProfileText;
    public Profiles loadedProfile = new Profiles();
    private Profiles profilesToSave = new Profiles();
    private Profile profile = new Profile();
    private static string Path => Application.persistentDataPath + "/Profiles/Profiles.json";
    void Start()
    {
        File.SetAttributes(Path, FileAttributes.Normal);
        loadedProfile = JsonUtility.FromJson<Profiles>(File.ReadAllText(Path));
    }

    public void SaveProfile(){
        bool alreadyExists = false;
        profile.profileName = inputProfileText.text;
        foreach(Profile tempProfile in loadedProfile.loadedProfiles){
            if(tempProfile.profileName == profile.profileName){
                alreadyExists = true;
            }
        }
        if(alreadyExists)return;
        loadedProfile.loadedProfiles.Add(profile);
        string json = JsonUtility.ToJson(loadedProfile, true);
        File.WriteAllText(Path, json);
        UpdateProfileList();
    }

    public void DeleteProfile(string _profile){
        foreach(Profile tempProfile in loadedProfile.loadedProfiles.ToList()){
            if(tempProfile.profileName == _profile){
                loadedProfile.loadedProfiles.Remove(tempProfile);
            }
        }
        string json = JsonUtility.ToJson(loadedProfile, true);
        File.WriteAllText(Path, json);
        UpdateProfileList();
    }
    public void UpdateProfileList(){
        loadedProfile = JsonUtility.FromJson<Profiles>(File.ReadAllText(Path));
    }
 

    


}
