# Uygulama Gözlem Formu — Paraşüt

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Paraşüt / Paraşüt Yazılım (Mikrogrup) |
| Sürüm | 5.25.0 (Android paketi; `01-ilk-acilis-carousel4.png`/`01b-carousel.png` karelerinde sürüm görünmüyor, koşum notu) |
| Test tarihi | **1 Eyl 2026** (mobil giriş öncesi yüzey) · **2 ve 9 Eyl 2026** (parasut.com kullanım kılavuzu) · **10 Eyl 2026** (tanıtım videosu, kullanıcı izledi) · **12 Eyl 2026** (Faz 7.5 doğrulama turu + kılavuz kontrolü) |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Türkçe / TL |
| Hesap veya plan türü | — (hesap açılamadı) |
| Erişim kısıtı | **Kayıt engeli** — mobil uygulamada kayıt yok; ekranda yalnız `Giriş yap` ve `Parolanızı mı unuttunuz?` var. Kayıt yalnız web'de (parasut.com), şirket/VKN bilgisi gerektiriyor ve ücretli (web kayıt koşulları karede yok, bu tur doğrulanmadı) |
| İnceleme türü | **Masa başı.** Manuel gözlem yalnız giriş öncesi karusele ait; geri kalan her şey `Resmî kaynak` |

### Kanıt tavanı (bu formun sınırı)

Bu uygulamaya **hiç girilemedi**. Ürünün içine dair her cümle üç kaynaktan
birine dayanıyor: mobil giriş öncesi ekran (manuel gözlem), parasut.com
kullanım kılavuzu (resmî kaynak) ve animasyonlu tanıtım videosunun kareleri
(resmî kaynak — pazarlama kurgusu). **Hiçbir davranış doğrulanmadı.**

Bu asimetri bilinçli: Paraşüt, KolayBi ve Logo İşbaşı aynı kulvarda
(Türk ön muhasebe) ve özellik yüzeyleri büyük ölçüde örtüşüyor; **derin
inceleme için KolayBi seçildi** (kullanıcı kararı). Paraşüt ve Logo bu
derinliğe kasten çıkarılmadı, yalnız metinleri kaynakla tutarlı hâle
getirildi. Belge 1/2'de bu fark açıkça yazılır.

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Muhasebe bilgisi gerektirmeden **fatura kesen, cari takip eden, e-dönüşümü yöneten** web tabanlı bulut ön muhasebe programı |
| Asıl hedef kullanıcı | KOBİ, mikro işletme, serbest çalışan, **e-ticaret satıcısı**, üretim/mağaza işletmesi. "İşletme" tarafı — patronun şahsi bütçesi hedefte değil |
| Çözdüğü ana iş | Satış/alış faturası + e-Fatura/e-Arşiv + cari hesap + tahsilat-ödeme takibi + nakit akışı |
| Açıkça kapsam dışı bıraktığı | Bordro (maaş kaydı var ama SGK/bordro hesabı yok), yatırım takibi, **kişisel/gündelik bütçe** |
| İş modeli | Ücretli abonelik; e-Fatura kontör ayrı |
| BusinessFinance ile aynı kulvarda mı | **Kısmen.** Gelir-gider + cari + fiş okutma + tekrarlayan ortak; ama Paraşüt bir **fatura/e-belge + firma defteri** ürünü. İşletme/şahsi tek havuz kavramı yok |

## Görev gözlemleri

| Görev | Sonuç | Not | Kanıt |
|---|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi (yalnız giriş öncesi) | **4 noktalı tanıtım karuseli** + tek CTA `Giriş yap` + `Parolanızı mı unuttunuz?`. Slayt 1: *"Paraşüt ile işletmenizin güncel durumunu anlık takip edin!"*; slayt 4: *"Oluşturduğunuz faturaları, müşterinizle veya tedarikçinizle hemen paylaşın"*. **Slayt 2 ve 3 çekilmedi.** Mobilde kayıt akışı yok | `01b-carousel.png` (slayt 1), `01-ilk-acilis-carousel4.png` (slayt 4) |
| K01–K08 | **Girilemedi → resmî kaynak** | Mobilde kayıt yok, web kaydı ücretli ve VKN istiyor. Akışlar aşağıdaki "Sistem işleyişi / pipeline" bölümünden | — (`Doğrulanamadı`) |

## Resmî kaynak — özellikler ve kapsam

`Resmî kaynak (kullanım kılavuzu / ürün sayfaları)`, parasut.com, 2 ve 9 Eyl
2026. Emülatörde doğrulanmadı.

**Not:** aşağıdaki "kapsam dışı" ibareleri bir **öneri kararıdır** (Belge 3),
gözlem değil. Her özellik Belge 1/2'de tarafsız anlatılır; patron kapsamı
genişletmek isteyebilir.

