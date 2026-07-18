using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	public abstract class DbProviderFactory
	{
		public virtual bool CanCreateDataSourceEnumerator
		{
			get
			{
				throw CreateNotImplementedException();
			}
		}

		private NotImplementedException CreateNotImplementedException()
		{
			return new NotImplementedException();
		}

		public virtual DbCommand CreateCommand()
		{
			throw CreateNotImplementedException();
		}

		public virtual DbCommandBuilder CreateCommandBuilder()
		{
			throw CreateNotImplementedException();
		}

		public virtual DbConnection CreateConnection()
		{
			throw CreateNotImplementedException();
		}

		public virtual DbDataAdapter CreateDataAdapter()
		{
			throw CreateNotImplementedException();
		}

		public virtual DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw CreateNotImplementedException();
		}

		public virtual DbParameter CreateParameter()
		{
			throw CreateNotImplementedException();
		}

		public virtual CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw CreateNotImplementedException();
		}

		public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw CreateNotImplementedException();
		}
	}
}
