using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DeletedRowInaccessibleException : DataException
	{
		public DeletedRowInaccessibleException()
			: base(global::Locale.GetText("This DataRow has been deleted"))
		{
		}

		public DeletedRowInaccessibleException(string s)
			: base(s)
		{
		}

		public DeletedRowInaccessibleException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DeletedRowInaccessibleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
