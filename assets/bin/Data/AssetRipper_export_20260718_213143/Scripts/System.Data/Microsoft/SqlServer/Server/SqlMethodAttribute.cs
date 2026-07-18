using System;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlMethodAttribute : SqlFunctionAttribute
	{
		private bool isMutator;

		private bool onNullCall;

		public bool IsMutator
		{
			get
			{
				return isMutator;
			}
			set
			{
				isMutator = value;
			}
		}

		public bool OnNullCall
		{
			get
			{
				return onNullCall;
			}
			set
			{
				onNullCall = value;
			}
		}

		public SqlMethodAttribute()
		{
			isMutator = false;
			onNullCall = false;
		}
	}
}
