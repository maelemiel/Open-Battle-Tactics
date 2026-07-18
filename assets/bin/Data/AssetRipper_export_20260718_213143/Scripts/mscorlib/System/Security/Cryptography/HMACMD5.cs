using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACMD5 : HMAC
	{
		public HMACMD5()
			: this(KeyBuilder.Key(8))
		{
		}

		public HMACMD5(byte[] key)
		{
			base.HashName = "MD5";
			HashSizeValue = 128;
			Key = key;
		}
	}
}
