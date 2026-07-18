using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class DesignerTransactionCloseEventArgs : EventArgs
	{
		private bool commit;

		private bool last_transaction;

		public bool LastTransaction
		{
			get
			{
				return last_transaction;
			}
		}

		public bool TransactionCommitted
		{
			get
			{
				return commit;
			}
		}

		public DesignerTransactionCloseEventArgs(bool commit, bool lastTransaction)
		{
			this.commit = commit;
			last_transaction = lastTransaction;
		}

		[Obsolete("Use another constructor that indicates lastTransaction")]
		public DesignerTransactionCloseEventArgs(bool commit)
		{
			this.commit = commit;
		}
	}
}
