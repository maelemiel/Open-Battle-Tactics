namespace System.Transactions
{
	public interface IPromotableSinglePhaseNotification : ITransactionPromoter
	{
		void Initialize();

		void Rollback(SinglePhaseEnlistment enlistment);

		void SinglePhaseCommit(SinglePhaseEnlistment enlistment);
	}
}
