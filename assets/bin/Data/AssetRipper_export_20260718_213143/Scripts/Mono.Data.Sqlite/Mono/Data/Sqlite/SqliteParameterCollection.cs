using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Mono.Data.Sqlite
{
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class SqliteParameterCollection : DbParameterCollection
	{
		private SqliteCommand _command;

		private List<SqliteParameter> _parameterList;

		private bool _unboundFlag;

		public override bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return null;
			}
		}

		public override int Count
		{
			get
			{
				return _parameterList.Count;
			}
		}

		public new SqliteParameter this[string parameterName]
		{
			get
			{
				return (SqliteParameter)GetParameter(parameterName);
			}
			set
			{
				SetParameter(parameterName, value);
			}
		}

		public new SqliteParameter this[int index]
		{
			get
			{
				return (SqliteParameter)GetParameter(index);
			}
			set
			{
				SetParameter(index, value);
			}
		}

		internal SqliteParameterCollection(SqliteCommand cmd)
		{
			_command = cmd;
			_parameterList = new List<SqliteParameter>();
			_unboundFlag = true;
		}

		public override IEnumerator GetEnumerator()
		{
			return _parameterList.GetEnumerator();
		}

		public SqliteParameter Add(string parameterName, DbType parameterType, int parameterSize, string sourceColumn)
		{
			SqliteParameter sqliteParameter = new SqliteParameter(parameterName, parameterType, parameterSize, sourceColumn);
			Add(sqliteParameter);
			return sqliteParameter;
		}

		public SqliteParameter Add(string parameterName, DbType parameterType, int parameterSize)
		{
			SqliteParameter sqliteParameter = new SqliteParameter(parameterName, parameterType, parameterSize);
			Add(sqliteParameter);
			return sqliteParameter;
		}

		public SqliteParameter Add(string parameterName, DbType parameterType)
		{
			SqliteParameter sqliteParameter = new SqliteParameter(parameterName, parameterType);
			Add(sqliteParameter);
			return sqliteParameter;
		}

		public int Add(SqliteParameter parameter)
		{
			int num = -1;
			if (!string.IsNullOrEmpty(parameter.ParameterName))
			{
				num = IndexOf(parameter.ParameterName);
			}
			if (num == -1)
			{
				num = _parameterList.Count;
				_parameterList.Add(parameter);
			}
			SetParameter(num, parameter);
			return num;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			return Add((SqliteParameter)value);
		}

		public SqliteParameter AddWithValue(string parameterName, object value)
		{
			SqliteParameter sqliteParameter = new SqliteParameter(parameterName, value);
			Add(sqliteParameter);
			return sqliteParameter;
		}

		public void AddRange(SqliteParameter[] values)
		{
			int num = values.Length;
			for (int i = 0; i < num; i++)
			{
				Add(values[i]);
			}
		}

		public override void AddRange(Array values)
		{
			int length = values.Length;
			for (int i = 0; i < length; i++)
			{
				Add((SqliteParameter)values.GetValue(i));
			}
		}

		public override void Clear()
		{
			_unboundFlag = true;
			_parameterList.Clear();
		}

		public override bool Contains(string parameterName)
		{
			return IndexOf(parameterName) != -1;
		}

		public override bool Contains(object value)
		{
			return _parameterList.Contains((SqliteParameter)value);
		}

		public override void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return GetParameter(IndexOf(parameterName));
		}

		protected override DbParameter GetParameter(int index)
		{
			return _parameterList[index];
		}

		public override int IndexOf(string parameterName)
		{
			int count = _parameterList.Count;
			for (int i = 0; i < count; i++)
			{
				if (string.Compare(parameterName, _parameterList[i].ParameterName, true, CultureInfo.InvariantCulture) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public override int IndexOf(object value)
		{
			return _parameterList.IndexOf((SqliteParameter)value);
		}

		public override void Insert(int index, object value)
		{
			_unboundFlag = true;
			_parameterList.Insert(index, (SqliteParameter)value);
		}

		public override void Remove(object value)
		{
			_unboundFlag = true;
			_parameterList.Remove((SqliteParameter)value);
		}

		public override void RemoveAt(string parameterName)
		{
			RemoveAt(IndexOf(parameterName));
		}

		public override void RemoveAt(int index)
		{
			_unboundFlag = true;
			_parameterList.RemoveAt(index);
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			SetParameter(IndexOf(parameterName), value);
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			_unboundFlag = true;
			_parameterList[index] = (SqliteParameter)value;
		}

		internal void Unbind()
		{
			_unboundFlag = true;
		}

		internal void MapParameters(SqliteStatement activeStatement)
		{
			if (!_unboundFlag || _parameterList.Count == 0 || _command._statementList == null)
			{
				return;
			}
			int num = 0;
			int num2 = -1;
			foreach (SqliteParameter parameter in _parameterList)
			{
				num2++;
				string text = parameter.ParameterName;
				if (text == null)
				{
					text = string.Format(CultureInfo.InvariantCulture, ";{0}", num);
					num++;
				}
				bool flag = false;
				int num3 = ((activeStatement != null) ? 1 : _command._statementList.Count);
				SqliteStatement sqliteStatement = activeStatement;
				for (int i = 0; i < num3; i++)
				{
					flag = false;
					if (sqliteStatement == null)
					{
						sqliteStatement = _command._statementList[i];
					}
					if (sqliteStatement._paramNames != null && sqliteStatement.MapParameter(text, parameter))
					{
						flag = true;
					}
					sqliteStatement = null;
				}
				if (flag)
				{
					continue;
				}
				text = string.Format(CultureInfo.InvariantCulture, ";{0}", num2);
				sqliteStatement = activeStatement;
				for (int i = 0; i < num3; i++)
				{
					if (sqliteStatement == null)
					{
						sqliteStatement = _command._statementList[i];
					}
					if (sqliteStatement._paramNames != null && sqliteStatement.MapParameter(text, parameter))
					{
						flag = true;
					}
					sqliteStatement = null;
				}
			}
			if (activeStatement == null)
			{
				_unboundFlag = false;
			}
		}
	}
}
