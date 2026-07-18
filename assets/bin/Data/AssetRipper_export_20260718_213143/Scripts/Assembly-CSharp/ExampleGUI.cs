using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

public class ExampleGUI : MonoBehaviour
{
	private int nr_buttons = 5;

	private static bool isEnabled;

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, Screen.height * 0 / nr_buttons, Screen.width, Screen.height / nr_buttons), "manual launch"))
		{
			Adjust.appDidLaunch("querty123456", AdjustUtil.AdjustEnvironment.Sandbox, AdjustUtil.LogLevel.Verbose, false);
			isEnabled = true;
		}
		if (GUI.Button(new Rect(0f, Screen.height * 1 / nr_buttons, Screen.width, Screen.height / nr_buttons), "track Event"))
		{
			Adjust.trackEvent("eve001");
			Dictionary<string, string> dictionary = new Dictionary<string, string>(2);
			dictionary.Add("key", "value");
			dictionary.Add("foo", "bar");
			Adjust.trackEvent("eve002", dictionary);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 2 / nr_buttons, Screen.width, Screen.height / nr_buttons), "track Revenue"))
		{
			Adjust.trackRevenue(3.44);
			Adjust.trackRevenue(3.45, "rev001");
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(2);
			dictionary2.Add("key", "value");
			dictionary2.Add("foo", "bar");
			Adjust.trackRevenue(0.1, "rev002", dictionary2);
		}
		if (GUI.Button(new Rect(0f, Screen.height * 3 / nr_buttons, Screen.width, Screen.height / nr_buttons), "callback"))
		{
			Adjust.setResponseDelegate(responseDelegate);
		}
		string text = ((!isEnabled) ? "enable sdk" : "disable sdk");
		if (GUI.Button(new Rect(0f, Screen.height * 4 / nr_buttons, Screen.width, Screen.height / nr_buttons), text))
		{
			isEnabled = !Adjust.isEnabled();
			Adjust.setEnabled(isEnabled);
		}
	}

	public void responseDelegate(ResponseData responseData)
	{
		Debug.Log("Was success? " + responseData.success);
		Debug.Log("Will retry? " + responseData.willRetry);
		if (!string.IsNullOrEmpty(responseData.activityKindString))
		{
			Debug.Log("activityKind " + responseData.activityKindString);
		}
		if (responseData.trackerName != null)
		{
			Debug.Log("trackerName " + responseData.trackerName);
		}
		if (responseData.trackerToken != null)
		{
			Debug.Log("trackerToken " + responseData.trackerToken);
		}
		if (responseData.network != null)
		{
			Debug.Log("network " + responseData.network);
		}
		if (responseData.campaign != null)
		{
			Debug.Log("campaign " + responseData.campaign);
		}
		if (responseData.adgroup != null)
		{
			Debug.Log("adgroup " + responseData.adgroup);
		}
		if (responseData.creative != null)
		{
			Debug.Log("creative " + responseData.creative);
		}
		if (responseData.error != null)
		{
			Debug.Log("error " + responseData.error);
		}
	}
}
