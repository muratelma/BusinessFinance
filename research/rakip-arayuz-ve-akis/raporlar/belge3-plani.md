# Belge 3 — yöntem planı

**Durum:** taslak (24 Eylül 2026). Kullanıcı ve dış göz incelemesi için yazıldı; Belge 3'e başlanmadı.
Açık kararlar §10'da, her biri seçenek ve önerimle. Önerim karar değildir.

**Girdi:** Belge 1 (`belge1/tam/belge1.pdf`, 97 sayfa) ve Belge 2 (`belge2/tam/belge2.pdf`, 105 sayfa)
kapandı. Karar filtresi ve beş karar sonucu `../README.md`'de tanımlı; bu plan onları kopyalamaz,
kullanır.

---

## 1 · Belge 3 ne işe yarayacak

Belge 1 rakiplerin arayüzünü, Belge 2 finansal akışlarını anlattı; ikisi de bilerek karar vermedi.
Belge 3 bu iki belgenin bıraktığı soruları **BusinessFinance için kararlara** çevirir.

"İşe yarar" burada ölçülebilir bir şey demek. Belge 3 şu beş sınavı geçerse bu kadar iş boşa gitmez:

| # | Sınav | Neden |
|---|---|---|
| S1 | **Her öneri bir aşama belgesine kopyalanabilir:** hedefi, kapsamı ve kabul ölçütü yazılı | Aşama açılırken araştırma yeniden yapılmaz |
| S2 | **Her öneri kanıta izlenebilir:** Belge 1/2 alt bölümü ve kare kimliği | Karar sorgulandığında dayanağı bir tıkla açılır |
| S3 | **Her öneri BusinessFinance'in bugünkü hâliyle karşılaştırılmış ve bugünkü hâl doğrulanmış** | Var olan bir şeyi önermek ya da olmayanı var saymak en pahalı hata |
| S4 | **"Alma" kararları gerekçesiyle kayıtlı** | Aynı soru altı ay sonra yeniden araştırılmaz |
| S5 | **Kararlar yaşayan bir kayıtta durur:** uygulandıkça durumu güncellenir | PDF bir anlık görüntüdür; geliştirme sürdükçe kayıt güncel kalır |

Belge 3 bir özellik istek listesi değildir. "Rakip X bunu yapıyor, biz de yapalım" tek başına öneri
sayılmaz. Karar filtresi (README) iki yönde işler: kendi kararlarımız ölçüt değil, karşılaştırmanın bir
tarafıdır (12 Eylül kullanıcı düzeltmesi). En mantıklı yaklaşım rakipte de olabilir, bizde de.

## 2 · Girdiler

| Kaynak | Ne var | Hacim | Dikkat |
|---|---|---|---|
| Belge 2 bölüm soruları | Her bölümün "Belge 3'e taşınan soru" kutusu | 37 soru | Soruyu netleştirir, kararı vermez |
| Belge 2 10.6 | Belgenin kök soruları ve kazanç/kayıp tablosu | 11 soru | 37'nin özeti, onlarla çakışır |
| Belge 1 Bölüm 12 | Çoğunda aynı olanlar, ikiye ayrılanlar, tek üründe görülenler | ~30 satır | Arayüz tercihleri; 06.2'nin doğrudan girdisi |
| Belge 1 bölümleri | Her alt sorunun ürün ürün cevabı | 60 alt soru | Karar kartının "rakipler ne yapıyor" kısmı buradan |
| Gözlem formları | "Bizde karşılığı" ve karar etiketi notları | ~290 anma, 64 ADR göndermesi | **Doğrulanmadan taşınmaz** (aşağıda) |
| `BULGU-DOGRULAMA-KAYDI.md` | "Belge 3 adayları" tabloları, düzeltilmiş kıyaslar | 91 anma | En değerli ön çalışma; aynı zamanda en çok düzeltilen yer |
| Eksik listeleri | Açık kalan kanıt (erişim yok, denenmedi) | bölüm başına 2–6 | "Henüz karar verme"nin olağan kaynağı |
| Ürün tarafı | `PRD-BusinessFinance.md`, 17 ADR, `stages/06.2-arayuz-duzeni.md`, `docs/backlog.md` | — | Kararın gideceği yer |

