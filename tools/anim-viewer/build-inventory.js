// Full inventory of animation clips and sprite textures.
// Reads clip data from all-animations.html (run build-all-viewer.js first) and scans every sprite .png.meta.
// Writes tools/anim-viewer/inventory/{README.md, animation-clips.csv, sprite-textures.csv}.
// Usage (from repo root): node tools/anim-viewer/build-inventory.js
const fs = require('fs');
const path = require('path');

const HERE = __dirname;
const ROOT = path.resolve(HERE, '..', '..');
const OUT = path.join(HERE, 'inventory');
const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');

const src = fs.readFileSync(path.join(HERE, 'all-animations.html'), 'utf8');
let json = src.slice(src.indexOf('const DATA = ') + 13, src.indexOf('\nconst EPS')).trim();
if (json.endsWith(';')) json = json.slice(0, -1);
const DATA = JSON.parse(json);

function walk(d, ext, o = []) {
  for (const e of fs.readdirSync(d, { withFileTypes: true })) {
    const p = path.join(d, e.name);
    if (e.isDirectory()) walk(p, ext, o); else if (e.name.endsWith(ext)) o.push(p);
  }
  return o;
}

// every sprite texture in the project
const tex = new Map(); // guid -> info
for (const m of walk(path.join(ROOT, 'Assets'), '.png.meta')) {
  const t = fs.readFileSync(m, 'utf8');
  if (!/textureType: 8\b/.test(t)) continue;
  const guid = (/^guid: (\w+)/m.exec(t) || [])[1];
  const mode = +((/spriteMode: (\d)/.exec(t) || [])[1] || 1);
  const n = mode === 2 ? (t.match(/^\s+- serializedVersion: 2\r?\n\s+name: /gm) || []).length : 1;
  const ppu = (/spritePixelsToUnits: (\S+)/.exec(t) || [])[1] || '';
  tex.set(guid, { kind: /_NormalMap\.png$/i.test(m.slice(0, -5)) ? 'normal map' : 'sprite', file: rel(m.slice(0, -5)), mode: mode === 2 ? 'multiple' : 'single', sprites: n, ppu, clips: new Set(), w: '', h: '' });
}
for (const e of tex.values()) {
  try {
    const fd = fs.openSync(path.join(ROOT, e.file), 'r'), b = Buffer.alloc(24);
    fs.readSync(fd, b, 0, 24, 0); fs.closeSync(fd);
    if (b.toString('ascii', 12, 16) === 'IHDR') { e.w = b.readUInt32BE(16); e.h = b.readUInt32BE(20); }
  } catch { /* missing or unreadable png */ }
}

