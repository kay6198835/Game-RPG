import json, re, sys
from collections import Counter, defaultdict

data = json.load(open(sys.argv[1]))
tpl = open(sys.argv[2], encoding="utf-8").read()
out = sys.argv[3]

tex_list = []
tex_idx = {}
def ti(g):
    if g not in tex_idx:
        tex_idx[g] = len(tex_list)
        t = data["textures"][g]
        tex_list.append([t["path"], t["ppu"]])
    return tex_idx[g]

def context(path):
    p = path.split("/")
    if p[1] == "Animation":
        if p[2] == "Player": return "Player · " + p[3]
        if p[2] == "Enemy" and p[3] == "Role": return "Enemy Role · " + p[4]
        if p[2] == "Enemy": return "Enemy · " + p[3]
        return "VFX · " + (p[3] if len(p) > 4 else p[2])
    if p[1] == "Sprite":
        return "Nguồn (Sprite/" + p[2] + "/" + p[3] + ")"
    return p[1]

DIR = re.compile(r"[ _]?dir[ _]?(\d)", re.I)
groups = defaultdict(list)
for c in data["clips"]:
    model = c["model"]
    if model == "(không có sprite)":
        parts = c["path"].split("/")
        model = parts[2] if parts[1] == "Sprite" else model
    action = DIR.sub("", c["name"]).strip(" _")
    folder = c["folder"]
    frames = []
    for t, g, s in c["frames"]:
        if g is None: frames.append([t, -1]); continue
        if s is None: frames.append([t, ti(g)]); continue
        if s == "?": frames.append([t, ti(g), "?"]); continue
        frames.append([t, ti(g), int(s[0]), int(s[1]), int(s[2]), int(s[3])])
    groups[(model, context(c["path"]), folder, action)].append({
        "n": c["name"], "p": c["path"], "d": c["dir"], "fps": c["fps"], "len": c["len"],
        "loop": c["loop"], "ev": c["events"], "u": c["used"], "f": frames, "m": c["missing"]})

models = defaultdict(lambda: defaultdict(list))
for (model, ctx, folder, action), clips in groups.items():
    clips.sort(key=lambda x: (x["d"] if x["d"] is not None else 99, x["n"]))
    models[model][ctx].append({"folder": folder, "action": action, "clips": clips})

def mkey(m):
    return (m.startswith("("), m.startswith("VFX"), m.startswith("Enemy /") or m.startswith("Player /"), m)
out_models = []
for m in sorted(models, key=mkey):
    ctxs = []
    for ctx in sorted(models[m], key=lambda c: (c.startswith("Nguồn"), not c.startswith("Player"), c)):
        acts = sorted(models[m][ctx], key=lambda a: (a["folder"], a["action"]))
        ctxs.append({"ctx": ctx, "actions": acts})
    out_models.append({"model": m, "ctxs": ctxs})

payload = json.dumps({"tex": tex_list, "models": out_models}, ensure_ascii=False, separators=(",", ":"))
payload = payload.replace("</", "<\\/")
open(out, "w", encoding="utf-8").write(tpl.replace("/*__DATA__*/null", payload))
print("written", len(payload) // 1024, "KB", len(out_models), "models")
