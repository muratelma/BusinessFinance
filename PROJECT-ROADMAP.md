# Geliştirme Roadmap'i

## Amaç

Bu roadmap, ürün kapsamını uygulanabilir geliştirme aşamalarına böler. Her
aşama ayrı `stages/` belgesinde ayrıntılandırılır. Aşamalar sırayla ilerler;
sonraki aşama teknolojisi erkenden eklenmez.

## Devralınan taban

Bu repo, çalışan ve testlerle korunan bir kod tabanıyla başladı. Aşağıdakiler
**şu an mevcuttur**, yeniden yapılacak iş değildir:

| Alan | Durum |
|---|---|
| Kimlik ve oturum | JWT access + refresh rotation, reuse tespiti, rate limit |
| Kullanıcı izolasyonu | Owner-scoped composite anahtar; Application ve SQL seviyesinde iki kapı |
| Hesaplar ve hareketler | Nakit/banka hesabı, hareketlerden hesaplanan bakiye, iptal modeli |
| Kategoriler ve bütçeler | Kategori, aylık bütçe, ilerleme |
| Kredi kartı | Limit, ekstre projeksiyonu, harcama, ödeme, asgari ödeme, taksit planı |
| Tekrarlayan planlar | Hesap veya kart kaynaklı plan, idempotent occurrence üretimi, gerçekleştirme |
| Borç ve alacak | Anüite faiz modeli, açılış kaynağı, anapara/faiz ayrımı |
| Hedefler | Manuel ve bakiye izleyen tasarruf hedefleri |
| Birleşik okuma modelleri | Tek SQL sorgusunda gerçekleşmiş feed + planlanan görünüm |
| Veri taşınabilirliği | CSV içe/dışa aktarma, idempotency, yedekleme ve geri yükleme |
| Belge ve fiş | Ek saklama, fiş/dekont fotoğrafından öneri üretme (öneri katmanı, ADR 0011) |
| Tasarım sistemi | Token'lar, pencere sınıfları, ortak bileşenler, erişilebilirlik kapısı |

Bu taban **kişisel bütçe** ürünü olarak kuruldu. Çift taraflı kayıt mantığı,
sahiplik izolasyonu ve para hassasiyeti yeni ürün yönünde de aynen geçerli;
değişen kapsam ve kelimeler.

## Zincirin kurucu kararı

Bütün zincir tek bir ürün kararının üstünde duruyor: **işletme ve şahsi, tek
havuzda bir boyuttur** (`documentation/adr/0013-business-and-personal-are-one-pool.md`).
Şahıs şirketinin tüzel kişiliği olmadığı için işletmenin kasası ile sahibinin
cebi aynı cep; ayrım bir raporlama boyutudur, ayrı bir veri alanı veya ayrı bir
mod değildir.

Bu karar sıralamayı da belirliyor: kapsam boyutu en altta, her şey onun üstüne
oturuyor.

## Aşama zinciri

| No | Aşama | Ana çıktı | Durum |
|---:|---|---|---|
| 01 | Kapsam boyutu ve işletme kimliği | Her kayıt işletmeye mi şahsa mı ait olduğunu bilir; işletme kategori seti | Tamamlandı (23 Ağu 2026) |
| 02 | Cari hesap: karşı taraf ve açık bakiye | Müşteri/tedarikçi başına yürüyen bakiye | Tamamlandı (24 Ağu 2026) |
| 03 | Yükümlülük ve vade | Ödenmemiş fatura kendi kabına kavuşur; plan bitiş sınırı | **Aktif** |
| 04 | Kasa, POS ve gezinme | Gün sonu kasa, POS tahsilatı ve bloke; ana sekmeler | Planlandı |
| 05 | Vergi ve muhasebeci | KDV taşıyan alanlar, vergi takvimi, ay sonu paketi | Planlandı |
| 06 | Bulut güvenli beta | Ürün kendi makinenden bağımsız çalışır | Planlandı |

Altı aşamanın belgesi de `stages/` altında yazılı. `Planlandı` durumundaki bir
belge yalnız kapsamı kayda geçirir; kullanıcı açıkça onaylayana kadar **Aktif**
olmaz ve kodu değiştirilmez. Belgeler, sıraları geldiğinde o günkü gerçek
duruma göre gözden geçirilir.

**Üç aşama bir ADR ile açılır**; kararı yazılmadan işe başlanmaz:

| Aşama | Yazılacak karar |
|---|---|
| 02 | Ekonomik olay tanır, ödeme taşır — kart, borç ve cari modellerinin ortak kuralı |
| 04 | Kart borcu ile kart tahsilatının ayrılması; bloke paranın projection olması; sekme kararı |
| 05 | Vergi alanları taşır, hesaplamaz; oran ve tarihler koda gömülmez |

### 01 — Kapsam boyutu ve işletme kimliği

Uygulama ev bütçesi uygulaması olmaktan çıkar. Kapsam alanı, hesap/kart/kategori
varsayılanları, işletme kategori seti, onboarding ön ayarı, kapsama duyarlı
raporlar. Veri sıfırlanır, yedek şeması v6 olur. Bakiye ve net varlık
bölünmez.

### 02 — Cari hesap: karşı taraf ve açık bakiye

`Counterparty` kendi boyutu olur — kategori değil. Açık hesap: n belge, m kısmi
tahsilat, yürüyen bakiye; taksit planı zorunluluğu yok. Mevcut sözleşmeli borç
modeli yerinde kalır ve aynı karşı tarafa bağlanır. Fiş okumanın düz metin
karşı taraf adı gerçek kayda bağlanır.

### 03 — Yükümlülük ve vade

