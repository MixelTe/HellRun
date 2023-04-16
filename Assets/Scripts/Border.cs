using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private GameObject _borderLeft;
    [SerializeField] private GameObject _borderRight;
    private int _linesBeforeSpawn = 0;
    private readonly GameObject[] _rightBorders = new GameObject[2];
    private readonly GameObject[] _leftBorders = new GameObject[2];
    private int _index = 0;


    private void Start()
    {
        transform.DestroyAllChildren();
		GameManager.GameField.OnLineMoved += OnLineMoved;
        SpawnBorder(Vector3.zero);
        OnLineMoved();
    }

	private void OnLineMoved()
	{
        _linesBeforeSpawn--;
        if (_linesBeforeSpawn < 0)
		{
            _linesBeforeSpawn = Settings.Height - 1;
            var pos = new Vector3(0, -GameManager.GameField.ScrolledLines - Settings.Height + 1);
            SpawnBorder(pos);
        }
	}

    private void SpawnBorder(Vector3 pos)
    {
        var left = Instantiate(_borderLeft, pos, Quaternion.identity, transform);
        var right = Instantiate(_borderRight, pos, Quaternion.identity, transform);

        _index = 1 - _index;
        Destroy(_leftBorders[_index]);
        Destroy(_rightBorders[_index]);
        _leftBorders[_index] = left;
        _rightBorders[_index] = right;
    }
}
