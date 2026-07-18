using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlAlreadyFilledException : SqlTypeException
	{
		public SqlAlreadyFilledException()
			: base(global::Locale.GetText("A SqlAlreadyFilled exception has occured."))
		{
		}

		public SqlAlreadyFilledException(string message)
			: base(message)
		{
		}

		public SqlAlreadyFilledException(string message, Exception inner)
			: base(message, inner)
		{
		}

		private new void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("SqlAlreadyFilledExceptionMessage", Message, typeof(string));
		}
	}
}
