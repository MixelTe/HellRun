using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //The time for which the game will accelerate by 1
    [SerializeField] private float _scrollSpeedAcceleration  = 120f;
    [SerializeField] private int _scoreToStop = 500;
    [SerializeField] private float _timeBeforeFirstStrike = 2f;
    private int _lastScoreStop = 0;

    private void Start()
    {
		GameManager.GameField.OnLineMoved += OnLineMoved;
        StartCoroutine(StartChains());
    }

	private void Update()
    {
        if (GameManager.GameIsRunning && GameManager.GameField.Scroling)
        {
            GameManager.GameField.ScrollSpeed += Time.deltaTime / _scrollSpeedAcceleration;
        }
    }

    private void OnLineMoved()
    {
        if (GameManager.Score.PlayerScore - _lastScoreStop > _scoreToStop)
        {
            GameManager.GameField.StopScrolling();
            _lastScoreStop += _scoreToStop;
        }
    }

    private IEnumerator StartChains()
    {
        yield return new WaitForSeconds(_timeBeforeFirstStrike);
        GameManager.ChainSpawner.StartSpawn();
    }
}