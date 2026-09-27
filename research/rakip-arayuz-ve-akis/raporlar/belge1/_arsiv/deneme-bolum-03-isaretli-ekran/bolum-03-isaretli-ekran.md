# Bölüm 3 · Gezinme ve ana ekran

Belge 1 · Rakip arayüz yaklaşımları · Bölüm 3 · İşaretli ekran anlatımı · 16 Eylül 2026 · taslak

> Bu dosya PDF ile aynı içeriğin okunabilir kopyasıdır; ikisi de `icerik.py`den üretilir.
> Sayfa düzeni, işaretler ve ölçüler yalnız PDF'te görünür.

## Giriş

Bu bölüm tek bir soruyu izler: uygulama açıldığında ekran neyi önce söylüyor ve kullanıcıyı nereye çağırıyor?

Anlatım ekranın kendisi üzerinden yürür. Her karenin üzerine numaralı işaretler konmuştur ve metin o numaralara konuşur. İşaretler yalnız bu bölümün kopyaları üzerine çizildi; araştırmanın özgün kareleri değişmedi.

Kanıt niteliği ürüne değil ifadeye bağlıdır. Bir ürünün canlı incelenmiş olması, o ürün hakkındaki her cümlenin sınanmış olduğu anlamına gelmez.

**Bu bölüm neyi ölçmez.** Bu bölüm başarı, hız, memnuniyet ve erişilebilirlik ölçmez; bunların hiçbiri için test yapılmadı. Kaydın finansal sonucu Belge 2'nin, BusinessFinance için tercihler Belge 3'ün konusudur.

### Kanıt anahtarı

- **Canlı kare** — Emülatörde açılan ekranın görüntüsü.
- **Koşum kaydı** — Gözlem formunda yazılı; karesi yok veya kare tek başına göstermiyor.
- **Kaynak görseli** — Ürünün kendi destek sayfası veya tanıtım videosu. Davranış denenmedi.
- **Temsili çizim** — Ürünün pazarlama görseli. Çalışan ekran olduğu doğrulanmadı.
- **Görülmedi** — Kanıt yok. Özelliğin bulunmadığı anlamına gelmez.

### Kapsam

- **Canlı koşumla incelendi** (Android): Money Manager · Bluecoins · Wallet · Hesap Defterim · Goodbudget
- **Yalnız kaynaktan görüldü** (web / masaüstü): KolayBi (destek sayfası, tanıtım videosu) · Paraşüt (tanıtım videosu)
- **Ana ekranı hiç görülmedi** (—): Logo İşbaşı · QuickBooks Solopreneur

## 2 · Beş ekran, beş farklı bölüşüm

Beş ürünün açılışta görünen karesi aynı yükseklikte yan yana konduğunda ekranın nasıl bölündüğü doğrudan okunabiliyor. Her karenin solundaki şerit, o karenin dikey bölgelerini gösterir.

| Ürün | Gezinme | Para bilgisi | Tanıtım | Kayıt bandı | Boş |
|---|--:|--:|--:|--:|--:|
| Money Manager | %16 | %59 | — | — | %25 |
| Bluecoins | %14 | %53 | %29 | — | %4 |
| Wallet | %26 | %19 | %55 | — | — |
| Hesap Defterim | %16 | %56 | — | %5 | %23 |
| Goodbudget | %13 | %36 | — | — | %51 |

- Wallet'ın karesinde hesap kartları dikey alanın yaklaşık altıda birini tutuyor; ürün, üyelik ve banka bağlantısı tanıtımları yarısından fazlasını kaplıyor.
- Bluecoins'te iki veri kartının arasına yaklaşık dörtte bir yükseklikte bir reklam giriyor.
- Goodbudget'ın karesinde zarflar bittikten sonra kalan alan boş; bu karede aşağıda başka bir blok yok.
- Hesap Defterim kayıt başlatmayı yüzen bir düğme olarak değil, ekranın altında sabit bir bant olarak taşıyor.
- Money Manager ve Goodbudget'ın bu karelerinde tanıtım alanı yok. Hesap Defterim'in dolu karesinde de yok; boş defter karesinde alt banner reklamı görünüyor (Şekil 8.3).

