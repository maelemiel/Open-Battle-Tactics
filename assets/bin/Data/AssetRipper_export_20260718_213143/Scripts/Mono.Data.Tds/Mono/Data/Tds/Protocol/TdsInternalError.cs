namespace Mono.Data.Tds.Protocol
{
	public sealed class TdsInternalError
	{
		private byte theClass;

		private int lineNumber;

		private string message;

		private int number;

		private string procedure;

		private string server;

		private string source;

		private byte state;

		public byte Class
		{
			get
			{
				return theClass;
			}
			set
			{
				theClass = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
			set
			{
				lineNumber = value;
			}
		}

		public string Message
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

		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
			}
		}

		public string Procedure
		{
			get
			{
				return procedure;
			}
			set
			{
				procedure = value;
			}
		}

		public string Server
		{
			get
			{
				return server;
			}
			set
			{
				server = value;
			}
		}

		public string Source
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}

		public byte State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
			}
		}

		public TdsInternalError(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			this.theClass = theClass;
			this.lineNumber = lineNumber;
			this.message = message;
			this.number = number;
			this.procedure = procedure;
			this.server = server;
			this.source = source;
			this.state = state;
		}
	}
}
