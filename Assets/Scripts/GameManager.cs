using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst;
	
	[SerializeField] private GameField _gameField;
	[SerializeField] private Player _player;

	[HideInInspector] public bool GameIsRunning = true;
	public static GameField GameField { get => _inst._gameField; }
	public static Player Player { get => _inst._player; }


	public static void OverGame() => _inst.OverGameImpl();
	private void Awake()
	{
		if (_inst != null) Destroy(gameObject);
		else _inst = this;
	}

	private void OverGameImpl()
	{
		GameIsRunning = false;
		GameField.StopScrolling();
		print("Over Game!");
	}
}
