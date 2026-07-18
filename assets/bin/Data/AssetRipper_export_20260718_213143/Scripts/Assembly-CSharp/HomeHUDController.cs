using UnityEngine;

public class HomeHUDController : MonoBehaviour
{
	private const string WIN_STREAK_SUFFIX = " GAME WIN STREAK";

	[SerializeField]
	private tk2dTextMesh winStreakLabel;

	public void Init(HomeController homeController)
	{
		UserProfile player = UserProfile.player;
		if ((bool)winStreakLabel)
		{
			winStreakLabel.text = player.winStreak + " GAME WIN STREAK";
		}
	}
}
