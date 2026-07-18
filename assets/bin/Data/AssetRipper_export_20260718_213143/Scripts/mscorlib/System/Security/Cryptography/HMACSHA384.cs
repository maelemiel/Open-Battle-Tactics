using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA384 : HMAC
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

		public HMACSHA384()
			: this(KeyBuilder.Key(8))
		{
			ProduceLegacyHmacValues = legacy_mode;
		}

		public HMACSHA384(byte[] key)
		{
			ProduceLegacyHmacValues = legacy_mode;
			base.HashName = "SHA384";
			HashSizeValue = 384;
			Key = key;
		}

		static HMACSHA384()
		{
			legacy_mode = Environment.GetEnvironmentVariable("legacyHMACMode") == "1";
		}
	}
}
