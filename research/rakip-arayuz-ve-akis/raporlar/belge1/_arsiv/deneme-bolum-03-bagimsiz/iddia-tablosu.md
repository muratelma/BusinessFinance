# İddia ve sınır kaydı — bağımsız Bölüm 3

16 Eylül 2026. Metin, tablolar, altyazılar ve sonuç cümleleri aynı kanıt sınırına tabidir. Satırların kapsamı aşağıda bölüm bazında belirlenmiştir. Mevcut kareler bir ifadeyi destekliyorsa eklenir; desteklemiyorsa ifade gözlenen yüzeyle sınırlanır.

| Yer | Gözlem / ifade kapsamı | Dayanak | Çıkarılmayan sonuç |
|---|---|---|---|
| Kapsam tablosu | Beş Android koşumu; kaynak panoları; girişte kalan ürünler | E0010, E0005, E0014, E0007, E0006, E0008, E0011, E0009, E0012 gözlem formları | Bütün ürünlerde aynı inceleme derinliği |
| 3.1 Money Manager | Ay, özet ve gün gruplu liste; hesaplar ayrı | E0228, E0236 | Toplam etiketlerinin finansal denkliği |
| 3.1 Bluecoins | Günlük Özet, reklam, Bütçe Özeti | E0103 | Dolu hesabın varsayılan açılış sekmesi |
| 3.1 Wallet | Hesaplar ardından tanıtım kartları | E0276 | Bütün Home içeriğinin ilk ekrana sığması |
| 3.1 Hesap Defterim | Seçili defter, liste ve sabit toplamlar | E0137 | Bütün defterlerin birleşik toplamı |
| 3.1 Goodbudget | Monthly/Available; etiketsiz iki zarf tutarı | E0115 | Kareden sayıların anlamını çıkarma |
| 3.1 kaynak panoları | Grafik/özet/vade blokları; tahsilat ve ödeme halkaları | E0211 destek; E0262 temsili çizim | Güncel çalışan sürüm |
| 3.2 Money Manager | Aynı dört alt sekme dört ana bölümde | E0228, E0231, E0236, E0254 | Her alt formda görünürlük |
| 3.2 Bluecoins | Üst sekmeler ve menü, iki Hesaplar etiketi | E0103, E0084 | İki hedefin aynı/farklı davranışı |
| 3.2 Wallet | Sol menü; Home içi iki sekme | E0275, E0276, E0376 | Bütün hedeflerin denenmiş olması |
| 3.2 Hesap Defterim | Menü ve başlıkta defter seçici | E0171, E0137 | Yeniden açılışta hangi defterin seçileceği |
| 3.2 diğerleri | Goodbudget dört üst sekme; KolayBi yan panel ve pano sekmeleri | E0115, E0211 | Sekme geçişlerinin performansı |
| 3.3 girişler | Dört +; iki para yönü düğmesi; Aktar menüsü; Hızlı İşlemler | E0228, E0103, E0026, E0276, E0115, E0137, E0171, E0211 | + ile başlayan tüm türlerin aynı akışı izlemesi; zorunlu tür seçimi |
| 3.4 Wallet | Plan kartının Home altındaki yeri; yükleniyor görünümü | E0274 | Yüklenmiş satırlar, gerçekleşme ve bakiye etkisi |
| 3.4 Bluecoins | Hatırlatıcılar ayrı sekmede; tarih ve durum etiketleri | E0056 | Ana panoda hiçbir alternatif gösterimin olmaması |
| 3.4 diğer canlı kareler | Bu yüzeylerde bekleyen alan görülmemesi | E0228, E0137, E0115; E0007 B1 taraması | Ürün genelinde özellik yokluğu |
| 3.4 kaynak devamı | KolayBi günü gelen işler; Paraşüt halka/durum çizimi | E0211, E0181, E0262; E0008 ve envanter kaynak incelemesi | Sürümleri tek ekrana birleştirme; canlı davranış |
| 3.5 başlangıç | Money Manager kullanıcı kaydı; Bluecoins karşılama; Wallet önceden açılmış hesap | E0010 K00; E0017; E0014 K00 | Ölçülmüş adım sayısı veya zorunluluk |
| 3.5 Hesap Defterim | Kullanım diyaloğu; yardım ve düğme adı farkı | E0135, E0007 K00 | Bu farkın kullanıcı hatasına yol açtığı |
| 3.5 Goodbudget | Giriş/yeni hane seçimi ve izlenen kurulum yolu | E0106, E0006 K00 | Bütçe veya kayıt adımlarının zorunlu/atlanabilir olduğu |
| 3.5 erişim sınırları | Paraşüt ve Logo giriş; QBO/Solopreneur ayrımı | E0011, E0009, E0012 | QBO ekranının Solopreneur'e mal edilmesi |
| 3.6 boşluk | Boş ayda +; temiz başlangıçta ilk işlem kartı; boş defterde yön düğmeleri | E0227, E0026, E0136 | Üç karenin aynı veri durumunu anlattığı |
| 3.6 ayrıntı | E0227 alt mesajı geri tuşuyla çıkış; E0018 boş kartlar farklı görünüm | E0227 ve E0018 | Çıkış mesajının ilk kayıt rehberi olduğu |
| Sonuç ve şekil altyazıları | Yukarıdaki gözlemlerin kısa sentezi | İlgili bölüm ve şeklin E kimliği | Hız, başarı, erişilebilirlik veya tercih puanı |

## Açık kanıt ihtiyaçları

- Wallet: ilk kurulum, boş Home ve yüklenmiş plan kartı.
- Bluecoins: dolu hesapta yeniden açılış sekmesi ve çift Hesaplar hedefleri.
- Hesap Defterim: çok defterli durumda açılış seçimi.
- Goodbudget: kurulum atlama yolları ve boş ana ekran.
- KolayBi/Paraşüt: güncel sürüm ve canlı pano davranışı.
- Logo İşbaşı/Solopreneur: iç gezinme ve ana ekran.

Bunlar bu bölümde kesinlik iddiasını sınırlar; yeni canlı koşum veya veri sıfırlama yapılmadı. Yeni bir kare de ancak ilgili bağlamı gösteriyorsa boşluğu kapatır.
