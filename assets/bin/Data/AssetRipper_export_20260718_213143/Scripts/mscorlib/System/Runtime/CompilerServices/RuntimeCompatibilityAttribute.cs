namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	public sealed class RuntimeCompatibilityAttribute : Attribute
	{
		private bool wrap_non_exception_throws;

		public bool WrapNonExceptionThrows
		{
			get
			{
				return wrap_non_exception_throws;
			}
			set
			{
				wrap_non_exception_throws = value;
			}
		}
	}
}
