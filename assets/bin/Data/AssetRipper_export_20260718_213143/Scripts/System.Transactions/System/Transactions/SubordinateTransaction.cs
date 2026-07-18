namespace System.Transactions
{
	[Serializable]
	public sealed class SubordinateTransaction : Transaction
	{
		public SubordinateTransaction(IsolationLevel level, ISimpleTransactionSuperior superior)
		{
			throw new NotImplementedException();
		}
	}
}
