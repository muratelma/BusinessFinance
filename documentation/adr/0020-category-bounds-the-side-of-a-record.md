# ADR 0020 — Kategori kaydın tarafını sınırlar; hesap paranın tarafını söyler

- Durum: **Kabul edildi** (8 Ekim 2026, kullanıcı onayı). Kararlar 6–8 Ekim
  2026'da alındı. Aşama 06.3 Grup 8 "0 · Zemin"in karar kapısıdır. Uygulama
  adım adım ilerler; neyin yazıldığı aşama belgesindedir.
- **Nasıl okunur:** bu ADR iki katmanlıdır. **§İlkeler bağlayıcıdır**; değişmesi
  kullanıcı kararı ve bu ADR'nin güncellenmesini ister. **§Başlangıç tasarımı**
  uygulamanın ilk hâlidir; kod yazılırken daha iyi ya da gerekli bir yol
  bulunursa ilkeleri bozmadan değişebilir (`AGENTS.md` "Kararlardan sapma").
- Bağlam: tarafın (`İşletme` / `Şahsi`; kodda `TransactionScope`, eski
  belgelerde "kapsam") her girişte aynı kuralla yazılması
- **Yerini aldığı:** ADR 0013 §2 ("Kapsam nereden gelir") ve o ADR'nin
  "Sonuçlar"ındaki "varsayılan zinciri (hesap → kategori)" maddesi. ADR 0013'ün
  tek havuzu (§1), tek anahtarı (§3) ve "kâr" denmez kuralı (§6) aynen
  geçerlidir.
- **Genişlettiği:** ADR 0013 §4 — "işletmem var" cevabı artık bir bölümün
  menüde görünmesini de ön ayar olarak belirler; yine hiçbir özelliği kapatmaz
  (İ12). ADR 0014 — cari hesapta bir borcun tek yoldan kapanması (İ10).
- İlgili: ADR 0018 İ9 (vergide taraf), ADR 0019 (gün sonu, POS, kasa), ADR 0004
  (birleşik okuma modelleri)
- Gerekçe belgeleri: `stages/06.3-butunsel-duzenleme.md` Grup 8 "0 · Zemin"
  (Y1–Y12, D1–D6, C1–C9, Z7, R1–R8), `research/DEVIR-2026-10-07.md`,
  `research/DIS-GOZ-2026-10-08-para-tarafi.md`,
  `research/DIS-GOZ-2026-10-08-karar-secimleri.md`

## Bağlam

ADR 0013 tarafı bir zincirle buluyordu: kullanıcının açık seçimi → hesabın ya da
kartın etiketi → kategorinin varsayılanı. 6–7 Ekim 2026'da tarafın yazıldığı her
giriş koddan çıkarıldı:

- Gelir ya da gider yazan **23 giriş** var. Dokuzunda kullanıcı tarafı görüp
  değiştirebiliyor; on dördünde taraf sessizce geliyor (kart sayfasından
  harcama, taksitli harcama, tekrarlayan plan, vergi toplu ödemesi, gün sonu,
  CSV ve parça kayıtlar).
- Tek kural yok, **dört kural** var: genel zincir, vergi, yatış kesintisi ve
  `Kendime aldım`.
- **Aynı olay girildiği ekrana göre farklı yazılıyor.** Kasadan alınıp şahsi
  harcanan para: kasa farkının açıklamasında `İşletme` gideri, `Kendime aldım`da
  `Şahsi` gider, `İşlem ekle`de çipi `İşletme` seçili bir kayıt.
- **Hesabın etiketi kategoriyi eziyor.** Varsayılan işletme setinde her kategori
  etiketli ve set çiftler hâlinde kurulu; kategoriler tarafı zaten söylüyor.
  Buna rağmen `Şahsi` etiketli bir hesaba bağlı POS'un satışı `Şahsi` gelir
  yazılıyor.

Üç işaretin (hesap, kategori, çip) üçü de yalnız "öneri" olduğu için hiçbiri
bağlayıcı değildi. Kullanıcının yönü: "kullanıcıya her şeyi bırakmayalım".

