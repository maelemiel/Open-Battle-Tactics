import Phaser from 'phaser';

import { BattleScene } from './ui/scenes/BattleScene';

new Phaser.Game({
  // CANVAS (not AUTO): 2D context keeps automated screenshots deterministic
  // (WebGL capture without preserveDrawingBuffer returns stale/partial frames).
  type: Phaser.CANVAS,
  parent: 'game',
  width: 960,
  height: 540,
  backgroundColor: '#1a1a2e',
  scale: {
    mode: Phaser.Scale.FIT,
    autoCenter: Phaser.Scale.CENTER_BOTH,
  },
  scene: [BattleScene],
});
