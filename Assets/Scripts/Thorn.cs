using UnityEngine;

public class Thorn : MonoBehaviour, IPollable
{
    [SerializeField] private Animator _animator;
    private int _growingState = 0;

    public bool IsDestroyedToPool { get; set; }

	private void Start()
    {
        GameManager.GameField.OnLineMoved += ChangeThornsStateOnMovedLine;
    }

    void IPollable.InitAsNew()
    {
        _growingState = 0;
    }

    private void ChangeThornsStateOnMovedLine()
    {
        if (IsDestroyedToPool) return;
        _growingState++;
        ChangeThornsState(_growingState);
    }

    public void ChangeThornsState(int growingState)
    {
        _growingState = growingState;
        _animator.SetFloat("Speed", GameManager.GameField.ScrollSpeed);
		if (_growingState == 0)
            _animator.SetTrigger("GoUp");
        else if (_growingState == 1)
            _animator.SetTrigger("SetUp");
        else if (_growingState == 2)
            _animator.SetTrigger("GoDown");
		else
		{
            _animator.SetTrigger("Reset");
            this.DestroyToPool();
        }
    }
}
