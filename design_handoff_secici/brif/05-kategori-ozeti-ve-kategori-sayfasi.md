# Brif 5 — Kategori özeti ve kategori sayfası

`00-ortak-cerceve.md` ile birlikte okunur. Konu iki sayfadır: **Kategori özeti** ve bir kategoriye dokununca
açılan **kategori sayfası**. İkisinin de çoğunu daha önce birlikte çizdik; bu brif onları baştan
tasarlatmıyor, yeni kararlara göre **düzenletiyor** ve eksik hâlleri ekletiyor.

## Neden

Çizdiğin çerçeveleri gerçek kayıtlarla karşılaştırdık. Yerleşim doğru; ama örnek verinin gizlediği dört şey
çıktı:

- **Menü satırları toplamı tutmuyordu.** `Hesap / kart` menüsünde yalnız hesaplar ve kartlar vardı. Oysa bazı
  giderlerin hesabı yok: vadeli alım ve ödenmemiş fatura gider yazılır ama parası o gün hiçbir hesaptan
  çıkmaz. Onlar için bir satır olmayınca satırların toplamı `Hepsi`den küçük kalıyordu.
- **Seçimden sonra liste aynı kalmıştı.** "Seçim sonrası" çerçevesinde kart `Ziraat Vadesiz`e inmiş, altındaki
  kategoriler ve paylar değişmemişti. Gerçekte liste, pay ve kategori sayısı da değişir.
- **Olamayacak bir satır vardı.** `Hal alışverişi · Test Toptancı · Dükkan Kasası`: bizim kayıtlarımızda
  kişisi olan gider hesapsızdır (vadeli alım, fatura), hesabı olan giderin kişisi yoktur.
- **Bazı giderlerin kendi satırı yok.** POS komisyonu, yatış kesintisi, borç taksidinin faizi ve transfer
  masrafı İşlemler'de ayrı satır değildir; bağlı oldukları işlemin tutarının altında yazarlar. Kategori
  sayfasında nasıl duracakları çizilmemişti.

## Elindeki çizimler (yerleşim değişmiyor)

- **Kategori özeti:** başlık, sağ üstte ay (`Ekim 2026 ⌄`), `Gider | Gelir` rayı, tek çip satırı
  (`Hepsi · İşletme · Şahsi` ve `Hesap / kart ▾`), özet kartı (ayın toplamı, kategori sayısı), kategori
  listesi (ikon, ad, pay, tutar, çubuk, ok). Çerçeveler: `M2 · Başlık + grup + ikon`,
  `M2 · Çok hesap, uzun ad, büyük tutar`, `Seçim sonrası`.
- **Menü:** çipin hemen altında açılır; üstte seçimi söyleyen satır, `Hepsi`, sonra gruplar (`Nakit ve banka`,
  `Kredi kartı`). Seçim yapılınca çip seçilenin adını yazar.
- **Ay seçici:** ekranın ortasında açılır; yıl okları, on iki ay, her ayın altında çubuk, `Bu ay`, `Vazgeç`,
  `Göster`. Çerçeveler: `K2 · Çubuk rol rengi · gider` ve `· gelir`.
- **Kategori sayfası:** başlıkta kategorinin adı ve ay, kart (ayın tutarı, `Geçen ay`), çipler
  (`Hesap / kart ▾`, `Kişi ▾`), gün başlıkları altında satırlar. Çerçeve: `Ticari mal alımı · Kişi menüsü`.

Kuralları ve sayıları gösteren kaba panolarımız var: `tasarim-onizleme/liste-sistemi/kategori/tur-5-son-hal/`
(01–07). **Görsel dilini izleme**; yalnız hangi durumda ne yazdığına ve sayıların nasıl tuttuğuna bak.

## Değişmez kurallar

1. **Bu iki sayfa yalnız gelir ve gideri gösterir.** Transfer, kart borcu ödemesi, tedarikçiye yapılan ödeme,
   borç tahsilatı ve POS yatışı gelir ya da gider değildir; bu sayfalarda tutar olarak **yer almaz**.
