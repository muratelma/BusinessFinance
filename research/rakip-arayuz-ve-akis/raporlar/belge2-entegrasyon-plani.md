# Belge 2 — ek koşum bulgularının entegrasyon planı

**Durum:** uygulandı (23 Eylül 2026). Kararlar ve sonuç §10'da.

**Girdi:** [eksik-kosum-ortak-listesi.md](eksik-kosum-ortak-listesi.md) — 22 Eylül
emülatör koşumu (112 kare) + 23 Eylül kullanıcı gözlemleri (BC-15, BC-X2, GB-04).
**Hedef:** `belge2/tam/belge2.pdf` (105 sayfa · 127 kare). Sonuç belgesi bozulmadan,
aynı kalıp ve kapılarla yeniden üretilir.

---

## 1 · Yöntem: kareden değil, sayfadan başla

Kareler tek tek gezilip "nereye koyarız" diye sorulmadı. Tersi yapıldı: Belge 2'nin
on bölümü baştan okundu, her kapanan eksik etkilediği **sayfa ve cümleye** bağlandı,
kare yalnız bir sayfa onu gerçekten istiyorsa aday yapıldı. Böylece:

- 112 karenin çoğu envantere hiç girmez; yalnız basılacak olanlar girer.
- Belgenin iskeleti (bölüm sırası, sayfa türleri, akış sayfaları) değişmez.
- Değişikliklerin önemli bir kısmı **mevcut yanlışı düzeltmek**; yeni içerik
  eklemek ikinci sırada.

Her değişiklik dört türden biridir:

| Tür | Anlamı | Örnek |
|---|---|---|
| **D** · düzeltme | Belgede basılı bir ifade yeni kanıtla yanlış çıkıyor | "Bütçe kurulmadığı için %0" — bütçe kuruluydu |
| **G** · güçlendirme | "Görülmedi / denenmedi" diyen bir hücre artık kanıtlı | Wallet aktarım formu artık kareli |
| **E** · ekleme | Belgede hiç olmayan, sayfanın sorusuna doğrudan cevap veren yeni bulgu | Wallet Labels görünümü yalnız etiketli parayı sayıyor |
| **—** · dokunma | Bulgu doğru ama bu sayfanın sorusu değil ya da Belge 1'e ait | Wallet Shopping lists |

## 2 · Doğrulama: koşum sonuçlarının hepsi olduğu gibi alınamaz

Belgeye taşımadan önce çelişen ve güçlü iddiaların kareleri tek tek açıldı.
Sonuçlar:

| ID | Koşumun iddiası | Kareye bakınca | Sonuç |
|---|---|---|---|
| **WL-11** | Sıfır tutar "sessizce reddediliyor, hata mesajı yok" | `f7-67` yalnız 0 TRY'lik formu gösteriyor; uyarı yok. Belgedeki E0281 aynı durumda **"Please fill in the amount."** baloncuğunu gösteriyor. Baloncuk birkaç saniye görünüp kayboluyor. | **Çürük.** Kare mesajın yokluğunu kanıtlamaz. Belge 2'nin 1.5'i doğru kalır; ortak listede durum "kapanmadı" olmalı |
| **BC-12** | İkinci bloğun adı "Net Kazanç" | `f7-79` doğruluyor: Varlıklar · Cari hesap · **Net Kazanç**. Dosya adındaki "net-deger" yanlış | Doğru. Dosya adı yanıltıcı; kare basılırsa şekil adı "Net Kazanç" olmalı. Kare yarı saydam bir üst şerit taşıyor, basım için uygun değil |
| **MM-05** | Bütçe kurulu, %0'ın nedeni bütçeli kategoride harcama olmaması | `70`: Ekim, Yiyecek 1.400 bütçe, harcanan 0, Gider 1.000 kart taksiti | Doğru. **Belge 2'nin 5.5 ve 5.7 cümlesi yanlış** (bkz. §3 Bölüm 5) |
| **BC-08 / BC-04** | Bölünmüş kaydın her satırı kendi etiketini taşıyor; hazır etiketler İş · Kişisel | `f7-58`: her satırda Durum ve Etiket var. `f7-67`: listede Doğum günü · Film · İş · Kişisel · Tatil | Doğru, **bir çekinceyle**: İş ve Kişisel'in ürünle hazır mı geldiği, daha önce mi eklendiği karede görünmüyor. Belgeye "listede var" diye girer, "hazır" diye girmez |
| **WL-07** | Labels görünümü yalnız etiketli ₺150'yi sayıyor | `f7-80`: This month ₺150,00, Isletme ₺100 · Sahsi | Doğru ve güçlü |
| **WL-05** | Kart harcaması dönem giderine giriyor; aradaki 6.000 "kart ve nakit" | `f7-76` tüm hesaplarda 21.600; Ana Hesap filtresinin karesi ayrıca açılmalı | **Açılmadı.** "Kart ve nakit" ifadesi 6.000'i karta tek başına bağlamıyor. Üretimden önce `f7-81`/`f7-82` açılıp teyit edilir; olmazsa hücre "görülmedi"de kalır |
| **GB-02** | Harcama raporundaki gelirin nedeni = Initial Envelope Fill | Açıklama **Income 3.284**'ü açıklıyor (E0423 ile zaten belgede). 7.4'ün sorusu başka: harcama raporunun toplamında 25.000'lik gelirin olması (E0123, −22.950) | **7.4'ün sorusunu cevaplamıyor.** 7.4 değişmez; eksik açık kalır |
| **BC-17** | Seyahat modu "seçilen etiketle çalışan bir süzgeç" | Kare yalnız anahtara basınca etiket seçicinin açıldığını gösteriyor; koşumda iptal edilmiş | **Abartılı.** Belgeye yalnız "açınca etiket seçici çıkıyor" girer; süzgeç olduğu çıkarımı yazılmaz |
| **WL-08** | Takvim ayına çevrilince Eylül: Income 5.000 · Expenses −21.600 | Kayıtların hepsi son 30 günde; 12W ile takvim ayı aynı sayıyı veriyor | 7.2'nin çıkarımını ("varsayılan dönem sonucu değiştiriyor") **sayıyla göstermez**. Yalnız dönem seçicinin yapısı (özel aralık) girer |
| **BC-02** | Kısmi ödeme sonrası dönem raporu | `f7-79` Eylül'ün bugünkü hâli; kısmi ödemenin önce/sonrası değil | Belgeye katkısı yok; 3.6 değişmez |
| **BC-X2** | (kullanıcı) Taksit ve tekrarlar kendiliğinden gerçekleşmiyor | Karesiz; `kosum` | Kapanır. Ama Bölüm 5'in ayrı eksiği "**otomatik kolun** çalışması" bununla kapanmaz — planların otomatik kolu açık muydu bilinmiyor |

**Ortak listede atlanan bir eksik daha bulundu:** Bölüm 5 eksik listesinde
"Bluecoins — Otomatik kolun çalışması ve hangi tarih kolunun seçildiği (5.2)"
satırı ortak listeye hiç girmemiş. BC-X2'nin kullanıcı gözlemi bunun **tarih**
yarısına ışık tutuyor (ileri tarihli taksitte "bugün / taksit tarihi" soruluyor),
otomatik kol yarısını kapatmıyor.

## 3 · Bölüm bölüm değişiklikler

Sütunlar: **Yer** (sayfa · öğe) · **Şimdi** (basılı ifade, kısaltılmış) · **Yeni** ·
**Tür** · **Kaynak** · **Kare**. Kare sütununda "—" metin değişikliğidir; kare adayı
§4'te toplanır.

