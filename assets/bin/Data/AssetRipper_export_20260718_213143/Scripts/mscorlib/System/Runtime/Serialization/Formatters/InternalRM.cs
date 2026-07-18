using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public sealed class InternalRM
	{
		[Conditional("_LOGGING")]
		public static void InfoSoap(params object[] messages)
		{
			throw new NotImplementedException();
		}

		public static bool SoapCheckEnabled()
		{
			throw new NotImplementedException();
		}
	}
}
