using UnityEngine;

public class CrittercismInit : MonoBehaviour
{
	private const string CrittercismAppID = "5322ddaaa6d3d73f2a000001";

	private const bool bDelaySendingAppLoad = false;

	private const bool bShouldCollectLogcat = false;

	private const string CustomVersionName = "1.0";

	private void Awake()
	{
		CrittercismAndroid.Init("5322ddaaa6d3d73f2a000001", false, false, "1.0");
		Object.Destroy(this);
	}

	private void Update()
	{
		CrittercismAndroid.Update();
	}
}
