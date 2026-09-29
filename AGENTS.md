# BusinessFinance — Agent Talimatları

Bu dosya reponun kökü altındaki bütün agent çalışmaları için geçerlidir.

## Proje

Flutter istemcisi ve ASP.NET Core backend'iyle çalışan, çok kullanıcılı bir
işletme finansı uygulaması. Hedef kitle **şahıs şirketleri ve esnaf**: şirketin
kasası ile sahibinin cebi hukuken ayrılmadığı için gündelik gider takibi ile
işletme takibi aynı üründe yaşar.

Ürünün kurucu kararı: **işletme ve şahsi para tek havuzda yaşar**, ayrım bir
raporlama boyutudur (`documentation/adr/0013-business-and-personal-are-one-pool.md`).
Finansal kod yazmadan önce bu ADR okunur.

Ürün bir **işletme bütçe uygulamasıdır, ön muhasebe değildir.** Ön muhasebe
ürünleri fikir için incelenir ama bir özelliği sunmaları bize gerektiğinin
kanıtı sayılmaz; "gerekli mi" sorusu bizim kullanıcımız, mevzuat ve bütçe
uygulaması sınırıyla cevaplanır.

Ürün kapsamı `PRD-BusinessFinance.md`, geliştirme sırası `PROJECT-ROADMAP.md`
belgesindedir; hepsinin belgesi yazılıdır (tamamlananlar `docs/archive/stages/`,
açık olanlar `stages/` altında). Aşama 06 bir **kümedir** (06, 06.1, 06.2, …):
ayrı ayrı kapanabilen aşamalar; kural `stages/README.md` içinde.
Backend ve Flutter tarafı aynı şekilde geliştirilir; ikisi arasında farklı bir
çalışma biçimi yoktur.

## Oturum başlangıcı

Kod veya dosya değişikliğinden önce sırasıyla:

1. `docs/project-status.md` — gerçekte nerede kalındığı
2. `stages/README.md` — aşama zinciri; hangi belge **Aktif**
3. O aktif `stages/<numara>-*.md` belgesi
4. `git status --short --branch` ve `git log -3 --oneline`

Ürün kapsamı veya aşama sırası tartışma konusuysa `PRD-BusinessFinance.md`
ve `PROJECT-ROADMAP.md` da okunur. Kullanıcının mevcut değişikliklerini silme,
taşıma veya üzerine yazma.

**Aktif aşama yoksa kod değişmez.** Zincirdeki bütün belgeler yazılı olabilir;
belgenin var olması onu başlatma izni değildir. Aktif aşama yokken yapılacak
iş, kullanıcıyla hangi aşamanın açılacağını konuşmaktır.

## Çelişen bilgi: iki ayrı eksen

Çelişki görülürse önce sorunun hangi eksende olduğu belirlenir. Bunlar tek bir
listede sıralanamaz; sıralamaya çalışmak kategori hatasıdır.

**"Kod ne durumda, ne oldu?"** — gözlem sorusu:

1. **Git gerçeği** ve çalışan kodun kendisi — hiçbir belge bunu ezemez
2. `docs/project-status.md`
3. Diğer belgeler

**"Ne yapılmalı, kural ne?"** — karar sorusu:

1. `PRD-BusinessFinance.md` (ürünün ne olacağı)
2. `documentation/adr/` (geri alınması pahalı kalıcı kararlar)
3. `AGENTS.md` (çalışma kuralları)
4. `PROJECT-ROADMAP.md` (sıra ve bağımlılık)
5. Aktif `stages/<numara>-*.md` (o aşamanın ayrıntısı)

`research/` altındaki kapanış listeleri (`kasa-pos-gun-sonu/KAPANIS.md`,
`vergi/YENI-YAKLASIM.md` §6.6) kararların gerekçesi ve ayrıntısıdır; bir ADR veya
aşama belgesiyle çeliştiklerinde ADR ve aşama belgesi geçerlidir ve çelişki
kullanıcıya sorulur. Neyin bağlayıcı, neyin başlangıç tasarımı olduğu
"Kararlardan sapma" bölümündedir.

