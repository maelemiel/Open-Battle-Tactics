using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class MethodImplAttribute : Attribute
	{
		private MethodImplOptions _val;

		public MethodCodeType MethodCodeType;

		public MethodImplOptions Value
		{
			get
			{
				return _val;
			}
		}

		public MethodImplAttribute()
		{
		}

		public MethodImplAttribute(short value)
		{
			_val = (MethodImplOptions)value;
		}

		public MethodImplAttribute(MethodImplOptions methodImplOptions)
		{
			_val = methodImplOptions;
		}
	}
}
