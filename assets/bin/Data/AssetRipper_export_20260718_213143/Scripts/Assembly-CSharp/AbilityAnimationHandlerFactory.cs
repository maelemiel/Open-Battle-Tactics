using System;
using System.Collections.Generic;

public class AbilityAnimationHandlerFactory
{
	private static Dictionary<string, Func<AbilityAnimationHandler>> factoryFunctions;

	private static void Init()
	{
		factoryFunctions = new Dictionary<string, Func<AbilityAnimationHandler>>();
		RegisterType("reroll", () => new ReRollAnimationHandler());
		RegisterType("target", () => new TargetingAnimationHandler());
		RegisterType("barrage", () => new BarrageAnimationHandler());
		RegisterType("drawfire", () => new DrawFireAnimationHandler());
		RegisterType("jammer", () => new JammerAnimationHandler());
		RegisterType("ion_strike", () => new IonStrikeAnimationHandler());
		RegisterType("small_ion_strike", () => new SmallIonStrikeAnimationHandler());
		RegisterType("self_destruct", () => new SelfDestructAnimationHandler());
		RegisterType("reroll_all", () => new ReRollAllAnimationHandler());
		RegisterType("firebomb", () => new FirebombAnimationHandler());
		RegisterType("cake_walk", () => new CakeWalkAnimationHandler());
		RegisterType("target_advance", () => new TargetingAdvanceAnimationHandler());
		RegisterType("em_pulse", () => new EMPulseAnimationHandler());
		RegisterType("respin2", () => new ReRoll2AnimationHandler());
		RegisterType("jammer2", () => new Jammer2AnimationHandler());
		RegisterType("douse", () => new ExtinguishAnimationHandler());
		RegisterType("intel", () => new IntelAnimationHandler());
		RegisterType("armour_buff", () => new IncreaseBaseArmourAnimationHandler());
		RegisterType("increase_base_attack", () => new IncreaseBaseAttackAnimationHandler());
		RegisterType("rail_gun", () => new RailGunAnimationHandler());
		RegisterType("aoe_damage", () => new AoeDamageAnimationHandler());
		RegisterType("hotshot", () => new HotShotAnimationHandler());
		RegisterType("napalm", () => new NapalmAnimationHandler());
		RegisterType("shortcircuit", () => new ShortCircuitAnimationHandler());
		RegisterType("first_strike_buff", () => new FirstStrikeBuffAnimationHandler());
		RegisterType("coin_increase", () => new CoinIncreaseAnimationHandler());
		RegisterType("parts_increase", () => new PartsIncreaseAnimationHandler());
		RegisterType("passive_evade", () => new EvadeAOEAnimationHandler());
		RegisterType("extinguish_unit", () => new ExtinguishUnitAnimationHandler());
		RegisterType("event_point_boost_passive", () => new EventPointBoostAnimationHandler());
		RegisterType("event_boss_damage_passive", () => new IncreaseRaidBossDamageAnimationHandler());
		RegisterType("boost_dice", () => new BoostDiceAnimationHandler());
		RegisterType("boost_special", () => new BoostSpecialAnimationHandler());
		RegisterType("boost_special_percentage", () => new BoostSpecialPercentageAnimationHandler());
		RegisterType("boost_dice_special", () => new BoostDiceAndSpecialAnimationHandler());
		RegisterType("minigun", () => new MinigunAnimationHandler());
		RegisterType("quick_draw_passive", () => new QuickDrawAnimationHandler());
		RegisterType("unit_event_boost", () => new UnitEventBoostAnimationHandler());
	}

	private static void RegisterType(string abilityType, Func<AbilityAnimationHandler> factoryFunc)
	{
		factoryFunctions[abilityType] = factoryFunc;
	}

	public static AbilityAnimationHandler Create(string abilityType, AbilityState abilityState, BattleController battleController)
	{
		if (factoryFunctions == null)
		{
			Init();
		}
		if (factoryFunctions.ContainsKey(abilityType))
		{
			AbilityAnimationHandler abilityAnimationHandler = factoryFunctions[abilityType]();
			abilityAnimationHandler.Init(battleController, abilityState);
			return abilityAnimationHandler;
		}
		return null;
	}
}
