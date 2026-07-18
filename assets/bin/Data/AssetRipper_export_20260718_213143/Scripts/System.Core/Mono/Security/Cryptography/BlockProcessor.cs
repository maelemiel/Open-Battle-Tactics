using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	public class BlockProcessor
	{
		private ICryptoTransform transform;

		private byte[] block;

		private int blockSize;

		private int blockCount;

		public BlockProcessor(ICryptoTransform transform)
			: this(transform, transform.InputBlockSize)
		{
		}

		public BlockProcessor(ICryptoTransform transform, int blockSize)
		{
			this.transform = transform;
			this.blockSize = blockSize;
			block = new byte[blockSize];
		}

		~BlockProcessor()
		{
			Array.Clear(block, 0, blockSize);
		}

		public void Initialize()
		{
			Array.Clear(block, 0, blockSize);
			blockCount = 0;
		}

		public void Core(byte[] rgb)
		{
			Core(rgb, 0, rgb.Length);
		}

		public void Core(byte[] rgb, int ib, int cb)
		{
			int num = System.Math.Min(blockSize - blockCount, cb);
			Buffer.BlockCopy(rgb, ib, block, blockCount, num);
			blockCount += num;
			if (blockCount == blockSize)
			{
				transform.TransformBlock(block, 0, blockSize, block, 0);
				int num2 = (cb - num) / blockSize;
				for (int i = 0; i < num2; i++)
				{
					transform.TransformBlock(rgb, num + ib, blockSize, block, 0);
					num += blockSize;
				}
				blockCount = cb - num;
				if (blockCount > 0)
				{
					Buffer.BlockCopy(rgb, num + ib, block, 0, blockCount);
				}
			}
		}

		public byte[] Final()
		{
			return transform.TransformFinalBlock(block, 0, blockCount);
		}
	}
}
