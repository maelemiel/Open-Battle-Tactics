using System.Reflection;

namespace NUnit.Framework
{
	public class TestDetails
	{
		public object Fixture { get; private set; }

		public MethodInfo Method { get; private set; }

		public string FullName { get; private set; }

		public string Type { get; private set; }

		public bool IsSuite { get; private set; }

		public TestDetails(object fixture, MethodInfo method, string fullName, string type, bool isSuite)
		{
			Fixture = fixture;
			Method = method;
			FullName = fullName;
			Type = type;
			IsSuite = isSuite;
		}
	}
}
