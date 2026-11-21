// Side-by-side preview of the two ways to move sprite clips to 30 fps.
//   A "keep duration": key/event/stop time snapped to the 30 fps grid (frames that land on the same tick merge).
//   B "keep frames":   key n of a clip sampled at sr is placed at frame n of 30 fps (t' = round(t*sr)/30).
// Reads DATA from all-animations.html (run build-all-viewer.js first).
// Usage (from repo root): node tools/anim-viewer/build-rate30-compare.js
const fs = require('fs');
const path = require('path');

const HERE = __dirname;
const src = fs.readFileSync(path.join(HERE, 'all-animations.html'), 'utf8');
let json = src.slice(src.indexOf('const DATA = ') + 13, src.indexOf('\nconst EPS')).trim();
if (json.endsWith(';')) json = json.slice(0, -1);
const DATA = JSON.parse(json);

const R = 30;
const snapA = (t) => Math.round(t * R) / R;
const clips = [];
const used = new Set();
for (const c of DATA.clips) {
  if (!c.frames.length || c.sampleRate === R || c.curves) continue;
  const sr = c.sampleRate;
  const toB = (t) => Math.round(t * sr) / R;
  const mk = (fn) => ({
    stop: fn(c.stop),
    frames: c.frames.map((f) => [fn(f.t), f.g, f.id]),
    events: c.events.map((e) => [fn(e.t), e.fn]),
  });
  clips.push({
    group: c.group, folder: c.folder, name: c.name, sr, loop: c.loop, file: c.file,
    O: { stop: c.stop, frames: c.frames.map((f) => [f.t, f.g, f.id]), events: c.events.map((e) => [e.t, e.fn]) },
    A: mk(snapA),
    B: mk(toB),
  });
  c.frames.forEach((f) => used.add(f.g));
}
const textures = {};
for (const g of used) textures[g] = DATA.textures[g];

