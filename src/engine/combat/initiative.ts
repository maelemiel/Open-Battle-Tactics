import type { DieFace } from '../types';
import { DIE_FACE_TYPE } from '../types';

import type { Side } from './events';

export interface InitiativeContender {
  readonly side: Side;
  readonly unitId: number;
  readonly face: DieFace;
}

/**
 * MVP initiative rule (TODO: validate against real gameplay):
 * 1. Exactly one side rolled an INITIATIVE face -> it strikes first.
 * 2. Both or neither -> higher CURRENT face value first. This matches the old
 *    JS prototype (getInitiativeOrder sorted by currentWheelValue, higher
 *    first), which is the only available behavioral evidence.
 * 3. Perfect tie -> stable order, player side first (prototype used a random
 *    tie-break; deterministic player-first is chosen so same seed = same battle).
 *
 * Alternative rejected for MVP: accumulating INITIATIVE face values across
 * rounds into a pool and comparing pools. No prototype evidence of a pool;
 * revisit if gameplay footage shows accumulation.
 */
export function computeInitiativeOrder(
  contenders: readonly InitiativeContender[],
): InitiativeContender[] {
  return [...contenders].sort(compareContenders);
}

function rolledInitiative(contender: InitiativeContender): boolean {
  return contender.face.type === DIE_FACE_TYPE.INITIATIVE;
}

/** Sort comparator: descending priority (priority-first). */
function compareContenders(a: InitiativeContender, b: InitiativeContender): number {
  const aInit = rolledInitiative(a) ? 1 : 0;
  const bInit = rolledInitiative(b) ? 1 : 0;
  if (aInit !== bInit) return bInit - aInit;
  if (a.face.value !== b.face.value) return b.face.value - a.face.value;
  if (a.side !== b.side) return a.side === 'player' ? -1 : 1;
  return 0; // Array.prototype.sort is stable: keeps team order
}
