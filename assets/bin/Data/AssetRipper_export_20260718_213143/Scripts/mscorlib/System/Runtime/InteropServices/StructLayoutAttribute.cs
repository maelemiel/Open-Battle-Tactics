namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	[ComVisible(true)]
	public sealed class StructLayoutAttribute : Attribute
	{
		public CharSet CharSet = CharSet.Auto;

		public int Pack = 8;

		public int Size;

		private LayoutKind lkind;

		public LayoutKind Value
		{
			get
			{
				return lkind;
			}
		}

		public StructLayoutAttribute(short layoutKind)
		{
			lkind = (LayoutKind)layoutKind;
		}

		public StructLayoutAttribute(LayoutKind layoutKind)
		{
			lkind = layoutKind;
		}
	}
}
