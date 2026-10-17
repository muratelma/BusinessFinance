# Brif 7 — Borç ve alacaklar, kişinin sayfası, ödeme ve plan

`00-ortak-cerceve.md` ile birlikte okunur. Brif 5'in ortak parçaları (işlem satırı, boş hâl, çipin altında
açılan menü) ve Brif 6'nın ayrıntı paneli burada aynen kullanılır; bu sayfalara özel kopya çizme.

Konu: borçların ve alacakların **tek yerde** toplanması. Bugün uygulamada aynı şeyin üç kapısı var:
`Yükümlülükler` (ödenmemiş fatura, tek seferlik alacak), `Borç ve alacaklar` (taksitli borç planları) ve
`Cari hesap` (veresiye defteri). Arka planda ilk ikisi tek bir borç kaydında birleşti; şimdi ekranlar da
birleşiyor ve dört eksik kapanıyor:

- **Kısmi ödeme yok.** Fatura yalnız tamamıyla, taksit yalnız tam tutarıyla kapanıyor.
- **Plan değişmiyor.** Tek ödemesi beklenen fatura sonradan taksite bölünemiyor; iptal edip yeniden açmak
  gerekiyor.
- **Ödeme geri alınamıyor.** Yanlış hesaba yazılan ödeme ancak borcun tamamı iptal edilerek düzeliyor.
- **Kredi kartıyla ödeme yok.** Faturayı kartla ödeyen kullanıcı ya kart harcaması yazıp gideri iki kez
  sayıyor ya da hesap seçip kart borcunu eksik bırakıyor.

## Elindeki çizimler

Kuralları ve sayıları gösteren kaba panolar: `tasarim-onizleme/liste-sistemi/cari/tur-4-duzeltme/` (01–29).
**Görsel dilini izleme**; yalnız hangi durumda ne yazdığına ve sayıların nasıl tuttuğuna bak. En çok
yararlanacakların: 02 (ekleme akışı), 03 (kısmi ödeme ve plan), 04 (kişinin sayfası ve ödeme hedefi),
09 (iki liste, tek kişi), 13 (geri alma ve kapananlar), 14 (`İşlem ekle` ve İşlemler), 15 (liste ve başlık
kuralı), 16 (indirim), 17 (fazla ödeme ve kartla ödeme), 18 (uygulamadan önceki borç), 22 (takip biçimi).

Bugünkü kişinin sayfası (cari kartı `Size borcu · Sizin borcunuz · Net - Borcunuz`, `Veresiye satış`,
`Vadeli alım`, `Tahsilat kaydet`, `Ödeme kaydet`, hareketler listesi) Son Tasarım'ın dilinde kodda duruyor;
onu **koru**, yalnız aşağıdaki bloğu ekle.

## Değişmez kurallar

**Model**

1. **Borç tek kayıttır; plan onun ayarıdır.** Plan üç seçenektir: `Tek ödeme` (son gün), `Taksitli`
   (taksitler), `Plan yok` (isteğe bağlı son gün). Plan sonradan değişir; değişince gelir ya da gider
   yazılmaz, yapılmış ödeme silinmez.
2. **Borç kendi ödemeleriyle kapanır.** Her ödeme tek borca bağlıdır ve istenen tutarda olabilir; kalanı
   aşamaz. Ödeme borçlar arasında dağıtılmaz: bir havaleyle iki borç ödendiyse iki ayrı ödeme yazılır.
3. **Taksitli borçta ödeme en eski açık taksitten başlar.** Kullanıcı taksit seçmez. 1.500'lük ödeme
   1.000'lik taksidi kapatır, sonrakinin 500'ünü öder.
4. **Faizli borç ilk sürümde istisnadır:** taksiti kısmen ödenmez, `Plan yok` seçilemez. Ekran bunu seçenek
   kapatarak söyler, hata göstererek değil.
5. **Fazla ödeme reddedilir ve yolu söylenir** (cari hariç; cari fazla ödemeyi bugünkü gibi kabul eder):
   borçta *"Kalan ₺3.000,00. Fazlasını ayrı yazın: geri verecekse Borç para verdim, vermeyecekse Gider."*;
   alacakta *"… geri verecekseniz Borç para aldım."*
