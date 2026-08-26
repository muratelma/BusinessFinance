# ADR 0016 — Vergi alanları taşır, hesaplamaz; oran ve tarih kullanıcınındır

- Durum: **Kabul edildi** (26 Ağustos 2026, Aşama 05 Grup 1)
- Bağlam: KDV alanları, indirilebilirlik, vergi/SGK takvimi ve ay sonu
  muhasebeci paketinin eklenmesi
- İlgili: ADR 0014 (ekonomik olay tanır, ödeme taşır), ADR 0013 (işletme ve
  şahsi tek havuzdur; kapsam bir raporlama boyutudur), ADR 0011 (fiş okuma bir
  öneri katmanıdır), ADR 0015 (POS komisyonu ayrı gider)

## Bağlam

Esnafın ay sonunda yaptığı iş bellidir: ayın belgelerini toplar, hangisinin
işle ilgili olduğunu ayırır ve muhasebecisine gönderir. Uygulama bugün bu işin
yarısını yapıyor — işletme hareketlerini biliyor, kapsamı taşıyor, belgeyi
kayda bağlıyor — ama KDV'yi taşımıyor, hangi giderin indirilebileceğini
bilmiyor ve gönderilecek paketi üretmiyor.

Boşluğu kapatmanın kolay ve yanlış yolu, uygulamayı bir vergi hesaplayıcısına
çevirmektir. Kolaydır çünkü formüller basit görünür: brüt tutarı `1,20`'ye böl,
farkı KDV yaz. Yanlıştır çünkü:

- **Oran sabit değildir ve zamanla değişir.** Bugün doğru olan bölme, oran
  değiştiğinde geçmişteki her kaydı sessizce yanlışlar. Uygulama içindeki bir
  sayı, mevzuatın kendisiymiş gibi davranmaya başlar.
- **Belgedeki KDV her zaman bölmenin sonucu değildir.** Farklı oranlı kalemler
  aynı fişte toplanır, yuvarlama farkı fiş üstünde durur, tevkifatlı ve
  istisnalı belgeler kuralın tamamen dışındadır. Hesaplanan sayı ile belgedeki
  sayı tuttuğunda kimse fark etmez; tutmadığında kullanıcı belgeye değil
  uygulamaya güvenir ve muhasebecisine yanlış rakam gönderir.
- **Beyan tarihleri idari kararla kayar.** Uygulamanın içine yazılmış bir
  takvim, ertelenen bir beyan döneminde kullanıcıyı yanlış tarihe hazırlar.

Bu ürün vergi mükellefi değildir, vergi danışmanı hiç değildir. Yaptığı iş,
kullanıcının bildiği bilgiyi kaydın üstünde tutmak ve ay sonunda derli toplu
teslim etmektir.

## Karar

### 1. Oranlar ve tarihler koda gömülmez; kullanıcınındır

- KDV oranı, beyan tarihi, SGK/Bağkur ödeme günü gibi **mevzuata bağlı hiçbir
  değer** koda sabit yazılmaz.
- Uygulama bir **başlangıç önerisi** sunabilir (tek dokunuşla kurulan takvim
  kalemleri, kategorinin varsayılan oranı). Öneri kurulduğu anda kullanıcının
  verisi olur; kullanıcı değiştirebilir, silebilir ve uygulama sonradan onu
  **kendiliğinden güncellemez**.
- Bu bilginin sahibi ekranda yazılıdır: *"Bu tarihler sizin girdiğiniz
  bilgilerdir; uygulama mevzuat takibi yapmaz."*

Reddedilenler ve gerekçeleri:

| Seçenek | Neden reddedildi |
|---|---|
| Güncel oran ve takvimi uygulama sürümüyle dağıtmak | Uygulama, güncel tutmayı taahhüt etmiş olur. Bir sürüm gecikmesi kullanıcının kaçırdığı bir beyan demektir; taahhüdü tutamayacağımız bir sözü vermeyiz |
| Oranları uzak bir yapılandırmadan çekmek | Aynı taahhüdün ağ üzerinden verilmiş hâli, üstüne yeni bir güven sınırı ve çevrimdışı davranış sorusu. Aşama 07'ya kadar zaten dış bağımlılık eklenmiyor |
| Hiçbir öneri sunmamak, her şeyi boş bırakmak | Dürüst ama işe yaramaz: kullanıcı dört kalemi elle kurmak zorunda kalır ve çoğu kurmaz. Öneri sunup sahipliği devretmek ikisinin arasını tutar |

### 2. Uygulama hiçbir vergi tutarını hesaplamaz

- KDV **tutarı** kullanıcıdan veya belgeden gelir. Oran alanı da taşınan bir
  alandır; uygulama oranı tutardan, tutarı orandan **türetmez**.
