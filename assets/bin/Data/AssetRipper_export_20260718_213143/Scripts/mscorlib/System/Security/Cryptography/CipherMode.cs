using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public enum CipherMode
	{
		CBC = 1,
		ECB = 2,
		OFB = 3,
		CFB = 4,
		CTS = 5
	}
}
