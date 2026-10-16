# ADR 0021 — Borç kendi ödemeleriyle kapanır; yükümlülük ve borç planı tek borçtur

- Durum: **Kabul edildi** (16 Ekim 2026). Kararlar kullanıcıyla 14–16 Ekim
  2026'da panolar üzerinden verildi; kullanıcı metnin onayını "problem yoksa
  onaylayabilirsin" diyerek bıraktı. Aşama 06.3 Grup 8 "Cari, borç ve
  alacak"ın karar kapısıdır. Dalga 0 bu ADR'ye bağlı değildir ve uygulandı.
- **Nasıl okunur:** iki katmanlıdır. **§İlkeler bağlayıcıdır**; değişmesi kullanıcı
  kararı ve bu ADR'nin güncellenmesini ister. **§Başlangıç tasarımı** uygulamanın
  ilk hâlidir ve ilkeleri bozmadan değişebilir (`AGENTS.md` "Kararlardan sapma").
- **Genişlettiği:**
  - ADR 0020 İ10: "tek yoldan kapanır" cümlesi "tek ödemeyle kapanır" demek
    değildir. Borç kendi kaydına bağlı birden çok ödeme alabilir.
  - ADR 0020 T6: `Yükümlülükler` bölümü kalkar; tek seferlik borç ve alacak
    `Borç ve alacaklar`a girer.
  - ADR 0009 §6: bilinmeyen açılışın yanına bilerek girilen devir eklenir.
    İkisi ayrı durumlardır.
  - ADR 0014: bedel düzeltmesi, alınmayacak para ve para hareketi ayrı olaylardır.
- İlgili: ADR 0010 (net varlık borcu anaparayla ölçer), ADR 0015 (kart borcu ve
  kartla tahsil), ADR 0018 (vergi nakit planıdır), ADR 0019 (gün sonu, POS, yatış)
- Gerekçe belgeleri:
  - `research/cari-borc-alacak/YAPI-ONERISI-2026-10-14.md` (tur-3)
  - `research/cari-borc-alacak/YAPI-ONERISI-2-2026-10-14.md` (§0–§8; K1–K23, S1–S6)
  - Dış göz raporları: `research/DIS-GOZ-2026-10-14-cari-borc-alacak-yapisi.md`,
    `research/DIS-GOZ-2026-10-16-cari-borc-alacak-tur-4.md`,
    `research/DIS-GOZ-2026-10-16-cari-borc-alacak-butun.md`
  - Panolar: `tasarim-onizleme/liste-sistemi/cari/tur-4-duzeltme/` 01–29 (Git dışı)

## Bağlam

Kişiyle ilgili borç bugün üç yerde duruyor: `Cari hesap`, `Yükümlülükler` ve
`Borç ve alacaklar`. Sunucuda bunlar üç model: `CounterpartyCharge`/`Payment`,
`Obligation`/`ObligationSettlement`, `DebtAgreement`/`DebtInstallment`.
Kullanıcının sözü: "üçü de aynı şeyi anlatıyor; mantığı iyi değil."

Kodda doğrulanan eksikler:

- `Obligation.Settle` tutarın tamamını tek kapanışla yazıyor.
  10.000'lik faturanın 3.000'i ödenince yazılacak yer yok.
- `DebtInstallment.MarkPaid` taksidi tam tutarla ödüyor. 1.000'lik takside 500
  yazılamıyor.
- Ödeme planı borcun kimliği: tek seferlik bir borç taksitliye çevrilemiyor.
  Çevirmek için iptal edip yeniden açmak gerekiyor; bu da ödemeleri siliyor.
- Borç formu kişiyi adla açıyor, var olan kişiyi seçtirmiyor. Şahsi borçla açılan
  kişi `Cari hesap` listesine düşüyor.
- "Arkadaştan 5.000 aldım, ne zaman olursa öderim" girilemiyor: borç planı vade
  ve taksit istiyor.
- Kendi kredi kartıyla fatura kapatmanın yolu yok. Kullanıcı ya hesap seçiyor
  (kart borcu eksik kalıyor) ya da kart harcaması yazıyor (gider iki kez).
- Cari ödeme, yükümlülük kapanışı ve taksit ödemesi istek kimliği taşımıyor.
  Cari ödeme tekrar gönderilirse iki kez yazılıyor.

