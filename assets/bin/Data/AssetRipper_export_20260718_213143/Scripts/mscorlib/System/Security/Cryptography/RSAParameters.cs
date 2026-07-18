using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public struct RSAParameters
	{
		[NonSerialized]
		public byte[] P;

		[NonSerialized]
		public byte[] Q;

		[NonSerialized]
		public byte[] D;

		[NonSerialized]
		public byte[] DP;

		[NonSerialized]
		public byte[] DQ;

		[NonSerialized]
		public byte[] InverseQ;

		public byte[] Modulus;

		public byte[] Exponent;
	}
}
