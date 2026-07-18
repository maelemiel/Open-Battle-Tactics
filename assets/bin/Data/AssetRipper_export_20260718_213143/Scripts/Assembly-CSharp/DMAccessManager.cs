using System;
using System.Collections.Generic;

public class DMAccessManager : NonUnitySingleton<DMAccessManager>
{
	private static bool VERBOSE_LOG;

	private DataModelAccess dataModelAccess;

	public DataModelAccess DataModelAccess
	{
		get
		{
			return dataModelAccess;
		}
	}

	public DMAccessManager()
	{
		if (VERBOSE_LOG)
		{
			Log.Debug("DMAccessManager.Init");
		}
		long num = DateTime.UtcNow.Ticks / 10000;
		dataModelAccess = new DataModelAccess();
		long num2 = DateTime.UtcNow.Ticks / 10000;
		Log.Debug("DMAccessManager.Init took: " + (num2 - num));
	}

	public AccessBase.Error Connect(string path, out ConnectDataModel m)
	{
		AccessBase.Error error = null;
		lock (dataModelAccess)
		{
			return dataModelAccess.Connect(path, out m);
		}
	}

	public AccessBase.Error GetSingleOld<T>(string id, out T q) where T : BaseDataModel, new()
	{
		AccessBase.Error result = null;
		lock (dataModelAccess)
		{
			q = dataModelAccess.GetSingle<T>(id);
			return result;
		}
	}

	public T GetSingle<T>(int id) where T : BaseDataModel, new()
	{
		return GetSingle<T>(id.ToString());
	}

	public T GetSingle<T>(string id) where T : BaseDataModel, new()
	{
		if (id == null)
		{
			return (T)null;
		}
		long num = DateTime.UtcNow.Ticks / 10000;
		Type typeFromHandle = typeof(T);
		bool flag = false;
		T val = (T)null;
		Dictionary<string, object> singleValueCache = CacheManager.GetSingleValueCache<T>();
		object value = null;
		if (singleValueCache.TryGetValue(id, out value))
		{
			val = value as T;
			flag = true;
		}
		else
		{
			lock (dataModelAccess)
			{
				try
				{
					val = dataModelAccess.GetSingle<T>(id);
				}
				catch
				{
					val = (T)null;
				}
			}
			singleValueCache.Add(id, val);
		}
		long num2 = DateTime.UtcNow.Ticks / 10000;
		if (VERBOSE_LOG && !flag)
		{
			Log.Debug(string.Concat("DMAccessManager.GetSingle type:", typeFromHandle, " id:", id.ToString(), " took: ", num2 - num, " fromCache:", flag));
		}
		return val;
	}

	public T GetSingleByQuery<T>(string whereClause) where T : BaseDataModel, new()
	{
		lock (dataModelAccess)
		{
			return dataModelAccess.GetSingleByQuery<T>(whereClause);
		}
	}

	public List<T> GetMultiByQuery<T>(string whereClause) where T : BaseDataModel, new()
	{
		lock (dataModelAccess)
		{
			return dataModelAccess.GetMultiByQuery<T>(whereClause);
		}
	}

	public List<T> GetAll<T>() where T : BaseDataModel, new()
	{
		long num = DateTime.UtcNow.Ticks / 10000;
		bool flag = false;
		List<T> list = null;
		Type typeFromHandle = typeof(T);
		if (CacheManager.cachedLists.ContainsKey(typeFromHandle))
		{
			list = CacheManager.cachedLists[typeFromHandle] as List<T>;
			flag = true;
		}
		else
		{
			lock (dataModelAccess)
			{
				list = dataModelAccess.GetAll<T>();
			}
			Dictionary<string, object> singleValueCache = CacheManager.GetSingleValueCache<T>();
			for (int i = 0; i < list.Count; i++)
			{
				T val = list[i];
				if (val.id != null)
				{
					if (singleValueCache.ContainsKey(val.id))
					{
						list[i] = singleValueCache[val.id] as T;
					}
					else
					{
						singleValueCache[val.id] = val;
					}
				}
			}
			CacheManager.cachedLists[typeFromHandle] = list;
		}
		long num2 = DateTime.UtcNow.Ticks / 10000;
		if (VERBOSE_LOG && !flag)
		{
			Log.Debug(string.Concat("DMAccessManager.GetAll type:", typeFromHandle, " count:", list.Count, " took: ", num2 - num, " fromCache:", flag));
		}
		return list;
	}

	public void CacheConstants()
	{
		lock (dataModelAccess)
		{
			dataModelAccess.CacheConstants();
		}
	}
}
