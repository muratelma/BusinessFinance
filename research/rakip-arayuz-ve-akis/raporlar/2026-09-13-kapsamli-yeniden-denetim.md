# Research yeniden denetim raporu — 13 Eylül 2026

Soru 1 ve Soru 2'deki bulgular yeniden sınandı. **Araştırma mevcut hâliyle güvenilir bir nihai karar girdisi olarak kapatılamaz.** Önceki temel hatalar duruyor; bu tur ayrıca yanlış gelir giriş yolu, eksik ürün özellikleri, mevcut BusinessFinance davranışının yanlış anlatılması ve gözlemden kanıtsız sistem davranışı çıkarılması konusunda yeni dayanaklar buldu.

Bu bir **araştırma denetimidir**. Rakiplerde yeni canlı koşum yapılmadı; eski test notları, yerel kanıtlar, mevcut BusinessFinance kodu ve seçilmiş resmî web kaynakları karşılaştırıldı. Uygulama kodu, mevcut araştırma belgeleri ve görseller değiştirilmedi; veri silinmedi, commit atılmadı. Bu rapor ve salt okunur yardımcı betik bu oturumun çıktısıdır.

## Kapsam ve yöntem

- Başlangıç envanteri: **387 dosya; 21 Markdown, 357 PNG, 9 diğer dosya**. Soru 1 ve Soru 2, önceki denetimin 19 Markdown sayısına eklenmiş iki tarihsel girdidir; içlerindeki eski örnekler güncel ihlal sayılmadı.
- Dokuz gözlem formunda görevler, kontrol değerleri, çıkarımlar, kararlar ve kapanış hükümleri incelendi. Yönlendirme belgelerinde durum, kanıt ve veri koruma kuralları karşılaştırıldı.
- 357 PNG'nin tamamında imza, parça uzunlukları, CRC, IEND ve sıkıştırılmış görüntü akışının açılması kontrol edildi; SHA-256 ile birebir kopya arandı. Bu, her görüntünün içerik doğruluğu veya tam görsel decoder testi değildir.
- **11 seçilmiş görsel** doğrudan açılıp incelendi: Bluecoins filtre/menu/export üçlüsü; Goodbudget işlem listesi; Hesap Defterim ayarlar alt bölümü; KolayBi eski/yeni cari ve eski kasa/yeni banka çiftleri ile proje listesi; QuickBooks plan ekranı.
- BusinessFinance için domain, rapor sorgusu ve Flutter kaynakları karşılaştırıldı. Mevcut test dosyaları bazı alanların kapsandığını gösteriyor; backend/Flutter testleri bu oturumda çalıştırılmadı.
- Goodbudget gelir yolu ve banka senkronizasyonu resmî yardım sayfalarıyla doğrulandı. Bütün rakiplerin bütün dış kaynakları yeniden doğrulanmadı.
- Yeniden çalıştırılabilir kontrol: [denetim-2026-09-13.cjs](denetim-2026-09-13.cjs). Node'un yerleşik modüllerini kullanır; paket kurmaz, dosya yazmaz.

## Otomatik kontrollerin sonucu

| Kontrol | Sonuç |
|---|---|
| PNG yapısı, CRC ve sıkıştırılmış veri | 357/357 geçti |
| SHA-256 ile birebir dosya kopyası | 0 |
| Dokuz formdaki taranabilen açık PNG tokenlarında bulunmayan dosya | 0 |
| Tam dosya adı kendi formunda bulunmayan görsel | **181/357** |
| Markdown tablo kolon uyumsuzluğu | **3 tablo, 24 veri satırı** |
| Yeni yardımcı betiğin 5 temel öz kontrolü | Geçti |
| Yeni yardımcı betiğin araştırma sonucu | **Çıkış 1**; atıf/tablo sorunları nedeniyle |
| Mevcut denetim.sh, dokuz uygulama | **17 sorun grubu**, buna rağmen **çıkış 0** |
| Mevcut denetim.sh, olmayan form adı | “Denetim temiz”, **çıkış 0** |

“17 sorun grubu”, 17 yanlış iddia veya 17 bozuk dosya demek değildir. Betik aralıkları ve kısa atıfları grup hâlinde sayar; açıklama amaçlı örnekleri de yakalar.

