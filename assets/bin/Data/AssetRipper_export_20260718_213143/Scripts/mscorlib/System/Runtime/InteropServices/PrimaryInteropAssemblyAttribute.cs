namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public sealed class PrimaryInteropAssemblyAttribute : Attribute
	{
		private int major;

		private int minor;

		public int MajorVersion
		{
			get
			{
				return major;
			}
		}

		public int MinorVersion
		{
			get
			{
				return minor;
			}
		}

		public PrimaryInteropAssemblyAttribute(int major, int minor)
		{
			this.major = major;
			this.minor = minor;
		}
	}
}
