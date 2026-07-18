using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public interface ICryptoTransform : IDisposable
	{
		bool CanReuseTransform { get; }

		bool CanTransformMultipleBlocks { get; }

		int InputBlockSize { get; }

		int OutputBlockSize { get; }

		int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

		byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
	}
}
