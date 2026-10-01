// v2 ortak parçalar. Veri ve DS, v1 Screens.jsx'ten gelir.
const LBL = { font: 'var(--text-label-weight) var(--text-label-size)/var(--text-label-line) var(--font-core)', letterSpacing: 'var(--text-label-tracking)', color: 'var(--ink-faint)' };
const HELP = { font: 'var(--text-helper-size)/var(--text-helper-line) var(--font-core)', color: 'var(--ink-muted)' };
const ROWT = { font: 'var(--text-row-weight) var(--text-row-size)/var(--text-row-line) var(--font-core)', color: 'var(--ink)' };

// Başlık: sağdaki eylemler ikon gliflerinin kenarı 16 px'e hizalanacak şekilde.
function TopBar({ title, actions, onBack }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 0, minHeight: 64, padding: onBack ? '8px 4px 4px 4px' : '8px 4px 4px 16px', flex: '0 0 auto' }}>
      {onBack && <ToolButton icon="arrow_back" label="Geri" onClick={onBack} />}
      <h1 style={{ flex: 1, margin: 0, marginLeft: onBack ? 4 : 0, font: '700 26px/1.25 var(--font-core)', letterSpacing: '-0.6px', color: 'var(--ink)' }}>{title}</h1>
      {actions}
    </div>
  );
}

function ToolButton({ icon, label, onClick }) {
  return (
    <button className="iconbtn" onClick={onClick} title={label} aria-label={label}>
      <DS.Icon name={icon} size={24} />
    </button>
  );
}

function AvatarButton({ onClick, badge }) {
  return (
    <button className="iconbtn" onClick={onClick} title="Hesabım" aria-label="Hesabım" style={{ marginRight: 4 }}>
      <span style={{ width: 32, height: 32, borderRadius: '50%', background: 'var(--brand)', color: 'var(--on-brand)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', font: '600 13px/1 var(--font-core)', letterSpacing: '.3px' }}>ME</span>
      {badge && <span style={{ position: 'absolute', top: 8, right: 6, width: 10, height: 10, borderRadius: '50%', background: 'var(--error)', border: '2px solid var(--surface-canvas)' }} />}
    </button>
  );
}

// Kapsam: tek parça ray, seçili dilim beyaz yüzey. Gölge yok.
function ScopeTabs({ value, onChange }) {
  const opts = [{ v: null, l: 'Hepsi' }, { v: 'business', l: 'İşletme' }, { v: 'personal', l: 'Şahsi' }];
  return (
    <div role="group" aria-label="Kapsam filtresi" style={{ display: 'flex', gap: 4, padding: 4, borderRadius: 'var(--radius-chip)', background: 'var(--surface-card-muted)', border: '1px solid var(--border)' }}>
      {opts.map((o) => {
        const on = value === o.v;
        return (
          <button key={o.l} type="button" aria-pressed={on} onClick={() => onChange(o.v)}
            style={{ flex: 1, minHeight: 36, border: on ? '1px solid var(--border-strong)' : '1px solid transparent', borderRadius: 'var(--radius-chip)', background: on ? 'var(--surface-card)' : 'transparent', color: on ? 'var(--ink)' : 'var(--ink-muted)', font: '600 14px/1.2 var(--font-core)', cursor: 'pointer' }}>
            {o.l}
          </button>
        );
      })}
    </div>
  );
}

function Section({ title, trailing, children, first }) {
  return (
    <div style={{ marginTop: first ? 0 : 'var(--space-lg)' }}>
      <DS.AppSectionHeader title={title} trailing={trailing} />
      {children}
    </div>
  );
}

// Kart içi satır listesi: ayırıcı yazının başladığı yerden.
function RowList({ children, inset = 16 }) {
  const items = React.Children.toArray(children).filter(Boolean);
  return items.map((c, i) => <React.Fragment key={i}>{i > 0 && <div style={{ height: 1, background: 'var(--border)', marginLeft: inset }} />}{c}</React.Fragment>);
}

function Capsule({ icon, tone, size = 40 }) {
  const map = { income: ['var(--income-container)', 'var(--on-income-container)'], expense: ['var(--expense-container)', 'var(--on-expense-container)'], neutral: ['var(--neutral-container)', 'var(--on-neutral-container)'], cancelled: ['var(--cancelled-container)', 'var(--on-cancelled-container)'] };
  const [bg, fg] = map[tone] || ['var(--surface-card-muted)', 'var(--ink)'];
  return <span style={{ width: size, height: size, flex: '0 0 auto', borderRadius: '50%', background: bg, display: 'inline-flex', alignItems: 'center', justifyContent: 'center' }}><DS.Icon name={icon} size={size >= 40 ? 20 : 18} color={fg} /></span>;
}

