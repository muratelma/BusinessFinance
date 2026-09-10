# Nihai Belgeler

Araştırmanın çıktısı **birbirine bağlı üç belgedir**. Üçü de önce bu klasörde
düzenlenebilir taslak (Markdown) olarak yazılır; patron onayından sonra Word
(`.docx`) ve PDF (`.pdf`) olarak dışa aktarılır. Word ve PDF farklı içerik
değil, aynı belgenin iki biçimidir.

## Belgeler

| No | Dosya (taslak) | İçerik | Girdi |
|---|---|---|---|
| 1 | `1-rakip-arayuz-yaklasimlari.md` | Ürün kimliği/asıl amaç; bilgi hiyerarşisi, gezinme, renk, tipografi, kart/liste/grafik, form desenleri; güçlü ve zayıf yaklaşımlar | `gozlemler/*` "Ürün kimliği ve asıl amaç" + "Arayüz incelemesi" + "Arayüz taraması" |
| 2 | `2-rakip-finansal-akislar.md` | Ana ekran, gelir/gider kaydı, işletme/şahsi ayrımı, transfer, kart borcu/ödemesi, planlama, fatura→tahsilat; **arka plan olay modeli, pipeline aşamaları, akışların birbirine bağlanışı**, entegrasyon temas noktaları ve kullanıcı sürtünmeleri | `gozlemler/*` "Görev gözlemleri" + "Sistem işleyişi / pipeline" + "Video/doküman akış yeniden kurulumu" + "Akış özeti" |
| 3 | `3-businessfinance-arayuz-ve-akis-onerisi.md` | 1 ve 2'deki bulguların `doğrudan al` / `uyarlayarak al` / `alma` / `henüz karar verme` kararına dönüştürülmesi; etkilenen ekran, akış, olay modeli ve ortak bileşenler | Belge 1 + Belge 2 (onaylı) |

## Kapsam

Belge 1 ve 2 rakibin **tüm özellik yüzeyini** anlatır — BusinessFinance'te
kapsam dışı sayılanlar (stok/depo, banka bağlama, KDV hesaplama, e-belge)
dahil. Rapor patrona gidiyor ve kapsamı patron belirler. Karar filtresi
(`doğrudan al` / `uyarlayarak al` / `alma` / `henüz karar verme`) yalnız
Belge 3'te uygulanır; kurucu ADR'yle çakışan özellik atılmaz, gerekçesiyle
yazılır.

## Sıra

Belge 3, Belge 1 ve 2 taslakları onaylanmadan yazılmaz — yoksa öneriler
kanıtsız kalır.

## Durum

| Belge | Taslak | Patron onayı | Word/PDF |
|---|---|---|---|
| 1 — Arayüz yaklaşımları | Başlanmadı | — | — |
| 2 — Finansal akışlar | Başlanmadı | — | — |
| 3 — BusinessFinance önerisi | Başlanmadı | — | — |

## Karar filtresi (Belge 3)

Her bulgu `README.md` "BusinessFinance karar filtresi" bölümündeki sabit
kararlarla karşılaştırılır. Rakipte bulunması bir özelliğin alınacağı anlamına
gelmez.
