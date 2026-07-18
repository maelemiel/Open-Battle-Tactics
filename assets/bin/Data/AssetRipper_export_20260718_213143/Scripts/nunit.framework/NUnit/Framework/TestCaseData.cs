using System;
using System.Collections;
using System.Collections.Specialized;

namespace NUnit.Framework
{
	public class TestCaseData : ITestCaseData
	{
		private static readonly string CATEGORIES = "_CATEGORIES";

		private object[] arguments;

		private object expectedResult;

		private bool hasExpectedResult;

		private Type expectedExceptionType;

		private string expectedExceptionName;

		private string testName;

		private string description;

		private IDictionary properties;

		private bool isIgnored;

		private bool isExplicit;

		private string ignoreReason;

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
		}

		public bool HasExpectedResult
		{
			get
			{
				return hasExpectedResult;
			}
		}

		public Type ExpectedException
		{
			get
			{
				return expectedExceptionType;
			}
		}

		public string ExpectedExceptionName
		{
			get
			{
				return expectedExceptionName;
			}
		}

		public string TestName
		{
			get
			{
				return testName;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
		}

		public bool Ignored
		{
			get
			{
				return isIgnored;
			}
		}

		public bool Explicit
		{
			get
			{
				return isExplicit;
			}
		}

		public string IgnoreReason
		{
			get
			{
				return ignoreReason;
			}
		}

		public IList Categories
		{
			get
			{
				if (Properties[CATEGORIES] == null)
				{
					Properties[CATEGORIES] = new ArrayList();
				}
				return (IList)Properties[CATEGORIES];
			}
		}

		public IDictionary Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new ListDictionary();
				}
				return properties;
			}
		}

		public TestCaseData(params object[] args)
		{
			if (args == null)
			{
				object[] array = new object[1];
				arguments = array;
			}
			else
			{
				arguments = args;
			}
		}

		public TestCaseData(object arg)
		{
			arguments = new object[1] { arg };
		}

		public TestCaseData(object arg1, object arg2)
		{
			arguments = new object[2] { arg1, arg2 };
		}

		public TestCaseData(object arg1, object arg2, object arg3)
		{
			arguments = new object[3] { arg1, arg2, arg3 };
		}

		public TestCaseData Returns(object result)
		{
			expectedResult = result;
			hasExpectedResult = true;
			return this;
		}

		public TestCaseData Throws(Type exceptionType)
		{
			expectedExceptionType = exceptionType;
			expectedExceptionName = exceptionType.FullName;
			return this;
		}

		public TestCaseData Throws(string exceptionName)
		{
			expectedExceptionName = exceptionName;
			return this;
		}

		public TestCaseData SetName(string name)
		{
			testName = name;
			return this;
		}

		public TestCaseData SetDescription(string description)
		{
			this.description = description;
			return this;
		}

		public TestCaseData SetCategory(string category)
		{
			Categories.Add(category);
			return this;
		}

		public TestCaseData SetProperty(string propName, string propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, int propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, double propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData Ignore()
		{
			isIgnored = true;
			return this;
		}

		public TestCaseData Ignore(string reason)
		{
			isIgnored = true;
			ignoreReason = reason;
			return this;
		}

		public TestCaseData MakeExplicit()
		{
			isExplicit = true;
			return this;
		}

		public TestCaseData MakeExplicit(string reason)
		{
			isExplicit = true;
			ignoreReason = reason;
			return this;
		}
	}
}