Bir belge çalışan koda aykırı bir şey **anlatıyorsa** belge yanlıştır ve
düzeltilir. Bir belge çalışan koddan farklı bir şey **istiyorsa** kod eksiktir
ve aşama belgesine iş olarak girer. İkisini ayırt etmek çağıranın işidir.

## Kararlardan sapma

Kararlar yazıldıkları gün doğru görünür; bir kısmı kod yazılırken yanlış ya da
eksik çıkar. Bu yüzden karar belgeleri **bağlayıcı olanı** ve **başlangıç
tasarımını** ayırır:

- **Bağlayıcı:** ADR'lerin **"İlkeler"** bölümü (ADR 0018'den itibaren ADR'ler iki
  katmanlıdır; 0001–0017 tek katmanlıdır ve onlarda **"Karar"** bölümü
  bağlayıcıdır), PRD ve bu dosyadaki ürün ve mimari kurallar.
- **Başlangıç tasarımı:** ADR'lerin **"Başlangıç tasarımı"** bölümü, aşama
  belgelerinin çalışma gruplarındaki ayrıntılar ve `research/` kapanış
  listelerinin ayrıntıları (varsayılanlar, alan listeleri, uç ve tablo biçimi,
  ekran akışının ayrıntısı).

Kod yazılırken daha iyi ya da gerekli bir yol bulunursa agent başlangıç
tasarımından **sapabilir**:

1. Hiçbir bağlayıcı ilkeyi ve kuralı bozmaz.
2. Sapma, neyin yazılı olduğu, neyin yapıldığı ve gerekçesiyle aktif aşama
   belgesinin **"Sapmalar"** tablosuna yazılır.
3. Checkpoint sonunda kullanıcıya söylenir.
4. **Kullanıcının gördüğü davranışı değiştiren** sapma (ekranda ne sorulduğu,
   hangi sayının nasıl hesaplanıp gösterildiği, bir akışın adımları)
   uygulanmadan önce kullanıcıya kısa bir soruyla sorulur. İç tasarım (şema,
   uç ve sınıf yapısı, sorgu biçimi) bu soruyu beklemez.

Sapma sayılmayanlar — bunlar her zaman önce sorulur: veri kaybettiren ya da geri
döndürülemeyen şema kararı, var olan API sözleşmesini kıran değişiklik, bir
bağlayıcı ilkenin değişmesi.

Bir ilke ya da kullanıcı kararı kodda **somut bir zarar** doğuruyorsa (çifte
sayım, veri kaybı, çözülemeyen bir çelişki) agent onu körü körüne uygulamaz:
durur, sorunu ve seçenekleri kullanıcıya getirir. İlke yalnız kullanıcı kararı
ve ADR güncellemesiyle değişir.

## Kullanıcıyla iletişim

- Türkçe konuş.
- Ne yapacağını kısaca söyle, sonra yap; adım adım onay bekleme.
- Yeni bir kütüphane, pattern veya dış servis eklerken neden seçildiğini ve
  temel trade-off'unu bir-iki cümleyle belirt.
- Kullanıcı bir konuyu öğrenmek isterse o görevde ayrıntılı anlat; bu istek
  gelmedikçe açıklamayı kısa tut.

## Git sorumluluğu

- **Çalışma, kullanıcının o an bulunduğu branch üzerinde yapılır.** Branch veya
  worktree oluşturma, branch değiştirme ve silme yalnız kullanıcı açıkça
  istediğinde yapılır. `main` üzerinde çalışmak yasak değildir.
- Commit mesajı, branch adı, tag, PR ve issue metinlerinde yapay zekâ imzası,
  `Co-Authored-By:` satırı veya "Generated with …" ibaresi yer almaz; commit
  yazarı daima kullanıcıdır.
- Commit'ler derlenebilir ve ilgili testleri geçen checkpoint'lerdir; kaç
  commit atılacağı işin şekline bırakılır.
- **Commit mesajları İngilizce yazılır.** Belgeler ve kullanıcıyla iletişim
  Türkçedir; Git geçmişi değildir.
- **Yalnız uygulamada gerçek bir geliştirme veya düzeltme olduğunda commit
  atılır.** Tek başına belge commit'i açılmaz: bir belge neden güncelleniyorsa,
  onu gerektiren kod değişikliğiyle **aynı** commit'e girer. Belgeyi
  güncelleyecek bir kod değişikliği yoksa commit de yoktur.
