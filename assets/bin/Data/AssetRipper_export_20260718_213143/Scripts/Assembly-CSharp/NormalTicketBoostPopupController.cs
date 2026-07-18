public class NormalTicketBoostPopupController : TicketBoostPopupController
{
	protected override void Start()
	{
		base.Start();
		if (!Constants.ShowBiggerBoostInNormalBattle)
		{
			_currentSelectedToggle = ToggleStates.StandardBoost;
			_boostToggleGroup.gameObject.SetActive(false);
		}
		ConfigureBoostToggle();
		foreach (BoostDataModel boost in _boostList)
		{
			switch (boost.Type)
			{
			case BoostType.TicketNormalBattle_1:
				_twoTicketsPointBoost.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketNormalBattle_2:
				_treeTicketsPointBoost.Init(boost, base.OnBattleButton);
				break;
			}
		}
	}

	protected override void ConfigureBiggerBoost()
	{
		foreach (BoostDataModel boost in _boostList)
		{
			switch (boost.Type)
			{
			case BoostType.TicketNormalBigBoost_1:
				_biggerPointBoost1.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketNormalBigBoost_2:
				_biggerPointBoost2.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketNormalBigBoost_3:
				_biggerPointBoost3.Init(boost, base.OnBattleButton);
				break;
			}
		}
	}
}
