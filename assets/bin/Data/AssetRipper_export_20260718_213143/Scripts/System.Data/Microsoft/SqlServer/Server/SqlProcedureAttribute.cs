using System;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlProcedureAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public SqlProcedureAttribute()
		{
			name = null;
		}
	}
}
