using ChartboostSDK;
using UnityEngine;

public class ChartboostExample : MonoBehaviour
{
	public GameObject inPlayIcon;

	public GameObject inPlayText;

	private CBInPlay inPlayAd;

	private void OnEnable()
	{
		Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial += didDismissInterstitial;
		Chartboost.didCloseInterstitial += didCloseInterstitial;
		Chartboost.didClickInterstitial += didClickInterstitial;
		Chartboost.didCacheInterstitial += didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial += shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial += didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps += didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps += didDismissMoreApps;
		Chartboost.didCloseMoreApps += didCloseMoreApps;
		Chartboost.didClickMoreApps += didClickMoreApps;
		Chartboost.didCacheMoreApps += didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps += shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps += didDisplayMoreApps;
		Chartboost.didFailToRecordClick += didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo += didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo += shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
		Chartboost.didCacheInPlay += didCacheInPlay;
		Chartboost.didFailToLoadInPlay += didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation += didPauseClickForConfirmation;
		Chartboost.willDisplayVideo += willDisplayVideo;
	}

	private void Update()
	{
		if (Input.touchCount > 0 && inPlayIcon.GetComponent<GUITexture>().HitTest(Input.GetTouch(0).position) && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			inPlayAd.click();
		}
	}

	private void OnGUI()
	{
		GUI.matrix = Matrix4x4.Scale(new Vector3(5f, 5f, 5f));
		if (GUILayout.Button("Cache Interstitial"))
		{
			Chartboost.cacheInterstitial(CBLocation.Default);
		}
		if (GUILayout.Button("Show Interstitial"))
		{
			Chartboost.showInterstitial(CBLocation.Default);
		}
		if (GUILayout.Button("Cache More Apps"))
		{
			Chartboost.cacheMoreApps(CBLocation.Default);
		}
		if (GUILayout.Button("Show More Apps"))
		{
			Chartboost.showMoreApps(CBLocation.Default);
		}
		if (GUILayout.Button("Cache Rewarded Video"))
		{
			Chartboost.cacheRewardedVideo(CBLocation.Default);
		}
		if (GUILayout.Button("Show Rewarded Video"))
		{
			Chartboost.showRewardedVideo(CBLocation.Default);
		}
		if (GUILayout.Button("Cache InPlay Ad"))
		{
			Chartboost.cacheInPlay(CBLocation.Default);
		}
		if (GUILayout.Button("Show InPlay Ad"))
		{
			inPlayAd = Chartboost.getInPlay(CBLocation.Default);
			if (inPlayAd != null)
			{
				inPlayIcon.GetComponent<GUITexture>().texture = inPlayAd.appIcon;
				inPlayText.GetComponent<GUIText>().text = inPlayAd.appName;
				inPlayAd.show();
			}
		}
	}

	private void OnDisable()
	{
		Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
		Chartboost.didDismissInterstitial -= didDismissInterstitial;
		Chartboost.didCloseInterstitial -= didCloseInterstitial;
		Chartboost.didClickInterstitial -= didClickInterstitial;
		Chartboost.didCacheInterstitial -= didCacheInterstitial;
		Chartboost.shouldDisplayInterstitial -= shouldDisplayInterstitial;
		Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
		Chartboost.didFailToLoadMoreApps -= didFailToLoadMoreApps;
		Chartboost.didDismissMoreApps -= didDismissMoreApps;
		Chartboost.didCloseMoreApps -= didCloseMoreApps;
		Chartboost.didClickMoreApps -= didClickMoreApps;
		Chartboost.didCacheMoreApps -= didCacheMoreApps;
		Chartboost.shouldDisplayMoreApps -= shouldDisplayMoreApps;
		Chartboost.didDisplayMoreApps -= didDisplayMoreApps;
		Chartboost.didFailToRecordClick -= didFailToRecordClick;
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
		Chartboost.didDismissRewardedVideo -= didDismissRewardedVideo;
		Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
		Chartboost.didClickRewardedVideo -= didClickRewardedVideo;
		Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
		Chartboost.shouldDisplayRewardedVideo -= shouldDisplayRewardedVideo;
		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;
		Chartboost.didCacheInPlay -= didCacheInPlay;
		Chartboost.didFailToLoadInPlay -= didFailToLoadInPlay;
		Chartboost.didPauseClickForConfirmation -= didPauseClickForConfirmation;
		Chartboost.willDisplayVideo -= willDisplayVideo;
	}

