import type { DieFace } from '../types';
import { DIE_FACE_TYPE } from '../types';

import type { Side } from './events';

export interface InitiativeContender {
  readonly side: Side;
  readonly unitId: number;
  readonly face: DieFace;
}

/**
 * Calculates total initiative points for a side.
 * Verified from decompiled C# source (`BattleLogic.GetInitiativeForTeam`):
 * Sums the roll values of all units whose face type is Initiative (40000 / type 2).
 */
export function computeTeamInitiative(
  contenders: readonly InitiativeContender[],
  side: Side,
): number {
  return contenders
    .filter((c) => c.side === side && c.face.type === DIE_FACE_TYPE.INITIATIVE)
    .reduce((sum, c) => sum + c.face.value, 0);
}

/**
 * Computes initiative order according to official rules (`BattleLogic.CalculateInitiativeWinner`).
 *
 * 1. Compare total initiative points of each team (`GetInitiativeForTeam`).
 *    - If player > enemy -> player attacks first.
 *    - If enemy > player -> enemy attacks first.
 * 2. In case of tie (including both 0, which happens when neither side rolls INITIATIVE):
 *    - A 50/50 tie-break is performed (`GetNextBattleRandom(0, 2)`).
 *    - `tieBreaker` returns 0 for player or 1 for enemy. Defaults to player-first (0)
 *      when no RNG is provided.
 * 3. The winning team's units attack first, followed by the opposing team's units.
 */
export function computeInitiativeOrder(
  contenders: readonly InitiativeContender[],
  tieBreaker: () => number = () => 0,
): InitiativeContender[] {
  const playerInit = computeTeamInitiative(contenders, 'player');
  const enemyInit = computeTeamInitiative(contenders, 'enemy');

  let winningSide: Side;
  if (playerInit > enemyInit) {
    winningSide = 'player';
  } else if (enemyInit > playerInit) {
    winningSide = 'enemy';
  } else {
    // 50/50 random tie-break (0 = player, 1 = enemy)
    winningSide = tieBreaker() === 0 ? 'player' : 'enemy';
  }

  const winners = contenders.filter((c) => c.side === winningSide);
  const losers = contenders.filter((c) => c.side !== winningSide);

  return [...winners, ...losers];
}
