// Brif 5 · Kategori özeti (A) ve kategori sayfası (B).
const KG = { // gider, İşletme, Ekim
  all: [['inventory_2', 'Ticari mal alımı', 12480, 46], ['storefront', 'İşyeri kirası', 6500, 24], ['badge', 'Personel giderleri', 4200, 16], ['receipt_long', 'İşyeri faturaları', 1860, 7], ['percent', 'Banka ve POS komisyonu', 1140, 4], ['request_quote', 'Faiz ve finansman gideri', 600, 2], ['difference', 'Kasa farkı', 150, 1]],
  ziraat: [['storefront', 'İşyeri kirası', 6500, 52], ['inventory_2', 'Ticari mal alımı', 4200, 34], ['percent', 'Banka ve POS komisyonu', 1140, 9], ['request_quote', 'Faiz ve finansman gideri', 600, 5]],
  vadeli: [['inventory_2', 'Ticari mal alımı', 2500, 69], ['receipt_long', 'İşyeri faturaları', 1110, 31]],
};
const KI = [['sell', 'Satış geliri', 41200, 92], ['handyman', 'Hizmet geliri', 3000, 7], ['savings', 'Faiz geliri', 350, 1]];
const KS = [['home', 'Kira', 7500, 41], ['shopping_basket', 'Market', 6200, 34], ['receipt_long', 'Faturalar', 2050, 11], ['directions_car', 'Ulaşım', 1800, 10], ['local_hospital', 'Sağlık', 800, 4]];
const MG = [{ label: 'Hepsi', amount: 26930 }, { group: 'Nakit ve banka' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 6950 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 12440 }, { group: 'Kredi kartı' }, { icon: 'credit_card', label: 'Bonus Kart', amount: 3930 }, { group: 'Hesaptan geçmeyen' }, { icon: 'event_upcoming', label: 'Vadeli', sub: 'Vadeli alım, fatura ve borçla alınanlar', amount: 3610 }];
const MI = [{ label: 'Hepsi', amount: 44550 }, { group: 'Nakit ve banka' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 15150 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 24900 }, { group: 'Hesaptan geçmeyen' }, { icon: 'event_upcoming', label: 'Vadeli', sub: 'Veresiye satış ve alacaklar', amount: 4500 }];
const MCOK = [{ label: 'Hepsi', amount: 1404650.4 }, { group: 'Nakit ve banka' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 98400 }, { icon: 'payments', label: 'Şube 2 kasası', amount: 41250 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 512560 }, { icon: 'account_balance', label: 'Türkiye İş Bankası Ticari Vadesiz', amount: 384200.4 }, { icon: 'account_balance', label: 'Vakıfbank POS hesabı', amount: 96000 }, { group: 'Kredi kartı' }, { icon: 'credit_card', label: 'Bonus Kart', amount: 93930 }, { icon: 'credit_card', label: 'Axess Business', amount: 58310 }, { group: 'Hesaptan geçmeyen' }, { icon: 'event_upcoming', label: 'Vadeli', sub: 'Vadeli alım, fatura ve borçla alınanlar', amount: 120000 }];

function CatRow({ c, dir, first }) {
  const [ic, t, v, p, none] = c;
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '14px 12px 14px 16px', borderTop: first ? 0 : '1px solid var(--border)', cursor: 'pointer' }}>
      <Cap6 n={ic} tone={none ? 'mu' : dir === 'Gelir' ? 'in' : 'out'} />
      <div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>{t}</div><div style={sub9}>{none ? 'Pay %' + p + ' · kategori seçilmeli' : 'Pay %' + p}</div></div>
      <div style={{ flex: 'none', width: 112, display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: 6 }}><Money9 v={v} /><div style={{ width: 96, height: 6, borderRadius: 999, background: 'var(--surface-card-muted)' }}><div style={{ width: Math.max(3, p) + '%', height: '100%', borderRadius: 999, background: none ? 'var(--ink-faint)' : dir === 'Gelir' ? 'var(--income-fill)' : 'var(--expense-fill)' }}></div></div></div>
      <I6 n="chevron_right" style={{ color: 'var(--ink-muted)' }} />
    </div>
  );
}