2. **Her yerde satırlar üstteki sayıyı tutar.** Menü satırlarının toplamı `Hepsi`ye, kategori satırlarının
   toplamı özet kartındaki tutara, kategori sayfasındaki satırların toplamı o sayfanın kartındaki tutara
   eşittir. Örnek veride de böyle olmalı (aşağıdaki sayılar hazır).
3. **`Hesap / kart` menüsünün kuralı:** bir gider ya da gelir, kaydında yazan hesabın ya da kartın altındadır.
   Hiçbiri yazmıyorsa `Vadeli` satırındadır. Menü en altta, kendi grubunda tek bir `Vadeli` satırı taşır.
   **Ayrı bir POS grubu yoktur**: kartlı satış ve komisyonu POS'un bağlı olduğu banka hesabının altındadır.
4. **`Vadeli` bir durum değildir.** Ödenmiş fatura da o satırda kalır; satırın adı ve alt yazısı "henüz
   ödenmedi" anlamına gelmemelidir.
5. **Seçim her şeyi daraltır:** özet kartı, kategori listesi, paylar ve kategori sayısı. Seçim varken bir
   kategori satırı kategorinin tamamını değil, **yalnız seçime düşen tutarı** yazar.
6. **Kategori sayfası özetteki seçimle açılır.** Sayfa kategorinin tamamını göstermiyorsa kart bunu söyler:
   etikette seçim (`Ekim gideri · Ziraat Vadesiz`), altında `Kategorinin tamamı ₺…`.
7. **Parça olan gider, bağlı olduğu işlemle gelir ve yalnız kendi tutarını yazar.** Komisyon kategorisinde
   kartlı satışın satırı durur, tutar olarak yalnız komisyon yazar, alt yazı satışın tutarını söyler
   (`₺8.400,00 satışın komisyonu`). Dokununca o işlemin ayrıntısı açılır. Aynı satış `Satış geliri`
   sayfasında kendi tutarıyla görünür.
8. **Fatura ödendiğini söyler, vadeli alım söylemez.** Ödenmemiş fatura olarak yazılan gider `Vadeli`dedir;
   satırı `Ödendi · 20 Eki · Ziraat Vadesiz` ya da `Açık` yazar (alacakta `Tahsil edildi`). Vadeli alım ve
   veresiye satış satırında böyle bir yazı **olmaz**: tedarikçiye yapılan ödeme belli bir alımı değil,
   toplam borcu düşürür.
9. **Bir satırda kişi ile hesap birlikte yazmaz.** Tek istisna borç taksidinin faizidir (borcun kişisi ve
   taksidin ödendiği hesap).
10. **Menüler birbirine uyar.** Bir çip seçiliyken öbürünün menüsü yalnız o seçimle birlikte kaydı olanları
    listeler. Seçecek bir şeyi kalmayan çip **soluk** durur. Boş liste oluşmaz.
11. **`Kişi` çipi yalnız o kategoride kişisi olan kayıt varsa çizilir.** Menüsünde `Kişi yok` satırı kalır
    (kişisi olmayan kayıtların toplamı).
12. **Kapsam.** Kategori özetindeki `Hepsi · İşletme · Şahsi` çipleri Özet'teki anahtarın aynısıdır;
    işletmesi olmayan kullanıcıda **hiç çizilmez**. Kategori sayfasında kapsam çipi **yoktur**. Kapsam yalnız
    sayfayı daralttığında kartta yazar (`Ekim gideri · İşletme`, altında `Kategorinin tamamı`). İki kapsamda
    da kaydı olan bir kategori `Hepsi` ile açıldıysa satırın alt yazısında kapsam yazar
    (`Bonus Kart · Şahsi`). Kategorilerin çoğu tek kapsamlıdır; onlarda kapsam ne kartta ne satırda yazar.
13. **`Kategorisi yok` satırı** kategori listesinin sonunda, yalnız kategorisi bulunamayan bir tutar varsa
    çizilir. Olağan durumda görünmez.
14. **Ay seçici sayfadaki seçimi izler:** yön, kapsam ve seçili hesap. Seçili ayın çubuğu ile sayfadaki tutar
    aynı sayıdır. Kaydı olan ileri ay seçilebilir; kaydı olmayan ileri ay kapalıdır.
15. **Ay değişince seçim korunur.** Seçili hesabın o ay kaydı yoksa sayfa boş hâlini ve `Filtreyi kaldır`ı
    gösterir; çip adını yazmaya devam eder.
