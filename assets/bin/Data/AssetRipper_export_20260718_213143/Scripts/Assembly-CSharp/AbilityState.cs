public class AbilityState : ServerAbilityState
{
	public AbilityAnimationHandler animationHandler;

	public new UnitState target
	{
		get
		{
			return base.target as UnitState;
		}
		set
		{
			base.target = value;
		}
	}

	public new TeamState team
	{
		get
		{
			return base.team as TeamState;
		}
		set
		{
			base.team = value;
		}
	}
}
