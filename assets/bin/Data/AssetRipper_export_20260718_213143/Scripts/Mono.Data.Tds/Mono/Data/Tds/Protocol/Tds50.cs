using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Mono.Data.Tds.Protocol
{
	[System.MonoTODO("FIXME: Can packetsize be anything other than 512?")]
	public sealed class Tds50 : Tds
	{
		public static readonly TdsVersion Version = TdsVersion.tds50;

		private int packetSize;

		private bool isSelectQuery;

		public Tds50(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds50(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, Version)
		{
			this.packetSize = packetSize;
		}

		public string BuildExec(string sql)
		{
			if (base.Parameters == null || base.Parameters.Count == 0)
			{
				return sql;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				stringBuilder3.Append(string.Format("declare {0}\n", item.Prepare()));
				stringBuilder2.Append(string.Format("select {0}=", item.ParameterName));
				if (item.Direction == TdsParameterDirection.Input)
				{
					stringBuilder2.Append(FormatParameter(item));
				}
				else
				{
					stringBuilder2.Append("NULL");
					stringBuilder.Append(item.ParameterName);
					if (num == 0)
					{
						stringBuilder.Append("select ");
					}
					else
					{
						stringBuilder.Append(", ");
					}
					num++;
				}
				stringBuilder2.Append("\n");
			}
			return string.Format("{0}{1}{2}\n{3}", stringBuilder3.ToString(), stringBuilder2.ToString(), sql, stringBuilder.ToString());
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			byte[] b = new byte[8] { 3, 239, 101, 65, 255, 255, 255, 214 };
			byte[] b2 = new byte[8] { 0, 0, 0, 6, 72, 0, 0, 8 };
			SetCharset(connectionParameters.Charset);
			SetLanguage(connectionParameters.Language);
			byte pad = 0;
			byte[] b3 = new byte[0];
			base.Comm.StartPacket(TdsPacketType.Logon);
			byte[] array = base.Comm.Append(connectionParameters.Hostname, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(connectionParameters.User, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(connectionParameters.Password, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append("37876", 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append((byte)3);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)6);
			base.Comm.Append((byte)10);
			base.Comm.Append((byte)9);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append(b3, 7, pad);
			array = base.Comm.Append(connectionParameters.ApplicationName, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(base.DataSource, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append(b3, 2, pad);
			array = base.Comm.Append(connectionParameters.Password, 253, pad);
			base.Comm.Append((byte)((array.Length >= 253) ? 255u : ((uint)(array.Length + 2))));
			base.Comm.Append((byte)5);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			array = base.Comm.Append(connectionParameters.ProgName, 10, pad);
			base.Comm.Append((byte)((array.Length >= 10) ? 10u : ((uint)array.Length)));
			base.Comm.Append((byte)6);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)13);
			base.Comm.Append((byte)17);
			array = base.Comm.Append(base.Language, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append((byte)1);
			base.Comm.Append((short)0);
			base.Comm.Append(b3, 8, pad);
			base.Comm.Append((short)0);
			base.Comm.Append((byte)0);
			array = base.Comm.Append(base.Charset, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append((byte)1);
			array = base.Comm.Append(packetSize.ToString(), 6, pad);
			base.Comm.Append((byte)((array.Length >= 6) ? 6u : ((uint)array.Length)));
			base.Comm.Append(b3, 8, pad);
			base.Comm.Append((byte)226);
			base.Comm.Append((short)20);
			base.Comm.Append((byte)1);
			base.Comm.Append(b);
			base.Comm.Append((byte)2);
			base.Comm.Append(b2);
			base.Comm.SendPacket();
			base.MoreResults = true;
			SkipToEnd();
			return base.IsConnected;
		}

		public override void ExecPrepared(string id, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			bool flag = base.Parameters != null && base.Parameters.Count > 0;
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append((byte)231);
			base.Comm.Append((short)(id.Length + 5));
			base.Comm.Append((byte)2);
			base.Comm.Append((byte)(flag ? 1u : 0u));
			base.Comm.Append((byte)id.Length);
			base.Comm.Append(id);
			base.Comm.Append((short)0);
			if (flag)
			{
				SendParamFormat();
				SendParams();
			}
			base.MoreResults = true;
			base.Comm.SendPacket();
			CheckForData(timeout);
			if (!wantResults)
			{
				SkipToEnd();
			}
		}

		public override void Execute(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			string sql2 = BuildExec(sql);
			ExecuteQuery(sql2, timeout, wantResults);
		}

		public override void ExecProc(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			ExecuteQuery(BuildProcedureCall(commandText), timeout, wantResults);
		}

		private string BuildProcedureCall(string procedure)
		{
			string text = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = 0;
			if (base.Parameters != null)
			{
				foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
				{
					if (item.Direction != TdsParameterDirection.Input)
					{
						if (num == 0)
						{
							stringBuilder2.Append("select ");
						}
						else
						{
							stringBuilder2.Append(", ");
						}
						stringBuilder2.Append(item.ParameterName);
						stringBuilder.Append(string.Format("declare {0}\n", item.Prepare()));
						if (item.Direction != TdsParameterDirection.ReturnValue)
						{
							if (item.Direction == TdsParameterDirection.InputOutput)
							{
								stringBuilder3.Append(string.Format("set {0}\n", FormatParameter(item)));
							}
							else
							{
								stringBuilder3.Append(string.Format("set {0}=NULL\n", item.ParameterName));
							}
						}
						num++;
					}
					if (item.Direction == TdsParameterDirection.ReturnValue)
					{
						text = item.ParameterName + "=";
					}
				}
			}
			text = "exec " + text;
			return string.Format("{0}{1}{2}{3} {4}\n{5}", stringBuilder.ToString(), stringBuilder3.ToString(), text, procedure, BuildParameters(), stringBuilder2.ToString());
		}

		private string BuildParameters()
		{
			if (base.Parameters == null || base.Parameters.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				if (item.Direction != TdsParameterDirection.ReturnValue)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					if (item.Direction == TdsParameterDirection.InputOutput)
					{
						stringBuilder.Append(string.Format("{0}={0} output", item.ParameterName));
					}
					else
					{
						stringBuilder.Append(FormatParameter(item));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string FormatParameter(TdsMetaParameter parameter)
		{
			if (parameter.Direction == TdsParameterDirection.Output)
			{
				return string.Format("{0} output", parameter.ParameterName);
			}
			if (parameter.Value == null || parameter.Value == DBNull.Value)
			{
				return "NULL";
			}
			switch (parameter.TypeName)
			{
			case "smalldatetime":
			case "datetime":
			{
				DateTime dateTime = (DateTime)parameter.Value;
				return string.Format(CultureInfo.InvariantCulture, "'{0:MMM dd yyyy hh:mm:ss tt}'", dateTime);
			}
			case "bigint":
			case "decimal":
			case "float":
			case "int":
			case "money":
			case "real":
			case "smallint":
			case "smallmoney":
			case "tinyint":
				return parameter.Value.ToString();
			case "nvarchar":
			case "nchar":
				return string.Format("N'{0}'", parameter.Value.ToString().Replace("'", "''"));
			case "uniqueidentifier":
				return string.Format("0x{0}", ((Guid)parameter.Value).ToString("N"));
			case "bit":
				if (parameter.Value.GetType() == typeof(bool))
				{
					return (!(bool)parameter.Value) ? "0x0" : "0x1";
				}
				return parameter.Value.ToString();
			case "image":
			case "binary":
			case "varbinary":
				return string.Format("0x{0}", BitConverter.ToString((byte[])parameter.Value).Replace("-", string.Empty).ToLower());
			default:
				return string.Format("'{0}'", parameter.Value.ToString().Replace("'", "''"));
			}
		}

		public override string Prepare(string sql, TdsMetaParameterCollection parameters)
		{
			base.Parameters = parameters;
			Random random = new Random();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 25; i++)
			{
				stringBuilder.Append((char)(random.Next(26) + 65));
			}
			string text = stringBuilder.ToString();
			sql = string.Format("create proc {0} as\n{1}", text, sql);
			short s = (short)(text.Length + sql.Length + 5);
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append((byte)231);
			base.Comm.Append(s);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)text.Length);
			base.Comm.Append(text);
			base.Comm.Append((short)sql.Length);
			base.Comm.Append(sql);
			base.Comm.SendPacket();
			base.MoreResults = true;
			SkipToEnd();
			return text;
		}

		protected override void ProcessColumnInfo()
		{
			isSelectQuery = true;
			base.Comm.GetTdsShort();
			int tdsShort = base.Comm.GetTdsShort();
			for (int i = 0; i < tdsShort; i++)
			{
				string columnName = base.Comm.GetString(base.Comm.GetByte());
				int num = base.Comm.GetByte();
				bool value = (num & 1) > 0;
				bool value2 = (num & 2) > 0;
				bool value3 = (num & 4) > 0;
				bool flag = (num & 0x10) > 0;
				bool value4 = (num & 0x20) > 0;
				bool value5 = (num & 0x40) > 0;
				base.Comm.Skip(4L);
				byte b = base.Comm.GetByte();
				bool flag2 = b == 36;
				TdsColumnType tdsColumnType = (TdsColumnType)b;
				int num2 = 0;
				byte value6 = 0;
				byte value7 = 0;
				if (tdsColumnType != TdsColumnType.Text && tdsColumnType != TdsColumnType.Image)
				{
					num2 = ((!Tds.IsFixedSizeColumn(tdsColumnType)) ? base.Comm.GetByte() : Tds.LookupBufferSize(tdsColumnType));
				}
				else
				{
					num2 = base.Comm.GetTdsInt();
					base.Comm.Skip(base.Comm.GetTdsShort());
				}
				if (tdsColumnType == TdsColumnType.Decimal || tdsColumnType == TdsColumnType.Numeric)
				{
					value6 = base.Comm.GetByte();
					value7 = base.Comm.GetByte();
				}
				base.Comm.Skip((int)base.Comm.GetByte());
				if (flag2)
				{
					base.Comm.Skip(base.Comm.GetTdsShort());
				}
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = tdsColumnType;
				tdsDataColumn.ColumnName = columnName;
				tdsDataColumn.IsIdentity = value5;
				tdsDataColumn.IsRowVersion = value3;
				tdsDataColumn.ColumnType = tdsColumnType;
				tdsDataColumn.ColumnSize = num2;
				tdsDataColumn.NumericPrecision = value6;
				tdsDataColumn.NumericScale = value7;
				tdsDataColumn.IsReadOnly = !flag;
				tdsDataColumn.IsKey = value2;
				tdsDataColumn.AllowDBNull = value4;
				tdsDataColumn.IsHidden = value;
			}
		}

		private void SendParamFormat()
		{
			base.Comm.Append((byte)236);
			int num = 2 + 8 * base.Parameters.Count;
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				TdsColumnType metaType = item.GetMetaType();
				if (!Tds.IsFixedSizeColumn(metaType))
				{
					num++;
				}
				if (metaType == TdsColumnType.Numeric || metaType == TdsColumnType.Decimal)
				{
					num += 2;
				}
			}
			base.Comm.Append((short)num);
			base.Comm.Append((short)base.Parameters.Count);
			foreach (TdsMetaParameter item2 in (IEnumerable)base.Parameters)
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				int i = 0;
				byte b = 0;
				if (item2.IsNullable)
				{
					b |= 0x20;
				}
				if (item2.Direction == TdsParameterDirection.Output)
				{
					b |= 1;
				}
				TdsColumnType metaType = item2.GetMetaType();
				base.Comm.Append((byte)empty2.Length);
				base.Comm.Append(empty2);
				base.Comm.Append(b);
				base.Comm.Append(i);
				base.Comm.Append((byte)metaType);
				if (!Tds.IsFixedSizeColumn(metaType))
				{
					base.Comm.Append((byte)item2.Size);
				}
				if (metaType == TdsColumnType.Numeric || metaType == TdsColumnType.Decimal)
				{
					base.Comm.Append(item2.Precision);
					base.Comm.Append(item2.Scale);
				}
				base.Comm.Append((byte)empty.Length);
				base.Comm.Append(empty);
			}
		}

		private void SendParams()
		{
			base.Comm.Append((byte)215);
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				TdsColumnType metaType = item.GetMetaType();
				bool flag = item.Value == DBNull.Value || item.Value == null;
				if (!Tds.IsFixedSizeColumn(metaType))
				{
					base.Comm.Append((byte)item.GetActualSize());
				}
				if (!flag)
				{
					base.Comm.Append(item.Value);
				}
			}
		}

		public override void Unprepare(string statementId)
		{
			base.Comm.StartPacket(TdsPacketType.Normal);
			base.Comm.Append((byte)231);
			base.Comm.Append((short)(3 + statementId.Length));
			base.Comm.Append((byte)4);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)statementId.Length);
			base.Comm.Append(statementId);
			base.MoreResults = true;
			base.Comm.SendPacket();
			SkipToEnd();
		}

		protected override bool IsValidRowCount(byte status, byte op)
		{
			if (isSelectQuery)
			{
				return isSelectQuery = false;
			}
			if ((status & 0x40) != 0 || (status & 0x10) == 0)
			{
				return false;
			}
			return true;
		}
	}
}
