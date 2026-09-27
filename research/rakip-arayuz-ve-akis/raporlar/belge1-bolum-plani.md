# Belge 1 — Bölüm planı

> **24 Eylül 2026 notu:** bu belge 17 Eylül'ün 13 bölümlük düzenini anlatır ve tarihseldir. Düzeltme
> turunda eski Bölüm 1 (okuma kılavuzu) ve 2 (ürün kimliği) yeni **Bölüm 1 · Giriş ve incelenen ürünler**de
> birleşti; eski 3–13 bugün 2–12'dir: 3→2 ana ekran · 4→3 görsel dil · 5→4 işlem ekleme · 6→5 hesap,
> kart, transfer · 7→6 sınıflandırma · 8→7 plan · 9→8 borç · 10→9 rapor · 11→10 veri · 12→11 modüller ·
> 13→12 ortak tercihler. Alt soru numaraları da birer azaldı (ör. eski 8.4 → 7.4). Güncel durum:
> [belge1-duzeltme-plani.md](belge1-duzeltme-plani.md) §12.

16 Eylül 2026 · v2 · Bölüm yazımına başlamadan önceki karar belgesi.

Bu dosya Belge 1'in **ne anlatacağını** belirler: bölümler, her bölümün alt soruları, her sorunun
hangi ürün ve kanıtla cevaplanacağı, ve her bölümün sınırı. Biçim kararları sonda ayrı bölümdedir.

**v2, dış inceleme turundan sonra yazıldı.** Değişenlerin listesi son bölümdedir.

**Nasıl üretilir** [`belge1/README.md`](belge1/README.md) içindedir; altyapı `belge1/ortak/`
altındadır (`motor.py` üretim motoru, `motor-testi.py` duman testi, `kanit-dizini.json` 357 karenin
kimlik/yol/hash/içerik açıklaması). Tema kayıtları kareleri dosya adıyla anar; dizin bunu E
kimliğine çevirir.

---

## 1 · Kararlar

**K** kullanıcının verdiği kararı, **Ö** bana bırakılan yetkiyle aldığım kararı gösterir.

| # | Karar | Gerekçe |
|---|---|---|
| K1 | **Konu ekseni.** Bölümler konuya göre; her bölümde aynı soru ürünlere yöneltilir. Ürün başına toplu anlatım yalnız Bölüm 2'de | Belge başvuru kaynağı olarak kullanılacak |
| K2 | **Her bölüm kendi biçimini alır.** Bölüm başına sabit sayfa veya sabit alt başlık sayısı yoktur | Kanıt yoğunluğu bölümden bölüme çok değişiyor |
| K3 | **Ağırlık dengesizdir.** Giriş ekranları, işlem ekranları, hesap ve kart yüzeyleri derin; veri aktarımı, yedek ve yardımcı modüller kısa | Ürün kararlarımızı besleyecek yerler bunlar |
| K4 | **Paraşüt ve Logo İşbaşı'nın kareleri basılmaz.** Gözlemleri ve kaynak beyanları **metinde kalır** | Kareler ürünün iç arayüzünü yeterince temsil etmiyor |
| Ö1 | **KolayBi'nin destek sayfası görselleri basılır, video kareleri basılmaz ama anılır** | Destek görseli ürünün arayüzü anlatmak için yayımladığı ekran görüntüsü; video karesi videonun anı. Panonun kaynaklarda iki farklı hâlde olması "güncel sürüm doğrulanmadı" sınırının dayanağı — basmadan söylenebilir |
| Ö2 | **Kanıt anahtarı beş türlüdür** (aşağıda) | Dört türlük anahtarda "ürünün kılavuzu şunu yazıyor" için yer yoktu; Paraşüt, Logo ve QuickBooks içeriğinin büyük kısmı tam olarak bu |
| Ö3 | **Kazandırdığı / Bedeli kalıbı kullanılmaz.** Yorum gerekiyorsa seyrek ve "olası etki — ölçülmedi" etiketiyle. Ürün tercihi Belge 3'e kalır | O kalıp her gözleme zorla bir avantaj ve dezavantaj buldurtuyor |
| Ö4 | **Kanıt niteliği ürüne değil ifadeye bağlanır** | Bir ürünün canlı incelenmesi, hakkındaki her cümlenin sınandığı anlamına gelmez |
| Ö5 | **Ekran argümandır, tablo ektir.** Kareler büyük basılır, üzerlerine numaralı işaret konur, metin o numaralara konuşur | İşaretli deneme bunu doğruladı |
| Ö6 | **Bir ürün, bir soruda kanıtı varken listeden düşmez** | Bluecoins hem üst sekme hem çekmece kullanıyor; denemede çekmece sayfasından düşmüştü |
| Ö7 | **Ayrıntının tek sahibi olur; kısa bağlam ve gönderme serbesttir** | "Bir konu ikinci kez anlatılmaz" kuralı fazla katıydı: ana ekranın haritası, aşağıda ne olduğunu söylemeden tamamlanmıyor |

### Kanıt anahtarı

| Tür | Ne demek | Basılır mı? |
|---|---|---|
| **Canlı kare** | Emülatörde açılan ekranın görüntüsü | Evet |
| **Koşum kaydı** | Gözlem formunda yazılı; karesi yok veya kare tek başına göstermiyor. Kullanıcı kontrolleri de buraya girer | — |
| **Kaynak görseli** | Ürünün arayüzünü anlatmak için yayımladığı ekran görüntüsü (KolayBi destek sayfası) | Evet |
| **Kaynak beyanı** | Ürünün kılavuzu, yardım merkezi veya ürün sayfası metni. Görsel yok | — |
| **Görülmedi** | Kanıt yok. Özelliğin bulunmadığı anlamına **gelmez** | — |

Basılmayan bir kaynağa dayanan cümle kaynak niteliğini kaybetmez. Kaynağın alt türü (destek metni,
video karesi, temsili çizim) `kaynaklar.md` içinde kayıtlı kalır; metinde gerektiğinde anılır.

### Ürün ve erişim düzeyi

