using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectButtonOnEnable : MonoBehaviour
{
	private Button _button;

	private void Awake()
	{
		_button = GetComponent<Button>();
	}

	private void Start()
	{
		_button.Select();
	}

	private void OnEnable()
	{
		_button.Select();
	}
}
