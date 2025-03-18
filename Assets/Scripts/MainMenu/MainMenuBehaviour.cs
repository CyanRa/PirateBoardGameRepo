using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MainMenuBehaviour : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject MultiplayerMenu;
    public GameObject ProfileMenu;
    public GameObject ProfileCreatorMenu;
    public GameObject OptionsMenu;
    public GameObject ProfileDisplayPrefab;
    public GameObject ProfileDisplayParent;
    private ProfileLoader _profileLoader;

    public Profile selectedProfile;

    public void Start()
    {
        _profileLoader = gameObject.GetComponent<ProfileLoader>();
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
    private void InstantiateProfiles(List<Profile> _profiles){
        foreach(Profile _profile in _profiles){
            GameObject profileChoiceButton = Instantiate(ProfileDisplayPrefab);
            profileChoiceButton.GetComponentInChildren<TextMeshProUGUI>().text = _profile.profileName;
            profileChoiceButton.transform.SetParent(ProfileDisplayParent.transform);
        }
    }
    public void OpenMultiplayerPanel(){
        MultiplayerMenu.SetActive(true);
    }

}
