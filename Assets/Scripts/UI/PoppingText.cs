using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoppingText : LocalizeTextDynamic
{
	[SerializeField] private AnimationCurve _anim;
	[SerializeField] private float _animTime;
	private float _curAnimTime = 1;

	private void Update()
	{
		if (_curAnimTime >= 1) return;
		_curAnimTime = Mathf.Min(_curAnimTime + Time.deltaTime / _animTime, 1);
		var scale = _anim.Evaluate(_curAnimTime);
		transform.localScale = Vector3.one * scale;
	}

	public void Pop()
	{
		_curAnimTime = 0;
	}
}
