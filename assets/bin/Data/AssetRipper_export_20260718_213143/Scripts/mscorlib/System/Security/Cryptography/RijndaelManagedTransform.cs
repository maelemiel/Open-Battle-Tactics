using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RijndaelManagedTransform : IDisposable, ICryptoTransform
	{
		private RijndaelTransform _st;

		private int _bs;

		public int BlockSizeValue
		{
			get
			{
				return _bs;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return _st.CanTransformMultipleBlocks;
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return _st.CanReuseTransform;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return _st.InputBlockSize;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return _st.OutputBlockSize;
			}
		}

		internal RijndaelManagedTransform(Rijndael algo, bool encryption, byte[] key, byte[] iv)
		{
			_st = new RijndaelTransform(algo, encryption, key, iv);
			_bs = algo.BlockSize;
		}

		void IDisposable.Dispose()
		{
			_st.Clear();
		}

		public void Clear()
		{
			_st.Clear();
		}

		[MonoTODO("Reset does nothing since CanReuseTransform return false.")]
		public void Reset()
		{
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			return _st.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			return _st.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
		}
	}
}
