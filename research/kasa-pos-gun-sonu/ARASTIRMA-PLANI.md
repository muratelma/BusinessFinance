# Kasa ve POS: esnafın gün sonu — araştırma planı

**Durum: tamamlandı, 28 Eylül 2026.** Bulgular `BULGULAR.md` ve `GUN-SONU-BELGELERI.md`; kararlar
`KAPANIS.md`. (P4: konuşma geçmişi dosyası kullanıcınındır, `stages/` altında kaldı.)

**Başlangıç girdisi:** `stages/Gelistirici_Asistani_Konusma_Gecmisi.md` (kullanıcının başka bir
yapay zekâyla yaptığı sohbet). Kullanıcı kararıyla **hiçbir iddiası doğru ya da yanlış kabul
edilmez**; iddiaları §6'da hipotez olarak sınanır.

---

## 1 · Neden bu çalışma

`Kasa` alt çubuğun dört sekmesinden biri (işletme profilinde üçüncü sekme, ADR 0015), ama içeriği
bunu hak edecek derinlikte değil: gün sonu sayımı ve elle girilen POS tahsilatı. Kasa ve POS
incelediğimiz dokuz rakipte yok; ürünü ayıracak bölüm buysa önce **gerçekten işlevli** olmalı.

Çalışmanın çıktısı **Kasa sekmesinin genel yapısıdır**: ne taşıdığı, hangi akışla çalıştığı ve alt
çubukta kalıp kalmadığı (ya da yerine başka bir şeyin gelip gelmediği).

## 2 · Kullanıcının sabitlediği çerçeve (28 Eylül 2026)

| # | Karar | Araştırmaya etkisi |
|---|---|---|
| Ç1 | Belirli bir sektör hedeflenmiyor | Tek iş akışı değil, tahsilat kanallarının çeşitliliği incelenir; fark ayarla ifade edilir |
| Ç2 | Satışların tek tek girilmesi beklenmez | Gün sonu toplamı birinci sınıf giriş olur; tekil satış istisnadır |
| Ç3 | İleride bir entegratörle banka entegrasyonu olabilir (bugün kapsam dışı) | Model, "gerçekleşen" tarafın bir gün dışarıdan dolabileceği varsayımıyla kurulur |
| Ç4 | ADR ve aşama kuralları bu çalışmada engel değil | Öneri bir ADR'yi değiştirebilir; koda geçerken yeni ADR ve aşama belgesi yine yazılır (kayıt için) |

## 3 · Bugünkü durum (koddan doğrulandı, 28 Eylül 2026)

**POS tahsilatı** (`Domain/PosSettlement.cs`):
- Brüt tutar, komisyon (tutar ya da oran; oran sunucuda tutara çevrilir, saklanmaz), tahsilat günü,
  beklenen geçiş günü, banka hesabı, satış kategorisi, komisyon kategorisi, kapsam, KDV.
- Tahsilat günü gelir brüt kadar tanınır, komisyon ayrı gider; bakiye kıpırdamaz. Geçiş günü hesap
  net kadar artar (ADR 0014/0015).
- `Hesaba geçti` **yalnız tarih alır, tutar almaz**: bankanın farklı tutar yatırması kaydedilemez.
- Her kayıt tek tek işaretlenir; toplu geçiş yok.
- Tek geçiş günü; taksitli ya da parçalı geçiş yok.
- Formda **canlı net tutar önizlemesi yok**; net yalnız kaydettikten sonra detayda görünür.
- Yoldaki para net varlıkta; **planlanan/yaklaşanlar görünümünde beklenen tahsilat yok**
  (`EfPlannedActivityRepository` POS okumuyor).

**Kasa sayımı** (`Domain/CashCount.cs`, `Application/Cash`):
- Beklenen bakiye = nakit hesabın girilen hareketlerden hesaplanan bakiyesi. Sayılan tutar girilir,
  fark gösterilir; fark kullanıcı seçtiği kategoriyle gelir/gider olarak kaydedilebilir.
- **Ç2 ile çelişki:** nakit satışlar tek tek girilmezse beklenen bakiye satışları bilmez ve her gün
  büyük bir "fazla" çıkar. Bu fark gerçekte günün satışıdır, açık/fazla değildir.

**Kasa ile POS birbirinden habersiz** iki ayrı özellik; ikisini bağlayan bir "gün" kavramı yok.

### 3.1 · Nakit ve banka: iki kasa (kullanıcı gözlemi, 28 Eylül 2026)

