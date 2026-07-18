using System.Runtime.InteropServices;
using UnityEngine;

public class IOSLog : BaseLog
{
	[DllImport("__Internal")]
	private static extern void IosLog(string input);

	public static void Write(string message)
	{
		IosLog(message);
	}

	public void DebugNoTrace(string message)
	{
		Write(message);
	}

	public void InfoNoTrace(string message)
	{
		Write(message);
	}

	public void WarningNoTrace(string message)
	{
		Write(message);
	}

	public void ErrorNoTrace(string message)
	{
		Write(message);
	}

	public void Debug(string message, Object context)
	{
		Write(message);
		UnityEngine.Debug.Log(message, context);
	}

	public void Info(string message, Object context)
	{
		Write(message);
		UnityEngine.Debug.Log(message, context);
	}

	public void Warning(string message, Object context)
	{
		Write(message);
		UnityEngine.Debug.LogWarning(message, context);
	}

	public void Error(string message, Object context)
	{
		Write(message);
		UnityEngine.Debug.LogError(message, context);
	}
}
