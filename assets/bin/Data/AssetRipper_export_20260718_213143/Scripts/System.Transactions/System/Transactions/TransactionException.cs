using System.Runtime.Serialization;

namespace System.Transactions
{
	[Serializable]
	public class TransactionException : SystemException
	{
		protected TransactionException()
		{
		}

		public TransactionException(string message)
			: base(message)
		{
		}

		public TransactionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected TransactionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
