using System;

public static class ParseUtility
{
	public static Enum ParseEnum(Type enumType, string txt)
	{
		try
		{
			Enum obj = (Enum)Enum.Parse(enumType, txt, true);
			if (Enum.IsDefined(enumType, obj))
			{
				return obj;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public static T ParseEnum<T>(string key, T defaultValue) where T : struct, IConvertible
	{
		if (!typeof(T).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type");
		}
		if (key == null || key == string.Empty)
		{
			return defaultValue;
		}
		Type typeFromHandle = typeof(T);
		string text = null;
		T result;
		try
		{
			T val = (T)Enum.Parse(typeFromHandle, key, true);
			if (Enum.IsDefined(typeFromHandle, val))
			{
				result = val;
			}
			else
			{
				result = defaultValue;
				text = "ParseUtility.ParseEnum...unknowkn key: " + key + "\nEnum type: " + typeof(T).ToString();
			}
		}
		catch (Exception ex)
		{
			text = "ParseUtility.ParseEnum..." + ex.ToString() + "\nEnum type: " + typeof(T).ToString();
			result = defaultValue;
		}
		if (text != null)
		{
			Log.Error(text);
		}
		return result;
	}
}
