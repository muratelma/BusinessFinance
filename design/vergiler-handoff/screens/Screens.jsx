// Ortak parçalar ve örnek veri — ekran görüntülerindeki değerlerle (Eylül 2026).
const DS = window.DesignSystem_02d8cd;
const fmt = (a) => DS.MoneyText.format(a, 'TRY');

function AppBar({ title, actions, onBack }) {
  return (
    <div className="appbar" style={onBack ? { paddingLeft: 4 } : undefined}>
      {onBack && <IconButton icon="arrow_back" label="Geri" onClick={onBack} />}
      <h1 style={onBack ? { marginLeft: 20 } : undefined}>{title}</h1>
      {actions}
    </div>
  );
}

function IconButton({ icon, label, onClick, badge, size = 26 }) {
  return (
    <button className="iconbtn" onClick={onClick} title={label} aria-label={label}>
      <DS.Icon name={icon} size={size} />
      {badge && <span style={{ position: 'absolute', top: 4, right: 0, width: 8, height: 8, borderRadius: '50%', background: 'var(--error)' }} />}
    </button>
  );
}

function SectionNote({ children }) {
  return <p style={{ margin: '0 0 var(--space-sm)', font: '14px/1.45 var(--font-core)', letterSpacing: '.2px', color: 'var(--ink-muted)', textWrap: 'pretty' }}>{children}</p>;
}

