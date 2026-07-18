using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class GachaSceneController : SceneController
{
	[Serializable]
	public class GachaCrateSkeletonData
	{
		public GachaTypes gachaType;

		public tk2dSpineSkeletonDataAsset skeletonDataAsset;
	}

	private const string BUTTON_INACTIVE_SPRITE_NAME = "large_button_grey";

	private const float NORMAL_WAIT_TIME = 0.8f;

	private const float MULTIBOX_WAIT_TIME = 1f;

	private const float MULTIBOX_DELAY_TIME = 0.5f;

	private const string openBoxMessageSingleKeyString = "2";

	private const string openBoxMessageVariousKeyString = "1";

	[SerializeField]
	private GameObject gachaButtonsParent;

	[SerializeField]
	private GameObject exitButton;

	[SerializeField]
	private GameObject SkipButton;

	[SerializeField]
	private PriceLabelController priceLabelController;

	[SerializeField]
	private GachaBoxesController gachaBoxesController;

	[SerializeField]
	private GachaItemController[] gachaItemControllers;

	[SerializeField]
	private ObjectShaker screenshakeTarget;

	[SerializeField]
	private tk2dTextMesh openBoxesMessage;

	private int currentGachaResults;

	private bool isShowingSummary;

	private GachaTypes gachaType;

	private List<GachaItemController> itemsCache = new List<GachaItemController>();

	private int GACHA_RESULT_COUNT_LIMIT = 3;

	private List<ItemCollectionDataModel.Item> gachaResults;

	private bool isMultiBoxGacha
	{
		get
		{
			if (gachaResults == null)
			{
				return false;
			}
			if (gachaResults.Count > Constants.StartReplacingCratesCount)
			{
				return true;
			}
			return false;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if ((bool)openBoxesMessage)
		{
			openBoxesMessage.gameObject.SetActive(false);
		}
		SceneController.resumeCallbackEnable = false;
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
			TopBarController.instance.Visible = false;
		}
		for (int i = 0; i < gachaItemControllers.Length; i++)
		{
			gachaItemControllers[i].buttonPressedCallback = GachaTankRequest;
		}
		AudioTrigger.StandardCrowd.PlayMusic();
		AudioTrigger.CrowdHush.Play();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		GachaRewardsSceneModel gachaResultsPayload = null;
		if (sceneModel == null)
		{
			Debug.LogWarning("No rewards!! Creating GachaRewardsSceneModel fake object..");
			ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 901, 2));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaResultsPayload = new GachaRewardsSceneModel(GachaTypes.PREMIUM, itemCollectionDataModel, null);
			sceneModel = new SceneModel(gachaResultsPayload);
		}
		else
		{
			gachaResultsPayload = (GachaRewardsSceneModel)sceneModel.payload;
		}
		gachaType = gachaResultsPayload.gachaType;
		if (gachaResultsPayload != null)
		{
			tk2dSpineSkeletonDataAsset gachaBoxSkeletonDataAsset = gachaBoxesController.GetGachaBoxSkeletonDataAsset(gachaType);
			for (int i = 0; i < gachaItemControllers.Length; i++)
			{
				gachaItemControllers[i].uiButton.enabled = false;
				if (gachaBoxSkeletonDataAsset != null)
				{
					gachaItemControllers[i].SetGachaAnimationSkeleton(gachaBoxSkeletonDataAsset);
				}
			}
		}
		if (gachaResultsPayload.gachaRewards != null)
		{
			SetGachaResults(gachaResultsPayload.gachaRewards);
		}
		else
		{
			gachaResultsPayload.OnRewardsAvailable += delegate
			{
				SetGachaResults(gachaResultsPayload.gachaRewards);
			};
		}
		StartCoroutine(IntroAnimation());
	}

	private IEnumerator IntroAnimation()
	{
		float waitTime = ((!isMultiBoxGacha) ? 0.8f : 1f);
		float delayTime = 0f;
		for (int i = 0; i < gachaItemControllers.Length; i++)
		{
			GachaItemController itemController = gachaItemControllers[i];
			itemsCache.Add(itemController);
			Vector3 originalPosition = itemController.transform.localPosition;
			itemController.transform.localPosition = originalPosition + new Vector3(0f, 1000f, 0f);
			HOTween.To(itemController.transform, waitTime, new TweenParms().Prop("localPosition", originalPosition).Delay(delayTime).Ease(EaseType.EaseInExpo)
				.OnComplete(OnBoxLanded));
			delayTime += ((!isMultiBoxGacha) ? UnityEngine.Random.Range(0.1f, 0.3f) : 0.5f);
		}
		waitTime += delayTime;
		yield return new WaitForSeconds(waitTime + 0.3f);
		if (gachaResults == null)
		{
			Log.Debug("No gacha rewards yet. Let's wait.");
			int loadingPopupID = LoadingPopupManager.ShowLoadingPopup(0f);
			while (gachaResults == null)
			{
				yield return 0;
			}
			LoadingPopupManager.ClearLoadingPopup(loadingPopupID);
		}
		if (!isMultiBoxGacha)
		{
			for (int j = 0; j < gachaItemControllers.Length; j++)
			{
				gachaItemControllers[j].uiButton.enabled = true;
			}
		}
		AudioTrigger.CrowdCheering.Play();
	}

	private void OnBoxLanded()
	{
		if (!isShowingSummary)
		{
			screenshakeTarget.Shake();
			if (gachaType == GachaTypes.PREMIUM)
			{
				AudioTrigger.CrateLandPremium.Play();
			}
			else
			{
				AudioTrigger.CrateLand.Play();
			}
			if (isMultiBoxGacha && itemsCache.Count > 0)
			{
				GachaTankRequest(itemsCache[0]);
				itemsCache.RemoveAt(0);
			}
		}
	}

	private void OnBoxReplace()
	{
		if (!isShowingSummary)
		{
			screenshakeTarget.Shake();
			if (gachaType == GachaTypes.PREMIUM)
			{
				AudioTrigger.CrateLandPremium.Play();
			}
			else
			{
				AudioTrigger.CrateLand.Play();
			}
			currentGachaResults++;
			if (gachaResults.Count > 0)
			{
				itemsCache[0].GachaResult(gachaResults[currentGachaResults - 1]);
				itemsCache.RemoveAt(0);
			}
			CheckTankCount();
		}
	}

	private void SetGachaResults(ItemCollectionDataModel itemCollection)
	{
		gachaResults = new List<ItemCollectionDataModel.Item>();
		foreach (ItemCollectionDataModel.Item item in itemCollection.items)
		{
			gachaResults.Add(item);
		}
		GACHA_RESULT_COUNT_LIMIT = gachaResults.Count;
		if ((bool)openBoxesMessage)
		{
			string empty = string.Empty;
			if (GACHA_RESULT_COUNT_LIMIT == 1)
			{
				empty = "2".Localize("CHOOSE 1 BOX");
			}
			else if (isMultiBoxGacha)
			{
				empty = string.Empty;
				SkipButton.SetActive(true);
			}
			else
			{
				empty = "1".Localize("CHOOSE {0} BOXES");
				empty = string.Format(empty, GACHA_RESULT_COUNT_LIMIT);
			}
			openBoxesMessage.gameObject.SetActive(true);
			openBoxesMessage.text = empty;
		}
	}

	public void OkPopUpButtonPressed()
	{
		SceneController.resumeCallbackEnable = true;
		SceneTransitionManager.ClearHistory();
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene, null, false);
	}

	public void GachaTankRequest(GachaItemController itemController)
	{
		if (currentGachaResults < gachaResults.Count)
		{
			currentGachaResults++;
			itemController.GachaResult(gachaResults[currentGachaResults - 1]);
			if (isMultiBoxGacha)
			{
				StartCoroutine(ReplaceBox(itemController));
			}
			CheckTankCount();
		}
	}

	private void CheckTankCount()
	{
		if (currentGachaResults >= GACHA_RESULT_COUNT_LIMIT)
		{
			StartCoroutine(ShowSummaryPopUpWithDelay(4f));
		}
	}

	private IEnumerator ReplaceBox(GachaItemController itemController)
	{
		float waitTime = ((!isMultiBoxGacha) ? 0.8f : 1f);
		float delayTime = 0f;
		while (itemController.Animating)
		{
			yield return new WaitForEndOfFrame();
		}
		itemController.ResetBox();
		itemsCache.Add(itemController);
		yield return new WaitForEndOfFrame();
		Vector3 originalPosition = itemController.transform.localPosition;
		itemController.transform.localPosition = originalPosition + new Vector3(0f, 1000f, 0f);
		HOTween.To(itemController.transform, waitTime, new TweenParms().Prop("localPosition", originalPosition).Delay(delayTime).Ease(EaseType.EaseInExpo)
			.OnComplete(OnBoxReplace));
		delayTime += ((!isMultiBoxGacha) ? UnityEngine.Random.Range(0.1f, 0.3f) : 0.5f);
	}

	private void OnSkipButtonPressed()
	{
		isShowingSummary = true;
		StartCoroutine(ShowSummaryPopUpWithDelay(0f));
	}

	private IEnumerator ShowSummaryPopUpWithDelay(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		base.SectionTitle = "Summary";
		base.ShowTopBar = true;
		SkipButton.SetActive(false);
		gachaButtonsParent.SetActive(false);
		if ((bool)openBoxesMessage)
		{
			openBoxesMessage.gameObject.SetActive(false);
		}
		if (gachaResults.Count != currentGachaResults && !isShowingSummary)
		{
			Log.Error("Gacha units returned from the server are not equal to local current gacha tanks", base.gameObject);
		}
		PopupManager.ShowPopup(PopupDataModel.GachaResult((GachaRewardsSceneModel)sceneModel.payload, "ui_gacha_result_title_2", OkPopUpButtonPressed));
		gachaResults.Clear();
	}

	private IEnumerator PlaySoundWithDelay(float delay, AudioTrigger soundToPlay)
	{
		yield return new WaitForSeconds(delay);
		soundToPlay.Play();
	}
}
