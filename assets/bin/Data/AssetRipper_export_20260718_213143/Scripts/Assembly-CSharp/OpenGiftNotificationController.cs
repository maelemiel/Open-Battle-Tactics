using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class OpenGiftNotificationController : SceneController
{
	[SerializeField]
	private GenericGachaBoxController gachaBox;

	[SerializeField]
	private tk2dTextMesh unlockText;

	[SerializeField]
	private tk2dTextMesh titleText;

	[SerializeField]
	private tk2dSprite abilityIcon;

	[SerializeField]
	private tk2dSprite currencyIcon;

	[SerializeField]
	private tk2dTextMesh currencyLabel;

	[SerializeField]
	private AnnouncerSpeechController announcerSpeechController;

	[SerializeField]
	private ScrollableCell unitContainer;

	private OpenGiftSequenceHandler sequenceHandler;

	private readonly Dictionary<GiftType, OpenGiftSequenceHandler> giftSequenceHandlers = new Dictionary<GiftType, OpenGiftSequenceHandler>
	{
		{
			GiftType.TUTORIAL_ONE_GIFT,
			new FirstGiftSequence()
		},
		{
			GiftType.TUTORIAL_TWO_GIFT_1,
			new SecondGiftSequence()
		},
		{
			GiftType.TUTORIAL_TWO_GIFT_2,
			new ThirdGiftSequence()
		},
		{
			GiftType.TUTORIAL_THREE_GIFT_1,
			new FourthGiftSequence()
		},
		{
			GiftType.TUTORIAL_THREE_GIFT_2,
			new FifthGiftSequence()
		},
		{
			GiftType.TUTORIAL_THREE_GIFT_3,
			new SixthGiftSequence()
		}
	};

	public override void Awake()
	{
		_showHomeButton = false;
		_showTopBar = false;
		base.Awake();
		base.SectionTitle = string.Empty;
		unlockText.Alpha = 0f;
		abilityIcon.gameObject.SetActive(false);
		currencyIcon.gameObject.SetActive(false);
		currencyLabel.gameObject.SetActive(false);
		unitContainer.gameObject.SetActive(false);
	}

	private void Start()
	{
		AudioTrigger.StandardCrowd.PlayMusic();
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
		if (sceneModel != null && sceneModel.payload is UserGiftDataModel && ((UserGiftDataModel)sceneModel.payload).items[0].itemType == UserInventory.ItemType.Unit)
		{
			gachaBox.gameObject.SetActive(false);
		}
	}

	private void Init()
	{
		object obj = null;
		if (sceneModel != null)
		{
			obj = sceneModel.payload;
		}
		if (obj == null)
		{
			obj = new UserGiftDataModel(UserInventory.ItemType.Unit, 130010101, 1);
			gachaBox.gameObject.SetActive(false);
			sequenceHandler = giftSequenceHandlers[GiftType.TUTORIAL_ONE_GIFT];
			if (sequenceHandler != null)
			{
				sequenceHandler.Init(this, announcerSpeechController);
			}
		}
		if (sceneModel is OpenGiftSceneModel)
		{
			sequenceHandler = giftSequenceHandlers[(sceneModel as OpenGiftSceneModel).giftType];
			if (sequenceHandler != null)
			{
				sequenceHandler.Init(this, announcerSpeechController);
			}
		}
		StartCoroutine(StartSequence(obj));
	}

	private IEnumerator StartSequence(object payload)
	{
		titleText.gameObject.SetActive(false);
		if (sequenceHandler != null)
		{
			yield return StartCoroutine(sequenceHandler.StartSequence());
		}
		titleText.gameObject.SetActive(true);
		if (payload is AbilityDataModel)
		{
			ConfigureAbility(payload as AbilityDataModel);
		}
		else
		{
			if (!(payload is UserGiftDataModel))
			{
				yield break;
			}
			UserGiftDataModel gift = payload as UserGiftDataModel;
			ItemCollectionDataModel.Item giftItem = gift.items[0];
			if (giftItem.itemType == UserInventory.ItemType.Unit)
			{
				UnitLevelProgressionDataModel unit = giftItem.Unit;
				ConfigureUnit(unit);
			}
			else if (giftItem.itemType.IsCurrency())
			{
				ConfigureCurrency(giftItem.itemType, giftItem.amount);
				gachaBox.uiButton.OnClick += delegate
				{
					StartCoroutine(OpenSequence());
				};
			}
		}
	}

	private void ConfigureAbility(AbilityDataModel ability)
	{
		abilityIcon.gameObject.SetActive(true);
		unlockText.text = "New Ability Unlocked !";
		abilityIcon.SetSprite(ability.ButtonIconArtName);
		gachaBox.uiButton.OnClick += delegate
		{
			StartCoroutine(ClaimAbilitySequence(ability));
		};
	}

	private void ConfigureUnit(UnitLevelProgressionDataModel unit)
	{
		gachaBox.gameObject.SetActive(false);
		unitContainer.gameObject.SetActive(true);
		unitContainer.DataObject = unit.UnitDataModel;
		unitContainer.ConfigureCell();
		unitContainer.GetComponent<tk2dUIItem>().OnClick += delegate
		{
			StartCoroutine(ClaimUnitSequence(unit));
		};
		unlockText.text = "New Unit Acquired !";
	}

	private void ConfigureCurrency(UserInventory.ItemType type, int amount)
	{
		currencyIcon.gameObject.SetActive(true);
		currencyLabel.gameObject.SetActive(true);
		unlockText.text = amount + " " + type.GetLocalizedName() + ", Awesome !";
		currencyIcon.SetSprite(type.GetIconName());
		currencyLabel.text = amount.ToString();
	}

	private IEnumerator OpenSequence()
	{
		AudioTrigger.CrateBreak.Play();
		AudioTrigger.CrowdExcited.Play();
		titleText.gameObject.SetActive(false);
		gachaBox.Open();
		Vector3 startPos = unlockText.transform.localPosition;
		SimpleTween.Start(1f, 0f, 0.5f, EaseType.EaseOutExpo, delegate(float val)
		{
			unlockText.transform.localPosition = startPos + new Vector3(0f, 100f, 0f) * val;
			unlockText.Alpha = 1f - val;
		});
		yield return new WaitForSeconds(3f);
		unlockText.gameObject.SetActive(false);
		if (sequenceHandler != null)
		{
			yield return StartCoroutine(sequenceHandler.GiftOpenedSequence());
		}
		UserNotification.ExecuteNotifications();
	}

	private IEnumerator ClaimUnitSequence(object payload)
	{
		AudioTrigger.GachaSuperRareRevealed.Play();
		AudioTrigger.CrowdExcited.Play();
		titleText.gameObject.SetActive(false);
		UnitLevelProgressionDataModel unitLevelProgressionDataModel = (UnitLevelProgressionDataModel)payload;
		unitContainer.gameObject.SetActive(false);
		PopupManager.ShowPopup(PopupDataModel.ClaimUnitPopUp(unitLevelProgressionDataModel.UnitDataModel, null, delegate
		{
			StartCoroutine(FinalClaimSequence());
		}));
		yield break;
	}

	private IEnumerator ClaimAbilitySequence(object payload)
	{
		AudioTrigger.CrateBreak.Play();
		AudioTrigger.CrowdExcited.Play();
		titleText.gameObject.SetActive(false);
		AbilityDataModel abilityDataModel = (AbilityDataModel)payload;
		gachaBox.Open();
		yield return new WaitForSeconds(1.5f);
		PopupManager.ShowPopup(PopupDataModel.InspectAbilityPopUp(abilityDataModel, delegate
		{
			StartCoroutine(FinalClaimSequence());
		}));
	}

	private IEnumerator FinalClaimSequence()
	{
		if (sequenceHandler != null)
		{
			yield return StartCoroutine(sequenceHandler.GiftOpenedSequence());
		}
		UserNotification.ExecuteNotifications();
	}
}
