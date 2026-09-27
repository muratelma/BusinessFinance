# Bölüm 1 · Okuma kılavuzu ve kanıt düzeyleri

Belge 1 · Rakip arayüz yaklaşımları · 16 Eylül 2026

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Bu belge neyi, hangi kanıtla anlatıyor ve nasıl okunur?

Belge 1, dokuz rakip ürünün arayüz yaklaşımlarını konu konu anlatır: ana ekran, kayıt formu, hesap ve kart, sınıflandırma, planlama, borç, rapor, veri aktarımı ve yardımcı araçlar. Ürünleri puanlamaz ve sıralamaz; her yaklaşımı ekranda görüldüğü gibi tarif eder.

Bir kaydın bakiyeye ve rapora etkisi Belge 2'nin, bu bulguların BusinessFinance için ne anlama geldiği Belge 3'ün konusudur. Bu belgede "→ Belge 2" yazan yerler o sınırı gösterir.

Canlı incelemeler 1–12 Eylül 2026 arasında Android emülatörde yapıldı; kullanıcı kontrolleri 14–15 Eylül'de eklendi. Test verisi sentetiktir: aynı senaryo (Ada Reklam geliri, market gideri, kart harcaması ve ödemesi, hesaplar arası aktarım, abonelik, taksit, alacak ve kısmi tahsilat) her ürüne, ürünün izin verdiği ölçüde girildi.

**Bu bölüme girmez**

- Ürünlerin başarısı, hızı, memnuniyeti, erişilebilirliği
- Kaydın finansal sonucu → Belge 2
- BusinessFinance için tercih → Belge 3

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare, Koşum kaydı |  |
| Bluecoins | Canlı kare, Koşum kaydı |  |
| Wallet | Canlı kare, Koşum kaydı |  |
| Hesap Defterim | Canlı kare, Koşum kaydı |  |
| Goodbudget | Canlı kare, Koşum kaydı |  |
| KolayBi | Kaynak görseli, Kaynak beyanı, Koşum kaydı |  |
| Paraşüt | Kaynak beyanı, Koşum kaydı | Kareleri basılmıyor. |
| Logo İşbaşı | Kaynak beyanı, Koşum kaydı | Kareleri basılmıyor. |
| QuickBooks Solopreneur | Kaynak beyanı | Kareleri basılmıyor. |


## 1.1 · Ne incelendi?

Beş ürün emülatörde canlı açıldı; dört ürün yalnız kendi yayımladıkları kaynaklardan incelendi. Kare sayıları kanıt envanterindendir.

| Ürün | Platform ve sürüm | Erişim | Tarih | Kare | Belgede basılan |
|---|---|---|---|---|---|
| **Canlı incelenen ürünler** | | | | | |
| Money Manager | Android 4.12.8, Türkçe arayüz | Canlı; kayıt ve giriş yok | 1, 10 Eylül | 30 | Kareler |
| Bluecoins | Android 13.1.45, ağırlıkla Türkçe | Canlı, derin; kayıt ve giriş yok | 1, 10, 11 Eylül | 92 | Kareler |
| Wallet (BudgetBakers) | Android 9.3.6, İngilizce arayüz | Canlı, derin; bulut hesabıyla | 1, 10, 11 Eylül | 109 | Kareler; hesap sahibinin adını taşıyan kare hariç |
| Hesap Defterim | Android, Türkçe arayüz | Canlı; kayıt ve giriş yok | 10–12 Eylül | 45 | Kareler |
| Goodbudget | Android 2.24.26013, İngilizce arayüz | Canlı; ücretsiz paket sınırlı | 11–12 Eylül | 31 | Kareler; hane adı karartılarak |
| **Kaynakla incelenen ürünler** | | | | | |
| KolayBi | Web paneli; mobilde yalnız giriş ekranı | Destek sayfası görselleri ve tanıtım videosu | 1–12 Eylül | 39 | Yalnız 31 destek görseli |
| Paraşüt | Android giriş ekranları; kılavuz | Hesap açılamadı; kılavuz ve tanıtım videosu | 1–12 Eylül | 9 | Hiçbiri |
| Logo İşbaşı | Android giriş ve kayıt ekranları | Ürün sayfaları ve tanıtım videosu | 1–12 Eylül | 6 | Hiçbiri |
| QuickBooks Solopreneur | — | Yalnız yardım merkezi; ürün ABD dışına kapalı | 1–12 Eylül | 4 | Hiçbiri |

- Toplam 365 kare incelendi ve envantere işlendi. Kullanıcı kontrolüyle eklenen kareler de bu sayıya dahildir.
- Sürüm ve dil ayrıntısı ile her ürünün bilinmeyenleri Bölüm 2'deki kartlardadır.

*Dayanak.* Koşum kaydı: E0010, E0005, E0014, E0007, E0006, E0008, E0011, E0009, E0012 (gözlem formları).

## 1.2 · Kanıt türleri

Her cümlenin yanında ya da sayfanın altındaki dayanak bandında beş türden biri yazar. Tür ürüne değil ifadeye bağlıdır: canlı incelenen bir ürün hakkındaki her cümle sınanmış değildir.

### Canlı kare *(Canlı kare)*

Emülatörde açılan ekranın görüntüsü. Belgede basılır; üstündeki numaralı işaretler metinle eşleşir.

### Koşum kaydı *(Koşum kaydı)*

Gözlem formunda yazılı; karesi yok ya da kare tek başına göstermiyor. Kullanıcının sonradan yaptığı ekran kontrolleri de bu türdedir.

### Kaynak görseli *(Kaynak görseli)*

