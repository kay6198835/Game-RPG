#!/usr/bin/env python3
"""Type-aware static audit of serialized YAML keys (top level AND nested structs/lists).

For every MonoBehaviour block, walks the YAML as a tree, resolves each key's declared C# type through the
class index and reports keys that the (nested) type no longer declares. Read-only.
"""
import json, re, sys
from collections import defaultdict
from pathlib import Path

ROOT = Path(sys.argv[1]) if len(sys.argv) > 1 else Path(".")
ASSETS = ROOT / "Assets"
BUILTIN = {"m_ObjectHideFlags", "m_CorrespondingSourceObject", "m_PrefabInstance", "m_PrefabAsset", "m_GameObject",
           "m_Enabled", "m_EditorHideFlags", "m_Script", "m_Name", "m_EditorClassIdentifier", "serializedVersion"}
CLASS_RE = re.compile(r"\b(?:class|struct)\s+(\w+)(?:\s*<[^>{]*>)?\s*(?::\s*([^{]+?))?\s*(?:where [^{]+)?\{", re.S)
VALUE_TYPES = {"int", "float", "bool", "string", "double", "long", "byte", "short", "uint", "char", "Vector2", "Vector3", "Vector4",
               "Color", "Quaternion", "Rect", "LayerMask", "AnimationCurve", "Bounds", "Vector2Int", "Vector3Int", "object"}


def strip_comments(src):
    src = re.sub(r"/\*.*?\*/", "", src, flags=re.S)
    return re.sub(r"//[^\n]*", "", src)


def class_bodies(src):
    out = []
    for m in CLASS_RE.finditer(src):
        name, bases = m.group(1), m.group(2) or ""
        i, depth, body = m.end(), 1, []
        while i < len(src) and depth > 0:
            c = src[i]
            if c == "{":
                depth += 1
                if depth == 2:
                    body.append("{ get; }" if src[i + 1:i + 24].lstrip().startswith(("get", "set", "init")) else "{}")
            elif c == "}":
                depth -= 1
                if depth == 0:
                    break
            elif depth == 1:
                body.append(c)
            i += 1
        out.append((name, [re.sub(r"<.*", "", b.strip()).split(".")[-1] for b in bases.split(",") if b.strip()], "".join(body)))
    return out


def parse_members(body):
    fields, types = {}, {}
    for st in re.split(r"(?<=;)|(?<=\})", body):
        s = st.strip()
        if not s:
            continue
        m = re.match(r"^((?:\[[^\]]*\]\s*)*)(.*)$", s, re.S)
        attrs, rest = m.group(1), re.sub(r"\s+", " ", m.group(2).strip())
        if not rest or rest.startswith(("using ", "namespace ", "enum ", "delegate ", "event ", "class ", "struct ", "interface ")):
            continue
        alias = re.findall(r"FormerlySerializedAs\(\s*\"([^\"]+)\"", attrs)
        prop = re.match(r"^(?:(?:public|private|protected|internal|static|virtual|override|abstract|new) )*([\w<>\[\],\.\? ]+?) (\w+) ?\{ ?(?:get|set)", rest)
        if prop and "(" not in rest.split("{")[0]:
            if re.search(r"\[\s*field\s*:\s*(?:UnityEngine\.)?SerializeField", attrs):
                key = f"<{prop.group(2)}>k__BackingField"
                fields.setdefault(key, set()).update(alias)
                types[key] = prop.group(1).strip()
            continue
        if "(" in rest.split("=")[0]:
            continue
        fm = re.match(r"^((?:(?:public|private|protected|internal|static|readonly|const|volatile|new) )*)([\w<>\[\],\.\? ]+?) (\w+(?: ?, ?\w+)*) ?(?:=.*)?;?$", rest)
        if not fm:
            continue
        mods = fm.group(1).split()
        if {"static", "const", "readonly"} & set(mods):
            continue
        serialized = ("public" in mods and "NonSerialized" not in attrs) or bool(re.search(r"\bSerializeField\b|\bSerializeReference\b", attrs))
        if re.search(r"\bNonSerialized\b", attrs):
            serialized = False
        if not serialized:
            continue
        for n in (x.strip() for x in fm.group(3).split(",")):
            fields.setdefault(n, set()).update(alias)
            types[n] = fm.group(2).strip()
    return fields, types


def build_index():
    index, guid_to_file = {}, {}
    for meta in ASSETS.rglob("*.cs.meta"):
        g = re.search(r"guid: (\w+)", meta.read_text(errors="ignore"))
        if g:
            guid_to_file[g.group(1)] = Path(str(meta)[:-5])
    for cs in ASSETS.rglob("*.cs"):
        try:
            src = strip_comments(cs.read_text(errors="ignore"))
        except OSError:
            continue
        for name, bases, body in class_bodies(src):
            fields, types = parse_members(body)
            if name not in index or len(fields) > len(index[name]["fields"]):
                index[name] = {"fields": fields, "types": types, "bases": bases, "file": cs}
    return index, guid_to_file


