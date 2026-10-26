import Phaser from 'phaser';

import diceDamageUrl from './assets/hud/diceeffects/2.png';
import diceInitiativeUrl from './assets/hud/diceeffects/11.png';
import diceSpecialUrl from './assets/hud/diceeffects/5.png';
import diceArmourPiercingUrl from './assets/hud/diceeffects/9.png';
import diceAcidUrl from './assets/hud/diceeffects/7.png';
import ability0Url from './assets/hud/abilityicons/12.png';
import ability1Url from './assets/hud/abilityicons/18.png';
import ability2Url from './assets/hud/abilityicons/22.png';
import ability3Url from './assets/hud/abilityicons/24.png';
import muzzleFlashUrl from './assets/hud/cannonhighfire/3.png';
import impactBurstUrl from './assets/hud/cannonhighhit/0.png';

import { DIE_FACE_TYPE } from '../engine/types';
import type { DieFaceType } from '../engine/types';

/**
 * Real TK2D HUD art (sliced by tools/slice_atlases.py, see
 * docs/HUD_ASSETS.md). One loader + key registry shared by BattleScene
 * and UnitView so textures are registered exactly once.
 */
export const HUD_KEYS = {
  diceDamage: 'hud-dice-damage',
  diceInitiative: 'hud-dice-initiative',
  diceSpecial: 'hud-dice-special',
  diceArmourPiercing: 'hud-dice-ap',
  diceAcid: 'hud-dice-acid',
  abilities: ['hud-ability-0', 'hud-ability-1', 'hud-ability-2', 'hud-ability-3'],
  muzzleFlash: 'hud-fx-muzzle',
  impactBurst: 'hud-fx-impact',
} as const;

/** Texture key for a die-face badge background (null = vector fallback). */
export function dieBadgeKey(faceType: DieFaceType): string | null {
  switch (faceType) {
    case DIE_FACE_TYPE.DAMAGE:
      return HUD_KEYS.diceDamage;
    case DIE_FACE_TYPE.INITIATIVE:
      return HUD_KEYS.diceInitiative;
    case DIE_FACE_TYPE.SPECIAL:
      return HUD_KEYS.diceSpecial;
    case DIE_FACE_TYPE.ARMOUR_PIERCING:
      return HUD_KEYS.diceArmourPiercing;
    case DIE_FACE_TYPE.ACID_STRIKE:
      return HUD_KEYS.diceAcid;
    case DIE_FACE_TYPE.NONE:
      return null; // no neutral badge art -> keep vector circle
  }
}

/** Registers every sliced HUD sprite with the Phaser loader. */
export function loadHudTextures(scene: Phaser.Scene): void {
  scene.load.image(HUD_KEYS.diceDamage, diceDamageUrl);
  scene.load.image(HUD_KEYS.diceInitiative, diceInitiativeUrl);
  scene.load.image(HUD_KEYS.diceSpecial, diceSpecialUrl);
  scene.load.image(HUD_KEYS.diceArmourPiercing, diceArmourPiercingUrl);
  scene.load.image(HUD_KEYS.diceAcid, diceAcidUrl);
  HUD_KEYS.abilities.forEach((key, i) => {
    const urls = [ability0Url, ability1Url, ability2Url, ability3Url];
    scene.load.image(key, urls[i]!);
  });
  scene.load.image(HUD_KEYS.muzzleFlash, muzzleFlashUrl);
  scene.load.image(HUD_KEYS.impactBurst, impactBurstUrl);
}
