# Nihai Belgeler

## Güncel üretim durumu — 23 Eylül 2026

**Belge 2 tamamlandı ve kapandı:** on bölüm, kanıt eki ve birleşik PDF
([belge2/tam/belge2.pdf](belge2/tam/belge2.pdf), 105 sayfa, 129 kare). Başlangıç noktası
[belge2/README.md](belge2/README.md). Son kontrol ve kararlar
[belge2-son-kontrol.md](belge2-son-kontrol.md) dosyasında. Belge 3'ün soru girdisi Belge 2'nin
kendisidir: 10.6 ve her bölümün "Belge 3'e taşınan soru" kutusu.

**Sıradaki: Belge 1 düzeltme turu**, ayrı oturumda. Devir ve açılış istemi
[devir-2026-09-23-sonraki-oturumlar.md](devir-2026-09-23-sonraki-oturumlar.md).

### Belge 1 — 17 Eylül durumu

**Belge 1 tamamlandı:** 13 bölüm, kanıt eki ve birleşik PDF
([belge1/tam/belge1.pdf](belge1/tam/belge1.pdf), 98 sayfa). Sıradaki adım patron onayı; ardından Belge 2.

- Devir notu ve plandan sapmalar: **[belge1/butun-harita.md](belge1/butun-harita.md) §6**

- Başlangıç noktası: **[belge1/README.md](belge1/README.md)**
- Tek başvuru kaynağı: **[belge1-bolum-plani.md](belge1-bolum-plani.md)** — 13 bölüm, alt sorular,
  ürün–kanıt eşlemesi, bölüm sınırları, biçim kuralları
- Üretim altyapısı: `belge1/ortak/` (motor, duman testi, 387 karelik kanıt dizini)

Belge 1 ve 2'nin bölüm düzeni **plandaki hâliyle geçerlidir**;
[0-ortak-icindekiler.md](0-ortak-icindekiler.md) pilot turunun taslağıdır ve Belge 1 için
plan tarafından güncellenmiştir. Belge 3 ilk iki raporun onayını bekler.

### Belge 2 — plan aşaması (20 Eylül, tarihsel)

**Kurgu seçildi:** gövde **A · Olay defteri**, ardından **C · Ürünlerin motorları**. Seçim
[belge2-nasil-olabilir/](belge2-nasil-olabilir/README.md) (10 sayfa, üç kurgu karşılaştırması)
üzerinden yapıldı; kurgu belgesindeki "kanıt haritası" sayfası (A2) Belge 2'ye **girmiyor**.

**Yapı kararı (20 Eylül):** belge **konu konu bütün hâlinde** ilerler —
*giriş → A (karşılaştırma) → C (işleyiş) → gerekirse ek → kapanış → sonraki konu*. Toplu A veya
toplu ürün anlatımı yoktur; ayrı senaryo bölümü de yoktur.

**Tek başvuru kaynağı: [belge2-bolum-plani.md](belge2-bolum-plani.md) — v2.** 12 bölüm, konu bloğu
anatomisi, bir satır + altı etki, ürün–kanıt eşlemesi, bölüm sınırları, dokuz anlatım bileşeni,
iddiaya bağlı koşum listesi ve §8'de bir kapanan (A1), beş açık karar (A2–A6). v2, dış inceleme turundan
([belge2-claude-code-plan-review.md](belge2-claude-code-plan-review.md)) sonra yazıldı; 24
değişikliğin listesi planın §11'indedir. Plan onaylanmadan bölüm yazımına başlanmaz.

**20 Eylül koşum ve devir kapanışı:** K1/K2/K3 ile S1 tamamlandı. 22 yeni kare ve sekiz eski
karenin E kimlikleri dizine girdi (387 kare / 422 kimlik). Kapanan gözlem soruları ve açık
nedensellik sınırları [koşum listesinde](belge2-kosum-listesi.md) ve
[bulgu kaydının](../BULGU-DOGRULAMA-KAYDI.md) sonundadır. Konu içinde A → C sırası korunur.

### Pilot turu (tarihsel)

Dört pilot üretildi ve karşılaştırıldı: [işlem ekleme](pilot/), [borç ve tahsilat](pilot-borc-tahsilat/),
[Belge 1 ana ekran](pilot-belge1-ana-ekran/) ve üç ayrı Bölüm 3 biçim denemesi (`belge1/` altında).
Seçilen yön: **ekran argümandır, tablo ektir.** Gerekçe ve karşılaştırma `belge1/README.md`'dedir.

## Tarihsel devir ve kapsam kayıtları

Aşağıdaki eski durum ifadeleri tarihseldir; güncel üretim durumu yukarıdadır.

**14 Eylül son durum: P1-K kanıtla kapandı (B17).** Denetim kapısı test edilebilir denetim.cjs'e taşındı
(15/15 test; hata varsa çıkış 1, olmayan form 2); P1'in yedi formu 22 aralık düzeltmesiyle 0 hata.
B01–B19: 11 kanıtla kapandı, 4 kapsamı sınırlandı, 4 açık. P1 paketleri tamam.
Görsel sayacı 164/360. Sıradaki tek paket P2-G01 (Bluecoins E0016–E0032); bu tur başlanmadı.
Aşağıdaki eski paket devirleri tarihseldir; Faz 8 henüz açılmadı.


14 Eylül kullanıcı kararı: mevcut koşumlar korunur; gerekli ek emülatör
kontrollerini agentın uygulama başına hazırladığı listeyle kullanıcı yapar.
Agent otomatik canlı test başlatmaz. Güncel sınır ve kapanış kuralları
[mevcut plandadır](../FAZ7-8-UYGULAMA-PLANI.md).
Goodbudget GB-U01 kullanıcı ekran kontrolüyle kapandı; P1-kolaybi-G01/G02, P1-B08, P1-B15, P1-hesap-defterim-G01/G02, P1-B11, P1-B12, P1-money-manager-G01, P1-parasut-G01, P1-logo-isbasi-G01, P1-quickbooks-G01, P1-B13, P1-B18 ve P1-K de kapandı, sonraki paket P2-G01.


