using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class MissingPrimaryKeyException : DataException
	{
		public MissingPrimaryKeyException()
			: base(global::Locale.GetText("This table has no primary key"))
		{
		}

		public MissingPrimaryKeyException(string s)
			: base(s)
		{
		}

		public MissingPrimaryKeyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected MissingPrimaryKeyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
