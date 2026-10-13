# Görev — Kategori özeti, kategori sayfası, hesap sayfası, Hesaplar ve Diğer

13 Ekim 2026. Bu belge işi uygulayacak agent içindir. Ne istendiğini, nereye
bakılacağını ve neyin gerçekten değişmez olduğunu söyler; nasıl yapılacağını
söylemez. Bu repoda işler böyle yürür: plan birlikte yapılır, uygulama senin
aklına bırakılır. Daha iyi bir yol görürsen onu seç; yolda bir hata bulursan
düzelt; eksik bir şey görürsen ekle. Tek şart **not almandır** (§7).

## 1. Önce oku

1. `AGENTS.md` (çalışma kuralları) ve `CLAUDE.md` (özeti).
2. `docs/project-status.md` "Aktif konum"un son iki maddesi ve
   `research/DEVIR-2026-10-12.md` §9.
3. `stages/06.3-butunsel-duzenleme.md` Grup 8 §3: **KÖ1–KÖ14** (kategori),
   **KH1–KH9** (hesap), "İşlem adları" paragrafı.
4. Tasarım teslimi `design_handoff_secici/`: önce `README.md`, sonra
   `brif/00`, `brif/05`, `brif/06`, `screenshots/` (53 kare), `screens-v9/*.jsx`.
5. Kararların çizildiği kaba panolar (Git dışı, yerelde):
   `tasarim-onizleme/liste-sistemi/hesap/tur-2-kararlar/` (özellikle `06`, `07`,
   `09`, `10`, `11`) ve `…/kategori/tur-5-son-hal/`. Görsel dilleri bağlayıcı
   değildir; hangi durumda ne yazdığı ve sayıların nasıl tuttuğu bağlayıcıdır.
6. `documentation/adr/0014`, `0019`, `0020` ve `documentation/design-system.md`.

**Hangisi geçerli.** Kural ve sayı için KÖ/KH kararları; yerleşim, metin ve
görünüş için teslimin `README.md`si ve kareleri. Kullanıcı tasarım sırasında
dört kararı bilerek değiştirdi; aşağıdaki tablo geçerlidir ve aşama belgesi bu
işle birlikte güncellenir:

| Karar | Aşama belgesinde yazan | Geçerli olan |
|---|---|---|
| KH2 | `Yolda` tek satırlık şerit | Bakiye kartının altında mavi kutu: toplam, en yakın tek satış, fazlası `Tümünü gör · N satış` |
| KH6 | Ara başlık toplamını yazar ve seçilebilir | Yön seçiliyken ara başlık yalnız etikettir. `Kategori`, `Tahsilat`, `Ödeme`, `Transfer` yalnız `Tümü` menüsünde satırdır ve oradan seçilir (kare H4, H8) |
| KH7 | Kart `Yolda ₺…` yazar | Kart yolda yazmaz; yoldaki para yalnız kutudadır |
| KH9 | Hesaplar satırında yoldaki para; transfer formu panel | Hesaplar'da yolda bilgisi yok; `Yeni transfer` bir sayfadır |

**Teslimin örnek verisi uydurmadır ve tasarım bizim kayıt kurallarımız
bilinmeden çizildi.** Görsel dili ve metinleri izle; sayının ve alanın anlamını
sunucu gerçeğiyle doğrula. Kare ile sunucu gerçeği çelişirse **sunucu gerçeği
geçerlidir**: ekranı ona uydur ve neyi değiştirdiğini not al (kullanıcı kararı,
13 Ekim 2026). Bilinen çelişkiler §5'te.

## 2. Bugün ne var, ne yok

**Var (yeniden yazma, genişlet):**

- `src/BusinessFinance.Infrastructure/Reports/RecognizedItems.cs`: gelir ve
  gider yazan sekiz kaynağın tek listesi. Bugün yalnız gün, tür, kapsam,
  kategori ve tutar taşır.
- `src/BusinessFinance.Infrastructure/Accounts/AccountMovements.cs`: hesabı
  hareket ettiren dokuz yolun tek listesi ve `FlowAsync`.
