using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricKeyExchangeFormatter
	{
		public abstract string Parameters { get; }

		public abstract byte[] CreateKeyExchange(byte[] data);

		public abstract byte[] CreateKeyExchange(byte[] data, Type symAlgType);

		public abstract void SetKey(AsymmetricAlgorithm key);
	}
}