6. **Ödemenin yeri:** borçta **hesap ya da kredi kartı**, alacakta **hesap ya da POS**. Kartla ödemede borç
   kapanır, kart borcu artar, gider yazılmaz (gider borcun açıldığı gün yazılmıştı). POS'la tahsilatta para
   `Yolda` görünür ve yatışla hesaba geçer.
7. **Geri alma yalnız kendi kapattığını açar.** Ödemeyi geri almak borcu açar, başka ödemeyi kaydırmaz;
   borç kapanmışsa açık borçlara döner. Gün sonunda sayılmış ya da yatışa bağlanmış ödeme geri alınamaz;
   ekran önce neyin geri alınacağını söyler.
8. **Borcu iptal etmek bir bütündür:** açılış ve bütün ödemeleri birlikte iptal olur. Kişi, tutar ya da gün
   yanlışsa düzeltme = iptal + doğrusunu yeniden yazmak.
9. **İndirim kaynağı düzeltir.** Mal ya da hizmet borcunda indirim bedeli düşürür; gider (alacakta gelir)
   **alımın kendi gününde** azalır, kalan azalır, para hareketi yoktur. Cari hesaptaki bir alımda da aynı.
   Bu dalgada `Borçtan düş` menüsünde yalnız `İndirim` vardır; `İade`, `Vazgeçme` ve `Tahsil edilemeyen`
   sonra eklenecek — menü dört seçeneği taşıyabilmeli.
   İşlemler'deki alım satırı **indirim düşülmüş tutarı** yazar; indirim tutarın altında küçük bir satırdır
   (`₺200,00 indirim`). Cari alım satırı da aynı (kullanıcı kararı, 16 Ekim 2026).
10. **Uygulamadan önceki borç** para hareketi ve gelir/gider yazmaz; yalnız kalanı ve planı girilir.
    Faizliyse kalan anapara da yazılır. Cari kişide bu, cariye `Devreden bakiye` olarak girer.

**Listeler ve kişi**

11. **İki liste, tek kişi sayfası.** `Diğer` menüsünde `Borç ve alacaklar` (herkeste) ve `Cari hesap`
    (işletmede) kalır; `Yükümlülükler` kalkar. İki listeden hangisinden dokunulsa aynı kişinin sayfası açılır.
    Ayrı bir `Kişiler` menüsü yoktur.
12. **`Borç ve alacaklar` iki görünümlüdür: `Borçlar` ve `Kişiler`.** İkisinde de `Borçlarım · Alacaklarım`.
    `Borçlar` görünümünde bölümler `Geciken · Yaklaşan · Vadesiz`, en altta `Kapananlar (n) ›`. Üstte tek
    toplam: `Kalan borçlarınız` / `Kalan alacaklarınız`, altında `Cari hesap dahil`.
13. **Cari kişi listede bir kez durur** (`Ayşe Terzi · cari hesap`): gecikmiş tutarı varsa `Geciken`'de, vadesi
    varsa `Yaklaşan`'da, yoksa `Vadesiz`'de. Tutarı bütün cari bakiyesidir; alt yazı gecikeni söyler.
14. **Satır başlığı açıklamadır; yoksa kişinin adı.** Alt yazı başlıkta olmayanı söyler: kişi, plan durumu
    (`son gün 30 Eki`, `sıradaki 10 Kas · ₺1.000,00`, `plan yok`). Tutar **kalandır**; taksitli ve ödemesi
    olan satırda altında `kalan` yazar. Kişinin sayfasındaki blokta başlık yine açıklamadır, açıklama yoksa
    türü yazar (`Borç para`).
15. **Kişinin sayfasında `Borç ve alacaklar` bloğu** cari kartın altındadır: *"Cari hesaba eklenmez; her biri
    kendi ödemeleriyle kapanır."* Bloğun altında `Bu kişiye toplam borcunuz` yalnız **bilgidir**, tek bir ödeme
    tutarı değildir.
