using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseSourceAttribute : Attribute
	{
		private readonly string sourceName;

		private readonly Type sourceType;

		private string category;

		public string SourceName
		{
			get
			{
				return sourceName;
			}
		}

		public Type SourceType
		{
			get
			{
				return sourceType;
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public TestCaseSourceAttribute(string sourceName)
		{
			this.sourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType, string sourceName)
		{
			this.sourceType = sourceType;
			this.sourceName = sourceName;
		}
	}
}
