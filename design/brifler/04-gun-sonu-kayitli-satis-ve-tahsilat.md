# Brif 4 — Gün sonu paneli: kayıtlı veresiye satış ve tahsilat

`00-ortak-cerceve.md` ile birlikte okunur. Konu yalnız **Gün sonu paneli**dir (Brif 2'deki K4); Kasa
sekmesinin geri kalanı bu brifin dışındadır. Beş çerçeve çizilecek.

## Neden

Gün sonu paneli bugün çalışıyor: esnaf akşam nakit ve kart tutarını yazar, uygulama o gün zaten tek tek
girilmiş satışları düşer ve kalanını yeni satış olarak yazar. İki şeyi yanlış yaptığını gördük:

- **Veresiye satış ve tahsilat.** Esnaf gün içinde bir veresiye satışı ya da bir borç tahsilatını
  `Cari hesap`tan yazmış olabilir. Bunların akşam yazılan nakit tutarının **içinde olup olmadığını
  uygulama bilemez**: yazar kasadan nasıl geçirildiğine bağlıdır. Eskiden tahsilatı "dışında" sayıyor,
  veresiye satışı hiç sormuyordu; ikisi de sessizce yanlış tutar yazabiliyordu.
- **Toplamdan hesaplama.** "Nakit, kart ve toplamdan ikisi yeter" kuralı kalktı. Raporun toplamı başka
  ödeme türlerini de içerebildiği için farkı nakde ya da karta yazmak olmayan bir parayı kaydediyordu.

Bu yüzden panel artık soruyor. İlk denemeyi kendimiz çizdik ve **beğenmedik: çok karmaşık oldu.** Alt alta
iki soru bloğu, her birinde bir seçim rayı, satırlar ve bir açıklama var; ekran bir formdan çok bir ankete
benziyor. Senden istediğimiz, aynı bilgiyi **daha sakin** soran bir panel.

Reddedilen ilk deneme (içeriği görmek için; yerleşimi tekrarlama):
`tasarim-onizleme/gun-sonu-g5/g5-01 … g5-05.png`. Panelin bugünkü hâli: `tasarim-onizleme/gun-sonu/gunsonu-01 … 06.png`.

## Değişmez kurallar

1. **Kutu parayı ikinci kez girmez.** Listede görünen her kayıt uygulamada zaten yazılıdır. "Dahil" demek,
   o tutarın bu akşam yazılan nakdin içinde olduğunu söylemektir; uygulama onu **yeni satıştan düşer**.
   Panel bunu rakamla göstermelidir (`1.670 − 670 = 1.000`).
2. **Uygulama cevap uydurmaz.** Veresiye satış ve nakit tahsilat **hazır cevapla gelmez**; cevaplanmadan
   gün sonu kaydedilmez. Tek tek girilmiş nakit satışlar ve POS satışları ise bugünkü gibi **işaretli**
   gelir (onlar yazar kasadan geçmiştir).
3. **On tahsilat on soru olmamalı.** Çoğu gün cevap hepsi için aynıdır; tek hareketle verilebilmeli.
   Tek tek seçmek de mümkün olmalı.
4. **Soru yalnız gerektiğinde çıkar:** o gün böyle bir kayıt varsa **ve** nakit tutarı yazıldıysa. Yalnız
   kart tutarı yazan kullanıcı bu sorularla hiç karşılaşmaz.
5. **Aynı para iki kez düşülmez.** Aynı kişinin o günkü satışı da tahsilatı da "dahil" seçilirse üçüncü
   bir bilgi gerekir: ikisi girilen tutarda **ayrı ayrı mı** sayıldı, **tahsilat satışın içinde mi**,
   yoksa **bir kısmı mı ortak**. Soru, tahsilatın hangi satışa ait olduğunu değil, **yazılan tutarda
   nasıl sayıldıklarını** sorar. Bu da cevaplanmadan kaydedilmez.
6. **Hiçbir tutarı istemci hesaplamaz.** Düşülen tutar, yeni satış, her seçeneğin sonucu ve toplam farkı
   sunucudan gelir; panel yalnız gösterir.
7. **Toplam kayıt üretmez.** Yazıldıysa yalnız farkı gösterir. Nakit ve kart alanlarından hangisi
   yazıldıysa yalnız o kaydedilir; ekran bütün satışların kaydedildiği izlenimini vermemelidir.
8. **Uygulama bir satışın parasının alınıp alınmadığını bilmez.** Veresiye satış satırında "tahsilat yok"
   ya da "şu kadarı alındı" yazılmaz; satış ve tahsilat ayrı kayıtlardır.

## Kelimeler

- Ekranda: `Veresiye satış`, `Tahsilat`, `Alacak faturası`, `Nakit satış`, `Yazılacak`, `Gün sonunu kaydet`.
- Ekranda **geçmez:** "vadeli satış", "grup", "örtüşme", "sayılan kayıt", "cevaplanmadı", "ortak tutar"
  (bu sonuncusu bizim iç adımız; yerine doğal bir ifade öner).
- Cümleler kısa ve doğal Türkçe; çeviri gibi durmasın. Uzun açıklama, büyük buton, büyük yazı yok.
- "Kâr", KDV, fatura kesme gibi ön muhasebe dili yok (`00-ortak-cerceve.md`).

## Panelin bugünkü parçaları (değişmeyenler)

Başlık `Gün sonu` · Gün alanı · `Nakit satış` alanı · ana POS'un alanı (`Diğer POS'lar (2)` ile
diğerleri) · `Toplam` alanı · **o gün tek tek girilmiş kayıtların listesi** · `Yazılacak` özeti ·
`Kasayı ya da kategoriyi değiştir` · `Vazgeç` / `Gün sonunu kaydet`.

Listede artık **üç tür** kayıt var ve asıl tasarım sorusu bunların tek bir sakin bölümde nasıl duracağı:

| Tür | Örnek satır | Nasıl gelir |
|---|---|---|
| Tek tek girilmiş satış (nakit gelir, POS satışı, kartla tahsilat) | `Satış geliri · Dükkan kasası · ₺250,00` | İşaretli; kullanıcı kaldırabilir (bugünkü gibi) |
| Nakit tahsilat (cari tahsilat, alacak faturasının nakit tahsilatı) | `Ahmet Bakkal · Dükkan kasası · ₺300,00` | Cevapsız |
| Veresiye satış, alacak faturası | `Ayşe Terzi · Veresiye satış · ₺450,00` | Cevapsız |

## Çizilecek çerçeveler

Hepsi panelin kendisidir; telefonun altından açılır. Beş çerçevede aynı gün ve aynı kayıtlar kullanılır
(aşağıda "Örnek veri"); G4 ve G5 kendi küçük örneğini kullanır.

### G1 · Cevap bekleyen kayıtlar var
Nakit tutarı yazılmış; o gün iki tahsilat ve bir veresiye satış var, hiçbiri cevaplanmamış. Kullanıcı ne
sorulduğunu ve neden kaydedemediğini bir bakışta anlamalı. `Yazılacak` özeti bu hâlde ne gösterir, kaydet
düğmesi nasıl durur: sen öner.

### G2 · Cevaplanmış ve hesap
Tahsilatların hepsi dahil, veresiye satış dahil değil. `Yazılacak` özeti hesabı açık yazar: girilen
₺1.670,00, kayıtlı ₺670,00 (₺250,00 satış + ₺420,00 tahsilat), yeni nakit satış **₺1.000,00**.

### G3 · Bazıları dahil
İki tahsilattan yalnız biri dahil (Ahmet Bakkal ₺300,00 dahil, Mehmet Usta ₺120,00 değil). Yeni nakit
satış ₺1.120,00.

### G4 · Satış ve tahsilat birlikte dahil
Aynı kişinin o günkü ₺500,00 veresiye satışı ve ₺300,00 tahsilatı birlikte dahil seçilmiş. Üç cevap ve
her birinin sonucu:

| Cevabın anlamı | Düşülen | Yeni nakit satış (girilen ₺1.600,00) |
|---|---:|---:|
| Ayrı ayrı sayıldı | ₺800,00 | ₺800,00 |
| Tahsilat satışın içinde sayıldı | ₺500,00 | ₺1.100,00 |
| Bir kısmı ortak: kullanıcı tutarı yazar (örnekte ₺200,00) | ₺600,00 | ₺1.000,00 |

Yazılabilecek tutar en çok ₺300,00'dir (ikisinden küçük olanı); sınırı sunucu verir. Aynı soru bir
**alacak faturası ile aynı gün alınan kendi tahsilatı** için de sorulur (örn. `Sentetik fatura ₺400,00`
ve tahsilatı ₺400,00); orada kişi adı yerine faturanın adı yazar. Bu sorunun panelde nerede duracağını
(satırın altında, ayrı bir adım, ayrı bir küçük panel) sen öner.

### G5 · Toplam farkı
Kullanıcı yalnız `Ziraat POS` ₺500,00 ve `Toplam` ₺1.800,00 yazmış, nakit boş. Kaydedilecek tek şey
kartla satıştır. Not nedeni **tahmin etmez** (eskiden "genelde faturalı satış ya da veresiye
tahsilatıdır" diyordu): yalnız kart satışının kaydedileceğini ve ₺1.300,00 farkın kaydedilmediğini söyler.

## Senden karar önerisi beklediğim yerler

Emin olmadığın yerde iki seçeneği yan yana çiz, hangisini önerdiğini yaz.

1. **Üç tür kayıt tek bölümde mi, ayrı bölümlerde mi?** İlk denemede tahsilatlar ve veresiye satışlar
   iki ayrı soru bloğuydu; karmaşıklığın kaynağı buydu.
2. **Toplu cevap nasıl verilir?** (`Hepsi` / `Hiçbiri` / `Bazıları` rayı bir çözümdü; daha sade bir yol
   varsa onu çiz.) Kural 3'ü ve Kural 2'yi birlikte sağlamalı: tek hareket, ama hazır cevap yok.
3. **Cevapsız hâl nasıl görünür?** Kaydet düğmesi ve neyin eksik olduğu.
4. **G4'teki sorunun yeri ve cümlesi.** "Ortak tutar" yerine doğal bir ifade.
5. **`Yazılacak` özetinde hesabın yazılışı.** Bugün `Kasa · ₺670,00 düşüldü` yazıyor; kullanıcı "düşüldü"
   kelimesinden parayı ikinci kez girdiğini sanabiliyor.

## Örnek veri (sentetik; 10 Ekim 2026 Cumartesi akşamı)

- Kasa: **Dükkan kasası**. POS: **Ziraat POS** (ana; %2; 1 iş günü).
- Gün içinde tek tek girilenler:
  - 11:20 nakit satış ₺250,00 (`Satış geliri`) — işaretli gelir.
  - 13:10 **Ahmet Bakkal**, eski borcundan nakit tahsilat ₺300,00.
  - 15:40 **Mehmet Usta**, nakit tahsilat ₺120,00.
  - 16:05 **Ayşe Terzi**, veresiye satış ₺450,00.
- Akşam yazılan: `Nakit satış` ₺1.670,00.
- G2: tahsilatların hepsi dahil, veresiye satış değil → kayıtlı ₺670,00, yeni nakit satış ₺1.000,00.
- G3: yalnız Ahmet Bakkal dahil → kayıtlı ₺550,00, yeni nakit satış ₺1.120,00.
- G4 (ayrı örnek): **Mehmet Usta** ₺500,00 veresiye satış + ₺300,00 tahsilat; yazılan nakit ₺1.600,00.
- G5 (ayrı örnek): `Ziraat POS` ₺500,00, `Toplam` ₺1.800,00, nakit boş; komisyon ₺10,00, 12 Eki Pazartesi
  hesaba geçecek.

## Teslim

`00-ortak-cerceve.md`deki gibi: beş çerçeve ayrı bir sayfada, README'de çerçeve çerçeve ölçü, metinler ve
davranış (hangi dokunuş neyi değiştirir). Yeni token ya da yeni bileşen ekleme; gerekiyorsa mevcut
bileşenin (`AppSegmentRail`, `AppInlineNotice`, onay kutulu satır, `AppFormSheet`) nasıl kullanıldığını yaz.
