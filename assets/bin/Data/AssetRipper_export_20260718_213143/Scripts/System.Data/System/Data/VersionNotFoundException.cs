using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class VersionNotFoundException : DataException
	{
		public VersionNotFoundException()
			: base(global::Locale.GetText("This DataRow has been deleted"))
		{
		}

		public VersionNotFoundException(string s)
			: base(s)
		{
		}

		protected VersionNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public VersionNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
