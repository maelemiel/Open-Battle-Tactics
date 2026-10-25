import type { DieFace } from '../types';
import { DIE_FACE_TYPE } from '../types';

import type { Side } from './events';

export interface InitiativeContender {
  readonly side: Side;
  readonly unitId: number;
  readonly face: DieFace;
}

/**
 * Team initiative = SUM of current roll VALUES of ALIVE units whose face type
 * is Initiative (`BattleLogic.cs:GetInitiativeForTeam` 952-964), each value
 * passed through roll hooks (`GetRollValueForUnit`, none in MVP), PLUS the
 * team's `intelBoost` (ULTRA intel ability only; 0 in MVP).
 */
export function computeTeamInitiative(
  contenders: readonly InitiativeContender[],
  side: Side,
  intelBoost = 0,
): number {
  return (
    contenders
      .filter((c) => c.side === side && c.face.type === DIE_FACE_TYPE.INITIATIVE)
      .reduce((sum, c) => sum + c.face.value, 0) + intelBoost
  );
}

/**
 * Applies `BattleLogic.cs:CalculateInitiativeWinner` (277-288):
 * - host > guest -> host, host < guest -> guest
 * - TIE (including 0 vs 0) -> coin toss from the BATTLE RNG stream
 *   (`GetNextBattleRandom(battle, 0, 2)`): toss() 0 = host wins, 1 = guest wins.
 *
 * The coin toss callback is MANDATORY: unlike the v1 engine, there is no
 * player-favoring default. `toss` must draw from the battle stream in call
 * order (it is only invoked on an actual tie).
 */
export function decideInitiativeWinner(
  hostInitiative: number,
  guestInitiative: number,
  toss: () => 0 | 1,
  hostSide: Side = 'player',
): { winner: Side; tied: boolean; toss: 0 | 1 } {
  if (hostInitiative > guestInitiative) {
    return { winner: hostSide, tied: false, toss: 0 };
  }
  if (hostInitiative < guestInitiative) {
    return { winner: hostSide === 'player' ? 'enemy' : 'player', tied: false, toss: 0 };
  }
  const coin = toss();
  return {
    // C#: `GetNextBattleRandom(battle, 0, 2) != 0 ? otherTeam : hostTeam`.
    winner: coin === 0 ? hostSide : hostSide === 'player' ? 'enemy' : 'player',
    tied: true,
    toss: coin,
  };
}