16. **Bu sayfalarda bütçe ve düzenleme yoktur.** Kategori adı, ikonu ve kapsamı `Kategori yönetimi`nde
    düzenlenir.
17. Tutarları, payları ve çubuk boylarını sunucu verir (`00-ortak-cerceve.md`).

## Kelimeler

- Ekranda: `Kategori özeti`, `Gider`, `Gelir`, `Hepsi`, `İşletme`, `Şahsi`, `Hesap / kart`, `Nakit ve banka`,
  `Kredi kartı`, `Vadeli`, `Kişi`, `Kişi yok`, `Pay`, `Geçen ay`, `Kategorinin tamamı`, `Ödendi`, `Açık`,
  `Tahsil edildi`, `Kategorisi yok`, `Filtreyi kaldır`, `Yolda`, `Hesaba geçti`.
- Parça alt yazıları: `… satışın komisyonu`, `… tahsilatın komisyonu`, `… yatışın kesintisi`,
  `… taksidin faizi`, `… transferin masrafı`.
- Ekranda **geçmez:** "taraf", "iki tarafa açık", "kalem", "parça", "tanıyan", "taşıyan", "süzgeç",
  "hesapsız", "kâr".
- Cümleler kısa ve doğal Türkçe. Uzun açıklama, büyük buton, büyük yazı yok.

## Çizilecek çerçeveler

**Düzenle** = elindeki çerçeve, yeni kural ve veriyle. **Yeni** = daha önce çizilmedi.

### A · Kategori özeti

| # | Çerçeve | Durum |
|---|---|---|
| A1 | Gider, seçim yok: yedi kategori | Düzenle (veri) |
| A2 | `Hesap / kart` menüsü açık: iki grup ve en altta `Vadeli` | Düzenle |
| A3 | `Ziraat Vadesiz` seçili: kart, liste, pay ve kategori sayısı daralmış | Düzenle ("Seçim sonrası") |
| A4 | `Vadeli` seçili: iki kategori | Yeni |
| A5 | Gelir, seçim yok: üç kategori | Yeni |
| A6 | Gelirde menü açık | Yeni |
| A7 | Uzun hesap adı ve büyük tutar; menüde `Vadeli` de var | Düzenle (ikinci `M2`) |
| A8 | Liste sonunda `Kategorisi yok` satırı | Yeni |
| A9 | Ay seçici, `Bonus Kart` seçiliyken: başlık seçimi yazar, çubuklar ona göre; kayıtlı ileri ay ve kapalı ileri ay | Düzenle (`K2`) |
| A10 | Seçili hesabın kaydı olmayan ay: boş hâl ve `Filtreyi kaldır` | Yeni |
| A11 | O ay hiç gider yok (seçim de yok): boş hâl | Yeni |
| A12 | İşletmesi olmayan kullanıcı: kapsam çipleri yok, satırda yalnız `Hesap / kart` | Yeni |

### B · Kategori sayfası

| # | Çerçeve | Durum |
|---|---|---|
| B1 | `Ticari mal alımı`: dört gider, biri vadeli alım (kişi adı var, durum yazısı yok) | Düzenle |
| B2 | B1'de `Hesap / kart` menüsü açık | Düzenle |
| B3 | B1'de `Kişi` menüsü açık (`Kişi yok` satırıyla) | Düzenle |
| B4 | B1'de `Ziraat Vadesiz` seçili: kartta `Kategorinin tamamı`, `Kişi` çipi soluk | Yeni |
| B5 | `İşyeri faturaları`: `Ödendi · 20 Eki · Ziraat Vadesiz` yazan fatura satırı; yanında `Açık` hâli | Yeni |
| B6 | `Banka ve POS komisyonu`: komisyon, kesinti, transfer masrafı ve kendi satırı olan bir gider birlikte | Yeni |
| B7 | `Faiz ve finansman gideri`: iki borç taksidinin faizi; `Kişi` çipi var | Yeni |
| B8 | `Satış geliri`: kartlı satış (altında komisyon, yanında `Yolda` / `Hesaba geçti`), nakit satış, veresiye satış | Yeni |
| B9 | `Sigorta`, iki hâl yan yana: `İşletme` ile açılmış (kartta kapsam) ve `Hepsi` ile açılmış (satırda kapsam) | Yeni |
| B10 | Seçili ayda bu kategoride kayıt yok: boş hâl | Yeni |

