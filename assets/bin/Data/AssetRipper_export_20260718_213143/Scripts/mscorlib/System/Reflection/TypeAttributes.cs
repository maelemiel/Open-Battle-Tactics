using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum TypeAttributes
	{
		VisibilityMask = 7,
		NotPublic = 0,
		Public = 1,
		NestedPublic = 2,
		NestedPrivate = 3,
		NestedFamily = 4,
		NestedAssembly = 5,
		NestedFamANDAssem = 6,
		NestedFamORAssem = 7,
		LayoutMask = 0x18,
		AutoLayout = 0,
		SequentialLayout = 8,
		ExplicitLayout = 0x10,
		ClassSemanticsMask = 0x20,
		Class = 0,
		Interface = 0x20,
		Abstract = 0x80,
		Sealed = 0x100,
		SpecialName = 0x400,
		Import = 0x1000,
		Serializable = 0x2000,
		StringFormatMask = 0x30000,
		AnsiClass = 0,
		UnicodeClass = 0x10000,
		AutoClass = 0x20000,
		BeforeFieldInit = 0x100000,
		ReservedMask = 0x40800,
		RTSpecialName = 0x800,
		HasSecurity = 0x40000,
		CustomFormatClass = 0x30000,
		CustomFormatMask = 0xC00000
	}
}