- Birleşik akış ve ayrıntı: `EfFinancialActivityRepository`,
  `GET /api/v1/financial-activities/{kind}/{id}/balances`; Flutter'da
  `lib/features/activities/` (satır, ayrıntı paneli, `ActivityKind.label`).
- POS: `PosSettlement`, `PosDeposit`, yoldaki para (`PosSettlementListDto`).
- Flutter ortak parçaları `lib/core/widgets/` altında ve
  `AppFinanceIcons.forCategory` (kategori ikonu addan gelir; sunucuda ikon alanı
  yok).
- Ekranlar: `features/more/presentation/more_page.dart`,
  `features/accounts/presentation/accounts_and_transfers_page.dart` (iki
  sekmeli), `features/categories/presentation/categories_page.dart`, Özet'teki
  `Kategori giderleri` kartı (`features/dashboard/`).

**Yok (bu işin konusu):**

- Kategoride rol. Dört kategori **adla** aranıyor: `Kasa farkı`
  (`CashCountContracts.DifferenceCategoryName`), `Faiz ve finansman gideri` ve
  `Faiz geliri` (`EfCategoryRepository`), dekont ücretinde `Diğer gider`
  (`lib/features/activities/data/receipt_fee_writer.dart`). Alacak taksidinin
  faizi kategorisizdir.
- Transferde masraf. `Transfer` yalnız tutar taşır; dekont ücreti istemcide ayrı
  bir gider olarak yazılıyor.
- Kategori özeti, kategori sayfası, hesap sayfası, yoldaki satışlar ve ay
  seçici için okuma uçları; `Hesaplar` toplamları.
- Bu sayfaların Flutter tarafı.

## 3. Değişmeyecek olanlar

Bunlar azdır ve gerçekten değişmez. Biri kodda somut bir zarar doğuruyorsa
(çifte sayım, tutmayan toplam, veri kaybı) körü körüne uygulama; dur ve
seçenekleriyle getir.

- **Para doğru sayılır.** Aynı para iki kez sayılmaz (ADR 0014 tanır / taşır).
  Satırlar üstteki sayıyı tutar: menü satırlarının toplamı `Hepsi`ye, kategori
  satırlarının toplamı karta, hesap satırlarının toplamı `giren` ve `çıkan`a
  eşittir. Bu eşitlikler testle korunur.
- **İstemci para toplamaz.** Toplam, pay, çubuk oranı, menü tutarı, ay sonu
  bakiyesi ve formdaki "toplam ₺… çıkar" cümlesinin tutarı sunucudan gelir.
- **Gelir/gider yazan yol `RecognizedItems`te, para yolu `AccountMovements`te
  bir kez tanımlıdır.** Bir okumaya ikinci bir kaynak listesi yazılmaz.
- **Kayıt silinmez, iptal edilir;** her okuma ve yazma sahiplik kapsamlıdır;
  para `decimal(19,4)` ve sözleşmede dört ondalıklı dizedir.
- **Var olan veri bozulmaz.** Migration `AGENTS.md` kurallarına uyar; eski
  yedek okunmaya devam eder (yedek sürümünün tek kaynağı
  `documentation/restore-runbook.md`). Veri kaybettiren bir şema kararı ve var
  olan bir sözleşmeyi kıran değişiklik önce sorulur.
