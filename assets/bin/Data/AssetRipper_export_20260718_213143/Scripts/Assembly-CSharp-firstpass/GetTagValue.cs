using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GetTagValue : MonoBehaviour
{
	public static void Get(string appKey, string trackingId, string tagName, Action<Dictionary<string, string>> onSuccess, Action<Dictionary<string, string>> onFailure)
	{
		GameObject gameObject = new GameObject();
		GetTagValue getTagValue = gameObject.AddComponent<GetTagValue>();
		getTagValue.StartCoroutine(getTagValue.Launch(appKey, trackingId, tagName, onSuccess, onFailure));
	}

	private IEnumerator Launch(string appKey, string trackingId, string tagName, Action<Dictionary<string, string>> onSuccess, Action<Dictionary<string, string>> onFailure)
	{
		WWW getTag = new WWW("https://tags.otherlevels.com/api/apps/" + appKey + "/tracking/" + trackingId + "/tag/" + tagName);
		yield return getTag;
		Regex regex = new Regex("\"([^\"]+)\"\\:\"?([^\",\\}]+)");
		if (getTag.error != null)
		{
			Debug.Log("Unable to find TrackingId or AppKey, OR There is no Internet Connection");
			Debug.Log("Get TagValue error: " + getTag.error);
			MatchCollection matches = regex.Matches(getTag.error);
			Dictionary<string, string> values = new Dictionary<string, string>();
			foreach (Match i in matches)
			{
				values.Add(i.Groups[1].Value, i.Groups[2].Value);
			}
			onFailure(values);
		}
		else
		{
			Debug.Log("Get TagValue success: " + getTag.text);
			MatchCollection matches2 = regex.Matches(getTag.text);
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
