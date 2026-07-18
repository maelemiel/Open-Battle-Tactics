using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAPKCS1KeyExchangeFormatter : AsymmetricKeyExchangeFormatter
	{
		private RSA rsa;

		private RandomNumberGenerator random;

		public RandomNumberGenerator Rng
		{
			get
			{
				return random;
			}
			set
			{
				random = value;
			}
		}

		public override string Parameters
		{
			get
			{
				return "<enc:KeyEncryptionMethod enc:Algorithm=\"http://www.microsoft.com/xml/security/algorithm/PKCS1-v1.5-KeyEx\" xmlns:enc=\"http://www.microsoft.com/xml/security/encryption/v1.0\" />";
			}
		}

		public RSAPKCS1KeyExchangeFormatter()
		{
		}

		public RSAPKCS1KeyExchangeFormatter(AsymmetricAlgorithm key)
		{
			SetRSAKey(key);
		}

		public override byte[] CreateKeyExchange(byte[] rgbData)
		{
			if (rgbData == null)
			{
				throw new ArgumentNullException("rgbData");
			}
			if (rsa == null)
			{
				string text = Locale.GetText("No RSA key specified");
				throw new CryptographicUnexpectedOperationException(text);
			}
			if (random == null)
			{
				random = RandomNumberGenerator.Create();
			}
			return PKCS1.Encrypt_v15(rsa, random, rgbData);
		}

		public override byte[] CreateKeyExchange(byte[] rgbData, Type symAlgType)
		{
			return CreateKeyExchange(rgbData);
		}

		private void SetRSAKey(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			rsa = (RSA)key;
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			SetRSAKey(key);
		}
	}
}