## Senden karar önerisi beklediğim yerler

Emin olmadığın yerde iki seçeneği yan yana çiz, hangisini önerdiğini yaz.

1. **`Vadeli` satırının grubu ve alt yazısı.** Kaba panoda grup `Hesaptan geçmeyen`, alt yazı giderde
   `Vadeli alım, fatura ve borçla alınanlar`, gelirde `Veresiye satış ve alacaklar`. Daha doğal bir ifade
   varsa öner; kural 4'ü bozmasın.
2. **Parça satırı.** Satırda iki tutar var: işlemin tutarı (₺8.400,00) ve bu kategoriye düşen parça (₺326,00).
   Sağdaki büyük tutar parçadır. İşlemin tutarı nerede ve nasıl yazsın?
3. **`Ödendi` yazısı.** Satırın neresinde durur, `Açık`tan nasıl ayrışır? Bilgi yalnız renkle taşınmasın.
   Bu yazıyı başka sayfalarda da kullanacağız.
4. **Soluk çip.** Seçecek bir şeyi kalmayan çip nasıl görünür; dokununca bir şey söyler mi?
5. **Kartın alt satırı.** Seçim yokken `Geçen ay ₺…`, seçim varken `Kategorinin tamamı ₺…` yazıyor. İkisi
   birlikte gerekirse nasıl sığar; yoksa seçim varken `Geçen ay` hiç yazmasın mı?
6. **`Kategorisi yok` satırı.** Öbür kategorilerden nasıl ayrışır; dokunulabilir mi görünmeli?
7. **Ay seçicide ileri ay.** Kaydı olan ileri ay ile kaydı olmayanı ayıran görünüş.
8. **Uzun satır.** `Kartlı satış · Ziraat POS` + `Ziraat Vadesiz · Hesaba geçti` + tutar + `komisyon ₺291,00`
   2.0× yazı ölçeğinde nasıl kırılır?

## Örnek veri (sentetik; Ekim 2026, kapsam `İşletme`)

### Gider · ₺26.930,00 · 7 kategori

| Kategori | Tutar | Pay | Geçen ay |
|---|---:|---:|---:|
| Ticari mal alımı | ₺12.480,00 | %46 | ₺10.940,00 |
| İşyeri kirası | ₺6.500,00 | %24 | ₺6.500,00 |
| Personel giderleri | ₺4.200,00 | %16 | ₺4.200,00 |
| İşyeri faturaları | ₺1.860,00 | %7 | ₺1.640,00 |
| Banka ve POS komisyonu | ₺1.140,00 | %4 | ₺980,00 |
| Faiz ve finansman gideri | ₺600,00 | %2 | ₺600,00 |
| Kasa farkı | ₺150,00 | %1 | ₺0,00 |

`Hesap / kart` menüsü: `Hepsi` ₺26.930,00 · **Nakit ve banka** `Dükkan Kasası` ₺6.950,00, `Ziraat Vadesiz`
₺12.440,00 · **Kredi kartı** `Bonus Kart` ₺3.930,00 · `Vadeli` ₺3.610,00.
(6.950 + 12.440 + 3.930 + 3.610 = 26.930)

Seçim yapılınca:

| Seçim | Toplam | Kategoriler |
|---|---:|---|
| `Ziraat Vadesiz` | ₺12.440,00 · 4 kategori | İşyeri kirası ₺6.500,00 (%52) · Ticari mal alımı ₺4.200,00 (%34) · Banka ve POS komisyonu ₺1.140,00 (%9) · Faiz ve finansman gideri ₺600,00 (%5) |
| `Vadeli` | ₺3.610,00 · 2 kategori | Ticari mal alımı ₺2.500,00 (%69) · İşyeri faturaları ₺1.110,00 (%31) |
| `Bonus Kart` | ₺3.930,00 · 2 kategori | Ticari mal alımı ₺3.180,00 (%81) · İşyeri faturaları ₺750,00 (%19) |