- **Ürün sınırı:** ön muhasebe özelliği, vergi hesabı ve "kâr" kelimesi yok.
- **Git:** iş `kategori-hesap-sayfalari` dalında yapılır (`ui-trials`tan
  açıldı; bu belge ve tasarım teslimi ilk commit'idir). Başka dal açma, dal
  değiştirme, `ui-trials` ya da `main`e birleştirme ve push yok; commit mesajı
  İngilizce ve yapay zekâ imzasız (`AGENTS.md` "Git sorumluluğu").

## 4. Serbest olduğun yerler

Aşağıdakilerin hepsi sende; daha iyisini görürsen seç ve not al:

- Uçların adı, sayısı ve biçimi; sınıf, sorgu ve tablo yapısı; paketlerin iç
  sırası ve bir paketin kaç commit olduğu.
- Hangi testin gerektiği ve nasıl yazılacağı.
- Var olan kodu yeniden düzenlemek (işi kolaylaştırıyor ya da bir tutarsızlığı
  kapatıyorsa).
- Yolda bulduğun hatayı düzeltmek, eksik bir denetimi ya da hâli eklemek.
- Karede çizilmemiş bir hâli teslimin diliyle kurmak.
- Karar verilmemiş bir ayrıntıda en makul yolu seçmek.

Kullanıcının gördüğü bir davranışı **kararların tersine** değiştirecek bir şey
düşünüyorsan uygulamadan önce sor; onun dışında sormadan ilerle.

## 5. Kararı verilmiş çelişkiler

1. **`Kategorisi yok` (kare A8–A8d).** Teslim, kategorisiz kayda dokunup
   `Kategori seç` paneliyle kategori atamayı ve `Geri al`ı çiziyor. Bizde gelir
   ve gider kategorisiz yazılamaz, yazılmış kaydın kategorisi değiştirilmez
   (düzeltme = iptal + yenisi) ve Paket 1'den sonra faiz de kategorisini rolden
   alır; satırın görüneceği tek durum eski veridir. **Karar:** A8 satırı ve A8b
   listesi kurulur, liste salt okunurdur (satıra dokununca işlemin ayrıntısı
   açılır; "üzerine dokunun" notu ve `Kategori seçilmeli` yazısı çizilmez).
   A8c ve A8d kurulmaz.
2. **`Hesaplar` toplamı ve pasif hesap (kare D5).** Kare pasif hesap için
   `Toplama girmez` diyor. Bizde pasif hesaba yeni kayıt yazılamaz ama bakiyesi
   kalabilir ve Özet'in hesap bakiyeleri ile net varlık onu sayar
   (`GetAccountBalancesAsync`). **Karar:** `Toplam bakiye` bütün hesapları
   sayar, Özet'le aynı sayıdır. Karttaki `Nakit` ve `Banka` aktif hesaplardır
   ve altlarındaki grup başlıklarıyla aynı sayıyı yazar; pasiflerin bakiyesi
   sıfır değilse karta üçüncü hücre (`Pasif ₺…`) gelir ki hücreler toplamı
   tutsun. `Pasif hesaplar · N` satırı `Toplama girmez` yazmaz; bakiye sıfır
   değilse tutarı yazar. Olağan durumda (pasif bakiye sıfır) ekran kareyle
   aynıdır.
3. **Transfer sayfasında kapsam (kare D3).** Çizilmemiş: çıkan hesabın
   varsayılan kapsamı yoksa ve masraf yazıldıysa masrafın kapsamı sorulur;
   var olan kapsam bölümüyle (`AppScopeSection`, `previewResolvedScope`) kur.
4. **İşlem ayrıntısı (kare D1, D2).** Ayrıntı **bu sayfalara özel bir panel
   değildir**; İşlemler'deki panelin (`activity_detail_sheet.dart`) kendisidir
   ve her sayfadan aynı panel açılır. Kareler eklenen satırları ve dizilişi
   gösterir, panelin bugünkü satırlarını eksiltmez: karede olmayan ama bugün
   panelde olan satır (işlem sonrası bakiye ve renk kuralı, kalan, kalan limit,
   iptal düğmesi ve kilitleri) **kalır**. Ölçüt 12 Ekim'in kararlarıdır (KH5 ve
   pano `06`):
   - Transfer: çıkan hesap, giren hesap, `Masraf` ve `Masrafın kategorisi`
     (masraf varsa), iki hesabın işlem sonrası bakiyesi. Başlıktaki tutar
     transferin kendisidir (masraf hariç).
   - POS yatışı: paranın geçtiği hesap, POS, kapattığı satışlar (tek satışsa
     günü, birkaç satışsa her biri), komisyon, kesinti (varsa), hesaba geçen,
     işlem sonrası bakiye. Kare D2'de bakiye satırı yok; panelde kalır.
   - Bu işte satırı değişen öbür işlemler (faizli borç taksidi, komisyonlu
     kartlı satış ve kartla tahsilat, masraflı transfer) için de aynı ölçüt:
     satırda tutarın altında yazan kısım ayrıntıda kendi satırıyla ve
     kategorisiyle görünür.
   Karede bir satırın yeri ya da adı bu ölçütle çelişirse ölçüt geçerlidir;
   neyi nasıl kurduğunu not al.

