using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public abstract class TestActionAttribute : Attribute, ITestAction
	{
		public virtual ActionTargets Targets
		{
			get
			{
				return ActionTargets.Default;
			}
		}

		public virtual void BeforeTest(TestDetails testDetails)
		{
		}

		public virtual void AfterTest(TestDetails testDetails)
		{
		}
	}
}
