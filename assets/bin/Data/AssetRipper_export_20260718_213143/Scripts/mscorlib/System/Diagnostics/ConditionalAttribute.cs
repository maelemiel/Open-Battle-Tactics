using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ConditionalAttribute : Attribute
	{
		private string myCondition;

		public string ConditionString
		{
			get
			{
				return myCondition;
			}
		}

		public ConditionalAttribute(string conditionString)
		{
			myCondition = conditionString;
		}
	}
}
