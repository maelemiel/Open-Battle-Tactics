using System;
using UnityEngine;

namespace MobageEditor
{
	public class ModelDiskCache
	{
		private static ModelDiskCache defaultInstance;

		private static DBContext context;

		public bool Enabled { get; set; }

		public static ModelDiskCache DefaultCache
		{
			get
			{
				if (defaultInstance == null)
				{
					ModelDiskCache modelDiskCache = new ModelDiskCache();
					modelDiskCache.Enabled = true;
					defaultInstance = modelDiskCache;
				}
				return defaultInstance;
			}
		}

		public DBContext DBContext
		{
			get
			{
				if (context == null)
				{
					context = new DBContext();
				}
				return context;
			}
		}

		public void ValueForKey(string key, Action<string> cb)
		{
			Action<string> cb2 = delegate(string val)
			{
				cb(val);
			};
			DBContext dBContext = DBContext;
			KeyValueLookupWithQuery(key, cb2);
		}

		public void SetValueForKey(string key, string v)
		{
			keyValueInsert(key, v);
		}

		public void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}

		private void KeyValueLookupWithQuery(string key, Action<string> cb)
		{
			Debug.Log(string.Format("Key: {0}", key));
			cb(PlayerPrefs.GetString(key));
		}

		private void keyValueInsert(string key, string v)
		{
			if (Enabled)
			{
				PlayerPrefs.SetString(key, v);
			}
		}
	}
}
