# Dış göz — Codex'in kategori/hesap işinin para tarafı (14 Ekim 2026)

İncelenen: `3bbc863` (sunucu) ve `97bce47`'nın sunucu dosyaları. 13 Ekim'de
yalnız testler koşmuştu; bu sefer kod satır satır okundu. Ölçüt
`DEVIR-2026-10-13.md` §2 madde 2 ve `DEVIR-2026-10-14.md` §6.

## Sonuç

Para tarafında **ürün hatası bulunmadı**. Bir doğrulama boşluğu bulundu ve
kapatıldı: elle değiştirilmiş bir yedek, transferi dururken masrafı iptal
edilmiş bir kayıt taşıyabiliyordu (§3). Uygulamanın kendisi bu durumu
üretemez.

## §1 — Bakılanlar ve hüküm

| Konu | Ne arandı | Hüküm |
|---|---|---|
| Migration `AddCategoryRolesAndTransferFees` | Kolonlar kısıttan önce mi, backfill CHECK'ten önce mi, eski bilgi nullable mı | **Doğru.** İki kolon nullable eklenir → ad anahtarından rol eşlemesi → indeks, CHECK, foreign key. Eski transferlerin masraf bağı boş kalır, uydurma masraf yazılmaz. Aynı rolün birden çok eski adayı varsa rol aktif olana, sonra kimlik sırasındaki ilkine verilir; diğerleri silinmez, birleştirilmez |
| Migration'ın dolu veritabanı testi | Test gerçekten eski şemaya veri yazıp yükseltiyor mu | **Evet.** Bir önceki migration'a kurar, SQL ile beş kategori yazar, sonra yükseltir |
| Rolün kapsamı açması | Rol alan kategori iki tarafa açılır | ADR 0020 İ6'ya uygun (taraf yalnız genişler). Karar KÖ9'da yazılı: işletme setinde `Banka ve POS komisyonu` iki tarafa açılır |
| Yedek v12 / v11 | v12 rolü ve masraf bağını taşıyor mu; v11 okunuyor mu | **Doğru.** v11'de rol adından türetilir, v12'de boş rol boş kalır; aynı rolün iki kez verildiği dosya reddedilir |
| Masraflı transferin yedekten dönüşü | Test var mı | **Yoktu** → yazıldı (§3) |
| `RecognizedItems` | İkinci bir kaynak listesi yazılmış mı | **Hayır.** Sekiz kaynak aynı yerde; yalnız alanlar eklendi (kaynak kimliği, bağlı kayıt, parça). Masraf ve kesinti sıradan gider olarak tanınmaya devam eder |
| `AccountMovements` | Masraf iki kez düşülüyor mu | **Hayır.** Masraf gideri hesap hareketlerinden çıkarılır, transferin çıkışına eklenir: hesapta tek hareket, tutar = transfer + masraf. Yatış da tek hareket: yatan tutar girer, kesinti ayrıca düşülmez |
| POS'un hesaba geçişi `PosSettlement` yerine `PosDeposit`'ten | Yatışsız "hesaba geçti" eski kayıt kalmış olabilir mi | **Kalamaz.** Veritabanındaki CHECK `TransferredOn` ile `PosDepositId`'yi birlikte zorunlu tutuyor. Toplam eskisiyle aynı: Σ(brüt − komisyon) − kesinti = yatan tutar |
| Eski okuyucular | Masrafı bilmeyen bir bakiye okuyucusu kalırsa | Yine doğru sonuç verir: masraf gerçek bir gider kaydı olduğu için "gider + transfer" toplamı aynı tutarı düşer |
| Masrafın tek başına iptali | Kilit | **Var.** Kökeni `transfer` olur, `transactions.cancel_origin_locked` (409); transfer iptali masrafı da iptal eder. Testle korunuyor |
| Gün sonu ve kasa sayımı kilitleri | Masraf gün sonunda sayılabilir mi | Sayılamaz: gün sonu yalnız gelir ve tahsilat sayar, masraf giderdir. Kasadan masraflı transfer sayımı eskitir (beklenen) |
| Sahiplik | Yeni okumalarda `UserId` süzgeci | Bütün sorgular sahibe göre süzülüyor; yeni uçların hepsi sahiplik testine eklenmiş. Masraf bağı kompozit (UserId, Id) foreign key |
| Rolün ilk açılışı aynı anda iki istekle | Çift kategori | SQL tekliği kazananı seçer; testle korunuyor |

## §2 — Küçük notlar (düzeltilmedi)

1. **Pasif rol kategorisi.** Kullanıcı `Banka masrafı`nı pasife alırsa masraflı
   transfer İngilizce bir doğrulama hatasıyla reddedilir
   (`transfers.validation`, `The fee category is inactive.`); faizli taksit
   ödemesi `debt.category_unavailable` ile. Ekranın bunu nasıl gösterdiği denenmedi.
2. **Rol kategorisi istek başarısız olsa da açılır.** Kategori ayrı kaydedilir;
   sonra transfer kapsam sorulmadığı için reddedilirse kategori kalır. Zararsız,
   zaten açılacak bir kategori.
3. **Rapor sorgusu ağırlaştı.** `RecognizedItems`'ta her gelir/gider satırı
   için dört ilişkili alt sorgu var (bağlı transfer ya da yatış). Doğru, ama
   veri büyüyünce rapor süresi izlenmeli.
4. Migration, aynı kullanıcıda hem `Banka ve POS komisyonu` hem `Banka
   masrafı` varsa rolü kimlik sırasına göre verir; hangisinin alacağı
   rastgeledir. Varsayılan setlerde ikisi birlikte yok.

## §3 — Yapılan düzeltme

- **Boşluk:** geri yükleme, transfer dururken masrafı iptal edilmiş ya da iki
  transfere (veya bir yatışın kesintisine) bağlanmış bir masraf taşıyan dosyayı
  kabul ediyordu. İlk durumda hesap bakiyesi iptal edilmiş masrafı düşerdi
  (transferin çıkışı masrafın iptal durumuna bakmaz); ikincisinde SQL tekliği
  kaydı 500 ile düşürürdü. Gün sonu ve yatış için aynı denetim zaten vardı.
- **Düzeltme:** `EfDataPortabilityRepository` bu dosyaları
  `restore.invalid_backup` ile reddeder.
- **Testler:** `Backup_RoundTripsTransferFeesWithTheirCancellation`,
  `Backup_RejectsATransferFeeThatDoesNotFollowItsTransfer`.

## §4 — Kontroller

| Kontrol | Sonuç |
|---|---|
| Backend build | 0 uyarı, 0 hata |
| Backend format | Temiz |
| Backend test, gerçek SQL, düzeltmeden önce (tam koşu) | Domain 402 · Application 400 · API 327 · Infrastructure 257 (+2 canlı sağlayıcı testi atlanır) |
| Yeni testler düzeltme olmadan | Ret testi **kırıldı** ("No exception was thrown") — boşluk gerçek |
| Yedek, sayfa ve rol testleri düzeltmeyle (gerçek SQL) | 40/40 geçti |

Düzeltmeden sonraki tam koşu (aynı gün, commit öncesi): 1388 geçti
(Domain 402 · Application 400 · API 327 · Infrastructure 259).

Bakılmayanlar: emülatörde masraflı transfer kaydedilmedi; pasif rol
kategorisinin ekrandaki hatası denenmedi.