- İkisi birden doluysa uygulama tutarsızlığı **söyleyebilir** ("girilen oran bu
  tutarla uyuşmuyor"), ama kaydı **düzeltmez** ve kaydı reddetmez. Belgede ne
  yazıyorsa doğru olan odur.
- KDV alanları **nullable**'dır. Her harekette KDV yoktur; boş bırakmak eksik
  veri değil, meşru bir cevaptır.
- Fiş okuma bu kuralı değiştirmez: ADR 0011 gereği okunan KDV bir **öneridir**,
  forma yazılır, onaylayan kullanıcıdır.
- KDV alanı gelir/gider tutarını, bakiyeyi, bütçe ilerlemesini ve işletme netini
  **hiç etkilemez**. Kayıt tutarı brüttür ve brüt kalır; KDV onun üstünde
  taşınan bir bilgidir. Bu, ADR 0015'te komisyonun brüt tutardan ayrı okunmasının
  aynısıdır.

### 3. İndirilebilirlik kapsamdan ayrı, iki durumlu bir alandır

- Kapsam (`TransactionScope`) "bu para kimin?" sorusunu, indirilebilirlik "bu
  gider vergi matrahından düşülebilir mi?" sorusunu yanıtlar. İkisi farklı
  sorulardır: her işletme gideri indirilebilir değildir (trafik cezası işletme
  giderdir ama indirilemez). Tek alanda birleştirmek, ADR 0013'ün reddettiği
  "kapsamı başka bir şeyle temsil etme" hatasının tekrarı olurdu.
- Alan **yalnız işletme kapsamlı** kayıtta anlamlıdır; şahsi kayıtta sorulmaz.
- Alan **iki durumludur**. Kısmi indirilebilirlik oranı bu aşamada modellenmez:
  oran girmek hesaplamaya giden ilk adımdır ve kısmi durumların kuralı
  muhasebecinindir. Kısmi durum pakete **not** olarak taşınır.
- İndirilebilirlik **işletme netini değiştirmez**. Nakit esaslı işletme neti
  paranın hareketini ölçer; indirilebilirlik matrahı ilgilendirir ve matrah bu
  üründe hesaplanmaz. Etkilediği tek çıktı muhasebeci paketidir.

### 4. Takvim bir hatırlatmadır; muhasebeci paketi bir okumadır

- Vergi/SGK takvimi **yükümlülük beyanı değildir**. Mevcut tekrarlayan
  yükümlülük altyapısının üstüne kurulur; yeni bir zamanlayıcı yazılmaz.
  Kaçırılan tarihin sorumluluğu kullanıcıdadır ve bu ekranda yazılıdır.
- Ay sonu paketi **yeni bir hesaplama yolu değildir**: aynı ayın işletme
  raporunu okur ve toplamları onunla **birebir** tutar. İki ayrı toplama yolu
  bırakılmaz — ayrıştıkları gün hangisinin doğru olduğunu kimse bilemez.
- Pakete **şahsi hiçbir kayıt girmez**; bu bir test kapısıdır, yorum değil.
- Paket kullanıcının kendi cihazından paylaşılır. Sunucu üçüncü kişiye bir şey
  göndermez; muhasebeci hesabı ve paylaşım linki kapsam dışıdır.

## Sonuçları

- Gider/gelir üreten kayıtlar nullable KDV oranı ve KDV tutarı taşır; para
  kuralı aynen geçerlidir (`decimal(19,4)`, API'de dört ondalıklı string).
- İndirilebilirlik kendi alanıdır, kategori bazlı varsayılan taşır ve kullanıcı
  her kayıtta düzeltebilir.
- Takvim kalemleri yaklaşanlar listesine mevcut yolla düşer; ikinci bir
  "yaklaşan" kaynağı doğmaz.
- `SavingsGoal` kapsam kazanır ve "vergi karşılığı" hazır bir hedef olarak
  sunulur; yeni bir modül yazılmaz.
- Yedek şeması v10'a çıkar ve yalnız v10 okur.
- Arayüz metinleri "vergi hesabı", "matrah", "beyanname" gibi hesaplama iddiası
  taşıyan kelimeleri kullanmaz; ürün "kâr" demediği gibi vergi de demez.

## Bu ADR neyi karara bağlamaz

- Beyanname üretme, tevkifat, istisna ve amortisman kuralları: **kapsam
  dışıdır** ve bu üründe hiçbir aşamada eklenmez.
- e-Fatura / e-Arşiv / e-Defter entegrasyonu: kapsam dışıdır.
- Muhasebeci hesabı, çok kullanıcılı erişim ve paylaşım linki: kapsam dışıdır.
- Paketin dosya biçimi ve içeriğinin ayrıntısı: Aşama 05 Grup 5'in işidir; bu
  ADR yalnız "tek okuma yolu" ve "şahsi kayıt sızmaz" sınırlarını koyar.
