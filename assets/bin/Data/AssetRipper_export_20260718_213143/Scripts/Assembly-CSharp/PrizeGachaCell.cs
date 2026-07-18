using System;
using System.Collections;
using UnityEngine;

public class PrizeGachaCell : ScrollableCell
{
	private const string OFFER_ATTACH_POINT_NAME = "offer";

	private const string DAILY_ATTACH_POINT_NAME = "daily";

	private const string GACHA_FREE_STRING_KEY_NAME = "ui_gacha_free";

	private const string GACHA_PLAY_STRING_KEY_NAME = "ui_home_play_button";

	private const string CONTRACT_REGULAR_SPRITENAME = "BG_gacha_02";

	private const string CONTRACT_RARE_SPRITENAME = "BG_gacha_01";

	private const string CONTRACT_EXCLUSIVE_SPRITENAME = "BG_gacha_03";

	private static float lastAuthorizeTime;

	public Color goldShineColor = Color.white;

	public Color silverShineColor = Color.white;

	public Color availableColor = Color.white;

	public Color unavailableColor = Color.white;

	public Color specialStateColor = Color.white;

	[SerializeField]
	private tk2dTextMesh title;

	[SerializeField]
	private tk2dTextMesh description;

	[SerializeField]
	private tk2dTextMesh coolDown;

	[SerializeField]
	private tk2dTextMesh countDown;

	[SerializeField]
	private tk2dTextMesh offerLabel;

	[SerializeField]
	private tk2dSlicedSprite spriteBackground;

	[SerializeField]
	private tk2dSlicedSprite spriteShineBackground;

	[SerializeField]
	private tk2dSlicedSprite shine;

	[SerializeField]
	private tk2dClippedSprite spriteTimerBar;

	[SerializeField]
	private tk2dSlicedSprite spriteTimerShadowBar;

	[SerializeField]
	private tk2dSlicedSprite spritePriceBar;

	[SerializeField]
	private tk2dBaseSprite countdownAvailable;

	[SerializeField]
	private tk2dUITweenItem cellButtonTween;

	[SerializeField]
	private tk2dUIItem cellButtonItem;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private PrefabProxy gachaPrizeSprite;

	[SerializeField]
	private tk2dBaseSprite[] screws;

	[SerializeField]
	private GameObject fastForwardButton;

	[SerializeField]
	private GameObject timerAssets;

	[SerializeField]
	private tk2dClippedSprite _coolDownProgressBar;

	public GachaPoolsDataModel prizeGachaPoolDataModel;

	public new bool enabled = true;

	private PrizeGachaCellStates currentState;

	private UserGachaPrize currentUserGachaPrize;

	private int loadingPopupID;

	private GachaRewardsSceneModel gachaRewardsSceneModel;

	private Vector2 spriteBackgroundDimensionsDefault;

	private Vector2 spriteShineBackgroundDefault;

	private Vector2 spritePriceBarDefault;

	private Vector2 shineDefault;

	private Vector3[] screwsDefault;

	private Vector3 colliderSizeDefault;

	private float ratio;

	private void Awake()
	{
		spriteBackgroundDimensionsDefault = spriteBackground.dimensions;
		spriteShineBackgroundDefault = spriteShineBackground.dimensions;
		spritePriceBarDefault = spritePriceBar.dimensions;
		shineDefault = shine.dimensions;
		ratio = spriteTimerBar.scale.x / spriteTimerShadowBar.dimensions.x;
		screwsDefault = new Vector3[4];
		screwsDefault[0] = screws[0].transform.localPosition;
		screwsDefault[1] = screws[1].transform.localPosition;
		screwsDefault[2] = screws[2].transform.localPosition;
		screwsDefault[3] = screws[3].transform.localPosition;
		colliderSizeDefault = ((BoxCollider)cellButtonItem.collider).size;
	}

	private void Update()
	{
		if (currentState == PrizeGachaCellStates.COOLDOWN)
		{
			if ((bool)coolDown && currentUserGachaPrize != null)
			{
				coolDown.text = TimeFormats.GetTimeString(currentUserGachaPrize.GetRemainingTime(), TimeFormat.NUMBER);
				if (currentUserGachaPrize.CooldownProgress >= 1f)
				{
					SetStateAvailable();
				}
				else if ((bool)_coolDownProgressBar)
				{
					_coolDownProgressBar.clipTopRight = new Vector2(0f, 1f);
					_coolDownProgressBar.clipBottomLeft = new Vector2(currentUserGachaPrize.CooldownProgress, 0f);
				}
			}
		}
		else if (currentState == PrizeGachaCellStates.COUNTDOWN && (bool)countDown && currentUserGachaPrize != null)
		{
			countDown.text = TimeFormats.GetTimeString(currentUserGachaPrize.GetRemainingTime(), TimeFormat.NUMBER);
			if (currentUserGachaPrize.CountDownProgress >= 1f)
			{
				SetStateDivisionCountdownNotAvailable();
			}
		}
	}