Bugünkü yapıda `Kasa` dükkânın **nakit** tarafını, `POS tahsilatları` **kart/banka** tarafını
karşılıyor. Kullanıcının gözlemi: nakit tarafında bir kontrol noktası (sayım) var, banka tarafında
yok. Sınanacak çerçeve:

| | Nakit | Banka |
|---|---|---|
| Beklenen | Kayıtlardan hesaplanan kasa | Kayıtlardan hesaplanan bakiye + yoldaki POS |
| Gerçek | Sayılan para | Banka uygulamasındaki bakiye / ekstre |
| Kontrol | Kasa sayımı (var) | Banka mutabakatı (**yok**) |
| Entegrasyon gelirse | Değişmez — nakit bankadan geçmez | "Gerçek" satırı dışarıdan dolar |

Bu çerçeveden çıkan sorular:
- **Nakit tarafı entegrasyonla çözülmez; gün sonu formülüyle çözülür.** Bağımsız bir satış kaynağı
  (Z raporu) varsa sayım farkı gerçek açık/fazladır. Yoksa satış sayımdan türetilir ve açık
  yakalanamaz. Bu ödünleşim kullanıcıya nasıl gösterilir?
- **Banka tarafında entegrasyon satışı değil yatışı getirir** (gecikmeli, net, çoğu zaman toplu).
  Yatış ADR 0014'e göre para taşımasıdır, gelir/gider üretmez; satış günü brüt tutar yine bir gün
  sonu girişinden gelir. Yatış onunla eşleştirilir. Üye işyeri verisi satış satış detay veriyorsa
  bu değişir (§4-D, F).
- **Elle banka mutabakatı bugün de yapılabilir:** kullanıcı gerçek bakiyeyi yazar, uygulama farkı ve
  geçmiş olabilecek yoldaki POS kayıtlarını gösterir. Entegrasyonda aynı ekranın "gerçek" satırı
  otomatik dolar.
- `Kasa` sekmesi "nakit sekmesi" değil "iki kasanın gün sonu" olursa alt çubukta kalmayı hak eder mi?

**Var olan ve işe yarayabilecek parçalar:** banka ekstresi CSV içe aktarımı (ayrıştırıcı), dekont
okuma (`bank-slip`), fiş okuma, KDV taşıma (ADR 0016), muhasebeci paketi.

## 4 · Araştırma soruları

### A · Tahsilat kanalları
Esnaf parayı hangi kanallardan alıyor ve her kanalda para, komisyon ve bilgi nasıl akıyor?
- Banka POS'u (fiziksel), yeni nesil yazar kasa POS (YN ÖKC), sanal POS / linkle ödeme,
  SoftPOS ve ödeme kuruluşları, yemek kartları, havale/EFT/FAST, karekod.
- Her kanal için: komisyon yapısı, geçiş süresi, esnafın veriyi nereden gördüğü.

### B · Komisyon ve geçiş süresi
- Ertesi gün, blokeli ve ara modeller; oran ile süre arasındaki ilişki gerçekten "tahterevalli" mi.
- Kart türüne göre fark: banka kartı / kredi kartı, yurt dışı kart, ticari kart, kendi bankanın kartı.
- Taksitli satış: komisyon ve paranın taksit taksit geçip geçmediği.
- Komisyon üzerindeki vergi (BSMV), sabit işlem ücretleri, aylık komisyon faturası.
- İş günü mü takvim günü mü; resmî tatiller.

### C · Gün sonu belgeleri
- POS gün sonu (batch) fişinde ne var, ne yok.
- Z raporunun içeriği: nakit/kart ayrımı, KDV dağılımı, iptal/iade; X raporu; mali hafıza.
- Muhasebeci Z raporunu nasıl kaydediyor (günlük satış toplamı olarak mı).
- Gün sonu alınmazsa para gerçekten geçmiyor mu.

### D · Banka tarafı ve mutabakat
- POS yatışı hesap hareketinde nasıl görünür: tek toplu satır mı, açıklama metni ne.
- Üye işyeri paneli ve raporları; esnafın indirebildiği biçimler.
- Bizim ekstre içe aktarıcımız POS yatışlarını tanıyıp beklenenle eşleştirebilir mi.
- İade, ters ibraz (chargeback) ve bankanın sonradan kestiği tutarlar.

### E · Nakit ve kasa
- Esnaf nakit satışını gerçekte nasıl izliyor: kasa defteri, açılış/kapanış, gün sonu sayımı.
- Bankaya nakit yatırma, kasadan tedarikçiye ödeme, kasadan şahsi çekim (ADR 0013 tek havuz ile bağı).
- Sabah kasası (bozuk para / devir) ve kasa avansı.
- Satışları gün sonu toplamıyla giren bir akışta sayım farkı nasıl anlamlı hâle gelir.

