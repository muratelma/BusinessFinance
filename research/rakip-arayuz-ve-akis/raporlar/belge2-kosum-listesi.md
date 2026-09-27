# Belge 2 — koşum listesi (Sıfır + Bir)

20 Eylül 2026 · [Bölüm planı](belge2-bolum-plani.md) §7 A1

> **20 Eylül — Sıfır ve Bir tamamlandı.** Kullanıcı emülatörü açıp koşumu agent'a devretti;
> K1, K2, K3 ve S1 aynı gün yapıldı. Sonuçlar §"Koşum sonucu"nda, kareler `E0395`–`E0424`
> kimlikleriyle envanterde, ayrıntı `gozlemler/money-manager.md` ve `gozlemler/goodbudget.md`
> içindeki 20 Eylül bölümlerinde. **Kalan iş: §"İki" ve §"Üç" maddeleri (yazımı durdurmuyor).**

Bu liste **kullanıcının koşacağı** işleri taşır; agent kendiliğinden canlı test başlatmaz
(`MANUEL-TEST-PROTOKOLU.md`, 14 Eylül kullanıcı kararı). 20 Eylül'de kullanıcı bu turu açıkça
agent'a devretti.

**Değişmeyen kurallar:** mevcut test verisi **silinmez, sıfırlanmaz, kontrol değerine
döndürülmez**. Ayar değiştirilirse ölçülür ve geri alınır. Kare dolu ekran gösterir, boş ekran
değil. Yeni kare tam dosya adıyla envantere ve gözlem formuna işlenir.

---

## Sıfır — koşum gerektirmeyen iki iş (agent yapar)

### S1 · Kanıt dizinini yenile · ✔ 20 Eylül’de yapıldı (357 → 387)

**Koşum öncesi durum:** `belge1/ortak/kanit-dizini.json` 357 kare içeriyordu; diskte ve
envanterde 365 vardı. Aşağıdaki sekiz eski kare E0395–E0402 kimliklerini aldı;
22 yeni kareyle birlikte dizin artık **387 kare / 422 kimlik** içeriyor.

| Ürün | Dosya | Nerede kullanılıyor |
|---|---|---|
| Goodbudget | `27-fill-from-new-income.png` · `28-fill-from-available.png` · `29-income-keep-available.png` | GB-U01; Belge 2 Bölüm 3.1 |
| Wallet | `48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png` · `49-u03-records-menu-bakiye-planli-secenekleri.png` · `50-u02-plan-menu-postpone-dismiss.png` | Tema 06/07/08; Belge 2 Bölüm 6.4 ve 7.3 |
| Bluecoins | `işlemler.png` · `ögeler özeti.png` | BC-U01; Belge 2 Bölüm 3.4 |

**Önceki engel kapandı:** Kanıtlar yok değildi; üretim dizininde kimlikleri eksikti.
Motor artık sekiz kareyi de çözüyor. Kanıtın içerik sınırları değişmedi.

**Yapıldı:** Envantere E kimlikleri eklendi, ardından `belge1/ortak/kanit-dizini-uret.py`
yeniden çalıştırıldı. Bluecoins'in `işlemler.png` ve `ögeler özeti.png` adları **korundu**:
denetleyici Unicode ve boşluklu adları destekliyor; dizin/hash doğrulaması geçiyor.
Yeniden adlandırma teknik bir kapanış koşulu değildir.

### S2 · Koşum gerektirmeyen uzlaştırmalar

Plan §7 "Sıfır" maddesi: taksitin 1.000 + 600 ayrımı, koşum tarihi ve filtrenin her iddiaya
yazılması, yokluk ifadelerinin taranan yüzeyle sınırlandırılması, E0022/E0279'un hangi deneye ait
olduğunun belirtilmesi. Bunlar plan v2'de yapıldı; bölüm yazımında tekrar denetlenir.

---

## Bir — üç koşum · ✔ 20 Eylül’de tamamlandı

Aşağıdaki adımlar koşum öncesi protokoldür; gerçekleşen sonuç ve farklı dosya adları
“Koşum sonucu” bölümünde ve envanterde kayıtlıdır.

### K1 · Money Manager · Eylül raporu

**Kapattığı iddia:** Bölüm 5.3'ün gelir/gider satırı — kart borcunun kısmen ödenmesi ürünün gider
toplamına giriyor mu? Şu anda bu hücre "görülmedi".

**Neden bu koşum izole çalışır.** 10 Eylül'den sonra veri değişti: 12 Eylül'de **Ağustos'taki**
₺600 tekrarlayan occurrence silindi (Ana Hesap 39.200 → 39.800). Ama silme Ağustos'u etkiledi;
**Eylül dokunulmadı.** Eylül'ün üç kaydı duruyor:

