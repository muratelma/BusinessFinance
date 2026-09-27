// Özet v4 — seçilenler birleşti: net (s3), kategori (s3), yaklaşanlar (s1).
// Bütçe için iki aday: büyük halkalar / aşılanlar listesi.
// Kapsama göre statik örnek veri. Gerçekte sunucu ?scope= ile hazır toplam döner; istemci hesaplamaz.
const OZ_BIZ_CATS = [
  { name: 'Ticari mal alımı', icon: 'sell', amount: '26507.2000', color: 'var(--category-1)' },
  { name: 'Personel ücretleri', icon: 'badge', amount: '22000.0000', color: 'var(--category-2)' },
  { name: 'SGK ve vergi ödemeleri', icon: 'account_balance', amount: '4120.0000', color: 'var(--category-3)' },
  { name: 'Elektrik, su, doğalgaz', icon: 'bolt', amount: '3950.8800', color: 'var(--category-4)' },
  { name: 'Diğer', icon: 'more_horiz', amount: '4169.1300', color: 'var(--category-other)', isOther: true },
];
const OZ_PER_CATS = [
  { name: 'Market ve gıda', icon: 'shopping_basket', amount: '2480.0000', color: 'var(--category-1)' },
  { name: 'Sağlık', icon: 'medical_services', amount: '1250.0000', color: 'var(--category-2)' },
  { name: 'Ulaşım', icon: 'directions_bus', amount: '1100.0000', color: 'var(--category-3)' },
  { name: 'Eğitim', icon: 'school', amount: '900.0000', color: 'var(--category-4)' },
  { name: 'Diğer', icon: 'more_horiz', amount: '1039.0000', color: 'var(--category-other)', isOther: true },
];
const OZ_PER_BUDGETS = [
  { name: 'Ulaşım', icon: 'directions_bus', scope: 'Şahsi', spent: '1100.0000', limit: '1000.0000', diff: '100.0000', over: true },
  { name: 'Market ve gıda', icon: 'shopping_basket', scope: 'Şahsi', spent: '2480.0000', limit: '3000.0000', diff: '520.0000' },
];
const OZ_PER_UP = [
  { title: 'Telefon faturası', due: '27 Eylül', kind: 'Tekrarlanan', amount: '450.0000' },
  { title: 'Ev kirası', due: '29 Eylül', kind: 'Tekrarlanan', amount: '8500.0000' },
];
const OZ_DAY = (u) => Number(u.due.split(' ')[0]);
const OZ_SCOPE = {
  all: { income: '68550.0000', expense: '67516.2100', cats: DATA.categories, catCount: 14, budgets: [...BUDGETS_V3, ...OZ_PER_BUDGETS], up: [...DATA.upcoming, ...OZ_PER_UP].sort((a, b) => OZ_DAY(a) - OZ_DAY(b)), upTotal: '23400.0000', overdue: DATA.overdue },
  business: { income: '66900.0000', expense: '60747.2100', cats: OZ_BIZ_CATS, catCount: 9, budgets: BUDGETS_V3, up: DATA.upcoming, upTotal: '14450.0000', overdue: { count: 4, oldest: '10 Ağustos' } },
  personal: { income: '1650.0000', expense: '6769.0000', cats: OZ_PER_CATS, catCount: 5, budgets: OZ_PER_BUDGETS, up: OZ_PER_UP, upTotal: '8950.0000', overdue: { count: 2, oldest: '5 Eylül' } },
};
const ozScope = (scope) => OZ_SCOPE[scope || 'all'];

