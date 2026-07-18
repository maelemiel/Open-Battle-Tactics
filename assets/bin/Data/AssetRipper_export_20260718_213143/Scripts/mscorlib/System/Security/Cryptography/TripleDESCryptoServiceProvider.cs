using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class TripleDESCryptoServiceProvider : TripleDES
	{
		public override void GenerateIV()
		{
			IVValue = KeyBuilder.IV(BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			KeyValue = TripleDESTransform.GetStrongKey();
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new TripleDESTransform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new TripleDESTransform(this, true, rgbKey, rgbIV);
		}
	}
}
