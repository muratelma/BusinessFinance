// Brif 6 · Hesap sayfası (H), ayrıntı, form ve Hesaplar listesi (D).
// Satır yönü: gelir/gider yeşil/kırmızı; para taşıyan satır mavi ama işaretli (+ / −) — yön işaretten okunur, renkten değil.
const mv = (o) => ({ cap: 'nt', tone: 'nt', ...o });
const ct = (o) => ({ cap: o.amount < 0 ? 'out' : 'in', tone: o.amount < 0 ? 'out' : 'in', ...o });
const ZR = {
  isletim: ct({ icon: 'percent', title: 'Hesap işletim ücreti', sub: 'Banka ve POS komisyonu', amount: -200 }),
  cari: mv({ icon: 'storefront', title: 'Test Toptancı', sub: 'Cari ödeme', amount: -2500 }),
  kart: mv({ icon: 'credit_card', title: 'Kart ödemesi', sub: 'Bonus Kart', amount: -3000 }),
  trOut: mv({ icon: 'swap_horiz', title: 'Transfer', sub: '→ Garanti Vadesiz', amount: -10050, under: '₺50,00 masraf dahil' }),
  elek: mv({ icon: 'receipt_long', title: 'Elektrik faturası', sub: 'Borç ödemesi', amount: -1110 }),
  ahmet: mv({ icon: 'request_quote', title: 'Ahmet Usta', sub: 'Borç taksidi ödemesi', amount: -1000, under: '₺150,00 faiz dahil' }),
  trIn: mv({ icon: 'swap_horiz', title: 'Transfer', sub: '← Dükkan Kasası', amount: 12000 }),
  pos15: mv({ icon: 'move_to_inbox', title: 'POS yatışı', sub: 'Ziraat POS · 14 Eki satışı', amount: 7169, under: '₺40,00 kesinti sonrası' }),
  web: ct({ icon: 'handyman', title: 'Web sitesi işi', sub: 'Hizmet geliri', amount: 3000 }),
  pos7: mv({ icon: 'move_to_inbox', title: 'POS yatışı', sub: 'Ziraat POS · 6 Eki satışı', amount: 5767 }),
  ornek: mv({ icon: 'request_quote', title: 'Örnek Banka', sub: 'Borç taksidi ödemesi', amount: -3000, under: '₺450,00 faiz dahil' }),
  topt: ct({ icon: 'inventory_2', title: 'Toptancı ödemesi', sub: 'Ticari mal alımı', amount: -4200 }),
  kira: ct({ icon: 'storefront', title: 'Ekim kirası', sub: 'İşyeri kirası', amount: -6500 }),
};
const ZDAYS = [['28 Ekim', ['isletim']], ['26 Ekim', ['cari']], ['24 Ekim', ['kart']], ['21 Ekim', ['trOut']], ['20 Ekim', ['elek', 'ahmet']], ['16 Ekim', ['trIn']], ['15 Ekim', ['pos15']], ['10 Ekim', ['web']], ['7 Ekim', ['pos7']], ['5 Ekim', ['ornek', 'topt']], ['1 Ekim', ['kira']]];
const zdays = (f = () => true) => ZDAYS.map(([d, ks]) => [d, ks.map((k) => ZR[k]).filter(f)]).filter(([, r]) => r.length);