### Bölüm 1 · Gelir ve gider

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 1.1 madde 4 | Bölmek "koşumda işletilmedi" | 50 + 60 = 110 iki satıra bölündü; ay gideri 11.015 → 11.125; listede **tek satır, "2 Kategoriler"**; her satır kendi kategori, hesap, durum ve etiketini taşıyor | G | BC-08 | — |
| 1.2 tablo · geri alma satırı | MM, Wallet: "Görülmedi · kurtarma katmanı görülmedi" | "Yok — silme onaylı; menü ve Ayarlar tarandı, çöp kutusu bulunamadı" | G | MM-11, WL-17 | — |
| 1.2 tablo · geri alma · Goodbudget | "Tek onayla siliniyor; geri alma görülmedi" | "Tek onayla siliniyor; Ayarlar'ın dört bölümü tarandı, geri alma yok" | G | GB-07 | — |
| 1.2 tablo · geri alma · Bluecoins | "Çöp kutusu var" | "Çöp kutusu çalışıyor; geri yüklenen kayıt listede ancak uygulama yeniden açılınca görünüyor" | G | BC-15 (`kosum`) | — |
| 1.2 alt not (yeni madde) | — | Tutar düzenleme: MM'de bir kaydın tutarı 0 → 580 yapıldı, ay gideri anında 1.600 → 2.180; ara adım yok. **Yalnız bir üründe ölçüldü**, diğer dördü eksik listesinde kalır | E | MM-10 | — |
| 1.5 madde 1 | "Geri alma yalnız iki üründe gözlendi … Wallet, MM ve Goodbudget'ın silme akışında ikinci bir kurtarma katmanı görülmedi" | Aynı cümle, dayanağı genişler: üçünde de silme bir onay diyaloğu, menü ve Ayarlar tarandı, kurtarma yüzeyi yok | G | MM-11, WL-17, GB-07 | — |
| 1.5 · MM sıfır tutar | Sayfa MM'yi "hatalı kaydı durduran" tarafa koyuyor (Şekil 1.14 hesap boşken) | **MM sıfır tutarı kabul ediyor**: boş tutarla kaydedilen kayıt ₺0,00 olarak listeye düşüyor, uyarı yok. Boş hesap ise durduruluyor. MM, Hesap Defterim'le aynı gruba geçer | **D** | MM-07 | aday `50` |
| 1.7 adım 4 | "*Money Manager · Wallet* — Form hatalı kaydı durduruyor" | Wallet tek başına kalır; MM "boş hesabı durduruyor, sıfır tutarı kabul ediyor" | **D** | MM-07 | — |
| 1.8 kazanç tablosu | Değişmez | — | — | — | — |

