public class BuiltTankNotificationChecker : INotificationChecker
{
	private bool previousState;

	private bool currentState;

	private UserProfile userProfile;

	public void Init()
	{
		userProfile = UserProfile.player;
		previousState = IsTankBuilt();
		if (previousState)
		{
			currentState = previousState;
			SendNotification();
		}
	}

	public void CheckConditions()
	{
		previousState = currentState;
		currentState = IsTankBuilt();
		if (currentState && previousState != currentState)
		{
			SendNotification();
		}
	}

	public void SendNotification()
	{
		LocalUserNotificationModel.UnitBuilt notification = new LocalUserNotificationModel.UnitBuilt((UnitDataModel)userProfile.ClaimableResearcher.ResearchItem);
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.AddLocalNotification(notification);
		}
	}

	private bool IsTankBuilt()
	{
		bool result = false;
		if (userProfile != null && userProfile.ClaimableResearcher != null)
		{
			result = UserProfile.player.ClaimableResearcher.researchType == UserResearcher.ResearchType.BuildTank;
		}
		return result;
	}
}