const html = `<!doctype html>
<html lang="en"><head><meta charset="utf-8"><title>30 fps Compare</title>
<meta name="viewport" content="width=device-width,initial-scale=1">
<style>
:root{--bg:#15171c;--panel:#1e2128;--line:#2c313b;--text:#e6e8ee;--dim:#8b93a3;--a:#5aa9ff;--b:#ffb454;--o:#9fd38a;--ev:#ff6b8a}
*{box-sizing:border-box}body{margin:0;background:var(--bg);color:var(--text);font:13px/1.4 system-ui,Segoe UI,sans-serif;display:flex;height:100vh}
#side{width:330px;border-right:1px solid var(--line);display:flex;flex-direction:column}
#side input{margin:8px;padding:6px 8px;background:var(--panel);border:1px solid var(--line);color:var(--text);border-radius:4px}
#list{overflow:auto;flex:1}
.grp{padding:6px 10px;color:var(--dim);font-weight:600;position:sticky;top:0;background:var(--bg);border-bottom:1px solid var(--line)}
.it{padding:4px 10px 4px 18px;cursor:pointer;display:flex;justify-content:space-between;gap:8px}
.it:hover{background:var(--panel)}.it.sel{background:#2a3446}
.it small{color:var(--dim)}
#main{flex:1;overflow:auto;padding:14px}
#bar{display:flex;gap:10px;align-items:center;flex-wrap:wrap;margin-bottom:12px}
button,select{background:var(--panel);color:var(--text);border:1px solid var(--line);padding:5px 10px;border-radius:4px;cursor:pointer}
#title{font-weight:600;font-size:15px}#path{color:var(--dim);font-size:12px}
#cols{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:12px}
.col{background:var(--panel);border:1px solid var(--line);border-radius:6px;padding:10px}
.col h3{margin:0 0 2px;font-size:13px}.col .sub{color:var(--dim);font-size:12px;margin-bottom:6px}
.col canvas.view{width:100%;aspect-ratio:1;background:#0f1115;border-radius:4px;display:block}
.col canvas.tl{width:100%;height:46px;display:block;margin-top:6px}
.stat{font-family:Consolas,monospace;font-size:12px;margin-top:6px;white-space:pre-wrap}
.O h3{color:var(--o)}.A h3{color:var(--a)}.B h3{color:var(--b)}
.warn{color:var(--ev)}
#evt{margin-top:14px;border-collapse:collapse;font-family:Consolas,monospace;font-size:12px}
#evt td,#evt th{border:1px solid var(--line);padding:3px 8px;text-align:right}#evt th{color:var(--dim)}
#evt td:first-child,#evt th:first-child{text-align:left}
#note{color:var(--dim);font-size:12px;margin:10px 0 0;max-width:900px}
@media (max-width:900px){body{flex-direction:column;height:auto}#side{width:auto;height:40vh}#cols{grid-template-columns:1fr}}
</style></head><body>
<div id="side"><input id="q" placeholder="Filter (name / folder / group)"><div id="list"></div></div>
<div id="main">
  <div id="bar"><button id="play">Pause</button>
    <label>Speed <select id="spd"><option>0.25</option><option>0.5</option><option selected>1</option><option>2</option></select></label>
    <label><input type="checkbox" id="sync" checked> Restart all together (loop on longest)</label>
    <span><span id="title"></span> <span id="path"></span></span></div>
  <div id="cols">
    <div class="col O"><h3>Original</h3><div class="sub" id="sO"></div><canvas class="view" id="vO" width="400" height="400"></canvas><canvas class="tl" id="tO" width="600" height="46"></canvas><div class="stat" id="xO"></div></div>
    <div class="col A"><h3>A · keep duration</h3><div class="sub">snap to 1/30 s, frames on one tick merge</div><canvas class="view" id="vA" width="400" height="400"></canvas><canvas class="tl" id="tA" width="600" height="46"></canvas><div class="stat" id="xA"></div></div>
    <div class="col B"><h3>B · keep frames</h3><div class="sub">frame n at n/30 s, clip stretches</div><canvas class="view" id="vB" width="400" height="400"></canvas><canvas class="tl" id="tB" width="600" height="46"></canvas><div class="stat" id="xB"></div></div>
  </div>
  <table id="evt"></table>
  <p id="note">Timeline: grey ticks = sprite keys, red = events, white line = playhead. A frame with several keys on one tick shows only the last one (same as Unity).</p>
</div>
<script>
const CLIPS = ${JSON.stringify(clips)};
const TEX = ${JSON.stringify(textures)};
const images = {};
function img(g){ if(!images[g]){ const i=new Image(); i.src=TEX[g].src; images[g]=i; } return images[g]; }
let cur = null, t0 = performance.now(), playing = true, pausedAt = 0;
const $ = (id) => document.getElementById(id);
function renderList(){
  const q = $('q').value.toLowerCase(); const list = $('list'); list.innerHTML='';
  let last='';
  CLIPS.forEach((c,i)=>{
    const key=c.group+' · '+c.folder;
    if(q && !(c.name+' '+key).toLowerCase().includes(q)) return;
    if(key!==last){ const h=document.createElement('div'); h.className='grp'; h.textContent=key; list.appendChild(h); last=key; }
    const d=document.createElement('div'); d.className='it'+(cur===c?' sel':''); d.innerHTML='<span>'+c.name+'</span><small>'+c.sr+' fps</small>';
    d.onclick=()=>select(c); list.appendChild(d);
  });
}
function uniqFrames(v){ const s=new Set(); v.frames.forEach(f=>s.add(Math.round(f[0]*1e4))); return s.size; }
function select(c){
  cur=c; try{localStorage.setItem('r30clip',c.file)}catch(e){}
  [...new Set(c.O.frames.map(f=>f[1]))].forEach(img);
  $('title').textContent=c.name; $('path').textContent=c.file;
  $('sO').textContent=c.sr+' fps · '+c.O.stop.toFixed(3)+'s';
  for(const k of ['O','A','B']){
    const v=c[k], n=uniqFrames(v), n0=uniqFrames(c.O);
    let s='duration '+v.stop.toFixed(3)+'s'+(k!=='O'?'  ('+(v.stop/c.O.stop).toFixed(2)+'x)':'')+'\\nvisible frames '+n+(k!=='O'&&n<n0?'  (-'+(n0-n)+')':'');
    $('x'+k).innerHTML=s.replace(/\\(-(\\d+)\\)/,'<span class="warn">(-$1)</span>');
  }
  const rows=['<tr><th>event</th><th>original</th><th>A</th><th>Δ A</th><th>B</th><th>Δ B</th></tr>'];
  c.O.events.forEach((e,i)=>{ const a=c.A.events[i][0], b=c.B.events[i][0];
    const da=a-e[0], db=b-e[0];
    rows.push('<tr><td>'+e[1]+'</td><td>'+e[0].toFixed(3)+'</td><td>'+a.toFixed(3)+'</td><td'+(Math.abs(da)>1e-3?' class="warn"':'')+'>'+(da>=0?'+':'')+da.toFixed(3)+'</td><td>'+b.toFixed(3)+'</td><td'+(Math.abs(db)>1e-3?' class="warn"':'')+'>'+(db>=0?'+':'')+db.toFixed(3)+'</td></tr>'); });
  $('evt').innerHTML=c.O.events.length?rows.join(''):'<tr><td>no events</td></tr>';
  t0=performance.now(); pausedAt=0; renderList();
}
function frameAt(v,t){ let idx=0; for(let i=0;i<v.frames.length;i++){ if(v.frames[i][0]<=t+1e-6) idx=i; else break; } return idx; }
function draw(k,v,t){
  const cv=$('v'+k), ctx=cv.getContext('2d'); ctx.clearRect(0,0,cv.width,cv.height);
  const f=v.frames[frameAt(v,t)], tex=TEX[f[1]], s=tex&&tex.sprites[f[2]], im=images[f[1]];
  if(s&&im&&im.complete){ const sc=cv.width/200, w=s.w*sc, h=s.h*sc, x=cv.width/2-s.px*w, y=cv.height*0.6-(1-s.py)*h; ctx.drawImage(im,s.x,s.y,s.w,s.h,x,y,w,h); }
  const tl=$('t'+k), tc=tl.getContext('2d'), W=tl.width, H=tl.height, span=Math.max(cur.O.stop,cur.A.stop,cur.B.stop)||1;
  tc.clearRect(0,0,W,H); tc.fillStyle='#0f1115'; tc.fillRect(0,0,W,H);
  tc.fillStyle='#2c313b'; tc.fillRect(0,0,W*v.stop/span,H);
  tc.fillStyle='#8b93a3'; v.frames.forEach(fr=>tc.fillRect(Math.round(W*fr[0]/span),H*0.45,1,H*0.4));
  tc.fillStyle='#ff6b8a'; v.events.forEach(e=>tc.fillRect(Math.round(W*e[0]/span)-1,0,3,H*0.4));
  tc.fillStyle='#fff'; tc.fillRect(Math.round(W*Math.min(t,v.stop)/span),0,1,H);
}
function tick(now){
  if(cur){
    const spd=+$('spd').value, el=playing?(now-t0)/1000*spd:pausedAt;
    const longest=Math.max(cur.O.stop,cur.A.stop,cur.B.stop)||1;
    for(const k of ['O','A','B']){ const v=cur[k], d=v.stop||1;
      const t=$('sync').checked ? Math.min(el%longest,d) : el%d; draw(k,v,t); }
  }
  requestAnimationFrame(tick);
}
$('play').onclick=()=>{ const spd=+$('spd').value; if(playing){ pausedAt=(performance.now()-t0)/1000*spd; $('play').textContent='Play'; } else { t0=performance.now()-pausedAt*1000/spd; $('play').textContent='Pause'; } playing=!playing; };
$('q').oninput=renderList;
let saved=null; try{saved=localStorage.getItem('r30clip')}catch(e){}
select(CLIPS.find(c=>c.file===saved)||CLIPS.find(c=>/Paladin_Dash_dir0/.test(c.name))||CLIPS[0]);
requestAnimationFrame(tick);
</script></body></html>`;
fs.writeFileSync(path.join(HERE, 'rate30-compare.html'), html);
console.log(`${clips.length} clips, ${used.size} textures -> tools/anim-viewer/rate30-compare.html`);
