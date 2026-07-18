using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal class MACAlgorithm
	{
		private SymmetricAlgorithm algo;

		private ICryptoTransform enc;

		private byte[] block;

		private int blockSize;

		private int blockCount;

		public MACAlgorithm(SymmetricAlgorithm algorithm)
		{
			algo = algorithm;
			algo.Mode = CipherMode.CBC;
			blockSize = algo.BlockSize >> 3;
			algo.IV = new byte[blockSize];
			block = new byte[blockSize];
		}

		public void Initialize(byte[] key)
		{
			algo.Key = key;
			if (enc == null)
			{
				enc = algo.CreateEncryptor();
			}
			Array.Clear(block, 0, blockSize);
			blockCount = 0;
		}

		public void Core(byte[] rgb, int ib, int cb)
		{
			int num = System.Math.Min(blockSize - blockCount, cb);
			Array.Copy(rgb, ib, block, blockCount, num);
			blockCount += num;
			if (blockCount == blockSize)
			{
				enc.TransformBlock(block, 0, blockSize, block, 0);
				int num2 = (cb - num) / blockSize;
				for (int i = 0; i < num2; i++)
				{
					enc.TransformBlock(rgb, num, blockSize, block, 0);
					num += blockSize;
				}
				blockCount = cb - num;
				if (blockCount > 0)
				{
					Array.Copy(rgb, num, block, 0, blockCount);
				}
			}
		}

		public byte[] Final()
		{
			byte[] result = ((blockCount <= 0 && (algo.Padding == PaddingMode.Zeros || algo.Padding == PaddingMode.None)) ? ((byte[])block.Clone()) : enc.TransformFinalBlock(block, 0, blockCount));
			if (!enc.CanReuseTransform)
			{
				enc.Dispose();
				enc = null;
			}
			return result;
		}
	}
}
