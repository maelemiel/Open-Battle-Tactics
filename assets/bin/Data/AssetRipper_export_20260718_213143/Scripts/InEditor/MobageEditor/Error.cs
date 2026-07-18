using System.Collections.Generic;

namespace MobageEditor
{
	public class Error : ISerializableItem
	{
		public const string ErrorDomain = "com.mobage.error.api";

		public string domain;

		public int code;

		public string localizedDescription;

		public Dictionary<string, object> PackForEnvironment(ModelSerializationEnvironment env)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("domain", "com.mobage.error.api");
			dictionary.Add("code", code);
			dictionary.Add("localizedDescription", localizedDescription);
			return dictionary;
		}
	}
}
