# COMBAT SPEC — Exact rules from decompiled source

Source of truth: `AssetRipper/` (ILSpy decompile of the 2015 APK, unobfuscated C#).
Every rule below cites `File.cs:method`. All file paths relative to `AssetRipper/`.
Core file: `BattleLogic.cs` (static, 1058 lines) — the entire battle simulation.

Architecture in one line: `BattleLogic` is a pure server-style simulator; Unity views only subscribe through `ServerBattleState.animationHandler` (null in headless sim) — all logic runs identically without it.

## 1. DieFaceType

- `DieFaceType.cs`: enum `None=0, DirectDamage=1, Initiative=2, Special=3, ArmourPiercing=4, AcidStrike=5`.
- DB id → enum: `UnitLevelProgressionDataModel.cs:70-104` (`faceTypeMap`):

| id | DieFaceType |
|---|---|
| 10000 | None |
| 20000 | DirectDamage |
| 30000 | Special |
| 40000 | Initiative |
| 50000 | Special |
| 60000 | Special |
| 70000 | ArmourPiercing |
| 80000 | AcidStrike |

- 30000/50000/60000 are ALL Special. 80000 = AcidStrike (exists in enum + map, not used as native face in `unit_level_progression` data).
- Dice are 5 faces, fixed order: `UnitLevelProgressionDataModel.cs:114-143` (`RollValues` = `face1Value..face5Value`, `RollTypes` via faceTypeMap). A roll selects a face INDEX (0-4), not a value.
- Partial upgrades (`unit_partial_level`) replace face values/types: `UnitMetadataAdapter.cs:61-116`. NOTE: its `faceNType` is the raw ENUM ordinal (0-5), cast directly — `UnitPartialLevelDataModel.cs:126-130` (`(DieFaceType)unitPartialLevel.face1Type`, 0 = keep base face) — NOT the 10000-scale ids.
- Display names: `BattleTypeExtensions.cs:GetDieFaceName`.
- Which faces attack: `BattleLogic.cs:CanUnitAttack` — DirectDamage, Initiative, ArmourPiercing, AcidStrike attack; Special and None do not (Special fires the unit's ability instead, see §5).

## 2. Initiative

`BattleLogic.cs:CalculateInitiativeWinner` (lines 277-288):

1. `GetInitiativeForTeam(hostTeam)` vs `GetInitiativeForTeam(hostTeam.otherTeam)`.
2. Team initiative = SUM of current roll VALUES of ALIVE units whose `CurrentRollType == Initiative` (`BattleLogic.cs:GetInitiativeForTeam`, 952-964), each value passed through ability hooks (`GetRollValueForUnit` — e.g. `FirstStrikeBuffAbility.cs` adds its boost), PLUS `team.intelBoost` (set by ULTRA `intel` ability: `IntelAbility.cs:Activate` → random value in `[BoostValue, SecondaryBoostValue]` via team RNG).
3. host > other → host. host < other → other. TIE (including 0 vs 0, no initiative faces anywhere) → coin toss: `GetNextBattleRandom(battle, 0, 2)`; result 0 → host wins, result 1 → host's other team wins. Uses the BATTLE RNG stream (not team RNG). Exactly 50/50, no player bias.
- Initiative faces DO attack normally that round (`CanUnitAttack` returns true for Initiative).
- Tie UI (coin toss animation): `InitiativePhase.cs:TieBreakerSequence`.
- RNG call: `BattleLogic.cs:GetNextBattleRandom` (619-626) → `battle.randomProvider.Next((uint)(to - from))`.

## 3. Round structure

Client phase enum `Phase.cs`: NONE, INTRO, FINDOPPONENT, ROUNDSTART, DECISION, WAIT_FOR_OPPONENT, INITIATIVE, RESOLUTION, OUTRO. Registered in `BattleController.cs:InitializePhaseManager` (273-283).

Real combat timing (logic, no ULTRA/PRE-COMBAT/REACTIVE phases exist in the sim):

- ROUNDSTART — `RoundStartPhase.cs:OnEnterPhase` calls `BattleLogic.cs:BeginRound` (135-163):
  - `currentRound++`, both teams: `energy = 3`, `currentRoundKills = 0`.
  - Per ALIVE unit: `armor = 0` (armor does NOT persist across rounds), `rollRepeats = 0`, `currentRoll = -1`, then initial roll `RerollUnit(..., isInitialRoll: true, canRepeat: true)` (dice rolled HERE, per unit, via the TEAM's own RNG — `BattleLogic.cs:RerollUnit` 643-659), `roundStartRoll = currentRoll`, `preventReroll = roundsUntilRerollEnabled > 0` (decrement if locked).
  - `EvaluateRollRewards(team, roundStartRoll: true)` per team (see below).
- DECISION — `DecisionPhase.cs`. Player (or bot AI: `SimpleAI.cs`) spends energy: `InvestEnergyAction` (1 energy into a team ability) / `UseAbilityAction` (activate), via `AbilityManager.cs` → `BattleLogic.cs:ApplyBattleAction` (522-542). Also `ForfeitAction` / `ReviveAction`.
- INITIATIVE — UI only (`InitiativePhase.cs`), displays faces + coin toss.
- RESOLUTION — `ResolutionPhase.cs:BattleSequence` calls `BattleLogic.cs:SimulateRound` (165-229). Exact order:
  1. `AbilityHooks.PreInitiativeEvent(battle)` — all handlers in ExecutionOrder (`AbilityHooks.cs`).
  2. `CalculateInitiativeWinner(battle)`.
  3. `AbilityHooks.PostInitiativeEvent(battle, initiativeWinner)`.
  4. Teams act in order `[initiativeWinner, initiativeWinner.otherTeam]` (`SimulateRound` 172-176). Per team:
     - `AbilityHooks.TeamBeginAttackEvent(team)` — barrages, ion strikes, jammers, self-destruct, EMPulse, cake_walk, extinguish, armour_buff fire here.
     - Per ALIVE unit in team order: `AbilityHooks.UnitFiringEvent(unit)` — if ANY handler returns true, the unit's normal attack is REPLACED (specials like shortcircuit/napalm/railgun/minigun/quickdraw fire here); otherwise `BattleLogic.cs:UnitAttack` (252-275). Break early if `battle.IsComplete`. After each unit: `DeactivateDeadUnitAbilities`.
     - `TeamEndAttack` (animation only).
  5. `AbilityHooks.EndOfRoundWithAbilitiesEvent` — extinguish_unit passive cleanses `damagePerRound`.
  6. Deactivate ALL abilities both teams (`SimulateRound` 216-220).
  7. `ApplyDamagePerRound` (544-566): every alive unit takes `damagePerRound` AND `acidPerRound` as `DamageType.PerRound` (from the other team as source). Both values persist across rounds until cleansed/stacked further.
  8. `IsComplete` → `FinishBattle`. Else back to ROUNDSTART.
- `UnitAttack` (252-275): target = `GetTargetForTeam` = RANDOM alive enemy chosen with the ATTACKING team's RNG (`GetRandomUnitOnTeam` 903-912), then targeting hooks may override (`TargetingAbility.cs`, `DrawFireAbility.cs`, `TargetingAdvanceAbility.cs`). Damage pipeline then `ApplyDamage`.
- Roll-reward rules (only for a full alive team of 4): `BattleLogic.cs:EvaluateRollRewards` (966-1017):
  - Unit stuck on face index 0 with `rollRepeats >= 3` consecutive same rolls → force-upgraded to LAST face index (`rollValues.Length - 1`).
  - All 4 units on last face → "Best Roll" coins (`battle_bestroll_reward` constant, default 50).
  - All 4 units on face 0 → "Bad Luck" full team reroll, no-repeat (`RerollTeam`).

## 4. Damage

Damage pipeline (`BattleLogic.cs:UnitAttack` + `GetDamageForUnit` 932-937):

1. `GetRollValueForUnit` = current face VALUE + hooks (`AbilityHooks.cs:GetRollValueForUnit`): `IncreaseBaseAttackAbility.cs` adds A to DirectDamage faces (+ ArmourPiercing if constant `battle_add_damage_boost_to_ap` = 1, default 1), `FirstStrikeBuffAbility.cs` adds A to Initiative faces, `BoostDiceAbility.cs`/`BoostDiceAndSpecialAbility.cs` add A to a face type, `IncreaseRaidBossDamageAbility.cs` vs raid boss.
2. `GetDamageForUnitTarget` hooks (currently none modify — hook exists at `ServerAbilityHandler.cs:GetDamageForUnitTarget`).
3. `ApplyDamageBoosts` (1019-1036): ONLY if target team is RaidBoss and attacker has ticket boosts → `floor(damage * Multiplier1)`.
4. `PreFireEvent` hooks (raid-boss damage tracking).

`ApplyDamage(unit, damage, source, dmgType)` (`BattleLogic.cs:706-744`), exact order:

- unit dead → return 0.
- `UnitReceivedDamage` hooks first (passive_evade rolls here).
- `damage += unit.extraDamage` — TARGET-side amplification debuff (applied by `TargetingAdvanceAbility.cs:Activate` `target.extraDamage += BoostValue`). Every hit on that unit is increased.
- `evadeNextAOE && dmgType == AOE` → damage = 0 (set by `EvadeAOEAbility.cs` on a previous AOE hit: team RNG `0..100 <= BoostValue`).
- `dmgType == ArmourPiercing` → `unit.armor = 0` and NO absorption: full damage to HP.
- else `absorbed = min(unit.armor, damage)`; `unit.armor -= absorbed`; `damage -= absorbed`.
- `dmgType == Acid` → `unit.acidPerRound++` (permanent stacking DoT, +1 per acid hit, applied end of every round, never decays; only `ExtinguishAbility`/`ExtinguishUnitAbility` cleanse `damagePerRound`, NOT acid).
- `unit.hp -= damage` — hp goes NEGATIVE, no clamp (`IsDead => hp <= 0`). Overkill = `hp <= -battle_overkill_threshold` (default 7).
- Death → `HandleUnitDeath` (752-803): killIndex/multikill coins (`battle_multikill_multiplier` 100), perfect kill (`hp == 0`) +10 coins, overkill +10 coins, gem drop chance `GetNextTeamRandom(damageSource, 0, 100) <= GemDropChance * gemsMultiplier`, part drops with `PartsMultiplier`.
- Returns the pre-absorption damage value (used for UI).

DamageType per face: `GetDamageTypeForUnit` (939-950): ArmourPiercing face → `DamageType.ArmourPiercing`, AcidStrike face → `DamageType.Acid`, everything else → `Standard`. `DamageType.cs` enum: Standard, AOE, PerRound, ArmourPiercing, Acid.

Armor: base 0 each round (`BeginRound`), granted by `DrawFireAbility.cs:Activate` (`target.armor += BoostValue`) and `IncreaseBaseArmourAbility.cs:TeamBeginAttackEvent` (all alive allies `+= floor(mult * A)` when the OTHER team begins attacking). There is NO defense stat anywhere — armor is a temp absorb pool only.

AcidStrike face: standard attack for face value + `acidPerRound++` on target (see `UnitAttack` 265-268 animation call + `ApplyDamage`).

## 5. SPECIAL (unit specials)

- `unit_special` table → `UnitSpecialDataModel.cs`: `Type` resolved via `handlerId` → `unit_special_handler.handler` string; `execution_order`; `IsUnitAbility = true`, `Cost = 0`, `TargetSelectType = None`. `related_action_id` exists in data but is NOT read by battle code.
- Handler registry: `AbilityHandlerFactory.cs:Init` — 38 type strings → handler classes. `UnitSpecialHandlerDataModel.cs` maps ids (601 increase_base_attack, 602 aoe_damage, 603 rail_gun, 604 armour_buff, 605 first_strike_buff, 606 hotshot, 607 napalm, 608 shortcircuit, 609 passive_evade, 610 minigun, 611 extinguish_unit, 612 event_point_boost_passive, 613 event_boss_damage_passive, 614 boost_dice, 615 boost_special, 616 boost_dice_special, 617 boost_special_percentage, 618 quick_draw_passive, 619 unit_event_boost).
- Wiring: `UnitMetadataAdapter.cs:SetData` (221-234): unit abilities = [special_id row (or partial special1), passive_id row (or partial special2)] with `special_boost_value_a/b` (+ partial/event boosts), then ALWAYS appends unit_special `60908` (handler 619 = `UnitEventBoostAbility`, an EMPTY no-op class) with 0 values (313-315).
- Activation: rolling a Special face auto-activates the unit's abilities at roll time — `BattleLogic.cs:SetUnitCurrentRoll` (661-691): deactivate previous, set roll, count repeats, `if (unit.CurrentRollType == DieFaceType.Special) ActivateAbility(ability, unit)` for each. Cost 0 → no energy.
- The special then FIRES at its hook, replacing the normal attack (`UnitFiringEvent` returns true):
  - SHORT CIRCUIT 60800 (VOLT L6 `110010206`, `special_boost_value_a = 11`): `ShortCircuitAbility.cs:UnitFiringEvent` — when its unit would fire: damage `floor(boostMultiplier * 11)` to a RANDOM enemy (`GetTargetForTeam`), then `target.roundsUntilRerollEnabled++` and `target.preventReroll = true` (enemy unit's die is locked next round). Returns true → replaces attack.
  - NAPALM 60700: `NapalmAbility.cs` — AOE `floor(mult * A)` + `damagePerRound += B` to whole enemy team. Replaces attack.
  - RAIL GUN 60300: `RailGunAbility.cs` — alternates deploy/fire rounds; fire = `floor(mult * A)` to random enemy. Replaces attack.
  - HOTSHOT 60600: `HotShotAbility.cs` — `floor(mult * A)` to random enemy + `target.damagePerRound += B`. Replaces attack.
  - MULTI-STRIKE 60200 (aoe_damage): `AoeDamageAbility.cs` — AOE `floor(mult * A)` to enemy team. Replaces attack.
  - MINIGUN 60301: `MinigunUnitAbility.cs` — `floor(mult * A)` + escalating `extraShots` of B each activation. Replaces attack.
  - ATTACK BUFF 60100 (increase_base_attack): `IncreaseBaseAttackAbility.cs:GetRollValueForUnit` — passive +A on DirectDamage/ArmourPiercing rolls while the face stays active.
  - ARMOR BUFF 60400: `IncreaseBaseArmourAbility.cs:TeamBeginAttackEvent` — when enemy team begins attack, all allies gain `floor(mult * A)` armor.
  - FIRST STRIKE BUFF 60500: `FirstStrikeBuffAbility.cs` — +A on Initiative rolls.
  - EVADE 60900: `EvadeAOEAbility.cs:UnitReceivedDamageEvent` — on AOE hit, team RNG `0..100 <= A` → next AOE evaded.
  - QUICK DRAW 60907: `QuickDrawAbility.cs:UnitFiringEvent` — RNG chance to attack B times with rerolls between.
  - EXTINGUISH 61100: `ExtinguishUnitAbility.cs:EndOfRoundWithAbilities` — cleanse `damagePerRound` (not acid) from all allies.
- Boost values are `special_boost_value_a/b` from `unit_level_progression` + partial/event additions (`UnitMetadataAdapter.cs:225-233`, `ServerAbilityState.cs:BoostValue` = base + `addedBoostA`).
- Cooldowns: `unit_cooldown` table = upgrade/research timers in SECONDS (`unit_cooldown.json`), NOT combat. Combat "cooldown" = `roundsUntilRerollEnabled` (die reroll lock from EMPulse/ShortCircuit), decremented in `BeginRound` (`BattleLogic.cs:150-154`).

## 6. Rolls & RNG

- THREE independent RNG streams per battle: one battle RNG (initiative coin toss only) + one per team (dice rolls, damage ranges, drop rolls, AI decisions). `BattleSetupUtils.cs:InitTeamRandom/InitBattleRandom` (96-106) → `MicrosoftRNG`.
- `MicrosoftRNG.cs`: MSVC rand() LCG — `seed = seed * 214013 + 2531011; return (uint)(seed / 65536 % max)`. Counter tracked (`GeneratedNumbers`).
- `MersenneTwister.cs` exists but is NOT used by battle setup.
- Seeds come from the SERVER in match JSON: `MatchParser.cs:31` `match_random_seed` → `battleSeed`; `MatchParser.cs:103` per-team `random_seed` in opponent containers. `BattleController.cs:BeginBattle` (205-223) wires all three. Client simulates deterministically from server state; `BattleSyncChecker.cs` re-verifies view vs logic state at each ROUNDSTART.
- Every team-RNG draw is logged with a reason string: `BattleLogic.cs:GetNextTeamRandom` (628-641) → `stats.generatedRandomNumbersData` (replay/anti-cheat).
- Dice roll = uniform face index: `RerollUnit` → `GetNextTeamRandom(unit.team, 0, unit.rollTypes.Length, ...)`.
- Simulator version tag: `BattleSimulatorVersion.cs` = 4 (serialized into battle output: `BattleOutputUtils.cs:23`).
- Tutorial battles hardcode seed 328372: `TutorialMatchHandler.cs:15`.

## 7. Victory / defeat / draw

- Battle over: `ServerBattleState.cs:IsComplete` — `teamOne.IsDead || teamTwo.IsDead || either forfeited`. `IsDead` = `aliveUnits.Length == 0` (`ServerTeamState.cs`).
- `BattleLogic.cs:FinishBattle` (340-365):
  - both forfeited → `winningTeam = null` (only true draw).
  - one forfeited → other wins.
  - teamOne dead, teamTwo alive → teamTwo; inverse → teamOne.
  - BOTH dead simultaneously (per-round damage, AOE, etc.) → `initiativeWinner` wins. No draw from mutual destruction.
- Forfeit: `ForfeitBattle` (488-500) — 999999 damage to own units.
- Revive (single-player): `ReviveTeam` (502-520) — full HP restore, rerun.

## 8. Abilities (34 team abilities, `ability` table)

- `AbilityDataModel.cs`: `Type` = `ability_handler.handler` via `handler_id`; `Cost` = `action_point` (energy); `TargetSelectType` = `(TargetType)target_group`; `LimitOnePerRound`; `ExecutionOrder`; `BoostValue/SecondaryBoostValue` = `action_boost_value_a/b`; `IsUnitAbility = false`.
- Equipped per team (`OpponentData.abilities` → `ServerTeamState.abilities`), max 3 visible in cube bar.
- Energy: 3 per round per team (`BeginRound`). Spend in DECISION phase: `AbilityManager.cs:DepositOneEnergy` (player) or `SimpleAI.cs` (bots, chance-gated by team RNG `BaseAI.cs:RandomChance`).
- Activation: `BattleLogic.cs:ActivateAbility` (300-316): `Cost > 0` → move 1 team energy in, reset ability energy, `isActive = true`, `lastActivationRound`, `handler.Activate(target)`. Gating: `ServerAbilityState.cs:CanActivate` — `team.energy >= 1 && !(LimitOnePerRound && lastActivationRound == currentRound)`.
- Target validation: `BattleLogic.cs:IsUnitTargetValid` (568-594): `TargetType.EnemyUnit` → opposite team; `PlayerUnit` → own team; `reroll`/`respin2` → target must not be `preventReroll`.
- Execution order: `BattleLogic.cs:_CreateOrderedAbilityArray` (105-118) + `_SortAbilityArray` (120-133, stable insertion sort): handler list = [own team abilities + own units' abilities, then OTHER team's same], sorted by `metadata.ExecutionOrder`. EVERY hook in `AbilityHooks.cs` iterates this single array.
- IMPORTANT: `ability_type` (1 ULTRA, 2 PRE-COMBAT, 3 REACTIVE, 4 PASSIVE) is NOT read by battle logic — only a UI check (`AbilityCell.cs:177` `abilityDataModel.abilityType != 4`). Combat timing is decided ONLY by which handler event the class overrides. Data: 6 ULTRA, 24 PRE-COMBAT, 4 REACTIVE, 0 PASSIVE.
- Handler behaviors (team-level, same classes as §5): reroll (`ReRollAbility.cs`, can-repeat), respin2 (`ReRoll2Ability.cs`, no-repeat), reroll_all (`ReRollAllAbility.cs`), target (`TargetingAbility.cs`, force-fire at chosen unit, countered by active jammer `ApplyCountermeasure`), target_advance (force-fire + victim takes `extraDamage += A`), drawfire (`DrawFireAbility.cs`, absorb: `target.armor += A` + enemy forced to fire at it), jammer/jammer2 (`JammerAbility.cs:TeamBeginAttackEvent`, disables abilities at phase start; jammer2 also reassigns ion strike/EMPulse targets via `JammedEvent`), barrage (`BarrageAbility.cs`, AOE random damage A..B per enemy via `ApplyDamageInRange`), ion_strike / small_ion_strike (range damage A..B to chosen target), self_destruct (999 self-damage + AOE A to all enemies), firebomb (AOE A + B per-round to enemies), em_pulse (`EMPulseAbility.cs`, A damage + reroll lock), cake_walk (+A coins to prize pool), douse/extinguish (reduce a unit's `damagePerRound` by A), intel (team `intelBoost` = random A..B).

## 9. Boosts (`boost`, `boost_ability_multiplier`)

- `ServerTeamState.boosts` = `BoostDataModel[]` parsed from opponent data ids (`BattleSetupUtils.cs:CreateTeamState`).
- Combat effects (all in `BattleLogic.cs`):
  - Damage vs raid boss only: `ApplyDamageBoosts` (1019-1036) — `TicketRBDmgBoost_1/2` → `floor(baseDamage * Multiplier1)` when target team is RaidBoss.
  - Special multiplier: `InitAbilityUnitBoosts` (1038-1057) — unit's FIRST ability gets `boostMultiplier = Multiplier2 * CacheManager.boostAbilityMultipliers[boostId][abilityId]`; every special handler computes `floor(boostMultiplier * BoostValue)`.
  - Gem drops: `_GetUnitDestroyGemReward` (823-839) — chance `* GemsMultiplier`.
  - Part drops: `_GetDroppedParts` (846-880) — chance `* PartsMultiplier`.
- `boost_ability_multiplier` table = per-boost per-ability-special multiplier map (`CacheManager.boostAbilityMultipliers`, consumed only at `InitAbilityUnitBoosts`).
- `BoostDataModel.cs` fields: multiplier1/multiplier2, cashMultiplier, gemMultiplier, partsMultiplier, durationMinutes.

## Fixes needed in our TS engine

Current engine: `src/engine/combat/engine.ts`, `initiative.ts`, `damage.ts`, `rng.ts`.

1. WRONG tie-break default: `initiative.ts:computeInitiativeOrder` defaults `tieBreaker = () => 0` (player wins ties when no RNG given). Real rule: tie ALWAYS coin-tossed from the battle RNG (`BattleLogic.cs:281`). Tie-break must be mandatory RNG, never a player-favoring default.
2. WRONG RNG stream layout: `engine.ts` uses ONE mulberry32 for everything; real game = 3 streams (battle RNG for coin toss only + 1 per team for all rolls/drops/AI) with `MicrosoftRNG` LCG (`seed*214013+2531011`, `(seed/65536)%max`). Byte-exact replay of original seeds impossible without MicrosoftRNG and per-team streams. Roll ORDER also differs: real rolls per team sequentially per unit at BeginRound (`bothTeams` = [host, other] order, `BattleLogic.cs:142-156`).
3. WRONG target selection: `engine.ts:resolveAttacks` picks the first alive opponent deterministically; real = RANDOM alive enemy via attacker's team RNG, overridable by targeting abilities (`BattleLogic.cs:GetTargetForTeam/GetRandomUnitOnTeam`).
4. MISSING round-end damage phase: `damagePerRound` + `acidPerRound` applied after abilities deactivate (`BattleLogic.cs:ApplyDamagePerRound`). AcidStrike currently deals only its face value; real game adds a PERMANENT +1/round stacking DoT per acid hit (`BattleLogic.cs:731-733`). No cleanse for acid exists in the original.
5. MISSING `extraDamage` in damage math: real `ApplyDamage` adds the TARGET's `extraDamage` to every hit (`BattleLogic.cs:713`). Our `damage.ts` has no such term (target_advance not implemented — fine to defer, but the formula must reserve it).
6. PARTIAL armour piercing: our `applyDamage` zeroes armor + full damage (matches), but real `ApplyDamage` zeroes armor only on the hit's TARGET and does NOT touch other units; also hp is NOT clamped at 0 in the original (overkill = `hp <= -7` stat, perfect kill `hp == 0`). Our `Math.max(0, ...)` clamp erases that distinction.
7. MISSING SPECIAL execution: `engine.ts` logs `special-triggered` and skips. Real: rolling Special auto-activates ALL unit specials at roll time (`SetUnitCurrentRoll`), they fire at `UnitFiringEvent` and REPLACE the attack (ShortCircuit/Napalm/RailGun/HotShot/AoeDamage/Minigun/QuickDraw) or act as passive roll modifiers (AttackBuff/FirstStrikeBuff/BoostDice). Route `unit_special` handlers per §5.
8. WRONG phase model: `state.ts:ROUND_PHASE_ORDER` (FIRST_STRIKE, ULTRA, PRE_COMBAT, ATTACK, REACTIVE) does not exist in the original. Real resolution = hook pipeline: PreInitiative → CalculateInitiative → PostInitiative → per-team [TeamBeginAttack hooks → per-unit UnitFiring-hook-or-UnitAttack] → EndOfRound hooks → deactivate all → per-round damage → win check (`BattleLogic.cs:SimulateRound`).
9. WRONG draw handling: `engine.ts:checkVictory` says "draw impossible". Real: both teams wiped in the same round → `initiativeWinner` wins (`BattleLogic.cs:362-364`); draw ONLY on double forfeit.
10. MISSING team structure: engine is 1v1 (`config.playerUnitId`); real = up to 4 units/team + team energy (3/round) + ULTRA ability activation in a DECISION step. Needed before abilities make sense.
11. MISSING roll rewards: bad-luck (all face-0 → team reroll), best-roll (+50 coins), stuck-on-worst 3 rounds → forced best face (`BattleLogic.cs:EvaluateRollRewards`).
12. MISSING ability hook pipeline + ExecutionOrder sorting (`BattleLogic.cs:_CreateOrderedAbilityArray`, `AbilityHooks.cs`) — the backbone for all 34 team abilities and 20 unit specials.
13. MINOR: `armor = 0` per round already matches (`engine.ts:rollPhase`); keep. Initiative faces attacking already matches (`damage.ts:faceDamage`). Face-type ids in `types.ts` (10000..80000) map correctly but consider enum ordinals for partial-level overrides (`UnitPartialLevelDataModel.cs:126-130`).
