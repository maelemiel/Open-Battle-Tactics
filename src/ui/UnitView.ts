import Phaser from 'phaser';

import { DIE_FACE_TYPE } from '../engine/types';
import type { DieFace } from '../engine/types';

const BODY_W = 120;
const BODY_H = 120;
const BAR_W = 136;
const BAR_H = 12;
const BAR_Y = BODY_H / 2 + 18; // local y of the HP bar, below the body
const FONT = 'monospace';

/** Short label for a rolled face, e.g. "DMG 6", "INIT 3", "SPEC". */
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
  }
}

export interface UnitViewConfig {
  readonly x: number;
  readonly y: number;
  readonly name: string;
  readonly level: number;
  readonly maxHp: number;
  readonly color: number;
}

/**
 * Vector-only unit sprite: body rectangle + name + level + HP bar + face
 * readout. No images: everything is Rectangle/Text/Graphics, animated via
 * tweens by the owning scene.
 */
export class UnitView {
  private readonly scene: Phaser.Scene;
  private readonly container: Phaser.GameObjects.Container;
  private readonly outline: Phaser.GameObjects.Rectangle;
  private readonly body: Phaser.GameObjects.Rectangle;
  private readonly faceText: Phaser.GameObjects.Text;
  private readonly hpBar: Phaser.GameObjects.Graphics;
  private readonly hpText: Phaser.GameObjects.Text;
  private readonly maxHp: number;
  private hpValue: number;

  constructor(scene: Phaser.Scene, config: UnitViewConfig) {
    this.scene = scene;
    this.maxHp = config.maxHp;
    this.hpValue = config.maxHp;

    // Flash outline (hidden until the scene triggers an initiative flash).
    this.outline = scene.add
      .rectangle(0, 0, BODY_W + 16, BODY_H + 16, 0x000000, 0)
      .setStrokeStyle(3, 0xffe066);
    this.outline.setVisible(false);

    this.body = scene.add.rectangle(0, 0, BODY_W, BODY_H, config.color);
    this.body.setStrokeStyle(2, 0xeeeeee);

    const nameText = scene.add
      .text(0, -BODY_H / 2 - 30, `${config.name} Lv.${config.level}`, {
        fontFamily: FONT,
        fontSize: '16px',
        color: '#ffffff',
        fontStyle: 'bold',
      })
      .setOrigin(0.5);

    // Rolled-face chip displayed above the unit.
    this.faceText = scene.add
      .text(0, -BODY_H / 2 - 66, '', {
        fontFamily: FONT,
        fontSize: '20px',
        color: '#ffe066',
        fontStyle: 'bold',
        backgroundColor: '#101024',
        padding: { x: 8, y: 4 },
      })
      .setOrigin(0.5);

    this.hpBar = scene.add.graphics();
    this.hpText = scene.add
      .text(0, BODY_H / 2 + 44, `${config.maxHp}/${config.maxHp}`, {
        fontFamily: FONT,
        fontSize: '14px',
        color: '#cfcfe8',
      })
      .setOrigin(0.5);

    this.container = scene.add.container(config.x, config.y, [
      this.outline,
      this.body,
      nameText,
      this.faceText,
      this.hpBar,
      this.hpText,
    ]);
    this.drawHp();
  }

  /** Shows the given face label (empty string hides the chip content). */
  setFace(label: string): void {
    this.faceText.setText(label);
  }

  /** Tweens the HP bar and counter down (or up) to the new value. */
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

  /** Brief yellow outline flash; resolves once fully faded. */
  flash(duration = 200): Promise<void> {
    return new Promise((resolve) => {
      this.outline.setVisible(true).setAlpha(1);
      this.scene.tweens.add({
        targets: this.outline,
        alpha: 0,
        duration,
        onComplete: () => {
          this.outline.setVisible(false);
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

  /** Grays the unit out and fades the whole sprite. */
  defeat(): void {
    this.body.setFillStyle(0x3c3c46);
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

  private drawHp(value: number = this.hpValue): void {
    const shown = Math.round(value);
    const ratio = Phaser.Math.Clamp(shown / this.maxHp, 0, 1);
    this.hpBar.clear();
    this.hpBar.fillStyle(0x5a1f24, 1);
    this.hpBar.fillRect(-BAR_W / 2, BAR_Y, BAR_W, BAR_H);
    this.hpBar.fillStyle(0x3fd463, 1);
    this.hpBar.fillRect(-BAR_W / 2, BAR_Y, BAR_W * ratio, BAR_H);
    this.hpText.setText(`${shown}/${this.maxHp}`);
  }
}
