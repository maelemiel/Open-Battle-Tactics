using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class KeyedHashAlgorithm : HashAlgorithm
	{
		protected byte[] KeyValue;

		public virtual byte[] Key
		{
			get
			{
				return (byte[])KeyValue.Clone();
			}
			set
			{
				if (State != 0)
				{
					throw new CryptographicException(Locale.GetText("Key can't be changed at this state."));
				}
				ZeroizeKey();
				KeyValue = (byte[])value.Clone();
			}
		}

		~KeyedHashAlgorithm()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			ZeroizeKey();
			base.Dispose(disposing);
		}

		private void ZeroizeKey()
		{
			if (KeyValue != null)
			{
				Array.Clear(KeyValue, 0, KeyValue.Length);
			}
		}

		public new static KeyedHashAlgorithm Create()
		{
			return Create("System.Security.Cryptography.KeyedHashAlgorithm");
		}

		public new static KeyedHashAlgorithm Create(string algName)
		{
			return (KeyedHashAlgorithm)CryptoConfig.CreateFromName(algName);
		}
	}
}
