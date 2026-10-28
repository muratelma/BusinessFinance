// POS tahsilatı paneli · bütün alanlar 56 px. HesapSec.jsx (HField, SelVal, HAccVal) üzerine.
const POSL = [{ name: 'Ziraat POS', acc: 'Ziraat vadesiz' }];
const PosIc = ({ manual }) => <span style={{ width: 20, flex: 'none', display: 'flex', color: manual ? 'var(--ink-muted)' : 'var(--neutral)' }}><I6 n={manual ? 'edit' : 'point_of_sale'} s={20} /></span>;

// Alandaki değer: ortak SelVal (32 px kapsül · 15/20 · 13/16) → alan 56 px
const PosValN = ({ p }) => p ? <SelVal icon="point_of_sale" c="var(--neutral)" t={p.name} s={'POS · ' + p.acc} /> : <SelVal icon="edit" t="Elle gir" />;
const PosAccN = HAccVal;

// Menü H1 ile aynı: başlık / ara yazı / çizgi yok.
const PosRow = ({ p, on }) => <div role="option" aria-selected={on} style={{ position: 'relative', display: 'flex', alignItems: 'center', gap: 12, boxSizing: 'border-box', minHeight: 56, padding: '6px 16px', background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: 'pointer' }}>
  {on && <span style={{ position: 'absolute', left: 0, top: 8, bottom: 8, width: 4, borderRadius: '0 4px 4px 0', background: 'var(--brand)' }}></span>}
  <PosIc manual={!p} />
  <span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: (on ? 600 : 400) + ' 15px/1.3 var(--font-core)' }}>{p ? p.name : 'Elle gir'}</span><span style={{ display: 'block', ...small9 }}>{p ? p.acc : 'POS seçmeden; hesabı siz seçersiniz'}</span></span>
</div>;
const PosMenu = ({ sel }) => <div role="listbox" style={{ position: 'absolute', top: 'calc(100% + 8px)', left: 0, right: 0, background: 'var(--surface-card)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', overflow: 'hidden', padding: '4px 0', animation: 'hmenuIn 150ms cubic-bezier(.2,0,0,1)' }}>
  <style>{'@keyframes hmenuIn{from{opacity:0;transform:translateY(-4px)}to{opacity:1;transform:none}}'}</style>
  {POSL.map((p) => <PosRow key={p.name} p={p} on={sel === p.name} />)}
  <PosRow on={sel === null} />
</div>;

