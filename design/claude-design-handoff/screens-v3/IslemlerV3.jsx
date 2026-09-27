// İşlemler v3 — iki yeni öneri. (1. öneri: screens-v2/IslemlerV2.jsx)
const TR_SHORT = ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'];
const signedSum = (items, eff) => items.filter((i) => i.effect === eff).reduce((s, i) => s + Number(i.amount), 0);
const groupByDay = (items) => {
  const g = [];
  items.forEach((it) => { const l = g[g.length - 1]; if (l && l.date === it.date) l.items.push(it); else g.push({ date: it.date, items: [it] }); });
  return g;
};
const subNoDate = (s) => s.replace(/ • \d+ \S+$/, '');

function SearchField({ placeholder = 'İşlem, kategori veya hesap ara' }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', minHeight: 48, padding: '0 var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)', boxSizing: 'border-box' }}>
      <DS.Icon name="search" size={20} color="var(--ink-muted)" />
      <span style={{ flex: 1, font: 'var(--text-body-size)/1.4 var(--font-core)', color: 'var(--ink-faint)' }}>{placeholder}</span>
    </div>
  );
}

function useTxnState() {
  const [detail, setDetail] = React.useState(null);
  const [confirm, setConfirm] = React.useState(null);
  const [cancelled, setCancelled] = React.useState(() => new Set());
  const overlays = (
    <>
      {detail && !confirm && <TransactionDetailSheet item={detail} cancelled={cancelled.has(detail.title)} onCancel={() => setConfirm(detail)} onClose={() => setDetail(null)} />}
      {confirm && (
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-md)', zIndex: 30 }}>
          <DS.AppConfirmDialog icon="block" destructive highlight={V2_EFFECT_LABEL[confirm.effect] + ' · ' + fmt(confirm.amount)}
            message="Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık etkilemez." confirmLabel="Hareketi iptal et"
            onConfirm={() => { setCancelled(new Set([...cancelled, confirm.title])); setConfirm(null); setDetail(null); }} onCancel={() => setConfirm(null)} />
        </div>
      )}
    </>
  );
  return { setDetail, cancelled, overlays };
}

function TxnRow({ item, cancelled, onClick }) {
  const sub = subNoDate(item.subtitle);
  return (
    <div style={{ opacity: cancelled ? 0.55 : 1 }}>
      <Row onClick={onClick} lead={<Capsule icon={item.icon} tone={cancelled ? 'cancelled' : item.effect} />} title={item.title}
        subtitle={cancelled ? 'İptal edildi · ' + sub : sub}
        trailing={<DS.AppMoneyText amount={item.amount} effect={item.effect} isCancelled={cancelled} signed size="row" />} />
    </div>
  );
}

function PlannedStrip({ onOpen }) {
  return (
    <button onClick={onOpen} style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: 48, padding: '0 8px 0 var(--space-md)', border: 'none', borderRadius: 'var(--radius-field)', background: 'var(--planned-container)', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box' }}>
      <DS.Icon name="event_repeat" size={20} color="var(--on-planned-container)" />
      <span style={{ flex: 1, font: '600 14px/1.3 var(--font-core)', color: 'var(--on-planned-container)' }}>{DATA.plannedSummary.count} planlanan işlem</span>
      <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--on-planned-container)' }}>Bakiyeye dahil değil</span>
      <DS.Icon name="chevron_right" size={22} color="var(--on-planned-container)" />
    </button>
  );
}

const TXN_FILTERS = [{ value: 'all', label: 'Tümü' }, { value: 'accounts', label: 'Hesaplar' }, { value: 'cards', label: 'Kredi kartları' }, { value: 'transfers', label: 'Transferler' }, { value: 'debts', label: 'Borçlar' }];

