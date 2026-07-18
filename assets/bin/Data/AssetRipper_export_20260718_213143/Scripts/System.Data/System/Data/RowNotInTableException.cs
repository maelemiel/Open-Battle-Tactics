using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class RowNotInTableException : DataException
	{
		public RowNotInTableException()
			: base(global::Locale.GetText("This DataRow is not in this DataTable"))
		{
		}

		public RowNotInTableException(string s)
			: base(s)
		{
		}

		public RowNotInTableException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected RowNotInTableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
