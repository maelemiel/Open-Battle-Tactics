using System.IO;
using System.Text;

namespace System
{
	public static class Console
	{
		internal static TextWriter stdout;

		private static TextWriter stderr;

		private static TextReader stdin;

		private static Encoding inputEncoding;

		private static Encoding outputEncoding;

		public static TextWriter Error
		{
			get
			{
				return stderr;
			}
		}

		public static TextWriter Out
		{
			get
			{
				return stdout;
			}
		}

		public static TextReader In
		{
			get
			{
				return stdin;
			}
		}

		public static Encoding InputEncoding
		{
			get
			{
				return inputEncoding;
			}
			set
			{
				inputEncoding = value;
			}
		}

		public static Encoding OutputEncoding
		{
			get
			{
				return outputEncoding;
			}
			set
			{
				outputEncoding = value;
			}
		}

		static Console()
		{
			Encoding encoding;
			Encoding encoding2;
			if (Environment.IsRunningOnWindows)
			{
				encoding = (encoding2 = Encoding.Default);
			}
			else
			{
				int code_page = 0;
				Encoding.InternalCodePage(ref code_page);
				encoding = ((code_page == -1 || ((code_page & 0xFFFFFFF) != 3 && (code_page & 0x10000000) == 0)) ? (encoding2 = Encoding.Default) : (encoding2 = Encoding.UTF8Unmarked));
			}
			stderr = new UnexceptionalStreamWriter(OpenStandardError(0), encoding2);
			((StreamWriter)stderr).AutoFlush = true;
			stderr = TextWriter.Synchronized(stderr, true);
			stdout = new UnexceptionalStreamWriter(OpenStandardOutput(0), encoding2);
			((StreamWriter)stdout).AutoFlush = true;
			stdout = TextWriter.Synchronized(stdout, true);
			stdin = new UnexceptionalStreamReader(OpenStandardInput(0), encoding);
			stdin = TextReader.Synchronized(stdin);
			GC.SuppressFinalize(stdout);
			GC.SuppressFinalize(stderr);
			GC.SuppressFinalize(stdin);
		}

		public static Stream OpenStandardError()
		{
			return OpenStandardError(0);
		}

		private static Stream Open(IntPtr handle, FileAccess access, int bufferSize)
		{
			try
			{
				return new FileStream(handle, access, false, bufferSize, false, bufferSize == 0);
			}
			catch (IOException)
			{
				return new NullStream();
			}
		}

		public static Stream OpenStandardError(int bufferSize)
		{
			return Open(MonoIO.ConsoleError, FileAccess.Write, bufferSize);
		}

		public static Stream OpenStandardInput()
		{
			return OpenStandardInput(0);
		}

		public static Stream OpenStandardInput(int bufferSize)
		{
			return Open(MonoIO.ConsoleInput, FileAccess.Read, bufferSize);
		}

		public static Stream OpenStandardOutput()
		{
			return OpenStandardOutput(0);
		}

		public static Stream OpenStandardOutput(int bufferSize)
		{
			return Open(MonoIO.ConsoleOutput, FileAccess.Write, bufferSize);
		}

		public static void SetError(TextWriter newError)
		{
			if (newError == null)
			{
				throw new ArgumentNullException("newError");
			}
			stderr = newError;
		}

		public static void SetIn(TextReader newIn)
		{
			if (newIn == null)
			{
				throw new ArgumentNullException("newIn");
			}
			stdin = newIn;
		}

		public static void SetOut(TextWriter newOut)
		{
			if (newOut == null)
			{
				throw new ArgumentNullException("newOut");
			}
			stdout = newOut;
		}

		public static void Write(bool value)
		{
			stdout.Write(value);
		}

		public static void Write(char value)
		{
			stdout.Write(value);
		}

		public static void Write(char[] buffer)
		{
			stdout.Write(buffer);
		}

		public static void Write(decimal value)
		{
			stdout.Write(value);
		}

		public static void Write(double value)
		{
			stdout.Write(value);
		}

		public static void Write(int value)
		{
			stdout.Write(value);
		}

		public static void Write(long value)
		{
			stdout.Write(value);
		}

		public static void Write(object value)
		{
			stdout.Write(value);
		}

		public static void Write(float value)
		{
			stdout.Write(value);
		}

		public static void Write(string value)
		{
			stdout.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(uint value)
		{
			stdout.Write(value);
		}

		[CLSCompliant(false)]
		public static void Write(ulong value)
		{
			stdout.Write(value);
		}

		public static void Write(string format, object arg0)
		{
			stdout.Write(format, arg0);
		}

		public static void Write(string format, params object[] arg)
		{
			stdout.Write(format, arg);
		}

		public static void Write(char[] buffer, int index, int count)
		{
			stdout.Write(buffer, index, count);
		}

		public static void Write(string format, object arg0, object arg1)
		{
			stdout.Write(format, arg0, arg1);
		}

		public static void Write(string format, object arg0, object arg1, object arg2)
		{
			stdout.Write(format, arg0, arg1, arg2);
		}

		[CLSCompliant(false)]
		public static void Write(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			object[] array = new object[remainingCount + 4];
			array[0] = arg0;
			array[1] = arg1;
			array[2] = arg2;
			array[3] = arg3;
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				array[i + 4] = TypedReference.ToObject(nextArg);
			}
			stdout.Write(string.Format(format, array));
		}

		public static void WriteLine()
		{
			stdout.WriteLine();
		}

		public static void WriteLine(bool value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(char value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(char[] buffer)
		{
			stdout.WriteLine(buffer);
		}

		public static void WriteLine(decimal value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(double value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(int value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(long value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(object value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(float value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(string value)
		{
			stdout.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(uint value)
		{
			stdout.WriteLine(value);
		}

		[CLSCompliant(false)]
		public static void WriteLine(ulong value)
		{
			stdout.WriteLine(value);
		}

		public static void WriteLine(string format, object arg0)
		{
			stdout.WriteLine(format, arg0);
		}

		public static void WriteLine(string format, params object[] arg)
		{
			stdout.WriteLine(format, arg);
		}

		public static void WriteLine(char[] buffer, int index, int count)
		{
			stdout.WriteLine(buffer, index, count);
		}

		public static void WriteLine(string format, object arg0, object arg1)
		{
			stdout.WriteLine(format, arg0, arg1);
		}

		public static void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			stdout.WriteLine(format, arg0, arg1, arg2);
		}

		[CLSCompliant(false)]
		public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			object[] array = new object[remainingCount + 4];
			array[0] = arg0;
			array[1] = arg1;
			array[2] = arg2;
			array[3] = arg3;
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				array[i + 4] = TypedReference.ToObject(nextArg);
			}
			stdout.WriteLine(string.Format(format, array));
		}

		public static int Read()
		{
			return stdin.Read();
		}

		public static string ReadLine()
		{
			return stdin.ReadLine();
		}
	}
}
