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
        {
            GetComponent<BoxCollider2D>().enabled = false;
            _animator.SetTrigger("GoUp");
        }
        else if (growingState == 1)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            _animator.SetTrigger("SetUp");
        }
        else if (growingState == 2)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            _animator.SetTrigger("GoDown");
        }
        else
        {
            GameManager.GameField.OnLineMoved -= ChangeThornsStateOnMovedLine;
            Destroy(gameObject);
        }
    }
}