## 6. İş paketleri

Sıra bir öneridir: her paket bir öncekinin üstüne kurulur ve sunucu Flutter'dan
önce gelir. Her checkpoint derlenen ve testleri geçen bir commit'tir. İş **tek
teslimdir**; arada durup onay bekleme.

### Paket 1 — Kategoride rol ve faiz (KÖ8)

Dört rol: faiz gideri, faiz geliri, kasa farkı, banka masrafı. Arama adla değil
rolle yapılır; ad serbesttir; rol taşıyan kategori iki tarafa açıktır ve
daraltılamaz; kullanıcı başına rol başına en çok bir kategori; kategori yoksa
ilk gerektiğinde açılır (`Kasa farkı`nın bugünkü deseni). Alacak taksidinin
faizi `Faiz geliri`ne bağlanır. Var olan kullanıcıda rol bugünkü adlarla
eşlenerek doldurulur; `Faiz geliri` iki tarafa genişletilir; işletme setinde
`Banka ve POS komisyonu` banka masrafı rolünü alır ve iki tarafa açılır,
kişisel sete `Banka masrafı` eklenir. Kategori cevabı rolü taşır. Rol yedeğe
girer; rolsüz eski yedek ada göre eşlenir.

### Paket 2 — İki tek listenin genişlemesi (KÖ2, KÖ6, KÖ12, KH1, KH3)

`RecognizedItem` kalemin nereden geldiğini taşır: kaynağın türü ve kimliği,
bağlı olduğu kayıt (komisyonun satışı, kesintinin yatışı, faizin taksidi,
masrafın transferi), hesap, kart, kişi ve giriş anı. `AccountMovement` satırın
neyi anlattığını taşır: türü, kaynağı, karşı hesap, kategori ve tutarın
içindeki kısım (masraf, kesinti, faiz).

Hesap satırları ve dönem toplamı KH3'e uyar: yatış, kesintisi düşülmüş tek
harekettir; masraflı transfer, masrafıyla birlikte tek harekettir. Net
değişmez. Kasa'nın günlük giren/çıkanı da aynı listeyi okur; oradaki sayılar
değişiyorsa not al.

### Paket 3 — Transfer masrafı ve dekont ücreti (KÖ9, KÖ10)

`Yeni transfer` isteğe bağlı masraf alır. Masraf sıradan bir gider kaydıdır ve
transfer onu kimliğiyle taşır (yatış kesintisinin deseni): tek yazmada
kaydedilir, akışta ayrı satır olmaz (transferin `feeAmount` alanı), tek başına
iptal edilemez, transferle birlikte iptal olur. Hesabı çıkan hesaptır;
kategorisi banka masrafı rolüdür; tarafı `TransactionScopeResolution`
zincirinden gelir (§5 madde 3). Formun bilgi kutusu (kare D3b) sunucudan bir
önizlemeyle dolar (POS önizlemesinin deseni).

Dekontun `Harcama`, `Kart ödemesi` ve `Geri bekliyorum` yollarında ücret kendi
gider satırı olarak kalır; değişen kategorisi (`Diğer gider` yerine banka
masrafı rolü) ve tarafıdır (kaynağın etiketi). Dekonttan açılan transferde
ücret `Masraf` alanına dolu gelir. Masraf alanı başka forma eklenmez.

