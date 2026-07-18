namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class TypeLibFuncAttribute : Attribute
	{
		private TypeLibFuncFlags flags;

		public TypeLibFuncFlags Value
		{
			get
			{
				return flags;
			}
		}

		public TypeLibFuncAttribute(short flags)
		{
			this.flags = (TypeLibFuncFlags)flags;
		}

		public TypeLibFuncAttribute(TypeLibFuncFlags flags)
		{
			this.flags = flags;
		}
	}
}
