public class PvpEventTicketBoostPopupController : TicketBoostPopupController
{
	protected override void Start()
	{
		base.Start();
		if (!Constants.ShowBiggerBoostInPvpTournament)
		{
			_currentSelectedToggle = ToggleStates.StandardBoost;
			_boostToggleGroup.gameObject.SetActive(false);
		}
		ConfigureBoostToggle();
		foreach (BoostDataModel boost in _boostList)
		{
			switch (boost.Type)
			{
			case BoostType.TicketPvpEventBoost_1:
				_twoTicketsPointBoost.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPvpEventBoost_2:
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
			case BoostType.TicketPvpEventBigBoost_1:
				_biggerPointBoost1.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPvpEventBigBoost_2:
				_biggerPointBoost2.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPvpEventBigBoost_3:
				_biggerPointBoost3.Init(boost, base.OnBattleButton);
				break;
			}
		}
	}
}
