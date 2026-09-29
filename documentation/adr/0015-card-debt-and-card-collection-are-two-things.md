# ADR 0015 — "Kart" iki ayrı şeydir: borçlandığın kart ve tahsil ettiğin POS

- Durum: **Kabul edildi** (24 Ağustos 2026, Aşama 04 Grup 1). **ADR 0019
  (kabul edildi, 29 Eylül 2026) bu kararı genişletir:** §1'in kelime kuralı ("kartla
  satış", "Kartla gelecek") ve §2–3'ün yoldaki para tanımı (POS satışı + kartla
  tahsil; test kapısı ikisini kapsar).
- Bağlam: Gün sonu kasa, POS tahsilatı ve ana gezinmenin eklenmesi
- İlgili: ADR 0014 (ekonomik olay tanır, ödeme taşır), ADR 0013 (işletme ve
  şahsi tek havuzdur; ön ayar özellik kapatmaz), ADR 0010 (net varlık borcu
  anaparayla ölçer), ADR 0003 (kart ekstresi ve taksitler)

## Bağlam

Uygulamada bugün `CreditCard` diye tek bir şey var ve o şey **borçlandığın
karttır**: limiti, kesim günü, ekstresi ve borcu vardır. Esnafın gündelik
dilinde "kart" ise çoğu zaman bunun tam tersidir — müşterinin ödediği, paranın
*geldiği* araçtır.

İki yön aynı kelimeyle aynı üründe yaşayınca kullanıcı hangi tarafa baktığını
bilemez. Somut hâli şu: kullanıcı "kartla 800 lira aldım" der. Bugünkü modelde
bunu yazacağı tek yer `CreditCardCharge`'dır ve o kayıt **borcunu 800 lira
artırır**. Tahsilat, borç olarak kaydedilir. Bu bir isimlendirme sorunu değil,
yanlış tarafa yazılmış paradır.

İkinci mesele paranın **zamanıdır**. POS'tan geçen para o gün hesaba girmez:
banka birkaç gün tutar, komisyonu keser, sonra net tutarı geçirir. Kullanıcı
bu parayı bugün harcayabileceğini sanırsa kasası açık verir. Aynı para net
varlığında vardır — onun parasıdır, yoldadır — ama kullanılabilir değildir.

Üçüncü mesele gezinmedir. Kasa ekranının oturacağı yer bugünkü dört sekmede
yok; `Bütçeler` ana sekme olarak bir ev bütçesi kavramı ve tezgâh üstü esnafın
ikinci ekranı değil.

## Karar

### 1. Borç tarafı adını korur, tahsilat tarafına "kart" denmez

- Borç tarafı arayüzde **`Kredi kartlarım`**: kullanıcının kendi kredi kartı,
  bugünkü modeliyle ve bugünkü adıyla kalır. Ekstre, taksit ve kart borcu
  ödemesi bu başlığın altındadır.
- Tahsilat tarafı **`POS tahsilatları`**: müşterinin ödediği para. Bu ekranda
  ve bu kayıtlarda **`kart` kelimesi tek başına hiç kullanılmaz.**
- Henüz hesaba geçmemiş para arayüzde **`yolda`** diye adlandırılır. `bloke`
  bankacılık jargonudur ve kullanıcının kelimesi değildir.

Reddedilenler ve gerekçeleri:

| Seçenek | Neden reddedildi |
|---|---|
| `CreditCard`'ı `Borç kartı` gibi bir ada çevirmek | Kimsenin konuşmadığı bir terim üretir; kullanıcı kendi kartını ararken bulamaz. Kod tarafında da geniş bir yeniden adlandırma maliyeti, karşılığında hiçbir davranış düzelmez |
| İki tarafı da yönle adlandırmak (`Borçlarım` / `Tahsilatlarım`) | Simetrik ama kullanıcının kartını aradığı kelime kaybolur. Ayrıca `Borçlarım` cari borç ve sözleşmeli borçla çakışır — bu üründe borç zaten üç ayrı şey |
| Tek bir `Kartlar` ekranında iki yönü sekmeyle göstermek | Sorunun kendisini korur: aynı ekranda aynı kelime iki yöne bakar. Yanlış tarafa yazılan 800 lira yine yazılabilir |

### 2. POS tahsilatı bir hesap türü **değildir**; yoldaki para projection'dır

`AccountType`'a yeni bir değer **eklenmez**. Bekleyen tahsilatlar bir hesapta
durmaz; yoldaki tutar, geçişi gerçekleşmemiş tahsilatların net toplamından
**her sorguda hesaplanır**.

Gerekçe:

