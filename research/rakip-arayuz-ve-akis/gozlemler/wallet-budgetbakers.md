# Uygulama Gözlem Formu — Wallet by BudgetBakers

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Wallet: Budget Expense Tracker / BudgetBakers |
| Sürüm | 9.3.6 (`versionCode=90164`) |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | İngilizce / TRY |
| Hesap veya plan türü | Ücretsiz, daha önce deneme için açılmış BudgetBakers hesabı (`My Wallet`) |
| Erişim kısıtı | Asıl manuel kayıt ve rapor akışlarında yok; banka senkronizasyonu ve bazı özellikler Premium tanıtımı taşıyor |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Kısmi | — | Önceden deneme için açılmış hesap doğrudan kullanılabildi; sentetik kayıtlar buluta senkronize hesaba eklendi | Kayıt/onboarding bu turda görülmedi; uygulama kullanıcı adı ve `My Wallet` çalışma alanını gösteriyor. Play Store test sırasında yüklü 9.3.6 için güncelleme bulunduğunu gösterdi | `00-magaza.png`, `02b-menu.png` |
| K01 Ana ekran | Tamamlandı | 0 | Hesaplar yatay kartlarda ve bakiyeleriyle ilk blokta; `Accounts` ile `Budgets & Goals` aynı ana yüzeyde | Hesapların altında büyük Premium, banka bağlantısı ve çapraz ürün tanıtımları asıl içeriği aşağı itiyor | `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı | ~6/hesap | Manual Input altında Cash, Checking account, Credit card, Savings, Loan, Mortgage ve overdraft dahil açık hesap türleri var; kartta limit, son ödeme tarihi, gösterilecek bakiye ve başlangıç değeri alanları bulunuyor | Cash/checking oluştururken açılış bakiyesi alanı yok; bu iki hesap protokol gereği sıfır açıldı. Checking hesabı kaydedilince veri içe aktarmayı etkinleştirmek için hesaba e-posta gönderildi | `03-dolu-ana-ekran.png` |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~12 | Hızlı form tutar + hesap + kategoriyle çok kısa; ayrıntıda tarih, not, payer, label ve ödeme türü eklenebiliyor | Form varsayılan olarak **Expense** açılıyor; Income seçimi yalnız sekme rengiyle anlaşılıyor. Hizmet geliri için işletme sınıfı yok, en yakın kategori `Sale`; ayrıntılar kayıt sonrası ikinci ekranda tamamlandı | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~10 | `Groceries` gibi iki katmanlı kategori ve notta serbest metin var; arama not içindeki `Market` sözcüğünü buldu | **İşletme/şahsi kapsam alanı yok.** Label veya kategoriyle taklit edilmedi; protokole göre bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~10 | Gider kart hesabına yazıldığı anda kart bakiyesi `-TRY 1,200`; Spending raporunda aynı gün gider oldu. Kart hesabı ayrıca limit ve vade alanı taşıyor | İşletme kapsamı yok; tedarikçi yalnız `Payee`, açıklama ise `Note` alanında tutuluyor | `05-siniflandirma.png`, `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı Transfer sekmesi var; From/To açıkça seçiliyor. Ana Hesap→Ortak Cüzdan ve Ana Hesap→İş Kartı iki bağlı satır (+/−) olarak görünse de toplam ve cash-flow etkisi sıfır | Liste aynı transferi iki satırda gösterdiği için ilk bakışta çift kayıt izlenimi verebiliyor; form yeni açıldığında son kullanılan yönü korumadı ve hesaplar yeniden seçildi | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~4 | Liste haftalara ayrılıyor; arama not metnini buluyor. Cash-flow tam `Income TRY 25,000`, `Expenses -TRY 2,050`, net `TRY 22,950` gösterdi; transferler ve kart ödemesi yeniden gelir/gider sayılmadı. Spending kategori dağılımı ve en büyük giderleri gösteriyor | Business/personal kırılımı yok; Records menüsündeki seçenekler yalnız “her kayıtta bakiye” ve “planlananları göster”, gelişmiş filtre görülmedi | `06-islem-listesi.png`, `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~3 | Satır → Record detail ile alanlar doğrudan değiştiriliyor; `Split` eylemi de aynı üst çubukta | Düzenleme geçmişi korumadan mevcut kaydı değiştiriyor. Delete tek Yes/No onayından sonra kalıcı; void/iptal veya geri alma yok | Record detail manuel gözlemi |

## Kontrol değeri doğrulaması

Wallet cash/checking hesaplarında açılış bakiyesi kabul etmediği için kontrol,
yalnız çekirdek işlemlerin ürettiği değerlere uyarlandı. Bu fark ürün davranışı
olarak kaydedildi; gelir işlemiyle yapay açılış bakiyesi oluşturulmadı.

| Değer | İşlem-only beklenen | Wallet | Durum |
|---|---:|---:|---|
| Ana Hesap bakiyesi | 20.800,00 | TRY 20,800.00 | ✓ |
| Ortak Cüzdan bakiyesi | 2.150,00 | TRY 2,150.00 | ✓ |
| İş Kartı bakiyesi/borcu | 0,00 | TRY 0 | ✓ |
| Toplam net bakiye | 22.950,00 | TRY 22,950.00 | ✓ |
| Gelir toplamı | 25.000,00 | TRY 25,000.00 | ✓ |
| Gider toplamı | 2.050,00 | TRY 2,050.00 | ✓ |

## Arayüz taraması (görev dışı, ~10 dk)

| Alan | Gezildi mi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler / alt görünümler | ✓ | Yan menü çok geniş: Home, Records, Investments, Statistics, Planned payments, Budgets, Debts, Goals, Shopping lists, Warranties, Loyalty cards, Currency rates, Group sharing ve Others. Statistics içinde Balance, Outlook, Cash-flow, Spending, Credit, Reports ve Assets sekmeleri var | `02b-menu.png` |
| Bir raporun içine tıklama | Kısmen | Cash-flow ve Spending açıldı; Spending kategori/label geçişi, trend ve Top 5 expenses taşıyor. `Go deeper` ayrıntısı açılmadı | `05-siniflandirma.png`, `07-rapor.png` |
| Bütçe / hedef / planlama ekranı | ✓ (boş durum) | Planned payments gelecekteki tahsilat/gideri anlatıp `+` yönlendirmesi yapıyor; Budgets doğrudan `Create Budget`; Debts Active/Closed, Goals Active/Paused/Reached sekmeli | `09-ozgun-ozellik.png` |
| Ayarların derinliği | Kısmen | Menü seviyesinde Dark mode, Hide Amounts, Currency rates, Group sharing, Help ve Settings görüldü; ayarların içine girilmedi | — |
| Arama ve filtre davranışı | ✓ | Serbest metin araması not içindeki `Market` ifadesiyle tek kaydı buldu. Records seçeneklerinde gelişmiş filtre görülmedi | — |
| Boş durum ekranları | ✓ | Planned payments, Budgets, Debts ve Goals boş durumları ne işe yaradığını açıklıyor ve ilk eylemi söylüyor; Money Manager'daki yalnız “No data” yaklaşımından daha yönlendirici | `09-ozgun-ozellik.png` |
| Hata / uç durum | ✓ | Sıfır tutarda kayıt engellendi ve alt toast/snackbar ile “Please fill in the amount.” dendi; alan içi hata yok. İlk yanlış tipte girilen sentetik kayıt Delete→Yes ile tamamen silindi | `08-hata-veya-bos-durum.png` |
| Widget / hızlı giriş / kısayol | Kısmen | Her ana ekranda bağlama göre mavi `+` var; cihaz widget'ı denenmedi | — |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Home hesap bakiyelerini önce gösteriyor; raporlar soru başlığı (“Am I spending less than I make?”, “Where does my money go?”) + toplam + kırılım düzeninde |
| Alt/üst gezinme | Kalıcı alt bar yok; çok geniş hamburger menüsü ve ekran içi yatay sekmeler kullanılıyor. İşlev sayısı arttıkça keşfedilebilirlik menüye yüklenmiş |
| Renklerin anlamı ve tutarlılığı | Hesap rengi hızlı işlem formunun tüm zeminini değiştiriyor. Seçili Income/Expense/Transfer sekmesi büyük ölçüde yalnız arka plan tonuyla belirtiliyor; yanlış seçim riski var |
| Tipografi ve para değerlerinin okunması | Para değerleri büyük ve sağa hizalı; negatifler `-TRY` ile açık. Uzun açıklamalar ve hesap adları bazı alanlarda kırpılıyor |
| Kart, liste ve grafik kullanımı | Home yatay hesap kartları; Records hafta gruplu liste; Cash-flow/Spending büyük özet + trend/grafik + en büyük kayıtlar düzeni |
| Form alanları ve varsayılanlar | Hızlı form yalnız tutar/hesap/kategori; ayrıntılar ikinci ekranda. Varsayılan Expense, son kullanılan hesap/kategori hatırlanıyor; transfer yönü güvenilir biçimde hatırlanmadı |
| Loading, boş, hata ve başarı geri bildirimi | Boş durumlar yönlendirici; sıfır tutar toast/snackbar ile engelleniyor. Başarı sessizce listeye dönüyor. Silme onaylı ama geri alınamaz |
| Erişilebilirlik / dokunma alanları / metin yoğunluğu | Dokunma alanları büyük. Sekme seçiminin renk ağırlıklı olması ve tek hamburger menüsündeki çok sayıda hedef bilişsel yük oluşturuyor; İngilizce arayüz kullanıldı |

## Akış özeti

- En kısa ve güçlü akış: Hızlı işlem girişi — tür → tutar → hesap → kategori → Save.
- En fazla sürtünme yaratan akış: İşletme geliri — varsayılan Expense'ten Income'a geçmek, uygun olmayan kişisel kategori ağacından `Sale` seçmek ve tarih/not/payer için kaydı tekrar açmak.
- Uygulamanın hedef kullanıcı varsayımı: Birden çok hesabı, hedefi ve planı olan kişisel/ortak hane finansı kullanıcısı; ayrıca Premium banka senkronizasyonu müşterisi.
- İşletme ve şahsi para yaklaşımı: **Yok.** Label var ama özel kapsam boyutu değil; taklit edilmedi.
- Transfer ve kart ödemesi yaklaşımı: Transfer ayrı tür ve raporda nötr; kart ödemesi Ana Hesap→İş Kartı transferiyle kart bakiyesini kapattı, gideri ikinci kez saymadı.
- Planlama, borç ve tahsilat yaklaşımı: Planned payments, Debts, Goals ve Budgets ayrı yüzeyler; Tur 1'de yalnız boş durumları incelendi.

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| Rapor başlığını kullanıcı sorusu olarak kurma | Uyarlayarak al | “Nakit akışım ne durumda?” gibi başlıklar grafiğin neden var olduğunu açıklar; muhasebe terimini tek başına bırakmaz | Özet ve raporlar |
| Cash-flow kartında gelir, gider ve neti aynı blokta gösterme | Doğrudan al / zaten kısmen var | Üç rakamın ilişkisi tek bakışta anlaşılıyor; transferler nötr kalıyor | Aylık özet |
| Yönlendirici boş durum + tek CTA | Doğrudan al | “Veri yok” yerine özelliği ve ilk adımı anlatıyor; proje empty-state kuralıyla uyumlu | Tüm boş listeler |
| Hızlı form ile ayrıntı formunu iki katmana ayırma | Uyarlayarak al | Sık giriş hızlanır; fakat kapsam/KDV/indirilebilirlik gibi görünür onay gerektiren alanlar gizlenmemeli | İşlem ekleme |
| Seçili işlem türünü yalnız renkle belirtme ve Expense'i varsayma | Alma | Gelirin gider olarak kaydedilmesi kolay; ikon/metin/işaretle çoklu seçililik göstergesi gerekir | İşlem türü seçimi |
| Geniş hamburger menüsünde bütün ürün yüzeylerini düz listeleme | Alma | İşlev sayısını gösteriyor ama çekirdek akışları gömüyor; BusinessFinance'te ana işler alt gezinmede kalmalı | Ana gezinme |
| Transferi iki bağlı satır gösterip toplamı nötr tutma | Uyarlayarak al | Kaynak ve hedef etkisi izlenebilir; satırların bağlı olduğu daha açık anlatılmalı | Aktivite feed'i ve transfer detayı |
| İşletme/şahsi boyutunun bulunmaması | Alma | ADR 0013'ün temel farklılaşmasını karşılamıyor | İşlem formu ve rapor filtresi |
| Doğrudan düzenleme + kalıcı silme | Alma | Finansal geçmişi değiştiriyor; BusinessFinance sil yerine iptal ve düzeltme kaydı kullanır | Kayıt detayı |
| Nakit/banka hesabında açılış bakiyesi alanı olmaması | Alma | Kullanıcıyı sahte gelir girmeye itebilir; bizde açılış bakiyesi raporu kirletmeden ayrı tutuluyor | Hesap oluşturma |

## Kanıt ve güven düzeyi

- Manuel gözlem: K01–K08 emülatörde sentetik veriyle tamamlandı; işlem-only kontrol değerleri birebir tuttu.
- Resmî kaynak: —
- Yorum: Hızlı formun varsayılan Expense seçimi ve renk ağırlıklı seçili durumu yanlış kayıt riskini artırır; bu turda ilk girişte gerçekten yanlış tür seçildi ve sonra silindi.
- Doğrulanamadı: Yeni hesap kayıt/onboarding'i, Premium özelliklerin asıl davranışı, banka bağlantısı, dışa aktarma, widget ve Tur 2 plan/borç derin akışları.

## Tek cümlelik sonuç

Wallet güçlü rapor soruları, yönlendirici boş durumları ve nötr transfer modeliyle iyi bir finansal UX referansı; fakat varsayılan Expense seçimi, kapsam boyutunun yokluğu ve aşırı geniş hamburger menüsü BusinessFinance için açık negatif örnekler.
