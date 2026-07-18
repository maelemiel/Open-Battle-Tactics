using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class ReadOnlyException : DataException
	{
		public ReadOnlyException()
			: base(global::Locale.GetText("Cannot change a value in a read-only column"))
		{
		}

		public ReadOnlyException(string s)
			: base(s)
		{
		}

		public ReadOnlyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ReadOnlyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
