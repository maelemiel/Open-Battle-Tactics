using System;
using UnityEngine;

public class CrittercismTestGUI : MonoBehaviour
{
	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.button);
		gUIStyle.fontSize = 30;
		int num = Screen.height / 8;
		if (GUI.Button(new Rect(0f, 0f, Screen.width, num), "Leave breadcrumb", gUIStyle))
		{
			CrittercismIOS.LeaveBreadcrumb("BreadCrumb");
		}
		if (GUI.Button(new Rect(0f, num, Screen.width, num), "Set User Metadata", gUIStyle))
		{
			CrittercismIOS.SetUsername("Username");
			CrittercismIOS.SetValue("5", "Game Level");
			CrittercismIOS.SetValue("Crashes a lot", "Status");
		}
		if (GUI.Button(new Rect(0f, num * 2, Screen.width, num), "C# Crash", gUIStyle))
		{
			causeDivideByZeroException();
		}
		if (GUI.Button(new Rect(0f, num * 3, Screen.width, num), "C# Handled Exception", gUIStyle))
		{
			try
			{
				causeDivideByZeroException();
			}
			catch (Exception e)
			{
				CrittercismIOS.LogHandledException(e);
			}
		}
	}

	private void causeDivideByZeroException()
	{
		interimMethod1("hi mom", 42);
	}

	private void interimMethod1(string demoParam1, int demoParam2)
	{
		interimMethod2(7, 7, "abc");
	}

	private void interimMethod2(byte demoParam1, int demoParam2, string demoParam3)
	{
		finallyDoTheCrash(100);
	}

	private void finallyDoTheCrash(int number)
	{
		number /= 0;
	}
}
