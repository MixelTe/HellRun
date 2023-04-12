using TMPro;
using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
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
                yield break;
            _currentScore++;
            _scoreText.text = "Счет: " + _currentScore;
            yield return new WaitForSeconds(.1f);
        }
    }
}