using System;
using System.Collections;
using Mono.Data.Sqlite;

public class KeyValueAccess : AccessBase
{
	private const string KEY_VALUE_SCHEMA = "CREATE TABLE KEY_VALUES (key       TEXT PRIMARY KEY,value     TEXT NOT NULL)";

	private const string KEY_VALUE_SELECT_SQL = "SELECT key, value FROM KEY_VALUES";

	private KeyValueModel ReadKeyValueDataMode(SqliteDataReader reader)
	{
		KeyValueModel keyValueModel = new KeyValueModel();
		int num = 0;
		keyValueModel.key = reader.GetString(num++);
		keyValueModel.value = reader.GetString(num++);
		return keyValueModel;
	}

	public Error Create()
	{
		try
		{
			SqliteCommand sqliteCommand = db.CreateCommand();
			try
			{
				sqliteCommand.CommandText = "CREATE TABLE KEY_VALUES (key       TEXT PRIMARY KEY,value     TEXT NOT NULL)";
				sqliteCommand.ExecuteNonQuery();
			}
			finally
			{
				if (sqliteCommand != null)
				{
					((IDisposable)(object)sqliteCommand).Dispose();
				}
			}
			return null;
		}
		catch (Exception e)
		{
			return Error.Exception(e);
		}
	}

	public Error GetSingle(string key, out KeyValueModel m)
	{
		m = null;
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			KeyValueModel internalM = null;
			OpenReader("SELECT key, value FROM KEY_VALUES where key='" + key + "'", delegate(SqliteDataReader reader)
			{
				if (reader.Read())
				{
					internalM = ReadKeyValueDataMode(reader);
				}
			});
			m = internalM;
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	public Error GetAll(out IList list)
	{
		list = null;
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			IList listInternal = null;
			OpenReader("SELECT key, value FROM KEY_VALUES order by key asc", delegate(SqliteDataReader reader)
			{
				listInternal = new ArrayList();
				while (reader.Read())
				{
					listInternal.Add(ReadKeyValueDataMode(reader));
				}
			});
			list = listInternal;
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	public Error InsertOrUpdate(KeyValueModel m)
	{
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			SqliteCommand sqliteCommand = db.CreateCommand();
			try
			{
				sqliteCommand.CommandText = "UPDATE KEY_VALUES SET value=? WHERE key=?";
				SqliteParameter sqliteParameter = sqliteCommand.CreateParameter();
				sqliteParameter.Value = m.value;
				sqliteCommand.Parameters.Add(sqliteParameter);
				sqliteParameter = sqliteCommand.CreateParameter();
				sqliteParameter.Value = m.key;
				sqliteCommand.Parameters.Add(sqliteParameter);
				if (sqliteCommand.ExecuteNonQuery() == 0)
				{
					sqliteCommand.CommandText = "INSERT INTO KEY_VALUES(value,key) VALUES(?,?)";
					sqliteCommand.ExecuteNonQuery();
				}
			}
			finally
			{
				if (sqliteCommand != null)
				{
					((IDisposable)(object)sqliteCommand).Dispose();
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	public Error Delete(string key)
	{
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			SqliteCommand sqliteCommand = db.CreateCommand();
			try
			{
				sqliteCommand.CommandText = "DELETE FROM KEY_VALUES WHERE key=?";
				SqliteParameter sqliteParameter = sqliteCommand.CreateParameter();
				sqliteParameter.Value = key;
				sqliteCommand.Parameters.Add(sqliteParameter);
				sqliteCommand.ExecuteNonQuery();
			}
			finally
			{
				if (sqliteCommand != null)
				{
					((IDisposable)(object)sqliteCommand).Dispose();
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}
}
