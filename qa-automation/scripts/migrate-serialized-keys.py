#!/usr/bin/env python3
"""Rename serialized YAML keys left behind by a field rename (QA-STATIC-03 findings).

Unity reads an asset by key name, so renaming the key in the .asset text moves the value
into the new field the next time the Editor imports it. A rule only fires inside the block
it names (parent key path suffix, optionally limited to one script GUID), so a key name that
is still valid elsewhere (e.g. DerivedStatFormula.targetStat) is never touched.

Usage (repo root):
  python qa-automation/scripts/migrate-serialized-keys.py            # dry run, prints changes
  python qa-automation/scripts/migrate-serialized-keys.py --apply    # writes the files
Then open Unity, let it import, and re-save the touched assets.
"""
import re
import sys
from pathlib import Path

# (script guid or None for any script, parent key path suffix, old key, new key, note)
RULES = [
    ("13e9d834fe92c2e479286dae6caa2c40", ("MonoBehaviour",), "attackDamege", "attackDamage",
     "AttackSO, BUG-105"),
    (None, ("statsModifiers",), "targetStat", "targetStatType",
     "StatModifier inside StatModifierGroup, BUG-121 / NOTE-20261103-2"),
    ("71c343067de33184e9bfce550a8e4826", ("statModifier",), "modifiers", "statsModifiers",
     "BuffDebuffStatsForDuration.statModifier (StatModifierGroup), pre-2026-08-21 name"),
    # 2026-11-09 audit: ItemSO / WeaponSO moved from public fields to [field: SerializeField] properties
    # (ItemSprite, ItemName) with no [FormerlySerializedAs]; the six PlatiumOre items lost their sprite.
    ("252a17c32f8cb8e408a37bd310fa064c", ("MonoBehaviour",), "itemSprite", "<ItemSprite>k__BackingField",
     "ItemSO.ItemSprite, serialized-rename audit 2026-11-09"),
    ("252a17c32f8cb8e408a37bd310fa064c", ("MonoBehaviour",), "nameItem", "<ItemName>k__BackingField",
     "ItemSO.ItemName, serialized-rename audit 2026-11-09"),
    ("f6aa03fbeb83ead488813503e17e3d23", ("MonoBehaviour",), "itemSprite", "<ItemSprite>k__BackingField",
     "WeaponSO : ItemSO, serialized-rename audit 2026-11-09"),
    ("f6aa03fbeb83ead488813503e17e3d23", ("MonoBehaviour",), "nameItem", "<ItemName>k__BackingField",
     "WeaponSO : ItemSO, serialized-rename audit 2026-11-09"),
]

KEY_RE = re.compile(r"^( *)(- )?([^\s:#][^:]*):(?: (.*))?$")
GUID_RE = re.compile(r"m_Script: \{fileID: \d+, guid: (\w+)")


def parse(lines):
    """Yield (index, key_indent, key, parent_path, parent_id) for every mapping key line."""
    stack = []  # (indent, key, line index)
    for i, raw in enumerate(lines):
        line = raw.rstrip("\r\n")
        if line.startswith("--- "):
            stack = []
            continue
        m = KEY_RE.match(line)
        if not m:
            continue
        indent = len(m.group(1)) + (2 if m.group(2) else 0)
        while stack and stack[-1][0] >= indent:
            stack.pop()
        path = tuple(s[1] for s in stack)
        parent_id = stack[-1][2] if stack else -1
        if m.group(2):
            parent_id = (parent_id, i)  # each list item is its own sibling scope
        yield i, indent, m.group(3), path, parent_id, m.group(2) is not None
        stack.append((indent, m.group(3), i))


def migrate(path, apply):
    lines = path.read_bytes().decode("utf-8").splitlines(keepends=True)

    # script guid of each YAML document ("--- !u!114 &id" ... "m_Script: {..., guid: X}")
    doc_guid, guid_of_line, current_doc, docs = {}, [], 0, {}
    for i, line in enumerate(lines):
        if line.startswith("--- "):
            current_doc = i
        g = GUID_RE.search(line)
        if g:
            docs.setdefault(current_doc, g.group(1))
        guid_of_line.append(current_doc)

    # sibling keys per scope, so a rename never produces a duplicate key
    entries, siblings, item_scope = [], {}, {}
    for i, indent, key, ppath, pid, is_item in parse(lines):
        if is_item:
            item_scope[(ppath, indent)] = pid
            scope = pid
        else:
            scope = item_scope.get((ppath, indent), pid)
        siblings.setdefault(scope, set()).add(key)
        entries.append((i, key, ppath, scope))

    changes, conflicts = [], []
    for i, key, ppath, scope in entries:
        for guid, suffix, old, new, note in RULES:
            if key != old or ppath[-len(suffix):] != suffix:
                continue
            if guid and docs.get(guid_of_line[i]) != guid:
                continue
            if new in siblings[scope]:
                conflicts.append((i + 1, old, new))
                continue
            lines[i] = lines[i].replace(old + ":", new + ":", 1)
            changes.append((i + 1, old, new, note))
    if changes and apply:
        path.write_bytes("".join(lines).encode("utf-8"))
    return changes, conflicts


def main():
    apply = "--apply" in sys.argv
    root = Path("Assets")
    total = 0
    for p in sorted(root.rglob("*")):
        if p.suffix not in (".asset", ".prefab", ".unity") or not p.is_file():
            continue
        try:
            changes, conflicts = migrate(p, apply)
        except UnicodeDecodeError:
            continue
        for ln, old, new, note in changes:
            print(f"{'RENAMED' if apply else 'WOULD RENAME'} {p}:{ln}  {old} -> {new}  ({note})")
        for ln, old, new in conflicts:
            print(f"SKIPPED {p}:{ln}  {old}: '{new}' already exists in the same block - fix by hand")
        total += len(changes)
    print(f"{total} key(s) {'renamed' if apply else 'to rename (dry run, pass --apply)'}")


if __name__ == "__main__":
    main()