	public override void ConfigureCell()
	{
		if (dataObject != null)
		{
			ConfigureCellData();
		}
	}

	public override void ConfigureCellData()
	{
		if (dataObject == null)
		{
			return;
		}
		if ((bool)offerLabel)
		{
			offerLabel.gameObject.SetActive(false);
		}
		prizeGachaPoolDataModel = (GachaPoolsDataModel)dataObject;
		if (prizeGachaPoolDataModel == null)
		{
			SetStateNone();
			return;
		}
		if ((bool)title)
		{
			title.text = prizeGachaPoolDataModel.Name;
		}
		if ((bool)description)
		{
			description.text = prizeGachaPoolDataModel.Description;
		}
		UserGachaPrize userGachaPrize = (currentUserGachaPrize = UserProfile.player.GetGachaPrizeData(int.Parse(prizeGachaPoolDataModel.ID)));
		if (userGachaPrize.IsUnlockedByTier)
		{
			SetStateDivisionCountdown();
		}
		else if (userGachaPrize.IsOnCooldown)
		{
			SetStateCooldown();
		}
		else
		{
			SetStateAvailable();
		}
		StartCoroutine(SetGachaSprite());
	}

	private void AdjustUIElementHeightsCustomizedSize()
	{
		Vector2 dimensions = spriteBackground.dimensions;
		dimensions.x = cellWidth;
		spriteBackground.dimensions = dimensions;
		spriteShineBackground.dimensions = dimensions + new Vector2(40f, 40f);
		spritePriceBar.dimensions = new Vector2(dimensions.x - 18f, spritePriceBar.dimensions.y);
		spriteTimerBar.scale = new Vector3(ratio * (dimensions.x - 18f), spriteTimerBar.scale.y, spriteTimerBar.scale.z);
		spriteTimerShadowBar.dimensions = new Vector2(dimensions.x - 18f, spritePriceBar.dimensions.y);
		shine.dimensions = new Vector2(dimensions.x, shine.dimensions.y);
		int num = 40;
		float num2 = cellWidth / 2f - (float)num;
		float xPosition = num2 * -1f;
		screws[0].transform.SetLocalXPosition(xPosition);
		screws[1].transform.SetLocalXPosition(num2);
		screws[2].transform.SetLocalXPosition(xPosition);
		screws[3].transform.SetLocalXPosition(num2);
		((BoxCollider)cellButtonItem.collider).size = new Vector3(dimensions.x - 18f, colliderSizeDefault.y, colliderSizeDefault.z);
	}

	private void AdjustUIElementHeightsDefaultSize()
	{
		spriteBackground.dimensions = spriteBackgroundDimensionsDefault;
		spriteShineBackground.dimensions = spriteShineBackgroundDefault;
		spritePriceBar.dimensions = spritePriceBarDefault;
		shine.dimensions = shineDefault;
		screws[0].transform.localPosition = screwsDefault[0];
		screws[1].transform.localPosition = screwsDefault[1];
		screws[2].transform.localPosition = screwsDefault[2];
		screws[3].transform.localPosition = screwsDefault[3];
		((BoxCollider)cellButtonItem.collider).size = colliderSizeDefault;
	}

	private void AdjustUIElementHeights()
	{
		AdjustUIElementHeightsCustomizedSize();
	}

	private IEnumerator SetGachaSprite()
	{
		yield return StartCoroutine(gachaPrizeSprite.ChangeAssetCoroutine(prizeGachaPoolDataModel.AssetLinkage));
		while (!gachaPrizeSprite.AssetReady)
		{
			yield return 0;
		}
		tk2dSprite gachaSprite = gachaPrizeSprite.Prefab.GetComponentInChildren<tk2dSprite>();
		if ((bool)gachaSprite)
		{
			tk2dSpriteDefinition.AttachPoint setupAttachPoint = gachaSprite.GetAttachPointByName("offer");
			if (setupAttachPoint != null && (bool)offerLabel)
			{
				offerLabel.gameObject.SetActive(true);
				offerLabel.transform.localPosition = setupAttachPoint.position;
			}
			setupAttachPoint = gachaSprite.GetAttachPointByName("daily");
			ConfigureBackground(setupAttachPoint != null);
		}
	}

