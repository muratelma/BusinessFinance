# Kategori seçici + POS tahsilatı · teslim

Tasarımların hepsi tek canvasta: `Kategori ve POS.html` (P satırı y=2900, K/Z satırı y=4140, POS + K2 satırı y=5240).
Kod: `kod/PosSec.jsx` (POS paneli), `kod/KatSec.jsx` (kategori seçici), `kod/HesapSec.jsx` (ortak `HField`, `SelVal`, `HAccVal`).
Ekran görüntüleri: `screenshots/` (412 × 892, uygulamanın 411 dp genişliği). Tek ekranı yeniden üretmek için: `_shot.html?id=K4o`.

Panel ve sayfa iskeleti (başlık, alt çubuk, zemin, kart dili) bugünkü uygulamadaki gibi kalır. Yalnız aşağıdakiler değişir.

---

## A. Ortak: form alanı (`HField`) ve seçili değer (`SelVal`)

Bütün formlarda (Gelir, Gider, POS) geçerli.

| Ölçü | Değer |
|---|---|
| Alan yüksekliği | **56 sabit**; içerik iki satır olsa da büyümez |
| Kenarlık | 1 px `--border-strong`; odakta 2 px `--brand` |
| Yarıçap | 16 |
| İç boşluk | 8 · 12 · 8 · 16 (odakta 7 · 11 · 7 · 15, içerik kaymaz) |
| Kenardaki etiket | 12/16, `--ink-muted` (odakta `--ink`); sol 12, üst −8, yatay 4 zemin boşluğu |
| **Değer ile sağdaki ok arası** | **6 px** (önceden 12). Tutar sağa yaslı, büyüdükçe sola uzar, sağ kenarı sabit |
| Alanlar arası | 16 |

`SelVal` (Hesap, POS, Elle gir, Kategori değeri):

| Parça | Ölçü |
|---|---|
| Simge kapsülü | 32 × 32, tam yuvarlak, zemin `--surface-card-muted`, simge 18 |
| Ad | 600 · 15/20, tek satır, `…` ile kısalır |
| Alt yazı | 400 · 13/16, `--ink-muted` (ad + alt yazı = 36 px) |
| Tutar (yalnız hesap) | 500 · 14, `--ink-muted`, sabit genişlikli rakam, kısalmaz |
| Simge–metin | 12 |

2.0× yazı ölçeğinde 56 korunamaz: yalnız o durumda `minHeight: 56`, ad kısalır, tutar kısalmaz.

---

## B. POS tahsilatı paneli

Ekranlar: `01`–`05`.

**Başlık altı metin (tek satır):** *Gelir bugün yazılır, para hesaba sonra geçer.*  (önceki iki satırlık metin kalktı)

### POS alanı ve menüsü
- Değer `SelVal`: POS → `point_of_sale` mavi (`--neutral`) · **Ziraat POS** · *POS · Ziraat vadesiz*. Elle gir → `edit` · **Elle gir** (alt yazı yok).
- POS ve hesap alanının **altında yardımcı metin yok**.
- Menü (`02`, `04`): alanın 8 px altında, alanla aynı genişlik; beyaz, yarıçap 16, kenarlıksız, `--elevation-floating`; 150 ms açılış. Başlık / ara yazı / çizgi yok.
  - Satır 56, yatay 16, simge 20 (kapsülsüz), ad 15/1.3 (seçili 600), alt yazı 13 `--ink-muted`.
  - POS satırı alt yazısı = bağlı hesap (*Ziraat vadesiz*). Elle gir alt yazısı: *POS seçmeden; hesabı siz seçersiniz*.
  - Seçili satır: zemin `--surface-card-muted` + solda 4 px `--brand` çubuk (üst/alt 8 içeride).

### Form sırası
**POS seçili (`01`):** POS · Müşterinin ödediği (+ *Brüt tutar: kestiğiniz fatura budur.*) · Tahsilat günü · özet panel · Açıklama.
Özet panel salt okunur: Komisyon (%oran) · Hesaba geçecek · Beklenen gün · Geçeceği hesap. Zemin `--surface-card-muted`, yarıçap 16, iç 4 · 16, satır 44, satır arası 1 px `--border`; sol 14 `--ink-muted`, sağ 14/600. Değer yoksa "—".

