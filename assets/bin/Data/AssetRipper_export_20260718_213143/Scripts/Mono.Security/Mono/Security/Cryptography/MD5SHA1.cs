using System;
using System.Security.Cryptography;
using Mono.Security.Protocol.Tls;

namespace Mono.Security.Cryptography
{
	internal class MD5SHA1 : HashAlgorithm
	{
		private HashAlgorithm md5;

		private HashAlgorithm sha;

		private bool hashing;

		public MD5SHA1()
		{
			md5 = MD5.Create();
			sha = SHA1.Create();
			HashSizeValue = md5.HashSize + sha.HashSize;
		}

		public override void Initialize()
		{
			md5.Initialize();
			sha.Initialize();
			hashing = false;
		}

		protected override byte[] HashFinal()
		{
			if (!hashing)
			{
				hashing = true;
			}
			md5.TransformFinalBlock(new byte[0], 0, 0);
			sha.TransformFinalBlock(new byte[0], 0, 0);
			byte[] array = new byte[36];
			Buffer.BlockCopy(md5.Hash, 0, array, 0, 16);
			Buffer.BlockCopy(sha.Hash, 0, array, 16, 20);
			return array;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			if (!hashing)
			{
				hashing = true;
			}
			md5.TransformBlock(array, ibStart, cbSize, array, ibStart);
			sha.TransformBlock(array, ibStart, cbSize, array, ibStart);
		}

		public byte[] CreateSignature(RSA rsa)
		{
			if (rsa == null)
			{
				throw new CryptographicUnexpectedOperationException("missing key");
			}
			RSASslSignatureFormatter rSASslSignatureFormatter = new RSASslSignatureFormatter(rsa);
			rSASslSignatureFormatter.SetHashAlgorithm("MD5SHA1");
			return rSASslSignatureFormatter.CreateSignature(Hash);
		}

		public bool VerifySignature(RSA rsa, byte[] rgbSignature)
		{
			if (rsa == null)
			{
				throw new CryptographicUnexpectedOperationException("missing key");
			}
			if (rgbSignature == null)
			{
				throw new ArgumentNullException("rgbSignature");
			}
			RSASslSignatureDeformatter rSASslSignatureDeformatter = new RSASslSignatureDeformatter(rsa);
			rSASslSignatureDeformatter.SetHashAlgorithm("MD5SHA1");
			return rSASslSignatureDeformatter.VerifySignature(Hash, rgbSignature);
		}
	}
}
