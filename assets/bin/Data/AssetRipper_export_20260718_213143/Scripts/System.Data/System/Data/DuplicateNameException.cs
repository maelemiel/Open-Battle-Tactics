using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DuplicateNameException : DataException
	{
		public DuplicateNameException()
			: base(global::Locale.GetText("There is a database object with the same name"))
		{
		}

		public DuplicateNameException(string s)
			: base(s)
		{
		}

		public DuplicateNameException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DuplicateNameException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
