using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class DescriptionAttribute : Attribute
	{
		private string description;

		public string Description
		{
			get
			{
				return description;
			}
		}

		public DescriptionAttribute(string description)
		{
			this.description = description;
		}
	}
}
