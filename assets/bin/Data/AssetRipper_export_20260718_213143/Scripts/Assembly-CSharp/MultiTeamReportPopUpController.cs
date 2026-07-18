using UnityEngine;

public class MultiTeamReportPopUpController : PopupController
{
	private const float OVERLAY_ALPHA = 0.7f;

	[SerializeField]
	private UnitMultiTeamReportView[] reportUnitViews;

	[SerializeField]
	private MultiTeamReportViewController multiTeamReportView;

	[SerializeField]
	private MovingObjectController movingObjectcontroller;

	[SerializeField]
	private tk2dBaseSprite overlaySprite;

	protected override void Start()
	{
		base.Start();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	public void Init()
	{
		UserMultiTeamReport.MultiTeamReport multiTeamReport = (UserMultiTeamReport.MultiTeamReport)model.payload;
		if (multiTeamReport == null)
		{
			Log.Error("MultiTeamReport data model not found", base.gameObject);
			Close();
		}
		multiTeamReportView.Init(this, multiTeamReport);
		if ((bool)movingObjectcontroller)
		{
			movingObjectcontroller.IsOpen = true;
			movingObjectcontroller.OnOpened += StartReportSequence;
		}
		else
		{
			StartReportSequence();
		}
		if ((bool)overlaySprite)
		{
			overlaySprite.Alpha = 0f;
			overlaySprite.gameObject.SetActive(true);
			overlaySprite.TweenAlpha(0.7f, movingObjectcontroller.timeToOpen);
		}
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.MovingPlatform);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.CrateLand);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.CoinsEarned);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.DieFaceSpin);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.PlayerUnEquipItem);
	}

	private void StartReportSequence()
	{
		StartCoroutine(multiTeamReportView.StartSequence());
	}

	public override void OnCloseButton()
	{
		if ((bool)movingObjectcontroller)
		{
			movingObjectcontroller.OnClosed += DestroyPopUp;
			movingObjectcontroller.IsOpen = false;
			if ((bool)overlaySprite)
			{
				overlaySprite.gameObject.SetActive(true);
				overlaySprite.TweenAlpha(0f, movingObjectcontroller.timeToOpen);
			}
		}
		else
		{
			DestroyPopUp();
		}
	}

	private void DestroyPopUp()
	{
		PopupManager.DestroyPopup(model);
	}
}
