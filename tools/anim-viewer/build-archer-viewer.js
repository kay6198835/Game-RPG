// Generates archer-animations.html: a standalone viewer for every Archer sprite clip.
// Usage (from repo root): node tools/anim-viewer/build-archer-viewer.js
// Images are referenced by relative path, so keep the HTML inside tools/anim-viewer/.
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..', '..');
const CLIP_DIR = path.join(ROOT, 'Assets/Animation/Player/Archer');
const SPRITE_ROOT = path.join(ROOT, 'Assets/Sprite');
const OUT = path.join(__dirname, 'archer-animations.html');

function walk(dir, ext, out = []) {
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, e.name);
    if (e.isDirectory()) walk(p, ext, out);
    else if (e.name.endsWith(ext)) out.push(p);
  }
  return out;
}

const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');

// guid -> png path, built lazily from every .png.meta under Assets/Sprite
const guidToPng = new Map();
for (const m of walk(SPRITE_ROOT, '.png.meta')) {
  const g = /^guid: ([0-9a-f]{32})/m.exec(fs.readFileSync(m, 'utf8'));
  if (g) guidToPng.set(g[1], m.slice(0, -5));
}

const textures = {};
function loadTexture(guid) {
  if (textures[guid]) return textures[guid];
  const png = guidToPng.get(guid);
  if (!png) return null;
  const buf = fs.readFileSync(png);
  const texW = buf.readUInt32BE(16);
  const texH = buf.readUInt32BE(20);
  const meta = fs.readFileSync(png + '.meta', 'utf8');
  const sprites = {};
  const re = /- serializedVersion: 2\s+name: (\S+)\s+rect:\s+serializedVersion: 2\s+x: ([\d.]+)\s+y: ([\d.]+)\s+width: ([\d.]+)\s+height: ([\d.]+)\s+alignment: (\d+)\s+pivot: \{x: ([-\d.e]+), y: ([-\d.e]+)\}[\s\S]*?internalID: (-?\d+)/g;
  let m;
  while ((m = re.exec(meta))) {
    const [, name, x, y, w, h, , px, py, id] = m;
    sprites[id] = { n: name, x: +x, y: texH - +y - +h, w: +w, h: +h, px: +px, py: +py };
  }
  textures[guid] = { src: '../../' + rel(png), w: texW, h: texH, sprites };
  return textures[guid];
}

const clips = [];
for (const file of walk(CLIP_DIR, '.anim').sort()) {
  const txt = fs.readFileSync(file, 'utf8');
  const name = /m_Name: (.+)/.exec(txt)[1].trim();
  const sampleRate = +/m_SampleRate: ([\d.]+)/.exec(txt)[1];
  const stop = +/m_StopTime: ([\d.e-]+)/.exec(txt)[1];
  const loop = /m_LoopTime: 1/.test(txt);

  const pptr = txt.slice(txt.indexOf('m_PPtrCurves:'), txt.indexOf('m_SampleRate:'));
  const frames = [];
  const kre = /- time: ([\d.e-]+)\s+value: \{fileID: (-?\d+), guid: ([0-9a-f]{32}),/g;
  let k;
  while ((k = kre.exec(pptr))) {
    const tex = loadTexture(k[3]);
    const ok = tex && tex.sprites[k[2]];
    frames.push({ t: +k[1], g: k[3], id: k[2], missing: !ok });
  }

  const events = [];
  const evBlock = txt.slice(txt.indexOf('m_Events:'));
  const ere = /- time: ([\d.e-]+)\s+functionName: (\S+)/g;
  let e;
  while ((e = ere.exec(evBlock))) events.push({ t: +e[1], fn: e[2] });

  const relDir = path.relative(CLIP_DIR, path.dirname(file)).split(path.sep);
  const dir = +(/_dir(\d)$/.exec(name) || [, -1])[1];
  clips.push({
    group: relDir[0],
    folder: relDir.slice(1).join(' / ') || relDir[0],
    name, dir, sampleRate, stop, loop, frames, events,
    file: rel(file),
  });
}

// keep only the sprite rects a clip actually references
const used = new Set(clips.flatMap((c) => c.frames.map((f) => f.g + ':' + f.id)));
for (const [g, t] of Object.entries(textures)) {
  for (const id of Object.keys(t.sprites)) if (!used.has(g + ':' + id)) delete t.sprites[id];
}

const data = { generated: new Date().toISOString(), clips, textures };
const tpl = fs.readFileSync(path.join(__dirname, 'viewer-template.html'), 'utf8');
fs.writeFileSync(OUT, tpl.replace('/*__DATA__*/null', JSON.stringify(data)));
const missing = clips.reduce((n, c) => n + c.frames.filter((f) => f.missing).length, 0);
console.log(`${clips.length} clips, ${Object.keys(textures).length} textures, ${missing} missing sprite refs -> ${rel(OUT)}`);