| Ürün | Platform | Erişim | Kare | Basılır mı? |
|---|---|---|---|---|
| Bluecoins | Android | Canlı, derin | 92 | Evet |
| Wallet (BudgetBakers) | Android | Canlı, derin | 109 | Evet |
| Hesap Defterim | Android | Canlı | 45 | Evet |
| Money Manager | Android | Canlı | 30 | Evet |
| Goodbudget | Android | Canlı, ücretsiz paket sınırlı | 31 | Evet |
| KolayBi | web | Masa başı | 39 | **Yalnız 31 destek görseli** |
| Paraşüt | mobil/masaüstü | Masa başı | 9 (2 canlı yakalanmış giriş yüzeyi + 7 video) | **Hayır** |
| Logo İşbaşı | mobil | Masa başı | 6 (4 canlı yakalanmış giriş/kayıt yüzeyi + 2 video) | **Hayır** |
| QuickBooks Solopreneur | — | Yalnız yardım merkezi | 4 (Solopreneur ekranı değil) | **Hayır** |

Paraşüt ve Logo'nun karelerinin bir kısmı gerçekten canlı yakalanmış giriş/kayıt yüzeyidir; hepsi
video karesi değildir. Basılmama gerekçesi **ürünün iç arayüzünü temsil etmemeleridir**, video olmaları
değil. Dayanak türleri kayıtta korunur.

---

## 2 · Bölümler — özet

| # | Bölüm | Ağırlık | Alt soru | Sayfa | Kaynak |
|---|---|---|---:|---:|---|
| 1 | Okuma kılavuzu ve kanıt düzeyleri | hafif | 4 | 2 | tema-01 |
| 2 | Ürün kimliği | orta | 9 kart | 6 | tema-01, formlar |
| 3 | **Gezinme ve ana ekran** | **ağır** | 7 | 9 | tema-02 |
| 4 | Görsel dil | orta | 5 | 6 | tema-02 + yeni tarama |
| 5 | **İşlem ekleme ve geri bildirim** | **ağır** | 9 | 10 | tema-03 |
| 6 | **Hesap, kart ve transfer** | **ağır** | 7 | 10 | tema-04 |
| 7 | Sınıflandırma yüzeyleri | orta | 5 | 5 | tema-05 |
| 8 | Plan, tekrar, taksit ve bekleyen | orta | 6 | 7 | tema-06 |
| 9 | Borç, fatura ve tahsilat | orta | 5 | 6 | tema-07 |
| 10 | Rapor ve dönem seçimi | orta | 6 | 6 | tema-08 |
| 11 | Veri aktarımı, yedek ve paylaşım | hafif | 5 | 3 | tema-09 |
| 12 | Diğer modüller ve yardımcı araçlar | hafif | 1 tablo | 3 | tema-10 |
| 13 | Ortak tercihler ve ayrışmalar | hafif | — | 3 | hepsi |
| Ek | Kanıt eki | — | — | 2 | envanter |

**Toplam ≈ 78 sayfa.** Tahminin nasıl çıktığı: ağır bölümlerde alt soru başına ortalama bir sayfa
(soru + 2–3 işaretli kare + notlar), orta bölümlerde alt soru başına bir sayfa, hafif bölümlerde
tablo başına bir sayfa. Bu bir **hedef değil**; bölüm bittiğinde sayfası kaç olursa odur. İlk bölüm
yazıldıktan sonra tahmin gerçek ölçüye göre güncellenir.

---

## 3 · Ağır bölümler

### Bölüm 3 · Gezinme ve ana ekran — 9 sayfa

**Ana soru:** Uygulama açıldığında ekran neyi önce söylüyor ve kullanıcıyı nereye çağırıyor?

| # | Alt soru | Ürün ve kanıt | Not |
|---|---|---|---|
| 3.1 | İlk açılışta ne isteniyor? | **Canlı kare:** Bluecoins karşılama (E0017) · Hesap Defterim kullanım diyaloğu (E0135) · Goodbudget LOG IN / CREATE NEW HOUSEHOLD (E0106). **Koşum kaydı:** Money Manager karşılama yok (E0010 K00) · Wallet ilk açılış görülmedi (E0014 K00). **Koşum kaydı, karesiz:** Logo kayıt formu üç alan + SMS + sözleşme; Paraşüt giriş ekranı geçilemedi | Hesap Defterim'de diyalog metni Ücretli/Alınan, düğmeler Ödendi/Alındı — çelişki tek karede görünür |
| 3.2 | Ana ekran neyi önce gösteriyor? | Beş canlı ürünün dolu ana ekranı yan yana, büyük. **Ölçü şeridi ve yüzde bloğu yok.** MM dönem özeti → gün listesi · BC özet kartları + reklam + bütçe özeti · WL hesap kartları → tanıtım kartları · HD liste + satır dengesi + sabit toplam · GB zarflar | **3.2 ve eski 3.3 birleşti.** Metin görünen ayrımları yazar: ilk sırada ne var, alt çubuk var mı, tanıtım var mı, toplam satırı sabit mi. Wallet'ın ana ekranı aşağıda devam ediyor: **"aşağısında planlı ödeme kartı bulunuyor; ayrıntısı Bölüm 8'de"** — ana ekranın haritası bu cümleyle tamamlanır |
| 3.3 | Bölüm seçici nerede duruyor? | Beş üründe gezinme bölgesi aynı genişlikte kırpılıp alt alta: MM alt sekme · GB üst sekme · BC kaydırılan üst sekme (son sekme kesiliyor) · WL çekmece düğmesi + ekran içi sekme · HD çekmece düğmesi + defter seçici | **Her kırpıntı konumunu yazar** (ekranın üstü / altı) ve tam ekran şekline gönderme yapar; kırpıntı tek başına konumu anlatmaz |
| 3.4 | Çekmecenin içinde ne var? | **Üç ürün:** Wallet (E0376) · Hesap Defterim (E0171) · **Bluecoins (E0084)** | Ö6'nın uygulaması. Üçünde de finansal bölüm ile yardımcı araç ayrımsız listeleniyor; BC'de aynı adlı iki "Hesaplar" kalemi var |
| 3.5 | Kayıt nereden başlıyor? | Dört üründe sağ altta tek yuvarlak düğme (kırpıntı) · HD'de iki yön düğmesi ekranda, üçüncü yön (Aktar) çekmecede | Formun içi Bölüm 5'te |
| 3.6 | Veri yokken ekranda ne kalıyor? | MM kayıtsız ay (E0227) · BC temiz kurulum (E0026) · HD boş defter (E0136). **Görülmedi:** Wallet, Goodbudget | Aşağıdaki nota bak |
| 3.7 | Kaynakta görülen ana ekran | KolayBi Güncel Durum panosu (kaynak görseli) | Demo verisi Ocak 2023 tarihli; güncel sürüm doğrulanmadı. **Kaynak beyanı:** Paraşüt'ün anlatımında ana ekran tahsilat ve ödemeyi vade durumuna göre ayırıyor |

