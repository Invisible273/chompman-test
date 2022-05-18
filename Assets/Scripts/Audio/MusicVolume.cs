using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicVolume : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.instance.onMusicVolumeChange += ChangeVolume;
    }

    private void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private void OnDestroy() {
        AudioManager.instance.onMusicVolumeChange -= ChangeVolume;
    }
}
