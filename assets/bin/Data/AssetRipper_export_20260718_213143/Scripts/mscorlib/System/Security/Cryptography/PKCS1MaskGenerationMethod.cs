using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class PKCS1MaskGenerationMethod : MaskGenerationMethod
	{
		private string hashName;

		public string HashName
		{
			get
			{
				return hashName;
			}
			set
			{
				hashName = ((value != null) ? value : "SHA1");
			}
		}

		public PKCS1MaskGenerationMethod()
		{
			hashName = "SHA1";
		}

		public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
		{
			HashAlgorithm hash = HashAlgorithm.Create(hashName);
			return PKCS1.MGF1(hash, rgbSeed, cbReturn);
		}
	}
}