**3.6 için düzeltilmiş çerçeve.** Önceki taslak "HD yapıyı korur sözcük vermez, BC tersi" diye keskin
bir karşıtlık kuruyordu; kareler bunu desteklemiyor. Bluecoins'in boş ekranı da sekmeleri ve bakiye
alanını koruyor; Money Manager'ın boş ayında dönem seçici, sıfırlanmış üç özet, alt sekmeler ve
kayıt düğmesi duruyor. Gerçek ayrım **hangi sözcüklerin bulunduğudur**: Bluecoins ilk eylemi adıyla
çağırıyor ve para birimini yazıyor; Money Manager yalnız "Veri yok." diyor; Hesap Defterim hiçbir
yönlendirme metni vermiyor. Üç kare ayrıca üç farklı duruma ait: kayıtsız bir ay, temiz kurulum,
boş defter.

**Bu bölüme girmez:** form alanları → 5 · hesap ve kart yüzeyi → 6 · bekleyen işler ekranı → 8 ·
rapor ekranı → 10 · renk ve tutar biçimi → 4 · kaydın finansal sonucu → Belge 2.

---

### Bölüm 5 · İşlem ekleme ve geri bildirim — 10 sayfa

**Ana soru:** Kullanıcı yeni bir kaydı nasıl giriyor ve kaydettikten sonra ne görüyor?

| # | Alt soru | Ürün ve kanıt | Not |
|---|---|---|---|
| 5.1 | Form nereden açılıyor, kaç adımda? | Beş canlı ürün; Bölüm 3.5'in devamı | Wallet'ta FAB menüsü ara adım getiriyor |
| 5.2 | Formda hangi alanlar var, hangi sırayla? | **MM** tek ekran beş alan (Tarih·Tutar·Kategori·Hesap·Not) · **BC** tek yoğun ekran (ad, tarih/saat, planlı, tutar, kategori, hesap, taksit, bölme, durum, etiket, not) · **WL** iki katman: hızlı form → kayıt sonrası Record detail · **HD** tür+tarih+saat+tutar+not, kategori alanı yok · **GB** Payee·Amount·Envelope·Account·Date·Check#·Schedule·Notes | **Beşi yan yana basılmaz.** Form alanları yan yana 4 cm'de okunmaz; 2+3 yerleşim veya ardışık sayfa kullanılır |
| 5.3 | Tür ve yön nasıl seçiliyor, varsayılan ne? | MM Gelir/Gider/Havale segmenti · BC altta GİDER/GELİR/TRANSFER, varsayılan GİDER (koşum kaydı) · WL varsayılan Expense, seçili tür yalnız zemin tonuyla · HD yön ana ekrandaki düğmede · GB formda | "Varsayılan tür" ile "her kayıtta seçim gerekiyor" ayrı iddialardır; ikincisi doğrulanmadı |
| 5.4 | Tutar nasıl giriliyor? | Dört üründe hesap makinesi tuş takımı; GB'de tam ekran | Cihaz klavyesi kullanan ürün görülmedi |
| 5.5 | Art arda kayıt ve kolaylıklar | MM "Devam et" · HD "Kaydet ve devam Et" + kalem dökümü (Öğe eklemek) + not önerileri + düğme adını değiştirme · WL Templates, Split · GB Quick Transactions widget, konuma göre payee önerisi · BC aynı formda planlama/taksit/bölme | Beş üründe beş ayrı kolaylık kümesi |
| 5.6 | Kaydetme sonrası geri bildirim | Sessiz dönüş (MM, WL, BC) · kısa toast "İşlem Eklendi" (HD) · oyunlaştırılmış tebrik (GB) | — |
| 5.7 | Hata ve zorunlu alan | **Alan dışı mesaj:** MM "Lütfen hesabı seçiniz." toast · WL sıfır tutarda snackbar · GB "Select an Envelope." **Alan yanında mesaj:** WL planlı ödemede kırmızı "Select category". **Sessiz:** HD boş tutarda no-op, ₺0 kabul | Mesajın nereye konduğu ürünler arasında ayrışıyor |
| 5.8 | Düzeltme ve silme | Satır/detayda doğrudan düzenleme (MM, WL, GB) · HD'de başka deftere taşıma, kopyalama, **çöp kutusu + Geri Yükle** · **Bluecoins'te de Çöp Kutusu yüzeyi var** · onay diyaloğu (BC, GB) | Geri alma yüzeyi iki üründe görüldü; geri yüklemenin sonucu yalnız Bluecoins'te kullanıcı kontrolüyle anlatıldı ve sınırları oradaki kayıtta yazılı |
| 5.9 | Kaynakta görülen formlar | **Kaynak görseli:** KolayBi "Yeni Genel Gider" (cari takibi, ödeme durumu, vade, gider tipi, proje, KDV bandı, dosya yükleme) ve satış/alış fatura formu. **Kaynak beyanı:** Paraşüt beş gider türü ve fiş okuma; Logo sesli fatura; QuickBooks banka feed'i ağırlıklı otomatik iniş | Alan adları kanıtlı, davranış değil |

**Bu bölüme girmez:** kaydın bakiyeye/rapora etkisi → Belge 2 §2 · tekrar ve taksit formu → 8 ·
kategori ve etiketin anlamı → 7 · **fiş eki ve okuma → 11** · form renkleri ve tutar biçimi → 4.

---

### Bölüm 6 · Hesap, kart ve transfer — 10 sayfa

**Ana soru:** Para nerede duruyor, ürün bunu nasıl gösteriyor ve hesaplar arasında nasıl geçiriliyor?

