using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
	private static Languages _language = Languages.ru;
	public static Languages Language
	{
		get => _language; 
		set
		{
			_language = value;
			UpdateLang();
		}
	}
	private static void UpdateLang()
	{
		var texts = GameObject.FindObjectsOfType<LocalizeText>(true);
		for (int i = 0; i < texts.Length; i++)
			texts[i].SetLang(_language);

		var images = GameObject.FindObjectsOfType<LocalizeImage>(true);
		for (int i = 0; i < images.Length; i++)
			images[i].SetLang(_language);

		var dynamicTexts = GameObject.FindObjectsOfType<LocalizeTextDynamic>(true);
		for (int i = 0; i < dynamicTexts.Length; i++)
			dynamicTexts[i].SetLang(_language);
	}
}

public enum Languages
{
	ru = 0,
	en = 1,
}
