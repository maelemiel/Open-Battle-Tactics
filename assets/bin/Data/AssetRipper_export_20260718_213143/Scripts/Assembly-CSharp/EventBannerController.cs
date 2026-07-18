using System.Collections;
using UnityEngine;

public class EventBannerController : MonoBehaviour
{
	public bool autoLoad;

	public bool checkUserInClub;

	[SerializeField]
	private PrefabProxy bannerPrefabProxy;

	private void Start()
	{
		if (autoLoad)
		{
			LoadBanner();
		}
	}

	public IEnumerator LoadBannerCoroutine(EventDataModel eventDataModel)
	{
		if (eventDataModel != null && (bool)bannerPrefabProxy && eventDataModel.LogoAssetLinkage != null)
		{
			yield return StartCoroutine(bannerPrefabProxy.ChangeAssetCoroutine(eventDataModel.LeaderboardsBannerAssetLinkage));
		}
	}

	public void LoadBanner()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent(checkUserInClub);
		StartCoroutine(LoadBannerCoroutine(activeOnCooldownEvent));
	}
}
