public interface IBattleAnimationHandler
{
	void RoundStart(int roundIndex);

	void RoundComplete(int roundIndex);

	void FinishBattle(ServerTeamState winningTeam);

	void PreInitiativeAbility(ServerAbilityState ability);

	void PreInitiativeResultsAbility(ServerAbilityState ability, int intelBoostTeam, int intelBoostOtherTeam);

	void InitiativeResults(ServerTeamState winningTeam, int hostTeamInitiative, int guestTeamInitiative);

	void PostInitiativeAbility(ServerAbilityState ability);

	void TeamBeginAttack(ServerTeamState team);

	void TeamBeginAttackAbility(ServerTeamState team, ServerAbilityState ability);

	void UnitFiringAbility(ServerUnitState unit, ServerAbilityState ability, ServerUnitState target);

	void PassiveFiringAnimation(ServerUnitState _unit, ServerAbilityState _ability);

	void UnitAttack(ServerUnitState unit, ServerUnitState target, int damage, DamageType dmgType);

	void UnitReceivedDamageAbility(ServerUnitState unit, int damage, DamageType dmgType, ServerAbilityState ability);

	void UnitDiedAbility(ServerUnitState unit, ServerTeamState team, ServerAbilityState ability);

	void TeamEndAttack(ServerTeamState team);

	void EndOfRoundWithAbilities(ServerTeamState team, ServerAbilityState ability);

	void PreFireAbility(ServerAbilityState ability, ServerUnitState unit, ServerUnitState target);

	void PreActivateAbility(ServerAbilityState ability, ServerUnitState target);

	void ActivateAbility(ServerAbilityState ability, ServerUnitState target);

	void DeactivateAbility(ServerAbilityState ability);

	void JamAbility(ServerAbilityState jammedAbility, ServerAbilityState jammerAbility);

	void InvestAbilityEnergy(ServerAbilityState ability);

	void ReRoll(ServerAbilityState ability, int index);

	void QuickDraw(ServerAbilityState animation);

	void TeamForfeit(ServerTeamState team);

	void TeamBestRoll(ServerTeamState team);

	void TeamWorstRoll(ServerTeamState team);

	void UnitWorstRolls(ServerUnitState unit);

	void UnitAcidDamage(ServerUnitState unit, int acidIncrease);

	void ApplyDamagePerRound();
}
