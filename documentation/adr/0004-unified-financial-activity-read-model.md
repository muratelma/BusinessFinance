# ADR 0004 — Ayrı Write Modelleri, Tek Birleşik Read Projection'ı

- Durum: Kabul edildi ve uygulandı
- Tarih: 2026-08-14
- Kapsam: Birleşik finansal hareket geçmişi, `GET /api/v1/financial-activities`

## Bağlam

Stage 10–12 sonunda kullanıcının gerçekleşmiş finansal olayları altı farklı
yazma modelinde tutuluyor: `BudgetTransaction`, `Transfer`, `CreditCardCharge`,
`CreditCardPayment`, ödenen `DebtInstallment` ve tahsil edilen alacak taksidi.
Her biri kendi domain kuralına sahip ve bu ayrım kasıtlı — ADR 0002 ve ADR 0003
transferin ve kart ödemesinin neden normal gelir/gider olmadığını kaydediyor.

Ancak kullanıcı için bunların hepsi tek bir şeydir: "param nereye gitti".
Bugün bu bilgi `İşlemler`, `Transferler ve kartlar`, `Planlama` ve `Veri
Araçları` ekranlarına dağılmış durumda. Kullanıcı ayın harcamasını görmek için
dört ekran gezmek zorunda ve hiçbir ekran tam kronolojik geçmişi vermiyor.

İki uçtaki çözüm de yanlış:

- **Write modellerini birleştirmek**: Transferi ve kart ödemesini
  `BudgetTransaction` yapmak raporu bozar ve ADR 0002/0003'ü geri alır.
- **Flutter'da birleştirmek**: İstemci altı ayrı sayfalı listeyi doğru sırayla
  ve doğru toplam sayıyla birleştiremez; sayfa sınırlarında kayıt kaybolur veya
  tekrarlanır.

## Karar

Write modelleri olduğu gibi kalır. Yalnız **okuma** tarafında ortak bir
`FinancialActivity` projection'ı eklenir.

```text
Yazma (değişmez)                     Okuma (yeni)
├── BudgetTransaction
├── Transfer
├── CreditCardCharge          ──►    FinancialActivity projection
├── CreditCardPayment                (kalıcı tablo değil)
├── DebtInstallment ödemesi                  │
└── DebtInstallment tahsilatı                ▼
                                    GET /api/v1/financial-activities
```

Kalıcı `FinancialActivity` tablosu **oluşturulmaz**. Projection her sorguda
mevcut tablolardan türetilir; ikinci bir gerçek kaynak ve senkronizasyon borcu
doğmaz.

### Tek olay, tek satır

Bir ekonomik olay listede tam olarak bir satırdır. Transfer iki hesabı etkiler
ama tek satırdır; kart ödemesi hem hesabı hem kart borcunu etkiler ama tek
satırdır. Gerçekleşen recurring occurrence ve taksit yalnız **sonuç** hareketi
olarak görünür, plan kaydı olarak değil.

### Beş bağımsız sınıflandırma boyutu

Tek bir `type` alanı kullanıcının filtrelerini karşılayamaz. Hareket beş
ortogonal boyut taşır: `activityKind` (hangi olay), `effect` (rapor etkisi),
`sourceGroup` (kullanıcıya gösterilen grup), `origin` (nasıl üretildiği),
`status` (yaşam döngüsü). Örneğin taksitten gerçekleşen bir kart harcaması
`card-charge` + `expense` + `credit-card` + `installment` + `realized`'dır;
tek enum bunu ifade edemez.

Kimlik tek başına türü belirtmez; Flutter liste anahtarı
`activityKind:activityId` bileşimidir.

### Birleştirme SQL'de yapılır

Kaynak tablolar ortak projection şekline map edilir ve EF Core `Concat`
(`UNION ALL`) ile tek sorguda toplanır. `OrderBy`, `Skip`, `Take` ve `Count`
veritabanına iner. Hesap/kategori/kart adları aynı sorguda join ile getirilir.

