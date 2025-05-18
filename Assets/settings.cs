using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    Resolution[] resolutions;
    int resToUse;
    public TMP_Dropdown resolutionDropdown;
    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen; 
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentRes = 0;
        
        List<String> options = new List<string>();
        foreach(Resolution resolution in resolutions){
            string option = resolution.width + "x" + resolution.height;
            options.Add(option);
            
            if(resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height){
                resToUse = currentRes; 
            }
            currentRes++;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = resToUse;
        resolutionDropdown.RefreshShownValue();
        
    }

    public void SetResolution(int resIndex){
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

}