**Ön çalışmadan alınacak ders.** Gözlem formları yazılırken birçok bulguya erken karar etiketi konmuş
(`uyarlayarak al`, `kararı yeniden sor` …). Bunların bir kısmı sonradan düzeltildi. Örneğin bir kıyas
"bizde KDV'yi yalnız toplama yolu yok" diyordu; kod okununca bu yeteneğin zaten var olduğu çıktı
(`BULGU-DOGRULAMA-KAYDI.md`, 15 Eylül kod kontrolü). **Kural:** eski etiketler ve "bizde karşılığı"
notları Belge 3'e **aday** olarak girer, karar olarak değil. Her biri §3'teki karşı gözlemle yeniden
sınanır.

## 3 · Eksik bacak: BusinessFinance'in kendi gözlemi

Belge 1 ve 2 dokuz ürünü aynı sorularla ve aynı senaryoyla ölçtü. **BusinessFinance'i ölçmedi.**
Karşılaştırmanın bir tarafı bugün yalnız ADR'lerin ve belleğin söylediği şey. S3 sınavı buna dayanır:
bir öneri "bizde yok" diyorsa bunun kanıtı olmalı, rakipteki gibi.

Önerilen iş: **Belge 3'ün dokunduğu her soru için BusinessFinance'in cevabı**, aynı kanıt türleriyle.
- **Davranış (Belge 2 soruları):** kod ve API okuması. Hangi kayıt hangi toplama giriyor, hangi
  endpoint ne döndürüyor. Dayanak: dosya yolu, test adı, sözleşme belgesi.
- **Arayüz (Belge 1 soruları):** uygulama emülatörde aynı senaryoyla açılır, ekran kareleri Belge 1'in
  kanıt kurallarıyla alınır (sentetik veri, kimlik, hash). Dayanak: kare kimliği (`BF-`).

Bu iş kod değiştirmez; aktif aşama kuralına dokunmaz. Uygulamayı yerel olarak çalıştırmak gerekir
(API + SQL + Pixel 8). Kapsam, yalnız karar kartlarının sorduğu ekran ve akışlarla sınırlıdır; tam bir
Belge 1 turu değildir. Seçenekler K1'de.

## 4 · Karar kartı

Belge 3'ün birimi karar kartıdır. Bir soru, bir kart. Kart şu alanları taşır:

| Alan | İçerik |
|---|---|
| **Kimlik** | `B3-NNN`, kalıcı; aşama belgeleri ve commit'ler bu kimlikle gönderir |
| **Soru** | Tek cümle, kullanıcının diliyle (ör. "Veresiye satışta tahsil edilen ile bekleyen ayrı görünmeli mi?") |
| **Rakipler ne yapıyor** | Gözlenen yaklaşımlar, ürün adıyla; her biri Belge 1/2 alt bölümüne ve kanıt türüne bağlı |
| **BusinessFinance bugün** | Doğrulanmış cevap ve dayanağı (§3). Doğrulanmadıysa kart karar alamaz |
| **Seçenekler** | Gözlenen yaklaşımlar + bizimki + varsa karma; en az iki |
| **Ödünleşim** | Her seçenek ne kazandırıyor, ne kaybettiriyor. Ölçü hedef kullanıcıdır: şahıs şirketi ve esnaf (PRD) |
| **Önerilen sonuç** | Beş sonuçtan biri (README) ve gerekçesi |
| **Güven** | Kanıtın gücü: birden çok üründe canlı kare > tek ürün > kaynak görseli > yalnız kaynak beyanı |
| **Hedef** | Kararın gideceği yer: 06.2 grup maddesi · yeni 06.x aşama adayı · backlog satırı · ADR sorusu · tasarım sistemi kuralı · yalnız kayıt |
| **Kabul ölçütü taslağı** | Hedef bir aşamaysa: "bittiğinde ne doğru olmalı", test edilebilir biçimde |
| **Bağımlılık** | Başka bir karta ya da ADR'ye bağlıysa |

Örnek (yalnız biçimi göstermek için; karar değildir):

