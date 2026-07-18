using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework
{
	public class TestContext
	{
		public class TestAdapter
		{
			private IDictionary _context;

			public string Name
			{
				get
				{
					return _context["Test.Name"] as string;
				}
			}

			public string FullName
			{
				get
				{
					return _context["Test.FullName"] as string;
				}
			}

			public IDictionary Properties
			{
				get
				{
					return _context["Test.Properties"] as IDictionary;
				}
			}

			public TestAdapter(IDictionary context)
			{
				_context = context;
			}
		}

		public class ResultAdapter
		{
			private IDictionary _context;

			public TestState State
			{
				get
				{
					return (TestState)_context["Result.State"];
				}
			}

			public TestStatus Status
			{
				get
				{
					switch (State)
					{
					default:
						return TestStatus.Inconclusive;
					case TestState.NotRunnable:
					case TestState.Skipped:
					case TestState.Ignored:
						return TestStatus.Skipped;
					case TestState.Success:
						return TestStatus.Passed;
					case TestState.Failure:
					case TestState.Error:
					case TestState.Cancelled:
						return TestStatus.Failed;
					}
				}
			}

			public ResultAdapter(IDictionary context)
			{
				_context = context;
			}
		}

		private const string contextKey = "NUnit.Framework.TestContext";

		private const string stateKey = "Result.State";

		private IDictionary _context;

		private TestAdapter _test;

		private ResultAdapter _result;

		public static TestContext CurrentContext
		{
			get
			{
				return new TestContext((IDictionary)CallContext.GetData("NUnit.Framework.TestContext"));
			}
		}

		public TestAdapter Test
		{
			get
			{
				if (_test == null)
				{
					_test = new TestAdapter(_context);
				}
				return _test;
			}
		}

		public ResultAdapter Result
		{
			get
			{
				if (_result == null)
				{
					_result = new ResultAdapter(_context);
				}
				return _result;
			}
		}

		public string TestDirectory
		{
			get
			{
				return (string)_context["TestDirectory"];
			}
		}

		public string WorkDirectory
		{
			get
			{
				return (string)_context["WorkDirectory"];
			}
		}

		public TestContext(IDictionary context)
		{
			_context = context;
		}
	}
}
