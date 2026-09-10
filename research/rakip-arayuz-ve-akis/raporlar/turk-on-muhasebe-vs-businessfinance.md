# Türk Ön Muhasebe Uygulamaları vs. BusinessFinance — Fark Notu

**Amaç:** Paraşüt, Logo İşbaşı ve KolayBi'nin içine girilemedi (kayıt web'de,
ücretli/deneme kapısı). Bu not, resmî kaynaklardan (web sitesi, yardım merkezi,
tanıtım videoları) derlenen özellik ve kapsam farklarını toparlar. Kanıt etiketi:
tümü **Resmî kaynak** — canlı ürün davranışı doğrulanmadı.

Kaynak tarih: 2 Eylül 2026. Ayrıntı: `gozlemler/parasut.md`, `gozlemler/logo-isbasi.md`,
`gozlemler/kolaybi.md`.

---

## 1. Bunlar ne tür ürün

| | Paraşüt | Logo İşbaşı | KolayBi | **BusinessFinance** |
|---|---|---|---|---|
| Tür | Bulut ön muhasebe + e-dönüşüm | Bulut ön muhasebe + e-dönüşüm | Bulut ön muhasebe + e-dönüşüm | İşletme **finansı** takip uygulaması (muhasebe değil) |
| Ana istemci | Web (mobil tamamlayıcı) | Web + mobil | Web + mobil | **Mobil (Flutter) ana istemci**, .NET backend |
| Kayıt | Web, VKN/şirket bilgisi | Mobil form + SMS + "hesabınız hazırlanıyor" | Web (mobilde kayıt yok) | Uygulama içi, tek soru (`HasBusiness`) |
| Hedef kitle | KOBİ, freelancer, e-ticaret, üretim | Mikro işletme, kurye, öğrenci, esnaf | KOBİ, start-up, **şahıs şirketi** | Şahıs şirketi ve esnaf; patronun cebi ile kasası ayrılmayan kesim |
| Fiyat | ~150 TL/ay+, e-fatura kontör ayrı | 463 TL/ay+KDV'den, 3 paket | PLUS paket + kampanya | — (kendi ürünümüz) |
| Erişim | Kayıt web'de, VKN ister | Kayıt sonrası "hesabınız hazırlanıyor" satış süreci | Kayıt yalnız web'de | Uygulama içi kayıt, anında kullanım |

---

## 2. Kurucu fark: işletme ve şahsi para

**Üç uygulamada da yok.** Hepsi klasik "firma defteri": tek bir işletmenin
alışını, satışını, cari hesabını tutar. Patronun şahsi harcaması ancak
"personel/ortak cari" veya "çekilen para" gibi dolaylı yollarla girer.

**BusinessFinance kurucu kararı (ADR 0013):** işletme ve şahsi para **tek
havuzda** yaşar; ayrım bir **raporlama boyutudur** (`TransactionScope` =
Business/Personal). Bakiye, kart borcu ve net varlık kapsam filtresinden
etkilenmez; yalnız gelir/gider raporu ikiye bölünür. Bu, üç rakibin hiçbirinde
karşılığı olmayan asıl farklılaşma noktası.

- **KolayBi'de** "proje bazlı gelir-gider" var — ikinci bir raporlama boyutu
  fikri, ama eksen farklı (proje ≠ işletme/şahsi). "Birden çok boyut" talebinin
  kanıtı, kopyalanacak çözüm değil.

---

## 3. Özellik karşılaştırması

