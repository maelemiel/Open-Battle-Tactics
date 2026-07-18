using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[Serializable]
	[ComVisible(true)]
	public enum SymAddressKind
	{
		ILOffset = 1,
		NativeRVA = 2,
		NativeRegister = 3,
		NativeRegisterRelative = 4,
		NativeOffset = 5,
		NativeRegisterRegister = 6,
		NativeRegisterStack = 7,
		NativeStackRegister = 8,
		BitField = 9,
		NativeSectionOffset = 10
	}
}