| # | Alt soru | Ürün ve kanıt | Not |
|---|---|---|---|
| 6.1 | Hesap listesi nasıl sunuluyor? | MM Nakit/Banka/Kredi Kartı grupları, üstte Varlıklar–Borçlar–Toplam · BC banka/nakit/kart/cari grupları · WL yatay hesap kartları · HD hesap türü yok, her hesap ayrı defter ve **liste bakiye göstermiyor** · GB Checking/Savings/Cash · Credit Card · Debt, hesap katmanı **varsayılan kapalı** | Yokluk ifadeleri incelenen sürüm ve yüzeyle sınırlıdır |
| 6.2 | Hesap açma ve açılış bakiyesi alanı | **Dört farklı alan tasarımı:** MM hesap formunda "Tutar" alanı ve kaydedince çıkan "İşlemler bölümünde gösterilsin mi?" sorusu · BC formda Başlangıç bakiyesi **ve** Açılış tarihi aynı ekranda · WL cash/checking oluştururken açılış alanı yok (koşum kaydı) · HD ayrı bir "Açılış bilançosu" kaydı, tarihli | **Alanlar ve sorulan soru Belge 1'de.** Açılışın hangi toplama girdiği → Belge 2 §3 |
| 6.3 | Kredi kartı yüzeyi | **MM ekstre dönemli:** Kaynak, Hesap Kesim Tarihi, Son Ödeme Tarihi · **BC alanlar var:** Kredi Limiti, Hesap Kesim Günü, Bitiş tarihi · **WL dönemsiz:** limit, Available Credit, tek Payment Due Date, ekstre yok · **HD kart kavramı yok** · **GB kart hesabı açılamadı** (ücretsiz pakette 1 hesap) | Beş farklı kart sunumu ve erişim durumu. Goodbudget'a erişememek ürün hakkında hüküm değildir |
| 6.4 | Kart borcu nasıl gösteriliyor? | MM Hesaplar'da **Bu Ay / Gelecek Ay** iki sütun; kart defterinde Para Yatırma/Çekme sütunları ve satır başına yürüyen bakiye · WL negatif bakiye ve eşik uyarısı · BC kart hesabı negatif | İncelenen beş üründen yalnız MM iki dönem sütunu gösteriyor |
| 6.5 | Kart ödemesi nereden başlıyor? | **MM'de kart defterinde ayrı "Ödeme" düğmesi**, borç tutarıyla ön doldurulmuş havale açılıyor, tutar düzenlenebilir · BC hesaplar arası transfer · WL ayrı ödeme akışı yok, serbest tutarlı transfer · HD jenerik aktarım | İncelenen beş canlı üründen yalnız birinde karta özel ödeme yüzeyi var |
| 6.6 | Transfer formu ve listede gösterimi | MM ayrı Havale segmenti, tek nötr satır · BC TRANSFER türü, ücret alanı, yön değiştirme ikonu, iki bağlı bacak · WL From/To sekmesi, listede iki satır · HD Aktar: Miktar+Kimden+Kime · GB ayrı Account Transfer ekranı | Listede tek satır mı iki satır mı — görünür ayrım |
| 6.7 | Kaynakta görülen hesap/kart yüzeyleri | **Kaynak görseli:** KolayBi Finans: Banka Hesapları / Kasalar / Kredi Kartları / Online Banka / Çekler / Senetler; her listede Açılış Tarihi + Bakiye kolonu; kredi kartı formunda Kesim Günü, Son Ödeme Günü, Limit, Minimum Ödeme Oranı, Kalan Limit. **Kaynak beyanı:** Paraşüt kasa ve bankaları tek listede IBAN ve döviz sütunuyla gösteriyor | Alan adlarıyla kanıtlı; tablo boş, davranış yok |

**Bu bölüme girmez:** kart harcamasının rapora etkisi → 10 ve Belge 2 · taksitli kart harcaması → 8 ·
cari/karşı taraf hesabı → 9 · transferin toplamlara etkisi → Belge 2 · banka bağlantısı → 11.

---

## 4 · Orta ağırlıktaki bölümler

### Bölüm 2 · Ürün kimliği — 6 sayfa

Dokuz ürün kartı. **Sabit alan listesi, değişken uzunluk** — kanıtı az olan ürün sayfa doldurmak
için genişletilmez.

Kart alanları: ürün ve incelenen platform/paket · kaynaklarda belirtilen kullanım odağı · gözlenen
temel arayüz yaklaşımı · inceleme kapsamı ve bilinmeyenler · ilgili bölümlere bağlantı.

- **Canlı beşi:** küçük ana ekran karesiyle, yarım sayfa
- **KolayBi:** kaynak görseliyle
- **Paraşüt, Logo İşbaşı, QuickBooks Solopreneur:** karesiz, yalnız metin

Logo'nun kayıt akışı burada anlatılır: üç alan + SMS + sözleşme, ardından **incelenen kayıtta hesap
hemen kullanıma açılmadı, satış/hazırlık sürecine girdi.** Bu tek koşumun gözlemidir; ürünün bütün
kullanıcılarına genellenmez.

**Girmez:** yaygınlık, memnuniyet, fiyat karşılaştırması. Fiyat kullanılacaksa paket, ülke, dönem ve
kaynak tarihi birlikte yazılır; tercihen hiç kullanılmaz.

### Bölüm 4 · Görsel dil — 6 sayfa

**Ana soru:** Ürün sayıyı, rengi ve durumu nasıl gösteriyor?

Bu bölümün kendi tema kaydı yok; mevcut kareler **beş sabit soruyla** yeniden taranarak beslenecek.
Yeni emülatör koşumu planlanmıyor, ama tarama sonunda gerekirse açılır.

1. **Tutar nasıl yazılıyor?** MM ₺ 25.000,00 · GB 2,050.00 simgesiz, tarih MM/DD/YYYY · WL Tur 1'de
   "TRY 20,800.00", sonraki koşumda "₺20.200,00" · HD tam sayı, binlik ayraç, simge yok · BC iki
   ondalık ₺
