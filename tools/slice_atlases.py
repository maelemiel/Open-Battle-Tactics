#!/usr/bin/env python3
"""Slice TK2D UI atlases into individual sprites (deterministic).

Connected-component analysis on the alpha channel: each opaque island in an
atlas becomes one PNG crop in src/ui/assets/hud/<atlas>/<index>.png plus a
manifest.json (rect + size) per atlas folder. Re-running regenerates every
output folder from scratch (delete + rebuild), so two runs always produce
byte-identical trees.

Dependencies: Python 3 + Pillow only.
Usage: python3 tools/slice_atlases.py [atlas_filename ...]   (default: all)
"""

from __future__ import annotations

import json
import shutil
import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parent.parent
ATLAS_DIR = (
    REPO
    / "assets/bin/Data/AssetRipper_export_20260718_213143/Assets/Texture2D"
)
OUT_ROOT = REPO / "src/ui/assets/hud"

# Battle-HUD-relevant TK2D collections (exact filenames, verified in repo).
DEFAULT_ATLASES = [
    "DiceEffectsSpriteCollection Data.png",
    "AbilityIconsSpriteCollection Data.png",
    "CannonHighFire_SpriteCollection Data.png",
    "CannonLowFireSpriteCollection Data.png",
    "CannonHighHitSpriteCollectionData.png",
    "CannonLowHitSpriteCollection Data.png",
    "SmokeEffectsCollection Data.png",
    "StrikeSpriteCollection Data.png",
    "BestEffect_SpriteCollection Data.png",
    "DustEffectSpriteCollection Data.png",
    "ArenaSceneSpriteCollection Data.png",
    # NOTE: HomeScreenSpriteCollection_NewUI excluded: its only alpha island
    # is one connected 494x924 mega-blob (panels touching), unsliceable by CC.
]

MIN_SIDE = 8          # islands smaller than MIN_SIDE x MIN_SIDE are noise
ALPHA_THRESHOLD = 8   # alpha >= this counts as opaque
FULLSCREEN_RATIO = 0.95  # islands spanning >= 95% of both dims = backgrounds


def atlas_slug(filename: str) -> str:
    """Output folder name: drop the '...Collection Data.png' suffix noise."""
    name = filename
    for suffix in (
        "SpriteCollection Data.png",
        "SpriteCollectionData.png",
        "EffectsCollection Data.png",
        "Collection Data.png",
        " Data.png",
        ".png",
    ):
        if name.endswith(suffix):
            name = name[: -len(suffix)]
            break
    name = name.replace("SpriteCollection", "").replace("Sprite", "")
    return (
        name.replace(" ", "-")
        .replace("_", "-")
        .replace("--", "-")
        .strip("-")
        .lower()
    )


def find_islands(im: Image.Image) -> list[dict]:
    """Bounding boxes of alpha-connected islands, sorted top-to-bottom."""
    width, height = im.size
    alpha = im.getchannel("A").load()
    seen = bytearray(width * height)
    islands: list[dict] = []

    for y0 in range(height):
        base0 = y0 * width
        for x0 in range(width):
            if seen[base0 + x0] or alpha[x0, y0] < ALPHA_THRESHOLD:
                continue
            queue = deque([(x0, y0)])
            seen[base0 + x0] = 1
            min_x = max_x = x0
            min_y = max_y = y0
            while queue:
                x, y = queue.popleft()
                if x < min_x:
                    min_x = x
                elif x > max_x:
                    max_x = x
                if y < min_y:
                    min_y = y
                elif y > max_y:
                    max_y = y
                for nx, ny in ((x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)):
                    if 0 <= nx < width and 0 <= ny < height:
                        idx = ny * width + nx
                        if not seen[idx] and alpha[nx, ny] >= ALPHA_THRESHOLD:
                            seen[idx] = 1
                            queue.append((nx, ny))
            islands.append(
                {"x": min_x, "y": min_y, "w": max_x - min_x + 1, "h": max_y - min_y + 1}
            )

    # Reading order (top-to-bottom, left-to-right) => stable indices.
    islands.sort(key=lambda r: (r["y"], r["x"]))
    return islands


def keep(rect: dict, width: int, height: int) -> bool:
    if rect["w"] < MIN_SIDE or rect["h"] < MIN_SIDE:
        return False  # noise specks
    if rect["w"] >= FULLSCREEN_RATIO * width and rect["h"] >= FULLSCREEN_RATIO * height:
        return False  # full-screen backdrop / filler
    return True


def slice_atlas(filename: str) -> tuple[str, int, int]:
    path = ATLAS_DIR / filename
    if not path.exists():
        print(f"  !! missing atlas: {filename} (skipped)")
        return atlas_slug(filename), 0, 0

    im = Image.open(path).convert("RGBA")
    width, height = im.size
    slug = atlas_slug(filename)
    out_dir = OUT_ROOT / slug
    if out_dir.exists():
        shutil.rmtree(out_dir)  # full regen keeps runs deterministic

    rects = [r for r in find_islands(im) if keep(r, width, height)]
    if not rects:
        return slug, 0, 0  # nothing usable (e.g. one full-screen backdrop)

    out_dir.mkdir(parents=True)
    manifest = []
    for index, rect in enumerate(rects):
        name = f"{index}.png"
        im.crop(
            (rect["x"], rect["y"], rect["x"] + rect["w"], rect["y"] + rect["h"])
        ).save(out_dir / name, optimize=True)
        manifest.append({"file": name, **rect, "atlas": {"w": width, "h": height}})

    (out_dir / "manifest.json").write_text(
        json.dumps({"source": filename, "count": len(manifest), "sprites": manifest}, indent=2)
        + "\n"
    )
    return slug, len(manifest), len(rects)


def main(argv: list[str]) -> int:
    atlases = argv if argv else DEFAULT_ATLASES
    if not OUT_ROOT.exists():
        OUT_ROOT.mkdir(parents=True)

    total = 0
    print(f"Slicing {len(atlases)} atlases -> {OUT_ROOT}")
    for filename in atlases:
        slug, kept, found = slice_atlas(filename)
        total += kept
        print(f"  {slug:<28} {kept:>3} sprites ({found} islands found)")
    print(f"Done: {total} sprites extracted.")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
