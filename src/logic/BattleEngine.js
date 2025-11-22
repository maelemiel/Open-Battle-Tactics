import { GAME_CONFIG } from '../config/GameData';

export default class BattleEngine {
    static calculateDamage(attacker, targetWheelValue) {
        // In the simplified model using real data: Damage = Wheel Value directly
        // (Real game logic might have modifiers, but this is safe for now)
        return attacker.currentWheelValue;
    }

    static getInitiativeOrder(units) {
        return units
            .map(unit => {
                // Simplified Initiative: Higher Roll goes first.
                // TODO: Re-implement 'Hover' logic (Lowest goes first) once we map Unit Types IDs.
                return { unit, effectiveRoll: unit.currentWheelValue };
            })
            .sort((a, b) => {
                // 0. DEBUG: Player Always Starts Priority
                if (GAME_CONFIG.PLAYER_ALWAYS_STARTS) {
                    if (a.unit.isPlayer && !b.unit.isPlayer) return -1; // a (player) comes first
                    if (!a.unit.isPlayer && b.unit.isPlayer) return 1;  // b (player) comes first
                }

                // 1. Higher roll goes first
                if (a.effectiveRoll !== b.effectiveRoll) {
                    return b.effectiveRoll - a.effectiveRoll;
                }
                // 2. Random tie-break
                return Math.random() - 0.5;
            })
            .map(entry => entry.unit);
    }

    static checkVictory(playerTeam, enemyTeam) {
        const playerAlive = playerTeam.some(t => t.hp > 0);
        const enemyAlive = enemyTeam.some(t => t.hp > 0);

        if (!playerAlive && !enemyAlive) return 'draw';
        if (playerAlive && !enemyAlive) return 'player';
        if (!playerAlive && enemyAlive) return 'enemy';
        return null;
    }

    static pickRandomTarget(targetTeam) {
        const aliveTargets = targetTeam.filter(t => t.hp > 0);
        if (aliveTargets.length === 0) return null;
        return aliveTargets[Math.floor(Math.random() * aliveTargets.length)];
    }
}