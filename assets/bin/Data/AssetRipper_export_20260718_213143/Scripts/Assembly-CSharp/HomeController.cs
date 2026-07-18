using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Spine;
using UnityEngine;

public class HomeController : SceneController
{
	[SerializeField]
	private HomeHUDController _hudController;

	[SerializeField]
	private Announcer _announcer;

	[SerializeField]
	private ProTipsViewController _tipsViewController;

	[SerializeField]
	private bool _autoEnableNews;

	[SerializeField]
	private Transform _newsParent;

	[SerializeField]
	private GameObject _newsPositionCenter;

	[SerializeField]
	private GameObject _newsPositionLeft;

	[SerializeField]
	private GameObject _newsPositionRight;

	[SerializeField]
	private ObjectShaker _tvShaker;

	[SerializeField]
	private GameObject _textGO;

	[SerializeField]
	private tk2dTextMesh _nextAdText;

	[SerializeField]
	private tk2dTextMesh _tapForInfoText;

	[SerializeField]
	private Transform _basicNewsPrefab;

	[SerializeField]
	private Transform _eventNewsPrefab;

	[SerializeField]
	private Transform _latestUpdatePrefab;

	[SerializeField]
	private Transform _newTankPrefab;

	[SerializeField]
	private Transform _salesPrefab;

	[SerializeField]
	private Transform _introducingPrefab;

	[SerializeField]
	private Transform _stepUpGachaPrefab;

	[SerializeField]
	private Transform _raidBossPrefab;

	[SerializeField]
	private Transform _joinClubPrefab;

	[SerializeField]
	private Transform _clubCratesPrefab;

	[SerializeField]
	private Transform _gemSalesPrefab;

	[SerializeField]
	private Transform _mixSalesPrefab;

	[SerializeField]
	private Transform _marketingPrefab;

	[SerializeField]
	private Transform _announcerFinalPosition;

	private Dictionary<GameObject, NewsDataModel> _newsToDisplay;

	private int _currentNewInShow;

	private NewsController _newsController1;

	private NewsController _newsController2;

	private Bone rootBone;

	private UserProfile userProfile;

	private static bool gameLoadedReportSent;

	public override void Awake()
	{
		if (UserProfile.player.tutorial.CurrentStep == TutorialStep.Complete)
		{
			OtherLevelsHelper.UsePlacement("app_start", "App Open", false);
		}
		base.Awake();
		_textGO.transform.SetLocalYPosition(-35f);
		_newsToDisplay = new Dictionary<GameObject, NewsDataModel>();
	}

