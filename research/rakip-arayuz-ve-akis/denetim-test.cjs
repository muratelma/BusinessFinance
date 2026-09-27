#!/usr/bin/env node
// denetim.cjs için pozitif ve negatif testler (B17 / P1-K).
// Kullanım: node denetim-test.cjs · Çıkış: 0 hepsi geçti · 1 en az bir test düştü.
// Her test geçici bir dizinde sahte form ve kare kurar; araştırma dosyalarına dokunmaz.
'use strict';
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const assert = require('node:assert/strict');
const { spawnSync } = require('node:child_process');
const { denetle, hucreSayisi, pngAtiflari } = require('./denetim.cjs');

const KAPI = path.join(__dirname, 'denetim.cjs');
const kokler = [];
const testler = [];
const test = (ad, fn) => testler.push([ad, fn]);

// İçerik denetlenmediği için kare gövdesi önemsizdir.
function kur(dosyalar) {
  const kok = fs.mkdtempSync(path.join(os.tmpdir(), 'denetim-test-'));
  kokler.push(kok);
  fs.mkdirSync(path.join(kok, 'gozlemler'), { recursive: true });
  fs.mkdirSync(path.join(kok, 'kanitlar', 'app'), { recursive: true });
  for (const [yol, icerik] of Object.entries(dosyalar)) {
    const tam = path.join(kok, yol);
    fs.mkdirSync(path.dirname(tam), { recursive: true });
    fs.writeFileSync(tam, icerik);
  }
  return kok;
}
const kare = ad => ({ ['kanitlar/app/' + ad]: 'png' });
const form = metin => ({ 'gozlemler/app.md': metin });
const calistir = (kok, siki = false) => denetle({ kok, uygulamalar: ['app'], siki });
const turler = r => r.sonuclar[0].hatalar.map(h => h.tur);

test('temiz form: tam ad, açık liste, sıralı akış ve tablo', () => {
  const kok = kur({
    ...kare('01-a.png'), ...kare('02-b.png'), ...kare('f7-53-export.png'),
    ...form('| Alan | Kanıt |\n|---|---|\n| x | `01-a.png`, `02-b.png` |\n\nAkış `01-a.png` → `f7-53-export.png`.\n'),
  });
  const r = calistir(kok);
  assert.equal(r.cikis, 0);
  assert.deepEqual(turler(r), []);
});

test('olmayan form çıkış 2 verir, temiz sayılmaz', () => {
  const r = denetle({ kok: kur(kare('01-a.png')), uygulamalar: ['app'] });
  assert.equal(r.cikis, 2);
  assert.equal(r.sonuclar[0].formYok, true);
});

test('ölü atıf: tam adla anılan dosya diskte yok', () => {
  const r = calistir(kur({ ...kare('01-a.png'), ...form('`01-a.png` ve `09-yok.png`\n') }));
  assert.equal(r.cikis, 1);
  assert.deepEqual(turler(r), ['ölü atıf']);
});

test('yollu atıf: geçerli yol kapsar, olmayan yol ölü atıftır', () => {
  const r = calistir(kur({ ...kare('01-a.png'), ...form('[kare](../kanitlar/app/01-a.png) ve kanitlar/app/99-yok.png\n') }));
  assert.deepEqual(turler(r), ['ölü atıf']);
  assert.equal(r.sonuclar[0].hatalar[0].mesaj, 'kanitlar/app/99-yok.png');
});

test('yetim kare: formda hiç anılmayan dosya hatadır', () => {
  const r = calistir(kur({ ...kare('01-a.png'), ...kare('02-b.png'), ...form('`01-a.png`\n') }));
  assert.deepEqual(turler(r), ['yetim kare']);
  assert.match(r.sonuclar[0].hatalar[0].mesaj, /02-b\.png formda hiç anılmıyor/);
});

test('yalnız kısa kodla anılan kare yetim sayılır', () => {
  const r = calistir(kur({ ...kare('01-a.png'), ...kare('02-b.png'), ...form('`01-a.png` ve `02`\n') }));
  assert.equal(r.cikis, 1);
  assert.match(r.sonuclar[0].hatalar[0].mesaj, /02-b\.png yalnız kısa kodla/);
});

