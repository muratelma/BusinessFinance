# ADR 0012 — Şema tek bir InitialCreate ile kurulur

- Durum: Kabul edildi (21 Ağustos 2026, repo taşıması)
- Bağlam: Kod tabanının `Kisisel-Butce-Mobil` reposundan `BusinessFinance`
  reposuna taşınması
- İlgili: ADR 0005 (tekrarlayan planın kaynağı — backfill'i bu zincirdeydi)

## Bağlam

Kod tabanı yeni bir repoya, tek temiz commit olarak taşındı. Devralınan şema 21
migration'la kurulmuştu: `InitialCreate` → `PersistApplicationPorts` →
`Stage7FinancialCore` → … → `AddCreditCardMinimumPaymentRate`. Designer ve
snapshot dosyalarıyla birlikte **43 dosya ve 36.766 satır**.

Bu zincirin tek işlevi, boş bir veritabanını 21 adımda bugünkü şemaya
getirmekti. Ürünün yayımlanmış bir sürümü, dolayısıyla yükseltilecek bir
veritabanı yoktu: tek veritabanı geliştirme makinesindeydi ve zaten zincirin
sonundaydı.

Aynı anda ürünün ticari olarak sunulabileceği netleşti. Bu, şemayı sadeleştirmek
için **son fırsattı**: gerçek kullanıcı verisi geldiğinde zincir dokunulmaz hale
gelir.

## Karar

**Migration zinciri taşınmadı; şema tek bir `InitialCreate` ile kurulur.**

- 43 dosya / 36.766 satır → 3 dosya / 5.924 satır
- Kopyalanan veritabanının `__EFMigrationsHistory` tablosu 21 satırdan 1 satıra
  indirildi; şema zaten doğru olduğu için hiçbir DDL çalıştırılmadı ve **hiçbir
  veri kaybedilmedi**
- Backfill migration'larının bıraktığı 5 artık DEFAULT kısıtı düşürüldü
  (`Accounts.OpeningBalance`, `BudgetTransactions.IsCancelled` ve üç
  `SourceType`). Bunlar mevcut satırları doldurmak için verilmişti; modelde
  karşılıkları yok. Düşürülmeseydi şema ile model sessizce ayrışmış kalırdı

Bundan sonra eklenen her migration **gerçek bir yükseltme yoludur** ve bu
serbestlik bir daha kullanılmaz.

## Denklik nasıl kanıtlandı

Karar, "büyük ihtimalle aynıdır" ile alınmadı. Tek `InitialCreate`'ten sıfır bir
veritabanı kuruldu ve gerçek veriyi taşıyan veritabanıyla karşılaştırıldı:

| Karşılaştırılan | Sonuç |
|---|---|
| Kolonlar (tip, uzunluk, precision, scale, nullable) | 261/261 aynı |
| İndeksler (tekillik, filtre, kolon sırası) | 104/104 aynı |
| CHECK kısıtları (ad ve tanım) | 81/81 aynı |
| Foreign key'ler (ad, hedef, delete action) | 58/58 aynı |
| PK/UNIQUE kısıtları | 39/39 aynı |
| **Toplam** | **543/543 satır birebir** |

Satır sayıları da 28 tabloda birebir korundu. Ardından bütün test takımı gerçek
SQL'e karşı koşuldu: 733 backend testi geçti.

## Bedeli

Zincire bağlı iki test geçersiz kaldı:

- `Stage125Migration_BackfillsExistingRecurringRowsAsAccountSource` **silindi**.
  Migration'ları ara bir noktaya kadar yürütüp eski şemanın yazdığı satırları
  ekliyor, sonra yükseltmeyi uygulayıp verinin `SourceType=Account` olarak
  hayatta kaldığını kanıtlıyordu. Zincir olmadan çalıştırılamaz. Koruduğu
  dönüşüm kopyalanan veritabanında zaten uygulanmış ve yükseltilecek başka
  veritabanı yok.
- `MigrationHistoryTests` **yeniden yazıldı**. Eskisi 21 migration'ın adını,
  sırasını ve operasyon sayılarını doğruluyordu; yenisi tekliği,
  `HasPendingModelChanges` yokluğunu ve yapısal sayımları doğruluyor.

Kaybolan asıl şey bu iki test değil, zincirin **yorum satırlarında taşıdığı
yükseltme güvenliği bilgisiydi**. Bu yüzden o kurallar `AGENTS.md` içindeki
"Migration kuralları" bölümüne yazıldı ve bundan sonraki her migration için
bağlayıcı kılındı:

- Backfill her zaman CHECK kısıtından önce çalışır.
- Kolonlar kısıtlardan önce eklenir.
- Geçmişi bilinmeyen bilgi için kolon nullable olur.
- Backfill `defaultValue`'sunun bıraktığı DEFAULT kısıtı, model istemiyorsa
  migration sonunda düşürülür.

## Sonuçlar

- Şema tek yerden okunur; yeni geliştirici 21 dosya arasında dolaşmaz.
- Migration klasörü %84 küçüldü.
- **Geçmiş bir yükseltme yolunu yeniden test etme imkânı kalmadı.** Kabul
  edilebilir, çünkü yükseltilecek eski veritabanı yok.
- İkinci bir migration eklendiğinde `MigrationHistoryTests` bilerek kırılır; o
  an tekliği doğrulayan test yerini zincir testine bırakır.
- Bu karar **tekrarlanamaz**: gerçek veri geldikten sonra zincir yeniden
  yazılamaz.
