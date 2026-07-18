namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
	public sealed class UnmanagedFunctionPointerAttribute : Attribute
	{
		private CallingConvention call_conv;

		public CharSet CharSet;

		public bool SetLastError;

		public bool BestFitMapping;

		public bool ThrowOnUnmappableChar;

		public CallingConvention CallingConvention
		{
			get
			{
				return call_conv;
			}
		}

		public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
		{
			call_conv = callingConvention;
		}
	}
}