const M_TUMU = [{ label: 'Hepsi' }, { group: 'Giren' }, { icon: 'category', label: 'Kategori', amount: 3000, sign: true, tone: 'in' }, { icon: 'call_received', label: 'Tahsilat', amount: 12936, sign: true, tone: 'nt' }, { icon: 'swap_horiz', label: 'Transfer', amount: 12000, sign: true, tone: 'nt' }, { group: 'Çıkan' }, { icon: 'category', label: 'Kategori', amount: -10900, sign: true, tone: 'out' }, { icon: 'call_made', label: 'Ödeme', amount: -10610, sign: true, tone: 'nt' }, { icon: 'swap_horiz', label: 'Transfer', amount: -10050, sign: true, tone: 'nt' }];
const M_GIREN = [{ label: 'Hepsi', amount: 27936, sign: true }, { group: 'Kategori' }, { icon: 'handyman', label: 'Hizmet geliri', amount: 3000, sign: true, tone: 'in' }, { group: 'Tahsilat' }, { icon: 'move_to_inbox', label: 'POS yatışı', amount: 12936, sign: true, tone: 'nt' }, { group: 'Transfer' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 12000, sign: true, tone: 'nt' }];
const M_CIKAN = [{ label: 'Hepsi', amount: -31560, sign: true }, { group: 'Kategori' }, { icon: 'storefront', label: 'İşyeri kirası', amount: -6500, sign: true, tone: 'out' }, { icon: 'inventory_2', label: 'Ticari mal alımı', amount: -4200, sign: true, tone: 'out' }, { icon: 'percent', label: 'Banka ve POS komisyonu', amount: -200, sign: true, tone: 'out' }, { group: 'Ödeme' }, { icon: 'request_quote', label: 'Borç taksidi ödemesi', amount: -4000, sign: true, tone: 'nt' }, { icon: 'credit_card', label: 'Kart ödemesi', amount: -3000, sign: true, tone: 'nt' }, { icon: 'groups', label: 'Cari ödeme', amount: -2500, sign: true, tone: 'nt' }, { icon: 'receipt_long', label: 'Borç ödemesi', amount: -1110, sign: true, tone: 'nt' }, { group: 'Transfer' }, { icon: 'account_balance', label: 'Garanti Vadesiz', amount: -10050, sign: true, tone: 'nt' }];

