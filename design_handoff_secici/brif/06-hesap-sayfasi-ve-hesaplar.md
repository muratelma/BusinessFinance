# Brif 6 — Hesap sayfası ve Hesaplar listesi

`00-ortak-cerceve.md` ve `05-kategori-ozeti-ve-kategori-sayfasi.md` ile birlikte okunur. Konu: bir hesaba
dokununca açılan **hesap sayfası**, `Hesaplar` listesi ve bu ikisinin değdiği iki yer (transfer formu, işlem
ayrıntısı). Hesap sayfasının bir çerçevesini daha önce çizmiştin (`Ziraat Vadesiz · Kategori menüsü`); bu brif
onu yeni kararlara göre düzenletiyor ve eksik hâlleri ekletiyor.

**Brif 5'teki ortak parçalar burada aynen kullanılır:** çipin altında açılan menü, ortada açılan ay seçici,
işlem satırı, boş hâl. Bu sayfaya özel bir kopya çizme.

## Neden

Bugün bir hesabın hareketleri hiçbir yerde tek başına okunamıyor: `Hesaplar` listesinde satıra dokununca
düzenleme formu açılıyor. Hesap sayfası bu boşluğu dolduruyor ve kategori özetinin tamamlayıcısı:

| Sayfa | Sorusu | Ne listeler |
|---|---|---|
| Kategori özeti | Ne kazandım, neye harcadım? | Yalnız gelir ve gider |
| Hesap sayfası | Bu hesaba ne girdi, bu hesaptan ne çıktı? | Hesabı hareket ettiren her şey |

Çizdiğin çerçeveyi gerçek kayıtlarla karşılaştırınca üç şey çıktı:

- **Menü yalnız kategorileri listeliyordu.** Hesabı hareket ettiren işlemlerin çoğunun kategorisi yok:
  transfer, kart ödemesi, tedarikçiye ödeme, fatura ödemesi, borç taksidi, POS yatışı. Örnekte menü satırları
  çıkanın üçte birini gösteriyordu.
- **`Giren · Satış geliri` satırı yanlıştı.** Kartlı satışın parası hesaba satış günü girmez; `POS yatışı`
  olarak, sonraki iş günü ve komisyonu düşülmüş girer.
- **Satırdaki tutar ile hesabın değişimi her zaman aynı değil.** Masraflı bir transferde hesaptan transfer
  tutarından fazlası çıkar; yatışta satışın tutarından azı girer.

## Elindeki çizimler (yerleşim değişmiyor)

- Başlıkta hesabın adı, sağ üstte ay (`Ekim 2026 ⌄`); ay seçici Brif 5'teki.
- Tek çip satırı: `Tümü · Giren · Çıkan` ve bir menü çipi.
- Kart: bakiye ve ayın giren / çıkan toplamı.
- Gün başlıkları altında işlem satırları.
- İlk turda beğenilen çerçeve: `tasarim-onizleme/liste-sistemi/tur-0-ilk-cizimler/1b-hesap-kendi-sayfasi.png`.

Kuralları ve sayıları gösteren kaba panolar: `tasarim-onizleme/liste-sistemi/hesap/tur-2-kararlar/` (07–11).
**Görsel dilini izleme**; yalnız hangi durumda ne yazdığına ve sayıların nasıl tuttuğuna bak.

## Değişmez kurallar

1. **Sayfa hesabı gerçekten hareket ettiren her şeyi listeler:** hesaptan yazılan gelir ve gider, transfer,
   kart ödemesi, tedarikçiye ödeme ve müşteriden tahsilat, fatura ödemesi, borç taksidi, POS yatışı.
2. **Satırlar toplamı tutar.** Listedeki satırların toplamı kartın `giren` ve `çıkan` tutarına, menü
   satırlarının toplamı seçili yönün toplamına eşittir. Örnek veride de böyle olmalı.
3. **Bu sayfa `giren / çıkan` der.** `Gelir` ve `gider` kelimeleri bu sayfanın toplamlarında kullanılmaz:
   transfer de girer ama gelir değildir.