16. **`Ödeme kaydet` / `Tahsilat kaydet` hedefi sorar, hazır seçmez.** Kişinin birden çok açık kaydı varsa
    önce *"Hangi borca ödeme kaydedeceksiniz?"*: cari hesap ve her borç, kalanıyla; hiçbiri seçili gelmez.
    Tek açık kaydı varsa doğrudan forma geçilir.
17. **Borçta kişi zorunlu değildir** (`İşyeri sigortası`); kişisiz borçta açıklama zorunludur. Kişi alanı
    borçta `Kimden`, alacakta `Kime`. Her form aynı kişi seçiciyi kullanır; pasif kişi seçicide gizlidir,
    aramada `Pasif` etiketiyle çıkar; pasif kişiye yeni borç yazılmaz, açık borcu ödenir.

**Ekleme**

18. **`Borç veya alacak ekle` iki sorudur.** Önce yön (`Borç ekle` / `Alacak ekle`) ve *"Ne oldu?"*:
    `Borç para aldım` · `Aldım, sonra ödeyeceğim` · `Uygulamadan önceki borç` (alacakta karşılıkları:
    `Borç para verdim` · `Sattım, sonra tahsil edeceğim` · `Uygulamadan önceki alacak`). Sonra form.
19. **Takip biçimi (yalnız işletmede, kişi seçilmiş mal ya da hizmet borcunda):** plan sorulmadan önce
    *"Nasıl takip edilsin?"* — `Cari hesaba yaz` (vade isteğe bağlı, plan sorulmaz) ya da `Ayrı borç olarak
    takip et` (plan sorulur). Kişinin sayfasındaki `Vadeli alım` / `Veresiye satış` bu soruyu hiç sormaz,
    doğrudan cariye yazar.
20. **Peşin ödenen tutar** (alımda) formda yazılabilir ve borcun ilk ödemesi olur: gider bir kez tam tutar,
    para çıkışı bir kez peşin tutar.
21. **Kişinin sayfasından açılınca** kişi ve yön hazır gelir; yalnız *"Ne oldu?"* ve plan sorulur.

**Girişler ve İşlemler**

22. **`İşlem ekle`:** `Ödenmemiş fatura` kalkar; yerine `Borç veya alacak ekle` ve `Ödeme veya tahsilat kaydet`
    gelir. İkincisi bütün kişilerin açık kayıtlarını (16'daki seçimin aynısı, `Borçlarım · Alacaklarım`,
    arama, geciken üstte) gösterir. Gider formuna borç sorusu **eklenmez**.
23. **Kamera:** fiş okunduktan sonra uygulama önce kayıtlı borcu ve cari alımı arar. Eşleşme varsa
    `Henüz ödemedim` yeni kayıt yazmaz; `Ödedim` ve `Kısmen ödedim` o kayda ödeme yazar. Eşleşme yoksa ekleme
    akışı belgeden dolu açılır.
24. **İşlemler'de tür adları:** `Borç / alacak açılışı`, `Borç ödemesi`, `Alacak tahsilatı`. `Yükümlülük`,
    `Yükümlülük ödemesi` ve `Borç taksidi ödemesi` kalkar. Satırın başlığı kişi, alt yazısı hangi borç
    (`Borç ödemesi · Nakliye faturası · Ziraat`). Ayrıntıda işlem sonrası bakiye ve **borcun kalanı** yazar.
25. Tutarları, kalanları, gecikmiş tutarı ve toplamları **sunucu verir**; formdaki "ödemeden sonra kalan"
    yalnız gösterimdir (`00-ortak-cerceve.md`).

## Kelimeler

