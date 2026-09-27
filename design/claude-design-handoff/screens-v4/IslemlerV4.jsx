// İşlemler — son öneri (Akış tabanlı) + yeniden tasarlanan işlem detayı.
const DETAIL_EXTRA = {
  'Banka ve POS komisyonu': { note: 'Gun sonu POS - yolda', accountLabel: 'Paranın geçeceği hesap', locked: 'Bu hareket türü iptal edilemez: POS tahsilatıyla birlikte oluşur.' },
};
const scopeOf = (it) => /Sahsi|Sağlık/.test(it.subtitle) ? 'Şahsi' : 'İşletme';
const longDate = (iso) => { const d = new Date(iso + 'T12:00:00'); return d.getDate() + ' ' + TR_MONTHS[d.getMonth()] + ' ' + d.getFullYear() + ', ' + TR_DAYS[d.getDay()]; };

function DetailRow({ icon, label, value }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 12, minHeight: 44 }}>
      <DS.Icon name={icon} size={18} color="var(--ink-muted)" />
      <span style={{ font: '14px/1.3 var(--font-core)', color: 'var(--ink-muted)', flex: '0 0 auto' }}>{label}</span>
      <span style={{ flex: 1, minWidth: 0, textAlign: 'right', font: '500 14px/1.35 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{value}</span>
    </div>
  );
}

function TxnDetailV4({ item, cancelled, onCancel, onClose }) {
  const { AppBottomSheet, AppMoneyText, AppStatusChip, AppSubmitButton, Icon } = DS;
  const isTransfer = item.effect === 'neutral';
  const route = isTransfer ? subNoDate(item.subtitle).split(' → ') : null;
  const tone = cancelled ? 'cancelled' : item.effect;
  const extra = DETAIL_EXTRA[item.title] || {};
  const d = new Date(item.date + 'T12:00:00');
  return (
    <AppBottomSheet onClose={onClose} maxHeight="80%" style={{ paddingBottom: 'var(--space-md)' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: -20 }}>
        <Capsule icon={item.icon} tone={tone} size={40} />
        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 0 }}>
          <span style={{ font: '600 16px/1.3 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{item.title}</span>
          <span style={{ font: '13px/1.3 var(--font-core)', color: 'var(--ink-muted)' }}>{V2_EFFECT_LABEL[item.effect]} · {scopeOf(item)}</span>
        </span>
        <button className="iconbtn" onClick={onClose} aria-label="Kapat" title="Kapat" style={{ marginRight: -12 }}><Icon name="close" size={22} /></button>
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)', marginTop: 'var(--space-md)' }}>
        <span style={{ flex: 1 }}><AppMoneyText amount={item.amount} effect={item.effect} isCancelled={cancelled} signed size="metric" style={{ fontSize: 28 }} /></span>
        {cancelled ? <AppStatusChip label="İptal edildi" icon="block" tone="cancelled" /> : <AppStatusChip label="Gerçekleşti" icon="check_circle" tone="planned" />}
      </div>

      <div style={{ marginTop: 12, padding: '0 var(--space-md)', borderRadius: 'var(--radius-field)', background: 'var(--surface-card-muted)' }}>
        <RowList inset={30}>
          <DetailRow icon="event" label="Tarih" value={d.getDate() + ' ' + TR_MONTHS[d.getMonth()] + ' ' + d.getFullYear()} />
          {isTransfer ? <DetailRow icon="swap_horiz" label="Hesaplar" value={route[0] + ' → ' + route[1]} /> : <DetailRow icon="local_offer" label="Kategori" value={item.category} />}
          {!isTransfer && <DetailRow icon={/Kart/.test(item.account) ? 'credit_card' : /Kasa|Cuzdan/.test(item.account) ? 'payments' : 'account_balance'} label={extra.accountLabel || 'Hesap'} value={item.account} />}
          {extra.note && <DetailRow icon="notes" label="Açıklama" value={extra.note} />}
          <DetailRow icon="edit_note" label="Köken" value="Elle eklendi" />
          <DetailRow icon="attach_file" label="Belge" value="Yok" />
        </RowList>
      </div>

      {cancelled && <div style={{ ...HELP, fontSize: 13, marginTop: 12 }}>Kayıt duruyor; toplamları artık etkilemez.</div>}
      {!cancelled && extra.locked && (
        <div style={{ display: 'flex', alignItems: 'flex-start', gap: 8, marginTop: 12, font: '13px/1.4 var(--font-core)', color: 'var(--ink-muted)' }}>
          <Icon name="lock" size={16} color="var(--ink-muted)" /><span>{extra.locked}</span>
        </div>
      )}
      {!cancelled && !extra.locked && (
        <div style={{ marginTop: 'var(--space-md)' }}>
          <AppSubmitButton label="Hareketi iptal et" icon="block" variant="outlined" fullWidth onSubmit={onCancel} />
        </div>
      )}
    </AppBottomSheet>
  );
}