// Öneri 2 — Akış: arama alanı görünür, liste kenardan kenara, gün başlıkları yapışkan.
function IslemlerAkis({ onOpenPlanned }) {
  const { AppFilterChips, AppMoneyText, AppEmptyView } = DS;
  const [chip, setChip] = React.useState('all');
  const { setDetail, cancelled, overlays } = useTxnState();
  const items = chip === 'all' ? DATA.activities : DATA.activities.filter((a) => a.kind === chip);
  return (
    <div className="screen">
      <TopBar title="İşlemler" actions={<ToolButton icon="tune" label="İşlemleri filtrele" />} />
      <div style={{ padding: '4px var(--space-md) 0', flex: '0 0 auto' }}><SearchField /></div>
      <AppFilterChips value={chip} onChange={setChip} label="İşlem türü" style={{ padding: '12px var(--space-md) 8px', flex: '0 0 auto' }} options={TXN_FILTERS} />
      <div className="body" style={{ paddingBottom: 'var(--fab-clearance)' }}>
        <div style={{ padding: '4px var(--space-md) 8px' }}><PlannedStrip onOpen={onOpenPlanned} /></div>
        {items.length === 0 ? <AppEmptyView icon="receipt_long" title="Henüz işlem yok" message="İlk gelir veya giderinizi ekleyebilirsiniz." /> : groupByDay(items).map((g) => {
          const [pre, base] = dayLabel(g.date);
          const live = g.items.filter((i) => !cancelled.has(i.title));
          const inc = signedSum(live, 'income'), exp = signedSum(live, 'expense');
          return (
            <div key={g.date}>
              <div style={{ position: 'sticky', top: 0, zIndex: 1, display: 'flex', alignItems: 'baseline', gap: 8, padding: '20px var(--space-md) 8px', background: 'var(--surface-canvas)' }}>
                <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{base}</span>
                <span style={HELP}>{pre}</span>
              </div>
              <div style={{ background: 'var(--surface-card)', borderTop: '1px solid var(--border)', borderBottom: '1px solid var(--border)' }}>
                <RowList inset={72}>{g.items.map((it) => <TxnRow key={it.title} item={it} cancelled={cancelled.has(it.title)} onClick={() => setDetail(it)} />)}</RowList>
              </div>
            </div>
          );
        })}
        <div style={{ ...HELP, textAlign: 'center', padding: '20px 0 0' }}>Toplam {items.length} hareket</div>
      </div>
      {overlays}
    </div>
  );
}

