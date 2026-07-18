using UnityEngine;

public class AAIDManager : MonoBehaviour
{
	private string _aaid;

	public void FetchAAID()
	{
		Log.Debug("OL FetchAAID");
		GameObject gameObject = new GameObject("AAIDManager");
		gameObject.AddComponent<AAIDManager>();
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobage.ww.a1933.Super_Battle_Tactics_Android.OLIDFA"))
		{
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				androidJavaClass.CallStatic("fetchAAID", androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity"), "AAIDManager", "OnFetchedAAID");
			}
		}
	}

	public void OnFetchedAAID(string aaid)
	{
		Log.Debug("OL OnFetchedAAID: " + aaid);
		_aaid = aaid;
	}
}
