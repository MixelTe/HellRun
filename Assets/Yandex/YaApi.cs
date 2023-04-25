using System.IO;
using System.Runtime.InteropServices;
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
	public static Task<bool> Reward() => _inst.UseReward();
	public static Task Adv() => _inst.RunAdv();

	private void Awake()
	{
		_inst = this;
	}

	private LeaderboardData _leaderboardData;
	private async Task<LeaderboardData> LoadLeaderboard()
	{
		GetLeaderboard();
		while (_leaderboardData == null)
			await Task.Yield();
		var leaderboardData = _leaderboardData;
		_leaderboardData = null;
		Debug.Log("Leaderboard loaded");
		return leaderboardData;
	}

	private LeaderboardDataRecord _playerData;
	private async Task<LeaderboardDataRecord> LoadPlayerData()
	{
		GetPlayerData();
		while (_playerData == null)
			await Task.Yield();
		var playerData = _playerData;
		_playerData = null;
		if (!playerData.IsPlayer)
		{
			playerData.IsPlayer = true;
			if (GameManager.Exist)
				playerData.Score = GameManager.Score.PlayerScore;
			else
				playerData.Score = PlayerPrefs.GetInt(Settings.PlayerPrefsRecordScore, 0);
		}
		Debug.Log("PlayerData loaded");
		return playerData;
	}

	private int _auth = -1;
	private async Task<bool> AskAuth()
	{
		AuthPlayer();
		while (_auth < 0)
			await Task.Yield();
		var auth = _auth;
		_auth = -1;
		Debug.Log($"Auth requested: {auth == 1}");
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
		Debug.Log("SetRecord");
		var record = await GetRecord();
		var score = GameManager.Score.PlayerScore;
		if (score > record)
		{
			Debug.Log("SetRecord: New record");
			PlayerPrefs.SetInt(Settings.PlayerPrefsRecordScore, score);
			if (CheckAuth())
			{
				SetScore(score);
				while (_scoreUpdated < 0)
					await Task.Yield();
				Debug.Log("SetRecord: Score set");
				_scoreUpdated = -1;
			}
		}
	}

	private bool _reward = false;
	private bool _rewardClosed = false;
	private async Task<bool> UseReward()
	{
		_reward = false;
		_rewardClosed = false;
		ShowRewardAdv();
		while (!_rewardClosed)
			await Task.Yield();
		Debug.Log($"Reward used: {_reward}");
		return _reward;
	}

	private bool _adv = false;
	private async Task RunAdv()
	{
		_adv = false;
		ShowAdv();
		while (!_adv)
			await Task.Yield();
	}

#if UNITY_EDITOR
	private static void GetLeaderboard()
	{
		var data = new LeaderboardData() {
			Records = new LeaderboardDataRecord[] {
				new LeaderboardDataRecord() { ID = "1", Score = 57, Rank = 6, Name = "Super Player 3000", Avatar = "https://avatars.yandex.net/get-music-content/5236179/6e5cc28a.p.3404212/100x100" },
				new LeaderboardDataRecord() { ID = "2", Score = 780, Rank = 1, Name = "Vasya", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
				new LeaderboardDataRecord() { ID = "3", Score = 650, Rank = 2, Name = "Ультра Вжик", Avatar = "https://avatars.yandex.net/get-music-content/3927581/777ae0d7.a.13658486-1/100x100" },
				new LeaderboardDataRecord() { ID = "4", Score = 495, Rank = 3, Name = "Dub Dubom", Avatar = "https://avatars.yandex.net/get-music-content/38044/f88a8857.a.3839675-1/100x100" },
				new LeaderboardDataRecord() { ID = "5", Score = 215, Rank = 4, Name = "The Петя", Avatar = "https://avatars.yandex.net/get-music-content/32236/c21fa65f.p.59307/100x100" },
				new LeaderboardDataRecord() { ID = "6", Score = 104, Rank = 5, Name = "Победятор", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
				new LeaderboardDataRecord() { ID = "7", Score = 10, Rank = 7, Name = "Лучший!", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
			}
		};
		_inst.SetLeaderboard(JsonUtility.ToJson(data));
	}
#else
	[DllImport("__Internal")]
	private static extern void GetLeaderboard();
#endif

	public void SetLeaderboard(string data)
	{
		_leaderboardData = JsonUtility.FromJson<LeaderboardData>(data);
		Debug.Log($"SetLeaderboard: {data}");
	}


#if UNITY_EDITOR
	private static void GetPlayerData()
	{
		var data = new LeaderboardDataRecord()
		{
			ID = "",
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
		Debug.Log($"SetPlayerData: {_playerData}");
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
		Debug.Log($"OnAuth: {auth}");
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
		Debug.Log($"OnScoreUpdated: {updated}");
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


#if UNITY_EDITOR
	private static void ShowRewardAdv()
	{
		_inst.OnReward(1);
		_inst.OnReward(0);
	}
#else
	[DllImport("__Internal")]
	private static extern void ShowRewardAdv();
#endif

	public void OnReward(int got)
	{
		Debug.Log($"OnReward: {got}");
		if (got == 0)
			_rewardClosed = true;
		else
			_reward = got > 0;
	}


#if UNITY_EDITOR
	private static void ShowAdv()
	{
		Debug.Log("ShowAdv");
		_inst.OnAdvClosed();
	}
#else
	[DllImport("__Internal")]
	private static extern void ShowAdv();
#endif

	public void OnAdvClosed()
	{
		_adv = true;
	}
}
