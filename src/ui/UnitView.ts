import Phaser from 'phaser';

import { DIE_FACE_TYPE } from '../engine/types';
import type { DieFace, UnitTypeId } from '../engine/types';

const FONT = 'monospace';

// ---------------------------------------------------------------------------
// Tank silhouette geometry (authored facing RIGHT, ~92x60, origin at hull
// center; enemy tanks mirror via container scaleX -1).
// ---------------------------------------------------------------------------

interface TankPalette {
  readonly base: number;
  readonly dark: number;
  readonly light: number;
}

const PLAYER_PALETTE: TankPalette = { base: 0x3d6db5, dark: 0x2a4c86, light: 0x6f9de2 };
const ENEMY_PALETTE: TankPalette = { base: 0xa8503a, dark: 0x7a3526, light: 0xcc7a5e };
const DEAD_PALETTE: TankPalette = { base: 0x4a4f57, dark: 0x34383f, light: 0x61666e };

const METAL_DARK = 0x1a1e22;
const TRACK = 0x262b30;
const WHEEL = 0x454c55;

// Header (above the tank, never mirrored).
const HP_PILL_W = 64;
const HP_PILL_H = 14;
const DIE_R = 10.5;
const TYPE_R = 9;
const HEADER_Y = -66;

/** Long label for announcer text, e.g. "DMG 6", "INIT 3". */
export function faceLabel(face: DieFace): string {
  switch (face.type) {
    case DIE_FACE_TYPE.DAMAGE:
      return `DMG ${face.value}`;
    case DIE_FACE_TYPE.INITIATIVE:
      return `INIT ${face.value}`;
    case DIE_FACE_TYPE.SPECIAL:
      return 'SPEC';
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
      return `AP ${face.value}`;
    case DIE_FACE_TYPE.ACID_STRIKE:
      return `ACID ${face.value}`;
    case DIE_FACE_TYPE.NONE:
      return 'NONE';
  }
}

/** Compact die-badge label: "6", "I3", "SP", "AP", "AC". */
export function faceBadgeLabel(face: DieFace): string {
  switch (face.type) {
    case DIE_FACE_TYPE.DAMAGE:
      return `${face.value}`;
    case DIE_FACE_TYPE.INITIATIVE:
      return `I${face.value}`;
    case DIE_FACE_TYPE.SPECIAL:
      return 'SP';
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
      return 'AP';
    case DIE_FACE_TYPE.ACID_STRIKE:
      return 'AC';
    case DIE_FACE_TYPE.NONE:
      return '-';
  }
}

/** Unit-type glyph for the little header icon (Assault/Command/Operative/Heli). */
export function typeGlyph(typeId: UnitTypeId): string {
  switch (typeId) {
    case 2:
    case 6:
    case 12:
      return '\u25C9'; // command
    case 3:
    case 7:
    case 13:
      return '\u271A'; // operative
    case 4:
    case 8:
    case 10:
      return '~'; // helicopter
    default:
      return '\u25B2'; // assault (also bosses / fallback)
  }
}

export interface UnitViewConfig {
  readonly x: number;
  readonly y: number;
  readonly name: string;
  readonly level: number;
  readonly typeId: UnitTypeId;
  readonly maxHp: number;
  /** 'player' = faces right (blue), 'enemy' = faces left (red-brown). */
  readonly side: 'player' | 'enemy';
}

/**
 * Cartoon tank unit: Graphics silhouette (hull, turret, barrel, tracks) +
 * header with HP pill, red die badge and unit-type icon. Everything is
 * vector-drawn (real sprites come in a later phase); animations are driven
 * by the owning scene.
 */
export class UnitView {
  private readonly scene: Phaser.Scene;
  private readonly container: Phaser.GameObjects.Container;
  /** Tank art only: mirrored for the enemy, kickback tweens live here. */
  private readonly tank: Phaser.GameObjects.Container;
  private readonly tankArt: Phaser.GameObjects.Graphics;
  private readonly flashRing: Phaser.GameObjects.Graphics;
  private readonly header: Phaser.GameObjects.Container;
  private readonly hpArt: Phaser.GameObjects.Graphics;
  private readonly hpText: Phaser.GameObjects.Text;
  private readonly dieArt: Phaser.GameObjects.Graphics;
  private readonly dieText: Phaser.GameObjects.Text;
  private readonly palette: TankPalette;
  private readonly facing: 1 | -1;
  private readonly maxHp: number;
  private hpValue: number;