**Elle gir (`03`, `05`):** POS · Paranın geçeceği hesap · **Satış kategorisi** · Müşterinin ödediği · Tahsilat günü · Paranın beklendiği gün · Komisyon · Açıklama.
(Satış kategorisi tutarın **üstünde**.) POS ↔ Elle gir geçişinde yazılmış tutar ve tarih korunur.

### Komisyon (`03`)
| Ölçü | Değer |
|---|---|
| Yükseklik | **48** (Vazgeç / Tahsilatı kaydet ile aynı) |
| Genişlik | tam genişlik, üç eşit parça: Yok · Tutar · Oran |
| Kenarlık / yarıçap | 1 px `--border-strong`, tam yuvarlak; parçalar arası 1 px çizgi |
| Yazı | 14; seçili 600, diğerleri 500 |
| Seçili | dolgu `--brand`, yazı `--on-brand`. **✓ yok** → parça genişlemez, yazı kaymaz |
| Etiket | üstte "Komisyon" 13 `--ink-muted`, 8 boşluk |
| Tutar / Oran seçilince | altında 8 boşlukla 56 px alan: "Komisyon tutarı" / "Komisyon oranı (%)" |

### Alt düğmeler
Sabit alt şerit, üstte 1 px `--border`; sağa yaslı Vazgeç (metin) + Tahsilatı kaydet (dolgu), ikisi 48.

---

## C. Kategori seçici (Gelir ekle, Gider ekle, POS Elle gir)

Ekranlar: `06`–`17`. **Tek tasarım**; gelir/gider farkı yalnız kategori sayısından gelir.

Form sırası her formda: **Hesap · Kategori · Tutar · Tarih · Ad**.

### Açılış davranışı
1. Kategori alanına dokununca form kayar, alan **sayfa başlığının hemen altına** gelir (alan formun kaçıncı sırasında olursa olsun aynı görünüm).
2. Menü alanın 8 px altında açılır; alana bağlı, alanla aynı genişlik, karartma / panel yok.
3. Menü içerik kadar uzar; **en fazla** alt çubuğun 16 px üstüne, klavye açıkken klavyenin 8 px üstüne kadar. Liste menünün içinde kayar, kaydırma çubuğu görünmez.
4. Menü açıkken alanın **altındaki alanlar ve Kaydet gizlenir** (yarım görünen alan kalmaz).
5. Satıra dokun = seç, menüyü kapat, form eski yerine döner. Seçmeden kapatma: alana tekrar dokunmak ya da geri tuşu.
6. Bu bir seçim, filtre değil: Uygula / temizle / çoklu seçim yok.

### Menü içeriği
| Parça | Ölçü |
|---|---|
| Arama | **yalnız 8'den fazla kategori varsa**. 44 yükseklik, kenarlardan 8, yarıçap 12, zemin `--surface-card-muted`; simge 20, yazı 15, yer tutucu *Kategori ara*; odakta 2 px `--brand`; yazı varken sağda × |
| Son kullanılanlar | yalnız 8'den fazla kategori varsa ve arama boşken; en çok 4; satır sağında kapsam (13 `--ink-muted`) |
| Gruplar | **İşletme · Şahsi · İşletme veya şahsi**; boş grup gösterilmez |
| Grup başlığı | 600 · 12, harf aralığı .04em, `--ink-faint`; iç 12 · 16 · 4; gruplar arası 1 px `--border` |
| Satır | 48, yatay 16, simge 20 `--ink-muted`, ad 15/1.3, tek satır |
| Seçili | zemin `--surface-card-muted` + solda 4 px `--brand` çubuk, ad 600. Son kullanılanlarda işaretlenmez, kendi grubunda işaretlenir |
| Arama eşleşmesi | eşleşen harfler 700 |

Gelirde kategori az olduğu için arama ve Son kullanılanlar kendiliğinden yok (`10`).

### Eşleşme yok (`09`)
H7 ("Henüz hesap yok") dili: solda `search_off` 20 · **Bu adla kategori yok** (600 · 15) · *Aradığınız adla yeni kategori ekleyebilirsiniz.* (14 `--ink-muted`) · metin düğmesi **+ "kırtasiye" ekle** (40, 600 · 14).

