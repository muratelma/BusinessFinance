// Özet v3 — sıra eski yapıdaki gibi: gecikme → net → kategori → bütçe →
// yaklaşanlar → varlık → bakiyeler. Ekranı olan bölümler dokununca açılır.
function NetWorthCard() {
  const { AppCard, AppMoneyText } = DS; const nw = DATA.netWorth;
  const block = (title, sum, rows, fill) => (
    <div style={{ marginTop: 'var(--space-md)' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, padding: '4px 0' }}>
        <span style={{ width: 10, height: 10, borderRadius: '50%', background: fill }} />
        <span style={{ ...ROWT, flex: 1 }}>{title}</span>
        <AppMoneyText amount={sum} size="row" />
      </div>
      {rows.map(([l, s, a, ic]) => <Row key={l} pad="8px 0" lead={<Capsule icon={ic} />} title={l} subtitle={s} trailing={<AppMoneyText amount={a} size="body" style={{ fontSize: 'var(--text-body-large-size)' }} />} />)}
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
      {block('Varlıklar', nw.assets, [['Likit varlık', 'Kasa ve banka', nw.liquid, 'savings'], ['Yolda', "POS tahsilatı · 27 Eylül'de geçer", nw.transit, 'schedule'], ['Alacak', 'faiz hariç', nw.receivable, 'handshake']], 'var(--neutral-fill)')}
      <div style={{ height: 1, background: 'var(--border)', marginTop: 8 }} />
      {block('Borçlar', nw.liabilities, [['Kart borcu', 'Dönem ekstresi dahil', nw.cardDebt, 'credit_card'], ['Borç', 'faiz hariç', nw.debt, 'account_balance']], 'var(--expense-fill)')}
    </AppCard>
  );
}

function OzetV3({ page = 1, onNavigate = () => {} }) {
  const { AppCard, AppMoneyText, Icon } = DS;
  const [scope, setScope] = React.useState(null);
  const Hero = { 1: Hero1, 2: Hero2, 3: Hero3 }[page];
  const Cat = { 1: Cat1, 2: Cat2, 3: Cat3 }[page];
  const Budget = { 1: Budget1, 2: Budget2, 3: Budget3 }[page];
  const Up = { 1: Up1, 2: Up2, 3: Up3 }[page];
  const openBudgets = () => onNavigate('budgets');
  const openPlanned = () => onNavigate('planned');
  const overCount = BUDGETS_V3.filter((b) => b.over).length;
  return (
    <div className="screen">
      <TopBar title="Özet" actions={<AvatarButton badge={!DATA.emailVerified} onClick={() => onNavigate('account')} />} />
      <div style={{ padding: '4px var(--space-md) 12px', flex: '0 0 auto' }}><ScopeTabs value={scope} onChange={setScope} /></div>
      <div className="body" style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <MonthStepper />
        {DATA.overdue.count > 0 && (
          <button onClick={openPlanned} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', width: '100%', margin: '8px 0 var(--space-md)', padding: '0 8px 0 var(--space-md)', minHeight: 52, border: 'none', borderRadius: 'var(--radius-card)', background: 'var(--expense-container)', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box', whiteSpace: 'nowrap' }}>
            <Icon name="warning" size={20} color="var(--on-expense-container)" />
            <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--on-expense-container)' }}>{DATA.overdue.count} gecikmiş ödeme</span>
            <span style={{ flex: 1 }} />
            <span style={{ font: '14px/1.3 var(--font-core)', color: 'var(--on-expense-container)' }}>En eskisi {DATA.overdue.oldest}</span>
            <Icon name="chevron_right" size={22} color="var(--on-expense-container)" />
          </button>
        )}
        <Hero scope={scope} />
        <Block title="Kategori giderleri" trailing={<CountLabel>{DATA.categoryCount} kategori</CountLabel>}><Cat /></Block>
        <Block title="Bütçeler" onOpen={openBudgets} trailing={page === 1 ? null : <CountLabel>{overCount} aşıldı</CountLabel>}><Budget onOpen={openBudgets} /></Block>
        <Block title="Yaklaşanlar" onOpen={openPlanned} trailing={<CountLabel>7 gün</CountLabel>}><Up onOpen={openPlanned} /></Block>
        <Block title="Varlık durumu">
          {scope !== null && <SectionNote>Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.</SectionNote>}
          <NetWorthCard />
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

// Bütçeler ekranı — Özet'teki bütçe bölümünden açılır; her bütçe tam ayrıntıyla.
function BudgetsV3({ onBack }) {
  const { AppCard, AppMoneyText, AppStatusChip, AppShareBar, AppSubmitButton } = DS;
  const sorted = [...BUDGETS_V3].sort((a, b) => Number(b.spent) / Number(b.limit) - Number(a.spent) / Number(a.limit));
  return (
    <div className="screen">
      <TopBar title="Bütçeler" onBack={onBack} actions={<ToolButton icon="add" label="Bütçe ekle" />} />
      <div className="body" style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <MonthStepper />
        <div className="stack" style={{ gap: 'var(--space-sm)', marginTop: 8 }}>
          {sorted.map((b) => {
            const r = Number(b.spent) / Number(b.limit);
            return (
              <AppCard key={b.name}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <Capsule icon={b.icon} tone={b.over ? 'expense' : undefined} />
                  <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
                    <span style={ROWT}>{b.name}</span><span style={HELP}>{b.scope}</span>
                  </span>
                  {b.over ? <AppStatusChip label="Aşıldı" icon="warning" tone="expense" /> : <AppStatusChip label="Limit içinde" icon="check_circle" tone="planned" />}
                </div>
                <div style={{ display: 'flex', alignItems: 'baseline', gap: 6, marginTop: 'var(--space-md)' }}>
                  <AppMoneyText amount={b.spent} size="metric" /><span style={HELP}>/ {fmt(b.limit)}</span>
                </div>
                <AppShareBar ratio={Math.min(1, r)} color={b.over ? 'var(--expense-fill)' : 'var(--neutral-fill)'} height={8} style={{ marginTop: 8 }} />
                <div style={{ display: 'flex', marginTop: 8, font: '600 13px/1.3 var(--font-core)' }}>
                  <span style={{ flex: 1, color: b.over ? 'var(--expense)' : 'var(--ink-muted)' }}>%{Math.round(r * 100)}</span>
                  <span style={{ color: b.over ? 'var(--expense)' : 'var(--ink-muted)' }}>{fmt(b.diff)} {b.over ? 'fazla' : 'kaldı'}</span>
                </div>
              </AppCard>
            );
          })}
        </div>
        <div style={{ marginTop: 'var(--space-md)' }}><AppSubmitButton label="Geçen ayın bütçelerini kopyala" icon="content_copy" variant="outlined" fullWidth onSubmit={() => {}} /></div>
      </div>
    </div>
  );
}
Object.assign(window, { OzetV3, BudgetsV3, NetWorthCard });
