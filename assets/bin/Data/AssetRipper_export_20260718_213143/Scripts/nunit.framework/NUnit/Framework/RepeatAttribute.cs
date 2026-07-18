using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RepeatAttribute : PropertyAttribute
	{
		public RepeatAttribute(int count)
			: base(count)
		{
		}
	}
}
