using System;
using UnityEngine;

public class BlueprintsTierView : MonoBehaviour
{
	[SerializeField]
	private GenericGachaBoxController gachaBox;

	[SerializeField]
	private GameObject unlockedRewardIcon;

	private ProgressionDivisionDataModel localDivisionDataModel;

	private int currentTier;

	private Action callback;

	[SerializeField]
	private Transform[] rewardPositions;

	public void ConfigureView(ProgressionDivisionDataModel divisionDataModel, bool active = true, Action callback = null)
	{
		currentTier = int.Parse(divisionDataModel.id);
		localDivisionDataModel = divisionDataModel;
		if (UserProfile.player != null)
		{
			UserProfile.player.OnResearchClaimed -= Refresh;
			UserProfile.player.OnResearchClaimed += Refresh;
		}
		this.callback = callback;
		Refresh();
	}

	public void OnDestroy()
	{
		if (UserProfile.player != null)
		{
			UserProfile.player.OnResearchClaimed -= Refresh;
		}
	}

	public void Refresh()
	{
		if (!(gachaBox == null))
		{
			if (UserProfile.player.HasClaimedDivisionReward(currentTier))
			{
				gachaBox.gameObject.SetActive(false);
				unlockedRewardIcon.SetActive(true);
			}
			else
			{
				gachaBox.gameObject.SetActive(true);
				unlockedRewardIcon.SetActive(false);
			}
		}
	}

	public void OnTierPressed()
	{
		if (UserProfile.player == null || !localDivisionDataModel.UnitsAllBuilt || UserProfile.player.HasClaimedDivisionReward(currentTier))
		{
			return;
		}
		GrantGiftEffect.Create(this, localDivisionDataModel.CompletionClaimReward, gachaBox.transform.position, new Vector3(0f, 500f, 0f), base.gameObject.layer, rewardPositions);
		Reporting.UnitSeriesCompleteEvent(currentTier, localDivisionDataModel.CompletionClaimReward);
		gachaBox.Open(delegate
		{
			UserProfile.player.TryClaimDivisionReward(currentTier, delegate
			{
				Refresh();
				if (callback != null)
				{
					callback();
				}
			});
		});
	}
}
