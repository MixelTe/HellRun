using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizeText : MonoBehaviour
{
	public string[] Localizations = new string[0];

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
		var text = GetComponent<TMP_Text>();
		text.text = Localizations[i];
	}
}