4. **Kartlı satış iki ayrı yerde durur, hiçbir zaman ikisinde birden değil.** Parası henüz yatmamış satış
   listede **yoktur**; üstteki tek satırlık `Yolda` şeridinde sayılır (`Yolda · 4 satış · ₺28.300,00`) ve
   toplama girmez. Şeride dokununca yoldaki satışların listesi açılır. Para yatınca şeritten çıkar, listede
   `POS yatışı` satırı olur. Şerit kaç satış olursa olsun **tek satırdır**; İşlemler'deki `Planlananlar`
   şeridiyle aynı parçadır.
5. **Satırın büyük tutarı, hesabın o işlemle değiştiği tutardır.** Transfer masrafıyla birlikte (−₺10.050,00),
   yatış kesintisi düşülmüş (+₺7.169,00), taksit faiziyle birlikte (−₺1.000,00). İçindeki kısım **hep tutarın
   altında**, tek satırla yazar.
6. **Satır, İşlemler'deki satırın aynısıdır.** Başlık: açıklama, yoksa kişi, yoksa işlemin adı. Alt yazı
   başlıkta olmayanı söyler. **Bu hesabın adı satırda hiç yazmaz**; sayfanın başlığı o.
7. **`POS yatışı` kapattığı satışı yazar:** tek satışsa günü (`14 Eki satışı`), birkaç satışsa sayısı ve
   aralığı (`3 satış · 4–6 Eki`).
8. **Tek menü, ara başlıklı.** Giren: `Kategori`, `Tahsilat`, `Transfer`. Çıkan: `Kategori`, `Ödeme`,
   `Transfer`. Ara başlık kendi toplamını yazar ve **seçilebilir**. `Tümü` seçiliyken menü yalnız ara
   başlıkları (iki yön altında) listeler; `Giren` ya da `Çıkan` seçiliyken ara başlıkların altındaki satırları
   da. İkinci bir menü çipi ya da ek bir filtre satırı **yoktur**.
9. **Kart.** Seçili ay bu aysa `Bakiye`. Geçmiş ay seçiliyken üstte o ayın sonundaki bakiye, altında bugünkü
   bakiye. `Giren · Çıkan · Net` her zaman seçili ayındır. Yoldaki parası olan hesapta `Yolda ₺…` yazar.
10. **Kapsam çipi yoktur.** Bakiye `İşletme / Şahsi` diye bölünmez; Özet'teki anahtar bu sayfayı etkilemez.
11. **Satıra dokununca İşlemler'deki ayrıntı paneli açılır**; bu sayfaya özel panel yok. Panele iki ek gelir:
    transferde `Masraf` ve kategorisi; POS yatışında kapattığı satışlar ve hesabı (satış − komisyon − kesinti).
12. **Hesaplar tek listedir.** `Transferler` sekmesi kalkar. Satıra dokununca hesabın sayfası açılır. Hesabın
    ayarları (ad, tür, varsayılan kapsam, pasife alma) bugünkü düzenleme formudur ve hesap sayfasından açılır.
13. **Transfer iki yerden yapılır ve ikisi aynı formu açar:** her ekrandaki `+` düğmesi (`Transfer`'e
    dokununca araya sayfa girmeden form) ve `Hesaplar` sayfası.
14. **`Yeni transfer` formuna tek alan eklenir: `Masraf`** (isteğe bağlı). Masraf çıkan hesaptan çıkar, gider
    olarak sayılır ve transferle birlikte iptal olur. İşlemler'de transferin tutarının altında yazar.
15. Tutarları ve toplamları sunucu verir (`00-ortak-cerceve.md`).

## Kelimeler

- Ekranda: `Bakiye`, `Yolda`, `Tümü`, `Giren`, `Çıkan`, `Net`, `Kategori`, `Tahsilat`, `Ödeme`, `Transfer`,
  `Masraf`, `Transfer yap`, `Hesap ekle`, `Filtreyi kaldır`.
