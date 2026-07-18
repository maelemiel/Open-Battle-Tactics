namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, Inherited = false)]
	public sealed class ClassInterfaceAttribute : Attribute
	{
		private ClassInterfaceType ciType;

		public ClassInterfaceType Value
		{
			get
			{
				return ciType;
			}
		}

		public ClassInterfaceAttribute(short classInterfaceType)
		{
			ciType = (ClassInterfaceType)classInterfaceType;
		}

		public ClassInterfaceAttribute(ClassInterfaceType classInterfaceType)
		{
			ciType = classInterfaceType;
		}
	}
}
