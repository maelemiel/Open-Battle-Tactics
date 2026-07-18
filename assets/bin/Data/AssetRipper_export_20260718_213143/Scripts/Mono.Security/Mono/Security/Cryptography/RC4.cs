using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	public abstract class RC4 : SymmetricAlgorithm
	{
		private static KeySizes[] s_legalBlockSizes = new KeySizes[1]
		{
			new KeySizes(64, 64, 0)
		};

		private static KeySizes[] s_legalKeySizes = new KeySizes[1]
		{
			new KeySizes(40, 2048, 8)
		};

		public override byte[] IV
		{
			get
			{
				return new byte[0];
			}
			set
			{
			}
		}

		public RC4()
		{
			KeySizeValue = 128;
			BlockSizeValue = 64;
			FeedbackSizeValue = BlockSizeValue;
			LegalBlockSizesValue = s_legalBlockSizes;
			LegalKeySizesValue = s_legalKeySizes;
		}

		public new static RC4 Create()
		{
			return Create("RC4");
		}

		public new static RC4 Create(string algName)
		{
			object obj = CryptoConfig.CreateFromName(algName);
			if (obj == null)
			{
				obj = new ARC4Managed();
			}
			return (RC4)obj;
		}
	}
}
