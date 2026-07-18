using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RC2CryptoServiceProvider : RC2
	{
		private bool _useSalt;

		public override int EffectiveKeySize
		{
			get
			{
				return base.EffectiveKeySize;
			}
			set
			{
				if (value != KeySizeValue)
				{
					throw new CryptographicUnexpectedOperationException(Locale.GetText("Effective key size must match key size for compatibility"));
				}
				base.EffectiveKeySize = value;
			}
		}

		[ComVisible(false)]
		[MonoTODO("Use salt in algorithm")]
		public bool UseSalt
		{
			get
			{
				return _useSalt;
			}
			set
			{
				_useSalt = value;
			}
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RC2Transform(this, false, rgbKey, rgbIV);
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return new RC2Transform(this, true, rgbKey, rgbIV);
		}

		public override void GenerateIV()
		{
			IVValue = KeyBuilder.IV(BlockSizeValue >> 3);
		}

		public override void GenerateKey()
		{
			KeyValue = KeyBuilder.Key(KeySizeValue >> 3);
		}
	}
}
