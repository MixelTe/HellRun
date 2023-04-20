using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int PlayerScore { get => _currentScore; }
    private int _currentScore = 0;

    private void Start()
    {
        StartCoroutine(UpdateScoreEverySecond());
    }

    private IEnumerator UpdateScoreEverySecond()
    {
        while (true)
        {
            if(!GameManager.GameIsRunning)
                break;
            if (GameManager.GameField.Scroling)
            {
                _currentScore++;
                GameManager.GameUI.UpdateScore(_currentScore);
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public void AddCoin()
    {
        _currentScore += 25;
        GameManager.GameUI.UpdateScore(_currentScore, true);
    }
}