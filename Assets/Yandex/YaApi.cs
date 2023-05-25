using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class YaApi : MonoBehaviour
{
	public static Task<LeaderboardData> Leaderboard() => _inst.LoadLeaderboard();
	public static Task<LeaderboardDataRecord> PlayerData() => _inst.LoadPlayerData();
	public static bool IsAuth() => CheckAuth();
	public static Task<bool> Auth() => _inst.AskAuth();
	public static Task UpdateRecord(LeaderboardDataRecord currentData) => _inst.SetRecord(currentData);
	public static bool Mobile() => IsMobile();
	public static Task<bool> Reward() => _inst.UseReward();
	public static Task Adv() => _inst.RunAdv();
	public static Languages Language() => GetLanguage();
	public static void MetrikaStatus(LeaderboardDataRecord currentData) => SetUserStatus(currentData);
	public static void MetrikaGoal(MetrikaGoals goal) => SendMetrikaGoal(goal);
	
	private static YaApi _inst;
	[Header("Dev values")]
	[SerializeField] private bool _isMobile;
	[SerializeField] private bool _isAuth;
	[SerializeField] private Languages _language;

	private void Awake()
	{
		_inst = this;
	}
	private void Start()
	{
		OnStart();
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
		if (!playerData.HasSavedRecord)
			playerData.Score = PlayerPrefs.GetInt(Settings.PlayerPrefsRecordScore, 0);
		SetUserStatus(playerData);
		Debug.Log("PlayerData loaded");
		return playerData;
	}

	private int _auth = -1;
	private async Task<bool> AskAuth()
	{
		MetrikaGoal(MetrikaGoals.AuthTry);
		AuthPlayer();
		while (_auth < 0)
			await Task.Yield();
		var authSuccessful = _auth == 1;
		_auth = -1;
		Debug.Log($"Auth requested: {authSuccessful}");
		if (authSuccessful)
			MetrikaGoal(MetrikaGoals.Auth);
		return authSuccessful;
	}

	private int _scoreUpdated = -1;
	private async Task SetRecord(LeaderboardDataRecord currentData)
	{
		Debug.Log("SetRecord");
		var record = currentData.HasSavedRecord ? 
			currentData.Score : 
			PlayerPrefs.GetInt(Settings.PlayerPrefsRecordScore, 0);
		var score = GameManager.Score.PlayerScore;
		if (score > record)
		{
			Debug.Log("SetRecord: New record");
			MetrikaGoal(MetrikaGoals.NewRecord);

			PlayerPrefs.SetInt(Settings.PlayerPrefsRecordScore, score);
			PlayerPrefs.Save();
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
		MetrikaGoal(MetrikaGoals.Adv);
		_reward = false;
		_rewardClosed = false;
		ShowRewardAdv();
		while (!_rewardClosed)
			await Task.Yield();
		Debug.Log($"Reward used: {_reward}");
		if (!_reward)
			MetrikaGoal(MetrikaGoals.AdvError);
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

	private static Languages GetLanguage()
	{
		var lang = GetLang();
		var langs = Enum.GetValues(typeof(Languages));
		if (lang >= langs.Length)
			Debug.Log($"Wrong language: {lang}");
		return (Languages)(lang % langs.Length);
	}

	private static void SendMetrikaGoal(MetrikaGoals goal)
	{
		var goalStr = MetrikaGoalsToString(goal);
		if (goalStr == "")
		{
			Debug.Log($"Wrong Goal: {goal}");
			return;
		}
		SendMetrika(goalStr);
	}
	private static string MetrikaGoalsToString(MetrikaGoals goal)
	{
		return goal switch
		{
			MetrikaGoals.Open => "open",
			MetrikaGoals.LanguageChanged => "language_changed",
			MetrikaGoals.VolumeChanged => "volume_changed",
			MetrikaGoals.Leaderboard => "leaderboard",
			MetrikaGoals.Start => "start",
			MetrikaGoals.Paused => "paused",
			MetrikaGoals.Adv => "adv",
			MetrikaGoals.AdvError => "adv_error",
			MetrikaGoals.Gameover => "gameover",
			MetrikaGoals.NewRecord => "new_record",
			MetrikaGoals.AuthTry => "auth_try",
			MetrikaGoals.Auth => "auth",
			MetrikaGoals.Restart => "restart",
			MetrikaGoals.Home => "home",
			MetrikaGoals.Error => "error",
			_ => "",
		};
	}
	public enum MetrikaGoals
	{
		Open,
		LanguageChanged,
		VolumeChanged,
		Leaderboard,
		Start,
		Paused,
		Adv,
		AdvError,
		Gameover,
		NewRecord,
		AuthTry,
		Auth,
		Restart,
		Home,
		Error,
	}

	private static void SetUserStatus(LeaderboardDataRecord currentData)
	{
		var rank = currentData.Rank;
		var record = currentData.Score;
		var volume_sound = Mathf.RoundToInt(PlayerPrefs.GetFloat(Settings.PlayerPrefs_SoundVolume, 1) / 2 * 100); // 2 - max volume, 100 - 100%
		var volume_music = Mathf.RoundToInt(PlayerPrefs.GetFloat(Settings.PlayerPrefs_MusicVolume, 1) / 2 * 100);
		var auth = IsAuth();
		var language = (int)Localization.Language;
		UpdateUserStatus(rank, record, volume_sound, volume_music, auth, language);
	}



#if UNITY_EDITOR
	private static void OnStart()
	{
		Debug.Log("OnStart");
	}
#else
	[DllImport("__Internal")]
	private static extern void OnStart();
#endif


#if UNITY_EDITOR
	private static void GetLeaderboard()
	{
		var data = new LeaderboardData() {
			Records = new LeaderboardDataRecord[] {
				new LeaderboardDataRecord() { ID = "1", Score = 57, Rank = 6, Name = "Super Player 3000", Avatar = "https://avatars.yandex.net/get-music-content/5236179/6e5cc28a.p.3404212/100x100" },
				new LeaderboardDataRecord() { ID = "2", Score = 780, Rank = 1, Name = "Vasya", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
				new LeaderboardDataRecord() { ID = "3", Score = 650, Rank = 3, Name = "Ультра Вжик", Avatar = "https://avatars.yandex.net/get-music-content/3927581/777ae0d7.a.13658486-1/100x100" },
				new LeaderboardDataRecord() { ID = "4", Score = 495, Rank = 4, Name = "Dub Dubom", Avatar = "https://avatars.yandex.net/get-music-content/38044/f88a8857.a.3839675-1/100x100" },
				new LeaderboardDataRecord() { ID = "5", Score = 215, Rank = 5, Name = "The Петя", Avatar = "https://avatars.yandex.net/get-music-content/32236/c21fa65f.p.59307/100x100" },
				new LeaderboardDataRecord() { ID = "6", Score = 104, Rank = 7, Name = "Победятор", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
				new LeaderboardDataRecord() { ID = "7", Score = 10, Rank = 8, Name = "Лучший!", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle" },
				new LeaderboardDataRecord() { ID = "8", Score = 780, Rank = 2, Name = "", Avatar = "" },
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
		Debug.Log($"SetLeaderboard: {_leaderboardData != null} {data}");
	}


#if UNITY_EDITOR
	private static void GetPlayerData()
	{
		var data = new LeaderboardDataRecord()
		{
			ID = "",
			IsPlayer = true,
			Avatar = "",
			Name = "",
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
		Debug.Log($"SetPlayerData: {_playerData != null} {data}");
	}


#if UNITY_EDITOR
	private static bool CheckAuth()
	{
		if (_inst != null)
			return _inst._isAuth;
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
		if (_inst != null) 
			return _inst._isMobile;
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


#if UNITY_EDITOR
	private static int GetLang()
	{
		if (_inst != null)
			return (int)_inst._language;
		return 0;
	}
#else
	[DllImport("__Internal")]
	private static extern int GetLang();
#endif



#if UNITY_EDITOR
	private static void UpdateUserStatus(int rank, int record, int volume_sound, int volume_music, bool auth, int language)
	{
		Debug.Log($"UpdateUserStatus: rank: {rank}, record: {record}, volume_sound: {volume_sound}, volume_music: {volume_music}, auth: {auth}, language: {language}");
		return;
	}
#else
	[DllImport("__Internal")]
	private static extern void UpdateUserStatus(int rank, int record, int volume_sound, int volume_music, bool auth, int language);
#endif


#if UNITY_EDITOR
	private static void SendMetrika(string goal)
	{
		Debug.Log($"SendMetrika: {goal}");
		return;
	}
#else
	[DllImport("__Internal")]
	private static extern void SendMetrika(string goal);
#endif
}
