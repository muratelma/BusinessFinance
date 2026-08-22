# Aşama 01 — Kapsam boyutu ve işletme kimliği

## Belge durumu

- Durum: Planlandı
- Ön koşul: Yok — zincirin ilk aşaması
- Sonraki aşama: Aşama 02 — Cari hesap: karşı taraf ve açık bakiye
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/restore-runbook.md`,
  `documentation/design-system.md`
- Kurucu karar: `documentation/adr/0013-business-and-personal-are-one-pool.md`
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Devralınan kod tabanı bir **ev bütçesi** uygulaması. Hedef kitle şahıs şirketi
ve esnaf olduğu hâlde uygulama, bir kaydın işletmeye mi sahibinin cebine mi ait
olduğunu bilmiyor; yeni kullanıcıyı `Market Alışverişi`, `Evcil hayvan`,
`Kişisel bakım` gibi otuz ev kategorisiyle karşılıyor ve ilk gelir kategorisi
`Maaş` — esnafın maaşı yoktur.

Bu aşama, ürünün kimliğini kuran boyutu ekliyor: her finansal kayıt kapsamını
bilir, raporlar bölünebilir hale gelir, bakiye tek kalır.

## Kullanıcıya katkı

Aşama bittiğinde esnaf şunu yapabiliyor olacak:

- Kaydolurken *"işletmeniz var mı?"* sorusuna cevap verip işletme kategori
  setiyle başlamak
- Dükkân kasasından girdiği kaydın kapsamını hiç dokunmadan doğru bulmak;
  istisnayı tek dokunuşla düzeltmek
- Özet ekranının başındaki `Hepsi · İşletme · Şahsi` anahtarıyla aynı ayı üç
  farklı soruyla okumak
- "Bu ay dükkân ne kazandı, ben ne harcadım, kasada ne kaldı" cümlesini tek
  ekranda kurmak — bugün kurulamıyor

## Değiştirilmeyecek mimari kararlar

Bu aşama aşağıdakilerin hiçbirini bozmaz; bozması gerekirse önce ADR yazılır:

- **Bakiye kalıcı kolon değildir** — açılış bakiyesi + iptal edilmemiş
  hareketlerden hesaplanır. Kapsam bunu değiştirmez.
- **Bakiye, kart borcu ve net varlık bölünmez.** Kapsam anahtarı hangi konumda
  olursa olsun bu üçü toplamı gösterir (ADR 0013).
- **Silme yerine iptal.** Kapsam düzeltmesi de kayıt silmez.
- **Transfer ve kart ödemesi gelir/gider değildir** (ADR 0002, ADR 0003); bu
  yüzden kapsam da taşımazlar.
- **Kategori kimlik taşımaz.** Kapsam kategoriyle temsil edilmez, ayrı
  boyuttur.
- **Para dört ondalıklı**, API sözleşmesinde string.
- Domain katmanı framework bağımsız kalır.

## Kapsam

### Dahil

- Veri sıfırlama ve temiz zemin
- `TransactionScope` boyutu ve onu taşıyan yazma modelleri
- Hesap, kart ve kategoride isteğe bağlı varsayılan kapsam
- Kapsam türetme zinciri (form → hesap/kart → kategori)
- İşletme varsayılan kategori seti ve onboarding ön ayarı
- Kapsama duyarlı okuma modelleri, raporlar ve bütçeler
- Flutter: global kapsam anahtarı, formda kapsam çipi
- Özet ekranı hero metriğinin yeniden tasarımı
- Yedek şeması v6 ve CSV dışa aktarmada kapsam

### Açıkça kapsam dışında

- Karşı taraf / cari hesap — Aşama 02
- Fatura, vade ve tek seferlik yükümlülük — Aşama 03
- Gün sonu kasa, POS tahsilatı, ana sekme yapısı — Aşama 04
- KDV alanları, **indirilebilirlik bayrağı**, vergi takvimi — Aşama 05
  (kapsamla birleştirilmez; ADR 0013 madde 5)
- Tasarruf hedeflerine kapsam eklenmesi — Aşama 05'te "vergi karşılığı" ile
  birlikte
- LTD/AŞ ayrımı, ortak cari hesabı, personel, stok

## Çalışma grupları

### Grup 1 — Veri sıfırlama ve temiz zemin

- Yerel veritabanı düşürülür ve `InitialCreate` ile sıfırdan kurulur.
  **Geri alınamaz; uygulanmadan önce kullanıcıdan ayrıca onay alınır.**
- Gerekçe: mevcut 28 tablodaki her kayıt kişisel bütçe uygulamasından
  kopyalanmış sentetik veri. Korunacak bir şey yok ve boş tablolar, Grup 4'ün
  `NOT NULL` kolonlarını backfill'siz eklemesini mümkün kılıyor.
- `manual-test-data/` içeriği yeni ürün yönüne göre gözden geçirilir.
- Ölçüt: temiz veritabanı ayakta, bütün backend testleri geçiyor.

### Grup 2 — Domain: kapsam boyutu

- `TransactionScope` enum: `Business = 1`, `Personal = 2`. Üçüncü belirsiz
  durum **yok** — veri sıfırlandığı için yorumlanacak geçmiş yok.
- **Zorunlu kapsam taşıyanlar:** `BudgetTransaction`, `CreditCardCharge`,
  `DebtAgreement`, `InstallmentPlan`, `RecurringTransaction`, `MonthlyBudget`.
  Ortak ölçüt: gelir/gider raporunu etkileyen ya da etkileyecek kayıt üreten
  her model.
- **Taşımayanlar ve gerekçesi:** `Transfer` ve `CreditCardPayment` gelir/gider
  raporuna sıfır etki eder (ADR 0002, ADR 0003) — kapsam sormak, cevabı hiçbir
  yerde kullanılmayan bir soru sormak olurdu.
- `Account`, `CreditCard`, `Category`: **nullable** varsayılan kapsam. Boş
  olması meşru — tek hesabıyla her şeyi yöneten esnaf için kapsam kategoriden
  türer.
- Ölçüt: domain unit testleri kapsam invariant'larını kanıtlıyor.

### Grup 3 — Application: kapsam türetme zinciri

- Türetme sırası: kullanıcının açık seçimi → hesabın/kartın etiketi →
  kategorinin varsayılanı. Üçü de boşsa istek reddedilir; sunucu kapsam
  **uydurmaz**.
- Bütün oluşturma/güncelleme use case'leri kapsamı taşır; owner-scope kuralları
  aynen korunur.
- Tekrarlayan plan ve taksit planı gerçekleştiğinde ürettiği kaydın kapsamı
  **plandan** gelir; gerçekleşme anında yeniden türetilmez. Aksi hâlde aynı
  plan farklı aylarda farklı kapsam üretebilirdi.
- Ölçüt: application testleri türetme sırasını ve reddi kanıtlıyor.

### Grup 4 — Migration

- İkinci gerçek migration: `AddTransactionScope`. ADR 0012 tek `InitialCreate`
  serbestliğinin **bir daha kullanılmayacağını** yazıyor; o karar burada
  uygulanıyor, `InitialCreate` yeniden üretilmiyor.
- Kolonlar `NOT NULL` eklenebilir **çünkü tablolar Grup 1'de boşaltıldı**. Bu,
  `AGENTS.md` migration kurallarının istisnası değil, ön koşulunun sağlanmış
  hâli; dolu tabloda aynı sıra kullanılamaz.
- `MigrationHistoryTests` zincir testine dönüşür — `AGENTS.md` bunu önceden
  yazmıştı: ikinci migration eklendiğinde tekliği doğrulayan test kırılır.
- Ölçüt: `dotnet ef database update` temiz veritabanında sorunsuz;
  `HasPendingModelChanges` yok.

### Grup 5 — Kategori setleri ve onboarding

- İki varsayılan set: **işletme** ve **kişisel**. Mevcut liste kişisel set
  olarak kalır (`Maaş` dâhil — kişisel kullanıcı için doğru).
- İşletme seti, işletme kalemleri (varsayılan kapsam `İşletme`) **ve** patronun
  gündelik hayatı için derli toplu bir kişisel alt küme (varsayılan kapsam
  `Şahsi`) içerir. İkisi birden gerekiyor, çünkü esnafın market alışverişi de
  aynı uygulamaya giriyor.
- İşletme kalemleri en az şunları kapsar: satış geliri, hizmet geliri, ticari
  mal alımı, işyeri kirası, personel ücreti, SGK ve vergi ödemesi,
  elektrik/su/doğalgaz, iletişim, nakliye ve kargo, ambalaj ve sarf malzemesi,
  bakım-onarım, araç ve yakıt, muhasebeci ve danışmanlık, banka ve POS
  komisyonu, reklam, sigorta, faiz ve finansman gideri.
- **Ödeme yöntemi kategori değildir**: "Satış geliri (kart)" gibi bir kalem
  açılmaz; kartla mı nakit mi tahsil edildiği hesaptan bellidir.
- Onboarding tek soru sorar ve **hiçbir özelliği kapatmaz**: yalnız hangi setin
  kurulacağını ve kapsam anahtarının başlangıç konumunu belirler. Sonradan
  değiştirilebilir.
- "İşletmem yok" diyen kullanıcıda kapsam boyutu arayüzde **tamamen gizlenir**;
  bütün kayıtlar sessizce şahsi olur.
- Mevcut davranış korunur: varsayılan set yalnız **hiç kategorisi olmayan**
  kullanıcıya uygulanır, sonradan geri getirilmez.
- Ölçüt: iki set de kurulabiliyor, kategori varsayılan kapsamları doğru.

### Grup 6 — Okuma modelleri, raporlar ve bütçeler

- Birleşik feed ve planlanan görünüm kapsam filtresi alır. Filtre **SQL'e
  iner**; bellekte birleştirme/filtreleme yasağı aynen geçerli.
- Aylık ve gelişmiş raporlar kapsam kırılımı verir.
- Bütçe ilerlemesi kapsama duyarlıdır: bir kategori iki kapsamı birden
  tutabildiği için bütçenin hangi tarafı sınırladığı açık olmalı.
- **Bölünmeyenler testle korunur:** hesap bakiyesi, kart borcu ve net varlık
  kapsam filtresinden etkilenmez. Bu, aşamanın en kolay sessizce bozulacak
  kuralı.
- Ölçüt: aynı ay üç kapsamda okunduğunda gelir/gider değişir, bakiye değişmez.

### Grup 7 — Flutter: kapsam anahtarı ve formlar

- Global kapsam anahtarı: Özet ekranının başlığında, `Hepsi · İşletme · Şahsi`.
  Seçim uygulama genelinde geçerli ve oturumlar arası hatırlanır. **Sekme başına
  ayrı filtre kurulmaz** (ADR 0013'te reddedildi).
- Bölünen ekranların başlığında aktif kapsam yazılı durur.
- Bölünmeyen ekranlar toplam gösterir ve bunu **açıkça yazar**; sessizce aynı
  kalmaz.
- İşlem formunda kapsam ayrı bir zorunlu alan değil, kategorinin yanında
  **düzeltilebilir bir çip**: varsayılan zincirden dolu gelir.
- `FinancialDataChanges` hedefleri gözden geçirilir: kapsam değişikliği hangi
  ekranları tazelemeli.
- Ölçüt: loading, empty, error, unauthorized ve stale-cache durumları kapsamla
  birlikte görünür ele alınmış.

### Grup 8 — Özet ekranının hero metriği

- Bugünkü `Bu ayın neti` kapsam varken hangi neti sorduğunu söylemiyor.
- Yeni okuma: **işletme neti** (işletme geliri − işletme gideri) ve **şahsi
  çekim** ayrı ayrı; ikisinin altında kasa değişimi.
- **"Kâr" kelimesi kullanılmaz.** Hesaplanan nakit esaslı işletme netidir;
  muhasebe kârı satılan malın maliyetini ister ve ürün sınırının dışındadır.
  Yanlış kelime kullanıcıyı vergi beyanında yanıltır.
- Kişisel-only kullanıcıda ekran bugünkü davranışını korur.
- Ölçüt: üç sayı birbiriyle tutarlı ve tasarım sistemi kontrast kapısını
  geçiyor.

### Grup 9 — Yedek v6, dışa aktarma ve runbook

- Yedek şeması **v6** yazar ve **yalnız v6 okur**. v2–v5 `restore.invalid` ile
  reddedilir: o yedeklerde kapsam alanı yok ve bir değer uydurmak, olmamış bir
  geçmiş uydurmak olurdu (ADR 0013).
- CSV dışa aktarma kapsam kolonu taşır; içe aktarma kapsamı türetme
  zincirinden alır.
- `documentation/restore-runbook.md` v6'ya göre yeniden yazılır.
- Ölçüt: v6 yedek alınıp boş kullanıcıya geri yükleniyor, kapsamlar korunuyor.

## Zorunlu testler

### Domain / Application

- Kapsam zorunlu olan modellerde kapsamsız oluşturma reddedilir.
- Türetme zinciri: form seçimi hesabı, hesap kategoriyi yener; üçü de boşsa
  reddedilir.
- Tekrarlayan plan ve taksit gerçekleşmesi kapsamı plandan alır.
- `Transfer` ve `CreditCardPayment` kapsam taşımaz.

### API ve gerçek SQL

- Kullanıcı izolasyonu: başka kullanıcının kaydına kapsam filtresiyle de
  erişilemez; pozitif **ve** negatif senaryo.
- Kapsam filtresi tek SQL sorgusunda iner; bounded query-count ölçüsü korunur.
- Aynı ay üç kapsamda okunduğunda gelir/gider değişir, **bakiye ve net varlık
  değişmez**.
- Migration zincir testi; `HasPendingModelChanges` yok.
- Yedek v6 yazma/okuma; v5 reddi.

### Flutter

- Kapsam anahtarı seçimi kalıcı; sekme değiştirince korunuyor.
- İşletme kullanıcısında çip dolu geliyor ve düzeltilebiliyor.
- "İşletmem yok" kullanıcısında kapsam arayüzü hiç görünmüyor.
- Bölünmeyen ekranlar toplam gösteriyor ve bunu yazıyor.
- Controller testleri: loading, empty, error, unauthorized, stale-cache.

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

- `documentation/architecture.md`: kapsam boyutu, türetme zinciri, bölünen ve
  bölünmeyen okuma modelleri.
- `documentation/flows.md`: onboarding ön ayarı, kapsamlı işlem ekleme, kapsam
  anahtarının akışı.
- `documentation/permissions.md`: kapsam alan yeni/değişen endpoint'ler ve
  sahiplik sınırı.
- `documentation/tests.md`: bu aşamanın test haritası.
- `documentation/restore-runbook.md`: yedek v6.
- `documentation/design-system.md`: kapsam anahtarı ve kapsam çipi bileşenleri.
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler.

Henüz uygulanmamış davranış, uygulanmış gibi yazılmaz.

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır; kapsam filtresi sahiplik kontrolünün
  **yerine geçmez**, üstüne biner.
- Yalnız sentetik veri kullanılır.
- Secret, connection string ve token belgeye veya loga yazılmaz.
- Veri sıfırlama yalnız yerel geliştirme veritabanında yapılır.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| Yanlış varsayılan sessizce yanlış etiketler | Kapsam onay ekranında görünür durur; tek dokunuşla düzeltilir |
| Bakiye veya net varlığın kapsam filtresine takılması | Bölünmezliği doğrulayan test, aşamanın çıkış koşulu |
| Aşamanın dokuz grupla büyümesi | Her grup kendi checkpoint'i; derlenebilir ve testi geçen noktada durulabilir |
| Veri sıfırlamanın geri alınamazlığı | Uygulamadan önce ayrı kullanıcı onayı; yalnız yerel sentetik veri |
| Kapsam ile indirilebilirliğin karıştırılması | 01'de vergi kelimesi hiç geçmez; ayrım ADR 0013'te yazılı |
| İkinci migration'ın yükseltme yolunu bozması | Boş tablo ön koşulu belgede yazılı; zincir testi eklenir |

## Çıkış koşulları

- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Aynı ay üç kapsamda okunduğunda bakiye ve net varlığın değişmediği testle
      kanıtlandı.
- [ ] İki farklı esnaf senaryosu sentetik veriyle uçtan uca girildi; işletme
      neti şahsi harcamadan etkilenmedi.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 02'yi açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
