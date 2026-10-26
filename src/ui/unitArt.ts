import Phaser from 'phaser';

import voltUrl from './assets/units/volt.png';
import longshotUrl from './assets/units/longshot.png';

/**
 * Authentic unit art (extracted from wiki card captures by
 * tools/extract_unit_art.py workflow, see docs/HUD_ASSETS.md).
 * Keyed by unit id; units without an entry fall back to the
 * vector tank silhouette.
 */
const ART_URLS: Record<number, string> = {
  11001: voltUrl, // VOLT
  11002: longshotUrl, // LONGSHOT
};

export function unitArtKey(unitId: number): string {
  return `unit-art-${unitId}`;
}

export function hasUnitArt(unitId: number): boolean {
  return unitId in ART_URLS;
}

/** Registers available unit textures with the Phaser loader. */
export function loadUnitTextures(scene: Phaser.Scene): void {
  for (const [unitId, url] of Object.entries(ART_URLS)) {
    scene.load.image(unitArtKey(Number(unitId)), url);
  }
}
