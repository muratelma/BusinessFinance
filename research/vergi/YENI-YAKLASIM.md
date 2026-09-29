# Vergi = nakit planı — yeni yaklaşım

**Durum: KAPANDI (28 Eylül 2026, kullanıcı onayı); 29 Eylül karar denetimiyle güncellendi.**
**Geçerli kararların tek tablosu §6.6'dır.** §1–§6.5 gerekçe ve karar geçmişidir; içlerindeki bazı satırlar
aşıldı (ör. §1 "iki yıllık kalem", §5 "veri kalır", §6.2 satır 1 ve 5, §6.3 tutar satırları, §6.4 durum
sütunu ve hazır türlerdeki "Tutar" sütunu). Bu belgenin içinde çelişki olursa §6.6 geçerlidir; ADR 0018
ya da aşama belgesiyle çelişirse onlar geçerlidir ve çelişki kullanıcıya sorulur.
Uygulama paketi: `YOL-HARITASI.md` P2 (kaldırmalar) ve P3 (vergi ekranı). Eski belge (`KARAR.md`) aşıldı.

## 0 · Kullanıcı kararları (28 Eylül)
1. Muhasebeci paketi ve KDV alanları kaldırılır.
2. Vergi takvimi kendi ekranı olur: vergiler orada tanımlanır, ödemesi onaylanır, tutar onay anında girilir;
   hazır vergi türleri kategori gibi seçilir, kullanıcı kendi türünü de ekler.
3. Muhasebeciden belge alma şimdilik yok (F17). Kasa'da gün sonu raporunu kamerayla okumak yapılacak (F04).

## 1 · Vergiler gerçekte ne zaman ödeniyor

