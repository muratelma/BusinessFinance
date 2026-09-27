// Özet v3 parçaları — her bölüm için üç öneri. Sayfalar bunları karıştırır.
const BUDGETS_V3 = [
  { name: 'Ticari mal alımı', icon: 'sell', scope: 'İşletme', spent: '26507.2000', limit: '25000.0000', diff: '1507.2000', over: true },
  { name: 'Araç ve yakıt', icon: 'local_gas_station', scope: 'İşletme', spent: '3260.0000', limit: '3000.0000', diff: '260.0000', over: true },
  { name: 'Elektrik, su, doğalgaz', icon: 'bolt', scope: 'İşletme', spent: '3950.8800', limit: '4500.0000', diff: '549.1200' },
  { name: 'Kırtasiye', icon: 'edit', scope: 'İşletme', spent: '640.0000', limit: '1000.0000', diff: '360.0000' },
];
const REL = { '26 Eylül': 'Yarın', '27 Eylül': '2 gün sonra', '28 Eylül': '3 gün sonra', '29 Eylül': '4 gün sonra', '30 Eylül': '5 gün sonra' };
const MUTED16 = { font: '500 16px/1.3 var(--font-core)', color: 'var(--ink-muted)' };

function heroData(scope) {
  if (scope === 'business') return { label: 'İşletme neti', amount: DATA.businessNet };
  if (scope === 'personal') return { label: 'Şahsi net', amount: DATA.personalNet };
  return { label: 'Bu ayın neti', amount: DATA.net, caption: DATA.netCaption, trend: true,
    parts: [['İşletme neti', DATA.businessNet], ['Şahsi net', DATA.personalNet]] };
}
function HeroCaption({ h }) {
  if (!h.caption) return null;
  return <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 6, ...HELP }}>{h.trend && <DS.Icon name="trending_up" size={18} color="var(--ink-muted)" />}{h.caption}</div>;
}