### F · Entegrasyon ufku (bugün kapsam dışı, modeli etkiliyor)
- Türkiye'de açık bankacılık: hesap bilgisi hizmeti, lisanslı sağlayıcılar, BKM GEÇİT; entegratörler.
- Bankaların üye işyeri API'leri; YN ÖKC verilerinin GİB'e gidişi ve üçüncü tarafa açılıp açılmadığı.
- Entegre olunduğunda modelin hangi alanı otomatik dolar; bugün elle dolan alanla aynı alan mı.

### G · Başkaları nasıl yapıyor
- Türk ön muhasebe ürünleri (KolayBi, Paraşüt, Logo İşbaşı ve benzerleri): POS'u hesap mı, ara
  hesap mı, tahsilat türü mü modelliyor. Araştırma klasöründeki KolayBi ve İşbaşı notları başlangıç.
- YN ÖKC üreticilerinin ve bankaların esnaf uygulamaları: gün sonu ve raporları nasıl sunuyor.

### H · Kendi uygulamamız
- Bir haftalık sentetik esnaf senaryosu (nakit ve kartlı satış, yemek kartı, iade, bankaya nakit
  yatırma, tedarikçiye kasadan ödeme, farklı süreli POS geçişleri) Pixel 8'de koşturulur.
- Her adım için: yapılabiliyor mu, kaç dokunuş, sonuç doğru mu, kullanıcı neyi göremiyor.

### I · Kasa sekmesinin yeri
- Sekme "gün" merkezli mi olmalı (bugün ne satıldı, ne tahsil edildi, ne yolda, kasada ne var)?
- Tahsilat merkezli mi (kasa, POS, cari tahsilat, havale bir arada)?
- Yoksa `Diğer` altına mı inmeli, alt çubuğa başka bir şey mi gelmeli?

## 5 · Yöntem ve kanıt düzeyi

- **Kaynak sırası:** resmî düzenleme (GİB, BDDK, TCMB, BKM) > bankanın kendi sayfası ve tarifesi >
  ödeme kuruluşu / üretici belgesi > muhasebeci yayınları > esnaf forumları ve şikâyet siteleri >
  yapay zekâ sohbetleri (yalnız hipotez).
- Her bulgu kaynak adresi ve erişim tarihiyle yazılır. Oran ve süre gibi değişken sayılar örnek
  olarak geçer; uygulamaya gömülmez (ADR 0016 ilkesi: oran kullanıcınındır).
- Uygulama tarafı (§3, H) koddan ve emülatörden doğrulanır; tahminle yazılmaz.
- Rakip araştırmasındaki tarafsızlık kuralı geçerli: kendi ADR'lerimiz ölçüt değil,
  karşılaştırmanın bir tarafıdır.

### 5.1 · Değer sınavı (kullanıcı kuralı, 28 Eylül 2026)

Araştırma her bulgu ve öneri için yalnız "doğru mu" değil, **"yapmaya değer mi"** diye de sorar.
Karar belgesindeki her seçenek şu dört soruyla değerlendirilir:

| # | Soru | Ne ölçülür |
|---|---|---|
| D1 | **Hangi ihtiyacı karşılıyor?** | Esnafın gerçekte yaşadığı somut bir sorun mu, yoksa "olsa iyi olur" mu. Kaynağı var mı |
| D2 | **Yeterli en az detay ne?** | Aynı ihtiyacı daha az alan, daha az adımla karşılayan bir sürüm var mı. Her ek alan, kullanıcı onu doldurmazsa ne kaybeder |
| D3 | **Detay işlevli mi?** | Kullanıcı o alanı gerçekten dolduracak mı, her gün mü, ayda bir mi. Doldurulmayan detay veriyi değil gürültüyü artırır |
| D4 | **Maliyetine değer mi?** | Yapım (migration, ekran, test) ve kullanıcıya bindirdiği yük, kazandırdığıyla karşılaştırılır |

Sonuç her seçenek için üç kovadan biridir: **yap** · **sade sürümünü yap** · **yapma** (gerekçeli).
Bir detay "doğru ama değmez" çıkabilir; bu meşru bir sonuçtur ve yazılır.

## 6 · Sınanacak hipotezler

Sohbetten gelenler (Konuşma Geçmişi):

