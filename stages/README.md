# Aşamalar

Bu klasör **aktif ve gelecek** aşama belgelerini tutar. Tamamlanan aşamaların
belgeleri `docs/archive/stages/` altına taşınır (klasör ilk aşama kapandığında
oluşur). Bir oturuma başlarken önce `AGENTS.md`, sonra `docs/project-status.md`,
sonra aşağıdaki tablodan **aktif** işaretli belge okunur.

## Zincir

| Aşama | Belge | Durum | Not |
|---|---|---|---|
| 01 | `docs/archive/stages/01-kapsam-boyutu-ve-isletme-kimligi.md` | Tamamlandı | 23 Ağustos 2026'da kapandı; dokuz grup, cihaz kabul turu |
| 02 | `docs/archive/stages/02-cari-hesap-ve-karsi-taraf.md` | Tamamlandı | 24 Ağustos 2026'da kapandı; sekiz grup, cihaz kabul turu |
| 03 | `docs/archive/stages/03-yukumluluk-ve-vade.md` | Tamamlandı | 24 Ağustos 2026'da kapandı; yedi grup + kapanış denetimi |
| 04 | `docs/archive/stages/04-kasa-pos-ve-gezinme.md` | Tamamlandı | 26 Ağustos 2026'da kapandı; sekiz grup, cihaz kabul turu |
| 05 | `docs/archive/stages/05-vergi-ve-muhasebeci.md` | Tamamlandı | 26 Ağustos 2026'da kapandı; sekiz grup, ADR 0016, cihaz kabul turu |
| 06 | `docs/archive/stages/06-hesap-ve-kalan-isler.md` | Tamamlandı | 28 Ağustos 2026'da kapandı; sekiz grup, ADR 0017. Kabul turu 06.1'e devredildi |
| 06.1 | `docs/archive/stages/06.1-guvenlik-taramasi.md` | Tamamlandı | 1 Eylül 2026'da kapandı; beş grup, dört CI tarama kapısı, Pixel 8 kabul turu |
| 06.2 | `06.2-arayuz-duzeni.md` | Beklemede | **Açık kapsamlı.** 2 Eylül 2026'da açıldı; 29 Eylül 2026'dan beri beklemede (kapanmadı), kalan işleri 06.3'ten sonra sürer |
| 06.3 | `06.3-butunsel-duzenleme.md` | Aktif | 29 Eylül 2026'da açıldı: kasa, gün sonu, POS ve vergi yeniden kurulur; kesin hatalar; listeler ve filtreler. Karar kapıları ADR 0018, ADR 0019 ve ADR 0020 (taraf kuralı; 8 Ekim 2026) |
| 06.x | — | Açılmadı | Uygulama büyüdükçe çıkan işler için; ihtiyaç oldukça açılır |
| 07 | `07-bulut-guvenli-beta.md` | Planlandı | Gerçek finansal veriye geçiş kapısı. **Bütün 06.x kapanmadan açılmaz**; ADR 0011 fiş veri sınırıyla açılır |

**29 Eylül 2026 itibarıyla Aşama 06.3 Aktif, 06.2 Beklemede** (kullanıcı
kararı: 28 Eylül'deki bütünsel düzenleme kararları 06.3 altında uygulanır, 06.2
sonra sürer). Aşağıdaki paragraf 06.2'nin geçmişidir.

**2 Eylül 2026 itibarıyla Aşama 06.2 Aktif.** İlk checkpoint yalnız yerel web
deneme temelidir; geniş arayüz gruplarının kapsamını rakip arayüz araştırması
beslemeye devam eder. 27 Eylül 2026 itibarıyla araştırmanın Belge 1 ve Belge 2'si
kapandı, Belge 3'e başlanmadı; arada cloud üzerinde Flutter arayüz denemeleri
checkpoint'i açıldı (`06.2-arayuz-duzeni.md`).

Zincirin tamamı ve her aşamanın gerekçesi `PROJECT-ROADMAP.md` içindedir.
Kurucu ürün kararı `documentation/adr/0013-business-and-personal-are-one-pool.md`
belgesindedir: işletme ve şahsi tek havuzda bir boyuttur.