| Vergi | Ritim | Kim öder | Uygulamada karşılığı | Kaynak |
|---|---|---|---|---|
| Bağkur (4/b) | Her ay, ay sonu | Herkes | Aylık | [1] |
| KDV | Her ay, 28'i | KDV mükellefi | Aylık | [2] |
| Muhtasar ve prim hizmet | Her ay, 26'sı | Çalışanı ya da stopajlı ödemesi olan | Aylık | [2] |
| Geçici vergi | 3 ayda bir, 17 Şubat · 17 Mayıs · 17 Ağustos · 17 Kasım (4. dönem 2025'ten beri yeniden var) | Gerçek usul | Üç aylık (var) | [3] |
| Yıllık gelir vergisi | 2 taksit: Mart ve Temmuz | Gerçek usul | İki yıllık kalem | [3] |
| Emlak vergisi + çevre temizlik (işyeri) | 2 taksit: Mayıs sonu ve Kasım sonu, **belediyeye** | Dükkânı olan | İki yıllık kalem | [4] |
| Motorlu taşıtlar vergisi | 2 taksit (Ocak ve Temmuz olduğu biliniyor; bu turda kaynakla doğrulanmadı) | Aracı olan | İki yıllık kalem | — |
| İlan-reklam (tabela) vergisi | Yılda bir, belediyeye (ay doğrulanmadı) | Tabelası olan | Yıllık | — |

**Uygulamanın bugünkü sıklıkları:** günlük · haftalık · aylık · üç aylık · yıllık (`RecurrenceFrequency`).
"Altı ayda bir" yok, ama gerek de yok: iki taksitli her vergi iki **yıllık** kalemle ifade edilir (ör.
"Emlak vergisi · 1. taksit" her yıl 31 Mayıs, "2. taksit" her yıl 30 Kasım). Hazır tür bunu tek dokunuşla
iki kalem olarak kurar. **Belediye vergileri bu yüzden modeli bozmaz**; yalnız ödeme yeri farklıdır, bu da
bir not alanıdır.

## 2 · Birikmiş ve geciken vergi
- Geç ödenen vergi ve Bağkur borcuna **aylık %3,7 gecikme zammı** biner; bir gün gecikme bile bir ay sayılır [5].
- Uygulama zammı **hesaplamaz**: kullanıcı ödeme onayında bankadan ne çıktıysa onu yazar. Tutar zaten
  onay anında girildiği için (karar 2) zam modeli değiştirmez.
- Gereken tek yetenek: **birden çok bekleyen vergiyi tek ödemeyle kapatmak** (ör. üç aylık Bağkur bir
  seferde). Bu, Kasa'daki "hesaba geçenleri işaretle" ile aynı desen: birden çok bekleyeni seç, tek tutar
  yaz. İki yerde aynı etkileşim → öğrenmesi tek (bütünlük).
- Zam tutarını ayrı göstermek (ör. "gecikme zammı ₺210") değer sınavında zayıf: tahmini tutar olmadığı
  için fark hesaplanamaz. **Yapılmaz.**
- Borç yapılandırılırsa (taksitlendirme) bu artık bir borç sözleşmesidir; mevcut borç özelliğiyle izlenir (F20).

## 3 · Vergi takvimi kendi ekranı olursa ne değişir

**Önerilen yol: ekran ayrı, veri aynı.** Vergi kalemi yine tekrarlayan plandır, yalnız türü "Vergi/SGK" olur.

| Soru | Cevap |
|---|---|
| Tekrarlayanlar listesinde görünür mü? | Hayır; orası "kira, abonelik, maaş" gibi düzenli ödemeler için kalır. Vergiler kendi ekranında |
| **Yaklaşanlar'da görünür mü?** | **Evet, hiçbir ek iş olmadan.** Yaklaşanlar bütün planları tek yerden okuyor; vergi de bir plan olduğu için orada kalır. Tutarı henüz yoksa "tutar ödemede girilecek" diye görünür, "7 günde çıkacak" toplamına tutarsız kalem olarak ayrıca yazılır |
| Tamamen ayrı bir kayıt türü yapsak? | Yaklaşanlar, İşlemler, yedek ve raporlara ayrı ayrı eklemek gerekirdi; kazancı yok. **Önerilmez** |
| Ekranda ne var? | Üstte **bekleyenler** (bu ay ve gecikenler, "Ödedim" düğmesiyle), altta **vergilerim** (tanımlı türler, sıradaki tarih), en altta "vergi ekle" (hazır türler + kendi türü) |
| "Ödedim" ne sorar? | Tutar · gün · nereden ödendi (hesap, kasa, kart ya da muhasebecim ödedi — §4) |
| Tutar önceden belli olursa? | İsteğe bağlı: bekleyen kaleme "tutar belli oldu" yazılabilir (uygulama bugün bekleyen kalemin düzeltilmesine izin veriyor); Yaklaşanlar'da tutarıyla görünür |
| Hazır türler | Bağkur · KDV · Muhtasar · Geçici vergi · Yıllık gelir vergisi (2 taksit) · Emlak ve çevre temizlik (2 taksit) · Motorlu taşıtlar (2 taksit) · Tabela; her birinde tek satır ipucu ("Basit usuldeyseniz genelde yalnız Bağkur") |

## 4 · Muhasebecinin esnaf adına ödediği masraflar

Durum: muhasebeci araç sigortasını ya da vergiyi kendi parasıyla öder; esnaf sonra (çoğu zaman muhasebe
ücretiyle birlikte) muhasebeciye öder.

Para akışı haritasıyla bakınca bu yeni bir yol değil, **tedarikçiden vadeli alım**la aynı yapı:
- Masraf o gün **gider tanır** (sigorta, vergi); esnafın hesabından para çıkmaz; **muhasebeciye borç** doğar.
- Esnaf muhasebeciye ödeyince borç kapanır; bu **taşımadır**, ikinci kez gider yazılmaz.
- Mevcut özellik: **cari hesap** (muhasebeci bir karşı taraf; borçlandırma + ödeme).

Vergi ekranıyla bağı: "Ödedim" sorusunda "nereden ödendi" seçeneklerinden biri **"Muhasebecim ödedi"**
olur; vergi kalemi kapanır ve muhasebecinin cari hesabına borç yazılır. Yeni olan tek şey, planın bir cari
kayda da gerçekleşebilmesi (bugün yalnız hesaba ya da karta).

Daha sade seçenek: "Muhasebecim ödedi" yok; esnaf muhasebeciye ödediği günü vergi ödemesi olarak girer.
Kaybettirdiği: vergi gerçekte ödendiği gün "bekliyor" görünür ve gecikti sanılır.

## 5 · KDV ve paketi kaldırmanın sonuçları
- **Formlar:** gelir/gider, kart harcaması, cari borçlandırma, ödenmemiş fatura ve POS formlarından
  "Vergi bilgisi" bölümü kalkar; **indirilebilirlik** de (tek tüketicisi paketti).
- **Veri:** girilmiş KDV değerleri veritabanında kalır, gösterilmez. Silmek geri alınamaz, sorulmadan yapılmaz (§7).
- **Fiş okuma:** fişteki KDV'yi okumaya devam edebilir ama kullanmaz; istemde de gereksizse çıkarılır.
- **Muhasebeci paketi** ekranı, uç noktası ve dışa aktarımı kalkar. Belge fotoğrafları kayıtlarda durmaya devam eder.
- **Belgeler:** ADR 0016'nın KDV, indirilebilirlik ve paket kısmı yeni bir ADR ile değişir; PRD §2
  ("muhasebeciye gidecek veri") ve §6.9 güncellenir; `CLAUDE.md`'deki ilgili kurallar düzeltilir.
- **Kasa:** gün sonu raporundaki KDV okunmaz (zaten sorulmayacaktı).

## 6 · Kasa gün sonu raporunun kamerayla okunması (F04)
- İki belge türü: **Z raporu** (yazar kasa POS; nakit + kredi toplamı, fiş sayısı, tarih) ve **banka POS gün
  sonu fişi** (yalnız kartlı satış toplamı). Alanlar mevzuatla standart, düzen marka marka değişir.
- Mevcut fiş okuma altyapısının (sunucu tarafında, öneri katmanı) üstüne kurulur; okunan değerler gün sonu
  panelini **doldurur**, kullanıcı onaylar.
- **Gerekli girdi:** gerçek rapor görselleri (farklı marka yazar kasa + en az bir banka POS fişi). İşletme adı
  ve vergi numarası karartılarak kullanılır.

## 6.1 · Gerekli görseller (uygulamaya geçmeden kullanıcı sağlayacak)
- 3–4 farklı marka yazar kasa POS'tan **Z raporu** (ör. Beko, Hugin, Ingenico, Pavo/Token)
- En az bir **banka POS gün sonu fişi** (yazar kasası olmayan esnafın belgesi)
- Varsa bir **yemek kartı** gün sonu dökümü
- Kural: işletme adı, adres, vergi/sicil numarası karartılır; tutarlar kalabilir.

## 6.2 · İkinci tur kararları (28 Eylül)

| # | Karar | Sonuç |
|---|---|---|
| 1 | Vergi ödemesi | Ödendiği hesaptan para düşer; **bütün vergiler işletme kapsamında** (şahsi ayrım yok — hedef kullanıcı şahıs şirketi). İşletme netini düşürür. Net varlıkta, tutarı bilinen bekleyen vergiler "Vergiler" başlığıyla (borçların yanında) görünür. İleride Özet'e ayrı vergi raporu/widget'ı (F22) |
| 2 | İndirilebilirlik | Kalkar (tek kullanıcısı paketti) |
| 3 | Eski KDV verisi | Silinebilir (hepsi test verisi). **Veri kaybettiren karar kullanıcıdan alındı**; `docs/project-status.md`'ye yazılacak |
| 4 | "Muhasebecim ödedi" | Kullanıcı Claude'a bıraktı → aşağıdaki §6.3 |
| 5 | Altı ayda bir | Kaçamak (iki yıllık kalem) değil, **kod değişikliğiyle yeni sıklık** eklenir |
| 6 | Görseller | Kullanıcı uygulamadan önce bulacak (§6.1) |

## 6.3 · Vergi tanımı ve ödeme senaryoları

Kullanıcının sorusu: "İlk başta tutar yazmak ne kadar mantıklı? Muhasebeci günü gelince ödüyor ya da rapor
veriyor; biz parayı muhasebeciye ödeyince tutarı öğrenmiş oluyoruz."

**Vergi tanımında sorulanlar** (hazır tür seçilince çoğu kendiliğinden dolar):

| Alan | Örnek | Not |
|---|---|---|
| Tür | Bağkur · KDV · … · kendi türün | Hazır türler kategori gibi |
| Sıklık | Bağkur → her ay · tabela → yılda bir · emlak → **6 ayda bir** | Türden gelir, değiştirilebilir |
| Gün / ay | Bağkur ay sonu · emlak 31 Mayıs ve 30 Kasım | Türden gelir |
| **Kim öder** | Ben · Muhasebecim | Senaryoyu belirler |
| **Tutar** | Sabit (Bağkur, emlak: yazılır) · Her dönem değişir (KDV, geçici vergi: boş) | Türden önerilir |
| Ödendiği hesap | Varsayılan hesap | "Ödedim"de değiştirilebilir |

**Üç senaryo, tek ekran:**

| Senaryo | Ekranda | Kayıt |
|---|---|---|
| **A · Kendim öderim** | Vade günü gelir, "Ödedim" → tutar (sabitse dolu gelir), gün, hesap | Vergi başına bir gider |
| **B · Muhasebecim öder, ona toplu öderim** (kullanıcının "muhasebeciye ödenen tutar" fikri) | "Muhasebecim" türündeki vergiler vadede **"gecikti" demez**, "muhasebecine ödenecek" der. **"Muhasebeciye ödedim"**: bir tutar + hangi vergileri kapattığı (hepsi seçili gelir) + isteğe bağlı her verginin payı | Payları girilmişse vergi başına gider; girilmemişse tek gider "Vergiler (muhasebeci)" ve seçilen vergilerin hepsi kapanır |
| **C · Birikmiş / gecikmiş ödeme** | Birden çok bekleyeni seç, tek tutar | A veya B ile aynı akış; gecikme zammı ödenen tutarın içindedir |

**"Muhasebecim ödedi" kararı (§7 soru 4, Claude'a bırakıldı):** Bu tur **cari hesaba bağlanmaz**; B senaryosu
yeterli. Gerekçe: kullanıcının tarif ettiği gerçek akış "parayı muhasebeciye ödeyince tutarı öğreniyoruz"; B
bunu doğrudan karşılıyor ve vergi ekranını cari hesaba bağlamıyor. Bedeli: muhasebeci vergiyi ödediği gün
ile esnafın ona ödediği gün arasında vergi "muhasebecine ödenecek" görünür — bu doğru bir durumdur (esnafın
muhasebeciye borcu var), yanlış alarm değildir. Muhasebecinin araç sigortası gibi **vergi dışı** masrafları
ödemesi cari hesapla karşılanır (F18); cari hesabın geliştirilmesi ayrı tema (F21).

## 6.5 · Dördüncü tur (28 Eylül) — 6.4'ü günceller

| Konu | Karar |
|---|---|
| Hazır türler | **Yalnız sıklık ve gün** getirir. "Tutar tipi" (sabit/değişken) kaldırıldı: yanlış sınıflandırma riski, kazancına değmez (kullanıcı). Kullanıcı sıklık ve günü değiştirebilir |
| V-K9 yeniden | Kullanıcının kastettiği: vergileri tek tek tanımlamadan **"bu ay ödediğim vergiler: X TL"** diye toplu yazmak. Vergi ekranında **"Vergi ödemesi ekle"**: tutar, gün, hesap (zorunlu), isteğe bağlı not ve dönem. Tanımlı bir vergiye bağlı olmak zorunda değil. Tanımlı bekleyen vergiler varsa hangilerini kapattığı isteğe bağlı işaretlenir |
| Vergi ödemesinin kimliği | Vergi kategorisi (işletme setinde "SGK ve vergi ödemesi"). Vergi ekranının "Ödenenler" listesi bu kategorideki giderlerdir; Gider formundan bu kategoriyle girilen ödeme de orada görünür — **tek gerçek, iki kapı**. İleride "her ay ne kadar vergi ödedim" raporu (F22) bu kategoriden okunur |
| V-K8 yeniden | "Muhasebeciye ödedim" ayrı bir akış değil: toplu vergi ödemesinin kendisi. **Vergi dışı kısım satırı yok** — muhasebe ücreti bugün de ayrı bir gider / tekrarlayan plan (test verisinde "Muhasebeci ücreti" planı var) |
| V-K13 | **Kaldırıldı.** Ödenmemiş vergi net varlığı ve işletme netini etkilemez; ödendiği gün etkiler (kullanıcı). Kart borcundan farkı: kartta tutar harcama günü bellidir, vergide çoğu zaman ödeme gününe kadar bilinmez |
| Ay kayması | Nakit esası: Eylül KDV'si 28 Ekim'de ödenir, Ekim'in gideridir. Sorun değil; ödemede isteğe bağlı **"Dönem"** etiketi (tanımlı vergiden kendiliğinden, toplu ödemede seçilir) raporları etkilemez, yalnız bilgi verir |
| V-K4 | Kullanıcı onayladı: kod değişikliğiyle sağlam çözüm — sıklık **"seçilen aylarda"** + mevcut başlangıç/bitiş tarihi. Uygulama P3'te |
| V-K5 | Tanımda hesap isteğe bağlı; **ödemede zorunlu** (kullanıcı onayladı). Tekrarlayan planın "kaynak zorunlu" kuralı vergi türünde gevşer |

## 6.6 · Geçerli kararlar (29 Eylül, denetim sonrası — tek tablo)

Kaynak: 28 Eylül kararları (§6.2–§6.5) + `research/DENETIM-2026-09-29.md` ve dış kaynak eki; kullanıcı:
"en uygun çözümleri uygulayabilirsin". Numaralar izlenebilirlik için korundu; kaldırılanlar işaretli.
**Statü:** bağlayıcı ilkeler ADR 0018'in "İlkeler" bölümündedir; bu tablonun ayrıntıları **başlangıç
tasarımıdır** ve `AGENTS.md` "Kararlardan sapma" kuralıyla değişebilir.

| # | Karar | Kaynak |
|---|---|---|
| V-K1 | Muhasebeci paketi **tamamen** kalkar (ekran, uç, dışa aktarma); KDV alanları (5 form) ve indirilebilirlik kalkar; kolonlar ve eski veri silinir. Gerekçe: bütçe uygulamasıyız, ön muhasebe değil; muhasebeciye veri vermek bizim işimiz değil | Kullanıcı (28 ve 29 Eyl) |
| V-K2 | Vergiler kendi ekranında: ekle, düzenle, sil/duraklat, "Ödedim", toplu ödeme, "Ödenenler". Arka planda tekrarlayan plan + **ayrı bir vergi türü alanı**; Tekrarlayanlar listesinde görünmez, **Yaklaşanlar'da görünür** | Kullanıcı + denetim B6 |
| V-K3 | Hazır türler **yalnız sıklık ve günü** getirir; kullanıcı değiştirir ve kendi türünü ekler | Kullanıcı (§6.5) |
| V-K4 | Sıklık **"seçilen aylarda"** eklenir; genel tekrarlayan formda da açılır. "Üç aylık" kalır | Kullanıcı; denetim B16 |
| V-K5 | Tanımda hesap/kart **isteğe bağlı**. **"Ödedim": tutar + gün + hesap ya da kart zorunlu** (kartla ödeme kart harcaması yazar). Kayıt vade gününe değil ödeme gününe yazılır. Kaynak zorunluluğu vergi türünde gevşer | Kullanıcı; denetim B5, B20, B25 |
| V-K6 | ~~"Kim öder"~~ — **kaldırıldı**: alan yok | Denetim B21 |
| V-K7 | Tanımda tutar **isteğe bağlı** (sabit vergide yazılabilir). Tutarsız plan olabilir; toplamlarda tutarsız kalemler ayrıca yazılır ("2 kalemin tutarı belli değil"). Tutar önceden belli olursa bekleyen kaleme "tutar belli oldu" ile yazılır | Kullanıcı; denetim B3 |
| V-K8 | ~~"Muhasebeciye ödedim"~~ — **kaldırıldı** (§6.5): toplu vergi ödemesinin kendisi; vergi dışı satır yok | Kullanıcı |
| V-K9 | **Toplu vergi ödemesi** ("Vergi ödemesi ekle"): tutar, gün, hesap/kart (zorunlu), isteğe bağlı not. Tanımlı bir vergiye bağlı olmak zorunda değil. Tanımlı bekleyenler listelenir, **vadesi gelmiş ve geçmiş olanlar seçili gelir**; seçilenler **"kapatıldı"** olur. Tutarla kendiliğinden eşleştirme yok (çoğu kalemin tutarı boş). Gecikme zammı hesaplanmaz, ödenen tutarın içindedir | Kullanıcı (§6.5); denetim B4 |
| V-K10 | ~~"Muhasebecine ödenecek" etiketi~~ — **kaldırıldı**: gecikmiş vergi her durumda "gecikti" der | Denetim B21 |
| V-K11 | Cari hesaba bağlanmaz (muhasebecinin ödediği vergi dışı masraflar cari hesapla — F18, F21) | Claude kararı (kullanıcı bıraktı) |
| V-K12 | Kapsam sorulmaz: **işletme profilinde vergiler işletme**, kişisel profilde şahsi; **MTV ve emlak türlerinde tanımda şahsi seçilebilir** (şahsi araç, konut). İşletme kapsamlı vergi işletme netini düşürür | Kullanıcı; denetim B7 |
| V-K13 | ~~Net varlıkta bekleyen vergiler~~ — **kaldırıldı** (§6.5): ödenmemiş vergi net varlığı ve işletme netini etkilemez, ödendiği gün etkiler | Kullanıcı |
| V-K14 | Silme: hiç ödenmemiş vergi silinir; ödeme geçmişi varsa duraklatılır. **Gerçekleşmiş ödeme vergi ekranından geri alınabilir**, kalem yeniden bekleyene döner | Karar; denetim B5 |
| V-K15 | Tahakkuk fişi okuma yok (F17). Kullanıcının muhasebecisinden aldığı tutarı kendisi girer | Kullanıcı |
| V-K16 | Özet'e vergi raporu ileride (F22) | Kullanıcı |
| V-K17 | **Kimlik:** kategoride bir **"vergi" işareti**; iki varsayılan sette birer kategori işaretli gelir ("SGK ve vergi ödemesi", "Vergi ve harç"). "Ödenenler" = işaretli kategorilerdeki giderler; Gider formundan girilen vergi de orada — tek gerçek, iki kapı | §6.5; denetim B6 |
| V-K18 | **Dönem:** ayrı alan yok; dönem kapatılan kalemden okunur; tanımsız toplu ödemede not alanı | §6.5; denetim B22 |

Hazır türler (V-K3; tutar yok — V-K7):

| Tür | Sıklık | Gün | Not |
|---|---|---|---|
| Bağkur | Her ay | Ay sonu | Herkes. Hangi aya ait olduğu SGK kaynağından doğrulanacak (dış kaynak §3) |
| KDV | Her ay | 28 | KDV mükellefi |
| Muhtasar ve prim hizmet | Her ay | 26 | Çalışanı ya da stopajlı ödemesi olan; süre uzatmaları olabilir |
| Geçici vergi | Seçilen aylarda: Şub, May, Ağu, Kas | 17 | Gerçek usul |
| Yıllık gelir vergisi | Seçilen aylarda: Mar, Tem | Ay sonu | Gerçek usul |
| Emlak ve çevre temizlik | Seçilen aylarda: May, Kas | Ay sonu | Dükkânı olan; belediyeye; şahsi seçilebilir |
| Motorlu taşıtlar | Seçilen aylarda: Oca, Tem | Ay sonu | Aracı olan; şahsi seçilebilir |
| İlan-reklam (tabela) | Yılda bir: Ocak | Ay sonu | Tabelası olan; belediyeye |

## 6.4 · Vergi teması kapanış listesi (ilk sürüm; 6.5 ile güncellendi — **aşıldı, geçerli olan §6.6**)

Üçüncü tur (28 Eylül) eklemeleri: hesap tanımda zorunlu değil; muhasebeci alanları seçenek, zorunlu değil.
MTV (Ocak, Temmuz) ve tabela (Ocak sonu) doğrulandı; yıllık gelir vergisinin Mart–Temmuz ritmi "6 ayda bir"e
sığmadığı için V-K4 önerisi.

| # | Karar | Durum |
|---|---|---|
| V-K1 | Muhasebeci paketi, KDV alanları (5 form), indirilebilirlik kalkar; kolonlar ve eski veri silinir | Kullanıcı kararı |
| V-K2 | Vergiler kendi ekranında: ekle, düzenle, sil/duraklat, "Ödedim". Arka planda tekrarlayan plan (tür: Vergi/SGK); Tekrarlayanlar listesinde görünmez, **Yaklaşanlar'da görünür** | Kullanıcı kararı + öneri |
| V-K3 | Hazır türler sıklığı, günü ve tutar tipini getirir; kullanıcı kendi türünü de ekler | Kullanıcı kararı |
| V-K4 | **Sıklık: "seçilen aylarda"** eklenir (6 ayda bir yerine; emlak 5–11, MTV 1–7, gelir vergisi 3–7) | **Onay bekliyor** |
| V-K5 | Tanımda **hesap isteğe bağlı**; "Ödedim"de sorulur (bakiye oradan düşer), tanımda seçildiyse dolu gelir | Kullanıcı kararı; ödeme anında sorulması **onay bekliyor** |
| V-K6 | **Kim öder:** varsayılan "Ben". "Muhasebecim" yalnız seçilirse; muhasebeci akışları yalnız o zaman görünür | Kullanıcı kararı |
| V-K7 | Tutar: sabit vergide tanımda yazılabilir, değişkende boş; her durumda "Ödedim"de girilir/düzeltilir | Kullanıcı kararı |
| V-K8 | "Muhasebeciye ödedim": tek tutar, birden çok vergi kapanır, paylar isteğe bağlı; **vergi dışı kısım** (ör. muhasebe ücreti) ayrı satır olarak eklenebilir | Kullanıcı fikri + ek **onay bekliyor** |
| V-K9 | Birikmiş/gecikmiş vergi: birden çok bekleyeni tek ödemeyle kapatma; zam hesaplanmaz, ödenen tutarın içinde | Karar |
| V-K10 | Muhasebeciye bağlı vergi vadede "gecikti" değil "muhasebecine ödenecek" der | Karar |
| V-K11 | Cari hesaba bağlanmaz (muhasebeci vergi dışı masrafları cari ile — F18, F21) | Claude kararı (kullanıcı bıraktı) |
| V-K12 | Bütün vergiler işletme kapsamında, kapsam sorulmaz; işletme netini düşürür | Kullanıcı kararı |
| V-K13 | Net varlıkta tutarı bilinen bekleyen vergiler "Vergiler" başlığıyla | Kullanıcı kararı |
| V-K14 | Silme: hiç ödenmemiş vergi silinir; ödeme geçmişi varsa duraklatılır (tekrarlayan planın bugünkü kuralı) | Karar (bütünlük) |
| V-K15 | Tahakkuk fişi okuma yok (F17) | Kullanıcı kararı |
| V-K16 | Özet'e vergi raporu ileride (F22) | Kullanıcı kararı |

Hazır türler (V-K3):

| Tür | Sıklık | Gün | Tutar | Not |
|---|---|---|---|---|
| Bağkur | Her ay | Ay sonu | Sabit | Herkes |
| KDV | Her ay | 28 | Değişken | KDV mükellefi |
| Muhtasar ve prim hizmet | Her ay | 26 | Değişken | Çalışanı ya da stopajlı ödemesi olan |
| Geçici vergi | Seçilen aylarda: Şub, May, Ağu, Kas | 17 | Değişken | Gerçek usul |
| Yıllık gelir vergisi | Seçilen aylarda: Mar, Tem | Ay sonu | Değişken | Gerçek usul |
| Emlak ve çevre temizlik | Seçilen aylarda: May, Kas | Ay sonu | Sabit | Dükkânı olan; belediyeye |
| Motorlu taşıtlar | Seçilen aylarda: Oca, Tem | Ay sonu | Sabit | Aracı olan |
| İlan-reklam (tabela) | Yılda bir: Ocak | Ay sonu | Sabit | Tabelası olan; belediyeye |

## 7 · Açık sorular (ilk tur — 6.2'de cevaplandı)
1. Gelir vergisi, geçici vergi ve Bağkur ödemeleri **işletme netini düşürsün mü**, yoksa Özet'te ayrı satır mı?
2. **İndirilebilirlik** de KDV ile birlikte kalkıyor mu? (Önerim: evet, tek kullanıldığı yer paketti.)
3. Girilmiş **eski KDV verisi**: gizli kalsın mı, silinsin mi? (Önerim: gizli kalsın.)
4. **"Muhasebecim ödedi"** (§4) bu tura girsin mi, sade seçenek mi?
5. Gün sonu okuması için **örnek rapor görselleri** nereden gelecek?

## Kaynaklar
1. Bağkur ödeme günü — https://www.muhasebetr.com/vergi-takvimi/
2. KDV ve muhtasar — https://gib.gov.tr/vergi-takvimi
3. Geçici vergi ve yıllık gelir vergisi taksitleri — https://www.parasut.com/blog/gecici-vergi , https://vergimerkezi.com.tr/dorduncu-gecici-vergi-donemi-geri-geldi/
4. Emlak ve çevre temizlik vergisi — https://www.gaziosmanpasa.bel.tr/emlak-ve-is-yeri-cevre-temizlik-vergisi-2-taksit-odemeleri , https://musavirlerkulubu.com.tr/beyanname/cevre-temizlik-vergisi
5. Gecikme zammı — https://savun.av.tr/gecikme-zammi-hesaplama-2026-vergi-sgk/
6. Tahakkuk fişi — https://www.parasut.com/blog/tahakkuk-fisi-nedir
