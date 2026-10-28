// Ay seçici + filtre seçici önerileri. Yalnız DS token'ları.
const I6 = ({ n, s = 20, f, style }) => <span className="material-symbols-outlined" aria-hidden="true" data-filled={f ? 'true' : undefined} style={{ fontSize: s, width: s, height: s, ...style }}>{n}</span>;
const fmt6 = (v) => '₺' + Math.abs(v).toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const Money6 = ({ v, tone, sign, style }) => {
  const c = tone === 'in' ? 'var(--income)' : tone === 'out' ? 'var(--expense)' : 'var(--ink)';
  return <span style={{ fontVariantNumeric: 'tabular-nums', fontWeight: 600, color: c, whiteSpace: 'nowrap', ...style }}>{sign ? (v < 0 ? '−' : '+') : ''}{fmt6(v)}</span>;
};
const AY6 = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
const muted6 = { color: 'var(--ink-muted)', font: '400 14px/1.35 var(--font-core)' };

const NAV_BACK6 = window.NavBack || (window.NavBack = React.createContext(() => {}));
function Header6({ title, month = 'Ekim 2026', stepper, onMonth }) {
  const back = React.useContext(NAV_BACK6);
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '12px 8px 4px' }}>
      <button className="iconbtn" aria-label="Geri" onClick={back}><I6 n="arrow_back" s={24} /></button>
      <div style={{ flex: 1, minWidth: 0, font: '700 26px/1.2 var(--font-core)', color: 'var(--ink)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{title}</div>
      {!stepper && <button onClick={onMonth} style={{ flex: 'none', whiteSpace: 'nowrap', display: 'flex', alignItems: 'center', gap: 4, height: 48, padding: '0 8px 0 12px', border: 0, background: 'transparent', borderRadius: 16, font: '600 16px/1 var(--font-core)', color: 'var(--ink)', cursor: 'pointer' }}>{month}<I6 n="expand_more" /></button>}
    </div>
  );
}
// A2: dönem şeridi — ±1 ay tek dokunuş, orta etiket paneli açar.
function PeriodBar({ month = 'Ekim 2026', nextOff = true }) {
  const b = { width: 48, height: 48, border: 0, background: 'transparent', borderRadius: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer', color: 'var(--ink)' };
  return (
    <div style={{ margin: '4px 16px 0', display: 'flex', alignItems: 'center', border: '1px solid var(--border)', borderRadius: 999, background: 'var(--surface-card)' }}>
      <button style={b} aria-label="Önceki ay"><I6 n="chevron_left" s={24} /></button>
      <button style={{ ...b, flex: 1, width: 'auto', gap: 4, font: '600 16px/1 var(--font-core)' }}>{month}<I6 n="expand_more" /></button>
      <button style={{ ...b, color: nextOff ? 'var(--border-strong)' : 'var(--ink)' }} aria-label="Sonraki ay" disabled={nextOff}><I6 n="chevron_right" s={24} /></button>
    </div>
  );
}

function Chip6({ label, on, menu, value, open, onClick }) {
  const sel = on || value || open;
  return (
    <button onClick={onClick} aria-expanded={menu ? !!open : undefined} style={{ flex: 'none', maxWidth: 170, display: 'flex', alignItems: 'center', gap: 4, height: 36, padding: on && !menu ? '0 14px 0 8px' : menu ? '0 6px 0 14px' : '0 14px', borderRadius: 999, border: '1px solid ' + (sel ? (open ? 'var(--ink)' : 'transparent') : 'var(--border-strong)'), background: sel ? 'var(--brand-container)' : 'var(--surface-card)', color: 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer', whiteSpace: 'nowrap' }}>
      {on && !menu && <I6 n="check" s={18} />}
      <span style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>{value || label}</span>
      {menu && <I6 n={open ? 'arrow_drop_up' : 'arrow_drop_down'} s={20} />}
    </button>
  );
}
const ChipRow = ({ children, menu }) => <div style={{ position: 'relative', zIndex: menu ? 25 : 'auto' }}><div style={{ display: 'flex', gap: 8, padding: '12px 16px 4px', overflowX: 'auto', scrollbarWidth: 'none' }}>{children}</div>{menu}</div>;

function Seg6({ a = 'Gider', b = 'Gelir' }) {
  return (
    <div style={{ margin: '4px 16px 0', display: 'flex', padding: 4, borderRadius: 999, background: 'var(--surface-card-muted)', border: '1px solid var(--border)' }}>
      <div style={{ flex: 1, height: 36, borderRadius: 999, background: 'var(--surface-card)', border: '1px solid var(--border-strong)', display: 'flex', alignItems: 'center', justifyContent: 'center', font: '600 15px/1 var(--font-core)' }}>{a}</div>
      <div style={{ flex: 1, height: 36, display: 'flex', alignItems: 'center', justifyContent: 'center', font: '600 15px/1 var(--font-core)', color: 'var(--ink-muted)' }}>{b}</div>
    </div>
  );
}
const Card6 = ({ children, style }) => <div style={{ margin: '12px 16px 0', background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 20, ...style }}>{children}</div>;
const Cap6 = ({ n, tone = 'out', s = 40 }) => {
  const m = { out: ['var(--expense-container)', 'var(--on-expense-container)'], in: ['var(--income-container)', 'var(--on-income-container)'], nt: ['var(--neutral-container)', 'var(--on-neutral-container)'], mu: ['var(--surface-card-muted)', 'var(--ink)'] }[tone];
  return <span style={{ flex: 'none', width: s, height: s, borderRadius: 999, background: m[0], color: m[1], display: 'flex', alignItems: 'center', justifyContent: 'center' }}><I6 n={n} /></span>;
};

const KAT6 = [['inventory_2', 'Ticari mal alımı', 12480, 47], ['storefront', 'İşyeri kirası', 6500, 24], ['badge', 'Personel ücreti', 4200, 16], ['bolt', 'Elektrik, su, doğalgaz', 1860, 7], ['percent', 'Banka ve POS komisyonu', 1140, 4], ['note_add', 'Kasa farkı', 150, 1]];
function KategoriOzeti({ month, stepper, hesap, menu, overlay, sumLabel = 'Ekim gideri · İşletme', total = 26330, onHesap, onOpen, onMonth }) {
  return (
    <div className="screen">
      <Header6 title="Kategori özeti" month={month} stepper={stepper} onMonth={onMonth} />
      {stepper && <PeriodBar month={month} />}
      <Seg6 />
      <ChipRow menu={menu}><Chip6 label="Hepsi" /><Chip6 label="İşletme" on /><Chip6 label="Şahsi" /><Chip6 label="Hesap / kart" menu value={hesap} open={!!menu} onClick={onHesap} /></ChipRow>
      <div className="body">
        <Card6 style={{ padding: '16px 20px' }}>
          <div style={muted6}>{sumLabel}</div>
          <div style={{ font: '700 28px/1.25 var(--font-core)', fontVariantNumeric: 'tabular-nums', margin: '4px 0' }}>{fmt6(total)}</div>
          <div style={muted6}>6 kategori</div>
        </Card6>
        <Card6 style={{ overflow: 'hidden' }}>
          {KAT6.map(([ic, t, v, p], i) => (
            <div key={t} onClick={onOpen ? () => onOpen(t) : undefined} style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '14px 12px 14px 16px', borderTop: i ? '1px solid var(--border)' : 0, cursor: onOpen ? 'pointer' : 'default' }}>
              <Cap6 n={ic} />
              <div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{t}</div><div style={muted6}>Pay %{p}</div></div>
              <div style={{ width: 96, display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 6 }}><Money6 v={v} /><div style={{ width: '100%', height: 6, borderRadius: 999, background: 'var(--surface-card-muted)' }}><div style={{ width: p + '%', height: '100%', borderRadius: 999, background: 'var(--expense-fill)' }}></div></div></div>
              <I6 n="chevron_right" style={{ color: 'var(--ink-muted)' }} />
            </div>
          ))}
        </Card6>
      </div>
      {overlay}
    </div>
  );
}

function HesapDetay({ overlay, kat, menu }) {
  const rows = [['5 Ekim', [['move_to_inbox', 'nt', 'POS yatışı', 'Banka ve POS · Ziraat Vadesiz', 7910, null]]], ['3 Ekim', [['south_west', 'in', 'Havale ile satış', 'Satış geliri · Ziraat Vadesiz', 10490, 'in']]], ['1 Ekim', [['north_east', 'out', 'Ekim kirası', 'İşyeri kirası · Ziraat Vadesiz', -6500, 'out'], ['north_east', 'out', 'Elektrik faturası', 'Elektrik, su, doğalgaz · Ziraat Vadesiz', -1860, 'out']]]];
  return (
    <div className="screen">
      <Header6 title="Ziraat Vadesiz" />
      <ChipRow menu={menu}><Chip6 label="Tümü" on /><Chip6 label="Giren" /><Chip6 label="Çıkan" /><Chip6 label="Kategori" menu value={kat} open={!!menu} /></ChipRow>
      <div className="body">
        <Card6 style={{ padding: '16px 20px' }}>
          <div style={muted6}>Bakiye · bugün</div>
          <div style={{ font: '700 28px/1.25 var(--font-core)', fontVariantNumeric: 'tabular-nums', margin: '4px 0 12px' }}>{fmt6(13916.87)}</div>
          <div style={{ display: 'flex', borderTop: '1px solid var(--border)', paddingTop: 12 }}><div style={{ flex: 1 }}><div style={muted6}>Ekim giren</div><Money6 v={18400} tone="in" /></div><div style={{ flex: 1 }}><div style={muted6}>Ekim çıkan</div><Money6 v={9250} tone="out" /></div></div>
        </Card6>
        {rows.map(([d, list]) => (
          <div key={d}><div style={{ margin: '20px 16px 0', font: '600 15px/1 var(--font-core)', color: 'var(--ink-muted)' }}>{d}</div>
            <Card6 style={{ marginTop: 8, overflow: 'hidden' }}>{list.map(([ic, tn, t, s, v, tone], i) => (
              <div key={t} style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '14px 16px', borderTop: i ? '1px solid var(--border)' : 0 }}><Cap6 n={ic} tone={tn} /><div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)' }}>{t}</div><div style={{ ...muted6, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{s}</div></div><Money6 v={v} tone={tone} sign={!!tone} /></div>
            ))}</Card6></div>
        ))}
      </div>
      {overlay}
    </div>
  );
}

