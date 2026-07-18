using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Diagnostics
{
	public class DefaultTraceListener : TraceListener
	{
		private enum DialogResult
		{
			None = 0,
			Retry = 1,
			Ignore = 2,
			Abort = 3
		}

		private const string ConsoleOutTrace = "Console.Out";

		private const string ConsoleErrorTrace = "Console.Error";

		private static readonly bool OnWin32;

		private static readonly string MonoTracePrefix;

		private static readonly string MonoTraceFile;

		private string logFileName;

		private bool assertUiEnabled;

		public bool AssertUiEnabled
		{
			get
			{
				return assertUiEnabled;
			}
			set
			{
				assertUiEnabled = value;
			}
		}

		[System.MonoTODO]
		public string LogFileName
		{
			get
			{
				return logFileName;
			}
			set
			{
				logFileName = value;
			}
		}

		public DefaultTraceListener()
			: base("Default")
		{
		}

		static DefaultTraceListener()
		{
			OnWin32 = Path.DirectorySeparatorChar == '\\';
			if (OnWin32)
			{
				return;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_TRACE_LISTENER");
			if (environmentVariable != null)
			{
				string text = null;
				string text2 = null;
				if (environmentVariable.StartsWith("Console.Out"))
				{
					text = "Console.Out";
					text2 = GetPrefix(environmentVariable, "Console.Out");
				}
				else if (environmentVariable.StartsWith("Console.Error"))
				{
					text = "Console.Error";
					text2 = GetPrefix(environmentVariable, "Console.Error");
				}
				else
				{
					text = environmentVariable;
					text2 = string.Empty;
				}
				MonoTraceFile = text;
				MonoTracePrefix = text2;
			}
		}

		private static string GetPrefix(string var, string target)
		{
			if (var.Length > target.Length)
			{
				return var.Substring(target.Length + 1);
			}
			return string.Empty;
		}

		public override void Fail(string message)
		{
			base.Fail(message);
		}

		public override void Fail(string message, string detailMessage)
		{
			base.Fail(message, detailMessage);
			if (ProcessUI(message, detailMessage) == DialogResult.Abort)
			{
				try
				{
					Thread.CurrentThread.Abort();
				}
				catch (MethodAccessException)
				{
				}
			}
			WriteLine(new StackTrace().ToString());
		}

		private DialogResult ProcessUI(string message, string detailMessage)
		{
			if (!AssertUiEnabled)
			{
				return DialogResult.None;
			}
			object obj;
			MethodInfo method;
			try
			{
				Assembly assembly = Assembly.Load("System.Windows.Forms, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
				if (assembly == null)
				{
					return DialogResult.None;
				}
				Type type = assembly.GetType("System.Windows.Forms.MessageBoxButtons");
				obj = Enum.Parse(type, "AbortRetryIgnore");
				method = assembly.GetType("System.Windows.Forms.MessageBox").GetMethod("Show", new Type[3]
				{
					typeof(string),
					typeof(string),
					type
				});
			}
			catch
			{
				return DialogResult.None;
			}
			if (method == null || obj == null)
			{
				return DialogResult.None;
			}
			string text = string.Format("Assertion Failed: {0} to quit, {1} to debug, {2} to continue", "Abort", "Retry", "Ignore");
			string text2 = string.Format("{0}{1}{2}{1}{1}{3}", message, Environment.NewLine, detailMessage, new StackTrace());
			switch (method.Invoke(null, new object[3] { text2, text, obj }).ToString())
			{
			case "Ignore":
				return DialogResult.Ignore;
			case "Abort":
				return DialogResult.Abort;
			default:
				return DialogResult.Retry;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteWindowsDebugString(string message);

		private void WriteDebugString(string message)
		{
			if (OnWin32)
			{
				WriteWindowsDebugString(message);
			}
			else
			{
				WriteMonoTrace(message);
			}
		}

		private void WriteMonoTrace(string message)
		{
			switch (MonoTraceFile)
			{
			case "Console.Out":
				Console.Out.Write(message);
				break;
			case "Console.Error":
				Console.Error.Write(message);
				break;
			default:
				WriteLogFile(message, MonoTraceFile);
				break;
			}
		}

		private void WritePrefix()
		{
			if (!OnWin32)
			{
				WriteMonoTrace(MonoTracePrefix);
			}
		}

		private void WriteImpl(string message)
		{
			if (base.NeedIndent)
			{
				WriteIndent();
				WritePrefix();
			}
			WriteDebugString(message);
			if (Debugger.IsLogging())
			{
				Debugger.Log(0, null, message);
			}
			WriteLogFile(message, LogFileName);
		}

		private void WriteLogFile(string message, string logFile)
		{
			try
			{
				WriteLogFileImpl(message, logFile);
			}
			catch (MethodAccessException)
			{
			}
		}

		private void WriteLogFileImpl(string message, string logFile)
		{
			if (logFile != null && logFile.Length != 0)
			{
				FileInfo fileInfo = new FileInfo(logFile);
				StreamWriter streamWriter = null;
				try
				{
					streamWriter = ((!fileInfo.Exists) ? fileInfo.CreateText() : fileInfo.AppendText());
				}
				catch
				{
					return;
				}
				using (streamWriter)
				{
					streamWriter.Write(message);
					streamWriter.Flush();
				}
			}
		}

		public override void Write(string message)
		{
			WriteImpl(message);
		}

		public override void WriteLine(string message)
		{
			string message2 = message + Environment.NewLine;
			WriteImpl(message2);
			base.NeedIndent = true;
		}
	}
}
