namespace System.Text.RegularExpressions
{
	internal enum OpCode : ushort
	{
		False = 0,
		True = 1,
		Position = 2,
		String = 3,
		Reference = 4,
		Character = 5,
		Category = 6,
		NotCategory = 7,
		Range = 8,
		Set = 9,
		In = 10,
		Open = 11,
		Close = 12,
		Balance = 13,
		BalanceStart = 14,
		IfDefined = 15,
		Sub = 16,
		Test = 17,
		Branch = 18,
		Jump = 19,
		Repeat = 20,
		Until = 21,
		FastRepeat = 22,
		Anchor = 23,
		Info = 24
	}
}