**Eski kuralın doğru yaptığı.** Zincir saçma değildi. Hesaplarını gerçekten
ayrı tutan kullanıcı için hesap iyi bir işaretti: ayrı hesap açmak bilinçli bir
ayrımdır ve o kullanıcı hiçbir şey seçmeden doğru tarafa yazıyordu. Bu ADR o
kolaylığın bir kısmını geri alır: işletme kartıyla personeline yemek alıp
`Yeme-içme` seçen kullanıcıda eski kural `İşletme` yazardı, yeni kural `Şahsi`
yazar ve kullanıcının doğru kategoriyi seçmesi gerekir. Yine de hesap üstün
tutulmadı, çünkü şahsi karttan işyeri gideri yapmak da aynı ölçüde gerçektir:
**hesap paranın nereden çıktığını, kategori harcamanın ne olduğunu söyler; biri
öbürünün doğrulaması değildir.** Karışık kullanan esnafın esas alınması bir
yargıdır; elde kullanıcı verisi yoktur. Kural iki kullanım biçimini de doğru
karşılayacak şekilde kuruldu: hesabın etiketi iki tarafa açık kategoride ön
seçim olarak yaşamaya devam eder (İ3).

**Doğrulananlar (8 Ekim 2026).** Kararların dayandığı iki iddia çalıştırılarak
sınandı:

- *Aynı borç iki yoldan kapanıyor.* Kişiye bağlı 1.000 liralık fatura → cari
  `Ödeme` 1.000 (hesap 5.000 → 4.000, kişi kapalı, yükümlülük hâlâ açık) →
  `Öde ve kapat` (hesap 3.000, kişi 1.000 fazla ödenmiş). Gider bir kez yazıldı;
  iki kez çıkan paradır.
- *İşletmesi olmayan kullanıcı kendi açtığı kategoriyle kayıt giremiyor.*
  Kategori etiketsiz açılıyor, hesap etiketsiz ve çip görünmüyor; istek
  "taraf belirlenemedi" ile reddediliyor.

Yönün kendisi on bir senaryoda koddaki kayıt yollarına karşı **okunarak**
yürütüldü, çalıştırılmadı; kararlar iki turda bağımsız bir okumadan geçti.

## İlkeler (bağlayıcı)

**İ1 — Kategori, kaydın alabileceği tarafları belirler.** Kategori yalnız
`İşletme` ya da yalnız `Şahsi` ise kaydın tarafı odur. İki tarafa açıksa İ2
uygulanır.

**İ2 — İki tarafa açık kategoride tarafı olayın bağlamı, bağlam yoksa
kullanıcının seçimi belirler.** Bağlam, girişin kendisinin tek bir tarafa ait
olmasıdır (POS satışı işletme satışıdır). Bağlamların listesi **kapalıdır**
(T2); yeni bir giriş kendine bağlam uyduramaz, listeye eklenir. Bağlam da seçim
de yoksa kayıt yazılmaz: **sunucu taraf uydurmaz.**

**İ3 — Hesabın ve kartın etiketi paranın tarafını söyler, kaydın tarafını
belirlemez.** İki işi vardır: İ2'deki seçimin ön değeri olmak ve bir kaydın
öbür tarafın listesinde para hareketi olarak görünmesine ipucu vermek (İ7).
Hesap ile kategorinin farklı tarafta olması hata değildir; uyarı ve engel
yoktur. Ölçüt: uygulama parayı yönetmez, olanı kaydeder.

**İ4 — Çelişen açık seçim reddedilir.** İstek, kategorinin izin vermediği **ya
da girişin bağlamıyla çelişen** bir taraf taşıyorsa sunucu onu sessizce
düzeltmez ve yok saymaz; reddeder. İstek taraf taşımıyorsa bağlam belirler.
Örnek: iki tarafa açık kategoriyle yazılan bir POS satışı `Şahsi` isterse
kategori buna izin verir ama bağlam vermez; istek reddedilir. Aksi hâlde
istemcinin gösterdiği ile yazılan ayrışırdı.

**İ5 — Taraf kaydın üzerinde saklanır ve yeniden türetilmez.** Kategori neye
izin verildiğini söyler; kaydın tarafı kendi alanıdır. Kategorinin tarafı
sonradan değişirse yazılmış kayıt yeniden yorumlanmaz; plan kendi tarafını
korur ve ürettiği kayıt tarafı plandan alır.

**İ6 — Kategorinin tarafı yalnız genişler.** Tek taraflı kategori her zaman iki
tarafa açılabilir. Daraltmak ya da çevirmek yalnız öbür tarafta kayıt, plan ve
bütçe yoksa mümkündür.