	private void SetStateNone()
	{
		AdjustUIElementHeights();
		if ((bool)title)
		{
			title.text = "Error!";
		}
		if ((bool)description)
		{
			description.text = "Error - Error - Error";
		}
		if ((bool)coolDown)
		{
			coolDown.gameObject.SetActive(true);
			coolDown.text = "Error!";
		}
		if ((bool)spriteShineBackground)
		{
			spriteShineBackground.gameObject.SetActive(false);
		}
		if ((bool)spritePriceBar)
		{
			spritePriceBar.color = unavailableColor;
		}
		priceLabel.gameObject.SetActive(false);
		if ((bool)cellButtonTween)
		{
			cellButtonTween.enabled = false;
		}
	}

	private void SetStateDivisionCountdownNotAvailable()
	{
		if ((bool)spritePriceBar)
		{
			spritePriceBar.color = unavailableColor;
		}
		if ((bool)spriteShineBackground)
		{
			spriteShineBackground.gameObject.SetActive(false);
		}
		if ((bool)coolDown)
		{
			coolDown.gameObject.SetActive(true);
			coolDown.text = "ui_limited_gacha_offer_expired".Localize("OFFER EXPIRED!");
		}
		priceLabel.gameObject.SetActive(false);
		countdownAvailable.gameObject.SetActive(false);
		if ((bool)cellButtonTween)
		{
			cellButtonTween.enabled = false;
		}
		if ((bool)cellButtonItem)
		{
			cellButtonItem.enabled = false;
		}
	}

	private void SetStateDivisionCountdown()
	{
		AdjustUIElementHeights();
		currentState = PrizeGachaCellStates.COUNTDOWN;
		if ((bool)countDown)
		{
			countDown.text = TimeFormats.GetTimeString(currentUserGachaPrize.GetRemainingTime(), TimeFormat.NUMBER);
			coolDown.gameObject.SetActive(false);
		}
		if ((bool)spritePriceBar)
		{
			spritePriceBar.color = availableColor;
		}
		if ((bool)spriteShineBackground)
		{
			spriteShineBackground.gameObject.SetActive(true);
		}
		if ((bool)countdownAvailable)
		{
			countdownAvailable.gameObject.SetActive(true);
		}
		if ((bool)fastForwardButton)
		{
			fastForwardButton.SetActive(false);
		}
		if ((bool)timerAssets)
		{
			timerAssets.SetActive(false);
		}
		priceLabel.gameObject.SetActive(true);
		if ((bool)cellButtonTween)
		{
			cellButtonTween.enabled = true;
		}
		if (IsPremiumGacha(prizeGachaPoolDataModel.GachaType))
		{
			priceLabel.gameObject.SetActive(true);
			priceLabel.ConfigurePriceLabel(prizeGachaPoolDataModel.GetPrice());
			if ((bool)spriteShineBackground)
			{
				spriteShineBackground.gameObject.SetActive(UserProfile.player.CanAfford(prizeGachaPoolDataModel.GetPrice()));
				spriteShineBackground.color = goldShineColor;
			}
		}
	}

	private void SetStateCooldown()
	{
		AdjustUIElementHeights();
		currentState = PrizeGachaCellStates.COOLDOWN;
		if ((bool)coolDown)
		{
			coolDown.gameObject.SetActive(true);
			coolDown.text = TimeFormats.GetTimeString(currentUserGachaPrize.GetRemainingTime(), TimeFormat.NUMBER);
		}
		if ((bool)spritePriceBar)
		{
			spritePriceBar.color = unavailableColor;
		}
		if ((bool)spriteShineBackground)
		{
			spriteShineBackground.gameObject.SetActive(false);
		}
		if (prizeGachaPoolDataModel.GachaType == GachaTypes.REGULAR)
		{
			if ((bool)fastForwardButton)
			{
				fastForwardButton.SetActive(true);
			}
			if ((bool)timerAssets)
			{
				timerAssets.SetActive(true);
			}
		}
		priceLabel.gameObject.SetActive(false);
		countdownAvailable.gameObject.SetActive(false);
		if ((bool)cellButtonTween)
		{
			cellButtonTween.enabled = false;
		}
	}