| Grup | Özellik | BusinessFinance açısından (Belge 3 ön değerlendirmesi) |
|---|---|---|
| Finans | Gelir-gider takibi, nakit akışı analizi, cari hesap (müşteri + tedarikçi) takibi | Cari bizde ADR 0014 ile var; nakit akışı görseli özet ekranıyla örtüşür |
| Fatura/e-belge | Fatura, teklif, e-Fatura, e-Arşiv, e-İrsaliye, e-SMM, e-İhracat faturası | Biz e-belge kesmiyoruz; `Obligation` / `CounterpartyCharge` tanıma modeli var |
| Tahsilat/ödeme | Çek-senet takibi, tahsilat hatırlatmaları, ödeme uyarıları, alacak/borç izleme | Tahsilat/ödeme bizde var; çek-senet ve hatırlatma yok (`upcoming-payments` kısmi karşılık) |
| Stok/lojistik | Çok depolu stok takibi, depolar arası transfer, sevkiyat takibi | ERP değiliz |
| **Tekrarlayan fatura** | Otomatik tekrarlayan fatura oluşturma; tekrarlayan gider ayrı akış | Bizde `RecurringTransaction` var ama fatura değil, kayıt üretir |
| **Fiş okutma** | AI OCR ile fiş fotoğrafından otomatik gider kaydı | Bizde ADR 0011: öneri katmanı, yönü/ödeme kaynağını seçmez |
| **KDV raporu** | `Hesaplanan KDV` / `İndirilecek KDV` / `Net KDV` — aşağıda ayrı başlık | **ADR 0016 ile net fark**; DURUM'da `kararı yeniden sor` işaretli konunun ikinci bağımsız kaynağı |
| Entegrasyon | Banka entegrasyonu, e-ticaret (Trendyol/N11/Shopify/Hepsiburada), online tahsilat, CRM, saha ekibi | Banka bağlantısı ve ödeme başlatma bizde kesin kapsam dışı |
| **Muhasebeci erişimi** | Muhasebeci hesaba erişip **veriyi anlık görüyor**; dosya/yedek paylaşımı yok | Logo Müşavir Portal ile aynı model. Bizde dosya tabanlı tek yönlü dışa aktarma paketi (Aşama 05 Grup 5) |
| Mobil | iOS/Android'de fatura kesme, gider, tahsilat, fiş okutma | Bizde mobil ana istemci |

### KDV raporu (12 Eyl 2026'da kılavuzdan doğrulandı)

Bu bölüm, daha önce yalnız bir **video karesinden** okunmuş olan "Paraşüt
KDV'yi hesaplıyor" cümlesinin yerini alıyor; artık resmî kılavuza dayanıyor.

- Menü: **`Raporlar > KDV raporları`** (yeni menüde **`Giderler > KDV
  raporları`**).
- Rapor, girilen gelir ve gider kalemlerindeki KDV tutarlarından **ay bazında**
  üç değer üretiyor: **`Hesaplanan KDV`**, **`İndirilecek KDV`**, **`Net KDV`**.
- Bir aya tıklanınca o ayın KDV'sini oluşturan satış ve gider dökümü açılıyor.
- Filtre: **`Tümü`** / **`Satışlar`** / **`Giderler`**; **`DIŞARI AKTAR`** ile
  Excel indiriliyor.

**Ne kazandırıyor:** aylık KDV beyannamesi veren kullanıcının hazırlık işini
ürün üstleniyor; müşavire giden döküm tek tıkla çıkıyor.
**Ne kaybettiriyor:** oran listesini üründe güncel tutma sorumluluğu doğuyor ve
yanlış hesap kullanıcıyı doğrudan yanıltır. ADR 0016 bu sorumluluğu almamayı
seçti (KDV taşınır, hesaplanmaz); **ödünleşim Belge 3'te tartışılır** ve aynı
desen KolayBi'de de görüldüğü için `kararı yeniden sor` başlığı altında
patrona sorulacak konular arasında.

## Sistem işleyişi / pipeline

`Resmî kaynak (kullanım kılavuzu)` — parasut.com, 9 Eyl 2026. **Canlı ürün
davranışı doğrulanmadı.**

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Gider kaydı türleri | **Beş tür:** Detaylı Fiş/Fatura (kalem + stopaj/tevkifat), Hızlı Fiş/Fatura (yalnız toplam tutar), Maaş/Prim (otomatik tekrarlama), Vergi/SGK Primi, Banka Gideri. Giriş yolu: `GİDERLER > Gider Listesi > DETAYLI FİŞ / FATURA` | Resmî kaynak (kullanım kılavuzu) |
| Kayıt ile ödeme ayrı adım | **Önce gider oluşturulur, sonra ödeme eklenir.** Ödeme kasa/banka bakiyesini azaltır; gider kaydının kendisi azaltmaz. Kısmi ödeme destekleniyor. Tedarikçi eşleştirmesi isteğe bağlı | Resmî kaynak (kullanım kılavuzu) |
| Cari borçlandırma yolları | Üçü de: (1) müşteri kaydında açılış bakiyesi, (2) satış faturası, (3) borç yokken ödeme ekleme = avans. Tedarikçi için simetrik | Resmî kaynak (kullanım kılavuzu) |
| Cari bakiye hesabı | Otomatik: fatura arttıkça borç artar, tahsilat yapıldıkça azalır. **Mahsuplaştırma otomatik** — para hareketi en çok gecikmiş açık faturadan başlayarak eşleşir | Resmî kaynak (kullanım kılavuzu) |
| Tahsilat akışı | `Tahsilat Ekle` → banka hesabı + tutar (çoklu döviz) → cari bakiye kapanır + banka bakiyesi artar. Vade gelince **otomatik hatırlatma e-postası** | Resmî kaynak (kullanım kılavuzu) |
| Fiş okutma | Mobil kamera → fiş fotoğrafı yükleme → AI OCR ile gider kaydına dönüşüm | Resmî kaynak (ürün sayfası) |
| Tekrarlayan | Tekrarlayan gider ve tekrarlayan fatura **ayrı akışlar**; abonelik faturalaması için otomatik oluşturma | Resmî kaynak (ürün sayfası) |
| KDV | `Hesaplanan` / `İndirilecek` / `Net KDV`, ay bazında, Excel'e aktarılabilir | Resmî kaynak (kullanım kılavuzu) |
| İşletme/şahsi ayrım | **Yok.** "Ortak/patron cebinden" gider ancak **personel/ortak cari bakiyesi** üzerinden dolaylı girebilir — yani şahsi harcama bir alacak-borç kalemine dönüşür | `Çıkarım` (kılavuzda böyle bir kullanım anlatılmıyor; cari modelinden türetildi) |
| Entegrasyon temas noktaları | Banka entegrasyonu (otomatik ödeme/mutabakat), e-ticaret pazaryerleri, online tahsilat, GİB e-belge, muhasebeci **canlı erişim** | Resmî kaynak (ürün sayfası) |
| Veri nereye yazılıyor | Bulut (web ana istemci); mobil tamamlayıcı | Resmî kaynak (ürün sayfası) |