*Ölçü sınırı: oranlar yalnız açılışta görünen kareden, dikey piksel bölgelerinden okundu. Kaydırıldığında görünen bölümler ölçüye girmiyor. Bluecoins ve Wallet karelerinde içerik alt kenarda kesiliyor; bu iki ürünün gerçek sayfa uzunluğu bu kareden bilinmiyor. Oran bir tasarım değerlendirmesi değil, karenin geometrisidir.*

## 3 · İlk açılışta ne isteniyor?

Uygulama ilk kez açıldığında ekran kullanıcıdan bir şey isteyebilir, ne yapılacağını anlatabilir veya doğrudan çalışmaya başlayabilir. İncelenen beş üründen üçünün ilk karesi var; ikisinde ilk açılış görülmedi.

**Şekil 3.1 · Bluecoins · karşılama ekranı** (E0017)

1. Mağaza rozeti uygulamanın kendi ekranında tekrar ediyor.
2. Karşılama cümlesi; ne yapılacağını söylemiyor.
3. Tek ilerleme yolu: Hadi Başlayalım.
4. Dil ekranın altında seçilebiliyor.

**Şekil 3.2 · Hesap Defterim · ilk açılış** (E0135)

5. Açıklama diyaloğu uygulamanın ne işe yaradığını anlatıyor.
6. Metin Ücretli ve Alınan düğmelerinden söz ediyor.
7. Ekrandaki düğmelerin adı Ödendi ve Alındı.
8. Diyalogdan tek çıkış: TAMAM MI.

**Şekil 3.3 · Goodbudget · ilk açılış** (E0106)

9. Hesabı olan giriş yapıyor.
10. Hesabı olmayan için tek yol yeni hane açmak.
11. Şifreleme notu ekranda.
12. Sürüm numarası ekranda: 2.24.26013 (180).

- Money Manager'ın ilk açılış karesi yok. Koşum kaydına göre kayıt, giriş veya karşılama ekranı gelmedi; uygulama doğrudan ana ekranla açıldı (E0010 K00, kullanıcı doğruladı).
- Wallet'ın ilk açılışı görülmedi; koşumlar önceden açılmış bir hesapla yapıldı (E0014 K00).
- Goodbudget üçü içinde hesap isteyen tek ürün. İzlenen yolda yeni haneden sonra bütçe kurulumu geliyor; atlanabilir olup olmadığı denenmedi.
- Hesap Defterim'in diyaloğu ekrandaki düğme adlarını farklı yazıyor — metin Ücretli/Alınan, düğmeler Ödendi/Alındı. İkisi aynı karede görünüyor (işaret 6 ve 7).
- Bu üç kare kurulumun tamamını göstermiyor; yalnız ilk görülen ekranı gösteriyor.

*Dayanak.* Canlı kare: E0017, E0135, E0106. Koşum kaydı: E0010 K00, E0014 K00. Görülmedi: Money Manager ve Wallet'ın ilk açılış karesi. Çıkarılmayan sonuç: adımların zorunlu veya atlanabilir olduğu; ilk açılışın kullanıcıyı kayda yöneltmedeki etkisi.

## 4 · Ana ekran önce neyi söylüyor?

İki ürün de açılışta bir toplam gösteriyor. Toplamın neyin toplamı olduğu farklı: birinde seçili ayın gelir ve gideri, diğerinde bütçe kovalarına dağıtılmış tutar.

**Şekil 4.1 · Money Manager · İşlemler › Gün** (E0228)

1. Dönem seçici. Ekrandaki her sayı bu aya ait.
2. Gelir, Gider ve Toplam; üçü de seçili ayın.
3. Gün başlığında o günün geliri ve gideri ayrıca yazıyor.
4. Satırda kategori, açıklama ve hesap adı birlikte.

**Şekil 4.2 · Goodbudget · ENVELOPES** (E0115)

5. Hesap adı. Bu kopyada karartıldı.
6. Total: zarflara dağıtılmış tutar. Hesap bakiyesi değil.
7. Zarf satırında iki sayı var; hangisinin ne olduğu ekranda yazmıyor.
8. Zarflar bitince kare boş devam ediyor.

- Money Manager'ın karesinde hesap bakiyesi yok; bakiyeler ayrı bir yüzeyde (E0236).
- Goodbudget'ın karesinde de hesap bakiyesi yok; ACCOUNTS ayrı sekmede.
- Zarf satırındaki iki sayının anlamı koşum kaydına dayanır; kare tek başına ayırmıyor.

