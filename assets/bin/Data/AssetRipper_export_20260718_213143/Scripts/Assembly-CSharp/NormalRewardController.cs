using UnityEngine;

public class NormalRewardController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh rankRange;

	[SerializeField]
	private GameObject backgroundDynamic;

	[SerializeField]
	private GameObject backgroundDynamicOdd;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dSlicedSprite backgroundSpriteBg;

	[SerializeField]
	private GameObject[] topRankBadges;

	public bool useOddColoring;

	public virtual void Configure(LeaderboardRewardsEntryData data, bool dinamicBackground)
	{
		if (data.rankStart == data.rankEnd)
		{
			rankRange.text = data.rankStart.ToString();
		}
		else
		{
			rankRange.text = string.Format("ui_leaderboards_rewardrank".Localize("{0} - {1}"), data.rankStart, data.rankEnd);
		}
		priceLabel.ConfigurePriceLabel(data.items);
		if (useOddColoring)
		{
			backgroundDynamic.SetActive(dinamicBackground);
			backgroundDynamicOdd.SetActive(!dinamicBackground);
		}
		bool flag = false;
		for (int i = 0; i < topRankBadges.Length; i++)
		{
			bool flag2 = i == data.rankStart - 1;
			topRankBadges[i].SetActive(flag2);
			flag = flag || flag2;
		}
	}
}
