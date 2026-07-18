using MiniJSON;

namespace LitJson0
{
	public class JsonMapper
	{
		public static string ToJson(object val)
		{
			return Json.Serialize(val);
		}

		public static JsonObject ToObject(string jsonText)
		{
			return new JsonObject(jsonText);
		}

		public static T ToObject<T>(string jsonText)
		{
			return default(T);
		}
	}
}
