using System;

public class TicketBoostPopupModel
{
	public Action<BoostType> onBattleCallback;

	public TicketBoostPopupModel(Action<BoostType> onBattleCallback)
	{
		this.onBattleCallback = onBattleCallback;
	}
}
