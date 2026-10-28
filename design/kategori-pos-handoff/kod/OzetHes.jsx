// Özet · Hesap bakiyeleri — özet ölçüsünde öneriler (Z1–Z5). HesapSec.jsx parçaları üzerine.
const ZACT = HA.filter((a) => a.k === 'nakit' || a.k === 'banka').sort((a, b) => b.v - a.v);
const ZPAS = HA.find((a) => a.k === 'pasif');
const zSum = (l) => l.reduce((t, a) => t + a.v, 0);
const ZChev = () => <I6 n="chevron_right" s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} />;

function ZScope() {
  const it = (t) => <div key={t} style={{ flex: 1, height: 40, borderRadius: 999, background: t === 'Hepsi' ? 'var(--surface-card)' : 'transparent', border: t === 'Hepsi' ? '1px solid var(--border-strong)' : '1px solid transparent', display: 'flex', alignItems: 'center', justifyContent: 'center', font: (t === 'Hepsi' ? 600 : 500) + ' 14px/1 var(--font-core)', color: t === 'Hepsi' ? 'var(--ink)' : 'var(--ink-muted)' }}>{t}</div>;
  return <div style={{ margin: '4px 16px 8px', display: 'flex', padding: 4, borderRadius: 999, background: 'var(--surface-card-muted)', border: '1px solid var(--border)' }}>{['Hepsi', 'İşletme', 'Şahsi'].map(it)}</div>;
}
// Bölüm başlığı Özet'in dilinde: "Kategori giderleri · 10 kategori ›" gibi.
const ZHead = ({ t = 'Hesap bakiyeleri', r }) => <div style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '28px 12px 8px 20px', cursor: 'pointer' }}><div style={{ flex: 1, font: '600 18px/1.3 var(--font-core)' }}>{t}</div>{r && <span style={sub9}>{r}</span>}<ZChev /></div>;
const ZTail = () => <div style={{ margin: '0 16px', background: 'var(--surface-card)', border: '1px solid var(--border)', borderTop: 0, borderRadius: '0 0 20px 20px', padding: '8px 16px 14px', display: 'flex', alignItems: 'center', gap: 14 }}><Cap6 n="account_balance" tone="out" /><div style={{ flex: 1 }}><div style={{ font: '600 16px/1.3 var(--font-core)' }}>Borç</div><div style={sub9}>faiz hariç</div></div><Money9 v={640} tone="out" /></div>;
function ZScreen({ children, tail = true }) {
  return <div className="screen">
    <div style={{ display: 'flex', alignItems: 'center', padding: '16px 12px 4px 20px' }}><div style={{ flex: 1, font: '700 26px/1.2 var(--font-core)' }}>Özet</div><button className="iconbtn" aria-label="Hesabım"><span style={{ width: 32, height: 32, borderRadius: 999, background: 'var(--brand)', color: 'var(--on-brand)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}><I6 n="person" s={18} /></span></button></div>
    <ZScope />
    <div className="body" style={{ paddingBottom: 56 }}>{tail && <ZTail />}{children}</div>
    <HNav on="Özet" />
  </div>;
}

// Hesap bakiyeleri · Özet'in kendi dili: Varlık durumu kartındaki "● Varlıklar ₺…" grup satırı. Pasif yok, hesap sayısı yok.
const ZGRP = [{ l: 'Nakit', k: 'nakit', icon: 'payments' }, { l: 'Banka', k: 'banka', icon: 'account_balance' }];
const zList = (k) => ZACT.filter((a) => a.k === k);
const ZNote = () => <div style={{ ...small9, padding: '0 20px 10px', marginTop: -6 }}>Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.</div>;
const ZName = ({ t, w = 500 }) => <span style={{ flex: 1, minWidth: 0, font: w + ' 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{t}</span>;
// Grup satırı: Varlık durumu'ndaki gibi 15/600, sağda toplam
const ZGrp = ({ g, lead, top, sum = true, pad = '14px 16px 6px', mt = 6 }) => <div style={{ display: 'flex', alignItems: 'center', gap: 10, padding: pad, borderTop: top ? '1px solid var(--border)' : 0, marginTop: top ? mt : 0 }}>{lead}<span style={{ flex: 1, font: '600 15px/1.3 var(--font-core)' }}>{g.l}</span>{sum && <Money9 v={zSum(zList(g.k))} size={15} />}</div>;
const zDot = (k) => <span style={{ width: 8, height: 8, borderRadius: 999, background: HC[k], flex: 'none' }}></span>;

// V1 · Varlık durumu gibi: nokta + grup toplamı; satırda simge, ad, kapsam, bakiye
const V1 = () => <ZScreen><ZHead />
  <Card6 style={{ marginTop: 0, paddingBottom: 6 }}>{ZGRP.map((g, gi) => <div key={g.k}><ZGrp g={g} top={gi > 0} lead={zDot(g.k)} sum={false} />
    {zList(g.k).map((a) => <div key={a.name} style={{ display: 'flex', alignItems: 'center', gap: 14, minHeight: 56, padding: '4px 16px', cursor: 'pointer' }}><HGlyph a={a} /><div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{a.name}</div><div style={sub9}>{a.sc}</div></div><Money9 v={a.v} style={{ flex: 'none' }} /></div>)}
  </div>)}</Card6></ZScreen>;

// V2 · Sık: grup satırında renkli simge bir kez; hesaplar simgesiz, ad ve bakiye tek satır
const V2 = () => <ZScreen><ZHead /><ZNote />
  <Card6 style={{ marginTop: 0, paddingBottom: 6 }}>{ZGRP.map((g, gi) => <div key={g.k}><ZGrp g={g} top={gi > 0} lead={<HGlyph a={g} s={32} />} />
    {zList(g.k).map((a) => <div key={a.name} style={{ display: 'flex', alignItems: 'center', gap: 8, minHeight: 44, padding: '0 16px 0 58px', cursor: 'pointer' }}><ZName t={a.name} w={400} /><span style={{ ...small9, flex: 'none' }}>{a.sc}</span><Money9 v={a.v} size={15} w={500} style={{ flex: 'none', minWidth: 104, textAlign: 'right' }} /></div>)}
  </div>)}</Card6></ZScreen>;

// V3 · Orta yol: satır 52 px, simge 32 küçük kapsülde, kapsam yalnız Şahsi'de (işletme varsayılan)
const V3 = () => <ZScreen><ZHead /><ZNote />
  <Card6 style={{ marginTop: 0, paddingBottom: 6 }}>{ZGRP.map((g, gi) => <div key={g.k}><ZGrp g={g} top={gi > 0} lead={zDot(g.k)} />
    {zList(g.k).map((a) => <div key={a.name} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 52, padding: '0 16px', cursor: 'pointer' }}><HGlyph a={a} s={32} /><ZName t={a.name} />{a.sc === 'Şahsi' && <span style={{ flex: 'none', height: 24, padding: '0 8px', borderRadius: 999, background: 'var(--surface-card-muted)', color: 'var(--ink-muted)', display: 'inline-flex', alignItems: 'center', font: '600 12px/1 var(--font-core)' }}>Şahsi</span>}<Money9 v={a.v} size={15} style={{ flex: 'none', minWidth: 104, textAlign: 'right' }} /></div>)}
  </div>)}</Card6></ZScreen>;

// V1.2 · V1 + boşluk düzeni: başlık üstte daha çok, altta daha az pay alır; kendi hesaplarına yapışır.
const V12 = () => <ZScreen><ZHead />
  <Card6 style={{ marginTop: 0, paddingBottom: 8 }}>{ZGRP.map((g, gi) => <div key={g.k}><ZGrp g={g} top={gi > 0} lead={zDot(g.k)} sum={false} pad="18px 16px 0" mt={10} />
    {zList(g.k).map((a) => <div key={a.name} style={{ display: 'flex', alignItems: 'center', gap: 14, minHeight: 56, padding: '4px 16px', cursor: 'pointer' }}><HGlyph a={a} /><div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{a.name}</div><div style={sub9}>{a.sc}</div></div><Money9 v={a.v} style={{ flex: 'none' }} /></div>)}
  </div>)}</Card6></ZScreen>;

function renderOzetHes(R) { R('V1', <V1 />); R('V12', <V12 />); R('V2', <V2 />); R('V3', <V3 />); }
Object.assign(window, { renderOzetHes });
