using System.Collections.Generic;

public class PrizeGachaNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.ShopItemsSuppliesScene;

	private MenuBarNotification menuBarNotification;

	public PrizeGachaNotificationHandler(MenuBarNotification menuBarNotification)
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
		List<UserGachaPrize> userGachaPrizes = UserProfile.player.userGachaPrizes;
		NotificationType result = NotificationType.NONE;
		if (userGachaPrizes.Count > 0)
		{
			int num = 0;
			for (int i = 0; i < userGachaPrizes.Count; i++)
			{
				if (userGachaPrizes[i] != null && userGachaPrizes[i].PrizeGachaDataModel != null && userGachaPrizes[i].CanPlayGachaPrize && userGachaPrizes[i].PrizeGachaDataModel.freeCooldown > 0)
				{
					num++;
				}
			}
			if (num > 0)
			{
				menuBarNotification.EnableNotification(NotificationType.GREEN, num);
				result = NotificationType.GREEN;
			}
			else
			{
				menuBarNotification.DisableNotification(NotificationType.GREEN);
			}
		}
		else
		{
			menuBarNotification.DisableNotification(NotificationType.GREEN);
		}
		return result;
	}
}
