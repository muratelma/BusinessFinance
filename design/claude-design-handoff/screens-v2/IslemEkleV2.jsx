// İşlem ekle v2 — en sık üç işlem büyük kutucukta, belge okuma ikinci sırada,
// seyrek işlemler tek satırlık kısa listede. Açıklamalar tek satır.
function IslemEkleV2({ onClose, onPick }) {
  const { AppBottomSheet, Icon } = DS;
  const pick = (id) => onPick && onPick(id);
  const primary = [
    { id: 'income', icon: 'south_west', label: 'Gelir', tone: 'income' },
    { id: 'expense', icon: 'north_east', label: 'Gider', tone: 'expense' },
    { id: 'transfer', icon: 'swap_horiz', label: 'Transfer', tone: 'neutral' },
  ];
  const docs = [
    { id: 'receipt', icon: 'receipt_long', label: 'Fiş veya fatura' },
    { id: 'slip', icon: 'account_balance', label: 'Banka dekontu' },
  ];
  const more = [
    { id: 'pos', icon: 'point_of_sale', label: 'POS tahsilatı', hint: 'Kartla satış, sonra hesaba geçer' },
    { id: 'obligation', icon: 'schedule', label: 'Ödenmemiş fatura', hint: 'Vadesi olan, henüz ödenmedi' },
    { id: 'card_payment', icon: 'credit_card', label: 'Kart borcu öde' },
    { id: 'recurring', icon: 'event_repeat', label: 'Tekrarlayan işlem', hint: 'Kira, abonelik, maaş' },
  ];
  const tile = { display: 'flex', flexDirection: 'column', alignItems: 'flex-start', gap: 12, padding: 14, minHeight: 96, border: '1px solid var(--border)', borderRadius: 'var(--radius-card)', background: 'var(--surface-card)', cursor: 'pointer', textAlign: 'left', boxSizing: 'border-box' };
  return (
    <AppBottomSheet title="İşlem ekle" onClose={onClose} maxHeight="92%" style={{ paddingBottom: 'var(--space-lg)' }}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, minmax(0,1fr))', gap: 'var(--space-sm)', marginTop: 20 }}>
        {primary.map((o) => (
          <button key={o.id} onClick={() => pick(o.id)} style={{ ...tile, alignItems: 'center', justifyContent: 'center', gap: 'var(--space-sm)', padding: 'var(--space-sm)', background: 'var(--' + o.tone + '-container)', borderColor: 'var(--' + o.tone + '-container)' }}>
            <Icon name={o.icon} size={24} color={'var(--on-' + o.tone + '-container)'} />
            <span style={{ font: '600 14px/1.3 var(--font-core)', color: 'var(--on-' + o.tone + '-container)', textAlign: 'center' }}>{o.label}</span>
          </button>
        ))}
      </div>

      <div style={{ ...LBL, margin: '24px 0 8px' }}>Belgeden oku</div>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, minmax(0,1fr))', gap: 'var(--space-sm)' }}>
        {docs.map((o) => (
          <button key={o.id} onClick={() => pick(o.id)} style={{ ...tile, flexDirection: 'row', alignItems: 'center', minHeight: 64, gap: 12, background: 'var(--surface-card-muted)', borderColor: 'var(--surface-card-muted)' }}>
            <Icon name={o.icon} size={22} color="var(--ink)" />
            <span style={{ font: '600 15px/1.3 var(--font-core)', color: 'var(--ink)' }}>{o.label}</span>
          </button>
        ))}
      </div>

      <div style={{ ...LBL, margin: '24px 0 4px' }}>Diğer</div>
      <div>
        <RowList inset={56}>
          {more.map((o) => (
            <Row key={o.id} onClick={() => pick(o.id)} pad="10px 0" lead={<Capsule icon={o.icon} />} title={o.label} subtitle={o.hint}
              trailing={<Icon name="chevron_right" size={22} color="var(--ink-muted)" />} />
          ))}
        </RowList>
      </div>
    </AppBottomSheet>
  );
}
Object.assign(window, { IslemEkleV2 });
