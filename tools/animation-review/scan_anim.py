import os, re, json, struct, sys
from collections import defaultdict, Counter

ROOT = sys.argv[1]  # repo root
ASSETS = os.path.join(ROOT, "Assets")
OUT = sys.argv[2]

guid_path = {}
for dp, dn, fn in os.walk(ASSETS):
    for f in fn:
        if f.endswith(".meta"):
            p = os.path.join(dp, f)
            try:
                with open(p, encoding="utf-8", errors="ignore") as fh:
                    head = fh.read(400)
                m = re.search(r"^guid: ([0-9a-f]{32})", head, re.M)
                if m:
                    guid_path[m.group(1)] = os.path.relpath(p[:-5], ROOT).replace(os.sep, "/")
            except Exception:
                pass

def png_size(path):
    try:
        with open(path, "rb") as f:
            h = f.read(24)
        if h[:8] == b"\x89PNG\r\n\x1a\n":
            return struct.unpack(">II", h[16:24])
    except Exception:
        pass
    return None

tex_cache = {}
def texture(guid):
    if guid in tex_cache:
        return tex_cache[guid]
    rel = guid_path.get(guid)
    info = None
    if rel and os.path.exists(os.path.join(ROOT, rel + ".meta")):
        meta = open(os.path.join(ROOT, rel + ".meta"), encoding="utf-8", errors="ignore").read()
        size = png_size(os.path.join(ROOT, rel)) if rel.lower().endswith(".png") else None
        ppu = re.search(r"spritePixelsToUnits: ([\d.]+)", meta)
        mode = re.search(r"spriteMode: (\d)", meta)
        sprites = {}
        for blk in re.split(r"\n    - serializedVersion: 2\n      name: ", meta)[1:]:
            r = re.search(r"x: ([\d.]+)\s+y: ([\d.]+)\s+width: ([\d.]+)\s+height: ([\d.]+)", blk)
            iid = re.search(r"internalID: (-?\d+)", blk)
            pv = re.search(r"pivot: \{x: ([-\d.e]+), y: ([-\d.e]+)\}", blk)
            if r and iid:
                x, y, w, hh = map(float, r.groups())
                sprites[iid.group(1)] = [x, y, w, hh, float(pv.group(1)) if pv else .5, float(pv.group(2)) if pv else .5]
        info = {"path": rel, "w": size[0] if size else None, "h": size[1] if size else None,
                "ppu": float(ppu.group(1)) if ppu else None, "mode": int(mode.group(1)) if mode else None,
                "sprites": sprites}
    tex_cache[guid] = info
    return info

# controllers
usage = defaultdict(set)
for dp, dn, fn in os.walk(ASSETS):
    for f in fn:
        if f.endswith(".controller") or f.endswith(".overrideController"):
            p = os.path.join(dp, f)
            txt = open(p, encoding="utf-8", errors="ignore").read()
            name = os.path.relpath(p, ASSETS).replace(os.sep, "/")
            for g in re.findall(r"(?:m_Motion|m_OriginalClip|m_OverrideClip): \{fileID: 7400000, guid: ([0-9a-f]{32})", txt):
                usage[g].add(name)

PPTR = re.compile(r"- time: ([-\d.e]+)\s+value: \{fileID: (-?\d+), guid: ([0-9a-f]{32})")
clips = []
for dp, dn, fn in os.walk(ASSETS):
    for f in fn:
        if not f.endswith(".anim"):
            continue
        p = os.path.join(dp, f)
        txt = open(p, encoding="utf-8", errors="ignore").read()
        rel = os.path.relpath(p, ROOT).replace(os.sep, "/")
        mg = re.search(r"^guid: ([0-9a-f]{32})", open(p + ".meta").read(), re.M) if os.path.exists(p + ".meta") else None
        guid = mg.group(1) if mg else None
        sec = txt.split("m_PPtrCurves:", 1)[1].split("m_SampleRate:", 1)[0] if "m_PPtrCurves:" in txt else ""
        sec = sec.split("m_EditorCurves")[0] if False else sec
        # only first block (stop at m_SampleRate) — PPtr curves live there
        frames = []
        for t, fid, g in PPTR.findall(sec):
            frames.append([round(float(t), 4), g, fid])
        sr = re.search(r"m_SampleRate: ([\d.]+)", txt)
        stop = re.search(r"m_StopTime: ([-\d.e]+)", txt)
        loop = re.search(r"m_LoopTime: (\d)", txt)
        events = re.findall(r"functionName: (\S+)", txt)
        has_other = bool(re.search(r"m_(PositionCurves|ScaleCurves|EulerCurves|FloatCurves): \n\s+- ", txt))
        texs = Counter(g for _, g, _ in frames)
        clips.append({"path": rel, "guid": guid, "name": os.path.splitext(f)[0],
                      "fps": float(sr.group(1)) if sr else None,
                      "len": float(stop.group(1)) if stop else None,
                      "loop": loop.group(1) == "1" if loop else None,
                      "events": events, "frames": frames, "otherCurves": has_other,
                      "used": sorted(usage.get(guid, []))})

# model resolution: from sprite texture path
def model_of(tex_path, clip_path):
    if tex_path:
        parts = tex_path.split("/")
        if len(parts) > 2 and parts[1] == "Sprite":
            if parts[2] in ("Enemy", "Player", "NPC", "VFX") and len(parts) > 4:
                return parts[2] + " / " + parts[3]
            return parts[2]
        return "/".join(parts[1:3])
    return "(không có sprite)"

textures = {}
for c in clips:
    texs = Counter(g for _, g, _ in c["frames"])
    main = texs.most_common(1)[0][0] if texs else None
    ti = texture(main) if main else None
    c["model"] = model_of(ti["path"] if ti else None, c["path"]) if main else "(không có sprite)"
    if main and not ti:
        c["model"] = "(sprite bị mất)"
    missing = 0
    fr = []
    for t, g, fid in c["frames"]:
        tinfo = texture(g)
        if not tinfo:
            missing += 1; fr.append([t, None, None]); continue
        textures.setdefault(g, {k: tinfo[k] for k in ("path", "w", "h", "ppu", "mode")})
        if tinfo["mode"] == 1 or fid == "21300000":
            fr.append([t, g, None])
        else:
            s = tinfo["sprites"].get(fid)
            if s is None:
                missing += 1; fr.append([t, g, "?"])
            else:
                fr.append([t, g, s])
    c["frames"] = fr
    c["missing"] = missing
    m = re.search(r"dir[ _]?(\d)", c["name"], re.I)
    c["dir"] = int(m.group(1)) if m else None
    c["folder"] = "/".join(c["path"].split("/")[1:-1])

json.dump({"clips": clips, "textures": textures}, open(OUT, "w"), separators=(",", ":"))
print(len(clips), "clips", len(textures), "textures", Counter(c["model"] for c in clips).most_common())
