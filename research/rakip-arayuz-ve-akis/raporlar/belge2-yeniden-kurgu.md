# Belge 2 — yeniden kurgu kararları

**Tarih:** 21 Eylül 2026 · **Durum:** karar bekliyor · **Kaynak:** kullanıcının 21 Eylül
eleştirisi (yedi madde) + mevcut belgenin ölçümü

Bu bir **karar belgesidir**, üretim planı değil. Her maddede sorun ölçüyle yazılı, seçenekler
sıralı ve bir öneri işaretli. Onaylanan seçenekler `belge2-bolum-plani.md` **v3**'e geçer; v2
arşive alınır. **Onay gelmeden hiçbir bölüm yazılmaz ve mevcut `belge2/` silinmez.**

Kullanıcı bu turda kural değiştirme yetkisini açıkça verdi: *"önemli olan ortaya çıkan ürün,
ondan dolayı kuralları esnetebilirsin veya değiştirebilirsin."* Aşağıdaki öneriler bu yetkiyi
kullanıyor; hangi kuralın hangi maddede kırıldığı her kararın altında yazılı.

---

## 1 · Mevcut belge, sayılarla

83 sayfa · 23.221 kelime · 5 basılı kare.

| Ölçü | Değer | Ne anlama geliyor |
|---|---:|---|
| "Görülmedi" / "görülmedi" | **204 kez** | Sayfa başına ortalama 2,5 |
| "ölçülmedi" | 49 | |
| "denenmedi" · "bulunamadı" · "koşulmadı" | 34 · 17 · 10 | |
| **Olumsuzluk cümlesi geçen satırların kelime payı** | **%30,0** | Belgenin üçte biri yapılmayan işi anlatıyor |
| "Dayanak. Çıkarılmayan sonuç: …" satırı | 36 | Her konu bloğunun sonunda, çıkarımın *yasaklandığını* yazan satır |
| Bölüm 1 + 2 (okuma kılavuzu + kapsam) | 2.140 kelime, 8 sayfa, **%9,3** | |
| C ("işleyiş") kısımları | 9 adet, %9,6 | 12 bölümün 4'ünde hiç yok |
| Basılan kare | **5** | Plan §6 "kare bütçesi ≈20–24" diyordu |
| Bluecoins'ten basılan kare | **0** | |

### Kanıt kullanımı — ürün başına

| Ürün | Envanterde kare | Belge 2'de anılan | Oran |
|---|---:|---:|---:|
| Money Manager | 45 | 32 | **%71** |
| Hesap Defterim | 45 | 26 | %58 |
| KolayBi | 40 | 17 | %42 |
| Wallet | **110** | 42 | %38 |
| Goodbudget | 39 | 15 | %38 |
| **Bluecoins** | **93** | 29 | **%31** |
| Paraşüt · Logo · QuickBooks | 22 | 0 | %0 |

En çok koşulan iki ürün (Wallet 110, Bluecoins 93) en az kullanılan ikisi. Bluecoins'te
**64 kare hiç girmemiş** — içlerinde kart kesim günü ve limit alanları (`E0048`), transfer formu
(`E0029`), taksit şartları sayfası (`E0033`), bağımsız hatırlatıcı listesi (`E0046`/`E0047`),
çöp kutusu (`E0089`), dışa aktarma (`E0104`), nakit akım ayarı (`E0086`) var. Bunların çoğu
belgenin *"görülmedi"* dediği konular.

### İki kuralın planda yazılıp uygulanmadığı

Bu önemli: kötülüğün bir kısmı **plan hatası değil, uygulama hatası**.

| Plan v2 ne diyordu | Belge ne yaptı |
|---|---|
| §6 Kare bütçesi: "≈20–24 kare" | 5 kare |
| §6: "sayfa doldurmak için belirsizlik üretilmez" | Kelimelerin %30'u belirsizlik |
| §2: bölüm tahminleri toplamı ≈65 sayfa | 83 sayfa |
| §2: "C, A'daki tabloyu cümleye çevirerek tekrarlamaz" | C'lerin bir kısmı tam olarak bunu yapıyor |

---

## 2 · Yedi eleştiri → kök neden

| # | Eleştiri | Ölçüsü | Kök neden |
|---|---|---|---|
| 1 | Okuma kılavuzu + kapsam gereksiz uzun | %9,3, 8 sayfa | Plan §2 bu iki bölüme 6 sayfa ayırmış; Belge 1'in "okuru hazırlama" kalıbı kopyalanmış |
| 1b | 2.2'de derin ürünler yanlış | Bluecoins %31, Wallet %38 · Hesap Defterim %58 | Derinlik **plandan elle** atanmış (§3), kanıt dizininden sayılmamış |
| 2 | Giriş okunmaz, sabit sözlük/karışan terimler gereksiz | 964 kelime | §0'ın "bir satır + altı etki" kavramı belgeye *öğretilecek bir sistem* olarak girmiş |
| 3 | Fazla dürüst, koşum detayı metne sızmış | 204 + 49 + 36 | A4 (hücre kendi koşum tarihini taşır), A5 (kanıtsız blok yazılır), T9 ve "Dayanak / Çıkarılmayan sonuç" kalıbı |
| 4 | Çıkarım yok | 36 kez "Çıkarılmayan sonuç" | Araştırma `README.md` §"Belge 1 ve 2 rakibi anlatır, puanlamaz" — **kural çıkarımı yasaklıyor** |
| 5 | Çok fazla "bilinmiyor" | %30 | A5 kararı + her bölümde zorunlu "Bilinmeyenler" bloğu |
| 6 | Goodbudget "nedeni bilinmiyor" kalmış | 3.6 | Aynı A5; oysa gözlem formunda **cevabı veren kanıt var** (§5) |
| 6b | 3.C1 başlığı ve tek şema | 9 C, 4 bölümde hiç yok | A/C iç kısaltması başlıklara sızmış; C konu başına tek şemaya donmuş |
| 6c | Son kısım uygulamamıza katkı sağlamıyor | — | Karar filtresi Belge 3'e kilitli; Belge 2'nin çıkışı Belge 3'e bağlanmamış |

---

## 3 · Kararlar

### K1 · Belgenin işi: anlatmak mı, çıkarım yapmak mı

**Gerçek bir çelişki var ve tek başıma çözemem.** `README.md` satır 211:
*"Gözlem formları ve Belge 1–2 rakibi anlatır, puanlamaz."* Karar filtresi
(`doğrudan al` / `alma` / …) yalnız Belge 3'e ait. Siz ise çıkarım ve değerlendirme istiyorsunuz.

| | Seçenek | Sonuç |
|---|---|---|
| A | Kural aynen kalsın | Belge bugünkü hâlinde kalır. **Reddedilir** — eleştirinin tamamını karşılamaz |
| **B** | **Dil dört katmana ayrılır** (öneri) | Aşağıdaki tablo |
| C | Tam serbest: kendi ADR'lerimizle kıyas da Belge 2'ye girsin | Belge 3'ün işini yer, tarafsızlık kaydını bozar. **Reddedilir** |

**Seçenek B'nin dört katmanı:**

| Katman | Belge 2'de | Örnek |
|---|---|---|
| **Gözlem** — karede okunan | ✅ kalır | "Zarf 43.784 → 45.018, hesap 41.734'te kaldı" |
| **Mekanizma çıkarımı** — kanıttan çıkan neden | ✅ **yeni** | "Hesap katmanı işlem listesinden hesaplanmıyor" |
| **Değerlendirme** — ne kazandırıyor / ne kaybettiriyor | ✅ **yeni** | "Zarf modeli parayı amaca bağlıyor; karşılığında 'param nerede' sorusu iki sayıya bölünüyor" |
| **Karar** — alalım / almayalım / ADR'ye meydan | ❌ Belge 3'te kalır | — |

Bu ayrım hem istediğiniz çıkarımı verir hem `README.md`'nin "ne kazandırıyor / ne kaybettiriyor"
ekseniyle uyumludur — o eksen zaten değerlendirme ister, yalnız **bizim kararımızı ölçüt
yapmayı** yasaklar. Onaylanırsa `README.md` satır 211 bu ayrımla güncellenir.

---

### K2 · Bölüm 1 ve 2

