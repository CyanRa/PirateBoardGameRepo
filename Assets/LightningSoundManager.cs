using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LightningSoundManager : MonoBehaviour
{
    private ParticleSystem myParticleSystem;
    private int _currentNumberOfParticles;

    public AudioClip lightningStrike;
    public AudioClip rainAudio;
    private AudioSource myAudioSource;
    private bool playLightning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        myAudioSource = GetComponent<AudioSource>();
        playLightning = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(myParticleSystem.particleCount > 0 && playLightning == true){
            PlayThunderSound();
            playLightning = false;
            StartCoroutine(ResetLightningSoundEffect());

        }
        
    }

    private void PlayThunderSound()
    {
        myAudioSource.PlayOneShot(lightningStrike);
   
    }
    private IEnumerator ResetLightningSoundEffect(){
        yield return new WaitForSeconds(2);
        playLightning = true;
    }
}
