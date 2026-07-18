using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public class PolicyException : SystemException, _Exception
	{
		public PolicyException()
			: base(Locale.GetText("Cannot run because of policy."))
		{
		}

		public PolicyException(string message)
			: base(message)
		{
		}

		protected PolicyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public PolicyException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
