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
	[SerializeField] private SoundSetting _soundSetting;
	[SerializeField] private CameraController _cameraController;
	private bool _gameIsRunning = true;
	private bool _gameIsPaused = false;
	private bool _rewardUsed = false;

	public static bool Exist { get => _inst != null; }
	public static GameField GameField { get => _inst._gameField; }
	public static PlayerInput PlayerInput { get => _inst._playerInput; }
	public static ChainSpawner ChainSpawner { get => _inst._chainSpawner; }
	public static Score Score { get => _inst._score; }
	public static GameUI GameUI { get => _inst._gameUI; }
	public static SoundPlayer SoundPlayer { get => _inst._soundPlayer; }
	public static PauseRiddleSpawner PauseRiddleSpawner { get => _inst._pauseRiddleSpawner; }
	public static Player Player { get => _inst._player; }
	public static SoundSetting SoundSetting { get => _inst._soundSetting; }
	public static CameraController CameraController { get => _inst._cameraController; }
	public static bool GameIsRunning { get => _inst._gameIsRunning; }
	public static bool GameIsPaused { get => _inst._gameIsPaused; }
	public static void OverGame() => _inst.OverGameImpl();
	public static void EndGame() => _inst.EndGameImpl();
	public static void UseReward() => _inst.UseRewardImpl();
	public static void TogglePause()
	{
		if (_inst._gameIsPaused) _inst.UnpauseGame();
		else _inst.PauseGame();
	}

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
			EndGameImpl();
		}
		else
		{
			Time.timeScale = 0;
			_gameIsPaused = true;
			_gameUI.ShowGameOver();
			_soundPlayer.PauseEnable();
			print("Over Game!");
		}
	}

	private void EndGameImpl()
	{
		Time.timeScale = 1;
		_gameIsPaused = false;
		_gameIsRunning = false;
		_gameField.StopScrolling();
		_gameUI.ShowEndGame();
		_soundPlayer.ChangeBackToCalm();
		print("End Game!");
	}

	private void UseRewardImpl()
	{
		_rewardUsed = true;
		_gameIsPaused = false;
		_gameUI.ShowGame();
		_soundPlayer.PauseDisable();
		_chainSpawner.DestroyChains();
		_pauseRiddleSpawner.DestroyObstacles();

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
		YaApi.MetrikaGoal(YaApi.MetrikaGoals.Restart);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ToStartScene()
	{
		YaApi.MetrikaGoal(YaApi.MetrikaGoals.Home);
		SceneManager.LoadScene("StartScene");
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
		_gameIsPaused = true;
		_gameUI.ShowPause();
		_soundPlayer.PauseEnable();
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1;
		_gameIsPaused = false;
		_gameUI.HidePause();
		_soundPlayer.PauseDisable();
	}
}
