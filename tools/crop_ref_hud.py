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
    stitched = stitched.resize((int(1280 * SCALE), int(165 * SCALE)), Image.LANCZOS)
    stitched.save(OUT / "bottom.png")
    print(f"wrote {OUT/'top.png'} and {OUT/'bottom.png'}")


if __name__ == "__main__":
    main()