| Uygulama | PNG | Kendi formunda tam adı bulunmayan | Eski betikte sorun grubu |
|---|---:|---:|---:|
| Money Manager | 30 | 4 | 2 |
| Wallet | 106 | 97 | 6 |
| Bluecoins | 90 | 80 | 5 |
| Hesap Defterim | 45 | 0 | 1 |
| Goodbudget | 28 | 0 | 1 |
| Paraşüt | 9 | 0 | 0 |
| Logo İşbaşı | 6 | 0 | 1 |
| KolayBi | 39 | 0 | 1 |
| QuickBooks | 4 | 0 | 0 |
| **Toplam** | **357** | **181** | **17** |

Tam adın bir yerde geçmesi, o görsele yapılan bütün atıfların düzgün olduğunu veya görselin iddiayı desteklediğini kanıtlamaz. KolayBi bunun somut örneğidir.

## Önceki raporlardaki bulguların durumu

| Önceki bulgu | Bu turdaki sonuç |
|---|---|
| denetim.sh sorun bulurken başarılı çıkıyor | Yeniden üretildi; olmayan formda da başarılı |
| Kısa atıf ve yol/aralık kör noktaları | Kodda sürüyor; tam ad taramasında yine 181 eksik |
| KolayBi eski/yeni sınıflandırması hatalı | Sürüyor; 05 ve 07 için görsel karşılaştırma tekrar yapıldı |
| Bluecoins bizim üründe fatura–tahsilat bağı var sayıyor | Kod ve ADR ile tekrar çürütüldü |
| KolayBi kart/asgari/limit/sıfır iddiaları | Sürüyor; kullanılabilir limitin **istemcide de gösterildiği** bu tur kesinleşti |
| Goodbudget bilinmeyen −600 nedenini kesinleştiriyor | Sürüyor |
| Goodbudget alternatif gelir yolu denenmeden genellenmiş | Sürüyor; resmî Android talimatı bu tur bulundu |
| KolayBi demo adlarından kullanıcı ihtiyacı çıkarıyor | Sürüyor; proje karesi tekrar incelendi |
| Bluecoins aritmetik, silme ve taksit genellemeleri | Sürüyor |
| Wallet birbirine zıt model hükümleri | Sürüyor |
| Özet karşılaştırma raporu eski hataları taşıyor | Sürüyor |
| Yanlış adlandırılmış/benzer süreç kareleri | Bluecoins filtre/menu/export üçlüsü tekrar doğrulandı; önceki diğer çiftler bu tur yeniden açılmadı |
| Üç tabloda kanıt sütunu uyumsuz | Yeniden üretildi; toplam 24 satır |

## Kararı etkileyen bulgular

### B01 — Yüksek: BusinessFinance'te fatura bazlı tahsilat eşleştirmesi varmış gibi karşılaştırılıyor

[Bluecoins formu](../gozlemler/bluecoins.md), satır 210, 259, 335–337: fatura bağı ve “takip garantisi” bizim modelin üstünlüğü olarak kullanılıyor. Wallet ve KolayBi kıyasları da bu kabulden etkileniyor.

[CounterpartyPayment.cs](../../../src/BusinessFinance.Domain/CounterpartyPayment.cs) karşı taraf ve hesap kimliklerini taşır; belirli bir borçlandırma/fatura kimliği taşımaz. [architecture.md](../../../documentation/architecture.md), satır 569, tahsilat ve ödemelerin tek borçlandırmaya bağlanmadığını açıkça söyler. [ADR 0014](../../../documentation/adr/0014-economic-event-recognizes-payment-carries.md) tanıma/taşıma ayrımını kurar; fatura bazında kapatma garantisi kurmaz.

**Etkisi:** rakipte eksik bulunan bir özellik bizde uygulanmış sayılıyor. Aynı müşteride iki fatura ve bir tahsilat olduğunda hangi faturanın kapandığı ile müşterinin net bakiyesi farklı sorulardır.

