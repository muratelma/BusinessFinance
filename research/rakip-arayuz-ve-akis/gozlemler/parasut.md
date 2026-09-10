# Uygulama Gözlem Formu — Paraşüt

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Paraşüt / Paraşüt Yazılım (Mikrogrup) |
| Sürüm | 5.25.0 |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | Türkçe |
| Hesap veya plan türü | — |
| Erişim kısıtı | **Kayıt engeli** — mobil uygulamada kayıt yok, yalnız "Giriş yap" / "Parolanızı mı unuttunuz?". Kayıt yalnız web'de (parasut.com), şirket/VKN bilgisi gerektirir, ücretli |
| İnceleme türü | **Resmî kaynak** (yalnız giriş karuseli manuel gözlem + "Paraşüt ile neler yapabilirsiniz?" tanıtım videosu, 10 Eyl 2026) |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | Muhasebe bilgisi gerektirmeden **fatura kesen, cari takip eden, e-dönüşümü yöneten** web tabanlı bulut ön muhasebe programı |
| Asıl hedef kullanıcı | KOBİ, mikro işletme, serbest çalışan, **e-ticaret satıcısı**, üretim/mağaza işletmesi. "İşletme" tarafı — patronun şahsi bütçesi hedefte değil |
| Çözdüğü ana iş | Satış/alış faturası + e-Fatura/e-Arşiv + cari hesap + tahsilat-ödeme takibi + nakit akışı |
| Açıkça kapsam dışı bıraktığı | Bordro (maaş kaydı var ama SGK/bordro hesabı yok), yatırım takibi, **kişisel/gündelik bütçe** |
| İş modeli | Ücretli abonelik (~150 TRY/ay+, e-Fatura kontör ayrı) |
| BusinessFinance ile aynı kulvarda mı | **Kısmen.** Gelir-gider + cari + fiş okutma + tekrarlayan ortak; ama Paraşüt bir **fatura/e-belge + firma defteri** ürünü, işletme/şahsi tek havuz kavramı yok. Klasik "şirket muhasebesi" tarafında |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi (giriş öncesi) | 4 slaytlık tanıtım karuseli (fatura oluşturma/paylaşma vurgusu) + tek CTA "Giriş yap". Mobilde kayıt akışı yok (`kanitlar/parasut/01`–`01c`). Ürün turu tanıtım videosundan yeniden kuruldu (`02`–`08`, bkz. aşağı) |
| K01–K08 | **Girilemedi → resmî kaynak** | Mobilde kayıt yok, web kaydı ücretli ve VKN istiyor. Akışlar aşağıdaki "Sistem işleyişi / pipeline" bölümünden |

## Resmî kaynak — özellikler ve kapsam (parasut.com, 2 Eyl 2026)

Aşağıdakiler `Resmî kaynak` etiketlidir; emülatörde doğrulanmadı.

### Ne bu ürün, kimin için

- **Web tabanlı bulut ön muhasebe programı.** Kurulum/güncelleme yok.
- Hedef kitle: **KOBİ, mikro işletme, serbest çalışan (freelancer), e-ticaret
  satıcısı, girişimci.** "Muhasebe bilgisi gerektirmeden dijital finans yönetmek
  isteyenler" için konumlanmış — bizim segmentimizle (şahıs şirketi, esnaf)
  büyük ölçüde çakışıyor, ama Paraşüt e-ticaret ve üretim/mağaza işletmelerine
  de açıkça hitap ediyor.
- İddia: sınırsız kullanıcı / müşteri / fatura / veri.

### Özellikler

Not: "kapsam dışı" ibaresi bir **öneri kararıdır** (Belge 3), gözlem değil.
Aşağıdaki her özellik Belge 1/2'de anlatılır; patron kapsamı genişletebilir.

