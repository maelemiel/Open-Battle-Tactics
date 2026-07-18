namespace System.Text.RegularExpressions
{
	[Flags]
	internal enum OpFlags : ushort
	{
		None = 0,
		Negate = 0x100,
		IgnoreCase = 0x200,
		RightToLeft = 0x400,
		Lazy = 0x800
	}
}
