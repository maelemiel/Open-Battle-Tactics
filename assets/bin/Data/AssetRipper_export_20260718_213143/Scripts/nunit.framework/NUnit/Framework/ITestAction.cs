namespace NUnit.Framework
{
	public interface ITestAction
	{
		ActionTargets Targets { get; }

		void BeforeTest(TestDetails testDetails);

		void AfterTest(TestDetails testDetails);
	}
}
