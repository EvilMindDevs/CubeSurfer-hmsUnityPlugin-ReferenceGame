using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] public AudioSource _musicSource;
    [SerializeField] public AudioSource _soundsSource;
    [SerializeField] bool audioEnabled = true;
    [SerializeField] GameObject mutedButton;

    public void PlayMusic(AudioClip clip){
        _musicSource.clip = clip;
        _musicSource.Play();
    }
    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1){
        _soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }
    public void PlaySound(AudioClip clip, float vol = 1){
        _soundsSource.PlayOneShot(clip,vol);
    }

    public void ManageAudio()
    {
        if (audioEnabled)
        {
            AudioListener.volume = 0;
            audioEnabled = false;
            mutedButton.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (!audioEnabled)
        {
            AudioListener.volume = 0.5f;
            audioEnabled = true;
            mutedButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        
    }

}
