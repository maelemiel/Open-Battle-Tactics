using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestAttribute : Attribute
	{
		private string description;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}
	}
}
