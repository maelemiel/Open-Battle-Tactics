using System;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlTriggerAttribute : Attribute
	{
		private string triggerEvent;

		private string name;

		private string target;

		public string Event
		{
			get
			{
				return triggerEvent;
			}
			set
			{
				triggerEvent = value;
			}
		}

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

		public string Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public SqlTriggerAttribute()
		{
			triggerEvent = null;
			name = null;
			target = null;
		}
	}
}