### Giderlerin kayıtları (kategori sayfaları)

**Ticari mal alımı · ₺12.480,00** (B1–B4)

| Gün | Satır | Nerede | Tutar |
|---|---|---|---:|
| 5 Ekim | Toptancı ödemesi | Ziraat Vadesiz | −₺4.200,00 |
| 4 Ekim | Kırtasiye toptan | Bonus Kart | −₺3.180,00 |
| 3 Ekim | Vadeli alım · Test Toptancı | Vadeli | −₺2.500,00 |
| 2 Ekim | Hal alışverişi | Dükkan Kasası | −₺2.600,00 |

`Hesap / kart` menüsü: `Hepsi` ₺12.480,00 · `Dükkan Kasası` ₺2.600,00 · `Ziraat Vadesiz` ₺4.200,00 ·
`Bonus Kart` ₺3.180,00 · `Vadeli` ₺2.500,00. `Kişi` menüsü: `Hepsi` ₺12.480,00 · `Test Toptancı` ₺2.500,00 ·
`Kişi yok` ₺9.980,00. `Ziraat Vadesiz` seçiliyken: kart `Ekim gideri · Ziraat Vadesiz` ₺4.200,00,
`Kategorinin tamamı ₺12.480,00`; tek satır; `Kişi` çipi soluk (o kaydın kişisi yok).

**İşyeri faturaları · ₺1.860,00** (B5)

| Gün | Satır | Nerede | Tutar |
|---|---|---|---:|
| 12 Ekim | Doğalgaz faturası | Bonus Kart | −₺750,00 |
| 3 Ekim | Elektrik faturası | Vadeli · `Ödendi · 20 Eki · Ziraat Vadesiz` | −₺1.110,00 |

**Banka ve POS komisyonu · ₺1.140,00** (B6)

| Gün | Satır | Alt yazı | Tutar |
|---|---|---|---:|
| 28 Ekim | Hesap işletim ücreti | Ziraat Vadesiz | −₺200,00 |
| 25 Ekim | Kartlı satış · Ziraat POS | ₺8.400,00 satışın komisyonu | −₺326,00 |
| 21 Ekim | Transfer · Ziraat Vadesiz → Garanti Vadesiz | ₺10.000,00 transferin masrafı | −₺50,00 |
| 15 Ekim | POS yatışı · Ziraat Vadesiz | ₺7.169,00 yatışın kesintisi | −₺40,00 |
| 14 Ekim | Kartlı satış · Ziraat POS | ₺7.500,00 satışın komisyonu | −₺291,00 |
| 6 Ekim | Kartlı satış · Ziraat POS | ₺6.000,00 satışın komisyonu | −₺233,00 |

(200 + 326 + 50 + 40 + 291 + 233 = 1.140. Hepsi `Ziraat Vadesiz`in altındadır.)

**Faiz ve finansman gideri · ₺600,00** (B7)

| Gün | Satır | Alt yazı | Tutar |
|---|---|---|---:|
| 20 Ekim | Borç taksidi ödemesi · Ahmet Usta | ₺1.000,00 taksidin faizi · Ziraat Vadesiz | −₺150,00 |
| 5 Ekim | Borç taksidi ödemesi · Örnek Banka | ₺3.000,00 taksidin faizi · Ziraat Vadesiz | −₺450,00 |

Öbür üç kategori tek kayıtlıdır: `İşyeri kirası` 1 Ekim, Ziraat Vadesiz, −₺6.500,00 · `Personel giderleri`
15 Ekim, Dükkan Kasası, −₺4.200,00 · `Kasa farkı` 18 Ekim, Dükkan Kasası, −₺150,00.

### Gelir · ₺44.550,00 · 3 kategori

| Kategori | Tutar | Pay | Geçen ay |
|---|---:|---:|---:|
| Satış geliri | ₺41.200,00 | %92 | ₺38.650,00 |
| Hizmet geliri | ₺3.000,00 | %7 | ₺2.400,00 |
| Faiz geliri | ₺350,00 | %1 | ₺350,00 |

`Hesap / kart` menüsü: `Hepsi` ₺44.550,00 · `Dükkan Kasası` ₺15.150,00 · `Ziraat Vadesiz` ₺24.900,00 ·
`Vadeli` ₺4.500,00. Gelirde kredi kartı grubu yoktur.

