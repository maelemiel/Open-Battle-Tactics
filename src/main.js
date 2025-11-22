import Phaser from 'phaser';
import BattleScene from './scenes/BattleScene';

const config = {
    type: Phaser.CANVAS, // Force Canvas rendering to avoid WebGL warnings
    scale: {
        mode: Phaser.Scale.FIT,
        autoCenter: Phaser.Scale.CENTER_BOTH,
        width: 1600,
        height: 900
    },
    render: {
        antialias: true,
        pixelArt: false,
        roundPixels: true, // Crucial for sharp text
    },
    parent: 'game-container',
    backgroundColor: '#000000',
    scene: [BattleScene],
    physics: {
        default: 'arcade',
        arcade: { debug: false }
    }
};

const game = new Phaser.Game(config);