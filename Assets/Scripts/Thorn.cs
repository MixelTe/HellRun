using UnityEngine;

public class Thorn : MonoBehaviour, IPollable
{
    [SerializeField] private Animator _animator;
    private int _growingState = 0;
    private bool _justCreated = true;

    public bool IsDestroyedToPool { get; set; }

	private void Start()
    {
        GameManager.GameField.OnLineMoved += ChangeThornsStateOnMovedLine;
    }

    void IPollable.InitAsNew()
    {
        _growingState = 0;
        _justCreated = true;
    }
	private void Update()
	{
        _justCreated = false;
        if (GameManager.GameField.Scroling)
            _animator.SetFloat("Speed", GameManager.GameField.ScrollSpeed);
        else 
            _animator.SetFloat("Speed", 0);
    }

    private void ChangeThornsStateOnMovedLine()
    {
        if (IsDestroyedToPool || _justCreated) return;
        _growingState++;
        ChangeThornsState(_growingState);
    }

    public void ChangeThornsState(int growingState)
    {
        _growingState = growingState;
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
