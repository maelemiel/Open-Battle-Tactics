namespace System.Data.SqlClient
{
	public sealed class SqlDependency
	{
		private string uniqueId = Guid.NewGuid().ToString();

		public string Id
		{
			get
			{
				return uniqueId;
			}
		}

		[System.MonoTODO]
		public bool HasChanges
		{
			get
			{
				return true;
			}
		}

		public event OnChangeEventHandler OnChange;

		[System.MonoTODO]
		public SqlDependency()
		{
		}

		[System.MonoTODO]
		public SqlDependency(SqlCommand command)
		{
		}

		[System.MonoTODO]
		public SqlDependency(SqlCommand command, string options, int timeout)
		{
		}

		[System.MonoTODO]
		public void AddCommandDependency(SqlCommand command)
		{
		}

		[System.MonoTODO]
		public static bool Start(string connectionString)
		{
			return true;
		}

		[System.MonoTODO]
		public static bool Start(string connectionString, string queue)
		{
			return true;
		}

		[System.MonoTODO]
		public static bool Stop(string connectionString)
		{
			return true;
		}

		[System.MonoTODO]
		public static bool Stop(string connectionString, string queue)
		{
			return true;
		}
	}
}
