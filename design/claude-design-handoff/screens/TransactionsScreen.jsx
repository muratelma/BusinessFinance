// İşlemler — kaynak: activity_feed_page.dart, activity_tile.dart.
// Filtre çipleri → planlanan özeti kartı → kenardan kenara beyaz liste
// (kart değil; satırlar ince çizgiyle ayrılır). Dokununca detay paneli.
const TONE_CAPSULE = {
  income: ['var(--income-container)', 'var(--on-income-container)'],
  expense: ['var(--expense-container)', 'var(--on-expense-container)'],
  neutral: ['var(--neutral-container)', 'var(--on-neutral-container)'],
};
const EFFECT_LABEL = { income: 'Gelir', expense: 'Gider', neutral: 'Transfer' };

function TransactionDetailSheet({ item, cancelled, onCancel, onClose }) {
  const { AppBottomSheet, AppMoneyText, AppKeyValueList, AppSubmitButton } = DS;
  const color = { income: 'var(--income)', expense: 'var(--expense)', neutral: 'var(--neutral)' }[item.effect];
  return (
    <AppBottomSheet title={item.title} subtitle="İşlem" onClose={onClose}>
      <div style={{ marginTop: 28 }}><AppMoneyText amount={item.amount} effect={item.effect} isCancelled={cancelled} size="hero" /></div>
      <div style={{ marginTop: 6, font: '16px/1.4 var(--font-core)', color: cancelled ? 'var(--cancelled)' : color }}>{cancelled ? 'İptal edildi' : EFFECT_LABEL[item.effect]}</div>
      <Gap h={20} /><Divider /><Gap h={20} />
      <AppKeyValueList rows={[
        ['Durum', cancelled ? 'İptal edildi' : 'Gerçekleşti'],
        ['İş tarihi', item.date],
        ['Kategori', item.category],
        ['Hesap', item.account],
        ['Köken', 'Elle eklendi'],
      ]} gap="12px" />
      <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start', gap: 10, marginTop: 28 }}>
        <AppSubmitButton label="Belgeler Veri Araçları ekranında" icon="attach_file" variant="outlined" disabled onSubmit={() => {}} />
        {!cancelled && <AppSubmitButton label="Hareketi iptal et" icon="block" variant="tonal" onSubmit={onCancel} />}
      </div>
    </AppBottomSheet>
  );
}

function TransactionsScreen({ onOpenPlanned }) {
  const { AppCard, AppListRow, AppMoneyText, AppFilterChips, AppConfirmDialog, AppEmptyView, Icon } = DS;
  const [chip, setChip] = React.useState('all');
  const [detail, setDetail] = React.useState(null);
  const [confirm, setConfirm] = React.useState(null);
  const [cancelled, setCancelled] = React.useState(() => new Set());
  const items = chip === 'all' ? DATA.activities : DATA.activities.filter((a) => a.kind === chip);

  return (
    <div className="screen">
      <AppBar title="İşlemler" actions={<IconButton icon="filter_alt" label="İşlemleri filtrele" size={30} />} />
      <div className="body" style={{ paddingBottom: 'var(--fab-clearance)' }}>
        <AppFilterChips value={chip} onChange={setChip} label="İşlem türü" style={{ padding: '4px var(--space-md) 0' }}
          options={[{ value: 'all', label: 'Tümü' }, { value: 'accounts', label: 'Hesaplar' }, { value: 'cards', label: 'Kredi Kartları' }, { value: 'transfers', label: 'Transferler' }, { value: 'debts', label: 'Borçlar' }]} />
        <div style={{ padding: '16px var(--space-md) 8px' }}>
          <AppCard padding={0} onClick={onOpenPlanned}>
            <AppListRow icon="schedule" iconColor="var(--ink)" title={DATA.plannedSummary.count + ' planlanan işlem'} subtitle={'En yakını ' + DATA.plannedSummary.nearest + ' • Bakiyeye dahil değil'}
              trailing={<Icon name="chevron_right" size={26} color="var(--ink)" />} style={{ padding: '14px var(--space-md)' }} />
          </AppCard>
        </div>
        <div style={{ background: 'var(--surface-card)' }}>
          {items.length === 0 ? (
            <AppEmptyView icon="receipt_long" title="Henüz işlem yok" message="İlk gelir veya giderinizi ekleyebilirsiniz." />
          ) : items.map((item, i) => {
            const isCancelled = cancelled.has(item.title);
            const [bg, fg] = isCancelled ? ['var(--cancelled-container)', 'var(--on-cancelled-container)'] : TONE_CAPSULE[item.effect];
            return (
              <React.Fragment key={item.title}>
                {i > 0 && <Divider />}
                <AppListRow icon={item.icon} iconBackground={bg} iconColor={fg} title={item.title} titleMaxLines={1} subtitle={item.subtitle} dimmed={isCancelled}
                  badge={isCancelled ? <DS.AppStatusChip label="İptal edildi" icon="block" tone="cancelled" /> : null}
                  trailing={<AppMoneyText amount={item.amount} effect={item.effect} isCancelled={isCancelled} signed size="row" />}
                  onClick={() => setDetail(item)} style={{ padding: '14px var(--space-md)', borderRadius: 0 }} />
              </React.Fragment>
            );
          })}
        </div>
      </div>
      {detail && !confirm && <TransactionDetailSheet item={detail} cancelled={cancelled.has(detail.title)} onCancel={() => setConfirm(detail)} onClose={() => setDetail(null)} />}
      {confirm && (
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-md)', zIndex: 30 }}>
          <AppConfirmDialog icon="block" destructive highlight={EFFECT_LABEL[confirm.effect] + ' · ' + fmt(confirm.amount)}
            message="Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık etkilemez." confirmLabel="Hareketi iptal et"
            onConfirm={() => { setCancelled(new Set([...cancelled, confirm.title])); setConfirm(null); }} onCancel={() => setConfirm(null)} />
        </div>
      )}
    </div>
  );
}

