using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InvalidConstraintException : DataException
	{
		public InvalidConstraintException()
			: base(global::Locale.GetText("Cannot access or create this relation"))
		{
		}

		public InvalidConstraintException(string s)
			: base(s)
		{
		}

		public InvalidConstraintException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidConstraintException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
