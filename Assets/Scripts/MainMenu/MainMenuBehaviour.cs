using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Alteruna;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuBehaviour : CommunicationBridge
{
    public Profile selectedProfile;
    public GameObject MainMenu;
    public GameObject MultiplayerMenu;
    public GameObject ProfileMenu;
    public GameObject ProfileCreatorMenu;
    public GameObject ProfileDeleteMenu;
    public GameObject OptionsMenu;
    public GameObject ProfileDisplayPrefab;
    public GameObject ProfileDisplayParent;
    public GameObject ProfileDeleteParent;
    public GameObject ProfileDisplayPrefabToDelete;
    private ProfileLoader _profileLoader;
    public AudioSource myAudioSource;
    public AudioClip selectShipAudioClip;
    public static string USERNAME;

    public Multiplayer myMultiplayer;
    public void Start()
    {
        _profileLoader = gameObject.GetComponent<ProfileLoader>();
        if(_profileLoader.loadedProfile != null){
            selectedProfile = _profileLoader.loadedProfile.loadedProfiles[0];
            USERNAME = selectedProfile.profileName;
        }
        PlaySelectShipAudioClip();
        
    }
     public void PlaySelectShipAudioClip(){
        myAudioSource.PlayOneShot(selectShipAudioClip);
    }

    public void OpenProfileMenu(){
        MainMenu.SetActive(false);
        ProfileMenu.SetActive(true);      
        _profileLoader.UpdateProfileList();
        InstantiateProfiles(_profileLoader.loadedProfile.loadedProfiles);
    }
    public void OpenProfileCreation(){
        ProfileCreatorMenu.SetActive(true);     
        ProfileMenu.SetActive(false);     
    }
    public void OpenMainMenu(){
        MainMenu.SetActive(true);
        ProfileMenu.SetActive(false);
    }
    public void CloseProfileCreation(){
        ProfileCreatorMenu.SetActive(false);
        ProfileMenu.SetActive(true); 
        InstantiateProfiles(_profileLoader.loadedProfile.loadedProfiles); 
    }
    private void InstantiateProfiles(List<Profile> _profiles){
        foreach(Transform _child in ProfileDisplayParent.transform){
            Destroy(_child.gameObject);
        }
        foreach(Profile _profile in _profiles){
            GameObject profileChoiceButton = Instantiate(ProfileDisplayPrefab);
            profileChoiceButton.GetComponent<Button>().onClick.AddListener(SelectProfile);
            profileChoiceButton.GetComponentInChildren<TextMeshProUGUI>().text = _profile.profileName;
            profileChoiceButton.transform.SetParent(ProfileDisplayParent.transform);
        }
    }

    private void InstantiateProfilesToDelete(List<Profile> _profiles){
        foreach(Transform _child in ProfileDisplayParent.transform){
            Destroy(_child.gameObject);
        }
        foreach(Profile _profile in _profiles){
            GameObject profileChoiceButton = Instantiate(ProfileDisplayPrefabToDelete);
            profileChoiceButton.GetComponent<Button>().onClick.AddListener(DeleteProfile);
            profileChoiceButton.GetComponentInChildren<TextMeshProUGUI>().text = _profile.profileName;
            profileChoiceButton.transform.SetParent(ProfileDeleteParent.transform);
        }
    }
    public void OpenMultiplayerPanel(){
        USERNAME = selectedProfile.profileName;
        MultiplayerMenu.SetActive(true);
        Multiplayer.Connect();
    }

    public void SelectProfile(){
        USERNAME = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        myMultiplayer.SetUsername(USERNAME);
        Debug.Log("Selected +" + USERNAME);
        MainMenu.SetActive(true);
        ProfileMenu.SetActive(false);

    }

    public void OpenProfileDeleteMenu(){
        ProfileMenu.SetActive(false);
        ProfileDeleteMenu.SetActive(true);
        InstantiateProfilesToDelete(_profileLoader.loadedProfile.loadedProfiles); 
    }
    public void DeleteProfile(){
        _profileLoader.DeleteProfile(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text);
        foreach(Transform _child in ProfileDeleteParent.transform){
            Destroy(_child.gameObject);
        }
        CloseProfileDeleteMenu();
    }

    public void CloseProfileDeleteMenu(){
        ProfileMenu.SetActive(true);
        ProfileDeleteMenu.SetActive(false);
        InstantiateProfiles(_profileLoader.loadedProfile.loadedProfiles); 

    }
    public void QuitGame(){
        Application.Quit();
    }

}
