namespace System.Security.Cryptography
{
	public abstract class Aes : SymmetricAlgorithm
	{
		protected Aes()
		{
			KeySizeValue = 256;
			BlockSizeValue = 128;
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(128, 128, 0);
		}

		public new static Aes Create()
		{
			return Create("System.Security.Cryptography.AesManaged, System.Core, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
		}

		public new static Aes Create(string algName)
		{
			return (Aes)CryptoConfig.CreateFromName(algName);
		}
	}
}
