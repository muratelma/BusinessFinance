// v5 ortak parçalar — Son Tasarim'ın dili (Common.jsx) üstüne; yeni token yok.
const MUTED_BOX = { padding: '4px var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)' };

// Tam ekran alt sayfa: geri oklu başlık, kayan gövde, isteğe bağlı sabit alt eylem şeridi.
function Page({ title, onBack = () => {}, actions, footer, scroll = 0, children }) {
  const ref = React.useRef(null);
  React.useEffect(() => { if (ref.current && scroll) ref.current.scrollTop = scroll; }, [scroll]);
  return (
    <div className="screen">
      <TopBar title={title} onBack={onBack} actions={actions} />
      <div ref={ref} className="body" style={{ padding: '4px var(--space-md) var(--space-xl)' }}>{children}</div>
      {footer && <div style={{ flex: '0 0 auto', padding: '12px var(--space-md) var(--space-md)', borderTop: '1px solid var(--border)', background: 'var(--surface-canvas)' }}>{footer}</div>}
    </div>
  );
}

// Panel içi etiket–değer bloğu.
function DetailBlock({ rows, style }) {
  return (
    <div style={{ ...MUTED_BOX, ...style }}>
      <RowList inset={0}>
        {rows.map(([l, v, link]) => {
          const Tag = link ? 'button' : 'div';
          return (
            <Tag key={l} type={link ? 'button' : undefined} onClick={link ? () => {} : undefined} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', minHeight: 48, flexWrap: 'wrap', padding: '4px 0', width: '100%', border: 'none', background: 'transparent', font: 'inherit', textAlign: 'left', cursor: link ? 'pointer' : 'default', boxSizing: 'border-box' }}>
              <span style={{ flex: '1 1 120px', ...HELP }}>{l}</span>
              <span style={{ flex: '0 1 auto', display: 'inline-flex', alignItems: 'center', gap: 2, textAlign: 'right', font: '500 14px/1.35 var(--font-core)', color: 'var(--ink)', fontWeight: link ? 600 : 500 }}>{v}{link && <DS.Icon name="chevron_right" size={18} color="var(--ink-muted)" />}</span>
            </Tag>
          );
        })}
      </RowList>
    </div>
  );
}

// Onay kutulu satır — Material onay kutusu glifi, 56 dp.
function CheckRow({ on, title, subtitle, trailing, onClick = () => {} }) {
  return (
    <button type="button" role="checkbox" aria-checked={on} onClick={onClick} style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: 56, padding: '10px 16px 10px 12px', border: 'none', background: 'transparent', textAlign: 'left', cursor: 'pointer', font: 'inherit', boxSizing: 'border-box' }}>
      <DS.Icon name={on ? 'check_box' : 'check_box_outline_blank'} size={24} color={on ? 'var(--brand)' : 'var(--ink-muted)'} />
      <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <span style={ROWT}>{title}</span>
        {subtitle && <span style={{ ...HELP, textWrap: 'pretty' }}>{subtitle}</span>}
      </span>
      {trailing}
    </button>
  );
}

// Büyük tutar alanı (Sayımı gir'deki dil); boşsa yer tutucu soluk.
function AmountInput({ label = 'Tutar', value = '', placeholder = '0' }) {
  const [v, setV] = React.useState(value);
  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 'var(--space-xs)' }}>
      <span style={{ ...LBL, whiteSpace: 'nowrap' }}>{label}</span>
      <label style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'center', gap: 4, minWidth: 220, padding: '4px var(--space-md) 8px', borderBottom: '2px solid var(--brand)', cursor: 'text' }}>
        <span style={{ font: '700 36px/1.1 var(--font-core)', color: v ? 'var(--ink)' : 'var(--ink-faint)' }}>₺</span>
        <input value={v} placeholder={placeholder} onChange={(e) => setV(e.target.value)} inputMode="decimal" aria-label={label} style={{ width: Math.max(2, (v || placeholder).length) + 0.6 + 'ch', border: 'none', outline: 'none', background: 'transparent', padding: 0, font: '700 36px/1.1 var(--font-core)', letterSpacing: '-0.8px', fontFeatureSettings: '"tnum" 1', color: 'var(--ink)' }} />
      </label>
    </div>
  );
}

