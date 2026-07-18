using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACRIPEMD160 : HMAC
	{
		public HMACRIPEMD160()
			: this(KeyBuilder.Key(8))
		{
		}

		public HMACRIPEMD160(byte[] key)
		{
			base.HashName = "RIPEMD160";
			HashSizeValue = 160;
			Key = key;
		}
	}
}
