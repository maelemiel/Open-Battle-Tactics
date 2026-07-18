using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class IgnoreAttribute : Attribute
	{
		private string reason;

		public string Reason
		{
			get
			{
				return reason;
			}
		}

		public IgnoreAttribute()
		{
			reason = "";
		}

		public IgnoreAttribute(string reason)
		{
			this.reason = reason;
		}
	}
}
