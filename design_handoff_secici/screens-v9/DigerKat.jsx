// Diğer — Para ve hesaplar'ın altında Kategoriler başlığı: Kategori özeti + Kategori yönetimi.
const DK = window.DesignSystem_02d8cd;
const dkLbl = { font: 'var(--text-label-weight) var(--text-label-size)/var(--text-label-line) var(--font-core)', letterSpacing: 'var(--text-label-tracking)', color: 'var(--ink-faint)' };
const dkRowT = { font: 'var(--text-row-weight) var(--text-row-size)/var(--text-row-line) var(--font-core)', color: 'var(--ink)' };
const dkHelp = { font: 'var(--text-helper-size)/var(--text-helper-line) var(--font-core)', color: 'var(--ink-muted)' };

function DkTop({ title, onBack, actions }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', minHeight: 64, padding: onBack ? '8px 4px 4px 4px' : '8px 4px 4px 16px', flex: '0 0 auto' }}>
      {onBack && <button className="iconbtn" aria-label="Geri" title="Geri"><DK.Icon name="arrow_back" size={24} /></button>}
      <h1 style={{ flex: 1, margin: 0, marginLeft: onBack ? 4 : 0, font: '700 26px/1.25 var(--font-core)', letterSpacing: '-0.6px', color: 'var(--ink)' }}>{title}</h1>
      {actions}
    </div>
  );
}
const DkCap = ({ icon }) => <span style={{ width: 40, height: 40, flex: '0 0 auto', borderRadius: '50%', background: 'var(--surface-card-muted)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center' }}><DK.Icon name={icon} size={20} color="var(--ink)" /></span>;
function DkRow({ icon, title, trailing }) {
  return (
    <button type="button" style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', width: '100%', minHeight: 56, padding: '8px 12px 8px 16px', border: 'none', background: 'transparent', textAlign: 'left', cursor: 'pointer', boxSizing: 'border-box', font: 'inherit' }}>
      <DkCap icon={icon} />
      <span style={{ ...dkRowT, flex: 1, minWidth: 0, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>{title}</span>
      {trailing}
    </button>
  );
}
const dkDiv = (inset) => <div style={{ height: 1, background: 'var(--border)', marginLeft: inset }}></div>;
function DkList({ children, inset = 72 }) {
  const items = React.Children.toArray(children).filter(Boolean);
  return items.map((c, i) => <React.Fragment key={i}>{i > 0 && dkDiv(inset)}{c}</React.Fragment>);
}

function DigerKat({ isBusiness = true }) {
  const groups = [
    { label: 'Para ve hesaplar', items: [
      { icon: 'account_balance_wallet', title: 'Hesaplar' },
      { icon: 'credit_card', title: 'Kredi kartlarım' },
      { icon: 'handshake', title: 'Borç ve alacaklar' },
      { icon: 'group', title: 'Cari hesap' },
    ] },
    { label: 'Kategoriler', items: [
      { icon: 'leaderboard', title: 'Kategori özeti' },
      { icon: 'category', title: 'Kategori yönetimi' },
    ] },
    { label: 'Planlama', items: [
      { icon: 'donut_small', title: 'Bütçeler' },
      { icon: 'event_note', title: 'Yükümlülükler' },
      { icon: 'savings', title: 'Tasarruf hedefleri' },
      { icon: 'event_repeat', title: 'Planlama ve raporlar' },
    ] },
    isBusiness && { label: 'Vergi', items: [
      { icon: 'event_available', title: 'Vergi takibi' },
    ] },
    { label: 'Ayarlar', items: [
      { icon: 'notifications_active', title: 'Hatırlatmalar' },
      { icon: 'folder', title: 'Veri ve yedek' },
    ] },
  ].filter(Boolean);
  const chev = <DK.Icon name="chevron_right" size={22} color="var(--ink-muted)" />;
  return (
    <div className="screen">
      <DkTop title="Diğer" />
      <div className="body" style={{ padding: '4px var(--space-md) var(--fab-clearance)' }}>
        <DK.AppCard padding={0}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', padding: 16 }}>
            <span style={{ width: 48, height: 48, borderRadius: '50%', background: 'var(--brand)', color: 'var(--on-brand)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', font: '600 17px/1 var(--font-core)', flex: '0 0 auto' }}>ME</span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 4 }}>
              <span style={dkRowT}>Hesabım</span>
              <span style={dkHelp}>esnaf@ornek.com</span>
            </span>
            {chev}
          </div>
        </DK.AppCard>
        {groups.map((g) => (
          <div key={g.label} style={{ marginTop: 'var(--space-lg)' }}>
            <div style={{ ...dkLbl, padding: '0 4px 8px' }}>{g.label}</div>
            <DK.AppCard padding={0}>
              <DkList>{g.items.map((it) => <DkRow key={it.title} icon={it.icon} title={it.title} trailing={chev} />)}</DkList>
            </DK.AppCard>
          </div>
        ))}
      </div>
    </div>
  );
}


// Öneri panoları: özet kartının alt satırı ve iki kapının ikonu.
const dkCap = { font: '600 13px/1.3 var(--font-core)', color: 'var(--ink-muted)', margin: '20px 4px 0' };
function OneriIkon() {
  const set = (t, items, note) => (
    <div key={t} style={{ marginTop: 'var(--space-lg)' }}>
      <div style={{ ...dkLbl, padding: '0 4px 8px' }}>{t}</div>
      <DK.AppCard padding={0}><DkList>{items.map(([ic, n]) => <DkRow key={ic} icon={ic} title={n} trailing={<span style={{ ...dkHelp, fontSize: 13 }}>{ic}</span>} />)}</DkList></DK.AppCard>
      {note && <div style={{ ...dkHelp, fontSize: 13, margin: '8px 4px 0', textWrap: 'pretty' }}>{note}</div>}
    </div>
  );
  return (
    <div style={{ padding: '4px 16px 24px' }}>
      {set('Kategori özeti', [['leaderboard', 'Sıralama (seçildi)'], ['pie_chart', 'Pasta'], ['bar_chart', 'Çubuk grafik'], ['analytics', 'Analiz']], 'Seçilen: leaderboard.')}
      {set('Kategori yönetimi', [['category', 'Şekiller (seçildi)'], ['label', 'Etiket'], ['edit_note', 'Not + kalem'], ['list_alt', 'Liste'], ['tune', 'Ayar sürgüleri']], 'Seçilen: category.')}
    </div>
  );
}

function renderDiger(R) {
  R('K0', <DigerKat />);
  R('O2', <OneriIkon />);
}
Object.assign(window, { DigerKat, renderDiger });
