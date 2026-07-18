using System.Runtime.InteropServices;

namespace System.Transactions
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDtcTransaction
	{
		void Abort(IntPtr manager, int whatever, int whatever2);

		void Commit(int whatever, int whatever2, int whatever3);

		void GetTransactionInfo(IntPtr whatever);
	}
}
