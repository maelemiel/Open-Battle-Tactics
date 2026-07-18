using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public class LockRecursionException : Exception
	{
		public LockRecursionException()
		{
		}

		public LockRecursionException(string message)
			: base(message)
		{
		}

		public LockRecursionException(string message, Exception e)
			: base(message, e)
		{
		}

		protected LockRecursionException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}
	}
}
