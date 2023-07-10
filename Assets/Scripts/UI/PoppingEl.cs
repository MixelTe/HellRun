using UnityEngine;

public class PoppingEl : MonoBehaviour
{
	[SerializeField] private AnimationCurve _anim;
	[SerializeField] private float _animTime;
	[SerializeField] private bool _popOnEnable;
	private float _curAnimTime = 1;

	private void Update()
	{
		if (_curAnimTime >= 1) return;
		_curAnimTime = Mathf.Min(_curAnimTime + Time.deltaTime / _animTime, 1);
		var scale = _anim.Evaluate(_curAnimTime);
		transform.localScale = Vector3.one * scale;
	}

	private void OnEnable()
	{
		if (_popOnEnable)
			Pop();
	}

	public void Pop()
	{
		_curAnimTime = 0;
	}
}
