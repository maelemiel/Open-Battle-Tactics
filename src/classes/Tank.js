import Phaser from 'phaser';

/**
 * Represents a single Tank unit on the battlefield.
 * Handles visualization, stats management, wheel rolling, and damage.
 */
export default class Tank extends Phaser.GameObjects.Container {
    /**
     * @param {Phaser.Scene} scene - The scene this tank belongs to.
     * @param {number} x - X position.
     * @param {number} y - Y position.
     * @param {number} unitId - The database ID of the unit (e.g., 11001).
     * @param {boolean} isPlayer - True if this tank belongs to the player.
     */
    constructor(scene, x, y, unitId, isPlayer) {
        super(scene, x, y);
        this.scene = scene;
        this.unitId = unitId;
        this.isPlayer = isPlayer;

        // Load Data from Cache (populated in Preload)
        const allUnits = scene.cache.json.get('units');
        this.unitData = allUnits.find(u => u.id == unitId); 

        if (!this.unitData) {
            console.error(`Unit ID ${unitId} not found! Fallback to default.`);
            this.unitData = { 
                name: "Unknown", 
                rarity: 1, 
                hp: 100, 
                wheelValues: [1, 2, 3, 4, 5],
                type: 1 
            };
        }

        // Dynamic Stats initialization
        this.maxHp = this.unitData.hp;
        this.hp = this.maxHp;
        this.currentWheelValue = 0;
        this.isBoosted = false;

        // Setup
        this.createVisuals();
        this.createUI();

        // Interaction Setup
        this.setInteractive(new Phaser.Geom.Rectangle(-40, -40, 80, 80), Phaser.Geom.Rectangle.Contains);
        this.on('pointerdown', () => this.scene.selectTank(this));
        this.on('pointerover', () => this.onHover(true));
        this.on('pointerout', () => this.onHover(false));

        scene.add.existing(this);
    }

    /**
     * Draws the tank chassis and rarity glow based on data.
     */
    createVisuals() {
        // Rarity Colors Mapping
        const rarityColors = {
            1: 0xbdc3c7, // Common (Grey)
            2: 0x2ecc71, // Uncommon (Green)
            3: 0x3498db, // Rare (Blue)
            4: 0x9b59b6, // Epic (Purple)
            5: 0xf1c40f  // Legendary (Gold)
        };
        const color = rarityColors[this.unitData.rarity] || 0xffffff;

        // Base Chassis (Hexagon-ish shape)
        this.chassis = this.scene.add.graphics();
        this.chassis.fillStyle(0x2c3e50, 1);
        
        const s = 40; // Size scalar
        this.chassis.beginPath();
        this.chassis.moveTo(-s, -s/2);
        this.chassis.lineTo(-s/2, -s);
        this.chassis.lineTo(s/2, -s);
        this.chassis.lineTo(s, -s/2);
        this.chassis.lineTo(s, s/2);
        this.chassis.lineTo(s/2, s);
        this.chassis.lineTo(-s/2, s);
        this.chassis.lineTo(-s, s/2);
        this.chassis.closePath();
        this.chassis.fillPath();
        
        // Border with Rarity Color
        this.chassis.lineStyle(3, color);
        this.chassis.strokePath();

        // Inner Detail (Direction indicator)
        this.chassis.fillStyle(color, 0.3);
        this.chassis.fillCircle(0, 0, 15);

        // Selection Glow (Hidden by default)
        this.glow = this.scene.add.graphics();
        this.glow.lineStyle(4, 0xffff00, 0.8);
        this.glow.beginPath();
        this.glow.moveTo(-s-5, -s/2-5);
        this.glow.lineTo(s+5, -s/2-5);
        this.glow.lineTo(s+5, s/2+5);
        this.glow.lineTo(-s-5, s/2+5);
        this.glow.closePath();
        this.glow.strokePath();
        this.glow.setVisible(false);

        // ORIENTATION FIX: Flip only the graphics, NOT the container/text
        const dir = this.isPlayer ? 1 : -1;
        this.chassis.setScale(dir, 1);
        this.glow.setScale(dir, 1);

        this.add([this.glow, this.chassis]);
    }