**Aynı soruda diğer üç ürün**

- **Bluecoins** — Hesaplar sekmesi hesap listesi değil, düzenlenebilir özet kartları gösteriyor (Şekil 2.2).
- **Wallet** — İlk blok doğrudan hesap kartları ve bakiyeleri (Şekil 2.3).
- **Hesap Defterim** — Seçili defterin hareketleri; her satırda o anki denge, altta sabit toplamlar (Şekil 2.4).

*Dayanak.* Canlı kare: E0228, E0115. Koşum kaydı: E0006 K01 (zarf sayılarının anlamı). Çıkarılmayan sonuç: iki toplamın finansal olarak denk olduğu.

## 5 · Bölümler nerede duruyor?

Beş üründe bölüm seçici beş ayrı yerde: ekranın altında sabit, üstünde sabit, üstte yana kaydırılan, yandan açılan çekmecede veya çekmece ile defter seçicinin birleşiminde. Çekmeceyi kullanan iki üründe çekmecenin içi tek düzeyde listeleniyor.

**Şekil 5.1 · Wallet · çekmecenin alt kısmı** (E0376)

1. Debts: borç defteri.
2. Araya başka bir ürünün tanıtımı giriyor.
3. Alışveriş listesi, garanti ve sadakat kartı aynı listede.
4. Görünüm anahtarları da aynı listede.

**Şekil 5.2 · Hesap Defterim · çekmece** (E0171)

5. Listenin ilk kalemi reklam kaldırma.
6. Aktar: hesaplar arası para hareketi burada.
7. Nakit hesap makinesi ve not defteri aynı listede.
8. Silinmiş işlemler de aynı düzeyde.

Gezinme bölgesi kırpıntıları: Money Manager (E0228), Goodbudget (E0115), Bluecoins (E0103), Wallet (E0276), Hesap Defterim (E0137)

- İki çekmecede de finansal bölümler ile yardımcı araçlar arasında görsel bir ayrım yok: başlık, ayraç veya gruplama görünmüyor.
- Bir bölümün var olup olmadığı çekmece açılmadan ekrandan anlaşılmıyor.
- Bluecoins çekmecesinde farklı simgeli iki 'Hesaplar' kalemi var; hedefleri açılmadı (E0084).

*Dayanak.* Canlı kare: E0228, E0115, E0103, E0276, E0137, E0376, E0171. Çıkarılmayan sonuç: menü uzunluğundan bulunabilirlik, öğrenme süresi veya işlem hızı.

## 6 · Kayıt nereden başlıyor?

Dört üründe kayıt tek bir yuvarlak düğmeyle başlıyor ve paranın yönü sonra açılan formda seçiliyor. Hesap Defterim yönü düğmenin adına taşımış: ekranda iki düğme var ve hangisine basıldığı yönü belirliyor.

**Şekil 6.1 · Hesap Defterim · ana ekran** (E0137)

1. Alındı: para girişi.
2. Ödendi: para çıkışı.
3. Her satırda o kayıttan sonraki denge yazıyor.
4. Ekranın altında dönem toplamları sabit duruyor.

Kayıt düğmesi kırpıntıları (Şekil 6.2): Money Manager (E0228), Bluecoins (E0103), Wallet (E0276), Goodbudget (E0115)

- Hesap Defterim'de iki yön ekranda; üçüncü yön olan hesaplar arası aktarım ekranda değil, çekmecedeki Aktar kaleminde (Şekil 5.7).
- Dört + düğmeli üründe formda tür seçenekleri görülüyor (E0229, E0074, E0277, E0116). Formun hangi türle açıldığı ve her kayıtta seçimin değiştirilmesi gerekip gerekmediği bu bölümde doğrulanmadı.
- Form alanlarının sırası ve kaydetme sonrası geri bildirim Bölüm 5'in konusu.

*Dayanak.* Canlı kare: E0137, E0171, E0228, E0103, E0276, E0115. Çıkarılmayan sonuç: adım sayısı, kayıt süresi, tür seçiminin zorunlu olduğu.

## 7 · Bekleyen işler nerede görünüyor?