2. **Renk hangi anlamı taşıyor, yanında sözcük var mı?** MM gelir mavi / gider turuncu-kırmızı ·
   BC gelir yeşil / gider pembe / transfer mavi · HD Alındı yeşil / Ödendi kırmızı — **düğme adında
   da yazıyor** · WL seçili tür yalnız zemin tonuyla
3. **Liste, kart, tablo, grafik nerede kullanılıyor?** HD sütun başlıklı tablo · BC kart panosu ·
   WL hesap kartı + grafik · MM liste · GB ilerleme çubuğu
4. **Durum, seçili filtre ve dönem nasıl görünür?** BC "Dün bitti" / "Bugün süresi doluyor" gibi
   **sözcükle yazılan durum** · dönem çipleri (HD) · ay gezgini (MM)
5. **Boş, hata ve yükleme durumları** Bölüm 3.6 ve 5.7'nin kareleri burada **biçim açısından**
   okunur (Ö7: ayrıntının sahibi orası, burada yalnız görsel dil). Wallet'ın iskelet yükleme blokları
   bu bölümün tek yükleme kanıtı

**Girmez:** gezinme yapısı → 3 · form alan sırası → 5 · raporun anlamı → 10 · erişilebilirlik hükmü
(ölçülmedi, hiçbir bölümde yazılmaz).

### Bölüm 7 · Sınıflandırma yüzeyleri — 5 sayfa

**Ana soru:** Kayıt hangi eksenlerde sınıflandırılıyor ve bu arayüzde nasıl görünüyor?

1. **Kategori formda nasıl seçiliyor?** MM ızgara · BC liste · GB zarf seçimi · **HD'de kategori
   alanı yok**, yerine serbest "İşlem adları" · WL kategori listesi (gider formunda gelir kategorisi
   seçilebiliyor)
2. **Etiket/label yüzeyi** BC etiketler (listede "İş" ve "Kişisel" adlı değerler **var** — kapsam
   boyutu olarak kullanıldığı iddia edilmez) · WL Labels
3. **Proje ekseni** KolayBi: kod, ad, para birimi, tarih, Aktif/Pasif; listede Gelir/Gider/Net;
   formlarda Proje alanı ve "Proje Takip'i kapatabilirsiniz" notu — kaynak görselleri
4. **Cari/karşı taraf bir eksen olarak** KolayBi Müşteri/Tedarikçi birleşik tipi; Ortaklar ve
   Personel Carileri. Ayrıntı → 9
5. **İşletme/şahsi ayrımı nerede var?** Canlı beş üründe ve üç Türk ön muhasebe ürününde kayıt
   düzeyinde kapsam alanı **görülmedi**. **Kaynak beyanı:** QuickBooks Solopreneur'ün yardım
   merkezinde işlem başına Business/Personal `Type` alanı ve Split anlatılıyor

**Girmez:** kapsamın BF için anlamı → Belge 3 · rapor kırılımı → 10 · demo proje adlarından kullanım
amacı çıkarımı (yasak).

### Bölüm 8 · Plan, tekrar, taksit ve bekleyen — 7 sayfa

**Ana soru:** Gelecekteki bir ödeme nasıl kuruluyor ve bekleyen işler nerede görünüyor?

1. **Tekrar kurulum formu** MM aylık form rozeti · BC sıklık + bitiş + otomatik giriş kutusu ·
   WL Planned payments → Recurrent payment · GB "Schedule this…" sıklık listesi + e-posta hatırlatma ·
   **HD'de finansal plan/tekrar yüzeyi görülmedi** (menü, ayar ve çekmece tarandı)
2. **Taksit kurulumu** MM 6 ay, başlıkta (1/6) · BC 6 ay + oran alanı + kaydetmeden önce altı kalem
   özeti · **WL'de incelenen yollarda taksit alanı yok** · GB "Split into multiple Envelopes" tutarı
   zarflara böler, zamana yaymaz
3. **Bekleyen liste ekranı** BC Hatırlatıcılar sekmesi: tarih grupları, "Dün bitti" / "Bugün süresi
   doluyor" etiketleri, taksit sayacı (2/6) · WL Planned payments
4. **Ana ekranda bekleyen iş** Wallet'ın Home'unun aşağısında "Upcoming planned payments" kartı;
   bu karede satırlar yükleniyor. Diğer dört üründe incelenen ana ekran karesinde bekleyen iş alanı
   görünmüyor — ürün genelinde yokluk iddia edilmez
5. **Onay ve otomatik tercihi** WL Confirm → Payment summary → otomatik/onaylı sorusu; menüde
   Postpone / Dismiss (sonuçları denenmedi) · BC otomatik giriş kutusu
6. **Bütçe ve hedef yüzeyi** WL bütçe formu, Spent/Remains, günlük ortalama, Forecasted Spend (yöntem
   bilinmiyor) · WL hedef: Target amount, Saved already, Desired date · GB zarf satırında
   kalan/bütçelenen ve ilerleme çubuğu · KolayBi Nakit Akış Raporu'nda tahmini dönem sonu alanı

**Girmez:** kaydın gerçekleşince bakiyeye etkisi → Belge 2 · borç/fatura zinciri → 9 · bütçe
raporunun okunması → 10 · tahmin formülü hakkında hüküm (bilinmiyor).

### Bölüm 9 · Borç, fatura ve tahsilat — 6 sayfa

**Ana soru:** Bir alacak nasıl kaydediliyor, tahsilat nasıl işleniyor ve kalan nerede görünüyor?

1. **Karşı taraf listesi** BC cari hesap grubu · WL Debts yüzeyi · KolayBi cari listesi
   (Müşteri/Tedarikçi birleşik tip) — kaynak görseli
2. **Borçlandırma hangi ekrandan yapılıyor?** BC: normal işlem formunda GELİR türü seçilip hesap
   olarak cari hesap gösteriliyor · WL: ayrı bir Debt kartı, tür olarak "I Lent"; form Record
   oluşturmayı ayrıca soruyor
3. **Tahsilat hangi ekrandan yapılıyor?** BC: TRANSFER formu, cari hesaptan bankaya · WL: aynı borç
   kartında Add Record → Repay debt
4. **Kalan tutar nerede yazıyor?** BC'de **cari hesap satırında**; belirli bir faturaya tahsis alanı
   görülmedi · WL'de **borç kartının üstünde**; aynı adlı iki borç ayrı kart olarak duruyor
