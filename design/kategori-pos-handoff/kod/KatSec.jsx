// Kategori seçici · tek tasarım (Gelir ekle + Gider ekle). Filtre değil: satıra dokunmak = seç ve kapat.
// Dokununca form kayar, Kategori alanı başlığın altına gelir; menü alanın altında açılır, en fazla alt çubuğun 16 px üstüne (klavyede klavyenin 8 px üstüne) kadar uzar.
// 8'den fazla kategori varsa: arama + Son kullanılanlar (en çok 4). Gruplar: İşletme · Şahsi · İşletme veya şahsi; boş grup yok.
const KCG = {
  son: [['inventory_2', 'Ticari mal alımı', 'İşletme'], ['receipt_long', 'İşyeri faturaları', 'İşletme'], ['local_gas_station', 'Araç ve yakıt', 'İşletme'], ['shopping_basket', 'Market', 'Şahsi']],
  isl: [['inventory', 'Ambalaj ve sarf malzemesi'], ['local_gas_station', 'Araç ve yakıt'], ['build', 'Bakım-onarım'], ['percent', 'Banka ve POS komisyonu'], ['call', 'İletişim'], ['receipt_long', 'İşyeri faturaları'], ['store', 'İşyeri kirası'], ['badge', 'Personel'], ['shield', 'Sigorta'], ['inventory_2', 'Ticari mal alımı'], ['gavel', 'Vergi ve harç']],
  sah: [['subscriptions', 'Abonelikler'], ['school', 'Eğitim'], ['theater_comedy', 'Eğlence'], ['receipt_long', 'Ev faturaları'], ['home', 'Ev kirası ve aidat'], ['checkroom', 'Giyim'], ['shopping_basket', 'Market'], ['local_hospital', 'Sağlık']],
  ort: [['request_quote', 'Faiz ve finansman gideri'], ['local_offer', 'Diğer gider']],
};
const KCI = { isl: [['sell', 'Satış geliri'], ['handyman', 'Hizmet geliri']], sah: [['home', 'Kira geliri']], ort: [['savings', 'Faiz geliri'], ['local_offer', 'Diğer gelir']] };
const kGrp = { padding: '12px 16px 4px', font: '600 12px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' };
const hl = (t, q) => { if (!q) return t; const i = t.toLocaleLowerCase('tr').indexOf(q); if (i < 0) return t; return <>{t.slice(0, i)}<b style={{ fontWeight: 700 }}>{t.slice(i, i + q.length)}</b>{t.slice(i + q.length)}</>; };

