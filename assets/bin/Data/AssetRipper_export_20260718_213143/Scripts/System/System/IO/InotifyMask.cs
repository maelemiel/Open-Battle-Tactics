namespace System.IO
{
	[Flags]
	internal enum InotifyMask : uint
	{
		Access = 1u,
		Modify = 2u,
		Attrib = 4u,
		CloseWrite = 8u,
		CloseNoWrite = 0x10u,
		Open = 0x20u,
		MovedFrom = 0x40u,
		MovedTo = 0x80u,
		Create = 0x100u,
		Delete = 0x200u,
		DeleteSelf = 0x400u,
		MoveSelf = 0x800u,
		BaseEvents = 0xFFFu,
		Umount = 0x2000u,
		Overflow = 0x4000u,
		Ignored = 0x8000u,
		OnlyDir = 0x1000000u,
		DontFollow = 0x2000000u,
		AddMask = 0x20000000u,
		Directory = 0x40000000u,
		OneShot = 0x80000000u
	}
}