// ---------- Ortak alt panel ----------
function Sheet6({ title, sub, children, footer, tall }) {
  return (
    <div className="sheet-scrim" style={{ zIndex: 20 }}>
      <div role="dialog" aria-label={title} style={{ width: '100%', maxHeight: tall ? '86%' : '78%', display: 'flex', flexDirection: 'column', background: 'var(--surface-card)', borderRadius: '28px 28px 0 0', boxShadow: 'var(--elevation-floating)' }}>
        <div style={{ display: 'flex', justifyContent: 'center', padding: '12px 0 4px' }}><span style={{ width: 32, height: 4, borderRadius: 999, background: 'var(--border-strong)' }}></span></div>
        {title && <div style={{ padding: '8px 24px 8px' }}><div style={{ font: '600 18px/1.3 var(--font-core)' }}>{title}</div>{sub && <div style={{ ...muted6, marginTop: 2, textWrap: 'pretty' }}>{sub}</div>}</div>}
        <div style={{ flex: 1, minHeight: 0, overflowY: 'auto', scrollbarWidth: 'none', paddingBottom: footer ? 0 : 24 }}>{children}</div>
        {footer && <div style={{ padding: '12px 16px 20px', borderTop: '1px solid var(--border)', display: 'flex', gap: 8 }}>{footer}</div>}
      </div>
    </div>
  );
}

