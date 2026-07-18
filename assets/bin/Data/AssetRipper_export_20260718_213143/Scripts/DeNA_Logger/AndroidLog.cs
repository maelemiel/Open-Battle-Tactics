using System;
using UnityEngine;

public class AndroidLog : BaseLog
{
	public void DebugNoTrace(string message)
	{
		Console.WriteLine(message);
	}

	public void InfoNoTrace(string message)
	{
		Console.WriteLine(message);
	}

	public void WarningNoTrace(string message)
	{
		Console.WriteLine(message);
	}

	public void ErrorNoTrace(string message)
	{
		Console.WriteLine(message);
	}

	public void Debug(string message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public void Info(string message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public void Warning(string message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	public void Error(string message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}
}
