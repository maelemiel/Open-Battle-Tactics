using System;
using System.Collections;
using System.Collections.Generic;
using SponsorPay;

public class SponsorPayManager : Singleton<SponsorPayManager>
{
	public enum BrandEngageResult
	{
		Success = 0,
		NoVideoAvailable = 1,
		Error = 2,
		Aborted = 3
	}

	public enum BrandEngageStatus
	{
		ReadyToRequest = 0,
		WaitingForRequestResult = 1,
		ReadyToPLay = 2,
		RequestFailed = 3,
		OnPlay = 4
	}

	private SponsorPayPlugin sponsorPlugin;

	private string appId;

	private string securityToken;

	private string userId;

	private string credentialsToken;

	private BrandEngageStatus _currentBrandEngageStatus;

	private BrandEngageResult _currentBrandEngageResult;

	private Action<BrandEngageResult> _showBrandCallback;

	public BrandEngageStatus CurrentBrandEngageStatus
	{
		get
		{
			return _currentBrandEngageStatus;
		}
	}

	public BrandEngageResult CurrentBrandEngageResult
	{
		get
		{
			return _currentBrandEngageResult;
		}
	}

	private void Awake()
	{
		_currentBrandEngageStatus = BrandEngageStatus.ReadyToRequest;
		appId = string.Empty;
		securityToken = string.Empty;
		userId = "_editor";
		if (AppConfig.currentEnvironmentType == AppConfig.EnvironmentType.Production)
		{
			appId = "24712";
			securityToken = "863f336b88a430081f3eaeafc5d09b58";
			userId = "_android";
		}
		else
		{
			appId = "25952";
			securityToken = "76d63caef58ad2433ab76ef669dcc186";
			userId = "_android";
		}
	}

	public IEnumerator InitCoroutine()
	{
		while (UserProfile.player == null)
		{
			yield return 0;
		}
		Init();
	}

	public void Init()
	{
		sponsorPlugin = SponsorPayPluginMonoBehaviour.PluginInstance;
		sponsorPlugin.EnableLogging(AppConfig.currentEnvironmentType != AppConfig.EnvironmentType.Production);
		sponsorPlugin.OnDeltaOfCoinsReceived += OnDeltaOfCoinsReceived;
		sponsorPlugin.OnDeltaOfCoinsRequestFailed += OnDeltaOfCoinsRequestFailed;
		sponsorPlugin.OnBrandEngageRequestResponseReceived += OnBrandEngageRequestResponseReceived;
		sponsorPlugin.OnBrandEngageRequestErrorReceived += OnBrandEngageRequestErrorReceived;
		sponsorPlugin.OnNativeExceptionReceived += OnNativeExceptionReceived;
		sponsorPlugin.OnOfferWallResultReceived += OnOfferWallResultReceived;
		sponsorPlugin.OnBrandEngageResultReceived += OnBrandEngageResultReceived;
		sponsorPlugin.OnInterstitialRequestResponseReceived += OnInterstitialRequestResponseReceived;
		sponsorPlugin.OnInterstitialRequestErrorReceived += OnInterstitialRequestErrorReceived;
		sponsorPlugin.OnInterstitialStatusCloseReceived += OnInterstitialStatusCloseReceived;
		sponsorPlugin.OnInterstitialStatusErrorReceived += OnInterstitialStatusErrorReceived;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("additionalParam", "WOOT");
		dictionary.Add("anotherOne", "ImHERE");
		sponsorPlugin.AddParameters(dictionary);
		credentialsToken = sponsorPlugin.Start(appId, UserProfile.player.id + userId, securityToken);
	}

	public void RunOfferWall()
	{
		sponsorPlugin.LaunchOfferWall(string.Empty, null);
	}

	public void RequestBrandEngage(Action<BrandEngageResult> cb)
	{
		Log.Debug("SponsorPayManager. RequestBrandEngage called.");
		if (UserProfile.player == null)
		{
			return;
		}
		if (TimeUtility.IsToday(UserProfile.player.videoAdsLastShow) && UserProfile.player.videoAdsCount >= Constants.MaxAdsVideoPerDay && AppConfig.currentEnvironmentType == AppConfig.EnvironmentType.Production)
		{
			PopupManager.ShowPopup(PopupDataModel.Ok(LocalizationManager.GetString("ui_fast_forward_maxed_title", "Fast Forwards Maxed"), LocalizationManager.GetString("ui_fast_forward_maxed_description", "You've used the maximum amount of Fast Forwards for the day. Try again tomorrow!")));
			return;
		}
		_showBrandCallback = cb;
		if (_currentBrandEngageStatus != BrandEngageStatus.ReadyToRequest)
		{
			Log.Warning("SponsorPayManager. Imposible to request a brand engage becouse the current state is {0}", _currentBrandEngageStatus);
			PopupManager.ShowPopup(PopupDataModel.Ok(LocalizationManager.GetString("ui_fast_forward_loading", "Loading advertisement"), LocalizationManager.GetString("ui_fast_forward_no_video_available", "No videos available at this time")));
		}
		else
		{
			_currentBrandEngageStatus = BrandEngageStatus.WaitingForRequestResult;
			PopupDataModel popupDataModel = new PopupDataModel();
			popupDataModel.closeButtonAction = ShowBrandCallback;
			PopupManager.ShowPopup(popupDataModel, SceneTransitionManager.Scene.FastForwardWaitingPopUp);
			sponsorPlugin.RequestBrandEngageOffers(credentialsToken, "Gems", false);
		}
	}

