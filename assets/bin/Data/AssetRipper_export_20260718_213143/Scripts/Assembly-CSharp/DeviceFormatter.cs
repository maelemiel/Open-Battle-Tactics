using System;
using System.Text;
using System.Threading;

public class DeviceFormatter : IMessageFormatter
{
	public string GetFormattedString(Log.Level level, string tag, string message, params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(DateTime.UtcNow.ToString("ss,fff"));
		stringBuilder.Append("   [");
		stringBuilder.Append(tag);
		stringBuilder.Append("] [");
		if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
		{
			stringBuilder.Append(Thread.CurrentThread.Name);
		}
		else
		{
			stringBuilder.Append(Thread.CurrentThread.ManagedThreadId);
		}
		stringBuilder.Append("]: ");
		if (args != null && args.Length > 0)
		{
			stringBuilder.AppendFormat(message, args);
		}
		else
		{
			stringBuilder.Append(message);
		}
		return stringBuilder.ToString();
	}
}
