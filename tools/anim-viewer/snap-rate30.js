// Moves sprite clips to 30 fps keeping their duration ("option A"):
// every sprite key, event and the stop time is snapped to the nearest 1/30 s.
// Keys that land on the same tick collapse to the last one (the one Unity shows),
// and the matching m_ClipBindingConstant.pptrCurveMapping entries are dropped with them.
// Clips with non-sprite curves are skipped and listed.
// Usage (from repo root): node tools/anim-viewer/snap-rate30.js [--apply] [--list]
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..', '..');
const APPLY = process.argv.includes('--apply');
const LIST = process.argv.includes('--list');
const R = 30;
const rel = (p) => path.relative(ROOT, p).split(path.sep).join('/');
function walk(d, o = []) {
  for (const e of fs.readdirSync(d, { withFileTypes: true })) {
    const p = path.join(d, e.name);
    if (e.isDirectory()) walk(p, o); else if (e.name.endsWith('.anim')) o.push(p);
  }
  return o;
}
// shortest decimal that reads back as the same float32, like Unity writes it
const fmt = (x) => {
  const f = Math.fround(x);
  for (let p = 1; p <= 9; p++) { const s = String(+f.toPrecision(p)); if (Math.fround(+s) === f) return s; }
  return String(f);
};
const snap = (t) => Math.round(t * R) / R;
const onGrid = (t) => Math.abs(t * R - Math.round(t * R)) < 1e-3;

const report = { changed: 0, keysDropped: 0, eventsMoved: 0, skipped: [], warn: [] };
for (const file of walk(path.join(ROOT, 'Assets'))) {
  const txt = fs.readFileSync(file, 'utf8');
  const eol = txt.includes('\r\n') ? '\r\n' : '\n';
  const t = txt.replace(/\r\n/g, '\n');
  const sr = +/m_SampleRate: (\S+)/.exec(t)[1];
  if (sr === R) continue;
  const hasPPtr = /m_PPtrCurves:\n  - (?:serializedVersion: 2\n    )?curve:/.test(t);
  if (!hasPPtr) continue;
  if (/m_(Rotation|Position|Scale|Euler|Float)Curves:\n\s+- /.test(t) || /m_EditorCurves:\n\s+- /.test(t)) {
    report.skipped.push(`${rel(file)} (sr${sr}, has non-sprite curves)`);
    continue;
  }
  const ppS = t.indexOf('  m_PPtrCurves:\n'), ppE = t.indexOf('  m_SampleRate:');
  const curves = t.slice(ppS, ppE).split(/\n(?=  - (?:serializedVersion: 2\n    )?curve:\n)/);
  const head = curves.shift();
  const keep = []; // per curve: kept key flags, in key order
  const newCurves = curves.map((block) => {
    const keyRe = /    - time: (\S+)\n      value: (\{[^}]*\})\n/g;
    const keys = [...block.matchAll(keyRe)].map((m) => ({ t: snap(+m[1]), v: m[2] }));
    const flags = keys.map((k, i) => i === keys.length - 1 || Math.abs(keys[i + 1].t - k.t) > 1e-6);
    keep.push(flags);
    let i = 0;
    return block.replace(keyRe, (all) => {
      const k = keys[i], f = flags[i++];
      return f ? `    - time: ${fmt(k.t)}\n      value: ${k.v}\n` : '';
    });
  });
  let out = t.slice(0, ppS) + [head, ...newCurves].join('\n') + t.slice(ppE);
  const dropped = keep.flat().filter((f) => !f).length;

  if (dropped) {
    const mapM = /(    pptrCurveMapping:\n)((?:    - \{[^}]*\}\n)*)/.exec(out);
    const entries = mapM ? mapM[2].split('\n').filter(Boolean) : [];
    const flat = keep.flat();
    if (entries.length !== flat.length) {
      report.warn.push(`${rel(file)}: pptrCurveMapping ${entries.length} entries vs ${flat.length} keys, skipped`);
      continue;
    }
    const kept = entries.filter((e, i) => flat[i]).map((e) => e + '\n').join('');
    out = out.replace(mapM[0], mapM[1] + kept);
  }

  const oldStop = +/m_StopTime: (\S+)/.exec(out)[1];
  out = out.replace(/m_SampleRate: \S+/, `m_SampleRate: ${R}`)
    .replace(/(m_StopTime: )\S+/, (a, p) => p + fmt(snap(oldStop)));
  const evS = out.indexOf('  m_Events:');
  let moved = 0;
  if (evS >= 0) {
    out = out.slice(0, evS) + out.slice(evS).replace(/  - time: (\S+)\n(    functionName: )/g, (a, v, rest) => {
      if (!onGrid(+v)) moved++;
      return `  - time: ${fmt(snap(+v))}\n${rest}`;
    });
  }
  report.changed++; report.keysDropped += dropped; report.eventsMoved += moved;
  if (LIST) console.log(`sr${sr} stop ${oldStop.toFixed(3)}->${snap(oldStop).toFixed(3)} dropped ${dropped} events moved ${moved}  ${rel(file)}`);
  if (APPLY) fs.writeFileSync(file, eol === '\n' ? out : out.replace(/\n/g, '\r\n'));
}
console.log(`${report.changed} clips ${APPLY ? 'rewritten' : '(dry run, pass --apply)'}; ${report.keysDropped} keys merged; ${report.eventsMoved} events moved`);
report.skipped.forEach((s) => console.log('  skip ' + s));
report.warn.forEach((s) => console.log('  ! ' + s));
