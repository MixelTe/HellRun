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
	public static void GamePlayed() => IncreaseGamesPlayed();
	public static Task<bool> CanRate(LeaderboardDataRecord currentData) => _inst.CheckCanRate(currentData);
	public static Task<bool> Rate(LeaderboardDataRecord currentData) => _inst.RateGame(currentData);

	private static YaApi _inst;
	[Header("Dev values")]
	[SerializeField] private bool _isMobile;
	[SerializeField] private bool _isAuth;
	[SerializeField] private int _canReview;
	[SerializeField] private bool _reviewSuccess;
	[SerializeField] private bool _ratedGame;
	[SerializeField] private bool _wasFirst;
	[SerializeField] private bool _wasTop;
	[SerializeField] private Languages _language;
	[SerializeField] private int _playerRank = 0;

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
		ProcessPlayerData(leaderboardData.PlayerRecord);
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
		ProcessPlayerData(playerData);
		Debug.Log("PlayerData loaded");
		return playerData;
	}
	private void ProcessPlayerData(LeaderboardDataRecord playerData)
	{
		Debug.Log("ProcessPlayerData");
		playerData.IsPlayer = true;

		if (playerData.HasSavedRecord)
			PlayerPrefs.SetInt(Settings.PlayerPrefs_RecordScore, playerData.Score);
		else
			playerData.Score = PlayerPrefs.GetInt(Settings.PlayerPrefs_RecordScore, 0);

		var gamesPlayedLoaded = playerData.GamesPlayed;
		var savedGamesPlayed = PlayerPrefs.GetInt(Settings.PlayerPrefs_GamesPlayed, 0);
		playerData.GamesPlayed = Mathf.Max(playerData.GamesPlayed, savedGamesPlayed);
		PlayerPrefs.SetInt(Settings.PlayerPrefs_GamesPlayed, playerData.GamesPlayed);
		
		Debug.Log($"PlayerData processed: {JsonUtility.ToJson(playerData)}");

		_ = SetData(playerData, false, gamesPlayedLoaded);
		SetUserStatus(playerData);
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
			PlayerPrefs.GetInt(Settings.PlayerPrefs_RecordScore, 0);
		var score = GameManager.Score.PlayerScore;
		if (score > record)
		{
			Debug.Log("SetRecord: New record");
			MetrikaGoal(MetrikaGoals.NewRecord);

			PlayerPrefs.SetInt(Settings.PlayerPrefs_RecordScore, score);
			PlayerPrefs.Save();
			if (CheckAuth())
			{
				await SetDataQueued(currentData, score);
			}
		}
	}

	private async Task SetData(LeaderboardDataRecord currentData, bool rated, int gamesPlayedLoaded = -1)
	{
		Debug.Log("SetData");
		if (CheckAuth())
		{
			var ratedGame = currentData.RatedGame || rated;
			var wasFirst = currentData.WasFirst || currentData.Rank == 1;
			var wasTop = currentData.WasTop || (currentData.Rank > 0 && currentData.Rank <= 5);
			var hasGear = currentData.HasGear;

			if ((gamesPlayedLoaded >= 0 && gamesPlayedLoaded != currentData.GamesPlayed) ||
				ratedGame != currentData.RatedGame ||
				wasTop != currentData.WasTop ||
				wasFirst != currentData.WasFirst ||
				hasGear != currentData.HasGear)
			{
				currentData.RatedGame = ratedGame;
				currentData.WasFirst = wasFirst;
				currentData.WasTop = wasTop;
				currentData.HasGear = hasGear;
				Debug.Log("SetData: New data");
				await SetDataQueued(currentData);
			}
		}
	}

	private static float _scoreUpdateTime = 0;
	private static float _scoreUpdateI = 0;
	private static float _scoreUpdateIlast = 0;
	private async Task SetDataQueued(LeaderboardDataRecord data, int score = -1)
	{
		var queueI = _scoreUpdateIlast++;
		Debug.Log($"SetDataQueued: Queue: queueI: {queueI} I: {_scoreUpdateI} Ilast: {_scoreUpdateIlast} Time: {_scoreUpdateTime}");

		while (Time.realtimeSinceStartup - _scoreUpdateTime < 1.1f || _scoreUpdateI < queueI)
			await Task.Yield();
		_scoreUpdateTime = Time.realtimeSinceStartup;

		Debug.Log($"SetDataQueued: Start: queueI: {queueI} I: {_scoreUpdateI} Ilast: {_scoreUpdateIlast} Time: {_scoreUpdateTime}");
		_scoreUpdated = -1;
		SetScore(score >= 0 ? score : data.Score, data.GamesPlayed, data.RatedGame, data.WasTop, data.WasFirst, data.HasGear);
		while (_scoreUpdated < 0)
			await Task.Yield();
		_scoreUpdated = -1;
		_scoreUpdateI++;
		Debug.Log($"SetDataQueued: Data set: queueI: {queueI} I: {_scoreUpdateI} Ilast: {_scoreUpdateIlast} Time: {_scoreUpdateTime}");
	}


	private static void IncreaseGamesPlayed()
	{
		var savedGamesPlayed = PlayerPrefs.GetInt(Settings.PlayerPrefs_GamesPlayed, 0);
		savedGamesPlayed++;
		PlayerPrefs.SetInt(Settings.PlayerPrefs_GamesPlayed, savedGamesPlayed);
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
			MetrikaGoals.Badges => "badges",
			MetrikaGoals.Start => "start",
			MetrikaGoals.Paused => "paused",
			MetrikaGoals.ContinueScreen => "continue_screen",
			MetrikaGoals.Adv => "adv",
			MetrikaGoals.AdvError => "adv_error",
			MetrikaGoals.Gameover => "gameover",
			MetrikaGoals.NewRecord => "new_record",
			MetrikaGoals.NewRank => "new_rank",
			MetrikaGoals.AuthTry => "auth_try",
			MetrikaGoals.Auth => "auth",
			MetrikaGoals.RateScreen => "rate_screen",
			MetrikaGoals.Rate => "rate",
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
		Badges,
		Start,
		Paused,
		ContinueScreen,
		Adv,
		AdvError,
		Gameover,
		NewRecord,
		NewRank,
		AuthTry,
		Auth,
		RateScreen,
		Rate,
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
		var auth = CheckAuth();
		var language = (int)Localization.Language;
		var rated_game = currentData.RatedGame;
		var was_top = currentData.WasTop;
		var was_first = currentData.WasFirst;
		var games_played = currentData.GamesPlayed;
		var has_gear = currentData.HasGear;
		UpdateUserStatus(rank, record, volume_sound, volume_music, auth, language, rated_game, was_top, was_first, games_played, has_gear);
	}

	private int _canRate = -1;
	private bool _alreadyRated = false;
	private async Task<bool> CheckCanRate(LeaderboardDataRecord currentData)
	{
		if (currentData.RatedGame)
			return false;

		if (!CheckAuth())
			return false;

		_canRate = -1;
		_alreadyRated = false;
		CanReview();
		while (_canRate < 0)
			await Task.Yield();

		if (_alreadyRated)
			_ = SetData(currentData, true);

		return _canRate > 0;
	}

	private int _rated = -1;
	private async Task<bool> RateGame(LeaderboardDataRecord currentData)
	{
		if (!CheckAuth())
			return false;

		_rated = -1;
		RequestReview();
		while (_rated < 0)
			await Task.Yield();

		var rated = _rated > 0;
		if (rated)
		{
			MetrikaGoal(MetrikaGoals.Rate);
			_ = SetData(currentData, true);
		}

		return rated;
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
				new LeaderboardDataRecord() { ID = "1", Score = 57, Rank = 7, GamesPlayed = 10, Name = "Super Player 3000", Avatar = "https://avatars.yandex.net/get-music-content/5236179/6e5cc28a.p.3404212/100x100", WasFirst = true },
				new LeaderboardDataRecord() { ID = "2", Score = 790, Rank = 1, GamesPlayed = 34, Name = "Vasya", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle", RatedGame = true, WasFirst = true, WasTop = true, HasGear = true },
				new LeaderboardDataRecord() { ID = "3", Score = 650, Rank = 4, GamesPlayed = 679, Name = "������ ����", Avatar = "https://avatars.yandex.net/get-music-content/3927581/777ae0d7.a.13658486-1/100x100", RatedGame = true, WasTop = true },
				new LeaderboardDataRecord() { ID = "4", Score = 495, Rank = 5, GamesPlayed = 69, Name = "Dub Dubom", Avatar = "https://avatars.yandex.net/get-music-content/38044/f88a8857.a.3839675-1/100x100", RatedGame = true, WasFirst = true },
				new LeaderboardDataRecord() { ID = "5", Score = 215, Rank = 6, GamesPlayed = 45, Name = "The ����", Avatar = "https://avatars.yandex.net/get-music-content/32236/c21fa65f.p.59307/100x100", WasFirst = true, HasGear = true },
				new LeaderboardDataRecord() { ID = "6", Score = 104, Rank = 8, GamesPlayed = 270, Name = "���������", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle", WasTop = true },
				new LeaderboardDataRecord() { ID = "7", Score = 10, Rank = 10, GamesPlayed = 2, Name = "������!", Avatar = "https://avatars.mds.yandex.net/get-yapic/0/0-0/islands-middle", HasGear = true },
				new LeaderboardDataRecord() { ID = "8", Score = 780, Rank = 2, GamesPlayed = 57, Name = "", Avatar = "", WasFirst = true, RatedGame = true },
				GetDevPlayerData(),
			},
			PlayerRecord = (_inst != null && _inst._isAuth) ? GetDevPlayerData() : null
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
	private static LeaderboardDataRecord GetDevPlayerData()
	{
		return new LeaderboardDataRecord()
		{
			ID = "",
			IsPlayer = true,
			Avatar = "",
			Name = "",
			Rank = _inst == null ? 0 : _inst._playerRank,
			Score = 0,
			RatedGame = _inst != null && _inst._ratedGame,
			WasFirst = _inst != null && _inst._wasFirst,
			WasTop = _inst != null && _inst._wasTop,
		};
	}
	private static void GetPlayerData()
	{
		var data = GetDevPlayerData();
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
	private static void SetScore(int score, int games_played, bool rated_game, bool was_top, bool was_first, bool has_gear)
	{
		_inst.OnScoreUpdated(1);
	}
#else
	[DllImport("__Internal")]
	private static extern void SetScore(int score, int games_played, bool rated_game, bool was_top, bool was_first, bool has_gear);
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
	private static void UpdateUserStatus(int rank, int record, int volume_sound, int volume_music, bool auth, int language, bool rated_game, bool was_top, bool was_first, int games_played, bool has_gear)
	{
		Debug.Log($"UpdateUserStatus: rank: {rank}, record: {record}, volume_sound: {volume_sound}, volume_music: {volume_music}, auth: {auth}, language: {language}, rated_game: {rated_game}, was_top: {was_top}, was_first: {was_first}, games_played: {games_played}, has_gear: {has_gear}");
		return;
	}
#else
	[DllImport("__Internal")]
	private static extern void UpdateUserStatus(int rank, int record, int volume_sound, int volume_music, bool auth, int language, bool rated_game, bool was_top, bool was_first, int games_played, bool has_gear);
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


#if UNITY_EDITOR
	private static void CanReview()
	{
		_inst.OnCanReview(_inst != null ? _inst._canReview : 0);
	}
#else
	[DllImport("__Internal")]
	private static extern void CanReview();
#endif

	public void OnCanReview(int canReview)
	{
		Debug.Log($"OnCanReview: {canReview}");
		_canRate = Mathf.Max(canReview, 0);
		_alreadyRated = canReview < 0;
	}


#if UNITY_EDITOR
	private static void RequestReview()
	{
		_inst.OnReviewRequested((_inst != null && _inst._reviewSuccess) ? 1 : 0);
	}
#else
	[DllImport("__Internal")]
	private static extern void RequestReview();
#endif

	public void OnReviewRequested(int rated)
	{
		Debug.Log($"OnReviewRequested: {rated}");
		_rated = rated;
	}
}
