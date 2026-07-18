using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager>
{
	public enum Language
	{
		English = 0,
		French = 1,
		Italian = 2,
		German = 3,
		Spanish = 4,
		Portuguese = 5,
		Japanese = 6,
		Chinese = 7,
		Turkish = 8,
		Russian = 9,
		Korean = 10,
		ChineseTraditional = 11,
		Test_Longest = 12,
		None = 13,
		COUNT = 13
	}

	public enum LanguageEncoding
	{
		ASCII = 0,
		Unicode = 1
	}

	private const Language DEFAULT_LANGUAGE = Language.English;

	private Language _currentLanguage = Language.None;

	public TextAsset[] languages;

	private Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	private Dictionary<string, string> mDefaultDictionary = new Dictionary<string, string>();

	private string mLanguage;

	private bool initialized;

	public string currentLanguage
	{
		get
		{
			return _currentLanguage.ToString();
		}
		set
		{
		}
	}

	public Language currentLanguageFromEnum
	{
		get
		{
			return _currentLanguage;
		}
		set
		{
		}
	}

	public string currentLanguageCode
	{
		get
		{
			return LanguageToLanguageCode(_currentLanguage);
		}
		set
		{
		}
	}

	public event Action OnInitializeListener;

	public IEnumerator Init()
	{
		Singleton<InitializationManager>.instance.DataModelUpdated += DataModelUpdated;
		List<LocalizationItemDataModel> dataModels = new List<LocalizationItemDataModel>();
		bool ready = false;
		AccessBase.Error e = NonUnitySingleton<DMAccessManager>.instance.DataModelAccess.GetAllLocalizationItems(out dataModels);
		if (e != null)
		{
			Log.Error(string.Concat("Localization.DataModelUpdated. Error reading database: ", e, " - ", e.description));
		}
		ready = true;
		while (!ready)
		{
			yield return 0;
		}
		LoadDefaultLanguage(dataModels);
		Log.Info("Detected the following language: " + Application.systemLanguage);
		Language deviceLanguage = SystemLanguageToLanguage(Application.systemLanguage);
		Log.Info("Chose the following language: " + deviceLanguage);
		SetCurrentLanguage(deviceLanguage, dataModels);
		initialized = true;
		if (this.OnInitializeListener != null)
		{
			this.OnInitializeListener();
		}
		yield return 0;
	}

	private void LoadDefaultLanguage(List<LocalizationItemDataModel> localizationItems)
	{
		for (int i = 0; i < localizationItems.Count; i++)
		{
			LocalizationItemDataModel localizationItemDataModel = localizationItems[i];
			string text = localizationItemDataModel.strings[0];
			text = text.Replace("\\n", "\n");
			mDefaultDictionary[localizationItemDataModel.id] = text;
		}
		mDictionary = mDefaultDictionary;
	}

	private void DataModelUpdated()
	{
		Log.Debug("Localization.DataModelUpdated()");
		List<LocalizationItemDataModel> localizationItems;
		AccessBase.Error allLocalizationItems = NonUnitySingleton<DMAccessManager>.instance.DataModelAccess.GetAllLocalizationItems(out localizationItems);
		if (allLocalizationItems != null)
		{
			Log.Error("Localization.DataModelUpdated...error reading database " + allLocalizationItems);
		}
		mDefaultDictionary.Clear();
		mDictionary.Clear();
		LoadDefaultLanguage(localizationItems);
		SetCurrentLanguage(_currentLanguage, localizationItems);
	}

	private void SetCurrentLanguage(Language lang, List<LocalizationItemDataModel> localizationItems)
	{
		_currentLanguage = lang;
		if (lang == Language.English)
		{
			mDictionary = mDefaultDictionary;
			return;
		}
		mDictionary.Clear();
		for (int i = 0; i < localizationItems.Count; i++)
		{
			LocalizationItemDataModel localizationItemDataModel = localizationItems[i];
			string text = localizationItemDataModel.strings[(int)lang];
			text = text.Replace("\\n", "\n");
			if (mDictionary.ContainsKey(localizationItemDataModel.id))
			{
				Log.Warning("LocalizationManager.SetCurrentLanguage: key '{0}' is present more than once in the metadata", localizationItemDataModel.id);
			}
			mDictionary[localizationItemDataModel.id] = text;
		}
	}

	public bool Contains(string key)
	{
		return mDictionary.ContainsKey(key) || mDefaultDictionary.ContainsKey(key);
	}

	public string Get(string key)
	{
		string value;
		if (mDictionary.TryGetValue(key, out value))
		{
			return value;
		}
		if (_currentLanguage != Language.English && mDefaultDictionary.TryGetValue(key, out value))
		{
			return value;
		}
		return key;
	}

	public static string GetString(string key, string defaultString = null)
	{
		if (Singleton<LocalizationManager>.instance.Contains(key))
		{
			return Singleton<LocalizationManager>.instance.Get(key);
		}
		return (defaultString != null) ? defaultString : key;
	}

	public static Language SystemLanguageToLanguage(SystemLanguage sysLanguage)
	{
		switch (sysLanguage)
		{
		case SystemLanguage.English:
			return Language.English;
		case SystemLanguage.French:
			return Language.French;
		case SystemLanguage.Italian:
			return Language.Italian;
		case SystemLanguage.German:
			return Language.German;
		case SystemLanguage.Spanish:
			return Language.Spanish;
		case SystemLanguage.Portuguese:
			return Language.Portuguese;
		case SystemLanguage.Japanese:
			return Language.English;
		case SystemLanguage.Chinese:
			return DetermineLocalChinese();
		case SystemLanguage.Turkish:
			return Language.Turkish;
		case SystemLanguage.Russian:
			return Language.Russian;
		case SystemLanguage.Korean:
			return Language.Korean;
		default:
			return Language.English;
		}
	}

	public static string LanguageToLanguageCode(Language language)
	{
		switch (language)
		{
		case Language.English:
			return "en";
		case Language.French:
			return "fr";
		case Language.Italian:
			return "it";
		case Language.German:
			return "de";
		case Language.Spanish:
			return "es";
		case Language.Portuguese:
			return "pt";
		case Language.Japanese:
			return "en";
		case Language.Chinese:
			return "sch";
		case Language.Turkish:
			return "tr";
		case Language.Russian:
			return "ru";
		case Language.Korean:
			return "ko";
		case Language.ChineseTraditional:
			return "tch";
		case Language.Test_Longest:
			return "test_longest";
		default:
			return "en";
		}
	}

	public static LanguageEncoding LanguageEncodingType()
	{
		switch (Singleton<LocalizationManager>.instance._currentLanguage)
		{
		case Language.Japanese:
		case Language.Korean:
			return LanguageEncoding.Unicode;
		case Language.Chinese:
			return LanguageEncoding.Unicode;
		case Language.ChineseTraditional:
			return LanguageEncoding.Unicode;
		default:
			return LanguageEncoding.ASCII;
		}
	}

	public bool IsInitialized()
	{
		return initialized;
	}

	public static Language DetermineLocalChinese()
	{
		Log.Info("Only detection for Simplified Chinese available on Android");
		return Language.Chinese;
	}
}
