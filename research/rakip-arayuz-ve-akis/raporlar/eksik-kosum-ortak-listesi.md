# Ortak eksik koşum listesi — Belge 1 + Belge 2

Belge 1'in 13 bölümü ve Belge 2'nin 10 bölümü altındaki `eksik-listesi.md`
dosyalarının **tek listede birleştirilmiş** hâli. Amaç: aynı uygulamaya iki ayrı
belge için iki kez girmemek. Bir uygulama bir kez açılır, o uygulamanın bütün
açık eksikleri aynı oturumda kapanır.

Kaynak dosyalara dokunulmadı; sonuçlar bu dosyada toplanır, bölüm listelerine
devri belge sahibi yapar.

## Koşum kuralları (bu koşuma özel)

- **Yeni test kaydı yazılabilir** (22 Eylül 2026 kullanıcı kararı). Test bitince
  kayıt silinmez, kontrol değerine döndürülmez — cihazda olduğu gibi kalır.
- Kareler `kanitlar/<uygulama>/` altına, her uygulamanın **kendi numara
  serisinin devamıyla** yazılır (README → "Dosya adlandırma", `10+` ek koşum).
- Gözlem tarafsız yazılır: ne yapıyor · ne kazandırıyor · ne kaybettiriyor.
  Kendi kararlarımız ölçüt değildir.
- Kişisel veri taşıyan yeni kare basılacaksa `belge1/ortak/kalip.py` →
  `KARARTMA_ORTAK` güncellenir.

## Durum sözlüğü

`açık` henüz koşulmadı · `kapandı` kare/cevap alındı · `kapanmadı` koşuldu ama
ürün cevabı vermedi · `erişim yok` ücretli/bölgesel engel, koşulamaz.

---

## Money Manager

| ID | Eksik | Kaynak | Öncelik | Durum | Sonuç / kare |
|---|---|---|---|---|---|
| MM-01 | Hesap makinesi tuş takımının karesi | B1 5.4 | Yüksek | **kapandı** | `47`, `48` — form açılır açılmaz tutar tuş takımı altta duruyor; sağ alttaki simge **tam ekran hesap makinesine** çeviriyor (AC ÷ × − + = 00 , BİTTİ) · **24 Eyl doğrulama:** simge tuş takımının **sağ sütununda**, sağ alttaki düğme "Bitti" (E0463, E0464) |
| MM-02 | Pastadan kategori ayrıntısına geçiş karesi | B1 10.1 | Yüksek | **kapandı** | `52`, `53` — pasta dilimi/satır `StatsDetail` açıyor: kategori toplamı, **sekiz aylık çubuk eğilimi** (Şub–Eyl) ve o kategorinin kayıt listesi · **24 Eyl doğrulama:** eğilim **çizgi** grafik, çubuk değil (E0466) |
| MM-03 | Kasım ve Aralık kart defteri kareleri | B2 3.7 | Yüksek | **kapandı** | `63`, `64` — Kas: çekme 1.000, bakiye 3.600 · Ara: çekme 1.000, bakiye 4.600. Zincir Eki 2.600 → Oca 5.600 ile tam |
| MM-04 | Tekrarlayan kurulum sonrası onay diyaloğu; gerçekleşme onaylı mı | B2 5.2 | Yüksek | **kapandı** | `65`, `66`, `67`, `68`, `69` — kaydederken **tek bir onay**: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" (HAYIR/EVET). Gerçekleşme **onaysızdır**: `Tekrar ne zaman uygulanır?` ayarı iki değer taşır (`Tarihte` · `Her ayın ilk günü`), per-kayıt onay yok. Vadesi gelmemiş tekrarlar ay listesinde ayrı bir **Tekrarlama** bloğunda durur ve **ay toplamına girmez** |
| MM-05 | Bütçe bloğunun kurulumu ve ilerlemenin %0 görünme nedeni | B2 5.5 | Yüksek | **kapandı** | `70`, `71`, `72` — Toplam sekmesindeki bütçe bloğu `Bütçe Ayarları`na gidiyor; kurulu **tek bütçe `Yiyecek ₺1.400`**. %0 bunun sonucu: ayın gideri `Diğer` ve `Günlük Yaşam` kategorilerinde, bütçesi olan kategoride hiç harcama yok. Bütçe kategori başına kurulur, `Varsayılan bütçe` + ay ay override taşır; değişiklik **önümüzdeki aydan** geçerli |
| MM-06 | Filtre uygulandığında toplamların yeniden hesaplanması | B2 7.3 | Yüksek | **kapandı** | `54`, `55`, `56` — filtresiz gider 2.180; `Is Karti` seçilince panel başlığı **anında** 1.000'e düşüyor (%45) ve havale `Havale : ₺400,00` diye ayrı satırda kalıyor. `Filtre` düğmesi aynı toplamı listeye uyguluyor |
| MM-07 | Sıfır tutarlı kayıt kabul ediliyor mu | B2 1.5 | Yüksek | **kapandı** | `49`, `50` — **kabul ediliyor**: tutar boş bırakılıp kaydedilen kayıt listeye `₺0,00` olarak giriyor, uyarı yok. Kaydet önce eksik **hesap** alanına atlıyor; boş tutar eksik sayılmıyor |
| MM-08 | CalcBox ve PC'den Yönet içerikleri | B1 12.2 + B2 8.5 | Yüksek | **kapandı** | `61`, `62` — ikisi de uygulama içi modül değil. CalcBox **ayrı bir ürünün Play sayfasını** açıyor (Realbyte Inc.; cihaz uyumsuz). PC'den Yönet **ücretli sürüm ekranına** gidiyor (3. fayda, ayrıca yönlendirici gerekiyor) |
| MM-09 | Aktarımın ay raporu toplamlarına etkisi (önce/sonra) | B2 2.3 | Yüksek | **kapandı** | `57`, `58` — ₺300 Ana Hesap → Ortak Cuzdan yazıldı; ay toplamı **değişmedi** (gelir 0, gider 2.180). Havale formu ayrıca bir **Harç** alanı taşıyor. Filtre panelinde havale `Giden/Gelen Havale` diye kendi sütununda |
| MM-10 | Tutar düzenlemenin ay toplamlarına etkisi (önce/sonra) | B2 1.2 | Yüksek | **kapandı** | `50`, `51` — aynı kaydın tutarı 0 → 580 yapıldı; Eylül gideri 1.600 → **2.180**'e anında döndü, ara onay veya yeniden hesaplama adımı yok |
| MM-11 | Silme onayı ve sonucu; silinen kaydın geri alınabildiği bir yüzey | B1 5.8 + B2 1.5 | Orta | **kapandı** | `59`, `60` — onay: "Silmek istediğinize emin misiniz?" (HAYIR/EVET). **Geri alma yüzeyi yok**: `Daha` ızgarası Ayarlar · Hesaplar · Giriş Kodu · CalcBox · PC'den Yönet · Yedekle · İletişim · Yardım · Tavsiye et; çöp kutusu kalemi yok |
| MM-X1 | İlk açılışın karesi (karşılama yok iddiası kullanıcı beyanı) | B1 3.1 | Önerilmez | erişim yok | Temiz kurulum gerekir; mevcut veriyi bozar |