Devralınan kod tabanının geçmiş aşama numaraları bu repoya taşınmadı; o kayıt
önceki repoda durur. Buradaki zincir 01'den başlar.

`Planlandı` durumundaki bir belge yalnız kapsamı kayda geçirir; kullanıcı
açıkça onaylayana kadar **Aktif** olmaz ve kodu değiştirilmez.

## Yeni bir aşama açmak

1. Kapsamı kullanıcı belirler; kapsam onaylanmadan belge açılmaz.
2. `templates/STAGE-TEMPLATE.md` kopyalanır: `stages/<numara>-<kisa-slug>.md`.
3. Belgenin **Belge durumu** bölümü doldurulur: durum, ön koşul (bir önceki
   aşama), sonraki aşama, dokunacağı `documentation/` belgeleri.
4. Bu dosyadaki tabloya satır eklenir; bir önceki aşamanın "Sonraki aşama"
   satırı yeni belgeye bağlanır.
5. `PROJECT-ROADMAP.md` aşama listesine satır eklenir ve durumu işaretlenir.
6. `docs/project-status.md` içindeki "Aktif konum" bölümü yeni aşamayı gösterir.

Numaralandırma tamamlanan son aşamadan devam eder: 01 → 02 → …

**Bir aşama kümesi alt numara alabilir** (06, 06.1, 06.2, …). Bu, "aynı hedefe
giden ama ayrı ayrı kapanabilen işler" içindir: her alt aşama kendi belgesi,
kendi çalışma grupları, kendi testleri ve kendi çıkış koşullarıyla tam bir
aşamadır — düz numaralı bir aşamadan hiçbir farkı yoktur, yalnız aynı kümeye
ait olduğunu adıyla söyler.

Küme, iş biriktiği için vardır: uygulamaya bir şey eklendikçe arkasında yeni iş
kalıyor. Tek bir dev aşama, listesi sürekli büyüdüğü için hiç kapanmaz; ayrı
numaralanan aşamalar kapanır ve yeni iş yeni bir numarayla (06.3, 06.4, …)
açılır. Küme bittiğinde sıradaki düz numaraya geçilir.

Alt numara **her durumda** kullanılmaz: bağımsız bir hedefi olan iş düz numara
alır. Bağı "Ön koşul" satırıyla kurulur.

Bir aşama **açık kapsamlı** olabilir: tek bir tezi baştan tarif etmek yerine iş
listesi kullanıcı yeni bir şey söyledikçe büyür ve aşama "liste bitti" diye
değil, kullanıcı kapatmak istediğinde kapanır. Bu, aşamanın kalite kapılarını
gevşetmez — her madde yine kendi checkpoint'idir, kendi testini ve belge
güncellemesini getirir.

Aynı anda yalnız bir aşama **Aktif** olur. Kullanıcı açıkça onaylamadan aktif
aşama değiştirilmez.

Bir aşama **Beklemede** olabilir: açılmıştır, kapanmamıştır, ama üzerinde
çalışılmaz; kodu yalnız yeniden Aktif olduğunda değişir. Başka bir aşamayı
öne almak için kullanılır ve yalnız kullanıcı kararıyla verilir.

## Bir aşamayı kapatmak

1. Belgenin "Çıkış koşulları" bölümündeki maddelerin hepsi karşılanır.
2. `docs/project-status.md` içine doğrulanmış tamamlanma kaydı yazılır.
3. `PROJECT-ROADMAP.md` durumu `Tamamlandı` olur.
4. Aşama belgesi `docs/archive/stages/` altına taşınır ve bu tablodaki satırı
   arşivi gösterir.
5. Sonraki aşama kullanıcı onayıyla **Aktif** yapılır.

## İlgili belgeler

- Çalışma kuralları ve belge güncelleme haritası: `AGENTS.md`
- Ürün kapsamı: `PRD-BusinessFinance.md`
- Aşama sırası ve kilometre taşları: `PROJECT-ROADMAP.md`
- Doğrulanmış ilerleme: `docs/project-status.md`
- Aşamaya bağlanmamış açık işler: `docs/backlog.md`
- Kalıcı teknik belgeler: `documentation/`
