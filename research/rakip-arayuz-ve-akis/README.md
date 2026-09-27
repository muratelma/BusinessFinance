# Rakip Arayüz ve Akış Araştırması

**16 Eylül son durum: pilot aşaması kapandı. Belge 1'in bölüm planı hazır, üretim motoru kuruldu
ve sınandı; bölüm yazımı sıradaki iş.** Başlangıç noktası
[raporlar/belge1/README.md](raporlar/belge1/README.md), tek başvuru kaynağı
[raporlar/belge1-bolum-plani.md](raporlar/belge1-bolum-plani.md). Ayrıntı
[DURUM.md](DURUM.md) "16 Eylül — Belge 1 üretim hazırlığı" bölümündedir.
Kanıt tarafında değişiklik yok; aşağıdaki satırlar o yüzden geçerli.

**15 Eylül kanıt durumu (geçerli):**
365/365 görsel incelendi ve envanterde (357 başlangıç + GB-U01 3, WL-U 3, BC-U01 2). B09 WL-U01 ile kapandı: Wallet'ın borç modeli ADR 0014'e eşdeğer değil.
B01–B19: 14 kanıtla kapalı, 5 kapsamı sınırlı (B06, B08, B10, B11, B12), 0 açık. P4-tema-01–10 kapandı. f7-54 hesap sahibinin adını gösterir: teslimde karartılmalı.
Ölçülmeyen davranışlar (Wallet Postpone/Dismiss sonucu, Bluecoins otomatik kol ve liste yenilenme kök nedeni, Hesap Defterim e-posta alıcısı/teslimi) doğrulanmış sayılmaz.
Mekanik kapı: dokuz form 0 hata/0 uyarı, 21/21 test. Geçiş kapısı 12/12; 15 Eylül kullanıcı onayı kaydedildi. Pilot değerlendirilmeden tam raporlar yazılmaz.
Sıradaki tek iş pilot değerlendirmesi. Teslim: raporlar/pilot/pilot-islem-ekleme.pdf ve .docx.
Aşağıdaki eski paket devirleri tarihseldir; güncel ayrıntı BULGU-DOGRULAMA-KAYDI.md sonundadır.


## Güncel Goodbudget kapsamı — 14 Eylül 2026

Goodbudget G01 28/28 tamam; form düzeltmeleri uygulandı. B06 nedensellik
iddiaları sınırlandırıldı, B07 hesap/paket bilgisi kaynakla kapandı. B05/GB-U01 kullanıcı ekran kontrolüyle kapandı. Goodbudget tamam;
agent yeni emülatör koşumu başlatmadı.
KolayBi G01/G02, P1-B08, P1-B15, P1-hesap-defterim-G01/G02, P1-B11, P1-B12, P1-money-manager-G01, P1-parasut-G01, P1-logo-isbasi-G01, P1-quickbooks-G01, P1-B13, P1-B18, P1-K kapandı (E0392); sıradaki paket P2-G01. Faz 8 açılmadı.

Aşağıdaki eski Goodbudget özetlerinde geçen zorunlu gelir-zarf ilişkisi,
liste dışı tekrar, kalıcı silme kusuru ve bütün hesap türlerine ortak limit
hükümleri tarihsel/geçersizdir; rapora taşınmaz. Güncel kaynak
[Goodbudget formu](gozlemler/goodbudget.md) ve
[bulgu kaydı](BULGU-DOGRULAMA-KAYDI.md) sonundaki kapanış bölümüdür.
28/357 yalnız yeni görsel denetimin sayacıdır; önceki koşumlar korunur.
Ek canlı kontrol iş bölümü FAZ7-8-UYGULAMA-PLANI.md'deki 14 Eylül kararıdır.


## Belge durumu

- Durum: Bluecoins P2-K kapandı (90/90); Wallet doğrulaması P3'te yapılacak.
  B09/B19 açık, B10 kapsamı sınırlı. Faz 7.5 genel kapanışı ve Faz 8 henüz açılmadı.
- **Amaç: BusinessFinance ürününün tamamı için karar girdisi üretmek.**
  Kapsam **yalnız arayüz değildir** — dört boyut birlikte incelenir:
  **arayüz · özellikler · akışlar · pipeline'lar (arka plan olay modeli).**