function BalCard({ bal = 21376, yolda, past, m = 'Ekim', g = 27936, c = 31560 }) {
  const n = g - c;
  const col = (l, v, tone, sign) => <div style={{ flex: 1, minWidth: 0 }}><div style={small9}>{m} {l}</div><Money9 v={v} tone={tone} sign={sign} size={15} /></div>;
  return (
    <Card6 style={{ padding: '16px 20px', marginTop: 8 }}>
      {past ? <><div style={sub9}>{m} sonu bakiye</div><div style={{ margin: '4px 0 2px' }}><Money9 v={past[0]} size={28} w={700} /></div><div style={sub9}>Bugün {fmt6(past[1])}</div></>
        : <><div style={sub9}>Bakiye</div><div style={{ margin: '4px 0 2px' }}><Money9 v={bal} size={28} w={700} /></div>{yolda && <div style={{ ...sub9, display: 'flex', alignItems: 'center', gap: 4 }}><I6 n="schedule" s={16} style={{ color: 'var(--neutral)' }} />Yolda {fmt6(yolda)}</div>}</>}
      <div style={{ display: 'flex', gap: 8, borderTop: '1px solid var(--border)', marginTop: 12, paddingTop: 12, flexWrap: 'wrap' }}>{col('giren', g, 'in')}{col('çıkan', c, 'out')}<div style={{ flex: 1, minWidth: 0 }}><div style={small9}>Net</div><Money9 v={n} sign size={15} /></div></div>
    </Card6>
  );
}
// Yolda — mavi kutu, bakiye kartının altında: başlık + toplam, en yakın tek satış, fazlası "Tümünü gör".
// Tutar hesaba geçecek tutardır (komisyon düşülmüş); altında "₺326,00 komisyon sonrası" — hesap listesindeki "kesinti sonrası" kuralı.
const YROWS1 = [['25 Ekim', 8400, 326]];
const YROWS4 = [['25 Ekim', 8400, 326], ['24 Ekim', 9200, 357], ['24 Ekim', 6100, 237], ['23 Ekim', 5650, 130]];
const ytot = (rows) => rows.reduce((t, [, v, k]) => t + v - k, 0);
const yTx = ([, v, k]) => ({ icon: 'point_of_sale', cap: 'nt', tone: 'nt', sign: true, title: 'Kartlı satış', sub: '26 Eki bekleniyor', amount: v - k, under: fmt6(k) + ' komisyon sonrası' });
function Strip9({ rows = YROWS1 }) {
  const t = yTx(rows[0]), on = 'var(--on-neutral-container)';
  return <div style={{ margin: '12px 16px 0', padding: rows.length > 1 ? '12px 12px 0' : 12, borderRadius: 20, background: 'var(--neutral-container)' }}>
    <div style={{ display: 'flex', alignItems: 'baseline', gap: 8, padding: '2px 4px 10px' }}><span style={{ flex: 1, font: '600 16px/1.3 var(--font-core)', color: on }}>Yolda</span><Money9 v={ytot(rows)} size={16} w={700} style={{ color: on }} /></div>
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px 14px', borderRadius: 16, background: 'var(--surface-card)' }}>
      <div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{t.title}</div><div style={{ ...sub9, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{t.sub}</div></div>
      <div style={{ flex: 'none', textAlign: 'right' }}><Money9 v={t.amount} tone="nt" sign /><div style={{ ...small9, whiteSpace: 'nowrap' }}>{t.under}</div></div>
    </div>
    {rows.length > 1 && <button style={{ width: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 2, height: 44, border: 0, background: 'transparent', font: '600 14px/1 var(--font-core)', color: on, cursor: 'pointer' }}>Tümünü gör · {rows.length} satış<I6 n="chevron_right" s={20} /></button>}
  </div>;
}
const SelHead = ({ l, v }) => <div style={{ margin: '16px 20px 0', display: 'flex', alignItems: 'baseline', gap: 8, font: '600 16px/1.3 var(--font-core)' }}><span style={{ flex: 1, minWidth: 0 }}>{l}</span><Money9 v={v} sign tone={v < 0 ? undefined : undefined} /></div>;
const EditBtn = <button className="iconbtn" aria-label="Hesap ayarları" style={{ flex: 'none' }}><I6 n="tune" s={22} /></button>;

function Hesap9({ title = 'Ziraat Vadesiz', month, dir = 'Tümü', tur, open, menu, card, strip, sel, days, empty, overlay, edit = true }) {
  return (
    <Screen9 overlay={overlay}>
      <Head9 title={title} month={month} extra={edit && EditBtn} />
      <Chips9 menu={menu}>{['Tümü', 'Giren', 'Çıkan'].map((d) => <Chip9 key={d} label={d} on={dir === d} />)}<Chip9 label="Tür" menu value={tur} open={open} /></Chips9>
      <Body9>
        <BalCard {...card} />
        {strip && <Strip9 {...strip} />}
        {sel && <SelHead {...sel} />}
        {empty || <Days9 days={days} flat />}
      </Body9>
    </Screen9>
  );
}

// ---------- Ayrıntı panelleri ----------
const DetHead = ({ icon, title, sub, v, tone = 'nt' }) => <>
  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}><Cap6 n={icon} tone={tone} /><span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: '600 18px/1.3 var(--font-core)' }}>{title}</span><span style={small9}>{sub}</span></span><button className="iconbtn" aria-label="Kapat" style={{ marginRight: -12 }}><I6 n="close" s={22} /></button></div>
  <div style={{ display: 'flex', alignItems: 'center', gap: 8, margin: '12px 0' }}><span style={{ flex: 1 }}><Money9 v={v} tone={tone} size={28} w={700} /></span><Tag9 kind="paid" label="Gerçekleşti" /></div>