5. **Kaynakta görülen akışlar** **Kaynak görseli:** KolayBi cari detayı — Borç/Alacak Ekle, Fatura
   Ekle, Ödeme/Tahsilat Ekle ayrı eylemler; hareketlerde borç/alacak, yürüyen bakiye, vade; açılış
   bloğu; Mahsuplaştır menüsü; ekstre PDF önizlemesi. **Kaynak beyanı:** Paraşüt'ün kılavuzunda
   gider/fatura önce, ödeme sonra; kısmi ödeme, avans ve en gecikmiş açık faturadan otomatik eşleme.
   Logo'nun anlatımında müşteri seçerken bakiyenin görünmesi. QuickBooks'un ürün sayfasında
   müşteri → kalem → ödeme yöntemi → fatura gönderimi

**Girmez:** tutarların bakiyeye etkisi ve zinciri → Belge 2 §2–3 · e-belge ve entegrasyon → 11 ·
rapor toplamları → 10 · tekrar eden fatura → 8 · ADR 0014 ile eşdeğerlik hükmü (reddedildi).

### Bölüm 10 · Rapor ve dönem seçimi — 6 sayfa

**Ana soru:** Rapor ekranı neyi nasıl gösteriyor; dönem ve filtre nasıl seçiliyor?

1. **Rapor ekranı ne gösteriyor?** MM Gelir/Gider/Toplam; Toplam sekmesinde nakit-banka gideri ile
   kart harcaması ayrı satırlarda · BC Net Kazançlar · WL Cash-flow + Spending kategori dağılımı ·
   GB Spending by Envelope, Income vs Spending · **HD grafik yerine tablo ve takvim**
2. **Toplamın etiketi neyi söylüyor?** HD ekranda "Toplam Alındı / Toplam Ödendi / Denge" yazıyor;
   **etikette bu toplamın neyi içerdiği yazmıyor.** Neyin dahil olduğu → Belge 2 §8. Belge 1'in
   bulgusu etiketin kendisi
3. **Dönem seçimi** MM ay gezgini · HD Herşey/Günlük/Haftalık/Aylık/Yıllık + dönem başlangıcı ayarı ·
   GB açılışta güncel ay · WL dönem başlangıç günü ayarı
4. **Filtre yüzeyi** BC tutar aralığı, tarih, tür, kategori, hesap, etiket, durum, metin ·
   MM GELİR/GİDER/HESAP sekmeleri · WL Settings → Filters girişi (denenmedi)
5. **Hesabı toplamdan çıkarma denetimi** MM'de hesap formunda "Toplama Dahil Et" anahtarı; kapalı
   hesabın satırı listede gri görünüyor · BC'de nakit akışı raporuna katılım ayarı. **İki ayrı
   ölçüyü etkiliyorlar, eşdeğer sayılmaz.** Anahtarın toplamlara etkisi → Belge 2
6. **Boş rapor ve kaynak rapor aileleri** GB "No transactions found." · **Kaynak görseli:**
   KolayBi'nin on raporluk üst barı ve KDV matrisi. **Kaynak beyanı:** Paraşüt Gelir/Gider ile
   Kasa/Banka raporlarını ayırıyor; Logo tarih aralıklı kategorize raporlardan söz ediyor;
   QuickBooks Business/Personal ayrımını vergi raporuna bağlıyor

**Girmez:** dosya çıktısı ve teslim → 11 · toplamların içeriği ve doğruluğu → Belge 2 · vergi
mevzuatı yorumu (hiçbir bölümde yok).

---

## 5 · Hafif bölümler

### Bölüm 1 · Okuma kılavuzu ve kanıt düzeyleri — 2 sayfa

1. Ne incelendi, hangi tarih aralığı, hangi platform
2. **Kanıt anahtarı** (yukarıdaki beş tür) ve "görülmedi ≠ yok" kuralı
3. **Neden bazı ürünlerin karesi basılmıyor.** Paraşüt ve Logo'nun karelerinin bir kısmı canlı
   yakalanmış giriş/kayıt yüzeyi, bir kısmı tanıtım videosu karesi; hiçbiri ürünün iç arayüzünü
   temsil etmiyor. Bu yüzden basılmıyorlar, gözlemleri metinde kalıyor. Bir kaynağın ne kadar
   uzaktan olduğu örnekle anlatılır — karesiz: bir tanıtım karesinde sol menü kutuları etiketsiz
   çizilmiş, iki halka aynı tutarı gösteriyor ve uyarı kutusundaki üçüncü tutar ikisiyle uyuşmuyor
4. Bu belge neyi ölçmez: başarı, hız, memnuniyet, erişilebilirlik. Kaydın finansal sonucu Belge 2'nin,
   BusinessFinance tercihleri Belge 3'ün konusu

### Bölüm 11 · Veri aktarımı, yedek ve paylaşım — 3 sayfa

Bir sayfa yüzey tablosu + iki sayfa kısa anlatım.

**Bölümün ortak sınırı, bir kez yazılır:** Seçenek görmek dosya üretmek değil; dosya üretmek teslim
etmek değil. Ürün satırlarında yalnız o satıra ait sınır yazılır, bu cümle tekrarlanmaz.

1. **Çıktı seçimi yüzeyi** HD Bildiri: dönem + PDF/EXCEL, defter başına da var · BC PDF/Yazıcı/
   Excel(.csv)/HTML · MM "Excel(.xlsx) e-posta olarak gönder" · KolayBi cari ekstrede yedi isteğe
   bağlı kolon + PDF önizleme
2. **İçe aktarma yüzeyi** BC Veri Yönetimi'nde Excel(.csv) ve QIF içe aktarma · KolayBi listelerinde
   "İçe Aktar" girişleri. Dosya şeması, eşleme ve sonuç denenmedi
3. **Ek dosya ve okuma** HD Fatura ekle (kamera/galeri/PDF), kayıt ataçı, tam ekran görüntüleme ·
   WL dosya/foto ekleme · KolayBi gider ekinde tür listesi ve 5 MB sınırı. **Fiş okuma (OCR) bu
   maddenin sahibidir:** Paraşüt ve Logo'nun kaynak beyanlarında fiş fotoğrafından okuma anlatılıyor;
   çalışırken görülmedi. Ek dosya saklamak, dosyadan veri okumak değildir