- Sınır: Bu çalışma **hiçbir aşamayı açmaz** ve uygulama kodunu değiştirmez.
  Çıktısı belgedir; hangi bulgunun hangi aşamaya gireceği ayrı bir karardır
- Veri: Yalnız sentetik test verisi kullanılır

**13 Eylül 2026 oturum devri:** [Faz 7/7.5 kapanışı ve Faz 8 üretim planı](FAZ7-8-UYGULAMA-PLANI.md)
kaydedildi ve P0.1 tamamlandı: [kanıt envanteri iskeleti](KANIT-ENVANTERI.md)
oluşturuldu. P0.2 bulgu/doğrulama kaydı da tamamlandı; güncel iş
[DURUM.md](DURUM.md)'dedir. Görsel paketleri hedef 20–30 görseldir; içerik
incelemesi henüz başlamadı. Geçmiş Tur 1/2 koşumları Faz 7.5 yerine geçmez.

**Kapsam notu (12 Eyl 2026, kullanıcı düzeltmesi):** bu dosya ve
`MANUEL-TEST-PROTOKOLU.md` başlangıçta amacı *"Aşama 06.2 arayüz düzeni"*
diye yazıyordu. Bu **yanlıştı ve daraltıcıydı**: saha çalışması para modeline
kadar indi (tanır/taşır ayrımı, KDV, tahakkuk-nakit, cari ekstre, tekrarlayan
plan üretimi) ve bu bulguların hiçbiri bir arayüz düzeni sorusu değil.
Araştırma **genel proje için** yapılır.

P0.2'de [bulgu/doğrulama kaydı](BULGU-DOGRULAMA-KAYDI.md) da oluşturuldu;
B01–B19 açık. 14 Eylül itibarıyla Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı, QuickBooks, KolayBi, Bluecoins ve Wallet metin haritaları tamam; P0.4 kabul kontrolüyle P0 kapandı; P1-B01, P1-B02-B04, P1-B14 ve P1-goodbudget-G01 kapandı; sıradaki paket
P1-goodbudget-T01. Önceki paket kaydı tarihsel ilerlemedir.

## Beklenen çıktılar

Araştırma sonunda birbiriyle bağlantılı üç belge hazırlanacak:

1. **Rakip Uygulamalarda Arayüz Yaklaşımları**
   - Bilgi hiyerarşisi, gezinme, renk, tipografi, kartlar, grafikler ve form
     desenleri
   - Güçlü ve zayıf yaklaşımlar
2. **Rakip Uygulamalarda Finansal Akışlar ve Özellikler**
   - Ana ekran, gelir/gider kaydı, işletme/şahsi ayrımı, planlama ve
     raporlama akışları
   - **Arayüzün arkasındaki işleyiş:** bir kaydın sistemde ne ürettiği, olay
     modeli, ekrandan ekrana pipeline aşamaları, entegrasyon temas noktaları
   - Akışlar arası bağlantılar ve kullanıcı sürtünmeleri
3. **BusinessFinance Arayüz ve Akış Önerisi**
   - Bulguların karara dönüştürülmesi — beş sonuç için aşağıdaki
     **"Karar sonuçları"** tablosuna bakın (tek kaynak orasıdır)
   - Etkilenen ekran, akış, olay modeli ve ortak bileşenler

Belgeler yalnız "ne göründüğü"nü değil, **nasıl işlediğini** anlatır:
uygulamanın asıl amacı, arka plan olay modeli ve pipeline aşamaları her
uygulama için gözlem formunda ayrı bölümlerdir (`UYGULAMA-GOZLEM-SABLONU.md`).
İçine girilemeyen uygulamalarda bu boyut yardım merkezi adım adım makaleleri,
ürün turu ve kullanıcının izlediği tanıtım/eğitim videolarından `Resmî kaynak`
etiketiyle yeniden kurulur.

Nihai belgeler düzenlenebilir Word dosyası olarak hazırlanacak, onaylanan
sürümleri ayrıca PDF olarak dışa aktarılacak. Word ve PDF farklı içerikler
değil, aynı belgenin iki biçimi olacak. Taslak dosyaları, dosya adları ve
onay durumu `raporlar/README.md` içindedir.