**Kapanış ölçütü:** üç boyut ayrı yazılmalı: gelir/giderin tanınma zamanı, hesap bakiyesine etki, tahsilatın belirli borca bağlanması. Mevcut ürün için yalnız gerçekten bulunan yetenekler işaretlenmeli.

### B02 — Yüksek: KolayBi karşılaştırmasında mevcut kart özellikleri yok sayılıyor

[KolayBi formu](../gozlemler/kolaybi.md), satır 314–315 ve 674:

| İddia | Kodda doğrulanan |
|---|---|
| Asgari ödeme oranı yok | CreditCard.MinimumPaymentRate var; varsayılan %20, hesaplama metodu da var |
| Kart limiti zorunlu değil | CreditCard kurucusu zorunlu Money limit alıyor ve doğruluyor |
| Kullanılabilir limit gösterimi yok | Domain hesaplıyor; Flutter kart listesi ve detayında gösteriliyor |
| Bedelsiz kaydın karşılığı bizde sıfır tutarlı işlem olur | Money, sıfır ve negatif işlem tutarını reddediyor |

Dayanak: [CreditCard.cs](../../../src/BusinessFinance.Domain/CreditCard.cs), satır 31, 55, 132, 147; [finance_page.dart](../../../mobile/business_finance_mobile/lib/features/cards/presentation/finance_page.dart), satır 253, 689 ve 1526; [Money.cs](../../../src/BusinessFinance.Domain/Money.cs), satır 10.

Sıfırın reddi normal Money taşıyan işlemler içindir; sıfır kasa sayımı veya sıfır açılış gibi farklı kavramlara genellenmemeli.

**Kapanış ölçütü:** bu alanlar yeni özellik adayından çıkarılıp mevcut davranış kıyası olarak yeniden yazılmalı.

### B03 — Yüksek, yeni: İsteğe bağlı plan bitişi bizde yok deniyor; hem domain hem formda var

[KolayBi formu](../gozlemler/kolaybi.md), satır 372 ve 669, bizim planın süresiz olduğunu ve isteğe bağlı bitişin iki üründe de bulunmadığını söylüyor.

[RecurringTransaction.cs](../../../src/BusinessFinance.Domain/RecurringTransaction.cs), satır 25 ve 339, nullable EndDate ve onu aşmama kontrolü taşıyor. [planning_page.dart](../../../mobile/business_finance_mobile/lib/features/planning/presentation/planning_page.dart), satır 1062, doğrudan **“Bitiş tarihi (isteğe bağlı)”** alanını gösteriyor; tarih ile tekrar sınırının birlikte verilebildiği de açıklanmış.

**Kapanış ölçütü:** bizde olmayan bir özellik önerisi olarak sunulması bırakılmalı; KolayBi'nin zorunlu tekrar sayısı ile bizim mevcut isteğe bağlı bitiş/sınır davranışımız karşılaştırılmalı.

### B04 — Yüksek, yeni: “Biz yalnız tahsil edilmiş geliri görürüz” anlamına gelen nakit karşılaştırması yanlış

[KolayBi formu](../gozlemler/kolaybi.md), proje detay tablosu ve karar tablosu, “bizde işletme neti yalnız nakit esaslı” ifadesinden tahakkuk–nakit ayrımının bizde bulunmadığına yöneliyor.

Ancak [EfFinancialReportRepository.cs](../../../src/BusinessFinance.Infrastructure/Reports/EfFinancialReportRepository.cs), satır 96–117, cari borçlandırmaları **ChargeDate** üzerinden gelir/gidere katıyor. Tahsilatı beklemiyor. ADR 0014 de saf nakit esasını açıkça reddediyor.

**Etkisi:** “tahsil edilmemiş satışı tanımak” ile “aynı ekranda tanınan ve tahsil edilen tutarı iki kartta göstermek” karıştırılıyor. İkincisi gerçek bir arayüz/rapor adayı olabilir; birincisi zaten mevcut.

Kurucu belgelerdeki “nakit esaslı işletme neti” terminolojisi bu yanlış okumayı kolaylaştırıyor. Bu denetim ürün kararını değiştirmiyor; mevcut çalışan davranışın karşılaştırmada doğru anlatılmasını istiyor.

