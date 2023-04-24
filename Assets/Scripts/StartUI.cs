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
	}

	public void StartGame()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void ShowMain()
	{
		_mainPanel.SetActive(true);
		_leaderboardPanel.SetActive(false);
	}

	public void ShowLeaderboard()
	{
		_mainPanel.SetActive(false);
		_leaderboardPanel.SetActive(true);
		_leaderboard.UpdateData();
	}
}