**Pipeline şeması (satış → tahsilat):**
`müşteri seç → satış faturası (kalem/vergi oranı/indirim) → e-Fatura/e-Arşiv
gönder → cari borç +tutar → (vade) otomatik hatırlatma → Tahsilat Ekle (banka +
tutar) → otomatik mahsup (en gecikmiş faturadan) → cari kapanır + banka +tutar
→ nakit akışı ve yaşlandırma raporu güncellenir`

**Pipeline şeması (gider):**
`gider türü seç (5) → tutar/kalem (+ tedarikçi ops.) → fiş fotoğrafı (OCR) →
kaydet → gider raporu +tutar, cari borç +tutar → Ödeme Ekle → kasa/banka
−tutar, cari kapanır`

## Tanıtım videosu kareleri — ve bu karelerin sınırı

**Kaynak:** "Paraşüt ile neler yapabilirsiniz?" tanıtım videosu (~2–3 dk,
animasyonlu); kullanıcı 10 Eyl 2026'da izleyip kareleri aktardı. Etiket:
`Resmî kaynak (tanıtım videosu)`. Kareler animasyonda **masaüstü pencere
çerçevesi** içinde çiziliyor; web mi masaüstü mü olduğu karede yazmıyor. Mobil
yalnız `02-video-fatura-gonderme.png`'de yan yana çıkıyor. `05-video-banka-entegrasyonu.png`'te oynatıcı öğeleri (video başlığı,
beğen/yorum/paylaş) görünüyor; tam URL ve yayın tarihi yine yok.

### Sayılar tutmuyor — kareler davranış kanıtı değildir

12 Eyl doğrulama turunda kareler tek tek okundu ve **dört ayrı karede
aritmetik tutarsızlık** bulundu. Bu, karelerin gerçek ürün çıktısı olarak
okunamayacağını ve **illüstrasyon** gibi ele alınması gerektiğini gösteriyor:

- `03-video-satis-faturasi-detay.png`: kalem `Çimento`, miktar **2,00**, birim fiyat 499,00 TL, vergi %5,
  **indirim %8** — ama `ARA TOPLAM` 499,00 (miktar yok sayılmış),
  `TOPLAM KDV` 24 TL, `GENEL TOPLAM` 523,00 TL (indirim uygulanmamış).
- `04-video-cari-hesap-durumu.png`: donut tutarları **`138.89,20 ₺`**, `94.45,10 ₺`, `39.12,00 ₺` —
  binlik/ondalık ayracı bozuk, okunabilir bir tutar değil.
- `07-video-gelir-gider-raporu.png`: `Gelirler 38.987,98` ama kategori kalemleri toplamı 39.348,98;
  `Giderler 10.387,91` ama kalemler toplamı 10.308,89. (Buna karşılık
  **`NET 28.600,07` iki toplamla tutarlı** — tek doğru aritmetik bu.)
- `08-video-nakit-akisi-raporu.png`: `80.987,98 − 22.987,98 = 58.000,00`, ama ekranda
  `Net Nakit Akışı 58.765,00 TL`. Ayrıca grafikte yalnız mavi (giriş)
  çubukları var, gösterge kırmızı (çıkış) da vaat ediyor.
- `05-video-banka-entegrasyonu.png`: beş hesabın (kasa hesabı dahil) **hepsinde aynı IBAN** yazıyor.
- 14 Eyl görsel incelemesinde eklenenler: `04-video-cari-hesap-durumu.png`'te `FATURA YOK` ve `ÖDEME YOK`
  onay işaretleri tutarlı donut'ların yanında duruyor, `TAHSİL EDİLECEK` ile
  `GECİKMİŞ` aynı tutarı taşıyor; `08-video-nakit-akisi-raporu.png`'de grafik ekseni `0 m`–`20 m` iken
  dönem girişi 80.987,98 TL, alttaki liste satırları (12–15 Ağustos 2022)
  başlıktaki `10 Ağustos 2022` bitişinin dışında.