## İlk karar: incelenecek uygulamalar

Her uygulamayı aynı tür rakip saymak yerine, BusinessFinance'in farklı bir
sorusuna cevap veren uygulamalar kullanıldı. **Başlangıçta yedi uygulama
seçildi; Faz 4'te (10–11 Eyl 2026) iki uygulama daha eklendi ve toplam dokuz
uygulama incelendi.**

### Çekirdek yedi uygulama (ilk seçim)

| Uygulama | Araştırmadaki rolü | Seçim gerekçesi |
|---|---|---|
| Paraşüt | Yerel doğrudan rakip | Türkiye'deki ön muhasebe, tahsilat/ödeme ve mobil işlem yaklaşımı |
| Logo İşbaşı | Yerel doğrudan rakip | Mikro işletme ve serbest meslek odağı; mobil nakit akışı ve belge girişi |
| KolayBi | Yerel akış referansı | Haftalık nakit akışı; fatura→tahsilat ve fatura→ödeme bağlantısı |
| QuickBooks Online / Solopreneur | Kavramsal yakın rakip | Tek kişilik işletmede işletme/şahsi işlem sınıflandırması |
| Money Manager (Realbyte) | Para modeli referansı | Manuel hesaplar, kart borcu/ödemesi, transfer, tekrarlayan işlem ve işletme/şahsi kayıtlar |
| Wallet by BudgetBakers | Finansal UX referansı | Günlük para takibi, ana ekran, kategori dağılımı ve rapor okunabilirliği |
| Bluecoins | Geniş para modeli referansı | Hesap, kart, borç, hatırlatma, nakit akışı, net varlık ve CSV akışları |

### Sonradan eklenen iki uygulama (Faz 4, 10–11 Eyl 2026)

"Bize benzemeyen" bir Tur 2 slotu için aday olarak eklendiler; ikisine de tam
Tur 1 koşumu yapıldı, Faz 5'te slota seçilmediler ama **bulguları Belge 1/2/3'e
Tur 1 derinliğinde girer.**

| Uygulama | Araştırmadaki rolü | Seçim gerekçesi |
|---|---|---|
| Hesap Defterim (Cash Book) | Esnaf kasa/veresiye defteri | TR yerelleşmiş, kayıt gerektirmeyen tek-sütunlu yürüyen bakiye modeli — bizden en uzak veri modeli |
| Goodbudget | Dijital zarf bütçe | Zarf bütçesi ve ayrı gelir/fonlama yolları; Credit sonuçları normal gelire genellenmez |

### Kullanılmayan yedek adaylar

**Bizim Hesap** ve **Spendee** yedek olarak listelenmişti; ihtiyaç doğmadığı
için **hiç kullanılmadılar** ve araştırmaya girmeyecekler. Kayıt olarak burada
bırakılıyorlar.

QuickBooks Solopreneur'a bölgesel ve ödeme kaynaklı erişilemedi; Solopreneur
bulguları resmî yardım sayfalarına dayanıyor ve `Resmî kaynak` etiketli.
Manuel gözlem yalnız QuickBooks mobil onboarding'inin dört karesidir; son kare
QBO `Simple Start` plan ekranıdır, Solopreneur fiyatı veya akışı değildir.

### Seçim ölçütleri

Bir uygulama listeye yalnız aşağıdaki sorulardan en az ikisine anlamlı cevap
veriyorsa girer:

- Şahıs şirketi veya mikro işletmenin gündelik finans işlerini kolaylaştırıyor mu?
- Gelir, gider, tahsilat, ödeme veya nakit akışı için incelenebilir bir akışı var mı?
- BusinessFinance'ten farklı ve öğretici bir arayüz yaklaşımı sunuyor mu?
- Deneme hesabı, demo veya güvenilir resmî kaynakla incelenebiliyor mu?

Stok, bordro, banka bağlantısı veya vergi hesaplama tek başına **uygulama
seçim** nedeni değildir. Ama seçilmiş bir uygulamanın bu özellikleri Belge 1
ve 2'de yine anlatılır — belgeleme ile kapsam kararı ayrı şeylerdir.

