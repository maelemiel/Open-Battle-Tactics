namespace System.Data.Common
{
	internal class SchemaInfo
	{
		private string columnName;

		private string tableName;

		private string dataTypeName;

		private bool allowDBNull;

		private bool isReadOnly;

		private int ordinal;

		private int size;

		private byte precision;

		private byte scale;

		private Type fieldType;

		public bool AllowDBNull
		{
			get
			{
				return allowDBNull;
			}
			set
			{
				allowDBNull = value;
			}
		}

		public string ColumnName
		{
			get
			{
				return columnName;
			}
			set
			{
				columnName = value;
			}
		}

		public int ColumnOrdinal
		{
			get
			{
				return ordinal;
			}
			set
			{
				ordinal = value;
			}
		}

		public int ColumnSize
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
			}
		}

		public string DataTypeName
		{
			get
			{
				return dataTypeName;
			}
			set
			{
				dataTypeName = value;
			}
		}

		public Type FieldType
		{
			get
			{
				return fieldType;
			}
			set
			{
				fieldType = value;
			}
		}

		public byte NumericPrecision
		{
			get
			{
				return precision;
			}
			set
			{
				precision = value;
			}
		}

		public byte NumericScale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public string TableName
		{
			get
			{
				return tableName;
			}
			set
			{
				tableName = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return isReadOnly;
			}
			set
			{
				isReadOnly = value;
			}
		}
	}
}
