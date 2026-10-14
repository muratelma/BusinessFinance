# Kategori ve hesap sayfaları — teslim ve yol günlüğü

13 Ekim 2026. Dal: `kategori-hesap-sayfalari`. Görev:
[GOREV-2026-10-13-kategori-hesap-sayfalari.md](GOREV-2026-10-13-kategori-hesap-sayfalari.md).

## Teslim ve checkpoint'ler

Yedi paketin sunucu ve Flutter uygulaması tamamlandı. Sunucu Paket 1–4
`3bbc863` checkpoint'indedir; Flutter Paket 5–7, işlem parçasının kapsam eki
ve aşağıdaki son düzeltmeler **bu raporu taşıyan uygulama commit'indedir**.
Dal değiştirilmedi; push/merge yapılmadı. Aşama 06.3 Aktif kalır; bu teslim
bütün aşamanın veya cihaz kabulünün kapanışı değildir.

| Paket | Uygulanan davranış |
|---|---|
| 1 — Rol ve faiz | Dört ad bağımsız rol, açık kapsam/teklik, gerektiğinde açılış; alacak taksidi faiz geliri rolünü kullanır. v12 yazılır, v11 okunur. |
| 2 — Kanonik listeler | `RecognizedItems` ve `AccountMovements` kaynak/bağlı işlem/hesap/kart/kişi/giriş anını taşır. Transfer çıkışı masraf dahil, POS yatışı net tek harekettir. |
| 3 — Transfer masrafı | Önizleme, tek atomik yazma/iptal ve masrafın bağımsız iptal kilidi. Dekont transferi masrafı forma taşır; diğer dekont yollarındaki ayrı gider banka masrafı rolünü kullanır. |
| 4 — Okumalar | Kategori özeti/sayfası/ayları, hesap sayfası/ayları/yolda/liste toplamları, kesişimli menüler ve gerçek parça kategorisi. SQL toplamları, 50 satır sayfalama ve sahiplik denetimleri. |
| 5 — Ortak UI ve Diğer | Çip altı ara başlıklı menü, merkezde ay seçici, gün başlığı, özet kartı ve tek işlem satırı/ayrıntı açıcısı. Diğer'de Hesaplar ve Kategoriler grubu. |
| 6 — Kategori | Gelir/gider ve kapsam/kaynak seçimi; kategori sayfasında kaynak/kişi kesişimi, parça tutarı, faturanın kapanışı, iki kapsamlı kategori satırı. Özet'in kategori kartı ay/kapsamla bu sayfaya gider. |
| 7 — Hesap ve transfer | Hesapta bakiye/giren/çıkan/net, Tür menüsü, net Yolda kutusu ve liste; Hesaplar tek listedir. Yeni transfer ayrı sayfadır; kaydet/geri geldiği yere döner. |

Yeni kütüphane, state paketi, dış servis veya migration eklenmedi. Mevcut
sunucu migration'ı geliştirme hedefine de uygulandı. Kullanıcının görev/devir
notları, kök PNG'leri ve `tasarim-onizleme/` korunup commit dışında bırakıldı.

## Emülatörde bildirilen sunucu hatası

Çalışan 5284 API'si güncel Debug derlemesini kullanırken geliştirme veritabanı
`20261013104629_AddCategoryRolesAndTransferFees` migration'ını bekliyordu.
Dolayısıyla kodun okuduğu `Categories.Role` ve `Transfers.FeeTransactionId`
kolonları şemada yoktu. API açılışta migration uygulamaz; `/health/ready` 200
vermesi tek başına şemanın güncel olduğunu kanıtlamadı.

Sentetik kullanıcıyla aynı API'deki yetkili `/api/v1/categories` isteği
önce **500 `server.unexpected_error`**, incelenmiş veri kaybettirmeyen
migration'dan sonra **200** döndü. `/accounts`, `/category-summary`,
`/accounts/overview` ve `/financial-activities` de **200** ile yeniden okundu.
API durdurulmadı, veri sıfırlanmadı; 5284'ün geliştirme hedefi yükseltildi.
Önce/sonra sonuçları `scratch/schema-recovery-2026-10-13.json` içindedir.

