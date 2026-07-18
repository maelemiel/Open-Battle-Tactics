using System.Collections.Generic;

public class ReadyToBuildTanksNotificationChecker : INotificationChecker
{
	private int previousTanksReadyToBuildCount;

	private List<UnitDataModel> previousUnits = new List<UnitDataModel>();

	private int currentTanksReadyToBuildCount;

	private UserProfile userProfile;

	public void Init()
	{
		userProfile = UserProfile.player;
		previousTanksReadyToBuildCount = GetTanksReadyToBuild();
		previousUnits = userProfile.GetBuildList();
	}

	public void CheckConditions()
	{
		previousTanksReadyToBuildCount = currentTanksReadyToBuildCount;
		currentTanksReadyToBuildCount = GetTanksReadyToBuild();
		if (currentTanksReadyToBuildCount > 0 && previousTanksReadyToBuildCount != currentTanksReadyToBuildCount)
		{
			SendNotification();
		}
	}

	public void SendNotification()
	{
		List<UnitDataModel> buildList = userProfile.GetBuildList();
		buildList.RemoveAll((UnitDataModel x) => previousUnits.Contains(x));
		for (int num = 0; num < buildList.Count; num++)
		{
			LocalUserNotificationModel.PartsCollected notification = new LocalUserNotificationModel.PartsCollected(buildList[num]);
			if ((bool)TopBarController.instance)
			{
				TopBarController.instance.AddLocalNotification(notification);
			}
		}
		previousUnits = userProfile.GetBuildList();
	}

	private int GetTanksReadyToBuild()
	{
		if (userProfile != null)
		{
			return userProfile.GetBuildCount();
		}
		return 0;
	}
}
