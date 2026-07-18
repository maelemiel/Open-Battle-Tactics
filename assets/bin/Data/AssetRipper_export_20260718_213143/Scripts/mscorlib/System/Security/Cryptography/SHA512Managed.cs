using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SHA512Managed : SHA512
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

		public SHA512Managed()
		{
			xBuf = new byte[8];
			W = new ulong[80];
			Initialize(false);
		}

		private void Initialize(bool reuse)
		{
			H1 = 7640891576956012808uL;
			H2 = 13503953896175478587uL;
			H3 = 4354685564936845355uL;
			H4 = 11912009170470909681uL;
			H5 = 5840696475078001361uL;
			H6 = 11170449401992604703uL;
			H7 = 2270897969802886507uL;
			H8 = 6620516959819538809uL;
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
			byte[] array = new byte[64];
			unpackWord(H1, array, 0);
			unpackWord(H2, array, 8);
			unpackWord(H3, array, 16);
			unpackWord(H4, array, 24);
			unpackWord(H5, array, 32);
			unpackWord(H6, array, 40);
			unpackWord(H7, array, 48);
			unpackWord(H8, array, 56);
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
			adjustByteCounts();
			for (int i = 16; i <= 79; i++)
			{
				W[i] = Sigma1(W[i - 2]) + W[i - 7] + Sigma0(W[i - 15]) + W[i - 16];
			}
			ulong num = H1;
			ulong num2 = H2;
			ulong num3 = H3;
			ulong num4 = H4;
			ulong num5 = H5;
			ulong num6 = H6;
			ulong num7 = H7;
			ulong num8 = H8;
			for (int j = 0; j <= 79; j++)
			{
				ulong num9 = num8 + Sum1(num5) + Ch(num5, num6, num7) + SHAConstants.K2[j] + W[j];
				ulong num10 = Sum0(num) + Maj(num, num2, num3);
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
			for (int k = 0; k != W.Length; k++)
			{
				W[k] = 0uL;
			}
		}

		private ulong rotateRight(ulong x, int n)
		{
			return (x >> n) | (x << 64 - n);
		}

		private ulong Ch(ulong x, ulong y, ulong z)
		{
			return (x & y) ^ (~x & z);
		}

		private ulong Maj(ulong x, ulong y, ulong z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		}

		private ulong Sum0(ulong x)
		{
			return rotateRight(x, 28) ^ rotateRight(x, 34) ^ rotateRight(x, 39);
		}

		private ulong Sum1(ulong x)
		{
			return rotateRight(x, 14) ^ rotateRight(x, 18) ^ rotateRight(x, 41);
		}

		private ulong Sigma0(ulong x)
		{
			return rotateRight(x, 1) ^ rotateRight(x, 8) ^ (x >> 7);
		}

		private ulong Sigma1(ulong x)
		{
			return rotateRight(x, 19) ^ rotateRight(x, 61) ^ (x >> 6);
		}
	}
}
