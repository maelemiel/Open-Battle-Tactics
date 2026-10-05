import Phaser from 'phaser';

import { createBattle } from '../../engine/combat/engine';
import type { Battle } from '../../engine/combat/engine';
import type { BattleEvent, Side, UnitRolledEvent } from '../../engine/combat/events';
import { getUnitById } from '../../engine/data/loader';
import type { DieFace } from '../../engine/types';
import { UnitView, faceLabel } from '../UnitView';

const WIDTH = 960;
const HEIGHT = 540;
const PLAYER_UNIT_ID = 11001; // VOLT
const ENEMY_UNIT_ID = 11002; // LONGSHOT
const LEVEL = 1;
const FONT = 'monospace';

/**
 * Full MVP battle loop, animated from engine events:
 * ROLL (dice spin) -> RESOLVE (initiative flashes, damage, specials,
 * deaths, victory banner) -> RESTART (fresh random seed).
 */
export class BattleScene extends Phaser.Scene {
  private battle!: Battle;
  private views!: Record<Side, UnitView>;
  private faces!: Record<Side, readonly DieFace[]>;
  private roundText!: Phaser.GameObjects.Text;
  private statusText!: Phaser.GameObjects.Text;
  private rollButton!: Phaser.GameObjects.Container;
  private resolveButton!: Phaser.GameObjects.Container;
  private restartButton!: Phaser.GameObjects.Container;
  private busy = false;

  constructor() {
    super('battle');
  }