</>;
const Note9 = ({ children }) => <div style={{ ...small9, margin: '12px 4px', textWrap: 'pretty' }}>{children}</div>;
const Gap = () => <div style={{ height: 8 }}></div>;
function TransferDet() {
  return <Sheet9><DetHead icon="swap_horiz" title="Transfer" sub="21 Ekim 2026 Çarşamba" v={10000} />
    <KV9 rows={[['Çıkan hesap', 'Ziraat Vadesiz'], ['Giren hesap', 'Garanti Vadesiz'], ['Masraf', '−₺50,00', 'out'], ['Masrafın kategorisi', 'Banka ve POS komisyonu']]} /><Gap />
    <KV9 rows={[['Ziraat Vadesiz', '₺27.076,00'], ['Garanti Vadesiz', '₺14.500,00']]} />
    <Note9>İşlemden sonraki bakiyeler. Masraf gider yazılır ve transferle birlikte iptal olur.</Note9>
    <Btn9 label="Hareketi iptal et" icon="block" danger /></Sheet9>;
}
function YatisDet({ many }) {
  const sales = many ? [['4 Eki · Kartlı satış', '₺2.000,00'], ['5 Eki · Kartlı satış', '₺2.500,00'], ['6 Eki · Kartlı satış', '₺1.500,00']] : [['14 Eki · Kartlı satış', '₺7.500,00']];
  const cut = many ? [['Komisyon', '−₺233,00', 'out']] : [['Komisyon', '−₺291,00', 'out'], ['Kesinti', '−₺40,00', 'out']];
  return <Sheet9><DetHead icon="move_to_inbox" title="POS yatışı" sub={(many ? '7 Ekim 2026 Çarşamba' : '15 Ekim 2026 Perşembe') + ' · Ziraat POS'} v={many ? 5767 : 7169} />
    <KV9 rows={[['Paranın geçtiği hesap', 'Ziraat Vadesiz']]} /><Gap />
    <div style={{ ...small9, font: '600 13px/1 var(--font-core)', margin: '8px 4px' }}>Kapattığı {many ? '3 satış' : 'satış'}</div>
    <KV9 rows={[...sales, ...cut, ['Hesaba geçen', many ? '₺5.767,00' : '₺7.169,00', null, true]]} />
    <Note9>Satış, yapıldığı gün gelir yazıldı. Komisyon{many ? '' : ' ve kesinti'} gider yazılır.</Note9>
    <Btn9 label="Yatışı geri al" icon="undo" danger /></Sheet9>;
}
function YoldaSayfa({ many }) {
  const rows = many ? YROWS4 : YROWS1, days = [];
  rows.forEach((x) => { let g = days.find((d) => d[0] === x[0]); if (!g) days.push(g = [x[0], []]); g[1].push(yTx(x)); });
  return <Screen9><Head9 title="Yolda" month={null} /><Body9>
    <Sum9 label="Ziraat Vadesiz'e geçecek" v={ytot(rows)} tone="nt" foot={rows.length + ' kartlı satış · komisyon sonrası'} />
    <Days9 days={days} flat />
    <div style={{ ...small9, margin: '12px 20px 0', textWrap: 'pretty' }}>Para hesaba geçince satış buradan çıkar; hesapta POS yatışı olarak görünür.</div>
  </Body9></Screen9>;
}
// ---------- Form ----------
const Field = ({ l, v, ph, focus }) => <div style={{ padding: '10px 16px', minHeight: 56, border: (focus ? 2 : 1) + 'px solid ' + (focus ? 'var(--brand)' : 'var(--border-strong)'), borderRadius: 16, background: 'var(--surface-card)' }}><div style={small9}>{l}</div><div style={{ font: '400 16px/1.4 var(--font-core)', color: v ? 'var(--ink)' : 'var(--ink-faint)', fontVariantNumeric: 'tabular-nums' }}>{v || ph}</div></div>;
const ACC9 = [{ value: 'ziraat', label: 'Ziraat Vadesiz' }, { value: 'garanti', label: 'Garanti Vadesiz' }, { value: 'kasa', label: 'Dükkan Kasası' }];
function TransferForm({ fee }) {
  const { AppTextField, AppSelectField } = window.DesignSystem_02d8cd;
  return <Screen9><Head9 title="Yeni transfer" month={null} />
    <div className="body" style={{ padding: '12px 16px 24px', display: 'flex', flexDirection: 'column', gap: 'var(--space-md)' }}>
      <AppTextField label="Tarih" value="21 Ekim 2026" prefixIcon="event" />
      <AppSelectField label="Çıkan hesap" value="ziraat" options={ACC9} onChange={() => {}} />
      <AppSelectField label="Giren hesap" value="garanti" options={ACC9} onChange={() => {}} />
      <AppTextField label="Tutar" value="₺10.000,00" />
      <AppTextField label="Masraf (isteğe bağlı)" value={fee ? '₺50,00' : ''} helperText={fee ? undefined : 'Bankanın aldığı ücret. Çıkan hesaptan düşer.'} />
      {fee && <div style={{ padding: '10px 14px', borderRadius: 16, background: 'var(--neutral-container)', color: 'var(--on-neutral-container)', font: '400 14px/1.4 var(--font-core)', display: 'flex', gap: 8 }}><I6 n="info" s={18} /><span>Ziraat Vadesiz'den toplam <b>₺10.050,00</b> çıkar. Masraf Banka ve POS komisyonu olarak gider yazılır.</span></div>}
      <AppTextField label="Açıklama (isteğe bağlı)" value="" />
    </div>
    <div style={{ flex: 'none', padding: '12px 16px 20px', borderTop: '1px solid var(--border)', background: 'var(--surface-canvas)' }}><Btn9 label="Kaydet" kind="filled" /></div>
  </Screen9>;
}
// ---------- İşlemler satırı (D4) ----------
function IslemlerD4() {
  return <Screen9><Head9 title="İşlemler" back={false} month={null} /><Body9><Days9 flat days={[['21 Ekim', [{ icon: 'swap_horiz', cap: 'nt', tone: 'nt', sign: false, title: 'Transfer', sub: 'Ziraat Vadesiz → Garanti Vadesiz', amount: 10000, under: '+ ₺50,00 masraf' }]], ['20 Ekim', [{ icon: 'request_quote', cap: 'nt', tone: 'nt', sign: false, title: 'Ahmet Usta', sub: 'Borç taksidi ödemesi · Ziraat Vadesiz', amount: 1000, under: '₺150,00 faiz dahil' }]]]} /></Body9></Screen9>;
}
// ---------- Hesaplar ----------
function AccRow({ icon, name, sub, v, first }) {
  return <div style={{ display: 'flex', alignItems: 'center', gap: 14, padding: '12px 12px 12px 16px', borderTop: first ? 0 : '1px solid var(--border)', cursor: 'pointer' }}>
    <Cap6 n={icon} tone="mu" />
    <div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{name}</div><div style={sub9}>{sub}</div></div>
    <Money9 v={v} />
    <I6 n="chevron_right" style={{ color: 'var(--ink-muted)' }} />
  </div>;
}
const AddBtn = <button className="iconbtn" aria-label="Hesap ekle" title="Hesap ekle" style={{ flex: 'none' }}><I6 n="add" s={24} /></button>;
function Hesaplar9({ empty }) {
  const G = ({ l, v, children }) => <><div style={{ margin: '20px 20px 8px', display: 'flex', alignItems: 'baseline', gap: 8, font: '600 13px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' }}><span style={{ flex: 1 }}>{l}</span><Money9 v={v} size={13} style={{ color: 'var(--ink-muted)' }} /></div><Card6 style={{ marginTop: 0, overflow: 'hidden' }}>{children}</Card6></>;
  const col = (l, v) => <div style={{ flex: 1, minWidth: 0 }}><div style={small9}>{l}</div><Money9 v={v} size={15} /></div>;
  return <Screen9><Head9 title="Hesaplar" month={null} extra={!empty && AddBtn} /><Body9>
    {empty ? <><Empty9 title="Henüz hesap yok" text="Kasanızı ya da banka hesabınızı ekleyerek başlayabilirsiniz." /><div style={{ padding: '0 16px' }}><Btn9 label="Hesap ekle" icon="add" kind="filled" /></div></> : <>
      <Card6 style={{ padding: '16px 20px', marginTop: 8 }}>
        <div style={sub9}>Toplam bakiye</div><div style={{ margin: '4px 0 0' }}><Money9 v={48330} size={28} w={700} /></div>
        <div style={{ display: 'flex', gap: 8, borderTop: '1px solid var(--border)', marginTop: 12, paddingTop: 12 }}>{col('Nakit', 12454)}{col('Banka', 35876)}</div>
      </Card6>
      <div style={{ padding: '12px 16px 0' }}><Btn9 label="Transfer yap" icon="swap_horiz" /></div>
      <G l="Nakit" v={12454}><AccRow first icon="payments" name="Dükkan Kasası" sub="İşletme" v={8454} /><AccRow icon="payments" name="Cüzdan" sub="Şahsi" v={4000} /></G>
      <G l="Banka" v={35876}><AccRow first icon="account_balance" name="Ziraat Vadesiz" sub="İşletme" v={21376} /><AccRow icon="account_balance" name="Garanti Vadesiz" sub="İşletme" v={14500} /></G>
      <Card6 style={{ marginTop: 20, overflow: 'hidden' }}><button style={{ width: '100%', display: 'flex', alignItems: 'center', gap: 14, minHeight: 56, padding: '8px 12px 8px 16px', border: 0, background: 'transparent', cursor: 'pointer', textAlign: 'left', font: 'inherit', color: 'var(--ink)' }}><Cap6 n="inventory" tone="mu" /><span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: '600 16px/1.3 var(--font-core)' }}>Pasif hesaplar · 1</span><span style={{ display: 'block', ...sub9 }}>Toplama girmez</span></span><I6 n="chevron_right" style={{ color: 'var(--ink-muted)' }} /></button></Card6>
    </>}
  </Body9></Screen9>;
}