Araştırmanın çıktısı **birbirine bağlı üç belgedir**. Üçü de önce bu klasörde
düzenlenebilir taslak (Markdown) olarak yazılır; patron onayından sonra Word
(`.docx`) ve PDF (`.pdf`) olarak dışa aktarılır. Word ve PDF farklı içerik
değil, aynı belgenin iki biçimidir.

## Belgeler

**14 Eylül 2026:** [Faz 7/7.5 kapanış ve Faz 8 üretim planı](../FAZ7-8-UYGULAMA-PLANI.md)
kaydedildi; P0.1/P0.2, Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı, QuickBooks, KolayBi, Bluecoins ve Wallet metin haritaları tamam; P0 kapandı; P1-B01, P1-B02-B04, P1-B14 ve P1-goodbudget-G01 kapandı;
sırada P1-goodbudget-T01 var.
Üç taslağa da henüz başlanmadı. Önce
kanıt/bulgu kayıtları, hata kapanışı ve Bluecoins/Wallet tam Faz 7.5 incelemeleri;
sonra geçiş onayı, Belge 1/2 pilot bölümleri ve tam taslaklar gelir.
Belge 3, ilk iki belgenin onayından sonra yazılır. Güncel ilerleme
[DURUM.md](../DURUM.md)'dedir. Bu klasördeki denetimler nihai üç rapor değildir.

| No | Dosya (taslak) | İçerik | Girdi |
|---|---|---|---|
| 1 | `1-rakip-arayuz-yaklasimlari.md` | Ürün kimliği/asıl amaç; bilgi hiyerarşisi, gezinme, renk, tipografi, kart/liste/grafik, form desenleri; güçlü ve zayıf yaklaşımlar | `gozlemler/*` "Ürün kimliği ve asıl amaç" + "Arayüz incelemesi" + "Arayüz taraması" |
| 2 | `2-rakip-finansal-akislar.md` | Ana ekran, gelir/gider kaydı, işletme/şahsi ayrımı, transfer, kart borcu/ödemesi, planlama, fatura→tahsilat; **arka plan olay modeli, pipeline aşamaları, akışların birbirine bağlanışı**, entegrasyon temas noktaları ve kullanıcı sürtünmeleri | `gozlemler/*` "Görev gözlemleri" + "Sistem işleyişi / pipeline" + "Video/doküman akış yeniden kurulumu" + "Akış özeti" |
| 3 | `3-businessfinance-arayuz-ve-akis-onerisi.md` | 1 ve 2'deki bulguların karara dönüştürülmesi (beş sonuç: `README.md` → "Karar sonuçları — tek kaynak"); etkilenen ekran, akış, olay modeli ve ortak bileşenler; ADR'ye meydan okuyan bulgular için ayrı **"Açık sorular"** başlığı | Belge 1 + Belge 2 (onaylı) |

## Kapsam

Belge 1 ve 2 rakibin **tüm özellik yüzeyini** anlatır — BusinessFinance'te
kapsam dışı sayılanlar (stok/depo, banka bağlama, KDV hesaplama, e-belge)
dahil. Rapor patrona gidiyor ve kapsamı patron belirler. Karar filtresi
(`doğrudan al` / `uyarlayarak al` / `alma` / `henüz karar verme` /
**`kararı yeniden sor`**) yalnız Belge 3'te uygulanır; kurucu ADR'yle çakışan
özellik atılmaz, gerekçesiyle yazılır.

**Belge 1 ve 2 rakibi anlatır, puanlamaz.** Kendi kararlarımız bu iki belgede
ölçüt olarak kullanılmaz; her yaklaşım "ne kazandırıyor / ne kaybettiriyor"
ile tarif edilir. Ayrıntı: `README.md` → "Bu liste ölçüt değil,
karşılaştırmanın bir tarafıdır".

## Sıra

Belge 3, Belge 1 ve 2 taslakları onaylanmadan yazılmaz — yoksa öneriler
kanıtsız kalır.

## Durum

| Belge | Plan | Taslak | Patron onayı | Word/PDF |
|---|---|---|---|---|
| 1 — Arayüz yaklaşımları | **Hazır** ([plan](belge1-bolum-plani.md)) | **Tamam** ([PDF](belge1/tam/belge1.pdf), 98 sayfa); düzeltme turu sırada | Bekliyor | PDF üretildi |
| 2 — Finansal akışlar | v2 plan → v3 üretim ([README](belge2/README.md)) | **Tamam** ([PDF](belge2/tam/belge2.pdf), 105 sayfa) | Bekliyor | PDF üretildi |
| 3 — BusinessFinance önerisi | — | Başlanmadı | — | — |

Belge 1 doğrudan PDF olarak üretilir (PyMuPDF); Word turu yoktur. Okunabilir Markdown kopyası
aynı içerik kaynağından birlikte çıkar.

## Karar filtresi (Belge 3)

**Tanımlar `../README.md`'dedir, buraya kopyalanmaz:** "BusinessFinance karar
filtresi", "Karar sonuçları — tek kaynak" ve "Gözlem formlarında dil kuralı".

Özet: rakipte bulunması bir özelliğin alınacağı anlamına gelmez; kararlarımızla
çakışması da otomatik olarak `alma` demek değildir — rakibin yaklaşımı bir
boyutta daha iyiyse sonuç **`kararı yeniden sor`** olur ve ilgili ADR
ödünleşimiyle birlikte patrona taşınır.
