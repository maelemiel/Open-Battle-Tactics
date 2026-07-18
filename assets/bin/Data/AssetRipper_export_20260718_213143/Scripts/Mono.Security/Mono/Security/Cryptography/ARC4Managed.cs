using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	public class ARC4Managed : RC4, IDisposable, ICryptoTransform
	{
		private byte[] key;

		private byte[] state;

		private byte x;

		private byte y;

		private bool m_disposed;

		public override byte[] Key
		{
			get
			{
				return (byte[])key.Clone();
			}
			set
			{
				key = (byte[])value.Clone();
				KeySetup(key);
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return false;
			}
		}

		public bool CanTransformMultipleBlocks
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
				return 1;
			}
		}

		public ARC4Managed()
		{
			state = new byte[256];
			m_disposed = false;
		}

		~ARC4Managed()
		{
			Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				x = 0;
				y = 0;
				if (key != null)
				{
					Array.Clear(key, 0, key.Length);
					key = null;
				}
				Array.Clear(state, 0, state.Length);
				state = null;
				GC.SuppressFinalize(this);
				m_disposed = true;
			}
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgvIV)
		{
			Key = rgbKey;
			return this;
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgvIV)
		{
			Key = rgbKey;
			return CreateEncryptor();
		}

		public override void GenerateIV()
		{
			IV = new byte[0];
		}

		public override void GenerateKey()
		{
			Key = KeyBuilder.Key(KeySizeValue >> 3);
		}

		private void KeySetup(byte[] key)
		{
			byte b = 0;
			byte b2 = 0;
			for (int i = 0; i < 256; i++)
			{
				state[i] = (byte)i;
			}
			x = 0;
			y = 0;
			for (int j = 0; j < 256; j++)
			{
				b2 = (byte)(key[b] + state[j] + b2);
				byte b3 = state[j];
				state[j] = state[b2];
				state[b2] = b3;
				b = (byte)((b + 1) % key.Length);
			}
		}

		private void CheckInput(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset", "< 0");
			}
			if (inputCount < 0)
			{
				throw new ArgumentOutOfRangeException("inputCount", "< 0");
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputBuffer", global::Locale.GetText("Overflow"));
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			CheckInput(inputBuffer, inputOffset, inputCount);
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (outputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("outputOffset", "< 0");
			}
			if (outputOffset > outputBuffer.Length - inputCount)
			{
				throw new ArgumentException("outputBuffer", global::Locale.GetText("Overflow"));
			}
			return InternalTransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		private int InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = 0; i < inputCount; i++)
			{
				x++;
				y = (byte)(state[x] + y);
				byte b = state[x];
				state[x] = state[y];
				state[y] = b;
				byte b2 = (byte)(state[x] + state[y]);
				outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ state[b2]);
			}
			return inputCount;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			CheckInput(inputBuffer, inputOffset, inputCount);
			byte[] array = new byte[inputCount];
			InternalTransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}
	}
}
