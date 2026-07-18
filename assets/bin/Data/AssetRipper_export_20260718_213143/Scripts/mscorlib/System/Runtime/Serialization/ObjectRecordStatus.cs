namespace System.Runtime.Serialization
{
	internal enum ObjectRecordStatus : byte
	{
		Unregistered = 0,
		ReferenceUnsolved = 1,
		ReferenceSolvingDelayed = 2,
		ReferenceSolved = 3
	}
}
