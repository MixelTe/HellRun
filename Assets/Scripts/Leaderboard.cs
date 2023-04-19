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

    private bool _updated = false;

    private void Start()
	{
        if (_updated) return;
		_container.transform.DestroyAllChildren();
		Instantiate(_recordPrefab, _container)
			.Init(new LeaderboardDataRecord()
			{
				IsPlayer = true,
				Score = GameManager.Score.PlayerScore,
				Name = "Вы",
			});
		Instantiate(_recordPrefab, _container)
		    .Init(new LeaderboardDataRecord()
		    {
			    Name = "Загрузка...",
		    });
	}

	public async void UpdateData()
    {
        var data = await YaApi.Leaderboard();
        await AddPlayerToData(data);
        _updated = true;
        _container.transform.DestroyAllChildren();
        LeaderboardRecord playerRecord = null;
        foreach (var recordData in data.Records)
        {
            var record = Instantiate(_recordPrefab, _container);
            record.Init(recordData);
            if (recordData.IsPlayer)
                playerRecord = record;
        }
        if (playerRecord != null)
            _scrollRect.ScrollTo(playerRecord.GetComponent<RectTransform>());
    }

	private async Task AddPlayerToData(LeaderboardData data)
	{
        var playerRecord = await YaApi.PlayerData();
        data.Records = data.Records.Append(playerRecord).OrderBy(v => -v.Score).ToArray();
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
    public int Score;
    public int Rank;
    public string Avatar;
    public string Name;
    public bool IsPlayer;
}