Kısmi ödeme ve plan değişikliği iki ayrı modelde iki kez yazılırdı. Gerçek
kullanıcı verisi henüz yok; birleşmenin en ucuz anı bugün.

## İlkeler (bağlayıcı)

**İ1 — Yükümlülük ve borç planı tek bir borçtur; plan borcun ayarıdır.**
Borcun kimliği yön (borç / alacak), kaynak (borç para / mal ya da hizmet), kişi
(isteğe bağlı), tutar ve gündür. Plan ayrıdır ve değişebilir: `Plan yok`,
`Tek ödeme`, `Taksitli`. Plan değişikliği gelir ya da gider yazmaz, yapılmış
ödemeyi silmez ve borcu başka bir kayda taşımaz.

**İ2 — Borç kendi ödemeleriyle kapanır.** Her ödeme ya da tahsilat tek bir
borca bağlıdır ve istenen tutarda olabilir. Bir ödeme borçlar arasında
dağıtılmaz. Birden çok borç ödendiyse her biri için ayrı bağlı ödeme yazılır.
Bir borcun kapanışları toplamı (ödeme, düzeltme, mahsup) kalanını aşamaz.

**İ3 — Cari ayrı kalır ve bir takip biçimidir, plan değildir** (ADR 0020 İ9
ve İ10 sürer). Mal ya da hizmet borcunun cariye mi, ayrı borca mı yazılacağı
girişte sorulur. Cari ile ayrı borç arasında sessiz taşıma yoktur. Cari fazla
ödemeyi bugünkü gibi kabul eder ve kırpmaz.

