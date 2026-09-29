# Para akışı haritası

**Durum: KAPANDI (28 Eylül 2026).** §7'deki H1–H6 kullanıcı onayıyla kapandı (H3 kullanıcının önerisiyle
değişti: gün içi satışlar gün sonu panelinde listelenip "bu Z'ye dahil mi?" diye sorulur). Geçerli kararlar
`research/kasa-pos-gun-sonu/KAPANIS.md` (KP1–KP21); bu belge haritanın kendisi ve gerekçesi olarak kalır.
**Neden var:** Kasa'yı tek başına tasarlamak yerine genelden Kasa'ya inmek (kullanıcı, 28 Eylül: "sadece
kasa ekranı olarak düşünme, genelden kasa ekranına indirgeyerek düşün"). İnceledikçe "bazı işler
birbiriyle karışıyor" gözlemi çıktı; bu harita her para yolunu tek yerde tanımlar.

**Ölçüt:** kullanıcının sözleri (§0). ADR'ler yalnız çatışma kontrolü için okundu; çatışma §8'de.

---

## 0 · Kullanıcının koyduğu çerçeve

| Tarih | Söz | Haritadaki karşılığı |
|---|---|---|
| 28 Eyl | "Kasa kısmı dükkânın nakit kısmını, POS tahsilatları kısmı da dükkânın kredi kartı / banka kısmını kapsıyor" | Kasa = nakit, POS = kart/banka tarafı (§1) |
| 28 Eyl | "Tek tek girmesini beklemek hiç mantıklı değil" | Satış gün sonu toplamıyla girilir (A1, A2) |
| 28 Eyl | "Para işlemler kısmına girer ama gider gelir olmaz" (POS parasının bankaya geçişi için) | **Taşıma** ile **tanıma**nın ayrımı (§1) |
| 28 Eyl | "Bu tarz denetimler kullanıcıdan %100 tüm hareketleri girmesini bekler … pratikte olmaz" | Banka sayımı yok; kontrol yalnız kasada |
| 28 Eyl | "Ekstra kesintiyi işlem detayına ekleyebiliriz" | Yatış kesintisi İşlemler detayında (A3) |
| 28 Eyl | "Şahsi kasası diye bir şey yok, ortalığı karıştırır" | Kasa yalnız işletme kasası (C4) |
| 28 Eyl | "Yemek kartı ayrımını komisyon miktarıyla sağlayabiliyoruz … POS'umuza isim vererek" | Yemek kartı = adı verilmiş POS (A2) |
| 28 Eyl | "Businessbudget uygulaması olduğumuzu unutmamamız lazım" | Muhasebe defteri, stok, satış satış kayıt yok (§6) |
| 28 Eyl | "Yapacağımız değişiklikler diğer özelliklerle bütün içinde çalışmalı" | Her yolun tek kaydı ve tek formu var; kapılar çok, form tek (§4) |
| 28 Eyl | "Esnaf mal aldı, fişi aldığı adamın POS'unda kayıtlı … bazı işler birbiriyle karışıyor" | "Kart" kelimesinin üç anlamı (§2), B3 |

## 1 · Haritanın iki sorusu

Her para yolu için iki şey sorulur:

- **Kazandım / harcadım mı?** (gelir veya gider **tanır**) — işletme netini, bütçeyi, muhasebeci paketini etkiler.
- **Para yer değiştirdi mi?** (bakiyeyi **taşır**) — kasa ve banka bakiyesini etkiler.

Bir olay ikisini aynı anda ya da farklı günlerde yapabilir. Veresiye satış bugün **tanır**, para
tahsil edilince **taşır**. Aynı olayı iki kez tanımak, aynı satışı iki kez gelir yazmaktır; haritadaki
karışıklıkların hepsi (§3) bu tek hatanın farklı yüzleri.

## 2 · "Kart" kelimesinin üç anlamı

Kullanıcının "fişi aldığı adamın POS'unda" sorusu buradan doğuyor:

| Kimin kartı | Esnaf için ne | Kayıt | Etkisi |
|---|---|---|---|
| Esnafın **kredi kartı** (tedarikçinin POS'unda ödedi) | Harcama | Kart harcaması | Gider tanır, kart borcu artar; kasa/banka kıpırdamaz |
| Esnafın **banka kartı** (tedarikçinin POS'unda ödedi) | Harcama | Gider (banka hesabından) | Gider tanır ve bankadan çıkar |
| **Müşterinin** kartı (esnafın **kendi** POS'unda) | Satış | POS tahsilatı | Gelir tanır; para yolda, sonra bankaya taşınır |

Esnafın kendi POS'u yalnız **müşteriden aldığı** parayı taşır. Tedarikçinin POS'unda yaptığı ödeme
esnafın POS'unu ilgilendirmez. Arayüzde bu ayrım kelimeyle kurulmalı: satış tarafında "kartla satış",
harcama tarafında "kartla ödedim".

## 3 · Harita

Sütunlar: **Olay** (esnafın dili) · **Bugün**: kayıt ve nereden girildiği · **Tanır / taşır** · **Sorun** ·
**Öneri**. ✓ sorun yok · ⚠ sorun var.

### A · Para giriyor: satış ve tahsilat

| # | Olay | Bugün | Tanır / taşır | Sorun | Öneri |
|---|---|---|---|---|---|
| A1 | Günün nakit satışı | Gelir, kasaya; + → Gelir, tek tek | İkisi aynı gün | ⚠ Tek tek girilmezse kasa sayımı satışı "fazla" sanıyor (U1) | **Gün sonu**: nakit toplamı tek satır |
| A2 | Günün kartlı satışı (yemek kartı dahil) | POS tahsilatı; + → POS tahsilatı, her akşam ~15 dokunuş | Tanır bugün, taşır geçiş günü | ⚠ Her akşam aynı beş seçim (U13), net görünmüyor (U6), hafta sonu sahte "Gecikti" | **Gün sonu**: adı verilmiş POS başına toplam; POS tanımı hesabı, oranı ve geçiş gününü bilir |
| A3 | Kartlı satışın bankaya geçmesi | "Hesaba geçti", tek tek, yalnız tarih | Taşır | ⚠ Toplu yatış ve eksik yatan tutar yazılamıyor (U2, U3); geri alınamıyor (U12) | Toplu işaretleme + yatan tutar; kesinti tek gider satırı, **İşlemler'de yatışın detayında** görünür; geri alınabilir |
| A4 | Havale/EFT ile satış veya hizmet bedeli (faturalı iş) | Gelir, bankaya | İkisi aynı gün | ✓ | Değişmez |
| A5 | Veresiye satış | Cari borçlandırma; Cari hesap sayfası | Tanır; para kıpırdamaz | ✓ | Değişmez |
| A6 | Veresiyenin nakit ya da havaleyle tahsili | Cari tahsilat | Taşır; gelir yok | ✓ | Değişmez; Kasa'dan açılırsa kasa seçili gelir |
| A7 | Veresiyenin **kartla** tahsili | Yolu yok. Kullanıcı POS'a girerse gelir **iki kez** sayılır (borçlandırmada bir, POS'ta bir) | Olması gereken: taşır, gelir yok, para yolda | ⚠ Çifte sayım riski | Cari tahsilatta ödeme yolu "kartla (POS)": para yola çıkar, gelir yazılmaz, bankaya geçişi A3 ile aynı işaretlenir (§7 H2) |
| A8 | Tek seferlik alacak ve tahsili | Yükümlülük (alacak yönü) + kapatma | Tanır sonra taşır | ✓ (A7'nin kartla hâli burada da var) | A7'nin çözümü buraya da uygulanır |
| A9 | Tekrarlayan gelir (kira geliri vb.) | Tekrarlayan plan → gerçekleştir | Gerçekleşince ikisi | ✓ | Değişmez |
| A10 | Borç alma (kredi, tanıdıktan) | Borç sözleşmesi açılışı | Taşır; gelir değil | ✓ | Değişmez |

### B · Para çıkıyor: gider ve ödeme

| # | Olay | Bugün | Tanır / taşır | Sorun | Öneri |
|---|---|---|---|---|---|
| B1 | Kasadan gider veya mal alımı | Gider, kasa elle seçilir; + → Gider | İkisi | ⚠ Kasa'dan açılsa da kasa seçili gelmiyor; Kasa ekranı kendini yenilemiyor (U11) | Kasa'da "Kasadan öde" kısayolu; kasa önden seçili |
| B2 | Banka kartı / havaleyle gider | Gider, bankadan | İkisi | ✓ | Değişmez |
| B3 | Kredi kartıyla gider (tedarikçinin POS'unda) | Kart harcaması; + → Gider → kaynak "Kredi kartları" | Tanır; kart borcu artar | ✓ Davranış doğru; kelime karışıklığı (§2) | Formda "kartla ödedim" dili; POS kelimesi harcama tarafında hiç geçmez |
| B4 | Kredi kartıyla taksitli alım | Taksit planı → her ay gerçekleşir | Gerçekleşen taksit tanır | ✓ | Değişmez. Satış tarafında taksit bu tur **yok**; açılırsa bu kavramla çakışmayacak adla (§6) |
| B5 | Kredi kartı borcunu ödeme | Kart ödemesi; + → Kart borcu öde | Taşır; gider yok | ✓ | Değişmez |
| B6 | Vadeli alım (cari) ve ödemesi | Cari borçlandırma + ödeme | Tanır sonra taşır | ✓ | Değişmez |
| B7 | Ödenmemiş fatura ve ödemesi | Yükümlülük + kapatma; + → Ödenmemiş fatura | Tanır sonra taşır | ✓ | Değişmez |
| B8 | Tekrarlayan gider (kira, SGK, vergi takvimi) | Tekrarlayan plan | Gerçekleşince | ✓ | Değişmez |
| B9 | Borç taksidi ödeme | Borç sözleşmesi taksidi | Taşır; faiz payı gider | ✓ | Değişmez |
| B10 | POS komisyonu ve kesintisi | Komisyon POS kaydıyla birlikte gider; kesinti yazılamıyor | Tanır | ⚠ Kesinti yok (A3) | Komisyon POS tanımından; kesinti yatışta |
| B11 | Fiş okuyarak gider | Fiş okuma → gider formu (öneri); iade fişi → harcamayı iptal | Formun yaptığı | ✓ | Değişmez |

### C · Para yer değiştiriyor: kendi hesaplarım arasında

| # | Olay | Bugün | Tanır / taşır | Sorun | Öneri |
|---|---|---|---|---|---|
| C1 | Kasadaki nakdi bankaya yatırma | Transfer; + → Transfer | Taşır | ⚠ Kasa ekranı yenilenmiyor (U11) | Kasa'da "Bankaya yatır" kısayolu |
| C2 | Bankadan nakit çekip kasaya koyma | Transfer | Taşır | ✓ | Kasa'da kısayol (isteğe bağlı) |
| C3 | Hesaplar arası aktarma | Transfer | Taşır | ✓ | Değişmez |
| C4 | Kasadan kendime para almak | Transfer (şahsi cüzdana) ya da şahsi gider | Duruma göre | ⚠ Kasa ekranında şahsi cüzdan da "kasa" gibi duruyor | Kasa yalnız işletme kasası; "Kendime aldım" kısayolu (§7 H4) |
| C5 | Hedefe (ör. vergi karşılığı) para ayırmak | Hedef | Hiçbiri | ✓ | Değişmez |

### D · Kontrol ve düzeltme

| # | Olay | Bugün | Sorun | Öneri |
|---|---|---|---|---|
| D1 | Kasa sayımı ve fark | Kasa sayımı; farkı gelir/gider yazma | ⚠ Satış girilmemişse fark anlamsız (U1); sonraki hareketleri bilmiyor (U10) | Gün sonunda satıştan sonra sayılır; sayımdan sonra değişen kasa "değişti" der |
| D2 | Yanlış kaydı düzeltme | Silme yerine iptal + yeni kayıt | ⚠ POS kaydı hiç iptal edilemiyor (U12) | POS ve yatış iptal edilebilir |
| D3 | Banka ekstresini içe aktarma | CSV → her satır gelir/gider | ⚠ POS yatışı ikinci kez gelir sayılır (U8) | Bu tur yok; ayrı tur (fikir F05) |
| D4 | Dekont okuma | Karar sayfası "Bu tutar ne?" | ✓ | Değişmez |

## 4 · Kapılar: nereden girilir

**İlke: tek form, çok kapı.** Her olayın tek formu vardır; bu form birden çok yerden açılır ve açıldığı
yere göre önden dolar. Aynı olay için ikinci bir form yazılmaz. Uygulamada bu ilke bugün de var
("menü ikinci bir kopya form açmaz, oraya götürür").

| Form | Kapılar |
|---|---|
| **Gün sonu** (yeni, A1+A2+D1) | + menüsü (işletme profilinde "POS tahsilatı"nın yerine) · Kasa "Gün sonunu gir" |
| Tek kartlı satış (A2, büyük tek satış) | Gün sonu panelinin içinde "tek satış ekle" · Kasa "Kartla gelecek" bölümü |
| Hesaba geçenler (A3) | Kasa "Kartla gelecek" · Özet'teki "Yolda" satırı |
| Gider (B1–B3) | + menüsü · Kasa "Kasadan öde" (kasa seçili) · fiş okuma |
| Transfer (C1–C4) | + menüsü · Kasa "Bankaya yatır" / "Kendime aldım" (önden dolu) |
| Cari tahsilat (A6, A7) | Cari hesap sayfası · Kasa'dan (kasa seçili) |
| İşlemler listesi (fikir F01–F03) | Alt çubuk · Özet'te kategori · hesap satırı · kart sayfası · Kasa'nın hareketleri |

**+ menüsü** bugün niyete göre gruplu (Para girdi · Para çıktı · Para taşı · Belge okut · Plan kur);
bu yapı korunur, yalnız "Para girdi" altında işletme profilinde "POS tahsilatı"nın yerini "Gün sonu"
alır. Kişisel profilde değişmez.

## 5 · Kasa'nın haritadaki yeri

Kasa sekmesi bu haritanın **tezgâh üstü gün sonu** kesitidir: A1, A2, A3, A7 (kartla tahsilatın yoldaki
parası), B1, C1, C2, C4, D1. Sekmenin dört sorusu (bugün ne sattım · kasa tuttu mu · kartla ne gelecek ·
ne geldi) bu satırlardan cevaplanır. Kasa kendi kaydını tutmaz; burada görünen her şey haritadaki bir
kaydın görünümüdür.

Taslak tel çerçeve (28 Eylül sohbeti; harita onaylanınca Kasa tasarım brifi bundan yazılır):

```
Kasa                                   ⟲
‹  Bugün · 28 Eylül Pazartesi  ›            gün gün geriye bakılır
BUGÜN                    ● Gün sonu yok
Satış                         ₺30.590,00
  Nakit ₺20.790        Kartla ₺9.800
Kasadan çıkan                  ₺1.850,00
[ Gün sonunu gir ]                           ana eylem (A1+A2+D1)
NAKİT · Dükkân Kasası
Kasada olmalı                 ₺31.200,00
Son sayım: bugün 19:40         ✓ Tuttu
[Kasayı say]   Kasadan öde · Bankaya yatır · Kendime aldım
KARTLA GELECEK               Yolda ₺33.012
Bugün geçecek
 ☐ Ziraat POS · 25–26 Eyl satışı  ₺23.383
Yarın · 29 Eylül Salı
 ○ Ziraat POS · 28 Eyl satışı      ₺9.628
[Hesaba geçenleri işaretle]     POS'larım ›
SON GÜNLER                           Tümü ›
26 Cmt  ₺14.100  Kasa ✓   Kart yatıyor
25 Cum  ₺12.300  Kasa −₺85  Kart geçti
```

Gün sonu paneli: nakit satış · POS başına kartlı satış (net ve geçiş günü canlı) · isteğe bağlı kasa
sayımı · "Günü kapat". Yatış paneli: beklenen · yatan · kesinti. Yatış detayı İşlemler'de: beklenen,
yatan, kesinti, kapattığı satış günleri, "Geri al".

## 6 · Kapsam dışı (işletme bütçesi sınırı)

Satış satış kayıt, adisyon ve stok · Z raporunun mali kaydı ve KDV beyanı · banka sayımı (kullanıcı
kararı) · kart türüne göre oran · ayrı BSMV alanı · tatil takvimi · satış tarafında taksit (ertelendi) ·
banka entegrasyonu · CSV'nin bu turda değişmesi (ayrı tur).

## 7 · Açık kararlar

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **H1** | + menüsü | A · bugünkü gibi ("POS tahsilatı" kalır, "Gün sonu" Kasa'da) · B · işletme profilinde "POS tahsilatı"nın yerine "Gün sonu" | **B.** Akşam girişinin tek adresi olur; tek satış gün sonunun içinde kalır |
| **H2** | Kartla veresiye tahsilatı (A7) | A · bu tur yok, cari tahsilat formu "kartla aldıysanız POS'a girmeyin" diye uyarır · B · cari tahsilatta "kartla (POS)" ödeme yolu: para yola çıkar, gelir yazılmaz | **B.** Uyarı, esnafın en olağan işlerinden birini (müşteri borcunu kartla kapatır) cevapsız bırakır; B aynı "yoldaki para"yı kullanır, yeni bir kavram açmaz |
| **H3** | Gün içinde tek tek girilen satış + akşam gün sonu | A · gün sonu toplamı "günün tamamı"dır; panel gün içinde girilenleri gösterir ve yalnız kalanı yazar · B · gün sonu yalnız "girilmemiş kısmı" sorar · C · ikisi ayrı; kullanıcı dikkat eder | **A.** Esnaf Z raporundaki toplamı olduğu gibi yazar, çifte sayımı uygulama önler |
| **H4** | "Kendime aldım" (C4) | A · şahsi bir hesaba transfer · B · kasadan şahsi gider · C · kullanıcı her seferinde seçer | **C, varsayılan A.** Şahsi hesabı olan transfer eder, parayı doğrudan harcayan şahsi gider yazar |
| **H5** | Kasa'da hangi hesaplar görünür | A · işletme kapsamlı nakit hesaplar · B · kullanıcının seçtiği nakit hesaplar | **A**, hesabın kapsamı değiştirilerek ayarlanabilir |
| **H6** | Satış ve harcama tarafının dili | Satış: "kartla satış", "POS". Harcama: "kartla ödedim", "kredi kartı" | Öneri tabloda; "POS" kelimesi yalnız satış tarafında |

## 8 · ADR çatışma kontrolü

Bu haritadaki önerilerin hiçbiri bir ADR ile çatışmıyor. Yakın geçen iki nokta, kullanıcının kararı için:

- **H2-B:** "gelir yazmayan yoldaki para", bugün POS tahsilatının her zaman gelir yazma kuralına yeni bir
  yol ekler. Kullanıcının "para işlemlere girer ama gelir gider olmaz" sözüyle aynı yönde; ADR'yi
  değiştirmez, genişletir.
- **H5:** Kasa'nın şahsi cüzdanı göstermemesi yalnız bir görünüm; işletme ve şahsi paranın tek havuzda
  kalmasına dokunmaz.