const MONTHS_SHORT = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];
// Seçilen aylarda: 12 ay çipi, çoklu seçim, 6×2 ızgara (2.0× yazıda 4×3'e iner).
function MonthChips({ selected = [] }) {
  const [sel, setSel] = React.useState(selected);
  return (
    <div role="group" aria-label="Aylar" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(52px, 1fr))', gap: 'var(--space-xs)' }}>
      {MONTHS_SHORT.map((m, i) => {
        const on = sel.includes(i);
        return <button key={m} type="button" aria-pressed={on} onClick={() => setSel(on ? sel.filter((x) => x !== i) : [...sel, i])} style={{ minHeight: 48, border: '1px solid ' + (on ? 'var(--brand)' : 'var(--border-strong)'), borderRadius: 'var(--radius-chip)', background: on ? 'var(--brand)' : 'var(--surface-card)', color: on ? 'var(--on-brand)' : 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer' }}>{m}</button>;
      })}
    </div>
  );
}

// Alan grubu etiketi (form içi).
function FieldLabel({ children, hint }) {
  return <div style={{ display: 'flex', flexDirection: 'column', gap: 2, margin: 'var(--space-md) 0 var(--space-sm)' }}><span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>{children}</span>{hint && <span style={HELP}>{hint}</span>}</div>;
}

function Rule({ children, style }) {
  return <p style={{ ...HELP, margin: 'var(--space-sm) 0 0', textWrap: 'pretty', ...style }}>{children}</p>;
}

// Panel başlığı (ayrıntı panelleri): kapsül + başlık + kapat.
function SheetHead({ icon, tone, title, subtitle, onClose = () => {} }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: -20 }}>
      <Capsule icon={icon} tone={tone} />
      <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column' }}><span style={ROWT}>{title}</span>{subtitle && <span style={HELP}>{subtitle}</span>}</span>
      <button className="iconbtn" onClick={onClose} aria-label="Kapat" title="Kapat" style={{ marginRight: -12 }}><DS.Icon name="close" size={22} /></button>
    </div>
  );
}

function Scrim({ children }) {
  return <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-md)', zIndex: 30 }}>{children}</div>;
}

// Dikey eylem yığını
function Actions({ children, style }) {
  return <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-sm)', marginTop: 'var(--space-md)', ...style }}>{children}</div>;
}

// Eylem listesi: ikincil ve yıkıcı eylemler kart içinde satır olarak — ne olacağını alt satır yazar.
function ActionRow({ icon, title, subtitle, danger, disabled, chevron, onClick = () => {} }) {
  const c = danger ? 'var(--expense)' : disabled ? 'var(--ink-faint)' : 'var(--ink)';
  return (
    <button type="button" disabled={disabled} aria-disabled={disabled} onClick={disabled ? undefined : onClick} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', width: '100%', minHeight: 64, padding: '10px 16px', border: 'none', background: 'transparent', textAlign: 'left', cursor: disabled ? 'default' : 'pointer', font: 'inherit', boxSizing: 'border-box' }}>
      <DS.Icon name={icon} size={22} color={c} />
      <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <span style={{ ...ROWT, color: c }}>{title}</span>
        {subtitle && <span style={{ ...HELP, textWrap: 'pretty' }}>{subtitle}</span>}
      </span>
      {chevron && !disabled && <DS.Icon name="chevron_right" size={20} color="var(--ink-muted)" />}
      {disabled && <DS.Icon name="lock" size={18} color="var(--ink-muted)" />}
    </button>
  );
}
function ActionCard({ children, style }) {
  return <div style={{ marginTop: 'var(--space-md)', border: '1px solid var(--border)', borderRadius: 'var(--radius-card)', background: 'var(--surface-card)', overflow: 'hidden', ...style }}><RowList inset={54}>{children}</RowList></div>;
}

// Panel alt eylem çubuğu: ayırıcı çizgi, eşit genişlikte en fazla iki buton (ikincil solda, birincil sağda).
function SheetFooter({ children }) {
  return <div style={{ display: 'flex', gap: 'var(--space-sm)', margin: 'var(--space-lg) calc(-1 * var(--space-md)) 0', padding: 'var(--space-md) var(--space-md) 0', borderTop: '1px solid var(--border)' }}>{React.Children.map(children, (c) => c && <div style={{ flex: 1, minWidth: 0 }}>{c}</div>)}</div>;
}
const DANGER_OUTLINE = { color: 'var(--expense)', border: '1px solid var(--expense-fill)' };

const R5 = (id, el) => { const n = document.getElementById(id); if (n) ReactDOM.createRoot(n).render(el); };
Object.assign(window, { Page, DetailBlock, CheckRow, AmountInput, MonthChips, MONTHS_SHORT, FieldLabel, Rule, SheetHead, Scrim, Actions, ActionRow, ActionCard, SheetFooter, DANGER_OUTLINE, MUTED_BOX, R5 });
