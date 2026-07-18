public class ServerUnitAbilityHandler : ServerAbilityHandler
{
	public ServerUnitState OwnerUnit
	{
		get
		{
			return abilityState.target;
		}
	}

	public bool IsOwnerUnitDead
	{
		get
		{
			return OwnerUnit.IsDead;
		}
	}

	public bool IsOwnerUnitAlive
	{
		get
		{
			return !OwnerUnit.IsDead;
		}
	}
}