// Seçenek satırı: kapsül ikon · ad + alt satır · tutar · seçim işareti (sağda, sabit sütun).
function Opt6({ icon, tone = 'mu', label, sub, amount, amountTone, sign, selected, onClick }) {
  return (
    <button role="radio" aria-checked={!!selected} onClick={onClick} style={{ display: 'flex', alignItems: 'center', gap: 14, width: '100%', minHeight: 64, padding: '8px 16px 8px 24px', border: 0, background: selected ? 'var(--surface-card-muted)' : 'transparent', textAlign: 'left', cursor: 'pointer', font: 'inherit', color: 'var(--ink)' }}>
      {icon && <Cap6 n={icon} tone={tone} />}
      <span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: (selected ? '600' : '500') + ' 16px/1.3 var(--font-core)' }}>{label}</span>{sub && <span style={{ display: 'block', ...muted6 }}>{sub}</span>}</span>
      {amount != null && <Money6 v={amount} tone={amountTone} sign={sign} style={{ fontSize: 15 }} />}
      <span style={{ width: 24, flex: 'none', display: 'flex' }}>{selected && <I6 n="check" s={24} />}</span>
    </button>
  );
}
const Group6 = ({ children }) => <div style={{ padding: '16px 24px 6px', font: '600 13px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' }}>{children}</div>;
const Search6 = ({ ph }) => <div style={{ margin: '4px 16px 8px', height: 48, display: 'flex', alignItems: 'center', gap: 10, padding: '0 14px', border: '1px solid var(--border-strong)', borderRadius: 16, color: 'var(--ink-faint)', font: '400 16px/1 var(--font-core)' }}><I6 n="search" />{ph}</div>;

