using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class IOException : SystemException
	{
		public IOException()
			: base("I/O Error")
		{
		}

		public IOException(string message)
			: base(message)
		{
		}

		public IOException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected IOException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public IOException(string message, int hresult)
			: base(message)
		{
			base.HResult = hresult;
		}
	}
}
