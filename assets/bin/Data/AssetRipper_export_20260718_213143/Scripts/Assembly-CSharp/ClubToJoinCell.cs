using System;
using UnityEngine;

public class ClubToJoinCell : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh clubNameLabel;

	[SerializeField]
	private tk2dTextMesh clubTypeLabel;

	[SerializeField]
	private tk2dTextMesh clubMembersLabel;

	[SerializeField]
	private tk2dBaseSprite backgroundSprite;

	[SerializeField]
	private GameObject buttonJoin;

	[SerializeField]
	private GameObject buttonJoinDisabled;

	[SerializeField]
	private PrefabProxy teamBadge;

	public bool useOddColoring;

	[NonSerialized]
	public UserClub clubData;

	public event Action<ClubToJoinCell> OnDetailsPressed;

	public event Action<ClubToJoinCell> OnJoinPressed;

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		clubData = (UserClub)dataObject;
		if (clubData != null)
		{
			if ((bool)clubMembersLabel)
			{
				clubMembersLabel.text = clubData.members.Count + "/" + Constants.ClubsTotalMembers;
			}
			if ((bool)clubNameLabel)
			{
				clubNameLabel.text = clubData.name;
			}
			if ((bool)clubTypeLabel)
			{
				clubTypeLabel.text = clubData.GetLocalizatedTeamType();
			}
			if ((bool)teamBadge)
			{
				StartCoroutine(teamBadge.ChangeAssetCoroutine(clubData.TeamBadgeAssetLinkage));
			}
			if (!UserProfile.player.IsClubMember)
			{
				if ((bool)buttonJoin)
				{
					buttonJoin.SetActive(!clubData.IsFull());
				}
				if ((bool)buttonJoinDisabled)
				{
					buttonJoinDisabled.SetActive(clubData.IsFull());
				}
			}
			else
			{
				if ((bool)buttonJoin)
				{
					buttonJoin.SetActive(false);
				}
				if ((bool)buttonJoinDisabled)
				{
					buttonJoinDisabled.SetActive(false);
				}
			}
		}
		if (useOddColoring)
		{
			if (dataIndex % 2 == 0)
			{
				backgroundSprite.gameObject.SetActive(false);
			}
			else
			{
				backgroundSprite.gameObject.SetActive(true);
			}
		}
	}

	private void OnDetails()
	{
		if (this.OnDetailsPressed != null)
		{
			this.OnDetailsPressed(this);
		}
	}

	private void OnJoin()
	{
		if (this.OnJoinPressed != null)
		{
			this.OnJoinPressed(this);
		}
	}
}
