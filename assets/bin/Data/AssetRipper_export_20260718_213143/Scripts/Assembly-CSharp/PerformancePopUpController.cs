using UnityEngine;

public class PerformancePopUpController : PopupController
{
	public const string KEY_PKVS_BATTERY_SAVER = "batterySaverOn";

	public const string KEY_PKVS_KAMCORD = "kamcordOn";

	[SerializeField]
	private tk2dUIToggleButton batterSaverToggleButton;

	[SerializeField]
	private tk2dUIToggleButton kamcordToggleButton;

	[SerializeField]
	private tk2dUIToggleButton fireworksToggleButton;

	protected override void Start()
	{
		base.Start();
		batterSaverToggleButton.IsOn = UserProfile.player.preferences.BatterySaverOn;
		kamcordToggleButton.IsOn = UserProfile.player.preferences.KamcordOn;
		fireworksToggleButton.IsOn = SceneController.fireWorksEnable;
		if (!Constants.EnableFireWorks)
		{
			fireworksToggleButton.gameObject.SetActive(false);
		}
		kamcordToggleButton.gameObject.SetActive(Kamcord.IsEnabled());
	}

	public void OnTapBatterySaver(tk2dUIToggleButton toggleControl)
	{
		bool isOn = toggleControl.IsOn;
		if (isOn)
		{
			Application.targetFrameRate = 30;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
		UserProfile.player.preferences.BatterySaverOn = isOn;
	}

	public void OnTapKamcord(tk2dUIToggleButton toggleControl)
	{
		bool isOn = toggleControl.IsOn;
		UserProfile.player.preferences.KamcordOn = isOn;
		if (isOn)
		{
			Reporting.KamcordShareEnable();
		}
		else
		{
			Reporting.KamcordShareDisable();
		}
	}

	public void OnTapFireWorks(tk2dUIToggleButton toggleControl)
	{
		bool isOn = toggleControl.IsOn;
		SceneController.fireWorksEnable = isOn;
	}
}
