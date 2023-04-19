using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _allSounSlider;

    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            _allSounSlider.value = PlayerPrefs.GetFloat("volume");
            SetSoundVolume(PlayerPrefs.GetFloat("volume"));
        }

        if (PlayerPrefs.HasKey("music"))
        {
            _musicSlider.value = PlayerPrefs.GetFloat("music");
            SetMusicVolume(PlayerPrefs.GetFloat("music"));
        }
    }

    public void SetSoundVolume(float volume)
    {
        _audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat("music", volume);
    }
}
