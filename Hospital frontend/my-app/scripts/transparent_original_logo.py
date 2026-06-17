"""
Remove light grey background + soft shadow from the original hospital logo PNG
(Images/hospital-logo.png) via edge flood-fill, then write public/hospital-logo.png.
"""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

import numpy as np
from PIL import Image

# my-app/scripts -> parents[3] = repo root (HMS)
_ROOT = Path(__file__).resolve().parents[3]
DEFAULT_SRC = _ROOT / "Images" / "hospital-logo.png"
PUBLIC = Path(__file__).resolve().parents[1] / "public"


def main() -> int:
    src = Path(sys.argv[1]).expanduser() if len(sys.argv) > 1 else DEFAULT_SRC
    if not src.exists():
        print("Missing source:", src, file=sys.stderr)
        return 1

    im = Image.open(src).convert("RGB")
    rgb = np.asarray(im, dtype=np.float32)
    h, w = rgb.shape[:2]

    border = np.concatenate(
        [rgb[0].reshape(-1, 3), rgb[-1].reshape(-1, 3), rgb[:, 0].reshape(-1, 3), rgb[:, -1].reshape(-1, 3)]
    )
    bg = np.median(border, axis=0)

    dist = np.linalg.norm(rgb - bg, axis=2)
    mx = rgb.max(axis=2)
    mn = rgb.min(axis=2)
    sat = mx - mn
    lum = (mx + mn) / 2.0

    # Strong brand colours — never treat as background
    r, g, b = rgb[:, :, 0], rgb[:, :, 1], rgb[:, :, 2]
    is_blue = (b > r + 18.0) & (sat > 12.0)
    is_green = (g > r + 10.0) & (g > b - 6.0) & (sat > 10.0)
    is_gold = (r > 165.0) & (g > 115.0) & (b < 145.0) & (lum > 85.0)
    is_dark_text = (lum < 92.0) & (sat < 55.0)

    protected = is_blue | is_green | is_gold | is_dark_text

    # Edge flood: stay tight to border grey so we do not leak into logo whites / 3D shading
    seed_tol = 12.0
    grow_tol = 12.0
    outside = np.zeros((h, w), dtype=bool)
    q: deque[tuple[int, int]] = deque()

    def try_seed(y: int, x: int) -> None:
        if outside[y, x] or protected[y, x]:
            return
        if dist[y, x] <= seed_tol:
            outside[y, x] = True
            q.append((y, x))

    for x in range(w):
        try_seed(0, x)
        try_seed(h - 1, x)
    for y in range(h):
        try_seed(y, 0)
        try_seed(y, w - 1)

    while q:
        y, x = q.popleft()
        for ny, nx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
            if ny < 0 or ny >= h or nx < 0 or nx >= w or outside[ny, nx] or protected[ny, nx]:
                continue
            if dist[ny, nx] <= grow_tol:
                outside[ny, nx] = True
                q.append((ny, nx))

    # Soft shadow under emblem: neutral grey, not part of logo chroma
    neutral = (sat < 32.0) & (lum > 88.0) & (lum < 208.0) & (r > 92.0) & (g > 92.0) & (b > 92.0)
    shadow = neutral & ~protected

    transparent = outside | shadow
    alpha = np.where(transparent, 0, 255).astype(np.uint8)

    out = np.dstack([rgb.astype(np.uint8), alpha])
    logo = Image.fromarray(out, "RGBA")
    bbox = logo.getbbox()
    if bbox:
        logo = logo.crop(bbox)

    PUBLIC.mkdir(parents=True, exist_ok=True)
    out_path = PUBLIC / "hospital-logo.png"
    logo.save(out_path, optimize=True)
    print("Wrote", out_path, "from", src)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
