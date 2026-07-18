using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MaxTimeAttribute : PropertyAttribute
	{
		public MaxTimeAttribute(int milliseconds)
			: base(milliseconds)
		{
		}
	}
}
