using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal class HMAC : KeyedHashAlgorithm
	{
		private HashAlgorithm hash;

		private bool hashing;

		private byte[] innerPad;

		private byte[] outerPad;

		public override byte[] Key
		{
			get
			{
				return (byte[])KeyValue.Clone();
			}
			set
			{
				if (hashing)
				{
					throw new Exception("Cannot change key during hash operation.");
				}
				if (value.Length > 64)
				{
					KeyValue = hash.ComputeHash(value);
				}
				else
				{
					KeyValue = (byte[])value.Clone();
				}
				initializePad();
			}
		}

		public HMAC()
		{
			hash = MD5.Create();
			HashSizeValue = hash.HashSize;
			byte[] array = new byte[64];
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			rNGCryptoServiceProvider.GetNonZeroBytes(array);
			KeyValue = (byte[])array.Clone();
			Initialize();
		}

		public HMAC(string hashName, byte[] rgbKey)
		{
			if (hashName == null || hashName.Length == 0)
			{
				hashName = "MD5";
			}
			hash = HashAlgorithm.Create(hashName);
			HashSizeValue = hash.HashSize;
			if (rgbKey.Length > 64)
			{
				KeyValue = hash.ComputeHash(rgbKey);
			}
			else
			{
				KeyValue = (byte[])rgbKey.Clone();
			}
			Initialize();
		}

		public override void Initialize()
		{
			hash.Initialize();
			initializePad();
			hashing = false;
		}

		protected override byte[] HashFinal()
		{
			if (!hashing)
			{
				hash.TransformBlock(innerPad, 0, innerPad.Length, innerPad, 0);
				hashing = true;
			}
			hash.TransformFinalBlock(new byte[0], 0, 0);
			byte[] array = hash.Hash;
			hash.Initialize();
			hash.TransformBlock(outerPad, 0, outerPad.Length, outerPad, 0);
			hash.TransformFinalBlock(array, 0, array.Length);
			Initialize();
			return hash.Hash;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			if (!hashing)
			{
				hash.TransformBlock(innerPad, 0, innerPad.Length, innerPad, 0);
				hashing = true;
			}
			hash.TransformBlock(array, ibStart, cbSize, array, ibStart);
		}

		private void initializePad()
		{
			innerPad = new byte[64];
			outerPad = new byte[64];
			for (int i = 0; i < KeyValue.Length; i++)
			{
				innerPad[i] = (byte)(KeyValue[i] ^ 0x36);
				outerPad[i] = (byte)(KeyValue[i] ^ 0x5C);
			}
			for (int j = KeyValue.Length; j < 64; j++)
			{
				innerPad[j] = 54;
				outerPad[j] = 92;
			}
		}
	}
}
