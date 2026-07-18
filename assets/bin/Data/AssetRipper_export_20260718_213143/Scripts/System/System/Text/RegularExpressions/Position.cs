namespace System.Text.RegularExpressions
{
	internal enum Position : ushort
	{
		Any = 0,
		Start = 1,
		StartOfString = 2,
		StartOfLine = 3,
		StartOfScan = 4,
		End = 5,
		EndOfString = 6,
		EndOfLine = 7,
		Boundary = 8,
		NonBoundary = 9
	}
}
