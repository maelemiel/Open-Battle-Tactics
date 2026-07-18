using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Data.Tds.Protocol
{
	public abstract class Tds
	{
		private TdsComm comm;

		private TdsVersion tdsVersion;

		protected internal TdsConnectionParameters connectionParms;

		protected readonly byte[] NTLMSSP_ID = new byte[8] { 78, 84, 76, 77, 83, 83, 80, 0 };

		private int packetSize;

		private string dataSource;

		private string database;

		private string originalDatabase = string.Empty;

		private string databaseProductName;

		private string databaseProductVersion;

		private int databaseMajorVersion;

		private CultureInfo locale = CultureInfo.InvariantCulture;

		private string charset;

		private string language;

		private bool connected;

		private bool moreResults;

		private Encoding encoder;

		private bool doneProc;

		private bool pooling = true;

		private TdsDataRow currentRow;

		private TdsDataColumnCollection columns;

		private ArrayList tableNames;

		private ArrayList columnNames;

		private TdsMetaParameterCollection parameters = new TdsMetaParameterCollection();

		private bool queryInProgress;

		private int cancelsRequested;

		private int cancelsProcessed;

		private ArrayList outputParameters = new ArrayList();

		protected TdsInternalErrorCollection messages = new TdsInternalErrorCollection();

		private int recordsAffected = -1;

		private long StreamLength;

		private long StreamIndex;

		private int StreamColumnIndex;

		private bool sequentialAccess;

		private bool isRowRead;

		private bool isResultRead;

		private bool LoadInProgress;

		private byte[] collation;

		internal int poolStatus;

		protected string Charset
		{
			get
			{
				return charset;
			}
		}

		protected CultureInfo Locale
		{
			get
			{
				return locale;
			}
		}

		public bool DoneProc
		{
			get
			{
				return doneProc;
			}
		}

		protected string Language
		{
			get
			{
				return language;
			}
		}

		protected ArrayList ColumnNames
		{
			get
			{
				return columnNames;
			}
		}

		public TdsDataRow ColumnValues
		{
			get
			{
				return currentRow;
			}
		}

		internal TdsComm Comm
		{
			get
			{
				return comm;
			}
		}

		public string Database
		{
			get
			{
				return database;
			}
		}

		public string DataSource
		{
			get
			{
				return dataSource;
			}
		}

		public bool IsConnected
		{
			get
			{
				return connected && comm != null && comm.IsConnected();
			}
			set
			{
				connected = value;
			}
		}

		public bool Pooling
		{
			get
			{
				return pooling;
			}
			set
			{
				pooling = value;
			}
		}

		public bool MoreResults
		{
			get
			{
				return moreResults;
			}
			set
			{
				moreResults = value;
			}
		}

		public int PacketSize
		{
			get
			{
				return packetSize;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return recordsAffected;
			}
			set
			{
				recordsAffected = value;
			}
		}

		public string ServerVersion
		{
			get
			{
				return databaseProductVersion;
			}
		}

		public TdsDataColumnCollection Columns
		{
			get
			{
				return columns;
			}
		}

		public TdsVersion TdsVersion
		{
			get
			{
				return tdsVersion;
			}
		}

		public ArrayList OutputParameters
		{
			get
			{
				return outputParameters;
			}
			set
			{
				outputParameters = value;
			}
		}

		protected TdsMetaParameterCollection Parameters
		{
			get
			{
				return parameters;
			}
			set
			{
				parameters = value;
			}
		}

		public bool SequentialAccess
		{
			get
			{
				return sequentialAccess;
			}
			set
			{
				sequentialAccess = value;
			}
		}

		public byte[] Collation
		{
			get
			{
				return collation;
			}
		}

		public TdsVersion ServerTdsVersion
		{
			get
			{
				switch (databaseMajorVersion)
				{
				case 4:
					return TdsVersion.tds42;
				case 5:
					return TdsVersion.tds50;
				case 7:
					return TdsVersion.tds70;
				case 8:
					return TdsVersion.tds80;
				case 9:
					return TdsVersion.tds90;
				case 10:
					return TdsVersion.tds100;
				default:
					return tdsVersion;
				}
			}
		}

		public event TdsInternalErrorMessageEventHandler TdsErrorMessage;

		public event TdsInternalInfoMessageEventHandler TdsInfoMessage;

		public Tds(string dataSource, int port, int packetSize, int timeout, TdsVersion tdsVersion)
		{
			this.tdsVersion = tdsVersion;
			this.packetSize = packetSize;
			this.dataSource = dataSource;
			columns = new TdsDataColumnCollection();
			comm = new TdsComm(dataSource, port, packetSize, timeout, tdsVersion);
		}

		private void SkipRow()
		{
			SkipToColumnIndex(Columns.Count);
			StreamLength = 0L;
			StreamColumnIndex = 0;
			StreamIndex = 0L;
			LoadInProgress = false;
		}

		private void SkipToColumnIndex(int colIndex)
		{
			if (LoadInProgress)
			{
				EndLoad();
			}
			if (colIndex < StreamColumnIndex)
			{
				throw new Exception("Cannot Skip to a colindex less than the curr index");
			}
			while (colIndex != StreamColumnIndex)
			{
				TdsColumnType? columnType = Columns[StreamColumnIndex].ColumnType;
				if (!columnType.HasValue)
				{
					throw new Exception("Column type unset.");
				}
				if (columnType != TdsColumnType.Image && columnType != TdsColumnType.Text && columnType != TdsColumnType.NText)
				{
					GetColumnValue(columnType, false, StreamColumnIndex);
					StreamColumnIndex++;
					continue;
				}
				BeginLoad(columnType);
				Comm.Skip(StreamLength);
				StreamLength = 0L;
				EndLoad();
			}
		}

		public object GetSequentialColumnValue(int colIndex)
		{
			if (colIndex < StreamColumnIndex)
			{
				throw new InvalidOperationException("Invalid attempt tp read from column ordinal" + colIndex);
			}
			if (LoadInProgress)
			{
				EndLoad();
			}
			if (colIndex != StreamColumnIndex)
			{
				SkipToColumnIndex(colIndex);
			}
			object columnValue = GetColumnValue(Columns[colIndex].ColumnType, false, colIndex);
			StreamColumnIndex++;
			return columnValue;
		}

		public long GetSequentialColumnValue(int colIndex, long fieldIndex, byte[] buffer, int bufferIndex, int size)
		{
			if (colIndex < StreamColumnIndex)
			{
				throw new InvalidOperationException("Invalid attempt to read from column ordinal" + colIndex);
			}
			try
			{
				if (colIndex != StreamColumnIndex)
				{
					SkipToColumnIndex(colIndex);
				}
				if (!LoadInProgress)
				{
					BeginLoad(Columns[colIndex].ColumnType);
				}
				if (buffer == null)
				{
					return StreamLength;
				}
				return LoadData(fieldIndex, buffer, bufferIndex, size);
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		private void BeginLoad(TdsColumnType? colType)
		{
			if (LoadInProgress)
			{
				EndLoad();
			}
			StreamLength = 0L;
			if (!colType.HasValue)
			{
				throw new ArgumentNullException("colType");
			}
			if (!colType.HasValue)
			{
				goto IL_014a;
			}
			switch (colType.Value)
			{
			case TdsColumnType.Image:
			case TdsColumnType.Text:
			case TdsColumnType.NText:
				break;
			case TdsColumnType.BigVarBinary:
			case TdsColumnType.BigVarChar:
			case TdsColumnType.BigBinary:
			case TdsColumnType.BigChar:
				goto IL_0110;
			case TdsColumnType.VarBinary:
			case TdsColumnType.VarChar:
			case TdsColumnType.Binary:
			case TdsColumnType.Char:
			case TdsColumnType.NVarChar:
			case TdsColumnType.NChar:
				goto IL_0133;
			default:
				goto IL_014a;
			}
			if (Comm.GetByte() != 0)
			{
				Comm.Skip(24L);
				StreamLength = Comm.GetTdsInt();
			}
			else
			{
				StreamLength = -2L;
			}
			goto IL_0157;
			IL_014a:
			StreamLength = -1L;
			goto IL_0157;
			IL_0110:
			Comm.GetTdsShort();
			StreamLength = Comm.GetTdsShort();
			goto IL_0157;
			IL_0157:
			StreamIndex = 0L;
			LoadInProgress = true;
			return;
			IL_0133:
			StreamLength = Comm.GetTdsShort();
			goto IL_0157;
		}

		private void EndLoad()
		{
			if (StreamLength > 0)
			{
				Comm.Skip(StreamLength);
			}
			StreamLength = 0L;
			StreamIndex = 0L;
			StreamColumnIndex++;
			LoadInProgress = false;
		}

		private long LoadData(long fieldIndex, byte[] buffer, int bufferIndex, int size)
		{
			if (StreamLength <= 0)
			{
				return StreamLength;
			}
			if (fieldIndex < StreamIndex)
			{
				throw new InvalidOperationException(string.Format("Attempting to read at dataIndex '{0}' is not allowed as this is less than the current position. You must read from dataIndex '{1}' or greater.", fieldIndex, StreamIndex));
			}
			if (fieldIndex >= StreamLength + StreamIndex)
			{
				return 0L;
			}
			int num = (int)(fieldIndex - StreamIndex);
			Comm.Skip(num);
			StreamIndex += fieldIndex - StreamIndex;
			StreamLength -= num;
			int num2 = (int)((size <= StreamLength) ? size : StreamLength);
			byte[] bytes = Comm.GetBytes(num2, true);
			StreamIndex += num2 + (fieldIndex - StreamIndex);
			StreamLength -= num2;
			bytes.CopyTo(buffer, bufferIndex);
			return bytes.Length;
		}

		protected internal void InitExec()
		{
			moreResults = true;
			doneProc = false;
			isResultRead = false;
			isRowRead = false;
			StreamLength = 0L;
			StreamIndex = 0L;
			StreamColumnIndex = 0;
			LoadInProgress = false;
			queryInProgress = false;
			cancelsRequested = 0;
			cancelsProcessed = 0;
			recordsAffected = -1;
			messages.Clear();
			outputParameters.Clear();
		}

		public void Cancel()
		{
			if (queryInProgress && cancelsRequested == cancelsProcessed)
			{
				comm.StartPacket(TdsPacketType.Cancel);
				try
				{
					Comm.SendPacket();
				}
				catch (IOException innerException)
				{
					connected = false;
					throw new TdsInternalException("Server closed the connection.", innerException);
				}
				cancelsRequested++;
			}
		}

		public abstract bool Connect(TdsConnectionParameters connectionParameters);

		public static TdsTimeoutException CreateTimeoutException(string dataSource, string method)
		{
			string message = "Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding.";
			return new TdsTimeoutException(0, 0, message, -2, method, dataSource, "Mono TdsClient Data Provider", 0);
		}

		public void Disconnect()
		{
			try
			{
				comm.StartPacket(TdsPacketType.Logoff);
				comm.Append((byte)0);
				comm.SendPacket();
			}
			catch
			{
			}
			connected = false;
			comm.Close();
		}

		public virtual bool Reset()
		{
			database = originalDatabase;
			return true;
		}

		protected virtual bool IsValidRowCount(byte status, byte op)
		{
			return (status & 0x10) != 0;
		}

		public void Execute(string sql)
		{
			Execute(sql, null, 0, false);
		}

		public void ExecProc(string sql)
		{
			ExecProc(sql, null, 0, false);
		}

		public virtual void Execute(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			ExecuteQuery(sql, timeout, wantResults);
		}

		public virtual void ExecProc(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			ExecuteQuery(string.Format("exec {0}", sql), timeout, wantResults);
		}

		public virtual void ExecPrepared(string sql, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			throw new NotSupportedException();
		}

		internal void ExecBulkCopyMetaData(int timeout, bool wantResults)
		{
			moreResults = true;
			try
			{
				Comm.SendPacket();
				CheckForData(timeout);
				if (!wantResults)
				{
					SkipToEnd();
				}
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		internal void ExecBulkCopy(int timeout, bool wantResults)
		{
			moreResults = true;
			try
			{
				Comm.SendPacket();
				CheckForData(timeout);
				if (!wantResults)
				{
					SkipToEnd();
				}
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		protected void ExecuteQuery(string sql, int timeout, bool wantResults)
		{
			InitExec();
			Comm.StartPacket(TdsPacketType.Query);
			Comm.Append(sql);
			try
			{
				Comm.SendPacket();
				CheckForData(timeout);
				if (!wantResults)
				{
					SkipToEnd();
				}
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		protected virtual void ExecRPC(string rpcName, TdsMetaParameterCollection parameters, int timeout, bool wantResults)
		{
			Comm.StartPacket(TdsPacketType.DBRPC);
			byte[] bytes = Comm.Encoder.GetBytes(rpcName);
			byte b = (byte)bytes.Length;
			ushort s = 0;
			ushort s2 = (ushort)(1 + b + 2);
			Comm.Append(s2);
			Comm.Append(b);
			Comm.Append(bytes);
			Comm.Append(s);
			try
			{
				Comm.SendPacket();
				CheckForData(timeout);
				if (!wantResults)
				{
					SkipToEnd();
				}
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		public bool NextResult()
		{
			if (SequentialAccess && isResultRead)
			{
				while (NextRow())
				{
				}
				isRowRead = false;
				isResultRead = false;
			}
			if (!moreResults)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			while (!flag)
			{
				TdsPacketSubType tdsPacketSubType = ProcessSubPacket();
				if (flag2)
				{
					moreResults = false;
					break;
				}
				switch (tdsPacketSubType)
				{
				case TdsPacketSubType.ColumnMetadata:
				case TdsPacketSubType.ColumnInfo:
				case TdsPacketSubType.RowFormat:
				{
					byte b = Comm.Peek();
					flag = b != 164;
					if (flag && doneProc && b == 209)
					{
						flag2 = true;
						flag = false;
					}
					break;
				}
				case TdsPacketSubType.TableName:
				{
					byte b = Comm.Peek();
					flag = b != 165;
					break;
				}
				case TdsPacketSubType.ColumnDetail:
					flag = true;
					break;
				default:
					flag = !moreResults;
					break;
				}
			}
			return moreResults;
		}

		public bool NextRow()
		{
			if (SequentialAccess && isRowRead)
			{
				SkipRow();
				isRowRead = false;
			}
			bool flag = false;
			bool result = false;
			do
			{
				switch (ProcessSubPacket())
				{
				case TdsPacketSubType.Row:
					result = true;
					flag = true;
					break;
				case TdsPacketSubType.Done:
				case TdsPacketSubType.DoneProc:
				case TdsPacketSubType.DoneInProc:
					result = false;
					flag = true;
					break;
				}
			}
			while (!flag);
			return result;
		}

		public virtual string Prepare(string sql, TdsMetaParameterCollection parameters)
		{
			throw new NotSupportedException();
		}

		public void SkipToEnd()
		{
			try
			{
				while (NextResult())
				{
				}
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		public virtual void Unprepare(string statementId)
		{
			throw new NotSupportedException();
		}

		[System.MonoTODO("Is cancel enough, or do we need to drop the connection?")]
		protected void CheckForData(int timeout)
		{
			if (timeout > 0 && !comm.Poll(timeout, SelectMode.SelectRead))
			{
				Cancel();
				throw CreateTimeoutException(dataSource, "CheckForData()");
			}
		}

		protected TdsInternalInfoMessageEventArgs CreateTdsInfoMessageEvent(TdsInternalErrorCollection errors)
		{
			return new TdsInternalInfoMessageEventArgs(errors);
		}

		protected TdsInternalErrorMessageEventArgs CreateTdsErrorMessageEvent(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			return new TdsInternalErrorMessageEventArgs(new TdsInternalError(theClass, lineNumber, message, number, procedure, server, source, state));
		}

		private Encoding GetEncodingFromColumnCollation(int lcid, int sortId)
		{
			if (sortId != 0)
			{
				return TdsCharset.GetEncodingFromSortOrder(sortId);
			}
			return TdsCharset.GetEncodingFromLCID(lcid);
		}

		protected object GetColumnValue(TdsColumnType? colType, bool outParam)
		{
			return GetColumnValue(colType, outParam, -1);
		}

		private object GetColumnValue(TdsColumnType? colType, bool outParam, int ordinal)
		{
			object obj = null;
			Encoding encoding = null;
			int lcid = 0;
			int sortId = 0;
			if (!colType.HasValue)
			{
				throw new ArgumentNullException("colType");
			}
			if (ordinal > -1 && tdsVersion > TdsVersion.tds70)
			{
				lcid = columns[ordinal].LCID.Value;
				sortId = columns[ordinal].SortOrder.Value;
			}
			if (colType.HasValue)
			{
				switch (colType.Value)
				{
				case TdsColumnType.IntN:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetIntValue(colType);
					goto IL_0632;
				case TdsColumnType.Int1:
				case TdsColumnType.Int2:
				case TdsColumnType.Int4:
				case TdsColumnType.BigInt:
					obj = GetIntValue(colType);
					goto IL_0632;
				case TdsColumnType.Image:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetImageValue();
					goto IL_0632;
				case TdsColumnType.Text:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetTextValue(false, encoding);
					goto IL_0632;
				case TdsColumnType.NText:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetTextValue(true, encoding);
					goto IL_0632;
				case TdsColumnType.VarChar:
				case TdsColumnType.Char:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetStringValue(colType, false, outParam, encoding);
					goto IL_0632;
				case TdsColumnType.BigVarBinary:
				{
					if (outParam)
					{
						comm.Skip(1L);
					}
					int num = comm.GetTdsShort();
					obj = comm.GetBytes(num, true);
					goto IL_0632;
				}
				case TdsColumnType.BigBinary:
					if (outParam)
					{
						comm.Skip(2L);
					}
					obj = GetBinaryValue();
					goto IL_0632;
				case TdsColumnType.BigVarChar:
				case TdsColumnType.BigChar:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(2L);
					}
					obj = GetStringValue(colType, false, outParam, encoding);
					goto IL_0632;
				case TdsColumnType.BigNVarChar:
				case TdsColumnType.NChar:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(2L);
					}
					obj = GetStringValue(colType, true, outParam, encoding);
					goto IL_0632;
				case TdsColumnType.NVarChar:
					encoding = GetEncodingFromColumnCollation(lcid, sortId);
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetStringValue(colType, true, outParam, encoding);
					goto IL_0632;
				case TdsColumnType.Real:
				case TdsColumnType.Float8:
					obj = GetFloatValue(colType);
					goto IL_0632;
				case TdsColumnType.FloatN:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetFloatValue(colType);
					goto IL_0632;
				case TdsColumnType.Money:
				case TdsColumnType.SmallMoney:
					obj = GetMoneyValue(colType);
					goto IL_0632;
				case TdsColumnType.MoneyN:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetMoneyValue(colType);
					goto IL_0632;
				case TdsColumnType.Decimal:
				case TdsColumnType.Numeric:
				{
					byte b;
					byte b2;
					if (outParam)
					{
						comm.Skip(1L);
						b = comm.GetByte();
						b2 = comm.GetByte();
					}
					else
					{
						b = (byte)columns[ordinal].NumericPrecision.Value;
						b2 = (byte)columns[ordinal].NumericScale.Value;
					}
					obj = GetDecimalValue(b, b2);
					if (b2 == 0 && b <= 19 && tdsVersion == TdsVersion.tds70 && !(obj is DBNull))
					{
						obj = Convert.ToInt64(obj);
					}
					goto IL_0632;
				}
				case TdsColumnType.DateTimeN:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetDateTimeValue(colType);
					goto IL_0632;
				case TdsColumnType.DateTime4:
				case TdsColumnType.DateTime:
					obj = GetDateTimeValue(colType);
					goto IL_0632;
				case TdsColumnType.VarBinary:
				case TdsColumnType.Binary:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = GetBinaryValue();
					goto IL_0632;
				case TdsColumnType.BitN:
					if (outParam)
					{
						comm.Skip(1L);
					}
					obj = ((comm.GetByte() != 0) ? ((object)(comm.GetByte() != 0)) : DBNull.Value);
					goto IL_0632;
				case TdsColumnType.Bit:
				{
					int num2 = comm.GetByte();
					obj = num2 != 0;
					goto IL_0632;
				}
				case TdsColumnType.UniqueIdentifier:
					{
						if (comm.Peek() != 16)
						{
							comm.GetByte();
							obj = DBNull.Value;
						}
						else
						{
							if (outParam)
							{
								comm.Skip(1L);
							}
							int num = comm.GetByte() & 0xFF;
							if (num > 0)
							{
								byte[] bytes = comm.GetBytes(num, true);
								if (!BitConverter.IsLittleEndian)
								{
									byte[] array = new byte[num];
									for (int i = 0; i < 4; i++)
									{
										array[i] = bytes[4 - i - 1];
									}
									for (int j = 4; j < 6; j++)
									{
										array[j] = bytes[6 - (j - 4) - 1];
									}
									for (int k = 6; k < 8; k++)
									{
										array[k] = bytes[8 - (k - 6) - 1];
									}
									for (int l = 8; l < 16; l++)
									{
										array[l] = bytes[l];
									}
									Array.Copy(array, 0, bytes, 0, num);
								}
								obj = new Guid(bytes);
							}
						}
						goto IL_0632;
					}
					IL_0632:
					return obj;
				}
			}
			return DBNull.Value;
		}

		private object GetBinaryValue()
		{
			object result = DBNull.Value;
			if (tdsVersion >= TdsVersion.tds70)
			{
				int tdsShort = comm.GetTdsShort();
				if (tdsShort != 65535 && tdsShort >= 0)
				{
					result = comm.GetBytes(tdsShort, true);
				}
			}
			else
			{
				int tdsShort = comm.GetByte() & 0xFF;
				if (tdsShort != 0)
				{
					result = comm.GetBytes(tdsShort, true);
				}
			}
			return result;
		}

		private object GetDateTimeValue(TdsColumnType? type)
		{
			int num = 0;
			if (!type.HasValue)
			{
				throw new ArgumentNullException("type");
			}
			if (type.HasValue)
			{
				switch (type.Value)
				{
				case TdsColumnType.DateTime4:
					num = 4;
					break;
				case TdsColumnType.DateTime:
					num = 8;
					break;
				case TdsColumnType.DateTimeN:
				{
					byte b = comm.Peek();
					if (b == 0 || b == 4 || b == 8)
					{
						num = comm.GetByte();
					}
					break;
				}
				}
			}
			DateTime dateTime = new DateTime(1900, 1, 1);
			object obj;
			switch (num)
			{
			case 8:
			{
				obj = dateTime.AddDays(comm.GetTdsInt());
				int tdsInt = comm.GetTdsInt();
				long num2 = (long)System.Math.Round((float)((long)tdsInt % 300L * 1000) / 300f);
				if (tdsInt != 0 || num2 != 0L)
				{
					obj = ((DateTime)obj).AddSeconds(tdsInt / 300);
					obj = ((DateTime)obj).AddMilliseconds(num2);
				}
				break;
			}
			case 4:
			{
				obj = dateTime.AddDays((int)(ushort)comm.GetTdsShort());
				short tdsShort = comm.GetTdsShort();
				if (tdsShort != 0)
				{
					obj = ((DateTime)obj).AddMinutes(tdsShort);
				}
				break;
			}
			default:
				obj = DBNull.Value;
				break;
			}
			return obj;
		}

		private object GetDecimalValue(byte precision, byte scale)
		{
			if (tdsVersion < TdsVersion.tds70)
			{
				return GetDecimalValueTds50(precision, scale);
			}
			return GetDecimalValueTds70(precision, scale);
		}

		private object GetDecimalValueTds70(byte precision, byte scale)
		{
			int[] array = new int[4];
			int num = (comm.GetByte() & 0xFF) - 1;
			if (num < 0)
			{
				return DBNull.Value;
			}
			bool flag = comm.GetByte() == 1;
			if (num > 16)
			{
				throw new OverflowException();
			}
			int num2 = 0;
			int num3 = 0;
			while (num2 < num && num2 < 16)
			{
				array[num3] = comm.GetTdsInt();
				num2 += 4;
				num3++;
			}
			if (array[3] != 0)
			{
				return new TdsBigDecimal(precision, scale, !flag, array);
			}
			return new decimal(array[0], array[1], array[2], !flag, scale);
		}

		private object GetDecimalValueTds50(byte precision, byte scale)
		{
			int[] array = new int[4];
			int num = comm.GetByte() & 0xFF;
			if (num == 0)
			{
				return DBNull.Value;
			}
			byte[] bytes = comm.GetBytes(num, false);
			byte[] array2 = new byte[4];
			bool isNegative = bytes[0] == 1;
			if (num > 17)
			{
				throw new OverflowException();
			}
			int num2 = 1;
			int num3 = 0;
			while (num2 < num && num2 < 16)
			{
				for (int i = 0; i < 4; i++)
				{
					if (num2 + i < num)
					{
						array2[i] = bytes[num - (num2 + i)];
					}
					else
					{
						array2[i] = 0;
					}
				}
				if (!BitConverter.IsLittleEndian)
				{
					array2 = comm.Swap(array2);
				}
				array[num3] = BitConverter.ToInt32(array2, 0);
				num2 += 4;
				num3++;
			}
			if (array[3] != 0)
			{
				return new TdsBigDecimal(precision, scale, isNegative, array);
			}
			return new decimal(array[0], array[1], array[2], isNegative, scale);
		}

		private object GetFloatValue(TdsColumnType? columnType)
		{
			if (!columnType.HasValue)
			{
				throw new ArgumentNullException("columnType");
			}
			int num = 0;
			if (columnType.HasValue)
			{
				switch (columnType.Value)
				{
				case TdsColumnType.Real:
					num = 4;
					break;
				case TdsColumnType.Float8:
					num = 8;
					break;
				case TdsColumnType.FloatN:
					num = comm.GetByte();
					break;
				}
			}
			switch (num)
			{
			case 8:
				return BitConverter.Int64BitsToDouble(comm.GetTdsInt64());
			case 4:
				return BitConverter.ToSingle(BitConverter.GetBytes(comm.GetTdsInt()), 0);
			default:
				return DBNull.Value;
			}
		}

		private object GetImageValue()
		{
			if (comm.GetByte() == 0)
			{
				return DBNull.Value;
			}
			comm.Skip(24L);
			int tdsInt = comm.GetTdsInt();
			if (tdsInt < 0)
			{
				return DBNull.Value;
			}
			return comm.GetBytes(tdsInt, true);
		}

		private object GetIntValue(TdsColumnType? type)
		{
			if (!type.HasValue)
			{
				throw new ArgumentNullException("type");
			}
			if (type.HasValue)
			{
				int num;
				switch (type.Value)
				{
				case TdsColumnType.BigInt:
					num = 8;
					goto IL_008e;
				case TdsColumnType.IntN:
					num = comm.GetByte();
					goto IL_008e;
				case TdsColumnType.Int4:
					num = 4;
					goto IL_008e;
				case TdsColumnType.Int2:
					num = 2;
					goto IL_008e;
				case TdsColumnType.Int1:
					{
						num = 1;
						goto IL_008e;
					}
					IL_008e:
					switch (num)
					{
					case 8:
						return comm.GetTdsInt64();
					case 4:
						return comm.GetTdsInt();
					case 2:
						return comm.GetTdsShort();
					case 1:
						return comm.GetByte();
					default:
						return DBNull.Value;
					}
				}
			}
			return DBNull.Value;
		}

		private object GetMoneyValue(TdsColumnType? type)
		{
			if (!type.HasValue)
			{
				throw new ArgumentNullException("type");
			}
			if (type.HasValue)
			{
				int num;
				switch (type.Value)
				{
				case TdsColumnType.Money4:
				case TdsColumnType.SmallMoney:
					num = 4;
					goto IL_0081;
				case TdsColumnType.Money:
					num = 8;
					goto IL_0081;
				case TdsColumnType.MoneyN:
					{
						num = comm.GetByte();
						goto IL_0081;
					}
					IL_0081:
					switch (num)
					{
					case 4:
					{
						int num4 = Comm.GetTdsInt();
						bool flag2 = num4 < 0;
						if (flag2)
						{
							num4 = ~(num4 - 1);
						}
						return new decimal(num4, 0, 0, flag2, 4);
					}
					case 8:
					{
						int num2 = Comm.GetTdsInt();
						int num3 = Comm.GetTdsInt();
						bool flag = num2 < 0;
						if (flag)
						{
							num2 = ~num2;
							num3 = ~(num3 - 1);
						}
						return new decimal(num3, num2, 0, flag, 4);
					}
					default:
						return DBNull.Value;
					}
				}
			}
			return DBNull.Value;
		}

		protected object GetStringValue(TdsColumnType? colType, bool wideChars, bool outputParam, Encoding encoder)
		{
			bool flag = false;
			Encoding enc = encoder;
			if (tdsVersion > TdsVersion.tds70 && outputParam && (colType == TdsColumnType.BigChar || colType == TdsColumnType.BigNVarChar || colType == TdsColumnType.BigVarChar || colType == TdsColumnType.NChar || colType == TdsColumnType.NVarChar))
			{
				byte[] bytes = Comm.GetBytes(5, true);
				enc = TdsCharset.GetEncoding(bytes);
				flag = true;
			}
			else
			{
				flag = tdsVersion >= TdsVersion.tds70 && (wideChars || !outputParam);
			}
			int len = ((!flag) ? (comm.GetByte() & 0xFF) : comm.GetTdsShort());
			return GetStringValue(wideChars, len, enc);
		}

		protected object GetStringValue(bool wideChars, int len, Encoding enc)
		{
			if (tdsVersion < TdsVersion.tds70 && len == 0)
			{
				return DBNull.Value;
			}
			if (len >= 0)
			{
				object obj = ((!wideChars) ? comm.GetString(len, false, enc) : comm.GetString(len / 2, enc));
				if (tdsVersion < TdsVersion.tds70 && ((string)obj).Equals(" "))
				{
					obj = string.Empty;
				}
				return obj;
			}
			return DBNull.Value;
		}

		protected int GetSubPacketLength()
		{
			return comm.GetTdsShort();
		}

		private object GetTextValue(bool wideChars, Encoding encoder)
		{
			string text = null;
			byte b = comm.GetByte();
			if (b != 16)
			{
				return DBNull.Value;
			}
			comm.Skip(24L);
			int tdsInt = comm.GetTdsInt();
			if (tdsInt == 0)
			{
				return string.Empty;
			}
			text = ((!wideChars) ? comm.GetString(tdsInt, false, encoder) : comm.GetString(tdsInt / 2, encoder));
			tdsInt /= 2;
			if ((byte)tdsVersion < 70 && text == " ")
			{
				text = string.Empty;
			}
			return text;
		}

		internal bool IsBlobType(TdsColumnType columnType)
		{
			return columnType == TdsColumnType.Text || columnType == TdsColumnType.Image || columnType == TdsColumnType.NText;
		}

		internal bool IsLargeType(TdsColumnType columnType)
		{
			return (byte)columnType > 128;
		}

		protected bool IsWideType(TdsColumnType columnType)
		{
			if (columnType == TdsColumnType.NText || columnType == TdsColumnType.NVarChar || columnType == TdsColumnType.NChar)
			{
				return true;
			}
			return false;
		}

		internal static bool IsFixedSizeColumn(TdsColumnType columnType)
		{
			switch (columnType)
			{
			case TdsColumnType.Int1:
			case TdsColumnType.Bit:
			case TdsColumnType.Int2:
			case TdsColumnType.Int4:
			case TdsColumnType.DateTime4:
			case TdsColumnType.Real:
			case TdsColumnType.Money:
			case TdsColumnType.DateTime:
			case TdsColumnType.Float8:
			case TdsColumnType.Money4:
			case TdsColumnType.SmallMoney:
			case TdsColumnType.BigInt:
				return true;
			default:
				return false;
			}
		}

		protected void LoadRow()
		{
			if (SequentialAccess)
			{
				if (isRowRead)
				{
					SkipRow();
				}
				isRowRead = true;
				isResultRead = true;
				return;
			}
			currentRow = new TdsDataRow();
			int num = 0;
			foreach (TdsDataColumn column in columns)
			{
				object columnValue = GetColumnValue(column.ColumnType, false, num);
				currentRow.Add(columnValue);
				if (doneProc)
				{
					outputParameters.Add(columnValue);
				}
				if (columnValue is TdsBigDecimal && currentRow.BigDecimalIndex < 0)
				{
					currentRow.BigDecimalIndex = num;
				}
				num++;
			}
		}

		internal static int LookupBufferSize(TdsColumnType columnType)
		{
			switch (columnType)
			{
			case TdsColumnType.Int1:
			case TdsColumnType.Bit:
				return 1;
			case TdsColumnType.Int2:
				return 2;
			case TdsColumnType.Int4:
			case TdsColumnType.DateTime4:
			case TdsColumnType.Real:
			case TdsColumnType.Money4:
			case TdsColumnType.SmallMoney:
				return 4;
			case TdsColumnType.Money:
			case TdsColumnType.DateTime:
			case TdsColumnType.Float8:
			case TdsColumnType.BigInt:
				return 8;
			default:
				return 0;
			}
		}

		protected internal int ProcessAuthentication()
		{
			int tdsShort = Comm.GetTdsShort();
			byte[] bytes = Comm.GetBytes(tdsShort, true);
			Type2Message type2Message = new Type2Message(bytes);
			Type3Message type3Message = new Type3Message();
			type3Message.Challenge = type2Message.Nonce;
			type3Message.Domain = connectionParms.DefaultDomain;
			type3Message.Host = connectionParms.Hostname;
			type3Message.Username = connectionParms.User;
			type3Message.Password = connectionParms.Password;
			Comm.StartPacket(TdsPacketType.SspAuth);
			Comm.Append(type3Message.GetBytes());
			try
			{
				Comm.SendPacket();
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
			return 1;
		}

		protected void ProcessColumnDetail()
		{
			int subPacketLength = GetSubPacketLength();
			byte[] array = new byte[3];
			string text = string.Empty;
			int num = 0;
			while (num < subPacketLength)
			{
				for (int i = 0; i < 3; i++)
				{
					array[i] = comm.GetByte();
				}
				num += 3;
				bool flag = (array[2] & 0x20) != 0;
				if (flag)
				{
					int num2;
					if (tdsVersion >= TdsVersion.tds70)
					{
						num2 = comm.GetByte();
						num += 2 * num2 + 1;
					}
					else
					{
						num2 = comm.GetByte();
						num += num2 + 1;
					}
					text = comm.GetString(num2);
				}
				byte index = (byte)(array[0] - 1);
				byte index2 = (byte)(array[1] - 1);
				bool flag2 = (array[2] & 4) != 0;
				TdsDataColumn tdsDataColumn = columns[index];
				tdsDataColumn.IsHidden = (array[2] & 0x10) != 0;
				tdsDataColumn.IsExpression = flag2;
				tdsDataColumn.IsKey = (array[2] & 8) != 0;
				tdsDataColumn.IsAliased = flag;
				tdsDataColumn.BaseColumnName = ((!flag) ? null : text);
				tdsDataColumn.BaseTableName = (flag2 ? null : ((string)tableNames[index2]));
			}
		}

		protected abstract void ProcessColumnInfo();

		protected void ProcessColumnNames()
		{
			columnNames = new ArrayList();
			int tdsShort = comm.GetTdsShort();
			int num = 0;
			int num2 = 0;
			while (num < tdsShort)
			{
				int num3 = comm.GetByte();
				string value = comm.GetString(num3);
				num = num + 1 + num3;
				columnNames.Add(value);
				num2++;
			}
		}

		[System.MonoTODO("Make sure counting works right, especially with multiple resultsets.")]
		protected void ProcessEndToken(TdsPacketSubType type)
		{
			byte b = Comm.GetByte();
			Comm.Skip(1L);
			byte op = comm.GetByte();
			Comm.Skip(1L);
			int tdsInt = comm.GetTdsInt();
			bool flag = IsValidRowCount(b, op);
			moreResults = (b & 1) != 0;
			bool flag2 = (b & 0x20) != 0;
			switch (type)
			{
			case TdsPacketSubType.DoneProc:
				doneProc = true;
				goto case TdsPacketSubType.Done;
			case TdsPacketSubType.Done:
			case TdsPacketSubType.DoneInProc:
				if (flag)
				{
					if (recordsAffected == -1)
					{
						recordsAffected = tdsInt;
					}
					else
					{
						recordsAffected += tdsInt;
					}
				}
				break;
			}
			if (moreResults)
			{
				queryInProgress = false;
			}
			if (flag2)
			{
				cancelsProcessed++;
			}
			if (messages.Count > 0 && !moreResults)
			{
				OnTdsInfoMessage(CreateTdsInfoMessageEvent(messages));
			}
		}

		protected void ProcessEnvironmentChange()
		{
			int subPacketLength = GetSubPacketLength();
			switch ((TdsEnvPacketSubType)comm.GetByte())
			{
			case TdsEnvPacketSubType.BlockSize:
			{
				int len = comm.GetByte();
				string s = comm.GetString(len);
				if (tdsVersion >= TdsVersion.tds70)
				{
					comm.Skip(subPacketLength - 2 - len * 2);
				}
				else
				{
					comm.Skip(subPacketLength - 2 - len);
				}
				packetSize = int.Parse(s);
				comm.ResizeOutBuf(packetSize);
				break;
			}
			case TdsEnvPacketSubType.CharSet:
			{
				int len = comm.GetByte();
				if (tdsVersion == TdsVersion.tds70)
				{
					SetCharset(comm.GetString(len));
					comm.Skip(subPacketLength - 2 - len * 2);
				}
				else
				{
					SetCharset(comm.GetString(len));
					comm.Skip(subPacketLength - 2 - len);
				}
				break;
			}
			case TdsEnvPacketSubType.Locale:
			{
				int len = comm.GetByte();
				int culture = 0;
				if (tdsVersion >= TdsVersion.tds70)
				{
					culture = (int)Convert.ChangeType(comm.GetString(len), typeof(int));
					comm.Skip(subPacketLength - 2 - len * 2);
				}
				else
				{
					culture = (int)Convert.ChangeType(comm.GetString(len), typeof(int));
					comm.Skip(subPacketLength - 2 - len);
				}
				locale = new CultureInfo(culture);
				break;
			}
			case TdsEnvPacketSubType.Database:
			{
				int len = comm.GetByte();
				string text = comm.GetString(len);
				len = comm.GetByte() & 0xFF;
				comm.GetString(len);
				if (originalDatabase == string.Empty)
				{
					originalDatabase = text;
				}
				database = text;
				break;
			}
			case TdsEnvPacketSubType.CollationInfo:
			{
				int len = comm.GetByte();
				collation = comm.GetBytes(len, true);
				int culture = TdsCollation.LCID(collation);
				locale = new CultureInfo(culture);
				SetCharset(TdsCharset.GetEncoding(collation));
				break;
			}
			default:
				comm.Skip(subPacketLength - 1);
				break;
			}
		}

		protected void ProcessLoginAck()
		{
			uint num = 0u;
			GetSubPacketLength();
			if (tdsVersion >= TdsVersion.tds70)
			{
				comm.Skip(1L);
				switch ((uint)comm.GetTdsInt())
				{
				case 117440512u:
					tdsVersion = TdsVersion.tds70;
					break;
				case 117506048u:
					tdsVersion = TdsVersion.tds80;
					break;
				case 1895825409u:
					tdsVersion = TdsVersion.tds81;
					break;
				case 1913192450u:
					tdsVersion = TdsVersion.tds90;
					break;
				}
			}
			if (tdsVersion >= TdsVersion.tds70)
			{
				int len = comm.GetByte();
				databaseProductName = comm.GetString(len);
				databaseMajorVersion = comm.GetByte();
				databaseProductVersion = string.Format("{0}.{1}.{2}", databaseMajorVersion.ToString("00"), comm.GetByte().ToString("00"), (256 * comm.GetByte() + comm.GetByte()).ToString("0000"));
			}
			else
			{
				comm.Skip(5L);
				short len2 = comm.GetByte();
				databaseProductName = comm.GetString(len2);
				comm.Skip(1L);
				databaseMajorVersion = comm.GetByte();
				databaseProductVersion = string.Format("{0}.{1}", databaseMajorVersion, comm.GetByte());
				comm.Skip(1L);
			}
			if (databaseProductName.Length > 1 && databaseProductName.IndexOf('\0') != -1)
			{
				int length = databaseProductName.IndexOf('\0');
				databaseProductName = databaseProductName.Substring(0, length);
			}
			connected = true;
		}

		protected void OnTdsErrorMessage(TdsInternalErrorMessageEventArgs e)
		{
			if (this.TdsErrorMessage != null)
			{
				this.TdsErrorMessage(this, e);
			}
		}

		protected void OnTdsInfoMessage(TdsInternalInfoMessageEventArgs e)
		{
			if (this.TdsInfoMessage != null)
			{
				this.TdsInfoMessage(this, e);
			}
			messages.Clear();
		}

		protected void ProcessMessage(TdsPacketSubType subType)
		{
			GetSubPacketLength();
			int tdsInt = comm.GetTdsInt();
			byte state = comm.GetByte();
			byte b = comm.GetByte();
			bool flag = false;
			if (subType == TdsPacketSubType.EED)
			{
				flag = b > 10;
				comm.Skip((int)comm.GetByte());
				comm.Skip(1L);
				comm.Skip(2L);
			}
			else
			{
				flag = subType == TdsPacketSubType.Error;
			}
			string message = comm.GetString(comm.GetTdsShort());
			string server = comm.GetString(comm.GetByte());
			string procedure = comm.GetString(comm.GetByte());
			byte lineNumber = comm.GetByte();
			comm.Skip(1L);
			string empty = string.Empty;
			if (flag)
			{
				OnTdsErrorMessage(CreateTdsErrorMessageEvent(b, lineNumber, message, tdsInt, procedure, server, empty, state));
			}
			else
			{
				messages.Add(new TdsInternalError(b, lineNumber, message, tdsInt, procedure, server, empty, state));
			}
		}

		protected virtual void ProcessOutputParam()
		{
			GetSubPacketLength();
			comm.GetString(comm.GetByte() & 0xFF);
			comm.Skip(5L);
			TdsColumnType value = (TdsColumnType)comm.GetByte();
			object columnValue = GetColumnValue(value, true);
			outputParameters.Add(columnValue);
		}

		protected void ProcessDynamic()
		{
			Comm.Skip(2L);
			Comm.GetByte();
			Comm.GetByte();
			Comm.GetString(Comm.GetByte());
		}

		protected virtual TdsPacketSubType ProcessSubPacket()
		{
			TdsPacketSubType tdsPacketSubType = (TdsPacketSubType)comm.GetByte();
			switch (tdsPacketSubType)
			{
			case TdsPacketSubType.Dynamic2:
				comm.Skip(comm.GetTdsInt());
				break;
			case TdsPacketSubType.AltName:
			case TdsPacketSubType.AltFormat:
			case TdsPacketSubType.Capability:
			case TdsPacketSubType.ParamFormat:
				comm.Skip(comm.GetTdsShort());
				break;
			case TdsPacketSubType.Dynamic:
				ProcessDynamic();
				break;
			case TdsPacketSubType.EnvironmentChange:
				ProcessEnvironmentChange();
				break;
			case TdsPacketSubType.Error:
			case TdsPacketSubType.Info:
			case TdsPacketSubType.EED:
				ProcessMessage(tdsPacketSubType);
				break;
			case TdsPacketSubType.Param:
				ProcessOutputParam();
				break;
			case TdsPacketSubType.LoginAck:
				ProcessLoginAck();
				break;
			case TdsPacketSubType.Authentication:
				ProcessAuthentication();
				break;
			case TdsPacketSubType.ReturnStatus:
				ProcessReturnStatus();
				break;
			case TdsPacketSubType.ProcId:
				Comm.Skip(8L);
				break;
			case TdsPacketSubType.Done:
			case TdsPacketSubType.DoneProc:
			case TdsPacketSubType.DoneInProc:
				ProcessEndToken(tdsPacketSubType);
				break;
			case TdsPacketSubType.ColumnName:
				Comm.Skip(8L);
				ProcessColumnNames();
				break;
			case TdsPacketSubType.ColumnMetadata:
			case TdsPacketSubType.ColumnInfo:
			case TdsPacketSubType.RowFormat:
				Columns.Clear();
				ProcessColumnInfo();
				break;
			case TdsPacketSubType.ColumnDetail:
				ProcessColumnDetail();
				break;
			case TdsPacketSubType.TableName:
				ProcessTableName();
				break;
			case TdsPacketSubType.ColumnOrder:
				comm.Skip(comm.GetTdsShort());
				break;
			case TdsPacketSubType.Control:
				comm.Skip(comm.GetTdsShort());
				break;
			case TdsPacketSubType.Row:
				LoadRow();
				break;
			}
			return tdsPacketSubType;
		}

		protected void ProcessTableName()
		{
			tableNames = new ArrayList();
			int tdsShort = comm.GetTdsShort();
			int num = 0;
			while (num < tdsShort)
			{
				int num2;
				if (tdsVersion >= TdsVersion.tds70)
				{
					num2 = comm.GetTdsShort();
					num += 2 * (num2 + 1);
				}
				else
				{
					num2 = comm.GetByte();
					num += num2 + 1;
				}
				tableNames.Add(comm.GetString(num2));
			}
		}

		protected void SetCharset(Encoding encoder)
		{
			comm.Encoder = encoder;
		}

		protected void SetCharset(string charset)
		{
			if (charset == null || charset.Length > 30)
			{
				charset = "iso_1";
			}
			if (this.charset == null || !(this.charset == charset))
			{
				if (charset.StartsWith("cp"))
				{
					encoder = Encoding.GetEncoding(int.Parse(charset.Substring(2)));
					this.charset = charset;
				}
				else
				{
					encoder = Encoding.GetEncoding("iso-8859-1");
					this.charset = "iso_1";
				}
				SetCharset(encoder);
			}
		}

		protected void SetLanguage(string language)
		{
			if (language == null || language.Length > 30)
			{
				language = "us_english";
			}
			this.language = language;
		}

		protected virtual void ProcessReturnStatus()
		{
			comm.Skip(4L);
		}

		protected IAsyncResult BeginExecuteQueryInternal(string sql, bool wantResults, AsyncCallback callback, object state)
		{
			InitExec();
			TdsAsyncResult tdsAsyncResult = new TdsAsyncResult(callback, state);
			tdsAsyncResult.TdsAsyncState.WantResults = wantResults;
			Comm.StartPacket(TdsPacketType.Query);
			Comm.Append(sql);
			try
			{
				Comm.SendPacket();
				Comm.BeginReadPacket(OnBeginExecuteQueryCallback, tdsAsyncResult);
				return tdsAsyncResult;
			}
			catch (IOException innerException)
			{
				connected = false;
				throw new TdsInternalException("Server closed the connection.", innerException);
			}
		}

		protected void EndExecuteQueryInternal(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar;
			if (tdsAsyncResult.IsCompletedWithException)
			{
				throw tdsAsyncResult.Exception;
			}
		}

		protected void OnBeginExecuteQueryCallback(IAsyncResult ar)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar.AsyncState;
			TdsAsyncState tdsAsyncState = tdsAsyncResult.TdsAsyncState;
			try
			{
				Comm.EndReadPacket(ar);
				if (!tdsAsyncState.WantResults)
				{
					SkipToEnd();
				}
			}
			catch (Exception e)
			{
				tdsAsyncResult.MarkComplete(e);
				return;
			}
			tdsAsyncResult.MarkComplete();
		}

		public virtual IAsyncResult BeginExecuteNonQuery(string sql, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteNonQuery(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual IAsyncResult BeginExecuteQuery(string sql, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteQuery(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual IAsyncResult BeginExecuteProcedure(string prolog, string epilog, string cmdText, bool IsNonQuery, TdsMetaParameterCollection parameters, AsyncCallback callback, object state)
		{
			throw new NotImplementedException("should not be called!");
		}

		public virtual void EndExecuteProcedure(IAsyncResult ar)
		{
			throw new NotImplementedException("should not be called!");
		}

		public void WaitFor(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
		}

		public void CheckAndThrowException(IAsyncResult ar)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)ar;
			if (tdsAsyncResult.IsCompleted && tdsAsyncResult.IsCompletedWithException)
			{
				throw tdsAsyncResult.Exception;
			}
		}
	}
}
