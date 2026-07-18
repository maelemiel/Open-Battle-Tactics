public class ServerAbilityState
{
	public IAbilityMetadata metadata;

	public int energy;

	public int lastActivationRound;

	public bool isActive;

	public int addedBoostA;

	public int addedBoostB;

	public ServerUnitState target;

	public ServerAbilityHandler handler;

	public IAbilityAnimationHandler animHandler;

	public bool firstAbility;

	public int abilityIndex;

	public float boostMultiplier = 1f;

	public ServerBattleState battle;

	public ServerTeamState team;

	public string AttackName = string.Empty;

	public bool CanActivate
	{
		get
		{
			if (team.energy < 1)
			{
				return false;
			}
			if (metadata.LimitOnePerRound && lastActivationRound == team.battle.currentRound)
			{
				return false;
			}
			return true;
		}
	}

	public string Name
	{
		get
		{
			return UnitSpecialDataModel.GetName(target.metadata.GetAbilityMetaData(abilityIndex), BoostValue, SecondaryBoostValue);
		}
	}

	public int BoostValue
	{
		get
		{
			int baseBoostValue = BaseBoostValue;
			if (metadata.IsUnitAbility)
			{
				return baseBoostValue + addedBoostA;
			}
			return baseBoostValue;
		}
	}

	public int SecondaryBoostValue
	{
		get
		{
			int baseSecondaryBoostValue = BaseSecondaryBoostValue;
			if (metadata.IsUnitAbility)
			{
				return baseSecondaryBoostValue + addedBoostB;
			}
			return baseSecondaryBoostValue;
		}
	}

	public int BaseBoostValue
	{
		get
		{
			if (metadata.IsUnitAbility)
			{
				return target.metadata.GetAbilityBoostValueA(abilityIndex);
			}
			return metadata.BoostValue;
		}
	}

	public int BaseSecondaryBoostValue
	{
		get
		{
			if (metadata.IsUnitAbility)
			{
				return target.metadata.GetAbilityBoostValueB(abilityIndex);
			}
			return metadata.SecondaryBoostValue;
		}
	}

	internal ServerAbilityState Clone()
	{
		ServerAbilityState serverAbilityState = new ServerAbilityState();
		serverAbilityState.metadata = metadata;
		serverAbilityState.energy = energy;
		serverAbilityState.lastActivationRound = lastActivationRound;
		serverAbilityState.isActive = isActive;
		serverAbilityState.target = target;
		serverAbilityState.firstAbility = firstAbility;
		serverAbilityState.addedBoostA = addedBoostA;
		serverAbilityState.addedBoostB = addedBoostB;
		return serverAbilityState;
	}
}
