import Phaser from 'phaser';

import arenaUrl from '../assets/arena.png';
import { createBattle } from '../../engine/combat/engine';
import type { Battle } from '../../engine/combat/engine';
import type { BattleEvent, Side, UnitRolledEvent } from '../../engine/combat/events';
import { getUnitById } from '../../engine/data/loader';
import type { DieFace } from '../../engine/types';
import { UnitView, faceLabel } from '../UnitView';
import { HUD_KEYS, loadHudTextures } from '../hudAssets';
import { loadUnitTextures } from '../unitArt';

const WIDTH = 960;
const HEIGHT = 540;
const PLAYER_UNIT_ID = 11001; // VOLT
const ENEMY_UNIT_ID = 11002; // LONGSHOT
const LEVEL = 1;
const FONT = 'monospace';

// Real arena art is 1024x1024: rows 0..~330 are dead black, the grey
// concrete wall band runs ~340..460, the field starts ~464. Cover-fit
// horizontally and crop so the wall + field fill the screen.
const ARENA_SOURCE = 1024;
const ARENA_SKIP_Y = 330;

// 4 staggered (quincunx) slots per side along the arena diagonal, matching
// the original screenshot: 2 ranks, front rank lower toward the center road.
// Slot 1 of each side hosts the MVP duel unit; front slots sit ~390px apart
// (a full shell flight) so the duel reads left vs right, not nose to nose.
const PLAYER_SLOTS: readonly { x: number; y: number }[] = [
  { x: 285, y: 352 },
  { x: 222, y: 250 },
  { x: 146, y: 398 },
  { x: 104, y: 300 },
];
const ENEMY_SLOTS: readonly { x: number; y: number }[] = PLAYER_SLOTS.map((s) => ({
  x: WIDTH - s.x,
  y: s.y,
}));

// Bottom command panel.
const PANEL_H = 80;
const PANEL_Y = HEIGHT - PANEL_H / 2;
const ABILITY_SLOTS = 4;
const ABILITY_R = 22;
const ACTION_BTN_W = 200;
const ACTION_BTN_H = 56;

type ActionState = 'roll' | 'fight' | 'restart' | 'hidden';

/**
 * Full MVP battle loop, animated from engine events:
 * ROLL (dice spin) -> RESOLVE (initiative flashes, damage, specials,
 * deaths, victory banner) -> RESTART (fresh random seed).
 * Cartoon-military skin over the real arena art.
 */
export class BattleScene extends Phaser.Scene {
  private battle!: Battle;
  private views!: Record<Side, UnitView>;
  private faces!: Record<Side, readonly DieFace[]>;
  private roundText!: Phaser.GameObjects.Text;
  private statusText!: Phaser.GameObjects.Text;
  private actionButton!: Phaser.GameObjects.Container;
  private actionBg!: Phaser.GameObjects.Graphics;
  private actionLabel!: Phaser.GameObjects.Text;
  private actionState: ActionState = 'hidden';
  private pulseTween: Phaser.Tweens.Tween | null = null;
  private busy = false;
  /** Armor absorbed by a side this hit; shown when the shell actually lands. */
  private pendingAbsorb: Partial<Record<Side, number>> = {};

  constructor() {
    super('battle');
  }

