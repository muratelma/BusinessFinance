# Backup Restore Runbook

Bu runbook yalnız sentetik yerel veridir. Yazılan şema **v7**; okunabilen
şema **yalnız v7**. v7 karşı tarafın kendisini (ad, not, aktiflik) ve cari
defterin iki hareket türünü taşır: borçlandırma (`counterpartyCharges`) ve
tahsilat (`counterpartyPayments`). Sözleşme karşı tarafı artık **adla değil
kimlikle** gösterir; ad yedeğin içinde tek yerde durur. Restore merge,
overwrite veya kullanıcı seçerek silme yapmaz; hedef kullanıcının finans alanı
boş olmalıdır. Yeni hesapta uygulamanın otomatik oluşturduğu, hiç değiştirilmemiş
başlangıç kategorileri boş alan sayılır ve yedekteki kategorilerle atomik olarak
değiştirilir.

## Veritabanı yükseltme notu — Aşama 03 Grup 2

`AddObligationsAndCounterpartyDueDates` migration'ı yedek şeması sürümünden
ayrı bir SQL yükseltmesidir. Dolu olabilen `CounterpartyCharges` tablosuna
`DueDate date NULL` **varsayılansız** eklenir; geçmiş hareketlerin bilinmeyen
vadesi doldurulmaz. `Obligations` ve `ObligationSettlements` bu adımda boş
doğduğu için zorunlu kolonları backfill istemez. Yükseltme testi önceki
`LinkDebtsToCounterparties` şemasına gerçek bir cari satırı yazar, migration'ı
uygular ve vadenin `null` kaldığını doğrular.

Bu değişiklik backup biçimini v8 yapmaz. Yazılan/okunan dosya hâlâ v7'dir ve
yükümlülükleri veya cari vadesini taşımaz; bunların kayıpsız backup kapsamına
alınması Aşama 03 Grup 7'nin işidir. Bu checkpoint'te v7 restore edilen cari
hareketler bilinçli olarak vadesiz (`null`) doğar.

## Ön koşullar

- SQL Server `healthy`, API `/health/ready` cevabı 200 olmalıdır.
- Backup dosyası `business-finance-backup` formatında ve şeması **v7**
  olmalıdır. v7, v6'nın taşıdığı her şeyin (her finansal kaydın kapsamı
  `scope`, hesap/kategori/kart varsayılan kapsamı `defaultScope`) üstüne cari
  defteri ekler. Borçlandırma kategori ve kapsam taşır, hesap taşımaz;
  tahsilat hesap taşır, kategori ve kapsam taşımaz (ADR 0014) — iki kaydın
  alan listesi dosyada da bilerek farklıdır.
- **Yedek kullanıcı profilini (işletmeniz var mı) taşımaz.** Profil finansal
  bir kayıt değil, bir arayüz tercihidir; geri yüklenen hesabın kendi cevabı
  geçerli kalır. Kategoriler yedekten geldiği için kapsam varsayılanları da
  yedekten gelir ve raporlar doğru bölünür.
- **v2–v6 yedekleri `restore.unsupported_version` ile reddedilir ve
  yükseltilmez.** v2–v5'te kapsam alanı yoktu; v6'da cari defter yoktu —
  o dosya karşı tarafı yalnız sözleşmenin taşıdığı ad olarak biliyordu, açık
  bakiyesi ve hareketleri hiç yoktu. Eksik alanı doldurmak için bir değer
  seçmek, kullanıcının işletme ile cebi arasındaki ayrımını (ADR 0013) ya da
  alacağını uydurmak olurdu; ikisini de yalnız kullanıcı bilir. Reddetme,
  sessizce yanlış bir geçmiş üretmekten iyidir. Eski bir yedeği taşımanın
  yolu yoktur; o veri sentetiktir ve yeniden girilir.
- Dosya en fazla 14 MiB envelope, decoded payload en fazla 10 MiB olmalıdır.
- Hedef kullanıcıda herhangi bir finans veya attachment metadata kaydı olmamalıdır.
  Yalnızca eksiksiz, aktif ve değiştirilmemiş başlangıç kategori setine izin verilir;
  özel, yeniden adlandırılmış veya pasif kategori hedefi dolu yapar.
- Gerçek veri kullanılmaz; dış paylaşım için backup şifreli değildir.

