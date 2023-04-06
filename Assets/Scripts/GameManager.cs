using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst;
	[SerializeField] private GameField _gameField;

	public static GameField GameField { get => _inst._gameField; }
	[HideInInspector] public bool GameIsRunning = true;


	private void Awake()
	{
		if (_inst != null) Destroy(gameObject);
		else _inst = this;
	}

	public void OverGame()
	{
		GameIsRunning = false;
		GameField.StopScrolling();
	}
}
