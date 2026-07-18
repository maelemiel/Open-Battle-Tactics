using System;
using System.Security.Cryptography;

namespace Mono.Security.Protocol.Tls
{
	internal class SslHandshakeHash : HashAlgorithm
	{
		private HashAlgorithm md5;

		private HashAlgorithm sha;

		private bool hashing;

		private byte[] secret;

		private byte[] innerPadMD5;

		private byte[] outerPadMD5;

		private byte[] innerPadSHA;

		private byte[] outerPadSHA;

		public SslHandshakeHash(byte[] secret)
		{
			md5 = HashAlgorithm.Create("MD5");
			sha = HashAlgorithm.Create("SHA1");
			HashSizeValue = md5.HashSize + sha.HashSize;
			this.secret = secret;
			Initialize();
		}

		public override void Initialize()
		{
			md5.Initialize();
			sha.Initialize();
			initializePad();
			hashing = false;
		}

		protected override byte[] HashFinal()
		{
			if (!hashing)
			{
				hashing = true;
			}
			md5.TransformBlock(secret, 0, secret.Length, secret, 0);
			md5.TransformFinalBlock(innerPadMD5, 0, innerPadMD5.Length);
			byte[] hash = md5.Hash;
			md5.Initialize();
			md5.TransformBlock(secret, 0, secret.Length, secret, 0);
			md5.TransformBlock(outerPadMD5, 0, outerPadMD5.Length, outerPadMD5, 0);
			md5.TransformFinalBlock(hash, 0, hash.Length);
			sha.TransformBlock(secret, 0, secret.Length, secret, 0);
			sha.TransformFinalBlock(innerPadSHA, 0, innerPadSHA.Length);
			byte[] hash2 = sha.Hash;
			sha.Initialize();
			sha.TransformBlock(secret, 0, secret.Length, secret, 0);
			sha.TransformBlock(outerPadSHA, 0, outerPadSHA.Length, outerPadSHA, 0);
			sha.TransformFinalBlock(hash2, 0, hash2.Length);
			Initialize();
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

		private void initializePad()
		{
			innerPadMD5 = new byte[48];
			outerPadMD5 = new byte[48];
			for (int i = 0; i < 48; i++)
			{
				innerPadMD5[i] = 54;
				outerPadMD5[i] = 92;
			}
			innerPadSHA = new byte[40];
			outerPadSHA = new byte[40];
			for (int j = 0; j < 40; j++)
			{
				innerPadSHA[j] = 54;
				outerPadSHA[j] = 92;
			}
		}
	}
}
