using System;
using System.Collections;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseAttribute : Attribute, ITestCaseData
	{
		private object[] arguments;

		private object expectedResult;

		private bool hasExpectedResult;

		private Type expectedExceptionType;

		private string expectedExceptionName;

		private string expectedMessage;

		private MessageMatch matchType;

		private string description;

		private string testName;

		private bool isIgnored;

		private bool isExplicit;

		private string reason;

		private string category;

		public object[] Arguments
		{
			get
			{
				return arguments;
			}
		}

		public object Result
		{
			get
			{
				return expectedResult;
			}
			set
			{
				expectedResult = value;
				hasExpectedResult = true;
			}
		}

		public object ExpectedResult
		{
			get
			{
				return expectedResult;
			}
		}

		public bool HasExpectedResult
		{
			get
			{
				return hasExpectedResult;
			}
		}

		public IList Categories
		{
			get
			{
				return (category == null) ? null : category.Split(',');
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

		public Type ExpectedException
		{
			get
			{
				return expectedExceptionType;
			}
			set
			{
				expectedExceptionType = value;
				expectedExceptionName = expectedExceptionType.FullName;
			}
		}

		public string ExpectedExceptionName
		{
			get
			{
				return expectedExceptionName;
			}
			set
			{
				expectedExceptionName = value;
				expectedExceptionType = null;
			}
		}

		public string ExpectedMessage
		{
			get
			{
				return expectedMessage;
			}
			set
			{
				expectedMessage = value;
			}
		}

		public MessageMatch MatchType
		{
			get
			{
				return matchType;
			}
			set
			{
				matchType = value;
			}
		}

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

		public string TestName
		{
			get
			{
				return testName;
			}
			set
			{
				testName = value;
			}
		}

		public bool Ignore
		{
			get
			{
				return isIgnored;
			}
			set
			{
				isIgnored = value;
			}
		}

		public bool Ignored
		{
			get
			{
				return isIgnored;
			}
			set
			{
				isIgnored = value;
			}
		}

		public bool Explicit
		{
			get
			{
				return isExplicit;
			}
			set
			{
				isExplicit = value;
			}
		}

		public string Reason
		{
			get
			{
				return reason;
			}
			set
			{
				reason = value;
			}
		}

		public string IgnoreReason
		{
			get
			{
				return reason;
			}
			set
			{
				reason = value;
				isIgnored = reason != null && reason != string.Empty;
			}
		}

		public TestCaseAttribute(params object[] arguments)
		{
			if (arguments == null)
			{
				object[] array = new object[1];
				this.arguments = array;
			}
			else
			{
				this.arguments = arguments;
			}
		}

		public TestCaseAttribute(object arg)
		{
			arguments = new object[1] { arg };
		}

		public TestCaseAttribute(object arg1, object arg2)
		{
			arguments = new object[2] { arg1, arg2 };
		}

		public TestCaseAttribute(object arg1, object arg2, object arg3)
		{
			arguments = new object[3] { arg1, arg2, arg3 };
		}
	}
}
