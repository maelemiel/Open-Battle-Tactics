using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExpectedExceptionAttribute : Attribute
	{
		private Type expectedException;

		private string expectedExceptionName;

		private string expectedMessage;

		private MessageMatch matchType;

		private string userMessage;

		private string handler;

		public Type ExpectedException
		{
			get
			{
				return expectedException;
			}
			set
			{
				expectedException = value;
				expectedExceptionName = expectedException.FullName;
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

		public string UserMessage
		{
			get
			{
				return userMessage;
			}
			set
			{
				userMessage = value;
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

		public string Handler
		{
			get
			{
				return handler;
			}
			set
			{
				handler = value;
			}
		}

		public ExpectedExceptionAttribute()
		{
		}

		public ExpectedExceptionAttribute(Type exceptionType)
		{
			expectedException = exceptionType;
			expectedExceptionName = exceptionType.FullName;
		}

		public ExpectedExceptionAttribute(string exceptionName)
		{
			expectedExceptionName = exceptionName;
		}
	}
}
