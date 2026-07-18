using UnityEngine;

public class PlayerView : MonoBehaviour
{
	private const string WIN_STREAK_LOC_KEY = "ui_battle_winstreak";

	[SerializeField]
	private Color pvpRatingTextcolor = Color.white;

	[SerializeField]
	private Color positivePVPDeltaTextColor = Color.white;

	[SerializeField]
	private Color negativePVPDeltaTextColor = Color.white;

	public tk2dTextMesh playerNameLabel;

	public PrefabProxy playerBadge;

	public tk2dTextMesh winStreakLabel;

	public tk2dTextMesh pvpRatingLabel;

	public StreamingThumbnail avatar;

	public void ConfigureView(OpponentData player, bool showDeltaPVP, int deltaPVP)
	{
		if ((bool)playerNameLabel)
		{
			playerNameLabel.text = player.name;
		}
		if ((bool)winStreakLabel)
		{
			winStreakLabel.text = "ui_battle_winstreak".Localize("Win Streak: ") + player.winStreak;
		}
		if ((bool)pvpRatingLabel)
		{
			pvpRatingLabel.text = pvpRatingTextcolor.InlineStyleCode() + "ui_pvp_results_rating".Localize("PVP Rating: ") + Color.white.InlineStyleCode() + player.pvpRating;
			if (showDeltaPVP)
			{
				string text = ((deltaPVP < 0) ? negativePVPDeltaTextColor.InlineStyleCode() : positivePVPDeltaTextColor.InlineStyleCode());
				tk2dTextMesh obj = pvpRatingLabel;
				string text2 = obj.text;
				obj.text = text2 + " [" + text + deltaPVP + Color.white.InlineStyleCode() + "]";
			}
		}
		if ((bool)playerBadge && player.division is ProgressionDivisionDataModel)
		{
			ProgressionDivisionDataModel progressionDivisionDataModel = (ProgressionDivisionDataModel)player.division;
			if (progressionDivisionDataModel != null)
			{
				AssetLinkageDataModel assetLinkageDataModel = progressionDivisionDataModel.BadgeLinkage;
				if (assetLinkageDataModel == null)
				{
					assetLinkageDataModel = AssetLinkageDataModel.GetSingle(7027);
				}
				StartCoroutine(playerBadge.ChangeAssetCoroutine(assetLinkageDataModel));
			}
		}
		if ((bool)avatar)
		{
			avatar.ChangeThumbnail(player.thumbnailURL);
			Debug.LogWarning("User with name: [" + player.name + "] has a thumbnail with URL: " + player.thumbnailURL);
		}
	}
}