// Öneri 3 — Hafta şeridi: üstte günler, seçilen günün gelir/gider/neti, altında o günün hareketleri.
function IslemlerHafta({ onOpenPlanned }) {
  const { AppCard, AppMoneyText, AppSegmentedButton, AppEmptyView } = DS;
  const [day, setDay] = React.useState(null);
  const [eff, setEff] = React.useState('all');
  const { setDetail, cancelled, overlays } = useTxnState();
  const week = Array.from({ length: 7 }, (_, i) => { const d = new Date('2026-09-21T12:00:00'); d.setDate(21 + i); return d; });
  const iso = (d) => '2026-09-' + String(d.getDate()).padStart(2, '0');
  const byDay = day ? DATA.activities.filter((a) => a.date === day) : DATA.activities;
  const items = eff === 'all' ? byDay : byDay.filter((a) => a.effect === eff);
  const live = byDay.filter((i) => !cancelled.has(i.title));
  const inc = signedSum(live, 'income'), exp = signedSum(live, 'expense'), net = inc - exp;
  return (
    <div className="screen">
      <TopBar title="İşlemler" actions={<><ToolButton icon="search" label="İşlem ara" /><ToolButton icon="tune" label="İşlemleri filtrele" /></>} />
      <div style={{ padding: '0 var(--space-md)', flex: '0 0 auto' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginRight: -12 }}>
          <span style={{ font: '600 16px/1.3 var(--font-core)', color: 'var(--ink)' }}>21 – 27 Eylül</span>
          <span style={{ display: 'flex' }}><ToolButton icon="chevron_left" label="Önceki hafta" /><ToolButton icon="chevron_right" label="Sonraki hafta" /></span>
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(7, minmax(0,1fr))', gap: 4 }}>
          {week.map((d) => {
            const k = iso(d); const sel = day === k; const future = k > TODAY_ISO; const today = k === TODAY_ISO;
            const has = DATA.activities.filter((a) => a.date === k);
            return (
              <button key={k} disabled={future} onClick={() => setDay(sel ? null : k)} aria-pressed={sel}
                style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4, minHeight: 72, padding: '8px 0', border: today && !sel ? '1px solid var(--border-strong)' : '1px solid transparent', borderRadius: 'var(--radius-field)', background: sel ? 'var(--brand)' : 'transparent', cursor: future ? 'default' : 'pointer', opacity: future ? 0.4 : 1 }}>
                <span style={{ font: '500 12px/1.2 var(--font-core)', color: sel ? 'var(--on-brand)' : 'var(--ink-muted)' }}>{TR_SHORT[d.getDay()]}</span>
                <span style={{ font: '700 18px/1.1 var(--font-core)', fontFeatureSettings: '"tnum" 1', color: sel ? 'var(--on-brand)' : 'var(--ink)' }}>{d.getDate()}</span>
                <span style={{ display: 'flex', gap: 3, height: 6 }}>
                  {has.some((a) => a.effect === 'income') && <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'var(--income-fill)' }} />}
                  {has.some((a) => a.effect === 'expense') && <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'var(--expense-fill)' }} />}
                </span>
              </button>
            );
          })}
        </div>
      </div>
      <div className="body" style={{ padding: '12px var(--space-md) var(--fab-clearance)' }}>
        <AppCard padding="var(--space-md)">
          <div style={{ font: '500 14px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>{day ? dayLabel(day).reverse().join(', ') : 'Bu hafta'}</div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1px 1fr 1px 1fr', gap: 12, marginTop: 12 }}>
            {[['Gelir', inc, 'income'], null, ['Gider', exp, 'expense'], null, ['Net', net, null]].map((c, i) => c === null
              ? <span key={i} style={{ background: 'var(--border)' }} />
              : (
                <span key={c[0]} style={{ display: 'flex', flexDirection: 'column', gap: 2, minWidth: 0 }}>
                  <span style={HELP}>{c[0]}</span>
                  <AppMoneyText amount={(c[2] ? c[1] : c[1]).toFixed(4)} effect={c[2] || undefined} signed={!!c[2]} size="body" style={{ fontWeight: 700, fontSize: 'var(--text-body-large-size)' }} />
                </span>
              ))}
          </div>
        </AppCard>
        <div style={{ margin: '12px 0' }}><PlannedStrip onOpen={onOpenPlanned} /></div>
        <AppSegmentedButton value={eff} onChange={setEff} options={[{ value: 'all', label: 'Hepsi' }, { value: 'income', label: 'Gelir' }, { value: 'expense', label: 'Gider' }, { value: 'neutral', label: 'Transfer' }]} />
        {items.length === 0 ? <AppEmptyView icon="event_busy" title="Bu günde hareket yok" message="Başka bir gün seçebilir veya yeni işlem ekleyebilirsiniz." /> : groupByDay(items).map((g) => {
          const [pre, base] = dayLabel(g.date);
          return (
            <div key={g.date} style={{ marginTop: 'var(--space-md)' }}>
              {!day && <div style={{ display: 'flex', alignItems: 'baseline', gap: 8, padding: '0 4px 8px' }}><span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>{base}</span><span style={HELP}>{pre}</span></div>}
              <AppCard padding={0}><RowList inset={72}>{g.items.map((it) => <TxnRow key={it.title} item={it} cancelled={cancelled.has(it.title)} onClick={() => setDetail(it)} />)}</RowList></AppCard>
            </div>
          );
        })}
      </div>
      {overlays}
    </div>
  );
}
Object.assign(window, { IslemlerAkis, IslemlerHafta });