| # | Hipotez |
|---|---|
| H1 | POS gün sonu fişinde komisyon ve net tutar yazmaz; bunlar yalnız banka panelinde görünür |
| H2 | Gün sonu alınmazsa satışlar hesaba geçmez ya da gecikir |
| H3 | Üç model var: ertesi gün (komisyonlu), blokeli (komisyonsuz), ara (kısa bloke, düşük komisyon) |
| H4 | Blokeli çalışmada komisyon sıfırdır |
| H5 | Oran ile süre ters orantılıdır |
| H6 | Anlaşmalar esnafa özeldir; süre ve oran uygulamada tahmin edilemez, kullanıcıdan alınmalıdır |
| H7 | Süre bazı bankalarda iş günüyle, bazılarında takvim günüyle sayılır |

Bizim eklediklerimiz:

| # | Hipotez |
|---|---|
| H8 | Z raporu nakit ve kartlı satışı ayrı verir; tek bir gün sonu girişi hem kasayı hem POS'u besleyebilir |
| H9 | Bankanın yatırdığı tutar hesaplanan netten farklı olabilir (yuvarlama, BSMV, iade, kesinti) |
| H10 | Banka bir günün POS satışlarını tek toplu hareket olarak yatırır |
| H11 | Taksitli satışta komisyon farklıdır ve para taksit taksit geçebilir |
| H12 | Komisyon kart türüne göre değişir |
| H13 | Yemek kartları POS'a benzer ama ayrı bir kanal gibi davranır (yüksek komisyon, uzun süre) |
| H14 | Esnafın çoğu nakit satışı tek tek kaydetmez; gün sonu toplamı ya da Z raporu kullanır |
| H15 | Banka hesap hareketinde POS yatışı satış gününü ve brüt tutarı taşımaz; satış tarafı entegrasyonla da bir gün sonu girişine ihtiyaç duyar |
| H16 | Esnaf banka bakiyesini uygulamadaki kayıtlarla karşılaştırmaya ihtiyaç duyar (banka tarafının sayımı); bugün bunu yapan bir yer yok |

## 7 · Çıktılar

1. **Bulgular** (`BULGULAR.md`): soru soru, kaynaklı; her hipotezin sonucu (doğrulandı / çürütüldü /
   kısmen / kanıt yok).
2. **Karar belgesi** (`KARAR.md`): seçenekli ve önerili. Taslak eksenler:
   - **Model:** A · bugünkü model + iyileştirmeler (canlı net, toplu geçiş, gerçekleşen tutar) ·
     B · gün sonu kapanışı (Kasa ile POS tek akışta) · C · B + tahsilat kanalı tanımları (POS
     cihazı, yemek kartı) + mutabakat.
   - **Sekme:** gün merkezli · tahsilat merkezli · `Diğer`e iner.
   - **Entegrasyona hazırlık:** beklenen / gerçekleşen / mutabakat ayrımı nerede durur.
3. Karar verilince: yeni ADR (ADR 0015'i genişleten ya da değiştiren), aşama belgesi, tasarım brifi.

## 8 · Sıra

| Adım | İş | Kullanıcı |
|---|---|---|
| 1 | Bu planın onayı ve §9 kararları | Karar verir |
| 2 | Masa başı araştırma: A–G, hipotezler | — |
| 3 | Kendi uygulamamızın senaryo koşumu (H) | Gerekirse emülatörde kontrol |
| 4 | Bulgular belgesi | İnceler |
| 5 | Karar belgesi (I dahil) | Karar verir, dış göze inceletebilir |
| 6 | ADR + aşama belgesi, sonra kod ve tasarım | Onaylar |

## 9 · Açık kararlar

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| P1 | Belgelerin biçimi | A · Markdown · B · Belge 1/2 gibi PDF | **A.** Hızlı döner, karar kısa sürede çıkar; gerekirse sonradan PDF'e dökülür |
| P2 | Senaryo koşumu (H) ne zaman | A · masa başı araştırmayla paralel · B · bulgulardan sonra | **A.** Bugünkü eksikleri erken görmek araştırma sorularını keskinleştirir |
| P3 | Entegrasyon ufku (F) ne kadar derin | A · yalnız modeli etkileyen kadar · B · sağlayıcı ve maliyet düzeyinde | **A.** Bugün kapsam dışı; amaç modelin kapıyı kapatmaması |
| P4 | Konuşma geçmişi dosyasının yeri | A · `stages/` altında kalır · B · bu klasöre kaynak olarak taşınır | **B.** Aşama belgesi değil, araştırma girdisi; `stages/` yalnız aşama belgelerini tutuyor |
