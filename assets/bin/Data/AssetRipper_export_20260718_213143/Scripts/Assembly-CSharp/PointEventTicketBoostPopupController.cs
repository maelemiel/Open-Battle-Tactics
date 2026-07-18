public class PointEventTicketBoostPopupController : TicketBoostPopupController
{
	protected override void Start()
	{
		base.Start();
		ConfigureBoostToggle();
		foreach (BoostDataModel boost in _boostList)
		{
			switch (boost.Type)
			{
			case BoostType.TicketPointEventBoost_1:
				_twoTicketsPointBoost.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPointEventBoost_2:
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
			case BoostType.TicketPointEventBigBoost_1:
				_biggerPointBoost1.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPointEventBigBoost_2:
				_biggerPointBoost2.Init(boost, base.OnBattleButton);
				break;
			case BoostType.TicketPointEventBigBoost_3:
				_biggerPointBoost3.Init(boost, base.OnBattleButton);
				break;
			}
		}
	}
}