4. **Yedek daveti ve veri konumu** HD "kayıtları sunucumuzda saklamıyoruz" diyaloğu; Drive yedek yolu
   denenmedi · MM ve BC'nin **araştırılan kurulumları** yerel ve girişsiz · WL bulut hesabı
5. **Üçüncü kişi erişimi** WL Group sharing ve Bank Sync girişleri · GB household. **Kaynak beyanı:**
   Logo müşavir portalı, Paraşüt muhasebeci canlı görüntüleme, KolayBi çok müşterili muhasebeci
   erişimi

**Dosya üretimi hakkında doğru ifade:** Hesap Defterim'de PDF/Excel üretimi ve dosya adları koşum
kaydında var, Android paylaşım yüzeyi de görüldü. Eksik olan, dosya içeriğinin kalıcı karesi ve
alıcıya teslim kanıtıdır. Diğer ürünlerde dosya üretimi denenmedi.

### Bölüm 12 · Diğer modüller ve yardımcı araçlar — 3 sayfa

Tek büyük tablo: ürün × modül. Satırlar — stok/depo · personel ve maaş · çek/senet · ürün ve hizmet
kartı · not defteri · takvim · nakit hesap makinesi (kupür sayımı) · alışveriş listesi · garanti ·
sadakat kartı · seyahat modu · km takibi.

Her hücrede yalnız: var / yok / görülmedi + kanıt türü. Anlatım iki paragrafı geçmez.

**Amaç:** rakibin özellik yüzeyini haritalamak. Bir modülün rakipte bulunması bizde ihtiyaç olduğu
anlamına gelmez; alma filtresi Belge 3'te.

### Bölüm 13 · Ortak tercihler ve ayrışmalar — 3 sayfa

Yeni kanıt yok; önceki bölümlerin çapraz okunması. Üç başlık:

- **Çoğunda aynı olanlar** — örneğin **dört canlı üründe** kayıt başlatmanın sağ altta tek düğme
  olması (Hesap Defterim'de iki yön düğmesi)
- **İkiye ayrılanlar** — sabit sekme / çekmece; ekstre dönemli kart / dönemsiz negatif bakiye
- **Tek üründe görülenler** — HD'nin yön adı taşıyan düğmeleri, MM'nin Bu Ay/Gelecek Ay sütunları,
  BC'nin sözcükle yazılan gecikme durumu

Her madde ilgili bölüme ve şekle bağlanır. Değerlendirme veya sıralama yapılmaz.

---

## 6 · Biçim kuralları

### Sayfa kalıpları

| Kalıp | Ne zaman | Yapısı |
|---|---|---|
| **A · Soru sayfası** | Varsayılan | Soru başlığı → 1–3 işaretli kare → sağ sütunda numaralı notlar → sınır maddeleri → altta dayanak satırı |
| **B · Yan yana karşılaştırma** | **Yalnız genel karşılaştırma** | Kareler eşit yükseklikte yan yana, altlarında ürün adı ve şekil numarası. **Beşli dizilim yalnız bütünü kavratmak için**; ayrıntı okunacaksa 2+3 yerleşim veya ardışık sayfalarda büyütme |
| **C · Şerit** | Aynı bölgenin ürünlerdeki yeri | Aynı genişlikte kırpıntılar alt alta, sağda kısa açıklama. **Her kırpıntı konumunu yazar** (ekranın üstü/altı) ve tam ekran şekline gönderme yapar |
| **D · Kart** | Bölüm 2 | Ürün başına sabit alan listesi, değişken uzunluk |
| **E · Tablo** | Bölüm 12 ve yüzey haritaları | Ürün × özellik, hücrede var/yok/görülmedi + kanıt türü |

A4 **yatay** sayfada beş telefon karesi ~4,3 cm genişliğe iner. Ana ekran gibi blok yapısı okunan
ekranlarda bu yeterli; form alanı gibi metin okunan ekranlarda değildir. Kalıp B'nin sınırı budur.

### İşaret kuralları

- İşaret **gösterdiği şeyin üstünü kapatmaz**; boş alana konur
- Numaralar soldan sağa, yukarıdan aşağıya ilerler; her numaranın metinde karşılığı vardır
- Karartma yalnız kişisel veri için; özgün kanıt değişmez, teslim kopyası da karartılır

### Yazım kuralları

- Ürün arayüz etiketleri **ekranda göründüğü dilde** yazılır (ENVELOPES, Alındı, Hızlı İşlemler)
- Nicelik belirteci ("her ekranda", "tek ürün", "hiçbirinde") kullanılacaksa o kapsamı taşıyan
  kanıt gösterilir; gösterilemiyorsa cümle gözlenen kapsamla sınırlanır
- "Görülmedi" ile "yok" ayrı şeylerdir ve karıştırılmaz
- Ölçülmemiş etki cümlesi kurulmaz; gerekiyorsa "olası etki — ölçülmedi" etiketiyle ve seyrek
- Ayrıntının tek sahibi vardır; başka bölümde kısa bağlam ve gönderme serbesttir (Ö7)

### Denetim ve sınırları

Üretim betiği şunları kontrol eder ve hata varsa **durur**: anılan her E kimliği sözlükte ve diskte
var mı · özgün karelerin hash'i envanterle aynı mı · metindeki her `Şekil N.M` göndermesinin karşılığı
basılı mı · sığmayan metin var mı · PDF **metninde** kişisel ad geçiyor mu.

**Betiğin yapamadıkları — bu yüzden elle kontrol zorunludur:**

- İddianın kanıttan gerçekten çıkıp çıkmadığını doğrulayamaz. `iddia-tablosu.md`'nin aynı kaynaktan
  üretilmesi **tutarlılık** sağlar, bağımsız doğruluk denetimi sağlamaz
- **Görselin içine gömülü kişisel adı yakalayamaz.** Metin araması yalnız PDF metnine bakar
- Bu nedenle her bölümün bütün sayfaları görüntüye çevrilip tek tek incelenir; bu adım atlanamaz

Her bölüm klasöründe betiğin ürettiği üç dosya bulunur: `iddia-tablosu.md` · `eksik-listesi.md` ·
`kaynaklar.md` + `kanit-manifest.json`.

**Mevcut test verisi silinmez veya sıfırlanmaz.** Yeni kurulum, oturum kapatma veya yeni hesap
gerektiren eksikler "Önerilmez" olarak işaretlenir.

### Bir bölüm ne zaman biter

1. Alt soruların hepsi yazıldı; kanıtı olmayan ürünler için "görülmedi" yazıldı
2. İddia tablosu ve eksik listesi üretildi
3. Betiğin beş otomatik kontrolü temiz
4. Bütün sayfalar görüntüye çevrilip tek tek incelendi (görsel içi kişisel veri dâhil)
5. "Bu bölüme girmez" listesindeki hiçbir konu bölüme sızmadı

---

## 7 · Açık işler

| # | İş | Gereken |
|---|---|---|
| 1 | Bölüm 4'ün beş sorusu için mevcut karelerin taranması | Tarama sonunda yeni koşum gerekip gerekmediği belli olur |
| 2 | Wallet'ın bekleyen ödeme kartının yüklenmiş hâli | Ana ekranı açıp kart yüklenene kadar beklemek — mevcut veriyle kapanır |
| 3 | Bluecoins'te dolu hesapta açılışta hangi sekmenin seçili geldiği | Uygulamayı tamamen kapatıp açmak |
| 4 | Hesap Defterim'de çok defterliyken açılış defteri | Aynı yöntem |
| ~~5~~ | ~~Bölüm 5.2 için beş formun karelerinin tam olup olmadığı~~ | **Kapandı (16 Eylül).** Beşinin de ana işlem formu karesi var: MM **E0229** · BC **E0020** · WL **E0277** · HD **E0138** · GB **E0116**. Koşum gerekmiyor |

**Yazım sırası:** 3 → 5 → 6 (ağırlar önce; biçim onlarda oturur) → 2 → 4 → 7 → 8 → 9 → 10 →
11 → 12 → 1 → 13 → Ek. Okuma kılavuzu ve ortak tercihler en sona kalır.

---

## 8 · v2'de değişenler

Dış inceleme turunun bulguları ve kendi denetimimden çıkanlar.

| # | Değişiklik | Kaynak |
|---|---|---|
| 1 | **Kanıt anahtarı dörtten beşe çıktı**; "kaynak beyanı" eklendi | Ürün kılavuzu ve yardım merkezi metinleri hiçbir türe girmiyordu |
| 2 | **Paraşüt/Logo gerekçesi düzeltildi.** "Hepsi video karesi" genellemesi yanlıştı: Logo'nun 4, Paraşüt'ün 2 karesi canlı yakalanmış giriş yüzeyi. Gerekçe "iç arayüzü temsil etmiyorlar" oldu | Dış inceleme; ben de dosya listesinde doğruladım |
| 3 | **"HD'nin çöp kutusu tek geri alma yüzeyi" düzeltildi** — Bluecoins'te de Çöp Kutusu var | Dış inceleme; Bluecoins çekmece karesinde doğrulandı |
| 4 | **"Hiçbirinde gerçek dosya üretilmedi" düzeltildi** — HD'de üretim ve dosya adları koşum kaydında var; eksik olan kalıcı kare ve teslim kanıtı | Dış inceleme |
| 5 | **"Esnaf tek başına giremiyor" düzeltildi** — tek koşumun gözlemi olarak sınırlandı | Dış inceleme |
| 6 | **Boş ekran karşıtlığı yumuşatıldı** — BC de sekme ve bakiye alanını, MM de dönem/özet/sekme/düğmeyi koruyor. Gerçek ayrım hangi sözcüklerin bulunduğu | Dış inceleme; karelerde doğrulandı |
| 7 | **Belge 1/2 sınırı dört yerde geri çekildi** (6.2, 9.2, 10.2, 10.5): alan, etiket ve düğme kalır; "şu toplam şu kadar değişti" Belge 2'ye gider | Dış inceleme |
| 8 | **"Beş farklı olgunluk düzeyi" → "beş farklı kart sunumu ve erişim durumu"** | Dış inceleme; puanlama çağrıştırıyordu |
| 9 | **Bölüm 3'te 3.2 ve 3.3 birleşti** (9 sayfa); Wallet'ın ana ekranı için tek cümlelik bağlam eklendi | Dış inceleme; tekrar riski |
| 10 | **"İkinci kez anlatılmaz" kuralı Ö7 ile değiştirildi:** ayrıntının tek sahibi olur, gönderme serbest | Dış inceleme; eski kural ana ekran haritasını eksik bırakıyordu |
| 11 | **Kalıp B'ye sınır kondu:** beşli dizilim yalnız genel karşılaştırma; form gibi metin okunan ekranlarda 2+3 | Dış inceleme; A4 yatayda beşli ~4,3 cm |
| 12 | **Kalıp C'ye konum kuralı eklendi:** kırpıntı tek başına kontrolün üstte mi altta mı olduğunu anlatmaz | Dış inceleme |
| 13 | **Denetimin sınırı yazıldı:** betik iddianın kanıttan çıktığını doğrulayamaz; görsele gömülü adı yakalayamaz; elle görsel kontrol zorunlu | Dış inceleme |
| 14 | **Bölüm 11'e içe aktarma maddesi eklendi** (BC CSV/QIF, KolayBi İçe Aktar); **OCR'ın sahibi 11.3 olarak netleşti** | Dış inceleme; iki kapsam boşluğu |
| 15 | "Seçenek–dosya–teslim" sınırı her maddede değil, bölüm başında bir kez | Dış inceleme; okunurluk |
| 16 | Nicelik belirteçleri kapsamlandı: "tek üründe" → "incelenen beş canlı üründen yalnız birinde"; "hemen hepsinde" → "dört canlı üründe" | Kendi denetimim |
| 17 | Editoryal ifadeler nötrleştirildi ("en öğretici sayfa", "en önemli gözlem", "esnaf için doğrudan ilgili") | Kendi denetimim |
| 18 | Sayfa tahmininin nasıl çıktığı yazıldı; toplam 79 → 78 | Dış inceleme |
