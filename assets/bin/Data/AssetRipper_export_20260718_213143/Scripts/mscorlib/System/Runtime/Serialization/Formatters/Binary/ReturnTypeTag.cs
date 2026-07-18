namespace System.Runtime.Serialization.Formatters.Binary
{
	internal enum ReturnTypeTag : byte
	{
		Null = 2,
		PrimitiveType = 8,
		ObjectType = 0x10,
		Exception = 0x20
	}
}
