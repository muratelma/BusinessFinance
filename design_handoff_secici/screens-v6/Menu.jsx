// Filtre çipi açılır menüsü: çipin altına bağlı, tek seçim, satırda tutar.
// Seçim işareti: ikon varsa ikonun yerine geçer (sol sütun boşa durmaz); checkRight ise tutarın sağında.
function MenuRow({ icon, label, sub, amount, tone, sign, on, onClick, checkRight }) {
  const left = checkRight ? icon : (icon || on);
  return (
    <button role="menuitemradio" aria-checked={on} onClick={onClick} style={{ display: 'flex', alignItems: 'center', gap: 12, width: '100%', minHeight: sub ? 56 : 48, padding: '4px 16px', border: 0, background: on ? 'var(--surface-card-muted)' : 'transparent', cursor: 'pointer', textAlign: 'left', color: 'var(--ink)', font: 'inherit' }}>
      {left && <span style={{ width: 20, flex: 'none', display: 'flex', color: on && !checkRight ? 'var(--ink)' : 'var(--ink-muted)' }}>{on && !checkRight ? <I6 n="check" s={20} style={{ fontVariationSettings: '"wght" 600' }} /> : <I6 n={icon} s={20} />}</span>}
      <span style={{ flex: 1, minWidth: 0 }}>
        <span style={{ display: 'block', font: (on ? '600' : '400') + ' 15px/1.3 var(--font-core)', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{label}</span>
        {sub && <span style={{ display: 'block', font: '400 13px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>{sub}</span>}
      </span>
      {amount != null && <Money6 v={amount} tone={tone} sign={sign} style={{ fontSize: 15, flex: 'none', fontWeight: on ? 700 : 600 }} />}
      {checkRight && <span style={{ width: 20, flex: 'none', display: 'flex' }}>{on && <I6 n="check" s={20} />}</span>}
    </button>
  );
}
function DropMenu({ side = 'right', width = 300, caption, groups, initial = 0, maxH = 404, checkRight, onPick }) {
  const [sel, setSel] = React.useState(initial);
  let k = 0;
  return (
    <div role="menu" style={{ position: 'absolute', top: 'calc(100% + 4px)', [side]: 16, width, background: 'var(--surface-card)', border: '1px solid var(--border)', borderRadius: 16, boxShadow: 'var(--elevation-floating)', overflow: 'hidden', display: 'flex', flexDirection: 'column', maxHeight: maxH }}>
      {caption && <div style={{ padding: '12px 16px 8px', font: '400 13px/1.3 var(--font-core)', color: 'var(--ink-muted)', borderBottom: '1px solid var(--border)' }}>{caption}</div>}
      <div className="menu-scroll" style={{ overflowY: 'auto', padding: '4px 0', scrollbarWidth: 'none' }}>
        {groups.map((g, gi) => (
          <div key={gi} style={{ borderTop: gi ? '1px solid var(--border)' : 0, marginTop: gi ? 4 : 0, paddingTop: gi ? 4 : 0 }}>
            {g.label && <div style={{ padding: '10px 16px 4px', font: '600 12px/1 var(--font-core)', letterSpacing: '.04em', color: 'var(--ink-faint)' }}>{g.label}</div>}
            {g.items.map((o) => { const id = k++; return <MenuRow key={id} {...o} checkRight={checkRight} on={sel === id} onClick={() => { setSel(id); onPick && onPick(o, id); }} />; })}
          </div>
        ))}
      </div>
    </div>
  );
}

const M_HESAP_SADE = [{ items: [{ icon: 'select_all', label: 'Hepsi', amount: 26330 }] }, { items: [{ icon: 'payments', label: 'Dükkan Kasası', amount: 9840 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 12560 }, { icon: 'credit_card', label: 'Bonus Kart', amount: 3930 }] }];
const M_HESAP = [{ items: [{ icon: 'select_all', label: 'Hepsi', amount: 26330 }] }, { label: 'Nakit ve banka', items: [{ icon: 'payments', label: 'Dükkan Kasası', amount: 9840 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 12560 }] }, { label: 'Kredi kartı', items: [{ icon: 'credit_card', label: 'Bonus Kart', amount: 3930 }] }];
const M_HESAP_COK = [{ items: [{ icon: 'select_all', label: 'Hepsi', amount: 1284650.4 }] }, { label: 'Nakit ve banka', items: [{ icon: 'payments', label: 'Dükkan Kasası', amount: 98400 }, { icon: 'payments', label: 'Şube 2 kasası', amount: 41250 }, { icon: 'account_balance', label: 'Ziraat Vadesiz', amount: 512560 }, { icon: 'account_balance', label: 'Türkiye İş Bankası Ticari Vadesiz', amount: 384200.4 }, { icon: 'account_balance', label: 'Vakıfbank POS hesabı', amount: 96000 }] }, { label: 'Kredi kartı', items: [{ icon: 'credit_card', label: 'Bonus Kart', amount: 93930 }, { icon: 'credit_card', label: 'Axess Business', amount: 58310 }] }];
const M_KISI = [{ items: [{ icon: 'select_all', label: 'Hepsi', amount: 12480 }] }, { items: [{ icon: 'storefront', label: 'Örnek Toptan Kağıt', amount: 3180 }, { icon: 'storefront', label: 'Test Toptancı', amount: 2600 }, { icon: 'person_off', label: 'Kişi yok', sub: 'Kişisi girilmemiş işlemler', amount: 6700 }] }];
const M_KAT = [{ items: [{ icon: 'select_all', label: 'Hepsi' }] }, { label: 'Giren', items: [{ icon: 'sell', label: 'Satış geliri', amount: 10490, tone: 'in', sign: true }] }, { label: 'Çıkan', items: [{ icon: 'storefront', label: 'İşyeri kirası', amount: -6500, tone: 'out', sign: true }, { icon: 'inventory_2', label: 'Ticari mal alımı', amount: -4200, tone: 'out', sign: true }, { icon: 'bolt', label: 'Elektrik, su, doğalgaz', amount: -1860, tone: 'out', sign: true }] }];

function TicariDetay({ menu, hesap, kisi, onChip, onMonth }) {
  const rows = [['5 Ekim', 'Toptan kağıt', 'Örnek Toptan Kağıt · Ziraat Vadesiz', -4200], ['4 Ekim', 'Kırtasiye toptan', 'Örnek Toptan Kağıt · Bonus Kart', -3180], ['2 Ekim', 'Hal alışverişi', 'Test Toptancı · Dükkan Kasası', -2600], ['1 Ekim', 'Ambalaj', 'Dükkan Kasası', -2500]];
  return (
    <div className="screen">
      <Header6 title="Ticari mal alımı" onMonth={onMonth} />
      <div className="body" style={{ overflow: 'visible' }}>
        <Card6 style={{ padding: '16px 20px', marginTop: 4 }}>
          <div style={muted6}>Ekim gideri</div>
          <div style={{ font: '700 28px/1.25 var(--font-core)', fontVariantNumeric: 'tabular-nums', margin: '4px 0', color: 'var(--expense)' }}>{fmt6(12480)}</div>
          <div style={muted6}>Geçen ay {fmt6(10940)}</div>
        </Card6>
        <ChipRow menu={menu}><Chip6 label="Hesap / kart" menu value={hesap} open={menu && menu.props.which === 'hesap'} onClick={onChip && (() => onChip('hesap'))} /><Chip6 label="Kişi" menu value={kisi} open={menu && menu.props.which === 'kisi'} onClick={onChip && (() => onChip('kisi'))} /></ChipRow>
        {rows.map(([d, t, s, v]) => (
          <div key={t}><div style={{ margin: '16px 16px 0', font: '600 15px/1 var(--font-core)', color: 'var(--ink-muted)' }}>{d}</div>
            <Card6 style={{ marginTop: 8 }}><div style={{ display: 'flex', alignItems: 'center', gap: 16, padding: '14px 16px' }}><Cap6 n="north_east" /><div style={{ flex: 1, minWidth: 0 }}><div style={{ font: '600 16px/1.3 var(--font-core)' }}>{t}</div><div style={{ ...muted6, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{s}</div></div><Money6 v={v} tone="out" sign /></div></Card6></div>
        ))}
      </div>
    </div>
  );
}
Object.assign(window, { DropMenu, TicariDetay, M_HESAP_SADE, M_HESAP, M_HESAP_COK, M_KISI, M_KAT });
