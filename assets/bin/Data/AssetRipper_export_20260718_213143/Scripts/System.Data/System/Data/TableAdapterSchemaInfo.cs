using System.Collections;
using System.Data.Common;

namespace System.Data
{
	internal class TableAdapterSchemaInfo
	{
		public DbProviderFactory Provider;

		public DbDataAdapter Adapter;

		public DbConnection Connection;

		public string ConnectionString;

		public string BaseClass;

		public string Name;

		public bool ShortCommands;

		public ArrayList Commands;

		public TableAdapterSchemaInfo(DbProviderFactory provider)
		{
			Provider = provider;
			Adapter = provider.CreateDataAdapter();
			Connection = provider.CreateConnection();
			Commands = new ArrayList();
			ShortCommands = false;
		}

		public TableAdapterSchemaInfo()
		{
			Commands = new ArrayList();
			ShortCommands = false;
		}
	}
}