function HeroV4({ scope }) {
  const { AppCard, AppMoneyText } = DS; const h = heroData(scope); const sd = ozScope(scope);
  const line = (l, a, e, ic, signed) => (
    <div key={l} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48 }}>
      <Capsule icon={ic} tone={e} size={32} />
      <span style={{ flex: 1, font: 'var(--text-body-large-size)/1.4 var(--font-core)', color: 'var(--ink)' }}>{l}</span>
      <AppMoneyText amount={a} effect={e} signed={signed} size="row" />
    </div>
  );
  return (
    <AppCard padding="var(--space-md) 20px 20px">
      {line('Gelir', sd.income, 'income', 'south_west', true)}
      {line('Gider', sd.expense, 'expense', 'north_east', true)}
      <div style={{ height: 2, background: 'var(--ink)', margin: '8px 0 var(--space-md)' }} />
      <div style={{ display: 'flex', alignItems: 'flex-end', gap: 12 }}>
        <span style={{ flex: 1, font: '600 16px/1.3 var(--font-core)', color: 'var(--ink)', paddingBottom: 6 }}>{h.label}</span>
        <AppMoneyText amount={h.amount} size="hero" />
      </div>
      <div style={{ display: 'flex', justifyContent: 'flex-end' }}><HeroCaption h={h} /></div>
      {h.parts && (
        <div style={{ marginTop: 'var(--space-md)', padding: '4px var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)' }}>
          {h.parts.map(([l, a], i) => (
            <div key={l} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48, borderTop: i ? '1px solid var(--border)' : 'none' }}>
              <DS.Icon name={i === 0 ? 'storefront' : 'person'} size={20} color="var(--ink-muted)" />
              <span style={{ flex: 1, font: 'var(--text-body-size)/1.4 var(--font-core)', color: 'var(--ink)' }}>{l}</span>
              <AppMoneyText amount={a} size="row" />
            </div>
          ))}
        </div>
      )}
    </AppCard>
  );
}

