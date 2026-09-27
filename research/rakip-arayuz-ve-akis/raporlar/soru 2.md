SORU 2
Gpt tüm research klasorunu kapsamlı tarar mısın bu tarz hatalar var mı diye?
Daha sonra raporla.


Evet, benzer hatalar var. Üstelik sorun yalnız eski fotoğrafların fazlalığı değil: bazı karşılaştırmalar BusinessFinance’i yanlış anlatıyor, bazı çıkarımlar doğrulanmış davranış gibi karar tablolarına taşınıyor. Bu nedenle “yedi uygulama temiz, yalnız Wallet ve Bluecoins kaldı” sonucunu güvenilir bulmuyorum.

Dosyalara dokunmadım; düzeltme, silme veya commit yapmadım.

## Taramanın kapsamı

research altındaki 385 dosyanın envanterini çıkardım: 19 Markdown, 357 PNG ve yardımcı dosyalar. Belgeler arası tutarlılığı, kanıt atıflarını, denetim scriptini ve kritik BusinessFinance karşılaştırmalarını mevcut kodla kontrol ettim.

357 görselin tamamına dosya bütünlüğü ve birebir kopya kontrolü uyguladım; şüpheli tekrarları ve seçilmiş kritik kareleri görsel olarak karşılaştırdım. 357 karenin tamamını tek tek içerik açısından doğruladığımı söylemiyorum. Rakip uygulamalarda yeniden canlı test veya güncel internet doğrulaması yapmadım.

## 1. BusinessFinance hakkında yanlış bilgiler var — yüksek önem

Bluecoins formu, bizim modelimizin tahsilatı belirli bir faturaya bağladığını ve “bu faturadan ne kaldı” takibini garanti ettiğini söylüyor. Ancak CounterpartyPayment böyle bir bağlantı taşımıyor; mimari belgesi de ödemelerin tek bir borçlandırmaya bağlanmadığını açıkça belirtiyor.

Bu, rakipteki eksikliği bizim üründe çözülmüş göstererek karşılaştırmanın sonucunu değiştiriyor.  
Bluecoins iddiası (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/bluecoins.md:335`) · Mevcut mimari (`/C:/Users/elma6/Documents/BusinessFinance/documentation/architecture.md:569`)

KolayBi formunda da mevcut kodla çelişen bilgiler var:

| Araştırmada yazan | Mevcut kod |
| :--- | :--- |
| Bizde asgari ödeme oranı yok | `MinimumPaymentRate` var |
| Kart limiti bizde zorunlu değil | `CreditCard`, zorunlu `Money` limit alıyor |
| Kullanılabilir limit gösterimi yok | Hesaplama ve API alanı mevcut; istemci gösterimi ayrıca doğrulanmalı |
| “Bizde ₺0 tutarlı kayıt olurdu” | `Money`, sıfır ve negatif tutarı reddediyor |

KolayBi kart karşılaştırması (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/kolaybi.md:314`) · Kart modeli (`/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Domain/CreditCard.cs:55`) · Sıfır tutar iddiası (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/kolaybi.md:386`) · Money doğrulaması (`/C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Domain/Money.cs:10`)

## 2. Goodbudget’ta bilinmeyen neden, kesin davranışa dönüşmüş — yüksek önem

Form, ₺600 bakiye düşüşünün nedenini açıkça “doğrulanamadı” diye kaydediyor. Fakat ilerleyen bölümlerde aynı fark:

> Tekrarlayan planın sonraki örneği, işlem listesine girmeden bakiyeyi değiştirdi.

şeklinde kesinleştirilmiş ve “Alma” kararına gerekçe yapılmış.

Tutarın abonelikle aynı olması, değişikliğin abonelikten kaynaklandığını kanıtlamaz. Bakiye farkı gözlem olarak korunabilir; neden ve mekanizma henüz açık.

Belirsizliğin kaydı (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/goodbudget.md:73`) · Kesinleştirilmiş karar (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/goodbudget.md:253`)

Ayrıca gelir testi Expense/Credit yolundan yapılmış; aynı belgede ayrı “From New Income” yolu bulunduğu yazıyor. Bu alternatif açıklığa kavuşmadan “gelir girişi zorunlu olarak harcama zarfına bağlanıyor” sonucunu bütün ürüne yaymak güvenli değil.  
Alternatif giriş yolu (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/goodbudget.md:217`)