function FilterSheet({ title, sub, groups, search, initial = 0, footer, tall }) {
  const [sel, setSel] = React.useState(initial);
  let k = 0;
  return (
    <Sheet6 title={title} sub={sub} footer={footer} tall={tall}>
      {search && <Search6 ph={search} />}
      {groups.map((g, gi) => (
        <div key={gi}>{g.label && <Group6>{g.label}</Group6>}
          {g.items.map((o) => { const id = k++; return <Opt6 key={id} {...o} selected={sel === id} onClick={() => setSel(id)} />; })}
        </div>
      ))}
    </Sheet6>
  );
}

const HESAP_GROUPS = [{ items: [{ icon: 'select_all', label: 'Hepsi', sub: '3 hesap', amount: 26330 }, { icon: 'payments', label: 'Dükkan Kasası', sub: 'Nakit', amount: 9840 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', sub: 'Banka', amount: 12560 }, { icon: 'credit_card', label: 'Bonus Kart', sub: 'Kredi kartı', amount: 3930 }] }];
const KISI_GROUPS = [{ items: [{ icon: 'select_all', label: 'Hepsi', amount: 12480 }] }, { label: 'Tedarikçi', items: [{ icon: 'local_shipping', label: 'Örnek Toptan Kağıt', sub: '2 işlem', amount: 3180 }, { icon: 'local_shipping', label: 'Test Toptancı', sub: '1 işlem', amount: 2600 }, { icon: 'local_shipping', label: 'Merkez Hal', sub: '1 işlem', amount: 2500 }] }, { items: [{ icon: 'person_off', label: 'Kişi atanmamış', sub: '3 işlem', amount: 4200 }] }];
const KAT_GROUPS = [{ items: [{ icon: 'select_all', label: 'Hepsi', sub: '4 kategori' }] }, { label: 'Giren', items: [{ icon: 'sell', tone: 'in', label: 'Satış geliri', amount: 10490, amountTone: 'in', sign: true }, { icon: 'point_of_sale', tone: 'nt', label: 'POS yatışı', sub: 'Gelir sayılmaz', amount: 7910 }] }, { label: 'Çıkan', items: [{ icon: 'storefront', tone: 'out', label: 'İşyeri kirası', amount: -6500, amountTone: 'out', sign: true }, { icon: 'inventory_2', tone: 'out', label: 'Ticari mal alımı', amount: -4200, amountTone: 'out', sign: true }, { icon: 'bolt', tone: 'out', label: 'Elektrik, su, doğalgaz', amount: -1860, amountTone: 'out', sign: true }] }];

// ---------- Ay seçici ----------
function MonthSheet({ selected = 9, today = 9, year0 = 2026, nowYear = 2026 }) {
  const [y, setY] = React.useState(year0);
  const [s, setS] = React.useState({ y: year0, m: selected });
  const atNow = s.y === nowYear && s.m === today;
  const arrow = (dir, off) => <button aria-label={dir < 0 ? 'Önceki yıl' : 'Sonraki yıl'} disabled={off} onClick={() => setY(y + dir)} style={{ width: 48, height: 48, border: 0, borderRadius: 16, background: 'transparent', display: 'flex', alignItems: 'center', justifyContent: 'center', color: off ? 'var(--border-strong)' : 'var(--ink)', cursor: off ? 'default' : 'pointer' }}><I6 n={dir < 0 ? 'chevron_left' : 'chevron_right'} s={24} /></button>;
  return (
    <Sheet6 footer={!atNow && <button onClick={() => { setY(nowYear); setS({ y: nowYear, m: today }); }} style={{ flex: 1, height: 48, borderRadius: 16, border: '1px solid var(--border-strong)', background: 'transparent', font: '600 15px/1 var(--font-core)', color: 'var(--ink)', cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8 }}><I6 n="today" s={18} />Bu aya dön · {AY6[today]} {nowYear}</button>}>
      <div style={{ display: 'flex', alignItems: 'center', padding: '0 12px 8px' }}>
        {arrow(-1, false)}
        <div style={{ flex: 1, textAlign: 'center', font: '700 22px/1 var(--font-core)', fontVariantNumeric: 'tabular-nums' }}>{y}</div>
        {arrow(1, y >= nowYear)}
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,minmax(0,1fr))', gap: 8, padding: '0 16px' }}>
        {AY6.map((m, i) => {
          const future = y > nowYear || (y === nowYear && i > today);
          const on = s.y === y && s.m === i;
          const now = y === nowYear && i === today;
          return (
            <button key={m} disabled={future} aria-pressed={on} onClick={() => setS({ y, m: i })} style={{ height: 56, borderRadius: 16, border: on ? '2px solid var(--brand)' : now ? '2px solid var(--border-strong)' : '1px solid var(--border)', background: on ? 'var(--brand)' : future ? 'transparent' : 'var(--surface-card)', color: on ? 'var(--on-brand)' : future ? 'var(--ink-faint)' : 'var(--ink)', borderStyle: future ? 'dashed' : 'solid', font: (on ? '700' : '500') + ' 15px/1.1 var(--font-core)', cursor: future ? 'default' : 'pointer', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 3 }}>
              {m}{now && <span style={{ font: '500 11px/1 var(--font-core)', letterSpacing: '.03em', color: on ? 'var(--on-brand)' : 'var(--ink-muted)' }}>bu ay</span>}
            </button>
          );
        })}
      </div>
      <div style={{ ...muted6, padding: '14px 24px 0', textWrap: 'pretty' }}>Aya dokunmak seçer ve paneli kapatır. Gelecek aylar boş olduğu için seçilemez.</div>
    </Sheet6>
  );
}

// ---------- B2: konumlu menü (yalnız karşılaştırma için) ----------
function AnchoredMenu() {
  const items = [['Hepsi', 26330, true], ['Dükkan Kasası', 9840], ['Ziraat Vadesiz', 12560], ['Bonus Kart', 3930]];
  return (
    <div style={{ position: 'absolute', top: 182, right: 16, width: 280, zIndex: 20, background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', padding: '8px 0' }}>
      {items.map(([l, v, on]) => (
        <div key={l} style={{ display: 'flex', alignItems: 'center', gap: 12, height: 48, padding: '0 16px', background: on ? 'var(--surface-card-muted)' : 'transparent' }}>
          <span style={{ flex: 1, font: (on ? '600' : '400') + ' 16px/1 var(--font-core)' }}>{l}</span><Money6 v={v} style={{ fontSize: 15 }} /><span style={{ width: 20, display: 'flex' }}>{on && <I6 n="check" />}</span>
        </div>
      ))}
    </div>
  );
}

Object.assign(window, { I6, Money6, Card6, Cap6, ChipRow, muted6, fmt6, KategoriOzeti, HesapDetay, MonthSheet, FilterSheet, AnchoredMenu, HESAP_GROUPS, KISI_GROUPS, KAT_GROUPS, Chip6, Header6 });
