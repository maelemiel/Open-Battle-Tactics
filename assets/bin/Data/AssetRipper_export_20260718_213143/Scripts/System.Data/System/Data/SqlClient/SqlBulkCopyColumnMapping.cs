namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopyColumnMapping
	{
		private int sourceOrdinal = -1;

		private int destinationOrdinal = -1;

		private string sourceColumn;

		private string destinationColumn;

		public string DestinationColumn
		{
			get
			{
				if (destinationColumn != null)
				{
					return destinationColumn;
				}
				return string.Empty;
			}
			set
			{
				destinationOrdinal = -1;
				destinationColumn = value;
			}
		}

		public string SourceColumn
		{
			get
			{
				if (sourceColumn != null)
				{
					return sourceColumn;
				}
				return string.Empty;
			}
			set
			{
				sourceOrdinal = -1;
				sourceColumn = value;
			}
		}

		public int DestinationOrdinal
		{
			get
			{
				return destinationOrdinal;
			}
			set
			{
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				destinationColumn = null;
				destinationOrdinal = value;
			}
		}

		public int SourceOrdinal
		{
			get
			{
				return sourceOrdinal;
			}
			set
			{
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				sourceColumn = null;
				sourceOrdinal = value;
			}
		}

		public SqlBulkCopyColumnMapping()
		{
		}

		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, int destinationOrdinal)
		{
			SourceOrdinal = sourceColumnOrdinal;
			DestinationOrdinal = destinationOrdinal;
		}

		public SqlBulkCopyColumnMapping(int sourceColumnOrdinal, string destinationColumn)
		{
			SourceOrdinal = sourceColumnOrdinal;
			DestinationColumn = destinationColumn;
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, int destinationOrdinal)
		{
			SourceColumn = sourceColumn;
			DestinationOrdinal = destinationOrdinal;
		}

		public SqlBulkCopyColumnMapping(string sourceColumn, string destinationColumn)
		{
			SourceColumn = sourceColumn;
			DestinationColumn = destinationColumn;
		}
	}
}
