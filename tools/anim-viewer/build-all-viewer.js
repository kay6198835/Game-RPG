// Generates all-animations.html: every .anim clip under Assets/, grouped by model.
// Usage (from repo root): node tools/anim-viewer/build-all-viewer.js
// Images are referenced by relative path, so keep the HTML inside tools/anim-viewer/.
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..', '..');
const ASSETS = path.join(ROOT, 'Assets');
const OUT = path.join(__dirname, 'all-animations.html');

function walk(dir, ext, out = []) {
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, e.name);
    if (e.isDirectory()) walk(p, ext, out);
    else if (e.name.endsWith(ext)) out.push(p);
  }
  return out;
}
const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');
const guidOf = (metaPath) => {
  try { return (/^guid: ([0-9a-f]{32})/m.exec(fs.readFileSync(metaPath, 'utf8')) || [])[1]; } catch { return null; }
};

const guidToPng = new Map();
for (const m of walk(ASSETS, '.png.meta')) {
  const g = guidOf(m);
  if (g) guidToPng.set(g, m.slice(0, -5));
}

const textures = {};
function loadTexture(guid) {
  if (guid in textures) return textures[guid];
  const png = guidToPng.get(guid);
  if (!png) return (textures[guid] = null);
  const buf = fs.readFileSync(png);
  const texW = buf.readUInt32BE(16);
  const texH = buf.readUInt32BE(20);
  const meta = fs.readFileSync(png + '.meta', 'utf8');
  const sprites = {};
  const byName = {};
  const re = /- serializedVersion: 2\s+name: (.+?)\s+rect:\s+serializedVersion: 2\s+x: ([\d.]+)\s+y: ([\d.]+)\s+width: ([\d.]+)\s+height: ([\d.]+)\s+alignment: (\d+)\s+pivot: \{x: ([-\d.e]+), y: ([-\d.e]+)\}[\s\S]*?internalID: (-?\d+)/g;
  let m;
  while ((m = re.exec(meta))) {
    const [, name, x, y, w, h, , px, py, id] = m;
    const s = { n: name, x: +x, y: texH - +y - +h, w: +w, h: +h, px: +px, py: +py };
    byName[name] = s;
    if (id !== '0') sprites[id] = s;
  }
  // Older importer format: internalID 0, ids live in internalIDToNameTable / fileIDToRecycleName
  const tbl = /- first:\s+213: (-?\d+)\s+second: (.+)/g;
  while ((m = tbl.exec(meta))) if (byName[m[2].trim()]) sprites[m[1]] = byName[m[2].trim()];
  const recycle = /^\s+(213\d+): (.+)$/gm;
  while ((m = recycle.exec(meta))) if (byName[m[2].trim()]) sprites[m[1]] = byName[m[2].trim()];
  if (/spriteMode: 1/.test(meta)) {
    const pv = /spritePivot: \{x: ([-\d.e]+), y: ([-\d.e]+)\}/.exec(meta) || [, 0.5, 0.5];
    sprites['21300000'] = { n: path.basename(png, '.png'), x: 0, y: 0, w: texW, h: texH, px: +pv[1], py: +pv[2] };
  }
  return (textures[guid] = { src: '../../' + rel(png), w: texW, h: texH, sprites });
}

// clip guid -> controller names that reference it
const clipUsers = new Map();
for (const f of [...walk(ASSETS, '.controller'), ...walk(ASSETS, '.overrideController')]) {
  const name = path.basename(f);
  for (const g of new Set(fs.readFileSync(f, 'utf8').match(/guid: [0-9a-f]{32}/g) || [])) {
    const k = g.slice(6);
    if (!clipUsers.has(k)) clipUsers.set(k, []);
    clipUsers.get(k).push(name);
  }
}

// Model = owner folder of the clip
function classify(file) {
  const p = rel(file).split('/').slice(1);
  const pick = (order, label, n) => ({ order, group: label, base: p.slice(0, n).join('/') });
  if (p[0] === 'Animation' && p[1] === 'Player') return pick(0, `Player · ${p[2]}`, 3);
  if (p[0] === 'Animation' && p[1] === 'Enemy' && p[2] === 'Role') return pick(1, `Enemy role · ${p[3]}`, 4);
  if (p[0] === 'Animation' && p[1] === 'Enemy') return pick(2, `Enemy · ${p[2]}`, 3);
  if (p[0] === 'Sprite' && p[1] === 'VFX') return pick(4, 'VFX · Sprite/VFX', 2);
  if (p[0] === 'Sprite') return pick(3, `Model · ${p[1]}`, 2);
  if (p[0] === 'Animation' && p[1] === 'Particle Effect') return pick(4, 'VFX · Particle Effect', 2);
  return pick(5, `Other · ${p.slice(0, -1).join('/')}`, p.length - 1);
}

