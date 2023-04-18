using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Header("Sounds")] 
    [SerializeField] private AudioSource _audioCoin;
    [SerializeField] private AudioSource _audioPlayerMoved;
    [SerializeField] private AudioSource _audioPlayerDead;
    [SerializeField] private AudioSource _audioChain;
    [SerializeField] private AudioSource _audioOnGameStopped;
    [SerializeField] private AudioSource _audioOnPlatformChangeState;

    public void PlayCoinSound()
    {
        if (!GameManager.GameIsRunning) return;
        _audioCoin.Play();
    }

    public void PlayPlayerMovedSound(float pitch)
    {
        if (!GameManager.GameIsRunning) return;
        _audioPlayerMoved.pitch = pitch*5;
        _audioPlayerMoved.Play();
    }

    public void PlayPlayerDeadSound()
    {
        if (!GameManager.GameIsRunning) return;
        _audioPlayerDead.Play();
    }

    public void PlayChainSound()
    {
        if (!GameManager.GameIsRunning) return;
        _audioChain.Play();
    }

    public void PlayPlatformStoppedSound()
    {
        if (!GameManager.GameIsRunning) return;
        _audioOnGameStopped.Play();
    }

    public void PlayOnPlatformChangeStateSound()
    {
        if (!GameManager.GameIsRunning) return;
        _audioOnPlatformChangeState.Play();
    }
}
