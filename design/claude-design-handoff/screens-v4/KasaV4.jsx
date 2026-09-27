// Kasa v4 — sekme yok, tek akış: bugünün sayımı → yoldaki POS → son sayımlar.
// Beklenen tutar nereden geldiğini gösterir; sayım panelde, fark canlı hesaplanır.
const KASA_ACCOUNTS = [
  { value: 'dukkan', label: 'Dükkan kasası', expected: '23185.0000', opening: '21400.0000', inflow: '2450.0000', outflow: '665.0000' },
  { value: 'cuzdan', label: 'Şahsi cüzdan', expected: '1840.0000', opening: '2100.0000', inflow: '0.0000', outflow: '260.0000' },
];
const KASA_HISTORY = [
  { day: '24', month: 'Eyl', counted: '21400.0000', expected: '21520.0000', diff: '-120.0000', saved: true },
  { day: '23', month: 'Eyl', counted: '19870.0000', expected: '19870.0000', diff: '0' },
  { day: '22', month: 'Eyl', counted: '18225.0000', expected: '18225.0000', diff: '0' },
  { day: '20', month: 'Eyl', counted: '16950.0000', expected: '16910.0000', diff: '40.0000', saved: true },
];

// Kasa seçici: iki dilimli ray; her dilim adını ve bakiyesini birlikte söyler.
function KasaPicker({ value, onChange }) {
  return (
    <div role="group" aria-label="Kasa" style={{ display: 'grid', gridTemplateColumns: 'repeat(' + KASA_ACCOUNTS.length + ', minmax(0,1fr))', gap: 'var(--space-xs)', padding: 'var(--space-xs)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)', border: '1px solid var(--border)' }}>
      {KASA_ACCOUNTS.map((o) => {
        const on = o.value === value;
        return (
          <button key={o.value} type="button" aria-pressed={on} onClick={() => onChange(o.value)} style={{ display: 'flex', alignItems: 'center', gap: 10, minWidth: 0, minHeight: 56, padding: '0 12px', border: on ? '1px solid var(--border-strong)' : '1px solid transparent', borderRadius: 12, background: on ? 'var(--surface-card)' : 'transparent', cursor: 'pointer', textAlign: 'left', font: 'inherit' }}>
            <DS.Icon name={o.value === 'dukkan' ? 'storefront' : 'wallet'} size={20} color={on ? 'var(--ink)' : 'var(--ink-muted)'} />
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
              <span style={{ font: '600 14px/1.25 var(--font-core)', color: on ? 'var(--ink)' : 'var(--ink-muted)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{o.label}</span>
              <DS.AppMoneyText amount={o.expected} size="body" style={{ fontSize: 13, fontWeight: 500, color: 'var(--ink-muted)' }} />
            </span>
          </button>
        );
      })}
    </div>
  );
}

// Son sayım satırı: sol sayılan + beklenen, sağ fark. Çip yok; metin kısa kalır.
function KasaHistoryRow({ h }) {
  const n = Number(h.diff);
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', padding: '12px 16px', minHeight: 64 }}>
      <DateLeaf day={h.day} month={h.month} />
      <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <DS.AppMoneyText amount={h.counted} size="row" />
        <span style={{ ...HELP, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>Beklenen {fmt(h.expected)}</span>
      </span>
      <span style={{ flex: '0 0 auto', display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 2 }}>
        {n === 0 ? <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, font: '600 14px/1.3 var(--font-core)', color: 'var(--income)' }}><DS.Icon name="check_circle" size={16} color="var(--income)" />Tuttu</span>
          : <DS.AppMoneyText amount={Math.abs(n).toFixed(4)} effect={n < 0 ? 'expense' : 'income'} signed size="body" style={{ fontWeight: 600 }} />}
        {n !== 0 && <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>{(n < 0 ? 'Eksik' : 'Fazla') + (h.saved ? ' · kaydedildi' : '')}</span>}
      </span>
    </div>
  );
}

function DiffChip({ diff, saved }) {
  const n = Number(diff);
  if (n === 0) return <StatusTag label="Tuttu" icon="check_circle" tone="income" />;
  if (saved) return <StatusTag label="Fark kaydedildi" icon="check_circle" tone="neutral" />;
  return <StatusTag label={n < 0 ? 'Eksik' : 'Fazla'} icon="error" tone={n < 0 ? 'expense' : 'neutral'} />;
}

const KASA_NOTES = [
  { v: 200, n: 100 }, { v: 100, n: 25 }, { v: 50, n: 8 }, { v: 20, n: 7 }, { v: 10, n: 4 }, { v: 5, n: 2 },
];

function NoteStepper({ value, onChange, label }) {
  const b = { width: 44, height: 44, display: 'inline-flex', alignItems: 'center', justifyContent: 'center', border: '1px solid var(--border)', borderRadius: '50%', background: 'var(--surface-card)', color: 'var(--ink)', cursor: 'pointer' };
  return (
    <span style={{ display: 'inline-flex', alignItems: 'center', gap: 'var(--space-xs)' }}>
      <button type="button" style={b} aria-label={label + ' azalt'} onClick={() => onChange(Math.max(0, value - 1))}><DS.Icon name="remove" size={20} /></button>
      <span style={{ width: 40, textAlign: 'center', font: '600 16px/1 var(--font-core)', fontFeatureSettings: '"tnum" 1', color: value ? 'var(--ink)' : 'var(--ink-faint)' }}>{value}</span>
      <button type="button" style={b} aria-label={label + ' artır'} onClick={() => onChange(value + 1)}><DS.Icon name="add" size={20} /></button>
    </span>
  );
}

// Sayım paneli: üstte büyük tutar ve canlı sonuç; altında iki yol — toplamı yaz ya da banknotla say.
function KasaCountSheet({ account, onClose, onSave, initialMode = 'total' }) {
  const { AppBottomSheet, AppMoneyText, AppSubmitButton, AppSegmentedButton, Icon } = DS;
  const [mode, setMode] = React.useState(initialMode);
  const [typed, setTyped] = React.useState('23100');
  const [notes, setNotes] = React.useState(KASA_NOTES.map((x) => x.n));
  const [coins, setCoins] = React.useState('10');
  const noteSum = KASA_NOTES.reduce((s, x, i) => s + x.v * notes[i], 0) + (Number(coins.replace(',', '.')) || 0);
  const counted = mode === 'total' ? (Number(String(typed).replace(/\./g, '').replace(',', '.')) || 0) : noteSum;
  const n = Number((counted - Number(account.expected)).toFixed(2));
  const res = n === 0 ? ['income', 'check_circle', 'Tuttu', 'Kasa uygulamayla aynı.'] : n < 0 ? ['expense', 'difference', fmt(Math.abs(n).toFixed(4)) + ' eksik', 'Sayılan tutar beklenenden az.'] : ['neutral', 'difference', fmt(n.toFixed(4)) + ' fazla', 'Sayılan tutar beklenenden çok.'];
  return (
    <AppBottomSheet title="Sayımı gir" subtitle={account.label + ' · 25 Eylül'} onClose={onClose} maxHeight="92%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <div style={{ marginTop: 'var(--space-md)' }}>
        <AppSegmentedButton value={mode} onChange={setMode} options={[{ value: 'total', label: 'Toplamı yaz' }, { value: 'notes', label: 'Banknotla say' }]} />
      </div>

      <div style={{ marginTop: 'var(--space-lg)', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 'var(--space-xs)' }}>
        <span style={LBL}>Elde sayılan</span>
        {mode === 'total' ? (
          <label style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'center', gap: 4, minWidth: 220, padding: '4px var(--space-md) 8px', borderBottom: '2px solid var(--brand)', cursor: 'text' }}>
            <span style={{ font: '700 36px/1.1 var(--font-core)', color: 'var(--ink)' }}>₺</span>
            <input value={typed} onChange={(ev) => setTyped(ev.target.value)} inputMode="decimal" aria-label="Elde sayılan tutar" style={{ width: Math.max(3, typed.length) + 0.6 + 'ch', border: 'none', outline: 'none', background: 'transparent', padding: 0, font: '700 36px/1.1 var(--font-core)', letterSpacing: '-0.8px', fontFeatureSettings: '"tnum" 1', color: 'var(--ink)', textAlign: 'left' }} />
          </label>
        ) : <AppMoneyText amount={counted.toFixed(4)} size="hero" />}
      </div>

      <div style={{ marginTop: 'var(--space-md)', display: 'flex', alignItems: 'center', gap: 12, padding: '12px var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--' + res[0] + '-container)' }}>
        <Icon name={res[1]} size={22} color={'var(--on-' + res[0] + '-container)'} />
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
          <span style={{ font: '600 16px/1.3 var(--font-core)', color: 'var(--on-' + res[0] + '-container)' }}>{res[2]}</span>
          <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--on-' + res[0] + '-container)' }}>Uygulamaya göre {fmt(account.expected)}</span>
        </span>
      </div>

      {mode === 'notes' && (
        <div style={{ marginTop: 'var(--space-md)', border: '1px solid var(--border)', borderRadius: 'var(--radius-card)', background: 'var(--surface-card)' }}>
          <RowList inset={16}>
            {KASA_NOTES.map((x, i) => (
              <div key={x.v} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', padding: '6px 12px 6px 16px', minHeight: 56 }}>
                <span style={{ width: 64, font: '600 16px/1 var(--font-core)', color: 'var(--ink)', fontFeatureSettings: '"tnum" 1' }}>₺{x.v}</span>
                <NoteStepper value={notes[i]} label={x.v + ' lira'} onChange={(v) => setNotes(notes.map((q, j) => (j === i ? v : q)))} />
                <span style={{ flex: 1, minWidth: 0, textAlign: 'right' }}><AppMoneyText amount={(x.v * notes[i]).toFixed(4)} size="body" style={{ color: notes[i] ? 'var(--ink)' : 'var(--ink-faint)' }} /></span>
              </div>
            ))}
            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', padding: '6px 12px 6px 16px', minHeight: 56 }}>
              <span style={{ flex: 1, font: '600 16px/1 var(--font-core)', color: 'var(--ink)' }}>Madeni para</span>
              <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4, height: 44, padding: '0 12px', border: '1px solid var(--border)', borderRadius: 'var(--radius-field)', background: 'var(--surface-canvas)' }}>
                <span style={HELP}>₺</span>
                <input value={coins} onChange={(ev) => setCoins(ev.target.value)} inputMode="decimal" aria-label="Madeni para toplamı" style={{ width: 72, border: 'none', outline: 'none', background: 'transparent', font: '500 15px/1 var(--font-core)', color: 'var(--ink)', textAlign: 'right' }} />
              </span>
            </div>
          </RowList>
        </div>
      )}

      <p style={{ ...HELP, margin: 'var(--space-md) 0', textWrap: 'pretty' }}>{mode === 'notes' ? 'Her banknotun adedini girin; toplam kendiliğinden hesaplanır. ' : ''}Sayım bir gözlemdir; kaydetmek bakiyeyi değiştirmez.</p>
      <AppSubmitButton label="Sayımı kaydet" icon="check" fullWidth onSubmit={() => onSave(counted.toFixed(4), n.toFixed(4))} />
    </AppBottomSheet>
  );
}