test('tam ad + kısa kod: varsayılan hata; siki false ile gevşetilemez', () => {
  const kok = kur({ ...kare('01-a.png'), ...form('`01-a.png` sonra yine `01`\n') });
  const normal = calistir(kok);
  assert.equal(normal.cikis, 1);
  assert.deepEqual(turler(normal), ['kısa kod']);
  const siki = calistir(kok, true);
  assert.equal(siki.cikis, 1);
  assert.deepEqual(turler(siki), ['kısa kod']);
});

test('kimlik kırpılmaz: f7-53 kısa kodu yalnız kendi karesine bağlanır', () => {
  const r = calistir(kur({
    ...kare('f7-53-export.png'), ...kare('f7-54-print.png'),
    ...form('`f7-53-export.png`, `f7-54-print.png`; ayrıca `f7-53`\n'),
  }));
  assert.equal(r.cikis, 1);
  assert.equal(r.sonuclar[0].hatalar.length, 1);
  assert.match(r.sonuclar[0].hatalar[0].mesaj, /^f7-53-export\.png/);
});

test('tam ad sınırı: 101-a.png, 01-a.png için atıf sayılmaz', () => {
  const r = calistir(kur({ ...kare('01-a.png'), ...form('`101-a.png`\n') }));
  assert.deepEqual(turler(r).sort(), ['yetim kare', 'ölü atıf'].sort());
});

test('belirsiz aralık: tire, en tire ve em tire; NN, dNN, f7-NN', () => {
  for (const metin of ['`10`–`12`', '`d01`-`d03`', '`f7-01` — `f7-03`']) {
    const r = calistir(kur(form(metin + '\n')));
    assert.equal(r.cikis, 1);
    assert.deepEqual(turler(r), ['belirsiz aralık', 'çözülemeyen kısa kod', 'çözülemeyen kısa kod'], metin);
  }
});

test('örnekler: çift ters tırnak ve çitli blok atıf, aralık veya yasak sayılmaz', () => {
  const r = calistir(kur(form(
    'Yasak olan `` `d01`–`d31` `` biçimi ve `` `zz-yok.png` `` adı.\n'
    + '```\n`10`–`12` negatif referans 99-yok.png\n| a |\n| b | c |\n```\n',
  )));
  assert.equal(r.cikis, 0);
  assert.deepEqual(turler(r), []);
});

test('yasak kalıp: metinde hata, tek ters tırnaklı alıntıda değil', () => {
  assert.deepEqual(turler(calistir(kur(form('Bu bir negatif referans.\n')))), ['yasak kalıp']);
  assert.equal(calistir(kur(form('Kalıp `negatif referans` alıntısı.\n'))).cikis, 0);
});

test('tablo: uyumsuz satır hata; kaçışlı boru hücre ayırmaz', () => {
  assert.deepEqual(turler(calistir(kur(form('| a | b | c |\n|---|---|---|\n| 1 | 2 | 3 | 4 |\n')))), ['tablo']);
  assert.equal(calistir(kur(form('| a | b |\n|---|---|\n| x \\| y | z |\n'))).cikis, 0);
  assert.equal(hucreSayisi('| a | b \\| c |'), 2);
});

test('karşılığı olmayan kısa kod hatadır', () => {
  const r = calistir(kur(form('`05` numarası boş\n')));
  assert.equal(r.cikis, 1);
  assert.deepEqual(turler(r), ['çözülemeyen kısa kod']);
});

// BC-U01: kullanıcı kanıtları yeniden adlandırılmadan Türkçe ve boşluklu adla anılır.
const bcU01 = { ...kare('işlemler.png'), ...kare('ögeler özeti.png') };

test('Türkçe ve boşluklu ad: işlemler.png ve ögeler özeti.png kırpılmaz, yetim kalmaz', () => {
  const r = calistir(kur({ ...bcU01, ...form('`işlemler.png`: liste. `ögeler özeti.png`: rapor.\n') }));
  assert.equal(r.cikis, 0);
  assert.deepEqual(turler(r), []);
});

