using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SHA1Managed : SHA1
	{
		private SHA1Internal sha;

		public SHA1Managed()
		{
			sha = new SHA1Internal();
		}

		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			State = 1;
			sha.HashCore(rgb, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			State = 0;
			return sha.HashFinal();
		}

		public override void Initialize()
		{
			sha.Initialize();
		}
	}
}
