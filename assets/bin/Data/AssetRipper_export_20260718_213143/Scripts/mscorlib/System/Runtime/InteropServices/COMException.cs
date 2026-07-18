using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public class COMException : ExternalException
	{
		public COMException()
		{
		}

		public COMException(string message)
			: base(message)
		{
		}

		public COMException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public COMException(string message, int errorCode)
			: base(message, errorCode)
		{
		}

		protected COMException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} (0x{1:x}): {2} {3}{4}{5}", GetType(), base.HResult, Message, (InnerException != null) ? InnerException.ToString() : string.Empty, Environment.NewLine, (StackTrace == null) ? string.Empty : StackTrace);
		}
	}
}
