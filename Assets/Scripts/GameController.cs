using UnityEngine;

public class GameController : MonoBehaviour
{
    //The time for which the game will accelerate by 2 times
    [SerializeField] private float _scrollSpeedAcceleration  = 120f;
    [SerializeField] private float _timeToStop = 120f;
    private float _timeLeftForStop = 0f;

    private void Start()
    {
        _timeLeftForStop = _timeToStop;
        //ChainSpawner.OnChainGroupEnded += StartNewChainGroup;
    }

    private void Update()
    {
        if (GameManager.GameIsRunning && GameManager.GameField.Scroling)
        {
            _timeLeftForStop -= Time.deltaTime;
            GameManager.GameField.ScrollSpeed += Time.deltaTime / _scrollSpeedAcceleration;
            if (_timeLeftForStop < 0)
            {
                GameManager.GameField.StopScrolling();
                _timeLeftForStop = _timeToStop;
            }
        }
    }

    private void StartNewChainGroup()
    {
        //ChainSpawner.StartChainGroup();
    }
}