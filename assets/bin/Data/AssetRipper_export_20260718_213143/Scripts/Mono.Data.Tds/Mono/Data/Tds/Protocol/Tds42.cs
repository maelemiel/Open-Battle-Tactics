using System;

namespace Mono.Data.Tds.Protocol
{
	public sealed class Tds42 : Tds
	{
		public static readonly TdsVersion Version = TdsVersion.tds42;

		public Tds42(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds42(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, Version)
		{
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			SetCharset(connectionParameters.Charset);
			SetLanguage(connectionParameters.Language);
			byte pad = 0;
			byte[] b = new byte[0];
			base.Comm.StartPacket(TdsPacketType.Logon);
			byte[] array = base.Comm.Append(connectionParameters.Hostname, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(connectionParameters.User, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(connectionParameters.Password, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append("00000116", 8, pad);
			base.Comm.Append(b, 16, pad);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)160);
			base.Comm.Append((byte)36);
			base.Comm.Append((byte)204);
			base.Comm.Append((byte)80);
			base.Comm.Append((byte)18);
			base.Comm.Append((byte)8);
			base.Comm.Append((byte)3);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)6);
			base.Comm.Append((byte)10);
			base.Comm.Append((byte)9);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)1);
			base.Comm.Append((byte)0);
			base.Comm.Append((byte)0);
			base.Comm.Append(b, 7, pad);
			array = base.Comm.Append(connectionParameters.ApplicationName, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			array = base.Comm.Append(base.DataSource, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append(b, 2, pad);
			array = base.Comm.Append(connectionParameters.Password, 253, pad);
			base.Comm.Append((byte)((array.Length >= 253) ? 255u : ((uint)(array.Length + 2))));
			base.Comm.Append((byte)((byte)Version / 10));
			base.Comm.Append((byte)((byte)Version % 10));
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
			base.Comm.Append(b, 8, pad);
			base.Comm.Append((short)0);
			base.Comm.Append((byte)0);
			array = base.Comm.Append(base.Charset, 30, pad);
			base.Comm.Append((byte)((array.Length >= 30) ? 30u : ((uint)array.Length)));
			base.Comm.Append((byte)1);
			array = base.Comm.Append(base.PacketSize.ToString(), 6, pad);
			base.Comm.Append((byte)3);
			base.Comm.Append(b, 8, pad);
			base.Comm.SendPacket();
			base.MoreResults = true;
			SkipToEnd();
			return base.IsConnected;
		}

		protected override void ProcessColumnInfo()
		{
			int tdsShort = base.Comm.GetTdsShort();
			int num = 0;
			while (num < tdsShort)
			{
				byte value = 0;
				byte value2 = 0;
				int num2 = -1;
				byte[] array = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = base.Comm.GetByte();
					num++;
				}
				bool value3 = (array[2] & 1) > 0;
				bool flag = (array[2] & 0xC) > 0;
				string baseTableName = string.Empty;
				TdsColumnType tdsColumnType = (TdsColumnType)base.Comm.GetByte();
				num++;
				switch (tdsColumnType)
				{
				case TdsColumnType.Image:
				case TdsColumnType.Text:
				{
					base.Comm.Skip(4L);
					num += 4;
					int tdsShort2 = base.Comm.GetTdsShort();
					num += 2;
					baseTableName = base.Comm.GetString(tdsShort2);
					num += tdsShort2;
					num2 = int.MinValue;
					break;
				}
				case TdsColumnType.Decimal:
				case TdsColumnType.Numeric:
					num2 = base.Comm.GetByte();
					num++;
					value2 = base.Comm.GetByte();
					num++;
					value = base.Comm.GetByte();
					num++;
					break;
				default:
					if (Tds.IsFixedSizeColumn(tdsColumnType))
					{
						num2 = Tds.LookupBufferSize(tdsColumnType);
						break;
					}
					num2 = base.Comm.GetByte() & 0xFF;
					num++;
					break;
				}
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				int index = base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = tdsColumnType;
				tdsDataColumn.ColumnSize = num2;
				tdsDataColumn.ColumnName = base.ColumnNames[index] as string;
				tdsDataColumn.NumericPrecision = value2;
				tdsDataColumn.NumericScale = value;
				tdsDataColumn.IsReadOnly = !flag;
				tdsDataColumn.BaseTableName = baseTableName;
				tdsDataColumn.AllowDBNull = value3;
			}
		}
	}
}
