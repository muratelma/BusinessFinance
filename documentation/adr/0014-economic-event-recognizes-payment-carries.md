# ADR 0014 — Ekonomik olay tanır, ödeme taşır

- Durum: **Kabul edildi** (23 Ağustos 2026, Aşama 02 Grup 1)
- Bağlam: Cari hesabın (karşı taraf ve açık bakiye) eklenmesi
- İlgili: ADR 0002 (transferin gelir/gider olmaması), ADR 0003 (kart ödemesinin
  ikinci kez gider sayılmaması), ADR 0009 (borç kaynağı ve anüite faizi),
  ADR 0010 (net varlığın borcu anaparadan ölçmesi), ADR 0011 (fiş okuma bir
  öneri katmanıdır)

## Bağlam

Uygulama bugüne kadar aynı kuralı üç kez, üç ayrı yerde uyguladı ve hiçbir
yerde yazmadı. Dördüncüsü kapıda: cari hesap.

Kural, esnafın sorduğu iki sorunun **aynı soru olmadığını** kabul etmekten
doğuyor:

| Soru | Cevabı veren |
|---|---|
| Bu ay ne sattım, ne harcadım? | Ekonomik olay — mal el değiştirdiği an |
| Kasamda ne var? | Ödeme — para el değiştirdiği an |

Veresiye satış bu ikisini birbirinden ayıran en keskin örnek. Malı bugün
verdiniz, parayı üç hafta sonra alacaksınız. "Bu ay ne sattım?" sorusunun cevabı
bugünü içerir; "kasamda ne var?" sorusununki içermez. Tek bir kayıt türüyle
ikisini birden doğru cevaplamanın yolu yok.

## Karar

**Bir kayıt ya ekonomik olayı tanır ya ödemeyi taşır; ikisini birden yapmaz.**

- **Tanıyan kayıt** gelir veya gider yazar, hesap bakiyesine dokunmaz.
- **Taşıyan kayıt** hesap bakiyesini değiştirir, gelir/gider üretmez.
- Bir kayıt ikisini birden yaptığında bu bir istisna değil, **aynı ekonomik
  olayın nakitle aynı anda gerçekleşmesidir**: peşin satış tek kayıttır çünkü
  olay ile ödeme aynı ana düşer.

Bu, aynı paranın iki kez sayılmasını önleyen tek yapıdır.

## Kanıt: kural zaten kodda

Karar yeni bir şey icat etmiyor; var olanı yazıya geçiriyor. Bugünkü davranış
(`EfFinancialReportRepository`, aylık rapor ve hesap bakiyesi hesabı):

| Kayıt | Gelir/gider tanır | Nakit taşır | Nerede görülür |
|---|---|---|---|
| `BudgetTransaction` (peşin gelir/gider) | **Evet** | **Evet** | Olay ile ödeme aynı anda |
| `CreditCardCharge` | **Evet** | Hayır | Gider toplamına girer, hesap bakiyesinde yoktur |
| `CreditCardPayment` | Hayır | **Evet** | Bakiyeden düşülür, gider toplamında yoktur (ADR 0003) |
| `Transfer` | Hayır | **Evet** (iki hesap arasında, net sıfır) | ADR 0002 |
| `DebtAgreement` açılışı, `Expense` kaynaklı | **Evet** | Hayır | Borcun doğduğu gün gider yazılır |
| `DebtAgreement` açılışı, `Cash` kaynaklı | Hayır | **Evet** | Para hesaba girer; borç almak gelir değildir |
| Borç taksiti ödemesi (anapara) | Hayır | **Evet** | Bakiyeyi düşürür, gideri tekrar yazmaz |

Aynı aggregate'in (`DebtAgreement`) kaynağına göre iki farklı davranması,
kuralın gücünü gösteriyor: belirleyici olan **kaydın türü değil, ne olduğu.**
Gider kaynaklı borç bir ekonomik olaydır ve tanır; nakit kaynaklı borç yalnız
para hareketidir ve taşır.

### Tek kenar durumu: faiz

Borç taksitindeki faiz payı **ödendiği ay gider yazılır**, borç doğduğu gün
değil. Bu kuralın istisnası değil, kuralın kendisidir: gelecekteki faiz henüz
**doğmamış** bir yükümlülüktür. Doğduğu an ödendiği andır ve o an hem tanınır
hem taşınır — tıpkı peşin satış gibi. Anapara ile faizin ayrı ölçülmesi
ADR 0010'un net varlık ölçüsüyle de tutarlıdır.

## Cari hesaba uygulanışı

| Cari kayıt | Tanır | Taşır |
|---|---|---|
| Borçlandırma — veresiye satış | **Gelir** (satış geliri, o gün) | Hayır |
| Borçlandırma — tedarikçiden vadeli alım | **Gider** (o gün) | Hayır |
| Tahsilat — müşteriden para alma | Hayır | **Evet** (hesap +) |
| Ödeme — tedarikçiye para verme | Hayır | **Evet** (hesap −) |

Sonuçları:

