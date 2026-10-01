// Brif 1 — Vergiler. Bugün 29 Eylül 2026 Salı, işletme profili. Veri sentetik.
const TAX_PENDING = [
  { id: 'kdv8', name: 'KDV', period: 'Ağustos', day: '28', month: 'Eyl', due: '28 Eyl', rel: '1 gün gecikti', late: true, amount: null },
  { id: 'bag9', name: 'Bağkur', period: 'Eylül', day: '30', month: 'Eyl', due: '30 Eyl', rel: 'yarın', amount: '8950.0000' },
  { id: 'kdv9', name: 'KDV', period: 'Eylül', day: '28', month: 'Eki', due: '28 Eki', rel: '29 gün', amount: null },
];
const TAX_DEFS = [
  { name: 'Bağkur', icon: 'health_and_safety', rhythm: 'Her ay · ay sonu', next: '30 Eyl', amount: '8950.0000' },
  { name: 'KDV', icon: 'receipt_long', rhythm: "Her ay · 28'i", next: '28 Eki' },
  { name: 'Geçici vergi', icon: 'event_repeat', rhythm: "Şub · May · Ağu · Kas · 17'si", next: '17 Kas' },
  { name: 'Motorlu taşıtlar', icon: 'directions_car', rhythm: 'Ocak, Temmuz · ay sonu', next: '31 Oca', tag: ['Şahsi', 'person', 'planned'] },
  { name: 'Tabela', icon: 'signpost', rhythm: 'Yılda bir · Ocak sonu', tag: ['Duraklatıldı', 'pause_circle', 'planned'] },
];
const TAX_PAID = [
  { day: '26', month: 'Eyl', name: 'Muhtasar', source: 'Dükkan hesabı', amount: '3240.0000' },
  { day: '15', month: 'Eyl', name: 'Vergi ödemesi', source: 'Nakit kasa', closed: 'Bağkur Temmuz, Bağkur Ağustos', amount: '12500.0000' },
  { day: '31', month: 'Tem', name: 'MTV 1. taksit', source: 'Bonus kart', amount: '2180.0000' },
];
const TAX_SOURCES = [
  { value: 'dukkan', label: 'Dükkan hesabı' }, { value: 'kasa', label: 'Nakit kasa' }, { value: 'ziraat', label: 'Ziraat işletme' }, { value: 'bonus', label: 'Bonus · kredi kartı' },
];

