namespace System.Transactions
{
	[System.MonoTODO]
	public static class TransactionInterop
	{
		[System.MonoTODO]
		public static IDtcTransaction GetDtcTransaction(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static byte[] GetExportCookie(Transaction transaction, byte[] exportCookie)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static Transaction GetTransactionFromDtcTransaction(IDtcTransaction dtc)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static Transaction GetTransactionFromExportCookie(byte[] exportCookie)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static Transaction GetTransactionFromTransmitterPropagationToken(byte[] token)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static byte[] GetTransmitterPropagationToken(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static byte[] GetWhereabouts()
		{
			throw new NotImplementedException();
		}
	}
}
