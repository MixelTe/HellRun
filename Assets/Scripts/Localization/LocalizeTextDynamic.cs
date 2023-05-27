using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(TMP_Text))]
public class LocalizeTextDynamic : MonoBehaviour
{
	[HideInInspector] public List<string> Localizations = new();
	[HideInInspector] public string[] ValuesTest = new string[0];
	private object[] _values = new object[0];
	private string _localization = "";

	private void Awake()
	{
		SetLang(Localization.Language);
	}

	public void SetLang(Languages language)
	{
		if (_localization == "" && Localizations.Count > 0)
			_localization = Localizations[0];
		var l = (int)language;
		var i = Localizations.IndexOf(_localization);
		if (i < 0)
			Debug.LogError($"[{gameObject.name}] Do not have localization key: {_localization}");
		else
			SetText(Localizations[i + 1 + l]);
	}

	public void SetText(string key, params object[] values)
	{
		if (Localizations.IndexOf(key) >= 0)
		{
			_localization = key;
			if (values != null)
				_values = values;
			SetLang(Localization.Language);
		}
		else
		{
			Debug.LogError($"[{gameObject.name}] Do not have set localization key: {key}");
		}
	}

	private void SetText(string text)
	{
		var values = Application.isPlaying ? _values : ValuesTest;
		var formated = "";
		try { formated = string.Format(text, values); } catch { }
		GetComponent<TMP_Text>().text = formated;
	}
}