- **Hesap, paranın *bulunduğu* yerdir; yoldaki para henüz hiçbir yerde
  değildir.** Onu hesap yapmak, kullanıcıya oradan transfer yapma, oradan kart
  borcu ödeme ve onu kasa sayımına katma yolunu açardı. Üçü de olmamış bir
  parayı harcamaktır.
- **Bakiye bu üründe kalıcı kolon değildir** (kurucu kural). "Bloke hesabı"
  ikinci bir gerçek kaynak yaratır ve iki sayı kaçınılmaz olarak ayrışır.
- Geçiş gerçekleştiğinde yoldaki tutar kendiliğinden düşer; düşürecek ikinci
  bir kayıt yazmak gerekmez. Silinecek bir şey olmadığı için tutarsızlaşacak
  bir şey de yoktur.

POS tahsilatı ADR 0014'ün kapsamındadır ve onun istisnası değildir:

| An | Tanır | Taşır |
|---|---|---|
| Tahsilat günü | **Gelir brüt tutar kadar**, komisyon gider olarak | Hiçbir hesap değişmez |
| Geçiş günü | Hiçbir gelir/gider yazılmaz | Hedef hesap **net tutar** kadar artar |

Komisyon brüt tutardan **ayrı okunur ve ona eklenmez**; dekonttaki işlem
ücretinde öğrenilen dersin aynısıdır. Brüt tutarı net göstermek, kullanıcının
gerçekten kestiği faturayı küçültür ve komisyonu görünmez bir gidere çevirir.

### 3. Kullanılabilir bakiye ile net varlık aynı soruya cevap vermez

- **Kullanılabilir bakiye** "bugün ne harcayabilirim?" sorusunun cevabıdır ve
  yoldaki parayı **içermez**.
- **Net varlık** "neyim var?" sorusunun cevabıdır ve yoldaki parayı **içerir**,
  ayrı bir satır olarak yazar.
- İkisi arasındaki fark **tam olarak** yoldaki tutardır; bu bir test kapısıdır,
  yorum değil.

Bu, ADR 0010'un net varlığı borcu anaparayla ölçmesiyle aynı çizgidedir: net
varlık sahip olunanı ölçer, kasa bugün elde olanı.

### 4. Üçüncü ana sekme onboarding ön ayarına göre değişir

- İşletme kullanıcısında üçüncü sekme **`Kasa`**, kişisel kullanıcıda
  **`Bütçeler`**.
- Yerini veren sekme **kaybolmaz**, `Diğer` altına iner.

```text
İşletme:  Özet · İşlemler · Kasa      · Diğer  (└ Bütçeler)
Kişisel:  Özet · İşlemler · Bütçeler  · Diğer  (└ Kasa)
```

Bu ADR 0013'ün "ön ayar hiçbir özelliği kapatmaz" kuralını **bozmaz**: hiçbir
ekran erişilemez olmuyor, yalnız hangisinin bir dokunuş uzakta olduğu
değişiyor. Kullanıcı cevabını sonradan değiştirse bile veri değişmez —
kategorilerde olduğu gibi, değişen tek şey yerleşimdir.

Reddedilen: her iki profilde sabit bir sekme listesi. Esnafta `Bütçeler`,
kişisel kullanıcıda `Kasa` günde bir kez bile açılmayan bir sekmeyi ana çubukta
tutardı; dört sekmenin biri ölü olurdu.

## Sonuçları

- `PosSettlement` kendi yazma modelidir; `CreditCard`'a alan eklenmez ve
  `AccountType` genişlemez.
- Yoldaki tutar hiçbir tabloda kolon değildir; Özet ekranında ve net varlık
  kırılımında projection olarak okunur.
- Arayüz metinleri bu ADR'nin kelimelerini kullanır: `Kredi kartlarım`,
  `POS tahsilatları`, `yolda`. Sözleşme tarafı her zamanki gibi kararlı makine
  değerleri gönderir; cümleyi istemci kurar.
- Gezinme iki yerleşim taşır ve ikisi de dört dolu sekmedir; hiçbir ekran
  yalnız bir yerleşimde erişilebilir olamaz — bu bir test kapısıdır.
- Aşama 04'ün kalan grupları bu kararların üzerine kurulur; yedek şeması v9
  POS tahsilatını ve kasa sayımını taşır.

## Bu ADR neyi karara bağlamaz

- Gerçek POS cihazı veya banka entegrasyonu: **kapsam dışıdır**, veriler elle
  girilir (`PROJECT-ROADMAP.md`, kapsam dışı tablosu).
- Komisyon oranının otomatik çekilmesi: oran kullanıcıdan alınır.
- Vardiya, kasiyer ve personel ayrımı: bu aşamanın dışındadır.
- KDV alanları: Aşama 05'in işidir ve kendi ADR'siyle açılır.
