using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst;
	
	[SerializeField] private GameField _gameField;
	[SerializeField] private PlayerInput _playerInput;
	private bool _gameIsRunning = true;

	public static GameField GameField { get => _inst._gameField; }
	public static PlayerInput PlayerInput { get => _inst._playerInput; }
	public static bool GameIsRunning { get => _inst._gameIsRunning; }
	public static void OverGame() => _inst.OverGameImpl();

	private void Awake()
	{
		if (_inst != null) Destroy(gameObject);
		else _inst = this;
	}

	private void OverGameImpl()
	{
		_gameIsRunning = false;
		_gameField.StopScrolling();
		print("Over Game!");
	}
}
