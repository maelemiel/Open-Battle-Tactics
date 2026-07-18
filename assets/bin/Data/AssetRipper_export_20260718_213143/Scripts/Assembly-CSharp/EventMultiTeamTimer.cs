using UnityEngine;

public class EventMultiTeamTimer : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh timer;

	private UserProfile userProfile;

	private UserMultiTeamReport userMultiTeam;

	public void Init()
	{
		userProfile = UserProfile.player;
		userMultiTeam = userProfile.userMultiTeamReport;
		base.gameObject.SetActive(userMultiTeam != null && userMultiTeam.RemainingTime != 0);
	}

	private void Update()
	{
		if (userMultiTeam != null)
		{
			if (userMultiTeam.RemainingTime == 0L)
			{
				base.gameObject.SetActive(false);
			}
			else if (base.gameObject.activeSelf)
			{
				timer.text = userMultiTeam.RemainingTimeText;
			}
		}
	}
}