// Takvim yaprağı: gün + ay kısaltması.
function DateLeaf({ day, month, tone }) {
  const red = tone === 'expense';
  return (
    <span style={{ width: 44, height: 48, flex: '0 0 auto', borderRadius: 'var(--radius-field)', border: '1px solid ' + (red ? 'var(--expense-container)' : 'var(--border)'), background: red ? 'var(--expense-container)' : 'var(--surface-card)', display: 'inline-flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: 1 }}>
      <span style={{ font: '700 17px/1 var(--font-core)', fontFeatureSettings: '"tnum" 1', color: red ? 'var(--on-expense-container)' : 'var(--ink)' }}>{day}</span>
      <span style={{ font: '600 11px/1 var(--font-core)', letterSpacing: '.4px', color: red ? 'var(--on-expense-container)' : 'var(--ink-muted)' }}>{month}</span>
    </span>
  );
}

function Row({ lead, title, subtitle, trailing, onClick, pad = '14px 16px' }) {
  const Tag = onClick ? 'button' : 'div';
  return (
    <Tag onClick={onClick} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', width: '100%', padding: pad, minHeight: 56, border: 'none', background: 'transparent', textAlign: 'left', cursor: onClick ? 'pointer' : 'default', boxSizing: 'border-box', font: 'inherit' }}>
      {lead}
      <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <span style={{ ...ROWT, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{title}</span>
        {subtitle && <span style={{ ...HELP, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>{subtitle}</span>}
      </span>
      {trailing}
    </Tag>
  );
}

const V2_PERIOD_LABEL = 'Eylül 2026';

function MonthStepper({ label = V2_PERIOD_LABEL }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', margin: '0 -12px 0 0' }}>
      <span style={{ font: 'var(--text-section-weight) var(--text-section-size)/var(--text-section-line) var(--font-core)', color: 'var(--ink)' }}>{label}</span>
      <span style={{ display: 'flex' }}>
        <ToolButton icon="chevron_left" label="Önceki ay" />
        <ToolButton icon="chevron_right" label="Sonraki ay" />
      </span>
    </div>
  );
}

// Durum etiketi — üç biçim denenir (TagStyle bağlamı): 
// text: ikon + metin, dolgu yok · outline: ince kenarlı kapsül, beyaz zemin · dot: renkli nokta + metin.
const TAG_TONE = { income: ['var(--income)', 'var(--income-fill)'], expense: ['var(--expense)', 'var(--expense-fill)'], neutral: ['var(--neutral)', 'var(--neutral-fill)'], planned: ['var(--planned)', 'var(--border-strong)'], cancelled: ['var(--cancelled)', 'var(--border-strong)'] };
const TagStyle = React.createContext('text');
function StatusTag({ label, icon, tone = 'planned' }) {
  const kind = React.useContext(TagStyle);
  const [col, line] = TAG_TONE[tone] || TAG_TONE.planned;
  const txt = { font: '600 13px/1.2 var(--font-core)', color: col, whiteSpace: 'nowrap' };
  if (kind === 'outline') return <span style={{ ...txt, display: 'inline-flex', alignItems: 'center', gap: 4, flex: '0 0 auto', height: 28, padding: icon ? '0 10px 0 8px' : '0 10px', borderRadius: 'var(--radius-chip)', border: '1px solid ' + line, background: 'var(--surface-card)' }}>{icon && <DS.Icon name={icon} size={16} color={col} />}{label}</span>;
  if (kind === 'dot') return <span style={{ ...txt, display: 'inline-flex', alignItems: 'center', gap: 6, flex: '0 0 auto' }}><span aria-hidden="true" style={{ width: 8, height: 8, borderRadius: '50%', background: line === 'var(--border-strong)' ? 'var(--planned)' : line }}></span>{label}</span>;
  return <span style={{ ...txt, display: 'inline-flex', alignItems: 'center', gap: 4, flex: '0 0 auto' }}>{icon && <DS.Icon name={icon} size={16} color={col} />}{label}</span>;
}
// Kart başlığı şeridi: solda bağlam (tarih), sağda durum; altında ince çizgi.
function CardHead({ title, meta, status }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', minHeight: 52, padding: '0 var(--space-md)', borderBottom: '1px solid var(--border)' }}>
      <span style={{ flex: 1, minWidth: 0, display: 'flex', alignItems: 'baseline', gap: 6, overflow: 'hidden', whiteSpace: 'nowrap' }}>
        <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{title}</span>
        {meta && <span style={{ font: '14px/1.3 var(--font-core)', color: 'var(--ink-muted)', overflow: 'hidden', textOverflow: 'ellipsis' }}>{meta}</span>}
      </span>
      {status}
    </div>
  );
}
// Başlık yanındaki eylem: dolgusuz metin düğmesi, dokunma alanı 48 px.
function TextAction({ label, icon, trailingIcon, onClick }) {
  return <button type="button" onClick={onClick} style={{ display: 'inline-flex', alignItems: 'center', gap: 4, minHeight: 48, padding: '0 4px', margin: '-12px -4px', border: 'none', background: 'transparent', color: 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer' }}>{icon && <DS.Icon name={icon} size={18} />}{label}{trailingIcon && <DS.Icon name={trailingIcon} size={18} />}</button>;
}

Object.assign(window, { StatusTag, TagStyle, CardHead, TextAction, LBL, HELP, ROWT, TopBar, ToolButton, AvatarButton, ScopeTabs, Section, RowList, Capsule, DateLeaf, Row, MonthStepper });
