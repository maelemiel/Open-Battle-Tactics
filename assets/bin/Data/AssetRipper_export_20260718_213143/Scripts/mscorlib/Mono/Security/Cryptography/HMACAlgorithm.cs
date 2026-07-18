using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal class HMACAlgorithm
	{
		private byte[] key;

		private byte[] hash;

		private HashAlgorithm algo;

		private string hashName;

		private BlockProcessor block;

		public HashAlgorithm Algo
		{
			get
			{
				return algo;
			}
		}

		public string HashName
		{
			get
			{
				return hashName;
			}
			set
			{
				CreateHash(value);
			}
		}

		public byte[] Key
		{
			get
			{
				return key;
			}
			set
			{
				if (value != null && value.Length > 64)
				{
					key = algo.ComputeHash(value);
				}
				else
				{
					key = (byte[])value.Clone();
				}
			}
		}

		public HMACAlgorithm(string algoName)
		{
			CreateHash(algoName);
		}

		~HMACAlgorithm()
		{
			Dispose();
		}

		private void CreateHash(string algoName)
		{
			algo = HashAlgorithm.Create(algoName);
			hashName = algoName;
			block = new BlockProcessor(algo, 8);
		}

		public void Dispose()
		{
			if (key != null)
			{
				Array.Clear(key, 0, key.Length);
			}
		}

		public void Initialize()
		{
			hash = null;
			block.Initialize();
			byte[] array = KeySetup(key, 54);
			algo.Initialize();
			block.Core(array);
			Array.Clear(array, 0, array.Length);
		}

		private byte[] KeySetup(byte[] key, byte padding)
		{
			byte[] array = new byte[64];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = (byte)(key[i] ^ padding);
			}
			for (int j = key.Length; j < 64; j++)
			{
				array[j] = padding;
			}
			return array;
		}

		public void Core(byte[] rgb, int ib, int cb)
		{
			block.Core(rgb, ib, cb);
		}

		public byte[] Final()
		{
			block.Final();
			byte[] array = algo.Hash;
			byte[] array2 = KeySetup(key, 92);
			algo.Initialize();
			algo.TransformBlock(array2, 0, array2.Length, array2, 0);
			algo.TransformFinalBlock(array, 0, array.Length);
			hash = algo.Hash;
			algo.Clear();
			Array.Clear(array2, 0, array2.Length);
			Array.Clear(array, 0, array.Length);
			return hash;
		}
	}
}
