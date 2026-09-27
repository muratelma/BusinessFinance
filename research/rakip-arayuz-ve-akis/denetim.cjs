#!/usr/bin/env node
// Gözlem formu / kanıt denetimi — makineyle kontrol edilebilen kurallar.
//
// Kullanım:
//   node denetim.cjs                         → dokuz uygulama
//   node denetim.cjs money-manager parasut   → seçilen uygulamalar
//   node denetim.cjs --siki                  → varsayılan sıkı denetim (uyumluluk)
// Çıkış kodu: 0 hata yok · 1 en az bir hata ·
//             2 kullanım hatası veya olmayan gözlem formu.
// Testler: node denetim-test.cjs
//
// Kural kaynakları: README.md → "Gözlem formlarında dil kuralı",
// MANUEL-TEST-PROTOKOLU.md → "Gözlem formu yazım kuralları".
// Bu yapısal bir kontroldür: karede yazan ile metinde yazanın aynı olduğunu
// (içerik onayı) göstermez.
//
// Çift ters tırnaklı kod parçaları (`` ... ``) ve çitli kod blokları kural
// örneği sayılır; atıf, aralık veya yasak kalıp olarak değerlendirilmez.
'use strict';
const fs = require('node:fs');
const path = require('node:path');

const VARSAYILAN_UYGULAMALAR = [
  'money-manager', 'wallet-budgetbakers', 'bluecoins', 'hesap-defterim',
  'goodbudget', 'parasut', 'logo-isbasi', 'kolaybi', 'quickbooks',
];

// README tarafsızlık kuralı. Hüküm vermeyi reddeden cümleler yasak değildir;
// kalıplar bilinçli olarak dar tutuldu.
const YASAK_KALIPLAR = [
  'tam da kaçındığımız', 'bizim tezimizin', 'zaten çözüyor', 'neden doğru olduğ',
  'daha doğru bir model', 'kaçınmamız gereken', 'negatif referans',
  'yaklaşımımız daha iyi', 'bizimki daha iyi', 'bizim modelimiz daha',
];

// `02`, `06b`, `d07`, `f7-53` — karenin dosya adındaki ilk tireden önceki kimlik.
const KISA_KOD = /^(?:[a-z]\d*-)?[a-z]?\d{2}[a-z]?$/;
const KOD = '(?:[a-z]\\d*-)?[a-z]?\\d{2}[a-z]?';
const ARALIK = new RegExp('`' + KOD + '`\\s*[–—-]\\s*`' + KOD + '`', 'g');
// Dosya adı karakteri Unicode harf/rakamdır: `işlemler.png` `lemler.png`e kırpılmaz.
const AD = '[\\p{L}\\p{N}_.-]';
const PNG_ATIF = new RegExp(`(?:${AD}+\\/)*${AD}+\\.png(?![\\p{L}\\p{N}_])`, 'gu');
// Boşluklu ad yalnız tek ters tırnaklı kod parçasının tamamıysa tek atıftır
// (`ögeler özeti.png`); içinde ikinci bir `.png` varsa ayrı adlar sayılır.
const BOSLUKLU_PNG = new RegExp(`^(?:${AD}+(?: ${AD}+)*\\/)*${AD}+(?: ${AD}+)*\\.png$`, 'u');

// Satırdaki ters tırnaklı kod parçaları: { bas, son (hariç), n (tırnak sayısı) }.
function kodParcalari(satir) {
  const parcalar = [];
  let i = 0;
  while (i < satir.length) {
    if (satir[i] !== '`') { i++; continue; }
    let n = 0;
    while (satir[i + n] === '`') n++;
    let j = i + n, kapandi = false;
    while (j < satir.length) {
      if (satir[j] !== '`') { j++; continue; }
      let m = 0;
      while (satir[j + m] === '`') m++;
      if (m === n) { parcalar.push({ bas: i, son: j + m, n }); i = j + m; kapandi = true; break; }
      j += m;
    }
    if (!kapandi) i += n;
  }
  return parcalar;
}

function maskele(satir, kosul) {
  const karakterler = satir.split('');
  for (const p of kodParcalari(satir)) {
    if (!kosul(p)) continue;
    for (let k = p.bas; k < p.son; k++) karakterler[k] = ' ';
  }
  return karakterler.join('');
}

