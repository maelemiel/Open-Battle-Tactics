namespace Mono.Data.Tds.Protocol
{
	public class Tds80 : Tds70
	{
		public static readonly TdsVersion Version = TdsVersion.tds80;

		protected override byte[] ClientVersion
		{
			get
			{
				return new byte[4] { 0, 0, 0, 113 };
			}
		}

		public Tds80(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds80(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, Version)
		{
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			return base.Connect(connectionParameters);
		}

		protected override void ProcessColumnInfo()
		{
			if (base.TdsVersion < TdsVersion.tds80)
			{
				base.ProcessColumnInfo();
				return;
			}
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
				if ((byte)tdsColumnType == 239)
				{
					tdsColumnType = TdsColumnType.NChar;
				}
				TdsColumnType tdsColumnType2 = tdsColumnType;
				if (IsLargeType(tdsColumnType) && tdsColumnType != TdsColumnType.NChar)
				{
					tdsColumnType -= 128;
				}
				string baseTableName = null;
				byte[] array2 = null;
				int value4 = 0;
				int value5 = 0;
				int num = (IsBlobType(tdsColumnType) ? base.Comm.GetTdsInt() : (Tds.IsFixedSizeColumn(tdsColumnType) ? Tds.LookupBufferSize(tdsColumnType) : ((!IsLargeType(tdsColumnType2)) ? (base.Comm.GetByte() & 0xFF) : base.Comm.GetTdsShort())));
				if (tdsColumnType2 == TdsColumnType.BigChar || tdsColumnType2 == TdsColumnType.BigNVarChar || tdsColumnType2 == TdsColumnType.BigVarChar || tdsColumnType2 == TdsColumnType.NChar || tdsColumnType2 == TdsColumnType.NVarChar || tdsColumnType2 == TdsColumnType.Text || tdsColumnType2 == TdsColumnType.NText)
				{
					array2 = base.Comm.GetBytes(5, true);
					value4 = TdsCollation.LCID(array2);
					value5 = TdsCollation.SortId(array2);
				}
				if (IsBlobType(tdsColumnType))
				{
					baseTableName = base.Comm.GetString(base.Comm.GetTdsShort());
				}
				byte value6 = 0;
				byte value7 = 0;
				switch (tdsColumnType)
				{
				case TdsColumnType.NText:
				case TdsColumnType.NVarChar:
				case TdsColumnType.NChar:
					num /= 2;
					break;
				case TdsColumnType.Decimal:
				case TdsColumnType.Numeric:
					value6 = base.Comm.GetByte();
					value7 = base.Comm.GetByte();
					break;
				}
				string columnName = base.Comm.GetString(base.Comm.GetByte());
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = tdsColumnType;
				tdsDataColumn.ColumnName = columnName;
				tdsDataColumn.IsAutoIncrement = value2;
				tdsDataColumn.IsIdentity = value3;
				tdsDataColumn.ColumnSize = num;
				tdsDataColumn.NumericPrecision = value6;
				tdsDataColumn.NumericScale = value7;
				tdsDataColumn.IsReadOnly = !flag;
				tdsDataColumn.AllowDBNull = value;
				tdsDataColumn.BaseTableName = baseTableName;
				tdsDataColumn.LCID = value4;
				tdsDataColumn.SortOrder = value5;
			}
		}

		protected override void ProcessOutputParam()
		{
			if (base.TdsVersion < TdsVersion.tds80)
			{
				base.ProcessOutputParam();
				return;
			}
			GetSubPacketLength();
			base.Comm.Skip((base.Comm.GetByte() & 0xFF) << 1);
			base.Comm.Skip(1L);
			base.Comm.Skip(4L);
			TdsColumnType value = (TdsColumnType)base.Comm.GetByte();
			object columnValue = GetColumnValue(value, true);
			base.OutputParameters.Add(columnValue);
		}
	}
}
