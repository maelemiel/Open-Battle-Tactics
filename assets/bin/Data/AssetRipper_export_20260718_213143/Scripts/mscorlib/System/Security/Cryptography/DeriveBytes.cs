using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class DeriveBytes
	{
		public abstract byte[] GetBytes(int cb);

		public abstract void Reset();
	}
}
