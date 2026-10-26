import type { DamageType, DieFace } from '../types';
import { DAMAGE_TYPE, DIE_FACE_TYPE } from '../types';
import { getUnitById } from '../data/loader';

import type { BattleEvent, Side } from './events';
import { computeTeamInitiative, decideInitiativeWinner } from './initiative';
import { applyDamage, canUnitAttack, damageTypeForFace, faceDamage } from './damage';
import type { DamageResult } from './damage';
import { resolveSpecial } from './specials';
import { createRngStreams, deriveSeeds } from './rng';
import type { BattleRngStreams } from './rng';
import type {
  Battle,
  BattleConfig,
  BattleState,
  BattleStep,
  Combatant,
  MutableCombatant,
  MutableTeamState,
  TeamState,
  UnitSetup,
} from './state';
import { spawn } from './state';

// Public API surface: import everything from './engine'.
export type {
  Battle,
  BattleConfig,
  BattleState,
  BattleStep,
  Combatant,
  UnitSetup,
} from './state';
export { statsForLevel } from './state';
export { MicrosoftRNG, deriveSeeds } from './rng';

const SIDES: readonly Side[] = ['player', 'enemy'];

export function createBattle(config: BattleConfig): Battle {
  if (config.seed === undefined && config.seeds === undefined) {
    throw new Error('createBattle: config.seed or config.seeds is required');
  }
  // Three independent RNG streams (COMBAT_SPEC.md §6). Explicit `seeds` (as
  // the server sends them) wins over the derived single-seed form.
  const seeds = config.seeds ?? deriveSeeds(config.seed!);
  const rng: BattleRngStreams = createRngStreams(seeds);
  // Host side for initiative ties: match JSON `playerIsHost`
  // (`BattleController.cs:212`). Player hosts by default in the MVP.
  const hostSide: Side = config.playerIsHost === false ? 'enemy' : 'player';

  const teams: Readonly<Record<Side, MutableTeamState>> = {
    player: makeTeam(
      'player',
      config.playerUnits ?? [{ unitId: config.playerUnitId, level: config.playerLevel }],
    ),
    enemy: makeTeam(
      'enemy',
      config.enemyUnits ?? [{ unitId: config.enemyUnitId, level: config.enemyLevel }],
    ),
  };
  // `bothTeams` order is [host, other] (`BattleLogic.cs:SetupConvenienceData`
  // 43-49): the host team rolls first in BeginRound.
  const bothTeams: readonly MutableTeamState[] = [
    teams[hostSide],
    teams[hostSide === 'player' ? 'enemy' : 'player'],
  ];

  let round = 0;
  let step: BattleStep = 'awaiting-roll';
  let winner: Side | null = null;
  let initiativeWinner: Side | null = null;
  let snapshot = publishState();

  function makeTeam(side: Side, roster: readonly UnitSetup[]): MutableTeamState {
    if (roster.length === 0) throw new Error(`createBattle: empty roster for ${side}`);
    return {
      side,
      energy: 0,
      intelBoost: 0, // Set only by the ULTRA `intel` ability (not in MVP).
      units: roster.map((u) => spawn(getUnitById(u.unitId), u.level, side)),
    };
  }

  function assertStep(expected: BattleStep, caller: string): void {
    if (winner !== null) throw new Error(`${caller}(): battle is over (winner=${winner})`);
    if (step !== expected) {
      throw new Error(`${caller}(): invalid step '${step}', expected '${expected}'`);
    }
  }

  const otherSide = (side: Side): Side => (side === 'player' ? 'enemy' : 'player');
  const teamRng = (side: Side) => (side === 'player' ? rng.player : rng.enemy);
  const aliveUnits = (side: Side): readonly MutableCombatant[] =>
    teams[side].units.filter((u) => u.alive);
  const isComplete = (): boolean =>
    aliveUnits('player').length === 0 || aliveUnits('enemy').length === 0;

  function viewOf(c: MutableCombatant): Combatant {
    return Object.freeze({
      side: c.side,
      unitId: c.unitId,
      name: c.name,
      maxHp: c.maxHp,
      hp: c.hp,
      armor: c.armor,
      alive: c.alive,
      currentFace: c.currentFace,
      faceIndex: c.faceIndex,
      acidPerRound: c.acidPerRound,
      damagePerRound: c.damagePerRound,
      roundsUntilRerollEnabled: c.roundsUntilRerollEnabled,
      rollLocked: c.preventReroll,
    });
  }

  /** Builds a fresh frozen snapshot; a new reference per mutation. */
  function publishState(): BattleState {
    const teamView = (team: MutableTeamState): TeamState =>
      Object.freeze({ side: team.side, energy: team.energy, intelBoost: team.intelBoost });
    return Object.freeze({
      round,
      step,
      winner,
      combatants: Object.freeze([...teams.player.units, ...teams.enemy.units].map(viewOf)),
      teams: Object.freeze({
        player: teamView(teams.player),
        enemy: teamView(teams.enemy),
      }),
    });
  }

  // -------------------------------------------------------------------------
  // Roll phase == BattleLogic.BeginRound (135-163)
  // -------------------------------------------------------------------------

  /**
   * `BattleLogic.cs:SetUnitCurrentRoll` (661-691): deactivate previous
   * abilities, track consecutive repeats, auto-activate the unit special when
   * the new face is Special (activation at ROLL time, not fire time).
   */
  function setUnitRoll(unit: MutableCombatant, index: number): void {
    unit.specialActive = false;
    unit.rollRepeats = unit.faceIndex === index ? unit.rollRepeats + 1 : 0;
    unit.faceIndex = index;
    unit.currentFace = unit.stats.faces[index]!;
    if (unit.currentFace.type === DIE_FACE_TYPE.SPECIAL && unit.stats.specialId !== 0) {
      unit.specialActive = true;
    }
  }

  /** `BattleLogic.cs:RerollUnit` (643-659). */
  function rerollUnit(unit: MutableCombatant, isInitialRoll: boolean, canRepeat: boolean): void {
    if (!isInitialRoll && unit.preventReroll) return; // reroll lock blocks non-initial rolls
    const faceCount = unit.stats.faces.length;
    let index = teamRng(unit.side).nextRange(0, faceCount);
    if (!canRepeat) {
      while (index === unit.faceIndex) {
        index = teamRng(unit.side).nextRange(0, faceCount);
      }
    }
    setUnitRoll(unit, index);
  }

  /**
   * `BattleLogic.cs:EvaluateRollRewards` (966-1017). Only fires for a full
   * alive team of 4 units. The stuck-on-worst-face forced upgrade can never
   * fire at round start in the original either (rollRepeats is reset to 0 by
   * BeginRound before the roll) — kept for fidelity with mid-round rerolls.
   * Best-roll coin reward has no coin model in the MVP (TODO: battle stats).
   */
  function evaluateRollRewards(team: MutableTeamState, events: BattleEvent[]): void {
    if (team.units.length !== 4 || aliveUnits(team.side).length !== 4) return;
    let allBest = true;
    let allWorst = true;
    let stuck: MutableCombatant | null = null;
    for (const unit of team.units) {
      if (unit.faceIndex !== 0) allWorst = false;
      if (unit.faceIndex !== unit.stats.faces.length - 1) allBest = false;
      if (unit.faceIndex === 0 && unit.rollRepeats >= 3) stuck = unit;
    }
    if (stuck) {
      setUnitRoll(stuck, stuck.stats.faces.length - 1);
      events.push(rollEvent(stuck));
      evaluateRollRewards(team, events);
    } else if (allBest) {
      // "Best Roll": +battle_bestroll_reward coins (default 50). TODO: coin stats.
    } else if (allWorst) {
      // "Bad Luck": full team reroll, no repeats.
      for (const unit of aliveUnits(team.side)) {
        rerollUnit(unit, false, false);
        events.push(rollEvent(unit));
      }
      evaluateRollRewards(team, events);
    }
  }

  function rollEvent(unit: MutableCombatant): BattleEvent {
    return {
      type: 'unit-rolled',
      round,
      side: unit.side,
      unitId: unit.unitId,
      faceIndex: unit.faceIndex,
      face: unit.currentFace!,
    };
  }

  function rollPhase(): BattleEvent[] {
    assertStep('awaiting-roll', 'rollPhase');
    round += 1; // C# increments after the loop; first for event labelling only.
    const events: BattleEvent[] = [];
    for (const team of bothTeams) {
      team.energy = 3; // `BattleLogic.cs:139`
      for (const unit of aliveUnits(team.side)) {
        unit.armor = 0; // armor never persists across rounds
        unit.rollRepeats = 0;
        unit.faceIndex = -1; // C# `currentRoll = -1`
        unit.currentFace = null;
        // Initial roll: isInitialRoll=true, so a reroll lock does NOT block it.
        rerollUnit(unit, true, true);
        // Lock bookkeeping (`BattleLogic.cs:150-154`): preventReroll latches
        // for this round, then the counter decrements. The lock only affects
        // non-initial rerolls (reroll/respin abilities); the die is NOT frozen.
        unit.preventReroll = unit.roundsUntilRerollEnabled > 0;
        if (unit.preventReroll) unit.roundsUntilRerollEnabled -= 1;
        events.push(rollEvent(unit));
      }
      evaluateRollRewards(team, events);
    }
    step = 'awaiting-resolve';
    snapshot = publishState();
    return events;
  }

  // -------------------------------------------------------------------------
  // Damage application (shared by attacks, specials and per-round ticks)
  // -------------------------------------------------------------------------

  interface Applied {
    readonly result: DamageResult;
    readonly target: MutableCombatant;
  }

  /**
   * Core of `BattleLogic.cs:ApplyDamage` (706-744). The pure math lives in
   * `damage.ts`; this owns mutation + death + events. Returns null when the
   * target is already dead (C# `if (unit.IsDead) return 0`).
   * TODO(combat-spec §4): UnitReceivedDamage hooks (passive_evade) and
   * `evadeNextAOE` (AOE only) hook in before the math — extension point.
   */
  function damageTarget(
    target: MutableCombatant,
    amount: number,
    damageType: DamageType,
    events: BattleEvent[],
    attacker?: { side: Side; unitId: number },
  ): Applied | null {
    if (target.hp <= 0) return null;
    const result = applyDamage(
      { hp: target.hp, armor: target.armor, extraDamage: 0, acidPerRound: target.acidPerRound },
      amount,
      damageType,
    );
    if (result.absorbed > 0) {
      events.push({
        type: 'armor-absorbed',
        round,
        side: target.side,
        unitId: target.unitId,
        absorbed: result.absorbed,
        armorRemaining: result.armor,
      });
    }
    target.hp = result.hp; // NOT clamped at 0: overkill stays negative
    target.armor = result.armor;
    target.acidPerRound = result.acidPerRound;
    if (attacker) {
      events.push({
        type: 'damage-dealt',
        round,
        source: attacker.side,
        sourceUnitId: attacker.unitId,
        target: target.side,
        targetUnitId: target.unitId,
        amount: result.total,
        targetHpAfter: target.hp,
        damageType,
      });
    }
    if (target.hp <= 0 && target.alive) {
      target.alive = false;
      target.specialActive = false;
      events.push({
        type: 'unit-defeated',
        round,
        side: target.side,
        unitId: target.unitId,
      });
    }
    return { result, target };
  }

  /**
   * `BattleLogic.cs:GetTargetForTeam`/`GetRandomUnitOnTeam` (903-919):
   * random ALIVE enemy, drawn from the ATTACKING team's RNG stream. The draw
   * advances the stream even when only one enemy is alive (max === 1).
   */
  function randomTarget(attackerSide: Side): MutableCombatant | null {
    const enemies = aliveUnits(otherSide(attackerSide));
    if (enemies.length === 0) return null;
    const index = teamRng(attackerSide).nextRange(0, enemies.length);
    return enemies[index]!;
  }

  // -------------------------------------------------------------------------
  // Resolve phase == BattleLogic.SimulateRound (165-229)
  // -------------------------------------------------------------------------

  function resolveRound(): BattleEvent[] {
    assertStep('awaiting-resolve', 'resolveRound');
    const events: BattleEvent[] = [];

    // 1. PreInitiativeEvent hooks — no team abilities in MVP (TODO hook point).
    // 2. CalculateInitiativeWinner (277-288).
    const contenders = [...aliveUnits('player'), ...aliveUnits('enemy')]
      .filter((u): u is MutableCombatant & { currentFace: DieFace } => u.currentFace !== null)
      .map((u) => ({ side: u.side, unitId: u.unitId, face: u.currentFace }));
    const hostInit = computeTeamInitiative(contenders, hostSide, teams[hostSide].intelBoost);
    const guest = otherSide(hostSide);
    const guestInit = computeTeamInitiative(contenders, guest, teams[guest].intelBoost);
    const decision = decideInitiativeWinner(hostInit, guestInit, () => {
      const toss = rng.battle.nextRange(0, 2);
      return (toss === 0 ? 0 : 1) as 0 | 1;
    }, hostSide);
    initiativeWinner = decision.winner;
    if (decision.tied) {
      events.push({
        type: 'initiative-cointoss',
        round,
        toss: decision.toss,
        winner: decision.winner,
      });
    }
    // 3. PostInitiativeEvent hooks — TODO hook point (first-strike etc.).
    const order = [...aliveUnits(decision.winner), ...aliveUnits(otherSide(decision.winner))];
    events.push({
      type: 'initiative-determined',
      round,
      order: order.map((u) => ({ side: u.side, unitId: u.unitId })),
    });

    // 4. Teams act: [initiativeWinner, other] (`SimulateRound` 172-176).
    for (const actingSide of [decision.winner, otherSide(decision.winner)]) {
      if (isComplete()) break;
      // TeamBeginAttackEvent hooks — barrages, ion strikes, jammers, armour
      // buffs fire here. TODO: team ability pipeline (combat-spec §8).
      const firing = aliveUnits(actingSide); // snapshot, like C# aliveUnits
      for (const unit of firing) {
        if (isComplete()) break;
        fireUnit(unit, events);
        // DeactivateDeadUnitAbilities (596-617): handled per death in
        // damageTarget (specialActive = false).
      }
      if (isComplete()) break;
    }

    // 5. EndOfRoundWithAbilitiesEvent — extinguish cleanses damagePerRound.
    //    TODO: no cleanse source in MVP.
    // 6. Deactivate ALL abilities both teams (`SimulateRound` 216-220).
    for (const side of SIDES) {
      for (const unit of teams[side].units) unit.specialActive = false;
    }

    // 7. ApplyDamagePerRound (544-566): damagePerRound then acidPerRound per
    //    alive unit, both as PerRound damage sourced from the other team.
    if (!isComplete()) {
      for (const team of bothTeams) {
        for (const unit of aliveUnits(team.side)) {
          // Snapshot like C#: a unit that dies from damagePerRound must NOT
          // also take its acidPerRound tick (`ApplyDamage` dead check).
          if (unit.damagePerRound > 0 && unit.hp > 0) {
            damageTarget(unit, unit.damagePerRound, DAMAGE_TYPE.PER_ROUND, events);
            if (unit.hp > 0) {
              events.push({
                type: 'dot-ticked',
                round,
                side: unit.side,
                unitId: unit.unitId,
                amount: unit.damagePerRound,
                damagePerRound: unit.damagePerRound,
                targetHpAfter: unit.hp,
              });
            }
          }
          if (unit.acidPerRound > 0 && unit.hp > 0) {
            damageTarget(unit, unit.acidPerRound, DAMAGE_TYPE.PER_ROUND, events);
            if (unit.hp > 0) {
              events.push({
                type: 'acid-ticked',
                round,
                side: unit.side,
                unitId: unit.unitId,
                amount: unit.acidPerRound,
                acidPerRound: unit.acidPerRound,
                targetHpAfter: unit.hp,
              });
            }
          }
        }
      }
    }

    // 8. FinishBattle when complete (`SimulateRound` 225-228).
    if (isComplete()) finishBattle(events);
    step = winner !== null ? 'over' : 'awaiting-roll';
    snapshot = publishState();
    return events;
  }

  /**
   * One unit's slot in the attack sequence: `AbilityHooks.UnitFiringEvent`
   * first (specials replace the attack), else `BattleLogic.cs:UnitAttack`
   * (252-275). Special and None faces never perform a standard attack
   * (`CanUnitAttack`).
   */
  function fireUnit(unit: MutableCombatant, events: BattleEvent[]): void {
    const face = unit.currentFace;
    if (!face) return;

    if (face.type === DIE_FACE_TYPE.SPECIAL) {
      // v1-compatible marker event; the UI pops the unit.
      events.push({
        type: 'special-triggered',
        round,
        side: unit.side,
        unitId: unit.unitId,
        specialId: unit.stats.specialId,
      });
      const resolution = resolveSpecial(unit);
      if (!resolution) return; // no special (specialId 0): the slot is spent
      if (resolution.implemented && resolution.handler === 'shortcircuit') {
        const target = randomTarget(unit.side);
        if (target) {
          // `ShortCircuitAbility.cs:UnitFiringEvent`: floor(mult * A) damage
          // (DamageType.Standard) to a random enemy, then reroll-lock it.
          damageTarget(target, resolution.damage, DAMAGE_TYPE.STANDARD, events, {
            side: unit.side,
            unitId: unit.unitId,
          });
          target.roundsUntilRerollEnabled += 1;
          target.preventReroll = true;
        }
        events.push({
          type: 'special-executed',
          round,
          side: unit.side,
          unitId: unit.unitId,
          specialId: unit.stats.specialId,
          handler: resolution.handler,
          implemented: resolution.implemented,
          damage: resolution.damage,
          targetUnitId: target?.unitId,
        });
        return;
      }
      // Documented stub (see specials.ts): effect NOT applied yet.
      events.push({
        type: 'special-executed',
        round,
        side: unit.side,
        unitId: unit.unitId,
        specialId: unit.stats.specialId,
        handler: resolution.handler,
        implemented: false,
      });
      return;
    }

    if (!canUnitAttack(face)) return; // None face: nothing.

    const target = randomTarget(unit.side);
    if (!target) return;
    // TODO(combat-spec §4): GetRollValueForUnit hooks (IncreaseBaseAttack,
    // FirstStrikeBuff, BoostDice) add to `faceDamage` here.
    const damage = faceDamage(face);
    if (damage <= 0) return;
    damageTarget(target, damage, damageTypeForFace(face), events, {
      side: unit.side,
      unitId: unit.unitId,
    });
  }

  /**
   * `BattleLogic.cs:FinishBattle` (340-365), minus forfeits (not modeled):
   * one side dead -> other wins; BOTH dead (per-round damage, AOE) ->
   * initiativeWinner wins. A true draw requires a double forfeit, which the
   * MVP cannot produce: the battle always ends with a winner.
   */
  function finishBattle(events: BattleEvent[]): void {
    const playerDead = aliveUnits('player').length === 0;
    const enemyDead = aliveUnits('enemy').length === 0;
    if (playerDead && enemyDead) winner = initiativeWinner ?? null;
    else if (playerDead) winner = 'enemy';
    else if (enemyDead) winner = 'player';
    if (winner !== null) {
      events.push({ type: 'battle-won', round, winner });
    }
  }

  return {
    get state(): BattleState {
      return snapshot;
    },
    rollPhase,
    resolveRound,
    isOver: (): boolean => winner !== null,
  };
}