  preload(): void {
    this.load.image('arena', arenaUrl);
    loadHudTextures(this);
    loadUnitTextures(this);
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
    this.pulseTween = null;

    this.buildArena();
    this.buildHud();

    const playerDef = getUnitById(PLAYER_UNIT_ID);
    const enemyDef = getUnitById(ENEMY_UNIT_ID);
    this.faces = {
      player: playerDef.stats.faces,
      enemy: enemyDef.stats.faces,
    };
    const maxHp = (side: Side): number =>
      this.battle.state.combatants.find((c) => c.side === side)!.maxHp;

    // Discreet empty-slot markers + the active duel units (front slots).
    this.drawEmptySlots(PLAYER_SLOTS.slice(1));
    this.drawEmptySlots(ENEMY_SLOTS.slice(1));
    this.views = {
      player: new UnitView(this, {
        ...PLAYER_SLOTS[0],
        unitId: PLAYER_UNIT_ID,
        name: playerDef.name,
        level: LEVEL,
        typeId: playerDef.typeId,
        maxHp: maxHp('player'),
        side: 'player',
      }),
      enemy: new UnitView(this, {
        ...ENEMY_SLOTS[0],
        unitId: ENEMY_UNIT_ID,
        name: enemyDef.name,
        level: LEVEL,
        typeId: enemyDef.typeId,
        maxHp: maxHp('enemy'),
        side: 'enemy',
      }),
    };

    this.buildCommandPanel();

    // Tanks roll in from their map edge with track dust; the action button
    // only appears once both are parked on their slots.
    this.busy = true;
    this.syncActionButton();
    void Promise.all([this.views.player.driveIn(-380), this.views.enemy.driveIn(380)]).then(() => {
      this.busy = false;
      this.syncActionButton();
    });

    this.input.keyboard?.on('keydown-SPACE', () => this.onActionKey());
    this.input.keyboard?.on('keydown-R', () => this.doRestart());

    this.syncActionButton();
  }

  // -------------------------------------------------------------------------
  // Static scenery / HUD / panel
  // -------------------------------------------------------------------------

  private buildArena(): void {
    const scale = WIDTH / ARENA_SOURCE; // cover: 960 wide from a 1024 square
    this.add
      .image(0, -ARENA_SKIP_Y * scale, 'arena')
      .setOrigin(0, 0)
      .setScale(scale)
      .setDepth(-10);
  }

  private buildHud(): void {
    this.add
      .text(WIDTH / 2, 10, 'OPEN BATTLE TACTICS', {
        fontFamily: FONT,
        fontSize: '16px',
        color: '#ffffff',
        fontStyle: 'bold',
        stroke: '#10131c',
        strokeThickness: 3,
      })
      .setOrigin(0.5, 0)
      .setDepth(9);
    this.roundText = this.add
      .text(WIDTH / 2, 34, 'ROUND 0', {
        fontFamily: FONT,
        fontSize: '13px',
        color: '#cfd6e4',
        fontStyle: 'bold',
        stroke: '#10131c',
        strokeThickness: 3,
      })
      .setOrigin(0.5)
      .setDepth(9);

    this.statusText = this.add
      .text(WIDTH / 2, HEIGHT - PANEL_H - 12, '', {
        fontFamily: FONT,
        fontSize: '13px',
        color: '#e8e4d4',
        stroke: '#10131c',
        strokeThickness: 3,
      })
      .setOrigin(0.5)
      .setDepth(9);
  }

  /** Faint slot circles where future squad units will stand. */
  private drawEmptySlots(slots: readonly { x: number; y: number }[]): void {
    const g = this.add.graphics().setDepth(1);
    for (const s of slots) {
      g.lineStyle(2, 0xffffff, 0.16);
      g.strokeCircle(s.x, s.y, 34);
      g.fillStyle(0xffffff, 0.05);
      g.fillCircle(s.x, s.y, 34);
    }
  }

