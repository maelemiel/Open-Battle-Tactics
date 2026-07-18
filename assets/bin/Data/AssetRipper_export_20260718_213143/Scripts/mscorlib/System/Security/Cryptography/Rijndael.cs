using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class Rijndael : SymmetricAlgorithm
	{
		protected Rijndael()
		{
			KeySizeValue = 256;
			BlockSizeValue = 128;
			FeedbackSizeValue = 128;
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(128, 256, 64);
		}

		public new static Rijndael Create()
		{
			return Create("System.Security.Cryptography.Rijndael");
		}

		public new static Rijndael Create(string algName)
		{
			return (Rijndael)CryptoConfig.CreateFromName(algName);
		}
	}
}
