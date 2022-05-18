using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private float musicVolume = 1;
    private float soundVolume = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public delegate void VolumeSetter (float volume);
    public void VolumeUpdate()
    {
        MusicVolumeUpdate();
        SoundVolumeUpdate();
    }

    public event VolumeSetter onMusicVolumeChange;
    public void MusicVolumeUpdate()
    {
        if (onMusicVolumeChange != null)
        {
            onMusicVolumeChange(musicVolume);
        }
    }
    public void MusicVolumeSet(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
    }

    public event VolumeSetter onSoundVolumeChange;
    public void SoundVolumeUpdate()
    {
        if (onSoundVolumeChange != null)
        {
            onSoundVolumeChange(soundVolume);
        }
    }
    public void SoundVolumeSet(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
    }
}
