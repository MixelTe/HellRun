using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst;
	
	[SerializeField] private GameField _gameField;
	[SerializeField] private PlayerInput _playerInput;
	[SerializeField] private ChainSpawner _chainSpawner;
	[SerializeField] private GameUI _gameUI;
	private bool _gameIsRunning = true;

	public static GameField GameField { get => _inst._gameField; }
	public static PlayerInput PlayerInput { get => _inst._playerInput; }
	public static ChainSpawner ChainSpawner { get => _inst._chainSpawner; }
	public static GameUI GameUI { get => _inst._gameUI; }
	public static bool GameIsRunning { get => _inst._gameIsRunning; }
	public static void OverGame() => _inst.OverGameImpl();

	private void Awake()
	{
		_inst = this;
	}

	private void OnEnable()
	{
		_inst = this;
	}

	private void OverGameImpl()
	{
		_gameIsRunning = false;
		_gameField.StopScrolling();
		_gameUI.ShowGameOver();
		print("Over Game!");
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
