using UnityEngine;

public class MyClubView : MonoBehaviour
{
	private const string InviteSubjectText = "Join my club on Super Battle Tactics.";

	private const string InviteMessageText = "Come join my club on Super Battle Tactics!\nClub Name: {0}\nPassword: {1}\nYou can download the game here: http://bit.ly/battletactics";

	[SerializeField]
	private ScrollableAreaController playerScrollableAreaController;

	[SerializeField]
	private PrefabProxy teamBadgePrefabProxy;

	[SerializeField]
	private PrefabProxy minTierPrefabProxy;

	[SerializeField]
	private tk2dTextMesh clubNameLabel;

	[SerializeField]
	private tk2dTextMesh clubDescriptionLabel;

	[SerializeField]
	private tk2dTextMesh clubTypeLabel;

	[SerializeField]
	private tk2dTextMesh totalMembersLabel;

	[SerializeField]
	private tk2dTextMesh currentMembersLabel;

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private GameObject inviteButton;

	[SerializeField]
	private GameObject disableInviteButton;

	private UserClub clubData;

	public UserClub ClubData
	{
		get
		{
			return clubData;
		}
	}

	public ScrollableAreaController ScrollableAreaController
	{
		get
		{
			return playerScrollableAreaController;
		}
		set
		{
			playerScrollableAreaController = value;
		}
	}

	private void Awake()
	{
		if ((bool)inviteButton)
		{
			inviteButton.SetActive(false);
		}
		if ((bool)disableInviteButton)
		{
			disableInviteButton.SetActive(false);
		}
		if ((bool)playerScrollableAreaController)
		{
			playerScrollableAreaController.gameObject.SetActive(false);
		}
	}

	public void ConfigureView(UserClub clubData)
	{
		if (clubData == null)
		{
			return;
		}
		this.clubData = clubData;
		if ((bool)playerScrollableAreaController)
		{
			playerScrollableAreaController.gameObject.SetActive(true);
			playerScrollableAreaController.InitializeWithData(clubData.members);
		}
		if ((bool)clubNameLabel)
		{
			clubNameLabel.text = clubData.name;
		}
		if ((bool)clubDescriptionLabel)
		{
			clubDescriptionLabel.text = clubData.description;
		}
		if ((bool)clubTypeLabel)
		{
			clubTypeLabel.text = clubData.GetLocalizatedTeamType();
		}
		if ((bool)currentMembersLabel)
		{
			currentMembersLabel.text = clubData.members.Count.ToString();
		}
		if ((bool)totalMembersLabel)
		{
			totalMembersLabel.text = "/" + Constants.ClubsTotalMembers;
		}
		if ((bool)teamBadgePrefabProxy && clubData.TeamBadgeAssetLinkage != null)
		{
			teamBadgePrefabProxy.ChangeAsset(clubData.TeamBadgeAssetLinkage);
		}
		if ((bool)minTierPrefabProxy && clubData.MinTierAssetLinkage != null)
		{
			minTierPrefabProxy.ChangeAsset(clubData.MinTierAssetLinkage);
		}
		if ((bool)inviteButton)
		{
			inviteButton.SetActive(false);
		}
		if ((bool)disableInviteButton)
		{
			disableInviteButton.SetActive(false);
		}
		if (clubData.teamType == ClubTypes.PRIVATE)
		{
			if (!clubData.UserIsLeader(UserProfile.player.id))
			{
				return;
			}
			if (clubData.IsFull())
			{
				if ((bool)disableInviteButton)
				{
					disableInviteButton.SetActive(true);
				}
			}
			else if ((bool)inviteButton)
			{
				inviteButton.SetActive(true);
			}
		}
		else
		{
			if (clubData.teamType != ClubTypes.PUBLIC)
			{
				return;
			}
			if (clubData.IsFull())
			{
				if ((bool)disableInviteButton)
				{
					disableInviteButton.SetActive(true);
				}
			}
			else if ((bool)inviteButton)
			{
				inviteButton.SetActive(true);
			}
		}
	}

	private void InviteButton()
	{
		Log.Warning("Invite");
		string subject = "ui_clubinvite_subject_template".Localize("Join my club on Super Battle Tactics.");
		string text = string.Format("ui_clubinvite_body_template".Localize("Come join my club on Super Battle Tactics!\nClub Name: {0}\nPassword: {1}\nYou can download the game here: http://bit.ly/battletactics"), clubData.name, clubData.password);
		if (Etcetera.isSMSAvailable())
		{
			EtceteraAndroid.showSMSComposer(text);
		}
		else
		{
			EtceteraAndroid.showEmailComposer(string.Empty, subject, text, true);
		}
	}
}
