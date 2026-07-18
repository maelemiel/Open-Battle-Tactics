using UnityEngine;

public class ResetBotBattleCapPopUpController : PopupController
{
	[SerializeField]
	private PriceLabelController resetPriceLabel;

	[SerializeField]
	private tk2dTextMesh bodyText;

	private int resetCost;

	protected override void Start()
	{
		base.Start();
		resetCost = UserProfile.player.botBattleRestoreCount * Constants.BotBattleDailyCapRestoreCost;
		Confugure();
	}

	protected override void Update()
	{
		base.Update();
		string countdownToNextDayString = NonUnitySingleton<TimeManager>.instance.GetCountdownToNextDayString(true);
		bodyText.text = string.Format("ui_reset_bot_battle_count_body".Localize("You reached the cap of bot battle. This value will be reseted in {0}. But you can press ok for reset rigth now and get {1} options for battle again bot."), countdownToNextDayString, Constants.MaxBostBattlesPerDay);
	}

	protected void Confugure()
	{
		UserPriceDataModel priceDataModel = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, resetCost);
		resetPriceLabel.ConfigurePriceLabel(priceDataModel);
	}

	private void OnClickYes()
	{
		if (UserProfile.player.gems < resetCost)
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
			{
				PopupManager.DestroyAllPopups();
				TopBarController.instance.LoadShop();
			}));
		}
		else
		{
			ResetTransaction();
		}
		base.Close();
	}

	private void ResetTransaction()
	{
		UserPriceDataModel price = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, resetCost);
		Singleton<SessionManager>.instance.ResetBotBattleDailyCount(price);
		if (model.rightAction != null)
		{
			model.rightAction();
		}
	}
}
