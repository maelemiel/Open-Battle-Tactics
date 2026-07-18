using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class EntryPointNotFoundException : TypeLoadException
	{
		private const int Result = -2146233053;

		public EntryPointNotFoundException()
			: base(Locale.GetText("Cannot load class because of missing entry method."))
		{
			base.HResult = -2146233053;
		}

		public EntryPointNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2146233053;
		}

		protected EntryPointNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public EntryPointNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233053;
		}
	}
}
