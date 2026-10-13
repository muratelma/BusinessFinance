// Brif 5 + 6 ortak parçaları: başlık, çip (soluk hâliyle), ara başlıklı menü, işlem satırı, boş hâl, ay seçici, alt panel.
// Secici.jsx'teki I6, Card6, Cap6, fmt6, muted6 üzerine kurulur. Yalnız DS token'ları.
const TONE9 = { out: 'var(--expense)', in: 'var(--income)', nt: 'var(--neutral)' };
const Money9 = ({ v, tone, sign, size = 16, w = 600, style }) => <span style={{ fontVariantNumeric: 'tabular-nums', font: w + ' ' + size + 'px/1.25 var(--font-core)', color: TONE9[tone] || 'var(--ink)', whiteSpace: 'nowrap', ...style }}>{sign ? (v < 0 ? '−' : '+') : ''}{fmt6(v)}</span>;
const sub9 = { font: '400 14px/1.35 var(--font-core)', color: 'var(--ink-muted)' };
const small9 = { font: '400 13px/1.35 var(--font-core)', color: 'var(--ink-muted)' };

function Head9({ title, month = 'Ekim 2026', back = true, extra }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 0, padding: '12px 4px 4px 8px' }}>
      {back && <button className="iconbtn" aria-label="Geri"><I6 n="arrow_back" s={24} /></button>}
      <div style={{ flex: 1, minWidth: 0, font: '700 26px/1.2 var(--font-core)', color: 'var(--ink)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis', paddingLeft: back ? 4 : 8 }}>{title}</div>
      {extra}
      {month && <button style={{ flex: 'none', whiteSpace: 'nowrap', display: 'flex', alignItems: 'center', gap: 2, height: 48, padding: '0 6px 0 10px', border: 0, background: 'transparent', borderRadius: 16, font: '600 16px/1 var(--font-core)', color: 'var(--ink)', cursor: 'pointer' }}>{month}<I6 n="expand_more" /></button>}
    </div>
  );
}

// Çip: on (kapsam/yön seçili) · menu (açılır) · value (seçilen ad) · open · faint (seçecek bir şeyi kalmadı)
function Chip9({ label, on, menu, value, open, faint }) {
  const sel = !faint && (on || value || open);
  return (
    <button aria-expanded={menu ? !!open : undefined} aria-disabled={faint || undefined} style={{ flex: 'none', maxWidth: 200, display: 'flex', alignItems: 'center', gap: 4, height: 36, padding: on && !menu ? '0 14px 0 8px' : menu ? '0 6px 0 14px' : '0 14px', borderRadius: 999, border: '1px ' + (faint ? 'dashed var(--border-strong)' : 'solid ' + (sel ? (open ? 'var(--ink)' : 'transparent') : 'var(--border-strong)')), background: faint ? 'transparent' : sel ? 'var(--brand-container)' : 'var(--surface-card)', color: faint ? 'var(--ink-faint)' : 'var(--ink)', font: (faint ? '500' : '600') + ' 14px/1 var(--font-core)', cursor: 'pointer', whiteSpace: 'nowrap' }}>
      {on && !menu && <I6 n="check" s={18} />}
      <span style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{value || label}</span>
      {menu && <I6 n={open ? 'arrow_drop_up' : 'arrow_drop_down'} s={20} />}
    </button>
  );
}
const Chips9 = ({ children, menu, note }) => <div style={{ position: 'relative', zIndex: menu ? 25 : 'auto', flex: 'none' }}><div style={{ display: 'flex', gap: 8, padding: '8px 16px 4px', overflowX: 'auto', scrollbarWidth: 'none' }}>{children}</div>{note}{menu}</div>;

function Seg9({ v = 'Gider' }) {
  const it = (t) => <div style={{ flex: 1, height: 36, borderRadius: 999, background: v === t ? 'var(--surface-card)' : 'transparent', border: v === t ? '1px solid var(--border-strong)' : '1px solid transparent', display: 'flex', alignItems: 'center', justifyContent: 'center', font: '600 15px/1 var(--font-core)', color: v === t ? 'var(--ink)' : 'var(--ink-muted)' }}>{t}</div>;
  return <div style={{ margin: '4px 16px 0', display: 'flex', padding: 4, borderRadius: 999, background: 'var(--surface-card-muted)', border: '1px solid var(--border)' }}>{it('Gider')}{it('Gelir')}</div>;
}