// Komisyon: tam genişlik 48 px (Vazgeç/Kaydet ile aynı), üç eşit parça; seçili = dolgu (--brand), ✓ yok → genişlik değişmez, yazı kaymaz. Tutar/Oran seçilince altında 56 px alan.
const PosSeg = ({ sel = 'Yok' }) => <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}><div style={{ ...small9, paddingLeft: 4 }}>Komisyon</div><div role="radiogroup" aria-label="Komisyon" style={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0, 1fr))', height: 48, boxSizing: 'border-box', border: '1px solid var(--border-strong)', borderRadius: 999, overflow: 'hidden' }}>{['Yok', 'Tutar', 'Oran'].map((l, i) => { const on = sel === l; return <div key={l} role="radio" aria-checked={on} style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', borderLeft: i ? '1px solid var(--border-strong)' : 0, background: on ? 'var(--brand)' : 'transparent', color: on ? 'var(--on-brand)' : 'var(--ink)', font: (on ? 600 : 500) + ' 14px/1 var(--font-core)', cursor: 'pointer' }}>{l}</div>; })}</div>{sel === 'Tutar' && <HField label="Komisyon tutarı" icon={null} />}{sel === 'Oran' && <HField label="Komisyon oranı (%)" icon={null} />}</div>;
// POS seçiliyken: hesap/gün/komisyon POS'tan gelir, gri özet panelde okunur
const PosSum = ({ p }) => <div style={{ background: 'var(--surface-card-muted)', borderRadius: 16, padding: '4px 16px' }}>
  {[['Komisyon (%1,79)', '—'], ['Hesaba geçecek', '—'], ['Beklenen gün', '1 iş günü'], ['Geçeceği hesap', p.acc]].map(([k, v], i) => <div key={k} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 44, borderTop: i ? '1px solid var(--border)' : 0 }}><span style={{ flex: 1, ...sub9 }}>{k}</span><span style={{ font: (v === '—' ? 400 : 600) + ' 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>{v}</span></div>)}
</div>;

function PosSheet({ field, p, katVal }) {
  return <div className="screen" style={{ background: 'var(--overlay)' }}>
    <div style={{ marginTop: 24, flex: 1, minHeight: 0, background: 'var(--surface-card)', borderRadius: '28px 28px 0 0', boxShadow: 'var(--elevation-floating)', display: 'flex', flexDirection: 'column' }}>
      <div style={{ width: 32, height: 4, borderRadius: 999, background: 'var(--border-strong)', margin: '16px auto 0', flex: 'none' }}></div>
      <div className="body" style={{ padding: '24px', display: 'flex', flexDirection: 'column', gap: 16 }}>
        <div><div style={{ font: '700 22px/1.25 var(--font-core)' }}>POS tahsilatı</div><div style={{ ...sub9, marginTop: 6, textWrap: 'pretty' }}>Gelir bugün yazılır, para hesaba sonra geçer.</div></div>
        {field}
        {!p && <HField label="Satış kategorisi">{katVal}</HField>}
        <HField label="Müşterinin ödediği" icon={null} help="Brüt tutar: kestiğiniz fatura budur." />
        {p ? <>
          <HField label="Tahsilat günü" icon="calendar_today"><HText t="28 Ekim 2026" /></HField>
          <PosSum p={p} />
        </> : <>
          <HField label="Tahsilat günü" icon="calendar_today"><HText t="28 Ekim 2026" /></HField>
          <HField label="Paranın beklendiği gün" icon="calendar_today"><HText t="28 Ekim 2026" /></HField>
          <PosSeg />
        </>}
        <HField label="Açıklama (isteğe bağlı)" icon={null} />
      </div>
      <div style={{ flex: 'none', display: 'flex', justifyContent: 'flex-end', alignItems: 'center', gap: 8, padding: '12px 24px 20px', borderTop: '1px solid var(--border)' }}><button style={{ height: 48, padding: '0 16px', border: 0, borderRadius: 16, background: 'transparent', color: 'var(--ink)', font: '600 15px/1 var(--font-core)' }}>Vazgeç</button><button style={{ height: 48, padding: '0 24px', border: 0, borderRadius: 16, background: 'var(--brand)', color: 'var(--on-brand)', font: '600 15px/1 var(--font-core)' }}>Tahsilatı kaydet</button></div>
    </div>
  </div>;
}

const EAcc = (o) => <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}><HField label="POS" focus={o} icon={o ? 'arrow_drop_up' : undefined} menu={o ? <PosMenu sel={null} /> : undefined}><PosValN p={null} /></HField><HField label="Paranın geçeceği hesap"><PosAccN a={HA[4]} /></HField></div>;
const P1 = () => <PosSheet p={POSL[0]} field={<HField label="POS"><PosValN p={POSL[0]} /></HField>} />;
const P1o = () => <PosSheet p={POSL[0]} field={<HField label="POS" focus icon="arrow_drop_up" menu={<PosMenu sel="Ziraat POS" />}><PosValN p={POSL[0]} /></HField>} />;
const P1e = () => <PosSheet field={EAcc(false)} />;
const P1eo = () => <PosSheet field={EAcc(true)} />;
function renderPosSec(R) { R('P1', <P1 />); R('P1o', <P1o />); R('P1e', <P1e />); R('P2', <P1eo />); }
Object.assign(window, { renderPosSec });
