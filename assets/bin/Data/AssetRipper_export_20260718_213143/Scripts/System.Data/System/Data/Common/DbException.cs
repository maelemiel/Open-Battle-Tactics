using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Data.Common
{
	[Serializable]
	public abstract class DbException : ExternalException
	{
		protected DbException()
		{
		}

		protected DbException(string message)
			: base(message)
		{
		}

		protected DbException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DbException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected DbException(string message, int errorcode)
			: base(message, errorcode)
		{
		}
	}
}
