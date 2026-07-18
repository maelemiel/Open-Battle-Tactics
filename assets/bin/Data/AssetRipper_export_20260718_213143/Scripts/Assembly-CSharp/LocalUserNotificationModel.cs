public class LocalUserNotificationModel
{
	public class UnitBuilt : LocalUserNotificationModel
	{
		public UnitBuilt(UnitDataModel unit)
		{
			dataObject = unit;
			notificationType = LocalNotificationTypes.TANK_BUILT;
		}

		public UnitDataModel GetNotificationDataObject()
		{
			return base.DataObject as UnitDataModel;
		}

		public override void ExecuteNotification()
		{
			if (SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.BlueprintsScene)
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene);
			}
		}
	}

	public class PartsCollected : LocalUserNotificationModel
	{
		public PartsCollected(UnitDataModel unit)
		{
			dataObject = unit;
			notificationType = LocalNotificationTypes.PARTS_COLLECTED;
		}

		public UnitDataModel GetNotificationDataObject()
		{
			return base.DataObject as UnitDataModel;
		}

		public override void ExecuteNotification()
		{
			if (SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.BlueprintsScene)
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene, new SceneModel(dataObject));
			}
		}
	}

	public class PrizeGachaReady : LocalUserNotificationModel
	{
		public PrizeGachaReady(GachaPoolsDataModel prizeGacha)
		{
			dataObject = prizeGacha;
			notificationType = LocalNotificationTypes.PRIZE_GACHA_READY;
		}

		public GachaPoolsDataModel GetNotificationDataObject()
		{
			return base.DataObject as GachaPoolsDataModel;
		}

		public override void ExecuteNotification()
		{
			if (SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ShopItemsSuppliesScene)
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
			}
		}
	}

	public class ClubCrateReady : LocalUserNotificationModel
	{
		public ClubCrateReady()
		{
			dataObject = null;
			notificationType = LocalNotificationTypes.CLUB_CRATE_READY;
		}

		public override void ExecuteNotification()
		{
			if (SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ClubScene)
			{
				TopBarController.instance.LoadClubs();
			}
		}
	}

	protected object dataObject;

	protected LocalNotificationTypes notificationType;

	public LocalNotificationTypes NotificationType
	{
		get
		{
			return notificationType;
		}
		set
		{
			notificationType = value;
		}
	}

	public object DataObject
	{
		get
		{
			return dataObject;
		}
		set
		{
			dataObject = value;
		}
	}

	public virtual void ExecuteNotification()
	{
	}
}