- **İstisna: karar belgeleri.** Şunlar kendi başlarına commit edilebilir:
  `PRD-BusinessFinance.md`, `PROJECT-ROADMAP.md`, `stages/` belgeleri,
  `documentation/adr/` kayıtları, `AGENTS.md` ve `CLAUDE.md`. Bunlar kodun
  kaydı değil, kodun **kararıdır** — ne yapılacağını ve nasıl çalışılacağını
  söylerler; kararın koddan önce yazılması kuralın ihlali değil, gereğidir.
  `documentation/` altındaki diğer belgeler (mimari, akış, izin, test,
  değişken, tasarım sistemi, runbook) bu istisnaya **girmez**: onlar mevcut
  davranışı anlatır ve davranışla aynı commit'e girer.
- **Commit tipi yapılan işi dürüstçe anlatır.** Özellik eklenmediyse `feat`
  yazılmaz: taşıma, yeniden adlandırma, altyapı ve araç işleri `chore`,
  davranış değiştirmeyen yeniden düzenleme `refactor`, hata düzeltmesi `fix`.
- Commit mesajı aşama numarasını değil, değişen gerçek ürün davranışını
  anlatır.
- Kullanıcı değişikliklerini silme veya geri alma.

## Mimari kurallar

- Katmanlı monolit kullan; mikroservis ekleme.
- Domain katmanı EF Core, HTTP, Flutter veya dış sağlayıcı bağımlılığı taşımaz.
- Application katmanı kullanım senaryolarını yürütür; endpoint içinde finansal
  iş kuralı bırakma.
- Genel repository veya MediatR/CQRS gibi soyutlamaları somut ihtiyaç olmadan
  ekleme.
- Para değerlerinde backend'de `decimal` kullan; API sözleşmesinde hassasiyet
  kaybını önle.
- Kullanıcı kimliğini request body veya query string'den alma; güvenli
  oturumdan türet.
- Bakiyeyi ikinci gerçek kaynak olarak elle saklama.
- Transfer, kart ödemesi ve taksitleri normal gider gibi modelleyerek raporları
  bozma.
- **İşletme/şahsi ayrımı tek havuz üzerinde bir boyuttur** (ADR 0013). Havuzu
  ikiye bölme, mod seçimi ekleme, kapsamı kategoriyle temsil etme. Bakiye,
  kart borcu ve net varlık kapsam filtresinden etkilenmez.
- **KDV alanları, indirilebilirlik ve muhasebeci paketi kaldırılıyor** (ADR 0018,
  Aşama 06.3 Grup 2). Kaldırılana kadar yeni kodu bunlara bağlama, onları
  genişletme.
- **Aynı satışı iki kez gelir yazma.** Gün sonu var olan kayıtları üretir ve o
  gün zaten girilmiş kayıtları hesaba katar; kartla tahsilat ve POS yatışı gelir
  yazmaz (ADR 0019, ADR 0014).
- Migration ve API sözleşme değişikliklerini incelemeden uygulama.

### Ürün sınırı — arayüz metnini de bağlar

- **Uygulama vergi hesaplamaz ve beyanname üretmez.** Vergi bir nakit planıdır
  (ADR 0018): vergi tutarı kullanıcıdan gelir; bir vergi tutarını türeten ya da
  tahmin eden kod yazma.
- **Ön muhasebe özelliği ekleme:** muhasebeciye veri paketi, KDV takibi, fatura
  kesme, satış satış kayıt ve adisyon kapsam dışıdır.
- **"Kâr" kelimesi kullanılmaz.** Hesaplanan şey nakit esaslı **işletme
  netidir**; muhasebe kârı satılan malın maliyetini ister ve kapsam dışıdır.
  Yanlış kelime kullanıcıyı vergi beyanında yanıltır.
- Kullanıcıya gösterilecek cümleyi API değil istemci üretir; API kararlı
  makine değerleri gönderir.

## Flutter kuralları

