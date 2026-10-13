# Kategori ve hesap sayfaları — teslim ve yol günlüğü

Dal: `kategori-hesap-sayfalari`. Görev: `GOREV-2026-10-13-kategori-hesap-sayfalari.md`.
Bu belge çalışma sırasında güncellenir; aşağıdaki tamamlanma ve kontrol
kayıtları yalnız gerçekten koşulan sonuçlarla doldurulur.

## Paketler ve checkpoint'ler

Çalışma sürüyor. Paket 1–4 sunucu checkpoint'i hazır; Flutter henüz yazılmadı.
Commit kimliği checkpoint kaydedilince bu bölüme eklenir.

- Paket 1: dört kategori rolü, ad bağımsızlığı, açık kapsam/teklik, iki sette
  rol eşlemesi ve alacak taksidinin faiz geliri kategorisi; v12 yedek ve v11
  uyumluluğu, dolu şema yükseltmesi.
- Paket 2: iki kanonik kaynak listesinin kaynak/bağlı işlem/hesap/kart/kişi/
  giriş anı bilgisi; masraf dahil transfer çıkışı ve net POS yatışı.
- Paket 3: masraf giderinin tek yazma ve tek iptal bağı, bağımsız iptal kilidi,
  kapsam zinciri, sunucu önizlemesi; dekontun Flutter bağlaması sonraki adımda.
- Paket 4: kategori özeti/sayfası/ayları, hesap sayfası/ayları/yolda/liste
  toplamları ve ortak işlem parçasının gerçek kategorisini okuyan uç.

## Yol günlüğü

| Konu | Karar ve gerekçe |
|---|---|
| Rol kategorisi açılırken aynı adlı özel kategori | Özel kategori yeniden yorumlanmaz; rol kategorisi ayırt edici numarayla açılır. Rolün tekliği kullanıcı ve rol üzerinde korunur. |
| Kategorisiz geçmiş | Salt okunur liste ve ortak işlem ayrıntısı; kategori atama ve geri al akışı uygulanmaz (görev §5.1). |
| Pasif hesap bakiyesi | Toplam bütün hesapları sayar; aktif Nakit/Banka ve sıfır olmayan Pasif hücresi toplama eşittir (görev §5.2). |
| Yedek v12 | Yeni yedekte bilerek boş rolü eski yedekte eksik rolden ayırmak için yeni sürüm yazılır; v11 okunmaya devam eder. |
| API örneğinin ayı | İleri gün yasağı nedeniyle brifin Ekim gün numaraları ve tutarları Eylül'e taşındı; ürünün saat kuralı değiştirilmedi. |
| H11 nakit toplamı | Brif 5'teki 2.000 alacak taksidi tahsilatı da hesaba girer; gerçek giren 16.800'dür. H11 14.800 ile bu satırı dışarıda bırakır. |
| H1/H2 yolda örneği | 28.300 dört satışın, 8.074 tek satışın toplamıdır; gerçek ekran aynı yoldaki kaynak listesinin toplamını ve en yakın kaydını kullanır. |
| A1/A5 geçen ay | JSX 24.180/41.300, brif kategori tabloları 24.860/41.400 yazar; ekrana sabit örnek sayı taşınmaz. |
| H13 ve D3 | Yükümlülük ödemesinde transfer masrafı yoktur; D3'te aynı kaynak/hedef geçerli transfer değildir. Sunucu alanları ve doğrulaması izlenir. |
| Ortak ayrıntı okuması | `parts` ucu her işlemde parça tutarı ve kaydın gerçek kategorisini döner; istemci addan veya rolden kategori tahmin etmez. |
| Yolda özetinin sorguları | Hesap sayfası yalnız en yakın kaydı okur; toplam ve sayı SQL'de tek sorgudadır. Tam yolda listesi 50 satırlık sayfalıdır. |

## Yolda düzeltilen veya değiştirilen yerler

- Özet'in mevcut kategori kartında pay ve çubuk için istemci para topluyordu.
  Mevcut aylık rapor dilim cevabına sunucunun `share` ve `barRatio` alanları
  eklendi; Flutter kartı bu alanlara bağlanacak.
- İlk rolün paralel açılışı tekil indekse çarpan istekte başarısız olabilirdi.
  Başarısız aday izleme bağlamından çıkarılır ve kazanan rol kategorisi okunur;
  dört paralel istek tek kimlikte buluşur (gerçek SQL testi).
- Ortak parça ucunun ilk negatif sahiplik koşusu 500 döndü. Parça
  projeksiyonundan önce mevcut kaydın sahiplik okuması kullanıldı; aynı probun
  tamamı sonrasında geçti. Boş satır kimliği listesi ayrıca sorgusuz döner.
- Kişisel kategori setine yeni rol eklendiğinde eski dokunulmamış setin yedek
  için değiştirilebilirliği kayboluyordu; eski set biçimi de tanınır.

## Kontroller ve gerçek API denemesi

Sunucu: Release build 0 uyarı/hata; format doğrulaması temiz. Son tam
katman koşularında Domain 402, Application 400, API 327, Infrastructure 254
test geçti; iki canlı sağlayıcı testi atlandı. Sonradan eklenen rapor payı
ve boş okuma koruması ilgili koşuda ayrıca doğrulandı: Domain 1,
Application 10, API 7, Infrastructure 18 test geçti. Ayrıntı/rol ek SQL koşusu
2 test geçti. Son toplamlar Flutter teslimi sonunda yeniden kaydedilecek.

Gerçek API: test veritabanına bağlı ayrı Release örneği,
`http://127.0.0.1:5291`. **64 kontrol geçti.** Deneme
`scripts/test-category-account-pages.py` ile tekrar üretilebilir; 5284 portu
betik tarafından reddedilir. Sentetik kullanıcının token/parolası rapora
ve çıktılara yazılmaz. 5284'teki süreç ve veritabanı değiştirilmedi.

| Karşılaştırma | Brif | Gerçek API |
|---|---:|---:|
| Gider | 26.930 | 26.930 |
| Gelir | 44.550 | 44.550 |
| Ziraat gideri | 12.440 | 12.440 |
| Vadeli gider | 3.610 | 3.610 |
| Bonus Kart gideri | 3.930 | 3.930 |
| Ziraat giren | 27.936 | 27.936 |
| Ziraat çıkan | 31.560 | 31.560 |
| Ziraat net | −3.624 | −3.624 |
| Ziraat bakiye | 21.376 | 21.376 |
| Tek yoldaki satış | 8.074 | 8.074 |
| Hesaplar toplamı | 48.330 | 48.330 |
| Transfer önizleme toplamı | 10.050 | 10.050 |

Bütün kategori sayfası satırları, kaynak menüleri, hesap giren/çıkan satırları
ve yön menüleri toplamlara eşit bulundu. Transfer iptali kaynak bakiyesine
10.050'yi geri verdi ve giderden 50'yi kaldırdı. Brifte yalnız geçen ay
toplamları verilmiş, o ayın kayıtları verilmemiştir; geçmiş ayın tasarım
örnekleri bu denemede ayrıca kurulmadı.

## Açık sorular ve denemediklerim

Cihaz kabulü henüz denenmedi.