// Bölüm başlığı: ekranı olan bölümde tüm başlık dokunulabilir, sağda ok.
function LinkHeader({ title, trailing, onClick }) {
  if (!onClick) return <DS.AppSectionHeader title={title} trailing={trailing} />;
  return (
    <button onClick={onClick} aria-label={title + ' ekranını aç'} style={{ display: 'block', width: '100%', padding: 0, border: 'none', background: 'transparent', cursor: 'pointer', textAlign: 'left' }}>
      <DS.AppSectionHeader title={title} trailing={<span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>{trailing}<DS.Icon name="chevron_right" size={22} color="var(--ink-muted)" /></span>} />
    </button>
  );
}
function Block({ title, trailing, onOpen, children }) {
  return <div style={{ marginTop: 'var(--space-lg)' }}><LinkHeader title={title} trailing={trailing} onClick={onOpen} />{children}</div>;
}
function CardFooterLink({ label, onClick }) {
  return (
    <button onClick={onClick} style={{ display: 'flex', alignItems: 'center', width: '100%', minHeight: 48, padding: '0 12px 0 var(--space-md)', border: 'none', borderTop: '1px solid var(--border)', background: 'transparent', cursor: 'pointer', font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>
      <span style={{ flex: 1, textAlign: 'left' }}>{label}</span><DS.Icon name="chevron_right" size={20} color="var(--ink-muted)" />
    </button>
  );
}

/* ---------- Net (hero) ---------- */
// 1 — Tek kart, altta gelir/gider renkli taban.
function Hero1({ scope }) {
  const { AppCard, AppMoneyText } = DS; const h = heroData(scope);
  const foot = (l, a, e, ic) => (
    <div style={{ padding: 'var(--space-md) 20px', background: 'var(--' + e + '-container)', display: 'flex', flexDirection: 'column', gap: 4, minWidth: 0 }}>
      <span style={{ display: 'flex', alignItems: 'center', gap: 6, font: '500 14px/1.3 var(--font-core)', color: 'var(--on-' + e + '-container)' }}><DS.Icon name={ic} size={18} color={'var(--on-' + e + '-container)'} />{l}</span>
      <AppMoneyText amount={a} effect={e} size="metric" onContainer />
    </div>
  );
  return (
    <AppCard padding={0} style={{ overflow: 'hidden' }}>
      <div style={{ padding: 'var(--space-lg) 20px 20px' }}>
        <div style={MUTED16}>{h.label}</div>
        <div style={{ marginTop: 8 }}><AppMoneyText amount={h.amount} size="hero" /></div>
        <HeroCaption h={h} />
        {h.parts && (
          <div style={{ display: 'flex', gap: 'var(--space-lg)', marginTop: 20 }}>
            {h.parts.map(([l, a]) => <div key={l} style={{ display: 'flex', flexDirection: 'column', gap: 2 }}><span style={HELP}>{l}</span><AppMoneyText amount={a} size="row" /></div>)}
          </div>
        )}
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr' }}>{foot('Gelir', DATA.income, 'income', 'south_west')}{foot('Gider', DATA.expense, 'expense', 'north_east')}</div>
    </AppCard>
  );
}
// 2 — Eskiye yakın: hero kartta kırılım satırları, altında renkli iki kutu.
function Hero2({ scope }) {
  const { AppCard, AppMoneyText, AppMetricTile, AppResponsiveGrid } = DS; const h = heroData(scope);
  return (
    <>
      <AppCard padding="var(--space-lg) 20px var(--space-md)">
        <div style={MUTED16}>{h.label}</div>
        <div style={{ marginTop: 8 }}><AppMoneyText amount={h.amount} size="hero" /></div>
        <HeroCaption h={h} />
        {h.parts && (
          <div style={{ marginTop: 'var(--space-md)' }}>
            {h.parts.map(([l, a]) => (
              <div key={l} style={{ display: 'flex', alignItems: 'center', minHeight: 48, borderTop: '1px solid var(--border)' }}>
                <span style={{ flex: 1, font: 'var(--text-body-large-size)/1.4 var(--font-core)', color: 'var(--ink)' }}>{l}</span>
                <AppMoneyText amount={a} size="row" />
              </div>
            ))}
          </div>
        )}
      </AppCard>
      <div style={{ height: 'var(--space-sm)' }} />
      <AppResponsiveGrid minItemWidth={150}>
        <AppMetricTile label="Gelir" amount={DATA.income} icon="south_west" effect="income" tinted style={{ borderColor: 'var(--income-container)' }} />
        <AppMetricTile label="Gider" amount={DATA.expense} icon="north_east" effect="expense" tinted style={{ borderColor: 'var(--expense-container)' }} />
      </AppResponsiveGrid>
    </>
  );
}
// 3 — Hesap özeti: gelir − gider = net, alt alta, ekstre gibi.
function Hero3({ scope }) {
  const { AppCard, AppMoneyText } = DS; const h = heroData(scope);
  const line = (l, a, e, ic) => (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48 }}>
      <Capsule icon={ic} tone={e} size={32} />
      <span style={{ flex: 1, font: 'var(--text-body-large-size)/1.4 var(--font-core)', color: 'var(--ink)' }}>{l}</span>
      <AppMoneyText amount={a} effect={e} signed size="row" />
    </div>
  );
  return (
    <AppCard padding="var(--space-md) 20px 20px">
      {line('Gelir', DATA.income, 'income', 'south_west')}
      {line('Gider', DATA.expense, 'expense', 'north_east')}
      <div style={{ height: 2, background: 'var(--ink)', margin: '8px 0 var(--space-md)' }} />
      <div style={{ display: 'flex', alignItems: 'flex-end', gap: 12 }}>
        <span style={{ flex: 1, ...MUTED16, color: 'var(--ink)', fontWeight: 600, paddingBottom: 6 }}>{h.label}</span>
        <AppMoneyText amount={h.amount} size="hero" />
      </div>
      <div style={{ display: 'flex', justifyContent: 'flex-end' }}><HeroCaption h={h} /></div>
      {h.parts && (
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 'var(--space-sm)', marginTop: 'var(--space-md)', paddingTop: 12, borderTop: '1px solid var(--border)' }}>
          {h.parts.map(([l, a]) => <div key={l} style={{ display: 'flex', flexDirection: 'column', gap: 2 }}><span style={HELP}>{l}</span><AppMoneyText amount={a} size="row" /></div>)}
        </div>
      )}
    </AppCard>
  );
}

