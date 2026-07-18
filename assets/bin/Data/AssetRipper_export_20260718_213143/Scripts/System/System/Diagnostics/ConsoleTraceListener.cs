namespace System.Diagnostics
{
	public class ConsoleTraceListener : TextWriterTraceListener
	{
		public ConsoleTraceListener()
			: this(false)
		{
		}

		public ConsoleTraceListener(bool useErrorStream)
			: base((!useErrorStream) ? Console.Out : Console.Error)
		{
		}

		internal ConsoleTraceListener(string data)
			: this(Convert.ToBoolean(data))
		{
		}
	}
}