function KasaPosSheet({ p, onClose }) {
  const { AppBottomSheet, AppMoneyText, AppStatusChip, AppSubmitButton } = DS;
  const done = p.state === 'done';
  return (
    <AppBottomSheet onClose={onClose} maxHeight="80%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: -20 }}>
        <Capsule icon="point_of_sale" />
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column' }}>
          <span style={ROWT}>{p.date} gün sonu</span>
          <span style={HELP}>POS tahsilatı · İşletme</span>
        </span>
        <button className="iconbtn" onClick={onClose} aria-label="Kapat" title="Kapat" style={{ marginRight: -12 }}><DS.Icon name="close" size={22} /></button>
      </div>
      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', marginTop: 'var(--space-md)' }}>
        <span style={{ flex: 1 }}><AppMoneyText amount={p.net} size="metric" style={{ fontSize: 28 }} /></span>
        {done ? <StatusTag label="Hesaba geçti" icon="check_circle" tone="income" /> : <StatusTag label={'Yolda · ' + p.expected} icon="schedule" tone="neutral" />}
      </div>
      <div style={{ ...HELP, marginTop: 2 }}>Hesaba geçecek net tutar</div>
      <div style={{ marginTop: 'var(--space-md)', padding: '4px var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)' }}>
        <RowList inset={0}>
          {[['Brüt satış', <AppMoneyText amount={p.gross} size="body" />], ['Komisyon', <AppMoneyText amount={p.commission} effect="expense" signed size="body" />], [done ? 'Geçtiği gün' : 'Beklenen gün', <span style={{ font: '500 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>{done ? p.transferredOn : p.expected}</span>], ['Geçeceği hesap', <span style={{ font: '500 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>Ziraat işletme</span>]].map(([l, v]) => (
            <div key={l} style={{ display: 'flex', alignItems: 'center', minHeight: 44 }}><span style={{ flex: 1, ...HELP }}>{l}</span>{v}</div>
          ))}
        </RowList>
      </div>
      <p style={{ ...HELP, margin: 'var(--space-md) 0', textWrap: 'pretty' }}>{done ? 'Tutar hesaba geçti; komisyon ayrı bir gider olarak kaydedildi.' : 'Yoldaki tutar net varlığa dahildir. Hesaba geçtiğinde işaretleyin; komisyon gider olarak kalır.'}</p>
      {!done && <AppSubmitButton label="Hesaba geçti" icon="check" fullWidth onSubmit={onClose} />}
    </AppBottomSheet>
  );
}

function KasaV4({ initialState = 'idle', initialPos = null }) {
  const { AppCard, AppMoneyText, AppStatusChip, AppSubmitButton, AppFilterChips, AppRowAction, Icon } = DS;
  const [acc, setAcc] = React.useState('dukkan');
  const [count, setCount] = React.useState(initialState === 'counted' || initialState === 'saved' ? { counted: '23100.0000', diff: '-85.0000' } : null);
  const [saved, setSaved] = React.useState(initialState === 'saved');
  const [sheet, setSheet] = React.useState(initialState === 'counting' || initialState === 'counting-notes');
  const [pos, setPos] = React.useState(initialPos ? DATA.pos.find((p) => p.id === initialPos) : null);
  const a = KASA_ACCOUNTS.find((x) => x.value === acc);
  const rootRef = React.useRef(null);
  const [host, setHost] = React.useState(null);
  React.useEffect(() => { setHost(rootRef.current && rootRef.current.closest('.phone')); }, []);
  const portal = (node) => (host ? ReactDOM.createPortal(node, host) : null);
  const transit = DATA.pos.filter((p) => p.state !== 'done');
  const done = DATA.pos.filter((p) => p.state === 'done');
  const transitTotal = transit.reduce((s, p) => s + Number(p.net), 0).toFixed(4);
  const kv = (l, amt, eff) => (
    <div key={l} style={{ display: 'flex', alignItems: 'center', minHeight: 36 }}>
      <span style={{ flex: 1, ...HELP }}>{l}</span>
      <AppMoneyText amount={amt} effect={eff} signed={!!eff} size="body" />
    </div>
  );
  return (
    <div className="screen" ref={rootRef}>
      <TopBar title="Kasa" actions={<ToolButton icon="history" label="Bütün sayımlar" />} />
      <div style={{ padding: '0 var(--space-md) 12px', flex: '0 0 auto' }}>
        <KasaPicker value={acc} onChange={(v) => { setAcc(v); setCount(null); setSaved(false); }} />
      </div>
      <div className="body" style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <AppCard padding={0}>
          <CardHead title="25 Eylül Perşembe" status={count ? <DiffChip diff={count.diff} saved={saved} /> : <StatusTag label="Sayılmadı" icon="schedule" tone="planned" />} />
          <div style={{ padding: 'var(--space-md)' }}>
          <div style={HELP}>{count ? 'Elde sayılan' : 'Uygulamaya göre kasada'}</div>
          <div style={{ marginTop: 2 }}><AppMoneyText amount={count ? count.counted : a.expected} size="hero" /></div>
          <div style={{ marginTop: 'var(--space-md)', paddingTop: 'var(--space-xs)', borderTop: '1px solid var(--border)' }}>
            {count ? <>
              {kv('Uygulamaya göre', a.expected)}
              {Number(count.diff) !== 0 && kv('Fark', Math.abs(Number(count.diff)).toFixed(4), Number(count.diff) < 0 ? 'expense' : 'income')}
            </> : <>
              {kv('Dünkü sayım', a.opening)}
              {kv('Bugün nakit giriş', a.inflow, 'income')}
              {kv('Bugün nakit çıkış', a.outflow, 'expense')}
            </>}
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: count && !saved && Number(count.diff) !== 0 ? 'minmax(0,1fr) minmax(0,1fr)' : '1fr', gap: 'var(--space-sm)', marginTop: 'var(--space-md)' }}>
            {!count && <AppSubmitButton label="Sayımı gir" icon="calculate" fullWidth onSubmit={() => setSheet(true)} />}
            {count && <AppSubmitButton label="Yeniden say" variant="outlined" fullWidth onSubmit={() => setSheet(true)} />}
            {count && !saved && Number(count.diff) !== 0 && <AppSubmitButton label="Farkı kaydet" icon="playlist_add_check" fullWidth onSubmit={() => setSaved(true)} />}
          </div>
          {count && <p style={{ ...HELP, margin: 'var(--space-sm) 0 0', textWrap: 'pretty' }}>{saved ? 'Tek bir gider kaydı oluştu; kasa sayılan tutara oturdu.' : Number(count.diff) === 0 ? 'Kasa uygulamayla aynı.' : 'Fark kendiliğinden yazılmaz. Kaydederseniz tek bir gider kaydı oluşur.'}</p>}
          </div>
        </AppCard>

        <Section title="POS tahsilatları" trailing={<TextAction label="Ekle" icon="add" onClick={() => {}} />}>
          <AppCard padding={0}>
            <CardHead title="Yolda" meta="hesaba geçmedi" status={<StatusTag label={transit.length + ' tahsilat'} icon="schedule" tone="neutral" />} />
            <div style={{ padding: '12px var(--space-md)' }}>
              <AppMoneyText amount={transitTotal} size="metric" />
              <div style={{ ...HELP, marginTop: 2 }}>Net varlığa dahildir</div>
            </div>
            <div style={{ height: 1, background: 'var(--border)' }} />
            <RowList inset={72}>
              {transit.map((p) => <Row key={p.id} onClick={() => setPos(p)} lead={<Capsule icon="schedule" tone="neutral" />} title={p.date + ' gün sonu'} subtitle={'Hesaba geçecek · ' + p.expected} trailing={<AppMoneyText amount={p.net} size="row" />} />)}
              {done.map((p) => <Row key={p.id} onClick={() => setPos(p)} lead={<Capsule icon="check" tone="income" />} title={p.date + ' gün sonu'} subtitle={'Hesaba geçti · ' + p.transferredOn} trailing={<AppMoneyText amount={p.net} size="row" style={{ color: 'var(--ink-muted)' }} />} />)}
            </RowList>
          </AppCard>
        </Section>

        <Section title="Son sayımlar" trailing={<TextAction label="Tümü" trailingIcon="chevron_right" onClick={() => {}} />}>
          <AppCard padding={0}>
            <RowList inset={76}>
              {KASA_HISTORY.map((h) => <KasaHistoryRow key={h.day} h={h} />)}
            </RowList>
          </AppCard>
        </Section>
      </div>
      {sheet && portal(<KasaCountSheet account={a} initialMode={initialState === 'counting-notes' ? 'notes' : 'total'} onClose={() => setSheet(false)} onSave={(c, d) => { setCount({ counted: c, diff: d }); setSaved(false); setSheet(false); }} />)}
      {pos && portal(<KasaPosSheet p={pos} onClose={() => setPos(null)} />)}
    </div>
  );
}
Object.assign(window, { KasaV4 });