## BusinessFinance karar filtresi

**Bu filtre yalnız Belge 3'te (öneri) uygulanır.** Belge 1 ve Belge 2 rakibin
tüm özellik yüzeyini tarafsız anlatır — bizde kapsam dışı sayılan özellikler
(stok, banka bağlama, KDV hesaplama, e-belge) dahil; raporu patron okuyup
kapsamı genişletmek isteyebilir. "Kapsam dışı" bir gözlem etiketi değil, bir
öneri kararıdır.

Rakipte bulunması bir özelliğin BusinessFinance'e alınacağı anlamına gelmez.
Belge 3'te her bulgu aşağıdaki kararlarımızla karşılaştırılır:

- İşletme ve şahsi para tek havuzda yaşar; ayrım raporlama boyutudur.
- Bakiye, kart borcu ve net varlık kapsam filtresinden etkilenmez.
- Kapsam ile indirilebilirlik ayrı alanlardır.
- Transfer ve kart ödemesi gider değildir.
- Vergi hesaplanmaz ve beyanname üretilmez.
- Muhasebe kârı yerine nakit esaslı işletme neti kullanılır.
- Banka bağlantısı ve açık bankacılık kapsam dışıdır.

### Bu liste ölçüt değil, karşılaştırmanın bir tarafıdır