// pay: 'pill' (önceki 1) | 'circle' (önceki 2) | 'outline' | 'inline' | 'square'
const PayStyle = React.createContext('outline');
function PayAction({ kind, onClick = () => {} }) {
  if (kind === 'pill') return <DS.AppRowAction label="Ödedim" onClick={onClick} />;
  if (kind === 'circle') return <PayCheck onClick={onClick} />;
  if (kind === 'square') return <button type="button" onClick={onClick} aria-label="Ödedim" title="Ödedim" style={{ flex: '0 0 auto', width: 48, height: 48, border: 'none', borderRadius: 'var(--radius-field)', background: 'var(--brand-container)', color: 'var(--on-brand-container)', display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer' }}><DS.Icon name="task_alt" size={24} /></button>;
  return <button type="button" onClick={onClick} style={{ flex: '0 0 auto', display: 'inline-flex', alignItems: 'center', gap: 6, minHeight: 36, padding: '0 14px 0 10px', border: '1px solid var(--border-strong)', borderRadius: 'var(--radius-chip)', background: 'var(--surface-card)', color: 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer', alignSelf: kind === 'inline' ? 'flex-start' : 'center' }}><DS.Icon name="check_circle" size={18} />Ödedim</button>;
}
function PendingRow({ p, onOpen, onPay }) {
  const { AppMoneyText } = DS;
  const kind = React.useContext(PayStyle);
  return (
    <div style={{ padding: '12px 16px', minHeight: 72, boxSizing: 'border-box' }}>
    <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
      <button type="button" onClick={onOpen} style={{ flex: 1, minWidth: 0, display: 'flex', alignItems: 'center', gap: 'var(--space-md)', border: 'none', background: 'transparent', padding: 0, textAlign: 'left', cursor: 'pointer', font: 'inherit' }}>
        <DateLeaf day={p.day} month={p.month} tone={p.late ? 'expense' : undefined} />
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 3 }}>
          <span style={ROWT}>{p.name} · {p.period}</span>
          <span style={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', columnGap: 6, rowGap: 2 }}>
            {p.late ? <StatusTag label="Gecikti" icon="error" tone="expense" /> : <StatusTag label="Yaklaşıyor" icon="schedule" tone="planned" />}
            <span style={{ ...HELP, whiteSpace: 'nowrap' }}>{p.rel}</span>
          </span>
          {p.amount ? <AppMoneyText amount={p.amount} size="body" style={{ fontWeight: 600 }} /> : <span style={{ ...HELP, whiteSpace: 'nowrap' }}>Tutar ödemede girilecek</span>}
        </span>
      </button>
      {kind !== 'inline' && <PayAction kind={kind} onClick={onPay} />}
    </div>
    {kind === 'inline' && <div style={{ display: 'flex', marginTop: 'var(--space-sm)', paddingLeft: 60 }}><PayAction kind="inline" onClick={onPay} /></div>}
    </div>
  );
}

// Satır eylemi: işaretlenecek bir yuvarlak + altında etiketi — "ödendi olarak işaretle" hissi.
function PayCheck({ onClick = () => {} }) {
  return (
    <button type="button" onClick={onClick} aria-label="Ödedim" style={{ flex: '0 0 auto', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4, minWidth: 56, minHeight: 56, padding: '4px 0', border: 'none', background: 'transparent', cursor: 'pointer', font: 'inherit' }}>
      <span style={{ width: 36, height: 36, boxSizing: 'border-box', borderRadius: 999, border: '2px solid var(--border-strong)', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'var(--surface-card)' }}><DS.Icon name="check" size={20} color="var(--ink)" /></span>
      <span style={{ font: '600 13px/1.2 var(--font-core)', color: 'var(--ink)' }}>Ödedim</span>
    </button>
  );
}

function PaidRow({ x, onOpen }) {
  return <Row onClick={onOpen} lead={<DateLeaf day={x.day} month={x.month} />} title={x.name} subtitle={x.source + (x.closed ? ' · Kapattı: ' + x.closed : '')} trailing={<DS.AppMoneyText amount={x.amount} effect="expense" signed size="row" />} />;
}

function PaidSection({ items = TAX_PAID }) {
  return (
    <Section title="Ödenenler" trailing={<TextAction label="Tümü" trailingIcon="chevron_right" />}>
      <DS.AppCard padding={0}><RowList inset={76}>{items.map((x) => <PaidRow key={x.day + x.name} x={x} />)}</RowList></DS.AppCard>
      <Rule>Vergi işaretli kategorilerdeki giderler; Gider formundan girilenler de burada görünür.</Rule>
    </Section>
  );
}

// variant: 'empty' | 'empty-paid' | 'full'
function VergilerScreen({ variant = 'full', scroll = 0, overlay = null, pay = 'outline' }) {
  const { AppCard, AppMoneyText, AppSubmitButton } = DS;
  const empty = variant === 'empty' || variant === 'empty-paid';
  const main = <AppSubmitButton label="Vergi ödemesi ekle" icon="add" fullWidth onSubmit={() => {}} />;
  return (
    <>
      <PayStyle.Provider value={pay}>
      <Page title="Vergiler" scroll={scroll} footer={main}>
        {empty ? (
          <>
            <AppCard padding="var(--space-lg)">
              <Capsule icon="receipt_long" size={56} />
              <div style={{ font: '600 18px/1.35 var(--font-core)', color: 'var(--ink)', marginTop: 'var(--space-md)' }}>Ödediğiniz vergiyi tek tutarla yazın</div>
              <p style={{ ...HELP, margin: 'var(--space-xs) 0 0', fontSize: 15, textWrap: 'pretty' }}>İsterseniz vergilerinizi tanımlayıp ne zaman ödeneceğini takip edin. Ödenene kadar hiçbir vergi bakiyenizi ya da bütçenizi etkilemez.</p>
              <div style={{ marginTop: 'var(--space-md)' }}><AppSubmitButton label="Vergilerimi tanımla" icon="event_repeat" variant="outlined" fullWidth onSubmit={() => {}} /></div>
            </AppCard>
            {variant === 'empty-paid' ? <PaidSection items={TAX_PAID.slice(0, 1)} /> : <Rule style={{ marginTop: 'var(--space-md)' }}>Ödenen vergi normal bir giderdir; İşlemler'de de görünür. Hangi vergileri ödemeniz gerektiğini muhasebeciniz söyler.</Rule>}
          </>
        ) : (
          <>
            <Section first title="Bekleyenler">
              <AppCard padding={0}>
                <CardHead title="Gecikenler ve 30 gün" status={<StatusTag label="1 gecikti" icon="error" tone="expense" />} />
                <RowList inset={76}>{TAX_PENDING.map((p) => <PendingRow key={p.id} p={p} />)}</RowList>
                <div style={{ display: 'grid', gridTemplateColumns: 'minmax(0,1fr) 1px minmax(0,1fr)', borderTop: '1px solid var(--border)', background: 'var(--surface-card-muted)' }}>
                  <div style={{ padding: '12px 16px', display: 'flex', flexDirection: 'column', alignItems: 'flex-start', gap: 4 }}><span style={{ ...HELP, whiteSpace: 'nowrap' }}>30 günde çıkacak</span><AppMoneyText amount="8950.0000" effect="expense" size="row" /></div>
                  <span style={{ background: 'var(--border)', margin: '12px 0' }}></span>
                  <div style={{ padding: '12px 16px', display: 'flex', flexDirection: 'column', alignItems: 'flex-start', gap: 4 }}><span style={{ ...HELP, whiteSpace: 'nowrap' }}>Tutarı belli olmayan</span><span style={{ font: '600 16px/1.3 var(--font-core)', color: 'var(--ink)', whiteSpace: 'nowrap' }}>2 ödeme</span></div>
                </div>
              </AppCard>
            </Section>
            <Section title="Vergilerim" trailing={<TextAction label="Ekle" icon="add" />}>
              <AppCard padding={0}>
                <RowList inset={72}>
                  {TAX_DEFS.map((d) => <Row key={d.name} lead={<Capsule icon={d.icon} tone={d.tag && d.tag[0] === 'Duraklatıldı' ? 'cancelled' : undefined} />} title={d.name}
                    subtitle={d.rhythm + (d.next ? ' · sıradaki ' + d.next : '')}
                    trailing={d.tag ? <StatusTag label={d.tag[0]} icon={d.tag[1]} tone={d.tag[2]} /> : d.amount ? <AppMoneyText amount={d.amount} size="body" /> : null} onClick={() => {}} />)}
                </RowList>
              </AppCard>
            </Section>
            <PaidSection />
          </>
        )}
      </Page>
      </PayStyle.Provider>
      {overlay}
    </>
  );
}

// V3 · Ödedim
function OdedimSheet({ kind = 'bagkur' }) {
  const { AppBottomSheet, AppTextField, AppSelectField, AppSubmitButton } = DS;
  const late = kind === 'kdv';
  const [src, setSrc] = React.useState(late ? 'bonus' : 'dukkan');
  const card = src === 'bonus';
  return (
    <AppBottomSheet title={late ? 'KDV · Ağustos' : 'Bağkur · Eylül'} subtitle={late ? 'Vade 28 Eylül · 1 gün gecikti' : 'Vade 30 Eylül · yarın'} maxHeight="92%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <div style={{ marginTop: 'var(--space-lg)' }}><AmountInput label="Ödenen tutar" value={late ? '' : '8.950,00'} /></div>
      <Rule style={{ textAlign: 'center' }}>{late ? 'Gecikme zammı dahil, ödediğiniz tutarı yazın; uygulama zammı hesaplamaz.' : 'Tanımdaki tutar; ödediğiniz farklıysa değiştirin.'}</Rule>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-md)', marginTop: 'var(--space-lg)' }}>
        <AppTextField label="Ödeme günü" value="29 Eylül 2026 · bugün" prefixIcon="event" labelBackdrop="var(--surface-card)" />
        <AppSelectField label="Nereden ödendi" value={src} onChange={setSrc} options={TAX_SOURCES} labelBackdrop="var(--surface-card)" helperText={card ? 'Kart harcaması olarak yazılır; kart borcunuza eklenir.' : late ? undefined : 'Tanımdan geldi.'} />
      </div>
      <Rule style={{ margin: 'var(--space-md) 0' }}>Gider ödeme gününe yazılır; vadeye değil.</Rule>
      <AppSubmitButton label="Ödemeyi kaydet" icon="check" fullWidth onSubmit={late ? undefined : () => {}} />
    </AppBottomSheet>
  );
}

// V4 / V5 · Vergi ödemesi ekle (toplu)
function OdemeEkleSheet({ withPending = false }) {
  const { AppBottomSheet, AppTextField, AppSelectField, AppSubmitButton } = DS;
  const [sel, setSel] = React.useState(['kdv8']);
  return (
    <AppBottomSheet title="Vergi ödemesi ekle" subtitle="Tek tutar; tanım gerekmez" maxHeight="94%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <div style={{ marginTop: 'var(--space-lg)' }}><AmountInput value={withPending ? '17.400' : '12.500'} /></div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-md)', marginTop: 'var(--space-lg)' }}>
        <AppTextField label="Ödeme günü" value="29 Eylül 2026 · bugün" prefixIcon="event" labelBackdrop="var(--surface-card)" />
        <AppSelectField label="Nereden ödendi" value="kasa" options={TAX_SOURCES} labelBackdrop="var(--surface-card)" />
        {!withPending && <AppTextField label="Not (isteğe bağlı)" placeholder="ör. Temmuz–Ağustos Bağkur" labelBackdrop="var(--surface-card)" />}
      </div>
      {withPending ? (
        <>
          <FieldLabel hint="Tutar kalemlere dağıtılmaz; seçtikleriniz ödendi sayılır ve bekleyenlerden düşer.">Bu ödeme hangilerini kapatıyor?</FieldLabel>
          <div style={{ border: '1px solid var(--border)', borderRadius: 'var(--radius-card)' }}>
            <RowList inset={48}>
              {TAX_PENDING.map((p) => <CheckRow key={p.id} on={sel.includes(p.id)} onClick={() => setSel(sel.includes(p.id) ? sel.filter((x) => x !== p.id) : [...sel, p.id])} title={p.name + ' · ' + p.period} subtitle={p.due + ' · ' + p.rel} trailing={p.late ? <StatusTag label="Gecikti" icon="error" tone="expense" /> : null} />)}
            </RowList>
          </div>
          <Rule>Hiçbirini seçmeden de kaydedebilirsiniz.</Rule>
        </>
      ) : (
        <DetailBlock style={{ marginTop: 'var(--space-md)' }} rows={[['Kategori', 'Vergi ödemesi']]} />
      )}
      <Rule style={{ margin: 'var(--space-md) 0' }}>Gider ödeme gününe yazılır ve Vergiler › Ödenenler'de görünür.</Rule>
      <AppSubmitButton label="Ödemeyi kaydet" icon="check" fullWidth onSubmit={() => {}} />
    </AppBottomSheet>
  );
}

// V6 · Vergi ekle — tür seçimi (ilk kurulumda çoklu)
const TAX_TYPES = [
  ['Bağkur', 'Her ay · ay sonu', 'Esnafın çoğu öder'],
  ['KDV', "Her ay · 28'i", 'Basit usuldeyseniz genelde yok'],
  ['Muhtasar ve prim hizmet', "Her ay · 26'sı", 'Çalışanınız varsa'],
  ['Geçici vergi', "Şub · May · Ağu · Kas · 17'si", 'Gerçek usuldeyseniz'],
  ['Yıllık gelir vergisi', 'Mart, Temmuz · ay sonu', 'Gerçek usuldeyseniz'],
  ['Emlak ve çevre temizlik', 'Mayıs, Kasım · ay sonu', 'Dükkânınız varsa; belediyeye'],
  ['Motorlu taşıtlar', 'Ocak, Temmuz · ay sonu', 'Aracınız varsa'],
  ['İlan-reklam (tabela)', 'Yılda bir · Ocak sonu', 'Tabelanız varsa; belediyeye'],
];
function TurSecimiScreen() {
  const [sel, setSel] = React.useState([0, 1, 3]);
  const { AppCard, AppSubmitButton } = DS;
  return (
    <Page title="Vergilerimi tanımla" footer={<AppSubmitButton label={sel.length + ' vergiyi ekle'} icon="check" fullWidth onSubmit={() => {}} />}>
      <p style={{ ...HELP, margin: '0 0 var(--space-md)', textWrap: 'pretty' }}>Ödediklerinizi seçin. Hazır tür yalnız ritmi ve günü önerir; tutarı ödediğinizde yazarsınız. Emin değilseniz muhasebecinize sorun.</p>
      <AppCard padding={0}>
        <RowList inset={48}>
          {TAX_TYPES.map(([n, r, h], i) => <CheckRow key={n} on={sel.includes(i)} onClick={() => setSel(sel.includes(i) ? sel.filter((x) => x !== i) : [...sel, i])} title={n} subtitle={r + ' · ' + h} />)}
        </RowList>
      </AppCard>
      <div style={{ marginTop: 'var(--space-sm)' }}><AppCard padding={0}><Row onClick={() => {}} lead={<Capsule icon="add" />} title="Kendi türüm" subtitle="Adını ve ritmini siz yazın" trailing={<DS.Icon name="chevron_right" size={20} color="var(--ink-muted)" />} /></AppCard></div>
    </Page>
  );
}

const RHYTHMS = [{ value: 'months', label: 'Seçilen aylarda' }, { value: 'month', label: 'Her ay' }, { value: 'q', label: 'Üç ayda bir' }, { value: 'y', label: 'Yılda bir' }];
// V7 · Vergi tanımı formu (Motorlu taşıtlar — kapsam seçeneği görünen tek tür ailesi)
function TanimFormScreen({ scroll = 0 }) {
  const { AppTextField, AppSelectField, AppFilterChips, AppSegmentedButton, AppSubmitButton } = DS;
  const [rh, setRh] = React.useState('months');
  const [scope, setScope] = React.useState('personal');
  return (
    <Page title="Vergi tanımı" scroll={scroll} footer={<AppSubmitButton label="Kaydet" icon="check" fullWidth onSubmit={() => {}} />}>
      <AppTextField label="Ad" value="Motorlu taşıtlar" helperText="Türden geldi; değiştirebilirsiniz." />
      <FieldLabel hint="Araç ve ev vergisinde şahsi seçilebilir.">Kapsam</FieldLabel>
      <AppSegmentedButton value={scope} onChange={setScope} options={[{ value: 'business', label: 'İşletme' }, { value: 'personal', label: 'Şahsi' }]} />
      <div style={{ marginTop: 'var(--space-lg)' }}><AppSelectField label="Ritim" value={rh} onChange={setRh} options={RHYTHMS} /></div>
      {rh === 'months' && <div style={{ marginTop: 'var(--space-sm)' }}><MonthChips selected={[0, 6]} /></div>}
      <div style={{ marginTop: 'var(--space-md)' }}><AppSelectField label="Gün" value="end" options={[{ value: 'end', label: 'Ay sonu' }, { value: '17', label: "17'si" }, { value: '26', label: "26'sı" }, { value: '28', label: "28'i" }]} helperText="Sıradaki: 31 Ocak 2027, sonra 31 Temmuz 2027" /></div>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-md)', marginTop: 'var(--space-md)' }}>
        <AppTextField label="Tutar (isteğe bağlı)" suffix="₺" helperText="Değişiyorsa boş bırakın; ödemede yazılır." />
        <AppSelectField label="Nereden ödenir (isteğe bağlı)" value="bonus" options={TAX_SOURCES} helperText="Ödediğinizde de seçebilirsiniz." />
        <AppTextField label="Başlangıç" value="31 Ocak 2027" prefixIcon="event" helperText="Varsayılan: sıradaki vade." />
      </div>
    </Page>
  );
}

// V8 · Bekleyen kalem ayrıntısı — başlık · tutar+durum · ayrıntı (tanım satırı bağlantı) · alt çubuk
function BekleyenSheet({ links = 'card' }) {
  const { AppBottomSheet, AppSubmitButton } = DS;
  return (
    <AppBottomSheet maxHeight="80%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <SheetHead icon="receipt_long" tone="expense" title="KDV · Ağustos" subtitle="Vergi · İşletme" />
      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', marginTop: 'var(--space-lg)' }}>
        <span style={{ flex: 1, font: '700 22px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>Tutar belli değil</span>
        <StatusTag label="1 gün gecikti" icon="error" tone="expense" />
      </div>
      <DetailBlock style={{ marginTop: 'var(--space-md)' }} rows={[['Vade', '28 Eylül Pazartesi'], ['Nereden ödenecek', 'Seçilmedi'], ...(links === 'inline' ? [['Tanım', "Her ay · 28'i", true]] : [])]} />
      {links === 'card' && <ActionCard><ActionRow icon="event_repeat" title="KDV ayrıntıları" subtitle="Her ay · 28'i" chevron /></ActionCard>}
      <SheetFooter>
        <AppSubmitButton label="Tutarı gir" icon="edit" variant="outlined" fullWidth onSubmit={() => {}} />
        <AppSubmitButton label="Ödedim" icon="check" fullWidth onSubmit={() => {}} />
      </SheetFooter>
    </AppBottomSheet>
  );
}

// V9 · Ödenmiş kalem / toplu ödeme / kapatılmış kalem. kind: 'single' | 'bulk' | 'closed'
function OdenmisSheet({ kind = 'single', confirm = false, links = 'card' }) {
  const { AppBottomSheet, AppSubmitButton, AppMoneyText, AppConfirmDialog } = DS;
  const bulk = kind === 'bulk', closed = kind === 'closed';
  const inl = links === 'inline';
  const link = closed ? ['Kapatan ödeme', '15 Eyl · Vergi ödemesi', true] : ['İşlem', 'İşlemlerde görüntüle', true];
  const base = closed
    ? [['Vade', '31 Ağustos'], ['Tanımdaki tutar', '₺8.950,00']]
    : bulk
      ? [['Ödeme günü', '15 Eylül Salı'], ['Nereden', 'Nakit kasa'], ['Not', 'Temmuz–Ağustos Bağkur']]
      : [['Ödeme günü', '31 Temmuz Cuma'], ['Nereden', 'Bonus · kart harcaması'], ['Vade', '31 Temmuz']];
  const rows = inl ? [...base, link] : base;
  const linkCard = inl ? null : <ActionCard>{closed ? <ActionRow icon="receipt_long" title="Kapatan ödemeyi aç" subtitle="15 Eyl · Vergi ödemesi" chevron /> : <ActionRow icon="swap_vert" title="İşlemlerde görüntüle" subtitle={bulk ? 'Nakit kasadan ödendi · 15 Eylül' : 'Bonus kartla ödendi · 31 Temmuz'} chevron />}</ActionCard>;
  return (
    <>
      <AppBottomSheet maxHeight="86%" style={{ paddingBottom: 'var(--space-lg)' }}>
        <SheetHead icon={bulk ? 'receipt_long' : closed ? 'health_and_safety' : 'directions_car'} tone={closed ? 'neutral' : 'expense'} title={bulk ? 'Vergi ödemesi' : closed ? 'Bağkur · Ağustos' : 'Motorlu taşıtlar · Temmuz'} subtitle={bulk ? 'Toplu ödeme · İşletme' : closed ? 'Vergi · İşletme' : 'Vergi · Şahsi'} />
        <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', marginTop: 'var(--space-lg)' }}>
          <span style={{ flex: 1 }}>{closed ? <span style={{ font: '700 22px/1.3 var(--font-core)', color: 'var(--ink)' }}>₺8.950,00</span> : <AppMoneyText amount={bulk ? '12500.0000' : '2180.0000'} effect="expense" signed size="metric" style={{ fontSize: 28 }} />}</span>
          {closed ? <StatusTag label="Kapatıldı" icon="task_alt" tone="neutral" /> : <StatusTag label="Ödendi" icon="check_circle" tone="income" />}
        </div>
        <DetailBlock style={{ marginTop: 'var(--space-md)' }} rows={rows} />
        {bulk && (
          <>
            <FieldLabel>Kapattığı kalemler</FieldLabel>
            <div style={{ border: '1px solid var(--border)', borderRadius: 'var(--radius-card)' }}>
              <RowList inset={16}>{['Bağkur · Temmuz', 'Bağkur · Ağustos'].map((t) => <Row key={t} title={t} subtitle="Kapatıldı" trailing={<DS.Icon name="task_alt" size={18} color="var(--neutral)" />} />)}</RowList>
            </div>
          </>
        )}
        {linkCard}
        {closed
          ? <Rule style={{ marginTop: 'var(--space-md)' }}>Geri alma kapatan ödemeden yapılır.</Rule>
          : <SheetFooter><AppSubmitButton label="Ödemeyi geri al" icon="undo" variant="outlined" fullWidth style={DANGER_OUTLINE} onSubmit={() => {}} /></SheetFooter>}
      </AppBottomSheet>
      {confirm && <Scrim><AppConfirmDialog icon="undo" destructive highlight={bulk ? 'Vergi ödemesi · ₺12.500,00' : 'Motorlu taşıtlar · ₺2.180,00'} message={bulk ? 'Gider iptal edilir; kapattığı 2 kalem yeniden bekler.' : 'Gider iptal edilir; kalem yeniden bekleyene döner.'} confirmLabel="Ödemeyi geri al" onConfirm={() => {}} onCancel={() => {}} /></Scrim>}
    </>
  );
}

// V10 · Vergi tanımı ayrıntısı
function TanimDetayScreen({ scroll = 0, overlay = null }) {
  const { AppCard, AppSubmitButton, AppMoneyText } = DS;
  return (
    <>
      <Page title="Bağkur" scroll={scroll} actions={<ToolButton icon="edit" label="Düzenle" />}>
        <DetailBlock rows={[['Ritim', 'Her ay · ay sonu'], ['Tutar', '₺8.950,00'], ['Nereden ödenir', 'Dükkan hesabı'], ['Kapsam', 'İşletme']]} style={{ background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 'var(--radius-card)' }} />
        <Section title="Sıradaki">
          <AppCard padding={0}>
            <RowList inset={76}>
              <Row lead={<DateLeaf day="30" month="Eyl" />} title="Eylül" subtitle={<StatusTag label="Yaklaşıyor · yarın" icon="schedule" tone="planned" />} trailing={<AppMoneyText amount="8950.0000" size="body" />} />
              <Row lead={<DateLeaf day="31" month="Eki" />} title="Ekim" subtitle="32 gün sonra" trailing={<AppMoneyText amount="8950.0000" size="body" />} />
              <Row lead={<DateLeaf day="30" month="Kas" />} title="Kasım" subtitle="62 gün sonra" trailing={<AppMoneyText amount="8950.0000" size="body" />} />
            </RowList>
          </AppCard>
        </Section>
        <Section title="Geçmiş ödemeler">
          <AppCard padding={0}>
            <RowList inset={76}>
              <Row lead={<DateLeaf day="31" month="Ağu" />} title="Ağustos" subtitle="Kapatıldı · 15 Eyl toplu ödemeyle" trailing={<DS.Icon name="task_alt" size={18} color="var(--neutral)" />} />
              <Row lead={<DateLeaf day="31" month="Tem" />} title="Temmuz" subtitle="Kapatıldı · 15 Eyl toplu ödemeyle" trailing={<DS.Icon name="task_alt" size={18} color="var(--neutral)" />} />
              <Row lead={<DateLeaf day="30" month="Haz" />} title="Haziran" subtitle="Ödendi 30 Haz · Dükkan hesabı" trailing={<AppMoneyText amount="8950.0000" effect="expense" signed size="body" />} />
            </RowList>
          </AppCard>
        </Section>
        <ActionCard style={{ marginTop: 'var(--space-lg)' }}>
          <ActionRow icon="pause_circle" title="Duraklat" subtitle="Yeni kalem üretmez; istediğinizde sürdürürsünüz." />
          <ActionRow icon="delete" danger disabled title="Sil" subtitle="Ödenmiş kalemleri olduğu için silinemez." />
        </ActionCard>
      </Page>
      {overlay}
    </>
  );
}

// Başka ekranlara dokunuşlar — kesitler
// Özet · Yaklaşanlar kartı — uygulamadaki zaman çizelgesi diline gecikenler eklendi.
const UPCOMING = [
  { day: '30', month: 'Eyl', title: 'Bağkur · Eylül', sub: 'Yarın · Vergi', amount: '8950.0000' },
  { day: '1', month: 'Eki', title: 'Dükkan kirası', sub: '2 gün sonra · Tekrarlanan', amount: '18500.0000' },
  { day: '5', month: 'Eki', title: 'Şahsi kart', sub: '6 gün sonra · Kart ekstresi', amount: '858.0000' },
  { day: '6', month: 'Eki', title: 'İşyeri elektrik faturası', sub: '7 gün sonra · Ödenecek', amount: '3950.8800' },
];
function TimelineRow({ x, first, last }) {
  const tone = x.late ? 'var(--expense)' : 'var(--ink)';
  return (
    <div style={{ display: 'grid', gridTemplateColumns: '40px 20px minmax(0,1fr) auto', columnGap: 12, alignItems: 'start', padding: '0 16px' }}>
      <div style={{ padding: '14px 0', textAlign: 'center', display: 'flex', flexDirection: 'column' }}>
        <span style={{ font: '700 20px/1.1 var(--font-core)', color: tone, fontFeatureSettings: '"tnum" 1' }}>{x.day}</span>
        <span style={{ font: '600 13px/1.3 var(--font-core)', color: x.late ? 'var(--expense)' : 'var(--ink-muted)' }}>{x.month}</span>
      </div>
      <div style={{ position: 'relative', alignSelf: 'stretch' }}>
        <span style={{ position: 'absolute', left: 9, width: 2, top: first ? 24 : 0, bottom: last ? 'calc(100% - 24px)' : 0, background: 'var(--border)' }}></span>
        <span style={{ position: 'absolute', left: 4, top: 18, width: 12, height: 12, boxSizing: 'border-box', borderRadius: 999, border: '2px solid ' + tone, background: x.late ? 'var(--expense)' : 'var(--surface-card)' }}></span>
      </div>
      <div style={{ padding: '14px 0', display: 'flex', flexDirection: 'column', gap: 3 }}>
        <span style={ROWT}>{x.title}</span>
        <span style={{ ...HELP, color: x.late ? 'var(--expense)' : 'var(--ink-muted)' }}>{x.sub}</span>
      </div>
      <div style={{ padding: '14px 0' }}><DS.AppMoneyText amount={x.amount} effect="expense" size="row" /></div>
    </div>
  );
}
function SnipYaklasan() {
  const { AppCard, AppMoneyText } = DS;
  const foot = { display: 'flex', alignItems: 'baseline', justifyContent: 'space-between', gap: 8 };
  return (
    <div style={{ padding: 'var(--space-md)' }}>
      <DS.AppSectionHeader title="Yaklaşanlar" trailing={<TextAction label="7 gün" trailingIcon="chevron_right" />} />
      <AppCard padding={0}>
        <button type="button" style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: 64, padding: '12px 16px', border: 'none', borderBottom: '1px solid var(--border)', background: 'var(--expense-container)', textAlign: 'left', cursor: 'pointer', font: 'inherit', boxSizing: 'border-box' }}>
          <DS.Icon name="error" size={22} color="var(--on-expense-container)" />
          <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
            <span style={{ ...ROWT, color: 'var(--on-expense-container)' }}>7 gecikmiş ödeme</span>
            <span style={{ ...HELP, color: 'var(--on-expense-container)' }}>En eskisi 12 Ağustos</span>
          </span>
          <AppMoneyText amount="41280.0000" size="row" style={{ color: 'var(--on-expense-container)' }} />
        </button>
        <div style={{ padding: '8px 0' }}>{UPCOMING.map((x, i) => <TimelineRow key={x.title} x={x} first={i === 0} last={i === UPCOMING.length - 1} />)}</div>
        <div style={{ padding: '14px 16px', borderTop: '1px solid var(--border)', background: 'var(--surface-card-muted)' }}>
          <div style={foot}><span style={{ font: '500 14px/1.3 var(--font-core)', color: 'var(--ink)' }}>7 günde çıkacak</span><AppMoneyText amount="32258.8800" effect="expense" size="body" style={{ fontWeight: 600 }} /></div>
        </div>
      </AppCard>
    </div>
  );
}
function SnipKategori() {
  const [on, setOn] = React.useState(true);
  return (
    <div style={{ padding: 'var(--space-md)' }}>
      <DS.AppTextField label="Kategori adı" value="Vergi ve harç" />
      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', marginTop: 'var(--space-md)', padding: '12px 16px', border: '1px solid var(--border)', borderRadius: 'var(--radius-card)', background: 'var(--surface-card)' }}>
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}><span style={ROWT}>Vergi</span><span style={{ ...HELP, textWrap: 'pretty' }}>Bu kategorideki giderler Vergiler › Ödenenler'de görünür.</span></span>
        <DS.AppSwitch value={on} onChange={setOn} label="Vergi" />
      </div>
    </div>
  );
}
function SnipPlan() {
  const [v, setV] = React.useState('months');
  return (
    <div style={{ padding: 'var(--space-md)' }}>
      <DS.AppSelectField label="Sıklık" value={v} onChange={setV} options={[{ value: 'months', label: 'Seçilen aylarda' }, { value: 'd', label: 'Günlük' }, { value: 'w', label: 'Haftalık' }, { value: 'm', label: 'Aylık' }, { value: 'q', label: 'Üç ayda bir' }, { value: 'y', label: 'Yılda bir' }]} />
      {v === 'months' && <div style={{ marginTop: 'var(--space-sm)' }}><MonthChips selected={[4, 10]} /></div>}
    </div>
  );
}

Object.assign(window, { VergilerScreen, OdedimSheet, OdemeEkleSheet, TurSecimiScreen, TanimFormScreen, BekleyenSheet, OdenmisSheet, TanimDetayScreen, SnipYaklasan, SnipKategori, SnipPlan });
