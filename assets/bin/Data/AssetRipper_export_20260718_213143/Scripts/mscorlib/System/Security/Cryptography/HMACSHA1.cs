using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA1 : HMAC
	{
		public HMACSHA1()
			: this(KeyBuilder.Key(8))
		{
		}

		public HMACSHA1(byte[] key)
		{
			base.HashName = "SHA1";
			HashSizeValue = 160;
			Key = key;
		}

		public HMACSHA1(byte[] key, bool useManagedSha1)
		{
			base.HashName = "System.Security.Cryptography.SHA1" + ((!useManagedSha1) ? "CryptoServiceProvider" : "Managed");
			HashSizeValue = 160;
			Key = key;
		}
	}
}
