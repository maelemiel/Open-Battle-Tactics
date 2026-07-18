using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InvalidExpressionException : DataException
	{
		public InvalidExpressionException()
			: base(global::Locale.GetText("This Expression is invalid"))
		{
		}

		public InvalidExpressionException(string s)
			: base(s)
		{
		}

		public InvalidExpressionException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidExpressionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