### Kapsam · Z5
- **Kapsamlı kategori (`11`):** başka hiçbir şey çıkmaz. Alanın alt yazısı kapsamı söyler: *Hizmet geliri · İşletme*.
- **Kapsamsız kategori (İşletme veya şahsi):** satırın sağında `expand_more`. Dokununca satır kalınlaşır, ok `expand_less` olur, altında iki alt satır açılır: **İşletme** (`storefront`) ve **Şahsi** (`person`); alt satır 48, sol iç 48, simge 20, yazı 15 (`12`, `14`).
- Alt satıra dokunmak kategoriyi **ve** kapsamı seçer, menü kapanır. Alanda: *Faiz geliri · İşletme* (`13`, `15`). Formda ayrı kapsam çubuğu **yok**. Değiştirmek için alana tekrar dokunulur.

### Ek görünümler (aynı bileşen, bayrakla)
- **Kapsamı sabit ekran (POS tahsilatı, gün sonu)** — ayrı ekran çizilmedi: menüde yalnız ekranın kapsamındaki kategoriler + İşletme veya şahsi grubu. O gruptaki satırda ok ve alt satır yok, tek dokunuşla seçilir. Kapalı alanda alt yazı ekranın kapsamı (`05`: *Satış geliri · İşletme*).
- **"İşletmem yok" kullanıcı (`16`, `17`)** — düz liste: grup başlığı, alt satır, ok yok. Kapalı alanda yalnız kategori adı, alt yazı yok.

---

## D. Dikkat edilecekler

- **Tarih biçimi:** uygulamada `2026-10-28` görünüyor → `28 Ekim 2026` (`DateText`).
- **Tutar:** yazarken ₺ ve binlik ayırıcı (`editableMoney`); özet paneldeki Komisyon ve Hesaba geçecek sunucudan gelir, istemci hesaplamaz; tutar yokken "—".
- **Klavye:** arama odaklanınca menünün alt kenarı klavyenin 8 px üstüne çekilir; liste kısalır, arama yerinde kalır (`08`).
- **Kaydırma geri dönüşü:** menü kapanınca form, açılmadan önceki kaydırma konumuna döner.
- **Hiç POS yoksa** alan doğrudan *Elle gir*; menüde yalnız Elle gir.
- **Kategori adları ve simgeleri örnek.** Gerçek liste sunucudan; simge kanonik addan türetilir, tutmazsa `local_offer`.
- **Açık konular (Claude Code ile):** düzenlemede kategori değişince eski kapsam korunur mu; hesabın kapsamı varsayılan önerilir mi; tekrarlayan kayıtta kapsam her seferinde mi sorulur.
- `design_handoff_hesap/` içindeki H1b ölçüleri bu dosyadaki `SelVal` ile güncellenmeli (ad 16 → 15/20, alt yazı 13/1.35 → 13/16, değer–ok arası 12 → 6).

## Ekran görüntüleri

| Dosya | Ne gösterir |
|---|---|
| 01-p1-pos-secili | POS seçili, özet panel |
| 02-p1o-pos-menu | POS menüsü açık |
| 03-p1e-elle-gir | Elle gir formu, komisyon 48 px |
| 04-p1eo-elle-gir-menu | Elle gir seçili menü |
| 05-pos-elle-gir-kategori-secildi | Elle gir, Satış kategorisi seçilmiş (tutarın üstünde) |
| 06-k4-gider-kapali | Gider ekle, alan sırası |
| 07-k4o-kategori-menu-acik | Form kaymış, menü açık, Son kullanılanlar |
| 08-k4s-arama-klavye | Arama + klavye |
| 09-k4n-eslesme-yok | Eşleşme yok |
| 10-g1-gelir-az-kategori | Gelir: arama ve Son kullanılanlar yok |
| 11-z0-kapsamli-secildi | Kapsamlı kategori seçildi |
| 12-z5-gelir-kapsam-menude | Kapsamsız kategori açıldı (gelir) |
| 13-z5b-gelir-secildi | Kapsam seçildi (gelir) |
| 14-z5g-gider-kapsam-menude | Kapsamsız kategori açıldı (gider, arama ile) |
| 15-z5gb-gider-secildi | Kapsam seçildi (gider) |
| 16-k2-isletmem-yok-acik | İşletmem yok, düz liste |
| 17-k2-isletmem-yok-secildi | İşletmem yok, seçildi |
