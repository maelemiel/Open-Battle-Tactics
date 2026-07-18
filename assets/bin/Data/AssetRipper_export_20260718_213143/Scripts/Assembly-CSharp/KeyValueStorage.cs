using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class KeyValueStorage
{
	public enum Storage
	{
		LAST_LOGIN = 0,
		ASSET_BUNDLE = 1,
		USER_PROFILE = 2,
		PREFERENCES = 3,
		TUTORIAL = 4,
		FACEBOOK_DATA = 5,
		DATA_MODEL_FILE = 6,
		ENVIRONMENT = 7,
		CLIENT = 8,
		LAST_BATTLE_TYPE = 9,
		DIALOG_TRIGGER_VISITS = 10,
		ASK_FOR_RATING = 11
	}

	private class Value<T>
	{
		private T _value;

		private string _data;

		private Value()
		{
		}

		private Value(string data)
		{
			_data = data;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				int result;
				if (int.TryParse(data, out result))
				{
					_value = (T)(object)result;
				}
				else
				{
					_value = (T)(object)0;
				}
			}
			else if (typeFromHandle == typeof(string))
			{
				_value = (T)(object)_data;
			}
			else
			{
				_value = JsonMapper.ToObject<T>(_data);
			}
		}

		public static Value<T> FromRaw(string data)
		{
			return new Value<T>(data);
		}

		public static Value<T> FromValue(T val)
		{
			return new Value<T>().Set(val);
		}

		public T Get()
		{
			return _value;
		}

		public Value<T> Set(T val)
		{
			_value = val;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				_data = val.ToString();
			}
			else if (typeFromHandle == typeof(string))
			{
				_data = _value.ToString();
			}
			else
			{
				_data = JsonMapper.ToJson(val);
			}
			return this;
		}

		public string GetRaw()
		{
			return _data;
		}
	}

	public const string KEY_VALUE_DB = "keyValue.db";

	private static Dictionary<Storage, KeyValueStorage> storages = new Dictionary<Storage, KeyValueStorage>(4);

	private static KeyValueAccess db;

	private static string dbPath;

	private Storage storage;

	private KeyValueStorage(Storage storage)
	{
		this.storage = storage;
	}

	public static void Init()
	{
		Log.Debug("KeyValueStorage.Init");
		long num = DateTime.UtcNow.Ticks / 10000;
		db = new KeyValueAccess();
		dbPath = Path.Combine(Singleton<InitializationManager>.instance.persistentDataPath, "keyValue.db");
		dbPath = Singleton<InitializationManager>.instance.persistentDataPath + "/keyValue.db";
		Log.Info("KVS: " + dbPath);
		AccessBase.Error error = db.Connect(dbPath);
		if (error != null)
		{
			throw new Exception(error.description + " " + error.code);
		}
		long num2 = DateTime.UtcNow.Ticks / 10000;
		Log.Debug("KeyValueStorage.Init took: " + (num2 - num));
	}

	public static KeyValueStorage Instance(Storage storage)
	{
		lock (storages)
		{
			KeyValueStorage value;
			if (!storages.TryGetValue(storage, out value))
			{
				value = (storages[storage] = new KeyValueStorage(storage));
			}
			return value;
		}
	}

	private string CreateStorageKey(string key)
	{
		return storage.ToString() + "_" + key;
	}

	public bool ContainsKey(string key)
	{
		AccessBase.Error single;
		KeyValueModel m;
		lock (db)
		{
			single = db.GetSingle(CreateStorageKey(key), out m);
		}
		if (single != null)
		{
			return false;
		}
		return m != null;
	}

	public T GetValue<T>(string key)
	{
		AccessBase.Error error = null;
		KeyValueModel m;
		lock (db)
		{
			error = db.GetSingle(CreateStorageKey(key), out m);
		}
		if (error != null)
		{
			Log.Error("Error geting value for key: {0}, Error: {1}", key, error.description);
			return default(T);
		}
		if (m != null)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(JsonData))
			{
				return (T)(object)JsonMapper.ToObject(m.value);
			}
			if (typeFromHandle == typeof(string))
			{
				return (T)(object)m.value;
			}
			if (typeof(IList).IsAssignableFrom(typeFromHandle))
			{
				if (typeFromHandle.IsArray)
				{
					return Value<T>.FromRaw(m.value).Get();
				}
				IList list = Value<object[]>.FromRaw(m.value).Get();
				return (T)list;
			}
			return Value<T>.FromRaw(m.value).Get();
		}
		return default(T);
	}

	public void SetValueAsync<T>(string key, T val)
	{
		WorkQueue.Do(delegate
		{
			SetValue(key, val);
		});
	}

	public void SetValueAsync<T>(string key, T val, AsyncQueue<WorkQueue, WorkQueue.Request, WorkQueue.Response>.Callback doneCallback)
	{
		WorkQueue.Do(delegate
		{
			SetValue(key, val);
			return (object)null;
		}, doneCallback);
	}

	public void SetValue<T>(string key, T val)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(double))
		{
			throw new Exception("Double is not supported by this storage");
		}
		string val2 = ((typeFromHandle != typeof(string)) ? Value<T>.FromValue(val).GetRaw() : ((string)(object)val));
		AccessBase.Error error;
		lock (db)
		{
			error = db.InsertOrUpdate(new KeyValueModel(CreateStorageKey(key), val2));
		}
		if (error != null)
		{
			throw new Exception(error.description);
		}
	}

	public AccessBase.Error Remove(string key)
	{
		lock (db)
		{
			return db.Delete(CreateStorageKey(key));
		}
	}

	public AccessBase.Error ClearValues()
	{
		lock (db)
		{
			IList list = new ArrayList();
			AccessBase.Error error = db.GetAll(out list);
			for (int i = 0; i < list.Count; i++)
			{
				KeyValueModel keyValueModel = (KeyValueModel)list[i];
				if (keyValueModel.key.StartsWith(storage.ToString()))
				{
					error = db.Delete(keyValueModel.key);
					if (error != null)
					{
						return error;
					}
				}
			}
			return error;
		}
	}

	public static void DeleteDBFile()
	{
		lock (db)
		{
			db.Disconnect();
			try
			{
				File.Delete(dbPath);
			}
			catch
			{
			}
		}
	}

	public static void Disconnect()
	{
		lock (db)
		{
			db.Disconnect();
		}
	}
}
