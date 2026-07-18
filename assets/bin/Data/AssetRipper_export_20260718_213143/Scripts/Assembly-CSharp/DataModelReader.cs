using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class DataModelReader<T> where T : BaseDataModel, new()
{
	private Dictionary<string, Action> source;

	public string sqlRequest;

	public string tableName;

	public T model;

	private SqliteDataReader reader;

	private int readerIndex;

	public DataModelReader(string tableName, Action<DataModelReader<T>> setItemsCb)
	{
		source = new Dictionary<string, Action>();
		this.tableName = tableName;
		setItemsCb(this);
		CreateRequestString();
	}

	public void AddItem(string sqlField, Action cb)
	{
		source[sqlField] = cb;
	}

	private void CreateRequestString()
	{
		sqlRequest = "SELECT ";
		foreach (KeyValuePair<string, Action> item in source)
		{
			sqlRequest = sqlRequest + item.Key + ", ";
		}
		sqlRequest = sqlRequest.Substring(0, sqlRequest.Length - 2);
		sqlRequest = sqlRequest + " FROM " + tableName + " ";
	}

	public T MakeSqlRequest(SqliteDataReader reader)
	{
		this.reader = reader;
		readerIndex = 0;
		model = new T();
		foreach (KeyValuePair<string, Action> item in source)
		{
			item.Value();
			readerIndex++;
		}
		readerIndex = 0;
		return model;
	}

	public R ReadOneValue<R>()
	{
		Type typeFromHandle = typeof(R);
		if (typeFromHandle == typeof(string))
		{
			string text = reader.GetValue(readerIndex) as string;
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			return (R)(object)text;
		}
		if (typeFromHandle == typeof(int))
		{
			return (R)(object)reader.GetInt32(readerIndex);
		}
		if (typeFromHandle == typeof(long))
		{
			return (R)(object)reader.GetInt64(readerIndex);
		}
		if (typeFromHandle == typeof(float))
		{
			return (R)(object)reader.GetFloat(readerIndex);
		}
		return (R)(object)null;
	}
}
