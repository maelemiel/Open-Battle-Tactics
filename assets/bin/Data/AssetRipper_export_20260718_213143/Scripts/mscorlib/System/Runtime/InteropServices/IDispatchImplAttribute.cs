namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Obsolete]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class IDispatchImplAttribute : Attribute
	{
		private IDispatchImplType Impl;

		public IDispatchImplType Value
		{
			get
			{
				return Impl;
			}
		}

		public IDispatchImplAttribute(IDispatchImplType implType)
		{
			Impl = implType;
		}

		public IDispatchImplAttribute(short implType)
		{
			Impl = (IDispatchImplType)implType;
		}
	}
}
