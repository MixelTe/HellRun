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
        if (PlayerPrefs.HasKey(Settings.PlayerPrefs_SoundVolume))
        {
            var volume = PlayerPrefs.GetFloat(Settings.PlayerPrefs_SoundVolume);
            _allSounSlider.value = volume;
            SetSoundVolume(volume);
        }

        if (PlayerPrefs.HasKey(Settings.PlayerPrefs_MusicVolume))
        {
            var volume = PlayerPrefs.GetFloat(Settings.PlayerPrefs_MusicVolume);
            _musicSlider.value = volume;
            SetMusicVolume(volume);
        }
    }

    public void SetSoundVolume(float volume)
    {
        SetVolume("sounds", Settings.PlayerPrefs_SoundVolume, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume("music", Settings.PlayerPrefs_MusicVolume, volume);
    }

    private void SetVolume(string mixerKey, string prefsKey, float volumeLiniar)
    {
        var volume = Mathf.Log(volumeLiniar) * 20f;
        _audioMixer.SetFloat(mixerKey, volume);
        PlayerPrefs.SetFloat(prefsKey, volumeLiniar);
        PlayerPrefs.Save();
    }

    public void Mute()
	{
        _audioMixer.SetFloat("master", -80);
    }

    public void UnMute()
    {
        _audioMixer.SetFloat("master", 0);
    }
}
