// Re-points directional clips so clip dirK uses sheet dirK of the same sprite set.
// Frames are matched by index (<sheet>_dirJ_<n> -> <sheet>_dirK_<n>); timing, events and loop stay.
// Usage (from repo root): node tools/anim-viewer/retarget-dir.js [--apply] <clip folder> [...]
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..', '..');
const ASSETS = path.join(ROOT, 'Assets');
const APPLY = process.argv.includes('--apply');
const folders = process.argv.slice(2).filter((a) => a !== '--apply');
const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');

function walk(dir, ext, out = []) {
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, e.name);
    if (e.isDirectory()) walk(p, ext, out);
    else if (e.name.endsWith(ext)) out.push(p);
  }
  return out;
}

const guidToPng = new Map(), pngToGuid = new Map();
for (const m of walk(ASSETS, '.png.meta')) {
  const g = (/^guid: ([0-9a-f]{32})/m.exec(fs.readFileSync(m, 'utf8')) || [])[1];
  if (g) { guidToPng.set(g, m.slice(0, -5)); pngToGuid.set(m.slice(0, -5), g); }
}
const sheets = new Map();
function sheet(g) {
  if (!sheets.has(g)) {
    const txt = fs.readFileSync(guidToPng.get(g) + '.meta', 'utf8');
    const idToName = {}, nameToId = {};
    for (const m of txt.matchAll(/- serializedVersion: 2\s+name: (.+?)\s+rect:[\s\S]*?internalID: (-?\d+)/g)) {
      idToName[m[2]] = m[1]; nameToId[m[1]] = m[2];
    }
    sheets.set(g, { idToName, nameToId });
  }
  return sheets.get(g);
}

let files = 0, keys = 0;
const errors = [];
for (const folder of folders) {
  for (const file of walk(path.join(ROOT, folder), '.anim').sort()) {
    const name = path.basename(file, '.anim');
    const dm = /dir ?(\d)$/i.exec(name);
    if (!dm) { errors.push(`${rel(file)}: no dir index in clip name`); continue; }
    const K = +dm[1];
    const txt = fs.readFileSync(file, 'utf8');
    const plan = new Map();
    for (const m of txt.matchAll(/\{fileID: (-?\d+), guid: ([0-9a-f]{32}),\s+type: 3\}/g)) {
      const [, id, g] = m;
      if (id === '0' || plan.has(g + ':' + id) || !guidToPng.has(g)) continue;
      const png = guidToPng.get(g);
      const pm = /^(.*_dir)(\d)\.png$/.exec(png);
      if (!pm) { errors.push(`${rel(file)}: sheet without dir index ${rel(png)}`); continue; }
      if (+pm[2] === K) continue;
      const targetPng = pm[1] + K + '.png';
      const tg = pngToGuid.get(targetPng);
      if (!tg) { errors.push(`${rel(file)}: missing ${rel(targetPng)}`); continue; }
      const sName = sheet(g).idToName[id];
      const fm = sName && /_(\d+)$/.exec(sName);
      const tName = fm && path.basename(targetPng, '.png') + '_' + fm[1];
      const tId = tName && sheet(tg).nameToId[tName];
      if (!tId) { errors.push(`${rel(file)}: no frame ${tName || id} in ${rel(targetPng)}`); continue; }
      plan.set(g + ':' + id, { tg, tId, from: path.basename(png), to: path.basename(targetPng) });
    }
    if (!plan.size) continue;
    let n = 0;
    const out = txt.replace(/\{fileID: (-?\d+), guid: ([0-9a-f]{32}),(\s+)type: 3\}/g, (all, id, g, ws) => {
      const p = plan.get(g + ':' + id);
      if (!p) return all;
      n++;
      return `{fileID: ${p.tId}, guid: ${p.tg},${ws}type: 3}`;
    });
    const pairs = [...new Set([...plan.values()].map((p) => `${p.from} -> ${p.to}`))].join(', ');
    console.log(`${APPLY ? 'fixed' : 'would fix'} ${n} refs  ${rel(file)}  [${pairs}]`);
    files++; keys += n;
    if (APPLY) fs.writeFileSync(file, out);
  }
}
console.log(`\n${keys} refs in ${files} clips ${APPLY ? 'rewritten' : '(dry run, pass --apply)'}; ${errors.length} errors`);
errors.forEach((e) => console.log('  ! ' + e));
