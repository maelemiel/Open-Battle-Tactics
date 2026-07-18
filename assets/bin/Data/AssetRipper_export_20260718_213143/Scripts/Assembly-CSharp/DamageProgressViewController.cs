using System.Collections.Generic;
using UnityEngine;

public class DamageProgressViewController : MonoBehaviour
{
	[SerializeField]
	private tk2dUIProgressBar progressBar;

	[SerializeField]
	private tk2dSlicedSprite progressBarMainSprite;

	[SerializeField]
	private GachaBoxesController gachaBoxReasourcesController;

	public RewardBoxView boxRewardViewPrefab;

	private List<EventRaidbossDamageDropRateDataModel> currentTeamDrops;

	private float initialBoxesHorizontalOffset;

	private float currentProgress;

	private float progressBarSizeConversionFactor;

	private float uiProgressBarLength;

	private float dataModelMaxThreshold;

	private float previousProgress;

	private List<RewardBoxView> rewardBoxes = new List<RewardBoxView>();

	private int completedTiers;

	private void Awake()
	{
		if ((bool)progressBarMainSprite)
		{
			uiProgressBarLength = progressBarMainSprite.dimensions.x;
		}
		initialBoxesHorizontalOffset = 0f - uiProgressBarLength * 0.5f;
	}

	public void Init(List<EventRaidbossDamageDropRateDataModel> teamDrops, float bossHP = 0f)
	{
		currentTeamDrops = teamDrops;
		currentTeamDrops.Sort((EventRaidbossDamageDropRateDataModel x, EventRaidbossDamageDropRateDataModel y) => x.threshold.CompareTo(y.threshold));
		dataModelMaxThreshold = ((currentTeamDrops.Count <= 0) ? bossHP : ((float)currentTeamDrops[currentTeamDrops.Count - 1].threshold));
		progressBarSizeConversionFactor = uiProgressBarLength / dataModelMaxThreshold;
		SetProgress(0f);
		SetupRewardBoxes();
	}

	private void SetupRewardBoxes()
	{
		float num = 0f;
		for (int i = 0; i < currentTeamDrops.Count; i++)
		{
			EventRaidbossDamageDropRateDataModel eventRaidbossDamageDropRateDataModel = currentTeamDrops[i];
			RewardBoxView rewardBoxView = Object.Instantiate(boxRewardViewPrefab) as RewardBoxView;
			rewardBoxView.transform.parent = base.gameObject.transform;
			num = (float)eventRaidbossDamageDropRateDataModel.threshold * progressBarSizeConversionFactor + initialBoxesHorizontalOffset;
			rewardBoxView.transform.localPosition = new Vector3(num, 0f, 0f);
			tk2dSpineSkeletonDataAsset gachaBoxSkeletonDataAsset = gachaBoxReasourcesController.GetGachaBoxSkeletonDataAsset((GachaTypes)currentTeamDrops[i].boxtype);
			rewardBoxView.SetupView(gachaBoxSkeletonDataAsset);
			if ((bool)rewardBoxView)
			{
				rewardBoxes.Add(rewardBoxView);
			}
		}
	}

	public void ResetProgress()
	{
		foreach (RewardBoxView rewardBox in rewardBoxes)
		{
			rewardBox.ResetEffect();
		}
	}

	public void SetProgress(float progressValue)
	{
		previousProgress = currentProgress;
		for (int i = 0; i < currentTeamDrops.Count; i++)
		{
			if ((float)currentTeamDrops[i].threshold + dataModelMaxThreshold * (float)completedTiers > previousProgress && (float)currentTeamDrops[i].threshold + dataModelMaxThreshold * (float)completedTiers <= progressValue)
			{
				rewardBoxes[i].PlayEffect();
			}
		}
		if (HasTeamDrops() && progressValue >= dataModelMaxThreshold * (float)(completedTiers + 1))
		{
			ResetProgress();
			completedTiers++;
		}
		currentProgress = progressValue;
		if ((bool)progressBar)
		{
			progressBar.Value = progressValue % dataModelMaxThreshold / dataModelMaxThreshold;
		}
	}

	private bool HasTeamDrops()
	{
		return currentTeamDrops.Count > 0;
	}
}