	public void StartBrandEngage()
	{
		Log.Debug("SponsorPayManager. StartBrandEngage called.");
		if (_currentBrandEngageStatus == BrandEngageStatus.RequestFailed)
		{
			_showBrandCallback = null;
			_currentBrandEngageStatus = BrandEngageStatus.ReadyToRequest;
		}
		else if (_currentBrandEngageStatus == BrandEngageStatus.ReadyToPLay)
		{
			_currentBrandEngageStatus = BrandEngageStatus.OnPlay;
			Reporting.VideoAdDisplay(appId);
			sponsorPlugin.StartBrandEngage();
		}
	}

	public void OnDeltaOfCoinsReceived(double deltaOfCoins, string transactionId)
	{
		Log.Debug("SponsorPayManager. OnDeltaOfCoinsReceived called. deltaOfCoins: {0} - transactionId: {1}.", deltaOfCoins.ToString(), transactionId);
	}

	public void OnDeltaOfCoinsRequestFailed(RequestError error)
	{
		Log.Debug("SponsorPayManager. OnDeltaOfCoinsRequestFailed called. error type: {0} - error message: {1}.", error.Type, error.Message);
	}

	public void OnOfferWallResultReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnOfferWallResultReceived called. message: {0}.", message);
	}

	public void OnBrandEngageRequestResponseReceived(bool offersAvailable)
	{
		Log.Debug("SponsorPayManager. OnBrandEngageRequestResponseReceived called. offersAvailable: {0}.", offersAvailable);
		if (offersAvailable)
		{
			_currentBrandEngageStatus = BrandEngageStatus.ReadyToPLay;
			return;
		}
		_currentBrandEngageResult = BrandEngageResult.NoVideoAvailable;
		_currentBrandEngageStatus = BrandEngageStatus.RequestFailed;
	}

	public void OnBrandEngageRequestErrorReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnBrandEngageRequestErrorReceived called. message: {0}.", message);
	}

	public void OnBrandEngageResultReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnBrandEngageResultReceived called. message: {0}.", message);
		_currentBrandEngageStatus = BrandEngageStatus.ReadyToRequest;
		switch (message)
		{
		case "STARTED":
			_currentBrandEngageResult = BrandEngageResult.NoVideoAvailable;
			break;
		case "CLOSE_FINISHED":
			_currentBrandEngageResult = BrandEngageResult.Success;
			break;
		case "CLOSE_ABORTED":
			_currentBrandEngageResult = BrandEngageResult.Aborted;
			break;
		case "ERROR":
			_currentBrandEngageResult = BrandEngageResult.Error;
			break;
		}
		Reporting.VideoAdComplete(appId, _currentBrandEngageResult);
	}

	public void OnInterstitialRequestResponseReceived(bool offersAvailable)
	{
		Log.Debug("SponsorPayManager. OnInterstitialRequestResponseReceived called. offersAvailable: {0}.", offersAvailable);
	}

	public void OnInterstitialRequestErrorReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnInterstitialRequestErrorReceived called. message: {0}.", message);
	}

	public void OnInterstitialStatusCloseReceived(string closeReason)
	{
		Log.Debug("SponsorPayManager. OnInterstitialStatusCloseReceived called. closeReason: {0}.", closeReason);
	}

	public void OnInterstitialStatusErrorReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnInterstitialStatusErrorReceived called. message: {0}.", message);
	}

	public void OnNativeExceptionReceived(string message)
	{
		Log.Debug("SponsorPayManager. OnNativeExceptionReceived called. message: {0}.", message);
	}

	private void ShowBrandCallback()
	{
		if (_showBrandCallback != null)
		{
			_showBrandCallback(_currentBrandEngageResult);
		}
	}
}
