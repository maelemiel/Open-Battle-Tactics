using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public class SafeArrayRankMismatchException : SystemException
	{
		private const int ErrorCode = -2146233032;

		public SafeArrayRankMismatchException()
			: base(Locale.GetText("The incoming SAVEARRAY does not match the rank of the expected managed signature"))
		{
			base.HResult = -2146233032;
		}

		public SafeArrayRankMismatchException(string message)
			: base(message)
		{
			base.HResult = -2146233032;
		}

		public SafeArrayRankMismatchException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233032;
		}

		protected SafeArrayRankMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
