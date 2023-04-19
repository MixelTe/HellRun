using System.Collections;
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
	[SerializeField] private SoundPlayer _soundPlayer;
	[SerializeField] private PauseRiddleSpawner _pauseRiddleSpawner;
	[SerializeField] private Player _player;
	private bool _gameIsRunning = true;
	private bool _gameIsPaused = false;
	private bool _rewardUsed = false;

	public static GameField GameField { get => _inst._gameField; }
	public static PlayerInput PlayerInput { get => _inst._playerInput; }
	public static ChainSpawner ChainSpawner { get => _inst._chainSpawner; }
	public static Score Score { get => _inst._score; }
	public static GameUI GameUI { get => _inst._gameUI; }
	public static SoundPlayer SoundPlayer { get => _inst._soundPlayer; }
	public static PauseRiddleSpawner PauseRiddleSpawner { get => _inst._pauseRiddleSpawner; }
	public static Player Player { get => _inst._player; }
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
		if (_rewardUsed)
		{
			EndGame();
		}
		else
		{
			Time.timeScale = 0;
			_gameIsPaused = true;
			_gameUI.ShowGameOver();
			_chainSpawner.DestroyChains();
			_pauseRiddleSpawner.DestroyObstacles();
			print("Over Game!");
		}
	}

	public void EndGame()
	{
		Time.timeScale = 1;
		_gameIsPaused = false;
		_gameIsRunning = false;
		_gameField.StopScrolling();
		_gameUI.ShowEndGame();
		_soundPlayer.ChangeBackToCalm();
		print("End Game!");
	}

	public void UseReward()
	{
		_rewardUsed = true;
		_gameIsPaused = false;
		_gameUI.ShowGame();

		StartCoroutine(ReRunGame());

		print("Reward Used!");
	}
	private IEnumerator ReRunGame()
	{
		Time.timeScale = 0;
		_player.Immortal = true;
		_player.Reborn();
		for (float t = 0; t < 1; t += Time.unscaledDeltaTime / 2)
		{
			Time.timeScale = Mathf.Lerp(0, 1, t);

			yield return new WaitForEndOfFrame();
		}
		_player.Immortal = false;
		Time.timeScale = 1;
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
