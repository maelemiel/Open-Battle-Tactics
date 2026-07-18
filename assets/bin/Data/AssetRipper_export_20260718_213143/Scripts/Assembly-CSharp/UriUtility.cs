using System;
using System.Text;

public static class UriUtility
{
	public static string QueryString(params string[] uriParams)
	{
		if (uriParams.Length % 2 != 0)
		{
			throw new Exception("uriParams Length is not divisble by 2. It should have a sequence of key value pairs.");
		}
		if (uriParams.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(16 * uriParams.Length);
		int num;
		for (num = 0; num < uriParams.Length; num++)
		{
			stringBuilder.Append(uriParams[num]);
			stringBuilder.Append("=");
			if (!string.IsNullOrEmpty(uriParams[++num]))
			{
				stringBuilder.Append(uriParams[num]);
			}
			stringBuilder.Append("&");
		}
		return stringBuilder.ToString(0, stringBuilder.Length - 1);
	}

	public static string QueryStringRepeat(string keyName, params string[] uriValues)
	{
		if (uriValues.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(16 * (uriValues.Length + 1));
		for (int i = 0; i < uriValues.Length; i++)
		{
			stringBuilder.Append(Uri.EscapeUriString(keyName));
			stringBuilder.Append("=");
			if (!string.IsNullOrEmpty(uriValues[i]))
			{
				stringBuilder.Append(Uri.EscapeUriString(uriValues[i]));
			}
			stringBuilder.Append("&");
		}
		return stringBuilder.ToString(0, stringBuilder.Length - 1);
	}

	public static string QueryJsonString(string[] uriValues)
	{
		object[] uriValues2 = new object[1] { uriValues };
		return QueryJsonString(uriValues2);
	}

	public static string QueryJsonString(params object[] uriValues)
	{
		if (uriValues.Length == 0)
		{
			return string.Empty;
		}
		string text = "{\"args\":[";
		string text2 = string.Empty;
		for (int i = 0; i < uriValues.Length; i++)
		{
			text2 = text2 + ParseParameter(uriValues[i]) + ",";
		}
		if (text2.IndexOf(",") != -1)
		{
			text2 = text2.Substring(0, text2.Length - 1);
		}
		return text + text2 + "]}";
	}

	public static string ParseParameter(object item)
	{
		string empty = string.Empty;
		if (item == null)
		{
			return string.Empty;
		}
		if (item.GetType() == typeof(string))
		{
			return "\"" + Uri.EscapeDataString(item.ToString()) + "\"";
		}
		if (item.GetType().IsArray)
		{
			empty = "[";
			Array array = (Array)item;
			foreach (object item2 in array)
			{
				empty = empty + ParseParameter(item2) + ",";
			}
			if (empty.IndexOf(",") != -1)
			{
				empty = empty.Substring(0, empty.Length - 1);
			}
			return empty + "]";
		}
		if (item.GetType() == typeof(bool))
		{
			if ((bool)item)
			{
				return "\"true\"";
			}
			return "\"false\"";
		}
		return item.ToString();
	}
}
