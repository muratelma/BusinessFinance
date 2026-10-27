# Brif 08 — Hesap ve kart seçici (envanter)

27 Ekim 2026. Uygulamada hesap ya da kart seçilen bütün yerlerin listesi ve
ortak tasarımın karşılaması gereken hâller. Tasarım Claude Design'da yapılır;
kategori özetindeki `Hesap / kart` menüsü (`AppFilterMenu`) çıkış noktasıdır.

## Bugünkü sorun

Kare: `hesapseçme.png` (`Gelir ekle` → `Hesap`). Seçim Material'ın düz açılır
listesiyle yapılıyor:

- Satırda yalnız ad var: nakit mi banka mı, işletme mi şahsi mi, içinde ne
  kadar para var, belli değil (`Dukkan Kasası`, `ziraat sahsi`, `Ziraat
  vadesiz` yan yana; hangisinin kasa olduğu addan tahmin ediliyor).
- Liste formun üstüne açılıyor, alanın kendisini ve başlığını örtüyor.
- Seçili satır yalnız gri zeminle ayrılıyor; ikon, işaret ya da grup yok.
- Gider formunda hesaplar ile kredi kartları aynı listede, yalnız küçük bir
  ara başlıkla ayrılıyor; kartın kullanılabilir limiti satırın sonuna
  `• ₺… kullanılabilir` diye sıkıştırılıyor ve kısalıyor.
- Her ekran kendi açılır listesini kuruyor (aşağıda 20 form alanı, 3
  süzgeç ve 2 POS seçici); görünüş ve
  davranış ekrandan ekrana değişiyor.

## Envanter

Kod yolları `mobile/business_finance_mobile/lib/features/` altındadır.

### A. Yalnız hesap (nakit ve banka birlikte)

| # | Ekran | Alan etiketi | Dosya |
|---|---|---|---|
| A1 | Gelir ekle | `Hesap` | `activities/presentation/quick_add_form_page.dart` |
| A2 | Yeni transfer (çıkan / giren) | `Nereden` / `Nereye` | `finance_pages/presentation/transfer_page.dart` |
| A3 | Kart borcu ödeme | `Ödeme hesabı` | `cards/presentation/finance_page.dart` |
| A4 | Tekrarlayan plan (gelir) | `Hesap` | `planning/presentation/planning_page.dart` |
| A5 | Fatura / yükümlülük kapatma | `Hesap` | `obligations/presentation/obligations_page.dart` |
| A6 | Cari tahsilat / ödeme | `Paranın gireceği hesap` / `Ödeme hesabı` | `counterparties/presentation/counterparty_forms.dart` |
| A7 | Borç taksidi ödeme / tahsilat (dialog) | `Ödeme hesabı` / `Tahsilat hesabı` | `debts/presentation/debts_page.dart` |
| A8 | Nakit borç açılışı | `Paranın girdiği hesap` / `Paranın çıktığı hesap` | `debts/presentation/debts_page.dart` |
| A9 | Hedef | `Bağlı hesap` | `goals/presentation/goals_page.dart` |
| A10 | Banka ekstresi satır eşleme (dialog) | `Hesap` | `data_tools/presentation/data_tools_page.dart` |

### B. Hesap ya da kredi kartı (ödeme kaynağı)

| # | Ekran | Alan etiketi | Not |
|---|---|---|---|
| B1 | Gider ekle (fişten de) | `Ödeme kaynağı` | Fiş "kredi kartı" yazıyorsa kartlar üstte; alanın altında fişin söylediği yazar. Boş başlar, zorunlu |
| B2 | Tekrarlayan plan (gider) | `Kaynak` | `planning_page.dart` |
| B3 | Vergi "Ödedim" / toplu ödeme | `Nereden ödendi` | `taxes/presentation/tax_sheets.dart`, `TaxSourceField` |
| B4 | Vergi planı | `Nereden ödenir (isteğe bağlı)` | `Seçilmedi` seçeneği var |

### C. Yalnız kredi kartı

