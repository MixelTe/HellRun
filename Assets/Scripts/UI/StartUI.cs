using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
	[SerializeField] private GameObject _mainPanel;
	[SerializeField] private GameObject _leaderboardPanel;
	[SerializeField] private Leaderboard _leaderboard;

	private void Start()
	{
		ShowMain();
		SetupLang();
	}

	private void SetupLang()
	{
		var lang = PlayerPrefs.GetInt(Settings.PlayerPrefs_Language, -1);
		if (lang < 0)
		{
			Localization.Language = YaApi.Language();
		}
		else
		{
			var langs = Enum.GetValues(typeof(Languages));
			if (lang >= langs.Length)
				Debug.Log($"Wrong language saved: {lang}");
			Localization.Language = (Languages)(lang % langs.Length);
		}
	}

	public void StartGame()
	{
		YaApi.MetrikaGoal(YaApi.MetrikaGoals.Start);
		SceneManager.LoadScene("MainScene");
	}

	public void ShowMain()
	{
		_mainPanel.SetActive(true);
		_leaderboardPanel.SetActive(false);
	}

	public void ShowLeaderboard()
	{
		YaApi.MetrikaGoal(YaApi.MetrikaGoals.Leaderboard);
		_mainPanel.SetActive(false);
		_leaderboardPanel.SetActive(true);
		_leaderboard.UpdateData();
	}

	public void ChangeLang()
	{
		YaApi.MetrikaGoal(YaApi.MetrikaGoals.LanguageChanged);
		Localization.Language = Localization.Language == Languages.ru ? Languages.en : Languages.ru;
		PlayerPrefs.SetInt(Settings.PlayerPrefs_Language, (int)Localization.Language);
	}
}