**Satış geliri · ₺41.200,00** (B8)

| Gün | Satır | Nerede | Tutar | Altında |
|---|---|---|---:|---|
| 25 Ekim | Kartlı satış · Ziraat POS | Ziraat Vadesiz · Yolda | +₺8.400,00 | komisyon ₺326,00 |
| 25 Ekim | Nakit satış · Gün sonu | Dükkan Kasası | +₺5.300,00 | |
| 14 Ekim | Kartlı satış · Ziraat POS | Ziraat Vadesiz · Hesaba geçti | +₺7.500,00 | komisyon ₺291,00 |
| 14 Ekim | Nakit satış · Gün sonu | Dükkan Kasası | +₺4.900,00 | |
| 14 Ekim | Veresiye satış · Ayşe Terzi | Vadeli | +₺2.500,00 | |
| 6 Ekim | Kartlı satış · Ziraat POS | Ziraat Vadesiz · Hesaba geçti | +₺6.000,00 | komisyon ₺233,00 |
| 6 Ekim | Nakit satış · Gün sonu | Dükkan Kasası | +₺4.600,00 | |
| 6 Ekim | Veresiye satış · Mehmet Usta | Vadeli | +₺2.000,00 | |

Menü bu sayfada: `Dükkan Kasası` ₺14.800,00 · `Ziraat Vadesiz` ₺21.900,00 · `Vadeli` ₺4.500,00.
`Hizmet geliri`: 10 Ekim, Ziraat Vadesiz, +₺3.000,00. `Faiz geliri`: 12 Ekim, `Alacak taksidi tahsilatı · Kemal Bey`,
`₺2.000,00 taksidin faizi · Dükkan Kasası`, +₺350,00.

### Ayrı küçük örnekler

- **A8 · `Kategorisi yok`:** gelir listesinde `Faiz geliri` satırı yerine `Kategorisi yok` ₺350,00; toplam
  yine ₺44.550,00.
- **A9 · Ay seçici** (`Gider · İşletme · Bonus Kart`, 2026): Oca ₺1.950 · Şub ₺2.700 · Mar ₺1.450 ·
  Nis ₺3.400 · May ₺3.050 · Haz ₺2.350 · Tem yok · Ağu ₺1.700 · Eyl yok · **Eki ₺3.930 (seçili)** ·
  Kas ₺1.100 (ileri ay, kaydı var) · Ara yok (ileri ay, kapalı).
- **A10:** `Eylül 2026`, `Bonus Kart` seçili, ₺0,00.
- **A7 · Uzun ad:** elindeki ikinci `M2` çerçevesindeki adlar ve tutarlar; kredi kartı satırlarından sonra
  bir `Vadeli` satırı ekle ve `Hepsi`yi satırların toplamına eşitle.
- **B9 · `Sigorta`** (yukarıdaki ₺26.930,00'ün dışında, kendi örneği): 8 Ekim `İşyeri sigortası`,
  Ziraat Vadesiz, `İşletme`, −₺1.200,00 · 15 Ekim `Kasko`, Bonus Kart, `Şahsi`, −₺800,00. `İşletme` ile
  açılınca kart `Ekim gideri · İşletme` ₺1.200,00, `Kategorinin tamamı ₺2.000,00`. `Hepsi` ile açılınca kart
  `Ekim gideri` ₺2.000,00, `Geçen ay ₺1.200,00`.
- **B5 · `Açık` hâli:** aynı elektrik faturası, ödenmemiş.

## Teslim

`00-ortak-cerceve.md`deki gibi: çerçeveler ayrı bir sayfada, README'de çerçeve çerçeve ölçü, metinler ve
davranış (hangi dokunuş neyi değiştirir).

**Menü, ay seçici, işlem satırı (parça ve `Ödendi` hâlleriyle) ve boş hâl bu iki sayfaya özel değildir**;
sıradaki hesap sayfasında da aynen kullanılacak. README'de bunları sayfadan bağımsız, tek tek tarif et.
Yeni token ekleme; yeni bir bileşen gerekiyorsa adını ve mevcut hangi bileşenden türediğini yaz.
