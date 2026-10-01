# Brif 1 — Vergiler ekranı

`00-ortak-cerceve.md` ile birlikte okunur.

## Neden

Esnaf için vergi bir **nakit çıkışıdır** ve tutarı çoğu zaman ödeme gününe kadar belli değildir: "parayı
muhasebeciye ödeyince tutarı öğrenmiş oluyoruz". Bir kısım kullanıcı yaklaşan vergileri hiç izlemez; yalnız
cebinden çıkan vergiyi **tek seferde, tek tutarla** yazıp bütçesini doğru tutmak ister. Diğerleri vergilerini
tanımlayıp sıradakini, gecikenleri ve ödenenleri görmek ister.

Bu ekran bugünkü iki sayfanın yerini alır: `Vergi takvimi` (hazır kalemleri tekrarlayan plan olarak kuran
basit bir liste) ve `Muhasebeci paketi` (tamamen kalkıyor). Formlardaki KDV ve "indirilebilir" alanları da
kalkıyor; onlar için tasarım gerekmez.

## Değişmez kurallar

1. **Tanımlamadan ödenebilir.** "Vergi ödemesi ekle" hiçbir tanım gerektirmez: tutar + gün + hesap/kart.
   Hiç vergi tanımlamamış kullanıcıda ekranın **ana eylemi budur**; "vergi tanımla" onu gölgelemez.
2. **Tutarı bilinmeyen vergi meşrudur.** Tanımda tutar isteğe bağlıdır. Tutarı olmayan kalem toplamlara
   katılmaz; ekran "tutar belli değil" der, tahmin göstermez.
3. **Ödenene kadar hiçbir şeyi etkilemez.** Ödenmemiş vergi bakiyeyi, net varlığı, işletme netini ve bütçeyi
   etkilemez. Ödendiği gün, ödendiği hesaptan ya da karttan etkiler.
4. **Ödeme ödeme gününe yazılır**, vade gününe değil. Kartla ödeme kart harcaması olarak yazılır.
5. **Geri alınabilir.** Yanlış girilen ödeme geri alınır; kalem yeniden bekleyene döner.
6. **Vergi ayrı bir kayıt türü değildir.** Ödenen vergi normal bir giderdir ve İşlemler'de görünür; tanımlı
   vergi Yaklaşanlar'da görünür. "Ödenenler" listesi, **vergi işaretli kategorilerdeki** giderlerdir: Gider
   formundan bu kategoriyle girilen ödeme de burada görünür (tek gerçek, iki kapı).
7. Metinler "genelde" diliyle yazılır ve kullanıcıyı muhasebecisine yönlendirir; uygulama vergi uzmanı gibi
   konuşmaz.

## Ekranın yapısı (öneri; daha iyisini gösterebilirsin)

Diğer › **Vergiler** ile açılır (tam ekran, geri oklu, alt çubuk yok).

1. **Başlık + ana eylem:** "Vergi ödemesi ekle" her hâlde görünür ve birincil eylemdir.
2. **Bekleyenler:** gecikenler ve bu ay/önümüzdeki 30 gün. Satır: vergi adı + dönem (`Bağkur · Eylül`),
   vade (`30 Eyl · yarın`, `28 Eyl · 1 gün gecikti`), sağda tutar ya da *"Tutar ödemede girilecek"*, durum
   etiketi (`Gecikti` kırmızı, `Yaklaşıyor` gri). Satır eylemi **Ödedim**.
3. **Vergilerim:** tanımlı türler. Satır: ad, ritim (`Her ay · ay sonu`, `Mayıs ve Kasım · ay sonu`,
   `Şub · May · Ağu · Kas · 17'si`), sıradaki tarih, gerekiyorsa `Şahsi` ya da `Duraklatıldı` etiketi.
   Bölüm başlığında `+ Ekle`.
4. **Ödenenler:** son ödemeler. Satır: gün, ad (kategori ya da not), kaynak hesap/kart, tutar (gider
   kırmızı). Toplu ödemede altında `Kapattı: Bağkur Ağustos, Bağkur Eylül`. `Tümü ›`.

## Çizilecek çerçeveler