/* ---------- Kategori giderleri ---------- */
const catSlices = () => DATA.categories.map((c) => ({ label: c.name, value: Number(c.amount), color: c.color, formattedValue: fmt(c.amount) }));
const catTotal = () => DATA.categories.reduce((s, c) => s + Number(c.amount), 0);
// 1 — Eskisi gibi: halka solda, efsane sağda (tasarım sisteminin kendi efsanesi).
function Cat1() {
  return <DS.AppCard><DS.AppDonutChart slices={catSlices()} size={128} thickness={22} /></DS.AppCard>;
}
// 2 — Büyük halka ortada, toplam içinde; efsane altta satır satır.
function Cat2() {
  const t = catTotal();
  return (
    <DS.AppCard padding="20px var(--space-md) 8px">
      <div style={{ display: 'flex', justifyContent: 'center' }}>
        <DS.AppDonutChart slices={catSlices()} size={184} thickness={22} showLegend={false} centerLabel="Toplam gider" centerValue={fmt(DATA.expense)} />
      </div>
      <div style={{ marginTop: 'var(--space-md)' }}>
        {DATA.categories.map((c) => (
          <div key={c.name} style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 44, borderTop: '1px solid var(--border)' }}>
            <span style={{ width: 10, height: 10, borderRadius: '50%', background: c.color, flex: '0 0 auto' }} />
            <span style={{ flex: 1, minWidth: 0, font: 'var(--text-body-size)/1.4 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{c.name}</span>
            <span style={{ ...HELP, width: 40, textAlign: 'right', fontFeatureSettings: '"tnum" 1' }}>%{Math.round(Number(c.amount) / t * 100)}</span>
            <DS.AppMoneyText amount={c.amount} size="body" style={{ fontWeight: 600, minWidth: 92, textAlign: 'right' }} />
          </div>
        ))}
      </div>
    </DS.AppCard>
  );
}
// 3 — Küçük halka + özet cümlesi; altında ikonlu satırlar ve pay çubukları.
function Cat3() {
  const t = catTotal(); const top = DATA.categories[0];
  return (
    <DS.AppCard>
      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
        <DS.AppDonutChart slices={catSlices()} size={88} thickness={14} showLegend={false} />
        <div style={{ display: 'flex', flexDirection: 'column', gap: 2, minWidth: 0 }}>
          <span style={HELP}>Toplam gider</span>
          <DS.AppMoneyText amount={DATA.expense} size="metric" />
          <span style={HELP}>En büyük: {top.name} %{Math.round(Number(top.amount) / t * 100)}</span>
        </div>
      </div>
      <div style={{ marginTop: 'var(--space-md)', display: 'flex', flexDirection: 'column', gap: 'var(--space-sm)' }}>
        {DATA.categories.map((c) => (
          <div key={c.name} style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <span style={{ width: 32, height: 32, borderRadius: '50%', background: 'var(--surface-card-muted)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', flex: '0 0 auto' }}><DS.Icon name={c.icon} size={18} color={c.color} /></span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 6 }}>
              <span style={{ display: 'flex', alignItems: 'baseline', gap: 8 }}>
                <span style={{ flex: 1, minWidth: 0, font: 'var(--text-body-size)/1.3 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{c.name}</span>
                <DS.AppMoneyText amount={c.amount} size="body" style={{ fontWeight: 600 }} />
              </span>
              <DS.AppShareBar ratio={Number(c.amount) / t} color={c.color} height={4} />
            </span>
          </div>
        ))}
      </div>
    </DS.AppCard>
  );
}