| | Seçenek | Sonuç |
|---|---|---|
| A | İkisini de tamamen kaldır | Erişim asimetrisi (kim canlı, kim yalnız kaynak) okur için kaybolur |
| **B** | **Tek sayfa giriş + yöntem eki** (öneri) | Aşağıda |
| C | Kalsın, kısaltılsın | Yarım tedbir; "sabit sözlük" yine belgeye girer |

**B'nin içeriği — giriş, tek sayfa:**

- Üç cümle: bu belge ne anlatıyor, Belge 1'den farkı ne, neye dayanıyor
- **2.1'in ürün × erişim tablosu** (bu tablo iyi, kalır — okur hangi iddianın tavanını bilmeli)
- Bitti.

**Ek'e (belgenin sonuna) inen:** kanıt düzeyleri tanımı, sabit sözlük, karışan terimler,
ortam ve veri, veriyi koruma kuralı. Okuyan okur, okumayan kaybetmez.

**Tamamen kalkan:** 1.1 "bir satır ve altı etki" sistemi (bu bir *üretim* aracıydı, okurun
öğrenmesi gereken bir şey değil), 1.3 "bir sayfa nasıl okunur", "bu belgenin yapmadıkları".

**2.2 tablosu** (hangi bölümde hangi ürün) kalkar — yerine K4 gelir.

---

### K3 · "Görülmedi"nin yeri

Mevcut kural A5: *"kanıtsız kalan blok yazılır, boşluk görünür kalır."* Sonucu: %30.

| | Seçenek | Sonuç |
|---|---|---|
| A | Aynen kalsın | **Reddedilir** |
| **B** | **Belgeden çıkar, eksik listesine iner** (öneri) | Aşağıda |
| C | Bloklar kalsın, kısalsın | Yarım tedbir |

**B'nin kuralı:** her bölümün `eksik-listesi.md`'si zaten var ve iyi çalışıyor (252 satır,
öncelik etiketli). Eksikler **orada** yaşar, belgede değil. Belge metnine yalnız şu girer:

- İddiayı **doğrudan sınırlayan** durum, cümle içinde, ayrı blok değil.
  *"Üç üründe ölçüldü"* yazılır; dördüncü için *"ölçülmedi"* cümlesi kurulmaz.
- Bir konuda hiç kanıt yoksa **konu açılmaz.** Boş konu bloğu basılmaz.
- Zorunlu "Bilinmeyenler" bloğu kalkar. "Dayanak. Çıkarılmayan sonuç: …" satırı (36 kez)
  tamamen kalkar.

**Sınır — dürüstlük nereye gidiyor:** kanıt düzeyi rozeti (canlı kare / koşum kaydı /
kaynak görseli / kaynak beyanı) **kalır**. Kaybolan şey belirsizliğin *anlatılması*, kanıt
düzeyinin *işaretlenmesi* değil. Bir iddianın hangi kareden çıktığı her zaman izlenebilir kalır.

---

### K4 · Ürün ağırlığı — Bluecoins, Wallet ve KolayBi neden yok

| | Seçenek | Sonuç |
|---|---|---|
| A | Ürün başına eşit sayfa | Kanıt eşit değil; yapay |
| **B** | **Ağırlık kanıt dizininden sayılır** (öneri) | Aşağıda |
| C | Ağırlığı kullanıcı bölüm bölüm atar | Sizi her bölümde meşgul eder |

**B'nin mekaniği:** bir bölüm yazılmadan önce o konuya ait kareler **ürün bazında sayılır**;
derin ürün, o konuda en çok kanıtı olan üründür — plandan değil dizinden çıkar. Üretim betiğine
bir kapı eklenir:

> **Kapı:** bölüm bittiğinde, o konuda 10'dan fazla karesi olan bir ürünün karelerinin
> **en az %60'ı** ya metinde anılmış ya eksik listesinde gerekçesiyle yazılmış olmalı.
> Aksi hâlde üretim durur.

Hedef: Bluecoins %31 → ≥%60, Wallet %38 → ≥%60. Bu tek başına Bluecoins'ten ~30, Wallet'tan
~25 yeni iddia demek.

**Not:** Hesap Defterim azaltılmaz. Sorun onun fazlalığı değil, diğerlerinin eksikliği.

---

### K5 · A/C kurgusu kalkar

Sorun üç katlı: (1) "A kısmı / C kısmı / İşleyiş" bizim iç kısaltmamızdı, başlıklara sızdı;
(2) C konu başına tek şemaya dondu — 3.C1 bütün gelir/gider sürecini tek kutuya sıkıştırdı;
(3) 8, 11 ve 12'de hiç C yok, yani kurgu tutarlı bile değil.

| | Seçenek | Sonuç |
|---|---|---|
| **A** | **Ayrım kalkar, tek akış** (öneri) | Aşağıda |
| B | A/C kalsın, C ürün başına bölünsün | Şema sayısı 9 → ~25; belge şişer |
| C | C bölümün başına alınsın | Aynı kurgu, sıra değişikliği |

**Önerilen konu bloğu — dört adım, iç terim yok:**

```
1. Ne oldu                     Olay bir cümle + önce/sonra sayısı. Kare burada basılır.
2. Ürünler nerede ayrışıyor    Karşılaştırma tablosu (bugünkü matris, temizlenmiş)
3. Neden ayrışıyor             Mekanizma çıkarımı. Şema yalnız gerçekten katmanlı bir şey
                               varsa; yoksa iki paragraf. Zorunlu değil.
4. Ne kazandırıyor / kaybettiriyor   Ürün başına tek satır + Belge 3'e taşınan soru
```

Başlıklar okurun dilinde olur: *"Kartla harcama yapınca borç nereye yazılıyor"* — "5.C1 ·
İşleyiş" değil. **"A", "C", "olay defteri", "matris", "konu bloğu" kelimeleri belgede geçmez.**

