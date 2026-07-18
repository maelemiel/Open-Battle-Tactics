using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(false)]
	public class WaitHandleCannotBeOpenedException : ApplicationException
	{
		public WaitHandleCannotBeOpenedException()
			: base(Locale.GetText("Named handle doesn't exists."))
		{
		}

		public WaitHandleCannotBeOpenedException(string message)
			: base(message)
		{
		}

		public WaitHandleCannotBeOpenedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected WaitHandleCannotBeOpenedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