**(12 Eyl 2026'da kullanıcı düzeltmesiyle eklendi.)** Bu araştırmanın amacı
rakiplerin aynı problemlere nasıl yaklaştığını görüp **en mantıklısını**
seçmektir. Kendi kararımızı baştan doğru sayarsak araştırmanın bir anlamı
kalmaz: çıktı, önceden verilmiş kararların kendini doğrulaması olur.

Bu yüzden bir bulgu yukarıdaki kararlardan biriyle çakıştığında **dört değil
beş** sonuç mümkündür:

### Karar sonuçları — tek kaynak

**Beş sonucun tek tanım yeri burasıdır.** `raporlar/README.md` ve
`UYGULAMA-GOZLEM-SABLONU.md` buraya işaret eder, listeyi kopyalamaz.

| Sonuç | Ne demek |
|---|---|
| `doğrudan al` | Kararlarımızla uyumlu, olduğu gibi alınabilir |
| `uyarlayarak al` | Fikir iyi, biçimi bize uymuyor |
| `alma` | Denendi, tartıldı, bize uymuyor — **gerekçe rakibin çözümünün neyi kaybettirdiğini söylemeli**, yalnız "ADR'miz böyle" demek yeterli değil |
| `henüz karar verme` | Veri yetmiyor veya kapsam kararı patronundur |
| **`kararı yeniden sor`** | **Rakibin yaklaşımı, hedef kullanıcımız için önemli olan ve adı konmuş bir boyutta bizimkinden iyi görünüyor; ilgili ADR yeniden değerlendirilmeli.** Belge 3'te ayrı başlık altında, ödünleşimiyle birlikte patrona sorulur |

**`kararı yeniden sor` etiketinin eşiği var.** Her fark bu etiketi hak etmez;
aksi hâlde Belge 3 bir ADR sorguları listesine döner ve asıl öneriler kaybolur.
İki koşul birlikte sağlanmalı:

1. **Boyut adı konmuş olmalı** — "daha iyi" yetmez; hangi boyutta iyi olduğu
   yazılmalı (ör. *"aylık KDV beyannamesi veren kullanıcının hazırlık süresi"*,
   *"veresiye satan esnafın tahsil edilen ile tahakkuk edeni ayırt etmesi"*).
2. **O boyut hedef kullanıcımız için önemli olmalı** — şahıs şirketi ve esnaf
   (`PRD-BusinessFinance.md`). Çok şubeli işletmeyi ilgilendiren bir üstünlük
   bu etiketi almaz, `alma` olur ve gerekçesinde neden bizim kitlemizi
   ilgilendirmediği yazılır.

İkisi sağlanmıyorsa sonuç `alma` veya `henüz karar verme`dir.

`kararı yeniden sor` bir uygulama izni de değildir: **ADR yalnız yeni bir ADR
ile değişir** ve o kararı kullanıcı verir (bkz. `AGENTS.md`). Araştırmanın işi
soruyu gerekçesiyle masaya koymaktır, kendi başına karar değiştirmek değil.

### Gözlem formlarında dil kuralı

Gözlem formları (`gozlemler/*.md`) ve Belge 1–2 **rakibi anlatır, puanlamaz.**
Bir yaklaşım tarif edilirken şu üç şey yazılır:

1. Rakip ne yapıyor (gözlem),
2. Bu yaklaşım **ne kazandırıyor** (hangi kullanıcı işini çözüyor),
3. **Ne kaybettiriyor** (hangi riski veya maliyeti doğuruyor).

Yasak kalıplar: *"tam da kaçındığımız şey"*, *"bizim tezimizin gerekçesi"*,
*"X bunu çözmemiş, biz zaten çözüyoruz"*, *"bizim kararımızın neden doğru
olduğunun kanıtı"*, *"daha doğru bir model"*. Bunlar gözlem değil, hüküm.
Rakibin bir şeyi farklı yapması **bizim haklı olduğumuzun kanıtı değildir** —
aynı ihtiyacın başka bir çözümüdür ve o çözümün de bir gerekçesi vardır.

Doğru biçim: *"KolayBi KDV'yi hesaplıyor; bu, aylık KDV beyannamesi veren
kullanıcının hazırlık işini üstleniyor. Karşılığında oran listesini üründe
güncel tutma sorumluluğu doğuyor ve yanlış hesap kullanıcıyı yanıltır.
Bizim ADR 0016 kararımız bu sorumluluğu almamayı seçti; **ödünleşim Belge
3'te tartışılır.**"*

## Yapay zekâ ve manuel test iş bölümü

### Yapay zekânın yapacağı

- Resmî ürün ve yardım sayfalarından masa başı araştırma; yardım merkezi
  adım adım makalelerinden akış ve **sistem işleyişi / pipeline** çıkarımı
- Emülatörü sürme (ham adb), ekran görüntüsü ve not alma
- Ekran görüntülerinin ortak başlıklarla incelenmesi
- Manuel ve kullanıcı video notlarının tablolara ve akış şemalarına dönüştürülmesi
- Uygulamalar arası benzerlik, fark ve çelişkilerin bulunması
- Tüm gözlem formu ve üç nihai belgenin taslağının yazılması

### Kullanıcının yapacağı

- İçine girilemeyen uygulamaların **tanıtım/eğitim videolarını izleyip** ekran
  görüntüsü + kısa not vermek (YouTube transkripti/kareleri araçla çekilemiyor)
- Manuel testte kayıt / SMS / e-posta / VKN / ödeme kapılarını geçmek
- Emülatörü canlı izleyip "şunu da dene" yönlendirmesi yapmak
- Yapay zekânın yorumlarını ve BusinessFinance için önerilen kararları onaylamak

## Kanıt etiketleri — tek kaynak

**Bu bölüm kanıt etiketlerinin tek tanım yeridir.** `DURUM.md`,
`MANUEL-TEST-PROTOKOLU.md` ve `UYGULAMA-GOZLEM-SABLONU.md` buraya işaret eder,
listeyi kopyalamaz.

Her bulgu aşağıdakilerden birini taşır:

| Etiket | Ne demek |
|---|---|
| **Manuel gözlem** | Uygulamada sentetik veriyle **davranış** doğrulandı |
| **Resmî kaynak** | Ürünün kendi sitesi, destek merkezi, yardım makalesi veya resmî videosunda anlatıldı. Alt tür parantezle yazılır: `Resmî kaynak (destek dokümanı)`, `(yardım merkezi)`, `(tanıtım videosu)`, `(video transkript)` |
| **Çıkarım** | Gözlem veya kaynaktan **türetilen** ifade — ekranda alan/etiket görüldü ama davranış görülmedi. Satır içinde `` `çıkarım` `` olarak işaretlenir |
| **Doğrulanamadı** | Erişim, ücretli plan veya bölge sınırı nedeniyle kanıtlanamadı |

`Çıkarım` gözlemlenmiş ürün davranışı gibi yazılmaz. Resmî mockup'tan okunan
**alan adı** gözlemdir; o alanın **ne yaptığı** çıkarımdır.

*(Not: 12 Eyl 2026 öncesi formlarda bu etiketin adı `Yorum`du; `Çıkarım`
kullanımda zaten yerleşmişti, tanım ona göre güncellendi.)*

## Uygulama sırası

Yedi rakip uygulama emülatöre kuruldu (adb ile doğrulandı, 1 Eylül 2026; yeni
PC'de Pixel_8 AVD ile 9 Eylül 2026 yeniden kuruldu). Pilot **Money Manager
(Realbyte)** ile yapıldı — şirket/VKN kaydı gerektirmeden temel para
hareketlerinin hızlı kurulabilmesi nedeniyle. **Canlı sürülen beş uygulama:** Money Manager, Wallet by BudgetBakers,
Bluecoins, Hesap Defterim, Goodbudget. **Masa başı (resmî kaynak) dört
uygulama:** Paraşüt, Logo İşbaşı, KolayBi, QuickBooks — kayıt/bölge/ücret
engeli nedeniyle uygulamaya girilemedi.

Manuel test yapay zekâ ve kullanıcı tarafından **aynı anda** yürütülür: yapay
zekâ emülatörü sürer ve not alır, kayıt/doğrulama kapılarında durup kullanıcıya
sorar, kullanıcı devam ettirir. Ortak akış ve canlı durum tablosu `DURUM.md`
içindedir.

Pilotta (1 Eyl 2026, Money Manager) yalnız şu beş alan incelendi; sonraki
uygulamalarda kapsam K00–K08 + ek koşum + arayüz taramasına genişledi
(`MANUEL-TEST-PROTOKOLU.md`):

1. Ana ekran
2. Gelir ekleme
3. Gider ekleme
4. İşletme/şahsi ayrımı veya en yakın sınıflandırma davranışı
5. Aylık rapor veya nakit akışı

## İnceleme türü — dokuz uygulamanın kesinleşmiş dağılımı

- **Manuel gözlem (canlı sürüldü, 5):** Money Manager (Realbyte), Wallet by
  BudgetBakers, Bluecoins, Hesap Defterim, Goodbudget
- **Resmî kaynak (masa başı, 4):** Paraşüt, Logo İşbaşı, KolayBi, QuickBooks
  Solopreneur (manuel kareleri yalnız QuickBooks mobil onboarding'i ve QBO
  Simple Start plan ekranı) — kayıt, ödeme veya bölge kapıları. Bulgular resmî
  destek merkezi + yardım makaleleri + ürün turu + kullanıcının izlediği
  videolardan, `Resmî kaynak` etiketli
- **Tur 2 derin koşum (3):** Bluecoins ve Wallet canlı; KolayBi masa başı
  (resmî destek merkezi taraması). Bu asimetri Belge 1/2'de açıkça yazılır
- **Kullanılmadı:** Bizim Hesap, Spendee

### Erişilemeyen rakipte kaynak sırası (12 Eyl 2026'da sabitlendi)

1. **Resmî destek/yardım merkezi** — ücretsiz, kullanıcıdan emek istemez ve
   genellikle en güncel arayüzü gösterir. Sayfa metni sığ olsa bile içindeki
   ekran görüntüleri alan seviyesinde okunur.
2. **Tanıtım/eğitim videosu + transkript** — ancak destek merkezi yetmezse,
   ve bir **doğrulama** katmanı olarak ([[rakip-video-transkript-yontemi]]).
3. Video karesi ile destek görseli çelişirse **tarihi yeni olan** kazanır ve
   çelişki forma yazılır (KolayBi'de kredi kartı böyle düzeltildi).

Test yöntemi ve ortak veri seti şu iki belgede tanımlıdır:

- `MANUEL-TEST-PROTOKOLU.md`
- `SENTETIK-TEST-VERISI.md`

Her uygulama tamamlandığında `UYGULAMA-GOZLEM-SABLONU.md` kopyalanarak ayrı bir
gözlem formu oluşturulur.
