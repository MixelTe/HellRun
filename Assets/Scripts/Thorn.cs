using UnityEngine;

public class Thorn : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public int GrowingState = 0;
    
    private void Start()
    {
        GameManager.GameField.OnLineMoved += ChangeThornsStateOnMovedLine;
        ChangeThornsState(GrowingState);
    }
    
    private void ChangeThornsStateOnMovedLine()
    {
        GrowingState++;
        ChangeThornsState(GrowingState);
    }

    public void ChangeThornsState(int growingState)
    {
        _animator.SetFloat("Speed", GameManager.GameField.ScrollSpeed);
		if (growingState == 0)
            _animator.SetTrigger("GoUp");
        else if (growingState == 1)
            _animator.SetTrigger("SetUp");
        else if (growingState == 2)
            _animator.SetTrigger("GoDown");
        else
            Kill();
    }

    private void Kill()
	{
        GameManager.GameField.OnLineMoved -= ChangeThornsStateOnMovedLine;
        Destroy(gameObject);
    }
}
