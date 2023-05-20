using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Ingame")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private PoppingText _score;

    [Header("Controls")]
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private GameObject _single;
    [SerializeField] private GameObject _double;
    [SerializeField] private float _widthForDouble;

    [Header("Pause")]
    [SerializeField] private GameObject _pausePanel;

    [Header("Game over")]
    [SerializeField] private GameObject _overPanel;
    [SerializeField] private LocalizeTextDynamic _recordLeftText;
    [SerializeField] private PoppingText _scoreFinal;
    [SerializeField] private Button _advButton;
    [SerializeField] private Notify _advError;

    [Header("End Game")]
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private PoppingText _scoreFinal2;
    [SerializeField] private PoppingText _scoreRecord;

    [Header("Leaderboard")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Leaderboard _leaderboard;
    [SerializeField] private Button _authButton;

    private LeaderboardDataRecord _playerData;
    private LeaderboardDataRecord _playerDataNew;

    private void Start()
    {
        ShowGame();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        _authButton.onClick.AddListener(() => Auth());
        _advButton.onClick.AddListener(() => ShowReward());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        UpdateScore(0);
        UpdateControls();
    }

    private void UpdateControls()
    {
        if (YaApi.Mobile())
		{
            if (_canvas.rect.width >= _widthForDouble)
            {
                _double.SetActive(true);
                _single.SetActive(false);
            }
            else
            {
                _double.SetActive(false);
                _single.SetActive(true);
            }
		}
		else
        {
            _double.SetActive(false);
            _single.SetActive(false);
        }
    }

    private void SetActivePanel(Panel panel)
	{
        _gamePanel.SetActive(panel == Panel.Game);
        _pausePanel.SetActive(panel == Panel.Pause);
        _overPanel.SetActive(panel == Panel.Over);
        _endPanel.SetActive(panel == Panel.End);
        _leaderboardPanel.SetActive(panel == Panel.Leaderboard);
    }
	enum Panel
	{
        Game,
        Pause,
        Over,
        End,
        Leaderboard,
    }

    private async Task Auth()
	{
        var auth = await YaApi.Auth();
        Debug.Log($"Auth: {auth}");
        if (auth && !GameManager.GameIsRunning)
		{
		    await YaApi.UpdateRecord(_playerData);
            _playerDataNew = await YaApi.PlayerData();
            _leaderboard.UpdateData(_playerDataNew);
            _authButton.gameObject.SetActive(false);
        }
    }

    public void ShowGame()
    {
        SetActivePanel(Panel.Game);
    }

    public void ShowGameOver()
	{
        SetActivePanel(Panel.Over);
        _recordLeftText.SetText("Continue");
        _scoreFinal.SetText("Empty");
        GameOver();
    }
    private async void GameOver()
    {
        _playerData = await YaApi.PlayerData();
        if (GameManager.Score.PlayerScore <= _playerData.Score)
		{
            _recordLeftText.SetText("Left");
            _scoreFinal.SetText("Left", _playerData.Score - GameManager.Score.PlayerScore);
            _scoreFinal.Pop();
		}
		else
        {
            _scoreFinal.SetText("Score", $"{GameManager.Score.PlayerScore}");
            _scoreFinal.Pop();
        }
    }

    private async void ShowReward()
	{
        GameManager.SoundSetting.Mute();
        var rewarded = await YaApi.Reward();
        GameManager.SoundSetting.UnMute();

        if (rewarded) GameManager.UseReward();
		else
		{
			GameManager.EndGame();
            _advError.Show();
		}
	}

    public void ShowEndGame()
    {
        YaApi.MetrikaGoal(YaApi.MetrikaGoals.Gameover);
        SetActivePanel(Panel.End);
        _scoreFinal2.Pop();
        _scoreRecord.SetText("Loading");
        EndGame();
    }
    private async void EndGame()
	{
        while (_playerData == null)
            await Task.Yield();
        _scoreRecord.SetText("Score", _playerData.Score);
        _scoreRecord.Pop();
        await YaApi.UpdateRecord(_playerData);
        _playerDataNew = await YaApi.PlayerData();
    }

    public void ShowLeaderboard()
    {
        SetActivePanel(Panel.Leaderboard);

        ShowAdv();
        LoadLeaderboard();

        bool hasAuth = YaApi.IsAuth();
        _authButton.gameObject.SetActive(!hasAuth);
    }

    private async void ShowAdv()
	{
        GameManager.SoundSetting.Mute();
        await YaApi.Adv();
        GameManager.SoundSetting.UnMute();
    }

    private async void LoadLeaderboard()
    {
        while (_playerDataNew == null)
            await Task.Yield();
        _leaderboard.UpdateData(_playerDataNew);
    }

    public void UpdateScore(int score, bool pop = false)
	{
        _score.SetText("Score", score);
        _scoreFinal.SetText("Score", score);
        _scoreFinal2.SetText("Score", score);

        if (pop)
            _score.Pop();
    }

    public void ShowPause()
    {
        YaApi.MetrikaGoal(YaApi.MetrikaGoals.Paused);
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void HidePause()
    {
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
    }
}
