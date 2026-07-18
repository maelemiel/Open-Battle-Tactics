using System.Collections.Generic;

public class PrizeGachaNotificationChecker : INotificationChecker
{
	private bool[] previousGachaStates;

	private UserProfile userProfile;

	public void Init()
	{
		userProfile = UserProfile.player;
		previousGachaStates = GetGachaStates(userProfile.userGachaPrizes);
		for (int i = 0; i < previousGachaStates.Length; i++)
		{
			if (previousGachaStates[i])
			{
				SendNotification();
				break;
			}
		}
	}

	public void CheckConditions()
	{
		if (userProfile == null)
		{
			return;
		}
		bool[] gachaStates = GetGachaStates(userProfile.userGachaPrizes);
		if (gachaStates.Length < previousGachaStates.Length)
		{
			Log.Warning("Current state length is different than the previous state. This shouldn't ever happen. The condition check is skipped");
			previousGachaStates = gachaStates;
			return;
		}
		for (int i = 0; i < previousGachaStates.Length; i++)
		{
			if (gachaStates[i] && previousGachaStates[i] != gachaStates[i])
			{
				previousGachaStates = gachaStates;
				SendNotification();
				break;
			}
		}
	}

	public void SendNotification()
	{
		for (int i = 0; i < previousGachaStates.Length; i++)
		{
			if (previousGachaStates[i])
			{
				LocalUserNotificationModel.PrizeGachaReady notification = new LocalUserNotificationModel.PrizeGachaReady(userProfile.userGachaPrizes[i].PrizeGachaDataModel);
				if ((bool)TopBarController.instance)
				{
					TopBarController.instance.AddLocalNotification(notification);
				}
			}
		}
	}

	private bool[] GetGachaStates(List<UserGachaPrize> gachaPrizes)
	{
		bool[] array = new bool[gachaPrizes.Count];
		for (int i = 0; i < gachaPrizes.Count; i++)
		{
			array[i] = gachaPrizes[i].CanPlayGachaPrize && gachaPrizes[i].PrizeGachaDataModel.freeCooldown > 0;
		}
		return array;
	}
}
