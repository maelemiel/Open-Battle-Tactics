using System;
using System.Collections;
using System.Collections.Specialized;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class PropertyAttribute : Attribute
	{
		private IDictionary properties = new ListDictionary();

		public IDictionary Properties
		{
			get
			{
				return properties;
			}
		}

		public PropertyAttribute(string propertyName, string propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		public PropertyAttribute(string propertyName, int propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		public PropertyAttribute(string propertyName, double propertyValue)
		{
			properties.Add(propertyName, propertyValue);
		}

		protected PropertyAttribute()
		{
		}

		protected PropertyAttribute(object propertyValue)
		{
			string text = GetType().Name;
			if (text.EndsWith("Attribute"))
			{
				text = text.Substring(0, text.Length - 9);
			}
			properties.Add(text, propertyValue);
		}
	}
}