- Ekranda: `Borç ve alacaklar`, `Cari hesap`, `Borçlar`, `Kişiler`, `Borçlarım`, `Alacaklarım`, `Geciken`,
  `Yaklaşan`, `Vadesiz`, `Kapananlar`, `Kalan`, `Ödenen`, `Tutar`, `Ödeme planı`, `Tek ödeme`, `Taksitli`,
  `Plan yok`, `Son gün`, `Sıradaki`, `Ödeme kaydet`, `Tahsilat kaydet`, `Planı değiştir`, `Borçtan düş`,
  `İndirim`, `Ödemeyi geri al`, `Borcu iptal et`, `Nereden ödendi?`, `Hesap`, `Kredi kartı`, `POS`,
  `Peşin ödenen`, `Kimden`, `Kime`, `Nasıl takip edilsin?`, `Cari hesaba yaz`, `Ayrı borç olarak takip et`,
  `Uygulamadan önceki borç`, `Devreden bakiye`, `Kalan anapara`.
- Düğme adları **kaydetmeyi** söyler (`Ödeme kaydet`): uygulama para göndermez, yapılmış ödemeyi kaydeder.
- Ekranda **geçmez:** `yükümlülük`, `tahsis`, `dağıtım`, `mahsup`, `kâr`, `KDV`, "taşır", "tanır".
- Cümleler kısa ve doğal Türkçe. Uzun açıklama, büyük buton, büyük yazı yok.

## Çizilecek çerçeveler

**Düzenle** = kodda var, yeni kurala göre. **Yeni** = daha önce çizilmedi.

### L · Borç ve alacaklar

| # | Çerçeve | Durum |
|---|---|---|
| L1 | `Borçlar` · `Borçlarım`: toplam, `Geciken · Yaklaşan · Vadesiz`, cari satırı, kişisiz borç, `Kapananlar (2) ›` | Yeni |
| L2 | `Borçlar` · `Alacaklarım`: gecikmiş tutarı olan cari satırı `Geciken`'de | Yeni |
| L3 | `Kişiler` görünümü: kişi başına bir satır, iki yönün toplamı | Yeni |
| L4 | `Kapananlar` | Yeni |
| L5 | Hiç borç yok (ilk kullanım) | Yeni |
| L6 | `Diğer` menüsü: `Yükümlülükler` kalkmış, `Borç ve alacaklar` ve `Cari hesap` | Düzenle |

### K · Kişinin sayfası

| # | Çerçeve | Durum |
|---|---|---|
| K1 | `Test Toptancı`: cari kartı (bugünkü), `Borç ve alacaklar` bloğu, toplam bilgisi, hareketler | Düzenle |
| K2 | `Ödeme kaydet`e dokununca: *"Hangi borca ödeme kaydedeceksiniz?"*, hiçbiri seçili değil | Yeni |
| K3 | Cari kaydı olmayan kişi (`Kuzen Ali`): cari kartı yok, yalnız blok | Yeni |

### B · Borcun sayfası

| # | Çerçeve | Durum |
|---|---|---|
| B1 | Tek ödeme, kısmen ödenmiş (`Raf faturası`): tutar / ödenen / kalan, plan satırı, `Ödeme kaydet`, ödemeler listesi | Yeni |
| B2 | Taksitli (`Soğutucu dolap`): kalan ve sıradaki, taksit listesi (ödendi · kısmen · açık), ödemeler | Yeni |
| B3 | Taksitli, faizli, uygulamadan önce (`İhtiyaç kredisi`): `Kalan ödeme` ve `Kalan anapara` iki satır | Yeni |
| B4 | Plan yok (`Kuzen Ali · Borç para`) | Yeni |
| B5 | Ayar menüsü: `Planı değiştir`, `Borçtan düş`, `Borcu iptal et` (sonucunu söyleyen satırlar) | Yeni |
| B6 | Ödeme satırına dokununca: ödeme ayrıntısı ve `Ödemeyi geri al`; geri alınamayan ödeme hâli | Yeni |
| B7 | Kapanmış borç | Yeni |

### F · Formlar

