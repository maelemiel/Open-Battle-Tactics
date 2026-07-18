using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	public sealed class AesManaged : Aes
	{
		public override byte[] IV
		{
			get
			{
				return base.IV;
			}
			set
			{
				base.IV = value;
			}
		}

		public override byte[] Key
		{
			get
			{
				return base.Key;
			}
			set
			{
				base.Key = value;
			}
		}

		public override int KeySize
		{
			get
			{
				return base.KeySize;
			}
			set
			{
				base.KeySize = value;
			}
		}

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
			return new AesTransform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new AesTransform(this, true, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateDecryptor()
		{
			return CreateDecryptor(Key, IV);
		}

		public override ICryptoTransform CreateEncryptor()
		{
			return CreateEncryptor(Key, IV);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
