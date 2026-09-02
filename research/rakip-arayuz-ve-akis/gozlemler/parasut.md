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
| Erişim kısıtı | **Kayıt engeli** — mobil uygulamada kayıt yok, yalnız "Giriş yap" / "Parolanızı mı unuttunuz?". Kayıt yalnız web'de (parasut.com), şirket/VKN bilgisi gerektirir |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi (giriş öncesi) | 4 slaytlık tanıtım karuseli (fatura oluşturma/paylaşma vurgusu) + tek CTA "Giriş yap". Mobilde kayıt akışı yok |
| K01–K08 | **Engelli** | Deneme hesabı olmadan giriş yapılamıyor; sahte VKN ile web kaydı yapılmayacak |

**Karar (1 Eyl 2026):** Kullanıcı atladı — mobilde kayıt yok, web girişi
ücretli ve uzun süreç. K01–K08 daha sonra `resmî kaynak` (parasut.com,
yardım merkezi, ürün videoları) ile masa başında incelenecek; bulgular
`Resmî kaynak` etiketi taşıyacak. Manuel emülatör testi yapılmayacak.

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

| Grup | Özellik | BusinessFinance açısından |
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

### Fiyat / deneme

- **14 gün ücretsiz deneme, kart/taahhüt istemiyor** (mobilde kayıt yok, web'den).
- Başlangıç ~150 TRY/ay; e-Fatura kontör ayrı. Yani **ücretli**.

### İşleyiş (yardım merkezi, doğrulanamadı)

- **Satış faturası:** müşteri seç → kalem/tutar → e-Fatura/e-Arşiv varyantı →
  kaydet/yazdır. Tekrarlayan fatura abonelik faturalaması için ayrı akış.
- **Gider:** hızlı giriş veya fiş tabanlı; kategoriler işletme gideri odaklı
  (kira, fatura, personel maaş/avans, banka masrafı, vergi/SGK). "Ortak/patron
  cebinden" gider **personel bakiyesi** üzerinden — yani şahsi harcama dolaylı.
- **Tahsilat/ödeme:** vade hatırlatması, çek-senet, banka entegrasyonuyla
  otomatik ödeme. Cari ekstre + mutabakat.
- **Dashboard:** hesap bakiyeleri, yaklaşan vadeler, 12 aylık nakit akışı
  projeksiyonu, KDV ve yaşlandırma raporları, Excel dışa aktarma.

### Karşılaştırma notu

Paraşüt kapsam olarak bizden **çok geniş** (e-belge, stok, çek-senet, e-ticaret,
banka). Ortak nokta: gelir-gider + cari + fiş okutma + muhasebeci paylaşımı +
tekrarlayan. **İşletme/şahsi tek havuz** kavramı Paraşüt'te yok — klasik firma
defteri. Bizim farkımız yine kapsam boyutu ve patronun gündelik hayatı ile
işletmesini tek üründe tutmak. Paraşüt'ün fiş okutma ve tekrarlayan fatura
akışları Tur 2 için iyi kıyas — deneme hesabı açılırsa incelenir.

## Kanıt

- `kanitlar/parasut/01-ilk-acilis-carousel4.png` — tanıtım karuseli + giriş ekranı
- Resmî kaynak: parasut.com ana sayfa + "Paraşüt nedir" kılavuzu (2 Eyl 2026)
- Doğrulanamadı: uygulama içi tüm ekranlar ve davranış (giriş yapılamadı)

## Tek cümlelik sonuç

Paraşüt, muhasebe bilgisi gerektirmeden çalışan geniş kapsamlı bir bulut ön
muhasebe + e-dönüşüm ürünü; fiş okutma, tekrarlayan fatura ve canlı muhasebeci
erişimi bize kıyas noktası olur, ama işletme/şahsi tek havuz kavramı olmadığı
için asıl farkımızı gösteren bir "klasik firma defteri" örneği.