Tek seferlik yükümlülük kaydı: ödenmemiş fatura artık tekrarlayan plan olmaya
zorlanmaz. Tekrarlayan plana bitiş sınırı eklenir. Vade, gecikme ve yaklaşanlar
görünümü; fiş okumanın "henüz ödemedim" yolu buraya bağlanır.

### 04 — Kasa, POS ve gezinme

Gün sonu nakit sayım ve fark. POS tahsilatı: tahsilat → bloke → hesaba geçiş,
komisyon ayrı gider. Ana sekme yapısı ve `İşlem ekle` menüsü bu aşamada
yeniden kurgulanır — kasanın oturacağı yer burasıdır ve menü burada taşar.

### 05 — Vergi ve muhasebeci

Hareket başına KDV oranı ve tutarı (taşınır, hesaplanmaz). İndirilebilirlik
**ayrı** bayrak olarak eklenir; kapsamla birleştirilmez. Vergi/SGK takvimi.
Ay sonu muhasebeci paketi. Tasarruf hedefi "vergi karşılığı" olarak
konumlanır; yeni modül yazılmaz.

### 06 — Bulut güvenli beta

Container yayını, yönetilen veritabanı, HTTPS, secret yönetimi, e-posta
doğrulama, parola sıfırlama, izleme, otomatik yedek, Google Play kapalı test.

## Yedek şeması sürümleri

Aşama 01'den 05'e her aşama yedek şemasını bir sürüm ilerletir (v6 → v10) ve
**yalnız kendi sürümünü okur.** Bu bilinçli: her aşama yedeğin taşıması gereken
yeni bir alan ekliyor ve eski yedekte o alan **yok**; bir değer uydurmak,
olmamış bir geçmiş uydurmak olurdu (aynı gerekçe ADR 0013 ve ADR 0012'de).

Bunun bedeli, geliştirme sırasında bir aşamada alınan yedeğin bir sonrakinde
geri yüklenememesidir. Gerçek kullanıcı verisi olmadığı sürece kabul edilebilir.
**Aşama 06'dan sonra bu serbestlik kapanır**: gerçek veri geldiğinde yedek
uyumluluğu bir yükseltme yolu olmak zorundadır.

## Sonraki kilometre taşları

Yön göstergesidir; şu an hiçbiri bağlayıcı değildir.

| Kilometre taşı | İçerik |
|---|---|
| Offline okunabilir cache | Bağlantısız görüntüleme, cache yaşı ve bağlantı durumu |
| Yatırım ve çoklu para birimi | Portföy, kur ve fiyat veri modeli |
| Production, iOS ve kalite | Google Play production, iOS, güvenlik ve operasyon |

## Kapsam dışı bırakılanlar

Devralınan plandan **çıkarılan** işler ve gerekçeleri:

| Çıkarılan | Gerekçe |
|---|---|
| Read-only açık bankacılık | Sağlayıcı, yetkilendirme ve sözleşme süreçleri ürünün kontrolü dışında; CSV içe aktarma aynı ihtiyacın çalışan karşılığı |
| Tam offline senkronizasyon | Kuyruk, idempotency ve conflict çözümü, online ürün oturmadan ödenecek bir maliyet. **04 buluta çıktığında yeniden değerlendirilir**: dükkânda internet kesikken satış kaydedilemiyorsa uygulama o an işe yaramaz |
| Personel ve bordro | Tek başına aşama büyüklüğünde; SGK tarafı vergi sınırına değiyor |
| Stok ve satılan malın maliyeti | Muhasebe kârı hesaplamak demek; ürün sınırının dışında |
| Muhasebe ve beyanname | Ürün vergi hesaplamaz, taşır ve raporlar |

## Bağımlılık kuralları

- Kapsam boyutu (01) oturmadan cari, fatura veya vergi alanı eklenmez; hepsi
  kapsamın üstüne oturur.
- Karşı taraf (02) gerçek bir kayıt olmadan fatura karşı tarafa bağlanmaz.
- Bulut güvenlik kapısı tamamlanmadan gerçek finansal veri veya dış test
  kullanıcısı eklenmez.
- Vergiye dair hiçbir alan hesaplayan bir alana dönüştürülmez; taşır ve
  raporlar.
- İndirilebilirlik bayrağı kapsam alanıyla birleştirilmez (ADR 0013).
- Manuel yatırım modeli doğrulanmadan fiyat API'si eklenmez.

## Her aşamanın ortak yapısı

Her aşama belgesi şunları içerir:

- Amaç ve kullanıcıya katkı
- Değiştirilmeyecek mimari kararlar
- Çalışma grupları
- Zorunlu test ve doğrulamalar
- Belge ve güvenlik güncellemeleri
- Açıkça kapsam dışında kalan işler
- Çıkış koşulları

## Ortak kalite kapısı

- İlgili backend build, test ve format kontrolleri başarılıdır.
- İlgili Flutter analyze/test/format kontrolleri başarılıdır.
- Kullanıcı izolasyonu ve finansal kurallar için negatif senaryolar bulunur.
- Migration eklendiyse `AGENTS.md` içindeki migration kurallarına uyar.
- Secret veya gerçek finansal veri Git'e girmez.
- `docs/project-status.md` ve ilgili `documentation/` belgeleri günceldir.
- Kullanıcı sonraki aşamayı açıkça onaylar.

## Kaynak belgeler

- Ürün ve kapsam: `PRD-BusinessFinance.md`
- Kurucu kapsam kararı: `documentation/adr/0013-business-and-personal-are-one-pool.md`
- Çalışma kuralları ve belge güncelleme haritası: `AGENTS.md`
- Güncel durum: `docs/project-status.md`
- Aşama zinciri ve yaşam döngüsü: `stages/README.md`
- Yeni aşama iskeleti: `templates/STAGE-TEMPLATE.md`