## 3. KolayBi’de demo görüntüsü, kullanıcı davranışı kanıtı sayılmış

Demo proje adlarında “Ev Elektrik”, “Bebek Bakım” gibi isimlerin bulunmasından:

- Kullanıcıların işletme/şahsi ayrımı yapmak istediği,
- Proje modülünü tasarlanmış amacı dışında kullandığı,
- Bunun bizim ürün tezimizin güçlü kanıtı olduğu

sonuçlarına geçiliyor.

Görüntü yalnız bu adlarla demo kayıtları bulunduğunu kanıtlıyor. Kayıtları kimin, hangi amaçla oluşturduğunu veya gerçek kullanıcı ihtiyacını kanıtlamıyor. Bu bölüm araştırma bulgusundan çok test edilmesi gereken ürün hipotezi olmalı.  
İlgili bölüm (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/kolaybi.md:234`)

## 4. Eski–yeni kare karşılaştırmasının gerekçeleri hâlâ sorunlu

KolayBi örtüşme tablosunda iki mantık hatası var:

- Ortaklar sekmesinin 2020’de bulunması, çözümün ürünün “baştan beri verdiği cevap” olduğunu kanıtlamaz. Yalnız 2020’de mevcut olduğunu gösterir.
- Eski Finans karesi olmadan “kredi kartı yok” iddiasının yanlışlığının gösterilemeyeceği söyleniyor. Güncel kredi kartı ekranı bu iddiayı zaten çürütür. Eski kare ancak tarihsel karşılaştırma için gereklidir.

Ayrıca 07 Kasalar, d19 Banka Hesapları ekranı. Menü karşılaştırması yapılabilir; bunlar aynı sekmenin eski–yeni görüntüsü değildir.

Dolayısıyla “silinmesi zorunlu” da diyemem, “hepsi mutlaka kalmalı” da. Kalma gerekçesi, araştırmada gerçekten gerekli benzersiz katkı olmalı.  
Örtüşme tablosu (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/kolaybi.md:612`)

## 5. Yanlış adlandırılmış ve tekrar eden görseller gerçekten var

Görsel karşılaştırmada doğruladığım örnekler:

| Dosyalar | Gerçekte görünen |
| :--- | :--- |
| Bluecoins `f7-31-filtre`, `f7-32-menu`, `f7-53-export` | Aynı filtre ekranı; “menu” ve “export” adları içeriği yansıtmıyor |
| Bluecoins `f7-06-ofis-kirasi-detay`, `f7-07-tap-icon`, `f7-08-tap-retry` | Detay ekranı yerine aynı hatırlatıcı listesi |
| Wallet `f7-39-i-lent-form2`, `f7-40-i-lent-form3` | Aynı borç kaydı seçim ekranı |

Örneğin Bluecoins “export” karesi (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/kanitlar/bluecoins/f7-53-export.png`), dışa aktarma seçeneklerini göstermiyor.

Bunlar otomatik silme listesi değil: başarısız dokunma veya önce–sonra kontrolü için saklanabilirler. Ancak süreç kaydı ile ürün özelliği kanıtı ayrılmalı.

## 6. Bluecoins’te aritmetik hata ve kapanmamış davranış iddiaları var

- ₺600 giderin etkisi anlatılırken net varlık ₺44.350 → ₺43.350 yazılmış. Bu değişim ₺1.000; anlatılan işlemle uyuşmuyor.  
  Sayısal hata (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/bluecoins.md:154`)
- Bir yerde silme “geri alınamaz”, başka yerde çöp kutusu nedeniyle silinen kayıtların oraya düşüyor olabileceği ve bunun doğrulanamadığı yazıyor. Geri yükleme denenmeden kalıcı silme hükmü kapanmış sayılmamalı.  
  Kesin hüküm (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/bluecoins.md:77`) · Açık belirsizlik (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/bluecoins.md:298`)
