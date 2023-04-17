using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Ingame")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private TMP_Text _score;

    [Header("Controls")]
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private GameObject _single;
    [SerializeField] private GameObject _double;
    [SerializeField] private float _widthForDouble;

    [Header("Pause")]
    [SerializeField] private GameObject _pausePanel;

    [Header("Game over")]
    [SerializeField] private GameObject _overPanel;
    [SerializeField] private TMP_Text _scoreFinal;

    private void Start()
    {
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(false);
    }

    private void Update()
    {
        if (_canvas.rect.width >= _widthForDouble)
        {
            _double.SetActive(true);
            _single.SetActive(false);
        }
        else
        {
            _double.SetActive(false);
            _single.SetActive(true);
        }
    }

    public void ShowGameOver()
	{
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _overPanel.SetActive(true);
    }

    public void UpdateScore(int score)
	{
        _score.text = "—чет: " + score;
        _scoreFinal.text = score.ToString();
    }

    public void ShowPause()
    {
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void HidePause()
    {
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
    }
}