Gelir/gider gibi ürünlerin gerçekten benzeştiği konularda 3. adımın tek şema olması meşrudur —
ama bu bir *bulgu* olarak yazılır ("beş üründe kayıt aynı biçimde yazılıyor, ayrışma sonrasında
başlıyor"), kurgunun zorunlu kıldığı bir kutu olarak değil.

---

### K6 · Kare politikası

Plan ≈20–24 kare diyordu, belge 5 bastı. Elimizde 387 kare var ve Belge 2 onların %1,3'ünü
gösterdi.

| | Seçenek | Sonuç |
|---|---|---|
| A | Plandaki 20–24'e dön | Plan zaten bunu diyordu, yine az |
| **B** | **Kare iddia taşıdığı yerde basılır — hedef 45–60** (öneri) | Aşağıda |
| C | Kare basılmasın (bugünkü hâl) | **Reddedilir** |

**B'nin kuralı:** Belge 1 ekranı anlatmak için kare basar (kare tek başına durur).
Belge 2 **sonucu göstermek için** basar: önce/sonra çifti, değişen sayı işaretli.
Aynı karenin iki belgede olması sorun değil — işi farklı.

- Her konuda **en az 1 kare**; sayı değişimi anlatılıyorsa **önce/sonra çifti**
- Kare altında tek satır: neye bakılacağı. Uzun açıklama yok
- Paraşüt / Logo / QuickBooks karesi basılmaz (Belge 1 K4 kararı korunur)

---

### K7 · Kapanışlar ve BusinessFinance'e bağ

Bugünkü kapanış: *Kanıtlanan farklar · Bilinmeyenler · Kısa senaryo · "Çıkarılmayan sonuç"*.
Önerilen kapanış:

1. **Ne ayrışıyor** — tek cümlelik iddia
2. **Neden** — mekanizma çıkarımı
3. **Ne kazandırıyor / ne kaybettiriyor** — ürün başına tek satır
4. **Belge 3'e taşınan soru** — 1–3 madde, *soruyu netleştirir, kararı vermez*

4. madde "belgeler uygulamamızı geliştirirken katkı sağlasın" isteğinin karşılığı. Karar Belge
3'te kalır ama Belge 2 artık **kararı hazır soruyla** teslim eder — bugün hiç etmiyor. Daha
ileri gitmek (Belge 2'de doğrudan öneri) seçilebilir; önermiyorum, Belge 3'ü gereksizleştirir.

---

### K8 · Koşum artığı dil temizlenir

- **A4 iptal:** "hücre kendi koşum tarihini taşır" kuralı kalkar. Tarih yalnız **sonucu
  değiştiriyorsa** yazılır (ör. iki üründe farklı ay koşulduğu için toplamlar kıyaslanamıyorsa).
- Koşum künyeleri, paket/sürüm notları, "hangi yüzey tarandı" cümleleri metinden çıkar;
  bölüm sonundaki kanıt satırına veya eke iner.
- "Sınır." (35) ve "Dayanak." (39) sabit etiketleri kalkar.

---

### K9 · Yeniden üretim kapsamı

| | Seçenek | Sonuç |
|---|---|---|
| A | Mevcut metni onar | Kelimelerin %30'u çıkacak, kurgu ve başlıklar değişiyor. **Reddedilir** |
| **B** | **İçerik sıfırdan, altyapı korunur** (öneri) | Aşağıda |

**Korunan:** üretim motoru (`belge1/ortak/`), kanıt dizini ve envanteri (387 kare / 422 kimlik),
karartma listesi, hash ve kırık atıf denetimleri, gözlem formları, `eksik-listesi` mekanizması.

**Yeniden yazılan:** bütün bölüm içerikleri, `ortak/b2.py` sayfa türleri ve kapıları.

**Saklanan:** mevcut `belge2/` → `belge2-v1/` olarak durur, silinmez.

---

### K10 · Yeni denetim kapıları

Eski kapılar §8'in kurallarını koruyordu; o kurallar değişiyor, kapılar da değişmeli.

| Kalkan kapı | Yerine gelen |
|---|---|
| "boş görülmedi hücresi yasak" | **Her konu en az bir mekanizma çıkarımı ve bir kazanç/kayıp satırı taşır** |
| "matris `fark` alanı taşır" | Korunur — bu kural iyiydi |
| — | **Ürün kanıt kullanımı ≥ %60** (K4) |
| — | **Her konu en az 1 kare** (K6) |
| — | **Yasak kelime taraması:** "A kısmı", "C kısmı", "olay defteri", "matris", "konu bloğu", "Çıkarılmayan sonuç" |

---

## 4 · Önerilen bölüm düzeni

| # | Bölüm | Bugünkü karşılığı |
|---|---|---|
| — | **Giriş** — 1 sayfa + ürün/erişim tablosu | Bölüm 1 + 2 (8 sayfa → 1) |
| 1 | Gelir ve gider kaydı | 3 |
| 2 | Hesaplar ve para aktarımı | 4 |
| 3 | Kart: harcama, taksit, borç, ödeme | 5 |
| 4 | Borç, cari ve tahsilat | 6 |
| 5 | Zaman: tekrar, plan, bütçe | 7 |
| 6 | Sınıflandırma: işletme mi, şahsi mi | 8 |
| 7 | Raporun neyi saydığı | 9 |
| 8 | Veri: dışa aktarma, içe aktarma, yedek | 10 + 11 birleşir |
| 9 | Ürünlerin birbirinden ayrıldığı yer — kapanış | 12 |
| Ek | Yöntem notu · eksik listesi · kanıt eki | 1.1–1.3, 2.3 buraya iner |

**Bölüm 11 (diğer modüller) 8'e birleşiyor:** 1.083 kelime harcıyor ama 2.2 kendi ağzıyla
*"hiçbiri açılmadı"* diyor. Kanıtı olan kadarı Veri bölümünün sonunda bir tabloya sığar.

---

## 5 · Örnek: aynı bulgu, iki yazım

Sizin 6. maddeniz ("Goodbudget'ta şu kaydın nedeni bilinmiyor yazıyorsun"). İşte o cümle ve
elimizdeki kanıtla nasıl yazılabileceği.

**Bugün (3.6, Bilinmeyenler):**

> Goodbudget'ta hesap bakiyesinin değişmeme nedeni.

**Önerilen:**

> **Goodbudget'ta zarf ve hesap iki ayrı defterdir; zarfı dolduran kayıt hesaba
> yazılmaz.** ₺1.234'lük gelir zarfı 43.784'ten 45.018'e çıkardı, Eylül raporunda
> geliri 2.050'den 3.284'e taşıdı, işlem listesinde `Ana Hesap` yazdı — ama
> `All Accounts` 41.734'te kaldı.
>
> *Çıkarım:* iki katman arasındaki fark, hesaba yazılmayan kayıtların toplamına eşit.
> Önce 2.050'ydi ve karşılığı `09/11 Initial Envelope Fill +2.050` satırıydı; yeni
> kayıttan sonra tam 1.234 büyüyerek 3.284 oldu. Sıradan gider kayıtları ise hesaba
> işliyor — Market −850 ve Mavi Yazilim −1.200 açılış zincirinde doğrulanıyor. Ayrışan
> şey kayıt türü: **zarf doldurma hesap adını taşısa bile hesap defterine girmiyor.**
> (Bunun *neden* böyle olduğu — senkronizasyon, ücretsiz paket sınırı veya tasarım —
> koşumdan çıkmıyor; belgede tek yan cümle olarak kalır.)
>
> *Kazanç:* zarf modeli parayı amaca bağlar ve kullanıcıyı "ne kadar harcayabilirim"
> sorusunda tutar. *Kayıp:* "param nerede" sorusunun iki cevabı olur ve ikisi tutmaz.
>
> *Belge 3'e taşınan soru:* bütçe ilerlemesi ile bakiye aynı kaynaktan mı beslenmeli?

Kanıt: `gozlemler/goodbudget.md` satır 58–73 (kontrol zinciri) ve 330–352 (K3 koşumu);
kareler `E0417`–`E0423`.

**Bu örnek aynı zamanda çıkarımın sınırını gösteriyor.** İlk yazdığım hâlde aynı
bulguyu "bakiye hiçbir işlem yapılmadan −616 kaydı" diyerek desteklemiştim; gözlem
formu o sapma için açıkça *"abonelikle tutar eşitliği nedensellik kanıtı değildir"*
diyor. Çıkarım serbestliği kanıtı gevşetmez — dayanağı formda yazılı olmayan bir
çıkarım belgeye girmez, bu yüzden G2 kapısı her çıkarımdan `dayanak` ister.

Fark: aynı malzeme, aynı kanıt düzeyi — ama "bilinmiyor" yerine **bulunan şey** yazılı ve
bizim uygulamamıza bir soru bırakıyor.

---

## 6 · Onayınızı bekleyen noktalar

Hepsinin bir önerisi var; onaylamak için "öneriler geçsin" demeniz yeterli. İtiraz ettiğiniz
maddeyi söylerseniz yalnız onu yeniden yazarım.

| Karar | Öneri | En tartışmalı yanı |
|---|---|---|
| K1 dil katmanları | B | `README.md`'nin "puanlamaz" kuralını değiştirmeyi gerektirir |
| K2 giriş | B | 2.1 tablosunun kalmasını istemeyebilirsiniz |
| K3 görülmedi | B | Kanıt düzeyi rozetinin kalmasına katılmayabilirsiniz |
| K4 ürün ağırlığı | B | %60 eşiği belgeyi uzatır |
| K5 A/C kalkar | A | — |
| K6 kare | B | 45–60 kare Belge 1 ile görsel tekrar demek |
| K7 kapanış | önerilen dörtlü | 4. madde Belge 3'ün sınırına yaklaşıyor |
| K8 koşum dili | temizlenir | — |
| K9 sıfırdan | B | Mevcut 83 sayfa kullanılmıyor |
| K10 kapılar | önerilen | — |

**Ayrıca iki şeyin önerisi yok, sizden öğrenmem gerekiyor:**

1. **Sayfa hedefi.** Bugün 83. Giriş −7, "görülmedi" temizliği −%30, çıkarım ve kare +%25
   sayarsak **~60–70 sayfa** çıkar. Daha kısa bir belge mi istiyorsunuz (ör. ~45), yoksa
   uzunluk serbest mi?
2. **Ek koşum.** K4 eşiği elimizdeki kanıtla tutar; ama Bluecoins kart kesim günü (`E0048`)
   gibi bazı konularda **bir ekran daha** çekilirse konu "görülmedi"den çıkıp iddiaya döner.
   Emülatör turu yapacak mısınız, yoksa yalnız mevcut 387 kareyle mi yazayım?

---

## 7 · K11 · Kaynaktan incelenen dört ürün nereye girecek

**21 Eylül, Bölüm 1 incelemesinden sonra eklendi.** Kullanıcının sorusu: Bölüm 1'de
yalnız canlı koşulan beş ürün var, dört kaynak ürünü nerede? **Cevap: bilinçli bir karar
değildi, eksikti.** KolayBi, Paraşüt, Logo İşbaşı ve QuickBooks Bölüm 1'de yalnız açılış
kapsam tablosunda duruyor; tek bir iddiada, tabloda veya akışta geçmiyorlar.

### Ellerinde gerçekte ne var

Dördü tek bir grup değil. Kanıt tavanları birbirinden çok uzak:

| Ürün | Kare | Ne gösteriyor | Basılabilir mi |
|---|---:|---|---|
| **KolayBi** | 40 | **31'i gerçek ürün ekranı** (destek materyali): gider formu ve gider tipleri, cari listesi/formu/ekstresi, alış faturası, personel carisi ve maaş, banka–kasa, **kredi kartı formu** (kesim günü, son ödeme, limit), çek/senet, KDV · gelir/gider · nakit akış raporları, proje ekseni | **Evet** — Belge 1 K4 destek görsellerini basmaya izin veriyor; video kareleri yalnız anılır |
| **Paraşüt** | 9 | 2 mağaza karuseli + 7 **tanıtım videosu** karesi: fatura gönderme, cari hesap durumu, banka entegrasyonu, stok/depo, gelir-gider ve nakit akış raporu | **Hayır** (K4) |
| **Logo İşbaşı** | 6 | Giriş, kayıt, sektör listesi, sözleşme + 2 video karesi (entegrasyonlar, müşavir portal). **Ürünün içi hiç görülmedi** | **Hayır** (K4) |
| **QuickBooks** | 4 | Yalnız onboarding ve ödeme duvarı. **Ürünün içi hiç görülmedi** — ama yardım merkezi metni kayıt düzeyinde `Business`/`Personal` alanını ve `Split transaction` akışını anlatıyor | **Hayır** (K4) |

### KolayBi'nin bölüm bölüm karşılığı

| Bölüm | KolayBi'de karşılığı | Güç |
|---|---|---|
| 1 · Gelir ve gider | Yeni Genel Gider formu, gider tipleri/kategorileri, gider listesi ve detayı | Güçlü |
| 2 · Hesaplar ve aktarım | Finans → Banka Hesapları, Kasalar | Orta |
| 3 · Kart | Kredi Kartları listesi + Yeni Kredi Kartı formu | Güçlü |
| 4 · Borç, cari, tahsilat | Cari listesi, cari formu, ekstre diyaloğu ve PDF önizlemesi, personel carisi | **En güçlü** |
| 5 · Zaman, plan, bütçe | Tekrarlı Maaş, Tekrarlı Genel Gider, notlar/hatırlatıcı | Orta |
| 6 · Sınıflandırma | **Proje ekseni** (kod/ad/gelir/gider/net, belge kırılımı) | Güçlü |
| 7 · Rapor | KDV, Gelir/Gider ve Nakit Akış raporları, rapor sekmeleri | Güçlü |
| 8 · Veri | Cari ekstresi PDF, içe/dışa aktarma, çek/senet bordroları | Orta |
| 9 · Kapanış | — | — |

### Asıl mesele: bunlar zayıf rakip değil, başka bir soruya cevap veriyor

Canlı beş ürün **kişisel finans defteri**; KolayBi, Paraşüt ve Logo **ön muhasebe**
(fatura, cari, stok, e-belge); QuickBooks Solopreneur **vergi ekseni**. Dördünü aynı
karşılaştırma tablosuna sütun olarak koymak, aynı eksende yarıştıklarını ima eder ve
okuru yanıltır — kanıt tavanları farklı olduğu için de haksız bir kıyas olur.

### Seçenekler

| | Seçenek | Sonuç |
|---|---|---|
| A | Dördü için ayrı bir bölüm | Kanıt tavanı karışmaz ama okur aynı konuyu iki yerde okur, karşılaştırma kopar |
| B | Her bölümün sonuna "kaynak bloğu" şeridi | **v1 bunu yaptı ve kopuk kaldı** — blok konuyla bağlanmıyordu |
| **C** | **Kanıt tavanına göre iki ayrı yol** (öneri) | Aşağıda |
| D | Hepsini tabloya sütun olarak ekle | Kanıt tavanı eşitmiş gibi görünür; yanıltıcı |

### Öneri — C: iki ayrı yol

**1. KolayBi konunun içine girer.** Karşılaştırma tablosunda kendi sütununu alır,
karesi basılır ve `Kaynak görseli` rozeti taşır. İddiası her zaman **yüzeydir**:
"formda Hesap Kesim Günü ve Kart Limiti alanları var" yazılır, "kart borcu şöyle
hesaplanıyor" yazılmaz — davranış görülmedi.

**2. Paraşüt · Logo · QuickBooks tabloya sütun olmaz.** Akış sayfasından sonra,
çıkarımdan önce gelen tek bir **"Kaynakta ne yazıyor"** bloğunda dururlar — ve yalnız
o konuda gerçekten bir şey söylüyorlarsa. Söylemiyorlarsa blok o bölümde hiç açılmaz.
Bu blok bir zorunluluk değil, bir olanaktır; ayrı sayfa da değildir.

**3. Farklı soru, girişte bir kez söylenir.** "Bu dört ürün ön muhasebe ve vergi
tarafındadır; aynı işi daha kötü yapmıyorlar, başka bir işi yapıyorlar." Bölüm içinde
tekrarlanmaz.

**4. Akış sayfasında dal açmazlar.** Akış ölçülmüş davranışın yoludur; kaynaktan
okunan bir yüzey yol üzerinde bir dal değildir. Gerekirse akışın altına tek satır not
düşer.

### K11-a · QuickBooks'un yeri özeldir

QuickBooks'un ürün içi hiç görülmedi ama yardım merkezi metni, **kurucu kararımıza en
yakın rakip yaklaşımı** anlatıyor: işlem başına `Business`/`Personal` alanı, üçüncü
değer yok, şahsi işaretlenen kayıt silinmiyor — ve kısmen işletme olan gider
`Split transaction` ile bölünüyor. Canlı beş ürünün hiçbirinde kayıt düzeyinde böyle
bir alan bulunamadı.

Bu, Bölüm 6'da (Sınıflandırma) **tek satırlık bir kaynak notu olamayacak kadar
merkezî**. Orada kendi sayfasını alır ve `Kaynak beyanı` rozetiyle, karesiz yazılır.
Bölüm 6 zaten canlı kanıtı en zayıf bölüm; ağırlığın oraya kayması doğrudur.

### Kapı

`ortak/b2.py`'ye eklenir: bir bölümün kapsam tablosunda `kaynak` veya `beyan` rozetiyle
sayılan ürün, o bölümün gövdesinde **en az bir kez** geçmek zorundadır — tabloda,
kaynak bloğunda veya kendi sayfasında. Geçmiyorsa kapsam tablosundan da çıkarılır.
Bölüm 1'in hatası tam olarak buydu: kapsamda sayıldılar, gövdede yoklar.

---

## 8 · K12 · Belge 1'in devrettiği söz: üç ürünün kaynak incelemesi

**K11'i düzeltir.** Kullanıcı Belge 1 hazırlanırken verilen kararı hatırlattı: KolayBi
dışındaki üç ürünün **iç arayüzü olmadığı** ve Belge 1 arayüz belgesi olduğu için,
onların **kaynak incelemesi Belge 2'ye devredildi** — Belge 2 daha teknik olacağı için.
K11'in "tek satırlık kaynak bloğu" önerisi bu sözü karşılamıyor.

### Malzeme zaten hazır ve Belge 2'nin tam konusu

Üç gözlem formunun her birinde `Sistem işleyişi / pipeline` başlığı ve **kurulmuş akış
şemaları** var:

| Ürün | Formda ne var |
|---|---|
| **Paraşüt** | Beş gider kaydı türü · **kayıt ile ödemenin ayrı adım olması** · üç cari borçlandırma yolu · otomatik mahsuplaştırma (en gecikmiş faturadan) · tahsilat akışı · tekrarlayan gider ve tekrarlayan fatura ayrı akışlar · KDV · iki tam pipeline şeması (satış→tahsilat, gider) |
| **Logo İşbaşı** | Kaynak özellikleri, entegrasyonlar, **müşavir portalı**, sesli fatura, pipeline bölümü |
| **QuickBooks** | **İşlem başına `Type` = Business/Personal** · `Split transaction` · `Rules` motoru · `Exclude` (silmez) · Schedule C eşlemesi · tam pipeline şeması · beş akışın adım adım yeniden kurulumu |

Bunlar yüzey notu değil, **model bilgisi** — ve Belge 2'nin sorduğu sorunun ta kendisi.

### Bulunan asıl şey: kayıt bir satır mı, bir belge mi

Canlı beş ürün **defterdir**: kayıt bir satırdır ve yazıldığı anda bakiyeyi değiştirir.
Paraşüt, KolayBi ve Logo **belge motorudur**: kaydı fatura/fiş doğurur, cari borç o anda
oluşur, **ödeme ayrı bir adımdır** ve kasa/banka ancak o adımda kıpırdar.

Bu, Belge 2'nin Bölüm 1'de sorduğu soruya bambaşka bir cevaptır ve tek satıra sığmaz.
QuickBooks ise bu üçlüye de benzemiyor — o bir **vergi ekseni sınıflandırıcısıdır**.

### Karar

**1. Üçlü için kendi bölümü açılır — ama "artıklar" bölümü değil, tezli bir bölüm.**

> **Bölüm 9 · Kaydın yerine belge geçince ne değişiyor**
> Paraşüt · KolayBi · Logo İşbaşı — kaynaktan kurulan akış modeli.

Bölüm, canlı beşle **aynı dört adımlı akışı** kullanır (kayıt → hangi deftere → toplama
ne girer → yanlışsa ne olur) ama cevapları kaynaktan gelir ve her satır `Kaynak beyanı`
veya `Kaynak görseli` rozeti taşır. Böylece okur aynı iskelette iki dünyayı karşılaştırır.

**2. QuickBooks bu bölüme girmez.** Bölüm 6'da (Sınıflandırma) kendi sayfasını alır:
canlı beş üründe kayıt düzeyinde işletme/şahsi alanı hiç bulunamadı, tek örnek odur.

**3. Konu bölümlerinde ne kalır.** KolayBi **yüzey** olarak içeride kalır (kare basılır,
iddiası "formda şu alan var"). Paraşüt ve Logo konu bölümlerinde yalnız o konuda
kaynaklarının söylediği **doğrudan karşılaştırılabilir** bir şey varsa, kısa bir
"Kaynakta ne yazıyor" bloğunda geçer; yoksa hiç geçmez — asıl yerleri Bölüm 9'dur.
Bu, K11'in 2. maddesini iptal etmez, **ikincil** hâle getirir.

**4. Bölüm 9 "görülmedi" bölümü değildir.** Ne ölçülemediği bölümün girişinde **bir kez**
söylenir (kanıt tavanı), sonra bir daha tekrarlanmaz. Bölümün işi kaynağın anlattığı
modeli kurmak ve canlı beşle **yapısal farkı** göstermektir.

### Uyarı — sızıntı riski

Üç formun da içinde **"BusinessFinance karşılığı"** sütunları var (ör. *"Bizde bir kaydın
tek kapsamı var; bölme yok"*, *"Bizde vergi hesaplanmaz — ADR 0016"*). Bunlar **Belge
3'ün malzemesidir** ve Belge 2'ye kopyalanamaz. Bölüm yazılırken bu sütunlar atılır;
karşılık kurulacaksa "Belge 3'e taşınan soru" kutusuna soru olarak yazılır.

`b2.py`'ye kapı eklenir: metinde `ADR`, `BusinessFinance`, `TransactionScope`,
`CounterpartyCharge` gibi kendi kavramlarımız geçemez.

### Belge 1'e düşen (Belge 2 bittikten sonra)

Belge 1 bu üç ürünü zaten anıyor (Paraşüt 39, Logo 34, QuickBooks 32 kez) ama iç
arayüzleri yok. İki belgenin kenetlenmesi için Belge 1'in ilgili yerlerine **tek satırlık
gönderme** düşer: *"iç arayüzü görülmedi; kaynaktan kurulan akış modeli Belge 2 Bölüm
9'dadır."* Bu, Belge 1 düzeltme turunun iş listesine yazıldı.

### Yeni bölüm dizisi

| # | Bölüm | Değişiklik |
|---|---|---|
| 1 | Gelir ve gider kaydı | KolayBi sütunu eklenecek |
| 2 | Hesaplar ve para aktarımı | |
| 3 | Kart | KolayBi kart formu |
| 4 | Borç, cari ve tahsilat | KolayBi en güçlü burada |
| 5 | Zaman: tekrar, plan, bütçe | |
| 6 | Sınıflandırma | **+ QuickBooks sayfası** |
| 7 | Raporun neyi saydığı | |
| 8 | Veri ve dışa aktarma | |
| **9** | **Kaydın yerine belge geçince ne değişiyor** | **YENİ** — Paraşüt · KolayBi · Logo |
| 10 | Ürünlerin ayrıldığı yer — kapanış | Bölüm 9'u da toplar |

---

## 9 · Kararların tam listesi ve Belge 1'e taşınacak olanlar

**Neden bu liste var.** Belge 2 yeniden kurulurken alınan kararlar bu belgenin içinde
K1–K12 olarak dağınık duruyordu; bir kısmı da sohbette karara bağlandı ve hiçbir yerde
yazılı değildi. Belge 1'in düzeltme turu geldiğinde hangi kuralın oraya da geçeceği
tek tek aranmasın diye hepsi burada toplandı.

**Ölçüt:** bir kural Belge 1'e ancak Belge 1'in kendi işine aykırı değilse taşınır.
Belge 1 **arayüzü** anlatıyor (ekran argümandır), Belge 2 **sonucu**. Bu yüzden kare
politikası gibi bazı kararlar Belge 2'ye özgüdür.

| # | Karar | Nereden | Belge 1'e |
|---|---|---|:---:|
| **A. Belgeye ne girer, ne girmez** ||||
| A1 | **Üretim tarihi belgeye girmez.** Koşum tarihleri, "20 Eylül'de koşuldu" gibi ifadeler ve kapaktaki üretim tarihi çıkar. Tek istisna **verinin dönemi** (Ağustos 2026) — o sayıyı değiştirir. Ayrı koşum "ayrı bir koşumda uçtan uca izlendi" diye yazılır | Sohbet · 21 Eylül | **Evet** |
| A2 | **Koşum artığı dil çıkar.** Hücre başına koşum künyesi, paket/sürüm notu, "hangi yüzey tarandı" cümlesi metinden çıkar; kanıt satırına veya eke iner. "Sınır." ve "Dayanak." sabit etiketleri kalkar | K8 | **Evet** |
| A3 | **Kendi kavramlarımız metne giremez.** Kendi ürün adımız, ADR numaraları, kendi sınıf adlarımız Belge 1–2'de geçmez; gözlem formlarındaki "bizde karşılığı" sütunları kopyalanmaz. Karşılık kurulacaksa Belge 3'e soru olarak taşınır | K12 · G7 | **Evet** |
| A4 | **"Görülmedi" metinden çıkar, eksik listesinde yaşar.** Metne yalnız iddiayı doğrudan sınırlayan durum girer, cümle içinde, ayrı blok değil. Hiç kanıtı olmayan konu açılmaz. Zorunlu "Bilinmeyenler" bloğu ve "Çıkarılmayan sonuç" satırı kalkar | K3 | **Evet** |
| A5 | **Kanıt düzeyi rozeti kalır.** Kaybolan şey belirsizliğin *anlatılması*, kanıt düzeyinin *işaretlenmesi* değil | K3 | **Evet** |
| **B. Okurun karşısına ne çıkar** ||||
| B1 | **Giriş tek sayfa.** Okuma kılavuzu, sabit sözlük, karışan terimler, ortam/veri/koruma kuralı yöntem ekine iner. Kalan: üç cümle + ürün×erişim tablosu | K2 | **Evet** |
| B2 | **Üretim kısaltmaları başlıklara sızmaz.** "A kısmı", "C kısmı", "işleyiş", "olay defteri", "matris" gibi aramızda konuştuğumuz terimler belgede geçmez; başlık okurun dilindedir | K5 · G4 | **Evet** |
| B3 | **Rozet yalnız istisnada.** Canlı kare varsayılandır ve rozet taşımaz; rozet kaynak/beyan/görülmedi/çıkarım hücrelerinde durur. Rozetsiz hücre de iddia tablosuna girer | Bölüm 1 gözle inceleme | **Evet** |
| **C. Belgenin işi ne** ||||
| C1 | **Dil dört katman:** gözlem (karede okunan) · **çıkarım** (kanıttan çıkan neden, ayrı rozetle) · **değerlendirme** (ne kazandırıyor / ne kaybettiriyor) · karar (Belge 3'te kalır). Çıkarım ve değerlendirme artık serbest, karar değil | K1 | **Evet** |
| C2 | **Her çıkarım dayanağını taşır.** Gözlem formunda yazılı olmayan bir çıkarım basılmaz; çıkarım serbestliği kanıtı gevşetmez | K1 · G2 | **Evet** |
| C3 | **Her bölüm Belge 3'e soru bırakır.** 1–3 madde, soruyu netleştirir, kararı vermez | K7 | **Evet** |
| C4 | **Kapanış üç şey:** ne ayrışıyor · neden · ne kazandırıyor/kaybettiriyor | K7 | **Evet** |
| **D. Kanıtın kullanımı** ||||
| D1 | **Ağırlık kanıt dizininden sayılır**, plandan elle atanmaz. Bir konuda en çok karesi olan ürün o konunun derin ürünüdür | K4 | **Evet** |
| D2 | **Kapsamda sayılan ürün gövdede geçer.** Geçmiyorsa kapsam tablosundan da çıkar | K11 · G8 | **Evet** |
| D3 | **Hangi kare en uygunsa o kullanılır** — başka belgede basılmış olması engel değil. Belge 1 arayüz öğesini, Belge 2 sonucu anlatır; aynı kare iki farklı iş görebilir | Sohbet · 21 Eylül | **Evet** |
| D4 | **Kaynak ürünler kanıt tavanına göre ayrılır:** ekranı olan (KolayBi) konunun içine girer ve karesi basılır, yalnız metni olan üçü kendi bölümünde kurulur ve konu bölümlerinde kısa bir kaynak bloğunda geçer | K11 · K12 | **Evet** |
| **E. Belge 2'ye özgü — Belge 1'e taşınmaz** ||||
| E1 | Kare politikası: Belge 2 sonucu göstermek için basar (önce/sonra çifti), bölüm başına en az 12 | K6 | Hayır — Belge 1 zaten kare belgesidir |
| E2 | Bölümde tek **akış** sayfası: ortak yol + ayrıldığı noktada çatallanma, çıkarımdan önce | Sohbet · G6 | Hayır — akış Belge 2'nin sorusudur |
| E3 | Çıkarım sayfası (mekanizma + kazanç/kayıp + Belge 3 sorusu) | K7 · G1 | Kısmen — C1/C3/C4 olarak taşınıyor |
| E4 | Bölüm düzeni: 10 bölüm, 9. bölüm kaynak üçlüsü | K12 | Hayır |

### Belge 1 düzeltme turunun iş listesi

Yukarıdaki "Evet"lerin Belge 1'de ne anlama geldiği:

1. **Tarih taraması** — Belge 1'in metninde ve kapağında üretim/koşum tarihi var mı, çıkar (A1).
2. **"Görülmedi" sayımı** — Belge 2 v1'de kelimelerin %30'uydu; Belge 1'de ölçülmedi. Önce sayılır, sonra A4'e göre eksik listesine indirilir.
3. **Kendi kavramlarımız taraması** — A3'ün kapısı (G7) Belge 1'in üretimine de eklenir ve bir kez çalıştırılır.
4. **Kapsam–gövde tutarlılığı** — D2'nin kapısı (G8) Belge 1'e eklenir; kapsamda sayılıp gövdede olmayan ürün var mı.
5. **Üç kaynak ürünün göndermesi** — Paraşüt, Logo ve QuickBooks'un iç arayüzü Belge 1'de yok; ilgili yerlere *"kaynaktan kurulan akış modeli Belge 2 Bölüm 9'dadır"* göndermesi düşer (K12).
6. **Rozet yoğunluğu** — B3: Belge 1'in tablolarında her hücrede rozet var mı, varsa yalnız istisnaya indirilir.
7. **Giriş bölümleri** — B1'in Belge 1'deki karşılığı ölçülür; Belge 1'in okuma kılavuzu kendi işine gerekli olabilir, bu yüzden kesmeden önce ölçülür.

**Sıra:** bu liste Belge 2 bitip beğenildikten sonra uygulanır. Belge 2 iyi çıkmazsa
kurallar da yeniden tartışılır — bu yüzden Belge 1'e şimdi dokunulmuyor.

---

## 10 · K4 düzeltmesi — %60 eşiği tutmadı, nedeni ölçüldü

**Üretim sonrası kayıt.** K4'e koyduğum kural şuydu: *"bir konuda 10'dan fazla karesi olan
ürünün karelerinin en az %60'ı ya metinde anılmış ya eksik listesinde gerekçesiyle yazılmış
olmalı."* On bölüm bittiğinde ölçüldü ve **tutmadı**.

| Ürün | Envanter | v1 | v3 | v1 % | v3 % |
|---|---:|---:|---:|---:|---:|
| Money Manager | 44 | 32 | 36 | %73 | **%82** |
| KolayBi | 39 | 17 | 26 | %44 | **%67** |
| Hesap Defterim | 45 | 26 | 25 | %58 | %56 |
| Goodbudget | 39 | 15 | 22 | %38 | **%56** |
| Bluecoins | 92 | 29 | 37 | %32 | **%40** |
| **Wallet** | **109** | **42** | **39** | **%39** | **%36** |

Anılan toplam kare v1'de 161, v3'te **185**. Ama Wallet geriledi ve hedefe iki üründe
yaklaşılamadı.

### Neden — ölçülmüş cevap

Kullanılmayan kareler tek tek tarandı:

- **Wallet'ın 70 kullanılmayan karesinin 20'si süreç artığı.** Kanıt dizininin kendi
  açıklamaları bunları "Arşiv", "adına karşın … değil" ya da "giriş artığı" diye işaretliyor:
  otomasyonla yanlış girilmiş tutarlar (86.400 · 864.006.666.666 · 42.222.110.000), geri dönüş
  kontrolleri, tekrar denemeler.
- **Kalan 50 kare ise gerçek ekran** — ama çoğu ayar sayfası, çekmece, kategori seçici ve
  gezinme kontrolü. Bunlar **Belge 1'in sorusuna** cevap veriyor: ekranda ne var. Belge 2'nin
  sorusu "paraya ne oluyor" ve o soruya cevap vermiyorlar.
- Bluecoins'te oran benzer: 55 kullanılmayan karenin 8'i artık, kalanların çoğu ayar ve
  gezinme karesi.

### Eşiğin yerine geçen kural

Yüzde hedefi yanlış şeyi ölçüyordu: bir belgenin kalitesi kaç kare andığıyla değil, **iddia
taşıyan hiçbir karenin dışarıda kalmamasıyla** ölçülür. K4'ün eşiği kaldırılır ve yerine şu
geçer:

> **Bölüm kapanmadan önce, o konuya ait bütün kareler tek tek okunur ve "bu kare bir iddia
> taşıyor mu" diye sorulur. Taşıyorsa girer; taşımıyorsa hiçbir şey yapılmaz.** Yüzde
> hedefi yoktur.

### Bu tarama iki olgusal hata buldu

Kuralı geriye dönük uygulayınca Bölüm 4'te iki yanlış cümle çıktı ve düzeltildi:

1. **"Wallet'ta tahsilat bu koşumda çalıştırılmadı."** Yanlış. `E0367`→`E0368`→`E0369`→`E0370`
   zinciri tahsilatı uçtan uca gösteriyor: *Choose how to add Record* → *Create Debt Record,
   Debt action **Repay debt***, yer tutucu *12.000,00 to Repay debt* → tutar 5.000 → borç
   **12.000'den 7.000'e** indi ve diğer kart değişmedi.
2. **"Borç kaydının gelir/gider toplamına etkisi ölçülmedi."** Yanlış. `E0398` son otuz günün
   Cash-flow'unu gösteriyor: Income 5.000 · Expenses −21.600 · net −16.600, ve açıklaması
   *"Record taşıyan borç kayıtları gelir/gidere dahil"* diyor. `E0372` kategorileri veriyor:
   tahsilat **Lending, renting +5.000**, borç verme **Loan, interests −5.000**.

İkincisi bölümün en güçlü bulgusunu da getirdi: **alacağı hesap yapan ürün ile borcu ayrı
nesne yapan ürün raporda taban tabana zıt sonuç veriyor.** Birinde tahsilat bir aktarım ve
gelir/gidere hiç dokunmuyor; ötekinde borç kayıtları kategori sisteminden geçip dönem
raporuna giriyor.

Bölüm 4 bu düzeltmeyle 14 kareden **18 kareye** çıktı ve yeni bir çıkarım bloğu kazandı.
Aynı tarama Bölüm 5'e iki kare (kapının sonradan değiştirilebilmesi, Postpone/Dismiss),
Bölüm 6'ya bir kare (etiket yönetimi) ve bir düzeltme (bölme yalnız QuickBooks'ta değil —
Wallet ve Bluecoins'te de var, ama kapsamsız), Bölüm 8'e bir kare ve **bir yanlış dayanak
düzeltmesi** ekledi: dışa aktarma hücresi *içe aktarma* karesiyle kanıtlanmıştı.

---

## 11 · K13 · Bölüm 9'un kapsamı — belge mi, ön muhasebenin tamamı mı

**Neden açıldı.** Bölüm 9 yazıldıktan sonra içeriği okundu ve iki şey görüldü:
bölüm kendi başlığının dışına çıkmış durumda, ve kaynak formlarda hiçbir bölüme
girmeyen gerçek bir ön muhasebe malzemesi duruyor. Karar verilmeden bölüm
kapanmıyor.

### Bulgu 1 — bölüm zaten başlığının dışında

Başlık *"Kaydın yerine belge geçince ne değişiyor"*. Ama **9.4 (Muhasebeci
nerede duruyor)** kayıt/belge sorusuyla ilgili değil: müşavir portalı, canlı
erişim, davet ve personel carisi verinin **kime açıldığını** anlatıyor. Bölüm
fiilen genel bir ön muhasebe bölümü olmaya başlamış; başlığı bunu söylemiyor.

### Bulgu 2 — hiçbir bölüme girmeyen malzeme

| Konu | Kaynakta ne var | Kanıt | Belgede |
|---|---|---|---|
| **KDV** | KolayBi: oran sütunları %20/%18/%10/%8/%1/Diğer, her birinde Matrah + Tutar; üst blok Satış Faturası + Alış İade → `Hesaplanan KDV Toplam`; `Matrahlı` anahtarı; satır başlarında `+` genişletici. Ayrıca Alış/Satış Raporu'nda `KDV Dahil` anahtarı — aynı rapor brüt/net okunabiliyor. Paraşüt: ay bazında `Hesaplanan` / `İndirilecek` / `Net KDV`, aya tıklayınca döküm | E0214 **basılabilir** · E0213 basılabilir · Paraşüt kaynak beyanı | **Yok.** Tek geçiş Bölüm 8'de bir yan cümle |
| **Stok / ürün-hizmet** | KolayBi: Tümü / Ürünler / Hizmetler / **Depolar** / **Varyantlar** sekmeleri. Logo: tek kayıt cari + kasa-banka + **stok**'u birlikte güncelliyor | E0218 basılabilir **ama kare neredeyse boş** (yalnız sekme ve düğme adları) | Yalnız 9.6'daki "üç defter" çıkarımında anılıyor |
| **Sesli fatura** | Logo'nun kendi duyurusu: fatura bilgileri butonlarla değil sesle kaydediliyor. Dokuz üründe benzeri görülmeyen tek özellik | Kaynak beyanı, kare yok, çalışırken görülmedi | **Yok** |
| **Güncel Durum panosu** | Üç sekme, son bir haftalık nakit akışı grafiği, Günü Gelen İşlemler, Tahsilat/Ödeme Özetleri | E0211 basılabilir ama karenin büyük kısmı kampanya şeridi | **Yok** |
| **Şube** | Alış/Satış Raporu'nda proje ve etiketin yanında dördüncü bir filtre boyutu | E0213 | Yalnız 7.6'nın bir notunda |
| **Personel cari / maaş** | Ayrı sekme, maaş/prim/avans ödeme menüsü, tipler | E0202–E0204 basılabilir | 4.5 ve 5.6'da not olarak var, karesi basılmıyor |

Ağırlık tek bir yerde: **KDV**. Üç kaynak ürünün üçü de vergi tarafında bir şey
yapıyor, canlı beş üründe kavram hiç yok, ve elde basılabilir bir kare var.
Bu, belgenin şu an sessiz kaldığı en keskin ayrışma.

### Seçenekler

| | Seçenek | Sonuç |
|---|---|---|
| A | Bölüm 9 olduğu gibi kalsın | KDV belgeye hiç girmez — 1–8 arasında yeri yok, 10 sentez bölümü yeni kanıt almıyor. Ayrışmanın en keskin yeri belgede yok sayılmış olur. 9.4'ün başlık dışılığı da sürer |
| **B** | **Bölüm 9 genişler, tezi korunur** (öneri) | Aşağıda |
| C | Bölüm 9 aynı kalır, ön muhasebenin kalanı için 10. bölüm açılır, kapanış 11 olur | Bölüm sayısı artar, iki bölüm aynı üç ürünü anlatır, kapanış tablosu (10.1) yeniden numaralanır. K12'nin "artıklar bölümü açma" uyarısına en yakın seçenek |

### B'nin gerekçesi — bunlar artık değil, tezin sonucu

Bölümün tezi *"kayıt bir satır değil bir belge"*. KDV, stok ve müşavir bu tezin
**sonuçları**: vergi belgenin üstünde yaşar (oran ve matrah faturanın kalemidir),
stok belgenin kalemlerinden düşer, müşavir belgeleri işleyen kişidir. Üçü de
bölüme "kalanlar" olarak değil, aynı mekanizmanın devamı olarak girer — K12'nin
*"artıklar bölümü değil, tezli bir bölüm"* şartı korunur.

**Eklenen bölümler:**

1. **Vergi belgenin üstünde yaşıyor** — E0214 basılır, Paraşüt kaynak beyanıyla
   yanına konur. Canlı beş üründe vergi alanı hiç bulunmadı; bu bir yokluk
   tespiti değil, iki ürün ailesinin farklı soruya cevap vermesi.
2. **Belge kalem taşıyor: stok ve ürün-hizmet** — E0218 zayıf olduğu için kare
   basılmayabilir; Logo'nun "tek kayıt üç defteri besliyor" beyanı bu bölümün
   asıl dayanağı. Kare basılırsa iddiası yalnız "sekmeler şunlar" olur.
3. **Sesli fatura** 9.1'in içinde bir kaynak bloğu olur — kaydın nasıl girildiği
   sorusunun devamı, ayrı bölüm değil.

**Alınmayanlar:** Güncel Durum panosu (karenin ağırlığı kampanya şeridi),
personel cari kareleri (iddiası zaten notlarda taşınıyor), Şube (Bölüm 6'nın
konusu, oraya bir not olarak düşer).

**Başlık.** Tez korunduğu için başlık da korunabilir; ama 9.4 ve yeni bölümler
"belge" kelimesinin altına sığmıyorsa alt başlık genişletilir:
*"Kaydın yerine belge geçince ne değişiyor — belge, vergi ve müşavir"*.

**Ölçü.** Bölüm 9 sayfa 4 kareden 5–6 kareye, 9 sayfadan ~12 sayfaya çıkar.
`EN_AZ_KARE` 4'ten 5'e alınır.

### Kapı uyarısı

Üç gözlem formunun KDV satırları **ADR 0016'ya açık atıf yapıyor** (*"bizde
vergi hesaplanmaz"*, *"bu sorumluluğu almamayı seçtik"*). Bu bölüm yazılırken o
sütunlar atılır; ödünleşim Belge 3'e **soru olarak** taşınır:

> Uygulama vergi tutarını hesaplamalı mı, yoksa kullanıcının girdiğini taşıyıp
> raporlamalı mı? Hesaplayan ürün ayın en zahmetli işini üstleniyor; karşılığında
> oran listesini güncel tutma sorumluluğunu ve yanlış sayının mali sonucunu alıyor.

G7 kapısı bunu zaten yakalar, ama yazarken bilinçli olunması gerekir.

### Karar

**Bekliyor.** A, B veya C.

---

## 12 · K14 · Bölüm 10'a ne eklenmeli

**Neden açıldı.** Bölüm 10 "diğer bölümleri toplayan" bölüm. Kendi başına
doygun görünüyor; eksik olup olmadığını anlamak için elimizdeki iki denenmiş
kurguyla karşılaştırıldı: `belge2-v1/bolum-12-baglar/` ve
`belge2-nasil-olabilir/`.

### Bugünkü Bölüm 10

| # | Bölüm | Biçim |
|---|---|---|
| 10.1 | Dokuz bölüm, dokuz ayrım noktası | tablo |
| 10.2 | Üç tekrar eden desen | çıkarım |
| 10.3 | Ürün ürün: neyi iyi yapıyor, neyi bırakıyor | tablo |
| 10.4 | Bir paranın belgedeki yolu | akış |
| 10.5 | Belgenin bıraktığı yer | çıkarım |

Yedi sayfa, kare yok. Dört ayrı sayfa türü kullanılmış; biçim çeşitliliği
açısından eksik değil.

### v1'in 12. bölümünden düşen üç şey

| v1'de vardı | v3'teki karşılığı | Değerlendirme |
|---|---|---|
| **12.5–12.6 · Ürünlerin motoru** — ürün başına sabit beş satır: para nerede yaşıyor · olay ile ödeme ayrılıyor mu · gelecek nasıl tutuluyor · gelir/gider ölçüsü neyi sayıyor · nerede durduk | 10.3 var ama o bir **değerlendirme** tablosu (kazandırdığı / kaybettirdiği), model değil | **Gerçek boşluk.** Belge hiçbir yerde tek bir ürünün modelini uçtan uca kurmuyor |
| **12.1–12.4 · Dört bağ** — aynı sorunun farklı bölümlerdeki parçalarını birleştiren tablo | 10.2'nin üç deseni aynı işi yapıyor (aynı şeyin iki sayısı · kaç defter · kararı kim veriyor) | **Boşluk değil**, örtüşüyor |
| **12.7 · Çözülmemiş yerler + araştırmanın göremedikleri** — ürünlerin kendi içinde tutarsız görünen yerleri (Goodbudget'ın açıklanamayan farkı, ters görünen sütun başlıkları, birleşmeyen borç kartları) ve yöntem sınırları | 10.2'nin içinde "Bu belgenin sınırı" tek blok, üç cümle | **Kısmi boşluk.** Ürün içi tutarsızlık kaydı tamamen düşmüş |

`belge2-nasil-olabilir/` üç kurgu önerip **"A omurga, C derinlik, B senaryo
ekinde"** diye bitiriyordu. v3 baştan sona A (konu ekseni) yürüdü; **C — ürünün
motoru — hiç yapılmadı.** Bu, o kararın yerine getirilmemiş kısmı.

### Seçenekler

| | Seçenek | Sonuç |
|---|---|---|
| A | Hiçbir şey eklenmez | Bölüm 10 doygun; ama belge tek bir ürünün modelini hiçbir yerde kurmamış olarak kapanır |
| **B** | **Ürünün motoru geri gelir** (öneri) | Aşağıda |
| C | Bölümler arası bağ tablosu eklenir | 10.2 ile örtüşür; **önerilmez** |
| **D** | **Çözülmemiş yerler ve göremedikler kendi sayfasına çıkar** (öneri) | Aşağıda |

### B · Ürünün motoru

Beş canlı ürün için sabit beş satır, ürün başına bir blok. Sabit biçim,
blokların yan yana okunabilmesi için:

```
Para nerede yaşıyor        Olay ile ödeme ayrılıyor mu
Gelecek nasıl tutuluyor    Gelir/gider ölçüsü neyi sayıyor
Nerede durduk
```

Kaynak üçlüsü Bölüm 9'da kendi modelini zaten aldı; burada yalnız bir satırla
anılır. Yeni kanıt getirmez — her satır ilgili bölüme dayanır, kare basılmaz.

**Neden değerli:** Belge 3 karar verirken "şu ürün şunu şöyle yapıyor" diye
tek tek bölüm aramak zorunda kalmaz; dokuz bölümün ürün başına okunmuş hâli
tek yerde durur. 10.3 "iyi/kötü" der, bu "nasıl çalışıyor" der.

**Bedeli:** 2 sayfa (+7 → 9). 10.3 ile sıra sorusu doğar — önerim motorun
10.3'ten **önce** gelmesi: önce model, sonra o modelin bedeli.

### D · Çözülmemiş yerler ve göremedikler

Bugün 10.2'nin içinde üç cümle. Kendi sayfasına çıkarsa iki liste olur:

1. **Ürünlerin kendi içinde çözülmemiş görünen yerleri** — kusur listesi değil,
   gözlenen tutarsızlıkların kaydı. v1'de dört madde vardı ve v3'e hiç
   taşınmadı.
2. **Bu araştırmanın göremedikleri** — fiziksel şema, dosya içerikleri, ağ
   davranışı, ücretli paketler, denenmeyen dallar, borcun tam kapanışı.

**Neden değerli:** belge kendi sınırını tek yerde ve açıkça söyler; okur
"bu neden yok" diye aramaz. Eksik listeleri bölüm bölüm duruyor ama
**yöntem sınırı** hiçbir yerde toplu yazılı değil.

**Bedeli:** 1 sayfa (+1). Kare yok.

### Ölçü

B + D uygulanırsa Bölüm 10: 7 → **10 sayfa**, kare yine 0. Belge 103 → 106.

### Karar — uygulandı

**B + D**, D daraltılarak. Kullanıcı kararı bana bıraktı (22 Eylül).

**D neden daraldı.** v1'in "ürünlerin kendi içinde çözülmemiş yerleri" listesi
v3'e taşınmadı, çünkü v3 onları **çözmüş**: Goodbudget'ın "açıklanamayan farkı"
1.8'de tam olarak hesaba yazılmayan kayıtların toplamı diye kanıtlandı
(2.050 → 3.284, tam 1.234 artış); Wallet'ın birleşmeyen borç kartları zaten
4.7'de duruyor; Money Manager'ın ters görünen sütun başlıklarının v3'te kanıtı
yok. Kanıtı olmayan bir listeyi geri getirmek belgeyi zayıflatırdı. Geriye
**yöntem sınırı** kaldı ve o gerçekten hiçbir yerde toplu yazılı değildi.

**Uygulanan yeni düzen:**

| # | Bölüm | Durum |
|---|---|---|
| 10.1 | Dokuz bölüm, dokuz ayrım noktası | Dokuzuncu satır yeni Bölüm 9'a göre yazıldı |
| 10.2 | Üç tekrar eden desen | "Bu belgenin sınırı" bloğu 10.6'ya taşındı |
| **10.3** | **Beş ürünün motoru, aynı beş soruyla** | **YENİ** — 5 soru × 5 ürün tablosu |
| 10.4 | Ürün ürün: neyi iyi yapıyor, neyi bırakıyor | Kaynak üçlüsünün satırları güncellendi |
| 10.5 | Bir paranın belgedeki yolu | Değişmedi |
| 10.6 | Belgenin bıraktığı yer | Vergi ve kalem soruları eklendi (7 → 9) |

Bölüm 10: 7 → **8 sayfa**, kare yine 0. Motor tablosu 10.4'ten **önce** geliyor:
önce model, sonra o modelin bedeli.

### D geri alındı (22 Eylül, kullanıcı kararı)

"Bu araştırmanın göremedikleri" sayfası yazıldı ve **kaldırıldı.** Gerekçe:
kanıt düzeyi zaten her sayfada rozetle yazılı ve her bölümün kendi eksik
listesi var; yöntem sınırını ayrıca bir sayfada toplamak belgeye okur için
yeni bir şey katmıyor, bilgi iç kayıtlarda kalıyor.

10.2'den 10.6'ya taşınan **"Bu belgenin sınırı"** bloğu da yerine döndürüldü —
kaldırma yalnız yeni sayfayı kapsıyor, ondan önce yazılmış olanı değil.

Sonuç: **B uygulandı, D uygulanmadı.**
