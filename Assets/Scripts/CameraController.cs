using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Animator _shakeAnimator;
    
    private void Start()
    {
        GameManager.GameField.ScrollStopped += Shake;
    }

    private void LateUpdate()
    {
        var position = transform.position;
        position.y =  -GameManager.GameField.ScrolledLines - GameManager.GameField.Scroll;
        
        transform.position = position;
    }

    private void Shake()
    {
        _shakeAnimator.SetTrigger("ShakeTriger");
    }
}