// Menü. Girdi düz liste:
//  { label, sub, amount, tone, sign, icon, indent, head }  seçilebilir satır (head = seçilebilir ara başlık)
//  { dir: 'Giren', amount, tone }                            seçilemeyen yön etiketi (yalnız Tümü hâlinde)
//  { group: 'Nakit ve banka' }                              seçilemeyen grup etiketi
function Menu9({ caption, items, sel = 0, side = 'right', width = 320, maxH = 520 }) {
  const [s, setS] = React.useState(sel);
  let k = -1, first = true;
  return (
    <div role="menu" style={{ position: 'absolute', top: 'calc(100% + 4px)', [side]: 16, width, background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', overflow: 'hidden', display: 'flex', flexDirection: 'column', maxHeight: maxH }}>
      {caption && <div style={{ padding: '12px 16px 8px', ...small9, borderBottom: '1px solid var(--border)' }}>{caption}</div>}
      <div className="menu-scroll" style={{ overflowY: 'auto', padding: '4px 0', scrollbarWidth: 'none' }}>
        {items.map((o, i) => {
          if (o.group) return <div key={i} style={{ borderTop: '1px solid var(--border)', marginTop: 4, padding: '12px 16px 4px', font: '600 12px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' }}>{o.group}</div>;
          if (o.dir) return <div key={i} style={{ borderTop: '1px solid var(--border)', marginTop: 4, padding: '12px 16px 4px', display: 'flex', alignItems: 'baseline', gap: 8, font: '600 12px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' }}><span style={{ flex: 1 }}>{o.dir}</span><Money9 v={o.amount} tone={o.tone} sign size={12} /></div>;
          const id = ++k, on = s === id;
          const divider = o.head && !first; first = false;
          return (
            <button key={i} role="menuitemradio" aria-checked={on} onClick={() => setS(id)} style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: o.sub ? 56 : 48, padding: '4px 16px', border: 0, borderTop: divider ? '1px solid var(--border)' : 0, marginTop: divider ? 4 : 0, background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: 'pointer', textAlign: 'left', color: 'var(--ink)', font: 'inherit' }}>
              <span style={{ width: 20, flex: 'none', display: 'flex', color: on ? 'var(--ink)' : 'var(--ink-muted)' }}>{on ? <I6 n="check" s={20} style={{ fontVariationSettings: '"wght" 600' }} /> : o.icon ? <I6 n={o.icon} s={20} /> : id === 0 ? <I6 n="layers" s={20} /> : null}</span>
              <span style={{ flex: 1, minWidth: 0 }}>
                <span style={{ display: 'block', font: (on || o.head ? '600' : '400') + ' 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{o.label}</span>
                {o.sub && <span style={{ display: 'block', ...small9 }}>{o.sub}</span>}
              </span>
              {o.amount != null && <Money9 v={o.amount} tone={o.tone} sign={o.sign} size={15} w={on || o.head ? 700 : 600} style={{ flex: 'none' }} />}
            </button>
          );
        })}
      </div>
    </div>
  );
}

// Durum yazısı (AppStatusTag): ikon 16 + metin 13/600, dolgusuz.
const TAG9 = { paid: ['check_circle', 'var(--income)'], open: ['schedule', 'var(--ink-muted)'], transit: ['schedule', 'var(--neutral)'], passive: ['pause_circle', 'var(--ink-muted)'] };
// bare: işlem satırının üçüncü satırı — ikonsuz, tek satır, sığmazsa kısalır.
const Tag9 = ({ kind, label, bare }) => bare
  ? <span style={{ display: 'block', color: TAG9[kind][1], font: '600 13px/1.35 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{label}</span>
  : <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, color: TAG9[kind][1], font: '600 13px/1.3 var(--font-core)' }}><I6 n={TAG9[kind][0]} s={16} />{label}</span>;

// İşlem satırı: kapsül · başlık (en çok 2 satır) · alt yazı · durum (tek satır) · tutar başlık hizasında · tutarın altı
function Tx9({ icon, cap = 'out', title, sub, tag, amount, tone, sign = true, under, flat, sepTop }) {
  return (
    <div style={{ borderTop: sepTop ? '1px solid var(--border)' : 0, display: 'flex', alignItems: flat ? 'center' : 'flex-start', gap: flat ? 16 : 14, padding: '12px 16px', minHeight: flat ? 64 : undefined, boxSizing: 'border-box' }}>
      <Cap6 n={icon} tone={cap} />
      <div style={{ flex: 1, minWidth: 0, paddingTop: flat ? 0 : 1 }}>
        <div style={{ font: '600 16px/1.3 var(--font-core)', color: 'var(--ink)', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>{title}</div>
        {sub && <div style={{ ...sub9, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>{sub}</div>}
        {tag && <Tag9 {...tag} bare />}
      </div>
      <div style={{ flex: 'none', display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 2, maxWidth: 160, paddingTop: flat ? 0 : 1 }}>
        <Money9 v={amount} tone={tone} sign={sign} />
        {under && <span style={{ ...small9, textAlign: 'right' }}>{under}</span>}
      </div>
    </div>
  );
}
// Gün başlığı: tarih (koyu) + gün adı (soluk), İşlemler'deki gibi.
const GUN9 = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
const wd9 = (d) => { const [n, m] = d.split(' '); const i = AYL9.indexOf(m); return i < 0 ? '' : GUN9[new Date(2026, i, +n).getDay()]; };
const Day9 = ({ d, wd }) => <div style={{ display: 'flex', alignItems: 'baseline', gap: 8, padding: '20px 16px 8px' }}><span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{d}</span><span style={sub9}>{wd || wd9(d)}</span></div>;
const sep9 = (inset) => <div style={{ height: 1, background: 'var(--border)', marginLeft: inset }}></div>;
// Aynı günün işlemleri tek kartta. flat = İşlemler'deki gibi tam genişlik beyaz blok + gün adı.
function Days9({ days, flat }) {
  return days.map(([d, rows, wd]) => (
    <div key={d}>
      <Day9 d={d} wd={wd} />
      <div style={flat ? { background: 'var(--surface-card)', borderTop: '1px solid var(--border)', borderBottom: '1px solid var(--border)' } : { margin: '0 16px', background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 20, overflow: 'hidden' }}>
        {rows.map((r, i) => <React.Fragment key={i}>{i > 0 && sep9(flat ? 72 : 70)}<Tx9 {...r} flat={flat} /></React.Fragment>)}
      </div>
    </div>
  ));
}

function Empty9({ title, text, action }) {
  return (
    <div style={{ padding: '40px 32px', textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6 }}>
      <span style={{ width: 56, height: 56, borderRadius: 999, background: 'var(--surface-card-muted)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--ink-muted)', marginBottom: 6 }}><I6 n="event_busy" s={28} /></span>
      <div style={{ font: '600 17px/1.3 var(--font-core)', color: 'var(--ink)', textWrap: 'pretty' }}>{title}</div>
      {text && <div style={{ ...sub9, textWrap: 'pretty' }}>{text}</div>}
      {action && <button style={{ marginTop: 6, height: 48, padding: '0 16px', border: 0, background: 'transparent', font: '600 15px/1 var(--font-core)', color: 'var(--ink)', textDecoration: 'underline', cursor: 'pointer' }}>{action}</button>}
    </div>
  );
}

// Özet kartı: etiket · büyük tutar · alt satır
const Sum9 = ({ label, v, tone, foot, children }) => (
  <Card6 style={{ padding: '16px 20px', marginTop: 8 }}>
    <div style={sub9}>{label}</div>
    <div style={{ margin: '4px 0 2px' }}><Money9 v={v} tone={tone} size={28} w={700} /></div>
    {foot && <div style={sub9}>{foot}</div>}
    {children}
  </Card6>
);

// Alt panel (telefonun en altından, alt çubuğun üstüne biner)
function Sheet9({ children, title, sub }) {
  return (
    <div className="sheet-scrim" style={{ zIndex: 30 }}>
      <div role="dialog" style={{ width: '100%', maxHeight: '90%', display: 'flex', flexDirection: 'column', background: 'var(--surface-card)', borderRadius: '28px 28px 0 0', boxShadow: 'var(--elevation-floating)' }}>
        <div style={{ display: 'flex', justifyContent: 'center', padding: '12px 0 4px' }}><span style={{ width: 32, height: 4, borderRadius: 999, background: 'var(--border-strong)' }}></span></div>
        {title && <div style={{ padding: '8px 24px 4px' }}><div style={{ font: '600 20px/1.3 var(--font-core)' }}>{title}</div>{sub && <div style={{ ...sub9, marginTop: 2 }}>{sub}</div>}</div>}
        <div style={{ flex: 1, minHeight: 0, overflowY: 'auto', scrollbarWidth: 'none', padding: '8px 16px 24px' }}>{children}</div>
      </div>
    </div>
  );
}
// Etiket–değer bloğu (AppDetailBlock)
function KV9({ rows, dashedFrom }) {
  return (
    <div style={{ padding: '0 16px', borderRadius: 16, background: 'var(--surface-card-muted)' }}>
      {rows.map(([k, v, tone, strong], i) => (
        <div key={i} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 44, padding: '6px 0', borderTop: i ? '1px solid var(--border)' : 0 }}>
          <span style={{ ...sub9, flex: 'none' }}>{k}</span>
          <span style={{ flex: 1, minWidth: 0, textAlign: 'right', font: (strong ? '700' : '500') + ' 14px/1.35 var(--font-core)', color: TONE9[tone] || 'var(--ink)', fontVariantNumeric: 'tabular-nums' }}>{v}</span>
        </div>
      ))}
    </div>
  );
}
const Btn9 = ({ label, icon, kind = 'outlined', danger }) => <button style={{ width: '100%', height: 52, borderRadius: 16, border: kind === 'filled' ? 0 : '1px solid ' + (danger ? 'var(--expense)' : 'var(--border-strong)'), background: kind === 'filled' ? 'var(--brand)' : 'transparent', color: kind === 'filled' ? 'var(--on-brand)' : danger ? 'var(--expense)' : 'var(--ink)', font: '600 16px/1 var(--font-core)', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8, cursor: 'pointer' }}>{icon && <I6 n={icon} s={20} />}{label}</button>;

// Ay seçici (K2): değerler dışarıdan; 0 = kaydı yok (geçmişte seçilebilir, çubuk yok), ileri ay kaydı yoksa kapalı.
const AYK9 = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];
const AYL9 = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
function MonthDlg9({ caption, values, now = 9, sel = 9, year = 2026, bar = 'var(--expense-fill)' }) {
  const [s, setS] = React.useState(sel);
  const max = Math.max(...values);
  return (
    <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', zIndex: 30 }}>
      <div role="dialog" aria-label="Ay seç" style={{ width: 348, background: 'var(--surface-card)', borderRadius: 28, boxShadow: 'var(--elevation-floating)', overflow: 'hidden' }}>
        <div style={{ padding: '20px 24px 16px', borderBottom: '1px solid var(--border)' }}>
          <div style={small9}>{caption}</div>
          <div style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'space-between', gap: 12, flexWrap: 'wrap', marginTop: 4 }}><span style={{ font: '700 22px/1.25 var(--font-core)' }}>{AYL9[s]} {year}</span><Money9 v={values[s] || 0} /></div>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', padding: '8px 12px 4px' }}><button className="iconbtn" aria-label="Önceki yıl"><I6 n="chevron_left" s={24} /></button><div style={{ flex: 1, textAlign: 'center', font: '600 18px/1 var(--font-core)' }}>{year}</div><button className="iconbtn" aria-label="Sonraki yıl" disabled style={{ color: 'var(--border-strong)' }}><I6 n="chevron_right" s={24} /></button></div>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4,minmax(0,1fr))', gap: 8, padding: '0 16px' }}>
          {AYK9.map((m, i) => {
            const v = values[i] || 0, fut = i > now, off = fut && !v, on = s === i, cur = i === now;
            return (
              <button key={m} disabled={off} aria-pressed={on} onClick={() => setS(i)} style={{ position: 'relative', height: 60, borderRadius: 16, border: on ? '2px solid var(--brand)' : cur ? '2px solid var(--ink)' : off ? '1px dashed var(--border-strong)' : '1px solid var(--border)', background: on ? 'var(--brand)' : off ? 'transparent' : 'var(--surface-card-muted)', color: on ? 'var(--on-brand)' : off ? 'var(--ink-faint)' : 'var(--ink)', font: (on || cur ? '700' : '500') + ' 15px/1 var(--font-core)', cursor: off ? 'default' : 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', paddingBottom: v ? 8 : 0 }}>
                {m}
                {v > 0 && <span aria-hidden="true" style={{ position: 'absolute', left: 12, right: 12, bottom: 10, height: 4, borderRadius: 999, background: on ? 'var(--ink-muted)' : 'var(--border)', overflow: 'hidden' }}><span style={{ display: 'block', height: '100%', width: Math.max(10, v / max * 100) + '%', borderRadius: 999, background: on ? 'var(--on-brand)' : bar }}></span></span>}
              </button>
            );
          })}
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '12px 12px 16px' }}>
          {s !== now && <button style={{ height: 48, padding: '0 16px', border: 0, background: 'transparent', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}>Bu ay</button>}
          <span style={{ flex: 1 }}></span>
          <button style={{ height: 48, padding: '0 16px', border: 0, background: 'transparent', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}>Vazgeç</button>
          <button style={{ height: 48, padding: '0 24px', border: 0, borderRadius: 999, background: 'var(--brand)', color: 'var(--on-brand)', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}>Göster</button>
        </div>
      </div>
    </div>
  );
}
const Screen9 = ({ children, overlay }) => <div className="screen">{children}{overlay}</div>;
const Body9 = ({ children }) => <div className="body" style={{ paddingBottom: 32 }}>{children}</div>;

Object.assign(window, { Money9, sub9, small9, Head9, Chip9, Chips9, Seg9, Menu9, Tag9, Tx9, Day9, Days9, Empty9, Sum9, Sheet9, KV9, Btn9, MonthDlg9, Screen9, Body9 });
