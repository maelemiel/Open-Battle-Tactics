using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SetUpFixtureAttribute : Attribute
	{
	}
}
