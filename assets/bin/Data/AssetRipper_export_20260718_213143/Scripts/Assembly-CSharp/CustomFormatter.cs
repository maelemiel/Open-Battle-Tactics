using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

public class CustomFormatter : IMessageFormatter
{
	private bool useColour;

	public CustomFormatter(bool useColour)
	{
		this.useColour = useColour;
	}

	public string GetFormattedString(Log.Level level, string tag, string message, params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(DateTime.UtcNow.ToString("ss,fff"));
		stringBuilder.Append("  [");
		if (useColour)
		{
			stringBuilder.Append("<color=#");
			stringBuilder.Append(HashColour(tag, level));
			stringBuilder.Append(">");
			stringBuilder.Append(tag.ToUpper());
			stringBuilder.Append("</color>");
		}
		else
		{
			stringBuilder.Append(tag.ToUpper());
		}
		stringBuilder.Append("][");
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

	private static string HashColour(string tag, Log.Level level)
	{
		switch (level)
		{
		case Log.Level.Error:
			return "ff0000";
		case Log.Level.Warning:
			return "ffff00";
		default:
		{
			Color.red.ToString();
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(tag));
			return BitConverter.ToString(array).Replace("-", string.Empty).Remove(6)
				.Trim();
		}
		}
	}
}