function HorizonBadge({ label }) {
  return <span style={{ padding: '6px 12px', borderRadius: 999, background: 'var(--brand-container)', font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>{label}</span>;
}

function CountLabel({ children }) {
  return <span style={{ font: '600 14px/1.3 var(--font-core)', letterSpacing: '.3px', color: 'var(--ink-faint)' }}>{children}</span>;
}

// Kart içindeki ikonlu satır: ikon · etiket (+ alt satır) · tutar.
function MetaRow({ icon, iconColor, label, subtitle, amount, effect, signed }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', minHeight: 32 }}>
      <DS.Icon name={icon} size={24} color={iconColor || 'var(--ink-muted)'} />
      <div style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column' }}>
        <span style={{ font: '16px/1.45 var(--font-core)', letterSpacing: '.2px', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{label}</span>
        {subtitle && <span style={{ font: '14px/1.45 var(--font-core)', letterSpacing: '.2px', color: 'var(--ink-muted)' }}>{subtitle}</span>}
      </div>
      {amount !== undefined && <DS.AppMoneyText amount={amount} effect={effect} signed={signed} size="row" />}
    </div>
  );
}

const Gap = ({ h = 16 }) => <div style={{ height: h, flex: '0 0 auto' }} />;
const Divider = ({ inset = 0, style }) => <div style={{ height: 1, background: 'var(--border)', marginLeft: inset, ...style }} />;

const DATA = {
  period: { year: 2026, month: 9 },
  businessNet: '6152.7900',
  personalNet: '-5119.0000',
  net: '1033.7900',
  netCaption: 'Geçen aya göre ₺11.176,54 daha iyi',
  income: '68550.0000',
  expense: '67516.2100',
  overdue: { count: 6, oldest: '10 Ağustos' },
  categoryCount: 14,
  categories: [
    { name: 'Ticari mal alımı', icon: 'sell', amount: '26507.2000', color: 'var(--category-1)' },
    { name: 'Personel ücretleri', icon: 'sell', amount: '22000.0000', color: 'var(--category-2)' },
    { name: 'SGK ve vergi ödemeleri', icon: 'account_balance', amount: '4120.0000', color: 'var(--category-3)' },
    { name: 'Elektrik, su, doğalgaz', icon: 'bolt', amount: '3950.8800', color: 'var(--category-4)' },
    { name: 'Diğer', icon: 'more_horiz', amount: '10938.1300', color: 'var(--category-other)', isOther: true },
  ],
  budgetsExceeded: [
    { name: 'Araç ve yakıt', icon: 'local_gas_station', remaining: '260.0000' },
    { name: 'Ticari mal alımı', icon: 'sell', remaining: '1507.2000' },
  ],
  budgetsOk: 2,
  upcomingTotal: '14450.0000',
  upcoming: [
    { title: 'Muhtasar beyanı', due: '26 Eylül', kind: 'Tekrarlanan', amount: '2800.0000' },
    { title: 'KDV beyanı', due: '28 Eylül', kind: 'Tekrarlanan', amount: '6250.0000' },
    { title: 'SGK / Bağkur primi', due: '30 Eylül', kind: 'Tekrarlanan', amount: '5400.0000' },
  ],
  netWorth: { total: '55012.0400', liquid: '62041.8700', cardDebt: '16868.0000', transit: '22253.6300', receivable: '13300.0000', debt: '25715.4600', assets: '97595.5000', liabilities: '42583.4600' },
  accounts: [
    { name: 'Birikim Hesabi', typeLabel: 'Banka hesabı', icon: 'account_balance', balance: '65000.0000' },
    { name: 'Dukkan Kasasi', typeLabel: 'Nakit', icon: 'payments', balance: '23185.0000' },
    { name: 'Sahsi Cuzdan', typeLabel: 'Nakit', icon: 'payments', balance: '-2290.0000' },
    { name: 'Ziraat Vadesiz', typeLabel: 'Banka hesabı', icon: 'account_balance', balance: '-1853.1300' },
  ],
  emailVerified: false,
  plannedSummary: { count: 21, nearest: '10 Ağustos' },
  activities: [
    { title: 'Gun sonu sayimi', subtitle: 'Diğer işletme gideri • Dukkan Kasasi • 24 Eylül', icon: 'north_east', effect: 'expense', amount: '85.0000', kind: 'accounts', date: '2026-09-24', category: 'Diğer işletme gideri', account: 'Dukkan Kasasi' },
    { title: 'Kart ekstresi kismi odeme', subtitle: 'Ziraat Vadesiz → Ticari Kart • 23 Eylül', icon: 'payments', effect: 'neutral', amount: '5000.0000', kind: 'cards', date: '2026-09-23', category: 'Kart ödemesi', account: 'Ziraat Vadesiz' },
    { title: 'Isyeri elektrik faturasi', subtitle: 'Elektrik, su, doğalgaz • 23 Eylül', icon: 'event_note', effect: 'expense', amount: '3950.8800', kind: 'accounts', date: '2026-09-23', category: 'Elektrik, su, doğalgaz', account: 'Ziraat Vadesiz' },
    { title: 'Gun sonu POS - yolda', subtitle: 'Satış geliri • Ziraat Vadesiz • 23 Eylül', icon: 'point_of_sale', effect: 'income', amount: '22650.0000', kind: 'accounts', date: '2026-09-23', category: 'Satış geliri', account: 'Ziraat Vadesiz' },
    { title: 'Banka ve POS komisyonu', subtitle: 'Ziraat Vadesiz • 23 Eylül', icon: 'percent', effect: 'expense', amount: '396.3800', kind: 'accounts', date: '2026-09-23', category: 'Banka ücreti', account: 'Ziraat Vadesiz' },
    { title: 'Dis hekimi', subtitle: 'Sağlık • Sahsi Cuzdan • 22 Eylül', icon: 'north_east', effect: 'expense', amount: '1250.0000', kind: 'accounts', date: '2026-09-22', category: 'Sağlık', account: 'Sahsi Cuzdan' },
    { title: 'Fazla odeme yapildi', subtitle: 'Dukkan Kasasi → Mahalle Kirtasiye • 22 Eylül', icon: 'price_check', effect: 'neutral', amount: '3300.0000', kind: 'transfers', date: '2026-09-22', category: 'Cari ödeme', account: 'Dukkan Kasasi' },
    { title: 'Toner ve zimba', subtitle: 'Kırtasiye • Ticari Kart • 21 Eylül', icon: 'north_east', effect: 'expense', amount: '640.0000', kind: 'cards', date: '2026-09-21', category: 'Kırtasiye', account: 'Ticari Kart' },
  ],
  planned: [
    { title: 'Muhasebeci ucreti', meta: 'Tekrarlanan • 10 Ağustos • Ziraat Vadesiz', kind: 'recurring', effect: 'expense', amount: '3500.0000', overdue: true },
    { title: 'Dijital abonelik', meta: 'Tekrarlanan • 20 Ağustos • Sahsi Kart', kind: 'recurring', effect: 'expense', amount: '429.0000', overdue: true },
    { title: 'Daire kirasi tahsilati', meta: 'Tekrarlanan • 1 Eylül • Ziraat Vadesiz', kind: 'recurring', effect: 'income', amount: '9000.0000', overdue: true },
    { title: 'Isyeri kirasi', meta: 'Tekrarlanan • 5 Eylül • Ziraat Vadesiz', kind: 'recurring', effect: 'expense', amount: '15000.0000', overdue: true },
    { title: 'Vitrin dolabi 4. taksit', meta: 'Taksit • 15 Ekim • Ticari Kart', kind: 'installment', effect: 'expense', amount: '4000.0000' },
  ],
  cash: {
    accounts: [{ value: 'dukkan', label: 'Dukkan Kasasi' }, { value: 'cuzdan', label: 'Sahsi Cuzdan' }],
    expected: '23185.0000',
    todayLabel: '25 Eylül',
    history: [{ date: '24 Eylül', note: 'Farkı kaydedildi', counted: '23185.0000' }],
  },
  pos: [
    { id: 'p1', title: 'Gun sonu POS - yolda', date: '23 Eylül', commission: '396.3800', net: '22253.6300', expected: '27 Eylül', gross: '22650.0000', state: 'transit' },
    { id: 'p2', title: 'Gun sonu POS', date: '9 Eylül', commission: '223.1300', net: '12526.8700', transferredOn: '11 Eylül', gross: '12750.0000', state: 'done' },
  ],
};

Object.assign(window, { DS, fmt, AppBar, IconButton, SectionNote, HorizonBadge, CountLabel, MetaRow, Gap, Divider, DATA });
