using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class DSASignatureDeformatter : AsymmetricSignatureDeformatter
	{
		private DSA dsa;

		public DSASignatureDeformatter()
		{
		}

		public DSASignatureDeformatter(AsymmetricAlgorithm key)
		{
			SetKey(key);
		}

		public override void SetHashAlgorithm(string strName)
		{
			if (strName == null)
			{
				throw new ArgumentNullException("strName");
			}
			try
			{
				SHA1.Create(strName);
			}
			catch (InvalidCastException)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("DSA requires SHA1"));
			}
		}

		public override void SetKey(AsymmetricAlgorithm key)
		{
			if (key != null)
			{
				dsa = (DSA)key;
				return;
			}
			throw new ArgumentNullException("key");
		}

		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			if (dsa == null)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("missing key"));
			}
			return dsa.VerifySignature(rgbHash, rgbSignature);
		}
	}
}
