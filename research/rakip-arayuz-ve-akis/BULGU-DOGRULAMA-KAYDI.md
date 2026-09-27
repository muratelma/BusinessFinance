# Bulgu ve doğrulama kaydı

**20 Eylül güncel ek:** Belge 2 K1/K2/K3 ve S1 tamamlandı. MM-Q03'ün görünür yer ve
GB-Q01'in From New Income / Fill Each Envelope sonuç soruları kapandı; üretim eşiği,
fiziksel saklama, eski fonlama seçimi ve GB hesap değişmeme nedeni açık sınırdır.
Dizin 387 kare / 422 kimliktir. Ayrıntı dosyanın sonundaki 20 Eylül kapanışındadır.
Aşağıdaki 15 Eylül ve önceki durum blokları tarihseldir.

**15 Eylül güncel durum:** P4-K kapandı; P2, P3, WL-U ve BC-U01/HD-U01 kullanıcı kontrolleri işlendi. Toplam **365/365** görsel.
B01–B19: 14 kanıtla kapalı, 5 kapsamı sınırlı (B06, B08, B10, B11, B12), 0 açık.
P4-tema-01–10 ve P4-K kapandı; geçiş kapısı 11/12, kullanıcı onayı açık. Faz 8 açılmadı.
Sıradaki tek iş: kullanıcının Faz 8 onayı; ardından P5-I, sonra P5-P pilot.
Güncel ayrıntı bu dosyanın sonundadır; aşağıdaki eski devirler tarihseldir.

## Güncel Goodbudget devam noktası — 14 Eylül 2026

G01'in 28/28 incelemesi tekrar edilmedi; form düzeltmeleri yapıldı. B06 kapsam
sınırıyla, B07 kaynakla kapandı. B05 ve GB-U01 kullanıcı ekran kanıtıyla kapandı; Goodbudget tamam. Eski paket sonu devirleri tarihseldir; güncel sonuç bu dosyanın
sonundaki Goodbudget kapanış bölümüdür. Diğer uygulamaların açık işleri korunur.

## P0.2 — Başlangıç iskeleti, 13 Eylül 2026

**P0.2 aktarımı tamamlandı; B01–B19'un tamamı açık.** Bu paket kaynak denetimini
okuma, mevcut E kimliklerine bağlama ve kapanış işini tarif etme paketidir.
Görseller açılmadı, kod ve dış kaynaklar yeniden doğrulanmadı, canlı test
çalıştırılmadı; bu kayıtta anlatılan dayanaklar kaynak raporunun bulgularıdır.
Kanıt etiketleri araştırma README'sindeki tek kaynaktan kullanılır. Bir hata
kaydı, içindeki rakip davranışının otomatik olarak Manuel gözlem olduğu anlamına gelmez.

Kaynak: [E0381 — raporlar/2026-09-13-kapsamli-yeniden-denetim.md](raporlar/2026-09-13-kapsamli-yeniden-denetim.md).
Eski rapor satır numaraları tarihsel konumdur; düzeltme sırasında ilgili ifade
başlık ve içerikle bulunur. Her B kimliği kaynak rapordaki aynı B kimliğine karşılık gelir.
Korpus envanteri: [E0391 — KANIT-ENVANTERI.md](KANIT-ENVANTERI.md).

## Paket kaydı

| Paket | Hedef | Durum | Çıktı / sonraki adım |
|---|---|---|---|
| P0.1 | E0001–E0391 yerel dosya iskeleti | Kapandı | İçerik onayı değil; dosya kapsamı DURUM.md'de |
| P0.2 | B01–B19 kaynak aktarımı | Kapandı | 19 açık kayıt; bu dosya E0392 |
| P0.3-money-manager | E0010 tamamı, 379 satır | Kapandı | MM-M01–MM-M16 haritası; MM-Q01–MM-Q10 açık kontrol soruları |
| P0.3-hesap-defterim | E0007 tamamı, 367 satır | Kapandı | HD-M01–HD-M19 haritası; HD-Q01–HD-Q12 açık soruları |
| P0.3-goodbudget | E0006 tamamı, 293 satır | Kapandı | GB-M01–GB-M17 haritası; GB-Q01–GB-Q12 açık soruları |
| P0.3-parasut | E0011 tamamı, 241 satır | Kapandı | PS-M01–PS-M14 haritası; PS-Q01–PS-Q12 açık soruları |
| P0.3-logo-isbasi | E0009 tamamı, 248 satır | Kapandı | LI-M01–LI-M13 haritası; LI-Q01–LI-Q10 açık soruları |
| P0.3-quickbooks | E0012 tamamı, 169 satır | Kapandı | QB-M01–QB-M13 haritası; QB-Q01–QB-Q12 açık soruları; B13 bağlamı satır bazında |
| P0.3-kolaybi | E0008 tamamı, 799 satır | Kapandı | KB-M01–KB-M23 haritası; KB-Q01–KB-Q17 açık soruları |
| P0.3-bluecoins | E0005 tamamı, 362 satır | Kapandı | BC-M01–BC-M18 haritası; BC-Q01–BC-Q16 açık soruları |
| P0.3-wallet | E0014 tamamı, 380 satır | Kapandı | WL-M01–WL-M18 haritası; WL-Q01–WL-Q16 açık soruları |
| P0.4 | P0.1–P0.3 kabul kontrolü; kalan inceleme paket listesi | Kapandı | P0 kapandı; 3 harita düzeltmesi; 17 G paketi (357 PNG), B/K paketleri, T adayları |
| P1-B01 | B01 fatura bazlı tahsilat bağı kıyası | Kapandı | B01 kanıtla kapandı; dört formda dokuz düzeltme; dokuz sorunun B01 kısmı |
| P1-B02-B04 | B02/B03/B04 mevcut kart, plan bitişi ve tanınan gelir kıyasları | Kapandı | Üç bulgu kanıtla kapandı; üç formda on bir düzeltme; beş sorunun ilgili kısmı |
| P1-B14 | B14 kupür aracı ile kasa sayımı ihtiyacı | Kapandı | B14 kanıtla kapandı; E0007'de bir düzeltme; HD-Q09 ve HD-Q12'nin B14 kısmı |
| P1-goodbudget-G01 | E0106–E0133 Goodbudget görselleri | Kapandı | 28/28 görsel sonucu E0391'de; 11 form düzeltme adayı ve GB-Q ilerlemesi bu dosyanın P1-goodbudget-G01 bölümünde |
| P1-goodbudget-T01 | B05 gelir yolu kaynak/metin ve kullanıcı ekran kontrolü | Kanıtla kapandı | GB-U01-A/B/C; Keep Available görüldü, yeni işlem testi yok |
| P1-goodbudget-T02 | B06 bakiye nedenselliği | İddia kapsamı sınırlandı | Yeni deney yapılmadan yanlış nedensellik kaldırıldı |
| P1-B07 | Paket ve hesap türleri | Kanıtla kapandı | E0112 ve GB-S02/GB-S03 |

P0.3 uygulama sırası: Money Manager, Hesap Defterim, Goodbudget, Paraşüt,
Logo İşbaşı, QuickBooks, KolayBi, Bluecoins, Wallet. Her devam isteği bir
uygulamanın içerik haritasını kapsar; uzunluk gerekirse başlık bazında bölünür.
Bu sıra yeni canlı test başlatmaz; P2/P3 tam doğrulamaları sonraki işlerdir.
G paketlerinin kesin E kimliği aralıkları P0.4 bölümünde atandı. B kayıtlarındaki
`nn` ifadeleri G paketi adlarına P0.4 tablosundan bağlanır; T numaraları G
sonuçlarından sonra verilir. Hiçbiri tamamlanmış test değildir.

## Kapanış disiplini

- Her kaydın ortak durumu **açık**; yeni doğrulama sonucu **henüz yok**.
- Güncel platform/paket/sürüm, koşum ayarı ve zaman bilgisi ilgili doğrulamada
  kaydedilecek; eski gözlem farklı ürüne veya sürüme genellenmeyecek.
- Kapatırken kanıt etiketi, E kimliği/URL/kod referansı, yöntem, tarih,
  gerçek sonuç ve kalan sınır yazılır. Ölçülmeyen yarar/bedel yorum olarak ayrılır.
- Etkilenen form ve ilgili özetlerde yayılım kontrolü ayrıca tamamlanır.
- Kapanış seçenekleri: kanıtla kapandı veya iddia kapsamı sınırlandı; ikincisi
  bilinmeyen davranışın doğru/yanlış olduğu anlamına gelmez. Engelde neden yazılır.
- Başlangıç öncelikleri planlama içindir. Karara etkisi değişirse gerekçeyle güncellenir.
- Kaynak raporundaki geçmiş bulgu metni değiştirilmez; güncel çözüm bu kayıtta izlenir.

## Açık bulgular

### B01 — Fatura bazlı tahsilat bağı

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B01). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Bizde belirli faturaya tahsilat bağlama ve kapanış garantisi varmış gibi kıyas yapılıyor.
- **Kaynak raporunun dayanağı:** CounterpartyPayment karşı taraf/hesap taşır; tanıma/taşıma ayrımı fatura tahsis garantisi değildir.
- **İlgili formlar / yayılım hedefi:** [E0005 — gozlemler/bluecoins.md](gozlemler/bluecoins.md), [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md), [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Ek dayanak / kontrol adresi:** [../../src/BusinessFinance.Domain/CounterpartyPayment.cs](../../src/BusinessFinance.Domain/CounterpartyPayment.cs), [../../documentation/architecture.md](../../documentation/architecture.md), [../../documentation/adr/0014-economic-event-recognizes-payment-carries.md](../../documentation/adr/0014-economic-event-recognizes-payment-carries.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Kod ve ADR 0014 ile gelir/gider tanıma, hesap etkisi ve belirli borca bağlanmayı ayrı kontrol et; yalnız mevcut yetenekleri yaz.
- **Bağlı iş:** P1-B01. **Rapor hedefi:** Belge 2 borç/tahsilat; Belge 3 mevcut durum.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** BusinessFinance cari tahsilatı
  belirli borçlandırmaya bağlamaz; bakiye karşı taraf toplamıdır. Belirli kayda bağlı
  kapanış yalnız tek seferlik yükümlülükte (tek kapanış, tam tutar) ve borç taksitinde
  var. 14 Eylül 2026. Rakip kanıt etiketi uygulanmaz; dayanak BusinessFinance kodu,
  mimari belge ve ADR 0014 (P1-B01 bölümü).
- **Yayılım kontrolü:** yapıldı (14 Eylül 2026). E0005, E0008, E0011 ve E0014'te dokuz
  yer düzeltildi; özet/durum belgelerinde B01 iddiası kalmadı. B09/B19'a ait kalanlar
  P1-B01 bölümünde listelendi.

### B02 — Kart özellikleri ve sıfır tutar

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B02-B04). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Asgari oran, zorunlu limit ve kullanılabilir limit gösterimi yok; sıfır tutarlı normal işlem mümkün deniyor.
- **Kaynak raporunun dayanağı:** Kaynak denetimi kart alanlarını domain/Flutter'da ve Money sıfır reddini buldu; sıfır kasa sayımına genellenmez.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Ek dayanak / kontrol adresi:** [../../src/BusinessFinance.Domain/CreditCard.cs](../../src/BusinessFinance.Domain/CreditCard.cs), [../../src/BusinessFinance.Domain/Money.cs](../../src/BusinessFinance.Domain/Money.cs), [../../mobile/business_finance_mobile/lib/features/cards/presentation/finance_page.dart](../../mobile/business_finance_mobile/lib/features/cards/presentation/finance_page.dart).
- **Yapılacak kontrol ve kapanış ölçütü:** CreditCard, finance_page ve Money kontrolüyle dört iddiayı ayrı uzlaştır; mevcut özellikleri yeni özellik önerisi olmaktan çıkar.
- **Bağlı iş:** P1-B02. **Rapor hedefi:** Belge 2 kart; Belge 3 mevcut yetenekler.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** BusinessFinance kartında asgari
  ödeme oranı (varsayılan %20) ve asgari tutar hesabı var; limit zorunlu ve pozitif;
  kullanılabilir limit domain'de hesaplanır ve istemcide gösterilir. Normal para
  hareketlerinde sıfır/negatif tutar domain'de ve istemci formlarında reddedilir;
  bu kural kasa sayımı gibi kavramlara genellenmez. 14 Eylül 2026. Rakip kanıt
  etiketi uygulanmaz; dayanak kod (P1-B02-B04 bölümü).
- **Yayılım kontrolü:** yapıldı (14 Eylül 2026). E0008'de 4, E0005'te 1, E0007'de 1
  yer düzeltildi; özet ve durum belgelerinde B02 iddiası bulunmadı.

### B03 — İsteğe bağlı plan bitişi

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B02-B04). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** BusinessFinance planları süresiz, isteğe bağlı bitiş yok diye sunuluyor.
- **Kaynak raporunun dayanağı:** Kaynak denetimi nullable EndDate ve istemcide isteğe bağlı bitiş alanını buldu.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Ek dayanak / kontrol adresi:** [../../src/BusinessFinance.Domain/RecurringTransaction.cs](../../src/BusinessFinance.Domain/RecurringTransaction.cs), [../../mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart](../../mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart).
- **Yapılacak kontrol ve kapanış ölçütü:** Domain/Flutter'da tarih ve tekrar sınırını doğrula; KolayBi zorunlu tekrar sayısıyla mevcut davranışı karşılaştır.
- **Bağlı iş:** P1-B03. **Rapor hedefi:** Belge 2 planlama; Belge 3 mevcut durum.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** tekrarlayan planda bitiş tarihi
  ve toplam tekrar sınırı isteğe bağlıdır; birlikte verilebilir, önce dolan geçerlidir;
  ikisi de boşsa plan süresizdir. İstemci iki alanı ve üretilen/sınır sayacını
  gösterir. KolayBi'den fark yalnız sayının orada zorunlu olması. 14 Eylül 2026.
  Rakip kanıt etiketi uygulanmaz; dayanak kod (P1-B02-B04 bölümü).
- **Yayılım kontrolü:** yapıldı (14 Eylül 2026). E0008'de 2 yer düzeltildi; başka
  form ve özette B03 iddiası bulunmadı.

### B04 — Tanınan ve tahsil edilen gelir

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B02-B04). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Nakit esaslı ifadesinden tahsil edilmeyen satışın bizde tanınmadığı çıkarılıyor.
- **Kaynak raporunun dayanağı:** Kaynak denetimi cari borçlandırmanın ChargeDate ile rapora katıldığını belirtti.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Ek dayanak / kontrol adresi:** [../../src/BusinessFinance.Infrastructure/Reports/EfFinancialReportRepository.cs](../../src/BusinessFinance.Infrastructure/Reports/EfFinancialReportRepository.cs), [../../documentation/adr/0014-economic-event-recognizes-payment-carries.md](../../documentation/adr/0014-economic-event-recognizes-payment-carries.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Rapor sorgusu ve ADR 0014'ü kontrol et; olayın tanınması ile ekranda tanınan/tahsil edilen ayrımını gösterme ihtiyacını ayır. PRD/ADR'yi bu düzeltmeyle değiştirme.
- **Bağlı iş:** P1-B04. **Rapor hedefi:** Belge 2 raporlama; Belge 3 rapor önerileri.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** cari borçlandırma olay tarihinde,
  yükümlülük düzenleme tarihinde gelir/gider raporuna girer; tahsilat girmez (ADR 0014
  saf nakit esasını reddeder). Tahsil edilmemiş tutar cari bakiye ve net varlıkta açık
  alacak olarak görünür; aylık rapor kartında tanınan/tahsil edilen kırılımı yoktur.
  Projedeki "nakit esaslı işletme neti" terimi tahsil edilmemiş satışın tanınmadığı
  anlamına gelmez; PRD/ADR ve README karar filtresi değiştirilmedi. 14 Eylül 2026.
  Rakip kanıt etiketi uygulanmaz; dayanak kod ve ADR (P1-B02-B04 bölümü).
- **Yayılım kontrolü:** yapıldı (14 Eylül 2026). E0008'de 2, E0007'de 1 yer düzeltildi;
  DURUM ve README'deki proje terimi kullanımları çıkarım içermediği için korundu,
  terimin yanlış okunma riski B19'a not edildi.

### B05 — Goodbudget gerçek gelir yolu

- **Durum:** kanıtla kapandı (14 Eylül 2026; GB-U01-A/B/C). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Expense/Credit örneği normal gelir yoluna genellenmiş.
- **Kaynak raporunun dayanağı:** Kaynak denetimi ayrı Fill Envelopes / Keep Unallocated gelir yolunu resmî kaynakta buldu; canlı karşılaştırma yok.
- **İlgili formlar / yayılım hedefi:** [E0006 — gozlemler/goodbudget.md](gozlemler/goodbudget.md).
- **Ek dayanak / kontrol adresi:** [Resmî kaynak](https://goodbudget.com/help/getting-started-guide/step-3-add-income/).
- **Yapılacak kontrol ve kapanış ölçütü:** GB-U01 kullanıcı kontrolü; yeni finansal işlem/karşılaştırma deneyi zorunlu değil.
- **Bağlı iş:** P1-goodbudget-T01 / P1-B05. **Rapor hedefi:** Belge 2 gelir/bütçe; Belge 3 öneriler.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** GB-S01 Android normal gelir yolunu açıklar; Credit normal gelir sayılmaz. Canlı sonuç karşılaştırması yapılmadı. Kullanıcı gelir formunu ve Keep Available seçeneğini gösterdi; GB-U01-A/B/C arşivlendi. 14 Eylül 2026. Kaynak ayrıntıları Goodbudget formundadır.
- **Yayılım kontrolü:** form düzeltildi; eski özetlerin Goodbudget hükümleri tarihsel/geçersiz olarak ayrıldı ve güncel özet eklendi.

### B06 — Goodbudget −600 nedenselliği

- **Durum:** iddia kapsamı sınırlandı (14 Eylül 2026). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Nedeni bilinmeyen fark bazı bölümlerde liste dışı tekrarlayan kayıt olarak kesinleştirilmiş.
- **Kaynak raporunun dayanağı:** Tek Bulut kaydı görülen ekran farkın nedenini kanıtlamaz; −16 silme sapması ayrı gözlemdir.
- **İlgili formlar / yayılım hedefi:** [E0006 — gozlemler/goodbudget.md](gozlemler/goodbudget.md).
- **Ek dayanak / kontrol adresi:** [E0130 — kanitlar/goodbudget/23-transactions-initial-envelope-fill.png](kanitlar/goodbudget/23-transactions-initial-envelope-fill.png).
- **Yapılacak kontrol ve kapanış ölçütü:** Mevcut kanıtla yukarıdaki sınırda kapandı; yeni canlı koşum gerekmiyor.
- **Bağlı iş:** P1-goodbudget-T02 / P1-B06. **Rapor hedefi:** Belge 2 tekrar ve bakiye; Belge 3 koşullu karar.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** E0120/E0125/E0129–E0132: 16 ve 600 farkları korunur; genel silme kusuru veya sonraki tekrar nedenselliği kaldırıldı. Kök neden deneyi yapılmayacak. 14 Eylül 2026. Kaynak ayrıntıları Goodbudget formundadır.
- **Yayılım kontrolü:** form düzeltildi; eski özetlerin Goodbudget hükümleri tarihsel/geçersiz olarak ayrıldı ve güncel özet eklendi.

### B07 — Goodbudget paket ve hesap türü

- **Durum:** kanıtla kapandı (14 Eylül 2026; görsel ve resmî kaynak). **Öncelik:** Anlatımı sınırlar; karara taşınırsa engeller.
- **İddia/çelişki:** Ücretsiz testten bütün üründe banka senkronizasyonu ve hesap türü yok sonucuna gidiliyor.
- **Kaynak raporunun dayanağı:** Kaynak denetimi Premium bank sync dokümanı ve formun kendi üç hesap grubu kaydını buldu.
- **İlgili formlar / yayılım hedefi:** [E0006 — gozlemler/goodbudget.md](gozlemler/goodbudget.md).
- **Ek dayanak / kontrol adresi:** [Resmî kaynak](https://goodbudget.com/help/automatic-bank-sync/how-does-automatic-bank-sync-work/).
- **Yapılacak kontrol ve kapanış ölçütü:** Mevcut kanıtla yukarıdaki sınırda kapandı; yeni canlı koşum gerekmiyor.
- **Bağlı iş:** P1-B07. **Rapor hedefi:** Belge 1 ürün profili; Belge 2 özellik/entegrasyon.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** E0112 hesap grupları; GB-S02 bir ana hesap ile bütçe dışı Debt hesabını ayırır. GB-S03 Premium senkronizasyonu açıklar. Ürün genelinde hesap türü/banka yokluğu kaldırıldı; banka bağlantısı kurulmadı. 14 Eylül 2026. Kaynak ayrıntıları Goodbudget formundadır.
- **Yayılım kontrolü:** form düzeltildi; eski özetlerin Goodbudget hükümleri tarihsel/geçersiz olarak ayrıldı ve güncel özet eklendi.

### B08 — Demo adları kullanıcı talebi değildir

- **Durum:** iddia kapsamı sınırlandı (14 Eylül 2026, P1-B08). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Demo proje adları gerçek kullanıcıların hane gideri ihtiyacına kanıt sayılıyor.
- **Kaynak raporunun dayanağı:** Demo ekranı adları gösterir; kimin neden kullandığını göstermez.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Ek dayanak / kontrol adresi:** [E0188 — kanitlar/kolaybi/d01-destek-proje-listesi.png](kanitlar/kolaybi/d01-destek-proje-listesi.png).
- **Yapılacak kontrol ve kapanış ölçütü:** Demo gözlemi ve serbest proje ekseninin kullanılabilirliği hipotezini ayır; kullanıcılar böyle kullanıyor hükmünü kaldır/sınırla. Yeni kullanıcı araştırması bu pakete dahil değil.
- **Bağlı iş:** P1-B08. **Rapor hedefi:** Belge 1 ürün kimliği; Belge 2 proje; Belge 3 ihtiyaç gerekçesi.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** Demo adları (E0188), kayıt seçenekleri (Logo E0222) ve cari sekme adları konumlandırma/arayüz gözlemidir; kullanıcı davranışı, talep veya ADR 0013 kanıtı değildir. Kullanım biçimleri `çıkarım`, esnek eksen kullanımı hipotez olarak yazıldı. 14 Eylül 2026. Ayrıntı: P1-B08 bölümü.
- **Yayılım kontrolü:** KolayBi, Logo İşbaşı, Paraşüt formları, DURUM özeti ve 2 Eylül fark notu düzeltildi; kalan eşleşmeler alıntı veya tarihsel belge (P1-B08 bölümü).

### B09 — Wallet D2/D3 model eşdeğerliği

- **Durum:** **kanıtla kapandı** (15 Eylül 2026; WL-U01-A kullanıcı ekran kontrolü). Wallet'ın borç modeli ADR 0014 tanıma/taşıma ayrımına **eşdeğer değil**: Record'lu borç verme gider, tahsilat gelir sayılıyor; Record'suz fatura hiç gelir yazılmıyor. *(P3-T'deki “kapsamı sınırlı” durumu bu kanıtla kapandı.)* **Öncelik:** Karar etkisi çözüldü; Belge 2 bu sonucu, Belge 3 “rakipte eşdeğer model var” varsayımı olmadan kullanır.
- **İddia/çelişki:** Aynı form ayrım yok ve ADR 0014 ile doğal eşdeğer model hükümlerini birlikte taşıyor.
- **Kaynak raporunun dayanağı:** Debt–Record bağı, gelir raporunda tanıma zamanı ve tekrar gelir sayılmamasını tek başına kanıtlamaz.
- **İlgili formlar / yayılım hedefi:** [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md).
- **Yapılacak kontrol ve kapanış ölçütü:** D2 öncesi/sonrası ve D3 sonrası hesap, açık borç, gelir/gider raporunu ayrı ölç; bağlantı gözlemini model eşdeğerliğinden ayır.
- **Bağlı iş:** P3-Tnn / P3-K. **Rapor hedefi:** Belge 2 borç/gelir; Belge 3 finansal model.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** P3-G02/G04 ve P3-T, 15 Eylül (Manuel gözlem, görsel). E0317: Record'lu borç Ana Hesap'ta Loan, interests −5.000 kaydı üretir. E0366/E0371: Record'suz D2 ₺12.000 ayrı borç kartı; D1 sonrası 4.800 → D3 sonrası 9.800 farkı yalnız D3'ün +5.000'i, D2'nin bakiye etkisi 0. E0372/E0373: D2 kayıt listesinde ve “Ada” aramasında yok; D3 Lending, renting +5.000 listede. E0368/E0370: D3 “₺12.000,00 to Repay debt” ile D2 kartına bağlı, kart 12.000 → 7.000. Ölçülmeyen: Record'lu kayıtların Cash-flow/gelir raporu etkisi (isteğe bağlı WL-U01).
- **P3-T değerlendirmesi (15 Eylül):** Belge 2 davranışı bu sınırla anlatır; Belge 3'ün önerisi bizde ADR 0014 ile kurulu olduğundan rakipte eşdeğerlik ölçümüne dayanmaz. Ek canlı kontrol kapanış şartı değildir; daha güçlü rapor iddiası istenirse WL-U01 yapılır.
- **Yayılım kontrolü:** Wallet formunda D2 sonrası “native destekleyen tek rakip” ve tek cümlelik sonuç daraltıldı. DURUM Tur 1 tablosu ve TUR2 Faz 7 özeti tarihsel koşum anlatımıdır; güncel dayanak bu kayıt ve formdur.

### B10 — Bluecoins aritmetik, silme ve otomasyon

- **Durum:** iddia kapsamı sınırlandı (15 Eylül 2026, P2-T); aritmetik doğrulandı, otomasyon/silme sonuçları bilinmiyor. **Öncelik:** Anlatımı sınırlar; doğrulanmamış dallarda ürün önerisi üretilemez.
- **İddia/çelişki:** 600 gider için 44.350→43.350 yazılıyor; silme ve otomasyon sonuçları genelleniyor.
- **Kaynak raporunun dayanağı:** 43.950 olasılığı yalnız aritmetik çıkarımdır; kalıcı silme ile çöp kutusu ve ilk taksit ile tekrar modu ayrılmamış.
- **İlgili formlar / yayılım hedefi:** [E0005 — gozlemler/bluecoins.md](gozlemler/bluecoins.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Başlangıç görseliyle tutarı doğrula; B1 onaylı/otomatik, B2 ilk/sonraki taksiti ayır. Silme/geri alma ayrı kontrollü senaryolar; veriyi sıfırlama ve zamanı değiştirme yok.
- **Bağlı iş:** P2-Gnn / P2-Tnn / P2-K. **Rapor hedefi:** Belge 2 tekrar/taksit/silme; Belge 3 öneriler.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** P2-G02, 15 Eylül: E0036/E0038/E0044/E0049/E0050 ile 43.950→43.350 net zinciri düzeltildi (görünür hesap toplamlarından çıkarım). B1 kapalı otomasyon modu ve B2 geçmiş ilk ödeme ayrıldı; açık otomasyon, geleceğe ilk ödeme ve silme/geri yükleme doğrulanmadı.
- **P2-G03 ek doğrulama (15 Eylül):** E0053 başlangıç neti 43.350; E0065 kira sonrası banka 29.700; E0075 cari gelir 12.000; E0079 transfer sonrası cari 7.000/banka 34.700 ve gün toplamı 2.000. Otomasyon/silme açık; B10 kapanmadı.
- **P2-G04 ek doğrulama (15 Eylül):** E0089/E0097 boş Çöp Kutusu hedefini gösteriyor; silinen kaydın buraya düşmesi, kalıcılık ve geri yükleme ölçülmedi. E0098/E0099 transferleri hariç günlük −10.000/+12.000/2.000 akışını doğruluyor. B10 kapanmadı.
- **P2-T değerlendirmesi (15 Eylül):** Mevcut kanıt ve form sınırları yeterli. B1 otomatik kapalı, B2 geçmiş ilk ödeme ve D1 aynı gün onayı ayrı tutulur; silme kalıcılığı/geri yükleme bilinmiyor. Bu dallar denenmiş veya kanıtla kapanmış sayılmaz. Kesin davranış iddiası ve öneri kurulmadığından ek canlı kontrol kapanış şartı değildir; yeni karar ihtiyacı doğarsa ilgili soru yeniden açılır.
- **Yayılım kontrolü:** Bluecoins B1/B2, bakiye tablosu, karar/güven/sonuç bölümleri; DURUM ve TUR2 güncel özetleri düzeltildi. Eski paket devirleri tarihsel, güncel dayanak değildir; ayrıntı P2-G02 sonunda.

### B11 — Açık e-posta ayarı gönderim kanıtı değil

- **Durum:** iddia kapsamı sınırlandı (14 Eylül 2026, P1-B11). **Öncelik:** Anlatımı sınırlar; güvenlik kararında engeller.
- **İddia/çelişki:** İşaretli ayardan varsayılan açık ve kendiliğinden finansal veri gönderimi sonucu çıkarılıyor.
- **Kaynak raporunun dayanağı:** Ekranda işaretli kutu var; alıcı, tetikleyici, ön kurulum ve teslim kanıtı yok.
- **İlgili formlar / yayılım hedefi:** [E0007 — gozlemler/hesap-defterim.md](gozlemler/hesap-defterim.md).
- **Ek dayanak / kontrol adresi:** [E0178 — kanitlar/hesap-defterim/43-ayarlar-alt-bolum-donem-baslangici.png](kanitlar/hesap-defterim/43-ayarlar-alt-bolum-donem-baslangici.png).
- **Yapılacak kontrol ve kapanış ölçütü:** Görünen ayarı gözlem olarak kaydet; varsayılan ve gerçek gönderimi doğrulanamadı diye ayır. Ayrı kullanıcı izni olmadan e-posta gönderme veya reset yapma.
- **Bağlı iş:** P1-B11. **Rapor hedefi:** Belge 1 ayarlar; Belge 2 dışa aktarım; Belge 3 güvenlik.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** E0178 yalnız ayarın 12 Eylül'de işaretli olduğunu gösterir (manuel gözlem). Kurulum varsayılanı, alıcı, tetikleyici ve teslim edilmiş e-posta doğrulanamadı; "tamamen yerel" hükmü uygulama içi beyan (E0149) olarak sınırlandı. E-posta gönderilmedi, reset yapılmadı. 14 Eylül 2026.
- **Yayılım kontrolü:** Hesap Defterim formu (7 yer) ve DURUM uygulama tablosu (1 yer) düzeltildi; ayrıntı P1-B11 bölümü.

### B12 — Görünmeyen fiziksel veritabanı şeması

- **Durum:** iddia kapsamı sınırlandı (14 Eylül 2026, P1-B12). **Öncelik:** Anlatımı sınırlar; model kararında engeller.
- **İddia/çelişki:** Arama/listeden fiziksel tablo ve kayıt saklama yapısı kesinleştiriliyor.
- **Kaynak raporunun dayanağı:** Aynı dış davranış farklı şemalarla üretilebilir.
- **İlgili formlar / yayılım hedefi:** [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md), [E0007 — gozlemler/hesap-defterim.md](gozlemler/hesap-defterim.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Fiziksel şema iddialarını gözlenebilir liste/arama/silme davranışıyla değiştir; şema ancak yetkili kod/DB veya açık teknik kaynakla kanıtlanır, erişim varsayma.
- **Bağlı iş:** P1-B12 / P3-K. **Rapor hedefi:** Belge 2 pipeline; Belge 3 mimari gerekçe.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** Fiziksel şema iddiaları gözlenebilir davranışla değiştirildi: Wallet'ta Record'suz borç aramada ve işlem listesinde görünmüyor, yalnız Debts ekranında görünüyor; Hesap Defterim'de Aktar listede iki ayrı satır olarak görünüyor ve toplamlar satırlarla tutarlı. Saklama yapısı, ikinci tablo yokluğu ve çift kayıt nesnesi ekrandan çıkarılamaz; yetkili kod/DB veya teknik kaynak yok. Manuel gözlem + çıkarım; 14 Eylül 2026.
- **Yayılım kontrolü:** Wallet formu (2), Hesap Defterim formu (5), DURUM (2), Tur 2 yol haritası (1) düzeltildi; ayrıntı P1-B12 bölümü.

### B13 — QuickBooks ürün/paket ayrımı

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B13). **Öncelik:** Anlatımı sınırlar; ürün kıyasını engeller.
- **İddia/çelişki:** Simple Start fiyatı ve onboarding'i Solopreneur alanına taşınıyor.
- **Kaynak raporunun dayanağı:** Kaynak denetimi ekranın Simple Start olduğunu ve deneme vaadinin erişilebilirlik kanıtı olmadığını belirtti.
- **İlgili formlar / yayılım hedefi:** [E0012 — gozlemler/quickbooks.md](gozlemler/quickbooks.md).
- **Ek dayanak / kontrol adresi:** [E0271 — kanitlar/quickbooks/04-choose-plan-paywall.png](kanitlar/quickbooks/04-choose-plan-paywall.png).
- **Yapılacak kontrol ve kapanış ölçütü:** Fiyat/onboarding satırları ve kanıt envanterinde QBO Simple Start bağlamını koru; bu karelerden Solopreneur fiyat/akışı çıkarma.
- **Bağlı iş:** P1-B13. **Rapor hedefi:** Belge 1 ürün profili; Belge 2 onboarding.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** P1-quickbooks-G01 görsel incelemesinde dört karenin hiçbirinde Solopreneur adı yok: karusel adsız (E0268), hoş geldin ve basic info "QuickBooks" (E0269/E0270), plan ekranı "Simple Start" (E0271). Fiyat, deneme ifadesi ve onboarding satırları QuickBooks mobil onboarding'i / QBO Simple Start bağlamıyla yazıldı; Solopreneur'ün fiyatı, deneme koşulu ve onboarding'i doğrulanamadı olarak ayrıldı. Manuel gözlem + resmî kaynak ayrımı; 14 Eylül 2026.
- **Yayılım kontrolü:** QuickBooks formu (10 yer), kanitlar/README klasör etiketi (1), DURUM Tur 1 tablosu (1), MANUEL-TEST-PROTOKOLU uygulama tablosu (1), README (2) düzeltildi; ayrıntı P1-B13 bölümü.

### B14 — Kupür aracı ile kasa sayımı ihtiyacı

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B14). **Öncelik:** Kararı engeller.
- **İddia/çelişki:** Fiziksel kasa ihtiyacının bizde karşılığı olmadığı söyleniyor.
- **Kaynak raporunun dayanağı:** CashCount ve kasa sayım/fark akışı var; kupür×adet aracıyla aynı özellik değil.
- **İlgili formlar / yayılım hedefi:** [E0007 — gozlemler/hesap-defterim.md](gozlemler/hesap-defterim.md).
- **Ek dayanak / kontrol adresi:** [../../src/BusinessFinance.Domain/CashCount.cs](../../src/BusinessFinance.Domain/CashCount.cs), [../../mobile/business_finance_mobile/lib/features/cash/presentation/cash_count_view.dart](../../mobile/business_finance_mobile/lib/features/cash/presentation/cash_count_view.dart).
- **Yapılacak kontrol ve kapanış ölçütü:** Domain ve istemciyi kontrol et; mevcut sayım/uzlaştırma ile kupür yardımcısını ayrı kıyasla, kupür aracı var diye de yazma.
- **Bağlı iş:** P1-B14. **Rapor hedefi:** Belge 2 kasa; Belge 3 öncelik.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** BusinessFinance'te gün sonu kasa
  sayımı var: yalnız aktif nakit hesapta sayılan tutar bir gözlem olarak saklanır
  (sıfır meşru, negatif reddedilir), bakiyeye ve rapora dokunmaz; beklenen bakiye ve
  fark okunduğu anda türetilir. Fark kendiliğinden yazılmaz; kullanıcı onaylarsa tek,
  idempotent gelir/gider düzeltmesi olur. İstemcide `Kasa > Gün sonu` iki sayıyı ve
  fazla/eksik durumunu gösterir. Kupür × adet yardımcısı yok; sayılan tutar tek serbest
  alandır. 14 Eylül 2026. Rakip kanıt etiketi uygulanmaz; dayanak kod (P1-B14 bölümü).
- **Yayılım kontrolü:** yapıldı (14 Eylül 2026). E0007'de bir yer düzeltildi; özet ve
  durum belgelerinde BusinessFinance'e kasa sayımı yokluğu atfeden ifade bulunmadı.

### B15 — KolayBi eski/yeni görsel bağlamı

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B15). **Öncelik:** Anlatımı sınırlar.
- **İddia/çelişki:** 01–08 aynı tarihli video sayılıyor, Kasalar ve Banka ekranı aynı ekranın iki sürümü diye eşleniyor.
- **Kaynak raporunun dayanağı:** 01 mobil, 02–07 eski video, 08 ayrı kaynak; işlem/yükleme tarihi UI sürümü değildir. Üç sayı 39 görseli kanıtlamaz.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Tüm ilgili kaynak/tarih ve ekran rolleri yeniden eşlensin; 05/d07 cari, 07/d19 kasa/banka ayrımı korunsun. Seçim veya arşiv önerisi silme yetkisi değil.
- **Bağlı iş:** P1-kolaybi-Gnn / P1-B15. **Rapor hedefi:** Belge 1 görsel karşılaştırma; Belge 2 kaynak sınırı.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** G01/G02'de 39 görselin tamamı açıldı. Roller: d32 (E0180) mobil giriş, manuel gözlem; d33–d38 (E0181–E0186) ~2020 tanıtım videosu; d39 (E0187) ayrı video, 2026 başı yükleme kullanıcı aktarımı; d01–d31 (E0188–E0218) destek mockup'ı. Karelerdeki 2020/2023/2024 tarihleri demo verisi, yükleme tarihi sürüm tarihi değildir. Dört eşleşmeden ikisi aynı ekran, ikisi (d37/d31, d38/d19) farklı sekme. Resmî kaynak + görsel inceleme; 14 Eylül 2026.
- **Yayılım kontrolü:** KolayBi formu (30 yer) ve DURUM (5 yer) düzeltildi; raporlar/ altındaki tarihsel belgeler değiştirilmedi (P1-B15 bölümü).

### B16 — Bluecoins filtre/menu/export görsel rolü

- **Durum:** kanıtla kapandı (15 Eylül 2026, P2-G04). **Öncelik:** Anlatımı sınırlar; export iddiasını engeller.
- **İddia/çelişki:** Farklı hash veya dosya adından farklı özellik ekranı sonucu çıkarılıyor.
- **Kaynak raporunun dayanağı:** Kaynak denetiminde üç dosya aynı filtre panelini gösterdi; export çıktısını göstermiyor.
- **İlgili formlar / yayılım hedefi:** [E0005 — gozlemler/bluecoins.md](gozlemler/bluecoins.md).
- **Ek dayanak / kontrol adresi:** [E0082 — kanitlar/bluecoins/f7-31-filtre.png](kanitlar/bluecoins/f7-31-filtre.png), [E0083 — kanitlar/bluecoins/f7-32-menu.png](kanitlar/bluecoins/f7-32-menu.png), [E0104 — kanitlar/bluecoins/f7-53-export.png](kanitlar/bluecoins/f7-53-export.png).
- **Yapılacak kontrol ve kapanış ölçütü:** Üç görseli metinle eşleştir; süreç/özellik kanıtı rolünü ayır. Dışa aktarma iddiasına doğru kanıt bul veya iddiayı sınırla. Görseli otomatik silme.
- **Bağlı iş:** P2-Gnn / P2-K. **Rapor hedefi:** Belge 1 seçilmiş görseller; Belge 2 export.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** E0082, E0083 ve E0104 aynı filtre panelidir; E0083 menü, E0104 export sonucu değildir. Gerçek seçenek yüzeyi E0105'tir ve yalnız PDF/Yazıcı, Excel (.csv), HTML seçeneklerini gösterir; dosya üretimi ve Premium son adımı doğrulanmadı. E0084 gerçek gezinme çekmecesidir. Manuel görsel inceleme, 15 Eylül 2026.
- **Yayılım kontrolü:** Bluecoins arama/filtre/export anlatımı, K07, aday kararlar, güven ve sonuç bölümleri ile envanterde 26 rol güncellendi. Dosyalar silinmedi; adları tarihsel kaynak olarak korundu.

### B17 — Denetim betiği kapanış kapısı değil

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-K). **Öncelik:** Geçiş kapısını engeller.
- **İddia/çelişki:** denetim.sh hata ve olmayan formda exit 0; atıf ayrıştırması eksik.
- **Kaynak raporunun dayanağı:** Kaynak koşumda 17 sorun grubu/exit 0, olmayan form temiz/exit 0; bağımsız cjs bütün semantik/görsel denetimin yerine geçmez.
- **İlgili formlar / yayılım hedefi:** [E0010 — gozlemler/money-manager.md](gozlemler/money-manager.md), [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md), [E0005 — gozlemler/bluecoins.md](gozlemler/bluecoins.md), [E0007 — gozlemler/hesap-defterim.md](gozlemler/hesap-defterim.md), [E0006 — gozlemler/goodbudget.md](gozlemler/goodbudget.md), [E0011 — gozlemler/parasut.md](gozlemler/parasut.md), [E0009 — gozlemler/logo-isbasi.md](gozlemler/logo-isbasi.md), [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md), [E0012 — gozlemler/quickbooks.md](gozlemler/quickbooks.md).
- **Ek dayanak / kontrol adresi:** [E0002 — denetim.sh](denetim.sh), [E0382 — raporlar/denetim-2026-09-13.cjs](raporlar/denetim-2026-09-13.cjs).
- **Yapılacak kontrol ve kapanış ölçütü:** Temiz/bozuk/olmayan dosya, tam/kısa/yollu/aralıklı atıf ve açıklama örneklerini pozitif/negatif testle kontrol et; gerçek hata sıfır olmayan çıkış versin. Yapısal kontrol ile içerik onayını ayır.
- **Bağlı iş:** P1-K. **Rapor hedefi:** Bütün raporların kanıt kalite kapısı.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** Eski betiğin iki kusuru yeniden üretildi (9 uygulamada 9 sorun → çıkış 0; olmayan form → "Denetim temiz", çıkış 0) ve P1 formlarındaki 22 en tireli aralığı görmediği bulundu. Kurallar test edilebilir `denetim.cjs`'e (E0393) taşındı; `denetim.sh` onu çağıran sarmalayıcı oldu. `denetim-test.cjs` (E0394) temiz/bozuk/olmayan form, tam/kısa/yollu/aralıklı atıf, örnek metni, yasak kalıp ve tablo için 15 pozitif/negatif test taşıyor; 15/15 geçti. Gerçek korpus: P1'in yedi formu 22 aralık düzeltmesinden sonra 0 hata (çıkış 0); Bluecoins 83, Wallet 100 hata (çıkış 1, P2/P3 borcu); olmayan form ve bilinmeyen bayrak çıkış 2. Yapısal kontroldür, içerik onayı değildir. Mekanik kontrol + test; 14 Eylül 2026.
- **Yayılım kontrolü:** Altı P1 formunda 22 aralık atıfı ve Hesap Defterim'de doğrulanamayan "on kare" sayısı; MANUEL-TEST-PROTOKOLU (2), DURUM (1), KolayBi formu (1), kanitlar/README klasör etiketleri (3); ayrıntı P1-K bölümü.

### B18 — Üç tablonun kanıt sütunu

- **Durum:** kanıtla kapandı (14 Eylül 2026, P1-B18). **Öncelik:** Geçiş kapısını engeller.
- **İddia/çelişki:** Üç başlık sütununa karşı dört hücreli satırlar var.
- **Kaynak raporunun dayanağı:** Kaynak koşumda üç tablo, sekizer satır, toplam 24 uyumsuz veri satırı.
- **İlgili formlar / yayılım hedefi:** [E0005 — gozlemler/bluecoins.md](gozlemler/bluecoins.md), [E0010 — gozlemler/money-manager.md](gozlemler/money-manager.md), [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md).
- **Yapılacak kontrol ve kapanış ölçütü:** Üç formdaki başlık/ayraç ve veri hücrelerini hizala; kanıt hücrelerinin görünürlüğünü ve mekanik kontrolü doğrula.
- **Bağlı iş:** P1-B18. **Rapor hedefi:** Belge 1/2 kaynak okunabilirliği.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** 14 Eylül ölçümü 13 Eylül denetimiyle aynı çıktı: Bluecoins, Money Manager ve Wallet arayüz taraması tablolarında başlık/ayraç 3 sütun, 24 veri satırı 4 hücre; gözlem formlarında başka uyumsuz tablo yok. Üç tabloya `Kanıt` başlığı ve dördüncü ayraç eklendi; veri hücreleri değişmedi. Düzeltme sonrası aynı hücre sayma kuralıyla formlarda uyumsuz satır 0. Mekanik kontrol; 14 Eylül 2026.
- **Yayılım kontrolü:** Bluecoins, Money Manager ve Wallet formları (birer tablo başlığı); MM-Q10 gereği Money Manager formunda dört görselin tam adı (4 yer); ayrıntı P1-B18 bölümü.

### B19 — Özetlere yayılan eski/yanlış iddialar

- **Durum:** **kanıtla kapandı** (15 Eylül 2026, P4-B19; yayılım kontrolü). **Öncelik:** Geçiş kapısı engeli kalktı; tarihsel bölümler tarihsel işaretli.
- **İddia/çelişki:** Eski fiyat, çek/senet yokluğu, genellenen yetkiler, toplu resmî kaynak etiketi ve çelişen durum/veri silme ifadeleri sürüyor.
- **Kaynak raporunun dayanağı:** Plan/devir düzenlemeleri yapıldı ancak içerik yayılımının tümü henüz denetlenmedi; B19 bu nedenle kapanmaz.
- **İlgili formlar / yayılım hedefi:** [E0008 — gozlemler/kolaybi.md](gozlemler/kolaybi.md), [E0009 — gozlemler/logo-isbasi.md](gozlemler/logo-isbasi.md), [E0012 — gozlemler/quickbooks.md](gozlemler/quickbooks.md), [E0006 — gozlemler/goodbudget.md](gozlemler/goodbudget.md), [E0014 — gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md).
- **Ek dayanak / kontrol adresi:** [E0386 — raporlar/turk-on-muhasebe-vs-businessfinance.md](raporlar/turk-on-muhasebe-vs-businessfinance.md), [E0003 — DURUM.md](DURUM.md), [E0389 — TUR2-YOL-HARITASI.md](TUR2-YOL-HARITASI.md), [E0272 — kanitlar/README.md](kanitlar/README.md), [E0388 — SENTETIK-TEST-VERISI.md](SENTETIK-TEST-VERISI.md).
- **Yapılacak kontrol ve kapanış ölçütü:** B01–B18 sonuçlarını özet/karar/durum/kanıt girişine yay; Logo yetkisini üç ürüne genelleme, fiyatı doğrula veya sınırla. Tarihsel notları ayır; veri koruma kuralıyla çelişen operasyon talimatlarını uzlaştır.
- **Bağlı iş:** P1-B19 / P4-K. **Rapor hedefi:** Bütün raporlar; güncel durum ve kapsam.
- **Yeni sonuç / doğrulama tarihi / kanıt etiketi:** P4-B19, 15 Eylül — denetim raporunda adı geçen her hedef formla uzlaştırıldı; ayrıntı bu dosyanın sonundaki P4-B19 bölümü.
- **Yayılım kontrolü:** yapıldı. B01–B18 yayılımları kendi P1/P2/P3 paketlerinde kayıtlı; P4-B19 yalnız bu bulgunun hedeflerini ve taranan eski ifadeleri doğruladı.

## B19 yayılım listesi ve bağımlılıklar

B19 diğer düzeltmelerin özetlere yayılımını izler; yalnız girişe devir notu
konması yeterli değildir. Başlangıç hedefleri: E0386 karşılaştırma özeti,
E0003 DURUM, E0389 yol haritası, E0272 kanıt girişi ve E0388 veri kuralı.
B01/B04/B09 model, B07/B13 paket, B08 kullanıcı ihtiyacı, B11/B12 aşırı kesin
mekanizma iddiaları özellikle çapraz kontrol edilir. Tarihsel alıntılar
korunabilir ama yürürlükte talimat veya güncel sonuç gibi sunulamaz.

## Kaynak raporundaki numarasız devir notları

B01–B19 dışında yeni doğrulanmış hata kimliği üretilmedi. Kaynak raporunun
'Dokuz uygulama için kapanış değerlendirmesi' bölümündeki şu noktalar P0.3'e taşınır:

- Paraşüt: aynı faturanın iki durumunun aynı kayıt kimliğiyle kanıtlanıp
  kanıtlanmadığı; otomatik mahsup ile bizim tahsilat bağının eşitlenmemesi.
- Logo İşbaşı: OCR → otomatik gider ve insan onayı adımlarının kesin kaynakları.
- Money Manager/Wallet/Bluecoins: tam ad atıf borcu B17/B18 ve G paketlerinde;
  yalnız tam adın bir yerde geçmesi tüm atıfların doğru olduğunu kanıtlamaz.

Bunlar henüz yeniden incelenmedi. P0.3'te mevcut B kaydına bağlanır veya gerçekten
ayrı bir soru ise yeni kimlikle açılır; kaynak raporu yeni test gibi gösterilmez.

## Oturum devri

P0 kapandı (P0.1–P0.4); P1-B01, P1-B02-B04, P1-B14 ve P1-goodbudget-G01 kapandı;
yarım aktarım yok. **Sıradaki tek paket P1-goodbudget-T01:** B05 kaydındaki gelir
yolu karşılaştırmasını mevcut household'da sentetik tutarla yap; değişiklikten önce
başlangıç durumunu kaydet, veriyi silme. G01 düzeltme adayları G01-5 bu pakete aittir. Paket listesi ve önerilen sıra P0.4 bölümündedir. Görsel incelemesi/canlı test başlatma; mevcut E/B kayıtlarına bağla.

## P0.3-money-manager — Tam metin içerik haritası

**13 Eylül 2026: E0010'un 379 satırının tamamı okundu.** Kaynak:
[Money Manager formu](gozlemler/money-manager.md). Okunan SHA-256:
`e9f6449a4b73c76a40cfd8bd559ff63ea3822dce8306574f7895d8566c7bf7ec`.
Kaynak dosya değiştirilmedi. Bu işlem **metin kapsam haritasıdır**, rakip
davranışlarının yeniden doğrulanması veya 30 görselin içerik incelemesi değildir.

### Kaynak bağlamı ve sınır

- Formun bildirdiği sürüm 4.12.8 AD / versionCode 1197; ücretsiz, reklamlı.
  Android emulator-5554, Türkçe/TRY; Android 17 bilgisi form beyanıdır, bu tur cihaz kontrol edilmedi.
- Tarih katmanları: 1 Eylül eski PC/İngilizce; 10 Eylül Türkçe/Ağustos verili
  boşluk koşumu; 12 Eylül Faz 7.5 ekleri. Temiz çekirdek durum ve sonraki
  taksit/tekrar/kısmi ödeme durumu birbirinin yerine kullanılamaz.
- Kanıt havuzu E0227–E0256: 30 PNG. Tam dosya adları KANIT-ENVANTERI.md'de.
  Bu aralık yalnız kapsam eşlemesidir; her görselin doğrulandığı anlamına gelmez.
- Formda doğrudan HTTP(S) kaynak adresi yok. Fiş/OCR bölümündeki eski resmî
  kaynak göndermesi URL taşımıyor; kaynak kaydı eksiği MM-Q07'de.
- Açıkça kapsam dışında bırakılmış: bütçe kurulumu, Excel export, yedek/restore.
  Widget denenmemiş; Türkçe gelir kategorisi ve swap/Fees doğrulanamamış.
  Bu sınırlar ürünün bu özelliklere sahip olmadığı şeklinde yazılamaz.
- Ayrı ürün kimliği ve sistem/pipeline başlıkları yok; malzeme oturum bilgisi,
  akış özeti ve Faz 1 alt başlıklarına dağılmış. P4'te bunlar bir araya getirilecek;
  eksik başlık, davranışın hiç incelenmediğini tek başına kanıtlamaz.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları bu okunan sürüme aittir. Belge 3 sütunu öneri onayı değil,
ileride onaylı Belge 1/2'den beslenecek karar konusunu gösterir.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| MM-M01 | Oturum bilgisi 3 | Sürüm, dil/kurulum farkı, kayıt gerektirmeme, ücretsiz/yerel çalışma | E0010; ayarlar için E0254 adayı | Belge 1 ürün profili; MM-Q01 |
| MM-M02 | Görev gözlemleri 16, K00–K04 | Onboarding yok; hazır hesapları düzenleme; ~7 adım form; seri giriş; not başlığı; kapsam yokluğu | E0227–E0230, E0232–E0233, E0235 | Belge 1 gezinme/form; Belge 2 kayıt; MM-Q07/Q08 |
| MM-M03 | Görev gözlemleri K05–K08; kontrol değerleri 30 | Kart harcaması gider, transfer/ödeme nötr, açılış gelire dahil değil; silme kalıcı; 44.950 çekirdek net | E0228, E0230–E0232, E0234–E0237 | Belge 2 çekirdek olaylar; MM-Q02/Q05/Q09 |
| MM-M04 | Arayüz taraması 50 | Beş zaman sekmesi; drill-down; bütçe bağlantısı; ayarlar; filtre; boş/hata; widget sınırı | E0227, E0232, E0234, E0244, E0250–E0251, E0254 | Belge 1 yüzey haritası; B18, MM-Q04/Q08/Q10 |
| MM-M05 | Arayüz incelemesi 63 | Renk, para biçimi, alt gezinme, gruplu liste, hesap makinesi, durum geri bildirimi ve reklam | E0227–E0234, E0254; ilgili görselle birebir eşleme henüz yok | Belge 1 desenler; MM-Q08 |
| MM-M06 | Akış özeti 77 | Günlük kişisel bütçe kimliği; kapsam, havale, kart dönemleri, planlama özeti | E0010; ilgili diğer MM-M kayıtları | Belge 1 ürün; Belge 2 özet; MM-Q05/Q07 |
| MM-M07 | Faz 1 86; kredi kartı modeli 90 | Kaynak hesap, kesim/ödeme günü; Bu Ay/Gelecek Ay; bizim modelle tam eşdeğerlik iddiası | E0233, E0236 | Belge 2 kart dönemi; MM-Q05 |
| MM-M08 | Kartın kendi defteri 114 | Ağustos üç hareket, ödeme/harcama yönü, yürüyen bakiye; Eylül 2/6 ve 400 ödeme | E0255–E0256; nakit defteri E0240 | Belge 1 defter; Belge 2 kart; MM-Q02/Q05 |
| MM-M09 | Kart Ödeme butonu 147; kısmi ödeme 208 | Ön dolu havale; 400 ödeme sonrası kesilmiş dönem 600, gelecek 1.000 | E0237, E0248–E0249, E0255–E0256 | Belge 2 ödeme; Belge 3 aday; MM-Q05 |
| MM-M10 | Tekrarlayan 157 | Geçmiş/bugün otomatik gerçekleşme, gelecek önizleme; tek occurrence silme, seri yönetimi aranmadı | E0238, E0241–E0244 | Belge 2 planlama; MM-Q03/Q09 |
| MM-M11 | Taksit 187 | 6.000/6; ilk ay 1.000; iki ekstrede toplam 2.000; sonraki dört borçta yok | E0245–E0247, E0255–E0256 | Belge 2 taksit; MM-Q03/Q05 |
| MM-M12 | Fiş/kamera 216; açılış bakiyesi 222 | Foto ekleme/OCR yok; bakiye farkı bugün tarihli; ana feed gösterimi seçimi | Kamera için doğrudan PNG belirtilmemiş; E0239–E0240 açılış | Belge 2 fiş/açılış; MM-Q07/Q09 |
| MM-M13 | Doğrulama 233 | Hesap eksikliğinde toast, sıfır tutar kaydı, kalıcı silme, taslak uyarısı yok | E0232 yalnız toast için aday; diğerleri metin koşum kaydı | Belge 1 hata; Belge 2 düzeltme; MM-Q08/Q09 |
| MM-M14 | Faz 7.5 245; iki iddia 251; anahtar 278; veri durumu 306 | Toplam kart satırı; havale başlıkları ters; toplam anahtarı açık/kapalı; 39.800 hesap, 42.350 net | E0250–E0254; kart E0255–E0256 | Belge 1/2 toplam/filtre; MM-Q01/Q02/Q04/Q06 |
| MM-M15 | BusinessFinance kararları 313 | Mevcut davranış benzerlikleri, önizleme, taksit, kasa açılışı, doğrulama ve kapsam tercihleri | E0010 karar tablosu; ilgili MM-M kanıtları | Yalnız Belge 3 için aday; MM-Q05/Q06/Q07/Q08 |
| MM-M16 | Kanıt/güven 335; tek cümlelik sonuç 377 | Tarihsel kapsam, 27 kare iddiası, doğrulanamayanlar, veri sapması ve nihai hüküm | E0010; E0227–E0256 kapsamı | Belge 1/2 yöntem; B17/B19, MM-Q01/Q02/Q10 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

MM-Q kimlikleri P0.3'te metinden çıkarılan sorulardır. B01–B19'u yeniden
numaralandırmaz; kapanmış bulgu veya canlı test sonucu değildir. Önce ilgili
görsel/metin/kod kontrolü yapılır; yalnız cevaplanamayan önemli davranış için
canlı test açılır. Bu pakette hepsinin durumu **açık**.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| MM-Q01 | Eski İngilizce kareler mi, Türkçe yenileri mi? 27 kare tamamlandı ifadesi mevcut 30 kareyle hangi tarihte örtüşüyor? | E0227–E0256 kaynak/tarihlerini ve 31/32 eklerini ayır; eksik 05 numarasını kayıp dosya sayma; eski kapanışı yeni tüm-görsel onayı sayma | B17/B19; G paketi |
| MM-Q02 | Güven bölümündeki 39.200 ile veri durumundaki 39.800 aynı checkpoint mi? 44.950, 42.350 ve 38.200 hangi durumlara ait? | E0235 çekirdek, E0252/E0253 anahtar, E0255/E0256 defter ve tekrar silme zincirini zaman sırasına koy; farkı varsayımla düzeltme | G paketi; gerekirse hedefli T |
| MM-Q03 | 10 Eylül koşumunda 15 Eylül taksiti borçta iken gelecekteki tekrar önizleme: taksit üretim eşiği tam olarak ne? | E0241–E0247/E0256 ile gerçekleşme, ekstreye yansıma ve gün/ay eşiğini ayır; tekrar ve taksitin otomatikliğini tek kurala genelleme | Belge 2 karar engeli; G sonra gerekirse T |
| MM-Q04 | Havale yönlerinin tersliği yalnız Türkçe çeviriye mi özgü? Toplamdaki 2.200 harcama mı borç mu? | E0250/E0251'i akış ve dönemle eşle; eski İngilizce notu aynı sürüm dil karşılaştırması yerine koyma; etiket hatası ile para hesabını ayır | Belge 1/2; G paketi |
| MM-Q05 | Bu Ay/Gelecek Ay tam ekstre eşdeğerliği ve kısmi kart ödemesinin D3 ile uyumu ne kadar kanıtlı? | Aynı gün/dönem koşulları ile kart modelini ve BF kodunu ayrı kontrol et; kart ödeme davranışından cari faturaya tahsis garantisi çıkarma | B01 ilkesi; Belge 2/3; kaynak/kod kontrolü |
| MM-Q06 | Toplama Dahil Et varsayılan mı; hiç uyarı yok mu; Bluecoins Nakit Akım Ayarı aynı metriği mi etkiliyor? | E0252/E0253'te gözlenen durum ile kurulum varsayılanını ayır; gri satır göstergesini dikkate al; net varlık ve nakit akışını eşitleme; BF toplam kapsamını kodla doğrula | B10/B16 ile çapraz; G/kod |
| MM-Q07 | Kapsamın hiçbir dolaylı yolu yok, OCR/fatura yok, tamamen yerel hükümleri hangi sınırda? | Formda etiket/ikinci defter yokluğu ve fiş resmî kaynağının dayanağını bul; PC'den Yönet/export yüzeyini veri yerelliğinden ayır; aranmadı/yok ayrımı. Yeni dış servis bağlantısı kurma | Belge 1/2 özellik haritası; kaynak kontrolü |
| MM-Q08 | ~7 dokunuş, ekran okuyucu riski, hiç yanlış okuma riski yok gibi ifadeler ölçüm mü yorum mu? Karar etiketleri ve yalnız ADR gerekçeli Alma satırları uygun mu? | Görev adımı ve gözlenen arayüzle sınırlı anlat; kullanılabilirlik/erişilebilirlik deneyi olmuş sayma; Not etiketini karar sonucuyla karıştırma; al/alma kararını Belge 3 onayı sayma | Belge 1/3; metin/etiket kontrolü |
| MM-Q09 | Sıfır tutar, geri dönüşsüz silme, geçmiş açılış tarihi ve uyarısız çıkış için yeterli kanıt var mı? | E0232'nin yalnız toast gösterdiğini ayır; eski koşum kaydını değerlendir; gerekirse tek soruluk güvenli T. Veri sıfırlama/silme veya e-posta gönderimi kendiliğinden yapılmaz | Belge 2 düzeltme/açılış; G sonra T |
| MM-Q10 | Dört görselin tam ad eksiği ve üç-sütun/dört-hücre tablosu nasıl kapanacak? | E0242/E0243/E0246/E0249'un formda tam adını ve tüm kısa kullanımları kontrol et; tablo kanıt başlığını düzeltme paketinde doğrula. Bu pakette düzeltme yok | B17/B18; P1-B18 ve mekanik kapı |

### Bu paket sonrası kapsam

- Okundu: E0010 tam metin. İçerik haritası MM-M01–MM-M16; açık sorular MM-Q01–MM-Q10.
- Yeni görsel incelemesi: **0/30**. Canlı test: **0**. Kapanan B bulgusu: **0**.
- B17/B18 Money Manager'a doğrudan bağlı; B19 özet ve tarihsel kapanış yayılımı.
  Diğer B bağlantıları ortak kontrol ilkesidir, otomatik yeni hata kanıtı değildir.
- Bütçe/export/restore kapsamı değiştirilmedi. P0.4 görsel paketini aynı
  akış/tarih bağlamına göre planlar; henüz bir G paketinin kapandığı yazılamaz.

## P0.3-hesap-defterim — Tam metin içerik haritası

**14 Eylül 2026: E0007'nin 367 satırının tamamı okundu.** Kaynak:
[Hesap Defterim formu](gozlemler/hesap-defterim.md). Okunan SHA-256:
`789f19dbb779373615f50303c0dae8f9691e3873f111c85384056e4d3dcfc68f`.
Kaynak dosya değiştirilmedi. Bu paket **metin kapsam haritasıdır**; eski
koşumları yeniden yapmış veya rakip davranışlarını doğrulamış sayılmaz.

### Kaynak bağlamı ve sınır

- Formun bildirdiği ürün ANKIT SARAF / Hesap Defterim (Cash Book), sürüm
  235 / versionCode 235, targetSdk 36; ücretsiz ve reklamlı. Android
  emulator-5554, Türkçe; para birimi simgesi görülmemiş. Android 17, mağaza
  ölçüleri ve 23 Ağustos güncellemesi bu tur doğrulanmış bilgi değil,
  formun tarihsel beyanıdır. Aynı adlı diğer ürünlerle karıştırılmamalı.
- Tarih katmanları: 10 Eylül çekirdek + A/B/B1/B2; 11 Eylül ek koşum 2 ve
  silinip yeniden kurulan iki kayıt; 12 Eylül Faz 7.5 ekleri. İşlem dönemi
  Ağustos 2026; ekran yakalama tarihiyle aynı şey değil. 44.950 ara/çekirdek
  durum, 47.300 ise formun son bildirdiği birleşik bakiyedir; bugünkü cihaz
  durumu bu pakette kontrol edilmedi.
- Kanıt havuzu **E0134–E0178: 45 PNG**. Tam dosya adları
  [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Eşlemeler metin atıflarına
  dayanır; görseller bu pakette açılmadı.
- Formda doğrudan HTTP(S) kaynak URL'si yok. Mağaza ve uygulama içi diyalog
  kaynakları E0134/E0149'a bağlanmış; mağaza URL'si/paket kimliği ve kaynak
  güncelliği sonraki kontrolün konusu. Bu paket dış kaynak taraması yapmaz.
- Açık sınırlar: Drive hesabı bağlanmamış, gerçek yedek/restore denenmemiş,
  `Verileri sil` açılmamış; yıl/hafta başlangıcı diyalog içerikleri
  doğrulanmamış. Excel içeriği protokol gereği kapsam dışında; PDF için eski
  doğrulama beyanı var. Galerinin boşluğu emülatör koşulu olarak yazılmış.
- `Açıklama / kategori ekle` son kayıtta açık bırakılmış; `İşlem adları`
  denemesi geri alınmış. Görünen ayar ile kurulum varsayılanı ayrılır.
  Ayrı Veresiye Defteri / Gelir Gider uygulamaları kurulmamış; geliştirici
  niyeti ve ürünler arası veri ayrılığı kesinleşmiş sayılmaz.

### Başlık → iddia → kanıt → rapor haritası

Satırlar okunan sürüme aittir. Belge 3 bağlantıları öneri onayı değil,
ileride onaylı Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| HD-M01 | Oturum bilgisi 3 | Ürün/sürüm, kayıt istememe, yerellik, reklam, mağaza ölçüleri | E0134–E0136, E0149, E0174 | Belge 1 profil; HD-Q01/Q06 |
| HD-M02 | Ürün kimliği 17 | Yürüyen bakiye defteri; hedef esnaf; ayrı uygulama ailesi ve bilinçli kapsam bölünmesi | E0007; E0177 çapraz tanıtım adayı | Belge 1 ürün; Belge 2 kapsam; HD-Q07/Q12 |
| HD-M03 | Görev gözlemleri 29, K00–K04 | İki eylem; bağımsız defter; seri giriş; not başlığı; kapsam/kategori yokluğu | E0135–E0138, E0145–E0146, E0173 | Belge 1 gezinme/form; HD-Q07/Q11 |
| HD-M04 | Görev gözlemleri K05–K08 | Negatif kart defteri, Aktar, birleşik liste/özet, düzenleme/taşıma/kopyalama/çöp kutusu | E0140–E0143, E0147–E0148, E0159 | Belge 2 kart/transfer/düzeltme; HD-Q02/Q03/Q08 |
| HD-M05 | Kontrol değeri 43 | 40.800/4.150/0 ve 44.950; 51.200 Alındı, 6.250 Ödendi içinde açılış ve transferler | E0137, E0140, E0142, E0148, E0173 | Belge 2 toplam anlamı; HD-Q02/Q04 |
| HD-M06 | Ek koşum A/B/B1/B2 69 | 400 kısmi ödeme; salt dosya eki; tekrar/taksit negatif taraması | E0153, E0157, E0150, E0171, E0177–E0178 | Belge 2 ödeme/fiş/planlama; HD-Q07/Q08/Q12 |
| HD-M07 | Ek bütünlük bulgusu; geçiş reklamı (Ek koşum) | Transfer bacağı silindiğinde diğerinin kalması; düzenleme/tarih genellemesi; reklam | E0154, E0174 | Belge 2 bütünlük; Belge 1 reklam; B12, HD-Q03/Q11 |
| HD-M08 | Ek koşum 2 94; metodoloji notu | 150 gider + 2.500 gelir; geçici silme, yeniden kurma ve veriyi koruma kuralı | E0170 ara, E0172 son kontrol; E0155–E0159 kayıt adayları | Belge 1/2 yöntem; HD-Q02 |
| HD-M09 | Ek koşum 2 tablosu: öğe/ek/özel ad | Kalem toplamı ve notun doldurulması; ek görüntüleme; global etiket değişimi | E0139, E0155–E0161 | Belge 1 form; Belge 2 belge eki; HD-Q05/Q07/Q12 |
| HD-M10 | Ek koşum 2 tablosu: doğrulama/arama/silme | Boş tutar sessiz ret, sıfır kabul; aramada toplamlar; çöp kutusundan kalıcı silme | E0162–E0164, E0167–E0168 | Belge 1 geri bildirim; Belge 2 düzeltme/filtre; HD-Q08/Q12 |
| HD-M11 | Ek koşum 2: Bildiri ve kasadefteri uyarısı | İki export yolu; PDF içerik beyanı, iki dosya paylaşımı, yanlış klasör metni | E0169, E0175–E0176; çıktı dosyasına doğrudan atıf yok | Belge 2 dışa aktarma; HD-Q05 |
| HD-M12 | Ek koşum 2: Not Defteri/Nakit Hesap Makinesi | Görev listesi ve bağımsız kupür hesabı; işlem üretmeme iddiası | E0165–E0166 | Belge 1 yardımcı yüzeyler; Belge 2/3 kasa; B14, HD-Q09 |
| HD-M13 | Faz 7.5 137 | Takvim düzeltmesi, 17 menü, ayar alt bölümü, atıf düzeltmeleri; 47.300 kontrol, ayar/veri devri | E0144, E0150–E0151, E0171–E0178 | Belge 1/2 yöntem; B11, HD-Q01/Q02/Q06/Q10 |
| HD-M14 | Açılış bakiyesi 194 | Döneme göre Önceki denge veya Alındı; görünürlük ayarı | E0152, E0137; ayar E0150 | Belge 2 açılış/dönem; HD-Q04 |
| HD-M15 | Arayüz taraması 208; incelemesi 227 | Drawer/defter seçici, takvim, dönem, yürüyen bakiye, renk/para biçimi, boş/hata/erişilebilirlik | E0136–E0139, E0143–E0152, E0162–E0169, E0171, E0174–E0178 | Belge 1 yüzey/desen; HD-Q04/Q06/Q10/Q11 |
| HD-M16 | Sistem işleyişi / pipeline 240 | Satır türevi raporlar; fiziksel tablo ve transfer saklama iddiaları; yerel veri/Drive/e-posta | E0007; E0140, E0147, E0149, E0154, E0177–E0178 | Belge 2 olay modeli; B11/B12, HD-Q03/Q06/Q07 |
| HD-M17 | Akış özeti 258 | Hızlı giriş, rapor sınırı, kapsam/kart/planlama özeti | E0007; ilgili HD-M kayıtları | Belge 1/2 özet; HD-Q04/Q07/Q11 |
| HD-M18 | BusinessFinance için kararlar 274 | Denge/seri giriş/öğe adayları; iptal, kısmi ödeme, filtre, kasa, dönem ve e-posta kıyasları | E0007 karar tablosu; ilgili HD-M kayıtları | Yalnız Belge 3 adayları; B11/B12/B14, HD-Q09/Q12 |
| HD-M19 | Kanıt ve güven 318; sonuç 359 | 10–12 Eylül tarihsel kapsam, 38 kare beyanı; negatifler, denenmeyenler ve ürün bölünmesi hükmü | E0007; E0134–E0178 kapsamı | Belge 1/2 yöntem; B17/B19, HD-Q01/Q07/Q11 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

HD-Q kimlikleri metinden çıkarılan sorulardır; mevcut B01–B19'u yeniden
numaralandırmaz. **HD-Q01–HD-Q12'nin tamamı açık.** Önce mevcut kanıt/kaynak
değerlendirilir; gerekli kalırsa tek soruluk canlı test planlanır. Bu paket
hiçbir davranış testini başlatmaz.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| HD-Q01 | 38 karelik tarihsel inceleme, 36–43 ekleri ve mevcut 45 PNG nasıl örtüşüyor? Sürüm/mağaza bağlamı yeterli mi? | E0134–E0178'i tarihle eşle; yeniden çekilen 09 ve 06b/28b eklerini ayır. Mevcut ağaçta silinmiş 23'ü yeniden üretme veya yeni kayıp ilan etme; önceki 38 kare onayını bugünkü tüm-korpus onayı sayma. Paket kimliği/mağaza URL'sini gerekirse kaydet | B17/B19; G/kaynak |
| HD-Q02 | 40.800 yerine 40.400 ifadesi net varlık mı Ana Hesap mı? 44.950 ara ve 47.300 son kontrol zinciri tutuyor mu? | E0153/E0154 ve E0170/E0172'yi defter/birleşik görünüm ve koşum sırasıyla eşle; K04'teki 1.150 ile kontrol 4.150'yi transfer öncesi/sonrası ayır. E0148'in kartın sıfır bakiyesine gerçekten kanıt olup olmadığını incele. Eski sayıyı bugünkü cihaza taşıma | Belge 2 sonuç engeli; G, gerekirse T |
| HD-Q03 | Tek bacak silme, tutar/tarih düzenleme ve saklama modeli aynı kanıtla mı destekleniyor? | E0154'teki etkiyi eski koşumla bağla; silmeyi düzenlemeye genelleme. Fiziksel tablo/atomiklik sonucu çıkarma; ikinci rapor tablosu yok iddiasını B12 kapsamında sınırla. Yeni silme deneyi kendiliğinden yapılmaz | B12; P1-B12/G, gerekirse izinli T |
| HD-Q04 | Alındı/Ödendi'yi gelir/gider diye adlandırmak ve karışık toplamı ürün hatası saymak hangi sınırda doğru? | E0140/E0142/E0148/E0152'de görünüm/dönemleri eşle; ürünün kendi iddiası ile BF rapor ihtiyacını ayır. Açılış devri, görünürlük ayarı ve arama toplamları ayrı davranışlardır; Tasarruf/net/Denge eşitliğini dönemden bağımsız yazma | Belge 2 toplam/açılış; G |
| HD-Q05 | PDF içeriği/etiket değişimi/gerçek dosya yolu için kalıcı çıktı kanıtı nerede? Excel ne kadar denenmiş? | E0169/E0175/E0176 seçim/uyarı ekranları dosya içeriği kanıtı değildir. Eski PDF kontrolünün dayanağını bul veya anlatımı sınırla; iki dosyanın türünü varsayma. Excel içeriği kapsam dışı; kasadefteri yokluğu gözlenen cihaz koşuluyla sınırlı | Belge 2 export; G/kaynak |
| HD-Q06 | Açık e-posta ayarı varsayılanı veya gerçekleşmiş gönderimi kanıtlıyor mu; tamamen yerel iddiasının sınırı ne? | E0178'de durum, kurulum varsayılanı, alıcı/tetikleyici/kanal ve gönderimi ayır; E0149 diyalog beyanı ağ denetimi değildir. Drive/export temaslarını yerellikle uzlaştır. Ayrı kullanıcı izni olmadan e-posta gönderme veya dış hesap bağlama | B11; P1-B11/G/kaynak |
| HD-Q07 | Kategori/kapsam/OCR/cari/planlama yokluğu ne kadar tarandı; ayrı uygulamalar bilinçli bölünmeyi kanıtlıyor mu? | Arandı-yok, denenmedi ve yorum ayrımını koru; serbest metni yapılandırılmış kategoriyle eşitleme. E0157 ek seçenekleri tüm üründe OCR yokluğunu tek başına kanıtlamaz. E0177 tanıtımından diğer ürünlerin veri/yedek modeli veya geliştirici niyeti kesinleşmez; banka/POS gibi negatiflerin dayanağını belirt | Belge 1/2 kapsam; G/kaynak; B19 yayılımı |
| HD-Q08 | Sessiz ret, taslak kaybı, taşıma/kopyalama ve iki aşamalı silme için kanıt yeterli mi? | E0162 tek karede toast yokluğu süre boyunca uyarı yokluğunu kanıtlamaz; koşum kaydıyla birlikte değerlendir. E0159'un K08 düzenleme/taşıma iddiasını ne kadar gösterdiğini kontrol et. İşlem çöp kutusu ile Not Defteri kalıcı silmesini ayır; yeni test veriyi korur | Belge 1/2 düzeltme; G, gerekirse T |
| HD-Q09 | Kupür yardımcısının eksikliği ile fiziksel kasa sayımı ihtiyacının eksikliği karıştırılıyor mu? | E0166'yı B14'teki CashCount/domain ve istemci kontrolüne bağla; sayım/uzlaştırma ile kupür×adet aracını ayrı kıyasla. Öncelik mevcut özellik yokluğu varsayımından türetilmesin | B14; P1-B14; Belge 2/3 kasa |
| HD-Q10 | Dönem ayarının varlığı gerçek sınır değişimini kanıtlıyor mu; takvim/yürüyen bakiye hangi ayarda? | E0144/E0150/E0178 ile görünen kontrolü/tarihi kaydet; yıl/hafta diyalogları denenmedi. Gün 1 ile özel dönem davranışını ayır; takvimde tek net sayı/yön rengini görselle kontrol et | Belge 1/2 dönem; G, gerekirse T |
| HD-Q11 | ~3–4/~5 adım, öğrenilebilirlik, büyük hedefler, ondalık yokluğu ve reklam finansmanı ölçüm mü yorum mu? | Minimum giriş ile görev adımlarını ayır; kullanıcı/ekran okuyucu deneyi yapılmış sayma. Tam sayı veriden ondalık desteği yok sonucu çıkarma; mağaza erişimini reklamın nedensel sonucu yapma. Renk/simge gözlemini ayar/sürümle sınırla | Belge 1 anlatım sınırı; G/metin |
| HD-Q12 | BF karar etiketleri, D3/ekstre, iptal-geri yükleme, fiş satırı ve filtre toplamı kıyasları uygun mu? | README karar sözlüğü ve BF kod/ADR gerçeğiyle kontrol et. Kartın kısmi ödemesi cari faturaya tahsis kanıtı değildir; çöp kutusu iptal semantiğiyle eşitlenmez. Sıfır tutar/kalem dökümü/filtre önerileri ürün kararı sayılmaz; toplamların kapsamını koru | B01 kontrol ilkesi, B11/B12/B14; Belge 3 adayı; kaynak/kod |

### Bu paket sonrası kapsam

- Okundu: E0007 tam metin; HD-M01–HD-M19 ve HD-Q01–HD-Q12 kaydedildi.
- Yeni görsel incelemesi **0/45**, canlı test **0**, kapanan B bulgusu **0**.
- B11/B12/B14 doğrudan bağlar; B17 atıf/mekanik kapı, B19 özetlere yayılım
  işidir. B01 bağlantısı kıyas ilkesi olup yeni hata doğrulaması değildir.
- Gözlem formu, görseller ve test araçları değişmedi; emülatörde veri/ayar
  değişikliği yapılmadı. Drive/Excel kapsamı genişletilmedi.
- Yarım metin bölümü yok. Sıradaki tek paket **P0.3-goodbudget (E0006)**;
  P0.4 ve G/T paketleri henüz başlamadı.


## P0.3-goodbudget — Tam metin içerik haritası

**14 Eylül 2026: E0006'nın 293 satırının tamamı okundu.** Kaynak:
[Goodbudget formu](gozlemler/goodbudget.md). Okunan SHA-256:
`02a036673d455696d12d388ed1625e4ad239701f4e758ee22ce17b872d2437b7`.
Kaynak form değişmedi. Bu işlem **metin kapsam haritasıdır**; eski koşumların
yeniden yapılması, görsel içerik onayı veya B05–B07'nin kapanışı değildir.

### Kaynak bağlamı ve sınır

- Formun bildirdiği ürün Goodbudget / Dayspring Technologies, sürüm
  2.24.26013 (180); Pixel 8 AVD / Android 17. İngilizce uygulama, Türkçe
  sistem; ABD sayı/tarih biçimleri ve Türkçe rapor tarihleri birlikte
  kaydedilmiş. Bunlar form beyanıdır; bu tur cihaz/sürüm kontrol edilmedi.
- Test edilen plan ücretsiz; form 10 zarf ve bütün türler için toplam
  1 hesap sınırı bildiriyor. K05/K06/A kart/transfer davranışları paywall
  nedeniyle tamamlanmamış. Ürün genelinde hesap türü veya banka özelliği
  yokluğu bu sınırlı koşumdan çıkarılamaz (B07).
- Tarih katmanları: 11 Eylül Tur 1 ve ek koşum; 12 Eylül Faz 7.5 ekleri.
  Çekirdek kayıtlar Ağustos; Initial Envelope Fill 11 Eylül; rapor aralığı
  kimi yerde Temmuz–Eylül. B1 için aylık protokol yerine Every 2 Weeks
  kullanıldığı sonradan düzeltilmiş. Bu fark korunur.
- Kanıt havuzu **E0106–E0133: 28 PNG**. Tam dosya adları
  [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Aşağıdaki eşlemeler metnin
  atıflarıdır; hiçbir görsel bu pakette açılmadı.
- Kaynak formda doğrudan HTTP(S) URL'si yok; kendi güven bölümünde resmî
  kaynak taranmadığı yazıyor. B05/B07'deki dış kaynaklar ayrı denetim
  katmanıdır: [gelir yolu](https://goodbudget.com/help/getting-started-guide/step-3-add-income/)
  ve [banka senkronizasyonu](https://goodbudget.com/help/automatic-bank-sync/how-does-automatic-bank-sync-work/).
  Bu iki URL mevcut B kayıtlarından devralındı; bu tur açılmadı ve güncel
  ürün doğrulaması sayılmaz.
- Settings hiç açılmamış; widget denenmemiş; Advanced Search bağlantısı
  görülmüş ama ayrıntılı davranışı belgelenmemiş. Export/yedek akışları
  protokol kapsamı dışında. Household kimliği veya kişisel kayıt bilgisi
  bu haritaya kopyalanmaz; emülatörde yeni veri/ayar işlemi yapılmadı.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. Belge 3 hedefleri öneri onayı değil,
ileride onaylı Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| GB-M01 | Oturum bilgisi 3 | Sürüm, ücretsiz plan, sistem/uygulama dili ve hesap erişimi | E0106, E0110, E0114 | Belge 1 profil; B07, GB-Q06/Q10 |
| GB-M02 | Ürün kimliği 16 | Zarf bütçesi; hedef aile; hesap türü/kart/banka yokluğu; aşırı harcamayı sınırlama tezi | E0006; E0109, E0112, E0115 adayları | Belge 1 ürün; Belge 2 kapsam; B07, GB-Q06/Q09/Q12 |
| GB-M03 | Görev gözlemleri 27, K00–K02 | Önce bütçe/doldurma sonra kayıt; LATER; iki Available; opsiyonel hesap katmanı ve paywall | E0106–E0115 | Belge 1 onboarding/ana ekran; GB-Q06/Q07/Q10 |
| GB-M04 | Görev gözlemleri K03–K04 | Expense/Credit formu, zorunlu zarf, 25.000 Market Credit ve 850 gider | E0116–E0117, E0130 | Belge 2 gelir; B05, GB-Q01/Q02 |
| GB-M05 | Görev gözlemleri K05–K06 | Kart yerine Ana Hesap gideri; ayrı transfer formu; tek hesap engeli | E0114, E0119–E0120 | Belge 1 transfer formu; Belge 2 erişim sınırı; B07, GB-Q06 |
| GB-M06 | Görev gözlemleri K07–K08 | Dönem değiştirme, negatif harcama; düzenleme/silme ve 16 farkı | E0121–E0125, E0127, E0129 | Belge 1 rapor/düzeltme; Belge 2 sonuç; B05/B06, GB-Q02/Q04 |
| GB-M07 | Kontrol değeri 43 | Hesap/zarf toplamı ayrımı; 42.950 → 42.334 → 41.734; 16 ve 600 farkları | E0120, E0125, E0129–E0132 | Belge 2 mutabakat; B06, GB-Q03/Q04/Q07 |
| GB-M08 | Rapor davranışı 87 | Initial Envelope Fill 2.050; Ağustos −22.950; Market satırı görünmezken tutarı toplamda | E0121, E0123–E0124, E0130 | Belge 2 fonlama/rapor; B05, GB-Q01/Q02 |
| GB-M09 | Ek koşum 114 | A engelli; ek/OCR yokluğu; Every 2 Weeks, ilk örnek, e-posta hatırlatması; taksit/split ayrımı | E0114, E0116, E0126–E0128, E0133 | Belge 2 kart/tekrar/fiş; B06/B07, GB-Q03/Q06/Q08 |
| GB-M10 | Faz 7.5 129 | Sıklık/kayıt sırası düzeltmeleri; 24 kare beyanı; 02b yeniden adlandırma, 23–26 ekleri, veri korunması | E0107–E0110, E0124, E0126, E0130–E0133 | Belge 1/2 yöntem; GB-Q03/Q10 |
| GB-M11 | Arayüz taraması 172 | Dört sekme, rapor drill-down, arama, boş/hata/paywall; Settings açılmadı; konum/widget seçenekleri | E0109, E0114–E0118, E0121–E0123, E0127, E0130–E0132 | Belge 1 yüzey haritası; GB-Q08/Q09/Q11 |
| GB-M12 | Arayüz incelemesi 186 | Zarf kalan/bütçe/çubuk, iki satırlı liste, renk, hesap makinesi, başarı metni ve erişilebilirlik yorumu | E0115–E0116, E0121–E0124, E0130; bazı iddialar yalnız metin | Belge 1 desenler; GB-Q09/Q11 |
| GB-M13 | Sistem işleyişi / pipeline 199 | Hesap/zarf güncellenmesi, Initial Fill, tekrar nedenselliği, entegrasyon yokluğu ve bulut beyanı | E0111, E0115–E0116, E0126, E0129–E0132 | Belge 2 olay modeli; B05/B06/B07, GB-Q01/Q03/Q07/Q08 |
| GB-M14 | Ayrı Fill Envelopes akışı 217; akış özeti 222 | From New Income / From Available yolu; özetin geliri yine zorunlu zarfa bağlaması | E0006 metin; E0108/E0110 kurulum adayları, tam gelir akışı kanıtı değil | Belge 2 giriş yolları; B05, GB-Q01 |
| GB-M15 | BusinessFinance için kararlar 235 | Zarf/gelir/tekrar/silme reddi; hesap katmanı, transfer, konum, mikro-metin adayları | E0006 karar tablosu; ilgili GB-M kayıtları | Yalnız Belge 3 adayları; B05/B06/B07, GB-Q12 |
| GB-M16 | Kanıt ve güven 262 | 22 kare beyanı, resmî kaynak yok; 600 nedeninin ve 16 iç mekanizmasının bilinmemesi; kapsam sınırları | E0006; E0106–E0133 kapsamı | Belge 1/2 yöntem; B06/B17/B19, GB-Q03/Q04/Q10 |
| GB-M17 | Tek cümlelik sonuç 284 | Gelir, fonlama, silme ve liste dışı tekrar hükümlerinin kesin sonuçta birleşmesi | E0006; GB-M04/M07/M08/M09 dayanakları | Belge 1/2 özet; B05/B06/B19, GB-Q05/Q12 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**Bu ilk haritanın yazıldığı tarihte GB-Q01–GB-Q12'nin tamamı açıktı.** Sonraki kapanışlar
14 Eylül Goodbudget kapanışında ve 20 Eylül K3 ekinde kayıtlıdır. Bunlar metinden çıkarılan kontrol sorularıdır;
B01–B19'u yeniden numaralandırmaz. Önce mevcut kanıt/kaynak değerlendirilir;
önceden tanımlı B05/B06 testleriyle aynı iş ikinci kez açılmaz.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| GB-Q01 | Credit yolunun sonucu bütün gelir girişlerine genellenebilir mi; Initial Fill hangi fonlama yoluyla oluşturuldu? | K03 ile formun ayrı From New Income/From Available kaydını ve B05 resmî dayanağını uzlaştır. Kontrollü karşılaştırmada Keep Unallocated, hesap, zarf, gelir/gider etkisini ayrı tut. Kurulumdaki 2.050'nin yolu belirlenmeden bütün Fill işlemlerine hüküm verme | B05; P1-B05 / mevcut P1-goodbudget-T01 |
| GB-Q02 | −22.950 hangi kayıt anına/döneme ait; 600 tekrar rapor çekildiğinde var mıydı? | E0121/E0123/E0124/E0130'u zaman sırasına koy. K07'nin 2.650 gider ifadesi ile 1.200+850−25.000 hesabını açıklığa kavuştur; tarihler farklıysa aynı checkpoint sayma. Market tutarının toplamda olması, satır görünürlüğü ve normal gelir yolunun sonucu ayrı kalsın | B05/B06; G sonra gerekirse T01/T02 |
| GB-Q03 | 600 farkı gerçekten sonraki tekrar mı; Every 2 Weeks için üretim zamanı/önizleme neyi gösteriyor? | E0126/E0129–E0133'te bakiye, liste, arama, dönem ve senkronizasyon sınırını ayrı kaydet. Tek Bulut sonucu neden kanıtı değildir; geçmiş tarihli plan ve ilk kayıt ile sonraki üretimi ayır. Onay hiç sorulmuyor veya plan liste dışı kayıt üretiyor hükümlerini kanıtsız kesinleştirme | B06; P1-B06 / mevcut P1-goodbudget-T02 |
| GB-Q04 | 16 → 13 → sil zincirinde hangi adımın hangi dış etkisi kanıtlı? | E0120/E0125/E0129'u aynı hesap/zarf başlangıcıyla bağla; düzenlemenin ✓ öncesi/sonrası kalıcılığı ve silmenin etkisini ayır. 16 farkını 600'ün açıklaması sayma; iç mekanizma bilinmiyor kaydını koru. Yeni silme/sıfırlama kendiliğinden yapılmaz | B06 ayrı senaryo; G, gerekirse izinli T |
| GB-Q05 | Negatif rapor, bakiye sapması ve eksik kayıt kelimeleri kullanıcı hatası/ürün hatası ayrımını aşmış mı? | B05 gelir yolu ve B06 nedensellik kapanmadan bu sonuçları kesin ürün kusuru veya BF kararının doğrulanması sayma. Gözlenen değer, koşum uyarlaması ve yorum ayrı yazılsın; iyi/bozuk bütün hesaplamalar genellemesine gitme | B05/B06/B19; metin ve özet yayılımı |
| GB-Q06 | Hesap türü/kart/banka/taksit yokluğu ücretsiz erişim engeliyle karıştırılıyor mu? | E0112 üç hesap grubunu, E0114 paywall'u gösteren adaylardır. K05'te Ana Hesap gideri kart testi sayılmaz; transferin ayrı formu raporda nötrlüğü kanıtlamaz. B07 resmî paket kaynağını kontrol et; banka bağlantısı kurma. Debt türünden fatura tahsisi varsayma; hesap türü yok hükmünü uzlaştır | B07; P1-B07/G/kaynak |
| GB-Q07 | İki Available satırı, Total/All Accounts ve hesap olmadan bütçe davranışı ne kadar açıklanmış? | E0111/E0113/E0115/E0130/E0131'i hesap açılışı ve fonlama sırasıyla karşılaştır; iki Available için aylık/aylık olmayan tahmini kesinleştirme. Bağımsız katman demek bakiyeler arası her ilişkinin yokluğu veya iki fiziksel tablo kanıtı değildir | Belge 2 para modeli; G/kaynak; B12 kontrol ilkesi |
| GB-Q08 | Konum/household/e-posta/widget seçeneklerinden gerçek dış davranış sonucu çıkıyor mu? | E0110/E0115/E0118/E0126'daki diyalog ve göstergeleri fiilî öneri, teslim, yedek/restore veya tüm veri yerleşimi kanıtından ayır. Hatırlatıcı teslimi/konum doğruluğu denenmiş sayılmaz; izinsiz mesaj gönderme veya hesap/servis bağlama. Export/yedek kapsamı genişlemez | Belge 1/2 entegrasyon sınırı; B11 ortak ilke; G/kaynak |
| GB-Q09 | Settings hiç açılmamışken yerelleştirme/fiş/hedef/planlama yokluğu ne kadar kesin? | Menüde Help görülmesi yalnız incelenen yüzeyin kanıtıdır. Dil/para ayarı ve hedef yokluğu ile bulunamadı ayrımını koru; widget ve Advanced Search görünürlüğünü çalışır davranış sayma. Başka uygulamada yok iddialarını bütün örneklem tamamlanmadan kesinleştirme | B07 kapsam ilkesi; Belge 1/2; G/kaynak |
| GB-Q10 | Faz 7.5'teki 24 kare ile güven bölümündeki 22 kare ve mevcut 28 PNG nasıl örtüşüyor? | E0106–E0133 kaynak/tarihlerini, 02b/02c ve 23–26 eklerini eşle. Eski 02b adını yeni kayıp dosya sayma; kısa atıfları da tam kimlikle doğrula. Eski inceleme sayısını yeni bütün-görsel onayı sayma; kayıt atlama iddiasını yalnız ilk kurulum sınırında değerlendir | B17/B19; G/mekanik kapı |
| GB-Q11 | Adım sayısı, hızlı/doğru arama, büyük hedefler ve mikro-metin yorumları hangi ölçüme dayanıyor? | Görev adımı ile minimum girişi ayır; kullanıcı/ekran okuyucu testi yapılmış sayma. Başarı mesajı ve hesap makinesi için yalnız metin kalan dayanakları işaretle; gözlenen ABD biçimini bütün cihaz/ayarlar için evrenselleştirme | Belge 1 anlatım sınırı; G/metin |
| GB-Q12 | Zarfın aşırı harcamayı fiilen durdurduğu, gelir dağıtımını zorunlu kıldığı ve BF'de karşılığı olmadığı önerileri geçerli mi? | Negatif zarf davranışı denenmeden engelleme garantisi yazma; bütçe disiplini ile teknik engeli ayır. README karar sözlüğü ve BF kod/ADR gerçeğini kontrol et; B05/B06/B07'ye bağlı karar/sonuçları B19'a yay. Formdaki Alma/Not ifadeleri Belge 3 onayı değildir | Belge 3 adayları; B05/B06/B07/B19; kaynak/kod |

### Bu paket sonrası kapsam

- Okundu: E0006 tam metin; GB-M01–GB-M17 ve GB-Q01–GB-Q12 kaydedildi.
- Yeni görsel incelemesi **0/28**, canlı test **0**, kapanan B bulgusu **0**.
- B05/B06/B07 doğrudan bağlar; B17 atıf/mekanik kapı, B19 özet yayılımıdır.
  B11/B12 bağlantıları ortak kontrol ilkesi olup yeni hata doğrulaması değildir.
- Kaynak form, görseller ve test araçları değişmedi; veri/ayar işlemi yapılmadı.
  Kart/transfer paywall'u ve export/yedek sınırları korunur.
- Yarım metin bölümü yok. Sıradaki tek paket **P0.3-parasut (E0011)**.
  P0.4, G/T paketleri ve Faz 8 henüz başlamadı.


## P0.3-parasut — Tam metin içerik haritası

**14 Eylül 2026: E0011'in 241 satırının tamamı okundu.** Kaynak:
[Paraşüt formu](gozlemler/parasut.md). Okunan SHA-256:
`ddcc29b4deb610efaaee3f325972a78724cceb693e66d04a9285928a70631eeb`.
Kaynak form değişmedi. Bu işlem **metin kapsam haritasıdır**; kılavuz
doğrulaması, görsel içerik onayı veya canlı ürün testi değildir.

### Kaynak bağlamı ve sınır

- Formun bildirdiği Android sürümü 5.25.0; Türkçe/TL, emulator-5554.
  1 Eylül'de yalnız giriş öncesi yüzey görülmüş; ürünün içine girilememiş.
  K01–K08 canlı koşulmamış. Web kayıt koşulları ve ücret bilgisi bu tur
  doğrulanmadı; güncel fiyatın bilinmediği kaynakta açıkça yazıyor.
- Kaynak katmanları: 2/9 Eylül kılavuz notları; 10 Eylül kullanıcının
  aktardığı tanıtım videosu kareleri ve özeti; 12 Eylül kılavuz/görsel kontrol
  beyanı. Karelerdeki 2022 işlem tarihleri sürüm veya video yayın tarihi değildir.
- Kanıt havuzu **E0258–E0266: 9 PNG**; E0258/E0259 mobil giriş öncesi,
  E0260–E0266 tanıtım videosu olarak kayıtlı. E0257 yalnız .gitkeep dosyasıdır,
  görsel değildir. Tam yollar [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de.
  Bu pakette hiçbir görsel açılmadı.
- Formda doğrudan HTTP(S) URL'si yok. parasut.com alan adı ve sayfa/video
  başlıkları anılıyor; tam kılavuz/video URL'si, yayın tarihi, sürüm bağlamı
  ve satır bazlı kaynak eşlemesi eksik. URL tahmin edilmedi; PS-Q01'e aktarıldı.
- Kılavuz beyanı, pazarlama karesindeki alan, kullanıcının video özeti ve
  yorum ayrı tutulur. Sayısal tutarsızlıklar gerçek ürün hesaplama kusuru
  sayılmaz; yalnız alan/ekran niyeti de güncel ürün davranışını kanıtlamaz.
- Derin inceleme KolayBi'ye ayrıldığı için Paraşüt'te derin koşum kasten
  yapılmamış; 36 dakikalık eğitim videosuna başvurulmamış. Mobil giriş sonrası,
  karusel 2/3 ve gerçek arayüz tasarımı doğrulanmamış. Bu paket erişim engelini
  kaldırmaz ve kapsamı genişletmez.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. Belge 3 hedefleri onaylı karar değil,
ileride onaylı Belge 1/2 üzerinden değerlendirilecek konulardır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| PS-M01 | Oturum bilgisi 3; kanıt tavanı 16 | Android sürümü, kayıt engeli, masa başı yöntem; canlı davranış yok | E0011; E0258–E0259 | Belge 1 profil/yöntem; PS-Q01/Q02 |
| PS-M02 | Ürün kimliği 29 | Fatura/e-belge/cari odak; hedef işletme; abonelik/kontör; kişisel bütçe ve bordro sınırı | E0011 kılavuz/ürün beyanı; tam URL yok | Belge 1 ürün; Belge 2 kapsam; PS-Q01/Q02/Q09 |
| PS-M03 | Görev gözlemleri 40 | Dört noktalı karuselin 1/4 slaytları; K01–K08 girilemedi | E0258–E0259 | Belge 1 giriş; PS-Q02/Q11 |
| PS-M04 | Resmî kaynak — özellikler 47 | Finans, e-belge, çek/senet, stok, tekrar, OCR, entegrasyon, muhasebeci erişimi; BF ön kıyasları | E0011; kılavuz/ürün sayfası atıfları tam değil | Belge 2 özellik; PS-Q01/Q06/Q08/Q12 |
| PS-M05 | KDV raporu 69 | Menü değişikliği, aylık üç KDV değeri, drill-down/filtre/Excel; kararın yeniden sorulması önerisi | E0011 kılavuz beyanı; KDV raporuna ait ayrı PNG yok | Belge 2 vergi; Belge 3 aday; PS-Q01/Q07 |
| PS-M06 | Sistem işleyişi 90: gider/cari/ödeme | Beş gider türü; kayıt/ödeme ayrımı; açılış/fatura/avans; gecikmiş faturadan otomatik mahsup; kısmi tahsilat | E0011 kılavuz beyanı; E0260/E0261 yalnız alan adayları | Belge 2 tanıma/ödeme; B01, PS-Q03/Q04 |
| PS-M07 | Pipeline: OCR/tekrar/entegrasyon/veri | OCR'den gider, ayrı tekrar akışları, e-posta, banka ve muhasebeci bulut erişimi; şahsi harcama çıkarımı | E0011 ürün/kılavuz beyanı | Belge 2 olay modeli; PS-Q06/Q08/Q09 |
| PS-M08 | Pipeline şemaları 109/115 | Fatura→e-belge→cari→hatırlatma→tahsilat→mahsup; gider→OCR→cari→ödeme | E0011 sentez şemaları; E0260–E0261 sınırlı görsel adayları | Belge 2 akış; PS-Q04/Q06/Q08 |
| PS-M09 | Video kaynak/sayı sınırı 120/127 | Dört karede aritmetik/biçim sorunu, hesaplarda aynı IBAN; illüstrasyon yorumu | E0261–E0263, E0265–E0266 | Belge 1/2 kanıt sınırı; PS-Q01/Q11 |
| PS-M10 | Ekran seviyesi akış 151: 02–04 | Kalan/tahsil edildi; aynı fatura iddiası; CARİSİZ; planlanmış/gecikmiş kovaları | E0260–E0262 | Belge 1 durum; Belge 2 tahsilat; B01, PS-Q03/Q04/Q05 |
| PS-M11 | Ekran seviyesi akış: 05–08 | Kasa/banka listesi; stok; iki rapor; vergiler dahil, kategorisiz, nakit/tahakkuk yorumu | E0263–E0266 | Belge 1 rapor; Belge 2 zaman/kapsam; PS-Q05/Q07/Q12 |
| PS-M12 | Kullanıcı video özeti 163 | E-belge ağırlığı, otomatik hatırlatma/mutabakat, stok/müşavir hikâyesi; izlenmeyen eğitim | E0011 kullanıcı özeti; E0260–E0266 bağlamı | Belge 1 ürün konumu; PS-Q01/Q08/Q09 |
| PS-M13 | BusinessFinance kararları 183 | KDV, tanıma/taşıma, raporlar, planlanan, kategorisiz, kapsam, banka, stok, OCR, tekrar, müşavir adayları | E0011 karar tablosu; ilgili PS-M kayıtları | Yalnız Belge 3 adayları; B01/B19, PS-Q04/Q07/Q12 |
| PS-M14 | Kanıt/güven 209; sonuç 234 | Kaynak katmanları; şahsi kullanım/kritik stok/vergi hariç çıkarımları; canlı davranış ve fiyat belirsizliği | E0011; E0258–E0266 kapsamı | Belge 1/2 yöntem; B17/B19, PS-Q01/Q09/Q10/Q11 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**PS-Q01–PS-Q12'nin tamamı açık.** Önceki denetimin numarasız Paraşüt
devir notları PS-Q03/PS-Q04'e bağlandı; B01–B19 yeniden numaralandırılmadı.
Kaynak/kod/görsel kontrolleri sonraki paketlerdir; yeni canlı test başlamaz.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| PS-Q01 | Kılavuz/ürün/video iddialarının tam kaynakları ve tarihleri nerede? | Sayfa/video adı→tam URL→erişim/yayın tarihi→iddia eşlemesi kur; kılavuz, ürün sayfası, kullanıcı özeti ve çıkarımı ayrı etiketle. Alan adı tek başına bütün satırları desteklemez. Video yayın tarihini 2022 işlem tarihlerinden türetme; bu pakette URL açılmadı | Belge 1/2 kaynak kapısı; kaynak kontrolü; B19 |
| PS-Q02 | Mobil kayıt yokluğundan web kaydının VKN/ücret zorunluluğuna nasıl varılmış? | E0258/E0259 yalnız görülen mobil giriş yüzeyidir. Web kayıt koşullarını paket/dönemle kaynakla veya doğrulanamadı olarak sınırla; giriş sonrası mobil özelliklerini masaüstü karelerinden çıkarma. Güncel fiyatı eski abonelik beyanıyla doldurma | Belge 1 profil/erişim; G/kaynak |
| PS-Q03 | 02/03 gerçekten aynı faturanın iki durumu mu? | E0260/E0261'de formun yazdığı farklı tarih ve belge numarası/durumlarını karşılaştır; aynı kayıt kimliği olmadan önce/sonra testi sayma. Kalan/Tahsil Edildi durumlarının ayrı örneklerde gösterilmesi ile aynı faturanın geçişi farklı kanıttır | Numarasız Paraşüt devri; Belge 2; G |
| PS-Q04 | Otomatik mahsup BF CounterpartyPayment ile aynı mı; kısmi ödeme her zaman cariyi kapatır mı? | Paraşüt mahsup kuralını tam kaynağa, BF'nin tahsis/ödeme modelini B01 kod kontrolüne bağla. Tanıma/taşıma benzerliği fatura bazlı tahsis eşdeğerliği değildir; en gecikmiş sırasının kapsam/istisnaları ayrı. Şemadaki cari kapanır ifadesini ödeme tutarı ve kalan borçla sınırla | B01 / P1-B01; numarasız Paraşüt devri; kaynak/kod |
| PS-Q05 | Planlanmış kovası tekrar mı; 07 tahakkuk, 08 nakit sayımını hangi kaynak kanıtlıyor? | E0262/E0265/E0266 alan adlarını gerçek rapor zamanlamasından ayır. Planlanmış etiketini yalnız ileri tarihli/tekrarlayan diye kesinleştirme; farklı raporların aynı olayı ne zaman saydığını kılavuzla açıkla. BF rapor tanıma zamanı ayrıca kodla doğrulanır | B04 kontrol ilkesi; Belge 2 rapor; G/kaynak/kod |
| PS-Q06 | OCR otomatik nihai gider mi, öneri/onay mı; her giderde cari ve fotoğraf zorunlu mu? | OCR ürün beyanının kaydetme/onay/ödeme kaynağı sınırını bul. Beş gider türünü ve isteğe bağlı tedarikçiyi tek zorunlu OCR→cari zincirine sıkıştırma; CARİSİZ alanı ile cari borç zorunluluğunu uzlaştır. Tekrarlayan fatura/giderde tanım ve üretim eşiklerini ayrı kaydet | Belge 2 olay modeli; kaynak; B12 ortak kanıt ilkesi |
| PS-Q07 | KDV raporunun topladığı/ürettiği değer ile vergi hesaplama sorumluluğu aynı mı; Vergiler hariç gerçekten var mı? | KDV kılavuzunun tam kaynağını ve alan anlamlarını bul; vergi hesabı, girilmiş KDV toplamı ve beyanname hazırlığı eşitlenmesin. E0265'te görünmeyen hariç seçeneğini E0264 stok sütunuyla kanıtlama. İki rakip örneği ihtiyaç veya karar değişikliği onayı değildir | Belge 2 vergi; Belge 3 adayı; kaynak/G; PRD/ADR kontrolü |
| PS-Q08 | Otomatik e-posta, banka mutabakatı ve müşavir erişimi koşulları ne? | Kaynakta tetikleme/kurulum/onay/rol sınırını bul; otomatik hareket çekme, mutabakat ve ödeme başlatma ayrı yeteneklerdir. Canlı erişim varlığı dosya paylaşımı yokluğunu kanıtlamaz. Mesaj gönderme, banka bağlama veya kullanıcı daveti yapma | Belge 2 entegrasyon; B11/B19 ortak ilkeler; kaynak |
| PS-Q09 | Patron cebinin yalnız cariyle temsil edildiği ve kişisel bütçe/bordro yokluğu hangi sınırda? | Formun kendi çıkarım etiketini karar/sonuçlarda koru; olası dolaylı yolu zorunlu kullanım sayma. Ürün hedefi, şahsi gideri kaydetme imkânı ve kapsam boyutu farklı konulardır. Kullanıcı video izlenimini kullanıcı araştırması veya bütün ürün yokluğu kanıtı sayma | Belge 1/2 kapsam; Belge 3; B08 ortak ilke; kaynak/metin |
| PS-Q10 | Menü/renk/kategori alanlarından gerçek veri modeli çıkarılıyor mu? | CARİSİZ, kırmızı stok satırı ve Kategorisiz etiketini kontrol et; boş kategori kovası ayrı kategori nesnesi demek değildir. Kırmızı satırın kritik stok açıklaması anlatıcıya mı çıkarıma mı ait, ayır. Gerçek görsel tasarım görülmedi sınırı korunur | Belge 1 arayüz; B12 ortak ilke; G/kaynak |
| PS-Q11 | Dokuz görselin tümünün rolü açık mı; sayısal tutarsızlıklar nasıl aktarılmalı? | E0258–E0266'yı tek tek değerlendir; E0257'yi görsel sayma. Silinmiş 01c'yi otomatik geri getirme veya slayt 2/3 kanıtı sayma. 03/04/07/08 aritmetik/biçim notlarını ürün hatası saymadan kontrol et; beşinci IBAN tekrarını aritmetik kusurla karıştırma. Kısa atıfları tam kimliğe bağla | B17 mekanik kapı; G; Belge 1/2 yöntem |
| PS-Q12 | BF'de hatırlatma yok, kategorisiz/kapsamsız aynı, tekrar doğrudan kayıt üretir kıyasları doğru mu? | Mevcut BF kod/ADR ve README karar sözlüğüyle kıyasla; cari hatırlatma ile genel cihaz/vade hatırlatmasını ayır. Kategori yokluğu ile kapsam filtresini birleştirme. Yarar/maliyetler ölçüm değilse yorum etiketi taşısın. KDV/banka/stok/e-belge önerisi mevcut ürün kararını değiştirmez; kararları B01 ve kaynak sonuçlarıyla B19'a yay | Belge 3 adayları; B01/B19; kaynak/kod |

### Bu paket sonrası kapsam

- Okundu: E0011 tam metin; PS-M01–PS-M14 ve PS-Q01–PS-Q12 kaydedildi.
- Yeni görsel incelemesi **0/9**, canlı test **0**, kapanan B bulgusu **0**.
- B01 kıyas işine ve numarasız Paraşüt devir notlarına bağlandı; B17/B19
  atıf/özet kontrolü sürüyor. Diğer B bağlantıları ortak kontrol ilkeleridir,
  yeni hata doğrulaması değildir.
- Kaynak form ve görseller değişmedi. Masa başı kanıt sınırı ve KolayBi'ye
  ayrılan derin inceleme kararı korundu; ürün kodu/PRD/ADR değiştirilmedi.
- Yarım bölüm yok. Sıradaki tek paket **P0.3-logo-isbasi (E0009)**.
  P0.4 ve G/T paketleri başlamadı; Faz 8 açılmadı.


## P0.3-logo-isbasi — Tam metin içerik haritası

**14 Eylül 2026: E0009'un 248 satırının tamamı okundu.** Kaynak:
[Logo İşbaşı formu](gozlemler/logo-isbasi.md). Okunan SHA-256:
`cee51ae09da8194a354c0549b78e5f4d4d371a308638e731dd4382cef1d07f4f`.
Kaynak form değişmedi. Bu paket **metin kapsam haritasıdır**; ürünün iç
davranışı, kaynak güncelliği veya görsel içeriği yeniden doğrulanmadı.

### Kaynak bağlamı ve sınır

- Form Android 3.20.0, emulator-5554 bildiriyor. Giriş/kayıt etiketleri
  İngilizce, sektör/sözleşme içeriği Türkçe olarak kaydedilmiş. Bunlar
  tarihsel form beyanıdır; bu tur uygulama açılmadı.
- Tarih katmanları: 1 Eylül giriş/kayıt yüzeyi; 2 Eylül kullanıcının kayıt
  deneyimi ve e-postayla gelen video; 9 Eylül ürün/blog notları; 12 Eylül
  sesli fatura/müşavir kaynak kontrolü beyanı. Blog erişim tarihi özellik
  yayın tarihi değildir; Android/iOS duyurusu bugünkü platform desteği sayılmaz.
- Kanıt havuzu **E0220–E0225: 6 PNG**. İlk dördü giriş öncesi yüzey, son
  ikisi pazarlama grafiği olarak kayıtlı. E0219 .gitkeep görsel değildir.
  Tam yollar [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Bu tur 0/6 görsel açıldı.
- Kaynakta doğrudan HTTP(S) URL'si yok; `isbasi.com/mali-musavirlere-ozel`
  şemasız adresi, isbasi.com/logo.com.tr alan adları ve iki blog başlığı var.
  Bunlar LI-Q01'de kaynak eşlemesine devredildi; URL tahmini yapılmadı ve
  web taraması başlatılmadı.
- Giriş sonrası hiçbir ekran/canlı davranış doğrulanmamış. Gerçek kayıt
  sonrası hazırlık mesajının görseli yok; kullanıcı aktarımı ile manuel
  ekran kanıtı ayrılır. SMS/kayıt engeli aşılmadı; kişisel kayıt bilgileri
  bu haritaya kopyalanmadı. Güncel fiyat ve sektör listesinin tamamı bilinmiyor.
- Derin inceleme KolayBi'ye ayrılmış; Logo'nun kapsamı bu paketle genişlemez.
  Üç paket, sesli fatura ve müşavir erişimi iddiaları ürün/sürüm/kaynak
  sınırlarıyla ele alınacak; pazarlama ikonları çalışan işlev kanıtı değildir.

### Başlık → iddia → kanıt → rapor haritası

Satırlar okunan sürüme aittir. Belge 3 bağlantıları onay değil, ileride
onaylı Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| LI-M01 | Oturum bilgisi 3; kanıt tavanı 16 | Sürüm, iki dil, kayıt/SMS ve hazırlık engeli; yalnız giriş öncesi gözlem | E0009; E0220–E0223 | Belge 1 profil/yöntem; LI-Q01/Q02 |
| LI-M02 | Ürün kimliği 34 | Firma bilgisi olmadan fatura, mikro hedef, üç paket, kapsam ve ERP kıyası | E0009; E0221–E0222 hedef adayları | Belge 1 ürün; Belge 2 kapsam; LI-Q02/Q08 |
| LI-M03 | Görev gözlemleri 45 | SSO ikonları, üç alan, sektör/sözleşme, sessiz dönüş; gerçek kayıt ve K01–K08 engeli | E0220–E0223; gerçek kayıt mesajına PNG yok | Belge 1 giriş akışı; LI-Q02/Q09 |
| LI-M04 | Arayüz incelemesi 57 | Register baskınlığı, SMS/sözleşme/hazırlık sürtünmesi, hata ve dil | E0220–E0223; süreç beyanı E0009 | Belge 1 desenler; LI-Q02/Q09 |
| LI-M05 | Resmî kaynak — özellikler 67 | E-belge, finans, stok/sipariş, sekiz entegrasyon, müşavir, paket/fiyat | E0009 ürün/blog atıfları; E0224 adlar için aday | Belge 2 özellik; LI-Q01/Q06/Q08 |
| LI-M06 | Sesli komut 85 | Türkiye'de ilk duyurusu; Android önce/iOS sonra; hız ve yanlış tutar yorumu | E0009 blog beyanı; çalışır akış/görsel yok | Belge 2 sesli giriş; Belge 3 adayı; LI-Q01/Q03 |
| LI-M07 | Müşavir Paneli 107 | Boş şablon satırları/ikonlar; firma mı fatura mı; müşteri adına işlem ve davet | E0225 + E0009 ürün sayfası beyanı | Belge 2 yetki; B19, LI-Q04 |
| LI-M08 | Video kareleri 136 | Sekiz entegrasyon adı; Müşavir Portal şeması; BF entegrasyon eşlemeleri | E0224–E0225 | Belge 1 yüzey niyeti; Belge 2 kapsam; LI-Q04/Q06 |
| LI-M09 | Sistem işleyişi 159 | Tek kayıtta cari/kasa/stok güncellemesi, fatura, OCR, ödeme, bulut, şahsi kullanım çıkarımı | E0009 ürün anlatımı; gerçek işlem kanıtı yok | Belge 2 olay modeli; LI-Q05/Q07/Q08 |
| LI-M10 | Satış/gider pipeline şemaları (159 altında) | Fatura→cari/stok→tahsilat; OCR→gider/cari→ödeme; müşavire yansıma | E0009 sentez şemaları | Belge 2 kayıt/ödeme; LI-Q04/Q05/Q07 |
| LI-M11 | BusinessFinance kararları 187 | Kayıt, sesli giriş, delegasyon, tek mutation, banka/e-belge, OCR/POS ve ERP önerileri | E0009 karar tablosu; ilgili LI-M kayıtları | Yalnız Belge 3 adayları; B19, LI-Q03/Q04/Q06/Q10 |
| LI-M12 | Kanıt ve güven 212 | Giriş öncesi/ürün/pazarlama ayrımı; kaynak adresi; bilinmeyen onay/fiyat/sektör; çıkarımlar | E0009; E0220–E0225 kapsamı | Belge 1/2 yöntem; B17/B19, LI-Q01/Q08/Q09 |
| LI-M13 | Sonuç 240 | Mikro hedef, sesli girişin tek örnekliği, müşavir yetkisi, insan onayı hükmü | E0009; LI-M01/M02/M06/M07 | Belge 1/2 özet; B19, LI-Q02/Q03/Q04/Q08 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**LI-Q01–LI-Q10'un tamamı açık.** Önceki numarasız Logo OCR/insan onayı
devir notu LI-Q05'e bağlandı. B01–B19 yeniden numaralandırılmadı; aşağıdaki
sorular kayıt altına alındı diye kapanmış sayılmaz.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| LI-Q01 | Her ürün/blog/video iddiasının tam kaynağı, yayın tarihi ve paket sınırı nerede? | Şemasız müşavir adresi ve blog başlıklarını tam kaynak kayıtlarına bağla; video dağıtım tarihi ile yayın/sürüm tarihini ayır. Ürün sayfası, duyuru, kullanıcı aktarımı ve çıkarım ayrı etiketlensin. Güncel fiyat/üç paket/Android-iOS desteği eski erişim tarihiyle doğrulanmış sayılmaz | Belge 1/2 kaynak; kaynak kontrolü; B19 |
| LI-Q02 | Hazırlık mesajı insan/satış onayı veya numaranın reddi anlamına mı geliyor? Firma bilgisi olmadan fatura ne demek? | E0221'de Company name alanı ile firma bilgisi olmadan tezini uzlaştır; vergi profilinin daha sonra istenmesi ihtimali ayrı kalsın. E0223 sözleşme karesi tek başına 10 saniye spinner, mesaj yokluğu veya hesap oluşmama kanıtı değildir. Kullanıcının hazırlık mesajı aktarımından arka planda insan onayı sonucu çıkarma; yeni kayıt/SMS işlemi başlatma | Belge 1 kayıt; G/metin/kaynak; B12 ortak ilke |
| LI-Q03 | Sesli fatura halen hangi platformda, hangi kapsamda; kullanıcı onayı var mı? Tek örnek/ilk iddiası neyi kanıtlıyor? | Duyurunun tarih ve sürümünü bul; ilk ifadesini sağlayıcının beyanı olarak tut. Sesle hazırlama, düzenleme, nihai kayıt ve e-belge gönderimi ayrı adımlar. Yanlış duyulan tutarın sessizce kaydolduğunu veya onayın olmadığını varsayma; eller doluyken kullanım/öğrenme yararı ölçülmüş değildir. Diğer sekiz üründe görülmedi ile piyasada yok ayrılır | Belge 2 sesli giriş; Belge 3 adayı; kaynak |
| LI-Q04 | Müşavir Portal satırları ve müşteri adına işlem yetkisinin kapsamı ne? Başka rakiplere yayılmış mı? | E0225 ikonlarını gerçek belge silme/düzenleme yetkisi sayma; firma listesi ve belge listesi olasılıklarını koru. Ürün kaynağında ekleme/rol/işlem sınırını doğrula; Logo yetkisini Paraşüt/KolayBi'ye genelleme. Erişim varlığı dosya paylaşımı yokluğu veya doğrulanmış kullanıcı ihtiyacı değildir; davet/mesaj gönderme | B19 / P1-B19; Belge 2 yetki; kaynak/G |
| LI-Q05 | Akıllı Fiş Okuma otomatik nihai gider mi, öneri/onay mı? | E0224 yalnız entegrasyon adını gösterir. OCR alanları, insan onayı, gider kaydı, cari/ödeme kaynağı etkilerini tam kaynakta ayrı takip et; her fişten zorunlu cari borç üretildiğini varsayma. Logo için önceki numarasız OCR→otomatik gider/insan onayı devri burada izlenir | Numarasız Logo devri; Belge 2 fiş; kaynak |
| LI-Q06 | İşbaşı POS = BF PosSettlement mı; banka hareketi = otomatik mutabakat mı? | E0224'teki adları işlev kanıtı sayma. POS satış/cihaz/online tahsilat ile bekleyen POS tutarının bankaya aktarımını ayrı kıyasla. Banka verisi alma, mutabakat ve ödeme başlatma ayrılır; entegrasyonların paket/kullanım sınırını kaynağa bağla. Banka bağlantısı/ödeme başlatma yapılmaz | Belge 2 entegrasyon; Belge 3 adayı; kaynak/kod |
| LI-Q07 | Tek kayıt cari+kasa+stok günceller ifadesi ayrı tahsilat şemasıyla nasıl uyuşuyor? | Fatura/fişin tanınması ile ödeme gerçekleşmesini ayır; stok olmayan kalem, ödeme durumu ve kısmi tutar koşullarını belirle. Tahsilat her durumda cariyi tamamen kapatır diye yazma. Ekran/ürün anlatısından transaction sınırı veya üç fiziksel defter çıkarmama; BF SaveChanges ve istemci FinancialDataChanges aynı mekanizma değildir | B01/B12 ortak ilkeler; Belge 2 olay modeli; kaynak/kod |
| LI-Q08 | Kurye/Öğrenci seçeneği gerçek hedef kitle veya kişisel bütçe yokluğu kanıtı mı? | E0222 yalnız görülen sektör seçeneklerini destekler; tüm liste bilinmiyor. Pazarlama konumu ile gerçek kullanım/ihtiyaç ayrılır; ortak cari dolaylı kullanım çıkarımı karar/sonuçta kesinleşmesin. Bordro, tekrar ve diğer özelliklerin var/yok iddiası için somut kaynak aranır | B08/B19 ortak ilkeler; Belge 1/2 kapsam; kaynak/G |
| LI-Q09 | Giriş öncesi görsellerden SSO başarısı, erişilebilirlik ve hata davranışı ne kadar çıkarılabilir? | E0220–E0223'ün gösterdiği alan/ikon/dil/hiyerarşiyi kaydet; SSO ikonunu başarılı oturum testi sayma. Süreç boyunca sessiz hata için eski koşum beyanını tek kareden ayır. Gerçek kayıt mesajının PNG eksiği ve sektör listesinin sınırı açık kalsın; giriş sonrası tasarım hakkında sonuç çıkarma | B17; Belge 1; G/mekanik kapı |
| LI-Q10 | BF onboarding tek soruya indirgenmiş, tek SaveChanges okuma modellerini besler ve müşavir ihtiyacı doğrulandı kıyasları doğru mu? | README karar sözlüğü ve BF kod/ADR gerçeğiyle sınırla; kayıt ile onboarding sorusu aynı kapsam değildir. Kullanıcı ihtiyacı, ürünün sunduğu yetenek ve mimari benzerlik ayrı kanıttır. POS/OCR/müşavir ve sektör kararları Belge 3 onayı değildir; kaynak sonuçlarını B19 özetlerine yay | B19; Belge 3 adayları; kaynak/kod |

### Bu paket sonrası kapsam

- Okundu: E0009 tam metin; LI-M01–LI-M13 ve LI-Q01–LI-Q10 kaydedildi.
- Yeni görsel incelemesi **0/6**, canlı test **0**, kapanan B bulgusu **0**.
- B19 yetki/özet yayılımı ve B17 kanıt atıflarıyla bağlantılı; numarasız OCR
  devri LI-Q05'te. Diğer B bağlantıları ortak ilkedir, yeni hata doğrulaması değil.
- Kaynak form ve görseller değişmedi; kayıt/SMS engeli aşılmadı. Masa başı
  sınırı ve KolayBi derin inceleme kararı korundu; ürün kodu/PRD/ADR değişmedi.
- Yarım bölüm yok. Sıradaki tek paket **P0.3-quickbooks (E0012)**.
  P0.4 ve G/T paketleri başlamadı; Faz 8 açılmadı.


## P0.3-quickbooks — Tam metin içerik haritası

**14 Eylül 2026: E0012'nin 169 satırının tamamı okundu.** Kaynak:
[QuickBooks formu](gozlemler/quickbooks.md). Okunan SHA-256:
`9870e7d8bdb8f22111ad8262aee00e98f8df987239911c9d55a2b1bf83950bf2`.
Kaynak form değişmedi. Bu paket **metin kapsam haritasıdır**; yardım merkezi
iddiaları, ürün/paket bağlamı veya görsel içeriği yeniden doğrulanmadı.

### Kaynak bağlamı ve sınır

- **B13 ürün/paket ayrımı bu haritanın ana eksenidir.** Form başlığı ve
  sistem/karar bölümleri QuickBooks **Solopreneur**'ü anlatıyor; dört kare
  ise formun kendi kanıt tavanına göre QuickBooks mobil onboarding'i ve
  QuickBooks **Online Simple Start** plan ekranıdır. Aşağıda her satır bu
  üç bağlamdan hangisine ait olduğuyla okunur: *QB mobil onboarding (manuel
  kare)*, *QBO Simple Start (plan kartı)*, *Solopreneur (yardım merkezi/ürün
  sayfası beyanı)*. Solopreneur fiyatı veya onboarding'i bu karelerden çıkarılmaz.
- Form mobil sürüm 30.4.7 (React Native), Android emülatör, İngilizce arayüz
  ve mağaza bölgesi Türkiye/TRY bildiriyor. Bu sürümün hangi uygulama paketine
  ait olduğu formda yazmıyor; bu tur cihaz/sürüm kontrol edilmedi.
- Tarih katmanları: 1–2 Eylül emülatör onboarding'i ve kullanıcının hesap
  açması; 9 Eylül yardım merkezi/ürün sayfası notları; 12 Eylül Faz 7.5 beyanı.
  Karedeki kampanya fiyatı yakalama anına aittir; güncel fiyat değildir.
- Kanıt havuzu **E0268–E0271: 4 PNG**. E0267 yalnız .gitkeep dosyasıdır,
  görsel değildir. Tam yollar [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de.
  Bu pakette 0/4 görsel açıldı; E0271'in içeriği yalnız 13 Eylül denetiminin
  B13 kaydından aktarılır, bu turun gözlemi değildir.
- Formda doğrudan HTTP(S) URL'si yok. Yedi makale/sayfa başlığı ve 9 Eylül
  erişim tarihi var; tam URL, yayın/güncelleme tarihi ve satır bazlı kaynak
  eşlemesi eksik. URL tahmin edilmedi, web taraması yapılmadı (QB-Q04).
- Solopreneur'ün hiçbir ekranı görülmemiş; ödeme kapısı kasten aşılmamış,
  kart bilgisi girilmemiş. Bu paket hesap açma, deneme başlatma veya banka
  bağlama yapmaz. Kullanıcının kart istemi notu kare kanıtı değildir.
- Kişisel hesap bilgisi haritaya kopyalanmadı; formdaki "Deniz Tasarim"
  sentetik işletme adı olarak yalnız kaynak satırında kalır.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. "Bağlam" ifadesi B13 ayrımını gösterir;
Belge 3 bağlantıları onay değil, ileride onaylı Belge 1/2'den beslenecek
değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| QB-M01 | Oturum bilgisi 3 | Solopreneur/Intuit başlığı; mobil 30.4.7; üç tarih; TRY/USD; hesap açma, plan kapısında durma, kart istemi kullanıcı notu, ABD dışına kapalılık; masa başı inceleme türü | E0012; E0268–E0271 onboarding kareleri. Bağlam karışık: sürüm/kareler QB mobil, erişim sınırı Solopreneur beyanı | Belge 1 profil/yöntem; B13, QB-Q01/Q02/Q03 |
| QB-M02 | Kanıt tavanı 15 | Uygulamaya girilmedi; dört kare Solopreneur değil QB mobil onboarding + Simple Start; Solopreneur cümleleri yalnız yardım merkezi; Type bulgusu da yalnız kaynağa dayanıyor | E0271 Simple Start; E0268–E0270 | Belge 1/2 kanıt sınırı; B13, QB-Q01/Q06 |
| QB-M03 | Ürün kimliği 30 | Banka indirme + İşletme/Şahsi etiketi + Schedule C tezi; ABD hedef kullanıcı; tahmini vergi/fatura/km; çift taraflı muhasebe, A/P–A/R ve hesap planı yokluğu; iş modeli hücresinde Simple Start kampanya fiyatı; en yakın kavramsal rakip değerlendirmesi | E0012 kaynak beyanı; fiyat hücresi yalnız E0271'e dayanıyor. Bağlam: tez Solopreneur, fiyat QBO Simple Start | Belge 1 ürün; Belge 2 kapsam; B13/B19, QB-Q01/Q08/Q09 |
| QB-M04 | Görev gözlemleri 41, K00 | Üç noktalı karusel ve fatura slaytı; hesap açma hoş geldin ekranı ve üç madde; tek alan Business name + ad kullan bağlantısı; Simple Start kartı, deneme üst metni, beş özellik ve alt bağlantılar; plan listesinin çekilmediği | E0268–E0271 sırasıyla. Bağlam: QB mobil onboarding, son kare QBO Simple Start | Belge 1 onboarding; Belge 2 erişim; B13, QB-Q03/Q05/Q12 |
| QB-M05 | K01–K08 satırı 49; vaat/kapı gerilimi 51 | K01–K08 engelli ve resmî kaynağa devredilmiş; 02'deki ücretsiz deneme maddesi ile 04'te görünen tek yolun abonelik kartı olduğu gerilimi; kart istemi karede yok | E0269, E0271; K01–K08 için kare yok | Belge 1 erişim/güven sunumu; B13, QB-Q03 |
| QB-M06 | Sistem işleyişi 56: kayıt ve sınıflandırma | Kategori→Schedule C satırı; P&L/nakit akışı türetimi; işlem başına Type Business/Personal, üçüncü değer yok; şahsi kayıt raporda yok ama silinmez; Split ve araç/yakıt istisnası; Exclude; 180 kategori, hesap planı yok | E0012 resmî kaynak beyanı; bu satırlara ait PNG yok. Bağlam: Solopreneur | Belge 2 işletme/şahsi sınıflandırma; QB-Q04/Q06/Q08 |
| QB-M07 | Sistem işleyişi 56: giriş/otomasyon/veri; pipeline şeması 74 | Banka/kart bağlama ve otomatik indirme; diğer müşterilerin kalıbına dayalı öneri; manuel/fiş girişi; 30 kurallı Rules; bulut; transfer ve muhasebeci tarafı doğrulanamadı; bağla→indir→öner→gözden geçir→kural→P&L/vergi/Schedule C şeması | E0012 resmî kaynak beyanı ve sentez şeması; PNG yok | Belge 2 olay modeli/entegrasyon; B07/B12 ortak ilke, QB-Q04/Q07/Q08 |
| QB-M08 | Doküman akış yeniden kurulumu 81 | Kategorize etme, Split, Rules, tahmini vergi, fatura kesme akışları; her birine BF karşılığı: bölme yok, recurring farklı iş, vergi hesabı yok/takvim önerisi, e-belge yok/tanıma modeli | E0012 makale metninden yeniden kurulum; ekran yok. Tahmini vergi ve fatura satırları ürün sayfası beyanı | Belge 2 akışlar; Belge 3 BF kıyası; QB-Q04/Q08/Q10 |
| QB-M09 | Arayüz incelemesi 94 | Düşük sürtünmeli onboarding ve ardından ödeme duvarı; ad kullan mikro kopyası; üstü çizili liste/kampanya fiyatı ve 3,3 kat yorumu; Type/Category iki sütun hiyerarşisi | E0268–E0271 onboarding/fiyat; hiyerarşi satırı yalnız resmî kaynak, ekran yok. Bağlam: fiyat QBO Simple Start | Belge 1 desenler; B13, QB-Q01/Q05/Q06 |
| QB-M10 | Akış özeti 103 | En kısa akış banka feed'i; en fazla sürtünme ürün adı ayrılmadan ödeme duvarı; hedef kullanıcı; tek akış + etiket; transfer/kart ödemesi belgelenmemiş; borç modülü yok, planlama tahmini vergi + hedefler | E0012 sentez; ödeme duvarı E0271, diğerleri resmî kaynak beyanı | Belge 1/2 özet; B13/B19, QB-Q01/Q07/Q08 |
| QB-M11 | BusinessFinance için kararlar 118 | On satır: Type ve Type/Category ayrımı doğrudan al; Split kararı yeniden sor; şahsi raporlama, Exclude, mikro kopya, sadelik değerlendirmeleri; banka/Rules, Schedule C/vergi ve fiyat/deneme sunumu alma | E0012 karar tablosu; ilgili QB-M kayıtları; fiyat/deneme satırı E0269/E0271 | Yalnız Belge 3 adayları; B13/B19, QB-Q03/Q09/Q10/Q11 |
| QB-M12 | Kanıt ve güven 140 | Manuel gözlem dört kareyle sınırlı ve Solopreneur değil; resmî kaynak başlık listesi; çıkarım olarak en yakın rakip; doğrulanamayanlar; ödeme kapısının kasten aşılmaması | E0012; E0268–E0271 kapsamı | Belge 1/2 yöntem; B13/B17/B19, QB-Q02/Q03/Q04/Q12 |
| QB-M13 | Tek cümlelik sonuç 161 | ADR 0013'ün pazardaki en yakın karşılığı; vergi ekseni, banka/vergi mekanizmaları; Split'in bizde karşılığı olmayan tek nokta olduğu hükmü | E0012; QB-M03/M06/M11 dayanakları | Belge 1/2 özet; B19, QB-Q09/Q10/Q11 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**QB-Q01–QB-Q12'nin tamamı açık.** B13 doğrudan QB-Q01/Q03'e bağlandı; bu
paket B13'ü kapatmaz ve formdaki ayrımı düzeltmez. B01–B19 yeniden
numaralandırılmadı. Kaynak/kod/görsel kontrolleri sonraki paketlerdir.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| QB-Q01 | Simple Start bağlamı hangi satırlarda Solopreneur anlatımına karışıyor? | Ürün kimliği iş modeli hücresi (38), arayüz fiyat sunumu (100), akış özeti sürtünme maddesi (107), karar tablosu fiyat/deneme satırı (138) ve vaat/kapı paragrafını (51) ürün/paket adıyla tek tek işaretle. Kanıt girişindeki `quickbooks/` klasör etiketinin Solopreneur/resmî kaynak yazması da aynı yayılımdır (E0272). Solopreneur fiyatı, deneme koşulu veya onboarding'i bu dört kareden türetme; kapanış formu ve özetleri birlikte uzlaştıran P1 işidir | B13 / P1-B13; B19 yayılımı; Belge 1 ürün profili |
| QB-Q02 | Mobil 30.4.7 hangi uygulamanın sürümü; ABD dışına kapalılık ve TRY/USD ekseni hangi kaynağa ve tarihe dayanıyor? | Sürümü paket kimliği ve yakalama tarihiyle bağla veya bilinmiyor işaretle; mobil uygulama sürümünü Solopreneur sürümü sayma. Bölge kısıtını tam kaynağa ve erişim tarihine bağla; mağaza bölgesi TRY gösterimi ürünün Türkiye'de sunulduğu veya Solopreneur'e erişilebildiği anlamına gelmez | B13; Belge 1 profil/erişim; kaynak/G |
| QB-Q03 | Ücretsiz deneme 04 karesinde görünüyor mu, görünmüyor mu? | Form K00 satırında 04 üstünde bir aylık deneme metni yazıyor; gerilim paragrafı ve karar satırı 04'te ücretsiz yol görünmediğini söylüyor. B13 kaydı deneme ifadesinin karede olduğunu ve erişilebilirlik kanıtı olmadığını belirtiyor. E0269/E0271'de vaat metni, tıklanabilir yol ve kart istemini ayrı kaydet; kart istemini kullanıcı notu olarak tut. Deneme başlatma veya ödeme bilgisi girme | B13; G paketi; Belge 1 güven/fiyat sunumu |
| QB-Q04 | Yardım merkezi ve ürün sayfası iddialarının tam kaynakları, tarihleri ve paket sınırı nerede? | Yedi başlığı tam URL, erişim/güncelleme tarihi ve iddia satırıyla eşle; yardım makalesi, ürün/pazarlama sayfası ve çıkarımı ayrı etiketle. 30 kural, 180 kategori, ödeme yöntemleri ve tahmini vergi gibi sayısal/özellik beyanlarını güncel ürün doğrulaması sayma. URL tahmin etme; bu pakette dış kaynak açılmadı | Belge 1/2 kaynak kapısı; kaynak kontrolü; B19 |
| QB-Q05 | Onboarding karelerinden çıkan tasarım yorumları gözlem mi, yorum mu? | Karusel nokta sayısı, alan ve metinler gözlem adayıdır; çok düşük sürtünme, sert duvar ve şahıs şirketi gerçeğini kabul eden dil yorumdur. 3,3 kat ifadesini iki fiyatın oranı olarak kontrol et; kampanya bitiminde bu fiyata dönüleceği ve vergi/koşul yıldızları kaynakla doğrulanmadıkça çıkarım kalır. Kullanılabilirlik ölçümü yapılmış sayma | Belge 1 anlatım sınırı; B13; G/metin |
| QB-Q06 | Type Business/Personal, üçüncü değer yokluğu ve şahsi kaydın raporda silinmeden düşmesi hangi kaynak cümlesine dayanıyor? | Her iddiayı makale cümlesine bağla; incelenmemiş/kategorisiz işlem durumu gibi ara hâllerin yokluğunu kaynak olmadan kesinleştirme. Hiyerarşi satırındaki iki sütun ifadesi Solopreneur ekranı görülmeden görsel tasarım gözlemi sayılmaz. Split kısıtları ve araç/yakıt istisnasının kapsamını ülke/paketle sınırla | Belge 2 sınıflandırma; Belge 3 kıyas dayanağı; kaynak |
| QB-Q07 | Otomatik indirme, öneri, Rules, Exclude, bulut ve eksik transfer/muhasebeci belgeleri hangi davranış sınırını gösteriyor? | Diğer müşterilerin kalıbı ve zamanla öğrenme ifadesini kaynak cümlesine bağla. Kural motorunu tekrarlayan kayıt üretimiyle eşitleme; sistem tablosundaki Tekrarlayan işlem başlığı ile doküman akışındaki farklı işler ayrımını uzlaştır. Exclude'un rapor/bakiye etkisini ayrı bul. Belgelenmemiş transfer/muhasebeci tarafını ürün yokluğu sayma; banka bağlama yapma | B07/B12 ortak ilkeler; Belge 2 olay modeli/entegrasyon; kaynak |
| QB-Q08 | Schedule C eşlemesi, tahmini vergi, fatura ve kapsam dışı modüller Solopreneur'e mi Simple Start karşılaştırmasına mı ait? | Kategori→Schedule C alıntısı, P&L/nakit akışı, tahmini vergi ve fatura ödeme yöntemlerini kaynak türüyle ayır. A/P–A/R, tek işletme ve Simple Start'a yönlendirme beyanını paket karşılaştırma kaynağına bağla; E0271'deki Simple Start özellik listesini Solopreneur özelliği sayma. Planlama maddesindeki hedefler için formda dayanak yok; kaynak bul veya sınırla | B13; Belge 2 vergi/fatura/kapsam; kaynak |
| QB-Q09 | Kavramsal en yakın rakip, pazardaki en güçlü doğrulayıcı ve ADR 0013'ün pazardaki karşılığı ifadeleri hangi sınırda? | Formun kendi çıkarım etiketini koru; dokuz uygulamalık örneklem pazar kapsamı değildir. README dil kuralına göre rakibin benzerliği bizim kararımızın doğrulanması gibi yazılmasın; vergi ekseni ile raporlama boyutu farkı ayrı kalsın. Karşılaştırmayı P4 tema aktarımında bütün uygulamalarla yeniden değerlendir | B19; Belge 1 ürün konumu; Belge 3; metin |
| QB-Q10 | BF karşılığı kıyasları mevcut kod ve ADR'lerle uyuşuyor mu? | Kayıt başına tek kapsam/bölme yokluğu, recurring tanımı ve realize, vergi takvimi önerisi, e-belge yokluğu ile Obligation/CounterpartyCharge, şahsi kaydın net varlıkta kalması, HasBusiness sorusu kod/ADR 0013/0014/0016 ile kontrol edilsin. Exclude ile idempotent iptal eşdeğerliğini rapor ve bakiye etkisi ayrı kontrol edilmeden kurma. Mevcut yeteneği yeni öneri gibi, olmayanı mevcut gibi yazma | B01–B04 kıyas ilkesi; Belge 3 mevcut durum; kaynak/kod |
| QB-Q11 | Karar etiketleri README sözlüğüne ve kararı yeniden sor eşiğine uyuyor mu? | Parantezli doğrudan al varyantlarını beş tanımlı sonuçla karşılaştır. Split satırında boyut adı ve hedef kullanıcı önemi yazılmış; hedef kitlede sık olduğu ifadesi ölçüm değildir, yorum olarak işaretlensin. Fiyat sunumu Alma satırının rakip yaklaşımı mı bizim fiyat kararımızı mı değerlendirdiğini ayır. Sonuçtaki tek nokta hükmünü bütün alanlar taranmadan kesinleştirme; formdaki kararlar Belge 3 onayı değildir | B19; Belge 3 adayları; README sözlüğü |
| QB-Q12 | Dört kareye yapılan kısa atıflar tam kimliğe bağlı mı; 12 Eylül Faz 7.5 beyanı neyi kapsıyor? | Formdaki kısa dosya adlarını E0268–E0271 tam yollarıyla eşle; E0267 .gitkeep'i görsel sayma. 13 Eylül denetiminde QuickBooks için tam ad/sorun sayısının sıfır çıkması ve yalnız E0271'in açılması tüm kareler için içerik onayı değildir. Faz 7.5 tarih satırının hangi kontrolleri kapsadığını kayıtla bağla veya sınırla | B17 mekanik kapı; G paketi; Belge 1/2 yöntem |

### Bu paket sonrası kapsam

- Okundu: E0012 tam metin; QB-M01–QB-M13 ve QB-Q01–QB-Q12 kaydedildi.
- Yeni görsel incelemesi **0/4**, canlı test **0**, kapanan B bulgusu **0**.
- B13 ürün/paket ayrımı her harita satırına bağlam olarak işlendi ve QB-Q01/Q03'e
  bağlandı; B17 atıf/mekanik kapı, B19 özet ve kanıt girişi yayılımıdır.
  B01–B04, B07 ve B12 bağlantıları ortak kontrol ilkesidir, yeni hata doğrulaması değildir.
- Kaynak form ve görseller değişmedi; ödeme/deneme kapısı aşılmadı, dış kaynak
  açılmadı. Masa başı sınırı korundu; ürün kodu/PRD/ADR değişmedi.
- Yarım bölüm yok. Sıradaki tek paket **P0.3-kolaybi (E0008)**.
  P0.4 ve G/T paketleri başlamadı; Faz 8 açılmadı.


## P0.3-kolaybi — Tam metin içerik haritası

**14 Eylül 2026: E0008'in 799 satırının tamamı okundu.** Kaynak:
[KolayBi formu](gozlemler/kolaybi.md). Okunan SHA-256:
`b0233d1959691fc9644ca925500baa2aff48e8412d13ea04f686b56ec8221944`.
Kaynak form değişmedi. Bu paket **metin kapsam haritasıdır**; destek
mockup'larının içeriği, video/kaynak güncelliği veya ürün davranışı yeniden
doğrulanmadı. Form uzun olduğu için bölüm bazında okundu; tek pakette kapandı.

### Kaynak bağlamı ve sınır

- Form mobil sürüm 3.3.1 (React Native), emulator-5554, Türkçe (TR/EN)
  bildiriyor. Mobilde kayıt yok; K01–K08 canlı koşulmamış. Bu tur uygulama
  açılmadı, sürüm/paket kimliği kontrol edilmedi.
- Tarih ve kaynak katmanları beştir ve birbirinin yerine kullanılmaz:
  1 Eylül emülatör giriş ekranı (manuel), 2/9 Eylül kolaybi.com ve kılavuz
  notları, 10 Eylül ~2020 tanıtım videosu + kullanıcı transkripti, 2026 başı
  yüklendiği söylenen ikinci videodan tek kare, 12 Eylül destek merkezi
  taraması ve Faz 7.5 beyanı. Karelerdeki 2020/2023/2024 tarihleri demo
  işlem verisidir; video yayın tarihi veya arayüz sürümü değildir (B15).
- Kanıt havuzu **E0180–E0218: 39 PNG**. E0180 mobil giriş; E0181–E0186
  eski video; E0187 ayrı 2026 videosu; E0188–E0218 destek mockup'ları
  (d01–d31). E0179 yalnız .gitkeep dosyasıdır. Tam yollar
  [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Bu pakette 0/39 görsel açıldı;
  E0184/E0186/E0188/E0194/E0206 içerikleri yalnız 13 Eylül denetiminin
  B08/B15 kayıtlarından aktarılır, bu turun gözlemi değildir.
- Formda doğrudan HTTP(S) URL'si yok. kolaybi.com, şemasız
  `kolaybi.com/destek/<modül>` kalıbı, on bir modül sayfa adı ve ilk video
  için YouTube kimliği var; 2026 videosunun kimliği, sayfa güncelleme ve
  video yayın tarihleri yok. URL tahmin edilmedi, web taraması yapılmadı (KB-Q02/Q04).
- Destek taraması 117 mockup'tan 31'ini repoya almış; kalan 86 mockup'ın
  incelendiği beyanı yerel kanıtla izlenemez. Ayarlar ekranı ve bütün alan
  davranışları formun kendi kaydında doğrulanamadı. Masa başı sınırı korunur;
  bu paket hesap açma, video izleme veya kaynak indirme yapmaz.
- Form uzun ve bazı bölümleri kendi içinde çelişiyor (ör. kısmi tahsilat
  "kapandı" ile "doğrulanamadı" aynı formda). Harita çelişkiyi düzeltmez,
  ilgili KB-Q satırına aktarır.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. Kısa kare kodları form ifadesidir; kanıt
sütunu tam E kimliğini verir. Belge 3 bağlantıları onay değil, ileride onaylı
Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| KB-M01 | Oturum bilgisi 3 | Geliştirici, QNB ekosistemi/yatırım, eski ad NKolayOfis ve noffix; mobil 3.3.1; beş tarih katmanı; TR/EN; mobilde kayıt yok; giriş manuel + tanıtım videosu + 2026 tek kare inceleme türü | E0008; E0180 giriş; E0181–E0187 video kareleri | Belge 1 profil/yöntem; B15, KB-Q01/Q02/Q03 |
| KB-M02 | Ürün kimliği 15 | Online ön muhasebe tezi; şahıs şirketi/KOBİ/muhasebeci hedefi; 40.000+ işletme; kişisel bütçe/yatırım kapsam dışı; 14 gün deneme; proje boyutunun kapsamla akrabalığı; tek havuz yokluğu | E0008 site beyanı; tam URL yok | Belge 1 ürün; Belge 2 kapsam; B19, KB-Q02/Q12 |
| KB-M03 | Görev gözlemleri 26; giriş öncesi arayüz 34 | 16 KB uyumluluk uyarısı; e-posta/şifre/QR/dil; kayıt bağlantısı yokluğu kullanıcı doğrulaması; K01–K08 engeli; renk, zorunlu alan ve QR özgün öğe | E0180; kayıt yokluğu kullanıcı aktarımı | Belge 1 giriş; KB-Q01 |
| KB-M04 | Resmî kaynak — özellikler ve kapsam 41 | Web+mobil bulut, sınırsız kullanıcı; finans/e-belge/stok/proje/entegrasyon/OCR/muhasebeci/rapor tablosu ve BF sütunu; PLUS kampanyası; Logo/Paraşüt ile aynı kategori | E0008 kolaybi.com 2 Eylül beyanı; PNG yok | Belge 2 özellik; B19, KB-Q02/Q13 |
| KB-M05 | Sistem işleyişi 81; pipeline 100 | Modüller; kayıttan cari/kasa/stok/projeye entegre yansıma; proje kâr marjı; tekrarlayan otomatik üretim; işletme/şahsi ayrımı yok ve Ortaklar/Personel carisi çıkarımı; entegrasyon/API/bulut; kapsamla aynı mimari fikir yorumu | E0008 site/kılavuz beyanı; PNG yok | Belge 2 olay modeli; B08/B12 ortak ilke, KB-Q05/Q09/Q12 |
| KB-M06 | Video akış kurulumu 110; kanıt gücü 124; destek sayı notu 133; transkript özeti 144 | Video ~2020 ve 08 ayrı kaynak; demo kiracı; alan adı okunur, davranış okunmaz; destek mockup'larında üç aritmetik kontrol ve banka/güncel bakiye eşleşmesi; bulut, özellik, pano, kurulum, entegrasyon ve e-Fatura özeti | E0181–E0187; aritmetik E0213/E0214/E0216 ve E0206 | Belge 1/2 kaynak gücü; B15, KB-Q03/Q07 |
| KB-M07 | Ekran seviyesi akış 165: 02–04 | 13 modül ve üç vade donutu tutarları; Günü Gelen İşlemler sayaçları ve gecikmiş dolar kalemi; vadesi belirsiz tahsilat listesi ve Kısmen Ödendi kaydı; readiness/attentionCode, sourceGroup, CounterpartyPayment/Balance eşlemeleri | E0181–E0183 | Belge 1 pano; Belge 2 vade/tahsilat; B01, KB-Q07/Q08/Q10/Q14 |
| KB-M08 | Ekran seviyesi akış 165: 05–08 | Cari sekmeleri ve Ortaklar; ürün tablosunda KDV/İndirim kolonu (boş); Kasalar dört sekme ve açılış tarihi; 2026 panosunda Fatura Ödeme, 1 haftalık grafik, çapraz satış ve iWallet; altı yıllık iskelet istikrarı yorumu | E0184–E0187 | Belge 1 sürüm karşılaştırma; B15, KB-Q03/Q12/Q13 |
| KB-M09 | Ekrandan ekrana pipeline 177; karşılaştırma notu 185 | Kurulum→Hızlı İşlemler→proje→pano→rapor→müşavir; ortak carisinin yerleşik muhasebe cevabı; üçlü vade; kısmi ödemenin gerçek veriyle doğrulandığı; KDV'nin ürün kartından geldiği çıkarımı; proje ekranının videoda olmaması | E0181–E0185, E0194 | Belge 2 akış; B01/B08, KB-Q08/Q11 |
| KB-M10 | Faz 6 destek taraması 209; Proje modülü 225 | 12 modül/117 mockup/31 kare yöntemi; proje listesinde Gelir/Gider/Net; yeni proje formu; detayda Kar/Zarar–Nakit Durumu ve Tahsil Edilen/Bekleyen; belge kırılımı ve nakit yönlü renk; BF netinin yalnız nakit esaslı olduğu kıyası | E0188–E0191 | Belge 1 proje; Belge 2 tanıma/tahsil; B04, KB-Q04/Q05/Q10 |
| KB-M11 | Beklenmedik bulgu — proje ekseni 234 | Demo proje adlarının hane harcaması olduğu; kullanıcıların ekseni amacı dışında kullandığı; ihtiyaç gerçek/esnek eksen iki okuması | E0188 | Belge 3 adayı; B08, KB-Q11 |
| KB-M12 | Genel Gider modülü 258; gider tipleri 276 | Cari Takibi Yok/Var ve Ödeme Durumu radyosu; vade girilmemiş varsayılanı; dört formda Proje Takip notu; açıklama şablonu; çift para birimi; 5 MB ek ve OCR izi yok çıkarımı; KDV'nin satırdan hesaplanması; iki seviyeli kategori; vergi türlerinin gider tipi olması | E0192–E0193; not tekrarları E0195, E0200, E0217 | Belge 2 gider/tanıma-taşıma; KB-Q05/Q06/Q09/Q13 |
| KB-M13 | Cari modülü ve cari ekstre 291 | Beş sekme, Cari Tipi, işaretli Yerel Bakiye; iki adımlı oluşturma, cari açılış bakiyesi ve vade günü; Borç/Alacak Ekle ile Ödeme/Tahsilat Ekle ayrı düğmeler, Mahsuplaştır, Pasifleştir/Sil; ekstre diyaloğu, PDF önizleme ve e-posta | E0194–E0197 | Belge 2 cari/ekstre; B01, KB-Q08/Q10 |
| KB-M14 | Finans modülü 301 | 2020 karesinden gelen kredi kartı yok tespitinin düzeltilmesi ve iki yeni sekme; banka IBAN/toplam; boş kart listesinde Kalan Limit çıkarımı; kart formunda zorunlu limit ve asgari ödeme oranı; çek ve senet defterleri, senet taksit çıkarımı | E0186, E0206–E0210 | Belge 2 kart/kıymetli evrak; B02/B15/B19, KB-Q03/Q10 |
| KB-M15 | Personel carileri ve maaş 319; tekrarlı maaş 362 | Çalışan tipleri ve karışık kod öneki; maaş/prim/avans eylemleri; pozisyona göre renk; ödenmemiş maaş tahakkukunu seçen ödeme diyaloğu; proje nakit bölümü uyarısı; brüt/net; zorunlu tekrar sayısı ve onay yokluğu çıkarımı; BF süresiz plan kıyası | E0201–E0205 | Belge 2 personel/tekrar; B01/B03, KB-Q08/Q10 |
| KB-M16 | Genel Gider listesi ve kayıt eylemleri 378 | e-Fatura durumu; Bedelsiz üçüncü hâl; Bakiye kolonunun ters deseni; boş Son Ödeme Tarihi; Tahsilata Kapalı/Aç, Dönüştür, Kopyala, Taslak, Ödeme Planı; GELİR/GİDER başlığı; bizde olmayan dört fikir | E0198–E0199, E0204 | Belge 1 kayıt eylemleri; Belge 2 durum; B02, KB-Q06/Q10 |
| KB-M17 | Satış / Satın Alma formları 426 | Tekrarlı satış/alış sekmeleri; alış faturasında zorunlu cari, ödeme durumu ve saat; d30 üç farkı ve dosya adı/başlık uyuşmazlığı; fatura = carisi zorunlu satır kalemli gider sonucu | E0200, E0217 | Belge 2 fatura; KB-Q09/Q16 |
| KB-M18 | Raporlar 456 | On rapor; Alış/Satış raporunda şube boyutu ve KDV Dahil; Hesaplanan/İndirilecek KDV matrisi ve kazanım/bedel; kapsamsız Gelir/Gider raporu; Nakit Akış raporunda tahmini dönem sonu ve tümü Belirsiz kovası | E0213–E0216 | Belge 2 rapor/vergi; Belge 3 adayları; KB-Q07/Q10/Q13/Q14 |
| KB-M19 | Güncel Durum ve diğerleri 469; Tur 2 katkısı 494; kaynak envanteri 505 | Dolu d24 grafiği ve 08'den eski sürüm çıkarımı; notlar/hatırlatıcı; boş varyantlar; Tur 2 konularında Kapandı etiketleri; modül başına mockup sayıları ve indirme yöntemi; Ayarlar/davranış açıkları; kullanıcıdan beklenen görseller | E0211–E0212, E0218; karşılaştırma E0187 | Belge 1/2 yüzey; B15, KB-Q03/Q04/Q05/Q08 |
| KB-M20 | Faz 7.5 532; örtüşme denetimi 605; kanıt atıfı borcu 625 | 39 karenin açıldığı beyanı; sayım/etiket ve kanıt seviyesi düzeltmeleri; keskinleşen bulgular; silinecek kare yok; dört eski/yeni çift ve rolleri; 118 kısa atıf/altı aralık düzeltmesi; denetim.sh kör noktaları | E0180–E0218 kapsamı; çiftler E0181/E0211/E0187, E0184/E0194, E0185/E0218, E0186/E0206 | Belge 1/2 yöntem; B15/B17, KB-Q03/Q16 |
| KB-M21 | BusinessFinance için kararlar 647 | 20 satır: QR, proje boyutu, ortak carisi, üçlü vade, Günü Gelen İşlemler, katalog/KDV, mobil kayıt, muhasebeci, tahmini bakiye, Dönüştür, tekrar sayısı, Mahsup, ekstre, Kar/Zarar–Nakit ve KDV için kararı yeniden sor, asgari ödeme, taslak/kopya, iki seviyeli kategori, kullanıcı tanımlı eksen, şube | E0008 karar tablosu; ilgili KB-M kayıtları | Yalnız Belge 3 adayları; B02/B03/B04/B08/B19, KB-Q10/Q11/Q17 |
| KB-M22 | Kanıt ve güven düzeyi 680 | Manuel gözlem yalnız giriş; video kimliği ve ~2020 sürüm; 08 tek kare; site/kılavuz tarihleri; destek taraması; demo kiracı ve tek dolu ekranın 04 olduğu; doğrulanamayan davranışlar; opsiyonel kullanıcı görselleri | E0008; E0180–E0218 kapsamı | Belge 1/2 yöntem; B15/B17/B19, KB-Q01/Q02/Q06/Q07 |
| KB-M23 | Tek cümlelik sonuç 718; video transkripti 730; yöntem notu 796 | Tahakkuk/nakit ayrımının ADR 0014'ün en olgun karşılığı olduğu; tahmini bakiyenin bizde olmadığı; ortak carisi + proje ekseniyle farklı cevap; kaba zaman damgalı tam transkript; video yöntemi | E0008; transkript bağlamı E0181–E0186 | Belge 1/2 özet/kaynak; B01/B08/B19, KB-Q02/Q11/Q12 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**KB-Q01–KB-Q17'nin tamamı açık.** B02/B03/B04 doğrudan KB-Q10'a, B08
KB-Q11'e, B15 KB-Q03'e, B17 KB-Q16'ya bağlandı; bu paket bunları kapatmaz.
B01–B19 yeniden numaralandırılmadı. Kaynak/kod/görsel kontrolleri sonraki paketlerdir.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| KB-Q01 | Mobil 3.3.1, 16 KB uyarısı ve kayıt yokluğu hangi paket/tarihe ait; güncel mobil arayüz hakkında ne söylenebilir? | E0180 giriş karesini, kullanıcının kayıt yokluğu doğrulamasını ve emülatör uyarısını ayrı kaydet. Paket kimliği/sürümü kaynağa bağla veya bilinmiyor işaretle. Web destek mockup'larından mobil özellik veya mobil tasarım sonucu çıkarma; hesap açma/kayıt denemesi yapma | Belge 1 profil/erişim; G/kaynak |
| KB-Q02 | Şirket geçmişi, yatırım, 40.000+ işletme, 14 gün deneme, PLUS kampanyası ve video tarihleri hangi tam kaynağa dayanıyor? | Her sağlayıcı beyanını tam URL, erişim ve yayın tarihiyle eşle; müşteri sayısını pazar/kullanım verisi sayma. İlk videonun kimliği var, 2026 videosunun kimliği ve yüklenme tarihi yok; ~2020 tarihlemesi demo işaretlerinden çıkarımdır. Güncel fiyat/kampanya eski erişimle doğrulanmış sayılmaz | Belge 1/2 kaynak kapısı; B19; kaynak |
| KB-Q03 | 01–08'in kaynak/tarih rolleri ve eski/yeni kare çiftleri doğru eşleniyor mu? | Satır 165 başlığı ve 609 ifadesi 01–08'i 2020 video karesi gibi topluyor; 01 mobil, 08 ayrı video. 05/d07 cari listesi, 07/d19 Kasalar ile Banka Hesapları farklı sekmelerdir; aynı ekranın iki sürümü sayılmasın. d24'ün 08'den eski olduğu kampanya yüzeyi farkından çıkarım; destek karelerinin 2023+ olduğu dayanağı demo tarihinden ayrılsın. Arşiv/seçim önerisi silme yetkisi değildir | B15 / P1-B15; G paketi |
| KB-Q04 | Destek taraması bütün 117 mockup'ı ve 12 modül sayfasını ne ölçüde kanıtlıyor? | Repodaki 31 kare ile repoya alınmayan 86 mockup'ı ayır; sayfa adları, sayfa güncelleme tarihi ve indirme yöntemini kaynak kaydına bağla. On bir sayfa + mockupsuz Ayarlar sayımını koru. Kapandı etiketlerini form/alan seviyesiyle sınırla; bu pakette indirme yapılmadı | Belge 1/2 kaynak kapsamı; kaynak |
| KB-Q05 | Proje ekranındaki Kar/Zarar–Nakit Durumu, Tahsil Edilen/Bekleyen ve Proje Takip anahtarı hangi davranışı gösteriyor? | Sekme ve kart adlarını tahakkuk/nakit hesap kuralı sayma; kayıtların projeye nasıl aktığını ve kâr marjı hesabını kaynakla doğrula. Proje Takip kapatma notu Ayarlar ekranı görülmeden etkisinin ürün genelinde tutarlı olduğunu veya bizim onboarding deseniyle aynı olduğunu kanıtlamaz | Belge 2 proje/tanıma; kaynak/G; plandaki Ayarlar/Proje Takip açığı |
| KB-Q06 | Ödeme Durumu, Cari Takibi, Bedelsiz, Tahsilata Kapalı ve Bakiye kolonu kasa/cari/rapor etkisi olarak ne anlama geliyor? | Radyonun aynı kayıtta tanıma ve taşıma yaptığı, Ödendi işaretinin kasa/banka bakiyesini değiştirdiği formun kendi kaydında doğrulanmadı. d11 Bakiye deseni gözlem, ödenen tutar açıklaması çıkarımdır; Bedelsiz satırı açık kalır. Kaynak veya kullanıcı görseliyle ayrı ayrı kapat; canlı erişim yokken davranışı tahminle kesinleştirme | Belge 2 tanıma/taşıma; plandaki Ödendi/kasa açığı; kaynak/G |
| KB-Q07 | Karelerden sayı okumanın güvenilirliği ve tek dolu ekran hükmü nasıl sınırlanmalı? | Üç aritmetik kontrol ve bir bakiye eşleşmesi 31 destek karesinin tamamı için sayı güvenliği kanıtı değildir. Satır 699'daki tek dolu ekran 04 ifadesi video karelerine mi bütün korpusa mı ait ayır; destek karelerinde dolu tablolar var. Çoklu döviz ve donut tutarlarını demo veriyle sınırla; Paraşüt kalite kıyasında kaynak türü farkını yaz | Belge 1/2 kaynak gücü; G paketi |
| KB-Q08 | Fatura→tahsilat bağı ve kısmi tahsilat gerçekten kapandı mı? | Tur 2 tablosu (500–501) ikisini kapandı sayarken 524 ve 705 kısmi tahsilatın bakiyeye etkisini doğrulanamadı diye bırakıyor; 201 demo kaydını gerçek veriyle doğrulama sayıyor. İki ayrı düğme ve yürüyen bakiye kolonu ödemenin belirli faturayı kapattığını kanıtlamaz. Maaş ödeme diyaloğu tahakkuk seçimi gösteren tek mockup'tır; Bluecoins/Wallet kıyası B09/B10 sonuçlarına bağlıdır | B01 / P1-B01; B09/B10 ortak ilke; plandaki kısmi tahsilat açığı; kaynak/G |
| KB-Q09 | Kayıt türü ve tanıma/taşıma kıyasları formun içinde tutarlı mı? | Satır 266 bizde iki ayrı kayıt türü olduğunu, 453 ayrımın kayıt türüyle değil cariye bağlanmayla yapıldığını söylüyor. Çift sayımın bizde yapısal olarak engellendiği iddiasını kodla kontrol et. Entegre yansıma ve GELİR/GİDER başlığından fiziksel kayıt/şema veya serbest gelir kaydı sonucu çıkarma. d30 farklarının hangi faturaya ait olduğunu KB-Q16 ile birlikte çöz | B12 ortak ilke; Belge 2 olay modeli; kaynak/kod |
| KB-Q10 | BF mevcut yetenek kıyasları kod ve ADR ile uyuşuyor mu? | B02: kullanılabilir limit, asgari ödeme oranı, zorunlu limit ve Bedelsiz→sıfır tutar (314–315, 386, 674). B03: süresiz plan/opsiyonel bitiş yokluğu (372, 669). B04: yalnız nakit esaslı işletme neti (231, 672). Ayrıca cari açılış devri, cari ekstre, Ödeme Planı–InstallmentPlan, taslak/kopya/dönüştür, brüt kayıt tutarı, scopeBreakdown, tahmini bakiye ve readiness/attentionCode/sourceGroup birebir eşlemelerini kodla doğrula. Mevcut özelliği yeni aday, olmayanı mevcut gibi yazma | B02/B03/B04 / P1-B02–B04; Belge 3 mevcut durum; kod/ADR |
| KB-Q11 | Demo proje adları kullanıcı davranışı veya talep kanıtı sayılabilir mi? | d01 adları demo gözlemidir; kullanıcıların ekseni hane gideri için kullandığı, aynı ihtiyacın üçüncü izi ve birden çok raporlama boyutu talebinin kanıtı ifadeleri (234–256, 660, 677, 726) hipotez olarak ayrılsın. Ortaklar sekmesinin varlığı gözlem, patron parasının oradan girmesi çıkarımdır. Yeni kullanıcı araştırması bu işe dahil değil | B08 / P1-B08; Belge 1/3 ihtiyaç gerekçesi |
| KB-Q12 | Formdaki doğrulayıcı/hüküm dili README kuralına uyuyor mu? | ADR 0014'ün en açık/en olgun karşılığı, kararımız için olumlu istikrar sinyali, kararımızın rakipte izolasyonu, Aşama 05 kararını desteklediği, aynı mimari fikir ve birebir ifadelerini gözlem, çıkarım ve Belge 3 tartışmasına ayır. Altı yıllık iskelet değişmedi hükmü ara sürümler görülmeden kesinleşmez | B19; Belge 1/2 anlatım; metin |
| KB-Q13 | KDV raporu, ürün kartı KDV alanı, OCR ve muhasebeci erişimi hangi yeteneği gösteriyor? | Matrisin KDV'yi oranlardan hesapladığını mı girilmiş KDV'yi topladığını mı gösterdiğini kaynakla ayır; iki Toplam KDV için tevkifat açıklaması çıkarımdır. Hedef kitlenin bu beyannameyi verdiği iddiasını PRD ile, orta yol tartışılmadı iddiasını ADR 0016'nın mevcut taşıma/raporlama ve muhasebeci paketiyle karşılaştır. Özellik listesindeki Fiş OCR (63) ile web formunda iz yok çıkarımı (273) uzlaşsın; muhasebeci canlı erişimini Paraşüt/Logo ile aynı sayma; sanal POS ≠ PosSettlement ayrımı kodla kontrol edilsin | B19 yetki yayılımı; Belge 2 vergi/entegrasyon; Belge 3 adayı; kaynak/ADR |
| KB-Q14 | Üçlü vade ve tahmini dönem sonu bakiyesi ne ölçer; bizde karşılığı gerçekten yok mu? | Donut tutarı ile sayaç adedini, Belirsiz kovası ile boş vade alanını ayrı kaydet. Tahmini Dönem Sonu formülü karedeki aritmetikten türetilmiştir, kaynak tanımı değildir; Ödemeler ₺0 ve tümü Belirsiz demo koşuludur. BF planlanan görünüm/upcoming modelinde tek toplam veya vadesiz hâl olup olmadığını kodla kontrol et | Belge 2 planlama/rapor; Belge 3 adayı; G/kaynak/kod |
| KB-Q15 | Tekrarlı planlar ve maaş tahakkuku otomatik mi, onaylı mı; dört tekrar sekmesi aynı kuralı mı izliyor? | d18 yalnız maaş formudur; periyot değerleri görünmüyor, onay adımı yokluğu çıkarımdır. Tekrarlı Genel Gider/Alış/Satış listelerinin üretim ve onay kuralını maaş formundan genelleme. Zorunlu tekrar sayısını B03'teki BF opsiyonel bitiş gerçeğiyle kıyasla | B03 ortak ilke; Belge 2 tekrar; plandaki tekrarlayan liste açığı; kaynak/G |
| KB-Q16 | Kare adı ve atıf düzeltme beyanları bugünkü formla uyuşuyor mu? | Satır 641 kısa kodların tamamının ve altı aralığın düzeltildiğini söylüyor; 217 ve 694'te d01–d31 aralık atfı, pek çok yerde 01–08 kısa kodu duruyor. d30 dosya adı bizim adlandırmamızdır, karedeki Alış İade başlığı ürünün kendi uyuşmazlığıyla aynı şey değildir. 534 tüm kareler açıldı ve 602 denetim ölü atıf bulmuyor beyanları B17 nedeniyle güncel içerik onayı sayılmaz. Bu pakette yeniden adlandırma/düzeltme yok | B17 / P1-K; G paketi; Belge 1/2 yöntem |
| KB-Q17 | Karar tablosu etiketleri ve gerekçeleri README sözlüğüne uyuyor mu? | Alma satırlarından katalog/KDV, mobil kayıt ve muhasebeci erişimi (664–666) kazandırdığı/kaybettirdiğini yazmıyor; Alma (biçim) tanımlı sonuç değil. İki kararı yeniden sor satırında boyut adı var, hedef kullanıcının veresiye satması ve KDV beyannamesi vermesi ölçüm değil yorumdur. 669/674 satırları B03/B02 sonucu gelmeden öneri sayılmaz. 43–45 ve 649 notlarına uygun olarak tablodaki kararlar Belge 3 onayı değildir | B19; Belge 3 adayları; README sözlüğü |

### Bu paket sonrası kapsam

- Okundu: E0008 tam metin; KB-M01–KB-M23 ve KB-Q01–KB-Q17 kaydedildi.
- Yeni görsel incelemesi **0/39**, canlı test **0**, kapanan B bulgusu **0**.
- B02/B03/B04/B08/B15/B17 doğrudan bağlandı; B01 tahsilat kıyası ve B19 özet,
  fiyat ve yetki yayılımıyla bağlı. B09/B10/B12 bağlantıları ortak kontrol
  ilkesidir, yeni hata doğrulaması değildir. Plandaki KolayBi açıkları
  (Ayarlar/Proje Takip, tekrarlayan liste, kısmi tahsilat, Ödendi→kasa)
  KB-Q05/Q06/Q08/Q15'e bağlandı.
- Kaynak form ve görseller değişmedi; kayıt, video izleme ve destek sayfası
  indirme yapılmadı. Masa başı sınırı korundu; ürün kodu/PRD/ADR değişmedi.
- Yarım bölüm yok. Sıradaki tek paket **P0.3-bluecoins (E0005)**.
  P0.4 ve G/T paketleri başlamadı; Faz 8 açılmadı.


## P0.3-bluecoins — Tam metin içerik haritası

**14 Eylül 2026: E0005'in 362 satırının tamamı okundu.** Kaynak:
[Bluecoins formu](gozlemler/bluecoins.md). Okunan SHA-256:
`b9eed5f684d8bd69f1eb019472eab57794a7867d2ecd1e1c126b09e869f9a059`.
Kaynak form değişmedi. Bu paket **metin kapsam haritasıdır**; P2 tam Faz 7.5
koşumunun yerine geçmez, eski canlı sonuçları yeniden doğrulamaz.

### Kaynak bağlamı ve sınır

- Form Bluecoins Finance & Budget / Mabuhay Software 13.1.45
  (versionCode 33111), emulator-5554 1080x2400, Türkçe/TRY, yerel ücretsiz
  sürüm bildiriyor; Faz 3 kurulumu 7 Eylül. Bunlar form beyanıdır; bu tur
  emülatör açılmadı, cihaz verisi ve ayarlar okunmadı veya değiştirilmedi.
- Koşum katmanları: 1 Eylül Tur 1 (eski PC), 10 Eylül Faz 3 tam yeniden koşum
  (yeni emülatör, bulut yok), 11 Eylül Faz 7 derin koşum. Oturum tablosundaki
  test tarihi satırı 11 Eylül'ü saymıyor. İşlem dönemi Ağustos 2026; açılışlar
  10 Eylül tarihli. Formda **Faz 7.5 bölümü yok**; DURUM bu doğrulamanın
  tamamlanmadığını kaydediyor.
- Kanıt havuzu **E0016–E0105: 90 PNG**. Form atıflarına göre 00–09
  (E0016–E0025) Tur 1, 10–34 (E0026–E0050) Faz 3, f7-00–f7-54 (E0051–E0105)
  Faz 7. E0015 yalnız .gitkeep dosyasıdır. Tam yollar
  [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Bu pakette 0/90 görsel açıldı;
  E0082/E0083/E0104 içeriği yalnız 13 Eylül denetiminin B16 kaydından aktarılır.
- Faz 7 bölümünde iddia başına kare atfı yok; f7 kareleri yalnız güven
  bölümündeki aralıkla bağlı. Aşağıdaki Faz 7 eşlemeleri **dosya adı adayıdır**,
  içerik eşlemesi değildir (BC-Q10).
- Formda HTTP(S) URL'si ve resmî kaynak yok (`Resmî kaynak: —`). Premium
  dışa aktarma satın alınmamış; yedek/geri yükleme, widget ve Seyahat Modu
  denenmemiş. Bu sınırlar ürünün bu özelliklere sahip olmadığı biçiminde yazılmaz.
- Veri koruma: Faz 3 verisi sıfırlanmadan Faz 7 kayıtları eklendi. Tekrarlayan
  seri ve hatırlatıcılar tarihe bağlı olduğundan bugünkü cihaz durumu formdaki
  son durumdan farklı olabilir; P2'de salt okunur başlangıç kaydı olmadan
  onay/gerçekleştirme tekrarlanmaz.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. Belge 3 bağlantıları onay değil, ileride
onaylı Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| BC-M01 | Oturum bilgisi 3 | Ürün/sürüm; test tarihleri; emülatör/dil; yerel ücretsiz sürüm, bulut yok ve Faz 3'te veri olmaması; hazır ₺0 örnek hesaplar; Premium dışa aktarma; klasör diyaloğu kilidi ve force-stop kurtarması | E0005; E0016 mağaza adayı | Belge 1 profil/yöntem; BC-Q01/Q09/Q14/Q15 |
| BC-M02 | Görev gözlemleri 15, K00–K04 | Kayıtsız başlangıç ve demo dosyası; yatay sekmeli ana ekran; geniş hesap türü ağacı, açılış bakiyesi+tarihi; yoğun tek form, varsayılan GİDER; gelir/giderde kapsam alanı yok | E0016–E0021, E0025 | Belge 1 giriş/form; Belge 2 kapsam; BC-Q01/Q15 |
| BC-M03 | Görev gözlemleri K05–K08 | Kart seçilince taksit alanı ve harcama tarihinde gider; ayrı transfer türü, iki bağlı satır ve ₺0 gün toplamı; gün netli liste ve raporlar; yerinde düzenleme, kalıcı silme, sıfır tutarın uyarısız kaydı | E0022–E0024 | Belge 2 kart/transfer/düzeltme; B10, BC-Q03/Q06/Q12 |
| BC-M04 | Kontrol değeri 29; Faz 3 kontrolü 43 | Tur 1 kontrol tablosu; Faz 3'te 3 hesap + 5 işlemin yeniden girişi; Ağustos 25.000/−2.050/22.950 ve Eylül net varlık 44.950; açılış tarihinin aydan öncesine çekilememesi | E0027, E0031, E0032 | Belge 2 çekirdek sonuç; B18, BC-Q01/Q02/Q07/Q11 |
| BC-M05 | Arayüz taraması 54 | Sekmeler/alt görünümler; rapor drill-down; bütçe/planlama kısmen; dashboard kartları; arama/filtre kısmen; boş durum; sıfır tutar ve silme onayı; widget denenmedi | E0017–E0018, E0023–E0025; tablo 3 sütun başlık/4 hücre veri | Belge 1 yüzey; B18, BC-Q11/Q15 |
| BC-M06 | Arayüz incelemesi 67 | Kart tabanlı pano ve liste hiyerarşisi; yatay sekme taşması; renkler; para biçimi; yoğun tek form; sessiz başarı; bilişsel yük yorumu | E0005 metin; satır başına kare atfı yok | Belge 1 desenler; BC-Q13/Q14 |
| BC-M07 | Akış özeti 80 | Tek ekran ayrıntılı giriş; hesap seçim sürtünmesi; ileri kişisel bütçe hedefi; kapsam yok; nötr transfer; plan/hatırlatıcı/taksit ve fatura bağı olmayan cari hesap özeti | E0005 sentez; ilgili BC-M kayıtları | Belge 1/2 özet; B01, BC-Q05/Q13 |
| BC-M08 | Faz 3 boşluk koşumu 89: işlem formu 93, kredi kartı modeli 101 | Formun tüm alanları ve alt bar; hesap makinesi donması; kart = Cari Hesap grubunda negatif hesap; kart ödemesi transfer, kısmi ödeme küçük transfer; ekstre döneminin aranmadığı | E0028; kart ödemesi E0050 | Belge 1 form; Belge 2 kart; BC-Q06/Q14 |
| BC-M09 | Taksit B2 109 | Taksit alt sayfası (oran, ay, ilk ödeme); 6.000 → 6×1.000; 1/6'nın ilk ödeme tarihinde anında gider olması; 2/6–6/6 hatırlatıcı; InstallmentPlan'a çok yakınlık ve ilk taksit farkı | E0033, E0035–E0038; E0034 metinde anılmıyor, ad adayı | Belge 2 taksit; B10, BC-Q02/Q04/Q12 |
| BC-M10 | Tekrarlayan B1 130 | Sıklık/son tarih/otomatik kutusu; geçmiş başlangıç; otomatik kutu kapalıyken tanımın kayıt üretmemesi; gecikmeli/bugün/gelecek hatırlatıcılar; Bugün/planlanan tarih onayı; 3.650 gider ve 44.350→43.350 net; RecurringTransaction ile birebir ve en yakın model hükmü | E0039–E0044 | Belge 2 planlama; B10, BC-Q02/Q04/Q07/Q13 |
| BC-M11 | Transfer detayı 157; Hatırlatıcılar 162; kart kesim günü 168; Bölmek 177; taslak uyarısı 184 | Benzer işlemler/Yinelenmek; tarih sıralı birleşik bekleyen liste ve attentionCode benzerliği; Kredi Limiti/Hesap Kesim Günü/Bitiş tarihi alanları; çok satırlı bölme; kaydetmeden çıkış onayı | E0045, E0048; hatırlatıcı listesi adayı E0047; transfer detayı ad adayları E0029–E0030 (metinde anılmıyor); taslak için atıf yok | Belge 1/2 kart/plan/form; BC-Q06/Q12 |
| BC-M12 | Bağımsız hatırlatıcı 189; cari hesap 199 | Bir Defa planlı işlem olarak Ofis kirası ve tek liste; Alacaklar/Cari hesap türleri; özel alanı olmayan cari hesap; fatura nesnesi ve tahsilat bağı yok; veresiye modelleme anlatımı; BF fatura bağı kıyası | E0046–E0049 | Belge 2 plan/cari; B01, BC-Q05/Q12 |
| BC-M13 | Kısmi kart ödemesi 212; fiş/kamera 218; Faz 3 sonrası veri durumu 225 | 500 transferle kart borcunun −1.000→−500 olması; ataç eki, OCR görülmedi ve hit-alanı tuzağı; 39.700/4.150/−500/0 ve 43.350 durumu; A/B/B1/B2 kapanış ve veri koruma kararı | E0050; fiş için kare yok | Belge 2 kart/fiş; B10, BC-Q02/Q14/Q16 |
| BC-M14 | Faz 7 D1/D2/D3 237 | Ofis kirası tek onayla işleme dönüşüp Ana Hesap 39.700→29.700; D2 ₺12.000 cari hesaba sıradan gelir, durum alanı yok; D3 ₺5.000 cari→banka transferi, bakiyeler 7.000/34.700, faturaya bağ yok; kazanım/bedel ve BF takip garantisi kıyası | Ad adayları: başlangıç E0051–E0055, D1 E0056–E0063, D2 E0064–E0075, D3 E0076–E0079 | Belge 2 borç/tahsilat; B01/B09 ortak ilke, BC-Q02/Q04/Q05/Q10 |
| BC-M15 | Faz 7 arama 261; filtre 266; dışa aktarma 272 | Canlı arama ve eşleşen toplam ₺12.000 açıklaması; çok kapsamlı filtre ve kayıtlı profiller; beş uygulama içinde en gelişmiş filtre hükmü; PDF/Excel/HTML tek dokunuş iddiası | Ad adayları E0080–E0084, E0104–E0105; E0082/E0083/E0104 aynı filtre paneli (B16 kaynak denetimi) | Belge 1/2 arama/export; B16, BC-Q09/Q10 |
| BC-M16 | Faz 7 tam arayüz taraması 275 | Nakit Akım Ayarı ve ₺0 nakit akışı nedeni; varsayılan kategori/hesap ve gizli hesaplar; çoklu para birimi ayarları; iki seviyeli kategori; İş/Kişisel etiketleri; çöp kutusu; takvim drill-down; CSV/QIF içe aktarma; atlanan genel ayarlar | Ad adayları E0085–E0103 | Belge 1 ayarlar; Belge 2 rapor/aktarım; B10, BC-Q03/Q08/Q15 |
| BC-M17 | BusinessFinance için kararlar 308 | 32 satır: running balance/gün neti, açılış tarihi, dinamik kart alanı, dashboard, yoğun form/geniş hesap/sıfır tutar/kalıcı silme/kapsam yokluğu alma; tekrarlayan/hatırlatıcı/taksit doğrudan al; faiz, Yinelenmek, bölme bekletme; cari/fatura bağı, nakit akım ayarı, çoklu para alma; filtre/export/takvim uyarlama | E0005 karar tablosu; ilgili BC-M kayıtları | Yalnız Belge 3 adayları; B01/B02/B10/B19, BC-Q12/Q13 |
| BC-M18 | Kanıt ve güven 345; sonuç 355 | Tur 1/Faz 3/Faz 7 manuel koşum özetleri ve kare aralıkları; resmî kaynak yok; eski Yorum etiketiyle en yakın model hükmü; doğrulanamayanlar; en güçlü para modeli referansı sonucu | E0005; E0016–E0105 kapsamı | Belge 1/2 yöntem; B10/B17/B19, BC-Q01/Q04/Q10/Q13 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**BC-Q01–BC-Q16'nın tamamı açık.** B10 BC-Q02/Q03/Q04'e, B01 BC-Q05'e,
B16 BC-Q09'a, B17 BC-Q10'a, B18 BC-Q11'e bağlandı; bu paket bunları
kapatmaz. B01–B19 yeniden numaralandırılmadı. P2 tam koşumu bu soruları
görsel ve gerekirse tek soruluk canlı test paketlerine dönüştürür.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| BC-Q01 | Hangi gözlem hangi koşuma ve cihaza ait? | K00–K08 tablosu ve ilk kontrol tablosu Tur 1 (eski PC) ile Faz 3 yeniden koşumunu birlikte taşıyor; sıfır tutar, kalıcı silme ve demo önerisinin Faz 3'te tekrarlanıp tekrarlanmadığı yazmıyor. Test tarihi satırına 11 Eylül girmemiş. 00–09 / 10–34 / f7 kare gruplarını koşum tarihiyle eşle; sürümü cihazdan salt okunur doğrula | B17/B19; P2-M/P2-G |
| BC-Q02 | Bakiye/net zinciri tutarlı mı? | Satır 154 net varlığı 44.350→43.350 yazıyor; 44.950 − 1.000 ilk taksit = 43.950, −600 = 43.350 ve satır 227 ile uyumlu, yani başlangıç sayısı çıkarımla 43.950 olmalı. E0038/E0044 ile doğrula. D1–D3 sonrası hesap ve net varlık tablosu yok; D2'nin ₺12.000 cari bakiyesinin net varlığa etkisi yazılmamış. Satır 46–47'deki Net Kazanç etiketinin net varlık anlamını ekran adıyla ayır | B10 / P2-G; gerekirse P2-K |
| BC-Q03 | Silme kalıcı mı, çöp kutusuna mı düşüyor; geri yükleme var mı? | Satır 27/64/77/320 kalıcı ve geri alınamaz silme, 296–298 boş çöp kutusu ve bilinmeyen davranış diyor. Tur 1 silmesi eski cihazdaydı, Faz 7'de silme yapılmadı. Silme ve geri yükleme ayrı, izinli sentetik senaryolarda ele alınır; mevcut veri silinmez veya sıfırlanmaz | B10 / P2-T |
| BC-Q04 | Otomatik/onaylı gerçekleşme hangi moda aittir? | B1 testi otomatik kutusu kapalıyken yapıldı; B2 ilk taksiti anında yazdı ve ilk ödeme tarihi (15 Ağustos) test anından önceydi, anında yazımın tarih etkisi mi taksit kuralı mı olduğu ayrılmadı. Tanım üretmez ve birebir hükümleri (143–145, 323, 348, 352, 358) yalnız gözlenen moda sınırlanmalı. D1'deki tek TAMAM onayı ile B1'deki Bugün/planlanan tarih sorusu farklı diyaloglar; birebir aynı mekanizma (242) kanıtla bağlanmalı. Taksit otomatik kolu doğrulanamadı | B10 / P2-G, P2-T |
| BC-Q05 | Cari hesap/fatura/tahsilat kıyası BF gerçeğiyle uyuşuyor mu? | Satır 209–210, 257–259, 335 ve 337 BF'de fatura bağı ve takip garantisi varmış gibi yazıyor; B01 kaynağına göre CounterpartyPayment belirli borçlandırma taşımaz. Kod/ADR 0014 kontrolüyle ifadeyi mevcut yeteneğe indir. D2 gelirinin gelir raporu/tanıma zamanı ve D3 transferinin rapor nötrlüğü ayrı ölçülmedi. Arama toplamındaki bacakların birbirini götürmesi açıklaması çıkarım. Wallet karşılaştırması B09 sonucuna bağlıdır | B01 / P1-B01; B09 ortak ilke; P2-T; kod/ADR |
| BC-Q06 | Kart kesim günü ve ekstre dönemi ne ölçüde incelendi? | Satır 107 Faz 3'te ekstre kesim/dönem kavramının aranmadığını, 168–175 aynı fazda kesim günü alanının var olduğunu söylüyor. Alan varlığı ekstre projeksiyonu veya dönem davranışı kanıtı değildir; Bitiş tarihi alanının anlamı açık. Kısmi ödemede dönem mantığının devreye girmediği gözlemi varsayılan alanlarla sınırlı. Wallet ile aynı, önemsiz ifadesi yorumdur; MM/Wallet kıyasları kendi haritalarıyla uzlaşsın | Belge 2 kart; P2-G, gerekirse P2-T; MM-Q05 çapraz |
| BC-Q07 | Geçmiş tarih kısıtları ürün davranışı mı emülatör etkileşimi mi? | Açılış tarihi aydan öncesine çekilemedi (51) takvim/hesap makinesi etkileşimine bağlanmış; tekrarlayan başlangıç geçmişe kurulabildi (141). Wallet'ın geçmiş tarihe izin vermediği karşılaştırması P3'ün açık Wallet sorusudur. Açılış tarihinin aynı formda olması kararı (314) bu kısıt çözülmeden doğrulayıcı olarak kullanılmasın. Sistem saati değiştirilmez | B09 ortak ilke; P2-T; P3 Wallet odağı |
| BC-Q08 | Nakit Akım Ayarı gerçekten varsayılan mı ve ₺0 nakit akışının nedeni mi? | Gözlenen ayar durumu ile kurulum varsayılanını ayır; Ana Hesap/Ortak Cüzdan kullanıcı hesabı, Birikimler/Çek/Cüzdan hazır örnek hesaptır. Widget'ın ₺0 göstermesi için neden hipotez olarak kalsın; hangi rapor ve net varlık hesaplarının etkilendiğini ayrı ölç. Bizde bakiye/rapor hesap türünden otomatik türer ifadesini kodla doğrula | B10 kontrol ilkesi; MM-Q06 çapraz; P2-G/T; kod |
| BC-Q09 | Dışa aktarma ve filtre iddialarının kanıtı nedir; Premium sınırı nerede? | Satır 272–273 tek dokunuşla üç format diyor; 13/26 Premium yükseltme ve 353 son adımın doğrulanamadığını söylüyor. B16'ya göre f7-31/f7-32/f7-53 aynı filtre panelidir; export seçenekleri için doğru kare bulunmalı veya iddia seçenek görünürlüğüyle sınırlanmalı. En gelişmiş filtre hükmü beş uygulamanın aynı ölçütle karşılaştırmasına dayanmıyor. CSV/QIF içe aktarma yalnız seçenek adıdır | B16 / P2-G; Belge 1/2 export |
| BC-Q10 | Faz 7 iddiaları ve kısa atıflar hangi kareye bağlanıyor? | Faz 7 bölümünde satır içi kare atfı yok; 55 f7 karesi yalnız f7-00–f7-54 aralığıyla anılıyor ve tap-icon, tap-retry, check, nav-check gibi süreç kareleri var. Kısa kodlar ve 10–28, 29–34 aralık atıfları tam ada bağlanmalı; 13, 14 ve 18 numaralı kareler (E0029, E0030, E0034) metinde hiç anılmıyor. 13 Eylül denetiminde 80 tam ad eksiği ve 5 sorun raporlandı; bu paket düzeltme yapmaz, rolleri P2-G belirler | B17 / P1-K, P2-G, P2-K |
| BC-Q11 | Arayüz taraması tablosunun gizli kanıt sütunu nasıl kapanacak? | Satır 56–65'te başlık ve ayraç üç sütun, sekiz veri satırı dört hücre; dördüncü hücredeki kanıt adları görünmeyebilir. Hizalama ve mekanik kontrol P1-B18 işidir; bu paket tabloyu düzeltmez | B18 / P1-B18 |
| BC-Q12 | Formdaki BF mevcut yetenek kıyasları kodla uyuşuyor mu? | Sıfır tutarın istemci+sunucuda reddi (319; B02 Money bulgusu), düzeltme/iptal modeli (320), taslak uyarısının bizde gerekmesi (334), bölmenin bizde olmaması (333), CSV export (342), yalnız TRY (287, 340), ilk taksitin açık realize istemesi (128, 327), realize isteğinin tutar/tarih taşıması (325), borç/vergi dahil tek planlanan görünüm (336), attentionCode karşılığı (166, 329) ve varsayılan kategori/hesap önerisini (339) kod/ADR ile kontrol et. Mevcut özelliği öneri, olmayanı mevcut gibi yazma | B02 ortak ilke; Belge 3 mevcut durum; kod/ADR |
| BC-Q13 | Karar etiketleri ve dil README sözlüğüne uyuyor mu? | Doğrudan al / mevcut yaklaşımı doğrular (314), Alma / dikkat (317) ve Not (331) tanımlı sonuç değil. Alma satırlarından 317–320, 322, 335, 338 ve 340 kazandırdığı/kaybettirdiğini yazmıyor. Doğru olan bu (324), birebir ve tüm rakiplerin en yakını (155, 323, 352, 358) ifadeleri hüküm kalıbıdır. Güven bölümündeki eski Yorum etiketi Çıkarım tanımıyla hizalanmalı. Kararlar Belge 3 onayı değildir | B19; README sözlüğü; Belge 3 adayları |
| BC-Q14 | Kararsızlık ve dokunma sorunları ürün mü ortam mı? | Klasör diyaloğu kilidi, hesap makinesi donması, ataç hit-alanı ve modal diyalogların dokunuşu işlememesi adb/emülatör koşuluyla birlikte kaydedilmiş. Gerçek cihazda sorun olmaz ifadesi denenmemiş varsayımdır; ürün kararsızlığı hükmü tekrar ve ortam bilgisiyle sınırlanmalı. OCR pazarlanmıyor iddiası kaynak taraması olmadan kesinleşmez | Belge 1 anlatım sınırı; P2-T erişim kontrolü; kaynak |
| BC-Q15 | Hesap, etiket, kategori ve ayar gözlemleri hangi sınırda? | Hazır ₺0 örnek hesapların net varlığa etkisi yok beyanı, İş/Kişisel etiketlerinin hazır mı kullanıcı eklemesi mi olduğu, iki seviyeli kategori, K03'teki Diğer ile ayarlardaki Others adı, çoklu para biriminin ayar adlarından çıkarımı ve Seyahat Modu davranışı ayrı kaydedilsin. Takvimin K görevlerinde hiç görülmediği ifadesi Tur 1 ana ekran Takvim kartıyla (61) uzlaşsın | Belge 1/2 yüzey; P2-G |
| BC-Q16 | P2 başlangıcında cihaz verisi formdaki son durumla aynı mı? | Formda D1–D3 sonrası bütün hesapların ve net varlığın tablosu yok; B1 serisinin sonraki vadeleri ve taksit hatırlatıcıları 14 Eylül itibarıyla yeni gecikmeli kalemler üretmiş olabilir. P2'de önce salt okunur başlangıç kaydı alınır; hatırlatıcı onayı, yeni kayıt, silme veya sıfırlama kendiliğinden yapılmaz | P2 başlangıç kaydı; Bölüm 13 kesinti kuralı |

### Bu paket sonrası kapsam

- Okundu: E0005 tam metin; BC-M01–BC-M18 ve BC-Q01–BC-Q16 kaydedildi.
- Yeni görsel incelemesi **0/90**, canlı test **0**, kapanan B bulgusu **0**.
- B01/B10/B16/B17/B18 doğrudan bağlandı; B19 karar/dil yayılımıdır.
  B02/B09 bağlantıları ortak kontrol ilkesidir, yeni hata doğrulaması değildir.
  Plandaki Bluecoins odakları (aritmetik, B1/B2 modları, silme/çöp kutusu,
  menü/export görselleri) BC-Q02/Q03/Q04/Q09'da.
- Kaynak form ve görseller değişmedi; emülatör açılmadı, veri/ayar değişmedi.
  Bu harita P2 tam Faz 7.5 koşumunu başlatmaz veya yerine geçmez.
- Yarım bölüm yok. Sıradaki tek paket **P0.3-wallet (E0014)**.
  P0.4, G/T ve P2/P3 paketleri başlamadı; Faz 8 açılmadı.


## P0.3-wallet — Tam metin içerik haritası

**14 Eylül 2026: E0014'ün 380 satırının tamamı okundu.** Kaynak:
[Wallet formu](gozlemler/wallet-budgetbakers.md). Okunan SHA-256:
`e424d5e4fff1c71b35e07a1e182fba25c80a3e5946af63bc1e3e6b6e4bbefb9b`.
Kaynak form değişmedi. Bu paket **metin kapsam haritasıdır**; P3 tam Faz 7.5
koşumunun yerine geçmez, B09 model eşdeğerliğini doğrulamaz veya kapatmaz.

### Kaynak bağlamı ve sınır

- Form Wallet: Budget Expense Tracker / BudgetBakers 9.3.6 (versionCode
  90164), emulator-5554 1080x2400, İngilizce arayüz/TRY ve ücretsiz bulut
  hesabı bildiriyor. Test sırasında mağazada daha yeni sürüm göründüğü
  yazıyor; bu tur uygulama açılmadı, sürüm ve bulut verisi kontrol edilmedi.
- Koşum katmanları: 1 Eylül Tur 1 (eski PC), 10 Eylül Faz 2 (yeni emülatör,
  bulut geri yükleme) ve aynı gün borç/plan ek koşumu, 11 Eylül Faz 7 derin
  koşum. Oturum tablosundaki test tarihi satırı 11 Eylül'ü saymıyor. İşlem
  dönemi çekirdekte Ağustos 2026; B2 kaydı Eylül tarihine yazılmış. Formda
  **Faz 7.5 bölümü yok**; DURUM bu doğrulamanın tamamlanmadığını kaydediyor.
- Kanıt havuzu **E0274–E0379: 106 PNG**. Form atıflarına göre 00/02b/03–09
  (E0274–E0282) Tur 1, 10–40 Faz 2, 41–47 ek koşum (E0283–E0320),
  f7-00–f7-58 (E0321–E0379) Faz 7. E0273 yalnız .gitkeep dosyasıdır.
  01/02 numaralı dosya yok; bu kayıp dosya ilanı değildir. Tam yollar
  [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md)'de. Bu pakette 0/106 görsel açıldı.
- 13, 30, 34 ve 37 numaralı kareler metinde doğrudan anılmıyor, yalnız
  aralıkta kalıyor. Faz 7 bölümünde iddia başına kare atfı yok; f7 kareleri
  yalnız güven bölümündeki aralığın iki ucuyla geçiyor. Aşağıdaki bu
  eşlemeler **dosya adı adayıdır**, içerik eşlemesi değildir (WL-Q11).
- Kanıt klasörünün `kanitlar/wallet/` → `kanitlar/wallet-budgetbakers/`
  yeniden adlandırması Git'te staged ve commit edilmemiş kullanıcı
  değişikliğidir; form ve envanter yeni yolu kullanıyor. Bu paket taşıma yapmadı.
- Formda HTTP(S) URL'si ve resmî kaynak yok (`Resmî kaynak: —`). Premium
  özellikler, banka bağlantısı, dışa aktarma/yedek, widget ve otomatik
  tekrarlama kolu doğrulanmamış; yokluk hükmüne çevrilmez.
- Veri koruma: sentetik kayıtlar kullanıcının bulut hesabında; hesap adı ve
  kişisel bilgiler haritaya kopyalanmadı. Planlı ödemeler ve borçlar tarihe
  bağlı olduğundan P3'te salt okunur başlangıç kaydı olmadan onay, Record,
  silme veya e-posta tetikleyen işlem yapılmaz.

### Başlık → iddia → kanıt → rapor haritası

Satır konumları okunan sürüme aittir. Belge 3 bağlantıları onay değil, ileride
onaylı Belge 1/2'den beslenecek değerlendirme konularıdır.

| Kimlik | Kaynak bölüm / başlangıç satırı | Önemli içerik ve iddialar | Kanıt eşlemesi | Hedef / kontrol bağı |
|---|---|---|---|---|
| WL-M01 | Oturum bilgisi 3 | Ürün/sürüm ve güncelleme bilgisi; test tarihleri; İngilizce arayüz; önceden açılmış bulut hesabı ve Faz 2 geri yükleme; Premium kilitleri; rehber izni reddinde borç formunun kapanması | E0014; E0274, E0275 | Belge 1 profil/yöntem; WL-Q01/Q15/Q16 |
| WL-M02 | Görev gözlemleri 15, K00–K04 | Kayıt/onboarding görülmedi; yatay hesap kartları ve tanıtım blokları; geniş manuel hesap türleri, cash/checking açılış bakiyesi yokluğu ve checking e-postası; varsayılan Expense ve Sale kategorisi; kapsam alanı yok | E0274–E0278 | Belge 1 giriş/form; Belge 2 kapsam; WL-Q01/Q07 |
| WL-M03 | Görev gözlemleri K05–K08 | Kart gideri anında kart bakiyesi ve harcama raporu; ayrı Transfer, iki satır ve nötr toplam; haftalık liste, arama, Cash-flow 25.000/−2.050/22.950 ve Spending; yerinde düzenleme, Split ve kalıcı silme | E0278–E0280; K08 için kare yok | Belge 2 kart/transfer/düzeltme; WL-Q10/Q11/Q13 |
| WL-M04 | Kontrol değeri 29; Faz 2 kontrolü 44 | Açılış bakiyesi olmadığı için işlem-only kontrol; 20.800/2.150/0/22.950; Faz 2'de bulutta korunan beş işlem ve B1/B2 sonrası 16.350 net | E0283–E0285 | Belge 2 çekirdek sonuç; B18, WL-Q04/Q07/Q12 |
| WL-M05 | Arayüz taraması 50 | Geniş hamburger menü ve Statistics sekmeleri; rapor drill-down kısmen; planlama boş durumları; ayarlara girilmemesi; not araması ve gelişmiş filtre görülmemesi; sıfır tutar toast'u ve silme; widget denenmedi | E0275, E0278, E0280–E0282; tablo 3 sütun başlık/4 hücre veri | Belge 1 yüzey; B18, WL-Q10/Q12/Q15 |
| WL-M06 | Arayüz incelemesi 63 | Soru başlıklı raporlar; alt bar yokluğu; hesap renginin form zeminine yayılması ve renkle tür seçimi riski; para biçimi; hızlı/ayrıntı formu; boş/hata/başarı geri bildirimi | E0014 metin; satır başına kare atfı yok | Belge 1 desenler; WL-Q14/Q15 |
| WL-M07 | Akış özeti 76 | Hızlı giriş; işletme gelirinde sürtünme; hane finansı ve Premium banka hedefi; kapsam yok; nötr transfer ve kart ödemesi; planlama/borç/hedef/bütçe ayrı yüzeyleri | E0014 sentez; ilgili WL-M kayıtları | Belge 1/2 özet; WL-Q14 |
| WL-M08 | Faz 2 koşumu 85; çekirdek 90; kart modeli 100; kart ödemesi 113 | Bulutta doğru tarihli beş işlem ve MM tarih sapmasının olmaması; kart negatif bakiye, ekstre/kesim yok; limit, Available Credit/Balance, Payment Due Date, bakiye ayarlama kalemi ve minimum bakiye uyarısı; ayrı kart ödeme akışı yok, kısmi ödeme küçük transfer | E0283–E0285, E0297–E0298 | Belge 2 kart; WL-Q06/Q07 |
| WL-M09 | Tekrarlayan B1 121 | Planned payments formu; önceki aya gidemeyen tarih seçici ve bugünkü başlangıç; tanımın para üretmemesi; Due today, Confirm/Postpone/Dismiss; düzenlenebilir Payment summary ve 20.800→20.200; plan bazlı otomatik/onaylı sorusunda No; yalnız sonraki örneğin görünmesi; tekrar rozeti yok | E0287–E0293, E0295, E0306; E0286 ad adayı | Belge 2 planlama; WL-Q05/Q08/Q13 |
| WL-M10 | Taksit B2 154 | Taksit alanı ve plan kavramı yok; Payment Type seçenekleri; ₺6.000'ın tek kart borcu ve kayıt tarihindeki (Eylül) tek gider olması; MM ve InstallmentPlan kıyası | E0294, E0296–E0297 | Belge 2 taksit; WL-Q05/Q13 |
| WL-M11 | Bütçe 166; hedef 181 | Kategori+hesap+etiket filtreli aylık bütçe; kart harcamasının tam tutarla aşım göstermesi (6.600/5.000); Forecasted Spend, günlük ortalama, dönem kıyası, Premium drill-down; hedef formu ve hesaba bağlanmayan ilerleme | E0299–E0302, E0304–E0305; E0303 ad adayı | Belge 1/2 bütçe/hedef; WL-Q08/Q13 |
| WL-M12 | Borç (Debts) 189 | I Lent/I Borrowed; mevcut kayda bağlama sorusu; zorunlu hesap ve vade; Record oluşturma sorusu ve bakiye değişimi seçimi; Yes ile Ada Reklam ₺5.000 Loan, interests gideri ve 15.200; açık Total ve Add Record; ADR 0014 ayrımının olmadığı hükmü; rehber izni tuzağı | E0308–E0309, E0314–E0317; E0307 ad adayı | Belge 2 borç; B09, WL-Q02/Q03/Q15 |
| WL-M13 | Plan yönetimi 213; split 223; fiş 230; Faz 2 sonrası veri 237 | Otomatik/onaylı kolunun dişliyle değişmesi; gerçekleşmiş örnek uyarısı olmadan plan silme; split ile alt kayıt oyma; yalnız dosya/fotoğraf eki, OCR yok; 15.200/2.150/−6.000 ve 11.350; A testinin yapılmadığı ve veri korunarak Faz 7'ye geçiş | E0318–E0320, E0311–E0313; E0310 ad adayı | Belge 2 plan/düzeltme/fiş; WL-Q04/Q08/Q10/Q16 |
| WL-M14 | Faz 7 A tamamlama 253; D1 258 | ₺400 kart ödemesi jenerik transfer (14.800, −5.600) ve Bluecoins/MM ile aynı model hükmü; One-Time planlı ödeme, 1–10 Eylül gri tarih ve zorunlu kategori; Confirm ile gerçekleşme ve 4.800 | Ad adayları: A E0321–E0334, D1 E0335–E0357 | Belge 2 kart/planlama; WL-Q04/Q05/Q06/Q11 |
| WL-M15 | Faz 7 D2 267; D3 278 | Hizmet faturası I Lent olarak ve No ile Record'suz; bakiye değişmedi, işlem geçmişinde yok; ADR 0014'ü native destekleyen tek rakip hükmü; aynı Debt'e Repay debt Record'u ile ₺5.000 kısmi tahsilat, borç 7.000 ve hesap 9.800; CounterpartyCharge/Payment'a en yakın model hükmü | Ad adayları: D2 E0358–E0366, D3 E0367–E0371 | Belge 2 borç/tahsilat; B01/B09, WL-Q02/Q03/Q04/Q13 |
| WL-M16 | Arama 289; tam arayüz taraması 297 | Ada aramasında üç kayıt ve Record'suz D2'nin çıkmaması, tabloya yazılmama hükmü; filtre/export menüsünün bulunamaması; Shopping lists/Warranties/Loyalty/Currency/Group sharing; Filters, Automatic rules ve transfer tanıma, Currencies, muhasebe dönemi başlangıç günü, Templates | Ad adayları E0372–E0379 | Belge 1 ayarlar; Belge 2 arama/otomasyon; B12, WL-Q09/Q10/Q13 |
| WL-M17 | BusinessFinance için kararlar 321 | 29 satır: soru başlıklı rapor, cash-flow bloğu, boş durum, iki katmanlı form uyarlama/al; renkle tür, geniş menü, kapsam yokluğu, kalıcı silme, açılış bakiyesi yokluğu, geçmiş tarih, taksit yokluğu, dönemsiz kart, Loan gideri, uyarısız silme alma; Confirm modeli, otomatik/onaylı tercih ve Debt/Record ile kısmi tahsilat doğrudan/uyarlayarak al; Forecast, split, alarm, transfer kuralı ve dönem başı bekletme | E0014 karar tablosu; ilgili WL-M kayıtları | Yalnız Belge 3 adayları; B01/B09/B19, WL-Q05/Q13/Q14 |
| WL-M18 | Kanıt ve güven 355; sonuç 372 | Tur 1/Faz 2/ek koşum/Faz 7 koşum özetleri ve aralıkları; resmî kaynak yok; eski Yorum etiketiyle UX ve TR pazarı hükümleri; doğrulanamayanlar; ADR 0014'ü native destekleyen tek rakip sonucu | E0014; E0274–E0379 kapsamı | Belge 1/2 yöntem; B09/B17/B19, WL-Q01/Q02/Q11/Q14 |

### Açık kontrol soruları — yeni doğrulanmış hata değildir

**WL-Q01–WL-Q16'nın tamamı açık.** B09 WL-Q02/Q03'e, B12 WL-Q09'a, B17
WL-Q11'e, B18 WL-Q12'ye, B01 WL-Q13'e, B19 WL-Q14'e bağlandı; bu paket
bunları kapatmaz. B01–B19 yeniden numaralandırılmadı. P3 tam koşumu bu
soruları görsel ve gerekirse tek soruluk canlı test paketlerine dönüştürür.

| Kimlik | Soru / neden | Doğrulama ve kapanış | Bağ / hedef iş |
|---|---|---|---|
| WL-Q01 | Hangi gözlem hangi koşuma, sürüme ve hesap durumuna ait? | Tur 1 (eski PC), Faz 2 bulut geri yükleme, 10 Eylül ek koşum ve 11 Eylül Faz 7 ayrı tutulsun; test tarihi satırı Faz 7'yi saymıyor. Mağazada görünen güncelleme nedeniyle 9.3.6 gözlemleri güncel sürüme genellenmesin. Kare gruplarını koşum tarihiyle eşle; kişisel hesap bilgisini kopyalamadan sürüm ve oturum erişimini salt okunur doğrula | B17/B19; P3-M/P3-G |
| WL-Q02 | Debt/Record mekanizması ADR 0014 tanıma/taşıma modeline eşdeğer mi? | Satır 206–208 ayrımın olmadığını, 274–276, 349 ve 376–377 ayrımı native destekleyen tek rakip olduğunu söylüyor. D2 No Record ile hiçbir gelir tanınmadı mı; D3'ün Lending, renting Record'u gelir sayılıyor mu; ilk borcun Loan, interests gideri raporu nasıl etkiliyor, ölçülmedi. I Lent borç verme türüyle hizmet faturası alacağının anlam farkı ayrı yazılsın. B09 ölçütüyle D2 öncesi/sonrası ve D3 sonrası hesap, açık borç ve gelir/gider raporu ayrı kaydedilsin; Debt–Record bağı gözlemi model eşdeğerliğinden ayrılsın | B09 / P3-T, P3-K; Belge 2/3 |
| WL-Q03 | Aynı adlı iki Ada Reklam borcu var mı; D3 hangi Debt'e bağlandı? | 10 Eylül ek koşumda I Lent Ada Reklam ₺5.000 (Record'lu), Faz 7'de aynı adla ₺12.000 (Record'suz) oluşturulmuş. D3'ün hangi kartta 12.000→7.000 düşürdüğünü, Debts listesinde kaç açık kayıt ve toplam olduğunu ve arama sonucundaki üç kaydın hangi Debt'e ait olduğunu kareyle eşle. Yeni borç/Record eklemeden salt okunur kontrol et | B09; P3-G, gerekirse P3-T |
| WL-Q04 | Hesap ve net bakiye zinciri tam ölçülmüş mü? | Metindeki sıra 20.800 → 20.200 (B1) → 15.200 (borç Record'u) → 14.800 (A) → 4.800 (D1) → 9.800 (D3); net 22.950 → 16.350 → 11.350. Aritmetik olarak tutarlı görünüyor ancak Faz 7 sonrası Ortak Cüzdan, İş Kartı ve net varlık tablosu yok; D2'nin net varlığa hiç etki etmediği ekranla ölçülmemiş. İlgili karelerle doğrula; eksik ölçümü tahminle doldurma | P3-G/P3-K; B09 dolaylı |
| WL-Q05 | Geçmiş tarih kısıtı ürün davranışı mı, ayar/kademe/koşum koşulu mu? | B1'de önceki aya gidilemediği ve bu sürümde/kademede ifadesi (130–135), D1'de ise ay içindeki geçmiş günlerin gri olduğu (259–261) yazıyor; ay ve gün düzeyi kısıt ayrı tarif edilmiş. B2'nin Ağustos yerine Eylül'e yazılması (161) aynı kısıttan mı ayrılsın. Kararlar 337 Alma ile 351 Henüz karar verme aynı gözleme farklı sonuç veriyor; bizde geçmiş tarihli plan kurulabildiği (351) kodla doğrulansın. Sistem saati değiştirilmez | Plan P3 odağı; P3-T; kod; BC-Q07 çapraz |
| WL-Q06 | Kart modelinde son ödeme tarihi ve ödeme akışı iddiaları tutarlı mı? | Satır 104 hesap kesim ve son ödeme tarihi yok, 106 Payment Due Date alanı var, K02 (21) kartta son ödeme tarihi alanı var diyor. Available Credit gösterimi, bakiye düzenleme kalemi ve minimum bakiye uyarısının hangi karede görüldüğü belirtilmemiş. A tamamlamasındaki Bluecoins/MM ile aynı model hükmü (253), MM'in ön doldurulmuş ödeme ve ekstre kuralı farkıyla (118–119) uzlaşsın | Belge 2 kart; P3-G; MM-Q05/BC-Q06 çapraz |
| WL-Q07 | Açılış bakiyesi gerçekten verilemiyor mu? | Cash/checking açılış alanı yokluğu (21, 31–33) ile hesap detayındaki bakiye ayarlama kalemi (108) karşılaştırılsın; ayarlamanın gelir/gider raporuna etkisi bilinmiyor. 334 Alma gerekçesindeki sahte gelir riski bu alternatif denenmeden kesinleşmez. Checking hesabı için gönderildiği yazılan e-posta dış etki taşır; yeniden tetiklenmez | Belge 2 açılış; P3-G, gerekirse izinli P3-T; B11 ortak ilke |
| WL-Q08 | Tekrarlayan ve bütçe gözlemleri hangi moda ve örneğe sınırlı? | B1 yalnız No/onaylı kolda koşuldu; Yes/otomatik kolunun üretimi doğrulanamadı (370). Yalnız sonraki örneğin görünmesi ve rozet yokluğu bu moda ait olabilir. Postpone/Dismiss'in BF realize/skip ile birebir eşlemesi (141, 335) ve bizde varsayılan onaylı ifadesi (336) kodla kontrol edilsin. Bütçenin kart harcamasını harcandığı ayda sayması tek örnektir; hedef-hesap bağı denenmedi | B10 ortak ilke; P3-T; kod |
| WL-Q09 | Record'suz borcun işlem tablosuna hiç yazılmadığı sonucu nasıl sınırlanmalı? | Arama sonucunda görünmeme ve yalnız Debts ekranında görünme gözlemdir; fiziksel tablo yokluğu kanıtı değildir (293). Statistics/Cash-flow gibi raporlarda görünüp görünmediği ayrıca ölçülmedi. İfade ölçülebilir dış davranışla yeniden yazılacak | B12 / P1-B12, P3-K |
| WL-Q10 | Filtre, dışa aktarma, otomatik kural ve OCR iddialarının sınırı nerede? | Tur 1 gelişmiş filtre görülmedi (26, 58) ile Faz 7 Settings→Filters (303) katmanlarını ayır; Records üç nokta menüsü bulunamadı ve dışa aktarma/yedek doğrulanamadı. Automatic Rule Premium kilidi (13) ile ayarlarda görünen transfer tanıma arasındaki erişim farkı ve hiçbir rakipte görülmemiş hükmünün (308) karşılaştırma dayanağı açık. OCR yok ifadesi ücretsiz akışla sınırlı (234); AI receipt Premium doğrulanamadı | Belge 1/2 özellik; P3-G; kaynak |
| WL-Q11 | Faz 7 iddiaları ve kısa atıflar hangi kareye bağlanıyor? | Faz 7 bölümünde satır içi kare atfı yok; 59 f7 karesi yalnız f7-00–f7-58 aralığıyla anılıyor ve back-check, cleared, nav-check gibi süreç kareleri var. 10–40 ve 41–47 aralıkları ile kısa kodlar tam ada bağlanmalı; 13/30/34/37 metinde anılmıyor; K08 kanıtı kare değil. 13 Eylül denetimi 97 tam ad eksiği ve 6 sorun raporladı. Bu paket düzeltme yapmaz | B17 / P1-K, P3-G, P3-K |
| WL-Q12 | Arayüz taraması tablosunun gizli kanıt sütunu nasıl kapanacak? | Satır 52–61'de başlık ve ayraç üç sütun, sekiz veri satırı dört hücre; kanıt adları görünmeyebilir. Hizalama ve mekanik kontrol P1-B18 işidir; bu paket tabloyu düzeltmez | B18 / P1-B18 |
| WL-Q13 | Formdaki BF kıyasları kod ve ADR ile uyuşuyor mu? | Kısmi tahsilatın aynı borca bağlanmasını CounterpartyCharge/Payment ve CounterpartyBalance ile eşleyen ifadeler (285–287, 340, 349, 350) B01'e göre sınırlanmalı: bizde ödeme belirli borçlandırmaya bağlanmaz. Muhasebe dönemi başlangıç gününün tam bizim kavramımızın karşılığı olduğu (313) ile MonthlyBudget'ta sabit ay başı (353) çelişiyor. Ayrıca 409 has_realized_history (342), CounterpartyCharge kategori/yön (341), InstallmentPlan (338), kapsam ekseni (344), split yokluğu (347) ve Transfer tipi (352) kodla kontrol edilsin | B01 / P1-B01; Belge 3 mevcut durum; kod/ADR |
| WL-Q14 | Karar etiketleri ve dil README sözlüğüne uyuyor mu? | Doğrudan al / zaten kısmen var (326) ve Not (346) tanımlı sonuç değil. Alma satırlarından 329–330, 332–334, 337–339 ve 341–342 kazandırdığı/kaybettirdiğini yazmıyor veya yalnız bizim kuralı veriyor. 349/350 Doğrudan al diyor ama gerekçe aynı işi bizde başka yapıyla yaptığımızı söylüyor. Birebir (335), mimari kuzen (172), tek rakip (275, 377) ve TR pazarı için belirgin eksik (360) ifadeleri ölçüm değildir; eski Yorum etiketi Çıkarım'la hizalanmalı | B19; README sözlüğü; Belge 3 adayları |
| WL-Q15 | İzin, renk ve menü gözlemleri ürün davranışı mı ortam/yorum mu? | Rehber izni reddinde uygulama içi Cancel ile formun kapanması emülatör tuzağı olarak yazılmış; gerçek cihaz davranışı bilinmiyor. Renkle seçili tür nedeniyle yanlış seçim riski, menü yükü ve boş durumların MM'den yönlendirici olması kullanılabilirlik ölçümü değildir. Sıfır tutar toast'u tek koşum gözlemidir | Belge 1 anlatım sınırı; P3-G |
| WL-Q16 | P3 başlangıcında bulut verisi formdaki son durumla aynı mı? | Formda Faz 7 sonrası tam hesap tablosu yok; B1 serisinin 10.10.2026 vadesi, D2/D3 açık ₺7.000 borcu, bütçe ve hedef bulut hesabında duruyor olabilir ve başka cihazdan değişmiş olabilir. P3'te önce oturum erişimi ve salt okunur başlangıç kaydı alınır; Confirm, Record, silme veya sıfırlama kendiliğinden yapılmaz | P3 başlangıç kaydı; Bölüm 13 kesinti kuralı |

### Bu paket sonrası kapsam

- Okundu: E0014 tam metin; WL-M01–WL-M18 ve WL-Q01–WL-Q16 kaydedildi.
- Yeni görsel incelemesi **0/106**, canlı test **0**, kapanan B bulgusu **0**.
- B01/B09/B12/B17/B18/B19 doğrudan bağlandı. B10/B11 bağlantıları ortak
  kontrol ilkesidir, yeni hata doğrulaması değildir. Plandaki Wallet odakları
  (D2/D3 etkileri, No Record sonucu, geçmiş tarih kısıtı, çelişen eski
  sonuçlar) WL-Q02–Q05'te.
- Kaynak form ve görseller değişmedi; emülatör ve bulut hesabı açılmadı,
  veri/ayar değişmedi. Bu harita P3 tam Faz 7.5 koşumunu başlatmaz.
- **Dokuz uygulamanın P0.3 metin haritası tamamlandı.** Yarım bölüm yok.
  Sıradaki tek paket **P0.4 — paket listesi ve P0 kabul kontrolü**.
  G/T, P1–P3 paketleri başlamadı; Faz 8 açılmadı.


## P0.4 — P0 kabul kontrolü ve paket listesi

**14 Eylül 2026: P0.1 envanteri, P0.2 B kayıtları ve dokuz P0.3 haritasının
tamamı birlikte yeniden kontrol edildi; önceki oturumlarda kapanan beş harita
(Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı) dahil.**
Kalan inceleme işi tam E kimliği aralıklarıyla paketlere bölündü. Bu paket
görsel açmadı, canlı test yapmadı, kaynak form veya görsel değiştirmedi ve
B bulgusu kapatmadı. Kontroller bu oturumda çalıştırılan geçici betik ve
taramalardır; betik araştırma klasörüne eklenmedi ve B17 mekanik kapısının
yerine geçmez.

### Kabul kontrolü sonuçları

| Kontrol | Yöntem | Sonuç |
|---|---|---|
| K1 Envanter ve disk | E0001–E0392 satırlarını ayrıştır; kimlik sırası, metin/bağlantı yolu, diskte varlık ve envanter dışı dosya | 392 kimlik sıralı ve benzersiz; eksik 0, fazla 0; yol uyuşmazlığı 0 |
| K2 PNG bütünlüğü ve uygulama aralıkları | 357 PNG'nin boyut ve SHA-256'sını P0.1 kaydıyla karşılaştır; klasör başına kimlik aralığı | 357/357 eşleşti; dokuz aralık kesintisiz ve envanterdeki uygulama sayılarıyla aynı |
| K3 Metin dosyalarında hash farkı | P0.1 anlık hash'i ile bugünkü hash | Yalnız E0003, E0004, E0383, E0387, E0389 farklı: P0 devir güncellemeleri. E0391/E0392 bilerek hash tutmaz. Dokuz gözlem formu P0.1 hash'iyle aynı |
| K4 P0.2 B kayıtları | B başlıklarını kaynak rapordaki B başlıklarıyla eşle; on zorunlu alan ve açık durum | 19/19 eşleşti; alanlar tam; 19'u da açık |
| K5 Harita kaynak kimliği | Beyan edilen satır sayısı ve SHA-256 ile bugünkü form ve envanter hash'i | 9/9 eşleşti |
| K6 Harita kimlikleri ve sayılar | M/Q sürekliliği; paket tablosu, bölüm kapanışı, envanter ayrıntısı ve DURUM'daki aralıkların aynılığı; tablo sütunları | 9/9 kesintisiz; dört yerdeki aralıklar aynı; sütun uyumsuzluğu 0; toplam 151 harita satırı, 117 soru |
| K7 Kanıt kimliği kapsamı | M/Q tablolarındaki E kimliklerinin kendi uygulama aralığında kalması; havuz aralığı ve PNG sayısının bölümde yazması | 9/9 havuz ve sayı yazılı. Aralık dışı üç atıf bilinçli: PS-Q11 E0257 ve QB-Q12 E0267 (.gitkeep'i görsel saymama uyarısı), QB-Q01 E0272 (kanıt girişi, B19) |
| K8 Çapraz atıf | Tam ve kısa Q atıflarının tanımlı kimliğe gitmesi; B kimliklerinin B01–B19 içinde kalması | Çözülmeyen atıf 0; geçersiz B kimliği 0 |
| K9 Harita satır atıfları | M tablosundaki başlangıç satırlarının formda başlık veya kalın paragrafa denk gelmesi; çıktı tek tek okundu | Dokuz haritada doğru; bir kesinsizlik düzeltildi (PS-M08) |
| K10 Soru satır atıfları | QB/KB/BC/WL soru satırlarındaki satır numaralarının form metni; çıktı tek tek okundu. Önceki beş haritanın soru satırlarında satır numarası yok | Bir hata düzeltildi (BC-Q02) |
| K11 Anılmayan kare beyanları | Kısa kare kodlarını form metninde tara | Wallet beyanı (13/30/34/37) doğru; Bluecoins beyanı eksikti, düzeltildi (BC-M11, BC-Q10) |
| K12 Numarasız devir notları | Paraşüt, Logo ve MM/Wallet/Bluecoins atıf notlarının soruya bağlanması | PS-Q03/Q04, LI-Q05; atıf borcu MM-Q10, BC-Q10/Q11, WL-Q11/Q12 |
| K13 Takip belgeleri | DURUM, plan, README, TUR2, raporlar/README, envanter ve bulgu kaydında sıradaki paket ifadeleri | P0.4 öncesi hepsi tutarlıydı; bu paketle P1-B01'e güncellendi. Paket bölümlerindeki eski sıradaki paket satırları tarihsel kapanış kaydıdır |

### Düzeltme kaydı

| Yer | Önce | Sonra | Dayanak |
|---|---|---|---|
| PS-M08 (önceki oturum) | Pipeline şemaları 111/116 | Pipeline şemaları 109/115 | E0011'de şema başlıkları 109 ve 115. satırda; 111/116 şema metninin içi |
| BC-Q02 (bu oturum) | Satır 48'deki Net Kazanç | Satır 46–47'deki Net Kazanç | E0005'te ifade 46–47. satırda; 48 açıklama cümlesi |
| BC-M11, BC-Q10 (bu oturum) | Metinde anılmayan kare yalnız E0034 | 13, 14 ve 18 numaralı kareler (E0029, E0030, E0034) | E0005 kısa kod taraması; BC-M09'daki E0034 notu doğru |

Düzeltmeler yalnız bu kayıttadır; kaynak formlar değişmedi. Sonucu veya paket
kapsamını değiştiren başka hata bulunmadı.

### P0 kabul kararı

**P0 kabul kapısı geçti; P0 kapandı.** Planın P0 ölçütü "malzeme ve B01–B19
izlenebilir": dokuz formun içeriği, 357 PNG'nin uygulama aralığı ve 19 açık
bulgu kalıcı kimliklerle izlenebilir. Kapanışın sınırları:

- Görsel içerik incelemesi 0/357; envanterin görsel başına ayrıntı alanları
  (gerçek ekran, sürüm, kaynak, tarih, rol) atanmadı. Bu G paketlerinin işidir.
- B01–B19 ve 117 açık soru kapanmadı; P0 kapanışı hiçbir iddiayı doğrulamaz.
- Bluecoins ve Wallet Faz 7.5 yapılmadı; P2/P3 kapsamıdır.
- Faz 8 geçiş kapısının hiçbir maddesi bu kapanışla işaretlenmez.

### Görsel paketleri — 17 paket, 357 PNG

Bölme ölçütü: aynı uygulama ve akış; az görselli uygulama tek paket; yoğun
uygulamada küçük paket. Sınırlar form atıflarına ve dosya adlarına dayanır;
içerik incelemesinde akış farklı çıkarsa paket bölünür, E kimliği değişmez.
Soru bağları başlangıç atamasıdır. 13 Eylül denetiminde açılmış 11 görsel
(E0082, E0083, E0104, E0130, E0178, E0184, E0186, E0188, E0194, E0206, E0271)
içerik onayı sayılmaz; kendi paketinde değerlendirilir.

| Paket | E kimlikleri | Adet | İlk – son dosya | Akış kapsamı | Başlangıç soru/bulgu bağı |
|---|---|---:|---|---|---|
| P1-money-manager-G01 | E0227–E0256 | 30 | 02-bos-ana-ekran – 32-kart-defteri-eylul-taksit-2-6 | Tur 1 çekirdek; Faz 1 kart/tekrar/taksit/açılış; Faz 7.5 toplam, filtre, kart defteri | MM-Q01–MM-Q10; B16/B17/B18 |
| P1-hesap-defterim-G01 | E0134–E0154 | 21 | 00-magaza – 19-transfer-bacagi-desync | Tur 1 K00–K08 (06b dahil); ek koşum A/B/B1/B2 | HD-Q01–HD-Q04, HD-Q10, HD-Q11; B12 |
| P1-hesap-defterim-G02 | E0155–E0178 | 24 | 20-oge-eklemek-dialog – 43-ayarlar-alt-bolum-donem-baslangici | Ek koşum 2 (öğe, ek, doğrulama, arama, not/kupür, silme, bildiri); Faz 7.5 ekleri | HD-Q01, HD-Q02, HD-Q05–HD-Q11; B11/B14 |
| P1-goodbudget-G01 | E0106–E0133 | 28 | 01-ilk-acilis – 26-schedule-frequency-listesi | Kurulum/zarf/hesap; gelir ve transfer; rapor/silme; tekrar/arama/ek; Faz 7.5 ekleri | GB-Q01–GB-Q11; B05/B06/B07 |
| P1-parasut-G01 | E0258–E0266 | 9 | 01b-carousel – 08-video-nakit-akisi-raporu | Mobil giriş öncesi ve tanıtım videosu kareleri; görsel kaynak eşlemesi | PS-Q01–PS-Q03, PS-Q05, PS-Q07, PS-Q10, PS-Q11 |
| P1-logo-isbasi-G01 | E0220–E0225 | 6 | 01-giris-ekrani – 06-video-musavir-portal | Giriş/kayıt yüzeyi ve pazarlama kareleri; görsel kaynak eşlemesi | LI-Q01, LI-Q02, LI-Q04–LI-Q06, LI-Q08, LI-Q09 |
| P1-quickbooks-G01 | E0268–E0271 | 4 | 01-onboarding – 04-choose-plan-paywall | QuickBooks mobil onboarding ve QBO Simple Start plan ekranı | QB-Q01–QB-Q03, QB-Q05, QB-Q12; B13 |
| P1-kolaybi-G01 | E0180–E0197 | 18 | d32-giris-ekrani … d39-guncel-arayuz-2026, d01-destek-proje-listesi – d10-destek-cari-ekstre-onizleme | Mobil giriş; eski video ve 2026 karesi; proje, gider, cari ve ekstre mockup'ları | KB-Q01, KB-Q03, KB-Q05–KB-Q09, KB-Q11, KB-Q14, KB-Q16; B08/B15 |
| P1-kolaybi-G02 | E0198–E0218 | 21 | d11-destek-gider-listesi – d31-destek-urun-varyantlar | Gider listesi, alış, personel/maaş, finans, pano/not, raporlar, satış/iade, varyant; eski/yeni çift karşılaştırmasının ikinci yarısı (G01 kayıtlarına bağlanır) | KB-Q03, KB-Q06–KB-Q09, KB-Q13–KB-Q16; B15 |
| P2-G01 | E0016–E0032 | 17 | 00-magaza – 16-kontrol-degerleri-net-kazanc-44950 | Tur 1; Faz 3 kurulum ve çekirdek kontrol | BC-Q01–BC-Q03, BC-Q07, BC-Q10, BC-Q14, BC-Q15 |
| P2-G02 | E0033–E0050 | 18 | 17-b2-taksit-sartlari-sheet – 34-kismi-kart-odemesi-500-transfer | B2 taksit, B1 tekrar, split, hatırlatıcı, kart alanları, cari, kısmi ödeme | BC-Q02, BC-Q04–BC-Q07, BC-Q10, BC-Q14; B10 |
| P2-G03 | E0051–E0079 | 29 | f7-00-baslangic – f7-28-d3-kaydedildi | Faz 7 başlangıç, D1, D2, D3 | BC-Q02, BC-Q04, BC-Q05, BC-Q10, BC-Q16; B01/B10 |
| P2-G04 | E0080–E0105 | 26 | f7-29-arama – f7-54-print | Arama, filtre/menü, kategori/nakit akım/etiket/çöp, ayarlar, takvim, seyahat, export/print | BC-Q03, BC-Q08–BC-Q10, BC-Q15; B16 |
| P3-G01 | E0274–E0298 | 25 | 00-magaza – 25-kart-hesap-ayarlari | Tur 1; Faz 2 çekirdek; B1 tekrar; B2 taksit; kart modeli | WL-Q01, WL-Q04–WL-Q08, WL-Q10, WL-Q11, WL-Q15 |
| P3-G02 | E0299–E0320 | 22 | 26-butce-olusturma-formu – 47-planned-silme-basit-onay-gecmis-uyarisi-yok | Bütçe, hedef, borç, split, fiş, plan yönetimi | WL-Q02, WL-Q03, WL-Q07, WL-Q08, WL-Q10, WL-Q11, WL-Q15; B09 |
| P3-G03 | E0321–E0357 | 37 | f7-00-baslangic – f7-36-nav-check | Faz 7 A kısmi kart ödemesi ve D1 planlı ödeme; çok sayıda süreç karesi | WL-Q04–WL-Q06, WL-Q11, WL-Q16 |
| P3-G04 | E0358–E0379 | 22 | f7-37-debts-fab – f7-58-advanced | D2, D3, arama, menü/ayarlar | WL-Q02–WL-Q04, WL-Q09–WL-Q11; B09/B12 |

### Hata, kod, kaynak ve mekanik paketleri

"Bağlı sorular" sütunu her haritanın "Bağ / hedef iş" sütununda ilgili B
kimliği geçen sorulardan betikle çıkarıldı; ortak ilke bağları dahildir.

| Paket | Bulgu | Bağlı sorular | Tür | Önkoşul |
|---|---|---|---|---|
| P1-B01 | B01 | BC-Q05, HD-Q12, KB-Q08, LI-Q07, MM-Q05, PS-Q04, PS-Q12, QB-Q10, WL-Q13 | Kod/ADR 0014 ve kıyas metinleri | Yok |
| P1-B02-B04 | B02, B03, B04 | BC-Q12, KB-Q10, KB-Q15, PS-Q05, QB-Q10 | Kod (kart, para, tekrarlayan plan, rapor sorgusu) ve kıyas metinleri | Yok |
| P1-B14 | B14 | HD-Q09, HD-Q12 | Kod (kasa sayımı, istemci) | Yok |
| P1-goodbudget-T01 | B05 | GB-Q01, GB-Q02, GB-Q05, GB-Q12 | Kaynak/metin + GB-U01 kullanıcı kontrolü | G01 tamam; yeni deney yok |
| P1-goodbudget-T02 | B06 | GB-Q02, GB-Q03, GB-Q04, GB-Q05, GB-Q12 | İddia kapsamı sınırlandı | G01; tekrar testi yapılmaz |
| P1-B07 | B07 | GB-Q06, GB-Q09, GB-Q12, QB-Q07 | Resmî kaynak; banka bağlantısı kurulmaz | Yok |
| P1-B08 | B08 | KB-Q11, LI-Q08, PS-Q09 | Metin sınırlandırma | P1-kolaybi-G01 |
| P1-B11 | B11 | GB-Q08, HD-Q06, HD-Q12, PS-Q08, WL-Q07 | Görsel/kaynak; e-posta gönderilmez | P1-hesap-defterim-G02 |
| P1-B12 | B12 | GB-Q07, HD-Q03, HD-Q12, KB-Q09, LI-Q02, LI-Q07, PS-Q06, PS-Q10, QB-Q07, WL-Q09 | Metin; şema iddiası gözlenebilir davranışa indirilir | Yok |
| P1-B13 | B13 | QB-Q01, QB-Q02, QB-Q03, QB-Q05, QB-Q08 | Metin/kaynak | P1-quickbooks-G01 |
| P1-B15 | B15 | KB-Q03 | Görsel rol ve tarih eşlemesi | P1-kolaybi-G01, G02 |
| P1-B18 | B18 | BC-Q11, MM-Q10, WL-Q12 | Üç form tablosunun hizalanması | Yok |
| P1-K | B17 | BC-Q01, BC-Q10, GB-Q10, HD-Q01, KB-Q16, LI-Q09, MM-Q01, MM-Q10, PS-Q11, QB-Q12, WL-Q01, WL-Q11 | Mekanik kapı, pozitif/negatif testler | G paketleri sonunda tekrar koşulur |
| P2-Tnn / P2-K | B10, B16 | B10: BC-Q02, BC-Q03, BC-Q04, BC-Q08, KB-Q08, MM-Q06, WL-Q08; B16: BC-Q09, MM-Q06 | Canlı Bluecoins ve uygulama kapanışı | P2-G01–P2-G04 |
| P3-Tnn / P3-K | B09 | BC-Q05, BC-Q07, KB-Q08, WL-Q02, WL-Q03, WL-Q04 | Canlı Wallet/bulut ve uygulama kapanışı | P3-G01–P3-G04; oturum erişimi |
| P1-B19 / P4-K | B19 | BC-Q01, BC-Q13, GB-Q05, GB-Q10, GB-Q12, HD-Q01, HD-Q07, KB-Q02, KB-Q12, KB-Q13, KB-Q17, LI-Q01, LI-Q04, LI-Q08, LI-Q10, MM-Q01, PS-Q01, PS-Q08, PS-Q12, QB-Q01, QB-Q04, QB-Q09, QB-Q11, WL-Q01, WL-Q14 | Özet, karar ve durum yayılımı | Diğer B kapanışları |

### Canlı test adayları — numara G sonuçlarından sonra verilir

Bağ sütununda T geçen sorular aday listesidir; G paketi soruyu cevaplarsa
test açılmaz. Masa başı dört uygulamada (Paraşüt, Logo İşbaşı, QuickBooks,
KolayBi) canlı test adayı yoktur.

| Uygulama | Aday sorular | Not |
|---|---|---|
| Money Manager | MM-Q02, MM-Q03, MM-Q09 | G01 sonrası |
| Hesap Defterim | HD-Q02, HD-Q03, HD-Q08, HD-Q10 | HD-Q03 silme deneyi ayrı izin ister |
| Goodbudget | GB-Q01, GB-Q02, GB-Q03, GB-Q04 | GB-Q01 T01'in, GB-Q03 T02'nin kendisi; GB-Q02 T01/T02'ye bağlı; GB-Q04 izinli |
| Bluecoins | BC-Q03, BC-Q04, BC-Q05, BC-Q06, BC-Q07, BC-Q08, BC-Q14 | P2-G sonrası; veri silinmez/sıfırlanmaz |
| Wallet | WL-Q02, WL-Q03, WL-Q05, WL-Q07, WL-Q08 | P3-G sonrası; bulut verisi salt okunur başlar |

P2-M ve P3-M için ayrı paket açılmaz: P0.3-bluecoins ve P0.3-wallet
haritaları planın P2/P3 birinci adımındaki MD girdisidir. Kaynak form
değişirse yalnız etkilenen bölüm yeniden okunur. Kullanıcı ayrı MD
incelemesi isterse bu karar değişir.

### Önerilen sıra ve uygulama başına ilerleme

1. P1-B01 → P1-B02-B04 → P1-B14: kararı engelleyen BusinessFinance
   kıyasları; yalnız kod/ADR, emülatör gerektirmez. P1 paketleri plan
   Bölüm 4 gereği ilgili kaynak formları ve özetleri düzeltir.
2. P1-goodbudget-G01 → P1-goodbudget-T01 → T02 → P1-B07.
3. P1-kolaybi-G01 → G02 → P1-B08 → P1-B15.
4. P1-hesap-defterim-G01 → G02 → P1-B11 → P1-B12.
5. P1-money-manager-G01; P1-parasut-G01; P1-logo-isbasi-G01;
   P1-quickbooks-G01 → P1-B13.
6. P1-B18 → P1-K.
7. P2-G01–G04 → P2-T → P2-K; ardından P3-G01–G04 → P3-T → P3-K.
8. P1-B19 / P4-K.

Uygulama başına son incelenen görsel yok; sonraki kimlik ilk G paketinin
başıdır: MM E0227, HD E0134, GB E0106, PS E0258, LI E0220, QB E0268,
KB E0180, BC E0016, WL E0274.

### Bu paket sonrası kapsam

- P0.1–P0.4 kapandı; P0 kabul kapısı geçti. Üç harita düzeltmesi kaydedildi.
- Paket sonu doğrulaması: 17 G paketinin aralık, adet, klasör ve ilk/son dosyası
  envanterle eşleşti; 357 PNG'nin her biri tam bir pakette. B/K tablosundaki
  bağlı sorular bağ sütunlarından çıkarılan kümeyle 16/16 satırda aynı. T aday
  listesi ilk yazımda GB-Q01 ve GB-Q03'ü atlamıştı; betik karşılaştırmasıyla
  bulundu, eklendi ve 23/23 eşleşti.
- Yeni görsel incelemesi **0/357**, canlı test **0**, kapanan B bulgusu **0**.
- Kaynak formlar, görseller, emülatör, bulut hesabı ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-B01 — fatura bazlı tahsilat bağı kıyası**.
  Faz 8 açılmadı.


## P1-B01 — Fatura bazlı tahsilat bağı kıyası

**14 Eylül 2026: B01, BusinessFinance kodu, mimari belge ve ADR 0014 ile kontrol
edildi; kanıtla kapandı.** Kullanıcı P0.4 sonrasında "devam et" diyerek kaynak
form düzeltmesi içeren P1 paketine geçti. Emülatör, görsel ve dış kaynak
kullanılmadı; ürün kodu değişmedi.

### BusinessFinance gerçeği — üç boyut

| Model | Gelir/gider tanıma zamanı | Hesap bakiyesine etki | Belirli borca bağlanma | Dayanak |
|---|---|---|---|---|
| Cari borçlandırma `CounterpartyCharge` + tahsilat/ödeme `CounterpartyPayment` | Borçlandırma `ChargeDate`'te gelir/gider; tahsilat rapora girmez | Borçlandırma dokunmaz; tahsilat artırır, ödeme azaltır | **Yok.** Tahsilat yalnız karşı taraf, hesap, yön, tutar ve tarih taşır. Açık bakiye karşı taraf başına hesaplanır; fazla tahsilat kırpılmaz. Vadesi geçmiş tutar, ödemeler önce vadesi geçmiş borçlandırmalardan düşülerek hesaplanır; bu gösterim kuralıdır, kayıt düzeyinde tahsis değildir. Kısmi tutar serbest | [CounterpartyPayment.cs](../../src/BusinessFinance.Domain/CounterpartyPayment.cs) satır 25–34; [CounterpartyCharge.cs](../../src/BusinessFinance.Domain/CounterpartyCharge.cs) satır 25–40; [architecture.md](../../documentation/architecture.md) satır 545–572, 670–673; [EfFinancialReportRepository.cs](../../src/BusinessFinance.Infrastructure/Reports/EfFinancialReportRepository.cs) satır 94–115, 508–539 |
| Tek seferlik yükümlülük `Obligation` + `ObligationSettlement` | Yükümlülük `IssueDate`'te yöne göre gelir/gider; kapanış rapora girmez | Yükümlülük dokunmaz; kapanış artırır veya azaltır | **Var, tam tutarla.** Kapanış `ObligationId` taşır; yükümlülük başına tek kapanış (idempotent, veritabanında tekil indeks), tutarı yükümlülüğün tamamı; kısmi kapanış yok. Durum Açık/Kapandı/İptal türetilir. Karşı taraflı açık yükümlülük cari bakiyeye katılır, kapanışta düşer | [Obligation.cs](../../src/BusinessFinance.Domain/Obligation.cs) satır 42–50, 179–210; [ObligationSettlement.cs](../../src/BusinessFinance.Domain/ObligationSettlement.cs) satır 15, 101, 106; architecture.md satır 592–599, 696–728; EfFinancialReportRepository.cs satır 116–131, 541–549 |
| Taksitli borç `DebtAgreement` / `DebtInstallment` | ADR 0014: gider kaynaklı açılış tanır; anapara ödemesi taşır; faiz ödendiği ay gider | Taksit ödemesi hesabı değiştirir | **Taksit başına, tam taksit.** `MarkPaid` taksite hesap ve tarih yazar; ikinci ödeme reddedilir | [DebtAgreement.cs](../../src/BusinessFinance.Domain/DebtAgreement.cs) satır 338–400; [ADR 0014](../../documentation/adr/0014-economic-event-recognizes-payment-carries.md) satır 45–66 |

ADR 0014 tanıyan ve taşıyan kaydı ayırır; **birbirine bağlanmalarını istemez** ve
fatura bazında kapanış garantisi kurmaz (satır 28–38, 68–90). Sonuç: "bu faturadan
ne kaldı" sorusunu BusinessFinance cari hesapta cevaplamaz; karşı tarafın toplam
açık bakiyesini ve vadesi geçmiş kısmını cevaplar. Belirli kaydın açık/kapalı
durumu yalnız tek seferlik yükümlülükte (tam tutar) ve taksitte vardır.

### Kaynak form düzeltmeleri

Satır numaraları düzeltme öncesi sürüme aittir. Her düzeltme formun içinde
"P1-B01 düzeltmesi, 14 Eyl 2026" notuyla ve eski iddia anılarak işaretlendi.
Rakip gözlemi, görsel ve kanıt etiketi değiştirilmedi.

| Form | Konum | Eski iddia | Yeni ifade | Karar etkisi |
|---|---|---|---|---|
| E0005 Bluecoins | 209–210 | BusinessFinance'in "fatura bağı modeli" Bluecoins'te yok | Fatura bağı BF modelinden çıkarıldı; bağlı kapanışın yükümlülük ve taksitte olduğu notu | — |
| E0005 Bluecoins | 257–259 | BF ayrımı "takip garantili" | BF cari tahsilatı belirli borca bağlamaz; soru cari hesapta BF'de de cevapsız | — |
| E0005 Bluecoins | 335 | Gerekçe: cari modelimiz "fatura bağı ister" | Gerekçe tanıma/taşıma ayrımına indirildi | Etiket değişmedi; kazanım/bedel eksiği BC-Q13'te açık |
| E0005 Bluecoins | 337 | Alma; "bizim modelimiz takibi garantiliyor" | Gerekçe düzeltildi; karar **Henüz karar verme** | Dayanak düştü; fatura bazlı tahsis ihtiyacı Belge 3 sorusu |
| E0014 Wallet | 285–287 | CounterpartyCharge/Payment çiftine "kavramsal en yakın rakip model" | Hüküm kaldırıldı; Debt'e kısmi bağın BF cari modelinde olmadığı yazıldı | Tanıma/taşıma eşdeğerliği B09'da açık |
| E0014 Wallet | 350 | CounterpartyBalance "Wallet'la aynı tarafta" | BF bakiyesi karşı taraf toplamı; borç başına kalanın karşılığı yalnız tam tutarlı yükümlülük | Etiket değişmedi; WL-Q14 açık |
| E0008 KolayBi | 171 | Bakiye kolonu CounterpartyBalance karşılığı; kısmi tahsilat "aynı" | KolayBi bakiyesi fatura satırında, BF'ninki karşı taraf toplamı | — |
| E0008 KolayBi | 341–344 | ADR 0014'e "ayrı ve birbirine bağlı" tezi | ADR 0014'ün bağ istemediği, BF'de bağın yükümlülük/taksitte olduğu, diyaloğun mockup olduğu | KB-Q08'in Tur 2 çelişkisi açık |
| E0011 Paraşüt | 196 | Kısmi ödeme ve otomatik mahsup CounterpartyPayment ile örtüşüyor | Kısmi ödeme ortak; mahsup örtüşmüyor, BF'de yalnız gecikme gösterim kuralı | Etiket değişmedi |

### Bağlı soruların B01 kısmı

İlk sütun bulguyu gösterir; soru kimliği tanım değil atıftır (tanımlar P0.3
haritalarındadır).

| Bulgu | Soru | B01 kısmının sonucu | Açık kalan kısım |
|---|---|---|---|
| B01 | BC-Q05 | Kapandı: 209–210, 257–259, 335, 337 düzeltildi | D2 gelir raporu ve D3 rapor nötrlüğü ölçümü (P2-T); arama toplamı çıkarımı; B09 ortak ilke |
| B01 | HD-Q12 | Formda BF'ye tahsis atfeden ifade yok; 294'teki D3 benzerliği serbest kısmi tutara dayanıyor ve BF'de doğru | 294'teki "hangi ekstreye" kart iddiası; B11/B12/B14 kıyasları |
| B01 | KB-Q08 | Kapandı: 171 ve 341–344 düzeltildi | Tur 2 tablosundaki kapandı ile doğrulanamadı çelişkisi; diyalog kanıtı (P1-kolaybi-G01/G02); B09/B10 ortak ilke |
| B01 | LI-Q07 | Formda BF'ye tahsis atfeden ifade yok; "cari kapanır" Logo'ya ait kaynak iddiası | Logo kaynağında kısmi tutar ve kapanış koşulu; B12 ortak ilke |
| B01 | MM-Q05 | Formda cari tahsis iddiası yok; 213–214 ve 330'daki D3 benzerliği kısmi tutar serbestliğine dayanıyor | Bu Ay/Gelecek Ay ile BF kart ekstre modeli eşdeğerliği için kod kontrolü |
| B01 | PS-Q04 | Kapandı: 196 gerekçesi düzeltildi | Paraşüt otomatik mahsup kuralının tam kaynağı ve istisnaları |
| B01 | PS-Q12 | 196 karar gerekçesi B01 sonucuyla uzlaştı | Hatırlatma, kategorisiz/kapsamsız ve tekrar kıyasları; B19 yayılımı |
| B01 | QB-Q10 | Formda tahsilat bağı iddiası yok; 92. satırdaki tanıma ifadesi doğru | B02–B04 ve diğer kıyaslar |
| B01 | WL-Q13 | Kapandı: 285–287 ve 350 düzeltildi | 313/353 dönem başı çelişkisi; 338, 341, 342, 344, 347, 352 kıyasları |

### Yayılım kontrolü

Taranan: dokuz gözlem formu, E0386 özet karşılaştırma, TUR2 yol haritası, kanıt
girişi, DURUM, test protokolü ve sentetik veri belgesi. Desenler: fatura/tahsilat
bağı, takip garantisi, birbirine bağlı, aynı nesne, borca/faturaya bağlanma,
Counterparty sınıfları, Obligation, ADR 0014 ve mahsup.

- Başka yerde BusinessFinance'e fatura bazlı tahsis atfeden iddia bulunmadı.
  E0386 ve dört formdaki "fatura yerine Obligation/CounterpartyCharge tanıma"
  ifadesi doğrudur.
- B01'e değil başka bulgulara ait kalanlar: Wallet sonucundaki ve TUR2 satır
  212'deki ADR 0014 "tek/en güçlü" hükümleri B09'a; KolayBi sonucundaki "en olgun
  karşılık" KB-Q12/B19'a; DURUM tarihsel bölümlerindeki model hükümleri B19'a.
  Kaynak denetim raporu ve Soru 2 tarihsel girdidir, değiştirilmedi.
- Bluecoins sonucundaki fatura/tahsilat bağlanmaması cümlesi BusinessFinance'e
  yetenek atfetmiyor; değiştirilmedi, dili BC-Q13'te değerlendirilir.

### Bu paket sonrası kapsam

- B01 **kanıtla kapandı**; B01–B19'dan 18'i açık.
- Kaynak form değişikliği: E0005, E0008, E0011, E0014. Yeni hash ve satır
  sayıları envanterin P1-B01 kaydındadır. E0005/E0008/E0014'te satır sayısı arttı;
  P0.3 haritalarındaki satır atıfları ve SHA değerleri okunan eski sürüme aittir.
  Görsel, emülatör, bulut hesabı ve ürün kodu değişmedi; canlı test yok.
- Sıradaki tek paket **P1-B02-B04**. Faz 8 açılmadı.


## P1-B02-B04 — Mevcut kart, plan bitişi ve tanınan gelir kıyasları

**14 Eylül 2026: B02, B03 ve B04 BusinessFinance kodu ve ADR 0014 ile kontrol
edildi; üçü de kanıtla kapandı.** Emülatör, görsel ve dış kaynak kullanılmadı;
ürün kodu değişmedi.

### BusinessFinance gerçeği

| Bulgu | İddia | Doğrulanan davranış | Dayanak |
|---|---|---|---|
| B02 | Asgari ödeme oranı yok | Var: yüzde olarak saklanır, varsayılan %20, 0–100 arası; asgari tutar ekstre borcundan yukarı yuvarlanarak hesaplanır ve borcu aşmaz. İstemci oran alanını, ekstrede asgari tutarı ve "Asgariyi öde" eylemini sunar | [CreditCard.cs](../../src/BusinessFinance.Domain/CreditCard.cs) satır 19–31, 125–135, 179–188; [finance_page.dart](../../mobile/business_finance_mobile/lib/features/cards/presentation/finance_page.dart) satır 799–801, 860–864, 1523–1526 |
| B02 | Kart limiti zorunlu değil | Zorunlu: kurucu `Money` limit alır ve doğrular; `Money` sıfırdan büyük olmalıdır | CreditCard.cs satır 51–79, 168–177; [Money.cs](../../src/BusinessFinance.Domain/Money.cs) satır 8–15 |
| B02 | Kullanılabilir limit gösterimi yok | Var: domain `limit − borç` hesaplar, sıfırın altına düşürmez; istemci kart listesinde ve ayrıntısında "Kullanılabilir" gösterir | CreditCard.cs satır 137–148; finance_page.dart satır 253–254, 689–690 |
| B02 | Bedelsiz kaydın karşılığı sıfır tutarlı işlem olur | Olmaz: `Money` sıfır ve negatif tutarı reddeder; işlem, hızlı ekleme ve yükümlülük formları da sıfırı reddeder. Sıfır reddi normal para hareketleri içindir, kasa sayımına genellenmez | Money.cs satır 10–15; [transactions_page.dart](../../mobile/business_finance_mobile/lib/features/transactions/presentation/transactions_page.dart) satır 664; [quick_add_form_page.dart](../../mobile/business_finance_mobile/lib/features/activities/presentation/quick_add_form_page.dart) satır 696; [obligation_form_page.dart](../../mobile/business_finance_mobile/lib/features/obligations/presentation/obligation_form_page.dart) satır 461 |
| B03 | Plan süresiz, isteğe bağlı bitiş yok | Bitiş tarihi ve toplam tekrar sınırı isteğe bağlı; bitiş başlangıçtan önce olamaz, sınır sıfırdan büyük olmalı; sınır dolunca plan pasifleşir, bitişten sonraki tarih üretilmez; ikisi birlikte verilebilir, önce dolan geçerli. İstemci iki alanı ve "üretilen / sınır tekrar" sayacını gösterir | [RecurringTransaction.cs](../../src/BusinessFinance.Domain/RecurringTransaction.cs) satır 25–27, 234–244, 283–299, 339; [planning_page.dart](../../mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart) satır 1062, 1079–1085, 1256–1257 |
| B04 | Nakit esaslı net tahsil edilmeyen satışı tanımaz | Tanır: cari borçlandırma olay tarihinde, yükümlülük düzenleme tarihinde gelir/gider raporuna girer; tahsilat girmez. ADR 0014 saf nakit esasını reddeder. Tahsil edilmemiş tutar cari bakiyede ve net varlıkta açık alacaktır. Aylık rapor kartında tanınan/tahsil edilen kırılımı yoktur; bu bir gösterim adayıdır, tanıma eksikliği değildir | [EfFinancialReportRepository.cs](../../src/BusinessFinance.Infrastructure/Reports/EfFinancialReportRepository.cs) satır 94–131; [ADR 0014](../../documentation/adr/0014-economic-event-recognizes-payment-carries.md) satır 94–98; [architecture.md](../../documentation/architecture.md) satır 626–640 |

"Nakit esaslı işletme neti" projedeki karar filtresinde ve durum belgesinde bir
ürün terimi olarak geçiyor. B04'ün kapanış ölçütü gereği PRD, ADR ve README karar
filtresi değiştirilmedi; terimin "yalnız tahsil edilen tanınır" diye okunma riski
B19 yayılımına not edildi.

### Kaynak form düzeltmeleri

Satır numaraları bu paketten önceki sürüme aittir (E0008 için P1-B01 sonrası
sürüm). Her düzeltme formda "P1-B02-B04 düzeltmesi, 14 Eyl 2026" notuyla ve eski
iddia anılarak işaretlendi. Rakip gözlemi, görsel ve kanıt etiketi değişmedi.

| Form | Konum | Bulgu | Eski iddia | Yeni ifade | Karar etkisi |
|---|---|---|---|---|---|
| E0008 KolayBi | 231 | B04 | Bizde işletme neti yalnız nakit esaslı | Olay gününde tanıma; tahsil edilmemiş kısım cari bakiyede; aynı kartta kırılım yok | — |
| E0008 KolayBi | 314 | B02 | Kullanılabilir limit gösterimi yok | Domain hesaplıyor, istemci gösteriyor | — |
| E0008 KolayBi | 315 | B02 | Asgari ödeme oranı yok; limit bizde zorunlu değil | İkisi de var; görünen fark kart numarası ve açılış tarihi | — |
| E0008 KolayBi | 376–378 | B03 | Plan süresiz tanımlanır | Bitiş ve tekrar sınırı isteğe bağlı, birlikte verilebilir | — |
| E0008 KolayBi | 391–392 | B02 | Bedelsiz bizde ₺0 tutarlı kayıt olurdu | Karşılığı yok; ₺0 kayıt açılamaz | — |
| E0008 KolayBi | 674 | B03 | İsteğe bağlı bitiş iki üründe de yok | BF'de isteğe bağlı bitiş ve sınır var; fark sayının zorunluluğu | Etiket değişmedi (Henüz karar verme) |
| E0008 KolayBi | 677 | B04 | Netimiz yalnız nakit esaslı | Olay gününde tanıma var; soru aynı kartta iki sayının gösterimi | Etiket değişmedi (Kararı yeniden sor); soru gösterim sorusuna indi |
| E0008 KolayBi | 679 | B02 | Asgari ödeme oranı modelimizde yok | Zaten var, yeni özellik değil | **Henüz karar verme → Doğrudan al** |
| E0005 Bluecoins | 326 | B02 | Sıfır tutarı istemci ve sunucu birlikte reddetmeli | İkisi de zaten reddediyor | Etiket değişmedi; kazanım/bedel eksiği BC-Q13'te |
| E0007 Hesap Defterim | 27 | B04 | Ortak nokta "nakit esaslı" | Ortak nokta elle giriş; BF olay gününde tanır | — |
| E0007 Hesap Defterim | 303 | B02 | Bizde sıfır ya reddedilmeli ya onay istenmeli | BF zaten reddediyor | Etiket değişmedi |

### Bağlı soruların ilgili kısmı

İlk sütun bulguyu gösterir; soru kimliği tanım değil atıftır.

| Bulgu | Soru | Sonuç | Açık kalan kısım |
|---|---|---|---|
| B02 | BC-Q12 | Sıfır tutar kıyası (326) düzeltildi | Düzeltme/iptal, taslak uyarısı, bölme, CSV, TRY, ilk taksit, realize isteği, planlanan görünüm, attentionCode ve varsayılan kıyasları |
| B02–B04 | KB-Q10 | Kart, plan bitişi, sıfır tutar ve tanıma kıyasları (231, 314, 315, 376–378, 391–392, 674, 677, 679) düzeltildi | Cari açılış devri, cari ekstre, Ödeme Planı–InstallmentPlan, taslak/kopya/dönüştür, brüt tutar, scopeBreakdown, tahmini bakiye ve birebir eşlemeler |
| B03 | KB-Q15 | BF opsiyonel bitiş/sınır gerçeği yazıldı (376–378, 674) | KolayBi tekrar planlarının onay ve periyot kuralı (kaynak/G) |
| B04 | PS-Q05 | Formda BF tanıma zamanına dair yanlış iddia yok; 161 ve 197'deki "bu ayın neti kasa değişimi değildir" ifadesi doğru | Planlanmış kovası ve 07/08 raporlarının zamanlaması (kaynak/G) |
| B02–B04 | QB-Q10 | Formda kart, plan bitişi veya tanıma zamanı iddiası yok | Exclude–iptal, bölme, HasBusiness ve diğer kıyaslar |
| B02/B04 | HD-Q12 (ek yayılım) | 27 ve 303 düzeltildi; HD-Q12 P0.4 eşlemesinde B01/B11/B12/B14'e bağlıydı, bu iki düzeltme ek yayılımdır | D3/ekstre, iptal-geri yükleme, fiş satırı ve filtre kıyasları |

### Yayılım kontrolü

Taranan: dokuz gözlem formu, E0386 özet karşılaştırma, TUR2, kanıt girişi,
DURUM, README, plan, raporlar girişi, test protokolü ve sentetik veri. Desenler:
asgari/minimum ödeme, kullanılabilir/kalan limit, limit zorunluluğu, sıfır ve ₺0
tutar, süresiz, isteğe bağlı bitiş, nakit esaslı ve tahakkuk.

- Düzeltilenler dışında BusinessFinance'e bu üç konuda yanlış yetenek atfeden
  ifade bulunmadı. E0386'daki kart, tekrarlayan ve KDV satırları doğrudur.
- DURUM 272 KolayBi kart alanlarını anlatıyor; TUR2 194 ve sentetik veri 77 rakip
  gözlemi/test tanımıdır; Money Manager ve Wallet'taki sıfır tutar satırları kendi
  ürünlerini anlatıyor. Değiştirilmedi.
- DURUM 526 (tarihsel) ve README 132 (karar filtresi) proje terimini kullanıyor;
  çıkarım içermiyor ve B04 kapanış ölçütü gereği korundu. Terim riski B19'a not.

### Bu paket sonrası kapsam

- B02, B03 ve B04 **kanıtla kapandı**; B01–B19'dan 15'i açık.
- Kaynak form değişikliği: E0005, E0007, E0008. Yeni hash ve satır sayıları
  envanterin P1-B02-B04 kaydındadır; P0.3 satır atıfları eski sürüme aittir.
- Görsel, emülatör, bulut hesabı ve ürün kodu değişmedi; canlı test yok.
- Sıradaki tek paket **P1-B14**. Faz 8 açılmadı.


## P1-B14 — Kupür yardımcısı ile kasa sayımı ihtiyacı

**14 Eylül 2026: B14 BusinessFinance domain, uygulama ve istemci koduyla kontrol
edildi; kanıtla kapandı.** Emülatör, görsel ve dış kaynak kullanılmadı; ürün kodu
değişmedi. E0166 (`31-nakit-hesap-makinesi.png`) açılmadı; rakip aracın tarifi
formun metnidir ve görsel onayı P1-hesap-defterim-G02'dedir.

### BusinessFinance gerçeği — iki ayrı yetenek

| Yetenek | Doğrulanan davranış | Dayanak |
|---|---|---|
| Kasa sayımı bir gözlem | Sayım para hareketi değildir: bakiyeye dokunmaz, gelir/gider yazmaz, rapora girmez, `İşlem ekle` menüsünde yer almaz | [CashCount.cs](../../src/BusinessFinance.Domain/CashCount.cs) satır 3–15; [CashCountUseCases.cs](../../src/BusinessFinance.Application/Cash/CashCountUseCases.cs) satır 81–87; [quick_add_models.dart](../../mobile/business_finance_mobile/lib/features/activities/presentation/quick_add_models.dart) satır 18–19 |
| Kimde, hangi tutarla | Yalnız aktif nakit hesap sayılır; banka bakiyesi sayılmaz. Sayılan tutar `Money` değildir: sıfır meşru, negatif ve dört basamaktan uzun tutar reddedilir; gelecek tarih reddedilir | CashCount.cs satır 25–29, 88–124 |
| Beklenen bakiye ve fark | Beklenen tutar ve fark saklanmaz; okuma anında bakiye projection'ından türetilir. Fark yönlüdür: fazla gelir, eksik gider tarafıdır; dengede düzeltme türü yoktur | CashCount.cs satır 137–156; [CashCountDifference.cs](../../src/BusinessFinance.Domain/CashCountDifference.cs) satır 13–15, 30–49 |
| Farkın kayda geçmesi | Kendiliğinden olmaz. Kullanıcı onaylarsa kategori seçilir ve tek bir gelir/gider kaydı yazılır; kapsam sayımdan gelir. İdempotenttir, dengede reddedilir; sonrasında beklenen bakiye sayılan tutara oturur | CashCount.cs satır 158–203; CashCountUseCases.cs satır 209–255 |
| Yeniden sayım | Aynı hesap ve gün için yeni sayım öncekini silmez, iptal eder | CashCount.cs satır 205–232; CashCountUseCases.cs satır 122–138 |
| İstemci yüzeyi | `Kasa` ekranının `Gün sonu` sekmesi: `Uygulamaya göre` ve `Elde sayılan` yan yana, `Fazla`/`Eksik`/`Sayım tuttu` durumu, `Sayımı gir`/`Yeniden say`, `Farkı kaydet` ve geçmiş sayımlar. Fark sunucudan gelir. `Kasa` işletme cevabında ana sekmedir, değilse `Diğer` altındadır | [cash_count_view.dart](../../mobile/business_finance_mobile/lib/features/cash/presentation/cash_count_view.dart) satır 22–25, 131–193, 232–266; [cash_page.dart](../../mobile/business_finance_mobile/lib/features/cash/presentation/cash_page.dart) satır 57–63; [main_shell.dart](../../mobile/business_finance_mobile/lib/features/shell/presentation/main_shell.dart) satır 38–42, 53–57; [more_page.dart](../../mobile/business_finance_mobile/lib/features/more/presentation/more_page.dart) satır 62–73 |
| Kupür × adet yardımcısı | **Yok.** Sayım formunda `Kasada sayılan` tek serbest tutar alanıdır. Araştırma klasörü dışında repoda `kupür`, `denomination` veya `banknot` geçmiyor | cash_count_view.dart satır 347–362; repo araması (14 Eylül 2026) |

Rakip aracın tarifi (E0007 satır 128): adet × kupür satır toplamı ve genel
toplam; muhasebe kaydı üretmez. İki ürün aynı fiziksel ihtiyaca farklı
katmanda dokunuyor: Hesap Defterim sayma işleminin aritmetiğine, BusinessFinance
sayımın sonucunu beklenen bakiyeyle uzlaştırmaya. Biri diğerinin yerine geçmez;
bu yüzden ne "kupür aracı bizde var" ne "kasa ihtiyacının bizde karşılığı yok"
yazılabilir.

[architecture.md](../../documentation/architecture.md) satır 732–734'teki
"yalnız Domain uygulanmıştır" notu güncel kodla uyuşmuyor (migration, endpoint
ve istemci mevcut). Ürün belgesi bu araştırma paketinin kapsamı dışında olduğu
için değiştirilmedi; not olarak kaydedildi.

### Kaynak form düzeltmesi

Satır numarası P1-B02-B04 sonrası sürüme aittir. Düzeltme formda "P1-B14
düzeltmesi, 14 Eyl 2026" notuyla ve eski iddia anılarak işaretlendi. Rakip
gözlemi (satır 128, 223), görsel ve kanıt etiketi değişmedi.

| Form | Konum | Bulgu | Eski iddia | Yeni ifade | Karar etkisi |
|---|---|---|---|---|---|
| E0007 Hesap Defterim | 310 | B14 | Fiziksel kasa mutabakatının dijital-öncelikli modelimizde karşılığı yok, düşük öncelik | Sayım ve fark akışı `Kasa > Gün sonu`'da var; karşılığı olmayan yalnız kupür × adet yardımcısı; öncelik bu kıyasla belirlenmedi | Etiket değişmedi (Not); "düşük öncelik" yanlış öncülden çıkarıldı, BF yüzeyi `—` → `Kasa > Gün sonu sayım formu` |

### Bağlı soruların B14 kısmı

İlk sütun bulguyu gösterir; soru kimliği tanım değil atıftır.

| Bulgu | Soru | Sonuç | Açık kalan kısım |
|---|---|---|---|
| B14 | HD-Q09 | Evet, karıştırılıyordu: 310 kupür yardımcısının yokluğunu kasa sayımı ihtiyacının karşılıksızlığı gibi yazıyordu. Düzeltildi; öncelik artık yokluk varsayımından türetilmiyor | E0166 görsel onayı (P1-hesap-defterim-G02); yardımcının değeri Belge 3 sorusu |
| B14 | HD-Q12 | B14 kısmında 310 dışında BF kasa kıyası yok; 310 HD-Q09 ile düzeltildi | D3/ekstre (294), iptal–geri yükleme, fiş satırı, filtre toplamı; B11/B12 kıyasları |

### Yayılım kontrolü

Taranan: dokuz gözlem formu, E0386 özet karşılaştırma, TUR2, kanıt girişi,
DURUM, README, plan, raporlar girişi, test protokolü ve sentetik veri. Desenler:
kupür, kasa sayım, Nakit Hesap Makinesi, dijital-öncelikli, CashCount.

- Düzeltilen satır dışında BusinessFinance'e kasa sayımı yokluğu atfeden ifade
  bulunmadı.
- E0007 satır 128 ve 223, DURUM 332/369/771 ve TUR2 142 rakip aracı anlatıyor;
  BF iddiası taşımıyor. Değiştirilmedi.
- Diğer formlardaki `kasa` geçişleri rakiplerin kasa/banka hesaplarını anlatıyor;
  B14 ile ilgisi yok. E0386'da kasa sayımı satırı yok.

### Kontroller

- Kod okuması: CashCount, CashCountDifference, CashCountUseCases, cash_count_view,
  cash_page, main_shell, more_page ve quick_add_models tam veya ilgili bölümüyle
  okundu; satır atıfları okunan sürüme aittir.
- Domain/API/istemci testleri mevcut (CashCountTests, CashCountEndpointTests,
  cash_pos_feature_test) ama çalıştırılmadı; ürün kodu değişmediği için
  build/test/analyze gerekmedi.

### Bu paket sonrası kapsam

- B14 **kanıtla kapandı**; B01–B19'dan 14'ü açık.
- Kaynak form değişikliği: E0007. Yeni hash envanterin P1-B14 kaydındadır; P0.3
  satır atıfları eski sürüme aittir (satır sayısı değişmedi).
- Görsel, emülatör, bulut hesabı ve ürün kodu değişmedi; canlı test yok.
- Önerilen sıranın 1. adımı (P1-B01 → P1-B02-B04 → P1-B14) tamamlandı.
  Sıradaki tek paket **P1-goodbudget-G01**. Faz 8 açılmadı.


## P1-goodbudget-G01 — E0106–E0133 görsel içerik incelemesi

**14 Eylül 2026: 28 görselin 28'i açıldı, E0006 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-goodbudget-G01 bölümüne hemen yazıldı.**
Emülatör açılmadı; kaynak form, görseller ve dosya adları değişmedi. Bu paket
B05/B06/B07'yi kapatmaz; canlı karşılaştırma T01/T02'nin, resmî kaynak P1-B07'nin işidir.

### Kareler zinciri — zaman sırası

Durum çubuğu saatleri, kareleri tek zaman çizelgesine koydu (11 Eylül; E0133
tarih alanı 12 Eylül'ü gösteriyor):

| Saat | Kare | Olay |
|---|---|---|
| 08:26–08:54 | E0106–E0109, E0108 | Açılış, zarf formu, bütçe kurulumu, doldurma diyaloğu |
| 09:00 | E0110 | Kayıt ekranı (koşumda kayıt tamamlandı, E0111 başlığı) |
| 09:02 | E0115 | Zarflar dolu, **hiçbir hesap yokken** |
| 09:03–09:06 | E0111–E0114 | Hesap katmanı açıldı, Ana Hesap 20.000, limit diyaloğu |
| 09:10–09:18 | E0116–E0118 | 25.000 Credit, zarf zorunluluğu, Market'e bağlanma |
| 09:29–09:30 | E0119–E0120 | Transfer formu; hesap 42.950 |
| 09:33–09:37 | E0121–E0124 | Raporlar: Eylül 2.050 gelir; Ağustos −22.950 |
| 09:43 | E0125 | K08 Test silme diyaloğu; hesap 42.934 |
| 09:50 | E0126 | B1 planı (600, Every 2 Weeks, 10 Ağu) |
| 09:54–09:56 | E0127–E0129 | Arama tek satır; hesap 42.334 |
| 12:20–12:24 | E0130–E0133 | Liste beş satır; hesap 41.734; arama tek satır; sıklık listesi |

### Form metni ile görsel arasındaki uyuşmazlıklar — düzeltme adayları

Kaynak form bu pakette değiştirilmedi. Satır numaraları E0006'nın P0.3 sürümüne
aittir (SHA `02a03667…`). Her aday, kapanışı yapacak pakete devredildi.

| No | Form konumu | Formdaki ifade | Görsel gerçeği | Devredilen paket |
|---|---|---|---|---|
| G01-1 | 25 | Hesap türü yok | E0111 Checking/Savings/Cards anıyor; E0112 üç grup (kart ve borç dahil) gösteriyor | P1-B07 |
| G01-2 | 33 | Ücretsiz sürümde üç tür ortak havuzu paylaşıyor | E0114 diyaloğu tür seçimi görünmeden çıkıyor; ortak havuz çıkarımdır | P1-B07 |
| G01-3 | 33 | Hesap formu sade: ad, açılış bakiyesi, tür | Hesap ekleme formu hiçbir karede yok | P1-B19 metin sınırı |
| G01-4 | 31, 141–145 | Kayıt `LATER` ile atlanabiliyor | E0110'da düğme var, atlama denenmedi; koşumda kayıt tamamlandı | P1-B19 metin sınırı |
| G01-5 | 34, 215 | `Available` havuzuna doğrudan yatırma bu formda yok | E0117'de zarf açılırının seçenekleri görünmüyor | P1-goodbudget-T01 (B05) |
| G01-6 | 38 | Ağustos'ta 2.650 gider vardı (−22.950 raporunun karşısında) | Rapor 09:36–09:37'de, B1'in 600'ü 09:50'de yazıldı; rapor anında gider 2.050'dir | P1-goodbudget-T02 (GB-Q02) |
| G01-7 | 39 | Düzenleme anında, onaysız kaydediliyor | E0125'te 13 yazılı ama hesap ve zarf 16'yı yansıtıyor; kayıt görünmüyor | P1-goodbudget-T02 (GB-Q04) |
| G01-8 | 77–78, 208, 253 | 600, planın bir sonraki örneğiyle aynı | 10 Ağu'dan iki haftalık planda 24 Ağu ve 7 Eyl 11 Eylül'de geçmişteydi; ikisi yazılsaydı fark −1.200 olurdu | P1-goodbudget-T02 (B06) |
| G01-9 | 120 | Varsayılan `Once` | E0126 ve E0133'te kutu işaretli, varsayılan görünmüyor | P1-B19 metin sınırı |
| G01-10 | 121 | `09` formun tüm alanlarını gösteriyor (B2 negatifi) | E0116'nın alt kısmı klavye çubuğu altında; `Split into multiple Envelopes` karede yok | P1-B19 metin sınırı |
| G01-11 | 38, 180 | Arama hızlı | E0127/E0132 yalnız sonucun doğruluğunu gösterir; hız ölçülmedi | P1-B19 metin sınırı (GB-Q11) |

**Form metnini güçlendiren görsel gerçekleri:** 25.000 Credit'in Market zarfına
yazıldığı üç ayrı karede görülüyor (E0118, E0125'teki 24.984 hesabı, E0130).
Zarflar hesap açılmadan dolduruldu (E0115 → E0113), doldurmanın hesaba dokunmadığı
bununla tutarlı. K08 silmesinin gerçekten yapıldığı E0130'da görülüyor, bu yüzden
−16 yorumu dayanağını koruyor. E0133 12 Eylül tarihli ölçümlerin tarihini
destekleyen tek kare.

**Formda anılmayan yeni gözlemler:** zarf başına dönem seçimi ve cihazda gizleme
(E0107); iki grupta ayrı sayaç (E0109); `Remaining 22,950` = tahmini gelir −
bütçe (E0108); transfer formunda zarf alanı yok (E0119); konum diyaloğunun
ikinci cümlesi (E0118); Income vs Spending çubuğunun tablo değerinden kısa
çizilmesi (E0124). Hepsi gözlemdir, değerlendirme değildir.

### Bağlı soruların ilerlemesi — hiçbiri kapanmadı

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| GB-Q01 | Credit → Market bağı üç karede kesin; doldurmanın hangi düğmeyle yapıldığı görünmüyor | Normal gelir yolu görsellerde yok (T01) |
| GB-Q02 | −22.950 raporu B1 kaydından önceki ana ait; 1.200 + 850 − 25.000 ile tutarlı; Market satırının yokluğunun ikinci bir açıklaması (net negatif zarf) aynı toplamı veriyor | Market satırının neden listelenmediği |
| GB-Q03 | Tek arama satırı iki gün aynı; iki geçmiş örnek kısıtı (G01-8) | −600'ün nedeni (T02) |
| GB-Q04 | Silme yapıldı (E0130); "onaysız kaydetme" desteklenmiyor (G01-7) | 13'ün kaydedilip kaydedilmediği, iç mekanizma |
| GB-Q06 | Üç hesap türü görünüyor; ortak havuz çıkarım; split alanı karede yok | Resmî paket kaynağı (P1-B07) |
| GB-Q07 | Zarflar hesapsız dolduruldu; sayaç grup bazlı görünüyor; iki `[Available]` anlamı hâlâ yazmıyor | İki `[Available]`, Total/All Accounts ilişkisi |
| GB-Q08 | Konum önerisi, e-posta hatırlatması ve "get reports" yalnız uygulama beyanı olarak görüldü | Hiçbir dış davranış denenmedi |
| GB-Q09 | Fiş eki yokluğu yalnız `Add Transaction` menüsüyle sınırlı | Settings açılmadı |
| GB-Q10 | 28 PNG'nin tamamı açıldı; 02b/02c/23–26 kareleri mevcut ve formdaki rollerle uyumlu; eski "22/24 kare" beyanları yeni onay sayılmadı | B17 mekanik kapısı |
| GB-Q11 | Hız ve adım yorumları karelerle ölçülemiyor (G01-11) | Kullanıcı testi yok |
| GB-Q05, GB-Q12 | Görsel katkısı yok | B05/B06 kapanışı ve Belge 3 kararları |

### Bu paket sonrası kapsam

- Görsel inceleme **28/357** (Goodbudget tamam). Canlı test 0; kapanan B 0; B01–B19'dan 14'ü açık.
- Kaynak form, görseller, emülatör ve bulut household'u değişmedi.
- Sıradaki tek paket **P1-goodbudget-T01**: B05 gelir yolu karşılaştırması.
  Emülatör ve mevcut household oturumu gerekir; veri silinmez/sıfırlanmaz;
  başlangıç durumu değişiklikten önce kaydedilir. Faz 8 açılmadı.



## Goodbudget kapanış hazırlığı — 14 Eylül 2026 kullanıcı kararı sonrası

G01 incelemesinin on bir düzeltme adayı forma uygulandı. Önceki G/M kayıtları
inceleme anının kaydıdır; eski satır numaraları veya açık sorular yeni test
talimatı değildir. Kaynak kimlikleri GB-S01–GB-S03 envantere kaydedildi.
Görsel sayacı 28/357; bu bütün araştırmanın değil yeni denetimin sayacıdır.

| Soru | Güncel durum | Sonuç / kalan sınır |
|---|---|---|
| GB-Q01 | Form 14 Eylül'de; **From New Income / Fill Each Envelope sonucu 20 Eylül K3'te kapandı** | GB-U01-A/B/C form ve Keep Available seçeneği; E0417–E0424 yeni +1.234 kaydın sonucunu gösterir. Eski Initial Fill fonlama seçimi, Keep Available sonucu ve hesap değişmeme nedeni bilinmiyor |
| GB-Q02 | Kanıtla kapandı / neden sınırlandı | Rapor B1 öncesi; gider 2.050. Market satırının görünmeme nedeni bilinmiyor |
| GB-Q03 | İddia kapsamı sınırlandı | 600 farkı sonraki tekrara bağlanmaz; geçmiş vade sayısından üretim kuralı çıkarılmaz |
| GB-Q04 | İddia kapsamı sınırlandı | Silinen satır yok; 13 düzenlemesinin kalıcılığı ve 16 farkının nedeni bilinmiyor |
| GB-Q05 | İddia kapsamı sınırlandı | Genel rapor/silme/tekrar kusuru hükümleri kaldırıldı; B05 ekran kontrolü GB-U01 ile tamam |
| GB-Q06 | Kanıtla kapandı / erişim sınırı | Hesap grupları ve paket kaynağı ayrıldı; kart/transfer/taksit canlı kapsamı genişletilmedi |
| GB-Q07 | İddia kapsamı sınırlandı | İki Available satırının anlamı ve iç depolama bilinmiyor; fiziksel tablo sonucu çıkarılmaz |
| GB-Q08 | İddia kapsamı sınırlandı | Konum/e-posta/backup/widget yüzeyi fiilî dış davranış kanıtı değil; dış etki denenmez |
| GB-Q09 | İddia kapsamı sınırlandı | Settings açılmadı; fiş/dil/hedef yokluğu yalnız gezilen yüzeyle sınırlı |
| GB-Q10 | Goodbudget içerik/atıf kısmı kapandı | G01 28/28; 22/24 eski sayılar tarihsel. Ortak B17 mekanik kapısı diğer uygulamalar için açık |
| GB-Q11 | İddia kapsamı sınırlandı | Hız/erişilebilirlik ölçülmedi; yalnız gözlenen sonuç ve görsel yorum |
| GB-Q12 | İddia kapsamı sınırlandı | Kesin otomasyon/silme/gelir reddi gerekçeleri kaldırıldı; öneriler Belge 3 onayı değil |

### GB-U01 — Tamamlanan kullanıcı kontrolü: normal gelir ekranı

- Neden: Credit yolu görüntülendi; normal gelir yolunun güncel Android arayüzü
  mevcut 28 karede yok. Resmî kaynak işlevi açıklıyor ama bu sürümün ekranı değil.
- İşlem: ENVELOPES sekmesindeki Fill Envelopes girişini aç. From New Income
  seçeneğine gir; How to Fill Envelopes veya karşılığı altındaki Keep Unallocated /
  dağıtmadan tutma seçeneğini göster.
- Kanıt: giriş formunun ve bu seçeneklerin ekran görüntüsü; seçenek yoksa gördüğün
  seçenek adları ve ekran yeterli. Mevcut işlem değiştirilmez; kayıt kaydedilmez.
- Bitiş: seçeneğin var/yok sonucu ve ekran kaydı. 25.000 Credit deneyi, yeni gelir
  kaydı, bakiye mutabakatı, 600 kök nedeni, silme deneyi veya yeni vade bekleme yok.
- Sonuç: tamamlandı. GB-U01-A/B/C arşivlendi; iki gelir/fonlama yüzeyi ve
  Keep Available seçeneği görüldü. B05 kapandı. Yeni işlem/rapor testi yapılmadı.
- Sonraki uygulama: Goodbudget kapandıktan sonra mevcut sırada P1-kolaybi-G01.
  Bu tur diğer uygulamaya veya Faz 8'e geçilmedi.

### Kullanıcının paylaştığı işlem listesi

14 Eylül konuşmasındaki 14:13 durum çubuklu karede beş satır ve tek
10 Ağustos 600 Bulut kaydı görülüyor. Eylül 10 satırı yok; kare hesap bakiyesini
ve kaydın sıklığını göstermiyor. Bu yüzden sonraki kesinti veya nedensellik
kanıtı değildir. Görsel henüz kalıcı envantere alınmadı; raporun tek dayanağı
yapılmaz. Aynı liste için mevcut E0130/E0132 yeterlidir.


## Goodbudget kapanışı — 14 Eylül 2026

Kullanıcı GB-U01'i üç ekranla tamamladı. Güncel ad Keep Available; kaynak
rehberdeki Keep Unallocated ifadesiyle karıştırılmaz. Kanıtlar formda ve
envanterde GB-U01-A/B/C olarak bağlıdır. B05 ve B07 kanıtla, B06 iddia
kapsamı sınırlandırılarak kapandı. GB-Q01–GB-Q12 için sonuç/sınır kaydı mevcut;
Goodbudget için bekleyen kullanıcı kontrolü kalmadı. Ortak B17/B19 paketleri
diğer uygulamalar ve son çapraz kontrol için açık; Goodbudget'ı yeniden
test etme gerekçesi değildir.

Goodbudget görsel kapsamı 31/31 (G01 28 + kullanıcı kontrolü 3).
B01–B19 içinde 7 kanıtla kapandı, 1 kapsamı sınırlandı, 11 açık.
Sonraki tek paket P1-kolaybi-G01; bu tur başlatılmadı. Faz 8 açılmadı.


## P1-kolaybi-G01 — E0180–E0197 görsel içerik incelemesi

**14 Eylül 2026: 18 görselin 18'i açıldı, E0008 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-kolaybi-G01 bölümüne yazıldı.** Kaynaklar mobil
giriş karesi (E0180), eski tanıtım videosu (E0181–E0186), ayrı 2026 videosu
(E0187) ve destek mockup'larıdır (E0188–E0197). Canlı test, web/destek sayfası
erişimi, hesap açma veya emülatör koşumu yapılmadı. Oturum kesintisi sonrası
kaydedilmiş 18 sonuç yeniden incelenmedi; paket kaydı ve yayılım tamamlandı.

### Forma uygulanan düzeltmeler

Mevcut kanıtla çözülen metin hataları canlı test beklemeden düzeltildi; formda
her biri `P1-kolaybi-G01 düzeltmesi` notu veya G01 başlık notuyla işaretli.

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| KG01-1 | E0188 | Demo proje adları kullanıcıların ekseni hane gideri için büktüğünü gösteriyor | Adlar destek materyalindeki örnek gözlemidir; talep veya amaç dışı kullanım kanıtı değil (B08/KB-Q11) |
| KG01-2 | E0188, E0190 | Listedeki Net ile detaydaki Kar/Zarar aynı sayının iki adı | d01 listesi d03'teki Proje1'i göstermiyor; iki farklı etiket gözlem, değer eşitliği kanıtlanmadı |
| KG01-3 | E0190, E0196 | Pasif seçeneği silme yerine pasifleştirme kuralı | Her iki karede Pasif/Pasifleştir yanında silme de var; kural çıkarılmaz |
| KG01-4 | E0192 | Ödeme Durumu radyosu aynı kayıtta tanıma+taşıma yapıyor, çifte sayımı önlüyor | Seçenek arayüzde var; oluşan kayıtlar ve kasa etkisi bilinmiyor; model eşdeğerliği kurulmaz |
| KG01-5 | E0197 | Ekstre e-posta ile gönderiliyor | Önizlemede düğme var; gönderim yapılmadı, teslim kanıtı değil |
| KG01-6 | E0196, E0197 | Diyalog ve önizleme tek ekstre işleminin önü/sonu | Form bitişi 26.09.2023, çıktı aralığı 25.09.2023 ile bitiyor; aynı işlem zinciri sayılmaz |
| KG01-7 | E0183 | Kısmi ödeme gerçek veriyle doğrulandı | Dolu demo satırı; ödeme öncesi/sonrası yok, davranış testi değil (KB-Q07/Q08) |
| KG01-8 | E0184 | Patron parası Ortaklar / Personel Carileri sekmesinden giriyor | Sekmelerin varlığı gözlem; Ortaklar hiç açılmadı, akış çıkarımdır (KB-Q11) |
| KG01-9 | E0181, E0187 | Çekirdek iskelet 6 yılda değişmemiş; olumlu istikrar sinyali | Ara sürümler görülmedi, özet alanı kesik; yalnız iki karenin benzerliği (KB-Q12) |

KG01-7…9 bu oturumda, envanter satırlarıyla hâlâ çelişen form cümleleri olarak
bulundu ve düzeltildi. Faz 7.5 bölümündeki eski "39 kare açıldı" beyanları
tarihsel kayıttır; G02 incelemesinin yerine geçmez.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| KB-Q01 | E0180 giriş alanları, QR ve TR/EN kaydedildi; karede kayıt bağlantısı yok | Paket/sürüm kimliği karede görünmüyor; 16 KB uyarısı görsel kanıtta yok, kullanıcı/koşum notu olarak kalır |
| KB-Q03 | 01 mobil, 02–07 eski video, 08 ayrı video rolleri ayrıldı; Mayıs 2020 ve 1.8.2024 içerik tarihi, yayın/sürüm tarihi değil; 05 arşiv, d07 birincil | 07/d19 ve 02/d24 çiftlerinin ikinci yarısı G02'de; B15 kapanışı P1-B15 |
| KB-Q05 | d03 sekme/kart adları ve Aktif/Pasif menüsü görüldü | Nakit Durumu içeriği, kâr marjı hesabı, Proje Takip ayarının etkisi (Ayarlar karesi yok) |
| KB-Q06 | d05 Cari Takibi ve Ödeme Durumu radyoları görüldü; form metni sınırlandırıldı (KG01-4) | Ödendi→kasa etkisi; Bedelsiz/Tahsilata Kapalı/Bakiye kolonu G02 (d11–d12) |
| KB-Q07 | E0181 Ödemeler satırının altı kesik; 04 dolu demo satırı; donut tutarları demo veri | d26/d27/d29 aritmetik kontrolleri G02 |
| KB-Q08 | E0183 durum kolonu ve d09 ayrı Borç/Alacak – Ödeme/Tahsilat düğmeleri; "gerçek veriyle doğrulandı" kaldırıldı | Ödemenin belirli faturayı kapattığı kanıtlanmadı; maaş seçim diyaloğu G02 (d16) |
| KB-Q09 | d05 radyolarından kayıt türü/şema çıkarımı kaldırıldı | 266/453 iç çelişkisi ve d30 fatura farkları G02 + KB-Q16 |
| KB-Q11 | d01 adları gözlem–hipotez olarak ayrıldı; Ortaklar akışı çıkarım işaretlendi | Karar tablosu ve özet satırlarına yayılım P1-B08 |
| KB-Q14 | E0181 donutlar tutar, E0182 sayaçlar adet; d05 vade boş bırakılabilir alan | Tahmini dönem sonu formülü d29 (G02); BF karşılığı kod kontrolü |
| KB-Q16 | E0180–E0197 dosyaları mevcut ve içerikleri adlarıyla uyumlu; d09/d10 tarih farkı kaydedildi | d30 başlık/ad uyuşmazlığı G02; kısa atıf beyanları B17 mekanik kapısı |

KB-Q02, KB-Q04, KB-Q10, KB-Q12 (KG01-9 dışında), KB-Q13, KB-Q15 ve KB-Q17'ye bu
paketin görsel katkısı yok.

### Ek kullanıcı kontrolü gerekmedi

KolayBi mobilde kayıt engelli olduğu için emülatör sorusu üretilmedi. Açık
kalan davranışlar (Ödendi→kasa, Proje Takip ayarı, belirli faturayı kapatma)
sınırlandırılmış gözlem olarak kalır; canlı test gibi gösterilmez. Kullanıcı
isterse web hesabı ayrı karardır.

### Bu paket sonrası kapsam

- Görsel inceleme **49/360** (eski korpus 46/357 + GB-U01 3/3). KolayBi 18/39;
  Goodbudget 31/31. Canlı test 0.
- B01–B19: 7 kanıtla kapandı, 1 kapsamı sınırlandı, **11 açık**. B08/B15 bu
  paketle kapanmadı; metin katkıları P1-B08/P1-B15'e devredildi.
- Kaynak form düzeltildi; PNG, dosya adları, ürün kodu, PRD/ADR değişmedi.
- Sıradaki tek paket **P1-kolaybi-G02 (E0198–E0218, 21 görsel)**; bu tur
  başlatılmadı. Faz 8 açılmadı.


## P1-kolaybi-G02 — E0198–E0218 görsel içerik incelemesi

**14 Eylül 2026: 21 görselin 21'i açıldı, E0008 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-kolaybi-G02 bölümüne yazıldı.** Hepsi destek
mockup'ıdır; canlı test, web/destek sayfası erişimi veya emülatör koşumu yok.

### Forma uygulanan düzeltmeler

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| KG02-1 | E0198 | Ödenmedi satırları dört (₺50.000, ₺500, ₺5.600, ₺30.000) | Beşinci satır ₺1.250/₺0 eklendi; e-Fatura Aktarıldı yalnız Ödendi satırlarında (gözlem, kural değil) |
| KG02-2 | E0203 | Diyalog ödenmemiş maaş tahakkuklarını listeliyor | Diyalog "ödenmemiş" demiyor; kalan tahakkuk listesi çıkarım; iki ₺1.000 satırı -₺100 bakiyeyle uzlaşmıyor |
| KG02-3 | E0204 | Brüt-net bordro mantığı | Alan adları; Ara/Brüt/Net/Genel hepsi ₺1.000, kesinti görülmedi; cari bilgisi boş |
| KG02-4 | E0205 | Tekrarlayan planın tek somut formu; Başlangıç Tarihi varsayılan bugün | Mevcut maaştan Tekrarlı Maaşa Dönüştür diyaloğu; tarih kaynak kaydın tarihiyle aynı (22.04.2023) |
| KG02-5 | E0214 | KolayBi KDV'yi hesaplıyor; alt blok İndirilecek | Matris ve Hesaplanan KDV Toplam görülüyor; hesaplama mı toplama mı ayırt edilemez; İndirilecek etiketi/toplamı karede yok |
| KG02-6 | E0215 | Aylık raporumuzun karşılığı | Rapor boş; aynı aralıklı KDV karesinde ₺12.000 fatura var; içerik kapsamı bilinmiyor |
| KG02-7 | E0217 | Satış/iade formunda üç fark | Karedeki form Yeni Alış İade Faturası; farklar alış faturasına göre, satış faturası formu karelerde yok; dosya adı değiştirilmedi, formdaki kare adı uyarısı geçerli |
| KG02-8 | E0211 | d24, 08'den daha eski sürüm | Menü/kampanya farkından çıkarım; aradaki sürüm geçmişi görülmedi |
| KG02-9 | E0201, E0202 | Liste ve detay aynı personeli gösteriyor (örtük) | Helin Kınay listede Serbest Çalışan/₺0, detayda Yarı Zamanlı/-₺100; ardışık aynı veri anı sayılmaz |
| KG02-10 | E0196, E0183 | Tur 2 tablosu: fatura→tahsilat bağı ve kısmi tahsilat Kapandı; ekstre e-posta | Arayüz seviyesinde görüldü, bağ ve bakiye etkisi doğrulanmadı; e-posta düğmesi, gönderim yok (KB-Q08 iç çelişkisi giderildi) |
| KG02-11 | E0186, E0206 | 07 ve d19 aynı ekranın iki sürümü | Seçili sekmeler farklı (Kasalar / Banka Hesapları); sekme çubuğu karşılaştırması (KB-Q03) |
| KG02-12 | E0212 | Bizde tek kullanıcı | Hesap başına tek kullanıcı, paylaşılan şirket alanı yok; hatırlatıcı bildirimi görülmedi |

Faz 7.5 bölümündeki "39 karenin hepsi açıldı" ve aritmetik kalite beyanı
tarihsel kayıttır: üç aritmetik kontrol (d26, d27, d29) bu turda yeniden
tuttu, ama farklı tarih aralıklı karelerden gelir ve bütün korpusun sayı
güvenliğini kanıtlamaz (KB-Q07).

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| KB-Q03 | d24'ün 08'den eski olduğu çıkarım işaretlendi; 07/d19 farklı sekme olarak ayrıldı; 06/d31 zaten farklı sekme | Rol/tarih eşlemesinin form geneline yayılımı P1-B15 |
| KB-Q05 | d15 uyarı bandı cari borç/ödeme → proje Nakit Ödemeler, alacak/tahsilat → Nakit Tahsilatlar metnini gösteriyor | Akışın fiilen gerçekleştiği ve Proje Takip ayarı (Ayarlar karesi yok) |
| KB-Q06 | d11 Bakiye deseni sekiz satırla yeniden sayıldı; d12 Tahsilata Kapalı rozeti ve Tahsilata Aç menüsü | Bakiye kolonunun anlamı, Bedelsiz, Ödendi→kasa etkisi |
| KB-Q07 | d26/d27/d29 aritmetiği tuttu; d26 08–15.08, d27/d28 19–26.09 aralıklı; d28 boş | Korpus genelinde sayı güvenliği iddiası kullanılmaz |
| KB-Q08 | d16 seçim diyaloğu var ama "ödenmemiş" etiketi ve seçim sonrası ekran yok; Tur 2 "Kapandı" satırları düzeltildi | Ödemenin belirli kaydı kapattığı ve kısmi tutar davranışı |
| KB-Q09 | d12 GELİR/GİDER başlığından serbest gelir sonucu çıkarılmadı; d30 farkları alış iade formuna bağlandı | 266/453 iç çelişkisi (metin; P1-B08/B19) |
| KB-Q13 | d27 hesaplama/toplama ayrılamadı; d13/d30 formlarında yalnız dosya alanı | KDV davranışı, OCR, muhasebeci erişimi kaynakla |
| KB-Q14 | d29 tüm tahsilat Belirsiz kovasında, Ödemeler satırı matriste yok; formül kareden türetildi | BF planlanan görünüm karşılığı kod kontrolü |
| KB-Q15 | d18 bağımsız plan formu değil dönüştürme diyaloğu; periyot değerleri görünmüyor | Tekrarlı Genel Gider/Alış/Satış listelerinin kuralı (yalnız sekme adları görüldü) |
| KB-Q16 | d30 dosya adı ile karedeki başlık uyuşmazlığı doğrulandı; ad değiştirilmedi | Kısa atıf beyanları B17 mekanik kapısı |

KB-Q01, KB-Q02, KB-Q04, KB-Q10, KB-Q11, KB-Q12 ve KB-Q17'ye bu paketin görsel
katkısı yok.

### Görsel adlandırma — yanlış anlama ve geri alma

Kullanıcı 14 Eylül'de ayrı duran sekiz kareyi (01–08) 31 destek karesinin
serisine katıp tek 39'luk sıra kurmayı istedi. İstek yanlış anlaşıldı: 31 destek
karesi yeniden adlandırılıp atıflar güncellendi. Kullanıcı düzeltince aynı gün
adlar ve atıflar tersine çevrildi; 31 hash korundu. Ardından kullanıcı
kararıyla eski 01–08 kareleri d32–d39 olarak serinin sonuna eklendi; 31 destek
karesinin adı değişmedi, 8 hash korundu. KolayBi formu (tam ad ve kısa kodlar),
envanter (E0180–E0187), DURUM ve kanitlar/README atıfları güncellendi. Bu kayıttaki,
DURUM'daki ve raporlardaki eski 01–08 kısa kodları başka uygulamaların numaralarıyla
karıştığı için değiştirilmedi; eşleme E0391 P1-kolaybi-G02 bölümündedir.

### Ek kullanıcı kontrolü gerekmedi

KolayBi mobilde kayıt engelli; emülatör sorusu üretilmedi. Açık davranışlar
sınırlandırılmış gözlem olarak kalır. Web hesabı ayrı kullanıcı kararıdır.

### Bu paket sonrası kapsam

- Görsel inceleme **70/360** (eski korpus 67/357 + GB-U01 3/3). KolayBi 39/39;
  Goodbudget 31/31. Canlı test 0.
- B01–B19: 7 kanıtla kapandı, 1 kapsamı sınırlandı, **11 açık**. B08/B15 bu
  paketle kapanmadı; metin ve rol yayılımı P1-B08 ve P1-B15'te.
- Kaynak form düzeltildi; 31 destek karesinin adı değişmedi, eski 01–08 kareleri
  d32–d39 oldu; PNG içerikleri değişmedi.
- Sıradaki tek paket **P1-B08**; bu tur başlatılmadı. Faz 8 açılmadı.

## P1-B08 — Demo verisi ve kayıt seçenekleri kullanıcı talebi değildir

**14 Eylül 2026: B08 iddia kapsamı sınırlandırılarak kapandı.** Kapanış ölçütü
metin sınırlandırmasıdır: demo/konumlandırma gözlemi ile kullanım/ihtiyaç
hipotezi ayrıldı; "kullanıcılar böyle yapıyor" hükümleri kaldırıldı. Yeni
kullanıcı araştırması, web/destek erişimi, görsel veya canlı test yapılmadı.
Ortak ilke bağlı sorulardaki Logo İşbaşı ve Paraşüt ifadelerine de uygulandı.

### Düzeltilen yerler (25)

| Dosya | Konum | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| E0008 KolayBi | Sistem işleyişi, işletme/şahsi satırı | Patron parası Ortaklar / Personel Carileri sekmesinden giriyor | Sekmeler var; giriş yolu çıkarım |
| E0008 KolayBi | Video kareleri, d36 satırı | Paraşüt'le birebir aynı workaround; patron parası cari borç-alacağına dönüşüyor | Benzer cari yapısı; dönüşüm çıkarım, davranış gözlenmedi |
| E0008 KolayBi | Video karşılaştırma notu | Tek havuz + kapsamla aynı ihtiyaca iki cevap | Aynı soruna verilebilecek iki cevap; rakipte kullanım çıkarım |
| E0008 KolayBi | Cari ekstre satırı ve karar tablosu (2) | Hesap özeti gönderme ihtiyacı bizde karşılanmıyor | İşlev bizde yok; ihtiyacın büyüklüğü ölçülmedi |
| E0008 KolayBi | Örtüşme denetimi, d36 sonucu | Ortak-carisi çözümü yeni bir ekleme değil | Ortaklar sekmesi eski görünümde de var; kullanım çıkarım |
| E0008 KolayBi | Karar: proje bazlı gelir-gider | "Birden çok raporlama boyutu" talebinin kanıtı | Rakibin tasarım seçimi; talep kanıtı değil |
| E0008 KolayBi | Karar: Ortaklar / Personel Carileri (2) | "…ile patron parası"; kaybettirdiği kesin | Sekmeler + çıkarım; kaybettirdiği koşullu |
| E0008 KolayBi | Karar: iki seviyeli kategori | Alt kategori talebinin rakipteki karşılığı | Yaklaşımın örneği; talep kanıtı değil |
| E0008 KolayBi | Tek cümlelik sonuç | Ayrımı ortak carisi ve proje ekseniyle çözüyor; aynı ihtiyaca farklı cevap | Tek havuz görülmedi; araçların bu amaçla kullanımı çıkarım |
| E0009 Logo İşbaşı | Ürün kimliği, hedef kullanıcı | Kurye/Öğrenci bile var — mikro/bireysel vurgu | Görünen seçenekler; konumlandırma gözlemi, kullanım/talep kanıtlanmaz |
| E0009 Logo İşbaşı | Sesli fatura, ne kazandırıyor | Kurye seçeneğiyle tutarlı kitle | Varsayımsal senaryo; seçenek kullanım göstermez |
| E0009 Logo İşbaşı | Karar: sesli fatura | Hedef kitlemizle örtüşen gerçek sürtünme noktası | Örtüşebilecek sürtünme hipotezi; ölçülmedi |
| E0009 Logo İşbaşı | Karar: Müşavir Portal | İhtiyaç gerçek ve doğrulandı (Paraşüt'te de var) | İki rakibin seçimi; ihtiyaç kullanıcıyla doğrulanmadı |
| E0009 Logo İşbaşı | Karar: Kurye/Öğrenci | Hedef kitleyi okumanın en net kanıtı; segmentimizle örtüşüyor | Konumlandırma gözlemi + çıkarım; kullanıcı dağılımı kanıtlanmaz |
| E0009 Logo İşbaşı | Tek cümlelik sonuç | Açıkça mikro işletmeyi hedefleyen | Mikro işletmeye konumlanan; kullanıcı dağılımı ölçülmedi |
| E0011 Paraşüt | Karar: işletme/şahsi (2) | Ayrım hiç yok; kullanıcı gündelik harcamasını ikinci uygulamada tutuyor | Kaynaklarda görülmedi; kullanıcı davranışı çıkarım |
| DURUM.md | Faz 6 KolayBi özeti | ADR 0013'ün üçüncü ve en güçlü kanıtı; kullanıcılar proje boyutunu büküyor | Demo adları gözlemi; ADR 0013 kanıtı değil, kullanım hipotez |
| DURUM.md | Uygulama tablosu, KolayBi satırı | Ortaklar ile patron parası (ADR 0013 farkının 2. kanıtı) | Sekmeler; giriş yolu çıkarım, fark yalnız sekme düzeyinde |
| DURUM.md | Rakip karşılaştırma tablosu ve ardındaki not (3) | Patron parası ortak/personel carisi workaround'uyla giriyor; aynı ihtiyaca iki cevap; bedeli kesin | Giriş yolu çıkarım; aynı soruna verilebilecek cevaplar; bedel koşullu |
| raporlar/turk-on-muhasebe-vs-businessfinance.md | Hedef kitle ve kurucu fark (2) | Kurye, öğrenci hedef kitle; patron harcaması dolaylı yoldan girer | Konumlandırma gözlemi; giriş yolu çıkarım |

Parantezli sayılar aynı satırda birden çok değişikliği gösterir; toplam 25 yer.
Ayrıca DURUM'daki KDV raporu cümlesi G02 düzeltmesinin (KG02-5) yayılımı olarak
düzeltildi; B08 sayısına girmez.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| KB-Q11 | B08 kısmı kapandı: demo adları, proje ekseni, Ortaklar ve "talep kanıtı" ifadeleri gözlem/çıkarım/hipotez olarak ayrıldı; karar tablosu ve sonuç yayılımı tamam. KB-M11 harita satırı P0.3 anının kaydıdır | Yeni kullanıcı araştırması yok; ihtiyacın varlığı Belge 3'te hipotez olarak tartışılır |
| LI-Q08 | B08 kısmı kapandı: Kurye/Öğrenci konumlandırma gözlemi, segment örtüşmesi ve ihtiyaç doğrulaması ifadeleri sınırlandı | Sektör listesinin tamamı; bordro/tekrar gibi var/yok iddialarının kaynağı (B19/kaynak) |
| PS-Q09 | B08 kısmı kapandı: patron cebi ve gündelik harcama davranışı çıkarım olarak yazıldı | Ürün hedefi ile şahsi gider kaydetme imkânının kaynakla ayrılması (B19/kaynak) |

### Yayılım kontrolü

Araştırmada "Ev Elektrik", "amacı dışında", "bük", "üçüncü … kanıt", "Kurye",
"Öğrenci", "patron cebi", "hane gideri", "talep kanıtı", "ihtiyaç gerçek"
kalıpları tarandı. Kalan eşleşmeler: düzeltilmiş cümleler, bulgu/envanter
kayıtlarındaki alıntılar, Logo formunun sektör listesi gözlemi ve
raporlar/Soru 2 ile 13 Eylül denetimi (tarihsel girdi, değiştirilmedi).
README karar filtresi, PRD ve ADR'ler değişmedi.

### Bu paket sonrası kapsam

- B01–B19: 7 kanıtla kapandı, **2 iddia kapsamı sınırlandı (B06, B08)**, **10 açık**.
- Görsel inceleme 70/360; canlı test 0. Görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-B15** (KolayBi kaynak/tarih rolleri ve görsel rol
  eşlemesi; KB-Q03); bu tur başlatılmadı. Faz 8 açılmadı.

## P1-B15 — KolayBi görsellerinin kaynak ve tarih rolleri

**14 Eylül 2026: B15 kanıtla kapandı.** Dayanak G01/G02 görsel incelemesidir
(39/39); yeni web/destek erişimi, canlı test veya görsel değişikliği yok.

### Kaynak rolleri

| Kareler | Kanıt | Kaynak / yöntem | Tarih bilgisi ve sınırı |
|---|---|---|---|
| d32 | E0180 | Emülatörde mobil giriş ekranı, manuel gözlem (1 Eyl 2026) | Yakalama tarihi; mobil sürüm/paket kimliği karede yok |
| d33–d38 | E0181–E0186 | "KolayBi' Nedir, Neden Kullanmalıyım?" tanıtım videosu karesi; transkript kullanıcıdan | Mayıs 2020 takvimi, #EVDEKALTÜRKİYE gibi ~2020 içerik işaretleri; yayın tarihi kaynakta yok |
| d39 | E0187 | Ayrı bir videodan tek kare, transkriptsiz | "2026 başı yükleme" kullanıcı aktarımı; 1.8.2024 grafik tarihi demo verisi; bugünkü sürüm olduğu doğrulanmadı |
| d01–d31 | E0188–E0218 | kolaybi.com/destek mockup'ları (12 Eyl 2026 erişim) | 2023 işlem tarihleri demo verisi; arayüzün video karelerinden yeni olduğu menü/sekme farkından çıkarım |

### Eski/yeni eşleşmeler

| Eşleşme | Sınıf | Kullanım |
|---|---|---|
| d33 ↔ d24, d39 | Aynı ekran (Güncel Durum), farklı görünüm | d33 dolu vade donut'ları için kalır; d24'ün d39'dan eski olduğu çıkarım |
| d36 ↔ d07 | Aynı ekran (cari listesi) | d07 birincil; d36 tarihsel çapa, seçim önerisi (silme/arşiv yetkisi değil) |
| d37 ↔ d31 | Aynı modül, farklı sekme (Tümü / Varyantlar) | İkisi de kalır |
| d38 ↔ d19 | Aynı modül, farklı sekme (Kasalar / Banka Hesapları) | Sekme çubuğu karşılaştırması; kredi kartı düzeltmesinin öncesi/sonrası |

Eşleşmeye girmeyenler: d32, d34, d35. d39 pano eşleşmesinde yeni taraftadır.

### Düzeltilen yerler (35)

- **E0008 KolayBi formu (30):** inceleme türü; video bölümü girişi ve demo
  uyarısı; destek aritmetik notu; ekran akışı başlığı; d39 satırı (2);
  karşılaştırma notu; Faz 6 girişi (2); cari listesi sekme notu; finans bölümü
  (2); pano karşılaştırma satırları (d36, d39 grafik başlığı); Faz 7.5 pano ve
  finans maddeleri (3); aritmetik maddesi (2); kare temizliği beyanı; örtüşme
  denetimi girişi ve tablo başlığı; d33/d36/d38 satırları (3); örtüşmeyenler;
  tarihsel çapa notu; kanıt düzeyi (3); karar tablosu d36 atfı.
- **DURUM.md (5):** Faz 6 gerekçesi (2023+ arayüz); Faz 7.5 KolayBi satırı (d24/d39
  çıkarımı, kanıt kalitesi, dört eşleşme); Tur 1 video notu kod eşlemesi.

Önceki "39 karenin hepsi açıldı, ölü atıf yok" beyanı 12 Eylül kaydıdır;
güncel mekanik onay B17/P1-K işidir.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| KB-Q03 | Kapandı: roller, tarih katmanları ve dört eşleşme sınıfı form ve özete işlendi | — |
| KB-Q07 | Aritmetik beyanı "kontrol edilen üç sayı" ile sınırlandı; aynı kiracı/an iddiası kaldırıldı | Korpus geneli sayı güvenliği iddia edilmez |
| KB-Q16 | 12 Eylül "tüm kareler açıldı / ölü atıf yok" beyanı tarihsel olarak işaretlendi | B17 mekanik kapısı (P1-K) |

### Bu paket sonrası kapsam

- B01–B19: **8 kanıtla kapandı**, 2 iddia kapsamı sınırlandı (B06, B08), **9 açık**.
- KolayBi paketleri (G01, G02, B08, B15) tamam. Görsel inceleme 70/360; canlı test 0.
- Görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-hesap-defterim-G01** (E0134–E0154, 21 görsel); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-hesap-defterim-G01 — E0134–E0154 görsel içerik incelemesi

**14 Eylül 2026: 21 görselin 21'i açıldı, E0007 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-hesap-defterim-G01 bölümüne yazıldı.** Emülatör
açılmadı; yeni canlı test, silme veya veri değişikliği yok. Görseller ve dosya
adları değişmedi.

### Kareler zinciri — zaman sırası

| Saat (10 Eyl) | Kare | Olay |
|---|---|---|
| 17:58 | E0135, E0136 | İlk açılış diyaloğu; boş ana ekran |
| 18:00–18:05 | E0145, E0146, E0152, E0138 | Defter ekleme; dört defter listesi; açılış sonrası Günlük görünüm; gelir formu (tarih henüz Eyl-10) |
| 18:08–18:09 | E0141, E0147 | Kart gideri -1.200 ve toast; 3.000 Aktar formu |
| 18:12–18:14 | E0148, E0140, E0142, E0149 | Kart özeti Denge 0; birleşik liste 44.950 (Herşey, Aylık); yedekleme diyaloğu |
| 18:16–18:21 | E0143, E0137, E0139, E0150, E0151, E0134 | Çöp kutusu ara anı; Ana Hesap 40.800; İşlem adları; ayarlar; kategori alanı açık; mağaza |
| 19:04–19:06 | E0153, E0154 | A testi: kart -600; Ana Hesap 40.400 |
| 11:39 (12 Eyl) | E0144 | Takvim yeniden çekimi, Ana Hesap Ağustos 43.150 |

### Forma uygulanan düzeltmeler (12)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| HG01-1 | E0134 | Sürüm 235 ve mağaza ölçüleri (4,8★, 139 B, 10 Mn+, PEGI 3, etiketler) bu kareye dayanıyor (3 yer) | Karede yalnız geliştirici, reklam/IAP, güncelleme tarihi ve yenilik notu var; diğerleri görsel kanıtı olmayan koşum notu |
| HG01-2 | E0150, E0151 | Açıklama/kategori anahtarı ilk kez ek koşum 2'de açıldı | 10 Eylül'de 18:19 kapalı, 18:20 açık görünüyor |
| HG01-3 | E0154 | Tek bacak silinince net varlık 40.400'e bozuldu; düzenleme de diğer bacağı güncellemiyor | Kare yalnız Ana Hesap defterini gösteriyor; 40.400 normal transfer sonrası bakiye; bozulma ve düzenleme karede yok |
| HG01-4 | E0144 | Takvimde tek net sayı, giriş/çıkış ayrı gösterilmiyor (2 yer) | Aynı günde iki yön yok, net/ayrı doğrulanamaz; yön renk + hücre içi konum |
| HG01-5 | E0135, E0136, E0149 | Yedekleme uyarısı her açılışta / tekrar tekrar (2 yer) | Şerit ve diyalog iki yüzeyde görüldü; sıklık ölçülmedi |
| HG01-6 | E0140 | Toplam Alındı 51.200 içinde açılış 22.000 | Açılış satırları karede kesik; 22.000 aritmetikle bulunur |
| HG01-7 | E0138 | Gelir formu → dolu ekran akışı | Form karesinde tarih henüz Eyl-10; kayıt sonra Ağu 03'e alınmış |
| HG01-8 | E0146, E0148, E0149 | Üç defter; kart sıfırı yalnız özet karesinde | Listede varsayılan defterle dört defter; kart sıfırı Herşey görünümünde de (E0149) |

Parantezli sayılar aynı düzeltmenin birden çok yerini gösterir; toplam 12 yer.

**Formu güçlendiren görsel gerçekleri:** 44.950 birleşik Denge (E0140/E0142),
Ana Hesap 40.800 (E0137), kartın -1.200 → 0 zinciri (E0141 → E0148/E0149), A
testinin -1.000 → -600 sonucu (E0153), Günlük görünümde Önceki denge satırı ve
Herşey'de açılışın Alındı satırı olması (E0152/E0137), çöp kutusu (E0143),
Aktar formunun iki deftere yazan tek form olması (E0147/E0140).

**Formda anılmayan yeni gözlemler:** Denge sıfırda mavi (E0136); özet
görünümünün başlığında "Özet" adı görünmüyor (E0148); takvimde yön hücre içi
konumla da veriliyor (E0144); İşlem adları diyaloğunda etkin seçim görünmüyor
(E0139); 12 Eylül takviminde Ağustos'ta 150 gider ve 2.500 gelir de var (ek
koşum 2 kayıtları, G02 kapsamı).

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| HD-Q01 | G01'in 21 PNG'si açıldı; 09 karesinin 12 Eylül yeniden çekimi saatle doğrulandı; mağaza ölçüleri kareye dayanmıyor | 36–43 ekleri ve 23 numaralı silinmiş kare G02; mağaza URL/paket kimliği kaynak işi |
| HD-Q02 | 40.400 Ana Hesap defteri, net varlık değil; 44.950 birleşik ve 40.800 Ana Hesap karelerle tutarlı; kart sıfırı iki görünümde kanıtlı | K04'teki 1.150 ve 47.300 son kontrol G02 (E0172/E0173); öksüz bacak etkisinin görsel kanıtı yok |
| HD-Q03 | Tek bacak silme ve düzenleme genellemesi bu karelerde görünmüyor; iddia koşum anlatımına sınırlandı | Saklama/fiziksel yapı ifadeleri P1-B12; yeni silme deneyi kendiliğinden yapılmaz |
| HD-Q04 | Açılışın Günlük'te Önceki denge, Herşey/Aylık'ta Alındı satırı olduğu kareyle doğrulandı; Tasarruf sütunu gün bazında görüldü | "Alındı/Ödendi = gelir/gider" kıyasının Belge 2 anlatımı |
| HD-Q10 | Takvimde net/ayrı gösterim doğrulanamadı; üst ayarlar listesi doğrulandı | Ay/yıl/hafta başlangıcı ayarları G02 (E0178); diyalog içerikleri denenmedi |
| HD-Q11 | Form alanları ve iki buton karelerle doğrulandı; tüm tutarlar tam sayı veri | Adım sayısı ve öğrenilebilirlik ölçüm değil; ondalık desteği bu veriden çıkarılmaz |

HD-Q05–HD-Q09 ve HD-Q12'ye bu paketin görsel katkısı yok (G02/B11/B14 kapsamı).

### Ek kullanıcı kontrolü gerekmedi

Öksüz transfer bacağının görsel kanıtı yok, ama plan gereği kök neden veya
davranışı yeniden üretmek kapanış şartı değil; iddia koşum anlatımı olarak
sınırlandı. Kullanıcı isterse tek soruluk kontrol: Is Karti ve Ana Hesap
defterlerinde Eyl 10 400 bacaklarının güncel durumu (salt okunur, silme yok).

### Bu paket sonrası kapsam

- Görsel inceleme **91/360** (eski korpus 88/357 + GB-U01 3/3). Hesap Defterim
  21/45; KolayBi 39/39; Goodbudget 31/31. Canlı test 0.
- B01–B19: 8 kanıtla kapandı, 2 iddia kapsamı sınırlandı, 9 açık. B12 bu
  paketle kapanmadı (P1-B12).
- Kaynak form düzeltildi; görseller, dosya adları, emülatör verisi ve ürün
  kodu değişmedi.
- Sıradaki tek paket **P1-hesap-defterim-G02** (E0155–E0178, 24 görsel); bu
  tur başlatılmadı. Faz 8 açılmadı.

## P1-hesap-defterim-G02 — E0155–E0178 görsel içerik incelemesi

**14 Eylül 2026: 24 görselin 24'ü açıldı, E0007 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-hesap-defterim-G02 bölümüne yazıldı.** Emülatör
açılmadı; yeni canlı test, e-posta gönderimi, silme veya veri değişikliği yok.
`23-kamera-onay-ekrani.png` envantere hiç alınmamıştır, sayılmadı. Hesap
Defterim görselleri 45/45.

### Kareler zinciri — zaman sırası

| Saat | Kare | Olay |
|---|---|---|
| 11 Eyl 05:30–05:36 | E0155–E0158 | Öğe dökümü 150; tutar/not dolu form; Fatura ekle menüsü; ataçlı kayıt (Ana Hesap 40.650, Eyl 10 -400 bacağı yok) |
| 11 Eyl 05:38–05:41 | E0159–E0161 | Ek tam ekran görüntüleme; İşlem adları Özel; yeniden adlandırılmış ana ekran |
| 11 Eyl 05:47–05:59 | E0162, E0163, E0164, E0165, E0166 | Boş form ve aynı oturumda kaydedilen 0; 2.500 gelir; arama "Ada"; Not Defteri; Nakit Hesap Makinesi |
| 11 Eyl 06:01–06:03 | E0167–E0169 | Sıfır kaydın çöp kutusu menüsü ve kalıcı silme onayı; dışa aktarma diyaloğu |
| 11 Eyl 06:12 | E0170 | Test kayıtları geçici silinmiş ara kontrol 44.950 |
| 12 Eyl 11:38–11:44 | E0171–E0173, E0175–E0178 | Menü; 47.300 (kayıtlar 07:29/07:38 saatleriyle yeniden kurulmuş); Ortak Cuzdan 4.150; defter başına Bildiri; kasadefteri uyarısı + 2 dosya paylaşımı; menü alt kısmı; ayarlar alt bölüm |
| Saat yok | E0174 | Geçiş reklamı (Test Ad) |

### Forma uygulanan düzeltmeler (11)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| HG02-1 | E0159 | K08 düzenleme/taşıma kanıtı bu kare | Düzenleme ekranı ve kopyala ikonu var; Hesaplar/taşıma alanı diyalog altında görünmüyor |
| HG02-2 | E0162 | Boş tutar sessiz ret, 12 Eylül'de yeniden ölçüldü | Kare 11 Eylül'ün boş formu; basma sonrası an ve 12 Eylül ölçümü karede yok |
| HG02-3 | E0164 | Aramada alt toplamlar yeniden hesaplanıyor | Ek olarak satır Denge'si de filtre içinde 25.000; eşleşme Notlar'da |
| HG02-4 | E0169, E0176 | PDF içeriği doğrulandı; dosya adları | PDF içeriği ve adlar hiçbir karede yok (koşum notu); 2 dosya paylaşımı E0176'da |
| HG02-5 | E0176 | Klasör yok, dosyalar Documents/Hesap Defterim/ altında (cihazda doğrulandı) | Uyarı ve paylaşım karede; klasör/yol dosya yöneticisi karesiyle belgelenmedi |
| HG02-6 | E0174 | Kapat birkaç saniye pasif; 12 Eylül'de yeniden görüldü (3 yer) | Karede Kapat etkin; pasiflik süresi ve tarih statik kareden doğrulanmaz |
| HG02-7 | E0161 | Yeniden adlandırma PDF sütun başlıklarını da değiştiriyor | Bu etki karelerde yok, koşum notu |
| HG02-8 | E0166 | TL kupürleri, ₺200–₺0,05 | Para simgesi yok; TL değerlerden çıkarım |
| HG02-9 | E0157, E0159 | Kamera izin + çekim onay akışı çalıştı | İzin/onay ekranı kalıcı kanıtta yok; önizleme ve tam ekran görüntüleme var |

Parantezli sayı aynı düzeltmenin birden çok yerini gösterir; toplam 11 yer.

**Formu güçlendiren görsel gerçekleri:** öğe dökümünün tutar ve nota geçmesi
(E0155→E0156); Özel adların ana ekran, buton ve toplam etiketlerine yayılması
(E0160→E0161→E0162); sıfırın başlıksız ve Denge'ye etkisiz kabulü (E0163);
iki aşamalı silme (E0167→E0168); ara kontrol 44.950 ve güncel 47.300 zinciri
(E0170, E0172: 51.200 + 2.500, 6.250 + 150); iki test kaydının silinip farklı
saatlerle yeniden kurulması metodoloji notunu destekliyor (05:28/05:42 →
07:29/07:38); Ortak Cuzdan 4.150 ve K04'teki 1.150 (E0173); 17 kalemlik menü,
Diğer uygulamalar ve 21 ayar (E0171, E0177, E0150+E0178); kategori anahtarının
açık bırakılması (E0178).

**Formda anılmayan yeni gözlemler:** Eyl 10 -400 bacağı 11 Eylül 05:36'da Ana
Hesap'ta yok (E0158); sıfır kayıt listede kırmızı, çöp kutusunda yeşil
(E0167); uzun özel ad başlıkta kırılıyor ve "Toplam" öneki kalkıyor (E0161);
aramada satır bakiyesi filtreye göre yeniden hesaplanıyor (E0164);
Gelir Gider tanıtımı "Borcu yönet.İndir" / "Kategori bilge harcamaları
yönetin" metinleri (E0177).

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| HD-Q01 | 45/45 görsel açıldı; 36–43'ün 12 Eylül ekleri saatle doğrulandı; 23 envanterde yok, kayıp ilan edilmedi | Paket kimliği/mağaza URL'si kaynak işi; B17 mekanik kapısı |
| HD-Q02 | 44.950 ara ve 47.300 son kontrol zinciri kareyle tutuyor; Ortak Cuzdan 1.150/4.150 doğrulandı; -400 bacağı 11 Eylül sabahı Ana Hesap'ta yok | Öksüz bacağın oluştuğu ve kaldırıldığı anın görsel kanıtı yok |
| HD-Q05 | Seçim/uyarı/paylaşım ekranları kanıtlı; PDF içeriği, dosya adları ve gerçek klasör kareye dayanmıyor, sınırlandırıldı | Çıktı dosyası kalıcı kanıtı; Excel kapsam dışı |
| HD-Q06 | E-posta ayarı 12 Eylül'de açık (E0178); varsayılan/gönderim kanıtı değil | Form metni P1-B11'de düzeltilecek; gönderim denenmez |
| HD-Q07 | Fatura ekle menüsü ve Diğer uygulamalar tanıtımı görüldü; OCR/bölünmüş ürün hükmü menü ve tanıtımla sınırlı | Diğer uygulamalar kurulmadı |
| HD-Q08 | İki aşamalı silme kanıtlı; sessiz ret ve K08 taşıma karelerle desteklenmiyor, sınırlandırıldı | Taslak kaybı ve sessiz ret yalnız koşum notu |
| HD-Q09 | Kupür × adet aracı ve canlı toplam görüldü; muhasebe kaydı üretmediği kareden görünmez | B14 kıyası kapalı (P1-B14) |
| HD-Q10 | Ayın ilk günü 1, yıl/hafta değeri görünmüyor, varsayılan süre Herşey (E0178) | Dönem ayarının sınır etkisi denenmedi |
| HD-Q11 | Reklam Kapat pasifliği ve adım/hız yorumları karelerle ölçülemiyor | Kullanıcı testi yok |

HD-Q03, HD-Q04 ve HD-Q12'ye bu paketin yeni görsel katkısı yok (G01 ve P1-B12 kapsamı).

### B11'e devredilen metin

Formdaki "İşlem dökümünü e-posta ile otomatik gönder ✓ (varsayılan açık)"
ifadeleri (arayüz taraması, sistem işleyişi ve karar tablosu) E0178'deki anlık
durumu kurulum varsayılanı gibi yazıyor. Bu pakette değiştirilmedi; B11
kapanışında düzeltilecek. Dışarıya e-posta gönderilmedi.

### Ek kullanıcı kontrolü gerekmedi

PDF içeriği, klasör yolu ve reklam Kapat süresi karar engeli değil, anlatım
sınırıdır; koşum notu olarak işaretlendi. Kullanıcı isterse salt okunur
kontrol: cihazda Documents/Hesap Defterim/ klasörü ve içindeki dosya adları.

### Bu paket sonrası kapsam

- Görsel inceleme **115/360** (eski korpus 112/357 + GB-U01 3/3). Hesap
  Defterim 45/45; KolayBi 39/39; Goodbudget 31/31. Canlı test 0.
- B01–B19: 8 kanıtla kapandı, 2 iddia kapsamı sınırlandı, 9 açık.
- Kaynak form düzeltildi; görseller, dosya adları, emülatör verisi ve ürün
  kodu değişmedi.
- Sıradaki tek paket **P1-B11** (açık e-posta ayarını gönderim veya varsayılan
  kanıtı saymama; HD-Q06); bu tur başlatılmadı. Faz 8 açılmadı.

## P1-B11 — Açık e-posta ayarı ve yerellik beyanı

**14 Eylül 2026: B11 iddia kapsamı sınırlandırılarak kapandı.** Kapanış ölçütü
metin sınırlandırmasıdır. Emülatör açılmadı; e-posta gönderilmedi, yeni kurulum
veya reset yapılmadı, dış hesap bağlanmadı.

### Kanıtın gösterdiği ve göstermediği

| Konu | Karede görünen | Doğrulanmayan |
|---|---|---|
| E-posta döküm ayarı | E0178: "İşlem dökümünü e-posta ile otomatik gönder" 12 Eylül 11:44'te işaretli | Kurulum varsayılanı; alıcı adresi; tetikleyici/sıklık; gerçek gönderim ve teslim |
| Yerellik | E0149: "Kayıtlarınızı sunucularımızda saklamıyoruz…" uygulama içi diyalog | Ağ trafiği; reklam SDK'sı, Drive ve e-posta kanallarının veri akışı |
| Drive yedeği | E0135/E0136/E0149 davetleri | Hesap bağlanmadı, yedek/geri yükleme denenmedi |

### Düzeltilen yerler (8)

| Dosya | Konum | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| E0007 Hesap Defterim | Oturum bilgisi, erişim kısıtı | Tamamen yerel | Sunucuda saklamama beyanı; ağ denetlenmedi, Drive/e-posta dış temas |
| E0007 Hesap Defterim | K00 görev satırı | Veriler yalnız yerelde | Uygulamanın beyanı |
| E0007 Hesap Defterim | Ayarlar alt yarı | E-posta ayarı ✓ (varsayılan açık) | 12 Eylül'de işaretli; varsayılan, alıcı, tetikleyici, gönderim doğrulanmadı |
| E0007 Hesap Defterim | Sistem işleyişi, dış temas | E-posta ayarı (varsayılan açık) | İşaretli ayar; alıcı/tetikleyici/gönderim görülmedi |
| E0007 Hesap Defterim | Sistem işleyişi, veri yeri | Tamamen yerel (cihaz) | Beyana göre cihazda; beyan denetlenmedi |
| E0007 Hesap Defterim | Karar: e-posta ayarı | Varsayılan açık; kullanıcı istemeden döküm dışarı çıkıyor | Koşullu kazanım/bedel; yalnız görünen ayara dayanır; ilke olarak kapalı doğmalı |
| E0007 Hesap Defterim | Karar: yedekleme uyarısı | Yerel-önce dürüst duruş | Yerel-önce beyanı; ağ denetlenmedi |
| DURUM.md | Uygulama tablosu, Hesap Defterim | Yerel-only veri | Uygulama beyanı; e-posta ayarı gönderim kanıtı değil |

### Bağlı soru

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| HD-Q06 | Görünen ayar, kurulum varsayılanı, alıcı/tetikleyici/kanal ve gönderim ayrıldı; E0149 beyanı ağ denetimi sayılmadı; form ve özet düzeltildi | Varsayılan ve gönderim ancak yeni kurulum ve ayrı kullanıcı izniyle doğrulanabilir; bu çalışmada yapılmayacak (karar engeli değil) |

### Yayılım kontrolü

Araştırmada "varsayılan açık", "e-posta ile otomatik", "tamamen yerel",
"yalnız yerelde", "yerel-önce", "Yerel-only" kalıpları tarandı. Kalan
eşleşmeler düzeltilmiş cümleler ve bulgu/envanter kayıtlarındaki alıntılardır.
DURUM Faz 7.5 satırındaki ayar listesi gönderim iddiası taşımadığı için
değiştirilmedi. README karar filtresi, PRD ve ADR'ler değişmedi.

### Bu paket sonrası kapsam

- B01–B19: 8 kanıtla kapandı, **3 iddia kapsamı sınırlandı (B06, B08, B11)**, **8 açık**.
- Görsel inceleme 115/360; canlı test 0. Görseller, dosya adları, emülatör
  verisi ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-B12** (ekran davranışından fiziksel veri şeması
  çıkarmama; Hesap Defterim ve Wallet; HD-Q03); bu tur başlatılmadı. Faz 8 açılmadı.

## P1-B12 — Ekran davranışından fiziksel veri yapısı çıkarılmaz

**14 Eylül 2026: B12 iddia kapsamı sınırlandırılarak kapandı.** Kapanış ölçütü
metin değişimidir: fiziksel tablo/saklama iddiaları gözlenebilir liste, arama,
toplam ve silme davranışıyla yeniden yazıldı. Rakiplerin kodu, veritabanı veya
teknik dokümanı incelenmedi; erişim varsayılmadı. Emülatör açılmadı.

### Düzeltilen yerler (10)

| Dosya | Konum | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| E0014 Wallet | Faz 7 arama | D2'nin aramada çıkmaması işlem tablosuna hiç yazılmadığını doğruluyor | Aramada ve işlem listesinde görünmüyor, Debts ekranında görünüyor; saklama bilinmiyor; rapor görünürlüğü ölçülmedi |
| E0014 Wallet | Karar: Record sorusu | Record'suz borcun işlem tablosuna hiç yazılmaması | İşlem listesinde ve aramada görünmemesi; saklama bilinmiyor |
| E0007 Hesap Defterim | Ek bütünlük bulgusu | Aktar tek çift-kayıt nesnesi değil, iki bağımsız satır olarak saklanıyor | Listede iki ayrı satır; tek nesne mi iki kayıt mı çıkarılamaz |
| E0007 Hesap Defterim | Sistem işleyişi: kayıt ne üretir | İkinci bir tablo/rapor kaydı yok; raporlar türev toplam | Başka kayıt görünmüyor, toplamlar tutarlı; ikinci tablo çıkarılamaz (etiket Manuel gözlem + Çıkarım) |
| E0007 Hesap Defterim | Sistem işleyişi: transfer | İki bağımsız satır üretir (çift kayıt nesnesi değil); Denge bacaklar senkronsa doğru | Listede iki ayrı satır; bacak silme/düzenleme etkisi koşum anlatımı |
| E0007 Hesap Defterim | Pipeline şeması | Özet/Takvim/Bildiri aynı satırların türevi | Toplamlar satırlarla tutarlı; türetme yapısı çıkarım |
| E0007 Hesap Defterim | Karar: Aktar | İki bağımsız satır; öksüz bacak; çift kayıt bütünlüğü yok | Listede iki satır; öksüz bacak koşum notu; kazanım/bedel koşullu; bütünlük kuralı doğrulanmadı |
| DURUM.md | Faz 4 Hesap Defterim özeti | İki bağımsız satır, öksüz bacak net varlığı bozuyor, çift kayıt bütünlüğü yok | Listede iki satır; koşum notu, görsel kanıtı ve saklama bilgisi yok |
| DURUM.md | Uygulama tablosu, Hesap Defterim | İki bağımsız satır (bir bacak silinince öksüz kalıyor) | Kalma durumu koşum notu, görsel kanıtı yok |
| TUR2-YOL-HARITASI.md | Faz 4 özeti | Transfer bacakları öksüz kalabiliyor | Koşum notu, görsel kanıtı yok |

BusinessFinance tarafındaki "`Transfer` tek atomik kayıt" ifadesi kendi
kodumuzun bilinen gerçeğidir; değiştirilmedi. Goodbudget'taki "fiziksel
tablo/defter yapısı bilinmiyor" ve KolayBi'deki "depolama eşdeğerliği
kanıtlanmış değildir" cümleleri zaten sınırlıydı.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| WL-Q09 | B12 kısmı kapandı: tablo yokluğu hükmü arama/liste/Debts görünürlüğüyle değiştirildi | Statistics/Cash flow gibi raporlarda görünürlük ölçülmedi (P3 kapsamı) |
| HD-Q03 | B12 kısmı kapandı: saklama, ikinci tablo ve atomiklik ifadeleri kaldırıldı; öksüz bacak koşum anlatımı olarak kaldı | Öksüz bacağın oluşumu için görsel kanıt yok; yeni silme deneyi kendiliğinden yapılmaz |

### Yayılım kontrolü

Araştırmada "tablosuna", "ikinci bir tablo", "saklanıyor", "kayıt nesnesi",
"çift kayıt", "öksüz", "bağımsız satır", "bütünlüğü yok", "veritabanı",
"şema", "atomik" kalıpları tarandı. Kalan eşleşmeler düzeltilmiş cümleler,
bulgu/envanter alıntıları, BusinessFinance kendi modeli ve "pipeline şeması"
başlıklarıdır. README karar filtresi, PRD ve ADR'ler değişmedi.

### Bu paket sonrası kapsam

- B01–B19: 8 kanıtla kapandı, **4 iddia kapsamı sınırlandı (B06, B08, B11, B12)**, **7 açık**.
- Görsel inceleme 115/360; canlı test 0. Görseller, dosya adları, emülatör
  verisi ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-money-manager-G01** (E0227–E0256, 30 görsel); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-money-manager-G01 — E0227–E0256 görsel içerik incelemesi

**14 Eylül 2026: 30 görselin 30'u açıldı, E0010 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-money-manager-G01 bölümüne yazıldı.** 30 dosyanın
SHA-256 değeri başlangıç kaydıyla aynı. Emülatör açılmadı; yeni canlı test,
silme veya veri değişikliği yok. Görseller ve dosya adları değişmedi. `05`
numarası envantere hiç alınmamıştır, kayıp dosya sayılmadı.

### Kareler zinciri — zaman sırası

Saatler durum çubuğundan; gün, dosya zamanıyla (+3 saat) eşleşir.

| Saat | Kare | Olay |
|---|---|---|
| 10 Eyl 11:08 | E0239, E0240 | Ortak Cuzdan Tutar 2.000 → fark diyaloğu; bugün tarihli Bakiye Farkı |
| 10 Eyl 11:11 | E0233 | Kart hesabı düzenleme (ad henüz "Kredi Kartı"); kesim/ödeme günü 1 |
| 10 Eyl 11:23–11:25 | E0230, E0236, E0237, E0235 | Ödemesiz Ağustos listesi; borç 1.200; ön dolu 1.200 ödeme; 44.950 çekirdek |
| 10 Eyl 11:26–11:30 | E0231, E0227, E0238, E0228, E0229, E0234, E0232 | İstatistik; boş Eylül; Tekrarlama listesi (Daha); dolu liste; kategori paneli; takvim; hesap toast'u |
| 10 Eyl 12:04–12:06 | E0241–E0244 | B1: Aylık 600 formu; Ağustos ve Eylül occurrence; Ekim önizleme |
| 10 Eyl 12:09–12:10 | E0245–E0247 | B2: 6.000/6 formu; (1/6) Ağustos; Bu Ay 1.000 / Gelecek Ay 1.000, Ana Hesap 39.600 |
| 10 Eyl 12:12 | E0248, E0249 | A: 400 kısmi ödeme; Bu Ay 600, Ana Hesap 39.200, net 41.750 |
| 12 Eyl 08:59–09:07 | E0252, E0253, E0254 | Toplama Dahil Et açık; kapalıyken 38.200 (Ana Hesap 39.800); ayarlar ızgarası |
| 12 Eyl 09:14 | E0255, E0256 | Kart defteri Ağustos ve Eylül |
| 12 Eyl 09:23–09:24 | E0250, E0251 | Toplam sekmesi (Gider 3.050); filtre paneli HESAP |

### Forma uygulanan düzeltmeler (25)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| MG01-1 | E0235, E0249, E0253, E0254 | Ekranda banner reklam; kalıcı banner alt kısmı kaplıyor, Korece reklam (2 yer) | Karelerde yalnız boş "Ad" yer tutucusu; İşlemler ekranlarında reklam alanı yok; Korece reklam koşum notu |
| MG01-2 | E0227 | K01 boş ana ekran kanıtı | Veri girildikten sonra boş Eylül ayı; ilk açılış karesi değil |
| MG01-3 | E0232 | K08 düzenleme/Sil/Kopya/Hızlı erişim kanıtı | Kare yalnız toast; düzenleme ve silme koşum notu |
| MG01-4 | E0234 | Takvimde hücre başına tek net sayı | Aynı günde iki yön yok; net/ayrı doğrulanamaz |
| MG01-5 | E0231, E0250 | İstatistik drill-down; kart satırı borcun yanında ödeneni veriyor | Drill-down karesi yok; 2.200 dönem kart harcaması (Çekme), borç 1.000; üst Gider 3.050 |
| MG01-6 | E0227, E0244 | İllüstrasyon ekrana göre değişiyor (2 yer) | Aynı Gün görünümünde iki farklı illüstrasyon; neye göre değiştiği bilinmiyor |
| MG01-7 | — | Sıfır tutar sessiz kabul, taslak uyarısı yok (arayüz taraması) | Koşum notu, karesi yok |
| MG01-8 | E0236, E0255 | Kart borcu negatif (₺ -1.200,00) gösteriliyor | Hesaplar'da işaretsiz kırmızı; eksi yalnız defter yürüyen bakiyesinde |
| MG01-9 | E0236, E0235 | 12 karesi hem borcu hem sıfırlanmayı gösteriyor | 1.200 ödeme öncesi `12`; sıfırlar `11` |
| MG01-10 | E0238 | Formda Tekrar/Taksit → Tekrarlama/Taksit menüsü, 14 karesi | Açılır menü karesi yok; liste Daha sekmesi altından açılmış, test öncesi |
| MG01-11 | E0241 | Kaydet'te tek onay diyaloğu | Diyalog karesi yok; tekrarlı formda Devam et yok |
| MG01-12 | E0250–E0253 | Ağu 10 silindi, Eyl 10 duruyor | Silme anı karede yok; 12 Eyl kareleri sonuçla tutarlı |
| MG01-13 | E0245 | Taksit "Aylık taksit (Ay sayısı)" alanı | Alan karesi yok; rozet var |
| MG01-14 | E0248, E0237 | Ön dolu 1.000 düzenlendi, 24 karesi | Karede yalnız 400; ön dolum E0237'deki 1.200'den çıkarılır |
| MG01-15 | E0252 | Tutar alanı = açılış bakiyesi | Alan güncel bakiyeyi gösterir; boş hesapta açılış işlevi görür |
| MG01-16 | — | Faz 7.5 girişinde 27 karenin tamamı | 12 Eyl kaydı; bugünkü 30 kareyle eşleştirilemiyor |
| MG01-17 | E0250, E0255 | Faz 7.5: borcun yanında ödenen, "ne borçlandın" | 2.200 = 1.200 + 1.000 taksit; ne harcadın / ne ödedin |
| MG01-18 | E0251 | Hata yalnız Türkçe çeviride | Çıkarım; aynı sürümün İngilizce karesi yok |
| MG01-19 | E0233, E0239, E0252 | Toplama Dahil Et varsayılan açık | Görülen her karede açık; kurulum varsayılanı doğrulanmadı |
| MG01-20 | E0253 | Açık/kapalı tablosunun kanıtı 29 | Kapalı satır kareli; açık satır aritmetik |
| MG01-21 | E0250 | Karar: `₺1.000,00(₺400,00)` borç ve ödeme | `₺2.200,00(₺1.200,00)` harcama ve ödeme; Eylül Toplam karesi yok |
| MG01-22 | E0251 | Karar: dört sütun dar ekranda sıkışıyor | İki sütunda ikişer satır; hücre başına iki sayı |
| MG01-23 | E0247, E0249, E0253 | B1/B2 sonrası Ana Hesap 39.200 | 39.600 (B1/B2) → 39.200 (A) → 39.800 (Ağu tekrarı silinmiş) |

Satırlardaki parantezli sayılarla birlikte toplam 25 yer; ayrıca "Kanıt ve güven
düzeyi"ne 14 Eylül görsel inceleme maddesi eklendi.

**Formu güçlendiren görsel gerçekleri:** 44.950 çekirdek kontrol (E0235) ve
ödeme öncesi/sonrası netin değişmemesi (E0236 → E0235); havale gün başlıkları
0/0 ve takvimde 0,00 (E0228/E0234); ön dolu ödemenin o anki borca eşitliği
(E0236 → E0237); Bakiye Farkı'nın feed'e girmemesi (E0240, E0243); tekrarın
geçmiş/bugün gerçek, gelecek önizleme olması (E0242–E0244); taksitin tek tek
ekstrelere dağılması (E0246/E0247/E0256); kısmi ödemenin yalnız Bu Ay'ı
düşürmesi (E0249); ters havale başlıkları (E0251); Toplama Dahil Et kapalıyken
grup 0 ve gri satır (E0253); ayarlar ızgarası ve sürüm (E0254); kart defterinin
üç okuması (E0255/E0256).

**Formda anılmayan yeni gözlemler:** kart hesabı önce "Kredi Kartı" adıyla
düzenlenmiş (E0233); tekrarlı formda Devam et yok (E0241); nakit hesabı
defterinde de "Faturalama donemi" etiketi (E0240); kart defterinde Bakiye
yardım ikonu (E0255); Ekim önizlemesi "Veri yok." ile aynı ekranda (E0244);
havale formunda kategori alanı ve Devam et yok (E0237/E0248); Tutar alanı
güncel bakiye (E0252).

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| MM-Q01 | Kapandı: 30 karenin tamamı Türkçe; `02`–`25` 10 Eyl, `26`–`32` 12 Eyl; eski İngilizce kare yok; `05` envanterde yok; "27 kare" tarihsel olarak işaretlendi | B17 mekanik kapısı (P1-K) |
| MM-Q02 | Kapandı: 44.950 (11:25) → 41.750 net / Ana 39.600 (12:10) → Ana 39.200 (12:12) → Ana 39.800, anahtar açık 42.350 (aritmetik), kapalı 38.200 (12 Eyl) | — |
| MM-Q03 | 10/12 Eylül bulgusu: 2/6 borçta ve listede; dört sonraki taksit iki dönem toplamında görünmüyordu. **20 Eylül K2: görünür yer sorusu kapandı**, Ekim–Ocak listelerinde bulundu (E0406–E0416) | Üretim eşiği ve fiziksel saklama açıklanmadı (B12); görünür yer yeniden koşulacak açık iş değildir |
| MM-Q04 | 2.200 kart harcamasıdır (1.200 + 1.000), borç değil; başlık tersliği Türkçe arayüzde kanıtlı, "yalnız Türkçe" çıkarım olarak işaretlendi | Aynı sürümün İngilizce karşılaştırması yapılmadı (karar engeli değil) |
| MM-Q05 | Kısmi ödemenin Bu Ay 1.000 → 600, Gelecek Ay 1.000 sabit sonucu kareli (E0247/E0249) | Tam ekstre eşdeğerliği ve D3 uyumu kod/kaynak kontrolü |
| MM-Q06 | Anahtar görülen her karede açık; varsayılan iddiası sınırlandırıldı; açık satır aritmetik | Kurulum varsayılanı; Bluecoins çaprazı ve BF toplam kapsamı (B10/B16, kod) |
| MM-Q07 | Formlarda kamera ikonu görünüyor; PC'den Yönet ve Yedekle kutuları var | Ek dosya/OCR yokluğu ve yerellik karelerde yok; kaynak kontrolü |
| MM-Q08 | Alan sayısı ve form ekranları kareli; adım sayısı ve erişilebilirlik karelerden ölçülmez | Metin/etiket kontrolü |
| MM-Q09 | E0232 yalnız toast; sıfır tutar, kalıcı silme ve taslak uyarısı koşum notu olarak işaretlendi; açılışın bugün tarihli olması kareli (E0240) | Sıfır tutar ve silme davranışı canlı test adayı olarak kalır |
| MM-Q10 | Dört görselin dosyası mevcut ve açıldı | Formdaki kısa kullanım ve tablo hizası P1-B18 |

### Canlı test adaylığı

MM-Q02 G paketiyle cevaplandı. Bu paketin yazıldığı tarihte MM-Q03/MM-Q09 adaydı;
20 Eylül K2, MM-Q03'ün görünür yer sorusunu kapattı. Üretim eşiği B12 sınırında kalır.
MM-Q09 için yeni koşum yapılmış sayılmaz; veri silinmez/sıfırlanmaz.

### Bu paket sonrası kapsam

- Görsel inceleme **145/360** (eski korpus 142/357 + GB-U01 3/3). Money Manager
  30/30; Hesap Defterim 45/45; KolayBi 39/39; Goodbudget 31/31. Canlı test 0.
- B01–B19: 8 kanıtla kapandı, 4 iddia kapsamı sınırlandı, 7 açık.
- Kaynak form düzeltildi; görseller, dosya adları, emülatör verisi ve ürün
  kodu değişmedi.
- Sıradaki tek paket **P1-parasut-G01** (E0258–E0266, 9 görsel); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-parasut-G01 — E0258–E0266 görsel içerik incelemesi

**14 Eylül 2026: 9 görselin 9'u açıldı, E0011 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-parasut-G01 bölümüne yazıldı.** 9 dosyanın SHA-256
değeri başlangıç kaydıyla aynı. Emülatör açılmadı, web/video kaynağı yeniden
açılmadı, hesap açılmadı. Görseller ve dosya adları değişmedi; çalışma
ağacındaki `01c-carousel.png` silinmesine dokunulmadı (envanterde yok).

### Kaynak katmanları

| Kareler | Kaynak | Tarih bilgisi ve sınır |
|---|---|---|
| E0258, E0259 | Emülatörde mobil giriş öncesi karusel, manuel gözlem | Durum çubuğu 1:18/1:19; 1 Eylül formun beyanı, dosya kopyası 7 Eylül. Sürüm karede yok |
| E0260–E0266 | "Paraşüt ile neler yapabilirsiniz?" tanıtım videosu (oynatıcı başlığı E0263'te) | Kullanıcı aktarımı 10 Eylül; 2022 işlem tarihleri kurgu verisi, yayın tarihi değil; URL yok |

### Forma uygulanan düzeltmeler (15)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| PG01-1 | E0258, E0259 | Sürüm 5.25.0 (Android paketi) | Sürüm karelerde görünmüyor; koşum notu |
| PG01-2 | E0258, E0259 | Kayıt yalnız web'de, VKN ve ücret gerekiyor | Web kayıt koşulları karede yok, bu tur doğrulanmadı |
| PG01-3 | E0260–E0266 | Kareler web/masaüstü sürümüne ait (pencere çubuğu) | Animasyonda pencere çerçevesi; istemci türü yazmıyor; E0263'te oynatıcı öğeleri |
| PG01-4 | E0261–E0266 | Tutarsızlıklar illüstrasyon olduğunun somut kanıtı | Kareler ürün çıktısı olarak okunamaz, illüstrasyon gibi ele alınır |
| PG01-5 | E0260 | Tahsil edilmemiş hâl: cari borç yazılmış | Cari borç karede görünmüyor, çıkarım |
| PG01-6 | E0260, E0261 | 03 aynı ekranın tahsil edilmiş hâli; aynı fatura iki durumda; TAHSİLAT EKLE pasif (3 yer) | İki ayrı örnek fatura (tarih/numara/tür/miktar farklı); düğme soluk görünümlü |
| PG01-7 | E0261 | CARİSİZ: cari bağlamadan fatura kesilebiliyor | Düğme görünüyor; anlamı çıkarım, karedeki faturada müşteri seçili |
| PG01-8 | E0262 | PLANLANMIŞ kovası = ileri tarihli/tekrarlayan | Kova içeriği karede yazmıyor, çıkarım |
| PG01-9 | E0264, E0265 | Hariç seçeneği çıkarım, ama 06'da VERGİLER HARİÇ geçiyor; Kategorisiz ayrı kategori (2 yer) | Stok sütunu bu açılırı kanıtlamaz; Kategorisiz boş daireli ayrı satır |
| PG01-10 | E0265, E0266 | 08 parayı kasaya girdiği gün, 07 tahakkuku sayıyor | 08 tahsilat/ödeme listeliyor; 07'nin sayım zamanı çıkarım |
| PG01-11 | E0260, E0261 | Karar: 02/03 aynı faturanın iki hâli yan yana | İki ayrı örnek faturada iki durum |
| PG01-12 | E0265, E0266 | Karar: iki zaman ekseni gerekçesi; Kategorisiz ayrı kategori (2 yer) | Zaman ekseni ekran adlarından çıkarım; Kategorisiz ayrı satır |

Parantezli sayılarla toplam 15 yer. Ayrıca "Sayılar tutmuyor" bölümüne E0262 ve
E0266 ek tutarsızlıkları, ekran tablosuna E0260 panel satırları, E0263 döviz/IBAN
ayrıntısı ve E0266 eksen ölçeği; "Kanıt ve güven düzeyi"ne üç çıkarım ve 14
Eylül görsel inceleme maddesi eklendi.

**Formu güçlendiren görsel gerçekleri:** giriş öncesi yüzeyde yalnız iki eylem
(E0258/E0259); 02/03'te Kalan ve Tahsil edildi durumlarının ayrı eylem olarak
görünmesi; formun dört aritmetik/biçim notunun ve beş aynı IBAN'ın yeniden
hesapla doğru çıkması (E0261–E0263, E0265/E0266); Kategorisiz'in iki listede de
görünmesi; NET kelimesi (E0265); nakit ve banka hesaplarının tek listede olması
(E0263).

**Formda anılmayan yeni gözlemler:** telefonda `E-Fatura`, masaüstü panelde
`e-Arşiv FATURA` (E0260); 02 panelinde İrsaliyeli Fatura / Müşteri Ekranı Açık /
Fatura Geçmişi; 03 ve 04'ün ikisinde de video başlığı "Cari Hesap Takibi";
04'te "yok" onaylarının tutarlı donut'larla birlikte durması; E0263'te döviz
kodu TRL ve kasa hesabında IBAN; E0266'da eksen ölçeği ve liste tarihlerinin
başlık aralığı dışında olması.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| PS-Q01 | Video başlığı oynatıcı öğesinde doğrulandı; 2022 tarihleri kurgu verisi olarak işaretlendi | Tam URL, yayın tarihi ve kılavuz sayfaları (kaynak işi) |
| PS-Q02 | Giriş öncesi iki eylem kareli; sürüm ve web kayıt koşulları karede yok, sınırlandırıldı | Web kayıt/VKN/ücret kaynağı |
| PS-Q03 | Kapandı: 02 ve 03 iki ayrı örnek fatura; aynı faturanın geçişi iddiası form ve karar tablosunda düzeltildi | — |
| PS-Q04 | Görsel katkı yok (mahsup ve kısmi ödeme karelerde yok) | Kaynak/kod (B01 kapalı; kıyas metni korunuyor) |
| PS-Q05 | PLANLANMIŞ ve nakit/tahakkuk ayrımı çıkarım olarak işaretlendi | Rapor sayım zamanının kılavuz kaynağı |
| PS-Q06 | CARİSİZ düğmesi görüldü, anlamı çıkarım; OCR karesi yok | OCR onay sınırı ve gider zorunlulukları (kaynak) |
| PS-Q07 | Vergiler dahil açılırında hariç seçeneği görünmüyor; E0264 sütunu kanıt olmaktan çıkarıldı; KDV raporuna ait kare yok | KDV kılavuzunun tam kaynağı ve alan anlamları |
| PS-Q08 | BANKA HESABI BAĞLA, müşteri hatırlatma ve tahsilat talep düğmeleri görüldü | Tetikleme/kurulum/rol koşulları (kaynak) |
| PS-Q09 | Görsel katkı yok | Kaynak/metin |
| PS-Q10 | Kapandı (görsel kısmı): CARİSİZ düğme, kırmızı satır ve Kategorisiz satırı veri modeli sayılmadan sınırlandırıldı | Gerçek görsel tasarım görülmedi sınırı korunur |
| PS-Q11 | Kapandı: dokuz görselin rolü atandı; E0257 ve 01c sayılmadı; aritmetik notlar yeniden hesaplandı, ek tutarsızlıklar eklendi | Kısa atıfların mekanik kontrolü B17/P1-K |
| PS-Q12 | Görsel katkı yok | BF kod/ADR kıyası |

### Bu paket sonrası kapsam

- Görsel inceleme **154/360** (eski korpus 151/357 + GB-U01 3/3). Paraşüt 9/9;
  Money Manager 30/30; Hesap Defterim 45/45; KolayBi 39/39; Goodbudget 31/31.
  Canlı test 0; Paraşüt'te canlı test adayı yoktur.
- B01–B19: 8 kanıtla kapandı, 4 iddia kapsamı sınırlandı, 7 açık.
- Kaynak form düzeltildi; görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-logo-isbasi-G01** (E0220–E0225, 6 görsel); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-logo-isbasi-G01 — E0220–E0225 görsel içerik incelemesi

**14 Eylül 2026: 6 görselin 6'sı açıldı, E0009 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-logo-isbasi-G01 bölümüne yazıldı.** 6 dosyanın
SHA-256 değeri başlangıç kaydıyla aynı. Emülatör açılmadı; kayıt, SMS veya
web/video erişimi yapılmadı. Görseller ve dosya adları değişmedi.

### Kaynak katmanları

| Kareler | Kaynak | Tarih bilgisi ve sınır |
|---|---|---|
| E0220–E0223 | Emülatörde giriş/kayıt yüzeyi, manuel gözlem | Durum çubuğu 1:27 → 1:29 → 1:29 → 1:30 (form akışıyla aynı sıra); 1 Eylül formun beyanı, dosya kopyası 7 Eylül. Sürüm karede yok |
| E0224, E0225 | Kullanıcıya 2 Eylül'de e-postayla gelen tanıtım videosu | Oynatıcı başlığı ve ilerleme çubuğu E0224'te; URL ve yayın tarihi yok. Pazarlama grafiği |

### Forma uygulanan düzeltmeler (10)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| LG01-1 | E0223 | Erişim kısıtı: sahte numara sessizce reddediliyor; hazırlık pop-up'ı (2 yer) | Ret anı koşum notu; pop-up kullanıcı aktarımı, karesi yok |
| LG01-2 | E0221 | Ürün tezi: firma bilgisi tanımlamadan fatura | Ürün beyanı; kayıtta Company name zorunlu, vergi profili bilinmiyor |
| LG01-3 | E0223, E0222 | Sahte numara denemesinin kanıtı `04-sozlesme.png` | Kare yalnız sözleşme adımı; spinner/geri dönüş koşum notu |
| LG01-4 | — | Hata geri bildirimi: mesaj vermeden giriş ekranına atıyor | Koşum notu, karesi yok |
| LG01-5 | E0225 | Müşavir Portal ayrı bir web arayüzü; düzenle/detay/sil ikonları | Stilize pencere; web yazmıyor; ikon anlamları çıkarım |
| LG01-6 | E0225 | Her satırda farklı firma ikonu | Farklı logo simgesi; firma olduğu çıkarım |
| LG01-7 | E0224 | Kaynak: e-postayla gelen video, 2 kare | Oynatıcı öğeleri görünüyor; URL/yayın tarihi yok |
| LG01-8 | E0225 | Video tablosu: web arayüzü, firma ikonu | Stilize arayüz, dört satır, logo simgesi |
| LG01-9 | E0220–E0223 | Kanıt: sahte numarayla kayıt reddedildi | Ret anı karede yok, koşum notu |

Parantezli sayılarla toplam 10 yer. Ayrıca kayıt formu satırına onay kutusunun
işaretli olduğu ve "Kanıt ve güven düzeyi"ne 14 Eylül inceleme notu eklendi.

**Formu güçlendiren görsel gerçekleri:** giriş ekranının alanları, SSO
düğmeleri ve Register baskınlığı (E0220); yalnız üç zorunlu alan ve VKN/TCKN
yokluğu (E0221); sektör listesinin görünen on seçeneği ve kaydırma çubuğu
(E0222); sözleşme başlığı ve CANCEL / I AGREE (E0223); İngilizce etiket ile
Türkçe içeriğin aynı akışta durması (E0221–E0223); sekiz entegrasyon adı
(E0224); Müşavir Portal metni ve boş şablon satırları (E0225).

**Formda anılmayan yeni gözlemler:** onay kutusu kayıt formunda işaretli
(E0221); E0222'deki klavye önerisi "Tasarim" deneme adıyla tutarlı; sözleşmede
I AGREE mavi, uygulamanın kırmızısı değil (E0223); E0224'te imleç Banka Hesap
Hareketleri üzerinde; E0225'te dört satır ve fotoğraflı profil simgesi.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| LI-Q01 | Video karelerinin oynatıcıdan alındığı görüldü; URL/yayın tarihi yok | Tam kaynak, yayın tarihi, paket sınırı (kaynak işi) |
| LI-Q02 | Company name zorunluluğu tezi sınırladı; sözleşme karesi spinner/ret kanıtı olmaktan çıkarıldı; hazırlık mesajı kullanıcı aktarımı olarak işaretlendi | Hazırlık mesajının anlamı ve vergi profili (kaynak) |
| LI-Q03 | Görsel katkı yok | Sesli fatura kaynağı |
| LI-Q04 | Dört satır, logo simgeleri ve işlem ikonları kaydedildi; web arayüzü ve firma ikonu ifadeleri çıkarım olarak düzeltildi | Müşteri adına işlem yetkisinin kapsamı (kaynak) |
| LI-Q05 | E0224 yalnız "Akıllı Fiş Okuma" adını gösteriyor; form bu sınırı zaten koruyor | OCR onay sınırı (kaynak) |
| LI-Q06 | E0224 yalnız POS ve banka hareketleri adlarını gösteriyor | İşlev ve mutabakat kıyası (kaynak/kod) |
| LI-Q07 | Görsel katkı yok | Kaynak/kod |
| LI-Q08 | Görünen on seçenek birebir; listenin devam ettiği doğrulandı | Listenin tamamı ve konum/kullanım ayrımı (kaynak) |
| LI-Q09 | Kapandı (görsel kısmı): alan, ikon, dil ve hiyerarşi kareyle kayıtlı; SSO yalnız düğme; sessiz hata ve hazırlık mesajı koşum notu; sektör sınırı açık yazıldı | Kısa atıfların mekanik kontrolü B17/P1-K |
| LI-Q10 | Görsel katkı yok | BF kod/ADR kıyası |

### Bu paket sonrası kapsam

- Görsel inceleme **160/360** (eski korpus 157/357 + GB-U01 3/3). Logo İşbaşı
  6/6; Paraşüt 9/9; Money Manager 30/30; Hesap Defterim 45/45; KolayBi 39/39;
  Goodbudget 31/31. Canlı test 0; Logo İşbaşı'nda canlı test adayı yoktur.
- B01–B19: 8 kanıtla kapandı, 4 iddia kapsamı sınırlandı, 7 açık.
- Kaynak form düzeltildi; görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-quickbooks-G01** (E0268–E0271, 4 görsel); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-quickbooks-G01 — E0268–E0271 görsel içerik incelemesi

**14 Eylül 2026: 4 görselin 4'ü açıldı, E0012 metniyle karşılaştırıldı ve
sonucu görsel başına E0391 P1-quickbooks-G01 bölümüne yazıldı.** 4 dosyanın
SHA-256 değeri başlangıç kaydıyla aynı. Emülatör açılmadı; hesap, deneme,
ödeme veya yardım merkezi erişimi yapılmadı. Görseller ve dosya adları
değişmedi. Bu paket B13'ü kapatmaz; Solopreneur/Simple Start yayılımı P1-B13'ün
işidir ve bu pakette o satırlara (iş modeli hücresi, K00 plan kapısı başlığı)
dokunulmadı.

### Kaynak katmanları

| Kareler | Kaynak | Tarih bilgisi ve sınır |
|---|---|---|
| E0268 | Emülatörde QuickBooks mobil karuseli, manuel gözlem | Durum çubuğu 1:34; 1 Eylül formun beyanı. Ürün adı ve sürüm karede yok |
| E0269–E0271 | Hesap açıldıktan sonraki onboarding ve Simple Start plan ekranı, manuel gözlem | Durum çubuğu 7:32–7:33; 2 Eylül formun beyanı. Hesap açma adımları, deneme yolu ve kart istemi karede yok |

### Forma uygulanan düzeltmeler (9)

| No | Kanıt | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| QG01-1 | E0268–E0271 | Sürüm Mobil 30.4.7 (React Native) | Sürüm karelerde görünmüyor; koşum notu |
| QG01-2 | E0269 | Hesap açma (e-posta + SMS) kanıtı `02` | Kare yalnız hoş geldin ekranı; hesap açma adımları koşum notu |
| QG01-3 | E0270 | "Deniz Tasarim" → Next | Alan karede boş; giriş koşum notu |
| QG01-4 | E0271 | Gerilim: 04'te görünen tek yol abonelik kartı | Deneme ifadesi üst bantta var; ayrı deneme düğmesi yok, Get started'ın ne başlattığı belli değil |
| QG01-5 | E0268–E0271 | Onboarding tonu: 3 kısa slayt, çok düşük sürtünme, sert ödeme duvarı | Bir slayt görüldü; sürtünme ve duvar yorumdur |
| QG01-6 | E0270 | Tek zorunlu alan Business name | Tek alan; zorunluluk işareti karede yok |
| QG01-7 | E0271 | Kampanya bitiminde fiyat 3,3 katına çıkacak | Oran ≈ 3,35; kampanya sonrası fiyat çıkarım, koşul bağlantısı açılmadı |
| QG01-8 | E0271 | Akış özeti: ödeme duvarı, ürün denenmeden karar | Plan ekranı; deneme yolu ve kart istemi görülmedi |
| QG01-9 | E0269, E0271 | Karar: 04'te ücretsiz yol görünmüyor, açı güveni zedeliyor | Deneme ifadesi var, ayrı düğme yok; güven etkisi yorum |

Toplam 9 yer. Ayrıca K00 karusel satırına ürün adının karede olmadığı, basic
info satırına alt metin ve "Kanıt ve güven düzeyi"ne saat/oturum notu eklendi.

**Formu güçlendiren görsel gerçekleri:** karusel metni ve iki eylem (E0268);
hoş geldin ekranının üç maddesi birebir (E0269); tek alan ve "No business
name? Use your name" (E0270); Simple Start kartı, iki fiyat, "for 6 months",
beş özellik ve alt bağlantılar birebir; plan listesinin karede olmaması
(E0271). Formun kanıt tavanındaki "bu kareler Solopreneur arayüzü değildir"
hükmü dört karenin hiçbirinde Solopreneur adı geçmemesiyle destekleniyor.

**Formda anılmayan yeni gözlemler:** karuselde ürün adı yok (E0268); basic
info alt metni "We'll use this to get you started in QuickBooks." (E0270);
özelliklerin her birinde ⓘ bilgi simgesi (E0271); E0268 ile E0269–E0271
arasında iki ayrı oturum saati.

### Bağlı soruların ilerlemesi

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| QB-Q01 | Karelerde Solopreneur adı yok: E0268 adsız, E0269/E0270 "QuickBooks", E0271 "Simple Start" | Formdaki Solopreneur/Simple Start yayılımı (P1-B13) |
| QB-Q02 | Sürüm hiçbir karede yok; TRY gösterimi E0271'de | Paket kimliği, bölge kısıtının kaynağı |
| QB-Q03 | Kapandı: deneme metni E0269 ve E0271 üst bandında var; ayrı deneme düğmesi yok; tek eylem Get started; kart istemi kullanıcı notu. Gerilim paragrafı, akış özeti ve karar satırı düzeltildi | — |
| QB-Q04 | Görsel katkı yok | Yardım merkezi kaynakları |
| QB-Q05 | Kapandı (görsel kısmı): nokta sayısı, alan ve metinler gözlem; düşük sürtünme ve sert duvar yorum; 3,35 oranı hesaplandı, kampanya sonrası fiyat çıkarım | Kullanılabilirlik ölçümü yok (karar engeli değil) |
| QB-Q06 | Type/Category hiyerarşisinin karesi yok (form zaten resmî kaynak diyor) | Kaynak cümleleri |
| QB-Q07 | Görsel katkı yok | Kaynak |
| QB-Q08 | E0271 özellik listesi Simple Start'a ait; Solopreneur özelliği sayılmadı | Paket karşılaştırma kaynağı (P1-B13/kaynak) |
| QB-Q09–QB-Q11 | Görsel katkı yok | Metin/kod/sözlük kontrolü |
| QB-Q12 | Kapandı (görsel kısmı): dört kısa atıf tam kimlikle eşleşti; E0267 sayılmadı; saatler kaydedildi | Faz 7.5 beyanının kapsamı ve B17 mekanik kapısı |

### Bu paket sonrası kapsam

- Görsel inceleme **164/360** (eski korpus 161/357 + GB-U01 3/3). QuickBooks
  4/4; Logo İşbaşı 6/6; Paraşüt 9/9; Money Manager 30/30; Hesap Defterim 45/45;
  KolayBi 39/39; Goodbudget 31/31. Canlı test 0; QuickBooks'ta canlı test adayı yoktur.
- B01–B19: 8 kanıtla kapandı, 4 iddia kapsamı sınırlandı, 7 açık. B13 açık.
- Kaynak form düzeltildi; görseller, dosya adları ve ürün kodu değişmedi.
- P1 masa başı G paketleri (MM, PS, LI, QB) tamam. Sıradaki tek paket
  **P1-B13** (QuickBooks ürün/paket ayrımı yayılımı; QB-Q01/Q03); bu tur
  başlatılmadı. Faz 8 açılmadı.

## P1-B13 — QuickBooks ürün/paket ayrımı

**14 Eylül 2026: B13 kanıtla kapandı.** Dayanak P1-quickbooks-G01 görsel
incelemesidir (4/4). Yeni yardım merkezi erişimi, hesap, deneme veya ödeme
işlemi yok; görseller değişmedi.

### Üç bağlam

| Bağlam | Kanıt | Bu belgelerde nasıl yazılır |
|---|---|---|
| QuickBooks mobil onboarding'i | E0268–E0270 manuel kareler; ürün adı karuselde yok, sonraki iki ekranda "QuickBooks" | Onboarding akışı, hesap açma sonrası ekranlar |
| QBO Simple Start plan ekranı | E0271 manuel kare | Fiyat (TRY 819,99 / 244,99 "for 6 months"), deneme ifadesi, beş özellik; hepsi Simple Start'a ait |
| QuickBooks Solopreneur | Intuit yardım merkezi/ürün sayfası beyanı; ekran yok | Type Business/Personal, Split, Rules, Schedule C; fiyat, deneme koşulu ve onboarding doğrulanamadı |

### Düzeltilen yerler (15)

| Dosya | Konum | Önceki ifade | Düzeltilmiş sınır |
|---|---|---|---|
| E0012 QuickBooks | Oturum bilgisi, erişim kısıtı | Plan ekranında durdu; deneme kart istiyor | QBO Simple Start plan ekranı; deneme notu Simple Start'a ait, Solopreneur ayrıca doğrulanmadı |
| E0012 QuickBooks | Ürün kimliği, iş modeli | Solopreneur satırında Simple Start fiyatı | Solopreneur fiyatı doğrulanmadı; karedeki fiyat QBO Simple Start kartı |
| E0012 QuickBooks | K00 ilk açılış | Onboarding karuseli | QuickBooks mobil uygulamasının karuseli |
| E0012 QuickBooks | K00 plan kapısı | Plan kapısı, ekran başlığı Simple Start | QBO Simple Start; Solopreneur plan ekranı değil |
| E0012 QuickBooks | Vaat/kapı paragrafı | Başlıksız gerilim | QuickBooks mobil → QBO Simple Start; Solopreneur deneme koşulu değil |
| E0012 QuickBooks | Arayüz, onboarding tonu | Ürün adı yok | QuickBooks mobil → QBO Simple Start |
| E0012 QuickBooks | Arayüz, fiyat sunumu | Ürün adı yok | QBO Simple Start kartı; Solopreneur fiyatı değil |
| E0012 QuickBooks | Akış özeti, en fazla sürtünme | Onboarding ardından plan ekranı | QuickBooks mobil → QBO Simple Start; Solopreneur akışı görülmedi |
| E0012 QuickBooks | Karar: fiyat/deneme sunumu | Ürün adsız kampanya fiyatı satırı | QBO Simple Start plan kartı; Solopreneur fiyat sunumu değil |
| E0012 QuickBooks | Kanıt: doğrulanamadı | Ücretsiz denemenin kart istediği | Simple Start denemesi; Solopreneur fiyatı/deneme/onboarding ayrıca eklendi |
| kanitlar/README.md (E0272) | Uygulama klasörleri | `quickbooks/` = QuickBooks Solopreneur, Resmî kaynak | QuickBooks mobil onboarding + QBO Simple Start; manuel gözlem (4 kare), Solopreneur bulguları resmî kaynak |
| DURUM.md | Tur 1 durum tablosu | 4 (onboarding→paywall) | QuickBooks mobil onboarding → QBO Simple Start; Solopreneur ekranı yok |
| MANUEL-TEST-PROTOKOLU.md | İncelenen uygulamalar | QuickBooks Solopreneur satırı bağlamsız | Karelerin QuickBooks mobil + QBO Simple Start olduğu yazıldı |
| README.md | Seçim sonrası not | Resmî yardım sayfaları ve ürün videoları | Solopreneur yardım sayfaları; manuel kareler QuickBooks mobil + Simple Start; formda dayanağı olmayan "ürün videoları" kaldırıldı |
| README.md | İnceleme türü dağılımı | QuickBooks Online/Solopreneur — kayıt web'de | QuickBooks Solopreneur; kareler Simple Start; kayıt/ödeme/bölge kapıları |

### Bağlı sorular

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| QB-Q01 | Kapandı: 13 Eylül listesindeki beş form yeri ve E0272 klasör etiketi ürün/paket adıyla işaretlendi; özetlere yayıldı | — |
| QB-Q03 | P1-quickbooks-G01'de kapanmıştı; bu pakette deneme ifadesinin Simple Start'a ait olduğu eklendi | — |
| QB-Q08 | E0271 özellik listesi Simple Start'a bağlandı | Solopreneur/Simple Start paket karşılaştırmasının kaynağı |

### Yayılım kontrolü

Araştırmada "Solopreneur", "Simple Start", "244,99", "819,99", "QBS" ve "try
free" kalıpları tarandı. Kalan eşleşmeler: Solopreneur yardım merkezi
iddiaları (doğru bağlamda), düzeltilmiş cümleler, bulgu/envanter kayıtları,
13 Eylül denetim raporu ve DURUM'daki 12 Eylül / P0.3 paket kayıtları
(tarihsel girdi, değiştirilmedi) ve README'nin ilk seçim tablosundaki
"QuickBooks Online / Solopreneur" aday adı (seçim anının kaydı). README karar
filtresi, PRD ve ADR'ler değişmedi.

B13 dışında kalan benzer etiket sorunu: `kanitlar/README.md`'de `parasut/` ve
`logo-isbasi/` klasörleri de "Resmî kaynak" yazıyor, ama ikisinde de manuel
giriş/kayıt kareleri var. Bu pakette değiştirilmedi; B17/P1-K mekanik kapısına
not olarak bırakıldı.

### Bu paket sonrası kapsam

- B01–B19: **9 kanıtla kapandı**, 4 iddia kapsamı sınırlandı (B06, B08, B11, B12), **6 açık**.
- Görsel inceleme 164/360; canlı test 0. Görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-B18** (Bluecoins, Money Manager ve Wallet formlarındaki
  tablo hizası; BC-Q11, MM-Q10, WL-Q12); bu tur başlatılmadı. Faz 8 açılmadı.

## P1-B18 — Üç arayüz taraması tablosunun kanıt sütunu

**14 Eylül 2026: B18 kanıtla kapandı.** Kapanış mekanik kontroldür: 13 Eylül
denetim betiğinin hücre sayma kuralı (kaçışlı `|` hariç, baş/son ayraç
atılır) bir yardımcı betikle bütün gözlem formlarına yeniden uygulandı.
Görsel, emülatör veya dış kaynak açılmadı.

### Ölçüm

| Form | Tablo | Başlık/ayraç | Veri satırı | Düzeltme öncesi | Düzeltme sonrası |
|---|---|---:|---:|---|---|
| E0005 Bluecoins | Arayüz taraması (satır 56) | 3 sütun | 8 × 4 hücre | 8 uyumsuz | 0 |
| E0010 Money Manager | Arayüz taraması (satır 52) | 3 sütun | 8 × 4 hücre | 8 uyumsuz | 0 |
| E0014 Wallet | Arayüz taraması (satır 52) | 3 sütun | 8 × 4 hücre | 8 uyumsuz | 0 |

Diğer gözlem formlarında uyumsuz tablo satırı yoktu. Üç tabloda da dördüncü
hücreler zaten kanıt dosya adı veya `—` taşıyordu; başlık `Kanıt` oldu, ayraç
dört sütuna çıktı, veri hücreleri değişmedi.

### Düzeltilen yerler (7)

| Dosya | Konum | Önceki | Sonraki |
|---|---|---|---|
| E0005 Bluecoins | Arayüz taraması başlık + ayraç | 3 sütun | `Alan / Gezildi mi / Kısa gözlem / Kanıt` |
| E0010 Money Manager | Arayüz taraması başlık + ayraç | 3 sütun | `Alan / Gezildi / Kısa gözlem / Kanıt` |
| E0014 Wallet | Arayüz taraması başlık + ayraç | 3 sütun | `Alan / Gezildi mi / Kısa gözlem / Kanıt` |
| E0010 Money Manager | Tekrarlayan üretim tablosu, 10 Ağu | `18` | `18-tekrarlayan-agustos-liste.png` |
| E0010 Money Manager | Tekrarlayan üretim tablosu, 10 Eyl | `19` | `19-tekrarlayan-eylul-otomatik.png` |
| E0010 Money Manager | Taksit davranışı | `22` | `22-taksit-1-6-agustos.png` |
| E0010 Money Manager | Kısmi kart ödemesi | `25` | `25-kismi-odeme-sonrasi-borc.png` |

### Bağlı sorular

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| BC-Q11 | Kapandı: Bluecoins tablosunun kanıt sütunu başlıklı ve görünür | — |
| MM-Q10 | Kapandı: tablo hizalandı; E0242/E0243/E0246/E0249 formda en az bir kez tam adla anılıyor, diğer kısa kullanımlar korundu | Korpus genelinde kısa ad → tam kimlik borcu B17/P1-K |
| WL-Q12 | Kapandı: Wallet tablosunun kanıt sütunu başlıklı ve görünür | — |

### Bu paket sonrası kapsam

- B01–B19: **10 kanıtla kapandı**, 4 iddia kapsamı sınırlandı (B06, B08, B11, B12), **5 açık** (B09, B10, B16, B17, B19).
- Görsel inceleme 164/360; canlı test 0. Görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P1-K** (B17 mekanik kanıt kapısı: tam ad izlenebilirliği,
  bağlantılar ve tablolar; pozitif/negatif testlerle); bu tur başlatılmadı.
  Faz 8 açılmadı.

## P1-K — Mekanik kanıt kapısı

**14 Eylül 2026: B17 kanıtla kapandı.** Kapanış ölçütü B17 kaydındaki gibi
pozitif/negatif testlerle doğrulanmış bir kapı ve gerçek hatada sıfırdan farklı
çıkıştır. Görsel, emülatör veya dış kaynak açılmadı; ürün kodu değişmedi.

### Taban: eski `denetim.sh`

| Koşum | Çıktı | Çıkış kodu |
|---|---|---:|
| Dokuz uygulama | Wallet 5, Bluecoins 4 sorun; diğer yedisi "temiz" | 0 |
| Olmayan form (`audit-nonexistent`) | "atlandı" + "Denetim temiz" | 0 |
| P1 formlarındaki en tireli aralıklar | Hiçbiri bulunmadı (Money Manager ve KolayBi "temiz") | — |

Kod okumasıyla doğrulanan ek sınırlar 13 Eylül raporundakilerle aynı: yollu
atıf görülmüyor, `f7-53` kimliği `f7`'ye kırpılıyor, tam ad gövdesinin herhangi
bir yerde geçmesi yeterli sayılıyor.

### Yeni kapı

| Dosya | Rol |
|---|---|
| `denetim.cjs` (E0393) | Kuralların tamamı; `denetle()` dışa açık, CLI çıkış kodu 0/1/2 |
| `denetim-test.cjs` (E0394) | Geçici dizinde sahte form/kare kuran 15 test; araştırma dosyalarına dokunmaz |
| `denetim.sh` (E0002) | `node denetim.cjs` çağıran sarmalayıcı; protokoldeki komut korunur |

| Kural | Sınıf | Eski betik | Yeni kapı |
|---|---|---|---|
| Olmayan form | Kullanım hatası (2) | Atlıyor, temiz | Çıkış 2 |
| Ölü atıf | Hata | Yalnız tek tırnaklı düz ad | Tırnaklı/tırnaksız ve yollu (`../kanitlar/...`) atıf |
| Yetim kare | Hata | Gövde metnin herhangi bir yerinde geçerse geçer | Tam dosya adı sınırıyla; yalnız kısa kodla anılan kare de yetim |
| Belirsiz aralık | Hata | Yerel ayarda en tireyi kaçırıyor | `NN`, `dNN`, `f7-NN`; tire, en tire, em tire |
| Yasak kalıp | Hata | Kod içindeki alıntıyı da sayıyor | Kod parçası dışındaki metin |
| Tablo sütunu | Hata | Yok | Başlık/ayraçla hücre sayısı (kaçışlı boru hariç) |
| Kısa kod (tam ad başka yerde var) | Uyarı; `--siki` ile hata | Tam ad varsa sessiz | Satır numarasıyla raporlanır |
| Karşılığı olmayan kısa kod | Uyarı | Yok | Raporlanır (ör. Money Manager `05`) |
| Kural örneği | Kapsam dışı | Örnekleri gerçek kullanım sayıyor | Çift ters tırnaklı kod ve çitli blok atlanır |

### Testler — 15/15 geçti

Temiz form (tam ad, açık liste, sıralı akış, tablo) · olmayan form çıkış 2 ·
ölü tam ad · geçerli ve ölü yollu atıf · yetim kare · yalnız kısa kodla anılan
kare · kısa kod uyarısı ve `--siki` hatası · `f7-53` kimliğinin kırpılmaması ·
`101-a.png`'nin `01-a.png` sayılmaması · üç tire türüyle üç aralık biçimi ·
çift ters tırnak ve çitli blok örnekleri · yasak kalıp metinde/alıntıda ·
uyumsuz tablo ve kaçışlı boru · karşılığı olmayan kısa kod · CLI çıkış kodları
0/1/2 ve bilinmeyen bayrak.

### Gerçek korpus

| Uygulama | Kare | Düzeltme öncesi hata | Düzeltme sonrası hata | Uyarı | `--siki` hata |
|---|---:|---:|---:|---:|---:|
| Money Manager | 30 | 11 aralık | 0 | 19 | 18 |
| Hesap Defterim | 45 | 4 aralık | 0 | 6 | 6 |
| Goodbudget | 31 | 3 aralık | 0 | 1 | 1 |
| Paraşüt | 9 | 0 | 0 | 9 | 9 |
| Logo İşbaşı | 6 | 1 aralık | 0 | 4 | 4 |
| KolayBi | 39 | 2 aralık | 0 | 9 | 9 |
| QuickBooks | 4 | 1 aralık | 0 | 2 | 2 |
| Bluecoins | 90 | 80 yetim + 3 aralık | değişmedi (P2) | 0 | — |
| Wallet | 106 | 97 yetim + 3 aralık | değişmedi (P3) | 0 | — |

P1'in yedi formu birlikte: 0 hata, 50 uyarı, çıkış 0. Dokuz uygulama birlikte:
183 hata, çıkış 1. 13 Eylül cjs ölçümü yeniden koşuldu; PNG 360, bozuk 0, kopya
0, ölü aday 0, tablo uyumsuzluğu 0; tam adsız kare yalnız Bluecoins 80 ve
Wallet 97.

### Forma uygulanan düzeltmeler (25)

| Form | Yer | Önceki | Sonraki |
|---|---|---|---|
| E0010 Money Manager | Oturum bilgisi | `02`–`13` eski kareler | Numara düz metin; eski kareler diskte yok |
| E0010 Money Manager | Kanıt: Faz 1 | `02`–`17` | E0227–E0241 |
| E0010 Money Manager | Kanıt: Faz 1 ek (4 aralık) | `17`–`20`, `21`–`23`, `24`–`25`, `02`–`25` | Dokuz karenin açık tam ad listesi; E0227–E0249 |
| E0010 Money Manager | Kanıt: veri sapma notu (3 aralık) | `24`–`25`, `17`–`20`, `21`–`23` | E0248–E0249, E0241–E0244, E0245–E0247 |
| E0010 Money Manager | Kanıt: 14 Eylül inceleme (2 aralık) | `02`–`25`, `26`–`32` | E0227–E0249, E0250–E0256 |
| E0007 Hesap Defterim | Ek koşum 2 başlığı | `36`–`43` | E0171–E0178 |
| E0007 Hesap Defterim | Kategori anahtarı sapması | `16`–`28b` arasındaki on kare | Alanı gösterdiği G02'de doğrulanan dört karenin tam adı; "on kare" sayısı kaldırıldı |
| E0007 Hesap Defterim | Kanıt: Tur 1 | `00`–`19` | E0134–E0154 |
| E0007 Hesap Defterim | Kanıt: Faz 7.5 | `36`–`43`, `09` | E0171–E0178, `09-ozgun-ozellik-takvim.png` |
| E0006 Goodbudget | Faz 7.5 başlığı | `23`–`26` | E0130–E0133 |
| E0006 Goodbudget | Kanıt: 11 Eylül | `01`–`22` | E0106–E0129 (02b/02c dahil) |
| E0006 Goodbudget | Kanıt: 12 Eylül | `23`–`26` | E0130–E0133 |
| E0009 Logo İşbaşı | İnceleme türü | `01`–`04` | Dört karenin tam adı |
| E0008 KolayBi | Eski/yeni eşleşme girişi (2 aralık) | `d33`–`d38`, `d01`–`d31` | E0181–E0186, E0188–E0218 (d32/d39 E0180/E0187) |
| E0012 QuickBooks | Kanıt: saat notu | `01`, `02`–`04` | `01-onboarding.png`, E0269–E0271 |

Parantezli sayılar aynı satırda birden çok aralığı gösterir; toplam 22 aralık
yeri, 2 kısa kod → tam ad dönüşümü (Hesap Defterim `09`, QuickBooks `01`) ve
Hesap Defterim'in sayı düzeltmesi dahil 25 yer. E kimliği aralıkları yalnız kapsam bildirir (bir oturumun/koşumun
kareleri); bir iddiayı destekleyen aralıklar açık tam ad listesine çevrildi.

### Diğer belgeler (7)

| Dosya | Konum | Değişiklik |
|---|---|---|
| MANUEL-TEST-PROTOKOLU.md | Yazım kuralları girişi | Yeni kurallar, çıkış kodları, `--siki`, örnek istisnası, test komutu |
| MANUEL-TEST-PROTOKOLU.md | Kural 2 notu | 235 kare notuna P1-K sonucu ve kalan Bluecoins/Wallet borcu |
| DURUM.md | Uygulama başına tek geçiş | 12 Eylül sayılarının eski betiğin tarihsel çıktısı olduğu |
| E0008 KolayBi | Kanıt atıfı borcu bölümü | "denetim.sh düzeltilmeli" cümlesine kapanış notu |
| kanitlar/README.md (E0272) | Klasör tablosu (3) | Paraşüt, Logo İşbaşı ve KolayBi manuel gözlem + resmî kaynak olarak ayrıldı (P1-B13 notu) |

### Bağlı sorular

| Soru | Bu paketin katkısı | Açık kalan |
|---|---|---|
| HD-Q01, MM-Q01, MM-Q10, PS-Q11, LI-Q09, QB-Q12, KB-Q16, GB-Q10 | Mekanik kapı kısmı kapandı: ilgili form yeni kapıdan 0 hatayla geçiyor | İçerik soruları kendi kayıtlarında |
| BC-Q01, BC-Q10 | Kapı Bluecoins borcunu ölçüyor: 80 tam adsız kare, 3 aralık | P2 G paketleri |
| WL-Q01, WL-Q11 | Kapı Wallet borcunu ölçüyor: 97 tam adsız kare, 3 aralık | P3 G paketleri |

### Açık karar

Protokol kural 2 tek başına kısa kodu (`` `04` ``) da yasak sayıyor; eski betik
tam adı başka yerde geçen kareyi sessiz geçiyordu. Kapı bu kullanımı uyarı
olarak raporluyor ve `--siki` ile hataya çeviriyor. P1 formlarında 50 kısa kod
uyarısı var. Bunların Belge 1/2 yazımından önce tam ada çevrilip çevrilmeyeceği
(ve varsayılanın `--siki` olması) kullanıcı kararıdır; bu pakette değiştirilmedi.

### Bu paket sonrası kapsam

- B01–B19: **11 kanıtla kapandı**, 4 iddia kapsamı sınırlandı (B06, B08, B11, B12), **4 açık** (B09, B10, B16, B19).
- P1 paketleri tamam; kalan açık bulgular P2/P3 (B09, B10, B16) ve P4 (B19) işidir.
- Görsel inceleme 164/360; canlı test 0. Görseller, dosya adları ve ürün kodu değişmedi.
- Sıradaki tek paket **P2-G01** (Bluecoins E0016–E0032, 17 görsel; Tur 1 ve Faz 3
  kurulum/çekirdek kontrol); bu tur başlatılmadı. Faz 8 açılmadı.


## P2-G01 — Bluecoins Tur 1 ve Faz 3 çekirdek görsel kontrolü — 14 Eylül 2026

Durum: kapandı. Hedef E0016–E0032, 17 PNG; her dosya ayrı açılır.
Kaynak PNG'ler değiştirilmez; canlı uygulama koşumu yapılmaz.
Tur 1 / Faz 3 tarihleri önceki koşum kaydına aittir; görüntü tek başına cihaz,
kurulum sürümü veya eylemin gerçekleştiği tarihi kanıtlamaz.
Kaynak türü: yerel ekran görüntüsü / önceki manuel emülatör koşumu.
E0016 mağaza yüzeyidir; dış kaynak URL'si ve yayın doğrulaması yapılmadı.
E0016–E0025 yakalama tarihi koşum kaydında 1 Eylül; E0026–E0032 10 Eylül 2026.
İşlem dönemleri satırlarda belirtilir; formlar/ayarlar için dönem uygulanmaz.
Ürün ücretsiz Android koşumu olarak kayıtlı; fiziksel cihaz/sürüm yeniden
doğrulanmadı. Her satır E0005 gözlem formunu düzeltir; BC soru bağları aşağıdadır.

| Kanıt | Görünen içerik | İddia kontrolü / sınır | Kullanım rolü |
|---|---|---|---|
| E0016 — 00-magaza.png | Bluecoins Finance & Budget, Mabuhay Software; İngilizce Google Play; v13.1 yenilik metni, 1 Eylül 2026 güncelleme tarihi | Ürün/geliştirici doğrulanır; 13.1.45 ve versionCode bu karede yok. Mağaza dili uygulama dilinden ayrılır | Kanıt eki — ürün kimliği |
| E0017 — 01-ilk-acilis.png | Türkçe karşılama, Hadi Başlayalım ve Turkish (Türkçe) | İlk İşlemi Ekle / Demo Dosyasını Dene burada yok; K00'ın bu atfı daraltılmalı | Ana anlatım — karşılama |
| E0018 — 02-bos-ana-ekran.png | Hesaplar seçili; İşlemler, Hatırlatıcılar ve kısmen görünen sonraki sekme; Günlük Özet/Bütçe Özeti boş, Eylül 2026 takvimi | Boş kartlar ve yatay gezinme doğrulanır; ilk işlem/demo çağrısı bu karede yok | Ana anlatım — boş kartlar |
| E0019 — 03-dolu-ana-ekran.png | Ekle üstünde hesap seçici; Arama/Yeni; Ana Hesap 40.800, Ortak Cuzdan 4.150, Is Karti 0; sıfır bakiyeli diğer hesaplar | Dosya adı yanıltıcı: dolu ana ekran değil hesap seçici. Açılış tarihi/bakiyesi formu ve bütün tür ağacı görünmüyor | Ana anlatım — hesap seçimi |
| E0020 — 04-islem-formu.png | 3 Ağustos 2026, Web tasarim hizmeti geliri, +25000 TRY; Diğer/Diğer; Banka/Ana Hesap; Planlı İşlemler, Bölmek, Durum, Etiket, Not; GELİR seçili | Yoğun gelir formu doğrulanır; varsayılan GİDER ve kaydetme sonucu bu kareden çıkmaz. Ayrı kapsam alanı görünmüyor | Ana anlatım — gelir formu |
| E0021 — 05-siniflandirma.png | Günlük Özet; Gelecek Tahmini kapalı; İşlem tipi/dönem seçicileri; 30 Gün, Tablo, Günlük İstatistikler; solda giriş aracı örtüşmesi | Sınıflandırma veya şahsi gider formu değil; K04 atfı yanlış. Örtüşen metinden tam seçenek adı çıkarılmaz | Kanıt eki — yanlış atıf/örtüşme |
| E0022 — 06-islem-listesi.png | Ağustos gelir 25.000; giderler 850 ve 1.200; 3.000 transfer ve 1.200 kart ödemesi ikişer bacak, gün netleri 0; hesap bakiyeleri; Ortak Cuzdan açılışı 1 Eylül | Çekirdek hareketler doğrulanır. Tarihsel satır bakiyesi ile güncel bakiye ayrılmalı: 5 Ağustos cüzdan -850, güncel seçicide 4.150 | Ana anlatım — çekirdek hareketler |
| E0023 — 07-rapor.png | Net Kazançlar; Ağustos/Eylül sütunları, GİDER -2.050/0; Araba ve Eve Ait alt kategorileri; filtre/yazıcı ikonları | Gider ve hiyerarşi doğrulanır. 44.950 net varlık ve dışa aktarma format/Premium seçimi bu karede görünmüyor | Ana anlatım — dönem raporu |
| E0024 — 08-hata-veya-bos-durum.png | Gider detayında 0; Diğer/Diğer, Nakit/Cüzdan; Silmek istediğinize emin misiniz? İptal/TAMAM | Sıfır tutarlı detay ve silme onayı var. Kayıt sırasında hiç uyarı çıkmaması koşum notudur; silme sonucu, kalıcılık ve geri yükleme kanıtlanmaz; B10/BC-Q03 açık | Kanıt eki — silme iddiasının sınırı |
| E0025 — 09-ozgun-ozellik.png | Ana Ekran ayarları; sekiz kart anahtarı, Cüzdan/Hesap-2/Hesap-3 ve kısmen Hesap-4 | Kart yapılandırma yüzeyi doğrulanır; son yuvanın alt kısmı kırpılmış. Android widget'ı değil | Ana anlatım — kart ayarları |
| E0026 — 10-fresh-bos-ana-ekran.png | Hoş geldiniz, Güncel Bakiye 0.00, Türk lirası TRY, İlk İşlemi Ekle kartının üstü | İlk işlem çağrısı Faz 3'te görünür; demo seçimi alt kırpımda yok. Yeni emülatör/kurulum geçmişi yalnız koşum notu | Ana anlatım — boş başlangıç |
| E0027 — 11-uc-hesap-kuruldu.png | Hesaplar; VARLIKLAR Banka 20.000, Nakit 2.000; CARİ HESAP Kredi Kartı 0; Ana Hesap/Ortak Cuzdan/Is Karti ve sıfır hesaplar; Nakit Akım Ayarı | Başlangıç bakiyeleri doğrulanır; çekirdek son bakiye 44.950'nin kanıtı değildir. Ayarın sonucu veya hesap türü kurulum formu görünmez | Kanıt eki — başlangıç bakiyesi |
| E0028 — 12-kart-gideri-taksit-alani.png | Tasarim yazilimi, 1200.0 TRY; Is Karti, Taksit şartlarını seçin; Diğer/Others, August 8, 2026; GİDER seçili | Taksit alanı doğrulanır. Türkçe arayüzde İngilizce tarih/alt kategori var; tamamen Türkçe diye aktarılmaz. Taksit kaydının sonucu değil | Ana anlatım — kart formu |
| E0029 — 13-transfer-formu.png | 3000 TRY; Ana Hesap → Ortak Cuzdan, Transfer ücreti, yön değiştirme ikonu; TRANSFER seçili; August 12, 2026 | Form kaynağı/hedefi ve ücret alanı doğrulanır. Takas metni görünmüyor; ücret uygulanması/kur davranışı kanıtlanmaz. Önceki yetim atıf kapanır | Ana anlatım — transfer formu |
| E0030 — 14-islem-listesi-running-bakiye.png | Çekirdek kart ödemesinden önceki liste; 12 Ağustos ±3000, gün neti 0; gelir 25.000, giderler 850/1200; 10 Eylül açılışları 22.000 | Ara durum: Ana Hesap 42.000, cüzdan 4.150, kart -1.200. Açılışlar geçmiş satır bakiyelerine eklenmemiş; formda yeni açık atıf | Kanıt eki — ara bakiye |
| E0031 — 15-cekirdek-5-islem-tamamlandi.png | Kart ödemesi ±1.200, 18 Ağustos gün neti 0; güncel Ana Hesap 40.800, Ortak Cuzdan 4.150, Is Karti 0; iki açılış 10 Eylül | Çekirdek son hesap durumu doğrulanır; geçmiş satırdaki Ana Hesap 20.800 güncel 40.800 yerine okunamaz | Ana anlatım — çekirdek son durum |
| E0032 — 16-kontrol-degerleri-net-kazanc-44950.png | Net Kazançlar Ağu: gelir 25.000, gider -2.050, net 22.950; Net Kazanç: Varlıklar Ağu 22.950/Eyl 44.950, Cari hesap 0 | İki benzer etiket farklı toplamlar gösteriyor: dönem akışı ve varlık. Son net hücresi + düğmesiyle kısmen örtülü; 44.950 görünür varlık ve 0 borçtan tutarlı çıkarımdır | Ana anlatım — akış/varlık ayrımı |

### P2-G01 sonucu ve açık sınırlar

**17/17 görsel incelemesi tamamlandı; paket kapandı.** Görsel sayacı 181/360;
Bluecoins 17/90. B01–B19 durumları değişmedi: 11 kanıtla kapandı,
4 kapsamı sınırlandı, 4 açık (B09, B10, B16, B19).

- BC-Q01: Tur 1/Faz 3 ayrıldı; tarih satırına Faz 7 koşum tarihi eklendi.
  Sürüm/device doğrulaması ve sonraki görseller açık; mağaza v13.1 metni
  versionCode doğrulaması sayılmadı.
- BC-Q02: çekirdek 44.950 doğrulandı; Net Kazançlar / Net Kazanç ayrımı ve
  açılışların tarihsel bakiye etkisi yazıldı. B1/taksit ve D1–D3 zinciri
  P2-G02 ve sonraki paketlerde; B10 kapanmadı.
- BC-Q03: kalıcı/geri alınamaz silme hükmü K08, arayüz özeti ve karar
  tablosunda kaldırıldı; B10 ve izinli canlı geri yükleme kontrolü açık.
- BC-Q07: önceki aya açılış girilememesinin nedeni doğrulanmadı;
  sonuç bakiyesi ile tarihsel bakiye ayrıldı. Sistem saati değiştirilmedi.
- BC-Q10: bu paketteki 17 karenin tamamı formda tam adla ve gerçek rolüyle
  bağlı. E0029/E0030 önceki yetimlikten çıktı. Kapsam aralıkları E kimliğine
  dönüştü; kalan 73 Bluecoins karesi sonraki paketlerin atıf işidir.
- BC-Q14/Q15: emülatör örtüşmesi ve karışık dil gözlemi kaydedildi.
  Görselden dokunma hedefi, ekran okuyucu, ürün varsayılanı veya fiziksel
  cihaz davranışı doğrulanmış sayılmadı.

**Sıradaki tek paket P2-G02:** E0033–E0050, 18 görsel; B2 taksit, B1 tekrar,
split, bağımsız hatırlatıcı, kart/cari alanları ve kısmi ödeme.
Yeni canlı koşum, veri değişikliği veya Faz 8 açılışı yapılmadı.

## P1-K ek kararı — Varsayılan sıkı atıf kapısı — 14 Eylül 2026

Kullanıcı karar noktasında en uygun seçeneği seçip uygulamamızı istedi.
Seçim: protokoldeki tam dosya adı zorunluluğu varsayılan kapıda hata olur;
eski betiğin uyarı davranışı korunmaz. `--siki` aynı sonucu veren uyumluluk
bayrağıdır. Karşılığı olmayan kısa kod da hata; dosya adı uydurulmaz.
Önceki P1-K bölümündeki uyarı/opsiyonel sıkılık anlatımı bu karardan önceki
durumdur ve güncel kural değildir.

Yedi P1 formunda **112 kısa kod kullanımı** tekil dosya adına çevrildi:
Money Manager 37, Hesap Defterim 6, Goodbudget 1, Paraşüt 27, Logo İşbaşı 4,
KolayBi 31, QuickBooks 6. Eski 50 uyarı, tekrar sayısı değil gruplu bulgu
sayısıydı. Money Manager'daki karşılıksız 05 ifadesi atıf değil, boş numara
açıklaması olarak düz metne çevrildi. Tarihsel gözlem iddiaları değiştirilmedi.

Kapının mevcut 15 testi yeni karara uyarlandı; API ve CLI'da varsayılan
kısa kod reddi, --siki uyumluluğu, çözülemeyen kod reddi ve 0/1/2 çıkışları
doğrulanır. Örnek blok istisnası korunur. Yeni kütüphane eklenmedi.


### Bu oturumun son doğrulaması — 14 Eylül 2026

- Denetim testleri 15/15 geçti; CLI hata/yanlış kullanım çıkışları 1/2.
- Protokoldeki bash sarmalayıcısı P1'in yedi formunda çıkış 0:
  164 kare, 0 hata, 0 uyarı.
- Bluecoins kapısı 73 hata (henüz incelenmeyen 73 karenin tam ad eksiği),
  Wallet 100 hata (97 yetim kare + 3 aralık) ile çıkış 1; korpusun tamamı
  temiz sayılmadı. Eski yardımcı denetim de eksik atıflar nedeniyle 1 döndü.
- Yardımcı kontrol: 5 iç kontrol geçti; 360 PNG'de bozuk/kopya dosya yok;
  Markdown sütun uyumsuzluğu ve ölü PNG atfı yok. Envanterdeki 357 E kaydı
  ve üç GB-U01 eki dahil 360/360 PNG SHA-256 eşleşti.
- P2-G01: 17 kesintisiz kanıt satırı ve formda 17/17 tam ad atfı doğrulandı.
  Git diff --check temiz. Uygulama kodu değişmedi; backend/Flutter
  build, test ve analyze çalıştırılmadı. Commit yapılmadı.
- Sıradaki tek görev: P2-G02 (E0033–E0050, 18 görsel).


## P2-G02 — Bluecoins taksit, tekrar ve diğer Faz 3 görselleri — 15 Eylül 2026

Durum: kapandı. Hedef E0033–E0050, 18 PNG. Kaynak: mevcut yerel ekran
görüntüleri, önceki manuel Android emülatör koşumu. Yakalama tarihi koşum
kaydında 10 Eylül 2026; inceleme 15 Eylül. Ücretsiz sürüm/13.1.45 bilgisi
önceki koşum kaydıdır, bu tur cihazdan doğrulanmadı. Form/ayar için dönem
uygulanmaz; işlemlerin/vadelerin tarihleri aşağıda. Görseller değiştirilmez,
canlı uygulama verisine dokunulmaz. Hedef belge E0005 Bluecoins formu;
B10 ve BC-Q02/Q04/Q05/Q06/Q07/Q10/Q14 soruları incelenir.
Her satırın kullanım rolü öneridir; raporun yayımlandığı anlamına gelmez.

| Kanıt | Görünen içerik | İddia kontrolü / açık sınır | Kullanım rolü |
|---|---|---|---|
| E0033 — 17-b2-taksit-sartlari-sheet.png | Tasarim ekipmani 6000 TRY, Is Karti; Taksit oranı 0,00, 2 ay, İlk ödeme 11 Eylül 2026; İptal/TAMAM | Başlangıç seçimleri görünür; açılır listenin bütün seçenekleri ve oranın faiz/% anlamı bu kareden doğrulanmaz (BC-Q04) | Ana anlatım — taksit ayar yüzeyi |
| E0034 — 18-b2-taksit-6ay-15agu.png | Aynı panelde 6 ay, oran 0,00 ve İlk ödeme 15 Ağustos 2026 | Geçmiş ilk ödeme seçimi doğrulanır; ilk taksidin yazılma nedeni tarih mi taksit kuralı mı ayrılmaz. Önceki yetim atıf kapanır (BC-Q04/Q07/Q10) | Ana anlatım — geçmiş tarihli kurulum |
| E0035 — 19-b2-6ay-hatirlatici-metni.png | Form tarihi 15 Ağustos; 6000 TRY; 1.000,00 tutarlı 6 aylık hatırlatıcı açıklaması; Değiştir/Sıfırla | Kaydetme öncesi plan özeti görünür; kaydedilen sonuç E0036/E0037 ile ayrılır (BC-Q04) | Ana anlatım — plan önizlemesi |
| E0036 — 20-b2-1-6-taksit-1000-kayit.png | İşlemler'de 15 Ağustos Tasarim ekipmani −1.000, 1/6; o satırda kart −2.200, 18 Ağustos ödeme satırında −1.000 | İlk taksit işlem listesinde doğrulanır. −1.000 güncel bakiye ile taksit anındaki −2.200 ayrılır; anında/otomatik oluşma önceki koşum zinciri, genel taksit kuralı değil (BC-Q02/Q04) | Ana anlatım — gerçekleşen ilk taksit |
| E0037 — 21-b2-kalan-5-taksit-hatirlatici.png | Hatırlatıcılar'da 2/6–6/6; 15 Eylül, Ekim, Kasım, Aralık 2026 ve 15 Ocak 2027; her biri −1.000 Is Karti | Beş bekleyen taksit ve tarihler doğrulanır; ileride otomatik gerçekleşip gerçekleşmeyeceği bu listeden çıkmaz (BC-Q04) | Ana anlatım — kalan taksitler |
| E0038 — 22-b2-sonrasi-rapor-gider-3050.png | Net Kazançlar Ağu gelir 25.000, gider −3.050, net 21.950; Net Kazanç Ağu varlık 22.950/cari −1.000/net 21.950; Eyl değerlerinin sonu + ile örtülü | Gider artışı 1.000 doğrulanır. Çekirdek 44.950'den sonra net 43.950, hesap zincirinden çıkarılır; Eyl hücresi tamamen okunmuş sayılmaz. Eski 44.350 başlangıcı aritmetik hata (B10/BC-Q02) | Ana anlatım — taksit sonrası rapor |
| E0039 — 23-b1-planli-islem-aylik-sheet.png | Bir Defa/Günlük/Haftalık/Aylık/Yıllık; Aylık seçili; Ayın günü; 11 Eylül 2026; Asla; Vade tarihinde otomatik olarak işlem olarak girin kutusu boş | Bu kare geçmiş tarih göstermiyor; geçmiş başlangıcın kanıtı E0040. Otomatik kol seçenek olarak mevcut, çalışması denenmemiş. Alt başka alan kısmen kırpılmış (BC-Q04/Q07) | Ana anlatım — tekrar ve otomasyon seçimi |
| E0040 — 24-b1-yenilenen-islem-banner.png | Bulut yazilim aboneligi 600 TRY/Ana Hesap; 10 Ağustos 2026; Yenilenen işlem Her ay tekrarla 10 Ağustos 2026 | Geçmiş başlangıç ve aylık özet doğrulanır. Otomatik kutunun son durumu bu karede yok; E0039 ve koşum kaydıyla ilişkilidir (BC-Q04/Q07) | Ana anlatım — geçmiş başlangıç |
| E0041 — 25-b1-hatirlatici-gecikmeli-bugun.png | 10 Ağustos −600: 31 gün gecikmeli; 10 Eylül −600: Bugün süresi doluyor; sonraki aylık −600 ve taksit −1.000 satırları aynı listede | Görünen geçmiş/bugün/gelecek hatırlatıcılar doğrulanır; tüm serinin sonsuz geleceği veya hiçbir gerçek işlemin oluşmadığı yalnız bu listeden kanıtlanmaz (BC-Q04) | Ana anlatım — birleşik bekleyen liste |
| E0042 — 26-b1-hatirlatici-detay-kaydet.png | 10 Ağustos Bulut yazilim aboneligi −600, Diğer/Others, Banka/Ana Hesap; Kaydet ve Düzenle; aylık seri özeti | Kaydet eylemi doğrulanır; gerçekleşme tarihi sorusu E0043, tutar etkisi E0044 ile ayrı izlenir (BC-Q04) | Ana anlatım — hatırlatıcı detayı |
| E0043 — 27-b1-islem-olarak-kaydet-bugun-mu.png | İşlem Olarak Kaydet? altında Bugün / 10 Ağustos 2026; arkada −600 abonelik detayı | İki tarih seçeneği görünür. Hangisine basıldığı tek karede yok; Ağustos gider artışı E0044 ile uyumlu. Bugün kolunun sonucu denenmiş sayılmaz (BC-Q04) | Ana anlatım — gerçekleşme tarihi kararı |
| E0044 — 28-b1-onay-sonrasi-rapor-gider-3650.png | Net Kazançlar Ağu gelir 25.000, gider −3.650, net 21.350; Eyl akışlar 0. Net Kazanç tablosu ekran altında kesilmiş | E0038'e göre gider 600 artmış. Net varlık 43.350 tam hücre olarak görünmüyor; hesap zinciriyle çıkarılır. 44.350, onay öncesi net varlık değildir (B10/BC-Q02) | Ana anlatım — onay sonrası akış raporu |
| E0045 — 29-bolmek-split-modu.png | Hepsini temizle / + Ekle; Toplam tutar 0,00; tek alt satır tutar, Diğer/Others, Nakit/Cüzdan, Not ve çöp ikonu | Bölme arayüzü var; birden çok dolu satırın farklı hesaplara kaydı ve muhasebeleşme etkisi denenmemiş. Taslak çıkış uyarısı bu karede yok (BC-Q14) | Ana anlatım — bölme formu; işleyiş kanıtı değil |
| E0046 — 30-bagimsiz-hatirlatici-bir-kez-program.png | Ofis kirasi 10000.0 TRY, Ana Hesap, 11 Eylül 2026 günü bir kez program yapın | Tek seferlik planın form özeti doğrulanır; Bir Defa seçici durumu veya + düğmesinin varsayılan tarihi burada görünmez (BC-Q04) | Ana anlatım — tek seferlik plan formu |
| E0047 — 31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png | 10 Eyl abonelik −600, 11 Eyl Ofis kirasi −10.000 Yarın borçlanacak, 15 Eyl taksit −1.000 ve sonraki tekrarlar | Üç tür aynı listede; Ağustos aboneliği bu üst bölümde artık yok. Ortak liste ortak domain modeli veya birebir BF davranışı kanıtı değildir (BC-Q04) | Ana anlatım — birleşik görünüm |
| E0048 — 32-kart-hesap-kesim-gunu-limit-alanlari.png | Yeni Hesap: Başlangıç bakiyesi, Açılış tarihi 10 Eylül, Hesap Tipi Kredi Kartı, Kredi Limiti 0, Hesap Kesim Günü/Bitiş tarihi Ayın 1. Günü, Hesap seçiminden gizle | Alan varlığı doğrulanır. Bitiş tarihi etiketinin anlamı, günlerin seçenekleri ve ekstre hesaplama davranışı açık. Cari türünün alanları bu karede yok (BC-Q05/Q06/Q07) | Ana anlatım — kart hesap formu |
| E0049 — 33-cari-hesap-olusturuldu.png | Hesaplar: Ana Hesap 40.200, Ortak Cuzdan 4.150, Cari hesap/Ada Reklam cari 0; Kredi Kartı grubu −1.000; Is Karti tutarı kısmen örtülü | B1 sonrası/kısmi ödeme öncesi görünür grup toplamları 43.350 net verir. Cari hesabın listede varlığı kanıt; özel alanı veya fatura bağı olmadığı kanıtı değil (BC-Q02/Q05) | Ana anlatım — hesap durumu ve cari satırı |
| E0050 — 34-kismi-kart-odemesi-500-transfer.png | 10 Eylül transfer ±500, Ana Hesap 39.700/Is Karti −500; açılışlar +22.000, gün neti 22.000. 10 Ağustos abonelik −600 işlem listesinde | Kısmi ödeme 500 ile kart −1.000→−500, banka 40.200→39.700; net 43.350 korunur. Gün başlığı sıfır değil, çünkü açılışlar aynı güne dahil. Her tutar/ekstre bağımsızlığı genellenmez (BC-Q02/Q04/Q06) | Ana anlatım — kısmi ödeme ve B1 gerçek kayıt |


### P2-G02 sonucu — 15 Eylül 2026

**18/18 görsel incelendi; paket kapandı.** Bluecoins 35/90, toplam 199/360.
Yeni görsel, dosya veya canlı koşum yok; kaynak PNG'ler ve emülatör verisi
değişmedi. Kaydedilmiş kanıtların hiçbiri yeniden üretilmedi.

| Soru / bulgu | Bu pakette sonuç | Açık kalan / sonraki iş |
|---|---|---|
| B10 / BC-Q02 | 44.350→43.350 net ifadesi düzeltildi. E0036 ve E0049/E0050 hesap zinciri: 44.950→43.950→43.350; transfer sonrası net aynı. 44.350, B1 sonrası banka+nakit toplamıdır | D1–D3 net zinciri P2-G03; kırpılmış E0038/E0044 net hücreleri doğrudan okuma sayılmadı |
| B10 / BC-Q04 | B1 kapalı otomasyon ve elle onay, B2 geçmiş ilk ödeme koşulu ayrıldı; tanım üretmez/birebir/en yakın hükümleri form ve güncel özetlerden kaldırıldı | Otomatik açık B1, geleceğe kurulan ilk taksit, kalan taksitlerin gerçekleşmesi ve Bugün kolu P2-T; D1 diyaloğu P2-G03 |
| BC-Q05 | E0049 yalnız cari satırını, E0048 yalnız kart formunu gösteriyor; cari alan/fatura bağı yok iddiaları bu karelere dayandırılmıyor | D2/D3 ilişki ve rapor etkisi P2-G03; gerekirse P2-T |
| BC-Q06 | Kesim günü alanı ve 500 kısmi ödeme kanıtlı; her tutar/ekstre bağımsızlığı genellemeleri kaldırıldı | Bitiş tarihi anlamı, gün ayarları ve ekstre davranışı P2-T; Wallet karşılaştırması P3 |
| BC-Q07 | B1 geçmiş başlangıç kanıtı E0039 değil E0040; kart formunda Açılış tarihi alanı var | Önceki aya açılış girişinde ürün/ortam ayrımı hâlâ açık |
| BC-Q10 | E0033–E0050 tam adla formda; önceki hiç anılmayan E0034 artık doğru kurulum adımına bağlı | 55 Faz 7 karesinin atıf borcu P2-G03/G04 |
| BC-Q14 | Bölme yalnız tek boş satır; taslak/ek/OCR ve gerçek cihaz iddiaları koşum notu veya doğrulanamadı olarak ayrıldı | Dolu çoklu bölme ve ortam ayrımı ayrıca kontrol edilmeden ürün davranışı sayılmaz |

**B10 bütünü açık:** aritmetik düzeltildi; otomasyon ve silme/geri yükleme
dalları tamamlanmadı. B01–B19 toplamı değişmedi: 11 kanıtla kapandı,
4 kapsamı sınırlandı, 4 açık (B09, B10, B16, B19).

Yayılım: E0005 B1/B2, bakiye zinciri, kart/cari/bölme/hatırlatıcı,
karar/güven/sonuç; E0003 DURUM güncel sentez ve Bluecoins satırları;
TUR2-YOL-HARITASI Faz 3 özeti; mevcut giriş/protokol/plan başlıkları.
Eski kronolojik devirlerdeki tamamlandı/en yakın/otomatik genellemeleri
tarihsel kayıttır; yeni raporda geçerli kanıt olarak kullanılmaz.

**Sıradaki tek paket P2-G03:** E0051–E0079, 29 görsel; Faz 7 başlangıcı,
D1, D2 ve D3. Bu pakete başlanmadı; Faz 8 açılmadı.


### P2-G02 son kontrolleri — 15 Eylül 2026

- Denetim testleri 15/15 geçti. P1'in yedi formu 0 hata/0 uyarı;
  Bluecoins 55 yetim kare, Wallet 97 yetim kare + 3 aralık hatasıyla
  kapıda başarısız: bütün korpus temiz sayılmadı.
- Yardımcı denetimin 5 iç kontrolü geçti; Markdown sütun uyumsuzluğu,
  ölü PNG atfı, bozuk PNG ve birebir kopya PNG yok. Araç kalan atıf borcu
  nedeniyle 1 döndü.
- 360/360 PNG hash'i envanterle eşleşti; P2-G02'de 18/18 ayrı kanıt ve
  envanter satırı, formda ilk 35/35 görselin tam adı doğrulandı.
- Bakiye kontrolü: 44.950−1.000−600=43.350;
  40.200+4.150−1.000 = 39.700+4.150−500 = 43.350.
- Git diff --check temiz. Uygulama/araç kodu değişmedi; backend ve
  Flutter build/test/analyze çalıştırılmadı; commit atılmadı.
- Sıradaki tek görev P2-G03: E0051–E0079, 29 görsel.


## P2-G03 — Bluecoins Faz 7 başlangıcı ve D1–D3 — 15 Eylül 2026

Durum: tamamlandı. Hedef E0051–E0079, 29 PNG; önceki manuel Android emülatör
koşumunun yerel ekran görüntüleri. Yakalama tarihi koşum kaydında 11 Eylül
2026; inceleme 15 Eylül. Sürüm/cihaz bu tur yeniden doğrulanmadı.
Canlı uygulama açılmadı, veri/ayar değişmedi. Tarih/vade/dönem satırlardadır.
Hedef E0005 gözlem formu; B10, BC-Q02/Q04/Q05/Q10/Q16. Görsel rolü rapor
adayıdır, yayın kararı değildir. Kayıtlı ekran bugünkü cihaz durumu sayılmaz.

| Kanıt | Görünen içerik | İddia kontrolü / sınır | Kullanım rolü |
|---|---|---|---|
| E0051 — f7-00-baslangic.png | Hesaplar, Günlük Özet boş dönem; 7 gün ortalama 0, 30 gün −33,33; Test Reklamı; Bütçe Özeti | Açılış görünümü; boş dönem bütün veritabanı boş demek değil. Ortalama dönemi/hesabı burada doğrulanmaz | Kanıt eki — başlangıç ve reklam |
| E0052 — f7-01-hesaplar-scroll.png | Eylül 2026 takvimi, 10/11/15 günlerinde işaretler; Ağu gelir 25.000, gider −3.650, net 21.350 | Faz 3 sonrası akış tutarları korunmuş; takvim noktalarının anlamı yalnız renkten çıkarılmaz | Kanıt eki — takvim ve akış |
| E0053 — f7-02-hesaplar-scroll2.png | Net Kazançlar Ağu 25.000/−3.650/21.350, Eyl 0; Net Kazanç Eyl varlık 43.850, cari −500, net 43.350 | P2-G02 son neti bu kez tam hücreyle doğrulandı; dönem akışı ile varlık ayrılır (BC-Q02) | Ana anlatım — başlangıç toplamları |
| E0054 — f7-03-hesap-listesi.png | Favori Hesaplar Ana Hesap 39.700, Ortak Cuzdan 4.150, Is Karti −500, Toplam 43.350; 11 Eylül. Nakit Akışı Ağu/Eyl 0 | Hesap başlangıcı doğrulanır; Nakit Akışı sıfırının nedeni burada bilinmiyor, ayar/nedensellik P2-G04'e ait (BC-Q02/Q08) | Ana anlatım — başlangıç hesapları |
| E0055 — f7-04-tum-hesaplar.png | Tümünü göster raporu: 31 Ağustos/11 Eylül; varlık 22.350/43.850, banka 20.200/39.700, nakit 2.150/4.150, cari −1.000/−500, Ada Reklam cari 0/0 | Cari başlangıç sıfır; tarihsel ve güncel bakiyeler ayrı. Kart satırı alt kırpımda ama grup toplamı görünür (BC-Q02/Q05/Q16) | Ana anlatım — dönem ve cari başlangıç |
| E0056 — f7-05-hatirlaticilar.png | 10 Eyl abonelik −600 Dün bitti; 11 Eyl kira −10.000 Bugün süresi doluyor; 15 Eyl 2/6 ve sonraki taksitler | Yeni güne rağmen B1 bekliyor; tüm modların otomasyonu hakkında hüküm vermez (BC-Q04/Q16) | Ana anlatım — D1 öncesi bekleyenler |
| E0057 — f7-06-ofis-kirasi-detay.png | Aynı bekleyen liste; kira satırı hâlâ listede | Dosya adı detay diyor, detay açılmamış; başarılı dokunuş/işlem kanıtı değil (BC-Q10) | Arşiv — gezinme ara karesi |
| E0058 — f7-07-tap-icon.png | Bekleyen liste, kira satırı ve sonraki vadeler | Liste değişmemiş; dosya adından dokunuşun nedeni/ürün hatası çıkarılmaz (BC-Q10/Q14) | Arşiv — sonuç üretmeyen ara kare |
| E0059 — f7-08-tap-retry.png | Bekleyen liste saat 13:55; kira −10.000 bekliyor | Tekrar adı gerçek eylem sayısını veya başarısızlık nedenini kanıtlamaz (BC-Q10/Q14) | Arşiv — tekrar kontrolü |
| E0060 — f7-09-ofis-kirasi-dialog.png | Gider detayında Ofis kirasi −10.000, Ana Hesap; 11 Eylül 16:01 ve bir kez program özeti; Kaydet/Düzenle; Test Reklamı | Dosya adına rağmen henüz onay diyaloğu yok, planlı kayıt detayı var (BC-Q04/Q10) | Ana anlatım — D1 detay ve eylemler |
| E0061 — f7-10-ofis-kirasi-kaydet-sonuc.png | Planlı kira detayı, −10.000, Ana Hesap, Kaydet/Düzenle; reklam değişmiş | Adındaki sonuç sözcüğü başarı kanıtı değil; hâlâ plan detayı (BC-Q10) | Arşiv — ara kare |
| E0062 — f7-11-kaydet-dogru.png | İşlem Olarak Kaydet?; İptal/TAMAM | D1 onayı tek TAMAM; B1 geçmiş vade diyaloğundan farklı, farkın nedeni bu kareyle belirlenemez (BC-Q04) | Ana anlatım — onay |
| E0063 — f7-12-ofis-kirasi-realized.png | Bekleyenlerde abonelik ve 15 Eylül taksiti; 11 Eylül kira satırı yok | Listeden çıkma tek başına gider yazıldığını kanıtlamaz; E0065 ile birlikte okunur | Ana anlatım — bekleyen listesinin sonrası |
| E0064 — f7-13-islemler.png | İşlemler listesi; üstte kira satırı gezinme katmanının altında kısmen örtülü | Sonuç için daha açık E0065 kullanılır; kırpılmış satırdan ek iddia üretilmez | Arşiv — kaydırma ara karesi |
| E0065 — f7-14-islemler-guncel.png | 11 Eylül kira −10.000; gün toplamı −10.000, Ana Hesap 29.700 | E0054 başlangıcı 39.700 ile fark −10.000; gerçekleşen gider doğrulandı. Net toplam bu karede yok (B10/BC-Q02/Q04) | Ana anlatım — D1 sonucu |
| E0066 — f7-15-yeni-islem.png | Kira sonrası işlem listesi ve artı düğmesi | Adına rağmen yeni işlem formu henüz açılmamış | Arşiv — form öncesi |
| E0067 — f7-16-fab-tap.png | Ekle formu; GİDER seçili, 0,00, Cüzdan; Durum, Etiket, Bölmek alanları | Genel Durum alanı var; eski durum alanı yok hükmü daraltılmalı. Seçenekler açılmamış (BC-Q05) | Ana anlatım — ortak form |
| E0068 — f7-17-isim-girildi.png | Ada Reklam hizmet faturasi adı yazılı; gider 0,00, Cüzdan | Fatura sözcüğü serbest işlem adı; ayrı fatura nesnesi kanıtı değil | Kanıt eki — ad girişi |
| E0069 — f7-18-gelir-tutar.png | Tutar odakta; GİDER hâlâ seçili, 0,00 | Dosya adındaki gelir sözcüğü seçili türü yansıtmıyor (BC-Q10) | Arşiv — tutar odağı |
| E0070 — f7-19-tutar-girildi.png | 12000 yazılmış; GİDER seçili, Cüzdan | Henüz gelir kaydı veya doğru hesap sonucu yok | Kanıt eki — tutar girişi |
| E0071 — f7-20-hesap-secim.png | Diğer/Others, İşveren/Bonus/Salary seçenekleri | Hesap değil kategori seçicisi; dosya adı yanıltıcı (BC-Q10) | Kanıt eki — kategori seçicisi |
| E0072 — f7-21-hesap-secici.png | Banka, Nakit, Cari hesap, Kredi Kartı grupları; Ana 29.700, Ortak 4.150, Ada 0, kart −500; Cüzdan seçili | D2 öncesi cari sıfır ve D1 sonrası banka doğrulanır (BC-Q02/Q05) | Ana anlatım — hesap seçicisi |
| E0073 — f7-22-hesap-secildi.png | GELİR 12000, Ortak Cuzdan, Durum alanı | Hesap hedef cari değil; henüz kaydedilmiş sonuç yok | Arşiv — hesap düzeltmesi öncesi |
| E0074 — f7-23-hesap-dogru-secildi.png | GELİR 12000, Ada Reklam cari, 11 Eylül 14:00; Durum alanı | Cari hesaba gelir formu; durum seçenekleri/fatura bağlantısı açılmamış (BC-Q05) | Ana anlatım — D2 hazır form |
| E0075 — f7-24-d2-kaydedildi.png | Ada Reklam hizmet faturasi +12.000, cari 12.000; kira −10.000, Ana 29.700; gün toplamı 2.000 | D2 cari 0→12.000; banka tahsilatı değil. Gün toplamı 12.000−10.000 (B10/BC-Q02/Q05) | Ana anlatım — D2 sonucu |
| E0076 — f7-25-transfer-formu.png | TRANSFER seçili; 0,00; iki hesap Cüzdan; Transfer ücreti, Durum, Etiket | Başlangıç formu; aynı hesap aktarımının kabul edildiği anlamına gelmez | Kanıt eki — transfer başlangıcı |
| E0077 — f7-26-check.png | Ada Reklam kismi tahsilat adı, tutar 0,00, hesaplar Cüzdan | Yalnız ad girilmiş; tamamlanmış transfer değil | Arşiv — ad girişi |
| E0078 — f7-27-transfer-hazir.png | 5000; Ada Reklam cari → Ana Hesap; TRANSFER; 11 Eylül 14:05 | D3 hazır form; belirli fatura seçimi görünmüyor, ürün genelinde bağ yokluğu kanıtlanmaz (BC-Q05) | Ana anlatım — D3 formu |
| E0079 — f7-28-d3-kaydedildi.png | −5.000 cari, bakiye 7.000; +5.000 Ana Hesap, bakiye 34.700; D2 +12.000 satırı duruyor; gün toplamı 2.000 | Cari 12.000→7.000 ve banka 29.700→34.700. Transfer neti sıfır; geçmiş D2 satırındaki 12.000 güncel cari bakiye değildir (B10/BC-Q02/Q05) | Ana anlatım — D3 sonucu |

### Paket sonucu ve sınırlar

29/29 görsel incelendi; Bluecoins 64/90, bütün araştırma 228/360.
BC-Q02: başlangıç neti 43.350 doğrudan görüldü. D1 banka −10.000,
D2 cari +12.000, D3 iki hesap arasında −5.000/+5.000 doğrulandı.
D3 öncesi ve sonrası banka+cari toplamı 41.700; gün toplamı 2.000 sabit.
D3 sonrası tüm hesapların net raporu bu pakette yok; 45.350 ancak diğer
hesapların değişmediği varsayımıyla hesaplanan değerdir, okunmuş rapor değildir.
BC-Q04: D1 aynı gün tek TAMAM onayı; B1 geçmiş vade iki tarih seçimi.
Farkın kaynağı ayrıştırılmadı; açık otomasyon ve silme dalları nedeniyle B10 açık.
BC-Q05: gelir ve transfer yolu doğrulandı. Genel Durum alanı görünür;
seçenekler açılmadı. Fatura nesnesi/ilişkisi ürün genelinde yok hükmü kaldırıldı.
7.000 kalan, bu koşumdaki cari hesap bakiyesidir; belirli faturanın açık tutarı
olarak sunulamaz. BF tarafında P1-B01 karşı taraf düzeyindeki bağ düzeltmesi korunur.
BC-Q10: dosya adları yerine görünen içerik esas alındı, 29 tam ad forma işlendi.
BC-Q16: başlangıç, 11 Eylül koşumuna aittir; 15 Eylül canlı cihaz durumu değildir.
Kullanım rolleri adaydır; kaynak PNG, kimlik ve hash değişmedi. Faz 8 açılmadı.


### P2-G03 son kontrolleri — 15 Eylül 2026

- Denetim betiği: 15/15 test geçti; sıkı denetim varsayılanı korundu.
- P1 yedi form: 0 hata/uyarı. Bluecoins 26 yetim kare (P2-G04);
  Wallet 97 yetim + 3 belirsiz aralık. Genel kapı çıkışı 1; temiz sayılmadı.
- Yardımcı denetim: 360 PNG, 5 öz kontrol; bozuk/kopya PNG, ölü atıf ve
  Markdown tablo uyumsuzluğu 0. Kalan atıf borcu nedeniyle çıkışı 1.
- 360/360 PNG hash'i envanterle eşleşti; G03 29 ayrı görsel kaydı,
  29 envanter rolü ve ilk 64 Bluecoins karesinin tam ad atfı doğrulandı.
- D1/D2/D3 aritmetik kontrolleri ve git diff --check geçti.
- Ürün kodu değişmedi; ürün build/test/analyze çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev P2-G04: E0080–E0105, 26 görsel; kalan Faz 7 taraması.

## P2-G04 — Bluecoins Faz 7 arama, ayar ve dışa aktarma — 15 Eylül 2026

Durum: tamamlandı. Hedef E0080–E0105, 26 PNG; önceki manuel Android emülatör
koşumunun 11 Eylül 2026 tarihli kayıtlarıdır. İnceleme 15 Eylül'de yapılıyor;
canlı uygulama açılmadı, cihaz/sürüm yeniden doğrulanmadı. Hedef E0005 gözlem
formu; BC-Q02, BC-Q08–BC-Q10, BC-Q14–BC-Q16. Görsel rolü rapor adayıdır.

| Kanıt | Görünen içerik | İddia kontrolü / sınır | Kullanım rolü |
|---|---|---|---|
| E0080 — f7-29-arama.png | İşlemler listesi; D3'ün iki bacağı, D2 ve kira; arama alanı açık değil | Dosya adına rağmen arama sonucu değildir; E0081 öncesi durumdur (BC-Q10) | Arşiv — arama öncesi |
| E0081 — f7-30-arama2.png | `Ada` sorgusu; D3 −5.000/+5.000 ve D2 +12.000; Toplam 12.000 | Eşleşen üç satırın neti 12.000'dir. Tek son kare, sonuçların her tuşta canlı güncellendiğini tek başına kanıtlamaz (BC-Q02/Q09) | Ana anlatım — arama sonucu |
| E0082 — f7-31-filtre.png | Filtre sayfası: metin, başlangıç/bitiş tutarı, tarih aralığı, işlem tipi, kategori, hesap, etiket, durum; Satır tarzı, sıfırla, kaydet/yükle ikonları; İptal/TAMAM | Alan varlığı doğrulanır. Çoklu seçim, kayıtlı profil ve ikonların davranışı açılmadı; test edilen uygulamalar arasında en gelişmiş olduğu bu kareyle ölçülmez (BC-Q09) | Ana anlatım — filtre yüzeyi |
| E0083 — f7-32-menu.png | E0082 ile aynı filtre sayfası; saat 14:11 | Dosya adına rağmen menü değil; yeni davranış kanıtlamayan tekrar kare (BC-Q10) | Arşiv — filtre tekrarı |
| E0084 — f7-33-menu2.png | Çekmece: iki ayrı `Hesaplar`, Takvim, Kategoriler, Etiketler, Çöp Kutusu, Ayarlar, QuickSync, Seyahat Modu kapalı, Arkadaşa Öner, Geri Bildirim Gönder | İkon+metin etiketleri ve ikincil gezinme görünür. İki aynı başlık yön bulmayı zorlaştırabilir; hedefleri açılmadan anlamları kesinleştirilmez (BC-Q15) | Ana anlatım — gezinme çekmecesi |
| E0085 — f7-34-kategoriler.png | Adına rağmen Hesaplar ekranı; banka 34.700, nakit 4.150, cari 7.000, kredi kartı −500; Nakit Akım Ayarı düğmesi | D3 sonrası hesap grupları toplamı 45.350'dir; bu hesaplamadır, ekranda bir net toplam satırı değildir. Dosya adı yanıltıcıdır (BC-Q02/Q10) | Ana anlatım — D3 sonrası hesaplar |
| E0086 — f7-35-nakit-akim-ayari.png | Nakit akışında kullanılacak nakit hesaplarını seçme açıklaması; Ana Hesap ve Ortak Cüzdan kapalı, üç sıfır hesap açık | Kaydedilmiş seçimler görünür; bunların ürün varsayılanı olduğu doğrulanmaz. Sıfır nakit akışıyla tutarlıdır, ayarlar değiştirilip sonuç ölçülmediği için nedensellik kesin değildir (BC-Q02/Q08) | Ana anlatım — nakit akışı kapsam ayarı |
| E0087 — f7-36-kategoriler2.png | Gider kategorilerinde Araba, Eve Ait, Eğlence başlıkları ve altlarında Fuel/Maintenance, Clothing/Grocery/Medicines/School, Dining Out/Movies/Shopping; Bütçe Kur | İki düzeyli görsel hiyerarşi ve karışık Türkçe/İngilizce etiketler doğrulanır; bütçe oluşturma davranışı açılmadı (BC-Q15) | Ana anlatım — kategori hiyerarşisi |
| E0088 — f7-37-etiketler.png | Etiket araması; Doğum günü, Film, İş, Kişisel, Tatil ve her satırda silme ikonu | Etiket listesi, arama ve silme eylemi görünür. Değerlerin kullanıcı tarafından serbest oluşturulduğu veya kapsam boyutu olduğu bu kareyle kanıtlanmaz (BC-Q15) | Ana anlatım — etiket listesi |
| E0089 — f7-38-cop-kutusu.png | Çöp Kutusu başlığı altında boş, açıklamasız yüzey | Çöp kutusu hedefi doğrulanır; silinen kaydın buraya düştüğü ve geri yükleme davranışı denenmedi. Boş durumda yönlendirme yok (B10/BC-Q03/Q14) | Ana anlatım — boş çöp kutusu |
| E0090 — f7-39-ayarlar.png | Ayarlar: Dil Tercihi Turkish, Para Birimi, Tarih Ayarları; Görünüm ve Kullanıcı Deneyimi, Veri Yönetimi, Bulut Ayarları, Diğer Ayarlar, Takvim, Bildirim Ayarları, Şifre ve Parmak İzi; ortada reklam | Ayar bilgi mimarisi ve reklamın içerik akışını böldüğü görünür; alt hedeflerin davranışı bu kareyle doğrulanmaz (BC-Q15) | Ana anlatım — ayar merkezi |
| E0091 — f7-40-diger-ayarlar.png | Adına rağmen Veri Yönetimi: Telefon hafızası yedekleme/geri yükleme, Excel (.csv) ve QIF içe aktarma, Verileri Sıfırla | İçe aktarma girişleri doğrulanır; dosya seçimi, şema, başarı ve banka ekstresi ayrıştırma yeteneği denenmedi (BC-Q09/Q10/Q15) | Ana anlatım — veri yönetimi |
| E0092 — f7-41-diger-ayarlar2.png | Diğer Ayarlar: Ürün Girişi, Kategori Ayarları, Hesap Ayarları, Gelişmiş Ayarlar | İkinci seviye ayar gruplaması doğrulanır; hedef içerikleri sonraki karelerle sınırlı okunur (BC-Q15) | Ana anlatım — diğer ayarlar |
| E0093 — f7-42-hesap-ayarlari.png | Adına rağmen Kategori Ayarları: Son Kategoriyi Hatırla kapalı; varsayılan gider ve gelir kategorileri Others; kategori simgeleri ve kompakt seçici açık | İki tür için ayrı varsayılan ve görünüm tercihleri doğrulanır. Bunların fabrika varsayılanı olduğu kanıtlanmaz (BC-Q10/Q15) | Ana anlatım — kategori ayarları |
| E0094 — f7-43-hesap-ayarlari2.png | Hesap Ayarları: Son Hesabı Hatırla kapalı; Varsayılan Hesap Cüzdan; Gizli hesapların seçimi kapalı | Mevcut ayarlar doğrulanır. Son etiket, gizli hesapları seçime katma anlamındadır; hesap gizleme akışının kendisi gösterilmez (BC-Q15) | Ana anlatım — hesap ayarları |
| E0095 — f7-44-gelismis-ayarlar.png | Gelişmiş Ayarlar: son para birimini hatırla kapalı, döviz kurunu hatırla açık; açılır hesap makinesi kapalı; satır tarzı; klavye hemen açık, hızlı tarih kapalı, zamanı göster ve genişletilmiş notlar açık | Dövizli işlemler ve son kullanılan kur tercihlerinin yüzeyi doğrulanır; kur dönüşümü ve hesaplama sonucu denenmedi. Form/listenin yoğun biçimde özelleştirilebildiği görünür (BC-Q15) | Ana anlatım — gelişmiş ayarlar üstü |
| E0096 — f7-45-gelismis-scroll.png | Gelişmiş Ayarlar devamı: genişletilmiş etiketler, hesap+kategoriyi tek satırda göster açık; doğrudan düzenleme modunda aç kapalı | Liste sunumu ve açılış davranışı tercihleri doğrulanır; anahtarların etkisi karşılaştırmalı ekranla ölçülmedi (BC-Q15) | Kanıt eki — gelişmiş ayarlar altı |
| E0097 — f7-46-check-nav.png | Boş Çöp Kutusu ekranı | Dosya adına rağmen gezinme sonucu hakkında yeni bilgi vermez; E0089 tekrarıdır (BC-Q10) | Arşiv — boş ekran tekrarı |
| E0098 — f7-47-takvim.png | Eylül 2026 takvimi, 11 Eylül seçili; Net Kazançlar: gider −10.000, gelir 12.000, net 2.000; kategori/alt kategori yüzde 100 | Günlük gelir-gider-net ayrımı ve kategori açılımı doğrulanır; iki transfer bacağı bu gelir/gider toplamlarını şişirmemiştir (B10/BC-Q02/Q05) | Ana anlatım — günlük takvim kırılımı |
| E0099 — f7-48-takvim-ayarlari.png | E0098 ile aynı 11 Eylül takvim ve 2.000 net görünümü | Dosya adına rağmen ayar ekranı açılmamış; yeni davranış kanıtlamayan daha açık toplam karesi (BC-Q10) | Kanıt eki — takvim toplamı |
| E0100 — f7-49-seyahat-modu.png | Takvim seçiliyken çekmece; Seyahat Modu kapalı | Anahtarın varlığı ve kapalı durumu görülür; seyahat davranışı veya önceki durum doğrulanmaz (BC-Q14/Q15) | Kanıt eki — seyahat anahtarı |
| E0101 — f7-50-seyahat-toggle.png | Çekmece önünde Etiketler seçim sayfası; Doğum günü, Film, İş, Kişisel, Tatil; İptal/TAMAM | Dosya adına rağmen seyahat anahtarının değiştiğini göstermiyor. Bağlam ve hangi eylemin açtığı bu kareden çıkarılamaz (BC-Q10/Q14) | Arşiv — bağlamı belirsiz etiket seçimi |
| E0102 — f7-51-check.png | Çekmece; Takvim seçili, Seyahat Modu hâlâ kapalı | Aç/kapa denemesinin sonucu kanıtlanmıyor; kapalı durum tekrarı (BC-Q14) | Arşiv — seyahat durumu tekrarı |
| E0103 — f7-52-nav-check.png | Hesaplar dashboard'u; günlük gider 10.000, 7/30 gün ortalamaları; reklam; Bütçe Özeti yüzde 100 Others | Gezinmeyle geri dönüş görünür. Reklam bilgi akışını keser; seyahat modu etkisi veya bütçe kuralı doğrulanmaz (BC-Q15) | Kanıt eki — dashboard dönüşü |
| E0104 — f7-53-export.png | E0082 ile aynı filtre sayfası | Dosya adına rağmen dışa aktarma ekranı değildir; filtre tekrarıdır (BC-Q10) | Arşiv — filtre tekrarı |
| E0105 — f7-54-print.png | İşlemi seçin sayfası: PDF veya Yazıcıya gönder, Excel (.csv), HTML | Üç çıktı seçeneğinin yüzeyi doğrulanır. Dosya üretimi, içerik, paylaşım ve Premium kapısı denenmedi; seçeneklerin çalıştığı sonucu çıkarılmaz (BC-Q09) | Ana anlatım — çıktı seçenekleri |

### P2-G04 sonucu ve sınırlar

26/26 görsel incelendi; Bluecoins 90/90, bütün araştırma 254/360.
Arama sonucu D2/D3 eşleşmelerinin net toplamını 12.000 gösteriyor; canlı
güncelleme hızı ölçülmedi. Filtre yüzeyi sekiz ana ölçüt ve satır tarzı ile
kaydet/yükle ikonlarını taşıyor; çoklu seçim ve profil davranışı açılmadı.
Nakit akışı hesap seçimi, kategori/hesap varsayımları, liste görünümü ve kur
hatırlama tercihleri görünür; fabrika varsayılanları ve anahtar sonuçları
ölçülmedi. Takvim 11 Eylül için −10.000 gider, +12.000 gelir ve 2.000 neti
göstererek transfer bacaklarının akışı şişirmediğini doğruluyor. Çöp kutusu
boş; silme/geri yükleme dalı açık kaldığı için B10 kapanmadı. Üç çıktı
seçeneği görünür, gerçek çıktı ve Premium son adımı doğrulanmadı.

BC-Q02 ve BC-Q08–BC-Q10'un bu paketle görülebilen kısımları kapandı.
BC-Q14–BC-Q16 yalnız ekran varlığı/tarihsel koşum sınırıyla daraltıldı;
canlı cihaz, erişilebilirlik, açılan alt seçenekler ve sonuç davranışları
denenmiş sayılmaz. 26 tam dosya adı E0005 formuna ve envantere işlendi.
Kaynak PNG, kimlik ve hash değişmedi; Faz 8 açılmadı.
P2-G04, B16'yı kanıtla kapattı. B01–B19 durumu: 12 kanıtla kapandı,
4 iddianın kapsamı sınırlandı, 3 açık (B09, B10, B19).


### P2-G04 son kontrolleri — 15 Eylül 2026

- Denetim betiğinin 15/15 testi geçti. Bluecoins 0 hata/uyarıyla 90/90
  tamamlandı; diğer yedi tamamlanmış form da 0 hata/uyarı verdi.
- Genel sıkı kapı yalnız Wallet'taki 97 yetim kare ve 3 belirsiz aralık
  nedeniyle çıkış 1 verdi; temiz sayılmadı.
- Yardımcı denetim: 360 PNG, 5 öz kontrol; bozuk/kopya PNG, ölü atıf ve
  Markdown tablo uyumsuzluğu 0. Wallet'taki 97 tam ad borcu nedeniyle çıkış 1.
- 360/360 PNG hash'i envanterle eşleşti; G04'te 26 ayrı kayıt, 26 envanter
  rolü ve Bluecoins formunda 90/90 tam dosya adı doğrulandı.
- Arama/takvim ve D3 sonrası hesap aritmetiği ile git diff --check geçti.
- Ürün kodu değişmedi; ürün build/test/analyze çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev P3-G01: E0274–E0298, 25 Wallet görseli.

## P2-T — Bluecoins gerekli kontrol değerlendirmesi — 15 Eylül 2026

Durum: tamamlandı. Sol High mevcut kayıtları salt okunur taradı; ana agent
soru tanımları, B10 sınırı ve karar etiketini kontrol ederek sonucu kaydetti.
90 görsel yeniden açılmadı; canlı emülatör, yeni veri veya kaynak taraması yapılmadı.
Bu değerlendirme P2-K kapanışının yerine geçmez.

| Soru | Değerlendirme | Rapor sınırı / sonraki bağ |
|---|---|---|
| BC-Q01 | Tarihsel kayıtla yeterli | Tur 1, Faz 3 ve Faz 7 ayrıldı; güncel cihaz/sürüm doğrulanmış sayılmaz |
| BC-Q02 | Kanıt yeterli | Net zinciri ve D1–D3 ayrıldı; hesaplardan türetilen toplam ekranda görünen toplam değildir |
| BC-Q03 | Kapsam sınırı yeterli | Silme onayı ve boş çöp kutusu görünür; kalıcılık/geri yükleme bilinmiyor, karar ertelenir |
| BC-Q04 | Kapsam sınırı yeterli | B1 kapalı otomasyon, B2 geçmiş ilk ödeme, D1 aynı gün onayı ayrı; diğer dallar bilinmiyor |
| BC-Q05 | Bluecoins kısmı yeterli | Fatura adı tahsis kanıtı değildir; Wallet kıyası B09/P3 sonucunu bekler |
| BC-Q06 | Kapsam sınırı yeterli | Kesim alanı görünür; ekstre/dönem davranışı doğrulanmadı |
| BC-Q07 | Kapsam sınırı yeterli | Geçmiş tarih etkileşiminin nedeni bilinmiyor; Wallet kıyası P3'e bağlı |
| BC-Q08 | Kapsam sınırı yeterli | Yakalanan hesap seçimi fabrika varsayılanı değildir; sıfır akışın nedeni ölçülmedi |
| BC-Q09 | Görünür yüzey için yeterli | Filtre ve üç çıktı seçeneği var; profil, gerçek dosya ve Premium sonucu bilinmiyor |
| BC-Q10 | Mevcut denetimle tamam | 90/90 tam ad ve önceki 0 hata/uyarı sonucu korundu |
| BC-Q11 | Mevcut denetimle tamam | Dört sütunlu tablo düzeltildi; B18 kapalı |
| BC-Q12 | Bluecoins kısmı yeterli | Mevcut yetenek ile aday öneri ayrıldı; nihai çapraz karar B19/P4'te |
| BC-Q13 | Yerel düzeltme tamam | Sözlük dışı Alma / dikkat etiketi Alma yapıldı; B19 genel kapanışı ayrı |
| BC-Q14 | Kapsam sınırı yeterli | Emülatör/adb etkileşimi ürün kusuru sayılmaz; denenmemiş cihaz genellemesi yok |
| BC-Q15 | Görünür yüzey için yeterli | Kategori, etiket ve ayarlar gözlenen düzeyde; seyahat ve tercih sonuçları bilinmiyor |
| BC-Q16 | Tarihsel kayıtla yeterli | Eski başlangıç ve koşum kayıtları kullanılır; bugünkü cihaz durumuna genellenmez |

**Zorunlu ek kullanıcı kontrolü: yok.** 14 Eylül kararına göre bilinmeyen
kök neden veya gelecekteki olayın yeniden üretilmesi kapanış şartı değildir.
Q03/Q04/Q06/Q07/Q08/Q14, ancak yeni bir rapor iddiası veya ürün kararı bu
davranışa dayanacaksa yeniden değerlendirilir; otomatik test kuyruğu oluşturulmaz.

P2-G04 tablosundaki 13 soru bağı düzeltildi; kanıt içeriği değişmedi.
B10 kanıtla kapalı sayılmadı, iddia kapsamı sınırlandı. Güncel B01–B19:
12 kanıtla kapalı, 5 kapsamı sınırlı, 2 açık (B09, B19).
Önceki P3-G01 devirleri P2 kapanış kapısını atladığı için geçersizdir.

Sıradaki tek paket **P2-K**: bu soru tablosu, Bluecoins formu ve güncel özetlerin
kapanış uyumunu değerlendir. Mevcut başarılı görsel ve denetim kayıtları tekrar
koşum gerekçesi olmadan korunur. P3-G01 ancak P2-K sonrasında gelir.

## P2-K — Bluecoins bütünlük ve kapanış kapısı — 15 Eylül 2026

**Durum: kapandı.** P2'nin belge/görsel kapsamı tamamlandı. Ölçülmemiş davranışlar
başarılı test sayılmadan, P2-T'deki gerekçeli sınırlarıyla rapora taşınır.
14 Eylül kapanış kararı ve paket kabul kuralı esas alındı.

| Kabul alanı | Dayanak ve sonuç |
|---|---|
| Tam metin kapsamı | P0 Bluecoins içerik haritası ve BC-Q01–BC-Q16; P2-T tablosunda her soru sonuç veya gerekçeli sınırla kayıtlı |
| Görsel kapsam | P2-G01 E0016–E0032: 17; P2-G02 E0033–E0050: 18; P2-G03 E0051–E0079: 29; P2-G04 E0080–E0105: 26. Toplam 90/90; roller envanterde |
| Aritmetik ve kanıt bağları | P2-G02/G03/G04 zinciri kayıtlı; B16 doğru filtre/çıktı ayrımıyla kapalı; G04 soru bağları P2-T'de düzeltildi |
| Gerekli ek kontrol | P2-T: 0. Silme, geri yükleme, açık otomasyon ve diğer ölçülmemiş sonuçlardan kesin davranış/öneri türetilmez |
| Mekanik kapı | Önceki Bluecoins sıkı kontrolü 0 hata/0 uyarı; G04 15/15 betik testi ve 360/360 hash sonucu kayıtlı. Bu tur yeniden çalıştırılmadı |
| Açıklar ve yayılım | B10 kapsamı sınırlı; B09 Wallet/P3, B19 ortak karar/P4 işi olarak açık. Form, DURUM, README, protokol, Tur 2 yolu ve uygulama planı güncellendi |

Sol High yalnız soru/form/özet tutarlılığını taradı; kapanışa engel somut
çelişki bulmadı. Ana agent kabul ölçütleri ve rapor sınırlarını değerlendirdi.
Eski Bluecoins ilerleme satırları ve silme kararının bekleme gerekçesi güncellendi.
Görseller, hash taraması ve başarılı testler tekrar edilmedi.

Güncel toplam 254/360 görsel. B01–B19: 12 kanıtla kapalı, 5 kapsamı sınırlı,
2 açık (B09, B19). Genel araştırma kapısı temiz ilan edilmedi; Wallet'ın
önceden kaydedilmiş 97 tam ad ve 3 aralık borcu P3 kapsamındadır.
Faz 8 açılmadı; ürün kodu değişmedi, ürün build/test/analyze çalıştırılmadı.
Commit atılmadı. Bu tur belge yamasında git diff --check kontrolü uygulanır.

**Sıradaki tek paket: P3-G01 — E0274–E0298, 25 Wallet görseli.**
Bu tur P3 başlatılmadı. Sonraki pakette yalnız hedef görseller ve ilgili form
bölümü Sol High'a verilir; ana agent kritik çelişkiler ve rapor kaydını üstlenir.

## P3-G01 — Wallet E0274–E0298 görsel incelemesi — 15 Eylül 2026

Kaynak: önceki manuel Android emülatör ekran görüntüleri. Bu paket canlı
emülatör, yeni veri, oturum veya sürüm doğrulaması yapmaz. Her satır yalnız
görselde doğrudan görülen içeriği ve bunun iddia sınırını kaydeder.

| Kanıt | Görülen içerik ve iddia sınırı | Soru / bulgu bağı |
|---|---|---|
| E0274 | Dosya adına karşın mağaza değil: Home altındaki Balance Trend kartında Today TRY 2,150.00, Ağustos çizgisi, Upcoming planned payments yükleme iskeleti, Add more cards ve FAB görünüyor. K00'daki “Play Store güncelleme gösterdi” iddiasını kanıtlamaz; Home kart düzenini kanıtlar. | WL-Q01, WL-Q11, WL-Q15 |
| E0275 | Gezinme çekmecesinin üstü: çalışma alanı başlığı, Get Premium, Bank Sync, seçili Home, Records, Investments (New), Statistics, Planned payments, Budgets, Debts, Goals ve aşağıda kısmen görünen ek hedefler. Geniş menüyü destekler; Currency rates/Group sharing/Settings gibi görünmeyen alt öğeleri bu kare tek başına kanıtlamaz. | WL-Q01, WL-Q10, WL-Q15 |
| E0276 | Home/Accounts: Ana Hesap TRY 20,800.00, Ortak Cuzdan TRY 2,150.00, Is Karti TRY 0; Add account, Select all ve Account Detail. Hemen altında “Wallet for your business”, Premium ve banka bağlantısı tanıtımları var. Hesap/net zincirini ve içeriğin aşağı itilmesini destekler; kart oluşturma alanlarını göstermez. | WL-Q04, WL-Q07, WL-Q15 |
| E0277 | Hızlı formda ortadaki Expense sekmesi zeminle birleşik/seçili, tutar −25,000 TRY; Account ANA HESAP ve Category SALE. Bu kare doğru gelir kaydını değil, varsayılan gider ve renge dayalı seçimi gösterir; gelir sonucu E0284/E0285 ile doğrulanır. Aynı anda Sale kategorisinin Expense altında seçilebilmesi güçlü bir yanlış sınıflandırma riski örneğidir. | WL-Q11, WL-Q15; yeni somut çelişki |
| E0278 | Statistics/Spending, 30D: TRY 2,050.00; Categories/Labels geçişi, donut'ta Communication, PC TRY 1,200 ve Food & Drinks, trend, Go deeper; 6M/1Y kilitli. Gider toplamını ve rapor katmanını destekler; dosya adına karşın işlem sınıflandırma formu, kapsam alanı yokluğu veya payee/note alanlarını göstermez. | WL-Q01, WL-Q10, WL-Q11 |
| E0279 | Records/30D toplamı TRY 22,950.00. Kart ödemesi (1,200) ve hesaplar arası aktarım (3,000) aynı tarih/not/yönle biri yeşil artı, biri kırmızı eksi iki satır; ilgili haftaların toplamı TRY 0. Liste nötrlüğü ve çift satır görünümünü doğrudan destekler. Her iki satırın etiketi de “Transfer, withdraw”; bağın veri modeli ekrandan çıkarılamaz. | WL-Q04, WL-Q11, WL-Q15 |
| E0280 | Statistics/Cash-flow 30D: net TRY 22,950.00, Income TRY 25,000.00, Expenses −TRY 2,050.00; soru başlığı “Am I spending less than I make?” ve altta Trend/Cumulative kartı. 6M/1Y kilitli. Toplamları ve soru başlıklı raporu kanıtlar; business/personal kırılımı ile transferlerin iç hesaplamasını tek başına göstermez. | WL-Q04, WL-Q10, WL-Q11 |
| E0281 | Expense hızlı formunda 0 TRY, Ortak Cuzdan/Groceries ve altta “Please fill in the amount.” snackbar'ı. Sıfır tutarın bu denemede engellendiğini ve hata mesajının alan yanında değil alt geri bildirimde verildiğini kanıtlar; silme veya genel doğrulama davranışını kanıtlamaz. | WL-Q11, WL-Q15 |
| E0282 | Yalnız Planned payments boş durumu: gelecek nakit akışını planlı alacak/giderlerle izleme açıklaması ve (+) ile ilk kaydı oluşturma yönlendirmesi. Yönlendirici boş durumu kanıtlar; Budgets, Debts ve Goals boş durumlarının içeriklerini bu kare kanıtlamaz. | WL-Q11, WL-Q15; yeni kapsam düzeltmesi |
| E0283 | Faz 2 Home/Accounts: Ana Hesap ₺20.800,00, Ortak Cuzdan ₺2.150,00, Is Karti ₺0; Add account ve Records. Üç hesap bakiyesini doğrudan doğrular. Net ₺22.950 ekranda yazmıyor, bu üç değerin aritmetik toplamıdır; sürüm/oturum bilgisi görünmez. | WL-Q01, WL-Q04, WL-Q16 |
| E0284 | Cash-flow'ta seçili aralık 12W/LAST 12 WEEKS; net ₺22.950,00, Income ₺25.000,00, Expenses −₺2.050,00. Dosya adına ve formdaki “Ağustos” ifadesine karşın bu kare ay filtresi göstermez; çekirdek toplamları 12 haftalık görünümde doğrular. | WL-Q01, WL-Q04, WL-Q11; yeni dönem kapsamı düzeltmesi |
| E0285 | Records/LAST 12 WEEKS toplamı ₺22.950,00; 18 Ağu kart ödemesi ±₺1.200 ve 12 Ağu hesap aktarımı ±₺3.000 çift satır, 8 Ağu Software −₺1.200 satırı görünür. Tarihlerin Ağustos olduğunu ve transfer haftalarının sıfırlandığını destekler; listenin altındaki diğer çekirdek satırlar bu karede görünmüyor. | WL-Q01, WL-Q04, WL-Q11 |
| E0286 | Faz 2 Planned payments boş durumu, metin ve CTA bakımından E0282 ile aynı. B1 kurulumu öncesi durumunu bağlar; yeni ürün davranışı eklemez ve B1 plan adını göstermez. | WL-Q01, WL-Q08, WL-Q11 |
| E0287 | Add Planned payment üstünde tarih seçici: 10 Eylül 2026 seçili; başlıkta ay değiştirme oku görünmüyor, 1–9 soluk. Arka formda Ana Hesap, 600, Cash, Recurrent ve Every 1 month seçili. Statik kare geçmiş günlerin gerçekten tıklanamazlığını veya kaydırmanın çalışmadığını tek başına kanıtlamaz; bu etkileşim önceki koşum notudur. | WL-Q05, WL-Q08, WL-Q11 |
| E0288 | Add Planned payment formunun üstü: Income/Expense/Transfer (Expense seçili), Name “Bulut yazilim aboneligi”, Category Software, apps, games, Account Ana Hesap, Amount 600, Currency TRY (soluk), Payment Type Cash, boş **Payee**, Frequency Recurrent payment; Start date ve Notifications yalnız etiket olarak kesik. Formdaki Payee alanı listede anılmıyor. Recurrence seçenekleri (haftalık/yıllık, “her 2. Cumartesi”, Forever/bitiş) bu karede görünmez; koşum notu olarak kalır. | WL-Q08, WL-Q11; form alan listesi düzeltmesi |
| E0289 | Plan detayı: başlıkta düzenle (kalem) ve dişli; Software, apps, games · Every 1 month · Ana Hesap · −₺600,00; PAYMENT OVERVIEW altında Today / turuncu “Due today”, Confirm ve ⋮. Bekleyen örneği ve tek dokunuşluk Confirm'i kanıtlar. ⋮ menüsü açık değil: formun “⋮ → Postpone / Dismiss, Kare 16” atfını bu kare desteklemez. | WL-Q08, WL-Q11; atıf düzeltmesi |
| E0290 | Payment summary diyaloğu: Date 10 Eyl 2026, Account Ana Hesap, Amount 600,00 — üçü açılır ok taşıyor; Currency TRY soluk; Cancel/Confirm. Onay öncesi tarih, hesap ve tutarın değiştirilebilir alan olarak sunulduğunu gösterir; değiştirilmiş bir değerle kaydın sonucu denenmedi. | WL-Q08, WL-Q11 |
| E0291 | Diyalog: “Do you want to create transactions automatically for future scheduled payments?” **Yes (Recommended) önceden seçili** (“Upcoming payments will be converted into records automatically.”), No (“Upcoming payment will wait for your approving.”), Confirm. Arkada 10.1… satırı ve onay işaretli Today görünür: soru ilk örnek onaylandıktan sonra geliyor. “No seçildi” bu karede görünmez; E0292'deki bekleyen sonraki örnek de otomatik kolda vade gelmeden aynı görünebileceği için seçimi tek başına kanıtlamaz. “Plan bazında” ifadesi soru metninde yok, çıkarımdır. | WL-Q08, WL-Q11; B10 ortak ilke |
| E0292 | Onay sonrası plan detayı: üstte 10.10.2026 / “Due in 30 days” + Confirm + ⋮; altta Today / “Paid Today” / −₺600,00. Bu anda yalnız sıradaki bir örneğin listelendiğini ve gerçekleşenin geçmiş satırı olarak kaldığını kanıtlar. Vadesinden önce Confirm düğmesinin görünür olması ürün davranışıdır; erken onayın sonucu denenmedi. | WL-Q08, WL-Q11, WL-Q16 |
| E0293 | Home/Accounts: Ana Hesap ₺20.200,00, Is Karti ₺0, Ortak Cuzdan ₺2.150,00; Cash-flow ve Planned payments kısayolları, ShareCost/Premium/banka bağlantısı kartları. E0283 ile birlikte 20.800 → 20.200 (−600) zincirini doğrular; net 22.350 aritmetiktir. Records'ta rozet yokluğu (form satır 151–152) bu karede yok, kare atfı olmayan koşum notudur. Para biçimi Tur 1'deki “TRY 20,800.00”den farklı (“₺20.200,00”): koşumların cihaz/yerel ayarı ayrı. | WL-Q01, WL-Q04, WL-Q11 |
| E0294 | Kaydedilmiş −₺6.000,00 kaydının ayrıntı ekranı (sarı başlık): Note boş, Labels, Payee, Date 10 Eyl 2026, Time 12:50, **Payment Type Cash**, Warranty None, Status Cleared, Place, Attachments/Add receipt. Görünen bölümde taksit alanı yok; ekranın altı ve hızlı form bu karede yok, Payment Type açılır listesi (Cash/Debit card/…/Web payment) açık değil — o liste koşum notudur. Payment Type bu anda Cash; E0295 arka planında “Credit…” görünür, yani tür sonradan değiştirildi. | WL-Q08, WL-Q11; form kare atfı sınırı |
| E0295 | Aynı kaydın tarih seçicisi (not “Tasarim ekipmani”): Eylül 2026 görünümü, **6 Eylül seçili**, bugün 10 vurgulu; ay değiştirme oku görünmüyor; CANCEL/OK. Bu, planlı ödeme değil **normal kayıt** tarih seçicisidir ve içinde bulunulan ayın geçmiş günü (6 Eylül) seçilebilir durumda. Formun B1 başlangıç tarihi için verdiği “Kare 22” atfı yanlış bağlamdır; kaydın geçmiş aya taşınamaması iddiası ok görünmemesiyle sınırlıdır, kaydırma denemesi statik karede görünmez. E0297'de kayıt “Today” olarak kalmış: 6 Eylül uygulanmadı. | WL-Q05, WL-Q11; yeni somut çelişki (atıf bağlamı) |
| E0296 | Home: Ana Hesap ₺20.200,00, **Is Karti −₺6.000,00**, Ortak Cuzdan ₺2.150,00; snackbar “Your balance on Is Karti dropped below the minimum threshold.” + Show. ₺6.000'ın tek parça kart bakiyesine yazıldığını doğrular; net 16.350 aritmetiktir. Uyarı kart bakiyesi **eksiye indiğinde** (borç arttığında) geldi; formdaki “kart borcu düştüğünde” yönü ters. Eşiğin nerede tanımlandığı kareden çıkmaz. | WL-Q04, WL-Q11; yön düzeltmesi |
| E0297 | Account Detail — Is Karti / Credit card: TODAY −₺6.000,00 yanında kalem (bakiye düzenleme girişi), Last 30 days −400%; 11 Ağu–Today grafiği −1,2k'dan 0'a, bugün −6k'ya iniyor. Last records: Electronics, accessories “Tasarim ekipmani” −₺6.000 Today; Transfer, withdraw Ana Hesap → Is Karti “Kart borcu odemesi” ₺1.200 18 Ağu; Software, apps, games “Tasarim yazilimi gideri - Mavi Yazilim” −₺1.200. Tek kayıt ve tarih = bugün doğrudan görünür; ekstre/dönem bölümü bu ekranda yok. Giderin Eylül raporuna girdiği bu karede görünmez, rapor karesi yok. | WL-Q04, WL-Q07, WL-Q11 |
| E0298 | Edit account — Is Karti: Bank account number boş, Type Credit card, **Credit card / Overdraft limit 0**, Balance Display Options **Available Credit**, **Payment Due Date Not set**, Currency TRY (soluk), Color, **Exclude from stats** (kapalı), üstte çöp kutusu. Alanların varlığını kanıtlar; son ödeme tarihinin “yalnız hatırlatıcı, ekstre kurmaz” olduğu açılmadığı için kanıtlanmaz. Available Credit seçiliyken Home/Detay −6.000 gösteriyor; limit 0 olduğundan iki gösterimin ayrımı bu veriyle ölçülemez. K02'deki başlangıç değeri alanı görünen bölümde yok. | WL-Q06, WL-Q07, WL-Q11 |

### P3-G01 sonucu ve sınırlar

**Durum: kapandı.** 25/25 görsel sonuçla kaydedildi; Wallet 25/106, bütün
araştırma 279/360. Oturum kesintisinde E0274–E0287 kayıtlıydı; kesinti
kuralı gereği yeniden açılmadı, E0288–E0298 bu devamda incelendi. Alt agent
kullanılmadı; inceleme ve kayıt ana agent tarafından yapıldı. Canlı
emülatör, bulut hesabı ve sürüm doğrulaması yapılmadı.

Somut çelişkiler ve forma işlenen düzeltmeler (E0014):

| Kanıt | Önceki form ifadesi | Düzeltme |
|---|---|---|
| E0274 | K00: Play Store güncelleme gösterdi, kanıt `00-magaza.png` | Kare mağaza değil Home kartları; iddia koşum notu |
| E0275, E0282 | Menü alt öğeleri; Budgets/Debts/Goals boş durumları kareli | Karede yalnız menü üstü ve Planned payments; kalanlar koşum notu |
| E0277, E0278 | K03/K04 form kanıtı | E0277 Expense + Sale −25,000 ara durumu; E0278 Spending raporu, form değil |
| E0283–E0285 | "Ağustos gelir ₺25.000 / gider ₺2.050" | Cash-flow 12 haftalık aralık; ay filtresi yok |
| E0287, E0295 | B1 başlangıç tarihi: Kare `14`, `22` | E0295 B2 normal kaydının tarih seçicisi; ayın geçmiş günü (6 Eylül) seçilebilir. Kaydırma/ay seçici yokluğu koşum notu |
| E0288 | B1 alan listesi | Currency ve Payee eklendi; Recurrence seçenekleri karede yok |
| E0289 | ⋮ → Postpone/Dismiss, Kare `16` | Menü kapalı; seçenekler koşum notu. İlgili karar satırına not düşüldü, etiket değişmedi |
| E0291 | "İlk onayda plan bazında sorulur"; "No seçildi", Kare `18` | Soru ilk onaydan sonra gelir; Yes (Recommended) önceden seçili; "plan bazında" çıkarım; "No" karede görünmez |
| E0294 | Hızlı ve ayrıntı katmanında taksit alanı yok; Payment Type listesi | Kare yalnız ayrıntı katmanının görünen bölümü; liste koşum notu |
| E0296 | "Kart borcu düştüğünde" minimum eşik uyarısı; hesap başına alarm | Uyarı kart bakiyesi −6.000'a indiğinde; eşiğin tanımı görülmedi; karar satırı adı daraltıldı, etiket değişmedi |
| E0298 | Payment Due Date yalnız hatırlatıcı; kartta başlangıç değeri alanı | Due Date Not set, açılmadı; başlangıç alanı görünen bölümde yok. Limit 0, Exclude from stats ve silme ikonu eklendi |

Doğrudan doğrulananlar: üç hesap 20.800/2.150/0 ve 22.950 net (E0276, E0283–E0285);
transferin iki satırı ve haftalık 0 etkisi (E0279, E0285); sıfır tutar snackbar'ı
(E0281); B1 bekleyen → ödeme özeti → Paid Today + tek sıradaki örnek ve
20.800→20.200 (E0289–E0293); B2 tek −6.000 kart kaydı, tarih Today ve
Ana 20.200/Kart −6.000/Ortak 2.150, net 16.350 aritmetiği (E0296–E0297).

Bağlı sorular: WL-Q01 (Tur 1 “TRY 20,800.00” / Faz 2 “₺20.200,00” biçim farkı),
WL-Q04 (Faz 2 B1/B2 zinciri 22.950 → 22.350 → 16.350 karelerle bağlandı; Faz 7
sonrası tablo P3-G03/G04), WL-Q05 (planlı ödeme ile normal kayıt tarih seçicisi
ayrıldı), WL-Q06/Q07 (kart alanları görünür; bakiye ayarlama ve gösterim etkisi
ölçülmedi), WL-Q08 (otomatik kol varsayılanı ve "No" seçiminin kare sınırı),
WL-Q11 (25 G01 karesi formda tam adla; E0286 ilk kez anıldı), WL-Q15 (renk/sekme,
snackbar ve boş durum gözlemleri kare sınırıyla). Hiçbiri tamamen kapanmadı;
kalan kısımlar P3-G02–G04 ve P3-T değerlendirmesine bağlı. WL-M09'daki E0295 bağı
B1 değil B2 kayıt tarih seçicisidir; harita satırı tarihsel olarak korunur.

Ek kullanıcı/emülatör kontrolü bu pakette istenmedi: sınırlar rapor iddiasını
daraltarak çözüldü. Otomatik kolun üretimi, Postpone/Dismiss ve erken onay yalnız
bir rapor kararı bunlara dayanacaksa P3-T'de değerlendirilir.

### P3-G01 son kontrolleri — 15 Eylül 2026

- Denetim betiği 15/15 test geçti. Sekiz form 0 hata/0 uyarı.
- Wallet sıkı kapısı 100 → 85 hata: G01'in 25 karesinin tamamı formda tam adla;
  kalan tek G01 bayrağı `10`–`40` aralığındaki kısa kod (aralık G02'ye uzanıyor).
  Kalan 81 yetim kare ve 3 aralık P3-G02–G04 kapsamı; genel kapı temiz sayılmadı.
- 357/357 envanter PNG hash'i eşleşti; diskteki 3 fazla PNG önceden kayıtlı GB-U01 ekleri.
- P3-G01 bulgu ve envanter tablolarında 25 benzersiz, sıralı satır; geçici
  yer tutucu işaretleri kaldırıldı.
- git diff --check temiz. Ürün kodu değişmedi; ürün build/test/analyze
  çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev P3-G02: E0299–E0320, 22 Wallet görseli.

## P3-G02 — Wallet E0299–E0320 görsel incelemesi — 15 Eylül 2026

Durum: kapandı (sonuç aşağıda). Hedef E0299–E0320, 22 PNG; önceki manuel Android emülatör
koşumunun 10 Eylül 2026 kayıtlarıdır (Faz 2 ve ek koşum). Canlı emülatör, bulut
hesabı ve sürüm doğrulaması yapılmaz. Hedef E0014 gözlem formu; WL-Q02, WL-Q03,
WL-Q07, WL-Q08, WL-Q10, WL-Q11, WL-Q15; B09.

| Kanıt | Görülen içerik ve iddia sınırı | Soru / bulgu bağı |
|---|---|---|
| E0299 | Boş New budget formu: Name, Period Monthly (önceden seçili), Set budget Amount 0 / Currency TRY, Define Your Budget altında Categories All, Accounts All, Labels/Add label, kesik Notifications başlığı. Alan varlığını ve varsayılanları kanıtlar; kategori/hesap için seçili liste seçeneği ve Period'un diğer değerleri açık değil. | WL-Q08, WL-Q11 |
| E0300 | Doldurulmuş form: Aylik gider butcesi, Monthly, 5.000 TRY, Categories All, Accounts All. Formdaki kurulum değerlerini doğrudan doğrular. | WL-Q08, WL-Q11 |
| E0301 | Budgets & Goals yüzeyi (başlık kaymış): “Budget Left to Spend / In TRY”, This month, Aylik gider butcesi **−1.600,00 / 5.000,00**, kırmızı çubuk, Over Budget; boş Goals kartı ve Create goal; snackbar “You have exceeded your budget for Aylik gider butcesi.” Saat 13:03, bütçe formuyla aynı dakika: bütçe **oluşturulduğu anda** ay içindeki önceki harcamayı sayıp aşım gösteriyor. 6.600'ün B1+B2 dökümü bu karede yok. | WL-Q08, WL-Q11, WL-Q15 |
| E0302 | Bütçe detayı: Monthly Budget ₺5.000,00, Status as of Eyl 10; Spent/Remains (Spent seçili); ₺6.600,00 ve 10 days; **₺6.600 Forecasted Spend** / 30 days; ₺660,00 average daily spending after 10 days; Spend Breakdown THİS MONTH ₺6.600,00, vs past period to date **+222%**; Categories/Labels, Go deeper, iki dilimli donut, This month gezinmesi; Overview/Records sekmeleri. Aritmetik: Eylül'deki tek kayıtlar B1 600 + B2 6.000 = 6.600, kart harcaması hesap filtresi All altında sayılmış (tek örnek). 6.600 / 2.050 = 3,22 → +222%; 1–10 Ağustos gideri (850 + 1.200 kart) ile tutarlı. Forecast 660×30 doğrusal yansıtma değil, harcanana eşit; yöntem kareden çıkmaz. Go deeper'da kilit işareti görünmez, Premium olduğu koşum notudur. | WL-Q08, WL-Q11; form düzeltmesi (Forecast, Premium) |
| E0303 | New goal giriş ekranı: “What are you saving for?”, boş Name, Create goal, “Some things people save for:” NEW VEHİCLE, NEW HOME, HOLİDAY TRİP, EDUCATİON, EMERGENCY FUND, HEALTH CARE, PARTY, KİDS SPOİLİNG ve kesik bir hediye simgesi. İngilizce büyük harfli etiketlerde Türkçe noktalı **İ** görünür (E0289 “OVERVİEW”, E0302 “THİS” ile aynı): sistem yerel ayarının İngilizce metni bozduğu yerelleştirme kusuru. Form bu kareyi anmıyordu. | WL-Q11, WL-Q15; yeni gözlem |
| E0304 | New goal formu: Name Yeni ekipman fonu, Target amount **0**, Saved already 0, Desired date 10 Eyl 2026 (bugün), Goal color, Icon, Note. Alanları kanıtlar; ₺20.000 hedef bu anda girilmemiş, değer E0305 ile doğrulanmalı. Desired date varsayılanının bugün olması gözlemdir. | WL-Q11 |
| E0305 | Budgets & Goals: bütçe kartı değişmeden −1.600,00 / 5.000,00, Over Budget; Goals “How much I already saved?”, Yeni ekipman fonu ₺0 ve 0 %, iki Show more. Özet kartını doğrular. ₺20.000 hedef tutarı bu yüzeyde de görünmez; formdaki “hedef ₺20.000” kare kanıtı taşımaz. | WL-Q11; form sınırı |
| E0306 | Planned payments listesi: Bulut yazilim aboneligi · Software, apps, games · Every 1 month · −₺600,00 · 10.10.2026; arama ve sıralama ikonları, alt sayfada All/Income/Expense/Transfer filtresi, FAB. Liste düzeyinde planın yalnız sıradaki vadesiyle göründüğünü ve B1 sonrası 10.10.2026 vadesini doğrular; otomatik/onaylı mod bu satırda yazmaz. | WL-Q08, WL-Q11, WL-Q16 |
| E0307 | Debts boş durumu (10 Eylül 13:08, Faz 2): Active/Closed sekmeleri, arama, “Track what you lent and borrowed / Manage all your debts here. Tap the plus button to add the first one.”, FAB. Debts için yönlendirici boş durumu ve iki sekmeyi doğrudan kanıtlar; Tur 1'e ait değil, Faz 2'de borç kurulmadan önceki durumdur. P3-G01'de kare kanıtı yok denen Debts boş durumu bu kareyle kanıtlanır. | WL-Q11, WL-Q15 |
| E0308 | Tam ekran soru, borç formundan önce: “Do you already have this debt record in Wallet?” / “Select a record to define your debt. If you do not have any record, you can create it later.” / Yes, select record · No, skip. Mevcut kayda bağlama ve atlama seçeneklerini kanıtlar; bağlama dalı denenmedi. “Create it later” metni kaydın borçtan sonra eklenebileceğini söyler, davranışı göstermez. | WL-Q02, WL-Q11; B09 |
| E0309 | Boş I Lent formu: Name “To whom you have lent?”, Description “What was it for?”, Account “Select an account, please.”, Amount in TRY 0, Date 10 Eyl 2026, **Due date 10 Eyl 2027**; alt bildirim “Contacts permission denied by user.” Varsayılan +1 yıllık vadeyi ve rehber izni reddinden sonra formun açık kaldığını kanıtlar. Hesabın zorunlu olduğu yalnız yer tutucu metinden çıkarılır, boş kaydetme denemesi karede yok. | WL-Q02, WL-Q11, WL-Q15 |
| E0310 | Records, LAST 12 WEEKS Σ ₺16.350,00. WEEK 37 (Balance ₺16.350,00, Σ −₺6.600,00): Electronics, accessories Is Karti “Tasarim ekipmani” −6.000 Today ve Software, apps, games Ana Hesap −600 Today. WEEK 34 ve 33 (Balance ₺22.950,00, Σ ₺0): 18 Ağu kart ödemesi ±1.200, 12 Ağu “Hesaplar arasi aktarim” ±3.000. 7D/30D/12W/6M/1Y; kilit simgesi görünmez. Borç öncesi net 16.350'yi ekranda doğrular; B1 kaydı notsuz ve tekrar rozetsiz sıradan gider satırıdır. Form bu kareyi anmıyordu. | WL-Q04, WL-Q08, WL-Q11 |
| E0311 | Split record ekranı (13:14): Original record — Electronics, accessories · Tasarim ekipmani · −₺6.000,00; Split düğmesi; kapat ve onay. Bölme girişini B2 kaydı üzerinde kanıtlar. | WL-Q11 |
| E0312 | Split record üstünde New record diyaloğu: Category None, Note boş, Amount 0; Cancel/Create. Alt kaydın kategori, not ve tutar alanlarını kanıtlar. Bölme tamamlanmadı (form da “kaydedilmedi” diyor); “orijinalin tutarı o kadar azalır” ifadesi hiçbir karede görünmez, çıkarımdır. | WL-Q11; form düzeltmesi |
| E0313 | B2 kaydının Record detail ekranı (Payment Type **Credit card**, Status Cleared, Warranty, Place, Attachments) üstünde Add receipt diyaloğu: yalnız **Pick a file** / **Take a picture**. Araç çubuğunda silme ve dallanan ok (split) ikonları; altta ACTİONS ve **Create Automatic Rule + kilit**. Fiş girişinin iki kaynaklı ek olarak sunulduğunu ve otomatik kuralın bu hesapta kilitli olduğunu kanıtlar. Dosya/foto seçildikten sonra hiçbir alanın doldurulmadığı denenmedi; “OCR yok” bu diyaloğun sunduğu seçeneklerle sınırlıdır. | WL-Q10, WL-Q11, WL-Q15 |
| E0314 | Dolu I Lent formu, **16:19** (boş form E0309 13:10'du; ayrı bir deneme): Name Ada Reklam, Description boş, Account Ana Hesap, Amount 5.000, Date 10 Eyl 2026, Due date 10 Eyl 2027. Ek koşum borcunun giriş değerlerini doğrular. | WL-Q02, WL-Q03, WL-Q11 |
| E0315 | Aynı form üstünde diyalog: “Do you want to create a Record for this Debt? / If you create a Record your balance will change.” — No · Yes, create record. Sorunun metnini ve iki seçeneği kanıtlar; hangisinin seçildiği bu karede görünmez (E0317'de okunur). | WL-Q02, WL-Q11; B09 |
| E0316 | Debts / Active, 16:20: I Lent başlığı, “ADA REKLAM OWES ME”, Ada Reklam ₺5.000,00 Today, Add Record, FAB. Borç kartını ve Add Record girişini kanıtlar. Açık Total ve bağlı Records listesi bu özet kartta görünmez; “Total 0'a inince borç kapanır” hiçbir G02 karesinde yok. Bu anda Active listede tek Ada Reklam borcu var (WL-Q03'ün 10 Eylül tarafı). | WL-Q02, WL-Q03, WL-Q11 |
| E0317 | Debt Records (16:21): **Total −₺5.000,00**; tek satır Loan, interests · Ana Hesap · “Me → Ada Reklam - Ada Reklam” · −₺5.000,00 Today; Add Record, ⋮. “Yes, create record” kolunun Ana Hesap'ta Loan, interests kategorili −5.000 kaydı ürettiğini kanıtlar. Toplam işareti borç kartındaki “OWES ME ₺5.000,00” (E0316) ile ters: liste kayıtların toplamını gösteriyor. Ana Hesap 20.200 → 15.200 ve kaydın raporda gider sayılması hiçbir G02 karesinde yok; aritmetik ve koşum notudur. | WL-Q02, WL-Q04, WL-Q11; B09; form sınırı |
| E0318 | Plan detayı üstünde otomatik/onaylı diyaloğu, **16:23**: aynı soru, bu kez **No önceden seçili**, Cancel + Confirm (E0291'de yalnız Confirm vardı). Arkada 10.1… bekleyen ve onaylı Today satırı. Diyaloğun ilk onaydan sonra yeniden açılabildiğini ve kayıtlı modun No olduğunu gösterir; bu, E0291 sonrasında No'nun seçildiğini dolaylı olarak destekler. Diyaloğu hangi düğmenin açtığı karede görünmez (araç çubuğunda kalem ve dişli var). | WL-Q08, WL-Q11; P3-G01 E0291 sınırı daraltıldı |
| E0319 | Edit planned payment (16:25): çöp kutusu ve onay; Expense; Bulut yazilim aboneligi, Software, apps, games, Ana Hesap, 600,00, TRY (soluk), Cash, Payee, Recurrent payment, Start date/Notifications etiketleri. Silmenin düzenleme formunda olduğunu kanıtlar. | WL-Q08, WL-Q11 |
| E0320 | Aynı form üstünde yalnız “Do you really want to delete this item?” No/Yes. Gerçekleşmiş örnek veya geçmiş hakkında uyarı metni **yok** — doğrudan kanıt. Yes'e basıldığı ve Paid Today kaydının silmeden sonra bağımsız kaldığı karede yok; formun Faz 2 sonrası veri durumu planın sürdüğünü (10.10.2026 vadesi) yazdığı için silme onaylanmamış görünüyor. | WL-Q08, WL-Q11; form düzeltmesi |

### P3-G02 sonucu ve sınırlar

**Durum: kapandı.** 22/22 görsel sonuçla kaydedildi; Wallet 47/106, bütün
araştırma 301/360. İnceleme ve kayıt ana agent tarafından yapıldı; canlı
emülatör, bulut hesabı ve sürüm doğrulaması yapılmadı.

Somut çelişkiler ve forma işlenen düzeltmeler (E0014):

| Kanıt | Önceki form ifadesi | Düzeltme |
|---|---|---|
| E0302 | Forecasted Spend “30 güne yansıtma”; Go deeper Premium | Tahmin ₺6.600 = harcanan, doğrusal değil; kilit görünmez, Premium koşum notu. +222% 1–10 Ağustos 2.050 ile tutarlı |
| E0301, E0310 | 6.600 B1+B2 dökümü kare atfıyla | Döküm ekranda yok; E0310'daki iki Eylül kaydının toplamı. Bütçe kurulduğu dakikada aşım gösterdi (yeni) |
| E0304, E0305 | Hedef ₺20.000 | Formda Target 0, özet kartında ₺0 / 0 %; ₺20.000 hiçbir karede yok |
| E0303 | — (anılmıyordu) | Hedef şablonları; İngilizce büyük harfte Türkçe noktalı İ yerelleştirme kusuru (E0289, E0302, E0313 ile tekrar) |
| E0309 | Account (zorunlu) | Yalnız yer tutucu metinden çıkarım; boş form 13:10, dolu form 16:19 iki ayrı deneme |
| E0312 | Split: orijinalin tutarı azalır | Bölme tamamlanmadı; çıkarım |
| E0313 | Fişte hiçbir tutar/tarih/satıcı okunmaz | Diyalog yalnız Pick a file / Take a picture; seçim sonrası denenmedi. Create Automatic Rule kilitli (yeni kare kanıtı) |
| E0317 | “Gerçek gider”, Ana Hesap 20.200 → 15.200, raporlarda gider sayılır | Kategori ve −5.000 görünür; bakiye aritmetik, rapor etkisi ölçülmedi. Debt Records Total −5.000 ile kartın OWES ME 5.000 işaret farkı eklendi; Total 0'da kapanma koşum notu |
| E0318 | Dişli → aynı diyalog | No önceden seçili, Cancel eklenmiş: kayıtlı mod No. P3-G01'deki E0291 “No seçildi” sınırını dolaylı kanıtla daraltır; dişlinin açtığı karede yok |
| E0320 | Paid Today kaydı silmeden sonra bağımsız kalır | Silme onaylanmadı (plan Faz 2 sonrasında sürüyor); davranış gözlenmedi. Uyarı metninin yokluğu doğrudan kanıt |
| E0307 | P3-G01: Debts boş durumu koşum notu | Faz 2 karesiyle kanıtlandı (Tur 1 değil) |

Formdaki üç belirsiz kare aralığı (Faz 2 bölüm girişi `10`–`40`, kanıt bölümünde
`10`–`40` ve `41`–`47`) envanter kimlik aralıklarına çevrildi; kareler ilgili
bölümlerde tam adla anılıyor. Faz 7 `f7-00`–`f7-58` aralığı P3-G03/G04 işidir.

Bağlı sorular: WL-Q02 (Record'lu borcun ürettiği kayıt görünür, rapor etkisi ve
D2/D3 Faz 7 tarafı açık — B09 kapanmadı), WL-Q03 (10 Eylül'de Active listede tek
Ada Reklam ₺5.000; Faz 7 borcu P3-G04), WL-Q04 (B2 sonrası 16.350 ekranda; borç
sonrası 11.350 aritmetik), WL-Q07 (bu pakette ek kanıt yok), WL-Q08 (mod No
dolaylı kanıtlı; otomatik kol ve silme sonrası davranış ölçülmedi; bütçenin kart
harcamasını saydığı tek örnek aritmetikle desteklendi), WL-Q10 (fiş eki seçenekleri
ve kilitli otomatik kural kareli; OCR yokluğu seçim öncesi yüzeyle sınırlı),
WL-Q11 (47 Tur 1/Faz 2 karesi formda tam adla), WL-Q15 (izin reddi sonrası formun
açık kalması E0309'da; noktalı İ yerelleştirme kusuru yeni). Hiçbiri tamamen kapanmadı.

Ek kullanıcı/emülatör kontrolü bu pakette istenmedi: sınırlar rapor iddiasını
daraltarak çözüldü. Borç kaydının rapor etkisi (WL-Q02) Faz 7 D2/D3 kareleriyle
birlikte P3-G04 ve P3-T'de değerlendirilir.

### P3-G02 son kontrolleri — 15 Eylül 2026

- Denetim betiği 15/15 test geçti. Sekiz form 0 hata/0 uyarı.
- Wallet sıkı kapısı 85 → 60 hata. Tur 1/Faz 2'nin 47 karesinin (G01+G02) hiçbiri
  bayraklı değil; kalan 59 yetim kare ve 1 belirsiz aralık (`f7-00`–`f7-58`) Faz 7
  kareleridir, P3-G03/G04 kapsamı. Genel kapı temiz sayılmadı.
- 357/357 envanter PNG hash'i eşleşti.
- P3-G02 bulgu ve envanter tablolarında 22 benzersiz, sıralı satır (E0299–E0320).
- git diff --check temiz. Ürün kodu değişmedi; ürün build/test/analyze
  çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev P3-G03: E0321–E0357, 37 Wallet Faz 7 görseli (A kısmi kart
  ödemesi ve D1 planlı ödeme; çok sayıda süreç karesi).

## P3-G03 — Wallet Faz 7 A ve D1 görsel incelemesi — 15 Eylül 2026

Durum: kapandı (sonuç aşağıda). Hedef E0321–E0357 (f7-00 – f7-36), 37 PNG; önceki manuel
Android emülatör koşumunun 11 Eylül 2026 kayıtlarıdır. Canlı emülatör, bulut
hesabı ve sürüm doğrulaması yapılmaz. Hedef E0014 gözlem formu; WL-Q04–WL-Q06,
WL-Q11, WL-Q16. Dosya adları çoğu karede içerikten sapıyor; rol içeriğe göre verilir.

| Kanıt | Görülen içerik ve iddia sınırı | Soru / bulgu bağı |
|---|---|---|
| E0321 | f7-00 — Home, 14:42: Ana Hesap **₺15.200,00**, Is Karti −₺6.000,00, Ortak Cuzdan ₺2.150,00; kısayollarda Planned payments, **Debts**, Cash-flow; “Wallet for your business” kartı. Faz 7 başlangıç bakiyelerini doğrudan kanıtlar: P3-G02'de aritmetik kalan 20.200 → 15.200 burada ekranda; net 11.350 üç bakiyenin toplamıdır. | WL-Q04, WL-Q16 |
| E0322 | f7-01-debts — adına karşın Debts değil, E0321 ile aynı Home (14:43). Yeni bilgi yok. | WL-Q11 |
| E0323 | f7-02 — Debts / Active: I Lent, ADA REKLAM OWES ME, Ada Reklam ₺5.000,00 **Yesterday**, Add Record. Faz 7 başlangıcında tek açık borç olduğunu ve 10 Eylül tarihli ek koşum borcu olduğunu kanıtlar (WL-Q03'ün başlangıç tarafı). | WL-Q03, WL-Q16 |
| E0324 | f7-03-fab-menu — Home FAB açık: Create first template, Transfer, New record. Hızlı eylemleri ve şablon girişini kanıtlar. | WL-Q11 |
| E0325 | f7-04-transfer-form — adına karşın **New template** formu: Name, Amount in TRY 0, Account “Select an account, please.”, Category, Payment Type Cash, Labels, Note, Payee, kesik Type (Expenses). Şablon alanlarını kanıtlar; transfer formu değil. | WL-Q11; form “Templates” satırı |
| E0326 | f7-05-back-check — Home, bakiyeler E0321 ile aynı (14:46): şablon kaydedilmeden geri dönüldü. Arşiv. | WL-Q11 |
| E0327 | f7-06-transfer-form2 — hızlı form TRANSFER sekmesi (yalnız ton farkıyla seçili), 0 TRY, From account ANA HESAP → To account IS KARTİ, “Target amount ~ ₺0” satırı ve hesap makinesi tuş takımı. Transferin ayrı tür ve hedef hesaplı olduğunu kanıtlar; sekme seçiminin renk tonuna dayanması ve noktalı İ (KARTİ) tekrar görünür. | WL-Q05, WL-Q11, WL-Q15 |
| E0328 | f7-07-tutar-400 — adına karşın tutar değil, **Account** seçici: Ana Hesap **General**, Is Karti Credit card, Ortak Cuzdan Cash ve “**…outside of Wallet**” Cash. Transferde Wallet dışına hedef seçeneğini kanıtlar (denenmedi). Ana Hesap türü General görünür; K02'deki “Checking hesabı” ifadesinin hangi hesaba ait olduğu bu kareyle eşleşmez. | WL-Q01, WL-Q07, WL-Q11; yeni gözlem |
| E0329 | f7-08-back-transfer — transfer formu (14:48), tutar alanında **8** TRY, Ana Hesap → Is Karti. Giriş sürecinin ara karesi; ürün davranışı eklemez. | WL-Q11 |
| E0330 | f7-09-tutar-400-v2 — tutar **86.400** TRY: hedeflenen 400'ün otomasyonla hatalı girilmiş hâli (emülatör/adb giriş artığı). Kaydedilmedi; ürün iddiası türetilmez. | WL-Q11, WL-Q15 |
| E0331 | f7-10-cleared — adına karşın temizlenmemiş: **864.006.666.666** TRY. Alan bu kadar büyük değeri görünür olarak kabul ediyor; kaydetmede sınır/uyarı olup olmadığı denenmedi. Giriş artığı; arşiv. | WL-Q11, WL-Q15 |
| E0332 | f7-11-cleared2 — tutar 0 TRY'ye dönmüş (14:51), hesaplar aynı. Arşiv. | WL-Q11 |
| E0333 | f7-12-tutar-final — hazır A formu: TRANSFER, **400 TRY**, From ANA HESAP → To IS KARTİ, “Target amount ~ **₺400,00**”. Kısmi kart ödemesinin serbest tutarlı sıradan transfer olarak girildiğini kanıtlar; kartta ekstre/asgari tutar önerisi görünmez. | WL-Q05, WL-Q06, WL-Q11 |
| E0334 | f7-13-a-kaydedildi — Home (14:52): Ana Hesap **₺14.800,00**, Is Karti **−₺5.600,00**, Ortak Cuzdan ₺2.150,00. A sonucunu doğrudan kanıtlar: 15.200 → 14.800 ve −6.000 → −5.600; net 11.350 değişmedi (aritmetik). | WL-Q04, WL-Q06, WL-Q11 |
| E0335 | f7-14-planned — Planned payments (11 Eylül 14:53): yalnız Bulut yazilim aboneligi, −₺600,00, 10.10.2026. B1 planının Faz 7'de sürdüğünü kanıtlar; P3-G02'deki “plan silme onaylanmadı” çıkarımı bu kareyle doğrulanır. | WL-Q08, WL-Q16 |
| E0336 | f7-15-add-planned — boş Add Planned payment: Expense, Name boş, Category “Select category”, Account Ana Hesap, Amount 0, TRY (soluk), Cash, Payee, **Frequency One-Time**, Date/Notifications etiketleri. D1'in tek seferlik plan olarak başladığını kanıtlar; One-Time'ın fabrika varsayılanı mı yoksa seçim mi olduğu tek kareden çıkmaz. | WL-Q05, WL-Q11 |
| E0337 | f7-16-name-amount — adına karşın ad/tutar değil, planlı ödeme **Accounts** seçicisi (14:55): Ana Hesap General (seçili), Is Karti Credit card, Ortak Cuzdan Cash; arama, FAB. Transfer seçicisindeki (E0328) “…outside of Wallet” seçeneği bu listede yok. | WL-Q11 |
| E0338 | f7-17-back-form — Add Planned payment: **Income** seçili, Name Ofis kirasi, Category boş, Ana Hesap, Amount 0, One-Time; Android klavye araç çubuğu formun bir kısmını örtüyor. D1 bir gider olduğu hâlde tür bu anda Income: otomasyon sırasında yanlış sekmeye geçilmiş (E0339'da Expense). Tür seçiminin kolay kaydığının örneği; ürün kusuru kanıtı değil. | WL-Q11, WL-Q15 |
| E0339 | f7-18-form-check — Expense seçili form üstünde tutar **hesap makinesi diyaloğu**: 0, rakamlar, ÷ x − = + işlemleri, Clear / Cancel / Insert. Planlı ödemede tutarın hızlı formdaki tuş takımından farklı bir diyalogla girildiğini kanıtlar. | WL-Q11 |
| E0340 | f7-19-amount-10000 — adına karşın 10.000 değil, diyalogda **42.222**: otomatik giriş artığı. Arşiv. | WL-Q11, WL-Q15 |
| E0341 | f7-20-amount-check2 — diyalogda **42.222.110.000**: giriş artığı. Diyalog çok büyük değeri gösteriyor; kaydetme sınırı denenmedi. Arşiv. | WL-Q11, WL-Q15 |
| E0342 | f7-21-cleared — Clear sonrası 0 (14:59). Arşiv. | WL-Q11 |
| E0343 | f7-22-amount-verify — diyalogda **10.000** (15:00), Insert öncesi. Doğru D1 tutarının girildiğini kanıtlar. | WL-Q11 |
| E0344 | f7-23-form-with-amount — form: Expense, Ofis kirasi, **Category “Select category” hâlâ boş**, Ana Hesap, Amount 10.000, TRY, One-Time; klavye araç çubuğu Payment Type'ı örtüyor. Kategori seçilmeden önceki durumdur; kategori zorunluluğu uyarısı bu karede yok. | WL-Q11 |
| E0345 | f7-24-date-picker — planlı ödeme **Date** seçicisi (15:02): Eylül 2026, 11 Eylül (Cuma, bugün) seçili; **1–10 Eylül soluk**, 12–30 koyu; ay değiştirme oku görünmüyor; CANCEL/OK. Planlı ödemede ayın geçmiş günlerinin görsel olarak devre dışı sunulduğunu doğrudan kanıtlar. P3-G01 E0295'teki normal kayıt seçicisinde (6 Eylül seçilebilir) böyle bir soluklaşma yoktu: kısıt planlı ödemeye özgü görünüyor. | WL-Q05, WL-Q11; B09 ortak ilke değil |
| E0346 | f7-25-day5-attempt — E0345 ile aynı seçici (15:03); seçim hâlâ 11 Eylül. Dosya adındaki 5 Eylül denemesinin sonrasıdır: seçim değişmemiş. Dokunuşun kendisi karede görünmez; E0345 ile birlikte geçmiş günün seçilemediğini destekler. | WL-Q05, WL-Q11 |
| E0347 | f7-26-d1-saved — adına karşın kaydedilmemiş: form üstünde **Category kırmızı**, “Select category” hata metni ve uyarı simgesi; Ana Hesap, 10.000, TRY, Cash, One-Time, **Date 11 Eyl 2026**, Notifications None. Kategorinin zorunlu olduğunu ve alan yanında hata verildiğini doğrudan kanıtlar (sıfır tutardaki alt snackbar'dan farklı geri bildirim). | WL-Q11, WL-Q15 |
| E0348 | f7-27-category — E0347 ile aynı hata durumu (15:05). Arşiv. | WL-Q11 |
| E0349 | f7-28-category-list — Category seçici: MOST FREQUENT Software, apps, ga…, Groceries, **Sale**, Electronics, accesso…; ALL CATEGORİES Food & Drinks, Shopping, Housing, Transportation, Vehicle, Life & Entertainment, Communication, PC, Financial expenses…; arama ve dişli. Gider planında sık kullanılanlarda gelir kategorisi Sale de sunuluyor (E0277'deki tür/kategori karışması riskiyle tutarlı). Noktalı İ tekrar. | WL-Q11, WL-Q15 |
| E0350 | f7-29-category-selected — adına karşın seçim sonucu değil, **Shopping** alt kategori sayfası: GENERAL Shopping; SUBCATEGORİES Clothes & shoes, Drug-store, Electronics, accessories, Free time, Gifts, Health and beauty, Home, garden, Jewels, Kids, Pets. Yanlış dala girilmiş gezinme karesi; iki düzeyli kategori yapısını gösterir. | WL-Q11 |
| E0351 | f7-30-housing — **Housing** alt kategorileri: GENERAL Housing; Energy, utilities, Maintenance, repairs, Mortgage, Property insurance, **Rent**, Services. Rent'in hazır ev/konut kategorisi altında olduğunu kanıtlar; işletme kirası için ayrı sınıf görünmez. D1 bu sayfadan Rent ile değil Property insurance ile kaydedildi (E0352). *(İlk yazımda “D1 için kullanılan Rent” denmişti; E0352 ile düzeltildi.)* | WL-Q11 |
| E0352 | f7-31-rent-selected — adına karşın Rent değil: form Category **Property insurance**, Ana Hesap, 10.000, TRY, Cash, One-Time, Date 11 Eyl 2026, Notifications None. D1 “Ofis kirasi” yanlış alt kategoriyle (Housing → Property insurance) doldurulmuş; bu otomasyon hatasıdır, ürün kusuru değil. Sentetik veri değerlendirmesinde D1 gideri “kira” değil “Property insurance” olarak durur. | WL-Q11; yeni somut çelişki (kategori) |
| E0353 | f7-32-d1-final — Planned payments (15:09): Ofis kirasi · Property insurance · One-Time · **−₺10.000,00 · Today** (turuncu saat ikonu) ve Bulut yazilim aboneligi · 10.10.2026. D1'in bugüne tek seferlik plan olarak kaydedildiğini ve tanımın henüz para üretmeden listede beklediğini kanıtlar. | WL-Q05, WL-Q08, WL-Q16 |
| E0354 | f7-33-tap-planned — Ofis kirasi detayı: Property insurance · One-Time · Ana Hesap · −₺10.000,00; Today / Due today, Confirm, ⋮. Araç çubuğunda yalnız kalem var; tekrarlayan B1 planındaki dişli (otomatik/onaylı) One-Time planda görünmez. | WL-Q08, WL-Q11 |
| E0355 | f7-34-confirm-dialog — Payment summary: Date 11 Eyl 2026, Account Ana Hesap, Amount 10.000,00, TRY (soluk); Cancel/Confirm. B1 ile aynı onay öncesi özetin tek seferlik planda da kullanıldığını kanıtlar; değer değiştirilmedi. | WL-Q08, WL-Q11 |
| E0356 | f7-35-d1-confirmed — detay (15:11): Today / **Paid Today** / −₺10.000,00; bekleyen satır kalmadı, sıradaki örnek yok. One-Time onaydan sonra otomatik/onaylı sorusunun çıktığına dair kare yok. | WL-Q08, WL-Q11 |
| E0357 | f7-36-nav-check — Home (15:12): Ana Hesap **₺4.800,00**, Is Karti −₺5.600,00, Ortak Cuzdan ₺2.150,00. D1 sonucunu doğrudan kanıtlar: 14.800 → 4.800; net 1.350 aritmetiktir. | WL-Q04, WL-Q16 |

### P3-G03 sonucu ve sınırlar

**Durum: kapandı.** 37/37 görsel sonuçla kaydedildi; Wallet 84/106, bütün
araştırma 338/360. İnceleme ve kayıt ana agent tarafından yapıldı; canlı
emülatör, bulut hesabı ve sürüm doğrulaması yapılmadı.

Karelerin 16'sı arşiv/süreç karesidir (Home tekrarları, hatalı otomatik tutar
girişleri, hata durumu tekrarı, yanlış kategori dalı). Dosya adlarının çoğu
içerikten sapıyor (ör. `f7-04-transfer-form` şablon formu, `f7-07-tutar-400` hesap
seçici, `f7-26-d1-saved` kayıt değil zorunlu alan hatası).

Somut çelişkiler ve forma işlenen düzeltmeler (E0014):

| Kanıt | Önceki form ifadesi | Düzeltme |
|---|---|---|
| E0352–E0356 | D1 “ofis kirası”; kategori belirtilmemiş, kare adı `rent-selected` | D1 **Property insurance** kategorisiyle kaydedildi, Rent ile değil. Otomasyon hatası; D1 gideri raporda kira sınıfında durmaz. Kendi E0351 ilk kaydım da düzeltildi |
| E0345, E0346, E0295 | Geçmiş tarih engeli B1'i doğruluyor | Planlı ödeme seçicisinde 1–10 Eylül soluk ve seçim değişmedi; normal kayıt seçicisinde (E0295) 6 Eylül seçilebilir — kısıt planlı ödemeye özgü |
| E0325, E0324 | Templates yalnız Settings'te anılıyordu | FAB'da “Create first template” ve şablon formu kareli; şablon kaydedilmedi |
| E0328 | — | Transfer hedefinde “…outside of Wallet” seçeneği (denenmedi); plan hesap seçicisinde yok (E0337) |
| E0354, E0356 | — | One-Time planda dişli (otomatik/onaylı) yok; onaydan sonra sıradaki örnek yok |
| E0330, E0331, E0340, E0341 | — | Hatalı otomatik tutar girişleri (86.400; 864.006.666.666; 42.222.110.000) yalnız giriş artığıdır; kaydetme sınırı denenmedi, ürün iddiası türetilmez |

Doğrudan doğrulananlar: Faz 7 başlangıcı Ana 15.200 / Kart −6.000 / Ortak 2.150 ve tek
açık borç Ada Reklam 5.000 (E0321, E0323) — P3-G02'de aritmetik kalan 15.200 artık
ekranda. A: serbest tutarlı ₺400 transfer, 15.200 → 14.800 ve −6.000 → −5.600 (E0333,
E0334). D1: zorunlu kategori hatası alan yanında (E0347), ödeme özeti, Paid Today ve
14.800 → 4.800 (E0355–E0357). B1 planı Faz 7'de hâlâ listede (E0335), P3-G02'deki
“silme onaylanmadı” çıkarımı karede doğrulandı.

Formdaki `f7-00`–`f7-58` belirsiz aralığı kimlik aralığına çevrildi; f7-00 – f7-36
kareleri Faz 7 paragraflarında tam adla anılıyor.

Bağlı sorular: WL-Q04 (Faz 7 zinciri 11.350 → 11.350 → 1.350 ekran bakiyeleriyle
bağlandı; D2/D3 sonrası P3-G04), WL-Q05 (tarih kısıtı planlı ödeme ile normal kayıt
arasında ayrıldı; neden ölçülmedi), WL-Q06 (kısmi kart ödemesinin sıradan transfer
olduğu kareli; MM ile aynı model hükmü açık), WL-Q11 (f7-00 – f7-36 tam adla),
WL-Q16 (Faz 7 başlangıç kaydı kareli). Hiçbiri tamamen kapanmadı.

Ek kullanıcı/emülatör kontrolü bu pakette istenmedi: sınırlar rapor iddiasını
daraltarak çözüldü.

### P3-G03 son kontrolleri — 15 Eylül 2026

- Denetim betiği 15/15 test geçti. Sekiz form 0 hata/0 uyarı.
- Wallet sıkı kapısı 60 → 22 hata. f7-00 – f7-36'nın hiçbiri bayraklı değil; kalan 22
  yetim kare tam olarak f7-37 – f7-58'dir (P3-G04). Belirsiz aralık hatası kalmadı.
  Genel kapı temiz sayılmadı.
- 357/357 envanter PNG hash'i eşleşti.
- P3-G03 bulgu ve envanter tablolarında 37 benzersiz, sıralı satır (E0321–E0357).
- “D1 = Rent/kira kategorisi” iddiası başka belgeye yayılmamış (form yalnız D1 adını veriyordu).
- git diff --check temiz. Ürün kodu değişmedi; ürün build/test/analyze
  çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev P3-G04: E0358–E0379, 22 Wallet Faz 7 görseli (D2, D3, arama,
  menü/ayarlar; B09/WL-Q02/WL-Q03).

## P3-G04 — Wallet Faz 7 D2, D3, arama ve ayarlar görsel incelemesi — 15 Eylül 2026

Durum: kapandı (sonuç aşağıda). Hedef E0358–E0379 (f7-37 – f7-58), 22 PNG; önceki manuel
Android emülatör koşumunun 11 Eylül 2026 kayıtlarıdır. Canlı emülatör, bulut
hesabı ve sürüm doğrulaması yapılmaz. Hedef E0014 gözlem formu; WL-Q02–WL-Q04,
WL-Q09–WL-Q11; B09/B12. Dosya adları çoğu karede içerikten sapıyor; rol içeriğe göre verilir.

| Kanıt | Görülen içerik ve iddia sınırı | Soru / bulgu bağı |
|---|---|---|
| E0358 | f7-37-debts-fab — Debts / Active (15:13), tek kart Ada Reklam ₺5.000,00 Yesterday; FAB açık: **I Lent** / **I Borrowed**. Borç eklemenin yalnız bu iki yönle başladığını kanıtlar; fatura/alacak türü seçeneği görünmez. | WL-Q02, WL-Q11 |
| E0359 | f7-38-i-lent-form — adına karşın form değil: FAB kapalı aynı Debts listesi (15:13). Arşiv. | WL-Q11 |
| E0360 | f7-39-i-lent-form2 — “Do you already have this debt record in Wallet?” tam ekran sorusu (15:14), Yes, select record / No, skip. P3-G02 E0308 ile aynı ekran; Faz 7'de de borç formundan önce sorulduğunu kanıtlar. | WL-Q02, WL-Q11 |
| E0361 | f7-40-i-lent-form3 — aynı soru ekranı (15:15). Arşiv. | WL-Q11 |
| E0362 | f7-41-i-lent-form4 — boş I Lent formu: Account “Select an account, please.”, Amount 0, Date 11 Eyl 2026, **Due date 11 Eyl 2027**. +1 yıl vade varsayılanını ikinci kez kanıtlar. | WL-Q02, WL-Q11 |
| E0363 | f7-42-form-filled — adına karşın dolu form değil: Debts / Active (15:17), hâlâ yalnız Ada Reklam ₺5.000,00. Formdan çıkılıp yeniden girildiği ara kare; ikinci borç bu anda yok. Arşiv. | WL-Q03, WL-Q11 |
| E0364 | f7-43-amount-12000 — I Lent formu (15:20): Name **Ada Reklam**, Description **Hizmet faturasi**, Account Ana Hesap (kesik), hesap makinesinde **12.000**. D2 hizmet faturası alacağının “I Lent” (borç verdim, “To whom you have lent?”) formuyla ve mevcut borçla **aynı adla** girildiğini kanıtlar; bu karede aynı ad için uyarı görünmez. Hizmet alacağı ile borç verme arasındaki anlam farkı üründe ayrı bir tür olarak sunulmuyor. | WL-Q02, WL-Q03, WL-Q11; B09 |
| E0365 | f7-44-d2-saved — adına karşın kayıt sonucu değil: form üstünde “Do you want to create a Record for this Debt? / If you create a Record your balance will change.” — No · Yes, create record (15:21); arkada Hizmet faturasi, Ana Hesap, 12… Sorunun D2'de de sorulduğunu kanıtlar; hangi seçeneğin seçildiği bu karede görünmez. | WL-Q02, WL-Q11; B09 |
| E0366 | f7-45-d2-final — Debts / Active (15:21): **iki ayrı kart**, ikisi de “ADA REKLAM OWES ME”: Ada Reklam · Hizmet faturasi · **₺12.000,00** · Today ve Ada Reklam · ₺5.000,00 · Yesterday. WL-Q03'ü kanıtla cevaplar: aynı adlı iki borç var, karşı taraf başına birleştirilmiyor ve toplam gösterilmiyor. Bu karede D2 için hangi kolun seçildiği görünmez; bakiye etkisi E0371/E0372 ile okunur. | WL-Q02, WL-Q03; B09 |
| E0367 | f7-46-add-record-form — adına karşın form değil: diyalog “Choose how to add Record to this Debt:” — **Select Record** (“to choose an existing Record to bind to the Debt”) / **Create new Record** (“to repay or increase the Debt manually”); Cancel. Arkada üstteki 12.000 kartı. Borca sonradan mevcut kayıt bağlama ve borcu **artırma** seçeneğinin de sunulduğunu kanıtlar; bağlama ve artırma denenmedi. | WL-Q02, WL-Q11; B09 |
| E0368 | f7-47-new-record-form — Create Debt Record: Debt action **Repay debt**, Account Ana Hesap, Amount yer tutucusu “**₺12.000,00 to Repay debt**”; tuş takımı. Yer tutucu kalan tutarı gösterdiği için D3'ün 12.000'lik D2 kartına bağlandığını kanıtlar. Debt action listesinin diğer değerleri açılmadı. | WL-Q02, WL-Q03, WL-Q11; B09 |
| E0369 | f7-48-amount-5000 — aynı form, Amount **5.000** (15:24). Kısmi tutarın serbestçe girildiğini kanıtlar; kalandan büyük tutarın davranışı denenmedi. | WL-Q02, WL-Q11 |
| E0370 | f7-49-d3-saved — Debts / Active (15:24): Ada Reklam · Hizmet faturasi · **₺7.000,00** · Today; diğer kart ₺5.000,00 değişmedi. D3'ün yalnız bağlandığı borcu 12.000 → 7.000 düşürdüğünü doğrudan kanıtlar. Borcun kendi kayıt listesi bu karede açılmadı. | WL-Q02, WL-Q03, WL-Q04; B09 |
| E0371 | f7-50-balance-check — Home (15:25): Ana Hesap **₺9.800,00**, Is Karti −₺5.600,00, Ortak Cuzdan ₺2.150,00. D1 sonrası 4.800 (E0357) ile birlikte: D2 + D3 toplam etkisi +5.000 = yalnız D3 tahsilatı; **D2'nin (₺12.000, Record'suz) hesap bakiyesine etkisi 0**. Net 6.350 aritmetik. | WL-Q02, WL-Q04; B09 |
| E0372 | f7-51-records — Records LAST 12 WEEKS **Σ ₺6.350,00**; WEEK 37 Balance ₺6.350,00, Σ −₺16.600,00: **Lending, renting** · Ana Hesap · “Ada Reklam → Me : Hizmet faturasi - Ada Reklam” · **+₺5.000,00** Today; Transfer ±400 Today; Property insurance −10.000 Today; Loan, interests −5.000 Yesterday; Electronics −6.000 Yesterday; Software (kesik) Yesterday. **12.000 tutarlı satır yok**: D2 işlem listesinde görünmez. Hafta toplamı +5.000 − 10.000 − 5.000 − 6.000 − 600 = −16.600 ve 22.950 − 16.600 = 6.350 aritmetiği tutar. D3 kaydı listede yeşil artı olarak görünür; Cash-flow/gelir raporunda gelir sayılıp sayılmadığı ölçülmedi. | WL-Q02, WL-Q04, WL-Q09; B09/B12 |
| E0373 | f7-52-search — arama “Ada”, LAST 12 WEEKS Σ ₺25.000,00: WEEK 37 Σ ₺0 — Lending, renting +5.000 Today ve Loan, interests −5.000 Yesterday; WEEK 32 Σ ₺25.000 — Sale +25.000 3 Ağu “Web tasarim hizmeti geliri - Ada Reklam”. Üç sonuç ve **D2'nin arama sonucunda olmaması** doğrudan kanıtlı; arama not metnini eşliyor. Her iki hafta başlığında Balance ₺6.350,00: filtreli görünümde bakiye hafta sonu değil güncel değer. “Canlı filtre” tek kareden kanıtlanmaz. | WL-Q09, WL-Q10, WL-Q11; B12 |
| E0374 | f7-53-more-options — adına karşın menü açık değil: Records arama alanı boş (“Search in records”), araç çubuğunda **⋮** ikonu, liste E0372 ile aynı. Records'ta üç nokta menü girişinin **var** olduğunu kanıtlar; içeriği (filtre/dışa aktarma) bu karede görünmez. Formdaki “üç nokta menüsü bulunamadı” ifadesi girişin yokluğu olarak okunmamalı. | WL-Q10, WL-Q11; form düzeltmesi |
| E0375 | f7-54-hamburger — gezinme çekmecesi üstü: hesap sahibinin adı ve My Wallet (kişisel ad kayda kopyalanmadı; teslimde karartma adayı), Get Premium, Bank Sync, Home (seçili), Records, Investments (New), Statistics (açılır), Planned payments, Budgets, Debts, Goals, Try our new app ShareCost (New), Shopping lists. | WL-Q10, WL-Q11, WL-Q15; P7 görsel politikası |
| E0376 | f7-55-menu-scroll — çekmecenin devamı: Debts, Goals, ShareCost, Shopping lists, **Warranties, Loyalty cards, Currency rates, Group sharing**, Others (açılır), Dark mode ve Hide Amounts anahtarları (kapalı), Invite friends, Follow us, Help, Settings. Formdaki geniş menü listesini Faz 7 karesiyle kanıtlar; P3-G01'de E0275 için “koşum notu” denen alt öğeler bu kareyle kareli. Statistics ve Others alt öğeleri açık değil. | WL-Q10, WL-Q11, WL-Q15 |
| E0377 | f7-56-settings — Settings: User profile (“…logout or delete data”), Premium plans; General: Accounts, Categories (“…add custom subcategories”), Labels, **Templates**, **Filters** (“Set custom filters that you can use in Statistics or Records”), kesik Automatic rules. Ayar bilgi mimarisini ve kayıtlı filtre girişini kanıtlar; filtre oluşturma denenmedi. | WL-Q10, WL-Q11 |
| E0378 | f7-57-settings-scroll — Settings devamı: Filters, **Automatic rules** (“…assign categories and labels to your records and recognize transfers.”), **Currencies** (“Add other currencies, adjust exchange rates”); Other settings: Notifications, Security (PIN or Fingerprint), **Advanced settings** (“Set number format, module after launch or initial day of the month”), Personal data & Privacy (GDPR), About Wallet. Automatic rules satırında kilit görünmez; kayıt detayındaki “Create Automatic Rule” ise kilitliydi (E0313): erişim farkı açık, kural içi denenmedi. | WL-Q10, WL-Q11 |
| E0379 | f7-58-advanced — Advanced settings: Number format “Use decimals within amounts” **açık**; Active module after launch “Dashboard module” kapalı; **Initial day of the month — Beginning of the accounting period: 1**. Muhasebe dönemi başlangıç günü ayarının varlığını ve değerini kanıtlar; değiştirildiğinde rapor/bütçe etkisi denenmedi. BF kıyası (WL-Q13) kareyle çözülmez. | WL-Q10, WL-Q13, WL-Q11 |

### P3-G04 sonucu ve sınırlar

**Durum: kapandı.** 22/22 görsel sonuçla kaydedildi. **Wallet 106/106; bütün
araştırma 360/360 görsel.** İnceleme ve kayıt ana agent tarafından yapıldı; canlı
emülatör, bulut hesabı ve sürüm doğrulaması yapılmadı.

Somut çelişkiler ve forma işlenen düzeltmeler (E0014):

| Kanıt | Önceki form ifadesi | Düzeltme |
|---|---|---|
| E0366–E0370 | WL-Q03: aynı adlı iki borç var mı, D3 hangisine bağlandı | İki ayrı “Ada Reklam” kartı (12.000 Hizmet faturasi, 5.000); karşı taraf başına birleştirme/toplam yok, aynı ad için uyarı görülmedi. D3 “₺12.000,00 to Repay debt” yer tutucusuyla 12.000 kartına bağlandı, kartı 7.000'e düşürdü |
| E0364 | D2 “I Lent” kaydı | Hizmet faturası alacağı “To whom you have lent?” borç verme formuyla girildi; fatura alacağı için ayrı tür yok |
| E0365, E0371, E0372 | “No” seçildi, bakiye değişmedi, işlem geçmişinde görünmedi | “No” karede yok; D2 bakiye etkisi 0 olduğu 4.800 → 9.800 farkının yalnız D3'ün +5.000'i olmasından ve listede 12.000 satırı olmamasından okunur |
| E0367, E0368 | D3 “Add Record → Create new Record” | Diyalog mevcut kaydı bağlama ve borcu **artırma** dallarını da sunuyor (denenmedi) |
| E0370, E0372 | “aynı Debt/Records nesnesi altında running balance” | Kanıt kart tutarıdır; borcun kayıt listesi açılmadı. D3 kaydı listede “Lending, renting” yeşil +5.000; gelir raporu etkisi ölçülmedi |
| E0374 | Records üç nokta menüsü bulunamadı | ⋮ girişi var, menü açılmadı; içerik bilinmiyor |
| E0373 | Arama “canlı filtre” | Tek son kare; canlılık kanıtlanmaz. Filtreli görünümde hafta başlığı güncel bakiyeyi gösteriyor |
| E0378, E0313 | Automatic rules, hiçbir rakipte görülmemiş | Settings'te kilitsiz giriş, kayıt detayında kilitli “Create Automatic Rule”; kural içi ve karşılaştırma dayanağı açık |
| E0375 | — | Çekmece hesap sahibinin adını gösteriyor; kayda kopyalanmadı, teslimde karartma adayı (P7) |

Doğrudan doğrulananlar: D2 ₺12.000 borç kartı; D3 5.000 ile 12.000 → 7.000 ve Ana
Hesap 4.800 → 9.800 (E0370, E0371); kayıt listesi Σ 6.350 ve hafta Σ −16.600
aritmetiği (E0372); “Ada” aramasının üç sonucu (Lending, renting +5.000; Loan,
interests −5.000; Sale +25.000) ve D2'nin sonuçta olmaması (E0373); menü, Settings ve
Advanced settings (muhasebe dönemi başlangıcı 1) girişleri (E0375–E0379).

Wallet Faz 7 bakiye zinciri ekranda tam: 15.200 → 14.800 (A) → 4.800 (D1) → 9.800 (D3);
kart −6.000 → −5.600; net 11.350 → 11.350 → 1.350 → 6.350.

Bağlı sorular ve bulgular:

| Kimlik | Durum | Kalan sınır |
|---|---|---|
| WL-Q03 | Kanıtla cevaplandı | — |
| WL-Q04 | Faz 7 hesap zinciri karelerle tam | Net varlık ekranda tek satır olarak görünmüyor; toplam aritmetik |
| WL-Q02 / B09 | Daraltıldı, **açık** | Record'suz D2'nin bakiyeye ve listeye etkisi 0; D3'ün aynı karta bağlandığı kareli. Record'lu kayıtların (Loan, interests; Lending, renting) gelir/gider raporuna etkisi ölçülmedi — tanıma/taşıma eşdeğerliği hükmü bu olmadan verilmez |
| WL-Q09 / B12 | Daraltıldı | Liste ve aramada yok; Statistics/Cash-flow görünürlüğü ölçülmedi, saklama yapısı bilinmez |
| WL-Q10 | Daraltıldı | Filtre, şablon, kural, para birimi yalnız giriş düzeyinde; ⋮ menüsü, dışa aktarma ve kural içi açılmadı |
| WL-Q11 | Kapandı | 106/106 kare formda tam adla; dosya adı–içerik sapmaları kayıtlı |
| WL-Q13 | Açık | Başlangıç günü ayarı kareli; BF MonthlyBudget kıyası kod işidir |

Ek kullanıcı/emülatör kontrolü bu pakette istenmedi. B09'un açık kalan kısmı
(Record'lu borç kayıtlarının rapor etkisi) P3-T'de rapor için gerekli olup olmadığıyla
birlikte değerlendirilir.

### P3-G04 son kontrolleri — 15 Eylül 2026

- Denetim betiği 15/15 test geçti.
- **Sıkı kapı dokuz formun tamamında 0 hata / 0 uyarı, çıkış 0** — araştırma korpusunda
  ilk kez genel kapı temiz. Wallet 106/106 kare tam adla anılıyor.
- 357/357 envanter PNG hash'i eşleşti; Wallet'ın 106 karesinin her biri için envanterde
  tek rol satırı var.
- P3-G04 bulgu ve envanter tablolarında 22 benzersiz, sıralı satır (E0358–E0379).
- git diff --check temiz. Ürün kodu değişmedi; ürün build/test/analyze
  çalıştırılmadı. Commit atılmadı.
- Sıradaki tek görev **P3-T**: Wallet gerekli kontrol değerlendirmesi (WL-Q01–WL-Q16
  için sonuç veya gerekçeli sınır; B09 açık kısmının rapor için gerekli olup olmadığı).
  Görseller yeniden incelenmez. Ardından P3-K.

## P3-T — Wallet gerekli kontrol değerlendirmesi — 15 Eylül 2026

Durum: tamamlandı. Ana agent P3-G01–G04 kayıtlarını, Wallet formunu ve BusinessFinance
kodunu **salt okunur** taradı; 106 görsel yeniden açılmadı, canlı emülatör, yeni veri veya
kaynak taraması yapılmadı. Kod okundu, değiştirilmedi. Bu değerlendirme P3-K kapanışının
yerine geçmez.

Kod dayanakları (BusinessFinance, 15 Eylül çalışma ağacı):
`src/BusinessFinance.Domain/RecurringOccurrenceStatus.cs` yalnız `Planned = 1`,
`Realized = 2` taşır; `RecurringTransactionOccurrence` bekleyen örnekte yalnız
`CorrectAmount` ve `RealizeWithTransaction`/`RealizeWithCharge` sunar — atlama,
erteleme veya otomatik gerçekleştirme yolu yoktur. `RecurringTransaction` başlangıç
tarihinde yalnız boş olmamayı ve bitişin başlangıçtan önce olmamasını denetler; geçmiş
başlangıç serbesttir. `MonthlyBudget.PeriodStart` her zaman ayın 1'idir. Domain'de
işlem bölme (split) kavramı bulunmadı.

| Soru | Değerlendirme | Rapor sınırı / sonraki bağ |
|---|---|---|
| WL-Q01 | Tarihsel kayıtla yeterli | Tur 1 (1 Eyl, “TRY 20,800.00” biçimi), Faz 2 (10 Eyl 12:47–13:16, “₺” biçimi), ek koşum (10 Eyl 16:19–16:25) ve Faz 7 (11 Eyl 14:42–15:32) kare saatleriyle ayrıldı. Güncel sürüm/cihaz doğrulanmış sayılmaz |
| WL-Q02 / B09 | Rapor için yeterli, **eşdeğerlik hükmü verilmez** | Kareli: Record'suz borç bakiyeye, listeye ve aramaya dokunmaz (E0366, E0371–E0373); Record'lu dal kategori taşıyan kayıt üretir (E0317 Loan, interests −5.000; E0372 Lending, renting +5.000); kısmi tahsilat aynı borç kartına bağlanır (E0368, E0370). Record'lu kayıtların gelir/gider raporuna etkisi ölçülmedi. Belge 2 davranışı bu sınırla anlatır; Belge 3'ün önerisi bizde ADR 0014 ile kurulu olduğundan rakipte eşdeğerlik ölçümüne dayanmaz. Form “native destekleyen tek rakip” ifadesi daraltıldı. B09 **kapsamı sınırlı** sayılır |
| WL-Q03 | Kanıtla kapandı (P3-G04) | — |
| WL-Q04 | Kanıt yeterli | Faz 2: 22.950 → 22.350 (B1) → 16.350 (B2) → 11.350 (Record'lu borç); Faz 7: 11.350 → 11.350 (A) → 1.350 (D1) → 6.350 (D3). Hesap bakiyeleri ekranda; net üç bakiyenin toplamıdır, tek satır olarak görünmez |
| WL-Q05 | Kapsam sınırı yeterli; BF kısmı kodla kapandı | Planlı ödemede ayın geçmiş günleri soluk ve seçilemedi (E0345/E0346); normal kayıtta ay içi geçmiş gün seçilebilir (E0295); iki seçicide de ay oku görünmez. Neden ölçülmedi. BF'de geçmiş başlangıç kodda serbest; “ADR gereği” dayanağı yoktu, kaldırıldı. Aynı gözleme verilen iki karar satırı `alma`da hizalandı |
| WL-Q06 | Kapsam sınırı yeterli | Kart alanları (limit 0, Available Credit, Payment Due Date Not set) ve ekstre/kesim yokluğu kareli (E0298); kısmi ödeme sıradan transfer (E0333/E0334). “Money Manager'la aynı model” hükmü MM'in ön doldurulmuş ödeme ve ekstre kuralıyla uyuşmadığı için daraltıldı. MM-Q05/BC-Q06 çaprazı P4 |
| WL-Q07 | Kapsam sınırı yeterli | Cash/checking açılış alanı yokluğu kare kanıtı taşımayan koşum notudur; kart detayında bakiye düzenleme girişi kareli (E0297), etkisi denenmedi; Ana Hesap türü General (E0328). `alma` gerekçesindeki sahte gelir riski çıkarım olarak kalır. Checking e-postası yeniden tetiklenmez |
| WL-Q08 | Wallet kısmı kapsam sınırı yeterli; BF kısmı kodla kapandı | Wallet: No/onaylı modu dolaylı kanıtlı (E0318), Yes kolu üretimi, Postpone/Dismiss ve erken onay ölçülmedi; bütçenin kart harcamasını sayması tek örnek (E0302). BF: skip/postpone yok, otomatik mod yok — “realize/skip ile birebir” ve “bizde varsayılan onaylı” ifadeleri yanlıştı, düzeltildi |
| WL-Q09 / B12 | Kapsam sınırı yeterli | Liste/arama/Debts görünürlüğü kareli; Statistics/Cash-flow görünürlüğü ve saklama yapısı bilinmez |
| WL-Q10 | Kapsam sınırı yeterli | Filtre, şablon, otomatik kural, para birimi yalnız giriş/açıklama düzeyinde (E0324/E0325, E0377/E0378); ⋮ menüsü, dışa aktarma, kural içi ve Premium OCR açılmadı; kural erişimi Settings/kayıt detayı arasında çelişkili |
| WL-Q11 | Kapandı (P3-G04) | 106/106 tam ad; sıkı kapı 0 hata |
| WL-Q12 | Kapandı (P1-B18) | — |
| WL-Q13 | Kodla kapandı | B01 kısmı P1-B01'de. Muhasebe dönemi başlangıç günü: BF'de `MonthlyBudget` sabit ay başı; “tam bizim kavramımızın karşılığı” ifadesi yanlıştı, düzeltildi. 409 `recurring.has_realized_history`, `CounterpartyCharge` yön→kategori türü, `InstallmentPlan`, kapsam ekseni ve Transfer tipi mimari özet ve ADR'lerle uyumlu; split yokluğu kodda doğrulandı |
| WL-Q14 | Yerel düzeltme tamam | Sözlük dışı “Doğrudan al / zaten kısmen var” → `doğrudan al`; “Not” → `alma` (kaybettirdiği yazıldı); aynı gözlemin iki etiketi hizalandı; “birebir”, “tek rakip”, “belirgin eksik” ve “Yorum” (→ Çıkarım) ifadeleri daraltıldı. B19 genel kapanışı ayrı (P4) |
| WL-Q15 | Kapsam sınırı yeterli | İzin reddi sonrası formun açık kalması (E0309), renk/ton seçimi ve tür kayması (E0277, E0327, E0338), noktalı İ yerelleştirme kusuru (E0289, E0302, E0303, E0313) kareli; kullanılabilirlik ölçümü değildir, gerçek cihaza genellenmez |
| WL-Q16 | Tarihsel kayıtla yeterli | Faz 7 başlangıcı kareli (E0321, E0323, E0335). P3 yeni canlı koşum yapmadığı için bugünkü bulut durumu gerekmez; veri silinmez/sıfırlanmaz |

**Zorunlu ek kullanıcı kontrolü: yok.** 14 Eylül kararına göre bilinmeyen davranış
sınırlandırılır, kapanış şartı değildir.

İsteğe bağlı kontrol (yalnız Belge 2'de Wallet borç modeli için “gelir/gider raporuna da
dokunmuyor” gibi daha güçlü bir iddia istenirse):

| Kimlik | Soru | Neden | Ekran / adımlar | Gereken gözlem | Bitiş ölçütü |
|---|---|---|---|---|---|
| WL-U01 | Record'lu borç kayıtları gelir/gider raporunda sayılıyor mu? | B09 eşdeğerlik hükmü | Wallet → Statistics → Cash-flow, 30D veya This month; mevcut veri, yeni kayıt eklenmez | Eylül Income/Expenses toplamı; Lending, renting +5.000 ve Loan, interests −5.000'in toplamlara girip girmediği | Tek ekran görüntüsü ve iki toplam; hesaplama tahmini yapılmaz |

Forma işlenen düzeltmeler (E0014, 12 yer): B1 “realize/skip'ine çok yakın”; karar
satırları “birebir” ve “varsayılan onaylı”; A “Money Manager'la aynı model”; D2 sonrası
“native destekleyen tek rakip”; Advanced settings “tam bizim kavramımızın karşılığı”;
bütçe “mimari kuzen”; etiketler “Doğrudan al / zaten kısmen var”, “Not”, geçmiş tarih
karar satırı ve “ADR gereği”; açılış bakiyesi `alma` gerekçesine sınır; “Yorum” satırı;
tek cümlelik sonuçtaki “tek rakip”.

Karar etiketi değişiklikleri (üçü de yanlış öncül veya sözlük dışı etiket nedeniyle):
Confirm/Postpone/Dismiss satırı `doğrudan al` → **`uyarlayarak al`** (öncül “BF realize/skip
ile birebir” kodla yanlış çıktı; atlama/erteleme bizde yok); fiş eki satırı “Not” →
`alma`; geçmiş tarihli planlı ödeme satırı `henüz karar verme` → `alma` (aynı gözlemin
diğer satırıyla hizalandı). “Doğrudan al / zaten kısmen var” yalnız sadeleştirildi,
sonuç değişmedi. Diğer karar etiketleri değişmedi.

Güncel B01–B19: 12 kanıtla kapalı, **6 kapsamı sınırlı (B09 eklendi)**, **1 açık (B19)**.

Sıradaki tek paket **P3-K**: soru tablosu, Wallet formu ve güncel özetlerin kapanış
uyumu. Başarılı görsel ve denetim kayıtları tekrar koşum gerekçesi olmadan korunur.

## P3-K — Wallet bütünlük ve kapanış kapısı — 15 Eylül 2026

**Durum: kapandı.** P3'ün belge/görsel kapsamı tamamlandı. Ölçülmemiş davranışlar
başarılı test sayılmadan, P3-T'deki gerekçeli sınırlarıyla rapora taşınır.
14 Eylül kapanış kararı ve paket kabul kuralı esas alındı. Görseller, hash taraması
ve başarılı testler yeni değişiklik gerekçesi olmadan tekrar edilmedi.

| Kabul alanı | Dayanak ve sonuç |
|---|---|
| Tam metin kapsamı | P0.3-wallet içerik haritası (WL-M01–WL-M18) ve WL-Q01–WL-Q16; P3-T tablosunda her soru sonuç veya gerekçeli sınırla kayıtlı |
| Görsel kapsam | P3-G01 E0274–E0298: 25; P3-G02 E0299–E0320: 22; P3-G03 E0321–E0357: 37; P3-G04 E0358–E0379: 22. Toplam 106/106; roller envanterde, her kareye tek rol satırı. Bütün araştırma 360/360 |
| Aritmetik ve kanıt bağları | Faz 2 (22.950 → 11.350) ve Faz 7 (11.350 → 6.350) hesap zinciri ekran bakiyeleriyle kayıtlı; net yalnız aritmetik. Otomasyon hataları (tutar artıkları, Income'a kayma, Property insurance kategorisi) ürün kusuru sayılmadı |
| BusinessFinance kıyası | P3-T'de kod okundu: atlama/erteleme ve otomatik gerçekleştirme yok, geçmiş başlangıç serbest, bütçe dönemi ay başı sabit, split yok. Formdaki yanlış kıyaslar düzeltildi; B01 kısmı P1-B01'de |
| Gerekli ek kontrol | P3-T: 0 zorunlu. İsteğe bağlı WL-U01 (Record'lu borç kayıtlarının Cash-flow etkisi) yalnız daha güçlü rapor iddiası için |
| Mekanik kapı | P3-G04 sonrası sıkı kapı dokuz formda 0 hata/0 uyarı, çıkış 0; P3-T sonrası tekrar 0; 15/15 betik testi; 357/357 envanter PNG hash'i. P3-K düzeltmelerinden sonra kapı ve diff kontrolü yeniden çalıştırılır |
| Açıklar ve yayılım | B09 kapsamı sınırlı (P3-T); B19 ortak karar/P4 işi olarak açık. Form oturum bilgisine Faz 7 tarihi ve kare saatleri eklendi (WL-Q01); formdaki kalan “running balance” ifadeleri kart tutarına daraltıldı. DURUM Faz 2/Faz 7 tarihsel özetlerine ve Tur 2 tablosundaki Wallet satırına, TUR2 Faz 7 kapanış satırına P3-K notu işlendi. `raporlar/` altındaki eski denetim metinleri sorunu tespit eden tarihsel kaynaklardır, değiştirilmedi |

Dikkat notu (P7 görsel politikası): E0375 `f7-54-hamburger.png` hesap sahibinin adını
gösteriyor; teslim edilecek belgelerde karartılmadan kullanılmamalı. Kaynak PNG, hash
ve kimlik değişmedi.

Güncel durum: 360/360 görsel. B01–B19: 12 kanıtla kapalı, 6 kapsamı sınırlı, 1 açık
(B19). P2 ve P3 kapandı. Faz 8 açılmadı; ürün kodu değişmedi, ürün build/test/analyze
çalıştırılmadı. Commit atılmadı.

**Sıradaki tek paket: P4-B19** — özetlere yayılan eski/yanlış iddiaların ortak kapanışı
ve “kullanılmaması gereken eski iddialar” listesi (plan Bölüm 6 madde 4); ardından
tema başına P4 paketleri ve P4-K. Faz 8 geçişi yalnız kullanıcı onayıyla açılır.

## WL-U01 — Kullanıcı kontrolü: Record'lu borç kayıtlarının gelir/gider etkisi — 15 Eylül 2026

Durum: **kanıtla kapandı** (kullanıcı ekran kontrolü). Kanıt WL-U01-A
`kanitlar/wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`
(özgün adı `cash-flow.png`). Kullanıcı mevcut veride yeni kayıt eklemeden çekti.
Durum çubuğu 08:07; kullanıcı önceki koşumlarla aynı cihazdan çekti (görüntüdeki çerçeve
farkı ekran görüntüsü alma yönteminden kaynaklanır).

Görülen: Statistics → Cash-flow, **LAST 30 DAYS** (30D seçili; 6M/1Y kilitli): net
**−₺16.600,00**, **Income ₺5.000,00**, **Expenses −₺21.600,00**, vs past period −172%.

Aritmetik eşleme (son 30 gün; 3–8 Ağustos çekirdek kayıtları aralık dışında):
- Gider 21.600 = B1 Software 600 + B2 Electronics 6.000 (kart) + D1 Property insurance
  10.000 + **Loan, interests 5.000**. Transferler (18 Ağu 1.200, 11 Eyl 400) sayılmıyor.
- Gelir 5.000 = yalnız **Lending, renting 5.000** (D3 tahsilatı).
- Net −16.600 = 5.000 − 21.600; kayıt listesindeki WEEK 37 Σ −16.600 (E0372) ile aynı.

Sonuç — B09 **kanıtla kapandı: Wallet'ın borç modeli ADR 0014 tanıma/taşıma ayrımına
eşdeğer değil.**
- Record'lu borç verme (10 Eyl, ₺5.000) **gider** sayılıyor; para el değiştirdi ama
  ekonomik bir gider yok.
- Record'suz D2 hizmet faturası (₺12.000) hiçbir zaman gelir yazılmıyor (E0366, E0371–E0373
  ve bu rapor): **tanıma yok**.
- D3 kısmi tahsilat (₺5.000) **gelir** sayılıyor: **tahsilat anında gelir** — ADR
  0014'te taşıyan kayıt gelir üretmez.
- Kısmi tahsilatın borç kartına bağlanması (E0368/E0370) ve Record'suz borcun bakiyeye
  dokunmaması doğru gözlem olarak kalır; ancak rapor tarafında gelir/gider yalnız para
  hareketiyle oluşuyor (nakit esaslı ve borç akışını gelir/gider sayan model).

Formdaki P3-T daraltması (“ayrımı kayıt anında soran tek mekanizma; eşdeğerlik hükmü
verilmez”) bu kanıtla kesinleşir: soru bakiye hareketini sorar, tanımayı değil. İlgili
karar satırları ve sonuç cümlesi bu kanıta göre düzeltilir. Güncel B01–B19:
**13 kanıtla kapalı, 5 kapsamı sınırlı, 1 açık (B19)**.

Bekleyen kullanıcı kontrolleri (15 Eylül, kullanıcıya bildirildi):
- WL-U02 (2.1) plan ⋮ menüsü: **kanıtla kapandı.** İlk yüklenen `bulut yazılım
  aboneliği.png` menüyü kapalı gösterdiği için yeniden çekim istendi; kullanıcı eski
  dosyayı kaldırdı. WL-U02-A `kanitlar/wallet-budgetbakers/50-u02-plan-menu-postpone-dismiss.png`
  (özgün adı `bulut yazılım aboneliği (2).png`, 08:28): Bulut yazilim aboneligi plan
  detayı, 10.10.2026 / Due in 25 days satırında ⋮ açık — **Postpone** (takvim simgesi) ve
  **Dismiss** (çöp kutusu simgesi); altta 10.09.2026 Paid −600. Bekleyen örnekte erteleme
  ve reddetme seçeneklerinin varlığını kanıtlar; seçimlerin sonucu denenmedi (Dismiss'in
  örneği silip silmediği, Postpone'un tarih sorup sormadığı bilinmiyor). P3-G01 E0289 ve
  WL-Q08'deki “Postpone/Dismiss koşum notudur” sınırı kapandı.
- WL-U03 (2.2) Records ⋮ menüsü: **kanıtla kapandı.** WL-U03-A
  `kanitlar/wallet-budgetbakers/49-u03-records-menu-bakiye-planli-secenekleri.png`
  (özgün adı `record.png`, 08:20, aynı cihaz). Records, LAST 12 WEEKS; ⋮ menüsü açık ve
  yalnız iki seçenek: **Show balance in every record** (kapalı) ve **Show planned
  payments** (açık). Bu menüde filtre veya dışa aktarma seçeneği yok. Tur 1 K07'deki
  “Records menüsündeki seçenekler yalnız her kayıtta bakiye ve planlananları göster”
  notu artık kareli; P3-G04 E0374'teki “⋮ girişi var, içeriği bilinmiyor” sınırı kapandı.
  Filtre yalnız Settings → Filters (E0377) girişinde; dışa aktarma bu turda hiçbir yerde
  görülmedi. Arka plandaki liste E0372 ile aynı (tarihler artık “11 Eyl/10 Eyl”).
- WL-U04 (2.3) Settings → Automatic rules: **kullanıcı beyanı** — ücretsiz sürümde
  açılmıyor; görsel eklenmedi. Kullanıcı gözlemi olarak kaydedildi; kilidin biçimi
  (Premium ekranı vs. uyarı) bilinmiyor. WL-Q10'daki erişim çelişkisi kullanıcı
  gözlemiyle “ücretsiz sürümde kullanılamıyor” yönünde daraltıldı.

Forma işlenen WL-U düzeltmeleri (E0014): D2 sonrası notuna WL-U01 sonucu ve eşdeğer değil
hükmü; tek cümlelik sonuç; Tur 1 K07 satırına WL-U03 karesi; Automatic rules notuna WL-U04
kullanıcı beyanı; “Kanıt ve güven düzeyi”ne kullanıcı kontrolü satırı. **Karar etiketi
değişikliği:** “Borç kaydederken Record oluşturursan bakiyen değişir sorusu” satırı
`doğrudan al` → **`uyarlayarak al`** — öncül “model buradan doğru kuruluyor” WL-U01 ile
çürüdü; alınabilecek olan yalnız bakiye etkisini açıkça söyleyen dil.

## P4-B19 — Özetlere yayılan eski/yanlış iddiaların ortak kapanışı — 15 Eylül 2026

**Durum: kapandı.** Hedefler 13 Eylül denetim raporunun B19 bölümünden ve bu kaydın B19
yayılım listesinden alındı. B01–B18'in kendi yayılımları P1/P2/P3 paketlerinde yapıldığı
için tekrar edilmedi; bu paket yalnız B19'un adlandırdığı kalemleri formla uzlaştırdı.
Görsel açılmadı, emülatör kullanılmadı, ürün kodu değişmedi.

| Hedef (denetim kalemi) | Bulunan | Yapılan |
|---|---|---|
| E0386 karşılaştırma — KolayBi çek/senet “—” | Form: Finans modülünde çek ve senet; `d22-destek-cekler.png`, `d23-destek-senetler.png` | Hücre ✓ ve kare adlarıyla düzeltildi; Paraşüt/Logo hücrelerine form dayanağı yazıldı |
| E0386 — formlardan doğrulanmayan fiyatlar | Paraşüt “~150 TL/ay”, Logo “463 TL/ay” formlarda yok; formlar yalnız ücretli abonelik, 3 paket ve KolayBi PLUS kampanyasını söylüyor | Tutarlar kaldırıldı, “tutar formda doğrulanmadı” yazıldı |
| E0386 — Logo karesinden üç ürüne düzenle/sil yetkisi | Logo formu: müşteri adına işlem ürün sayfasında; kare boş şablon, ikon anlamı çıkarım. Paraşüt: anlık görüntüleme; KolayBi: çok müşterili erişim | Bölüm 4 yeniden yazıldı; yetki yalnız Logo'ya kaynaklı, diğerlerinde doğrulanmadı |
| E0386 — toplu “Resmî kaynak” etiketi | BF sütunu proje kodu/ADR, kayıt yüzeyleri manuel gözlem, karşılaştırmalar çıkarım | Başlıkta etiketler katmanlara ayrıldı; belge tarihsel özet olarak işaretlendi |
| E0386 — formla uzlaşmayan diğer hücreler (P4-B19 taraması) | Logo çoklu döviz, 17 banka, özet grafik, + Excel ve Paraşüt kredi kartı, 12 aylık projeksiyon formlarda yok; Excel Paraşüt KDV dışa aktarımına aitti | Hücreler “formda anılmıyor/doğrulanmadı” ya da form ifadesine göre düzeltildi; KolayBi kart hücresi Kredi Kartları sekmesiyle somutlaştı |
| E0003 DURUM — “Sıradaki Faz 8” | 12 Eylül tarihsel devir içindeki satır | “O tarihteki sıradaki adım” + tarihsel not; Faz 8 kullanıcı onayı bekler |
| E0003 DURUM — KolayBi/Goodbudget “TAMAM” | Faz 7.5 ilerleme tablosundaki 12 Eylül kapanışları; Wallet satırı hâlâ “Bekliyor” | Tablo altına tarihsel not; Wallet satırı P3-K ve WL-U sonucuyla güncellendi |
| E0003 DURUM — “Test kayıtları koşum sonrası silinir” | Tur 2 ek koşum kalemleri paragrafında | Üstü çizildi; 11 Eylül kararı ve tek kaynak SENTETIK-TEST-VERISI.md yazıldı |
| E0389 TUR2 — Wallet model eşdeğerliği genellemesi | P3-K'de notlandı; WL-U01 ile B09 kanıtla kapandı | Başlık WL-U01 sonucunu taşıyor; ek değişiklik gerekmedi |
| E0272 kanıt girişi — QuickBooks “Solopreneur/resmî kaynak” | Satır 30 onboarding + Simple Start ayrımını zaten taşıyor (P1-B13) | Değişiklik gerekmedi |
| E0388 veri kuralı | “Ek koşum kayıtları test sonrası silinmez” — doğru ve tek kaynak | Değişiklik gerekmedi; DURUM ona bağlandı |

Kalan sınır: DURUM ve TUR2'deki eski devir bölümleri silinmedi; tarihsel başlık veya satır
içi notla işaretli ve güncel dayanak bu kayıt, formlar ve belgelerin üst durum başlığıdır.
`raporlar/2026-09-13-kapsamli-yeniden-denetim.md` ve `raporlar/soru 2.md` sorunu tespit eden
tarihsel kaynaklar olarak değiştirilmedi.

Güncel B01–B19: **14 kanıtla kapalı, 5 kapsamı sınırlı, 0 açık.** P4-B19 ile Faz 8 geçiş
kapısının “B01–B19 için kapanış/sınırlandırma gerekçeleri yazılı” ve “düzeltmeler özet ve
durum ifadelerine yayılmış” maddeleri karşılandı; kapı kutuları P4-K'de ayrıca değerlendirilir.

**Sıradaki tek paket: P4-tema-01 (ürün kimliği ve kapsam).** Plan Bölüm 6'daki on tema
sırayla bulgu ve rapor bölümüne eşlenir; ardından P4-K geçiş kapısı ve kullanıcıya onay özeti.

## P4-tema-01 — Ürün kimliği, kapsam ve kanıt yöntemi haritası — 15 Eylül 2026

**Durum: kapandı.** Plan Bölüm 6 madde 1, 3, 4 ve 6'nın ilk teması. Dokuz formun oturum
bilgisi, kanıt tavanı, ürün kimliği/akış özeti ve tek cümlelik sonucu okundu; görsel
açılmadı, yeni doğrulama yapılmadı. Bu bölüm Faz 8'e taşınacak eşlemedir; rapor metni değildir.

### Kanıt yöntemi grupları (Belge 1 §1 inceleme sınırları, Belge 2 §1 kaynak asimetrisi)

| Grup | Uygulama (form) | Erişim ve kanıt tavanı | Görsel kanıt |
|---|---|---|---|
| A — canlı, derin | Bluecoins (E0005) | Yerel ücretsiz, giriş yok; Tur 1 + Faz 3 + Faz 7 emülatör koşumu | 90 kare (P2) |
| A — canlı, derin | Wallet (E0014) | Bulut hesabı, kullanıcı girişi; Tur 1 + Faz 2 + ek koşum + Faz 7; kullanıcı kontrolleri | 106 kare (P3) + WL-U01-A, U02-A, U03-A |
| B — canlı, Tur 1 + ek koşum | Money Manager (E0010) | Yerel, kayıt yok; Tur 1 eski PC İngilizce kareleri diskte yok, Faz 1 Türkçe yeniden koşum | 30 kare |
| B — canlı, Tur 1 + ek koşum | Hesap Defterim (E0007) | Yerel, kayıt yok, reklamlı; Tur 1 + ek koşum 2 | 45 kare |
| B — canlı, Tur 1 + ek koşum | Goodbudget (E0006) | Ücretsiz household (10 zarf, 1 hesap); kart/transfer erişim nedeniyle denenemedi | 28 kare + GB-U01-A/B/C |
| C — masa başı, derin | KolayBi (E0008) | Mobilde kayıt yok, web ücretli; giriş ekranı manuel, iç yüzey destek merkezi + video | 39 kare (destek mockup, video, 1 giriş) |
| D — masa başı, sınırlı | Paraşüt (E0011) | Hiç girilemedi; giriş öncesi karusel manuel, geri kalanı kılavuz + tanıtım videosu | 9 kare |
| D — masa başı, sınırlı | Logo İşbaşı (E0009) | Kayıt yapıldı ama hesap satış/hazırlık sürecine girdi; yalnız giriş/kayıt yüzeyi manuel | 6 kare |
| D — masa başı, sınırlı | QuickBooks Solopreneur (E0012) | Solopreneur ABD dışına kapalı; kareler QuickBooks mobil onboarding + QBO Simple Start, Solopreneur ekranı yok | 4 kare |

Belge 1/2 kuralı: A ve B davranış anlatabilir (sınırlarıyla); C arayüz alanlarını ve kaynak
akışını anlatır, davranışı doğrulanmış yazmaz; D yalnız ürünün kendi anlatımını ve görülen
giriş yüzeyini aktarır. Asimetri her iki belgenin girişinde açıkça yazılır (formlarda zaten var).

### Ürün kimliği ve kapsam eşlemesi

| Uygulama | Ürün tezi / hedef (kaynak tipi) | İşletme–şahsi | Rapor hedefi |
|---|---|---|---|
| Bluecoins | İleri seviye kişisel bütçe; çok hesap, kredi, ipotek, rapor (manuel gözlem çıkarımı) | Yok; kategori/etiket var | B1 §1, B2 §1 ve planlama/taksit temaları |
| Wallet | Kişisel/ortak hane finansı; Premium banka senkronizasyonu (manuel gözlem + ekran kartları) | Yok; label var | B1 §1, B2 §1; borç teması (B09 sonucu) |
| Money Manager | Kişisel/gündelik bütçe defteri; hızlı giriş (manuel gözlem) | Yok | B1 §1, B2 §1; kart ekstre teması |
| Hesap Defterim | Esnafın tek sütunlu yürüyen bakiye defteri; veresiye ve kategorili gelir-gider aynı geliştiricinin ayrı uygulamalarına bölünmüş (manuel gözlem, `42-drawer-diger-uygulamalar-veresiye-gelirgider.png`) | Yok | B1 §1 (ürün hattı bölme yaklaşımı), B2 §1 |
| Goodbudget | Zarf bütçeleme: harcamadan önce dağıt (manuel gözlem + GB-S01) | Yok | B1 §1, B2 §1 |
| KolayBi | Bulut ön muhasebe + e-dönüşüm; şahıs şirketi hedefte adıyla, proje bazlı gelir-gider ekseni (resmî kaynak) | Yok; proje ekseni ve ortak carisi var (kullanım amacı çıkarım) | B1 §1, B2 §1; cari/rapor temaları |
| Paraşüt | Bulut ön muhasebe + e-dönüşüm; KOBİ, e-ticaret (resmî kaynak) | Yok | B2 §1 (masa başı sınırıyla) |
| Logo İşbaşı | Mobil fatura + ön muhasebe; mikro işletme konumlandırması (kayıt formu manuel, gerisi resmî kaynak) | Yok | B2 §1; sesli fatura ve Müşavir Portal ürün yüzeyi olarak |
| QuickBooks Solopreneur | ABD Schedule C için işlem başına Business/Personal etiketi (yalnız yardım merkezi) | Var — vergi eksenli etiket; incelenen dokuz uygulama içinde kavramsal olarak en yakın | B2 §1; kapsam teması; Belge 3 açık soru adayı (split) |

Tema sonucu: incelenen dokuz uygulamanın hiçbirinde işletme ve şahsi parayı tek havuzda
tutup ayrımı raporlama boyutu yapan ürün görülmedi; en yakın kavram QuickBooks Solopreneur'ün
vergi eksenli etiketi ve yalnız resmî kaynağa dayanıyor. Bu, rakip anlatımıdır; Belge 3'te
ADR 0013 için karar dayanağı olarak değil bağlam olarak kullanılır.

### Faz 8'e taşınmaması veya nitelemeyle taşınması gereken ifadeler

| İfade (kaynak) | Neden | Kullanım |
|---|---|---|
| Hesap Defterim “4,8★ · 139 B yorum · 10 Mn+ indirme” (E0007 oturum bilgisi) | Karede yok; yaygınlık/memnuniyet iddiası (plan §8) | Belge 1/2'ye taşınmaz |
| KolayBi “40.000+ aktif işletme”, QNB ekosistemi (E0008) | Ürünün kendi beyanı; pazar verisi değil | Yalnız “ürünün beyanı” diye, gerekirse |
| QuickBooks “pazardaki en yakın kavramsal karşılık” (E0012 sonuç) | Pazar taraması yapılmadı | “İncelenen dokuz uygulama içinde” |
| QBO Simple Start “TRY 244,99/ay for 6 months” (E0012) | Solopreneur fiyatı değil; kampanya | Yalnız paket, TR mağaza, 2 Eylül 2026 ve kampanya nitelemesiyle; tercihen kullanılmaz |
| Paraşüt/Logo fiyat tutarları (eski E0386) | Formlarda yok; P4-B19'da kaldırıldı | Taşınmaz |
| KolayBi “ADR 0014'ün rakipteki en olgun karşılığı” (E0008 sonuç) | Masa başı çıkarım; B09'daki gibi rapor etkisi sınanmadı | Belge 3'e “arayüzde tahakkuk/nakit ayrımı görünüyor” diye, eşdeğerlik hükmü olmadan |
| Money Manager “bizim kart ve rapor tasarımımıza doğrudan girdi” (E0010 sonuç) | Karar dili; Belge 1/2 puanlamaz | Yalnız Belge 3 |
| Bluecoins “yapımıza en yakın” (DURUM/TUR2 tarihsel) | Model eşdeğerliği P2'de doğrulanmadı | Taşınmaz |
| Logo “dokuz uygulama içinde tek sesli fatura” (E0009) | Doğru ama kaynak ürün sayfası; davranış görülmedi | “İncelenen uygulamalar içinde yalnız Logo'nun kaynağında” |

Açık soru ve bağlar (sonraki temalara): KolayBi “tahmini dönem sonu bakiyesi bizde hiç yok”
ifadesi BF planlanan görünümüyle kodla karşılaştırılmadı → planlama teması. QuickBooks split
ile BF split yokluğu kodla doğrulandı (P3-T) → kapsam teması, Belge 3 açık soru adayı.

**Sıradaki tek paket: P4-tema-02 (gezinme ve özet/ana ekran).**

## P4-tema-02 — Gezinme, ana ekran/özet ve tutar gösterimi — 15 Eylül 2026

**Durum: kapandı.** Dokuz formun arayüz taraması ve arayüz incelemesi tabloları, KolayBi pano
bölümü, Paraşüt video kareleri ve QuickBooks onboarding incelemesi okundu. Görsel açılmadı.
Rapor hedefi ağırlıkla **Belge 1 §2** (bilgi hiyerarşisi, gezinme, ana ekran) ve **§3** (kart/liste/
grafik, renk, tutar gösterimi); boş durum **§4**'e de bağlanır. Kanıt grubu P4-tema-01 tablosundadır.

### Gezinme modeli ve ana ekranın ilk bloğu

| Uygulama | Ana gezinme | Ana ekran / özet ilk blok | Dayanak | Sınır |
|---|---|---|---|---|
| Money Manager | Kalıcı **alt çubuk** (İşlemler / İstatistik / Hesaplar / Daha); üstte ay gezgini + arama + filtre | Gün listesi; her ana ekranda aynı **Gelir / Gider / Toplam** üç sütun başlığı | Manuel gözlem; `10-takvim-gorunumu.png`, `26-toplam-sekmesi-agustos.png` | Widget denenmedi |
| Bluecoins | Üstte **kaydırılabilir çok sekme** + çekmece; sağ altta bağlama göre `+` | Düzenlenebilir **dashboard kartları**; işlem listesinde gün toplamı → satır → işlem sonrası bakiye | Manuel gözlem; `09-ozgun-ozellik.png`, `f7-33-menu2.png` | Çekmecede iki ayrı “Hesaplar” etiketi görülür; anlam farkı açılmadı |
| Wallet | Kalıcı alt çubuk yok; **geniş hamburger çekmece** + Home'da kısayol çipleri + ekran içi yatay sekmeler | Home: yatay **hesap kartları**, Records/Cash-flow/Planned payments/Debts çipleri; altında Premium, banka bağlantısı ve çapraz ürün tanıtım kartları | Manuel gözlem; `03-dolu-ana-ekran.png`, `f7-54-hamburger.png`, `f7-55-menu-scroll.png` | Menüdeki hedeflerin çoğu açılmadı |
| Hesap Defterim | Alt çubuk yok; **drawer (17 kalem)** + üstte **defter seçici**; altta sabit iki büyük eylem düğmesi | Gün gruplu liste; ekran altında sabit **Toplam Alındı / Toplam Ödendi / Denge**; grafik yok | Manuel gözlem; `36-drawer-menu-ust.png`, `13-ozet-tasarruf-agustos.png` | — |
| Goodbudget | Üstte **dört sabit sekme** (Envelopes / Transactions / Accounts / Reports); her ekranda `+` | **Zarf listesi**: satırda kalan ve bütçelenen iki sayı + ilerleme çubuğu | Manuel gözlem; `08-envelopes-filled-home.png` | Settings içi açılmadı |
| KolayBi | Web panel **sol menü** (mobil iç yüzey görülmedi) | **Güncel Durum panosu**: nakit akışı grafiği + Tahsilat/Ödeme Özetleri + Günü Gelen İşlemler; sağ sütun ve kampanya şeridi sürüme göre değişiyor | Resmî kaynak; `d24-destek-guncel-durum-panosu.png`, `d39-guncel-arayuz-2026.png` | Mobil gezinme ve güncel sürüm doğrulanmadı |
| Paraşüt | Masaüstü panel (video) | **Güncel Durum**: Tahsilatlar / Ödemeler donutları, gecikmiş tahsilat vurgusu | Resmî kaynak (tanıtım videosu); `04-video-cari-hesap-durumu.png` | Pazarlama karesi; mobil ana ekran görülmedi |
| Logo İşbaşı | Görülmedi (yalnız giriş/kayıt) | — | Manuel gözlem giriş yüzeyi | İç yüzey hiç görülmedi |
| QuickBooks Solopreneur | Görülmedi | İşlem listesi merkezli; `Type` ve `Category` ayrı sütun | Yalnız resmî kaynak | Solopreneur ekranı yok |

Gözlenen yaklaşım aileleri (Belge 1 §2): (1) sabit alt çubuk ve her ekranda aynı özet satırı
(Money Manager); (2) sabit üst sekmeler (Goodbudget) veya kaydırılabilir üst sekmeler (Bluecoins);
(3) çekmece ağırlıklı, ana ekranda kısayol/kart (Wallet, Hesap Defterim); (4) web panoda nakit
akışı + tahsilat/ödeme özetleri (KolayBi, Paraşüt; resmî kaynak). İlk bloğu doğrudan hesap
bakiyeleri (Wallet, Bluecoins kartları), zaman sıralı işlem listesi (Money Manager, Hesap
Defterim), bütçe zarfları (Goodbudget) veya tahsilat/ödeme vadeleri (KolayBi, Paraşüt) oluşturuyor.

### Renk ve tutar gösterimi (Belge 1 §3)

| Uygulama | Gelir / gider / nötr renk | Tutar biçimi | Dayanak |
|---|---|---|---|
| Money Manager | Gelir mavi, gider turuncu-kırmızı, net/transfer siyah | Sağa hizalı, iki ondalık, ₺ ön ek; kart borcu Hesaplar'da işaretsiz kırmızı, defterde eksi | `12-hesaplar-kart-borcu-bu-ay.png`, `31-kart-defteri-agustos-hareketler.png` |
| Bluecoins | Gelir yeşil, gider pembe/kırmızı, transfer mavi | Sağda, iki ondalık, ₺; gün toplamı ile satır ayrışıyor | P2 kareleri |
| Wallet | Seçili tür yalnız zemin tonuyla; negatifler `-TRY`/`−₺` | Büyük, sağa hizalı; Tur 1'de “TRY 20,800.00”, Faz 2'den sonra “₺20.200,00” biçimi | `04-islem-formu.png`, `20-b1-sonrasi-ana-hesap-20200.png` |
| Hesap Defterim | Alındı yeşil, Ödendi kırmızı, Denge işaretine göre | Tam sayı, binlik ayraç, simge yok | `02-bos-ana-ekran.png` |
| Goodbudget | Gelir yeşil ve +; gider koyu | ABD biçimi `25,000.00`, simge yok; tarih MM/DD/YYYY | `08-envelopes-filled-home.png` |

### Boş durum (Belge 1 §4)

- Ne işe yaradığını ve ilk eylemi söyleyen boş durum: Wallet Planned payments ve Debts
  (`09-ozgun-ozellik.png`, `34-debts-bos-durum.png`), Bluecoins karşılama ve “İlk İşlemi Ekle”
  (`01-ilk-acilis.png`, `10-fresh-bos-ana-ekran.png`).
- Yalnız veri olmadığını söyleyen boş durum: Money Manager “Veri yok.” + illüstrasyon
  (`02-bos-ana-ekran.png`), Goodbudget rapor “No transactions found.” (`15-report-empty-state.png`),
  Hesap Defterim başlık satırı + sıfır toplamlar (`02-bos-ana-ekran.png`), Bluecoins boş Çöp Kutusu
  (`f7-38-cop-kutusu.png`).

### Taşıma nitelemeleri

| İfade (form) | Kullanım |
|---|---|
| Wallet “çekmecedeki çok sayıda hedef bilişsel yük oluşturuyor”; Bluecoins “iki Hesaplar etiketi yönü belirsizleştiriyor”; erişilebilirlik satırları | Statik ekran yorumu; kullanılabilirlik veya erişilebilirlik ölçümü değil (plan §8). Belge 1'de “görülen yapı” olarak, sonuç iddiası olmadan |
| Wallet “Money Manager'ın ‘No data’ yaklaşımından daha yönlendirici” | Karşılaştırmalı değerlendirme yerine iki yaklaşım betimlenir (yukarıdaki boş durum listesi) |
| Goodbudget konuma göre payee önerisi ve tebrik mikro metni “başka uygulamada görülmedi” | “İncelenen dokuz uygulama içinde” nitelemesiyle |
| Money Manager Korece reklam gözlemi | Karesi yok; taşınmaz |
| KolayBi/Paraşüt pano yapısı | Resmî kaynak/pazarlama karesi; davranış veya güncel sürüm iddiası olmadan |

Forma yayılan düzeltme (E0014): Wallet arayüz taraması tablosundaki “Tüm ana sekmeler”, “Ayarların
derinliği” ve “Arama ve filtre” satırları P3-G04 ve WL-U kanıtıyla güncellendi (çekmece alt öğeleri,
Settings/Advanced settings ve Records menüsü artık kareli).

**Sıradaki tek paket: P4-tema-03 (işlem girişi, formlar ve geri bildirim).**

## P4-tema-03 — İşlem girişi, formlar ve geri bildirim — 15 Eylül 2026

**20 Eylül eki — GB-Q01/K3:** From New Income / Fill Each Envelope yolu artık yalnız
form değildir: E0417–E0424 ile zarf +1.234, hesap değişmedi (41.734), Eylül Income 3.284
sonucu gözlendi. Önceki 2.050 raporu önceki koşumdandır; Initial Fill'in eski fonlama seçimi,
Keep Available sonucu ve hesap bakiyesinin değişmeme nedeni bilinmiyor. Aşağıdaki 15 Eylül
karşılaştırmaları kendi koşumlarına aittir; güncel kapanış dosyanın sonundadır.

**Durum: kapandı.** K00–K08 görev tabloları, işlem formu bölümleri, KolayBi gider/fatura formları,
Paraşüt kılavuz pipeline'ı, Logo kayıt/sesli fatura ve QuickBooks onboarding bölümleri okundu.
Görsel açılmadı. Rapor hedefi **Belge 1 §4** (formlar, seçimler, geri bildirim, boş/hata durumları)
ve **Belge 2 §2** (gelir/gider girişi, kayıt sonrası görünür sonuç). Kanıt grubu P4-tema-01'de.

### Form yapısı ve giriş kolaylıkları

| Uygulama | Form yapısı | Tür seçimi / varsayılan | Giriş kolaylığı | Dayanak | Sınır |
|---|---|---|---|---|---|
| Money Manager | Tek ekran, beş alan (Tarih · Tutar · Kategori · Hesap · Not) + Detay/kamera; tutar için hesap makinesi tuş takımı; sağ üstte Tekrar/Taksit | Gelir / Gider / Havale segmenti | **Devam et** ile seri giriş; not girilince liste başlığı notu gösteriyor | Manuel gözlem; `04-islem-formu-ve-kategori.png`, `06-islem-listesi-transfer-notr.png` | Gelir kategori ızgarası Türkçe koşumda kareli değil |
| Bluecoins | **Tek yoğun ekran**: ad, tarih/saat, Planlı İşlemler, tutar (hesap makinesi), kategori, hesap, kartta taksit, Bölmek, Durum, Etiket, Not; altta GİDER / GELİR / TRANSFER | Seçili tür dolu zemin + tutar işareti; varsayılan GİDER koşum notu | Aynı formda planlama, taksit ve bölme | Manuel gözlem; `04-islem-formu.png`, `12-kart-gideri-taksit-alani.png`, `13-transfer-formu.png` | Hesap makinesi widget'ının donması emülatör tuzağı |
| Wallet | **İki katman**: hızlı form (tür, tutar tuş takımı, hesap, kategori) → kayıt sonrası Record detail (Note, Labels, Payee, Date/Time, Payment Type, Warranty, Status, Place, Attachments) | Varsayılan Expense; seçili tür yalnız zemin tonuyla; gider formunda gelir kategorisi seçilebiliyor | Templates (FAB'da “Create first template”); Split | Manuel gözlem; `04-islem-formu.png`, `21-b2-islem-detay-taksit-alani-yok.png`, `f7-03-fab-menu.png`, `f7-04-transfer-form.png`, `38-split-record-ekrani.png` | Şablon ve split kaydedilmedi |
| Hesap Defterim | Tek ekran: tür (Alındı / Ödendi) + tarih + saat + tutar (hesap makinesi) + Notlar (sesli giriş) + isteğe bağlı Açıklama/Kategori metni + Fatura ekle + Öğe eklemek | Ana ekranda iki büyük renkli düğme ayrı giriş | **Kaydet ve devam Et**; kalem dökümü (Öğe eklemek); not önerileri; buton etiketleri yeniden adlandırılabiliyor | Manuel gözlem; `04-islem-formu-alindi.png`, `20-oge-eklemek-dialog.png`, `05-siniflandirma-islem-adlari.png` | Kategori ve müşteri alanı yok |
| Goodbudget | Tek ekran Add Transaction: Payee · Amount + tür · Envelope · Account · Date · Check # · Schedule · Notes; tutar için tam ekran hesap makinesi; Account Transfer ayrı ekran | Tür seçimi formda | Quick Transactions widget seçeneği; konuma göre payee önerisi | Manuel gözlem; `09-credit-type-selected.png`, `12-account-transfer-screen.png`, `11-save-location-prompt.png` | Normal gelir yolu ayrı Fill Envelopes (GB-U01) |
| KolayBi | Web “Yeni Genel Gider”: Cari Takibi Yok/Var, Ödeme Durumu Ödenmedi/Ödendi, Son Ödeme Tarihi (boş bırakılabilir), Gider Tipi*, Proje (ayarlardan kapatılabilir), açıklama şablonu, iki para birimi alanı, dosya yükleme, KDV bandı; fatura formu cari zorunlu + kalem tablosu | Form türü menüden (gider / fatura / iade) | Açıklamayı şablon kaydetme; iki seviyeli gider tipi | Resmî kaynak; `d05-destek-yeni-gider-formu.png`, `d06-destek-gider-tipleri.png`, `d13-destek-alis-faturasi-formu.png`, `d30-destek-satis-fatura-formu.png` | Davranış ve kayıt etkisi doğrulanmadı |
| Paraşüt | Beş gider türü (detaylı/hızlı fiş-fatura, maaş/prim, vergi/SGK, banka gideri); kayıt ile ödeme ayrı adım; fiş fotoğrafından OCR | — | Mobil fiş okutma (ürün sayfası) | Resmî kaynak (kılavuz) | Form ekranı görülmedi |
| Logo İşbaşı | İç form görülmedi; kayıt formu üç alan + SMS + sözleşme | — | Sesle fatura kesme (ürün duyurusu) | Kayıt yüzeyi manuel; özellik resmî kaynak | Sesli giriş çalışırken görülmedi |
| QuickBooks Solopreneur | İş adı tek alan + “No business name? Use your name”; işlem girişi banka bağlantısıyla otomatik iniş ağırlıklı | Business/Personal etiketi işlem başına | Otomatik kategorileme | Onboarding manuel; iş akışı yalnız yardım merkezi | Solopreneur formu görülmedi |

Yaklaşım aileleri (Belge 1 §4): az alanlı tek ekran + seri giriş (Money Manager, Hesap Defterim);
tüm seçenekleri tek yoğun ekranda sunma (Bluecoins; KolayBi web formları); hızlı form + kayıt
sonrası ayrıntı (Wallet); zarf merkezli form (Goodbudget); kaynakta anlatılan otomatik içe aktarım
(QuickBooks) ve sesle giriş (Logo).

### Hata, başarı ve düzeltme geri bildirimi

| Durum | Gözlenen | Dayanak |
|---|---|---|
| Zorunlu alan / geçersiz tutar — alan dışı mesaj | Money Manager “Lütfen hesabı seçiniz.” toast; Wallet sıfır tutarda “Please fill in the amount.” snackbar; Goodbudget Credit yolunda “Select an Envelope.” toast | `08-hata-toast-hesap-sec.png`, `08-hata-veya-bos-durum.png` (Wallet), `10-income-requires-envelope.png` |
| Zorunlu alan — alan yanında mesaj | Wallet planlı ödemede Category kırmızı “Select category” | `f7-26-d1-saved.png` |
| Sessiz davranış | Hesap Defterim boş tutarla kaydetme sessiz no-op, ₺0 kabul; formdan geri çıkışta taslak uyarısı yok (12 Eylül'de yeniden denendi) | `28b-bos-tutar-sessiz-red.png`, `28-sifir-tutar-kabul-edildi.png` |
| Başarı | Sessiz dönüş (Money Manager, Wallet, Bluecoins); kısa toast “İşlem Eklendi” (Hesap Defterim); oyunlaştırılmış tebrik (Goodbudget) | İlgili form tabloları |
| Düzeltme | Satır/detayda doğrudan düzenleme (Money Manager, Wallet, Goodbudget); Hesap Defterim'de başka deftere taşıma ve kopyalama | `18-delete-confirmation.png` (Goodbudget), `25-fatura-tam-ekran-goruntuleme.png` (Hesap Defterim) |
| Silme | Onay diyaloğu (Bluecoins, Goodbudget); Hesap Defterim'de çöp kutusu + Geri Yükle | `08-hata-veya-bos-durum.png` (Bluecoins), `18-delete-confirmation.png`, `08-silinmis-islemler.png` |

### Taşıma nitelemeleri

| İfade (form) | Kullanım |
|---|---|
| Money Manager ve Wallet “silme tek onaydan sonra kalıcı, geri alma yok” | İkisinde de silme onayı veya sonucu karede yok; koşum notu olarak, ölçülmüş davranış diye değil |
| Wallet “transfer formu son yönü korumadı” | Koşum notu; `f7-06-transfer-form2.png` Ana Hesap → Is Karti ön seçili görünüyor, yön hatırlama davranışı ölçülmedi |
| Bluecoins “varsayılan GİDER”; Money Manager “0,00 sessiz kabul” ve taslak uyarısı yok | Koşum notu, karesi yok |
| Money Manager “başlık = not, BF `title = açıklama ?? kategori` ile birebir”; “CreditCardCharge kuralıyla aynı” | Karar/kıyas dili; yalnız Belge 3 |
| KolayBi Proje anahtarı “bağımsız olarak aynı çözüme varılmış” | Çıkarım; Belge 1/2'de alan ve not metni betimlenir |
| Logo sesli fatura “12 Eyl'de doğrulandı” | Kaynakla doğrulandı, çalışırken görülmedi; form başlığı daraltıldı |
| Wallet “seçili türün yalnız renkle belirtilmesi yanlış seçim riski” | Statik ekran yorumu; otomasyon sırasındaki Income kayması (E0338) kullanıcı hatası ölçümü değildir |

Forma yayılan düzeltme (E0009): Logo sesli fatura başlığındaki “doğrulandı” ifadesi kaynak
doğrulamasıyla sınırlandı.

**Sıradaki tek paket: P4-tema-04 (hesap, kart ve transfer).**

## P4-tema-04 — Hesap, kart ve transfer — 15 Eylül 2026

**Durum: kapandı.** Money Manager kart/ödeme/açılış bölümleri, Bluecoins kart/transfer/kısmi ödeme,
Wallet kart modeli ve Faz 7 A, Hesap Defterim açılış bakiyesi, KolayBi Finans modülü ile P4-tema-03'te
okunan K02/K05/K06 satırları kullanıldı. Görsel açılmadı. Rapor hedefi **Belge 2 §2** (hesap/transfer/
kart işlemleri) ve **Belge 1 §3** (hesap ve kart gösterimi).

### Hesap türleri ve açılış bakiyesi

| Uygulama | Hesap yapısı | Açılış bakiyesi davranışı | Dayanak | Sınır |
|---|---|---|---|---|
| Money Manager | Hazır hesaplar; Nakit / Banka Hesapları / Kredi Kartı grupları; para birimi hesap başına | Hesap “Tutar” alanı; kaydedince fark “Bakiye Farkı” olarak hesabın defterine bugünün tarihiyle yazılıyor; “İşlemler bölümünde gösterilsin mi?” sorusunda **HAYIR seçilen koşumda** ana İşlemler akışına ve gelir raporuna girmiyor; geçmiş tarihe konamıyor | `09-kart-hesap-ekstre-modeli.png`, `15-acilis-bakiye-farki-dialog.png`, `16-acilis-bakiye-farki-defter.png` | — |
| Bluecoins | Banka / Nakit / Kredi Kartı / cari hesap grupları; hazır sıfır bakiyeli örnek hesaplar | Yeni hesap formunda **Başlangıç bakiyesi + Açılış tarihi** aynı ekranda | `03-dolu-ana-ekran.png`, `32-kart-hesap-kesim-gunu-limit-alanlari.png` | Ay öncesine açılış girilememesinin nedeni ölçülmedi (BC-Q07) |
| Wallet | Manual Input: Cash, Checking, Credit card, Savings, Loan, Mortgage, overdraft; Ana Hesap türü “General”; transferde “…outside of Wallet” hedefi | Cash/checking oluştururken açılış alanı yok (koşum notu); kart detayında bakiye düzenleme girişi var, etkisi denenmedi | `25-kart-hesap-ayarlari.png`, `24-kart-hesap-detay-negatif-bakiye.png`, `f7-07-tutar-400.png` | Açılış alanı yokluğu karesiz |
| Hesap Defterim | Hesap türü yok; her “hesap” ayrı yürüyen bakiyeli **defter**; liste bakiye göstermiyor | Tarihli “Açılış bilançosu”: dönem 1 Ağustos'u kapsıyorsa Alındı satırı ve **Toplam Alındı'ya giriyor**, kapsamıyorsa ayrı “Önceki denge” satırı | `10-hesap-eklem-formu.png`, `11-hesaplar-defterler-listesi.png`, `17-onceki-denge-gunluk-gorunum.png`, `03-dolu-ana-ekran.png` | — |
| Goodbudget | Checking/Savings/Cash · Credit Card · Debt grupları; **Accounts katmanı varsayılan kapalı**; ücretsiz sürümde 1 hesap | Hesap formu karelerde yok | `04-accounts-off-by-default.png`, `07-account-limit-paywall.png` | Açılış davranışı görülmedi |
| KolayBi | Finans: Banka Hesapları / Kasalar / Kredi Kartları / Online Banka / Çekler / Senetler; her listede Açılış Tarihi + Bakiye kolonları | Liste kolonlarında açılış tarihi; davranış görülmedi | Resmî kaynak; `d19-destek-banka-hesaplari.png` | Video karesinde dört sekme vardı; eklenme zamanı bilinmiyor |
| Paraşüt | Kasa ve Bankalar tek listede, IBAN ve döviz cinsi sütunu, “Banka hesabı bağla” | — | Resmî kaynak (video); `05-video-banka-entegrasyonu.png` | Pazarlama karesi |

### Kredi kartı modeli ve kart ödemesi

| Uygulama | Kart modeli | Kart ödemesi ve kısmi ödeme | Dayanak | Sınır |
|---|---|---|---|---|
| Money Manager | **Ekstre dönemli**: Kaynak (ödeme hesabı), Hesap Kesim Tarihi, Son Ödeme Tarihi; Hesaplar'da **Bu Ay** (kesilmiş dönem) ve **Gelecek Ay** (kesilmemiş) iki sütun; kart defterinde Para Yatırma / Çekme sütunları ve satır başına yürüyen bakiye | Kart defterinde ayrı **Ödeme** düğmesi = borç tutarıyla ön doldurulmuş havale; tutar düzenlenebilir. ₺400 kısmi ödeme Bu Ay 1.000 → 600, Gelecek Ay 1.000 değişmedi | `09-kart-hesap-ekstre-modeli.png`, `12-hesaplar-kart-borcu-bu-ay.png`, `31-kart-defteri-agustos-hareketler.png`, `13-odeme-butonu-onfoldurulmus-havale.png`, `24-kismi-kart-odemesi-400.png`, `25-kismi-odeme-sonrasi-borc.png` | Kısmi ödemenin yalnız kesilmiş ekstreye uygulanması tek denemedir |
| Bluecoins | Kart hesabı alanları: Kredi Limiti, Hesap Kesim Günü, Bitiş tarihi, Başlangıç bakiyesi; kart gideri formunda taksit alanı | Ödeme hesaplar arası transfer; 500 TRY kısmi ödeme Ana Hesap 40.200 → 39.700, kart −1.000 → −500 | `32-kart-hesap-kesim-gunu-limit-alanlari.png`, `12-kart-gideri-taksit-alani.png`, `34-kismi-kart-odemesi-500-transfer.png` | Dönem/ekstre davranışı doğrulanmadı (BC-Q06); ayrı ödeme akışının yokluğu çıkarılmaz |
| Wallet | **Dönemsiz negatif bakiye**: limit, Balance Display Options (Available Credit), tek Payment Due Date; ekstre yok | Ayrı ödeme akışı yok; serbest tutarlı transfer. ₺400: Ana Hesap 15.200 → 14.800, kart −6.000 → −5.600; bakiye eksiye inince eşik uyarısı | `25-kart-hesap-ayarlari.png`, `f7-12-tutar-final.png`, `f7-13-a-kaydedildi.png`, `23-b2-6000-tek-kart-borcu.png` | Payment Due Date'in işlevi açılmadı |
| Hesap Defterim | Kart kavramı yok; kart = eksiye giden defter | Aktar ile defterler arası; ₺400 kısmi ödeme jenerik aktarım | `06-islem-listesi.png` | Kart borcu/limit/ekstre yok |
| Goodbudget | Credit Card hesap grubu var | Kart hesabı açılamadı (1 hesap limiti); kart gideri sıradan hesaba yazıldı | `07-account-limit-paywall.png` | Kart modeli hiç görülemedi |
| KolayBi | Kredi Kartları listesi ve formu: Hesap Kesim Günü*, Son Ödeme Günü*, Kart Limiti*, Minimum Ödeme Oranı (%)*, Kalan Limit kolonu | — | Resmî kaynak; `d20-destek-kredi-kartlari-listesi.png`, `d21-destek-yeni-kredi-karti-formu.png` | Tablo boş; Kalan Limit hesaplaması çıkarım |
| Paraşüt, Logo, QuickBooks | Formlarda kredi kartı modeli anlatılmıyor | — | — | Doğrulanmadı |

### Transfer

| Uygulama | Transfer gösterimi | Rapor etkisi | Dayanak |
|---|---|---|---|
| Money Manager | Ayrı **Havale** segmenti; tek nötr siyah satır “Ana Hesap → Ortak Cuzdan” | Gün başlığı ₺0,00; gelir/gidere girmiyor | `06-islem-listesi-transfer-notr.png` |
| Bluecoins | TRANSFER türü; transfer ücreti ve yön değiştirme ikonu; iki bağlı bacak ve satır başına işlem sonrası bakiye; detayda “Yinelenmek” | Transfer bacaklarının neti sıfır; gün başlığı başka kayıtlar varsa sıfır olmayabilir | `13-transfer-formu.png`, `14-islem-listesi-running-bakiye.png` |
| Wallet | Transfer sekmesi, From/To; listede iki satır (+/−), ikisi de “Transfer, withdraw” | Haftalık toplam ve Cash-flow'a etkisi 0 | `06-islem-listesi.png`, `12-kontrol-records-listesi.png`, `48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png` |
| Hesap Defterim | Aktar: Miktar + Kimden + Kime; iki ayrı defter satırı | **Toplam Alındı / Toplam Ödendi'ye giriyor**; gelir/giderden ayrışmıyor | `12-aktar-transfer-formu.png`, `06b-islemler-butun-hesaplar.png` |
| Goodbudget | Account Transfer ayrı ekran (From / To / Amount / Description / Date / Schedule / Notes) | Tek hesap nedeniyle gerçek transfer yapılamadı | `12-account-transfer-screen.png`, `13-transfer-single-account-blocked.png` |

Yaklaşım aileleri (Belge 2 §2): kart borcu **ekstre dönemli** ve özel ödeme düğmeli (Money Manager);
alanlar var ama dönem davranışı görülmedi (Bluecoins, KolayBi kaynak); **dönemsiz negatif bakiye**
(Wallet); **kart kavramı yok** (Hesap Defterim). Transfer üç üründe gelir/giderden ayrı ve raporda
nötr, Hesap Defterim'de gelir/gider toplamlarına karışıyor. Açılış bakiyesi dört farklı biçimde ele
alınıyor: rapora girmeyen bakiye farkı hareketi (Money Manager), hesap formunda tarihli başlangıç
(Bluecoins), alan yokluğu (Wallet, koşum notu), dönem filtresine göre gelir satırına dönüşen açılış
(Hesap Defterim).

### Taşıma nitelemeleri

| İfade (form) | Kullanım |
|---|---|
| Money Manager “Bu tam olarak bizim ekstre projeksiyon modeli”, “CreditCardPayment modelimizle birebir”, “kısmi ödeme bizim D3 modelimizle uyumlu” | Kıyas/karar dili; yalnız Belge 3. Belge 2'de davranış betimlenir |
| KolayBi “CreditCard modelimize en yakın rakip tanımı”, “Açılış Tarihi + Bakiye = OpeningBalance”, senet “DebtAgreement'ın karşılığı” | Resmî kaynak kolon adlarından kıyas; yalnız Belge 3 ve çıkarım etiketiyle |
| Money Manager “kısmi ödeme yalnız kesilmiş ekstreye uygulanıyor” | “₺400'lük tek denemede” nitelemesiyle |
| Money Manager “kart için Para Yatırma/Çekme biraz zorlama” | Değerlendirme; Belge 1'de sütun adları betimlenir |
| Money Manager “silme tek onay → kalıcı” (Faz 1 doğrulama listesi) | Karesi yok; koşum notu |
| Wallet “minimum bakiye alarmı” (eski) | P3-G01'de düzeltildi: bakiye eksiye inince eşik uyarısı; eşiğin tanımı bilinmiyor |

**Sıradaki tek paket: P4-tema-05 (işletme/şahsi sınıflandırma).**

## P4-tema-05 — İşletme/şahsi sınıflandırma ve ek raporlama eksenleri — 15 Eylül 2026

**Durum: kapandı.** Dokuz formun K04 satırları ve işletme/şahsi yaklaşım satırları (P4-tema-01/03'te
okundu), QuickBooks sistem işleyişi ve karar tablosu, KolayBi proje ve personel/ortak carisi
bölümleri, dokuz formun karar tablolarındaki kapsam/proje/ortak satırları ve Logo'daki ilgili ifadeler
kullanıldı. Görsel açılmadı. Rapor hedefi **Belge 2 §2** (kapsam ayrımı) ve **Belge 1 §4** (sınıflandırma
alanının formda ve listede sunumu); Belge 3 için açık soru adayları ayrıca listelendi.

### Ürün başına yaklaşım

Bu tablodaki yokluk ifadeleri incelenen sürüm, yüzey ve kaynaklarla sınırlıdır; ürünün bütün paketlerinin tarandığı anlamına gelmez.

| Uygulama | İşletme/şahsi ayrımı | Varsa yakın araç | Dayanak | Sınır |
|---|---|---|---|---|
| QuickBooks Solopreneur | **İşlem başına tek `Type` alanı: Business / Personal**, üçüncü değer yok; `Type` ve `Category` ayrı sütun; listede `Type`'a göre süzme; şahsi işaretlenen işlem işletme/vergi raporundan düşer, silinmez | **Split transaction**: tutarı parçalara bölüp her parçaya ayrı Business/Personal ve kategori; araç/yakıt için bölmeme istisnası; `Rules` ile otomatik etiketleme; `Exclude` | Yalnız resmî kaynak (Intuit yardım merkezi); onboarding'de “No business name? Use your name” `03-onboarding-basic-info.png` | Solopreneur arayüzünün hiçbir ekranı görülmedi |
| KolayBi | Kapsam alanı yok | **Kullanıcı tanımlı Proje ekseni** (kod, ad, para birimi, tarih, Aktif/Pasif; listede Gelir / Gider / Net); formlarda Proje alanı ve “Ayarlar sayfasından Proje Takip seçeneğini kapatabilirsiniz” notu; ayrıca Etiketler, Şube (rapor) ve **Ortaklar / Personel Carileri** | Resmî kaynak; `d01-destek-proje-listesi.png`, `d02-destek-yeni-proje-formu.png`, `d03-destek-proje-detay-ozet.png`, `d05-destek-yeni-gider-formu.png`, `d07-destek-cari-listesi.png`, `d26-destek-alis-satis-raporu.png` | Proje ve ortak carisinin patron parası ya da hane gideri için kullanıldığı çıkarımdır; demo proje adları (Ev Elektrik, Bebek Bakım) ihtiyaç kanıtı değildir (B08) |
| Paraşüt | Yok | Ortak/personel carisi üzerinden dolaylı giriş olasılığı | Resmî kaynak; dolaylı yol `çıkarım` (kılavuzda anlatılmıyor) | — |
| Logo İşbaşı | Yok | Ortak cari / çekilen para üzerinden dolaylı giriş olasılığı | Resmî kaynak; dolaylı yol `çıkarım` | — |
| Money Manager | Yok | Kapsam alanı, işletme kategorisi veya etiket bulunmuyor; kategori listesi kişisel | Manuel gözlem; `04-islem-formu-ve-kategori.png` | — |
| Bluecoins | Yok | Kategori ve etiketler; etiket listesinde “İş” ve “Kişisel” adlı değerler var | Manuel gözlem; `f7-37-etiketler.png`, `04-islem-formu.png` | Etiketlerin kapsam boyutu olarak kullanıldığı çıkarılmaz; koşumda taklit edilmedi |
| Wallet | Yok | Labels var | Manuel gözlem; kapsam alanı yokluğu koşum notu (P3-G01: `05-siniflandirma.png` rapor karesi) | Label ile taklit edilmedi |
| Hesap Defterim | Yok | Tek dolaylı yol ayrı bir **defter** açmak; buton etiketlerini yeniden adlandırma | Manuel gözlem; `11-hesaplar-defterler-listesi.png`, `05-siniflandirma-islem-adlari.png` | İkinci defterle ayrım denenmedi |
| Goodbudget | Yok | Zarflar sınıflandırma sağlıyor | Manuel gözlem; `08-envelopes-filled-home.png` | — |

Tema sonucu (Belge 2 §2): canlı incelenen beş üründe ve kaynakla incelenen üç Türk ön muhasebe
ürününde işletme ile şahsi parayı kayıt düzeyinde ayıran bir alan görülmedi. Kayıt başına ayrım
yalnız QuickBooks Solopreneur'ün yardım merkezinde anlatılıyor ve oradaki ayrım ABD vergi formuna
hizalı; ayrıca tek kaydı iki tarafa bölme (Split) sunuluyor. Türk ön muhasebe ürünlerinde ek
raporlama ekseni olarak kullanıcı tanımlı proje (KolayBi) ve ortak/personel carileri görülüyor;
bunların şahsi para için kullanıldığı kaynakta anlatılmıyor.

### Belge 3 için açık soru adayları (karar değil)

| Aday | Kaynak karar etiketi | Not |
|---|---|---|
| QuickBooks **Split transaction** — tek harcamayı işletme/şahsi parçalara bölme | `kararı yeniden sor` (E0012) | Yalnız resmî kaynak. BF'de işlem bölme kodda yok (P3-T); kapsam kayıt düzeyinde tek değer. Adı konmuş boyut formda yazılı |
| KolayBi **kullanıcı tanımlı proje ekseni** ile sabit iki değerli kapsam | `henüz karar verme` (E0008) | Demo adları ihtiyaç kanıtı değil; kullanım amacı bilinmiyor |
| KolayBi **Ortaklar/Personel carisi** ile patron parasının borç-alacak olarak izlenmesi | `alma` (E0008) | Kullanım yolu çıkarım; ADR 0013'ün tersi ödünleşim |

### Taşıma nitelemeleri

| İfade (form) | Kullanım |
|---|---|
| QuickBooks “`TransactionScope` ile birebir aynı”, “pazardaki en güçlü doğrulayıcı referans” (E0012 karar tablosu) | Kıyas ve pazar iddiası; Belge 3'te “incelenen uygulamalar içinde yardım merkezinde anlatılan en yakın kavram” diye, Belge 1/2'ye taşınmaz |
| KolayBi Proje anahtarı “bizim onboarding cevabımızla aynı desen, bağımsız olarak aynı çözüme varılmış” | `çıkarım`; Belge 3 |
| “Patronun şahsi harcaması ortak/personel carisi üzerinden girer” (Paraşüt, Logo, KolayBi) | Kaynaklarda anlatılmıyor; yalnız çıkarım etiketiyle ve kullanıcı davranışı iddiası olmadan |
| KolayBi demo proje adları (Ev Elektrik, Ev Su, Bebek Bakım) | Destek materyalindeki örnek adlar; hane ihtiyacı veya amaç dışı kullanım kanıtı değil (B08) |
| Money Manager, Bluecoins, Wallet, Hesap Defterim karar gerekçelerinde “ADR 0013'ün temel ihtiyacını karşılamıyor” | Belge 3 dili; Belge 2'de “kapsam alanı yok” betimlenir |
| Hesap Defterim “ikinci defter bakiyeyi böler, raporu bölmez” | Denenmedi; değerlendirme olarak Belge 3 |
| Bluecoins etiket listesindeki “İş” / “Kişisel” | Etiket değerlerinin varlığı; kapsam boyutu kullanımı iddiası taşınmaz |

**Sıradaki tek paket: P4-tema-06 (planlama, tekrar ve taksit).**

## P4-tema-06 — Planlama, tekrar ve taksit — 15 Eylül 2026

**20 Eylül eki — MM-Q03/K2:** Kalan dört taksit Ekim–Ocak gün listelerinde ve aylık
1.000 gider toplamlarında bulundu (E0407–E0411); tekrar −600 ayrı önizlemede ve toplama
girmiyor. Ocak kart defteri Bakiye 5.600 ile Hesaplar Borçlar 1.600 farklı dönem kapsamlarıdır
(E0413/E0414). Görünür yer sorusu kapandı; üretim eşiği ve saklama biçimi açıklanmadı (B12).
Aşağıdaki 15 Eylül bulguları kendi koşumlarıdır; dosya sonunda güncel kapanış vardır.

**Durum: kapandı.** Money Manager B1/B2, Bluecoins P2-G02 ve karar tablosu,
Wallet B1/B2, bütçe/hedef ve plan yönetimi, Goodbudget B1/B2 ve B06 sınırı,
Hesap Defterim B1/B2 negatif taraması, KolayBi tekrar formu ve nakit akış raporu
kullanıldı. Kaynak formlar: E0010, E0005, E0014, E0006, E0007, E0008.
Görsellerin önceki inceleme kayıtları kullanıldı; yeniden görsel veya canlı test
yapılmadı. Rapor hedefi **Belge 2 §2** (planlama, tekrar, taksit), **Belge 1 §3–4**
(bekleyen liste, durum etiketleri, kurulum/onay formları). Bütçe/tahmin alanları
raporlama temasını da besler; karar adayları yalnız Belge 3 içindir.

### Tekrarlayan işlem: kurulum ile gerçekleşme ayrı okunur

| Uygulama | Kurulum ve gözlenen sonuç | Dayanak | Sınır |
|---|---|---|---|
| Money Manager | Aylık form rozeti; 10 Eylül koşumunda geçmiş 10 Ağustos ve o günkü 10 Eylül örnekleri gerçek işlem. 10 Ekim ayrı Tekrarlama önizlemesinde, ay toplamları sıfır | `17-tekrarlayan-aylik-form.png`, `18-tekrarlayan-agustos-liste.png`, `19-tekrarlayan-eylul-otomatik.png`, `20-tekrarlayan-ekim-onizleme.png` | Otomatik üretim bu koşumda gözlendi; bütün sıklıklara genellenmez. Kurulum onayı ve Tekrar/Taksit açılır menüsü karesiz |
| Bluecoins | Tekrar sıklığı, bitiş seçenekleri ve otomatik giriş kutusu. Otomatik kapalı koşumda geçmiş örnek “31 gün gecikmeli”, aynı günkü “Bugün süresi doluyor”. Kaydet → Bugün / 10 Ağustos; geçmiş tarih kolu sonunda Ağustos gideri 600 arttı | `23-b1-planli-islem-aylik-sheet.png`, `24-b1-yenilenen-islem-banner.png`, `25-b1-hatirlatici-gecikmeli-bugun.png`, `27-b1-islem-olarak-kaydet-bugun-mu.png`, `28-b1-onay-sonrasi-rapor-gider-3650.png` | Otomatik açık ve Bugün kolları denenmedi; kurulum sonrası/onay öncesi ayrı rapor karesi yok. Kapalı kutu panelde, geçmiş tarih sonraki formda görülür |
| Wallet | Planned payments → Recurrent payment; planlı ödemede geçmiş gün kısıtı. Confirm → Payment summary; ilk 600 onayından sonra Ana Hesap 20.800 → 20.200. Ardından otomatik/onaylı sorusu, Yes ön seçili; sıradaki örnek bekleyen | `14-planned-gecmis-tarih-kapali.png`, `16-b1-plan-detay-due-today.png`, `17-b1-confirm-payment-summary.png`, `18-b1-otomatik-mi-onayli-mi-secimi.png`, `19-b1-onay-sonrasi-paid-today-siradaki-pending.png`, `20-b1-sonrasi-ana-hesap-20200.png` | No seçimi koşum notu; sonraki ayar diyaloğunda No seçili (`45-planned-otomatik-onayli-toggle-her-zaman.png`). Otomatik üretim, erken onay ve değiştirilmiş tutarla onay denenmedi |
| Goodbudget | Schedule this…; sıklık listesi, tarih önizlemesi ve e-posta hatırlatma seçeneği. Senaryo aylık hedeflense de **Every 2 Weeks** seçilmiş; ilk 10 Ağustos aboneliği gerçek listede ve aramada | `19-recurring-schedule-turkish-dates.png`, `26-schedule-frequency-listesi.png`, `20-search-results.png` | Sonraki üretim zamanı/onayı ve 600 farkının nedeni bilinmiyor (B06); e-posta gönderimi kanıtlanmadı |
| Hesap Defterim | İncelenen form, menüler, çekmece ve ayarlarda finansal plan/tekrar yok; B1 kurulamadı | `36-drawer-menu-ust.png`, `42-drawer-diger-uygulamalar-veresiye-gelirgider.png`, `15-ayarlar.png`, `43-ayarlar-alt-bolum-donem-baslangici.png`; E0007 B1/B2 | Not Defteri ayrı görev/not yüzeyidir; finansal plan değildir |
| KolayBi | Gider, alış, satış ve maaş için Tekrarlı yüzeyler; mevcut gider/maaştan dönüştürme. Maaş formunda periyot ve tekrar sayısı zorunlu; başlangıç ve Belirsiz/Belirli ödeme tarihi | Resmî kaynak: `d12-destek-gider-detay-islemler.png`, `d17-destek-calisan-maasi-detay.png`, `d18-destek-tekrarli-maas-formu.png` | Sayı zorunluluğu görülen maaş formuna aittir; diğer formlara genellenmez. Kayıt üretimi, onay ve ödeme etkisi canlı doğrulanmadı |

Paraşüt, Logo İşbaşı ve QuickBooks için bu temada karşılaştırılabilir kurulum–
gerçekleşme zinciri bulunmuyor; ürünlerinde özellik yokluğu anlamına gelmez.

### Taksit: zamana bölme, ilk kayıt ve kalanlar

| Uygulama | Gözlenen | Dayanak | Sınır |
|---|---|---|---|
| Money Manager | 6.000 / 6 ay; başlıkta (1/6), Ağustos'ta 1.000 gider. Kartta Bu Ay 1.000 + Gelecek Ay 1.000; dört sonraki taksit bu toplamda yok. Eylül defterinde (2/6) 1.000 | `21-taksit-6ay-form.png`, `22-taksit-1-6-agustos.png`, `23-taksit-kart-borcu-bu-gelecek-ay.png`, `32-kart-defteri-eylul-taksit-2-6.png` | Kalan bütün vadelerin gelecekteki üretimi izlenmedi; görünen borç tüm taahhüdün toplamı değildir |
| Bluecoins | 6 ay, oran 0,00, geçmiş ilk ödeme; kaydetmeden önce altı 1.000 özeti. İlk 1/6 gerçek listede ve Ağustos giderinde; 2/6–6/6 beş bekleyen hatırlatıcı | `18-b2-taksit-6ay-15agu.png`, `19-b2-6ay-hatirlatici-metni.png`, `20-b2-1-6-taksit-1000-kayit.png`, `21-b2-kalan-5-taksit-hatirlatici.png`, `22-b2-sonrasi-rapor-gider-3050.png` | İlk kaydın oluşması geçmiş tarihten mi taksit kuralından mı ayrıştırılmadı; sonraki gerçekleşme ve sıfır dışı oran etkisi bilinmiyor |
| Wallet | İncelenen yollarda taksit alanı bulunmadı; 6.000 tek kart harcaması, kart −6.000 ve Eylül giderine tam tutar | `21-b2-islem-detay-taksit-alani-yok.png`, `23-b2-6000-tek-kart-borcu.png`, `37-records-listesi-b1-b2.png` | Normal tek harcamanın sonucu; bütün paketlere yokluk genellenmez |
| Goodbudget | Split into multiple Envelopes tutarı zarflara dağıtır; zamana yayılan taksit kurulmadı | E0006 B2; `09-credit-type-selected.png` kısmi form | Kart hesabı paywall nedeniyle açılamadı; ürün geneline taksit yokluğu çıkarılmaz |
| Hesap Defterim | Taksit planı kurulamadı; elle defter kaydı yaklaşımı | E0007 B1/B2 negatif taraması | Altı elle kayıt alternatifi kullanım önerisidir, koşulmuş taksit deneyi değildir |

### Bekleyen görünüm, bütçe ve hedef

| Yüzey | Gözlenen yaklaşım | Dayanak / sınır |
|---|---|---|
| Bluecoins ortak bekleyen liste | Tek seferlik kira, abonelik ve taksitler tarih sıralı; gecikme/bugün/yarın etiketleri | `31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png`; tek liste ortak domain veya fiziksel şema kanıtı değildir |
| Wallet örnek yönetimi | Confirm; menüde Postpone / Dismiss; Records menüsünde Show planned payments | `50-u02-plan-menu-postpone-dismiss.png`, `49-u03-records-menu-bakiye-planli-secenekleri.png`; erteleme/atlama ve görünüm anahtarının sonuçları denenmedi |
| Wallet bütçe | Aylık 5.000 bütçe; kategori, hesap, etiket alanları. 6.600 harcama, −1.600 kalan ve aşım geri bildirimi; Spent/Remains, günlük ortalama, Forecasted Spend | `27-butce-formu-dolu.png`, `28-butce-olusturuldu-over-budget.png`, `29-butce-detay-6600-harcama.png`; tahmin yöntemi bilinmiyor, doğrusal ay sonu hesabı denemez |
| Wallet hedef | Ad, Target amount, Saved already, Desired date; özet ilerlemesi | `30-goal-olusturma.png`, `31-goal-detay-formu.png`, `32-butce-ve-goal-birlikte.png`; 20.000 hedef tutarı koşum notu, hesap bağlantısı denenmedi |
| Goodbudget bütçe | Zarf satırında kalan/bütçelenen ve ilerleme; hesap katmanı opsiyonel | `08-envelopes-filled-home.png`, `04-accounts-off-by-default.png`; bütçe kalanı ile hesap bakiyesi ayrı göstergeler |
| KolayBi vade ve tahmin | Günü Gelen İşlemler; Nakit Akış Raporu'nda güncel bakiye, tahsilatlar, ödemeler ve tahmini dönem sonu; vade matrisi | Resmî kaynak: `d34-video-gunu-gelen-islemler.png`, `d29-destek-nakit-akis-raporu.png`. Demo tutarı tümüyle Belirsiz kovasında; on iki ay kovası boş. Bu kare aylara dağılmış tahmini kanıtlamaz |

Money Manager bütçe kurulumu önceki kullanıcı kararıyla kapsam dışı;
Bluecoins'te bütçe özeti görülmesi kurulum/hesaplama doğrulaması değildir.
Hesap Defterim'de finansal bütçe/plan yüzeyi bulunmadı. Not hatırlatıcıları
finansal vadeyle birleştirilmez; KolayBi Notlar bildirim davranışı da ölçülmedi.

### BusinessFinance kıyasının dayanağı ve Belge 3 adayları

15 Eylül salt okunur kod kontrolü: [Application sözleşmesi](../../src/BusinessFinance.Application/FinancialActivities/PlannedActivityContracts.cs)
`PlannedActivityListResult`, tarih/ufuk/kapsam yanında yalnız `TotalCount`,
`NearestDueDate`, `Items` taşır. [API sözleşmesi](../../src/BusinessFinance.Api/Features/FinancialActivities/PlannedActivityContracts.cs)
aynı liste yapısını sunar; toplam tutar veya tahmini bakiye yoktur. API yorumunda
bu tercih gerekçelidir: planlanan gelir, gider, ekstre ve nötr yükümlülükleri
tek sayıda toplamak yanıltabilir. Satırdaki `IsProjected` bir bakiye tahmini değildir.
KolayBi kıyası bu liste sözleşmesine aittir; yeni bir toplam önerisi bütün satırları
toplayarak uygulanabilecek hazır bir arayüz işi sayılmaz.

| Aday | Kaynak karar etiketi | Rapora taşıma sınırı |
|---|---|---|
| Money Manager gelecek önizlemesi ve taksit sıra etiketi | Önizleme `uyarlayarak al`; taksit `doğrudan al (aynı fikir)` | Otomatik gerçekleşme ile BF açık onayı ayrılır; “aynı model / tek fark otomasyon” kanıtlanmış sonuç değildir |
| Bluecoins ortak liste, gecikme etiketi, gerçekleşme tarihi seçimi | `uyarlayarak al` | Bugün kolu ve otomatik açık yol için sonuç uydurulmaz |
| Wallet onay özeti, otomatik/onaylı tercih ve sonraki ayar diyaloğu | `uyarlayarak al`; ayarı değiştirme satırı `doğrudan al` | Kaynak etiketleri, kabul edilmiş karar değil. BF'de otomatik mod, atlama ve erteleme olmadığı P3-T'de kaydedildi; yeni davranış kararı gerekir |
| KolayBi mevcut kayıttan tekrara dönüştürme | `uyarlayarak al` | Menü/form kanıtı; otomatik üretim iddiası çıkarım olarak kalır |
| KolayBi zorunlu tekrar sayısı | `henüz karar verme` | Görülen maaş formuyla sınırlı. BF isteğe bağlı bitiş tarihi ve toplam tekrar sınırını zaten taşır (P1-B02-B04); eksik bitiş özelliği diye sunulmaz |
| KolayBi tahmini dönem sonu bakiyesi | `uyarlayarak al` | Vadesiz kalemlerin anlamı ve çifte sayım sınırı çözülmeden öneri kesinleşmez; demo eşitliği genel algoritmayı kanıtlamaz |
| Wallet Forecasted Spend; Goodbudget sonraki tekrar mekanizması | `henüz karar verme` | Tahmin formülü ve açıklanamayan 600 farkı öneri gerekçesi yapılamaz |

### Taşıma nitelemeleri

- Wallet “her örnek bekler / hiç otomatik yapmaz” ifadeleri **No seçilen koşumla**
  sınırlandırılır; ürün otomatik seçeneği de sunar. Silme diyaloğunda geçmiş uyarısı
  görülmemesi, silmenin gerçekleşmiş kaydı koruduğu veya sildiği anlamına gelmez.
- Bluecoins ilk taksit sonucu genel otomatik kayıt kuralı değildir; oran alanına
  yüzde/faiz anlamı ve vergi kıyası eklenmez. Goodbudget koşumu aylık diye anlatılmaz.
- KolayBi tekrar sayısı zorunluluğu yalnız maaş formunda görüldü; onay alanının
  görünmemesinden arka planda otomatik üretim kesinliği çıkmaz.
- Money Manager seri silme, Wallet geçmiş aya kaydırma ve hedef tutarı gibi
  karesiz ayrıntılar koşum notu olarak kalır. Gelecek vadeleri bekleyerek veya
  kapanmış incelemeleri yeniden açarak yeni test turu yapılmadı.

**Paket kontrolleri:** `git diff --check` geçti; Tema 06 başlığı tekil,
52 farklı görsel adı mevcut dosyalarla eşleşiyor, iki kod bağlantısı geçerli,
yeni bölümün tablo sütunları ve satır sonu boşlukları temiz; beş takip başlığı
Tema 07'yi gösteriyor. Bu atıf kontrolü yeniden görsel içerik denetimi değildir.
Kaynak formlar, görseller ve uygulama kodu değişmedi; build/test/analyze ve
form denetim kapısı tekrar çalıştırılmadı. Commit yapılmadı.

**Sıradaki tek paket: P4-tema-07 (borç, fatura ve tahsilat).**

## Temalar 01–06 — Hızlı tutarlılık kontrolü — 15 Eylül 2026

Kullanıcı isteğiyle tema sırası, kapanışlar, rapor hedefleri, görsel adları ve
önemli taşıma sınırları kontrol edildi. Altı tema tekil ve kapalı; her birinde
rapor hedefi var, anılan PNG adları mevcut dosyalarla eşleşiyor. Görseller yeniden
incelenmedi; bu kontrol önceki içerik denetimlerinin yerine geçmez.

- Tema 04: Money Manager açılışının ana akış dışında kalmasına **HAYIR seçimi**
  koşulu eklendi; dayanak E0010 Açılış bakiyesi bölümü.
- Tema 05: yokluk ifadeleri incelenen sürüm/yüzey/kaynakla sınırlandırıldı.
- Tema 06: planın gerçekleşmesi, otomatik mod ve taksit belirsizlikleri ayrılmış;
  BF tahmini bakiye kıyasının iki kod sözleşmesi mevcut. Yeni test ihtiyacı çıkmadı.
- Plan Bölüm 6 **10 tema** sayar: 01 ürün kimliği, 02 gezinme/özet, 03 işlem girişi,
  04 hesap/kart/transfer, 05 kapsam, 06 planlama/tekrar/taksit, 07 borç/fatura/tahsilat,
  08 raporlama, 09 veri aktarımı/entegrasyon, 10 diğer ürün modülleri. Ardından
  ayrı **P4-K geçiş kontrolü** vardır; on birinci tema değildir.

## P4-tema-07 — Borç, fatura ve tahsilat — 15 Eylül 2026

**Durum: kapandı.** Kaynaklar: Bluecoins E0005 P2-G03 D2/D3; Wallet E0014
Faz 7 D2/D3 ve WL-U01; KolayBi E0008 Genel Gider/Cari ve KG02 sınırları;
Paraşüt E0011 sistem akışı ve PG01; Logo E0009 sistem akışı; Hesap Defterim
E0007 sistem akışı; Goodbudget E0006 B07/Debt sınırı; Money Manager E0010
akış özeti; QuickBooks E0012 fatura satırı. P1-B01 ve P1-B02-B04 kod kıyasları
korundu. Yeni görsel/canlı koşum yapılmadı. Rapor hedefi **Belge 2 §2–3**
(borç/fatura/tahsilat ve olayların bağlanışı), **Belge 1 §3–4** (cari liste,
kalan tutar, form ve işlem etkisinin sunumu). E-belge entegrasyonunun ayrıntısı
Tema 09'a, raporların ayrıntısı Tema 08'e taşınır.

### İki canlı zincir: aynı 12.000 ve 5.000, farklı anlam

| Konu | Bluecoins | Wallet | Kanıt ve sınır |
|---|---|---|---|
| 12.000 hizmet kaydı | GELİR türünde Ada Reklam cari hesabına yazıldı; cari 0 → 12.000, banka 29.700 kaldı | I Lent türünde, Record oluşturmadan ikinci ayrı borç kartı; OWES ME 12.000. Bakiye, Records ve aramaya etkisi yok | Bluecoins `f7-23-hesap-dogru-secildi.png`, `f7-24-d2-kaydedildi.png`; Wallet `f7-43-amount-12000.png`, `f7-44-d2-saved.png`, `f7-45-d2-final.png`. İkisinde de bu koşum bir e-fatura üretimi değildir |
| 5.000 kısmi tahsilat | Cari → Ana Hesap TRANSFER; cari 12.000 → 7.000, banka 29.700 → 34.700. Banka+cari 41.700 ve gün neti 2.000 korunur | Aynı Debt kartında Add Record → Create new Record → Repay debt; borç 12.000 → 7.000, banka 4.800 → 9.800 | Bluecoins `f7-27-transfer-hazir.png`, `f7-28-d3-kaydedildi.png`; Wallet `f7-47-new-record-form.png`, `f7-48-amount-5000.png`, `f7-49-d3-saved.png`, `f7-50-balance-check.png` |
| 7.000 neyin kalanı? | **Cari hesap bakiyesi**; belirli faturaya tahsis gösterilmedi | **Seçilen borç kartının kalanı**; aynı adlı iki borç ayrı kart | Bluecoins'te fatura seçim alanı görünmemesi ürün genelinde bu bağın yokluğunu kanıtlamaz; Wallet borç kartı ticari fatura nesnesi diye adlandırılmaz |
| Gelir/gider etkisi | 12.000 gelir listede; 5.000 transfer iki nötr bacak. D3 sonrası bütün hesaplar net raporu yok | WL-U01'de önceki 5.000 borç verme gider, D3 5.000 tahsilat gelir; Record'suz 12.000 bu incelenen dönemde gelirde yok | Wallet `48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`: son 30 gün gelir 5.000, gider 21.600, net −16.600. **ADR 0014'e eşdeğerlik iddiası reddedildi**; bütün borç türleri için genelleme yapılmaz |

Wallet'taki “Record oluşturursan bakiyen değişir” sorusu **bakiye hareketini**
sorar. Ekonomik olayın hangi tarihte gelir/gider olacağını doğru modellediğinin
kanıtı değildir. Borca bağlı kısmi tahsilatın varlığı ile raporun tanıma kuralı
iki ayrı bulgudur. Bluecoins'in banka+cari toplamı da kasadaki para diye sunulmaz.

### Kaynakla incelenen ön muhasebe akışları

| Uygulama | Görülen / kaynakta anlatılan | Dayanak | Taşıma sınırı |
|---|---|---|---|
| KolayBi gider | Cari Takibi Yok/Var, Ödeme Durumu Ödenmedi/Ödendi ve boş bırakılabilen vade; ödeme ile cari tercihleri ayrı alan | `d05-destek-yeni-gider-formu.png` | Resmî destek karesi; Ödendi seçiminin kasa/cari/rapor etkisi ve çifte sayım canlı doğrulanmadı |
| KolayBi cari | Müşteri/Tedarikçi birleşik tipi; Borç/Alacak Ekle, Fatura Ekle, Ödeme/Tahsilat Ekle ayrı eylemler. Hareketlerde borç/alacak, yürüyen bakiye, vade, banka/kasa ve proje | `d07-destek-cari-listesi.png`, `d09-destek-cari-detay-ekstre-dialog.png` | Form ve kolon ayrımı kanıtlı; işlem modeli eşdeğerliği, belirli faturaya tahsis ve kısmi kapanış doğrulanmadı |
| KolayBi açılış / mahsup / ekstre | Açılış bakiyesi bloğu ve vade alanları; Mahsuplaştır menüsü; tarih/para birimi/kolon seçimiyle ekstre ve PDF önizleme | `d08-destek-cari-olusturma-formu.png`, `d09-destek-cari-detay-ekstre-dialog.png`, `d10-destek-cari-ekstre-onizleme.png` | Mahsup algoritması denenmedi. E-Posta ile Gönder düğmesi gönderim kanıtı değildir; çıktı teması 09 |
| Paraşüt | Formun kılavuz özetine göre gider/fatura önce, ödeme sonra; kısmi ödeme ve avans; en gecikmiş açık faturadan otomatik eşleme anlatılıyor | E0011 Sistem işleyişi / pipeline; P1-parasut-G01 ve PS-Q04 | **Yalnız resmî kaynak aktarımı.** Mahsup/kısmi ödeme görsel veya canlı sonuçla sınanmadı. Video 02/03 aynı faturanın önce/sonrası değildir, iki ayrı örnektir |
| Logo İşbaşı | Ürün anlatımında müşteri seçerken bakiye, ürün/kalemle fatura, cari içinde nakit/banka tahsilatı ve ödeme | E0009 Sistem işleyişi / pipeline | İç akış çalışırken görülmedi. Kısmi tahsis veya bir faturayla üç bakiyenin koşulsuz değişmesi sonucuna varılmaz; stok/kasa etkisi kayıt türü ve ödeme durumu görülmeden kesinleştirilmez |
| QuickBooks Solopreneur | Formun ürün sayfası özetinde müşteri → kalem → ödeme yöntemi → fatura gönderimi ve hatırlatma | E0012 Fatura kesme satırı | Solopreneur iç yüzeyi görülmedi; QBO Simple Start kareleri bunun davranış kanıtı değildir. Fatura gönderebilmek kapsamlı A/P–A/R modülüyle aynı şey değildir |

### Diğer ürünlerin kapsama katkısı

- **Hesap Defterim:** İncelenen üründe ayrı cari/müşteri/fatura kapanışı bulunmadı;
  kullanıcı defterinin bakiyesini tutar. Veresiye Defteri ayrı uygulama bağlantısı
  (`42-drawer-diger-uygulamalar-veresiye-gelirgider.png`); o ürün test edilmedi.
  Müşteri başına defter önerisi denenmiş borç tahsis zinciri sayılmaz.
- **Goodbudget:** Fatura→tahsilat zinciri görülmedi; Debt hesap grubu ve kaynakta
  bütçe dışı Debt desteği var, içeriği denenmedi (B07). “Borç özelliği yok” denmez.
- **Money Manager:** Bu tema için cari/fatura→kısmi tahsilat zinciri kurulmadı;
  kart borcu/ödeme bulguları Tema 04'tedir. Kart borcu işletme carisinin yerine geçmez.

### BusinessFinance kıyası ve Belge 3'e devredilen sorular

15 Eylül salt okunur kontrol: [CounterpartyPayment](../../src/BusinessFinance.Domain/CounterpartyPayment.cs)
`CounterpartyId` ve `AccountId` taşır, belirli `CounterpartyChargeId` taşımaz;
ödeme kategori/kapsam taşımadan para hareketi üretir. P1-B01/B04'te doğrulanan
ayrım korunur: ekonomik olay borçlandırmada tanınır, tahsilat tekrar gelir değildir.
[ObligationSettlement](../../src/BusinessFinance.Domain/ObligationSettlement.cs)
`ObligationId` taşır; [Obligation.Settle](../../src/BusinessFinance.Domain/Obligation.cs)
yükümlülüğün kendi tam `Amount` değerini kapanışa verir. Cari toplamı, tek borç
kalanı ve tam kapanış birbirinin yerine yazılmaz. Vadesi geçmiş cari tutarı için
ödemeleri sırayla düşen okuma kuralı da kalıcı fatura tahsisi değildir (P1-B01).

| Belge 3 adayı | Kaynak / mevcut etiket | Açık soru ve sınır |
|---|---|---|
| Bakiye etkisini kayıt anında açıkça söylemek | Wallet `uyarlayarak al` | Kullanıcıya hangi para hareketi sorulur? Yalnız açıklama deseni; Wallet gelir/gider kuralı kopyalanmaz |
| Borç başına kalan ve bağlı kısmi tahsilat | Wallet `doğrudan al` | Cari toplamının yanında belirli borca tahsis gerekli mi? Kaynak etiketi kabul kararı değildir; BF'de yeni finansal davranış gerektirir |
| Ayrı borçlandırma ve ödeme eylemleri | KolayBi destek ekranı | Aynı cari detayda niyet ayrımı nasıl sunulur? Butonlar model eşdeğerliği kanıtı değildir |
| Mahsup ve fatura sıralaması | Paraşüt kılavuz özeti; KolayBi menüsü | Otomatik fatura eşleştirme ile alacak/borç netleştirme aynı işlem sayılmaz; görülen menü algoritmayı açıklamaz |
| Cari açılış devri ve karşı tarafa ekstre | KolayBi destek ekranları | Yeni kayıt/çıktı ihtiyacı kullanıcı kararıdır; kaynakta bulunması ihtiyaç ölçümü değildir. Rapor/aktarım ayrıntıları Tema 08/09'a gider |

### Forma yayılan düzeltme ve korunacak sınırlar

Wallet E0014 D3 ve arama paragraflarında kalan “rapor etkisi ölçülmedi / B09 açık”
ifadeleri WL-U01 kapanışına bağlandı. Yeni finansal sonuç üretilmedi; 15 Eylül
kullanıcı kanıtının önceki satırlara yayılması tamamlandı. Borcu artırma, mevcut
Record bağlama, silme sonrası etki ve diğer borç türleri hâlâ denenmiş sayılmaz.
Kapanmış B01/B09 için yeni emülatör turu açılmadı; ürün kodu değişmedi.

**Paket kontrolleri:** Wallet form denetimi 109 karede 0 hata/0 uyarı; `git diff --check` temiz. Tema 07'de 18 farklı görsel adı ve 3 kod bağlantısı geçerli; yedi temanın tablo/boşluk kontrolü ve beş devir başlığı geçti. Görsel içerikleri yeniden incelenmedi. Uygulama kodu değişmedi; build/test/analyze çalıştırılmadı, commit yapılmadı.

**Sıradaki tek paket: P4-tema-08 (raporlama).**

## P4-tema-08 — Raporlama: toplamın anlamı, dönem ve kırılım — 15 Eylül 2026

**20 Eylül eki:** MM K1 (E0403–E0405) Eylül giderini 1.600 olarak doğruladı;
400 kart ödemesi gider toplamına eklenmiyor, kart satırında ödeme parantezinde gösteriliyor.
GB K3 (E0423) Eylül Income 3.284 / Spending 0; aynı koşumda önce raporu yok, 2.050 eski
koşumdan. Liste, hesap ve zarf etkileriyle birlikte dosya sonundaki kapanışta anlatıldı.

**Durum: kapandı.** Dokuz formun rapor yüzeyi kullanıldı: E0010 Toplam/filtre/
Toplama Dahil Et, E0005 kontrol değerleri ve P2-G04, E0014 Cash-flow/Spending,
WL-U01 ve bütçe, E0006 rapor davranışı, E0007 birleşik defter/özet/arama,
E0008 proje ve rapor tabloları, E0011 rapor/kılavuz, E0009 raporlama beyanı,
E0012 Type ve vergi özeti. Önceki görsel incelemeleri korundu; yeni görsel,
emülatör veya dış kaynak koşumu yapılmadı. Rapor hedefi **Belge 1 §3–4**
(grafik, tablo, filtre, boş durum) ve **Belge 2 §2–3** (rapora giren olaylar,
dönem, raporlar arası bağ). Dışa aktarımın dosya/teslim davranışı Tema 09'a aittir.

### Canlı incelenen ürünlerde rapor yüzeyi

| Ürün | Gösterilen ölçü ve sunum | Dayanak | Sınır |
|---|---|---|---|
| Money Manager | Gelir/Gider/Toplam; Toplam sekmesinde nakit-banka gideri ve kart harcaması ayrı. Ağustos 3.050 gider = 850 nakit/banka + 2.200 kart; kart satırındaki (1.200) ödeme, dönem sonu borcu değil | `26-toplam-sekmesi-agustos.png`, `31-kart-defteri-agustos-hareketler.png` | Kart dönem sonu borcu 1.000; 2.200 tutarı borç diye etiketlenmez. Pasta → kategori ayrıntısı koşum notu |
| Bluecoins | Net Kazançlar Ağustos: gelir 25.000, gider −2.050, dönem neti 22.950; varlık görünümü ayrı. Takvimde 11 Eylül −10.000/+12.000, net 2.000 | `16-kontrol-degerleri-net-kazanc-44950.png`, `f7-47-takvim.png` | 44.950 varlık sonucu başka ölçüdür; karede ilgili net satırının sonu düğmeyle örtülü. Açılış tarihi geçmiş dönem bakiyelerini etkiler |
| Wallet | Cash-flow çekirdek Ağustos: 25.000 gelir, −2.050 gider, 22.950 net; Spending kategori dağılımı ve büyük giderler. Bütçede harcanan/kalan, günlük ortalama, tahmin | `11-kontrol-cash-flow-agustos.png`, `05-siniflandirma.png`, `29-butce-detay-6600-harcama.png` | Çekirdek veri ile son 30 gün borç koşumu farklıdır. Forecasted Spend yöntemi bilinmiyor; 30 güne doğrusal yansıtma denmez |
| Goodbudget | Spending by Envelope ve Income vs Spending; aylık karşılaştırma. Ağustos Credit koşumunda Spending −22.950, gelir 0; Eylül Income 2.050 | `16-spending-by-envelope-negative-bug.png`, `17-income-vs-spending-bug.png`, `14-reports-default-current-month.png` | 25.000 Credit ile girildi; normal gelir yolu aynı sonucu üretir denmez. Initial Fill eşleşmesi bütün zarf dağıtımlarının gelir sayıldığı anlamına gelmez |
| Hesap Defterim | Birleşik defterde Alındı 51.200, Ödendi 6.250, Denge 44.950. Özet gün gün Alındı/Ödendi/Tasarruf; grafik yerine tablo ve takvim | `06b-islemler-butun-hesaplar.png`, `07-rapor-aylik-butun-hesaplar.png`, `13-ozet-tasarruf-agustos.png` | Alındı/Ödendi açılış ve transferi içerir; saf gelir/gider değildir. 44.950 çekirdek koşumuna ait, sonraki 47.300 ile aynı an değildir |

### Dönem ve filtre: hangi kayıtlar dahil?

| Ürün | Gözlenen | Sınır |
|---|---|---|
| Money Manager | Ay seçimi; GELİR/GİDER/HESAP filtre sekmeleri; hesap başına gelen/giden sütunları (`27-filtre-paneli-agustos-hesap.png`) | Türkçe havale başlıkları veri yönüyle ters. Hatanın yalnız Türkçeye özgü olduğu çıkarım; aynı sürümün İngilizce karşılaştırması yok |
| Bluecoins | Tutar aralığı, tarih, tür, kategori, hesap, etiket, durum ve metin alanları (`f7-31-filtre.png`); Ada araması iki transfer bacağı + gelir, net 12.000 (`f7-30-arama2.png`) | Kaydet/yükle ikonları kayıtlı filtre davranışının denenmesi değildir; canlı arama ve çoklu seçim tek kareden çıkarılmaz |
| Wallet | Records hafta grupları; Settings → Filters giriş açıklaması Statistics/Records için özel filtrelerden söz eder (`f7-56-settings.png`); dönem başlangıç günü ayarı (`f7-58-advanced.png`) | Filtre kurma ve dönem ayarının rapora etkisi denenmedi; Records menüsünde görülen iki anahtar gelişmiş filtre değildir |
| Goodbudget | Rapor açılışında güncel ay; başka ayın kayıtları için dönem değiştirme; boş raporda No transactions found (`15-report-empty-state.png`) | Tek boş ekran veri kaybı veya rapor kusuru kanıtı değildir |
| Hesap Defterim | Herşey/Günlük/Haftalık/Aylık/Yıllık; ay/hafta/yıl başlangıcı ayarları (`43-ayarlar-alt-bolum-donem-baslangici.png`); Ada aramasında özet 25.000/0/25.000 (`29-arama-canli-filtre.png`) | Arama özeti filtrelenen kayıtları anlatır, tüm defter bakiyesini değil. Ayarların her rapora yayılımı ayrı ölçülmedi |

**Hesabı toplamdan çıkarma iki farklı ölçüdür:** Money Manager kapalı anahtarda
Ortak Cuzdan 4.150 satırı gri kalırken net varlık 38.200 ve nakit grubu 0 görünür
(`29-toplama-dahil-kapali-net-varlik.png`). Açık hâlin 42.350 toplamı aynı anda
çekilmiş ayrı kare değil, kapalı kare + cüzdan tutarından yapılan aritmetiktir.
Bluecoins `f7-35-nakit-akim-ayari.png` ise nakit akışı raporuna katılımı gösterir;
kullanılan hesapların kapalı olması gözlemdir, fabrika varsayılanı ve sıfır raporun
kesin nedeni değildir. Net varlık anahtarıyla eşdeğer sayılmaz.

### Masa başı rapor yüzeyleri

| Ürün / yüzey | Kaynağın gösterdiği veya anlattığı | Sınır |
|---|---|---|
| KolayBi proje özeti | Gelir/Gider/Net listesi; detayda tahsil edilen/bekleyen, ödenen/bekleyen ve ayrı nakit görünümü (`d01-destek-proje-listesi.png`, `d03-destek-proje-detay-ozet.png`, `d04-destek-proje-belge-kirilimi.png`) | Liste ve detay aynı proje değil; farklı ekran etiketlerinin aynı formüle ait olduğu kanıtlanmadı. Ekran ayrımı model eşdeğerliği değildir |
| KolayBi alış/satış | Cari/proje/ürün/etiket kırılımları; ödeme durumu, para birimi, şube, belge tarihi ve vade aralığı; KDV Dahil anahtarı (`d26-destek-alis-satis-raporu.png`) | Filtre ve anahtar varlığı; açıp kapama sonucu denenmedi. Para birimi kolonları canlı kur dönüşümü kanıtı değil |
| KolayBi gelir/gider | Para birimi ve yerel toplam matrisi; işlem türü, cari, belge, tarih, ödeme yöntemi/banka sütunları (`d28-destek-gelir-gider-raporu.png`) | Kare boş; aynı tarihli KDV karesinin dolu olması fatura dışlama kuralı veya kusur kanıtlamaz; veri anları bilinmiyor |
| KolayBi rapor ailesi | Üst barda on rapor: alış/satış, cari bakiye, banka/kasa, KDV, stok, ödeme/tahsilat, çek, senet, gelir/gider, nakit akış (`d27-destek-kdv-raporu.png`) | Menünün görülmesi on raporun hesaplamasının denetlendiği anlamına gelmez |
| Paraşüt | Videoda Gelir/Gider ve Kasa/Banka ayrı; ikincisinde tahsilat/ödeme listesi ve zaman ölçekli bar grafik (`07-video-gelir-gider-raporu.png`, `08-video-nakit-akisi-raporu.png`) | Tanıtım kurgusu; sayılar gerçek koşum sonucu değildir. Gelir/Gider raporunun tanıma zamanı karede yazmaz; vergi hariç açılır seçeneği görülmedi |
| Logo İşbaşı | Ürün anlatımında tarih aralığı, kategorize raporlar ve grafikler | E0009 kaynak beyanı; iç rapor ekranı/hesaplama davranışı görülmedi |
| QuickBooks Solopreneur | Kaynakta Business/Personal Type ayrımı ve işletme/vergi raporu ilişkisi; üç aylık vergi tahmini | E0012 kaynak beyanı; Solopreneur rapor ekranı yok. ABD ürün bağlamı ve QBO Simple Start ayrımı korunur; güncel vergi kuralı diye aktarılmaz |

### KDV ve tahmin: görünen sayı, türetme kuralı değildir

- KolayBi matrah/tutar matrisi ve Hesaplanan KDV Toplam satırı gösterir
  (`d27-destek-kdv-raporu.png`). Alt toplam ve İndirilecek etiketi karede görünmez.
  12.000 matrah ile 2.400 tutarın aritmetik uyumu, uygulamanın oran üzerinden
  hesap yaptığını kanıtlamaz. Görülen tarihsel oran listesi güncel mevzuat değildir.
- Paraşüt formunun kılavuz aktarımı aylık Hesaplanan/İndirilecek/Net KDV,
  Tümü/Satışlar/Giderler ve ayın belge dökümünü anlatır; canlı test değildir.
  Bu özet bir beyannamenin doğruluğunun veya üretildiğinin kanıtı sayılmaz.
- KolayBi tahmini dönem sonu, demo karesinde 19.543,53 + 143.252,50 = 162.796,03;
  tahsilatların tümü Belirsiz kovasındadır (`d29-destek-nakit-akis-raporu.png`).
  Eşitlik genel algoritma ve ay ay tahmin doğruluğu kanıtı değildir (Tema 06).
- Wallet WL-U01 son 30 gün gelir 5.000, gider 21.600 sonucu borç verme ve
  tahsilat kayıtlarını içerir (`48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png`).
  Çekirdek Ağustos 22.950 netiyle karşılaştırıp rapor çelişkisi üretilmez (Tema 07).

### BusinessFinance dayanağı ve Belge 3 adayları

[ADR 0013](../../documentation/adr/0013-business-and-personal-are-one-pool.md)
kapsamı rapor boyutu olarak tutar; bakiye/kart borcu/net varlık havuzun toplamıdır.
P1-B04 ve Tema 07'deki tanıma/taşıma ayrımı korunur: işletme neti kasa değişimi
değildir. [ADR 0016](../../documentation/adr/0016-tax-fields-carry-they-do-not-calculate.md)
KDV oranı ve tutarını ayrı taşır; kayıt tutarı brüt kalır, vergi türetilmez.

**15 Eylül kod kontrolü:** [AccountantPackageUseCases](../../src/BusinessFinance.Application/Taxes/AccountantPackageUseCases.cs)
işletme kapsamlı aylık raporun gelir/gider/netini kullanır; satırların girilmiş
`VatAmount` alanlarını gelir ve gider için ayrı toplar ve boş olanları sayar.
[Sözleşme](../../src/BusinessFinance.Application/Taxes/AccountantPackageContracts.cs)
`VatOnIncome`, `VatOnExpense`, `LinesWithoutVat` taşır. Dolayısıyla “KDV'yi yalnız
toplama orta yolu hiç ele alınmadı” kıyası yanlıştır; bu yetenek zaten vardır.

| Aday | Kaynak etiketi / bağ | Açık soru ve taşıma sınırı |
|---|---|---|
| Kart harcaması ile ödeneni birlikte sunma | Money Manager `uyarlayarak al` | Harcama, ödeme ve kalan borcun anlamı açık etiketlenmeli; parantezli tutar kendi başına yeterli açıklama sayılmaz |
| Dahil edilen hesapları görünür anlatma | Money Manager / Bluecoins çapraz bulgusu | Nakit akışı filtresi ile havuz net varlığı ayrılmalı; kaynakların aynı ölçüyü değiştirdiği varsayılmaz |
| Tanınan tutarın tahsil edilen/bekleyen kırılımı | KolayBi `kararı yeniden sor` | Sunum ihtiyacı değerlendirilebilir; BF'nin veresiye satışı zaten tanıdığı unutulmaz. Kaynak ekran adı BF arayüzüne taşınmaz |
| KDV özetini oran/belge türüyle inceleme | KolayBi `kararı yeniden sor` | Mevcut toplamı daha ayrıntılı sunma sorusudur; vergi türetme veya beyanname üretme yetkisi değildir |
| Filtrelenmiş toplam ve görünür dönem | Bluecoins / Wallet / Hesap Defterim / Goodbudget | Hangi kayıtların dahil olduğu anlaşılmalı; karşılaştırmalı kullanılabilirlik ölçülmedi, “en iyi filtre” sıralaması yapılmaz |
| Tahmin ve geçmiş dönem kıyası | Wallet `henüz karar verme`; KolayBi Tema 06 | Formül ve vadesiz kalem sınırı açıklanmadan kesin tahmin vaadi verilmez |

### Yayılım ve sınırlar

Money Manager E0010 çapraz bulgusu, Bluecoins P2-G04 sınırına hizalandı.
KolayBi E0008 KDV karar satırındaki görünmeyen İndirilecek etiketi ve BF'de
KDV toplama yokluğu öncülü düzeltildi; karar etiketi korunup soru daraltıldı.
Diğer eski karar gerekçelerindeki mevzuat, kullanıcı ihtiyacı ve mali risk
ifadeleri doğrulanmış araştırma sonucu diye taşınmaz. Goodbudget Market satırının
görünmeme nedeni, normal gelir yolunun rapor sonucu ve açıklanamayan farklar
kesin kusur veya öneri gerekçesi yapılmaz. Yeni ürün kararı alınmadı.

**Paket kontrolleri:** Money Manager 30 ve KolayBi 39 kare için form denetimi 0 hata/0 uyarı; `git diff --check` temiz. Tema 08'de 33 farklı görsel adı ve 4 kod/ADR bağlantısı geçerli; tablo, boşluk, tekil başlık ve beş devir başlığı kontrol edildi. Bu kontrol görselleri yeniden inceleme değildir. Uygulama kodu değişmedi; build/test/analyze çalıştırılmadı, commit yapılmadı.

**Sıradaki tek paket: P4-tema-09 (veri aktarımı ve entegrasyon).**

## P4-tema-09 — Veri aktarımı, yedekleme ve entegrasyon — 15 Eylül 2026

**Durum: kapandı.** Kaynaklar: E0007 Bildiri/ek dosya/Drive ve P1-B11;
E0005 P2-G04 çıktı/veri yönetimi; E0010 ayar ızgarası ve kapsam dışı kayıtları;
E0014 P3-G04/WL-U ve dosya eki; E0006 B07/kaynak sınırı; E0008 cari ekstre,
form/listeler ve entegrasyon beyanı; E0011 kılavuz/video; E0009 müşavir portalı;
E0012 banka feed'i/kaynak akışı. Önceki inceleme sonuçları kullanıldı; yeniden
görsel, dış servis veya canlı uygulama koşumu yapılmadı. Rapor hedefi **Belge 1
§3–4** (çıktı seçimi, önizleme, paylaşım, bağlantı girişleri) ve **Belge 2 §3–4**
(verinin giriş/çıkış yolları ve dış sistem temasları). Tema 10 diğer modülleri
haritalar; bu bölüm o modüllerin iç işleyişinin yerine geçmez.

### Dosya dışa aktarma: seçenek, dosya ve teslim ayrı kanıtlardır

| Ürün | Görülen / kaydedilen yol | Dayanak | Kanıt sınırı |
|---|---|---|---|
| Hesap Defterim | Bildiri-Bütün Hesaplar: dönem + PDF/EXCEL; defter başına Bildiri: PDF/Excel. Android paylaşım yüzeyinde “2 dosya paylaşılıyor” | `34-bildiri-pdf-excel-secim.png`, `40-bildiri-defter-basina-pdf-excel.png`, `41-bildiri-kasadefteri-uyarisi.png` | PDF içeriği, dosya adları ve yeniden adlandırılan sütunların çıktıya yansıması **koşum notu**; kalıcı karelerde dosya içeriği yok. İki dosyanın türü/içeriği yalnız sayaçtan çıkarılmaz; alıcıya teslim kanıtı değil |
| Bluecoins | PDF veya Yazıcıya gönder / Excel (.csv) / HTML | `f7-54-print.png` | Gerçek dosya üretimi, paylaşım ve Premium son adımı denenmedi. Excel etiketi .csv uzantısı taşır; .xlsx ürettiği söylenmez |
| Money Manager | Toplam sekmesinde “Excel(.xlsx) e-posta olarak gönder”; ayarlarda Yedekle ve PC'den Yönet girişleri | E0010 arayüz taraması; `26-toplam-sekmesi-agustos.png`, `30-ayarlar-izgarasi.png` | Excel üretimi ve yedek/geri yükleme önceki kullanıcı kararıyla kapsam dışı. Gönder düğmesi e-posta gönderildiği anlamına gelmez |
| Wallet | Araştırılan Records menüsünde yalnız satır bakiyesi ve planlanan kayıt anahtarları; bu turda dışa aktarma görülmedi | `49-u03-records-menu-bakiye-planli-secenekleri.png`; E0014 P3-G04/WL-U03 | “Ürün dışa aktaramaz” sonucu çıkarılmaz; Settings'in bütün alt akışları denenmedi |
| Goodbudget | Dışa aktarma/yedek akışları protokol kapsamı dışında | E0006 Kanıt ve güven düzeyi | Yokluk veya biçim iddiası kurulmaz |
| KolayBi | Cari ekstrede tarih, para birimi ve yedi isteğe bağlı kolon; PDF önizlemede Yeni Sekmede Görüntüle/E-Posta ile Gönder. Proje, gider ve raporlarda Dışarıya Aktar girişleri | `d09-destek-cari-detay-ekstre-dialog.png`, `d10-destek-cari-ekstre-onizleme.png`, `d01-destek-proje-listesi.png`, `d11-destek-gider-listesi.png`, `d26-destek-alis-satis-raporu.png` | Resmî destek materyali; agent dosya üretmedi/göndermedi. Kolonların karede kapalı olması fabrika varsayılanını kanıtlamaz |
| Paraşüt | Formun KDV kılavuz aktarımında ay dökümü → Dışarı Aktar → Excel; giriş karuselinde faturayı paylaşma beyanı | E0011 KDV raporu; `01-ilk-acilis-carousel4.png` | Kılavuz/pazarlama aktarımı; indirme veya teslim denenmedi |
| Logo / QuickBooks Solopreneur | Logo müşavir erişimi, Solopreneur banka/fatura kaynak akışı anlatılıyor | E0009, E0012 | Bunlardan belirli CSV/PDF dışa aktarma yeteneği türetilmez; Solopreneur muhasebeci erişimi/dışa aktarması formda doğrulanamadı |

### İçe aktarma, ek dosya ve otomatik okuma

| Yol | Dayanak | Sınır |
|---|---|---|
| Bluecoins Excel (.csv) / QIF içe aktarma | `f7-40-diger-ayarlar.png` Veri Yönetimi | Dosya şeması, eşleme, hata/tekrar kaydı yönetimi ve sonuç denenmedi; banka ekstresi ayrıştırma başarısı kanıtı değildir |
| KolayBi listelerden İçe Aktar | `d01-destek-proje-listesi.png`, `d11-destek-gider-listesi.png`, `d37-video-urun-ve-hizmetler.png` | İçe aktarılabilecek biçim ve kayıt etkisi yalnız düğmeden çıkmaz |
| Hesap Defterim Fatura ekle | Kamera / Fotoğraf Galerisi / PDF seçimi; kayıt ataçı ve ek görüntüleme (`22-fatura-ekle-secenekler.png`, `24-kayit-eklendi-atac-ikonu.png`, `25-fatura-tam-ekran-goruntuleme.png`) | Ek dosya saklama; OCR ile tutar/tarih çıkarma kanıtı değil. Galerinin boş olması ortam kısıtı; PDF eklemek PDF'den veri içe aktarmak değildir |
| Wallet dosya/foto ekleme | `40-add-receipt-dosya-veya-foto.png` | Seçim öncesi diyalog; ücretsiz incelenen yol için OCR görülmedi. Ürünün bütün paketlerine yokluk genellenmez |
| KolayBi gider eki | `d05-destek-yeni-gider-formu.png` dosya türleri ve 5 MB sınırı | Formda görülen kabul metni; gerçek yükleme doğrulaması veya mobil OCR yokluğu kanıtı değil |
| Paraşüt / Logo fiş okuma | E0011/E0009 resmî ürün anlatımı | OCR sonucu, kullanıcı onayı ve hatalı okumanın düzeltme yolu çalışırken görülmedi; “otomatik kayıt” beyanı kesin uçtan uca davranış diye yazılmaz |

### Yedek, senkronizasyon ve veri konumu

- **Hesap Defterim:** Uygulama kayıtları sunucusunda saklamadığını söylüyor
  (`14-yedekleme-nag-dialog.png`); Drive yedek/geri yükleme yolu gerçek Google
  hesabıyla denenmedi. `43-ayarlar-alt-bolum-donem-baslangici.png` içinde otomatik
  e-posta ayarı işaretli; varsayılanı, alıcı, tetikleyici ve gönderim bilinmiyor
  (P1-B11). “Tamamen çevrimdışı / istemsiz gönderiyor” hükümleri taşınmaz.
- **Money Manager / Bluecoins:** Araştırılan kurulumlar yerel ve girişsiz;
  yeni emülatörde eski kayıtların bulunmaması önceki koşum notudur. Bu gözlem
  bütün ürünün bulut özelliği olmadığını veya yedeğin başarısızlığını kanıtlamaz.
  Yedek menüsü görmek geri yükleme testi yapmak değildir.
- **Wallet:** Eski çekirdek kayıtların sonraki oturumda korunması gözlendi;
  hesap/bulut kullanımı kayıtlı. Çok cihazlı çatışma çözümü, çevrimdışı kuyruk ve
  yedekten geri yükleme sınanmadı. Group sharing girişinin varlığı rol/izin kanıtı değil.
- **Goodbudget:** Household/hesap yüzeyleri görüldü; aktarım/yedek kapsam dışı.
  Household kavramı eşzamanlı düzenleme ve yetki kurallarının doğrulanması değildir.
- **KolayBi / Paraşüt / Logo / Solopreneur:** Bulut/web/mobil ifadeleri ürün
  kaynaklarının beyanıdır. Veri merkezi, şifreleme, kurtarma süresi veya yedek
  bütünlüğü bu araştırmada ölçülmedi; rapor bunlar hakkında hüküm kurmaz.

### Dış servis ve muhasebeci temasları

| Ürün | Kayıtlı temas | Kanıt sınırı |
|---|---|---|
| Wallet | Bank Sync ve Group sharing girişleri (`f7-54-hamburger.png`, `f7-55-menu-scroll.png`) | Banka bağlanmadı, grup paylaşımı yapılmadı. Karedeki hesap sahibi adı nihai teslimde karartılacak (P3-G04) |
| Goodbudget | Premium banka senkronizasyonu | E0006 GB-S03 resmî kaynak kaydı; bölge/kurum uygunluğu ve canlı bağlantı denenmedi |
| KolayBi | Banka, GİB e-belge, pazaryeri, sanal POS, geliştirici API ve çok müşterili muhasebeci erişimi | E0008 kaynak beyanı; `d39-guncel-arayuz-2026.png` davet/bağlantı girişleri. Kurum sayıları tarihsel beyan, güncel uyumluluk listesi değildir; bağlantı/ödeme/API çağrısı yapılmadı |
| Paraşüt | Banka, e-ticaret, online tahsilat, e-belge; muhasebeci canlı görüntüleme | E0011 kaynak kaydı; `05-video-banka-entegrasyonu.png`. Kaynakta anlatılan entegrasyonlar canlı denenmedi; Logo'nun işlem yapma yetkisi Paraşüt'e taşınmaz |
| Logo İşbaşı | Müşteri mali müşavir olarak ekledikten sonra müşavir müşteri adına işlem yapabilir | E0009 resmî ürün anlatımı; `06-video-musavir-portal.png` yalnız stilize boş liste. İkonlardan düzenle/sil izin matrisi çıkarılmaz; davet ve yetki iptali denenmedi |
| QuickBooks Solopreneur | Banka/kart feed'i → kategori/Type inceleme → Rules; kaynakta fatura ödeme yöntemleri | E0012 kaynak akışı; Solopreneur iç ekranı yok. Banka feed'ini etiketleme, tekrarlayan işlem üretme veya kullanıcının bankasından ödeme başlatmayla eşitlenmez |

### BusinessFinance karşılığı ve Belge 3'e devir

15 Eylül salt okunur kod kontrolü: [ImportUseCases](../../src/BusinessFinance.Application/Imports/ImportUseCases.cs)
CSV'yi aday satırlara alır; kullanıcıya ait hesap/kategori eşleme, yinelenen kayıt
kararı ve seçili satırları onaylama yolları ayrıdır. Dosya içe aktarma zaten vardır;
canlı banka bağlantısı değildir. [Mevcut akış belgesi](../../documentation/flows.md)
işlem/cari CSV, JSON ve backup için önizleme/paylaşma/cihaza kaydetme yollarını
anlatır. İşlem CSV çıktısı tam geri yükleme yedeğiyle aynı sözleşme değildir.
[Geri yükleme kaynağı](../../documentation/restore-runbook.md) şema ve geri yükleme
kurallarının tek kaynağıdır; sürüm numarası bu temada ikinci kez tutulmaz.

[ADR 0016](../../documentation/adr/0016-tax-fields-carry-they-do-not-calculate.md)
muhasebeci paketini cihazdan paylaşılan, işletme kayıtlarıyla sınırlı dosya olarak
seçmiştir. “Cari ekstresi yok” kıyası cari CSV'nin yokluğu anlamına gelmez;
KolayBi'nin karşı tarafa yönelik biçimlendirilmiş PDF önizlemesi ayrı bir sunumdur.

| Aday | Kaynak etiketi / bağ | Değerlendirme sınırı |
|---|---|---|
| PDF/yazıcı, CSV ve HTML seçenekleri | Bluecoins `uyarlayarak al` | Görülen biçim menüsü; rakipte üretim/başarı denenmedi. BF mevcut CSV/yedek yoluna biçim ekleme ihtiyacı ayrı değerlendirilir |
| Cari ekstre PDF ve alıcıya paylaşma | KolayBi `henüz karar verme` | Muhasebeci paketiyle amaç aynı değil; karşı taraf özeti ihtiyacı ölçülmedi |
| Dönem seçimi → dosya → sistem paylaşımı | Hesap Defterim Bildiri | Kareli seçim/paylaşım, karesiz dosya içeriği ayrımı korunur; “sağlam export” genel kalite hükmü verilmez |
| Yedek daveti ve veri konumunu anlatma | Hesap Defterim P1-B11 | Sunucuda saklamama uygulama beyanı; tekrar eden uyarının kullanıcıya etkisi ölçülmedi |
| Muhasebeci canlı erişimi | Paraşüt/Logo/KolayBi kaynakları | Üç ürünün yetkileri ortaklaştırılmaz. Mevcut dosya paylaşımından farklı bir sahiplik/yetki kararıdır |
| Banka / e-belge / pazaryeri bağlantıları | Kaynak formlar | Rakip özellik haritasında tutulur; bu araştırma BF'ye entegrasyon ekleme kararı değildir. Banka bağlantısı ve ödeme başlatma için mevcut ürün sınırı korunur |

**Sonuç:** Dosya eki, kayıt içe aktarma, rapor çıktısı, tam yedek, bulut
senkronizasyonu ve üçüncü kişiye erişim verme ayrı akışlardır. Birinin kanıtı
diğerinin başarılı veya güvenilir olduğunu göstermez. Kaynak formlarda bu tema
kapsamında yeni bir düzeltme gerekmedi; mevcut P1/P2/P3 sınırları taşındı.

**Paket kontrolleri:** `git diff --check` temiz; 27 farklı görsel adı ve 4 kod/belge bağlantısı geçerli. Tablo sütunları, boşluklar, tekil tema ve beş devir başlığı doğrulandı. Görsel içerikleri yeniden incelenmedi. Form değişmediği için form denetimi tekrar koşulmadı. Uygulama kodu değişmedi; build/test/analyze çalıştırılmadı, commit yapılmadı.

**Sıradaki tek paket: P4-tema-10 (diğer ürün modülleri).**

## P4-tema-10 — Diğer ürün modülleri ve yardımcı araçlar — 15 Eylül 2026

**Durum: kapandı.** Kaynaklar: E0007 ek koşum 2 ve P1-B14; E0008 personel,
maaş, çek/senet, ürün/hizmet ve Notlar; E0011 stok/operasyon kaynakları;
E0009 operasyon ve sesli fatura; E0012 ürün kimliği/km takibi; E0005 P2-G04;
E0014 P3-G04 menü taraması; E0010 ayar ızgarası; E0006 ürün kapsamı ve
household/zarf sınırları. Mevcut incelemeler kullanıldı; yeni görsel veya canlı
koşum yapılmadı. Rapor hedefi **Belge 1 §1–4** (ürün kapsamı, gezinme, araç
formları) ve **Belge 2'nin diğer özellik yüzeyleri**. Belge 3 adayları karar
değildir; modülün rakipte bulunması kullanıcı ihtiyacı ölçümü sayılmaz.

### İşletme operasyonları: stok, personel ve kıymetli evrak

| Ürün / modül | Gözlenen veya kaynakta anlatılan | Dayanak | Sınır |
|---|---|---|---|
| KolayBi ürün/hizmet | Tür, kod, ad, etiket, alış/satış fiyatı, KDV/indirim kolonları; Tümü/Ürünler/Hizmetler/Depolar/Varyantlar | `d37-video-urun-ve-hizmetler.png`, `d31-destek-urun-varyantlar.png` | Resmî kaynak; ürün tablosu boş, varyant sayfasında yalnız Yeni Varyant var. Stok hareketi, varyant modeli ve fiyat/vergi hesabı doğrulanmadı |
| Paraşüt stok/depo | Depo başlığı, adres, Ürünler/Stok Geçmişi; miktar ve vergiler hariç alış/satış sütunları. Kaynakta çok depo, transfer, sevkiyat | `06-video-stok-depo.png`; E0011 özellik tablosu | Tanıtım karesi; kırmızı satırın kritik stok anlamı çıkarım. Gerçek alım/satım sonrası stok değişimi test edilmedi |
| Logo operasyon | Stok, teklif ve sipariş; kaynakta sesli fatura girişi | E0009 Resmî kaynak ve sesli komut bölümleri | İç akış görülmedi. Sesli girişin tanıma başarısı, düzeltme/onay adımı ve stok etkisi ölçülmedi; otomasyon kalitesi iddiası kurulmaz |
| KolayBi personel carisi | Serbest/Yarı Zamanlı/Tam Zamanlı Çalışan; bakiye, Maaş/Prim Oluştur, Ödeme Yap ve Avans Ver | `d14-destek-personel-carileri.png`, `d15-destek-personel-cari-detay.png`, `d16-destek-maas-odeme-secimi.png` | Resmî destek ekranları; aynı kişi farklı çalışma tipi/bakiye gösterdiğinden kareler aynı veri anının önce/sonrası değildir |
| KolayBi maaş seçimi | Ödeme yapılacak maaşı seçme diyaloğu; Çalışan Maaşı detayında Ödenmedi, Ödeme Planı, brüt/net ücret alanları | `d16-destek-maas-odeme-secimi.png`, `d17-destek-calisan-maasi-detay.png` | Seçim sonrası sonuç yok; brüt/net alanlar aynı tutarda. Bordro/kesinti hesabı ve bağlı kısmi ödeme kanıtlanmadı. Tekrarlı maaş Tema 06'dadır |
| KolayBi çek/senet | Çekte keşideci, hamil, vade ve Toplu Çek Ekle/Bordrolar; senette kefil, ödenen/kalan toplam ve taksit durumu | `d22-destek-cekler.png`, `d23-destek-senetler.png` | Senet tablosu boş; kısmi ödeme, ciro ve taksit davranışı kolon adından doğrulanmaz. Senet genel borç/taksit modeliyle eşdeğer sayılmaz |
| Paraşüt / Logo diğer işletme yüzeyleri | Paraşüt kaynakta çek-senet ve maaş/prim kaydı; Logo kaynakta çek giriş/çıkış | E0011/E0009 özellik ve sistem akışı | Kaynak beyanı; çek/senet yaşam döngüsü ve bordro çalışırken görülmedi. Maaş kaydı bordro hesaplama kanıtı değildir |

### Günlük kullanım araçları ve genişleyen menü

| Ürün / araç | Gözlenen | Dayanak | Sınır |
|---|---|---|---|
| Hesap Defterim Not Defteri | Tarih/saat, metin, tamamlandı kutusu, arama, tarih filtresi ve tamamlandı/beklemede sayaçları | `30-not-defteri-checklist.png` | Finansal yükümlülük değildir; notun silme davranışına ilişkin koşum notu işlem iptal modeliyle birleştirilmez |
| Hesap Defterim Nakit Hesap Makinesi | Kupür × adet satırları ve toplam; sayı girişine göre tutar üretme önceki koşumda kaydedildi | `31-nakit-hesap-makinesi.png` | Finansal hareket üretmeyen yardımcı araç; karede para simgesi yok, TL nitelemesi çıkarım |
| Hesap Defterim Öğe eklemek | Öğe/miktar/birim/fiyat ile kalem toplamı; iki kalem toplamı 150'nin tutar ve not alanına taşınması | `20-oge-eklemek-dialog.png`, `21-oge-eklemek-tutar-notlar-otomatik.png` | Kayıt giriş yardımcısı; ürün kataloğu, stok veya ticari fatura modülü kanıtı değil |
| Hesap Defterim ayrı ürün bağlantıları | Veresiye ve gelir-gider uygulamalarına yönlendirme | `42-drawer-diger-uygulamalar-veresiye-gelirgider.png` | Bağlantı hedefleri test edilmedi; aralarında senkronizasyon olmadığı veya üç ayrı yedek gerektiği ölçülmedi |
| KolayBi Notlar | Başlık/not, Hatırlatıcı Pasif/Aktif, Özel Not/Şirket Notu; tarih ve not grubu filtreleri | `d25-destek-notlar-hatirlatici.png` | Kaynak ekranı; bildirim teslimi ve özel/şirket izin davranışı denenmedi |
| Wallet ek modüller | Investments, Shopping lists, Warranties, Loyalty cards, Currency rates; kayıt ayrıntısında Warranty alanı | `f7-54-hamburger.png`, `f7-55-menu-scroll.png`, `21-b2-islem-detay-taksit-alani-yok.png` | Menü/alan varlığı; yatırım getirisi, alışveriş listesi ve garanti takibi sonuçları test edilmedi. Group sharing Tema 09'dadır |
| Bluecoins Seyahat Modu | Çekmecede kapalı anahtar; kur/para birimi tercihleri | `f7-49-seyahat-modu.png`, `f7-44-gelismis-ayarlar.png` | Seyahat açılmadı; `f7-50-seyahat-toggle.png` adı yanıltıcı, içerik etiket seçimidir. Döviz dönüşümü denenmedi |
| Money Manager CalcBox / PC'den Yönet | Ayar ızgarasında iki ayrı giriş | `30-ayarlar-izgarasi.png` | İç akışları görülmedi. CalcBox, Hesap Defterim kupür aracıyla yalnız adından eşitlenmez; PC girişinin ağ/mimari davranışı çıkarılmaz |
| QuickBooks Solopreneur km takibi | Ürün kimliği/kaynak anlatımında kilometre takibi | E0012 ürün kapsamı | Solopreneur ekranı yok; GPS kaydı, izin, otomatik yolculuk tanıma veya vergi hesabı sonucu doğrulanmadı |
| Goodbudget | İncelenen yüzey zarf bütçelemesi, hesaplar ve raporlar; ek işletme operasyon modülü gösterilmedi | E0006 ürün kapsamı ve arayüz taraması | Diğer paketlerin tamamı taranmadı. Debt desteği Tema 07, household/banka bağlantısı Tema 09 sınırlarıyla korunur |

### Daha önce eşlenen yüzeyler ve kalan kapsam

Bu tema önceki bulguları tekrar yeni özellik olarak saymaz:

- Ürün/paket, giriş ve ödeme duvarı: Tema 01; gezinme ve genel ayar kapıları: Tema 02.
- Şablon, split, ek/fiş ve hızlı giriş: Tema 03; aktarım/OCR sınırları ayrıca Tema 09.
- Hesap, kart ve transfer: Tema 04; proje/etiket/ortak carisinin kapsamla ilişkisi: Tema 05.
- Bütçe, hedef ve finansal hatırlatıcı: Tema 06; borç/fatura/tahsilat: Tema 07.
- Proje, KDV ve diğer rapor menüleri: Tema 08; e-belge/banka/online ödeme/API ve
  muhasebeci erişimi: Tema 09. KolayBi Fatura Ödeme girişinden yeni ödeme koşumu çıkarılmaz.
- Reklam, üyelik, yardım/destek, güvenlik ayarı ve sosyal davet girişleri görüldükleri
  ölçüde ürün yüzeyine dahildir; bütün iç ekranları veya teknik altyapıları denenmiş
  sayılmaz. Uygulama güvenlik denetimi bu araştırmanın sonucu değildir.

### BusinessFinance karşılığı ve Belge 3 adayları

**P1-B14 kapanışı korunur:** BusinessFinance'te fiziksel kasa sayımı, beklenen/sayılan
farkı ve açık onayla düzeltme akışı zaten vardır. Eksik olduğu saptanan şey yalnız
kupür × adet yardımcısıdır; “kasa sayımı yok” diye yeni ihtiyaç yaratılmaz. Dayanak
bu kaydın P1-B14 kod/istemci incelemesidir; bu tur aynı kontrol tekrarlanmadı.

| Aday | Kaynak etiketi / bağ | Açık soru ve sınır |
|---|---|---|
| Kupür × adet yardımcısı | Hesap Defterim P1-B14 | Mevcut sayım formuna giriş kolaylığı gerekli mi? Sayım modelini baştan kurma işi değildir; öncelik ölçülmedi |
| Kalemden tutar ve not üretme | Hesap Defterim Öğe eklemek | Seri giriş ihtiyacını karşılar mı? Stok/fatura kapsamını kendiliğinden açmaz |
| Not/görev yüzeyi | Hesap Defterim ve KolayBi | Finansal vade ile genel görevin aynı üründe yaşaması gerekli mi? Bildirim ve izin sonuçları bilinmiyor |
| Stok/depo/katalog | Paraşüt `henüz karar verme`; KolayBi/Logo kaynakları | Ürün kapsamı kararıdır; demo stok veya menü, hedef kullanıcı ihtiyacını kanıtlamaz |
| Personel maaş/prim/avans | KolayBi kaynak formları | Genel cari kayıt yeterli mi, ayrı personel akışı gerekli mi? Bordro motoru varlığı varsayılmaz |
| Çek/senet | KolayBi/Paraşüt/Logo | Kendine özgü belge yaşam döngüsü gerektirir; mevcut borç/taksit modeliyle eşdeğer kabul edilmez |
| Yatırım, garanti, sadakat, seyahat, km | Wallet/Bluecoins/Solopreneur | Çoğu yalnız menü veya kaynak beyanı; uygulanabilirlik, kullanım sıklığı ve ürün katkısı doğrulanmadı |

Bunlar yeni ürün kararları değildir; [araştırma planının](FAZ7-8-UYGULAMA-PLANI.md)
Belge 3 değerlendirmesine girdidir. Kaynak etiketleri aynen kabul edilmiş öneri
sayılmaz. [Aktif ürün aşaması](../../stages/06.2-arayuz-duzeni.md) ve ürün kapsamı
bu araştırmayla değişmedi. Kaynak formlarda bu paket için yeni düzeltme gerekmedi.

**On temanın eşlemesi tamamlandı.** Bu, Faz 8'in veya araştırmanın tamamlandığı
anlamına gelmez. P4-K; kapsam, kanıt/bulgu kayıtları, kalan sınırlar, yayılım ve
rapor geçiş özetini ayrıca kontrol eder. Faz 8 kullanıcı geçiş onayını bekler.

**Paket kontrolleri:** On tema başlığı sıralı ve tekil; Tema 10'da 22 farklı görsel adı ve 2 belge bağlantısı geçerli. Tablo/boşluk kontrolü, beş devir başlığı ve `git diff --check` geçti. Bu kontrol P4-K kabul kapısının yerine geçmez. Görseller yeniden incelenmedi; formlar değişmediği için form denetimi tekrarlanmadı. Kod değişmedi; build/test/analyze çalıştırılmadı, commit yapılmadı.

**Sıradaki tek paket: P4-K (geçiş kontrolü ve onay özeti).**

## BC-U01 ve HD-U01 — Kullanıcı ek kontrolü — 15 Eylül 2026

P4-K sırasında gelen yeni kanıt nedeniyle geçiş özeti henüz sonlandırılmadı.

### BC-U01 — Silme/geri yükleme sonrası liste ve toplam farkı (B10)

- Kullanıcı beyanı: Cüzdan hesabına 10 TL gider eklendi; silince işlem ve bakiye etkisi kalktı. Çöp Kutusu'ndan geri yüklenince −10 TL etkisi döndü, işlem listesinde satır görünmedi. Ardından ayrı 5 TL gider eklendi; bu satır göründü, silinmedi.
- BC-U01-A: `kanitlar/bluecoins/işlemler.png`, 11:34. 15 Eylül başlığında −15 TL; görünen tek satır “gider” −5 TL, Cüzdan satır bakiyesi −15 TL. Ardından 11 Eylül başlığı geliyor; “Silme denemesi” satırı bu görünümde yok.
- BC-U01-B: `kanitlar/bluecoins/ögeler özeti.png`, 11:35. 1–30 Eylül döneminde “Silme denemesi” −10 TL ve “gider” −5 TL ayrı ayrı görünür.
- Sonuç: Rapor 10 TL kaydı içeriyor; yakalanan İşlemler görünümünde satır görünmezken gün toplamı iki giderin toplamını içeriyor. Bu, gözlenen liste/toplam farkıdır. Silme ve geri yükleme geçişleri kullanıcı beyanıdır; başlangıç ve ara adımların kareleri yoktur. Genel veri kaybı, kalıcı silme veya kök neden kanıtlanmadı.
- Hesap ayrımı: İşlem Cüzdan hesabındadır. Ortak Cüzdan'ın 4.150 TL değeri ayrı hesabın tarihsel işlem satırında görünür. Kullanıcının 4.140 TL “Nakit” toplamı beyanı bu iki karede görünmez; Cüzdan bakiyesiyle aynı alan sayılmaz.
- Tek ek kontrol: Yeni işlem eklemeden uygulamayı kapatıp yeniden aç; aynı tarih/hesap görünümünde “Silme denemesi” satırını kontrol et. Hâlâ yoksa adıyla ara ve etkin filtreleri görüntüle. Amaç yenileme/görünüm olasılığını ayırmaktır; şimdilik kök neden hükmü yok.
- B10 genel durumu kapsamı sınırlı kalır: geri yükleme etkisi kullanıcı kontrolüyle desteklendi, liste tutarsızlığı yeni gözlemdir; otomatik tekrar ve gelecekte ilk taksit denenmedi. Eski “silme/geri yükleme hiç ölçülmedi” ifadeleri bu ek kayıtla güncellenir; önceki paketler tarihseldir.

### HD-U01 — E-posta adresi bulunamadı (B11)

Kullanıcı ayarlarda adres bulamadığını bildirdi. E0178 tekrar açıldı: Ayarlar alt bölümünde “Verileri sil” altında, “Ekranını açık tut” üstünde “İşlem dökümünü e-posta ile otomatik gönder” kutusu var; adres alanı yok. Adresin konumu bilinmiyor. Kullanıcıdan gönderim veya reset istenmedi. B11 kapsamı sınırlı kalır; kutu alıcı/tetikleyici/teslim kanıtı değildir.

**BC-U01 kayıt kontrolü:** İki görsel yerel dosyadan açıldı; envantere SHA-256 ile işlendi. Hesap Defterim form denetimi temiz. Bluecoins denetimi 4 hata verdi: denetim.cjs:40 ASCII dosya adı deseni nedeniyle Türkçe/boşluklu iki adı kırpıyor (2 ölü atıf, 2 yetim kare). Dosyalar mevcut; bu bir araç kapsam eksikliği, görsellerin eksikliği değil. Kullanıcı dosyaları yeniden adlandırılmadı. P4-K öncesinde bu mekanik engel giderilmeli; kapı temiz sayılmaz.
*(P4-K, 15 Eylül: engel giderildi; aşağıdaki P4-K bölümü.)*


### BC-U01 / HD-U01 takip sonuçları

15 Eylül takip sonucu (kullanıcı beyanı, yeni görsel yok): Kullanıcı Bluecoins uygulamasını kapatıp yeniden açınca “Silme denemesi” satırının İşlemler listesine geldiğini bildirdi. BC-U01 yeniden açma kontrolü tamamlandı; bu olayda liste görünümü yeniden açılışla düzeldi. Önceki iki kare tutarın raporda/toplamda bulunduğunu gösterir. Liste yenilemesiyle ilgili geçici bir sorun olasılığı desteklenir; teknik kök neden veya bütün sürümlere genelleme yapılmaz. Yeni arama/filtre kontrolü gerekmiyor. B10 otomasyon ve diğer denenmemiş dallar nedeniyle genel olarak kapsamı sınırlı kalır.

15 Eylül takip sonucu (kullanıcı beyanı, yeni görsel yok): Otomatik e-posta kutusu açılıp kapanıyor; dokununca pencere veya e-posta seçimi açılmıyor. Kullanıcının ilk girişte alınmış adrese gönderiliyor olabileceği düşüncesi hipotezdir. Önceki koşum notunda uygulamanın giriş/e-posta istemediği yazılıdır; ilk girişte adres alındığı doğrulanmış değildir. Alıcı, tetikleyici ve gerçek teslim bilinmiyor; gönderim var/yok sonucu çıkarılmaz. HD-U01 görünür ayar kontrolü tamamlandı, ek adres araması gerekmiyor; B11 kapsam sınırı korunur.

## P4-K — Geçiş kapısı ve onay özeti — 15 Eylül 2026

**Durum: kapandı (kullanıcı onayı maddesi hariç).** Faz 8 açılmadı; deneme belgesi
yazılmadı. Tamamlanmış görsel incelemeleri tekrar edilmedi; yalnız envanter/atıf
bütünlüğü ve denetim aracı yeniden çalıştırıldı. Ürün kodu değişmedi; commit yok.

### Mekanik engelin giderilmesi (denetim.cjs)

| Konu | Önce | Sonra |
|---|---|---|
| Tırnaksız/tırnaklı ad deseni | `[A-Za-z0-9_.-]`; `işlemler.png` → `lemler.png`, `ögeler özeti.png` → `zeti.png` | Unicode harf/rakam (`\p{L}\p{N}`); Türkçe ad kırpılmaz |
| Boşluklu ad | Desteklenmiyor | Yalnız tek ters tırnaklı kod parçasının **tamamı** `.png` yoluysa tek atıf; tırnaksız metinde birleştirilmez, parça içinde ikinci `.png` varsa ayrı adlar |
| NFC/NFD | Farklı kodlanmış aynı ad bulunamazdı | Ad NFC ile eşleştirilir; farklı ad hâlâ ölü atıf |
| Gevşeme | — | Yok: benzer ad kapsanmaz (`islemler.png`, `eski ögeler özeti.png` ölü atıf ve gerçek kare yetim kalır); kısa kod, aralık, yasak kalıp, tablo ve `--siki` davranışı değişmedi |

`denetim-test.cjs`e altı test eklendi: iki gerçek BC-U01 adı temiz; olmayan dört benzer ad tam
adıyla ölü atıf ve iki gerçek kare yetim; tırnaksız Türkçe ad ve yollu boşluklu ad; tırnaksız
boşluklu ad birleşmez; tek parçadaki iki ad; NFD disk adı ve çift ters tırnaklı örnek.
Kullanıcı dosyaları yeniden adlandırılmadı.

Yeni hash (E0393/E0394 envanter satırları P1-K anını gösterir): `denetim.cjs`
`1a16dfbd1e1b7ceb68e644d50d15890971c208b5d1158b77f0da20ec5469a1dc`, `denetim-test.cjs`
`09a33396ab376ba8fc4b3786be656ad5587ff93ad79afb1bbc069c40aa94432e`.

### Kontroller

| Kontrol | Sonuç |
|---|---|
| `node denetim-test.cjs` | 21/21 (15 önceki + 6 yeni), çıkış 0 |
| Hatada başarısızlık | Kopyada eski ASCII desen geri konunca 6 yeni test düştü (15/21, çıkış 1) ve Bluecoins'te aynı 4 hata döndü (çıkış 1) |
| `node denetim.cjs`, `--siki`, `denetim.sh bluecoins hesap-defterim` | Dokuz form 0 hata / 0 uyarı; çıkış 0. Bluecoins 92, Wallet 109, Goodbudget 31 kare |
| Envanter | Diskte 365 PNG (357 + GB-U01 3 + WL-U 3 + BC-U01 2); her PNG'nin envanter satırı var; envanterdeki 581 yerel bağlantının 0'ı kırık; BC-U01-A/B SHA-256 diskle aynı |
| İnceleme kaydı | P0.1 kimliklerinin tamamı sonraki G kaydında; kimliği envanter ayrıntısında geçmeyen 13 Bluecoins karesi (E0017–E0031 arası) E0392 P2-G01 tablosunda |
| Boşluk kontrolü | `git diff --check` ve `--cached --check` temiz (izlenen dosyalar). İzlenmeyen beş dosya (bu kayıt, plan, envanter, iki `.cjs`) `git diff --no-index --check` ile ayrıca tarandı: 0 sorun |
| Bağlantı notu | Envanterdeki `raporlar/Soru%201.md` ve `raporlar/soru%202.md` URL kodlu geçerli bağlantılardır; çözülünce var |

### B01–B19 son durum

| Durum | Bulgular |
|---|---|
| Kanıtla kapalı (14) | B01, B02, B03, B04, B05, B07, B09, B13, B14, B15, B16, B17, B18, B19 |
| Kapsamı sınırlı (5) | B06 (Goodbudget nedensellik), B08 (KolayBi demo adları), B10 (Bluecoins silme/otomasyon), B11 (Hesap Defterim e-posta), B12 (Hesap Defterim transfer bacağı) |
| Açık | 0 |

B10: BC-U01 ile geri yüklenen tutarın rapora ve toplama döndüğü iki karede görülür; listede
eksik satırın yeniden açılışla geldiği kullanıcı beyanıdır. Kök neden bilinmiyor. Açık B1
otomasyonu, geleceğe kurulan ilk taksit, kalan taksitlerin gerçekleşmesi, Bugün kolu,
ekstre/dönem ve gerçek çıktı dosyası denenmedi; tamamlanmış sayılmaz. B11: kutu açılıp
kapanıyor, pencere/adres seçimi açmıyor (kullanıcı beyanı); alıcı, tetikleyici ve teslim
bilinmiyor; ilk girişte adres alındığı hipotezdir.

### Faz 8 geçiş kapısı (plan Bölüm 7)

| Kutu | Karar | Dayanak |
|---|---|---|
| Kapsam ve erişim sınırı | ✓ | P4-tema-01 A–D grupları |
| Bluecoins/Wallet Faz 7.5 | ✓ | P2-K, P3-K; ölçülmeyen dallar 14 Eylül kapanış kararıyla sınır |
| Görseller | ✓ | 365/365; yukarıdaki envanter kontrolü |
| Uygulama MD'leri | ✓ | P0.3 dokuz harita; P2-T, P3-T |
| Çelişkiler | ✓ | P4-B19; tema taşıma nitelemeleri |
| B01–B19 ve yeni bulgular | ✓ | Yukarıdaki tablo; GB-U01, WL-U01–U04, BC-U01, HD-U01 |
| BF karşılaştırmaları | ✓ | P1-B01–B04, P1-B14, P3-T, tema 06–10 kod okumaları (14–15 Eylül kod anı; bu tur yeniden okunmadı) |
| Paket/platform/sürüm/tarih | ✓ | P1-B13, P1-B15, WL-Q01, BC-Q01/Q16; güncel sürüm doğrulanmış sayılmaz |
| Yayılım | ✓ | P4-B19; bu paketin başlık uzlaştırması (DURUM, README, protokol, TUR2, plan, bu kayıt) |
| Mekanik kontroller | ✓ | Yukarıdaki kontroller |
| Tematik aktarım | ✓ | P4-tema-01–10 |
| Kullanıcı onayı | Açık | Agent işaretlemez |

Engel: yok. Kullanıcı onayı kapının son maddesidir, engel değildir.

**Sıradaki tek iş:** kullanıcı DURUM.md geçiş özetini görüp Faz 8'i açıkça onaylar. Onaydan
sonra önce P5-I (içindekiler), ardından P5-P (örnek bölüm ve teknik dışa aktarım denemesi).
Kullanıcı pilotu değerlendirmeden tam rapor üretimine geçilmez.

## Faz 8 açılışı ve P5-I — 15 Eylül 2026

Kullanıcı P4-K özetini paylaşarak “gpt her sey tamamsa faz8 gecebılırız” dedi. Kayıtlı kapı sonuçları kontrol edildi, açık onay işlendi; 12/12 tamam. [Ortak içindekiler](raporlar/0-ortak-icindekiler.md) hazır: iki belgenin bölüm düzeni, on tema eşlemesi ve pilot kapsamı. Pilot henüz yazılmadı; sıradaki tek paket P5-P. Önceki onay bekleyen ifadeler tarihsel kayıttır. Kaynak formlar, kanıtlar ve uygulama kodu değişmedi; mevcut main.dart değişikliği korundu. Başarılı P4-K testleri tekrarlanmadı. Commit yapılmadı.

## P5-P — Pilot ve teknik dışa aktarım — 15 Eylül 2026

Pilot üretimi tamam; kullanıcı değerlendirmesi bekleniyor. [PDF](raporlar/pilot/pilot-islem-ekleme.pdf), [Word](raporlar/pilot/pilot-islem-ekleme.docx), [Markdown](raporlar/pilot/pilot-islem-ekleme.md). Altı sayfa: okuma kılavuzu, üç uygulamanın görselli arayüz incelemesi, finansal akış örneği, kanıt/değerlendirme bölümü. Money Manager, Wallet, Hesap Defterim; E0229/E0228/E0277/E0279/E0138/E0141 seçildi ve tekrar görüntülendi. Form/sonuç karelerinin aynı kaydın kesintisiz zinciri olmadığı belirtildi. Wallet gider formu gelir sonucu sayılmadı. Yeni canlı koşum yok.

Ortam: mevcut python-docx 1.2.0 ve Word; Git dışında yerel venv + PyMuPDF 1.28.2. Markdown tek içerik kaynağı; uret.py Word'ü, export-word.ps1 aynı Word sayfa düzeninden PDF'yi üretir. Gereksinimler ve yeniden üretim adımları pilot/README.md içinde. ui-ux-pro-max rehberinin form/geri bildirim/okunabilirlik ilkeleri kullanıldı; ölçülmemiş kullanılabilirlik veya erişilebilirlik sonucu üretilmedi.

Kontroller: 6/6 PDF sayfası görsel incelendi; 6 gömülü görsel, 4 göreli kaynak bağlantısı, gömülü Calibri fontları, Türkçe karakter ve tutar metinleri, sayfa sınırları doğrulandı. Altı görsel kopyasının hash'i özgün kanıtla aynı. Yeni dosyalar dahil metin/bağlantı/boşluk kontrolleri yapılır. Başarılı P4-K kapısı tekrarlanmadı; kaynak formlar ve uygulama kodu değişmedi. Commit yok.

**Sıradaki tek iş:** kullanıcının pilotun ayrıntı, dil, görsel büyüklüğü ve başvuru değeri hakkında geri bildirimi. Tam rapor ve Belge 3 başlamadı. Bu teslim, pilot kabulü veya Faz 8 kapanışı değildir.

## 20 Eylül 2026 — Belge 2 K1/K2/K3 ve S1 kapanışı

Bu kayıt, önceki tarihli açık soru ve tema sonuçlarının güncel ekidir. Yeni canlı test yapılmadı;
Claude'un 20 Eylül koşumundan kalan 22 kare (E0403–E0424) tek tek görsel olarak denetlendi.
Sekiz eski karenin E0395–E0402 kimlik eşlemesi ve 387 özgün görselin tamamının yol/boyut/SHA-256
bütünlüğü doğrulandı. Özgün görseller değiştirilmedi.

| İş / soru | Kapanan kapsam ve kanıt | Kapanmayan sınır |
|---|---|---|
| K1 — MM kısmi ödeme | E0403–E0405: Eylül'de 600 abonelik + 1.000 taksit = 1.600 gider; 400 havale ayrıca kart ödeme parantezinde. İncelenen raporda ödeme gidere eklenmiyor | Tam ödeme ve bütün dönem/ayar kombinasyonları sınanmadı |
| K2 — MM-Q03 | E0406–E0416: kalan dört taksitin görünür yeri bulundu; Ekim–Ocak gün listelerinde (3/6–6/6) ve aylık 1.000 gider toplamlarında. Şubat işlem toplamı sıfır; −600 tekrar önizlemesi sürüyor. **Görünür yer sorusu kapandı** | Orijinal sorudaki üretim eşiği ve arka plandaki saklama biçimi belirlenmedi (B12); ayrı plan nesnesi yokluğu çıkarılmaz |
| K3 — GB-Q01 | E0417–E0424: From New Income / Fill Each Envelope, +1.234 sentetik giriş; zarf 0 → 1.234, Total 43.784 → 45.018, hesap 41.734 → 41.734. Liste +1.234'ü Ana Hesap adıyla gösteriyor; Eylül Income 3.284. **Bu yolun gözlenen sonuç sorusu kapandı** | İlk kurulumdaki Initial Fill fonlama seçimi, Keep Available/From Available sonuçları ve hesap değişmeme nedeni bilinmiyor. Önceki rapor 2.050 başka koşumdan (E0121/E0124); aynı koşumun önce raporu değil |
| S1 | Sekiz eski kare E0395–E0402, yeni 22 kare E0403–E0424; dizin 357 → 387 kare, 422 kimlik | Yeni kanıt kimliği verilmesi mevcut gözlemi daha derin bir test yapmaz |

**Nitelemeler:** MM Hesaplar Borçlar 1.600 (11:05) ve Ocak 2027 kart defteri Bakiye 5.600 (11:07)
aynı koşumdaki farklı dönem kapsamlarıdır; eşzamanlı çelişki diye yazılmaz. Ekim–Ocak dört taksit
4.000 farkıyla aritmetik olarak tutarlıdır. Goodbudget'ın yeniden açılması koşum kaydıdır;
kare son değeri doğrular, gecikmenin bütün türlerini dışlamaz. Bütçe bloğundaki %0'ın nedeni
ölçülmedi. Zarf seçeneklerinde No change ve dört doldurma kipi birlikte görülüyor.

**Devirde bulunan ve düzeltilenler:** Gözlem formlarında 12 mekanik hata (kısa numaralı atıflar,
belirsiz aralık ve yetim kareler); güncel plandaki eski açık kayıtlar; ayrı plan nesnesi yokluğu ve
gecikme olasılığının elendiği yönündeki aşırı ifadeler. Bluecoins'in Türkçe/boşluklu iki dosya adı
korundu: mevcut denetleyici bunları destekliyor. Goodbudget hane adı teslim kopyasında karartılır.

**Üretim sınırı:** Eski 10 sayfalık kurgu denemesinin motor kontrolleri, nihai Belge 2'nin
planlanan etki-boyutu kapısının uygulanmış olduğu anlamına gelmez. Yeniden üretimde Windows
konsol kodlaması hatası görüldü; Python komutları `-X utf8` ile çalıştırılır. Belge 2 bölüm yazımı
bu devir tamamlamasının kapsamında başlatılmadı; konu içinde A → C kararı korunur.

### 20 Eylül — ikinci devir kontrolü (Claude)

Codex'in devir denetiminden sonra bağımsız bir tur daha yapıldı. Üç bulgu; hiçbiri koşum
sonucunu değiştirmiyor, üçü de kanıtın yazıma nasıl gireceğiyle ilgili.

| # | Bulgu | Durum |
|---|---|---|
| 1 | **Karartma yalnız düz metinde yazılıydı, motorda değildi.** Üretim motorunun `KARARTMA_ORTAK` listesi altı eski Goodbudget karesini sayıyordu; 20 Eylül'ün hane adı taşıyan altı yeni karesi (E0417, E0418, E0420–E0423) listede yoktu. Belge 2 bunlardan birini bassaydı hane adı karartılmadan çıkardı | **Düzeltildi.** Altısı listeye eklendi; kutu görsel olarak doğrulandı (adı tam örtüyor). E0419 ve E0424 `Fill Envelopes` başlığındadır, hane adı taşımaz, bilerek dışarıda |
| 2 | **Bu koşumda iki ayrı ₺1.600 var ve bileşimleri farklı.** Eylül gideri 1.600 = 600 abonelik + 1.000 taksit; Is Karti borcu 1.600 = 600 (Ağustos taksiti − 400 ödeme, E0249) + 1.000 (Eylül taksiti). Aynı gözlem formunda on beş satır arayla duruyorlardı, aralarındaki fark yazılı değildi | **Düzeltildi.** `gozlemler/money-manager.md` K2'ye bileşim tablosu eklendi. ₺600 abonelik gidere girer borca girmez; ₺400 havale borcu düşürür gidere girmez |
| 3 | **Yedi kırık bağlantı** — `belge2-claude-code-plan-review.md` mutlak Windows yolu (`/C:/Users/...`) taşıyordu. Plana verilen iki satır numarası v1'e aitti ve v2'de bambaşka yere düşüyordu | **Düzeltildi.** Göreli yola çevrildi; v1 satır numaraları öyle işaretlendi. Gözlem formlarına ve üretim planına verilen beş satır kontrol edildi, hâlâ doğru satırı gösteriyor. Repodaki 1.342 yerel bağlantının tamamı artık çözülüyor |

Denetleyici (`--siki` dahil) ve 21/21 test bu düzeltmelerden sonra yeniden koşuldu: temiz.
Özgün kareler değiştirilmedi, yeni koşum yapılmadı, commit atılmadı.

### Devir doğrulama sonucu

- 21/21 denetleyici testi; dokuz gözlem formunda 0 hata / 0 uyarı (ilk koşudaki 12 hata düzeltildi).
- Motor duman testi geçti. Dizin yeniden üretildi: 422 kimlik / 387 kare, tüm kareler açıklamalı.
- Kurgu denemesi `-X utf8` ile çıkış 0: 10 sayfa, 30 kanıt hash doğrulaması ve mevcut otomatik kontroller temiz. PDF sayfaları bu turda yeniden gözle incelenmedi.
- Nihai Belge 2 bölümleri ve etki-boyutu kapısı üretilmedi; yeni emülatör koşumu yapılmadı.
- Bluecoins dosya adları korundu; özgün görseller yeniden adlandırılmadı veya değiştirilmedi.

## 20 Eylül 2026 — Belge 2 yazıldı (A2–A6 kapatıldı, on iki bölüm)

Kullanıcı kalan işi devretti ("yapılacakları sana bırakıyorum"). Plandaki beş açık karar
kapatıldı ve Belge 2'nin on iki bölümü yazıldı. **Yeni canlı koşum yapılmadı; yeni kanıt
üretilmedi.** Belge tamamen mevcut 387 karenin ve dokuz gözlem formunun üstüne kuruldu.

### Kararlar (plan §8)

| # | Karar | Seçim | Yazımda ürettiği kural |
|---|---|---|---|
| A2 | Yazım sırası | Ağırlar önce: 5 → 6 → 7 → 3 → 4 → 9 → 8 → 10 → 11 → 12 → 2 → 1 | Bölüm bitmeden sonrakine geçilmez |
| A3 | Karşılaştırılabilirlik | Fark karşılaştırılır, mutlak tutar değil | **Her matris `fark` alanı taşır**; yoksa basılmaz |
| A4 | Koşum birleştirme | Hücre kendi koşum tarihini taşır | Zincir farklı koşumlardan kuruluyorsa `kirik` işaretlenir |
| A5 | Kanıtsız blok | Yazılır, "görülmedi" kalır | Boş "görülmedi" hücresi yasak; taranan yüzey yazılır |
| A6 | Ürünün bütünsel görünümü | Bölüm 12'de sabit beş satır | Altı ürün aynı beş başlıkla özetlenir |

### Üretim altyapısı

`raporlar/belge2/` kuruldu. Belge 1'in motoru ve kalıbı **değiştirilmeden** kullanılıyor;
`ortak/b2.py` dört yeni sayfa türü (matris, zincir, dönem, şema) ekliyor ve iki şey yapıyor:

1. **Plan kurallarını kapıya çeviriyor.** A3/A4/A5 ve sabit satır sözlüğü ihlal edilirse üretim
   durur. Bu, karartma dersinin uygulamasıdır: yalnız düz metinde yazılı kural uygulanmıyor
   demektir.
2. **Kalıbın tablo çizicisini geçersiz kılıyor.** Belge 1'de yalnız rozetli hücreler iddia
   tablosuna girerdi; Belge 2'de argüman matrisin kendisi olduğu için **dolu her hücre** kendi
   dayanağıyla denetime giriyor. Bölüm 5 tek başına 206 satırlık bir iddia tablosu üretti.

Ayrıca satır içi kalın metin sorunu çözüldü: motor `insert_textbox` ile düz metin basıyor,
`**` işaretleri çizimde temizleniyor ve Markdown kopyasında korunuyor; vurgu hücre fontu ve
etiketli satırlarla veriliyor.

### Sonuç

On iki bölüm, birleşik belge **83 sayfa**, beş basılı kare, bölüm başına iddia tablosu ve eksik
listesi. Otomatik kapılar bütün bölümlerde ve birleşik belgede temiz; hash denetimi tamam.

**Gözle incelemede bulunan ve düzeltilen yedi şey:** ham `**` işaretleri · rozetli boş `—`
hücreleri · "kavram yok" ifadesinin üstünde duran Görülmedi rozeti (ikisi ayrı şeydir) ·
ilgisiz kutunun içinden geçen kesik ok (iki bölümde) · sayfayı taşıran modül tablosu ·
Bölüm 12'deki numara boşluğu · rozet sütununda etiketin iki kez yazılması.

### Bu turun sınırları

- **Yeni koşum yok.** Belgedeki bütün eksikler koşum listesindeki eksiklerle aynı; bölüm başına
  `eksik-listesi.md` dosyaları bunları ürün, sayfa ve öncelikle birlikte sayıyor.
- **83 sayfanın tamamı gözle incelenmedi.** Her bölümden örnek sayfalar ve birleşik belgenin
  kapak/içindekiler/kanıt eki sayfaları incelendi; tam sayfa incelemesi kullanıcıya kalıyor.
- Özgün kareler değiştirilmedi, yeni kanıt kimliği verilmedi, commit atılmadı.
- Belge 3 başlatılmadı; Belge 1'in patron onayı hâlâ bekliyor.