- İşlem adları **uygulamadaki adlardır**: `Transfer`, `Kart ödemesi`, `Cari ödeme`, `Cari tahsilat`,
  `POS yatışı`; taksitte `Borç taksidi ödemesi` ve `Alacak taksidi tahsilatı`; tek seferlik borcun ve
  alacağın kapanışında `Borç ödemesi` ve `Alacak tahsilatı`. **Bu adlar ileride değişebilir**; tasarım belirli
  bir kelimenin uzunluğuna yaslanmasın, en uzun adı da (`Alacak taksidi tahsilatı`) taşısın.
- Ekranda **geçmez:** bu sayfanın toplamlarında `gelir` / `gider`; "sayılmaz", "taşıyan", "hareket", "parça",
  "süzgeç", "kâr".
- Cümleler kısa ve doğal Türkçe. Uzun açıklama, büyük buton, büyük yazı yok.

## Çizilecek çerçeveler

**Düzenle** = elindeki çerçeve, yeni kural ve veriyle. **Yeni** = daha önce çizilmedi.

### H · Hesap sayfası

| # | Çerçeve | Durum |
|---|---|---|
| H1 | `Tümü`, bu ay: kart, `Yolda` şeridi, liste | Düzenle |
| H2 | `Giren` seçili | Yeni |
| H3 | `Çıkan` seçili | Yeni |
| H4 | Menü, `Tümü` seçiliyken: iki yön altında yalnız ara başlıklar | Düzenle |
| H5 | Menü, `Giren` seçiliyken: ara başlıklar ve satırları | Yeni |
| H6 | Menü, `Çıkan` seçiliyken (en uzun hâl) | Yeni |
| H7 | `Çıkan` ve bir satır seçili (`Borç taksidi ödemesi`): çip adı, daralmış liste, seçimin toplamı | Yeni |
| H8 | `Çıkan` ve bir ara başlık seçili (`Kategori`) | Yeni |
| H9 | Geçmiş ay (`Eylül 2026`): kartta ay sonu bakiyesi ve bugünkü | Yeni |
| H10 | `Yolda` şeridine dokununca: tek satış ve dört satış hâli | Yeni |
| H11 | Yoldaki parası olmayan hesap (`Dükkan Kasası`): şerit ve `Yolda` satırı yok | Yeni |
| H12 | Seçili ayda hareket yok: boş hâl | Yeni |
| H13 | Uzun hesap adı, uzun işlem adı ve büyük tutar | Yeni |

### D · Ayrıntı, form ve liste

| # | Çerçeve | Durum |
|---|---|---|
| D1 | Ayrıntı · Transfer: `Masraf` ve kategorisi eklenmiş | Düzenle (mevcut panel) |
| D2 | Ayrıntı · POS yatışı: kapattığı satışlar; tek satış ve üç satış hâli | Düzenle (mevcut panel) |
| D3 | `Yeni transfer` formu: `Masraf` alanı boş ve dolu; dolu hâlde çıkan toplamı söyleyen satır | Düzenle (mevcut form) |
| D4 | İşlemler'de iki satır: masraflı transfer ve faizli borç ödemesi (ikisi de tutarın altında) | Düzenle (mevcut satır) |
| D5 | `Hesaplar`: tek liste, sekmesiz; toplam, hesap satırları (ad, tür, bakiye, yoldaki para), pasif hesap, `Transfer yap`, `Hesap ekle` | Yeni |
| D6 | `Hesaplar`: hiç hesap yok (ilk kullanım) | Yeni |

## Senden karar önerisi beklediğim yerler

Emin olmadığın yerde iki seçeneği yan yana çiz, hangisini önerdiğini yaz.

1. **Menü çipinin adı.** İçinde hem kategoriler hem işlem türleri var; `Kategori` eksik kalıyor. Kaba panoda
   `Tür` yazdık. Seçim yapılınca çip seçilenin adını alıyor.
2. **Para taşıyan satırda yön.** Ortak çerçevede transfer ve yatış mavi ve işaretsiz. Bu sayfada girenle çıkan
   ayrışmalı ve yalnız renkle taşınmamalı. İşaret mi, ok mu, başka bir şey mi?
3. **`Yolda` iki yerde yazıyor:** kartta tutar olarak, listenin üstünde şerit olarak. İkisi birden gerekli mi,
   yoksa biri öbürünün işini görür mü?
