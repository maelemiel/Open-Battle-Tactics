using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrittercismTestGUIAndroid : MonoBehaviour
{
	private void OnGUI()
	{
		if (Screen.height != 0 && Screen.width != 0)
		{
			int num = Screen.height / 8;
			if (GUI.Button(new Rect(0f, 0f, Screen.width, num), "Null Reference"))
			{
				CrittercismAndroid.LeaveBreadcrumb("Null Reference incoming?!");
				string text = null;
				text = text.ToLower();
			}
			if (GUI.Button(new Rect(0f, num, Screen.width, num), "Divide By Zero"))
			{
				CrittercismAndroid.LeaveBreadcrumb("Lets divide by zero!");
				int num2 = 0;
				num2 = 2 / num2;
			}
			if (GUI.Button(new Rect(0f, num * 2, Screen.width, num), "Index Out Of Range"))
			{
				string[] array = new string[1];
				array[2] = "Crash";
			}
			if (GUI.Button(new Rect(0f, num * 3, Screen.width, num), "Custom Exception"))
			{
				throw new Exception("Custom Exception");
			}
			if (GUI.Button(new Rect(0f, num * 4, Screen.width, num), "Coroutine Custom Exception"))
			{
				StartCoroutine(MonoCorutineCrash());
			}
			if (GUI.Button(new Rect(0f, num * 5, Screen.width, num), "Coroutine Null Exception"))
			{
				StartCoroutine(MonoCorutineNullCrash());
			}
			if (GUI.Button(new Rect(0f, num * 7, Screen.width, num), "Test Messages"))
			{
				Debug.Log("User Test");
				CrittercismAndroid.SetUsername("Eddie Freeman");
				Debug.Log("Metadata Test 1");
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				list.Add("Locale");
				list.Add("playerID");
				list.Add("playerLVL");
				list2.Add("en");
				list2.Add("23958");
				list2.Add("34");
				CrittercismAndroid.SetMetadata(list.ToArray(), list2.ToArray());
				Debug.Log("Breadcrumb Test");
				CrittercismAndroid.LeaveBreadcrumb("BreadCrumb");
				Debug.Log("Metadata Test 2");
				CrittercismAndroid.SetMetadata(new string[3] { "Age", "Email", "Extra" }, new string[3] { "26", "email@test.com", "Data" });
			}
		}
	}

	private IEnumerator MonoCorutineNullCrash()
	{
		string crash = null;
		crash = crash.ToLower();
		yield break;
	}

	private IEnumerator MonoCorutineCrash()
	{
		throw new Exception("Custom Coroutine Exception");
	}
}
