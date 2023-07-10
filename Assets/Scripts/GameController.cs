using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //The time for which the game will accelerate by 1
    [SerializeField] private float _scrollSpeedAcceleration = 120f;
    [SerializeField] private int _scoreToStop = 500;
    private int _lastScoreStop = 0;

    private void Start()
    {
		GameManager.GameField.OnLineMoved += OnLineMoved;
        GameManager.PlayerInput.OnMoved += StartGame;
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

    private void StartGame(Vector2Int vector2)
    {
        GameManager.PlayerInput.OnMoved -= StartGame;
        GameManager.GameField.StartScrolling();
        GameManager.ChainSpawner.StartSpawn();
    }
}