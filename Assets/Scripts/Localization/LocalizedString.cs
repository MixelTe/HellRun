using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalizedString
{
	[SerializeField] private string[] _localizations;
	public string Value
	{
		get
		{
			var i = (int)Localization.Language;
			return _localizations.Length > i ? _localizations[i] : "None";
		}
	}
}

