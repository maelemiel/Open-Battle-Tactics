using System.Collections.Generic;
using System.Linq;

namespace MobageEditor
{
	public static class MyExtensions
	{
		public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			try
			{
				return "{" + string.Join(",", dictionary.Select((KeyValuePair<TKey, TValue> kv) => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray()) + "}";
			}
			catch
			{
				return "error";
			}
		}

		public static int IndexOf(this JsonData data, string key)
		{
			for (int i = 0; i < data.Count; i++)
			{
				if (key == (string)data[i])
				{
					return i;
				}
			}
			return data.Count;
		}
	}
}
