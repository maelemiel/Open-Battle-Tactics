using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAPKCS1KeyExchangeDeformatter : AsymmetricKeyExchangeDeformatter
	{
		private RSA rsa;

		private RandomNumberGenerator random;

		public override string Parameters
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public RandomNumberGenerator RNG
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

		public RSAPKCS1KeyExchangeDeformatter()
		{
		}

		public RSAPKCS1KeyExchangeDeformatter(AsymmetricAlgorithm key)
		{
			SetKey(key);
		}

		public override byte[] DecryptKeyExchange(byte[] rgbIn)
		{
			if (rsa == null)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("No key pair available."));
			}
			byte[] array = PKCS1.Decrypt_v15(rsa, rgbIn);
			if (array != null)
			{
				return array;
			}
			throw new CryptographicException(Locale.GetText("PKCS1 decoding error."));
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			rsa = (RSA)key;
		}
	}
}
