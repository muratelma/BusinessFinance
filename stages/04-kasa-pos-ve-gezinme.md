# Aşama 04 — Kasa, POS ve gezinme

## Belge durumu

- Durum: Planlandı
- Ön koşul: Aşama 03 — Yükümlülük ve vade
- Sonraki aşama: Aşama 05 — Vergi ve muhasebeci
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/design-system.md`,
  `documentation/financial-activity-api-contract.md`,
  `documentation/restore-runbook.md`, **yeni ADR** (aşağıda)
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Tezgâh üstü esnafın günlük ritmi uygulamada yok. Gün sonunda kasayı sayar —
uygulamada karşılığı yok. Müşteri kartla öder, para birkaç gün sonra komisyon
düşülmüş olarak bankaya geçer — uygulamada karşılığı yok; üstelik uygulamadaki
`CreditCard` tam **tersi** şeydir, borçlandığın karttır.

Bu aşama günlük ritmi ekler ve gezinmeyi yeniden kurar. Gezinme burada, çünkü
kasa ekranının oturacağı yer burasıdır ve `İşlem ekle` menüsü tam bu aşamada
taşar.

## Kullanıcıya katkı

- Gün sonunda kasayı sayıp uygulamanın beklediği tutarla karşılaştırmak
- Kartla yapılan tahsilatı girmek; paranın ne zaman hesaba geçeceğini görmek
- POS komisyonunu gerçek gider olarak görmek — bugün görünmüyor
- Henüz hesaba geçmemiş parayı kullanılabilir bakiye sanmamak
- Aradığı ekranı dört sekmede bulmak; `İşlem ekle` menüsünde ne seçeceğini
  bilmek

## Değiştirilmeyecek mimari kararlar

- **Ekonomik olay tanır, ödeme taşır** (Aşama 02 ADR'si). POS tahsilatı bu
  kuralın kapsamındadır.
- Bakiye kalıcı kolon değildir; bloke tutar da kalıcı kolon değildir.
- Kapsam boyutu ve türetme zinciri (ADR 0013). Onboarding ön ayarı **özellik
  kapatmaz**.
- Net varlık borcu anaparayla ölçer (ADR 0010).
- Silme yerine iptal.
- Tasarım sistemi token'ları ve erişilebilirlik kapısı.

## Bu aşamanın karar kapısı: "kredi kartı" iki şeydir

Aşama başlamadan **bir ADR yazılır.** Bugünkü `CreditCard` borçlandığın karttır;
esnafın gündelik dilinde "kart" ise tahsilat aracıdır. İkisi aynı ekranda aynı
kelimeyle görünürse kullanıcı hangi yöne baktığını bilemez.

ADR'nin karara bağlayacağı üç şey:

1. Arayüz adlandırması: borç tarafı ve tahsilat tarafı **hangi kelimelerle**
   ayrılır.
2. POS tahsilatının bir hesap türü **olmadığı**: bloke edilmiş para bir
   projection'dır, `AccountType`'a yeni değer eklenmez.
3. Kullanılabilir bakiye ile net varlığın farkı: yoldaki para net varlığa
   girer, kullanılabilir bakiyeye girmez.

## Kapsam

### Dahil

- Gün sonu nakit sayımı ve fark kaydı
- POS tahsilatı: brüt tutar, komisyon, net tutar, hesaba geçiş
- Bloke (yoldaki) tutarın projection olarak hesaplanması
- Ana sekme yapısının yeniden kurulması
- `İşlem ekle` menüsünün niyet eksenine geçmesi
- Yedek şeması v9

### Açıkça kapsam dışında

- Gerçek POS cihazı veya banka entegrasyonu — **kapsam dışı**, veriler elle
  girilir
- Z raporu okuma, yazarkasa entegrasyonu
- Vardiya, kasiyer, personel ayrımı
- Otomatik komisyon oranı çekme; oran kullanıcıdan alınır
- KDV alanları — Aşama 05

## Çalışma grupları

### Grup 1 — ADR: kart borcu ile kart tahsilatı ayrımı

- Yukarıdaki üç karar yazılır; reddedilen seçenekler (yeni `AccountType`,
  bloke tutarın kalıcı kolon olması) gerekçeleriyle kaydedilir.
- Ölçüt: ADR kabul edildi ve `CLAUDE.md` belge haritasına eklendi.

### Grup 2 — Domain: gün sonu kasa sayımı

- `CashCount`: tarih, nakit hesabı, sayılan tutar, kapsam, not. **Beklenen
  tutar saklanmaz** — sayım anındaki bakiye projection'dan okunur ve fark
  türetilir.
- Fark **otomatik düzeltme hareketi üretmez.** Sayım hatasını gerçek para
  hareketi gibi yazmak, olmamış bir gideri kayda geçirmek olurdu.
- Kullanıcı açıkça onaylarsa fark, `Kasa farkı` kategorisiyle bir gelir/gider
  kaydı üretir. Bu ikinci ve ayrı bir eylemdir.
- Aynı gün ve aynı hesap için ikinci sayım öncekini iptal eder, üzerine
  yazmaz.
- Ölçüt: fark doğru hesaplanıyor; onaysız hiçbir finansal kayıt üretilmiyor.

### Grup 3 — Domain: POS tahsilatı

- `PosSettlement`: tahsilat tarihi, brüt tutar, komisyon oranı veya tutarı,
  net tutar, beklenen geçiş tarihi, hedef hesap, gerçekleşme durumu, kapsam.
- **Ekonomik olay tahsilat anındadır:** gelir brüt tutar kadar tanınır,
  komisyon gider olarak yazılır. Hesap bakiyesi **değişmez**.
- **Geçiş anında:** hedef hesap net tutar kadar artar, gelir/gider **sıfır**.
  Aynı satışın ikinci kez sayılması böyle önlenir.
- Komisyon brüt tutardan **ayrı okunur ve ona eklenmez** — dekonttaki işlem
  ücretinde öğrenilen dersin aynısı.
- Bloke (yoldaki) tutar = geçmemiş tahsilatların net toplamı; **projection**.
- Ölçüt: tahsilat + geçiş senaryosunda gelir bir kez, komisyon bir kez sayılıyor.

### Grup 4 — Bakiye, net varlık ve Özet ekranı

- Kullanılabilir bakiye: yoldaki parayı **içermez**.
- Net varlık: yoldaki parayı **içerir**, ayrı satır olarak gösterilir.
- Özet ekranına "yolda olan" satırı eklenir; gün sonu farkı varsa uyarı bandına
  düşer.
- Ölçüt: iki sayı arasındaki fark tam olarak yoldaki tutara eşit.

### Grup 5 — Ana sekme yapısı

- Bugünkü sekmeler: `Özet · İşlemler · Bütçeler · Diğer`. `Bütçeler` ana sekme
  olarak bir ev bütçesi kavramı; işletme kullanıcısının ikinci ekranı kasadır.
- **Önerilen karar:** üçüncü sekme onboarding ön ayarına göre değişir —
  işletme kullanıcısında `Kasa`, kişisel kullanıcıda `Bütçeler`. Yerini
  değiştiren sekme **kaybolmaz**, `Diğer` altına iner.
- Bu, ADR 0013'ün "ön ayar özellik kapatmaz" kuralını bozmaz: hiçbir özellik
  kapanmıyor, yalnız hangisinin bir dokunuş uzakta olduğu değişiyor. Kararın
  kendisi Grup 1 ADR'sine yazılır.
- Ölçüt: iki profilde de dört sekme dolu ve hiçbir ekran erişilemez değil.

### Grup 6 — `İşlem ekle` menüsünün yeniden kurgusu

- Bugün yedi düz seçenek: gider, fiş, gelir, dekont, transfer, kart ödemesi,
  tekrarlayan plan. Bu aşamada kasa ve POS eklenince liste taşıyor.
- Düz liste yerine **niyet ekseni**: para girdi / para çıktı / belge okut /
  plan kur. Alt seçenekler bu dört başlığın altına dağılır.
- Menü hâlâ tek giriş noktasıdır; shell ve İşlemler ekranındaki iki eylem aynı
  launcher'ı açmaya devam eder.
- Ölçüt: sekiz üstü seçenek tek ekranda kaydırmadan okunabiliyor; erişilebilirlik
  kapısı geçiyor.

### Grup 7 — Flutter ekranları

- `features/cash/`: gün sonu sayım ekranı, geçmiş sayımlar, fark kaydı onayı.
- POS tahsilatı girişi ve bekleyen geçişler listesi; geçişi işaretleme.
- `Diğer` menüsü yeni yerleşime göre düzenlenir.
- `FinancialDataChanges` yeni hedefler alır; kasa sayımı bütçeyi yükseltmez.
- Ölçüt: zorunlu ekran durumları; tasarım sistemi kontrol listesi tamamlandı.

### Grup 8 — Yedek v9 ve sözleşme belgeleri

- Şema **v9** yazar, yalnız v9 okur.
- Feed sözleşmesi iki yeni kaynakla genişletilir.
- `documentation/design-system.md` yeni bileşenlerle güncellenir.
- Ölçüt: sayımlar ve tahsilatlar kayıpsız geri yükleniyor.

## Zorunlu testler

### Domain / Application

- Kasa farkı otomatik finansal kayıt üretmez; onaylanınca tek kayıt üretir.
- Aynı gün ikinci sayım öncekini iptal eder.
- POS tahsilatı geliri tanır, hesabı değiştirmez; geçiş hesabı değiştirir,
  geliri **tekrar tanımaz**.
- Komisyon brüt tutara eklenmez; ayrı gider olarak görünür.
- Yoldaki tutar projection'dır; kalıcı kolon yok.

### API ve gerçek SQL

- Kullanıcı izolasyonu; pozitif **ve** negatif senaryo.
- Kullanılabilir bakiye ile net varlık farkı = yoldaki tutar.
- Feed ve planlanan görünüm tek sorguda kalır.
- Yedek v9 yazma/okuma; v8 reddi.

### Flutter

- Gün sonu akışı: sayım gir, farkı gör, onayla veya onaylama.
- POS tahsilatı gir, geçişi işaretle; bakiyenin ne zaman değiştiğini doğrula.
- Sekme yapısı iki profilde de doğru.
- `İşlem ekle` menüsünün yeni yerleşimi ve erişilebilirliği.

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

- Yeni ADR: kart borcu ile kart tahsilatı ayrımı, sekme kararı
- `documentation/architecture.md`: kasa, POS ve bloke projection'ı
- `documentation/flows.md`: gün sonu, POS tahsilatı ve geçiş akışları
- `documentation/permissions.md`: yeni endpoint'ler
- `documentation/design-system.md`: yeni bileşenler ve gezinme
- `documentation/financial-activity-api-contract.md`, `tests.md`,
  `restore-runbook.md`
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Yalnız sentetik veri kullanılır; gerçek POS verisi girilmez.
- Banka veya POS sağlayıcısına bağlantı kurulmaz.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| POS tahsilatının satışı iki kez saydırması | Tanıma/geçiş ayrımı testi, çıkış koşulu |
| Yoldaki paranın kullanılabilir bakiyeye karışması | İki sayının farkını doğrulayan test |
| Kasa farkının sessizce finansal kayda dönüşmesi | Onaysız kayıt üretilmediğini doğrulayan test |
| Sekme değişikliğinin bir ekranı erişilemez bırakması | İki profilde de bütün ekranların erişilebilirliği testi |
| Menü yeniden kurgusunun mevcut akışları kırması | Tek launcher kuralı korunur; yönlendirme testleri |
| "Kredi kartı" kelimesinin iki yerde birden kalması | ADR'de adlandırma kararı; arayüz metni testi |

## Çıkış koşulları

- [ ] ADR yazıldı ve kabul edildi.
- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Bir günlük perakende senaryosu girildi: gün sonu farkı doğru, POS parası
      geçene kadar kullanılabilir bakiye şişmiyor, komisyon gider olarak
      görünüyor.
- [ ] İki profilde de dört sekme dolu ve hiçbir ekran erişilemez değil.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 05'i açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