> **B3-0xx · Kart harcaması ile ödenen aynı yerde nasıl okunmalı?**
> *Rakipler:* Money Manager toplamı ödeme kaynağına göre bölüp satır adında yazıyor (Belge 1 9.1,
> Belge 2 3.x); Bluecoins ve Wallet tek sayı veriyor. *BusinessFinance bugün:* … (doğrulanacak).
> *Seçenekler:* A tek sayı · B kaynağa göre bölünmüş satır · C tek sayı + açıklama satırı.
> *Hedef:* sunum değişikliği ise 06.2 Grup 4; toplamın anlamı değişiyorsa yeni aşama (06.2 finansal
> davranış değiştirmez).

## 5 · Kurallar ve kapılar

Belge 1 ve 2'nin kapıları üretimi durduruyordu; Belge 3'ün kapıları kart düzeyinde çalışır.

| Kapı | Kural |
|---|---|
| K-kanıt | Her kartta en az bir Belge 1/2 göndermesi ve geçerli kare kimliği var |
| K-bugün | "BusinessFinance bugün" alanı dolu ve dayanaklı; boşsa kart `henüz karar verme` dışında sonuç alamaz |
| K-alma | `alma` gerekçesi rakibin çözümünün **neyi kaybettirdiğini** söylüyor; "ADR'miz böyle" tek gerekçe olamaz (README) |
| K-yeniden-sor | `kararı yeniden sor` iki koşulu yazılı taşıyor: adı konmuş boyut ve o boyutun hedef kullanıcı için önemi. Sağlanmıyorsa sonuç `alma` ya da `henüz karar verme` |
| K-hedef | Her kartın hedefi var; hedef 06.2 ise öneri yeni özellik ya da finansal davranış değişikliği içermiyor (06.2 kapsam sınırı) |
| K-güven | Yalnız kaynak beyanına dayanan öneri `doğrudan al` alamaz |
| K-ADR | Hiçbir kart bir ADR'yi değiştirmez; en fazla soruyu masaya koyar (ADR yalnız yeni ADR ile değişir) |

## 6 · Belgenin yapısı

Önerilen iskelet (K2'nin B seçeneği):

1. **Yönetici özeti (2–3 sayfa):** en önemli 10 karar, "kararı yeniden sor" soruları, yol haritasına
   etkisi tek tabloda. Patron yalnız bunu okusa bile karar verebilmeli.
2. **Yöntem (1 sayfa):** karar sonuçları, güven düzeyi, BusinessFinance karşı gözlemi.
3. **Konu bölümleri:** kartlar konuya göre, Belge 1 ve 2'nin ortak eksenleriyle. Taslak: gezinme ve
   görsel dil · kayıt formu · hesap, kart, transfer · plan ve bekleyen · borç, cari, tahsilat ·
   sınıflandırma ve işletme/şahsi · rapor ve toplamın tanımı · veri ve paylaşım · vergi ve muhasebeci.
4. **Kararı yeniden sor:** README'nin istediği ayrı başlık; her soru ödünleşimiyle.
5. **Yol haritasına etkisi:** 06.2'ye girecek maddeler (grup grup), yeni aşama adayları (kapsam
   taslağıyla), backlog satırları. Bunlar **öneridir**; aşama belgesine kullanıcı onayıyla geçer.
6. **Alma ve ertelenenler kaydı:** her `alma` ve `henüz karar verme` bir satırda, gerekçe ve "ne
   değişirse yeniden bakılır" koşuluyla.
7. **Ek:** karar kaydının tamamı (tablo), kanıt dizini.

## 7 · Çıktı: belge ve yaşayan kayıt

İki çıktı öneriyorum:
- **Belge 3 (PDF):** Belge 1 ve 2 ile aynı motor ve görsel dil; patron ve dış göz için okunur belge.
  Tarihli bir anlık görüntüdür.
- **Karar kaydı (Markdown):** her kartın tek satırı ve durumu (`açık` · `aşamaya bağlandı: 06.x` ·
  `uygulandı: commit` · `reddedildi`). Geliştirme sürdükçe güncellenen tek dosya. Aşama belgeleri
  `B3-NNN` ile buna gönderir. Belge 3 PDF'i bu kayıttan üretilir, ters yönde değil.

