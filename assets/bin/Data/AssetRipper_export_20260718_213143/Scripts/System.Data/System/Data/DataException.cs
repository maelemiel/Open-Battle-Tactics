using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DataException : SystemException
	{
		public DataException()
			: base(global::Locale.GetText("A Data exception has occurred"))
		{
		}

		public DataException(string s)
			: base(s)
		{
		}

		protected DataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public DataException(string s, Exception innerException)
			: base(s, innerException)
		{
		}
	}
}
