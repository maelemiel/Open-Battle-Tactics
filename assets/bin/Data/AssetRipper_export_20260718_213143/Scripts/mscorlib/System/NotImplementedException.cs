using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class NotImplementedException : SystemException
	{
		private const int Result = -2147467263;

		public NotImplementedException()
			: base(Locale.GetText("The requested feature is not implemented."))
		{
			base.HResult = -2147467263;
		}

		public NotImplementedException(string message)
			: base(message)
		{
			base.HResult = -2147467263;
		}

		public NotImplementedException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147467263;
		}

		protected NotImplementedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
