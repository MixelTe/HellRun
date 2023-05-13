using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickButtonOnKey : MonoBehaviour
{
	[SerializeField] private KeyCode[] _keys = new KeyCode[] { KeyCode.Return };
	private Button _button;
	private bool[] _keyDown;

	private void Awake()
	{
		_button = GetComponent<Button>();
		_keyDown = new bool[_keys.Length];
	}

	private void Update()
	{
		for (int i = 0; i < _keys.Length; i++)
		{
			var key = _keys[i];
			if (Input.GetKeyDown(key))
				_keyDown[i] = true;

			if (_keyDown[i] && Input.GetKeyUp(key))
			{
				_keyDown[i] = false;
				_button.onClick.Invoke();
			}
		}
	}
}
