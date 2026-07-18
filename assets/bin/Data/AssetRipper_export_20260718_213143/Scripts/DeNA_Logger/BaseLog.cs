using UnityEngine;

public interface BaseLog
{
	void DebugNoTrace(string message);

	void InfoNoTrace(string message);

	void WarningNoTrace(string message);

	void ErrorNoTrace(string message);

	void Debug(string message, Object context);

	void Info(string message, Object context);

	void Warning(string message, Object context);

	void Error(string message, Object context);
}
