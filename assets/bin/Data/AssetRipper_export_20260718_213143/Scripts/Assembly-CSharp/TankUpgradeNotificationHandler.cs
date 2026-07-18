using System.Collections.Generic;

public class TankUpgradeNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.ArenaScene;

	private MenuBarNotification menuBarNotification;

	public TankUpgradeNotificationHandler(MenuBarNotification menuBarNotification)
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
		int num = 0;
		foreach (UserUnit value in UserProfile.player.unitInventory.Values)
		{
			if (!value.UpgradeCostsParts())
			{
				continue;
			}
			List<UnitPartialLevelDataModel> partialLevelsForCurrentLevel = value.GetPartialLevelsForCurrentLevel();
			if (partialLevelsForCurrentLevel.Count <= 0)
			{
				if (!value.IsMaxLevel && UserProfile.player.CanAfford(value.GetUpgradePrice()))
				{
					num++;
				}
				continue;
			}
			for (int i = 0; i < partialLevelsForCurrentLevel.Count; i++)
			{
				if ((value.partialLevel & (1 << partialLevelsForCurrentLevel[i].partIndex)) == 0 && UserProfile.player.CanAfford(partialLevelsForCurrentLevel[i].requirementPriceId))
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			menuBarNotification.EnableNotification(NotificationType.RED, num);
			result = NotificationType.RED;
		}
		else
		{
			menuBarNotification.DisableNotification(NotificationType.RED);
		}
		return result;
	}
}