### V1 · Vergiler — ilk kullanım (hiç tanım yok)
Tek büyük eylem "Vergi ödemesi ekle"; altında bir cümle: *"Ödediğiniz vergiyi tek tutarla yazın. İsterseniz
vergilerinizi tanımlayıp ne zaman ödeneceğini takip edin."* İkincil eylem "Vergilerimi tanımla".
Ödenenler boş olmayabilir (Gider formundan girilmiş vergi) — bu hâli de göster.

### V2 · Vergiler — dolu
Örnek veri aşağıda. Bekleyenler, Vergilerim, Ödenenler.

### V3 · "Ödedim" paneli
Başlık `Bağkur · Eylül` + vade. Alanlar:
- **Tutar** (zorunlu; tanımda ya da "tutar belli oldu" ile yazılmışsa dolu gelir, değiştirilebilir)
- **Ödeme günü** (varsayılan bugün)
- **Nereden ödendi** — hesap ya da kredi kartı (zorunlu; tanımda seçilmişse dolu gelir)
- Kural metni: *"Gider ödeme gününe yazılır."*; kart seçilince *"Kart harcaması olarak yazılır; kart
  borcunuza eklenir."*
Gecikmiş kalemde ödenen tutar gecikme zammını da içerir; uygulama zammı hesaplamaz. Bunu söylemek gerekip
gerekmediğini sen öner (tek satır, gri).

### V4 · "Vergi ödemesi ekle" (toplu) — tanım yokken
Tutar, gün, hesap/kart (zorunlu), not (isteğe bağlı; ipucu *"ör. Temmuz–Ağustos Bağkur"*). Kategori
kendiliğinden vergi kategorisidir; göstermek gerekiyor mu, sen öner.

### V5 · "Vergi ödemesi ekle" — bekleyenler varken
Aynı panel + **"Bu ödeme hangilerini kapatıyor?"** listesi: tanımlı bekleyen kalemler, onay kutusuyla;
**vadesi gelmiş ve geçmiş olanlar seçili gelir**. Tutar kalemlere dağıtılmaz ve eşleştirilmez (çoğunun
tutarı yok); bunu bir cümleyle söyle. Hiçbiri seçilmeden de kaydedilebilir. Seçilen kalemler "kapatıldı"
olur ve bekleyenlerden düşer.

### V6 · Vergi ekle — tür seçimi
Hazır türler kategori seçer gibi bir listede; her birinin altında tek satır ipucu. En altta
**"Kendi türüm"**. Hazır tür yalnız **ritim ve günü** önerir, tutar önermez.

| Tür | Önerilen ritim | İpucu (öneri) |
|---|---|---|
| Bağkur | Her ay · ay sonu | Esnafın çoğu öder |
| KDV | Her ay · 28'i | Basit usuldeyseniz genelde yok |
| Muhtasar ve prim hizmet | Her ay · 26'sı | Çalışanınız varsa |
| Geçici vergi | Şubat, Mayıs, Ağustos, Kasım · 17'si | Gerçek usuldeyseniz |
| Yıllık gelir vergisi | Mart, Temmuz · ay sonu | Gerçek usuldeyseniz |
| Emlak ve çevre temizlik | Mayıs, Kasım · ay sonu | Dükkânınız varsa; belediyeye |
| Motorlu taşıtlar | Ocak, Temmuz · ay sonu | Aracınız varsa |
| İlan-reklam (tabela) | Yılda bir · Ocak sonu | Tabelanız varsa; belediyeye |

### V7 · Vergi tanımı formu (hazır türden önden dolu)
- Ad (türden gelir, değiştirilebilir)
- **Ritim:** `Her ay · Üç ayda bir · Yılda bir · Seçilen aylarda`. "Seçilen aylarda" yeni: 12 ay çipi
  (çoklu seçim) + gün. Gün: sayı ya da "ay sonu".
- **Tutar (isteğe bağlı):** *"Her dönem değişiyorsa boş bırakın; ödediğinizde yazarsınız."*
- **Nereden ödenir (isteğe bağlı):** hesap ya da kart; *"Ödediğinizde de seçebilirsiniz."*
- **Kapsam sorulmaz.** İşletmesi olan kullanıcıda vergiler işletme, olmayanda şahsidir. Yalnız Motorlu
  taşıtlar ve Emlak türünde "Şahsi (aracım / evim)" seçeneği görünür.
- Başlangıç tarihi (varsayılan: sıradaki vade).

