using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public class RequiredAddinAttribute : Attribute
	{
		private string requiredAddin;

		public string RequiredAddin
		{
			get
			{
				return requiredAddin;
			}
		}

		public RequiredAddinAttribute(string requiredAddin)
		{
			this.requiredAddin = requiredAddin;
		}
	}
}
