using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CultureAttribute : IncludeExcludeAttribute
	{
		public CultureAttribute()
		{
		}

		public CultureAttribute(string cultures)
			: base(cultures)
		{
		}
	}
}