const csv = (v) => { const s = String(v ?? ''); return /[",\n]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s; };
const fx = (n) => (Math.round(n * 1000) / 1000).toString();

// clips
const clipRows = [['group', 'folder', 'clip', 'dir', 'sample_rate', 'duration_s', 'loop', 'sprite_keys', 'unique_sprites', 'events', 'sprite_textures', 'controllers', 'file']];
const sets = new Map(); // group|folder -> aggregate
for (const c of DATA.clips) {
  const gs = [...new Set(c.frames.map((f) => f.g))];
  gs.forEach((g) => tex.get(g) && tex.get(g).clips.add(c.file));
  const sheets = gs.map((g) => (tex.get(g) ? tex.get(g).file : (DATA.textures[g] || {}).src || g));
  const uniq = new Set(c.frames.map((f) => f.g + f.id)).size;
  const evs = c.events.map((e) => `${e.fn}@${fx(e.t)}`).join(' ');
  clipRows.push([c.group, c.folder, c.name, c.nodir ? '' : c.dir, c.sampleRate, fx(c.stop), c.loop ? 1 : 0, c.frames.length, uniq, evs, sheets.join(' | '), (c.ctrl || []).join(' | '), c.file]);
  const key = c.group + '\u0000' + c.folder;
  const s = sets.get(key) || { group: c.group, folder: c.folder, clips: 0, dirs: new Set(), sr: new Set(), dur: new Set(), keys: new Set(), ev: new Set(), sheets: new Set(), ctrl: new Set(), empty: 0 };
  s.clips++;
  if (!c.nodir) s.dirs.add(c.dir);
  s.sr.add(c.sampleRate); s.dur.add(fx(c.stop)); s.keys.add(c.frames.length);
  if (!c.frames.length) s.empty++;
  s.ev.add(c.events.map((e) => e.fn).join(', ') || '-');
  sheets.forEach((x) => s.sheets.add(path.basename(path.dirname(x)) + '/' + path.basename(x).replace(/_?dir ?\d(?=\.png$)|Direction_\d(?=\.png$)/i, '*')));
  (c.ctrl || []).forEach((x) => s.ctrl.add(x));
  sets.set(key, s);
}
clipRows.sort((a, b) => (a === clipRows[0] ? -1 : b === clipRows[0] ? 1 : (a[0] + a[1] + a[2]).localeCompare(b[0] + b[1] + b[2])));

const texRows = [['texture', 'kind', 'sprite_mode', 'sprites', 'width', 'height', 'pixels_per_unit', 'used_by_clips', 'example_clip']];
const texList = [...tex.values()].sort((a, b) => a.file.localeCompare(b.file));
const spr = texList.filter((t) => t.kind === 'sprite');
for (const t of texList) texRows.push([t.file, t.kind, t.mode, t.sprites, t.w, t.h, t.ppu, t.clips.size, [...t.clips][0] || '']);

fs.mkdirSync(OUT, { recursive: true });
fs.writeFileSync(path.join(OUT, 'animation-clips.csv'), '﻿' + clipRows.map((r) => r.map(csv).join(',')).join('\r\n') + '\r\n');
fs.writeFileSync(path.join(OUT, 'sprite-textures.csv'), '﻿' + texRows.map((r) => r.map(csv).join(',')).join('\r\n') + '\r\n');

// markdown summary
const md = [];
const groups = [...new Set([...sets.values()].map((s) => s.group))].sort();
const list = (set, max = 4) => { const a = [...set]; return a.length <= max ? a.join(', ') : a.slice(0, max).join(', ') + ` (+${a.length - max})`; };
md.push('# Animation and Sprite Inventory', '');
md.push(`Generated ${new Date().toISOString().slice(0, 10)} by \`node tools/anim-viewer/build-inventory.js\` from \`all-animations.html\`.`, '');
md.push(`- Clips: ${DATA.clips.length} (${DATA.clips.filter((c) => !c.frames.length).length} without sprite keys)`);
md.push(`- Sprite textures: ${spr.length} (${spr.filter((t) => t.clips.size).length} used by a clip, ${spr.filter((t) => !t.clips.size).length} not used by any clip: UI, tiles, items, VFX, unused sheets) + ${texList.length - spr.length} normal maps`);
md.push('- Full per-clip detail: `animation-clips.csv`; full per-texture detail: `sprite-textures.csv`.', '');
md.push('## Clips by model', '');
md.push('| Model | Clips | Folders |', '|---|---:|---:|');
for (const g of groups) { const ss = [...sets.values()].filter((s) => s.group === g); md.push(`| ${g} | ${ss.reduce((n, s) => n + s.clips, 0)} | ${ss.length} |`); }
md.push('');
for (const g of groups) {
  md.push(`### ${g}`, '');
  md.push('| Folder | Clips | Dirs | FPS | Duration (s) | Keys | Events | Sprite sheet | Controller |', '|---|---:|---|---|---|---|---|---|---|');
  for (const s of [...sets.values()].filter((x) => x.group === g).sort((a, b) => a.folder.localeCompare(b.folder))) {
    const dirs = s.dirs.size ? [...s.dirs].sort().join('') : '-';
    md.push(`| ${s.folder} | ${s.clips}${s.empty ? ` (${s.empty} empty)` : ''} | ${dirs} | ${list(s.sr)} | ${list(s.dur, 3)} | ${list(s.keys, 3)} | ${[...s.ev].join(' / ')} | ${list(s.sheets, 2) || '-'} | ${list(s.ctrl, 2) || '-'} |`);
  }
  md.push('');
}
md.push('## Sprite textures by folder', '');
md.push('| Folder | Textures | Sprites | Used by a clip | Not used | Normal maps |', '|---|---:|---:|---:|---:|---:|');
const byDir = new Map();
for (const t of texList) {
  const d = path.dirname(t.file);
  const e = byDir.get(d) || { n: 0, sp: 0, used: 0, nm: 0 };
  if (t.kind === 'normal map') e.nm++;
  else { e.n++; e.sp += t.sprites; if (t.clips.size) e.used++; }
  byDir.set(d, e);
}
for (const [d, e] of [...byDir].sort()) md.push(`| ${d} | ${e.n} | ${e.sp} | ${e.used} | ${e.n - e.used} | ${e.nm} |`);
md.push('');
fs.writeFileSync(path.join(OUT, 'README.md'), md.join('\n'));
console.log(`${DATA.clips.length} clips, ${spr.length} sprite textures (${spr.filter((t) => !t.clips.size).length} not used by a clip), ${texList.length - spr.length} normal maps -> ${rel(OUT)}`);