	private void SetStateAvailable()
	{
		AdjustUIElementHeights();
		currentState = PrizeGachaCellStates.AVAILABLE;
		if ((bool)fastForwardButton)
		{
			fastForwardButton.SetActive(false);
		}
		if ((bool)timerAssets)
		{
			timerAssets.SetActive(false);
		}
		if ((bool)cellButtonTween)
		{
			cellButtonTween.enabled = true;
		}
		switch (prizeGachaPoolDataModel.GachaType)
		{
		case GachaTypes.PREMIUM:
		case GachaTypes.UNLOCKBYTIER:
		case GachaTypes.EVENT:
		case GachaTypes.STEPGACHA:
			countdownAvailable.gameObject.SetActive(false);
			coolDown.gameObject.SetActive(false);
			priceLabel.gameObject.SetActive(true);
			priceLabel.ConfigurePriceLabel(prizeGachaPoolDataModel.GetPrice());
			if ((bool)spriteShineBackground)
			{
				spriteShineBackground.gameObject.SetActive(UserProfile.player.CanAfford(prizeGachaPoolDataModel.GetPrice()));
				spriteShineBackground.color = goldShineColor;
			}
			if ((bool)spritePriceBar)
			{
				spritePriceBar.color = availableColor;
			}
			break;
		case GachaTypes.ITEMS_MIXER:
			countdownAvailable.gameObject.SetActive(false);
			coolDown.gameObject.SetActive(true);
			coolDown.text = "ui_home_play_button".Localize("PLAY!");
			priceLabel.gameObject.SetActive(false);
			if ((bool)spritePriceBar)
			{
				spritePriceBar.color = specialStateColor;
			}
			break;
		case GachaTypes.REGULAR:
			if ((bool)spriteShineBackground)
			{
				spriteShineBackground.gameObject.SetActive(true);
				spriteShineBackground.color = silverShineColor;
			}
			coolDown.gameObject.SetActive(true);
			coolDown.text = "ui_gacha_free".Localize("FREE!");
			priceLabel.gameObject.SetActive(false);
			countdownAvailable.gameObject.SetActive(false);
			if ((bool)spritePriceBar)
			{
				spritePriceBar.color = availableColor;
			}
			break;
		default:
			Log.Error("Unsupported state");
			break;
		}
	}

