using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SHA256Managed : SHA256
	{
		private const int BLOCK_SIZE_BYTES = 64;

		private const int HASH_SIZE_BYTES = 32;

		private uint[] _H;

		private ulong count;

		private byte[] _ProcessingBuffer;

		private int _ProcessingBufferCount;

		private uint[] buff;

		public SHA256Managed()
		{
			_H = new uint[8];
			_ProcessingBuffer = new byte[64];
			buff = new uint[64];
			Initialize();
		}

		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			State = 1;
			if (_ProcessingBufferCount != 0)
			{
				if (cbSize < 64 - _ProcessingBufferCount)
				{
					Buffer.BlockCopy(rgb, ibStart, _ProcessingBuffer, _ProcessingBufferCount, cbSize);
					_ProcessingBufferCount += cbSize;
					return;
				}
				int num = 64 - _ProcessingBufferCount;
				Buffer.BlockCopy(rgb, ibStart, _ProcessingBuffer, _ProcessingBufferCount, num);
				ProcessBlock(_ProcessingBuffer, 0);
				_ProcessingBufferCount = 0;
				ibStart += num;
				cbSize -= num;
			}
			for (int num = 0; num < cbSize - cbSize % 64; num += 64)
			{
				ProcessBlock(rgb, ibStart + num);
			}
			if (cbSize % 64 != 0)
			{
				Buffer.BlockCopy(rgb, cbSize - cbSize % 64 + ibStart, _ProcessingBuffer, 0, cbSize % 64);
				_ProcessingBufferCount = cbSize % 64;
			}
		}

		protected override byte[] HashFinal()
		{
			byte[] array = new byte[32];
			ProcessFinalBlock(_ProcessingBuffer, 0, _ProcessingBufferCount);
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i * 4 + j] = (byte)(_H[i] >> 24 - j * 8);
				}
			}
			State = 0;
			return array;
		}

		public override void Initialize()
		{
			count = 0uL;
			_ProcessingBufferCount = 0;
			_H[0] = 1779033703u;
			_H[1] = 3144134277u;
			_H[2] = 1013904242u;
			_H[3] = 2773480762u;
			_H[4] = 1359893119u;
			_H[5] = 2600822924u;
			_H[6] = 528734635u;
			_H[7] = 1541459225u;
		}

		private void ProcessBlock(byte[] inputBuffer, int inputOffset)
		{
			uint[] k = SHAConstants.K1;
			uint[] array = buff;
			count += 64uL;
			for (int i = 0; i < 16; i++)
			{
				array[i] = (uint)((inputBuffer[inputOffset + 4 * i] << 24) | (inputBuffer[inputOffset + 4 * i + 1] << 16) | (inputBuffer[inputOffset + 4 * i + 2] << 8) | inputBuffer[inputOffset + 4 * i + 3]);
			}
			for (int i = 16; i < 64; i++)
			{
				uint num = array[i - 15];
				num = ((num >> 7) | (num << 25)) ^ ((num >> 18) | (num << 14)) ^ (num >> 3);
				uint num2 = array[i - 2];
				num2 = ((num2 >> 17) | (num2 << 15)) ^ ((num2 >> 19) | (num2 << 13)) ^ (num2 >> 10);
				array[i] = num2 + array[i - 7] + num + array[i - 16];
			}
			uint num3 = _H[0];
			uint num4 = _H[1];
			uint num5 = _H[2];
			uint num6 = _H[3];
			uint num7 = _H[4];
			uint num8 = _H[5];
			uint num9 = _H[6];
			uint num10 = _H[7];
			for (int i = 0; i < 64; i++)
			{
				uint num = num10 + (((num7 >> 6) | (num7 << 26)) ^ ((num7 >> 11) | (num7 << 21)) ^ ((num7 >> 25) | (num7 << 7))) + ((num7 & num8) ^ (~num7 & num9)) + k[i] + array[i];
				uint num2 = ((num3 >> 2) | (num3 << 30)) ^ ((num3 >> 13) | (num3 << 19)) ^ ((num3 >> 22) | (num3 << 10));
				num2 += (num3 & num4) ^ (num3 & num5) ^ (num4 & num5);
				num10 = num9;
				num9 = num8;
				num8 = num7;
				num7 = num6 + num;
				num6 = num5;
				num5 = num4;
				num4 = num3;
				num3 = num + num2;
			}
			_H[0] += num3;
			_H[1] += num4;
			_H[2] += num5;
			_H[3] += num6;
			_H[4] += num7;
			_H[5] += num8;
			_H[6] += num9;
			_H[7] += num10;
		}

		private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ulong num = count + (ulong)inputCount;
			int num2 = 56 - (int)(num % 64);
			if (num2 < 1)
			{
				num2 += 64;
			}
			byte[] array = new byte[inputCount + num2 + 8];
			for (int i = 0; i < inputCount; i++)
			{
				array[i] = inputBuffer[i + inputOffset];
			}
			array[inputCount] = 128;
			for (int j = inputCount + 1; j < inputCount + num2; j++)
			{
				array[j] = 0;
			}
			ulong length = num << 3;
			AddLength(length, array, inputCount + num2);
			ProcessBlock(array, 0);
			if (inputCount + num2 + 8 == 128)
			{
				ProcessBlock(array, 64);
			}
		}

		internal void AddLength(ulong length, byte[] buffer, int position)
		{
			buffer[position++] = (byte)(length >> 56);
			buffer[position++] = (byte)(length >> 48);
			buffer[position++] = (byte)(length >> 40);
			buffer[position++] = (byte)(length >> 32);
			buffer[position++] = (byte)(length >> 24);
			buffer[position++] = (byte)(length >> 16);
			buffer[position++] = (byte)(length >> 8);
			buffer[position] = (byte)length;
		}
	}
}