Kayıt üreten her yolun uyduğu kurallar masraf için de geçerlidir: ileri güne
yazılmaz, yedek onu taşır, gün sonu ve kasa sayımı kilitleri ona da uygulanır.

### Paket 4 — Okuma uçları

Kararlı makine değerleri döner; cümleyi istemci kurar. Sınırlı sayıda sorguyla
çalışır (bellekte birleştirme ve sayfa başına bütün geçmişi okuma yok).
Sözleşmesi örnek JSON ile belgelenir.

- **Kategori özeti** (kare A1–A12): yön, ay, kapsam ve `Hesap / kart` seçimiyle
  ayın toplamı, aynı seçimle geçen ay, kategori sayısı, kategori satırları
  (tutar, pay, çubuk), varsa `Kategorisi yok` tutarı ve menü (`Nakit ve banka`,
  `Kredi kartı`, `Vadeli`; KÖ2, KÖ3, KÖ5, KÖ7).
- **Kategori sayfası** (B1–B10): kalemler gün gün; yükümlülükte durum
  (`Ödendi · gün · hesap` / `Açık`; cari borçlandırmada durum yok, KÖ4), parça
  kalemde bağlı kaydın türü ve tutarı (KÖ6), kartlı satışta `Yolda` /
  `Hesaba geçti`, iki kapsamlı kategoride satırın kapsamı (KÖ14);
  `Hesap / kart` ve `Kişi` menüleri birbirine uyar (KÖ12, KÖ13); seçim varken
  kategorinin tamamı.
- **Yılın ayları**: on iki ayın tutarı, sayfadaki seçimi izler; kaydı olan
  ileri ay seçilebilir, olmayan kapalıdır (KÖ11). Hesap sayfasında aynı parça
  ay sonu bakiyesini gösterir (kare H15).
- **Hesap sayfası** (H1–H13): bakiye (geçmiş ayda ay sonu ve bugünkü), ayın
  giren / çıkan / neti, yoldaki özet (toplam, sayı, en yakın satış ve beklenen
  günü; tutar hesaba geçecek olandır), satırlar (büyük tutar hesabın değiştiği
  tutardır; içindeki kısım ayrı alandır), `Tür` menüsü (H4–H8) ve seçimin
  toplamı.
- **Yoldaki satışlar** (H10): hesabın yoldaki kartlı satışları ve kartla
  tahsilatları; satırların toplamı kutudaki toplama eşittir.
- **Hesaplar** (D5, D6): §5 madde 2'deki toplamlar.
- **Ayrıntı ekleri**: §5 madde 4.

**Sunucu bitince, Flutter'a geçmeden önce kendin dene:** güncel kodu geçici bir
Release örneğinde ayrı bir portta kaldır (kullanıcının 5284'teki API'sine
dokunma), sentetik bir kullanıcıyla brif 5 ve 6'daki örnek veriyi yaz ve
uçların o tablolardaki sayıları verdiğini gör. Tutmayan sayı ya kodda ya
brifte bir hatadır; hangisi olduğunu bul ve not al.

### Paket 5 — Flutter: ortak parçalar ve Diğer

Teslimin "Ortak" bölümündeki parçalar bu sayfalara özel değildir; sıradaki
sayfalar (cari kişi, kredi kartı, POS listesi) da kullanacak. Var olan bileşeni
genişlet, kopyasını yazma: çipin altında açılan ara başlıklı menü, ortada
açılan ay seçici, gün başlığı (`25 Ekim Pazar`), işlem satırı (başlık, alt
yazı, durum satırı, tutar ve tutarın altı; İşlemler'in satırıyla **tek
bileşen**, KH4), özet kartı, boş hâl, bilgi notu. İşlem adı tek yerden okunur
(`ActivityKind.label` / `kindLabel`).

`Diğer` menüsü (kare K0): `Hesaplar ve transferler` → `Hesaplar`; yeni
`Kategoriler` grubu (`Kategori özeti`, `Kategori yönetimi`; ikincisi bugünkü
Kategoriler ekranıdır ve `Ayarlar`dan taşınır). Koşullu satırlar (`Kasa`,
`Bütçeler`, `Cari hesap`) bugünkü kurallarıyla kalır.

