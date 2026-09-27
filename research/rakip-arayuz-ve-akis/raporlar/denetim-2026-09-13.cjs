// Salt okunur: node research/rakip-arayuz-ve-akis/raporlar/denetim-2026-09-13.cjs
// PNG yapisi/CRC/zlib, SHA-256, tam ad izlenebilirligi ve tablo kolonlari.
// Kisa kodlarin anlami, gorsel icerik ve urun iddialari insan incelemesi ister.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const zlib = require('node:zlib');
const assert = require('node:assert/strict');
const root = path.resolve(__dirname, '..');
const research = path.dirname(root);
function walk(dir) {
  return fs.readdirSync(dir, { withFileTypes: true }).flatMap(e =>
    e.isDirectory() ? walk(path.join(dir, e.name)) : [path.join(dir, e.name)]);
}
const relative = f => path.relative(research, f).replaceAll('\\', '/');
function crc32(bytes) {
  let crc = 0xffffffff;
  for (const byte of bytes) {
    crc ^= byte;
    for (let k = 0; k < 8; k++) crc = (crc >>> 1) ^ ((crc & 1) ? 0xedb88320 : 0);
  }
  return (crc ^ 0xffffffff) >>> 0;
}
function checkPng(b) {
  assert.equal(b.subarray(0, 8).toString('hex'), '89504e470d0a1a0a');
  let offset = 8, ended = false, header = false;
  const compressed = [];
  while (offset < b.length) {
    assert.ok(offset + 12 <= b.length, 'truncated chunk');
    const len = b.readUInt32BE(offset), end = offset + len + 12;
    assert.ok(end <= b.length, 'truncated payload');
    const type = b.toString('ascii', offset + 4, offset + 8);
    assert.equal(crc32(b.subarray(offset + 4, end - 4)), b.readUInt32BE(end - 4), 'CRC');
    if (!header) { assert.equal(type, 'IHDR'); assert.equal(len, 13); header = true; }
    if (type === 'IDAT') compressed.push(b.subarray(offset + 8, end - 4));
    offset = end;
    if (type === 'IEND') { assert.equal(len, 0); ended = true; break; }
  }
  assert.ok(ended && offset === b.length, 'IEND/trailing bytes');
  assert.ok(compressed.length > 0, 'missing IDAT');
  assert.ok(zlib.inflateSync(Buffer.concat(compressed)).length > 0, 'empty image');
}
function cells(line) {
  return line.trim().replace(/^\|/, '').replace(/\|$/, '').split(/(?<!\\)\|/).length;
}
function exactName(text, name) {
  const escaped = name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  return new RegExp('(?<![\\w-])' + escaped + '(?![\\w.-])').test(text);
}
assert.equal(crc32(Buffer.from('123456789')), 0xcbf43926);
assert.equal(cells('| a | b \\| c |'), 2);
assert.equal(exactName('`01-a.png`', '01-a.png'), true);
assert.equal(exactName('`01-a`', '01-a.png'), false);
assert.equal(exactName('`101-a.png`', '01-a.png'), false);
const files = walk(research);
const pngs = files.filter(f => /\.png$/i.test(f));
const markdown = files.filter(f => /\.md$/i.test(f));
const invalid = [], hashes = new Map(), tables = [], missingRefs = [];
for (const f of pngs) {
  const b = fs.readFileSync(f);
  try { checkPng(b); } catch (e) { invalid.push({ file: relative(f), error: e.message }); }
  const h = crypto.createHash('sha256').update(b).digest('hex');
  hashes.set(h, [...(hashes.get(h) || []), relative(f)]);
}
const coverage = [];
for (const form of markdown.filter(f => path.dirname(f) === path.join(root, 'gozlemler') && path.basename(f) !== 'README.md')) {
  const app = path.basename(form, '.md'), text = fs.readFileSync(form, 'utf8');
  const appPngs = pngs.filter(f => path.dirname(f) === path.join(root, 'kanitlar', app));
  coverage.push({ app, png: appPngs.length, missingFullName: appPngs.filter(f => !exactName(text, path.basename(f))).map(f => path.basename(f)) });
  const lines = text.split(/\r?\n/);
  // Acik PNG tokenlari: basename referansi kendi uygulamasinda, yol referansi
  // once belgeye gore, sonra arastirma kokune gore cozulur. Wildcardlar adaydir.
  for (let i = 0; i < lines.length; i++) {
    for (const m of lines[i].matchAll(/(?:[A-Za-z0-9_.-]+\/)*[A-Za-z0-9_-]+\.png\b/g)) {
      const token = m[0];
      const candidates = token.includes('/')
        ? [path.resolve(path.dirname(form), token), path.resolve(root, token)]
        : [path.join(root, 'kanitlar', app, token)];
      if (!candidates.some(f => fs.existsSync(f))) missingRefs.push({ file: relative(form), line: i + 1, token });
    }
  }
}
for (const f of markdown) {
  const lines = fs.readFileSync(f, 'utf8').split(/\r?\n/);
  let columns = null, fenced = false;
  for (let i = 0; i < lines.length; i++) {
    const l = lines[i];
    if (/^\s*(```|~~~)/.test(l)) { fenced = !fenced; columns = null; continue; }
    if (fenced || !/^\s*\|/.test(l)) { columns = null; continue; }
    if (columns === null) columns = cells(l);
    else if (cells(l) !== columns) tables.push({ file: relative(f), line: i + 1, expected: columns, actual: cells(l) });
  }
}
const duplicates = [...hashes.values()].filter(a => a.length > 1);
const report = {
  scope: 'Mevcut yerel research dosyalari; urun davranisini veya her pikseli dogrulamaz.',
  files: files.length, markdown: markdown.length, png: pngs.length,
  helperSelfChecks: 5, pngInvalid: invalid, exactDuplicates: duplicates,
  coverage, missingPngReferenceCandidates: missingRefs, tableMismatches: tables,
};
console.log(JSON.stringify(report, null, 2));
process.exitCode = invalid.length || duplicates.length || missingRefs.length || tables.length || coverage.some(c => c.missingFullName.length) ? 1 : 0;
