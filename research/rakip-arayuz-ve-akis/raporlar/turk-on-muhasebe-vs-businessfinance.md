# Türk Ön Muhasebe Uygulamaları vs. BusinessFinance — Fark Notu

**Amaç:** Paraşüt, Logo İşbaşı ve KolayBi'nin içine girilemedi (kayıt web'de,
ücretli/deneme kapısı). Bu not, resmî kaynaklardan (web sitesi, yardım merkezi,
tanıtım videoları) derlenen özellik ve kapsam farklarını toparlar. Kanıt etiketleri
katmanlıdır *(P4-B19 düzeltmesi, 15 Eyl 2026: önceki metin “tümü Resmî kaynak” diyordu)*:
rakip sütunlarındaki özellik hücreleri **Resmî kaynak** (ürün sayfası, destek dokümanı,
tanıtım videosu); kayıt/giriş yüzeyleri **Manuel gözlem** (Paraşüt ve KolayBi giriş, Logo
kayıt kareleri); BusinessFinance sütunu **proje kodu ve ADR'ler**; karşılaştırma ve
“neden” cümleleri **Çıkarım**. Rakiplerde canlı ürün davranışı doğrulanmadı.

Kaynak tarih: 2 Eylül 2026 (ilk derleme). Bu not tarihsel bir özettir; 15 Eylül'de
fiyat, çek-senet, muhasebeci yetkisi ve etiket bakımından formlarla uzlaştırıldı. Güncel
dayanak her zaman `gozlemler/parasut.md`, `gozlemler/logo-isbasi.md`,
`gozlemler/kolaybi.md` ve `BULGU-DOGRULAMA-KAYDI.md`'dir.

---

## 1. Bunlar ne tür ürün

| | Paraşüt | Logo İşbaşı | KolayBi | **BusinessFinance** |
|---|---|---|---|---|
| Tür | Bulut ön muhasebe + e-dönüşüm | Bulut ön muhasebe + e-dönüşüm | Bulut ön muhasebe + e-dönüşüm | İşletme **finansı** takip uygulaması (muhasebe değil) |
| Ana istemci | Web (mobil tamamlayıcı) | Web + mobil | Web + mobil | **Mobil (Flutter) ana istemci**, .NET backend |
| Kayıt | Web, VKN/şirket bilgisi | Mobil form + SMS + "hesabınız hazırlanıyor" | Web (mobilde kayıt yok) | Uygulama içi, tek soru (`HasBusiness`) |
| Hedef kitle | KOBİ, freelancer, e-ticaret, üretim | Mikro işletme, esnaf (kayıt listesinde Kurye/Öğrenci seçeneği — konumlandırma gözlemi, kullanıcı dağılımı değil) | KOBİ, start-up, **şahıs şirketi** | Şahıs şirketi ve esnaf; patronun cebi ile kasası ayrılmayan kesim |
| Fiyat | Ücretli abonelik; e-Fatura kontör ayrı (tutar formda doğrulanmadı) | Ücretli abonelik, 3 paket (tutar formda doğrulanmadı) | Ücretli; PLUS paketi kampanyası (kısıtsız e-fatura kontör + 1 yıl e-imza + banka entegrasyonu) | — (kendi ürünümüz) |
| Erişim | Kayıt web'de, VKN ister | Kayıt sonrası "hesabınız hazırlanıyor" satış süreci | Kayıt yalnız web'de | Uygulama içi kayıt, anında kullanım |

---

## 2. Kurucu fark: işletme ve şahsi para