| Tarih | Kayıt | Hesap | Beklenen tür |
|---|---|---|---|
| 10 Eyl | Abonelik ₺600 (tekrarlayan) | Ana Hesap | Gider |
| 10 Eyl | Kart ödemesi ₺400 (Havale) | Ana Hesap → Is Karti | **Havale — nötr olması bekleniyor** |
| 15 Eyl | Tasarim ekipmani (2/6) ₺1.000 | Is Karti | Gider (kart harcaması) |

**Karar kuralı:**

- **Eylül Gider = ₺1.600** → ödeme gider toplamına **girmiyor**; 5.3'ün satırı kapanır
- **Eylül Gider = ₺2.000** → ödeme gider yazılıyor; bu, Ağustos'taki nötr davranışla çelişir ve
  ayrıca incelenir
- **Başka bir sayı** → Eylül'de beklenmeyen kayıt var; önce adım 1'deki liste karesine bakılır

**Adımlar**

1. **İşlemler** sekmesi → dönemi **Eylül 2026** yap → **kare**
   `33-eylul-islem-listesi.png`
   *Bu kare koşumun kendi denetimidir: Eylül'de hangi kayıtların bulunduğunu gösterir.*
2. **İstatistik** sekmesi → **Ay** → **Eylül 2026** → **Gider** sekmesi seçili → **kare**
   `34-istatistik-eylul-gider.png`
