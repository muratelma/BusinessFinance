// Hesaplar ve Transferler — kaynak: accounts_and_transfers_page.dart, accounts_page.dart, account_form_page.dart
const ACCOUNTS_DATA = {
  accounts: [
    { name: 'Nakit kasa', typeLabel: 'Nakit', icon: 'payments', balance: '8420.0000' },
    { name: 'Ziraat işletme', typeLabel: 'Banka hesabı', icon: 'account_balance', balance: '52310.7500' },
    { name: 'Enpara şahsi', typeLabel: 'Banka hesabı', icon: 'account_balance', balance: '6120.0000' },
  ],
  transfers: [
    { from: 'Nakit kasa', to: 'Ziraat işletme', amount: '8000.0000', date: '2026-08-14' },
    { from: 'Ziraat işletme', to: 'Bankkart', amount: '3000.0000', date: '2026-08-13' },
  ],
};

function AccountForm({ onClose }) {
  const { AppFormSheet, AppFormField, AppTextField, AppSelectField, AppScopeDefaultField } = DS;
  const [name, setName] = React.useState('');
  const [type, setType] = React.useState('bank');
  const [opening, setOpening] = React.useState('');
  const [scope, setScope] = React.useState(null);
  return (
    <div className="sheet-scrim" onClick={onClose}>
      <div onClick={(e) => e.stopPropagation()} style={{ width: '100%', maxHeight: '90%', overflowY: 'auto' }}>
        <AppFormSheet title="Hesap ekle" submitLabel="Kaydet" onSubmit={onClose} cancelLabel="Vazgeç" onCancel={onClose}>
          <AppFormField><AppTextField label="Hesap adı" value={name} onChange={setName} /></AppFormField>
          <AppFormField><AppSelectField label="Tür" value={type} options={[{ value: 'cash', label: 'Nakit' }, { value: 'bank', label: 'Banka hesabı' }]} onChange={setType} /></AppFormField>
          <AppFormField><AppTextField label="Açılış bakiyesi" value={opening} onChange={setOpening} suffix="TRY" /></AppFormField>
          <AppFormField><AppScopeDefaultField value={scope} onChange={setScope} helperText="Bu hesaptaki hareketler varsayılan olarak bu kapsamda açılır." /></AppFormField>
        </AppFormSheet>
      </div>
    </div>
  );
}

function AccountsScreen({ onBack }) {
  const { AppCard, AppListRow, AppMoneyText, AppSegmentedButton, AppEmptyView, AppSectionHeader } = DS;
  const [period, setPeriod] = React.useState('3m');
  const [form, setForm] = React.useState(false);
  const [detail, setDetail] = React.useState(null);
  return (
    <div className="screen">
      <AppBar title="Hesaplar ve transferler" onBack={onBack} actions={<IconButton icon="add" label="Hesap ekle" onClick={() => setForm(true)} />} />
      <div className="body" style={{ padding: 'var(--space-md) var(--space-md) var(--fab-clearance)' }}>
        <AppSectionHeader title="Hesaplar" />
        <AppCard padding="var(--space-sm) 0">
          {DATA.accounts.map((a, i) => (
            <React.Fragment key={a.name}>
              {i > 0 && <div className="divider" style={{ margin: '0 var(--space-md)' }} />}
              <AppListRow icon={a.icon} title={a.name} subtitle={a.typeLabel} trailing={<AppMoneyText amount={a.balance} size="row" />} onClick={() => setDetail(a)} />
            </React.Fragment>
          ))}
        </AppCard>
        <div style={{ height: 'var(--space-lg)' }} />
        <AppSectionHeader title="Transferler" />
        <AppSegmentedButton value={period} options={[{ value: '3m', label: 'Son 3 ay' }, { value: '6m', label: 'Son 6 ay' }, { value: 'y', label: 'Bu yıl' }, { value: 'all', label: 'Tümü' }]} onChange={setPeriod} />
        <div style={{ height: 'var(--space-sm)' }} />
        {ACCOUNTS_DATA.transfers.length === 0 ? (
          <AppEmptyView icon="swap_horiz" title="Henüz transfer yok" message="Hesaplar arası ilk transferinizi ekleyebilirsiniz." />
        ) : (
          <div className="stack" style={{ gap: 'var(--space-sm)' }}>
            {ACCOUNTS_DATA.transfers.map((t) => (
              <AppCard key={t.date + t.from} padding={0}>
                <AppListRow icon="swap_horiz" title={t.from + ' → ' + t.to} subtitle={DS.DateText.dayMonth(t.date)} trailing={<AppMoneyText amount={t.amount} size="row" />} />
              </AppCard>
            ))}
          </div>
        )}
      </div>
      {form && <AccountForm onClose={() => setForm(false)} />}
      {detail && (
        <div className="sheet-scrim" onClick={() => setDetail(null)}>
          <div onClick={(e) => e.stopPropagation()} style={{ width: '100%', background: 'var(--surface-card)', borderRadius: 'var(--radius-sheet) var(--radius-sheet) 0 0', padding: 'var(--space-md)' }}>
            <div style={{ font: '700 20px/1.3 var(--font-core)', color: 'var(--ink)' }}>{detail.name}</div>
            <div style={{ margin: 'var(--space-sm) 0' }}><AppMoneyText amount={detail.balance} size="metric" /></div>
            <div className="divider" />
            <div style={{ padding: 'var(--space-sm) 0', font: '13px/1.45 var(--font-core)', color: 'var(--ink-muted)' }}>Bu hesabın hareket listesi burada görünür.</div>
          </div>
        </div>
      )}
    </div>
  );
}
Object.assign(window, { AccountsScreen });
