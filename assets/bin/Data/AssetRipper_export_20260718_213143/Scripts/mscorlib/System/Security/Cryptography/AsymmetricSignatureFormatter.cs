using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricSignatureFormatter
	{
		public abstract void SetHashAlgorithm(string strName);

		public abstract void SetKey(AsymmetricAlgorithm key);

		public abstract byte[] CreateSignature(byte[] rgbHash);

		public virtual byte[] CreateSignature(HashAlgorithm hash)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			SetHashAlgorithm(hash.ToString());
			return CreateSignature(hash.Hash);
		}
	}
}
