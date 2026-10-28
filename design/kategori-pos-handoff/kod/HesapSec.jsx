// Hesap seçici önerileri (H1–H6) + ortak parçalar. Secici.jsx (I6, Card6, Cap6) ve KHParts.jsx (Money9, Chip9, Btn9…) üzerine.
const HA = [
  { k: 'nakit', icon: 'payments', name: 'Dükkan Kasası', sc: 'İşletme', v: 89520 },
  { k: 'nakit', icon: 'payments', name: 'Şahsi cüzdan', sc: 'Şahsi', v: 650 },
  { k: 'nakit', icon: 'payments', name: 'Ziraat şahsi', sc: 'Şahsi', v: 1000 },
  { k: 'banka', icon: 'account_balance', name: 'Ziraat vadesiz', sc: 'İşletme', v: 30658.35 },
  { k: 'banka', icon: 'account_balance', name: 'Garanti Vadesiz', sc: 'İşletme', v: 14500 },
  { k: 'kart', icon: 'credit_card', name: 'İşletme kartı', sc: 'İşletme', v: -3930 },
  { k: 'pasif', icon: 'account_balance', name: 'Garanti', sc: 'İşletme', v: 20000 },
];
const HC = { nakit: 'var(--category-3)', banka: 'var(--category-5)', kart: 'var(--neutral)', pasif: 'var(--ink-faint)' };
const HT = { nakit: 'Nakit', banka: 'Banka hesabı', kart: 'Kredi kartı', pasif: 'Pasif' };
const hOf = (k) => HA.filter((a) => a.k === k);
const hGrpLbl = { font: '600 12px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' };
const HGlyph = ({ a, s = 40 }) => <span style={{ flex: 'none', width: s, height: s, borderRadius: 999, background: 'var(--surface-card-muted)', color: HC[a.k], display: 'flex', alignItems: 'center', justifyContent: 'center' }}><I6 n={a.icon} s={s >= 36 ? 20 : 18} /></span>;

