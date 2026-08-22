# Aşama 06 — Bulut güvenli beta

## Belge durumu

- Durum: Planlandı
- Ön koşul: Aşama 05 — Vergi ve muhasebeci
- Sonraki aşama: Henüz açılmadı (kilometre taşları `PROJECT-ROADMAP.md` içinde)
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/variables.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/restore-runbook.md`,
  `documentation/local-setup-and-acceptance.md`
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Uygulama bugün yalnız geliştirme makinesinde çalışıyor: API loopback'e bağlı,
SQL Server yerel Docker container'ında, bilgisayar kapalıyken uygulama hiçbir
şey göremiyor. Bu hâliyle ürünün gerçek bir kullanıcısı olamaz.

Bu aşama ürünü kendi makinenden ayırıyor ve gerçek finansal veriye geçiş
kapısını açıyor. **Kapı bu aşamanın kendisidir**: bugüne kadar bütün aşamalar
"gerçek veri kullanılmaz" kuralıyla çalıştı; o kural ancak buradaki kontroller
geçtikten sonra gevşer.

## Kullanıcıya katkı

- Uygulamayı bilgisayar kapalıyken de kullanmak
- Parolasını unuttuğunda kendi başına sıfırlamak
- E-postasını doğrulayarak hesabını güvenceye almak
- Verisinin düzenli ve doğrulanmış biçimde yedeklendiğini bilmek
- Google Play üzerinden kurulum yapmak; APK dosyası taşımamak

## Değiştirilmeyecek mimari kararlar

- Katmanlı monolit; mikroservis eklenmez.
- Kullanıcı izolasyonu: kimlik JWT claim'den gelir, her sorgu owner-scoped.
- Refresh token rotation ve reuse tespiti.
- Kimlik doğrulama hatalarının aynı genel cevaba dönmesi (enumeration önleme).
- Bakiye ve türetilen tutarlar kalıcı kolon değildir.
- Mobil uygulamaya connection string veya sunucu secret'ı konmaz.

## Bu aşamanın karar kapısı: gerçek veriye geçiş

Aşağıdakilerin **tamamı** doğrulanmadan hiçbir gerçek finansal veri girilmez ve
hiçbir dış test kullanıcısı davet edilmez. Bu liste aşamanın çıkış koşulundan
ayrıdır ve ondan daha katıdır:

- HTTPS zorunlu, düz HTTP kapalı
- Secret'lar yönetilen kasada; repoda ve istemcide yok
- Kullanıcı izolasyonu negatif testlerle bulut ortamında da kanıtlı
- Rate limiting açık ve ölçülmüş
- Otomatik yedek çalışıyor **ve** geri yükleme tatbikatı yapılmış
- Hassas finansal veri loglara yazılmıyor
- KVKK metni ve mağaza gizlilik politikası hazır

## Kapsam

### Dahil

- API'nin container olarak yayımlanması
- Yönetilen veritabanı ve bağlantı güvenliği
- Secret yönetimi
- HTTPS ve sertifika
- E-posta doğrulama ve parola sıfırlama
- İzleme, log ve hata takibi
- Otomatik yedek ve geri yükleme tatbikatı
- Belge eklerinin yönetilen object store'a taşınması
- Dağıtım hattı ve geri alınabilir sürüm
- Google Play kapalı test

### Açıkça kapsam dışında

- **Offline yazma ve senkronizasyon.** Bu aşamada eklenmez, ama kararı burada
  yeniden açılır: kasa ve POS buluta çıktığında dükkânda bağlantı kesikken
  satış kaydedilemiyorsa uygulama o an işe yaramaz. Değerlendirme sonucu
  `PROJECT-ROADMAP.md` içine yazılır.
- iOS uygulaması ve Play production yayını
- Çoklu bölge, yüksek erişilebilirlik, otomatik ölçeklenme
- Push bildirimleri (altyapı kurulur, kullanıcıya görünen bildirim ayrı iş)
- Banka bağlantısı — kalıcı olarak kapsam dışı

## Çalışma grupları

### Grup 1 — Yapılandırma ve secret sınırı

- Bütün secret'lar yönetilen kasaya taşınır; `documentation/variables.md`
  envanteri bulut kaynaklarıyla güncellenir.
- Yerel geliştirme user-secrets ile çalışmaya devam eder; iki ortam aynı kod
  yolunu kullanır.
- Secret rotation prosedürü yazılır.
- Ölçüt: repoda ve APK'da hiçbir secret yok; secret taraması temiz.

### Grup 2 — Container yayını ve veritabanı

- API container imajı üretilir ve yayımlanır.
- Yönetilen veritabanı kurulur; bağlantı yalnız uygulamadan, şifreli.
- Migration'lar dağıtımın kontrollü bir adımıdır; uygulama açılışında sessizce
  koşmaz.
- Ölçüt: temiz ortama sıfırdan dağıtım yapılabiliyor ve geri alınabiliyor.

### Grup 3 — HTTPS ve ağ sınırı

- HTTPS zorunlu; düz HTTP yönlendirilir veya reddedilir.
- Flutter `API_BASE_URL` bulut adresine yönlendirilir; debug HTTP yalnız yerel
  profilde kalır.
- Ölçüt: düz HTTP isteği kabul edilmiyor.

### Grup 4 — E-posta doğrulama ve parola sıfırlama

- Kayıt sonrası e-posta doğrulama; doğrulanmamış hesabın sınırı tanımlanır.
- Parola sıfırlama: tek kullanımlık, süreli, kullanıldığında geçersiz.
- Her ikisi de **enumeration sızdırmaz**: var olmayan e-posta ile var olan
  aynı cevabı verir.
- Sıfırlama başarılı olduğunda kullanıcının **bütün aktif oturumları kapanır**.
- Auth uçlarındaki mevcut rate limit bu iki yola da uygulanır.
- Ölçüt: negatif testler enumeration ve tekrar kullanım denemelerini reddediyor.

### Grup 5 — İzleme ve log sınırı

- Uygulama telemetrisi, hata takibi ve temel metrikler.
- **Hassas finansal veri loglanmaz**: tutar, karşı taraf adı, açıklama, token
  ve e-posta log gövdesine girmez.
- Sağlık uçları: canlılık ve hazır olma ayrı.
- Ölçüt: log örneklemesinde hiçbir finansal değer görünmüyor.

### Grup 6 — Yedek, geri yükleme ve belge depolama

- Otomatik yedek planı ve saklama süresi.
- **Geri yükleme tatbikatı yapılır ve kaydedilir**; yedeğin varlığı yeterli
  sayılmaz.
- Belge ekleri yönetilen object store'a taşınır; erişim yalnız sahibine, süreli
  imzalı bağlantıyla.
- `documentation/restore-runbook.md` bulut prosedürüyle genişletilir.
- Ölçüt: yedekten geri yükleme başarılı ve adım adım belgeli.

### Grup 7 — Dağıtım hattı

- Ana branch kalite kapıları dağıtımın ön koşulu olur.
- Sürüm geri alınabilir; hangi imajın canlı olduğu izlenebilir.
- Ölçüt: bir sürüm dağıtılıp geri alınabiliyor.

### Grup 8 — Google Play kapalı test

- Release imzalama anahtarı üretilir ve **repo dışında** saklanır.
- Kapalı test sürümü yüklenir; kritik akışlar gerçek cihazda koşulur.
- Mağaza gizlilik metni ve KVKK aydınlatma metni hazırlanır.
- Ölçüt: kapalı testte kayıt, giriş, işlem ekleme, cari, fatura, kasa ve ay
  sonu paketi akışları tamamlanıyor.

## Zorunlu testler

### API ve gerçek SQL

- Kullanıcı izolasyonu bulut ortamında da pozitif **ve** negatif senaryoyla
  kanıtlanır.
- Parola sıfırlama: tek kullanım, süre aşımı, oturum kapatma.
- E-posta doğrulama: tekrar gönderim, süresi geçmiş bağlantı.
- Enumeration önleme: bilinmeyen e-posta ile bilinen aynı cevabı verir.
- Rate limit sınırları ölçülür.

### Sistem

- Sıfırdan dağıtım ve geri alma.
- Yedekten geri yükleme tatbikatı.
- Secret taraması.
- Log örneklemesinde hassas veri araması.

### Flutter

- Bulut adresine karşı gerçek API integration akışı.
- Oturum süresi dolduğunda unauthorized durumunun görünür ele alınması.
- Release build ve erişilebilirlik kontrolü.

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
flutter build appbundle --release
```