const KSub = ({ ic, l }) => <div role="option" style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48, padding: '0 16px 0 48px', cursor: 'pointer' }}><I6 n={ic} s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} /><span style={{ flex: 1, font: '400 15px/1.3 var(--font-core)' }}>{l}</span></div>;
const KRow = ({ ic, t, sc, on, q, exp }) => exp ? <div>
  <div style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 48, padding: '0 16px' }}><I6 n={ic} s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} /><span style={{ flex: 1, minWidth: 0, font: '600 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{hl(t, q)}</span><I6 n="expand_less" s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} /></div>
  <KSub ic="storefront" l="İşletme" /><KSub ic="person" l="Şahsi" />
</div> : <div role="option" aria-selected={on} style={{ position: 'relative', display: 'flex', alignItems: 'center', gap: 12, boxSizing: 'border-box', minHeight: 48, padding: '0 16px', background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: 'pointer' }}>
  {on && <span style={{ position: 'absolute', left: 0, top: 8, bottom: 8, width: 4, borderRadius: '0 4px 4px 0', background: 'var(--brand)' }}></span>}
  <I6 n={ic} s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} />
  <span style={{ flex: 1, minWidth: 0, font: (on ? 600 : 400) + ' 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{hl(t, q)}</span>
  {sc === 'ort' ? <I6 n="expand_more" s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} /> : sc && <span style={{ flex: 'none', ...small9 }}>{sc}</span>}
</div>;
const KSearch2 = ({ q, focus }) => <div style={{ display: 'flex', alignItems: 'center', gap: 10, height: 44, margin: '8px 8px 4px', padding: '0 10px 0 12px', boxSizing: 'border-box', borderRadius: 12, background: 'var(--surface-card-muted)', border: focus ? '2px solid var(--brand)' : '2px solid transparent', flex: 'none' }}>
  <I6 n="search" s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} />
  <span style={{ flex: 1, font: '400 15px/1 var(--font-core)', color: q ? 'var(--ink)' : 'var(--ink-muted)' }}>{q || 'Kategori ara'}{focus && <span style={{ display: 'inline-block', width: 2, height: 18, background: 'var(--ink)', verticalAlign: -3, marginLeft: 1 }}></span>}</span>
  {q && <I6 n="close" s={20} style={{ color: 'var(--ink-muted)', flex: 'none' }} />}
</div>;
function KList({ data, sel, q, exp, mode }) {
  const f = (arr) => q ? arr.filter((c) => c[1].toLocaleLowerCase('tr').includes(q)) : arr;
  const ort = (arr) => arr.map((c) => [c[0], c[1], mode ? null : 'ort']);
  const big = data.isl.length + data.sah.length + data.ort.length > 8;
  if (mode === 'flat') { const all = f([...data.isl, ...data.sah, ...data.ort]); return all.map((c) => <KRow key={c[1]} ic={c[0]} t={c[1]} on={sel === c[1]} q={q} />); }
  const groups = [mode !== 'filter' && big && !q && data.son && ['Son kullanılanlar', data.son, true], ['İşletme', f(data.isl)], ['Şahsi', f(data.sah)], ['İşletme veya şahsi', ort(f(data.ort))]].filter((g) => g && g[1].length);
  if (!groups.length) return <div role="status" style={{ display: 'flex', gap: 12, padding: '12px 16px 8px' }}><I6 n="search_off" s={20} style={{ color: 'var(--ink-muted)', flex: 'none', marginTop: 1 }} /><div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 15px/1.35 var(--font-core)' }}>Bu adla kategori yok</div><div style={{ ...sub9, marginTop: 2 }}>Aradığınız adla yeni kategori ekleyebilirsiniz.</div><button style={{ display: 'flex', alignItems: 'center', gap: 6, height: 40, marginTop: 4, marginLeft: -8, padding: '0 8px', border: 0, borderRadius: 12, background: 'transparent', color: 'var(--ink)', font: '600 14px/1 var(--font-core)', cursor: 'pointer' }}><I6 n="add" s={18} /><span>{'"' + q + '" ekle'}</span></button></div></div>;
  const tum = mode === 'filter' && !q ? <KRow ic="select_all" t="Tümü" on={sel === 'Tümü'} /> : null;
  return <>{tum}{groups.map((g, gi) => <div key={g[0]}>
    <div style={{ ...kGrp, borderTop: gi || tum ? '1px solid var(--border)' : 0, marginTop: gi || tum ? 4 : 0 }}>{g[0]}</div>
    {g[1].map((c) => <KRow key={g[0] + c[1]} ic={c[0]} t={c[1]} sc={c[2]} on={!g[2] && sel === c[1]} q={q} exp={exp === c[1]} />)}
  </div>)}</>;
}
const KMenu2 = ({ data, sel, q, focus, h, exp, mode }) => { const big = data.isl.length + data.sah.length + data.ort.length > 8; return <div role="listbox" style={{ position: 'absolute', top: 'calc(100% + 8px)', left: 0, right: 0, maxHeight: h, background: 'var(--surface-card)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', overflow: 'hidden', display: 'flex', flexDirection: 'column', animation: 'hmenuIn 150ms cubic-bezier(.2,0,0,1)' }}>
  <style>{'@keyframes hmenuIn{from{opacity:0;transform:translateY(-4px)}to{opacity:1;transform:none}}'}</style>
  {big && <KSearch2 q={q} focus={focus} />}
  <div className="menu-scroll" style={{ flex: 1, minHeight: 0, overflowY: 'auto', padding: big ? '0 0 8px' : '4px 0 8px', scrollbarWidth: 'none' }}><KList data={data} sel={sel} q={q} exp={exp} mode={mode} /></div>
</div>; };
const KKeyboard = () => <div aria-hidden="true" style={{ position: 'absolute', left: 0, right: 0, bottom: 0, height: 290, background: 'var(--surface-card-muted)', borderTop: '1px solid var(--border)', display: 'flex', alignItems: 'center', justifyContent: 'center', ...small9, zIndex: 40 }}>Klavye</div>;

// Kapsam çubuğu: yalnız kapsamsız kategori seçiliyken, alanın 8 px altında.
const kapSeg = (sel, err) => <div role="radiogroup" aria-label="Kapsam" style={{ display: 'flex', height: 48, boxSizing: 'border-box', border: '1px solid ' + (err ? 'var(--expense)' : 'var(--border-strong)'), borderRadius: 999, overflow: 'hidden' }}>
  {['İşletme', 'Şahsi'].map((l, i) => { const on = sel === l; return <div key={l} role="radio" aria-checked={on} style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6, borderLeft: i ? '1px solid ' + (err ? 'var(--expense)' : 'var(--border-strong)') : 0, background: on ? 'var(--brand)' : 'transparent', color: on ? 'var(--on-brand)' : 'var(--ink)', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}>{on && <I6 n="check" s={18} />}{l}</div>; })}
</div>;
const kapErr = <div style={{ display: 'flex', alignItems: 'center', gap: 6, padding: '4px 16px 0', font: '400 13px/1.35 var(--font-core)', color: 'var(--expense)' }}><I6 n="error" s={16} />Kapsam seçin.</div>;

