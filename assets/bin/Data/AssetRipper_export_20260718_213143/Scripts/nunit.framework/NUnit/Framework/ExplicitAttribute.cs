using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExplicitAttribute : Attribute
	{
		private string reason;

		public string Reason
		{
			get
			{
				return reason;
			}
		}

		public ExplicitAttribute()
		{
			reason = "";
		}

		public ExplicitAttribute(string reason)
		{
			this.reason = reason;
		}
	}
}
