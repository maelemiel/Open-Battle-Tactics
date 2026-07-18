namespace System.Runtime.Serialization.Formatters.Binary
{
	internal enum MethodFlags
	{
		NoArguments = 1,
		PrimitiveArguments = 2,
		ArgumentsInSimpleArray = 4,
		ArgumentsInMultiArray = 8,
		ExcludeLogicalCallContext = 16,
		IncludesLogicalCallContext = 64,
		IncludesSignature = 128,
		FormatMask = 15,
		GenericArguments = 32768,
		NeedsInfoArrayMask = 32972
	}
}
