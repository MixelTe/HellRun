using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Animator _shakeAnimator;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _bestHeight;
    [SerializeField] private float _minWidth;

    private void Start()
    {
        GameManager.GameField.OnScrollStopped += Shake;
    }

    private void LateUpdate()
    {
        var position = transform.position;
		position.y = -GameManager.GameField.ScrolledLines - GameManager.GameField.Scroll;

		transform.position = position;
		SetAspect();
    }

    private void Shake()
    {
        _shakeAnimator.SetTrigger("ShakeTriger");
    }

    private void SetAspect()
	{
        var h = _bestHeight;
        var w = _camera.aspect * h; 
        if (w < _minWidth)
            h = _minWidth / _camera.aspect;
        _camera.orthographicSize = h;
    }
}
