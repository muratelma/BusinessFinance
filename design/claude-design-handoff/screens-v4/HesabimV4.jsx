// Hesabım — kaynak: account_page.dart. Sıra tehlikeye göre: kim olduğunuz,
// nereden açık olduğunuz, en sonda geri dönüşü olmayan eylem.
function HesabimV4({ onBack, initialScroll = 0, initialVerified = false }) {
  const { AppCard, AppSwitch, AppSubmitButton, AppConfirmDialog, AppStatusChip, Icon } = DS;
  const [verified, setVerified] = React.useState(initialVerified);
  const [business, setBusiness] = React.useState(true);
  const [sessions, setSessions] = React.useState([
    { id: 'a', current: true, opened: '22.09.2026 09:14', until: '22.10.2026' },
    { id: 'b', opened: '18.09.2026 21:40', until: '18.10.2026' },
    { id: 'c', opened: '02.09.2026 11:05', until: '02.10.2026' },
  ]);
  const [dialog, setDialog] = React.useState(null);
  const bodyRef = React.useRef(null);
  React.useEffect(() => { if (bodyRef.current) bodyRef.current.scrollTop = initialScroll; }, []);
  const chev = <Icon name="chevron_right" size={22} color="var(--ink-muted)" />;
  const gap = { height: 'var(--space-md)' };
  return (
    <div className="screen">
      <TopBar title="Hesabım" onBack={onBack} />
      <div className="body" ref={bodyRef} style={{ padding: 'var(--space-xs) var(--space-md) var(--space-xl)' }}>
        <AppCard padding="var(--space-md)">
          <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
            <span style={{ width: 48, height: 48, borderRadius: '50%', background: 'var(--brand)', color: 'var(--on-brand)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center', font: '600 17px/1 var(--font-core)', flex: '0 0 auto' }}>ME</span>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 'var(--space-xs)' }}>
              <span style={LBL}>E-posta</span>
              <span style={{ ...ROWT, overflow: 'hidden', textOverflow: 'ellipsis' }}>esnaf@ornek.com</span>
              <span style={HELP}>Hesap açılışı: 14 Mart 2026</span>
            </span>
          </div>
          {verified && <div style={{ marginTop: 'var(--space-md)' }}><AppStatusChip label="E-posta doğrulandı" icon="check_circle" tone="income" /></div>}
        </AppCard>
        {!verified && <>
          <div style={gap} />
          <AppCard padding="var(--space-md)">
            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-sm)' }}>
              <Icon name="mark_email_unread" size={22} color="var(--ink)" />
              <span style={ROWT}>E-posta adresiniz doğrulanmadı</span>
            </div>
            <p style={{ ...HELP, margin: 'var(--space-xs) 0 var(--space-md)', textWrap: 'pretty' }}>Uygulamayı kullanmaya devam edebilirsiniz; doğrulama hesabınızı güvenceye alır ve parolanızı unuttuğunuzda geri almanızı sağlar.</p>
            <AppSubmitButton label="Adresimi doğrula" icon="mark_email_read" variant="tonal" onSubmit={() => setVerified(true)} />
          </AppCard>
        </>}
        <div style={gap} />
        <AppCard padding="var(--space-md)">
          <div style={{ display: 'flex', alignItems: 'flex-start', gap: 'var(--space-md)' }}>
            <span style={{ flex: 1, minWidth: 0, display: 'flex', flexDirection: 'column', gap: 'var(--space-xs)' }}>
              <span style={ROWT}>İşletmem var</span>
              <span style={{ ...HELP, textWrap: 'pretty' }}>Açıkken kayıtlarınızı işletme ve şahsi olarak ayrı okuyabilirsiniz. Kapalıyken bu ayrım hiç görünmez. Kategorileriniz iki durumda da olduğu gibi kalır.</span>
            </span>
            <AppSwitch value={business} onChange={setBusiness} label="İşletmem var" />
          </div>
        </AppCard>
        <Section title="Açık oturumlar">
          <AppCard padding={0}>
            <RowList inset={72}>
              {sessions.map((s) => (
                <Row key={s.id} lead={<Capsule icon={s.current ? 'smartphone' : 'devices'} />} title={s.current ? 'Bu cihaz' : 'Başka bir cihaz'} subtitle={'Açılış: ' + s.opened + ' · Geçerlilik: ' + s.until}
                  trailing={s.current ? <AppStatusChip label="Şu an" icon="check_circle" tone="neutral" /> : <ToolButton icon="close" label="Oturumu kapat" onClick={() => setDialog({ kind: 'revoke', s })} />} pad="12px 4px 12px 16px" />
              ))}
            </RowList>
          </AppCard>
        </Section>
        <Section title="Güvenlik">
          <AppCard padding={0}>
            <RowList inset={72}>
              <Row lead={<Capsule icon="password" />} title="Parolamı değiştir" subtitle="Değiştirdiğinizde diğer cihazlardaki oturumlar kapanır." trailing={chev} onClick={() => {}} pad="12px 12px 12px 16px" />
              <Row lead={<Capsule icon="logout" />} title="Çıkış yap" subtitle="Bu cihazdaki güvenli oturum kapatılır." onClick={() => setDialog({ kind: 'logout' })} pad="12px 12px 12px 16px" />
            </RowList>
          </AppCard>
        </Section>
        <div style={{ height: 'var(--space-lg)' }} />
        <AppCard padding="var(--space-md)">
          <span style={{ ...ROWT, color: 'var(--expense)' }}>Hesabı kapat</span>
          <p style={{ ...HELP, margin: 'var(--space-xs) 0 var(--space-sm)', textWrap: 'pretty' }}>Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir. Bu işlem geri alınamaz.</p>
          <button type="button" onClick={() => setDialog({ kind: 'delete' })} style={{ display: 'inline-flex', alignItems: 'center', gap: 'var(--space-sm)', minHeight: 48, padding: '0 var(--space-md) 0 var(--space-sm)', margin: '0 0 0 calc(-1 * var(--space-sm))', border: 'none', borderRadius: 'var(--radius-field)', background: 'transparent', color: 'var(--expense)', font: '600 15px/1 var(--font-core)', cursor: 'pointer' }}>
            <Icon name="delete_forever" size={20} color="var(--expense)" />Hesabımı sil
          </button>
        </AppCard>
      </div>
      {dialog && (
        <div className="sheet-scrim" style={{ alignItems: 'center', justifyContent: 'center', padding: 'var(--space-lg)' }}>
          {dialog.kind === 'revoke' && <AppConfirmDialog icon="no_accounts" message="Oturum kapatılsın mı? O cihaz bir sonraki denemesinde yeniden giriş yapmak zorunda kalır." highlight={'Açılış: ' + dialog.s.opened} confirmLabel="Kapat" destructive onCancel={() => setDialog(null)} onConfirm={() => { setSessions(sessions.filter((x) => x.id !== dialog.s.id)); setDialog(null); }} />}
          {dialog.kind === 'logout' && <AppConfirmDialog icon="logout" message="Çıkış yapılsın mı? Bu cihazdaki oturum bilgileri güvenli biçimde silinecek." confirmLabel="Çıkış yap" onCancel={() => setDialog(null)} onConfirm={() => setDialog(null)} />}
          {dialog.kind === 'delete' && <AppConfirmDialog icon="delete_forever" message="Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir; geri getirilemez. Silmeden önce Diğer › Veri ve yedek adımından yedeğinizi almanız önerilir." highlight="esnaf@ornek.com" confirmLabel="Devam et" destructive onCancel={() => setDialog(null)} onConfirm={() => setDialog(null)} />}
        </div>
      )}
    </div>
  );
}
Object.assign(window, { HesabimV4 });