4. **Kartın düzeni.** Bakiye, `Yolda`, `Giren · Çıkan · Net` ve geçmiş ayda iki bakiye. İlk çizimde giren /
   çıkan ayrı bir karttaydı.
5. **Hesabı düzenleme girişi.** Sağ üstte artık ay duruyor; ilk çizimde orada kalem vardı.
6. **Menüde ara başlık.** Seçilebilir olduğu nasıl anlaşılır; `Tümü` hâlinde iki yön nasıl ayrışır?
7. **Seçimin toplamı.** Bir satır ya da ara başlık seçilince (`Borç taksidi ödemesi · ₺4.000,00`) toplam nerede yazar:
   kartta mı, listenin başında mı?
8. **Tutarın altındaki satırın cümlesi.** Kaba panoda `₺50,00 masraf dahil`, `₺40,00 kesinti sonrası`,
   `₺150,00 faiz dahil`. Üçü farklı okunmalı (masraf eklenir, kesinti düşülür, faiz içindedir) ama aynı yerde
   ve aynı biçimde durmalı. Daha doğal bir ifade varsa öner.
9. **`Hesaplar` listesi.** `Transfer yap`ın yeri; yoldaki paranın satırda nasıl yazacağı; nakit ve banka
   hesaplarının gruplanıp gruplanmayacağı.

## Örnek veri (sentetik; `Ziraat Vadesiz`, Ekim 2026)

Brif 5'teki kayıtlarla aynı ay. Ay başı bakiye ₺25.000,00 · giren ₺27.936,00 · çıkan ₺31.560,00 ·
net −₺3.624,00 · **bakiye ₺21.376,00** · yolda ₺8.074,00 (25 Ekim'in ₺8.400,00'lük kartlı satışı, komisyon
₺326,00, 26 Eki Pazartesi bekleniyor).

### Liste (en yeni üstte)

| Gün | Başlık | Alt yazı | Tutar | Tutarın altında |
|---|---|---|---:|---|
| 28 Ekim | Hesap işletim ücreti | Banka ve POS komisyonu | −₺200,00 | |
| 26 Ekim | Test Toptancı | Cari ödeme | −₺2.500,00 | |
| 24 Ekim | Kart ödemesi | Bonus Kart | −₺3.000,00 | |
| 21 Ekim | Transfer | → Garanti Vadesiz | −₺10.050,00 | ₺50,00 masraf dahil |
| 20 Ekim | Elektrik faturası | Borç ödemesi | −₺1.110,00 | |
| 20 Ekim | Ahmet Usta | Borç taksidi ödemesi | −₺1.000,00 | ₺150,00 faiz dahil |
| 16 Ekim | Transfer | ← Dükkan Kasası | +₺12.000,00 | |
| 15 Ekim | POS yatışı | Ziraat POS · 14 Eki satışı | +₺7.169,00 | ₺40,00 kesinti sonrası |
| 10 Ekim | Web sitesi işi | Hizmet geliri | +₺3.000,00 | |
| 7 Ekim | POS yatışı | Ziraat POS · 6 Eki satışı | +₺5.767,00 | |
| 5 Ekim | Örnek Banka | Borç taksidi ödemesi | −₺3.000,00 | ₺450,00 faiz dahil |
| 5 Ekim | Toptancı ödemesi | Ticari mal alımı | −₺4.200,00 | |
| 1 Ekim | Ekim kirası | İşyeri kirası | −₺6.500,00 | |

Gelir ve gider satırları yeşil ve kırmızı (üç gider, bir gelir); öbür dokuzu para taşır.
Giren: 5.767 + 3.000 + 7.169 + 12.000 = 27.936. Çıkan: 6.500 + 4.200 + 3.000 + 1.110 + 1.000 + 10.050 + 3.000 +
2.500 + 200 = 31.560.

### Menü

**`Tümü` seçiliyken (H4):**

| Yön | Ara başlık | Tutar |
|---|---|---:|
| Giren · ₺27.936,00 | Kategori | +₺3.000,00 |
| | Tahsilat | +₺12.936,00 |
| | Transfer | +₺12.000,00 |
| Çıkan · ₺31.560,00 | Kategori | −₺10.900,00 |
| | Ödeme | −₺10.610,00 |
| | Transfer | −₺10.050,00 |

