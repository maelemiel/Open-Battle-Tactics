namespace System.Security.Cryptography
{
	internal enum AsnDecodeStatus
	{
		NotDecoded = -1,
		Ok = 0,
		BadAsn = 1,
		BadTag = 2,
		BadLength = 3,
		InformationNotAvailable = 4
	}
}
