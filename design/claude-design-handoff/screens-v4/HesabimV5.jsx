// Hesabım v5 — iyileştirmeler:
// 1) Doğrulama uyarısı kimlik kartının içinde (ayrı kart yok).
// 2) "İşletmem var" Tercihler başlığı altında, etkisini tek satırda söyler.
// 3) Oturumlar: sayaç + "Diğerlerini kapat" toplu eylemi, göreli zaman.
// 4) Çıkış yap kendi düğmesi; silme öncesinde yedek adımı aynı kartta.
function HesabimV5({ onBack, initialScroll = 0, onNavigate = () => {} }) {
  const { AppCard, AppSwitch, AppSubmitButton, AppConfirmDialog, AppStatusChip, AppInlineNotice, AppRowAction, Icon } = DS;
  const [verified, setVerified] = React.useState(false);
  const [business, setBusiness] = React.useState(true);
  const [sessions, setSessions] = React.useState([
    { id: 'a', current: true, rel: 'Şu an açık', until: '22 Ekim' },
    { id: 'b', rel: '7 gün önce açıldı', until: '18 Ekim' },
    { id: 'c', rel: '23 gün önce açıldı', until: '2 Ekim' },
  ]);
  const [dialog, setDialog] = React.useState(null);
  const bodyRef = React.useRef(null);
  React.useEffect(() => { if (bodyRef.current) bodyRef.current.scrollTop = initialScroll; }, []);
  const chev = <Icon name="chevron_right" size={22} color="var(--ink-muted)" />;
  const others = sessions.filter((s) => !s.current);
  return (
    <div className="screen">
      <TopBar title="Hesabım" onBack={onBack} />
      <div className="body" ref={bodyRef} style={{ padding: 'var(--space-xs) var(--space-md) var(--space-xl)' }}>
        <AppCard padding="var(--space-md)">
          <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
            <span style={{ width: 56, height: 56, borderRadius: '50%', background: 'var(--brand)', color: 'var(--on-brand)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', font: '600 19px/1 var(--font-core)', flex: '0 0 auto' }}>ME</span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
              <span style={{ font: '600 17px/1.3 var(--font-core)', color: 'var(--ink)', overflow: 'hidden', textOverflow: 'ellipsis' }}>esnaf@ornek.com</span>
              <span style={HELP}>Hesap açılışı · 14 Mart 2026</span>
            </span>
          </div>
          <div style={{ marginTop: 'var(--space-md)' }}>
            {verified ? <AppStatusChip label="E-posta doğrulandı" icon="check_circle" tone="income" />
              : <AppInlineNotice icon="mark_email_unread" message="E-posta adresiniz doğrulanmadı. Uygulamayı kullanmaya devam edebilirsiniz; doğrulama parolanızı unuttuğunuzda hesabı geri almanızı sağlar." actionLabel="Adresimi doğrula" onAction={() => setVerified(true)} />}
          </div>
        </AppCard>

        <Section title="Tercihler">
          <AppCard padding={0}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)', padding: '12px var(--space-md)' }}>
              <Capsule icon="storefront" />
              <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 2 }}>
                <span style={ROWT}>İşletmem var</span>
                <span style={{ ...HELP, textWrap: 'pretty' }}>{business ? 'Kayıtlar işletme ve şahsi olarak ayrı okunur.' : 'İşletme ve şahsi ayrımı gizli.'} Kategoriler değişmez.</span>
              </span>
              <AppSwitch value={business} onChange={setBusiness} label="İşletmem var" />
            </div>
          </AppCard>
        </Section>

        <Section title={'Açık oturumlar · ' + sessions.length} trailing={others.length > 0 && <AppRowAction label="Diğerlerini kapat" icon="logout" onClick={() => setDialog({ kind: 'revokeAll' })} />}>
          <AppCard padding={0}>
            <RowList inset={72}>
              {sessions.map((s) => (
                <Row key={s.id} lead={<Capsule icon={s.current ? 'smartphone' : 'devices'} tone={s.current ? 'neutral' : undefined} />} title={s.current ? 'Bu cihaz' : 'Başka bir cihaz'} subtitle={s.rel + ' · ' + s.until + "'e kadar geçerli"}
                  trailing={s.current ? null : <ToolButton icon="close" label="Oturumu kapat" onClick={() => setDialog({ kind: 'revoke', s })} />} pad={s.current ? '12px 16px' : '12px 4px 12px 16px'} />
              ))}
            </RowList>
          </AppCard>
        </Section>

        <Section title="Güvenlik">
          <AppCard padding={0}>
            <Row lead={<Capsule icon="password" />} title="Parolamı değiştir" subtitle="Diğer cihazlardaki oturumlar kapanır; bu cihazda açık kalırsınız." trailing={chev} onClick={() => {}} pad="12px 12px 12px 16px" />
          </AppCard>
        </Section>

        <div style={{ height: 'var(--space-lg)' }} />
        <AppSubmitButton label="Çıkış yap" icon="logout" variant="outlined" fullWidth onSubmit={() => setDialog({ kind: 'logout' })} />

        <Section title="Hesabı kapat">
          <AppCard padding={0}>
            <Row lead={<Capsule icon="folder" />} title="Önce yedeğinizi alın" subtitle="Diğer › Veri ve yedek" trailing={chev} onClick={() => onNavigate('data_tools')} pad="12px 12px 12px 16px" />
            <div style={{ height: 1, background: 'var(--border)', marginLeft: 72 }} />
            <Row lead={<Capsule icon="delete_forever" tone="expense" />} title={<span style={{ color: 'var(--expense)' }}>Hesabımı sil</span>} subtitle="Hesap ve bütün kayıtlar kalıcı olarak silinir; geri alınamaz." onClick={() => setDialog({ kind: 'delete' })} pad="12px 12px 12px 16px" />
          </AppCard>
        </Section>
      </div>
      {dialog && (
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-lg)' }}>
          {dialog.kind === 'revoke' && <AppConfirmDialog icon="no_accounts" message="O cihaz bir sonraki denemesinde yeniden giriş yapmak zorunda kalır." highlight={dialog.s.rel} confirmLabel="Oturumu kapat" destructive onCancel={() => setDialog(null)} onConfirm={() => { setSessions(sessions.filter((x) => x.id !== dialog.s.id)); setDialog(null); }} />}
          {dialog.kind === 'revokeAll' && <AppConfirmDialog icon="no_accounts" message={others.length + ' cihazdaki oturum kapanır; o cihazlar yeniden giriş yapmak zorunda kalır. Bu cihazda açık kalırsınız.'} confirmLabel="Hepsini kapat" destructive onCancel={() => setDialog(null)} onConfirm={() => { setSessions(sessions.filter((x) => x.current)); setDialog(null); }} />}
          {dialog.kind === 'logout' && <AppConfirmDialog icon="logout" message="Bu cihazdaki oturum bilgileri güvenli biçimde silinir; kayıtlarınız sunucuda kalır." confirmLabel="Çıkış yap" onCancel={() => setDialog(null)} onConfirm={() => setDialog(null)} />}
          {dialog.kind === 'delete' && <AppConfirmDialog icon="delete_forever" message="Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir; geri getirilemez. Sonraki adımda parolanız sorulur." highlight="esnaf@ornek.com" confirmLabel="Devam et" destructive onCancel={() => setDialog(null)} onConfirm={() => setDialog(null)} />}
        </div>
      )}
    </div>
  );
}
Object.assign(window, { HesabimV5 });