function KatOzet9({ dir = 'Gider', scope = true, hesap, menu, label, total, count, cats, noneCat, empty, overlay, faint, month }) {
  return (
    <Screen9 overlay={overlay}>
      <Head9 title="Kategori özeti" month={month} />
      <Seg9 v={dir} />
      <Chips9 menu={menu}>{scope && <><Chip9 label="Hepsi" /><Chip9 label="İşletme" on /><Chip9 label="Şahsi" /></>}<Chip9 label="Hesap / kart" menu value={hesap} open={!!menu} faint={faint} /></Chips9>
      <Body9>
        <Sum9 label={label} v={total} foot={count} />
        {empty ? empty : <Card6 style={{ overflow: 'hidden' }}>{cats.map((c, i) => <CatRow key={c[1]} c={c} dir={dir} first={!i} />)}</Card6>}
        {noneCat && <><div style={{ margin: '16px 20px 8px', ...small9 }}>Bu tutarın kategorisi bulunamadı</div><Card6 style={{ marginTop: 0, overflow: 'hidden', borderStyle: 'dashed', borderColor: 'var(--border-strong)' }}><CatRow c={noneCat} dir={dir} first /></Card6></>}
      </Body9>
    </Screen9>
  );
}

// ---------- B · Kategori sayfası ----------
function KatSayfa9({ title, label = 'Ekim gideri', v, tone = 'out', foot, chips, menu, note, days, empty, month, overlay, flat }) {
  return (
    <Screen9 overlay={overlay}>
      <Head9 title={title} month={month} />
      <div className="body" style={{ overflow: menu ? 'visible' : undefined, paddingBottom: 32 }}>
        <Sum9 label={label} v={v} tone={tone} foot={foot} />
        {chips && <Chips9 menu={menu} note={note}>{chips}</Chips9>}
        {empty || <Days9 days={days} flat={flat} />}
      </div>
    </Screen9>
  );
}
const out = (o) => ({ cap: 'out', tone: 'out', ...o, amount: -Math.abs(o.amount) });
const inc = (o) => ({ cap: 'in', tone: 'in', ...o });
const TICARI = [['5 Ekim', [out({ icon: 'inventory_2', title: 'Toptancı ödemesi', sub: 'Ziraat Vadesiz', amount: 4200 })]], ['4 Ekim', [out({ icon: 'inventory_2', title: 'Kırtasiye toptan', sub: 'Bonus Kart', amount: 3180 })]], ['3 Ekim', [out({ icon: 'inventory_2', title: 'Vadeli alım · Test Toptancı', sub: 'Vadeli', amount: 2500 })]], ['2 Ekim', [out({ icon: 'inventory_2', title: 'Hal alışverişi', sub: 'Dükkan Kasası', amount: 2600 })]]];
const M_TIC_H = [{ label: 'Hepsi', amount: 12480 }, { group: 'Nakit ve banka' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 2600 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 4200 }, { group: 'Kredi kartı' }, { icon: 'credit_card', label: 'Bonus Kart', amount: 3180 }, { group: 'Hesaptan geçmeyen' }, { icon: 'event_upcoming', label: 'Vadeli', sub: 'Vadeli alım, fatura ve borçla alınanlar', amount: 2500 }];
const M_TIC_K = [{ label: 'Hepsi', amount: 12480 }, { icon: 'storefront', label: 'Test Toptancı', amount: 2500 }, { icon: 'person_off', label: 'Kişi yok', amount: 9980 }];
const elek = (tag) => out({ icon: 'receipt_long', title: 'Elektrik faturası', sub: 'Vadeli', tag, amount: 1110 });
const FATURA = (tag) => [['12 Ekim', [out({ icon: 'receipt_long', title: 'Doğalgaz faturası', sub: 'Bonus Kart', amount: 750 })]], ['3 Ekim', [elek(tag)]]];
const KOMISYON = [['28 Ekim', [out({ icon: 'percent', title: 'Hesap işletim ücreti', sub: 'Ziraat Vadesiz', amount: 200 })]], ['25 Ekim', [out({ icon: 'point_of_sale', title: 'Kartlı satış · Ziraat POS', sub: '₺8.400,00 satışın komisyonu', amount: 326 })]], ['21 Ekim', [out({ icon: 'swap_horiz', title: 'Transfer · Ziraat Vadesiz → Garanti Vadesiz', sub: '₺10.000,00 transferin masrafı', amount: 50 })]], ['15 Ekim', [out({ icon: 'move_to_inbox', title: 'POS yatışı · Ziraat Vadesiz', sub: '₺7.169,00 yatışın kesintisi', amount: 40 })]], ['14 Ekim', [out({ icon: 'point_of_sale', title: 'Kartlı satış · Ziraat POS', sub: '₺7.500,00 satışın komisyonu', amount: 291 })]], ['6 Ekim', [out({ icon: 'point_of_sale', title: 'Kartlı satış · Ziraat POS', sub: '₺6.000,00 satışın komisyonu', amount: 233 })]]];
const FAIZ = [['20 Ekim', [out({ icon: 'request_quote', title: 'Borç taksidi ödemesi · Ahmet Usta', sub: '₺1.000,00 taksidin faizi · Ziraat Vadesiz', amount: 150 })]], ['5 Ekim', [out({ icon: 'request_quote', title: 'Borç taksidi ödemesi · Örnek Banka', sub: '₺3.000,00 taksidin faizi · Ziraat Vadesiz', amount: 450 })]]];
const kart = (v, k, st) => inc({ icon: 'point_of_sale', title: 'Kartlı satış · Ziraat POS', sub: 'Ziraat Vadesiz · ' + st, amount: v, under: 'komisyon ' + fmt6(k) });
const nakit = (v) => inc({ icon: 'payments', title: 'Nakit satış · Gün sonu', sub: 'Dükkan Kasası', amount: v });
const veresiye = (k, v) => inc({ icon: 'sell', title: 'Veresiye satış · ' + k, sub: 'Vadeli', amount: v });
const SATIS = [['25 Ekim', [kart(8400, 326, 'Yolda'), nakit(5300)], 'Pazar'], ['14 Ekim', [kart(7500, 291, 'Hesaba geçti'), nakit(4900), veresiye('Ayşe Terzi', 2500)], 'Çarşamba'], ['6 Ekim', [kart(6000, 233, 'Hesaba geçti'), nakit(4600), veresiye('Mehmet Usta', 2000)], 'Salı']];
const M_SATIS = [{ label: 'Hepsi', amount: 41200 }, { group: 'Nakit ve banka' }, { icon: 'payments', label: 'Dükkan Kasası', amount: 14800 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 21900 }, { group: 'Hesaptan geçmeyen' }, { icon: 'event_upcoming', label: 'Vadeli', sub: 'Veresiye satış ve alacaklar', amount: 4500 }];

