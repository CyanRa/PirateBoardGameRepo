using UnityEngine;

public class AnimationSoundController : MonoBehaviour
{
    
    public AudioSource myAudioSource;
    public AudioClip StartTurnSwordHitAudioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
     public void PlayStartTurnSound(){
            myAudioSource.PlayOneShot(StartTurnSwordHitAudioClip);
    }
}
