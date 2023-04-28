using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Notify : MonoBehaviour
{
    [SerializeField] private float _animTime;
    [SerializeField] private AnimationCurve _xMove;
    [SerializeField] private AnimationCurve _yMove;

    private RectTransform _transform;
    private Coroutine _anim;

	private void Awake()
	{
        gameObject.SetActive(false);
	}

	public void Show(bool hideOnEnd = true)
	{
        if (_anim != null)
            StopCoroutine(_anim);
        UpdatePosition(0);
        gameObject.SetActive(true);
        _anim = StartCoroutine(ShowAnim(hideOnEnd));
    }

    private IEnumerator ShowAnim(bool hideOnEnd)
	{
		for (float t = 0; t < 1; t += Time.unscaledDeltaTime / _animTime)
		{
			UpdatePosition(t);
			yield return new WaitForEndOfFrame();
		}
		UpdatePosition(1);
		_anim = null;
		if (hideOnEnd)
			gameObject.SetActive(false);
    }

	private void UpdatePosition(float t)
	{
		if (_transform == null)
			_transform = GetComponent<RectTransform>();
		var pos = _transform.anchoredPosition;
		pos.x = _xMove.Evaluate(t);
		pos.y = _yMove.Evaluate(t);
		_transform.anchoredPosition = pos;
	}
}