const H = (p) => <Chip9 label="Hesap / kart" menu {...p} />;
const K = (p) => <Chip9 label="Kişi" menu {...p} />;
const FaintNote = <div style={{ margin: '4px 16px 0', padding: '10px 14px', borderRadius: 16, background: 'var(--neutral-container)', color: 'var(--on-neutral-container)', font: '400 14px/1.4 var(--font-core)', display: 'flex', gap: 8 }}><I6 n="info" s={18} /><span>Ziraat Vadesiz kayıtlarında kişi yok.</span></div>;


// ---------- A8 · Kategorisi yok: kayıtlar açılır, kategori oradan verilir ----------
const YOK = [['22 Ekim', [{ icon: 'category', cap: 'mu', tone: 'in', title: 'Havale · Mehmet Yılmaz', sub: 'Ziraat Vadesiz', tag: { kind: 'open', label: 'Kategori seçilmeli' }, amount: 200 }]], ['9 Ekim', [{ icon: 'category', cap: 'mu', tone: 'in', title: 'Nakit giriş', sub: 'Dükkan Kasası', tag: { kind: 'open', label: 'Kategori seçilmeli' }, amount: 150 }]]];
function KatYok9({ rows = YOK, v = 350, overlay, toast }) {
  return (
    <Screen9 overlay={overlay}>
      <Head9 title="Kategorisi yok" />
      <Body9>
        <Sum9 label="Ekim geliri · İşletme" v={v} tone="in" foot="Geçen ay ₺120,00" />
        <div style={{ margin: '12px 16px 0', padding: '10px 14px', borderRadius: 16, background: 'var(--neutral-container)', color: 'var(--on-neutral-container)', font: '400 14px/1.4 var(--font-core)', display: 'flex', gap: 8 }}><I6 n="info" s={18} /><span>Kayıtların kategorisini seçmek için üzerine dokunun.</span></div>
        {rows.length ? <Days9 days={rows} /> : <Empty9 title="Bütün kayıtların kategorisi var" text="Ekim gelirinin tamamı kategorilere dağıldı." action="Kategori özetine dön" />}
      </Body9>
      {toast && <div role="status" style={{ position: 'absolute', left: 16, right: 16, bottom: 24, zIndex: 20, display: 'flex', alignItems: 'center', gap: 8, minHeight: 48, padding: '4px 4px 4px 16px', borderRadius: 16, background: 'var(--ink)', color: 'var(--surface-card)', boxShadow: 'var(--elevation-floating)', font: '400 14px/1.35 var(--font-core)' }}><span style={{ flex: 1, minWidth: 0 }}>{toast}</span><button style={{ height: 40, padding: '0 12px', border: 0, borderRadius: 12, background: 'transparent', color: 'var(--surface-card)', font: '600 14px/1 var(--font-core)', textDecoration: 'underline', cursor: 'pointer' }}>Geri al</button></div>}
    </Screen9>
  );
}
function KatSec9({ sel }) {
  const [s, setS] = React.useState(sel);
  return (
    <Sheet9 title="Kategori seç" sub="Havale · Mehmet Yılmaz · +₺200,00">
      <div style={{ border: '1px solid var(--border)', borderRadius: 20, overflow: 'hidden' }}>
        {KI.map(([ic, t], n) => { const on = s === n; return (
          <button key={t} role="radio" aria-checked={on} onClick={() => setS(n)} style={{ display: 'flex', alignItems: 'center', gap: 14, width: '100%', minHeight: 56, padding: '8px 16px', border: 0, borderTop: n ? '1px solid var(--border)' : 0, background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: 'pointer', textAlign: 'left', font: 'inherit', color: 'var(--ink)' }}>
            <Cap6 n={ic} tone="in" />
            <span style={{ flex: 1, minWidth: 0, font: (on ? '600' : '400') + ' 16px/1.3 var(--font-core)' }}>{t}</span>
            {on && <I6 n="check" s={22} />}
          </button>); })}
      </div>
      <button style={{ display: 'flex', alignItems: 'center', gap: 8, height: 48, margin: '8px 0 12px', padding: '0 4px', border: 0, background: 'transparent', font: '600 15px/1 var(--font-core)', color: 'var(--ink)', cursor: 'pointer' }}><I6 n="add" s={20} />Yeni kategori</button>
      <Btn9 kind="filled" label="Kaydet" />
    </Sheet9>
  );
}

