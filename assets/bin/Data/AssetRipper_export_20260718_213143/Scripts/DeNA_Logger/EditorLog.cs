using UnityEngine;

public class EditorLog : BaseLog
{
	public void DebugNoTrace(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	public void InfoNoTrace(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	public void WarningNoTrace(string message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	public void ErrorNoTrace(string message)
	{
		UnityEngine.Debug.LogError(message);
	}

	public void Debug(string message, Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public void Info(string message, Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public void Warning(string message, Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	public void Error(string message, Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}
}
