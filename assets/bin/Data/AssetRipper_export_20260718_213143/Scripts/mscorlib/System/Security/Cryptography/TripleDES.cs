using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class TripleDES : SymmetricAlgorithm
	{
		public override byte[] Key
		{
			get
			{
				if (KeyValue == null)
				{
					GenerateKey();
					while (IsWeakKey(KeyValue))
					{
						GenerateKey();
					}
				}
				return (byte[])KeyValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Key");
				}
				if (IsWeakKey(value))
				{
					throw new CryptographicException(Locale.GetText("Weak Key"));
				}
				KeyValue = (byte[])value.Clone();
			}
		}

		protected TripleDES()
		{
			KeySizeValue = 192;
			BlockSizeValue = 64;
			FeedbackSizeValue = 8;
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(128, 192, 64);
			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(64, 64, 0);
		}

		public static bool IsWeakKey(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new CryptographicException(Locale.GetText("Null Key"));
			}
			if (rgbKey.Length == 16)
			{
				for (int i = 0; i < 8; i++)
				{
					if (rgbKey[i] != rgbKey[i + 8])
					{
						return false;
					}
				}
			}
			else
			{
				if (rgbKey.Length != 24)
				{
					throw new CryptographicException(Locale.GetText("Wrong Key Length"));
				}
				bool flag = true;
				for (int j = 0; j < 8; j++)
				{
					if (rgbKey[j] != rgbKey[j + 8])
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					for (int k = 8; k < 16; k++)
					{
						if (rgbKey[k] != rgbKey[k + 8])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public new static TripleDES Create()
		{
			return Create("System.Security.Cryptography.TripleDES");
		}

		public new static TripleDES Create(string str)
		{
			return (TripleDES)CryptoConfig.CreateFromName(str);
		}
	}
}
