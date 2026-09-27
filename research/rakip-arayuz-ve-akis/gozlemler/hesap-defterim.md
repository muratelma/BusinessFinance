# Uygulama Gözlem Formu — Hesap Defterim (Cash Book)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Hesap Defterim (Cash Book) / **ANKIT SARAF** |
| Sürüm | `versionName=235` (`versionCode=235`), targetSdk 36; Play'de "Son güncelleme 23 Ağu 2026", yenilik notu: *"Her kayıtta tarihi gösterme seçeneği eklendi (Ayarlar'a bakın)"* (`00-magaza.png`). Sürüm numarası bu karede görünmüyor; koşum notudur *(P1-hesap-defterim-G01)* |
| Test tarihi | **10 Eylül 2026** (Tur 1, K00–K08 + arayüz taraması) · **11 Eylül 2026** (ek koşum 2) · **12 Eylül 2026** (Faz 7.5 doğrulama turu) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400, Android 17) |
| Dil / para birimi | **Türkçe arayüz** (sistem dili) / para birimi simgesiz sayı (₺/simge gösterilmedi; "Para birimi biçimi" ayarı var) |
| Hesap veya plan türü | Ücretsiz — "Reklam içerir • Uygulama içi satın alma" (reklam kaldırma IAP). Ekranda AdMob banner + navigasyonda tam ekran geçiş reklamı (`39-gecis-reklami-interstitial.png`) |
| Erişim kısıtı | **Yok** — kayıt/giriş/e-posta/telefon/VKN **hiç istemiyor**; uygulama içi diyalog kayıtların sunucuda saklanmadığını söylüyor (beyan; ağ trafiği denetlenmedi, Drive yedeği ve e-posta ayarı dış temas noktası — P1-B11). Açılışta yalnız "nasıl kullanılır" bilgi diyaloğu + Google Drive yedekleme daveti |
| İnceleme türü | Manuel gözlem |
| Mağaza verisi | **4,8★ · 139 B yorum · 10 Mn+ indirme · PEGI 3.** Etiketler: İş, Giderler, Muhasebe. Tez: "Günlük giderler akışını, günlük nakit dengesini yönetin." Bu değerler `00-magaza.png` karesinde görünmüyor (karede yalnız geliştirici, reklam/IAP, güncelleme tarihi ve yenilik notu var); görsel kanıtı olmayan koşum notudur *(P1-hesap-defterim-G01)* |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Tek sütunlu **yürüyen bakiye defteri**: para girdi (Alındı) / çıktı (Ödendi) → Denge. "Khatabook" türünün Türkçe'ye yerelleşmiş hâli. |
| Asıl hedef kullanıcı | Esnaf / küçük dükkân sahibi; elle kasa/veresiye defteri tutan kişi. Kişisel bütçeci de kullanabilir (arayüz yeniden adlandırılabiliyor). |
| Çözdüğü ana iş | Günlük nakit hareketini ve o anki bakiyeyi kâğıt deftere yazmak yerine telefonda tutmak; PDF/Excel çıktı ve Drive yedeği almak. |
| Açıkça kapsam dışı bıraktığı | Kategori sistemi, hesap türleri (nakit/banka/kart), işletme/şahsi ayrımı, fatura nesnesi, cari/müşteri kaydı (kişi olarak), banka bağlantısı, vergi, tekrarlayan/planlı işlem, bütçe. |
| **Bu eksikler ürün hattı kararı** | Drawer'ın dibindeki **"Diğer uygulamalar"** bölümü aynı geliştiricinin iki ayrı uygulamasını tanıtıyor: **`Veresiye Defteri` — "Borcu yönet"** ve **`Gelir Gider` — "Kategori bilge harcamaları yönetin"**. Yani cari/veresiye **ve** kategorili gelir-gider, bu üründen çıkarılmış değil, **başka ürünlere ayrılmış**. `42-drawer-diger-uygulamalar-veresiye-gelirgider.png` |
| İş modeli | Ücretsiz + reklam + tek seferlik "reklamları kaldır" IAP; üç uygulamalık bir aile. |
| BusinessFinance ile aynı kulvarda mı | **Hayır** — bizde çok hesaplı havuz + kapsam boyutu + kart borcu/ekstre + planlama tek üründe; burada bu işlerin bir kısmı ayrı uygulamalarda. Ortak nokta yalnız elle giriş olması; BusinessFinance'te veresiye satış ve kart harcaması ödeme gününde değil olay gününde tanındığı için ürün saf nakit defteri değildir. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki metin ortak noktayı "nakit esaslı" diye yazıyordu.)* |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 0 | Kayıt/giriş **hiç yok**; ilk açılışta "Uygulama nasıl kullanılır?" bilgi diyaloğu (TAMAM MI) + hazır "Hesap Defterim" defteri, anında kullanılabilir | Açılıştan itibaren **agresif Google Drive yedekleme daveti**; uygulama verilerin sunucuda saklanmadığını söylüyor ("Kayıtlarınızı sunucularımızda saklamıyoruz"; beyan, ağ trafiği denetlenmedi — P1-B11). Diyaloğun kendi metni ana ekranın butonlarıyla **uyuşmuyor**: "Ücretli butonuna basıp…", "Alınan butonuna basarak…" derken ekrandaki butonlar `Ödendi` / `Alındı` | `01-ilk-acilis-hosgeldin.png` |
| K01 Ana ekran | Tamamlandı | 0 | İki dev renkli buton: **Alındı** (yeşil) / **Ödendi** (kırmızı); üstte `Herşey` / `Günlük` / `Haftalık` / `Aylık` / `Yıllık`; altta `Toplam Alındı` / `Toplam Ödendi` / **`Denge`** | Alt kısımda kalıcı AdMob banner (Test Ad); üst bölge yedekleme + reklam kaldırma davetleriyle dolu | `02-bos-ana-ekran.png`, `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı (uyarlanarak) | ~6/defter | **Nakit/banka/kart kavramı yok.** "Hesap" = ayrı bir **defter**, her biri kendi yürüyen bakiyesiyle. `HESAP EKLEM` → `İsim` + `Açılış bilançosu [İsteğe bağlı]` + `+`/`−` radyo + tarih | Defter ≠ hesap türü; para birimi/tür sorulmuyor. **`Hesaplar` listesi ve defter seçici yalnız isim gösteriyor, bakiye göstermiyor** — hangi defterde ne olduğunu görmek için deftere girmek gerekiyor. Açılış bakiyesi tarihi ayrı bir davranış üretiyor (aşağıda) | `10-hesap-eklem-formu.png`, `11-hesaplar-defterler-listesi.png` (listede varsayılan `Hesap Defterim` dahil dört defter) |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~5 | Form tek ekran: tür (`Alındı`/`Ödendi`) + tarih + saat + tutar + `Notlar` (+ isteğe bağlı `Açıklama / Kategori`) + `Fatura ekle` + `Öğe eklemek`. `Kaydet ve çık` / **`Kaydet ve devam Et`** (seri giriş) | **Kategori yok, müşteri alanı yok** → "Ada Reklam" yalnız `Notlar`'a yazıldı. Liste satırında başlık = `Notlar` metni | `04-islem-formu-alindi.png` → `03-dolu-ana-ekran.png` (04'te tarih alanı henüz Eyl-10; kayıt Ağu 03'e alınmış) |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~5 | `Ödendi` butonu → aynı form, kırmızı; `Ortak Cuzdan` defterine −850 yazıldı, o andaki Denge 1.150 | **İşletme/şahsi (kapsam) ayrımı hiç yok.** Protokol gereği kategoriyle taklit edilmedi → bu boyut `Desteklenmiyor` | `38-ortak-cuzdan-defteri.png` |
| K05 İşletme kart gideri | Tamamlandı (kart modeli yok) | ~5 | `Is Karti` defterine −1.200 `Ödendi`; defter bakiyesi **−1.200'e düştü** | **Kart borcu / ekstre / kesim tarihi kavramı yok.** Kart = sadece eksiye giden bir defter. Kredi limiti, "ödenecek vs kesilmemiş" ayrımı yok | `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı (ayrışmıyor) | ~6 | Menü → **`Aktar`**: `Miktar` + `Kimden:` (defter) + `Kime` (defter) + tarih + saat + `Notlar`. Kaynak −, hedef + | Transfer **iki ayrı defter satırı** üretiyor (`Kime Ortak Cuzdan` / `Kimden: Ana Hesap`) ve her ikisi de o defterin **`Toplam Alındı`/`Toplam Ödendi`sine giriyor** — gelir/giderden ayrışmıyor | `12-aktar-transfer-formu.png`, `06b-islemler-butun-hesaplar.png` |
| K07 Liste ve rapor | Tamamlandı (rapor zayıf) | ~3 | **`İşlemler-Bütün Hesaplar`** (birleşik defter) Denge **44.950** = kontrol net varlık ✓. **`Özet` / `Hesaplar Özet`** = gün gün `Alındı` / `Ödendi` / **`Tasarruf`** (net). **`Bildiri`** = PDF/Excel çıktı | Kategori kırılımı, işletme/şahsi kırılımı, grafik **yok**. `Toplam Alındı` 51.200 / `Toplam Ödendi` 6.250 = açılış bakiyesi + transferler + gerçek gelir/gider **karışık** | `07-rapor-aylik-butun-hesaplar.png`, `13-ozet-tasarruf-agustos.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~2–3 | Satıra dokun → **`İşlemi Düzenle`** (satır içi tam düzenleme + `Hesaplar` alanıyla **başka deftere taşıma** + üst barda kopyala ikonu). Alt barda kırmızı `Silme` + mavi `Kayıt etmek`. Sil → tek onay → **`Silinmiş işlemler`** çöp kutusu → `Geri Yükle` | Toast'ta anlık "geri al" yok; çöp kutusundan restore gerekiyor. Formdan "geri" ile çıkınca **taslak uyarısı yok**, girilen tutar sessizce kayboluyor (12 Eyl'de yeniden doğrulandı: ₺777 yazılıp geri basıldı, kayıt oluşmadı, Denge değişmedi) | `25-fatura-tam-ekran-goruntuleme.png` (düzenleme ekranı ve kopyala ikonu; Hesaplar/taşıma alanı bu karede diyalog altında görünmüyor — G02), `08-silinmis-islemler.png` |

## Kontrol değeri doğrulaması

5 çekirdek işlem (kart ödemesi dâhil), Ağustos 2026 tarihleriyle, 3 defter
(`Ana Hesap` +20.000 / `Ortak Cuzdan` +2.000 / `Is Karti` 0 açılış).

| Değer | `SENTETIK-TEST-VERISI` beklenen | Hesap Defterim | Durum | Kanıt |
|---|---:|---:|---|---|
| `Ana Hesap` bakiyesi (Denge) | 40.800,00 | 40.800 | ✓ | `03-dolu-ana-ekran.png` |
| `Ortak Cuzdan` bakiyesi (Denge) | 4.150,00 | 4.150 (2.000 + 3.000 transfer − 850) | ✓ | `38-ortak-cuzdan-defteri.png` |
| `Is Karti` bakiyesi (kart ödemesinden sonra) | 0,00 | 0 (−1.200 + 1.200) | ✓ | `13-ozet-tasarruf-agustos.png` (Ağustos), `14-yedekleme-nag-dialog.png` (Herşey, Denge 0) |
| Net varlık (birleşik Denge) | 44.950,00 | **44.950** | ✓ | `06b-islemler-butun-hesaplar.png`, `07-rapor-aylik-butun-hesaplar.png` |
| Ağustos "gelir" toplamı | 25.000,00 | **yok** — `Toplam Alındı` 51.200 (açılış 22.000 + gelir 25.000 + transfer bacakları 4.200; açılış satırları karede kesik, 22.000 aritmetikle bulunur) | ✗ kirli | `06b-islemler-butun-hesaplar.png` |
| Ağustos "gider" toplamı | 2.050,00 | **yok** — `Toplam Ödendi` 6.250 (gider 2.050 + transfer bacakları 4.200) | ✗ kirli | `06b-islemler-butun-hesaplar.png` |

**Sonuç:** Bakiye ve net varlık modeli tutuyor (Denge her yerde birebir).
Uygulamanın **ayrı bir gelir/gider toplamı yoktur**: `Toplam Alındı`/`Toplam
Ödendi` açılış bakiyesini ve transfer bacaklarını da içine katar.

**Ne kazandırıyor:** kullanıcının öğrenmesi gereken tek kavram var — "girdi /
çıktı / kalan". Transferi ayrı modellemeyen bir defterde hiçbir hareket
"görünmez" olmuyor, her satır listede duruyor ve toplamlar satırların
doğrudan toplamı olduğu için doğrulanabiliyor.
**Ne kaybettiriyor:** "bu ay ne kazandım" sorusunun cevabı yok; kullanıcı
transferini ve açılış bakiyesini kafasından çıkarmak zorunda. İki defter
arasında para gezdiren biri, gezdirdikçe kendi cirosunu şişmiş görüyor.

## Ek koşum: A / B / B1 / B2 (kredi kartı kısmi ödeme, fiş OCR, tekrarlayan, taksit)

Sonradan standarda eklenen 4 test, `SENTETIK-TEST-VERISI.md` tutarlarıyla
koşuldu (10 Eyl 2026).

| Test | Beklenen | Hesap Defterim | Sonuç | Kanıt |
|---|---|---|---|---|
| **A — Kredi kartı kısmi ödemesi** | Kart ödemesinde kısmi tutar girilebiliyor mu | **Evet, doğal olarak.** `Is Karti`'ye −₺1.000 "Test taksit ekipman" borç, sonra `Aktar` ₺400 (`Ana Hesap`→`Is Karti`) → `Is Karti` −1.000'den **−600'e**. Kart ödemesi jenerik bir `Aktar` olduğu için tutar zaten serbest; "ekstre öde" / "asgari tutar" / ödeme vadesi gibi override edilecek bir kavram **yok** | `Tamamlandı` | `18-kismi-odeme-is-karti.png` |
| **B — Fiş / kamera** | OCR mu, sadece fotoğraf mı | **Sadece dosya eki.** `Fatura ekle` → `Kamera` / `Fotoğraf Galerisi` / `PDF`. Otomatik okuma/ayrıştırma yok | `Tamamlandı` | `22-fatura-ekle-secenekler.png` |
| **B1 — Tekrarlayan gider (₺600/ay, ilk çekim 10 Ağu, aylık)** | Tanım nasıl kuruluyor; otomatik mi onaylı mı düşüyor | **Özellik YOK.** İşlem formunda, form 3-nokta menüsünde, ana ekran 3-nokta menüsünde, drawer'ın 17 kaleminde ve `Ayarlar`'ın 21 kaleminde hiçbir **tekrarla / planlı / hatırlatıcı / abonelik** seçeneği yok. Kullanıcı her ay eliyle girer | `Desteklenmiyor` | `36-drawer-menu-ust.png`, `42-drawer-diger-uygulamalar-veresiye-gelirgider.png`, `15-ayarlar.png`, `43-ayarlar-alt-bolum-donem-baslangici.png` |
| **B2 — Taksitli kart harcaması (₺6.000 = 6×₺1.000, ilk taksit 15 Ağu)** | Taksit planı nasıl kuruluyor; ekstreye nasıl bölünüyor | **Özellik YOK.** Taksit planı kavramı yok. Kullanıcı ya tek ₺6.000 `Ödendi` kaydı, ya da elle 6 ayrı ₺1.000 kaydı açar. "6 taksit" bilgisi hiçbir yere taşınmaz | `Desteklenmiyor` | — (negatif; yukarıdaki beş tarama karesi) |

**Ek bütünlük bulgusu — transferin öksüz bacağı:** `Aktar` listede **iki ayrı işlem
satırı** olarak görünüyor (`Kime X` / `Kimden: Y`); arka planda tek nesne mi iki
kayıt mı saklandığı ekrandan çıkarılamaz *(P1-B12 düzeltmesi: önceki metin "tek
çift-kayıt nesnesi değil, iki bağımsız satır olarak saklanıyor" diyordu)*. A testinin temizliğinde `Is Karti` tarafındaki ₺400 bacağı
silindi; `Ana Hesap` tarafındaki −₺400 bacağı **silinmeden kaldı** → net varlık
sessizce yanlışa döndü (40.800 yerine 40.400). Aynı şekilde bir bacağın
tutarı/tarihi düzenlenince diğeri güncellenmiyor.
`19-transfer-bacagi-desync.png`
*(P1-hesap-defterim-G01 sınırı: kare yalnız `Ana Hesap` defterini gösteriyor;
40.400 bu defterin 400'lük transfer sonrası bakiyesidir ve iki bacak da
dururken aynı çıkar. Karşı bacağın silindiği, birleşik net varlığın bozulduğu
ve düzenleme davranışı bu karede görünmüyor; bunlar koşum anlatımıdır, görsel
kanıtı yok. Saklama yapısı ifadeleri P1-B12'de ele alınır.)*

**Ek gözlem — geçiş reklamı:** Drawer'dan bir ekrana geçerken ücretsiz sürümde
**tam ekran geçiş reklamı** (interstitial) açılıyor; `Kapat` düğmesi birkaç
saniye pasif kalıyor. Kalıcı banner'a ek olarak navigasyonda kesintili reklam
var. 12 Eyl'de yeniden görüldü. `39-gecis-reklami-interstitial.png` *(G02 sınırı:
karede `Kapat` onay işaretiyle etkin görünüyor; pasif kalma süresi ve 12 Eylül
tarihi statik kareden doğrulanmaz.)*

## Ek koşum 2: uygulamaya özgü derinleştirme (11 Eylül 2026)

Kullanıcı isteğiyle, formun kendine özgü kalan boşlukları + uygulamaya has
özellikler canlı test edildi. Yeni sentetik veri: "Ofis malzemesi" gideri
₺150 (20 Ağustos 2026, öğe dökümlü) ve "Danismanlik geliri" ₺2.500
(22 Ağustos 2026). Bu koşumun kareleri aşağıdaki tabloda tek tek anılıyor.

**Ara doğrulama karesi:** `35-kontrol-degeri-geri-yuklendi.png`, test kayıtları
**geçici olarak silinmişken** alınan kontrol görüntüsüdür (birleşik Denge
₺44.950) — nihai durum değildir, aşağıdaki "Metodoloji notu"na bakın.

**Metodoloji notu (düzeltildi, 11 Eyl):** Bu iki test kaydı, testler bitince
yapay zekâ tarafından **kullanıcıya sorulmadan** silinmişti ve kontrol değeri
(`Ana Hesap` ₺40.800 / net ₺44.950) geri yüklenmişti. Bu, Money Manager /
Wallet / Bluecoins'in boşluk koşumunda (10 Eyl) izlenen yöntemle
**tutarsızdı**: o üç uygulamada B1/B2/A ek koşum verisi bilerek temizlenmedi,
"Tur 2'ye seçilirse veri sıfırlanıp yeniden kurulur" notuyla cihazda
bırakıldı. Kullanıcı bunu fark edip düzeltilmesini istedi: **iki kayıt aynı
tutar/tarih/öğe dökümü/fatura ekiyle emülatörde yeniden oluşturuldu** ve bu kez
**silinmeden bırakıldı**. Kural sabitlendi: **ek koşum test verisi hangi
uygulamada olursa olsun varsayılan olarak silinmez**, yalnız kullanıcı açıkça
isterse silinir.

| Test | Bulgu | Sonuç | Kanıt |
|---|---|---|---|
| **`Öğe eklemek` tam akış** | Kalem kalem döküm dialoğu (`Öğe` / `Miktar` / `Birim` / `Fiyat` → `Ekle`); "Kagit 5 adet @ 20 = 100" + "Kalem 10 adet @ 5 = 50" eklendi, dialog içi `Öğe (2)` / `Toplam 150` anında hesaplandı. `TAMAM MI` → ana forma dönünce **tutar alanı otomatik 150 oldu VE `Notlar` alanına kalem dökümü otomatik yazıldı**. Liste satırında bu `Notlar` metni başlık olarak görünüyor. Ürün kataloğu/stok yok, her kalem her seferinde elle yazılıyor | `Tamamlandı` — gerçek bir alt-toplam motoru | `20-oge-eklemek-dialog.png` → `21-oge-eklemek-tutar-notlar-otomatik.png` |
| **`Fatura ekle` tam akış** | `Kamera` / `Fotoğraf Galerisi` / `PDF` seçenekleri doğrulandı. `Fotoğraf Galerisi` emülatörde boş çıktı (ortam kısıtı, uygulama sınırlaması değil). `Kamera` gerçek izin diyaloğu istedi, çekim + onay akışı çalıştı, form altına küçük thumbnail + kaldır ikonu eklendi *(G02 sınırı: izin ve çekim onay ekranı kalıcı kanıtta yok; önizleme ve tam ekran görüntüleme `25-…` karesinde)*. Kayıttan sonra listede **ataç (📎) ikonu** görünüyor; düzenleme ekranında thumbnail'e dokununca **tam ekran görüntüleme diyaloğu** (`İptal etmek` / `Silme`) açılıyor. OCR yok, salt ek | `Tamamlandı` | `22-fatura-ekle-secenekler.png` → `24-kayit-eklendi-atac-ikonu.png` → `25-fatura-tam-ekran-goruntuleme.png` |
| **`İşlem adları` — `Özel` tam test** | Diyalogdaki üç seçenek ekranda **`Ödendi Alındı` / `Gelir Gider` / `Özel`**. `Özel` seçilince iki serbest metin alanı çıkıyor: `Aldığınız para için ad` / `Verdiğiniz para için ad`. "Tahsilat" / "FaturaOdemesi" girilip kaydedildi → **uygulama genelinde her yer değişti**: ana ekran kolon başlıkları, iki dev buton, alt toplam etiketleri, yeni işlem formunun başlığı/sekmesi/alan etiketi, **PDF export'un sütun başlıkları** (bu son etki karelerde yok, koşum notu — G02). Yeni adla oluşturulan kayıt normal davranıyor (sadece etiket değişimi; üçüncü bir işlem türü veya kategori eklemiyor) | `Tamamlandı` — global yeniden adlandırma, yeni tür değil | `05-siniflandirma-islem-adlari.png`, `26-islem-adlari-ozel-form.png` → `27-yeniden-adlandirilmis-basliklar.png` |
| **Zorunlu alan / boş tutar** | Tutar boş bırakılıp `Kaydet ve çık` denendiğinde **hiçbir hata mesajı, toast veya inline uyarı çıkmıyor; form da kapanmıyor** — sessiz no-op. (Tutar alanının etiketi `Ödendi` kayıtlarında kırmızı görünüyor, ama bu hata işareti değil, türün rengi.) 12 Eyl'de yeniden ölçüldü, davranış aynı | `Belirsiz` davranış — kullanıcıya geri bildirim yok | `28b-bos-tutar-sessiz-red.png` (11 Eylül karesi, yalnız boş form; basma sonrası an ve 12 Eylül ölçümü karede yok — G02) |
| **Sıfır (₺0) tutar** | ₺0 ile kaydetme **sessizce kabul ediliyor**, listeye başlıksız bir "0" satırı giriyor, Denge'ye etkisi yok | `Tamamlandı (uyarısız)` | `28-sifir-tutar-kabul-edildi.png` |
| **Arama** | Üstteki büyüteç → gerçek zamanlı canlı filtre. "Ada" yazınca yalnız eşleşen kayıt (`Notlar`/`Açıklama` alanında arıyor) kaldı, **alt toplamlar da filtrelenmiş sonuca göre yeniden hesaplandı** (25.000 / 0 / 25.000); satırın yürüyen Denge'si de filtre içinde yeniden hesaplanıyor (25.000). Karede eşleşme Notlar metninde *(G02)* | `Tamamlandı` | `29-arama-canli-filtre.png` |
| **Kalıcı silme (çöp kutusu)** | `Silinmiş işlemler` ekranında kayda basılı tutma → **`Geri Yükle` / `Silme`** context menüsü. `Silme` → ikinci bir onay diyaloğu (`İşlemi Sil` / `İptal etmek` / `Silme`) → kalıcı silme. **İki aşamalı silme modeli**: normal silme (soft-delete, çöp kutusuna düşer) + çöp kutusundan onaylı kalıcı silme | `Tamamlandı` | `32-silinmis-islemler-context-menu.png` → `33-kalici-silme-onay.png` |
| **`Bildiri` (PDF/Excel) gerçek dosya** | İki ayrı çıktı yolu var: drawer'daki **`Bildiri-Bütün Hesaplar`** (`Herşey` / `Tarih Aralığı Seçin` + `PDF`·`EXCEL` radyo + `TAMAM MI`) ve üst bardaki **defter başına `Bildiri`** (yalnız `PDF` / `Excel` listesi, dönem sormaz). İkisi de Android paylaşım sayfasını açıyor ("2 dosya paylaşılıyor"). PDF içeriği doğrulandı: tam biçimlendirilmiş tablo (Tarih / Notlar / Açıklama-Kategori / Alındı / Ödendi / Hesaplar sütunları, renkli, alt toplamlarla). Dosya adı kaynağa göre değişiyor: defter başına `Ana Hesap Eyl-12-2026.pdf`, bütün hesaplar `hesapdefterim Eyl-11-2026.pdf` *(G02 sınırı: PDF içeriği ve dosya adları hiçbir karede yok, koşum notudur; "2 dosya paylaşılıyor" `41-bildiri-kasadefteri-uyarisi.png` karesinde görünüyor)* | `Tamamlandı` — sağlam export | `34-bildiri-pdf-excel-secim.png`, `40-bildiri-defter-basina-pdf-excel.png` |
| **`Not Defteri` (uygulamaya özgü)** | Menüden ayrı bir ekran: tarih/saat + serbest metin not + **checkbox** (tamamlandı/beklemede) + `+` FAB + üst barda arama ve takvim ikonu. Alt özet `Tamamlandı` / `Beklemede` / `Toplam` sayaçları tutuyor. Tarih aralığı filtresi (`Herşey`/`Günlük`/`Haftalık`/`Aylık`/`Yıllık`) var. Kayıt silme tek onaylı **kalıcı** silme (işlemlerin çöp kutusu modeli burada yok). Fiilen basit bir görev listesi — muhasebe kaydıyla hiçbir bağı yok | `Not` | `30-not-defteri-checklist.png` |
| **`Nakit Hesap Makinesi` (uygulamaya özgü)** | kupür değerlerine göre (200'den 0,05'e; karede para simgesi yok, TL çıkarımı — G02) adet × kupür = tutar hesaplayan bir kasa sayım aracı. Kupür adedi girilince satır toplamı VE genel `Toplam` canlı hesaplanıyor; üstteki serbest tutar kutusuna da otomatik yazılıyor. Muhasebe kaydı üretmiyor, salt hesap makinesi | `Not` | `31-nakit-hesap-makinesi.png` |

**Ek bulgu — `kasadefteri` uyarı metni yanlış:** Export diyaloğu
*"Dışa Aktarılan Veriler — Veriler, SD kartta veya Dahili Depolamada
**kasadefteri** adlı bir klasöre kaydedilir."* diyor. 12 Eyl'de cihazda
doğrulandı: **böyle bir klasör yok**; dosyalar `Documents/Hesap Defterim/`
altına yazılıyor. Küçük ama gerçek bir UX/metin tutarsızlığı. *(G02 sınırı: uyarı
metni ve paylaşım sayfası `41-…` karesinde; klasörün yokluğu ve gerçek yol dosya
yöneticisi karesiyle belgelenmedi, koşum notudur.)*
`41-bildiri-kasadefteri-uyarisi.png`

## Faz 7.5 doğrulama turu (12 Eylül 2026)

Bu tur formu yeniden okudu, 38 kareyi tek tek açtı ve emülatörde
cevaplanabilecek soruları ölçtü. Yapılanlar ve bulunanlar:

**Düzeltilenler (kare ile metin uyuşmuyordu):**

- **Takvim iddiası yanlıştı.** Eski `09-ozgun-ozellik-takvim.png` karesi **Eylül 2026'da ve tamamen
  boştu**, ama metin "her gün hücresinde o günün giriş/çıkışı" diyordu. Kare
  Ağustos 2026'da yeniden çekildi: her gün hücresinde **tek bir net sayı** var
  ve yön **renkle** veriliyor (giriş yeşil, çıkış kırmızı), giriş ve çıkış ayrı
  ayrı gösterilmiyor. `09-ozgun-ozellik-takvim.png` *(G01 sınırı: Ağustos
  karesinde hiçbir günde iki yön birlikte yok; sayının net mi ayrı mı olduğu bu
  kareden doğrulanamaz. Yön renk ve hücre içi konumla — gelir üstte, gider
  altta — veriliyor.)*
- **Drawer 14 değil 17 kalem**: `Reklamları kaldırmak` · `Özet` ·
  `Hesaplar Özet` · `İşlemler-Bütün Hesaplar` · `Hesaplar` · `Aktar` ·
  `Bildiri-Bütün Hesaplar` · `İşlem adları` · `Not Defteri` · `Takvim` ·
  `Nakit Hesap Makinesi` · `Yedekleme ve geri yükleme` · `Ayarlar` ·
  `Silinmiş işlemler` · `Yardım` · `Bizi değerlendirin` · `Önermek`; altında
  ayrıca **`Diğer uygulamalar`** bölümü. `36-drawer-menu-ust.png`,
  `42-drawer-diger-uygulamalar-veresiye-gelirgider.png`
- **`Ayarlar`'ın alt yarısı hiç kaydedilmemişti** (aşağıdaki arayüz taraması
  tablosunda tam liste). `43-ayarlar-alt-bolum-donem-baslangici.png`
- İki kanıt atıfı yanlış kareyi gösteriyordu: K03'ün kanıtı `Is Karti`
  defterine (`06-islem-listesi.png`), K04'ünki `İşlem adları` diyaloğuna (`05-siniflandirma-islem-adlari.png`) bağlıydı; ikisi
  de kendi iddiasını desteklemiyordu. Doğru karelerle değiştirildi.
- Ekran etiketleri ekrandaki hâline çevrildi: `dil` → **`Dilim`**,
  `Ödendi/Alındı` → **`Ödendi Alındı`**, `Gelir/Gider` → **`Gelir Gider`**.
- *"Doğrulanamadı: zorunlu-alan hata mesajının tam metni (sıfır/boş tutarla
  kaydetme denenmedi)"* satırı düştü — o test 11 Eyl'de zaten koşulmuştu,
  12 Eyl'de bir kez daha ölçüldü.

**Emülatörde tamamlananlar (8 yeni kare, E0171–E0178):**

- `37-kontrol-degeri-guncel-47300.png` — **güncel kontrol durumunun karesi hiç
  yoktu.** `İşlemler-Bütün Hesaplar` / `Herşey`: `Toplam Alındı` 53.700 /
  `Toplam Ödendi` 6.400 / **`Denge` 47.300**. Formdaki rakam birebir tuttu.
- `38-ortak-cuzdan-defteri.png` — üç defterden `Ortak Cuzdan`'ın kendi defteri
  çekilmemişti (protokolün "her hesap türünün kendi defteri" kuralı).
- `42-…` ve `43-…` — drawer alt bölümü ve `Ayarlar`'ın alt yarısı.
- `39-gecis-reklami-interstitial.png`, `40-bildiri-defter-basina-pdf-excel.png`, `41-bildiri-kasadefteri-uyarisi.png` — geçiş reklamı, defter başına export diyaloğu, `kasadefteri`
  uyarı metni (bu üçüncüsü formda yazılıydı ama **hiçbir kareye
  dayanmıyordu**).

**Ayar durumu (protokol kuralı 5):**

- `İşlem adları` = `Özel` denemesi (`Tahsilat` / `FaturaOdemesi`) **geri
  alınmış**; 12 Eyl'de canlı doğrulandı, ana ekran yine `Alındı` / `Ödendi`
  diyor. `37-kontrol-degeri-guncel-47300.png`
- `Ayarlar` → `Açıklama / kategori ekle` anahtarı 10 Eyl'de **kapalıydı**
  (`15-ayarlar.png`, 18:19); aynı oturumda bir dakika sonra açık görünüyor
  (`16-kategori-alani-acik-form.png`, 18:20) ve ek koşum 2'de açık kullanılıp
  **açık bırakıldı** *(P1-hesap-defterim-G01 düzeltmesi: önceki metin anahtarın
  ilk kez ek koşum 2'de açıldığını söylüyordu)*. Bilinçli
  sapma: "Ofis malzemesi" test kaydının verisi bu alanda duruyor, anahtarı
  kapatmak o veriyi arayüzden gizler ve alanı açık gösteren karelerle
  (`16-kategori-alani-acik-form.png`, `21-oge-eklemek-tutar-notlar-otomatik.png`,
  `22-fatura-ekle-secenekler.png`, `28b-bos-tutar-sessiz-red.png`) çelişirdi
  *(P1-K: önceki "on kare" sayısı karelerle doğrulanamadı; G02'de alanı
  gösterdiği doğrulanan dört kare yazıldı)*. Durum burada kayıtlı.
- Başka hiçbir ayar değiştirilmedi; `Verileri sil` hiç açılmadı.

**Test verisi:** dokunulmadı. `Ana Hesap` ₺43.150, birleşik net ₺47.300 —
10–11 Eyl'de bırakıldığı gibi.

## Açılış bakiyesi davranışı (dikkat çekici)

Defterin `Açılış bilançosu` alanı **tarihli** (biz Ağu-01-2026 verdik):

| Görünüm | Açılış bakiyesi nasıl görünüyor | `Toplam Alındı`'ya etkisi |
|---|---|---|
| `Günlük` / "Bugün" (dönem Ağustos'u kapsamıyor) | Üstte **`Önceki denge 20.000`** satırı (italik, ayrı). Alt özet de ikiye bölünüyor: normal üçlünün altına `Önceki denge` + `Denge` satırları ekleniyor | ❌ Sayılmıyor |
| `Herşey` / `Aylık`-Ağustos (dönem 1 Ağu'yu kapsıyor) | Normal **`Açılış bilançosu 20.000`** `Alındı` satırı | ✅ `Toplam Alındı`'ya giriyor |

Yani açılış bakiyesi, dönem filtresine göre "taşınan bakiye" veya "gelir
kalemi" gibi davranıyor. `Ayarlar` → `Önceki denge` onay kutusu bu satırın
gösterilip gösterilmeyeceğini kontrol ediyor.
`17-onceki-denge-gunluk-gorunum.png`, `03-dolu-ana-ekran.png`

## Arayüz taraması (görev dışı)

| Alan | Gezildi | Kısa gözlem | Kanıt |
|---|---|---|---|
| Menü (drawer) | ✓ | 17 kalem (yukarıdaki liste) + `Diğer uygulamalar` çapraz tanıtımı | `36-drawer-menu-ust.png`, `42-drawer-diger-uygulamalar-veresiye-gelirgider.png` |
| `İşlem adları` | ✓ | **Buton etiketlerini yeniden adlandırma**: `Ödendi Alındı` ↔ `Gelir Gider` ↔ `Özel` (kendi kelimen). Uygulamanın **jenerik bir giriş/çıkış defteri** olduğunu, "kasa defteri" veya "gelir-gider takibi" olarak çerçevelenebildiğini gösteriyor. Kategori değil, etiket | `05-siniflandirma-islem-adlari.png` |
| `Özet` / `Hesaplar Özet` | ✓ | Aktif defterin gün gün toplandığı özet: `Alındı` / `Ödendi` / **`Tasarruf`** sütunları + dönem gezgini (`<` `>`). Kategori kırılımı yok. **Dönem çipleri ana ekrandan farklı**: burada `Günlük` yok (`Herşey` / `Haftalık` / `Aylık` / `Yıllık`) | `13-ozet-tasarruf-agustos.png` |
| `Bildiri` | ✓ | İki ayrı diyalog: bütün hesaplar (dönem seçimli) ve defter başına (dönemsiz). Ekranda rapor değil, dosya üretir | `34-bildiri-pdf-excel-secim.png`, `40-bildiri-defter-basina-pdf-excel.png` |
| `Takvim` | ✓ | Aktif defterin aylık takvim ızgarası; dolu günlerde tek sayı, yönü renk ve hücre içi konumla; aynı günde iki yön olmadığı için net mi ayrı mı gösterildiği doğrulanmadı *(G01)* | `09-ozgun-ozellik-takvim.png` |
| `Ayarlar` (üst yarı) | ✓ | `Tarihi Biçimlendir` (`Gösteri zamanı` anahtarı) · `Zaman formatı` · `Para birimi biçimi` · **`Her kayıtta tarihi göster`** (23 Ağu 2026 sürümünün yeniliği) · **`Dilim`** · `Karanlık Mod` · `Parmak İzi Şifresi` · `Şifre Ayarları` · `Önceki denge` ✓ · `Her işlemden sonra bakiyeyi göster` ✓ · `Açıklama / kategori ekle` · `Not önerileri` ✓ | `15-ayarlar.png` |
| `Ayarlar` (alt yarı) | ✓ | `Raporlarda zamanı göster` ☐ · **`Yılın ilk gününü ayarlayın`** · **`Ayın ilk gününü ayarlayın: 1`** · **`Haftanın ilk gününü ayarlayın`** · **`Varsayılan süreyi ayarla: Herşey`** · **`Verileri sil`** (kırmızı) · **`İşlem dökümünü e-posta ile otomatik gönder`** ✓ (12 Eylül'de işaretli görüldü; kurulum varsayılanı, alıcı, tetikleyici ve gönderim doğrulanmadı — P1-B11) · `Ekranını açık tut` ☐ · `Gizlilik Politikası` | `43-ayarlar-alt-bolum-donem-baslangici.png` |
| `Açıklama / Kategori` alanı | ✓ | Ayardan açılınca forma **`Açıklama / Kategori`** adlı **ikinci serbest metin alanı** ekleniyor — önceden tanımlı kategori listesi/ikonu yok, sadece ikinci bir etiket kutusu | `16-kategori-alani-acik-form.png` |
| `Silinmiş işlemler` | ✓ | **Çöp kutusu** — silinen işlem burada durur; satıra basılı tut → `Geri Yükle` / kalıcı `Silme` | `08-silinmis-islemler.png`, `32-silinmis-islemler-context-menu.png` |
| `Fatura ekle` | ✓ | İşlem formunda `Fatura ekle` → `Kamera` / `Fotoğraf Galerisi` / `PDF`. Dosya eki, OCR yok | `22-fatura-ekle-secenekler.png` |
| `Öğe eklemek` | ✓ | Formda tek işlem içinde **kalem kalem döküm** dialoğu; `Öğe (N)` + `Toplam` işlem tutarına yazılıyor | `20-oge-eklemek-dialog.png` |
| `Not Defteri`, `Nakit Hesap Makinesi` | ✓ | Serbest not/görev defteri + nakit kupür sayım makinesi | `30-not-defteri-checklist.png`, `31-nakit-hesap-makinesi.png` |
| Boş durum | ✓ | Boş defter: yalnız `Tarih` / `Alındı` / `Ödendi` başlık satırı + sıfır toplamlar. Yönlendirici metin yok | `02-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Formdan "geri" ile çıkış **taslak uyarısı vermiyor**, girilen veri sessiz kayboluyor. Boş tutarla kaydetme sessiz no-op, ₺0 sessiz kabul. Silme tek onaylı (ama soft-delete). Yedekleme kapalıyken hem ana ekran şeridi hem tam ekran uyarı diyaloğu görüldü (sıklık ölçülmedi) | `14-yedekleme-nag-dialog.png`, `28b-bos-tutar-sessiz-red.png`, `28-sifir-tutar-kabul-edildi.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Tek eksen: zaman (gün grupları) → satırlar. Her ekranın altında sabit `Toplam Alındı` / `Toplam Ödendi` / `Denge` üçlüsü. Satır başına isteğe bağlı yürüyen `Denge` |
| Alt/üst gezinme | Alt gezinme çubuğu yok; her şey **hamburger drawer** + üstteki **defter seçici açılır menü** (`Ana Hesap ▼`, içinde arama + `Düzenle` + `Hesap Eklem`). İki büyük eylem butonu altta sabit |
| Renklerin anlamı | `Alındı`/gelir = **yeşil**, `Ödendi`/gider = **kırmızı**, `Denge` = yeşil (pozitif) / kırmızı (negatif). Tutarlı. Üst bar canlı mavi |
| Tipografi ve para | Sayılar büyük ve sağa hizalı; binlik ayraçlı, ondalık gösterilmiyor (tam sayı). Para birimi simgesi ekranda görünmedi |
| Kart/liste/grafik | **Grafik hiç yok.** Yalnız gün-gruplu liste + özet tablosu + takvim ızgarası |
| Form alanları | Tek ekran, ~5 alan; tutar için ayrı hesap-makinesi ikonu; `Notlar`'da sesli giriş (mikrofon); tarih hem `<` `>` oklarıyla hem takvim diyaloğuyla (varsayılan bugün) |
| Loading/boş/hata/başarı | Başarı = küçük toast (`İşlem Eklendi` / `İşlem Silindi`). Boş durum yönlendirici değil. **Hata durumu için hiçbir görsel dil yok**: zorunlu alan boşken ne toast ne inline mesaj çıkıyor, form sessizce açık kalıyor |
| Erişilebilirlik | Dokunma alanları büyük (iki dev buton). Kalıcı reklam banner'ı alt kısmı yer yer kaplıyor; geçiş reklamında `Kapat`ın pasif kalma süresi ölçülmedi (G02). Karanlık mod var. Metin yoğunluğu düşük |

## Sistem işleyişi / pipeline

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Yalnız aktif defterin **yürüyen bakiyesini** günceller (`Alındı` → +, `Ödendi` → −). Ayrı bir rapor ekranında başka kayıt görünmüyor; özet, takvim ve dışa aktarma toplamları listedeki satırlarla tutarlı. Arka planda ikinci bir tablo olup olmadığı ekrandan çıkarılamaz *(P1-B12)* | Manuel gözlem + `Çıkarım` |
| Kart harcaması → kart borcu → kart ödemesi zinciri | **Yok.** Kart = negatif bakiyeli bir defter. "Ödeme" = `Ana Hesap`→kart defteri `Aktar`. Borç, ekstre, kesim/son ödeme tarihi kavramı hiç yok | Manuel gözlem |
| Transfer / kart ödemesi gelir-gider raporundan ayrışıyor mu | **Hayır.** `Aktar` listede **iki ayrı işlem satırı** olarak görünür (saklama yapısı bilinmiyor — P1-B12); her ikisi de o defterin ve birleşik görünümün `Toplam Alındı`/`Toplam Ödendi`sine girer. Yalnız net `Denge` doğru kalır; bir bacak silinir veya düzenlenirse ne olduğu koşum anlatımıdır, görsel kanıtı yok (G01) | Manuel gözlem |
| Fatura/borç → tahsilat/ödeme → kapanış akışı | **Yok.** Fatura nesnesi, cari/müşteri kaydı yok. Bu iş geliştiricinin **ayrı bir uygulamasına** (`Veresiye Defteri`) ayrılmış. Bu üründeki en yakın çözüm: müşteri başına ayrı defter açıp veresiye bakiyesini takip etmek (kişi/rollup yok) | Manuel gözlem |
| Tekrarlayan/planlı kayıt: tanım mı üretir, onay mı bekler | **Tekrarlayan/planlı işlem özelliği yok** (5 yerde tarandı: işlem formu, form 3-nokta, ana ekran 3-nokta, drawer'ın 17 kalemi, `Ayarlar`'ın 21 kalemi). B1 ve B2 bu uygulamada kurulamaz | Manuel gözlem |
| İşletme/şahsi ayrım hangi katmanda | **Hiçbir katmanda.** Ne kapsam, ne kategori, ne mod. En fazla "işletme defteri" / "şahsi defter" diye ayrı defter | Manuel gözlem |
| Dönem tanımı ayarlanabilir mi | **Evet ve bu tek başına dikkat çekici**: `Ayarlar` içinde `Yılın ilk günü`, **`Ayın ilk günü` (varsayılan 1)**, `Haftanın ilk günü` ve `Varsayılan süreyi ayarla` var — yani dönem sınırı kullanıcınındır | Manuel gözlem |
| Ekrandan ekrana tipik yol | `Alındı`/`Ödendi` butonu → tutar → (`Notlar`) → tarih → `Kaydet ve çık` → defterin listesi + `Denge` güncellenir | Manuel gözlem |
| Entegrasyon/dış sistem temas noktaları | **Google Drive** (yedekleme/geri yükleme), **PDF/Excel** dışa aktarma + Android paylaşım sayfası, ve `İşlem dökümünü e-posta ile otomatik gönder` ayarı (12 Eylül'de işaretli; alıcı, tetikleyici ve gerçek gönderim görülmedi — P1-B11). Banka/e-belge/POS/muhasebeci entegrasyonu yok | Manuel gözlem |
| Veri nereye yazılıyor | **Uygulamanın beyanına göre cihazda** — "Kayıtlarınızı sunucularımızda saklamıyoruz"; Drive yedeği kapalıysa cihaz kaybı = veri kaybı. Beyan ağ trafiğiyle denetlenmedi; Drive yedeği ve e-posta ayarı dış temas noktasıdır *(P1-B11 düzeltmesi: önceki metin "tamamen yerel" diyordu)*. Export dosyaları `Documents/Hesap Defterim/` | Resmî kaynak (uygulama içi diyalog) + Manuel gözlem |

**Pipeline şeması (kısa):**
`+ Alındı/Ödendi → tutar + not + tarih → Kaydet → aktif defter listesi + yürüyen Denge → (Özet/Takvim/Bildiri toplamları aynı satırlarla tutarlı; türetme yapısı çıkarım)`

## Akış özeti

- **En kısa ve güçlü akış:** İşlem ekleme — dev renkli buton → tutar → Kaydet
  (~3–4 dokunuş); `Kaydet ve devam Et` ile seri giriş
- **En fazla sürtünme yaratan akış:** Anlamlı gelir/gider raporu istemek —
  transferler ve açılış bakiyesi toplamlara karıştığı için "bu ay ne kazandım"
  sorusunun net cevabı yok
- **Uygulamanın hedef kullanıcı varsayımı:** Kâğıt kasa/veresiye defteri tutan
  esnaf; tek elle giriş, tek bakış bakiye
- **İşletme ve şahsi para yaklaşımı:** **Yok.** Kapsam/kategori/mod
  bulunmuyor; en fazla ayrı defter
- **Transfer ve kart ödemesi yaklaşımı:** `Aktar` var ama **gelir/giderden
  ayrışmıyor** — yalnız net bakiye doğru
- **Planlama, borç ve tahsilat yaklaşımı:** Tekrarlayan/plan/taksit **yok**;
  borç = negatif defter bakiyesi; tahsilat = `Aktar`. Cari işi ayrı uygulamada

## BusinessFinance için kararlar

> `alma` ve `kararı yeniden sor` satırlarında "ne kazandırıyor / ne
> kaybettiriyor" zorunludur (`MANUEL-TEST-PROTOKOLU.md` → yazım kuralı 6).
> Sonuç tanımları: `README.md` → "Karar sonuçları — tek kaynak".
>
> **`kararı yeniden sor` eşiğini geçen bulgu çıkmadı.** En yakın iki aday —
> `Ayın ilk gününü ayarlayın` (dönem sınırının kullanıcıya bırakılması) ve
> `Öğe eklemek` (kalem dökümü) — bir ADR'mizle çatışmıyor, bizde henüz
> olmayan birer özellik; ikisi de `henüz karar verme` / `uyarlayarak al`
> olarak işlendi.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşletme/şahsi ayrımının hiç olmaması | Alma | **Kazandırdığı:** kavram sayısı minimumda kalıyor; form beş alan, giriş 3–4 dokunuş, esnaf günlük kaydı akşam üstü hızla yazabiliyor. **Kaybettirdiği:** şahıs şirketi sahibi aynı defterde iki tür parayı ayıramıyor ve tek dolaylı yol "ikinci bir defter açmak" — o da bakiyeyi bölüyor, raporu bölmüyor. ADR 0013 ters ödünleşimi seçiyor: bir boyut fazla, ayrım mümkün | — |
| Kategori yok; açılabilen `Açıklama / Kategori` yalnız ikinci serbest metin | Alma | **Kazandırdığı:** liste ve arama tek bir metin alanı üstünden çalışıyor, kullanıcı kategori kurmak zorunda kalmıyor, "yanlış kategoriye attım" sorunu hiç doğmuyor. **Kaybettirdiği:** serbest metin toplanamıyor — "bu ay yakıta ne verdim" sorusu yazım tutarlılığına kalıyor ve bütçe ekseni kurulamıyor. Geliştirici de bunu kabul etmiş görünüyor: kategorili takip ayrı bir uygulamaya (`Gelir Gider`) ayrılmış | Kategori seçimi |
| `Aktar`ın gelir/gider toplamlarına karışması (yalnız net `Denge` doğru) | Alma | **Kazandırdığı:** transfer için ayrı bir kayıt türü, ayrı ekran ve ayrı rapor kuralı öğretmeye gerek kalmıyor; her hareket aynı listede, aynı biçimde. **Kaybettirdiği:** `Toplam Alındı`/`Toplam Ödendi` ciro gibi okunamıyor; para gezdirdikçe iki toplam da şişiyor ve kullanıcı bunu ancak kendisi çıkarabiliyor | Aylık rapor, birleşik feed |
| Kart = sadece negatif bakiyeli defter; borç/ekstre/kesim yok | Alma | **Kazandırdığı:** kart da bir defter olduğu için ayrı bir model, ayrı ekran ve ekstre kavramı hiç öğrenilmiyor; kısmi ödeme kendiliğinden serbest. **Kaybettirdiği:** "şu an ödemem gereken" ile "henüz kesilmemiş" ayrılmıyor, son ödeme günü hatırlatılmıyor ve limit görünmüyor — kartla dönen esnaf için en kritik iki sayı eksik | Kart ekranı, ekstre |
| **Tekrarlayan (B1) ve taksit (B2) özelliğinin hiç olmaması** | Alma | **Kazandırdığı:** "plan mı, gerçekleşme mi" ayrımı hiç yok; defterde yalnız olmuş şeyler var, bakiye her zaman gerçeğin kendisi, sürpriz üretmiyor. **Kaybettirdiği:** her ay tekrarlayan kirayı ve taksiti kullanıcı eliyle giriyor; unutulan ay sessizce eksik kalıyor ve gelecek yükümlülük hiçbir yerde görünmüyor | Planlama, tekrarlayan, taksit |
| **`Aktar` listede iki ayrı satır olarak görünüyor; koşum notuna göre bir bacak silinince diğeri kalıyor** (görsel kanıtı yok — G01, P1-B12) | Alma | **Kazandırdığı:** koşum notu doğruysa her satır tek başına düzenlenebiliyor — kullanıcı transferin bir ucunu başka deftere taşıyabiliyor, tarihini ayrı verebiliyor, kilit yok. **Kaybettirdiği:** koşum notu doğruysa tek bacak silinince net varlık **uyarısız** yanlışa döner ve kullanıcıyı uyaran bir işaret yoktur; saklama yapısı ve bütünlük kuralı ekrandan doğrulanmadı. Bizde `Transfer` tek atomik kayıt: esneklik gidiyor, sessiz bozulma da gidiyor | Transfer modeli |
| Kısmi kart ödemesi serbest (jenerik `Aktar`, override edilecek kavram yok) | Doğrudan al (uyumlu) | Bizim kısmi ödeme/tahsilat (D3) ile aynı yön; bizde ayrıca "hangi ekstreye" mantığı var | Kart ödemesi |
| `Fatura ekle` = Kamera / Galeri / PDF eki, OCR yok | Not | **Kazandırdığı:** yanlış okuma riski sıfır, kullanıcı ne eklediğini biliyor. **Kaybettirdiği:** tutar/tarih/satıcı yine elle giriliyor; ADR 0011'in öneri katmanının verdiği hız yok | — |
| Ücretsiz sürümde kalıcı banner + navigasyonda tam ekran geçiş reklamı | Alma | **Kazandırdığı:** ürün gerçekten ücretsiz kalıyor; 10 Mn+ indirmelik erişim ve kayıt istememe lüksü bunu finanse ediyor. **Kaybettirdiği:** para kaydı tutarken ekranın kesilmesi güven duygusunu zedeliyor; geçiş reklamı hızlı giriş akışını kesiyor (`Kapat`ın pasif kalma süresi ölçülmedi — G02) | — |
| **`Önceki denge` satırı** — dönem başında taşınan bakiyeyi ayrı, italik bir satırda + alt özette ayrı bir satırda göstermek | Uyarlayarak al | **Kazandırdığı:** bir döneme girerken "nereden başladık" sorusu listenin ilk satırında cevaplanıyor; devir bakiyesi görünür olunca dönem toplamı da anlaşılır hâle geliyor. **Kaybettirdiği:** satır dönem içine düşerse dönem toplamına karışıyor ve toplam yanlış okunuyor — uyarlarken bu satırın toplam dışında tutulması gerekir | Hesap detayı / dönemli feed |
| **Satır başına yürüyen `Denge`** (her işlemden sonra bakiye, ayardan kapatılabilir) | Uyarlayarak al | Hesap detay defterinde her satırın yanında o andaki bakiyeyi göstermek klasik ve faydalı; bizde bakiye zaten hesaplanıyor | Hesap detayı |
| **Soft-delete + `Silinmiş işlemler` çöp kutusu + `Geri Yükle` + ayrı onaylı kalıcı silme** | Doğrudan al (ruhen) | "Sil yerine koru" ilkemizle aynı yön; üstüne kalıcı silmeye ikinci bir onay ekliyor. Biz idempotent iptali tercih ediyoruz, geri alınabilirlik aynı hedefte | İşlem iptal/düzeltme |
| `Kaydet ve devam Et` (seri giriş) | Uyarlayarak al | Money Manager bulgusuyla aynı — hızlı ardışık giriş; bizde ek alanlar var | İşlem ekle formu |
| **`Öğe eklemek` — kalem dökümünün tutarı VE notu otomatik doldurması** | Uyarlayarak al | Kalem kalem döküm girilince toplamın tutar alanına, dökümün açıklamaya otomatik yazılması gerçek bir kolaylık; bizde `Öğe` kavramı yok ama fiş/fatura satırı kırılımı ileride düşünülebilir | İşlem ekle formu (gelecek) |
| **Boş tutarla kaydetmede sessiz no-op (hiçbir geri bildirim yok)** | Alma | **Kazandırdığı:** form hiç kırmızıya boyanmıyor, hata dili hiç kurulmuyor, hızlı girişte görsel gürültü sıfır. **Kaybettirdiği:** kullanıcı neden kaydedilmediğini anlamıyor; butona birkaç kez basıp uygulamayı donmuş sanabiliyor. Bizde zorunlu alan hatası açık mesajla ve alanın yanında gösterilmeli | Form doğrulama |
| **Sıfır (₺0) tutarın uyarısız kabul edilmesi** | Alma | **Kazandırdığı:** hiçbir tutar reddedilmiyor; "şimdilik 0 yazayım sonra düzeltirim" gibi kullanımlara kapı açık. **Kaybettirdiği:** başlıksız, sıfır tutarlı bir satır sessizce geçmişe giriyor ve raporu kirletiyor. BusinessFinance'te normal para hareketlerinde sıfır tutar istemcide ve sunucuda zaten reddediliyor. *(P1-B02-B04 düzeltmesi, 14 Eyl 2026: önceki gerekçe "bizde ya reddedilmeli ya açık onay istenmeli" diyordu.)* | Form doğrulama |
| **Formdan çıkışta taslak uyarısı olmaması** | Alma | **Kazandırdığı:** yanlış açılan form tek dokunuşla kapanıyor, "kaydetmeden çıkmak istediğine emin misin?" sorusu hiç sorulmuyor. **Kaybettirdiği:** yazılmış tutar ve not uyarısız gidiyor; kalem dökümü gibi uzun bir girişte bu gerçek bir kayıp | Form doğrulama |
| **Arama sonucuna göre alt toplamların yeniden hesaplanması** | Doğrudan al (ruhen) | Filtrelenmiş görünümde toplamların da filtreye uyması okunabilirliği artırıyor; bizde aktivite feed'i filtrelerken aynı ilke uygulanabilir | Aktivite feed / arama |
| **Export diyaloğunun anlattığı klasörle gerçek kayıt yerinin tutarsız olması** (`kasadefteri` yok, dosya `Documents/Hesap Defterim/`) | Alma | **Kazandırdığı:** hiçbir şey — bu bir bakım borcu, muhtemelen eski sürümden kalma metin. **Kaybettirdiği:** kullanıcı dosyayı anlatılan yerde arayıp bulamıyor ve "export çalışmadı" sonucuna varıyor. Bizim export/yedek akışlarımızda gösterilen yol her zaman gerçek yolla birebir tutmalı | Dışa aktarma akışı |
| **`Ayarlar` → `Ayın ilk gününü ayarlayın` (dönem sınırı kullanıcının)** | Henüz karar verme | **Kazandırdığı:** ayın 15'inde kapanış yapan esnafın raporu kendi dönemine oturuyor; Wallet'ta da aynı ayar var (`Initial day of the month`), yani tekil bir tercih değil. **Kaybettirdiği:** "Ağustos raporu" artık herkes için aynı şey olmuyor; dışa aktarılan dosya ve müşavire giden sayı, ayarı bilmeyen için yanıltıcı. Bizde ay sabit 1'de başlıyor; ihtiyacın gerçekliği ölçülmeden değiştirilmemeli | Aylık rapor dönemi |
| **`İşlem dökümünü e-posta ile otomatik gönder` ayarı** (12 Eylül'de işaretli görüldü) | Alma | **Kazandırdığı:** ayar çalışıyorsa yerel-tek-kopya riskine karşı ikinci bir kanal olur ve kullanıcı dökümünü ek işlem yapmadan alabilir. **Kaybettirdiği:** ayar gerçekten kurulumda açık geliyor ve gönderim yapıyorsa finansal döküm kullanıcı açıkça istemeden dışarı çıkabilir. Kurulum varsayılanı, alıcı, tetikleyici ve gerçek gönderim doğrulanmadı; bu satır yalnız görünen ayara dayanır. İlke olarak böyle bir kanal bizde kapalı doğmalı ve kullanıcı açmalı *(P1-B11 düzeltmesi: önceki metin varsayılan açıklığı ve istemsiz gönderimi kesin yazıyordu)* | Yedekleme / dışa aktarma |
| `Not Defteri` — basit görev/hatırlatıcı listesi | Not | Muhasebe kaydıyla bağı yok; bizim planlama/hatırlatıcı modelimizle karıştırılmamalı | — |
| `Nakit Hesap Makinesi` — kupür bazlı kasa sayım yardımcısı | Not | Esnafın fiziksel kasayı sayarken toplama yapmasını kolaylaştıran araç; muhasebe kaydı üretmiyor. BusinessFinance'te fiziksel kasa sayımı ve fark akışı zaten var (`Kasa > Gün sonu`: beklenen bakiye ile sayılan tutar yan yana, fazla/eksik, açık onayla tek düzeltme kaydı); karşılığı olmayan yalnız kupür × adet yardımcısıdır, sayılan tutar tek serbest alana girilir. Öncelik bu kıyasla belirlenmedi; yardımcının değeri Belge 3'te ayrıca tartılır. *(P1-B14 düzeltmesi, 14 Eyl 2026: önceki gerekçe "dijital-öncelikli modelimizde karşılığı yok, düşük öncelik" diyordu.)* | Kasa > Gün sonu sayım formu |
| İki dev renkli giriş/çıkış butonu (yeşil `Alındı` / kırmızı `Ödendi`) | Uyarlayarak al | Birincil eylemi tartışmasız kılıyor; bizde daha çok kayıt türü var ama gelir/gider ayrımını renkle güçlü vermek doğru (ADR 0008 iki-ton) | İşlem ekle launcher |
| `İşlem adları` — buton etiketlerini kullanıcıya yeniden adlandırtma (PDF başlıklarına kadar) | Alma | **Kazandırdığı:** tek ürün hem "kasa defteri", hem "gelir-gider", hem kullanıcının kendi kelimeleriyle çalışıyor; çıktı da aynı dili konuşuyor. **Kaybettirdiği:** destek, paylaşılan çıktı ve eğitim materyali ortak bir kelime dağarcığı kuramıyor; iki kullanıcının ekranı aynı şeyi farklı adlandırıyor. Bizde alan semantiği sabit (gelir/gider/transfer/kart ödemesi/cari) — yeniden adlandırma o anlamı bulanıklaştırır | — |
| Çok-defter modeli (her defter bağımsız bakiye, liste bakiye bile göstermiyor) | Alma | **Kazandırdığı:** "hesap türü" diye bir kavram öğrenmeye gerek yok; kullanıcı defteri istediği eksende (kişi, şube, kart, amaç) açıyor. **Kaybettirdiği:** defterler arası tek ortak görünüm `İşlemler-Bütün Hesaplar`; `Hesaplar` listesi bakiye bile göstermediği için "nerede ne kadar var" sorusu defter defter gezmeyi gerektiriyor | — |
| Yerleşik PDF/Excel çıktı (`Bildiri`), iki ayrı diyalog | Not | Bizim kendi dışa aktarma/yedek tasarımımız var; ayrı incelenir | — |
| Agresif yedekleme uyarısı + yalnız-yerel veri | Alma | **Kazandırdığı:** uyarı doğru bir riski anlatıyor ("sunucuda saklamıyoruz") ve kullanıcıyı yedeğe itiyor; gizlilik açısından yerel-önce bir beyan (ağ trafiği denetlenmedi; e-posta ayarı ve Drive dış temas noktası — P1-B11). **Kaybettirdiği:** uyarı şerit ve diyalog olarak birden çok yüzeyde çıkıyor (sıklık karelerle ölçülmedi); sık tekrarlanırsa kullanıcı onu okumadan kapatmayı öğrenir ve uyarı işlevini kaybeder *(G01)*. Bizde backup/restore bir sürüm kapısı; aynı kaygıyı **nag yerine güven veren** bir akışla çözmeliyiz | Yedekleme akışı |
| **Cari/veresiye ve kategorili takibin ayrı uygulamalara bölünmüş olması** | Alma | **Kazandırdığı:** her uygulama tek bir işi yapıyor, öğrenme eğrisi düz kalıyor, kullanıcı ihtiyacı olmayanı hiç görmüyor. **Kaybettirdiği:** aynı esnafın kasası, veresiyesi ve kategorili gideri üç ayrı veri kümesine dağılıyor; hiçbir yerde tek bir "işletmem nerede" sayısı üretilemiyor ve üç yedek, üç geri yükleme doğuyor. Bizim ADR 0013 kararımız tersini seçti — bedeli tek üründe daha çok kavram | — |

## Kanıt ve güven düzeyi

- **Manuel gözlem (10 Eyl 2026):** K00–K08 + arayüz taraması + A/B/B1/B2 ek
  koşumu, Ağustos 2026 sentetik veriyle, 3 defter + 5 çekirdek işlem (transfer
  + kart ödemesi dâhil). Kontrol değerleri (bakiye/net) birebir tuttu. Bu
  koşumun A/B1/B2 test kayıtları o gün silinmişti (o sırada kural henüz
  yazılmamıştı). Kareler E0134–E0154
- **Manuel gözlem (11 Eyl 2026, ek koşum 2):** `Öğe eklemek`, `İşlem adları`
  (`Özel`), zorunlu alan/sıfır tutar, arama, kalıcı silme, `Bildiri` (PDF
  gerçek dosya doğrulandı), `Not Defteri`, `Nakit Hesap Makinesi`. Ek sentetik
  kayıtlar (₺150 gider + ₺2.500 gelir) MM/Wallet/Bluecoins konvansiyonuna
  uyacak şekilde **cihazda bırakıldı**. Kareler
  `20-oge-eklemek-dialog.png` – `34-bildiri-pdf-excel-secim.png`, artı ara
  doğrulama karesi `35-kontrol-degeri-geri-yuklendi.png`
- **Manuel gözlem (12 Eyl 2026, Faz 7.5 doğrulama turu):** 38 kare tek tek
  açıldı ve metinle karşılaştırıldı; güncel kontrol değeri, `Ortak Cuzdan`
  defteri, Ağustos takvimi, drawer'ın tamamı, `Ayarlar`'ın alt yarısı,
  `kasadefteri` uyarı metni ve gerçek dosya yolu, defter başına export
  diyaloğu, geçiş reklamı ölçüldü; boş tutar ve taslak uyarısı davranışı
  yeniden doğrulandı. Kareler E0171–E0178, `09-ozgun-ozellik-takvim.png` yeniden çekildi
- **Resmî kaynak (mağaza sayfası):** Play listesi — 4,8★ / 139 B yorum /
  10 Mn+ indirme / geliştirici ANKIT SARAF / son güncelleme 23 Ağu 2026 /
  sürüm notu "Her kayıtta tarihi gösterme seçeneği eklendi". `00-magaza.png` —
  karede yalnız geliştirici, güncelleme tarihi ve sürüm notu görünüyor; puan,
  yorum ve indirme değerlerinin kare kanıtı yok *(P1-hesap-defterim-G01)*
- **Resmî kaynak (uygulama içi diyalog):** "Kayıtlarınızı sunucularımızda
  saklamıyoruz, bu nedenle yedekleme kapalıysa verilerinizi geri
  yükleyemeyiz." `14-yedekleme-nag-dialog.png`
- **Çıkarım:** "Anlamlı gelir/gider toplamı yok" — kontrol değerlerinde
  `Toplam Alındı`/`Toplam Ödendi`nin açılış bakiyesi + transferleri
  içermesinden türetildi (`06b-islemler-butun-hesaplar.png`); uygulama böyle
  bir toplam sunduğunu hiçbir yerde iddia etmiyor
- **Çıkarım:** cari ve kategorili takibin ayrı uygulamalara bölünmüş olması,
  drawer'daki iki tanıtım satırından (`42-…`) okunuyor; o uygulamalar
  kurulmadı ve içleri görülmedi
- **B1/B2 negatifi doğrulandı:** tekrarlayan ve taksit yokluğu 5 ayrı yerde
  tarandı (işlem formu, form 3-nokta, ana ekran 3-nokta, drawer'ın 17 kalemi,
  `Ayarlar`'ın 21 kalemi) — hiçbirinde plan/tekrar/hatırlatıcı yok
- **Doğrulanamadı:** `Yedekleme ve geri yükleme` gerçek Drive akışı (Google
  hesabı bağlanmadı); `Verileri sil` (geri alınamaz, kasten açılmadı);
  `Yılın / Haftanın ilk gününü ayarlayın` diyaloglarının içeriği
- **Kapsam dışı (protokol):** Excel çıktısının içeriği (PDF doğrulandı)

## Tek cümlelik sonuç

Hesap Defterim, esnafın kâğıt kasa defterini birebir dijitalleştiren, etiketleri
kullanıcının kendi kelimeleriyle yeniden adlandırılabilen tek sütunlu
yürüyen-bakiye defteri: tek kavramla ("girdi / çıktı / kalan") çalıştığı için
giriş hızı ve öğrenilebilirliği yüksek, buna karşılık gelir/gider toplamı
transferlerle karışıyor, kart borcu ve plan kavramı hiç yok; kategorili takibi
ve cariyi aynı geliştirici **ayrı uygulamalara** ayırmış — yani bizim tek
üründe birleştirmeye çalıştığımız işleri, o bilinçli olarak bölmüş.


## 15 Eylül kullanıcı kontrolü — HD-U01 (B11)

Kullanıcı ayarlarda e-posta adresi bulamadı. Eski kanıt 43-ayarlar-alt-bolum-donem-baslangici.png yeniden incelendi: Verileri sil ile Ekranını açık tut arasında otomatik e-posta kutusu var, adres alanı yok. Alıcının konumu bilinmiyor; gönderim yapılmadı. B11 sınırı korunur.


15 Eylül takip sonucu (kullanıcı beyanı, yeni görsel yok): Otomatik e-posta kutusu açılıp kapanıyor; dokununca pencere veya e-posta seçimi açılmıyor. Kullanıcının ilk girişte alınmış adrese gönderiliyor olabileceği düşüncesi hipotezdir. Önceki koşum notunda uygulamanın giriş/e-posta istemediği yazılıdır; ilk girişte adres alındığı doğrulanmış değildir. Alıcı, tetikleyici ve gerçek teslim bilinmiyor; gönderim var/yok sonucu çıkarılmaz. HD-U01 görünür ayar kontrolü tamamlandı, ek adres araması gerekmiyor; B11 kapsam sınırı korunur.

## 23 Eylül 2026 — ek eksik koşumu (22 Eylül) ve kare doğrulaması

Koşum listesi `raporlar/eksik-kosum-ortak-listesi.md` (HD-01…HD-05). Defter verisi
değişmedi; yalnız dışa aktarma dosyası üretildi.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Dışa aktarılan PDF'in içeriği (HD-01) | Bildiri › PDF iki dosya üretip paylaşım sayfası açıyor. PDF: başlıkta defter adı (Ana Hesap), altında dönem; kolonlar Tarih · Notlar · Açıklama/Kategori · Gelir · Gider · Denge; üstte Önceki denge. **Açılış bilançosu 20.000 Gelir sütununda**; aktarım bacakları "Kime Ortak Cuzdan" 3.000 ve "Kime Is Karti" 1.200 **Gider satırı**; özet Toplam Gelir 47.500 · Toplam Gider 4.350 · Denge 43.150. Ekrandaki toplam tanımı dosyaya da taşınıyor | `49-uretilen-pdf-icerigi.png` (E0427) |
| kasadefteri klasörü (HD-02) | Uyarı: "Veriler, SD kartta veya Dahili Depolamada kasadefteri adlı bir klasöre kaydedilir." Koşumun dosya sistemi araması (adb, karesiz): `/sdcard` altında bu adla **klasör yok**; PDF'ler `Documents/Hesap Defterim/` altına, Excel uygulamanın kendi `Android/data/…/files/Documents/Hesap Defterim/` dizinine yazılmış. Uyarı oluşmayan bir klasörün adını veriyor | `48-kasadefteri-klasoru-uyarisi.png` (E0447), E0176; dosya sistemi araması koşum kaydı |
| Haftalık ve Yıllık çipleri (HD-05) | Haftalık Eyl-21 → Eyl-27: hareket yok, Önceki denge 43.150, Denge 43.150. Yıllık Oca-01 → Ara-31: Toplam Gider 4.350, Önceki denge 0, Denge 43.150. Dönem yalnız kendi aralığının hareketlerini topluyor, devreden dengeyi ayrı satırda taşıyor | `45-haftalik-cipi-onceki-denge-43150.png` (E0458), E0447 (Yıllık) |

## 24 Eylül 2026 — Belge 1 düzeltme turu: kare doğrulaması

Belge 1'e dayanak yapılmadan önce kareler tek tek açıldı. Aşağıdakiler ya ortak listedeki koşum
özetini kareye göre düzeltir ya da Belge 1'in kullanmadığı bir karede görülen arayüz ayrıntısıdır.
Yeni koşum yapılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Açılış defteri (HD-03) | Ana Hesap defteri ekranı. Soğuk açılış ve geri tuşunun listeye dönmemesi karede değil, koşum kaydında | E0496 |
| Ayarlar | Para birimi biçimi · Önceki denge (açık) · Her işlemden sonra bakiyeyi göster (açık) · Açıklama / kategori ekle · Not önerileri · Parmak İzi Şifresi | E0150 |
