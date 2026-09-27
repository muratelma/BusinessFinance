# Ham Kanıt Dosyaları

Her bulgunun arkasındaki ekran görüntüsü burada tutulur. Bir gözlem formundaki
satır, kanıt dosyasının adını yazmak zorundadır; adı yazılmayan bulgu
`Doğrulanamadı` sayılır.

## Neyin Git'e girdiği

**Ekran görüntüleri (`.png`) Git'e dâhildir** ve commit edilir — raporların
kanıtı onlar, sentetik veriyle üretildiler, gerçek finansal bilgi taşımıyorlar.

Git dışında bırakılan yalnız **büyük medya**dır (`.gitignore`): `.mp4`, `.m4a`,
`.wav`, `.mov` — ekran kaydı ve sesli not. Bunlar yereldedir.

*(12 Eyl 2026 düzeltmesi: bu dosya daha önce "medya dosyaları `.gitignore` ile
Git dışında bırakılmıştır" diyordu; PNG'ler için bu **yanlıştı**.)*

## Uygulama klasörleri

| Klasör | Uygulama | İnceleme türü |
|---|---|---|
| `money-manager/` | Money Manager (Realbyte) | Manuel gözlem |
| `wallet-budgetbakers/` | Wallet by BudgetBakers | Manuel gözlem |
| `bluecoins/` | Bluecoins | Manuel gözlem |
| `hesap-defterim/` | Hesap Defterim (Cash Book) | Manuel gözlem |
| `goodbudget/` | Goodbudget | Manuel gözlem |
| `parasut/` | Paraşüt | Manuel gözlem (2 giriş öncesi kare) + resmî kaynak (7 tanıtım videosu karesi) |
| `logo-isbasi/` | Logo İşbaşı | Manuel gözlem (4 giriş/kayıt karesi) + resmî kaynak (2 tanıtım videosu karesi) |
| `kolaybi/` | KolayBi | Manuel gözlem (1 giriş karesi) + resmî kaynak (7 video karesi, 31 destek mockup'ı) |
| `quickbooks/` | QuickBooks mobil onboarding + QBO Simple Start plan ekranı (form konusu Solopreneur; Solopreneur ekranı yok) | Manuel gözlem (4 kare); Solopreneur bulguları resmî kaynak |

**Klasör adı gözlem formunun adıyla aynıdır** (`gozlemler/<ad>.md` ↔
`kanitlar/<ad>/`). Aynı uygulama için ikinci bir klasör açılmaz — Wallet'ın
Faz 7 kareleri bir süre ayrı bir `wallet/` klasöründe durdu, 12 Eyl 2026'da
`wallet-budgetbakers/` içine taşındı.

## Dosya adlandırma

Ön ek hangi koşumdan geldiğini söyler; numara koşum içindeki sırayı verir.

| Ön ek | Ne zaman | Örnek |
|---|---|---|
| `00`–`09` | Tur 1 sabit kontrol noktaları (aşağıdaki liste) | `04-islem-formu.png` |
| `10`+ | Aynı uygulamanın ek koşumu (A/B/B1/B2, boşluk koşumu) | `17-taksit-plani.png` |
| `f7-` | Tur 2 derin koşumu (Faz 7) | `f7-12-ofis-kirasi-realized.png` |
| `d` | Resmî **destek dokümanı** mockup'ı | `d05-destek-yeni-gider-formu.png` |
| `NN-video-` | Tanıtım/eğitim **videosundan** kare (KolayBi'de `d` serisinin sonunda: `d33`–`d38`) | `d34-video-gunu-gelen-islemler.png` |

Tur 1 sabit kontrol noktaları:

- `00-magaza.png` — uygulama adı, geliştirici, sürüm
- `01-ilk-acilis.png` — onboarding veya kayıt yaklaşımı
- `02-bos-ana-ekran.png` — veri girilmeden ana ekran
- `03-dolu-ana-ekran.png` — ortak veriler girildikten sonra
- `04-islem-formu.png` — gelir/gider formunun en açıklayıcı hâli
- `05-siniflandirma.png` — kategori, kapsam veya en yakın sınıflandırma
- `06-islem-listesi.png` — liste ve bilgi hiyerarşisi
- `07-rapor.png` — aylık rapor / nakit akışı
- `08-hata-veya-bos-durum.png` — varsa açıklayıcı hata/boş durum
- `09-ozgun-ozellik.png` — uygulamayı ayıran tek güçlü örnek

## Kural

**Kare dolu olmalı; dönem seçilebiliyorsa Ağustos 2026.** Boş ekran kanıt
değildir; verinin çoğunluğu Ağustos 2026'ya girildiği için ay gezgini/filtre/
rapor mümkünse o aya alınır. **Üç istisna var** (dönemsiz ekranlar, verinin
başka aya düştüğü durum, varsayılan dönemin kendisinin bulgu olduğu durum) ve
hesap kavramı olan üründe her hesap türünün **kendi defteri** ayrı bir karedir.
Ayrıntı: `../MANUEL-TEST-PROTOKOLU.md` → "Kanıt karesi dolu olmalı ve dönemi
Ağustos 2026".

Ham görüntü kırpılmaz; cihaz ve saat bağlamı korunur. Gerçek bilgi yanlışlıkla
görünürse paylaşmadan önce bulanıklaştırılır. Test verisi sentetiktir
(`../SENTETIK-TEST-VERISI.md`) ve **kullanıcı istemeden silinmez**.
