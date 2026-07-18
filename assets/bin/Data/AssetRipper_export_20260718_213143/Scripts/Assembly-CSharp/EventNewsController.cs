using System;
using System.Collections;
using UnityEngine;

public class EventNewsController : NewsController
{
	[SerializeField]
	private tk2dSpineAnimation _logoAnimation;

	[SerializeField]
	private GameObject _chineseLogo;

	[SerializeField]
	protected EventLogoController _eventLogoController;

	[SerializeField]
	private EventBackgroundController _eventBackgroundController;

	[SerializeField]
	protected EventCountDownController _eventCountDownController;

	[SerializeField]
	protected UnitProxy _eventUnitProxy;

	[SerializeField]
	private GameObject _eventContentGameObject;

	[SerializeField]
	private GameObject _loadingIcon;

	[SerializeField]
	private GameObject _environmentalEffect;

	[SerializeField]
	private UnitProxy _eventUnitProxy2;

	[SerializeField]
	private EnterFromController _eventLogoEFC;

	[SerializeField]
	private EnterFromController _unit1LogoEFC;

	[SerializeField]
	private EnterFromController _unit2LogoEFC;

	protected int _randomDisplay;

	protected bool _finishInit;

	protected EventDataModel currentEventDataModel;

	protected EventBlueprintsState currentEventState = EventBlueprintsState.NONE;

	protected Vector3 initialLogoPosition = Vector3.zero;

	protected Vector3 _announcerOnscreenPos;

	private BasicWeaponController _weaponController1;

