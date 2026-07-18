using System.Collections;
using LitJson0;
using UnityEngine;

public class PickYourNameController : PopupController
{
	[SerializeField]
	private tk2dUITextInput inputText;

	[SerializeField]
	private GameObject PickYourNameConfirm;

	[SerializeField]
	private GameObject PickYourNameInput;

	[SerializeField]
	private GameObject Btn_Check;

	[SerializeField]
	private GameObject Btn_CheckButtonUp;

	[SerializeField]
	private GameObject Btn_CheckButtonUnavailable;

	[SerializeField]
	private GameObject Btn_Exit;

	[SerializeField]
	private GameObject Btn_Accept;

	[SerializeField]
	private GameObject Btn_AcceptButtonUp;

	[SerializeField]
	private GameObject Btn_AcceptButtonUnavailable;

	[SerializeField]
	private tk2dTextMesh CurrentName;

	[SerializeField]
	private tk2dTextMesh AvailableText;

	[SerializeField]
	private tk2dTextMesh UnavailableText;

	[SerializeField]
	private tk2dTextMesh NewUserName;

	private ChangeUserNameController changeUserNameController;

	private string storedNamed;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(ShowTutorialSequence());
		EnableAcceptButton(false);
		initAvailableText();
		CurrentName.text = UserProfile.player.nickname;
		changeUserNameController = GetComponent<ChangeUserNameController>();
	}

	private void initAvailableText()
	{
		AvailableText.text = string.Empty;
		UnavailableText.text = string.Empty;
	}

	public void OnClickCheck()
	{
		string text = inputText.Text.Trim();
		if (!string.IsNullOrEmpty(text))
		{
			CheckUserName(text);
		}
	}

	public void OnClickAccept()
	{
		string text = inputText.Text.Trim();
		if (!string.IsNullOrEmpty(text))
		{
			storedNamed = text;
			EnableAcceptButton(false);
			SendUserName(text);
		}
	}

	private void CheckUserName(string userName)
	{
		Singleton<SessionManager>.instance.CheckUsername(userName, delegate(ServerUtilities.BaseResponse response)
		{
			ParseCheckResponse(response);
		});
	}

	private void SendUserName(string userName)
	{
		if (!string.IsNullOrEmpty(UserProfile.player.username))
		{
			UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(Constants.ChangeNamePriceId);
			PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(priceForID, "ui_name_change_title".Localize("Change your name?"), "ui_name_change_desc".Localize("Changing your name will cost the following"), PurchaseNewName, null, base.OnCloseButton));
		}
		else
		{
			PurchaseNewName();
		}
	}

	private void PurchaseNewName()
	{
		changeUserNameController.ChangeUserName(storedNamed, ParseAcceptResponse, null);
	}

	private void SetSuggestions()
	{
	}

	private void ParseAcceptResponse(ServerUtilities.BaseResponse response)
	{
		NewUserName.text = storedNamed;
		PickYourNameConfirm.SetActive(true);
		PickYourNameInput.SetActive(false);
		EnableAcceptButton(true);
	}

	private void ParseCheckResponse(ServerUtilities.BaseResponse response)
	{
		JsonObject jsonObject = response.json.GetObject("result");
		int num = jsonObject.GetInt("status");
		if (!(Btn_CheckButtonUp == null))
		{
			EnableCheckButton(false);
			EnableAcceptButton(true);
			switch (num)
			{
			case 0:
				AvailableText.text = string.Format("ui_pickyourname_available".Localize("{0} is Available"), inputText.Text.Trim());
				break;
			case 1:
			{
				string text2 = jsonObject.GetString("reason");
				AvailableText.text = text2;
				EnableCheckButton(true);
				EnableAcceptButton(false);
				break;
			}
			case 2:
			{
				UnavailableText.text = string.Format("ui_pickyourname_unavailable".Localize("{0} is Unavailable"), inputText.Text.Trim());
				string text = jsonObject.GetString("suggested");
				AvailableText.text = string.Format("ui_pickyourname_available".Localize("{0} is Available"), text);
				inputText.Text = text;
				break;
			}
			}
		}
	}

	public void OnChangeInputText()
	{
		EnableCheckButton(true);
		EnableAcceptButton(false);
		initAvailableText();
	}

	private void EnableCheckButton(bool enabled)
	{
		Btn_CheckButtonUp.SetActive(enabled);
		Btn_CheckButtonUnavailable.SetActive(!enabled);
		tk2dUIItem component = Btn_Check.GetComponent<tk2dUIItem>();
		component.enabled = enabled;
	}

	private void EnableAcceptButton(bool enabled)
	{
		Btn_AcceptButtonUp.SetActive(enabled);
		Btn_AcceptButtonUnavailable.SetActive(!enabled);
		tk2dUIItem component = Btn_Accept.GetComponent<tk2dUIItem>();
		component.enabled = enabled;
	}

	private IEnumerator ShowTutorialSequence()
	{
		if (UserProfile.player.tutorial.CurrentStep == TutorialStep.ChangeName)
		{
			allowBackButton = false;
			Btn_Exit.SetActive(false);
			yield return StartCoroutine(AnnouncerController.DialogTrigger("TutorialChangeName"));
		}
	}
}
