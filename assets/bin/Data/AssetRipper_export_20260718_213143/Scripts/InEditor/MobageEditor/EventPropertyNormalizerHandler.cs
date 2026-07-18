using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MobageEditor
{
	public class EventPropertyNormalizerHandler : IJSONDictionaryHandler
	{
		private List<string> propertiesToNormalize = new List<string> { "afam", "asku", "srcty", "evcl", "evid", "pltfmsku", "carr", "srv" };

		public void Process(JsonData json)
		{
			foreach (string item in propertiesToNormalize)
			{
				try
				{
					if (json[item].IsString)
					{
						json[item] = normalize(json[item].ToString());
					}
				}
				catch (KeyNotFoundException)
				{
				}
			}
		}

		private string normalize(string value)
		{
			return Regex.Replace(value, "[\\W_]", string.Empty);
		}
	}
}
