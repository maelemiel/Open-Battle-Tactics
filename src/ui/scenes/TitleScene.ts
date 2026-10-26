import Phaser from 'phaser';

import { HUD_KEYS, loadHudTextures } from '../hudAssets';

const WIDTH = 960;
const HEIGHT = 540;
const FONT = 'monospace';

/**
 * Title screen: the authentic TitleScreen illustration (768x1280, shown
 * letterboxed at native aspect) + the game title and a start prompt.
 * Any key or click fades into the battle scene.
 */
export class TitleScene extends Phaser.Scene {
  private starting = false;

  constructor() {
    super('title');
  }

  preload(): void {
    loadHudTextures(this);
  }

  create(): void {
    this.starting = false;
    this.cameras.main.setBackgroundColor('#0d1117');

    // Authentic title art, letterboxed right (324x540 = 768x1280 aspect).
    const artW = (HEIGHT / 1280) * 768;
    this.add
      .image(WIDTH - artW / 2 - 24, HEIGHT / 2, HUD_KEYS.titleArt)
      .setDisplaySize(artW, HEIGHT)
      .setAlpha(0.95);

    // Soft separation edge.
    const edge = this.add.graphics();
    edge.fillStyle(0x000000, 0.35);
    edge.fillRect(WIDTH - artW - 48, 0, 24, HEIGHT);

    const textX = (WIDTH - artW - 48) / 2;
    this.add
      .text(textX, 130, 'SUPER', {
        fontFamily: FONT,
        fontSize: '58px',
        color: '#ffd24a',
        fontStyle: 'bold',
        stroke: '#10131c',
        strokeThickness: 6,
      })
      .setOrigin(0.5);
    this.add
      .text(textX, 196, 'BATTLE TACTICS', {
        fontFamily: FONT,
        fontSize: '44px',
        color: '#ffd24a',
        fontStyle: 'bold',
        stroke: '#10131c',
        strokeThickness: 6,
      })
      .setOrigin(0.5);
    this.add
      .text(textX, 262, 'OPEN BATTLE TACTICS - fan rebuild', {
        fontFamily: FONT,
        fontSize: '15px',
        color: '#8f9bb3',
      })
      .setOrigin(0.5);

    const prompt = this.add
      .text(textX, 400, 'PRESS SPACE TO FIGHT', {
        fontFamily: FONT,
        fontSize: '24px',
        color: '#ffffff',
        fontStyle: 'bold',
        stroke: '#10131c',
        strokeThickness: 4,
      })
      .setOrigin(0.5);
    this.tweens.add({
      targets: prompt,
      alpha: 0.35,
      duration: 600,
      yoyo: true,
      repeat: -1,
      ease: 'Sine.easeInOut',
    });

    this.add
      .text(textX, 468, 'VOLT vs LONGSHOT - space: roll / fight, R: restart', {
        fontFamily: FONT,
        fontSize: '12px',
        color: '#5c6878',
      })
      .setOrigin(0.5);

    const start = () => this.startBattle();
    this.input.keyboard?.on('keydown', start);
    this.input.on('pointerdown', start);
  }

  private startBattle(): void {
    if (this.starting) return;
    this.starting = true;
    this.cameras.main.fadeOut(350, 13, 17, 23);
    this.cameras.main.once(Phaser.Cameras.Scene2D.Events.FADE_OUT_COMPLETE, () => {
      this.scene.start('battle');
    });
  }
}
