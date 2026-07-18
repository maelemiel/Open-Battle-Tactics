using System.Collections;
using UnityEngine;

public class EventBackgroundController : MonoBehaviour
{
	private const string EVENT_BACKGROUND_SPRITE_NAME = "event_background";

	public bool autoLoad;

	public bool checkUserInClub;

	[SerializeField]
	private tk2dBaseSprite backgroundSprite;

	[SerializeField]
	private PrefabProxy backgroundPrefabProxy;

	private void Start()
	{
		if (autoLoad)
		{
			LoadBackground();
		}
	}

	public IEnumerator LoadBackgroundCoroutine(EventDataModel eventDataModel)
	{
		if (eventDataModel == null || !backgroundPrefabProxy || !backgroundSprite || eventDataModel.BackgroundAssetLinkage == null)
		{
			yield break;
		}
		yield return StartCoroutine(backgroundPrefabProxy.ChangeAssetCoroutine(eventDataModel.BackgroundAssetLinkage));
		yield return StartCoroutine(backgroundPrefabProxy.WaitForAssetReady());
		tk2dSprite eventBackgroundSprite = backgroundPrefabProxy.Prefab.GetComponent<tk2dSprite>();
		if ((bool)eventBackgroundSprite)
		{
			if ((bool)backgroundSprite)
			{
				backgroundSprite.SetSprite(eventBackgroundSprite.Collection, "event_background");
			}
			backgroundPrefabProxy.gameObject.SetActive(false);
		}
	}

	public void LoadBackground()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent(checkUserInClub);
		StartCoroutine(LoadBackgroundCoroutine(activeOnCooldownEvent));
	}
}
