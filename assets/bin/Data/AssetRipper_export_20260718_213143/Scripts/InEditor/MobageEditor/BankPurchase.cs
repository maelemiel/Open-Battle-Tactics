using System;
using System.Collections.Generic;

namespace MobageEditor
{
	public class BankPurchase
	{
		public delegate void createTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void closeTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void continueTransaction_onCompleteCallback(CancelableAPIStatus status, Error error, Transaction transaction);

		public delegate void cancelTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void getTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void getUnfinishedItemTransactions_onCompleteCallback(SimpleAPIStatus status, Error error, List<Transaction> transactions);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();
	}
}
