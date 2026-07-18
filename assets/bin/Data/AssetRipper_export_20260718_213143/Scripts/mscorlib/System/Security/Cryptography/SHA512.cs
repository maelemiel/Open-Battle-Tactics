using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class SHA512 : HashAlgorithm
	{
		protected SHA512()
		{
			HashSizeValue = 512;
		}

		public new static SHA512 Create()
		{
			return Create("System.Security.Cryptography.SHA512");
		}

		public new static SHA512 Create(string hashName)
		{
			return (SHA512)CryptoConfig.CreateFromName(hashName);
		}
	}
}
