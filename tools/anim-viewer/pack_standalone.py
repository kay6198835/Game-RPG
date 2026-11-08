"""Builds character-animations-standalone.html: the character viewer with every sprite embedded.

Each used sprite is trimmed to its opaque bounds, optionally downscaled, packed into one WebP
atlas per source texture, and embedded as a data: URL, so the HTML opens on any device offline.

Usage (from repo root, after build-character-viewer.js):
    py -I tools/anim-viewer/pack_standalone.py [--scale 1.0] [--quality 80]
"""
import argparse
import base64
import io
import json
import os
import sys

from PIL import Image

HERE = os.path.dirname(os.path.abspath(__file__))
SRC = os.path.join(HERE, 'character-animations.html')
OUT = os.path.join(HERE, 'character-animations-standalone.html')
MARK = 'const DATA = '

ap = argparse.ArgumentParser()
ap.add_argument('--scale', type=float, default=1.0)
ap.add_argument('--quality', type=int, default=80)
ap.add_argument('--atlas-width', type=int, default=2048)
args = ap.parse_args()

html = open(SRC, encoding='utf-8').read()
start = html.index(MARK) + len(MARK)
end = html.index('\nconst EPS', start)
data = json.loads(html[start:end].rstrip().rstrip(';'))

total = 0
for gi, (guid, tex) in enumerate(data['textures'].items()):
    png = os.path.normpath(os.path.join(HERE, tex['src']))
    img = Image.open(png).convert('RGBA')
    pieces = []
    for sid, s in tex['sprites'].items():
        crop = img.crop((s['x'], s['y'], s['x'] + s['w'], s['y'] + s['h']))
        bbox = crop.getchannel('A').point(lambda a: 255 if a > 4 else 0).getbbox()
        if bbox is None:
            bbox = (0, 0, 1, 1)
        part = crop.crop(bbox)
        if args.scale != 1.0:
            part = part.resize((max(1, round(part.width * args.scale)), max(1, round(part.height * args.scale))),
                               Image.LANCZOS)
        pieces.append((sid, s, bbox, part))

    # shelf packing, tallest first
    pieces.sort(key=lambda p: -p[3].height)
    W = args.atlas_width
    x = y = shelf = 0
    placed = []
    for sid, s, bbox, part in pieces:
        if x + part.width > W:
            x, y, shelf = 0, y + shelf + 1, 0
        placed.append((sid, s, bbox, part, x, y))
        x += part.width + 1
        shelf = max(shelf, part.height)
    H = y + shelf
    W = max((px + p.width for _, _, _, p, px, _ in placed), default=1)
    atlas = Image.new('RGBA', (max(W, 1), max(H, 1)), (0, 0, 0, 0))
    for sid, s, bbox, part, px, py in placed:
        atlas.paste(part, (px, py))
        tex['sprites'][sid] = {
            'n': s['n'], 'x': px, 'y': py, 'w': part.width, 'h': part.height,
            'ow': s['w'], 'oh': s['h'], 'ox': bbox[0], 'oy': bbox[1],
            'tw': bbox[2] - bbox[0], 'th': bbox[3] - bbox[1],
            'px': s['px'], 'py': s['py'],
        }
    buf = io.BytesIO()
    atlas.save(buf, 'WEBP', quality=args.quality, method=6, alpha_quality=90)
    b = buf.getvalue()
    total += len(b)
    tex['file'] = tex['src']
    tex['src'] = 'data:image/webp;base64,' + base64.b64encode(b).decode()
    tex['w'], tex['h'] = atlas.size
    print(f'\r{gi + 1}/{len(data["textures"])} textures, {total / 1e6:.1f} MB', end='', file=sys.stderr)
print(file=sys.stderr)

data['standalone'] = True
out = html[:start] + json.dumps(data, separators=(',', ':')) + ';' + html[end:]

# trimmed sprites: draw the opaque sub-rect at its offset inside the original sprite rect
old_draw = "    ctx.drawImage(images[f.g], spr.x, spr.y, spr.w, spr.h, x, y, w, h);"
new_draw = ("    if (spr.ow) { const k = w / spr.ow; ctx.drawImage(images[f.g], spr.x, spr.y, spr.w, spr.h,"
            " x + spr.ox * k, y + spr.oy * k, spr.tw * k, spr.th * k); }\n"
            "    else ctx.drawImage(images[f.g], spr.x, spr.y, spr.w, spr.h, x, y, w, h);")
old_size = "    const w = spr.w * scale, h = spr.h * scale;"
new_size = "    const w = (spr.ow || spr.w) * scale, h = (spr.oh || spr.h) * scale;"
old_cov = "    coverage[g + ':' + id] = n * (s.w * s.h) / (MS * MS);"
new_cov = "    coverage[g + ':' + id] = n * ((s.tw || s.w) * (s.th || s.h)) / (MS * MS);"
for a, b in ((old_draw, new_draw), (old_size, new_size), (old_cov, new_cov)):
    if a not in out:
        sys.exit('template changed, patch target not found: ' + a.strip())
    out = out.replace(a, b)

open(OUT, 'w', encoding='utf-8').write(out)
print(f'{os.path.relpath(OUT)}: {os.path.getsize(OUT) / 1e6:.1f} MB (images {total / 1e6:.1f} MB)')
