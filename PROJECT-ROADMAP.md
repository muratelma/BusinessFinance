# Geliştirme Roadmap'i

## Amaç

Bu roadmap, PRD'deki kapsamı uygulanabilir geliştirme aşamalarına böler.
Her aşama ayrı `stages/` belgesinde ayrıntılandırılır. Aşamalar sırayla
ilerler; sonraki aşama teknolojisi erkenden eklenmez.

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
| Veri taşınabilirliği | CSV içe/dışa aktarma, idempotency, yedek şeması v5 (v2–v5 okur) |
| Belge ve fiş | Ek saklama, fiş/dekont fotoğrafından öneri üretme (öneri katmanı, ADR 0011) |
| Tasarım sistemi | Token'lar, pencere sınıfları, ortak bileşenler, erişilebilirlik kapısı |

Bu tabanın **ürün yönü değişti**: kişisel bütçeden şahıs şirketi/esnaf
finansına. Kod tabanı çift taraflı kayıt mantığı üzerine kurulu olduğu için bu
değişiklik altyapıyı değil, kapsamı ve kelimeleri etkiler.

## Aşama zinciri

Yeni ürün yönüyle birlikte aşama zinciri sıfırdan başlar. Devralınan tabanın
geçmiş aşama numaraları bu repoya taşınmadı.

| No | Aşama | Ana çıktı | Durum |
|---:|---|---|---|
| 01 | Henüz açılmadı | Kapsamı kullanıcı belirler | Kapsam onayı bekliyor |

İlk aşamanın kapsamı için tartışılan seçenekler (karar verilmedi):

- **Vergi farkındalıklı kategori** — kategori/hareket başına oran bilgisi ve
  ay sonu özeti. Hesaplamaz, taşır ve raporlar.
- **Müşteri/tedarikçi etiketi** — mevcut borç/alacak modelinin hafif
  genişlemesi; tam cari hesap modülü değil.
- **Kapsam değişikliği yok** — yalnız konumlandırma değişir, ürün aynı kalır.

## Sonraki kilometre taşları

Aşağıdakiler yön göstergesidir; belgeleri sırası geldiğinde o günkü gerçek
duruma göre yazılır. Şu an hiçbiri bağlayıcı değildir.

| Kilometre taşı | İçerik |
|---|---|
| Bulut güvenli beta | Bulut yayını, e-posta doğrulama, izleme, Play kapalı test |
| Offline okunabilir cache | Bağlantısız görüntüleme |
| Tam offline senkronizasyon | Kuyruk, idempotency ve conflict çözümü |
| Read-only açık bankacılık | Sandbox/provider adapter ve mutabakat |
| Yatırım ve çoklu para birimi | Portföy, kur ve fiyat veri modeli |
| Production, iOS ve kalite | Google Play, iOS, güvenlik ve operasyon |

## Aşama mekanizması

- Kapsamı kullanıcı belirler; onay olmadan belge açılmaz.
- Kapsam **açık** da olabilir: aşama tek bir tezle değil, kullanıldıkça çıkan
  işlerin biriktiği bir listeyle yürür ve kullanıcı kapatmak istediğinde
  kapanır. Kalite kapıları aynen geçerlidir.
- Belge `templates/STAGE-TEMPLATE.md` kopyalanarak oluşturulur.
- Açma, aktif etme ve kapatma adımları `stages/README.md` içindedir.
- Aynı anda yalnız bir aşama **Aktif** olur.

## Bağımlılık kuralları

- Online ürün kararlı olmadan offline yazma veya banka senkronizasyonu eklenmez.
- Bulut güvenlik kapısı tamamlanmadan gerçek finansal veri veya dış test
  kullanıcısı eklenmez.
- CSV idempotency kanıtlanmadan canlı banka hareketi eşleştirmesi yapılmaz.
- Manuel yatırım modeli doğrulanmadan fiyat API'si eklenmez.
- Vergiye dair hiçbir alan, PRD'deki ürün sınırı gözden geçirilmeden
  hesaplayan bir alana dönüştürülmez.

## Her aşamanın ortak yapısı

Her aşama belgesi şunları içerir:

- Amaç ve kullanıcıya katkı
- Seçimin nedeni ve trade-off'u
- Uygulanacak çalışma grupları
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
- Çalışma kuralları ve belge güncelleme haritası: `AGENTS.md`
- Güncel durum: `docs/project-status.md`
- Aşama zinciri ve yaşam döngüsü: `stages/README.md`
- Yeni aşama iskeleti: `templates/STAGE-TEMPLATE.md`
