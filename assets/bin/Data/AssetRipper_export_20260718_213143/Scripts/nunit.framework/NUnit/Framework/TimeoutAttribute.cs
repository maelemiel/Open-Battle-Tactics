using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class TimeoutAttribute : PropertyAttribute
	{
		public TimeoutAttribute(int timeout)
			: base(timeout)
		{
		}
	}
}
