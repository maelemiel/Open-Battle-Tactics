using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RijndaelManaged : Rijndael
	{
		public override void GenerateIV()
		{
			IVValue = KeyBuilder.IV(BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			KeyValue = KeyBuilder.Key(KeySizeValue >> 3);
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RijndaelManagedTransform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RijndaelManagedTransform(this, true, rgbKey, rgbIV);
		}
	}
}