	private void BuyGachaPrizeRequest()
	{
		if (!enabled || Time.time - lastAuthorizeTime < 2f || prizeGachaPoolDataModel == null || PopupManager.Acting)
		{
			return;
		}
		if ((bool)cellButtonItem)
		{
			cellButtonItem.enabled = false;
		}
		ShopItemsSceneController.instance.ChangeListButtonStatus(false);
		GachaTypes gachaType = ((prizeGachaPoolDataModel.freeCooldown > 0) ? GachaTypes.REGULAR : GachaTypes.PREMIUM);
		gachaRewardsSceneModel = new GachaRewardsSceneModel(gachaType);
		if (prizeGachaPoolDataModel.GachaType == GachaTypes.ITEMS_MIXER)
		{
			ShopItemsSceneController.instance.ChangeListButtonStatus(false);
			AuthorizeTransaction(ResumeScene);
			return;
		}
		if (!currentUserGachaPrize.CanPlayGachaPrize)
		{
			if (currentUserGachaPrize.IsOnCooldown)
			{
				ShopItemsSceneController.instance.ChangeListButtonStatus(true);
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
				{
					ResumeScene();
					PopupManager.DestroyAllPopups();
					TopBarController.instance.LoadShop();
				}, ResumeScene, ResumeScene));
			}
			if ((bool)cellButtonItem)
			{
				cellButtonItem.enabled = true;
			}
			return;
		}
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
		if (prizeGachaPoolDataModel.GachaType == GachaTypes.EVENT && (activeOnCooldownEvent == null || prizeGachaPoolDataModel.eventId != int.Parse(activeOnCooldownEvent.id)))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("key_title_expired_gacha_event".Localize("CONTRACT EXPIRED:"), "key_description_expired_gacha_event".Localize("The Contract you attempted to purchase has expired."), ResumeScene));
			return;
		}
		if (prizeGachaPoolDataModel.GetPrice().items.Count > 0)
		{
			string msgBody = string.Format("ui_prizegacha_confirmation_text".Localize("CHOOSE {0} PRIZES FROM OUR SPONSORS?"), prizeGachaPoolDataModel.boxCount);
			PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(prizeGachaPoolDataModel.GetPrice(), "ui_prizegacha_confirmation_title".Localize("Confirmation"), msgBody, delegate
			{
				AuthorizeTransaction(ResumeScene);
			}, null, ResumeScene));
		}
		else
		{
			ShopItemsSceneController.instance.ChangeListButtonStatus(false);
			AuthorizeTransaction(ResumeScene);
		}
		if ((bool)cellButtonItem)
		{
			cellButtonItem.enabled = true;
		}
	}

	private void ConfigureBackground(bool isExclusive)
	{
		if ((bool)spriteBackground)
		{
			if (isExclusive)
			{
				spriteBackground.SetSprite("BG_gacha_03");
				return;
			}
			bool flag = IsPremiumGacha(prizeGachaPoolDataModel.GachaType);
			spriteBackground.SetSprite((!flag) ? "BG_gacha_02" : "BG_gacha_01");
		}
	}

	private void ResumeScene()
	{
		if (ShopItemsSceneController.instance != null)
		{
			ShopItemsSceneController.instance.ChangeListButtonStatus(true);
		}
	}

	private void AuthorizeTransaction(Action unlockSceneCallback = null)
	{
		if (Time.time - lastAuthorizeTime < 2f)
		{
			return;
		}
		lastAuthorizeTime = Time.time;
		AudioTrigger.CrowdCheering.Play();
		if (prizeGachaPoolDataModel.GachaType == GachaTypes.ITEMS_MIXER)
		{
			gachaRewardsSceneModel.gachaType = GachaTypes.ITEMS_MIXER;
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ItemsMixer, new ItemMixerSceneModel(gachaRewardsSceneModel), unlockSceneCallback);
			Singleton<SessionManager>.instance.GetNextItemsMixerSet(prizeGachaPoolDataModel.eventId, delegate(ItemCollectionDataModel result)
			{
				gachaRewardsSceneModel.SetRewards(result, prizeGachaPoolDataModel);
			});
		}
		else
		{
			Singleton<SessionManager>.instance.BuyPrizeGacha(prizeGachaPoolDataModel, null, OnBuyGachaPrizeResponse);
			TopBarController.instance.Hide(0f);
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.GachaScene, new SceneModel(gachaRewardsSceneModel), false, unlockSceneCallback);
		}
	}

	private void OnBuyGachaPrizeResponse(GachaResult gachaResult)
	{
		Reporting.GachaEvent(currentUserGachaPrize.gachaPrizeID, gachaResult.itemCollection, prizeGachaPoolDataModel.GetPrice());
		gachaRewardsSceneModel.SetRewards(gachaResult.itemCollection, prizeGachaPoolDataModel);
	}

	public void OnPressInfo()
	{
		if (!SceneTransitionManager.transitionActive)
		{
			string text = (prizeGachaPoolDataModel.keyName + "_popUp").Localize("Gacha Prizes!");
			string text2 = "prize_gacha_info_description".Localize("You'll have a chance to get any of the following:");
			Reporting.AccessDetails(prizeGachaPoolDataModel.ID);
			PopupManager.ShowPopup(PopupDataModel.GachaInfoPopUp(text, text2, prizeGachaPoolDataModel, null));
		}
	}

	public void OnPressFastForward()
	{
		if (!enabled)
		{
			return;
		}
		ShopItemsSceneController.instance.ChangeListButtonStatus(false);
		Singleton<SponsorPayManager>.instance.RequestBrandEngage(delegate(SponsorPayManager.BrandEngageResult result)
		{
			if (result == SponsorPayManager.BrandEngageResult.Success)
			{
				Singleton<SessionManager>.instance.FastForwardGacha(prizeGachaPoolDataModel.id, OnPressFastForwardComplete);
			}
		});
	}

	private void OnPressFastForwardComplete()
	{
		StartCoroutine(OnPressFastForwardCompleteCoroutine());
	}

	private IEnumerator OnPressFastForwardCompleteCoroutine()
	{
		long count = 0L;
		int decreseFactor = 300000;
		while (count < Constants.VideoAdsDecreaseGachaTime)
		{
			if (count + decreseFactor > Constants.VideoAdsDecreaseGachaTime)
			{
				currentUserGachaPrize.startTime -= Constants.VideoAdsDecreaseGachaTime - count;
			}
			else
			{
				currentUserGachaPrize.startTime -= decreseFactor;
			}
			count += decreseFactor;
			yield return new WaitForSeconds(0.01f);
		}
	}

	private bool IsPremiumGacha(GachaTypes gachaType)
	{
		return gachaType == GachaTypes.EVENT || gachaType == GachaTypes.PREMIUM || gachaType == GachaTypes.STEPGACHA || gachaType == GachaTypes.UNLOCKBYTIER;
	}
}