| # | Çerçeve | Durum |
|---|---|---|
| F1 | `Ödeme kaydet`: tutar kalanla dolu gelir, gün, `Nereden ödendi?` `Hesap`, "ödemeden sonra kalan" | Yeni |
| F2 | `Ödeme kaydet`, `Kredi kartı` seçili: kart ve sonucu söyleyen satır | Yeni |
| F3 | `Tahsilat kaydet`, `POS` seçili: POS, komisyon ve "yolda" satırı | Yeni |
| F4 | Fazla tutar: alan hatası ve yolu söyleyen satır, `Kaydet` kapalı | Yeni |
| F5 | Taksitli borçta kısmi ödeme: hangi taksitlerin kapanacağını gösteren satır | Yeni |
| F6 | `Planı değiştir`: `Tek ödeme · Taksitli · Plan yok`, yeni takvimin önizlemesi; faizli borçta kapalı seçenek | Yeni |
| F7 | `Borçtan düş` · `İndirim`: önce → sonra satırları (tutar, kalan, alım gününün gideri) | Yeni |
| F8 | `Ödemeyi geri al` ve `Borcu iptal et` onayları | Yeni |

### E · Ekleme ve girişler

| # | Çerçeve | Durum |
|---|---|---|
| E1 | `Borç veya alacak ekle` · adım 1: yön ve *"Ne oldu?"* (üç seçenek) | Yeni |
| E2 | `Aldım, sonra ödeyeceğim`, işletme, kişi seçili: *"Nasıl takip edilsin?"* | Yeni |
| E3 | Ayrı borç formu: kişi, tutar, kategori, gün, `Peşin ödenen`, ödeme planı, özet satırı | Yeni |
| E4 | `Borç para aldım` formu: hesap, plan, faiz (toplam geri ödeme) | Yeni |
| E5 | `Uygulamadan önceki borç`: kalan, kalan taksit ve sıradaki, kalan anapara; cari kişide `Devreden bakiye` | Yeni |
| E6 | Kişinin sayfasından açılınca: kişi ve yön hazır | Yeni |
| E7 | `İşlem ekle`: `Ödenmemiş fatura` yerine iki giriş | Düzenle |
| E8 | `Ödeme veya tahsilat kaydet`: bütün kişiler, arama, geciken üstte | Yeni |
| E9 | Kamera: eşleşme bulundu; `Henüz ödemedim · Ödedim · Kısmen ödedim` | Yeni |
| E10 | İşlemler'de üç satır ve `Borç ödemesi` ayrıntısı (borcun kalanı) | Düzenle |
| E11 | 2.0× yazı ölçeği: en uzun satır (`Örnek Elektrik Dağıtım A.Ş.`, `−₺1.284.650,40`, `sıradaki 10 Kas · ₺1.000,00`) | Yeni |

## Senden karar önerisi beklediğim yerler

Emin olmadığın yerde iki seçeneği yan yana çiz, hangisini önerdiğini yaz.

1. **Borcun sayfasında eylemler.** Dış göz tek ana eylem öneriyor (`Ödeme kaydet`), gerisi tek bir ayar
   menüsünde (`Planı değiştir`, `Borçtan düş`, `Borcu iptal et`). Kaba panolarda `Planı değiştir` ikinci
   düğmeydi. Hangisi?
2. **Taksit listesi ile ödemeler listesi.** Taksitli borçta ikisi de var; ikisi aynı sayfada mı, yoksa
   biri sekme ya da açılır bölüm mü?
3. **Kısmen ödenmiş taksit** satırda nasıl yazar (`₺500,00 kaldı · Gecikmiş`)?
4. **`Borçlar` / `Kişiler` görünüm seçimi** ile `Borçlarım / Alacaklarım` iki ray üst üste mi, yoksa biri
   başlıkta mı?
5. **Ekleme akışı adımları.** Panolarda adım çizgisi vardı; iki ya da üç adımda (yön → ne oldu → [takip
   biçimi] → form) ilerlemenin en sade hâli.
6. **Kredi kartı ve POS seçimi** formda ray mı, yoksa hesap seçicinin içinde ayrı grup mu?
7. **`Kalan borçlarınız` toplamı** cari dahil tek sayı mı, yoksa cari ayrı satır mı (panoda tek sayı, altında
   `Cari hesap dahil`)?

## Örnek veri (sentetik; bugün 28 Ekim 2026 Çarşamba)