Bu doğrulama gerçek HTTP isteğidir; emülatörde kullanıcı arayüzü kabulü
sayılmaz. ADB devam oturumunda bağlı cihaz göstermedi.

## Kontroller

| Kontrol | Sonuç |
|---|---|
| Backend Release build | 0 uyarı, 0 hata |
| `dotnet format --verify-no-changes` | Temiz |
| Gerçek SQL dahil bütün backend testleri | **1384 geçti**, 2 canlı sağlayıcı testi atlandı |
| Katmanlar | Domain 402 · Application 400 · API 327 · Infrastructure 255 |
| Flutter analyze | Temiz |
| `dart format --set-exit-if-changed lib test` | 354 dosya, 0 değişiklik |
| Tam Flutter test | **1164 geçti**, 180 görüntü/çizim testi normal koşuda atlandı |
| Yeni sayfa + gerçek JSON sözleşmesi | 25 test; büyük yazı, tema, yarış, menü ve transfer kapıları dahil |
| Etkin teslim görüntü koşusu | **51/51 geçti**; A8c/A8d kullanıcı kararıyla kapsam dışı |
| Android debug APK | Derlendi: `mobile/business_finance_mobile/build/app/outputs/flutter-apk/app-debug.apk` |
| Bağımsız gerçek API | **78 sentetik kontrol geçti**, ayrı loopback Release örneği |

Gemini/Brevo canlı anahtarları olmadığı için iki sağlayıcı testi atlandı.
SQL testleri atlanmadı. Son backend logları `scratch/backend-pages-resumed-*`,
TRX kayıtları `scratch/test-results/resumed-pages/`; Flutter logları
`scratch/flutter-pages-resumed-final.log`, `…-shots.log`, `…-apk.log`.

51 gerçek bileşen karesi `scratch/category-account-resumed-shots/kh-*.png`
altında; bütün kareler altı kontakt sayfasıyla teslimin yanında incelendi,
ana filtre/ödeme/Yolda/ay seçici/transfer hâlleri ayrıca tek tek kontrol
edildi. Başarı yalnız test çıkış koduna dayanmaz. Örnek verilerdeki farklı
sayılara aşağıdaki gerekçelerle sunucu cevabı üstün tutuldu.

## Yeni uçlar ve örnek cevaplar

Tam uç tablosu ve JSON sözleşmesi:
[category-account-pages-api-contract.md](../documentation/category-account-pages-api-contract.md).

| Uç | İşlev |
|---|---|
| `GET /category-summary`, `/category-summary/months` | Özet ve seçimin 12 ayı |
| `GET /categories/{id}/page`, `/months` | Kalem listesi ve yıl; kişi/kaynak kesişimi |
| `GET /categories/uncategorized/page`, `/months` | Salt okunur geçmiş |
| `GET /accounts/overview` | Aktif ve pasif hesaplarla doğru toplam |
| `GET /accounts/{id}/page`, `/months`, `/in-transit` | Hareket, ay sonu ve net Yolda |
| `GET /financial-activities/{kind}/{id}/parts` | Gerçek parça tutarı/kategorisi |
| `POST /transfers/preview` | Toplam çıkış ve gerekli masraf kapsamı |
| `POST /categories/bank-fee` | Sahibe ait rol kategorisini bul/aç |

Tabloda bütün yollar `/api/v1` altındadır. Mevcut transfer isteği isteğe bağlı
masraf/kapsam alanlarıyla genişledi. Kimlik oturumdan gelir; para F4 dizedir.

```json
{"year":2026,"month":9,"direction":"expense","scope":"business",
 "total":"26930.0000","previousTotal":"0.0000","categoryCount":7}
```

```json
{"balance":"21376.0000","currentBalance":"21376.0000",
 "inflow":"27936.0000","outflow":"31560.0000","net":"-3624.0000",
 "transit":{"total":"8074.0000","count":1}}
```

Bu örnekler kısaltılmıştır. Tam sentetik cevaplar, kaynak seçimleri ve aylar
`mobile/business_finance_mobile/test/fixtures/category-account-contract.json`
dosyasındadır; token/parola içermez. Kalem `scope`u bağlı nötr işlemin değil
kendi kapsamıdır. Kapanış `settled`, transfer masrafı `fee` olarak okunur.