- **Cari bakiye kalıcı kolon değildir.** Borçlandırmalar eksi tahsilatlar
  olarak hesaplanan bir projection'dır — hesap bakiyesi ve kart ekstresiyle
  aynı ilke.
- **Tahsilat kategori taşımaz.** Kategori "ne alındı/satıldı" sorusunu
  cevaplar ve o soru borçlandırmada sorulmuştur; tahsilata kategori vermek aynı
  satışı iki kovaya yazmak olurdu. Kart ödemesinin kategori taşımamasıyla aynı
  gerekçe.
- **Kapsam borçlandırmada durur**, tahsilatta değil. Gelir/gider raporunu
  bölen taraf odur; tahsilat raporu bölmez, çünkü rapora hiç girmez (ADR 0013).
- Aynı müşteriye üç satış + iki kısmi tahsilat girildiğinde gelir raporu üç
  satışı, kasa iki tahsilatı, cari bakiye aradaki farkı gösterir. Üçü aynı
  anda doğrudur ve hiçbir tutar iki kez sayılmaz.

## Reddedilen alternatifler

**1. Saf nakit esası: geliri tahsilatta yaz.**
Reddedildi. Veresiye satış o ay hiç görünmezdi ve esnaf "bu ay ne sattım"
sorusunu uygulamaya soramazdı. Dahası mevcut kart modeliyle çelişirdi: kart
harcaması bugün gider yazılıyor, ödemesi gelecek ay yapılıyor. İki model aynı
üründe yaşayamaz.

**2. Hem borçlandırmayı hem tahsilatı gelir/gider saymak.**
Reddedildi ve bu kararın var olma sebebi bu: aynı satış iki kez gelir olurdu.
Kullanıcı bunu ancak yıl sonunda, iki katına çıkmış bir ciroyla fark ederdi.

**3. Cari bakiyeyi karşı taraf üzerinde kolon olarak tutmak.**
Reddedildi. Bakiyenin ikinci bir gerçek kaynağı olur; iptal edilen bir
borçlandırma ya da tahsilat kolonu güncellemeyi unuttuğu anda uygulama iki
farklı doğru gösterir. Projection kuralı bütün ürün boyunca aynı.

**4. Müşteri ve tedarikçiyi ayrı tip yapmak.**
Reddedildi. Mahalle esnafında aynı kişi hem alıcı hem satıcıdır; ikiye bölmek
onu iki kayıt hâline getirir ve "Ahmet'le hesabım ne?" sorusunu cevapsız
bırakır. Yön kaydın kendisinde durur.

## Sonuçlar

**Kolaylaşan:** yeni bir finansal model eklenirken sorulacak soru tek: bu kayıt
tanır mı, taşır mı? Cevap, raporlara ve bakiyeye nasıl gireceğini kendiliğinden
belirler.

**Pahalılaşan:** veresiye satış iki kayıt ister (borçlandırma + sonradan
tahsilat). Bu ekstra iş değil, gerçeğin kendisi — ama arayüzün tahsilatı tek
dokunuşa indirmesi gerekiyor, yoksa kullanıcı ikinci kaydı hiç girmez ve cari
bakiye şişer.

**Ölçülecek:** aynı karşı tarafın cari bakiyesi ile taksitli sözleşmesi net
varlıkta **bir kez** sayılmalı. Aşama 02'nin çıkış koşulu bu.

## Bilinen tutarsızlık ve devri

Kuralın dışında kalan tek yer, fiş okumanın **"faturayı henüz ödemedim"**
yoludur. Bugün bu yol hiçbir şey tanımıyor: kullanıcıyı planlama ekranına
götürüp tekrarlayan plan öneriyor (`_recordInvoice`, `app_router.dart`).
Sonuç, aynı faturanın hangi ekrandan girildiğine göre farklı davranması:

- Kartla ödenmiş fatura → gider **bugün** tanınır.
- Ödenmemiş fatura → hiçbir şey tanınmaz, yalnız bir plan önerilir.

Ödenmemiş fatura, tanınması gereken bir ekonomik olaydır ve tekrarlayan plan
değildir — tek seferliktir, vadesi vardır ve bir karşı tarafı vardır.

**Bu aşamada düzeltilmiyor.** Doğru kabı (vadeli yükümlülük) Aşama 03'te
açılıyor; burada yapılacak bir çözüm, üç hafta sonra taşınacak bir yapı üretir.
ADR tutarsızlığı kayda geçirir ve Aşama 03'e devreder.

## Uygulanma kanıtı

Şu testler kararı koruyor (Aşama 02 zorunlu testleri; Grup 2'de domain
seviyesi yazıldı, kalanı kendi gruplarında gelir):

- Borçlandırma gelir/gider tanır, hesap bakiyesini değiştirmez.
- Tahsilat hesap bakiyesini değiştirir, gelir/gider üretmez.
- Aynı satış hem borçlandırma hem tahsilat girildiğinde iki kez gelir sayılmaz.
- Net varlıkta aynı karşı tarafın cari ve taksitli borcu bir kez sayılır.
