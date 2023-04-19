using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private Vector2 _bgPitch;
    [SerializeField] private float _scrollSpeedMax;
    [SerializeField] private float _bgFadeTime;
    [SerializeField] private float _bgChangeTime;
    [SerializeField] private float _bgDimVolume;
    [Header("Sounds")]
    [SerializeField] private AudioSource _audioCoin;
    [SerializeField] private AudioSource _audioPlayerMoved;
    [SerializeField] private AudioSource _audioPlayerDead;
    [SerializeField] private AudioSource _audioChain;
    [SerializeField] private AudioSource _audioOnGameStopped;
    [SerializeField] private AudioSource _audioOnPlatformChangeState;
    [SerializeField] private AudioSource _audioBack;
    [SerializeField] private AudioSource _audioBackCalm;

    private float _scrollSpeedStart;
    private float _bgVolume;

    private void Start()
	{
        _scrollSpeedStart = GameManager.GameField.ScrollSpeed;
        _bgVolume = _audioBack.volume;
        StartCoroutine(FadeInBack());
    }

	private void Update()
	{
        var t = (GameManager.GameField.ScrollSpeed - _scrollSpeedStart) / (_scrollSpeedMax - _scrollSpeedStart);
        _audioBack.pitch = Mathf.Lerp(_bgPitch.x, _bgPitch.y, t);
    }

    private IEnumerator FadeInBack()
	{
        _audioBack.volume = 0;
        _audioBack.Play();
        for (float t = 0; t < 1; t += Time.deltaTime / _bgFadeTime)
        {
            _audioBack.volume = Mathf.Lerp(0, _bgVolume, t);
            yield return new WaitForEndOfFrame();
        }
        _audioBack.volume = _bgVolume;
    }

    public void ChangeBackToCalm()
	{
        StartCoroutine(ChangeBack());
    }

    private IEnumerator ChangeBack()
    {
        var volumeCalm = _audioBackCalm.volume;
        _audioBackCalm.volume = 0;
        _audioBackCalm.Play();
        for (float t = 0; t < 1; t += Time.deltaTime / _bgChangeTime)
        {
            _audioBack.volume = Mathf.Lerp(_bgVolume, 0, t);
            _audioBackCalm.volume = Mathf.Lerp(0, volumeCalm, t);
            yield return new WaitForEndOfFrame();
        }
        _audioBackCalm.volume = volumeCalm;
        _audioBack.Stop();
    }

    public void PauseEnable()
	{
        _audioBack.volume = _bgDimVolume;
    }

    public void PauseDisable()
    {
        _audioBack.volume = _bgVolume;
    }

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
