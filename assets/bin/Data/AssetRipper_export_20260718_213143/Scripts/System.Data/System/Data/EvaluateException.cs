using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class EvaluateException : InvalidExpressionException
	{
		public EvaluateException()
			: base(global::Locale.GetText("This expression cannot be evaluated"))
		{
		}

		public EvaluateException(string s)
			: base(s)
		{
		}

		public EvaluateException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected EvaluateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