def merged(name, index, seen=None):
    seen = seen or set()
    if name in seen or name not in index:
        return {}, {}
    seen.add(name)
    f = {k: set(v) for k, v in index[name]["fields"].items()}
    t = dict(index[name]["types"])
    for b in index[name]["bases"]:
        bf, bt = merged(b, index, seen)
        for k, v in bf.items():
            f.setdefault(k, set()).update(v)
        for k, v in bt.items():
            t.setdefault(k, v)
    return f, t


def element_type(t):
    t = t.strip().rstrip("?")
    m = re.match(r"^(?:List|IList|IReadOnlyList|HashSet)<(.+)>$", t)
    if m:
        return element_type(m.group(1))
    if t.endswith("[]"):
        return element_type(t[:-2])
    return t.split(".")[-1]


def parse_tree(lines):
    """Parse block YAML (indent based) into [(key, children|None, lineno)] lists."""
    def rec(i, indent):
        items = []
        while i < len(lines):
            line = lines[i]
            if not line.strip() or line.lstrip().startswith("#"):
                i += 1
                continue
            cur = len(line) - len(line.lstrip(" "))
            if cur < indent:
                break
            text = line.strip()
            if text.startswith("- "):
                # list entry; may open a mapping at indent+2
                body = text[2:]
                km = re.match(r"^([A-Za-z_<][^:]*):(?: (.*))?$", body)
                if km:
                    sub_lines = [" " * (cur + 2) + body] + []
                    j = i + 1
                    while j < len(lines):
                        l2 = lines[j]
                        if l2.strip() and (len(l2) - len(l2.lstrip(" "))) <= cur and not (l2.lstrip().startswith("- ") and False):
                            break
                        sub_lines.append(l2)
                        j += 1
                    sub, _ = parse_tree_block(sub_lines)
                    items.append(("-", sub, i))
                    i = j
                    continue
                items.append(("-", [], i))
                i += 1
                continue
            km = re.match(r"^([A-Za-z_<][^:]*):(?: (.*))?$", text)
            if not km:
                i += 1
                continue
            key = km.group(1)
            j = i + 1
            # children = following lines indented deeper (or list items at the same indent)
            while j < len(lines):
                l2 = lines[j]
                if not l2.strip():
                    j += 1
                    continue
                c2 = len(l2) - len(l2.lstrip(" "))
                if c2 > cur or (c2 == cur and l2.lstrip().startswith("- ")):
                    j += 1
                    continue
                break
            child_lines = lines[i + 1:j]
            sub, _ = parse_tree_block(child_lines) if child_lines else ([], None)
            items.append((key, sub, i))
            i = j
        return items, i
    return rec(0, 0)


def parse_tree_block(lines):
    if not lines:
        return [], 0
    base = min(len(l) - len(l.lstrip(" ")) for l in lines if l.strip())
    norm = [l[base:] if l.strip() else l for l in lines]
    return parse_tree(norm)


def check(tree, type_name, index, path, out, depth=0):
    fields, ftypes = merged(type_name, index)
    valid = set(fields)
    for al in fields.values():
        valid |= al
    for key, children, _ in tree:
        if key == "-":
            check(children, type_name, index, path, out, depth)
            continue
        if key in BUILTIN:
            continue
        if key not in valid:
            out.append((type_name, key, path))
            continue
        # resolve the declared type of this key (or of its alias target)
        decl = ftypes.get(key)
        if decl is None:
            for f, al in fields.items():
                if key in al:
                    decl = ftypes.get(f)
        if decl is None or depth > 6 or not children:
            continue
        et = element_type(decl)
        if et in VALUE_TYPES or et not in index or et in ("GameObject", "Transform"):
            continue
        # only recurse into plain serializable classes/structs (not UnityEngine.Object subclasses)
        check(children, et, index, path, out, depth + 1)


def main():
    index, guid_to_file = build_index()
    class_of_file = defaultdict(list)
    for name, d in index.items():
        class_of_file[d["file"]].append(name)
    hits = defaultdict(lambda: defaultdict(set))  # (type,key) -> files
    for ext in ("*.asset", "*.prefab", "*.unity"):
        for path in ASSETS.rglob(ext):
            rel = str(path.relative_to(ROOT)).replace("\\", "/")
            text = path.read_text(errors="ignore")
            for block in re.split(r"(?m)^--- ", text):
                if not block.startswith("!u!114 "):
                    continue
                g = re.search(r"m_Script: \{fileID: \d+, guid: (\w+)", block)
                cs = guid_to_file.get(g.group(1)) if g else None
                if cs is None:
                    continue
                names = class_of_file.get(cs, [])
                cls = next((n for n in names if n == cs.stem), names[0] if names else None)
                if cls is None:
                    continue
                lines = block.splitlines()[1:]
                if lines and lines[0].startswith("MonoBehaviour:"):
                    lines = lines[1:]
                tree, _ = parse_tree_block(lines)
                out = []
                check(tree, cls, index, rel, out)
                for t, k, p in out:
                    hits[(t, k)]["files"].add(p)
    res = {f"{t}.{k}": sorted(v["files"]) for (t, k), v in sorted(hits.items())}
    json.dump(res, sys.stdout, indent=1, ensure_ascii=False)


main()