Bekleyen iş iki üründe iki ayrı yerde duruyor: ana ekranın aşağısında bir kart ya da kendi sekmesinde tarih gruplu bir liste. İkisi de gelecekteki veya vadesi geçmiş kayıtların görünür bir yerini gösteriyor; gerçekleşme kuralları bu bölümün dışında.

**Şekil 7.1 · Wallet · ana ekranın aşağısı** (E0274)

1. Bakiye eğilimi kartı.
2. Upcoming planned payments: bekleyen ödemeler kartı.
3. Kartın satırları bu karede yükleniyor; içerik görünmüyor.
4. Ana ekrana kart ekleme davetiyesi.

**Şekil 7.2 · Bluecoins · Hatırlatıcılar sekmesi** (E0056)

5. Durum sözcükle yazılıyor: Dün bitti.
6. Bugün süresi doluyor; renk tek başına taşımıyor.
7. Tarih grubunun başlığında o günün toplamı.
8. Taksitli kayıtta kaçıncı taksit olduğu satırda.

- "Canlı incelenen ürünlerin hiçbirinde ana ekranda bekleyen iş yok" cümlesi yazılamaz; Şekil 7.1 bunu çürütür. Bu, kapalı bir karşılaştırmanın açık kanıtla sınanmasıdır.
- Wallet kartının yüklenmiş hâli görülmedi: satır içeriği, sayısı ve tutarları bu kareden bilinmiyor.
- Money Manager, Hesap Defterim ve Goodbudget'ın incelenen ana ekran karelerinde bekleyen iş alanı görünmüyor. Bu, ürün genelinde böyle bir yüzey olmadığı anlamına gelmez.

*Dayanak.* Canlı kare: E0274, E0056. Görülmedi: Wallet kartının yüklenmiş içeriği. Çıkarılmayan sonuç: bekleyen kaydın gerçekleşmesi, bakiyeye etkisi, hatırlatma davranışı.

## 8 · Veri yokken ekranda ne kalıyor?

Boş ekranda iki ayrı şey korunabilir: ekranın yapısı ve ne yapılacağını söyleyen sözcükler. Üç kare üçünü de aynı anda vermiyor ve üçü aynı boşluk durumuna ait değil.

**Şekil 8.1 · Money Manager · kayıtsız ay** (E0227)

1. Dönem bağlamı duruyor: Eylül 2026.
2. Üç özet sayısı sıfır olarak kalıyor.
3. Tek metin: Veri yok.
4. Alttaki mesaj çıkışla ilgili, ilk kayıt rehberi değil.

**Şekil 8.2 · Bluecoins · temiz kurulum** (E0026)

5. Karşılama sözcüğü.
6. Sıfır bakiye, etiketiyle birlikte.
7. Para birimi ekranda yazıyor.
8. İlk eylem adıyla çağrılıyor: İlk İşlemi Ekle.

**Şekil 8.3 · Hesap Defterim · boş defter** (E0136)

9. Yedekleme daveti.
10. Reklam kaldırma daveti.
11. Sütun başlıkları duruyor.
12. İki yön düğmesi duruyor.
13. Altta banner reklam.

- Hesap Defterim yapıyı koruyor, sözcük vermiyor: başlıklar, düğmeler ve sıfır toplamlar yerinde, ilk kayıt için yönlendirme metni yok.
- Bluecoins tersini yapıyor: liste yapısı yok, ama ilk eylem ve para birimi sözcükle söyleniyor.
- Money Manager ikisini de vermiyor; koruduğu tek şey dönem bağlamı ve sıfırlanmış özet satırı.
- Üç kare üç farklı durum: kayıtsız bir ay, temiz kurulum ve boş defter. Aynı soruya cevap veriyorlar ama aynı başlangıç noktasından değil.

*Dayanak.* Canlı kare: E0227, E0026, E0136. Görülmedi: Wallet ve Goodbudget'ın boş ana ekranı (yeni hesap gerektirir, mevcut test verisi silinmez). Çıkarılmayan sonuç: boş ekranın kullanıcıyı ilk kayda yöneltmedeki etkisi.

## 9 · Kaynaktan görülen pano: KolayBi

