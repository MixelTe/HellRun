using UnityEngine;

public static class Utils
{
	public static T GetRandom<T>(this T[] array)
	{ 
		return array[Random.Range(0, array.Length)];
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
}