/* ---------- Bütçeler (dokununca Bütçeler ekranı) ---------- */
const overBudgets = () => BUDGETS_V3.filter((b) => b.over);
// 1 — Yalnız aşılanlar gösterilir, altta "Tüm bütçeler".
function Budget1({ onOpen }) {
  const over = overBudgets();
  return (
    <DS.AppCard padding={0} onClick={onOpen}>
      <div style={{ display: 'flex', gap: 'var(--space-sm)', padding: 'var(--space-md) var(--space-md) 4px', flexWrap: 'wrap' }}>
        <DS.AppStatusChip label={over.length + ' aşıldı'} icon="warning" tone="expense" />
        <DS.AppStatusChip label={(BUDGETS_V3.length - over.length) + ' limit içinde'} icon="check_circle" tone="planned" />
      </div>
      {over.map((b) => (
        <div key={b.name} style={{ padding: '12px var(--space-md)' }}>
          <div style={{ display: 'flex', alignItems: 'baseline', gap: 8 }}>
            <span style={{ ...ROWT, flex: 1, minWidth: 0, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{b.name}</span>
            <span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--expense)' }}>{fmt(b.diff)} fazla</span>
          </div>
          <DS.AppShareBar ratio={1} color="var(--expense-fill)" height={6} style={{ marginTop: 8 }} />
          <div style={{ ...HELP, marginTop: 6 }}>{fmt(b.spent)} / {fmt(b.limit)}</div>
        </div>
      ))}
      <CardFooterLink label={'Tüm bütçeler (' + BUDGETS_V3.length + ')'} onClick={onOpen} />
    </DS.AppCard>
  );
}
// 2 — Tek satırlık özet; altında her bütçe için küçük bir doluluk çubuğu.
function Budget2({ onOpen }) {
  const over = overBudgets();
  return (
    <DS.AppCard onClick={onOpen}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
        <Capsule icon="donut_small" tone={over.length ? 'expense' : undefined} />
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
          <span style={ROWT}>{BUDGETS_V3.length} bütçeden {over.length} tanesi aşıldı</span>
          <span style={{ ...HELP, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{over.map((b) => b.name).join(', ')}</span>
        </span>
        <DS.Icon name="chevron_right" size={22} color="var(--ink-muted)" />
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(' + BUDGETS_V3.length + ', minmax(0,1fr))', gap: 'var(--space-sm)', marginTop: 'var(--space-md)' }}>
        {BUDGETS_V3.map((b) => {
          const r = Number(b.spent) / Number(b.limit);
          return (
            <div key={b.name} style={{ display: 'flex', flexDirection: 'column', gap: 6, minWidth: 0 }}>
              <DS.AppShareBar ratio={Math.min(1, r)} color={b.over ? 'var(--expense-fill)' : 'var(--neutral-fill)'} height={6} />
              <span style={{ ...LBL, color: b.over ? 'var(--expense)' : 'var(--ink-muted)' }}>%{Math.round(r * 100)}</span>
            </div>
          );
        })}
      </div>
    </DS.AppCard>
  );
}
// 3 — Küçük halkalar yan yana; en fazla dört, gerisi Bütçeler ekranında.
function Budget3({ onOpen }) {
  const shown = [...BUDGETS_V3].sort((a, b) => Number(b.spent) / Number(b.limit) - Number(a.spent) / Number(a.limit)).slice(0, 4);
  return (
    <DS.AppCard padding={0} onClick={onOpen}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, minmax(0,1fr))', gap: 4, padding: 'var(--space-md) 8px' }}>
        {shown.map((b) => {
          const r = Number(b.spent) / Number(b.limit);
          const fill = b.over ? 'var(--expense-fill)' : 'var(--neutral-fill)';
          return (
            <div key={b.name} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, minWidth: 0 }}>
              <DS.AppDonutChart size={64} thickness={7} showLegend={false} centerValue={'%' + Math.round(r * 100)}
                slices={[{ label: 'Harcanan', value: Math.min(1, r), color: fill }, { label: 'Kalan', value: Math.max(0, 1 - r), color: 'var(--surface-card-muted)' }]} />
              <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--ink)', textAlign: 'center', width: '100%', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{b.name}</span>
              <span style={{ font: '600 12px/1.2 var(--font-core)', color: b.over ? 'var(--expense)' : 'var(--ink-muted)' }}>{b.over ? 'Aşıldı' : fmt(b.diff) + ' kaldı'}</span>
            </div>
          );
        })}
      </div>
      <CardFooterLink label="Tüm bütçeler" onClick={onOpen} />
    </DS.AppCard>
  );
}