Ürünün kendi arayüzünü anlatmak için yayımladığı ekran görüntüsü. Bu belgede yalnız KolayBi'nin destek sayfası görselleri. Demo verisi taşır; güncel sürümü gösterdiği doğrulanmadı.

### Kaynak beyanı *(Kaynak beyanı)*

Ürünün kılavuzu, yardım merkezi veya ürün sayfası metni. Görsel yok; anlatılan davranış çalışırken görülmedi.

### Görülmedi *(Görülmedi)*

Kanıt yok. Özelliğin bulunmadığı anlamına gelmez: yalnız incelenen sürümde, incelenen yüzeyde ve incelenen kaynakta görülmediğini söyler.

### "Görülmedi" ile "yok" ayrıdır

"Yok" ancak karede ya da koşumda yokluk gözlendiyse ve kapsamıyla birlikte yazılır: "incelenen form ve kayıt ayrıntısında taksit alanı yok" gibi.

"Her ekranda", "tek ürün", "hiçbirinde" gibi nicelikler yalnız o kapsamı taşıyan kanıt gösterildiğinde kullanılır; aksi hâlde cümle gözlenen kapsamla sınırlanır.


## 1.3 · Neden bazı kareler basılmıyor?

İncelenen her kare belgeye girmez. Basılmayan bir kareye dayanan cümle kaynak niteliğini kaybetmez; kimliği dayanak bandında ve bölümün kaynaklar dosyasında kalır.

### Paraşüt ve Logo İşbaşı

Paraşüt'ün 9 karesinin 2'si, Logo İşbaşı'nın 6 karesinin 4'ü canlı yakalanmış giriş ve kayıt ekranıdır; geri kalanlar tanıtım videosu karesidir.

Hiçbiri ürünün iç arayüzünü temsil etmiyor. Bu yüzden kareler basılmıyor; gözlemler ve kaynak beyanları metinde kalıyor.

### Bir tanıtım karesi ne kadar uzaktır?

Paraşüt'ün tanıtım videosundaki cari hesap ekranında sol menü kutuları etiketsiz çizilmiş; iki halka aynı tutarı gösteriyor, uyarı kutusundaki üçüncü tutar ikisiyle de uyuşmuyor ve tutarların yazımı geçerli bir Türkçe biçime uymuyor. *(Koşum kaydı)*

Böyle bir kare, ürünün bir ekranı olduğunu değil, ürünün kendini nasıl anlattığını gösterir.

### QuickBooks Solopreneur

Görülen 4 kare başka bir paketin kayıt ve plan ekranıdır. Solopreneur'ün hiçbir ekranı görülmedi; anlatım yalnız yardım merkezine dayanır.

### KolayBi

Destek sayfası görselleri basılır. Tanıtım videosu kareleri basılmaz, gerektiğinde anılır: aynı ekranın kaynaklarda iki farklı hâlde görünmesi, güncel sürümün doğrulanmadığını gösterir.

### Kişisel veri

Goodbudget'ın hane adını taşıyan kareleri karartılarak basılır. Wallet'ın çekmecesinin üst kısmı hesap sahibinin adını taşıdığı için hiç basılmaz; yerine çekmecenin alt kısmı kullanılır.

KolayBi destek görsellerindeki demo kişi adları ve iletişim bilgileri, içerik için gerekmediğinde bölüm kopyasında karartılır. Karartma yalnız kopyaya uygulanır; özgün kanıt değişmez.


*Dayanak.* Koşum kaydı: E0011, E0009, E0012, E0008. Paraşüt tanıtım videosu karesi: E0262 (basılmadı).

## 1.4 · Sayfalar nasıl okunur ve belge neyi ölçmez?

### Ekran ve işaret

Soru sayfalarında kare büyük basılır ve üstüne numaralı işaretler konur. Sağ sütundaki numaralı satırlar o işaretleri açıklar. İşaret gösterdiği şeyin üstüne değil, yanındaki boş alana konur.

Kareler aynı anın görüntüsü değildir; farklı ürünlerin sayıları birbiriyle karşılaştırılmaz.

### Notlar ve diğer ürünler

Numaralı satırların altındaki madde işaretli notlar, karede okunmayan bilgiyi ya da kareden çıkan sonucun sınırını yazar. "Aynı soruda diğer ürünler" listesi, karesi basılmayan ürünlerin aynı sorudaki durumunu kanıt türüyle verir.

### Dayanak bandı

Her sayfanın altında sayfanın kanıt kimlikleri ve "çıkarılmayan sonuç" yazar: sayfadaki gözlemlerden çıkarılmaması gereken sonuç.

### Göndermeler

"→ 8.4" başka bir bölümün alt sorusuna gönderir. Bir konunun ayrıntısı tek bir bölümdedir; öteki bölümler kısa bağlam verip oraya gönderir.

Ekranda görünen etiketler ürünün kendi dilinde ve yazımıyla aktarılır (ENVELOPES, Alındı, Hızlı İşlemler).

### Bu belge neyi ölçmez?

Başarıyı, hızı, memnuniyeti ve erişilebilirliği ölçmez; kullanılabilirlik testi değildir.

Ölçülmemiş bir etki için cümle kurulmaz. Ürünlerin iyi veya kötü olduğuna dair bir sıralama yapılmaz.

### Sıradaki belgeler

Belge 2 aynı akışların arkasındaki olay modelini ve toplamların içeriğini anlatır. Belge 3, iki belgenin bulgularını BusinessFinance için alma, uyarlama veya yeniden sorma kararlarına çevirir.


## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
