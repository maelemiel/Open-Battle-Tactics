using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;

namespace LitJson0
{
	public class JsonObject
	{
		private Dictionary<string, object> _jsonDictionary;

		public object this[string index]
		{
			get
			{
				object result = null;
				if (_jsonDictionary != null)
				{
					if (_jsonDictionary.ContainsKey(index))
					{
						result = _jsonDictionary[index];
					}
					else
					{
						Console.WriteLine("JsonData dictionary does not contain index: [" + index + "]");
					}
				}
				else
				{
					Console.WriteLine("In JsonData _jsonDictionary is null, returning null for index.");
				}
				return result;
			}
			set
			{
				if (value is JsonObject)
				{
					_jsonDictionary[index] = (value as JsonObject)._jsonDictionary;
				}
				else
				{
					_jsonDictionary[index] = value;
				}
			}
		}

		public Dictionary<string, object> Dictionary
		{
			get
			{
				return _jsonDictionary;
			}
		}

		public JsonObject()
		{
			_jsonDictionary = new Dictionary<string, object>();
		}

		public JsonObject(string jsontext)
		{
			_jsonDictionary = (Dictionary<string, object>)Json.Deserialize(jsontext);
		}

		private JsonObject(Dictionary<string, object> jsondata)
		{
			_jsonDictionary = jsondata;
		}

		public string ToJson()
		{
			return Json.Serialize(_jsonDictionary);
		}

		public bool Contains(string index)
		{
			return _jsonDictionary.ContainsKey(index);
		}

		public string GetString(string index)
		{
			return (string)this[index];
		}

		public void SetString(string index, string value)
		{
			this[index] = value;
		}

		public int GetInt(string index)
		{
			long num = (long)this[index];
			if (int.MinValue > num || int.MaxValue < num)
			{
				Log.Error("Possible loss of precision. Integer read from json object is not an int.");
			}
			return (int)num;
		}

		public int GetIntOrDefault(string name, int defaultValue)
		{
			return (!Contains(name)) ? defaultValue : GetInt(name);
		}

		public void SetInt(string index, int value)
		{
			this[index] = (long)value;
		}

		public long GetLong(string index)
		{
			long result = 0L;
			object obj = this[index];
			if (obj != null)
			{
				result = (long)obj;
			}
			return result;
		}

		public long GetLongOrDefault(string name, long defaultValue)
		{
			return (!Contains(name)) ? defaultValue : GetLong(name);
		}

		public void SetLong(string index, long value)
		{
			this[index] = value;
		}

		public float GetFloat(string index)
		{
			return Convert.ToSingle(this[index]);
		}

		public void SetFloat(string index, float value)
		{
			this[index] = value;
		}

		public bool GetBooleanOrDefault(string index, bool defaultValue)
		{
			return (!Contains(index)) ? defaultValue : GetBoolean(index);
		}

		public bool GetBoolean(string index)
		{
			return (bool)this[index];
		}

		public void SetBoolean(string index, bool value)
		{
			this[index] = value;
		}

		public JsonObject GetObject(string index)
		{
			JsonObject result = null;
			if (index != null && _jsonDictionary.ContainsKey(index))
			{
				result = new JsonObject((Dictionary<string, object>)this[index]);
			}
			return result;
		}

		public List<JsonObject> GetObjectList(string index)
		{
			if (!Contains(index))
			{
				return null;
			}
			IList list = GetList(index);
			List<JsonObject> list2 = new List<JsonObject>();
			foreach (object item in list)
			{
				if (item == null)
				{
					list2.Add(null);
				}
				else
				{
					list2.Add(new JsonObject((Dictionary<string, object>)item));
				}
			}
			return list2;
		}

		public void SetObjectList(string propertyName, List<JsonObject> list)
		{
			this[propertyName] = list.Select((JsonObject x) => x._jsonDictionary).ToList();
		}

		public IList GetList(string index)
		{
			return (IList)this[index];
		}

		public void SetList(string index, IList values)
		{
			this[index] = values;
		}

		public void SetList(string index, object[] values)
		{
			this[index] = values.ToList();
		}
	}
}