**Sonuç:** bu karelerden **ekran adları, alan adları ve akış niyeti** okunur —
bunlar gözlemdir. **Hesaplama davranışı okunmaz**; o yüzden "KDV'yi
hesaplıyor" cümlesi bu karelerden değil, yukarıdaki kullanım kılavuzu
bölümünden alınmıştır.

### Ekran seviyesi akış (video kareleri)

| # | Kare | Ekranda görünen | BusinessFinance açısından |
|---|---|---|---|
| 02 | `02-video-fatura-gonderme.png` | Solda mobil: e-Fatura belgesi + **"Fatura Gönderildi!"**. Sağda masaüstü `Satış Faturaları > Fatura`: `MRT Yapı Malzemeleri`, `21 Eylül 2022`, `#FA020200000409`; kalem `Çimento 499,00 TL %5 523,00 TL`; `ARA TOPLAM 499,00` / `TOPLAM KDV 24 TL` / `GENEL TOPLAM 523,00 TL`. Sağ panel: `FATURA GÖNDERİLDİ`, `e-Arşiv FATURA`, `PAYLAŞ`, **`KALAN 523,00 TL`** + *"7 gün sonra tahsil edilecek"*, `Müşteri hatırlatma ekle`, `Tahsilat talep et`, **`TAHSİLAT EKLE`** (aktif), `İrsaliyeli Fatura`, `Müşteri Ekranı Açık`, `Fatura Geçmişi`. Mobildeki belge başlığı `E-Fatura`, masaüstü paneli `e-Arşiv FATURA` | **Tahsil edilmemiş hâl.** Fatura kesilmiş ama para gelmemiş; tahsilat ayrı bir eylem olarak duruyor. Cari borcun yazıldığı karede görünmüyor (`çıkarım`) |
| 03 | `03-video-satis-faturasi-detay.png` | Video başlığı "Cari Hesap Takibi"; **tahsil edilmiş** bir satış faturası (`02-video-fatura-gonderme.png`'den ayrı örnek: tarih, belge numarası, belge türü ve miktar sütunu farklı): `Satış Faturaları > Satış Faturası`, `08 Haziran 2022`, `#GÖNDERİLİYOR`, `Temel e-Fatura • GÖNDERİLİYOR`; kalem `Çimento` `ÇIKIŞ DEPO: IST` `MİKTAR 2,00` `499,00 TL` `%5` `523,00 TL`, **`İNDİRİM %8`**; iki alıcı e-postası "Yolda"; sağ panelde yeşil ✓ **`TAHSİL EDİLDİ 523,00 TL`**, `TAHSİLAT EKLE` soluk (pasif görünümlü); ayrıca `İrsaliyeli Fatura`, `Müşteri Ekranı Açık`, `BARKOD`, `CARİSİZ`, `DEPO` | **Kayıt ≠ ödeme burada görünür**: `02-video-fatura-gonderme.png` "Kalan", `03-video-satis-faturasi-detay.png` "Tahsil edildi" — iki ayrı örnek faturada iki durum; aynı faturanın geçişi gösterilmiyor. Bizde `CounterpartyCharge` tanır + `CounterpartyPayment` taşır ile aynı ayrım (ADR 0014). `CARİSİZ` düğmesi de dikkat çekici: cari bağlamadan fatura kesilebildiği `çıkarım` (karedeki faturada müşteri seçili) |
| 04 | `04-video-cari-hesap-durumu.png` | Video başlığı "Cari Hesap Takibi"; ekranda **`Güncel Durum`**. `Tahsilatlar`: `TAHSİL EDİLECEK` / `GECİKMİŞ` / `FATURA YOK` (üç donut). `Ödemeler`: `ÖDENECEK` / `ÖDEME YOK` / **`PLANLANMIŞ`**. Sağda yeşil `BUGÜN - 22 EYLÜL` ve kırmızı `4 GÜN GECİKTİ — Tahsilat: 19.989,00 ₺` | Bizim **planlanan görünüm + counterparty balance** projeksiyonunun karşılığı. `PLANLANMIŞ` kovasının ileri tarihli/tekrarlayan ödemeleri tuttuğu `çıkarım`; `GECİKMİŞ` vurgusu bizdeki `attentionCode` ile paralel |
| 05 | `05-video-banka-entegrasyonu.png` | `Kasa ve Bankalar`: `HESAP İSMİ` / `IBAN` / `DÖVİZ CİNSİ` / `BAKİYE`. Satırlar: Yapı Kredi Maaş 40.000 TL, Ziraat Bankası Vadeli 10.000 TL, Akbank-Kadıköy 1.000 TL, Garanti Bankası Vadesiz 500,00 TL, **`Kasa Hesabı` 5.000 TL**. Döviz sütununda `TRL`; kasa hesabında da IBAN yazıyor. Butonlar: **`BANKA HESABI BAĞLA`**, `KASA EKLE`, `BANKA EKLE`. Solda 12 banka logosu | **Nakit ve banka aynı listede** — bizim `Account` modelimizle örtüşüyor. `BANKA HESABI BAĞLA` (otomatik hareket çekme) bizde kesin kapsam dışı; bizde hesap elle eklenir, bakiye açılış + hareketlerden hesaplanır |
| 06 | `06-video-stok-depo.png` | `Depolar > Depo`; başlık `Ana Depo (Varsayılan Depo)`, `Adres Pendik/IST`, sekmeler `Ürünler` / `Stok Geçmişi`; sütunlar `ÜRÜN ADI` / `STOK MİKTARI` / **`ALIŞ (VERGİLER HARİÇ)`** / **`SATIŞ (VERGİLER HARİÇ)`**. Dört satırın sonuncusu (`Çelik Konstrüksiyon`, 10 Adet) **kırmızı zeminli** | Bizde stok yok. Kırmızı satırın "kritik stok" anlamına geldiği **`çıkarım`** — ekranda bunu söyleyen bir etiket yok, yalnız videonun anlatısı öyle diyor. Alım/satım kaydına bağlı stok Belge 3'te `henüz karar verme` adayı |
| 07 | `07-video-gelir-gider-raporu.png` | `Gelir ve Gider Raporu`: `Filtrele` + tarih aralığı (`10 Şubat 2022 - 31 Temmuz 2022`) + **`Vergiler dahil`** açılırı. `Gelirler 38.987,98 TL` (Kategorisiz / Diğer / Altın / Gümüş) ve `Giderler 10.387,91 TL` (Kargo / Diğer / **`Kategorisiz`**), iki pasta grafik; altta **`NET 28.600,07 TL`** ve `DIŞARI AKTAR` | Bizim aylık gelir-gider raporumuzun karşılığı. **`Vergiler dahil` açılırı**: raporu KDV'siz okuma seçeneği (`hariç` seçeneği ekranda görünmüyor, `çıkarım`; `06-video-stok-depo.png`'daki `VERGİLER HARİÇ` stok sütunu başka ekrandır, bu açılırın seçeneğini kanıtlamaz). Bizde rapor tutarı brüttür ve brüt kalır (ADR 0016). **`Kategorisiz` listede boş daireli ayrı bir satır olarak duruyor** (ayrı kategori nesnesi olduğu karede görünmez) — kategorisiz kaydı gizlemek yerine adlandırmak, bizde de düşünülebilir. `NET` kelimesi seçilmiş, "kâr" değil |
| 08 | `08-video-nakit-akisi-raporu.png` | `Kasa / Banka Raporu`: `Toplam Nakit Girişi` / `Toplam Nakit Çıkışı` / **`Net Nakit Akışı`**; aylık bar grafik (Ocak–Ağustos) + `GÜN` `HAFTA` **`AY`** `YIL` (eksen `0 m`–`20 m`); altta `01 Ocak 2022 - 10 Ağustos 2022 Arası Yapılan Tahsilat ve Ödemeler` listesi: `İŞLEM TÜRÜ` / `İŞLEM TARİHİ` / `MÜŞTERİ / TEDARİKÇİ / ÇALIŞAN` / `AÇIKLAMA` / `ÇIKIŞ` / `GİRİŞ` | **Nakit akışı ile gelir-gider ayrı iki rapor**: bu rapor tahsilat/ödeme hareketlerini listeliyor; `07-video-gelir-gider-raporu.png`'nin tahakkuk esaslı saydığı karede yazmıyor (`çıkarım`, kılavuzla doğrulanmadı). Bizde de "bu ayın neti" kasa değişimi değildir (kart harcaması harcandığı gün gider yazılır). İki farklı zaman ekseninin ayrı ekranlarda durması iyi bir kıyas noktası |

