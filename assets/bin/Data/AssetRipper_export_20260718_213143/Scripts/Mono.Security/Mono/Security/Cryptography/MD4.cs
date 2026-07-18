using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	public abstract class MD4 : HashAlgorithm
	{
		protected MD4()
		{
			HashSizeValue = 128;
		}

		public new static MD4 Create()
		{
			return Create("MD4");
		}

		public new static MD4 Create(string hashName)
		{
			object obj = CryptoConfig.CreateFromName(hashName);
			if (obj == null)
			{
				obj = new MD4Managed();
			}
			return (MD4)obj;
		}
	}
}
