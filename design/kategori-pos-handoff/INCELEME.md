# Kategori seçici + POS tahsilatı · teslim incelemesi (28 Ekim 2026)

Teslim: bu klasör (masaüstündeki `design_handoff_kategori_pos`'tan kopyalandı).
Uygulamaya bu oturumda geçilmedi; kod değişmedi.

`01`–`04` POS kareleri bilerek alınmadı (kullanıcı, 28 Ekim); POS paneli
README ve canvas'tan okunur.

## Bugünkü koddan farklar (uygulanacak)

| # | Teslim | Bugün |
|---|---|---|
| F1 | Panel açıklaması tek satır: *Gelir bugün yazılır, para hesaba sonra geçer.* | İki satırlık eski metin |
| F2 | Elle gir sırası: POS · Hesap · **Satış kategorisi** · Müşterinin ödediği · … | Kategori tutarın altında |
| F3 | Komisyon: tam genişlik, **48** (alt düğmelerle aynı), seçili dolgu `brand`, ✓ yok, 14 (seçili 600) | Material segment, ✓ var, ~40, dar |
| F4 | Değer ile sağdaki ok arası 6 px | 12 px |
| F5 | Kategori alanı `SelVal` (simge kapsülü, ad, kapsam alt yazısı), 56 | Düz açılır liste, 57,2 |
| F6 | Kategori simgeleri (kanonik addan; tutmazsa `local_offer`) | Uygulamada kategori simgesi yok; eşleme tablosu yazılacak |

## Kararlar (kullanıcı, 28 Ekim 2026)

| # | Konu | Karar |
|---|---|---|
| D1 | Menünün açılışı | **Teslimdeki gibi, yalnız kategoride**: form kayar, alan başlığın altına gelir, alttaki alanlar ve Kaydet gizlenir, menü formun içinde açılır. Hesap seçici yüzen menü olarak kalır |
| D2 | Son kullanılanlar | **Sunucudan**: son kayıtlardan okunur, her cihazda aynı |
| D3 | Son kullanılanlarda kapsamsız kategori | **Kapsamıyla birlikte saklanır**: `Faiz geliri · İşletme` ve `Faiz geliri · Şahsi` ayrı satır; dokununca kategori ve kapsam birden seçilir |
| D4 | `+ "kırtasiye" ekle` | Bana bırakıldı → kategori formu adı ve türü dolu açılır, kaydedince forma dönülür ve seçili gelir (`Hesap ekle` deseni). Yeni kategori kapsamsızsa menüdeki gibi kapsam sorulur |
| D5 | Hesabın kapsamı ön değer | Bana bırakıldı → **ön değer yok**; kapsamsız kategoride kapsam her seferinde açıkça seçilir |
| D6 | Tekrarlayan planda kapsam | Planda bir kez seçilir; gerçekleşen kayıt plandan alır (mevcut kural) |
| D7 | Kategori değişince kapsam | Düzenleme yok (iptal + yeni). Önceden doldurulan formda kategori değişirse kapsam yeniden seçilir |

## Uygulama planı (sonraki oturum, commit'lerden sonra)

1. **Ortak bileşen** `lib/core/widgets/app_category_field.dart` (`AppCategoryField`):
   kapalı alan `AppAccountField` ile aynı 56'lık `SelVal`; menü formun **içinde**
   açılır (D1). Bayraklar: `fixedScope` (K1), kapsam görünmüyorsa düz liste
   (K2, `ScopeController.isVisible`), arama ve son kullanılanlar yalnız 8'den
   fazla kategoride. Değer `kategori + kapsam` çiftidir.
2. **Formun kayması ve alttakilerin gizlenmesi:** alan açılınca kendini kaydırılan
   gövdenin başına getirir (`Scrollable.ensureVisible`), form "açık alan" bilgisini
   alır ve alttaki alanları çizmez; kapanınca eski kaydırma konumuna döner. Alttan
   açılan panelde (POS) referans panelin başlığıdır.
3. **Simgeler:** varsayılan kategori adlarından simge tablosu
   (`lib/core/localization/default_category_labels.dart` adları); tutmazsa
   `local_offer`.
4. **Son kullanılanlar (sunucu, D2/D3):** `RecognizedItems` (gün, tür, kapsam,
   kategori, `EntryAtUtc`) üstünden `(kategori, kapsam)` çiftine göre en yeni 4;
   yalnız kullanıcının formda kategori seçtiği kaynaklar sayılmalı (POS komisyonu,
   yatış kesintisi, gün sonu gibi kendiliğinden yazılanlar değil). Uç:
   `GET /api/v1/categories/recent?type=` (ya da `/categories` cevabına alan).
   `documentation/permissions.md` ve sözleşme belgesi güncellenir.
5. **`+ ekle` (D4):** `AccountCreatePage` gibi `CategoryCreatePage` ve rota
   (`/more/categories/new?name=&type=`), `addCategoryFromForm` → `BudgetCategory?`.
6. **Bağlanacak alanlar** (eski `DropdownButtonFormField`):
   - Ana tasarım: `activities/quick_add_form_page.dart` (Gelir/Gider; kategori +
     `AppScopeSection` çipi kalkar), `budgets/budgets_page.dart` (`Gider kategorisi`
     + çip), `cards/finance_page.dart` (kart harcaması ve taksit planındaki çipler),
     `planning/planning_page.dart` (+ çip), `debts/debts_page.dart` (+ çip),
     `taxes/tax_plan_form_page.dart`, `taxes/tax_sheets.dart`,
     `cash/cash_count_view.dart` (kasa farkı `Gider`), `data_tools/data_tools_page.dart`
     (ekstre eşleme).
   - K1 `İşletme`: `pos/pos_settlements_view.dart` (Satış + Komisyon),
     `pos/pos_definitions_page.dart` (Satış + Komisyon), `pos/card_collection_fields.dart`
     (Komisyon), `pos/pos_deposit_sheets.dart` (Kesinti, iki yer),
     `day_close/day_close_sheets.dart` (Satış), `counterparties/counterparty_forms.dart`.
   - K1 `Şahsi`: `cash/cash_withdrawal_sheet.dart` (`Kendime aldım`).
   - Dokunulmaz: `obligations/` (kalkacak), `transactions/transactions_page.dart`
     (eski sayfa, açılmıyor), `transfer_page.dart` çipi (masrafın kapsamı, kategori değil).
7. **POS paneli farkları** F1–F4 (açıklama, sıra, komisyon 48, ok arası 6).
8. Testler: `test/support/account_field_helpers.dart` gibi kategori yardımcıları;
   kapsam çipine bakan testler (`quick_add_scope_test.dart` vb.) menü alt satırına
   geçer. Kareler: `test/screenshots/` altında teslim karelerinin karşılıkları.
9. Belgeler: `documentation/design-system.md`, `flows.md`, aşama belgesi Sapmalar.

## Teslimde olmayan, teslimin diliyle kurulacak

- Kategori seçili, kapsam boş (fişten öneri) ve `Kaydet`te hata hâli.
- Hiç kategori yok (H7 dili + `Kategori ekle`).
- K1 kapsamı sabit ekranlar: POS tahsilatı, POS ekle, kartla tahsil, yatış kesintisi,
  gün sonu, cari borçlandırma (İşletme); `Kendime aldım` (Şahsi). README notuna göre:
  yalnız o kapsamın kategorileri + "İşletme veya şahsi" grubu, o grupta ok ve alt
  satır yok.
- Bütçe, plan, taksit, borç, vergi, kasa farkı, banka ekstresi: ana tasarım.
- Panel içinde (alttan açılan POS paneli) D1-A'nın "başlığın altına kayma"sı:
  panelin başlığı referans alınır.
- K3 (süzgeç) bu işte kullanılmaz; İşlemler filtresi tasarlanırken.
