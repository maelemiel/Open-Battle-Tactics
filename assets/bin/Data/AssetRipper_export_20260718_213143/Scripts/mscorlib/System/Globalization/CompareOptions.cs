using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum CompareOptions
	{
		None = 0,
		IgnoreCase = 1,
		IgnoreNonSpace = 2,
		IgnoreSymbols = 4,
		IgnoreKanaType = 8,
		IgnoreWidth = 0x10,
		StringSort = 0x20000000,
		Ordinal = 0x40000000,
		OrdinalIgnoreCase = 0x10000000
	}
}
