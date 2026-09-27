// Kasa v2 — Gün sonu: tek sayım kartı (beklenen / sayılan / fark) + geçmiş.
// POS: yoldaki toplam + her tahsilatta brüt · komisyon · net ızgarası.
function KasaSayimV2() {
  const { AppCard, AppMoneyText, AppStatusChip, AppSubmitButton, AppSelectField } = DS;
  const cash = DATA.cash;
  const [accountId, setAccountId] = React.useState(cash.accounts[0].value);
  const [count, setCount] = React.useState(null);
  const [adjusted, setAdjusted] = React.useState(false);
  return (
    <div className="body" style={{ padding: '16px var(--space-md) var(--fab-clearance)' }}>
      <AppSelectField label="Kasa" value={accountId} options={cash.accounts} onChange={setAccountId} />
      <div style={{ height: 12 }} />
      <AppCard padding="var(--space-lg)">
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <span style={{ ...LBL, flex: 1 }}>Uygulamaya göre · Bugün {cash.todayLabel}</span>
          {count === null ? <AppStatusChip label="Sayılmadı" icon="schedule" tone="planned" /> : <AppStatusChip label={adjusted ? 'Fark kaydedildi' : 'Eksik'} icon={adjusted ? 'check_circle' : 'difference'} tone={adjusted ? 'income' : 'expense'} />}
        </div>
        <div style={{ marginTop: 6 }}><AppMoneyText amount={cash.expected} size="hero" /></div>
        {count !== null && (
          <div style={{ marginTop: 16, borderTop: '1px solid var(--border)' }}>
            {[['Elde sayılan', count.counted, null], ['Fark', count.difference, 'expense']].map(([l, a, e]) => (
              <div key={l} style={{ display: 'flex', alignItems: 'center', padding: '12px 0 0' }}>
                <span style={{ flex: 1, font: '15px/1.4 var(--font-core)', color: 'var(--ink)' }}>{l}</span>
                <AppMoneyText amount={a} effect={e || undefined} signed={!!e} size="row" />
              </div>
            ))}
          </div>
        )}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-sm)', marginTop: 20 }}>
          {count === null && <AppSubmitButton label="Sayımı gir" icon="calculate" fullWidth onSubmit={() => { setCount({ counted: '23100.0000', difference: '85.0000' }); setAdjusted(false); }} />}
          {count !== null && !adjusted && <AppSubmitButton label="Farkı kaydet" icon="playlist_add_check" fullWidth onSubmit={() => setAdjusted(true)} />}
          {count !== null && <AppSubmitButton label="Yeniden say" icon="calculate" variant="outlined" fullWidth onSubmit={() => { setCount(null); setAdjusted(false); }} />}
        </div>
        <p style={{ ...HELP, margin: '12px 0 0', textWrap: 'pretty' }}>
          {count === null ? 'Sayım bir gözlemdir; yazmak hiçbir bakiyeyi değiştirmez.' : adjusted ? 'Tek bir gider kaydı oluştu; kasa sayılan tutara oturdu.' : 'Fark kendiliğinden yazılmaz. Kaydederseniz tek bir gider kaydı oluşur.'}
        </p>
      </AppCard>
      <Section title="Geçmiş sayımlar">
        <AppCard padding={0}>
          <RowList inset={76}>
            {cash.history.map((h) => {
              const [d, m] = h.date.split(' ');
              return <Row key={h.date} lead={<DateLeaf day={d} month={m.slice(0, 3)} />} title={fmt(h.counted)} subtitle={h.note} />;
            })}
          </RowList>
        </AppCard>
      </Section>
    </div>
  );
}

function KasaPosV2() {
  const { AppCard, AppMoneyText, AppStatusChip, AppSegmentedButton, AppSubmitButton, AppEmptyView } = DS;
  const [mode, setMode] = React.useState('all');
  const transit = DATA.pos.filter((p) => p.state !== 'done');
  const items = mode === 'transit' ? transit : DATA.pos;
  const total = transit.reduce((s, p) => s + Number(p.net), 0).toFixed(4);
  return (
    <div className="body" style={{ padding: '16px var(--space-md) var(--fab-clearance)' }}>
      <AppCard padding="var(--space-lg)">
        <span style={LBL}>Yolda</span>
        <div style={{ marginTop: 6 }}><AppMoneyText amount={total} size="hero" /></div>
        <div style={{ ...HELP, marginTop: 2 }}>{transit.length === 0 ? 'Bekleyen tahsilat yok' : transit.length + ' tahsilat hesaba geçmedi · net varlığa dahil'}</div>
        <div style={{ marginTop: 20 }}><AppSubmitButton label="Tahsilat ekle" icon="add" variant="tonal" fullWidth onSubmit={() => {}} /></div>
      </AppCard>
      <div style={{ height: 20 }} />
      <AppSegmentedButton value={mode} onChange={setMode} options={[{ value: 'all', label: 'Hepsi' }, { value: 'transit', label: 'Yolda' }]} />
      <div style={{ height: 12 }} />
      {items.length === 0 ? <AppEmptyView icon="point_of_sale" title="Tahsilat yok." message="Kartla satış yaptığınızda buraya ekleyin." /> : (
        <div className="stack" style={{ gap: 'var(--space-sm)' }}>
          {items.map((p) => (
            <AppCard key={p.id}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
                  <span style={ROWT}>{p.title}</span>
                  <span style={HELP}>{p.date} · {p.state === 'done' ? 'Geçti ' + p.transferredOn : 'Beklenen ' + p.expected}</span>
                </span>
                {p.state === 'done' ? <AppStatusChip label="Hesaba geçti" icon="check_circle" tone="income" /> : <AppStatusChip label="Yolda" icon="schedule" tone="planned" />}
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0,1fr))', gap: 8, marginTop: 14, paddingTop: 12, borderTop: '1px solid var(--border)' }}>
                {[['Brüt', p.gross, null], ['Komisyon', p.commission, 'expense'], ['Net', p.net, null]].map(([l, a, e]) => (
                  <span key={l} style={{ display: 'flex', flexDirection: 'column', gap: 2, minWidth: 0 }}>
                    <span style={{ ...LBL, color: 'var(--ink-muted)' }}>{l}</span>
                    <AppMoneyText amount={a} effect={e || undefined} signed={!!e} size="body" style={{ fontWeight: l === 'Net' ? 700 : 500 }} />
                  </span>
                ))}
              </div>
            </AppCard>
          ))}
        </div>
      )}
    </div>
  );
}

function KasaV2({ initialTab = 0 }) {
  const [tab, setTab] = React.useState(initialTab);
  return (
    <div className="screen">
      <TopBar title="Kasa" actions={<ToolButton icon="history" label="Geçmiş" />} />
      <DS.AppTabBar tabs={['Gün sonu', 'POS tahsilatları']} value={tab} onChange={setTab} />
      {tab === 0 ? <KasaSayimV2 /> : <KasaPosV2 />}
    </div>
  );
}
Object.assign(window, { KasaV2 });