// Tek form: kind 'gider' (kategori 4. sırada) | 'gelir' (2. sırada). val = [simge, ad, alt yazı]; kap = kapsamsız → çubuk.
function KF({ kind = 'gider', open, q, focus, kb, val, kap, kapSel, err, sel, exp, mode, dataOv }) {
  const body = React.useRef(null), kat = React.useRef(null);
  const h = kb ? 594 - 144 : 800 - 144; // menü üstü 144; alt sınır alt çubuk 816−16 / klavye 602−8
  const data = dataOv || (kind === 'gider' ? KCG : KCI);
  const katF = <div ref={kat} style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
    <HField label="Kategori" focus={open} icon={open ? 'arrow_drop_up' : 'arrow_drop_down'} menu={open ? <KMenu2 data={data} sel={sel} q={q} focus={focus} h={h} exp={exp} mode={mode} /> : null}>{val ? <SelVal icon={val[0]} t={val[1]} s={val[2]} /> : null}</HField>
    {kap && <div>{kapSeg(kapSel, err)}{err && kapErr}</div>}
  </div>;
  const F = { tutar: <HField key="t" label="Tutar" icon={null}><HText t="₺1.250,00" /></HField>, hesap: <HField key="h" label="Hesap"><HAccVal a={HA[3]} /></HField>, tarih: <HField key="d" label="Tarih" icon="calendar_today"><HText t="28 Ekim 2026" /></HField>, kat: <React.Fragment key="k">{katF}</React.Fragment> };
  const order = ['hesap', 'kat', 'tutar', 'tarih'];
  return <div className="screen">
    <div style={{ display: 'flex', alignItems: 'center', gap: 4, padding: '12px 16px 8px 4px', height: 68, boxSizing: 'border-box', flex: 'none' }}><button className="iconbtn" aria-label="Geri"><I6 n="arrow_back" s={24} /></button><div style={{ font: '700 22px/1.2 var(--font-core)' }}>{kind === 'gider' ? 'Gider ekle' : 'Gelir ekle'}</div></div>
    <div className="body" ref={body} style={{ position: 'relative', padding: '12px 16px 32px', overflowY: open ? 'hidden' : 'auto' }}><div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
      {open && <div style={{ marginTop: -16 - order.indexOf('kat') * 72, flex: 'none' }}></div>}
      {order.map((k, i) => i > order.indexOf('kat') && open ? <div key={k} style={{ visibility: 'hidden' }}>{F[k]}</div> : F[k])}
      <div style={{ visibility: open ? 'hidden' : 'visible', display: 'flex', flexDirection: 'column', gap: 16 }}><HField label="Ad (isteğe bağlı)" icon={null} help="Boş bırakılırsa kategori adı kullanılır." />
      <Btn9 label="Kaydet" kind="filled" /></div>
      {open && <div style={{ height: 700, flex: 'none' }}></div>}
    </div></div>
    <HNav />{kb && <KKeyboard />}
  </div>;
}

const FAIZ = ['savings', 'Faiz geliri'];
// Ek görünümler (Z5 bileşeni, mode ile): fixed = kapsam sabit (yalnız o kapsam + İşletme veya şahsi, ok yok) · flat = kapsam görünmez · filter = süzgeç, Tümü satırı
const KD_SAH = { isl: [], sah: [['shopping_basket', 'Market'], ['directions_bus', 'Ulaşım'], ['receipt_long', 'Faturalar'], ['local_hospital', 'Sağlık'], ['local_offer', 'Diğer gider']], ort: [] };
const EK2 = () => <KF open mode="flat" dataOv={KD_SAH} sel="Market" val={['shopping_basket', 'Market']} />;
const EK2c = () => <KF mode="flat" dataOv={KD_SAH} val={['shopping_basket', 'Market']} />;
function renderKatSec(R) {
  R('EK2', <EK2 />); R('EK2c', <EK2c />); R('EKP', <PosSheet field={<div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}><HField label="POS"><PosValN p={null} /></HField><HField label="Paranın geçeceği hesap"><HAccVal a={HA[4]} /></HField></div>} katVal={<SelVal icon="sell" t="Satış geliri" s="İşletme" />} />);
  R('K4', <KF />);
  R('K4o', <KF open sel="İşyeri faturaları" val={['receipt_long', 'İşyeri faturaları', 'İşletme']} />);
  R('K4s', <KF open q="fat" focus kb />);
  R('K4n', <KF open q="kırtasiye" focus kb />);
  R('G1', <KF kind="gelir" open />);
  R('Z0', <KF kind="gelir" val={['handyman', 'Hizmet geliri', 'İşletme']} />);
  R('Z5', <KF kind="gelir" open exp="Faiz geliri" />);
  R('Z5g', <KF open q="faiz" exp="Faiz ve finansman gideri" />);
  R('Z5gb', <KF val={['request_quote', 'Faiz ve finansman gideri', 'Şahsi']} />);
  R('Z5b', <KF kind="gelir" val={[...FAIZ, 'İşletme']} />);
}
Object.assign(window, { renderKatSec });
