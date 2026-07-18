using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_MethodRental))]
	public sealed class MethodRental : _MethodRental
	{
		public const int JitImmediate = 1;

		public const int JitOnDemand = 0;

		private MethodRental()
		{
		}

		void _MethodRental.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodRental.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodRental.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodRental.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void SwapMethodBody(Type cls, int methodtoken, IntPtr rgIL, int methodSize, int flags)
		{
			if (methodSize <= 0 || methodSize >= 4128768)
			{
				throw new ArgumentException("Data size must be > 0 and < 0x3f0000", "methodSize");
			}
			if (cls == null)
			{
				throw new ArgumentNullException("cls");
			}
			if (cls is TypeBuilder && !((TypeBuilder)cls).is_created)
			{
				throw new NotSupportedException(string.Concat("Type '", cls, "' is not yet created."));
			}
			throw new NotImplementedException();
		}
	}
}
