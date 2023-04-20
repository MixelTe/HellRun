using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
	public static T GetRandom<T>(this T[] array)
	{ 
		return array[Random.Range(0, array.Length)];
	}
	public static int GetRandom(this Vector2Int vec)
	{
		return Random.Range(vec.x, vec.y);
	}

	public static Rect Inflate(this Rect rect, float v)
	{
		return new Rect(
			rect.x - v,
			rect.y - v,
			rect.width + v * 2,
			rect.height + v * 2
			);
	}

	public static Color ColorFromRGB(int r, int g, int b)
	{
		return new Color(r / 255f, g / 255f, b / 255f);
	}

	public static void DestroyAllChildren(this Transform transform)
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			var child = transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
	}

	public static void ScrollTo(this ScrollRect instance, RectTransform child)
	{
		if (child == null) return;
		Canvas.ForceUpdateCanvases();
		var viewportLocalPosition = instance.viewport.localPosition;
		var childLocalPosition = child.localPosition;
		var result = new Vector2(
			0 - (viewportLocalPosition.x + childLocalPosition.x),
			0 - (viewportLocalPosition.y + childLocalPosition.y)
		);
		instance.content.localPosition = result;
	}
}