- Flutter stable ve Dart kullan, Material 3 tasarım sistemiyle.
- View, ViewModel/Controller, Repository ve Service sorumluluklarını ayır.
- API DTO'larını açık Dart modelleriyle temsil et; ham `Map`/JSON geçirme.
- Finansal kurallara yalnız istemci doğrulamasıyla güvenme.
- Loading, empty, error, unauthorized ve stale-cache durumlarını görünür ele al.
- Token ve hassas oturum bilgisini güvenli mobil depoda tut; connection string
  veya sunucu secret'ını uygulamaya koyma.
- Kapsamlı state yönetimi veya kod üretimi paketlerini gerçek ihtiyaçtan önce
  ekleme.
- Erişilebilirlik, metin ölçeklendirme ve klavye/ekran okuyucu kullanımını
  doğrula.

## Migration kuralları

Şema `InitialCreate` ile kuruldu ve devralınan yükseltme zinciri taşınmadı
(ADR 0012). Zincirin güncel hâli tek yerde, `src/BusinessFinance.Infrastructure`
migration klasöründedir; bu dosya sayı tutmaz.

**Bundan sonra eklenen her migration gerçek bir yükseltme yoludur** ve aşağıdaki
kurallara uyar. Bunlar geçmişte pahalıya öğrenilmiş kurallardır, tercih
değildir:

- **Backfill her zaman CHECK kısıtından önce çalışır.** SQL Server yeni bir
  CHECK'i mevcut satırlara karşı doğrular; sırayı ters kurmak dolu bir
  veritabanında yükseltmeyi patlatır.
- **Kolonlar kısıtlardan önce eklenir.** Aynı gerekçe.
- **Geçmişi bilinmeyen bilgi için kolon nullable olur.** Bu ayrımdan önce
  oluşmuş satırların değeri bilinmiyorsa `NOT NULL` + uydurma varsayılan
  yazmak, olmamış bir geçmiş uydurur ve türetilen bütün tutarları bozar.
- **Backfill için verilen `defaultValue` kalıcı bir veritabanı varsayılanı
  bırakır.** Model bunu istemiyorsa migration sonunda o DEFAULT kısıtı
  düşürülür; yoksa şema ile model sessizce ayrışır.
- İkinci bir migration eklendiğinde `MigrationHistoryTests` bilerek kırılır;
  o an tekliği doğrulayan test yerini zincir testine bırakır.
- **Tek istisna — gerçekten boş tablo.** Tablo boşsa `NOT NULL` kolon
  backfill'siz eklenebilir; yorumlanacak geçmiş yoktur. İstisnanın ön koşulu,
  tablonun boş olduğunun **aşama belgesinde yazılı** ve o aşamada kasıtlı
  olmasıdır. Dolu tabloda bu yol kullanılmaz.

## Veri ve güvenlik

- **Uygulamanın henüz gerçek kullanıcısı ve gerçek verisi yok**; yerel
  veritabanındaki her kayıt sentetiktir ve gerektiğinde sıfırlanabilir.
  Ancak ürün ticari olarak sunulabilir: **veri kaybettiren veya geri
  döndürülemeyen bir şema kararı artık varsayılan olarak kabul edilmez.**
  Böyle bir karar gerekiyorsa gerekçesiyle birlikte kullanıcıya sorulur ve
  `docs/project-status.md` içine yazılır. Bu pencere gerçek veri geldiğinde
  tamamen kapanır.
- Davranış kuralları hiçbir koşulda gevşemez: silme yerine iptal, çifte sayım
  yasağı, sahiplik izolasyonu ve para hassasiyeti ürün kurallarıdır; veri
  değerli olduğu için değil doğru olduğu için vardır.
- Geliştirme ve testte yalnız sentetik veri kullan.
- Gerçek finansal veri, güvenlik ve geri yükleme kapısı tamamlanmadan
  kullanılmaz.
- `.env`, parola, token, signing key ve connection string Git'e eklenmez.
- Secret ve finansal veriyi terminal çıktısında veya loglarda gösterme.
- Ürünü Aşama 07 tamamlanmadan internete açma; API ve SQL loopback'e bağlı kalır.
  Giden bir HTTPS çağrısı (ör. e-posta gönderimi) bu kuralı ihlal etmez;
  yasak olan dışarıdan gelen bağlantıyı kabul etmektir.
