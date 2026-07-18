using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class ConstraintException : DataException
	{
		public ConstraintException()
			: base(global::Locale.GetText("This operation violates a constraint"))
		{
		}

		public ConstraintException(string s)
			: base(s)
		{
		}

		public ConstraintException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ConstraintException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
