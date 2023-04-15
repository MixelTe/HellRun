using System.Collections;
using UnityEngine;

public class Chain : MonoBehaviour
{
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private Transform _chain;
	[SerializeField] private float _strikeTime;
	[SerializeField] private AnimationCurve _strikeScale;
	[SerializeField] private AnimationCurve _strikeAlpha;
	[SerializeField] private float _a;
	[SerializeField] private float _b;

	public void Strike(float duration)
	{
		StartCoroutine(StrikeImp(duration));
	}

	private IEnumerator StrikeImp(float duration)
	{
		_collider.enabled = false;
		yield return Fade(duration);
		
		yield return StrikeAnim(_strikeTime);

		Destroy(gameObject);
	}

	private IEnumerator Fade(float duration)
	{
		var color = _renderer.color;
		var pos = _chain.localPosition;
		for (float t = 0; t < 1; t += Time.fixedDeltaTime / duration)
		{
			color.a = Mathf.Lerp(0, 0.15f, t);
			_renderer.color = color;

			pos.x = Mathf.Sin(t * _a) * _b;
			_chain.localPosition = pos;

			yield return new WaitForFixedUpdate();
		}
		color.a = 0;
		_renderer.color = color;
	}

	private IEnumerator StrikeAnim(float duration)
	{
		var color = _renderer.color;
		var scale = transform.localScale;
		_collider.enabled = true;

		for (float t = 0; t < 1; t += Time.deltaTime / duration)
		{
			color.a = _strikeAlpha.Evaluate(t);
			_renderer.color = color;

			scale.x = _strikeScale.Evaluate(t);
			transform.localScale = scale;

			yield return new WaitForEndOfFrame();
		}
		_collider.enabled = false;
	}
}