function CatV4({ cats = DATA.categories, total = DATA.expense }) {
  const t = cats.reduce((s, c) => s + Number(c.amount), 0); const top = cats[0];
  const slices = cats.map((c) => ({ label: c.name, value: Number(c.amount), color: c.color, formattedValue: fmt(c.amount) }));
  return (
    <DS.AppCard>
      <div style={{ display: 'flex', alignItems: 'center', gap: 20 }}>
        <DS.AppDonutChart slices={slices} size={104} thickness={16} showLegend={false} />
        <div style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
          <span style={{ font: '500 14px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>Bu ay toplam</span>
          <DS.AppMoneyText amount={total} effect="expense" size="metric" />
          <span style={{ display: 'flex', flexDirection: 'column', gap: 2, marginTop: 4, paddingTop: 6, borderTop: '1px solid var(--border)' }}>
            <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>En büyük pay</span>
            <span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{top.name} · %{Math.round(Number(top.amount) / t * 100)}</span>
          </span>
        </div>
      </div>
      <div style={{ marginTop: 'var(--space-lg)', display: 'flex', flexDirection: 'column', gap: 12 }}>
        {cats.map((c) => (
          <div key={c.name} style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <span style={{ width: 32, height: 32, borderRadius: '50%', background: 'var(--surface-card-muted)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', flex: '0 0 auto' }}><DS.Icon name={c.icon} size={18} color={c.color} /></span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 6 }}>
              <span style={{ display: 'flex', alignItems: 'baseline', gap: 8 }}>
                <span style={{ flex: 1, minWidth: 0, font: 'var(--text-body-size)/1.3 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{c.name}</span>
                <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--ink-muted)', fontFeatureSettings: '"tnum" 1' }}>%{Math.round(Number(c.amount) / t * 100)}</span>
                <DS.AppMoneyText amount={c.amount} size="body" style={{ fontWeight: 600, minWidth: 88, textAlign: 'right' }} />
              </span>
              <DS.AppShareBar ratio={Number(c.amount) / t} color={c.color} height={4} />
            </span>
          </div>
        ))}
      </div>
    </DS.AppCard>
  );
}

// Bütçe A — halkalar: 1 bütçe tek, 2–3 bütçe ikili, 4+ bütçe dörtlü (en dolu olanlar).
function BudgetRingsV4({ onOpen, items = BUDGETS_V3 }) {
  const n = items.length >= 4 ? 4 : items.length >= 2 ? 2 : items.length;
  const shown = [...items].sort((a, b) => Number(b.spent) / Number(b.limit) - Number(a.spent) / Number(a.limit)).slice(0, n);
  return (
    <DS.AppCard padding="var(--space-md)" onClick={onOpen}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(' + Math.min(n, 2) + ', minmax(0,1fr))', gap: 'var(--space-sm)' }}>
        {shown.map((b) => {
          const r = Number(b.spent) / Number(b.limit);
          const fill = b.over ? 'var(--expense-fill)' : 'var(--neutral-fill)';
          return (
            <div key={b.name} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 8, padding: 'var(--space-md) 8px', borderRadius: 'var(--radius-field)', background: b.over ? 'var(--expense-container)' : 'var(--surface-card-muted)', minWidth: 0 }}>
              <div style={{ position: 'relative' }}>
                <DS.AppDonutChart size={96} thickness={10} showLegend={false}
                  slices={[{ label: 'Harcanan', value: Math.min(1, r), color: fill }, { label: 'Kalan', value: Math.max(0, 1 - r), color: 'var(--surface-card)' }]} />
                <span style={{ position: 'absolute', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 3 }}>
                  <DS.Icon name={b.icon} size={16} color={b.over ? 'var(--on-expense-container)' : 'var(--ink-muted)'} />
                  <span style={{ font: '700 16px/1 var(--font-core)', fontFeatureSettings: '"tnum" 1', color: b.over ? 'var(--on-expense-container)' : 'var(--ink)' }}>%{Math.round(r * 100)}</span>
                </span>
              </div>
              <span style={{ font: '600 14px/1.3 var(--font-core)', color: b.over ? 'var(--on-expense-container)' : 'var(--ink)', textAlign: 'center', width: '100%', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{b.name}</span>
              <span style={{ font: '13px/1.3 var(--font-core)', color: b.over ? 'var(--on-expense-container)' : 'var(--ink-muted)', fontFeatureSettings: '"tnum" 1' }}>{fmt(b.diff)} {b.over ? 'fazla' : 'kaldı'}</span>
            </div>
          );
        })}
      </div>
    </DS.AppCard>
  );
}

// Bütçe B — yalnız aşılanlar; rozet yok, tek ok başlıkta.
function BudgetListV4({ onOpen }) {
  const over = BUDGETS_V3.filter((b) => b.over);
  return (
    <DS.AppCard padding={0} onClick={onOpen}>
      <RowList inset={0}>
        {over.map((b) => (
          <div key={b.name} style={{ padding: 'var(--space-md)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <Capsule icon={b.icon} tone="expense" />
              <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
                <span style={{ ...ROWT, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{b.name}</span>
                <span style={{ ...HELP, fontFeatureSettings: '"tnum" 1' }}>{fmt(b.spent)} / {fmt(b.limit)}</span>
              </span>
              <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--expense)', fontFeatureSettings: '"tnum" 1' }}>+{fmt(b.diff)}</span>
            </div>
            <DS.AppShareBar ratio={1} color="var(--expense-fill)" height={6} style={{ marginTop: 12, marginLeft: 52 }} />
          </div>
        ))}
      </RowList>
    </DS.AppCard>
  );
}

function NetWorthV4() {
  const { AppCard, AppMoneyText } = DS; const nw = DATA.netWorth;
  const block = (title, sum, rows, tone) => (
    <div style={{ marginTop: 'var(--space-md)' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, padding: '4px 0' }}>
        <span style={{ width: 10, height: 10, borderRadius: '50%', background: 'var(--' + tone + '-fill)' }} />
        <span style={{ ...ROWT, flex: 1 }}>{title}</span>
        <AppMoneyText amount={sum} size="row" />
      </div>
      {rows.map(([l, s, a, ic, when]) => (
        <Row key={l} pad="8px 0" lead={<Capsule icon={ic} tone={tone} />} title={l} subtitle={s}
          trailing={
            <span style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 2 }}>
              <AppMoneyText amount={a} size="body" style={{ fontSize: 'var(--text-body-large-size)', fontWeight: 600, color: 'var(--' + tone + ')' }} />
              {when && <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, font: '12px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}><DS.Icon name="event" size={14} color="var(--ink-muted)" />{when}</span>}
            </span>
          } />
      ))}
    </div>
  );
  return (
    <AppCard>
      <span style={{ font: '500 16px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>Net varlık</span>
      <div style={{ marginTop: 4 }}><AppMoneyText amount={nw.total} size="metric" /></div>
      <div style={{ display: 'flex', gap: 2, height: 10, borderRadius: 'var(--radius-chip)', overflow: 'hidden', marginTop: 'var(--space-md)' }} aria-hidden="true">
        <span style={{ flex: Number(nw.assets), background: 'var(--neutral-fill)' }} />
        <span style={{ flex: Number(nw.liabilities), background: 'var(--expense-fill)' }} />
      </div>
      {block('Varlıklar', nw.assets, [['Likit varlık', 'Kasa ve banka', nw.liquid, 'savings'], ['Yolda', 'POS tahsilatı', nw.transit, 'schedule', '27 Eylül'], ['Alacak', 'faiz hariç', nw.receivable, 'handshake']], 'neutral')}
      <div style={{ height: 1, background: 'var(--border)', marginTop: 8 }} />
      {block('Borçlar', nw.liabilities, [['Kart borcu', 'Dönem ekstresi dahil', nw.cardDebt, 'credit_card'], ['Borç', 'faiz hariç', nw.debt, 'account_balance']], 'expense')}
    </AppCard>
  );
}

function OzetV4({ budget = 'rings', budgetCount = 4, onNavigate = () => {}, initialScope = null, initialScroll = 0 }) {
  const { AppCard, AppMoneyText, Icon } = DS;
  const [scope, setScope] = React.useState(initialScope);
  const bodyRef = React.useRef(null);
  React.useEffect(() => { const t = setTimeout(() => { if (bodyRef.current) bodyRef.current.scrollTop = initialScroll; }, 300); return () => clearTimeout(t); }, []);
  const openBudgets = () => onNavigate('budgets');
  const openPlanned = () => onNavigate('planned');
  const sd = ozScope(scope);
  const bItems = budgetCount >= 4 ? sd.budgets : sd.budgets.slice(0, budgetCount);
  const over = sd.budgets.filter((b) => b.over).length;
  return (
    <div className="screen">
      <TopBar title="Özet" actions={<AvatarButton badge={!DATA.emailVerified} onClick={() => onNavigate('account')} />} />
      <div style={{ padding: '4px var(--space-md) 12px', flex: '0 0 auto' }}><ScopeTabs value={scope} onChange={setScope} /></div>
      <div className="body" ref={bodyRef} style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <MonthStepper />
        {sd.overdue.count > 0 && (
          <button onClick={openPlanned} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', width: '100%', margin: '8px 0 var(--space-md)', padding: '0 8px 0 var(--space-md)', minHeight: 52, border: 'none', borderRadius: 'var(--radius-card)', background: 'var(--expense-container)', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box', whiteSpace: 'nowrap' }}>
            <Icon name="warning" size={20} color="var(--on-expense-container)" />
            <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--on-expense-container)' }}>{sd.overdue.count} gecikmiş ödeme</span>
            <span style={{ flex: 1 }} />
            <span style={{ font: '14px/1.3 var(--font-core)', color: 'var(--on-expense-container)' }}>En eskisi {sd.overdue.oldest}</span>
            <Icon name="chevron_right" size={22} color="var(--on-expense-container)" />
          </button>
        )}
        <HeroV4 scope={scope} />
        <Block title="Kategori giderleri" trailing={<CountLabel>{sd.catCount} kategori</CountLabel>}><CatV4 cats={sd.cats} total={sd.expense} /></Block>
        {budget === 'rings'
          ? <Block title="Bütçeler" onOpen={openBudgets} trailing={<CountLabel>{bItems.length} bütçe</CountLabel>}><BudgetRingsV4 onOpen={openBudgets} items={bItems} /></Block>
          : <Block title="Bütçeler" onOpen={openBudgets} trailing={<CountLabel>{over} / {sd.budgets.length} aşıldı</CountLabel>}><BudgetListV4 onOpen={openBudgets} /></Block>}
        <Block title="Yaklaşanlar" onOpen={openPlanned} trailing={<CountLabel>7 gün</CountLabel>}><Up1 onOpen={openPlanned} items={sd.up} total={sd.upTotal} /></Block>
        <Block title="Varlık durumu">
          {scope !== null && <SectionNote>Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.</SectionNote>}
          <NetWorthV4 />
        </Block>
        <Block title="Hesap bakiyeleri" onOpen={() => onNavigate('accounts')}>
          <AppCard padding={0}>
            <RowList inset={72}>
              {DATA.accounts.map((a) => <Row key={a.name} onClick={() => onNavigate('accounts')} lead={<Capsule icon={a.icon} />} title={a.name} subtitle={a.typeLabel} trailing={<AppMoneyText amount={a.balance} size="row" />} />)}
            </RowList>
          </AppCard>
        </Block>
      </div>
    </div>
  );
}
Object.assign(window, { OzetV4 });
