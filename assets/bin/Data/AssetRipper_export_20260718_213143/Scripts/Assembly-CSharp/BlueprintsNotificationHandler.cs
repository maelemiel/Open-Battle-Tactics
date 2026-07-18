using System.Collections.Generic;

public class BlueprintsNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.BlueprintsScene;

	private MenuBarNotification menuBarNotification;

	public BlueprintsNotificationHandler(MenuBarNotification menuBarNotification)
	{
		this.menuBarNotification = menuBarNotification;
	}

	public NotificationType UpdateNotifications()
	{
		MenuBarNotification menuBarNotification = this.menuBarNotification;
		if (menuBarNotification == null)
		{
			return NotificationType.NONE;
		}
		NotificationType result = NotificationType.NONE;
		int builtTanksCount = GetBuiltTanksCount();
		if (builtTanksCount > 0)
		{
			menuBarNotification.EnableNotification(NotificationType.GREEN, builtTanksCount);
			menuBarNotification.DisableNotification(NotificationType.RED);
			result = NotificationType.GREEN;
		}
		else
		{
			int tanksToBuildCount = GetTanksToBuildCount();
			if (tanksToBuildCount > 0)
			{
				menuBarNotification.EnableNotification(NotificationType.RED, tanksToBuildCount);
				result = NotificationType.RED;
			}
			else
			{
				menuBarNotification.DisableNotification(NotificationType.RED);
			}
			menuBarNotification.DisableNotification(NotificationType.GREEN);
		}
		return result;
	}

	public static int GetBuiltTanksCount()
	{
		int num = 0;
		List<UserResearcher> researchers = UserProfile.player.researchers;
		for (int i = 0; i < researchers.Count; i++)
		{
			UserResearcher userResearcher = researchers[i];
			if (userResearcher.CanClaim && userResearcher.researchType == UserResearcher.ResearchType.BuildTank)
			{
				num++;
			}
		}
		return num;
	}

	public static int GetTanksToBuildCount()
	{
		int num = 0;
		UserProfile player = UserProfile.player;
		int divisionInt = player.divisionInt;
		List<UnitDataModel> all = UnitDataModel.GetAll();
		for (int i = 0; i < all.Count; i++)
		{
			UnitDataModel unitDataModel = all[i];
			if (unitDataModel.UnitType.IsExclusive() || divisionInt < unitDataModel.unlockTier || unitDataModel.unlockTier == 0)
			{
				continue;
			}
			int num2 = player.TimesBuiltUnit(unitDataModel.id);
			if (num2 <= 0)
			{
				UserPriceDataModel buildPrice = unitDataModel.GetBuildPrice(num2);
				if (player.CanAfford(buildPrice))
				{
					num++;
				}
			}
		}
		return num;
	}
}
