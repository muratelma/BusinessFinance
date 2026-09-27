# Uygulama Gözlem Formu — Goodbudget

## G01 sonrası düzeltme — 14 Eylül 2026

Önceki agentın E0106–E0133 kapsamındaki 28/28 görsel incelemesi kullanıldı.
Görseller yeniden açılmadı; eski koşumlar korunur. Yeni canlı test yapılmadı.
Eski 22/24 sayıları tarihsel kayıt tutarsızlığıdır; güncel kapsam 28/28'dir.
Goodbudget 14 Eylül kullanıcı kontrolüyle kapandı; toplam 31 görsel incelendi.
Kapanış kaydı BULGU-DOGRULAMA-KAYDI.md sonundadır.

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Goodbudget (Dayspring Technologies) |
| Sürüm | 2.24.26013 (180) — açılış ekranında yazıyor (`01-ilk-acilis.png`) |
| Test tarihi | **11 Eylül 2026** (Tur 1 + A/B/B1/B2) · **12 Eylül 2026** (Faz 7.5 doğrulama turu) |
| Cihaz / işletim sistemi | Pixel 8 AVD (Android 17), sistem dili Türkçe |
| Dil / para birimi | Uygulama arayüzü tamamen İngilizce (para birimi/yerelleştirme ayarları incelenmedi); sistem bileşenleri (tarih seçici, klavye, toolbar) Türkçe. Tarih biçimi **MM/DD/YYYY** (ABD), rapor başlıkları ise Türkçe ay kısaltmalı (`1 Ağu 2026 - 31 Ağu 2026`) — aynı ekranda iki yerelleştirme karışıyor |
| Hesap veya plan türü | Ücretsiz (household `elma60038`), gerçek kullanıcı e-postasıyla kayıt |
| Erişim kısıtı | Ücretsiz sürüm: 10 zarf, **1 ana hesap** sınırı; Debt hesapları resmî kaynakta ayrı tutulur |
| İnceleme türü | Manuel gözlem |

## Ürün kimliği ve asıl amaç

