using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class DSASignatureFormatter : AsymmetricSignatureFormatter
	{
		private DSA dsa;

		public DSASignatureFormatter()
		{
		}

		public DSASignatureFormatter(AsymmetricAlgorithm key)
		{
			SetKey(key);
		}

		public override byte[] CreateSignature(byte[] rgbHash)
		{
			if (dsa == null)
			{
				throw new CryptographicUnexpectedOperationException(Locale.GetText("missing key"));
			}
			return dsa.CreateSignature(rgbHash);
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
	}
}