### Videonun anlattığı hikâye (kullanıcı özeti, 10 Eyl)

- Açılış problemi: tedarikçi carileri, müşteri faturaları, tahsilat/ödeme ve
  banka hareketlerini ayrı ayrı yönetmek iş yükü; ciro büyüdükçe Excel + cari
  defter + matbu fatura yetmiyor, e-Faturaya geçiş gündeme geliyor.
- Çözüm kurgusu: matbu fatura maliyeti yerine **e-kontör**. Kullanıcı yorumu:
  *"Sanırım fatura kesme maliyetini e-kontör yapısıyla ücretlendirmişler."*
- Bilgisayardan veya telefondan fatura gönder; cari hesaplar tek yerde ve
  karşı tarafla paylaşılıyor; banka entegrasyonuyla hareketler otomatik;
  otomatik hatırlatmalar tahsilatı kaçırtmıyor; kritik stok uyarısı erken
  sipariş sağlıyor; mali müşavir hesaba eklenip belgeyi anında alıyor.
- Kullanıcı genel izlenimi: uygulama **e-Fatura üstünde çok duruyor**; küçük
  işletmeden çok orta/büyük işletmenin iş yükünü hafifletmeye konumlu.
  **Kişisel bütçeyle ilgili hiçbir şey yok.**

**Kullanıcı notu:** 20 sn'lik ikinci promo videoda ek bilgi yok. 36 dk'lık
"Uçtan Uca Program Eğitimi" videosu web sürümünü **gerçek arayüzle**
gösteriyor — derin inceleme KolayBi'ye verildiği için bu kaynağa
başvurulmadı.

