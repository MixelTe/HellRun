using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private SpriteRenderer _renderer; // Temp

	public void Strike(float duration)
	{
		StartCoroutine(StrikeImp(duration));
	}

	private IEnumerator StrikeImp(float duration)
	{
		// Temp
		var color = _renderer.color;
		color.a = 0.5f;
		_renderer.color = color;
		// Temp

		_collider.enabled = false;
		yield return new WaitForSeconds(duration);

		// Temp
		color.a = 1f;
		_renderer.color = color;
		// Temp

		_collider.enabled = true;
		yield return new WaitForSeconds(duration);
		Destroy(gameObject);
	}
}
