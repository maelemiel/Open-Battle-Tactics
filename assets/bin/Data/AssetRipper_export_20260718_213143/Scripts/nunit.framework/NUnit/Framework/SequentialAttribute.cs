using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SequentialAttribute : PropertyAttribute
	{
		public SequentialAttribute()
			: base("_JOINTYPE", "Sequential")
		{
		}
	}
}