| Grup | Özellik | BusinessFinance açısından (Belge 3 ön değerlendirmesi) |
|---|---|---|
| Finans | Gelir-gider takibi, nakit akışı analizi ve görselleştirme, cari hesap (müşteri+tedarikçi) takibi | Cari bizde ADR 0014 ile var; nakit akışı görseli bizim özet ekranıyla örtüşür |
| Fatura/e-belge | Fatura, teklif, e-Fatura, e-Arşiv, e-İrsaliye, e-SMM, e-İhracat faturası | **Kapsam dışı** — biz e-belge kesmiyoruz; `Obligation`/`CounterpartyCharge` tanıma modeli var |
| Tahsilat/ödeme | Çek-senet takibi, tahsilat hatırlatmaları, ödeme uyarıları, alacak/borç izleme | Tahsilat/ödeme bizde var; çek-senet ve hatırlatma yok (upcoming-payments kısmi karşılık) |
| Stok/lojistik | Çok depolu stok takibi, depolar arası transfer, sevkiyat takibi | **Kapsam dışı** — ERP değiliz |
| **Tekrarlayan fatura** | Otomatik tekrarlayan fatura oluşturma | Bizde `RecurringTransaction` var ama fatura değil, kayıt üretir |
| **Fiş okutma** | AI OCR ile fiş fotoğrafından otomatik işleme | Bizde ADR 0011: öneri katmanı, yönü/ödeme kaynağını seçmez |
| Entegrasyon | Banka entegrasyonu, e-ticaret (Trendyol/N11/Shopify/Hepsiburada), online tahsilat, CRM, saha ekibi | Banka bağlantısı ve ödeme başlatma bizde **kesin kapsam dışı** |
| **Muhasebeci erişimi** | Muhasebeci hesaba erişip **veriyi anlık görüyor**; dosya/yedek paylaşımı yok, eş zamanlı çalışma | Logo Müşavir Portal ile aynı model. **BusinessFinance farkı:** bizde dosya tabanlı tek yönlü dışa aktarma paketi (Aşama 05 Grup 5); canlı çok-taraflı erişim sahiplik izolasyonunu zorlar |
| Mobil | iOS/Android'de fatura kesme, gider, tahsilat, fiş okutma — tam özellikli | Bizde mobil ana istemci |

### Fiyat

- Başlangıç ~150 TRY/ay; e-Fatura kontör ayrı. **Ücretli ürün.**
- Kayıt yalnız web'de ve şirket/VKN bilgisi istiyor; bu çalışmada uygulamaya
  girilemedi, inceleme tümüyle resmî kaynak.

## Sistem işleyişi / pipeline

`Resmî kaynak` — parasut.com kullanım kılavuzu (9 Eyl 2026). Canlı ürün
davranışı doğrulanmadı.

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Gider kaydı türleri | **Beş tür:** Detaylı Fiş/Fatura (kalem + stopaj/tevkifat), Hızlı Fiş/Fatura (yalnız toplam tutar), Maaş/Prim (otomatik tekrarlama), Vergi/SGK Primi, Banka Gideri. Tür seçimi kaydın davranışını belirliyor | Resmî kaynak |
| Kayıt ile ödeme ayrı adım | **Önce gider oluşturulur, sonra ödeme eklenir.** Ödeme kasa/banka bakiyesini gider tutarı kadar azaltır; gider kaydının kendisi azaltmaz. Kısmi ödeme destekleniyor. Tedarikçi eşleştirmesi isteğe bağlı | Resmî kaynak |
| Cari borçlandırma yolları | Üçü de: (1) müşteri kaydında açılış bakiyesi, (2) satış faturası, (3) borç yokken ödeme ekleme = avans. Tedarikçi için simetrik | Resmî kaynak |
| Cari bakiye hesabı | Otomatik: fatura arttıkça borç artar, tahsilat yapıldıkça azalır. **Mahsuplaştırma otomatik** — para hareketi en çok gecikmiş açık faturadan başlayarak eşleşir | Resmî kaynak |
| Tahsilat akışı | Tahsilat Ekle → banka hesabı + tutar (çoklu döviz) → cari bakiye kapanır + banka bakiyesi artar. Vade gelince **otomatik hatırlatma e-postası** | Resmî kaynak |
| Fiş okutma | Mobil kamera → fiş fotoğrafı doğrudan yükleme → AI OCR ile gider kaydına dönüşüm; "işlemleri bekletmeden" | Resmî kaynak |
| Tekrarlayan | Tekrarlayan gider ve tekrarlayan fatura ayrı akışlar; abonelik faturalaması için otomatik oluşturma | Resmî kaynak |
| Dashboard | Hesap bakiyeleri, yaklaşan vadeler, **12 aylık nakit akışı projeksiyonu**, KDV ve yaşlandırma raporları, Excel dışa aktarma | Resmî kaynak |
| İşletme/şahsi ayrım | **Yok.** "Ortak/patron cebinden" gider ancak **personel/ortak cari bakiyesi** üzerinden dolaylı girer — yani şahsi harcama bir alacak-borç kalemi olur | Resmî kaynak + Yorum |
| Entegrasyon temas noktaları | Banka entegrasyonu (otomatik ödeme/mutabakat), e-ticaret pazaryerleri, online tahsilat, GİB e-belge, muhasebeci **canlı erişim** | Resmî kaynak |
| Veri nereye yazılıyor | Bulut (web ana istemci); mobil tamamlayıcı, gerçek zamanlı senkron | Resmî kaynak |