**Üç uygulamada da yok.** Hepsi klasik "firma defteri": tek bir işletmenin
alışını, satışını, cari hesabını tutar. Patronun şahsi harcamasının ancak
"personel/ortak cari" veya "çekilen para" gibi dolaylı yollarla gireceği
çıkarımdır; kaynaklar böyle bir kullanım anlatmıyor, kullanıcı davranışı
gözlenmedi *(P1-B08 düzeltmesi, 14 Eyl 2026)*.

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
| Kasa-banka / nakit | ✓ (kasa ve bankalar aynı listede) | ✓ (kasa-banka yönetimi; çoklu döviz formda doğrulanmadı) | ✓ | ✓ (bakiye kalıcı kolon değil, hareketlerden hesaplanır) |
| Kredi kartı borcu + ödemesi | Formda anılmıyor (doğrulanmadı) | Formda anılmıyor (doğrulanmadı) | ✓ ayrı Kredi Kartları sekmesi | ✓ (çift sayım önleyen model: charge gider yazar, payment yazmaz) |
| Transfer (hesaplar arası) | Dolaylı | Var | Var | ✓ (gelir/gidere 0 etki) |
| Tekrarlayan | Tekrarlayan **fatura** | Var | Tekrarlayan **işlem** | ✓ `RecurringTransaction` (tanım rapor üretmez, onayla realize) |
| Fatura / e-Fatura / e-Arşiv / e-İrsaliye / e-SMM | ✓ tümü | ✓ tümü | ✓ tümü + e-İmza | ✗ **kapsam dışı** — fatura yerine `Obligation`/`CounterpartyCharge` tanıma |
| Sesli komutla fatura | — | ✓ | — | ✗ |
| Fiş okuma (OCR) | ✓ AI OCR | ✓ Akıllı Fiş Okuma | ✓ | ✓ ama **öneri katmanı** (ADR 0011): yönü ve ödeme kaynağını seçmez |
| Stok / depo / sipariş | ✓ çok depolu | ✓ | ✓ | ✗ **kapsam dışı** (ERP değiliz) |
| Banka entegrasyonu | ✓ (`BANKA HESABI BAĞLA`; karede 12 banka logosu) | ✓ (Banka Hesap Hareketleri entegrasyonu; banka sayısı formda yok) | ✓ (25+ banka) | ✗ **kesin kapsam dışı** (açık bankacılık yasak) |
| Online / sanal POS tahsilat | ✓ | ✓ İşbaşı POS | ✓ Sanal POS | ✗ ödeme başlatma yok; `PosSettlement` = fiziksel POS gün sonu |
| e-Ticaret entegrasyonu | ✓ Trendyol/N11/Shopify… | ✓ | ✓ | ✗ |
| Çek-senet | ✓ (tahsilat/ödeme: çek-senet takibi) | ✓ (çek giriş/çıkış) | ✓ Finans modülünde çek ve senet ekranları (`d22-destek-cekler.png`, `d23-destek-senetler.png`) *(P4-B19: önceki hücre “—” idi)* | ✗ |
| KDV raporu | ✓ | ✓ (KDV tahakkuk) | ✓ | KDV **taşınır, hesaplanmaz** (ADR 0016); muhasebeci paketi çıktısı |
| Çoklu döviz | ✓ (hesap listesinde döviz cinsi sütunu) | Formda anılmıyor (doğrulanmadı) | ✓ | ✗ tek para birimi (TRY) |
| Nakit akışı projeksiyonu | Nakit akışı raporu geçmiş tahsilat/ödemeleri gösteriyor; 12 aylık projeksiyon formda doğrulanmadı | Özet raporlar (projeksiyon formda yok) | ✓ Tahmini Dönem Sonu Bakiyesi | Planlanan görünüm (7/30/90 gün) |
| Muhasebeci erişimi | **Canlı erişim** (anlık görüntüleme) | **Müşavir Portal** (canlı; müşteri adına işlem) | Çok müşterili canlı erişim | **Dosya tabanlı tek yönlü paket** (`accountant-package.zip`) |
| Vergi/SGK takvimi | Kısmi (ödeme takibi) | — | — | ✓ öneri döner, hiçbir şey yazmaz (ADR 0016) |
| İşletme/şahsi kapsam boyutu | ✗ | ✗ | ✗ (proje boyutu var) | ✓ **kurucu fark** |

---

## 4. Muhasebeci tarafı — biçim farkı

Üç rakip de kaynaklarına göre muhasebeciye **canlı erişim** veriyor (`Resmî kaynak`):
Paraşüt'te muhasebeci hesaba erişip veriyi anlık görüyor; KolayBi'de çok müşterili
muhasebeci erişimi var. **Müşteri adına işlem yetkisi yalnız Logo için kaynakta
yazıyor:** müşteri müşaviri ekledikten sonra müşavir müşteri adına işlem yapabiliyor
(ürün sayfası). `06-video-musavir-portal.png` karesindeki satırlar boş şablondur; kalem/çöp
ikonlarının “düzenle/sil” anlamı ve listenin kimin verisi olduğu `çıkarım`dır. Paraşüt ve
KolayBi'de muhasebecinin düzenleme veya silme yetkisi doğrulanmadı. *(P4-B19 düzeltmesi,
15 Eyl 2026: önceki metin Logo karesinden üç ürüne “görür, düzenler, siler” yetkisini
genelliyor ve görselin bunu “net gösterdiğini” söylüyordu.)*

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
- **Kıyas değeri olan yerler** (o tarihte Tur 2'ye bırakılmıştı; KolayBi Faz 6'da destek
  merkeziyle derinleştirildi, Paraşüt ve Logo masa başı sınırında kaldı — güncel ayrıntı
  formlardadır): fiş okutma akışı, tekrarlayan (fatura vs.
  işlem) modeli, nakit akışı projeksiyonu ekranı, cari hesap ekstresi/mutabakat,
  muhasebeci aktarımı.
- **Kopyalanmayacak yerler**: e-belge, stok/sipariş, banka bağlantısı, çoklu
  döviz, ödeme başlatma — proje kapsamı dışı, karar filtresi bunları `Alma`
  diyor.
