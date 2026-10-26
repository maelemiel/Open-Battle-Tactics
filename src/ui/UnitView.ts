import Phaser from 'phaser';

import { DIE_FACE_TYPE } from '../engine/types';
import type { DieFace, UnitTypeId } from '../engine/types';
import { dieBadgeKey } from './hudAssets';
import { unitArtKey } from './unitArt';

const FONT = 'monospace';

// ---------------------------------------------------------------------------
// Cartoon tank geometry (authored facing RIGHT, ~110x96 incl. antenna, origin
// at hull center; enemy tanks mirror via container scaleX -1). Three-tone
// banding (light top / base mid / dark shadow) + 2px dark outline per part.
// ---------------------------------------------------------------------------

interface TankPalette {
  readonly base: number;
  readonly dark: number;
  readonly light: number;
  readonly shadow: number;
}

const PLAYER_PALETTE: TankPalette = {
  base: 0x4a79b8,
  dark: 0x2d5084,
  light: 0x8fb4e8,
  shadow: 0x1f3a63,
};
const ENEMY_PALETTE: TankPalette = {
  base: 0xa85a38,
  dark: 0x7a3c22,
  light: 0xd08a5e,
  shadow: 0x5a2c18,
};
const DEAD_PALETTE: TankPalette = {
  base: 0x4a4f57,
  dark: 0x34383f,
  light: 0x6d737c,
  shadow: 0x26292f,
};

const OUTLINE = 0x14181d;
const TRACK = 0x262b30;
const WHEEL = 0x454c55;
const HUB = 0x1a1e22;

/** Silhouette variant per unit family (mirrors typeGlyph mapping). */
type TankVariant = 'assault' | 'command' | 'operative' | 'helicopter';

function tankVariant(typeId: UnitTypeId): TankVariant {
  switch (typeId) {
    case 2:
    case 6:
    case 12:
      return 'command';
    case 3:
    case 7:
    case 13:
      return 'operative';
    case 4:
    case 8:
    case 10:
      return 'helicopter';
    default:
      return 'assault'; // assault / bosses / fallback
  }
}

// Header (above the tank, never mirrored).
const HP_PILL_W = 64;
const HP_PILL_H = 14;
const BADGE_H = 40; // die-badge display height (real art ~64x75)
const TYPE_R = 9;
const HEADER_Y = -66;
/** Ground line for authentic sprites (origin bottom). */
const TANK_BASE_Y = 34;
const BADGE_X = HP_PILL_W / 2 + 26;

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
  readonly unitId: number;
  readonly name: string;
  readonly level: number;
  readonly typeId: UnitTypeId;
  readonly maxHp: number;
  /** 'player' = faces right (blue), 'enemy' = faces left (red-brown). */
  readonly side: 'player' | 'enemy';
}

/**
 * Cartoon tank unit: Graphics silhouette (hull with 3-tone banding, turret,
 * barrel with muzzle brake, tracks with road wheels, stowage details; 4
 * silhouette variants) + header with the real TK2D die badge (per face type,
 * docs/HUD_ASSETS.md), HP pill and unit-type icon. Animations are driven by
 * the owning scene.
 */
export class UnitView {
  private readonly scene: Phaser.Scene;
  private readonly container: Phaser.GameObjects.Container;
  /** Tank art only: mirrored for the enemy, kickback tweens live here. */
  private readonly tank: Phaser.GameObjects.Container;
  private readonly tankArt: Phaser.GameObjects.Graphics;
  /** Authentic sprite when available (unitArt.ts); null = vector fallback. */
  private readonly tankSprite: Phaser.GameObjects.Image | null;
  private readonly flashRing: Phaser.GameObjects.Graphics;
  private readonly header: Phaser.GameObjects.Container;
  private readonly hpArt: Phaser.GameObjects.Graphics;
  private readonly hpText: Phaser.GameObjects.Text;
  private readonly dieArt: Phaser.GameObjects.Graphics;
  private readonly dieBadge: Phaser.GameObjects.Image;
  private readonly dieText: Phaser.GameObjects.Text;
  private readonly variant: TankVariant;
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
    this.variant = tankVariant(config.typeId);