3. Aynı ekranda **Toplam** sekmesi → **kare**
   `35-toplam-sekmesi-eylul.png`
   *Bu sekme gideri kaynağa göre bölüyor ve kart satırında parantez içinde o dönem ödenen tutarı
   veriyor (Ağustos'ta `₺2.200,00(₺1.200,00)` idi). Eylül'de beklenen: kart satırı
   `₺1.000,00(₺400,00)`, Havale satırı `₺0,00`. Ödemenin nereye yazıldığını en net bu kare gösterir.*

**Yeterlilik koşulu:** Üç kare birlikte alınmalı. Yalnız İstatistik karesi yeterli değildir —
Eylül'de başka kayıt olup olmadığı görünmeden toplam yorumlanamaz.

**Yapılmayacak:** Hiçbir kayıt silinmez, düzenlenmez, eklenmez. Dönem ayarı değiştirilmez.

---

### K2 · Money Manager · kalan dört taksit nerede

**Kapattığı iddia:** Bölüm 5.2 ve 12.3 — "incelenen Bu Ay/Gelecek Ay toplamlarında sonraki dört
taksit görünmüyor" cümlesi şu anda **nerede göründüklerini** söyleyemiyor (MM-Q03).

**Bu bir tarama koşumudur.** Bulunursa kare alınır; bulunmazsa **hangi yüzeylerin tarandığı**
yazılır — "yok" değil, "şu beş yüzeyde görülmedi" denir.

**Taranacak yüzeyler**

| # | Yüzey | Nasıl açılır | Aranan |
|---|---|---|---|
| 1 | İşlem formu → **Tekrar/Taksit** açılır menüsü | Yeni işlem → sağ üst | Menünün kendisi. **Karesi hiç yok** (koşum notu); açıldığında kare al: `36-tekrar-taksit-menusu.png` |
| 2 | **Ekim 2026** İşlemler ekranı | İşlemler → dönemi Ekim yap | Ağustos/Eylül'de görülen "Tekrarlama" önizleme bölümü var mı; **taksit 3/6 orada mı** → `37-ekim-onizleme-taksit.png` |
| 3 | **Is Karti** kart defteri, Ekim dönemi | Hesaplar → Is Karti → dönemi Ekim yap | Gelecek taksitin kart defterinde görünüp görünmediği → `38-kart-defteri-ekim.png` |
| 4 | Alt sekme **Daha** | Alt gezinme → Daha | Taksit/tekrar listesi girişi var mı |
| 5 | **Ayarlar** ızgarası | Daha → Ayarlar | Tekrarlı/taksitli işlem yönetimi girişi var mı *(ızgaranın karesi zaten var: E0254)* |

**Yeterlilik koşulu:** Beş yüzeyin **hepsi** açılmalı. Bulunmazsa sonuç yine değerlidir: cümle
"beş yüzey tarandı, görülmedi" diye yazılır — Hesap Defterim'in plan taraması gibi.

**Yapılmayacak:** Yeni taksitli işlem kurulmaz. Var olan taksit düzenlenmez veya silinmez.

---

### K3 · Goodbudget · normal gelir yolunun sonucu

**Kapattığı iddia:** Bölüm 3.1 — gelir anlatımının yalnız Expense/Credit yoluna dayanması (GB-Q01).

**Yarısı zaten yapılmış.** GB-U01 (14 Eylül) formu kareye aldı: `27-fill-from-new-income.png`
(From New Income formu: Received from, How to fill Envelopes, Amount, Account, Date, Schedule),
`28-fill-from-available.png`, `29-income-keep-available.png`. **Eksik olan formun kendisi değil,
kaydın sonucu.**

**Adımlar**

1. **Önce** — iki kare:
   - Ana ekran (zarflar ve kalanları) → `30-oncesi-zarflar.png`
   - Accounts ekranı (hesap bakiyesi) → `31-oncesi-hesap-bakiyesi.png`
2. Fill Envelopes → **From New Income** → doldur:
   - Received from: sentetik bir ad (ör. `Beta Tasarim`)
   - **Amount: ₺1.234** — kasıtlı olarak tuhaf bir sayı; sonraki toplamlarda izini sürmek için
   - Account: mevcut tek hesap · Date: bugün · Schedule: yok
   - How to fill Envelopes: **Fill Each Envelope** → tutarı bir zarfa yaz
   - Kaydetmeden → `32-gelir-formu-dolu.png`
3. Kaydet → **Sonra** — üç kare:
   - Ana ekran (zarflar) → `33-sonrasi-zarflar.png`
   - Accounts → `34-sonrasi-hesap-bakiyesi.png`
   - Transactions listesi (yeni satır nasıl görünüyor, türü ne) → `35-sonrasi-islem-satiri.png`
4. Reports → **Income vs Spending** → bu ayı seç → `36-rapor-income-vs-spending.png`

**Karar kuralı:** Bu yolun gelir kaydı
**(a)** hesap bakiyesini artırıyor mu, **(b)** zarf kalanını artırıyor mu, **(c)** raporda Income
olarak mı görünüyor — üçü ayrı ayrı okunur. Expense/Credit yolunda gelir **zorunlu bir zarfa**
bağlanıyordu; bu yolda bağlanmak zorunda mı, yoksa "Keep Available" ile dağıtılmadan durabiliyor mu.

**Yeterlilik koşulu:** Önce/sonra çiftleri olmadan koşum sayılmaz. Yalnız formu doldurup
kaydetmeden çıkmak bu iddiayı kapatmaz — zaten GB-U01'de o yapıldı.

**Uyarı:** Bu koşum **veri ekler**. Hesap bakiyesinin +1.234 ile 42.968 olması yalnız
sınanacak beklentidir; zorunlu sonuç değildir. Gerçek koşumda hesap 41.734'te kaldı,
zarf toplamı arttı; aşağıdaki sonuç kaydı geçerlidir. **Kayıt sonradan
silinmez** — koşum kaydı olarak kalır ve gözlem formuna yazılır.

---

## Koşum sonucu — 20 Eylül 2026

### S1 · Kanıt dizini yenilendi ✔

`KANIT-ENVANTERI.md`'ye yeni bir bölüm eklendi ve `kanit-dizini-uret.py` yeniden çalıştırıldı.

- **E0395–E0402:** daha önce `GB-U01-*`, `WL-U*`, `BC-U01-*` kodlarıyla duran sekiz kare artık
  E kimlikli. Belgelerde basılabilir hâle geldiler; Bölüm 6.4 (Wallet cash-flow) ve 3.4 (Bluecoins
  geri yükleme) için mevcut kanıtların üretim dizinine erişimi sağlandı; içerik sınırları değişmedi.
- **E0403–E0424:** bu koşumun 22 karesi.
- Dizin **357 → 387 kare** (422 kimlik); "diskte eksik kare yok". Mevcut üretim (`belge2-nasil-olabilir`)
  yeniden koşuldu, mevcut motor kontrolleri temiz. Belge 2 için planda tarif edilen etki-boyutu
  kapısı bu kurgu denemesinin kontrolü değildir; uygulanmış sayılmaz. Özgün karelerin hash'i doğrulandı.
- Bluecoins'in `işlemler.png` ve `ögeler özeti.png` adları korundu. Türkçe/boşluklu adlar
  denetleyici testleri ve üretim dizini tarafından destekleniyor; teknik engel yok.

### K1 · Money Manager — kısmi ödemenin gider etkisi ✔ **kapandı**

**Eylül Gider ₺1.600,00** — yani karar kuralının "ödeme gider toplamına girmiyor" dalı.
Toplam sekmesi bunu daha net gösteriyor: `Gider (Nakit, Banka Hesapları) ₺600,00` ·
`Gider (Kredi Kartı, Ödeme) ₺1.000,00(₺400,00)` · `Havale ₺0,00`. Ödeme yalnız kart satırının
parantezinde "o dönem ödenen" olarak görünüyor. Kanıt: `E0403`, `E0404`, `E0405`.

Plan Bölüm 5.3'ün Money Manager gelir/gider hücresi artık "görülmedi" değil.

### K2 · Money Manager — kalan taksitler (MM-Q03) ✔ **kapandı**

Beş yüzey tarandı. Kalan taksitler **gelecek ayların işlem listesinde görünür satırlar**:
arka planda ayrı plan nesnesi olup olmadığı bilinmiyor (B12). 3/6 Ekim, 4/6 Kasım, 5/6 Aralık, 6/6 Ocak 2027; her biri o ayın Gider
toplamında (₺1.000). Şubat 2027 işlem alanında "Veri yok", toplam sıfır; Tekrarlama önizlemesi sürüyor.

**Tekrarlayan plan bunun tersi davranıyor:** her ayda ayrı bir `Tekrarlama` önizleme bölümünde
(₺−600) duruyor ve ayın toplamına **girmiyor**. Aynı üründe iki farklı gelecek-kayıt davranışı.

**İki yüzey farklı dönem kapsamını gösteriyor** (aynı koşum, aynı kart; 11:05 ve 11:07): Hesaplar → Borçlar **₺1.600**
(Bu Ay 600 + Gelecek Ay 1.000) · Is Karti defteri Ocak 2027 yürüyen Bakiye **₺5.600**.

Taranan ve **taksit yönetim listesi görülmeyen** yüzeyler: `Daha` sekmesi · `Ayarlar → Tekrarlayan İşlemler` (yalnız
abonelik var, taksit yok). `Tekrar/Taksit` menüsünün karesi ilk kez alındı (`E0406`).
Kanıt: `E0406`–`E0416`.

### K3 · Goodbudget — normal gelir yolu (GB-Q01) ✔ **kapandı**

`Beta Tasarim` ₺1.234, `Fill Each Envelope` ile tek zarfa. Üç gösterge ayrı ayrı:

| Soru | Cevap | Değer |
|---|---|---|
| Hesap bakiyesini artırıyor mu? | **Hayır** | `All Accounts` 41.734,00 → 41.734,00 |
| Zarf kalanını artırıyor mu? | **Evet** | `Tasarim Yazilimi` 0,00 → 1.234,00; toplam 43.784 → 45.018 |
| Raporda gelir sayılıyor mu? | **Evet** | Eylül Income 3.284; önceki koşumda 2.050 (E0121/E0124), bu turda ayrı önce raporu yok |

İşlem kaydı oluştu ve hesabı gösteriyor (`09/20 Beta Tasarim +1.234,00 Ana Hesap`), buna rağmen
hesap bakiyesi değişmedi. Koşum kaydına göre uygulama kapatılıp yeniden açıldıktan sonra
da sonuç aynıydı; kare son bakiyeyi kanıtlar. Bu, tüm gecikme/senkronizasyon olasılıklarını elemez. **Nedeni bilinmiyor ve ürün kusuru diye yazılmıyor.** Zarf–hesap farkı 2.050 → 3.284.
Kanıt: `E0417`–`E0424`.

**Kontrol değeri:** hesap 41.734,00 (değişmedi), zarf toplamı 43.784,00 → **45.018,00**.

### Bu turda kapanmayanlar

Kart borcunun tamamen ödenmesi · gelecek taksit satırlarının arka planda kayıt mı görünüm mü
olduğu (B12) · Money Manager'ın Bütçe bloğunun kurulumu · Goodbudget'ta hesap bakiyesinin neden
değişmediği · `Keep Available` kipi ve `FROM AVAİLABLE` sekmesinin sonucu.

## Koşum sonrası · ✔ devir denetiminde tamamlandı

1. Yeni kareler envanter ve gözlem formlarında tam dosya adlarıyla doğrulandı; 12 mekanik atıf hatası düzeltildi.
2. Kanıt dizini yeniden üretildi: 387/387 kare açıklamalı, 422 kimlik; özgün hash'ler korundu.
3. Tema-03/06/08, soru kapanışları, güncel durum ve plan §7 güncellendi.
4. Nedensellik, fiziksel model ve denenmeyen kipler açık sınır olarak korundu; yeni koşum başlatılmadı.

## Bu listede olmayanlar ve nedeni

| İş | Neden burada değil |
|---|---|
| Wallet Postpone/Dismiss · Bluecoins geri yükleme · Bluecoins otomatik kol · HD Aktar formu · MM hazır tutar · WL Payment Due Date · WL dışa aktarma | Plan §7 "İki" ve "Üç"; yazımı durdurmuyorlar, ikinci turda |
| Kart borcunun tamamen ödenmesi · aktarımın bir bacağının silinmesi · ekstre döneminin geçmesi | Mevcut veriyi koruma kuralıyla çatışıyor (plan §7, "yapılamayanlar (a)") |
| Goodbudget ücretli kart/transfer · Paraşüt, Logo, QuickBooks iç ekranları | Erişim yok (plan §7, "yapılamayanlar (b)") |
