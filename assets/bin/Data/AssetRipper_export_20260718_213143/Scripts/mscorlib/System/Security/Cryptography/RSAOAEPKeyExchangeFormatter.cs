using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAOAEPKeyExchangeFormatter : AsymmetricKeyExchangeFormatter
	{
		private RSA rsa;

		private RandomNumberGenerator random;

		private byte[] param;

		public byte[] Parameter
		{
			get
			{
				return param;
			}
			set
			{
				param = value;
			}
		}

		public override string Parameters
		{
			get
			{
				return null;
			}
		}

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

		public RSAOAEPKeyExchangeFormatter()
		{
			rsa = null;
		}

		public RSAOAEPKeyExchangeFormatter(AsymmetricAlgorithm key)
		{
			SetKey(key);
		}

		public override byte[] CreateKeyExchange(byte[] rgbData)
		{
			if (random == null)
			{
				random = RandomNumberGenerator.Create();
			}
			if (rsa == null)
			{
				string text = Locale.GetText("No RSA key specified");
				throw new CryptographicUnexpectedOperationException(text);
			}
			SHA1 hash = SHA1.Create();
			return PKCS1.Encrypt_OAEP(rsa, hash, random, rgbData);
		}

		public override byte[] CreateKeyExchange(byte[] rgbData, Type symAlgType)
		{
			return CreateKeyExchange(rgbData);
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			rsa = (RSA)key;
		}
	}
}
