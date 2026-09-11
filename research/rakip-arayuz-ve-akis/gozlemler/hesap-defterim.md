# Uygulama Gözlem Formu — Hesap Defterim (Cash Book)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Hesap Defterim (Cash Book) / **ANKIT SARAF** |
| Sürüm | `versionName=235` (`versionCode=235`), targetSdk 36; Play'de "Son güncelleme 23 Ağu 2026" |
| Test tarihi | **10 Eylül 2026** (Tur 1, K00–K08 + arayüz taraması, yapay zekâ sürdü) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400, Android 17) |
| Dil / para birimi | **Türkçe arayüz** (sistem dili) / para birimi simgesiz sayı (₺/simge gösterilmedi; "Para birimi biçimi" ayarı var) |
| Hesap veya plan türü | Ücretsiz — "Reklam içerir • Uygulama içi satın alma" (reklam kaldırma IAP). Ekranda AdMob banner (Test Ad) |
| Erişim kısıtı | **Yok** — kayıt/giriş/e-posta/telefon/VKN **hiç istemiyor**, tamamen yerel. Açılışta yalnız "nasıl kullanılır" bilgi diyaloğu + Google Drive yedekleme daveti |
| İnceleme türü | Manuel gözlem |
| Mağaza verisi | **4,8★ · 139 B yorum · 10 Mn+ indirme · PEGI 3.** Etiketler: İş, Giderler, Muhasebe. Tez: "Günlük giderler akışını, günlük nakit dengesini yönetin." |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Tek sütunlu **yürüyen bakiye defteri**: para girdi (Alındı) / çıktı (Ödendi) → Denge. "Khatabook" türünün Türkçe'ye yerelleşmiş hâli. |
| Asıl hedef kullanıcı | Esnaf / küçük dükkân sahibi; elle kasa/veresiye defteri tutan kişi. Kişisel bütçeci de kullanabilir (arayüz yeniden adlandırılabiliyor). |
| Çözdüğü ana iş | Günlük nakit hareketini ve o anki bakiyeyi kâğıt deftere yazmak yerine telefonda tutmak; PDF/Excel çıktı ve Drive yedeği almak. |
| Açıkça kapsam dışı bıraktığı | Kategori sistemi, hesap türleri (nakit/banka/kart), işletme/şahsi ayrımı, fatura nesnesi, cari/müşteri kaydı (kişi olarak), banka bağlantısı, vergi, tekrarlayan/planlı işlem, bütçe. |
| İş modeli | Ücretsiz + reklam + tek seferlik "reklamları kaldır" IAP. |
| BusinessFinance ile aynı kulvarda mı | **Hayır** — bizde çok hesaplı havuz + kapsam boyutu + kart borcu/ekstre + planlama var; burada bunların hiçbiri yok. Ortak nokta yalnız "elle giriş, nakit esaslı" olması. |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 0 | Kayıt/giriş **hiç yok**; ilk açılışta "Uygulama nasıl kullanılır?" bilgi diyaloğu (TAMAM MI) + hazır "Hesap Defterim" defteri, anında kullanılabilir | Açılıştan itibaren **agresif Google Drive yedekleme daveti**; veriler yalnız yerelde ("Kayıtlarınızı sunucularımızda saklamıyoruz") | `01-ilk-acilis-hosgeldin.png` |
| K01 Ana ekran | Tamamlandı | 0 | İki dev renkli buton: **Alındı** (yeşil) / **Ödendi** (kırmızı); üstte Herşey/Günlük/Haftalık/Aylık/Yıllık; altta Toplam Alındı / Toplam Ödendi / **Denge** | Alt kısımda kalıcı AdMob banner (Test Ad); üst bölge yedekleme + reklam kaldırma davetleriyle dolu | `02-bos-ana-ekran.png`, `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı (uyarlanarak) | ~6/defter | **Nakit/banka/kart kavramı yok.** "Hesap" = ayrı bir **defter**, her biri kendi yürüyen bakiyesiyle. `HESAP EKLEM` → İsim + Açılış bilançosu (isteğe bağlı, +/− radyo, tarihli) | Defter ≠ hesap türü; para birimi/tür sorulmuyor. Açılış bakiyesi tarihi ayrı bir davranış üretiyor (aşağıda) | `10-hesap-eklem-formu.png`, `11-hesaplar-defterler-listesi.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~5 | Form tek ekran: tür (Alındı/Ödendi) + tarih + saat + tutar + Notlar (+ isteğe bağlı Açıklama/Kategori) + Fatura ekle + Öğe eklemek. "Kaydet ve çık" / **"Kaydet ve devam Et"** (seri giriş) | **Kategori yok, müşteri alanı yok** → "Ada Reklam" yalnız Notlar'a yazıldı. Liste satırında başlık = Notlar metni | `04-islem-formu-alindi.png`, `06-islem-listesi.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~5 | Ödendi butonu → aynı form, kırmızı; Ortak Cüzdan defterine −850 yazıldı, Denge 1.150 | **İşletme/şahsi (kapsam) ayrımı hiç yok.** Protokol gereği kategoriyle taklit edilmedi → bu boyut `Desteklenmiyor` | `05-siniflandirma-islem-adlari.png` |
| K05 İşletme kart gideri | Tamamlandı (kart modeli yok) | ~5 | "Is Karti" defterine −1.200 Ödendi; defter bakiyesi **−1.200'e düştü** | **Kart borcu / ekstre / kesim tarihi kavramı yok.** Kart = sadece eksiye giden bir defter. Kredi limiti, "ödenecek vs kesilmemiş" ayrımı yok | `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı (ayrışmıyor) | ~6 | Menü → **Aktar**: Miktar + Kimden (defter) + Kime (defter) + tarih + not. Kaynak −, hedef + | Transfer **iki ayrı defter satırı** üretiyor ("Kime X" / "Kimden: Y") ve her ikisi de o defterin **Toplam Alındı/Ödendi'sine giriyor** — gelir/giderden ayrışmıyor | `12-aktar-transfer-formu.png`, `06b-islemler-butun-hesaplar.png` |
| K07 Liste ve rapor | Tamamlandı (rapor zayıf) | ~3 | **İşlemler-Bütün Hesaplar** (birleşik defter) Denge **44.950** = kontrol net varlık ✓. **Özet/Hesaplar Özet** = gün gün Alındı/Ödendi/**Tasarruf** (net). **Bildiri** = PDF/Excel çıktı | Kategori kırılımı, işletme/şahsi kırılımı, grafik **yok**. Toplam Alındı 51.200 / Ödendi 6.250 = açılış bakiyesi + transferler + gerçek gelir/gider **karışık** | `07-rapor-aylik-butun-hesaplar.png`, `13-ozet-tasarruf-agustos.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~2–3 | Satıra dokun → **İşlemi Düzenle** (satır içi tam düzenleme + "Hesaplar" alanıyla **başka deftere taşıma** + kopyala ikonu). Sil → tek onay → **"Silinmiş işlemler"** çöp kutusu → **Geri Yükle** | Toast'ta anlık "geri al" yok; çöp kutusundan restore gerekiyor. Formdan "geri" ile çıkınca **taslak uyarısı yok** | `08-silinmis-islemler.png` |

## Kontrol değeri doğrulaması

5 çekirdek işlem (kart ödemesi dâhil), Ağustos 2026 tarihleriyle, 3 defter
(Ana Hesap +20.000 / Ortak Cuzdan +2.000 / Is Karti 0 açılış). Kaynak:
`03-dolu-ana-ekran.png`, `06b-islemler-butun-hesaplar.png`.

| Değer | `SENTETIK-TEST-VERISI` beklenen | Hesap Defterim | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi (Denge) | 40.800,00 | 40.800 | ✓ |
| Ortak Cüzdan bakiyesi (Denge) | 4.150,00 | 4.150 (2.000 + 3.000 transfer − 850) | ✓ |
| Is Karti bakiyesi (kart ödemesinden sonra) | 0,00 | 0 (−1.200 + 1.200) | ✓ |
| Net varlık (birleşik Denge) | 44.950,00 | **44.950** (İşlemler-Bütün Hesaplar) | ✓ |
| Ağustos "gelir" toplamı | 25.000,00 | **yok** — Toplam Alındı 51.200 (açılış 22.000 + gelir 25.000 + transfer bacakları 4.200) | ✗ kirli |
| Ağustos "gider" toplamı | 2.050,00 | **yok** — Toplam Ödendi 6.250 (gider 2.050 + transfer bacakları 4.200) | ✗ kirli |

**Sonuç:** Bakiye ve net varlık modeli doğru (Denge her yerde tutuyor). Ama
uygulamada **anlamlı bir gelir/gider toplamı yok**: "Toplam Alındı/Ödendi"
açılış bakiyesini ve transferleri de içine katıyor. Rakibin finansal modeli
"tek yürüyen bakiye" olduğu için bu bir hata değil, **tasarım sınırı** — ama
BusinessFinance'in ayrı gelir/gider raporu ihtiyacının neden var olduğunu
gösteriyor.

## Ek koşum: A / B / B1 / B2 (kredi kartı kısmi ödeme, fiş OCR, tekrarlayan, taksit)

Sonradan standarda eklenen 4 test, `SENTETIK-TEST-VERISI.md` tutarlarıyla
koşuldu (10 Eyl 2026). Kanıt: `18-kismi-odeme-is-karti.png`,
`19-transfer-bacagi-desync.png`.

| Test | Beklenen | Hesap Defterim | Sonuç |
|---|---|---|---|
| **A — Kredi kartı kısmi ödemesi** | Kart ödemesinde kısmi tutar girilebiliyor mu | **Evet, doğal olarak.** Test: Is Karti'ye −₺1.000 "Test taksit ekipman" borç, sonra **Aktar ₺400** (Ana Hesap→Is Karti) → Is Karti −1.000'den **−600'e**. Kart ödemesi jenerik bir Aktar olduğu için tutar zaten serbest; "ekstre öde" / "asgari tutar" / ödeme vadesi gibi override edilecek bir kavram **yok** | `Tamamlandı` |
| **B — Fiş / kamera** | OCR mu, sadece fotoğraf mı | **Sadece dosya eki.** "Fatura ekle" → **Kamera / Fotoğraf Galerisi / PDF**. Otomatik okuma/ayrıştırma yok. ADR 0011 öneri katmanının karşılığı yok | `Tamamlandı` |
| **B1 — Tekrarlayan gider (₺600/ay, ilk çekim 10 Ağu, aylık)** | Tanım nasıl kuruluyor; otomatik mi onaylı mı düşüyor | **Özellik YOK.** İşlem formunda, form 3-nokta menüsünde (`Açıklama/kategori ekle` tek kalem), ana ekran 3-nokta menüsünde, drawer'ın 14 kaleminde ve Ayarlar'da hiçbir **tekrarla / planlı / hatırlatıcı / abonelik** seçeneği yok. Kullanıcı her ay eliyle girer | `Desteklenmiyor` |
| **B2 — Taksitli kart harcaması (₺6.000 = 6×₺1.000, ilk taksit 15 Ağu)** | Taksit planı nasıl kuruluyor; ekstreye nasıl bölünüyor | **Özellik YOK.** Taksit planı kavramı yok. Kullanıcı ya tek ₺6.000 Ödendi kaydı, ya da elle 6 ayrı ₺1.000 kaydı açar. "6 taksit" bilgisi hiçbir yere taşınmaz | `Desteklenmiyor` |

**Ek bütünlük bulgusu — transferin öksüz bacağı:** "Aktar" tek bir çift-kayıt
nesnesi değil, **iki bağımsız işlem satırı** olarak saklanıyor ("Kime X" /
"Kimden: Y"). A testinin temizliğinde Is Karti tarafındaki ₺400 bacağını sildim;
Ana Hesap tarafındaki −₺400 bacağı **silinmeden kaldı** → net varlık sessizce
yanlışa döndü (40.800 yerine 40.400). Aynı şekilde bir bacağın tutarı/tarihi
düzenlenince diğeri güncellenmiyor. `19-transfer-bacagi-desync.png`. Çift kayıt
bütünlüğü yok — BusinessFinance `Transfer` tek atomik kayıt olduğu için burada
ayrışamaz.

**Ek gözlem — geçiş reklamı:** Formdan çıkarken ücretsiz sürümde **tam ekran
geçiş reklamı** (interstitial) açıldı. Kalıcı banner'a ek olarak navigasyonda
kesintili reklam var.

**Ek gözlem — "Öğe eklemek":** Tek işlem içinde kalem dökümü (Öğe adı / Miktar /
Birim / Fiyat → Ekle); kalemlerin toplamı işlem tutarına yazılıyor. Ürün
kataloğu/stok yok, her seferinde elle yazılıyor. Fatura satırı benzeri ama
raporlamaya girmiyor.

## Ek koşum 2: uygulamaya özgü derinleştirme (11 Eyl 2026)

Kullanıcı isteğiyle, formun kendine özgü kalan boşlukları + uygulamaya has
özellikler canlı test edildi. Yeni sentetik veri: "Ofis malzemesi" gideri
₺150 (20 Ağustos 2026, öğe dökümlü) ve "Danismanlik geliri" ₺2.500
(22 Ağustos 2026). Kareler `20`–`35`.

**Metodoloji notu (düzeltildi, 11 Eyl):** Bu iki test kaydı, testler bitince
yapay zekâ tarafından **kullanıcıya sorulmadan** silinmişti ve kontrol değeri
(Ana Hesap ₺40.800 / net ₺44.950) geri yüklenmişti. Bu, Money Manager/Wallet/
Bluecoins'in boşluk koşumunda (10 Eyl) izlenen yöntemle **tutarsızdı**: o üç
uygulamada B1/B2/A ek koşum verisi bilerek temizlenmedi, "Tur 2'ye seçilirse
veri sıfırlanıp yeniden kurulur" notuyla cihazda bırakıldı (bkz.
`money-manager.md`, `wallet-budgetbakers.md`, `bluecoins.md` "veri durumu"
notları). Kullanıcı bunu fark edip düzeltilmesini istedi: **iki kayıt aynı
tutar/tarih/öğe dökümü/fatura ekiyle emülatörde yeniden oluşturuldu** ve bu
kez **silinmeden bırakıldı**. Kural sabitlendi: **ek koşum test verisi hangi
uygulamada olursa olsun varsayılan olarak silinmez**, yalnız kullanıcı açıkça
isterse silinir. Hesap Defterim artık diğer üç uygulamayla tutarlı: Ana Hesap
₺43.150, genel net ₺47.300 (₺44.950 + ₺2.500 gelir − ₺150 gider) — kalıcı
olarak sapmış durumda; Tur 2'ye seçilse bile **sıfırlanmaz**, üzerine eklenir
(kural genelleştirildi, 11 Eyl — bkz. `MANUEL-TEST-PROTOKOLU.md`).

| Test | Bulgu | Sonuç | Kanıt |
|---|---|---|---|
| **Öğe eklemek tam akış** | Kalem kalem döküm dialoğu (Öğe/Miktar/Birim/Fiyat → Ekle); "Kagit 5 adet @ 20 = 100" + "Kalem 10 adet @ 5 = 50" eklendi, dialog içi Toplam **150** anında hesaplandı. TAMAM MI → ana forma dönünce **tutar alanı otomatik 150 oldu VE Notlar alanına kalem dökümü otomatik yazıldı** ("Kagit 5 adet @ 20 = 100\nKalem 10 adet @ 5 = 50"). Liste satırında bu Notlar metni başlık olarak görünüyor. | `Tamamlandı` — gerçek bir alt-toplam motoru, sahte değil | `20-oge-eklemek-dialog.png`, `21-oge-eklemek-tutar-notlar-otomatik.png` |
| **Fatura ekle tam akış** | Kamera/Fotoğraf Galerisi/PDF seçenekleri doğrulandı. **Fotoğraf Galerisi** emülatörde boş çıktı (ortam kısıtı, uygulama sınırlaması değil). **Kamera** gerçek izin diyaloğu istedi ("Uygulamayı kullanırken"), çekim + onay (✓) akışı çalıştı, form altına küçük thumbnail + kaldır (X) ikonu eklendi. Kayıttan sonra listede **ataç (📎) ikonu** görünüyor; düzenleme ekranında thumbnail'e dokununca **tam ekran görüntüleme diyaloğu** (İptal etmek/Silme) açılıyor. OCR yok, salt ek — ADR 0011 karşılığı yok (önceki bulguyla tutarlı) | `Tamamlandı` | `22-fatura-ekle-secenekler.png`, `23-kamera-onay-ekrani.png`, `24-kayit-eklendi-atac-ikonu.png`, `25-fatura-tam-ekran-goruntuleme.png` |
| **İşlem adları — "Özel" tam test** | "Özel" seçilince iki serbest metin alanı çıkıyor: "Aldığınız para için ad" / "Verdiğiniz para için ad". "Tahsilat" / "FaturaOdemesi" girilip kaydedildi → **uygulama genelinde her yer değişti**: ana ekran kolon başlıkları, iki dev buton, alt toplam etiketleri, yeni işlem formunun başlığı/sekmesi/alan etiketi, hatta **PDF export'un sütun başlıkları**. Yeni adla oluşturulan kayıt normal davranıyor (sadece etiket değişimi, üçüncü bir işlem türü/kategori eklemiyor — önceki "kategori DEĞİL" bulgusu doğrulandı) | `Tamamlandı` — global yeniden adlandırma, yeni tür değil | `26-islem-adlari-ozel-form.png`, `27-yeniden-adlandirilmis-basliklar.png` |
| **Zorunlu alan / boş tutar** | Tutar boş bırakılıp "Kaydet ve çık" denendiğinde **hiçbir hata mesajı çıkmıyor, form da kapanmıyor** — sessiz no-op. | `Belirsiz` davranış — kullanıcıya geri bildirim yok | `28b-bos-tutar-sessiz-red.png` |
| **Sıfır (₺0) tutar** | ₺0 ile kaydetme **sessizce kabul ediliyor**, listeye "0" satırı giriyor, Denge'ye etkisi yok. Önceki "Faz 4a" formunda tahmin edilen davranış şimdi canlı doğrulandı. | `Tamamlandı (uyarısız)` | `28-sifir-tutar-kabul-edildi.png` |
| **Arama** | Üstteki büyüteç → gerçek zamanlı canlı filtre. "Ada" yazınca yalnız eşleşen kayıt (Notlar/Açıklama alanında arıyor) kaldı, **alt toplamlar (Alındı/Ödendi/Denge) da filtrelenmiş sonuca göre yeniden hesaplandı**. | `Tamamlandı` | `29-arama-canli-filtre.png` |
| **Kalıcı silme (çöp kutusu)** | Silinmiş işlemler ekranında kayda basılı tutma → **Geri Yükle / Silme** context menüsü. "Silme" → ikinci bir onay diyaloğu ("İşlemi Sil" / İptal etmek / Silme) → kalıcı silme. **İki aşamalı silme modeli**: normal silme (soft-delete, çöp kutusuna düşer) + çöp kutusundan onaylı kalıcı silme. | `Tamamlandı` | `32-silinmis-islemler-context-menu.png`, `33-kalici-silme-onay.png` |
| **Bildiri (PDF/Excel) gerçek dosya** | "Dışa Aktarılan Veriler: SD kartta veya Dahili Depolamada **kasadefteri** adlı bir klasöre kaydedilir" uyarısı çıkıyor — ama **gerçek dosya farklı bir yola yazılıyor**: `Documents/Hesap Defterim/hesapdefterim <tarih>.pdf` (uyarı metni ile gerçek davranış arasında tutarsızlık, muhtemelen eski/güncellenmemiş metin). PDF içeriği doğrulandı: tam biçimlendirilmiş tablo (Tarih/Notlar/Açıklama-Kategori/Alındı/Ödendi/Hesaplar sütunları, renkli, alt toplamlarla) — **gerçek, kullanılabilir bir rapor**, boş kabuk değil. Yeniden adlandırılmış işlem adları (Tahsilat/FaturaOdemesi) export'a da yansıyor. | `Tamamlandı` — sağlam export | `34-bildiri-pdf-excel-secim.png` |
| **Not Defteri (uygulamaya özgü)** | Menüden ayrı bir ekran: tarih/saat + serbest metin not + **checkbox** (tamamlandı/beklemede). Alt özet **Tamamlandı/Beklemede/Toplam** sayaçları tutuyor (checkbox işaretlenince canlı güncellendi). Tarih aralığı filtresi (Herşey/Günlük/Haftalık/Aylık/Yıllık) var. Kayıt silme tek onaylı **kalıcı** silme (işlemlerin çöp kutusu modeli burada yok). Fiilen basit bir görev/hatırlatıcı listesi — muhasebe kaydıyla hiçbir bağı yok. | `Not` — "khatabook ailesi" özelliği, bizim kapsamımız dışı | `30-not-defteri-checklist.png` |
| **Nakit Hesap Makinesi (uygulamaya özgü)** | TL kupürlerine göre (₺200'den ₺0,05'e) adet × kupür = tutar hesaplayan bir kasa sayım aracı. Kupür adedi girilince satır toplamı VE genel Toplam canlı hesaplanıyor; üstteki serbest tutar kutusuna da otomatik yazılıyor (muhtemelen bir işlem formuna değer aktarma amaçlı). Muhasebe kaydı üretmiyor, salt hesap makinesi. | `Not` — esnafın fiziksel kasa sayımı için khatabook özelliği | `31-nakit-hesap-makinesi.png` |

Kare `35-kontrol-degeri-geri-yuklendi.png` bu ek koşumun **ara** doğrulama
adımına aittir (test kayıtları geçici olarak silinmişken alınan kontrol
görüntüsü); nihai durum değildir — bkz. yukarıdaki "Metodoloji notu".

**Ek bulgu — "kasadefteri" uyarı metni yanlış:** Export diyaloğu kullanıcıya
`kasadefteri` adlı bir klasörden bahsediyor ama dosya gerçekte
`Documents/Hesap Defterim/` altına yazılıyor. Küçük ama gerçek bir UX/metin
tutarsızlığı — büyük ihtimalle uygulamanın eski bir sürümünden kalma metin.

## Açılış bakiyesi davranışı (dikkat çekici)

Defterin "Açılış bilançosu" alanı **tarihli** (biz Ağu-01-2026 verdik):

| Görünüm | Açılış bakiyesi nasıl görünüyor | Toplam Alındı'ya etkisi |
|---|---|---|
| Günlük / "Bugün" (dönem Ağustos'u kapsamıyor) | Üstte **"Önceki denge 20.000"** satırı (italik, ayrı) | ❌ Sayılmıyor |
| Herşey / Aylık-Ağustos (dönem 1 Ağu'yu kapsıyor) | Normal **"Açılış bilançosu 20.000"** Alındı satırı | ✅ Toplam Alındı'ya giriyor |

Yani açılış bakiyesi, dönem filtresine göre "taşınan bakiye" veya "gelir
kalemi" gibi davranıyor. Money Manager'ın "Bakiye Farkı" quirk'ine benzer ama
burada dönem içine düştüğünde topluyor. **Ayarlar → "Önceki denge"** onay kutusu
bu satırın gösterilip gösterilmeyeceğini kontrol ediyor. `17-onceki-denge-gunluk-gorunum.png`

## Arayüz taraması (görev dışı)

| Alan | Gezildi | Kısa gözlem |
|---|---|---|
| Menü (drawer) | ✓ | Özet · Hesaplar Özet · İşlemler-Bütün Hesaplar · Hesaplar · Aktar · Bildiri-Bütün Hesaplar · İşlem adları · Not Defteri · Takvim · Nakit Hesap Makinesi · Yedekleme ve geri yükleme · Ayarlar · Silinmiş işlemler · Yardım |
| "İşlem adları" | ✓ | **Buton etiketlerini yeniden adlandırma**: "Ödendi/Alındı" ↔ "Gelir/Gider" ↔ "Özel" (kendi kelimen). Uygulamanın aslında **jenerik bir giriş/çıkış defteri** olduğunu, "kasa defteri" veya "gelir-gider takibi" olarak çerçevelenebildiğini kanıtlıyor. Kategori DEĞİL | `05-siniflandirma-islem-adlari.png` |
| Özet / Hesaplar Özet | ✓ | Aktif defterin gün gün toplandığı özet: Alındı / Ödendi / **Tasarruf** (net) sütunları + dönem gezgini (`<` `>`). Kategori kırılımı yok | `13-ozet-tasarruf-agustos.png` |
| Bildiri-Bütün Hesaplar | ✓ | **PDF / EXCEL çıktı diyaloğu** (Herşey / Tarih Aralığı Seçin, PDF·EXCEL radyo, TAMAM MI). Ekranda rapor değil, dosya üretir. (Protokolde export kapsam dışı ama varlığı not) | — |
| Takvim | ✓ | Aktif defterin aylık takvim ızgarası; her gün hücresinde o günün giriş/çıkışı | `09-ozgun-ozellik-takvim.png` |
| Ayarlar | ✓ | Tarih/saat biçimi, para birimi biçimi, dil, **Karanlık Mod**, Parmak İzi + Şifre kilidi, "Önceki denge" aç/kapa, "Her işlemden sonra bakiyeyi göster" aç/kapa (satır başına yürüyen Denge), **"Açıklama / kategori ekle"** aç/kapa, Not önerileri | `15-ayarlar.png` |
| "Açıklama / Kategori" alanı | ✓ | Ayardan açılınca forma **"Açıklama / Kategori"** adlı **ikinci serbest metin alanı** ekleniyor — önceden tanımlı kategori listesi/ikonu YOK, sadece ikinci bir etiket kutusu | `16-kategori-alani-acik-form.png` |
| Silinmiş işlemler | ✓ | **Çöp kutusu** — silinen işlem burada durur; satıra dokun → **Geri Yükle** / kalıcı **Silme**. Money Manager'ın kalıcı silmesinden iyi | `08-silinmis-islemler.png` |
| Fatura ekle | ✓ | İşlem formunda "Fatura ekle" → **Kamera / Fotoğraf Galerisi / PDF**. Dosya eki, OCR yok. ADR 0011 öneri katmanının karşılığı değil |
| Öğe eklemek | ✓ | Formda "Öğe eklemek" = tek işlem içinde **kalem kalem döküm** dialoğu: Öğe adı / Miktar / Birim / Fiyat → Ekle; "Öğe (N)" + "Toplam" işlem tutarına yazılıyor. Ürün kataloğu/stok yok |
| Not Defteri, Nakit Hesap Makinesi | Menüden görüldü | Serbest not defteri + nakit/kupür sayım makinesi (khatabook aile özellikleri) |
| Boş durum | ✓ | Boş defter: yalnız "Tarih / Alındı / Ödendi" başlık satırı + sıfır toplamlar. Yönlendirici metin yok | `02-bos-ana-ekran.png` |
| Hata / uç durum | Kısmen | Formdan "geri" ile çıkış **taslak uyarısı vermiyor**, girilen veri sessiz kayboluyor. Silme tek onaylı (ama soft-delete). Yedekleme kapalıyken tekrar tekrar uyarı diyaloğu | `14-yedekleme-nag-dialog.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Tek eksen: zaman (gün grupları) → satırlar. Her ekranın altında sabit **Toplam Alındı / Toplam Ödendi / Denge** üçlüsü. Satır başına isteğe bağlı yürüyen "Denge" |
| Alt/üst gezinme | Alt gezinme çubuğu yok; her şey **hamburger drawer** + üstteki **defter seçici açılır menü** ("Hesap Defterim ▼"). İki büyük eylem butonu altta sabit |
| Renklerin anlamı | Alındı/gelir = **yeşil**, Ödendi/gider = **kırmızı**, Denge = mavi/siyah (negatifse kırmızı). Tutarlı. Üst bar canlı mavi |
| Tipografi ve para | Sayılar büyük ve sağa hizalı; binlik ayraçlı, ondalık gösterilmiyor (tam sayı). Para birimi simgesi ekranda görünmedi |
| Kart/liste/grafik | **Grafik hiç yok.** Yalnız gün-gruplu liste + özet tablosu + takvim ızgarası |
| Form alanları | Tek ekran, ~5 alan; tutar için ayrı hesap-makinesi ikonu; Notlar'da sesli giriş (mikrofon); tarih ayrı takvim diyaloğu (varsayılan bugün) |
| Loading/boş/hata/başarı | Başarı = küçük toast ("İşlem Eklendi" / "İşlem Silindi"). Boş durum yönlendirici değil. Hata mesajı görülmedi (zorunlu alan denenmedi); form terkinde uyarı yok |
| Erişilebilirlik | Dokunma alanları büyük (iki dev buton). Kalıcı reklam banner'ı alt kısmı yer yer kaplıyor. Karanlık mod var. Metin yoğunluğu düşük |

## Sistem işleyişi / pipeline

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Yalnız aktif defterin **yürüyen bakiyesini** günceller (Alındı → +, Ödendi → −). İkinci bir tablo/rapor kaydı yok; "rapor"lar aynı satırların türev toplamı | Manuel gözlem |
| Kart harcaması → kart borcu → kart ödemesi zinciri | **Yok.** Kart = negatif bakiyeli bir defter. "Ödeme" = Ana Hesap→kart defteri Aktar. Borç, ekstre, kesim/son ödeme tarihi kavramı hiç yok | Manuel gözlem |
| Transfer / kart ödemesi gelir-gider raporundan ayrışıyor mu | **Hayır.** Aktar **iki bağımsız işlem satırı** üretir (çift kayıt nesnesi değil); her ikisi de o defterin ve birleşik görünümün Toplam Alındı/Ödendi'sine girer. Yalnız net Denge doğru kalır — o da bacaklar senkronsa: bir bacak silinince diğeri öksüz kalıp net varlığı bozuyor | Manuel gözlem |
| Fatura/borç → tahsilat/ödeme → kapanış akışı | **Yok.** Fatura nesnesi, cari/müşteri kaydı yok. En yakını: müşteri başına ayrı defter açıp veresiye bakiyesini takip etmek (ama kişi/rollup yok) | Manuel gözlem |
| Tekrarlayan/planlı kayıt: tanım mı üretir, onay mı bekler | **Tekrarlayan/planlı işlem özelliği yok** (5 yerde tarandı). B1 (₺600/ay abonelik) ve B2 (₺6.000/6 taksit) bu uygulamada kurulamaz — kullanıcı elle tekrar girer | Manuel gözlem |
| İşletme/şahsi ayrım hangi katmanda | **Hiçbir katmanda.** Ne kapsam, ne kategori, ne mod. En fazla "işletme defteri" / "şahsi defter" diye ayrı defter | Manuel gözlem |
| Ekrandan ekrana tipik yol | `Alındı/Ödendi butonu → tutar → (Notlar) → tarih → Kaydet ve çık → defterin listesi + Denge güncellenir` | Manuel gözlem |
| Entegrasyon/dış sistem temas noktaları | Yalnız **Google Drive** (yedekleme/geri yükleme) ve **PDF/Excel** dışa aktarma. Banka/e-belge/POS/muhasebeci entegrasyonu yok | Manuel gözlem |
| Veri nereye yazılıyor | **Tamamen yerel** (cihaz). "Kayıtlarınızı sunucularımızda saklamıyoruz" — Drive yedeği kapalıysa cihaz kaybı = veri kaybı | Resmî kaynak (uygulama içi diyalog) |

**Pipeline şeması (kısa):**
`+ Alındı/Ödendi → tutar + not + tarih → Kaydet → aktif defter listesi + yürüyen Denge → (Özet/Takvim/Bildiri hepsi aynı satırların türevi)`

## Akış özeti

- **En kısa ve güçlü akış:** İşlem ekleme — dev renkli buton → tutar → Kaydet (~3–4 dokunuş); "Kaydet ve devam Et" ile seri giriş
- **En fazla sürtünme yaratan akış:** Anlamlı gelir/gider raporu istemek — transferler ve açılış bakiyesi toplamlara karıştığı için "bu ay ne kazandım" sorusunun net cevabı yok
- **Uygulamanın hedef kullanıcı varsayımı:** Kâğıt kasa/veresiye defteri tutan esnaf; tek elle giriş, tek bakış bakiye
- **İşletme ve şahsi para yaklaşımı:** **Yok.** Kapsam/kategori/mod bulunmuyor; en fazla ayrı defter
- **Transfer ve kart ödemesi yaklaşımı:** Aktar özelliği var ama **gelir/giderden ayrışmıyor** — yalnız net bakiye doğru
- **Planlama, borç ve tahsilat yaklaşımı:** Tekrarlayan/plan/taksit **yok**; borç = negatif defter bakiyesi; tahsilat = Aktar

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşletme/şahsi ayrımının hiç olmaması | Alma (kurucu kararımız tersi) | ADR 0013: ayrım bir raporlama boyutu. Hesap Defterim bu ihtiyacı hiç karşılamıyor — asıl farkımızın negatif referansı | — |
| Kategori yok; açılabilen "Açıklama/Kategori" yalnız ikinci serbest metin | Alma | Bizde kategori paylaşılan raporlama kovası + bütçe ekseni; serbest metin bunu karşılamaz | Kategori seçimi |
| Transfer'in gelir/gider toplamlarına karışması (yalnız net Denge doğru) | Alma | ADR 0013 + finansal kurallar: Transfer gelir/gider raporuna 0 etki eder. Güçlü negatif örnek — "sadece bakiye tutuyor" yetmez | Aylık rapor, birleşik feed |
| Kart = sadece negatif bakiyeli defter; borç/ekstre/kesim yok | Alma | Bizde CreditCardCharge/Payment + ekstre projeksiyonu var; tek negatif bakiye kart borcunu modellemiyor | Kart ekranı, ekstre |
| **Tekrarlayan (B1) ve taksit (B2) özelliği hiç yok** | Alma (negatif referans) | Bizde `RecurringTransaction` + `InstallmentPlan` + planlanan projeksiyon var. Hesap Defterim bu ihtiyacı hiç karşılamıyor — kullanıcı her ayı/taksiti elle giriyor | Planlama, tekrarlayan, taksit |
| **Transfer iki bağımsız satır; bir bacak silinince/düzenlenince diğeri öksüz kalıyor** (net varlık sessizce bozuluyor) | Alma | BusinessFinance `Transfer` tek atomik kayıt (kaynak −, hedef +, rapora 0). Çift kayıt bütünlüğü olmadan transfer güvenilmez | Transfer modeli |
| Kısmi kart ödemesi serbest (jenerik Aktar, override edilecek kavram yok) | Doğrudan al (uyumlu) | Bizim kısmi ödeme/tahsilat (D3) ile aynı yön; ama bizde ayrıca "hangi ekstreye" mantığı var | Kart ödemesi |
| Fiş = Kamera / Galeri / PDF eki, OCR yok | Not | ADR 0011 öneri katmanı burada yok | — |
| Ücretsiz sürümde navigasyonda tam ekran geçiş reklamı | Alma | — | — |
| **"Önceki denge" satırı** — dönem başında taşınan bakiyeyi ayrı, italik bir satırda göstermek | Uyarlayarak al | Hesap defteri / aktivite feed'inde bir döneme girerken "devir bakiyesi"ni göstermek okunabilirliği artırır. **Ama** dönem içine düştüğünde toplama sızması bizim kaçınmamız gereken hata | Hesap detayı / dönemli feed |
| **Satır başına yürüyen "Denge"** (her işlemden sonra bakiye) | Uyarlayarak al | Hesap detay defterinde her satırın yanında o andaki bakiyeyi göstermek klasik ve faydalı; bizde bakiye zaten hesaplanıyor | Hesap detayı |
| **Soft-delete + "Silinmiş işlemler" çöp kutusu + Geri Yükle** | Doğrudan al (ruhen) | "Sil yerine koru" ilkemizle uyumlu; Money Manager'ın kalıcı silmesinden iyi. Biz idempotent iptal tercih ediyoruz ama geri alınabilirlik aynı yönde | İşlem iptal/düzeltme |
| "Kaydet ve devam Et" (seri giriş) | Uyarlayarak al | Money Manager bulgusuyla aynı — hızlı ardışık giriş; bizde ek alanlar var | İşlem ekle formu |
| **"Öğe eklemek" — kalem dökümünün tutarı VE notu otomatik doldurması** | Uyarlayarak al | Kalem kalem döküm girilince toplamın tutar alanına, dökümün açıklamaya otomatik yazılması gerçek bir kolaylık; bizde "Öğe" kavramı yok ama fiş/fatura satırı kırılımı ileride düşünülebilir | İşlem ekle formu (gelecek) |
| **Boş tutarla kaydetmede sessiz red (hata mesajı yok)** | Alma | Kullanıcı neden kaydedilmediğini anlamıyor; bizde zorunlu alan hatası açık mesajla gösterilmeli | Form doğrulama |
| **Sıfır (₺0) tutarın uyarısız kabul edilmesi** | Alma | Anlamsız bir kayıt sessizce oluşuyor; bizde ya reddedilmeli ya açık onay istenmeli | Form doğrulama |
| **Arama sonucuna göre alt toplamların yeniden hesaplanması** | Doğrudan al (ruhen) | Filtrelenmiş görünümde toplamların da filtreye uyması okunabilirliği artırıyor; bizde aktivite feed'i filtrelerken benzer ilke uygulanabilir | Aktivite feed / arama |
| **İki aşamalı silme: soft-delete + çöp kutusunda ayrı onaylı kalıcı silme** | Doğrudan al (ruhen) | "Sil yerine koru" ilkemizle uyumlu, üstüne kalıcı silmeye ikinci bir güvenlik onayı ekliyor | İşlem iptal/düzeltme |
| **Export diyaloğunun anlattığı klasörle gerçek kayıt yerinin tutarsız olması** | Alma (negatif referans) | Kullanıcıya yanlış bilgi veren bir uyarı metni; bizim export/yedek akışlarımızda gösterilen yol her zaman gerçek yolla birebir tutmalı | Dışa aktarma akışı |
| Not Defteri — basit görev/hatırlatıcı listesi (checkbox + Tamamlandı/Beklemede sayacı) | Not | Muhasebe kaydıyla bağı yok, "khatabook ailesi" ek özelliği; bizim planlama/hatırlatıcı modelimizle karıştırılmamalı | — |
| Nakit Hesap Makinesi — kupür bazlı kasa sayım aracı | Not | Esnaf için fiziksel kasa mutabakatı; bizim dijital-öncelikli modelimizde karşılığı yok, düşük öncelik | — |
| İki dev renkli giriş/çıkış butonu (yeşil Alındı / kırmızı Ödendi) | Uyarlayarak al | Birincil eylemi tartışmasız kılıyor; bizde daha çok kayıt türü var ama "gelir/gider" ayrımını renkle güçlü vermek doğru (ADR 0008 iki-ton) | İşlem ekle launcher |
| "İşlem adları" — buton etiketlerini kullanıcıya yeniden adlandırtma | Alma | Bizde alan semantiği sabit (gelir/gider/transfer/kart ödemesi/cari...). Yeniden adlandırma anlamı bulanıklaştırır | — |
| Çok-defter modeli (her defter bağımsız bakiye) = hesap/kapsam yerine geçen çözüm | Alma | Bizde gerçek çok-hesap + tek havuz + kapsam boyutu var; "her şey için ayrı defter" bunun zayıf ikamesi | — |
| Yerleşik PDF/Excel çıktı (Bildiri) | Not | Bizim kendi dışa aktarma/yedek tasarımımız var; ayrı incelenir | — |
| Formdan çıkışta taslak uyarısı yok | Alma | Money Manager bulgusuyla aynı; bizde girilen veri korunmalı/uyarılmalı | Form doğrulama |
| Agresif yedekleme uyarısı + yalnız-yerel veri | Alma / dikkat | Bizde backup/restore bir sürüm kapısı. Ama "veriniz son yedeğiniz kadar güvende" kaygısını **nag yerine güven veren** bir akışla çözmeliyiz | Yedekleme akışı |
| Fatura ekle = sadece fotoğraf, OCR yok | Not | ADR 0011 öneri katmanı burada yok | — |

## Kanıt ve güven düzeyi

- **Manuel gözlem (10 Eyl 2026):** K00–K08 + arayüz taraması + **A/B/B1/B2 ek koşumu**, Ağustos 2026 sentetik veriyle, 3 defter + 5 çekirdek işlem (transfer + kart ödemesi dâhil). Kontrol değerleri (bakiye/net) birebir tuttu; A/B1/B2 test kayıtları koşumdan sonra silinip çöp kutusundan tümüyle temizlendi (kontrol durumu geri yüklendi). Kareler `kanitlar/hesap-defterim/00`–`19`
- **Manuel gözlem (11 Eyl 2026, ek koşum 2):** Öğe eklemek, İşlem adları (Özel), zorunlu alan/sıfır tutar, arama, kalıcı silme, Bildiri (PDF gerçek dosya doğrulandı), Not Defteri, Nakit Hesap Makinesi. Ek sentetik kayıtlar (₺150 gider + ₺2.500 gelir) MM/Wallet/Bluecoins konvansiyonuna uyacak şekilde **cihazda bırakıldı** (bkz. "Metodoloji notu"); Ana Hesap ₺43.150, genel net ₺47.300. Kareler `kanitlar/hesap-defterim/20`–`35`
- **Resmî kaynak:** Play Store listesi (4,8★ / 139 B yorum / 10 Mn+ indirme / geliştirici ANKIT SARAF / son güncelleme 23 Ağu 2026); uygulama içi "veriler sunucuda saklanmıyor" diyaloğu
- **Yorum:** "Anlamlı gelir/gider toplamı yok" — kontrol değerlerinde Toplam Alındı/Ödendi'nin açılış bakiyesi + transferleri içermesiyle desteklendi
- **B1/B2 negatifi doğrulandı:** tekrarlayan ve taksit özelliği yokluğu 5 ayrı yerde tarandı (işlem formu, form 3-nokta, ana ekran 3-nokta, drawer'ın 14 kalemi, Ayarlar) — hiçbirinde plan/tekrar/hatırlatıcı yok
- **Doğrulanamadı:** zorunlu-alan hata mesajının tam metni (sıfır/boş tutarla kaydetme denenmedi)
- **Kapsam dışı (protokol):** PDF/Excel export ve Drive yedek/geri yükleme akışları detaylandırılmadı

## Tek cümlelik sonuç

Hesap Defterim, esnafın kâğıt kasa/veresiye defterini birebir dijitalleştiren,
etiketleri yeniden adlandırılabilen tek-sütunlu yürüyen-bakiye defteri;
"Önceki denge" devir satırı, satır başına yürüyen bakiye ve soft-delete çöp
kutusu alınmaya değer, ama hesap türü, kategori, işletme/şahsi ekseni ve
transfer ayrımı hiç olmadığı için ağırlıkla **negatif referans** — havuzu bölmeyen
ama kapsamla raporlayan modelimizin neden gerektiğini doğruluyor.