## BusinessFinance için kararlar

> `alma` ve `kararı yeniden sor` satırlarında "ne kazandırıyor / ne
> kaybettiriyor" zorunludur (`MANUEL-TEST-PROTOKOLU.md` → yazım kuralı 6).
> Sonuç tanımları: `README.md` → "Karar sonuçları — tek kaynak".
>
> **Bu tablo masa başı kaynaklara dayanıyor**; hiçbir satır canlı davranışla
> doğrulanmadı. Aynı kulvardaki derin inceleme KolayBi'de yapıldığı için
> buradaki kararlar KolayBi formundakilerle birlikte okunmalıdır.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| **KDV'nin üründe hesaplanması** (`Hesaplanan` / `İndirilecek` / `Net KDV`, ay bazında, Excel'e aktarılır) | **Kararı yeniden sor** | **Boyut:** aylık KDV beyannamesi veren kullanıcının hazırlık süresi — hedef kitlemizin (şahıs şirketi, esnaf) doğrudan işi. **Kazandırdığı:** kullanıcı ayın KDV'sini üründe görüyor, müşavire giden döküm tek tıkla çıkıyor. **Kaybettirdiği:** oranları güncel tutma sorumluluğu ürüne geçiyor ve yanlış hesap kullanıcıyı doğrudan yanıltır; vergi hesabı bir kez üstlenilince geri alınamıyor. ADR 0016 bunu almamayı seçti. **Aynı desen KolayBi'de de var** — iki bağımsız kaynak; Belge 3'te ödünleşimiyle patrona sorulur | Vergi alanları, muhasebeci paketi |
| Kayıt ile ödemenin ayrı adım olması (gider oluştur → `Ödeme Ekle`; fatura kes → `TAHSİLAT EKLE`) | Doğrudan al (zaten böyle) | ADR 0014'ün "tanır / taşır" ayrımıyla aynı yapı; `02-video-fatura-gonderme.png` ve `03-video-satis-faturasi-detay.png` kareleri iki ayrı örnek faturada "Kalan" ve "Tahsil edildi" durumlarını gösteriyor. Kısmi ödeme bizde de `CounterpartyPayment` ile var; otomatik mahsup örtüşmüyor: bizde tahsilat belirli faturayı kapatmaz, yalnız cari bakiyenin vadesi geçmiş kısmı ödemeler önce vadesi geçmiş borçlandırmalardan düşülerek hesaplanır *(P1-B01 düzeltmesi, 14 Eyl 2026: önceki gerekçe otomatik mahsubun bizde örtüştüğünü söylüyordu)* | Cari akışı |
| Nakit akışı raporu ile gelir-gider raporunun **ayrı iki ekran** olması | Doğrudan al (ruhen) | İki farklı zaman ekseni (para ne zaman hareket etti / gelir-gider ne zaman doğdu) ayrı ekranlarda duruyor (ekran adlarından `çıkarım`; `07-video-gelir-gider-raporu.png`'nin sayım zamanı kılavuzla doğrulanmadı); bizim "bu ayın neti kasa değişimi değildir" ilkemizin doğrudan karşılığı | Özet ekranı, aylık rapor |
| Cari `Güncel Durum` panosundaki **`PLANLANMIŞ`** kovası ve `4 GÜN GECİKTİ` uyarı kartı | Uyarlayarak al | Yaklaşan ve gecikmiş tahsilatı tek bakışta veren kova mantığı, bizim planlanan görünümümüz + `attentionCode` ile aynı yönde; kova adlandırması referans alınabilir | Planlanan görünüm, upcoming-payments |
| Gelir-gider raporunda **`Kategorisiz`in ayrı bir satır olarak gösterilmesi** | Uyarlayarak al | Kategorisi olmayan kaydı gizlemek yerine adlandırıp toplama dâhil etmek, raporun toplamıyla kalemlerin toplamını ayrıştırmayı önlüyor; bizde de kapsamsız/kategorisiz kayıtlar için aynı ilke uygulanabilir | Aylık rapor kırılımı |
| Raporda **`Vergiler dahil`** seçeneği (raporu KDV'li/KDV'siz okuma) | Henüz karar verme | Fikir açık ama ADR 0016 gereği bizde rapor tutarı brüttür; KDV'siz okuma seçeneği sunmak, KDV'yi hesaplamaya çok yakın bir adım. Veri yetmiyor, KDV kararıyla birlikte ele alınmalı | Aylık rapor |
| İşletme/şahsi ayrımının kaynaklarda görülmemesi; patronun cebinin ancak **ortak/personel carisi** üzerinden girebileceği `çıkarım`ı | Alma | **Kazandırdığı:** ürün tek bir işi yapıyor ve o işte derin; müşavirin beklediği dili konuşuyor, şirketten çekilen para muhasebenin yerleşik yöntemiyle izlenebilir kalıyor. **Kaybettirdiği:** şahıs şirketi sahibinin kasası ile cebi hukuken ayrılmadığı için kullanıcının gündelik harcamasını ya ikinci bir uygulamada ya her seferinde bir cari kaydı açarak tutması gerekeceği `çıkarım`dır; kullanıcı davranışı gözlenmedi *(P1-B08)*. ADR 0013 ters ödünleşimi seçiyor: tek defter, kapsam bir raporlama boyutu | — |
| Banka hesabı bağlama / otomatik hareket çekme | Alma | **Kazandırdığı:** hareketler elle girilmiyor, mutabakat kendiliğinden oluşuyor ve kullanıcının veri girme yükü büyük ölçüde kalkıyor. **Kaybettirdiği:** sağlayıcı bağımlılığı, banka kimlik bilgisi güven sınırı ve düzenleme yükü doğuyor; `PROJECT-ROADMAP` bunu kapsam dışı bırakıyor. Bizde hesap elle eklenir, bakiye açılış + hareketlerden hesaplanır | — |
| e-Fatura / e-Arşiv / e-İrsaliye kesme (GİB entegrasyonu) | Alma | **Kazandırdığı:** faturayı kesen ile kaydı tutan aynı ürün olunca çifte giriş bitiyor; zorunlu e-dönüşüm kapsamındaki kullanıcı tek yerde kalıyor. **Kaybettirdiği:** kontör/maliyet modeli, GİB uyum yükü ve sürekli mevzuat takibi getiriyor — bu başlı başına ayrı bir ürün. Bizde `CounterpartyCharge` yalnız **tanır**, belge kesmez | — |
| Stok / çok depolu takip, kritik stok vurgusu | Henüz karar verme | Esnafın gerçek ihtiyacı olabilir ama ERP sınırına giriyor; kapsam kararı patronundur | — |
| Muhasebecinin hesaba **canlı erişimi** | Alma | **Kazandırdığı:** ay sonu dosya alışverişi tamamen kalkıyor, müşavir veriyi anlık görüyor, hata payı düşüyor. **Kaybettirdiği:** çok taraflı canlı erişim, bizim `ICurrentUser` + owner-scoped izolasyon modelimizi doğrudan zorlar ve yeni bir yetki katmanı gerektirir. Aşama 05 Grup 5'te dosya tabanlı tek yönlü paket seçildi: daha az güç, çok daha küçük güven sınırı | Muhasebeci paketi |
| Fiş okutma (AI OCR ile otomatik gider kaydı) | Uyarlayarak al | Bizde ADR 0011 zaten bir öneri katmanı tanımlıyor; fark, Paraşüt'ün "otomatik gider kaydına dönüştürme" iddiası — bizde model yönü ve ödeme kaynağını seçmez | Fiş okuma akışı |
| Tekrarlayan **fatura** ile tekrarlayan **giderin** ayrı akışlar olması | Henüz karar verme | Bizde tek bir `RecurringTransaction` var ve kayıt üretir, belge üretmez; ayrıştırmanın bize değer katıp katmayacağı e-belge kararına bağlı | Planlama / tekrarlayan |

## Kanıt ve güven düzeyi

- **Manuel gözlem (1 Eyl 2026):** yalnız giriş öncesi yüzey — 4 noktalı
  karusel ve `Giriş yap` ekranı. Kareler `01-ilk-acilis-carousel4.png`
  (slayt 4), `01b-carousel.png` (slayt 1)
- **Resmî kaynak (kullanım kılavuzu):** parasut.com — "Paraşüt nedir",
  gider türleri, cari hesap bakiyesi takibi, tahsilat ekleme (2 ve 9 Eyl
  2026); **KDV Raporları** sayfası (12 Eyl 2026)
- **Resmî kaynak (tanıtım videosu):** `02-video-fatura-gonderme.png` …
  `08-video-nakit-akisi-raporu.png` — "Paraşüt ile neler yapabilirsiniz?"
  videosunun kareleri. **Animasyonlu pazarlama kurgusu**; ekran ve alan
  adları gözlem, sayılar ve hesaplama davranışı değil (yukarıdaki "Sayılar
  tutmuyor" bölümü)
- **Çıkarım:** patronun şahsi harcamasının ortak/personel carisine
  düşeceği — kılavuzda böyle bir kullanım anlatılmıyor, cari modelinden
  türetildi
- **Çıkarım:** `06-video-stok-depo.png`'daki kırmızı satırın kritik stok anlamına geldiği;
  `07-video-gelir-gider-raporu.png`'deki `Vergiler dahil` açılırının bir `hariç` seçeneği olduğu; `03-video-satis-faturasi-detay.png`'teki
  `CARİSİZ` düğmesinin carisiz fatura anlamı; `04-video-cari-hesap-durumu.png`'teki `PLANLANMIŞ` kovasının
  içeriği; `07-video-gelir-gider-raporu.png` ile `08-video-nakit-akisi-raporu.png`'in farklı zaman eksenlerinde saydığı
- **14 Eyl 2026 görsel incelemesi:** 9/9 kare açıldı; `02-video-fatura-gonderme.png` ve `03-video-satis-faturasi-detay.png` iki ayrı
  örnek faturadır. Ayrıntı: `KANIT-ENVANTERI.md` P1-parasut-G01
- **Doğrulanamadı:** ürünün **hiçbir canlı davranışı** ve gerçek görsel
  tasarımı (giriş yapılamadı); mobil arayüzün giriş sonrası tamamı;
  karusel slayt 2 ve 3; güncel fiyatlandırma (form daha önce "~150 TRY/ay"
  yazıyordu, bu tur doğrulanmadığı için çıkarıldı)
- **Kasten yapılmadı:** derin inceleme. Aynı kulvarda KolayBi seçildi;
  36 dk'lık "Uçtan Uca Program Eğitimi" videosuna başvurulmadı

## Tek cümlelik sonuç

Paraşüt, muhasebe bilgisi gerektirmeden çalışan geniş kapsamlı bir bulut ön
muhasebe + e-dönüşüm ürünü: kayıt ile ödemeyi ayrı adımlarda tutması ve nakit
akışı ile gelir-gideri ayrı raporlarda göstermesi bizimkiyle aynı yönde,
KDV'yi ay bazında hesaplayıp Net KDV'ye kadar götürmesi ise ADR 0016'dan
ayrıldığımız net nokta — ama bu formun tamamı masa başı kaynaklara dayanıyor
ve uygulamanın hiçbir davranışı doğrulanmadı.

---

## Faz 8 — hedefli kaynak taraması (masa başı, 22 Eylül 2026)

Belge 2 Bölüm 9 için açık kalan sorular tarandı. Hepsi `Resmî kaynak`; ürünün
içi yine görülmedi.

### KDV raporunun ekranı — kolonlar artık biliniyor

`parasut.com/kullanim-kilavuzu/kdv-raporlari` iki ekran görüntüsü taşıyor:

- **Aylara Göre KDV Raporları** — kolonlar `AY` · `HESAPLANAN KDV` ·
  `İNDİRİLECEK KDV` · `NET KDV`. İndirilecek KDV negatif işaretli; **Net KDV
  negatife düşebiliyor** (örnek: Ağustos 152,54 ₺ / −1.270,00 ₺ / −1.117,46 ₺).
  Altta `DAHA FAZLA GÖSTER`.
- **Ay Dökümü** — kolonlar `İŞLEM TÜRÜ` · `FATURA #` · `AÇIKLAMA` ·
  `MÜŞTERİ/TEDARİKÇİ` · `DÜZENLEME TARİHİ` · `KDV`. İşlem türleri Satış
  Faturası, Alış Faturası, Harcama.

Tablonun sağ üstünde `Tümü / Satışlar / Giderler`; dışa aktarım Excel.

`çıkarım`: `Net KDV = Hesaplanan − İndirilecek` olarak sunuluyor ve devreden
KDV durumu ekranda gizlenmiyor. Dayanağı: iki ekranın ilişkisi ve negatif
örnek satır.

### KDV orandan türetiliyor

Kılavuz: *"KDV raporları, **girdiğiniz gelir ve gider kalemlerindeki KDV
tutarları yoluyla** ay bazında size hesaplanan, indirilecek ve net KDV'yi
gösterir."*

Kalem tarafı: *"Hizmet/ürün kalemi bazında **vergi oranı değişikliği**
yapabilir, **birim fiyat girip KDV dahil toplamın hesaplanmasını veya KDV
dahil toplam girip birim fiyatın hesaplanmasını** sağlayabilirsiniz."*
(`…/detayli-fis-fatura-olusturmak`). Ürün kartında saklananlar arasında
*"KDV oranı"* var (`…/hizmet-veya-urun-eklemek`). İstisna faturasında
*"KDV oranını '0 / %0' olarak belirleyin"* (`parasut.com/blog/istisna-faturasi-nedir`).

`çıkarım`: Kullanıcı **oranı** seçiyor, tutarı uygulama hesaplıyor. Kalem
düzeyinde elle KDV tutarı girme yolu Paraşüt'ün kendi kaynaklarında geçmiyor.

**Açık kalan:** KDV oranı listesinin kapalı bir liste mi serbest giriş mi
olduğu hiçbir resmî sayfada tanımlı değil. Kalem düzeyinde tutar girişinin
mümkün olup olmadığı ne doğrulanıyor ne reddediliyor — tek ipucu, e-fatura
içeri alınırken Toplam KDV uyuşmazlığının **hata değil uyarı** sayılması
(resmî duyuru akışı).

### Stok ücretsiz ve çoklu depolu

*"Paraşüt abonesi iseniz **stok ve depo takibi hizmeti almak için ekstra bir
ücret ödemeniz gerekmez**"* (`parasut.com/stok-ve-depo-takip-programi`).
Hesap ilk açıldığında `Stok Takibi` **otomatik devrede**
(`…/kullanim-kilavuzu/stok-takibi-yapmak`). Varsayılan Depo, Giriş/Çıkış
Deposu, **Depolar Arası Transfer** (transfer fişi/irsaliye çıktısı), depo
bazında **kritik stok seviyesi**. `Stoktaki ürünler raporu` stok maliyeti,
satış değeri ve potansiyel kazancı veriyor.

### e-Belge kontörle fiyatlanıyor

*"Gönderilen **veya alınan** her e-fatura bir e-kontör değerindedir."* Ön
ödemeli paketler, **kullanım süresi 12 ay**; mali mühür + kart okuyucu ayrı
kalem; e-faturaya geçiş için bir kereye mahsus giriş bedeli ve hediye kontör
(`…/elektronik-fatura-hizmetinin-maliyeti`, `parasut.com/e-fatura`).
