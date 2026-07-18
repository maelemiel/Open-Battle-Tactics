using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public enum PaddingMode
	{
		None = 1,
		PKCS7 = 2,
		Zeros = 3,
		ANSIX923 = 4,
		ISO10126 = 5
	}
}
