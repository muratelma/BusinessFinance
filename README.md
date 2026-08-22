# BusinessFinance

Bu repository; ASP.NET Core, EF Core, SQL Server ve Flutter ile geliştirilen
**şahıs şirketi ve esnaf için işletme finansı uygulamasını** ve testlerini
içerir. Uygulama hesap, kategori, gelir/gider, kredi kartı ve taksit, tekrarlayan
plan, borç/alacak, hedef, birleşik finansal hareket akışı, CSV içe/dışa aktarma,
yedekleme ve fiş okuma akışlarını Pixel 8 Android emulatoründe uçtan uca
çalıştırır.

İşletme ve şahsi harcama tek havuzda yaşar; ayrım bir raporlama boyutudur.
Şahıs şirketinin tüzel kişiliği olmadığı için işletmenin kasası ile sahibinin
cebi aynı ceptir.

Ürün vergi hesaplamaz, beyanname üretmez ve muhasebe kârı hesaplamaz;
muhasebeciye giden veriyi hazırlar. Gerekçesi `PRD-BusinessFinance.md`
içindeki "Ürün sınırı" bölümündedir.

## Hızlı başlangıç

Windows üzerinde SQL, API, emulator ve temiz debug APK kurulumu için
[`documentation/local-setup-and-acceptance.md`](documentation/local-setup-and-acceptance.md)
rehberini izleyin. Yalnız sentetik veri kullanın; mevcut APK production release
artefact'i değildir.

```bash
docker compose up -d                                   # SQL Server (127.0.0.1:14334)
dotnet build BusinessFinance.slnx -c Release
dotnet test  BusinessFinance.slnx -c Release
cd mobile/business_finance_mobile && flutter test
```

## Kaynakların öncelik sırası

Birbiriyle çelişen bilgi görülürse aşağıdaki sıra uygulanır:

1. `docs/project-status.md`: Gerçekte nerede kalındığı
2. Aktif `stages/` belgesi: O aşamanın ayrıntılı uygulama planı
3. `AGENTS.md`: Kalıcı çalışma kuralları
4. Kapsam tartışmalıysa `PRD-BusinessFinance.md` ve `PROJECT-ROADMAP.md`
5. Git gerçeği: Branch, commit ve çalışma alanının gerçek durumu

Git ile durum belgesi çelişirse Git gerçeği esas alınır. Ürün kapsamı yalnız
PRD güncellenerek değiştirilir.

## Ana belgeler

- `PRD-BusinessFinance.md`: Ürün kapsamı, mimari ve araçlar
- `documentation/adr/0013-business-and-personal-are-one-pool.md`: Zincirin
  kurucu kararı — işletme ve şahsi tek havuzda bir boyuttur
- `AGENTS.md`: Repo genelinde geçerli kalıcı agent talimatları
- `PROJECT-ROADMAP.md`: Aşamaların sırası, bağımlılıkları ve kilometre taşları
- `stages/README.md`: Aşama zinciri, aktif aşama ve aşama açma/kapatma adımları
- `stages/`: Aktif ve gelecek aşamaların işleri, testleri ve çıkış koşulları
- `templates/STAGE-TEMPLATE.md`: Yeni aşama belgesi iskeleti
- `templates/PROJECT-STATUS-TEMPLATE.md`: İlerleme belgesi başlangıç şablonu
- `docs/project-status.md`: Doğrulanmış ilerleme kaydı
- `docs/backlog.md`: Aşamaya bağlanmamış açık işler
- `documentation/`: Mimari, akış, izin, değişken, test ve yerel kabul belgeleri
- `documentation/adr/`: Geri alınması pahalı kalıcı teknik kararlar
- `src/`: .NET katmanlı monolit ve test projeleri
- `mobile/business_finance_mobile/`: Flutter Android istemcisi

## Başlangıç ilkesi

Bir oturuma başlarken `AGENTS.md`, `docs/project-status.md` ve aktif `stages/`
belgesi okunur. Aktif aşamanın çıkış koşulları tamamlanmadan sonraki aşamanın
teknolojisi kurulmaz veya production kodu yazılmaz.

## Kapsam ve çalışma yönteminin ayrımı

- PRD, ürünün ne olacağını anlatır.
- Roadmap, ürünün hangi sırayla geliştirileceğini anlatır.
- Aşama belgeleri, o sıradaki küçük hedefleri anlatır.
- `AGENTS.md`, kodun hangi kurallarla üretileceğini anlatır.
- Durum belgesi, planlananı değil gerçekten tamamlananı kaydeder.

Bu ayrım, uzun yol haritasının tek oturumda uygulanmasını ve geleceğin
altyapısının erkenden eklenmesini önler.