### Borçlarım (L1) — toplam ₺37.650,00, cari hesap dahil

| Bölüm | Başlık | Alt yazı | Kalan |
|---|---|---|---:|
| Geciken | Nakliye faturası | Test Toptancı · son gün 25 Eki | ₺950,00 |
| Yaklaşan | Raf faturası | Demir Mobilya · son gün 30 Eki | ₺7.000,00 kalan |
| Yaklaşan | Test Toptancı · cari hesap | Açık hesap · vade 2 Kas | ₺1.500,00 |
| Yaklaşan | İhtiyaç kredisi | Ziraat Bankası · sıradaki 5 Kas · ₺1.500,00 | ₺18.000,00 kalan |
| Yaklaşan | Soğutucu dolap | Test Toptancı · sıradaki 10 Kas · ₺1.000,00 | ₺6.000,00 kalan |
| Yaklaşan | İşyeri sigortası | son gün 15 Kas | ₺1.200,00 |
| Vadesiz | Kuzen Ali | Borç para · plan yok | ₺3.000,00 |

1.500 + 950 + 7.000 + 6.000 + 3.000 + 18.000 + 1.200 = 37.650.

### Alacaklarım (L2) — toplam ₺16.000,00, cari hesap dahil

| Bölüm | Başlık | Alt yazı | Kalan |
|---|---|---|---:|
| Geciken | Ayşe Terzi · cari hesap | Açık hesap · ₺2.000,00 gecikmiş | ₺3.500,00 |
| Yaklaşan | Montaj işi | Kaan Yapı · son gün 15 Kas | ₺4.000,00 |
| Yaklaşan | Overlok makinesi | Ayşe Terzi · sıradaki 15 Kas · ₺1.500,00 | ₺4.500,00 kalan |
| Vadesiz | Telefon | Elif · plan yok | ₺4.000,00 kalan |

### Kişiler görünümü (L3)

Test Toptancı borcunuz ₺8.450,00 (cari 1.500 + fatura 950 + dolap 6.000) · Ayşe Terzi size borcu ₺8.000,00
(cari 3.500 + overlok 4.500) · Ziraat Bankası ₺18.000,00 · Demir Mobilya ₺7.000,00 · Kaan Yapı ₺4.000,00 ·
Elif ₺4.000,00 · Kuzen Ali ₺3.000,00. Kişisiz `İşyeri sigortası` bu görünümde yoktur.

### Kapananlar (L4)

`Borç para` · Ece · 5 Eki'de kapandı · ₺1.000,00 · `Boya malzemesi` · Test Toptancı · 12 Eyl'de kapandı ·
₺2.000,00.

### Borçların ayrıntısı

- **B1 · Raf faturası:** Demir Mobilya · Demirbaş · 1 Ekim 2026 · tutar ₺10.000,00 · ödenen ₺3.000,00 ·
  kalan ₺7.000,00 · `Tek ödeme · son gün 30 Eki`. Ödemeler: `Peşin ödeme` 1 Ekim · Ziraat Vadesiz ·
  ₺3.000,00.
- **B2 · Soğutucu dolap:** Test Toptancı · Ticari mal · 1 Ağustos 2026 · 8 × ₺1.000,00. 10 Eyl ve 10 Eki
  ödendi (Ziraat Vadesiz); kalan ₺6.000,00, sıradaki 10 Kas.
- **B3 · İhtiyaç kredisi:** Ziraat Bankası · uygulamadan önce · 12 taksit × ₺1.500,00 · kalan ödeme
  ₺18.000,00 · kalan anapara ₺15.800,00 · sıradaki 5 Kas. Faiz (₺2.200,00) taksit ödendikçe gider olur.
- **B4 · Kuzen Ali · Borç para:** 1 Eylül 2026 · ₺3.000,00 · plan yok · Kasa'ya girdi · şahsi.
- **B7 · kapanmış:** `Boya malzemesi` · ₺2.000,00 · 12 Eyl'de son ödemeyle kapandı.

### Formlar

