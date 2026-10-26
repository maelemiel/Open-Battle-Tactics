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
import dicePlateUrl from './assets/hud/battledicepopup/0.png';
import coinFaceUrl from './assets/hud/cointoss/17.png';
import coinBackUrl from './assets/hud/cointoss/19.png';
import coinFlashUrl from './assets/hud/cointoss/11.png';
import victoryBurstUrl from './assets/hud/victorysheen/14.png';
import sparkBurstUrl from './assets/hud/sparks/1.png';
import shortCircuitBurstUrl from './assets/hud/shortcircuitfx/20.png';
import shortCircuitArcsUrl from './assets/hud/shortcircuitfx/22.png';
import apBeamUrl from './assets/hud/ionstrikerevised/4.png';
import titleArtUrl from './assets/hud/titlescreen/0.png';
import refTopUrl from './assets/hud/ref/top.png';
import refBottomUrl from './assets/hud/ref/bottom.png';
import fightButtonUrl from './assets/hud/ref/fight_button.png';

import { DIE_FACE_TYPE } from '../engine/types';
import type { DieFaceType } from '../engine/types';

/**
 * Real TK2D HUD art (sliced by tools/slice_atlases.py, see
 * docs/HUD_ASSETS.md). One loader + key registry shared by BattleScene,
 * TitleScene and UnitView so textures are registered exactly once.
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
  /** White rounded plate from BattleDicePopUp (the roll popup backdrop). */
  dicePlate: 'hud-dice-plate',
  /** Coin faces + glow from CoinToss (initiative tie flip). */
  coinFace: 'hud-coin-face',
  coinBack: 'hud-coin-back',
  coinFlash: 'hud-coin-flash',
  /** Radial bursts: victory sheen + gold sparks. */
  victoryBurst: 'hud-fx-victory',
  sparkBurst: 'hud-fx-sparks',
  /** Electric burst/arcs from ShortCircuitFX (special 60800). */
  shortCircuitBurst: 'hud-fx-sc-burst',
  shortCircuitArcs: 'hud-fx-sc-arcs',
  /** Yellow beam from IonStrikeRevised (armour-piercing shot). */
  apBeam: 'hud-fx-ap-beam',
  /** Full title-screen illustration (TitleScreen collection, 768x1280). */
  titleArt: 'hud-title-art',
  /** Real HUD bands cropped from the authentic gameplay screenshot. */
  refTop: 'hud-ref-top',
  refBottom: 'hud-ref-bottom',
  /** The gold action button extracted standalone (interactive sprite). */
  fightButton: 'hud-ref-fight-button',
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
  scene.load.image(HUD_KEYS.dicePlate, dicePlateUrl);
  scene.load.image(HUD_KEYS.coinFace, coinFaceUrl);
  scene.load.image(HUD_KEYS.coinBack, coinBackUrl);
  scene.load.image(HUD_KEYS.coinFlash, coinFlashUrl);
  scene.load.image(HUD_KEYS.victoryBurst, victoryBurstUrl);
  scene.load.image(HUD_KEYS.sparkBurst, sparkBurstUrl);
  scene.load.image(HUD_KEYS.shortCircuitBurst, shortCircuitBurstUrl);
  scene.load.image(HUD_KEYS.shortCircuitArcs, shortCircuitArcsUrl);
  scene.load.image(HUD_KEYS.apBeam, apBeamUrl);
  scene.load.image(HUD_KEYS.titleArt, titleArtUrl);
  scene.load.image(HUD_KEYS.refTop, refTopUrl);
  scene.load.image(HUD_KEYS.refBottom, refBottomUrl);
  scene.load.image(HUD_KEYS.fightButton, fightButtonUrl);
}
