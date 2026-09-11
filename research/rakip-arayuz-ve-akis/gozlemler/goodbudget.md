# Uygulama Gözlem Formu — Goodbudget

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Goodbudget (Dayspring Technologies) |
| Sürüm | 2.24.26013 (180) |
| Test tarihi | 11 Eylül 2026 |
| Cihaz / işletim sistemi | Pixel 8 AVD (Android 17), sistem dili Türkçe |
| Dil / para birimi | Uygulama arayüzü tamamen İngilizce (para birimi/yerelleştirme ayarı yok); sistem bileşenleri (tarih seçici, klavye, toolbar) Türkçe |
| Hesap veya plan türü | Ücretsiz (household), gerçek kullanıcı e-postasıyla kayıt |
| Erişim kısıtı | Ücretsiz sürüm: 10 zarf, **1 hesap** (tüm türler dahil toplam) sınırı |
| İnceleme türü | Manuel gözlem |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | "Zarf bütçeleme" (envelope budgeting): harcamadan önce parayı kategorilere (zarflara) dağıt, yalnız zarfta parası olan kategoriden harca |
| Asıl hedef kullanıcı | Kişisel/aile bütçesi tutan, nakit akışını "önceden planlanmış" tutmak isteyen bireysel kullanıcı — dijital bir zarf sistemi klasiği (Dave Ramsey tipi bütçeleme kültürü) |
| Çözdüğü ana iş | Aylık geliri kategorilere önceden dağıtıp aşırı harcamayı zarf bazında sınırlamak |
| Açıkça kapsam dışı bıraktığı | İşletme muhasebesi, kart borcu/ekstre modeli, taksit, fatura/tahsilat, banka bağlama (senkron/otomatik içe aktarma yok) |
| İş modeli | Freemium: 10 zarf + 1 hesap ücretsiz; sınırsız zarf/hesap için abonelik ("Subscribe") |
| BusinessFinance ile aynı kulvarda mı | Hayır — işletme/şahsi ayrımı, kategori, hesap türü hiç yok; felsefe de ters yönde ("harcamadan önce dağıt" vs bizim "harca, sonra raporla") |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | ~12 | Household kaydı (e-posta/şifre) hızlı; ardından "Setup Budget" sihirbazı örnek zarflarla (Groceries/Gas/Savings) geliyor, hepsi silinip/yeniden adlandırılabiliyor | Kayıt formu placeholder'ı gerçek görünümlü (`johnj@email.com`); "Fill Envelopes" adımı (gelirin zarflara fiilen aktarılması) ayrı bir ekran — bütçe planlamak ile parayı fiilen dağıtmak iki ayrı adım | `01-ilk-acilis.png`, `02-setup-budget-envelope.png`, `03-register-household.png` |
| K01 Ana ekran | Tamamlandı | — | Ana ekran = Envelopes sekmesi: "Monthly" ve "Available" (dağıtılmamış fon) grupları, her zarfta üstte kalan bakiye/altta bütçelenen tutar, ince bir ilerleme çubuğu | İki farklı "0.00"/"₺" satırı `[Available]` etiketiyle görünüyor — hangi ikisinin ne olduğu arayüzden anlaşılmıyor (muhtemelen para birimi/dönem slotları) | `08-envelopes-filled-home.png` |
| K02 Hesap/cüzdan oluşturma | Kısmi (paywall) | ~10 | Hesap ekleme formu sade: Ad, Açılış bakiyesi, Tür (Checking/Savings/Cash · Credit Card · Debt) | **Accounts özelliği varsayılan kapalı** ("TURN ON ACCOUNTS" ile açılıyor) — zarf/bütçe modelinden tamamen ayrı bir katman. Ücretsiz sürümde **toplam 1 hesap sınırı, tüm türler ortak havuzu paylaşıyor** (Checking+Credit Card+Debt toplamı 1) | `04-accounts-off-by-default.png`, `05-edit-accounts-empty.png`, `06-ana-hesap-created.png`, `07-account-limit-paywall.png` |
| K03 İşletme geliri | Tamamlandı (ama mimari sorunlu) | ~9 | Tutar girişi + hesap seçimi hızlı; gelir türü net bir isimle ayrılmış | Gelir türü "Income" değil **"Credit"** (banka ekstresi terimi). **Zarf alanı gelirde de zorunlu** — "-Select Envelope-" ile kayıt reddediliyor ("Select an Envelope." uyarısı), gelir bir harcama zarfına veya "Split into multiple Envelopes"e bağlanmak zorunda; "Available"a (dağıtılmamış havuz) doğrudan yatırma seçeneği bu formda yok | `09-credit-type-selected.png`, `10-income-requires-envelope.png`, `11-save-location-prompt.png` |
| K04 Şahsi gider | Tamamlandı | ~6 | Sorunsuz: Market zarfından ₺850 gider, standart akış | — | — |
| K05 İşletme kart gideri | Kısmi (paywall) | ~7 | Form aynı Add Transaction ekranı | **Kredi kartı hesabı oluşturulamadı** (1-hesap limiti zaten Ana Hesap'ta doldu) — kart harcaması gerçek bir "Credit Card" hesabına değil, Ana Hesap'a sıradan gider olarak işlendi. Kart borcu/ekstre modeli hiç görülemedi | — |
| K06 Transfer | Engelli (paywall) | ~4 | "Account Transfer" ayrı, birinci sınıf bir işlem türü (From/To/Amount/Description/Date/Schedule/Notes, açıklama otomatik "Account Transfer") — bizim Transfer modelimize kavramsal olarak çok yakın | Tek hesap olduğu için From/To aynı tek hesaba (Ana Hesap) düşüyor; gerçek bir transfer testi teknik olarak imkânsız | `12-account-transfer-screen.png`, `13-transfer-single-account-blocked.png` |
| K07 Liste, detay ve aylık rapor | Tamamlandı (raporlarda ciddi bulgu) | ~10 | İşlem listesi sade, renkli (+gelir yeşil); satıra dokunma direkt düzenleme formunu açıyor; arama hızlı ve doğru filtreliyor | **Raporlar varsayılan olarak içinde bulunulan takvim ayını (Eylül) gösteriyor**, işlem tarihlerini değil — Ağustos verisi görünmesi için elle tarih aralığı değiştirmek gerekiyor. Tarih aralığı değiştirilince **"Spending by Envelope" raporu Ağustos için "Total Spending: -22.950,00" gösterdi** (negatif!) ve gerçek ₺850 Market harcamasını raporda hiç göstermedi (yalnız Tasarım Yazılımı %100 ₺1.200 görünüyor). Aynı şekilde "Income vs Spending" raporunda Ağustos: **Income 0,00 / Spending -22.950,00** — gerçekte tam tersiydi (+25.000 gelir, -2.050 gider). Kök neden: K03'te zorunlu tutulan gelir→zarf bağlantısı, gelirin "harcama" hesaplamasına karışmasına yol açıyor | `14-reports-default-current-month.png`, `15-report-empty-state.png`, `16-spending-by-envelope-negative-bug.png`, `17-income-vs-spending-bug.png`, `20-search-results.png` |
| K08 Düzeltme/iptal | Tamamlandı (bir bakiye tutarsızlığı bulundu) | ~6 | Düzeltme anında, onaysız kaydediliyor; silme tek onaylı ("Are you sure you want to delete this transaction?" Yes/No) | Test: ₺16 gider oluşturuldu → ₺13'e düzeltildi → silindi. Silme sonrası hem Ana Hesap hem Market zarfı, düzeltme öncesi ilk tutar kadar (₺16) eksik kaldı — **düzenlenip sonra silinen bir kaydın bakiyeye etkisi tam geri alınmıyor** (kalıcı ₺16 sapma). Kesin iç mekanizma doğrulanamadı, yalnız net etki gözlemlendi | `18-delete-confirmation.png`, `22-final-balance-discrepancy.png` |

Sonuç değerleri: `Tamamlandı`, `Desteklenmiyor`, `Ücretli`, `Engelli`, `Belirsiz`, `Kısmi`.

## Ek koşum — A / B / B1 / B2

| Kimlik | Görev | Sonuç | Gözlem | Kanıt |
|---|---|---|---|---|
| A | Kısmi kredi kartı ödemesi (₺400) | `Engelli` (paywall) | Kart hesabı oluşturulamadığı için (1-hesap limiti) test edilemedi; "Account Transfer" özelliği kart ödemesi için kullanılabilecek gibi görünüyor ama doğrulanamadı | — |
| B | Fiş / kamera | `Desteklenmiyor` | Add Transaction ekranının overflow menüsünde yalnız "Help" var; dosya/foto eki, OCR veya herhangi bir belge ekleme seçeneği **hiç yok** — test edilen 5 uygulama arasında bu özelliği hiç taşımayan tek uygulama | `21-no-receipt-attachment.png` |
| B1 | Tekrarlayan gider (₺600/ay, ilk çekim 10 Ağu) | `Tamamlandı` | "Schedule this…" kutusu + tekrarlama sıklığı (Once/Weekly/Every 2-4 Weeks/**Every Month**/Last Day of Month/Every 2-3-6 Months) + "e-posta ile N gün önce hatırlat". Önizleme sonraki 2 örneğin tarihini gösteriyor (Türkçe ay kısaltmasıyla: "Eyl 10, 2026, Eki 10, 2026, Kas 10, 2026"). **İlk örnek gerçek bir işlem olarak anında kaydediliyor**; geleceğe dönük örnekler işlem geçmişinde/aramada görünmüyor (muhtemelen ayrı bir hatırlatıcı/e-posta mekanizmasıyla yürüyor, uygulama içi "bekleyen işlemler" listesi bulunamadı) | `19-recurring-schedule-turkish-dates.png` |
| B2 | Taksitli kart harcaması (₺6.000 = 6×₺1.000) | `Desteklenmiyor` | Add Transaction'da yalnız "Split into multiple Envelopes" var — bu tek işlemi birden fazla **zarfa** bölüştürme (kategori bölme), zaman içine yayılan bir taksit planı değil. Gerçek bir taksit/ödeme planı kavramı yok | — |

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler | Evet | Envelopes / Transactions / Accounts / Reports — dört sabit sekme, hepsi aynı üst çubukta |
| Bir raporun içine tıklama (drill-down) | Evet | Rapor kartına dokunma tam ekran grafiğe götürüyor, oradan tarih aralığı değiştirilebiliyor |
| Bütçe / hedef / planlama ekranı | Evet | Zarf bazlı bütçe zaten ana ekran; ayrı bir "hedef/goal" ekranı yok (Wallet'taki Goal'a benzer bir şey Goodbudget'ta bulunamadı) |
| Ayarların derinliği | Kısmen | Overflow menüde Settings var (derinlemesine gezilmedi — kapsam dışı export/yedek) |
| Arama ve filtre davranışı | Evet | "Transaction Search" — metin/tutar/çek no ile arama, "Advanced Search" seçeneği de var; doğru ve hızlı sonuç veriyor |
| Boş durum ekranları | Evet | "No transactions found." + boş pasta grafik ikonu (rapor tarafında) |
| Hata / uç durum | Evet | Zorunlu alan boşken "All fields are required." toast'ı; envelope seçilmeden kredi/gider kaydı "Select an Envelope." ile reddediliyor; hesap/zarf limiti aşımında "Account Limit Reached" / "8 of 10 free Envelopes left" paywall uyarıları |
| Widget / hızlı giriş / kısayol | Kısmen görüldü | "Add to Quick Transactions widget" seçeneği Add Transaction formunda var (denenmedi) |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Zarf listesinde iki sayı (kalan bakiye / bütçelenen tutar) yan yana — okunması bir süre alıyor ama tutarlı |
| Alt/üst gezinme | Üstte sabit 4 sekme, alt gezinme çubuğu yok; FAB (+) her ekranda işlem eklemeye götürüyor |
| Renklerin anlamı ve tutarlılığı | Yeşil = gelir/pozitif, kırmızı = gider/negatif, tutarlı; zarf ilerleme çubuğu da yeşil/gri |
| Tipografi ve para değerlerinin okunması | Büyük, net rakamlar; virgüllü binlik ayracı (`25,000.00`) — TL yerelleştirmesi yok, İngilizce format sabit |
| Kart, liste ve grafik kullanımı | Raporlar kart+grafik (pasta, çubuk) düzeninde, temiz |
| Form alanları ve varsayılanlar | Amount alanı özel bir tam ekran hesap makinesi popup'ı açıyor (klavye değil); bu popup'ta buton konumları ekran boyutuna göre hesaplanmalı, adb ile dokunuş koordinatı bulmak zorlaştı (uygulamaya özgü bir bulgu değil, test notudur) |
| Loading, boş, hata ve başarı geri bildirimi | Her kayıttan sonra rastgele bir tebrik mesajı çıkıyor: **"Smile! Your transaction is in Goodbudget!"**, **"Good job, Goodbudgeter!"**, **"Way to go, Goodbudgeter!"**, **"Wow! You're so good at budgeting!"** — oyunlaştırılmış (gamification) mikro-metin, diğer 4 uygulamada görülmedi |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Dokunma alanları büyük, metin yoğunluğu düşük — sade bir arayüz |

## Sistem işleyişi / pipeline

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Hesap bakiyesini (varsa) VE seçilen zarfın kalan bakiyesini aynı anda günceller — iki ayrı "defter" (hesap = gerçek para, zarf = bütçe planı) paralel güncelleniyor | Manuel gözlem |
| Kart harcaması → kart borcu → kart ödemesi zinciri | Test edilemedi (paywall); "Credit Card" hesap türü var ama içeriği görülemedi | Doğrulanamadı |
| Transfer / kart ödemesi gelir-gider raporundan ayrışıyor mu | "Account Transfer" ayrı bir işlem türü (gelir/gider değil); raporlara etkisi test edilemedi (tek hesap engeli) | Doğrulanamadı |
| Fatura/borç → tahsilat/ödeme → kapanış akışı | Yok — böyle bir kavram bulunamadı | Manuel gözlem |
| Tekrarlayan/planlı kayıt: tanım mı üretir, onay mı bekler | **Karma model**: ilk örnek anında gerçek kayıt olarak yazılıyor (Money Manager'a benzer), ama geleceğe dönük örnekler geçmişte/aramada görünmüyor — muhtemelen yalnız e-posta hatırlatıcısı olarak yürüyor; uygulama içi "bekleyen tekrarlayan işlemler" listesi bulunamadı | Manuel gözlem (kısmi — tam mekanizma doğrulanamadı) |
| İşletme/şahsi (veya en yakın) ayrım hangi katmanda | Yok. En yakın benzer boyut zarf (kategori) ama işletme/şahsi diye ayrı bir alan/mod yok | Manuel gözlem |
| Ekrandan ekrana tipik yol | `+ → Payee → Amount+Tür(Expense/Credit) → Envelope(zorunlu) → Account → Date → (opsiyonel: Schedule) → Save → zarf+hesap güncellenir` | Manuel gözlem |
| Entegrasyon/dış sistem temas noktaları | Yok — banka bağlama, e-belge, POS, muhasebeci aktarımı hiçbiri yok | Manuel gözlem |
| Veri nereye yazılıyor | Bulut (household hesabı, gerçek e-posta ile); "Last Backup: <1m ago" göstergesi sürekli görünüyor | Manuel gözlem |

**Pipeline şeması (kısa):**
`+ → Expense/Credit seç → Tutar (özel hesap makinesi popup) → Zarf seç (zorunlu, "Available"a doğrudan yatırma yok) → Hesap seç → Tarih → Kaydet → hesap bakiyesi + zarf bakiyesi + rapor (cari ay varsayılan) güncellenir`

**Ayrı "Fill Envelopes" akışı (bütçe fonlama):** `Envelopes sekmesi → menü → Fill Envelopes → "From New Income" / "From Available" → her zarfa "Set to Budget Amt" veya elle tutar → Next → household bakiyesine dokunmadan/dokunarak zarfları doldurur`. Bu, normal Add Transaction akışından tamamen ayrı, felsefeye özgü bir ikinci giriş noktası.

## Akış özeti

- En kısa ve güçlü akış: Zarf içi basit gider (K04) — hızlı, sürtünmesiz.
- En fazla sürtünme yaratan akış: Gelir kaydı (K03) — zorunlu zarf seçimi hem kavramsal olarak tuhaf hem de rapor bütünlüğünü bozuyor (bkz. K07 bulgusu).
- Uygulamanın hedef kullanıcı varsayımı: Tek bir gerçek kişi/aile, sabit aylık gelir, harcamadan önce bütçeleyen disiplinli kullanıcı.
- İşletme ve şahsi para yaklaşımı: Yok; kavram uygulamada hiç mevcut değil.
- Transfer ve kart ödemesi yaklaşımı: Transfer birinci sınıf bir tür (Account Transfer) ama test edilemedi; kart ödemesi hiç görülemedi (paywall).
- Planlama, borç ve tahsilat yaklaşımı: Yalnız "Schedule this…" ile tekrarlayan işlem var; borç/tahsilat/fatura kavramı yok.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Zarf bazlı "harcamadan önce dağıt" felsefesi | Alma | Kurucu tezimizle (kaydet → raporla, tek havuz) doğrudan çelişiyor; BusinessFinance bütçe modülü zaten kategori+kapsam üzerinden ilerliyor | Bütçe akışı |
| Gelirin zorunlu olarak bir harcama-zarfına bağlanması ve bunun rapor bütünlüğünü bozması | Alma (olumsuz referans olarak kullan) | K07'de somut kanıtı görüldü: gelir/gider ayrımının raporlama katmanında zorunlu ve ayrı tutulması gerektiğinin gerçek dünya kanıtı — ADR 0013/gelir-gider ayrı raporu kuralımızı doğrudan doğruluyor | `documentation/adr/0013-*.md` gerekçe metni, muhasebe raporu tasarımı |
| Hesap (gerçek para) ile bütçe (zarf) katmanının birbirinden tamamen ayrılması, hesapların opsiyonel olması | Henüz karar verme | İlginç bir mimari fikir (bütçe planı ile gerçek bakiye ayrı) ama bizim modelimizde hesap zorunlu ve merkezi; büyük bir yeniden tasarım gerektirir | Hesap modeli |
| Tekrarlayan işlem: ilk örneği anında gerçek kayıt yapıp geleceği yalnız hatırlatıcıya bırakma | Alma (olumsuz referans) | BusinessFinance'in "tanım hiçbir şey üretmez, occurrence açık realize edilir" modeli (Bluecoins'e en yakın) bundan daha tutarlı; Goodbudget'ın karma modeli kafa karıştırıcı | Recurring/planlanan akış |
| Düzenleme sonrası silmede bakiye tutarsızlığı (K08 bulgusu) | Uyarlayarak al (ters örnek) | Düzeltme=iptal+yeni kayıt prensibimizin (idempotent iptal) neden önemli olduğunu somutlaştıran bir olumsuz örnek | İşlem iptal/düzeltme akışı |
| "Account Transfer" birinci sınıf, ayrı bir işlem türü (Expense/Credit değil) | Uyarlayarak al | Bizim Transfer modelimize kavramsal olarak yakın; From/To/Amount/Description/Date alan seti referans alınabilir | Transfer formu |
| Fiş/kamera desteğinin tamamen yokluğu | Doğrudan al (fark yarat) | Rakiplerin en zayıf halkası bile en az dosya eki sunuyordu; BusinessFinance'in ADR 0011 öneri katmanı burada net bir fark yaratabilir | Fiş okuma akışı |
| Oyunlaştırılmış başarı mikro-metinleri ("Good job, Goodbudgeter!") | Henüz karar verme | Hoş bir dokunuş ama ton/marka kararı; işletme kullanıcısına ne kadar uygun düşer tartışmalı | Genel UX tonu |
| 1-hesap / 10-zarf ücretsiz sınırı ve paywall diyalogları | Alma | Freemium taktik bir karar, BusinessFinance'in kapsamı dışında | — |

## Kanıt ve güven düzeyi

- Manuel gözlem: K00–K08 + A/B/B1/B2 ek koşumu, tüm ekran görüntüleri canlı emülatörde alındı (kareler `01`–`22`).
- Resmî kaynak: Yok.
- Yorum: K08'deki bakiye tutarsızlığının tam iç mekanizması (hangi adımda tam olarak kayboldu) doğrulanamadı, yalnız net dış etki (₺16 kalıcı sapma) gözlemlendi.
- Doğrulanamadı: B1'in gelecek örneklerinin gerçekte nasıl/nerede yürütüldüğü (e-posta hatırlatıcısı dışında uygulama içi bir "bekleyen" listesi bulunamadı, ama derinlemesine aranmadı); A ve K05/K06 kart/transfer davranışları paywall nedeniyle hiç gözlemlenemedi.

## Tek cümlelik sonuç

Goodbudget, "harcamadan önce bütçeye dağıt" felsefesiyle BusinessFinance'ten en uzak duran rakip; ama gelirin zorunlu olarak bir harcama-zarfına bağlanmasının rapor bütünlüğünü bozduğunu somut olarak göstermesi (K07), bizim ayrı gelir/gider raporu ve tek-havuz kararlarımızın (ADR 0013) neden doğru olduğuna dair en güçlü olumsuz kanıtı sağlıyor.
