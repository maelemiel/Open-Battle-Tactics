using UnityEngine;

public class ClubDataViewController : MonoBehaviour
{
	[SerializeField]
	private tk2dUITextInput clubNameLabel;

	[SerializeField]
	private tk2dUITextInput clubDescriptionLabel;

	[SerializeField]
	private tk2dUITextInput clubPasswordLabel;

	[SerializeField]
	private TierBadgeSelectionList tierSelection;

	[SerializeField]
	private TeamTypeSelectionList teamTypeSelection;

	[SerializeField]
	private TeamBadgeSelectionList teamBadgeSelection;

	public void Reset()
	{
		clubNameLabel.Text = string.Empty;
		clubDescriptionLabel.Text = string.Empty;
		clubPasswordLabel.Text = string.Empty;
		tierSelection.SetSelectedItem(0);
		teamTypeSelection.SetSelectedItem(0);
		teamBadgeSelection.SetSelectedItem(0);
	}

	public void SetClubData(UserClub club)
	{
		if (club == null)
		{
			Reset();
			return;
		}
		clubNameLabel.Text = club.name;
		clubNameLabel.SetFocus();
		clubDescriptionLabel.Text = club.description;
		clubPasswordLabel.Text = club.password;
		teamTypeSelection.SetSelectedItem((int)club.teamType);
		int itemIndex = tierSelection.GetItemIndex(club.MinTierAssetLinkage);
		if (itemIndex != -1)
		{
			tierSelection.SetSelectedItem(itemIndex);
		}
		itemIndex = teamBadgeSelection.GetItemIndex(club.TeamBadgeAssetLinkage);
		if (itemIndex != -1)
		{
			teamBadgeSelection.SetSelectedItem(itemIndex);
		}
	}

	public UserClub GetClubData()
	{
		string text = clubNameLabel.Text;
		string text2 = clubDescriptionLabel.Text;
		string text3 = clubPasswordLabel.Text;
		ProgressionDivisionDataModel selectedDivision = tierSelection.GetSelectedDivision();
		ClubTypes selectedItem = teamTypeSelection.GetSelectedItem();
		string id = teamBadgeSelection.GetSelectedItem().id;
		UserClub userClub = new UserClub(text, text2, text3, int.Parse(selectedDivision.id), selectedItem, id, string.Empty);
		if (ValidateInputs(userClub))
		{
			return userClub;
		}
		return null;
	}

	public void ValidatePassword()
	{
		clubPasswordLabel.Text = clubPasswordLabel.Text.Replace(" ", string.Empty);
	}

	private bool ValidateInputs(UserClub clubData)
	{
		if (string.IsNullOrEmpty(clubData.name.Trim()))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "ui_clubs_error_club_name_empty".Localize("Club name field cannot be empty")));
			return false;
		}
		if (string.IsNullOrEmpty(clubData.description.Trim()))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "ui_clubs_error_club_desc_empty".Localize("Club description cannot be empty")));
			return false;
		}
		if (clubData.teamType == ClubTypes.PRIVATE && string.IsNullOrEmpty(clubData.password.Trim()))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "ui_clubs_error_club_password_empty".Localize("Private clubs must have a password")));
			return false;
		}
		if (clubData.teamType == ClubTypes.PUBLIC && !string.IsNullOrEmpty(clubData.password.Trim()))
		{
			clubData.password = string.Empty;
			return true;
		}
		return true;
	}
}