    // --- Tank art: authentic sprite or vector silhouette ---------------------
    const artKey = unitArtKey(config.unitId);
    let groundShadow: Phaser.GameObjects.Graphics | null = null;
    if (scene.textures.exists(artKey)) {
      const img = scene.add.image(0, 0, artKey).setOrigin(0.5, 1);
      const targetH = 118;
      img.setScale(targetH / img.height);
      img.setY(TANK_BASE_Y);
      this.tankSprite = img;
      // Contact shadow grounds the sprite on the arena floor.
      groundShadow = scene.add.graphics();
      groundShadow.fillStyle(0x000000, 0.3);
      groundShadow.fillEllipse(0, TANK_BASE_Y + 4, img.displayWidth * 0.7, 15);
      this.tankArt = scene.add.graphics(); // kept for defeat()/API compat
      this.tankArt.setVisible(false);
    } else {
      this.tankSprite = null;
      this.tankArt = scene.add.graphics();
      this.drawTank(this.palette);
    }

    // Initiative flash ring (hidden until the scene flashes it).
    this.flashRing = scene.add.graphics();
    this.flashRing.lineStyle(3, 0xffe066, 1);
    this.flashRing.strokeEllipse(0, -6, 110, 80);
    this.flashRing.setVisible(false);

    const tankChildren: Phaser.GameObjects.GameObject[] = [
      ...(groundShadow ? [groundShadow] : []),
      this.tankSprite ?? this.tankArt,
      this.flashRing,
    ];
    this.tank = scene.add.container(0, 0, tankChildren);
    if (this.facing === -1) this.tank.setScale(-1, 1);

    // --- Header: type icon + HP pill + die badge ----------------------------
    const headerBg = scene.add.graphics();
    // Type icon (left).
    headerBg.fillStyle(0x1c2733, 0.92);
    headerBg.fillCircle(-HP_PILL_W / 2 - 4 - TYPE_R, HEADER_Y, TYPE_R);
    headerBg.lineStyle(1.5, OUTLINE, 1);
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

    // Die badge (right): real TK2D art per face type when rolled; the dark
    // red vector circle is the placeholder until the first roll (and the
    // fallback for the NONE face). Text sits on top with a stroke so white
    // keeps >= 4.5:1 contrast against the badge art.
    this.dieBadge = scene.add.image(BADGE_X, HEADER_Y, '__DEFAULT').setVisible(false);
    this.dieArt = scene.add.graphics();
    this.dieArt.fillStyle(0xb3372b, 1);
    this.dieArt.fillCircle(BADGE_X, HEADER_Y, 10.5);
    this.dieArt.lineStyle(2, 0x7e241b, 1);
    this.dieArt.strokeCircle(BADGE_X, HEADER_Y, 10.5);
    this.dieText = scene.add
      .text(BADGE_X, HEADER_Y + 2, '-', {
        fontFamily: FONT,
        fontSize: '12px',
        color: '#ffffff',
        fontStyle: 'bold',
        stroke: '#1a0d08',
        strokeThickness: 3,
      })
      .setOrigin(0.5);

    this.header = scene.add.container(0, 0, [
      headerBg,
      typeText,
      this.hpArt,
      this.hpText,
      this.dieBadge,
      this.dieArt,
      this.dieText,
    ]);

