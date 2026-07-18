using UnityEngine;

public class ChatPopUpController : PopupController
{
	[SerializeField]
	private ChatWindowController chatWindowController;

	[SerializeField]
	private GameObject chatButton;

	protected override void Start()
	{
		base.Start();
		string userClubID = (string)model.payload;
		if ((bool)chatWindowController)
		{
			chatWindowController.Init(userClubID);
			chatWindowController.IsOpen = true;
		}
		if ((bool)chatButton && (bool)TopBarController.instance)
		{
			chatButton.transform.position = TopBarController.instance.ChatButton.transform.position;
		}
	}

	public override void OnCloseButton()
	{
		if ((bool)chatButton)
		{
			chatButton.SetActive(false);
		}
		if ((bool)chatWindowController)
		{
			chatWindowController.OnChatClosed += DestroyPopUp;
			chatWindowController.IsOpen = false;
		}
	}

	private void DestroyPopUp()
	{
		PopupManager.DestroyPopup(model);
	}

	private void OnCreateChatClick()
	{
		if (Constants.GetIntConstantWithID("menu_button_club") <= UserProfile.player.divisionInt)
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_chat_not_in_a_club_title".Localize("You don't have a club"), "ui_chat_not_in_a_club_description".Localize("You are not part of a club. Would you like to create one?"), GoToClubScene));
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Sorry", "Higher tier required to create or enter a Club"));
		}
	}

	private void GoToClubScene()
	{
		OnCloseButton();
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
	}
}
