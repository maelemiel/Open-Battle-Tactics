using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public struct DSAParameters
	{
		public int Counter;

		public byte[] G;

		public byte[] J;

		public byte[] P;

		public byte[] Q;

		public byte[] Seed;

		[NonSerialized]
		public byte[] X;

		public byte[] Y;
	}
}
