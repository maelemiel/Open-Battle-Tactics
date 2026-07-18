using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventInfoPopUpController : PopupController
{
	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private EventInfoUnitsViewController eventUnitsInfoViewController;

	[SerializeField]
	private EventInfoUnitsViewController eventBuildableUnitsInfoViewController;

	[SerializeField]
	private GameObject noUnitsGameObject;

	[SerializeField]
	private GameObject noBuildableUnitsGameObject;

	[SerializeField]
	private GameObject popupContentGameObject;

	[SerializeField]
	private GameObject leaderBoardsBannerGameObject;

	[SerializeField]
	private tk2dTextMesh labelH1;

	[SerializeField]
	private tk2dTextMesh labelH2;

	[SerializeField]
	private tk2dTextMesh bodyMessageLabel1;

	[SerializeField]
	private tk2dTextMesh bodyMessageLabel2;

	[SerializeField]
	private tk2dTextMesh bodyMessageLabel3;

	[SerializeField]
	private tk2dTextMesh bodyMessageLabel4;

	[SerializeField]
	private tk2dTextMesh bodyMessageLabel5;

	[SerializeField]
	private tk2dUIScrollableArea popupScrollableArea;

	[SerializeField]
	private GameObject videoPlayButton;

	public float unitSetViewControllerHeight = 350f;

	protected override void Start()
	{
		base.Start();
		EventDataModel eventDataModel = (EventDataModel)model.payload;
		if (eventDataModel == null)
		{
			OnCloseButton();
			return;
		}
		StartCoroutine(ToggleButton(Constants.GetEventIntroMovieURL));
		if ((bool)eventLogoController)
		{
			StartCoroutine(eventLogoController.LoadLogoCoroutine(eventDataModel));
		}
		if ((bool)labelH1)
		{
			labelH1.text = eventDataModel.PopupEventInfoH1;
		}
		if ((bool)labelH2)
		{
			labelH2.text = eventDataModel.PopupEventInfoH2;
		}
		if ((bool)bodyMessageLabel1)
		{
			bodyMessageLabel1.text = eventDataModel.PopupEventBodyMessage1;
		}
		if ((bool)bodyMessageLabel2)
		{
			bodyMessageLabel2.text = eventDataModel.PopupEventBodyMessage2;
		}
		if ((bool)bodyMessageLabel3)
		{
			bodyMessageLabel3.text = eventDataModel.PopupEventBodyMessage3;
		}
		if ((bool)bodyMessageLabel4)
		{
			bodyMessageLabel4.text = eventDataModel.PopupEventBodyMessage4;
		}
		if ((bool)bodyMessageLabel5)
		{
			bodyMessageLabel5.text = eventDataModel.PopupEventBodyMessage5;
		}
		if (!eventUnitsInfoViewController)
		{
			return;
		}
		List<EventUnitsDataModel> list = new List<EventUnitsDataModel>(eventDataModel.GetEventUnits());
		list.RemoveAll((EventUnitsDataModel x) => x.CanBuild);
		List<EventUnitsDataModel> list2 = new List<EventUnitsDataModel>(eventDataModel.GetEventUnits());
		list2.RemoveAll((EventUnitsDataModel x) => !x.CanBuild);
		bool flag = SetupUnitsView(eventUnitsInfoViewController, list, eventDataModel.PopupEventInfoUnitSetTitle);
		bool flag2 = SetupUnitsView(eventBuildableUnitsInfoViewController, list2, string.Empty);
		if (!flag)
		{
			eventUnitsInfoViewController.gameObject.SetActive(false);
			if ((bool)noUnitsGameObject && (bool)popupContentGameObject)
			{
				popupContentGameObject.transform.localPosition = noUnitsGameObject.transform.localPosition;
			}
			popupScrollableArea.ContentLength -= unitSetViewControllerHeight;
		}
		if (!flag2)
		{
			eventBuildableUnitsInfoViewController.gameObject.SetActive(false);
			if ((bool)noBuildableUnitsGameObject && (bool)leaderBoardsBannerGameObject)
			{
				leaderBoardsBannerGameObject.transform.localPosition = noBuildableUnitsGameObject.transform.localPosition;
			}
			popupScrollableArea.ContentLength -= unitSetViewControllerHeight;
		}
	}

	private bool SetupUnitsView(EventInfoUnitsViewController unitsView, List<EventUnitsDataModel> eventUnits, string title)
	{
		if (!unitsView)
		{
			return false;
		}
		if (eventUnits == null || eventUnits.Count == 0)
		{
			unitsView.gameObject.SetActive(false);
			return false;
		}
		unitsView.ConfigureView(eventUnits, title);
		return true;
	}

	private IEnumerator ToggleButton(string videoURL)
	{
		WWW www = new WWW(videoURL);
		yield return www;
		if (www != null && www.isDone && www.error == null)
		{
			videoPlayButton.SetActive(true);
		}
		else if (www.error != null)
		{
			videoPlayButton.SetActive(false);
		}
	}

	private IEnumerator PlayVideo(string eventID)
	{
		SceneController.resumeCallbackEnable = false;
		string videoFileName = Path.Combine(Application.persistentDataPath, eventID + "intro.mp4");
		if (File.Exists(videoFileName))
		{
			Handheld.PlayFullScreenMovie(videoFileName, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
			Reporting.EventIntroDisplay(eventID);
		}
		else
		{
			yield return StartCoroutine(DownloadVideo(Constants.GetEventIntroMovieURL, videoFileName));
			if (File.Exists(videoFileName))
			{
				Handheld.PlayFullScreenMovie(videoFileName, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
				Reporting.EventIntroDisplay(eventID);
			}
			else
			{
				Log.Debug("##### The file doesn't exist after download attempt....");
			}
		}
		SceneController.resumeCallbackEnable = true;
		yield return null;
	}

	private IEnumerator DownloadVideo(string videoURL, string videoName)
	{
		WWW www = new WWW(videoURL);
		yield return www;
		if (www != null && www.isDone && www.error == null)
		{
			FileStream stream = new FileStream(videoName, FileMode.OpenOrCreate);
			stream.Write(www.bytes, 0, www.bytes.Length);
			stream.Close();
		}
		else if (www.error != null)
		{
			Log.Error("Downloading the intro video had an error: " + www.error);
		}
		yield return null;
	}

	public void PlayVideoButton()
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (activeEvent != null)
		{
			StartCoroutine(PlayVideo(activeEvent.id));
		}
	}
}
