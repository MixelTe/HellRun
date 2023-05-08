using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocalizeImage : MonoBehaviour
{
	public Sprite[] Localizations = new Sprite[0];

	private void Awake()
	{
		SetLang(Localization.Language);
	}

	public void SetLang(Languages language)
	{
		var i = (int)language;
		if (Localizations.Length <= i)
		{
			Debug.LogError($"[{gameObject.name}] Do not have localization: {Localization.Language}");
			return;
		}
		var image = GetComponent<Image>();
		image.sprite = Localizations[i];
	}
}
