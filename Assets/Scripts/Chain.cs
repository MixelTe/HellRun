using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private float _strikeTime;

	public void Strike(float duration)
	{
		StartCoroutine(StrikeImp(duration));
	}

	private IEnumerator StrikeImp(float duration)
	{
		_collider.enabled = false;
		yield return Fade(0.5f, duration);

		var color = _renderer.color;
		color.a = 1;
		_renderer.color = color;
		_collider.enabled = true;

		yield return new WaitForSeconds(_strikeTime);

		_collider.enabled = false;

		yield return Fade(1f, duration - _strikeTime);

		Destroy(gameObject);
	}

	private IEnumerator Fade(float startA, float duration)
	{
		var color = _renderer.color;
		for (float t = 0; t < 1; t += Time.fixedDeltaTime / duration)
		{
			color.a = Mathf.Lerp(startA, 0, t);
			_renderer.color = color;
			yield return new WaitForFixedUpdate();
		}
		color.a = 0;
		_renderer.color = color;
	}
}
