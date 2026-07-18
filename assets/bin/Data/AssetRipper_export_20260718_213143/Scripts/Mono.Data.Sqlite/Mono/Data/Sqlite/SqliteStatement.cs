using System;
using System.Data;
using System.Globalization;

namespace Mono.Data.Sqlite
{
	internal sealed class SqliteStatement : IDisposable
	{
		internal SQLiteBase _sql;

		internal string _sqlStatement;

		internal SqliteStatementHandle _sqlite_stmt;

		internal int _unnamedParameters;

		internal string[] _paramNames;

		internal SqliteParameter[] _paramValues;

		internal SqliteCommand _command;

		private string[] _types;

		internal string[] TypeDefinitions
		{
			get
			{
				return _types;
			}
		}

		internal SqliteStatement(SQLiteBase sqlbase, SqliteStatementHandle stmt, string strCommand, SqliteStatement previous)
		{
			_sql = sqlbase;
			_sqlite_stmt = stmt;
			_sqlStatement = strCommand;
			int num = 0;
			int num2 = _sql.Bind_ParamCount(this);
			if (num2 <= 0)
			{
				return;
			}
			if (previous != null)
			{
				num = previous._unnamedParameters;
			}
			_paramNames = new string[num2];
			_paramValues = new SqliteParameter[num2];
			for (int i = 0; i < num2; i++)
			{
				string text = _sql.Bind_ParamName(this, i + 1);
				if (string.IsNullOrEmpty(text))
				{
					text = string.Format(CultureInfo.InvariantCulture, ";{0}", num);
					num++;
					_unnamedParameters++;
				}
				_paramNames[i] = text;
				_paramValues[i] = null;
			}
		}

		internal bool MapParameter(string s, SqliteParameter p)
		{
			if (_paramNames == null)
			{
				return false;
			}
			int num = 0;
			if (s.Length > 0 && ":$@;".IndexOf(s[0]) == -1)
			{
				num = 1;
			}
			int num2 = _paramNames.Length;
			for (int i = 0; i < num2; i++)
			{
				if (string.Compare(_paramNames[i], num, s, 0, System.Math.Max(_paramNames[i].Length - num, s.Length), true, CultureInfo.InvariantCulture) == 0)
				{
					_paramValues[i] = p;
					return true;
				}
			}
			return false;
		}

		public void Dispose()
		{
			if (_sqlite_stmt != null)
			{
				_sqlite_stmt.Dispose();
			}
			_sqlite_stmt = null;
			_paramNames = null;
			_paramValues = null;
			_sql = null;
			_sqlStatement = null;
		}

		internal void BindParameters()
		{
			if (_paramNames != null)
			{
				int num = _paramNames.Length;
				for (int i = 0; i < num; i++)
				{
					BindParameter(i + 1, _paramValues[i]);
				}
			}
		}

		private void BindParameter(int index, SqliteParameter param)
		{
			if (param == null)
			{
				throw new SqliteException(1, "Insufficient parameters supplied to the command");
			}
			object value = param.Value;
			DbType dbType = param.DbType;
			if (Convert.IsDBNull(value) || value == null)
			{
				_sql.Bind_Null(this, index);
				return;
			}
			if (dbType == DbType.Object)
			{
				dbType = SqliteConvert.TypeToDbType(value.GetType());
			}
			switch (dbType)
			{
			case DbType.Date:
			case DbType.DateTime:
			case DbType.Time:
				_sql.Bind_DateTime(this, index, Convert.ToDateTime(value, CultureInfo.CurrentCulture));
				break;
			case DbType.Int64:
			case DbType.UInt64:
				_sql.Bind_Int64(this, index, Convert.ToInt64(value, CultureInfo.CurrentCulture));
				break;
			case DbType.Byte:
			case DbType.Boolean:
			case DbType.Int16:
			case DbType.Int32:
			case DbType.SByte:
			case DbType.UInt16:
			case DbType.UInt32:
				_sql.Bind_Int32(this, index, Convert.ToInt32(value, CultureInfo.CurrentCulture));
				break;
			case DbType.Currency:
			case DbType.Double:
			case DbType.Single:
				_sql.Bind_Double(this, index, Convert.ToDouble(value, CultureInfo.CurrentCulture));
				break;
			case DbType.Binary:
				_sql.Bind_Blob(this, index, (byte[])value);
				break;
			case DbType.Guid:
				if (_command.Connection._binaryGuid)
				{
					_sql.Bind_Blob(this, index, ((Guid)value).ToByteArray());
				}
				else
				{
					_sql.Bind_Text(this, index, value.ToString());
				}
				break;
			case DbType.Decimal:
				_sql.Bind_Text(this, index, Convert.ToDecimal(value, CultureInfo.CurrentCulture).ToString(CultureInfo.InvariantCulture));
				break;
			default:
				_sql.Bind_Text(this, index, value.ToString());
				break;
			}
		}

		internal void SetTypes(string typedefs)
		{
			int num = typedefs.IndexOf("TYPES", 0, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException();
			}
			string[] array = typedefs.Substring(num + 6).Replace(" ", string.Empty).Replace(";", string.Empty)
				.Replace("\"", string.Empty)
				.Replace("[", string.Empty)
				.Replace("]", string.Empty)
				.Replace("`", string.Empty)
				.Split(',', '\r', '\n', '\t');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					array[i] = null;
				}
			}
			_types = array;
		}
	}
}