const DIR_RE = /[_ ]?dir ?(\d)$/i;
const raw = [];
for (const file of walk(ASSETS, '.anim').sort()) {
  const txt = fs.readFileSync(file, 'utf8');
  const name = (/m_Name: (.+)/.exec(txt) || [, path.basename(file, '.anim')])[1].trim();
  const sampleRate = +(/m_SampleRate: ([\d.]+)/.exec(txt) || [, 60])[1];
  const stop = +(/m_StopTime: ([\d.e-]+)/.exec(txt) || [, 0])[1];
  const loop = /m_LoopTime: 1/.test(txt);

  const pptr = txt.slice(txt.indexOf('m_PPtrCurves:'), txt.indexOf('m_SampleRate:'));
  const frames = [];
  const kre = /- time: ([\d.e-]+)\s+value: \{fileID: (-?\d+), guid: ([0-9a-f]{32}),/g;
  let k;
  while ((k = kre.exec(pptr))) {
    const tex = loadTexture(k[3]);
    frames.push({ t: +k[1], g: k[3], id: k[2], missing: !(tex && tex.sprites[k[2]]) });
  }
  frames.sort((a, b) => a.t - b.t);

  const curveAttrs = new Set();
  const bindings = txt.slice(txt.indexOf('m_ClipBindingConstant:') > 0 ? 0 : 0, txt.indexOf('m_AnimationClipSettings:'));
  for (const a of bindings.matchAll(/attribute: (\S+)/g)) curveAttrs.add(a[1]);
  for (const a of txt.matchAll(/^\s+path: (.+)$/gm)) if (a[1].trim()) curveAttrs.add('bone:' + a[1].trim().split('/').pop());
  const boneCount = [...curveAttrs].filter((a) => a.startsWith('bone:')).length;
  const attrs = [...curveAttrs].filter((a) => !a.startsWith('bone:') && a !== 'm_Sprite');
  const curves = frames.length ? '' : [boneCount ? `${boneCount} bone` : '', ...attrs.slice(0, 6)].filter(Boolean).join(', ') || 'không có curve';

  const events = [];
  const evBlock = txt.slice(txt.indexOf('m_Events:'));
  for (const e of evBlock.matchAll(/- time: ([\d.e-]+)\s+functionName: (\S+)/g)) events.push({ t: +e[1], fn: e[2] });

  const cls = classify(file);
  const relDir = path.relative(path.join(ASSETS, cls.base), path.dirname(file)).split(path.sep).filter(Boolean).join(' / ');
  const dm = DIR_RE.exec(name);
  const guid = guidOf(file + '.meta');
  raw.push({
    ...cls, relDir, name, dir: dm ? +dm[1] : -1, stem: dm ? name.slice(0, dm.index) : name,
    sampleRate, stop, loop, frames, events, curves, file: rel(file),
    ctrl: (clipUsers.get(guid) || []).filter((n, i, a) => a.indexOf(n) === i),
  });
}

// directional clips sharing a folder with a single stem keep the folder name; otherwise split by stem
const stemsPerDir = new Map();
for (const c of raw) if (c.dir >= 0) {
  const key = c.group + '|' + c.relDir;
  if (!stemsPerDir.has(key)) stemsPerDir.set(key, new Set());
  stemsPerDir.get(key).add(c.stem);
}
const join = (...a) => a.filter(Boolean).join(' / ');
const clips = raw.map((c) => {
  let folder, nodir = false;
  if (c.dir >= 0) folder = stemsPerDir.get(c.group + '|' + c.relDir).size === 1 && c.relDir ? c.relDir : join(c.relDir, c.stem);
  else { folder = join(c.relDir, c.name); nodir = true; }
  return {
    order: c.order, group: c.group, folder, name: c.name, dir: nodir ? 0 : c.dir, nodir,
    sampleRate: c.sampleRate, stop: c.stop, loop: c.loop, frames: c.frames, events: c.events,
    curves: c.curves, ctrl: c.ctrl, file: c.file,
  };
});
clips.sort((a, b) => a.order - b.order || a.group.localeCompare(b.group) || a.folder.localeCompare(b.folder, undefined, { numeric: true }) || a.dir - b.dir);

const used = new Set(clips.flatMap((c) => c.frames.map((f) => f.g + ':' + f.id)));
for (const [g, t] of Object.entries(textures)) {
  if (!t) { delete textures[g]; continue; }
  for (const id of Object.keys(t.sprites)) if (!used.has(g + ':' + id)) delete t.sprites[id];
}
for (const c of clips) delete c.order;

// The shared template is patched at build time so the other viewers stay untouched.
// 450+ sheets (~1.7 Gpx) cannot all be decoded at once: load the sheets of the selected folder only.
const LAZY_LOADER = `const images = {};
let redrawTimer = 0;
function ensureImages(f) {
  for (const c of f.clips) if (c) for (const fr of c.frames) {
    const g = fr.g;
    if (images[g] || !DATA.textures[g]) continue;
    const img = new Image();
    img.onload = () => {
      try { measureTexture(g); } catch (e) { coverageOk = false; }
      clearTimeout(redrawTimer);
      redrawTimer = setTimeout(() => { updateStats(); renderList(); if (state.folder) updateSide(); }, 60);
    };
    img.src = DATA.textures[g].src;
    images[g] = img;
  }
}`;
const patches = [
  [/Archer Animation Check/g, 'All Animations Check'],
  [/const images = \{\};\nlet pending[\s\S]*?\n\}\n/, () => LAZY_LOADER + '\n'],
  ['if (spr && images[f.g].complete)', 'if (spr && images[f.g] && images[f.g].complete)'],
  ['state.folder = f; state.t = 0;', 'state.folder = f; state.t = 0; ensureImages(f);'],
  [/archerAnimFolder/g, 'allAnimFolder'],
  // small pixel-art sprites (Bat, Rat...) have < 200 opaque px by design
  ['if (v < EMPTY_PX) n++;', 'const s = DATA.textures[f.g].sprites[f.id]; if (v < Math.min(EMPTY_PX, 0.03 * s.w * s.h)) n++;'],
  ['const hold = c.stop - (lk + frameDur);', 'const hold = c.frames.length ? c.stop - (lk + frameDur) : 0;'],
  ['<label><input type="checkbox" id="onlyIssues">',
    '<select id="modelSel" style="max-width:150px"><option value="">Tất cả model</option></select> <label><input type="checkbox" id="onlyIssues">'],
  ["const listEl = document.getElementById('list');",
    "const listEl = document.getElementById('list');\nconst modelSel = document.getElementById('modelSel');\n" +
    "for (const g of new Set(DATA.clips.map(c => c.group))) modelSel.insertAdjacentHTML('beforeend', `<option>${g}</option>`);\n" +
    "modelSel.onchange = () => { renderList(); const f = [...folders.values()].find(x => !modelSel.value || x.group === modelSel.value); if (f) selectFolder(f); };"],
  ['if (only && !iss.length) continue;', 'if (only && !iss.length) continue;\n    if (modelSel.value && f.group !== modelSel.value) continue;'],
  ['clips: [] });', 'clips: [], nodir: !!c.nodir });'],
  ["for (let d = 0; d < 8; d++) if (!f.clips[d])", "if (!f.nodir) for (let d = 0; d < 8; d++) if (!f.clips[d])"],
  ["if (!c.frames.length) out.push(['bad', 'Clip không có keyframe sprite nào.']);",
    "if (!c.frames.length) out.push(c.curves && c.curves !== 'không có curve' ? ['warn', 'Clip không dùng sprite (curve: ' + c.curves + ') — không xem trước được ở đây.'] : ['bad', 'Clip không có keyframe sprite nào.']);\n" +
    "  if (!c.ctrl.length) out.push(['warn', 'Không Animator Controller / Override nào tham chiếu clip này (có thể thừa).']);"],
  ["<dt>Hướng</dt><dd>dir${state.dir} — ${DIRS[state.dir][0]}</dd>",
    "<dt>Hướng</dt><dd>${f.nodir ? 'không chia hướng' : 'dir' + state.dir + ' — ' + DIRS[state.dir][0]}</dd>\n    <dt>Controller</dt><dd class=\"mono\">${c.ctrl.length ? c.ctrl.join('<br>') : '—'}</dd>"],
];
let tpl = fs.readFileSync(path.join(__dirname, 'viewer-template.html'), 'utf8').replace(/\r\n/g, '\n');
for (const [from, to] of patches) {
  const before = tpl;
  tpl = tpl.replace(from, to);
  if (tpl === before) throw new Error('template patch did not apply: ' + String(from).slice(0, 60));
}
const data = { generated: new Date().toISOString(), clips, textures };
fs.writeFileSync(OUT, tpl.replace('/*__DATA__*/null', () => JSON.stringify(data)));

if (process.argv.includes('--report')) {
  const rep = {};
  for (const c of clips) {
    const n = c.frames.filter((f) => f.missing);
    if (n.length) rep[`${c.group} | ${c.folder}`] = `${n.length} (${n[0].g}:${n[0].id}${guidToPng.has(n[0].g) ? '' : ' no-png'})`;
  }
  console.log(rep);
}
const missing = clips.reduce((n, c) => n + c.frames.filter((f) => f.missing).length, 0);
const groups = new Set(clips.map((c) => c.group));
const px = Object.values(textures).reduce((n, t) => n + t.w * t.h, 0);
console.log(`${clips.length} clips, ${groups.size} models, ${Object.keys(textures).length} textures (${(px / 1e6).toFixed(0)} Mpx), ${missing} missing sprite refs, ${clips.filter((c) => !c.ctrl.length).length} clips not in any controller -> ${rel(OUT)}`);