    createUI() {
        // Name Text (Top)
        this.nameText = this.scene.add.text(0, -70, this.unitData.name, {
            fontSize: '16px',
            color: '#ffffff',
            fontFamily: 'Arial Black',
            stroke: '#000000',
            strokeThickness: 4
        }).setOrigin(0.5);

        // HP Text (Replaces Bar)
        this.hpText = this.scene.add.text(0, -50, `${this.hp}/${this.maxHp}`, {
            fontSize: '18px',
            color: '#2ecc71', // Start Green
            fontFamily: 'monospace',
            fontStyle: 'bold',
            stroke: '#000000',
            strokeThickness: 3
        }).setOrigin(0.5);

        // Wheel Values Display (The 5 potential numbers)
        const valuesStr = this.unitData.wheelValues.join('  ');
        this.statsText = this.scene.add.text(0, 60, valuesStr, {
            fontSize: '14px',
            color: '#aaaaaa',
            fontFamily: 'monospace',
            fontStyle: 'bold',
            backgroundColor: '#00000088'
        }).setOrigin(0.5);

        // Wheel Result (Floating Holo Text - Center)
        // Background for the roll result to make it pop
        const rollBg = this.scene.add.circle(0, 0, 28, 0x000000, 0.7);
        this.add(rollBg);

        this.wheelText = this.scene.add.text(0, 0, '?', {
            fontSize: '42px',
            fontStyle: 'bold',
            color: '#00FFFF', // Cyan for better visibility
            stroke: '#000000',
            strokeThickness: 6
        }).setOrigin(0.5).setVisible(true);

        this.add([this.nameText, this.hpText, this.statsText, this.wheelText]);
    }

    updateHealthDisplay() {
        this.hpText.setText(`${this.hp}/${this.maxHp}`);
        
        const pct = this.hp / this.maxHp;
        if (pct <= 0.3) this.hpText.setColor('#e74c3c'); // Red
        else if (pct <= 0.6) this.hpText.setColor('#f1c40f'); // Yellow
        else this.hpText.setColor('#2ecc71'); // Green
    }

    roll() {
        const values = this.unitData.wheelValues;
        // Pick random value
        const randomIndex = Phaser.Math.Between(0, values.length - 1);
        this.currentWheelValue = values[randomIndex];
        
        // Reset visuals
        this.isBoosted = false;
        this.wheelText.setColor('#ffffff');
        this.wheelText.setText(this.currentWheelValue);
        this.highlight(false);
        
        // Ensure visible immediately
        this.wheelText.setAlpha(1);
        this.wheelText.setScale(0.5);

        // Pop Animation (Scale only, no alpha fade out)
        this.scene.tweens.add({
            targets: this.wheelText,
            scale: 1.5,
            duration: 200,
            yoyo: true, // Scale goes up then down (Pop effect)
            onComplete: () => {
                this.wheelText.setScale(1); // Ensure it settles at normal size
                this.wheelText.setAlpha(1); // Ensure it stays visible
            }
        });

        return this.currentWheelValue;
    }

    boost() {
        if (this.isBoosted) return;
        
        this.isBoosted = true;
        const values = this.unitData.wheelValues;
        
        // Boost Logic: Max Value + 2 (Deterministic bonus)
        // In a full game, this might reroll with advantage, but flat bonus is good for tactics
        const boostedValue = Math.max(...values) + 2;
        
        this.currentWheelValue = boostedValue;
        
        // Visuals
        this.wheelText.setColor('#f1c40f'); // Gold
        this.wheelText.setText(this.currentWheelValue + '!');
        this.highlight(true);

        // Pop animation
        this.scene.tweens.add({
            targets: this.wheelText,
            scale: { from: 1, to: 2 },
            duration: 200,
            yoyo: true
        });
    }

    highlight(isActive) {
        this.glow.setVisible(isActive);
        if (isActive) {
             this.scene.tweens.add({
                targets: this.glow,
                alpha: 0.4,
                duration: 500,
                yoyo: true,
                repeat: -1
            });
        } else {
            this.scene.tweens.killTweensOf(this.glow);
            this.glow.setAlpha(0.8);
        }
    }

    onHover(state) {
        if (this.isPlayer) {
            this.chassis.setAlpha(state ? 0.8 : 1);
            this.scene.input.setDefaultCursor(state ? 'pointer' : 'default');
        }
    }

    takeDamage(amount) {
        // Floating Combat Text
        this.spawnDamageText(amount);

        this.hp = Phaser.Math.Clamp(this.hp - amount, 0, this.maxHp);
        this.updateHealthDisplay();

        if (this.hp <= 0) {
            this.wheelText.setText('KO');
            // In Canvas mode, setTint is not supported on Graphics.
            // Use alpha to show disabled state.
            this.setAlpha(0.3); 
            return true; // Destroyed
        }
        return false;
    }

    spawnDamageText(amount) {
        const txt = this.scene.add.text(this.x, this.y - 60, `-${amount}`, {
            fontSize: '24px',
            color: '#ff0000',
            fontStyle: 'bold',
            stroke: '#fff',
            strokeThickness: 2
        }).setOrigin(0.5).setDepth(200);

        this.scene.tweens.add({
            targets: txt,
            y: this.y - 100,
            alpha: 0,
            duration: 800,
            onComplete: () => txt.destroy()
        });
    }

    resetWheelDisplay() {
        this.wheelText.setText('?');
        this.wheelText.setColor('#ffffff');
    }
}