### Bölüm 2 · Hesaplar ve aktarım

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 2.1 madde 2 | Wallet: "açılış alanı arandı, bulunamadı. Bu bir koşum kaydıdır — formun karesi alınmadı" | Hesap **düzenleme** formu kareli; alanlar: ad, hesap no, para birimi, renk, içe aktarma e-postası, istatistikten çıkar, arşiv, min/max bakiye — açılış bakiyesi yok. **Oluşturma formu değil**: dördüncü hesap ücretsiz pakette açılamıyor | G | WL-02 | aday `f7-65` (f7-64 kişisel veri taşıyor) |
| 2.2 giriş | "biri aktarım ücreti soruyor" | **İkisi** ücret soruyor: MM'nin havale formunda Harç alanı, Bluecoins'te transfer ücreti | **D** | MM-09 | — |
| 2.2 Şekil 2.6 madde 3 | "Ücretin nasıl işlendiği görülmedi" | Ücret ayrı bir blok: kendi tutarı, hesabı ve kategorisi (varsayılan Diğer). Aktarımın parçası değil, ona bağlı ayrı bir gider satırı | G | BC-10 | aday `f7-75` |
| 2.2 madde 3 | "Wallet'ın aktarım formunun karesi alınamadı" | Form kareli: Income/Expense/**Transfer** aynı formun üçüncü sekmesi; From · To · Target amount · tuş takımı | G | WL-03 | aday `f7-66` |
| 2.2 giriş sayımı | "ikisinde işlem formunun üçüncü türü, üçünde kendi ekranı" | Wallet artık kanıtlı olarak formun üçüncü sekmesi → **üçünde form türü, ikisinde ayrı ekran**. 2.3 tablosunun "Aktarım ayrı bir tür mü" satırındaki Wallet hücresi ("ekleme menüsünde ayrı giriş") ile birlikte yeniden yazılır; ikisi de doğru olabilir (menüden girilip forma iniyor), üretimde tek cümleye bağlanır | **D** | WL-03 | — |
| 2.3 tablo · "Form ne soruyor" · Wallet | "Görülmedi" | "From · To · Target amount" | G | WL-03 | — |
| 2.3 tablo · "Form ne soruyor" · MM | "Kaynak · Giriş · tutar · not" | + Harç | **D** | MM-09 | — |
| 2.3 tablo · "Ayın gelir/gider toplamı" | Üç ürün "Etkilenmedi" | Değişmez; önce/sonra kareleri (MM `58`, Wallet `f7-68`, BC `f7-80`) dayanağı güçlendirir, basılmaz | G | MM-09, WL-04, BC-05 | — |
| 2.5 madde 2 ve 4 | Aynı Nakit Akım Ayarı cümlesi **iki kez** yazılmış (mevcut kusur) | Tek madde: ekranın kendi cümlesi "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz"; hesap başına onay kutusu. Açıp kapatıp raporun değiştiği **ölçülmedi** | G + temizlik | BC-09 | — |
| 2.7 kazanç · Bluecoins | "Aktarım ücretinin nasıl işlendiği yüzeyden anlaşılmıyor" | Kaybettirdiği yeniden yazılır: "Ücret aktarımdan ayrı bir gider kaydı; aktarım listede yine iki bacak" veya başka bir gözlenen bedel. **Açık karar K7** | **D** | BC-10 | — |
| 2.7 kazanç · Wallet | "Hesap açarken açılış bakiyesi alanı bulunamadı" | "Hesap formunda açılış bakiyesi alanı yok" | G | WL-02 | — |

### Bölüm 3 · Kart

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 3.1 madde 4 | Wallet: "limit 0 bırakıldı; Available Credit gösteriminin limitle birlikte ne yazdığı ölçülmedi" | Credit sekmesi limit 0 iken kullanımı **%2147483647** gösteriyor — sıfıra bölmenin görünür taşması. Payment Due Date ayın günü seçicisi (1–31) | G + E | WL-05, WL-18 | aday `f7-82` |
| 3.3 Şekil 3.10 madde 3 | "İleride kendiliğinden oluşup oluşmayacağı görülmedi" | Kalan taksitler Hatırlatıcılar'da bekliyor ve **kendiliğinden kayda dönüşmüyor**; her biri Kaydet ile elle gerçekleşiyor; ileri tarihli taksitte "bugün / taksit tarihi" soruluyor | G | BC-X2 (`kosum`) | — |
| 3.5 tablo · Wallet · "Kart harcaması gider sayılıyor mu" | "Görülmedi" | "Evet" — **yalnız §2'deki WL-05 teyidi geçerse** | G | WL-05 | — |
| 3.5 tablo · Wallet · "Kısmi ödeme neyi düşürüyor" | "Görülmedi" | "Tek parça bakiye: −5.600 → −5.300; ödeme hesaptan karta aktarım" | G | WL-16 | — |
| 3.5 tablo · Wallet · "Ödeme gider sayılıyor mu" | "Görülmedi" | "Hayır — dönem gideri 21.600'de kaldı" | G | WL-16, WL-04 | — |
| 3.6 madde 4 | "Wallet'ta kısmi ödeme bu koşumda denenmedi" | Denendi: 300 kısmi ödeme kartı −5.600'den −5.300'e çekti, gider toplamına dokunmadı. Dönem sorusu üründe oluşmuyor (cümlenin ikinci yarısı kalır) | G | WL-16 | — |
| 3.7 | Aradaki 4.000 = Ekim–Ocak taksitleri (aritmetik) | Kasım 3.600 ve Aralık 4.600 kareyle görüldü; zincir 2.600 → 3.600 → 4.600 → 5.600 kesintisiz | G | MM-03 | — (bkz. K5) |
| 3.8 adım 4 | "*Bluecoins · Hesap Defterim* — tek parça" · "*Ölçülen üç üründe de* — ödeme gider sayılmıyor" | Wallet tek parça grubuna girer; "ölçülen **dört** üründe" | G | WL-16 | — |
| 3.9 son çıkarım | Başlık "Ödeme **dört** üründe de gider değil", gövde "ölçülebilen **üç** üründe" (mevcut tutarsızlık) | Gövde dört ürüne çıkar; tutarsızlık kendiliğinden kapanır. Dayanağa `f7-68` eklenir (basılmasa da kimliği gerekir → envanter) | **D** | WL-16 | — |
| 3.9 kazanç · Bluecoins | "kalan beşinin kendiliğinden gerçekleşip gerçekleşmediği yüzeyden anlaşılmıyor" | "Kalan taksitler kendiliğinden gerçekleşmiyor; her biri elle kaydediliyor — unutulan taksit gidere hiç girmiyor" (son yarı çıkarım) | **D** | BC-X2 | — |
| 3.1 kart dönemi (Bluecoins) | "dönem davranışı görülmedi" | **Değişmez** — BC-X1 açık kaldı | — | — | — |

### Bölüm 4 · Borç ve cari

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 4.2 madde 2 | "Bağlama kolu bu koşumda denenmedi" | Denendi: Select Record bütün mevcut kayıtları etiketleriyle listeliyor | G | WL-13 | aday `f7-86` |
| 4.3 Şekil 4.9 madde 2 | "Durum ve fatura bağlantısı açılmadı" | Durum dört değer: Yok · Kontrol · Mutabık · İptal edildi. Fatura bağlama alanı yok; ek yalnız ataç simgesiyle | G | BC-03 | — |
| 4.3b Şekil 4.13 | "Borcu artırma da bir seçenek" | Debt action tam iki değer: Repay debt · Increase debt | G | WL-12 | — |
| 4.4 tablo · Bluecoins · vade | "Görülmedi · Cari hesapta vade alanı görülmedi" | "Yok — cari hesap formu tarandı: ad, not, başlangıç bakiyesi, son bakiye, açılış tarihi, hesap tipi, iki anahtar" | G | BC-11 | — |
| 4.6 madde 2 | "Bluecoins'in cari hesabında vade alanı görülmedi" | "…vade alanı yok" | G | BC-11 | — |
| 4.x (yeni madde) | Closed sekmesi hiç anılmıyor | "Closed sekmesi boş: No closed debts" — borcun kapanıp oraya düşmesi (WL-X3) eksik listesinde kalır | E | WL-06 | — |

### Bölüm 5 · Zaman — en çok düzeltme burada

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 5.2 giriş | "Bir üründe kapı hiç yok" (hangisi olduğu belirsiz) | Üç model netleşiyor: **MM** — kapı yok, vadesi gelen tekrar onaysız kayda dönüşüyor, zamanlama bir ayar; **Wallet** — kapı kullanıcının; **Bluecoins** — her gerçekleşme elle | **D** | MM-04, BC-X2 | — |
| 5.2 madde 2 | MM: "onaydan mı kendiliğinden mi oluştuğu tek kareden okunamıyor" | Kendiliğinden: `Tekrar ne zaman uygulanır?` iki değer (Tarihte · Her ayın ilk günü), kayıt başına onay yok. Kurulumda tek bir onay: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" | **D** | MM-04 | aday `66`, `69` |
| 5.3 madde 3 | "MM'nin ayarlarında 'Tekrar ne zaman uygulanır?' ayarı var; seçili değer Tarihte" | İki değeri de yazılır | G | MM-04 | — |
| 5.3 · Wallet | "Yalnız sıradaki vade görünüyor" | + sıralama (vade yeni/eski, ad A→Z/Z→A) ve tür filtresi; ileri vadeler yine toplu görünmüyor | G | WL-21 | — |
| 5.3 · Bluecoins | Hatırlatıcı listesi üç türü topluyor | + listedeki tekrar ve taksit kendiliğinden gerçekleşmiyor (bkz. 3.3) | G | BC-X2 | — |
| 5.4 tablo · "Gerçekleşme kapısı" · MM | "Bugünkü örnek listede gerçek kayıt; onay izi görülmedi" | "Kapı yok: vadesi gelince onaysız kayda dönüşüyor; zamanlama ayardan" | **D** | MM-04 | — |
| 5.4 tablo · "Gerçekleşme kapısı" · Bluecoins | "Kayda çevirirken tarih soruluyor" | "Her gerçekleşme elle; kayda çevirirken tarih soruluyor" | G | BC-X2 | — |
| 5.4 tablo · "Bütçe var mı" · MM | "Evet — Toplam sekmesinde blok; koşumda %0" | "Evet — kategori başına; kurulu tek bütçe Yiyecek 1.400" | G | MM-05 | — |
| 5.4 tablo · "Bütçe var mı" · Bluecoins | "Bütçe Özeti ana ekranda; kurulumu koşulmadı" | "Bütçe Özeti var; hiçbir kategoriye bütçe kurulmamış, bu yüzden tamamı Others" | G | BC-18 | — |
| 5.5 Şekil 5.16 madde 3 + madde 3 | "MM'nin bütçe bloğu **koşumda kurulmadığı için** ilerleme çubuğu boş kaldı" | **Yanlış.** Bütçe kurulu (Yiyecek 1.400, şeklin kendi ikinci maddesi de bunu söylüyor). %0'ın nedeni o ay Yiyecek'te harcama olmaması. Bütçe kategori başına, varsayılan + ay ay değer, değişiklik **önümüzdeki aydan** geçerli | **D** | MM-05 | aday `71` veya `72` |
| 5.5 madde 4 | "Bluecoins'in … Bütçe Özeti bölümü var; kurulumu koşulmadı" | Kurulum okundu: İşlem tipi Gider · Bu Ay · Kategoriye göre; bütçe kurulmamış | G | BC-18 | aday `f7-56` |
| 5.7 adım 3 · MM | "onay izi kareye alınmadı" | "Onay yok: vade gelince kendiliğinden" | **D** | MM-04 | — |
| 5.7 adım 3 · Bluecoins | Tarih sorusu | + her gerçekleşme elle | G | BC-X2 | — |
| 5.7 adım 4 · MM | "koşumda kurulmadığı için ilerleme boş kaldı" | Düzeltilir (5.5 ile aynı) | **D** | MM-05 | — |
| 5.8 çıkarım "Kapının kime ait olduğu" | 3. paragraf MM'yi belirsiz bırakıyor | Üç uç: MM kapıyı kaldırmış (zamanlamayı ayara koymuş), Wallet kapıyı kullanıcıya vermiş, Bluecoins her gerçekleşmeyi kullanıcıya yıkmış. Dayanağa `66`/`69` kimlikleri | **D** | MM-04, BC-X2 | — |
| 5.8 kazanç · MM | "Gerçekleşmenin onaylı mı kendiliğinden mi olduğu yüzeyden okunmuyor; bütçe bloğu Toplam sekmesinin içinde saklı" | İlk yarı düşer (artık okunuyor: kurulum diyaloğu söylüyor). Yerine gözlenen bir bedel: bütçe değişikliğinin ancak önümüzdeki ay geçerli olması, veya %0'ın nedeninin ekranda yazmaması. **K7** | **D** | MM-04, MM-05 | — |
| 5.8 kazanç · Bluecoins | "Otomatik kolun çalışması görülmedi; …" | "Tekrar ve taksit kendiliğinden gerçekleşmiyor; her vade bir dokunuş istiyor" + tek liste cümlesi kalır. Otomatik kol cümlesi **eksik listesine** taşınır | **D** | BC-X2 | — |
| Goodbudget sıklık / gerçekleşme | "Ölçülmedi" | **Değişmez** — GB-06 açık | — | — | — |

### Bölüm 6 · Sınıflandırma

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 6.2 Wallet · Labels | "Etiket kategoriden bağımsız bir katman" | + Labels görünümü **yalnız etiketli parayı** sayıyor: ayın 21.750'sinin 150'si görünüyor, etiketsiz kayıt bu görünümde hiç yok. Etiket formunda "Auto assign to new records" anahtarı var | E | WL-07 | aday `f7-80` (E0278'in yerine; Categories/Labels geçişini o da gösteriyor) |
| 6.2 Bluecoins · etiket | Şekil 6.6 "Değerlerin nasıl üretildiği görülmedi" | Etiket listesinde **İş** ve **Kişisel** var (kimin eklediği görülmedi); çoklu seçim; filtre panelinde Etiketler ayrı bir boyut | G + E | BC-04 | aday `f7-67` (E0088'in yerine) |
| 6.2 madde 3 | Wallet Automatic rules "denenmedi" | Değişmez; Filters formu (Labels, Transfers/Debts include) bir madde olarak eklenebilir | E | WL-15 | — |
| 6.5 tablo · Bluecoins · en yakın araç | "Etiket alanı ve iki katmanlı kategori ağacı" | + etiket listesinde İş/Kişisel | G | BC-04 | — |
| 6.5 tablo · Wallet · sonuç | "Rapor Categories/Labels olarak ikiye ayrılabiliyor" | + "ama Labels yalnız etiketli parayı sayıyor" | E | WL-07 | — |
| 6.6 adım 3 | "Kaydı bölmek mümkün … **ama parçalara kapsam verilemiyor; bölme kategori içindir**" | **Bluecoins'te yanlış:** her parça kendi etiketini taşıyor. "Bluecoins'te parçaya etiket verilebiliyor; etiketle raporun parça düzeyinde ayrılıp ayrılmadığı ölçülmedi" | **D** | BC-08 | — |
| 6.7 çıkarım "Yerine kullanılan araçlar…" | "Etiket … sonucun doğruluğu tamamen kullanıcının disiplinine kalıyor" | Soyut ifade kanıtla somutlaşıyor: etiketlenmemiş kayıt Labels raporundan tamamen düşüyor (150 / 21.750). Dayanağa `f7-80` | G | WL-07 | — |
| 6.7 Belge 3 sorusu 2 | "Split tek örnek ve kapsamı kalem düzeyine indiriyor" | Bluecoins'in parça etiketi ikinci, ölçülmemiş bir örnek olarak anılır | **D** | BC-08 | — |

### Bölüm 7 · Rapor

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| 7.2 Wallet | 12W varsayılan; 6M/1Y ücretli | + dönem seçici üç sayfa: göreli çipler, adlandırılmış dönem, özel tarih aralığı | G | WL-08 | — |
| 7.2 Hesap Defterim | Çipler var | + Haftalık ve Yıllık "Önceki denge"yi taşıyor; dönem yalnız kendi aralığının hareketlerini topluyor | G | HD-05 | — |
| 7.3 MM filtre | Filtre paneli sütun adları | + hesap filtresi toplamı anında yeniden hesaplıyor (2.180 → 1.000, %45) ve havale ayrı satırda kalıyor | G | MM-06 | — |
| 7.3 Bluecoins iki blok | "Benzer iki etiket" | Adlar kesin: **Net Kazançlar** (gelir-gider) · **Net Kazanç** (varlık) — tek harf farkı | G | BC-12 | — |
| 7.4 Goodbudget | Harcama raporunda gelir | **Değişmez** — GB-02 bu soruyu cevaplamıyor (§2) | — | — | — |

### Bölüm 8 · Veri — iki çıkarım yeniden kurulur

| Yer | Şimdi | Yeni | Tür | Kaynak | Kare |
|---|---|---|---|---|---|
| Kapsam tablosu · Wallet | "Görülmedi · Dışa aktarma yüzeyi bu koşumda aranmadı" | "Canlı kare · Dışa aktarma çekmecenin katlanmış Others bölümünde" | G | WL-09 | — |
| 8.1 Şekil 8.2 madde 2 | "Ürün dosyanın yerini kullanıcıya söylüyor" | **Yanlış.** Ürün bir klasör adı söylüyor ama **o klasör oluşmuyor**; dosyalar başka iki yere yazılıyor | **D** | HD-02 (`kosum`; dosya sistemi araması karesiz) | — |
| 8.1 madde 4 | "Hiçbir üründe dışa aktarılan dosyanın içeriği açılmadı" | Hesap Defterim'in PDF'i açıldı: defter adı, dönem, Tarih · Notlar · Açıklama · Gelir · Gider · Denge; üstte Önceki denge, altta özet. **Aktarım bacakları PDF'te düz gider satırı** — Bölüm 1'deki toplamın dosyaya da taşındığı yer | **D** + E | HD-01 | aday `49` |
| 8.1 · Wallet (yeni madde) | — | Export formu: hesap, tür, ödeme tipi, tarih aralığı, **Include account transfers**, PDF · XLS · CSV. Ayarlar'da ve Records menüsünde yok | E | WL-09 | aday `f7-105` |
| 8.1 Şekil 8.4 | "Çıktı için üç ayrı seçenek … dosya üretimi denenmedi" | Seçeneklerin adı: PDF veya Yazıcı · Excel (.csv) · HTML. Dosya üretimi yine denenmedi | G | BC-16 | — |
| 8.1 Şekil 8.5 | "Dosya seçimi ve şema denenmedi. Banka ekstresi ayrıştırma ölçülmedi" | İçe aktarma yalnız CSV ve QIF; banka ekstresi ayrıştırıcısı yok | G | BC-14 | — |
| 8.2 Şekil 8.8 | "Silinen kaydın buraya düştüğü denenmedi" | Düşüyor ve geri yükleniyor; geri yüklenen kayıt listede yeniden açılınca görünüyor | G | BC-15 (`kosum`) | — |
| 8.2 madde 2 | "MM, Wallet ve Goodbudget'ta böyle bir katman … görülmedi" | "…taranan menü ve ayarlarda yok" | G | MM-11, WL-17, GB-07 | — |
| 8.4 tablo · Wallet dışa aktarma / dosya | "Aranmadı" / "Ölçülmedi" | "PDF · XLS · CSV; aktarımlar isteğe bağlı" / dosya yeri ölçülmedi | G | WL-09 | — |
| 8.4 tablo · HD · dosya nereye | "Cihazda kasadefteri klasörü; ürün söylüyor" | "Söylediği klasör oluşmuyor; PDF Documents altına, Excel uygulamanın kendi dizinine" | **D** | HD-02 | — |
| 8.4 tablo · MM · silinen kayıt | "Görülmedi" | "Yok — Daha ızgarasında çöp kutusu kalemi yok" | G | MM-11 | — |
| 8.4 tablo · Bluecoins · silinen kayıt | "Çöp kutusu yüzeyi var" | "Çöp kutusu çalışıyor" | G | BC-15 | — |
| 8.5 Şekil 8.14 | "Açık hâlinin davranışı görülmedi" | "Anahtar açılınca etiket seçici çıkıyor; seçilen etikete ne yaptığı görülmedi" | G (sınırlı) | BC-17 | — |
| 8.5 madde 2 | MM CalcBox ve PC'den Yönet "içi bu koşumda açılmadı" | CalcBox ayrı bir ürünün mağaza sayfasını açıyor; PC'den Yönet ücretli sürüm ekranına gidiyor. İkisi de uygulama içi araç değil | G | MM-08 | — |
| 8.6 akış · adım 2 | Wallet yok | Wallet eklenir: katlanmış menüde, üç biçim | G | WL-09 | — |
| 8.6 madde 1 | "hiçbir üründe dışa aktarılan dosya açılmadı" | Düşer | **D** | HD-01 | — |
| 8.7 çıkarım "Cihazdaki defter…" | "ürün klasörün adını veriyor" | Çıkarım korunur ama cümle düzelir: ürün verinin cihazda olduğunu doğru söylüyor, **dosyanın yerini yanlış söylüyor** — kullanıcıyı sahibi yapan ürün, dosyasını bulmayı ona bırakıyor. **K4** | **D** | HD-02 | — |
| 8.7 çıkarım "Hesap tabanlı ürünlerde soru hiç sorulmuyor" | "Biri bu taramada hiç bulunamadı, ötekinde aranmadı" | Wallet'ta bulundu ama **Ayarlar'da değil, katlanmış bir menü bölümünde** — çıkarım yeni kanıtla ayakta kalır. **K3** | **D** | WL-09 | — |
| 8.7 kazanç · HD | "dosyanın klasör adı" (kazandırdığı) | Kazandırdığından çıkar; kaybettirdiğine "söylediği klasör oluşmuyor" | **D** | HD-02 | — |
| 8.7 kazanç · MM | "masaüstünden bağlanma girişi var" | "…ücretli sürümde" | **D** | MM-08 | — |
| 8.7 kazanç · Bluecoins | "İçe aktarmanın hangi biçimleri okuduğu ve çöp kutusunun gerçekten çalıştığı ölçülmedi" | Kaybettirdiği yeniden: "İçe aktarma yalnız CSV/QIF, banka ekstresi okumuyor; geri yüklenen kayıt listeye ancak uygulama yeniden açılınca geliyor" | **D** | BC-14, BC-15 | — |
| 8.7 Belge 3 sorusu 3 | "İki üründe var, üçünde görülmedi" | "…üçünde taranan yüzeylerde yok" | G | MM-11, WL-17, GB-07 | — |

### Bölüm 9 · Ön muhasebe

Değişiklik yok. WB-01 (KolayBi Ortaklar = cari kartının alt türü) Bölüm 9'un değil
Belge 1 §7'nin eksiği; Bölüm 6.5'in Paraşüt/Logo satırlarıyla ilgili ama KolayBi
satırı "proje ekseni" üzerine. **İsteğe bağlı:** 6.5'te KolayBi "en yakın araç"
hücresine "Ortaklar cari alt türü" eklenebilir — önce `gozlemler/kolaybi.md`'ye
resmî kaynak URL'siyle yazılması gerekir (G2).

### Bölüm 10 · Kapanış — yalnız türeyen satırlar

| Yer | Şimdi | Yeni | Tür | Kaynak |
|---|---|---|---|---|
| 10.1 tablo satır 5 | Değişmez | — | — | — |
| 10.2 "Kararı kim veriyor" + 10.7 "Kararı kullanıcıya sormak nadir…" | "Üç yerde / iki üründe … borç, gelecek ödemeler, plan tarihi" | MM'nin kurulum diyaloğu sayılacak mı: **K2** | **D?** | MM-04 |
| 10.3 "Gelecek nasıl tutuluyor" · MM | "tekrarlayan plan önizlemede kalıyor" | + "vadesi gelince onaysız kayda dönüşüyor" | G | MM-04 |
| 10.3 "Gelecek nasıl tutuluyor" · Bluecoins | "gerçekleşirken hangi tarihe yazılacağı soruluyor" | + "hiçbir vade kendiliğinden gerçekleşmiyor" | G | BC-X2 |
| 10.4 Wallet · güçlü | "Kararı kullanıcıya soran ve sonucunu yazan **tek** ürün" | K2'ye bağlı | **D?** | MM-04 |
| 10.4 Hesap Defterim · kaybettirdiği | Değişmez; isteğe bağlı "dışa aktarma uyarısı oluşmayan bir klasörü gösteriyor" | E? | HD-02 |

## 4 · Kare seçimi

**İlke:** yeni kare yalnız (a) belgedeki bir yanlışı görünür biçimde düzeltiyorsa,
(b) sayfanın sorusuna metnin taşıyamayacağı bir kanıt getiriyorsa basılır. Bir
sayfanın kare sayısı artırılmaz; mümkünse **aynı iddiayı daha güçlü gösteren kare
eskisinin yerine geçer.** Böylece sayfa taşması ve bölüm uzunluğu riski düşer.

| # | Kare | Sayfa | Nasıl | Gerekçe | Kontrol değeri notu |
|---|---|---|---|---|---|
| 1 | `wallet-budgetbakers/f7-80` Labels 150 | 6.2 | **E0278'in yerine** | Categories/Labels geçişini o da gösteriyor + etiketsiz paranın düştüğünü | 21.750 ekranda yok; not gerekmez |
| 2 | `bluecoins/f7-67` etiket seçici | 6.2 | **E0088'in yerine** | İş/Kişisel etiketleri — bölümün sorusuna en yakın canlı kanıt | — |
| 3 | `hesap-defterim/49` PDF içeriği | 8.1 | ekleme veya E0169'un yerine | "Hiçbir dosya açılmadı" cümlesinin yerine geçen kanıt | Defter verisi değişmedi |
| 4 | `wallet-budgetbakers/f7-105` export formu | 8.1 | ekleme | Tabloda "aranmadı" diyen ürünün cevabı | — |
| 5 | `money-manager/69` tekrar zamanı ayarı | 5.2 | **E0243'ün yerine** veya ekleme | "onay izi yok" belirsizliğini kapatan kare | — |
| 6 | `money-manager/66` onay diyaloğu | 5.2 | 5 ile birlikte veya yalnız biri | Diyaloğun kendi cümlesi | — |
| 7 | `money-manager/71` bütçe listesi | 5.5 | ekleme | Yanlış "kurulmadı" cümlesinin düzeltmesi | Yalnız Yiyecek 1.400 |
| 8 | `wallet-budgetbakers/f7-82` Credit %2147483647 | 3.1 | ekleme | Metinle anlatılması zor, görünür taşma | — |
| 9 | `money-manager/50` sıfır tutar kabul | 1.5 | ekleme | MM'nin gruptan çıkması | Ekranda Eylül 1.600 — belgeyle uyumlu |
| 10 | `bluecoins/f7-75` transfer ücreti | 2.2 | **E0029'un yerine** (form alanlarını o da gösteriyorsa) | "Ücretin nasıl işlendiği görülmedi"nin cevabı | — |
| 11 | `wallet-budgetbakers/f7-66` aktarım formu | 2.2 | ekleme | Sayfada "karesi alınamadı" yazan formun kendisi | — |

**Basılmayan ama envantere girmesi gereken kareler** — çıkarım dayanağında veya
hücrenin `d` alanında kimlikle anılacakları için: `f7-68` (Wallet ödeme sonrası gider),
`f7-56` (Bluecoins bütçe özeti), `f7-65` (Wallet hesap formu), `f7-86`
(bağlama listesi), `f7-73`, `f7-71` (Bluecoins çıktı/içe aktarma), `58`, `72`,
`48`. Yaklaşık **20 kare envantere** girer; 112'nin kalanı Belge 1 turunda
değerlendirilir.

**Basılmaz:** `f7-64` (kişisel veri; yerine `f7-65`), `f7-67` Wallet (WL-11
çürük), `f7-79` Bluecoins (üst şerit yarı saydam; iddia E0053 ile zaten basılı),
Goodbudget `39` ve `41` (Belge 1'in işi; karartma orada yapılır).

**Sayfa düzeni riski:** 1.5, 2.2, 5.2 ve 8.1 şu an dört-beş kareli `soru` sayfaları.
Ekleme yapılan her sayfa üretimde önizlemeyle denenir; taşarsa ekleme yerine
değiştirme yapılır ya da bulgu metinde kalır. `EN_AZ_KARE` kapısı düşmez — değişiklik
yalnız ekleme ve yer değiştirme.

## 5 · Kontrol değerleri

Koşum test verisi silinmedi; yeni karelerin bir kısmı belgedeki sayılardan farklı
sayı taşıyor (MM Eylül 1.600 → 2.180; Wallet 21.600 → 21.750; Bluecoins 11.015 → 11.181).

**Önerilen kural:** §4'teki seçim bu farkı taşıyan kareleri (MM `51`–`58`, Wallet
`f7-76`/`f7-81`, Bluecoins `f7-79`) bilerek dışarıda bıraktı. Bu planı yazarken 1 ve 2
açılıp bakıldı (7'nin komşusu `70` de); diğerleri envantere girerken açılıp aynı gözle kontrol edilir — eski
kontrol değeriyle çelişen bir toplam taşıyan kare basılmaz. Metinde yeni sayı geçen yerlerde (1.2 tutar düzenleme 1.600 → 2.180;
6.2 Labels 150 / 21.750) cümle sayının **koşumda eklenen kayıttan sonra** okunduğunu
söyler. Tarih yazılmaz.

## 6 · Uygulama sırası (onaydan sonra)

1. **Ortak listeyi düzelt:** WL-11 → kapanmadı · GB-02 → 7.4'ü cevaplamıyor ·
   BC-17 sınırı · eksik Bölüm 5 satırı (BC otomatik kol) eklenir.
2. **Envanter:** §4'teki ~20 kare → `KANIT-ENVANTERI.md` (E0425'ten başlayarak;
   yol, boyut, SHA-256, içerik açıklaması) → `kanit-dizini-uret.py` yeniden koşar.
3. **Gözlem formları (G2):** kullanılacak her bulgu ilgili `gozlemler/*.md`
   dosyasına kare ve kimliğiyle yazılır; karesiz üçü (BC-15, BC-X2, HD-02) `kosum`
   olarak.
4. **Bölüm dosyaları:** `icerik.py` bölüm bölüm; önce **D** satırları, sonra G, en son E.
   Her bölüm kendi `uret.py`'siyle üretilir; kapılar + `onizleme/` gözle inceleme.
5. **Eksik listeleri:** kapananlar düşer, açık kalanlar (BC-X1, GB-05, GB-06, BC
   otomatik kol, GB-02'nin asıl sorusu, WL-11) kalır.
6. **`tam/` yeniden üretilir;** sayfa sayısı, kare sayısı, hash ve kırık atıf raporu.
7. **`belge2/README.md`** gerçek sayılarla güncellenir (şu an zaten eski).

Tahmini etki: 10 bölümün 8'i değişir; 9 dokunulmaz, 10 yalnız türeyen satırlar.
Sayfa sayısı 105 ± 2 beklenir.

---

## 7 · Açık kararlar

Önerim karar değildir; kullanıcı ve dış inceleme için yazıldı.

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **K1** | Karesiz kullanıcı gözlemi (BC-15, BC-X2, GB-04) ve karesiz koşum bulgusu (HD-02 dosya sistemi araması) nasıl basılır | A · motorun var olan `kosum` türüyle, hücre ve maddede · B · yalnız eksik listesinde "kullanıcı teyidi" notu, gövdede yok · C · emülatörde yeniden karesi alınır | **A.** `kosum` tanımı zaten "karesi yok … kullanıcı kontrolleri de buraya girer" diyor; Belge 2 Hesap Defterim bacak testinde aynı türü kullanıyor |
| **K2** | MM'nin kurulum diyaloğu ("Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?") Bölüm 10'daki "kararı kullanıcıya soran ürünler" sayısına girer mi | A · girer: "üç ürün, dört yer"; 10.4'teki Wallet "tek ürün" ifadesi düşer · B · girmez: diyalog bir davranış kararı değil, kaydın tekrarlı olduğunu doğrulama; ama 5.2'de "sonucu cümlede yazan onay" olarak anılır · C · 10.4 "kapının davranışını soran tek ürün" diye daraltılır, MM ayrı anılır | **C.** Wallet'ın ayrıcalığı kapının kendisini sorması; MM kurulumu soruyor, gerçekleşmeyi sormuyor. İkisini aynı sayıma koymak bulguyu bulanıklaştırır |
| **K3** | 8.7 "Hesap tabanlı ürünlerde soru hiç sorulmuyor" çıkarımı | A · korunur, dayanak değişir: Wallet'ta dışa aktarma var ama katlanmış menüde · B · çıkarım kaldırılır, Goodbudget tek örnek kalır | **A.** Yeni kanıt çıkarımı çürütmüyor, "öne çıkmıyor" iddiasını görünür kılıyor |
| **K4** | Hesap Defterim'in yanlış klasör uyarısı ne kadar ağırlık alır | A · 8.1 + 8.4 + 8.7 çıkarım ve kazanç düzeltmesi · B · yalnız 8.4 hücresi · C · A + 10.4'e bir cümle | **A.** Belge şu an ürünü tam bu konuda övüyor ("dosyanın yerini söylüyor"); yalnız hücreyi düzeltmek çıkarımı yanlış bırakır. 10.4'e taşımak ağırlığı abartır |
| **K5** | MM Kasım/Aralık kart defteri kareleri (3.7) | A · basılmaz, metinde zincir tamamlanır · B · Şekil 3.21 ile 3.22 arasına iki kare eklenir | **A.** Sayfanın iddiası iki uç sayının farkı; ara iki kare aritmetiği görsel olarak tekrar eder, yeni bilgi getirmez |
| **K6** | Bölüm 6'daki Bluecoins bulgusu (İş/Kişisel etiketi + parça başına etiket) bölümün "bir yokluğun bölümü" girişini değiştirir mi | A · giriş aynen kalır, bulgu 6.2 ve 6.6'da · B · giriş "alan yok, ama bir üründe hazır İş/Kişisel etiketleri var" diye yumuşar | **A.** Kapsam **alanı** yine yok; etiketlerin ürünle mi geldiği de görülmedi. Girişi değiştirmek için ikisinin de kanıtı gerekir |
| **K7** | Kanıt değişince kazanç tablosunun "kaybettirdiği" hücresi boşalıyorsa (2.7 Bluecoins, 5.8 MM, 8.7 Bluecoins) | A · yeni kanıttan çıkan gözlenen bir bedelle doldurulur · B · hücre "—" kalır | **A**, ama yalnız gözlem formunda yazılı bir bedelle (G2). Aşağıda her biri için aday: Bluecoins 2.7 → "ücret aktarımdan ayrı bir gider kaydı: aynı işlem listede üç satır"; MM 5.8 → "bütçe değişikliği ancak önümüzdeki aydan geçerli"; Bluecoins 8.7 → "geri yüklenen kayıt listeye ancak yeniden açılınca geliyor" |
| **K8** | WL-05 (Wallet kartı gider sayıyor mu) teyit edilemezse | A · hücre "görülmedi"de kalır · B · "tüm hesaplarda 21.600, Ana Hesap'ta 15.600" gibi yalnız gözlenen sayılar yazılır, yorum yapılmaz | **Önce teyit.** Olmazsa **B** — sayılar doğru, yalnız 6.000'in kaynağı iddia edilmez |

## 8 · Dış inceleme için sorular

1. §2'deki doğrulamada gözden kaçan bir çelişki var mı? Özellikle: WL-11'in çürük
   sayılması, GB-02'nin 7.4'ü cevaplamadığı yargısı.
2. §3'teki **D** satırlarının her biri gerçekten belgede basılı bir yanlış mı, yoksa
   dar okunmuş bir ifade mi? (5.5 bütçe, 8.1 klasör, 6.6 bölme, 1.7 adım 4, 2.2 sayım.)
3. §4'teki "kare sayısını artırma, yer değiştir" ilkesi E0278 → `f7-80` ve
   E0088 → `f7-67` değişimlerinde eski karenin taşıdığı bir iddiayı kaybettiriyor mu?
4. Bölüm 5'teki yeni üçlü (MM kapıyı kaldırmış · Wallet kullanıcıya vermiş ·
   Bluecoins her gerçekleşmeyi kullanıcıya yıkmış) kanıtın söylediğinden fazlasını
   söylüyor mu? Bluecoins'in otomatik kolu hâlâ ölçülmedi.
5. K2'deki ayrım (kurulumu sormak ile kapıyı sormak) okur için anlamlı mı, yoksa iç
   bir inceltme mi?

## 9 · Bu planın kapsamı dışında — Belge 1'e kalanlar

MM-01, MM-02, WL-01, WL-10, WL-14, WL-15, WL-18 (3.1'e giren kısmı hariç), WL-19,
WL-20, BC-01, BC-06, BC-07, BC-13, BC-16 (8.1'e giren kısmı hariç), GB-01, GB-03,
GB-04, HD-03, WB-01, WB-02, HD-04. Belge 1 turu bu planın uygulanmasından sonra
aynı yöntemle yapılır; Goodbudget `39`/`41` ve Wallet `f7-64` karartması orada
çözülür.


---

## 10 · Uygulama sonucu (23 Eylül 2026)

**Kararlar** — K1 kullanıcı kararıyla A; K2–K8 kullanıcının devriyle önerildiği gibi
(K2 C · K3 A · K4 A · K5 A · K6 A · K7 A · K8 teyit → çıkarım rozetiyle basıldı).

**Yedek:** `_yedek/2026-09-23-belge2-entegrasyon-oncesi/` (belge2/, envanter, dizin,
kalip.py, gözlem formları, ortak liste).

**Sonuç:** `belge2/tam/belge2.pdf` — 105 sayfa · 129 kare · hash 129/129 · kırık atıf
yok · uyarı yok. Bölüm başlangıç sayfaları değişmedi. Değişen sayfaların hepsi
önizlemeden gözle incelendi.

**Planın uygulanırken değişen yerleri**

- 38 kare envantere girdi (E0425–E0462), plandaki ~20 değil: metinde "görülmedi"den
  "kanıtlı"ya geçen her iddianın dayanağına kimlik yazılabilsin diye.
- Plan §4'teki 11 kare adayından basılanlar: E0425 (6.2), E0427, E0428, E0445 (8.1),
  E0430 (5.2), E0434 (1.5), E0435, E0436, E0462 (2.2). **Basılmayanlar:** `f7-82` —
  %2147483647 karede görünmüyor, iddia düştü; `71` — 5.5'teki E0405 zaten Yiyecek
  1.400'ü gösteriyordu, yalnız satırı düzeltildi; `f7-67` Bluecoins — E0088 aynı
  etiket listesini koşumdan önce gösteriyor ve daha güçlü kanıt.
- E0088'in koşumdan önceki tarihte de İş ve Kişisel etiketlerini göstermesi Bölüm 6'nın
  bir çıkarımını düzeltti: "ikinci ekseni olan tek canlı ürün Wallet" yanlıştı.
- WL-05 (Wallet kartı gider sayıyor mu) kareyle değil aritmetikle kuruldu: 21.600 =
  10.000 + 6.000 kart + 5.000 + 600 (E0440, E0441, E0398); hücre Çıkarım rozetli.
- Money Manager'ın Harç alanı için E0237 yerine E0462 basıldı; E0237 Harç'ı göstermiyor.
- Bluecoins BC-15 (çöp kutusu) 15 Eylül BC-U01 kaydıyla (E0401, E0402) ikinci kez
  doğrulandı; belge o kaydın gerisindeydi.

**Yolda bulunup düzeltilen eski hatalar:** 3.9 başlık "dört" / gövde "üç" ürün ·
2.5'te aynı madde iki kez · Bölüm 1 eksik listesi çıkarımı 1.6'ya gönderiyordu (1.8) ·
8.14'ün kırpması seyahat modu anahtarını kesiyordu · 8.4'te "dört üründe dosya eki"
cümlesiyle tablo tutmuyordu.

**Yeni eklenen motor olmayan yardımcı:** yok. Düzenlemeler yalnız `icerik.py`
dosyalarında; `kalip.py` ve `b2.py` değişmedi.


---

## 11 · Kare kırpma turu (23 Eylül 2026)

**Neden.** Kullanıcı incelemesi (s.35–38 Bölüm 4, Şekil 7.4 ve 7.6): karelerin tepesinde
saat ve pil çubuğu kalıyor, iddianın kendisi alttan kesiliyordu. Kök neden: telefon
kareleri `(0, 55, 1080, 1150–1550)` kutusuyla kesilmişti; 55 piksel durum çubuğunu atmaya
yetmiyor (çubuk 0–120 px), alt sınır ise ekranın yarısını atıyordu. Şekil 4.2 ve 4.3 bu
yüzden aynı kare gibi görünüyordu: ayırt edici alt kısım kutunun dışındaydı.

**Yedek:** `_yedek/2026-09-23-kirpma-oncesi/`.

**Kural.**

1. Telefon karesinde (1080×2400) üst sınır **120 px**: durum çubuğu atılır, uygulamanın
   başlık çubuğu korunur. Altı uygulamada ölçülerek doğrulandı.
2. Alt sınır **şeklin altındaki satırlarda yazılı son öğeyi** içerir; değer kare açılıp
   piksel olarak okunur. Satırın anlattığı şey karede görünmüyorsa ya kırpma düzelir ya
   satır değişir.
3. İçerik ekranın iki ucundaysa üst sınır da aşağı alınabilir (örn. E0308 `700–2300`:
   süs görseli atılıp soru ve düğmeler birlikte).
4. Sayfada boş yer varsa `yukseklik` artırılır (250 → 280–310); taşma kapısı sınırı
   belirler.
5. Kırpması yazılmamış telefon karesi motorun varsayılanıyla basılır:
   `(0, 120, 1080, 2330)` — durum çubuğu ve alttaki kaydırma çizgisi atılır. Bu kural
   `belge2/ortak/b2.py` içinde (`TELEFON_KIRPMA`, `Bolum._kirpma`); yalnız Belge 2'yi
   etkiler, `kalip.py` değişmedi. İşaretli karelere dokunmaz (işaret koordinatları
   kırpma sonrası uzayda).
6. KolayBi masaüstü kareleri (1412×960) zaten başlık çubuğunu atıyordu; değişmedi.

**Uygulama.** Önce bütün açık kutular `(0, 55, 1080, Y)` → `(0, 120, 1080, Y+65)`
kaydırıldı (78 kutu; aynı boy ve ölçek, düzen değişmez). Sonra kırpmalı her sayfa kareler
tam boy yan yana açılarak tek tek denetlendi ve şu şekillerin kutusu satırlarına göre
yeniden kuruldu:

| Sayfa | Şekil satırının anlattığı ama kutunun dışında kalan | Kareler |
|---|---|---|
| 4.1 | CARİ HESAP grubu; I Lent / I Borrowed menüsü; "Diğer uygulamalar" | E0049, E0316, E0358, E0177 |
| 4.2 | "Yes, select record · No, skip"; Due date; diyaloğun düğmeleri | E0308, E0309, E0315, E0317 |
| 4.3 | Bölmek · Durum · Etiket; CARİ HESAP satırları | E0074, E0055 |
| 4.3b | Diyaloğun tamamı; ikinci borç kartı (5.000) | E0367, E0368, E0370, E0398 |
| 5.1 | Sıklık listesinin sonu (Yıllık); otomatik kol kutucuğu; Frequency | E0238, E0039, E0288 |
| 5.3 | All / Income / Expense / Transfer filtresi | E0306 |
| 5.5 | Aşım uyarısı; günlük ortalama ve +%222 | E0301, E0302 |
| 6.1 | On bir kategorinin tamamı; Status ve Place | E0229, E0294 |
| 6.2 | Eğlence başlığı; alttaki Tahsilat / FaturaOdemesi düğmeleri | E0087, E0161 |
| 7.1 | Pasta altındaki kategori satırları; Net Total | E0231, E0023, E0423 |
| 7.2 | 6M ve 1Y kilitleri; Net Total | E0284, E0121 |
| 7.3 | İkinci blok (Net Kazanç); gider satırları | E0032, E0053, E0251, E0405 |
| 7.4 | **Total Spending −22.950 ve tablo — sayfanın iddiası tamamen kutunun dışındaydı** | E0123, E0124 |
| 8.1 | Android paylaşma sayfası; Include account transfers ve PDF/XLS/CSV | E0176, E0254, E0428 |
| 8.2 | Atla / Yedeklemeyi Aç | E0149 |
| 8.5 | Tamamlandı / Beklemede / Toplam sayaçları | E0165 |

1.4, 3.4, 3.7 ve 5.2'nin kutuları ilk kaydırmadan sonra satırlarını karşılıyordu,
değişmedi. Kırpmasız telefon kareleri varsayılan kutuyla basılıyor.

**Kullanıcı incelemesinden sonra (23 Eylül, ikinci oturum).** Kullanıcı yalnız s.76'yı
bildirdi: Şekil 8.10 ve 8.11'in (E0196, E0197) altı görünmüyordu. Neden kırpmaydı:
KolayBi masaüstü karelerinin `(96, 112, 1305, 700)` kutusu, pencerelerin 690–700 px'deki
düğmelerini kesiyordu → `740`. Aynı kutuyu taşıyan öteki yedi KolayBi karesi satırlarına
karşı kontrol edildi. Yalnız E0215 (Şekil 7.16) kesiliyordu: "Ödeme Yöntemi ve Banka"
kolonları ~796 px'deydi → `870`. Kalan altı karede (E0194, E0188, E0216, E0198, E0209,
E0210) şeklin anlattığı her şey kutunun içinde.

Aynı turda kullanıcı, "Neden ayrışıyorlar" sayfalarının ilk sayfasında alt çizgi ve sayfa
numarası olmadığını bildirdi. Neden: `b2.py` `_yer` yeni sayfa açarken biten sayfanın
`alt_bant`'ını çizmiyordu; yalnız son sayfa alıyordu. Düzeltildi; on bölüm ve `tam/`
yeniden üretildi. Artık sayfa numarası olmayan tek sayfa kapak.

**Belge 1 için not.** Belge 1'de şekillerin çoğu kırpmasız (tam ekran) ve 156 işaretli
şekil var; işaret koordinatları kırpma sonrası uzayda. Bu kural Belge 1'e aynen taşınamaz,
karar Belge 1 planında sorulur (`devir-2026-09-23-sonraki-oturumlar.md` bölüm 3).


---

## 12 · Çıkarım sayfalarının denetimi (23 Eylül 2026)

On bölümün "Neden ayrışıyorlar" sayfaları (çıkarım blokları, kazanç/kayıp tabloları,
Belge 3 soruları) kanıta ve belgenin kendi gövdesine karşı okundu. Ölçüt: çıkarım
serbesttir, ama belgenin başka bir sayfasıyla çelişemez, kanıtın söylediğinden fazlasını
söyleyemez ve "kaybettirdiği" hücresi bir kanıt boşluğu değil ürünün bir bedeli olmalıdır.

**Belgenin kendi gövdesiyle çelişenler (düzeltildi)**

| Yer | Eski | Neden yanlıştı | Yeni |
|---|---|---|---|
| 2.7 | "Aktarım hiçbir üründe tek bir nesne değil" | 2.3: Money Manager tek Havale satırı | "Bir üründe tek kayıt, üçünde iki satır" |
| 3.9 kazanç · Wallet | "eşik uyarısı kart mantığına özgü" | 3.2: uyarı her hesaptaki en az bakiye bildiriminden (E0437) | "ödeme günü alanı kart hesabına özgü" |
| 9.9 | "Canlı beş üründe bu ayrım hiç yok" | Bölüm 3 (kart) ve 4 (Bluecoins carisi) ayrımı gösteriyor; 10.3 da öyle | "Yalnız kartta ve caride beliriyor" |
| 9.9 | Goodbudget zarfı "ikinci bir defter değil" | Bölüm 1 ve 10.5: iki ayrı defter | "Goodbudget'ta iki defter" |
| 9.9 | "Canlı beş üründe kayıt kimseye ait değil" | Bluecoins'in cari hesabı bir karşı taraf | "Sıradan kayıt"; Bluecoins carisi istisna olarak eklendi |
| 10.6 | "Ölçülen bütün farklar tek soruya bağlanıyor" | 10.1: dokuzdan yedisi | "Farkların çoğu — dokuzdan yedisi" |
| 10.6 | "Üç ayrı bölümde" ile "dokuz bölümün her birinde" aynı paragrafta | İç çelişki | "Kart, borç ve rapor bölümlerinde" |
| 10.5 adım 3 | Money Manager için "gerçekleşme için bir kapı var" | 5.2: Money Manager'da kapı yok | "Kendiliğinden, onayla ya da elle" |
| 10.5 adım 4 | Wallet "dönemi ekranda yazmıyor" | Wallet LAST 30 DAYS / 12 WEEKS yazıyor | "Neyi saydığı ekranda yazmıyor" |

**Kanıtın söylediğinden fazlası (düzeltildi)**

- **Wallet'ın "varsayılan aralığı son 12 hafta"** (7.2 notu, 7.5 tablo, 7.7 akış, 7.8 çıkarım
  ve kazanç, 10.3, 10.4 — yedi yer). Karelerde iki ayrı pencere seçili: E0280 son 30 gün,
  E0284 son 12 hafta; ana ekran kartı son 30 gün. Varsayılanı gösteren kanıt yok.
  Desteklenen şey: rapor takvim ayıyla değil kayan bir pencereyle açılıyor.
- 1.8 "Beş üründe de kategori kovadır": Hesap Defterim'de seçici yok; "dört üründe".
  Aynı blokta Hesap Defterim için "kimlik ile kovayı tek alanda birleştiriyor" yanlıştı —
  kimlik Notlar'da, kategori "Açıklama / Kategori" adlı ayrı bir serbest metin kutusunda.
- 3.9 Bluecoins taksidi "bir ay 1.000 sonra sessizlik": kullanıcı kontrolüne göre kalan
  taksitler elle kaydedildikçe gider oluyor.
- 5.8 başlığı "Gerçekleşmemiş plan hiçbir üründe…" → "ölçülen hiçbir üründe"; Money
  Manager'ın taksitleri bu kuralın dışında olduğu eklendi.
- 5.8 Goodbudget'ta "'bütçeyi aştım' diye bir durum yok": aşım denenmedi; "ayrı bir bütçe
  ekranı yok, aşım denenmedi". Money Manager'ın bütçesi bu çıkarıma eklendi.
- 6.7 "Şahsi kayıt … canlı ürünlerde karşılığı olmayan": hesap düzeyinde karşılığı var
  (Toplama Dahil Et, Exclude from stats).
- 6.7 "canlı beş ürün tek cep varsayıyor": Bluecoins'in İş ve Kişisel etiketleri eklendi.
- 7.8 "öteki 43.850": Net Kazanç 43.350 (43.850 Varlıklar satırı).
- 8.7 Belge 3 sorusu "canlı ürünler tek biçim sunuyor": dört biçim var; "çıktı yalnız bir dosya".
- 9.9 "oran değişince rapor sessizce eskir": her fatura kendi oranını taşıyor; risk
  güncellenmeyen listeyle yeni belgelerin eski oranla kesilmesi.
- 10.6 "yanlış okumanın imkânsız hâle gelmesi" → "zorlaşması".

**Kanıt boşluğu olan "kaybettirdiği" hücreleri (ürünün gerçek bedeliyle değiştirildi)**

3.9 Goodbudget · 4.7 KolayBi · 5.8 Goodbudget · 6.7 QuickBooks · 8.7 KolayBi ·
9.9 KolayBi, Paraşüt, Logo İşbaşı. Her birinde yeni bedel aynı bölümün gövdesinde ya da
gözlem formunda yazılı olandan alındı.

Sayfa sayıları değişmedi; bütün bölümler ve `tam/` yeniden üretildi.