| Özellik | Paraşüt | Logo İşbaşı | KolayBi | **BusinessFinance** |
|---|---|---|---|---|
| Gelir-gider takibi | ✓ | ✓ | ✓ | ✓ (çekirdek) |
| Cari hesap (müşteri/tedarikçi) | ✓ | ✓ | ✓ | ✓ (ADR 0014: tanır/taşır ayrımı) |
| Kasa-banka / nakit | ✓ | ✓ (çoklu döviz) | ✓ | ✓ (bakiye kalıcı kolon değil, hareketlerden hesaplanır) |
| Kredi kartı borcu + ödemesi | Çek-senet ağırlıklı | Var | Var | ✓ (çift sayım önleyen model: charge gider yazar, payment yazmaz) |
| Transfer (hesaplar arası) | Dolaylı | Var | Var | ✓ (gelir/gidere 0 etki) |
| Tekrarlayan | Tekrarlayan **fatura** | Var | Tekrarlayan **işlem** | ✓ `RecurringTransaction` (tanım rapor üretmez, onayla realize) |
| Fatura / e-Fatura / e-Arşiv / e-İrsaliye / e-SMM | ✓ tümü | ✓ tümü | ✓ tümü + e-İmza | ✗ **kapsam dışı** — fatura yerine `Obligation`/`CounterpartyCharge` tanıma |
| Sesli komutla fatura | — | ✓ | — | ✗ |
| Fiş okuma (OCR) | ✓ AI OCR | ✓ Akıllı Fiş Okuma | ✓ | ✓ ama **öneri katmanı** (ADR 0011): yönü ve ödeme kaynağını seçmez |
| Stok / depo / sipariş | ✓ çok depolu | ✓ | ✓ | ✗ **kapsam dışı** (ERP değiliz) |
| Banka entegrasyonu | ✓ | ✓ (17 banka) | ✓ (25+ banka) | ✗ **kesin kapsam dışı** (açık bankacılık yasak) |
| Online / sanal POS tahsilat | ✓ | ✓ İşbaşı POS | ✓ Sanal POS | ✗ ödeme başlatma yok; `PosSettlement` = fiziksel POS gün sonu |
| e-Ticaret entegrasyonu | ✓ Trendyol/N11/Shopify… | ✓ | ✓ | ✗ |
| Çek-senet | ✓ | ✓ | — | ✗ |
| KDV raporu | ✓ | ✓ (KDV tahakkuk) | ✓ | KDV **taşınır, hesaplanmaz** (ADR 0016); muhasebeci paketi çıktısı |
| Çoklu döviz | ✓ | ✓ | ✓ | ✗ tek para birimi (TRY) |
| Nakit akışı projeksiyonu | ✓ 12 ay | Özet grafik | ✓ | Planlanan görünüm (7/30/90 gün) |
| Muhasebeci erişimi | **Canlı portal** (anlık) | **Müşavir Portal** (canlı) + Excel | Çok kullanıcılı canlı erişim | **Dosya tabanlı tek yönlü paket** (`accountant-package.zip`) |
| Vergi/SGK takvimi | Kısmi (ödeme takibi) | — | — | ✓ öneri döner, hiçbir şey yazmaz (ADR 0016) |
| İşletme/şahsi kapsam boyutu | ✗ | ✗ | ✗ (proje boyutu var) | ✓ **kurucu fark** |

---

## 4. Muhasebeci tarafı — biçim farkı

Üç rakip de muhasebeciye **canlı erişim** veriyor: muhasebeci kendi ekranından
müşterinin fatura/fiş satırlarını görür, düzenler, siler (Logo'nun "Müşavir
Portal" görseli bunu net gösteriyor; `kanitlar/logo-isbasi/06`).

**BusinessFinance farkı bilinçli:** muhasebeciye veri, o ayın **işletme
raporunun okuması** olan bir `.zip` paketiyle gider (Aşama 05 Grup 5). İkinci
bir hesaplama yolu değil; canlı çok-taraflı erişim sahiplik izolasyonunu ve
"tek hesaplama yolu" kuralını zorlar. İhtiyaç doğrulandı, çözüm biçimi dar
tutuldu.

---

## 5. Sonuç — nerede duruyoruz

- Üç rakip **aynı kategoride**: geniş kapsamlı ön muhasebe + e-dönüşüm + ERP'ye
  yakın modüller (stok, sipariş, e-ticaret, banka). Fiyatlı, web öncelikli,
  muhasebe okuryazarlığı olan veya muhasebecisiyle çalışan işletme varsayıyorlar.
- **BusinessFinance daha dar ve daha derin bir tez**: tek kişilik işletmede
  işletme parası ile şahsi para hukuken ayrılmadığı için ikisini **tek üründe,
  tek havuzda** tutmak; ayrımı rapora bırakmak. Fatura kesmiyoruz, stok
  tutmuyoruz, bankaya bağlanmıyoruz, vergi hesaplamıyoruz.
- **Kıyas değeri olan yerler** (Tur 2'de resmî kaynak + kullanıcının video
  notlarıyla derinleştirilecek): fiş okutma akışı, tekrarlayan (fatura vs.
  işlem) modeli, nakit akışı projeksiyonu ekranı, cari hesap ekstresi/mutabakat,
  muhasebeci aktarımı.
- **Kopyalanmayacak yerler**: e-belge, stok/sipariş, banka bağlantısı, çoklu
  döviz, ödeme başlatma — proje kapsamı dışı, karar filtresi bunları `Alma`
  diyor.