  private buildCommandPanel(): void {
    const g = this.add.graphics().setDepth(8);
    g.fillStyle(0x12182a, 0.92);
    g.fillRect(0, HEIGHT - PANEL_H, WIDTH, PANEL_H);
    g.lineStyle(2, 0x2c3650, 1);
    g.lineBetween(0, HEIGHT - PANEL_H, WIDTH, HEIGHT - PANEL_H);

    // Greyed-out ability slots (decorative until abilities ship): real TK2D
    // ability icons in disabled state (dark tint + reduced alpha).
    const iconFrame = this.textures.getFrame(HUD_KEYS.abilities[0]);
    const iconScale = iconFrame ? (ABILITY_R * 1.4) / Math.max(iconFrame.width, iconFrame.height) : 1;
    for (let i = 0; i < ABILITY_SLOTS; i++) {
      const cx = 70 + i * 60;
      g.fillStyle(0x2c3548, 1);
      g.fillCircle(cx, PANEL_Y, ABILITY_R);
      g.lineStyle(2, 0x46536e, 1);
      g.strokeCircle(cx, PANEL_Y, ABILITY_R);
      g.fillStyle(0x1d2434, 1);
      g.fillCircle(cx, PANEL_Y, ABILITY_R - 6);
      // Decorative until abilities ship: real TK2D icon, disabled state.
      this.add
        .image(cx, PANEL_Y, HUD_KEYS.abilities[i])
        .setScale(iconScale)
        .setTint(0x59627a) // disabled: darkened + desaturated
        .setAlpha(0.85)
        .setDepth(9);
    }

    // Keyboard hint between slots and action button.
    this.add
      .text(400, PANEL_Y, 'SPACE: roll / fight - R: restart after victory', {
        fontFamily: FONT,
        fontSize: '11px',
        color: '#8f9bb3',
      })
      .setOrigin(0, 0.5)
      .setDepth(9);

    // Morphing action button: ROLL -> FIGHT -> (hidden) -> RESTART.
    this.actionBg = this.add.graphics();
    this.actionLabel = this.add
      .text(0, 0, '', {
        fontFamily: FONT,
        fontSize: '22px',
        color: '#3a2a00',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);
    this.actionButton = this.add.container(WIDTH - 130, PANEL_Y, [this.actionBg, this.actionLabel]);
    this.actionButton.setDepth(9);
    const hit = new Phaser.Geom.Rectangle(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H);
    this.actionButton.setInteractive(
      hit,
      Phaser.Geom.Rectangle.Contains,
    );
    this.actionButton.input!.cursor = 'pointer';
    this.actionButton.on('pointerdown', () => this.onActionClick());
  }

  // -------------------------------------------------------------------------
  // Actions (guarded by step + busy; engine throws never reach the user)
  // -------------------------------------------------------------------------

  private onActionKey(): void {
    const step = this.battle.state.step;
    if (step === 'awaiting-roll') void this.onRoll();
    else if (step === 'awaiting-resolve') void this.onResolve();
  }

  private onActionClick(): void {
    if (this.battle.isOver()) {
      this.doRestart();
      return;
    }
    this.onActionKey();
  }

  private async onRoll(): Promise<void> {
    if (this.busy || this.battle.isOver() || this.battle.state.step !== 'awaiting-roll') return;
    this.busy = true;
    this.syncActionButton();
    let events: BattleEvent[];
    try {
      events = this.battle.rollPhase();
    } catch (err) {
      this.showEngineError('rollPhase', err);
      this.busy = false;
      this.syncActionButton();
      return;
    }
    this.clearStatus();
    this.pendingAbsorb = {};
    this.roundText.setText(`ROUND ${this.battle.state.round}`);
    await this.animateRolls(events);
    this.busy = false;
    this.syncActionButton();
  }

  private async onResolve(): Promise<void> {
    if (this.busy || this.battle.isOver() || this.battle.state.step !== 'awaiting-resolve') return;
    this.busy = true;
    this.syncActionButton();
    let events: BattleEvent[];
    try {
      events = this.battle.resolveRound();
    } catch (err) {
      this.showEngineError('resolveRound', err);
      this.busy = false;
      this.syncActionButton();
      return;
    }
    await this.playEvents(events);
    this.busy = false;
    this.syncActionButton();
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
    await Promise.all(rolls.map((roll) => this.spinFace(roll.side, roll.face)));
    for (const roll of rolls) this.views[roll.side].pulseDieBadge();
    const rollSummary = rolls.map((r) => `${r.side} rolled ${faceLabel(r.face)}`).join(', ');
    this.announce(`Round ${this.battle.state.round}: ${rollSummary}`);
  }

  /** Cycles random die faces ~490ms, then settles on the rolled face. */
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
            view.setFace(faces[Phaser.Math.Between(0, faces.length - 1)]);
          } else {
            view.setFace(final);
            resolve();
          }
        },
      });
    });
  }

  /** Plays resolve-round events sequentially as one cause -> effect chain. */
  private async playEvents(events: readonly BattleEvent[]): Promise<void> {
    for (const event of events) {
      switch (event.type) {
        case 'unit-rolled':
          break; // already animated during the roll phase
        case 'initiative-cointoss':
          this.announce(`Initiative tie! Coin toss: ${event.winner} acts first.`);
          await this.coinPop(event.winner);
          break;
        case 'initiative-determined':
          this.announce(`Initiative: ${event.order[0]?.side} strikes first.`);
          await this.views[event.order[0]!.side].flash(400);
          break;
        case 'armor-absorbed':
          // Held back: the ripple + BLOCKED label play when the shell lands.
          this.pendingAbsorb[event.side] = event.absorbed;
          this.announce(`${event.side} armor will absorb ${event.absorbed}.`);
          break;
        case 'damage-dealt': {
          const shooter = this.views[event.source];
          const target = this.views[event.target];
          const absorbed = this.pendingAbsorb[event.target] ?? 0;
          this.pendingAbsorb[event.target] = 0;
          const hpLoss = Math.max(0, event.amount - absorbed);
          this.announce(
            `${event.source} hits ${event.target} for ${hpLoss} damage${absorbed ? ` (${absorbed} blocked)` : ''}. Remaining HP: ${event.targetHpAfter}.`,
          );
          shooter.kickback(); // firing recoil (UnitView.cs:1043)
          this.playMuzzleFlash(event.source);
          await this.fireShell(shooter.muzzleX, shooter.muzzleY, target.bodyX, target.bodyY);
          this.playImpact(event.target);
          this.cameras.main.shake(110, 0.004);
          target.hitFlash(0xffb0a0);
          if (absorbed > 0) {
            target.armorRipple();
            target.floatLabel(`-${absorbed} BLOCKED`, '#7fd4ff', true);
          }
          target.floatLabel(`-${hpLoss}`, '#ff6b6b');
          target.setHp(event.targetHpAfter);
          target.shake();
          await this.wait(560);
          break;
        }
        case 'acid-ticked':
        case 'dot-ticked': {
          const label = event.type === 'acid-ticked' ? 'ACID' : 'BURN';
          const view = this.views[event.side];
          this.announce(`${event.side} takes ${event.amount} ${label.toLowerCase()} damage. HP: ${event.targetHpAfter}.`);
          view.hitFlash(0x9fe07f);
          view.floatLabel(`-${event.amount} ${label}`, '#9fe07f');
          view.setHp(event.targetHpAfter);
          await this.wait(380);
          break;
        }
        case 'special-triggered':
          this.announce(`${event.side} triggered special ${event.specialId}.`);
          await this.views[event.side].specialPop(); // stub, like the engine
          break;
        case 'special-executed': {
          this.announce(
            `${event.side} executes ${event.handler}${event.implemented ? '' : ' (engine stub)'}.`,
          );
          await this.views[event.side].specialPop();
          if (event.implemented && event.damage != null) {
            const victim = this.views[event.side === 'player' ? 'enemy' : 'player'];
            victim.hitFlash(0xfff0a0);
            victim.floatLabel(`-${event.damage}`, '#ffe066');
            this.syncHpFromState();
          }
          await this.wait(300);
          break;
        }
        case 'unit-defeated':
          this.announce(`${event.side} unit defeated.`);
          this.views[event.side].defeat();
          this.playWreckSmoke(event.side);
          this.cameras.main.shake(180, 0.007);
          await this.wait(520);
          break;
        case 'battle-won':
          this.announce(`Battle ended: ${event.winner.toUpperCase()} WINS!`);
          this.showBanner(event.winner);
          await this.wait(400);
          break;
      }
    }
  }

  /** Resyncs both HP pills from engine state (paths without hpAfter fields). */
  private syncHpFromState(): void {
    for (const c of this.battle.state.combatants) {
      this.views[c.side].setHp(c.hp);
    }
  }

  /** Announces combat events to assistive technologies via aria-live DOM region. */
  private announce(message: string): void {
    if (typeof document !== 'undefined') {
      const el = document.getElementById('combat-announcer');
      if (el) el.textContent = message;
    }
  }

  // -------------------------------------------------------------------------
  // Real-art combat FX (TK2D cannon flashes, docs/HUD_ASSETS.md)
  // -------------------------------------------------------------------------

  /**
   * Muzzle flash at the shooter's barrel tip (~300ms): the horizontal flame
   * plume from the high-cannon fire atlas, popped and faded. Flipped so the
   * plume always points downrange.
   */
  private playMuzzleFlash(side: Side): void {
    const frame = this.textures.getFrame(HUD_KEYS.muzzleFlash);
    if (!frame) return;
    const view = this.views[side];
    const facing = side === 'player' ? 1 : -1;
    const scale = 110 / frame.width;
    const flash = this.add
      .image(view.muzzleX, view.muzzleY, HUD_KEYS.muzzleFlash)
      .setOrigin(0, 0.5) // plume grows from the barrel tip
      .setScale(scale * 0.9)
      .setDepth(8)
      .setFlipX(facing === -1); // always point downrange
    this.tweens.add({
      targets: flash,
      scaleX: scale * 1.15,
      scaleY: scale * 1.05,
      alpha: 0,
      duration: 300,
      ease: 'Quad.easeOut',
      onComplete: () => flash.destroy(),
    });
  }

  /** Spiky hit-impact burst on the target (~300ms pop + fade). */
  private playImpact(side: Side): void {
    const frame = this.textures.getFrame(HUD_KEYS.impactBurst);
    if (!frame) return;
    const view = this.views[side];
    const scale = 84 / frame.width;
    const burst = this.add
      .image(view.bodyX, view.bodyY, HUD_KEYS.impactBurst)
      .setScale(scale * 0.7)
      .setDepth(8)
      .setAlpha(0.95);
    this.tweens.add({
      targets: burst,
      scaleX: scale,
      scaleY: scale,
      alpha: 0,
      duration: 300,
      ease: 'Quad.easeOut',
      onComplete: () => burst.destroy(),
    });
  }

  /**
   * Glowing tracer shell from the shooter's muzzle to the target hull along a
   * shallow arc, dropping smoke puffs along the path. The visible
   * cause -> effect link between fire and impact.
   */
  private fireShell(x0: number, y0: number, x1: number, y1: number): Promise<void> {
    return new Promise((resolve) => {
      const shell = this.add.graphics().setDepth(8);
      const dist = Math.abs(x1 - x0);
      const duration = Phaser.Math.Clamp(dist * 0.9, 170, 340);
      const apex = -Math.min(46, dist * 0.16);
      let lastPuff = 0;
      this.tweens.addCounter({
        from: 0,
        to: 1,
        duration,
        onUpdate: (t) => {
          const k = t.getValue() ?? 0;
          const x = x0 + (x1 - x0) * k;
          const y = y0 + (y1 - y0) * k + 4 * apex * k * (1 - k);
          shell.clear();
          shell.fillStyle(0xffe9a0, 0.35);
          shell.fillCircle(x, y, 8);
          shell.fillStyle(0xffd24a, 1);
          shell.fillCircle(x, y, 3.5);
          if (k - lastPuff > 0.12) {
            lastPuff = k;
            this.smokePuff(x, y);
          }
        },
        onComplete: () => {
          shell.destroy();
          resolve();
        },
      });
    });
  }

  private smokePuff(x: number, y: number, color = 0xc9c2b4): void {
    const puff = this.add.graphics().setDepth(7);
    const r = Phaser.Math.Between(4, 7);
    this.tweens.addCounter({
      from: 0,
      to: 1,
      duration: 380,
      onUpdate: (t) => {
        const k = t.getValue() ?? 1;
        puff.clear();
        puff.fillStyle(color, 0.4 * (1 - k));
        puff.fillCircle(x, y - 10 * k, r + 6 * k);
      },
      onComplete: () => puff.destroy(),
    });
  }

  /** Dark smoke column rising off a wreck. */
  private playWreckSmoke(side: Side): void {
    const view = this.views[side];
    for (let i = 0; i < 5; i++) {
      this.time.delayedCall(i * 130, () =>
        this.smokePuff(view.bodyX + Phaser.Math.Between(-14, 14), view.bodyY, 0x4a4a52),
      );
    }
  }

  /** Center-screen coin-toss callout on initiative ties. */
  private coinPop(winner: Side): Promise<void> {
    return new Promise((resolve) => {
      const text = this.add
        .text(WIDTH / 2, 205, `COIN TOSS: ${winner.toUpperCase()} FIRST`, {
          fontFamily: FONT,
          fontSize: '24px',
          color: '#ffe066',
          fontStyle: 'bold',
          stroke: '#10131c',
          strokeThickness: 4,
        })
        .setOrigin(0.5)
        .setAlpha(0)
        .setScale(0.4)
        .setDepth(15);
      this.tweens.add({
        targets: text,
        alpha: 1,
        scale: 1,
        duration: 180,
        ease: 'Back.easeOut',
        onComplete: () => {
          this.tweens.add({
            targets: text,
            alpha: 0,
            delay: 420,
            duration: 220,
            onComplete: () => {
              text.destroy();
              resolve();
            },
          });
        },
      });
    });
  }

  // -------------------------------------------------------------------------
  // Action button states
  // -------------------------------------------------------------------------

  /** One morphing button: ROLL (amber) -> FIGHT (bright yellow + pulse) -> RESTART. */
  private syncActionButton(): void {
    const step = this.battle.state.step;
    const over = this.battle.isOver();
    const next: ActionState = this.busy
      ? 'hidden'
      : over
        ? 'restart'
        : step === 'awaiting-roll'
          ? 'roll'
          : step === 'awaiting-resolve'
            ? 'fight'
            : 'hidden';
    if (next === this.actionState) return;
    this.actionState = next;
    this.pulseTween?.stop();
    this.pulseTween = null;
    this.actionButton.setScale(1).setVisible(next !== 'hidden');

    const g = this.actionBg;
    g.clear();
    switch (next) {
      case 'roll':
        g.fillStyle(0xe0a83a, 1);
        g.fillRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        g.lineStyle(3, 0x8a6414, 1);
        g.strokeRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        this.actionLabel.setText('ROLL').setColor('#3a2a00');
        break;
      case 'fight':
        g.fillStyle(0xffd24a, 1);
        g.fillRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        g.lineStyle(3, 0x8a6414, 1);
        g.strokeRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        this.actionLabel.setText('FIGHT').setColor('#3a2a00');
        this.pulseTween = this.tweens.add({
          targets: this.actionButton,
          scale: 1.04,
          duration: 420,
          yoyo: true,
          repeat: -1,
          ease: 'Sine.easeInOut',
        });
        break;
      case 'restart':
        g.fillStyle(0x26304a, 1);
        g.fillRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        g.lineStyle(3, 0xffd24a, 1);
        g.strokeRoundedRect(-ACTION_BTN_W / 2, -ACTION_BTN_H / 2, ACTION_BTN_W, ACTION_BTN_H, 14);
        this.actionLabel.setText('RESTART').setColor('#ffffff');
        break;
      case 'hidden':
        break;
    }
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
  }

  private showEngineError(caller: string, err: unknown): void {
    const message = err instanceof Error ? err.message : String(err);
    this.statusText.setText(`engine error in ${caller}: ${message}`).setColor('#ff6b6b');
  }

  private clearStatus(): void {
    this.statusText.setText('').setColor('#e8e4d4');
  }

  private wait(ms: number): Promise<void> {
    return new Promise((resolve) => this.time.delayedCall(ms, resolve));
  }
}
