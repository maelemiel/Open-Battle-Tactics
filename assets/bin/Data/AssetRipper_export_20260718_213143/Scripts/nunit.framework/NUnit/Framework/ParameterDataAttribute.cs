using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public abstract class ParameterDataAttribute : Attribute
	{
		public abstract IEnumerable GetData(ParameterInfo parameter);
	}
}
