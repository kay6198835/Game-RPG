// Re-points .anim sprite keyframes whose fileID no longer exists in the texture's current .meta
// (sheet re-imported -> internalIDs regenerated). Old fileID -> sprite name is looked up in the
// git history of that .meta; the name is then resolved to the current internalID.
// Usage (from repo root): node tools/anim-viewer/fix-missing-sprites.js [--apply]
const fs = require('fs');
const path = require('path');
const { execFileSync } = require('child_process');

const ROOT = path.resolve(__dirname, '..', '..');
const ASSETS = path.join(ROOT, 'Assets');
const APPLY = process.argv.includes('--apply');
const git = (...a) => execFileSync('git', a, { cwd: ROOT, maxBuffer: 1 << 28 }).toString();

function walk(dir, ext, out = []) {
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, e.name);
    if (e.isDirectory()) walk(p, ext, out);
    else if (e.name.endsWith(ext)) out.push(p);
  }
  return out;
}
const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');

function spriteIds(metaText) {
  const re = /- serializedVersion: 2\s+name: (.+?)\s+rect:[\s\S]*?internalID: (-?\d+)/g;
  const idToName = {}, nameToId = {};
  let m;
  while ((m = re.exec(metaText))) { idToName[m[2]] = m[1]; nameToId[m[1]] = m[2]; }
  if (/spriteMode: 1/.test(metaText)) idToName['21300000'] = nameToId.__single = '21300000';
  return { idToName, nameToId };
}

const guidToMeta = new Map();
for (const m of walk(ASSETS, '.png.meta')) {
  const g = (/^guid: ([0-9a-f]{32})/m.exec(fs.readFileSync(m, 'utf8')) || [])[1];
  if (g) guidToMeta.set(g, m);
}

const current = new Map();
const cur = (g) => {
  if (!current.has(g)) current.set(g, spriteIds(fs.readFileSync(guidToMeta.get(g), 'utf8')));
  return current.get(g);
};

// old id -> name, collected from every committed version of the meta (newest first)
const history = new Map();
function oldName(g, id) {
  if (!history.has(g)) {
    const map = {};
    const metaRel = rel(guidToMeta.get(g));
    for (const c of git('log', '--all', '--format=%H', '--', metaRel).split('\n').filter(Boolean)) {
      let txt;
      try { txt = git('show', `${c}:${metaRel}`); } catch { continue; }
      for (const [i, n] of Object.entries(spriteIds(txt).idToName)) if (!(i in map)) map[i] = n;
    }
    history.set(g, map);
  }
  return history.get(g)[id];
}

const report = { files: 0, fixed: 0, unresolved: [] };
for (const file of walk(ASSETS, '.anim')) {
  const txt = fs.readFileSync(file, 'utf8');
  let changed = 0;
  // matches keyframe values and m_ClipBindingConstant.pptrCurveMapping entries
  const out = txt.replace(/\{fileID: (-?\d+), guid: ([0-9a-f]{32}),(\s+)type: 3\}/g, (all, id, g, ws) => {
    if (!guidToMeta.has(g) || id === '0') return all;
    const c = cur(g);
    if (c.idToName[id]) return all;
    const name = oldName(g, id);
    const newId = name && c.nameToId[name];
    if (!newId) { report.unresolved.push(`${rel(file)}: ${g}:${id} (${name || 'name unknown'})`); return all; }
    changed++;
    return `{fileID: ${newId}, guid: ${g},${ws}type: 3}`;
  });
  if (changed) {
    report.files++; report.fixed += changed;
    console.log(`${APPLY ? 'fixed' : 'would fix'} ${changed} keys  ${rel(file)}`);
    if (APPLY) fs.writeFileSync(file, out);
  }
}
console.log(`\n${report.fixed} keyframes in ${report.files} clips ${APPLY ? 'rewritten' : '(dry run, pass --apply)'}; ${report.unresolved.length} unresolved`);
report.unresolved.slice(0, 30).forEach((u) => console.log('  ? ' + u));