## Gerçek API denemesi

`http://127.0.0.1:5291` üzerinde ayrı Release örneği kullanıldı; betik 5284'ü
ve loopback dışını reddeder. Brif 5/6 gün numaraları ileri kayıt yasağı
nedeniyle **Eylül 2026**'ya taşındı, tutarlar değiştirilmedi. Bu geçici API
kontrolden sonra yalnız doğrulanmış kendi PID'siyle kapatıldı; 5284 çalışır.

| Karşılaştırma | Brif / beklenen | Gerçek API |
|---|---:|---:|
| Gider / gelir | 26.930 / 44.550 | 26.930 / 44.550 |
| Ziraat gider / kasa / kart / vadeli | 12.440 / 6.950 / 3.930 / 3.610 | Aynı |
| Ziraat giren / çıkan / net | 27.936 / 31.560 / −3.624 | Aynı |
| Ziraat bakiye / tek Yolda | 21.376 / 8.074 | Aynı |
| Hesaplar toplam / nakit / banka / pasif | 48.330 / 12.454 / 35.876 / 0 | Aynı |
| Nakit giren / çıkan / net / bakiye | 16.800 / 18.950 / −2.150 / 8.454 | Aynı; H11'in giren 14.800 örneği eksik |
| Masraflı transfer toplam çıkışı | 10.050 | 10.050 |
| İptal sonrası kaynak bakiye / gider | 31.426 / 26.880 | Aynı |

Kategori kalemleri/menüler, hesap hareketleri/yön menüleri, sahiplik, parça
kapsamı ve iptal kontrol edildi. Brifte geçen ayın kayıtları verilmediğinden
tasarımın geçen ay toplamları yeniden kurulmadı. Geçmiş ay davranışı gerçek
kurulmuş veriyle okundu; tasarımın geçen ay sayısı ürüne sabit yazılmadı.

## Yol günlüğü

| Konu | Uygulanan ve gerekçe |
|---|---|
| A8c/A8d | Kategori atama/geri al yok. Kategorisiz geçmiş mevcut ortak ayrıntıyı açar; kullanıcı kararı §5.1. |
| Pasif hesap | Bütün hesaplar toplamda; aktif Nakit/Banka ve sıfır değilse Pasif hücresi toplamı tutar. `Toplama girmez` yazılmaz. |
| Rol/ad/eskiden eksik rol | Aynı adlı özel kategori korunur; yeni rolün adı numarayla ayrılır. v12 explicit null rolü yeniden tahmin etmez; v11 eski ad eşlemesini kullanır. |
| H1/H2 ve H10b | 8.074 tek satış, 28.300 dört kayıtlı ayrı görsel senaryodur. Üründe toplam ve en yakın satır aynı sunucu kaynağındandır. |
| H11 | 2.000 alacak taksidi tahsilatı da hesaba girer; gerçek nakit giren 16.800'dür. |
| A1/A5 geçen ay | JSX 24.180/41.300 ile brif 24.860/41.400 çelişir; sunucu cevabı gösterilir. |
| H13 ve D3 | Yükümlülük kapanışına transfer masrafı eklenmez; aynı kaynak/hedef transferi geçersizdir. D3 boş/geçerli seçim bekler. |
| Yolda ve parça | Hesap özetinde en yakın tek kayıt + SQL toplam; tam liste ayrı sayfalıdır. Net Yolda satırı komisyon kategorisinin kalemi gibi adlandırılmaz. |
| Ortak ayrıntı | Feed ve yeni sayfalar aynı açıcıyı kullanır. POS'ta mevcut zengin panel, bakiye ve geri alma korunur; ayrı sayfa ayrıntısı kopyası yoktur. |
| Çizilmemiş hâller | Loading/error/401/bayat veri, kayıtsız seçim, masraf kapsamı, rolü yeniden adlandırma, sayfalı devam ve Yolda boş hâli mevcut DS diliyle kuruldu. |
| Ortak bileşenler | Yeni UI paketi yok; mevcut parçalara compact/dashed/ölçek/semantik ayarları eklendi. Büyük yazıda tutar kesilmeden yer değiştirir. |
| Ay seçici | Para/çubuk oranı sunucudan; geçmiş yıl isteği nesil korumalı. Hücrelerde 10 px tutar yığını yerine seçilen ayın tutarı başlıktadır. |
| Menüler | Tümü menüsünde Giren önce, Çıkan sonra; ara başlıklar seçilmez. Etiketler teslimdeki Kategori/Tahsilat/Ödeme/Transfer'dir; makine anahtarı değişmedi. |

