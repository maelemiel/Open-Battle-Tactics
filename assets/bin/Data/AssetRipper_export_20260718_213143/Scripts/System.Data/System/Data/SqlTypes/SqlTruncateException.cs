using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlTruncateException : SqlTypeException, ISerializable
	{
		public SqlTruncateException()
			: base(global::Locale.GetText("This value is being truncated"))
		{
		}

		public SqlTruncateException(string message)
			: base(message)
		{
		}

		public SqlTruncateException(string message, Exception e)
			: base(message, e)
		{
		}

		private SqlTruncateException(SerializationInfo si, StreamingContext sc)
			: base(si.GetString("SqlTruncateExceptionMessage"))
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("SqlTruncateExceptionMessage", Message, typeof(string));
		}
	}
}
