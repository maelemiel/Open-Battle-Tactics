using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class RSAOAEPKeyExchangeDeformatter : AsymmetricKeyExchangeDeformatter
	{
		private RSA rsa;

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

		public RSAOAEPKeyExchangeDeformatter()
		{
		}

		public RSAOAEPKeyExchangeDeformatter(AsymmetricAlgorithm key)
		{
			SetKey(key);
		}

		public override byte[] DecryptKeyExchange(byte[] rgbData)
		{
			if (rsa == null)
			{
				string text = Locale.GetText("No RSA key specified");
				throw new CryptographicUnexpectedOperationException(text);
			}
			SHA1 hash = SHA1.Create();
			byte[] array = PKCS1.Decrypt_OAEP(rsa, hash, rgbData);
			if (array != null)
			{
				return array;
			}
			throw new CryptographicException(Locale.GetText("OAEP decoding error."));
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			rsa = (RSA)key;
		}
	}
}
