using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class YaApi : MonoBehaviour
{
	private static YaApi _inst;
	public static Task<LeaderboardData> Leaderboard() => _inst.LoadLeaderboard();
	public static Task<LeaderboardDataRecord> PlayerData() => _inst.LoadPlayerData();
	public static bool IsAuth() => CheckAuth();
	public static Task<bool> Auth() => _inst.AskAuth();
	public static Task<int> Record() => GetRecord();
	public static Task UpdateRecord() => _inst.SetRecord();
	public static bool Mobile() => IsMobile();

	private void Awake()
	{
		_inst = this;
	}

	private LeaderboardData _leaderboardData;
	private async Task<LeaderboardData> LoadLeaderboard()
	{
		GetLeaderboard();
		while (_leaderboardData == null)
			await Task.Delay(100);
		var leaderboardData = _leaderboardData;
		_leaderboardData = null;
		return leaderboardData;
	}

	private LeaderboardDataRecord _playerData;
	private async Task<LeaderboardDataRecord> LoadPlayerData()
	{
		GetPlayerData();
		while (_playerData == null)
			await Task.Delay(100);
		var playerData = _playerData;
		_playerData = null;
		if (!playerData.IsPlayer)
		{
			playerData.IsPlayer = true;
			playerData.Score = GameManager.Score.PlayerScore;
		}
		return playerData;
	}

	private int _auth = -1;
	private async Task<bool> AskAuth()
	{
		AuthPlayer();
		while (_auth < 0)
			await Task.Delay(250);
		var auth = _auth;
		_auth = -1;
		return auth == 1;
	}

	private static async Task<int> GetRecord()
	{
		if (CheckAuth())
		{
			var data = await _inst.LoadPlayerData();
			var score = data.Score;
			PlayerPrefs.SetInt(Settings.PlayerPrefsRecordScore, score);
			return score;
		}
		else
		{ 
			var score = PlayerPrefs.GetInt(Settings.PlayerPrefsRecordScore, 0);
			return score;
		}
	}

	private int _scoreUpdated = -1;
	private async Task SetRecord()
	{
		var record = await GetRecord();
		var score = GameManager.Score.PlayerScore;
		if (score > record)
		{
			PlayerPrefs.SetInt(Settings.PlayerPrefsRecordScore, score);
			if (CheckAuth())
			{
				SetScore(score);
				while (_scoreUpdated < 0)
					await Task.Delay(50);
				_scoreUpdated = -1;
			}
		}
	}

#if UNITY_EDITOR
	private static void GetLeaderboard()
	{
		var json = Path.Join(Application.dataPath, "Yandex", "leaderboard.json");
		if (!File.Exists(json))
		{
			Debug.LogError($"File dont exist: {json}");
			return;
		}

		using var file = File.OpenText(json);
		_inst.SetLeaderboard(file.ReadToEnd());
	}
#else
	[DllImport("__Internal")]
	private static extern void GetLeaderboard();
#endif

	public void SetLeaderboard(string data)
	{
		_leaderboardData = JsonUtility.FromJson<LeaderboardData>(data);
	}


#if UNITY_EDITOR
	private static void GetPlayerData()
	{
		var data = new LeaderboardDataRecord()
		{
			IsPlayer = false,
			Avatar = "",
			Name = "Вы",
			Rank = 0,
			Score = 0,
		};
		_inst.SetPlayerData(JsonUtility.ToJson(data));
	}
#else
	[DllImport("__Internal")]
	private static extern void GetPlayerData();
#endif

	public void SetPlayerData(string data)
	{
		_playerData = JsonUtility.FromJson<LeaderboardDataRecord>(data);
	}


#if UNITY_EDITOR
	private static bool CheckAuth()
	{
		return false;
	}
#else
	[DllImport("__Internal")]
	private static extern bool CheckAuth();
#endif


#if UNITY_EDITOR
	private static void AuthPlayer()
	{
		_inst.OnAuth(0);
	}
#else
	[DllImport("__Internal")]
	private static extern void AuthPlayer();
#endif

	public void OnAuth(int auth)
	{
		_auth = auth;
	}


#if UNITY_EDITOR
	private static void SetScore(int score)
	{
		_inst.OnScoreUpdated(score);
	}
#else
	[DllImport("__Internal")]
	private static extern void SetScore(int score);
#endif

	public void OnScoreUpdated(int updated)
	{
		_scoreUpdated = updated;
	}


#if UNITY_EDITOR
	private static bool IsMobile()
	{
		return false;
	}
#else
	[DllImport("__Internal")]
	private static extern bool IsMobile();
#endif
}