    // --- Nameplate below the tank ------------------------------------------
    const nameText = scene.add
      .text(0, 48, `${config.name} Lv.${config.level}`, {
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

    // Idle breathing so the units never look pasted on the backdrop;
    // per-unit duration/delay so the two sides never bob in sync.
    scene.tweens.add({
      targets: this.tank,
      y: -2,
      duration: 1400 + (config.unitId % 5) * 110,
      delay: (config.unitId % 3) * 240,
      yoyo: true,
      repeat: -1,
      ease: 'Sine.easeInOut',
    });
  }

  /** World position of the unit (container root), for scene-side FX. */
  get x(): number {
    return this.container.x;
  }

  get y(): number {
    return this.container.y;
  }

  /** Barrel-tip world position: where muzzle flashes start and shells leave. */
  get muzzleX(): number {
    const reach = this.tankSprite ? this.tankSprite.displayWidth * 0.4 : 54;
    return this.container.x + this.facing * reach;
  }

  get muzzleY(): number {
    return this.container.y + (this.tankSprite ? TANK_BASE_Y - 44 : -40);
  }

  /** Hull world position: where shells land and impacts burst. */
  get bodyX(): number {
    return this.container.x + this.facing * 6;
  }

  get bodyY(): number {
    return this.container.y + (this.tankSprite ? TANK_BASE_Y - 36 : -28);
  }

  /**
   * Battle-start drive-in: the tank rolls in from `fromDx` px off its slot
   * (player: negative, enemy: positive) kicking up dust, and resolves once
   * parked. Replayed on every restart via scene.restart().
   */
  driveIn(fromDx: number): Promise<void> {
    const destX = this.container.x;
    this.container.x = destX + fromDx;
    return new Promise((resolve) => {
      const dust = this.scene.time.addEvent({ delay: 80, callback: () => this.dustPuff() });
      this.scene.tweens.add({
        targets: this.container,
        x: destX,
        duration: 700,
        ease: 'Cubic.easeOut',
        onComplete: () => {
          dust.remove();
          this.dustPuff();
          resolve();
        },
      });
    });
  }

  /** One expanding, fading ground-dust puff behind the tracks. */
  private dustPuff(): void {
    const baseY = this.container.y + (this.tankSprite ? TANK_BASE_Y + 2 : 30);
    const bx = this.container.x - this.facing * 30;
    const puff = this.scene.add.graphics().setDepth(4);
    this.scene.tweens.addCounter({
      from: 0,
      to: 1,
      duration: 430,
      onUpdate: (t) => {
        const k = t.getValue() ?? 1;
        puff.clear();
        puff.fillStyle(0x9c8f7a, 0.45 * (1 - k));
        puff.fillCircle(bx, baseY - 8 * k, 5 + 10 * k);
      },
      onComplete: () => puff.destroy(),
    });
  }

  /** Brief whole-sprite tint pulse (hit = warm, acid = green). */
  hitFlash(tint = 0xffb0a0): void {
    if (!this.tankSprite) return;
    this.tankSprite.setTint(tint);
    this.scene.time.delayedCall(130, () => this.tankSprite?.clearTint());
  }

  /** Expanding armor ring at the hull: the "blocked" visual. */
  armorRipple(): void {
    const cx = this.container.x + this.facing * 6;
    const cy = this.container.y - 26;
    const ring = this.scene.add.graphics().setDepth(7);
    this.scene.tweens.addCounter({
      from: 0,
      to: 1,
      duration: 380,
      ease: 'Quad.easeOut',
      onUpdate: (t) => {
        const k = t.getValue() ?? 1;
        ring.clear();
        ring.lineStyle(3, 0x6ec6ff, 1 - k);
        ring.strokeEllipse(cx, cy, 48 + 44 * k, 56 + 50 * k);
      },
      onComplete: () => ring.destroy(),
    });
  }

  /**
   * Shows the given face: swaps the real badge art to match the face type
   * (vector circle fallback for NONE) and updates the compact label.
   */
  setFace(face: DieFace): void {
    const key = dieBadgeKey(face.type);
    if (key !== null && this.scene.textures.exists(key)) {
      this.dieBadge.setTexture(key).setVisible(true);
      this.dieBadge.setScale(BADGE_H / this.dieBadge.height);
      this.dieArt.setVisible(false);
    } else {
      this.dieBadge.setVisible(false);
      this.dieArt.setVisible(true);
    }
    this.dieText.setText(faceBadgeLabel(face));
  }

  /** Quick pop of the die badge (called when a roll settles). */
  pulseDieBadge(): void {
    this.scene.tweens.add({
      targets: [this.dieBadge, this.dieArt, this.dieText],
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

  /** Repaints the tank in dead-grey, stops the idle bob and tilts the wreck. */
  defeat(): void {
    this.scene.tweens.killTweensOf(this.tank);
    this.tank.angle = this.facing * 6; // dead lean, opposite the firing crouch
    if (this.tankSprite) {
      this.tankSprite.setTint(0x8a8a8a);
    } else {
      this.drawTank(DEAD_PALETTE);
    }
    this.scene.tweens.add({
      targets: this.container,
      alpha: 0.45,
      duration: 450,
    });
  }

  /** Rising, fading combat number (damage) above the unit. */
  floatLabel(message: string, color: string, small = false): void {
    const text = this.scene.add
      .text(this.container.x - (small ? 36 : 0), this.container.y - (small ? 104 : 80), message, {
        fontFamily: FONT,
        fontSize: small ? '15px' : '24px',
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
    g.fillEllipse(0, 30, 106, 16);

    this.drawTracks(g);
    this.drawHull(g, p);
    this.drawRearDetails(g, p);
    switch (this.variant) {
      case 'command':
        this.drawCommandTurret(g, p);
        break;
      case 'operative':
        this.drawOperativeTurret(g, p);
        break;
      case 'helicopter':
        this.drawHelicopterTop(g, p);
        break;
      default:
        this.drawAssaultTurret(g, p);
    }
  }

  /** Tracks with road wheels, front sprocket, rear idler and a top band. */
  private drawTracks(g: Phaser.GameObjects.Graphics): void {
    g.fillStyle(TRACK, 1);
    g.fillRoundedRect(-50, -10, 100, 28, 13);
    g.lineStyle(2, OUTLINE, 1);
    g.strokeRoundedRect(-50, -10, 100, 28, 13);
    // Upper track band ( darker tread strip ).
    g.fillStyle(0x1c2126, 1);
    g.fillRoundedRect(-48, -9, 96, 7, 3);
    // Road wheels.
    for (const wx of [-36, -21, -6, 9, 24]) {
      g.fillStyle(WHEEL, 1);
      g.fillCircle(wx, 5, 5.5);
      g.fillStyle(HUB, 1);
      g.fillCircle(wx, 5, 2);
    }
    // Front drive sprocket + rear idler ( slightly larger rims ).
    for (const [wx, r] of [
      [42, 6.5],
      [-42, 6],
    ] as const) {
      g.fillStyle(WHEEL, 1);
      g.fillCircle(wx, 3, r);
      g.fillStyle(HUB, 1);
      g.fillCircle(wx, 3, 2.4);
    }
  }

  /** Hull with 3-tone banding + sloped front glacis facing forward. */
  private drawHull(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    // Mid body, then light top plate, then dark lower shadow strip, one
    // outline around the whole box: reads as a cartoon gradient.
    g.fillStyle(p.base, 1);
    g.fillRoundedRect(-44, -32, 84, 24, 4);
    g.fillStyle(p.light, 1);
    g.fillRoundedRect(-44, -32, 84, 8, 4);
    g.fillStyle(p.dark, 1);
    g.fillRoundedRect(-44, -15, 84, 7, 3);
    g.lineStyle(2, OUTLINE, 1);
    g.strokeRoundedRect(-44, -32, 84, 24, 4);

    // Sloped front glacis ( light face, drawn over the hull front ).
    g.fillStyle(p.light, 1);
    g.fillTriangle(38, -32, 54, -18, 38, -8);
    g.lineStyle(2, OUTLINE, 1);
    g.strokeTriangle(38, -32, 54, -18, 38, -8);
    g.fillStyle(p.dark, 1);
    g.fillTriangle(38, -32, 45, -25, 38, -22);

    // Rear engine deck grate ( shadow slats ).
    g.fillStyle(p.shadow, 1);
    for (const gx of [-40, -34, -28]) {
      g.fillRect(gx, -30, 3, 18);
    }
  }

  /** Rear stowage: box + jerrycan + antenna base. */
  private drawRearDetails(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    // Stowage box on the rear deck.
    g.fillStyle(p.shadow, 1);
    g.fillRoundedRect(-48, -30, 9, 13, 2);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeRoundedRect(-48, -30, 9, 13, 2);
    // Jerrycan hanging off the back.
    g.fillStyle(p.dark, 1);
    g.fillRoundedRect(-52, -18, 8, 9, 1.5);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeRoundedRect(-52, -18, 8, 9, 1.5);
    g.lineBetween(-50, -18, -46, -18);
  }

  /** 3-tone rounded turret body + hatch; shared by the tracked variants. */
  private drawTurretBody(
    g: Phaser.GameObjects.Graphics,
    p: TankPalette,
    cx: number,
    cy: number,
    w: number,
    h: number,
  ): void {
    g.fillStyle(p.base, 1);
    g.fillRoundedRect(cx - w / 2, cy - h / 2, w, h, 5);
    g.fillStyle(p.light, 1);
    g.fillRoundedRect(cx - w / 2, cy - h / 2, w, h * 0.42, 5);
    g.fillStyle(p.dark, 1);
    g.fillRoundedRect(cx - w / 2, cy + h / 2 - h * 0.3, w, h * 0.3, 4);
    g.lineStyle(2, OUTLINE, 1);
    g.strokeRoundedRect(cx - w / 2, cy - h / 2, w, h, 5);
    // Hatch highlight.
    g.fillStyle(p.light, 1);
    g.fillCircle(cx - 4, cy - 2, 4);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeCircle(cx - 4, cy - 2, 4);
  }

  /** Barrel with base sleeve and double-baffle muzzle brake. */
  private drawBarrel(
    g: Phaser.GameObjects.Graphics,
    p: TankPalette,
    x: number,
    y: number,
    len: number,
  ): void {
    g.fillStyle(p.dark, 1);
    g.fillRect(x, y - 2.5, len, 5);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeRect(x, y - 2.5, len, 5);
    // Base sleeve.
    g.fillStyle(p.shadow, 1);
    g.fillRect(x, y - 4, 8, 8);
    g.strokeRect(x, y - 4, 8, 8);
    // Muzzle brake ( wider than the barrel ).
    const mx = x + len - 7;
    g.fillStyle(OUTLINE, 1);
    g.fillRect(mx, y - 5, 7, 10);
    g.fillStyle(p.shadow, 1);
    g.fillRect(mx + 2, y - 3.5, 3, 7);
  }

  /** Assault: compact turret + long barrel. */
  private drawAssaultTurret(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    this.drawTurretBody(g, p, -4, -40, 24, 14);
    this.drawBarrel(g, p, 8, -40, 42);
    // Whippy radio antenna on the rear deck.
    g.lineStyle(1.5, OUTLINE, 1);
    g.lineBetween(-30, -32, -38, -54);
    g.fillStyle(OUTLINE, 1);
    g.fillCircle(-38, -54, 1.5);
  }

  /** Command: wide turret, medium gun, twin antennas. */
  private drawCommandTurret(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    this.drawTurretBody(g, p, -4, -40, 32, 15);
    this.drawBarrel(g, p, 12, -40, 30);
    for (const [x0, x1, y1] of [
      [-18, -24, -60],
      [-10, -8, -63],
    ] as const) {
      g.lineStyle(1.5, OUTLINE, 1);
      g.lineBetween(x0, -46, x1, y1);
      g.fillStyle(OUTLINE, 1);
      g.fillCircle(x1, y1, 1.5);
    }
  }

  /** Operative: compact turret, stub gun, radar dish on a mast. */
  private drawOperativeTurret(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    this.drawTurretBody(g, p, 0, -38, 18, 11);
    this.drawBarrel(g, p, 9, -38, 12);
    // Radar: mast + dish.
    g.lineStyle(2, OUTLINE, 1);
    g.lineBetween(-10, -44, -14, -56);
    g.fillStyle(p.light, 1);
    g.fillEllipse(-14, -60, 16, 8);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeEllipse(-14, -60, 16, 8);
    g.fillStyle(OUTLINE, 1);
    g.fillCircle(-14, -60, 1.5);
  }

  /** Helicopter: rotor mast + blades + tail boom ( hull kept tank-like ). */
  private drawHelicopterTop(g: Phaser.GameObjects.Graphics, p: TankPalette): void {
    this.drawTurretBody(g, p, 0, -40, 20, 12);
    // Tail boom to the rear.
    g.fillStyle(p.base, 1);
    g.fillRoundedRect(-52, -42, 26, 6, 3);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeRoundedRect(-52, -42, 26, 6, 3);
    g.fillStyle(p.dark, 1);
    g.fillCircle(-52, -39, 4);
    g.strokeCircle(-52, -39, 4);
    // Rotor mast + long thin blades.
    g.lineStyle(2, OUTLINE, 1);
    g.lineBetween(0, -46, 0, -56);
    g.fillStyle(p.light, 1);
    g.fillEllipse(0, -57, 52, 3.5);
    g.lineStyle(1.5, OUTLINE, 1);
    g.strokeEllipse(0, -57, 52, 3.5);
    g.fillStyle(OUTLINE, 1);
    g.fillCircle(0, -57, 2);
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
    g.lineStyle(1.5, OUTLINE, 1);
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
