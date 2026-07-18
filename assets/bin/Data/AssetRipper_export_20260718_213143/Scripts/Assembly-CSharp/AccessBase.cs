using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class AccessBase
{
	public class Error
	{
		public enum Code
		{
			None = 0,
			Unknown = 1,
			NotConnected = 2,
			Exception = 3,
			EmptyResult = 4,
			OldVersion = 5,
			WrongDataModel = 6
		}

		public Code code;

		public string description;

		public Error(Code code, string description)
		{
			this.code = code;
			this.description = description;
		}

		public static Error NotConnected()
		{
			return new Error(Code.NotConnected, "Not connected to database");
		}

		public static Error Exception(Exception e)
		{
			return new Error(Code.Exception, e.ToString());
		}

		public static Error EmptyResult()
		{
			return new Error(Code.EmptyResult, "Empty result set");
		}

		public static Error OldVersion()
		{
			return new Error(Code.OldVersion, "The current DataModel db is too old for this client");
		}

		public static Error WrongDataModel(string dataModel)
		{
			return new Error(Code.WrongDataModel, "Wrong data model:" + dataModel);
		}
	}

	protected delegate void DoRead(SqliteDataReader reader);

	protected SqliteConnection db;

	protected void OpenReader(string query, DoRead doRead)
	{
		SqliteCommand sqliteCommand = db.CreateCommand();
		try
		{
			sqliteCommand.CommandText = query;
			SqliteDataReader reader = sqliteCommand.ExecuteReader();
			try
			{
				doRead(reader);
			}
			finally
			{
				IDisposable disposable = reader as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		finally
		{
			if (sqliteCommand != null)
			{
				((IDisposable)(object)sqliteCommand).Dispose();
			}
		}
	}

	protected Error GetSingle<OUT>(string query, out OUT m, Func<SqliteDataReader, OUT> parser) where OUT : class
	{
		m = (OUT)null;
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			OUT internalM = (OUT)null;
			OpenReader(query, delegate(SqliteDataReader reader)
			{
				if (reader.Read())
				{
					internalM = parser(reader);
				}
			});
			if (internalM == null)
			{
				return Error.EmptyResult();
			}
			m = internalM;
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	protected Error GetList<OUT>(string query, out List<OUT> list, Func<SqliteDataReader, OUT> parser) where OUT : class
	{
		list = null;
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			List<OUT> internalList = null;
			OpenReader(query, delegate(SqliteDataReader reader)
			{
				internalList = new List<OUT>();
				while (reader.Read())
				{
					internalList.Add(parser(reader));
				}
			});
			list = internalList;
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	public bool IsConnected()
	{
		if (db == null)
		{
			return false;
		}
		return true;
	}

	public Error Connect(string path)
	{
		try
		{
			if (db != null)
			{
				db.Close();
			}
			string text = "URI=file:" + path;
			Log.Debug("Connection string: " + text);
			db = new SqliteConnection(text);
			db.Open();
			return null;
		}
		catch (Exception e)
		{
			db = null;
			return Error.Exception(e);
		}
	}

	public void Disconnect()
	{
		if (db != null)
		{
			db.Close();
			db = null;
		}
	}

	protected T GetSingleObject<T>(string query, Func<SqliteDataReader, T> parser) where T : class
	{
		T m = (T)null;
		try
		{
			if (!IsConnected())
			{
				throw new Exception(Error.NotConnected().ToString());
			}
			OpenReader(query, delegate(SqliteDataReader reader)
			{
				if (reader.Read())
				{
					m = parser(reader);
				}
			});
			return m;
		}
		catch (Exception ex)
		{
			Log.Error("Error on AccessBase.GetSingleObject" + ex.ToString() + "\nquery:" + query);
			return (T)null;
		}
	}

	protected List<T> GetMultiObject<T>(string query, Func<SqliteDataReader, T> parser) where T : class
	{
		List<T> list = new List<T>();
		try
		{
			if (!IsConnected())
			{
				throw new Exception(Error.NotConnected().ToString());
			}
			OpenReader(query, delegate(SqliteDataReader reader)
			{
				while (reader.Read())
				{
					list.Add(parser(reader));
				}
			});
			return list;
		}
		catch (Exception ex)
		{
			Log.Error("Error on AccessBase.GetSingleObject" + ex.ToString());
			return new List<T>();
		}
	}
}
