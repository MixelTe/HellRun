using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _bestHeight;
    [SerializeField] private float _minWidth;
    [Header("Shake(float power, float time, float timeMul, float angle)")]
    [SerializeField] private Vector4 _shakeOnStop;
    [SerializeField] private Vector3 _shakeOnChain;

    private float _shakeT = 0;
    private float _shakeA = 0;
    private float _shakeTMul = 0;
    private float _shakePower;
    private float _shakeTime;

    private void Start()
    {
        GameManager.GameField.OnScrollStopped += ShakeOnStoped;
    }

    private void LateUpdate()
    {
        var position = transform.position;
        position.y = -GameManager.GameField.ScrolledLines - GameManager.GameField.Scroll;

		transform.position = position;
		SetAspect();

        if (_shakeT > 0)
		{
            _shakeT -= Time.deltaTime / _shakeTime;
            _shakeT = Mathf.Max(_shakeT, 0);
            var pos = _camera.transform.localPosition;
            var v = Mathf.Sin(_shakeT * _shakeTMul);
            pos.x = Mathf.Cos(_shakeA) * v * _shakePower * _shakeT;
            pos.y = Mathf.Sin(_shakeA) * v * _shakePower * _shakeT;
            _camera.transform.localPosition = pos;
        }
    }

    public void Shake(float power, float time, float timeMul, float angle)
    {
        _shakePower = _shakePower * _shakeT + power;
        _shakeTime = Mathf.Max(_shakeTime * _shakeT, time);
        _shakeTMul = Mathf.Lerp(timeMul, _shakeTMul, _shakeT);
        _shakeA = Mathf.Lerp(angle * Mathf.Deg2Rad, _shakeA, _shakeT);
        _shakeT = 1;
    }

    public void ShakeChain()
    {
        Shake(_shakeOnChain.x, _shakeOnChain.y, _shakeOnChain.z, Random.Range(0, 360));
    }

    private void ShakeOnStoped()
	{
        Shake(_shakeOnStop.x, _shakeOnStop.y, _shakeOnStop.z, _shakeOnStop.w);
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