/* ---------- Yaklaşanlar (dokununca Planlananlar) ---------- */
// 1 — Zaman çizelgesi: solda tarih, ortada çizgi, sağda tutar; altta toplam.
function Up1({ onOpen, items = DATA.upcoming, total = DATA.upcomingTotal }) {
  const n = items.length;
  return (
    <DS.AppCard padding={0} onClick={onOpen}>
      <div style={{ padding: '8px var(--space-md)' }}>
        {items.map((u, i) => {
          const [d, m] = u.due.split(' ');
          return (
            <div key={u.title} style={{ display: 'grid', gridTemplateColumns: '40px 16px minmax(0,1fr) auto', alignItems: 'center', columnGap: 8, minHeight: 60 }}>
              <span style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <span style={{ font: '700 18px/1.1 var(--font-core)', color: 'var(--ink)', fontFeatureSettings: '"tnum" 1' }}>{d}</span>
                <span style={{ font: '600 12px/1.2 var(--font-core)', color: 'var(--ink-muted)' }}>{m.slice(0, 3)}</span>
              </span>
              <span style={{ position: 'relative', alignSelf: 'stretch', display: 'flex', justifyContent: 'center' }}>
                <span style={{ position: 'absolute', top: i === 0 ? '50%' : 0, bottom: i === n - 1 ? '50%' : 0, width: 2, background: 'var(--border)' }} />
                <span style={{ position: 'relative', alignSelf: 'center', width: 10, height: 10, borderRadius: '50%', background: i === 0 ? 'var(--ink)' : 'var(--surface-card)', border: '2px solid var(--ink)', boxSizing: 'border-box' }} />
              </span>
              <span style={{ display: 'flex', flexDirection: 'column', gap: 2, minWidth: 0 }}>
                <span style={{ ...ROWT, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{u.title}</span>
                <span style={HELP}>{REL[u.due]} · {u.kind}</span>
              </span>
              <DS.AppMoneyText amount={u.amount} effect="expense" size="row" />
            </div>
          );
        })}
      </div>
      <div style={{ display: 'flex', alignItems: 'center', minHeight: 48, padding: '0 var(--space-md)', borderTop: '1px solid var(--border)', background: 'var(--surface-card-muted)', borderRadius: '0 0 var(--radius-card) var(--radius-card)' }}>
        <span style={{ flex: 1, font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>7 günde çıkacak</span>
        <DS.AppMoneyText amount={total} effect="expense" size="row" />
      </div>
    </DS.AppCard>
  );
}
// 2 — Takvim yaprağı + "Yarın / 3 gün sonra".
function Up2({ onOpen }) {
  return (
    <DS.AppCard padding={0} onClick={onOpen}>
      <RowList inset={76}>
        {DATA.upcoming.map((u) => {
          const [d, m] = u.due.split(' ');
          return <Row key={u.title} lead={<DateLeaf day={d} month={m.slice(0, 3)} />} title={u.title} subtitle={REL[u.due] + ' · ' + u.kind} trailing={<DS.AppMoneyText amount={u.amount} effect="expense" size="row" />} />;
        })}
      </RowList>
      <CardFooterLink label={'Tüm planlananlar (' + DATA.plannedSummary.count + ')'} onClick={onOpen} />
    </DS.AppCard>
  );
}
// 3 — Yatay kaydırılan kartlar; en sonda "Tümünü gör".
function Up3({ onOpen }) {
  const card = { flex: '0 0 164px', minHeight: 132, padding: 'var(--space-md)', border: '1px solid var(--border)', borderRadius: 'var(--radius-card)', background: 'var(--surface-card)', display: 'flex', flexDirection: 'column', gap: 8, boxSizing: 'border-box', textAlign: 'left', cursor: 'pointer' };
  return (
    <div style={{ display: 'flex', gap: 'var(--space-sm)', overflowX: 'auto', margin: '0 calc(var(--space-md) * -1)', padding: '0 var(--space-md)', scrollbarWidth: 'none' }}>
      {DATA.upcoming.map((u, i) => (
        <button key={u.title} onClick={onOpen} style={card}>
          <span style={{ alignSelf: 'flex-start', padding: '4px 10px', borderRadius: 'var(--radius-chip)', background: i === 0 ? 'var(--brand)' : 'var(--surface-card-muted)', color: i === 0 ? 'var(--on-brand)' : 'var(--ink)', font: '600 12px/1.3 var(--font-core)' }}>{REL[u.due]}</span>
          <span style={{ ...ROWT, flex: 1 }}>{u.title}</span>
          <span style={HELP}>{u.due}</span>
          <DS.AppMoneyText amount={u.amount} effect="expense" size="row" />
        </button>
      ))}
      <button onClick={onOpen} style={{ ...card, flex: '0 0 112px', alignItems: 'center', justifyContent: 'center', background: 'var(--surface-card-muted)', borderColor: 'var(--surface-card-muted)' }}>
        <DS.Icon name="arrow_forward" size={24} color="var(--ink)" />
        <span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--ink)', textAlign: 'center' }}>Tümünü gör</span>
      </button>
    </div>
  );
}

Object.assign(window, { BUDGETS_V3, LinkHeader, Block, CardFooterLink, Hero1, Hero2, Hero3, Cat1, Cat2, Cat3, Budget1, Budget2, Budget3, Up1, Up2, Up3 });
