using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[Obsolete("this type is obsoleted in 2.0 profile")]
	public class ContextMarshalException : SystemException
	{
		private const int Result = -2146233084;

		public ContextMarshalException()
			: base(Locale.GetText("Attempt to marshal and object across a context failed."))
		{
			base.HResult = -2146233084;
		}

		public ContextMarshalException(string message)
			: base(message)
		{
			base.HResult = -2146233084;
		}

		protected ContextMarshalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ContextMarshalException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233084;
		}
	}
}
