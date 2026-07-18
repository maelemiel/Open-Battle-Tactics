using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public sealed class DBConcurrencyException : SystemException
	{
		private DataRow[] rows;

		public DataRow Row
		{
			get
			{
				if (rows != null)
				{
					return rows[0];
				}
				return null;
			}
			set
			{
				rows = new DataRow[1] { value };
			}
		}

		public int RowCount
		{
			get
			{
				if (rows != null)
				{
					return rows.Length;
				}
				return 0;
			}
		}

		public DBConcurrencyException()
			: base("Concurrency violation.")
		{
		}

		public DBConcurrencyException(string message)
			: base(message)
		{
		}

		public DBConcurrencyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public DBConcurrencyException(string message, Exception inner, DataRow[] dataRows)
			: base(message, inner)
		{
			rows = dataRows;
		}

		private DBConcurrencyException(SerializationInfo si, StreamingContext sc)
			: base(si, sc)
		{
		}

		[System.MonoTODO]
		public void CopyToRows(DataRow[] array)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public void CopyToRows(DataRow[] array, int ArrayIndex)
		{
			throw new NotImplementedException();
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			if (si == null)
			{
				throw new ArgumentNullException("si");
			}
			base.GetObjectData(si, context);
		}
	}
}
