using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Data.Tds.Protocol
{
	public class Tds70 : Tds
	{
		private static readonly decimal SMALLMONEY_MIN = -214748.3648m;

		private static readonly decimal SMALLMONEY_MAX = 214748.3647m;

		protected virtual byte[] ClientVersion
		{
			get
			{
				return new byte[4] { 0, 0, 0, 112 };
			}
		}

		public Tds70(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds70(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, TdsVersion.tds70)
		{
		}

		public Tds70(string server, int port, int packetSize, int timeout, TdsVersion version)
			: base(server, port, packetSize, timeout, version)
		{
		}

		private string BuildExec(string sql)
		{
			string arg = sql.Replace("'", "''");
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				return BuildProcedureCall(string.Format("sp_executesql N'{0}', N'{1}', ", arg, BuildPreparedParameters()));
			}
			return BuildProcedureCall(string.Format("sp_executesql N'{0}'", arg));
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
				string text = item.ParameterName;
				if (text[0] == '@')
				{
					text = text.Substring(1);
				}
				if (item.Direction != TdsParameterDirection.ReturnValue)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					if (item.Direction == TdsParameterDirection.InputOutput)
					{
						stringBuilder.AppendFormat("@{0}={0} output", text);
					}
					else
					{
						stringBuilder.Append(FormatParameter(item));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string BuildPreparedParameters()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(item.Prepare());
				if (item.Direction == TdsParameterDirection.Output)
				{
					stringBuilder.Append(" output");
				}
			}
			return stringBuilder.ToString();
		}

		private string BuildPreparedQuery(string id)
		{
			return BuildProcedureCall(string.Format("sp_execute {0},", id));
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
					string text2 = item.ParameterName;
					if (text2[0] == '@')
					{
						text2 = text2.Substring(1);
					}
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
						stringBuilder2.Append("@" + text2);
						stringBuilder.Append(string.Format("declare {0}\n", item.Prepare()));
						if (item.Direction != TdsParameterDirection.ReturnValue)
						{
							if (item.Direction == TdsParameterDirection.InputOutput)
							{
								stringBuilder3.Append(string.Format("set {0}\n", FormatParameter(item)));
							}
							else
							{
								stringBuilder3.Append(string.Format("set @{0}=NULL\n", text2));
							}
						}
						num++;
					}
					if (item.Direction == TdsParameterDirection.ReturnValue)
					{
						text = "@" + text2 + "=";
					}
				}
			}
			text = "exec " + text;
			return string.Format("{0}{1}{2}{3} {4}\n{5}", stringBuilder.ToString(), stringBuilder3.ToString(), text, procedure, BuildParameters(), stringBuilder2.ToString());
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			connectionParms = connectionParameters;
			SetLanguage(connectionParameters.Language);
			SetCharset("utf-8");
			byte[] b = new byte[0];
			short num = 0;
			byte pad = 0;
			byte[] array = new byte[21]
			{
				6, 125, 15, 253, 255, 0, 0, 0, 0, 224,
				131, 0, 0, 104, 1, 0, 0, 9, 4, 0,
				0
			};
			byte[] array2 = new byte[21]
			{
				6, 0, 0, 0, 0, 0, 0, 0, 0, 224,
				3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0
			};
			byte[] array3 = null;
			array3 = ((!connectionParameters.DomainLogin) ? array2 : array);
			string text = connectionParameters.User;
			string text2 = null;
			int num2 = text.IndexOf("\\");
			if (num2 == -1)
			{
				text2 = (connectionParameters.DefaultDomain = Environment.UserDomainName);
			}
			else
			{
				text2 = text.Substring(0, num2);
				text = text.Substring(num2 + 1);
				connectionParameters.DefaultDomain = text2;
				connectionParameters.User = text;
			}
			short num3 = (short)(86 + (connectionParameters.Hostname.Length + connectionParameters.ApplicationName.Length + base.DataSource.Length + connectionParameters.LibraryName.Length + base.Language.Length + connectionParameters.Database.Length + connectionParameters.AttachDBFileName.Length) * 2);
			if (connectionParameters.DomainLogin)
			{
				num = (short)(32 + (connectionParameters.Hostname.Length + text2.Length));
				num3 += num;
			}
			else
			{
				num3 += (short)((text.Length + connectionParameters.Password.Length) * 2);
			}
			int i = num3;
			base.Comm.StartPacket(TdsPacketType.Logon70);
			base.Comm.Append(i);
			base.Comm.Append(ClientVersion);
			base.Comm.Append(base.PacketSize);
			base.Comm.Append(b, 3, pad);
			base.Comm.Append(array3);
			short num4 = 86;
			base.Comm.Append(num4);
			base.Comm.Append((short)connectionParameters.Hostname.Length);
			num4 += (short)(connectionParameters.Hostname.Length * 2);
			if (connectionParameters.DomainLogin)
			{
				base.Comm.Append((short)0);
				base.Comm.Append((short)0);
				base.Comm.Append((short)0);
				base.Comm.Append((short)0);
			}
			else
			{
				base.Comm.Append(num4);
				base.Comm.Append((short)text.Length);
				num4 += (short)(text.Length * 2);
				base.Comm.Append(num4);
				base.Comm.Append((short)connectionParameters.Password.Length);
				num4 += (short)(connectionParameters.Password.Length * 2);
			}
			base.Comm.Append(num4);
			base.Comm.Append((short)connectionParameters.ApplicationName.Length);
			num4 += (short)(connectionParameters.ApplicationName.Length * 2);
			base.Comm.Append(num4);
			base.Comm.Append((short)base.DataSource.Length);
			num4 += (short)(base.DataSource.Length * 2);
			base.Comm.Append(num4);
			base.Comm.Append((short)0);
			base.Comm.Append(num4);
			base.Comm.Append((short)connectionParameters.LibraryName.Length);
			num4 += (short)(connectionParameters.LibraryName.Length * 2);
			base.Comm.Append(num4);
			base.Comm.Append((short)base.Language.Length);
			num4 += (short)(base.Language.Length * 2);
			base.Comm.Append(num4);
			base.Comm.Append((short)connectionParameters.Database.Length);
			num4 += (short)(connectionParameters.Database.Length * 2);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append(num4);
			if (connectionParameters.DomainLogin)
			{
				base.Comm.Append(num);
				num4 += num;
			}
			else
			{
				base.Comm.Append((short)0);
			}
			base.Comm.Append(num4);
			base.Comm.Append((short)connectionParameters.AttachDBFileName.Length);
			num4 += (short)(connectionParameters.AttachDBFileName.Length * 2);
			base.Comm.Append(connectionParameters.Hostname);
			if (!connectionParameters.DomainLogin)
			{
				base.Comm.Append(connectionParameters.User);
				string s = EncryptPassword(connectionParameters.Password);
				base.Comm.Append(s);
			}
			base.Comm.Append(connectionParameters.ApplicationName);
			base.Comm.Append(base.DataSource);
			base.Comm.Append(connectionParameters.LibraryName);
			base.Comm.Append(base.Language);
			base.Comm.Append(connectionParameters.Database);
			if (connectionParameters.DomainLogin)
			{
				Type1Message type1Message = new Type1Message();
				type1Message.Domain = text2;
				type1Message.Host = connectionParameters.Hostname;
				type1Message.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateDomainSupplied | NtlmFlags.NegotiateWorkstationSupplied | NtlmFlags.NegotiateAlwaysSign;
				base.Comm.Append(type1Message.GetBytes());
			}
			base.Comm.Append(connectionParameters.AttachDBFileName);
			base.Comm.SendPacket();
			base.MoreResults = true;
			SkipToEnd();
			return base.IsConnected;
		}

		private static string EncryptPassword(string pass)
		{
			int num = 23130;
			int length = pass.Length;
			char[] array = new char[length];
			for (int i = 0; i < length; i++)
			{
				int num2 = pass[i] ^ num;
				int num3 = (num2 >> 4) & 0xF0F;
				int num4 = (num2 << 4) & 0xF0F0;
				array[i] = (char)(num3 | num4);
			}
			return new string(array);
		}

		public override bool Reset()
		{
			if (!base.Comm.IsConnected())
			{
				return false;
			}
			base.Comm.ResetConnection = true;
			base.Reset();
			return true;
		}

		public override void ExecPrepared(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				ExecRPC(TdsRpcProcId.ExecuteSql, commandText, parameters, timeout, wantResults);
			}
			else
			{
				ExecuteQuery(BuildPreparedQuery(commandText), timeout, wantResults);
			}
		}

		public override void ExecProc(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			ExecRPC(commandText, parameters, timeout, wantResults);
		}

		private void WriteRpcParameterInfo(TdsMetaParameterCollection parameters)
		{
			if (parameters == null)
			{
				return;
			}
			foreach (TdsMetaParameter item in (IEnumerable)parameters)
			{
				if (item.Direction != TdsParameterDirection.ReturnValue)
				{
					string parameterName = item.ParameterName;
					if (parameterName != null && parameterName.Length > 0 && parameterName[0] == '@')
					{
						base.Comm.Append((byte)parameterName.Length);
						base.Comm.Append(parameterName);
					}
					else
					{
						base.Comm.Append((byte)(parameterName.Length + 1));
						base.Comm.Append("@" + parameterName);
					}
					short num = 0;
					if (item.Direction != TdsParameterDirection.Input)
					{
						num |= 1;
					}
					base.Comm.Append((byte)num);
					WriteParameterInfo(item);
				}
			}
		}

		private void WritePreparedParameterInfo(TdsMetaParameterCollection parameters)
		{
			if (parameters != null)
			{
				string text = BuildPreparedParameters();
				base.Comm.Append((byte)0);
				base.Comm.Append((byte)0);
				WriteParameterInfo(new TdsMetaParameter("prep_params", (text.Length <= 4000) ? "nvarchar" : "ntext", text));
			}
		}

		private void ExecRPC(TdsRpcProcId rpcId, string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			InitExec();
			base.Comm.StartPacket(TdsPacketType.Proc);
			base.Comm.Append(ushort.MaxValue);
			base.Comm.Append((ushort)rpcId);
			base.Comm.Append((short)2);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			TdsMetaParameter param = new TdsMetaParameter("sql", (sql.Length <= 4000) ? "nvarchar" : "ntext", sql);
			WriteParameterInfo(param);
			WritePreparedParameterInfo(parameters);
			WriteRpcParameterInfo(parameters);
			base.Comm.SendPacket();
			CheckForData(timeout);
			if (!wantResults)
			{
				SkipToEnd();
			}
		}

		protected override void ExecRPC(string rpcName, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			InitExec();
			base.Comm.StartPacket(TdsPacketType.Proc);
			base.Comm.Append((short)rpcName.Length);
			base.Comm.Append(rpcName);
			base.Comm.Append((short)0);
			WriteRpcParameterInfo(parameters);
			base.Comm.SendPacket();
			CheckForData(timeout);
			if (!wantResults)
			{
				SkipToEnd();
			}
		}

		private void WriteParameterInfo(TdsMetaParameter param)
		{
			param.IsNullable = true;
			TdsColumnType tdsColumnType = param.GetMetaType();
			param.IsNullable = false;
			bool flag = false;
			int num = param.Size;
			if (num < 1)
			{
				if (num < 0)
				{
					flag = true;
				}
				num = param.GetActualSize();
			}
			TdsColumnType tdsColumnType2 = tdsColumnType;
			switch (tdsColumnType)
			{
			case TdsColumnType.BigNVarChar:
				if (num == param.Size)
				{
					num <<= 1;
				}
				if (num >> 1 > 4000)
				{
					tdsColumnType = TdsColumnType.NText;
				}
				break;
			case TdsColumnType.BigVarChar:
				if (num > 8000)
				{
					tdsColumnType = TdsColumnType.Text;
				}
				break;
			case TdsColumnType.BigVarBinary:
				if (num > 8000)
				{
					tdsColumnType = TdsColumnType.Image;
				}
				break;
			}
			if (base.TdsVersion > TdsVersion.tds81 && flag)
			{
				base.Comm.Append((byte)tdsColumnType2);
				base.Comm.Append((short)(-1));
			}
			else if (base.ServerTdsVersion > TdsVersion.tds70 && tdsColumnType2 == TdsColumnType.Decimal)
			{
				base.Comm.Append((byte)108);
			}
			else
			{
				base.Comm.Append((byte)tdsColumnType);
			}
			if (IsLargeType(tdsColumnType))
			{
				base.Comm.Append((short)num);
			}
			else if (IsBlobType(tdsColumnType))
			{
				base.Comm.Append(num);
			}
			else
			{
				base.Comm.Append((byte)num);
			}
			if (param.TypeName == "decimal" || param.TypeName == "numeric")
			{
				base.Comm.Append((byte)((param.Precision == 0) ? 29 : param.Precision));
				base.Comm.Append(param.Scale);
				if (param.Value != null && param.Value != DBNull.Value && (decimal)param.Value != decimal.MaxValue && (decimal)param.Value != decimal.MinValue)
				{
					decimal num2 = new decimal(System.Math.Pow(10.0, (int)param.Scale));
					int num3 = (int)((decimal)param.Value * num2);
					param.Value = (decimal)num3;
				}
			}
			if (base.Collation != null && (tdsColumnType == TdsColumnType.BigChar || tdsColumnType == TdsColumnType.BigNVarChar || tdsColumnType == TdsColumnType.BigVarChar || tdsColumnType == TdsColumnType.NChar || tdsColumnType == TdsColumnType.NVarChar || tdsColumnType == TdsColumnType.Text || tdsColumnType == TdsColumnType.NText))
			{
				base.Comm.Append(base.Collation);
			}
			num = (((tdsColumnType != TdsColumnType.BigVarChar && tdsColumnType != TdsColumnType.BigNVarChar && tdsColumnType != TdsColumnType.BigVarBinary) || (param.Value != null && param.Value != DBNull.Value)) ? param.GetActualSize() : (-1));
			if (IsLargeType(tdsColumnType))
			{
				base.Comm.Append((short)num);
			}
			else if (IsBlobType(tdsColumnType))
			{
				base.Comm.Append(num);
			}
			else
			{
				base.Comm.Append((byte)num);
			}
			if (num <= 0)
			{
				return;
			}
			switch (param.TypeName)
			{
			case "money":
			{
				decimal num6 = (decimal)param.Value;
				int[] bits2 = decimal.GetBits(num6);
				if (num6 >= 0m)
				{
					base.Comm.Append(bits2[1]);
					base.Comm.Append(bits2[0]);
				}
				else
				{
					base.Comm.Append(~bits2[1]);
					base.Comm.Append(~bits2[0] + 1);
				}
				break;
			}
			case "smallmoney":
			{
				decimal num4 = (decimal)param.Value;
				if (num4 < SMALLMONEY_MIN || num4 > SMALLMONEY_MAX)
				{
					throw new OverflowException(string.Format(CultureInfo.InvariantCulture, "Value '{0}' is not valid for SmallMoney.  Must be between {1:N4} and {2:N4}.", num4, SMALLMONEY_MIN, SMALLMONEY_MAX));
				}
				int[] bits = decimal.GetBits(num4);
				int num5 = ((num4 > 0m) ? 1 : (-1));
				base.Comm.Append(num5 * bits[0]);
				break;
			}
			case "datetime":
				base.Comm.Append((DateTime)param.Value, 8);
				break;
			case "smalldatetime":
				base.Comm.Append((DateTime)param.Value, 4);
				break;
			case "varchar":
			case "nvarchar":
			case "char":
			case "nchar":
			case "text":
			case "ntext":
			{
				byte[] bytes = param.GetBytes();
				base.Comm.Append(bytes);
				break;
			}
			case "uniqueidentifier":
				base.Comm.Append(((Guid)param.Value).ToByteArray());
				break;
			default:
				base.Comm.Append(param.Value);
				break;
			}
		}

		public override void Execute(string commandText, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			base.Parameters = parameters;
			string sql = commandText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				ExecRPC(TdsRpcProcId.ExecuteSql, commandText, parameters, timeout, wantResults);
				return;
			}
			if (wantResults)
			{
				sql = BuildExec(commandText);
			}
			ExecuteQuery(sql, timeout, wantResults);
		}

		private string FormatParameter(TdsMetaParameter parameter)
		{
			string text = parameter.ParameterName;
			if (text[0] == '@')
			{
				text = text.Substring(1);
			}
			if (parameter.Direction == TdsParameterDirection.Output)
			{
				return string.Format("@{0}=@{0} output", text);
			}
			if (parameter.Value == null || parameter.Value == DBNull.Value)
			{
				return string.Format("@{0}=NULL", text);
			}
			string text2 = null;
			switch (parameter.TypeName)
			{
			case "smalldatetime":
			case "datetime":
			{
				DateTime dateTime = Convert.ToDateTime(parameter.Value);
				text2 = string.Format(base.Locale, "'{0:MMM dd yyyy hh:mm:ss.fff tt}'", dateTime);
				break;
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
			{
				object obj = parameter.Value;
				Type type = obj.GetType();
				if (type.IsEnum)
				{
					obj = Convert.ChangeType(obj, Type.GetTypeCode(type));
				}
				text2 = obj.ToString();
				break;
			}
			case "nvarchar":
			case "nchar":
				text2 = string.Format("N'{0}'", parameter.Value.ToString().Replace("'", "''"));
				break;
			case "uniqueidentifier":
				text2 = string.Format("'{0}'", ((Guid)parameter.Value).ToString(string.Empty));
				break;
			case "bit":
				text2 = ((parameter.Value.GetType() != typeof(bool)) ? parameter.Value.ToString() : ((!(bool)parameter.Value) ? "0x0" : "0x1"));
				break;
			case "image":
			case "binary":
			case "varbinary":
			{
				byte[] array = (byte[])parameter.Value;
				text2 = ((array.Length != 0) ? string.Format("0x{0}", BitConverter.ToString(array).Replace("-", string.Empty).ToLower()) : "0x");
				break;
			}
			default:
				text2 = string.Format("'{0}'", parameter.Value.ToString().Replace("'", "''"));
				break;
			}
			return "@" + text + "=" + text2;
		}

		public override string Prepare(string commandText, TdsMetaParameterCollection parameters)
		{
			base.Parameters = parameters;
			TdsMetaParameterCollection tdsMetaParameterCollection = new TdsMetaParameterCollection();
			TdsMetaParameter tdsMetaParameter = new TdsMetaParameter("@Handle", "int", null);
			tdsMetaParameter.Direction = TdsParameterDirection.Output;
			tdsMetaParameterCollection.Add(tdsMetaParameter);
			tdsMetaParameterCollection.Add(new TdsMetaParameter("@VarDecl", "nvarchar", BuildPreparedParameters()));
			tdsMetaParameterCollection.Add(new TdsMetaParameter("@Query", "nvarchar", commandText));
			ExecProc("sp_prepare", tdsMetaParameterCollection, 0, true);
			SkipToEnd();
			return base.OutputParameters[0].ToString();
		}

		protected override void ProcessColumnInfo()
		{
			int tdsShort = base.Comm.GetTdsShort();
			for (int i = 0; i < tdsShort; i++)
			{
				byte[] array = new byte[4];
				for (int j = 0; j < 4; j++)
				{
					array[j] = base.Comm.GetByte();
				}
				bool value = (array[2] & 1) > 0;
				bool flag = (array[2] & 0xC) > 0;
				bool value2 = (array[2] & 0x10) > 0;
				bool value3 = (array[2] & 0x10) > 0;
				TdsColumnType tdsColumnType = (TdsColumnType)(base.Comm.GetByte() & 0xFF);
				byte b = 0;
				if (IsLargeType(tdsColumnType))
				{
					b = (byte)tdsColumnType;
					if (tdsColumnType != TdsColumnType.NChar)
					{
						tdsColumnType -= 128;
					}
				}
				string baseTableName = null;
				int num;
				if (!IsBlobType(tdsColumnType))
				{
					num = (Tds.IsFixedSizeColumn(tdsColumnType) ? Tds.LookupBufferSize(tdsColumnType) : ((!IsLargeType((TdsColumnType)b)) ? (base.Comm.GetByte() & 0xFF) : base.Comm.GetTdsShort()));
				}
				else
				{
					num = base.Comm.GetTdsInt();
					baseTableName = base.Comm.GetString(base.Comm.GetTdsShort());
				}
				if (IsWideType(tdsColumnType))
				{
					num /= 2;
				}
				byte b2 = 0;
				byte b3 = 0;
				if (tdsColumnType == TdsColumnType.Decimal || tdsColumnType == TdsColumnType.Numeric)
				{
					b2 = base.Comm.GetByte();
					b3 = base.Comm.GetByte();
				}
				else
				{
					b2 = GetPrecision(tdsColumnType, num);
					b3 = GetScale(tdsColumnType, num);
				}
				string columnName = base.Comm.GetString(base.Comm.GetByte());
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = tdsColumnType;
				tdsDataColumn.ColumnName = columnName;
				tdsDataColumn.IsAutoIncrement = value2;
				tdsDataColumn.IsIdentity = value3;
				tdsDataColumn.ColumnSize = num;
				tdsDataColumn.NumericPrecision = b2;
				tdsDataColumn.NumericScale = b3;
				tdsDataColumn.IsReadOnly = !flag;
				tdsDataColumn.AllowDBNull = value;
				tdsDataColumn.BaseTableName = baseTableName;
				tdsDataColumn.DataTypeName = Enum.GetName(typeof(TdsColumnType), b);
			}
		}

		public override void Unprepare(string statementId)
		{
			TdsMetaParameterCollection tdsMetaParameterCollection = new TdsMetaParameterCollection();
			tdsMetaParameterCollection.Add(new TdsMetaParameter("@P1", "int", int.Parse(statementId)));
			ExecProc("sp_unprepare", tdsMetaParameterCollection, 0, false);
		}

		protected override bool IsValidRowCount(byte status, byte op)
		{
			if ((status & 0x10) == 0 || op == 193)
			{
				return false;
			}
			return true;
		}

		protected override void ProcessReturnStatus()
		{
			int tdsInt = base.Comm.GetTdsInt();
			if (base.Parameters == null)
			{
				return;
			}
			foreach (TdsMetaParameter item in (IEnumerable)base.Parameters)
			{
				if (item.Direction == TdsParameterDirection.ReturnValue)
				{
					item.Value = tdsInt;
					break;
				}
			}
		}

		private byte GetScale(TdsColumnType type, int columnSize)
		{
			switch (type)
			{
			case TdsColumnType.DateTime:
				return 3;
			case TdsColumnType.DateTime4:
				return 0;
			case TdsColumnType.DateTimeN:
				switch (columnSize)
				{
				case 4:
					return 0;
				case 8:
					return 3;
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Fixed scale not defined for column type '{0}' with size {1}.", type, columnSize));
				}
			default:
				return byte.MaxValue;
			}
		}

		private byte GetPrecision(TdsColumnType type, int columnSize)
		{
			switch (type)
			{
			case TdsColumnType.Binary:
				return byte.MaxValue;
			case TdsColumnType.Bit:
				return byte.MaxValue;
			case TdsColumnType.Char:
				return byte.MaxValue;
			case TdsColumnType.DateTime:
				return 23;
			case TdsColumnType.DateTime4:
				return 16;
			case TdsColumnType.DateTimeN:
				switch (columnSize)
				{
				case 4:
					return 16;
				case 8:
					return 23;
				}
				break;
			case TdsColumnType.Real:
				return 7;
			case TdsColumnType.Float8:
				return 15;
			case TdsColumnType.FloatN:
				switch (columnSize)
				{
				case 4:
					return 7;
				case 8:
					return 15;
				}
				break;
			case TdsColumnType.Image:
				return byte.MaxValue;
			case TdsColumnType.Int1:
				return 3;
			case TdsColumnType.Int2:
				return 5;
			case TdsColumnType.Int4:
				return 10;
			case TdsColumnType.IntN:
				switch (columnSize)
				{
				case 1:
					return 3;
				case 2:
					return 5;
				case 4:
					return 10;
				}
				break;
			case TdsColumnType.Void:
				return 1;
			case TdsColumnType.Text:
				return byte.MaxValue;
			case TdsColumnType.UniqueIdentifier:
				return byte.MaxValue;
			case TdsColumnType.VarBinary:
				return byte.MaxValue;
			case TdsColumnType.VarChar:
				return byte.MaxValue;
			case TdsColumnType.Money:
				return 19;
			case TdsColumnType.NText:
				return byte.MaxValue;
			case TdsColumnType.NVarChar:
				return byte.MaxValue;
			case TdsColumnType.BitN:
				return byte.MaxValue;
			case TdsColumnType.MoneyN:
				switch (columnSize)
				{
				case 4:
					return 10;
				case 8:
					return 19;
				}
				break;
			case TdsColumnType.Money4:
				return 10;
			case TdsColumnType.NChar:
				return byte.MaxValue;
			case TdsColumnType.BigBinary:
				return byte.MaxValue;
			case TdsColumnType.BigVarBinary:
				return byte.MaxValue;
			case TdsColumnType.BigVarChar:
				return byte.MaxValue;
			case TdsColumnType.BigNVarChar:
				return byte.MaxValue;
			case TdsColumnType.BigChar:
				return byte.MaxValue;
			case TdsColumnType.SmallMoney:
				return 10;
			case TdsColumnType.Variant:
				return byte.MaxValue;
			case TdsColumnType.BigInt:
				return byte.MaxValue;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Fixed precision not defined for column type '{0}' with size {1}.", type, columnSize));
		}

		public override IAsyncResult BeginExecuteNonQuery(string cmdText, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string sql = cmdText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				sql = BuildExec(cmdText);
			}
			return BeginExecuteQueryInternal(sql, false, callback, state);
		}

		public override void EndExecuteNonQuery(IAsyncResult ar)
		{
			EndExecuteQueryInternal(ar);
		}

		public override IAsyncResult BeginExecuteQuery(string cmdText, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string sql = cmdText;
			if (base.Parameters != null && base.Parameters.Count > 0)
			{
				sql = BuildExec(cmdText);
			}
			return BeginExecuteQueryInternal(sql, true, callback, state);
		}

		public override void EndExecuteQuery(IAsyncResult ar)
		{
			EndExecuteQueryInternal(ar);
		}

		public override IAsyncResult BeginExecuteProcedure(string prolog, string epilog, string cmdText, bool IsNonQuery, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			base.Parameters = parameters;
			string arg = BuildProcedureCall(cmdText);
			string sql = string.Format("{0};{1};{2};", prolog, arg, epilog);
			return BeginExecuteQueryInternal(sql, !IsNonQuery, callback, state);
		}

		public override void EndExecuteProcedure(IAsyncResult ar)
		{
			EndExecuteQueryInternal(ar);
		}
	}
}