	private BasicWeaponController _weaponController2;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)_environmentalEffect)
		{
			_environmentalEffect.SetActive(false);
		}
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		int backgroundSpeed = newsDM.backgroundSpeed;
		_randomDisplay = UnityEngine.Random.Range(0, 2);
		if (_randomDisplay == 1)
		{
			newsDM.backgroundSpeed = 0;
		}
		yield return StartCoroutine(base.Init(newsDM));
		newsDM.backgroundSpeed = backgroundSpeed;
		currentEventDataModel = FetchEventDataModel();
		ConfigureEventState();
		if ((bool)_loadingIcon)
		{
			_loadingIcon.SetActive(true);
		}
		if ((bool)_eventCountDownController)
		{
			EventCountDownController eventCountDownController = _eventCountDownController;
			eventCountDownController.AlertLessThanZero = (Action)Delegate.Combine(eventCountDownController.AlertLessThanZero, new Action(ReachedZeroTimer));
		}
		if (UserProfile.player.divisionInt <= Constants.TierEventNewsLockOut)
		{
			currentEventState = EventBlueprintsState.NO_EVENT;
		}
		switch (currentEventState)
		{
		case EventBlueprintsState.EVENT_COOLDOWN:
			_eventUnitProxy.gameObject.SetActive(false);
			_eventUnitProxy2.gameObject.SetActive(false);
			if ((bool)_loadingIcon)
			{
				_loadingIcon.SetActive(false);
			}
			if (LocalizationManager.LanguageEncodingType() == LocalizationManager.LanguageEncoding.Unicode && Singleton<LocalizationManager>.instance.currentLanguageFromEnum == LocalizationManager.Language.Chinese)
			{
				if ((bool)_logoAnimation)
				{
					_logoAnimation.gameObject.SetActive(false);
				}
				if ((bool)_chineseLogo)
				{
					_chineseLogo.SetActive(true);
				}
			}
			break;
		case EventBlueprintsState.EVENT_ACTIVE:
			yield return StartCoroutine(SetupLogoAndBackground());
			if ((bool)_loadingIcon)
			{
				_loadingIcon.SetActive(false);
			}
			if ((bool)_eventUnitProxy)
			{
				yield return StartCoroutine(_eventUnitProxy.ChangeAssetCoroutine(currentEventDataModel.EventUnitAssetBundleId));
			}
			if (_randomDisplay == 0)
			{
				_eventUnitProxy2.gameObject.SetActive(false);
			}
			else
			{
				StartCoroutine(SetupUnitAndArmys());
			}
			break;
		case EventBlueprintsState.NO_EVENT:
			if ((bool)_loadingIcon)
			{
				_loadingIcon.SetActive(false);
			}
			break;
		case EventBlueprintsState.NEXT_EVENT:
			_eventUnitProxy.gameObject.SetActive(false);
			_eventUnitProxy2.gameObject.SetActive(false);
			if ((bool)_loadingIcon)
			{
				_loadingIcon.SetActive(false);
			}
			if (LocalizationManager.LanguageEncodingType() == LocalizationManager.LanguageEncoding.Unicode && Singleton<LocalizationManager>.instance.currentLanguageFromEnum == LocalizationManager.Language.Chinese)
			{
				if ((bool)_logoAnimation)
				{
					_logoAnimation.gameObject.SetActive(false);
				}
				if ((bool)_chineseLogo)
				{
					_chineseLogo.SetActive(true);
				}
			}
			break;
		}
		_finishInit = true;
	}

	public override void TvButtonPress()
	{
		if (currentEventState == EventBlueprintsState.EVENT_ACTIVE || currentEventState == EventBlueprintsState.EVENT_COOLDOWN)
		{
			Reporting.NewsTemplateClick(NewsTypes.EventNew, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.EventInfoPopUp.ToString());
			PopupManager.ShowPopup(PopupDataModel.EventInfoPopUp(currentEventDataModel, null));
		}
		else
		{
			base.TvButtonPress();
		}
	}

	public override void BeforeMovingInAction()
	{
		base.BeforeMovingInAction();
		if ((bool)_environmentalEffect)
		{
			_environmentalEffect.SetActive(false);
		}
	}

	public override void BeforeMovingOutAction()
	{
		base.BeforeMovingInAction();
		if ((bool)_environmentalEffect)
		{
			_environmentalEffect.SetActive(false);
		}
		if (_weaponController1 != null)
		{
			_weaponController1.InitReady = false;
		}
		if (_weaponController2 != null)
		{
			_weaponController2.InitReady = false;
		}
		_eventUnitProxy.gameObject.SetActive(false);
		_eventUnitProxy2.gameObject.SetActive(false);
	}

	public override IEnumerator AfterMovingInAction()
	{
		yield return StartCoroutine(base.AfterMovingInAction());
		if ((bool)_environmentalEffect)
		{
			_environmentalEffect.SetActive(true);
		}
		yield return StartCoroutine(FinishInit());
	}

	protected virtual IEnumerator FinishInit()
	{
		while (!_finishInit)
		{
			yield return 0;
		}
		switch (currentEventState)
		{
		case EventBlueprintsState.EVENT_COOLDOWN:
			if (LocalizationManager.LanguageEncodingType() == LocalizationManager.LanguageEncoding.Unicode && Singleton<LocalizationManager>.instance.currentLanguageFromEnum == LocalizationManager.Language.Chinese)
			{
				OutroSequence(null);
			}
			else if ((bool)_logoAnimation)
			{
				_logoAnimation.gameObject.SetActive(true);
				yield return new WaitForSeconds(1.5f);
				OutroSequence(null);
			}
			break;
		case EventBlueprintsState.EVENT_ACTIVE:
			yield return StartCoroutine(SetupEventContentOutro());
			break;
		case EventBlueprintsState.NO_EVENT:
			yield return StartCoroutine(SetupNormalLogo());
			break;
		case EventBlueprintsState.NEXT_EVENT:
			yield return StartCoroutine(SetupNormalLogo());
			break;
		}
	}

	private IEnumerator SetupNormalLogo()
	{
		if (LocalizationManager.LanguageEncodingType() == LocalizationManager.LanguageEncoding.Unicode && Singleton<LocalizationManager>.instance.currentLanguageFromEnum == LocalizationManager.Language.Chinese)
		{
			OutroSequence(null);
		}
		else if ((bool)_logoAnimation)
		{
			_logoAnimation.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.5f);
			OutroSequence(null);
		}
	}

	private void ReachedZeroTimer()
	{
		Log.DebugTag("Zero Time on Event Reached ", null, "EventContentControler");
		if ((bool)_eventCountDownController)
		{
			EventCountDownController eventCountDownController = _eventCountDownController;
			eventCountDownController.AlertLessThanZero = (Action)Delegate.Remove(eventCountDownController.AlertLessThanZero, new Action(ReachedZeroTimer));
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene, null, false, true);
	}

	private void OutroSequence(tk2dSpineAnimation animation)
	{
		StartCoroutine(SetupEventContentOutro());
	}

	public virtual IEnumerator SetupEventContentOutro()
	{
		switch (currentEventState)
		{
		case EventBlueprintsState.EVENT_ACTIVE:
			if ((bool)_eventLogoController)
			{
				_eventLogoController.gameObject.SetActive(true);
				yield return StartCoroutine(_eventLogoEFC.Init());
				OnLogoLanded();
			}
			else
			{
				TopBarController.instance.ShowProgressBanner = true;
			}
			if (_randomDisplay == 0)
			{
				if ((bool)_eventContentGameObject)
				{
					_eventContentGameObject.SetActive(true);
					_eventContentGameObject.transform.localScale = Vector3.zero;
					_eventContentGameObject.transform.TweenLocalScale(1f, 1f);
				}
				StartCoroutine(UnitDriveInSequence(2f));
			}
			else
			{
				if (_eventUnitProxy.AssetReady)
				{
					StartCoroutine(_unit1LogoEFC.Init());
				}
				if (_eventUnitProxy2.AssetReady)
				{
					StartCoroutine(_unit2LogoEFC.Init());
				}
				if (_weaponController1 != null && _weaponController2 != null)
				{
					StartCoroutine(ShootCoroutine());
				}
			}
			if ((bool)_eventCountDownController)
			{
				_eventCountDownController.gameObject.SetActive(true);
				_eventCountDownController.Init(currentEventDataModel, currentEventState);
			}
			break;
		case EventBlueprintsState.NEXT_EVENT:
			if ((bool)_eventCountDownController)
			{
				_eventCountDownController.gameObject.SetActive(true);
				_eventCountDownController.Init(currentEventDataModel, currentEventState);
			}
			break;
		}
	}

	private void OnLogoLanded()
	{
		if ((bool)_logoAnimation)
		{
			_logoAnimation.gameObject.SetActive(false);
		}
		if ((bool)_chineseLogo)
		{
			_chineseLogo.SetActive(false);
		}
		if (TvShaker != null)
		{
			TvShaker();
		}
		AudioTrigger.CrateLand.Play();
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
	}

	private IEnumerator UnitDriveInSequence(float tweenTime)
	{
		if ((bool)_eventUnitProxy)
		{
			Vector3 currentLocalScale = _eventUnitProxy.gameObject.transform.localScale;
			currentLocalScale.x = 0f - currentLocalScale.x;
			_eventUnitProxy.gameObject.transform.localScale = currentLocalScale;
			_eventUnitProxy.transform.TweenLocalXPosition(-280f, tweenTime);
			yield return new WaitForSeconds(tweenTime * 0.6f);
		}
	}

	private IEnumerator SetupLogoAndBackground()
	{
		if ((bool)_eventLogoController)
		{
			initialLogoPosition = _eventLogoController.gameObject.transform.localPosition;
			_eventLogoController.gameObject.transform.SetLocalYPosition(600f);
			yield return StartCoroutine(_eventLogoController.LoadLogoCoroutine(currentEventDataModel));
		}
		if ((bool)_eventBackgroundController)
		{
			yield return StartCoroutine(_eventBackgroundController.LoadBackgroundCoroutine(currentEventDataModel));
		}
	}

	private EventDataModel FetchEventDataModel()
	{
		EventDataModel eventDataModel = null;
		if (UserProfile.player != null)
		{
			eventDataModel = UserProfile.player.GetActiveEvent();
			if (eventDataModel == null)
			{
				eventDataModel = UserProfile.player.GetActiveOnCooldownEvent();
				if (eventDataModel == null)
				{
					eventDataModel = UserProfile.player.GetNextEvent();
				}
			}
		}
		return eventDataModel;
	}

	public void ConfigureEventState()
	{
		if (currentEventDataModel == null)
		{
			currentEventState = EventBlueprintsState.NO_EVENT;
		}
		else if (currentEventDataModel.IsActive)
		{
			currentEventState = EventBlueprintsState.EVENT_ACTIVE;
		}
		else if (currentEventDataModel.IsOnCooldown)
		{
			currentEventState = EventBlueprintsState.EVENT_COOLDOWN;
		}
		else
		{
			currentEventState = EventBlueprintsState.NEXT_EVENT;
		}
	}

	private IEnumerator ShootCoroutine()
	{
		while (true)
		{
			if (_eventUnitProxy.Prefab != null && _weaponController1 != null && _weaponController1.InitReady)
			{
				StartCoroutine(_weaponController1.FiringAnimation(0, _eventUnitProxy.Prefab.transform, true));
			}
			if (_eventUnitProxy2.Prefab != null && _weaponController2 != null && _weaponController2.InitReady)
			{
				StartCoroutine(_weaponController2.FiringAnimation(0, _eventUnitProxy2.Prefab.transform));
			}
			yield return new WaitForSeconds(2f);
		}
	}

	private IEnumerator SetupUnitAndArmys()
	{
		while (!_eventUnitProxy.AssetReady)
		{
			yield return 0;
		}
		if ((bool)_eventUnitProxy2)
		{
			yield return StartCoroutine(_eventUnitProxy2.ChangeAssetCoroutine(currentEventDataModel.EventUnitAssetBundleId));
			while (!_eventUnitProxy2.AssetReady)
			{
				yield return 0;
			}
		}
		if (currentEventDataModel.EventUnitLevel != null)
		{
			int unityRarity = currentEventDataModel.EventUnitLevel.Rarity;
			_weaponController1 = _eventUnitProxy.GetComponent<BasicWeaponController>();
			if (_weaponController1 != null && (bool)_eventUnitProxy.Prefab)
			{
				tk2dSpriteDefinition spriteDef = _eventUnitProxy.Prefab.GetComponent<tk2dSprite>().CurrentSprite;
				_weaponController1.Init(unityRarity, spriteDef);
			}
			_weaponController2 = _eventUnitProxy2.GetComponent<BasicWeaponController>();
			if (_weaponController2 != null && (bool)_eventUnitProxy2.Prefab)
			{
				tk2dSpriteDefinition spriteDef2 = _eventUnitProxy2.Prefab.GetComponent<tk2dSprite>().CurrentSprite;
				_weaponController2.Init(unityRarity, spriteDef2);
			}
		}
	}
}
