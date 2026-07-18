using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CryptoAPITransform : IDisposable, ICryptoTransform
	{
		private bool m_disposed;

		public bool CanReuseTransform
		{
			get
			{
				return true;
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
				return 0;
			}
		}

		public IntPtr KeyHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 0;
			}
		}

		internal CryptoAPITransform()
		{
			m_disposed = false;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (disposing)
				{
				}
				m_disposed = true;
			}
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			return 0;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			return null;
		}

		[ComVisible(false)]
		public void Reset()
		{
		}
	}
}
