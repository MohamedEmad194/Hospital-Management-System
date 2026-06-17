"""Hero JPEGs + transparent logo PNG from hospital facade photo (hospital-source.png)."""
from __future__ import annotations

import sys
from pathlib import Path

import numpy as np
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
PUBLIC = ROOT / "public"


def resolve_source() -> Path:
    if len(sys.argv) > 1:
        return Path(sys.argv[1]).expanduser()
    candidates = [
        PUBLIC / "hospital-source.png",
        PUBLIC / "hospital-source.jpg",
        PUBLIC / "hospital-building.jpg",
    ]
    for p in candidates:
        if p.exists():
            return p
    return PUBLIC / "hospital-source.png"


def main() -> int:
    src = resolve_source()
    if not src.exists():
        print("Missing facade image. Add public/hospital-source.png or pass a path.", file=sys.stderr)
        return 1

    img = Image.open(src).convert("RGB")
    w, h = img.size

    img.save(PUBLIC / "hospital-building.jpg", quality=92, optimize=True)
    img.save(PUBLIC / "hospital-hero.jpg", quality=92, optimize=True)
    about = PUBLIC / "images" / "about"
    about.mkdir(parents=True, exist_ok=True)
    img.save(about / "hospital-exterior.jpg", quality=92, optimize=True)

    left, right = int(w * 0.22), int(w * 0.78)
    top, bottom = int(h * 0.10), int(h * 0.44)
    crop = img.crop((left, top, right, bottom))
    arr = np.asarray(crop).astype(np.float32)
    rgb = arr[:, :, :3]

    ring = np.concatenate(
        [
            arr[0, :, :3].reshape(-1, 3),
            arr[-1, :, :3].reshape(-1, 3),
            arr[:, 0, :3].reshape(-1, 3),
            arr[:, -1, :3].reshape(-1, 3),
        ]
    )
    bg = np.median(ring, axis=0)

    mx = rgb.max(axis=2)
    mn = rgb.min(axis=2)
    sat = mx - mn
    lum = (mx + mn) / 2.0
    dist = np.linalg.norm(rgb - bg, axis=2)

    r, g, b = rgb[:, :, 0], rgb[:, :, 1], rgb[:, :, 2]

    # Light wall + sky (unchanged idea)
    is_panel = (dist < 42.0) & (sat < 38.0) & (lum > 120.0)
    is_bright = (lum > 200.0) & (sat < 55.0)

    # Foreground: brand blues, green leaves, white ECG — drop dark matte panel behind them
    is_neutral_dark = (sat < 24.0) & (lum < 105.0)
    is_green = (g > r + 14.0) & (g > b - 15.0) & (sat > 20.0)
    is_blue_brand = (b > r + 28.0) & (sat > 30.0)
    is_white_ecg = (lum > 188.0) & (sat < 40.0)
    keep_fg = (is_green | is_blue_brand | is_white_ecg) & ~is_neutral_dark

    alpha = np.where((is_panel | is_bright | ~keep_fg), 0, 255).astype(np.uint8)

    rgba = np.dstack([arr[:, :, :3].astype(np.uint8), alpha])
    logo_im = Image.fromarray(rgba, "RGBA")
    bbox = logo_im.getbbox()
    if bbox:
        logo_im = logo_im.crop(bbox)
    logo_im.save(PUBLIC / "hospital-logo.png", optimize=True)

    print("OK: hospital-building.jpg, hospital-hero.jpg, images/about/hospital-exterior.jpg, hospital-logo.png")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
