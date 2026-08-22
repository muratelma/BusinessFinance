# Backup Restore Runbook

Bu runbook yalnız sentetik yerel veridir. Yazılan şema **v6**; okunabilen
şema **yalnız v6**. Restore merge,
overwrite veya kullanıcı seçerek silme yapmaz; hedef kullanıcının finans alanı
boş olmalıdır. Yeni hesapta uygulamanın otomatik oluşturduğu, hiç değiştirilmemiş
başlangıç kategorileri boş alan sayılır ve yedekteki kategorilerle atomik olarak
değiştirilir.

## Ön koşullar

- SQL Server `healthy`, API `/health/ready` cevabı 200 olmalıdır.
- Backup dosyası `business-finance-backup` formatında ve şeması **v6**
  olmalıdır. v6, her finansal kaydın kapsamını (`scope`) ve hesap/kategori/kart
  varsayılan kapsamını (`defaultScope`) taşır.
- **Yedek kullanıcı profilini (işletmeniz var mı) taşımaz.** Profil finansal
  bir kayıt değil, bir arayüz tercihidir; geri yüklenen hesabın kendi cevabı
  geçerli kalır. Kategoriler yedekten geldiği için kapsam varsayılanları da
  yedekten gelir ve raporlar doğru bölünür.
- **v2–v5 yedekleri `restore.unsupported_version` ile reddedilir ve
  yükseltilmez.** O dosyalarda kapsam alanı yok; eksik alanı doldurmak için bir
  değer seçmek, kullanıcının işletme ile cebi arasındaki ayrımını uydurmak
  olurdu ve bu ayrımı yalnız kullanıcı bilir (ADR 0013). Reddetme, sessizce
  yanlış etiketlenmiş bir geçmiş üretmekten iyidir. Eski bir yedeği taşımanın
  yolu yoktur; o veri sentetiktir ve yeniden girilir.
- Dosya en fazla 14 MiB envelope, decoded payload en fazla 10 MiB olmalıdır.
- Hedef kullanıcıda herhangi bir finans veya attachment metadata kaydı olmamalıdır.
  Yalnızca eksiksiz, aktif ve değiştirilmemiş başlangıç kategori setine izin verilir;
  özel, yeniden adlandırılmış veya pasif kategori hedefi dolu yapar.
- Gerçek veri kullanılmaz; dış paylaşım için backup şifreli değildir.

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
6. En az bir attachment'ı indir; kaynakla SHA-256/byte eşitliğini otomasyon doğrular.
7. Kaynak kullanıcıya geri dönerek kaynak kayıtların değişmediğini kontrol et.

## Beklenen hata davranışları

| Durum | Beklenen sonuç |
|---|---|
| Bozuk byte/hash veya eksik attachment | 400; hedefe yazma yok |
| Schema v1/gelecek schema | 422 unsupported version |
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