// Baş/son ayraç atılır; kaçışlı boru (\|) hücre ayırmaz.
function hucreSayisi(satir) {
  let t = satir.trim();
  if (t.startsWith('|')) t = t.slice(1);
  if (t.endsWith('|') && !t.endsWith('\\|')) t = t.slice(0, -1);
  let n = 1;
  for (let i = 0; i < t.length; i++) if (t[i] === '|' && t[i - 1] !== '\\') n++;
  return n;
}

// Satırdaki .png atıfları metin sırasıyla.
function pngAtiflari(satir) {
  const atiflar = [];
  const karakterler = satir.split('');
  for (const p of kodParcalari(satir)) {
    if (p.n !== 1) continue;
    const ic = satir.slice(p.bas + 1, p.son - 1).trim();
    if (!ic.includes(' ') || /\.png /.test(ic) || !BOSLUKLU_PNG.test(ic)) continue;
    atiflar.push({ i: p.bas, t: ic });
    for (let k = p.bas; k < p.son; k++) karakterler[k] = ' ';
  }
  for (const m of karakterler.join('').matchAll(PNG_ATIF)) atiflar.push({ i: m.index, t: m[0] });
  return atiflar.sort((a, b) => a.i - b.i).map(a => a.t);
}

// Aynı ad NFC/NFD olarak farklı kodlanmış olabilir (ör. macOS kopyası).
function dosyaBul(yol) {
  if (fs.existsSync(yol)) return yol;
  const hedef = path.basename(yol).normalize('NFC');
  try {
    const ad = fs.readdirSync(path.dirname(yol)).find(f => f.normalize('NFC') === hedef);
    return ad ? path.join(path.dirname(yol), ad) : null;
  } catch { return null; }
}

function pngDosyalari(dizin) {
  try { return fs.readdirSync(dizin).filter(f => /\.png$/i.test(f)).sort(); } catch { return []; }
}

function satirListesi(satirlar) {
  return satirlar.slice(0, 5).join(', ') + (satirlar.length > 5 ? '…' : '');
}

