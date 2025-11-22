import Phaser from 'phaser';

export default class UIManager {
    constructor(scene) {
        this.scene = scene;
        this.width = scene.scale.width;
        this.height = scene.scale.height;

        // Containers
        this.hudContainer = scene.add.container(0, 0).setDepth(100);
        this.overlay = scene.add.container(0, 0).setDepth(200);

        this.createTopBar();
        this.createBottomBar();
        this.createVictoryOverlay();
    }

    createTopBar() {
        // Simple translucent bar
        const bg = this.scene.add.rectangle(this.width / 2, 30, this.width, 60, 0x000000, 0.5);

        // Title Label
        this.titleText = this.scene.add.text(this.width / 2, 30, 'Open Battle Tactics', {
            fontFamily: 'Arial', fontSize: '32px', fontStyle: 'bold', color: '#ffffff'
        }).setOrigin(0.5);

        // Phase Label (Left)
        this.phaseText = this.scene.add.text(20, 30, 'PHASE: DEPLOY', {
            fontFamily: 'Arial', fontSize: '24px', fontStyle: 'bold', color: '#FFD700'
        }).setOrigin(0, 0.5);

        this.hudContainer.add([bg, this.titleText, this.phaseText]);
    }

    createBottomBar() {
        const barHeight = 100;
        // Ensure it's fully on screen with padding
        const yPos = this.height - barHeight + 10;

        // Background: Solid Grey Panel
        const bg = this.scene.add.rectangle(this.width / 2, 50, this.width, barHeight, 0x333333);
        const border = this.scene.add.rectangle(this.width / 2, 0, this.width, 6, 0x555555);

        // Container for the whole bottom bar
        this.bottomContainer = this.scene.add.container(0, yPos).setDepth(105);
        this.bottomContainer.add([bg, border]);

        // AP (Energy) Section - Left
        this.apSlots = [];
        this.createAPDisplay(100, 50);

        // Fight Button - Right
        this.createFightButton(this.width - 150, 50);

        // Status Text - Center
        this.statusText = this.scene.add.text(this.width / 2, 50, 'READY COMMANDER', {
            fontFamily: 'Arial', fontSize: '20px', color: '#ffffff'
        }).setOrigin(0.5);
        this.bottomContainer.add(this.statusText);

        this.hudContainer.add(this.bottomContainer);
    }

    createAPDisplay(x, y) {
        // Label
        const label = this.scene.add.text(x - 60, y, 'AP:', {
            fontSize: '28px', fontStyle: 'bold', color: '#00E5FF'
        }).setOrigin(0.5);
        this.bottomContainer.add(label);

        // Slots (Circles)
        for (let i = 0; i < 3; i++) {
            const slot = this.scene.add.circle(x + (i * 40), y, 16, 0x222222);
            slot.setStrokeStyle(2, 0xffffff);
            this.apSlots.push(slot);
            this.bottomContainer.add(slot);
        }
        this.drawAP(3);
    }

    createFightButton(x, y) {
        this.fightBtnContainer = this.scene.add.container(x, y);

        // Cartoon Button: Rounded Rect with heavy border
        const btnBg = this.scene.add.rectangle(0, 0, 200, 70, 0xFF4444).setInteractive();
        btnBg.setStrokeStyle(4, 0xFFFFFF);

        const btnText = this.scene.add.text(0, 0, 'FIGHT!', {
            fontFamily: 'Arial Black', fontSize: '32px', color: '#FFFFFF'
        }).setOrigin(0.5);

        // Hover & Click Effects
        btnBg.on('pointerover', () => {
            btnBg.setFillStyle(0xFF6666);
            this.scene.input.setDefaultCursor('pointer');
        });
        btnBg.on('pointerout', () => {
            btnBg.setFillStyle(0xFF4444);
            this.scene.input.setDefaultCursor('default');
        });
        btnBg.on('pointerdown', () => {
            btnBg.setFillStyle(0xCC0000);
            btnBg.setScale(0.95);
            btnText.setScale(0.95);
        });
        btnBg.on('pointerup', () => {
            btnBg.setFillStyle(0xFF6666);
            btnBg.setScale(1);
            btnText.setScale(1);
            this.scene.handleFightButton();
        });

        this.fightBtnContainer.add([btnBg, btnText]);
        this.fightBtnContainer.bg = btnBg;
        this.fightBtnContainer.text = btnText;
        this.bottomContainer.add(this.fightBtnContainer);
    }

    createVictoryOverlay() {
        const bg = this.scene.add.rectangle(this.width/2, this.height/2, this.width, this.height, 0x000000, 0.8);
        this.victoryText = this.scene.add.text(this.width/2, this.height/2, '', {
            fontFamily: 'Arial Black', fontSize: '72px', color: '#FFD700', stroke: '#000', strokeThickness: 6
        }).setOrigin(0.5);

        this.overlay.add([bg, this.victoryText]);
        this.overlay.setVisible(false);
    }

    drawAP(current) {
        this.apSlots.forEach((slot, index) => {
            if (index < current) {
                slot.setFillStyle(0x00E5FF, 1); // Cyan filled
            } else {
                slot.setFillStyle(0x222222, 1); // Empty dark grey
            }
        });
    }

    updateAP(current, max) {
        this.drawAP(current);
    }

    updateStatus(msg) {
        this.statusText.setText(msg);
    }

    updatePhase(phase) {
        this.phaseText.setText(`PHASE: ${phase}`);
    }

    setFightButtonState(text, enabled) {
        this.fightBtnContainer.text.setText(text);
        if (enabled) {
            this.fightBtnContainer.setAlpha(1);
            this.fightBtnContainer.bg.setInteractive();
        } else {
            this.fightBtnContainer.setAlpha(0.5);
            this.fightBtnContainer.bg.disableInteractive();
        }
    }

    showEndScreen(isVictory) {
        this.overlay.setVisible(true);
        this.victoryText.setText(isVictory ? 'VICTORY!' : 'DEFEAT');
        this.victoryText.setColor(isVictory ? '#FFD700' : '#FF4444');
    }
}