- “Tekrarlayan/taksit tanımı hiçbir şey üretmez” özeti, ilk taksidin anında oluştuğunu anlatan kendi test kaydıyla uyumsuz. Tekrarlayan plan ve taksit ayrı değerlendirilmeliydi.  
  Test kaydı ve genelleme (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/bluecoins.md:348`)

## 7. Wallet’ın model karşılaştırması kendi içinde uzlaştırılmamış

Bir bölümde Wallet’ta ADR 0014’ün “tanır/taşır” ayrımının bulunmadığı söyleniyor; sonraki bölümde aynı ürün bu ayrımı doğal olarak destekleyen tek rakip ilan ediliyor.

İkinci koşum ilk sonucu değiştirmiş olabilir. Sorun, eski hükmün korunması ve yeni sonucun hesap bakiyesi, borç bakiyesi ve raporlama etkileri ayrı ayrı karşılaştırılmadan genelleştirilmesi.

“Borç açarken hesap bakiyesini değiştirmeme seçeneği” ile “aynı finansal model” eşdeğer değil.  
İlk değerlendirme (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/wallet-budgetbakers.md:206`) · Sonraki değerlendirme (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/gozlemler/wallet-budgetbakers.md:267`)

## 8. Ayrı karşılaştırma raporu, düzeltilmiş formlarla güncellenmemiş

`turk-on-muhasebe-vs-businessfinance.md` içinde:

- KolayBi’nin çek/senet hücresi hâlâ —.
- Formlardan doğrulanmamış diye çıkarılan fiyat rakamları duruyor.
- Logo görselinden hareketle üç ürünün muhasebecisine düzenleme/silme yetkisi genelleniyor.
- Bunların tamamı topluca “Resmî kaynak” etiketi taşıyor.

Belge 2 Eylül tarihli; fakat sonraki düzeltmeler karşısında tarihsel/yerine yenisi geçmiş belge olarak açıkça ayrılmamış. Böylece yanlış bilgi özet üzerinden yeniden kullanılabilir.  
Karşılaştırma raporu (`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/raporlar/turk-on-muhasebe-vs-businessfinance.md:21`)

## 9. Düzeltilmiş denetim scripti de güvenilir bir kapanış kapısı değil

`/C:/Users/elma6/Documents/BusinessFinance/research/rakip-arayuz-ve-akis/denetim.sh:35` üzerinde doğruladığım sorunlar:

- `f7-...` dosyalarının kısa kodunu yalnız `f7` olarak çıkarıyor. Tek bir `f7-...` atfı, başka kareleri de “kısa kodla anılmış” sınıfına sokabiliyor.
- Tam dosya gövdesinin metinde herhangi bir yerde bulunması yeterli; diğer kısa/yanlış kullanımları denetlemiyor.
- Yol içeren bazı dosya atıfları ölü atıf kontrolünün dışında kalıyor.
- Yasak aralıkların açıklama amaçlı örneklerini gerçek ihlal gibi yakalıyor.
- Sorun bulduğunda bile çıkış kodu 0. KolayBi koşumunda bunu doğrudan gördüm.

Dolayısıyla scriptin verdiği “93/77 kısa kod” sayıları, tek tek doğrulanmış kanıt bağlantısı sayıları değil.

Bağımsız tam-dosya-adı taramam:

| Uygulama | PNG | Kendi formunda tam adı bulunmayan |
| :--- | :---: | :---: |
| Wallet | 106 | 97 |
| Bluecoins | 90 | 80 |
| Money Manager | 30 | 4 |
| Diğer altı uygulama | 131 | 0 |
| **Toplam** | **357** | **181** |

Bu 181 yanlış görsel demek değil; 181 görselin tam dosya adıyla izlenebilirliği eksik.

Ek olarak Bluecoins, Wallet ve Money Manager’ın arayüz tablolarında üç sütunluk başlığa karşı dört sütunluk satırlar var. Kanıt sütunu Markdown görüntüleyicide kaybolabilir.

## Son değerlendirme

Olumlu taraf: bozuk PNG, birebir dosya kopyası veya taradığım açık PNG adlarında diskte bulunmayan dosya çıkmadı. Önceki düzeltmelerin bir kısmı gerçekten yararlı.

Ama asıl açık dosya temizliği değil, kanıttan sonuca geçişin güvenilirliği. “Tamamlandı” görünen KolayBi ve Goodbudget dâhil, kararları etkileyen hatalar duruyor.

Sıradaki tek iş: ürün kararlarına taşınan iddiaları yeniden doğrulamak; önce BusinessFinance karşılaştırmaları ve Goodbudget’ın neden–sonuç hükümleri. Dosya adı ve tekrar temizliği bunun ardından gelmeli. Bu inceleme salt okunurdu; build/test çalıştırmadım.