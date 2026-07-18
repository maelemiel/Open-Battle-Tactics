using System;
using System.Collections.Generic;

public class AbilityHandlerFactory
{
	private static Dictionary<string, Func<ServerAbilityHandler>> factoryFunctions;

	private static void Init()
	{
		factoryFunctions = new Dictionary<string, Func<ServerAbilityHandler>>();
		RegisterType("reroll", () => new ReRollAbility());
		RegisterType("target", () => new TargetingAbility());
		RegisterType("barrage", () => new BarrageAbility());
		RegisterType("drawfire", () => new DrawFireAbility());
		RegisterType("jammer", () => new JammerAbility());
		RegisterType("ion_strike", () => new IonStrikeAbility());
		RegisterType("small_ion_strike", () => new SmallIonStrikeAbility());
		RegisterType("self_destruct", () => new SelfDestructAbility());
		RegisterType("reroll_all", () => new ReRollAllAbility());
		RegisterType("firebomb", () => new FirebombAbility());
		RegisterType("cake_walk", () => new CakeWalkAbility());
		RegisterType("target_advance", () => new TargetingAdvanceAbility());
		RegisterType("em_pulse", () => new EMPulseAbility());
		RegisterType("respin2", () => new ReRoll2Ability());
		RegisterType("jammer2", () => new Jammer2Ability());
		RegisterType("douse", () => new ExtinguishAbility());
		RegisterType("intel", () => new IntelAbility());
		RegisterType("armour_buff", () => new IncreaseBaseArmourAbility());
		RegisterType("increase_base_attack", () => new IncreaseBaseAttackAbility());
		RegisterType("rail_gun", () => new RailGunAbility());
		RegisterType("aoe_damage", () => new AoeDamageAbility());
		RegisterType("hotshot", () => new HotShotAbility());
		RegisterType("napalm", () => new NapalmAbility());
		RegisterType("shortcircuit", () => new ShortCircuitAbility());
		RegisterType("first_strike_buff", () => new FirstStrikeBuffAbility());
		RegisterType("coin_increase", () => new CoinIncreaseAbility());
		RegisterType("parts_increase", () => new PartsIncreaseAbility());
		RegisterType("passive_evade", () => new EvadeAOEAbility());
		RegisterType("extinguish_unit", () => new ExtinguishUnitAbility());
		RegisterType("event_point_boost_passive", () => new EventPointBoostPassiveAbility());
		RegisterType("event_boss_damage_passive", () => new IncreaseRaidBossDamageAbility());
		RegisterType("boost_dice", () => new BoostDiceAbility());
		RegisterType("boost_special", () => new BoostSpecialAbility());
		RegisterType("boost_special_percentage", () => new BoostSpecialPercentageAbility());
		RegisterType("boost_dice_special", () => new BoostDiceAndSpecialAbility());
		RegisterType("minigun", () => new MinigunUnitAbility());
		RegisterType("quick_draw_passive", () => new QuickDrawAbility());
		RegisterType("unit_event_boost", () => new UnitEventBoostAbility());
	}

	private static void RegisterType(string abilityType, Func<ServerAbilityHandler> factoryFunc)
	{
		factoryFunctions[abilityType] = factoryFunc;
	}

	public static ServerAbilityHandler Create(string abilityType, ServerAbilityState abilityState)
	{
		if (factoryFunctions == null)
		{
			Init();
		}
		if (abilityType != null && factoryFunctions.ContainsKey(abilityType))
		{
			Func<ServerAbilityHandler> func = factoryFunctions[abilityType];
			ServerAbilityHandler serverAbilityHandler = func();
			serverAbilityHandler.Init(abilityState);
			return serverAbilityHandler;
		}
		return null;
	}
}
