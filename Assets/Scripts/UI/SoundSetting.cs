using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _soundChange;
	private float _sendMetrikaDelay = -1;
	private float _soundDelay = -1;
    private bool _disableOnChange = true;

    private void Start()
    {
        if (PlayerPrefs.HasKey(Settings.PlayerPrefs_SoundVolume))
        {
            var volume = PlayerPrefs.GetFloat(Settings.PlayerPrefs_SoundVolume);
            _soundSlider.value = volume;
            SetSoundVolume(volume);
        }

        if (PlayerPrefs.HasKey(Settings.PlayerPrefs_MusicVolume))
        {
            var volume = PlayerPrefs.GetFloat(Settings.PlayerPrefs_MusicVolume);
            _musicSlider.value = volume;
            SetMusicVolume(volume);
        }
        _soundSlider.onValueChanged.AddListener(SetSoundVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _disableOnChange = false;
    }

    private void SetSoundVolume(float volume)
    {
        SetVolume("sounds", Settings.PlayerPrefs_SoundVolume, volume);
        PlaySound();
    }

    private void SetMusicVolume(float volume)
    {
        SetVolume("music", Settings.PlayerPrefs_MusicVolume, volume);
    }

    private void SetVolume(string mixerKey, string prefsKey, float volumeLiniar)
    {
        var volume = Mathf.Log(volumeLiniar) * 20f;
        _audioMixer.SetFloat(mixerKey, volume);
        PlayerPrefs.SetFloat(prefsKey, volumeLiniar);
        PlayerPrefs.Save();
        SendMetrika();
    }

    public void Mute()
	{
        _audioMixer.SetFloat("master", -80);
    }

    public void UnMute()
    {
        _audioMixer.SetFloat("master", 0);
    }

    private async void SendMetrika()
	{
        if (_disableOnChange)
            return;

        var alreadyRan = _sendMetrikaDelay >= 0;
        _sendMetrikaDelay = 2;
        if (alreadyRan)
            return;

		while (_sendMetrikaDelay > 0)
		{
            _sendMetrikaDelay -= Time.unscaledDeltaTime;
            await Task.Yield();
        }

        YaApi.MetrikaGoal(YaApi.MetrikaGoals.VolumeChanged);
        _sendMetrikaDelay = -1;
    }

    private async void PlaySound()
    {
        if (_disableOnChange)
            return;

        var alreadyRan = _soundDelay >= 0;
        _soundDelay = 0.15f;
        if (alreadyRan)
            return;

        while (_soundDelay > 0)
        {
            _soundDelay -= Time.unscaledDeltaTime;
            await Task.Yield();
        }

        _soundChange.Play();
        _soundDelay = -1;
    }
}
