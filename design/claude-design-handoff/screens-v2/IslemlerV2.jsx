// İşlemler v2 — ay özeti şeridi, filtre çipleri, gün gün gruplanmış liste.
// Gün başlığı günün net hareketini (transferler hariç) sağda gösterir.
const TR_MONTHS = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
const TR_DAYS = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
const TODAY_ISO = '2026-09-25';
const V2_EFFECT_LABEL = { income: 'Gelir', expense: 'Gider', neutral: 'Transfer' };

function dayLabel(iso) {
  const d = new Date(iso + 'T12:00:00');
  const base = d.getDate() + ' ' + TR_MONTHS[d.getMonth()];
  const diff = Math.round((new Date(TODAY_ISO + 'T12:00:00') - d) / 86400000);
  const prefix = diff === 0 ? 'Bugün' : diff === 1 ? 'Dün' : TR_DAYS[d.getDay()];
  return [prefix, base];
}

function IslemlerV2({ onOpenPlanned }) {
  const { AppCard, AppMoneyText, AppFilterChips, AppConfirmDialog, AppEmptyView, AppStatusChip, Icon } = DS;
  const [chip, setChip] = React.useState('all');
  const [detail, setDetail] = React.useState(null);
  const [confirm, setConfirm] = React.useState(null);
  const [cancelled, setCancelled] = React.useState(() => new Set());
  const items = chip === 'all' ? DATA.activities : DATA.activities.filter((a) => a.kind === chip);
  const groups = [];
  items.forEach((it) => { const g = groups[groups.length - 1]; if (g && g.date === it.date) g.items.push(it); else groups.push({ date: it.date, items: [it] }); });

  return (
    <div className="screen">
      <TopBar title="İşlemler" actions={<><ToolButton icon="search" label="İşlem ara" /><ToolButton icon="tune" label="İşlemleri filtrele" /></>} />
      <div className="body" style={{ paddingBottom: 'var(--fab-clearance)' }}>
        <div style={{ padding: '4px var(--space-md) 0' }}>
          <AppCard padding="12px 16px">
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <span style={{ ...ROWT, flex: 1 }}>Eylül</span>
              <span style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 2 }}>
                <span style={{ ...LBL, color: 'var(--ink-muted)' }}>Gelir</span>
                <AppMoneyText amount={DATA.income} effect="income" signed size="body" style={{ fontWeight: 600 }} />
              </span>
              <span style={{ width: 1, alignSelf: 'stretch', background: 'var(--border)' }} />
              <span style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 2 }}>
                <span style={{ ...LBL, color: 'var(--ink-muted)' }}>Gider</span>
                <AppMoneyText amount={DATA.expense} effect="expense" signed size="body" style={{ fontWeight: 600 }} />
              </span>
            </div>
          </AppCard>
        </div>
        <AppFilterChips value={chip} onChange={setChip} label="İşlem türü" style={{ padding: '12px var(--space-md) 0' }}
          options={[{ value: 'all', label: 'Tümü' }, { value: 'accounts', label: 'Hesaplar' }, { value: 'cards', label: 'Kredi kartları' }, { value: 'transfers', label: 'Transferler' }, { value: 'debts', label: 'Borçlar' }]} />
        <div style={{ padding: '12px var(--space-md) 0' }}>
          <button onClick={onOpenPlanned} style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: 48, padding: '8px 8px 8px 12px', border: '1px dashed var(--border-strong)', borderRadius: 'var(--radius-field)', background: 'transparent', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box' }}>
            <Icon name="schedule" size={20} color="var(--ink-muted)" />
            <span style={{ flex: 1, font: '14px/1.4 var(--font-core)', color: 'var(--ink)' }}><b style={{ fontWeight: 600 }}>{DATA.plannedSummary.count} planlanan işlem</b> <span style={{ color: 'var(--ink-muted)' }}>· bakiyeye dahil değil</span></span>
            <Icon name="chevron_right" size={22} color="var(--ink-muted)" />
          </button>
        </div>

        {items.length === 0 ? (
          <AppEmptyView icon="receipt_long" title="Henüz işlem yok" message="İlk gelir veya giderinizi ekleyebilirsiniz." />
        ) : groups.map((g) => {
          const [pre, base] = dayLabel(g.date);
          const net = g.items.filter((i) => !cancelled.has(i.title)).reduce((s, i) => s + (i.effect === 'income' ? 1 : i.effect === 'expense' ? -1 : 0) * Number(i.amount), 0);
          return (
            <div key={g.date} style={{ padding: '0 var(--space-md)' }}>
              <div style={{ position: 'sticky', top: 0, zIndex: 1, background: 'var(--surface-canvas)', display: 'flex', alignItems: 'baseline', gap: 8, padding: '20px 4px 8px' }}>
                <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{base}</span>
                <span style={HELP}>{pre}</span>
                <span style={{ flex: 1 }} />
              </div>
              <AppCard padding={0}>
                <RowList inset={72}>
                  {g.items.map((item) => {
                    const isC = cancelled.has(item.title);
                    const sub = item.subtitle.replace(/ • \d+ \S+$/, '');
                    return (
                      <div key={item.title} style={{ opacity: isC ? 0.55 : 1 }}>
                        <Row onClick={() => setDetail(item)} lead={<Capsule icon={item.icon} tone={isC ? 'cancelled' : item.effect} />} title={item.title}
                          subtitle={isC ? 'İptal edildi · ' + sub : sub}
                          trailing={<AppMoneyText amount={item.amount} effect={item.effect} isCancelled={isC} signed size="row" />} />
                      </div>
                    );
                  })}
                </RowList>
              </AppCard>
            </div>
          );
        })}
        <div style={{ ...HELP, textAlign: 'center', padding: '20px 0 0' }}>Toplam {items.length} hareket</div>
      </div>
      {detail && !confirm && <TransactionDetailSheet item={detail} cancelled={cancelled.has(detail.title)} onCancel={() => setConfirm(detail)} onClose={() => setDetail(null)} />}
      {confirm && (
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-md)', zIndex: 30 }}>
          <AppConfirmDialog icon="block" destructive highlight={V2_EFFECT_LABEL[confirm.effect] + ' · ' + fmt(confirm.amount)}
            message="Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık etkilemez." confirmLabel="Hareketi iptal et"
            onConfirm={() => { setCancelled(new Set([...cancelled, confirm.title])); setConfirm(null); setDetail(null); }} onCancel={() => setConfirm(null)} />
        </div>
      )}
    </div>
  );
}
Object.assign(window, { IslemlerV2 });