### V8 · Bekleyen kalem ayrıntısı
`AppDetailBlock`: vade, tutar ya da "belli değil", nereden ödenecek. Eylemler: **Ödedim** (birincil),
**Tutar belli oldu** (kaleme tutar yazar; plan değişmez), kalemi tanımın ayrıntısına götüren bağlantı.

### V9 · Ödenmiş kalem / ödeme ayrıntısı + geri alma
Ödenmiş tek kalem: ödeme günü, tutar, hesap/kart, `Ödendi` durumu, **"Ödemeyi geri al"** → onay:
*"Gider iptal edilir; kalem yeniden bekleyene döner."*
Toplu ödeme: kapattığı kalemlerin listesi, **"Ödemeyi geri al"** → *"Gider iptal edilir; kapattığı 2 kalem
yeniden bekler."* Kapatılmış bir kalemin kendi ayrıntısında `Kapatıldı · 30 Eyl toplu ödemeyle` yazar ve
geri alma o ödemeden yapılır.

### V10 · Vergi tanımı ayrıntısı
Sıradaki kalemler, geçmiş ödemeler, eylemler: Düzenle · Duraklat/Sürdür · Sil. Hiç ödenmemiş vergi
silinir; ödeme geçmişi varsa silme yerine duraklatmaya yönlendir (*"Ödenmiş kalemleri geçmişte kalsın diye
bu vergi silinemez; duraklatabilirsiniz."*).

## Başka ekranlara dokunuşlar (küçük; aynı teslimde göster)

- **Diğer:** `Vergi ve muhasebe` grubu tek satıra iner: **Vergiler**. `Muhasebeci paketi` kalkar.
- **Yaklaşanlar** (Özet'teki 7 gün kartı ve Planlama): tutarı olmayan vergi kalemi
  *"Tutar ödemede girilecek"* der; alttaki toplam `7 günde çıkacak ₺12.400,00 · 2 kalemin tutarı belli
  değil` gibi okunur.
- **Kategori formu:** "Vergi" anahtarı; alt metin *"Bu kategorideki giderler Vergiler › Ödenenler'de
  görünür."*
- **Tekrarlayan plan formu:** aynı "Seçilen aylarda" ritmi burada da seçilebilir (V7'deki ay çipleri).
- **Özet:** işletme neti artık işletme vergilerini (gelir vergisi, Bağkur dahil) düşülmüş gösteriyor. Hero'nun
  altına bunu söyleyen kısa bir açıklama gerekip gerekmediğini öner.

## Senden karar önerisi beklediğim yerler

1. **Ana eylemin yeri:** başlıktaki ikon mu, gövdenin altında tam genişlik düğme mi, Bekleyenler kartının
   üstünde mi? (Kural: her hâlde görünür, tanım eylemi onu gölgelemez.)
2. **Bekleyenler kaç gün ileriyi gösterir:** yalnız gecikenler + bu ay mı, 30 gün mü?
3. **"Ödedim" satırda düğme mi, satıra dokununca açılan ayrıntıda mı?** (Liste satırında dolgulu düğme
   kullanmıyoruz; satır eylemi `AppRowAction` biçiminde.)
4. **Tür seçimi:** hazır türler tek tek mi eklenir, yoksa ilk kurulumda çoklu seçimle ("Bağkur + KDV +
   Geçici vergi") tek seferde mi?

## Örnek veri (sentetik; bugün 29 Eylül 2026, işletme profili)

- Bekleyenler: **KDV · Ağustos** — 28 Eyl, 1 gün gecikti, tutar belli değil · **Bağkur · Eylül** — 30 Eyl,
  yarın, ₺8.950,00 (tanımda sabit tutar) · **Geçici vergi · 3. dönem** — 17 Kas, tutar belli değil.
- Vergilerim: Bağkur (Her ay · ay sonu, ₺8.950,00, Dükkan hesabı) · KDV (Her ay · 28'i) · Geçici vergi
  (Şub · May · Ağu · Kas · 17'si) · Motorlu taşıtlar (Ocak, Temmuz · ay sonu, **Şahsi**) · Tabela
  (Duraklatıldı).
- Ödenenler: 26 Eyl Muhtasar ₺3.240,00 · Dükkan hesabı · 15 Eyl "Vergi ödemesi" ₺12.500,00 · Nakit kasa ·
  *Kapattı: Bağkur Temmuz, Bağkur Ağustos* · 31 Tem MTV 1. taksit ₺2.180,00 · Bonus kart.
