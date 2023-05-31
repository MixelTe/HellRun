using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomChildOnEnable : MonoBehaviour
{
	[SerializeField] private Button _button;
	[SerializeField] private GameObject _container;

	private void OnEnable()
	{
		for (int i = 0; i < _container.transform.childCount; i++)
			_container.transform.GetChild(i).gameObject.SetActive(false);

		var children = _container.transform.GetComponentsInChildren<Image>(true);
		var child = children.GetRandom();
		child.gameObject.SetActive(true);

		_button.targetGraphic = child;
	}
}