- Azure ve offline cache'i kendi roadmap aşaması gelmeden ekleme.
- **Banka bağlantısı / açık bankacılık kapsam dışıdır.** Sağlayıcı SDK'sı,
  adapter veya sandbox bağlantısı eklenmez; bankadan ödeme veya transfer
  başlatma hiçbir koşulda eklenmez. Gerekçesi `PROJECT-ROADMAP.md`
  "Kapsam dışı bırakılanlar" tablosunda.
- Kullanıcı verisi izolasyonunu pozitif ve negatif integration testlerle kanıtla.

## Belge sorumluluğu

- Repo içindeki Markdown belgelerini agent oluşturur ve günceller.
- Henüz bulunmayan özellik için sahte "uygulandı" belgesi oluşturulmaz; belge
  davranışla **aynı** commit'te güncellenir, sonraya bırakılmaz.
- Aşama başına ayrı öğrenme notu tutulmaz.

### Belge güncelleme haritası

Ne değiştiyse hangi belgeye dokunulacağı:

| Değişiklik | Güncellenecek belge |
|---|---|
| Yeni/değişen endpoint, request veya response sözleşmesi | `documentation/permissions.md` (sahiplik sınırı), varsa ilgili API sözleşme belgesi |
| Kullanıcının gördüğü yeni akış (ekran, form, aksiyon zinciri) | `documentation/flows.md` |
| Katman, aggregate, okuma modeli veya veri akışı değişikliği | `documentation/architecture.md` |
| Geri alınması pahalı kalıcı teknik karar | Yeni `documentation/adr/NNNN-*.md` — iki katmanlı: **İlkeler** (bağlayıcı) ve **Başlangıç tasarımı** (değişebilir); sık değişecek ayrıntı ADR'ye değil aşama belgesine yazılır |
| Başlangıç tasarımından sapma | Aktif `stages/<numara>-*.md` "Sapmalar" tablosu ("Kararlardan sapma") |
| Yeni test sınıfı/kapsamı veya kabul senaryosu | `documentation/tests.md` |
| Yeni environment değişkeni, secret veya build flag | `documentation/variables.md` |
| Yeni ekran, bileşen, token veya gezinme değişikliği | `documentation/design-system.md` |
| Migration, backup şeması veya restore adımı | `documentation/restore-runbook.md` (yedek sürümünün **tek** kaynağı) |
| Doğrulanmış checkpoint (build+test geçti, commit atıldı) | `docs/project-status.md` |
| Aşama kapsamının kendisi değiştiyse | Aktif `stages/<numara>-*.md` |
| Aşama başladı/bitti, yeni aşama açıldı | `stages/README.md` + `PROJECT-ROADMAP.md` |
| Ürünün ne olacağı değiştiyse | `PRD-BusinessFinance.md` (yalnız kullanıcı kararıyla) |
| Çalışma kuralı değiştiyse | `AGENTS.md` (+ gerekiyorsa `CLAUDE.md`) |

Aşama yaşam döngüsünün adımları (yeni aşama açma, aktif aşamayı değiştirme,
biten aşamayı arşive taşıma) `stages/README.md` belgesinde tanımlıdır ve orada
yazdığı gibi uygulanır.

## Kalite ve aşama geçişi

- Backend değişikliğinde build, test ve format kontrollerini çalıştır.
- Flutter değişikliğinde analyze, ilgili test ve build kontrollerini çalıştır.
- Migration, secret ve API sözleşmesi değişikliklerini ayrıca incele.
- **Aşama belgesinin var olması onu başlatma izni değildir.** Zincirdeki bütün
  belgeler yazılı; kod yalnız durumu `Aktif` olan aşamada değişir.
- **Bazı aşamalar bir ADR ile açılır** (`PROJECT-ROADMAP.md` içindeki tablo).
  O ADR yazılıp kabul edilmeden ilgili aşamanın koduna başlanmaz.
- Aşama çıkış koşullarının tümü tamamlanmadan aşamayı bitmiş sayma.
- Kullanıcı açıkça onay vermeden sonraki aşamaya geçme; yeni aşamanın paket,
  servis veya altyapısını erkenden ekleme.

## Oturum kapanışı

Kısaca şunları belirt: ne değişti, hangi build/test/analyze kontrolleri geçti,
sıradaki tek görev ne.