**Pipeline şeması (satış → tahsilat):**
`müşteri seç → satış faturası (kalem/tutar, e-Fatura/e-Arşiv varyantı) → kaydet →
cari borç +tutar → (vade) otomatik hatırlatma → Tahsilat Ekle (banka + tutar) →
otomatik mahsup (en gecikmiş faturadan) → cari bakiye kapanır + banka +tutar →
nakit akışı ve yaşlandırma raporu güncellenir`

**Pipeline şeması (gider):**
`gider türü seç (5) → tutar/kalem (+ tedarikçi ops.) → fiş fotoğrafı (OCR) →
kaydet → gider raporu +tutar, cari borç +tutar → Ödeme Ekle → kasa/banka −tutar,
cari kapanır`

## Video/doküman akış yeniden kurulumu

**Kaynak:** "Paraşüt ile neler yapabilirsiniz?" tanıtım videosu (~2–3 dk,
animasyonlu), kullanıcı 10 Eyl 2026'da izledi, kareleri aktardı. Etiket:
`Resmî kaynak (tanıtım videosu)`. **Ekranlar animasyonlu pazarlama
kurgusudur** — gerçek ürün arayüzü birebir değil, akışın niyetini gösterir.
**Kareler web/masaüstü sürümüne ait** (pencere çubuğu — küçült/büyüt/kapat —
görünüyor); mobil yalnız `02`'de yan yana çıkıyor. Mobil arayüz bu çalışmada
hâlâ doğrulanmadı.

Kullanıcı notu (10 Eyl): 20 sn'lik ikinci promo videoda ("Paraşüt'ü şimdi
deneyin") ek bilgi yok. 36 dk'lık "Uçtan Uca Program Eğitimi" videosu web
sürümünü gerçek arayüzle gösteriyor — **Tur 2 kaynağı**, şu an gerekmiyor.

### Videonun anlattığı hikâye (kullanıcı özeti)

- Açılış problemi: tedarikçi carileri, müşteri faturaları, tahsilat/ödeme ve
  banka hareketlerini ayrı ayrı yönetmek iş yükü. Excel + cari defter + matbu
  fatura alışkanlığı ciro büyüdükçe yetmiyor, **e-Faturaya geçiş şart** oluyor.
- Çözüm kurgusu: matbu fatura maliyetinden kurtul → **e-kontör** al, kâğıt
  fatura masrafını bitir. Kullanıcı yorumu (tırnak içi kendi ifadesi):
  *"Sanırım fatura kesme maliyetini e-kontör yapısıyla ücretlendirmişler."*
- Bilgisayardan **veya** telefondan, müşteri bilgisiyle fatura gönder.
- Tüm cari hesaplar tek yerde; tedarikçiyle **anında paylaşılıyor**.
- Banka entegrasyonuyla tüm hesaplardaki gelir-gider her an kontrolde.
- Otomatik hatırlatmalar → tahsilat kaçmıyor, alacak günü gününe toplanıyor.
- Kritik stok uyarısı → tedarikçiye erken sipariş, satış kaçmıyor.
- İstediği zaman rapor → "aylar sonrasının durumu" görülüp planlı hareket.
- Mali müşavir Paraşüt hesabına eklenip her ay **saniyeler içinde** belge
  aktarımı. Veri bulutta.
- Kullanıcı genel izlenimi: uygulama **e-Fatura üstünde çok duruyor**; küçük
  işletmeden çok **orta/büyük işletmenin** iş yükünü hafifletmeye konumlu
  (küçük işletme de kullanabilir). **Kişisel bütçeyle ilgili hiçbir şey yok**;
  şahsi tarafa dokunan tek nokta olsa olsa banka hesabı bağlantısı.

### Ekran seviyesi akış (video kareleri)