| Alan | Kısa not |
|---|---|
| Tek cümlelik ürün tezi | "Zarf bütçeleme" (envelope budgeting): harcamadan önce parayı kategorilere (zarflara) dağıt, yalnız zarfta parası olan kategoriden harca |
| Asıl hedef kullanıcı | Kişisel/aile bütçesi tutan, nakit akışını "önceden planlanmış" tutmak isteyen bireysel kullanıcı — dijital zarf sistemi klasiği |
| Çözdüğü ana iş | Aylık geliri kategorilere önceden dağıtıp aşırı harcamayı zarf bazında sınırlamak |
| Açıkça kapsam dışı bıraktığı | Koşumda işletme/fatura akışı bulunamadı. Kart/transfer erişim nedeniyle denenmedi; taksit/fiş incelenen yüzeyde bulunamadı. Debt ve Premium banka senkronizasyonu kaynakta var |
| İş modeli | Freemium: 10 zarf + 1 hesap ücretsiz; sınırsızı abonelik (`SUBSCRİBE` / `GET UNLİMİTED ACCOUNTS`) |
| BusinessFinance ile aynı kulvarda mı | Hayır — incelenen akışta işletme/şahsi boyutu yok; zarflar sınıflandırma sağlar, hesap türleri vardır; para modeli de ters yönde kuruluyor ("harcamadan önce dağıt" vs bizim "kaydet, sonra raporla") |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | ~12 | Açılışta `LOG IN` / `CREATE NEW HOUSEHOLD` ikilisi. **Kayıt en sona bırakılabiliyor**: önce `Setup Budget` sihirbazında zarflar ve `Estimated Monthly Income` giriliyor, zarflar dolduruluyor, `Register Household` ekranı ancak ondan sonra geliyor ve `LATER` düğmesi görünüyor; atlama denenmedi | Kayıt formu placeholder'ı gerçek görünümlü (`johnj@email.com`), boş alan sanılabiliyor. Bütçeyi **planlamak** (`Setup Budget`) ile parayı zarflara **fiilen koymak** (`Fill Envelopes`) iki ayrı adım; ikincisi ayrı bir diyalogla (`Great! Your budget is set.` → `LET ME DO İT` / `FİLL 'EM FOR ME!`) başlıyor | `01-ilk-acilis.png` → `02b-add-envelope-formu.png` → `02-setup-budget-envelope.png` → `02c-fill-envelopes-prompt.png` → `03-register-household.png` |
| K01 Ana ekran | Tamamlandı | — | Ana ekran = `ENVELOPES` sekmesi: üstte `Last Backup: <1m ago` ve `Total`, altında `Monthly` grubu (her zarfta üstte kalan bakiye, altta bütçelenen tutar ve ince bir ilerleme çubuğu), sonra `Available` grubu | `Available` başlığının altında **iki ayrı `[Available]` satırı** görünüyor; ikisinin neyi ayırdığı arayüzde hiçbir yerde yazmıyor (`çıkarım`: biri aylık, diğeri aylık olmayan zarf grubunun dağıtılmamış fonu — doğrulanamadı). Ayrıca bu sekmedeki `Total`, **hesap bakiyesi değil zarflardaki para**; hesap parası ayrı sekmede | `08-envelopes-filled-home.png` |
| K02 Hesap/cüzdan oluşturma | Kısmi (paywall) | ~10 | `Edit Accounts` üç grup gösteriyor: `Checking, Savings, or Cash` · `Credit Card` · `Debt`. Hesap alanları eski koşum notunda ad/açılış/tür olarak anlatılmış; formun kendisi karelerde yok | **`ACCOUNTS` özelliği varsayılan kapalı** (`TURN ON ACCOUNTS` ile açılıyor) — zarf/bütçe katmanından tamamen ayrı bir katman, kullanılmadan da bütçe tutulabiliyor. Ücretsiz sürümde **toplam 1 hesap**, bu limite Debt hesapları genellenemez: `Account Limit Reached — You've reached your limit of 1 Account.` | `04-accounts-off-by-default.png`, `05-edit-accounts-empty.png`, `06-ana-hesap-created.png`, `07-account-limit-paywall.png` |
| K03 İşletme geliri | Tamamlandı (Credit denemesi; normal gelir değil) | ~9 | `Add Transaction` tek ekran: `Payee` · `Amount` + tür · `Envelope` · `Account` · `Date` · `Check #` · `Schedule this…` · `Notes` · `Add to Quick Transactions widget` · `Save location to transaction`. Tarih varsayılanı bugün | Denenen tür **`Credit`**; **bu yolda zarf zorunlu**: `- Select Envelope -` ile kaydetmek `Select an Envelope.` toast'ıyla reddediliyor. ₺25.000 Market zarfına Credit olarak yazıldı. Açılır listenin tamamı görünmüyor. Normal gelir ayrı Fill Envelopes yoludur; bu sonuç bütün gelirlere genellenmez | `09-credit-type-selected.png`, `10-income-requires-envelope.png` |
| K04 Şahsi gider | Tamamlandı | ~6 | `Market` zarfından ₺850 gider; standart akış, sürtünmesiz | — | `23-transactions-initial-envelope-fill.png` (`08/05 Market 850.00`) |
| K05 İşletme kart gideri | Kısmi (paywall) | ~7 | Form aynı `Add Transaction` ekranı | **Kredi kartı hesabı oluşturulamadı** (1-hesap limiti `Ana Hesap`'ta dolmuştu) — kart harcaması `Credit Card` türünde bir hesaba değil, `Ana Hesap`'a sıradan gider olarak işlendi. Kart borcu/ekstre modeli **hiç görülemedi** | `07-account-limit-paywall.png` |
| K06 Transfer | Engelli (paywall) | ~4 | `Account Transfer` ayrı, birinci sınıf bir işlem türü: `From` · `To` · `Amount` · `Description` (otomatik "Account Transfer") · `Date` · `Schedule this…` · `Notes`. Gelir/gider türlerinden tamamen ayrı bir ekran | Tek hesap olduğu için `From`/`To` açılırları aynı tek kaydı (`Ana Hesap`) listeliyor; gerçek transfer testi teknik olarak imkânsız | `12-account-transfer-screen.png`, `13-transfer-single-account-blocked.png` |
| K07 Liste, detay ve aylık rapor | Tamamlandı (Credit koşumunun rapor sonucu) | ~10 | İşlem listesi sade ve okunaklı: tarih + payee + `Zarf \| Hesap` alt satırı, gelir yeşil ve `+` işaretli. Beklenen kayıt bulundu; hız ölçülmedi. Rapor kartına dokunma tam ekran grafiğe ve oradan tarih aralığına götürüyor | **Raporlar varsayılan olarak içinde bulunulan takvim ayını gösteriyor**, verinin bulunduğu ayı değil — Ağustos verisi için aralığı elle değiştirmek gerekiyor. Aralık Ağustos'a çekilince `Spending by Envelope` **`Total Spending: -22,950.00`** (negatif) veriyor ve listede yalnız `Tasarim Yazilimi 100% 1,200.00` satırı çıkıyor. `Income vs Spending` (1 Tem – 30 Eyl) tablosunda **Ağustos: Income 0,00 / Spending −22.950,00 / Net 22.950,00**; rapor anında +25.000 Credit ve 2.050 gider vardı; 600 gider daha sonra eklendi | `14-reports-default-current-month.png`, `15-report-empty-state.png`, `16-spending-by-envelope-negative-bug.png`, `17-income-vs-spending-bug.png`, `20-search-results.png` |
| K08 Düzeltme/iptal | Tamamlandı (farkın nedeni belirsiz) | ~6 | Edit Transaction, ✓ ve çöp kutusu; silme onayı görüldü | Eski not 16 → 13 → silme anlatıyor; 13 değişikliğinin kaydedildiği karede doğrulanmıyor. Silinen satır sonraki listede yok, hesapta 16 farkı kaldı; genel silme kusuru sonucu çıkarılmaz | `18-delete-confirmation.png`, `22-final-balance-discrepancy.png`, `23-transactions-initial-envelope-fill.png` |

Sonuç değerleri: `Tamamlandı`, `Desteklenmiyor`, `Ücretli`, `Engelli`, `Belirsiz`, `Kısmi`.

## Kontrol değeri doğrulaması

Goodbudget'ta **iki ayrı toplam** var ve ikisi aynı şeyi saymıyor: `ACCOUNTS`
sekmesindeki `All Accounts` (gerçek para) ve `ENVELOPES` sekmesindeki `Total`
(zarflardaki para). Kontrol yalnız birincisiyle yapılabilir.

`23-transactions-initial-envelope-fill.png` karesindeki işlem listesinin
tamamı (5 satır) ve `Ana Hesap`'ın ₺20.000 açılış bakiyesi:

| Adım | Tutar | Beklenen `Ana Hesap` |
|---|---:|---:|
| Açılış bakiyesi | +20.000 | 20.000 |
| `08/03 Ada Reklam` (Credit) | +25.000 | 45.000 |
| `08/05 Market` | −850 | 44.150 |
| `08/08 Mavi Yazilim` | −1.200 | 42.950 |
| `08/10 Bulut Yazilim Abonelik` (B1 ilk örneği) | −600 | **42.350** |
| `09/11 Initial Envelope Fill` (+2.050) | hesaba dokunmuyor | 42.350 |

| Ölçüm | Beklenen | Ekranda | Fark | Kanıt |
|---|---:|---:|---:|---|
| `Ana Hesap`, 11 Eyl 09:30 (K08 testinden önce) | 42.950 | 42.950 | 0 ✓ | `13-transfer-single-account-blocked.png` |
| `Ana Hesap`, 11 Eyl 09:56 (koşum sonu) | 42.350 | **42.334** | **−16** | `22-final-balance-discrepancy.png` |
| `Ana Hesap`, 12 Eyl 12:20 (hiçbir işlem yapılmadan) | 42.350 | **41.734** | **−616** | `24-accounts-balance-41734.png` |

**İki ayrı sapma var:**

1. **−16**: K08 sonrası 16 farkı kaydedildi; silinen test satırı sonraki
   listede yok. 13 değişikliğinin kaydedildiği ve farkın hangi adımdan doğduğu
   bilinmiyor. `18-delete-confirmation.png`, `22-final-balance-discrepancy.png`,
   `23-transactions-initial-envelope-fill.png`.
2. **−600**: Eski koşum notu arada işlem yapılmadığını bildiriyor. Hesap
   görüntüleri 42.334 ve 41.734; liste/arama tek 10 Ağustos 600 kaydı gösteriyor.
   Neden bilinmiyor; abonelikle tutar eşitliği nedensellik kanıtı değildir.
   Eski rapor karesi sonraki bakiye anını kanıtlamaz.
   `22-final-balance-discrepancy.png`, `24-accounts-balance-41734.png`,
   `23-transactions-initial-envelope-fill.png`, `25-arama-tek-bulut-satiri.png`.

**Değerlendirme:** koşum farkları korunur; silmenin genel olarak para iade
etmediği veya tekrarın liste dışı para ürettiği sonucu çıkarılmaz. Kök neden
için yeni deney veya vade beklenmez.

## Rapor davranışı — sayılar nereden geliyor

Rakibin en dikkat çekici davranışı burada, o yüzden ayrı tutuldu.

- `Fill Envelopes` işlemi, işlem listesinde **`Initial Envelope Fill` adlı
  gerçek bir satır** olarak duruyor: `09/11`, **+2.050,00**, zarf/hesap alt
  satırı yok. Eylül raporunda da `Income 2,050` vardır. Eşleşme gözlemdir;
  kurulumun fonlama seçimi görünmediği için bütün Fill işlemlerine genellenmez.
  `23-transactions-initial-envelope-fill.png`, `14-reports-default-current-month.png`
- Ağustos'ta `Spending by Envelope` `Total Spending: -22,950.00` veriyor ama
  listede tek satır var (`Tasarim Yazilimi 100% 1,200.00`). Aritmetik tutuyor:
  `1.200 + 850 − 25.000 = −22.950`. Yani ₺850'lik `Market` harcaması **toplamın
  içinde var, satır listesinde yok**; ₺25.000 gelir ise bir harcama zarfına
  yazıldığı için **negatif harcama** gibi toplanıyor.
  `16-spending-by-envelope-negative-bug.png`
- `Income vs Spending` aynı veriyi ay ay gösteriyor: Tem 0/0, **Ağu
  0,00 / −22.950,00 / Net 22.950,00**, Eyl 2.050/0/2.050.
  `17-income-vs-spending-bug.png`

**Anlatım sınırı:** Sonuçlar Credit ile girilmiş 25.000 ve kurulumdaki
Initial Fill kaydına aittir. Normal gelir ve From Available yollarının aynı
sonucu ürettiği ölçülmedi. Market satırının neden görünmediği bilinmiyor;
negatif net zarf açıklaması hipotezdir. Genel rapor kusuru hükmü kurulmaz.

## Ek koşum — A / B / B1 / B2

| Kimlik | Görev | Sonuç | Gözlem | Kanıt |
|---|---|---|---|---|
| A | Kısmi kredi kartı ödemesi (₺400) | `Engelli` (paywall) | Kart hesabı oluşturulamadığı için test edilemedi; `Account Transfer` kart ödemesi için kullanılabilecek gibi görünüyor ama doğrulanamadı | `07-account-limit-paywall.png` |
| B | Fiş / kamera | `Desteklenmiyor` | `Add Transaction` overflow menüsünde **yalnız `Help`** var; dosya/foto eki, OCR veya belge ekleme seçeneği hiç yok. Sonuç yalnız incelenen işlem formuna aittir; Settings açılmadı | `21-no-receipt-attachment.png` |
| B1 | Tekrarlayan gider (₺600, ilk çekim 10 Ağu) | `Tamamlandı` | `Schedule this…` kutusu + sıklık açılırı. Tam liste kareye alındı: `Once` · `Weekly` · `Every 2 Weeks` · `Every 4 Weeks` · `Every Month` · `Last Day of Month` · `Every 2 Months` · `Every 3 Months` · `Every 6 Months` (+ ekrana sığmayan en az bir seçenek daha), `Once` bir seçenektir; varsayılanı kare kanıtlamaz. Ayrıca "…mail **3** days before" ile e-posta hatırlatıcısı. **Koşumda `Every 2 Weeks` seçildi**; form altında "…ansactions will be on: …2026, Eyl 7, 2026, Eyl 21, 2026" önizlemesi çıktı. **İlk örnek anında gerçek bir işlem olarak kaydedildi** (`08/10 Bulut Yazilim Abonelik`, aramada çıkıyor) | `19-recurring-schedule-turkish-dates.png`, `26-schedule-frequency-listesi.png`, `20-search-results.png` |
| B2 | Taksitli kart harcaması (₺6.000 = 6×₺1.000) | `Desteklenmiyor` | `Add Transaction`'da yalnız `Split into multiple Envelopes` var — tek işlemi birden fazla **zarfa** bölüştürme (kategori bölme), zamana yayılan bir taksit planı değil. İncelenen yolda taksit kurulmadı; ürün geneline yokluk genellenmez | — (negatif; `09-credit-type-selected.png` kısmi formu gösteriyor; alt alanlar bütünüyle görünmüyor) |

**B1 sınırı:** İlk 10 Ağustos kaydı ve Every 2 Weeks seçimi kanıtlıdır.
Sonraki üretimin zamanı/onayı ve 600 farkıyla ilişkisi bilinmiyor. Ayrı
bekleyen liste bulunmaması ilk kaydı geçersiz kılmaz. Geçmiş iki vadenin
mutlaka iki kayıt üretmesi gerektiği de kanıtlanmış bir ürün kuralı değildir.

## Faz 7.5 doğrulama turu (12 Eylül 2026)

24 kare tek tek açıldı ve metinle karşılaştırıldı; emülatörde household
oturumu hâlâ açıktı, bu yüzden davranış soruları da ölçülebildi.

**Kare ile metin uyuşmuyordu:**

- **B1'in sıklığı ve önizleme tarihleri yanlış yazılmıştı.** Form "**Every
  Month**" ve önizleme olarak "Eyl 10, 2026, Eki 10, 2026, Kas 10, 2026"
  diyordu; karede seçili olan **`Every 2 Weeks`** ve önizleme
  "…Eyl 7, 2026, Eyl 21, 2026". Sıklık listesi de formda kanıtsız sayılmıştı,
  şimdi kareye alındı (`26-schedule-frequency-listesi.png`).
- **Kayıt sırası tersti.** Form "household kaydı hızlı; **ardından** Setup
  Budget sihirbazı geliyor" diyordu. Karelerde sıra tam tersi: önce bütçe
  kurulumu ve zarf doldurma, **sonra** `Register Household` — üstelik `LATER`
  düğmesi görünüyor; atlama denenmedi (`03-register-household.png` üstündeki banner:
  "Envelopes Filled! Register to see your budget on the web…").
- **"Örnek zarflarla (Groceries/Gas/Savings) geliyor"** iddiasını hiçbir kare
  desteklemiyordu; sihirbaz karesinde yalnız bizim girdiğimiz iki zarf var.
  İddia düştü.
- **"₺850 Market harcamasını raporda hiç göstermedi"** yarı doğruydu: harcama
  satır listesinde yok ama `−22.950` toplamının **içinde** (aritmetik
  yukarıda). Cümle kesinleştirildi.
- `17-income-vs-spending-bug.png` yalnız Ağustos'u değil **1 Tem – 30 Eyl**
  aralığını gösteriyor; Eylül satırındaki `Income 2.050` formda hiç
  anılmamıştı ve asıl mekanizmanın (`Initial Envelope Fill`) ipucuydu.
- `[Available]` çiftliği için yazılmış "muhtemelen para birimi/dönem slotları"
  tahmini `çıkarım` etiketi taşımıyordu; etiketlendi ve tahmin daraltıldı.

**İki yetim kare forma bağlandı:** `02c-fill-envelopes-prompt.png` (bütçe
kurulumundan `Fill Envelopes`e geçiş diyaloğu) ve `02b-add-envelope-formu.png` — ikincisi aslında
"envelope suggestions" değil **`Add Envelope` formu** olduğu için
`02b-add-envelope-formu.png` adına çevrildi ve K00 akışına yerleştirildi.

**Emülatörde tamamlananlar (4 yeni kare, E0130–E0133):** işlem listesinin tamamı
(formda hiç kare yoktu ve `Initial Envelope Fill` satırı burada görüldü),
güncel hesap bakiyesi, `Bulut` aramasının tek satır döndürmesi, sıklık
listesi. Kontrol değeri tablosu bu karelerden kuruldu — **formda hiç kontrol
değeri doğrulaması yoktu.**

**Test verisi:** dokunulmadı; hiçbir kayıt eklenmedi, silinmedi veya
düzenlenmedi. Açılan `Add Transaction` formu kaydedilmeden kapatıldı.

## Arayüz taraması (görev dışı)

| Alan | Gezildi mi | Kısa gözlem | Kanıt |
|---|---|---|---|
| Tüm ana sekmeler | Evet | `ENVELOPES` / `TRANSACTİONS` / `ACCOUNTS` / `REPORTS` — dört sabit üst sekme | `08-envelopes-filled-home.png`, `23-transactions-initial-envelope-fill.png`, `24-accounts-balance-41734.png`, `14-reports-default-current-month.png` |
| Rapor drill-down | Evet | Rapor kartına dokunma tam ekran grafiğe götürüyor, üst barda takvim ikonuyla tarih aralığı değiştiriliyor | `15-report-empty-state.png`, `16-spending-by-envelope-negative-bug.png` |
| Bütçe / hedef / planlama ekranı | Evet | Zarf bazlı bütçe zaten ana ekran; gezilen yüzeyde ayrı hedef/goal ekranı bulunmadı | `08-envelopes-filled-home.png` |
| Ayarların derinliği | **Hayır** | Overflow menüde `Settings` var ama içi hiç gezilmedi — formda "kısmen" yazıyordu, gerçekte hiç açılmadı | — (`Doğrulanamadı`) |
| Arama ve filtre | Evet | `Search Transactions` diyaloğu: serbest metin + `Advanced Search` bağlantısı → `Transaction Search` sonuç ekranı. Beklenen kayıt bulundu; hız ölçülmedi | `20-search-results.png`, `25-arama-tek-bulut-satiri.png` |
| Boş durum ekranları | Evet | Rapor tarafında `No transactions found.` + gri boş pasta grafik | `15-report-empty-state.png` |
| Hata / uç durum | Evet | Zarf seçilmeden kayıt `Select an Envelope.` toast'ıyla reddediliyor; hesap limitinde `Account Limit Reached`, zarf sayacında `8 of 10 free Envelopes left` | `10-income-requires-envelope.png`, `07-account-limit-paywall.png`, `02-setup-budget-envelope.png` |
| Konum izni / payee önerisi | Evet | `Save location to transaction` işaretlenince `Save Location? Goodbudget will suggest this payee when you're at this location.` diyaloğu çıkıyor — konuma göre payee önerisi. İncelenen uygulamalarda başka örneği görülmedi | `11-save-location-prompt.png` |
| Widget / hızlı giriş | Kısmen | Formda `Add to Quick Transactions widget (Edit from Settings)` seçeneği var; widget denenmedi | `09-credit-type-selected.png` |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Zarf satırında iki sayı (kalan / bütçelenen) alt alta + ilerleme çubuğu; tutarlı ama ilk bakışta hangisinin hangisi olduğu öğrenilmeyi gerektiriyor |
| Alt/üst gezinme | Üstte sabit 4 sekme, alt gezinme çubuğu yok; FAB (`+`) her ekranda işlem eklemeye götürüyor |
| Renklerin anlamı | İşlem listesinde pozitif tutarlar yeşil ve + işaretli, giderler koyu renkte; zarf ilerleme çubuğu yeşil, boş zarfta gri. Tutarlı |
| Tipografi ve para | Büyük, net rakamlar; **`25,000.00`** biçimi (ABD binlik/ondalık ayracı), koşumda bu biçim görüldü. İncelenen tutarlarda para birimi simgesi yok; ayarlar denenmedi |
| Kart, liste ve grafik | Raporlar kart + grafik (pasta, çubuk) düzeninde, temiz; işlem listesi iki satırlı ve okunaklı |
| Form alanları ve varsayılanlar | `Amount` alanı özel bir tam ekran hesap makinesi popup'ı açıyor (sistem klavyesi değil). Tarih varsayılanı bugün, biçim `MM/DD/YYYY` |
| Loading, boş, hata ve başarı | Her kayıttan sonra rastgele bir tebrik mesajı: `Smile! Your transaction is in Goodbudget!`, `Good job, Goodbudgeter!`, `Way to go, Goodbudgeter!`, `Wow! You're so good at budgeting!` — oyunlaştırılmış mikro-metin, incelenen diğer uygulamalarda görülmedi |
| Erişilebilirlik | Görsel yorum: metin yoğunluğu düşük. Dokunma hedefi ölçümü ve erişilebilirlik testi yapılmadı |

## Sistem işleyişi / pipeline

| Konu | Gözlem | Kanıt etiketi |
|---|---|---|
| Bir gelir/gider kaydı arka planda ne üretir | Hesap ve zarf ayrı ölçülür (fiziksel yapı bilinmiyor). İlk Expense/Credit koşumunda iki gösterge etkilenmişti; 20 Eylül K3 From New Income / Fill Each Envelope yolunda zarf +1.234, hesap değişmedi (41.734). Bütün kayıt türlerine aynı güncelleme kuralı genellenmez; hesap katmanı opsiyoneldir | Manuel gözlem; K3 neden bilinmiyor |
| Zarf doldurma ne üretir | Initial Envelope Fill 2.050 satırı ve Eylül Income 2.050 eşleşiyor; kurulumun fonlama yolu bilinmiyor. Bütün Fill işlemlerine genellenmez | Manuel gözlem; neden bilinmiyor |
| Kart harcaması → kart borcu → kart ödemesi zinciri | Test edilemedi (paywall); `Credit Card` hesap türü listede var ama içeriği görülemedi | Doğrulanamadı |
| Transfer / kart ödemesi gelir-gider raporundan ayrışıyor mu | `Account Transfer` ayrı bir işlem türü (gelir/gider değil); raporlara etkisi tek hesap engeli yüzünden test edilemedi | Doğrulanamadı |
| Fatura/borç → tahsilat/ödeme → kapanış akışı | Yok — böyle bir kavram bulunamadı | Manuel gözlem |
| Tekrarlayan/planlı kayıt: tanım mı üretir, onay mı bekler | İlk kayıt normal listede; sonraki örneklerin zamanı/onayı ve 600 farkıyla ilişkisi bilinmiyor. Ayrı bekleyen liste bulunamadı | Manuel gözlem; mekanizma doğrulanamadı |
| İşletme/şahsi ayrım hangi katmanda | Yok. En yakın boyut zarf (kategori); işletme/şahsi diye bir alan veya mod yok | Manuel gözlem |
| Ekrandan ekrana tipik yol | `+ → Payee → Amount + tür (Expense/Credit) → Envelope (zorunlu) → Account → Date → (opsiyonel Schedule) → ✓ → zarf + hesap güncellenir` | Manuel gözlem |
| Entegrasyon/dış sistem temas noktaları | Household, e-posta ve konum önerisi yüzeyleri görüldü; dış davranışları denenmedi. Premium banka senkronizasyonu resmî kaynakta var, bağlantı kurulmadı | Manuel gözlem / resmî kaynak |
| Veri nereye yazılıyor | Household ve Last Backup göstergesi görüldü; bütün verinin yerleşimi ve restore başarısı kanıtlanmadı | Manuel gözlem (gösterge) |

**Pipeline şeması (kısa):**
`+ → Expense/Credit → Tutar (özel hesap makinesi) → Zarf (denenen Credit yolunda zorunlu) → Hesap → Tarih → ✓ → hesap bakiyesi + zarf bakiyesi + rapor (varsayılan cari ay)`

**Ayrı `Fill Envelopes` akışı (bütçe fonlama):**
`ENVELOPES → menü → Fill Envelopes → "From New Income" / "From Available" → her zarfa "Set to Budget Amt" veya elle tutar → Next`.
Normal `Add Transaction` akışından tamamen ayrı, felsefeye özgü ikinci bir
giriş noktası. Kurulumdaki Initial Fill kaydının hangi fonlama seçimiyle üretildiği bilinmiyor.

## Akış özeti

- **En kısa ve güçlü akış:** Zarf içi basit gider (K04) — hızlı, sürtünmesiz
- **En fazla sürtünme yaratan akış:** Credit ile gelir girişi denemesi (K03); normal gelir
  yolu ile karıştırılması eski yorumu değiştirmişti
- **Uygulamanın hedef kullanıcı varsayımı:** Tek kişi/aile, düzenli aylık
  gelir, harcamadan önce bütçeleyen kullanıcı
- **İşletme ve şahsi para yaklaşımı:** Yok; kavram uygulamada hiç mevcut değil
- **Transfer ve kart ödemesi yaklaşımı:** Transfer birinci sınıf bir tür
  (`Account Transfer`) ama test edilemedi; kart ödemesi hiç görülemedi
- **Planlama, borç ve tahsilat yaklaşımı:** Yalnız `Schedule this…`;
  fatura/tahsilat akışı bulunamadı; Debt içeriği denenmedi

## BusinessFinance için kararlar

> `alma` ve `kararı yeniden sor` satırlarında "ne kazandırıyor / ne
> kaybettiriyor" zorunludur (`MANUEL-TEST-PROTOKOLU.md` → yazım kuralı 6).
> Sonuç tanımları: `README.md` → "Karar sonuçları — tek kaynak".
>
> **`kararı yeniden sor` eşiğini geçen bulgu çıkmadı.** En yakın aday, hesap
> katmanının opsiyonel olması (bütçe planı ile gerçek bakiyenin birbirinden
> bağımsız yaşaması); adı konmuş bir boyutta bizden iyi olduğu
> gösterilemediği için `henüz karar verme` olarak işlendi.

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Zarf bazlı "harcamadan önce dağıt" felsefesi | Alma | **Kazandırdığı:** kullanıcı ayın başında bütün parasını bir işe atamak zorunda kalıyor; "nereye gitti" sorusu doğmadan önce cevaplanıyor ve harcama disiplini desteklenebilir (çıkarım; teknik engelleme denenmedi). **Kaybettirdiği:** düzensiz gelirli kullanıcı (esnaf, serbest çalışan) her para girişinde dağıtım yapmak zorunda; dağıtım ek iş getirebilir; geliri dağıtmadan tutma yolu vardır. Bizim kategori + kapsam raporlaması bu disiplini dayatmıyor, karşılığında da o disiplinin verdiği durdurucu etkiyi vermiyor | Bütçe akışı |
| Credit ile normal gelir ayrımı | Henüz karar verme | Credit örneği bütün gelirlere genellenemez; kaynak ayrı dağıtılmadan gelir tutma yolunu açıklar. 20 Eylül K3'te From New Income / Fill Each Envelope sonucu ölçüldü; Keep Available sonucu ve hesap bakiyesinin değişmeme nedeni bilinmiyor | Gelir ve rapor |
| Initial Fill ile rapordaki 2.050 eşleşmesi | Henüz karar verme | Fonlama seçimi bilinmiyor; her iç dağıtım gelir sayılır hükmü kaldırıldı. Satır görünürlüğü arayüz referansıdır | Rapor ve liste |
| Raporun varsayılan olarak cari takvim ayını göstermesi | Alma | **Kazandırdığı:** uygulamayı açan kullanıcı "şu an neredeyim" sorusunun cevabını hiçbir seçim yapmadan görüyor; en sık sorulan soru varsayılan hâline getirilmiş. **Kaybettirdiği:** veri başka bir aydaysa ekran boş görünüyor (`No transactions found.`) ve kullanıcı raporun çalışmadığını sanabiliyor; ay değişiminde önceki ayın kapanışı da kendiliğinden gözden kayboluyor | Rapor dönem seçimi |
| Hesap (gerçek para) ile zarf (bütçe) katmanının ayrılması, `ACCOUNTS`'ın opsiyonel olması | Henüz karar verme | **Kazandırdığı:** yalnız bütçelemek isteyen kullanıcı hesap kavramını hiç öğrenmeden ürünü kullanabiliyor; ürün kendini kademeli açıyor. **Kaybettirdiği:** iki ayrı "toplam" doğuyor (`ENVELOPES` sekmesindeki `Total` ile `ACCOUNTS` sekmesindeki `All Accounts`) ve hangisinin gerçek para olduğu hiçbir yerde yazmıyor. Bizde hesap zorunlu ve merkezî; bu ayrımı almak büyük bir yeniden tasarım olur | Hesap modeli |
| İlk tekrar kaydı ve sonraki fark | Henüz karar verme | İlk kayıt görünür; sonraki 600 farkının nedeni bilinmiyor. Kesin otomasyon kusuru gerekçesi yapılmaz | Planlama |
| K08 sonrası 16 farkı | Henüz karar verme | Silinen satır yok; düzenlemenin kaydı ve farkın nedeni bilinmiyor. Genel silme hükmü kurulmaz | Düzeltme |
| `Account Transfer` birinci sınıf, ayrı bir işlem türü (Expense/Credit değil) | Uyarlayarak al | Bizim `Transfer` modelimize kavramsal olarak yakın; `From`/`To`/`Amount`/`Description`/`Date`/`Notes` alan seti ve açıklamanın otomatik dolması referans alınabilir | Transfer formu |
| İncelenen formda fiş eki bulunmaması | Henüz karar verme | Form gözlemi korunur; ürün genelinde yokluk kanıtlanmadı | İşlem formu |
| Konuma göre payee önerisi (`Save Location?`) | Henüz karar verme | Giriş hızını artıran gerçek bir fikir, ama konum izni istemek bizim gizlilik duruşumuz için ayrı bir karar; veri yetmiyor | İşlem ekle formu |
| Oyunlaştırılmış başarı mikro-metinleri (`Good job, Goodbudgeter!`) | Henüz karar verme | Ton/marka kararı; işletme kullanıcısına ne kadar uyacağı tartışmalı | Genel UX tonu |
| 1-hesap / 10-zarf ücretsiz sınırı ve paywall diyalogları | Alma | **Kazandırdığı:** ürün gerçekten denenebiliyor ve sınır açıkça sayıyla söyleniyor (`8 of 10 free Envelopes left`), sürpriz yok. **Kaybettirdiği:** sınır ürünün kendi tezini kesiyor — tek hesapla transfer de kart da kurulamıyor, yani ücretsiz sürümde bazı özellikler görülemiyor bile. Fiyatlandırma bizim kapsamımız dışında | — |
| ABD tarih/sayı biçiminin Türkçe sistemde de sabit kalması (`09/12/2026`, `25,000.00`) | Alma | **Kazandırdığı:** tek biçim, yerelleştirme hatası riski yok. **Kaybettirdiği:** Türkçe cihazda `08/10` okuyan kullanıcı 8 Ekim mi 10 Ağustos mu olduğunu ayırt edemiyor; rapor başlıkları Türkçe ay kısaltması kullanırken (`1 Ağu 2026`) form ABD biçiminde kalıyor ve ikisi aynı üründe çelişiyor | Tarih/para biçimlendirme |

## Kanıt ve güven düzeyi

- **Manuel gözlem (11 Eyl 2026):** K00–K08 + A/B/B1/B2 ek koşumu, canlı
  emülatörde gerçek household hesabıyla. Kareler E0106–E0129 (02b/02c dahil)
- **Manuel gözlem (12 Eyl 2026, Faz 7.5 doğrulama turu):** eski notlardaki 22/24
  sayıları tutarsızdır; güncel G01 kapsamı 28/28. işlem listesinin tamamı, güncel hesap bakiyesi, `Bulut` araması ve
  tekrarlama sıklığı listesi ölçüldü; kontrol değeri tablosu bu karelerden
  kuruldu. Kareler E0130–E0133
- **Resmî kaynak:** aşağıdaki 14 Eylül kaynak kontrolü; yeni canlı test değildir
- **Çıkarım:** `−22.950` toplamının `1.200 + 850 − 25.000` olduğu, karedeki
  sayılardan aritmetikle türetildi; uygulama bu formülü hiçbir yerde yazmıyor
- **Çıkarım:** `Available` başlığı altındaki iki `[Available]` satırının
  aylık ve aylık olmayan zarf gruplarına karşılık geldiği — arayüzde
  yazmıyor, doğrulanmadı
- **Doğrulanamadı:** 12 Eyl'de ölçülen ₺600'lük bakiye düşüşünün **nedeni**
  (tutar abonelik tutarıyla aynı ama uygulama
  bunu hiçbir yerde göstermiyor); K08 sonrasındaki ₺16 farkının nedeni
  (yalnız net dış etki ölçüldü); B1'in gelecek örneklerinin nerede
  yürütüldüğü; `Settings` ekranının tamamı (hiç açılmadı); A, K05 ve K06
  (kart/transfer davranışları) paywall nedeniyle
- **Kapsam dışı (protokol):** export/yedek akışları

## GB-U01 — Normal gelir ekranı, kullanıcı kontrolü (14 Eylül 2026)

Kullanıcı Fill Envelopes girişini açıp iki fonlama yolunu ve dağıtım seçimini
gösterdi. Bu sürümün etiketi **Keep Available**; resmî Android rehberindeki
Keep Unallocated adıyla aynı yazı değil. Kaynak ve gözlenen arayüz adı ayrı tutulur.

| Kanıt | Doğrudan görülen | Rapor kullanımı |
|---|---|---|
| GB-U01-A: `27-fill-from-new-income.png` | From New Income; Received from, How to fill Envelopes, Amount, Account, Date, Schedule ve zarflar | Belge 1 form düzeni; Belge 2 ayrı gelir girişi |
| GB-U01-B: `28-fill-from-available.png` | From Available; Currently Available 20.000, Used this Fill 0, Left Available 20.000; zarflar No change | Belge 2 mevcut paradan dağıtım yüzeyi; sağ altta ekran aracı kısmi örtüşüyor |
| GB-U01-C: `29-income-keep-available.png` | How do you want to fund your Envelopes? diyaloğu; Fill Each Envelope ve Keep Available; ikincisi seçili | Belge 1 seçim diyaloğu; Belge 2 dağıtmadan tutma seçeneği |

**Sonuç:** Normal gelir için Expense/Credit'ten ayrı yol ve geliri dağıtmadan
tutma seçeneği güncel arayüzde görüldü. B05'in gelir zorunlu harcama zarfına
bağlanır genellemesi geçersizdir. Yeni işlem kaydı veya kayıt sonrası
hesap/rapor testi yapılmış sayılmaz. Eski Initial Fill fonlama seçimi,
16 ve 600 farklarının nedeni bilinmiyor; bu sınırlar kapanışı engellemez.
GB-U01 tamam; Goodbudget kapanışı için başka kullanıcı kontrolü yok.

## 20 Eylül 2026 — Belge 2 koşumu (K3): normal gelir yolunun sonucu (GB-Q01 kapandı)

GB-U01 (14 Eylül) formu kareye almıştı ama **kaydın sonucu ölçülmemişti**. Bu koşumda yol uçtan uca
işletildi. Koşum listesi `raporlar/belge2-kosum-listesi.md`.

**Eklenen tek kayıt:** `Beta Tasarim`, ₺1.234, 20.09.2026, hesap `Ana Hesap`, `Fill Each Envelope`
ile tamamı `Tasarim Yazilimi` zarfına. Tuhaf tutar izlenebilirlik için seçildi. **Kayıt silinmedi.**

### Yol ve seçenekler

`Fill Envelopes` düğmesi Envelopes başlığındadır (content-desc `Fill Envelopes`); alt `+` düğmesi
onu değil **Add Transaction** formunu açar. Ekranda iki sekme var: `FROM NEW INCOME` ve
`FROM AVAİLABLE`. `How to fill Envelopes` iki kip sunuyor: `Fill Each Envelope`, `Keep Available`.
Zarf başına `No change` dışında dört doldurma kipi vardır: `Add Budget Amt (1.200,00)`, `Set to Budget Amt (1.200,00)`,
`Add Specific Amount`, `Set to Specific Amount` (`37-zarf-doldurma-secenekleri.png`). Tutar tam
dağıtılmazsa formda `Sweep the remaining X into [Available]` satırı çıkıyor; tam dağıtılınca
kayboluyor (`37-zarf-doldurma-secenekleri.png`, `32-gelir-formu-dolu.png`).

### Sonuç — üç gösterge ayrı ayrı

| Gösterge | Önce | Sonra | Fark | Kanıt |
|---|---:|---:|---:|---|
| Zarf toplamı (`Total`) | 43.784,00 | 45.018,00 | **+1.234** | `30-oncesi-zarflar.png`, `33-sonrasi-zarflar.png` |
| `Monthly` | 23.784,00 | 25.018,00 | +1.234 | `30-oncesi-zarflar.png`, `33-sonrasi-zarflar.png` |
| `Tasarim Yazilimi` zarfı | 0,00 | 1.234,00 | +1.234 | `30-oncesi-zarflar.png`, `33-sonrasi-zarflar.png` |
| `Available` | 20.000,00 | 20.000,00 | 0 | `30-oncesi-zarflar.png`, `33-sonrasi-zarflar.png` |
| **Hesap (`All Accounts` / `Ana Hesap`)** | **41.734,00** | **41.734,00** | **0** | `31-oncesi-hesap-bakiyesi.png`, `34-sonrasi-hesap-bakiyesi-degismedi.png` |
| Eylül `Income vs Spending` → Income | 2.050 *(önceki koşumun E0121/E0124 raporları; bu turda ayrı önce karesi yok)* | **3.284** | +1.234 *(önceki koşumla karşılaştırma)* | `36-rapor-income-vs-spending-3284.png`; `35-sonrasi-islem-satiri.png` iki gelir satırını gösterir |

**Cevaplar:**

- **Hesap bakiyesini artırıyor mu? Hayır.** `All Accounts` 41.734,00'de kaldı.
- **Zarf kalanını artırıyor mu? Evet.** Seçilen zarf 0,00 → 1.234,00.
- **Raporda gelir sayılıyor mu? Evet.** Eylül Income 2.050 → 3.284, Spending 0, Net Total 3.284.

**İşlem kaydı oluştu ve hesaba atfedilmiş görünüyor:** `09/20 Beta Tasarim +1.234,00 Ana Hesap`
(`35-sonrasi-islem-satiri.png`). Kayıt hesabı gösterdiği hâlde hesap bakiyesi değişmedi.

*Yenilenme kontrolü yapıldı.* Sekmeler arasında iki kez gidilip gelindi, ardından uygulama
`force-stop` ile kapatılıp yeniden açıldı; `All Accounts` yine 41.734,00 (`34-sonrasi-hesap-bakiyesi-degismedi.png`).
Yeniden açma adımı **koşum kaydıdır**; kare son bakiyeyi gösterir, kapatılma adımını değil.
Bu kontrolden sonra değer değişmedi; bütün gecikme veya senkronizasyon olasılıkları elenmiş değildir.
**Nedeni bilinmiyor**; senkronizasyon, ücretsiz paket sınırı
veya tasarım tercihi olup olmadığı bu koşumdan çıkarılamaz ve ürün kusuru diye yazılmaz.

**İki katman arasındaki fark büyüdü:** zarf toplamı − hesap = önce 2.050, sonra **3.284**. Önceki
2.050 farkın karşılığı `09/11 Initial Envelope Fill +2.050,00` satırıdır ve o satırın hesap atfı
yoktur; yeni kayıt ise hesap atfı taşıdığı hâlde aynı farkı kendi tutarı kadar büyütmüştür.

### Kontrol değeri güncellemesi

Hesap kontrol değeri **41.734,00 değişmedi**. Zarf toplamı kontrol değeri **43.784,00 → 45.018,00**
oldu. Sonraki koşumlar bu yeni değerden başlar.

### Bu koşumda kapanmayanlar

- `Keep Available` kipi ve `FROM AVAİLABLE` sekmesi bu koşumda işletilmedi (yalnız 14 Eylül'deki
  form kareleri var).
- Hesap bakiyesinin neden değişmediği açıklanmadı.
- Kart ve transfer yüzeyleri ücretli paket nedeniyle hâlâ kapalı (B07).

## Resmî kaynak kontrolü — 14 Eylül 2026

- [GB-S01 — Step 3. Add Your Income](https://goodbudget.com/help/getting-started-guide/step-3-add-income/): Android gelir yolu Fill Envelopes üzerinden açılır; Keep Unallocated geliri dağıtmadan tutar. Sayfa etiketi 21 Şubat 2020; erişim 14 Eylül 2026. Bu, kaynak açıklamasıdır; sürümde yeni canlı doğrulama değildir.
- [GB-S02 — Getting Started with Accounts](https://goodbudget.com/help/using-accounts/getting-started-with-accounts/): ücretsiz planda bir ana hesap ve sınırsız bütçe dışı Debt hesabı açıklanır. Sayfa etiketi 1 Şubat 2020; erişim 14 Eylül 2026.
- [GB-S03 — Can I Link my Bank Accounts?](https://goodbudget.com/help/using-accounts/link-bank-accounts/): Premium banka senkronizasyonu sunar. Sayfa etiketi 5 Şubat 2020; erişim 14 Eylül 2026. Bölge/kurum uygunluğu ve canlı bağlantı denenmedi.

## Tek cümlelik sonuç

Goodbudget zarf bütçelemesi, dört üst sekme, işlem listesi ve ayrı hesap/transfer
yüzeyleriyle arayüz referansıdır. Credit sonuçları normal gelire genellenmez;
açıklanamayan bakiye farkları kesin silme/tekrar kusuru değildir.

## 23 Eylül 2026 — ek eksik koşumu (22 Eylül), kullanıcı kontrolü ve kare doğrulaması

Koşum listesi `raporlar/eksik-kosum-ortak-listesi.md` (GB-01…GB-07). Goodbudget'a test
kaydı yazılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| Silinen kaydın geri alınması (GB-07) | Ayarlar'ın dört bölümü tarandı (Household · Device · Localization · Advanced · About); çöp kutusu veya geri alma kalemi yok. Alt yarıda Close Household "Permanently delete your login and data" | `42-ayarlar-alt.png` (E0453); üst yarı `41` hane adı taşıdığı için envantere girmedi |
| Hesap makinesi (GB-04) | Advanced › Calculator "Use calculator to enter amounts" bir ayar ve açık. **Kullanıcı kontrolü (23 Eyl):** Add Transaction'da Amount alanına dokununca tutar girmek için basit bir hesap makinesi açılıyor, yalnız dört işlem. Kayıttan sonraki tebrik mesajı gözlenmedi | E0453; kullanıcı kontrolü |
| Harcama raporundaki gelir (GB-02) | Koşum Income 3.284 = 1.234 + 2.050 Initial Envelope Fill açıklamasını verdi; bu 20 Eylül K3'te (E0423) zaten kayıtlı. Harcama raporunun toplamında 25.000'lik gelirin durması (E0123) **cevapsız kalıyor** | E0423, E0123 |

## 24 Eylül 2026 — Belge 1 düzeltme turu: kare doğrulaması

Belge 1'e dayanak yapılmadan önce kareler tek tek açıldı. Aşağıdakiler ya ortak listedeki koşum
özetini kareye göre düzeltir ya da Belge 1'in kullanmadığı bir karede görülen arayüz ayrıntısıdır.
Yeni koşum yapılmadı.

| Soru | Gözlem | Kanıt |
|---|---|---|
| İlk açılış sırası | İlk ekran LOG IN / CREATE NEW HOUSEHOLD. Yeni haneden sonra **Setup Budget** (zarf ekleme, "8 of 10 free Envelopes left" — sayaç grup başına, tahmini aylık gelir ve kalan); e-posta ve parolalı **Register Household** zarflar doldurulduktan sonra geliyor ("Envelopes Filled! Register to see your budget on the web…") ve **LATER** taşıyor (atlama denenmedi) | E0106, E0109, E0110 |
| Sayı ve tarih biçimi | Settings › Localization: Date Order ("month first or day first") · Decimal Digits · Decimal Mark ("Only affects this device") | E0453, E0495 |
| Gelir rengi (GB-01) | Gelir satırı yeşil ve + önekli, gider düz koyu ve işaretsiz; tutarlar ABD biçiminde (1,234.00) | E0494 |
| Arama ve form menüsü | Transaction Search var. Add Transaction formunun menüsünde yalnız Help; formda isteğe bağlı Check # alanı | E0127, E0128 |
