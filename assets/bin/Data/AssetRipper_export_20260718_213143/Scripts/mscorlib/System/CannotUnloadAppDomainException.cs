using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class CannotUnloadAppDomainException : SystemException
	{
		private const int Result = -2146234347;

		public CannotUnloadAppDomainException()
			: base(Locale.GetText("Attempt to unload application domain failed."))
		{
			base.HResult = -2146234347;
		}

		public CannotUnloadAppDomainException(string message)
			: base(message)
		{
			base.HResult = -2146234347;
		}

		protected CannotUnloadAppDomainException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public CannotUnloadAppDomainException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146234347;
		}
	}
}