**İ4 — Üç olay birbirinin yerine geçmez** (ADR 0014'ü genişletir).
- **Bedel değiştiyse** (indirim, iade) kaynak kaydın kendisi düzelir; gelir ya
  da gider **kendi gününde** azalır ve düzeltmenin günü iz olarak kalır.
- **Para alınmayacak ya da verilmeyecekse** (vazgeçme, tahsil edilemeyen
  alacak) **o gün** bir gelir ya da gider yazılır. Yapılmış satışın geliri
  düşmez.
- **Para gerçekten hareket ediyorsa** gerçek gününe ve gerçek hesabına ya da
  kartına bir ödeme veya tahsilat yazılır.

Bir düzeltme para hareketi yazmaz; bir para hareketi gelir ya da gider yazmaz.

**İ5 — Uygulamadan önceki borç açılış yazmaz.** Bilerek devredilen borçta para
hareketi ve gelir/gider yazılmaz. Kaynağı (borç para / mal ya da hizmet) ve
"şu gün itibarıyla" kalanı saklanır. Bu durum açılışı bilinmeyen eski borçtan
(ADR 0009 §6) **ayrıdır**: migration ve geri yükleme bilinmeyeni devir saymaz.
Bilinmeyen açılış sonradan tamamlanabilir kalır.

**İ6 — Bir istek, bir etki.** Ödeme, tahsilat, düzeltme ve mahsup istekleri
istemcinin ürettiği bir istek kimliği taşır. Aynı kimlik aynı sonucu döner.
Kalan denetimi ve yazma aynı güvenli yazma sınırındadır: eşzamanlı iki istek
kalanı aşamaz.

**İ7 — Bağlı etkiler birlikte yazılır ve birlikte geri alınır.**
- Bir ödemenin geri alınması yalnız kendi kapattığını yeniden açar.
- Mahsup iki tarafıyla birlikte yazılır ve birlikte geri alınır; yarım kalamaz.
- Bağlı bir etkisi olan kayıt (mahsup, gün sonu sayımı, POS yatışı) tek başına
  iptal edilemez. Önce bağ geri alınır.

**İ8 — Kişi tek kayıttır.** `Borç ve alacaklar` ve `Cari hesap` iki listedir;
ikisi de aynı kişinin sayfasını açar. Ayrı bir kişiler bölümü yoktur. Her form
aynı kişi seçiciyi kullanır. Kişi borçta zorunlu değildir.

**İ9 — Vergi bu modele girmez** (ADR 0018). Vergi `Vergi takibi`nde kalır;
borç olarak açılmaz ve borç listesinde gösterilmez.

Reddedilenler:

| Reddedilen | Neden |
|---|---|
| İki modelin (yükümlülük + borç planı) kalması | Kısmi ödeme, geri alma ve plan değişikliği iki yerde yazılırdı. `Tek ödeme → Taksitli` dönüşümü yapılamazdı (K11) |
| Ödemenin kişinin borçlarına kendiliğinden dağıtılması (Paraşüt modeli) | Dağıtım kaydı ve dağıtımın geri alınması gerekir; ön muhasebe yükü. F29 parkta kalır |
| `Cari hesaba`nın bir plan seçeneği olması | Plan değişince kayıt sessizce cariden çıkardı; cari bakiye ödeme olmadan düşerdi (K22, pano 29) |
| İadenin eski gideri iptal edip kalanı eski günle yeniden yazması (bugünkü iade fişi yolu) | Paranın döndüğü gün ve hesap kaybolur; yarıda kalırsa gider de kaybolur (backlog 16) |
| Tahsil edilemeyen alacakta satış gelirinin düşmesi | Satış gerçekten yapıldı; alacağın kaybı ayrı ve sonraki bir olaydır |
| Devrin bilinmeyen açılışla aynı işareti taşıması | Ya bilinçli devre para eklenirdi ya da eksik eski kaydı tamamlama imkânı kaybolurdu |
| Ayrı bir `Kişiler` menüsü | Üçüncü bir kişi listesi; "üç yer" sorunu geri gelirdi (K12) |

## Başlangıç tasarımı (uygulamada değişebilir)

### T1. Model

| Kavram | İşi |
|---|---|
| Borç | Yön, kaynak, kişi, kategori (mal/hizmette), taraf, tutar, gün, açıklama, açılış biçimi (`kayıtlı` / `uygulamadan önce` / `bilinmiyor`), "itibarıyla" günü (devirde) |
| Plan | `Plan yok` (isteğe bağlı son gün), `Tek ödeme` (son gün), `Taksitli` (taksitler; faizliyse anapara/faiz payı, ADR 0009) |
| Ödeme | Tek borca bağlı; tutar, gün, hesap ya da kart (ödemede), hesap ya da POS (tahsilatta); kapattığı taksitler ve tutarları; istek kimliği |
| Düzeltme | `İndirim`, `İade`, `Vazgeçme`, `Tahsil edilemeyen`; tutar, gün, gerekirse kategori |
| Mahsup | Tek kimlik; iki borç, tutar, gün |

Adlar ve tablo biçimi uygulamada belirlenir. Cari modeli (`CounterpartyCharge`,
`CounterpartyPayment`) değişmez; yalnız kart ile ödeme (T4), indirim/iade (T3)
ve istek kimliği (İ6) eklenir.

### T2. Ödeme, plan ve sıra

- Ödeme bir borcun içinde **en eski açık taksitten** başlar. Kullanıcı taksit
  seçmez. Geçmiş tarihli yeni ödeme de bu sırayla yerleşir.
- Ödeme geri alınınca yalnız kendi kapattığı taksitler açılır; sonraki ödemeler
  yer değiştirmez.
- Plan değişince yeni takvim o anki kalanı böler. Plandan önceki bir ödeme
  sonradan geri alınırsa tutarı yeni planın **ilk açık taksidine** eklenir.
- Faizli borçta ilk sürümde: `Plan yok` seçilemez, taksit kısmi ödenmez, mahsuba
  girmez. "Her borç kısmi ödeme alır" cümlesi bu istisnayla yazılır.
- Fazla tutar (cari hariç) reddedilir; ekran nedenini ve yolunu yönüne göre
  söyler: borçta "geri verecekse `Borç para verdim`, değilse gider", alacakta
  "geri verecekseniz `Borç para aldım`".

### T3. Düzeltmeler

| Düzeltme | Kaynak | Ne olur |
|---|---|---|
| İndirim | Mal/hizmet borcu, cari borçlandırma | Bedel düşer; gelir ya da gider kendi gününde azalır; kalan azalır; para yok |
| İade | Aynı; ayrıca peşin ödenmiş gider | Bedel düşer. Ödenen yeni bedeli aşıyorsa fark için **iade alacağı** doğar (gelir değildir). Para geldiği gün gerçek hesabına tahsil edilir |
| Vazgeçme | Borç para | Bağışlanan borç o gün gelir, bağışlanan alacak o gün gider; kategoriyi kullanıcı seçer |
| Tahsil edilemeyen | Mal/hizmet alacağı | Kalan o gün gider; satış geliri değişmez |
| Erken kapatma, faizli | Borç para, taksitli | Bankanın bildirdiği tutar; kalan anaparadan az olamaz; fark `Faiz ve kapama masrafı` gideridir; yazılmamış plan faizi gider olmaz |
| Erken kapatma, faizsiz | Her kaynak | Kalandan az ödenirse fark kaynağa göre indirim ya da vazgeçmedir |

- Devredilmiş mal/hizmet borcunda düzeltilecek gider yoktur; indirim o gün gelir
  ya da gider olarak yazılır.
- Gün sonunda sayılmış satışta indirim ve iade reddedilir (`*.day_close_counted`).
- Bugünkü iade fişi yolu bu tabloya geçer (backlog 16b).

### T4. Kartla ödeme ve kartla tahsil

- Ödemede yer **hesap ya da kredi kartı**dır. Kartla ödemede borç kapanır, kart
  borcu artar, gider yazılmaz. Faizli taksitte faiz payı yine gider olur. Kart
  borcu (`CardDebt`) ve kalan limit bu ödemeyi okur. İlk sürümde tek çekim.
- Tahsilatta yer **hesap ya da POS**tur, her alacakta. POS'la tahsil yolda
  görünür ve yatışla hesaba geçer (ADR 0019). Satış geliri yeniden yazılmaz;
  komisyon POS'un kuralıyla yazılır.

### T5. Uygulamadan önceki borç

- Ekleme akışında `Uygulamadan önceki borç` / `… alacak`. Kaynak yine sorulur.
- Formda "şu gün itibarıyla" kalan yazılır.
- Faizliyse kalan anapara ve kalan taksitler girilir. Dağılım ADR 0009'un
  anüite yoluyla kurulur. Net varlık kalan anaparayı sayar (ADR 0010).
- Cari kişide `Cari hesaba` seçilirse cariye **devreden bakiye** girer ve gelir
  yazmaz.

### T6. Mahsup

Kişinin sayfasında iki yönde açık borç varsa `Karşılıklı düş` görünür.
- İki hedef ve tutar seçilir; ikisi de o kadar azalır.
- Para hareketi, gelir ve gider yazılmaz. Kendiliğinden yapılmaz.
- Mahsupla azalmış borcun iptali `*.offset_locked` ile reddedilir; önce mahsup
  geri alınır.

### T7. Giriş akışları

- **Genel giriş:** `İşlem ekle` › `Borç veya alacak ekle`. Sorular sırayla:
  yön ve "Ne oldu?" (borç para / sonra ödenecek alım ya da satış / uygulamadan
  önceki), sonra kişi.
- **Takip biçimi (K22):** işletmede, kişi seçilmiş mal/hizmet borcunda önce
  "Nasıl takip edilsin?" sorulur: `Cari hesaba yaz` (vade isteğe bağlı, plan
  sorulmaz) ya da `Ayrı borç olarak takip et` (plan sorulur). Kişinin
  sayfasındaki `Veresiye satış` / `Vadeli alım` doğrudan cariye yazar.
- **Kişinin sayfasından açılınca** kişi ve yön hazır gelir. Alımda peşin ödenen
  tutar formda yazılabilir ve borcun ilk ödemesi olur.
- **Kamera:** önce kayıtlı borcu ve cari alımı arar.
  - `Henüz ödemedim` + eşleşme → yeni kayıt yazılmaz.
  - `Ödedim` / `Kısmen ödedim` + eşleşme → o kayda ödeme yazılır.
  - Eşleşme yoksa ekleme akışı belgeden dolu açılır.
- **`İşlem ekle`:** `Ödenmemiş fatura` kalkar; yerine `Borç veya alacak ekle` ve
  `Ödeme veya tahsilat kaydet` gelir. Gider formuna borç sorusu eklenmez.
- **Pasif kişi:** yeni borç yazılmaz; açık borcu ödenir. Seçicide gizlidir,
  aramada `Pasif` etiketiyle çıkar.

### T8. Okumalar

- **`Borç ve alacaklar`:** `Borçlar` ve `Kişiler` görünümleri; `Borçlarım` /
  `Alacaklarım`. Bölümler `Geciken · Yaklaşan · Vadesiz`.
- **Cari satırı:** listede bir kez durur.
  - Gecikmiş tutar varsa `Geciken`; yoksa en yakın açık vadeyle `Yaklaşan`;
    hiç vade yoksa `Vadesiz`.
  - Gecikmiş tutar ve en yakın vade, ödemelerin **önce en eski alımı**
    düşürdüğü sırayla hesaplanır. Bugünkü hesap vadesiz eski alımlara yapılan
    ödemeleri de vadeli alımdan düşüyor (YAPI-ONERISI-2 §8).
  - Cari fazla ödeme kırpılmaz.
- **Kalan:** listede `Kalan ödeme`; faizli ayrıntıda ikinci satır
  `Kalan anapara`. Net varlık anaparayı sayar (ADR 0010).
- **Başlık:** açıklama; yoksa kişinin adı. Kişinin sayfasındaki blokta tür yazar.
  Aynı kişiden açıklamasız iki borç alt satırdaki gün ve tutarla ayrılır.
  Migration eski kayda açıklama uydurmaz.
- **Kapananlar:** listenin altında. Geri alınan ödemeyle açılan borç listeye
  döner.
- **Tür adları:** İşlemler'de `Borç / alacak açılışı`, `Borç ödemesi`,
  `Alacak tahsilatı`. `Yükümlülük` ve `Yükümlülük ödemesi` kalkar.

### T9. Migration ve yedek

| Bugün | Tek borç modelinde |
|---|---|
| `Obligation` | Borç; kaynak mal/hizmet; plan `Tek ödeme` (son gün = vade) |
| `ObligationSettlement` (POS bağı dahil) | Borcun ödemesi; POS bağı ve gün sonu bağı birebir taşınır |
| `DebtAgreement` (`Cash` / `Expense` / `Income`) | Borç; kaynak buna göre; plan `Taksitli` |
| `DebtAgreement` (`Unrecorded`) | Borç; açılış biçimi `bilinmiyor` (devir **değil**) |
| `DebtInstallment` ödemesi | Taksite bağlı ödeme; anapara/faiz payı korunur |
| İptal edilmiş kayıtlar, giriş anları, eski kimlikler | Birebir taşınır |

- Birleşme **ekran değişmeden** ayrı bir adımda yapılır. Kanıtı sentetik veri
  üzerinde önce/sonra karşılaştırmasıdır: rapor toplamları, kapsam kırılımı, net
  varlık, planlananlar, kişi bakiyeleri, hesap hareketleri, kart borcu,
  hatırlatma listesi, gün sonu ekranı birebir aynı çıkar.
- Yedek şeması ilerler. Eski sürüm okunur ve dönüştürülür. Sürümün tek kaynağı
  `documentation/restore-runbook.md`.
- Migration kuralları `AGENTS.md`'dekilerdir (backfill CHECK'ten önce; geçmişi
  bilinmeyen bilgi nullable).

