using System.Collections.Generic;
using System.IO;

public class WindowsAccessor
{
	public const string KEY_VALUE_DB = "keyValue.json";

	public static void Connect(out Dictionary<string, string> result)
	{
		result = new Dictionary<string, string>();
		string path = Path.Combine(Singleton<InitializationManager>.instance.persistentDataPath, "keyValue.json");
		using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split('|');
					result.Add(array[0], array[1]);
				}
			}
		}
	}

	public static void Save(Dictionary<string, string> db)
	{
		string path = Path.Combine(Singleton<InitializationManager>.instance.persistentDataPath, "keyValue.json");
		using (FileStream stream = File.Open(path, FileMode.OpenOrCreate))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				foreach (KeyValuePair<string, string> item in db)
				{
					streamWriter.WriteLine(item.Key + "|" + item.Value);
				}
			}
		}
	}
}
