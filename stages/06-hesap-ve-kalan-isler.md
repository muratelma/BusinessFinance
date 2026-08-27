# Aşama 06 — Hesap ve kalan işler

## Belge durumu

- Durum: **Aktif** (27 Ağustos 2026'da kullanıcı onayıyla açıldı)
- Ön koşul: Aşama 05 — Vergi ve muhasebeci
- Sonraki aşama: Aşama 06.1 — Güvenlik taraması
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/variables.md`, `documentation/tests.md`,
  `documentation/local-setup-and-acceptance.md`, `documentation/adr/`
- Doğrulanmış ilerleme: `docs/project-status.md`
- **Açık kapsamlı aşamadır** (`stages/README.md`): iş listesi kullanıcı yeni bir
  şey söyledikçe büyür ve aşama liste bittiği için değil, kullanıcı kapatmak
  istediğinde kapanır. Bu, kalite kapılarını gevşetmez — her grup yine kendi
  checkpoint'idir.

## Bu aşamanın zincirdeki yeri

Ürün buluta çıkmadan önce **tamamlanma** turuna giriyor. Bu tur tek bir aşama
değil, bir aşama kümesidir:

| Aşama | İçerik |
|---|---|
| **06** | Kalan işler ve hesap kısmı — bu belge |
| 06.1 | Güvenlik taraması ve çıkan bulguların çözülmesi |
| 06.2 | Arayüz düzeni (frontend) |
| 06.x | Uygulama büyüdükçe çıkan yeni işler; ihtiyaç oldukça açılır |
| 07 | Bulut güvenli beta — **hepsi kapandıktan sonra** |

Bölmenin gerekçesi: her eklenen özellik kendi arkasında iş bırakıyor. Tek bir
dev aşama, listesi sürekli büyüdüğü için hiç kapanmaz; ayrı ayrı numaralanan
aşamalar kapanabilir ve yeni iş yeni bir numarayla açılır.

## Amaç

Beş aşama boyunca ürünün **finansal** tarafı kuruldu: kapsam boyutu, cari
hesap, yükümlülük, kasa/POS, vergi. İki şey geride kaldı:

1. **Kullanıcının kendisi.** Uygulamada profil ekranı yok. Kullanıcı kendi
   e-postasını göremiyor, parolasını değiştiremiyor, açık oturumlarını
   göremiyor, hesabını silemiyor, parolasını unutursa yapabileceği hiçbir şey
   yok. Kimlik altyapısı (JWT, refresh rotation, reuse tespiti) hazır; eksik
   olan bunun kullanıcıya açılan yüzü.
2. **Önceki aşamalardan kalan işler.** `docs/backlog.md` içinde dört madde
   duruyor: kırpılan kart alacağı, ayarlanamayan varsayılan kapsam, yapılmamış
   fiş kabul turu ve kapsamı netleşmemiş bütçe ekranı iyileştirmeleri. Hepsi
   hâlâ geçerli ve hiçbiri bir aşamaya bağlı değil.

## Kullanıcıya katkı

- Hesabına Özet ekranının sağ üstünden tek dokunuşla girmek
- Uygulamada kendi hesabını görmek: e-posta, işletme cevabı, açık oturumlar
- Parolasını uygulama içinden değiştirmek; değiştirdiğinde diğer cihazlardaki
  oturumların kapandığını görmek
- Parolasını unuttuğunda kendi başına sıfırlamak
- E-postasını doğrulayarak hesabını güvenceye almak
- Kendi hesabını ve verisini silebilmek
- Vadesi gelen fatura, vergi ve taksit için telefonundan hatırlatma almak
- Hesabına, kartına ve kategorisine "bu işletmenin" diyebilmek
- Kartını fazla ödediğinde parasının net varlıkta görünmesi

## Değiştirilmeyecek mimari kararlar

- Katmanlı monolit; mikroservis eklenmez.
- Kullanıcı kimliği `ICurrentUser` üzerinden JWT claim'inden gelir; hiçbir yeni
  uç kullanıcı kimliğini body veya query string'den almaz.
- Refresh token rotation ve reuse tespiti korunur; sunucuda yalnız hash saklanır.
- Kimlik doğrulama ve sıfırlama hataları aynı genel cevaba döner
  (enumeration önleme).
- İşletme ve şahsi tek havuzdur (ADR 0013); bakiye, kart borcu ve net varlık
  kapsam filtresinden etkilenmez.
- Ekonomik olay tanır, ödeme taşır (ADR 0014).
- Vergi alanları taşır, hesaplamaz (ADR 0016).
- Silme yerine iptal: finansal hareket fiziksel silinmez. **İstisna değil, ayrı
  bir konu**: kullanıcının kendi hesabının tamamen silinmesi bir finansal
  düzeltme değil, bir hesap kapatmadır ve Grup 1'de kendi kararına bağlanır.
- Mobil uygulamaya connection string veya sunucu secret'ı konmaz.
- Ürün bu aşamada da **internete açılmaz**: API ve SQL loopback'e bağlı kalır.

## Bu aşamanın dış servis sınırı

Bu aşama bir dış servis ekliyor ve bu "ürünü internete açmak" değildir:
uygulama hâlâ loopback'e bağlıdır, dışarıdan gelen hiçbir bağlantı kabul
edilmez. Eklenen şey **giden** bir HTTPS çağrısıdır.

- **Brevo (transactional e-posta).** E-posta doğrulama ve parola sıfırlama için.
  Ücretsiz katmanı günlük gönderim sınırıyla gelir ve geliştirme için fazlasıyla
  yeterlidir. Trade-off: kimlik akışına bir dış bağımlılık girer; servis
  ulaşılamazsa kayıt olan kullanıcı doğrulama postası alamaz. Bunun için
  doğrulama **hesabı kilitlemez** (Grup 2, "doğrulanmamış hesabın sınırı").
  Gönderici adresinin doğrulanması ve teslim edilebilirlik ayarları servisin
  kendi tarafında yapılır; kurulum adımları
  `documentation/local-setup-and-acceptance.md` içine yazılır.
- **Bağlantı değil kod gönderilir.** Bu aşamanın en kritik tasarım kaydı: API
  loopback'te olduğu için e-postaya konacak bir `https://…` bağlantısı
  telefondan **açılamaz**. Bu yüzden hem doğrulama hem sıfırlama, e-postayla
  gönderilen **altı haneli, süreli, tek kullanımlık bir kod** ve uygulamadaki
  bir kod alanı üzerinden yürür. Kod yolu bulutta da aynen çalışmaya devam
  eder; derin bağlantı (deep link) Aşama 07'nin isteğe bağlı bir iyileştirmesi
  olabilir, bu aşamanın işi değildir.
- **Firebase FCM bu aşamada eklenmez.** Gerekçe: FCM sunucudan cihaza push'tur
  ve **sunucu ayakta değilse hiçbir şey göndermez**. Uygulama bugün yalnız
  geliştirme makinesi açıkken çalışıyor; bilgisayar kapalıyken vadesi gelen
  faturanın bildirimi de gitmez. Bu hâliyle FCM, değerini teslim edemeyecek bir
  altyapı olur ve `CLAUDE.md` içindeki "sonraki aşamanın altyapısını erken
  ekleme" kuralına takılır. Üstüne Flutter tarafına Firebase Gradle eklentisi ve
  iki paketlik bir bağımlılık zinciri getirir.
  **Bu aşamada hatırlatma cihazda kurulur** (Grup 3): planlanan görünüm ve
  yaklaşan ödemeler zaten sunucudan geliyor; bildirim telefonun kendi
  zamanlayıcısına yazılır, internet gerektirmez ve bilgisayar kapalıyken de
  çalışır. FCM kararı Aşama 07'de, sunucu sürekli ayakta olduğunda yeniden
  açılır ve sonucu `PROJECT-ROADMAP.md` içine yazılır.

## Kapsam

### Dahil

- Hesap ve güvenlik ekranı: profil, parola değiştirme, oturumlar, hesap silme
- E-posta doğrulama ve parola sıfırlama (kod tabanlı, Brevo ile)
- Cihaz üstü hatırlatma (yerel bildirim)
- Hesap/kart/kategori varsayılan kapsamının uygulamadan ayarlanması
  (`docs/backlog.md` madde 5)
- Fazla ödenmiş kart bakiyesinin net varlıkta kaybolması hatası (madde 1)
- Fiş/dekont akışının cihaz kabul turu (madde 3)
- Bütçe ekranı iyileştirmelerinin somut listeye dönmesi ve yapılması (madde 2)

### Açıkça kapsam dışında

- **Güvenlik taraması ve bulguların çözülmesi** — Aşama 06.1.
- **Arayüz düzeni, gezinme ve frontend işleri** — Aşama 06.2.
- **Fiş okumada veri sınırı kararı** (`docs/backlog.md` madde 4) — Aşama 07'nin
  açılış kararıdır. Gerçek ve değerli belgeye geçiş kapısı orasıdır; karar
  oraya başlamadan yazılır.
- **Bulut, HTTPS, container, yönetilen veritabanı, dağıtım hattı, Google Play.**
  Hepsi Aşama 07. Bu aşamada Dockerfile, secret kasası, sertifika veya release
  imzalama işi yapılmaz.
- **Firebase FCM ve sunucu push'u** — yukarıdaki gerekçe; Aşama 07.
- **Telefon numarası ve ad/soyad alanı.** Bugün hiçbir akış tüketmiyor: SMS yok,
  karşı tarafla iletişim kuran bir yol yok. Taşıdığı bir şey olmayan alan
  eklenmez. İşletme unvanı gerçekten gerekirse (muhasebeci paketine veya
  faturaya basılacaksa) "profil süsü" olarak değil, işletme kimliği olarak ve
  kendi gerekçesiyle açılır.
- **İki faktörlü kimlik doğrulama.** Ayrı bir iş; e-posta doğrulama oturmadan
  açılmaz.
- Çoklu para birimi, offline yazma, banka bağlantısı — kalıcı olarak veya kendi
  kilometre taşlarında.

## Çalışma grupları

Sıra bağlayıcı değildir.

### Grup 1 — Hesap ve güvenlik ekranı

- `Diğer` altındaki dağınık hesap parçaları (işletme anahtarı, çıkış) tek bir
  **Hesabım** sayfasında toplanır; e-posta görünür olur.
- **Giriş noktası Özet ekranının sağ üstündedir**: `AppBar` action'ı olarak bir
  hesap ikonu, dokununca `Hesabım` sayfası. Gerekçesi üç katlı: Özet ana
  sekmedir, yani hesap her yerden bir dokunuş uzakta olur; sağ üst köşe her iki
  platformda da hesabın beklendiği yerdir; ve `Diğer` menüsünden bir satır
  düşer — bu, Aşama 06.2'nin hedefiyle aynı yöne çalışır.
  Çakışma yok: `Özet`'in `AppBar`'ı bugün `actions` taşımıyor ve **kapsam
  anahtarı AppBar'da değil**, altındaki ayrı barda ve sola hizalı duruyor
  (`dashboard_page.dart`, `_ScopeBar`). Anahtarın kaydırılan gövdenin dışında
  durma kuralı bozulmaz.
- İkon bir profil fotoğrafı gibi okunmamalı — uygulamada avatar yok. Dokunma
  hedefi ve ekran okuyucu etiketi tasarım sistemi kuralına uyar
  (`documentation/design-system.md`).
- **Doğrulanmamış e-postanın kalıcı uyarısı** (Grup 2) bu ikonun üstünde küçük
  bir nokta olarak taşınabilir: uyarının doğal yeri, gidip düzelteceği sayfanın
  kapısıdır.
- Sayfa `Diğer` menüsünde de kalır mı, yoksa yalnız bu ikondan mı girilir —
  **kararı Aşama 06.2 Grup 1 verir.** Bu aşamada ikisi birden durur; iki giriş
  noktası bir kusur değil, gezinme kararı verilene kadar geçici bir hâldir.
- Parola değiştirme: mevcut parola doğrulanır, yeni parola kayıt kurallarına
  uyar, başarı hâlinde **kullanıcının aktif oturumları kapanır** (mevcut cihaz
  dâhil mi hariç mi — kararı bu grupta verilir ve belgeye yazılır).
- Aktif oturumlar listesi: `RefreshSession` zaten sunucuda duruyor, eksik olan
  okuma ucu ve tek tek/toplu iptal. Oturum satırı hassas bilgi taşımaz (token
  veya hash gösterilmez).
- Hesabı silme: **ADR 0017 bu grupta yazılır** — hesabın silinmesi verinin
  gerçekten silinmesi mi, anonimleştirme mi. Bu karar "silme yerine iptal"
  finansal kuralıyla çelişmez ama onunla karıştırılmaya açıktır; yazılı olması
  şart. Silme geri döndürülemez olduğu için açık onay ve yeniden kimlik
  doğrulama ister; kullanıcıya önce **yedeğini alması** önerilir.
- Ölçüt: dört akış da Pixel 8'de çalışıyor; Özet'in sağ üstündeki ikon
  `Hesabım` sayfasını açıyor; parola değiştirme sonrası eski refresh token
  reddediliyor; başka kullanıcının oturumu listelenmiyor ve iptal edilemiyor
  (negatif test).

### Grup 2 — E-posta doğrulama ve parola sıfırlama

- Brevo yapılandırması `documentation/variables.md` envanterine girer; API
  anahtarı **yalnız** user-secrets'ta durur, repoya ve terminale yazılmaz.
- Gönderici Application katmanında bir port olarak soyutlanır; birim ve
  integration testlerinin **hiçbiri ağa çıkmaz**. Brevo'ya giden gerçek çağrı,
  `[SqlServerFact]` desenindeki gibi ortam değişkeni yokken **açıkça skip** olan
  tek bir sözleşme testinde durur.
- Kayıt sonrası doğrulama kodu gönderilir; **doğrulanmamış hesabın sınırı bu
  grupta tanımlanır ve belgeye yazılır.** Varsayılan öneri: hesap çalışır ama
  uygulama içinde kalıcı bir uyarı taşır — doğrulanmadı diye kilitlemek, dış
  servis çöktüğünde ürünü kullanılamaz hâle getirirdi.
- Parola sıfırlama: kod tek kullanımlık, süreli, kullanıldığında geçersiz;
  başarıyla sıfırlandığında **bütün aktif oturumlar kapanır**.
- İkisi de **enumeration sızdırmaz**: var olmayan e-posta ile var olan aynı
  cevabı verir.
- Mevcut auth rate limit'i (IP başına dakikada 10) bu iki yola da uygulanır;
  ayrıca e-posta başına gönderim aralığı konur (aynı adrese kod yağdırılamaz).
- Ölçüt: negatif testler enumeration, tekrar kullanım ve süre aşımı
  denemelerini reddediyor; test koşusu ağa çıkmadan yeşil.

### Grup 3 — Hatırlatma (cihaz üstü bildirim)

- Vadesi gelen yükümlülük, vergi takvimi kalemi, kart ekstresi ve taksit için
  telefonun kendi zamanlayıcısına yazılan yerel bildirim.
- Kaynak kanonik planlanan projection'dır; istemci ikinci bir vade mantığı
  kurmaz — sunucudan gelen listeyi zamanlayıcıya çevirir.
- Kullanıcı hangi türler için hatırlatma istediğini ve saatini seçer; hiç
  istemiyorsa hiç bildirim kurulmaz (izin istenmeden bildirim planlanmaz).
- Veri değişince (ödeme yapıldı, yükümlülük kapandı) kurulmuş bildirim **iptal
  edilir**; ödenmiş faturayı hatırlatmak güveni bozar.
- Bildirim gövdesinde **tutar ve karşı taraf adı geçmez** — kilit ekranında
  finansal veri görünür olurdu. Ne olduğunu söyler, ne kadar olduğunu değil.
- Ölçüt: bir günlük senaryoda hatırlatma kuruluyor, ödeme sonrası düşüyor, izin
  reddedildiğinde uygulama sessizce çalışmaya devam ediyor.

### Grup 4 — Varsayılan kapsamın uygulamadan ayarlanması

Devralınan açık iş (`docs/backlog.md` madde 5). `defaultScope` alanı API'de
oluşturma ve güncellemede var; Flutter ne gönderiyor ne gösteriyor.

- Hesap, kart ve kategori formlarına varsayılan kapsam alanı eklenir.
- Böylece kapsam türetme zincirinin **orta halkası** (kaynağın etiketi)
  uygulamadan kurulabilir; tek hesabına "dükkân kasası" diyen esnaf her kaydı
  tek tek işaretlemek zorunda kalmaz.
- Alan **nullable kalır**; boş olması meşrudur, eksik veri değildir.
- Ölçüt: hesabına varsayılan kapsam veren kullanıcının yeni kaydı, kategori ne
  derse desin hesabın etiketini alıyor; kapsamı görmeyen kullanıcıda alan hiç
  çıkmıyor.

### Grup 5 — Fazla ödenmiş kart bakiyesi

Devralınan açık iş (`docs/backlog.md` madde 1). Kart borcu
`Math.Max(0, harcama − ödeme)` ile kart başına kırpılıyor; kartı fazla ödeyen
kullanıcının alacaklı bakiyesi net varlıkta 0 sayılıyor.

- Bu, Aşama 02'de cari hesap için verilen "fazla tahsilat **kırpılmaz**"
  kararıyla doğrudan çelişiyor: aynı ürün aynı soruya iki farklı cevap veriyor.
- Kırpma kaldırılır; kartın alacaklı bakiyesi net varlığa girer. Arayüzde bu
  durumun nasıl anlatılacağına karar verilir ("borcunuz yok, kartınızda şu kadar
  alacağınız var").
- Gerçek veri gelmeden düzeltilmesi çok daha ucuz; sonrasında türetilen bütün
  tutarların anlamı değişir.
- Ölçüt: fazla ödeme senaryosu için birim + gerçek SQL testi; net varlık toplamı
  kart alacağını içeriyor; kırılan her mevcut test bilinçli olarak güncelleniyor.

### Grup 6 — Fiş/dekont akışının cihaz kabul turu

Devralınan açık iş (`docs/backlog.md` madde 3). Kod ve testler yerinde; eksik
olan cihaz doğrulaması.

- Belge yönünün altı yolu (harcama, gelir, iade, fatura ödeme, taksit, borç
  verme) Pixel 8'de tek tek koşulur.
- Öneri katmanının sınırı doğrulanır (ADR 0011): model yönü ve ödeme kaynağını
  seçmez, önerir; kullanıcının kabul/red rozeti görünür.
- **Yalnız sentetik fiş kullanılır.** Gerçek ve değerli belge, veri sınırı
  kararı yazılmadan (Aşama 07) gönderilmez.
- Ölçüt: altı yol da cihazda tamamlandı ve sonucu `docs/project-status.md`
  içine kaydedildi; bulunan her kusur ya düzeltildi ya backlog'a gerekçesiyle
  yazıldı.

### Grup 7 — Bütçe ekranı iyileştirmeleri

Devralınan açık iş (`docs/backlog.md` madde 2). Kayıtta "kapsamı belirsiz;
alınmadan önce kısa bir öneri listesine dönmesi gerekiyor" yazıyor.

- **Grubun ilk işi listeyi çıkarmaktır**: mevcut bütçe ekranı incelenir ve
  somut, tek tek yapılabilir maddeler önerilir. Liste kullanıcıyla onaylanmadan
  kod değişmez.
- Bütçenin hangi kapsamı sınırladığı sorusu Aşama 01'de cevaplandı (harcama
  **kategori + kapsam çiftiyle** toplanır); bu grup o kararı değiştirmez.
- Bütçe ekranının **görsel** düzeniyle ilgili çıkan maddeler Aşama 06.2'ye
  devredilebilir; buradaki iş bütçenin davranışıdır.
- Ölçüt: liste yazılı ve onaylı; onaylanan maddeler uygulandı veya gerekçesiyle
  backlog'a geri yazıldı.

## Zorunlu testler

### Domain / Application

- Parola değiştirme: yanlış mevcut parola reddedilir; başarı oturumları kapatır.
- Sıfırlama kodu: tek kullanım, süre aşımı, yanlış kod, yeniden üretim.
- Doğrulama kodu: tekrar gönderim aralığı, süresi geçmiş kod.
- Hesap silme: sahiplik, geri döndürülemezlik, bağlı kayıtların akıbeti.
- Fazla ödenmiş kart bakiyesi: net varlık ve kart borcu ayrı ayrı.
- Kapsam türetme zinciri kaynağın etiketiyle: hesap → kart → kategori sırası.

### API ve gerçek SQL

- Kullanıcı izolasyonu için pozitif **ve** negatif senaryo — yeni uçların
  hepsinde (oturumlar, parola değiştirme, hesap silme, doğrulama).
- Enumeration önleme: bilinmeyen e-posta ile bilinen aynı cevabı verir.
- Rate limit: auth uçlarının yeni yolları da sınırlı.
- Refresh token: parola değişince eski token reddediliyor.
- Hesap silme sonrası kullanıcının hiçbir kaydı okunamıyor.

### Flutter

- Yeni ekranlar için controller/repository testleri; loading, empty, error,
  unauthorized ve stale-cache durumları görünür.
- Özet'teki hesap ikonu: rota açılıyor, kapsam anahtarı yerinde kalıyor, ikon
  yükleme ve hata durumunda da görünüyor (AppBar gövdeden bağımsızdır).
- Kod alanı: yanlış kod, süresi dolmuş kod, tekrar gönder akışı.
- Bildirim: izin reddi, veri değişince iptal, hiç bildirim istememe.
- Varsayılan kapsam alanı: kapsamı görmeyen kullanıcıda hiç çıkmıyor.

## Kalite komutları

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
```

```bash
flutter analyze
flutter test
dart format --set-exit-if-changed lib test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

## Belge güncellemeleri

- `documentation/architecture.md`: e-posta gönderici portu ve güven sınırı;
  giden çağrının neden "internete açılmak" olmadığı
- `documentation/flows.md`: doğrulama, sıfırlama, parola değiştirme, hesap silme
  ve hatırlatma akışları
- `documentation/permissions.md`: yeni uçların sahiplik sınırı
- `documentation/variables.md`: Brevo yapılandırması ve secret envanteri
- `documentation/local-setup-and-acceptance.md`: gönderici kurulumu ve kabul
  adımları
- `documentation/tests.md`: yeni test kapsamı
- `documentation/adr/0017-*`: hesabın silinmesi (Grup 1'de yazılır)
- `docs/backlog.md`: kapanan maddeler (1, 2, 3, 5) satırdan düşer; madde 4
  Aşama 07'ye bağlandığı için "Aşamaya bağlananlar" bölümüne geçer
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

Henüz uygulanmamış davranış, uygulanmış gibi yazılmaz.

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Yalnız sentetik veri kullanılır; gerçek finansal veri Aşama 07'nin karar
  kapısı geçilmeden kullanılmaz.
- Brevo API anahtarı yalnız user-secrets'ta; repoya, terminale, loga ve belgeye
  yazılmaz.
- Doğrulama ve sıfırlama kodları sunucuda **açık saklanmaz** (hash'lenir),
  loglanmaz ve API cevabında dönmez.
- Bildirim gövdesinde tutar veya karşı taraf adı geçmez.
- API ve SQL loopback'e bağlı kalır; dışarıdan gelen bağlantı kabul edilmez.
- Test e-postası gerçek bir adrese gidecekse kullanıcının kendi adresidir.

## Yedek şeması

Bu aşama yedek şemasını **ilerletmez**. Eklenen alanların hiçbiri finansal kayıt
taşımıyor: oturum ve doğrulama kaydı kullanıcının kimliğine ait geçici durumdur,
yedeklenip geri yüklenecek bir geçmiş değildir. Sürüm v10'da kalır.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| E-posta servisinin kimlik akışını rehin alması | Doğrulanmamış hesap kilitlenmez; sınır Grup 2'de yazılı olarak tanımlanır |
| E-postadaki bağlantının loopback'ten açılamaması | Bağlantı değil **kod** gönderilir; kod yolu bulutta da çalışır |
| Parola sıfırlamanın hesap devralmaya açılması | Tek kullanım, süre sınırı, bütün oturumların kapanması, rate limit |
| Hesap silmenin yanlışlıkla tetiklenmesi | Yeniden kimlik doğrulama, açık onay, önce yedek alma önerisi |
| Kart kırpmasının kaldırılmasının mevcut testleri kırması | Kırılan her test bilinçli güncellenir ve gerekçesi commit'te yazılır |
| Bildirimin ödenmiş kalemi hatırlatması | Veri değişiminde kurulmuş bildirimin iptali zorunlu ölçüt |
| Bütçe grubunun kapsamsız büyümesi | Önce liste, sonra onay, sonra kod; onaysız madde uygulanmaz |
| Aşamanın açık kapsamlı olduğu için dağılması | Her grup kendi checkpoint'i; kapatma kararı kullanıcının |

## Çıkış koşulları

- [ ] Kullanıcının kapsama aldığı bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu, yeni uçların hepsinde negatif senaryolarla kanıtlandı.
- [ ] Enumeration önleme ve tek kullanımlık kod testleri geçti.
- [ ] `docs/backlog.md` maddeleri 1, 2, 3 ve 5 kapandı veya gerekçeyle yeniden
      yazıldı.
- [ ] ADR 0017 yazıldı ve kabul edildi.
- [ ] Pixel 8'de kabul turu tamamlandı (hesap akışları, fiş yönü, hatırlatma).
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 06.1'i açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