| # | Ekran | Alan etiketi | Not |
|---|---|---|---|
| C1 | Kart harcaması / taksitli harcama | `Kart` | `cards/presentation/finance_page.dart`; kart sayfasından açılınca alan çizilmez |

### D. Hesap türü sınırlı

| # | Ekran | Alan etiketi | Liste |
|---|---|---|---|
| D1 | POS ekle / düzenle | `Paranın geçeceği hesap` | Yalnız banka (`pos/presentation/pos_definitions_page.dart`) |
| D2 | POS tahsilatı | `Paranın geçeceği hesap` | Yalnız banka (`pos/presentation/pos_settlements_view.dart`) |
| D3 | Kartla tahsil (cari) | `Paranın geçeceği hesap` | Yalnız banka (`pos/presentation/card_collection_fields.dart`) |
| D4 | Gün sonu | `Kasa` | Yalnız nakit (`day_close/presentation/day_close_sheets.dart`) |
| D5 | Kasa farkı → `Kendime aldım` | `Şahsi hesap` | Şahsi etiketli hesaplar (`cash/presentation/cash_withdrawal_sheet.dart`) |

### E. Süzgeç (form değil)

| # | Ekran | Bugünkü biçim |
|---|---|---|
| E1 | Kategori özeti / kategori sayfası `Hesap / kart` | `AppFilterMenu`: gruplu, ikonlu, tutarlı. **Çıkış noktası** |
| E2 | Hesap sayfası `Tür` | `AppFilterMenu` |
| E3 | Eski İşlemler süzgeci (`transactions_page.dart`) | Dialog içinde iki düz açılır liste (`Hesap`, `Kategori`), `Tümü` seçeneğiyle |

### F. Aynı aileden: POS seçici (karar bekliyor)

| # | Ekran | Alan etiketi | Not |
|---|---|---|---|
| F1 | POS tahsilatı | `POS` | `pos/presentation/pos_settlements_view.dart`; ana POS seçili gelir, seçim hesabı ve kategoriyi doldurur |
| F2 | Kartla tahsil (cari) | `POS` | `pos/presentation/card_collection_fields.dart` |

POS hesap değildir ama seçimi parayı bir hesaba bağlar ve aynı düz listeyle
yapılıyor. Öneri: aynı panel, satırda POS adı ve altında geçeceği hesap
(`Ziraat vadesiz`). Sonucu: POS seçimi de tek bileşene girer; satır ikonu
`point_of_sale`.

### Gösterilen ama seçilmeyen hesaplar (bu brifin dışında)

POS yatışı ve tahsilat ayrıntısındaki `Hesap` / `Geçeceği hesap` satırları
(`pos_deposit_sheets.dart`, `card_collection_fields.dart`,
`pos_settlements_view.dart`) yalnız bilgi gösterir; yatışın hesabı seçilen
tahsilatlardan gelir, ayrı bir seçici yoktur.

### Arama yöntemi

`DropdownButtonFormField` / `DropdownMenu` / `DropdownButton` kullanan bütün
alanlar, etiketinde `hesap`, `kart`, `kasa`, `kaynak`, `nereden` geçen bütün
form alanları ve açılır liste dışındaki seçiciler (`ChoiceChip`,
`RadioListTile`, `AppSegmentRail`, `AppFilterMenu`, dialog içi seçimler)
tarandı (27 Ekim 2026). Hesap ya da kart seçen başka yer bulunmadı.

### Kapsam dışı (bilerek)

- **Kasa sekmesindeki kasa seçici** (`cash/presentation/cash_page.dart`,
  `_CashPicker`): bir seçim alanı değil, ekranın hangi kasayı gösterdiğini
  değiştiren yan yana raydır; adın altında bakiye zaten yazıyor.
- Hesap türü, kategori, POS, ritim gibi hesap/kart olmayan açılır listeler
  (aynı Material listesi, ama bu brifin konusu değil; ortak tasarım
  oturunca kategori seçici ayrıca ele alınabilir).

## Ortak tasarımın karşılaması gerekenler

