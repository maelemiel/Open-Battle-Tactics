using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InRowChangingEventException : DataException
	{
		public InRowChangingEventException()
			: base(global::Locale.GetText("Cannot EndEdit within a RowChanging event"))
		{
		}

		public InRowChangingEventException(string s)
			: base(s)
		{
		}

		public InRowChangingEventException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InRowChangingEventException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
