using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlNotFilledException : SqlTypeException, ISerializable
	{
		public SqlNotFilledException()
			: base(global::Locale.GetText("A SqlNotFilled exception has occured."))
		{
		}

		public SqlNotFilledException(string message)
			: base(message)
		{
		}

		public SqlNotFilledException(string message, Exception e)
			: base(message, e)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("SqlNotFilledExceptionMessage", Message, typeof(string));
		}
	}
}