Bu, kararın en kolay atlanan parçasıdır. Kaynakları ayrı ayrı belleğe çekip C#
tarafında birleştirmek "sabit sayıda sorgu" ölçüsünü geçer ve bounded
query-count testini yanıltır; fakat her sayfa isteğinde kullanıcının bütün
geçmişini okur ve maliyet toplam kayıt sayısıyla doğrusal büyür.

### Origin-aware iptal kısıtlaması

`RecurringTransactionOccurrence.Realize` ve `InstallmentItem.Realize` tek sonuç
kimliği taşır ve geri dönüşü yoktur. Mevcut `CancelTransactionUseCase` ve kart
harcaması iptali bugün origin ayrımı yapmadan herhangi bir sonuç hareketini
iptal edebiliyor — bu Stage 12.5 öncesine ait mevcut bir tutarsızlıktır.
Birleşik feed bunu üretmez ama bottom sheet üzerinden çok daha
keşfedilebilir hale getirir.

`canCancel`, `activityKind + origin + status` bileşiminden hesaplanır ve
kısıtlama yalnız UI'da değil backend'de uygulanır: kaynak use case'ler
hareketin occurrence/item bağlantısını kontrol eder ve varsa `409 Conflict`
döner. Böylece eski ekran veya doğrudan API çağrısı aynı tutarsızlığı yeniden
üretemez.

## Reddedilen seçenekler

- **Kalıcı `FinancialActivity` tablosu (materialized feed)**: Her mutation'ın
  ikinci bir tabloyu tutarlı tutmasını gerektirir; yarım yazma, drift ve
  yeniden inşa (rebuild) sorumluluğu doğar. Bu ölçekte okuma maliyeti buna
  değmez.
- **Write modellerini `BudgetTransaction` altında birleştirmek**: ADR 0002 ve
  0003'ü geri alır, rapor sınıflandırmasını bozar.
- **Flutter'da istemci tarafı birleştirme**: Sayfalama ve toplam sayı doğru
  kurulamaz; finansal doğruluk istemciye taşınır.
- **`Unrealize()` ile tam geri alma**: Occurrence/item tek sonuç kimliği
  taşıdığı için bağlantıyı temizlemek "eski iptal edilmiş sonuç + yeni aktif
  sonuç" geçmişini kaybeder. Doğru çözüm realization-history/reversal modelidir
  ve bilinçli olarak Stage 12.5 kapsamı dışındadır.
- **Tek `type` enum'u**: Kullanıcı filtrelerinin beş bağımsız boyutunu tek
  eksene sıkıştırır.

## Sonuçlar ve trade-off'lar

Okuma sorgusu altı tabloyu birleştirdiği için tek tabloya göre daha pahalıdır ve
her yeni finansal olay türü projection'a ayrıca eklenmelidir — unutulursa olay
feed'de görünmez. Buna karşılık yazma tarafı sade kalır, domain invariant'ları
korunur, senkronizasyon borcu oluşmaz ve rapor doğruluğu tek gerçek kaynaktan
gelir.

Serbest metin araması bu projection üzerinde ilk sürümde desteklenmez; açıklama
normalizasyonu ve indeks ihtiyacı ayrı bir performans kararıdır.

`GET /api/v1/transactions` kaldırılmaz. Attachment akışı, `Hesap işlemleri CSV`
export'u ve mevcut testler yalnız `BudgetTransaction` üzerinde çalışır ve o
daraltılmış sözleşmeye bağlıdır; feed onun yerine geçmez, yanında yaşar.

## Kanıt

Bu bölüm Grup 4 ve Grup 7 doğrulandığında testlerle doldurulur. Beklenen
kanıtlar: her activity türünün doğru effect/sourceGroup/origin/status eşlemesi,
owner izolasyonu negatif testleri, deterministik sayfalama ve doğru total count,
sabit sayfa isteğinde okunan satır sayısının fixture büyürken sabit kalması,
recurring/installment/borç kaynaklı iptal isteğinin `409 Conflict` dönmesi ve
aynı ekonomik olayın feed ile raporda yalnız bir kez sayılması.