**İ7 — Kayıt bir kez, kendi tarafında sayılır.** Gelir ve gider toplamlarında
`İşletme + Şahsi = Hepsi` her zaman tutar. Bir kaydın, parasının girdiği ya da
çıktığı öbür tarafın listesinde görünmesi bir sayım değildir ve hiçbir gelir ya
da gider toplamını değiştirmez.

**İ8 — Ad tarafı söyler.** Taraf bağlayıcı olduğu için tek taraflı varsayılan
kategorinin adı tarafını belli eder (`İşyeri kirası`, `Ev faturaları`). Adı iki
tarafta aynı anlama gelen kalem (vergi, faiz, sigorta) iki tarafa açıktır.

**İ9 — Cari hesap işletmeye özeldir.** Cari borçlandırma (veresiye satış, vadeli
alım) her zaman `İşletme` yazılır. Şahsi kişi borçlarının yeri `Borç ve
alacaklar`dır.

**İ10 — Bir borç tek yerde durur ve tek yoldan kapanır.** Cari bakiye yalnız
cari hareketlerden oluşur. Kişiye bağlı tek seferlik yükümlülük o kişinin
bilgisidir: cari bakiyeye girmez ve yalnız kendi kapanışıyla kapanır. Bir
kişinin borçlarının birlikte gösterilmesi, hepsinin aynı ödemeyle kapanmasını
gerektirmez.

**İ11 — Tek havuz değişmez** (ADR 0013 §1, §3). Bakiye, kart borcu ve net varlık
tarafla bölünmez. Kasadan yapılan şahsi harcama tek kayıttır; "işletmeye gider +
şahsiye gelir" diye bölünmez.

