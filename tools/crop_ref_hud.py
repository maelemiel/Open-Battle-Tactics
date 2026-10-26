#!/usr/bin/env python3
"""Crop the real battle HUD bands from example_fight.jpg (1280x720, the
authentic gameplay screenshot). Output: two RGBA PNGs pre-scaled to the
960x540 game canvas (x0.75), pasted-ready:

  src/ui/assets/hud/ref/top.png     960x21  (top status bar, y 0..28 src)
  src/ui/assets/hud/ref/bottom.png  960x124 (command panel y 589..720 src,
                                            plus the raised right-side chrome
                                            x 930..1280, y 555..720 which
                                            overlaps the battlefield)

Rectangles were located by pixel analysis (luminance row transitions, gold
density column runs) on 2026-10-26; see docs/HUD_ASSETS.md "wave 4".
Deterministic: no thresholds in this script, fixed boxes only.
"""

from __future__ import annotations

from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parent.parent
SRC = REPO / "example_fight.jpg"
OUT = REPO / "src/ui/assets/hud/ref"
SCALE = 0.75

# Source rectangles (example_fight.jpg, 1280x720).
TOP_BAND = (0, 0, 1280, 28)
PANEL_LEFT = (0, 589, 930, 720)    # panel-only zone (nothing above y589)
PANEL_RIGHT = (930, 555, 1280, 720)  # raised chrome: gold strips + button
# The gold action button (a tall warm-gold slab, y ~545..719, x ~1183..1280).
BUTTON_REGION = (1178, 543, 1282, 720)


def is_gold(r: int, g: int, b: int) -> bool:
    """Warm gold-family pixel (covers the slab, its shading and baked text;
    excludes the navy panel and the green/tan battlefield)."""
    return r > 110 and r > b + 40 and g > r * 0.45 and g < r * 1.05


def crop_scaled(box: tuple[int, int, int, int]) -> Image.Image:
    return Image.open(SRC).convert("RGB").crop(box).resize(
        (int((box[2] - box[0]) * SCALE), int((box[3] - box[1]) * SCALE)),
        Image.LANCZOS,
    )


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)

    crop_scaled(TOP_BAND).save(OUT / "top.png")

    # Bottom: stitch left panel (lowered) + right raised chrome into one
    # 1280x165-src-space image (transparent above the left panel), then scale.
    stitched = Image.new("RGBA", (1280, 165), (0, 0, 0, 0))
    ref = Image.open(SRC).convert("RGB")
    stitched.paste(ref.crop(PANEL_LEFT), (0, 34))
    stitched.paste(ref.crop(PANEL_RIGHT), (930, 0))
    # Punch out the action-button slab: it ships as its own interactive
    # sprite (fight_button.png), so the band must not double-paint it.
    # Stitched-x maps 1:1 to source-x; stitched-y = source-y - 555 (the
    # PANEL_RIGHT paste origin).
    local = (
        BUTTON_REGION[0],
        max(BUTTON_REGION[1] - 555, 0),
        BUTTON_REGION[2],
        BUTTON_REGION[3] - 555,
    )
    stitched.paste((0, 0, 0, 0), (local[0], local[1], local[2], local[3]))
    stitched = stitched.resize((int(1280 * SCALE), int(165 * SCALE)), Image.LANCZOS)
    stitched.save(OUT / "bottom.png")

    # The gold action button as a standalone alpha sprite: keep warm-gold
    # pixels, drop navy panel / green battlefield, trim to content bbox.
    region = ref.crop(BUTTON_REGION)
    rgba = region.convert("RGBA")
    out = Image.new("RGBA", rgba.size, (0, 0, 0, 0))
    sp, op = rgba.load(), out.load()
    for y in range(rgba.height):
        for x in range(rgba.width):
            r, g, b, _ = sp[x, y]
            if is_gold(r, g, b):
                op[x, y] = (r, g, b, 255)
    bbox = out.getbbox()
    if bbox:
        out = out.crop(bbox)
    out = out.resize((int(out.width * SCALE), int(out.height * SCALE)), Image.LANCZOS)
    out.save(OUT / "fight_button.png")
    print(f"wrote top.png, bottom.png, fight_button.png {out.size} into {OUT}")


if __name__ == "__main__":
    main()