// Planlananlar — İşlemler'den (ve Özet'teki gecikme bandından) açılır; alt çubuk kalır.
function PlannedScreen({ onBack }) {
  const { AppCard, AppListRow, AppMoneyText, AppFilterChips, AppSegmentedButton, AppStatusChip, AppRowAction } = DS;
  const [horizon, setHorizon] = React.useState('30');
  const [kind, setKind] = React.useState('all');
  const [done, setDone] = React.useState(() => new Set());
  const items = DATA.planned.filter((p) => !done.has(p.title) && (kind === 'all' || p.kind === kind));
  return (
    <div className="screen">
      <AppBar title="Planlananlar" onBack={onBack} />
      <div className="body" style={{ paddingBottom: 'var(--fab-clearance)' }}>
        <div style={{ padding: '4px var(--space-md) 0' }}>
          <AppSegmentedButton value={horizon} onChange={setHorizon} maxWidth={300} options={[{ value: '7', label: '7 gün' }, { value: '30', label: '30 gün' }, { value: '90', label: '90 gün' }]} />
        </div>
        <AppFilterChips value={kind} onChange={setKind} label="Plan türü" style={{ padding: '18px var(--space-md) 0' }}
          options={[{ value: 'all', label: 'Tümü' }, { value: 'recurring', label: 'Tekrarlanan' }, { value: 'installment', label: 'Taksit' }, { value: 'statement', label: 'Ekstre' }, { value: 'debt', label: 'Borç' }]} />
        <div style={{ padding: '20px var(--space-md) 0' }}>
          <SectionNote>Planlananlar gerçekleşene kadar bakiyeye ve raporlara girmez.</SectionNote>
          <div className="stack" style={{ gap: 10 }}>
            {items.map((p) => (
              <AppCard key={p.title} padding={0}>
                <AppListRow icon="event_repeat" title={p.title} subtitle={p.meta}
                  badge={p.overdue ? <AppStatusChip label="Gecikmiş" icon="warning" tone="expense" /> : <AppStatusChip label="Planlandı" icon="schedule" tone="planned" />}
                  action={<AppRowAction label="Gerçekleştir" onClick={() => setDone(new Set([...done, p.title]))} />}
                  trailing={<AppMoneyText amount={p.amount} effect={p.effect} size="row" />} style={{ padding: 'var(--space-md)' }} />
              </AppCard>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
Object.assign(window, { TransactionsScreen, PlannedScreen, TransactionDetailSheet });
