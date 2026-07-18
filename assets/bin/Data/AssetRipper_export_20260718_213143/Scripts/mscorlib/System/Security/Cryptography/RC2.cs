using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class RC2 : SymmetricAlgorithm
	{
		protected int EffectiveKeySizeValue;

		public virtual int EffectiveKeySize
		{
			get
			{
				if (EffectiveKeySizeValue == 0)
				{
					return KeySizeValue;
				}
				return EffectiveKeySizeValue;
			}
			set
			{
				EffectiveKeySizeValue = value;
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
				EffectiveKeySizeValue = value;
			}
		}

		protected RC2()
		{
			KeySizeValue = 128;
			BlockSizeValue = 64;
			FeedbackSizeValue = 8;
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(40, 128, 8);
			LegalBlockSizesValue = new KeySizes[1];
			LegalBlockSizesValue[0] = new KeySizes(64, 64, 0);
		}

		public new static RC2 Create()
		{
			return Create("System.Security.Cryptography.RC2");
		}

		public new static RC2 Create(string AlgName)
		{
			return (RC2)CryptoConfig.CreateFromName(AlgName);
		}
	}
}
