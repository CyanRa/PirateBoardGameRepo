using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class rainAudioBehaviour : MonoBehaviour
{
    private ParticleSystem myParticleSystem;

    public AudioClip rainAudio;
    private AudioSource myAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.PlayOneShot(rainAudio);
        
    }

    
}
