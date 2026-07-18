using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal abstract class SymmetricTransform : IDisposable, ICryptoTransform
	{
		protected SymmetricAlgorithm algo;

		protected bool encrypt;

		private int BlockSizeByte;

		private byte[] temp;

		private byte[] temp2;

		private byte[] workBuff;

		private byte[] workout;

		private int FeedBackByte;

		private int FeedBackIter;

		private bool m_disposed;

		private bool lastBlock;

		private RandomNumberGenerator _rng;

		public virtual bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		public virtual bool CanReuseTransform
		{
			get
			{
				return false;
			}
		}

		public virtual int InputBlockSize
		{
			get
			{
				return BlockSizeByte;
			}
		}

		public virtual int OutputBlockSize
		{
			get
			{
				return BlockSizeByte;
			}
		}

		private bool KeepLastBlock
		{
			get
			{
				return !encrypt && algo.Padding != PaddingMode.None && algo.Padding != PaddingMode.Zeros;
			}
		}

		public SymmetricTransform(SymmetricAlgorithm symmAlgo, bool encryption, byte[] rgbIV)
		{
			algo = symmAlgo;
			encrypt = encryption;
			BlockSizeByte = algo.BlockSize >> 3;
			rgbIV = ((rgbIV != null) ? ((byte[])rgbIV.Clone()) : KeyBuilder.IV(BlockSizeByte));
			if (rgbIV.Length < BlockSizeByte)
			{
				string text = global::Locale.GetText("IV is too small ({0} bytes), it should be {1} bytes long.", rgbIV.Length, BlockSizeByte);
				throw new CryptographicException(text);
			}
			temp = new byte[BlockSizeByte];
			Buffer.BlockCopy(rgbIV, 0, temp, 0, System.Math.Min(BlockSizeByte, rgbIV.Length));
			temp2 = new byte[BlockSizeByte];
			FeedBackByte = algo.FeedbackSize >> 3;
			if (FeedBackByte != 0)
			{
				FeedBackIter = BlockSizeByte / FeedBackByte;
			}
			workBuff = new byte[BlockSizeByte];
			workout = new byte[BlockSizeByte];
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SymmetricTransform()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (disposing)
				{
					Array.Clear(temp, 0, BlockSizeByte);
					temp = null;
					Array.Clear(temp2, 0, BlockSizeByte);
					temp2 = null;
				}
				m_disposed = true;
			}
		}

		protected virtual void Transform(byte[] input, byte[] output)
		{
			switch (algo.Mode)
			{
			case CipherMode.ECB:
				ECB(input, output);
				break;
			case CipherMode.CBC:
				CBC(input, output);
				break;
			case CipherMode.CFB:
				CFB(input, output);
				break;
			case CipherMode.OFB:
				OFB(input, output);
				break;
			case CipherMode.CTS:
				CTS(input, output);
				break;
			default:
				throw new NotImplementedException("Unkown CipherMode" + algo.Mode);
			}
		}

		protected abstract void ECB(byte[] input, byte[] output);

		protected virtual void CBC(byte[] input, byte[] output)
		{
			if (encrypt)
			{
				for (int i = 0; i < BlockSizeByte; i++)
				{
					temp[i] ^= input[i];
				}
				ECB(temp, output);
				Buffer.BlockCopy(output, 0, temp, 0, BlockSizeByte);
				return;
			}
			Buffer.BlockCopy(input, 0, temp2, 0, BlockSizeByte);
			ECB(input, output);
			for (int j = 0; j < BlockSizeByte; j++)
			{
				output[j] ^= temp[j];
			}
			Buffer.BlockCopy(temp2, 0, temp, 0, BlockSizeByte);
		}

		protected virtual void CFB(byte[] input, byte[] output)
		{
			if (encrypt)
			{
				for (int i = 0; i < FeedBackIter; i++)
				{
					ECB(temp, temp2);
					for (int j = 0; j < FeedBackByte; j++)
					{
						output[j + i] = (byte)(temp2[j] ^ input[j + i]);
					}
					Buffer.BlockCopy(temp, FeedBackByte, temp, 0, BlockSizeByte - FeedBackByte);
					Buffer.BlockCopy(output, i, temp, BlockSizeByte - FeedBackByte, FeedBackByte);
				}
				return;
			}
			for (int k = 0; k < FeedBackIter; k++)
			{
				encrypt = true;
				ECB(temp, temp2);
				encrypt = false;
				Buffer.BlockCopy(temp, FeedBackByte, temp, 0, BlockSizeByte - FeedBackByte);
				Buffer.BlockCopy(input, k, temp, BlockSizeByte - FeedBackByte, FeedBackByte);
				for (int l = 0; l < FeedBackByte; l++)
				{
					output[l + k] = (byte)(temp2[l] ^ input[l + k]);
				}
			}
		}

		protected virtual void OFB(byte[] input, byte[] output)
		{
			throw new CryptographicException("OFB isn't supported by the framework");
		}

		protected virtual void CTS(byte[] input, byte[] output)
		{
			throw new CryptographicException("CTS isn't supported by the framework");
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

		public virtual int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("Object is disposed");
			}
			CheckInput(inputBuffer, inputOffset, inputCount);
			if (outputBuffer == null)
			{
				throw new ArgumentNullException("outputBuffer");
			}
			if (outputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("outputOffset", "< 0");
			}
			int num = outputBuffer.Length - inputCount - outputOffset;
			if (!encrypt && 0 > num && (algo.Padding == PaddingMode.None || algo.Padding == PaddingMode.Zeros))
			{
				throw new CryptographicException("outputBuffer", global::Locale.GetText("Overflow"));
			}
			if (KeepLastBlock)
			{
				if (0 > num + BlockSizeByte)
				{
					throw new CryptographicException("outputBuffer", global::Locale.GetText("Overflow"));
				}
			}
			else if (0 > num)
			{
				if (inputBuffer.Length - inputOffset - outputBuffer.Length != BlockSizeByte)
				{
					throw new CryptographicException("outputBuffer", global::Locale.GetText("Overflow"));
				}
				inputCount = outputBuffer.Length - outputOffset;
			}
			return InternalTransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		private int InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			int num = inputOffset;
			int num2;
			if (inputCount != BlockSizeByte)
			{
				if (inputCount % BlockSizeByte != 0)
				{
					throw new CryptographicException("Invalid input block size.");
				}
				num2 = inputCount / BlockSizeByte;
			}
			else
			{
				num2 = 1;
			}
			if (KeepLastBlock)
			{
				num2--;
			}
			int num3 = 0;
			if (lastBlock)
			{
				Transform(workBuff, workout);
				Buffer.BlockCopy(workout, 0, outputBuffer, outputOffset, BlockSizeByte);
				outputOffset += BlockSizeByte;
				num3 += BlockSizeByte;
				lastBlock = false;
			}
			for (int i = 0; i < num2; i++)
			{
				Buffer.BlockCopy(inputBuffer, num, workBuff, 0, BlockSizeByte);
				Transform(workBuff, workout);
				Buffer.BlockCopy(workout, 0, outputBuffer, outputOffset, BlockSizeByte);
				num += BlockSizeByte;
				outputOffset += BlockSizeByte;
				num3 += BlockSizeByte;
			}
			if (KeepLastBlock)
			{
				Buffer.BlockCopy(inputBuffer, num, workBuff, 0, BlockSizeByte);
				lastBlock = true;
			}
			return num3;
		}

		private void Random(byte[] buffer, int start, int length)
		{
			if (_rng == null)
			{
				_rng = RandomNumberGenerator.Create();
			}
			byte[] array = new byte[length];
			_rng.GetBytes(array);
			Buffer.BlockCopy(array, 0, buffer, start, length);
		}

		private void ThrowBadPaddingException(PaddingMode padding, int length, int position)
		{
			string text = string.Format(global::Locale.GetText("Bad {0} padding."), padding);
			if (length >= 0)
			{
				text += string.Format(global::Locale.GetText(" Invalid length {0}."), length);
			}
			if (position >= 0)
			{
				text += string.Format(global::Locale.GetText(" Error found at position {0}."), position);
			}
			throw new CryptographicException(text);
		}

		private byte[] FinalEncrypt(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			int num = inputCount / BlockSizeByte * BlockSizeByte;
			int num2 = inputCount - num;
			int num3 = num;
			switch (algo.Padding)
			{
			case PaddingMode.PKCS7:
			case PaddingMode.ANSIX923:
			case PaddingMode.ISO10126:
				num3 += BlockSizeByte;
				break;
			default:
				if (inputCount == 0)
				{
					return new byte[0];
				}
				if (num2 != 0)
				{
					if (algo.Padding == PaddingMode.None)
					{
						throw new CryptographicException("invalid block length");
					}
					byte[] array = new byte[num + BlockSizeByte];
					Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
					inputBuffer = array;
					inputOffset = 0;
					inputCount = array.Length;
					num3 = inputCount;
				}
				break;
			}
			byte[] array2 = new byte[num3];
			int num4 = 0;
			while (num3 > BlockSizeByte)
			{
				InternalTransformBlock(inputBuffer, inputOffset, BlockSizeByte, array2, num4);
				inputOffset += BlockSizeByte;
				num4 += BlockSizeByte;
				num3 -= BlockSizeByte;
			}
			byte b = (byte)(BlockSizeByte - num2);
			switch (algo.Padding)
			{
			case PaddingMode.ANSIX923:
				array2[array2.Length - 1] = b;
				Buffer.BlockCopy(inputBuffer, inputOffset, array2, num, num2);
				InternalTransformBlock(array2, num, BlockSizeByte, array2, num);
				break;
			case PaddingMode.ISO10126:
				Random(array2, array2.Length - b, b - 1);
				array2[array2.Length - 1] = b;
				Buffer.BlockCopy(inputBuffer, inputOffset, array2, num, num2);
				InternalTransformBlock(array2, num, BlockSizeByte, array2, num);
				break;
			case PaddingMode.PKCS7:
			{
				int num5 = array2.Length;
				while (--num5 >= array2.Length - b)
				{
					array2[num5] = b;
				}
				Buffer.BlockCopy(inputBuffer, inputOffset, array2, num, num2);
				InternalTransformBlock(array2, num, BlockSizeByte, array2, num);
				break;
			}
			default:
				InternalTransformBlock(inputBuffer, inputOffset, BlockSizeByte, array2, num4);
				break;
			}
			return array2;
		}

		private byte[] FinalDecrypt(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputCount % BlockSizeByte > 0)
			{
				throw new CryptographicException("Invalid input block size.");
			}
			int num = inputCount;
			if (lastBlock)
			{
				num += BlockSizeByte;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			while (inputCount > 0)
			{
				int num3 = InternalTransformBlock(inputBuffer, inputOffset, BlockSizeByte, array, num2);
				inputOffset += BlockSizeByte;
				num2 += num3;
				inputCount -= BlockSizeByte;
			}
			if (lastBlock)
			{
				Transform(workBuff, workout);
				Buffer.BlockCopy(workout, 0, array, num2, BlockSizeByte);
				num2 += BlockSizeByte;
				lastBlock = false;
			}
			byte b = (byte)((num > 0) ? array[num - 1] : 0);
			switch (algo.Padding)
			{
			case PaddingMode.ANSIX923:
			{
				if (b == 0 || b > BlockSizeByte)
				{
					ThrowBadPaddingException(algo.Padding, b, -1);
				}
				for (int num5 = b - 1; num5 > 0; num5--)
				{
					if (array[num - 1 - num5] != 0)
					{
						ThrowBadPaddingException(algo.Padding, -1, num5);
					}
				}
				num -= b;
				break;
			}
			case PaddingMode.ISO10126:
				if (b == 0 || b > BlockSizeByte)
				{
					ThrowBadPaddingException(algo.Padding, b, -1);
				}
				num -= b;
				break;
			case PaddingMode.PKCS7:
			{
				if (b == 0 || b > BlockSizeByte)
				{
					ThrowBadPaddingException(algo.Padding, b, -1);
				}
				for (int num4 = b - 1; num4 > 0; num4--)
				{
					if (array[num - 1 - num4] != b)
					{
						ThrowBadPaddingException(algo.Padding, -1, num4);
					}
				}
				num -= b;
				break;
			}
			}
			if (num > 0)
			{
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(array, 0, array2, 0, num);
				Array.Clear(array, 0, array.Length);
				return array2;
			}
			return new byte[0];
		}

		public virtual byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("Object is disposed");
			}
			CheckInput(inputBuffer, inputOffset, inputCount);
			if (encrypt)
			{
				return FinalEncrypt(inputBuffer, inputOffset, inputCount);
			}
			return FinalDecrypt(inputBuffer, inputOffset, inputCount);
		}
	}
}