	private void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
	{
		Debug.Log(string.Format("didFailToLoadInterstitial: {0} at location {1}", error, location));
	}

	private void didDismissInterstitial(CBLocation location)
	{
		Debug.Log("didDismissInterstitial: " + location);
	}

	private void didCloseInterstitial(CBLocation location)
	{
		Debug.Log("didCloseInterstitial: " + location);
	}

	private void didClickInterstitial(CBLocation location)
	{
		Debug.Log("didClickInterstitial: " + location);
	}

	private void didCacheInterstitial(CBLocation location)
	{
		Debug.Log("didCacheInterstitial: " + location);
	}

	private bool shouldDisplayInterstitial(CBLocation location)
	{
		Debug.Log("shouldDisplayInterstitial: " + location);
		return true;
	}

	private void didDisplayInterstitial(CBLocation location)
	{
		Debug.Log("didDisplayInterstitial: " + location);
	}

	private void didFailToLoadMoreApps(CBLocation location, CBImpressionError error)
	{
		Debug.Log(string.Format("didFailToLoadMoreApps: {0} at location: {1}", error, location));
	}

	private void didDismissMoreApps(CBLocation location)
	{
		Debug.Log(string.Format("didDismissMoreApps at location: {0}", location));
	}

	private void didCloseMoreApps(CBLocation location)
	{
		Debug.Log(string.Format("didCloseMoreApps at location: {0}", location));
	}

	private void didClickMoreApps(CBLocation location)
	{
		Debug.Log(string.Format("didClickMoreApps at location: {0}", location));
	}

	private void didCacheMoreApps(CBLocation location)
	{
		Debug.Log(string.Format("didCacheMoreApps at location: {0}", location));
	}

	private bool shouldDisplayMoreApps(CBLocation location)
	{
		Debug.Log(string.Format("shouldDisplayMoreApps at location: {0}", location));
		return true;
	}

	private void didDisplayMoreApps(CBLocation location)
	{
		Debug.Log("didDisplayMoreApps: " + location);
	}

	private void didFailToRecordClick(CBLocation location, CBImpressionError error)
	{
		Debug.Log(string.Format("didFailToRecordClick: {0} at location: {1}", error, location));
	}

	private void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
	{
		Debug.Log(string.Format("didFailToLoadRewardedVideo: {0} at location {1}", error, location));
	}

	private void didDismissRewardedVideo(CBLocation location)
	{
		Debug.Log("didDismissRewardedVideo: " + location);
	}

	private void didCloseRewardedVideo(CBLocation location)
	{
		Debug.Log("didCloseRewardedVideo: " + location);
	}

	private void didClickRewardedVideo(CBLocation location)
	{
		Debug.Log("didClickRewardedVideo: " + location);
	}

	private void didCacheRewardedVideo(CBLocation location)
	{
		Debug.Log("didCacheRewardedVideo: " + location);
	}

	private bool shouldDisplayRewardedVideo(CBLocation location)
	{
		Debug.Log("shouldDisplayRewardedVideo: " + location);
		return true;
	}

	private void didCompleteRewardedVideo(CBLocation location, int reward)
	{
		Debug.Log(string.Format("didCompleteRewardedVideo: reward {0} at location {1}", reward, location));
	}

	private void didDisplayRewardedVideo(CBLocation location)
	{
		Debug.Log("didDisplayRewardedVideo: " + location);
	}

	private void didCacheInPlay(CBLocation location)
	{
		Debug.Log("didCacheInPlay called: " + location);
	}

	private void didFailToLoadInPlay(CBLocation location, CBImpressionError error)
	{
		Debug.Log(string.Format("didFailToLoadInPlay: {0} at location: {1}", error, location));
	}

	private void didPauseClickForConfirmation()
	{
		Debug.Log("didPauseClickForConfirmation called");
	}

	private void willDisplayVideo(CBLocation location)
	{
		Debug.Log("willDisplayVideo: " + location);
	}
}
