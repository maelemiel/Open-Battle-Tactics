using UnityEngine;

public class RaidBossTicketBoostPopupController : PopupController
{
	[SerializeField]
	private PointBoostController _standartPointBoost;

	[SerializeField]
	private PointBoostController _twoTicketsPointBoost;

	[SerializeField]
	private PointBoostController _treeTicketsPointBoost;

	private TicketBoostPopupModel popupModel;

	protected override void Start()
	{
		base.Start();
		popupModel = (TicketBoostPopupModel)model.payload;
		_standartPointBoost.Init(null, OnBattleButton);
		foreach (BoostDataModel item in BoostDataModel.GetAll())
		{
			switch (item.Type)
			{
			case BoostType.TicketRBDmgBoost_1:
				_twoTicketsPointBoost.Init(item, OnBattleButton);
				break;
			case BoostType.TicketRBDmgBoost_2:
				_treeTicketsPointBoost.Init(item, OnBattleButton);
				break;
			}
		}
	}

	public override void OnCloseButton()
	{
		if (model.closeButtonAction != null)
		{
			model.closeButtonAction();
		}
		Close();
	}

	private void OnBattleButton(BoostType boostType, int energy_count)
	{
		if (UserProfile.player.energy >= energy_count)
		{
			GoToBattle(boostType);
			return;
		}
		if (energy_count == 1)
		{
			TopBarController.instance.BuyEnergyThenDoAction("ui_tickets_depleted_title", "ui_buy_tickets_message", delegate
			{
				GoToBattle(boostType);
			});
			return;
		}
		int energy = UserProfile.player.energy;
		TopBarController.instance.BuyBulkEnergyThenDoAction(energy_count - energy, energy_count, "ui_tickets_depleted_title", "ui_buy_bulk_tickets_message", delegate
		{
			GoToBattle(boostType);
		});
	}

	private void GoToBattle(BoostType boostType)
	{
		Close();
		if (popupModel != null && popupModel.onBattleCallback != null)
		{
			popupModel.onBattleCallback(boostType);
		}
	}
}
