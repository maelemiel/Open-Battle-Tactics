using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SplitTestNotification : MonoBehaviour
{
	public static void Start(string pushtext, string campaign, Action<Dictionary<string, string>> onSuccess, Action<Dictionary<string, string>> onFailure)
	{
		GameObject gameObject = new GameObject();
		SplitTestNotification splitTestNotification = gameObject.AddComponent<SplitTestNotification>();
		splitTestNotification.StartCoroutine(splitTestNotification.Launch(pushtext, campaign, onSuccess, onFailure));
	}

	private IEnumerator Launch(string pushtext, string campaign, Action<Dictionary<string, string>> onSuccess, Action<Dictionary<string, string>> onFailure)
	{
		WWWForm form = new WWWForm();
		form.AddField("campaigntoken", campaign);
		form.AddField("responsetype", "json");
		form.AddField("pushtext", pushtext);
		WWW post = new WWW("https://mdn.otherlevels.com/message/analytics", form);
		yield return post;
		Regex regex = new Regex("\"([^\"]+)\"\\:\"?([^\",\\}]+)");
		if (post.error != null)
		{
			Debug.Log("Split test error: " + post.error);
			MatchCollection matches = regex.Matches(post.error);
			Dictionary<string, string> values = new Dictionary<string, string>();
			foreach (Match i in matches)
			{
				values.Add(i.Groups[1].Value, i.Groups[2].Value);
			}
			onFailure(values);
		}
		else
		{
			Debug.Log("Split test success: " + post.text);
			MatchCollection matches2 = regex.Matches(post.text);
			Dictionary<string, string> values2 = new Dictionary<string, string>();
			foreach (Match i2 in matches2)
			{
				values2.Add(i2.Groups[1].Value, i2.Groups[2].Value);
			}
			onSuccess(values2);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
