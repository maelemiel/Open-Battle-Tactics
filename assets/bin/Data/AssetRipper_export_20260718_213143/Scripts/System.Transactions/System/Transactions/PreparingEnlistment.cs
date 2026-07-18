namespace System.Transactions
{
	public class PreparingEnlistment : Enlistment
	{
		private bool prepared;

		private Transaction tx;

		private IEnlistmentNotification enlisted;

		internal bool IsPrepared
		{
			get
			{
				return prepared;
			}
		}

		internal PreparingEnlistment(Transaction tx, IEnlistmentNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
		}

		public void ForceRollback()
		{
			ForceRollback(null);
		}

		[System.MonoTODO]
		public void ForceRollback(Exception ex)
		{
			tx.Rollback(ex, enlisted);
		}

		[System.MonoTODO]
		public void Prepared()
		{
			prepared = true;
		}

		[System.MonoTODO]
		public byte[] RecoveryInformation()
		{
			throw new NotImplementedException();
		}
	}
}
