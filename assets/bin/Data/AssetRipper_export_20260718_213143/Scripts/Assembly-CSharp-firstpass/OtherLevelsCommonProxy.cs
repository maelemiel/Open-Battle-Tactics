using System;
using UnityEngine;

public class OtherLevelsCommonProxy : MonoBehaviour
{
	public string bundleId = "BUNDLE_ID";

	private static OtherLevelsCommonProxy _instance;

	public static string BundleId
	{
		get
		{
			return Instance.bundleId;
		}
	}

	private static OtherLevelsCommonProxy Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<OtherLevelsCommonProxy>();
			}
			if (!_instance)
			{
				throw new ArgumentNullException("OtherLevelsCommonProxy.instance");
			}
			return _instance;
		}
	}
}
