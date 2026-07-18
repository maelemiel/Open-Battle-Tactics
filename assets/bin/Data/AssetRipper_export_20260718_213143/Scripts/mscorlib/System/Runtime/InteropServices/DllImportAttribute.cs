namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class DllImportAttribute : Attribute
	{
		public CallingConvention CallingConvention;

		public CharSet CharSet;

		private string Dll;

		public string EntryPoint;

		public bool ExactSpelling;

		public bool PreserveSig;

		public bool SetLastError;

		public bool BestFitMapping;

		public bool ThrowOnUnmappableChar;

		public string Value
		{
			get
			{
				return Dll;
			}
		}

		public DllImportAttribute(string dllName)
		{
			Dll = dllName;
		}
	}
}
