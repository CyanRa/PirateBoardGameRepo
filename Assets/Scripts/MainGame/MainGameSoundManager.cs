using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MainGameSoundManager : MonoBehaviour
{
    public AudioClip mainGameTheme;
    private AudioSource myAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.PlayOneShot(mainGameTheme);
        
    }

    
}
