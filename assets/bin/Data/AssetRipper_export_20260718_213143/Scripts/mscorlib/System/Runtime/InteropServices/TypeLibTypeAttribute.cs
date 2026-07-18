namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
	public sealed class TypeLibTypeAttribute : Attribute
	{
		private TypeLibTypeFlags flags;

		public TypeLibTypeFlags Value
		{
			get
			{
				return flags;
			}
		}

		public TypeLibTypeAttribute(short flags)
		{
			this.flags = (TypeLibTypeFlags)flags;
		}

		public TypeLibTypeAttribute(TypeLibTypeFlags flags)
		{
			this.flags = flags;
		}
	}
}
