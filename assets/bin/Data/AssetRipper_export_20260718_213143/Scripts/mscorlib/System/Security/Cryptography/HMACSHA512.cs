using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA512 : HMAC
	{
		private static bool legacy_mode;

		private bool legacy;

		public bool ProduceLegacyHmacValues
		{
			get
			{
				return legacy;
			}
			set
			{
				legacy = value;
				base.BlockSizeValue = ((!legacy) ? 128 : 64);
			}
		}

		public HMACSHA512()
			: this(KeyBuilder.Key(8))
		{
			ProduceLegacyHmacValues = legacy_mode;
		}

		public HMACSHA512(byte[] key)
		{
			ProduceLegacyHmacValues = legacy_mode;
			base.HashName = "SHA512";
			HashSizeValue = 512;
			Key = key;
		}

		static HMACSHA512()
		{
			legacy_mode = Environment.GetEnvironmentVariable("legacyHMACMode") == "1";
		}
	}
}