> **Yedek ile CSV dışa aktarma aynı şey değildir.** İşlem CSV'si okumak ve
> arşivlemek içindir; kaydın kapsamını (`scope` kolonu) taşır ama geri
> yüklenemez — gelir/gider türü ve kapsam ayrı kolonlarda durduğu için banka
> ekstresi importer'ına güvenle verilemez ve verilirse her hareket ikinci kez
> yazılır. Veriyi bir hesaptan diğerine taşımanın **tek** yolu bu runbook'taki
> yedek/geri yükleme akışıdır. İstemci, kendi dışa aktarımını içe aktarma
> ekranında tanır ve reddeder.

> **Cari defterin kendi CSV'si vardır** (`/api/v1/exports/counterparty-ledger.csv`).
> İşlem CSV'sine karşı taraf kolonu **eklenmedi**: o dosya `BudgetTransaction`
> tablosunun dökümüdür ve cari hareket orada hiç bulunmaz — kolon her satırda
> boş kalırdı. Her dışa aktarma tek kaydın dökümü olduğu sürece kullanıcı ne
> okuduğunu bilir. Cari CSV'si de geri yüklenemez.

> **Önceki uygulamanın yedekleri okunmaz.** Kişisel bütçe uygulaması
> `personal-budget-backup` format kimliğiyle ve `.pbbackup.json` uzantısıyla
> yazıyordu; bu uygulama `business-finance-backup` ve `.bfbackup.json`
> kullanır ve farklı format kimliğini `restore.invalid` ile reddeder. Bu
> bilinçlidir: iki uygulamanın verisi karışmaz. Devralınan veri dosya üzerinden
> değil, SQL seviyesinde `BACKUP`/`RESTORE` ile taşındı (bkz. ADR 0012).

## Tatbikat

1. Kaynak sentetik kullanıcıyla Flutter `Veri araçları > Yedek` ekranından backup
   oluştur ve güvenli yerel hedefe paylaş.
2. Boş ikinci sentetik kullanıcıyla giriş yap.
3. `Backup doğrula ve geri yükle` seçeneğiyle dosyayı seç.
4. Schema, entity count ve checksum özetini kontrol et; yalnız beklenen dosyada
   ikinci onayı ver.
5. Restore sonrası hesap/transaction, borç, hedef ve attachment listelerini aç.
   **Cari hesabı da aç:** karşı taraf listesi, notu, pasif olanlar ve her
   birinin açık bakiyesi kaynaktakiyle aynı olmalı. Bakiye kalıcı kolon
   değildir; hareketler eksik gelseydi bakiye sessizce küçülürdü.
6. **Kapsamı doğrula:** Özet ekranında anahtarı `İşletme` ve `Şahsi`
   konumlarına al; iki tarafın gelir/gider toplamları kaynaktakiyle aynı
   olmalı. Bir hesabın ve bir kategorinin varsayılan kapsamının da geri
   geldiğini kontrol et — gelmezse yeni kayıtlar zinciri çözemez ve
   `*.scope_unresolved` ile reddedilir.
7. En az bir attachment'ı indir; kaynakla SHA-256/byte eşitliğini otomasyon doğrular.
8. Kaynak kullanıcıya geri dönerek kaynak kayıtların değişmediğini kontrol et.

## Beklenen hata davranışları

| Durum | Beklenen sonuç |
|---|---|
| Bozuk byte/hash veya eksik attachment | 400; hedefe yazma yok |
| Schema v2–v6 veya gelecek bir schema | 422 `restore.unsupported_version`; yükseltme denenmez |
| Dolu hedef kullanıcı | 409 destination not empty; mevcut kayıt korunur |
| SQL constraint/save hatası | Transaction rollback; hedef SQL graph'ı boş |
| Object write sonrası SQL hatası | O çağrıda yazılan object key'ler silinir |
| API/SQL bağlantı kesilmesi | Başarı varsayılmaz; hedef ve trace tekrar incelenir |

## Geri dönüş ve inceleme

Restore “undo” endpoint'i içermez. Başarısız restore atomik rollback yapmalıdır;
manuel tablo silme uygulanmaz. Beklenmeyen kısmi durum görülürse servisi durdur,
sentetik hedefi izole et, trace id ve DB/object metadata sayımlarını kaydet. Kaynak
backup dosyasını değiştirme. Gerçek veri için olay müdahalesi, şifreleme anahtarı,
retention ve yönetilen object store prosedürü bulut aşamasından (Aşama 06) önce ayrıca yazılmalıdır.