## Wallet by BudgetBakers

| ID | Eksik | Kaynak | Öncelik | Durum | Sonuç / kare |
|---|---|---|---|---|---|
| WL-01 | Bekleyen ödeme kartının (Upcoming planned payments) yüklenmiş hâli | B1 4.5 / 8.4 | Yüksek | **kapandı** | `f7-59` — kart yüklenmiş hâlde: "Bulut yazilim aboneligi · Software, apps, games · −₺600 · 10 Eki". Aynı kare **Balance Trend**'in yüklenmiş hâlini de taşıyor (TODAY ₺6.350,00), B1 3.4/E8 de kapanıyor |
| WL-02 | Hesap oluşturma formunda açılış bakiyesi alanının aranması | B1 6.2 + B2 2.1 | Yüksek | **kapandı** | `f7-62`, `f7-63`, `f7-64`, `f7-65` — dördüncü hesap **ücretsiz sürümde açılamıyor** (premium duvarı), o yüzden mevcut hesabın düzenleme formu tarandı: Account name · Bank account number · Currency · Color · Import email · Exclude from stats · Archive · Minimum/Maximum balance. **Açılış bakiyesi alanı yok.** `f7-64` **kişisel veri taşıyor** (import e-postası) — basılacaksa karartılmalı |
| WL-03 | Aktarım formunun kendisi | B2 2.2 | Yüksek | **kapandı** | `f7-66` — INCOME/EXPENSE/**TRANSFER** aynı formun üçüncü sekmesi; From account · To account · `Target amount ∼` · hesap makinesi tuş takımı |
| WL-04 | Aktarımın ay raporu toplamlarına etkisi | B2 2.3 | Yüksek | **kapandı** | `f7-68`, `f7-72` — ₺300 Ana Hesap → Is Karti yazıldı; hesap bakiyeleri değişti (9.800→9.500, −5.600→−5.300) ama dönem gider toplamı **₺21.600,00'de kaldı**. Takvim ayına çevrilmiş cash-flow da aktarımı saymıyor |
| WL-05 | Kart harcamasının dönem gider toplamına etkisi (Statistics/Spending) | B2 3.5 | Yüksek | **kapandı (çıkarım, 23 Eyl doğrulama)** | Kart harcaması dönem giderine **giriyor**, ama bir hesap filtresi karesiyle değil aritmetikle: `f7-76` (E0440) ₺21.600 = 10.000 Property insurance + **6.000 Electronics (Is Karti)** + 5.000 borç verme + 600 Software; kalemler `f7-86` (E0441) ve E0398'de. Koşum notundaki "yalnız Ana Hesap ₺15.600" diyen kare **bulunamadı**. `f7-82` (E0461) %2147483647'yi **göstermiyor** — görünen Credit Limits Utilization %0, limit 0; taşma iddiası kanıtsız, basılmaz |
| WL-06 | Debts → Closed sekmesinin içeriği | B2 4.1 | Yüksek | **kapandı** | `f7-83` — boş durum: "No closed debts" |
| WL-07 | Labels ile kapsam ayrımı denemesi ve Statistics/Labels okuması | B2 6.2 | Yüksek | **kapandı** | `f7-77`, `f7-79`, `f7-80` — iki kayıt `Isletme` ve `Sahsi` etiketiyle yazıldı. Labels görünümü **yalnız etiketli parayı** topluyor: ayın ₺21.750 giderinden **₺150** görünüyor, etiketsiz geri kalan bu görünümde hiç yok. Etiket formunda `Auto assign to new records` anahtarı var · **24 Eyl:** `f7-79` (etiket oluşturma formu: Name, Color, Auto assign to new records kapalı) E0497 olarak envantere girdi; Belge 1 6.2 |
| WL-08 | Aralık takvim ayına çevrildiğinde cash-flow toplamları | B2 7.2 | Yüksek | **kapandı** | `f7-69`, `f7-70`, `f7-71`, `f7-72` — dönem seçici üç sayfa: göreli çipler (7D/30D/12W, **6M ve 1Y kilitli**), adlandırılmış dönem (Today/This week/This month/This year) ve özel tarih aralığı. Takvim ayına çevrilince: Cash Flow −₺16.600,00 · Income ₺5.000,00 · Expenses −₺21.600,00 · **24 Eyl doğrulama:** adlandırılmış dönem listesinde **This year da kilitli** (E0475) |
| WL-09 | Dışa aktarma yüzeyinin aranması | B2 8.1 | Yüksek | **kapandı** | `f7-95`, `f7-97`, `f7-104`, `f7-105` — Ayarlar'da ve Records taşma menüsünde **yok**; çekmecedeki **katlanmış `Others`** bölümünde: Imports · Exports · Locations. Form: Account · Type · Payment Type · From/To tarih · `Include account transfers` · **PDF · XLS · CSV** |
| WL-10 | Shopping lists, Warranties, Loyalty cards içeriği | B1 12.2 | Yüksek | **kapandı** | `f7-89`, `f7-91`, `f7-93` — Shopping lists'te bir liste (`Elbise`, ₺0, 0/0 items, `Share list`); Warranties ve Loyalty cards **boş**, ikisi de "Tap the plus button to add the first one" |
| WL-11 | Sıfır tutarlı kayıt kabul ediliyor mu | B2 1.5 | Yüksek | **kapanmadı (23 Eyl doğrulama)** | `f7-67` (E0460) formu 0 TRY'de gösteriyor; kayıt oluşmadı. Koşumun "hata mesajı yok" sonucu **kareyle kanıtlanmıyor**: E0281'de aynı durumda "Please fill in the amount." baloncuğu görünüyor ve baloncuk kısa süre kalıyor. Belge 2 1.5'teki ifade (E0281) doğru kalır. Sıfırın reddedildiği zaten E0281 ile kayıtlıydı |
| WL-12 | Debt action listesinin diğer değerleri (borcu artırma kolu) | B1 9.3 + B2 4.3b | Orta | **kapandı** | `f7-88` — liste tam olarak iki değer: **Repay debt** ve **Increase debt** |
| WL-13 | "Yes, select record" — mevcut kayda bağlama kolu | B2 4.2 | Orta | **kapandı** | `f7-85`, `f7-86` — borca kayıt eklerken iki kol: `Select Record` ("to choose an existing Record to bind to the Debt") ve `Create new Record` ("to repay or increase the Debt manually"). İlk kol bütün mevcut kayıtları listeliyor, etiketleriyle |
| WL-14 | Borcun kayıt listesi (D3 sonrası) | B1 9.4 | Orta | **kapandı** | `f7-84` — `Debt Records`: Total ₺5.000,00 + bağlı kayıt satırı (`Lending, renting · Ana Hesap · 11 Eyl`), altında `Add Record` ve `Manage debt` · **24 Eyl doğrulama:** `f7-84`'te (E0478) Manage debt **yok**; altta yalnız Add Record |
| WL-15 | Settings › Filters ile filtre kurulumu | B1 10.4 | Orta | **kapandı** | `f7-73`, `f7-74`, `f7-75` — filtre listesi boş; form: Name · Type · Record confirmation · Categories · **Labels** · Currencies · Payment Type · Status · **Transfers (Include/Exclude)** · **Debts (Include/Exclude)** |
| WL-16 | Kısmi kart ödemesi ve sonrasındaki dönem raporu | B2 3.6 | Orta | **kapandı** | `f7-66`, `f7-68` — Wallet'ta kart ödemesi ayrı bir kavram değil, **hesaptan karta aktarım**. ₺300 kısmi ödeme kart borcunu −5.600'den −5.300'e çekti, dönem gider toplamına **dokunmadı** |
| WL-17 | Silme onayı ve silinen kaydın geri alınabildiği yüzey | B1 5.8 + B2 1.5 | Orta | **kapandı** | `f7-102`, `f7-103` — onay: "Do you really want to delete this item?" (No/Yes). **Geri alma yüzeyi yok**: çekmecede ve Ayarlar'da çöp kutusu kalemi yok. Kayıt ayrıntısının araç çubuğunda ayrıca bir **Split** eylemi var |
| WL-18 | Payment Due Date alanının işlevi | B1 6.3 | Düşük | **kapandı** | `f7-98`, `f7-99` — kart hesabının düzenleme formunda; **ayın günü seçici** (1–31), varsayılan `Not set`. Aynı formda `Credit card / Overdraft limit = 0` ve `Balance Display Options = Available Credit` |
| WL-19 | İki tutar yazımının nedeni (sayı biçimi ayarı) | B1 4.1 | Düşük | **kapandı** | `f7-96`, `f7-106`, `f7-107` — `Ayarlar › Advanced settings › Number format` tek bir anahtar: "Use decimals within amounts." Kapatılınca bütün tutarlar ondalıksız yazılıyor; **değişiklik ancak uygulama yeniden başlatılınca** uygulanıyor. Koşumda kapatıldı, kare alındı, **tekrar açıldı** (mevcut kanıt serisiyle biçim tutarlı kalsın diye) · **24 Eyl doğrulama (Belge 1):** ayar yalnız ondalığı açıp kapatıyor (E0482, E0485); Belge 1 4.1'in sorduğu para kodu ve ayraç farkını açıklamıyor. Belge 1 için **kısmen** |
| WL-20 | Hedef tutarı 20.000'in karesi | B1 8.6 | Düşük | **kapandı** | `f7-60` — `Yeni ekipman fonu`, No target date, **0 / 20.000 ₺**, %0; `Add saved amount` / `Set goal as reached` |
| WL-21 | İleri dönem plan vadelerinin toplu görünümü; sıralama ve filtre | B2 5.3 | Düşük | **kapandı** | `f7-100`, `f7-101` — liste **plan başına tek satır** (yalnız bir sonraki vade), ileri dönemler toplu gösterilmiyor. Altta `All / Income / Expense / Transfer` filtre çubuğu, araç çubuğunda `Sorting`: vade yeni/eski, ad A→Z / Z→A · **24 Eyl:** `f7-101` (Sorting: vadeye ve ada göre dört seçenek) E0498 olarak envantere girdi; Belge 1 7.3 |
| WL-X1 | İlk açılış / kayıt-giriş ekranı, boş ana ekran | B1 3.1, 3.5 | Önerilmez | erişim yok | Oturum kapatma gerekir; bulut hesabı etkilenir |
| WL-X2 | Postpone / Dismiss sonucu | B1 8.5 | Önerilmez | erişim yok | Plan verisini geri dönülmez değiştirir |
| WL-X3 | Borcun tamamen kapanması ve Closed'a düşmesi | B2 4.3b | Önerilmez | erişim yok | Kurulu borç bakiyesini bozar |
| WL-X4 | Group sharing ve Bank Sync | B1 11.5 | Önerilmez | erişim yok | Üçüncü kişi ve gerçek banka hesabı gerekir |

## Bluecoins

| ID | Eksik | Kaynak | Öncelik | Durum | Sonuç / kare |
|---|---|---|---|---|---|
| BC-01 | İşlem formunun varsayılan türü | B1 5.3 | Yüksek | **kapandı** | `f7-57` — form **GİDER** seçili açılıyor (alt çubukta kırmızı, tutarın yanında kırmızı − rozeti). Aynı kare formun tamamını veriyor: İsim · tarih/saat · `Planlı İşlemler` · tutar · kategori · hesap · **Bölmek** · **Durum** · **Etiket** · Not |
| BC-02 | Kısmi ödeme sonrası dönem raporunun okunması | B2 3.6 | Yüksek | **kapandı** | `f7-79` — dönem raporu iki blok hâlinde okunuyor (aşağıda BC-12); Eylül: gelir 12.001 · gider −11.181 · net 820 |
| BC-03 | Cari hareketine fatura bağlama; Durum ve Etiket alanları | B1 9.3 + B2 4.3 | Yüksek | **kapandı** | `f7-57`, `f7-66` — `Durum` dört değer taşıyor: **Yok · Kontrol · Mutabık · İptal edildi**. Fatura/belge bağlama alanı yok; ek yalnız formun üstündeki ataç simgesiyle |
| BC-04 | Etiket alanının kayda ve rapora etkisi | B2 6.2 | Yüksek | **kapandı** | `f7-67`, `f7-68`, `f7-69` — hazır etiketler **Doğum günü · Film · İş · Kişisel · Tatil**; çoklu seçim. ₺56'lık bir kayda `İş` verildi, liste satırının altında çip olarak görünüyor. Rapor tarafı: filtre panelinde **Etiketler** ayrı bir filtre boyutu |
| BC-05 | Aktarımın ay raporu toplamlarına etkisi | B2 2.3 | Yüksek | **kapandı** | `f7-76`, `f7-80` — ₺500 Cüzdan → Ana Hesap yazıldı; bakiyeler değişti (34.700→35.200 ve −180→−680), ay gideri **₺11.181'de kaldı**. Listede aktarım **iki bacak** olarak görünüyor (`(Transfer)` −500 ve +500) ve gün toplamına net 0 katıyor |
| BC-06 | Açılışta hangi sekmenin seçili geldiği (dolu hesapta) | B1 3.2 / 3.4 | Orta | **kapandı** | `f7-55` — soğuk açılışta **Hesaplar** sekmesi seçili; ekran kart panosu (Günlük Özet, Bütçe Özeti) |
| BC-07 | Sol menüdeki iki "Hesaplar" kaleminin gittiği ekranlar | B1 3.2 | Orta | **kapandı** | `f7-61`, `f7-62` — ilki (ızgara simgesi) **kart panosu**, ikincisi (banka simgesi) **hesap kurulum listesi** (VARLIKLAR/CARI HESAP grupları, üstte `Nakit Akım Ayarı`) |
| BC-08 | Bölmek (split) alanının kayda ve toplama etkisi | B2 1.1 | Orta | **kapandı** | `f7-58`, `f7-59`, `f7-60`, `f7-65` — her satır kendi tutarı, kategorisi, hesabı, notu, durumu ve etiketiyle; üstte `Toplam tutar`. 50 (Grocery) + 60 (Others) = **₺110** yazıldı; ay gideri 11.015 → **11.125** oldu, bütçe özeti iki kategoriye dağıldı, listede **tek satır**: "2 Kategoriler" |
| BC-09 | Nakit Akım Ayarı hesabı nakit akışından çıkarıyor mu | B2 2.5 | Orta | **kapandı** | `f7-63` — ekranın kendi cümlesi: "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz." Hesap başına onay kutusu; işareti kaldırılan hesap nakit akışı hesabından düşer |
| BC-10 | Transfer ücreti alanının kayda ve iki bakiyeye etkisi | B2 2.2 | Orta | **kapandı** | `f7-74`, `f7-75` — ücret ayrı bir blok açıyor: **kendi tutarı, kendi hesabı ve kendi kategorisi** (varsayılan Diğer/Others). Yani ücret aktarımın parçası değil, ona bağlı ayrı bir gider satırı |
| BC-11 | Cari hesapta vade alanının aranması | B2 4.4 | Orta | **kapandı** | `f7-77` — cari hesap formu: Ad · Not · Başlangıç bakiyesi · Son Bakiye · **Açılış tarihi** · Hesap Tipi · "Hesap seçiminden gizle" · "Hesap hareketlerini dahil etme". **Vade alanı yok** |
| BC-12 | Net Kazançlar: iki bloğun tam adı ve dönem seçicinin etkisi | B2 7.3 | Orta | **kapandı** | `f7-79` — iki blok ay sütunlarıyla yan yana: **`Net Kazançlar`** (Gelir · Gider · Net Kazançlar) ve **`Net Kazanç`** (Varlıklar · Cari hesap · Net Kazanç). Adlar bir harf farkıyla ayrılıyor, ilki gelir-gider, ikincisi net varlık |
| BC-13 | Kayıtlı filtre profili (kaydet simgesi) | B1 10.4 | Orta | **kapandı** | `f7-69`, `f7-70` — filtre panelinin üstünde üç simge: **sıfırla · kaydet · aç**. Kaydet, ad alanı olan bir `Kaydet` sayfası açıyor — filtreler adlandırılıp geri çağrılabiliyor |
| BC-14 | İçe aktarmanın okuduğu biçimler ve dosya seçici | B2 8.1 | Orta | **kapandı** | `f7-71`, `f7-72` — `Ayarlar › Veri Yönetimi`: içe aktarma yalnız **Excel (.csv)** ve **QIF**. Banka ekstresi ayrıştırıcısı yok |
| BC-15 | Çöp kutusunun gerçekten çalışması | B2 8.2 | Orta | **kapandı (kullanıcı gözlemi)** | `f7-64` yalnız kutunun **boş** hâli (ajan koşumunda liste satırları ham `adb` dokunuşlarına yanıt vermedi). Kullanıcı elle denedi: silinen kayıt **çöp kutusuna düşüyor** ve oradan **geri yüklenebiliyor**. Geri yüklenen kayıt işlemler ekranında **hemen görünmedi**; uygulamadan çıkıp yeniden girince listede yerini aldı. Karesi yok — kanıt türü `kosum` |
| BC-16 | CSV/HTML dışa aktarma çıktısının üretimi | B1 11.1 | Orta | **kapandı** | `f7-72`, `f7-73` — uygulamanın kendi cümlesi: "Tüm raporları PDF, Excel (csv) veya Html'e aktarmak için soldaki yazıcı simgesinin olduğu her yerde bulunur." Yazıcı simgesi **PDF veya Yazıcıya gönder · Excel (.csv) · HTML** seçeneklerini açıyor · **24 Eyl doğrulama (Belge 1):** seçenekler ve cümle kareli, ama **dosya üretilmedi**; Belge 1'in eksiği (üretim) açık kalır. Karedeki yazım "Excel (cvs)" (E0493) |
| BC-17 | Seyahat modunun açık hâldeki davranışı | B2 8.5 | Düşük | **kısmen (23 Eyl doğrulama)** | `f7-78` (E0454) — anahtara dokununca **etiket seçici** açılıyor. Koşumda seçim yapılmadan iptal edildi; seçilen etiketin ne yaptığı görülmedi. "Etiketle çalışan bir süzgeç" çıkarımı kareden çıkmıyor, yazılmaz |
| BC-X1 | Kart dönemi ve Hesap Kesim Günü davranışı / kart ekstresi | B1 6.3 + B2 3.1 | Orta | **açık** | Kesim günü girili bir dönemi geçirmek gerekiyor; cihaz saati değiştirilmedi (diğer uygulamaların verisini de etkilerdi) |
| BC-X2 | Kalan beş taksidin kendiliğinden gerçekleşip gerçekleşmediği | B2 3.3 | Orta | **kapandı (kullanıcı gözlemi)** | Cihaz saati değiştirilmeden kapandı. Yaklaşan **tekrarlayan işlemler ve taksit ödemeleri**, işlemlerin yanındaki **Hatırlatıcılar** bölümünde duruyor ve **kendiliğinden işlemlere girmiyor**. Hatırlatıcıya dokununca altta **Kaydet** ve **Düzenle** çıkıyor; Kaydet işlemi gerçekleştiriyor ve kayıt işlemler listesine geçiyor. **İleri tarihli** bir taksitte Kaydet'e basınca ödeme tarihi soruluyor: **bugün** veya **taksitin kendi tarihi**. Yani gerçekleşme her taksit için **elle onaylanıyor**. Karesi yok — kanıt türü `kosum` |
| BC-X3 | CSV/QIF içe aktarma | B1 11.2 | Önerilmez | erişim yok | Ayrıştırıcıyı görmek yetti (BC-14) |
| BC-18 | Bütçe Özeti bölümünün kurulumu | B2 5.5 | Orta | **kapandı** | `f7-56` — **birleştirmede atlanmıştı, koşumda eklendi.** Bütçe Özeti kartı: Others `Güncel ₺11.015,00 / Bütçe ₺0,00`; ayarları İşlem tipi=Gider, Tarih Aralığı=Bu Ay, Grafik Türü=Kategoriye Göre. %100 "Others" görünmesinin nedeni **hiçbir kategoriye bütçe kurulmamış olması** |
| BC-19 | Otomatik kolun çalışması ve hangi tarih kolunun seçildiği | B2 5.2 | Orta | **açık** | **Birleştirmede atlanmıştı, 23 Eylül doğrulamasında eklendi.** Tarih yarısına BC-X2'nin kullanıcı gözlemi ışık tutuyor (ileri tarihli taksitte "bugün / taksit tarihi" soruluyor). Planlı işlem sayfasındaki **otomatik kol** açıkken vadenin kendiliğinden kayda dönüşüp dönüşmediği görülmedi; koşumdaki planlarda otomatik kolun açık olup olmadığı bilinmiyor |

## Goodbudget

| ID | Eksik | Kaynak | Öncelik | Durum | Sonuç / kare |
|---|---|---|---|---|---|
| GB-01 | Gelir renginin karesi (işlem listesinde gelir satırı) | B1 4.2 | Yüksek | **kapandı** | `39` — gelir satırı **yeşil ve `+` önekli** (`+1.234,00`, `+25.000,00`); gider satırı **düz siyah ve işaretsiz**. Gidere kırmızı kullanılmıyor. Kare üst çubukta **hane adını taşıyor**, basılacaksa karartılmalı |
| GB-02 | Harcama raporundaki gelir kaydının nedeni | B2 7.4 | Yüksek | **kapanmadı (23 Eyl doğrulama)** | Aşağıdaki sonuç doğru ama **7.4'ün sorusunu cevaplamıyor**: 7.4 harcama raporunun toplamında 25.000'lik gelirin durmasını soruyor (E0123, −22.950); bu satır Income 3.284'ü açıklıyor ve o açıklama Belge 2 1.8'de E0423 ile zaten var. `40` — Eylül raporu: `Income 3.284 · Spending 0 · Net Total 3.284`. 3.284 = 1.234 (Beta Tasarim) **+ 2.050 (`Initial Envelope Fill`)**. Yani **zarf doldurma hareketi rapora gelir olarak giriyor**; gerçek gelir tek başına 1.234 |
| GB-03 | Zarftaki iki sayıdan hangisi kalan, hangisi bütçelenen | B1 3.4 | Orta | **kapandı** | `38` — erişilebilirlik ağacı ayrımı veriyor: üstteki büyük sayı `envelope_item_amount` (**kalan**), altındaki küçük sayı `envelope_item_budget` (**bütçelenen**). Örn. Market 23.784,00 kalan / 850,00 bütçelenen |
| GB-04 | Tam ekran hesap makinesi ve tebrik mesajının karesi | B1 5.4 / 5.6 | Orta | **kısmen** | **Hesap makinesi kapandı:** `42` — bir ayar: `Ayarlar › Advanced › Calculator — "Use calculator to enter amounts"`. Kullanıcı gözlemi: Add Transaction'da **Amount** alanına dokununca tutar girmek için **basit bir hesap makinesi** açılıyor, yalnız **dört işlem** yapılabiliyor (karesi yok, `kosum`). **Tebrik mesajı açık kalır** — gözlenmedi, eksik olarak bırakıldı |
| GB-05 | Keep Available kipi ve FROM AVAILABLE sekmesinin sonucu | B2 1.4 | Orta | **açık** | Koşulmadı — ikinci bir doldurma kipi çalıştırmak gerekiyor |
| GB-06 | Planın gerçekleşmesi ve sıklık listesinin tamamı | B2 5.4 | Orta | **açık** | Koşulmadı — planın vadesini geçirmek gerekiyor |
| GB-07 | Silinen kaydın geri alınabildiği bir yüzey | B2 1.5 | Orta | **kapandı** | `41`, `42` — Ayarlar baştan sona tarandı: Household · Device · Localization · Advanced. **Çöp kutusu / geri alma kalemi yok.** `41` hane adını taşıyor, basılacaksa karartılmalı |
| GB-X1 | Boş ana ekran ve bütçe kurulumunu atlama yolu | B1 3.1 / 3.5 | Önerilmez | erişim yok | Yeni hane gerekir |
| GB-X2 | Aktarım, kart hesabı, Debt hesap türü | B1 6.x, 9.1 + B2 3.1, 4.1 | Önerilmez | erişim yok | Ücretli paket |

## Hesap Defterim

| ID | Eksik | Kaynak | Öncelik | Durum | Sonuç / kare |
|---|---|---|---|---|---|
| HD-01 | Üretilen PDF ve Excel dosyalarının içeriği | B1 11.1 + B2 8.1 | Yüksek | **kapandı** | `47`, `49` — `Bildiri › PDF` **iki dosya** üretip paylaşım sayfası açıyor. PDF: başlıkta defter adı, altında dönem (`Oca-01-2026 Bitiş Ara-31-2026`), kolonlar **Tarih · Notlar · Açıklama/Kategori · Gelir · Gider · Denge**, üstte `Önceki denge` satırı, altta özet (Toplam Gelir 47.500 · Toplam Gider 4.350 · Denge 43.150). Aktarım bacakları PDF'te düz gider satırı olarak görünüyor (`Kime Ortak Cuzdan`, `Kime Is Karti`) |
| HD-02 | `kasadefteri` klasörünün gerçekte oluşup oluşmadığı | B2 8.1 | Yüksek | **kapandı** | `48` — uygulamanın kendi uyarısı: "Veriler, SD kartta veya Dahili Depolamada **kasadefteri** adlı bir klasöre kaydedilir." Dosya sisteminde **böyle bir klasör yok** (`/sdcard` altında hiç eşleşme çıkmadı). PDF'ler `/sdcard/Documents/Hesap Defterim/` altına `<Defter adı> <Tarih>.pdf` adıyla, Excel ise uygulamanın kendi dış dizinine (`Android/data/cashbook.cashbook/files/Documents/Hesap Defterim/Hesap Defterim.xls`) yazılıyor. **Uyarı, oluşturmadığı bir klasörün adını veriyor** |
| HD-03 | Çok defterliyken açılışta hangi defterin seçili geldiği | B1 3.2 / 3.4 | Orta | **kapandı** | `44` — soğuk açılış doğrudan **son kullanılan deftere** (`Ana Hesap`) giriyor; arada defter seçim ekranı yok ve geri tuşu da bir listeye dönmüyor |
| HD-04 | Dönem başlangıç ayarının rapora etkisi | B1 10.3 | Orta | **açık** | Ayarı değiştirip haftalık/aylık dönem sınırlarını yeniden okumak gerekiyor; koşumda ayarlar ekranına ulaşılamadı (menüler bu oturumda açılmadı) |
| HD-05 | Haftalık ve Yıllık çiplerinin sonucu | B2 7.2 | Orta | **kapandı** | `45`, `46` — `Haftalık` → `Eyl-21-2026 → Eyl-27-2026`, gelir/gider 0, **Önceki denge 43.150** ve **Denge 43.150**. `Yıllık` → `Oca-01-2026 → Ara-31-2026`, Toplam Gider 4.350, Önceki denge 0, Denge 43.150. Her iki çip de **devreden bakiyeyi taşıyor**; dönem yalnız o aralığın hareketlerini toplar |
| HD-X1 | Bacak silmenin birleşik net varlığa etkisi | B2 2.4 | Önerilmez | erişim yok | Mevcut defteri bozar |
| HD-X2 | Drive yedek ve geri yükleme | B1 11.4 | Önerilmez | erişim yok | Gerçek Google hesabı gerekir |

## Emülatörde koşulamayanlar (masa başı / destek sayfası)

| ID | Ürün | Eksik | Kaynak | Durum |
|---|---|---|---|---|
| WB-01 | KolayBi | Ortaklar sekmesinin içeriği | B1 7.4 | **kapandı (resmî kaynak)** — `Ortaklar`, **Cari Hesaplar** modülünün beşli sekme şeridinde: `Genel Cariler · Potansiyel Müşteriler · Personel Carileri · Ortaklar · Tekrarlı Maaşlar`. Resmî metin: "Şirketinize ait ortakları **cari oluşturma adımına benzer şekilde** oluşturunuz … Daha sonra bu ortağınıza ilişkin **maaş ödemesi, prim ödemesi** vb. işlemler yapabilirsiniz." Yani ayrı bir modül değil, **cari kartının bir alt türü**. Kaynak: `kolaybi.com/destek/cari-hesaplar`. **Ortaklar sekmesinin kendi ekran görseli destek sayfasında yayımlanmamış** (paragrafın altındaki görselde `Personel Carileri` aktif; görsel numarası `carihesaplar-11` atlanmış) |
| WB-02 | KolayBi | Oluşturma Periyodu değerleri | B1 8.1 | **kapanmadı** — alanın yeri ve formu doğrulandı (`Tekrarlı Maaş Oluştur`, `Tekrarlı Proformaya Dönüştür`, `Tekrarlı Genel Gidere Dönüştür` modallerinin ortak ilk alanı; kalıp: **periyot + başlangıç tarihi + tekrar sayısı**, bitiş `Belirsiz/Belirli` radyosuyla). **Seçenek değerleri resmî kaynakta yok**: her iki modal görselinde de açılır liste kapalı, örnek listeler boş. Uydurulmadı. Ancak canlı üründen (7 günlük ücretsiz deneme) görülebilir |
| WB-X1 | KolayBi | Ödeme durumu seçiminin kasaya/cariye/rapora etkisi; proje; raporlar | B2 6.3, 7.6, 9.1 | erişim yok — ücretli |
| WB-X2 | Paraşüt | İç arayüzün tamamı; cari ve tahsilat; rapor; stok ve çek | B1 2.2, 9.5, 10.6, 12.1 + B2 9.1 | erişim yok — ücretli |
| WB-X3 | Logo İşbaşı | İç arayüzün tamamı ve müşavir portalı | B1 2.2, 9.5, 10.6 + B2 9.4 | erişim yok — ücretli |
| WB-X4 | QuickBooks Solopreneur | Solopreneur ekranı, Type alanı, Split akışı, km takibi | B1 2.2, 7.5, 12.2 + B2 6.4 | erişim yok — ABD hesabı |
| WB-X5 | Üç ön muhasebe ürünü | Fiş okumanın gerçek doğruluğu | B2 9.1 | erişim yok — canlı erişim gerekir |

---

## Sayım ve koşum sonucu

22 Eylül 2026 emülatör koşumu (Pixel_8, ham adb) + 23 Eylül kullanıcı gözlemleri
(BC-15, BC-X2, GB-04'ün hesap makinesi yarısı).

| | Koşulabilir | Kapandı | Kısmen | Açık kaldı | Erişim yok |
|---|---|---|---|---|---|
| Money Manager | 11 | **11** | — | — | 1 |
| Wallet | 21 | **20** | — | 1 (WL-11) | 4 |
| Bluecoins | 21 | **18** | 1 (BC-17) | 2 (BC-X1, BC-19) | 1 |
| Goodbudget | 7 | **3** | 1 (GB-04) | 3 (GB-02, GB-05, GB-06) | 2 |
| Hesap Defterim | 5 | **4** | — | 1 (HD-04) | 2 |
| Masa başı | 2 | **1** (WB-01) | — | 1 (WB-02) | 5 |
| **Toplam** | **67** | **57** | **2** | **8** | **15** |

Toplam 82 eksik (80 birleştirilen + BC-18 + BC-19). 22 Eylül sayımı Bluecoins'i 19 ve
toplamı 65 yazıyordu; BC-18 sonradan eklendiği için doğrusu 20 ve 66'dır.
HD-X1 satırı "açık" yazıyordu ama "erişim yok" sütununda sayılıyordu; satır
sayıma uyduruldu.

**23 Eylül kare doğrulaması** (`belge2-entegrasyon-plani.md` §2): koşum sonuçları
kareler tek tek açılarak denetlendi. WL-11 ve GB-02 `kapanmadı`'ya, BC-17 `kısmen`'e
indi; WL-05 kapandı ama kanıtı kare değil aritmetik (çıkarım). Belge 2'ye taşınan
37 kare envantere E0425–E0461 olarak girdi.

**Koşum kapandı (23 Eylül kullanıcı kararı).** Açık ve kısmen kalanlar için yeni
koşum yapılmaz; bölüm eksik listelerinde eksik olarak kalırlar. Kullanıcı
gözlemiyle kapananların karesi yoktur; belgeye `kosum` türüyle girer ve G2
gereği önce ilgili gözlem formuna yazılır.

Birleştirmede **dokuz eksik iki belgede de yazılıydı** ve tek satıra indi:
MM-08, MM-11, WL-02, WL-12, WL-17, BC-03, BC-X1, HD-01 ve Wallet bekleyen
ödeme kartı (WL-01 — Belge 1'de üç ayrı bölüm dosyasında tekrar ediyordu).
Birleştirme sırasında **bir eksik atlanmıştı** (Bluecoins `Bütçe Özeti`, B2 5.5);
koşumda fark edilip BC-18 olarak eklendi ve kapatıldı.

### Koşumda yazılan test verisi (silinmedi)

| Ürün | Yazılan | Kontrol değerine etkisi |
|---|---|---|
| Money Manager | ₺0 kayıt → ₺580'e düzenlendi · ₺300 havale · ₺150 aylık tekrarlayan plan | Eylül gideri 1.600 → **2.180**; Ortak Cuzdan 4.150 → **4.450**; Ekim'e ₺150 bekleyen tekrar |
| Wallet | 2 etiketli gider (₺100 `Isletme`, ₺50 `Sahsi`) · ₺300 Ana Hesap → Is Karti aktarımı · 2 yeni etiket | Ay gideri 21.600 → **21.750**; Ana Hesap 9.800 → **9.350**; Is Karti −5.600 → **−5.300** |
| Bluecoins | ₺110 bölünmüş kayıt (50 Grocery + 60 Others) · ₺56 `İş` etiketli gider · ₺1 kayıt · ₺500 Cüzdan → Ana Hesap aktarımı | Eylül gideri 11.015 → **11.181**; Ana Hesap 34.700 → **35.200**; Cüzdan −180 → **−680** |
| Hesap Defterim | Yeni PDF: `/sdcard/Documents/Hesap Defterim/Ana Hesap Eyl-22-2026.pdf` | Defter verisi değişmedi |
| Goodbudget | — | — |

Geri alınan tek şey **Wallet'ın `Number format` anahtarı**: yanlışlıkla kapatıldı,
kare alındıktan sonra tekrar açıldı — kapalı hâlde bütün tutarlar ondalıksız
yazılıyor ve mevcut 400 kareyle tutarsız olurdu.

### Karartma gerektiren yeni kareler

`belge1/ortak/kalip.py` → `KARARTMA_ORTAK` güncellenmeden **basılmamalı**:

| Kare | Taşıdığı kişisel veri |
|---|---|
| `wallet-budgetbakers/f7-64-hesap-duzenleme-acilis-bakiyesi-alani-yok.png` | Hesaba bağlı içe aktarma e-postası |
| `goodbudget/39-islem-listesi-gelir-yesil-arti.png` | Üst çubukta hane adı |
| `goodbudget/41-ayarlar-ust-cop-kutusu-yok.png` | Hane adı (Log Out satırı) |

Wallet çekmecesinin başlığı kullanıcının **gerçek adını** taşıyor; o yüzden
çekmece kareleri (`f7-90`, `f7-92`, `f7-104`) bilinçli olarak **başlık kaydırılmış
hâlde** alındı.

### Açık kalanlar ve nedenleri

| ID | Neden kapanmadı | Ne gerekiyor |
|---|---|---|
| BC-X1 | Cihaz saati ileri alınmadı | Kesim günü geçmiş bir tarihe gitmek — **diğer altı uygulamanın verisini de etkiler** |
| GB-04 (tebrik mesajı) | Hesap makinesi kullanıcı gözlemiyle kapandı; tebrik mesajı gözlenmedi | Goodbudget'ta bir kayıt girip kayıttan sonraki mesajı okumak |
| GB-05, GB-06 | İkinci doldurma kipi / plan vadesi | Ayrı bir koşum |
| HD-04 | Hesap Defterim'in menüleri bu oturumda açılmadı | Ayarlar → dönem başlangıcını değiştirip dönem sınırlarını okumak |
| WB-02 | Değerler resmî kaynakta yayımlanmamış | KolayBi'nin 7 günlük ücretsiz denemesi |
| WL-11 | Kare uyarı baloncuğu kaybolduktan sonra alınmış; mesajın yokluğunu kanıtlamıyor | Gerek yok — E0281 aynı durumu mesajıyla kanıtlıyor; eksik yalnız bu koşumun sonucu için |
| GB-02 | Koşumun cevabı başka bir sayıyı açıklıyor | Harcama raporunda 25.000'lik gelirin neden durduğu — ayrı bir soru |
| BC-17 | Seyahat modunda etiket seçildikten sonrası görülmedi | Bir etiket seçip ana ekranı okumak |
| BC-19 | Otomatik kolun açık olduğu bir plan izlenmedi | Otomatik kolu açık bir plan kurup vadesini geçirmek (cihaz saati) |
