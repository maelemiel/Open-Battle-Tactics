using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class DllNotFoundException : TypeLoadException
	{
		private const int Result = -2146233052;

		public DllNotFoundException()
			: base(Locale.GetText("DLL not found."))
		{
			base.HResult = -2146233052;
		}

		public DllNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2146233052;
		}

		protected DllNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public DllNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233052;
		}
	}
}