  constructor(scene: Phaser.Scene, config: UnitViewConfig) {
    this.scene = scene;
    this.maxHp = config.maxHp;
    this.hpValue = config.maxHp;
    this.palette = config.side === 'player' ? PLAYER_PALETTE : ENEMY_PALETTE;
    this.facing = config.side === 'player' ? 1 : -1;

    // --- Tank silhouette ---------------------------------------------------
    this.tankArt = scene.add.graphics();
    this.drawTank(this.palette);

    // Initiative flash ring (hidden until the scene flashes it).
    this.flashRing = scene.add.graphics();
    this.flashRing.lineStyle(3, 0xffe066, 1);
    this.flashRing.strokeEllipse(0, -6, 104, 74);
    this.flashRing.setVisible(false);

    this.tank = scene.add.container(0, 0, [this.tankArt, this.flashRing]);
    if (this.facing === -1) this.tank.setScale(-1, 1);

    // --- Header: type icon + HP pill + die badge ----------------------------
    const headerBg = scene.add.graphics();
    // Type icon (left).
    headerBg.fillStyle(0x1c2733, 0.92);
    headerBg.fillCircle(-HP_PILL_W / 2 - 4 - TYPE_R, HEADER_Y, TYPE_R);
    headerBg.lineStyle(1.5, METAL_DARK, 1);
    headerBg.strokeCircle(-HP_PILL_W / 2 - 4 - TYPE_R, HEADER_Y, TYPE_R);
    const typeText = scene.add
      .text(-HP_PILL_W / 2 - 4 - TYPE_R, HEADER_Y, typeGlyph(config.typeId), {
        fontFamily: FONT,
        fontSize: '10px',
        color: '#ffffff',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);

    this.hpArt = scene.add.graphics();
    this.hpText = scene.add
      .text(0, HEADER_Y, `${config.maxHp}/${config.maxHp}`, {
        fontFamily: FONT,
        fontSize: '12px',
        color: '#ffffff',
        fontStyle: 'bold',
        stroke: '#0a0d12',
        strokeThickness: 2,
      })
      .setOrigin(0.5);

    // Die badge (right): dark red so white text keeps >= 4.5:1 contrast.
    this.dieArt = scene.add.graphics();
    this.dieArt.fillStyle(0xb3372b, 1);
    this.dieArt.fillCircle(HP_PILL_W / 2 + 4 + DIE_R, HEADER_Y, DIE_R);
    this.dieArt.lineStyle(2, 0x7e241b, 1);
    this.dieArt.strokeCircle(HP_PILL_W / 2 + 4 + DIE_R, HEADER_Y, DIE_R);
    this.dieText = scene.add
      .text(HP_PILL_W / 2 + 4 + DIE_R, HEADER_Y, '-', {
        fontFamily: FONT,
        fontSize: '11px',
        color: '#ffffff',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);

    this.header = scene.add.container(0, 0, [
      headerBg,
      typeText,
      this.hpArt,
      this.hpText,
      this.dieArt,
      this.dieText,
    ]);

    // --- Nameplate below the tank ------------------------------------------
    const nameText = scene.add
      .text(0, 46, `${config.name} Lv.${config.level}`, {
        fontFamily: FONT,
        fontSize: '11px',
        color: '#f2ead8',
        fontStyle: 'bold',
        stroke: '#14100a',
        strokeThickness: 3,
      })
      .setOrigin(0.5);

    this.container = scene.add.container(config.x, config.y, [
      this.tank,
      this.header,
      nameText,
    ]);
    this.container.setDepth(5);
    this.drawHp();
  }

  /** Shows the given compact face label on the red die badge. */
  setFace(label: string): void {
    this.dieText.setText(label);
  }

  /** Quick pop of the die badge (called when a roll settles). */
  pulseDieBadge(): void {
    this.scene.tweens.add({
      targets: [this.dieArt, this.dieText],
      y: HEADER_Y - 4,
      duration: 90,
      yoyo: true,
      ease: 'Quad.easeOut',
    });
  }

  /** Tweens the HP pill fill and counter down (or up) to the new value. */
  setHp(hp: number): void {
    const from = this.hpValue;
    this.hpValue = hp;
    this.scene.tweens.addCounter({
      from,
      to: hp,
      duration: 450,
      onUpdate: (tween) => this.drawHp(tween.getValue() ?? this.hpValue),
    });
  }

  /**
   * Firing recoil (UnitView.cs:1043 `PlayKickbackAnimation`): the shooter
   * slides ~12px backwards along its facing axis, then returns in 200ms.
   */
  kickback(): void {
    const offset = -this.facing * 12;
    this.scene.tweens.add({
      targets: this.tank,
      x: offset,
      duration: 120,
      ease: 'Quad.easeOut',
      onComplete: () => {
        this.scene.tweens.add({
          targets: this.tank,
          x: 0,
          duration: 200,
          ease: 'Linear',
        });
      },
    });
  }

  /** Brief yellow ring flash; resolves once fully faded. */
  flash(duration = 200): Promise<void> {
    return new Promise((resolve) => {
      this.flashRing.setVisible(true).setAlpha(1);
      this.scene.tweens.add({
        targets: this.flashRing,
        alpha: 0,
        duration,
        onComplete: () => {
          this.flashRing.setVisible(false);
          resolve();
        },
      });
    });
  }

  /** Quick horizontal rattle; ends back on the resting position. */
  shake(): void {
    this.scene.tweens.add({
      targets: this.container,
      x: this.container.x + 9,
      duration: 45,
      yoyo: true,
      repeat: 3,
    });
  }

  /** Repaints the tank in dead-grey and fades the whole unit out. */
  defeat(): void {
    this.drawTank(DEAD_PALETTE);
    this.scene.tweens.add({
      targets: this.container,
      alpha: 0.45,
      duration: 450,
    });
  }

  /** Rising, fading combat number (damage) above the unit. */
  floatLabel(message: string, color: string): void {
    const text = this.scene.add
      .text(this.container.x, this.container.y - 80, message, {
        fontFamily: FONT,
        fontSize: '24px',
        color,
        fontStyle: 'bold',
      })
      .setStroke('#000000', 3)
      .setOrigin(0.5)
      .setDepth(10);
    this.scene.tweens.add({
      targets: text,
      y: text.y - 48,
      alpha: 0,
      duration: 700,
      ease: 'Cubic.easeOut',
      onComplete: () => text.destroy(),
    });
  }

  /** "SPECIAL!" pop; resolves after the full pop + hold + fade cycle. */
  specialPop(): Promise<void> {
    return new Promise((resolve) => {
      const text = this.scene.add
        .text(this.container.x, this.container.y - 130, 'SPECIAL!', {
          fontFamily: FONT,
          fontSize: '24px',
          color: '#ffe066',
          fontStyle: 'bold',
        })
        .setOrigin(0.5)
        .setScale(0.3)
        .setAlpha(0)
        .setDepth(10);
      this.scene.tweens.add({
        targets: text,
        scale: 1,
        alpha: 1,
        duration: 220,
        ease: 'Back.easeOut',
        onComplete: () => {
          this.scene.tweens.add({
            targets: text,
            alpha: 0,
            delay: 350,
            duration: 250,
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
  // Drawing
  // -------------------------------------------------------------------------

  private drawTank(p: TankPalette): void {
    const g = this.tankArt;
    g.clear();

    // Ground shadow.
    g.fillStyle(0x000000, 0.25);
    g.fillEllipse(0, 28, 96, 16);

    // Tracks.
    g.fillStyle(TRACK, 1);
    g.fillRoundedRect(-46, -10, 92, 24, 11);
    g.lineStyle(2, METAL_DARK, 1);
    g.strokeRoundedRect(-46, -10, 92, 24, 11);
    // Road wheels.
    for (const wx of [-33, -16.5, 0, 16.5, 33]) {
      g.fillStyle(WHEEL, 1);
      g.fillCircle(wx, 2, 6);
      g.fillStyle(METAL_DARK, 1);
      g.fillCircle(wx, 2, 2);
    }

    // Hull.
    g.fillStyle(p.base, 1);
    g.fillRoundedRect(-40, -28, 78, 20, 4);
    g.lineStyle(2, p.dark, 1);
    g.strokeRoundedRect(-40, -28, 78, 20, 4);
    // Front glacis (facing forward).
    g.fillStyle(p.light, 1);
    g.fillTriangle(38, -28, 47, -16, 38, -8);
    // Rear stowage box.
    g.fillStyle(p.dark, 1);
    g.fillRoundedRect(-42, -26, 10, 14, 2);

    // Turret.
    g.fillStyle(p.base, 1);
    g.fillCircle(-6, -38, 13);
    g.lineStyle(2, p.dark, 1);
    g.strokeCircle(-6, -38, 13);
    g.fillStyle(p.light, 1);
    g.fillCircle(-9, -41, 5); // hatch highlight

    // Barrel + muzzle brake.
    g.fillStyle(p.dark, 1);
    g.fillRect(4, -43, 36, 6);
    g.fillStyle(METAL_DARK, 1);
    g.fillRect(40, -46, 8, 12);

    // Antenna (rear).
    g.lineStyle(1.5, METAL_DARK, 1);
    g.lineBetween(-32, -28, -37, -50);
  }

  private drawHp(value: number = this.hpValue): void {
    const shown = Math.max(0, Math.round(value));
    const ratio = Phaser.Math.Clamp(shown / this.maxHp, 0, 1);
    const fill = ratio > 0.5 ? 0x3fd463 : ratio > 0.25 ? 0xe8b93e : 0xe0483e;

    const g = this.hpArt;
    g.clear();
    // Pill background + rim.
    g.fillStyle(0x101722, 0.92);
    g.fillRoundedRect(-HP_PILL_W / 2, HEADER_Y - HP_PILL_H / 2, HP_PILL_W, HP_PILL_H, HP_PILL_H / 2);
    g.lineStyle(1.5, METAL_DARK, 1);
    g.strokeRoundedRect(-HP_PILL_W / 2, HEADER_Y - HP_PILL_H / 2, HP_PILL_W, HP_PILL_H, HP_PILL_H / 2);
    // Fill (2px inset).
    if (ratio > 0) {
      const innerW = (HP_PILL_W - 4) * ratio;
      g.fillStyle(fill, 1);
      g.fillRoundedRect(
        -HP_PILL_W / 2 + 2,
        HEADER_Y - HP_PILL_H / 2 + 2,
        Math.max(innerW, HP_PILL_H - 4),
        HP_PILL_H - 4,
        (HP_PILL_H - 4) / 2,
      );
    }
    this.hpText.setText(`${shown}/${this.maxHp}`);
  }
}
