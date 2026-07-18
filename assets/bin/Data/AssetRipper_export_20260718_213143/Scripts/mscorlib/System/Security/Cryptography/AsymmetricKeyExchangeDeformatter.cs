using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricKeyExchangeDeformatter
	{
		public abstract string Parameters { get; set; }

		public abstract byte[] DecryptKeyExchange(byte[] rgb);

		public abstract void SetKey(AsymmetricAlgorithm key);
	}
}
