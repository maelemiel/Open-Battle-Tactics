using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class StrongTypingException : DataException
	{
		public StrongTypingException()
			: base(global::Locale.GetText("Trying to access a DBNull value in a strongly-typed DataSet"))
		{
		}

		public StrongTypingException(string message)
			: base(message)
		{
		}

		public StrongTypingException(string s, Exception innerException)
			: base(s, innerException)
		{
		}

		protected StrongTypingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
