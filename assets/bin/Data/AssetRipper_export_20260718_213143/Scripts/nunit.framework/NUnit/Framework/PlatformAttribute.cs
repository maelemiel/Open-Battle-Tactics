using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class PlatformAttribute : IncludeExcludeAttribute
	{
		public PlatformAttribute()
		{
		}

		public PlatformAttribute(string platforms)
			: base(platforms)
		{
		}
	}
}
