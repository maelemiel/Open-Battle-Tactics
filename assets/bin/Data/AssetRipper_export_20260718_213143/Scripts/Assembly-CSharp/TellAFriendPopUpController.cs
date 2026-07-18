using UnityEngine;

public class TellAFriendPopUpController : PopupController
{
	[SerializeField]
	private LocalizeTextMesh[] confirmButtonLabelLocalizers;

	[SerializeField]
	private tk2dTextMesh[] confirmButtonLabels;

	private string channel;

	protected override void Start()
	{
		if (Etcetera.isSMSAvailable())
		{
			channel = "sms";
			for (int i = 0; i < confirmButtonLabelLocalizers.Length; i++)
			{
				confirmButtonLabelLocalizers[i].TextKey = "ui_tellafriend_button_sms";
			}
			for (int j = 0; j < confirmButtonLabels.Length; j++)
			{
				confirmButtonLabels[j].text = "ui_tellafriend_button_sms".Localize("MESSAGE A FRIEND");
			}
		}
		else
		{
			channel = "email";
		}
	}

	private void OnConfirm()
	{
		Reporting.TellAFriendEvent("send", channel);
		if (Etcetera.isSMSAvailable())
		{
			EtceteraAndroid.showSMSComposer("ui_tellafriend_sms_template_android".Localize("Check out the new game Super Battle Tactics! http://www.google.com"));
		}
		else if (Etcetera.isEmailAvailable())
		{
			EtceteraAndroid.showEmailComposer(string.Empty, "ui_tellafriend_email_subject_android".Localize("Awesome new game you should play!"), "ui_tellafriend_email_body_android".Localize("Check out the new game Super Battle Tactics! <a href=\"http://www.google.com\">Get it here!</a>"), true);
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_setup_email_title".Localize("Email account setup"), "ui_setup_email".Localize("Please first setup an Email account on the device.")));
		}
		Close();
	}

	public override void OnCloseButton()
	{
		Reporting.TellAFriendEvent("close", channel);
		base.OnCloseButton();
	}
}
