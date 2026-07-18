using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
	public class ValueSourceAttribute : Attribute
	{
		private readonly string sourceName;

		private readonly Type sourceType;

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

		public ValueSourceAttribute(string sourceName)
		{
			this.sourceName = sourceName;
		}

		public ValueSourceAttribute(Type sourceType, string sourceName)
		{
			this.sourceType = sourceType;
			this.sourceName = sourceName;
		}
	}
}
