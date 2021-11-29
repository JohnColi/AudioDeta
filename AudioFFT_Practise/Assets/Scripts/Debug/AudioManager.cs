using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float f)
    {
        Debug.Log("111");
        audioMixer.SetFloat("MasterVolume", f);
    }

    public void SetMusicVolume(float f)
    {
        Debug.Log("222");
        audioMixer.SetFloat("MusicVolume", f);
    }

    public void SetSoundEffectVolume(float f)
    {
        Debug.Log("333");
        audioMixer.SetFloat("SoundEffectVolume", f);
    }
}
