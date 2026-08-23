# Aşama 02 — Cari hesap: karşı taraf ve açık bakiye

## Belge durumu

- Durum: **Aktif** (23 Ağustos 2026'da kullanıcı onayıyla açıldı)
- Ön koşul: Aşama 01 — Kapsam boyutu ve işletme kimliği (**tamamlandı**,
  `docs/archive/stages/01-kapsam-boyutu-ve-isletme-kimligi.md`)
- Sonraki aşama: Aşama 03 — Yükümlülük ve vade
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/financial-activity-api-contract.md`,
  `documentation/restore-runbook.md`, **yeni ADR** (aşağıda)
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

"Ahmet'ten ne alacağım var?" sorusunun bugün cevabı yok. Karşı taraf adı iki
yerde düz metin olarak duruyor — `DebtAgreement.CounterpartyName` ve fiş
okumanın çıkardığı `counterpartyName` — ve ikisi birbirini tanımıyor. Aynı
müşteriye üç satış yapıldığında uygulama bunların aynı kişiye ait olduğunu
bilmiyor.

Mevcut borç modeli bu boşluğu dolduramıyor: `DebtAgreement` anapara, taksit
sayısı, ilk vade ve amortisman istiyor. Veresiye defteri böyle çalışmaz — aynı
müşteriye on iki satış, beş kısmi tahsilat, yürüyen bir bakiye vardır ve hiçbiri
taksit planı değildir.

## Kullanıcıya katkı

- Müşteri ve tedarikçi kaydı tutmak; her birinin yürüyen bakiyesini görmek
- Veresiye satmak: satış geliri hemen görünür, para henüz gelmemiştir
- Kısmi tahsilat almak; kalan bakiyenin kendiliğinden düşmesi
- Tedarikçiden vadeli mal almak ve ödemeyi parça parça yapmak
- Bir karşı tarafın bütün geçmişini tek ekranda görmek — açık cari **ve**
  varsa taksitli sözleşmesi birlikte

## Değiştirilmeyecek mimari kararlar

- Kapsam boyutu ve türetme zinciri (ADR 0013). Karşı taraf zincire **dördüncü
  kaynak olarak eklenmez**: kategori zaten "ne alındı" sorusunu cevaplıyor ve
  kapsamı yeterince belirliyor.
- **Bakiye kalıcı kolon değildir.** Cari bakiye de saklanmaz; belgelerden ve
  tahsilatlardan hesaplanan bir projection'dır. Ekstre projection'ının aynısı.
- Silme yerine iptal.
- Kategori kimlik taşımaz — karşı taraf kategoriyle temsil edilmez.
- Birleşik feed tek SQL sorgusunda kalır; bellekte birleştirme yasağı sürer.

## Bu aşamanın karar kapısı: ekonomik olay tanır, ödeme taşır

Aşama başlamadan önce **bir ADR yazılır.** Sebebi, kod tabanında zaten var olan
ama hiçbir yerde açıkça yazılmamış bir kural:

| Kayıt | Gelir/gider tanır mı | Nakit taşır mı |
|---|---|---|
| `CreditCardCharge` | **Evet** | Hayır |
| `CreditCardPayment` | Hayır | **Evet** |
| `DebtAgreement` açılışı (`Expense`/`Income` kaynak) | **Evet** | Hayır |
| Borç taksiti ödemesi | Hayır | **Evet** |

Kural tek cümleyle: **ekonomik olay tanır, ödeme taşır.** Cari hesap bu kuralın
aynısını uygular:

- **Cari borçlandırma** (veresiye satış / vadeli alım): gelir veya gider
  tanınır, hesap bakiyesi değişmez.
- **Cari tahsilat/ödeme**: hesap bakiyesi değişir, gelir/gider **sıfır**.

Bu, aynı paranın iki kez sayılmasını önleyen tek yapıdır ve kart modelinin
birebir aynadaki hâlidir.

**Kuralın dışında kalan bir yer var ve Aşama 03 onu düzeltir:** fiş okumanın
"faturayı henüz ödemedim" yolu bugün hiçbir şey tanımıyor, yalnız plan
öneriyor. Bu aşamada dokunulmaz; ADR bu tutarsızlığı **yazılı olarak kayda
geçirir** ve 03'e devreder.

## Kapsam

### Dahil

- `Counterparty` varlığı ve yönetimi
- Açık cari: borçlandırma ve tahsilat/ödeme kayıtları
- Cari bakiyenin projection olarak hesaplanması
- Mevcut `DebtAgreement`'ın karşı tarafa bağlanması
- Fiş okumanın karşı taraf önerisini gerçek kayda bağlaması
- Birleşik feed ve net varlığa katılma
- Flutter: karşı taraf listesi, ayrıntı ve hızlı tahsilat/ödeme
- Yedek şeması v7

### Açıkça kapsam dışında

- **Vade tarihi, gecikme ve hatırlatma** — Aşama 03
- Fatura kesme, fatura numarası, e-Fatura/e-Arşiv
- Karşı tarafa özel fiyat, iskonto, sözleşme
- Karşı taraf bazlı kredi limiti
- Adres, vergi numarası gibi kimlik alanları (yalnız ad ve not tutulur)
- Karşı tarafın kapsam ipucu taşıması

## Çalışma grupları

### Grup 1 — ADR: ekonomik olay tanır, ödeme taşır — **Tamamlandı**

- Yukarıdaki kural, mevcut kart ve borç davranışından kanıtıyla birlikte
  yazılır.
- Fiş okumanın "ödemedim" yolundaki tutarsızlık açıkça kaydedilir ve 03'e
  devredilir.
- Ölçüt: ADR kabul edildi ve `CLAUDE.md` belge haritasına eklendi.
- Sonuç: `documentation/adr/0014-economic-event-recognizes-payment-carries.md`
  yazıldı ve 23 Ağustos 2026'da kullanıcı onayıyla kabul edildi. Kanıt tablosu
  koddan çıkarıldı; en güçlü kanıt `DebtAgreement`'ın **kaynağına göre** yön
  değiştirmesi — gider kaynaklı açılış tanır ve taşımaz, nakit kaynaklı açılış
  taşır ve tanımaz. Dört alternatif gerekçesiyle reddedildi. Fiş okumanın
  "faturayı ödemedim" yolundaki tutarsızlık kayda geçti ve Aşama 03'e
  devredildi.

### Grup 2 — Domain: karşı taraf ve cari hareketler — **Tamamlandı**

- `Counterparty`: `Id`, `UserId`, `Name`, `Note`, `IsActive`. Teklik
  `(UserId, Name)` üzerinde.
- **Müşteri/tedarikçi ayrı tip değildir.** Aynı kişi hem alıcı hem satıcı
  olabilir — mahalle esnafında sık. Yön, hareketin kendisinde durur.
- `CounterpartyCharge`: karşı tarafı borçlandıran/alacaklandıran ekonomik olay.
  Taşıdıkları: yön, tutar, tarih, kategori, kapsam, açıklama.
- `CounterpartyPayment`: tahsilat veya ödeme. Taşıdıkları: hesap, tutar, tarih,
  açıklama. **Kategori taşımaz** — kart ödemesinin kategori taşımamasıyla aynı
  gerekçe.
- Karşı taraf **fiziksel silinmez**; hareketi varsa pasifleştirilir. Hiç
  hareketi yoksa silinebilir (boş hesap kuralının aynısı, `409` + pasife alma
  yönlendirmesi).
- Ölçüt: domain testleri iki kayıt türünün yan etkilerini kanıtlıyor.
- Sonuç: `Counterparty`, `CounterpartyCharge`, `CounterpartyPayment` ve
  `CounterpartyBalance` eklendi; 17 domain testi geçiyor. Grup **yalnız domain
  katmanına** dokundu: EF modeline girmediği için migration üretmedi ve
  `HasPendingModelChanges` temiz kaldı — kalıcılık Grup 3'ün işi.
- **Yönü `DebtDirection` taşıyor**, ikinci bir enum açılmadı: sorduğu soru aynı
  (yükümlülük kimin üzerinde) ve aynı iki değeri taşıyan ikinci bir tip, aynı
  kavramı iki adla anlatmak olurdu.
- **Yön kategorinin türünü belirliyor**: alacak doğuran borçlandırma gelir
  kategorisi, borç doğuran gider kategorisi ister. Tutmayan istek reddediliyor —
  yoksa kayıt raporun yanlış tarafına düşerdi.
- **Karar:** pasif karşı tarafa yeni borçlandırma yazılamaz ama **tahsilat
  yazılabilir**. Belgede yalnız "hareketi varsa pasifleştirilir" yazıyordu;
  ikisini de engellemek, artık iş yapılmayan bir müşterinin kalan borcunu
  kapatılamaz hâle getirirdi.
- **Karar:** fazla tahsilat kırpılmıyor, taraf eksiye düşüyor. Kırpmak
  kullanıcının parasını ekranda yok ederdi; aynı hatanın kart tarafındaki hâli
  `docs/backlog.md` 1. maddede duruyor.

### Grup 3 — Cari bakiye projection'ı — **Tamamlandı**

- Bakiye = borçlandırmalar − tahsilatlar, karşı taraf ve owner kapsamlı.
- Liste ekranı için **tek sorguda** karşı taraf başına bakiye; N+1 yok.
- Sıfır bakiyeli karşı taraf listede kalır ama ayrı okunur (kapanmış cari).
- Ölçüt: bounded query-count testi; 50 karşı taraflı sentetik veride tek sorgu.
- Sonuç: üç tablo kalıcılığa girdi (`AddCounterparties` migration'ı),
  `ICounterpartyRepository` portu ve tek sorgulu EF uygulaması yazıldı.
  Ölçü gerçek SQL üzerinde alındı: **53 karşı taraf, tek okuma komutu.**
- **Migration yalnız yeni ve boş tablo kuruyor**, mevcut hiçbir tabloya kolon
  veya kısıt eklemiyor; backfill kuralının "gerçekten boş tablo" istisnası
  burada geçerli ve testle sabitlendi.
- **Toplamlar ilişkili alt sorgu olarak karşı tarafın satırının içinde**;
  filtre ve sıralama da veritabanında. Kişi başına toplam sorgusu elli
  kayıtta yüzün üzerinde sorgu demek olurdu.
- **Karar:** eksi bakiye (fazla tahsilat) *kapanmış* sayılmıyor. Kapanmış
  cari iki tarafı da sıfır olandır; eksi bakiyede hâlâ konuşulacak para var.
- Yazma yolu (endpoint, use case) bu grupta açılmadı: Grup 3 okuma modelidir.

### Grup 4 — Mevcut borç modelinin bağlanması

- `DebtAgreement.CounterpartyName` → `CounterpartyId`. Ad artık karşı taraftan
  okunur.
- Borç oluştururken yeni bir ad yazılırsa karşı taraf **bulunur ya da
  oluşturulur**; kullanıcı iki adımlı bir akışa zorlanmaz.
- Karşı taraf ayrıntı ekranı iki bloğu birlikte gösterir: açık cari bakiyesi ve
  varsa taksitli sözleşmeleri. **Toplam tek kez** hesaplanır.
- Ölçüt: aynı karşı tarafın iki kaynaktaki tutarı net varlıkta bir kez sayılıyor.

### Grup 5 — Birleşik feed ve raporlar

- İki yeni ekonomik olay feed'e katılır; `activityKind`, `effect`,
  `sourceGroup`, `origin`, `status` beş boyutu doldurulur.
- `canCancel` kuralı bu kayıtlar için tanımlanır: tahsilatı iptal etmek
  bakiyeyi geri alır, borçlandırmayı iptal etmek geliri geri alır.
- Net varlık: alacak artırır, borç azaltır. **Anapara ölçüsü korunur**
  (ADR 0010).
- **Hesap bakiyesi tahsilatı görür.** `CounterpartyPayment` parayı taşıyan
  bir kayıttır; hesap bakiyesi hesabına katılmazsa tahsil edilen para
  kasada hiç görünmez. Grup 3 yalnız cari tarafını hesapladı, hesap
  bakiyesi bu grupta genişler.
- `documentation/financial-activity-api-contract.md` yeni kaynaklarla
  genişletilir.
- Ölçüt: feed tek SQL sorgusunda kalıyor; sayfalama ve sıralama veritabanında.

### Grup 6 — Fiş okumanın bağlanması

- Fişten okunan `counterpartyName` artık düz metin olarak kaydedilmez: mevcut
  karşı taraflarla eşleştirilir ve **öneri olarak** gösterilir.
- Eşleşme bulunamazsa yeni karşı taraf oluşturma tek dokunuş olur.
- **ADR 0011 bozulmaz:** model karşı tarafı seçmez, önerir; kullanıcı onaylar.
- Ölçüt: yanlış eşleşme kullanıcıya görünür ve tek dokunuşla reddedilir.

### Grup 7 — Flutter

- `features/counterparties/`: liste (bakiyeye göre sıralı), ayrıntı, hareket
  geçmişi.
- Hızlı eylem: karşı taraf ayrıntısından tahsilat/ödeme.
- `Diğer` menüsüne tek kapı eklenir; `Borç ve alacaklar` kapısı bu ekranla
  ilişkilendirilir.
- `İşlem ekle` menüsüne **yeni seçenek eklenmez**: menü zaten yedi seçenekli ve
  yeniden kurgusu Aşama 04'te. Cari hareketleri karşı taraf ekranından girilir.
- `FinancialDataChanges` yeni hedef alır.
- Ölçüt: loading, empty, error, unauthorized, stale-cache görünür ele alınmış.

### Grup 8 — Yedek v7 ve dışa aktarma

- Şema **v7** yazar, yalnız v7 okur.
- CSV dışa aktarma karşı taraf kolonu taşır.
- `documentation/restore-runbook.md` güncellenir.
- Ölçüt: karşı taraf ve cari hareketler kayıpsız geri yükleniyor.

## Zorunlu testler

### Domain / Application

- Borçlandırma gelir/gider tanır, hesap bakiyesini değiştirmez.
- Tahsilat hesap bakiyesini değiştirir, gelir/gider üretmez.
- Aynı satış hem borçlandırma hem tahsilat girildiğinde **iki kez gelir
  sayılmaz**.
- Hareketi olan karşı taraf silinemez; pasifleştirilebilir.
- Kısmi tahsilat sonrası bakiye doğru; fazla tahsilat davranışı tanımlı.

### API ve gerçek SQL

- Kullanıcı izolasyonu: başka kullanıcının karşı tarafına ve hareketine
  erişilemez; pozitif **ve** negatif senaryo.
- Karşı taraf başına bakiye tek sorguda; bounded query-count.
- Net varlıkta aynı karşı tarafın cari ve taksitli borcu bir kez sayılır.
- Feed tek SQL sorgusunda kalır.
- Yedek v7 yazma/okuma; v6 reddi.

### Flutter

- Karşı taraf listesi, ayrıntı ve hızlı tahsilat akışı.
- Fiş okuma önerisinin kabulü ve reddi.
- Zorunlu ekran durumları.

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

- Yeni ADR: ekonomik olay tanır, ödeme taşır
- `documentation/architecture.md`: karşı taraf boyutu, cari projection
- `documentation/flows.md`: veresiye satış, tahsilat, karşı taraf eşleştirme
- `documentation/permissions.md`: yeni endpoint'ler ve sahiplik sınırı
- `documentation/financial-activity-api-contract.md`: iki yeni kaynak
- `documentation/tests.md`, `documentation/restore-runbook.md`
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Karşı taraf adı kullanıcı verisidir; loga yazılmaz.
- Yalnız sentetik veri kullanılır.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| Aynı paranın cari ve borç modelinde iki kez sayılması | Net varlık tek sayım testi, çıkış koşulu |
| Cari bakiyenin kalıcı kolona kaçması | Projection kuralı ADR'de; bounded query testi |
| Karşı taraf listesinin N+1 üretmesi | Tek sorgulu bakiye, query-count ölçüsü |
| Fiş okumanın yanlış karşı tarafa bağlaması | Öneri katmanı; kullanıcı onayı zorunlu (ADR 0011) |
| Aynı kişinin iki kez oluşturulması | `(UserId, Name)` tekliği + oluştururken benzer ad uyarısı |

## Çıkış koşulları

- [ ] ADR yazıldı ve kabul edildi.
- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Bir müşteriye 3 satış + 2 kısmi tahsilat senaryosu girildi; cari bakiye,
      net varlık ve gelir raporu birbirini tutuyor, hiçbir tutar iki kez
      sayılmıyor.
- [ ] Feed'in tek SQL sorgusunda kaldığı kanıtlandı.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 03'ü açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