| # | Kare | Ekranda görünen | BusinessFinance açısından |
|---|---|---|---|
| 02 | `02-video-fatura-gonderme.png` | e-Fatura/e-Arşiv belgesi mobil + masaüstünde; "Fatura Gönderildi!" onayı | Belge 2'de anlatılır. Bizde e-belge kesme / fatura gönderme yok; `CounterpartyCharge` tanıma modeli var. e-belge Belge 3'te `alma` (GİB entegrasyonu ayrı bir ürün) |
| 03 | `03-video-satis-faturasi-detay.png` | Satış Faturası detayı: kalem (Çimento ×2, birim 499 TL, **vergi %5**, **indirim %8**), Ara toplam 499 / Toplam KDV 24 / Genel toplam 523. Sağ panel: "FATURA GÖNDERİLDİ → Temel e-Fatura GÖNDERİLİYOR", alıcı e-postaları "Yolda", **PAYLAŞ**; "TAHSİL EDİLDİ 523,00 TL" (yeşil) / diğer karede "KALAN 523,00 TL — 7 gün sonra tahsil edilecek"; "Müşteri hatırlatma ekle", "Tahsilat talep et", **TAHSİLAT EKLE**; "İrsaliyeli Fatura", "Müşteri Ekranı Açık" | **Kayıt ≠ ödeme** burada görünür: fatura kesildi (gelir/cari borç yazıldı), tahsilat ayrı adım ("Tahsilat Ekle"). Bizde `CounterpartyCharge` tanır + `CounterpartyPayment` taşır ile aynı ayrım. **KDV Paraşüt'te faturadan hesaplanıyor** (%5 oran → 24 TL); ADR 0016'da biz KDV'yi **taşırız, hesaplamayız** — bu net fark. İndirim kalemi bizde yok |
| 04 | `04-video-cari-hesap-durumu.png` | "Cari Hesap Takibi > Güncel Durum": Tahsilatlar (Tahsil edilecek / Gecikmiş / Fatura yok, donut) ve Ödemeler (Ödenecek / Ödeme yok / **Planlanmış**). Sağda "BUGÜN – 22 EYLÜL", "4 GÜN GECİKTİ — Tahsilat 19.989,00 ₺" uyarı kartı | Bizim **upcoming-payments** + counterparty balance projeksiyonunun karşılığı. "Planlanmış" = tekrarlayan/ileri tarihli. "Gecikmiş" vurgusu bizde `attentionCode` ile paralel |
| 05 | `05-video-banka-entegrasyonu.png` | "Kasa ve Bankalar" listesi: hesap ismi / IBAN / döviz cinsi / bakiye. Butonlar: **BANKA HESABI BAĞLA**, KASA EKLE, BANKA EKLE. "Kasa Hesabı" (nakit) de aynı listede. Solda ~12 banka logosu (İş, Akbank, QNB, Enpara, TEB, VakıfBank, YapıKredi, Garanti, ING, DenizBank, Fibabanka, Albaraka) | Nakit + banka **aynı listede** — bizim `Account` modeliyle örtüşür. "Banka hesabı bağla" (otomatik hareket çekme) bizde **kesin kapsam dışı** (PROJECT-ROADMAP); bizde hesap elle eklenir, bakiye açılış + hareketlerden hesaplanır |
| 06 | `06-video-stok-depo.png` | "Depolar > Ana Depo (Varsayılan Depo)": ürün adı / stok miktarı / **Alış (vergiler hariç)** / **Satış (vergiler hariç)**. Kritik stoktaki satır kırmızı vurgulu | Belge 2'de anlatılır. Bizde şu an yok; alım/satım kaydına bağlı stok + kritik stok uyarısı Belge 3'te `henüz karar verme` adayı (patron kapsam kararı) |
| 07 | `07-video-gelir-gider-raporu.png` | "Gelir ve Gider Raporu": Filtrele + tarih aralığı + **"Vergiler dahil / hariç" toggle**. Gelirler ve Giderler kategori kırılımı + pasta grafik; "Kategorisiz" bir kategori; altta **NET**; DIŞARI AKTAR | Bizim aylık gelir-gider raporunun karşılığı. **"Vergiler dahil/hariç" toggle**: Paraşüt raporu KDV'yi ayırıp gösterebiliyor; bizde rapor tutarı **brüttür ve brüt kalır** (ADR 0016). "NET" kelimesi — bizde de "işletme neti", "kâr" değil |
| 08 | `08-video-nakit-akisi-raporu.png` | "Kasa / Banka Raporu": Toplam Nakit Girişi / Çıkışı / **Net Nakit Akışı**; aylık bar grafik (Nakit Girişi/Çıkışı, GÜN/HAFTA/AY/YIL); altında tahsilat-ödeme listesi (işlem türü / tarih / müşteri-tedarikçi-çalışan / açıklama / çıkış / giriş) | **Nakit akışı ≠ gelir-gider**: bu rapor parayı kasaya girdiği/çıktığı gün sayıyor, gelir-gider raporu tahakkuku sayıyor — Paraşüt ikisini ayrı rapor tutuyor. Bizde de "bu ayın neti" kasa değişimi değil (kart harcaması harcandığı gün gider). İyi kıyas: iki farklı zaman ekseni ayrı ekranda |

