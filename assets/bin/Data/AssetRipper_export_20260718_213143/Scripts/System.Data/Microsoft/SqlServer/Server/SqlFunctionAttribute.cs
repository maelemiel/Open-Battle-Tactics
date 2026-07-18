using System;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SqlFunctionAttribute : Attribute
	{
		private DataAccessKind dataAccess;

		private bool isDeterministic;

		private bool isPrecise;

		private SystemDataAccessKind systemDataAccess;

		public DataAccessKind DataAccess
		{
			get
			{
				return dataAccess;
			}
			set
			{
				dataAccess = value;
			}
		}

		public bool IsDeterministic
		{
			get
			{
				return isDeterministic;
			}
			set
			{
				isDeterministic = value;
			}
		}

		public bool IsPrecise
		{
			get
			{
				return isPrecise;
			}
			set
			{
				isPrecise = value;
			}
		}

		public SystemDataAccessKind SystemDataAccess
		{
			get
			{
				return systemDataAccess;
			}
			set
			{
				systemDataAccess = value;
			}
		}

		public SqlFunctionAttribute()
		{
			dataAccess = DataAccessKind.None;
			isDeterministic = false;
			isPrecise = false;
			systemDataAccess = SystemDataAccessKind.None;
		}
	}
}
