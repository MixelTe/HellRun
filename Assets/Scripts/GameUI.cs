using System.Threading.Tasks;
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
    [SerializeField] private PoppingText _scoreFinal;
    [SerializeField] private Button _advButton;

    [Header("End Game")]
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private PoppingText _scoreFinal2;
    [SerializeField] private PoppingText _scoreRecord;

    [Header("Leaderboard")]
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Leaderboard _leaderboard;
    [SerializeField] private Button _authButton;

    private bool _isMobile;

    private void Start()
    {
        ShowGame();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        _authButton.onClick.AddListener(() => Auth());
        _advButton.onClick.AddListener(() => ShowReward());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        _isMobile = YaApi.Mobile();
    }

    private void Update()
    {
        if (_isMobile)
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

    private async Task Auth()
	{
        var auth = await YaApi.Auth();
        Debug.Log($"Auth: {auth}");
        if (auth)
            ShowLeaderboard();
    }

    public void ShowGame()
    {
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(false);
        _endPanel.SetActive(false);
        _leaderboardPanel.SetActive(false);
    }

    public void ShowGameOver()
	{
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(true);
        _endPanel.SetActive(false);
        _leaderboardPanel.SetActive(false);
        _scoreFinal.Pop();
    }

    private async void ShowReward()
	{
        var rewarded = await YaApi.Reward();
        if (rewarded) GameManager.UseReward();
        else GameManager.EndGame();
	}

    public void ShowEndGame()
    {
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(false);
        _endPanel.SetActive(true);
        _leaderboardPanel.SetActive(false);
        _scoreFinal2.Pop();
        _scoreRecord.SetText("Загрузка рекорда");
        LoadRecord();
    }
    private async void LoadRecord()
	{
        var record = await YaApi.Record();
        _scoreRecord.SetText($"Рекорд: {record}");
        _scoreRecord.Pop();
    }

    public void ShowLeaderboard()
    {
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(false);
        _endPanel.SetActive(false);
        _leaderboardPanel.SetActive(true);

        if (YaApi.IsAuth())
            SaveRecord();
		else
            _leaderboard.UpdateData();
    }
    private async void SaveRecord()
    {
        _authButton.gameObject.SetActive(false);
        await YaApi.UpdateRecord();
        _leaderboard.UpdateData();
    }

    public void UpdateScore(int score, bool pop = false)
	{
        _score.SetText("Счёт: " + score);
        _scoreFinal.SetText(score.ToString());
        _scoreFinal2.SetText("Счёт: " + score);

        if (pop)
            _score.Pop();
    }

    public void ShowPause()
    {
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void HidePause()
    {
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
    }
}
