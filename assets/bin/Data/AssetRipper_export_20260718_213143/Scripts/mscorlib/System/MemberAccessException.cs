using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class MemberAccessException : SystemException
	{
		private const int Result = -2146233062;

		public MemberAccessException()
			: base(Locale.GetText("Cannot access a class member."))
		{
			base.HResult = -2146233062;
		}

		public MemberAccessException(string message)
			: base(message)
		{
			base.HResult = -2146233062;
		}

		protected MemberAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MemberAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233062;
		}
	}
}
