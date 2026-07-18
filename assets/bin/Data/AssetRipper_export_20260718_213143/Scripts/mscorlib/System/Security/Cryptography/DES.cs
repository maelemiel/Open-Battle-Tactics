using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class DES : SymmetricAlgorithm
	{
		private const int keySizeByte = 8;

		internal static readonly byte[,] weakKeys = new byte[4, 8]
		{
			{ 1, 1, 1, 1, 1, 1, 1, 1 },
			{ 31, 31, 31, 31, 15, 15, 15, 15 },
			{ 225, 225, 225, 225, 241, 241, 241, 241 },
			{ 255, 255, 255, 255, 255, 255, 255, 255 }
		};

		internal static readonly byte[,] semiWeakKeys = new byte[12, 8]
		{
			{ 0, 30, 0, 30, 0, 14, 0, 14 },
			{ 0, 224, 0, 224, 0, 240, 0, 240 },
			{ 0, 254, 0, 254, 0, 254, 0, 254 },
			{ 30, 0, 30, 0, 14, 0, 14, 0 },
			{ 30, 224, 30, 224, 14, 240, 14, 240 },
			{ 30, 254, 30, 254, 14, 254, 14, 254 },
			{ 224, 0, 224, 0, 240, 0, 240, 0 },
			{ 224, 30, 224, 30, 240, 14, 240, 14 },
			{ 224, 254, 224, 254, 240, 254, 240, 254 },
			{ 254, 0, 254, 0, 254, 0, 254, 0 },
			{ 254, 30, 254, 30, 254, 14, 254, 14 },
			{ 254, 224, 254, 224, 254, 240, 254, 240 }
		};

		public override byte[] Key
		{
			get
			{
				if (KeyValue == null)
				{
					GenerateKey();
				}
				return (byte[])KeyValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Key");
				}
				if (value.Length != 8)
				{
					throw new ArgumentException(Locale.GetText("Wrong Key Length"));
				}
				if (IsWeakKey(value))
				{
					throw new CryptographicException(Locale.GetText("Weak Key"));
				}
				if (IsSemiWeakKey(value))
				{
					throw new CryptographicException(Locale.GetText("Semi Weak Key"));
				}
				KeyValue = (byte[])value.Clone();
			}
		}

		protected DES()
		{
			KeySizeValue = 64;
			BlockSizeValue = 64;
			FeedbackSizeValue = 8;
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(64, 64, 0);
			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(64, 64, 0);
		}

		public new static DES Create()
		{
			return Create("System.Security.Cryptography.DES");
		}

		public new static DES Create(string algName)
		{
			return (DES)CryptoConfig.CreateFromName(algName);
		}

		public static bool IsWeakKey(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new CryptographicException(Locale.GetText("Null Key"));
			}
			if (rgbKey.Length != 8)
			{
				throw new CryptographicException(Locale.GetText("Wrong Key Length"));
			}
			for (int i = 0; i < rgbKey.Length; i++)
			{
				int num = rgbKey[i] | 0x11;
				if (num != 17 && num != 31 && num != 241 && num != 255)
				{
					return false;
				}
			}
			for (int j = 0; j < weakKeys.Length >> 3; j++)
			{
				int k;
				for (k = 0; k < rgbKey.Length && (rgbKey[k] ^ weakKeys[j, k]) <= 1; k++)
				{
				}
				if (k == 8)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsSemiWeakKey(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new CryptographicException(Locale.GetText("Null Key"));
			}
			if (rgbKey.Length != 8)
			{
				throw new CryptographicException(Locale.GetText("Wrong Key Length"));
			}
			for (int i = 0; i < rgbKey.Length; i++)
			{
				int num = rgbKey[i] | 0x11;
				if (num != 17 && num != 31 && num != 241 && num != 255)
				{
					return false;
				}
			}
			for (int j = 0; j < semiWeakKeys.Length >> 3; j++)
			{
				int k;
				for (k = 0; k < rgbKey.Length && (rgbKey[k] ^ semiWeakKeys[j, k]) <= 1; k++)
				{
				}
				if (k == 8)
				{
					return true;
				}
			}
			return false;
		}
	}
}
