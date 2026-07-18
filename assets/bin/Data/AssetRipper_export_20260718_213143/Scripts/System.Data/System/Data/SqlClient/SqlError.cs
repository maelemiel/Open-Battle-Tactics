namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlError
	{
		private byte errorClass;

		private int lineNumber;

		private string message = string.Empty;

		private int number;

		private string procedure = string.Empty;

		private string source = string.Empty;

		private byte state;

		[NonSerialized]
		private string server = string.Empty;

		public byte Class
		{
			get
			{
				return errorClass;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
		}

		public int Number
		{
			get
			{
				return number;
			}
		}

		public string Procedure
		{
			get
			{
				return procedure;
			}
		}

		public string Server
		{
			get
			{
				return server;
			}
		}

		public string Source
		{
			get
			{
				return source;
			}
		}

		public byte State
		{
			get
			{
				return state;
			}
		}

		internal SqlError(byte errorClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			this.errorClass = errorClass;
			this.lineNumber = lineNumber;
			this.message = message;
			this.number = number;
			this.procedure = procedure;
			this.server = server;
			this.source = source;
			this.state = state;
		}

		public override string ToString()
		{
			return Message;
		}
	}
}
