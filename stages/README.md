# Aşamalar

Bu klasör **aktif ve gelecek** aşama belgelerini tutar. Tamamlanan aşamaların
belgeleri `docs/archive/stages/` altına taşınır (klasör ilk aşama kapandığında
oluşur). Bir oturuma başlarken önce `AGENTS.md`, sonra `docs/project-status.md`,
sonra aşağıdaki tablodan **aktif** işaretli belge okunur.

## Zincir

| Aşama | Belge | Durum | Not |
|---|---|---|---|
| 01 | Henüz açılmadı | Kapsam onayı bekliyor | Kapsamı kullanıcı belirler; seçenekler `PROJECT-ROADMAP.md` içinde |

Devralınan kod tabanının geçmiş aşama numaraları (00–12.11) bu repoya
taşınmadı; o kayıt önceki repoda durur. Buradaki zincir 01'den başlar.

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

Numaralandırma tamamlanan son aşamadan devam eder: 01 → 02 → … Bir aşamanın
devamı olan iş alt seviye numara (01.1 gibi) almaz; sıradaki düz numarayı alır
ve bağı "Ön koşul" satırıyla kurar.

Bir aşama **açık kapsamlı** olabilir: tek bir tezi baştan tarif etmek yerine iş
listesi kullanıcı yeni bir şey söyledikçe büyür ve aşama "liste bitti" diye
değil, kullanıcı kapatmak istediğinde kapanır. Bu, aşamanın kalite kapılarını
gevşetmez — her madde yine kendi checkpoint'idir, kendi testini ve belge
güncellemesini getirir.

Aynı anda yalnız bir aşama **Aktif** olur. Kullanıcı açıkça onaylamadan aktif
aşama değiştirilmez.

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
