using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ArgumentNullException : ArgumentException
	{
		private const int Result = -2147467261;

		public ArgumentNullException()
			: base(Locale.GetText("Argument cannot be null."))
		{
			base.HResult = -2147467261;
		}

		public ArgumentNullException(string paramName)
			: base(Locale.GetText("Argument cannot be null."), paramName)
		{
			base.HResult = -2147467261;
		}

		public ArgumentNullException(string paramName, string message)
			: base(message, paramName)
		{
			base.HResult = -2147467261;
		}

		public ArgumentNullException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147467261;
		}

		protected ArgumentNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