İçine girilemeyen ürünlerde ana ekran yalnız ürünün kendi yayımladığı görsellerden görülüyor. KolayBi'nin destek sayfasındaki bu görselde modül adları, sekmeler ve sayaçlar okunabiliyor; yine de canlı bir koşum değil.

**Şekil 9.1 · KolayBi · destek sayfası panosu (kaynak görseli)** (E0211)

1. Solda sabit modül paneli: satış, satın alma, gider, cari, finans, projeler, raporlar.
2. Sağ üstte Hızlı İşlemler menüsü; kapalı olduğu için seçenekleri görünmüyor.
3. Panonun içinde iş ortağı tanıtım bandı.
4. Gelir ve gider için iki çizgili nakit akışı.
5. Günü Gelen İşlemler: Bugün, Yaklaşanlar, Tarihi Geçenler.
6. Baloncuktaki demo verisi 5.1.2023 tarihli.

- Görsel bir tarayıcı penceresi olarak çerçevelenmiş ve gölgelendirilmiş; ham ekran görüntüsü değil, destek sayfası için hazırlanmış bir yayın görselidir.
- İçindeki veri demo verisidir ve grafiğin baloncuğu Ocak 2023'ü gösteriyor. Bu kare, bugünkü sürümün ekranı olduğunu kanıtlamaz.
- Panonun bir kısmı alt kenarda kesiliyor: Tahsilat Ve Ödeme Özetleri başlığı görünüyor, içeriği bu karede yok.
- İkinci bir video karesi (E0181) aynı panoyu farklı bir düzenle gösteriyor; iki kaynak tek bir güncel ekran gibi birleştirilmedi.
- Pano masaüstü genişliğinde. Telefon ekranlarıyla aynı alan koşullarında karşılaştırılamaz; bölüm 2'nin oranları bu görsele uygulanmadı.

*Dayanak.* Kaynak görseli: E0211 (destek sayfası), E0181 (tanıtım videosu). Görülmedi: mobil iç yüzey, canlı davranış, güncel sürüm. Çıkarılmayan sonuç: bu düzenin bugün çalışan ekran olduğu.

## 10 · Temsili çizim: Paraşüt

Paraşüt'ün ana ekranı yalnız bir tanıtım videosu karesinden görülüyor. Bu kare bir ekran görüntüsü değil, çizimdir. Aşağıdaki dört işaret bunu ayrı ayrı gösteriyor; bu yüzden kareden hiçbir davranış veya tutar sonucu çıkarılmadı.

**Şekil 10.1 · Paraşüt · tanıtım videosu karesi (temsili çizim)** (E0262)

1. Sol menü kutuları boş çizilmiş; hiçbirinde etiket yok.
2. Tahsil edilecek halkası: 138.89,20 ₺.
3. Gecikmiş halkası birebir aynı tutarı gösteriyor.
4. Uyarı kutusundaki 19.989,00 ₺ bu iki halkanın hiçbiriyle uyuşmuyor.
5. Sağ sütun da etiketsiz boş dikdörtgenlerden oluşuyor.

- Dört işaret birlikte çizim olduğunu gösteriyor: etiketsiz menü kutuları, iki halkada yinelenen aynı tutar, uyarı kutusuyla uyuşmayan üçüncü tutar ve geçerli bir Türkçe biçime uymayan sayı yazımı (138.89,20).
- Yine de kareden okunabilen bir şey var: ürünün kendi anlatımında ana ekran tahsilat ve ödemeyi vade durumuna göre ayırıyor — tahsil edilecek, gecikmiş, fatura yok; ödenecek, ödeme yok, planlanmış.
- Bu bir ürün iddiasıdır, gözlem değildir. Paraşüt'ün gerçek ana ekranı hiçbir kaynakta görülmedi.
- Mobil uygulamada yalnız giriş öncesi tanıtım ekranları görüldü; giriş ekranı geçilemedi.

*Dayanak.* Temsili çizim: E0262. Koşum kaydı: E0011 (giriş ekranı geçilemedi). Görülmedi: çalışan ana ekran, mobil iç yüzey, her türlü davranış. Çıkarılmayan sonuç: karedeki tutarların bir senaryo sonucu olduğu.

## Kanıt eki

### Şekil dizini