test('Türkçe benzer ama olmayan adlar tam adıyla ölü atıftır; gerçek kareyi kapsamaz', () => {
  const r = calistir(kur({
    ...bcU01,
    ...form('`islemler.png`, `işlemler-eski.png`, `ögeler ozeti.png` ve `eski ögeler özeti.png`\n'),
  }));
  assert.equal(r.cikis, 1);
  const olu = r.sonuclar[0].hatalar.filter(h => h.tur === 'ölü atıf').map(h => h.mesaj);
  assert.deepEqual(olu, ['islemler.png', 'işlemler-eski.png', 'ögeler ozeti.png', 'eski ögeler özeti.png']);
  const yetim = r.sonuclar[0].hatalar.filter(h => h.tur === 'yetim kare').map(h => h.mesaj);
  assert.deepEqual(yetim.sort(), ['işlemler.png formda hiç anılmıyor', 'ögeler özeti.png formda hiç anılmıyor']);
});

test('tırnaksız Türkçe ad ve yollu boşluklu ad çözülür; yolda olmayan ad ölüdür', () => {
  assert.equal(calistir(kur({
    ...bcU01, ...form('Liste işlemler.png karesinde; rapor `../kanitlar/app/ögeler özeti.png`.\n'),
  })).cikis, 0);
  const r = calistir(kur({ ...bcU01, ...form('işlemler.png ve `kanitlar/app/ögeler özetı.png`\n') }));
  assert.deepEqual(r.sonuclar[0].hatalar.map(h => `${h.tur}: ${h.mesaj}`).sort(), [
    'yetim kare: ögeler özeti.png formda hiç anılmıyor', 'ölü atıf: kanitlar/app/ögeler özetı.png',
  ].sort());
});

test('tırnaksız boşluklu ad birleştirilmez: yalnız son sözcük atıf olur ve ölüdür', () => {
  const r = calistir(kur({ ...bcU01, ...form('`işlemler.png` ve ögeler özeti.png\n') }));
  assert.deepEqual(turler(r).sort(), ['yetim kare', 'ölü atıf'].sort());
  assert.equal(r.sonuclar[0].hatalar.find(h => h.tur === 'ölü atıf').mesaj, 'özeti.png');
});

test('tek kod parçasındaki iki ad boşluklu tek ada birleşmez', () => {
  assert.deepEqual(pngAtiflari('`01-a.png 02-b.png` ve `işlemler.png, ögeler özeti.png`'),
    ['01-a.png', '02-b.png', 'işlemler.png', 'özeti.png']);
  assert.equal(calistir(kur({ ...kare('01-a.png'), ...kare('02-b.png'), ...form('`01-a.png 02-b.png`\n') })).cikis, 0);
});

test('NFD diskteki ad ile NFC metindeki ad aynı karedir; örnekteki Türkçe ad atıf sayılmaz', () => {
  const nfd = 'ögeler özeti.png'.normalize('NFD');
  assert.equal(calistir(kur({ ...kare(nfd), ...form('`ögeler özeti.png`\n') })).cikis, 0);
  assert.equal(calistir(kur(form('Örnek `` `ögeler yok.png` `` biçimi.\n'))).cikis, 0);
});

test('CLI çıkış kodları: 0 temiz, 1 hata, 2 olmayan form ve bilinmeyen bayrak', () => {
  const kos = (kok, argumanlar) => spawnSync(process.execPath, [KAPI, ...argumanlar], {
    env: { ...process.env, DENETIM_KOK: kok }, encoding: 'utf8',
  }).status;
  assert.equal(kos(kur({ ...kare('01-a.png'), ...form('`01-a.png`\n') }), ['app']), 0);
  assert.equal(kos(kur({ ...kare('01-a.png'), ...form('hiç atıf yok\n') }), ['app']), 1);
  assert.equal(kos(kur(form('x\n')), ['yok-boyle-uygulama']), 2);
  assert.equal(kos(kur(form('x\n')), ['app', '--bilinmeyen']), 2);
  const kisa = kur({ ...kare('01-a.png'), ...form('`01-a.png` ve `01`\n') });
  assert.equal(kos(kisa, ['app']), 1);
  assert.equal(kos(kisa, ['app', '--siki']), 1);
});

let dusen = 0;
for (const [ad, fn] of testler) {
  try {
    fn();
    console.log('✓ ' + ad);
  } catch (e) {
    dusen++;
    console.log('✗ ' + ad + '\n  ' + String(e.message).split('\n').join('\n  '));
  }
}
for (const kok of kokler) fs.rmSync(kok, { recursive: true, force: true });
console.log(`\n${testler.length - dusen}/${testler.length} test geçti`);
process.exitCode = dusen ? 1 : 0;