function HNav({ on = 'İşlemler' }) {
  const T = ({ n, l }) => <div style={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 4, color: on === l ? 'var(--ink)' : 'var(--ink-muted)', font: (on === l ? 600 : 500) + ' 12px/1 var(--font-core)' }}><I6 n={n} s={24} f={on === l} />{l}</div>;
  return <div style={{ position: 'relative', flex: 'none', height: 76, background: 'var(--surface-card)', borderTop: '1px solid var(--border)', display: 'flex', alignItems: 'center', padding: '0 8px' }}>
    <T n="space_dashboard" l="Özet" /><T n="receipt_long" l="İşlemler" /><div style={{ width: 72, flex: 'none' }}></div><T n="point_of_sale" l="Kasa" /><T n="more_horiz" l="Diğer" />
    <button aria-label="İşlem ekle" style={{ position: 'absolute', left: '50%', top: -24, transform: 'translateX(-50%)', width: 56, height: 56, borderRadius: 999, border: 0, background: 'var(--brand)', color: 'var(--on-brand)', boxShadow: 'var(--elevation-raised)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}><I6 n="add" s={26} /></button>
  </div>;
}

// Form alanı: değer varken etiket kenarda; focus = 2 px marka kenarlığı. Değer ile ok arası 6 px (okun kendi iç boşluğuyla görünürde ~11 px); tutar sağa yaslı, sola uzar.
function HField({ label, children, focus, icon = 'arrow_drop_down', menu, help }) {
  const has = children != null;
  return <div style={{ position: 'relative', zIndex: menu ? 20 : 'auto' }}>
    <div style={{ position: 'relative', minHeight: 56, boxSizing: 'border-box', border: focus ? '2px solid var(--brand)' : '1px solid var(--border-strong)', borderRadius: 16, background: 'var(--surface-card)', display: 'flex', alignItems: 'center', gap: 6, height: 56, padding: focus ? '7px 11px 7px 15px' : '8px 12px 8px 16px', cursor: 'pointer' }}>
      {has && <span style={{ position: 'absolute', top: -8, left: 12, padding: '0 4px', background: 'var(--surface-card)', font: '400 12px/16px var(--font-core)', color: focus ? 'var(--ink)' : 'var(--ink-muted)' }}>{label}</span>}
      <div style={{ flex: 1, minWidth: 0 }}>{has ? children : <span style={{ font: '400 16px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>{label}</span>}</div>
      {icon && <I6 n={icon} s={24} style={{ color: 'var(--ink-muted)', flex: 'none' }} />}
    </div>
    {help && <div style={{ ...small9, padding: '4px 16px 0' }}>{help}</div>}
    {menu}
  </div>;
}
const HText = ({ t }) => <span style={{ font: '400 16px/1.3 var(--font-core)' }}>{t}</span>;
// Seçili hesabın alandaki görünüşü: simge · ad + tür · bakiye
// Ortak seçili değer (Hesap, POS, Elle gir): 32 px kapsül · ad 15/20 600 · alt yazı 13/16 · sağda tutar. İçerik 36 px → alan her durumda 56 px.
const SelVal = ({ icon, c = 'var(--ink-muted)', t, s, v }) => <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
  <span style={{ flex: 'none', width: 32, height: 32, borderRadius: 999, background: 'var(--surface-card-muted)', color: c, display: 'flex', alignItems: 'center', justifyContent: 'center' }}><I6 n={icon} s={18} /></span>
  <span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: '600 15px/20px var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{t}</span>{s && <span style={{ display: 'block', font: '400 13px/16px var(--font-core)', color: 'var(--ink-muted)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{s}</span>}</span>
  {v != null && <Money9 v={v} size={14} w={500} sign={v < 0} style={{ flex: 'none', color: 'var(--ink-muted)' }} />}
</div>;
const HAccVal = ({ a }) => <SelVal icon={a.icon} c={HC[a.k]} t={a.name} s={HT[a.k] + ' · ' + a.sc} v={a.v} />;

function HForm({ title = 'Gelir ekle', children, overlay, kind = 'gelir' }) {
  return <div className="screen">
    <div style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '12px 16px 8px 4px' }}><button className="iconbtn" aria-label="Geri"><I6 n="arrow_back" s={24} /></button><div style={{ font: '700 22px/1.2 var(--font-core)' }}>{title}</div></div>
    <div className="body" style={{ padding: '12px 16px 32px', display: 'flex', flexDirection: 'column', gap: 16 }}>
      {children}
      <HField label="Kategori" />
      <HField label="Tutar" icon={null} />
      <HField label="Tarih" icon="calendar_today"><HText t="27 Ekim 2026" /></HField>
      <HField label="Ad (isteğe bağlı)" icon={null} help="Boş bırakılırsa kategori adı kullanılır." />
      <Btn9 label="Kaydet" kind="filled" />
    </div>
    <HNav />{overlay}
  </div>;
}

// Alana bağlı menü: kategori filtresindeki menünün aynısı, alan genişliğinde.
function HMenu({ caption, groups, sel, off, foot = false, maxH = 600 }) {
  return <div role="listbox" style={{ position: 'absolute', top: 'calc(100% + 8px)', left: 0, right: 0, background: 'var(--surface-card)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', overflow: 'hidden', display: 'flex', flexDirection: 'column', maxHeight: maxH, animation: 'hmenuIn 150ms cubic-bezier(.2,0,0,1)' }}>
    <style>{'@keyframes hmenuIn{from{opacity:0;transform:translateY(-4px)}to{opacity:1;transform:none}}'}</style>
    {caption && <div style={{ padding: '12px 16px 8px', ...small9, borderBottom: '1px solid var(--border)' }}>{caption}</div>}
    <div className="menu-scroll" style={{ overflowY: 'auto', padding: '4px 0 8px', scrollbarWidth: 'none' }}>
      {groups.map((g, gi) => <div key={g.l}>
        <div style={{ borderTop: gi ? '1px solid var(--border)' : 0, marginTop: gi ? 4 : 0, padding: '12px 16px 4px', ...hGrpLbl }}>{g.l}</div>
        {g.list.map((a) => <HMenuRow key={a.name} a={a} on={sel === a.name} off={off === a.name} />)}
      </div>)}
    </div>
    {foot && <div style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48, padding: '0 16px', borderTop: '1px solid var(--border)', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}><I6 n="add" s={20} />Yeni hesap</div>}
  </div>;
}
const HMenuRow = ({ a, on, off }) => <div role="option" aria-selected={on} aria-disabled={off || undefined} style={{ position: 'relative', display: 'flex', alignItems: 'center', gap: 12, minHeight: 56, padding: '6px 16px', background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: off ? 'default' : 'pointer', color: off ? 'var(--ink-faint)' : 'var(--ink)' }}>
  {on && <span style={{ position: 'absolute', left: 0, top: 8, bottom: 8, width: 4, borderRadius: '0 4px 4px 0', background: 'var(--brand)' }}></span>}
  <span style={{ width: 20, flex: 'none', display: 'flex', color: off ? 'var(--ink-faint)' : HC[a.k] }}><I6 n={a.icon} s={20} /></span>
  <span style={{ flex: 1, minWidth: 0 }}><span style={{ display: 'block', font: (on ? 600 : 400) + ' 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{a.name}</span><span style={{ display: 'block', ...small9, color: off ? 'var(--ink-faint)' : 'var(--ink-muted)' }}>{off ? 'Para buradan çıkıyor' : a.k === 'kart' ? 'Kart borcu · ' + a.sc : a.sc}</span></span>
  {!off && <Money9 v={a.v} size={15} w={on ? 700 : 600} sign={a.v < 0} tone={a.v < 0 ? 'out' : undefined} style={{ flex: 'none' }} />}
</div>;
const G_GELIR = [{ l: 'Nakit', list: hOf('nakit') }, { l: 'Banka', list: hOf('banka') }];
const G_GIDER = [...G_GELIR, { l: 'Kredi kartı', list: hOf('kart') }];

// H1 · Alana bağlı menü (gelir)
const H1 = () => <HForm><HField label="Hesap" focus icon="arrow_drop_up" menu={<HMenu groups={G_GELIR} sel="Dükkan Kasası" />}><HAccVal a={HA[0]} /></HField></HForm>;
// H1b · Seçildikten sonra kapalı alan
const H1b = () => <HForm><HField label="Hesap"><HAccVal a={HA[3]} /></HField></HForm>;
// H1g · Giderde kredi kartı da listede
const H1g = () => <HForm title="Gider ekle"><HField label="Hesap" focus icon="arrow_drop_up" menu={<HMenu groups={G_GIDER} sel="İşletme kartı" />}><HAccVal a={HA[5]} /></HField></HForm>;

// H6 · Transfer: Nereye menüsünde kaynak hesap kapalı
const H6 = () => <HForm title="Transfer">
  <HField label="Nereden"><HAccVal a={HA[0]} /></HField>
  <HField label="Nereye" focus icon="arrow_drop_up" menu={<HMenu groups={[{ l: 'Nakit', list: hOf('nakit') }, { l: 'Banka', list: hOf('banka') }]} sel="Ziraat vadesiz" off="Dükkan Kasası" />}><HAccVal a={HA[3]} /></HField>
</HForm>;

// H7 · Hiç hesap yok. Alana dokununca beyaz kart alanın altında açılır, form aşağı kayar (üstüne binilmez). Tekrar dokununca kapanır.
const H7Card = () => <div role="status" style={{ background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 16, padding: '12px 16px 8px', display: 'flex', gap: 12, animation: 'h7In 200ms cubic-bezier(.2,0,0,1)' }}>
  <style>{'@keyframes h7In{from{opacity:0;transform:translateY(-6px)}to{opacity:1;transform:none}}'}</style>
  <I6 n="account_balance_wallet" s={20} style={{ color: 'var(--ink-muted)', flex: 'none', marginTop: 1 }} />
  <div style={{ flex: 1, minWidth: 0 }}>
    <div style={{ font: '600 15px/1.35 var(--font-core)' }}>Henüz hesap yok</div>
    <div style={{ ...sub9, marginTop: 2 }}>Kayıt için önce bir hesap ekleyin.</div>
    <button style={{ display: 'flex', alignItems: 'center', gap: 6, height: 40, marginTop: 4, marginLeft: -8, padding: '0 8px', border: 0, borderRadius: 12, background: 'transparent', color: 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer' }}><I6 n="add" s={18} />Hesap ekle</button>
  </div></div>;
function H7({ open: o0 = false }) {
  const [open, setOpen] = React.useState(o0);
  return <HForm><div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
    <div onClick={() => setOpen(!open)}><HField label="Hesap" focus={open} icon={open ? 'arrow_drop_up' : 'arrow_drop_down'} /></div>
    {open && <H7Card />}
  </div></HForm>;
}

function renderHesapSec(R) { R('H1', <H1 />); R('H1b', <H1b />); R('H1g', <H1g />); R('H6', <H6 />); R('H7', <H7 />); R('H7o', <H7 open />); }
Object.assign(window, { SelVal, renderHesapSec, HForm, HField, HAccVal, HA, HC, HT, hOf, HGlyph, HNav, hGrpLbl });
