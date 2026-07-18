using System;
using System.Collections.Generic;
using UnityEngine;

public class RegisterLogTool
{
	private static RegisterLogTool instance;

	private Action<string, string, LogType> handleLog;

	private List<Action<string, string, LogType>> callBackList;

	public static RegisterLogTool Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new RegisterLogTool();
			}
			return instance;
		}
	}

	public RegisterLogTool()
	{
		callBackList = new List<Action<string, string, LogType>>();
		Application.RegisterLogCallback(HandleLog);
	}

	private void HandleLog(string context, string stacktrace, LogType type)
	{
		if (handleLog != null)
		{
			handleLog(context, stacktrace, type);
		}
	}

	public bool AddLogListener(Action<string, string, LogType> callBack)
	{
		if (!callBackList.Contains(callBack))
		{
			callBackList.Add(callBack);
			handleLog = (Action<string, string, LogType>)Delegate.Combine(handleLog, callBack);
			return true;
		}
		return false;
	}

	public void RemoveLogListener(Action<string, string, LogType> callBack)
	{
		if (callBackList.Contains(callBack))
		{
			callBackList.Remove(callBack);
			handleLog = (Action<string, string, LogType>)Delegate.Remove(handleLog, callBack);
		}
	}
}
