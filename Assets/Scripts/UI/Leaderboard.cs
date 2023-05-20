using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _container;
    [SerializeField] private LeaderboardRecord _recordPrefab;
    [SerializeField] private LocalizedString _text_itsYou;
    [SerializeField] private LocalizedString _text_Loading;
    [SerializeField] private LocalizedString _text_HiddenPlayer;

    private bool _updated = false;

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

    public async void UpdateData(LeaderboardDataRecord playerData = null)
    {
        var data = await YaApi.Leaderboard();
        await AddPlayerToData(data, playerData);
        _updated = true;
        _container.transform.DestroyAllChildren();
        LeaderboardRecord playerRecord = null;
        foreach (var recordData in data.Records)
        {
            if (recordData.Name == "")
                recordData.Name = _text_HiddenPlayer.Value;

            var record = Instantiate(_recordPrefab, _container);
            record.Init(recordData);
            if (recordData.IsPlayer)
                playerRecord = record;
        }
        if (playerRecord != null)
            _scrollRect.ScrollTo(playerRecord.GetComponent<RectTransform>());
    }

	private async Task AddPlayerToData(LeaderboardData data, LeaderboardDataRecord playerData)
	{
        var playerRecord = playerData ?? await YaApi.PlayerData();
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
}

[Serializable]
public class LeaderboardDataRecord
{
    public string ID;
    public int Score;
    public int Rank;
    public string Avatar;
    public string Name;
    public bool IsPlayer;
    public bool HasSavedRecord;
}