const BONUS_AY = [1950, 2700, 1450, 3400, 3050, 2350, 0, 1700, 0, 3930, 1100, 0];

function renderKat(R) {
  const g = (o) => <KatOzet9 label="Ekim gideri · İşletme" total={26930} count="Geçen ay ₺24.180,00" cats={KG.all} {...o} />;
  const i = (o) => <KatOzet9 dir="Gelir" label="Ekim geliri · İşletme" total={44550} count="Geçen ay ₺41.300,00" cats={KI} {...o} />;
  R('A1', g());
  R('A2', g({ menu: <Menu9 caption="Ekim gideri · İşletme" items={MG} width={340} /> }));
  R('A3', g({ hesap: 'Ziraat Vadesiz', label: 'Ekim gideri · İşletme · Ziraat Vadesiz', total: 12440, count: 'Geçen ay ₺11.200,00', cats: KG.ziraat }));
  R('A4', g({ hesap: 'Vadeli', label: 'Ekim gideri · İşletme · Vadeli', total: 3610, count: 'Geçen ay ₺2.900,00', cats: KG.vadeli }));
  R('A5', i());
  R('A6', i({ menu: <Menu9 caption="Ekim geliri · İşletme" items={MI} width={340} /> }));
  R('A7', g({ total: 1404650.4, count: 'Geçen ay ₺1.280.400,00', menu: <Menu9 caption="Ekim gideri · İşletme" items={MCOK} width={348} sel={4} maxH={690} /> }));
  R('A8', i({ cats: KI.slice(0, 2), noneCat: ['category', 'Kategorisi yok', 350, 1, true] }));
  R('A9', g({ hesap: 'Bonus Kart', label: 'Ekim gideri · İşletme · Bonus Kart', total: 3930, count: 'Geçen ay ₺1.700,00', cats: [['inventory_2', 'Ticari mal alımı', 3180, 81], ['receipt_long', 'İşyeri faturaları', 750, 19]], overlay: <MonthDlg9 caption="Gider · İşletme · Bonus Kart" values={BONUS_AY} /> }));
  R('A10', g({ month: 'Eylül 2026', hesap: 'Bonus Kart', label: 'Eylül gideri · İşletme · Bonus Kart', total: 0, count: 'Geçen ay ₺1.700,00', empty: <Empty9 title="Eylül'de Bonus Kart'tan gider yok" text="Seçim korunur; başka bir aya geçince yine Bonus Kart'ı gösterir." action="Filtreyi kaldır" /> }));
  R('A11', g({ label: 'Ekim gideri · İşletme', total: 0, count: 'Geçen ay ₺22.900,00', faint: true, empty: <Empty9 title="Ekim'de gider yok" text="Gider ekledikçe kategoriler burada toplanır." /> }));
  R('A12', <KatOzet9 scope={false} label="Ekim gideri" total={18350} count="Geçen ay ₺17.600,00" cats={KS} />);

  R('A8b', <KatYok9 />);
  R('A8c', <KatYok9 overlay={<KatSec9 sel={0} />} />);
  R('A8d', <KatYok9 rows={YOK.slice(1)} v={150} toast="Havale · Mehmet Yılmaz → Satış geliri" />);

  R('B1', <KatSayfa9 title="Ticari mal alımı" v={12480} foot="Geçen ay ₺10.940,00" chips={<>{H()}{K()}</>} days={TICARI} />);
  R('B2', <KatSayfa9 title="Ticari mal alımı" v={12480} foot="Geçen ay ₺10.940,00" chips={<>{H({ open: true })}{K()}</>} menu={<Menu9 side="left" caption="Ekim · Ticari mal alımı" items={M_TIC_H} width={340} />} days={TICARI} />);
  R('B3', <KatSayfa9 title="Ticari mal alımı" v={12480} foot="Geçen ay ₺10.940,00" chips={<>{H()}{K({ open: true })}</>} menu={<Menu9 side="left" caption="Ekim · Ticari mal alımı" items={M_TIC_K} width={320} />} days={TICARI} />);
  R('B4', <KatSayfa9 title="Ticari mal alımı" label="Ekim gideri · Ziraat Vadesiz" v={4200} foot="Kategorinin tamamı ₺12.480,00" chips={<>{H({ value: 'Ziraat Vadesiz' })}{K({ faint: true })}</>} days={TICARI.slice(0, 1)} />);
  R('B4b', <KatSayfa9 title="Ticari mal alımı" label="Ekim gideri · Ziraat Vadesiz" v={4200} foot="Kategorinin tamamı ₺12.480,00" chips={<>{H({ value: 'Ziraat Vadesiz' })}{K({ faint: true })}</>} note={FaintNote} days={TICARI.slice(0, 1)} />);
  R('B5', <KatSayfa9 title="İşyeri faturaları" v={1860} foot="Geçen ay ₺1.640,00" chips={H()} days={FATURA({ kind: 'paid', label: 'Ödendi · 20 Eki · Ziraat Vadesiz' })} />);
  R('B5b', <KatSayfa9 title="İşyeri faturaları" v={1860} foot="Geçen ay ₺1.640,00" chips={H()} days={FATURA({ kind: 'open', label: 'Açık' })} />);
  R('B6', <KatSayfa9 title="Banka ve POS komisyonu" v={1140} foot="Geçen ay ₺980,00" chips={H()} days={KOMISYON} />);
  R('B7', <KatSayfa9 title="Faiz ve finansman gideri" v={600} foot="Geçen ay ₺600,00" chips={<>{H()}{K()}</>} days={FAIZ} />);
  R('B8', <KatSayfa9 title="Satış geliri" label="Ekim geliri" v={41200} tone="in" foot="Geçen ay ₺38.650,00" chips={<>{H()}{K()}</>} days={SATIS} />);
  R('B8b', <KatSayfa9 title="Satış geliri" label="Ekim geliri" v={41200} tone="in" foot="Geçen ay ₺38.650,00" chips={<>{H({ open: true })}{K()}</>} menu={<Menu9 side="left" caption="Ekim · Satış geliri" items={M_SATIS} width={340} />} days={SATIS} />);
  R('B9', <KatSayfa9 title="Sigorta" label="Ekim gideri · İşletme" v={1200} foot="Kategorinin tamamı ₺2.000,00" chips={H()} days={[['8 Ekim', [out({ icon: 'shield', title: 'İşyeri sigortası', sub: 'Ziraat Vadesiz', amount: 1200 })]]]} />);
  R('B9b', <KatSayfa9 title="Sigorta" v={2000} foot="Geçen ay ₺1.200,00" chips={H()} days={[['15 Ekim', [out({ icon: 'shield', title: 'Kasko', sub: 'Bonus Kart · Şahsi', amount: 800 })]], ['8 Ekim', [out({ icon: 'shield', title: 'İşyeri sigortası', sub: 'Ziraat Vadesiz · İşletme', amount: 1200 })]]]} />);
  R('B10', <KatSayfa9 title="Kasa farkı" month="Eylül 2026" label="Eylül gideri" v={0} foot="Geçen ay ₺80,00" empty={<Empty9 title="Eylül'de bu kategoride gider yok" text="Başka bir ay seçebilirsiniz." />} />);
}
Object.assign(window, { renderKat, KatOzet9, KatSayfa9, KatYok9, KatSec9 });
