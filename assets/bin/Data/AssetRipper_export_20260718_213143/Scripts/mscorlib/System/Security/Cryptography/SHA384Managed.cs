using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SHA384Managed : SHA384
	{
		private byte[] xBuf;

		private int xBufOff;

		private ulong byteCount1;

		private ulong byteCount2;

		private ulong H1;

		private ulong H2;

		private ulong H3;

		private ulong H4;

		private ulong H5;

		private ulong H6;

		private ulong H7;

		private ulong H8;

		private ulong[] W;

		private int wOff;

		public SHA384Managed()
		{
			xBuf = new byte[8];
			W = new ulong[80];
			Initialize(false);
		}

		private void Initialize(bool reuse)
		{
			H1 = 14680500436340154072uL;
			H2 = 7105036623409894663uL;
			H3 = 10473403895298186519uL;
			H4 = 1526699215303891257uL;
			H5 = 7436329637833083697uL;
			H6 = 10282925794625328401uL;
			H7 = 15784041429090275239uL;
			H8 = 5167115440072839076uL;
			if (reuse)
			{
				byteCount1 = 0uL;
				byteCount2 = 0uL;
				xBufOff = 0;
				for (int i = 0; i < xBuf.Length; i++)
				{
					xBuf[i] = 0;
				}
				wOff = 0;
				for (int j = 0; j != W.Length; j++)
				{
					W[j] = 0uL;
				}
			}
		}

		public override void Initialize()
		{
			Initialize(true);
		}

		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			while (xBufOff != 0 && cbSize > 0)
			{
				update(rgb[ibStart]);
				ibStart++;
				cbSize--;
			}
			while (cbSize > xBuf.Length)
			{
				processWord(rgb, ibStart);
				ibStart += xBuf.Length;
				cbSize -= xBuf.Length;
				byteCount1 += (ulong)xBuf.Length;
			}
			while (cbSize > 0)
			{
				update(rgb[ibStart]);
				ibStart++;
				cbSize--;
			}
		}

		protected override byte[] HashFinal()
		{
			adjustByteCounts();
			ulong lowW = byteCount1 << 3;
			ulong hiW = byteCount2;
			update(128);
			while (xBufOff != 0)
			{
				update(0);
			}
			processLength(lowW, hiW);
			processBlock();
			byte[] array = new byte[48];
			unpackWord(H1, array, 0);
			unpackWord(H2, array, 8);
			unpackWord(H3, array, 16);
			unpackWord(H4, array, 24);
			unpackWord(H5, array, 32);
			unpackWord(H6, array, 40);
			Initialize();
			return array;
		}

		private void update(byte input)
		{
			xBuf[xBufOff++] = input;
			if (xBufOff == xBuf.Length)
			{
				processWord(xBuf, 0);
				xBufOff = 0;
			}
			byteCount1++;
		}

		private void processWord(byte[] input, int inOff)
		{
			W[wOff++] = ((ulong)input[inOff] << 56) | ((ulong)input[inOff + 1] << 48) | ((ulong)input[inOff + 2] << 40) | ((ulong)input[inOff + 3] << 32) | ((ulong)input[inOff + 4] << 24) | ((ulong)input[inOff + 5] << 16) | ((ulong)input[inOff + 6] << 8) | input[inOff + 7];
			if (wOff == 16)
			{
				processBlock();
			}
		}

		private void unpackWord(ulong word, byte[] output, int outOff)
		{
			output[outOff] = (byte)(word >> 56);
			output[outOff + 1] = (byte)(word >> 48);
			output[outOff + 2] = (byte)(word >> 40);
			output[outOff + 3] = (byte)(word >> 32);
			output[outOff + 4] = (byte)(word >> 24);
			output[outOff + 5] = (byte)(word >> 16);
			output[outOff + 6] = (byte)(word >> 8);
			output[outOff + 7] = (byte)word;
		}

		private void adjustByteCounts()
		{
			if (byteCount1 > 2305843009213693951L)
			{
				byteCount2 += byteCount1 >> 61;
				byteCount1 &= 2305843009213693951uL;
			}
		}

		private void processLength(ulong lowW, ulong hiW)
		{
			if (wOff > 14)
			{
				processBlock();
			}
			W[14] = hiW;
			W[15] = lowW;
		}

		private void processBlock()
		{
			ulong[] w = W;
			ulong[] k = SHAConstants.K2;
			adjustByteCounts();
			ulong num;
			ulong num2;
			for (int i = 16; i <= 79; i++)
			{
				num = w[i - 15];
				num = ((num >> 1) | (num << 63)) ^ ((num >> 8) | (num << 56)) ^ (num >> 7);
				num2 = w[i - 2];
				num2 = ((num2 >> 19) | (num2 << 45)) ^ ((num2 >> 61) | (num2 << 3)) ^ (num2 >> 6);
				w[i] = num2 + w[i - 7] + num + w[i - 16];
			}
			num = H1;
			num2 = H2;
			ulong num3 = H3;
			ulong num4 = H4;
			ulong num5 = H5;
			ulong num6 = H6;
			ulong num7 = H7;
			ulong num8 = H8;
			for (int j = 0; j <= 79; j++)
			{
				ulong num9 = ((num5 >> 14) | (num5 << 50)) ^ ((num5 >> 18) | (num5 << 46)) ^ ((num5 >> 41) | (num5 << 23));
				num9 += num8 + ((num5 & num6) ^ (~num5 & num7)) + k[j] + w[j];
				ulong num10 = ((num >> 28) | (num << 36)) ^ ((num >> 34) | (num << 30)) ^ ((num >> 39) | (num << 25));
				num10 += (num & num2) ^ (num & num3) ^ (num2 & num3);
				num8 = num7;
				num7 = num6;
				num6 = num5;
				num5 = num4 + num9;
				num4 = num3;
				num3 = num2;
				num2 = num;
				num = num9 + num10;
			}
			H1 += num;
			H2 += num2;
			H3 += num3;
			H4 += num4;
			H5 += num5;
			H6 += num6;
			H7 += num7;
			H8 += num8;
			wOff = 0;
			for (int l = 0; l != w.Length; l++)
			{
				w[l] = 0uL;
			}
		}
	}
}