## Ek düzeltilen veya değiştirilen yerler

- **Çalışan API / şema uyumsuzluğu:** yukarıdaki gerçek 500 → 200 doğrulaması
  ve veri kaybettirmeyen yükseltme; hata çözüldü. Health tek başına yeterli
  kabul edilmedi. Global exception'ın ayrıntısı kullanıcıya/loga açılmadı.
- **Parçanın kapsamı:** nötr ebeveyn yerine kalemin `scope`u. SQL testi ve
  gerçek JSON sözleşmesi doğrular; ek alan mevcut cevabı kırmaz.
- **Özet'in istemci toplamı:** pay/çubuk artık sunucunun mevcut aylık rapor
  dilim alanlarından gelir.
- **Paralel rol açılışı:** tekil indeksin kaybedeni kazanan rolü okur;
  kullanıcı özel kategorisi dönüştürülmez. Eski kişisel varsayılan setin
  yedek tanınması korundu.
- **Parça ucunda sahiplik:** projeksiyondan önce sahip işlem okunur;
  yok/yabancı aynı 404, boş kimlik listesi sorgusuz dönüş.
- **Görsel hizalar:** mavi Yolda/beyaz en yakın kayıt, kategori ikonları ve
  kesik kategorisiz kart, sabit kart alt satırı, solda filtreler, koyu tarih/
  soluk gün, ikonsuz renkli ödeme durumu ve yön menüsünün sırası düzeltildi.
- **Önizleme ve filtre yarışları:** eski yanıt yeni ayı/seçimi ezmez;
  kaydetme önizleme/kapsam olmadan açılmaz, masraflı transfer bütçeyi yeniler.
- **Ayrıntıda iptal reddi:** panel içinde görünür. Gelen transfer satırında
  çıkan hesabın masrafı tekrar gösterilmez; parça kategorisi addan tahmin edilmez.
- **Eski kopyalar:** transfer sekmesi/liste/form/ayrıntı kopyaları ve artık
  olmayan ekranların testleri kaldırıldı; yeni sayfalar yerini aldı.
  Temizlikte yanlış kaldırılmış `_moneyError` geri kondu ve tam suite geçti.
- **Test altyapısı:** gölge bayrağı test gövdesinde geri yüklenir;
  fixture kaynak GUID'leri gerçek cevaplarla eşlendi, filtreli/tür seçili
  toplamlar düzeltildi. Eski Kapat beklentisi teslimin Vazgeç eylemini sınar;
  More gezinme testi yeni gruplar sonrası satıra kaydırır. Token kapısı
  gevşetilmedi; ham boşluklar mevcut `AppSpacing` değerlerine bağlandı.

## Açık sorular ve denemediklerim

Bu görev için yeni bir bağlayıcı karar sorusu kalmadı. Önceden kabul edilmiş
A8/aktif-pasif hesap/masraf kapsamı/ortak ayrıntı kararları uygulandı.

**Denemedim:** emülatörde kurulum ve kullanıcı turu, gerçek Android cihaz,
canlı TalkBack/VoiceOver, klavyeyle cihaz turu, canlı Gemini/Brevo çağrıları.
ADB bağlı cihaz göstermedi. API'nin 200 dönmesi ve widget erişilebilirlik
kapısı cihaz kabulü olarak sunulmadı. Android debug APK hazırdır.

**Sıradaki tek görev:** yeni APK ile kategori/hesap/Yolda/transfer akışının
cihaz kabulü. Bu işten sonra cari kişi, kredi kartı, POS listesi, ödenen
vergiler veya genel filtreye kendiliğinden başlanmadı; aşama değiştirilmedi.
