namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	[ComVisible(true)]
	public sealed class MarshalAsAttribute : Attribute
	{
		private UnmanagedType utype;

		public UnmanagedType ArraySubType;

		public string MarshalCookie;

		[ComVisible(true)]
		public string MarshalType;

		[ComVisible(true)]
		public Type MarshalTypeRef;

		public VarEnum SafeArraySubType;

		public int SizeConst;

		public short SizeParamIndex;

		public Type SafeArrayUserDefinedSubType;

		public int IidParameterIndex;

		public UnmanagedType Value
		{
			get
			{
				return utype;
			}
		}

		public MarshalAsAttribute(short unmanagedType)
		{
			utype = (UnmanagedType)unmanagedType;
		}

		public MarshalAsAttribute(UnmanagedType unmanagedType)
		{
			utype = unmanagedType;
		}
	}
}