## Belge güncellemeleri

- `documentation/architecture.md`: bulut topolojisi ve güven sınırları
- `documentation/variables.md`: bulut yapılandırması ve secret envanteri
- `documentation/permissions.md`: doğrulama ve sıfırlama uçlarının sınırı
- `documentation/restore-runbook.md`: bulut yedek ve geri yükleme
- `documentation/local-setup-and-acceptance.md`: yerel ile bulut ayrımı
- `documentation/tests.md`: bulut test kapsamı
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Gerçek finansal veri, yukarıdaki karar kapısının **tamamı** geçilmeden
  kullanılmaz.
- Secret'lar yalnız yönetilen kasada; terminale, loga ve belgeye yazılmaz.
- İmzalama anahtarı repoya girmez.
- Belge eklerine erişim yalnız sahibine ve süreli.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| Secret'ın repoya veya istemciye sızması | Secret taraması kalite kapısında; APK içeriği kontrolü |
| Yedeğin var olup geri yüklenememesi | Tatbikat zorunlu ve belgeli; yedeğin varlığı yeterli sayılmaz |
| Hassas finansal verinin loglara düşmesi | Log örneklemesi testi; alan bazlı maskeleme |
| Parola sıfırlamanın hesap devralmaya açılması | Tek kullanım, süre sınırı, bütün oturumların kapanması |
| Dükkânda bağlantı kesikken satış kaydedilememesi | Offline yazma kararı bu aşamada yeniden açılır ve roadmap'e yazılır |
| Migration'ın canlı veritabanında beklenmedik davranması | Dağıtımın kontrollü adımı; `AGENTS.md` migration kuralları |

## Çıkış koşulları

- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Karar kapısındaki maddelerin **tamamı** doğrulandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve release build kontrolleri geçti.
- [ ] Uygulama, geliştirme bilgisayarı kapalıyken çalışıyor.
- [ ] Yedekten geri yükleme tatbikatı başarılı ve belgeli.
- [ ] Secret istemciye veya repoya girmiyor; tarama temiz.
- [ ] Kapalı testte kritik kullanıcı akışları gerçek cihazda tamamlandı.
- [ ] Offline yazma kararı yeniden değerlendirildi ve sonucu roadmap'e yazıldı.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı sonraki kilometre taşını açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
