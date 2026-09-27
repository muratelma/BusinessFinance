# Belge 2 — Bölüm planı

20 Eylül 2026 · **v2** · Bölüm yazımına başlamadan önceki karar belgesi.

Bu dosya Belge 2'nin **ne anlatacağını** belirler: konu blokları, her bloğun alt soruları, her
sorunun hangi ürün ve kanıtla cevaplanacağı, ve her bloğun sınırı. Biçim kararları §6'da, kanıt
durumu §7'de, kararların tamamı ve gerekçeleri §8'dedir. **Plan karar bakımından kapalıdır:**
A1–A6'nın altısı da kapandı, bölüm yazımı başlayabilir.

**v2, dış inceleme turundan ve kullanıcının yapı kararından sonra yazıldı.** Değişenlerin listesi
§11'dedir. İnceleme kaydı: [`belge2-claude-code-plan-review.md`](belge2-claude-code-plan-review.md).

**Kaynaklar:** `0-ortak-icindekiler.md` (Belge 2'nin taslak listesi) · `../BULGU-DOGRULAMA-KAYDI.md`
(P4-tema-01…10, bu planın içerik kaynağı) · `../FAZ7-8-UYGULAMA-PLANI.md` §8 (Belge 2'nin tanımı) ·
`README.md` (üç belgenin girdi tablosu) · `belge1-bolum-plani.md` (kural mirası) ·
`belge2-nasil-olabilir/` (kurgu kararı) · `belge2-claude-code-plan-review.md` (v1 incelemesi).

---

## 0 · Belge 2 nedir, Belge 1'den nasıl ayrılır

**Ana soru** (üretim planı §8): *"Kullanıcı işi nasıl tamamlıyor ve gözlenebilir sistem sonucu ne?"*

**İki yarısı da Belge 2'nindir.** Belge 1 formun görünüşünü anlatır: alanlar, düğmeler, yerleşim,
mesajın nereye konduğu. Belge 2 işin **tamamlanma koşulunu** ve **sonucunu** anlatır: kaç adımda
bitiyor, onay gerekiyor mu, hangi kayıt oluşuyor, paraya ne oluyor, geri alınabiliyor mu.

| | Belge 1 | Belge 2 |
|---|---|---|
| Konu | Ekranın kendisi | İşin tamamlanması ve sonucu |
| Argümanın birimi | Bir ekran / bir soru | **Bir ekonomik olay** |
| Kanıtın rolü | Argüman ("ekran argümandır") | **Doğrulama** — argüman sayı ve oluşan kayıttır |
| Ürün anlatımı | Yalnız Bölüm 2 (ürün kimliği) | Her konunun **C kısmında** |

**Sınır cümlesi:** alanın adı, formun düzeni ve düğmenin yeri Belge 1'dedir. Belge 2 o alanın **ne
ürettiğini** yazar. Kısa bağlam ve gönderme serbesttir (Belge 1 planı, Ö7); alan adını tümden
yasaklamak bağımsız okumayı zorlaştırır.

### Bir satır + altı etki

Üretim planı §8 şunu yazıyor — **"uygun olan" dâhil, birebir**:

> *"Her senaryoda **uygun olan** hesap, borç, gelir/gider, rapor, zaman ve geri alma etkileri ayrı
> anlatılır."*

Altı boyut Belge 2'nin sabit sözlüğüdür. Önüne bir satır daha eklenir: olayın ürettiği **kayıt**.
İncelemenin yakaladığı eksik buydu — bazı olaylarda sonuç bir sayı değil, oluşan (veya oluşmayan)
bir nesnedir: Wallet'ta Record'suz borç kartı, Money Manager'da gelecek aylarda görünen
taksit satırları (20 Eylül K2), Bluecoins'te bekleyen bir hatırlatıcı. Görünür satır fiziksel
saklama nesnesinin kanıtı değildir (B12).

| # | Satır | Ekranda ne aranır |
|---|---|---|
| **0** | **Ne oluştu** | Hangi kayıt, nesne veya bekleyen durum doğdu; nereden tekrar bulunuyor |
| 1 | Hesap bakiyesi | Hangi hesabın/defterin tutarı değişti, ne kadar |
| 2 | Borç / alacak | Kart borcu, cari bakiye veya borç kartının kalanı ne oldu |
| 3 | Gelir/gider toplamı | Ürünün kendi gelir/gider ölçüsüne girdi mi, hangi tutarla |
| 4 | **Rapor ve diğer göstergeler** | Genel toplam/net varlık · bütçe veya zarf kalanı · rapora dahil olma · kırılım |
| 5 | Dönem ve zaman | Hangi döneme yazıldı; ekstre, kesim günü, vade sınırı var mı |
| 6 | Geri alma ve düzeltme | Silinebiliyor mu, düzeltilebiliyor mu, sonucu ne |

**Hücre dört değerden birini alır:**

| Değer | Ne demek |
|---|---|
| *(tutar veya tanım)* | Ölçüldü, kanıtı var |
| **değişmedi** | Ölçüldü ve etkisi olmadığı görüldü — bu da bir sonuçtur |
| **görülmedi** | Araştırılmadı veya kanıtı yok. Özelliğin bulunmadığı anlamına **gelmez** |
| **uygulanmaz** | Bu olay için bu boyut anlamsız. **Gerekçesi yazılır** |

"Görülmedi" ile "uygulanmaz" karıştırılmaz: birincisi bizim eksiğimiz, ikincisi olayın doğası.
Satır boş bırakılamaz; dördünden biri yazılır.

**v1'in hatası ve düzeltmesi:** v1 kaynaktaki "uygun olan" koşulunu düşürmüş, altı satırı mutlak
zorunlu yapmış ve üretimi durduran bir kapı koymuştu. Bu, dışa aktarma veya sınıflandırma gibi
olaylara zorla borç/bakiye satırı yazdırır ve "uygulanmaz" ile "araştırılmadı"yı karıştırırdı.
Yeni kural yukarıdadır; denetim kapısı da §6'da buna göre değişti.

---

## 1 · Kararlar

**K** kullanıcının verdiği kararı, **Ö** bana bırakılan yetkiyle aldığım kararı gösterir.

| # | Karar | Gerekçe |
|---|---|---|
| K1 | **Birim bir ekonomik olaydır.** Olay başlık, ürünler sütun, satırlar etki | Kurgu karşılaştırması (20 Eylül); ana soruyu ve altı boyutu karşılayan tek kurgu |
| K2 | **Belge konu konu ilerler; her konu bütün hâlinde biter.** Blok düzeni: **giriş → A (karşılaştırma) → C (işleyiş) → gerekirse ek → kapanış → sonraki konu** | Okur bir tablonun açıklamasını bulmak için ileriki bölüme gitmemeli. Belge 1'in konu konu ilerleyişiyle aynı mantık |
| K3 | **Toplu A ve toplu C yoktur.** Ayrı bir "Ürünlerin motorları" bölümü kurulmaz; C içeriği konusunun içinde kalır | K2'nin sonucu |
| K4 | **"Kanıt haritası" sayfası Belge 2'ye girmez** | Denetim dosyalarının işi; belgenin işi değil |
| K5 | **Tek tip tablo kullanılmaz;** konuya göre farklı anlatım bileşenleri kurulur (§6) | Belge 2 uygulamaların işleyişine giriyor |
| K6 | **Kanıt toplama devam eder** ve yeni kare hem A hem C kısmına girebilir | Kullanıcı kararı, 20 Eylül |
| Ö1 | **Ayrı bir "işlem zinciri şeridi" konmaz.** İşin nasıl tamamlandığı C kısmının açılışında anlatılır | K2'de C zaten A'nın hemen ardında; şerit aynı şeyi üçüncü kez anlatırdı (giriş + şerit + C) |
| Ö2 | **Kanıt anahtarı Belge 1 ile aynı beş türdür** ve yeniden tanımlanmaz | İki belge aynı envanteri kullanıyor |
| Ö3 | **Sütunlar bölüm içinde sabittir.** Bir bölümün tamamında söyleyecek şeyi olmayan ürün kısa nota iner ve o not **taramanın sonucu olduğunu** yazar | Belge 1 Ö6 ("bir ürün, kanıtı varken listeden düşmez") korunur; sessiz düşme olmaz |
| Ö4 | **Etiket ürüne değil iddiaya bağlanır.** Aynı ürünün aynı sayfasında farklı dayanaklar olabilir | İncelemenin asimetri düzeltmesi |
| Ö5 | **Her iddia kendi "bilinmiyor"unu taşır** — ama sayfa doldurmak için belirsizlik üretilmez | Ö4'ün tamamlayıcısı; v1'in "her sayfada 2–3 bilinmeyen" kuralı kalktı |
| Ö6 | **Fiziksel veri modeli hiçbir yerde uydurulmaz** (B12) | Kayıtlı sınır |
| Ö7 | **Yokluk ifadeleri incelenen sürüm, paket ve yüzeyle sınırlanır** | Tema 05 düzeltmesi; bütün bloklara yayılır |
| Ö8 | **Senaryo ayrı bölüm değildir.** Bir konunun kapanışında, o konuya ait kısa bir zincir olarak yer alır | K2'nin sonucu; v1'deki Bölüm 14 kalktı |

### Kanıt anahtarı (Belge 1 ile aynı)

| Tür | Ne demek | Belge 2'de tipik yeri |
|---|---|---|
| **Canlı kare** | Emülatörde açılan ekranın görüntüsü | Sayı satırlarının çoğu |
| **Koşum kaydı** | Gözlem formunda yazılı; karesi yok veya kare tek başına göstermiyor | Silme, düzeltme, tarama sonuçları |
| **Kaynak görseli** | Ürünün yayımladığı ekran görüntüsü (KolayBi destek sayfası) | KolayBi satırları |
| **Kaynak beyanı** | Ürünün kılavuzu, yardım merkezi veya ürün sayfası metni | Paraşüt, Logo, QuickBooks |
| **Görülmedi** | Kanıt yok. Özelliğin bulunmadığı anlamına **gelmez** | Boş hücreler |

### Ürün ve kanıt derinliği

Derinlik **olay bazındadır**, ürün bazında değil. Bir ürünün canlı incelenmiş olması her olayının
sınandığı anlamına gelmez.

| Ürün | Erişim | Belge 2'de tipik kanıt | Sınır |
|---|---|---|---|
| Money Manager | Canlı, derin | Kart/taksit/tekrar zincirleri sayıyla kanıtlı | Cari/fatura zinciri kurulmadı; bütçe kapsam dışı |
| Bluecoins | Canlı, derin | Cari, transfer, tekrar, taksit sayıyla kanıtlı | Ekstre dönemi doğrulanmadı (BC-Q06) |
| Wallet | Canlı, derin | Borç kartı, plan onayı, bütçe, hedef | Taksit yolu bulunamadı; dışa aktarma taranmadı |
| Hesap Defterim | Canlı | Defter mantığı, aktarım, ek dosya, dışa aktarma seçimi | Plan/tekrar bulunamadı; dosya içeriği karesiz |
| Goodbudget | Canlı, ücretsiz paket sınırlı | Zarf, tekrar, rapor | Kart ve transfer paywall; From New Income / Fill Each Envelope sonucu ölçüldü (GB-Q01/K3), değişmeme nedeni ve Keep Available sonucu bilinmiyor |
| KolayBi | Masa başı, destek görselleri | Cari, fatura, tekrar, rapor ailesi, çek/senet | Davranış hiç doğrulanmadı; tablolar boş |
| Paraşüt | Masa başı, kılavuz + video | Gider/ödeme ayrımı, mahsup, KDV raporu | Yalnız kaynak aktarımı |
| Logo İşbaşı | Masa başı, ürün anlatımı | Cari tahsilat, müşavir erişimi | İç akış görülmedi |
| QuickBooks Solopreneur | Yalnız yardım merkezi | Business/Personal Type, Split, Rules | Solopreneur ekranı hiç görülmedi |

---

## 2 · Belgenin yapısı

### Konu bloğunun anatomisi

Bölüm 3–11'in hepsi aynı beş parçadan kurulur:

```
1. Giriş            ¼–½ sayfa · Hangi soruyu inceliyoruz? Hangi koşumlara,
                    hangi tarihlere ve hangi ürünlere dayanıyoruz?
2. A — Karşılaştırma  Olay × etki matrisleri. Bir konuda birden çok olay varsa
                    her olay kendi matrisini alır.
3. C — İşleyiş      İlgili ürünlerin o konudaki mekanizması: işin nasıl
                    tamamlandığı, ürünün yazdığı kayıtlar, kanıtlı davranış şeması,
                    gözlendi / çıkarım / bilinmiyor ayrımı.
4. (Ek)             Konunun gerektirdiği başka bileşen: terim şeridi, dönem tablosu,
                    kaynak bloğu. A'dan önce de gelebilir, C'den sonra da.
5. Kapanış          Kanıtlanan farklar · bilinmeyenler · gerekiyorsa o konuya ait
                    kısa senaryo.
```

**Uzun konularda A ve C ikiye bölünebilir.** Kart bölümünde örneğin:
*harcama A → harcama C → taksit A → taksit C → ödeme A → ödeme C.* Ölçüt tek: **okurun
anlayabilmek için ihtiyaç duyduğu açıklama, ilgili karşılaştırmadan uzaklaşmasın.** Bu her küçük
tabloya zorunlu bir kalıp değildir.

**C, A'daki tabloyu cümleye çevirerek tekrarlamaz.** A sonuçları karşılaştırır; C o sonucu üreten
işlem zincirini ve ürünün kendine özgü davranışını açıklar.

### Bölümler

| # | Bölüm | Ağırlık | A kısmındaki olaylar | Sayfa |
|---|---|---|---:|---:|
| 1 | Okuma kılavuzu: bir satır + altı etki, kanıt düzeyleri, sözlük | hafif | — | 3 |
| 2 | Kapsam ve erişim haritası | orta | — | 3 |
| 3 | **Gelir ve gider kaydı** | orta | 5 | 5 |
| 4 | **Hesaplar ve hesaplar arası para** | orta | 3 | 5 |
| 5 | **Kart: harcama, taksit, borç, ödeme** | **ağır** | 6 | 9 |
| 6 | **Borç, cari ve tahsilat** | **ağır** | 6 | 8 |
| 7 | **Zaman: tekrar, plan, bütçe** | **ağır** | 5 | 9 |
| 8 | **Sınıflandırma** | orta | 4 | 4 |
| 9 | **Raporun neyi saydığı** | orta | 5 | 5 |
| 10 | **Veri: dışa aktarma, içe aktarma, yedek, entegrasyon** | orta | 4 | 5 |
| 11 | **Diğer modüller ve yardımcı araçlar** | hafif | 1 tablo | 3 |
| 12 | Bölümler arası bağlar ve sınırlar | orta | — | 4 |
| Ek | Kanıt eki | — | — | 2 |

**Toplam ≈ 65 sayfa.** Yukarıdaki sütunun toplamı: 3+3+5+5+9+8+9+4+5+5+3+4+2 = **65**. Bu bir
**hedef değildir**; bölüm bittiğinde sayfası kaç olursa odur. Tahmin, bölüm bittikçe gerçek ölçüye
göre güncellenir.

**v1'in aritmetik hatası:** v1 "≈40–46 sayfa" diyordu, ama kendi bölüm tahminlerinin toplamı 68–72
ediyordu. Bu tablo o hatayı düzeltiyor ve toplamı açıkça gösteriyor.

**Yazım sırası:** 5 → 6 → 7 (ağırlar önce; blok biçimi onlarda oturur) → 3 → 4 → 9 → 8 → 10 →
11 → 12 → 2 → 1 → Ek. §8 A2'de kapandı ve gerekçesi orada.

---

## 3 · Konu blokları

Aşağıdaki tablolar her bloğun **A kısmındaki olayları** ve **C kısmında hangi ürünün ne
anlatacağını** gösterir. Kanıt kimliklerinin dosya karşılığı `belge1/ortak/kanit-dizini.json`'dadır.

---

### Bölüm 3 · Gelir ve gider kaydı — orta, ~5 sayfa

**Giriş:** En sıradan kayıt kaydedildikten sonra üründe ne değişiyor? Dayanak: beş canlı ürünün
Tur 1 çekirdek koşumları (Ağustos 2026 verisi) ve Hesap Defterim'in ek koşumları.

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 3.1 | Gelir kaydı | **Canlı kare:** beşinin de ana işlem formu var — MM **E0229** · BC **E0020** · WL **E0277** · HD **E0138** · GB **E0116**. MM Ağustos İstatistik Gelir ₺25.000 (E0231); HD birleşik defterde Alındı 51.200 (E0140); BC Net Kazançlar gelir 25.000; WL Cash-flow 25.000. **20 Eylül K3 — canlı kare:** GB From New Income / Fill Each Envelope ile 1.234 girildi (E0419); zarf 0 → 1.234 (E0417, E0420), hesap 41.734 → 41.734 (E0418, E0421), işlem listesinde hesap adlı +1.234 satırı (E0422), Eylül Income 3.284 (E0423). **GB-Q01'in bu yolun sonucu sorusu kapandı.** | Önceki 2.050 raporu önceki koşumdandır (E0121/E0124); bu turda raporun ayrı önce karesi yok. Hesap bakiyesinin değişmeme nedeni, eski Initial Fill'in fonlama yolu ve Keep Available sonucu bilinmiyor. Credit ve yeni gelir yolu birbirine genellenmez |
| 3.2 | Gider kaydı | MM Ağustos Gider 2.050 = 850 + 1.200 (E0230, E0231); BC gider −2.050; WL −2.050; HD Ödendi 6.250 — **açılış ve transferi içerir, saf gider değil** (E0140); GB Spending by Envelope | HD satırı Bölüm 9'un konusunu açar |
| 3.3 | Kaydın düzeltilmesi | **Canlı kare:** HD başka deftere taşıma/kopyalama ve ad değiştirme (E0160, E0161). **Koşum kaydı:** MM, WL, GB satır/detayda doğrudan düzenleme | Düzeltmenin rapora etkisi **hiçbir üründe ölçülmedi** — 4. satır "görülmedi" |
| 3.4 | Kaydın silinmesi ve geri alınması | **Canlı kare:** HD çöp kutusu + Geri Yükle + kalıcı silme onayı (E0143, E0167, E0168); BC'de de Çöp Kutusu yüzeyi var; GB silme onayı. **Koşum kaydı:** MM ve WL'de "tek onaydan sonra kalıcı" — karesi yok | Geri **yüklemenin sonucu** yalnız Bluecoins'te kullanıcı kontrolüyle anlatıldı |
| 3.5 | Geçersiz ve sıfır tutarlı kayıt | HD boş tutarda sessiz no-op (E0162) ve ₺0 kabul (E0163); MM "Lütfen hesabı seçiniz." (E0232); WL sıfır tutarda snackbar; GB "Select an Envelope." | Belge 1 mesajın nereye konduğunu, Belge 2 **kaydın oluşup oluşmadığını** yazar |

**C — İşleyiş.** Dört ürün: **Hesap Defterim** (tek nesne defter satırı; Alındı → +, Ödendi → −;
yürüyen Denge; formdaki pipeline bölümünden) · **Goodbudget** (iki katman: hesap bakiyesi + zarf
kalanı; Initial Envelope Fill; formdaki pipeline bölümünden) · **Money Manager** ve **Bluecoins**
(davranıştan türetilen kısa model: kayıt → hesap defteri satırı → dönem toplamı).

**Kapanış:** Beş üründe kaydın tamamlanma koşulu aynı mı; hangi üründe kaydın sonucu sessiz.
Bilinmeyen: düzeltmenin rapora etkisi, Goodbudget'ın normal gelir yolu.

**Bu bölüme girmez:** form alanları ve düzeni → Belge 1 §4 · transfer → 4 · kart harcaması → 5 ·
raporun toplam tanımı → 9.

---

### Bölüm 4 · Hesaplar ve hesaplar arası para — orta, ~5 sayfa

**Giriş:** Para nereye konuyor, hesaplar arasında geçince hangi toplamlar değişiyor? Dayanak:
Tur 1 çekirdek koşumları ve Money Manager Faz 1 açılış bakiyesi koşumu (10 Eylül).

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 4.1 | Açılış bakiyesi | **Dört farklı davranış.** MM: "Bakiye Farkı" hareketi, bugünün tarihiyle; "İşlemler bölümünde gösterilsin mi?" sorusunda **HAYIR seçilen koşumda** ana akışa ve gelir raporuna girmiyor (E0239, E0240). BC: formda Başlangıç bakiyesi + Açılış tarihi. WL: cash/checking'de açılış alanı yok — **koşum notu, karesiz**. HD: tarihli "Açılış bilançosu"; dönem kapsıyorsa **Toplam Alındı'ya giriyor** (E0137), kapsamıyorsa ayrı "Önceki denge" satırı (E0152) | MM'in HAYIR koşulu mutlaka yazılır. BC'de ay öncesine açılış girilememesinin nedeni ölçülmedi (BC-Q07) |
| 4.2 | Hesaplar arası transfer | MM ayrı Havale segmenti, tek nötr satır, **işlem listesinde** gün başlığı ₺0,00 (E0230) · BC TRANSFER, iki bağlı bacak, satır başına işlem sonrası bakiye · WL iki satır (+/−), ikisi de "Transfer, withdraw"; **işlem listesinde** hafta toplamı TRY 0 (E0279) · HD Aktar, iki defter satırı ve **her ikisi de Toplam Alındı/Ödendi'ye giriyor** (E0137, E0153, E0154) · GB ayrı Account Transfer ekranı; **tek hesap nedeniyle gerçek transfer yapılamadı** | **Yöntem uyarısı:** liste başlığının sıfır olması, gelir ve gider toplamlarının ayrı ayrı değişmediğini kanıtlamaz. İki karşıt hareket neti sıfır tutarken brüt toplamları artırabilir — Hesap Defterim tam olarak bunu yapıyor. Üç üründe 3. satır "**görülmedi**", ayrı rapor kanıtı gerekir |
| 4.3 | Hesabı toplamdan çıkarma | MM "Toplama Dahil Et" kapalıyken Ortak Cuzdan 4.150 gri, net varlık 38.200, nakit grubu 0 (E0253) · BC nakit akışı raporuna katılım anahtarı | **İki farklı ölçü**, eşdeğer sayılmaz. MM'in açık hâli ayrı kare değil, aritmetiktir; bu yazılır. Bölüm 9'da tekrar edilmez, oradan buraya gönderme yapılır |

**C — İşleyiş.** İki ürün derin: **Hesap Defterim** (aktarım iki deftere birer satır; tek kayıt mı
iki kayıt mı bilinmiyor — B12; bir bacağın silinmesi koşum kaydı) · **Bluecoins** (iki bağlı bacak,
satır başına işlem sonrası bakiye). MM ve WL kısa: transferin listedeki temsili ve nötr davranışı.

**Kapanış:** Aktarım üç üründe gelir/giderden ayrı görünüyor, Hesap Defterim'de toplamlara karışıyor.
Bilinmeyen: üç üründe transferin rapor toplamına etkisi ölçülmedi; aktarımın saklama yapısı.

**Bu bölüme girmez:** hesap listesinin sunumu → Belge 1 §3 · kart hesabı → 5 · cari hesap → 6 ·
raporun toplam tanımı → 9.

---

### Bölüm 5 · Kart: harcama, taksit, borç, ödeme — **ağır**, ~9 sayfa

**Giriş:** Kartla harcanan para ne zaman gider oluyor, borç nasıl birikiyor, ödendiğinde ne
değişiyor? Dayanak: Money Manager Faz 1 (10 Eylül) ve Faz 7.5 (12 Eylül) koşumları, Bluecoins
P2-G02, Wallet Faz 7, Hesap Defterim Ek koşum A. **Tutarlar ve tarihler ürünler arasında farklı
(₺400/₺500, 10–11 Eylül); satırlar farkı karşılaştırır, tutarı değil.**

Bu bölüm A ve C'yi üç kez dönüşümlü kurar: **harcama → taksit → ödeme.**

**A1 — Kart harcaması**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 5.1 | Kart harcaması | MM: 08 Ağu Mavi Yazilim 1.200 kart defterinde (E0255) **ve** Ağustos Gider 2.050'nin içinde (E0230, E0231) — harcandığı gün gider · BC kart gideri formunda taksit alanı · WL 6.000 tek kart harcaması; kart −6.000, Eylül giderine tam tutar · HD kart = eksiye giden defter · GB kart hesabı açılamadı, kart gideri sıradan hesaba yazıldı | **Bölümün en önemli tek satırı:** harcamanın gidere ne zaman girdiği |

**C1 — İşleyiş:** Money Manager'ın kart defteri (Para Yatırma / Çekme sütunları, satır başına
yürüyen bakiye, başlıkta "Faturalama donemi"); Wallet'ın dönemsiz negatif bakiyesi.

**A2 — Taksit**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 5.2 | Taksitli kart harcaması | MM 6.000/6: başlıkta (1/6), Ağustos listesine **yalnız 1.000 düştü** (E0246); kartta Bu Ay 1.000 + Gelecek Ay 1.000, **incelenen Bu Ay/Gelecek Ay toplamlarında sonraki dört taksit görünmüyor** (E0247); Eylül defterinde (2/6) (E0256). **20 Eylül koşumu (MM-Q03 kapandı):** kalan taksitler gelecek ayların işlem listesinde görünür satır ve o ayın gider toplamında; arkada ayrı plan nesnesi olup olmadığı bilinmiyor (B12) (3/6 Eki, 4/6 Kas, 5/6 Ara, 6/6 Oca 2027; Şub işlem toplamı sıfır, Tekrarlama önizlemesi sürüyor — E0407–E0411). Kart defterinin yürüyen bakiyesi geleceğe uzuyor: Eyl 1.600 → Eki 2.600 → Oca 5.600 (E0412, E0413) · BC 6 ay, oran 0,00, ilk 1/6 gerçek kayıt ve Ağustos giderinde, 2/6–6/6 beş bekleyen hatırlatıcı · WL taksit alanı incelenen yollarda bulunmadı · GB Split zarflara dağıtır, zamana yaymaz · HD taksit planı kurulamadı | **Sayı uyarısı:** MM'de Ağustos gideri 2.050 → 3.650 yükseldi; bu **1.000 taksit + 600 abonelik** birleşik sonucudur (gözlem formu). Yalnız taksidin etkisini kanıtlayan temiz bir önce/sonra çifti yoktur. BC'de ilk kaydın geçmiş tarihten mi taksit kuralından mı oluştuğu ayrıştırılmadı |

**C2 — İşleyiş:** Money Manager'da taksitin ay ay ekstreye düşmesi; Bluecoins'te ilk kayıt +
beş bekleyen hatırlatıcı modeli.

**A3 — Borç ve ödeme**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 5.3 | Kart borcunun kısmen ödenmesi | MM 400 → Bu Ay 1.000→600, Gelecek Ay 1.000 değişmedi, Toplam 41.750 değişmedi (E0247, E0249) · BC 500 → Ana Hesap 40.200→39.700, kart −1.000→−500 (E0049, E0050) · WL 400 → 15.200→14.800, −6.000→−5.600 (E0321, E0334) · HD 400 → Denge −1.000→−600 **ve** Ana Hesap Ödendi 4.200→4.600 (E0137, E0153, E0154) | 3. satır: **MM kapandı (20 Eylül koşumu):** Eylül Gider ₺1.600; Toplam sekmesinde `Gider (Kredi Kartı, Ödeme) ₺1.000,00(₺400,00)` ve `Havale ₺0,00` — ödeme gider toplamına girmiyor, yalnız kart satırının parantezinde "o dönem ödenen" olarak görünüyor (E0403, E0405). BC ve WL için elde yalnız **işlem listesi** kanıtı var (E0022, E0279) ve **bunlar aynı türden daha eski 1.200'lük bir ödemeye ait** — o iki hücre "görülmedi" kalır. HD'de iki toplama birden giriyor |
| 5.4 | Kart borcunun tamamının ödenmesi | **Görülmedi — dört üründe de** | Dönem ve ekstre etkisi bu olayla açılırdı; §7'de "kısıt yüzünden yapılamayan" listesinde |
| 5.5 | Kart dönemi ve ekstre | MM Bu Ay/Gelecek Ay + "Faturalama donemi" başlığı (E0255, E0256); Kaynak, Hesap Kesim Tarihi, Son Ödeme Tarihi alanları · BC Kredi Limiti, Hesap Kesim Günü, Bitiş tarihi — **dönem davranışı doğrulanmadı** (BC-Q06) · WL: gözlenen bakiye biçimi dönemsiz negatif bakiye; tek Payment Due Date alanı ve Not set (E0298); eşik uyarısı; **incelenen yüzeylerde ekstre görülmedi** · HD: **incelenen yüzeylerde kart dönemi, limit veya son ödeme alanı görülmedi** | Dört ayrı durum; "yok" değil "görülmedi" |
| 5.6 | Kaynakta anlatılan kart modeli | **Kaynak görseli:** KolayBi Kredi Kartları listesi ve formu — Hesap Kesim Günü, Son Ödeme Günü, Kart Limiti, Minimum Ödeme Oranı (%), Kalan Limit kolonu (E0207, E0208). Tablo boş; Kalan Limit hesabı **çıkarım** | Paraşüt, Logo, QuickBooks formlarında kart modeli anlatılmıyor — doğrulanmadı |

**C3 — İşleyiş:** Money Manager'ın ödeme düğmesi → ön doldurulmuş Havale → iki deftere birer satır
zinciri (E0237, E0248); Bluecoins ve Wallet'ta ödemenin ayrı bir akış olmaması; Hesap Defterim'de
aynı hareketin iki toplamı birden büyütmesi.

**Kapanış:** Dört ürün, dört kart modeli. Kanıtlanan fark: yalnız MM'de dönem ayrımı ve karta özel
ödeme yüzeyi. Bilinmeyenler: tam ödeme hiç denenmedi; üç üründe kısmi ödemenin rapor etkisi;
BC'nin ekstre davranışı; MM'de taksitlerin saklama biçimi ve üretim eşiği (B12).
MM-Q03'ün görünür yer sorusu 20 Eylül K2 ile kapandı. **Kısa senaryo:** MM'de taksitli
ekipmandan kısmi ödemeye uzanan zincir (§4'teki sınıflandırmayla birlikte).

**Bu bölüme girmez:** kart hesabının ekrandaki sunumu → Belge 1 §3 · kartın tekrarlayan planla
ilişkisi → 7 · kart harcamasının rapordaki etiketi → 9.

---

### Bölüm 6 · Borç, cari ve tahsilat — **ağır**, ~8 sayfa

**Giriş:** Bir alacak doğduğunda ve sonra tahsil edildiğinde para ile gelir/gider ayrı ayrı ne
yapıyor? Dayanak: Bluecoins P2-G03 D2/D3, Wallet Faz 7 D2/D3 ve WL-U01, KolayBi/Paraşüt/Logo
kaynakları. **İncelenen zincirler ağırlıkla alacak yönündedir** (bize borçlu olan); borçlanma ve
ödeme yönü aynı ölçüde incelenmedi ve sonuç ters yöne taşınmaz.

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 6.1 | Alacağın doğması | **BC:** 12.000 hizmet, GELİR türünde Ada Reklam cari hesabına; cari 0 → 12.000, banka 29.700 **değişmedi** · **WL:** I Lent türünde, Record oluşturmadan ayrı borç kartı; OWES ME 12.000; bakiye, Records ve arama etkisi yok | **0. satır burada belirleyici:** BC'de bir cari hareketi, WL'de Record'suz bir borç kartı oluştu. İkisi de e-fatura üretimi değildir |
| 6.2 | Kısmi tahsilat | **BC:** cari → Ana Hesap TRANSFER; cari 12.000 → 7.000, banka 29.700 → 34.700; banka+cari 41.700 ve gün neti 2.000 korunuyor · **WL:** aynı Debt kartında Add Record → Repay debt; borç 12.000 → 7.000, banka 4.800 → 9.800 | Bluecoins'in banka+cari toplamı kasadaki para diye sunulmaz |
| 6.3 | "Kalan" neyin kalanı | BC'de **cari hesap bakiyesi**; belirli faturaya tahsis gösterilmedi · WL'de **seçilen borç kartının kalanı**; aynı adlı iki borç ayrı kart | BC'de fatura seçim alanının görünmemesi ürün genelinde bu bağın yokluğunu kanıtlamaz |
| 6.4 | Borç ve tahsilatın gelir/gidere etkisi | BC: 12.000 gelir listede; 5.000 transfer iki nötr bacak; D3 sonrası bütün hesaplar net raporu **yok** · WL: WL-U01'de önceki 5.000 borç verme **gider**, 5.000 tahsilat **gelir**; Record'suz 12.000 incelenen dönemde gelirde yok. Cash-flow son 30 gün: gelir 5.000, gider 21.600, net −16.600 | ADR 0014 eşdeğerlik iddiası **reddedildi** (B09); burada tekrar edilmez, davranış betimlenir. Çekirdek Ağustos 22.950 netiyle karşılaştırıp rapor çelişkisi üretilmez |
| 6.5 | Yaşam döngüsünün kapanışı | **Görülmedi:** tam kapanış, iptal, borcun silinmesi ve geri yüklenmesi hiçbir üründe denenmedi | Kısa kapsam kaydı olarak yazılır; ayrı bölüm açılmaz |
| 6.6 | Kaynakta anlatılan fatura → tahsilat zinciri | **KolayBi:** cari tipi birleşik; Borç/Alacak Ekle, Fatura Ekle, Ödeme/Tahsilat Ekle **ayrı eylemler**; hareketlerde borç/alacak, yürüyen bakiye, vade; açılış bloğu; Mahsuplaştır menüsü; ekstre + PDF önizleme · **Paraşüt:** gider/fatura önce ödeme sonra; kısmi ödeme, avans, en gecikmiş açık faturadan otomatik eşleme · **Logo:** müşteri seçerken bakiye, cari içinde nakit/banka tahsilatı · **QuickBooks:** müşteri → kalem → ödeme yöntemi → gönderim ve hatırlatma | Dördü de **yalnız kaynak**. Mahsup algoritması denenmedi; butonlar model eşdeğerliği kanıtı değildir; fatura gönderebilmek kapsamlı A/P–A/R modülüyle aynı şey değildir |

**C — İşleyiş.** İki ürün derin: **Bluecoins** (cari bir hesap grubudur; borçlandırma gelir kaydı,
tahsilat transfer; tahsis bağı gözlenmedi) · **Wallet** (borç kartı Records'tan ayrı bir nesne;
Record oluşturma tercihi bakiye hareketini belirliyor). **Hesap Defterim, Goodbudget, Money
Manager** için kısa kapsam kaydı: incelenen yüzeylerde ayrı cari/fatura kapanışı gözlenmedi —
HD'de iş ayrı bir uygulamaya ayrılmış (E0177, o ürün test edilmedi), GB'de Debt hesap grubu var
ama içeriği denenmedi (B07), MM'de bu tema için zincir kurulmadı. **T7 kaynak bloğu:** KolayBi,
Paraşüt, Logo, QuickBooks.

**Kapanış:** Kanıtlanan fark — iki üründe alacağın doğması ile tahsilatı ayrı davranıyor, ikisinde
de "kalan" farklı bir şeyin kalanı. Bilinmeyenler: borç yönü, tam kapanış, tahsis bağı, kaynakta
anlatılan mahsup.

**Bu bölüme girmez:** cari listesinin sunumu → Belge 1 §3–4 · **cari ekstre dosyası, kolonları ve
teslimi → 10** (buradan gönderme yapılır) · e-belge entegrasyonu → 10 · BF kıyası → Belge 3.

---

### Bölüm 7 · Zaman: tekrar, plan, bütçe — **ağır**, ~9 sayfa

**Giriş:** Gelecekteki bir ödeme tanımlandığında ne oluşuyor; kayıt ne zaman ve kimin onayıyla
gerçek oluyor? Dayanak: MM Faz 1 B1/B2, BC P2-G02, WL Faz 7 B1/B2 ve bütçe/hedef, GB B1/B2,
HD negatif taraması, KolayBi kaynakları.

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 7.1 | Tekrarlayan planın kurulması | MM aylık form rozeti (E0241) · BC tekrar sıklığı, bitiş seçenekleri, **otomatik giriş kutusu** · WL Planned payments → Recurrent payment; planlı ödemede geçmiş gün kısıtı · GB "Schedule this…", sıklık listesi, e-posta hatırlatma · HD: **beş yüzey tarandı** (form, form menüsü, ana ekran menüsü, çekmecenin 17 kalemi, Ayarlar'ın 21 kalemi), plan/tekrar bulunamadı · KolayBi gider/alış/satış/maaş için Tekrarlı yüzeyler | HD satırı bir **tarama sonucudur** ve öyle yazılır. MM'de kurulum onayı ve Tekrar/Taksit menüsü karesiz |
| 7.2 | Planın gerçekleşmesi | MM: 10 Eylül koşumunda geçmiş 10 Ağustos ve o günkü 10 Eylül örnekleri **gerçek işlem**; 10 Ekim ayrı önizlemede, ay toplamları sıfır (E0242–E0244) · BC: otomatik **kapalı** koşumda geçmiş örnek "31 gün gecikmeli", aynı günkü "Bugün süresi doluyor"; onay sonrası Ağustos gideri 600 arttı · WL: Confirm → Payment summary; ilk 600 onayından sonra Ana Hesap 20.800 → 20.200; ardından otomatik/onaylı sorusu, Yes ön seçili · GB: sonraki üretim zamanı ve onayı **bilinmiyor** (B06) | **Üç ayrı davranış karıştırılmaz:** otomatik üretim (MM koşumu), kapalı otomatikle gecikme etiketi (BC), açık onay (WL). WL'in "her örnek bekler" ifadesi **No seçilen koşumla** sınırlıdır |
| 7.3 | Bekleyen görünüm ve durum | BC ortak bekleyen liste: kira, abonelik ve taksitler tarih sıralı; gecikme/bugün/yarın etiketleri · WL menüde Postpone / Dismiss, Records menüsünde Show planned payments · KolayBi "Günü Gelen İşlemler" | BC'nin tek listesi **ortak domain veya fiziksel şema kanıtı değildir**. WL'de erteleme/atlamanın sonucu denenmedi |
| 7.4 | Bütçe ve hedef | WL aylık 5.000 bütçe; 6.600 harcama, −1.600 kalan, aşım geri bildirimi; Spent/Remains, günlük ortalama, Forecasted Spend · WL hedef: Target amount, Saved already, Desired date · GB zarf satırında kalan/bütçelenen · MM bütçe **kullanıcı kararıyla kapsam dışı** · HD: incelenen yüzeylerde finansal bütçe bulunamadı | **4. satırın asıl yeri burası:** bütçe/zarf kalanı genel toplamdan ayrı bir göstergedir. WL tahmin yöntemi bilinmiyor |
| 7.5 | Tahmin ve dönem sonu | KolayBi Nakit Akış Raporu: güncel bakiye, tahsilatlar, ödemeler, **tahmini dönem sonu**; demo 19.543,53 + 143.252,50 = 162.796,03; tahsilatların **tümü Belirsiz kovasında**, on iki ay kovası boş | Aritmetik uyum genel algoritmayı ve ay ay tahmini kanıtlamaz |

**C — İşleyiş.** Üç ürün derin: **Money Manager** (geçmiş ve bugünkü örnekler gerçek işlem, gelecek
ayrı önizleme) · **Bluecoins** (hatırlatıcı nesnesi + onayla kayda dönüşme) · **Wallet** (plan →
örnek → Confirm → Payment summary → sonraki örnek bekliyor). **Goodbudget** kısa: ilk kayıt normal
listede, sonrası bilinmiyor. **T7:** KolayBi.

Paraşüt, Logo ve QuickBooks için karşılaştırılabilir kurulum–gerçekleşme zinciri bulunmuyor; bu,
ürünlerinde özellik yokluğu anlamına gelmez.

**Kapanış:** Kanıtlanan fark — planın gerçekleşmesi üç üründe üç ayrı kapıdan geçiyor. Bilinmeyen:
otomatik kolun sonucu (BC), erteleme/atlama (WL), sonraki üretim (GB), tahmin formülü (WL, KolayBi).
**Kısa senaryo:** Wallet'ta plan → onay → bakiye değişimi → sonraki örnek.

**Bu bölüme girmez:** plan formunun alanları → Belge 1 §4 · taksitin kart borcuna etkisi → 5 ·
bekleyen listesinin görsel düzeni → Belge 1 §3.

---

### Bölüm 8 · Sınıflandırma — orta, ~4 sayfa

**Giriş:** Bir kaydın işletmeye mi şahsa mı ait olduğu üründe nasıl ve nerede söyleniyor?
Dayanak: dokuz formun K04 satırları, QuickBooks yardım merkezi, KolayBi proje/cari bölümleri.

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 8.1 | Kayda kapsam yazmak | **Yalnız QuickBooks Solopreneur, yalnız kaynak beyanı:** işlem başına tek `Type` (Business / Personal), üçüncü değer yok; `Type` ve `Category` **ayrı sütun**; şahsi işaretlenen işlem işletme/vergi raporundan düşer, silinmez. **Canlı incelenen beş üründe ve üç Türk ön muhasebe ürününde kayıt düzeyinde kapsam alanı görülmedi** | Ürünün ABD vergi formuna hizalı olduğu yazılır. Solopreneur'ün hiçbir ekranı görülmedi |
| 8.2 | Yakın araçlar | KolayBi kullanıcı tanımlı **Proje** ekseni + Etiketler, Şube, **Ortaklar/Personel Carileri** · BC etiket listesinde "İş" ve "Kişisel" **adlı değerler var** · WL Labels · HD tek dolaylı yol **ayrı defter** · GB zarflar · **MM:** işlem formu ve kategori ızgarasında (E0229) kapsam alanı, işletme kategorisi veya etiket gözlenmedi | **B08 sınırı zorunlu:** KolayBi demo proje adları (Ev Elektrik, Bebek Bakım) hane ihtiyacı kanıtı **değildir**. BC etiketlerinin kapsam boyutu olarak kullanıldığı **çıkarılmaz** |
| 8.3 | Bir kaydı ikiye bölmek | QuickBooks **Split transaction** (her parçaya ayrı Business/Personal ve kategori; araç/yakıt istisnası) — kaynak beyanı · GB Split into multiple Envelopes — zarflara dağıtır · WL Split — kaydedilmedi · BC "Bölmek" alanı formda | Dört üründe üç ayrı anlam: kapsam bölme, zarf bölme, kayıt bölme. Aynı ada bakıp aynı şey denmez |
| 8.4 | Otomatik sınıflandırma | QuickBooks `Rules` + `Exclude` — kaynak beyanı · KolayBi iki seviyeli gider tipi | Yalnız kaynak; davranış görülmedi |

**C — İşleyiş.** Bu konuda anlatılacak canlı mekanizma yok; C kısmı kısadır ve **T7 kaynak bloğu**
ağırlıklıdır (QuickBooks'un Type → rapor ilişkisi, KolayBi'nin proje → Gelir/Gider/Net listesi).

**Kapanış:** Kanıtlanan: kayıt düzeyinde kapsam yalnız bir ürünün yardım merkezinde anlatılıyor.
Bilinmeyen: yakın araçların gerçekte bu amaçla kullanılıp kullanılmadığı (B08).

**Bu bölüme girmez:** alanın formdaki yeri → Belge 1 §4 · kapsamın raporu bölmesi → 9 ·
ADR 0013 kıyası → Belge 3.

---

### Bölüm 9 · Raporun neyi saydığı — orta, ~5 sayfa

**Giriş:** Ekrandaki "gelir", "gider" ve "net" hangi kayıtları içeriyor, hangi dönemi ve hangi
hesapları kapsıyor? Bu bölüm **ölçünün tanımını** verir; tek tek olayların rapor etkisi kendi
bloklarındadır.

**A — Karşılaştırma**

| # | Soru | Ürün ve kanıt | Not |
|---|---|---|---|
| 9.1 | Gelir/gider toplamı neyi sayıyor | MM Toplam sekmesinde **nakit-banka gideri ve kart harcaması ayrı**: Ağustos 3.050 = 850 + 2.200; kart satırındaki (1.200) ödeme, **dönem sonu borcu değil** (E0250) · BC Net Kazançlar 25.000 / −2.050 / 22.950 · WL Cash-flow aynı üç sayı · HD Alındı 51.200 / Ödendi 6.250 / Denge 44.950 — **açılış ve transferi içerir** (E0140, E0142) · GB Spending by Envelope, Income vs Spending | MM'de kart dönem sonu borcu 1.000; **2.200 tutarı borç diye etiketlenmez** |
| 9.2 | Dönem ve filtre | MM ay seçimi + GELİR/GİDER/HESAP sekmeleri (E0251) · BC tutar aralığı, tarih, tür, kategori, hesap, etiket, durum, metin · WL Records hafta grupları; dönem başlangıç günü ayarı · HD Herşey/Günlük/Haftalık/Aylık/Yıllık + ay/hafta/yıl başlangıcı (E0178); **arama özeti filtrelenen kayıtları anlatır, tüm defteri değil** (E0164) · GB açılışta güncel ay | MM'in Türkçe havale başlıkları veri yönüyle ters; **yalnız Türkçeye özgü olduğu çıkarımdır**. WL'de filtre kurmanın rapora etkisi denenmedi |
| 9.3 | Hangi hesaplar dahil | **4.3'ten gönderme.** Burada yalnız ölçü farkı yazılır: MM net varlık anahtarı ile BC nakit akışı katılım anahtarı **aynı şeyi değiştirmiyor** | Sayılar ve kareler 4.3'te; burada tekrarlanmaz |
| 9.4 | Rapor ailesi — kaynaktan | KolayBi üst barda **on rapor**; proje özeti; alış/satış kırılımları ve KDV Dahil anahtarı · Paraşüt videoda Gelir/Gider ve Kasa/Banka ayrı · Logo tarih aralığı ve kategorize raporlar · QuickBooks üç aylık vergi tahmini | **Menünün görülmesi on raporun hesaplamasının denetlendiği anlamına gelmez.** Paraşüt videosu tanıtım kurgusudur |
| 9.5 | KDV ve vergi özeti | KolayBi matrah/tutar matrisi ve "Hesaplanan KDV Toplam" satırı; alt toplam ve İndirilecek etiketi **karede görünmüyor**; 12.000 ↔ 2.400 aritmetiği **oran üzerinden hesap yapıldığını kanıtlamaz** · Paraşüt kılavuzunda aylık Hesaplanan/İndirilecek/Net KDV | Görülen tarihsel oran listesi güncel mevzuat değildir; beyanname üretildiğinin kanıtı sayılmaz |

**C — İşleyiş.** İki ürün: **Hesap Defterim** ("rapor" ekranı ayrı bir hesaplama değil, dönem
filtresi uygulanmış aynı liste — E0142) · **Money Manager** (Toplam sekmesinin nakit/kart ayrımı).
**T7:** KolayBi, Paraşüt, Logo, QuickBooks.

**Kapanış:** Kanıtlanan: hiçbir iki ürün "gider" sözcüğüyle aynı kümeyi saymıyor. Bilinmeyen:
filtrelerin rapora etkisi (WL), KDV'nin türetilip türetilmediği (KolayBi).

**Bu bölüme girmez:** grafik ve tablo biçimi → Belge 1 §3–4 · dosya çıktısı → 10 · BF rapor
kıyasları → Belge 3.

---

### Bölüm 10 · Veri: dışa aktarma, içe aktarma, yedek, entegrasyon — orta, ~5 sayfa

**Giriş:** Veri üründen nasıl çıkıyor, nasıl giriyor, nereye gidiyor? **Bölümün kuralı bir kez
burada yazılır: seçenek ≠ dosya ≠ teslim.**

**A — Karşılaştırma**

| # | Olay | Ürün ve kanıt | Not |
|---|---|---|---|
| 10.1 | Dışa aktarma | HD Bildiri: dönem + PDF/EXCEL, defter başına ayrı; paylaşım yüzeyinde "2 dosya paylaşılıyor" (E0169, E0175, E0176) · BC PDF/Yazıcı / Excel(.csv) / HTML · MM "Excel(.xlsx) e-posta olarak gönder" · WL bu turda görülmedi · KolayBi cari ekstre + PDF önizleme, listelerde Dışarıya Aktar | HD'de dosya içeriği koşum notudur, karesi yok (HD-Q05). BC'nin "Excel" etiketi .csv uzantısı taşır. WL için "dışa aktaramaz" **çıkarılmaz**. **Cari ekstrenin kolonları ve teslimi burada**, Bölüm 6'dan gönderme gelir |
| 10.2 | İçe aktarma ve ek dosya | BC Excel(.csv)/QIF içe aktarma girişi · KolayBi listelerden İçe Aktar · HD Fatura ekle: Kamera/Galeri/PDF, ataç, tam ekran (E0157–E0159) · WL dosya/foto ekleme diyaloğu · KolayBi gider eki 5 MB sınırı | **Ek dosya saklamak OCR değildir**; PDF eklemek PDF'den veri almak değildir |
| 10.3 | Belgeden kayıt üretimi | **Kaynak beyanı:** Paraşüt fiş fotoğrafından OCR; Logo sesle fatura kesme. **Canlı incelenen beş üründe OCR görülmedi** | OCR sonucu, kullanıcı onayı ve hatalı okumanın düzeltme yolu çalışırken görülmedi; "otomatik kayıt" beyanı uçtan uca davranış diye yazılmaz |
| 10.4 | Yedek, veri konumu, dış servis | HD: kayıtları sunucusunda saklamadığını söylüyor (E0149); Drive yedeği denenmedi; otomatik e-posta ayarı işaretli, alıcı/tetikleyici bilinmiyor (P1-B11) · MM/BC yerel ve girişsiz · WL bulut + Bank Sync, Group sharing girişleri · GB premium banka senkronizasyonu · KolayBi banka, GİB e-belge, pazaryeri, sanal POS, API, muhasebeci erişimi · Paraşüt banka/e-belge + muhasebeci canlı görüntüleme · Logo müşavir müşteri adına işlem yapabiliyor · QuickBooks banka feed'i → Rules | "Tamamen çevrimdışı" ve "istemsiz gönderiyor" hükümleri taşınmaz. Üç ürünün müşavir yetkileri ortaklaştırılmaz; ikonlardan izin matrisi çıkarılmaz. Karedeki hesap sahibi adı teslimde karartılır (P3-G04) |

**C — İşleyiş.** Bir ürün derin: **Hesap Defterim** (dönem seçimi → dosya türü → sistem paylaşımı
zinciri; zincirin hangi halkasında kanıt bittiği). Diğerleri kısa. **T7:** KolayBi, Paraşüt, Logo,
QuickBooks.

**Kapanış:** Kanıtlanan: seçim ekranları kareli, üretilen dosyaların içeriği hiçbir üründe kareli
değil. Bilinmeyen: gerçek dosya içerikleri, gönderim, entegrasyon çağrıları.

**Bu bölüme girmez:** çıktı ekranlarının düzeni → Belge 1 §3–4 · cari bakiyenin anlamı → 6 ·
BF içe aktarma/yedek kıyasları → Belge 3.

---

### Bölüm 11 · Diğer modüller ve yardımcı araçlar — hafif, ~3 sayfa

**Giriş:** Ürünün diğer modülleri para hareketi üretiyor mu, yoksa yardımcı araç mı?
*(v1'deki "finans dışı modüller" başlığı yanlıştı: maaş, çek ve senet finans dışı değil.)*

Tek tablo: ürün × modül; hücrede "var / görülmedi / kaynak beyanı" + **para hareketi üretiyor mu**.
Kapsam: KolayBi ürün/hizmet, personel carisi, maaş, çek/senet, Notlar · Paraşüt stok/depo,
çek-senet, maaş · Logo stok/teklif/sipariş, sesli fatura · HD Not Defteri, Nakit Hesap Makinesi,
Öğe eklemek, ayrı ürün bağlantıları · WL Investments, Shopping lists, Warranties, Loyalty cards,
Currency rates · BC Seyahat Modu · MM CalcBox, PC'den Yönet · QuickBooks km takibi.

**Sınırlar:** HD Nakit Hesap Makinesi finansal hareket üretmiyor; karede para simgesi yok, TL
nitelemesi çıkarımdır. "Öğe eklemek" kayıt giriş yardımcısıdır. KolayBi personel kareleri aynı veri
anının önce/sonrası değildir. BC Seyahat Modu açılmadı. MM CalcBox ile HD kupür aracı yalnız
adından eşitlenmez.

**Bu bölüme girmez:** modüllerin arayüzü → Belge 1 §10 · BF'ye alma kararı → Belge 3.

---

### Bölüm 12 · Bölümler arası bağlar ve sınırlar — orta, ~4 sayfa

**Giriş:** Konular birbirine nasıl bağlanıyor ve nerede birbiriyle çelişiyor? **Bu bölüm yeni kanıt
veya yeni kesin hüküm üretmez;** önceki blokların sonuçlarını bağlar.

| # | Alt soru | İçerik |
|---|---|---|
| 12.1 | Aynı para kaç kez sayılıyor | HD'de aktarım Alındı ve Ödendi toplamlarını artırırken Denge'yi değiştirmiyor; bu toplamlar saf gelir/gider ölçüsüyle aynı içeriği taşımıyor. MM için K1, Eylül kısmi kart ödemesinin gider toplamına girmediğini doğruladı (5.3); bu sonuç bütün transferlere genellenmez. BC/WL kısmi ödeme koşumlarının doğrudan rapor etkisi ölçülmedi |
| 12.2 | Tanıma ile ödeme ayrı mı | BC cari (banka değişmeden gelir) · WL Record'lu/Record'suz borç · KolayBi/Paraşüt kaynakta ayrı eylemler · **MM ve HD'nin incelenen akışlarında ayrı bir doğum–ödeme zinciri kurulmadı**. ADR 0014 adı geçmez |
| 12.3 | Görünen borç gerçek taahhüt mü | MM'de **incelenen Bu Ay/Gelecek Ay toplamlarında** sonraki dört taksit görünmüyor; 20 Eylül K2'de bunlar Ekim–Ocak işlem listelerinde ve aylık giderlerde görüldü (5.2). Hesaplar Borçlar 1.600 ile Ocak defteri Bakiye 5.600 farklı dönem kapsamlarını gösterir; BC'de beş bekleyen hatırlatıcı; WL'de tam tutar tek seferde |
| 12.4 | Dönem kimin | Kullanıcı ayarlı dönem (HD, WL), ürün ayarlı ekstre (MM), alanları var davranışı bilinmeyen (BC) |
| 12.5 | Ürünlerin kendi içinde çözülmemiş yerleri | MM Türkçe havale başlıkları ters · GB açıklanamayan 600 farkı ve K3 gelirinde hesap bakiyesinin değişmemesi (3.1) · BC silme ile çöp kutusu arasındaki belirsizlik · WL'in iki ayrı borç değerlendirmesi. **Ürün kusuru listesi değildir**; gözlenen tutarsızlıkların kaydıdır |
| 12.6 | Bu araştırmanın göremedikleri | Fiziksel şema (B12) · gerçek dosya içerikleri · gönderim ve entegrasyon çağrıları · ücretli paketler · denenmeyen dallar · borç yönü · tam kapanış |

---

## 4 · (Kaldırıldı — Kısım II artık yok)

v1'deki ayrı "Ürünlerin motorları" bölümü **kalktı**. İçeriği konu bloklarının C kısımlarına
dağıldı (K2, K3). Ürünün bütünsel görünümü §8 A6'da karara bağlandı: basılabilir altı ürün bu
bölümde sabit beş satırla özetlenir.

---

## 5 · (Kaldırıldı — Senaryolar ayrı bölüm değil)

v1'deki Bölüm 14 **kalktı**. Senaryolar konu kapanışlarında, o konuya ait kısa zincirler olarak yer
alır (Ö8). **Zincir türü her seferinde etiketlenir:**

| Tür | Ne demek |
|---|---|
| **Kesintisiz gözlenen zincir** | Aynı oturumda, aynı kayıtlar izlenerek |
| **Yeniden kurulan zincir** | Farklı oturum veya tarihlerdeki kanıtlardan birleştirilmiş |
| **Son adımı doğrulanmamış zincir** | Sonuncu halkanın etkisi ölçülmedi |

Yalnız birincisine "tam gözlenmiş" denir. Bilinen tek aday — MM taksit → borç → kısmi ödeme →
sonraki dönem — **ikinci türdendir** (10 ve 12 Eylül koşumları). Ayrıca envanter E0256 için
"15 Eyl tarihli taksit 12 Eyl'de gerçek satır olarak duruyor" diyor; bu kare **"sonraki tarihte
otomatik oluştu"yu değil, sonraki dönem görünümünü** kanıtlar.

---

## 6 · Biçim kuralları

### Anlatım bileşenleri

Belge 1'in beş kalıbı (A soru · B yan yana · C şerit · D kart · E tablo) Belge 2'de kullanılmaz;
onlar ekran anlatmak içindi. Belge 2'nin katalogu — **hepsi tablo değildir**:

| Tip | Ne | Ne zaman |
|---|---|---|
| **T1 · Olay × etki matrisi** | tablo | **Varsayılan.** Her olay: bir satır (Ne oluştu) + altı etki, ürünler sütun |
| **T2 · Önce/sonra sayı bloğu** | tablo | Sayılar zincir hâlinde okunuyorsa. Ekranda olmayan toplam soluk basılır, nereden toplandığı yazılır |
| **T3 · Terim karşılıkları şeridi** | şerit | Bölüm girişinde. **Terimler eşanlamlı gibi sıralanmaz**; "Bu Ay", "Kesim Günü" ve "Payment Due Date" farklı tür kavramlardır ve farkları yazılır |
| **T4 · Dönem dağılımı tablosu** | tablo | Taksit, ekstre ve tekrarın aylara düşmesi |
| **T5 · Zincir tablosu** | tablo | C kısmında işin adımları ürünler arasında karşılaştırılacaksa |
| **T6 · Sonuç alanı tablosu** | tablo | Bir form alanının **sonucu** anlatılacaksa. Alanın adı ve yeri Belge 1'dedir |
| **T7 · Kaynak bloğu** | açıklama | Canlı kanıtı olmayan ürünler. Ürün · kaynağın söylediği · kaynağın türü · **ne doğrulanmadı**. Tabloya karışmaz. **Aynı kaynak metni her sayfada tekrarlanmaz** |
| **T8 · İşleyiş şeması** | şema | C kısmında. Katmanlı kutu-ok; son kat **"incelenen yüzeylerde gözlenmeyen kavramlar"**, oksuz |
| **T9 · Sınır notu** | not | Bloğun kapanışında. **Gerçekten bulunan sınır yazılır; sayfa doldurmak için belirsizlik üretilmez** |

**Kaldırılan kural:** v1'deki "arka arkaya ikiden fazla aynı tip tablo gelirse bölüm yanlış
kurulmuştur". Üç ardışık sorunun aynı yapıda olması mümkündür ve sırf çeşitlilik için tip
değiştirmek karşılaştırmayı zorlaştırır. Ölçüt tip sayısı değil, **sorunun en açık nasıl
anlatıldığıdır**.

### Kanıt nasıl gösterilir

**Rozet yalnız istisnada.** "Canlı kare" varsayılandır ve işaret taşımaz; rozet yalnız **koşum
kaydı**, **kaynak görseli**, **kaynak beyanı** ve **görülmedi** hücrelerinde durur. Göz zayıf
kanıta takılsın diye.

**Her iddiadan dayanağına kısa bir bağ kalır:** hücrede küçük bir dipnot işareti, karşılığı sayfanın
altındaki dayanak satırında. Kimlikleri yalnız sayfa altına yığmak iddia–kanıt bağını koparıyor.

### Kare politikası

- **A kısmında kare varsayılan olarak basılmaz**; argüman sayı ve oluşan kayıttır. İstisna: sayı
  ancak karede okunuyorsa (T2 ile birlikte küçük kare).
- **C kısmında konu başına 1–2 kare**: o konudaki işleyişin okunduğu ekran. Ürün başına değil,
  **konu başına** — aksi hâlde altı ürün × dokuz konu kare üretir.
- Belge 1'in K4 kararı geçerli: **Paraşüt, Logo İşbaşı ve QuickBooks karesi basılmaz.** KolayBi'nin
  destek görselleri basılır, video kareleri anılır.
- **Kare bütçesi:** kart 4 · borç/cari 3 · zaman 3 · hesaplar 2 · gelir/gider 2 · rapor 2 · veri 2 ·
  sınıflandırma 1 · diğer 0 · bağlar 0 = **19**, artı A kısımlarında birkaç istisna → **≈20–24**.
  *(v1'in "≈15 kare, Kısım II'de 9" tahmini hem yapıya hem K4'e aykırıydı: basılabilir ürün altı
  tanedir, dokuz değil.)*

### Yazım kuralları

- Ürün arayüz etiketleri **ekranda göründüğü dilde** (Havale, TRANSFER, Alındı, OWES ME, Bu Ay)
- Nicelik belirteci o kapsamı taşıyan kanıtla yazılır; yazılamıyorsa cümle gözlenen kapsamla
  sınırlanır. **"Hiçbir toplamda", "hiçbiri", "yok" gibi ifadeler hangi yüzeyin tarandığını söyler**
- **"Görülmedi" ile "yok" karıştırılmaz;** yokluk ifadeleri incelenen sürüm, paket ve yüzeyle sınırlı
- Ürünün kendi ölçüsü hakkında **değer hükmü kurulmaz**. "Toplamlar bozuluyor" değil, "bu toplamlar
  saf gelir/gider ölçüsüyle aynı içeriği taşımıyor"
- Ölçülmemiş etki cümlesi kurulmaz; gerekiyorsa "olası etki — ölçülmedi" etiketiyle ve seyrek
- **Bizim kavramlarımız rakibe uygulanmaz:** ADR adları, "tanır/taşır", `TransactionScope`,
  `CounterpartyPayment` Belge 2'de geçmez
- **"Olay"** araştırmacının karşılaştırma birimidir, ürünlerin kendi dili değildir; Bölüm 1'de bir
  cümleyle söylenir. **Her eyleme "ekonomik olay" denmez** — ek dosya eklemek veya bütçe ayarı
  değiştirmek olay defterinin satırı değildir

### Denetim ve sınırları

Üretim betiği Belge 1'inkiyle aynı beş kapıyı çalıştırır ve hata varsa **durur**: anılan her E kimliği
sözlükte ve diskte var mı · özgün karelerin hash'i envanterle aynı mı · her `Şekil N.M` göndermesinin
karşılığı basılı mı · sığmayan metin var mı · PDF metninde kişisel ad geçiyor mu.

**Belge 2'ye özgü altıncı kapı:** her olay matrisinde **bir satır + altı boyutun hepsi
değerlendirilmiş mi** — yani her hücre {tutar · değişmedi · görülmedi · uygulanmaz} değerlerinden
birini taşıyor mu, ve **"uygulanmaz" gerekçeli mi**. Kapı satırın *basılmasını* değil,
*değerlendirilmesini* arar.

**Betiğin yapamadıkları:** iddianın kanıttan çıktığını doğrulayamaz; görsele gömülü kişisel adı
yakalayamaz. Her bölümün bütün sayfaları görüntüye çevrilip tek tek incelenir. **Son kontrolde
yalnız kaynak dosyasının varlığı değil, iddianın gerçekten o kaynaktan çıkıp çıkmadığı denetlenir.**

### Bir konu bloğu ne zaman biter

1. Girişte hangi soru ve hangi koşumlar yazıldı
2. A kısmının bütün olayları yazıldı; her hücre dört değerden birini taşıyor
3. C kısmı A'yı tekrar etmiyor; işin tamamlanma koşulunu ve ürünün mekanizmasını anlatıyor
4. Kapanışta kanıtlanan farklar, bilinmeyenler ve varsa etiketli senaryo var
5. İddia tablosu, eksik listesi ve kaynaklar üretildi
6. Altı otomatik kapı temiz
7. Bütün sayfalar görüntüye çevrilip tek tek incelendi
8. "Bu bölüme girmez" listesindeki hiçbir konu bloğa sızmadı

---

## 7 · Kanıt toplama — hangi iddiayı kapatıyor?

**Mevcut test verisi silinmez veya sıfırlanmaz.** Agent canlı test başlatmaz; bu, kullanıcının
koşacağı iş listesidir. Sıralama "kolay ekran alınır"a göre değil, **kapatacağı iddiaya** göredir.

**Koşum adımları, karar kuralları ve dosya adları ayrı bir belgededir:**
[`belge2-kosum-listesi.md`](belge2-kosum-listesi.md). Orada ayrıca iki bulgu kayıtlı: Goodbudget'ın
From New Income formu GB-U01'de zaten kareye alınmıştı; sonucu 20 Eylül K3'te ölçüldü.
Dizindeki eski sekiz eksik kare S1 ile E0395–E0402 kimliklerini aldı; yeni 22 kareyle birlikte
dizin artık **387 kare / 422 kimlik** içeriyor.

### Sıfır — koşum gerektirmeyen uzlaştırma

Yeni kare istemeyen düzeltmeler önce yapılır: taksit rakamlarının 1.000 + 600 ayrımı, koşum
tarihlerinin ve filtrelerin her iddiaya yazılması, yokluk ifadelerinin taranan yüzeyle
sınırlandırılması, E0022/E0279'un hangi deneye ait olduğunun belirtilmesi.

### Bir — yazımı doğrudan etkileyenler · ✔ 20 Eylül’de koşuldu ve kapandı

**Sonuç:** K1 E0403–E0405 ile Eylül giderine kart ödemesinin eklenmediğini; K2 E0406–E0416 ile
kalan taksitlerin görünür yerini; K3 E0417–E0424 ile From New Income / Fill Each Envelope
sonucunu doğruladı. Aşağıdaki tablo koşum öncesi iş ve yeterlilik tanımıdır; bu üç iş tekrar
beklenmiyor. Saklama biçimi, üretim eşiği, GB değişmeme nedeni ve diğer gelir kipleri açık kalır.

| # | Ürün | İş | Kapattığı iddia | Yeterlilik koşulu |
|---|---|---|---|---|
| 1 | Money Manager | Eylül İstatistik ekranını aç | 5.3'ün 3. satırı: kısmi ödemenin gider toplamına etkisi | **Tek başına yetmez:** 10 Eylül'den sonra kayıt/silme/düzenleme olduysa 400'ün etkisi izole edilemez. Rapor alınmadan önce o dönemde başka değişiklik olup olmadığı yazılır |
| 2 | Money Manager | Taksit/plan listesini ara | 5.2 ve 12.3: kalan dört taksitin nerede tutulduğu (MM-Q03) | Bulunursa ekran kaydı; bulunmazsa **hangi yüzeylerin tarandığı** yazılır |
| 3 | Goodbudget | From New Income yolunu **koş** | 3.1: gelir anlatımının yalnız Credit yoluna dayanması | **Formu açmak yetmez.** Gelir girilip hesap ve zarf kalanının önce/sonrası alınmalı |

### S1 · Kanıt dizinini yenile · ✔ yapıldı, 357 → 387

Sekiz eski kare E0395–E0402, 22 yeni kare E0403–E0424 kimlikleriyle dizindedir.
Toplam 422 kimlik / 387 kare; dosya yolu, boyut ve SHA-256 eşleşmesi devir denetiminde doğrulandı.
Goodbudget hane adı teslim kopyasında karartılır; özgün kanıt değiştirilmez. Karartma 20 Eylül
devir kontrolünde motorun `KARARTMA_ORTAK` listesine işlendi: E0417/E0418/E0420–E0423 kapsamda,
E0419 ve E0424 hane adı taşımadığı için dışarıda.

### İki — yaşam döngüsünü derinleştirenler

| # | Ürün | İş | Kapattığı iddia | Yeterlilik koşulu |
|---|---|---|---|---|
| 4 | Wallet | Postpone / Dismiss dene | 7.3: erteleme ve atlamanın sonucu | Plan ve bekleyen örneğin önce/sonrası; planın kendisi mi değişti, örnek mi kapandı |
| 5 | Bluecoins | Çöp kutusundan geri yükle | 3.4: geri yüklemenin sonucu | Kaydın listeye ve bakiyeye dönüşü ayrı ayrı |
| 6 | Bluecoins | Otomatik giriş **açık** planı koş | 7.2: otomatik kolun davranışı | **Kutuyu işaretlemek yetmez;** üretim zamanı gelmiş bir plan gerekir |
| 7 | Hesap Defterim | 400'lük ödemenin Aktar formu | 5.3: formun kendi karesi (şimdi başka aktarımın formu kullanılıyor) | Kaydetmeden görüntü |

### Üç — tamamlayıcılar

| # | Ürün | İş | Kapattığı iddia |
|---|---|---|---|
| 8 | Money Manager | Ödeme düğmesinin hazır tutarı | 5.3 notu: ön doldurulmuş tutarın 400'lük denemedeki değeri |
| 9 | Wallet | Payment Due Date alanını aç | 5.5: alanın işlevi |
| 10 | Wallet | Settings alt akışlarını tara | 10.1: dışa aktarma yolunun varlığı |

### Yapılamayanlar — iki ayrı sebep

**(a) Önemli, ama mevcut veriyi koruma kuralıyla çatışıyor.** Testin değeri düşük olduğu için değil:

| Ürün | İş | Neden |
|---|---|---|
| Dört canlı ürün | Kart borcunun tamamen ödenmesi | Mevcut borç bakiyelerini değiştirir. **5.4 ve dönem/ekstre iddiasının tek kapısı bu** |
| Hesap Defterim | Aktarımın bir bacağının silinmesi | Silme kalıcı |
| Bluecoins | Ekstre döneminin geçmesi | Beklemek gerekir |

**(b) Bu raporun ana iddiasına katkısı düşük olduğu için ertelenenler:** Goodbudget'ın ücretli kart
ve transfer yüzeyleri; Paraşüt/Logo/QuickBooks'un iç ekranları.

**HD'nin dışa aktarma dosyası üç ayrı adımdır** ve karıştırılmaz: dosyayı üretmek · cihazda
içeriğini incelemek · üçüncü kişiye göndermek. İçerik incelemesi teslimle eşitlenmez; paylaşım
gerçekten kaçınılmazsa ayrıca belirtilir.

---

## 8 · Kararlar — tamamı kapandı

v1'deki on dört kararın sekizi v2'de kapandı ve §0–§6'ya girdi. A1, 20 Eylül'de Sıfır + Bir
koşularak kapandı. **A2–A6, 20 Eylül'de kullanıcının açık devriyle kapatıldı** ("yapılacakları
sana bırakıyorum"). Beşinde de planın kendi önerisi seçildi; seçim gerekçesi ve — daha önemlisi —
her kararın **yazımda ürettiği kural** aşağıdadır. Seçenekler tarihsel kayıt olarak korunur.

| # | Karar | Seçenekler | Sonuç |
|---|---|---|---|
| **A1 — kapandı** | §7'nin hangi kısmı yazımdan önce koşulacak | (a) Sıfır + Bir (dört iş) · (b) Sıfır + Bir + İki · (c) Yalnız Sıfır | **(a)** uygulandı; K1/K2/K3 ve S1 koşuldu |
| **A2 — kapandı** | Konu yazım sırası | (a) Ağırlar önce · (b) Belge sırası · (c) Kanıtı en sağlamdan | **(a)** |
| **A3 — kapandı** | Olayların karşılaştırılabilirlik koşulu | (a) Evet, fark karşılaştırılır · (b) Yalnız aynı tutarlı · (c) Ayrı işaretle | **(a)** + giriş koşulu |
| **A4 — kapandı** | Farklı koşumların birleştirilme kuralı | (a) Evet, hücre tarih taşır · (b) Tek koşum · (c) Zincir kurulamaz | **(a)** + zincir etiketi |
| **A5 — kapandı** | Ana iddiası kanıtsız kalan blok | (a) Yazılır, "görülmedi" kalır · (b) Ertelenir · (c) Küçültülüp katılır | **(a)** |
| **A6 — kapandı** | Ürünün bütünsel görünümü | (a) Bölüm 12'ye ürün özeti · (b) Ayrı bölüm · (c) Hiç | **(a)** + sabit beş satır |

### A2 · Yazım sırası — ağırlar önce

**Sıra:** 5 → 6 → 7 → 3 → 4 → 9 → 8 → 10 → 11 → 12 → 2 → 1 → Ek.

Gerekçe iki katlı. Blok biçimi (giriş → A → C → kapanış) en çok olayı taşıyan bölümde sınanmalı;
kart bölümü altı olayla en ağırı ve T1/T2/T4/T8'in hepsini birden kullanan tek bölüm. Orada
oturmayan bir biçim hafif bölümlerde oturmuş gibi görünür ve hata en sona saklanır. İkincisi,
Bölüm 1 (okuma kılavuzu) ve Bölüm 2 (kapsam haritası) belgenin **kendisini** tarif eder; ne
anlattığını bilmeden yazılırsa iki kez yazılır. Belge 1'de de böyle yapıldı.

**Kural:** bölüm bitmeden sonrakine geçilmez; her bölüm kendi kapılarından (üretim + gözle sayfa
incelemesi) geçtikten sonra sıradaki başlar.

### A3 · Karşılaştırılabilirlik — fark karşılaştırılır, tutar değil

Matrislerdeki olaylar farklı tutar ve tarihlerle koşuldu: kart ödemesi MM'de ₺400, BC'de ₺500,
WL'de ₺400; koşumlar 10–20 Eylül arasına yayılı. **(a)** seçildi: satırın karşılaştırdığı şey
tutarın kendisi değil, **tutarın yarattığı fark**tır — bakiye düştü mü, gider toplamına girdi mi,
rapor değişti mi. Bunlar tutardan bağımsız olarak karşılaştırılabilir.

**Kural (bağlayıcı):** her matrisin girişinde, o matristeki olayların **tutar ve tarih farkı tek
cümleyle yazılır**. Yazılmamış matris eksiktir. Mutlak tutarların ürünler arasında toplanması,
oranlanması veya "daha çok/az" denmesi yasaktır; karşılaştırılan yalnız etkinin yönü ve türüdür.

### A4 · Farklı koşumların birleştirilmesi — hücre kendi tarihini taşır

**(a)** seçildi: bir olayın satırı farklı oturumlardan gelen hücreler taşıyabilir. Aksi hâli
(b) elde olan kanıtın büyük kısmını çöpe atardı — ürünler aynı gün koşulmadı ve bazı ölçüler
(Goodbudget'ın "önce 2.050" raporu) yalnız önceki koşumda var.

**Kural (bağlayıcı):** (1) Farklı koşumdan gelen hücre **koşum tarihini taşır**. (2) Aynı satırda
birden çok koşum varsa bu satırın altında belirtilir. (3) **Önce/sonra zinciri farklı koşumlardan
kurulursa** T2 bloğunda zincirin kırıldığı yer açıkça işaretlenir ve o hücre "koşum kaydı"
rozetini alır — Goodbudget 2.050 → 3.284 tam olarak budur. Kesintisiz tek koşumdan gelen zincir
bu işareti taşımaz.

### A5 · Kanıtsız kalan blok — yazılır, boşluk görünür kalır

**(a)** seçildi. Blok yazılır, ana satır `görülmedi` rozetiyle durur, aynı satır bölümün kapanış
sınır notuna ve belge sonundaki eksik listesine düşer.

Gerekçe: Belge 3 kararı verecek olan belgedir ve **neyin bilinmediğini bilmek zorundadır**.
Ertelenen blok (b) belgede hiç görünmez; okur eksiği fark edemez. Küçültüp başka bloğa katmak (c)
eksiği başka bir konunun içinde gizler. Boşluğun görünür kalması bu araştırmanın tutarlı kuralıdır.

**Kural:** "görülmedi" hücresi **hangi yüzeyin tarandığını** yazar. Taranmamış bir yüzey için
"yok" yazılmaz. Sayfa doldurmak için belirsizlik üretilmez (T9).

### A6 · Ürünün bütünsel görünümü — Bölüm 12'de sabit beş satır

**(a)** seçildi, sabit bir biçimle. Kurgu kararıyla C kısmı dokuz bloğa dağıldı; "bu ürün nasıl
düşünüyor" cevabı hiçbir yerde toplu durmuyor (§10 risk 5). Bölüm 12 zaten bölümler arası bağların
bölümü; ürün özeti oraya doğal girer ve yeni bölüm açmaz.

**Kural:** basılabilir altı ürünün her biri Bölüm 12'de **aynı beş satırla** özetlenir. Sabit
biçim, özetlerin yan yana okunabilmesi içindir:

1. **Para nerede yaşıyor** — hesap mı, zarf mı, defter mi; bakiye neyin toplamı
2. **Olay ile ödeme ayrılıyor mu** — kayıt borcu tanıdığında bakiye kıpırdıyor mu
3. **Gelecek nasıl tutuluyor** — plan ayrı bir nesne mi, listedeki satır mı, hiç yok mu
4. **Gelir/gider ölçüsü neyi sayıyor** — hangi hareket toplama girer, hangisi girmez
5. **Nerede durdu** — o üründe incelenmemiş veya kapanmamış tek önemli nokta

Bu beş satır **yeni iddia üretmez**; yalnız bölümlerdeki kanıtlı sonuçlara gönderme yapar. Kanıtı
olmayan satır `görülmedi` yazar (A5).

---

## 9 · Dış inceleme için sorular

İlk tur tamamlandı ([`belge2-claude-code-plan-review.md`](belge2-claude-code-plan-review.md)).
İkinci tur yapılacaksa bunlara bakılmalı:

1. §0'daki **bir satır + altı etki** ve dört hücre değeri gerçekten yeterli mi? "Uygulanmaz"
   gerekçesi nasıl denetlenir?
2. Konu bloğu anatomisi (giriş → A → C → kapanış) her bölümde uygulanabilir mi? Hangi bölümde
   C kısmı A'yı tekrar etme riski en yüksek?
3. Bölüm 5 ile 6 arasındaki sınır doğru yerde mi? Kart borcu bir cari borç mudur?
4. §6'daki yokluk ve değer hükmü kuralları bölüm tablolarında **fiilen** uygulanmış mı?
   Uygulanmamış satırları göster.
5. Bu plan rakibi puanlıyor mu? BF'yi ölçüt yapan, ADR diline kayan bir cümle kaldı mı?
6. §7'deki yeterlilik koşulları yeterince sıkı mı? "Formu açmak yetmez" uyarısı eksik kalan
   madde var mı?
7. Kare bütçesi (≈20–24) ve sayfa tahmini (65) yeni yapıyla tutarlı mı?
8. §8'deki altı kararda seçenekler gerçekten ayrışıyor mu? Sorulmamış karar var mı?
9. A6 (ürünün bütünsel görünümü) çözümü Belge 3 için yeterli mi?
10. v1'de bulunan hatalardan hangileri bu sürümde **tam** düzeltilmemiş?

---

## 10 · Açık işler ve riskler

| # | İş / risk | Durum |
|---|---|---|
| 1 | §7'nin Sıfır ve Bir kısmı | ✔ K1/K2/K3 ve S1 kapandı; İki/Üç yazımı durdurmayan açık işler |
| 2 | Belge 1'in inceleme notlarındaki 20 madde | İşlenmedi; Belge 2'den bağımsız iş |
| 3 | Belge 1'in patron onayı | Bekliyor; Belge 2 yazımı bunu beklemiyor |
| 4 | **Risk:** C kısmının A'yı cümleye çevirerek tekrarlaması | §2 ve §6'daki bitirme ölçütü bunu arar |
| 5 | **Risk:** bir ürünün mekanizmasının dokuz bloğa dağılıp kaybolması | ✔ A6 kapandı: Bölüm 12'de ürün başına sabit beş satır |
| 6 | **Risk:** çıkarım payının C kısımlarında yükselmesi | Gözlendi/çıkarım/bilinmiyor ayrımı ve Ö4 zorunlu |
| 7 | **Risk:** "görülmedi" hücrelerinin belgeyi boş göstermesi | Kabul edilir; boşluk saklanmaz |
| 8 | Üretim altyapısı | Hazır. Belge 1'in `ortak/motor.py` ve `kalip.py` değiştirilmeden kullanılır; T1–T9 bileşenleri bölüm klasöründe tanımlanır |

---

## 11 · v2'de değişenler

Kaynak: dış inceleme (`belge2-claude-code-plan-review.md`), kullanıcının yapı kararı ve kendi
denetimim. İncelemenin bulgularını kaynaktan doğruladım; hangisinin hangi dosyada teyit edildiği
aşağıda.

| # | Değişiklik | Kaynak |
|---|---|---|
| 1 | **Yapı baştan kuruldu: konu konu blok** (giriş → A → C → kapanış). Ayrı "Ürünlerin motorları" bölümü ve ayrı senaryo bölümü kalktı; 14 bölüm 12'ye indi | Kullanıcı kararı |
| 2 | **"Uygun olan" geri geldi.** Altı boyut değerlendirilir; hücre {tutar · değişmedi · görülmedi · **uygulanmaz**} alır. Denetim kapısı "basıldı mı"dan "değerlendirildi mi"ye döndü | İnceleme §2; `FAZ7-8-UYGULAMA-PLANI.md:321` ile doğrulandı |
| 3 | **4. satır "rapor ve diğer göstergeler" oldu**; genel toplam, bütçe/zarf kalanı, rapora dahil olma ve kırılım birlikte | İnceleme §2 |
| 4 | **"Ne oluştu" satırı eklendi** (0. satır): hangi kayıt, nesne veya bekleyen durum doğdu | İnceleme §6 |
| 5 | **Ayrı zincir şeridi konmadı**; işin tamamlanma koşulu C kısmının işi (Ö1) | Kendi kararım; K2 ile gerekçeli |
| 6 | **Asimetri teşhisi düzeltildi.** "Pipeline başlığı varsa ürünün kendi anlatımı" iddiası kaldırıldı; etiket iddiaya bağlanıyor (Ö4) | İnceleme §3; `goodbudget.md:200` ve `hesap-defterim.md:259` kanıt etiketleriyle doğrulandı |
| 7 | **MM taksit sayısı düzeltildi:** 2.050 → 3.650 farkı 1.000 taksit + 600 abonelik | İnceleme §4; `money-manager.md:205` ile doğrulandı |
| 8 | **Liste neti ile rapor toplamı ayrıldı.** E0022/E0279'un aynı türden daha eski bir ödemeye ait olduğu ve gelir/gider satırının "görülmedi" kaldığı yazıldı | İnceleme §4 |
| 9 | **Beş tarafsızlık ihlali düzeltildi:** "toplamlar bozuluyor", "hiçbir toplamda yok", "MM hiçbiri", "MM/HD'de ayrım yok", "bulunmayan nesneler" | İnceleme §5 |
| 10 | **Sayfa tahmini yeniden hesaplandı:** 65, toplamı gösterilerek. v1'in 40–46'sı kendi alt tahminleriyle çelişiyordu | İnceleme §8; topladım, 48 + 12–14 + 4 + 4–6 |
| 11 | **Kare bütçesi yeniden kuruldu:** ≈20–24, konu başına. v1'in "Kısım II'de 9 kare"si K4 ile çelişiyordu (basılabilir ürün altı) | İnceleme §8 |
| 12 | **Katı biçim kuralları kaldırıldı:** "ikiden fazla aynı tip tablo" ve "her sayfada 2–3 bilinmeyen". T3'te kavram farkı yazılıyor | İnceleme §8 |
| 13 | **Bölüm 11 başlığı düzeldi:** "finans dışı modüller" → "diğer modüller ve yardımcı araçlar" | İnceleme §7 |
| 14 | **§7 baştan yazıldı:** "hangi iddiayı kapatıyor" sıralaması, her işe yeterlilik koşulu, "Önerilmez" ikiye ayrıldı | İnceleme §9 |
| 15 | **Senaryolar üç zincir türüne ayrıldı**; MM zincirinin "yeniden kurulan" olduğu ve E0256'nın neyi kanıtlamadığı yazıldı | İnceleme §10 |
| 16 | **Üç yeni karar eklendi:** A3 karşılaştırılabilirlik, A4 koşum birleştirme, A5 kanıtsız blok | İnceleme son bölüm |
| 17 | **Kanıt gösterimine dipnot bağı eklendi** (yalnız sayfa altına yığma) | İnceleme D4 |
| 18 | **Belge 1 tekrarı yasağı gevşetildi:** kısa bağlam + gönderme serbest | İnceleme D12 |
| 19 | **Ürün sütunu kuralı netleşti** (Ö3): bölüm içinde sabit, düşen ürün tarama sonucu olarak not edilir | Kendi denetimim; Belge 1 Ö6 ile çatışmayı önlemek için |
| 20 | **4.3 ile 9.3 arasındaki tekrar giderildi**; sayılar 4.3'te, gönderme 9.3'te | Kendi denetimim |
| 21 | **Eksik kanıt kimlikleri girdi:** BC E0020 · WL E0277 · GB E0116 | Kendi denetimim; `belge1-bolum-plani.md:434` |
| 22 | **6.5 eklendi:** yaşam döngüsünün kapanışı (tam kapanış, iptal, silme) kısa kapsam kaydı olarak | İnceleme §7 |
| 23 | **10.3 eklendi:** belgeden kayıt üretimi (Paraşüt/Logo fiş okuma) ayrı alt soru | İnceleme §7 |
| 24 | **Borç yönü sınırı yazıldı:** incelenen zincirler ağırlıkla alacak yönünde; sonuç ters yöne taşınmaz | İnceleme §7 |

---

**Bu plan onaylanmadan bölüm yazımına başlanmaz.** Onaylanan sürüm ve tarihi `raporlar/README.md`
durum tablosuna işlenir.
