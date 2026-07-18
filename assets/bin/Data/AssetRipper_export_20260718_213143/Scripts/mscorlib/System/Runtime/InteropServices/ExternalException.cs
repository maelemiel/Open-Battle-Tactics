using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public class ExternalException : SystemException
	{
		public virtual int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}

		public ExternalException()
			: base(Locale.GetText("External exception"))
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message)
			: base(message)
		{
			base.HResult = -2147467259;
		}

		protected ExternalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ExternalException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147467259;
		}

		public ExternalException(string message, int errorCode)
			: base(message)
		{
			base.HResult = errorCode;
		}
	}
}
