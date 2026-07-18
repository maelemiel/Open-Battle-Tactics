using System;

namespace NUnit.Framework
{
	public interface ITestCaseData
	{
		object[] Arguments { get; }

		object Result { get; }

		bool HasExpectedResult { get; }

		Type ExpectedException { get; }

		string ExpectedExceptionName { get; }

		string TestName { get; }

		string Description { get; }

		bool Ignored { get; }

		bool Explicit { get; }

		string IgnoreReason { get; }
	}
}
