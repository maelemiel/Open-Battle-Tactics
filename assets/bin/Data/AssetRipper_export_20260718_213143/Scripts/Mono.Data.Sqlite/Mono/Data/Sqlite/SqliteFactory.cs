using System;
using System.Data.Common;
using System.Reflection;

namespace Mono.Data.Sqlite
{
	public sealed class SqliteFactory : DbProviderFactory, IServiceProvider
	{
		public static readonly SqliteFactory Instance;

		private static Type _dbProviderServicesType;

		private static object _sqliteServices;

		static SqliteFactory()
		{
			Instance = new SqliteFactory();
			_dbProviderServicesType = Type.GetType("System.Data.Common.DbProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			if (serviceType == typeof(ISQLiteSchemaExtensions) || (_dbProviderServicesType != null && serviceType == _dbProviderServicesType))
			{
				return GetSQLiteProviderServicesInstance();
			}
			return null;
		}

		public override DbCommand CreateCommand()
		{
			return new SqliteCommand();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new SqliteCommandBuilder();
		}

		public override DbConnection CreateConnection()
		{
			return new SqliteConnection();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new SqliteConnectionStringBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new SqliteDataAdapter();
		}

		public override DbParameter CreateParameter()
		{
			return new SqliteParameter();
		}

		private object GetSQLiteProviderServicesInstance()
		{
			if (_sqliteServices == null)
			{
				Type type = Type.GetType("Mono.Data.Sqlite.SQLiteProviderServices, Mono.Data.Sqlite.Linq, Version=2.0.38.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139", false);
				if (type != null)
				{
					FieldInfo field = type.GetField("Instance", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
					_sqliteServices = field.GetValue(null);
				}
			}
			return _sqliteServices;
		}
	}
}