### Ekrandan ekrana pipeline (videodan)

`fatura oluştur (kalem + vergi oranı + indirim) → e-Fatura/e-Arşiv gönder
(alıcı e-posta "Yolda" → "Gönderildi") → cari borç +tutar → PAYLAŞ / Müşteri
Ekranı → (vade) otomatik hatırlatma → TAHSİLAT EKLE (banka + tutar) → "Tahsil
edildi" → Cari Hesap Takibi panosu + Kasa/Banka raporu + Gelir-Gider raporu
güncellenir → ay sonu: mali müşavire aktar`

### Karşılaştırma notu

Video, form'daki metin kaynaklı okumayı **doğruluyor ve keskinleştiriyor**:

- **İşletme/şahsi tek havuz kavramı Paraşüt'te yok** — video baştan sona firma
  defteri anlatıyor, kişisel bütçeye tek değinme yok. Bu, ADR 0013 farkımızın
  en net kanıtı.
- **Kayıt ≠ ödeme ayrımı** ekranda görünür ("Tahsil edildi" vs "Kalan …
  7 gün sonra"); bizim tanır/taşır modelimizle (ADR 0014) aynı yapı.
- **KDV farkı** somutlaştı: Paraşüt oranı faturaya uygulayıp KDV tutarını
  **hesaplıyor** ve raporda ayırabiliyor; biz KDV'yi taşırız, hesaplamayız,
  rapor brüttür (ADR 0016).
- **Nakit akışı ile gelir-gider ayrı raporlar** — bizim "bu ayın neti kasa
  değişimi değildir" ilkemizin Paraşüt'teki karşılığı.
- Kapsam olarak Paraşüt bizden **çok geniş** (e-belge, stok/depo, banka bağlama,
  müşteriyle canlı ekran). Ortak nokta: gelir-gider + cari + tekrarlayan +
  muhasebeci aktarımı + fiş okutma. Tur 2 kıyas noktaları: fatura→tahsilat
  akışı, cari mahsup, rapor dışa aktarma.

## Kanıt

- Manuel gözlem: `kanitlar/parasut/01-ilk-acilis-carousel4.png`, `01b-carousel.png`,
  `01c-carousel.png` — tanıtım karuseli + giriş ekranı (yalnız giriş öncesi)
- Resmî kaynak (tanıtım videosu): `kanitlar/parasut/02-video-fatura-gonderme.png`
  … `08-video-nakit-akisi-raporu.png` — "Paraşüt ile neler yapabilirsiniz?"
  videosunun kareleri (kullanıcı 10 Eyl 2026 izledi); animasyonlu pazarlama
  kurgusu, gerçek arayüz birebir değil
- Resmî kaynak: parasut.com ana sayfa + "Paraşüt nedir" kılavuzu (2 Eyl 2026);
  kullanım kılavuzu — farklı gider türleri, cari hesap bakiyesi takibi, mobilde
  gider takibi, tahsilat ekleme (9 Eyl 2026)
- Doğrulanamadı: uygulama içi ekranların **canlı davranışı ve gerçek görsel
  tasarımı** (giriş yapılamadı); işleyiş adımları yardım merkezi + tanıtım
  videosundan çıkarıldı. Video kareleri stilize animasyon
- Kullanıcıdan bekleniyor: (opsiyonel, Tur 2) 36 dk'lık "Uçtan Uca Program
  Eğitimi" videosundan gerçek web arayüzü kareleri — fatura→tahsilat akışı için

## Tek cümlelik sonuç

Paraşüt, muhasebe bilgisi gerektirmeden çalışan geniş kapsamlı bir bulut ön
muhasebe + e-dönüşüm ürünü; fiş okutma, tekrarlayan fatura ve canlı muhasebeci
erişimi bize kıyas noktası olur, ama işletme/şahsi tek havuz kavramı olmadığı
için asıl farkımızı gösteren bir "klasik firma defteri" örneği.
