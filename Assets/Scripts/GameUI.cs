using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Ingame")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private TMP_Text _score;

    [Header("Game over")]
    [SerializeField] private GameObject _overPanel;
    [SerializeField] private TMP_Text _scoreFinal;

    private void Start()
    {
        _gamePanel.SetActive(true);
        _overPanel.SetActive(false);
    }

    public void ShowGameOver()
	{
        _gamePanel.SetActive(false);
        _overPanel.SetActive(true);
    }

    public void UpdateScore(int score)
	{
        _score.text = "—чет: " + score;
        _scoreFinal.text = score.ToString();
    }
}