**`Giren` seçiliyken (H5):** `Hepsi` +₺27.936,00 · **Kategori** `Hizmet geliri` +₺3.000,00 · **Tahsilat**
`POS yatışı` +₺12.936,00 · **Transfer** `Dükkan Kasası` +₺12.000,00.

**`Çıkan` seçiliyken (H6):** `Hepsi` −₺31.560,00 · **Kategori** `İşyeri kirası` −₺6.500,00, `Ticari mal alımı`
−₺4.200,00, `Banka ve POS komisyonu` −₺200,00 · **Ödeme** `Borç taksidi ödemesi` −₺4.000,00, `Kart ödemesi`
−₺3.000,00, `Cari ödeme` −₺2.500,00, `Borç ödemesi` −₺1.110,00 · **Transfer** `Garanti Vadesiz` −₺10.050,00.

Transfer başlığının altındaki satır öbür hesabın adıdır.

### Ayrı küçük örnekler

- **H7:** `Çıkan · Borç taksidi ödemesi`: iki satır (20 Ekim Ahmet Usta −₺1.000,00, 5 Ekim Örnek Banka −₺3.000,00),
  toplam ₺4.000,00.
- **H8:** `Çıkan · Kategori`: üç satır, toplam ₺10.900,00.
- **H9 · Eylül 2026:** Eylül sonu bakiyesi ₺25.000,00, bugün ₺21.376,00; Eylül giren ₺31.200,00, çıkan
  ₺28.400,00, net +₺2.800,00.
- **H10 · tek satış:** `Kartlı satış · Ziraat POS`, 25 Ekim, ₺8.400,00, komisyon ₺326,00, hesaba geçecek
  ₺8.074,00, 26 Eki Pazartesi bekleniyor. **Dört satış hâli:** şerit `Yolda · 4 satış · ₺28.300,00`.
- **H11 · `Dükkan Kasası`:** nakit hesap; yolda yok. Girenleri `Nakit satış · Gün sonu` satırları, çıkanında
  16 Ekim'deki ₺12.000,00 transfer (`→ Ziraat Vadesiz`) var.
- **H13:** hesap `Türkiye İş Bankası Ticari Vadesiz`, satır başlığı `Örnek Elektrik Dağıtım A.Ş.`, tutar
  `−₺1.284.650,40`, altında `₺12.480,00 masraf dahil`.
- **D1 · Transfer:** 21 Ekim, ₺10.000,00, `Ziraat Vadesiz` → `Garanti Vadesiz`, masraf −₺50,00
  (`Banka ve POS komisyonu`); işlemden sonra Ziraat Vadesiz ₺27.076,00, Garanti Vadesiz ₺14.500,00.
- **D2 · POS yatışı, tek satış:** 15 Ekim, ₺7.169,00; 14 Eki kartlı satış ₺7.500,00, komisyon −₺291,00,
  kesinti −₺40,00, hesaba geçen ₺7.169,00. **Üç satış:** ₺2.000,00 + ₺2.500,00 + ₺1.500,00 = ₺6.000,00,
  komisyon −₺233,00, kesinti yok, hesaba geçen ₺5.767,00.
- **D5 · Hesaplar:** `Dükkan Kasası` Nakit ₺8.454,00 · `Ziraat Vadesiz` Banka ₺21.376,00 (yolda ₺8.074,00) ·
  `Garanti Vadesiz` Banka ₺14.500,00 · `Cüzdan` Nakit ₺4.000,00 · pasif `Eski Kasa` ₺0,00. Toplam ₺48.330,00.

## Teslim

`00-ortak-cerceve.md`deki gibi: çerçeveler ayrı bir sayfada, README'de çerçeve çerçeve ölçü, metinler ve
davranış. Brif 5'in ortak parçalarında bu sayfa için değişen bir şey varsa (menüde ara başlık, satırda yön,
`Yolda` şeridi) README'de **ortak parçanın yeni hâli** olarak yaz; iki sayfa için iki ayrı tarif olmasın.
Yeni token ekleme.
