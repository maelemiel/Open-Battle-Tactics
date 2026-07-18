using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class SetCultureAttribute : PropertyAttribute
	{
		public SetCultureAttribute(string culture)
			: base("_SETCULTURE", culture)
		{
		}
	}
}
