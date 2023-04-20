using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class YaApiTest : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;
	[DllImport("__Internal")]
	private static extern void ShowAdv();
	[DllImport("__Internal")]
	private static extern void ShowRewardAdv();
	[DllImport("__Internal")]
	private static extern bool IsMobile();
	[DllImport("__Internal")]
	private static extern void AuthPlayer();
	[DllImport("__Internal")]
	private static extern void GetLeaderboard();
	[DllImport("__Internal")]
	private static extern void SetScore(int score);
	[DllImport("__Internal")]
	private static extern void GetScore();

	public void Log(IConvertible v)
	{
		_text.text += "\n" + v.ToString();
	}

	public void Adv()
	{
        Debug.Log("Adv");
        ShowAdv();
	}

	public void AdvRew()
	{
		Debug.Log("AdvRew");
		ShowRewardAdv();
	}

	public void CheckMobile()
	{
		Debug.Log("CheckMobile");
		var mobile = IsMobile();
		Log(mobile);
	}

	public void Auth()
	{
		Debug.Log("Auth");
		AuthPlayer();
	}

	public void Leaderboard()
	{
		Debug.Log("Leaderboard");
		GetLeaderboard();
	}

	public void UpdateScore(float score)
	{
		Debug.Log("UpdateScore");
		SetScore(Mathf.FloorToInt(score));
	}

	public void LoadScore()
	{
		Debug.Log("LoadScore");
		GetScore();
	}

	public void SetLeaderboard(string data)
	{
		Log(data);
	}

	public void SetCurScore(int score)
	{
		Log(score);
	}

	public void GetReward()
	{
		Log("Reward");
	}

	public void CancelReward()
	{
		Log("Canceled reward");
	}

	public void OnAuth(int auth)
	{
		Log($"Auth: {auth == 1}");
	}
}
