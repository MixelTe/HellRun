using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{
	[SerializeField] Gradient _color;
	[SerializeField] SpriteRenderer[] _cracks;

	public void SetRandom()
	{
		foreach (var crack in _cracks)
			crack.gameObject.SetActive(false);
		var active = _cracks.GetRandom();
		active.gameObject.SetActive(true);

		var rotation = Random.Range(0, 4) * 90;
		transform.rotation = Quaternion.Euler(0, 0, rotation);

		active.color = _color.Evaluate(Random.value);
	}
}
