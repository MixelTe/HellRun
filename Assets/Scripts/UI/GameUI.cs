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
    [SerializeField] private PoppingText _newRecord;
    [SerializeField] private ParticleSystem _particles;

    [Header("Leaderboard")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Leaderboard _leaderboard;
    [SerializeField] private Button _authButton;
    [SerializeField] private Notify _authError;
    [SerializeField] private NewRankAnim _newRankAnim;

    [Header("Rate Game")]
    [SerializeField] private GameObject _ratePanel;
    [SerializeField] private GameObject _rateThanksPanel;
    [SerializeField] private LeaderboardRecord _playerRecord;
    [SerializeField] private LeaderboardRecord _playerRecord2;
    [SerializeField] private Button _rateButton;
    
    [Header("Badges")]
    [SerializeField] private GameObject _badgesPanel;


    private LeaderboardDataRecord _playerData;
    private LeaderboardDataRecord _playerDataNew;
    private NewRankAnimData _newRankAnimData;
    private bool _isMobile;
    private bool _advClosed = false;

    private void Start()
    {
        ShowGame();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        _authButton.onClick.AddListener(() => Auth());
        _advButton.onClick.AddListener(() => ShowReward());
        _rateButton.onClick.AddListener(() => Rate());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        UpdateScore(0);
        _isMobile = YaApi.Mobile();
        _newRecord.SetText("Empty");
    }

	private void Update()
	{
        if (_isMobile)
		{
            var isDouble = _canvas.rect.width >= _widthForDouble;
            _double.SetActive(isDouble);
            _single.SetActive(!isDouble);
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
        _ratePanel.SetActive(panel == Panel.Rate);
        _rateThanksPanel.SetActive(panel == Panel.RateThanks);
        _badgesPanel.SetActive(panel == Panel.Badges);
    }
	enum Panel
	{
        Game,
        Pause,
        Over,
        End,
        Leaderboard,
        Rate,
        RateThanks,
        Badges,
    }

    private async Task Auth()
	{
		var auth = await YaApi.Auth();
		Debug.Log($"Auth: {auth}");
		if (GameManager.GameIsRunning)
			return;
		
        if (auth)
		{
			_authButton.gameObject.SetActive(false);
			await YaApi.UpdateRecord(_playerData);
            var r = await _leaderboard.UpdateData();
			_playerDataNew = r.PlayerRecord;
		}
		else
		{
			_authError.Show();
		}
    }

    private async Task Rate()
    {
        var rate = await YaApi.Rate(_playerDataNew);
		Debug.Log($"Rate: {rate}");
		if (GameManager.GameIsRunning)
			return;

		if (rate)
		{
			SetActivePanel(Panel.RateThanks);
			_leaderboard.UpdatePlayer(_playerDataNew);
            _particles.Play();
		}
		else
		{
			ReturnToLeaderboard();
		}
	}

    public void ShowGame()
    {
        SetActivePanel(Panel.Game);
    }

    public void ShowGameOver()
	{
        YaApi.MetrikaGoal(YaApi.MetrikaGoals.ContinueScreen);
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
        YaApi.GamePlayed();
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
        if (GameManager.Score.PlayerScore > _playerData.Score)
		{
            _newRecord.SetText("NewRecord");
            _newRecord.Pop();
            _particles.Play();
        }
        var r = await _leaderboard.UpdateData();
        _playerDataNew = r.PlayerRecord;
        _newRankAnimData = r.newRankAnim;
    }

    public void ShowLeaderboard()
    {
        SetActivePanel(Panel.Leaderboard);

        ShowAdv();
        LoadLeaderboard();

        bool hasAuth = YaApi.IsAuth();
        _authButton.gameObject.SetActive(!hasAuth);

        _leaderboard.UpdateScroll();
    }

    private async void ShowAdv()
	{
        _advClosed = false;
        GameManager.SoundSetting.Mute();
        await YaApi.Adv();
        _advClosed = true;
        GameManager.SoundSetting.UnMute();
    }

    private async void LoadLeaderboard()
    {
        while (_playerDataNew == null)
            await Task.Yield();
        _newRankAnimData.PastRank = _playerData.Rank;
        while (!_advClosed)
            await Task.Yield();
        if (_newRankAnimData.RecordPlayer != null && _newRankAnimData.RecordPlayer.Rank < _newRankAnimData.PastRank)
		{
            _newRankAnim.Show(_newRankAnimData);
            YaApi.MetrikaGoal(YaApi.MetrikaGoals.NewRank);
        }
        else
            TryShowRate();
    }

    private async void TryShowRate()
	{
        var gamesPlayed = _playerDataNew.GamesPlayed;
        if (!(gamesPlayed == 10 || (gamesPlayed > 10 && gamesPlayed % 8 == 0)))
            return;
        
        var canRate = await YaApi.CanRate(_playerDataNew);
		if (!canRate)
		{
            if (_playerDataNew.RatedGame)
                _leaderboard.UpdatePlayer(_playerDataNew);
			return;
		}

		ShowRate();
	}

    private void ShowRate()
    {
        YaApi.MetrikaGoal(YaApi.MetrikaGoals.RateScreen);
        SetActivePanel(Panel.Rate);
        _playerDataNew.RatedGame = true;
        _playerRecord.Init(_playerDataNew);
        _playerRecord2.Init(_playerDataNew);
        _playerDataNew.RatedGame = false;
    }

    public void ReturnToLeaderboard()
    {
        SetActivePanel(Panel.Leaderboard);
    }

    public void ShowBadges()
	{
        YaApi.MetrikaGoal(YaApi.MetrikaGoals.Badges);
        SetActivePanel(Panel.Badges);
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