function renderHesap(R) {
  const card = {}, strip = { rows: YROWS4 };
  const all = zdays();
  R('H1', <Hesap9 card={card} strip={strip} days={all} />);
  R('H2', <Hesap9 dir="Giren" card={{}} strip={strip} days={zdays((r) => r.amount > 0)} />);
  R('H3', <Hesap9 dir="Çıkan" card={{}} days={zdays((r) => r.amount < 0)} />);
  R('H4', <Hesap9 card={{}} strip={strip} days={all} open menu={<Menu9 caption="Ekim · Ziraat Vadesiz" items={M_TUMU} width={320} />} />);
  R('H5', <Hesap9 dir="Giren" card={{}} strip={strip} days={zdays((r) => r.amount > 0)} open menu={<Menu9 caption="Ekim giren · Ziraat Vadesiz" items={M_GIREN} width={320} />} />);
  R('H6', <Hesap9 dir="Çıkan" card={{}} days={zdays((r) => r.amount < 0)} open menu={<Menu9 caption="Ekim çıkan · Ziraat Vadesiz" items={M_CIKAN} width={340} maxH={600} />} />);
  R('H7', <Hesap9 dir="Çıkan" tur="Borç taksidi ödemesi" card={{}} sel={{ l: 'Borç taksidi ödemesi', v: -4000 }} days={zdays((r) => r.sub === 'Borç taksidi ödemesi')} />);
  R('H8', <Hesap9 dir="Çıkan" tur="Kategori" card={{}} sel={{ l: 'Kategori', v: -10900 }} days={zdays((r) => r.amount < 0 && r.cap === 'out')} />);
  R('H9', <Hesap9 month="Eylül 2026" card={{ past: [25000, 21376], m: 'Eylül', g: 31200, c: 28400 }} days={[['28 Eylül', [mv({ icon: 'swap_horiz', title: 'Transfer', sub: '→ Garanti Vadesiz', amount: -12000 })]], ['22 Eylül', [mv({ icon: 'move_to_inbox', title: 'POS yatışı', sub: 'Ziraat POS · 4 satış · 15–19 Eyl', amount: 18200 })]], ['15 Eylül', [mv({ icon: 'swap_horiz', title: 'Transfer', sub: '← Dükkan Kasası', amount: 13000 }), ct({ icon: 'inventory_2', title: 'Toptancı ödemesi', sub: 'Ticari mal alımı', amount: -9900 })]], ['1 Eylül', [ct({ icon: 'storefront', title: 'Eylül kirası', sub: 'İşyeri kirası', amount: -6500 })]]]} />);
  R('H10', <YoldaSayfa />);
  R('H10b', <YoldaSayfa many />);
  R('H11', <Hesap9 title="Dükkan Kasası" card={{ bal: 8454, g: 14800, c: 18950 }} days={[['25 Ekim', [ct({ icon: 'payments', title: 'Nakit satış · Gün sonu', sub: 'Satış geliri', amount: 5300 })]], ['18 Ekim', [ct({ icon: 'difference', title: 'Kasa farkı', sub: 'Kasa farkı', amount: -150 })]], ['16 Ekim', [mv({ icon: 'swap_horiz', title: 'Transfer', sub: '→ Ziraat Vadesiz', amount: -12000 })]], ['15 Ekim', [ct({ icon: 'badge', title: 'Personel ücreti', sub: 'Personel giderleri', amount: -4200 })]], ['14 Ekim', [ct({ icon: 'payments', title: 'Nakit satış · Gün sonu', sub: 'Satış geliri', amount: 4900 })]], ['6 Ekim', [ct({ icon: 'payments', title: 'Nakit satış · Gün sonu', sub: 'Satış geliri', amount: 4600 })]], ['2 Ekim', [ct({ icon: 'inventory_2', title: 'Hal alışverişi', sub: 'Ticari mal alımı', amount: -2600 })]]]} />);
  R('H12', <Hesap9 title="Garanti Vadesiz" month="Eylül 2026" card={{ past: [4500, 14500], m: 'Eylül', g: 0, c: 0 }} empty={<Empty9 title="Eylül'de bu hesapta hareket yok" text="Başka bir ay seçebilirsiniz." />} />);
  R('H15', <Hesap9 card={{}} strip={strip} days={all} overlay={<MonthDlg9 caption="Ziraat Vadesiz · ay sonu bakiye" values={[14200, 16850, 15300, 18900, 17400, 20100, 19650, 22300, 25000, 21376, 0, 0]} bar="var(--ink-muted)" />} />);
  R('H13', <Hesap9 title="Türkiye İş Bankası Ticari Vadesiz" card={{ bal: 12845320.75, g: 2450000, c: 1284650.4 }} days={[['20 Ekim', [mv({ icon: 'receipt_long', title: 'Örnek Elektrik Dağıtım A.Ş.', sub: 'Borç ödemesi', amount: -1284650.4, under: '₺12.480,00 masraf dahil' })]], ['12 Ekim', [mv({ icon: 'request_quote', title: 'Kemal Bey', sub: 'Alacak taksidi tahsilatı', amount: 2000 })]], ['5 Ekim', [mv({ icon: 'swap_horiz', title: 'Transfer', sub: '← Ziraat Vadesiz', amount: 2448000 })]]]} />);

  R('D1', <Hesap9 card={{}} strip={strip} days={all} overlay={<TransferDet />} />);
  R('D2', <Hesap9 card={{}} strip={strip} days={all} overlay={<YatisDet />} />);
  R('D2b', <Hesap9 card={{}} strip={strip} days={all} overlay={<YatisDet many />} />);
  R('D3', <TransferForm />);
  R('D3b', <TransferForm fee />);
  R('D4', <IslemlerD4 />);
  R('D5', <Hesaplar9 />);
  R('D6', <Hesaplar9 empty />);
}
Object.assign(window, { renderHesap, Hesap9, Hesaplar9 });