| Şekil | Kimlik | Koşum | Tür | Özgün dosya |
|---|---|---|---|---|
| 2.1 | E0228 | Money Manager · Faz 1, 10 Eylül 2026 | canlı kare | `money-manager/03-dolu-ana-ekran.png` |
| 2.2 | E0103 | Bluecoins · Faz 7, 11 Eylül 2026 | canlı kare | `bluecoins/f7-52-nav-check.png` |
| 2.3 | E0276 | Wallet · Tur 1 | canlı kare | `wallet-budgetbakers/03-dolu-ana-ekran.png` |
| 2.4 | E0137 | Hesap Defterim · Tur 1, 10 Eylül 2026 | canlı kare | `hesap-defterim/03-dolu-ana-ekran.png` |
| 2.5 | E0115 | Goodbudget · 11 Eylül 2026 | canlı kare | `goodbudget/08-envelopes-filled-home.png` |
| 3.1 | E0017 | Bluecoins · Tur 1 | canlı kare | `bluecoins/01-ilk-acilis.png` |
| 3.2 | E0135 | Hesap Defterim · Tur 1, 10 Eylül 2026 | canlı kare | `hesap-defterim/01-ilk-acilis-hosgeldin.png` |
| 3.3 | E0106 | Goodbudget · 11 Eylül 2026 | canlı kare | `goodbudget/01-ilk-acilis.png` |
| 4.1 | E0228 | Money Manager · Faz 1, 10 Eylül 2026 | canlı kare | `money-manager/03-dolu-ana-ekran.png` |
| 4.2 | E0115 | Goodbudget · 11 Eylül 2026 | canlı kare | `goodbudget/08-envelopes-filled-home.png` |
| 5.1 | E0228 | Money Manager · Faz 1, 10 Eylül 2026 | canlı kare | `money-manager/03-dolu-ana-ekran.png` |
| 5.2 | E0115 | Goodbudget · 11 Eylül 2026 | canlı kare | `goodbudget/08-envelopes-filled-home.png` |
| 5.3 | E0103 | Bluecoins · Faz 7, 11 Eylül 2026 | canlı kare | `bluecoins/f7-52-nav-check.png` |
| 5.4 | E0276 | Wallet · Tur 1 | canlı kare | `wallet-budgetbakers/03-dolu-ana-ekran.png` |
| 5.5 | E0137 | Hesap Defterim · Tur 1, 10 Eylül 2026 | canlı kare | `hesap-defterim/03-dolu-ana-ekran.png` |
| 5.6 | E0376 | Wallet · Faz 7, 11 Eylül 2026 | canlı kare | `wallet-budgetbakers/f7-55-menu-scroll.png` |
| 5.7 | E0171 | Hesap Defterim · 12 Eylül 2026 | canlı kare | `hesap-defterim/36-drawer-menu-ust.png` |
| 6.1 | E0137 | Hesap Defterim · Tur 1, 10 Eylül 2026 | canlı kare | `hesap-defterim/03-dolu-ana-ekran.png` |
| 6.2 | E0228 | Money Manager · Faz 1, 10 Eylül 2026 | canlı kare | `money-manager/03-dolu-ana-ekran.png` |
| 6.2 | E0103 | Bluecoins · Faz 7, 11 Eylül 2026 | canlı kare | `bluecoins/f7-52-nav-check.png` |
| 6.2 | E0276 | Wallet · Tur 1 | canlı kare | `wallet-budgetbakers/03-dolu-ana-ekran.png` |
| 6.2 | E0115 | Goodbudget · 11 Eylül 2026 | canlı kare | `goodbudget/08-envelopes-filled-home.png` |
| 7.1 | E0274 | Wallet · Tur 1 | canlı kare | `wallet-budgetbakers/00-magaza.png` |
| 7.2 | E0056 | Bluecoins · Faz 7, 11 Eylül 2026 | canlı kare | `bluecoins/f7-05-hatirlaticilar.png` |
| 8.1 | E0227 | Money Manager · Faz 1, 10 Eylül 2026 | canlı kare | `money-manager/02-bos-ana-ekran.png` |
| 8.2 | E0026 | Bluecoins · Faz 3, 10 Eylül 2026 | canlı kare | `bluecoins/10-fresh-bos-ana-ekran.png` |
| 8.3 | E0136 | Hesap Defterim · Tur 1, 10 Eylül 2026 | canlı kare | `hesap-defterim/02-bos-ana-ekran.png` |
| 9.1 | E0211 | KolayBi · destek sayfası görseli, 12 Eylül 2026 erişim | kaynak görseli | `kolaybi/d24-destek-guncel-durum-panosu.png` |
| 10.1 | E0262 | Paraşüt · tanıtım videosu karesi | temsili çizim | `parasut/04-video-cari-hesap-durumu.png` |

