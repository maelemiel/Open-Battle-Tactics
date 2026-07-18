namespace System.Transactions
{
	public class SinglePhaseEnlistment : Enlistment
	{
		private Transaction tx;

		private ISinglePhaseNotification enlisted;

		internal SinglePhaseEnlistment(Transaction tx, ISinglePhaseNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
		}

		public void Aborted()
		{
			Aborted(null);
		}

		public void Aborted(Exception e)
		{
			tx.Rollback(e, enlisted);
		}

		[System.MonoTODO]
		public void Committed()
		{
		}

		[System.MonoTODO("Not implemented")]
		public void InDoubt()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO("Not implemented")]
		public void InDoubt(Exception e)
		{
			throw new NotImplementedException();
		}
	}
}
