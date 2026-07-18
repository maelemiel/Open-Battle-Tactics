using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class CategoryAttribute : Attribute
	{
		protected string categoryName;

		public string Name
		{
			get
			{
				return categoryName;
			}
		}

		public CategoryAttribute(string name)
		{
			categoryName = name.Trim();
		}

		protected CategoryAttribute()
		{
			categoryName = GetType().Name;
			if (categoryName.EndsWith("Attribute"))
			{
				categoryName = categoryName.Substring(0, categoryName.Length - 9);
			}
		}
	}
}
