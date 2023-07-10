using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _container;
    [SerializeField] private LeaderboardRecord _recordPrefab;
    [SerializeField] private LocalizedString _text_itsYou;
    [SerializeField] private LocalizedString _text_Loading;
    [SerializeField] private LocalizedString _text_HiddenPlayer;

    private bool _updated = false;
    private LeaderboardRecord _playerRecord;

	private void OnEnable()
	{
        if (_playerRecord != null)
            StartCoroutine(ScrollPlayerIntoCenter());
            
    }
    private IEnumerator ScrollPlayerIntoCenter()
	{
        yield return new WaitForEndOfFrame();
        _scrollRect.ScrollTo(_playerRecord.GetComponent<RectTransform>());
    }

    private void Start()
	{
        if (_updated) return;
		_container.transform.DestroyAllChildren();
        if (GameManager.Exist)
		    Instantiate(_recordPrefab, _container)
			    .Init(new LeaderboardDataRecord()
			    {
				    IsPlayer = true,
				    Score = GameManager.Score.PlayerScore,
				    Name = _text_itsYou.Value,
			    });
		Instantiate(_recordPrefab, _container)
		    .Init(new LeaderboardDataRecord()
		    {
			    Name = _text_Loading.Value,
		    });
	}

    public async Task<LeaderboardUpdateResult> UpdateData()
    {
        var data = await YaApi.Leaderboard();
        ProcessData(data);
        _updated = true;
        _container.transform.DestroyAllChildren();
        _playerRecord = null;
        foreach (var recordData in data.Records)
        {
            if (recordData.Name == "")
                recordData.Name = _text_HiddenPlayer.Value;

            var record = Instantiate(_recordPrefab, _container);
            record.Init(recordData);
            if (recordData.IsPlayer)
                _playerRecord = record;
        }
        if (_playerRecord != null)
		{
            _scrollRect.ScrollTo(_playerRecord.GetComponent<RectTransform>());
            var playerI = Array.IndexOf(data.Records, _playerRecord.Data);
            var recordNext1 = playerI - 2 >= 0 ? data.Records[playerI - 2] : null;
            var recordNext2 = playerI - 1 >= 0 ? data.Records[playerI - 1] : null;
            var recordPlayer = data.Records[playerI];
            var recordPast1 = playerI + 1 < data.Records.Length ? data.Records[playerI + 1] : null;
            var recordPast2 = playerI + 2 < data.Records.Length ? data.Records[playerI + 2] : null;
            var recordPast3 = playerI + 3 < data.Records.Length ? data.Records[playerI + 3] : null;

            return new LeaderboardUpdateResult(data.PlayerRecord) {
                newRankAnim = new NewRankAnimData(recordNext1, recordNext2, recordPlayer, recordPast1, recordPast2, recordPast3, 0) 
            };
        }
        return new LeaderboardUpdateResult(data.PlayerRecord);
    }

    public void UpdatePlayer(LeaderboardDataRecord playerData)
	{
        _scrollRect.ScrollTo(_playerRecord.GetComponent<RectTransform>());
        _playerRecord.Init(playerData);
    }

    public void UpdateScroll()
	{
        if (_playerRecord != null)
            _scrollRect.ScrollTo(_playerRecord.GetComponent<RectTransform>());
    }

    private void ProcessData(LeaderboardData data)
	{
        var playerRecord = data.PlayerRecord;
        if (!playerRecord.HasSavedRecord || playerRecord.Name == "") 
            playerRecord.Name = _text_itsYou.Value;
        data.Records = data.Records
            .Where(v => v.ID != playerRecord.ID)
            .Append(playerRecord)
            .OrderBy(v => -v.Score)
            .ToArray();
    }
}

[Serializable]
public class LeaderboardData
{
    public LeaderboardDataRecord[] Records;
    public LeaderboardDataRecord PlayerRecord;
}

[Serializable]
public class LeaderboardDataRecord
{
    public string ID = "";
    public int Score = 0;
    public int Rank = 0;
    public string Avatar = "";
    public string Name = "";
    public bool IsPlayer = false;
    public bool HasSavedRecord = false;
    public bool RatedGame = false;
    public bool WasTop = false;
    public bool WasFirst = false;
    public bool HasGear = false;
    public int GamesPlayed = 0;
}

public struct LeaderboardUpdateResult
{
    public NewRankAnimData newRankAnim;
    public LeaderboardDataRecord PlayerRecord;

	public LeaderboardUpdateResult(LeaderboardDataRecord playerRecord) : this()
	{
		PlayerRecord = playerRecord;
	}
}
