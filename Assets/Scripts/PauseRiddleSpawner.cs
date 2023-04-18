using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseRiddleSpawner : MonoBehaviour
{
    [SerializeField] private Animator _thornPrefab;
	[SerializeField] private Coin _coinPrefab;
	[SerializeField] private Vector2Int _coinCount;
    [SerializeField] private float _strikeStartDelay;
    [SerializeField] private float _strikeTime;
    [SerializeField] private float _strikeThornMul;
    [SerializeField] private ThornGroup[] _thornGroups;

    private float CurStrikeTime { get => _strikeTime / GameManager.GameField.ScrollSpeed; }
    private int _coinLeft;
    private Coin _curCoin;
    private Coroutine _thornSpawner;

    private void Start()
    {
		GameManager.GameField.OnScrollStopped += OnScrollStopped;
    }

	private void OnScrollStopped()
	{
		if (!GameManager.GameIsRunning) return;

        _thornSpawner = StartCoroutine(SpawnThorns());

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
            if (_thornSpawner != null)
                StopCoroutine(_thornSpawner);
            _thornSpawner = null;
            transform.DestroyAllChildren();
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

    private IEnumerator SpawnThorns()
    {
        yield return new WaitForSeconds(_strikeStartDelay);
        while (true)
        {
            if (!GameManager.GameIsRunning) break;

            var group = _thornGroups.GetRandom();
            foreach (var strike in group.Thorns)
            {
                if (!GameManager.GameIsRunning) break;

                SpawnStrike(strike);

                yield return new WaitForSeconds(CurStrikeTime);
            }
            yield return new WaitForSeconds(CurStrikeTime);
        }
    }

	private void SpawnStrike(ThornStrike strike)
	{
        for (int y = 0; y < strike.Height; y++)
            for (int x = 0; x < strike.Width; x++)
                if (strike[x, y])
                    SpawnThorn(x, y);
    }

    private void SpawnThorn(int x, int y)
    {
        var positon = new Vector3(x, Settings.Height - y - 2 - GameManager.GameField.ScrolledLines);
        var chain = Instantiate(_thornPrefab, positon, Quaternion.identity, transform);
        chain.SetFloat("Speed", 2.1f / (CurStrikeTime * _strikeThornMul));
    }
}