  create(): void {
    this.battle = createBattle({
      playerUnitId: PLAYER_UNIT_ID,
      playerLevel: LEVEL,
      enemyUnitId: ENEMY_UNIT_ID,
      enemyLevel: LEVEL,
      seed: Math.floor(Math.random() * 2 ** 31),
    });
    this.busy = false;

    this.add
      .text(WIDTH / 2, 14, 'OPEN BATTLE TACTICS - prototype', {
        fontFamily: FONT,
        fontSize: '20px',
        color: '#eeeeee',
        fontStyle: 'bold',
      })
      .setOrigin(0.5, 0);
    this.roundText = this.add
      .text(WIDTH / 2, 48, 'ROUND 0', {
        fontFamily: FONT,
        fontSize: '16px',
        color: '#9a9ab8',
      })
      .setOrigin(0.5);
    this.add
      .text(WIDTH / 2, HEIGHT / 2 - 10, 'VS', {
        fontFamily: FONT,
        fontSize: '34px',
        color: '#5a5a7a',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);

    const playerDef = getUnitById(PLAYER_UNIT_ID);
    const enemyDef = getUnitById(ENEMY_UNIT_ID);
    this.faces = {
      player: playerDef.stats.faces,
      enemy: enemyDef.stats.faces,
    };
    const maxHp = (side: Side): number =>
      this.battle.state.combatants.find((c) => c.side === side)!.maxHp;
    this.views = {
      player: new UnitView(this, {
        x: 225,
        y: 280,
        name: playerDef.name,
        level: LEVEL,
        maxHp: maxHp('player'),
        color: 0x3d6db5,
      }),
      enemy: new UnitView(this, {
        x: 735,
        y: 280,
        name: enemyDef.name,
        level: LEVEL,
        maxHp: maxHp('enemy'),
        color: 0xb5483d,
      }),
    };

    this.statusText = this.add
      .text(WIDTH / 2, 448, '', {
        fontFamily: FONT,
        fontSize: '13px',
        color: '#9a9ab8',
      })
      .setOrigin(0.5);

    this.rollButton = this.makeButton('ROLL', WIDTH / 2 - 95, 492, () =>
      void this.onRoll(),
    );
    this.resolveButton = this.makeButton('RESOLVE', WIDTH / 2 + 95, 492, () =>
      void this.onResolve(),
    );
    this.restartButton = this.makeButton('RESTART', WIDTH / 2, 492, () =>
      this.doRestart(),
    );
    this.restartButton.setVisible(false);
    this.add
      .text(
        WIDTH / 2,
        524,
        'SPACE: roll / resolve - R: restart after victory',
        { fontFamily: FONT, fontSize: '11px', color: '#6a6a88' },
      )
      .setOrigin(0.5);

    this.input.keyboard?.on('keydown-SPACE', () => this.onActionKey());
    this.input.keyboard?.on('keydown-R', () => this.doRestart());

    this.syncButtons();
  }

  // -------------------------------------------------------------------------
  // Actions (guarded by step + busy; engine throws never reach the user)
  // -------------------------------------------------------------------------

  private onActionKey(): void {
    const step = this.battle.state.step;
    if (step === 'awaiting-roll') void this.onRoll();
    else if (step === 'awaiting-resolve') void this.onResolve();
  }

  private async onRoll(): Promise<void> {
    if (this.busy || this.battle.isOver() || this.battle.state.step !== 'awaiting-roll') return;
    this.busy = true;
    this.syncButtons();
    let events: BattleEvent[];
    try {
      events = this.battle.rollPhase();
    } catch (err) {
      this.showEngineError('rollPhase', err);
      this.busy = false;
      this.syncButtons();
      return;
    }
    this.clearStatus();
    this.roundText.setText(`ROUND ${this.battle.state.round}`);
    await this.animateRolls(events);
    this.busy = false;
    this.syncButtons();
  }

  private async onResolve(): Promise<void> {
    if (this.busy || this.battle.isOver() || this.battle.state.step !== 'awaiting-resolve') return;
    this.busy = true;
    this.syncButtons();
    let events: BattleEvent[];
    try {
      events = this.battle.resolveRound();
    } catch (err) {
      this.showEngineError('resolveRound', err);
      this.busy = false;
      this.syncButtons();
      return;
    }
    await this.playEvents(events);
    this.busy = false;
    this.syncButtons();
  }

  private doRestart(): void {
    if (!this.battle.isOver()) return;
    this.scene.restart(); // re-runs create(): fresh battle + full visual reset
  }

  // -------------------------------------------------------------------------
  // Event -> animation plumbing
  // -------------------------------------------------------------------------

  private async animateRolls(events: readonly BattleEvent[]): Promise<void> {
    const rolls = events.filter((e): e is UnitRolledEvent => e.type === 'unit-rolled');
    // TODO: dice spin is a rough text cycle; replace with a real die widget.
    await Promise.all(rolls.map((roll) => this.spinFace(roll.side, roll.face)));
  }

  /** Cycles random face labels ~490ms, then settles on the rolled face. */
  private spinFace(side: Side, final: DieFace): Promise<void> {
    const view = this.views[side];
    const faces = this.faces[side];
    const ticks = 6;
    let tick = 0;
    return new Promise((resolve) => {
      this.time.addEvent({
        delay: 70,
        repeat: ticks,
        callback: () => {
          tick += 1;
          if (tick <= ticks) {
            view.setFace(faceLabel(faces[Phaser.Math.Between(0, faces.length - 1)]));
          } else {
            view.setFace(faceLabel(final));
            resolve();
          }
        },
      });
    });
  }

  /** Plays resolve-round events sequentially, with visible delays. */
  private async playEvents(events: readonly BattleEvent[]): Promise<void> {
    for (const event of events) {
      switch (event.type) {
        case 'unit-rolled':
          break; // already animated during the roll phase
        case 'initiative-determined':
          for (const ref of event.order) {
            await this.views[ref.side].flash(180);
          }
          break;
        case 'damage-dealt': {
          const target = this.views[event.target];
          target.floatLabel(`-${event.amount}`, '#ff6b6b');
          target.setHp(event.targetHpAfter);
          target.shake();
          await this.wait(550);
          break;
        }
        case 'special-triggered':
          await this.views[event.side].specialPop(); // stub, like the engine
          break;
        case 'unit-defeated':
          this.views[event.side].defeat();
          await this.wait(450);
          break;
        case 'battle-won':
          this.showBanner(event.winner);
          await this.wait(400);
          break;
      }
    }
  }

  // -------------------------------------------------------------------------
  // Widgets
  // -------------------------------------------------------------------------

  private makeButton(
    label: string,
    x: number,
    y: number,
    onClick: () => void,
  ): Phaser.GameObjects.Container {
    const root = this.add.container(x, y);
    const bg = this.add.rectangle(0, 0, 160, 44, 0x26264a).setStrokeStyle(2, 0xeeeeee);
    bg.setInteractive({ useHandCursor: true });
    bg.on('pointerdown', () => onClick());
    const text = this.add
      .text(0, 0, label, {
        fontFamily: FONT,
        fontSize: '18px',
        color: '#ffffff',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);
    root.add([bg, text]);
    return root;
  }

  /** ROLL/RESOLVE visible only for the current actionable step (never after win). */
  private syncButtons(): void {
    const step = this.battle.state.step;
    const actionable = !this.busy && !this.battle.isOver();
    this.rollButton.setVisible(actionable && step === 'awaiting-roll');
    this.resolveButton.setVisible(actionable && step === 'awaiting-resolve');
  }

  private showBanner(winner: Side): void {
    const label = winner === 'player' ? 'PLAYER WINS' : 'ENEMY WINS';
    const banner = this.add.container(WIDTH / 2, 190);
    const bg = this.add
      .rectangle(0, 0, 460, 90, 0x0f0f23, 0.92)
      .setStrokeStyle(3, 0xffe066);
    const text = this.add
      .text(0, 0, label, {
        fontFamily: FONT,
        fontSize: '34px',
        color: '#ffe066',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);
    banner.add([bg, text]);
    banner.setAlpha(0);
    banner.setDepth(20);
    this.tweens.add({ targets: banner, alpha: 1, duration: 250 });
    this.restartButton.setVisible(true);
  }

  private showEngineError(caller: string, err: unknown): void {
    const message = err instanceof Error ? err.message : String(err);
    this.statusText.setText(`engine error in ${caller}: ${message}`).setColor('#ff6b6b');
  }

  private clearStatus(): void {
    this.statusText.setText('').setColor('#9a9ab8');
  }

  private wait(ms: number): Promise<void> {
    return new Promise((resolve) => this.time.delayedCall(ms, resolve));
  }
}
