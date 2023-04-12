using TMPro;
using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private int _currentScore = 0;

    private void Start()
    {
        //GameField.OnLineMoved += UpdateScoreOnLineMoved;
        StartCoroutine(UpdateScoreEverySecond());
    }

    private void UpdateScoreOnLineMoved()
    {
        _currentScore++;
        _scoreText.text = "Счет: " + _currentScore * 10;
        
    }

    private IEnumerator UpdateScoreEverySecond()
    {
        while (true)
        {
            if(!GameManager.GameIsRunning)
                yield break;
            _currentScore++;
            _scoreText.text = "Счет: " + _currentScore * 10;
            yield return new WaitForSeconds(1);
        }
    }
}