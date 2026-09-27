// Diğer v2 — üstte hesap kartı, altında dört başlık altında gruplanmış kapılar.
function DigerV2({ onNavigate = () => {}, isBusiness = true }) {
  const { AppCard, AppStatusChip, Icon } = DS;
  const groups = [
    { label: 'Para ve hesaplar', items: [
      { icon: 'account_balance_wallet', title: 'Hesaplar ve transferler', to: 'accounts' },
      { icon: 'credit_card', title: 'Kredi kartlarım', to: 'cards' },
      { icon: 'handshake', title: 'Borç ve alacaklar', to: 'debts' },
      { icon: 'group', title: 'Cari hesap', to: 'counterparties' },
    ] },
    { label: 'Planlama', items: [
      { icon: 'donut_small', title: 'Bütçeler', to: 'budgets' },
      { icon: 'event_note', title: 'Yükümlülükler', to: 'obligations' },
      { icon: 'savings', title: 'Tasarruf hedefleri', to: 'goals' },
      { icon: 'event_repeat', title: 'Planlama ve raporlar', to: 'planning' },
    ] },
    isBusiness && { label: 'Vergi ve muhasebe', items: [
      { icon: 'event_available', title: 'Vergi takvimi', to: 'tax' },
      { icon: 'description', title: 'Muhasebeci paketi', to: 'tax' },
    ] },
    { label: 'Ayarlar', items: [
      { icon: 'category', title: 'Kategoriler', to: 'categories' },
      { icon: 'notifications_active', title: 'Hatırlatmalar', to: 'reminders' },
      { icon: 'folder', title: 'Veri ve yedek', to: 'data_tools' },
    ] },
  ].filter(Boolean);
  const chev = <Icon name="chevron_right" size={22} color="var(--ink-muted)" />;
  return (
    <div className="screen">
      <TopBar title="Diğer" />
      <div className="body" style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <AppCard padding={0} onClick={() => onNavigate('account')}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', padding: 16 }}>
            <span style={{ width: 48, height: 48, borderRadius: '50%', background: 'var(--brand)', color: 'var(--on-brand)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', font: '600 17px/1 var(--font-core)', flex: '0 0 auto' }}>ME</span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 4 }}>
              <span style={ROWT}>Hesabım</span>
              <span style={HELP}>esnaf@ornek.com</span>
              {!DATA.emailVerified && <span><AppStatusChip label="E-posta doğrulanmadı" icon="mail" tone="expense" /></span>}
            </span>
            {chev}
          </div>
        </AppCard>
        {groups.map((g) => (
          <div key={g.label} style={{ marginTop: 'var(--space-lg)' }}>
            <div style={{ ...LBL, padding: '0 4px 8px' }}>{g.label}</div>
            <AppCard padding={0}>
              <RowList inset={72}>
                {g.items.map((it) => <Row key={it.title} onClick={() => onNavigate(it.to)} lead={<Capsule icon={it.icon} />} title={it.title} trailing={chev} pad="8px 12px 8px 16px" />)}
              </RowList>
            </AppCard>
          </div>
        ))}
      </div>
    </div>
  );
}
Object.assign(window, { DigerV2 });
