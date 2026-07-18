namespace System.Diagnostics
{
	public sealed class Debug
	{
		private Debug()
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, string details)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, string details, object[] args)
		{
		}

		[Conditional("DEBUG")]
		public static void WriteLine(object obj)
		{
			Console.WriteLine(obj);
		}

		[Conditional("DEBUG")]
		public static void WriteLine(string message)
		{
			Console.WriteLine(message);
		}

		[Conditional("DEBUG")]
		public static void WriteLine(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}
	}
}