### B05 — Yüksek, yeni dış doğrulama: Goodbudget gelir testi yanlış yolun sonucunu genelliyor

[Goodbudget formu](../gozlemler/goodbudget.md), K03, karar tablosu ve sonuç: gelir Expense/Credit formunda **Credit** olarak Market zarfına girilmiş; ardından gelir girişinin zorunlu olarak harcama zarfına bağlandığı ve raporu bozduğu sonucuna geçilmiş.

Formun kendi satır 218'i ayrı **From New Income / From Available** yolunu kaydediyor. Resmî [Step 3. Add Your Income](https://goodbudget.com/help/getting-started-guide/step-3-add-income/) sayfası Android'de Fill Envelopes üzerinden gelir eklenebildiğini ve **Keep Unallocated** ile gelirin zarfa dağıtılmadan tutulabildiğini açıkça anlatıyor.

**Kesin sonuç:** “gelir her zaman harcama zarfına bağlanmak zorunda” genellemesi desteklenmiyor. Ekrandaki negatif harcama ölçümü korunabilir; bunun normal gelir yolundaki ürün hatası olduğu kanıtlanmış değil.

**Gerekli yeniden test:** aynı sentetik gelir, resmî gelir yolu ve Credit yolu üzerinden ayrı kontrollü örneklerde girilmeli; hesap, zarf, gelir ve gider raporu etkileri ayrı karşılaştırılmalı. Bu tur canlı koşulmadı; eski raporun bozuk olduğunu göstermek, rakibin bütün hesaplamalarının doğru olduğunu göstermek değildir.

### B06 — Yüksek: Goodbudget −600 bakiye farkının nedeni hâlâ bilinmiyor

Aynı formda satır 79 **nedeni doğrulanamadı** derken satır 123, pipeline, karar satırı 253 ve sonuç bölümü bu farkı tekrarlayan planın sonraki örneği olarak anlatıyor.

[23-transactions-initial-envelope-fill.png](../kanitlar/goodbudget/23-transactions-initial-envelope-fill.png) tekrar açıldı: tek Bulut kaydı var. Bu, listedeki durumu destekler; farkın hangi mekanizma yüzünden oluştuğunu göstermez.

**Kapanış ölçütü:** −600 gözlemi korunmalı, nedensellik hipotez olarak yazılmalı. Aynı planın vade öncesi/sonrası, senkronizasyon ve hesap hareketleri birlikte izlenmeden “liste dışı tekrar üretimi” kesin hükmü verilmemeli. −16 silme sapması da gözlenen dış etki olarak ayrı tutulmalı.

### B07 — Orta/yüksek, yeni: Goodbudget özellik yokluğu ücretsiz koşumdan bütün ürüne taşınmış

