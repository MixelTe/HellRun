using TMPro;
using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
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
            GameManager.GameUI.UpdateScore(_currentScore);
            yield return new WaitForSeconds(.1f);
        }
    }
}