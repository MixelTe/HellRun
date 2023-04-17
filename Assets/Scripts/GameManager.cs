using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager _inst;
	
	[SerializeField] private GameField _gameField;
	[SerializeField] private PlayerInput _playerInput;
	[SerializeField] private ChainSpawner _chainSpawner;
	[SerializeField] private Score _score;
	[SerializeField] private GameUI _gameUI;
	private bool _gameIsRunning = true;
	private bool _gameIsPaused = false;

	public static GameField GameField { get => _inst._gameField; }
	public static PlayerInput PlayerInput { get => _inst._playerInput; }
	public static ChainSpawner ChainSpawner { get => _inst._chainSpawner; }
	public static Score Score { get => _inst._score; }
	public static GameUI GameUI { get => _inst._gameUI; }
	public static bool GameIsRunning { get => _inst._gameIsRunning; }
	public static bool GameIsPaused { get => _inst._gameIsPaused; }
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

	public void PauseGame()
	{
		Time.timeScale = 0;
		_gameIsPaused = true;
		_gameUI.ShowPause();
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1;
		_gameIsPaused = false;
		_gameUI.HidePause();
	}
}
