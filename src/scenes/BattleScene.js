import Phaser from 'phaser';
import Tank from '../classes/Tank';
import { GAME_CONFIG } from '../config/GameData';
import BattleEngine from '../logic/BattleEngine';
import UIManager from '../ui/UIManager';

export default class BattleScene extends Phaser.Scene {
    constructor() {
        super('BattleScene');
        this.actionPoints = GAME_CONFIG.ACTION_POINTS_PER_TURN;
        this.victoryAchieved = false;
    }

    preload() {
        // Load our extracted data
        this.load.json('units', 'src/config/units.json');
        this.load.json('constants', 'src/config/constants.json');
    }

    create() {
        this.playerTeam = [];
        this.enemyTeam = [];
        this.isTurnProcessing = false;

        // Init UI
        this.ui = new UIManager(this);
        this.ui.updateAP(this.actionPoints, 3);

        this.createBattlefield();

        // Setup Teams with Real IDs from the DB
        const playerIds = [11001, 12001, 11004, 13001];
        const enemyIds = [11002, 13002, 12002, 11005];

        this.createTeam(true, GAME_CONFIG.POSITIONS.PLAYER_X, playerIds);
        this.createTeam(false, GAME_CONFIG.POSITIONS.ENEMY_X, enemyIds);

        // Start the first turn immediately
        this.startTacticsPhase();
    }

    handleFightButton() {
        if (this.victoryAchieved) {
            this.scene.restart();
            return;
        }
        void this.executeCombatPhase();
    }

    createBattlefield() {
        const w = this.scale.width;
        const h = this.scale.height;
        const cx = w / 2;
        const cy = h / 2;

    }

    createTeam(isPlayer, xPosition, unitIds) {
        const team = isPlayer ? this.playerTeam : this.enemyTeam;
        const baseY = GAME_CONFIG.POSITIONS.BASE_Y;
        const spacing = GAME_CONFIG.POSITIONS.SPACING;
        const stagger = GAME_CONFIG.POSITIONS.STAGGER_OFFSET;

        unitIds.forEach((id, index) => {
            // Quincunx Logic: Odd indexes are offset inwards
            let xOffset = 0;
            if (index % 2 !== 0) {
                xOffset = isPlayer ? stagger : -stagger;
            }

            const finalX = xPosition + xOffset;
            const finalY = baseY + index * spacing;

            const tank = new Tank(this, finalX, finalY, id, isPlayer);
            tank.setDepth(5);

            // Global scale up for visibility
            tank.setScale(1.2);

            team.push(tank);
        });
    }

    selectTank(tank) {
        if (!tank || !tank.isPlayer || tank.hp <= 0) return;

        // Can only boost during Tactics phase
        if (this.isTurnProcessing || this.victoryAchieved) return;

        if (tank.isBoosted) {
            this.showStatus('UNIT ALREADY OVERCLOCKED');
            return;
        }

        if (this.actionPoints <= 0) {
            this.showStatus('INSUFFICIENT ENERGY');
            return;
        }

                // Apply Boost Instantly
                tank.boost();
                this.actionPoints -= 1;
                
                this.showStatus(`${tank.unitData.name} OVERCLOCKED!`);
                this.ui.updateAP(this.actionPoints, 3);    }

    showStatus(message) {
        this.ui.updateStatus(message);
    }

    async startTacticsPhase() {
        this.isTurnProcessing = false;
        this.actionPoints = GAME_CONFIG.ACTION_POINTS_PER_TURN;

        this.ui.updatePhase('TACTICS');
        this.ui.updateAP(this.actionPoints, 3);
        this.ui.setFightButtonState('ENGAGE', true);
        this.showStatus('ROLLING DICE...');

        // Auto-Roll for everyone at start of phase
        this.playerTeam.concat(this.enemyTeam).forEach(tank => {
            if (tank.hp > 0) tank.roll();
            else tank.resetWheelDisplay();
        });

        await this.wait(500);
        this.showStatus('ASSIGN ENERGY THEN ENGAGE');
    }

    async executeCombatPhase() {
        if (this.isTurnProcessing || this.victoryAchieved) return;

        this.isTurnProcessing = true;
        this.ui.updatePhase('COMBAT');
        this.ui.setFightButtonState('EXECUTING...', false);
        this.showStatus('COMBAT SEQUENCE INITIATED');

        const initiativeOrder = BattleEngine.getInitiativeOrder(this.getAliveUnits());

        for (const attacker of initiativeOrder) {
            if (attacker.hp <= 0) continue;

            const targetTeam = attacker.isPlayer ? this.enemyTeam : this.playerTeam;
            const target = BattleEngine.pickRandomTarget(targetTeam);

            if (!target) break;

            await this.resolveAttack(attacker, target);

            const winner = this.checkVictory();
            if (winner) {
                await this.finishBattle(winner);
                return;
            }
        }

        await this.wait(500);
        // Loop back to Tactics Phase
        this.startTacticsPhase();
    }

    async resolveAttack(attacker, target) {
        // Use the wheel value (real data now!)
        // Basic mechanic: Damage = Wheel Value (can be more complex later)
        const damage = attacker.currentWheelValue;

        // Attack Animation
        await this.tweenPromise(attacker, {
            x: attacker.x + (attacker.isPlayer ? 50 : -50),
            duration: 100,
            yoyo: true,
            ease: 'Back.easeOut'
        });

        // Hit Effect
        this.cameras.main.shake(50, 0.005); // Tiny shake
        const destroyed = target.takeDamage(damage);

        if (destroyed) {
            this.spawnExplosion(target);
        } else {
            this.tweenPromise(target, { alpha: 0.5, duration: 50, yoyo: true, repeat: 3 });
        }

        await this.wait(400);
    }

    spawnExplosion(target) {
        const burst = this.add.circle(target.x, target.y, 10, 0xff4444, 1).setDepth(19);
        this.tweens.add({
            targets: burst,
            scale: 4,
            alpha: 0,
            duration: 500,
            onComplete: () => burst.destroy()
        });
    }

    checkVictory() {
        const winner = BattleEngine.checkVictory(this.playerTeam, this.enemyTeam);
        if (winner) {
            this.victoryAchieved = true;
            this.ui.showEndScreen(winner === 'player');
            this.ui.updatePhase('VICTORY');
            return winner;
        }
        return null;
    }

    async finishBattle(winner) {
        this.isTurnProcessing = false;
        this.ui.setFightButtonState('RESTART MISSION', true);
    }

    getAliveUnits() {
        return [...this.playerTeam, ...this.enemyTeam].filter((tank) => tank.hp > 0);
    }

    tweenPromise(target, config) {
        return new Promise((resolve) => {
            this.tweens.add({
                targets: target,
                ...config,
                onComplete: () => {
                    if (typeof config.onComplete === 'function') {
                        config.onComplete();
                    }
                    resolve();
                },
            });
        });
    }

    wait(duration) {
        return new Promise((resolve) => {
            this.time.delayedCall(duration, resolve);
        });
    }
}