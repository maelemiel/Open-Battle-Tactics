using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class NoNullAllowedException : DataException
	{
		public NoNullAllowedException()
			: base(global::Locale.GetText("Cannot insert a NULL value"))
		{
		}

		public NoNullAllowedException(string s)
			: base(s)
		{
		}

		public NoNullAllowedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NoNullAllowedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
