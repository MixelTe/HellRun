using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseRiddleSpawner : MonoBehaviour
{
    //[SerializeField] private GameObject _thornPrefab;
    [SerializeField] private Coin _coinPrefab;
	[SerializeField] private Vector2Int _coinCount;

    private int _coinLeft;
    private Coin _curCoin;

	private void Start()
    {
		GameManager.GameField.OnScrollStopped += OnScrollStopped;
    }

	private void OnScrollStopped()
	{
		if (!GameManager.GameIsRunning) return;

        _coinLeft = _coinCount.GetRandom();
        SpawnCoin();
    }

    private void OnCoinCollected()
	{
        _curCoin.OnColected -= OnCoinCollected;
        _curCoin = null;
        _coinLeft -= 1;
        if (_coinLeft > 0)
		{
            SpawnCoin();
		}
		else
		{
            GameManager.GameField.ContinueScrolling();
        }
    }

    private void SpawnCoin()
    {
        var x = Random.Range(0, Settings.Width - 1);
		var y = Random.Range(3, Settings.Height - 2) - GameManager.GameField.ScrolledLines;
		var pos = new Vector2(x, y);

        _curCoin = Instantiate(_coinPrefab, pos, Quaternion.identity, transform);
        _curCoin.OnColected += OnCoinCollected;
    }
}