function formuDenetle(kok, uygulama) {
  const formYolu = path.join(kok, 'gozlemler', uygulama + '.md');
  const kareDizini = path.join(kok, 'kanitlar', uygulama);
  const sonuc = { uygulama, kare: 0, hatalar: [], uyarilar: [], formYok: false };
  if (!fs.existsSync(formYolu)) { sonuc.formYok = true; return sonuc; }

  const hata = (tur, satir, mesaj) => sonuc.hatalar.push({ tur, satir, mesaj });
  const kareler = pngDosyalari(kareDizini);
  sonuc.kare = kareler.length;

  const tamAdlaAnilan = new Set();
  const kisaKullanim = new Map();
  const satirlar = fs.readFileSync(formYolu, 'utf8').split(/\r?\n/);
  let citli = false, tabloSutun = null;

  for (let i = 0; i < satirlar.length; i++) {
    const no = i + 1, ham = satirlar[i];
    if (/^\s*(```|~~~)/.test(ham)) { citli = !citli; tabloSutun = null; continue; }
    if (citli) continue;

    if (/^\s*\|/.test(ham)) {
      const c = hucreSayisi(ham);
      if (tabloSutun === null) tabloSutun = c;
      else if (c !== tabloSutun) hata('tablo', no, `satır ${c} hücre, başlık ${tabloSutun} sütun`);
    } else {
      tabloSutun = null;
    }

    const ornekSiz = maskele(ham, p => p.n >= 2);

    for (const t of pngAtiflari(ornekSiz)) {
      const adaylar = t.includes('/')
        ? [path.resolve(path.dirname(formYolu), t), path.resolve(kok, t)]
        : [path.join(kareDizini, t)];
      const bulunan = adaylar.map(dosyaBul).find(Boolean);
      if (!bulunan) { hata('ölü atıf', no, t); continue; }
      if (path.dirname(bulunan) === kareDizini) tamAdlaAnilan.add(path.basename(bulunan));
    }

    for (const m of ornekSiz.matchAll(ARALIK)) hata('belirsiz aralık', no, m[0]);

    for (const p of kodParcalari(ornekSiz)) {
      if (p.n !== 1) continue;
      const ic = ornekSiz.slice(p.bas + 1, p.son - 1).trim();
      if (!KISA_KOD.test(ic)) continue;
      const hedefler = kareler.filter(k => k.startsWith(ic + '-'));
      if (hedefler.length === 0) { hata('çözülemeyen kısa kod', no, '`' + ic + '`'); continue; }
      for (const k of hedefler) {
        if (!kisaKullanim.has(k)) kisaKullanim.set(k, []);
        kisaKullanim.get(k).push(no);
      }
    }

    const kodsuz = maskele(ham, () => true);
    for (const kalip of YASAK_KALIPLAR) if (kodsuz.includes(kalip)) hata('yasak kalıp', no, kalip);
  }

  for (const k of kareler) {
    const kisa = kisaKullanim.get(k);
    if (tamAdlaAnilan.has(k)) {
      if (!kisa) continue;
      const mesaj = `${k} tam adla da anılıyor; ${kisa.length} yerde kısa kodla (satır ${satirListesi(kisa)})`;
      hata('kısa kod', kisa[0], mesaj);
    } else if (kisa) {
      hata('yetim kare', kisa[0], `${k} yalnız kısa kodla anılıyor (satır ${satirListesi(kisa)})`);
    } else {
      hata('yetim kare', 0, `${k} formda hiç anılmıyor`);
    }
  }
  return sonuc;
}

function denetle({ kok, uygulamalar = VARSAYILAN_UYGULAMALAR }) {
  const tamKok = path.resolve(kok);
  const sonuclar = uygulamalar.map(u => formuDenetle(tamKok, u));
  const cikis = sonuclar.some(s => s.formYok) ? 2 : sonuclar.some(s => s.hatalar.length) ? 1 : 0;
  return { sonuclar, cikis };
}

function yazdir({ sonuclar, cikis }) {
  let toplamHata = 0, toplamUyari = 0;
  for (const s of sonuclar) {
    if (s.formYok) {
      console.log(`❌ ${s.uygulama.padEnd(22)} gözlem formu yok (gozlemler/${s.uygulama}.md)`);
      continue;
    }
    toplamHata += s.hatalar.length;
    toplamUyari += s.uyarilar.length;
    const isaret = s.hatalar.length ? '❌' : '✅';
    console.log(`${isaret} ${s.uygulama.padEnd(22)} ${String(s.kare).padStart(3)} kare · ${s.hatalar.length} hata · ${s.uyarilar.length} uyarı`);
    for (const [liste, ad] of [[s.hatalar, 'hata'], [s.uyarilar, 'uyarı']]) {
      const gruplar = new Map();
      for (const b of liste) {
        if (!gruplar.has(b.tur)) gruplar.set(b.tur, []);
        gruplar.get(b.tur).push(b);
      }
      for (const [tur, bulgular] of gruplar) {
        console.log(`   ${ad} · ${tur}: ${bulgular.length}`);
        for (const b of bulgular.slice(0, 8)) console.log(`      ${b.satir ? 'satır ' + b.satir + ': ' : ''}${b.mesaj}`);
        if (bulgular.length > 8) console.log(`      … ${bulgular.length - 8} daha`);
      }
    }
  }
  console.log('');
  if (cikis === 2) console.log('Kullanım hatası: olmayan gözlem formu istendi.');
  else if (cikis === 1) console.log(`Toplam ${toplamHata} hata, ${toplamUyari} uyarı. Kurallar: MANUEL-TEST-PROTOKOLU.md → "Gözlem formu yazım kuralları"`);
  else console.log(`Hata yok (${toplamUyari} uyarı). Yapısal kontroldür; içerik doğruluğu elle kontrol edilir.`);
}

if (require.main === module) {
  const argumanlar = process.argv.slice(2);
  const bilinmeyenBayrak = argumanlar.filter(a => a.startsWith('-') && a !== '--siki');
  const uygulamalar = argumanlar.filter(a => !a.startsWith('-'));
  if (bilinmeyenBayrak.length || uygulamalar.some(u => !/^[a-z0-9-]+$/.test(u))) {
    console.error('Kullanım: node denetim.cjs [uygulama ...] [--siki]');
    process.exit(2);
  }
  const kok = process.env.DENETIM_KOK ? path.resolve(process.env.DENETIM_KOK) : __dirname;
  const sonuc = denetle({ kok, uygulamalar: uygulamalar.length ? uygulamalar : VARSAYILAN_UYGULAMALAR });
  yazdir(sonuc);
  process.exitCode = sonuc.cikis;
}

module.exports = { denetle, hucreSayisi, kodParcalari, pngAtiflari, VARSAYILAN_UYGULAMALAR };
