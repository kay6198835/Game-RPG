// Static server for the animation viewers (http:// lets the viewer scan for empty frames; file:// cannot).
// Usage (from repo root): node tools/anim-viewer/serve.js [port]
// Then open http://localhost:8765/tools/anim-viewer/all-animations.html
const http = require('http');
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..', '..');
const PORT = +(process.argv[2] || 8765);
const TYPES = { '.html': 'text/html; charset=utf-8', '.png': 'image/png', '.js': 'text/javascript', '.webp': 'image/webp' };

http.createServer((req, res) => {
  const p = path.join(ROOT, decodeURIComponent(new URL(req.url, 'http://x').pathname));
  if (!p.startsWith(ROOT)) { res.writeHead(403).end(); return; }
  fs.stat(p, (err, st) => {
    if (err || !st.isFile()) {
      if (req.url === '/') { res.writeHead(302, { Location: '/tools/anim-viewer/all-animations.html' }).end(); return; }
      res.writeHead(404).end('not found');
      return;
    }
    res.writeHead(200, { 'Content-Type': TYPES[path.extname(p).toLowerCase()] || 'application/octet-stream', 'Content-Length': st.size });
    fs.createReadStream(p).pipe(res);
  });
}).listen(PORT, '127.0.0.1', () => console.log(`http://localhost:${PORT}/tools/anim-viewer/all-animations.html`));
