using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class FromBase64Transform : IDisposable, ICryptoTransform
	{
		private const byte TerminatorByte = 61;

		private FromBase64TransformMode mode;

		private byte[] accumulator;

		private int accPtr;

		private bool m_disposed;

		private byte[] lookupTable;

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 3;
			}
		}

		public FromBase64Transform()
			: this(FromBase64TransformMode.IgnoreWhiteSpaces)
		{
		}

		public FromBase64Transform(FromBase64TransformMode whitespaces)
		{
			mode = whitespaces;
			accumulator = new byte[4];
			accPtr = 0;
			m_disposed = false;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FromBase64Transform()
		{
			Dispose(false);
		}

		public void Clear()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (accumulator != null)
				{
					Array.Clear(accumulator, 0, accumulator.Length);
				}
				if (disposing)
				{
					accumulator = null;
				}
				m_disposed = true;
			}
		}

		private byte lookup(byte input)
		{
			if (input >= lookupTable.Length)
			{
				throw new FormatException(Locale.GetText("Invalid character in a Base-64 string."));
			}
			byte b = lookupTable[input];
			if (b == byte.MaxValue)
			{
				throw new FormatException(Locale.GetText("Invalid character in a Base-64 string."));
			}
			return b;
		}

		private int ProcessBlock(byte[] output, int offset)
		{
			int num = 0;
			if (accumulator[3] == 61)
			{
				num++;
			}
			if (accumulator[2] == 61)
			{
				num++;
			}
			lookupTable = Base64Constants.DecodeTable;
			switch (num)
			{
			case 0:
			{
				int num2 = lookup(accumulator[0]);
				int num3 = lookup(accumulator[1]);
				int num4 = lookup(accumulator[2]);
				int num5 = lookup(accumulator[3]);
				output[offset++] = (byte)((num2 << 2) | (num3 >> 4));
				output[offset++] = (byte)((num3 << 4) | (num4 >> 2));
				output[offset] = (byte)((num4 << 6) | num5);
				break;
			}
			case 1:
			{
				int num2 = lookup(accumulator[0]);
				int num3 = lookup(accumulator[1]);
				int num4 = lookup(accumulator[2]);
				output[offset++] = (byte)((num2 << 2) | (num3 >> 4));
				output[offset] = (byte)((num3 << 4) | (num4 >> 2));
				break;
			}
			case 2:
			{
				int num2 = lookup(accumulator[0]);
				int num3 = lookup(accumulator[1]);
				output[offset] = (byte)((num2 << 2) | (num3 >> 4));
				break;
			}
			}
			return 3 - num;
		}

		private void CheckInputParameters(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset", "< 0");
			}
			if (inputCount > inputBuffer.Length)
			{
				throw new OutOfMemoryException("inputCount " + Locale.GetText("Overflow"));
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputOffset", Locale.GetText("Overflow"));
			}
			if (inputCount < 0)
			{
				throw new OverflowException("inputCount < 0");
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("FromBase64Transform");
			}
			CheckInputParameters(inputBuffer, inputOffset, inputCount);
			if (outputBuffer == null || outputOffset < 0)
			{
				throw new FormatException("outputBuffer");
			}
			int num = 0;
			while (inputCount > 0)
			{
				if (accPtr < 4)
				{
					byte b = inputBuffer[inputOffset++];
					if (mode == FromBase64TransformMode.IgnoreWhiteSpaces)
					{
						if (!char.IsWhiteSpace((char)b))
						{
							accumulator[accPtr++] = b;
						}
					}
					else
					{
						accumulator[accPtr++] = b;
					}
				}
				if (accPtr == 4)
				{
					num += ProcessBlock(outputBuffer, outputOffset);
					outputOffset += 3;
					accPtr = 0;
				}
				inputCount--;
			}
			return num;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("FromBase64Transform");
			}
			CheckInputParameters(inputBuffer, inputOffset, inputCount);
			int num = 0;
			int num2 = 0;
			if (mode == FromBase64TransformMode.IgnoreWhiteSpaces)
			{
				int num3 = inputOffset;
				for (int i = 0; i < inputCount; i++)
				{
					if (char.IsWhiteSpace((char)inputBuffer[num3]))
					{
						num++;
					}
					num3++;
				}
				if (num == inputCount)
				{
					return new byte[0];
				}
				int num4 = inputOffset + inputCount - 1;
				int num5 = Math.Min(2, inputCount);
				while (num5 > 0)
				{
					char c = (char)inputBuffer[num4--];
					if (c == '=')
					{
						num2++;
						num5--;
					}
					else if (!char.IsWhiteSpace(c))
					{
						break;
					}
				}
			}
			else
			{
				if (inputBuffer[inputOffset + inputCount - 1] == 61)
				{
					num2++;
				}
				if (inputBuffer[inputOffset + inputCount - 2] == 61)
				{
					num2++;
				}
			}
			if (inputCount < 4 && num2 < 2)
			{
				if (accPtr > 2 && accumulator[3] == 61)
				{
					num2++;
				}
				if (accPtr > 1 && accumulator[2] == 61)
				{
					num2++;
				}
			}
			int num6 = (accPtr + inputCount - num >> 2) * 3 - num2;
			if (num6 <= 0)
			{
				return new byte[0];
			}
			byte[] array = new byte[num6];
			TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}
	}
}