	private void Start()
	{
		allowsBackButton = true;
		if (!gameLoadedReportSent)
		{
			Reporting.GameLoadedEvent();
			gameLoadedReportSent = true;
		}
		AudioTrigger.MenuBackground_Music.PlayMusic();
		if ((bool)_tipsViewController)
		{
			_tipsViewController.gameObject.SetActive(_autoEnableNews);
			if (_autoEnableNews)
			{
				_tipsViewController.Init(LinesType.NEWS);
			}
		}
		SceneTransitionManager.readyToTransitionIn = true;
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowButtons = true;
		}
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	public override void OnBackButton()
	{
		if (allowsBackButton && !SceneTransitionManager.transitionActive && PopupManager.PopupCount == 0)
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_exit_app_title".Localize("Exit"), "ui_exit_app_description".Localize("Do you want to exit the game?"), delegate
			{
				Application.Quit();
				Debug.LogWarning("Closing the game...!");
			}));
		}
	}

	public void Init()
	{
		LoadingPopupManager.HideSceneTransitionPopUp();
		userProfile = UserProfile.player;
		if (userProfile == null)
		{
			Log.Error("User profile not found");
		}
		if (!_hudController)
		{
			Log.Error("Hud controller not found");
		}
		_hudController.Init(this);
		if (UserProfile.player.tutorial.CurrentStep >= TutorialStep.BuildFirstTank)
		{
			StartCoroutine(ActivateTopBarWithDelay(1f));
		}
		if (UserProfile.player.userClub == null && UserProfile.player != null)
		{
			if (!string.IsNullOrEmpty(UserProfile.player.clubID))
			{
				Singleton<SessionManager>.instance.GetClub(delegate(UserClub fetchedUserClub)
				{
					if (fetchedUserClub != null)
					{
						UserProfile.player.userClub = fetchedUserClub;
					}
				});
			}
			else
			{
				Singleton<SessionManager>.instance.GetSoloEventPoints();
			}
		}
		InitNews();
		string getEventIntroMovieURL = Constants.GetEventIntroMovieURL;
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (activeEvent != null)
		{
			string id = activeEvent.id;
			string videoFileName = Path.Combine(Application.persistentDataPath, id + "intro.mp4");
			StartCoroutine(DownloadVideo(getEventIntroMovieURL, videoFileName));
		}
	}

	private void InitNews()
	{
		bool flag = false;
		if (Singleton<UserProfileManager>.instance.IsFirstTimeInScene(sceneModel._scene))
		{
			flag = true;
		}
		if (AppConfig.IsNewVersion)
		{
			foreach (string key in CacheManager.newsCache.Keys)
			{
				if (CacheManager.newsCache[key].showNumber != 0 && CacheManager.newsCache[key].Enabled && UserProfileManager.Kvs.ContainsKey("new_count_" + key))
				{
					UserProfileManager.Kvs.SetValue("new_count_" + key, 0);
				}
			}
		}
		EventDataModel eventDataModel = FetchEventDataModel();
		NewsDataModel newsDataModel = null;
		foreach (NewsDataModel value in CacheManager.newsCache.Values)
		{
			if (!value.Enabled || (value.showNumber != 0 && UserProfileManager.Kvs.ContainsKey("new_count_" + value.id) && UserProfileManager.Kvs.GetValue<int>("new_count_" + value.id) >= value.showNumber))
			{
				continue;
			}
			NewsTypes newsTypes = (NewsTypes)(int)Enum.Parse(typeof(NewsTypes), value.id, true);
			if (newsTypes == NewsTypes.BasicNew)
			{
				newsDataModel = value;
			}
			if (flag)
			{
				if (newsDataModel == null)
				{
					continue;
				}
				_newsToDisplay.Add(_basicNewsPrefab.gameObject, newsDataModel);
				break;
			}
			switch (newsTypes)
			{
			case NewsTypes.EventNew:
				if (eventDataModel != null)
				{
					_newsToDisplay.Add(_eventNewsPrefab.gameObject, value);
				}
				break;
			case NewsTypes.LatestUpdate:
				_newsToDisplay.Add(_latestUpdatePrefab.gameObject, value);
				break;
			case NewsTypes.Sales:
				if (SalesController.CanShow(value))
				{
					_newsToDisplay.Add(_salesPrefab.gameObject, value);
				}
				break;
			case NewsTypes.RaidBoss:
				if (RaidBossController.CanShow(value))
				{
					_newsToDisplay.Add(_raidBossPrefab.gameObject, value);
				}
				break;
			case NewsTypes.NewTank:
				if (NewTankController.CanShow(value))
				{
					_newsToDisplay.Add(_newTankPrefab.gameObject, value);
				}
				break;
			case NewsTypes.IntroducingTank:
				if (IntroducingController.CanShow(value))
				{
					_newsToDisplay.Add(_introducingPrefab.gameObject, value);
				}
				break;
			case NewsTypes.StepUpGacha:
				if (StepUpGachaController.CanShow(value))
				{
					_newsToDisplay.Add(_stepUpGachaPrefab.gameObject, value);
				}
				break;
			case NewsTypes.JoinClub:
				if (JoinClubController.CanShow(value))
				{
					_newsToDisplay.Add(_joinClubPrefab.gameObject, value);
				}
				break;
			case NewsTypes.ClubCrates:
				if (ClubCratesController.CanShow(value))
				{
					_newsToDisplay.Add(_clubCratesPrefab.gameObject, value);
				}
				break;
			case NewsTypes.GemSales:
				_newsToDisplay.Add(_gemSalesPrefab.gameObject, value);
				break;
			case NewsTypes.MixSales:
				if (MixSalesController.CanShow(value))
				{
					_newsToDisplay.Add(_mixSalesPrefab.gameObject, value);
				}
				break;
			case NewsTypes.Marketing:
				if (MarketingController.CanShow(value))
				{
					_newsToDisplay.Add(_marketingPrefab.gameObject, value);
				}
				break;
			}
		}
		if (_newsToDisplay.Count == 0)
		{
			_newsToDisplay.Add(_basicNewsPrefab.gameObject, newsDataModel);
		}
		SetupNewsToDisplay();
	}

	private void SetupNewsToDisplay(bool firstTime = true)
	{
		List<GameObject> list = new List<GameObject>(_newsToDisplay.Keys);
		GameObject gameObject = list[_currentNewInShow];
		if (firstTime)
		{
			_newsController1 = InstantiateNew(gameObject, _newsPositionCenter.transform.localPosition, _newsToDisplay[gameObject]);
		}
		else
		{
			Log.DebugTag("swaping news controller 1 by news controller 2", null, "Default");
			UnityEngine.Object.Destroy(_newsController1.gameObject);
			_newsController1 = _newsController2;
		}
		if (_currentNewInShow == list.Count - 1)
		{
			_currentNewInShow = 0;
		}
		else
		{
			_currentNewInShow++;
		}
		if (firstTime)
		{
			StartCoroutine(_newsController1.AfterMovingInAction());
		}
		if (list.Count > 1)
		{
			gameObject = list[_currentNewInShow];
			_newsController2 = InstantiateNew(gameObject, _newsPositionRight.transform.localPosition, _newsToDisplay[gameObject]);
			StartCoroutine(NewsScrollCoroutine());
		}
		else
		{
			StartCoroutine(ShowAnnouncer(_newsController1.AnnouncerType));
		}
	}

	private NewsController InstantiateNew(GameObject objectToInst, Vector3 position, NewsDataModel newsDM)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(objectToInst) as GameObject;
		if (gameObject != null)
		{
			gameObject.transform.transform.parent = _newsParent;
			gameObject.transform.localPosition = position;
			NewsController component = gameObject.GetComponent<NewsController>();
			if (component != null)
			{
				component.TvShaker = TvShaker;
				StartCoroutine(component.Init(newsDM));
			}
			return component;
		}
		return null;
	}

	private IEnumerator NewsScrollCoroutine()
	{
		_newsController1.ShowTimes++;
		StartCoroutine(ShowAnnouncer(_newsController1.AnnouncerType));
		_nextAdText.text = string.Format("ui_home_nextAd".Localize("NEXT AD IN {0}"), Constants.NewsRotationTime);
		_textGO.transform.TweenLocalYPosition(0f, 0.3f);
		yield return new WaitForSeconds(0.3f);
		for (int i = Constants.NewsRotationTime; i > 0; i--)
		{
			_nextAdText.text = string.Format("ui_home_nextAd".Localize("NEXT AD IN {0}"), i.ToString());
			yield return new WaitForSeconds(1f);
		}
		_nextAdText.text = string.Format("ui_home_nextAd".Localize("NEXT AD IN {0}"), 0);
		_textGO.transform.TweenLocalYPosition(-35f, 0.3f);
		yield return new WaitForSeconds(0.3f);
		_newsController1.swaping = true;
		_newsController1.BeforeMovingOutAction();
		_newsController1.transform.TweenLocalXPosition(_newsPositionLeft.transform.localPosition.x, 0.8f);
		_newsController2.BeforeMovingInAction();
		_newsController2.transform.TweenLocalXPosition(0f, 0.8f);
		yield return new WaitForSeconds(0.8f);
		yield return StartCoroutine(_newsController2.AfterMovingInAction());
		SetupNewsToDisplay(false);
	}

	private IEnumerator ActivateTopBarWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		base.ShowTopBar = true;
	}

	private IEnumerator ShowAnnouncer(AnnouncerType announcerType)
	{
		if (announcerType != AnnouncerType.NONE)
		{
			Vector3 initialPosition = _announcer.transform.localPosition;
			_announcer.Move(announcerType, initialPosition, new Vector3(_announcerFinalPosition.localPosition.x, _announcerFinalPosition.localPosition.y, _announcerFinalPosition.localPosition.z), Vector3.one, (float)Constants.NewsRotationTime - 1.5f, 20);
		}
		yield break;
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

	private void OnTapToInfo()
	{
		if (_newsController1 != null)
		{
			if (!_newsController1.swaping)
			{
				_newsController1.TvButtonPress();
			}
			else if (_newsController2 != null)
			{
				_newsController2.TvButtonPress();
			}
		}
	}

	private void ArenaButton()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
	}

	private void TvShaker()
	{
		if ((bool)_tvShaker)
		{
			_tvShaker.Shake();
		}
	}
}
