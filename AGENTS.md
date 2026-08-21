# BusinessFinance — Agent Talimatları

Bu dosya reponun kökü altındaki bütün agent çalışmaları için geçerlidir.

## Proje

Flutter istemcisi ve ASP.NET Core backend'iyle çalışan, çok kullanıcılı bir
işletme finansı uygulaması. Hedef kitle **şahıs şirketleri ve esnaf**: şirketin
kasası ile sahibinin cebi hukuken ayrılmadığı için gündelik gider takibi ile
işletme takibi aynı üründe yaşar.

Ürün kapsamı `PRD-BusinessFinance.md`, geliştirme sırası `PROJECT-ROADMAP.md`
belgesindedir. Backend ve Flutter tarafı aynı şekilde geliştirilir; ikisi
arasında farklı bir çalışma biçimi yoktur.

## Oturum başlangıcı

Kod veya dosya değişikliğinden önce sırasıyla:

1. `docs/project-status.md` — gerçekte nerede kalındığı
2. `stages/README.md` — aşama zinciri; hangi belge **Aktif**
3. O aktif `stages/<numara>-*.md` belgesi
4. `git status --short --branch` ve `git log -3 --oneline`

Ürün kapsamı veya aşama sırası tartışma konusuysa `PRD-BusinessFinance.md`
ve `PROJECT-ROADMAP.md` da okunur. Durum belgesiyle Git çelişirse Git gerçeği
esas alınır. Kullanıcının mevcut değişikliklerini silme, taşıma veya üzerine
yazma.

## Kullanıcıyla iletişim

- Türkçe konuş.
- Ne yapacağını kısaca söyle, sonra yap; adım adım onay bekleme.
- Yeni bir kütüphane, pattern veya dış servis eklerken neden seçildiğini ve
  temel trade-off'unu bir-iki cümleyle belirt.
- Kullanıcı bir konuyu öğrenmek isterse o görevde ayrıntılı anlat; bu istek
  gelmedikçe açıklamayı kısa tut.

## Git sorumluluğu

- Feature çalışması doğrudan `main` üzerinde yapılmaz.
- Kullanıcı istemedikçe yeni branch veya worktree açılmaz.
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
- Migration ve API sözleşme değişikliklerini incelemeden uygulama.

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

Şema tek bir `InitialCreate` ile kurulur; ürünün yayımlanmış bir sürümü
olmadığı için önceki yükseltme zinciri taşınmadı. Bundan sonra eklenen her
migration **gerçek bir yükseltme yolu** sayılır ve aşağıdaki kurallara uyar.
Bunlar geçmişte pahalıya öğrenilmiş kurallardır, tercih değildir:

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
- İlk yerel MVP'yi internete açma.
- Azure, offline sync ve banka entegrasyonunu roadmap aşamasından önce ekleme.
- Open Banking yalnız read-only kapsamındadır; ödeme başlatma ekleme.
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
| Geri alınması pahalı kalıcı teknik karar | Yeni `documentation/adr/NNNN-*.md` |
| Yeni test sınıfı/kapsamı veya kabul senaryosu | `documentation/tests.md` |
| Yeni environment değişkeni, secret veya build flag | `documentation/variables.md` |
| Migration, backup şeması veya restore adımı | `documentation/restore-runbook.md` ve ilgili şema bölümü |
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
- Aşama çıkış koşullarının tümü tamamlanmadan aşamayı bitmiş sayma.
- Kullanıcı açıkça onay vermeden sonraki aşamaya geçme; yeni aşamanın paket,
  servis veya altyapısını erkenden ekleme.

## Oturum kapanışı

Kısaca şunları belirt: ne değişti, hangi build/test/analyze kontrolleri geçti,
sıradaki tek görev ne.