### T10. Uygulama sırası

| Dalga | İçerik |
|---|---|
| 0 · hemen (bu ADR'ye bağlı değil) | Cari ödeme/tahsilata istek kimliği; iade fişinde yarıda kalma (16a) |
| 1 · birleşme | T1 ve T9; ekran değişmez |
| 2 · çekirdek | Menü ve iki liste, ortak liste, kişi sayfası ve seçici, K22, kısmi ödeme ve geri alma, plan değişikliği, kartla ödeme ve tahsil, uygulamadan önceki borç, indirim, kamera, `İşlem ekle` girişleri |
| 3 · tamamlama | İade ve iade alacağı (iade fişi yolu dahil), vazgeçme, tahsil edilemeyen, faizli erken kapatma, mahsup, fazla ödeme yönlendirmesi |

Ölçüt: yokluğu kullanıcıyı **yanlış** kayda zorlayan özellik ikinci dalgadadır;
yalnız uzun yola zorlayan üçüncüde. Her dalga kendi testleri ve kendi commit'iyle
kapanır.

## Kabul senaryoları

| Senaryo | Beklenen |
|---|---|
| 10.000 alım, 3.000 ödeme, 1.000 indirim | Bedel 9.000 (alım gününde); ödeme 3.000 değişmez; kalan 6.000 |
| 10.000 alım, 9.000 ödeme, 2.000 iade | Bedel 8.000; borç kapanır; 1.000 iade alacağı; hesap değişmez |
| A'dan 10.000 peşin gider, bir ay sonra B'ye 1.000 iade | A'nın çıkışı 10.000 kalır; gider 9.000; B'ye iade gününde +1.000 |
| 3.000 borç para, 2.000 ödeme + 1.000 vazgeçme | Borç 0; hesap −2.000; vazgeçme gününde 1.000 gelir |
| 4.500 kalan alacak tahsil edilemeyecek | Kalan 0; o gün 4.500 gider; satış geliri değişmez |
| 950 faturayı kartla kapatma, sonra kart ödeme | Gider bir kez 950; önce kart +950 / banka 0; sonra banka −950 |
| Faizli taksidi kartla ödeme | Anapara yeniden gider olmaz; faiz payı gider olur; kart borcu taksit kadar artar |
| 18.000 kalan / 15.800 anapara devri | Hesap ve gelir/gider açılışı yok; net varlık 15.800; plan 18.000 |
| Eski `Unrecorded` borç; migration ve yedekten dönüş | Bilinmeyen kalır; devir sayılmaz; tamamlanabilir |
| 4.000 alacak / 1.500 borç, 1.500 mahsup, sonra geri alma | Önce 2.500 / 0, sonra 4.000 / 1.500; hesap hareketi yok |
| Mahsuplu borcu iptal etmek | `*.offset_locked` |
| 1.000 vadeli alıma 1.200 cari ödeme | Listede 200 alacak; kırpılmaz |
| Aynı ödeme isteği iki kez; eşzamanlı iki ödeme | Tek etki; kalan aşılmaz |
| Ödeme → plan değişikliği → eski ödemeyi geri alma | Bağ korunur; tutar yeni planın ilk açık taksidine eklenir |
| Cari kişiyle alım, `Cari hesaba`, sonra `Planı değiştir` | Cari kayıtta plan değişikliği yoktur; kayıt cariden çıkmaz |
| Pasif kişi; başka kullanıcının borcu | Ödeme yazılır, yeni borç reddedilir; başkasının kaydı `NotFound` |
| Migration ve yedek | İptal edilmişler, ödeme günleri, faiz/anapara, POS yatış ve gün sonu bağları, eski kimlikler birebir |

## Sonuçları

- **Sözleşme.** `/api/v1/obligations` uçları ve borç uçları yeni borç uçlarına
  geçer; eski uçlar kalkar (istemci aynı commit'te değişir). Cari uçları kalır,
  ödeme isteklerine istek kimliği eklenir. Ayrıntı ilgili API sözleşme
  belgesinde, uygulamayla aynı commit'te yazılır.
- **Şema.** Yeni tablolar ve migration; yükümlülük ve borç planı tabloları
  taşınır. Veri kaybettiren adım yoktur.
- **Yedek.** Sürüm ilerler; eski sürüm okunur.
- **Okumalar.** `RecognizedItems`, `AccountMovements`, `CardDebt`, planlananlar,
  net varlık, kişi bakiyeleri, fatura tekrar araması, gün sonu ve hatırlatmalar
  yeni modeli okur. Yeni bir tanıma ya da para yolu bu listelere eklenir.
- **Bilinen bedel.** Cari ile planı olmayan borç kullanıcı için iki ayrı "açık
  hesap" olarak kalır (K22 bunu bir soruya çevirir). Yanlış yola yazılan alım
  taşınamaz; düzeltme iptal ve yeniden yazmadır. Kapsam büyüktür: T10'un
  dalgaları bu yüzden vardır.
- **Belgeler.** Kabulle birlikte: `PROJECT-ROADMAP.md` ADR tablosu,
  `stages/README.md`, `CLAUDE.md`, ADR 0020'nin başına not. Davranışı anlatan
  `documentation/` belgeleri davranışla aynı commit'te güncellenir.

## Bu ADR neyi karara bağlamaz

- Ekranların yerleşimi. Brifle Claude Design'a verilir; panolar yalnız kuralı
  gösterir. Öneri: borcun ayrıntısında tek ana eylem (`Ödeme kaydet`), gerisi
  tek bir `Düzelt` menüsünde (dış göz R6).
- Kişileri birleştirme (dış göz R7).
- Cari ile planı olmayan borçların uzun vadede tek görünümde birleşmesi (dış göz R2).
- Ödemenin birden çok borca kendiliğinden dağıtılması (F29).
- Kartla ödemede taksitlendirme; faizli borçta kısmi ödeme ve plansız faizli borç.
- Faizli borçta kalandan azla anlaşmalı kapanış.