- **F1:** Nakliye faturasına ₺500,00 · 28 Ekim · Ziraat Vadesiz → *"Ödemeden sonra kalan: ₺450,00. Fatura
  açık kalır."*
- **F2:** Nakliye faturası ₺950,00 · İş kartı → *"Fatura kapanır. İş kartının borcu +₺950,00; gider
  yazılmaz, fatura gününde yazılmıştı."*
- **F3:** Overlok makinesi ₺1.500,00 · Ana POS · komisyon ₺45,00 → *"Hesaba ₺1.455,00 geçecek · 29 Eki
  bekleniyor."*
- **F4:** Kuzen Ali'ye ₺3.200,00 (kalan ₺3.000,00) → kural 5'teki cümle.
- **F5:** Soğutucu dolaba ₺1.500,00 → *"10 Kas taksiti kapanır; 10 Ara taksitinden ₺500,00 kalır."*
- **F6:** Raf faturası kalan ₺7.000,00, `Tek ödeme` → `Taksitli` 2 taksit, ilk 30 Kasım: 30 Kas ₺3.500,00 ·
  30 Ara ₺3.500,00. Faizli `İhtiyaç kredisi`nde `Plan yok` kapalı.
- **F7:** Raf faturasına ₺1.000,00 indirim · 28 Ekim: tutar ₺10.000,00 → ₺9.000,00 · ödenen ₺3.000,00 ·
  kalan ₺7.000,00 → ₺6.000,00 · `Gider · 1 Ekim` ₺10.000,00 → ₺9.000,00. Cari örneği: Test Toptancı'nın
  3 Ekim `Vadeli alım`ı ₺2.500,00 → ₺2.000,00, `Sizin borcunuz` ₺1.500,00 → ₺1.000,00.
- **F8 · Ödemeyi geri al (peşin ödeme):** *"Geri alınınca borcun kalanı ₺10.000,00 olur; Ziraat Vadesiz'e
  ₺3.000,00 döner; gider değişmez."* **Borcu iptal et:** *"Gider ve ₺3.000,00 peşin ödeme de iptal olur.
  Kayıt silinmez."* Geri alınamayan: *"Bu tahsilat gün sonunda sayıldı. Önce 26 Ekim gün sonunu geri
  alın."*

### Ekleme

- **E2/E3:** Test Toptancı'dan 28 Ekim'de ₺2.500,00'lük ticari mal (cari) · ayrı borç örneği Demir
  Mobilya'dan 1 Ekim'de ₺10.000,00'lük raf, ₺3.000,00 peşin (Ziraat Vadesiz), tek ödeme son gün 30 Ekim;
  özet *"Gider ₺10.000,00 · bugün çıkan ₺3.000,00 · kalan borç ₺7.000,00"*.
- **E4:** Kuzen Ali'den ₺3.000,00 borç para, Kasa'ya, plan yok.
- **E5:** Ziraat Bankası ihtiyaç kredisi (B3'teki); cari örneği Deniz Market'in ₺1.200,00 devreden bakiyesi
  (*"Uygulamadan önce · gelir yazılmaz"*).
- **E9:** Fiş: `Test Toptancı · Nakliye · ₺950,00 · 20 Ekim` → *"Bu fatura kayıtlı: Nakliye faturası · kalan
  ₺950,00."*
- **E10:** 28 Ekim: `Test Toptancı` · `Borç ödemesi · Nakliye faturası · Ziraat` · −₺500,00 (mavi, para
  taşır) · `Ayşe Terzi` · `Alacak tahsilatı · Overlok makinesi` · +₺1.500,00 `yolda · Ana POS` · `Market` ·
  `Gıda · Kasa` · −₺420,00 (gider).

## Teslim

`00-ortak-cerceve.md`deki gibi: çerçeveler ayrı bir sayfada, README'de çerçeve çerçeve ölçü, metinler ve
davranış. Ortak parçalarda (işlem satırı, ayrıntı paneli, kişi seçici) değişen bir şey varsa README'de
**ortak parçanın yeni hâli** olarak yaz. Yeni token ekleme.