**İ12 — "İşletmem var" cevabı bir ön ayardır; kayıtların tarafını değiştirmez
ve özellik kapatmaz** (ADR 0013 §4'ü genişletir). Cevap üç şeyi belirler: hangi
varsayılan kategori setinin kurulacağı, tarafın arayüzde sorulup sorulmayacağı
ve işletmeye özel bölümlerin menüde görünüp görünmeyeceği. Cevabı "yok" olan
kullanıcıda taraf sorulmaz: iki tarafa açık kategoride hesabın ya da kartın
etiketi varsa o, yoksa `Şahsi` yazılır. Etiket kullanıcının kendi işaretidir
ve cevaba üstün tutulur: cevabını sonradan "yok" yapan kullanıcının `İşletme`
etiketli kasasından yazdığı sigorta gideri `İşletme` kalır; hiç işletmesi
olmamış kullanıcıda etiket de iki tarafa açık kategori de yoktur ve her kayıt
`Şahsi`dir. İ1 ve bağlamlar bu kullanıcıda da geçerlidir. Cevap yazılmış
kayıtların, var olan planların (İ5) ve bağlamı olan girişlerin (İ2, İ9) tarafını
değiştirmez: cevabını sonradan "yok" yapan kullanıcının işletme planı `İşletme`
üretmeye, cari borçlandırması `İşletme` yazılmaya devam eder. Menüden gizlenen
bölüm kilit değildir: o bölümde kaydı olan kullanıcıda görünür kalır ve sunucu
onu kapatmaz.

Reddedilenler (ilke düzeyinde):

| Reddedilen | Neden |
|---|---|
| Hesabın etiketinin kategoriyi ezmesi (ADR 0013 §2'nin zinciri) | Satışın tarafını hesabın belirlemesi ve aynı olayın üç ekrandan üç farklı yazılması bu zincirin sonucuydu |
| Her kayıtta taraf sormak | Kullanıcının yönü: "kullanıcıya her şeyi bırakmayalım". Çoğu kategoride cevap kategoriden bellidir |
| Kategori ile hesap farklı taraftaysa uyarı penceresi | Şahsi kartla işyeri gideri de, kasadan market de olağan olaylardır; uyarı olağanı hata gibi gösterir (İ3) |
| Tarafın yalnız kategoriden hesaplanıp kayıtta saklanmaması | Kategorinin tarafı değişince geçmiş yeniden yazılırdı (İ5) |
| Kişinin yükümlülükten kaldırılması, kişili faturanın cariye yazılması | Fatura Planlananlar'dan ve yaklaşan ödeme toplamından düşerdi, şahsi faturada satıcı seçilemezdi (İ9), hangi faturanın ödendiği bilgisi kaybolurdu. 7 Ekim'de karar olarak yazılmış, 8 Ekim'de geri alınmıştır |
| Cari ödemenin kişinin faturalarını da kapatması | Kısmi ödeme ve fatura eşleştirme ister; ihtiyacın zorunlu olduğuna dair dayanak yok (`research/fikir-kaydi.md` F29) |
| Etiketsiz kartın ekstresini taraflara bölerek göstermek | Ekstre tek ödemedir; bölünmüş tutar hiçbir yerde ödenmeyen, türetilmiş bir sayı olurdu |

## Başlangıç tasarımı (uygulamada değişebilir)

Aşağıdakiler Aşama 06.3 Grup 8 "0 · Zemin"in ilk hâlidir; numaralı kararların
ayrıntısı aşama belgesindedir. Kod yazılırken değişirse sapma aşama belgesinin
"Sapmalar" tablosuna yazılır; kullanıcının gördüğü davranışı değiştiren sapma
uygulanmadan önce kullanıcıya sorulur.

### T1. Formlar (İ1–İ4)

- Tek taraflı kategoride çip çıkmaz; taraf bilgi olarak yazar ("Şahsi ·
  kategoriden").
- İki tarafa açık kategoride çip çıkar; ön seçim hesabın ya da kartın
  etiketinden gelir. O da boşsa kullanıcı seçmeden kayıt gitmez.
- Çip üç forma eklenir: kart sayfasından harcama, taksitli harcama, tekrarlayan
  plan.
- İstemcide kopyalanan bir sıra kalmaz: form ya kategorinin tarafını yazar ya
  çip sorar. Durum tablosu (kategorinin türü × hesabın etiketi × giriş) sunucu ve
  istemci testlerinin ortak girdisidir.

### T2. Bağlamlar (İ2'nin kapalı listesi)

| Giriş | Yazdığı taraf | Listelediği kategoriler |
|---|---|---|
| POS tahsilatı, gün sonu | `İşletme` | `İşletme` ve iki tarafa açık gelirler |
| POS komisyonu, yatış kesintisi | `İşletme` | `İşletme` ve iki tarafa açık giderler |
| Cari borçlandırma | `İşletme` (İ9) | `İşletme` ve iki tarafa açık kategoriler |
| Kasa farkının açıklaması | Kategorinin tarafı; iki tarafa açıksa ve `Bilmiyorum`da `İşletme` | Hepsi |
| `Kendime aldım` | `Şahsi` | `Şahsi` ve iki tarafa açık giderler |
| Vergi | Açık seçim → profilin tarafı (ADR 0018 İ9; değişmez) | Vergi işaretli kategoriler; iki tarafa açıktır |
| Plandan doğan kayıt (tekrarlayan, taksit) | Planın tarafı (İ5) | — |

Listede olmayan kategori o girişte seçilemez ve sunucu da reddeder. Bu tablo
dışındaki her giriş T1'e göre davranır.

### T3. Soru sorulamayan ve kategorisiz yollar

- Kategorisi olmayan kayıt (nakit borç alma ve verme) iki tarafa açık kategori
  gibi davranır: çip çıkar, ön seçim hesaptan gelir.
- CSV içe aktarmada tarafı çözülemeyen satıra inceleme adımında çip gelir
  (bugün bütün aktarım reddediliyor).
- İşletmesi olmayan kullanıcının açtığı yeni kategori `Şahsi` yazılır (İ12).

### T4. Düzeltme

- Tarafı düzeltme yalnız iki tarafa açık kategorideki kayıt için gerekir.
- Yanlış kategori bugünkü gibi iptal ve yeniden girişle düzelir.

### T5. Kategori seti (İ8)

İşletme setinde değişenler; kişisel set değişmez (orada taraf görünmez). Set
yalnız yeni kullanıcıya kurulur.

| Kategori | Bugün | Olacak | Paket |
|---|---|---|---|
| `SGK ve vergi ödemesi` | `İşletme` | İki tarafa açık | Çekirdek |
| `Faiz ve finansman gideri` | `İşletme` | İki tarafa açık | Çekirdek |
| `Sigorta` | `İşletme` | İki tarafa açık | Çekirdek |
| `Elektrik, su, doğalgaz` | `İşletme` | `İşyeri faturaları` | Çekirdek |
| `Faturalar` | `Şahsi` | `Ev faturaları` | Çekirdek |
| `Konut` | `Şahsi` | `Ev kirası ve aidat` | Çekirdek |
| `Personel ücreti` | `İşletme` | `Personel giderleri` | Çekirdek |
| `Hediye` (gider) | Yok | Eklenir, `Şahsi` | Sonrası |
| `Ekipman ve demirbaş` (gider) | Yok | Eklenir, `İşletme` | Sonrası |
| `Maaş` (gelir) | Yok | Eklenir, `Şahsi` | Sonrası |

`Kasa farkı` ve faiz kategorisi bugün adıyla bulunuyor; kalıcı bir işaretle
bulunmaları ikinci paketin işidir.

### T6. Cari, yükümlülük, borç (İ9, İ10)

| Bölüm | İşi | Taraf | Kişi |
|---|---|---|---|
| `Cari hesap` | Kişiyle yürüyen işletme hesabı | Her zaman `İşletme` | Zorunlu |
| `Yükümlülükler` | Tek seferlik, vadeli fatura ya da alacak | İki taraf; kategori söyler | İsteğe bağlı, yalnız bilgi |
| `Borç ve alacaklar` | Kişiyle yapılan borç; şahsi kişi borçlarının yeri | İki taraf | Zorunlu |

- Cari kuralının yeri uygulama katmanıdır; Domain invariant'ı ve SQL kısıtı
  eklenmez (eski yedekteki şahsi cari kayıt geri yüklenebilmelidir).
- İ10 bir **okuma kuralıdır** ve üç okumayı değiştirir: kişinin cari bakiyesi,
  işlem sonrası açık bakiye ve **yükümlülüğün kendi ayrıntısı** (yükümlülük ve
  kapanışı bugün "karşı tarafın açık bakiyesi"ni değiştiren olay gibi
  sunuluyor; artık sunulmaz, kişi bilgi olarak kalır). Eski kayıtlar ve geri
  yüklenen yedekler aynı kurala girer; şema ve yedek şeması değişmez. Net
  varlık ve Planlananlar yükümlülüğü zaten kendi tablosundan okur.
- Kişinin ayrıntısında açık faturalar ayrı blokta durur ve bilgi amaçlı bir
  toplam gösterilir (cari borç + açık faturalar + borç planları). Toplam
  sunucudan gelir ve düğmesi yoktur; her borç kendi düğmesiyle kapanır.
- İşletmesi olmayan kullanıcıda `Cari hesap` menüsü gizlenir; cari hareketi olan
  kullanıcıda görünür kalır (İ12).
- Fiş okumanın "ödemedim" yolu değişmez.

İ10 kullanıcının iki yoldan ödemeye **yönlendirilmesini** kaldırır; elle fazla
tutar yazmayı engellemez. Fazla ödeme kırpılmaz (ADR 0014).

### T7. Dükkan kasası

Satışın yazıldığı dükkan kasası tektir; Kasa hiçbir yerde taraf sormaz, gün
sonunda hesap seçimi kalkar ve "zaten girilmiş" okumaları türü nakit olan
hesapları değil o kasayı okur. Kasanın nasıl belirleneceği Grup 6'dadır.

### T8. Öbür tarafın listesinde görünme (İ7)

Para taşıyan kayıt, bağlı olduğu kaydın tarafında ve parasının girip çıktığı
etiketli hesabın ya da kartın tarafında görünür; hiçbir ipucu yoksa iki tarafta.
Yedi kayıt türünün kuralı aşama belgesinde, Z7 tablosundadır. Bu parça **en son,
tasarımıyla birlikte** uygulanır; o zamana kadar taşıyan kayıt taraf seçiliyken
bugünkü gibi yalnız `Hepsi`de görünür.

### T9. Uygulama sırası

1. **Çekirdek:** sunucuda taraf kuralı ve durum tablosu → kategori seti (T5'in
   çekirdek satırları), İ6 ve T3'ün son maddesi → formlar (T1, T2) → cari (T6)
   → Kasa (T7; kasa belirlenince). Tek başına teslim edilir ve cihazda denenir.
2. **Sonrası:** yeni kategoriler, kalıcı kategori işareti, borç formunun
   faizsiz varsayılanı, iki taraflı bütçe, T4.
3. **En son:** T8 ve sayfalar.

## Sonuçları

- **Sözleşme.** Var olan uçların davranışı değişir: kategoriyle çelişen açık
  seçim, cari borçlandırmada şahsi kategori ve T2'de listede olmayan kategori
  yeni hata kodlarıyla reddedilir; kişinin cari bakiyesi yükümlülükleri
  içermez ve yükümlülüğün ayrıntısı cari bakiye döndürmez. Yükümlülük isteği
  değişmez.
- **Şema.** Cari bölümü için değişiklik yoktur. Kategorinin tarafı bugünkü
  nullable etikettir (boş = iki tarafa açık); çekirdeğin şema değişikliği
  istemediği öngörülüyor ve uygulama sırasında doğrulanır. Kalıcı kategori
  işareti ve iki taraflı bütçe ikinci pakette migration ister.
- **Veri.** Yerel sentetik veri kurala geçerken sıfırlanabilir; eski kuralla
  yazılmış çelişen kayıtlar taşınmaz.
- **Bilinen bedel.** Bütün yük kategoriye biner: yanlış kategori sessizce yanlış
  taraf demektir. Bakiyeyi bozmaz ama **işletme netini yanlış gösterir**; bu,
  uygulamanın temel sorusuna yanlış cevaptır. Azaltımı İ8 (ad tarafı söyler),
  T1 (form tarafı gösterir) ve genel anlamlı kalemlerin iki tarafa açık
  olmasıdır; yok etmez. Düzeltme iptal ve yeniden girişle yapıldığı için plana
  ya da gün sonuna bağlı kayıtta pahalıdır ve geçmişi olan tekrarlayan planın
  tarafı bugün hiç düzeltilemiyor. Çekirdeğin cihaz kabulü bu yüzden yalnız
  doğru girişi değil, yanlış kategoriyle girilmiş kaydın düzeltilmesini de
  dener.
- **"İşletmem var" cevabına bağımlılık artar** (cari menüsü, yeni kategorinin
  tarafı, vergi kuralı). O soru kalkarsa üçü başka bir kaynağa bağlanır;
  istemcide tek kaynak `ScopeController.isVisible`dır.
- **Belgeler.** Kabulle birlikte güncellendi: `PRD-BusinessFinance.md` §6.2'nin
  iki satırı (zincir satırı ve "kategoride isteğe bağlı varsayılan kapsam";
  kategoride artık varsayılan değil, izin verilen taraflar vardır), ADR 0013'ün
  başındaki not, `PROJECT-ROADMAP.md` ADR tablosu, `stages/README.md`,
  `CLAUDE.md` ve `AGENTS.md`'deki yönlendirme. `CLAUDE.md` ve
  `documentation/architecture.md` içindeki "kapsam tek yerde türetilir" ve
  "kapsamsız satır düşer" kuralları bugünkü kodu anlatır; davranışla aynı
  commit'te güncellenir.

## Bu ADR neyi karara bağlamaz

- Sayılmayan satırın görünüşü, kategori sayfasının ve hesap sayfasının listesi:
  ilgili sayfa çizilirken.
- Taraf seçiliyken **yaklaşan ödeme toplamı**: etiketsiz kartın ekstresi iki
  tarafta da görünürse o toplamda `İşletme + Şahsi = Hepsi` tutmaz. İ7'nin
  eşitliği gelir ve gider toplamlarının kuralıdır; taşıyan kayıtların toplamı
  T8 ile birlikte tanımlanır.
- Hesabın ya da kartın etiketi değişince eski satırların öbür taraftaki
  görünürlüğü.
- Dükkan kasasının nasıl belirleneceği (Grup 6) ve çok kasa (F28).
- Tek ödemeyle birden çok faturayı kapatma (F29).
- Kişinin bilgi amaçlı toplamında borç planının hangi tutarla sayılacağı (kalan
  anapara mı, faiz dahil kalan ödeme mi) ve alacak ile borcun birlikte nasıl
  gösterileceği: cari sayfası çizilirken.
- "İşletmem var" sorusunun kalkması; cevabı "yok" olup `İşletme` kaydı bulunan
  kullanıcıda o kayıtların tarafının nasıl gösterileceği.
