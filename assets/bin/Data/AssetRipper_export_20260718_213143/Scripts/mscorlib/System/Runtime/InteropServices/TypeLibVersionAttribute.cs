namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	[ComVisible(true)]
	public sealed class TypeLibVersionAttribute : Attribute
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

		public TypeLibVersionAttribute(int major, int minor)
		{
			this.major = major;
			this.minor = minor;
		}
	}
}