Tasarım tek bileşen olmalı; aşağıdaki farklar parametredir, ayrı ekran değil.

1. **Kapalı alan.** Seçilmemişken etiket ve `Seçin`; seçilince ikon + ad, altında
   tür/kapsam (`Nakit · İşletme`). Hata hâli (`Ödeme kaynağı seçin.`) ve alanın
   altındaki yardım metni (B1'deki fiş notu) yer almalı.
2. **Açılan liste.** Formu örtmemeli (alttan açılan panel öneri; menü
   `AppFilterMenu` dilinde de olabilir). Başlıkta ne seçildiği yazar
   (`Ödeme kaynağı`, `Paranın geçeceği hesap`).
3. **Gruplar:** `Nakit`, `Banka`, `Kredi kartı` — yalnız listede bulunan
   gruplar çizilir. B1'de fişe göre kartlar üstte olabilir.
4. **Satır:** tür ikonu (nakit / banka / kart; ikonlar `AppFinanceIcons`),
   ad, alt satırda kapsam etiketi (`İşletme` / `Şahsi`, yoksa boş), sağda
   **hesapta bakiye, kartta kullanılabilir limit**. Seçili satırda işaret.
5. **Boş ve tek seçenek:** hiç hesap yoksa `Henüz hesap yok` + `Hesap ekle`;
   türü sınırlı listede (D1–D3) "banka hesabınız yok" durumu ayrı yazılır.
6. **İsteğe bağlı alan:** B4 ve E3'te en üstte `Seçilmedi` / `Tümü` satırı.
7. **Erişilebilirlik:** 2× yazıda ad kısalır, tutar kısalmaz; satır ekran
   okuyucuda tek düğüm (`Ziraat vadesiz, banka, İşletme, ₺30.658,35`).
8. **Pasif hesap ve kart listelenmez** (bugünkü gibi); düzenlemede seçili
   olan pasifse alan boş başlar.

## Açık sorular (seçenek + öneri)

| Soru | Seçenekler | Öneri | Sonucu |
|---|---|---|---|
| Satırda bakiye yazsın mı? | (a) yazsın (b) yazmasın | **(a)** — parayı nereden çıkaracağını seçerken en çok bakılan bilgi | Bazı ekranların seçenek okumasına bakiye eklenmesi gerekir (sunucu); gider formunda kart limiti zaten geliyor |
| Açılış biçimi | (a) alttan panel (b) alanın altında menü | **(a)** | Uzun listede kayar, formu örtmez; filtre menüsüyle görünüş ortak, açılış farklı |
| Dialog içindeki seçimler (A7, A10) | (a) aynı paneli dialogdan aç (b) dialogu panelle değiştir | **(b)** A7 için | `Seç` tek dokunuşla kabul edilmesin kuralı (A7 yorumu) panelde onay düğmesiyle korunur |

## Özet sayfasındaki `Hesap bakiyeleri` bölümü

Bugün (`dashboard/presentation/dashboard_sections.dart`,
`DashboardAccountBalances`): tek kart içinde her hesap bir satır — kapsül
ikon, ad, alt satırda `Nakit` / `Banka hesabı`, sağda bakiye; satıra ya da
başlığa dokununca `Hesaplar` sayfası açılır. Kapsam seçiliyken başlığın
altında "bölünmez" notu yazar (ADR 0013: bakiye kapsamla bölünmez). Toplam,
kapsam etiketi, grup ve pasif hesap yok; kredi kartı borcu bu bölümde değil,
`Varlık durumu` kartında.

Yeni tasarım için elde olan veri: hesap adı, türü ve bakiyesi
(`report.accountBalances`); `Hesaplar` sayfasındaki toplam, nakit/banka/pasif
payları ve kapsam etiketi ayrı bir uçtan (`/api/v1/accounts/overview`) gelir.
Bölümü `Hesaplar` sayfasının (`hesaplar yeni.png`) dilinde kurmak bu ikinci
okumayı Özet'e de taşımayı ister.
