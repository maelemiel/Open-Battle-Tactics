using System;

namespace UnityEngine
{
	[Flags]
	public enum EventModifiers
	{
		Shift = 1,
		Control = 2,
		Alt = 4,
		Command = 8,
		Numeric = 0x10,
		CapsLock = 0x20,
		FunctionKey = 0x40
	}
}
