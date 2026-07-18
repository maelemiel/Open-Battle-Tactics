using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class SetUICultureAttribute : PropertyAttribute
	{
		public SetUICultureAttribute(string culture)
			: base("_SETUICULTURE", culture)
		{
		}
	}
}