İki karede dosya adı ile içerik uyuşmuyor; ikisi de bu turda açılarak doğrulandı ve adlar değiştirilmedi, çünkü E kimlikleri sabittir. E0231'in adı 07-rapor-agustos.png, ekranın alt çubuğunda seçili sekme İstatistik. E0274'ün özgün dosya adı 00-magaza.png; içeriği mağaza görseli değil, canlı emülatör karesidir (durum çubuğu, iskelet yükleme blokları ve uygulamanın kendi kayıt düğmesi görünür). Ad ile içerik uyuşmazlığı burada kayda geçirildi; dosya adı değiştirilmedi, çünkü E kimlikleri sabittir.

### Çıkarılmayan sonuçlar

| Yer | Kurulmayan cümle |
|---|---|
| Bölüm geneli | Ürünlerin aynı derinlikte incelendiği |
| Bölüm 2 oranları | Oranların bir tasarım kalitesi ölçüsü olduğu; kaydırılan sayfanın tamamını anlattığı |
| Bölüm 3 | İlk açılış adımlarının zorunlu veya atlanabilir olduğu |
| Bölüm 5 | Bu yerleşimlerden bulunabilirlik, öğrenme süresi veya işlem hızı |
| Bölüm 4 | Money Manager ile Goodbudget toplamlarının finansal olarak denk olduğu |
| Bölüm 6 | Menü uzunluğundan bulunabilirlik, öğrenme süresi veya işlem hızı |
| Bölüm 7 | Kayıt adımlarının sayısı; tür seçiminin her kayıtta zorunlu olduğu |
| Bölüm 8 | Bekleyen kaydın gerçekleşmesi ve bakiyeye etkisi; ürün genelinde yokluk |
| Bölüm 9 | Boş ekranın kullanıcıyı ilk kayda yöneltmedeki etkisi |
| Bölüm 10 | KolayBi panosunun bugün çalışan ekran olduğu; masaüstü ile mobil yoğunluğun kıyaslanabilirliği |
| Bölüm 11 | Paraşüt çizimindeki tutarların bir senaryo sonucu olduğu |

### Eksik kanıt ve nasıl kapanır

Mevcut test verisi silinmez veya sıfırlanmaz. Yeni kurulum, oturum kapatma veya yeni hesap gerektiren eksikler bu nedenle Önerilmez olarak işaretlidir.

| Ürün | Eksik | Sayfa | Nasıl tamamlanır | Öncelik |
|---|---|---|---|---|
| Money Manager | İlk açılışın karesi yok; karşılama olmadığı kullanıcı beyanı | 3, 9 | Temiz kurulum gerekir; mevcut veriyi etkiler | Önerilmez |
| Wallet | İlk açılış ve boş ana ekran görülmedi | 3, 9 | Oturum kapatma veya yeni hesap gerekir; bulut hesabı etkilenir | Önerilmez |
| Goodbudget | Boş ana ekran ve kurulumu atlama yolu görülmedi | 3, 9 | Yeni hane gerekir | Önerilmez |
| Wallet | Bekleyen ödeme kartının yüklenmiş hâli görülmedi | 8 | Ana ekranı açıp kart yüklenene kadar bekleyip görüntü alın | Yüksek |
| Bluecoins | Dolu hesapta açılışta hangi sekmenin seçili geldiği bilinmiyor | 2 | Uygulamayı son uygulamalardan tamamen kapatıp açın; ilk ekranın görüntüsü | Orta |
| Hesap Defterim | Birden fazla defter varken açılışta seçili defter bilinmiyor | 2, 4 | Uygulamayı tamamen kapatıp açın; ilk ekranın görüntüsü | Orta |
| KolayBi / Paraşüt | Güncel sürüm ve canlı pano davranışı | 10, 11 | Ücretli hesap gerekir | Önerilmez |