Form satır 23 ve entegrasyon satırı banka senkronizasyonunu ürün genelinde yok sayıyor. Resmî [How does Automatic Bank Sync work?](https://goodbudget.com/help/automatic-bank-sync/how-does-automatic-bank-sync-work/) sayfası Premium'da otomatik banka hareketi aktarımını anlatıyor. Sayfanın kendi güncelleme tarihi 22 Eylül 2023; yalnız bu tur eklenmiş yeni bir özellik diye açıklanamaz.

Ayrıca satır 25 “hesap türü yok” diyor; K02 aynı belgede Checking/Savings/Cash, Credit Card ve Debt gruplarını listeliyor.

**Kapanış ölçütü:** “test edilen ücretsiz planda erişilemedi”, “üründe bulunamadı” ve “ürün genelinde yok” ayrılmalı. Banka özelliğini incelemek bağlantı kurmayı gerektirmez; bu tur hiçbir banka bağlantısı yapılmadı.

### B08 — Yüksek: KolayBi demo proje adları kullanıcı araştırması sayılmış

[KolayBi formu](../gozlemler/kolaybi.md), satır 234 sonrası; [DURUM.md](../DURUM.md), üstteki KolayBi özeti.

[d01-destek-proje-listesi.png](../kanitlar/kolaybi/d01-destek-proje-listesi.png) tekrar açıldı: Ev Elektrik, Bebek Bakım gibi adlar gerçekten görünüyor. Ancak kayıtların kimin ihtiyacını temsil ettiği, gerçek müşteri kullanımı mı eğitim örneği mi olduğu ve hangi niyetle oluşturulduğu bu kareden çıkmıyor.

**Etkisi:** ürün tezini desteklemek için gerçek kullanıcı verisi yerine demo metni kullanılıyor.

**Kapanış ölçütü:** “demo listesinde bu adlar var” gözlemi ile “serbest proje ekseni hane gideri için kullanılabilir” hipotezi ayrılmalı. “Kullanıcılar böyle yapıyor” sonucuna kullanıcı araştırması gerekir.

### B09 — Yüksek: Wallet'ın D2/D3 sonucu tanıma/taşıma eşdeğerliğini kanıtlamıyor

[Wallet formu](../gozlemler/wallet-budgetbakers.md), satır 206 ayrım yok derken satır 274 ve sonuç ayrımı doğal olarak destekleyen tek rakip diyor.

D2'nin hesap bakiyesini değiştirmemesi ve D3'ün aynı Debt'e bağlanması önemli gözlemler. Fakat D2'nin gelir raporunda ne zaman tanındığı ve D3'ün yeniden gelir sayılıp sayılmadığı bu koşumda karşılaştırılmamış. D3 aramada gelir kategorili bir Record olarak görünüyor.

**Kapanış ölçütü:** D2 öncesi/sonrası ve D3 sonrası için hesap bakiyesi, açık borç, gelir/gider raporu ayrı ölçülmeli. “Debt–Record bağı var” doğrulanmış sonuç olarak kalabilir; “ADR 0014 ile aynı finansal model” hükmü açık kalmalı.

### B10 — Orta/yüksek: Bluecoins aritmetik ve davranış genellemeleri kapanmamış

[Bluecoins formu](../gozlemler/bluecoins.md):

- Satır 154: ₺600 gider için ₺44.350 → ₺43.350 yazılmış; fark **₺1.000**. Önceki ₺44.950 netten ilk taksit ₺1.000 düşülmüşse tekrarlayan öncesi beklenen ₺43.950'dir; bu bir aritmetik çıkarımdır, başlangıç karesiyle doğrulanmalıdır.
- Satır 77 kalıcı/geri alınamaz silme diyor; satır 296 çöp kutusunu ve geri yükleme davranışının bilinmediğini anlatıyor.
- B2 ilk taksiti anında yazıyor; sonuçta tekrarlayan ve taksit birlikte “tanım üretmez” diye özetleniyor.
- Otomatik yazma kutusu kapalıyken yapılan tekrarlayan testi, bütün modların değişmez davranışı gibi anlatılmamalı.

**Kapanış ölçütü:** B1 onaylı, B1 otomatik ve B2 ilk/sonraki taksitler ayrı satırlarda; silme ve geri yükleme ayrı deneylerde ele alınmalı.

### B11 — Orta/yüksek, yeni: Hesap Defterim'de açık ayar, fiilen gerçekleşmiş e-posta gönderimi sayılmış

[Hesap Defterim formu](../gozlemler/hesap-defterim.md), satır 308, kullanıcı hiçbir şey yapmadan döküm aldığı ve finansal verinin istemeden dışarı çıktığı sonucuna varıyor.

[43-ayarlar-alt-bolum-donem-baslangici.png](../kanitlar/hesap-defterim/43-ayarlar-alt-bolum-donem-baslangici.png) yalnız “İşlem dökümünü e-posta ile otomatik gönder” kutusunun **bu oturumda işaretli** olduğunu gösteriyor. Alıcı, tetikleyici, ön kurulum, izin veya teslim edilmiş e-posta kanıtı yok. Yeni kurulum karşılaştırması olmadan “varsayılan açık” da daha güçlü bir iddiadır.

**Kapanış ölçütü:** görünen ayar gözlem; kurulum varsayılanı ve gönderim davranışı doğrulanamadı olarak ayrılmalı. Bu denetimde kimseye e-posta gönderilmedi.

### B12 — Orta, yeni: Ekran davranışından veritabanı yapısı kesinleştiriliyor

Wallet satır 293, aramada çıkmayan D2 için “işlem tablosuna hiç yazılmadığını doğruluyor” diyor. Hesap Defterim pipeline'ı “ikinci bir tablo/rapor kaydı yok” ve transferin nasıl saklandığını kesin anlatıyor.

Arama/listede görünmeme fiziksel tablo yokluğunu kanıtlamaz. Aynı dış davranış farklı şemalarla üretilebilir.

**Kapanış ölçütü:** “işlem listesinde görünmedi”, “bir bacağın silinmesi diğerini etkilemedi” gibi ölçülebilir dış davranış yazılmalı. Fiziksel şema iddiası için kod/veritabanı erişimi veya açık teknik doküman gerekir.

### B13 — Orta, yeni: QuickBooks ürün ayrımı uyarısı bütün tabloya taşınmamış

[QuickBooks formu](../gozlemler/quickbooks.md) doğru biçimde dört karenin Solopreneur değil genel QuickBooks onboarding'i ve Simple Start olduğunu belirtiyor. Ancak “Ürün kimliği” altındaki iş modeli hücresi hâlâ Simple Start fiyatını Solopreneur bölümüne koyuyor; sonraki akış özeti ürün adını tekrar ayırmadan onboarding kapısını anlatıyor.

[04-choose-plan-paywall.png](../kanitlar/quickbooks/04-choose-plan-paywall.png) tekrar açıldı: başlık Simple Start; üstte bir aylık ücretsiz deneme ifadesi de var. Bu ifade denemenin erişilebilir olduğunu kanıtlamaz; “ekranda ücretsiz deneme vaadi yok” şeklinde de okunmamalı.

**Kapanış ölçütü:** her fiyat/onboarding satırında ürün açıkça QBO Simple Start olarak yazılmalı. Solopreneur'ün ücret tutarı ve kendi onboarding'i bu dört kareden çıkarılmamalı.

### B14 — Orta, yeni: Fiziksel kasa sayımı ihtiyacı BusinessFinance'te yokmuş gibi düşük öncelik verilmiş

[Hesap Defterim formu](../gozlemler/hesap-defterim.md), satır 310, kupür hesap makinesini “dijital-öncelikli modelimizde karşılığı yok” diye küçültüyor.

Bizde [CashCount.cs](../../../src/BusinessFinance.Domain/CashCount.cs) ve [cash_count_view.dart](../../../mobile/business_finance_mobile/lib/features/cash/presentation/cash_count_view.dart) ile gerçek gün sonu kasa sayımı ve fark akışı mevcut.

**Doğru fark:** kupür × adet yardımcı aracı ile sayılan tutarı kaydedip beklenen bakiye ile karşılaştırma aynı özellik değildir. Kupür aracı bizde var denemez; fakat hitap ettiği fiziksel kasa ihtiyacının bizde karşılığı olmadığını söylemek de yanlıştır.

## Kanıt dosyaları, betik ve özet belgeler

### B15 — Orta: Eski/yeni görsel sınıflandırmasının gerekçeleri hâlâ yanlış

KolayBi satır 609, 01–08'i 2020 video kareleri diye topluyor. Oysa 01 mobil giriş, 02–07 eski video, 08 ise daha sonra yüklenmiş ayrı videodan kare.

- **05 vs d07:** ikisi de cari listesi; d07 daha geniş güncel alan seti taşıyor. 05 yalnız tarihsel karşılaştırma kullanılacaksa değerli. 2020'de bir sekmenin görülmesi “ürünün başından beri vardı” sonucunu vermez.
- **07 vs d19:** biri **Kasalar**, diğeri **Banka Hesapları**. Aynı ekranın iki sürümü değil. 07'nin katkısı eski bir analist hatasını kanıtlamak değil, kasa sekmesi/alanları ve dönemin menüsü.
- 08 hem yeni karşılaştırma tarafında hem “örtüşmeyen eski dörtlü” içinde kullanılmış.
- Destek karesindeki 2023 işlem tarihi veya videonun 2026 yükleme tarihi, tek başına arayüzün üretim tarihini kanıtlamaz.
- Üç aritmetik eşleşmeden “39 karenin tamamı tutarlı ve aynı kiracıdan” sonucuna gidilemez; yalnız kontrol edilen sayılar için tutarlılık söylenebilir.

Hiçbir görsel silinmedi. 05 bir **seçim adayıdır**, otomatik silme kararı değildir.

### B16 — Orta: Dosya adı ve içerik uyuşmuyor; byte kopyası olmaması içerik benzersizliği değildir

Yeniden açılan üç Bluecoins karesi:
[f7-31-filtre.png](../kanitlar/bluecoins/f7-31-filtre.png),
[f7-32-menu.png](../kanitlar/bluecoins/f7-32-menu.png),
[f7-53-export.png](../kanitlar/bluecoins/f7-53-export.png).

Üçü de filtre panelini gösteriyor. Saat farkları var; bu yüzden SHA-256 farklı olsa da ürün açısından aynı ekran içeriğini taşıyorlar. “export” adlı kare PDF/CSV/HTML seçeneklerini göstermiyor.

Bunlar başarısız dokunma/süreç kaydı olarak anlamlı olabilir. Ürün özelliğinin kanıtı olarak kullanımları ayrı değerlendirilmelidir.

### B17 — Yüksek: denetim.sh güvenilir kapanış kapısı değil

Mevcut betik değiştirilmeden çalıştırıldı. İki canlı sonuç:

1. Dokuz formda 17 sorun grubu → **exit 0**.
2. Olmayan “audit-nonexistent” formu → atlandı, **“Denetim temiz”**, **exit 0**.

Kod okumasıyla doğrulanan sınırlar:

- Tam dosya gövdesi metnin herhangi bir yerinde varsa PNG uzantısı aranmadan geçiliyor.
- f7-53-export dosyasının kısa kodu f7 olarak çıkarılıyor; kare bazında kimlik kayboluyor.
- Ölü atıf regex'i yalnız belirli backtick içindeki basename biçimini yakalıyor; yolları kapsamıyor.
- Bir görsele bir kez doğru atıf yapılması, diğer kısa/yanlış kullanımları temizlemiyor.
- Kural açıklamalarındaki yasak örnekler gerçek kullanım gibi raporlanıyor.

Yeni yardımcı betik bu inceleme için bağımsız ölçüm verir; **mevcut betiğin tam yerine geçmez**. Doğal dildeki kısa kod/aralık atıflarını semantik olarak çözmez, görsel içeriği veya dış kaynak doğruluğunu kontrol etmez.

### B18 — Orta: Üç tablonun kanıt hücreleri başlıksız

Bluecoins satır 56–65, Money Manager ve Wallet satır 52–61: başlık/ayraç 3 sütun; sekizer veri satırı 4 sütun.

Markdown tablo yorumlayıcısına göre fazla hücreler görünmeyebilir. Her üçüne de doğru kanıt başlığı/ayracı gerekir. Bu tur kaynak belgeler değiştirilmedi.

### B19 — Yüksek: Özet ve durum belgeleri aynı hataları yeniden yayıyor

[turk-on-muhasebe-vs-businessfinance.md](turk-on-muhasebe-vs-businessfinance.md):

- KolayBi çek/senet hücresi “—”; yeni formda iki ekran mevcut.
- Gözlem formlarından doğrulanmadığı için çıkarılan fiyatlar duruyor.
- Logo karesinden üç ürünün muhasebecisine düzenleme/silme yetkisi genelleniyor.
- Bütün içerik topluca “Resmî kaynak” sayılıyor; çıkarımlar ve bizim kodumuz ayrı etiketlenmiyor.
- Eski tarih yazılmış olması, belgenin güncel kullanıma kapatıldığını göstermiyor.

[DURUM.md](../DURUM.md):

- Üstte Faz 7.5 bitmeden Faz 8 başlamaz denirken satır 87 civarında “Sıradaki Faz 8” yazıyor.
- KolayBi ve Goodbudget “TAMAM” görünmesine rağmen yukarıdaki içerik hataları duruyor.
- Satır 595'te **“Test kayıtları koşum sonrası silinir”** kuralı kalmış. [SENTETIK-TEST-VERISI.md](../SENTETIK-TEST-VERISI.md) bunun tersini, kullanıcı istemeden ek koşum verisinin silinmemesini söylüyor.
- [TUR2-YOL-HARITASI.md](../TUR2-YOL-HARITASI.md) Wallet için aynı model eşdeğerliği genellemelerini tekrarlıyor; doğrulama kapısı canlı panoyla tek anlam taşımıyor.
- [kanitlar/README.md](../kanitlar/README.md) QuickBooks klasörünü Solopreneur/resmî kaynak diye etiketliyor; dosyalar QBO onboarding manuel kareleri. Formdaki doğru ayrım envantere taşınmamış.

**Kapanış ölçütü:** yalnız formları düzeltmek yetmez; her değişen iddianın özet, karar tablosu ve durumdaki bütün kopyaları birlikte uzlaştırılmalı. Tarihsel notlar açıkça tarihsel ayrılmalı.

## Dokuz uygulama için kapanış değerlendirmesi

| Uygulama | Bu denetimin hükmü |
|---|---|
| Money Manager | Tam ad borcu ve tablo kusuru açık; “temiz” kapanışı desteklenmiyor |
| Wallet | Model eşdeğerliği kararı açık; yüksek atıf borcu var |
| Bluecoins | Bizim modeli yanlış anlatma, aritmetik, silme ve taksit genellemeleri açık |
| Hesap Defterim | Sayısal/görsel çalışması değerli; e-posta, fiziksel şema ve kasa kıyası aşırı kesin |
| Goodbudget | Gelir testi yorumu ve −600 neden-sonuç hükmü yeniden ele alınmadan karar üretilemez |
| Paraşüt | Pazarlama karelerinin sınırı iyi belirtilmiş; “aynı fatura iki durumda” anlatısı aynı kayıt kimliğiyle kanıtlanmış değil; otomatik mahsup bizim kayıt bağıyla eşitlenmemeli |
| Logo İşbaşı | Kanıt tavanı yazılmış; OCR → otomatik gider ve insan onayı gibi mekanizmalar kesin adım kaynağına bağlanmalı; eski özetle uzlaşmıyor |
| KolayBi | Mevcut BusinessFinance özelliklerini yok sayan kıyaslar ve demo→ihtiyaç çıkarımı nedeniyle kapanamaz |
| QuickBooks | Simple Start/Solopreneur ayrımı önemli bir iyileşme; bütün satırlara ve envantere yayılmamış |

“Bu tur yeni kusur kanıtlanmadı” ile “uygulama formu bütünüyle doğrulandı” aynı sonuç değildir. Bu rapor dokuz uygulamayı yeniden canlı test etmiş gibi okunmamalı.

## Önerilen tek sonraki görev

**Karara taşınan iddiaları dayanaklarıyla uzlaştıran bir düzeltme turu.** Önce B01–B09: BusinessFinance gerçek yetenekleri, Goodbudget gelir/nedensellik ve Wallet tanıma/taşıma kıyası. Doğrulanamayan davranışlar açıkça beklemeye alınmalı. Ardından ilgili özetler, tam atıflar ve tablo sorunları aynı turda güncellenmeli.

Araştırmayı kapatma ölçütü “betik yeşil” olamaz: her önemli sonuç için iddia → gözlem/kaynak → sınır → karar zinciri okunabilir olmalı. Bu denetimde yalnız rapor ve yardımcı betik üretildi; düzeltme, yeni canlı rakip testleri ve nihai Belge 1/2/3 yazımı yapılmadı.

## Çalıştırma

Repo kökünde:

```powershell
node research/rakip-arayuz-ve-akis/raporlar/denetim-2026-09-13.cjs
& 'C:/Program Files/Git/bin/bash.exe' research/rakip-arayuz-ve-akis/denetim.sh
$LASTEXITCODE
```

Yeni dosyalar eklendiği için sonraki envanter toplamı artar; 357 PNG, 181 tam ad eksiği ve 24 tablo satırı bu koşumun içerik ölçüleridir. Kullanıcıya ait mevcut değişiklikler ve staged yeniden adlandırmalar korunmuştur.

