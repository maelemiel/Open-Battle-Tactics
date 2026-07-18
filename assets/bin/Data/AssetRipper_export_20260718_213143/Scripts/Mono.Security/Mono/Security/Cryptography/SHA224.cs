using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	public abstract class SHA224 : HashAlgorithm
	{
		public SHA224()
		{
			HashSizeValue = 224;
		}

		public new static SHA224 Create()
		{
			return Create("SHA224");
		}

		public new static SHA224 Create(string hashName)
		{
			object obj = CryptoConfig.CreateFromName(hashName);
			if (obj == null)
			{
				obj = new SHA224Managed();
			}
			return (SHA224)obj;
		}
	}
}
