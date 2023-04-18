using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PoppingText : MonoBehaviour
{
	[SerializeField] private AnimationCurve _anim;
	[SerializeField] private float _animTime;
	private TMP_Text _text;
	private string _textStr;
	private float _curAnimTime = 1;

	private void Awake()
	{
		_text = GetComponent<TMP_Text>();
		_text.text = _textStr;
	}

	private void Update()
	{
		if (_curAnimTime == 1) return;
		_curAnimTime = Mathf.Min(_curAnimTime + Time.deltaTime / _animTime, 1);
		var scale = _anim.Evaluate(_curAnimTime);
		transform.localScale = Vector3.one * scale;
	}

	public void SetText(string value)
	{
		if (_text != null)
			_text.text = value;
		else
			_textStr = value;
	}

	public void Pop()
	{
		_curAnimTime = 0;
	}
}
