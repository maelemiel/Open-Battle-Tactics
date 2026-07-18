using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ExecutionEngineException : SystemException
	{
		public ExecutionEngineException()
			: base(Locale.GetText("Internal error occurred."))
		{
		}

		public ExecutionEngineException(string message)
			: base(message)
		{
		}

		public ExecutionEngineException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal ExecutionEngineException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