function IslemlerSon({ onOpenPlanned, initialDetail = null, initialCancelled = [] }) {
  const { AppFilterChips, AppEmptyView, AppStatusChip, AppConfirmDialog, AppMoneyText } = DS;
  const [chip, setChip] = React.useState('all');
  const [detail, setDetail] = React.useState(initialDetail ? DATA.activities.find((a) => a.title === initialDetail) : null);
  const [confirm, setConfirm] = React.useState(null);
  const [cancelled, setCancelled] = React.useState(() => new Set(initialCancelled));
  const items = chip === 'all' ? DATA.activities : DATA.activities.filter((a) => a.kind === chip);
  const rootRef = React.useRef(null);
  const [host, setHost] = React.useState(null);
  React.useEffect(() => { setHost(rootRef.current && rootRef.current.closest('.phone')); }, []);
  const portal = (node) => (host ? ReactDOM.createPortal(node, host) : node);
  return (
    <div className="screen" ref={rootRef}>
      <TopBar title="İşlemler" actions={<ToolButton icon="tune" label="İşlemleri filtrele" />} />
      <div style={{ padding: '4px var(--space-md) 0', flex: '0 0 auto' }}><SearchField /></div>
      <AppFilterChips value={chip} onChange={setChip} label="İşlem türü" style={{ padding: '12px var(--space-md) 8px', flex: '0 0 auto' }} options={TXN_FILTERS} />
      <div className="body" style={{ paddingBottom: 'var(--fab-clearance)' }}>
        <div style={{ padding: '4px var(--space-md) 0' }}><PlannedStrip onOpen={onOpenPlanned} /></div>
        {items.length === 0 ? <AppEmptyView icon="receipt_long" title="Henüz işlem yok" message="İlk gelir veya giderinizi ekleyebilirsiniz." /> : groupByDay(items).map((g) => {
          const [pre, base] = dayLabel(g.date);
          return (
            <div key={g.date}>
              <div style={{ position: 'sticky', top: 0, zIndex: 1, display: 'flex', alignItems: 'baseline', gap: 8, padding: '20px var(--space-md) 8px', background: 'var(--surface-canvas)' }}>
                <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{base}</span>
                <span style={HELP}>{pre}</span>
              </div>
              <div style={{ background: 'var(--surface-card)', borderTop: '1px solid var(--border)', borderBottom: '1px solid var(--border)' }}>
                <RowList inset={72}>
                  {g.items.map((it) => {
                    const isC = cancelled.has(it.title);
                    return (
                      <button key={it.title} onClick={() => setDetail(it)} style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', width: '100%', minHeight: 64, padding: '12px var(--space-md)', border: 'none', background: 'transparent', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box' }}>
                        <span style={{ opacity: isC ? 0.55 : 1, display: 'flex' }}><Capsule icon={it.icon} tone={isC ? 'cancelled' : it.effect} /></span>
                        <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
                          <span style={{ ...ROWT, opacity: isC ? 0.55 : 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{it.title}</span>
                          <span style={{ ...HELP, opacity: isC ? 0.55 : 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{subNoDate(it.subtitle)}</span>
                          {isC && <span style={{ marginTop: 4 }}><AppStatusChip label="İptal edildi" icon="block" tone="cancelled" /></span>}
                        </span>
                        <span style={{ opacity: isC ? 0.55 : 1 }}><AppMoneyText amount={it.amount} effect={it.effect} isCancelled={isC} signed size="row" /></span>
                      </button>
                    );
                  })}
                </RowList>
              </div>
            </div>
          );
        })}
        <div style={{ ...HELP, textAlign: 'center', padding: '20px 0 0' }}>Toplam {items.length} hareket</div>
      </div>
      {detail && !confirm && portal(<TxnDetailV4 item={detail} cancelled={cancelled.has(detail.title)} onCancel={() => setConfirm(detail)} onClose={() => setDetail(null)} />)}
      {confirm && portal(
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-md)', zIndex: 30 }}>
          <AppConfirmDialog icon="block" destructive highlight={V2_EFFECT_LABEL[confirm.effect] + ' · ' + fmt(confirm.amount)}
            message="Kayıt silinmez; iptal edildi olarak işaretlenir ve toplamları artık etkilemez." confirmLabel="Hareketi iptal et"
            onConfirm={() => { setCancelled(new Set([...cancelled, confirm.title])); setConfirm(null); }} onCancel={() => setConfirm(null)} />
        </div>
      )}
    </div>
  );
}
Object.assign(window, { IslemlerSon, TxnDetailV4 });
