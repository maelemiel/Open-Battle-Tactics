using System;
using System.Collections.Generic;

public static class BattleLogic
{
	public static void BeginBattle(ServerBattleState state)
	{
		SetupConvenienceData(state);
		state.currentRound = 0;
		state.cashPrizePool = 0;
		ServerTeamState[] bothTeams = state.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			ServerAbilityState[] abilities = serverTeamState.abilities;
			foreach (ServerAbilityState serverAbilityState in abilities)
			{
				serverAbilityState.energy = 0;
				serverAbilityState.handler.MatchBegin();
			}
			serverTeamState.stats = new ServerTeamStatsState();
			serverTeamState.stats.partsEarned = new List<IPartMetadata>();
			serverTeamState.stats.giftDrops = new List<int>();
			ServerUnitState[] units = serverTeamState.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				serverUnitState.rarity = serverUnitState.metadata.Rarity;
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					ability.target = serverUnitState;
					ability.handler.MatchBegin();
				}
				InitAbilityUnitBoosts(serverUnitState);
			}
			serverTeamState.aliveUnits = new ServerUnitState[serverTeamState.units.Length];
			for (int l = 0; l < serverTeamState.units.Length; l++)
			{
				serverTeamState.aliveUnits[l] = serverTeamState.units[l];
			}
		}
	}

	public static void SetupConvenienceData(ServerBattleState state)
	{
		if (state.hostTeam == state.teamTwo)
		{
			state.bothTeams = new ServerTeamState[2] { state.teamTwo, state.teamOne };
		}
		else
		{
			state.bothTeams = new ServerTeamState[2] { state.teamOne, state.teamTwo };
		}
		int num = 0;
		ServerTeamState[] bothTeams = state.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			List<ServerAbilityHandler> list = new List<ServerAbilityHandler>();
			serverTeamState.index = num++;
			serverTeamState.battle = state;
			serverTeamState.otherTeam = ((serverTeamState != state.teamOne) ? state.teamOne : state.teamTwo);
			ServerAbilityState[] abilities = serverTeamState.abilities;
			foreach (ServerAbilityState serverAbilityState in abilities)
			{
				if (serverAbilityState != null)
				{
					_SetupAbilityState(serverAbilityState, serverTeamState);
					list.Add(serverAbilityState.handler);
				}
			}
			int num2 = 0;
			ServerUnitState[] units = serverTeamState.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				serverUnitState.index = num2++;
				serverUnitState.team = serverTeamState;
				serverUnitState.battle = state;
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (ability != null)
					{
						_SetupAbilityState(ability, serverTeamState);
						list.Add(ability.handler);
					}
				}
			}
			serverTeamState.abilityHandlers = new ServerAbilityHandler[list.Count];
			for (int l = 0; l < list.Count; l++)
			{
				serverTeamState.abilityHandlers[l] = list[l];
			}
		}
		_CreateOrderedAbilityArray(state.teamOne);
		_CreateOrderedAbilityArray(state.teamTwo);
	}

	private static void _SetupAbilityState(ServerAbilityState ability, ServerTeamState team)
	{
		ability.team = team;
		ability.battle = team.battle;
		if (ability.handler == null)
		{
			ability.handler = AbilityHandlerFactory.Create(ability.metadata.Type, ability);
		}
	}

	private static void _CreateOrderedAbilityArray(ServerTeamState team)
	{
		team.allBattleAbilities = new ServerAbilityHandler[team.abilityHandlers.Length + team.otherTeam.abilityHandlers.Length];
		int num = team.abilityHandlers.Length;
		for (int i = 0; i < num; i++)
		{
			team.allBattleAbilities[i] = team.abilityHandlers[i];
		}
		for (int j = num; j < team.allBattleAbilities.Length; j++)
		{
			team.allBattleAbilities[j] = team.otherTeam.abilityHandlers[j - num];
		}
		_SortAbilityArray(team.allBattleAbilities);
	}

	private static void _SortAbilityArray(ServerAbilityHandler[] array)
	{
		for (int i = 1; i < array.Length; i++)
		{
			ServerAbilityHandler serverAbilityHandler = array[i];
			int num = i;
			while (num > 0 && array[num - 1].abilityState.metadata.ExecutionOrder > serverAbilityHandler.abilityState.metadata.ExecutionOrder)
			{
				array[num] = array[num - 1];
				num--;
			}
			array[num] = serverAbilityHandler;
		}
	}

	public static void BeginRound(ServerBattleState battle)
	{
		ServerTeamState[] bothTeams = battle.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			serverTeamState.energy = 3;
			serverTeamState.currentRoundKills = 0;
			ServerUnitState[] aliveUnits = serverTeamState.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				serverUnitState.armor = 0;
				serverUnitState.rollRepeats = 0;
				serverUnitState.currentRoll = -1;
				RerollUnit(serverUnitState, false, true, true);
				serverUnitState.roundStartRoll = serverUnitState.currentRoll;
				serverUnitState.preventReroll = serverUnitState.roundsUntilRerollEnabled > 0;
				if (serverUnitState.preventReroll)
				{
					serverUnitState.roundsUntilRerollEnabled--;
				}
			}
			EvaluateRollRewards(serverTeamState, true);
		}
		battle.currentRound++;
		if (battle.animationHandler != null)
		{
			battle.animationHandler.RoundStart(battle.currentRound);
		}
	}

	public static void SimulateRound(ServerBattleState battle)
	{
		if (!battle.IsComplete)
		{
			AbilityHooks.PreInitiativeEvent(battle);
			CalculateInitiativeWinner(battle);
			AbilityHooks.PostInitiativeEvent(battle, battle.initiativeWinner);
			ServerTeamState[] array = new ServerTeamState[2]
			{
				battle.initiativeWinner,
				battle.initiativeWinner.otherTeam
			};
			for (int i = 0; i < array.Length; i++)
			{
				ServerTeamState serverTeamState = (battle.actingTeam = array[i]);
				if (battle.animationHandler != null)
				{
					battle.animationHandler.TeamBeginAttack(serverTeamState);
				}
				AbilityHooks.TeamBeginAttackEvent(serverTeamState);
				ServerUnitState[] aliveUnits = serverTeamState.aliveUnits;
				foreach (ServerUnitState unit in aliveUnits)
				{
					if (battle.IsComplete)
					{
						break;
					}
					if (!AbilityHooks.UnitFiringEvent(unit))
					{
						if (battle.IsComplete)
						{
							break;
						}
						UnitAttack(unit);
					}
					DeactivateDeadUnitAbilities(battle);
				}
				if (battle.animationHandler != null)
				{
					battle.animationHandler.TeamEndAttack(serverTeamState);
				}
				if (battle.IsComplete)
				{
					break;
				}
			}
		}
		if (!battle.IsComplete)
		{
			AbilityHooks.EndOfRoundWithAbilitiesEvent(battle, battle.initiativeWinner);
		}
		ServerAbilityHandler[] allBattleAbilities = battle.teamOne.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			DeactivateAbility(serverAbilityHandler.abilityState);
		}
		if (!battle.IsComplete)
		{
			ApplyDamagePerRound(battle);
		}
		if (battle.IsComplete)
		{
			FinishBattle(battle);
		}
	}

	public static bool CanUnitAttack(ServerUnitState unit)
	{
		if (unit.CurrentRollType == DieFaceType.DirectDamage)
		{
			return true;
		}
		if (unit.CurrentRollType == DieFaceType.Initiative)
		{
			return true;
		}
		if (unit.CurrentRollType == DieFaceType.ArmourPiercing)
		{
			return true;
		}
		if (unit.CurrentRollType == DieFaceType.AcidStrike)
		{
			return true;
		}
		return false;
	}

	public static ServerUnitState UnitAttack(ServerUnitState unit, bool animateAttack = true)
	{
		if (!CanUnitAttack(unit))
		{
			return null;
		}
		ServerUnitState targetForTeam = GetTargetForTeam(unit.team, string.Empty);
		int damage = GetDamageForUnit(unit, targetForTeam);
		DamageType damageTypeForUnit = GetDamageTypeForUnit(unit);
		if (targetForTeam != null)
		{
			AbilityHooks.PreFireEvent(unit, targetForTeam);
			damage = ApplyDamage(targetForTeam, damage, unit.team, damageTypeForUnit);
			if (damageTypeForUnit == DamageType.Acid && unit.battle.animationHandler != null)
			{
				unit.battle.animationHandler.UnitAcidDamage(targetForTeam, 1);
			}
		}
		if (animateAttack && unit.battle.animationHandler != null)
		{
			unit.battle.animationHandler.UnitAttack(unit, targetForTeam, damage, damageTypeForUnit);
		}
		return targetForTeam;
	}

	public static ServerTeamState CalculateInitiativeWinner(ServerBattleState battle)
	{
		int initiativeForTeam = GetInitiativeForTeam(battle.hostTeam);
		int initiativeForTeam2 = GetInitiativeForTeam(battle.hostTeam.otherTeam);
		ServerTeamState serverTeamState = (battle.initiativeWinner = ((initiativeForTeam > initiativeForTeam2) ? battle.hostTeam : ((initiativeForTeam >= initiativeForTeam2) ? ((GetNextBattleRandom(battle, 0, 2) != 0) ? battle.hostTeam.otherTeam : battle.hostTeam) : battle.hostTeam.otherTeam)));
		serverTeamState.stats.initiativeWins++;
		if (battle.animationHandler != null)
		{
			battle.animationHandler.InitiativeResults(serverTeamState, initiativeForTeam, initiativeForTeam2);
		}
		return serverTeamState;
	}

	private static void _DepositEnergyIntoAbility(ServerAbilityState ability)
	{
		if (ability.metadata.Cost > 0)
		{
			ability.energy++;
			ability.team.energy--;
			ability.team.stats.energySpent++;
		}
	}

	public static void ActivateAbility(ServerAbilityState ability, ServerUnitState target)
	{
		_DepositEnergyIntoAbility(ability);
		ability.energy = 0;
		ability.target = target;
		ability.isActive = true;
		ability.lastActivationRound = ability.team.battle.currentRound;
		if (ability.battle.animationHandler != null)
		{
			ability.battle.animationHandler.PreActivateAbility(ability, target);
		}
		ability.handler.Activate(target);
		if (ability.battle.animationHandler != null)
		{
			ability.battle.animationHandler.ActivateAbility(ability, target);
		}
	}

	public static void InvestAbilityEnergy(ServerAbilityState ability)
	{
		_DepositEnergyIntoAbility(ability);
		if (ability.battle.animationHandler != null)
		{
			ability.battle.animationHandler.InvestAbilityEnergy(ability);
		}
	}

	public static void DeactivateAbility(ServerAbilityState ability)
	{
		if (ability.isActive)
		{
			ability.handler.Deactivate();
			if (ability.battle.animationHandler != null)
			{
				ability.battle.animationHandler.DeactivateAbility(ability);
			}
		}
		ability.isActive = false;
	}

	public static void FinishBattle(ServerBattleState battle)
	{
		if (battle.teamOne.forfeited && battle.teamTwo.forfeited)
		{
			battle.winningTeam = null;
		}
		else if (battle.teamOne.forfeited && !battle.teamTwo.forfeited)
		{
			battle.winningTeam = battle.teamTwo;
		}
		else if (!battle.teamOne.forfeited && battle.teamTwo.forfeited)
		{
			battle.winningTeam = battle.teamOne;
		}
		else if (!battle.teamOne.IsDead && battle.teamTwo.IsDead)
		{
			battle.winningTeam = battle.teamOne;
		}
		else if (battle.teamOne.IsDead && !battle.teamTwo.IsDead)
		{
			battle.winningTeam = battle.teamTwo;
		}
		else if (battle.teamOne.IsDead && battle.teamTwo.IsDead)
		{
			battle.winningTeam = battle.initiativeWinner;
		}
		ServerTeamState[] bothTeams = battle.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			serverTeamState.stats.isWinner = serverTeamState.IsBattleWinner;
			serverTeamState.stats.unitsSurvived = serverTeamState.aliveUnits.Length;
			if (serverTeamState.IsBattleWinner)
			{
				serverTeamState.stats.baseCoins += serverTeamState.divisionMetadata.BaseWinCoinReward;
			}
			else
			{
				serverTeamState.stats.baseCoins += serverTeamState.divisionMetadata.BaseLoseCoinReward;
			}
			serverTeamState.stats.pointsEarned = 0;
			serverTeamState.stats.coinsFromUnitsSurvived = 0;
			serverTeamState.stats.raidBossDamageDealt = 0;
			serverTeamState.stats.victoryPointsEarned = 0;
			serverTeamState.stats.eventPointsEarned = 0;
			serverTeamState.stats.coinsFromBoost = 0;
			int num = 0;
			ServerUnitState[] aliveUnits = serverTeamState.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				serverTeamState.stats.coinsFromUnitsSurvived += serverUnitState.metadata.SurviveCash;
				serverTeamState.stats.pointsEarned += serverUnitState.metadata.SurvivePoints;
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (ability.metadata.Type == "event_point_boost_passive")
					{
						num += ability.BoostValue;
					}
				}
			}
			ServerUnitState[] units = serverTeamState.otherTeam.units;
			foreach (ServerUnitState serverUnitState2 in units)
			{
				if (serverUnitState2.metadata.UnitType == UnitType.RAID_BOSS)
				{
					serverTeamState.stats.raidBossDamageDealt += serverUnitState2.totalDamageReceived;
				}
				if (serverUnitState2.IsDead)
				{
					serverTeamState.stats.victoryPointsEarned++;
					serverTeamState.stats.coinsFromUnitsDestroyed += serverUnitState2.metadata.DestroyCash;
					serverTeamState.stats.pointsEarned += serverUnitState2.metadata.DestroyPoints;
					serverTeamState.stats.eventPointsEarned += serverUnitState2.metadata.DestroyEventPoints + serverUnitState2.extraDestroyEventPoints;
					if (serverUnitState2.metadata.UnitType == UnitType.RAID_BOSS)
					{
						serverTeamState.stats.raidBossDamageDealt += serverUnitState2.metadata.DestroyEventPoints;
					}
				}
			}
			int num2 = serverTeamState.otherTeam.pvpRating;
			if (num2 < 1000 && serverTeamState.otherTeam.type == TeamType.Bot)
			{
				num2 = 1000;
			}
			float num3 = (float)Math.Max(1, battle.GetConstantInt("battle_victory_enemy_elo", 100)) / 100f;
			serverTeamState.stats.victoryPointsEarned *= (int)Math.Floor((float)num2 * num3);
			serverTeamState.stats.pointsEarned += ((!serverTeamState.IsBattleWinner) ? serverTeamState.divisionMetadata.LosePoint : serverTeamState.divisionMetadata.WinPoint);
			num3 = (float)Math.Max(1, battle.GetConstantInt("battle_victory_player_elo", 100)) / 100f;
			if (serverTeamState.IsBattleWinner)
			{
				serverTeamState.stats.eventPointsEarned += serverTeamState.divisionMetadata.WinEventPoint;
				serverTeamState.stats.victoryPointsEarned += (int)Math.Floor((float)serverTeamState.pvpRating * num3);
			}
			if (battle.IsPvPMatch)
			{
				serverTeamState.stats.eventPointsEarned += serverTeamState.divisionMetadata.EventPointPVPBonus;
			}
			serverTeamState.stats.giftDrops = new List<int>();
			if (serverTeamState.otherTeam.type == TeamType.RaidBoss)
			{
				int num4 = 0;
				EventRaidbossDamageDropRateDataModel[] rewardDropRate = serverTeamState.otherTeam.rewardDropRate;
				ServerUnitState[] units2 = serverTeamState.otherTeam.units;
				foreach (ServerUnitState serverUnitState3 in units2)
				{
					if (serverUnitState3.metadata.UnitType == UnitType.RAID_BOSS)
					{
						num4 += serverUnitState3.totalDamageReceived;
					}
				}
				if (num4 > 0 && rewardDropRate != null && rewardDropRate.Length > 0 && rewardDropRate.Length > 0)
				{
					EventRaidbossDamageDropRateDataModel eventRaidbossDamageDropRateDataModel = rewardDropRate[rewardDropRate.Length - 1];
					int num5 = num4 / eventRaidbossDamageDropRateDataModel.threshold;
					num4 -= num5 * eventRaidbossDamageDropRateDataModel.threshold;
					EventRaidbossDamageDropRateDataModel[] array = rewardDropRate;
					foreach (EventRaidbossDamageDropRateDataModel eventRaidbossDamageDropRateDataModel2 in array)
					{
						for (int n = 0; n < num5; n++)
						{
							serverTeamState.stats.giftDrops.Add(eventRaidbossDamageDropRateDataModel2.giftid);
						}
						if (num4 >= eventRaidbossDamageDropRateDataModel2.threshold)
						{
							serverTeamState.stats.giftDrops.Add(eventRaidbossDamageDropRateDataModel2.giftid);
						}
					}
				}
			}
			serverTeamState.stats.coinsFromPrizePool = (serverTeamState.IsBattleWinner ? battle.cashPrizePool : 0);
			ServerUnitState[] units3 = serverTeamState.otherTeam.units;
			foreach (ServerUnitState serverUnitState4 in units3)
			{
				if (serverUnitState4.droppedParts == null)
				{
					continue;
				}
				foreach (IPartMetadata droppedPart in serverUnitState4.droppedParts)
				{
					serverTeamState.stats.partsEarned.Add(droppedPart);
				}
			}
		}
		if (battle.animationHandler != null)
		{
			battle.animationHandler.FinishBattle(battle.winningTeam);
		}
	}

	private static void ForfeitBattle(ServerTeamState team)
	{
		team.forfeited = true;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState unit in aliveUnits)
		{
			ApplyDamage(unit, 999999, team.otherTeam, DamageType.Standard);
		}
		if (team.battle.animationHandler != null)
		{
			team.battle.animationHandler.TeamForfeit(team);
		}
	}

	private static void ReviveTeam(ServerTeamState team)
	{
		ServerUnitState[] units = team.units;
		foreach (ServerUnitState serverUnitState in units)
		{
			serverUnitState.hp = serverUnitState.metadata.StartingHealth;
			serverUnitState.extraDamage = 0;
			serverUnitState.damagePerRound = 0;
			serverUnitState.preventReroll = false;
			serverUnitState.roundsUntilRerollEnabled = 0;
			foreach (ServerAbilityState ability in serverUnitState.abilities)
			{
				ability.handler = null;
			}
		}
		team.stats.revivesUsed++;
		UpdateAliveUnits(team);
		SetupConvenienceData(team.battle);
	}

	public static void ApplyBattleAction(ServerTeamState team, BattleAction action)
	{
		if (action is InvestEnergyAction)
		{
			InvestEnergyAction investEnergyAction = action as InvestEnergyAction;
			InvestAbilityEnergy(team.GetAbilityByID(investEnergyAction.abilityID));
		}
		else if (action is UseAbilityAction)
		{
			UseAbilityAction useAbilityAction = action as UseAbilityAction;
			ActivateAbility(team.GetAbilityByID(useAbilityAction.abilityID), BattleAction.GetUnitByID(useAbilityAction.targetUnitID, team.battle));
		}
		else if (action is ForfeitAction)
		{
			ForfeitBattle(team);
		}
		else if (action is ReviveAction)
		{
			ReviveTeam(team);
		}
	}

	public static void ApplyDamagePerRound(ServerBattleState battle)
	{
		ServerTeamState[] bothTeams = battle.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			ServerUnitState[] aliveUnits = serverTeamState.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				if (serverUnitState.damagePerRound > 0)
				{
					ApplyDamage(serverUnitState, serverUnitState.damagePerRound, serverUnitState.team.otherTeam, DamageType.PerRound);
				}
				if (serverUnitState.acidPerRound > 0)
				{
					ApplyDamage(serverUnitState, serverUnitState.acidPerRound, serverUnitState.team.otherTeam, DamageType.PerRound);
				}
			}
		}
		if (battle.animationHandler != null)
		{
			battle.animationHandler.ApplyDamagePerRound();
		}
	}

	public static bool IsUnitTargetValid(ServerTeamState team, ServerUnitState target, ServerAbilityState abilityState)
	{
		bool flag = true;
		switch (abilityState.metadata.TargetSelectType)
		{
		case TargetType.EnemyUnit:
			flag = team != target.team;
			break;
		case TargetType.PlayerUnit:
			flag = team == target.team;
			break;
		}
		if (!flag)
		{
			return flag;
		}
		switch (abilityState.metadata.Type)
		{
		case "reroll":
			flag = !target.preventReroll;
			break;
		case "respin2":
			flag = !target.preventReroll;
			break;
		}
		return flag;
	}

	public static void DeactivateDeadUnitAbilities(ServerBattleState battle)
	{
		ServerTeamState[] bothTeams = battle.bothTeams;
		foreach (ServerTeamState serverTeamState in bothTeams)
		{
			ServerUnitState[] units = serverTeamState.units;
			foreach (ServerUnitState serverUnitState in units)
			{
				if (!serverUnitState.IsDead)
				{
					continue;
				}
				foreach (ServerAbilityState ability in serverUnitState.abilities)
				{
					if (!ability.handler.ActivateOnDeath())
					{
						DeactivateAbility(ability);
					}
				}
			}
		}
	}

	public static int GetNextBattleRandom(ServerBattleState battle, int from, int to)
	{
		if (from == to)
		{
			return from;
		}
		return from + (int)battle.randomProvider.Next((uint)(to - from));
	}

	public static int GetNextTeamRandom(ServerTeamState team, int from, int to, string reason = "")
	{
		if (from == to)
		{
			return from;
		}
		int result = from + (int)team.randomProvider.Next((uint)(to - from));
		if (team.stats != null)
		{
			string item = string.Format("{0} - \t {1}", team.randomProvider.GeneratedNumbers, reason);
			team.stats.generatedRandomNumbersData.Add(item);
		}
		return result;
	}

	public static void RerollUnit(ServerUnitState unit, bool evaluateRollRewards, bool isInitialRoll, bool canRepeat)
	{
		if (!isInitialRoll && unit.preventReroll)
		{
			return;
		}
		int currentRoll = unit.currentRoll;
		int nextTeamRandom = GetNextTeamRandom(unit.team, 0, unit.rollTypes.Length, "RerollUnit: " + unit.index);
		if (!canRepeat)
		{
			while (nextTeamRandom == currentRoll)
			{
				nextTeamRandom = GetNextTeamRandom(unit.team, 0, unit.rollTypes.Length, "RerollUnit-NoRepeat");
			}
		}
		SetUnitCurrentRoll(unit, nextTeamRandom, evaluateRollRewards);
	}

	private static void SetUnitCurrentRoll(ServerUnitState unit, int rollValue, bool evaluateRollRewards)
	{
		foreach (ServerAbilityState ability in unit.abilities)
		{
			if (ability.isActive)
			{
				DeactivateAbility(ability);
			}
		}
		int currentRoll = unit.currentRoll;
		unit.currentRoll = rollValue;
		if (unit.currentRoll == currentRoll)
		{
			unit.rollRepeats++;
		}
		else
		{
			unit.rollRepeats = 0;
		}
		foreach (ServerAbilityState ability2 in unit.abilities)
		{
			if (unit.CurrentRollType == DieFaceType.Special)
			{
				ActivateAbility(ability2, unit);
			}
		}
		if (evaluateRollRewards)
		{
			EvaluateRollRewards(unit.team, false);
		}
	}

	public static void RerollTeam(ServerTeamState team, bool evaluateRollRewards)
	{
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState unit in aliveUnits)
		{
			RerollUnit(unit, false, false, false);
		}
		if (evaluateRollRewards)
		{
			EvaluateRollRewards(team, false);
		}
	}

	public static int ApplyDamage(ServerUnitState unit, int damage, ServerTeamState damageSource, DamageType dmgType)
	{
		if (unit.IsDead)
		{
			return 0;
		}
		AbilityHooks.UnitReceivedDamage(unit, damage, dmgType);
		damage += unit.extraDamage;
		if (unit.evadeNextAOE && dmgType == DamageType.AOE)
		{
			damage = 0;
		}
		unit.totalDamageReceived += damage;
		int num = damage;
		int num2 = 0;
		if (dmgType == DamageType.ArmourPiercing)
		{
			unit.armor = 0;
		}
		else
		{
			num2 = Math.Min(unit.armor, num);
			unit.armor -= num2;
			num -= num2;
		}
		if (dmgType == DamageType.Acid)
		{
			unit.acidPerRound++;
		}
		unit.hp -= num;
		ServerTeamStatsState stats = damageSource.stats;
		stats.totalDamageDealt += num2;
		stats.totalDamageDealt += num + Math.Min(0, unit.hp);
		if (unit.IsDead)
		{
			HandleUnitDeath(unit, damageSource);
		}
		return damage;
	}

	public static int ApplyDamageInRange(ServerUnitState unit, int damageMin, int damageMax, ServerTeamState damageSource, DamageType dmgType)
	{
		int nextTeamRandom = GetNextTeamRandom(unit.team.otherTeam, damageMin, damageMax + 1, "ApplyDamageInRange");
		return ApplyDamage(unit, nextTeamRandom, damageSource, dmgType);
	}

	private static void HandleUnitDeath(ServerUnitState unit, ServerTeamState damageSource)
	{
		ServerBattleState battle = unit.battle;
		bool flag = false;
		if (unit.team == damageSource)
		{
			flag = true;
		}
		UpdateAliveUnits(unit.team);
		unit.destroyCoins = GetUnitDestroyCoinReward(unit, damageSource);
		if (flag)
		{
			unit.killIndex = 0;
		}
		else
		{
			unit.killIndex = unit.team.otherTeam.currentRoundKills;
			unit.team.otherTeam.currentRoundKills++;
			AbilityHooks.UnitDiedEvent(unit, damageSource);
		}
		unit.destroyCoins += unit.extraDestroyCoins;
		List<BoostDataModel> list = new List<BoostDataModel>();
		if (!flag)
		{
			for (int i = 0; i < damageSource.boosts.Length; i++)
			{
				list.Add(damageSource.boosts[i]);
			}
		}
		_GetDroppedParts(unit, list);
		ServerTeamStatsState stats = damageSource.stats;
		int num = _GetUnitDestroyGemReward(unit, damageSource, list);
		stats.unitsDestroyed++;
		stats.gemsEarned += num;
		if (unit.killIndex > 0)
		{
			float num2 = (float)battle.GetConstantInt("battle_multikill_multiplier", 100) / 100f;
			int num3 = (int)Math.Floor((float)(unit.destroyCoins * unit.killIndex) * num2);
			stats.coinsFromMultiKill += num3;
		}
		if (unit.hp == 0 && !flag)
		{
			int constantInt = battle.GetConstantInt("battle_perfectkill_reward", 10);
			stats.coinsFromPerfectKills += constantInt;
		}
		int constantInt2 = battle.GetConstantInt("battle_overkill_threshold", 7);
		if (unit.hp <= -constantInt2)
		{
			int constantInt3 = battle.GetConstantInt("battle_overkill_reward", 10);
			stats.coinsFromOverKills += constantInt3;
		}
	}

	public static void UpdateAliveUnits(ServerTeamState team)
	{
		List<ServerUnitState> list = new List<ServerUnitState>();
		ServerUnitState[] units = team.units;
		foreach (ServerUnitState serverUnitState in units)
		{
			if (!serverUnitState.IsDead)
			{
				list.Add(serverUnitState);
			}
		}
		team.aliveUnits = new ServerUnitState[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			team.aliveUnits[j] = list[j];
		}
	}

	private static int _GetUnitDestroyGemReward(ServerUnitState destroyedUnit, ServerTeamState damageSource, List<BoostDataModel> opponentBoost)
	{
		int num = 0;
		float num2 = 1f;
		foreach (BoostDataModel item in opponentBoost)
		{
			num2 *= item.GemsMultiplier;
		}
		if ((float)GetNextTeamRandom(damageSource, 0, 100, "GetUnitDestroyGemReward-Chance") <= (float)destroyedUnit.metadata.GemDropChance * num2)
		{
			int gemDropMin = destroyedUnit.metadata.GemDropMin;
			int gemDropMax = destroyedUnit.metadata.GemDropMax;
			num += GetNextTeamRandom(damageSource, gemDropMin, gemDropMax, "GetUnitDestroyGemReward-Amount");
		}
		destroyedUnit.gemsDropped = num;
		return num;
	}

	public static int GetUnitDestroyCoinReward(ServerUnitState destroyedUnit, ServerTeamState damageSource)
	{
		return destroyedUnit.metadata.DestroyCash;
	}

	private static void _GetDroppedParts(ServerUnitState destroyedUnit, List<BoostDataModel> opponentBoost)
	{
		if (destroyedUnit.metadata.PartDrops == null)
		{
			return;
		}
		float num = 1f;
		foreach (BoostDataModel item in opponentBoost)
		{
			num *= item.PartsMultiplier;
		}
		List<IPartMetadata> list = _GetSortedPartList(destroyedUnit.metadata.PartDrops);
		foreach (IPartMetadata item2 in list)
		{
			int num2 = item2.DropChance;
			if (destroyedUnit.extraDestroyPartDropChance > 0)
			{
				num2 = ((item2.PartType != 1) ? (num2 + destroyedUnit.extraDestroyPartDropChance) : 100);
			}
			num2 = (int)Math.Ceiling((float)num2 * num);
			if (GetNextTeamRandom(destroyedUnit.team, 0, 100, "GetDroppedParts-Chance-PartID: " + item2.ID) > num2)
			{
				continue;
			}
			int nextTeamRandom = GetNextTeamRandom(destroyedUnit.team, item2.DropMin, item2.DropMax + 1, "GetDroppedParts-Amount-PartID: " + item2.ID);
			for (int i = 0; i < nextTeamRandom; i++)
			{
				if (destroyedUnit.droppedParts == null)
				{
					destroyedUnit.droppedParts = new List<IPartMetadata>();
				}
				destroyedUnit.droppedParts.Add(item2);
			}
		}
	}

	private static List<IPartMetadata> _GetSortedPartList(IPartMetadata[] partsList)
	{
		List<IPartMetadata> list = new List<IPartMetadata>();
		for (int i = 0; i < partsList.Length; i++)
		{
			list.Add(partsList[i]);
		}
		for (int j = 1; j < list.Count; j++)
		{
			IPartMetadata partMetadata = list[j];
			int num = j;
			while (num > 0 && list[num - 1].DropChance > partMetadata.DropChance)
			{
				list[num] = list[num - 1];
				num--;
			}
			list[num] = partMetadata;
		}
		return list;
	}

	public static ServerUnitState GetRandomUnitOnTeam(ServerTeamState team, ServerTeamState seedTeam, string reason = "")
	{
		ServerUnitState result = null;
		if (!team.IsDead)
		{
			int nextTeamRandom = GetNextTeamRandom(seedTeam, 0, team.aliveUnits.Length, reason + "-GetRandomUnitOnTeam-OnOpponent?:" + (team != seedTeam));
			result = team.aliveUnits[nextTeamRandom];
		}
		return result;
	}

	public static ServerUnitState GetTargetForTeam(ServerTeamState team, string reason = "")
	{
		ServerUnitState serverUnitState = null;
		serverUnitState = GetRandomUnitOnTeam(team.otherTeam, team, reason + "GetTargetForTeam");
		return AbilityHooks.GetTargetForTeam(team, serverUnitState);
	}

	public static DieFaceType GetRollActionForUnit(ServerUnitState unit)
	{
		return unit.CurrentRollType;
	}

	public static int GetRollValueForUnit(ServerUnitState unit)
	{
		int currentRollValue = unit.CurrentRollValue;
		return AbilityHooks.GetRollValueForUnit(unit, currentRollValue);
	}

	public static int GetDamageForUnit(ServerUnitState unit, ServerUnitState target)
	{
		int rollValueForUnit = GetRollValueForUnit(unit);
		rollValueForUnit = AbilityHooks.GetDamageForUnitTarget(unit, rollValueForUnit);
		return ApplyDamageBoosts(unit, target, rollValueForUnit);
	}

	public static DamageType GetDamageTypeForUnit(ServerUnitState unit)
	{
		if (unit.CurrentRollType == DieFaceType.ArmourPiercing)
		{
			return DamageType.ArmourPiercing;
		}
		if (unit.CurrentRollType == DieFaceType.AcidStrike)
		{
			return DamageType.Acid;
		}
		return DamageType.Standard;
	}

	public static int GetInitiativeForTeam(ServerTeamState team)
	{
		int num = 0;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState unit in aliveUnits)
		{
			if (GetRollActionForUnit(unit) == DieFaceType.Initiative)
			{
				num += GetRollValueForUnit(unit);
			}
		}
		return num + team.intelBoost;
	}

	public static void EvaluateRollRewards(ServerTeamState team, bool roundStartRoll)
	{
		if (team.aliveUnits.Length < 4 || team.aliveUnits.Length != team.units.Length)
		{
			return;
		}
		bool flag = true;
		bool flag2 = true;
		ServerUnitState serverUnitState = null;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState2 in aliveUnits)
		{
			if (serverUnitState2.currentRoll != 0)
			{
				flag2 = false;
			}
			if (serverUnitState2.currentRoll != serverUnitState2.rollValues.Length - 1)
			{
				flag = false;
			}
			if (serverUnitState2.currentRoll == 0 && serverUnitState2.rollRepeats >= 3)
			{
				serverUnitState = serverUnitState2;
			}
		}
		if (serverUnitState != null)
		{
			SetUnitCurrentRoll(serverUnitState, serverUnitState.rollValues.Length - 1, false);
			if (team.battle.animationHandler != null)
			{
				team.battle.animationHandler.UnitWorstRolls(serverUnitState);
			}
			EvaluateRollRewards(team, false);
		}
		else if (flag)
		{
			team.stats.coinsFromBestRolls += team.battle.GetConstantInt("battle_bestroll_reward", 50);
			if (team.battle.animationHandler != null)
			{
				team.battle.animationHandler.TeamBestRoll(team);
			}
		}
		else if (flag2)
		{
			RerollTeam(team, false);
			if (team.battle.animationHandler != null)
			{
				team.battle.animationHandler.TeamWorstRoll(team);
			}
			EvaluateRollRewards(team, false);
		}
	}

	public static int ApplyDamageBoosts(ServerUnitState unit, ServerUnitState target, int baseDamage)
	{
		if (unit.team.boosts != null && unit.team.boosts.Length > 0 && target.team.type == TeamType.RaidBoss)
		{
			for (int i = 0; i < unit.team.boosts.Length; i++)
			{
				BoostDataModel boostDataModel = unit.team.boosts[i];
				BoostType type = boostDataModel.Type;
				if (type == BoostType.TicketRBDmgBoost_1 || type == BoostType.TicketRBDmgBoost_2)
				{
					int num = (int)Math.Floor((float)baseDamage * boostDataModel.Multiplier1);
					unit.team.stats.raidBossTicketBuffDamageTotal += num - baseDamage;
					baseDamage = num;
				}
			}
		}
		return baseDamage;
	}

	public static void InitAbilityUnitBoosts(ServerUnitState unit)
	{
		if (unit.team.boosts == null || unit.abilities.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < unit.team.boosts.Length; i++)
		{
			BoostDataModel boostDataModel = unit.team.boosts[i];
			if (boostDataModel.Type == BoostType.TicketRBDmgBoost_1 || boostDataModel.Type == BoostType.TicketRBDmgBoost_2)
			{
				string id = boostDataModel.id;
				string iD = unit.abilities[0].metadata.ID;
				if (CacheManager.boostAbilityMultipliers.ContainsKey(id) && CacheManager.boostAbilityMultipliers[id].ContainsKey(iD))
				{
					unit.abilities[0].boostMultiplier = boostDataModel.Multiplier2 * CacheManager.boostAbilityMultipliers[id][iD];
				}
			}
		}
	}
}
