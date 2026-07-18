using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class SyntaxErrorException : InvalidExpressionException
	{
		public SyntaxErrorException()
			: base(global::Locale.GetText("There is a syntax error in this Expression"))
		{
		}

		public SyntaxErrorException(string s)
			: base(s)
		{
		}

		protected SyntaxErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public SyntaxErrorException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