Kaydın yeri bir karardır (K4): araştırma klasöründe kalırsa geliştirme oturumları onu görmez.
`docs/` altında ve `CLAUDE.md` belge haritasında yer alırsa her oturum okur. `CLAUDE.md` ve `AGENTS.md`
karar belgeleridir; değişiklik kullanıcı onayıyla yapılır.

## 8 · Önceliklendirme

Kartlar dört ölçüyle sıralanır, puanla değil gerekçeyle:
1. **Hedef kullanıcıya etkisi:** şahıs şirketi ya da esnafın gündelik işini değiştiriyor mu
2. **Kanıt gücü:** §4'teki güven düzeyi
3. **Maliyet ve risk:** yalnız sunum mu, veri modeli mi, migration mı
4. **Aşama uygunluğu:** 06.2'ye bugün girebilir mi, yoksa kendi aşamasını mı ister

Sonuç üç kovadır: **hemen** (06.2'ye madde), **sıradaki aşama adayları**, **ertelenen** (koşuluyla).

## 9 · Aşamalar

| Aşama | İş | Çıktı | Kullanıcı |
|---|---|---|---|
| 0 · Soru havuzu | Bütün girdiler (§2) tek havuza; çakışanlar birleşir, her soru `B3-NNN` alır, kaynakları yanında | `belge3-soru-havuzu.md`; tahmini 90–120 ham soru → 45–60 kart | Havuzu görür, kapsam dışı bıraktıklarını söyler |
| 1 · Karşı gözlem | Her kart için BusinessFinance bugün (§3): kod okuması ve gerekenlerde emülatör karesi | Kartların "bugün" alanı dolu; `BF-` kareleri envanterde | — |
| 2 · Kart taslakları | Seçenekler, ödünleşim, önerilen sonuç, hedef, ölçüt; kapılar | `belge3-kartlar.md` (seçenekli, önerili) | Kart kart karar; dış göz |
| 3 · Kararlar | Kullanıcı kararları işlenir; "kararı yeniden sor" soruları ayrı tartışılır | Karar kaydı | Karar verir |
| 4 · Belge | Belge 3 PDF'i kayıttan üretilir; kapılar, gözle inceleme | `belge3/tam/belge3.pdf` | İnceler |
| 5 · Bağlama | Onaylı "hemen" kartları 06.2 Grup 6'ya, aşama adayları taslak olarak, backlog satırları | Aşama belgesi ve backlog güncellemesi | Her bağlamayı onaylar |

Aşama 0 ile 1 birbirini besler: havuz çıkmadan hangi ekranın karşı gözleme gireceği bilinmez.

## 10 · Açık kararlar

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **K1** | BusinessFinance karşı gözlemi nasıl yapılır | A · yalnız kod ve belge okuması · B · yalnız emülatörde aynı senaryo · C · ikisi: davranış koddan, arayüz ekrandan | **C.** Belge 2 soruları davranış sorusu, koddan kesin cevaplanır. Belge 1 soruları ekran sorusu; koddan okunan ekran ile görünen ekran ayrışabilir (06.2'nin varlık nedeni bu). Ön çalışmanın KDV hatası kod okunmadığı için oldu |
| **K2** | Belgenin ana ekseni | A · Belge 2'nin bölüm sırası · B · konu bölümleri + yönetici özeti + yol haritası bölümü (§6) · C · yalnız hedefe göre (06.2 / yeni aşama / ADR / alma) | **B.** A okuru rakip sırasına bağlar; C karar verirken bağlamı koparır. B hem okunur hem yol haritasına çevrilir |
| **K3** | Belge 1'in soruları nasıl girer | A · Bölüm 12'nin satırları ve bölüm alt soruları havuza girer · B · yalnız Bölüm 12 · C · Belge 1 yalnız "rakipler ne yapıyor" kanıtı olur, soru üretmez | **A.** Belge 1 karar gereği soru bırakmadı; ama 06.2'nin doğrudan girdisi o. Yalnız Bölüm 12 bazı alt soruları (ör. 7.4 ana ekranda bekleyen iş) kaçırır |
| **K4** | Karar kaydının yeri | A · `research/…/raporlar/belge3/karar-kaydi.md` · B · `docs/rakip-kararlari.md` ve `CLAUDE.md` belge haritasında bir satır · C · B + `AGENTS.md` belge güncelleme haritasında "aşama açılırken ilgili B3 kartları okunur" kuralı | **C.** S5'in şartı. Kayıt geliştirme oturumunun okuduğu yerde değilse ilk aşamada unutulur. `AGENTS.md`/`CLAUDE.md` değişikliği kullanıcı onayıyla |
| **K5** | Kapsam dışı konular (banka bağlantısı, e-belge, stok, vergi hesaplama) | A · karta girmez · B · kart alır, sonucu gerekçesiyle yazılır · C · ayrı bir "kapsam sınırları" bölümünde toplu | **B.** README "kapsam dışı bir gözlem etiketi değil, öneri kararıdır" diyor; patron kapsamı genişletmek isteyebilir. Kart, "neden şimdi değil ve ne değişirse" sorusunu cevaplar |
| **K6** | Kaynak ürünlerin (KolayBi, Paraşüt, Logo, QuickBooks) ağırlığı | A · canlı ürünlerle eşit · B · güven düzeyiyle işaretli, `doğrudan al` alamaz (K-güven) · C · yalnız bağlam | **B.** Türk ön muhasebe tarafının akış modeli (Belge 2 Bölüm 9) hedef kullanıcımıza en yakın olanı; ama kanıtı en zayıf olanı da o |
| **K7** | Belge 3'ün boyutu | A · sınırsız, her kart tam · B · yönetici özeti + en fazla ~60 kart; küçükler kayıtta tek satır · C · yalnız öncelikli 20 kart | **B.** Kart sayısı kısıtlanmazsa belge okunmaz; C S4'ü (alma kaydı) bozar |
| **K8** | Dış göz ne zaman | A · yalnız son belgede · B · kart taslaklarında (Aşama 2) ve son belgede · C · her aşamada | **B.** Kararın değiştiği yer kart taslağı; son belgede yalnız biçim kalır |

## 11 · Riskler

| Risk | Azaltım |
|---|---|
| Kendi kararlarımızı ölçüt sayıp kendini doğrulayan bir belge yazmak | K-alma ve K-yeniden-sor kapıları; her kartta en az iki seçenek; "BusinessFinance bugün" de ödünleşimle değerlendirilir |
| İstek listesine dönüşmek | Her kartın hedefi ve kabul ölçütü var; hedefsiz kart yazılmaz |
| Var olanı önermek, olmayanı var saymak | Aşama 1 karşı gözlemi; K-bugün kapısı |
| Eski karar etiketlerini devralmak | §2 kuralı: aday olarak girer, yeniden sınanır |
| 06.2'ye kapsamını aşan iş yüklemek | K-hedef: 06.2 yeni özellik ve finansal davranış değişikliği almaz; bunlar aşama adayı olur |
| Belgenin okunmaması | Yönetici özeti tek başına yeterli; kartlar kayıtta tek satır olarak da var |
| Kararın kayıtta unutulması | K4-C: kayıt geliştirme oturumunun okuduğu yerde |

## 12 · Dış göz için sorular

1. §1'deki beş sınav, "bu belge işe yaradı mı" sorusunu ölçmeye yetiyor mu? Eksik bir sınav var mı?
2. Karar kartı (§4) bir aşama belgesine kopyalanabilecek kadar somut mu, yoksa bir alan eksik mi
   (ör. tahmini iş büyüklüğü)?
3. Karşı gözlemin kapsamı (§3, K1) doğru mu? Bütün uygulamayı değil, yalnız kartların sorduğu ekranları
   ölçmek bir yanlılık üretir mi?
4. `kararı yeniden sor` eşiği (README) bir ADR'yi gerçekten sorgulamaya izin verecek kadar geniş mi,
   yoksa pratikte hiçbir soruyu geçirmeyecek kadar dar mı?
5. Kaynak ürünlerin ağırlığı (K6): hedef kullanıcıya en yakın ürünlerin kanıtı en zayıf; bu çelişki
   başka nasıl çözülebilir?
