using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Serialization.Formatters
{
	[Serializable]
	[ComVisible(true)]
	[SoapType]
	public sealed class ServerFault
	{
		private string exceptionType;

		private string message;

		private string stackTrace;

		private Exception exception;

		public string ExceptionType
		{
			get
			{
				return exceptionType;
			}
			set
			{
				exceptionType = value;
			}
		}

		public string ExceptionMessage
		{
			get
			{
				return message;
			}
			set
			{
				message = value;
			}
		}

		public string StackTrace
		{
			get
			{
				return stackTrace;
			}
			set
			{
				stackTrace = value;
			}
		}

		public ServerFault(string exceptionType, string message, string stackTrace)
		{
			this.exceptionType = exceptionType;
			this.message = message;
			this.stackTrace = stackTrace;
		}
	}
}