### Paket 6 — Flutter: Kategori özeti ve kategori sayfası

Kare A1–A12 ve B1–B10. Özet'teki `Kategori giderleri` kartı bu sayfayı ay ve
kapsamla açar. Kapsam çipleri `ScopeController.isVisible` yanlışsa çizilmez;
kategori sayfasında kapsam çipi yoktur (KÖ14).

### Paket 7 — Flutter: Hesap sayfası, Yolda, Hesaplar ve transfer

Kare H1–H15 ve D1–D6. `Hesaplar` tek listedir, `Transferler` sekmesi kalkar;
satıra dokununca hesap sayfası açılır; hesabın ayarları bugünkü düzenleme
formudur ve başlıktaki `tune` ikonundan açılır. `Yeni transfer` bir sayfadır;
`+ → Transfer` ve `Transfer yap` aynı sayfayı açar; geri ve kaydet geldiği yere
döner. İşlemler'de masraf ve faiz tutarın altına iner (D4). Transfer geçmişi
hesap sayfalarında ve İşlemler'in `Transferler` çipinde okunur.

Her ekranda: loading, boş, hata, yetkisiz ve bayat veri hâlleri; 2.0× yazı
ölçeği ve ekran okuyucu adları; `FinancialDataChanges` hedefleri; arayüz metni
doğal Türkçe, iç terim ekrana çıkmaz (`brif/05`, `brif/06` "Kelimeler");
`test/screenshots/` altında teslimin karelerini aynı çerçevede çizen görüntü
testleri.

## 7. Kontroller, belgeler ve rapor

**Kontroller (bitmiş sayılması için).** Backend: `build` 0 uyarı, `dotnet
format --verify-no-changes`, bütün testler gerçek SQL ile (bağlantı
`documentation/variables.md`). Flutter: `dart format --set-exit-if-changed lib
test`, `flutter analyze`, `flutter test`, debug APK.

**Belgeler davranışla aynı commit'te** (`AGENTS.md` "Belge güncelleme
haritası"): sözleşme, `permissions.md`, `flows.md`, `architecture.md`,
`tests.md`, `design-system.md`, `restore-runbook.md`, aşama belgesi (KH tablosu
ve "Sapmalar"), `docs/project-status.md`, `CLAUDE.md` özeti.

**Yol günlüğü.** Çalışırken şunları yaz (aşama belgesinin "Sapmalar" tablosu
ve teslim raporu):

- Karardan ya da kareden ayrıldığın her yer ve nedeni.
- Kareyi sunucu gerçeğine uydurduğun her yer (§1, §5).
- İstenmeden eklediğin her şey (uç, alan, denetim, test, yeniden düzenleme).
- Yolda bulup düzelttiğin her hata; bulup düzeltmediğin her hata.
- Teslimde çizilmemiş olup kurduğun her hâl.
- Kendi başına verdiğin her karar.

**Teslim raporu.** Ne değişti (paket paket, commit kimliğiyle); koşan
kontroller ve sayıları; yeni uçlar ve örnek cevapları; gerçek API denemesinin
sonucu (brifin sayılarıyla karşılaştırma); yol günlüğü; karar bekleyen sorular;
bakmadığın ya da deneyemediğin şeyler (açıkça "denemedim" diye). Kontrol ve
gerçek API denemesi iş bittikten sonra kullanıcının öbür oturumunda yeniden
yapılacak; raporun o incelemenin başlangıç noktasıdır.

## 8. Bu işin dışında

Cari kişi sayfası, kredi kartı sayfası, POS listesi, ödenen vergiler,
İşlemler'in genel filtresi; `docs/backlog.md` 13 (işlem adları), 14 (`Tek
seferlik`) ve 15 (kalemi atlamak); bütçe ekranı; kategori yönetimi ekranının
kendisi (yalnız menüdeki yeri değişir).
