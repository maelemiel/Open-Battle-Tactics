namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class InterfaceTypeAttribute : Attribute
	{
		private ComInterfaceType intType;

		public ComInterfaceType Value
		{
			get
			{
				return intType;
			}
		}

		public InterfaceTypeAttribute(ComInterfaceType interfaceType)
		{
			intType = interfaceType;
		}

		public InterfaceTypeAttribute(short interfaceType)
		{
			intType = (ComInterfaceType)interfaceType;
		}
	}
}
