using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class HashAlgorithm : IDisposable, ICryptoTransform
	{
		protected internal byte[] HashValue;

		protected int HashSizeValue;

		protected int State;

		private bool disposed;

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
				return true;
			}
		}

		public virtual byte[] Hash
		{
			get
			{
				if (HashValue == null)
				{
					throw new CryptographicUnexpectedOperationException(Locale.GetText("No hash value computed."));
				}
				return HashValue;
			}
		}

		public virtual int HashSize
		{
			get
			{
				return HashSizeValue;
			}
		}

		public virtual int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public virtual int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		protected HashAlgorithm()
		{
			disposed = false;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Dispose(true);
		}

		public byte[] ComputeHash(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return ComputeHash(buffer, 0, buffer.Length);
		}

		public byte[] ComputeHash(byte[] buffer, int offset, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("HashAlgorithm");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentException("count", "< 0");
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("offset + count", Locale.GetText("Overflow"));
			}
			HashCore(buffer, offset, count);
			HashValue = HashFinal();
			Initialize();
			return HashValue;
		}

		public byte[] ComputeHash(Stream inputStream)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("HashAlgorithm");
			}
			byte[] array = new byte[4096];
			for (int num = inputStream.Read(array, 0, 4096); num > 0; num = inputStream.Read(array, 0, 4096))
			{
				HashCore(array, 0, num);
			}
			HashValue = HashFinal();
			Initialize();
			return HashValue;
		}

		public static HashAlgorithm Create()
		{
			return Create("System.Security.Cryptography.HashAlgorithm");
		}

		public static HashAlgorithm Create(string hashName)
		{
			return (HashAlgorithm)CryptoConfig.CreateFromName(hashName);
		}

		protected abstract void HashCore(byte[] array, int ibStart, int cbSize);

		protected abstract byte[] HashFinal();

		public abstract void Initialize();

		protected virtual void Dispose(bool disposing)
		{
			disposed = true;
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
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
				throw new ArgumentException("inputCount");
			}
			if (inputOffset < 0 || inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputBuffer");
			}
			if (outputBuffer != null)
			{
				if (outputOffset < 0)
				{
					throw new ArgumentOutOfRangeException("outputOffset", "< 0");
				}
				if (outputOffset > outputBuffer.Length - inputCount)
				{
					throw new ArgumentException("outputOffset + inputCount", Locale.GetText("Overflow"));
				}
			}
			HashCore(inputBuffer, inputOffset, inputCount);
			if (outputBuffer != null)
			{
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
			}
			return inputCount;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputCount < 0)
			{
				throw new ArgumentException("inputCount");
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputOffset + inputCount", Locale.GetText("Overflow"));
			}
			byte[] array = new byte[inputCount];
			Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
			HashCore(inputBuffer, inputOffset, inputCount);
			HashValue = HashFinal();
			Initialize();
			return array;
		}
	}
}
