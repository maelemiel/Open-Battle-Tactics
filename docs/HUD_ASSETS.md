# HUD Assets (real TK2D art, sliced 2026-10-26)

Source: `assets/bin/Data/AssetRipper_export_20260718_213143/Assets/Texture2D/*SpriteCollection*.png`
Slicer: `python3 tools/slice_atlases.py` (PIL only, connected components on alpha, deterministic: 2 runs = identical tree).
Output: `src/ui/assets/hud/<atlas>/<index>.png` + `manifest.json` (rects). 11 atlases -> 145 sprites.

## Retained (imported by src/ui)

| Category | Sprite | Usage |
|---|---|---|
| die badge DAMAGE | `diceeffects/2.png` (64x75 orange burst) | UnitView die badge bg, face label text on top |
| die badge INITIATIVE | `diceeffects/11.png` (37x36 cyan orb) | same |
| die badge SPECIAL | `diceeffects/5.png` (62x65 gold star) | same |
| die badge ARMOUR_PIERCING | `diceeffects/9.png` (64x67 orange/dark-red slanted burst) | same |
| die badge ACID_STRIKE | `diceeffects/7.png` (64x67 purple splat) | same |
| ability icon x4 | `abilityicons/12.png` (dynamite), `18.png` (fireball), `22.png` (parachute crate), `24.png` (lightning) | 4 slots bottom panel, disabled state = dark tint + alpha |
| FX muzzle flash | `cannonhighfire/3.png` (223x114 horizontal flame plume) | damage-dealt, ~300ms at shooter barrel, flipped for enemy |
| FX impact | `cannonhighhit/0.png` (141x140 spiky starburst) | damage-dealt, ~300ms on target |

## Identified but unused (available for future FX)

- `cannonlowfire/5.png` (226x112 yellow horizontal blast) - alt muzzle flash (low cannon)
- `cannonhighhit/3.png`, `cannonlowhit/0.png` (121x121 / 141x140 pale gold flash) - alt impact
- `smoke/0.png`, `smoke/1.png` (185-197px dark smoke clouds) - death/lingering smoke
- `dusteffect/*` (grey dust puffs) - movement dust
- `besteffect/*` (white/yellow/red/pink/green/cyan star sparkles) - crit/special accents
- `strike/*` (39 sprites: shell streaks + fiery airbursts) - artillery specials
- `diceeffects/0.png` (16x17 tiny orange die) - too small, blurry when scaled up

## Categories NOT found -> vector fallback kept

- HP bar/pill bg: `ArenaSceneSpriteCollection` = ONE full-screen alpha island (backdrop,
  CC-unsliceable, 0 sprites). `HomeScreenSpriteCollection_NewUI` = single connected
  494x924 panel mega-blob (excluded from slicer defaults). -> keep vector HP pill.
- Generic panel/button yellow: same HomeScreen blob. -> keep vector panel + ROLL/FIGHT button.
- Die badge NONE face: no neutral frame found -> keep vector grey circle.

## Notes

- IDs verified visually via downscaled contact sheets (indices = manifest order, y-then-x).
- Sprites are committed build assets; Vite copies only imported files into dist/.

## Wave 3 (2026-10-26, "real UI/UX" pass)

Newly sliced collections (14 more atlases, +158 sprites) and wired:

| Use | Sprite | Where |
| --- | --- | --- |
| roll popup plate | `battledicepopup/0.png` (128x128 white rounded plate) | both final faces shown together ~1s after roll settles |
| coin faces + glow | `cointoss/17.png`, `19.png` (230px gold coins), `11.png` (127x126 radial glow) | initiative-tie flip (3 turns, face swap on scaleX 0) |
| victory burst | `victorysheen/14.png` (127x126 white radial) | behind the victory banner, additive |
| spark burst | `sparks/1.png` (121x121 gold radial) | on every damage impact, additive |
| SHORT CIRCUIT FX | `shortcircuitfx/20.png` (124x117 cyan burst), `22.png` (79x89 arcs) | special-executed 60800 on the victim |
| AP beam | `ionstrikerevised/4.png` (396x21 yellow beam) | armour-piercing damage-dealt: instant muzzle->hull beam (replaces the arcing shell) |
| title art | `titlescreen/0.png` (768x1280 full illustration, opaque) | TitleScene letterboxed right panel |

Engine companion change: `DamageDealtEvent.damageType` now exposed so the UI can pick
the AP beam vs standard shell VFX.

Sliced but unused (candidates): `topbar/*` (HUD top bar pieces), `targeteffect/*`
(reticle), `specialbuffing|buffed/*` (aura frames), `cameraflash/*`, `arenabronzeparallax/*`,
`inbattletexteffects/*` (3 streaks, no digit glyphs -> damage text stays styled text).

## Wave 4 (2026-10-26): the REAL battle HUD, cropped from the gameplay screenshot

`example_fight.jpg` (1280x720, authentic in-game capture) IS the real combat UI.
`tools/crop_ref_hud.py` cuts it into two overlay bands (pre-scaled x0.75 to the
960x540 canvas):

| Band | Source rect | Game placement | Contents (baked) |
| --- | --- | --- | --- |
| `ref/top.png` | (0,0,1280,28) | (0,0), 960x21 | top status bar chrome |
| `ref/bottom.png` | (0,589,930,720)+(930,555,1280,720) stitched | (0,417), 960x123 | command panel, 6 card slots, left portrait column, gold strips, FIGHT button |

Dynamic elements placed ON the real art (positions from pixel analysis of the
screenshot): dice tray cards = slots 0-1 at x 400/452 (badges+labels live-update
during rolls), ability icons = slots 2-5 at x 504..660 (disabled tint), action
control = label/glow over the real gold button at (923,495) - the button's baked
caption (bottom of face, y~653-716 src) stays visible; FIGHT state = pulsing
glow only, no duplicate label.

ArenaSceneSpriteCollection rechecked: 1024x1024 FULLY OPAQUE = background art,
not a HUD frame (alpha map 100% everywhere). Not usable as an overlay.

## Wave 5 (2026-10-26): the gold button is now a REAL interactive sprite

- `tools/crop_ref_hud.py` also extracts `ref/fight_button.png` (76x132): the
  warm-gold slab pulled out of the screenshot by color classification
  (`is_gold()`: r>b+40 family keeps slab + baked caption, drops navy panel and
  green field), trimmed to bbox; the bottom band gets a matching transparent
  hole so the sprite replaces the baked pixels 1:1.
- Click behavior via [rexButton](https://rexrainbow.github.io/phaser3-rex-notes/docs/site/button)
  from [phaser3-rex-plugins](https://www.jsdelivr.com/package/npm/phaser3-rex-plugins)
  1.80.20 (npm; ships its own .d.ts): pixel-true hit area on the actual art,
  clickInterval 250ms, press feedback = sprite scale 0.95 on pointerdown.
- E2E proof (IAB, real CUA clicks on the button): click 1 -> "Round 1: player
  rolled DMG 4, enemy rolled DMG 10", click 2 -> "Initiative: player strikes
  first." Keyboard SPACE / R unchanged.
